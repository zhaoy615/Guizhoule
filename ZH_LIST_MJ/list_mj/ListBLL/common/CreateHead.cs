using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.common
{
   public  class CreateHead
    {
        /// <summary>
        /// 创建消息
        /// </summary>
        /// <param name="messageNum">消息号</param>
        /// <param name="messageLength">消息体长度</param>
        /// <param name="messgaeResNum">返回的消息号</param>
        ///   <param name="body">消息体</param>
        /// <returns></returns>
        public static Byte[] CreateMessage(int messageNum, int messageLength, int messgaeResNum,byte[] body)
        {
            List<byte> head = new List<byte>();
            head.AddRange(IntToByte.intToBytes(messageNum));
            head.AddRange(IntToByte.intToBytes(messageLength));
            head.AddRange(IntToByte.intToBytes(messgaeResNum));
            head.AddRange(body);
            return head.ToArray();
        }
    }
}
