using DAL.DAL;
using ListBLL.common;
using ListBLL.model;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListBLL.Logic
{
    public class Package : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11101"; }

        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            var sendUserPackage = SendUserPackage.ParseFrom(requestInfo.Body);

            PackageDAL packageDAL = new PackageDAL();
            var list= packageDAL.GetPackage(sendUserPackage.Openid);

            var userPackage= ReturnUserPackage.CreateBuilder();

            byte[] userPackageData = null;

            userPackage.SetOpenID(Convert.ToInt32(sendUserPackage.Openid) );
            foreach (var item in list)
            {
                 var prize=  Prize.CreateBuilder().SetPrizeCounts(item.PrizeCounts).SetPrizeDetails(item.prizeDetails)
                    .SetPrizeID(item.PrizeID).SetPrizeImage(item.prizeImage).SetPrizeName(item.prizeName);
                userPackage.AddPrizeList(prize);
            }

            userPackageData = userPackage.Build().ToByteArray();
            session.Send(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1102, userPackageData.Length, requestInfo.MessageNum, userPackageData)));
        }
    }
}
