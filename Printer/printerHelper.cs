using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printer
{
    public class printerHelper
    {
        public static byte[] SwapUshort(ushort value)
        {
            var bytes = new byte[2];
            bytes[0] = (byte)((0xff00 & value) >> 8);
            bytes[1] = (byte)(0xff & value);

            return bytes;
        }

        /// <summary>
        /// 发送喷码机变量
        /// </summary>
        /// <param name="sendData">变量内容</param>
        /// <param name="index">变量编号</param>
        /// <returns></returns>
        public static byte[] SendDataByte(string sendData, int index)
        {
            var byteArray = Encoding.ASCII.GetBytes(sendData);
            var len = 1 + 2 + 1 + 2 + byteArray.Length + 1;
            var results = new byte[len];

            results[0] = 0xE8;
            results[1] = SwapInt(byteArray.Length + 3)[0];
            results[2] = SwapInt(byteArray.Length + 3)[1];
            results[3] = (byte)index;
            results[4] = SwapInt(byteArray.Length)[0];
            results[5] = SwapInt(byteArray.Length)[1];
            Array.Copy(byteArray, 0, results, 6, byteArray.Length);

            var temp = new byte[len - 1];
            Array.Copy(results, 0, temp, 0, len - 1);
            var crc = Crc(ToHexString(temp));
            results[len - 1] = Convert.ToByte(crc, 16);

            return results;
        }

        /// <summary>
        /// 选择模板
        /// </summary>
        /// <param name="index">模板编号</param>
        /// <returns></returns>
        public static byte[] SendSelectByte(int index)
        {
            const int len = 1 + 2 + 1 + 1 + 1;
            var results = new byte[len];

            results[0] = 0x98;
            results[1] = 0x00;
            results[2] = 0x02;
            results[3] = 0x00;
            results[4] = (byte)index;

            var temp = new byte[len - 1];
            Array.Copy(results, 0, temp, 0, len - 1);
            var crc = Crc(ToHexString(temp));
            results[5] = Convert.ToByte(crc, 16);
            return results;
        }

        private static byte[] SwapInt(int value)
        {
            var bytes = new byte[2];
            bytes[0] = (byte)((0xff00 & value) >> 8);
            bytes[1] = (byte)(0xff & value);

            return bytes;
        }

        public static string ToHexString(byte[] bytes)
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

        /// <summary>
        /// CRC异或校验
        /// </summary>
        /// <param name="cmdString">命令字符串</param>
        /// <returns></returns>
        private static string Crc(string cmdString)
        {
            //CRC寄存器
            var crcCode = 0;

            //将字符串拆分成为16进制字节数据然后两位两位进行异或校验
            for (var i = 1; i < cmdString.Length / 2; i++)
            {
                var cmdHex = cmdString.Substring(i * 2, 2);

                if (i == 1)
                {
                    var cmdPrvHex = cmdString.Substring((i - 1) * 2, 2);
                    crcCode =
                        (byte)Convert.ToInt32(cmdPrvHex, 16) ^ (byte)Convert.ToInt32(cmdHex, 16);
                }
                else
                    crcCode = (byte)crcCode ^ (byte)Convert.ToInt32(cmdHex, 16);
            }

            //返回16进制校验码
            return Convert.ToString(crcCode, 16).ToUpper();
        }

        /// <summary>
        /// 激活数据队列的功能
        /// </summary>
        /// <returns></returns>
        public static byte[] enableDataQueue()
        {
            const int len = 1 + 1 + 1 + 1 + 1+1+1;
            var results = new byte[len];
            results[0] = 0x6d;
            results[1] = 0x00;
            results[2] = 0x03;
            results[3] = 0x00;
            results[4] = 0x01;
            results[5] = 0x01;
            var temp = new byte[len - 1];
            Array.Copy(results, 0, temp, 0, len - 1);
            var crc = Crc(ToHexString(temp));
            results[len - 1] = Convert.ToByte(crc, 16);
            return results;
        }

        public static byte[] disableDataQueue()
        {
            const int len = 1 + 1 + 1 + 1 + 1 + 1 + 1;
            var results = new byte[len];
            results[0] = 0x6d;
            results[1] = 0x00;
            results[2] = 0x03;
            results[3] = 0x00;
            results[4] = 0x01;
            results[5] = 0x00;
            var temp = new byte[len - 1];
            Array.Copy(results, 0, temp, 0, len - 1);
            var crc = Crc(ToHexString(temp));
            results[len - 1] = Convert.ToByte(crc, 16);
            return results;
        }

        public static byte[] resetDataQueue()
        {
            const int len = 1 + 1 + 1 + 1 + 1 + 1;
            var results = new byte[len];
            results[0] = 0x6d;
            results[1] = 0x00;
            results[2] = 0x02;
            results[3] = 0x00;
            results[4] = 0x02;
            var temp = new byte[len - 1];
            Array.Copy(results, 0, temp, 0, len - 1);
            var crc = Crc(ToHexString(temp));
            results[len - 1] = Convert.ToByte(crc, 16);

            return results;
        }
        /// <summary>
        /// 发送队列，list中的每个元素就是一个队列
        /// </summary>
        /// <param name="sendData"></param>
        /// <returns></returns>
        public static byte[] sendDataQueue(List<string> sendData)
        {
            int datalen = 0;
            List<byte[]> sendDatabyte = new List<byte[]>();
            for (int i = 0; i < sendData.Count; i++)
            {
                byte[] byteArray = Encoding.ASCII.GetBytes(sendData[i]);
                sendDatabyte.Add(byteArray);
                datalen = datalen + byteArray.Length;
            }
            int len = 1 + 2 + 1 + 1 + 2 + 1 + 3 * sendDatabyte.Count + datalen + 1;
            var results = new byte[len];
            results[0] = 0x6d;
            results[1] = SwapInt(len - 4)[0];
            results[2] = SwapInt(len - 4)[1];
            results[3] = 0x00;
            results[4] = 0x0a;
            results[5] = SwapInt(sendData.Count)[0];
            results[6] = SwapInt(sendData.Count)[1];
            results[7] = SwapInt(1)[1];
            int prelength = 0;
            for (int i = 0; i < sendDatabyte.Count; i++)
            {
                results[prelength + i * 3 + 8] = SwapInt(1)[1];
                results[prelength + i * 3 + 9] = SwapInt(sendDatabyte[i].Length)[0];
                results[prelength + i * 3 + 10] = SwapInt(sendDatabyte[i].Length)[1];
                Array.Copy(sendDatabyte[i], 0, results, prelength + 11 + i * 3, sendDatabyte[i].Length);
                prelength = prelength + sendDatabyte[i].Length;
            }
            var temp = new byte[len - 1];
            Array.Copy(results, 0, temp, 0, len - 1);
            var crc = Crc(ToHexString(temp));
            results[len - 1] = Convert.ToByte(crc, 16);
            return results;

        }

        public static byte[] sendDataQueue2(List<List<string>> sendData)
        {
            int datalen = 0;
            List<List<byte[]>> sendDatabyte = new List<List<byte[]>>();
            //int varInQuene = sendData[0].Count;  //每个队列中的变量个数
            for (int i = 0; i < sendData.Count; i++)
            {
                List<byte[]> temp2 = new List<byte[]>();
                for(int j=0;j<sendData[i].Count;j++)
                {
                    byte[] byteArray = Encoding.ASCII.GetBytes(sendData[i][j]);
                    temp2.Add(byteArray);
                    
                    datalen = datalen + byteArray.Length;
                }
                sendDatabyte.Add(temp2);
                
               
            }
            int len = 1 + 2 + 1 + 1 + 2 + 1 + 3 * sendData[0].Count * sendData.Count + datalen + 1;
            var resultsList = new List<byte>();
            resultsList.Add(0x6d);
            resultsList.Add(SwapInt(len - 4)[0]);
            resultsList.Add(SwapInt(len - 4)[1]);
            resultsList.Add(0x00);
            resultsList.Add(0x0a);
            resultsList.Add(SwapInt(sendData.Count)[0]);
            resultsList.Add(SwapInt(sendData.Count)[1]);
            resultsList.Add(SwapInt(sendData[0].Count)[1]);
            for(int i=0;i<sendDatabyte.Count;i++)
            {
                for(int j=0;j<sendDatabyte[i].Count;j++ )
                {
                    resultsList.Add(SwapInt(j+1)[1]);
                    resultsList.Add(SwapInt(sendDatabyte[i][j].Count())[0]);
                    resultsList.Add(SwapInt(sendDatabyte[i][j].Count())[1]);
                    resultsList.AddRange(sendDatabyte[i][j]);
                }
            }

            var crc = Crc(ToHexString(resultsList.ToArray()));
            resultsList.Add( Convert.ToByte(crc, 16));
            return resultsList.ToArray();
            //var results = new byte[len];
            //var resultsList=new List<byte>();
            //results[0] = 0x6d;
            //resultsList.Add(0x6d);
            //results[1] = SwapInt(len - 4)[0];
            //resultsList.Add(SwapInt(len - 4)[0]);
            //results[2] = SwapInt(len - 4)[1];
            //resultsList.Add(SwapInt(len - 4)[1]);
            //results[3] = 0x00;
            //results[4] = 0x0a;
            //results[5] = SwapInt(sendData.Count)[0];
            //results[6] = SwapInt(sendData.Count)[1];
            //results[7] = SwapInt(1)[sendData[0].Count];
            //int prelength = 0;
            //for (int i = 0; i < sendDatabyte.Count; i++)
            //{
            //    for (int j = 0; j < sendDatabyte[i].Count;j++ )
            //    {
            //        results[prelength + i * 3 + 8] = SwapInt(j+1)[1];
            //    }
            //        results[prelength + i * 3 + 8] = SwapInt(sendDatabyte.Count)[1];
            //    results[prelength + i * 3 + 9] = SwapInt(sendDatabyte[i].Length)[0];
            //    results[prelength + i * 3 + 10] = SwapInt(sendDatabyte[i].Length)[1];
            //    Array.Copy(sendDatabyte[i], 0, results, prelength + 11 + i * 3, sendDatabyte[i].Length);
            //    prelength = prelength + sendDatabyte[i].Length;
            //}
            //var temp = new byte[len - 1];
            //Array.Copy(results, 0, temp, 0, len - 1);
            //var crc = Crc(ToHexString(temp));
            //results[len - 1] = Convert.ToByte(crc, 16);
            //return results;

        }

        public static byte[] searchErrorCode()
        {
            List<byte> result = new List<byte>();
            result.Add(0x6d);
            result.Add(0x00);
            result.Add(0x02);
            result.Add(0x00);
            result.Add(0x03);
            var crc = Crc(ToHexString(result.ToArray()));
            result.Add(Convert.ToByte(crc, 16));
            return result.ToArray();
        }

        public static byte[] enableRepeat()
        {
            List<byte> result = new List<byte>();
            result.Add(0xe9);
            result.Add(0x00);
            result.Add(0x01);
            result.Add(0x00);
            result.Add(0xe8);
            return result.ToArray();
        }
        public static byte[] disableRepeat()
        {
            List<byte> result = new List<byte>();
            result.Add(0xe9);
            result.Add(0x00);
            result.Add(0x01);
            result.Add(0x01);
            result.Add(0xe9);
            return result.ToArray();
        }

        public static byte[] clearCounter()
        {
            List<byte> result = new List<byte>();
            result.Add(0x97);
            result.Add(0x00);
            result.Add(0x02);
            result.Add(0x00);
            result.Add(0x01);
            var crc = Crc(ToHexString(result.ToArray()));
            result.Add(Convert.ToByte(crc, 16));
            return result.ToArray();
        }



        /// <summary>
        /// This transmission selects a job library according 
        /// to its position and send prining.
        /// The message must be present in the library beforehand.
        /// This command can be used in AUTO or CUSTOM rank library mode
        /// </summary>
        /// <param name="jobIndex">001~999</param>
        /// <returns></returns>
        public static byte[] SelectJobByIndex(int jobIndex)
        {
            const int len = 1 + 2 + 1 + 1 + 1;
            var results = new byte[len];

            results[0] = 0x98;
            results[1] = 0x00;
            results[2] = 0x02;
            results[3] = 0x00;
            results[4] = (byte)jobIndex;

            var temp = new byte[len - 1];
            Array.Copy(results, 0, temp, 0, len - 1);
            var crc = Crc(ToHexString(temp));
            results[5] = Convert.ToByte(crc, 16);
            return results;

        }

        public void StartJet()
        {
            //C6 00 01 00
        }

        public void StopJet()
        {
            //C6 00 01 01
        }
        public void StopPrinter()
        {
            //C6 00 01 08
        }

        #region Promotionnal coding

        /*************************************************************************
         * The main purpose of the queue management system implemented in 9232 is for promotional coding 
         * → printing unique codes on each product. So the idea is to send to the
            printer a list of data (can be complete messages or only external variables) and each
            data will be used once for one printout..
            A new V24 command 0x6D has been added to the existing one in order to:
            - Enable/Disable the data queue management
            - Reset the queue
            - Read data queue status
            - Send data list into the data queue
            - Send application options
            - Get the max items number to transfer
            The printer will always answer with another 0x6D command with reporting and
            data status. The particularity with this answer is that the printer will not wait for any
            acknowledgement.
         ******************************************************************** */
        public static byte[] DataQueueEnable()
        {
            const int len = 1 + 1 + 1 + 1 + 1 + 1 + 1;
            var results = new byte[len];
            results[0] = 0x6d;
            results[1] = 0x00;
            results[2] = 0x03;
            results[3] = 0x00;
            results[4] = 0x01;
            results[5] = 0x01;
            var temp = new byte[len - 1];
            Array.Copy(results, 0, temp, 0, len - 1);
            var crc = Crc(ToHexString(temp));
            results[len - 1] = Convert.ToByte(crc, 16);
            return results;
        }
        public static byte[] DataQueueDisable()
        {
            //6D 00 03 00 01 00 checksum
            const int len = 1 + 1 + 1 + 1 + 1 + 1 + 1;
            var results = new byte[len];
            results[0] = 0x6d;
            results[1] = 0x00;
            results[2] = 0x03;
            results[3] = 0x00;
            results[4] = 0x01;
            results[5] = 0x00;
            var temp = new byte[len - 1];
            Array.Copy(results, 0, temp, 0, len - 1);
            var crc = Crc(ToHexString(temp));
            results[len - 1] = Convert.ToByte(crc, 16);
            return results;
        }

        public bool DataQueueReadStatus()
        {
            //6D 00 02 00 03 checksum
            return true;
        }

        public static byte[] DataQueueReset()
        {
            //6D 00 02 00 02 checksum
            const int len = 1 + 1 + 1 + 1 + 1 + 1;
            var results = new byte[len];
            results[0] = 0x6d;
            results[1] = 0x00;
            results[2] = 0x02;
            results[3] = 0x00;
            results[4] = 0x02;
            var temp = new byte[len - 1];
            Array.Copy(results, 0, temp, 0, len - 1);
            var crc = Crc(ToHexString(temp));
            results[len - 1] = Convert.ToByte(crc, 16);

            return results;
        }
        /// <summary>
        /// Identifier (1 byte) 6Dh
        /// Length(2 bytes) xxh, xxh
        /// Application type(1 byte) 00h
        /// Sub-command(1 byte) 0Ah
        /// Number of external variables(2 bytes).
        /// Number of external variables per group(1 byte)
        /// Data list of variables(N bytes)
        /// - Variable number(1 byte)
        /// - Variable length(2 bytes)
        /// - Variable content(X bytes)
        /// binary
        /// Checksum(1 byte) xxh
        /// </summary>
        /// <returns></returns>
        public static byte[] DataQueueSendDataList(List<List<string>> sendData)
        {
            //6D ** ** 00 0A data0~datan checksum
            int datalen = 0;
            List<List<byte[]>> sendDatabyte = new List<List<byte[]>>();
            //int varInQuene = sendData[0].Count;  //每个队列中的变量个数
            for (int i = 0; i < sendData.Count; i++)
            {
                List<byte[]> temp2 = new List<byte[]>();
                for (int j = 0; j < sendData[i].Count; j++)
                {
                    byte[] byteArray = Encoding.ASCII.GetBytes(sendData[i][j]);
                    temp2.Add(byteArray);

                    datalen = datalen + byteArray.Length;
                }
                sendDatabyte.Add(temp2);


            }
            int len = 1 + 2 + 1 + 1 + 2 + 1 + 3 * sendData[0].Count * sendData.Count + datalen + 1;
            var resultsList = new List<byte>();
            resultsList.Add(0x6d);
            resultsList.Add(SwapInt(len - 4)[0]);
            resultsList.Add(SwapInt(len - 4)[1]);
            resultsList.Add(0x00);
            resultsList.Add(0x0a);
            resultsList.Add(SwapInt(sendData.Count)[0]);
            resultsList.Add(SwapInt(sendData.Count)[1]);
            resultsList.Add(SwapInt(sendData[0].Count)[1]);
            for (int i = 0; i < sendDatabyte.Count; i++)
            {
                for (int j = 0; j < sendDatabyte[i].Count; j++)
                {
                    resultsList.Add(SwapInt(j + 1)[1]);
                    resultsList.Add(SwapInt(sendDatabyte[i][j].Count())[0]);
                    resultsList.Add(SwapInt(sendDatabyte[i][j].Count())[1]);
                    resultsList.AddRange(sendDatabyte[i][j]);
                }
            }

            var crc = Crc(ToHexString(resultsList.ToArray()));
            resultsList.Add(Convert.ToByte(crc, 16));
            return resultsList.ToArray();
        }

        /// <summary>
        /// 6Dh Identifier (1 byte)
        /// 00h, 0Ah Length(2 bytes)
        /// xxh xxh Command report(2 bytes)
        /// Binary Status(8 bytes)
        /// xxh Checksum(1 byte)
        /// 
        /// Command report definition
        /// 0 x 0000 Unknown Command
        /// 0 x 0001 Command processed
        /// 0 x 0002 State already asked(for enable/disable)
        /// 0 x 0003 Data processinf function nonexistant
        /// 0 x 0004 Not validated data queue
        /// 0 x 00FF Data queue full(command not processed)
        /// 
        /// Status definition
        /// Data queue status 1 byte
        /// Printer status 1 byte
        /// Remaining item in the queue 2 bytes
        /// Remaining memory available in the queue in number of bytes 4 bytes
        /// 
        /// Data queue status values
        /// 0 x 00 Data queue in use
        /// 0 x 01 Job in place without external variable
        /// 0 x 02 Queue empty, no data to print
        /// 
        /// Printer status values
        /// Bit 7 Printer not ready
        /// Bit 6 -
        /// Bit 5 -
        /// Bit 4 Consumable empty
        /// Bit 3 Consumable low
        /// Bit 2 Warning except consumable
        /// Bit 1 Fault on printer
        /// Bit 0 -
        /// </summary>
        /// <returns></returns>
        private bool AnswerResult(List<byte> datas)
        {
            return true;
        }

        #endregion

    }
}
