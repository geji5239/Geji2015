using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using Panasia.Core;
using Panasia.Core.Sys;
using System.Data.SqlClient;
using System.Web;
using System.Net.Mail;


namespace Panasia.Core.Sys
{
    public class UserDatagrid
    {
      public  int total;
      public  List<UserModel> rows;
    }
    public class RoleDatagrid
    {
        public int total;
        public List<RoleModel> rows;
    }
    public static class SysService
    {
        private static object _UserLock = new object();
        private static Dictionary<string, Dictionary<string, RolePageModel>> _UserPages = new Dictionary<string, Dictionary<string, RolePageModel>>();

        public static UserPrincipal GetCurrentUser()
        {
            if (HttpContext.Current == null || HttpContext.Current.User == null)
            {
                return null;
            }

            return HttpContext.Current.User as UserPrincipal;
        }

        #region 用户相关
        #region 登录
        /// <summary>
        /// 登录，如果成功则用户信息写入Cookie
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool WebLogin(this LoginViewModel model,bool redirect=true)
        {
            var userName = model.UserName;
            var user = GetUser(userName);

            if (user != null && user.IsValid && user.Password.Equals(model.Password.GetMd5_32()))
            {
                bool isPersistent = model.RememberMe;
                UserModel userModel = new UserModel
                {
                    UserID = user.UserID,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName
                };

                string userData = userModel.ToJson();

                var response = HttpContext.Current == null ? null : HttpContext.Current.Response;
                if (response == null)
                {
                    throw new Exception("此方法只能Web调用");
                }

                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                    1,
                    user.UserName,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(30),
                    isPersistent,
                    userData,
                    FormsAuthentication.FormsCookiePath
                    );
                string encTicket = FormsAuthentication.Encrypt(authTicket);
                response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                if (redirect)
                {
                    response.Redirect(FormsAuthentication.GetRedirectUrl(user.UserName, isPersistent));
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 查询用户列表并分页
        public static UserDatagrid GetUsersWithPage(string nameOrEmail, bool isValid, string email, string fullname,string page,string rows)
        {
            var query = GetUsers(nameOrEmail, isValid, email, fullname);
            UserDatagrid ud = new UserDatagrid();
            ud.total = query.Count;
            query = query.OrderByDescending(u => u.UserID).Skip((page.ToInt32() - 1) * rows.ToInt32()).Take(rows.ToInt32()).ToList();
            ud.rows = query;
            
            return ud;
        }
        #endregion

        #region 查询用户列表
        public static List<UserModel> GetUsers(string nameOrEmail, bool isValid, string email, string fullname)
        {
            using (var db = SysContext.GetCurrent())
            {
                Func<User, bool> nameWhere = (u) => true;
               
                var query = db.Users.Where(u => u.IsValid == isValid&&u.UserName!="admin").ToList();
                var user = SysService.GetCurrentUser();
                if (user != null&&user.UserID!="U00001")
                {
                    var companyfilter = ((from u in db.Users
                               join sr in db.UserRoles on u.UserID equals sr.UserID
                               join rp in db.RolePages on sr.RoleID equals rp.RoleID
                               where (u.UserID.Equals(user.UserID) && rp.PageID.Equals("P01001"))
                               select rp.DataFilter.ToString()).Distinct()).ToList();
                    var deptfilter = ((from u in db.Users
                                       join sr in db.UserRoles on u.UserID equals sr.UserID
                                       join rp in db.RolePages on sr.RoleID equals rp.RoleID
                                       where (u.UserID.Equals(user.UserID) && rp.PageID.Equals("P01002"))
                                       select rp.DataFilter.ToString()).Distinct()).ToList();
                    List<string> eu =new List<string>();
                    
                    eu = (from e in db.hr_Employees where (deptfilter.Contains(e.DeptID)||deptfilter.Contains(e.CompanyID))select e.UserID).Distinct().ToList();

                    if (eu.Count != 0)
                    {
                        query = query.Where(u => eu.Contains(u.UserID)).ToList();
                    }
                   
                }
                if (!string.IsNullOrEmpty(nameOrEmail) || !string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(fullname))
                {
                    query = string.IsNullOrEmpty(nameOrEmail) ? query : query.Where(u => u.UserName.Contains(nameOrEmail)).ToList();
                    query = string.IsNullOrEmpty(email) ? query : query.Where(u => u.Email.Contains(email)).ToList();
                    query = string.IsNullOrEmpty(fullname) ? query : query.Where(u => u.FullName.Contains(fullname)).ToList();
                }

               // UserDatagrid ud = new UserDatagrid();

              //  var demo = "";
                return query
                    .Select((u) => new UserModel
                    {
                        UserID = u.UserID,
                        UserName = u.UserName,
                        FullName = u.FullName,
                        Email = u.Email,
                        Roles = (from r in db.Roles join s in db.UserRoles on r.RoleID equals s.RoleID where s.UserID.Equals(u.UserID) select r.Name.ToString()).ToArray(),
                        Company=(from em in db.hr_Employees join com in db.hr_Companies on em.CompanyID equals com.CompanyID where em.UserID.Equals(u.UserID) select com.Name).FirstOrDefault(),
                        Dept = (from em in db.hr_Employees join dept in db.hr_Depts on em.DeptID equals dept.DeptID where em.UserID.Equals(u.UserID) select dept.Name.ToString()).FirstOrDefault(),
                        Job = (from em in db.hr_Employees join job in db.hr_Jobs on em.JobID equals job.JobID where em.UserID.Equals(u.UserID) select job.Name.ToString()).FirstOrDefault(),
                        IsValid = u.IsValid
                    }).ToList();
            }
        }
        #endregion

        #region 根据ID或用户名取用户信息
        public static UserModel GetUserModel(string userIDorName)
        {
            using (var db = SysContext.GetCurrent())
            {
                var query = db.Users.Where(u => u.UserID.Equals(userIDorName)||u.UserName.Equals(userIDorName,StringComparison.OrdinalIgnoreCase));
            //    var user = db.Users.FirstOrDefault(u => (u.UserID.Equals(userIDorName)
             //       || u.UserName.Equals(userIDorName, StringComparison.OrdinalIgnoreCase)));
                var user=  query.Select((u) => new UserModel
                  {
                      UserID = u.UserID,
                      UserName = u.UserName,
                      FullName = u.FullName,
                      Email = u.Email,
                      IsValid = u.IsValid
                  }).ToList().FirstOrDefault();
                if (userIDorName.IndexOf("U") != -1)
                {
                    user.Roles = (from r in db.Roles join s in db.UserRoles on r.RoleID equals s.RoleID where s.UserID.Equals(userIDorName) select r.Name).ToArray();
                }
                return user;
            }
        }
        #endregion

        #region 新增用户
        public static UserModel CreateUser(string userName, string email, string fullName, string password)
        {
            lock (_UserLock)
            {
                using (var db = SysContext.GetCurrent())
                {
                    var oldUser = db.Users.FirstOrDefault(u => u.IsValid
                        && (u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                        || u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));

                    if (oldUser != null)
                    {
                        throw new Exception("重复的用户名或邮件");
                    }
                    var userID = db.GetNextUserID();

                    User newUser = new User
                    {
                        UserID = userID,
                        Email = email,
                        UserName = userName,
                        FullName = fullName,
                        IsValid = true,
                        Password = password.GetMd5_32()
                    };
                    newUser.ResetCreated();
                    db.Users.Add(newUser);
                    db.SaveChanges();
                    return new UserModel
                    {
                        UserID = newUser.UserID,
                        UserName = newUser.UserName,
                        Email = newUser.Email,
                        IsValid = newUser.IsValid,
                        FullName = newUser.FullName
                    };
                }
            }
        }
        #endregion

        #region 修改用户
        public static UserModel UpdateUser(UserModel user)
        {
            lock (_UserLock)
            {
                using (var db = SysContext.GetCurrent())
                {
                    var oldUser = db.Users.FirstOrDefault(u => u.UserID.Equals(user.UserID));

                    if (oldUser == null)
                    {
                        throw new Exception("用户不存在");
                    }

                    oldUser.UserName = user.UserName;
                    oldUser.FullName = user.FullName;
                    oldUser.Email = user.Email;
                    oldUser.ResetUpdated();
                    db.SaveChanges();
                    return user;
                }
            }
        }
        #endregion

        #region 删除用户
        public static void DeleteUsers(string userKeys)
        {
            lock (_UserLock)
            {
                using (var db = SysContext.GetCurrent())
                {
                    var userIDs = userKeys.Split(',');
                    var oldUsers = db.Users.Where(u => userIDs.Contains(u.UserID));
                    foreach (var oldUser in oldUsers)
                    {
                        oldUser.IsValid = false;
                        oldUser.ResetUpdated();
                    }
                    db.SaveChanges();
                }
            }
        }

        public static void DeleteUser(string userID)
        {
            lock (_UserLock)
            {
                using (var db = SysContext.GetCurrent())
                {
                    var oldUser = db.Users.FirstOrDefault(u => u.UserID.Equals(userID));
                    if (oldUser != null)
                    {
                        oldUser.IsValid = false;
                        oldUser.ResetUpdated();
                        db.SaveChanges();
                    }
                }
            }
        }
        #endregion

        #region 重置用户密码
        public static void ResetPassword(string userID)
        {
            lock (_UserLock)
            {
                using (var db = SysContext.GetCurrent())
                {
                    var oldUser = db.Users.FirstOrDefault(u => u.UserID.Equals(userID));
                    if (oldUser != null)
                    {
                        oldUser.Password = Sys.SystemConsts.DefaultPassword.GetMd5_32().ToString();
                        oldUser.ResetUpdated();
                        db.SaveChanges();
                    }
                }
            }
        }
        #endregion

        #region 修改用户密码
        public static bool ChangePassword(string oldpassword, string password)
        {
                using (var db = SysContext.GetCurrent())
                {
                    string id = SysService.GetCurrentUser().UserID;
                    var oldUser = db.Users.FirstOrDefault(u => u.UserID.Equals(id));
                    var op = oldpassword.GetMd5_32();
                    var np = password.GetMd5_32();
                    if (oldUser != null && oldUser.Password == op)
                    {
                        oldUser.Password = np;
                        oldUser.ResetUpdated();
                        db.SaveChanges();
                        return true;
                    }
                    else { 
                     return false;
                    }
            }
        }
        #endregion

        #region 取用户页面
        [Export(SystemConsts.Func_GetCurrentUserPageActionValue,typeof(Func<string,int>))]
        public static int GetCurrentUserPageActionValue(string pageID)
        {
            var user = SysService.GetCurrentUser();
            if(user==null)
            {
                return 0;
            }
            if (user.UserID.Equals("U00001"))
            {
                return 0x7FFFFFFF;
            }
            var userPage = GetUserPage(user.UserID, pageID);
            return userPage == null ? 0 : userPage.ActionValue;
        }

        public static RolePageModel GetUserPage(string userID, string pageID)
        {
            var pages = _UserPages.GetDictionaryValue(userID, null);
            if (pages == null)
            {
                lock (_UserPages)
                {
                    if (!_UserPages.ContainsKey(userID))
                    {
                        var userPages = GetUserPages(userID);
                        pages = userPages.ToDictionary<RolePageModel, string>(m => m.PageID);
                        _UserPages.Add(userID, pages);
                    }
                    else
                    {
                        pages = _UserPages.GetDictionaryValue(userID, null);
                    }
                }
            }
            return pages.GetDictionaryValue(pageID, null);
        }
        #endregion

        #region 取用户所有页面
        //如果同一个页面的某个功能，A角色有权限，B角色没有权限，但某用户A、B角色都有，那该用户应该有权
        public static List<RolePageModel> GetUserPages(string userID)
        {
            using (var db = SysContext.GetCurrent())
            {
                var items = (from ur in db.UserRoles
                             join rp in db.RolePages on ur.RoleID equals rp.RoleID
                             where ur.UserID.Equals(userID)
                             select new RolePageModel
                             {
                                 RoleID = rp.RoleID,
                                 PageID = rp.PageID,
                                 ActionValue = rp.ActionValue
                             }).ToList();

                db.RolePages.Where(rp => rp.RoleID.Equals(userID)).
                    Select((rp) => new RolePageModel
                            {
                                RoleID = rp.RoleID,
                                PageID = rp.PageID,
                                ActionValue = rp.ActionValue
                            }).ToList().AddToCollection(items);

                var groups = items.GroupBy(g => g.PageID).Select(g => new RolePageModel
                {
                    PageID = g.Key,
                    ActionValue = g.Count() > 1 ? g.Select(s => s.ActionValue).Aggregate((v1, v2) => v1 | v2) : g.First().ActionValue,
                    DataFilter = g.Select(r=>r.DataFilter).AggregateSplitStrings(",")
                }).ToList();

                return groups;
            }
        }
        #endregion

        #region 用户权限
        public static bool IsActionAllowed(string userID, string pageID, int actionID)
        {
            if (userID.Equals("U00001"))
            {
                return true;
            }
            var item = GetUserPage(userID, pageID);
            if (item == null)
            {
                return false;
            }
            return (item.ActionValue & actionID) == actionID;
        }
        #endregion

        internal static User GetUser(string userName)
        {
            using (var db = SysContext.GetCurrent())
            {

                return db.Users.FirstOrDefault(u => (u.UserID.Equals(userName)
                    || u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)&&u.IsValid.Equals(true)));
            }
        }

        internal static UserModel CreateUserModel(this User user)
        {
            return new UserModel
            {
                UserID = user.UserID,
                UserName = user.UserName,
                Email = user.Email,
                IsValid = user.IsValid,
                FullName = user.FullName
            };
        }
        #endregion

        #region 角色相关

        public static List<RoleModel> GetRoles()
        {
            using (var db = SysContext.GetCurrent())
            {
                return db.Roles.Select(r => new RoleModel
                {
                    RoleID = r.RoleID,
                    Name = r.Name,
                    Description = r.Description
                    
                }).ToList();
            }
        }

        public static RoleModel GetRole(string roleID)
        {
            using (var db = SysContext.GetCurrent())
            {
                return db.Roles.Where(r=>r.RoleID.Equals(roleID, StringComparison.OrdinalIgnoreCase))
                    .Select(r => new RoleModel
                {
                    RoleID = r.RoleID,
                    Name = r.Name,
                    Description = r.Description
                }).FirstOrDefault();
            }
        }

        public static RoleDatagrid GetRolesWithPage(string searchName, string description, string page, string rows)
        {
            var query = GetRoles(searchName, description);
            query=(from r in query where (r.Name.Contains(searchName) && r.Description.Contains(description))  select r).ToList();
            RoleDatagrid rd = new RoleDatagrid();
            rd.total = query.Count;
            query = query.OrderByDescending(u => u.RoleID).Skip((page.ToInt32()-1 )* rows.ToInt32()).Take(rows.ToInt32()).ToList();
            rd.rows = query;
            return rd;
        }
        public static List<RoleModel> GetRoles(string searchName,string description)
        {
            using (var db = SysContext.GetCurrent())
            {
                var user = SysService.GetCurrentUser();
                if (user.UserID == "U00001")
                {
                  return  GetRoles();
                }
                else
                {
                    return (from r in db.Roles join ur in db.UserRoles on r.RoleID equals ur.RoleID where ur.UserID.Equals(user.UserID) select r)
                                        .Select(r => new RoleModel
                                    {
                                        RoleID = r.RoleID,
                                        Name = r.Name,
                                        Description = r.Description
                                    }).ToList();
                }
                
            }
        }

        public static RoleModel CreateRole(string name, string description="")
        {
            using (var db = SysContext.GetCurrent())
            {
                
                var oldItem = db.Roles.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (oldItem != null)
                {
                    throw new Exception("角色已存在");
                }
                var newID = db.GetNextRoleID();
                Role role = new Role { RoleID = newID, Name = name, Description = description };
                role.ResetCreated();
                db.Roles.Add(role);                
                var user = SysService.GetCurrentUser();
                if (user.UserID != "U00001")
                {
                    UserRole ur = new UserRole
                    {
                        UserID = user.UserID,
                        RoleID = role.RoleID
                    };
                    ur.ResetCreated();
                    db.UserRoles.Add(ur);
                }
                db.SaveChanges();

                return new RoleModel { RoleID = newID, Name = name, Description = description };
            }
        }

        public static void UpdateRole(string roleID, string name, string description)
        {
            using (var db = SysContext.GetCurrent())
            {
                var oldItem = db.Roles.FirstOrDefault(r => r.RoleID.Equals(roleID, StringComparison.OrdinalIgnoreCase));
                if (oldItem == null)
                {
                    throw new Exception("角色不存在");
                }
                oldItem.Name = name;
                oldItem.Description = description;
                oldItem.ResetUpdated();
                db.SaveChanges();
            }
        }

        public static void DeleteRole(string roleID)
        {
            using (var db = SysContext.GetCurrent())
            {
                var oldItem = db.Roles.FirstOrDefault(r => r.RoleID.Equals(roleID, StringComparison.OrdinalIgnoreCase));
                if (oldItem == null)
                {
                    throw new Exception("角色不存在");
                }

                var count = db.UserRoles.Count(r => r.RoleID.Equals(roleID, StringComparison.OrdinalIgnoreCase));
                if (count > 0)
                {
                    throw new Exception("有用户关联此角色，删除取消.");
                }

                foreach (var item in db.RolePages.Where(r => r.RoleID.Equals(roleID)).ToList())
                {
                    db.RolePages.Remove(item);
                    db.SaveChanges();
                }
                oldItem.ResetUpdated();
                db.Roles.Remove(oldItem);
                
                db.SaveChanges();
            }
        }
        #endregion

        #region 角色页面
        public static IEnumerable<RolePageModel> GetRolePages(string roleID)
        {
            using (var db = SysContext.GetCurrent())
            {
                return db.RolePages.Where(w => w.RoleID.Equals(roleID, StringComparison.OrdinalIgnoreCase)).Select(w => new RolePageModel
                {
                    RoleID = roleID,
                    PageID = w.PageID,
                    DataFilter=w.DataFilter,
                    ActionValue = w.ActionValue
                }).ToList();;
                //return count.Select(w => new RolePageModel
                //{ 
                //    RoleID=roleID,
                //    PageID=w.PageID,
                //    DataFilter=w.DataFilter,
                //    ActionValue=w.ActionValue
                //}).ToList();
              //RolePage rolepage = list;


              
            }
        }

        public static void UpdateRolePages(string roleID, IEnumerable<RolePage> items)
        {
            using (var db = SysContext.GetCurrent())
            {
                var oldItems = db.RolePages.Where(w => w.RoleID.Equals(roleID)).ToList();

                var adds = items.Where(w => oldItems.FirstOrDefault(t => t.PageID.Equals(w.PageID)) == null).ToList();
                var removes = oldItems.Where(w => items.FirstOrDefault(t => t.PageID.Equals(w.PageID)) == null).ToList();
                var updates = oldItems.Where(w => items.FirstOrDefault(t => t.PageID.Equals(w.PageID)) != null).ToList();

                foreach (var t in removes)
                {
                    db.RolePages.Remove(t);
                }

                foreach (var t in updates)
                {
                    var newItem = items.FirstOrDefault(w => t.PageID.Equals(w.PageID));
                    var dataFilter = newItem.DataFilter;
                    if (newItem.DataFilter == null)
                    {
                        dataFilter = "";
                    }
                    if (newItem == null)
                    {
                        continue;
                    }
                    t.DataFilter = dataFilter;
                    t.ActionValue = newItem.ActionValue;
                    t.ResetUpdated();
                }

                foreach (var t in adds)
                {
                    var dataFilter=t.DataFilter;
                       if(t.DataFilter==null){
                           dataFilter = "";
                       }
                       var newItem = new RolePage
                       {
                           RoleID = roleID,
                           PageID = t.PageID,
                           ActionValue = t.ActionValue,
                           DataFilter = dataFilter
                       };
                    newItem.ResetCreated();
                    db.RolePages.Add(newItem);
                }

                //如果缓存了本角色相关的用户，则重置缓存
                var users = db.UserRoles.Where(r => r.RoleID.Equals(roleID)).Select(r => r.UserID).ToArray();
                if (users.Length > 0)
                {
                    lock (_UserPages)
                    {
                        users.ForEach(u =>
                        {
                            _UserPages.Remove(u);
                        });
                    }
                }

                db.SaveChanges();
            }
        }

        #endregion

        #region 用户角色

        public static string[] GetUserRoles(string userID)
        {
            using (var db = SysContext.GetCurrent())
            {
                return db.UserRoles.Where(r => r.UserID.Equals(userID)).Select(r => r.RoleID).ToArray();
            }
        }

        public static void SaveUserRole(string userID, string[] roles)
        {
            using (var db = SysContext.GetCurrent())
            {
                var oldItems = db.UserRoles.Where(r => r.UserID.Equals(userID));

                var adds = roles.Where(w => oldItems.FirstOrDefault(t => t.RoleID.Equals(w)) == null).ToList();
                var removes = oldItems.Where(w => roles.FirstOrDefault(t => t.Equals(w.RoleID)) == null).ToList();

                foreach (var t in removes)
                {
                    db.UserRoles.Remove(t);
                }

                foreach (var t in adds)
                {
                    var newItem = new UserRole
                    {
                        UserID = userID,
                        RoleID = t
                    };
                    newItem.ResetCreated();
                    db.UserRoles.Add(newItem);
                }
                db.SaveChanges();
            }
        }

        #endregion

        #region MD5加密

        internal static string GetMd5_16(this string sourceString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(sourceString)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }

        internal static string GetMd5_32(this string sourceString)
        {
            string cl = sourceString;
            StringBuilder sb = new StringBuilder();
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                sb.Append(s[i].ToString("X"));
            }
            return sb.ToString();
        }
        #endregion

