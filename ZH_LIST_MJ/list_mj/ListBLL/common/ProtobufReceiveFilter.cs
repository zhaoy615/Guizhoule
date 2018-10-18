using System;
using Google.ProtocolBuffers;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.Facility.Protocol;
using SuperSocket.Common;

namespace ListBLL.common
{
    /// <summary>
    /// 带消息头的协议处理器
    /// </summary>
    public class ProtobufReceiveFilter : FixedHeaderReceiveFilter<ProtobufRequestInfo>//: IReceiveFilter<ProtobufRequestInfo>, IOffsetAdapter, IReceiveFilterInitializer
    {
        public ProtobufReceiveFilter() : base(12)
        {
        }
        ProtobufRequestInfo info = new ProtobufRequestInfo();
        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            info.MessageNum= IntToByte.bytesToInt(header.CloneRange(offset, length),0);
            info.Messagelength = IntToByte.bytesToInt(header.CloneRange(offset, length), 4);
            info.MessageResNum = IntToByte.bytesToInt(header.CloneRange(offset, length), 8);
            info.Key = info.MessageNum.ToString();
            return info.Messagelength;
        }

        protected override ProtobufRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            info.Body = bodyBuffer.CloneRange(offset, length);
            return info;
        }
    }
}