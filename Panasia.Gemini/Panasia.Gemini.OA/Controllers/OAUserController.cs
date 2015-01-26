/*----------------------------------------------------------------
// Copyright (C) 2004 清风软件
// 版权所有。
//
// 文件名：UserController.cs
// 文件功能描述：Fl000控制器,用户登录登出功能。
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Gemini.FL.Data.Common;
using System.Web.Security;
using Panasia.Core.Sys;
using Panasia.Core.Auth.Models;

namespace Panasia.Gemini.FL.Controllers
{
    public class OAUserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Fl000()
        {
            return View();
        }

        public ActionResult Login(Panasia.Gemini.FL.Models.LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var loginModel = new LoginViewModel { UserName = model.UserName, Password = model.Password };

                if (!loginModel.WebLogin(false))
                {
                    ModelState.AddModelError("", Messages.System_InvalidUserNameOrPassword);
                    this.AddSystemLog("登录失败", "Account/Login", Messages.System_InvalidUserNameOrPassword, model.UserName);
                    return View(model); 
                }
                else
                {
                    UserSession session = new UserSession();
                    session.UserModel = SysService.GetUserModel(model.UserName);
                    session.Roles = SysService.GetRoles();
                    Session["userSession"] = session;
                    this.AddSystemLog("登录成功", "Account/Login", "UserName:" + model.UserName, session.UserModel.FullName);

                    System.Data.DataSet dsLoginInfo = new Data.Common.sys_User().Login(model.UserName, GetMd5_32(model.Password));
                    if (dsLoginInfo != null && dsLoginInfo.Tables[0].Rows.Count == 1)
                    {
                        HttpCookie login = new HttpCookie("login");
                        System.Data.DataRow row = dsLoginInfo.Tables[0].Rows[0];
                        foreach (System.Data.DataColumn col in dsLoginInfo.Tables[0].Columns)
                        {
                            login.Values.Add(col.ColumnName, HttpUtility.UrlEncode(row[col.ColumnName].ToString()));
                        }

                        login.Expires = DateTime.Now.AddDays(1);
                        Response.Cookies.Add(login);

                        Response.Redirect("~/");
                    }
                    else
                    {
                        ViewBag.LoginError = true;
                    }
                    return null;
                }
            }

            //if (ValidateUser(model.UserName, model.Password))
            //{
            //    QX(model.UserName);
            //    return RedirectToAction("Rc006", "Interview_Info");
            //}
            return View("Fl000");
        }

        public ActionResult Logout()
        {
            HttpCookie login = Request.Cookies["login"];
            if (login != null)
            {
                login.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(login);
            }
            Session.Clear();
            return View("Fl000");
        }
        public  ActionResult MessagesCount()//消息总数
        {
            string id = Panasia.Gemini.FL.LoginInfo.UserID;
            if (id != null)
            {
                using (Panasia.Gemini.FL.Data.Common.SysContext db = new Panasia.Gemini.FL.Data.Common.SysContext())
                {
                    db.Database.Connection.Open();
                    var cmd = db.Database.Connection.CreateCommand();
                    cmd.CommandText = "select * FROM [dbo].[hr_fl_MessageRemind]('0'," + "'" + id + "'" + ")";
                    List<UserMessage> ls = new List<UserMessage>();
                    using (var reader = cmd.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                    {
                        while (reader.Read())
                        {
                            UserMessage um = new UserMessage();
                            um.MsgTitle = reader["MsgTitle"].ToString();
                            um.MsgType = reader["MsgType"].ToString();
                            um.Count = Convert.ToInt32(reader["Count"]);
                            um.MsgUrl = reader["MsgUrl"].ToString();
                            ls.Add(um);
                        }
                        reader.Close();
                    }
                    db.Database.Connection.Close();
                    foreach (var l in ls)
                    {
                        var formid=Convert.ToInt32(l.MsgType);
                        var dataform = db.fl_Form.Where(f => f.FormID.Equals(formid)).SingleOrDefault().DataFrom;
                        l.MsgType = dataform;
                    }
                    return Json(ls,JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return null;
            }
        }

        public ActionResult GetSideBar()
        {
            using (Panasia.Gemini.FL.Data.Common.SysContext db = new Panasia.Gemini.FL.Data.Common.SysContext())
            {
                string id = Panasia.Gemini.FL.LoginInfo.EmployeeID;
                if (id != null)
                {
                    var sidbar = (from fl_as in db.fl_ApproveState
                                join form in db.fl_Form on fl_as.FormID equals form.FormID
                                where
                                    fl_as.EmployeeID.Equals(id)
                                select new { FormName = form.FormName, FormID = form.FormID,DataForm=form.DataFrom }).Distinct().ToList();
                    return Json(sidbar, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
        }
        private string GetMd5_32(string sourceString)
        {
            string cl = sourceString;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                sb.Append(s[i].ToString("X"));
            }
            return sb.ToString();
        }
    }
}