        #region 消息通知
        public static string Messages()//逐条显示当前用户消息
        {
            string id = SysService.GetCurrentUser().UserID;
            //string str="server='10.1.15.235';database='PA_Sys';uid='pguser';pwd='sa123456'";
            //SqlConnection con = new SqlConnection(str);
            //con.Open();
            //string strsql = "select * FROM [dbo].[hr_fl_MessageRemind]('1'," + "'" + id + "'" + ")";
            //SqlCommand cmd = new SqlCommand(strsql, con);
            //SqlDataReader rd = cmd.ExecuteReader();
            using (var db = new SysContext())
            {
                db.Database.Connection.Open();
                var cmd = db.Database.Connection.CreateCommand();
                
                cmd.CommandText = "select * FROM [dbo].[hr_fl_MessageRemind]('1'," + "'" + id + "'" + ")";
                List<UserMessage> ls = new List<UserMessage>();
                using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                {
                    while (reader.Read())
                    {
                        UserMessage um = new UserMessage();
                        um.MsgTitle = reader["MsgTitle"].ToString();
                        um.Count = Convert.ToInt32(reader["Count"]);
                        um.MsgUrl = reader["MsgUrl"].ToString();
                        ls.Add(um);
                    }
                    reader.Close();
                }
                db.Database.Connection.Close();
                return ls.ToJson();
            }
        }
        public static int MessagesCount()//消息总数
        {
            string id = SysService.GetCurrentUser().UserID;
            using (var db = new SysContext()) {
                db.Database.Connection.Open();
                var cmd = db.Database.Connection.CreateCommand();
                cmd.CommandText = "select isnull(Counts,0) from (select sum(Count)as Counts FROM [dbo].[hr_fl_MessageRemind]('1'," + "'" + id + "'" + "))a";
                var a = cmd.ExecuteScalar();
                int mc=System.Int32.Parse(a.ToString());
                return (mc);
            }
        }
        #endregion


