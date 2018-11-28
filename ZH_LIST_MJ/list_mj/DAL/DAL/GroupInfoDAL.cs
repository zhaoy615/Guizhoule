using DAL.DBHelp;
using DAL.Model;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAL.DAL
{
    public class GroupInfoDAL
    {

        public IEnumerable<int> GetGroupIDByUserID(long userID,int type)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "select GroupID from groupinfo_tb where CreateUserID=@CreateUserID and isExist=0 and type=@type";
                return Conn.Query<int>(sql, new { CreateUserID = userID, type= type });
            }
        }

        /// <summary>
        /// 根据群id获取群信息
        /// </summary>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        public GroupInfo GetGroupInfoByGroupID(long GroupID,int type)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "select * from groupinfo_tb where GroupID=@GroupID and isExist=0 and type=@type";
                return Conn.Query<GroupInfo>(sql, new { GroupID = GroupID, type = type }).FirstOrDefault<GroupInfo>();
            }
        }

        /// <summary>
        /// 根据userid获取群id
        /// </summary>
        /// <param name="GroupUserID"></param>
        /// <returns></returns>
        public List<long> GetGroupIDListByUserID(long GroupUserID,int type)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = @"select GroupID from groupinfo_tb where CreateUserID=@CreateUserID and isExist=0 and type=@type
                            union all
                         select GroupID from groupstaffinfo_tb where GroupUserID = @GroupUserID ";
                //string sql = @"select GroupID from groupinfo_tb where CreateUserID=@CreateUserID and isExist=0";
                return Conn.Query<long>(sql, new { GroupUserID = GroupUserID, CreateUserID = GroupUserID, type= type }).ToList<long>();
            }
        }

        public int GetGroupPeopleNumber(long groupID)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = @"
                         select count(0) from groupstaffinfo_tb where GroupID = @GroupID ";
                //string sql = @"select GroupID from groupinfo_tb where CreateUserID=@CreateUserID and isExist=0";
                return Conn.Query<int>(sql, new { GroupID = groupID }).FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据groupid获取群userid列表
        /// </summary>
        /// <returns></returns>
        public List<long> GetUsersIDListByGroupID(long GroupID)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "select GroupUserID from groupstaffinfo_tb where GroupID=@GroupID ";
                return Conn.Query<long>(sql, new { GroupID = GroupID }).ToList<long>();
            }
        }

        /// <summary>
        /// 判断用户是否在圈子里
        /// </summary>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        public int IsUserInGroup(long GroupID, long GroupUserID)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "select  COUNT(*) from groupstaffinfo_tb where GroupID=@GroupID and  GroupUserID=@GroupUserID";
                return Conn.QueryFirstOrDefault<int>(sql, new { GroupID = GroupID, GroupUserID = GroupUserID });
            }
        }


        /// <summary>
        /// 根据userid删除圈内用户
        /// </summary>
        /// <returns></returns>
        public int DelUsersByUserID(long GroupID, long GroupUserID)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "DELETE FROM groupstaffinfo_tb WHERE GroupID=@GroupID and GroupUserID=@GroupUserID";
                return Conn.Execute(sql, new { GroupID = GroupID, GroupUserID = GroupUserID });
            }
        }



        /// <summary>
        /// 根据userid删除申请记录
        /// </summary>
        /// <returns></returns>
        public int DelApplyByUserID(long GroupID, long ApplyJoinUserID)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "DELETE FROM groupapplyinfo_tb WHERE GroupID=@GroupID and ApplyJoinUserID=@ApplyJoinUserID";
                return Conn.Execute(sql, new { GroupID = GroupID, ApplyJoinUserID = ApplyJoinUserID });
            }
        }

        /// <summary>
        /// 添加用户到圈子
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="GroupUserID"></param>
        /// <param name="AddType"></param>
        /// <returns></returns>
        public int AddUserToGroup(long GroupID, long GroupUserID, int AddType)
        {
            int r = 0;
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                Conn.Open();
                IDbTransaction trans = Conn.BeginTransaction();
                //string sql = "INSERT INTO groupstaffinfo_tb (GroupID,GroupUserID,AddTime,AddType) VALUES(@GroupID,@GroupUserID,@AddTime,@AddType)";
                try
                {
                    string sql = "insert into groupstaffinfo_tb(GroupID, GroupUserID, AddTime, AddType) VALUES(@GroupID, @GroupUserID, NOW(), @AddType)";
                    r = Conn.Execute(sql, new { GroupID = GroupID, GroupUserID = GroupUserID, AddType = AddType }, trans);
                    string sql1 = "INSERT group_tips (groupID,userID,type,datetime) VALUES(@groupID,@userID,@type,@datetime)";
                    r += Conn.Execute(sql1, new { groupID = GroupID, userID = GroupUserID, type = 1, datetime = DateTime.Now }, trans);
                    trans.Commit();
                }
                catch
                {
                    if (r != 0)
                        trans.Rollback();
                    r = 0;
                }
                finally
                {
                    Conn.Close();
                }
            }
            return r;
        }
        public long QueryGroupIdByUserid(long userId)
        {
            long groupId = 0;
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
               
                try
                {
                    Conn.Open();

                    //string sql = "select count(*) from groupapplyinfo_tb where ApplyJoinUserID=@ApplyJoinUserID and GroupID=@GroupID and ApplyStatus=@ApplyStatus";
                    //return Conn.QueryFirstOrDefault<int>(sql, new { ApplyJoinUserID = ApplyJoinUserID, GroupID = GroupID, ApplyStatus = ApplyStatus });
                    string sql = "select groupID from group_tips where userID = @userId";

                }
                catch
                {

                }
                finally
                {
                    Conn.Close();
                }
            }
            return groupId;
        }



        /// <summary>
        /// 申请加入
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public int ApplyToJoin(long GroupID, long UserID)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                // string sql = "insert into groupstaffinfo_tb(GroupID,ApplyJoinUserID,ApplyDateTime,ApplyStatus) VALUES(@GroupID,@ApplyJoinUserID,NOW(),@ApplyStatus)";

                string sql = "insert into groupapplyinfo_tb (GroupID,ApplyJoinUserID,ApplyDateTime,ApplyStatus) VALUES (@GroupID,@ApplyJoinUserID,NOW(),@ApplyStatus)";
                return Conn.Execute(sql, new { GroupID = GroupID, ApplyJoinUserID = UserID, ApplyStatus = 0 });
            }
        }

        /// <summary>
        /// 玩家申请的入群记录
        /// </summary>
        /// <returns></returns>
        public List<ApplyRecord> PlayerApplyRecord(long ApplyJoinUserID)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "select a.* ,b.GroupName from groupapplyinfo_tb as a,groupinfo_tb as b  where  a.GroupID=b.GroupID and  a.ApplyJoinUserID=@ApplyJoinUserID";
                return Conn.Query<ApplyRecord>(sql, new { ApplyJoinUserID = ApplyJoinUserID }).ToList<ApplyRecord>();
            }
        }



        public int PlayerApplyRecord(long ApplyJoinUserID, long GroupID, int ApplyStatus)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "select count(*) from groupapplyinfo_tb where ApplyJoinUserID=@ApplyJoinUserID and GroupID=@GroupID and ApplyStatus=@ApplyStatus";
                return Conn.QueryFirstOrDefault<int>(sql, new { ApplyJoinUserID = ApplyJoinUserID, GroupID = GroupID, ApplyStatus = ApplyStatus });//   .ToList<ApplyRecord>();
            }
        }
        /// <summary>
        /// 申请入该群的玩家申请记录
        /// </summary>
        /// <returns></returns>
        public List<ApplyRecord> GroupApplyRecord(long GroupID)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "select a.* ,b.GroupName from groupapplyinfo_tb as a,groupinfo_tb as b  where a.GroupID=b.GroupID and  a.GroupID=@GroupID  and (ApplyStatus=0 or ApplyStatus=4)";
                return Conn.Query<ApplyRecord>(sql, new { GroupID = GroupID }).ToList<ApplyRecord>();
            }
        }

        /// <summary>
        /// 申请通过或拒绝
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="ApplyJoinUserID"></param>
        /// <param name="ApplyStatus"></param>
        /// <returns></returns>
        public int AgreeApplyStatus(long GroupID, long ApplyJoinUserID, int ApplyStatus,int oldApplyStatus=4)
        {
            int r = 0;
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                Conn.Open();
                IDbTransaction trans = Conn.BeginTransaction();
                try
                {
                    string sql = "update groupapplyinfo_tb set ApplyStatus=@ApplyStatus where GroupID=@GroupID and ApplyJoinUserID=@ApplyJoinUserID and ApplyStatus=@oldApplyStatus";
                    r += Conn.Execute(sql, new { GroupID = GroupID, ApplyJoinUserID = ApplyJoinUserID, ApplyStatus = ApplyStatus, oldApplyStatus= oldApplyStatus }, trans);
                    if (r !=0)
                    {
                        string sql1 = "INSERT group_tips (groupID,userID,type,datetime) VALUES(@groupID,@userID,@type,@datetime)";
                        //类型：1加入提示，2房主拒绝申请加入提示,3房主踢出提示,5同意退出提示，6房主拒绝退出房间提示
                        r += Conn.Execute(sql1, new { groupID = GroupID, userID = ApplyJoinUserID, type = ApplyStatus, datetime = DateTime.Now }, trans);
                    }
                        trans.Commit();
                }
                catch (Exception ex)
                {
                    if (r != 0)
                        trans.Rollback();
                    throw ex;
                }
                finally
                {
                    Conn.Close();
                }
                return r;
            }
        }

        /// <summary>
        /// 获取是否有这种申请状态
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="ApplyJoinUserID"></param>
        /// <param name="ApplyStatus"></param>
        /// <returns></returns>
        public int GetIsExistenceApplyStatus(long GroupID, long ApplyJoinUserID, int ApplyStatus)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "select 1 from groupapplyinfo_tb where GroupID=@GroupID and ApplyJoinUserID=@ApplyJoinUserID and ApplyStatus=@ApplyStatus";
                return Conn.QueryFirstOrDefault<int>(sql, new { GroupID = GroupID, ApplyJoinUserID = ApplyJoinUserID, ApplyStatus = ApplyStatus });
            }
        }
        /// <summary>
        /// 获取是否有这种申请状态
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="ApplyJoinUserID"></param>
        /// <param name="ApplyStatus"></param>
        /// <returns></returns>
        public int GetIsExistenceInGroup(long GroupID, long ApplyJoinUserID, int ApplyStatus)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "select 1 from groupstaffinfo_tb where GroupID=@GroupID and GroupUserID=@ApplyJoinUserID ";
                return Conn.QueryFirstOrDefault<int>(sql, new { GroupID = GroupID, ApplyJoinUserID = ApplyJoinUserID });
            }
        }
        /// <summary>
        /// 修改申请状态
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="ApplyJoinUserID"></param>
        /// <param name="ApplyStatus"></param>
        /// <returns></returns>
        public int ChangeApplyStatus(long GroupID, long ApplyJoinUserID, int ApplyStatus)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "update groupapplyinfo_tb set ApplyStatus=@ApplyStatus where GroupID=@GroupID and ApplyJoinUserID=@ApplyJoinUserID and ApplyStatus=1";
                return Conn.Execute(sql, new { GroupID = GroupID, ApplyJoinUserID = ApplyJoinUserID, ApplyStatus = ApplyStatus });
            }
        }


        public List<CreateRoomRecord> GetCreateRoomRecord(long CreateUserID, long GroupID)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "	SELECT * from  group_log where CreateUserID=@CreateUserID and GroupID=@GroupID  order by CreateDate ASC limit 10";
                return Conn.Query<CreateRoomRecord>(sql, new { CreateUserID = CreateUserID, GroupID = GroupID }).ToList<CreateRoomRecord>();
            }
        }
        public int AddCreateRoomRecord(long CreateUserID, long GroupID,long roomid,long id)
        {
           
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "INSERT group_log (GuoupID,RoomID,CreatUserID,,CreateDate,UserRoomCard,id) VALUES(@)";
                return Conn.Execute(sql, new { GuoupID = CreateUserID, RoomID = roomid, CreateUserID = CreateUserID, CreateDate = DateTime.Now, UserRoomCard  = 1, id  = 1});
            }


        }

        /// <summary>
        /// 创建朋友圈
        /// </summary>
        /// <param name="GroupName"></param>
        /// <param name="CreateUserID"></param>
        /// <returns></returns>
        public int CreateGroup(string croupName, long createUserID,string createUserUnionID)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                DynamicParameters dp = new DynamicParameters();
                dp.Add("pCreateUserID", createUserID);
                dp.Add("pGroupName", croupName);
                dp.Add("pUnionID", createUserUnionID);
                dp.Add("pGroupID", "", DbType.Int32, ParameterDirection.Output);
                //string sql = @"insert into  groupinfo_tb set GroupName=@GroupName,CreateUserID=@CreateUserID,CreateTime=Now();select LAST_INSERT_ID()";
                //return Conn.ExecuteScalar<int>(sql, new { GroupName = GroupName, CreateUserID = CreateUserID });
                Conn.Execute("CreateGroup", dp, null, null, CommandType.StoredProcedure);
                return dp.Get<int>("pGroupID");
            }
        }


        /// <summary>
        /// 解散朋友圈
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="CreateUserID"></param>
        /// <returns></returns>
        public int DeleteGroup(long GroupID, long CreateUserID)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = @"UPDATE groupinfo_tb set  isExist=1 where GroupID=@GroupID  and CreateUserID=@CreateUserID";
                return Conn.ExecuteScalar<int>(sql, new { GroupID = GroupID, CreateUserID = CreateUserID });
            }
        }
        /// <summary>
        ///  添加朋友圈提示
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="userID"></param>
        /// <param name="tipsType">1加入提示，2同意退出提示,3房主踢出提示</param>
        /// <returns></returns>
        public int AddGroupTips(long groupID, long userID, int tipsType)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql1 = "INSERT group_tips (groupID,userID,type,datetime) VALUES(@groupID,@userID,@type,@datetime)";
                return Conn.Execute(sql1, new { groupID = groupID, userID = userID, type = tipsType, datetime = DateTime.Now });
            }
        }
        /// <summary>
        /// 根据userid删除圈内用户
        /// </summary>
        /// <param name="groupID">圈子ID</param>
        /// <param name="groupUserID">被删除的用户ID</param>
        /// <param name="applyStatus">删除状态</param>
        /// <param name="tipsUserID">通知的用户ID</param>
        /// <param name="isApply">是否为申请</param>
        /// <returns></returns>
        public int DelUsersByUserIDTransaction(long groupID, long groupUserID, int applyStatus, long tipsUserID, bool isApply = false)
        {
            int r = 0;

            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                Conn.Open();
                IDbTransaction trans = Conn.BeginTransaction();
                try
                {
                    if (!isApply)//如果是申请退出就不能删掉记录
                    {
                        //根据userid删除圈内用户
                        string sql = "DELETE FROM groupstaffinfo_tb WHERE GroupID=@GroupID and GroupUserID=@GroupUserID";
                        r += Conn.Execute(sql, new { GroupID = groupID, GroupUserID = groupUserID }, trans);
                    }
                    //修改申请状态
                    string sql2 = "update groupapplyinfo_tb set ApplyStatus=@ApplyStatus where GroupID=@GroupID and ApplyJoinUserID=@ApplyJoinUserID and ApplyStatus=1";
                    r += Conn.Execute(sql2, new { GroupID = groupID, ApplyJoinUserID = groupUserID, ApplyStatus = applyStatus }, trans);
                    if (r != 0)
                    {
                        string sql1 = "INSERT group_tips (groupID,userID,type,datetime) VALUES(@groupID,@userID,@type,@datetime)";
                        r += Conn.Execute(sql1, new { groupID = groupID, userID = groupUserID, type = applyStatus, datetime = DateTime.Now }, trans);
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    if (r > 0)
                        trans.Rollback();
                    r = 0;
                    throw ex;
                }
                finally
                {
                    Conn.Close();
                }
            }

            return r;
        }
        /// <summary>
        /// 根据userid删除圈内用户
        /// </summary>
        /// <param name="groupID">圈子ID</param>
        /// <param name="groupUserID">被删除的用户ID</param>
        /// <param name="applyStatus">删除状态</param>
        /// <param name="tipsUserID">通知的用户ID</param>
        /// <param name="isApply">是否为申请</param>
        /// <returns></returns>
        public int ApplyUsersOutByUserIDTransaction(long groupID, long groupUserID, int applyStatus, long tipsUserID, bool isApply = false)
        {
            int r = 0;

            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                Conn.Open();
                IDbTransaction trans = Conn.BeginTransaction();
                try
                {

                    //修改申请状态
                    string sql = "update groupapplyinfo_tb set ApplyStatus=@ApplyStatus where GroupID=@GroupID and ApplyJoinUserID=@ApplyJoinUserID and ApplyStatus=1";
                    r += Conn.Execute(sql, new { GroupID = groupID, ApplyJoinUserID = groupUserID, ApplyStatus = applyStatus }, trans);
                    if (r == 0)
                    {
                        string sql2 = "INSERT groupapplyinfo_tb (GroupID,ApplyJoinUserID,ApplyDateTime,ApplyStatus) VALUES(@GroupID,@ApplyJoinUserID, @ApplyDateTime,@ApplyStatus)";
                        r += Conn.Execute(sql2, new { GroupID = groupID, ApplyJoinUserID = groupUserID, ApplyStatus = applyStatus, ApplyDateTime = DateTime.Now }, trans);
                    }
                    if (r != 0)
                    {
                        string sql1 = "INSERT group_tips (groupID,userID,type,datetime) VALUES(@groupID,@userID,@type,@datetime)";
                        r += Conn.Execute(sql1, new { groupID = groupID, userID = groupUserID, type = applyStatus, datetime = DateTime.Now }, trans);
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    if (r > 0)
                        trans.Rollback();
                    r = 0;
                    throw ex;
                }
                finally
                {
                    Conn.Close();
                }
            }

            return r;
        }

        /// <summary>
        /// 圈主同意用户退出
        /// </summary>
        /// <param name="groupID">圈子ID</param>
        /// <param name="groupUserID">退出的用户ID</param>
        /// <param name="applyStatus">退出状态</param>
        /// <returns></returns>
        public int DelUsersByUserIDTransaction(long groupID, long groupUserID, int applyStatus)
        {
            int r = 0;
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                Conn.Open();
                IDbTransaction trans = Conn.BeginTransaction();
                try
                {
                    //根据userid删除圈内用户
                    string sql = "DELETE FROM groupstaffinfo_tb WHERE GroupID=@GroupID and GroupUserID=@GroupUserID";
                    r += Conn.Execute(sql, new { GroupID = groupID, GroupUserID = groupUserID }, trans);
                    if (r != 0)
                    {
                        string sql1 = "INSERT group_tips (groupID,userID,type,datetime) VALUES(@groupID,@userID,@type,@datetime)";
                        r += Conn.Execute(sql1, new { groupID = groupID, userID = groupUserID, type = applyStatus, datetime = DateTime.Now }, trans);
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    if (r > 0)
                        trans.Rollback();
                    r = 0;
                    throw ex;
                }
                finally
                {
                    Conn.Close();
                }
            }
            return r;
        }

        /// <summary>
        /// 根据groupid获取群userid列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GroupStaffInfo> GetGroupStaffInfoByGroupID(long GroupID)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "select * from groupstaffinfo_tb where GroupID=@GroupID ";
                return Conn.Query<GroupStaffInfo>(sql, new { GroupID = GroupID });
            }
        }
        /// <summary>
        /// 根据groupid获取群userid列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetUpdateTipsByUserID(long userID)
        {
            using (var Conn = new MySqlConnection(DbHelperMySQL.connectionString))
            {
                string sql = "select * from group_tips where userID=@userID and  whetherTips=0";
                var list = Conn.Query(sql, new { userID = userID });
                List<string> rList = new List<string>();

                if (list.Any())
                {
                    string sql1 = "update group_tips SET whetherTips=1 where userID=@userID";
                    Conn.Execute(sql1, new { userID = userID });
                    var groupList = list.GroupBy(w => new { w.groupID, w.userID, w.type });
                    foreach (var item in groupList)
                    {
                        rList.Add(GetStringByMessage(item.Key.type, item.Key.groupID));
                    }                  
                }
                return rList;

            }
        }
        public string GetStringByMessage(int type,int groupID)
        {
            string messgae = string.Empty;
            switch(type)
            {
                case 1:
                    messgae = string.Format("加入朋友圈{0}成功！", groupID);//，2房主拒绝申请加入提示,3房主踢出提示，4申请退出圈子,,5同意退出提示，6房主拒绝退出房间提示
                    break;
                case 2:
                    messgae = string.Format("加入朋友圈{0}失败！", groupID); //"加入失败";，2房主拒绝申请加入提示,3房主踢出提示，4申请退出圈子,,5同意退出提示，6房主拒绝退出房间提示
                    break;
                case 3:
                    messgae = string.Format("您已退出朋友圈{0}！", groupID);// "";，2房主拒绝申请加入提示,3房主踢出提示，4申请退出圈子,,5同意退出提示，6房主拒绝退出房间提示
                    break;
                case 5:
                    messgae = string.Format("您已退出朋友圈{0}！", groupID);// "您已退出";，2房主拒绝申请加入提示,3房主踢出提示，4申请退出圈子,,5同意退出提示，6房主拒绝退出房间提示
                    break;
                case 6:
                    messgae = string.Format("您申请退出朋友圈{0}被拒绝！", groupID);// "您已退出";，2房主拒绝申请加入提示,3房主踢出提示，4申请退出圈子,,5同意退出提示，6房主拒绝退出房间提示
                    break;
            }
            return messgae;
        }
    }
}
