using DAL.DAL;
using DAL.Model;
using ListBLL.common;
using ListBLL.model;
using MJBLL.model;
using Newtonsoft.Json;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace ListBLL.Logic
{
    public class Login : ICommand<GameSession, ProtobufRequestInfo>
    {
        GroupStaffInfoDAL groupStaffInfoDAL = new GroupStaffInfoDAL();
        GroupInfoDAL groupInfoDAL = new GroupInfoDAL();
        public string Name
        {
            get { return "11001"; }

        }
        /// <summary>
        /// 证书
        /// </summary>
        public string CerStr { get; set; }
        /// <summary>
        /// 用户类型1龙宝0微信
        /// </summary>
        public int UserType { get; set; }
        /// <summary>
        /// 用户龙宝数量
        /// </summary>
        public long UserLongBao { get; set; }
        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {
            IPEndPoint clientipe = session.RemoteEndPoint;

            session.Logger.Debug("登陆sssionID--------" + session.SessionID);

            string fileName = string.Empty;//文件名
            string headImg = string.Empty;//头像图片
            session.Logger.Debug("登录游戏----------" + DateTime.Now);
            var userinfo = SendLogin.ParseFrom(requestInfo.Body);
            mjuserinfo usermodel = new mjuserinfo();

            mjuserinfo model = new mjuserinfo()
            {
                nickname = userinfo.Nickname,
                openid = userinfo.Openid,
                city = userinfo.City,
                headimg = userinfo.Headimg,
                province = userinfo.Province,
                unionid = userinfo.Unionid,
                sex = int.Parse(userinfo.Sex),
                Oldheadimg = userinfo.Headimg,
                is_band = UserType
            };

            usermodel = AddUser(model, session);
            session.Logger.Debug("登录用户openid:" + userinfo.Openid + "|昵称:" + userinfo.Nickname + "----------" + DateTime.Now);
            // UserInfo olduser = Gongyong.userlist.Find(u => u.openid == userinfo.Openid);

            RedisLoginModel olduser = RedisUtility.Get<RedisLoginModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERLIST, userinfo.Openid, userinfo.Unionid));

            if (olduser == null)
            {
                NewUserLogin(userinfo, usermodel, session, clientipe, GameInformationBase.serverName, requestInfo);
            }
            else if (olduser.ServerName.Equals(GameInformationBase.serverName))
            {


                UserInfo userInfo = Gongyong.userlist.Find(u => u.openid == userinfo.Openid);

                // 可能会存在缓存服务器有用户信息，而服务器没有的情况。因此需要再次判断 
                if (userInfo == null)
                {
                    NewUserLogin(userinfo, usermodel, session, clientipe, GameInformationBase.serverName, requestInfo);
                    session.Logger.Debug("新登录用户openid:" + userinfo.Openid);
                }
                else if (userInfo.session.Connected)
                {

                    ReturnLogin log = ReturnLogin.CreateBuilder().SetLoginstat(2).SetUserID(int.Parse(usermodel.id.ToString())).SetUserRoomCard(0).Build();
                    byte[] msg = log.ToByteArray();

                    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1002, msg.Length, requestInfo.MessageNum, msg)));

                    session.Logger.Debug("登录游戏失败,当前对象存在----------" + DateTime.Now);
                    session.Logger.Debug("sssionID--------" + session.SessionID);
                }
                else
                {
                    session.Logger.Debug("登录游戏成功,当前对象存在----------" + DateTime.Now);
                    var redisUserInfo = RedisUtility.Get<RedisGameModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERGAME, userinfo.Openid, userinfo.Unionid));
                    // UserInfo user = Gongyong.userlist.Find(u => u.openid == userinfo.Openid);
                    userInfo.city = userinfo.City;

                    //判断图片是否保存至图片服务器，保存相应头像地址
                    userInfo.headimg = usermodel.headimg;
                    userInfo.nickname = userinfo.Nickname;
                    userInfo.openid = userinfo.Openid;
                    userInfo.province = userinfo.Province;
                    userInfo.session = session;
                    userInfo.sex = userinfo.Sex;
                    userInfo.unionid = userinfo.Unionid;
                    userInfo.Lat = userinfo.Latitude;
                    userInfo.UserID = long.Parse(usermodel.id.ToString());
                    userInfo.UserIP = clientipe.Address.ToString();
                    userInfo.GroupID = groupStaffInfoDAL.GetGroupIDByUserID(usermodel.id).ToList();
                    userInfo.Type = UserType;
                    //userInfo.GroupID.AddRange(groupInfoDAL.GetGroupIDByUserID(usermodel.id));//因为圈主 不存在圈子成员表中， 所以需要单独再添加一次
                    RedisUtility.Set(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, userinfo.Openid, userinfo.Unionid), new RedisUserInfoModel(userInfo));
                    // mjuser mju = Gongyong.mulist.Find(u => u.Openid == userinfo.Openid);
                    bool isSendCL = false;
                    if (redisUserInfo != null && redisUserInfo.RoomID != 0)
                    {
                        //SendCL(session, userinfo.Openid, requestInfo);
                        isSendCL = RedisUtility.GetServerIP(redisUserInfo.ServerName, requestInfo.MessageNum, session, 2, userinfo.Openid, userinfo.Unionid, true, redisUserInfo.RoomID);//2为有未结束的游戏
                    }
                    //UserClient userClient = new UserClient();
                    //userClient.UpdateUserInfo(new registrationactivitiesModel { GameUserID = userInfo.UserID.ToString(), HeadImgurl = userInfo.headimg, UnionID = userInfo.unionid, LastTime = DateTime.Now, NickName = userInfo.nickname });
                    //var resultData = JsonConvert.DeserializeObject<ResultData>(userClient.GetGameCurrency(userInfo.unionid)).Data;
                    long roomCard = 0;
                    roomCard = UserType == 1 ? UserLongBao : RoomCardUtility.GetRoomCard(userInfo.UserID);

                    //long.TryParse(resultData == null ? "0" : resultData.ToString(), out roomCard);
                    userInfo.session = session;
                    var log = ReturnLogin.CreateBuilder().SetLoginstat(1).SetUserID(int.Parse(usermodel.id.ToString())).SetUserRoomCard(roomCard);
                    if (!string.IsNullOrEmpty(CerStr))
                    {
                        log.SetCertificate(CerStr);//返回证书
                        log.SetUnionid(userInfo.unionid);
                        log.SetHeadimg(userInfo.headimg);
                        log.SetUserName(userInfo.nickname);
                    }
                    byte[] msg = log.Build().ToByteArray();
                    // if(!isSendCL)
                    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1002, msg.Length, requestInfo.MessageNum, msg)));

                    var list = new AnnouncementDAL().GetAnnouncement();//获取公告列表
                    var returnAnnouncement = ReturnAnnouncement.CreateBuilder();
                    foreach (var item in list)
                    {
                        returnAnnouncement.AddAnnouncement(AnnouncementInfo.CreateBuilder().SetTitle(item.Title).SetContent(item.Content).Build());
                    }
                    var dataInfo = returnAnnouncement.Build().ToByteArray();
                    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 8001, dataInfo.Length, requestInfo.MessageNum, dataInfo)));
                    session.Logger.Debug("登录游戏成功,当前对象存在----------" + DateTime.Now);
                }
            }
            else//如果用户登录的服务器不是当前服务器，则返回登录的服务器IP和端口
            {
                UserInfo userInfo = Gongyong.userlist.Find(u => u.openid == userinfo.Openid);
                var log = ReturnLogin.CreateBuilder().SetLoginstat(1).SetUserID(int.Parse(usermodel.id.ToString())).SetUserRoomCard(0);
                if (!string.IsNullOrEmpty(CerStr))
                {
                    log.SetCertificate(CerStr);//返回证书
                    log.SetUnionid(userInfo.unionid);
                    log.SetHeadimg(userInfo.headimg);
                    log.SetUserName(userInfo.nickname);
                }
                byte[] msg = log.Build().ToByteArray();
                session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1002, msg.Length, requestInfo.MessageNum, msg)));
                RedisUtility.GetServerIP(olduser.ServerName, requestInfo.MessageNum, session, 1, userinfo.Openid, userinfo.Unionid);//2为有未结束的游戏
                session.Logger.Debug("有未结束的游戏");
            }

        }

        /// <summary>
        /// 新用户登录信息保存
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="usermodel"></param>
        /// <param name="session"></param>
        /// <param name="clientipe"></param>
        /// <param name="serverName"></param>
        /// <param name="requestInfo"></param>
        private void NewUserLogin(SendLogin userinfo, mjuserinfo usermodel, GameSession session, IPEndPoint clientipe, string serverName, ProtobufRequestInfo requestInfo)
        {

            UserInfo user = new UserInfo()
            {
                city = userinfo.City,
                //判断图片是否保存至图片服务器，保存相应头像地址
                headimg = usermodel.headimg,
                nickname = userinfo.Nickname,
                openid = userinfo.Openid,
                province = userinfo.Province,
                session = session,
                sex = userinfo.Sex,
                unionid = userinfo.Unionid,
                Lat = userinfo.Latitude,
                UserID = usermodel.id,
                UserIP = clientipe.Address.ToString(),
                GroupID = groupStaffInfoDAL.GetGroupIDByUserID(usermodel.id).ToList(),
                Type = UserType,
                ConnTime = DateTime.Now
            };
            var redisUserInfo = RedisUtility.Get<RedisGameModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERGAME, userinfo.Openid, userinfo.Unionid));
            //user.GroupID.AddRange(groupInfoDAL.GetGroupIDByUserID(usermodel.id));//因为圈主 不存在圈子成员表中， 所以需要单独再添加一次
            // UserClient userClient = new UserClient();
            // userClient.UpdateUserInfo(new registrationactivitiesModel { GameUserID = user.UserID.ToString(), HeadImgurl = user.headimg, UnionID = user.unionid, LastTime = DateTime.Now, NickName = user.nickname });
            //var resultData = JsonConvert.DeserializeObject <ResultData> (userClient.GetGameCurrency(user.unionid)).Data;
            long roomCard = roomCard = UserType == 1 ? UserLongBao : RoomCardUtility.GetRoomCard(user.UserID);
            // long.TryParse(resultData == null ?"0":resultData.ToString(), out roomCard); //resultData==null?"0": resultData.ToString()//userClient.GetGameCurrencyList(user.unionid)//resultData ? Convert.ToInt64(resultData.Data) : 0;

            Gongyong.userlist.Add(user);
            RedisUtility.Set(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERLIST, userinfo.Openid, userinfo.Unionid), new RedisLoginModel { Openid = userinfo.Openid, ServerName = serverName, Unionid = userinfo.Unionid });
            RedisUtility.Set(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERINFO, userinfo.Openid, userinfo.Unionid), new RedisUserInfoModel(user));

            var A = RedisUtility.Get<RedisLoginModel>(RedisUtility.GetKey(GameInformationBase.COMMUNITYUSERLIST, userinfo.Openid, userinfo.Unionid));


           var log = ReturnLogin.CreateBuilder().SetLoginstat(1).SetUserID(int.Parse(usermodel.id.ToString())).SetUserRoomCard(roomCard);
            if (!string.IsNullOrEmpty(CerStr))
            {
                log.SetCertificate(CerStr);//返回证书
                log.SetUnionid(user.unionid);
                log.SetHeadimg(user.headimg);
                log.SetUserName(user.nickname);
            }
            byte[] msg = log.Build().ToByteArray();
            bool isSendCL = false;
            if (redisUserInfo != null && redisUserInfo.RoomID != 0)
            {
                //SendCL(session, userinfo.Openid, requestInfo);
                isSendCL = RedisUtility.GetServerIP(redisUserInfo.ServerName, requestInfo.MessageNum, session, 2, userinfo.Openid, userinfo.Unionid, true, redisUserInfo.RoomID);//2为有未结束的游戏
            }
            // if (!isSendCL)
            session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1002, msg.Length, requestInfo.MessageNum, msg)));
            var list = new AnnouncementDAL().GetAnnouncement();//获取公告列表
            var returnAnnouncement = ReturnAnnouncement.CreateBuilder();
            foreach (var item in list)
            {
                returnAnnouncement.AddAnnouncement(AnnouncementInfo.CreateBuilder().SetTitle(item.Title).SetContent(item.Content).Build());
            }
            var data = returnAnnouncement.Build().ToByteArray();
            session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 8001, data.Length, requestInfo.MessageNum, data)));
            session.Logger.Debug("登录游戏成功----------" + DateTime.Now);


        }


        /// <summary>
        /// 判断登陆用户信息（如无则添加）
        /// </summary>
        /// <param name="md"></param>
        /// <returns></returns>
        public mjuserinfo AddUser(mjuserinfo md, GameSession session)
        {
            mjuserinfoDAL dal = new mjuserinfoDAL();
            mjuserinfo model = dal.GetModel(md.openid);
            LogDAL Ldal = new LogDAL();
            login_log Lmodel = new login_log();
            if (model == null)
            {
                try
                {
                    md.addtime = DateTime.Now;
                    // string fileName =Guid.NewGuid() + ".jpg"; ;
                    //  string imgurl = PictureUtility.SavePicture(md.headimg, out fileName);//将图片保存至本地返回详细路径和文件名
                    //    if (!string.IsNullOrEmpty(imgurl))//验证是否保存成功
                    //     md.headimg = PutObjectSample.PutObjectFromFile(md.headimg, fileName, -1);//服务器没有图片，因此不能存储至图片服务器

                    dal.Add(md);
                    Lmodel.ID = Guid.NewGuid().ToString();
                    Lmodel.login_state = 1;
                    Lmodel.login_time = DateTime.Now;
                    Lmodel.openid = md.openid;
                    Lmodel.City = md.city;
                    Ldal.Add(Lmodel);
                }
                catch (Exception ex)
                {
                    session.Logger.Error(ex);
                }
                return dal.GetModel(md.openid); ;
            }
            else
            {
                try
                {
                    md.id = model.id;
                    //if (!md.Oldheadimg.Equals(model.Oldheadimg))
                    //{
                    //    string fileName = Guid.NewGuid() + ".jpg"; ;
                    //    //服务器没有图片，因此不能存储至图片服务器  md.headimg = PutObjectSample.PutObjectFromFile(md.headimg, fileName, -1);
                    //}
                    dal.Update(md);
                    Lmodel.ID = Guid.NewGuid().ToString();
                    Lmodel.login_state = 1;
                    Lmodel.login_time = DateTime.Now;
                    Lmodel.openid = md.openid;
                    Lmodel.City = md.city;
                    Ldal.Add(Lmodel);
                }
                catch (Exception ex)
                {

                    session.Logger.Error(ex);
                }

                return md;
            }

        }


    }
}
