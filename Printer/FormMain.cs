using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Printer
{
    public partial class FormMain : Form
    {

        enum E_system_status
        {
            Idle,
            Feed,
            Stable,
            Ready,
            Printing,
            PrintPreEnd,
            PrintEnd,
            Error,
        };
        enum EPrintStep
        {
            PrintIdle,
            SendStartPoint,
            MoveDoneStartPoint,
            SendEndPoint
        }

        private int prevLength = 30;  //加速和减速的距离
        private DBHelper dbHelper = new DBHelper();
        //public SocketClient TcpPrinter = null;  //喷码机连接对象
        public SocketClient TcpPrinter = new SocketClient("192.168.1.120", 2000);  //喷码机连接对象

        public Socket TcpController = null; //控制器连接对象

        public HslCommunication.ModBus.ModbusTcpNet ModbusPlc = new HslCommunication.ModBus.ModbusTcpNet("192.168.1.10");  //与PLC的链接对象

        int currentDay = 0;
        int serialNo = 1;
        int serialNo2 = 1;
        int serialNo3 = 1;
        int serialNo4 = 1;
        int serialNo5 = 1;
        int serialNo6 = 1;
        int serialNo7 = 1;
        int serialNo8 = 1;



        private List<ProductData> impossiblePointList = new List<ProductData>();
        private List<route> routeList = new List<route>(); //用于保存本次打印所有的路径和需要发送的数据以及路径上需要打印的点
        private List<ProductData> resultList = new List<ProductData>();
        private List<string> logstring = new List<string>();


        float thisAngle = 0;


        float XPianyi = -173.23f;
        float YPianyi = 159.65f;
        float ZPinyi = -76.8f;

        bool bStartService=false;
        bool bReset;
        bool bStop;
        bool bPrintSensor;
        bool bAfterPrintSensor;
        bool bBeforePrintSensor;
        bool bBeforeSingal;
        bool bAfterSingal;
        bool bIsOpen;
        bool bCanOpen;
        bool bSendData=false;

        bool bIsSend = false;

        E_system_status eSystemStatus = E_system_status.Idle;
        EPrintStep ePrintStep = EPrintStep.PrintIdle;

        int nRouteIndex = 0;
        int nPrintLineIndex = 0;
        bool bStart = true;
        bool bMoveDone = false;

        DateTime preTime;
        DateTime laTime;

        private const string PrintPosCylinderAddress = "0";
        private const string MoveBeltAddress = "1";
        private const string RedLampAddress = "2";
        private const string GreenLampAddress = "3";
        private const string YellowLampAddress = "4";
        private const string AlarmBeepAddress = "5";
        private const string FeedProductAddress = "6";
        private List<int> qsTemplateNo = new List<int>() { 16, 17, 18, 19, 20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39 };

        private string statusMsg = string.Empty;
        private int delay100Ms = 0;

        public FormMain()
        {
            InitializeComponent();
            //var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");

            //var time2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            //var temp = printerHelper.SendDataToTemplate(new List<string>() { "12345", "abcde","ABCDE" });
            //IsCanConnect("172.168.0.136");
            //加载偏移量
            try
            {
                string pianyistr = ConfigurationManager.AppSettings["pianyiliang"];
                string[] pianyiarr = pianyistr.Split(',');
                XPianyi = Convert.ToSingle(pianyiarr[0]);
                YPianyi = Convert.ToSingle(pianyiarr[1]);
                ZPinyi = Convert.ToSingle(pianyiarr[2]);
                string open= ConfigurationManager.AppSettings["canOpen"];
                if(open=="0")
                {
                    bCanOpen = false;
                }
                string sendData = ConfigurationManager.AppSettings["senData"];
                {
                    if(sendData=="1")
                    {
                        bSendData = true;
                    }
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Common.GetSystemConfigFromXmlFile();

            //TcpPrinter = ConnectSocket("192.168.1.120", 2000);
            TcpPrinter.HandleRecMsg = new Action<byte[], SocketClient>((bytes, theClient) =>
            {
                //TXTLogHelper.LogBackup("喷码机收到消息:" + byteToHexString(bytes));  绑定接收消息
                if(bIsSend)
                {
                    if(bytes[0]==0xE7)
                    {
                        TcpPrinter.Send(printerHelper.SendDataToTemplate(sendDataList[sendNum].Split(',').ToList()));
                        sendNum++;
                    }
                }
            });

            TcpPrinter.StartClient();

            TcpPrinter.Send(printerHelper.sendStart());
            TcpController = ConnectSocket("192.168.1.30", 8088);

            ModbusPlc.ConnectServer();

            ////黄灯亮，显示设备准备状态
            bool[] arrData = { false, true, false, false };
            ModbusPlc.WriteCoil(RedLampAddress, arrData);

            //MoveToHome();

            var tempProductData = new ProductData
            {
                PosX = Convert.ToSingle(0),
                PosY = Convert.ToSingle(0),
                PosZ = 0f,
                Angle = Convert.ToSingle(0),
                Time = 0f,
                TemplateNo = Convert.ToInt32(1)
            };
            impossiblePointList.Add(tempProductData);

            currentDay = DateTime.Now.Day;
        }
        #region 路径计算
        public class route
        {
            public float Angle { get; set; }
            public float startX { get; set; }
            public float startY { get; set; }
            public float endX { get; set; }
            public float endY { get; set; }
            public string dataType { get; set; }
            public int tempNo { get; set; }
            public int TemplateNo { get; set; }
            public List<string> dataList { get; set; }

            public List<ProductData> printPointList { get; set; }
        }
        public class Point
        {
            public float X { get; set; }
            public float Y { get; set; }
        }
        public void PrintCal(List<SystemConfigProductData> dataList)
        {
            List<route> routeListTemp = new List<route>();
            //根据角度，计算每个点的b
            for (int i = 0; i < dataList.Count; i++)
            {
                double k = 0;
                double b = 0;
                if (dataList[i].Angle == "90")
                {
                    k = 0;
                    b = Convert.ToDouble(dataList[i].PosY);
                    thisAngle = 90f;
                }
                else
                {
                    k = 1;
                    b = Convert.ToDouble(dataList[i].PosX);

                }
                //double k = Math.Tan(Convert.ToInt32(dataList[i].Angle));
                //double b = Convert.ToDouble(dataList[i].PosX) - k * Convert.ToDouble(dataList[i].PosY);
                dataList[i].b = b;
                //dataList[i].k = k;
            }
            //根据角度分组
            IEnumerable<IGrouping<string, SystemConfigProductData>> queryAngle = dataList.GroupBy(x => x.Angle);//角度一样的分组
            foreach (IGrouping<string, SystemConfigProductData> info in queryAngle)
            {
                var sameAngleList = info.ToList();
                IEnumerable<IGrouping<double, SystemConfigProductData>> queryAngleAndB = sameAngleList.GroupBy(x => x.b);//角度和B一样分组
                foreach (IGrouping<double, SystemConfigProductData> info2 in queryAngleAndB)
                {
                    List<SystemConfigProductData> sameAngleAndBList = info2.ToList<SystemConfigProductData>();
                    IEnumerable<IGrouping<string, SystemConfigProductData>> queryAngleAndBAndType = sameAngleAndBList.GroupBy(x => x.TemplateNo);//角度和B和模板一样的分组
                    int no = 1;
                    foreach (IGrouping<string, SystemConfigProductData> info3 in queryAngleAndBAndType)
                    {
                        List<SystemConfigProductData> sameAngleAndBAndTypeList = info3.ToList<SystemConfigProductData>().OrderByDescending(x => Convert.ToSingle(x.PosX)).OrderBy(x => Convert.ToSingle(x.PosY)).ToList();//角度和B和模板一样的list,并且根据x坐标排序
                        //计算路径的起始点和结束点
                        route routeTemp = new route();
                        routeTemp.dataType = sameAngleAndBAndTypeList[0].Type;
                        routeTemp.Angle = Convert.ToSingle(sameAngleAndBAndTypeList[0].Angle);
                        if (routeTemp.Angle == 90f)
                        {
                            routeTemp.startX = Convert.ToSingle(Convert.ToSingle(sameAngleAndBAndTypeList[0].PosX) + prevLength);
                            routeTemp.startY = Convert.ToSingle(Convert.ToSingle(sameAngleAndBAndTypeList[0].PosY));
                            routeTemp.endX = Convert.ToSingle(Convert.ToSingle(sameAngleAndBAndTypeList[sameAngleAndBAndTypeList.Count - 1].PosX) - prevLength);
                            routeTemp.endY = Convert.ToSingle(Convert.ToSingle(sameAngleAndBAndTypeList[sameAngleAndBAndTypeList.Count - 1].PosY));
                        }
                        else
                        {
                            routeTemp.startX = Convert.ToSingle(Convert.ToSingle(sameAngleAndBAndTypeList[0].PosX));
                            routeTemp.startY = Convert.ToSingle(Convert.ToSingle(sameAngleAndBAndTypeList[0].PosY) - prevLength);
                            routeTemp.endX = Convert.ToSingle(Convert.ToSingle(sameAngleAndBAndTypeList[sameAngleAndBAndTypeList.Count - 1].PosX));
                            routeTemp.endY = Convert.ToSingle(Convert.ToSingle(sameAngleAndBAndTypeList[sameAngleAndBAndTypeList.Count - 1].PosY) + prevLength);
                        }
                        //routeTemp.startX = Convert.ToSingle(Convert.ToSingle(sameAngleAndBAndTypeList[0].PosX) - Math.Sin(Convert.ToSingle(sameAngleAndBAndTypeList[0].Angle) ) * prevLength);
                        //routeTemp.startY = Convert.ToSingle(Convert.ToSingle(sameAngleAndBAndTypeList[0].PosY) - Math.Cos(Convert.ToSingle(sameAngleAndBAndTypeList[0].Angle) ) * prevLength);
                        //routeTemp.endX = Convert.ToSingle(Convert.ToSingle(sameAngleAndBAndTypeList[sameAngleAndBAndTypeList.Count - 1].PosX) + Math.Sin(Convert.ToSingle(sameAngleAndBAndTypeList[sameAngleAndBAndTypeList.Count - 1].Angle) ) * prevLength);
                        //routeTemp.endY = Convert.ToSingle(Convert.ToSingle(sameAngleAndBAndTypeList[sameAngleAndBAndTypeList.Count - 1].PosY) + Math.Cos(Convert.ToSingle(sameAngleAndBAndTypeList[sameAngleAndBAndTypeList.Count - 1].Angle) ) * prevLength);
                        routeTemp.tempNo = no;
                        no = no + 1;
                        routeTemp.TemplateNo = Convert.ToInt32(sameAngleAndBAndTypeList[0].TemplateNo);
                        List<string> routedataList = new List<string>();
                        List<ProductData> printPointList = new List<ProductData>();
                        foreach (SystemConfigProductData de in sameAngleAndBAndTypeList)
                        {
                            routedataList.Add(de.Text);
                            ProductData temp = new ProductData();
                            temp.PosX = Convert.ToSingle(de.PosX);
                            temp.PosY = Convert.ToSingle(de.PosY);
                            temp.PosZ = Convert.ToSingle(de.Angle);
                            temp.Angle = Convert.ToSingle(de.Angle);
                            temp.Time = 0f;
                            temp.TemplateNo = Convert.ToInt32(de.TemplateNo);
                            printPointList.Add(temp);
                        }
                        routeTemp.dataList = routedataList;
                        routeTemp.printPointList = printPointList;
                        routeListTemp.Add(routeTemp);
                    }
                }
            }

            IEnumerable<IGrouping<float, route>> sameAngeleRoyteList = routeListTemp.GroupBy(x => x.Angle);//角度一样的分组
            routeList.Clear();
            foreach (IGrouping<float, route> info in sameAngeleRoyteList)
            {

                if (info.Key == 0f)
                {
                    List<route> shuList = info.ToList<route>().OrderByDescending(x => x.startX).ToList();//
                    routeList.AddRange(shuList);
                }
                if (info.Key == 90f)
                {
                    List<route> hengList = info.ToList<route>().OrderBy(x => x.startY).ToList();//
                    routeList.AddRange(hengList);
                }

            }

            //routeList = routeList.OrderByDescending(x => x.startX).ToList();

            if (routeList.Count > 0)
            {
                #region 对routeList进行最短路径规划
                //List<route> shortestList = new List<route>() { routeList[0]};
                //List<route> temp = routeList.FindAll(x=>x.tempNo!=1);
                //while (temp.Count > 0)
                //{
                //    float tempDistance = 0;
                //    int shortestIndex = 0;
                //    bool startPoint = true;
                //    for (int i = 1; i < temp.Count; i++)  //从第二条路径开始循环，计算每个点到第一条路径end点的距离
                //    {
                //        float startDis = dis(shortestList[i-1].endX, shortestList[i-1].endY, temp[i].startX, temp[i].startY);
                //        float endDis = dis(shortestList[i-1].endX, shortestList[i-1].endY, temp[i].endX, temp[i].endY);
                //        if(startDis>endDis)
                //        {
                //            startPoint = false;
                //        }
                //        if(i==1)
                //        {
                //            tempDistance = startDis > endDis ? endDis : startDis;
                //            shortestIndex = temp[i].tempNo;
                //        }
                //        else
                //        {
                //            float smallOne = startDis > endDis ? endDis : startDis;
                //            if(smallOne<tempDistance)
                //            {
                //                tempDistance = smallOne;
                //                shortestIndex = temp[i].tempNo;
                //            }
                //        }
                //    }
                //    route nextRoute = temp.Find(x => x.tempNo == shortestIndex);

                //    route routeAdd = TransReflection<route, route>(nextRoute);
                //    if(startPoint==false)
                //    {

                //        routeAdd.startX = nextRoute.endX;
                //        routeAdd.startY = nextRoute.endY;
                //        routeAdd.endX = nextRoute.startX;
                //        routeAdd.endY = nextRoute.startY;

                //    }
                //    shortestList.Add(routeAdd);
                //    temp.Remove(nextRoute);
                //}
                //routeList = shortestList;
                #endregion


                this.richTextBoxMsg.Text = "";
                //this.richTextBox1.Text=
                for (int i = 0; i < routeList.Count; i++)
                {
                    this.richTextBoxMsg.Text += "StartX:" + (routeList[i].startX) + "StartY:" + (routeList[i].startY) + " endX:" + (routeList[i].endX) + "endY:" + (routeList[i].endY) + "\r\n";
                }
            }
        }
        #endregion
       

        #region socket收发相关
        private Socket ConnectSocket(string server, int port)
        {
            Socket s = null;
            //IPHostEntry hostEntry = null;

            IPAddress addr = IPAddress.Parse(server);

            IPEndPoint endp = new IPEndPoint(addr, port);
            Socket tempSocket =
                    new Socket(endp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            tempSocket.Connect(endp);

            if (tempSocket.Connected)
            {
                s = tempSocket;
                //break;
            }
            TXTLogHelper.LogBackup(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " " + "socket连接成功");
            return s;
        }

        private List<Byte> SocketSendReceive(Socket s, Byte[] bytesSent)
        {
            TXTLogHelper.LogBackup("发送给喷码机: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " " + byteToHexString(bytesSent));
            s.Send(bytesSent, bytesSent.Length, 0);
           
            int bytes = 0;
            List<Byte> result = new List<Byte>();
            Byte[] bytesReceived = new Byte[256];
            bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
           
            if (bytesReceived[0] != 0x06)
            {
                SocketSendReceive(s, bytesSent);
            }
            else
            {
                if (bytes == 1)
                {
                    bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                    for (int i = 0; i < bytes; i++)
                    {
                        result.Add(bytesReceived[i]);
                    }
                }
                else
                {
                    for (int i = 1; i < bytes; i++)
                    {
                        result.Add(bytesReceived[i]);
                    }
                }
            }
            TXTLogHelper.LogBackup(DateTime.Now.ToString("喷码机接收: " + "yyyy-MM-dd HH:mm:ss fff") + " " + byteToHexString2(result));
            return result;
        }
        private List<Byte> ControlSocketSendReceive(Socket s, Byte[] bytesSent)
        {
            TXTLogHelper.LogBackup("发送给控制器:  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " " + byteToHexString(bytesSent));
            s.Send(bytesSent, bytesSent.Length, 0);
           
            // Receive the server home page content.
            int bytes = 0;
            List<Byte> result = new List<Byte>();
            Byte[] bytesReceived = new Byte[256];

            //Thread.Sleep(100);

            bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);

            for (int i = 0; i < bytes; i++)
            {
                result.Add(bytesReceived[i]);
            }
            TXTLogHelper.LogBackup("控制器接收:  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " " + byteToHexString2(result));
            return result;
        }
        private void SelectTemplate(Socket s, Byte[] bytesSent)
        {
            s.Send(bytesSent, bytesSent.Length, 0);
            TXTLogHelper.LogBackup("发送给喷码机:  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " " + byteToHexString(bytesSent));
            int bytes = 0;
            List<Byte> result = new List<Byte>();
            Byte[] bytesReceived = new Byte[256];
            bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
            TXTLogHelper.LogBackup("喷码机接收:  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " " + byteToHexString3(bytesReceived[0]));
            if (bytesReceived[0] != 0x06)
            {
                SocketSendReceive(s, bytesSent);
            }
            //bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
            //for (int i = 0; i < bytes; i++)
            //{
            //    result.Add(bytesReceived[i]);
            //}
            //return result;
        }
        private void SendOutVar(Socket s, Byte[] bytesSent)
        {
            TXTLogHelper.LogBackup("发送给喷码机:  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " " + byteToHexString(bytesSent));
            s.Send(bytesSent, bytesSent.Length, 0);
           
            // Receive the server home page content.
            int bytes = 0;
            List<Byte> result = new List<Byte>();
            Byte[] bytesReceived = new Byte[256];
            bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
            //TXTLogHelper.LogBackup("喷码机接收:  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff") + " " + byteToHexString3(bytesReceived[0]));
            //if (bytesReceived[0] != 0x06)
            //{
            //    SocketSendReceive(s, bytesSent);
            //}
            //bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
            //for (int i = 0; i < bytes; i++)
            //{
            //    result.Add(bytesReceived[i]);
            //}
            //return result;
        }

        #endregion
        #region  界面按钮功能，数据上传数据库功能等
        private void 启动服务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(bStartService)
            {
                MessageBox.Show("服务已启动");
                return;
            }
            bStartService = true;
             //TcpController.Send(controlHelper.PackageModbusTcpFrame10(new byte[] { 0x00, 0x0e }, new byte[] { 0x01, 0x00 }));

            ControlSocketSendReceive(TcpController, controlHelper.PackageModbusTcpFrame10(new byte[] { 0x00, 0x0e }, new byte[] { 0x01, 0x00 }));
            Thread.Sleep(50);
            List<byte> returnlist = ControlSocketSendReceive(TcpController, controlHelper.readData());
            analyPosition(returnlist);
            //TcpController.Send(controlHelper.readData());

            //startService = true;


            Task.Factory.StartNew(() =>
            {
                CircleMain();
            });
        }
        private void readPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Byte> bytes = ControlSocketSendReceive(TcpController, controlHelper.readData());
            analyPosition(bytes);
            //TcpController.Send(controlHelper.readData());
            //var result = new byte[TcpController._sk.Offset];
            //Array.Copy(TcpController._sk.RecBuffer, 0, result, 0, TcpController._sk.Offset);
            //if (result.Length >= 9)
            //{
            //    if (result[0] == 0x00 && result[1] == 0x01 && result[2] == 0x00 && result[3] == 0x00 && result[4] == 0x00 && result[5] == 0x1d && result[6] == 0x01 && result[7] == 0x03 && result[8] == 0x00)
            //    {
            //        var res = analyPosition(result); //次数暂时注销，看看效果
            //    }
            //}
        }
        private void 移动到指定点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormInputPosition frmSelectProduct = new FormInputPosition();
            string result = "";
            //List<ProductData> resultList = new List<ProductData>();
            if (frmSelectProduct.ShowDialog() == DialogResult.OK)
            {
                labelTextProduct.Text = frmSelectProduct.textBox1.Text;
                List<byte> routePosList = new List<byte>();
                routePosList.Add(0x02);
                routePosList.Add(0x00);
                routePosList.Add(0x00);
                routePosList.Add(0x00);

                byte[] PosX = BitConverter.GetBytes(250f);  //sudu  
                byte[] PosY = BitConverter.GetBytes(0f);   //空位
                byte[] PosZ = BitConverter.GetBytes(Convert.ToSingle(frmSelectProduct.textBox1.Text));   //y
                byte[] Angle = BitConverter.GetBytes(Convert.ToSingle(frmSelectProduct.textBox2.Text));   //z
                byte[] Angle1 = BitConverter.GetBytes(Convert.ToSingle(frmSelectProduct.textBox3.Text));   //x
                byte[] Angle2 = BitConverter.GetBytes(0f);
                routePosList.AddRange(PosX.ToList());
                routePosList.AddRange(PosY.ToList());
                routePosList.AddRange(PosZ.ToList());
                routePosList.AddRange(Angle.ToList());
                routePosList.AddRange(Angle1.ToList());
                routePosList.AddRange(Angle2.ToList());
                ControlSocketSendReceive(TcpController, controlHelper.PackageModbusTcpFrame10(new byte[] { 0x22, 0xc0 }, routePosList.ToArray()));
                Thread.Sleep(50);
                ControlSocketSendReceive(TcpController, controlHelper.PackageModbusTcpFrame10(new byte[] { 0x00, 0x69 }, new byte[] { 0x01, 0x00 }));
                //TcpController.Send(controlHelper.PackageModbusTcpFrame10(new byte[] { 0x22, 0xc0 }, routePosList.ToArray()));
                //Thread.Sleep(50);
                //TcpController.Send(controlHelper.PackageModbusTcpFrame10(new byte[] { 0x00, 0x69 }, new byte[] { 0x01, 0x00 }));
            }
        }
        private void addProductToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormAddProduct frmAddProduct = new FormAddProduct();
            frmAddProduct.Show();
        }
        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void 模拟查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //GetStatusOfRtexController();
            //SystemConfig current = Common.SystemConfig;
            //List<SystemConfigProduct> plist = current.Products.ToList();
            //SystemConfigProduct product = plist.Find(x => x.Name == labelTextProduct.Text);
            //product.BaseCords.BaseCord1 = "99";
            //Common.SaveConfigToFile(current);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(bStartService)
            {
                MessageBox.Show("服务已启动");
                return;
            }
            bStartService = true;

            ControlSocketSendReceive(TcpController, controlHelper.PackageModbusTcpFrame10(new byte[] { 0x00, 0x0e }, new byte[] { 0x01, 0x00 }));
            //MoveToHome();
            //List<Byte> bytes = ControlSocketSendReceive(TcpController, controlHelper.readData());

            
            //analyPosition(bytes);
            Task.Factory.StartNew(() =>
            {
                CircleMain();
            });
        }
        //public void logssave()
        //{
        //    while (true)
        //    {
        //        Thread.Sleep(500);
        //        if(logstring.Count>0)
        //        {

        //        }
                    
        //    }
        //}
        public string byteToHexString(byte[] bytes)
        {
            var hexString = string.Empty;

            if (bytes == null)
                return hexString;
            var strB = new StringBuilder();

            foreach (var t in bytes)
                strB.Append(t.ToString("X2"));
            hexString = strB.ToString();

            return hexString;
        }
        public string byteToHexString2(List<byte> bytes)
        {
            var hexString = string.Empty;

            if (bytes == null)
                return hexString;
            var strB = new StringBuilder();

            foreach (var t in bytes)
                strB.Append(t.ToString("X2"));
            hexString = strB.ToString();

            return hexString;
        }
        public string byteToHexString3(byte bytes)
        {
            var hexString = string.Empty;

            if (bytes == null)
                return hexString;
            var strB = new StringBuilder();

            strB.Append(bytes.ToString("X2"));
            hexString = strB.ToString();

            return hexString;
        }
        private void pianyiSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPIanYi frmSelectProduct = new FormPIanYi();
            frmSelectProduct.textBox1.Text = XPianyi.ToString();
            frmSelectProduct.textBox2.Text = YPianyi.ToString();
            frmSelectProduct.textBox3.Text = ZPinyi.ToString();
            string result = "";
            //List<ProductData> resultList = new List<ProductData>();
            if (frmSelectProduct.ShowDialog() == DialogResult.OK)
            {
                Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
               cfa.AppSettings.Settings["pianyiliang"].Value = frmSelectProduct.textBox1.Text+","+ frmSelectProduct.textBox2.Text+","+ frmSelectProduct.textBox3.Text;

              
                cfa.Save();
            }
        }
        private bool IsCanConnect(string url)
        {
            try
            {
                Ping objPingSender = new Ping();
                PingOptions objPinOptions = new PingOptions();
                objPinOptions.DontFragment = true;
                string data = "";
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                int intTimeout = 120;
                PingReply objPinReply = objPingSender.Send(url, intTimeout, buffer, objPinOptions);
                string strInfo = objPinReply.Status.ToString();
                if (strInfo == "Success")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void addProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAddProduct frmAddProduct = new FormAddProduct();
            frmAddProduct.Show();
        }
        private void selectProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSelectProduct frmSelectProduct = new FormSelectProduct();
            string result = "";
            List<ProductData> resultList = new List<ProductData>();
            if (frmSelectProduct.ShowDialog() == DialogResult.OK)
            {
                labelTextProduct.Text = frmSelectProduct.labelComboxProducts.Text;

                var product = Common.SystemConfig.Products.ToList().Find(t => t.Name == labelTextProduct.Text);

                if (product == null) return;


                labelTextProduct.textBox.Text = product.Name;
                labelTextSum.textBox.Text = product.BaseCords.BaseCord1;
                labelText1.textBox.Text = product.BaseCords.BaseCord2;
                labelText2.textBox.Text = product.BaseCords.BaseCord3;

                labelText4.textBox.Text = product.BaseCords.BaseCord4;
                labelText5.textBox.Text = product.BaseCords.BaseCord5;
                labelText6.textBox.Text = product.BaseCords.BaseCord6;
                labelText7.textBox.Text = product.BaseCords.BaseCord7;
                labelText8.textBox.Text = product.BaseCords.BaseCord8;

                serialNo = product.BaseCords.BaseCord1 == null ? 1 : Convert.ToInt32(product.BaseCords.BaseCord1);
                serialNo2 = product.BaseCords.BaseCord2 == null ? 1 : Convert.ToInt32(product.BaseCords.BaseCord2);
                serialNo3 = product.BaseCords.BaseCord3 == null ? 1 : Convert.ToInt32(product.BaseCords.BaseCord3);
                serialNo4 = product.BaseCords.BaseCord4 == null ? 1 : Convert.ToInt32(product.BaseCords.BaseCord4);
                serialNo5 = product.BaseCords.BaseCord5 == null ? 1 : Convert.ToInt32(product.BaseCords.BaseCord5);
                serialNo6 = product.BaseCords.BaseCord6 == null ? 1 : Convert.ToInt32(product.BaseCords.BaseCord6);
                serialNo7 = product.BaseCords.BaseCord7 == null ? 1 : Convert.ToInt32(product.BaseCords.BaseCord7);
                serialNo8 = product.BaseCords.BaseCord8 == null ? 1 : Convert.ToInt32(product.BaseCords.BaseCord8);

                dataGridView1.Rows.Clear();
                foreach (var d in product.Datas)
                {

                    var rowNum = dataGridView1.Rows.Add();
                    dataGridView1.Rows[rowNum].Cells["No"].Value = d.No;
                    dataGridView1.Rows[rowNum].Cells["Type"].Value = d.Type;
                    dataGridView1.Rows[rowNum].Cells["data"].Value = d.Text;
                    dataGridView1.Rows[rowNum].Cells["PosX"].Value = d.PosX;
                    dataGridView1.Rows[rowNum].Cells["PosY"].Value = d.PosY;
                    dataGridView1.Rows[rowNum].Cells["Angle"].Value = d.Angle;
                    dataGridView1.Rows[rowNum].Cells["TemplateNo"].Value = d.TemplateNo;
                    ProductData temp = new ProductData();
                    temp.PosX = Convert.ToSingle(d.PosX);
                    temp.PosY = Convert.ToSingle(d.PosY);
                    temp.PosZ = 0f;
                    temp.Angle = Convert.ToSingle(d.Angle);
                    temp.Time = 0f;
                    temp.TemplateNo = Convert.ToInt32(d.TemplateNo);


                    resultList.Add(temp);

                }
            }



            if (labelTextProduct.Text != "")
            {
                //清空list中的数据
                routeList.Clear();
                //_bStartPrint = true;
                var product = Common.SystemConfig.Products.ToList().Find(x => x.Name == labelTextProduct.Text);
                if (product == null) return;
                PrintCal(product.Datas.ToList());
            }

            pictureBox1.Refresh();
            MoveToHome();


        }
        private void testPrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAddProduct form = new FormAddProduct();
            form.isEdit = 1;
            form.productName = labelTextProduct.Text;
            form.Show();
        }
        public float dis(float x1, float y1, float x2, float y2)
        {
            double dx, dy;
            dx = x2 - x1;
            dy = y2 - y1;
            return Convert.ToSingle(Math.Sqrt(dx * dx + dy * dy));
        }
        private void viewDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBatchInstead form = new FormBatchInstead();

            form.productName = labelTextProduct.Text;
            form.Show();
        }

        /// <summary>
        /// 复制产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemConfig current = Common.SystemConfig;
            List<SystemConfigProduct> plist = current.Products.ToList();
            SystemConfigProduct product = plist.Find(x => x.Name == labelTextProduct.Text);
            SystemConfigProduct copyNew = TransReflection<SystemConfigProduct, SystemConfigProduct>(product);

            copyNew.Name = product.Name + "-copy";
            plist.Add(copyNew);
            current.Products = plist.ToArray();

            Common.SaveConfigToFile(current);
            Common.GetSystemConfigFromXmlFile();
            MessageBox.Show("复制成功，请重新打开程序");
        }
        /// <summary>
        /// 利用反射进行深拷贝
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="tIn"></param>
        /// <returns></returns>
        private static TOut TransReflection<TIn, TOut>(TIn tIn)
        {
            TOut tOut = Activator.CreateInstance<TOut>();
            var tInType = tIn.GetType();
            foreach (var itemOut in tOut.GetType().GetProperties())
            {
                var itemIn = tInType.GetProperty(itemOut.Name); ;
                if (itemIn != null)
                {
                    itemOut.SetValue(tOut, itemIn.GetValue(tIn));
                }
            }
            return tOut;
        }
        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //Graphics g = e.Graphics.DrawLine;
            for (int i = 0; i < routeList.Count; i++)
            {
                e.Graphics.DrawLine(Pens.Black, Math.Abs(routeList[i].startX), Math.Abs(routeList[i].startY), Math.Abs(routeList[i].endX), Math.Abs(routeList[i].endY));
            }
            //e.Graphics.DrawLine(Pens.Black, 10, 10, 100, 100);

        }
        //计算追溯号
        public string GenerateDate()
        {
            DateTime currentDateTime = DateTime.Now;
            string year = currentDateTime.Year.ToString().Substring(3, 1);
            string month = currentDateTime.Month.ToString();
            string day = currentDateTime.Day.ToString();

            switch (month)
            {
                case "1":
                    month = "1";
                    break;
                case "2":
                    month = "2";
                    break;
                case "3":
                    month = "3";
                    break;
                case "4":
                    month = "4";
                    break;
                case "5":
                    month = "5";
                    break;
                case "6":
                    month = "6";
                    break;
                case "7":
                    month = "7";
                    break;
                case "8":
                    month = "8";
                    break;
                case "9":
                    month = "9";
                    break;
                case "10":
                    month = "X";
                    break;
                case "11":
                    month = "Y";
                    break;
                case "12":
                    month = "Z";
                    break;
                default:
                    month = "";
                    break;
            }

            switch (day)
            {
                case "1":
                    day = "1";
                    break;
                case "2":
                    day = "2";
                    break;
                case "3":
                    day = "3";
                    break;
                case "4":
                    day = "4";
                    break;
                case "5":
                    day = "5";
                    break;
                case "6":
                    day = "6";
                    break;
                case "7":
                    day = "7";
                    break;
                case "8":
                    day = "8";
                    break;
                case "9":
                    day = "9";
                    break;
                case "10":
                    day = "A";
                    break;
                case "11":
                    day = "B";
                    break;
                case "12":
                    day = "C";
                    break;
                case "13":
                    day = "D";
                    break;
                case "14":
                    day = "E";
                    break;
                case "15":
                    day = "F";
                    break;
                case "16":
                    day = "G";
                    break;
                case "17":
                    day = "H";
                    break;
                case "18":
                    day = "J";
                    break;
                case "19":
                    day = "K";
                    break;
                case "20":
                    day = "L";
                    break;
                case "21":
                    day = "M";
                    break;
                case "22":
                    day = "N";
                    break;
                case "23":
                    day = "P";
                    break;
                case "24":
                    day = "Q";
                    break;
                case "25":
                    day = "R";
                    break;
                case "26":
                    day = "S";
                    break;
                case "27":
                    day = "T";
                    break;
                case "28":
                    day = "U";
                    break;
                case "29":
                    day = "V";
                    break;
                case "30":
                    day = "W";
                    break;
                case "31":
                    day = "X";
                    break;
                default:
                    day = "";
                    break;
            }

            return year + month + day + "Y";
        }
        #endregion







        #region 喷码相关
        public void CircleMain()
        {
            while (true)
            {
                Thread.Sleep(100);

                #region 超时管理定时器计数
                if (delay100Ms < int.MaxValue)
                {
                    delay100Ms++;
                }
                #endregion

                #region 数据采集及变量映射

                var result = ModbusPlc.ReadDiscrete("0", 8);
                if (!result.IsSuccess)
                {
                    eSystemStatus = E_system_status.Error;
                    continue;
                }
                bool[] mappingResult = result.Content;
                bReset = mappingResult[0];
                bStop = mappingResult[1];
                bBeforePrintSensor = mappingResult[2];
                bPrintSensor = mappingResult[3];
                bAfterPrintSensor = mappingResult[4];
                bBeforeSingal = mappingResult[5];
                bAfterSingal = mappingResult[6];
                bIsOpen = mappingResult[7];
                statusMsg = bIsOpen.ToString();
                #endregion

                #region 急停按钮
                if (bStop == false)
                {
                    eSystemStatus = E_system_status.Error;
                }
                if (bCanOpen == false)
                {
                    if (bIsOpen == false)
                    {
                        eSystemStatus = E_system_status.Error;
                        //statusMsg = "门没有关闭!";
                    }
                }
                #endregion
                #region 状态机

                switch (eSystemStatus)
                {
                    case E_system_status.Idle:
                        //开始前打印位置有产品，需要取出
                        if (bPrintSensor)
                        {
                            eSystemStatus = E_system_status.Error;
                            statusMsg = "开始前打印位置有产品，需要取出后才能运行!";
                        }
                        else
                        {
                            ModbusPlc.WriteCoil(FeedProductAddress, true);  //前道要板命令
                            eSystemStatus = E_system_status.Feed;
                        }
                        break;
                    case E_system_status.Feed:

                        if (bBeforePrintSensor)
                        {
                            ModbusPlc.WriteCoil(MoveBeltAddress, true);
                            ModbusPlc.WriteCoil(FeedProductAddress, false);
                            ModbusPlc.WriteCoil(PrintPosCylinderAddress, true);
                            eSystemStatus = E_system_status.Stable;
                            delay100Ms = 0;
                        }
                        break;
                    case E_system_status.Stable:
                        //5S后还没有运行到打印位置，可能卡板或者传感器失效
                        if (delay100Ms > 50)
                        {
                            eSystemStatus = E_system_status.Error;
                            statusMsg = "3S后还没有运行到打印位置，可能卡板或者传感器失效";
                        }
                        if (bPrintSensor)
                        {
                            eSystemStatus = E_system_status.Ready;
                            //正式喷码前触发一次喷印
                            //TcpPrinter.SendData(new byte[] { 0x94, 0x00, 0x00, 0x94 });
                            delay100Ms = 0;
                        }
                        break;
                    case E_system_status.Ready:
                        if (delay100Ms > 10)
                        {
                            ModbusPlc.WriteCoil(MoveBeltAddress, false);
                            eSystemStatus = E_system_status.Printing;
                            statusMsg = "开始打印......";
                            //TcpClient.SendData(_startPrint);

                        }
                        break;

                    case E_system_status.Printing:

                        if (nRouteIndex < routeList.Count)
                        {
                            if (ePrintStep == EPrintStep.PrintIdle)
                            {
                                SendPrintPoint(impossiblePointList, false);
                                MoveNextPoint(routeList[nRouteIndex].startX, routeList[nRouteIndex].startY, routeList[nRouteIndex].Angle);
                                ePrintStep = EPrintStep.SendStartPoint;
                                bMoveDone = false;
                            }
                            else if (ePrintStep == EPrintStep.SendStartPoint)
                            {
                                bMoveDone = GetStatusOfRtexController();
                                if (bMoveDone)
                                {
                                    ePrintStep = EPrintStep.MoveDoneStartPoint;
                                }
                            }
                            else if (ePrintStep == EPrintStep.MoveDoneStartPoint)
                            {
                                ePrintStep = EPrintStep.SendEndPoint;
                                SendPrinterData(routeList[nRouteIndex]);
                                SendPrintPoint(routeList[nRouteIndex].printPointList, true);
                                MoveNextPoint(routeList[nRouteIndex].endX, routeList[nRouteIndex].endY, routeList[nRouteIndex].Angle);
                                bMoveDone = false;
                            }
                            else if (ePrintStep == EPrintStep.SendEndPoint)
                            {
                                bMoveDone = GetStatusOfRtexController();
                                if (bMoveDone)
                                {
                                    bIsSend = false;  //移动到结束点了
                                    ePrintStep = EPrintStep.PrintIdle;
                                    nRouteIndex++;
                                }
                            }
                        }
                        else
                        {
                            eSystemStatus = E_system_status.PrintPreEnd;
                            nRouteIndex = 0;
                            //插入数据库
                            if (bSendData)
                            {
                                try
                                {
                                    if (IsCanConnect("192.168.0.136"))
                                    {
                                        string sql = " insert into smtprinterdata(line,productName,createDate) values('A线','" + this.labelTextProduct.textBox.Text + "',getdate()) ";
                                        dbHelper.Execute(sql);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    statusMsg = ex.ToString();
                                }
                            }
                        }
                        break;
                    case E_system_status.PrintPreEnd:
                        if (bAfterSingal)
                        {
                            MoveToHome();
                            ModbusPlc.WriteCoil(PrintPosCylinderAddress, false);  //气缸下降
                            ModbusPlc.WriteCoil(MoveBeltAddress, true);  //输送带启动
                            eSystemStatus = E_system_status.PrintEnd;
                            delay100Ms = 0;

                        }
                        break;
                    case E_system_status.PrintEnd:
                        //卡板或者传感器失效
                        if (delay100Ms > 50)
                        {
                            eSystemStatus = E_system_status.Error;
                            statusMsg = "输出卡板或者后端传感器失效";
                        }
                        if (!bAfterSingal)
                        {
                            ModbusPlc.WriteCoil(MoveBeltAddress, false);
                            eSystemStatus = E_system_status.Idle;
                            statusMsg = "打印完成，设备等待打印";
                        }
                        break;
                    case E_system_status.Error:
                        ModbusPlc.WriteCoil(RedLampAddress, true);
                        ModbusPlc.WriteCoil(GreenLampAddress, false);
                        ModbusPlc.WriteCoil(AlarmBeepAddress, true);
                        if (bReset)
                        {
                            delay100Ms = 0;
                            ModbusPlc.WriteCoil(RedLampAddress, false);
                            ModbusPlc.WriteCoil(GreenLampAddress, true);
                            ModbusPlc.WriteCoil(AlarmBeepAddress, false);
                            eSystemStatus = E_system_status.Idle;
                            statusMsg = "已复位";
                        }

                        break;
                    default:
                        break;
                }

                #endregion

                #region 显示状态

                Action<string> actionDisplayStatus = (msg) =>
                {
                    this.lblStatus.Text = msg;
                };
                Invoke(actionDisplayStatus, statusMsg + " " + ePrintStep.ToString());

                #endregion
            }
        }

        public bool GetStatusOfRtexController()
        {
            List<byte> returnList = ControlSocketSendReceive(TcpController, controlHelper.isReachPoint());
            if (returnList.Count > 10)
            {
                if (returnList[0] == 0x00 && returnList[1] == 0x01 && returnList[2] == 0x00 && returnList[3] == 0x00 && returnList[4] == 0x00 && returnList[5] == 0x05 && returnList[6] == 0x01 && returnList[7] == 0x03 && returnList[8] == 0x00 && returnList[9] == 0x00 && returnList[10] == 0x00)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            //TcpController.Send(controlHelper.isReachPoint());
        }
        public void SendPrintPoint(List<ProductData> printPointList, bool isPianyi)
        {
            //测试不触发喷码的情况下是否会死机
            //isPianyi = false;
            if (isPianyi)
            {
                ControlSocketSendReceive(TcpController, controlHelper.sendPrintPos(printPointList, XPianyi, YPianyi, ZPinyi));
                //TcpController.Send(controlHelper.sendPrintPos(printPointList, XPianyi, YPianyi, ZPinyi));
            }
            else
            {
                ControlSocketSendReceive(TcpController, controlHelper.sendPrintPos(printPointList, 0, 0, 0));
                //TcpController.Send(controlHelper.sendPrintPos(printPointList, 0, 0, 0));
            }
        }
        public void MoveNextPoint(float x, float y, float z)
        {
            float currentRouteSpeed = 250f;

            //if (Math.Abs(routeList[nRouteIndex].Angle - 90f) < 0.001)
            //{
            //    currentRouteSpeed = 230f;
            //}

            List<byte> routePosList = new List<byte> { 0x02, 0x00, 0x00, 0x00 };
            routePosList.AddRange(BitConverter.GetBytes(currentRouteSpeed));
            routePosList.AddRange(BitConverter.GetBytes(0f));
            routePosList.AddRange(BitConverter.GetBytes(x + XPianyi));
            routePosList.AddRange(BitConverter.GetBytes(y + YPianyi));
            routePosList.AddRange(BitConverter.GetBytes(z + ZPinyi));
            routePosList.AddRange(BitConverter.GetBytes(z));

            List<byte> results = new List<byte> { 0x00, 0x01, 0x00, 0x00 };
            results.AddRange(BitConverter.GetBytes(7 + 28));
            results.AddRange(new byte[] { 0x01, 0x10, 0x22, 0xC0 });
            results.AddRange(BitConverter.GetBytes(28));
            results.Add(0x00);
            results.AddRange(routePosList.ToArray());
            //controlHelper.sendInitPosData(new byte[] { 0x22, 0xc0 }, routePosList.ToArray())
            ControlSocketSendReceive(TcpController, controlHelper.PackageModbusTcpFrame10(new byte[] { 0x22, 0xc0 }, routePosList.ToArray()));
            //TcpController.Send(controlHelper.PackageModbusTcpFrame10(new byte[] { 0x22, 0xc0 }, routePosList.ToArray()));
            Thread.Sleep(50);
            ControlSocketSendReceive(TcpController, controlHelper.PackageModbusTcpFrame10(new byte[] { 0x00, 0x69 }, new byte[] { 0x01, 0x00 }));
            //TcpController.Send(controlHelper.PackageModbusTcpFrame10(new byte[] { 0x00, 0x69 }, new byte[] { 0x01, 0x00 }));
        }
        /// <summary>
        /// 打完一次以后移动到原点，节约下次的时间
        /// </summary>
        public void MoveToHome()
        {
            List<byte> routePosList = new List<byte>();
            routePosList.Add(0x02);
            routePosList.Add(0x00);
            routePosList.Add(0x00);
            routePosList.Add(0x00);

            byte[] speed = BitConverter.GetBytes(200f);
            byte[] empty = BitConverter.GetBytes(0f);

            byte[] StartPosX = BitConverter.GetBytes(XPianyi + 50); //y
            byte[] StartPosY = BitConverter.GetBytes(YPianyi - 50); //z
            byte[] StartPosZ = BitConverter.GetBytes(ZPinyi + thisAngle); //x
            byte[] angle = BitConverter.GetBytes(0f);



            routePosList.AddRange(speed.ToList());
            routePosList.AddRange(empty.ToList());

            routePosList.AddRange(StartPosX.ToList());
            routePosList.AddRange(StartPosY.ToList());
            routePosList.AddRange(StartPosZ.ToList());

            routePosList.AddRange(angle.ToList());

            ControlSocketSendReceive(TcpController, controlHelper.PackageModbusTcpFrame10(new byte[] { 0x22, 0xc0 }, routePosList.ToArray()));
            Thread.Sleep(50);
            ControlSocketSendReceive(TcpController, controlHelper.PackageModbusTcpFrame10(new byte[] { 0x00, 0x69 }, new byte[] { 0x01, 0x00 }));

            //TcpController.Send(controlHelper.PackageModbusTcpFrame10(new byte[] { 0x22, 0xc0 }, routePosList.ToArray()));
            //Thread.Sleep(50);
            //TcpController.Send(controlHelper.PackageModbusTcpFrame10(new byte[] { 0x00, 0x69 }, new byte[] { 0x01, 0x00 }));
        }
        List<string> sendDataList = new List<string>();
        int sendNum = 0;
        public void SendPrinterData(route routeEntity)
        {
            SystemConfig current = Common.SystemConfig;
            List<SystemConfigProduct> plist = current.Products.ToList();
            SystemConfigProduct product = plist.Find(x => x.Name == labelTextProduct.Text);
            string chaseNo = GenerateDate();

            Action<string> actionDisplayChaseNo = (msg) =>
            {
                this.labelText3.textBox.Text = msg;
            };
            Invoke(actionDisplayChaseNo, chaseNo);

            sendDataList.Clear();
            

            //发送给喷码机的计数器初始值
            int currentCount = 0;

            for (int i = 0; i < routeEntity.dataList.Count; i++)
            {
                sendDataList.Add(routeEntity.dataList[i]);
                if (sendDataList[i].IndexOf("追溯号") >= 0)
                {
                    string temp = chaseNo;
                    sendDataList[i] = sendDataList[i].Replace("追溯号", temp.Substring(0, 3));

                }
                if (sendDataList[i].IndexOf("追溯横") >= 0)
                {

                    sendDataList[i] = sendDataList[i].Replace("追溯横", chaseNo);
                }
                if (sendDataList[i].IndexOf("追溯竖") >= 0)
                {

                    string chaseNo2 = "";
                    for (int m = 0; m < chaseNo.Length; m++)
                    {
                        chaseNo2 = chaseNo2 + chaseNo[m] + ",";
                    }
                    chaseNo2 = chaseNo2.Substring(0, chaseNo2.Length - 1);
                    sendDataList[i] = sendDataList[i].Replace("追溯竖", chaseNo2);

                }
                if (routeList[nRouteIndex].dataList[i].IndexOf("****") >= 0)
                {
                    string no = string.Format("{0:0000}", serialNo);
                    sendDataList[i] = sendDataList[i].Replace("****", no);
                    if (currentDay != DateTime.Now.Day)
                    {
                        currentDay = DateTime.Now.Day;
                        serialNo = 1;
                    }
                    else
                    {
                        if (serialNo < 9999)
                        {
                            serialNo++;
                        }
                        else
                        {
                            serialNo = 1;
                        }
                    }
                    product.BaseCords.BaseCord1 = serialNo.ToString();

                    Action<List<string>> action = (data) =>
                    {
                        this.labelTextSum.textBox.Text = data[0];

                    };
                    Invoke(action, new List<string>() { serialNo.ToString() });
                }
                else if (routeList[nRouteIndex].dataList[i].IndexOf("####") >= 0)
                {
                    string no = string.Format("{0:0000}", serialNo2);
                    sendDataList[i] = sendDataList[i].Replace("####", no);
                    if (currentDay != DateTime.Now.Day)
                    {
                        currentDay = DateTime.Now.Day;
                        serialNo2 = 1;
                    }
                    else
                    {
                        if (serialNo2 < 9999)
                        {
                            serialNo2++;
                        }
                        else
                        {
                            serialNo2 = 1;
                        }
                    }
                    product.BaseCords.BaseCord2 = serialNo2.ToString();
                    Action<List<string>> action = (data) =>
                    {
                        this.labelText1.textBox.Text = data[0];

                    };
                    Invoke(action, new List<string>() { serialNo2.ToString() });
                }
                else if (routeList[nRouteIndex].dataList[i].IndexOf("%%%%") >= 0)
                {
                    string no = string.Format("{0:0000}", serialNo3);
                    sendDataList[i] = sendDataList[i].Replace("%%%%", no);
                    if (currentDay != DateTime.Now.Day)
                    {
                        currentDay = DateTime.Now.Day;
                        serialNo3 = 1;
                    }
                    else
                    {
                        if (serialNo3 < 9999)
                        {
                            serialNo3++;
                        }
                        else
                        {
                            serialNo3 = 1;
                        }
                    }
                    product.BaseCords.BaseCord3 = serialNo3.ToString();
                    Action<List<string>> action = (data) =>
                    {
                        this.labelText2.textBox.Text = data[0];

                    };
                    Invoke(action, new List<string>() { serialNo3.ToString() });
                }
                else if (routeList[nRouteIndex].dataList[i].IndexOf("!!!!") >= 0)
                {
                    string no = string.Format("{0:0000}", serialNo4);
                    sendDataList[i] = sendDataList[i].Replace("!!!!", no);
                    if (currentDay != DateTime.Now.Day)
                    {
                        currentDay = DateTime.Now.Day;
                        serialNo4 = 1;
                    }
                    else
                    {
                        if (serialNo4 < 9999)
                        {
                            serialNo4++;
                        }
                        else
                        {
                            serialNo4 = 1;
                        }
                    }
                    product.BaseCords.BaseCord4 = serialNo4.ToString();
                    Action<List<string>> action = (data) =>
                    {
                        this.labelText4.textBox.Text = data[0];

                    };
                    Invoke(action, new List<string>() { serialNo4.ToString() });
                }
                else if (routeList[nRouteIndex].dataList[i].IndexOf("----") >= 0)
                {
                    string no = string.Format("{0:0000}", serialNo5);
                    sendDataList[i] = sendDataList[i].Replace("----", no);
                    if (currentDay != DateTime.Now.Day)
                    {
                        currentDay = DateTime.Now.Day;
                        serialNo5 = 1;
                    }
                    else
                    {
                        if (serialNo5 < 9999)
                        {
                            serialNo5++;
                        }
                        else
                        {
                            serialNo5 = 1;
                        }
                    }
                    product.BaseCords.BaseCord5 = serialNo5.ToString();
                    Action<List<string>> action = (data) =>
                    {
                        this.labelText5.textBox.Text = data[0];

                    };
                    Invoke(action, new List<string>() { serialNo5.ToString() });
                }
                else if (routeList[nRouteIndex].dataList[i].IndexOf("++++") >= 0)
                {
                    string no = string.Format("{0:0000}", serialNo6);
                    sendDataList[i] = sendDataList[i].Replace("++++", no);
                    if (currentDay != DateTime.Now.Day)
                    {
                        currentDay = DateTime.Now.Day;
                        serialNo6 = 1;
                    }
                    else
                    {
                        if (serialNo6 < 9999)
                        {
                            serialNo6++;
                        }
                        else
                        {
                            serialNo6 = 1;
                        }
                    }
                    product.BaseCords.BaseCord6 = serialNo6.ToString();

                    Action<List<string>> action = (data) =>
                    {
                        this.labelText6.textBox.Text = data[0];

                    };
                    Invoke(action, new List<string>() { serialNo6.ToString() });
                }
                else if (routeList[nRouteIndex].dataList[i].IndexOf("||||") >= 0)
                {
                    string no = string.Format("{0:0000}", serialNo7);
                    sendDataList[i] = sendDataList[i].Replace("||||", no);
                    if (currentDay != DateTime.Now.Day)
                    {
                        currentDay = DateTime.Now.Day;
                        serialNo7 = 1;
                    }
                    else
                    {
                        if (serialNo7 < 9999)
                        {
                            serialNo7++;
                        }
                        else
                        {
                            serialNo7 = 1;
                        }
                    }
                    product.BaseCords.BaseCord7 = serialNo7.ToString();
                    Action<List<string>> action = (data) =>
                    {
                        this.labelText7.textBox.Text = data[0];

                    };
                    Invoke(action, new List<string>() { serialNo7.ToString() });
                }
                else if (routeList[nRouteIndex].dataList[i].IndexOf("$$$$") >= 0)
                {
                    string no = string.Format("{0:0000}", serialNo8);
                    sendDataList[i] = sendDataList[i].Replace("$$$$", no);
                    if (currentDay != DateTime.Now.Day)
                    {
                        currentDay = DateTime.Now.Day;
                        serialNo8 = 1;
                    }
                    else
                    {
                        if (serialNo8 < 9999)
                        {
                            serialNo8++;
                        }
                        else
                        {
                            serialNo8 = 1;
                        }
                    }
                    product.BaseCords.BaseCord8 = serialNo8.ToString();
                    Action<List<string>> action = (data) =>
                    {
                        this.labelText8.textBox.Text = data[0];

                    };
                    Invoke(action, new List<string>() { serialNo8.ToString() });
                }

            }

            TcpPrinter.Send(printerHelper.SendSelectByte(routeEntity.TemplateNo)); // 切换模板2
            Thread.Sleep(100);
            TcpPrinter.Send(printerHelper.SendDataToTemplate(sendDataList[0].Split(',').ToList()));
            bIsSend = true;
            sendNum = 1;

            if (routeEntity.dataType == "Barcode")
            {
                Common.SaveConfigToFile(current);
            }
            //if (routeEntity.dataType == "Text")
            //{
            //    //SocketSendReceive(TcpPrinter, printerHelper.resetDataQueue());
            //    //Thread.Sleep(100);
            //    //SelectTemplate(TcpPrinter, printerHelper.SelectJobByIndex(routeEntity.TemplateNo));
            //    //Thread.Sleep(100);
            //    //SocketSendReceive(TcpPrinter, printerHelper.DataQueueDisable());
            //    //Thread.Sleep(100);
            //    //SocketSendReceive(TcpPrinter, printerHelper.enableDataQueue());
            //    //Thread.Sleep(100);
            //    ////SocketSendReceive(TcpPrinter, printerHelper.resetDataQueue());

                //    //List<List<string>> sendData = new List<List<string>>();
                //    //for (int i = 0; i < sendDataList.Count; i++)
                //    //{
                //    //    List<string> temp = sendDataList[i].Split(',').ToList();
                //    //    sendData.Add(temp);
                //    //}

                //    //SocketSendReceive(TcpPrinter, printerHelper.DataQueueSendDataList(sendData));
                //    //TcpPrinter.Send(printerHelper.sendDataQueue2(sendData));
                //    TcpPrinter.Send(printerHelper.SendSelectByte(routeEntity.TemplateNo)); // 切换模板2
                //    //SelectTemplate(TcpPrinter, printerHelper.SelectJobByIndex(routeEntity.TemplateNo));
                //    //TcpPrinter.Send(printerHelper.SendSelectByte(routeEntity.TemplateNo)); // 切换模板2
                //    //Thread.Sleep(50);
                //    for (int i = 0; i < sendDataList[0].Split(',').Length; i++)
                //    {
                //        SelectTemplate(TcpPrinter, printerHelper.SendDataByte(sendDataList[0].Split(',')[i], i + 1));
                //        //TcpPrinter.Send(printerHelper.SendDataByte(sendDataList[0].Split(',')[i], i + 1));
                //        //Thread.Sleep(50);
                //    }




                //}
                //else if (routeEntity.dataType == "Barcode")
                //{
                //    Common.SaveConfigToFile(current);

                //    SocketSendReceive(TcpPrinter, printerHelper.resetDataQueue());
                //    Thread.Sleep(100);
                //    SelectTemplate(TcpPrinter, printerHelper.SelectJobByIndex(routeEntity.TemplateNo));
                //    Thread.Sleep(100);
                //    SocketSendReceive(TcpPrinter, printerHelper.DataQueueDisable());
                //    Thread.Sleep(100);
                //    SocketSendReceive(TcpPrinter, printerHelper.enableDataQueue());
                //    Thread.Sleep(100);
                //    SocketSendReceive(TcpPrinter, printerHelper.sendDataQueue(sendDataList));

                //}
        }
        /// <summary>
        /// 解析返回的坐标
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public List<float> analyPosition(List<byte> result)
        {
            byte[] buffer = result.ToArray();
            List<byte> bufferX = new List<byte>();
            bufferX.Add(buffer[9]);
            bufferX.Add(buffer[10]);
            bufferX.Add(buffer[11]);
            bufferX.Add(buffer[12]);

            List<byte> bufferY = new List<byte>();
            bufferY.Add(buffer[13]);
            bufferY.Add(buffer[14]);
            bufferY.Add(buffer[15]);
            bufferY.Add(buffer[16]);

            List<byte> bufferZ = new List<byte>();
            bufferZ.Add(buffer[17]);
            bufferZ.Add(buffer[18]);
            bufferZ.Add(buffer[19]);
            bufferZ.Add(buffer[20]);

            float mPosX = BitConverter.ToSingle(bufferX.ToArray(), 0);
            float mPosY = BitConverter.ToSingle(bufferY.ToArray(), 0);
            float mPosZ = BitConverter.ToSingle(bufferZ.ToArray(), 0);

            Action<List<string>> action = (data) =>
            {
                this.label2.Text = data[0].ToString();
                this.label3.Text = data[1].ToString();
                this.label4.Text = data[2].ToString();
                this.label5.Text = data[3].ToString();
            };
            Invoke(action, new List<string>() { mPosX.ToString(), mPosY.ToString(), mPosZ.ToString(), nPrintLineIndex.ToString() });

            var fPosX = BitConverter.ToSingle(buffer, 9);
            var fPosY = BitConverter.ToSingle(buffer, 13);
            var fPosZ = BitConverter.ToSingle(buffer, 17);

            return new List<float>() { fPosX, fPosY, fPosZ };
        }
        #endregion




    }
}
