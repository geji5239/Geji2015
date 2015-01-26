using Panasia.Core;
using Panasia.Core.Auth;
using Panasia.Core.Web;
using Panasia.Gemini.Web.Models;
using Panasia.Gemini.Web.Models.Helpers;
using Panasia.Gemini.Web.Progress;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Panasia.Core.Sys;

namespace Panasia.Gemini.Web.Controllers
{
    public class GenerateRecordResultController :Controller
    {
        public ActionResult UploadFile()
        {
            try
            {
                if (Request.Files.Count == 0)
                {
                    return Json(ResultData.CreateError("没有数据"));
                }
                else
                {
                    HttpPostedFileBase file = Request.Files[0];
                    if (file.ContentType == "application/vnd.ms-excel" || file.ContentType == "text/csv" || file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")//excel版本不同类型也不同2014-10-11 ysh
                    {
                        string filePath = HttpRuntime.AppDomainAppPath.ToString() + @"Upload\CheckRecord";//AppDomainAppPath为当前物理驱动器路径
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        string savepath = string.Format("{0}\\{1}", filePath, Path.GetFileName(file.FileName));
                        file.SaveAs(savepath);
                        return ImportToDbII(savepath);
                    }
                    else return Json(ResultData.CreateError("文件类型错误,只能导入.csv或者.xlsx文件"), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(ResultData.CreateError("导入出错，请重新选择文件"), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet); 
            }
        }

        public ActionResult ImportToDbII(string fileName)
        {
            //DataTable dataTable = new DataTable();
            //dataTable.Columns.Add("CardID", typeof(string));
            //dataTable.Columns.Add("Source", typeof(string));
            //dataTable.Columns.Add("CardTime", typeof(DateTime));
            ExcelImportResult importResult = new ExcelImportResult();
            ResultData result = new ResultData();
            try
            {
                //调用通用导入,进度条值问题，创建集合，每插入一条更新值，页面实时访问获取todo
                //ExcelImportResult result = ExcelImportCommand.OnExecute(fileName);
                /*调用读取excel文件的工具，数据读取到表*/
                DataSet ds = new ResumeImportHelpers().ReadExlToDt(fileName);
                DataTable dtTable = ds.Tables[0];
                dtTable = dtTable.DefaultView.ToTable(true, new string[] { "个人编号", "考勤机名称", "刷卡时间" });
                int rowsCount = dtTable.Rows.Count;
                //object[] obj = new object[dataTable.Columns.Count];
                //for (int i = 0; i < rowsCount; i++)
                //{
                //    dtTable.Rows[i].ItemArray.CopyTo(obj, 0);
                //    dataTable.Rows.Add(obj);
                //}
                SqlCommand cmd = new SqlCommand();
                System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["PASys"].ConnectionString);
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "sp_hr_CheckRecordToAttendance";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@InsertData", SqlDbType.Structured);
                cmd.Parameters["@InsertData"].Value = dtTable;
                cmd.Parameters.Add("@UserID", SqlDbType.VarChar,6);
                cmd.Parameters["@UserID"].Value = Panasia.Core.Sys.SysService.GetCurrentUser().UserID;
                cmd.Parameters.Add("@updateNumber", SqlDbType.Int);
                cmd.Parameters["@updateNumber"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ErrorMsg", SqlDbType.VarChar, 800);
                cmd.Parameters["@ErrorMsg"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                importResult.InsertCount = int.Parse(cmd.Parameters["@updateNumber"].Value.ToString());
                importResult.CancelCount = rowsCount - importResult.InsertCount;
                con.Close();
                if (cmd.Parameters["@ErrorMsg"].Value.ToString() == "")
                {
                    result.HasError = false; 
                    result.Data = importResult;
                }
                else
                {
                    result.HasError = true; 
                    result.ErrorMessage = cmd.Parameters["@ErrorMsg"].Value.ToString();
                }
                return Json(result, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                ServiceExtensions.Log(null,e.Message);
                result.HasError = true;
                result.ErrorMessage = e.Message;
                return Json(result, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult MakeMonthResult(FormCollection form)
        {
            ResultData data = new ResultData();
            try
            {
                SqlCommand cmd = new SqlCommand();
                System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["PASys"].ConnectionString);
                con.Open();
                cmd.Connection = con;
                cmd.CommandText = "sp_hr_ToCheckResult";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UserID", SqlDbType.VarChar, 6);
                cmd.Parameters["@UserID"].Value = Panasia.Core.Sys.SysService.GetCurrentUser().UserID;
                cmd.Parameters.Add("@CompanyID", SqlDbType.VarChar, 6);
                cmd.Parameters["@CompanyID"].Value = form["CompanyID"];
                cmd.Parameters.Add("@ReturnMessage", SqlDbType.VarChar,50);
                cmd.Parameters["@ReturnMessage"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                con.Close();
                if (string.IsNullOrEmpty(cmd.Parameters["@ReturnMessage"].Value.ToString()))
                {
                    data.HasError = false;
                    data.ErrorMessage = "生成成功！";
                }
                else
                {
                    data.HasError = true;
                    data.ErrorMessage = cmd.Parameters["@ReturnMessage"].Value.ToString();
                }
                return Json(data, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);

            }
            catch (DbEntityValidationException ex)
            {
                data.HasError = true;
                data.ErrorMessage = ex.Message;
                return Json(data, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
            }
        }



        #region 作废
        //判断文件存不存在和是否被占用,以后放到通用模板
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
                //FileStream：支持同步和异步读写
                try
                {
                    fileStream = System.IO.File.Open(fileName, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);

                    result = false;
                }
                catch (System.IO.IOException ioEx)
                {
                    //记录日志(I/O异常)
                    result = true;
                }
                catch (System.Exception ex)
                {
                    //记录日志（应用程序执行过程中异常）
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

        private Dictionary<string, string> weeks = new Dictionary<string, string>();

        /// <summary>
        /// 提示生成当月的考勤结果还是上月结果，todo只能生成上月提示
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateMessage()
        {
            ResultData data = new ResultData();
            var currentDay = DateTime.Now.Date.ToString("dd");//获取当前时间,几号
            var user = Panasia.Core.Sys.SysService.GetCurrentUser();
            Hr_Rule rule;
            using (var db = new Panasia.Core.Sys.SysContext())
            {
                Panasia.Core.Sys.hr_Employee employee = db.hr_Employees.Where(x => x.UserID == user.UserID).FirstOrDefault();
                rule = GetRule(employee.CardID);
            }
            if (rule != null)
            {
                if (Int32.Parse(currentDay) < rule.MonthClosingDay)//当前时间未超过限制的考勤日期,4应该是系统设置参数
                {
                    //生成的提示是覆盖上个月记录
                    data.HasError = false;
                    //data.ErrorMessage = LangTexts.Current.GetLangText("2023", "");
                    data.ErrorMessage = "重新生成后将会覆盖上月考勤月报表数据，确认更新？";//应该获取统一提示
                    data.Data = null;
                }
                else//todo,这个日期非工作日延后了怎么弄,做个定时器在这日期之前某个点自动执行生成月考勤？
                {
                    data.HasError = true;
                    data.ErrorMessage = "超过了月报表截止日期，不能生成";
                    data.Data = null;
                }
                return Json(data, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
            }
            else
            {
                data.HasError = true;
                data.ErrorMessage = "当前不存在考勤规则，不能生成！";//应该获取统一提示
                data.Data = null;
                return Json(data, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
            }
        }

        #region 先导入,在生成考勤日出勤记录
        [HttpPost]
        /// <summary>
        /// 导入数据库成功或失败都需要记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FileName">上传的文件路径和名称</param>
        /// <returns></returns>
        public ActionResult ImportToDb(string fileName)
        {
            var importResult = new ExcelImportResult();
            ResultData result = new ResultData();
            
            var lastMonth = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");//获取服务器当前时间年月,作为上传的文件中刷卡时间的判断依据
            if (FileIsUsed(fileName))
            {
                return Json(ResultData.CreateError("文件正在使用中！"), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
            }
            //调用通用导入,进度条值问题，创建集合，每插入一条更新值，页面实时访问获取todo
            //ExcelImportResult result = ExcelImportCommand.OnExecute(fileName);
            /*调用读取excel文件的工具，数据读取到表*/
            DataSet ds = new ResumeImportHelpers().ReadExlToDt(fileName);
            DataTable dtTable = new DataTable();
            try
            {
                SysContext db = new SysContext();
                Panasia.Core.Sys.SysContext dbsys=new Core.Sys.SysContext();
                var list = db.CheckRecords.ToList();
                var datebase = db.Database;
                var user = Panasia.Core.Sys.SysService.GetCurrentUser();
                Panasia.Core.Sys.hr_Employee employee = dbsys.hr_Employees.Where(x => x.UserID == user.UserID).FirstOrDefault();
                ProgressDirectory.Clear(user.UserID);//清除缓存
                for (int i = 0; i < ds.Tables.Count;i++ )
                {
                    dtTable = ds.Tables[i];
                    var dv = dtTable.DefaultView;
                    dv.Sort = "刷卡时间 desc";//上传文件中刷卡时间数据排序
                    DataTable dt2 = dv.ToTable();
                    string maxTime = dt2.Rows[0]["刷卡时间"].ToString();//获取最大的刷卡时间
                    string minTime = dt2.Rows[dtTable.Rows.Count - 1]["刷卡时间"].ToString();//获取最小的刷卡时间
                    /*最大刷卡时间和最小刷卡时间截取年月，判断数据是否是同一个月*/
                    string maxImportCardTime = DateTime.Parse(maxTime).ToString("yyyy-MM");
                    string minImportCardTime = DateTime.Parse(minTime).ToString("yyyy-MM");
                    if (maxImportCardTime != lastMonth || minImportCardTime != lastMonth)
                    {
                        result.HasError = true;
                        result.ErrorMessage = "报表包含非法数据，取消导入，只能导入上个月数据！";//应该获取统一提示
                        result.Data = null;
                        return Json(result, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                    }
                    
                    using (var trans = datebase.BeginTransaction())
                    {
                        /*创建导入进度*/
                        FileProgress progress = new FileProgress();
                        progress.FileName = fileName;
                        progress.ImportTotalCount = dtTable.Rows.Count;
                        
                        try
                        {
                            foreach (DataRow row in dtTable.Rows)
                            {
                                var name = row["姓名"].ToString();
                                var cardID = row["个人编号"].ToString();
                                var cardTime = row["刷卡时间"].ToString();
                                var source = row["考勤机名称"].ToString();
                                
                                //判断重复导入
                                var filterList = list.Where(x => x.CardID == cardID).Where(x => x.CardTime == DateTime.Parse(cardTime)).Where(x => x.Source == source);
                                if (filterList != null && filterList.Count() > 0)
                                {
                                    importResult.CancelCount += 1;//这块有没有必要去执行下更新？
                                }
                                else
                                {
                                    //判断是否是临时卡，临时卡数据可能不存在公司，部门相关信息，自定义赋值
                                    var isExist = dbsys.hr_Employees.Where(x => x.CardID == cardID && x.IsActive == true);
                                    if (isExist == null || isExist.Count() == 0)
                                    {
                                        string tempsql = "insert into [hr_CheckRecord] (CompanyID,DeptID,CardID,Name,CardTime,Source,IsActive,CreatedUser,CreatedTime,ModifiedUser,ModifiedTime)" +
                                        " values('999999','999999','" + cardID + "','" + name + "','" + cardTime + "','" + source + "',1,'" + user.UserID + "','" + DateTime.Now + "','" + user.UserID + "','" + DateTime.Now + "')";
                                        var tempCount = datebase.ExecuteSqlCommand(tempsql);
                                        //0、不更新；1、新增成功；2、更新成功；
                                        switch (tempCount)
                                        {
                                            case 0:
                                                importResult.CancelCount += 1;
                                                break;
                                            case 1:
                                                importResult.InsertCount += 1;
                                                break;
                                            case 2:
                                                importResult.UpdateCount += 1;
                                                break;
                                        }
                                        progress.CurrentCount += 1;
                                        continue;
                                    }
                                    string sql = "insert into [hr_CheckRecord] (CompanyID,DeptID,Name,CardID,CardTime,Source,IsActive,CreatedUser,CreatedTime,ModifiedUser,ModifiedTime) ";
                                    string sqlVal = "select CompanyID,DeptID,Name,'" + cardID + "', '" + cardTime + "','" + source + "',1,'" + user.UserID + "','" + DateTime.Now + "','" + user.UserID + "','" + DateTime.Now + "'" + " from [hr_Employee] where CardID='" + cardID + "' and IsActive =1";
                                    var count = datebase.ExecuteSqlCommand(sql + sqlVal);
                                    //0、不更新；1、新增成功；2、更新成功；
                                    switch (count)
                                    {
                                        case 0:
                                            importResult.CancelCount += 1;
                                            break;
                                        case 1:
                                            importResult.InsertCount += 1;
                                            break;
                                        case 2:
                                            importResult.UpdateCount += 1;
                                            break;
                                    }
                                    
                                }
                                progress.CurrentCount += 1;
                                
                                ProgressDirectory.PutValue(user.UserID, progress);//这边UserID定义为key，只要是唯一性的都可以定义key，防止多人访问冲突
                            }
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            importResult = new ExcelImportResult();
                            throw;
                        }
                        /*执行生成一天出勤的记录*/
                        CreateAttendance(employee, result);
                    }//结束导入
                }
                    
            }catch(Exception e)
            {
                ServiceExtensions.Log(null,e.Message);
            }
            result.HasError = false; result.Data = importResult;
            return Json(result, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 生成员工一天的出勤
        /// </summary>
        public void CreateAttendance(Panasia.Core.Sys.hr_Employee employee,ResultData result)
        {
            SysContext db = new SysContext();
            Hr_Rule companyRule = db.Rules.Where(r => r.CompanyID == employee.CompanyID).FirstOrDefault();
            if (companyRule == null)
            {
                result.ErrorMessage = "公司没有考勤规则，取消生成！";
                result.HasError = true;
                result.Data = null;
                return;
            }
            var dataFilter = DateTime.Now.AddMonths(-1);//上月时间
            /*原本做法，先删除记录*/
            //db.Database.ExecuteSqlCommand("delete from [hr_Attendance] where Year='" + dataFilter.Year + "' and Month='" + dataFilter.Month + "' and CompanyID='" + employee.CompanyID + "'");//先删除记录,按年月
            //db.Database.ExecuteSqlCommand("delete from [hr_Attendance] where Year='" + dataFilter.Year + "' and Month='10' and CompanyID='" + employee.CompanyID + "'");//临时测试10月
            //获取用户一天考勤最早刷卡时间和最晚刷卡时间
            string sql = "select convert(varchar(100),min([CardTime]),25) as MinCardTime, convert(varchar(100),max([CardTime]),25) as MaxCardTime, convert(date,[CardTime])as [CardTime],CardID FROM (select * from hr_CheckRecord  where IsActive=1 and convert(nvarchar(7),[CardTime],120) ='" + dataFilter.ToString("yyyy-MM") + "') as a group by convert(date,a.[CardTime]),a.CardID";
            //临时测试的SQL
            //string sql = "select convert(varchar(100),min([CardTime]),25) as MinCardTime, convert(varchar(100),max([CardTime]),25) as MaxCardTime,"+
            //    "convert(date,[CardTime])as [CardTime],CardID FROM (select * from hr_CheckRecord "+
            //    "where IsActive=1 and convert(nvarchar(7),[CardTime],120) ='2014-10') as a group by convert(date,a.[CardTime]),a.CardID";
            DataTable dt = new DataTable();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["PASys"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                conn.Open();
                sda.Fill(dt);
                conn.Close();
            }
            foreach(DataRow row in dt.Rows)
            {
                FileProgress progress = ProgressDirectory.GetProgress(employee.UserID);
                progress.AttendanceTotalCount = dt.Rows.Count;
                #region 基本信息
                Attendance attendance = new Attendance();
                string minTime = row["MinCardTime"].ToString();
                string maxTime = row["MaxCardTime"].ToString();
                string CardID = row["CardID"].ToString();
                string cardTime = row["CardTime"].ToString();
                ArrangeWork work = GetCompanyeArrangeWork(CardID);
                /*类似这样的没有规则的记录，生成考勤记录时跳过，怎么给前端提示生成时取消了多少条
                 目前没有临时表存这类数据，可以考虑放内存，页面查看明细时从内存先取数据，然后在取数据库，返回*/
                if(work == null)
                {
                    progress.CurrentMakeCount += 1;
                    continue;
                }
                Hr_Rule rule = GetRule(CardID);
                if(rule == null)
                {
                    progress.CurrentMakeCount += 1;
                    continue;
                }
                Panasia.Core.Sys.hr_Employee everyEmployee = GetEmployee(CardID);
                if (everyEmployee == null)
                {
                    progress.CurrentMakeCount += 1;
                    continue;
                }
                //假期获取
                IEnumerable<Holiday> holidays = GetHoliday(CardID);

                attendance.Year = DateTime.Parse(minTime).Year.ToString();
                attendance.Month = DateTime.Parse(minTime).ToString("MM");
                attendance.Day = DateTime.Parse(minTime).Date.ToString("dd");
                attendance.StartTime = TimeSpan.Parse(DateTime.Parse(minTime).ToString("HH:mm:ss"));
                attendance.EndTime = TimeSpan.Parse(DateTime.Parse(maxTime).ToString("HH:mm:ss"));
                if (everyEmployee != null)
                {
                    attendance.DeptID = everyEmployee.DeptID;
                    attendance.EmployeeID = everyEmployee.EmployeeID;
                    attendance.JobID = everyEmployee.JobID;
                    attendance.CompanyID = everyEmployee.CompanyID;
                }
                attendance.IsActive = true;
                var weekDay = DateTime.Parse(minTime).DayOfWeek.ToString();
                #endregion
                #region 假期设定处理
                if (holidays != null)
                {
                    foreach (var holiday in holidays)
                    {
                        DateTime holidayStart = holiday.StartDate.Value;
                        DateTime holidatEnd = holiday.EndDate.Value;
                        string buBanDate = holiday.BuBanDate;//补班算正常上班
                        var date = DateTime.Parse(minTime).Date;
                        //加班取日期
                        #region
                        if (date >= holidayStart && date <= holidatEnd)
                        {
                            //假期中间打卡算加班
                            TimeSpan overTime = DateTime.Parse(maxTime) - DateTime.Parse(minTime);//这个加班数不走正常的上班时间设定，计算实际刷卡时间
                            var min = overTime.TotalMinutes;
                            if (min >= rule.MinOverTimeMinutes)
                            {
                                int overRule = rule.RealCalculateMinutes.Value;
                                double d = (min / overRule)* 0.5;
                                attendance.Overtime = Math.Round(d).ToDecimal();
                            }
                            else
                            {
                                //不算加班
                            }
                        }
                        #endregion
                        else
                        {
                            if (weeks.ContainsKey(weekDay))//如果是正常上班时间,周一到周五
                            {
                                validateRecord(minTime, maxTime, work, rule, attendance);
                            }
                            else//算加班
                            {
                                //剔除补班日期
                                if(buBanDate != null)
                                {
                                    List<string> buBanArray = buBanDate.Split(',').ToList();
                                    string time = DateTime.Parse(minTime).Date.ToString("yyyy-MM-dd");
                                    if (buBanArray.Contains(time))
                                    {
                                        validateRecord(minTime, maxTime, work, rule, attendance);
                                    }
                                    else
                                    {
                                        //实际加班
                                        TimeSpan overTime = DateTime.Parse(maxTime) - DateTime.Parse(minTime);//这个加班数按照实际考勤时间
                                        var min = overTime.TotalMinutes;
                                        if (min >= rule.MinOverTimeMinutes)
                                        {
                                            int overRule = rule.RealCalculateMinutes.Value;
                                            double d = (min / overRule) * 0.5;
                                            attendance.Overtime = Math.Round(d).ToDecimal();
                                        }
                                        else
                                        {
                                            //不符合加班时间设置
                                        }
                                    }
                                }else
                                {
                                    //实际加班
                                    TimeSpan overTime = DateTime.Parse(maxTime) - DateTime.Parse(minTime);//这个加班数按照实际考勤时间
                                    var min = overTime.TotalMinutes;
                                    if (min >= rule.MinOverTimeMinutes)
                                    {
                                        int overRule = rule.RealCalculateMinutes.Value;
                                        double d = (min / overRule) * 0.5;
                                        attendance.Overtime = Math.Round(d).ToDecimal();
                                    }
                                    else
                                    {
                                        //不符合加班时间设置
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
                #region 假期没设定的判断
                else
                {
                    if (weeks.ContainsKey(weekDay))//如果是正常上班时间,周一到周五
                    {
                        validateRecord(minTime, maxTime, work, rule, attendance);
                    }
                    else//算加班
                    {
                        TimeSpan overTime = DateTime.Parse(maxTime) - DateTime.Parse(minTime);//这个加班数是按照考勤时间
                        var min = overTime.TotalMinutes;
                        if (min > rule.MinOverTimeMinutes)
                        {
                            int overRule = rule.RealCalculateMinutes.Value;
                            double d = (min / overRule) * 0.5;
                            attendance.Overtime = Math.Round(d).ToDecimal();
                            attendance.ResetCreated();
                        }
                        else
                        {
                            //不算加班
                        }
                    }
                }
                #endregion
                attendance.ResetCreated();
                var isExist = db.Attendances.Where(x => x.Year == attendance.Year && x.Month == attendance.Month && x.Day == attendance.Day && x.CompanyID == attendance.CompanyID && x.DeptID == attendance.DeptID && x.EmployeeID == attendance.EmployeeID).Count();
                if (isExist != 0)
                {
                    db.Entry(attendance).State = EntityState.Modified;//更新记录
                }
                else
                {
                    db.Attendances.Add(attendance);//保存一条记录
                }
                try
                {
                    progress.CurrentMakeCount += 1;
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    //写入日志
                }
            }
        }

        /// <summary>
        /// 获取用户公司的工作时间
        /// </summary>
        public ArrangeWork GetCompanyeArrangeWork(string CardID)
        {
            weeks = new Dictionary<string,string>();
            var result = new ArrangeWork() ;
            using (var db = new SysContext())
            {
                result = db.Database.SqlQuery<ArrangeWork>("select * from [hr_ArrangeWork] where CompanyID = (select top 1 CompanyID from hr_Employee where CardID='" + CardID + "' and IsActive=1)").FirstOrDefault();
                if(result != null)
                {
                    string [] ss = result.WorkDate.Split(',');
                    for (var i = 0; i < ss.Length;i++ )
                    {
                        switch (ss[i]) 
                        {
                            case "周一":
                                weeks.Add("Monday", ss[i]);
                                break;
                            case "周二":
                                weeks.Add("Tuesday", ss[i]);
                                break;
                            case "周三":
                                weeks.Add("Wednesday", ss[i]);
                                break;
                            case "周四":
                                weeks.Add("Thursday", ss[i]);
                                break;
                            case "周五":
                                weeks.Add("Friday", ss[i]);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取考勤规则
        /// </summary>
        public Hr_Rule GetRule(string CardID)
        {
            var result = new Hr_Rule();
            using (var db = new SysContext())
            {
                result = db.Database.SqlQuery<Hr_Rule>("select * from [hr_Rule] where CompanyID = (select top 1 CompanyID from hr_Employee where CardID='" + CardID + "')").FirstOrDefault();
            }
            return result; 
        }

        /// <summary>
        /// 获取假期设定,一个公司应该很多假期设定
        /// </summary>
        /// <param name="CardID"></param>
        /// <returns></returns>
        public IEnumerable<Holiday> GetHoliday(string CardID)
        {
            using (var db = new SysContext())
            {
                var result = db.Database.SqlQuery<Holiday>("select * from [hr_Holiday] where IsWork=0 and CompanyID = (select top 1 CompanyID from hr_Employee where CardID='" + CardID + "')").ToList();
                return result;
            }
        }

        /// <summary>
        /// 根据员工编号获取员工
        /// </summary>
        /// <param name="CardID"></param>
        /// <returns></returns>
        public Panasia.Core.Sys.hr_Employee GetEmployee(string CardID)
        {
            Panasia.Core.Sys.SysContext dbsys = new Core.Sys.SysContext();
            var result = new Panasia.Core.Sys.hr_Employee();
            using (var db = new SysContext())
            {
                result = dbsys.hr_Employees.Where(x => x.CardID == CardID && x.IsActive == true ).FirstOrDefault();
            }
            return result;
        }
        #endregion
        #region 生成考勤月报表
        /// <summary>
        /// 生成考勤月报表
        /// </summary>
        /// <returns></returns>
        public ActionResult MakeMonthResults()
        {
            ResultData data = new ResultData();
            SysContext db = new SysContext();
            Panasia.Core.Sys.SysContext dbsys = new Core.Sys.SysContext();
            var user = Panasia.Core.Sys.SysService.GetCurrentUser();
            Panasia.Core.Sys.hr_Employee employee = dbsys.hr_Employees.Where(x => x.UserID == user.UserID).FirstOrDefault();
            Hr_Rule rule = db.Database.SqlQuery<Hr_Rule>("select * from [hr_Rule] where CompanyID = '"+ employee.CompanyID+"'").FirstOrDefault();
            if (rule == null)
            {
                data.HasError = false;
                data.ErrorMessage = "当前公司没有考勤规则，不能生成！";
                return Json(data, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
            }
            var dataFilter = DateTime.Now;//默认当前时间
            /*只能生成上月考勤结果，不存在生成当月情况*/
            /*todo生成时的限制条件，按照工资生成？*/
            var currentDay = dataFilter.Date.ToString("dd");//获取当前时间,几号
            if (Int32.Parse(currentDay) < rule.MonthClosingDay)//当前时间未超过限制的考勤日期,4应该是系统设置参数
            {
                dataFilter = dataFilter.AddMonths(-1);//上月时间
            }
            else
            {
                data.HasError = true;
                data.ErrorMessage = "超过了当前公司的考勤截止日期，不能生成！";
                return Json(data, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet); 
            }
            
            //using (var trans = db.Database.BeginTransaction())
            //{
                try 
                {
                    //todo改成更新
                    //db.Database.ExecuteSqlCommand("delete from [hr_CheckResult] where Year='" + dataFilter.Year + "' and Month='" + dataFilter.Month + "' and CompanyID='" + employee.CompanyID + "'");//先删除记录，重新生成
                    //db.Database.ExecuteSqlCommand("delete from [hr_CheckResult] where Year='" + dataFilter.Year + "' and Month='10' and CompanyID='" + employee.CompanyID + "'");//临时测试10月SQL

                    List<Attendance> recordList = GetALLCheckRecordByMonth(dataFilter);
                    var query = recordList.GroupBy(p => new { p.EmployeeID }).Select(g => g.First()).ToList();
                    List<LeaveType> leaveTypes = GetAllLeaveTypes();
                    if (query != null && query.Count > 0)
                    {
                        foreach (var record in query)//遍历日出勤记录
                        {
                            CheckResult checkResult = new CheckResult();
                            string time = record.Year + "-" + record.Month;
                            checkResult.Year = record.Year;
                            checkResult.Month = record.Month;
                            checkResult.CompanyID = record.CompanyID;
                            checkResult.DeptID = record.DeptID;
                            checkResult.EmployeeID = record.EmployeeID;
                            checkResult.JobID = record.JobID;
                            checkResult.FullDays = GetFullAttendanceDays(record.Year, record.Month, record.CompanyID);//满勤天数
                            //加班时数
                            checkResult.Overtime = GetOvertimeApplicationForm(record.EmployeeID, time);

                            //外出申请单
                            var goOutForms = GetGoOutForm(record.EmployeeID, time, null).Count();
                            //考勤异常申请单
                            var abnormalAttendances = GetAbnormalAttendance(record.EmployeeID, time, null).Count();

                            //实际出勤天数 = 考勤原始记录+ 异常补单+外出申请单,加班要去除，工资计算不算加班
                            var realDays = recordList.Where(x => x.EmployeeID == record.EmployeeID && x.Overtime == null).Count();
                            checkResult.RealAttendanceDays = realDays + goOutForms + abnormalAttendances;
                            //迟到或早退不存在，走一般的流程，否则要去计算真正的迟到和早退
                            int leaveEarlyCount = recordList.Where(x => x.EmployeeID == record.EmployeeID && x.LeaveEarlyMinutes != null).Count();
                            if (leaveEarlyCount != 0)
                            {
                                validateLateOrEarlyTimes(recordList, record, db, checkResult, rule, true);//早退次数
                            }
                            else
                            {
                                checkResult.LeaveEarlyTimes = 0;
                            }
                            int lateCount = recordList.Where(x => x.EmployeeID == record.EmployeeID && x.MinutesLate != null).Count();
                            if (lateCount != 0)
                            {
                                validateLateOrEarlyTimes(recordList, record, db, checkResult, rule, false); ;//迟到次数
                            }
                            else
                            {
                                checkResult.LaterTimes = 0;
                            }

                            //年假，事假，病假，产假，护理假，产前检查，婚假，丧假，小产假，工伤假应该是天最小单位，哺乳假按照小时最小单位
                            #region 假类所有循环取一次，依次判断每类假别存不存在的记录
                            foreach (var leaveType in leaveTypes)
                            {
                                switch (leaveType.LvID)
                                { 
                                    case "L001":
                                        //带薪假天数
                                        var paidHolidays = GetLeaveApplicationForm(record.EmployeeID, time, "L001", null);
                                        foreach (var ph in paidHolidays)
                                        {
                                            decimal day = ph.ApplicationHours;//年假最小单位天，TimeUnit单位就是D，单据填写时要注意判断
                                            checkResult.PaidHolidays += day;
                                        }
                                        break;
                                    case "L002"://事假
                                        var casualLeavesForms = GetLeaveApplicationForm(record.EmployeeID, time, "L002", null);//事假天数
                                        foreach (var lf in casualLeavesForms)
                                        {
                                            checkResult.LeaveDays += lf.ApplicationHours;
                                        }
                                        break;
                                    case "L003"://病假
                                        var sickForms = GetLeaveApplicationForm(record.EmployeeID, time, "L003", null);//病假天数
                                        foreach (var lf in sickForms)
                                        {
                                            checkResult.SickDays += lf.ApplicationHours;
                                        }
                                        break;
                                    case "L004"://产假
                                        var produceHolidays = GetLeaveApplicationForm(record.EmployeeID, time, "L004", null);
                                        foreach (var ph in produceHolidays)
                                        {
                                            decimal day = ph.ApplicationHours;
                                            checkResult.PaidHolidays += day;
                                        }
                                        break;
                                    case "L005"://护理假
                                        var nurseHolidays = GetLeaveApplicationForm(record.EmployeeID, time, "L005", null);
                                        foreach (var ph in nurseHolidays)
                                        {
                                            decimal day = ph.ApplicationHours;
                                            checkResult.PaidHolidays += day;
                                        }
                                        break;
                                    case "L006"://产前检查
                                        var checkHolidays = GetLeaveApplicationForm(record.EmployeeID, time, "L006", null);
                                        foreach (var ph in checkHolidays)
                                        {
                                            decimal day = ph.ApplicationHours;
                                            checkResult.PaidHolidays += day;
                                        }
                                        break;
                                    case "L007"://婚假
                                        var marryHolidays = GetLeaveApplicationForm(record.EmployeeID, time, "L007", null);
                                        foreach (var ph in marryHolidays)
                                        {
                                            decimal day = ph.ApplicationHours;
                                            checkResult.PaidHolidays += day;
                                        }
                                        break;
                                    case "L008"://丧假
                                        var diedHolidays = GetLeaveApplicationForm(record.EmployeeID, time, "L008", null);
                                        foreach (var ph in diedHolidays)
                                        {
                                            decimal day = ph.ApplicationHours;
                                            checkResult.PaidHolidays += day;
                                        }
                                        break;
                                    case "L009"://小产假
                                        var smallHolidays = GetLeaveApplicationForm(record.EmployeeID, time, "L009", null);
                                        foreach (var ph in smallHolidays)
                                        {
                                            decimal day = ph.ApplicationHours;
                                            checkResult.PaidHolidays += day;
                                        }
                                        break;
                                    case "L010"://工伤假
                                        var hurtHolidays = GetLeaveApplicationForm(record.EmployeeID, time, "L010", null);
                                        foreach (var ph in hurtHolidays)
                                        {
                                            decimal day = ph.ApplicationHours;
                                            checkResult.PaidHolidays += day;
                                        }
                                        break;
                                    case "L011"://其他
                                        break;
                                    default:
                                        break;
                                }
                            }
                            #endregion
                            //调休小时数
                            var changeleaveForms = GetPaidLeaveForm(record.EmployeeID, time, null);
                            foreach (var paidForm in changeleaveForms)
                            {
                                checkResult.PaidHolidays += paidForm.ApplicationHours / 8;//todo,如果调休个2小时，这怎么算，算半天也不合理
                            }

                            //出勤天数=实际出勤天数+ 带薪假天数+ 调休天数
                            checkResult.AttendanceDays = checkResult.RealAttendanceDays + checkResult.PaidHolidays;

                            //旷工天数或缺勤天数 = 满勤天数- 出勤天数
                            Panasia.Core.Sys.hr_Employee he = dbsys.hr_Employees.Where(x => x.EmployeeID == record.EmployeeID && x.IsActive == true).FirstOrDefault();

                            if (checkResult.FullDays >= checkResult.AttendanceDays)
                            {
                                if (he.JobState == "在职" && he.State == "在职")
                                {
                                    checkResult.AbsenteeismDays += checkResult.FullDays - checkResult.AttendanceDays;
                                }
                                else
                                {
                                    //有bug，也存在缺勤和旷工2种
                                    checkResult.AbsenceDays = checkResult.FullDays - checkResult.AttendanceDays;
                                }
                            }
                            checkResult.State = he.State;
                            checkResult.Remark = null;

                            checkResult.ResetCreated();
                            try
                            {
                                var isExist = db.AttendanceResults.Where(x => x.Year == checkResult.Year && x.Month == checkResult.Month && x.CompanyID == checkResult.CompanyID && x.DeptID == checkResult.DeptID && x.EmployeeID == checkResult.EmployeeID).Count();
                                if (isExist != 0)
                                {
                                    db.Entry(checkResult).State = EntityState.Modified; 
                                }else
                                {
                                    db.AttendanceResults.Add(checkResult);
                                }
                                
                                db.SaveChanges();
                                //加上事务保存前2条正常，后面报错，保存的第三个对象不是创建的对象why？
                                //trans.Commit();
                            }
                            catch (Exception e)
                            {
                            }
                        }
                    }
                    else
                    {
                        data.HasError = false;
                        data.ErrorMessage = "上个月没有数据可生成！";
                        return Json(data, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception e)
                {
                    //trans.Rollback();
                    data.HasError = true;
                    data.ErrorMessage = "生成失败！";
                }
            //}
                data.HasError = false;
                data.ErrorMessage = "生成成功！";
            return Json(data, "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取所有日考勤记录
        /// </summary>
        /// <returns></returns>
        public List<Attendance> GetALLCheckRecordByMonth(DateTime dataFilter)
        {
            using (SysContext db = new SysContext())
            {
                var recordList = db.Attendances.Where( x => x.Month == dataFilter.Month.ToString() && x.Year== dataFilter.Year.ToString() && x.IsActive==true).ToList();
                //var recordList = db.Attendances.Where(x => x.Month == "10" && x.Year=="2014" && x.IsActive == true).ToList();//临时测试10月
                return recordList;
            }
        }

        #region 获取各种考勤相关单据
        /// <summary>
        /// 调休单
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public IEnumerable<PaidLeaveForm> GetPaidLeaveForm(string employeeID, string month,string day)
        {
            using (var db = new SysContext())
            {
                //只取签核通过的
                string sql = null;
                if (day == null)
                {
                    sql = "select p.* from [hr_PaidLeaveForm] p left join fl_MappingForm m on p.PLID = m.FormID " +
                           "where m.Approved=2 and convert(varchar(7),p.TStartDate,120) ='" + month + "' and p.EmployeeID='" + employeeID + "' and p.PLID = m.FormID";
                }
                else
                {
                    sql = "select p.* from [hr_PaidLeaveForm] p left join fl_MappingForm m on p.PLID = m.FormID " +
                           "where m.Approved=2 and convert(varchar(10),p.TStartDate,120) ='" + day + "' and p.EmployeeID='" + employeeID + "' and p.PLID = m.FormID";
                }
                var result = db.Database.SqlQuery<PaidLeaveForm>(sql).ToList();
                return result;
            }
        }

        /// <summary>
        /// 外出单
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public IEnumerable<GoOutForm> GetGoOutForm(string employeeID, string month,string day)
        {
            using (var db = new SysContext())
            {
                //只取签核通过的
                string sql = null;
                if (day == null)
                {
                    sql = "select g.* from [hr_GoOutForm] g left join fl_MappingForm m on g.GOFID = m.FormID " +
                           "where m.Approved=2 and convert(varchar(7),g.StartDate,120) ='" + month + "' and g.EmployeeID='" + employeeID + "' and g.GOFID = m.FormID";
                }else
                {
                    sql = "select g.* from [hr_GoOutForm] g left join fl_MappingForm m on g.GOFID = m.FormID " +
                               "where m.Approved=2 and convert(varchar(10),g.StartDate,120) ='" + day + "' and g.EmployeeID='" + employeeID + "' and g.GOFID = m.FormID";
                }
                var result = db.Database.SqlQuery<GoOutForm>(sql).ToList();
                return result;
            }
        }

        /// <summary>
        /// 考勤异常申请单，申请考勤异常时注意要判断规则最大漏打卡次数
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public IEnumerable<AbnormalAttendance> GetAbnormalAttendance(string employeeID, string month,string day)
        {
            using (var db = new SysContext())
            {
                //只取签核通过的
                string sql = null;
                if (day == null)
                {
                    sql = "select a.* from [hr_AbnormalAttendance] a left join fl_MappingForm m on a.AAID = m.FormID " +
                    "where m.Approved=2 and convert(varchar(7),a.StartDate,120) ='" + month + "' and a.EmployeeID='" + employeeID + "' and a.AAID = m.FormID";
                }
                else
                {
                    sql = "select a.* from [hr_AbnormalAttendance] a left join fl_MappingForm m on a.AAID = m.FormID " +
                    "where m.Approved=2 and convert(varchar(10),a.StartDate,120) ='" + day + "' and a.EmployeeID='" + employeeID + "' and a.AAID = m.FormID";
                }
                var result = db.Database.SqlQuery<AbnormalAttendance>(sql).ToList();
                return result;
            }
        }

        /// <summary>
        /// 休假申请单
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="month"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<LeaveApplicationForm> GetLeaveApplicationForm(string employeeID, string month,string type,string day)
        {
            using (var db = new SysContext())
            {
                //只取签核通过的
                string sql = null;
                if(day == null)
                {
                    sql = "select e.* from [hr_LeaveApplicationForm] e left join fl_MappingForm m on e.LAFID = m.FormID " +
                    "where m.Approved=2 and convert(varchar(7),e.StartDate,120) ='" + month + "' and e.EmployeeID='" + employeeID + "' and e.LeaveType='" +
                    type + "' and e.LAFID = m.FormID";
                }else
                {
                    sql = "select e.* from [hr_LeaveApplicationForm] e left join fl_MappingForm m on e.LAFID = m.FormID " +
                    "where m.Approved=2 and convert(varchar(10),e.StartDate,120) ='" + day + "' and e.EmployeeID='" + employeeID + "' and e.LAFID = m.FormID";
                }
                var result = db.Database.SqlQuery<LeaveApplicationForm>(sql).ToList();
                return result;
            }
        }

        /// <summary>
        /// 离职单
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public DimissionForm GetDimissionForm(string employeeID)
        {
            using (var db = new SysContext())
            {
                //只取签核通过的
                string sql = "select d.* from [hr_DimissionForm] d left join fl_MappingForm m on d.DFID = m.FormID " +
                           "where m.Approved=2 and d.EmployeeID='" + employeeID + "' and d.DFID = m.FormID";
                var result = db.Database.SqlQuery<DimissionForm>(sql).FirstOrDefault();
                return result;
            }
        }

        /// <summary>
        /// 加班单,这边直接获取，如果填写的加班日期不对，OA就过不了，这边可以直接取加班统计时数
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public decimal GetOvertimeApplicationForm(string employeeID,string month)
        {
            using (var db = new SysContext())
            {
                //只取签核通过的
                string sql = "select d.* from [hr_OvertimeApplicationForm] d left join fl_MappingForm m on d.OAFID = m.FormID " +
                           "where m.Approved=2 and d.EmployeeID='" + employeeID + "' and d.OAFID = m.FormID and convert(varchar(7),d.StartDate,120) ='" + month+"'";
                var result = db.Database.SqlQuery<OvertimeApplicationForm>(sql).ToList();
                decimal hours = 0;
                foreach (var overTime in result)
                {
                    hours += overTime.ApplicationHours;
                }
                return hours;
            }
        }
        #endregion

        /// <summary>
        /// 获取公司某年某月满勤天数
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="companyID"></param>
        /// <returns></returns>
        public int GetFullAttendanceDays(string year,string month,string companyID)
        {
            //一个月多少天
            int totalDays = DateTime.DaysInMonth(int.Parse(year),int.Parse(month));
            //假期天数
            int holidayCount = 0;
            //补班天数
            int buBanDaysCount = 0;
            //假期占用的双休天数
            int weeksInHolidays = 0;
            //公司上班时间设定
            Dictionary<string, string> weeks = new Dictionary<string, string>();
            using (var db = new SysContext())
            {
                var result = db.Database.SqlQuery<ArrangeWork>("select * from [hr_ArrangeWork] where CompanyID = '" + companyID + "'").FirstOrDefault();
                var holidays = db.Database.SqlQuery<Holiday>("select * from [hr_Holiday] where CompanyID =  '" + companyID + "' and convert(varchar(7), StartDate, 120) = '"+(year+"-"+month)+"'").ToList();//数据库加个额外字段解决补班的问题？
                if (result != null)
                {
                    string[] ss = result.WorkDate.Split(',');
                    for (var i = 0; i < ss.Length; i++)
                    {
                        switch (ss[i])
                        {
                            case "周一":
                                weeks.Add("Monday", ss[i]);
                                break;
                            case "周二":
                                weeks.Add("Tuesday", ss[i]);
                                break;
                            case "周三":
                                weeks.Add("Wednesday", ss[i]);
                                break;
                            case "周四":
                                weeks.Add("Thursday", ss[i]);
                                break;
                            case "周五":
                                weeks.Add("Friday", ss[i]);
                                break;
                            default:
                                break;
                        }
                    }
                }
                if(holidays != null)
                {
                    foreach (var holiday in holidays)
                    {
                        DateTime startDate = holiday.StartDate.Value;
                        DateTime endDate = holiday.EndDate.Value;
                        TimeSpan ts = endDate - startDate;
                        
                        holidayCount = (int)ts.TotalDays + 1;//放假多天
                        for (var i = 0; i < holidayCount;i++ )
                        {
                            var date = startDate.AddDays(i);
                            if (!weeks.ContainsKey(date.DayOfWeek.ToString()))
                            {
                                weeksInHolidays = weeksInHolidays + 1;
                            }
                        }
                        if(!string.IsNullOrEmpty(holiday.BuBanDate))//补班天数
                        {
                            buBanDaysCount = holiday.BuBanDate.Split(',').Length;
                        }
                    }
                }
            }

            //计算当月的上班天数
            int workdays = 0;
            DateTime dayStart = DateTime.Parse(year + "-" + month + "-01");
            DateTime dayEnd = DateTime.Parse(year + "-" + month + "-" + totalDays.ToString());
            for (int i = 0; i < totalDays; i++)
            {
                if (weeks.ContainsKey(dayStart.AddDays(i).DayOfWeek.ToString()))
                {
                    workdays = workdays + 1;
                }
            }

            return workdays - holidayCount + weeksInHolidays + buBanDaysCount;
        }
        #endregion

        #region 获取所有请假类别
        /// <summary>
        /// 获取所有请假类别
        /// </summary>
        /// <returns></returns>
        public List<LeaveType> GetAllLeaveTypes()
        {
            using (SysContext db = new SysContext())
            {
                var recordList = db.LeaveTypes.ToList();
                return recordList;
            }
        }
        #endregion

        /// <summary>
        /// 验证考勤记录
        /// </summary>
        /// <param name="minTime"></param>
        /// <param name="maxTime"></param>
        /// <param name="work"></param>
        /// <param name="rule"></param>
        /// <param name="attendance"></param>
        private void validateRecord(string minTime, string maxTime, ArrangeWork work, Hr_Rule rule, Attendance attendance)
        {
            var hm = DateTime.Parse(minTime).ToString("HH:mm");

            /*上班刷卡时间*/
            if (DateTime.Parse(hm) <= DateTime.Parse(work.UpTime.ToString()))
            {
                //上班刷卡正常
            }
            else
            {
                //迟到
                TimeSpan span = DateTime.Parse(hm) - DateTime.Parse(work.UpTime.ToString());
                var minutesLate = span.TotalMinutes;
                if (minutesLate <= rule.IgnoreMinutesLate)//小于规则中迟到设置
                {
                    //不算迟到
                }
                else if (minutesLate > rule.IgnoreMinutesLate && minutesLate <= rule.MinutesAbsent)
                {
                    //迟到时间小于旷工分钟规则
                    attendance.Remark = "迟到";
                    attendance.MinutesLate = (int)minutesLate;//这个迟到分钟是根据公司Rule计算
                }
                else if (minutesLate > rule.MinutesAbsent && minutesLate <= rule.MinAbsentHours*60)//小时单位转分钟
                {
                    //迟到时间大于旷工分钟设定小于最小旷工设定
                    attendance.Remark += "旷工" + rule.MinAbsentHours + "小时";
                    attendance.MinutesLate = (int)minutesLate;
                    attendance.AbsenteeismHours = (decimal)rule.MinAbsentHours;
                }
                else
                {
                    //迟到时间大于最小旷工设定
                    attendance.Remark += "旷工" + rule.MaxAbsentHours + "小时";
                    attendance.MinutesLate = (int)minutesLate;
                    attendance.AbsenteeismHours = (decimal)rule.MaxAbsentHours;
                }
            }
            /*下班刷卡时间*/
            var leave_hm = DateTime.Parse(maxTime).ToString("HH:mm");
            if (DateTime.Parse(leave_hm) >= DateTime.Parse(work.DownTime.ToString()))
            {
                //下班刷卡正常
            }
            else
            {
                //早退
                TimeSpan span = DateTime.Parse(work.DownTime.ToString()) - DateTime.Parse(leave_hm);
                var minutesEarly = span.TotalMinutes;
                if (minutesEarly <= rule.IgnoreMinutesEarlyleave)//小于规则中早退设置
                {
                    //不算早退
                }
                else if (minutesEarly > rule.IgnoreMinutesEarlyleave && minutesEarly <= rule.MinutesAbsent)
                {
                    //早退时间在旷工分钟内
                    attendance.Remark += "早退";
                    attendance.LeaveEarlyMinutes = (int)minutesEarly;//这个早退是根据Rule设置
                }
                else if (minutesEarly > rule.MinutesAbsent && minutesEarly <= rule.MinAbsentHours*60)
                {
                    attendance.Remark += "旷工" + rule.MinAbsentHours + "小时";
                    attendance.LeaveEarlyMinutes = (int)minutesEarly;//这个早退是根据Rule设置
                    attendance.AbsenteeismHours = (decimal)rule.MinAbsentHours;
                }
                else
                {
                    attendance.Remark += "旷工" + rule.MaxAbsentHours + "小时";
                    attendance.LeaveEarlyMinutes = (int)minutesEarly;
                    attendance.AbsenteeismHours = (decimal)rule.MaxAbsentHours;
                }
            }
        }

        /// <summary>
        /// 迟到或早退次数判断
        /// </summary>
        /// <param name="recordList"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        private void validateLateOrEarlyTimes(List<Attendance> recordList, Attendance record, SysContext db, CheckResult checkResult,Hr_Rule rule,bool isLate)
        {
            IEnumerable<Attendance> list = null;
            if (isLate)
            {
                list = recordList.Where(x => x.EmployeeID == record.EmployeeID && x.MinutesLate != null);
            }
            else
            {
                list = recordList.Where(x => x.EmployeeID == record.EmployeeID && x.LeaveEarlyMinutes != null);
            }
            //判断各种表单，外出，调休，异常，休假
            //当所有表单不存在当前考勤记录才去判断迟到，旷工，只要表单存在记录，才去判断异常出勤的记录和表单能不能组合成一天的出勤，有半天的情况
            foreach(var attendance in list)
            {
                IEnumerable<object> result = null;
                string time = attendance.Year + "-" + attendance.Month +"-"+ attendance.Day;
                string employeeID = attendance.EmployeeID;
                result = GetGoOutForm(employeeID, null, time);//外出单
                if(result.Count() == 0)
                {
                    result = GetPaidLeaveForm(employeeID, null, time);//调休单,小时单位
                    if (result.Count() == 0)
                    {
                        result = GetAbnormalAttendance(employeeID,null,time);//异常申请单
                        if(result.Count() == 0)
                        {
                            result = GetLeaveApplicationForm(employeeID,null,null,time);//休假单
                            if(result.Count() == 0)
                            {
                                int minute = 0;
                                //开始计算迟到或旷工
                                if(isLate)
                                {
                                    minute = attendance.MinutesLate.Value;
                                }else
                                {
                                    minute = attendance.LeaveEarlyMinutes.Value;
                                }
                                if (minute <= rule.MinutesAbsent)
                                {
                                    //真正迟到
                                    checkResult.LaterTimes += 1;
                                }
                                else if (minute > rule.MinutesAbsent && minute <= rule.MinAbsentHours *60)
                                {
                                    //算旷工4小时
                                    checkResult.AbsenteeismDays = (decimal)0.5;
                                }
                                else
                                { 
                                    //算旷工8小时
                                    checkResult.AbsenteeismDays = (decimal)1;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 实时进度条值
        /// </summary>
        /// <returns>result</returns>
        public ActionResult GetCurrentProgress()
        {
            var user = Panasia.Core.Sys.SysService.GetCurrentUser();
            FileProgress progress = ProgressDirectory.GetProgress(user.UserID);
            ResultData result = new ResultData();
            double[] list = new double[2];
            result.HasError = false;
            if (progress.ImportTotalCount != 0)
            {
                int _count = progress.CurrentCount;
                int _tCount = progress.ImportTotalCount;
                //list[0] = Math.Round((double)_count / _tCount, 2) * 100;
                //result.Data = Math.Round((double)_count / _tCount, 2) * 100;
                list[0] = Math.Round((double)_count / _tCount, 2) * 100;
            }
            else 
            {
                list[0] = 0;
                //result.Data = 0;
            }
            if (progress.AttendanceTotalCount != 0)
            {
                int _count = progress.CurrentMakeCount;
                int _tCount = progress.AttendanceTotalCount;
                //result.Data = Math.Round((double)_count / _tCount, 2) * 100;
                list[1] = Math.Round((double)_count / _tCount, 2) * 100;
            }
            else 
            {
                list[1] = 0;
            }
            result.Data = list;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
