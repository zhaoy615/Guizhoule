using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.common
{
   public class MySocket
    {
        Socket sc;
        public MySocket(string ip,int point)
        {
            IPAddress ipaddress;
            EndPoint pointAddress;
            sc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipaddress = IPAddress.Parse(ip);
            pointAddress = new IPEndPoint(ipaddress, point);
            sc.Connect(pointAddress);
            sc.Blocking = true;
          //var str = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new { UserID = "压力1" + i, Integral = 100 + i, Coupons = 100 + i }));
          
        }
        public int Send(byte[] data)
        {
           return sc.Send(data);
        }
        public List<ReceiveData> SendReceive(byte[] data)
        {
            List<ReceiveData> receiveData = new List<ReceiveData>();
            Send(data);
            int count;
            byte[] reqdata = new byte[2048];
            MemoryStream ms = new MemoryStream();
            while (0 != (count = sc.Receive(reqdata)))
            {
                ms.Write(reqdata, 0, count);
                if (count != reqdata.Length)
                    break;
                // if (count < 1024) break;
            }
            while (ms.Length > 0)
            {
                var datainfo = ms.ToArray();
                int number = IntToByte.bytesToInt(datainfo, 0);//消息号
                int length = IntToByte.bytesToInt(datainfo, 4);//消息长度
                int resnumber = IntToByte.bytesToInt(datainfo, 8);//返回消息号
             
               
                if (length > ms.Length)
                    break;
                byte[] body = new byte[length];
                Array.Copy(datainfo, 12, body, 0, length);
                receiveData.Add(new ReceiveData { Data = body, length= length, resnumber= resnumber , number= number });
                ms = new MemoryStream();
                if (datainfo.Length > length + 12)
                {
                    ms.Write(datainfo, length + 12, datainfo.Length - length - 12);
                }
            }
            sc.Close();
            return receiveData;
        }
    }

    public class ReceiveData
    {
        /// <summary>
        /// 请求消息号
        /// </summary>
        public int number { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public int length { get; set; }
        /// <summary>
        /// 返回消息号
        /// </summary>
        public int resnumber { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { get; set; }
    }
}
