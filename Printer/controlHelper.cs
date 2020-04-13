using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printer
{
    public class controlHelper
    {
        /// <summary>
        /// 发送路径,10命令写寄存器
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="sendData"></param>
        /// <returns></returns>
        public static byte[] PackageModbusTcpFrame10(byte[] startAddress,byte[] sendData)
        {
            //var len = 4 + 2 + 1 + 1 + 2 + 2 + list.Count * 4*5;
            List<byte> results = new List<byte>();
            results.Add(0x00);
            results.Add(0x01);
            results.Add(0x00);
            results.Add(0x00);  
            results.Add(SwapInt(7+sendData.Length)[0]); //数据长度
            results.Add(SwapInt(7 + sendData.Length)[1]); //数据长度
            results.Add(0x01);  //标识符
            results.Add(0x10);  //功能码
            results.Add(startAddress[0]);  //寄存器起始地址
            results.Add(startAddress[1]);  //寄存器起始地址

            results.Add(SwapInt(sendData.Length/2+ sendData.Length % 2)[0]); //寄存器数量
            results.Add(SwapInt(sendData.Length/2 + sendData.Length % 2)[1]); //寄存器数量
            results.Add(0x00);
            for(int i=0;i<sendData.Length;i++)
            {
                results.Add(sendData[i]);
            }
            //List<byte> resultList = results.ToList();
            //for(int i=0;i<list.Count;i++)
            //{
            //    byte[] PosX = BitConverter.GetBytes(list[i].PosX);  //5[]
            //    byte[] PosY = BitConverter.GetBytes(list[i].PosY);
            //    byte[] PosZ = BitConverter.GetBytes(list[i].PosZ);
            //    byte[] Angle = BitConverter.GetBytes(list[i].Angle);
            //    byte[] Time = BitConverter.GetBytes(list[i].Time);
            //    results.AddRange(PosX);
            //    results.AddRange(PosY);
            //    results.AddRange(PosZ);
            //    results.AddRange(Angle);
            //    results.AddRange(Time);
            //}
            return results.ToArray();
        }

        public static byte[] searchOrder()
        {
            var results = new byte[99];
            results[0] = 0x00;
            results[1] = 0x01;
            results[2] = 0x00;
            results[3] = 0x00;
            return null;
        }

        private static byte[] SwapInt(int value)
        {
            var bytes = new byte[2];
            bytes[0] = (byte)((0xff00 & value) >> 8);
            bytes[1] = (byte)(0xff & value);

            return bytes;
        }

        public static byte[] senRoutePos(float X,float Y,float Z,float Angle)
        {
            List<byte> results = new List<byte>();
            results.Add(0x00);
            results.Add(0x01);
            results.Add(0x00);
            results.Add(0x00);
            results.Add(SwapInt(16 + 7)[0]); //数据长度
            results.Add(SwapInt(16 + 7)[1]); //数据长度
            results.Add(0x01);  //标识符
            results.Add(0x10);  //功能码
            results.Add(0x48);  //寄存器起始地址
            results.Add(0xb7);  //寄存器起始地址

            results.Add(SwapInt(8)[0]); //寄存器数量
            results.Add(SwapInt(8)[1]); //寄存器数量
            results.Add(SwapInt(4)[0]);

            List<byte> resultList = results.ToList();
            byte[] PosX = BitConverter.GetBytes(X);
            byte[] PosY = BitConverter.GetBytes(Y);
            byte[] PosZ = BitConverter.GetBytes(Z);
            byte[] AngleArr = BitConverter.GetBytes(Angle);

            results.AddRange(PosX);
            results.AddRange(PosY);
            results.AddRange(PosZ);
            results.AddRange(AngleArr);
            return results.ToArray();
        }
        /// <summary>
        /// 读取位置信息
        /// </summary>
        /// <returns></returns>
        public static byte[] readData()
        {
            List<byte> results = new List<byte>();
            results.Add(0x00);
            results.Add(0x01);
            results.Add(0x00);
            results.Add(0x00);
            results.Add(0x00);
            results.Add(0x06);
            results.Add(0x01);
            results.Add(0x03);
            results.Add(0x23);
            results.Add(0x58);
            results.Add(0x00);
            results.Add(0x0d);
            return results.ToArray();
        }

        public static byte[] isReachPoint()
        {
            List<byte> results = new List<byte>();
            results.Add(0x01);
            results.Add(0x00);
            results.Add(0x00);
            results.Add(0x01);
            results.Add(0x00);
            results.Add(0x06);
            results.Add(0x01);
            results.Add(0x03);
            results.Add(0x00);
            results.Add(0x69);
            results.Add(0x00);
            results.Add(0x01);
            return results.ToArray();
        }
        /// <summary>
        /// 需要打印的点位
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static byte[] sendPrintPos(List<ProductData> list,float XPianyi,float YPianyi,float ZPianyi)
        {
            List<byte> results = new List<byte>();
            results.Add(0x01);
            results.Add(0x00);
            results.Add(0x00);
            results.Add(0x01);
            results.Add(SwapInt(list.Count * 24 + 2)[0]);
            results.Add(SwapInt(list.Count * 24 + 2)[1]);
            results.Add(0x01);
            results.Add(0x71);
            for (int i=0;i<list.Count;i++)
            {

                results.AddRange(BitConverter.GetBytes(Convert.ToInt32(XPianyi * 100 + list[i].PosX * 100)));
                results.AddRange(BitConverter.GetBytes(Convert.ToInt32(YPianyi*100+list[i].PosY * 100)));
                results.AddRange(BitConverter.GetBytes(Convert.ToInt32(ZPianyi * 100 + list[i].Angle * 100)));
                results.AddRange(BitConverter.GetBytes(Convert.ToInt32(list[i].Angle * 100)));

                results.Add(0x00);
                results.Add(0x00);
                results.Add(0x00);
                results.Add(0x00);

                results.Add(0x64);
                results.Add(0x00);
                results.Add(0x00);
                results.Add(0x00);
            }
            return results.ToArray();
        }

    }
}
