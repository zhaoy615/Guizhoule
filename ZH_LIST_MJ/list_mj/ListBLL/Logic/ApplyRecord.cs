using DAL.DAL;
using ListBLL.common;
using ListBLL.model;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ListBLL.Logic
{
    public class ApplyRecord : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11060"; }

        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            GroupInfoDAL groupInfoDAL = new GroupInfoDAL();


            var sendApplyRecord = SendApplyRecord.ParseFrom(requestInfo.Body);

     //       var player = Player.CreateBuilder();
            var groupApplyInfo = GroupApplyInfo.CreateBuilder();
            var returnGroupApplyInfo = ReturnGroupApplyInfo.CreateBuilder();
            Byte[] returnData = null;


            List<DAL.Model.ApplyRecord> list = null;

            if (sendApplyRecord.HasGroupID)
            {
                list = groupInfoDAL.GroupApplyRecord(sendApplyRecord.GroupID);
                if (list.Count == 0)
                {
                    returnData = returnGroupApplyInfo.Build().ToByteArray();
                    session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1061, returnData.Length, requestInfo.MessageNum, returnData)));
                }
                else
                {
                    foreach (var item in list)
                    {
                        DAL.Model.mjuserinfo userinfo = new DAL.DAL.mjuserinfoDAL().GetModel(item.ApplyJoinUserID);
                        var player = Player.CreateBuilder().SetGroupUserID(userinfo.id).SetNickName(HttpUtility.UrlDecode(HttpUtility.UrlDecode(userinfo.nickname))).SetPicture(userinfo.headimg);
                        groupApplyInfo.SetGroupID(item.GroupID).SetApplyStatus(item.ApplyStatus).AddPlayerInfo(player).SetApplyDateTime(TimeToLong.ConvertDateTimeInt(item.ApplyDateTime)).SetGroupName(HttpUtility.UrlDecode(HttpUtility.UrlDecode(item.GroupName)));
                        //       returnGroupApplyInfo.AddGroupApplyInfoList(groupApplyInfo);
                        returnGroupApplyInfo.AddGroupApplyInfoList(groupApplyInfo);
                    }
                    //      groupApplyInfo.SetGroupID(item.GroupID).SetApplyStatus(item.ApplyStatus).AddPlayerInfo(player).SetApplyDateTime(TimeToLong.ConvertDateTimeInt(item.ApplyDateTime)).SetGroupName(item.GroupName);
                  
                    returnData = returnGroupApplyInfo.Build().ToByteArray();
                    session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1061, returnData.Length, requestInfo.MessageNum, returnData)));
                    //session.Close();
                }
            }
            else
            {
                list = groupInfoDAL.PlayerApplyRecord(sendApplyRecord.UserID);

                if (list.Count == 0)
                {
                    returnData = returnGroupApplyInfo.Build().ToByteArray();
                    session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1061, returnData.Length, requestInfo.MessageNum, returnData)));
                }
                else
                {
                    foreach (var item in list)
                    {
                        DAL.Model.mjuserinfo userinfo = new DAL.DAL.mjuserinfoDAL().GetModel(item.ApplyJoinUserID);
                        var player = Player.CreateBuilder().SetGroupUserID(userinfo.id).SetNickName(HttpUtility.UrlDecode(HttpUtility.UrlDecode(userinfo.nickname))).SetPicture(userinfo.headimg);
                        groupApplyInfo.SetGroupID(item.GroupID).SetApplyStatus(item.ApplyStatus).AddPlayerInfo(player).SetApplyDateTime(TimeToLong.ConvertDateTimeInt(item.ApplyDateTime)).SetGroupName(HttpUtility.UrlDecode(HttpUtility.UrlDecode(item.GroupName)));
                        //   returnGroupApplyInfo.AddGroupApplyInfoList(groupApplyInfo);
                        returnGroupApplyInfo.AddGroupApplyInfoList(groupApplyInfo);
                    }
                   
                    returnData = returnGroupApplyInfo.Build().ToByteArray();
                    session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1061, returnData.Length, requestInfo.MessageNum, returnData)));
                }
            }
        }
    }
}
