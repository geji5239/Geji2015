/*----------------------------------------------------------------
// Copyright (C) 2004 清风软件
// 版权所有。
//
// 文件名：TemplateController.cs
// 文件功能描述：Fl002,Fl003控制器,模板表单的增删改查以及表单映射功能。
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Gemini.FL.Models;
using Panasia.Gemini.FL.Data.Models;
using Panasia.Gemini.FL.Data.Common;
using System.Data;
using System.IO;
using Panasia.Core;
using System.Xml;

namespace Panasia.Gemini.FL.Controllers
{
    public class TemplateController : Controller
    {
        public ActionResult Fl002()
        {
            SysContext db = new SysContext();
            var tf = db.fl_Form.AsEnumerable();
            ViewData["Company"] = new SelectList(db.hr_Company.Where(c=>c.IsActive==true).OrderBy(c => c.SortID).AsEnumerable(), "CompanyID", "Name");
            return View();
        }
        public ActionResult Fl003()
        {
            return View();
        }
        public ActionResult GetCompanyName(string formid)
        {
            using(var db=new SysContext())
            {
                var id = Convert.ToInt32(formid);
                var name = (from c in db.hr_Company where c.IsActive orderby c.SortID select new { ID = c.CompanyID, Name = c.JC }).ToList();
                var companys = db.fl_Form.SingleOrDefault(f => f.FormID.Equals(id)).Companys;
                return Json(new { CompanyName = name, Companys = companys }, JsonRequestBehavior.AllowGet);
            }
        }
        public class Combo
        {
         public   string Name { get; set; }
         public string ID { get; set; }
        }
        public ActionResult GetTempName()
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(Server.MapPath("/Configs/Forms.xml"));
            XmlElement root = xmldoc.DocumentElement;
            XmlNodeList NodeList = root.SelectNodes("/Forms/Form");

            List<Combo> combo = new List<Combo>();
            foreach (XmlNode node in NodeList)
            {
                Combo com = new Combo();
                com.Name= node.Attributes["FormName"].Value;
                com.ID = node.Attributes["DataFrom"].Value;
                combo.Add(com);
            }
            return Json(combo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTempInfo()
        {
            var dataform = Request["DataFrom"];
            var formid =Convert.ToInt32( Request["FormID"]);

            string[] dirs = Directory.GetFiles(Server.MapPath("/Templates/") + dataform);
            string url = "";
            var name = "";
            var tempName="";
            var tempID="";
            foreach (string dir in dirs)
            {
                string[] filepart=dir.Split('\\');
                name+=filepart[filepart.Length-1].ToString()+",";
                url+="/Templates/"+dataform+'/'+filepart[filepart.Length-1].ToString()+',';

                using (SysContext db = new SysContext())
                {
                    string filename = filepart[filepart.Length - 1].ToString();
                    filename = filename.Substring(0, filename.IndexOf("."));
                    var temp = db.fl_FormTemplate.SingleOrDefault(ft => ft.Template_FileName.Equals(filename) && ft.FormID.Equals(formid));
                    if (temp == null)
                    {
                        tempName += "" + ',';
                        tempID += "" + ',';
                    }
                    else {
                        tempName += temp.Template_Name+',';
                        tempID += temp.TemplateID.ToString() + ',';
                    }
                }
            }
            return Json(new { Name=name,Url=url,TempID=tempID,TempName=tempName},JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveTemplate()
        {
            var FormID =Convert.ToInt32( Request["FormID"]);
            var TemplateDetail_Name = Request["TemplateDetail_Name"];
            var TemplateID = Request["TemplateID"];
            var Template_Url = Request["Template_Url"];
            var Template_FileName = Request["Template_FileName"];

            var Temp = TemplateID.Split(',');
            var TempName=TemplateDetail_Name.Split(',');
            var TempFileName=Template_FileName.Split(',');
            var TempUrl=Template_Url.Split(',');

            string Message = "OK";
            try
            {
                using (SysContext db = new SysContext())
                {
                    for (var i = 0; i < Temp.Length; i++)
                    {
                        if (Temp[i] != "")
                        {
                            var tempid = Convert.ToInt32(Temp[i]);
                            var Template = db.fl_FormTemplate.SingleOrDefault(ft => ft.TemplateID.Equals(tempid));
                            Template.Template_Name = TempName[i];
                            Template.ResetUpdated();
                            db.SaveChanges();
                        }
                        else
                        {
                            if (TempName[i] != "")
                            {
                                fl_FormTemplate FormTemplate = new fl_FormTemplate
                                {
                                    FormID = FormID,
                                    Template_Name = TempName[i],
                                    Template_FileName = TempFileName[i],
                                    Template_Url = TempUrl[i],
                                };
                                FormTemplate.ResetCreated();
                                db.fl_FormTemplate.Add(FormTemplate);
                                db.SaveChanges();
                            }
                        }
                    }
                    return Json(Message, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
               return Json( Message = e.Message,JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public string UpdateCompanys()
        {
            var FormID =Convert.ToInt32( Request["FromID"]);
            var Companys = Request["Companys"];
            string Message = "OK";
            using (SysContext db=new SysContext())
            {
                try {
                    var form = db.fl_Form.FirstOrDefault(f => f.FormID.Equals(FormID));
                    form.Companys = Companys;
                    form.ResetUpdated();
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Message="Error";
                }
            }

            return Message;
        }
        public string AddTemplate(TemplateModel model)
        {
            string template_Name = model.FormName;
            string table_Name = model.DataFrom;
            string template_FileName = model.Template_FileName;
            string template_Url = model.Template_Url;
            string templatedetail_Name = model.TemplateDetail_Name;
            string Message = "OK";
            using (SysContext db = new SysContext())
            {
                System.Data.Entity.DbContextTransaction tran = db.Database.BeginTransaction();
                try
                {
                    fl_Form entity_fl_Form = db.fl_Form.FirstOrDefault(t => t.FormName == template_Name && t.DataFrom == table_Name);

                    if (entity_fl_Form == null)
                    {
                        entity_fl_Form = new fl_Form();
                        entity_fl_Form.Companys = "";
                        entity_fl_Form.FormName = template_Name;
                        entity_fl_Form.DataFrom = table_Name;
                        entity_fl_Form.CreatedUser = LoginInfo.UserID;
                        entity_fl_Form.ModifiedUser = LoginInfo.UserID;
                        entity_fl_Form.ResetCreated();
                        db.fl_Form.Add(entity_fl_Form);
                        db.SaveChanges();
                    }

                    fl_FormTemplate entity_fl_FormTemplate = new fl_FormTemplate();
                    entity_fl_FormTemplate.FormID = entity_fl_Form.FormID;
                    entity_fl_FormTemplate.Template_Name = templatedetail_Name;
                    entity_fl_FormTemplate.Template_FileName = template_FileName;
                    entity_fl_FormTemplate.Template_Url = template_Url;
                    entity_fl_FormTemplate.CreatedUser = LoginInfo.UserID;
                    entity_fl_FormTemplate.ModifiedUser = LoginInfo.UserID;
                    entity_fl_FormTemplate.ResetCreated();
                    db.fl_FormTemplate.Add(entity_fl_FormTemplate);
                    db.SaveChanges();
                    tran.Commit();
                    Message = "OK," + entity_fl_Form.FormID;
                }
                catch
                {
                    tran.Rollback();
                    Message = "Error";
                }
            }

            return Message;
        }


        public string AddMapping(MappingModel model)
        {
            string table_Name = model.Table_Name;
            string FormID = model.FormID;
            int Approved = model.Approved;
            string Message = "OK";
            using (SysContext db = new SysContext())
            {
                System.Data.Entity.DbContextTransaction tran = db.Database.BeginTransaction();
                try
                {
                        fl_MappingForm entity_fl_MappingForm = new fl_MappingForm();
                        entity_fl_MappingForm.FormNo = FormID;
                        entity_fl_MappingForm.Table_Name = table_Name;
                        entity_fl_MappingForm.Approved = Approved;
                        entity_fl_MappingForm.ApprovedDate = DateTime.Now;
                        entity_fl_MappingForm.TemplateUrl = "";
                        entity_fl_MappingForm.CreatedUser = LoginInfo.UserID;
                        entity_fl_MappingForm.ModifiedUser = LoginInfo.UserID;
                        entity_fl_MappingForm.ResetCreated();
                        db.fl_MappingForm.Add(entity_fl_MappingForm);
                        db.SaveChanges();
                        tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    Message = "Error";
                }
            }

            return Message;
        }

        public string EditMapping(MappingModel model)
        {
            string table_Name = model.Table_Name;
            string FormID = model.FormID;
            int Approved = model.Approved;
            int MappingFormID=model.MappingFormID;
            string Message = "OK";
            using (SysContext db = new SysContext())
            {
                try
                {
                    fl_MappingForm entity_fl_MappingForm = db.fl_MappingForm.Single(d => d.MappingFormID == MappingFormID);
                    if (entity_fl_MappingForm != null)
                    {
                        entity_fl_MappingForm.FormNo = FormID;
                        entity_fl_MappingForm.Table_Name = table_Name;
                        entity_fl_MappingForm.Approved = Approved;
                        entity_fl_MappingForm.ModifiedUser = LoginInfo.UserID;
                        entity_fl_MappingForm.ModifiedTime = DateTime.Now;
                        entity_fl_MappingForm.ResetUpdated();
                        db.SaveChanges();
                       
                    }
                }
                catch
                {
                    Message = "Error";
                }
            }

            return Message;
        }

        public string DeleteMapping()
        {
            int MappingFormID = int.Parse(Request.Form["id"]);
            string Message = "OK";
            try
            {
                using (SysContext db = new SysContext())
                {
                    //fl_TemplateFormDetail templatedetail = db.fl_TemplateFormDetail.Single(d => d.TFDID == TFDID);
                    //db.fl_TemplateFormDetail.Remove(templatedetail);

                    ////如果该表单下所有的模版都删除了，那么删除该表单
                    //if (!db.fl_TemplateFormDetail.Any(d => d.TemplateID == templatedetail.TemplateID))
                    //{
                    db.fl_MappingForm.Remove(db.fl_MappingForm.Single(t => t.MappingFormID == MappingFormID));
                    //}

                    db.SaveChanges();
                }
            }
            catch
            {
                Message = "Error";
            }

            return Message;
        }


        public ActionResult TemplateList()
        {
            SysContext db = new SysContext();
            //var query = db.fl_TemplateForm.Join(db.fl_TemplateFormDetail, t => t.TemplateID, d => d.TemplateID,
            //    (t, d) => new
            //    {
            //        d.TFDID,
            //        t.Template_Name,
            //        t.Table_Name,
            //        TemplateDetail_Name = d.Template_Name,
            //        d.Template_FileName,
            //        d.Template_Url
            //    }).ToList();
            var page = Convert.ToInt32(Request["page"]);
            var rows = Convert.ToInt32(Request["rows"]);
           
            var query =( from p in db.fl_Form
                        select new
                        {
                            p.FormID,
                            p.FormName,
                            p.DataFrom,
                            Companys=(from com in db.hr_Company where p.Companys.Contains(com.CompanyID) select com.JC),
                            TemplateName=(from temp in db.fl_FormTemplate where temp.FormID.Equals(p.FormID) select temp.Template_Name+temp.Template_Url)
                        }).ToList();

            var totalpage = query.Count / Convert.ToInt32(rows);
            if (query.Count % Convert.ToInt32(rows) > 0) { totalpage = totalpage + 1; }
            JsonResult json = Json(new { rows = query.Skip((page - 1) * rows).Take(rows), page = page, total = totalpage, records = query.Count }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public ActionResult TemplateDetailList()
        {

            int id =int.Parse(Request.QueryString["id"]);
            SysContext db = new SysContext();
            var query = db.fl_Form.Join(db.fl_FormTemplate, t => t.FormID, d => d.FormID,
                (t, d) => new
                {
                    d.TemplateID,
                    t.FormID,
                    t.FormName,
                    t.DataFrom,
                    TemplateDetail_Name = d.Template_Name,
                    d.Template_FileName,
                    d.Template_Url
                }).Where(t => t.FormID == id).ToList();

        

            JsonResult json = Json(new { rows = query }, JsonRequestBehavior.AllowGet);
            return json;
        }

        public string EditTemplate(TemplateModel model)
        {
            int TFDID = model.TemplateID;
            int TemplateID = model.FormID;
            string template_Name = model.FormName;
            string table_Name = model.DataFrom;
            string template_FileName = model.Template_FileName;
            string template_Url = model.Template_Url;
            string templatedetail_Name = model.TemplateDetail_Name;
            string Message = "OK";
            try
            {
                using (SysContext db = new SysContext())
                {
                    fl_FormTemplate entity_fl_FormTemplate = null;
                    if (TFDID != 0)
                    {
                        entity_fl_FormTemplate = db.fl_FormTemplate.Single(d => d.TemplateID == TFDID);
                        entity_fl_FormTemplate.Template_Name = templatedetail_Name;
                        entity_fl_FormTemplate.Template_FileName = template_FileName;
                        entity_fl_FormTemplate.Template_Url = template_Url;
                        entity_fl_FormTemplate.ModifiedUser = LoginInfo.UserID;
                        entity_fl_FormTemplate.ModifiedTime = DateTime.Now;
                        entity_fl_FormTemplate.ResetUpdated();
                    }
                    else
                    {
                        entity_fl_FormTemplate = new fl_FormTemplate();
                        entity_fl_FormTemplate.FormID = TemplateID;
                        entity_fl_FormTemplate.Template_Name = templatedetail_Name;
                        entity_fl_FormTemplate.Template_FileName = template_FileName;
                        entity_fl_FormTemplate.Template_Url = template_Url;
                        entity_fl_FormTemplate.CreatedUser = LoginInfo.UserID;
                        entity_fl_FormTemplate.ModifiedUser = LoginInfo.UserID;
                        entity_fl_FormTemplate.ResetCreated();
                        db.fl_FormTemplate.Add(entity_fl_FormTemplate);
                    }

                    fl_Form entity_fl_Form = db.fl_Form.Single(t => t.FormID == entity_fl_FormTemplate.FormID);
                    entity_fl_Form.FormName = template_Name;
                    entity_fl_Form.DataFrom = table_Name;
                    entity_fl_Form.ModifiedUser = LoginInfo.UserID;
                    entity_fl_Form.ModifiedTime = DateTime.Now;
                    entity_fl_Form.ResetUpdated();

                    db.SaveChanges();
                }
            }
            catch
            {
                Message = "Error";
            }

            return Message;
        }

        public string DeleteTemplate()
        {
            int TemplateID = int.Parse(Request["id"]);
            string Message = "OK";
            try
            {
                using (SysContext db = new SysContext())
                {
                   // fl_TemplateFormDetail templatedetail = db.fl_TemplateFormDetail.Single(d => d.TFDID == TFDID);
                   // db.fl_TemplateFormDetail.Remove(templatedetail);

                    //如果该表单下所有的模版都删除了，那么删除该表单
                   // if (!db.fl_TemplateFormDetail.Any(d => d.TemplateID == templatF:\Code2014\Panasia.Gemini\Panasia.Gemini.FL\Views\Shared\_Layout.cshtmledetail.TemplateID))
                  //  {
                        db.fl_Form.Remove(db.fl_Form.Single(t => t.FormID == TemplateID));
                  //  }

                    db.SaveChanges();
                }
            }
            catch
            {
                Message = "Error";
            }

            return Message;
        }

        public string DeleteTemplateDetail()
        {
            int TFDID = int.Parse(Request.Form["id"]);
            string Message = "OK";
            try
            {
                using (SysContext db = new SysContext())
                {
                    fl_FormTemplate templatedetail = db.fl_FormTemplate.Single(d => d.TemplateID == TFDID);
                    db.fl_FormTemplate.Remove(templatedetail);

                    //如果该表单下所有的模版都删除了，那么删除该表单
                    //if (!db.fl_TemplateFormDetail.Any(d => d.TemplateID == templatedetail.TemplateID))
                    //{
                    //    db.fl_TemplateForm.Remove(db.fl_TemplateForm.Single(t => t.TemplateID == templatedetail.TemplateID));
                    //}

                    db.SaveChanges();
                }
            }
            catch
            {
                Message = "Error";
            }

            return Message;
        }

        public ActionResult MappingList()
        {
            int pageIndex = string.IsNullOrEmpty(Request.Params["page"]) ? 1 : int.Parse(Request.Params["page"]);
            int pageSize = string.IsNullOrEmpty(Request.Params["rows"]) ? 10 : int.Parse(Request.Params["rows"]);
            string keyword = Request.Params["keyword"];
            Pager dal_Pager = new Pager();
            PagerSet entity_PagerSet = new PagerSet();
            entity_PagerSet.SortExpression = "MappingFormID";
            entity_PagerSet.SortDirection = "Desc";
            entity_PagerSet.PageCurr = pageIndex;
            entity_PagerSet.PageSize = pageSize;
            string where = "";
            if (!string.IsNullOrEmpty(keyword))
            {
                where= "where  FormID LIKE '%" + keyword + "%'";
            }

            entity_PagerSet.TableOrViewName = "fl_MappingForm";
            entity_PagerSet.ConditionString = where;
            entity_PagerSet.NeedColumn = "MappingFormID,Table_Name,FormID,Approved";

            DataTable dt_DocumentList = dal_Pager.ExecutePager(entity_PagerSet);

            int total = entity_PagerSet.RecordCount;

            JsonResult json = null;
            if (dt_DocumentList != null)
            {
                var query = from p in dt_DocumentList.AsEnumerable()
                            select new
                            {
                                MappingFormID = p["MappingFormID"].ToString(),
                                Table_Name = p["Table_Name"].ToString(),
                                FormID = p["FormID"].ToString(),
                                Approved = p["Approved"].ToString()
                            };
                json = Json(new { total = total, rows = query }, JsonRequestBehavior.AllowGet);
            }



            return json;
        }
    }
}