        #region 忘记密码
        public static string EmailId(string email)//根据用户填写的email获取用户id
        {
            using (var db = new SysContext())
            {
                db.Database.Connection.Open();
                var cmd = db.Database.Connection.CreateCommand();
                cmd.CommandText = "select UserID from sys_User where Email ='" + email + "'";
                var u = cmd.ExecuteScalar();
                if (u != null)
                {
                    string uid = u.ToString();
                    return uid;
                }
                else {
                    return null;
                };
                
            }
        }

        public static bool EmailReset(string uid)//邮件链接重置密码
        {
            lock (_UserLock)
            {
                using (var db = SysContext.GetCurrent())
                {
                    var oldUser = db.Users.FirstOrDefault(u => u.UserID.Equals(uid));
                    if (oldUser != null)
                    {
                        oldUser.Password = Sys.SystemConsts.DefaultPassword.GetMd5_32();
                        oldUser.ResetUpdated();
                        db.SaveChanges();
                    }
                }
            }
            return true;
        }
         public static string SendMail(string from, string password, string fromname, string smtp, string port, string chaosong, string misong, string to, string title, string body)
        {
            try
            {
                //发送类 
                MailMessage mail = new MailMessage();
                //是谁发送的邮件 
                mail.From = new MailAddress(from, fromname);
                //发送给谁 
                mail.To.Add(to);
                //标题 
                mail.Subject = title;
                //内容编码 
                mail.BodyEncoding = Encoding.Default;
                //发送优先级 
                mail.Priority = MailPriority.High;
                //内容 
                mail.Body = body;
                //是否HTML形式发送 
                mail.IsBodyHtml = true;
                //抄送
                //if (!string.IsNullOrEmpty(chaosong))
                //    mail.CC.Add(chaosong);
                //密送
                if (!string.IsNullOrEmpty(misong))
                    mail.Bcc.Add(misong);
                //附件 
                //if (fujian.Length > 0)
                //{
                //    mail.Attachments.Add(new Attachment(fujian));
                //}
                //服务器和端口 
                SmtpClient SMTP = new SmtpClient(smtp, int.Parse(port));
                SMTP.UseDefaultCredentials = true;
                //指定发送方式 
                SMTP.DeliveryMethod = SmtpDeliveryMethod.Network;
                //指定登录名和密码 
                SMTP.Credentials = new System.Net.NetworkCredential(from, password);
                ////超时时间 
                SMTP.Timeout = 10000;
                SMTP.Send(mail);
                return "OK";
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
        }

        #endregion
    }
}


