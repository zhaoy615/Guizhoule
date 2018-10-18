using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using MJBLL.common;
using MJBLL.model;
using SuperSocket.SocketBase.Command;
using System.Net;
using System.IO;
using System.Web;
using DAL.Model;
using DAL.DAL;
using System.Net.Sockets;

namespace MJBLL.mjrule
{
    /// <summary>
    /// 微信登录
    /// </summary>
    public class WXLogin : ICommand<GameSession, ProtobufRequestInfo>
    {
        public string Name
        {
            get { return "11001"; }

        }

        public void ExecuteCommand(GameSession session, ProtobufRequestInfo requestInfo)
        {


            System.Net.IPEndPoint clientipe = (System.Net.IPEndPoint)session.RemoteEndPoint;

            MsgLog msglog = new MsgLog();

            session.Logger.Debug("登陆sssionID--------" + session.SessionID);

            string fileName = string.Empty;//文件名
            string headImg = string.Empty;//头像图片
           session.Logger.Debug("登录游戏----------" + DateTime.Now);
            var userinfo = SendLogin.ParseFrom(requestInfo.Body);
            mjuserinfo usermodel = new mjuserinfo();

            mjuserinfo model = new mjuserinfo()
            {
                nickname = HttpUtility.UrlEncode(userinfo.Nickname),
                openid = userinfo.Openid,
                city = userinfo.City,
                headimg = userinfo.Headimg,
                province = userinfo.Province,
                unionid = userinfo.Unionid,
                sex = int.Parse(userinfo.Sex)
            };

            usermodel = AddUser(model,session);






           session.Logger.Debug("登录用户openid:" + userinfo.Openid + "|昵称:" + userinfo.Nickname + "----------" + DateTime.Now);
            UserInfo olduser = Gongyong.userlist.Find(u => u.openid == userinfo.Openid);
            if (olduser == null)
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
                    UserID = int.Parse(usermodel.id.ToString()),
                    UserIP = clientipe.Address.ToString()


                };
                Gongyong.userlist.Add(user);
                ReturnLogin log = ReturnLogin.CreateBuilder().SetLoginstat(1).SetUserID(int.Parse(usermodel.id.ToString())).Build();
                byte[] msg = log.ToByteArray();

                session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1002, msg.Length, requestInfo.MessageNum, msg)));


                session.Logger.Debug("登录游戏成功----------" + DateTime.Now);
            }
            else
            {

                if (olduser.session.Connected)
                {
                    ReturnLogin log = ReturnLogin.CreateBuilder().SetLoginstat(2).SetUserID(int.Parse(usermodel.id.ToString())).Build();
                    byte[] msg = log.ToByteArray();

                    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1002, msg.Length, requestInfo.MessageNum, msg)));

                    session.Logger.Debug("登录游戏失败,当前对象存在----------" + DateTime.Now);
                    session.Logger.Debug("sssionID--------" + session.SessionID);
                }
                else
                {
                    mjuser mju = Gongyong.mulist.Find(u => u.Openid == userinfo.Openid);
                    if (mju != null)
                    {
                        SendCL(session, userinfo.Openid, requestInfo);
                    }


                    olduser.session = session;
                    ReturnLogin log = ReturnLogin.CreateBuilder().SetLoginstat(1).SetUserID(int.Parse(usermodel.id.ToString())).Build();
                    byte[] msg = log.ToByteArray();
                    session.TrySend(new ArraySegment<byte>(CreateHead.CreateMessage(GameInformationBase.BASEAGREEMENTNUMBER + 1002, msg.Length, requestInfo.MessageNum, msg)));
                    session.Logger.Debug("登录游戏成功,当前对象存在----------" + DateTime.Now);
                }
            }
        }

      

        /// <summary>
        /// 判断登陆用户信息（如无则添加）
        /// </summary>
        /// <param name="md"></param>
        /// <returns></returns>
        public mjuserinfo AddUser(mjuserinfo md, GameSession session )
        {

            int returnid = 0;
            mjuserinfoDAL dal = new mjuserinfoDAL();
            mjuserinfo model = dal.GetModel(md.openid);
            login_logDAL Ldal = new login_logDAL();
            login_log Lmodel = new login_log();



            if (model == null)
            {
                try
                {
                    returnid = dal.GetMaxID() + 1;
                    md.id = returnid;
                    md.addtime = DateTime.Now;
                    string fileName = string.Empty;
                    string imgurl = PictureUtility.SavePicture(md.headimg, out fileName);//将图片保存至本地返回详细路径和文件名
                    if (!string.IsNullOrEmpty(imgurl))//验证是否保存成功
                        md.headimg = PutObjectSample.PutObjectFromFile(imgurl, fileName);
                    dal.Add(md);
                    Lmodel.id = Ldal.GetMaxID() + 1;
                    Lmodel.login_state = 1;
                    Lmodel.login_time = DateTime.Now;
                    Lmodel.openid = md.openid;
                    Ldal.Add(Lmodel);
                }
                catch (Exception ex)
                {

                    session.Logger.Error(ex);



                }




                return md;
            }
            else
            {
                try
                {
                    Lmodel.id = Ldal.GetMaxID() + 1;
                    Lmodel.login_state = 1;
                    Lmodel.login_time = DateTime.Now;
                    Lmodel.openid = md.openid;
                    Ldal.Add(Lmodel);
                }
                catch (Exception)
                {

                    throw;
                }

                return model;
            }

        }




    }
}
