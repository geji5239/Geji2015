using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Core.Sys;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Panasia.Core.Web;
using Newtonsoft.Json;
using Panasia.Core.App;
using System.Reflection;

namespace Panasia.Core.Auth.Controllers
{
    public class UserController : Controller
    {
        [AuthAction("P00001", 1)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthAction("P00001", 2)]
        #region 新增用户
        public ActionResult Create()
        {
            return View();
        }

         [AuthAction("P00001", 2)]
        [HttpPost]
        public ActionResult Add(UserCreateModel model)
        {
            try
            {
                var user = Sys.SysService.CreateUser(model.UserName, model.Email, model.FullName, Sys.SystemConsts.DefaultPassword);
                return Json(ResultData.Create(user));
            }
            catch (Exception ex)
            {
                return Json(ResultData.CreateError(ex.Message), "text/html");
            }
        }
        #endregion

         [AuthAction("P00001", 4)]
        #region 修改用户
        public ActionResult Edit(string ID)
        {
            var model = SysService.GetUserModel(ID);
            return View(model);
        }

        [AuthAction("P00001", 4)]
        [HttpPost]
        public ActionResult Update(UserModel model)
        {
            try
            {
                var user = SysService.UpdateUser(model);
                return Json(ResultData.Create(user),"text/html");
            }
            catch (Exception ex)
            {
                return Json(ResultData.CreateError(ex.Message), "text/html");
            }
        }
        #endregion

        [AuthAction("P00001", 8)]
        #region 查看用户
        public ActionResult Detail(string ID)
        {
            var model = SysService.GetUserModel(ID);
            return View(model);
        }
        #endregion

        [AuthAction("P00001", 16)]
        #region 删除用户
        public ActionResult Delete(string keys)
        {
            if(string.IsNullOrEmpty(keys))
            {
                return Json(ResultData.CreateError("主键为空，删除取消"));
            }
            if(keys.Contains(','))
            {
                SysService.DeleteUsers(keys);
            }
            else
            {
                SysService.DeleteUser(keys);
            }
            return Json(ResultData.Create(true), "text/html"); 
        }
        #endregion

        [AuthAction("P00001", 32)]
        #region 重置密码
        public ActionResult ResetPassword()
        {
            return View();
        }

        [AuthAction("P00001", 32)]
        [HttpPost]
        public ActionResult ResetPassword(string ID)
        {
            Sys.SysService.ResetPassword(ID);
            return Json(ResultData.Create(true), "text/html");
        }
        #endregion

        #region 修改密码
        
        public ActionResult ChangePassword()
        {
            return View();
            
        }
        public ActionResult UpdatePassword1(string oldpassword, string password)
        {
            //ResultData data = new ResultData();
            if (Sys.SysService.ChangePassword(oldpassword, password))
            {
                return Content("true");
            }else
            {
                return Content("false");
            }
        }
        #endregion

        [AuthAction("P00001", 64)]
        #region 检索
        public ActionResult Search()
        {
            return View();
        }
        [AuthAction("P00001", 64)]
        [HttpPost]
        public ActionResult Query()
        {
            string name = Request["name"];
            string email = Request["email"];
            string fullname = Request["fullname"];
            string page = Request["page"];
            string rows = Request["rows"];
            return Json(SysService.GetUsersWithPage(name, true, email, fullname,page,rows));
        }
        #endregion

