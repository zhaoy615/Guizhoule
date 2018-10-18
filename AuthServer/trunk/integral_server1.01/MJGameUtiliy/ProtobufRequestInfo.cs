using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJBLL.common
{
    /// <summary>
    /// 接收消息信息对象
    /// </summary>
    public class ProtobufRequestInfo : IRequestInfo
    {
        /// <summary>
        /// 消息名称用来
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 消息体
        /// </summary>
        public byte[] Body { get; set; }
        /// <summary>
        /// 消息号
        /// </summary>
        public int MessageNum { get; set; }
        /// <summary>
        /// 消息长度
        /// </summary>
        public int Messagelength { get; set; }
        /// <summary>
        /// 返回的消息号
        /// </summary>
        public int MessageResNum { get; set; }
        public ProtobufRequestInfo(int messageNum,int messagelength, int messageResNum,byte[] body)
        {
            MessageNum = messageNum;
            Messagelength = messagelength;
            MessageResNum = messageResNum;
            Body = body;
        }
        public ProtobufRequestInfo()
        {
          
        }
    }
}