        #region 导入
        public ActionResult Import()
        {
            return View();
        }

        
        /// <summary>
        /// UploadFile控件的保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFile()
        {
            int iTotal = Request.Files.Count;
            if (iTotal == 0)
            {
                return Json(ResultData.CreateError("没有数据"));
            }
            else
            {
                for (int i = 0; i < iTotal; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    if (file.ContentType != "text/csv" && file.ContentType != "application/vnd.ms-excel" && file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")//excel版本不同类型也不同2014-10-11 ysh
                    {
                        return Json(ResultData.CreateError("文件类型错误,只能导入.csv或者.xlsx文件"), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                    }
                    if (file.ContentLength > 0 || !string.IsNullOrEmpty(file.FileName))
                    {
                        //保存文件
                        string filePath = string.Format(HttpRuntime.AppDomainAppPath.ToString() + @"Upload");
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        string filename = Path.GetFileName(file.FileName);
                        string savepath = string.Format("{0}\\{1}", filePath, filename);
                        file.SaveAs(savepath);
                        var json = ImportToDb(savepath);
                        return json;
                    }
                }
            }
            return null;
        }

        [HttpPost]
        public ActionResult UploadImageFile()
        {
            int iTotal = Request.Files.Count;
            if (iTotal == 0)
            {
                return Json(ResultData.CreateError("没有数据"));
            }
            else
            {
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif", "bmp" };
                for (int i = 0; i < iTotal; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);

                    Random r = new Random();
                    string fileName = DateTime.Now.ToString("yyyyMMddHHmmss")+r.Next(10000)+"."+fileExt;
                    

                    if (!supportedTypes.Contains(fileExt))
                    {
                        return Json(ResultData.CreateError("文件类型错误"), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                    }
                    if (file.ContentLength > 0 || !string.IsNullOrEmpty(file.FileName))
                    {
                        //保存文件
                        string filePath = string.Format(HttpRuntime.AppDomainAppPath.ToString() + @"/EmployeeImg/Temp");
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        string filename = Path.GetFileName(fileName);
                        string savepath = string.Format("{0}\\{1}", filePath, filename);
                        file.SaveAs(savepath);
                        //var json = ImportToDb(savepath);
                        return Json(fileName,"text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 导入数据库成功或失败都需要记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public ActionResult ImportToDb(string fileName)
        {
            if (FileIsUsed(fileName))
            {
                return Json(ResultData.CreateError("文件正在使用中！"), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
            }
            //调用通用导入
            ExcelImportResult result = ExcelImportCommand.OnExecute(fileName);
            
            return Json(ResultData.Create(result), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
        }

        //判断文件存不存在和是否被占用
        public static Boolean FileIsUsed(String fileName)
        {
            Boolean result = false;

            //判断文件是否存在，如果不存在，直接返回 false
            if (!System.IO.File.Exists(fileName))
            {
                result = false;

            }//end: 如果文件不存在的处理逻辑
            else
            {//如果文件存在，则继续判断文件是否已被其它程序使用

                //逻辑：尝试执行打开文件的操作，如果文件已经被其它程序使用，则打开失败，抛出异常，根据此类异常可以判断文件是否已被其它程序使用。
                System.IO.FileStream fileStream = null;
                try
                {
                    fileStream = System.IO.File.Open(fileName, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);

                    result = false;
                }
                catch (System.IO.IOException ioEx)
                {
                    //记录日志
                    result = true;
                }
                catch (System.Exception ex)
                {
                    //记录日志
                    result = true;
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }

            }//end: 如果文件存在的处理逻辑

            //返回指示文件是否已被其它程序使用的值
            return result;

        }
        #endregion

        #region 导出
        public ActionResult Export()
        {
            return View();
        }

        [HttpPost]
        public void ExportUser()
        {
            var isValidate = string.Empty;
            var keys = Request.Form.AllKeys;
            foreach(var key in keys){
                isValidate = Request.Form[key];
            }
            List<UserModel> list = SysService.GetUsers("", Boolean.Parse(isValidate), "", "");
            StringBuilder sb = new StringBuilder();
            sb.Append("<style type=\"text/css\">");
            sb.Append("<!--");
            sb.Append(".text");
            sb.Append("{mso-style-parent:style0;");
            sb.Append("font-size:10.0pt;");
            sb.Append("font-family:\"Arial Unicode MS\", sans-serif;");
            sb.Append("mso-font-charset:0;");
            sb.Append(@"mso-number-format:\@;");
            sb.Append("text-align:center;");
            sb.Append("border:.5pt solid black;");
            sb.Append("white-space:normal;}");
            sb.Append("-->");
            sb.Append("</style>");
            sb.Append("<table cellspacing=\"0\" rules=\"all\" border=\"1\" style=\"border-collapse:collapse;\">");
            sb.Append("<tr align=\"Center\" style=\"font-weight:bold;\">");
            sb.Append("<td>用户ID</td><td>用户名</td><td>全名</td><td>邮箱</td><td>是否有效</td>");
            sb.Append("</tr>");
            foreach (var user in list)
            {
                string value = user.IsValid ? "是" : "否";
                sb.Append("<tr align=\"Center\"><td>" + user.UserID + "</td><td>" + user.UserName + "</td><td>" + user.FullName + "</td><td>" + user.Email + "</td><td>" + value + "</td></tr>");
            }
            sb.Append("</table>");
            HttpResponseBase response = HttpContext.Response;
            response.Clear();
            response.ContentType = "application/ms-excel";
            if (HttpContext.Request.Browser.Type.StartsWith("Inter"))
            {
                response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("用户清单.xls", Encoding.GetEncoding("UTF-8")));
            }
            response.AppendHeader("Content-Disposition", "attachment;filename=" + "用户清单.xls");
            response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            response.AddHeader("Pragma", "public");
            response.Write(sb.ToString());
            response.End();
            HttpContext.ApplicationInstance.CompleteRequest();
        }
        #endregion

        [AuthAction("P00001", 128)]
        #region 修改用户角色
        public ActionResult UserRole(string ID)
        {
            var userRole = SysService.GetUserRoles(ID);
            UserAuthModel model = new UserAuthModel { UserID = ID, Roles = new List<AuthRoleModel>() };
            foreach (var r in SysService.GetRoles("",""))
            {
                model.Roles.Add(new AuthRoleModel
                {
                    RoleID = r.RoleID,
                    Name = r.Name,
                    IsSelected = userRole.FirstOrDefault(f => f.Equals(r.RoleID)) != null
                });
            }
            return View(model);
        }
        [AuthAction("P00001", 128)]
        [HttpPost]
        public ActionResult UpdateUserRole()
        {
            try 
            {
                var userID = Request["UserID"];
                string[] roles = Request["Roles[]"].Split(',');
                SysService.SaveUserRole(userID, roles);


                return Json(ResultData.Create(SysService.GetUserModel(userID)), "text/html");
            }
            catch(Exception ex)
            {
                return Json(ResultData.CreateError(ex.Message),"text/html");
            }
        }
        #endregion

        #region 考勤记录导出
        [HttpPost]
        public void ExportCheckRecord()
        {
            var page = Panasia.Core.App.AppConfig.Current.GetPage("P01045");
            var action = page.Config.Actions["ExportQuery"];
            var context = new WebContext
            {
                Controller = this,
                Request = Request.Params,
                Page = page,
                Files = Request.Files,
                ActionValue = SysService.GetCurrentUserPageActionValue(page.PageID)
            };
            /*导出命名方法测试*/
            #region
            context.Execute(action);
            StringBuilder sb = new StringBuilder();
            sb.Append("<style type=\"text/css\">");
            sb.Append("<!--");
            sb.Append(".text");
            sb.Append("{mso-style-parent:style0;");
            sb.Append("font-size:10.0pt;");
            sb.Append("font-family:\"Arial Unicode MS\", sans-serif;");
            sb.Append("mso-font-charset:0;");
            sb.Append(@"mso-number-format:\@;");
            sb.Append("text-align:center;");
            sb.Append("border:.5pt solid black;");
            sb.Append("white-space:normal;}");
            sb.Append("-->");
            sb.Append("</style>");
            sb.Append("<table cellspacing=\"0\" rules=\"all\" border=\"1\" style=\"border-collapse:collapse;\">");
            sb.Append("<tr align=\"Center\" style=\"font-weight:bold;\">");
            sb.Append("<td align=\"Center\" style=\"font-weight:bold;\" ColumnSpan =\"3\">测试测试</td>");
            sb.Append("</tr>");
            sb.Append("<tr align=\"Center\" style=\"font-weight:bold;\">");
            sb.Append("<td align=\"Center\" style=\"font-weight:bold;\" ColumnSpan =\"3\">日期：2014-10-15</td>");
            sb.Append("</tr>");
            sb.Append("<tr align=\"Center\" style=\"font-weight:bold;\">");
            sb.Append("<td>部门</td><td>姓名</td><td>入职日期</td><td>离职日期</td><td>出勤天数</td><td>实际出勤天数</td><td>带薪假天数</td><td>迟到次数</td>");
            sb.Append("<td>早退次数</td><td>病假天数</td><td>事假天数</td><td>旷工天数</td><td>缺勤天数</td><td>满勤天数</td><td>异常出勤明细</td>");//td显示应该是工具配置，对应哪个字段
            sb.Append("</tr>");
            
            var queryResult = context.Results["导出查询"] as CommandResult;
            //var f = queryResult.GetFieldValue<string>("data");
            //var n = queryResult.GetFieldNames();
            //var v = queryResult.GetNameValues();
            //var pv = queryResult.GetPathValue("导出查询");
            //var items  = queryResult.GetValue("Items");
            dynamic prov = queryResult.GetPropertyValue("Items");//和GetValue返回数据一样
            if(prov != null)
            {
                //Assembly tempAssembly = Assembly.LoadFrom(@"D:\code\PA\Panasia.Gemini\Documents\Dll\Panasia.Core.Commands.dll");
                //object o = tempAssembly.CreateInstance("RowEntity");
                //Type t = tempAssembly.GetType();
                foreach (var item in prov)
                {
                    sb.Append("<tr align=\"Center\">");
                    sb.Append("<td>" + item["DeptName"] + "</td>");//工具取字段名
                    sb.Append("<td>" + item["UserName"] + "</td>");
                    sb.Append("<td>" + item["HireDate"] + "</td>");
                    sb.Append("<td>" + item["Terminate"] + "</td>");
                    sb.Append("<td>" + item["AttendanceDays"] + "</td>");
                    sb.Append("<td>" + item["RealAttendanceDays"] + "</td>");
                    sb.Append("<td>" + item["PaidHolidays"] + "</td>");
                    sb.Append("<td>" + item["LaterTimes "]+ "</td>");
                    sb.Append("<td>" + item["LeaveEarlyTimes"] + "</td>");
                    sb.Append("<td>" + item["SickDays"] + "</td>");
                    sb.Append("<td>" + item["LeaveDays"] + "</td>");
                    sb.Append("<td>" + item["AbsenteeismDays"] + "</td>");
                    sb.Append("<td>" + item["AbsenceDays"] + "</td>");
                    sb.Append("<td>" + item["FullDays"] + "</td>");
                    sb.Append("<td>" + item["Remark"] + "</td>");
                    sb.Append("</tr>");
                }
            }
            #endregion
            #region
            //var result = context.Execute(action);
            ////以上代码应该调用工具配置和PageController生成的结果,下面处理结果存入表单
            //var demo = result.ToJson().ToString();
            //int index1 = demo.IndexOf("[");
            //int index2 = demo.IndexOf("]");
            //demo = demo.Substring(index1, index2);
            //var index3 = demo.LastIndexOf("]") - 1;
            //demo = demo.Substring(1, index3);
            //demo = demo.Replace("\\r\\n", "").Replace("\\", "");
            //demo = demo.Replace("},", "}|");
            //string[] ss = demo.Split('|');
            //for (int i = 0; i < ss.Length;i++ ) 
            //{
            //    dynamic data = JsonConvert.DeserializeObject(ss[i]);
            //    if(data != null)
            //    {
            //        sb.Append("<tr align=\"Center\">");
            //        sb.Append("<td>" + data.DeptName +"</td>");
            //        sb.Append("<td>" + data.UserName +"</td>");
            //        sb.Append("<td>" + data.HireDate +"</td>");
            //        sb.Append("<td>" + data.Terminate + "</td>");
            //        sb.Append("<td>" + data.AttendanceDays +"</td>");
            //        sb.Append("<td>" + data.RealAttendanceDays +"</td>");
            //        sb.Append("<td>" + data.PaidHolidays +"</td>");
            //        sb.Append("<td>" + data.LaterTimes +"</td>");
            //        sb.Append("<td>" + data.LeaveEarlyTimes +"</td>");
            //        sb.Append("<td>" + data.SickDays +"</td>");
            //        sb.Append("<td>" + data.LeaveDays +"</td>");
            //        sb.Append("<td>" + data.AbsenteeismDays +"</td>");
            //        sb.Append("<td>" + data.AbsenceDays + "</td>");
            //        sb.Append("<td>" + data.FullDays + "</td>");
            //        sb.Append("<td>" + data.Remark + "</td>");
            //        sb.Append("</tr>");
            //    }
            //}
            #endregion
            sb.Append("</table>");
            HttpResponseBase response = HttpContext.Response;
            response.Clear();
            response.ContentType = "application/ms-excel";
            if (HttpContext.Request.Browser.Type.StartsWith("Inter"))
            {
                response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("考勤月报表.xls", Encoding.GetEncoding("UTF-8")));
            }
            response.AppendHeader("Content-Disposition", "attachment;filename=" + "考勤月报表.xls");
            response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            response.AddHeader("Pragma", "public");
            response.Write(sb.ToString());
            response.End();
            HttpContext.ApplicationInstance.CompleteRequest();
        }
        #endregion

        #region 消息通知
        public ActionResult Message() 
        {

            return View();
        }
        public string Messages()
        {
            string msg = Sys.SysService.Messages();
            return msg;
        }
        public int MessageCount()
        { 
            int mc=Sys.SysService.MessagesCount();
            return mc;
        }
        #endregion

        #region 忘记密码
        public ActionResult ForgetPsw(string email)//根据用户输入发送email到用户邮箱
        {
            string id = SysService.EmailId(email);
            if(id==null)
            {
                return Content("您输入的邮箱不存在", "text/html");
            }
            else{
            string from= "492058555@qq.com";
            string password="geji920407";
            string fromname="系统管理员";
            string smtp = "smtp.qq.com";
            string port="25";
            string chaosong="";
            string misong="";
            string to = email;
            string title ="双子星系统重置密码通知";
            string body = "以下链接将重置您的密码为系统初始密码：<br><p>http://localhost:14003/User/EmailResetPsw?uid="+id+"</p>";
            string f = Sys.SysService.SendMail( from,password,fromname,smtp,port,chaosong, misong,to,title,body);
            return Content("邮件发送成功", "text/html");
                }
        }

        public ActionResult EmailResetPsw(string uid) //用户点击email中地址链接重置密码
        {
            var r=SysService.EmailReset(uid);
            if (r == true)
            {
                return View();//TODO 返回？
            }
            else
            {
                return View();
            }
            
        }
        public ActionResult ResetSuccess() //重置成功返回页面
        {
            return View();
        }

        #endregion
    }
}
