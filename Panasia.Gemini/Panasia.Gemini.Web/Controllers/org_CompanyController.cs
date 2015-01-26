using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Panasia.Gemini.Web.Models.BaseModels;
using Panasia.Gemini.Web.Models;
using Panasia.Gemini.Web.Models.CompanyPageModels;
using Panasia.Gemini.Web.Models.Helpers.ModelHelper;
using Panasia.Core;

namespace Panasia.Gemini.Web.Controllers
{
    public class org_CompanyController : BaseController
    {
        private PADBContext db = new PADBContext();

        private string _ControllerName = "org_Company";

        #region dataGrid
        public ContentResult dataGrid()
        {
           string data = db.org_Companies.ToJson();
            return Content(data);
        }
        #endregion
        #region List
        public ContentResult AjaxList()
        {
            org_Company Company = new org_Company();
            org_Department Department = new org_Department();
            string contentCom = "[";
            string contentDep = "";
            for (int i = 0; i < db.org_Companies.ToList().LongCount(); i++)
            {
                contentCom += "{\"id\":\"" + db.org_Companies.ToList()[i].Company_ID + "\",\"text\":\"" + db.org_Companies.ToList()[i].Company_Name + "\",\"children\":[";
                for (int j = 0; j <db.org_Departments.ToList().LongCount(); j++)
                { 
                    if(db.org_Companies.ToList()[i].Company_ID==db.org_Departments.ToList()[j].Company_ID)
                    {
                        contentDep += "{\"id\":" +db.org_Departments.ToList()[j].Department_ID+ ",\"text\":\"" + db.org_Departments.ToList()[j].Department_Name + "\"}";
                    }
                    if (j != (db.org_Departments.ToList().LongCount() - 1))
                    {
                        contentDep += ",";
                    }
                }
                contentCom += contentDep+"]}";
                contentDep = "";
                if (i != (db.org_Companies.ToList().LongCount()-1))
                {
                    contentCom += ",";
                }
            }
            contentCom += "]";


            return Content(contentCom);


        }
        public override ActionResult List()
        { 
            CompanyListModel pageModel = new CompanyListModel();
            pageModel.CompanyList = db.org_Companies.ToList();
            return View(pageModel);
        }
        #endregion

        #region Add
        public ActionResult Add()
        {
            AddModel addModel = new AddModel();
            addModel.Company = new org_Company();
            addModel.Control = new List<FormControl>() 
            {
                new FormControl{title="公司名称",name="Company.Company_Name",type="textbox",width=50},
                new FormControl{title="公司说明",name="Company.Is_Active",type="textarea"},
            };
            addModel.Control[0].validtype = new ValidateType();
            addModel.Control[0].validtype.Required = true;
            return View(addModel);

        }
        [HttpPost]
        public JsonResult Add(AddModel addModel, FormCollection form)
        {
            if (ModelState.IsValid)
            {
            }
            addModel.Company = new org_Company() { Company_ID = 10, Company_Name = form[0], Is_Active = 1 };
            addModel.Error = new ErrorModel();
            addModel.Error.HasError = false;
            addModel.Error.ErrorMessage = "已存在相同数据";
            return Json(addModel.ToJson());
        }
        #endregion Add

        #region Edit
        #endregion
        #region Details
   
        public override ActionResult ViewDetail(int autoKey,string errorMsg)
        {
            CompanyViewDetailModel pageModel = new CompanyViewDetailModel();
            org_Company company = db.org_Companies.Find(autoKey);
            pageModel.CompanyDetail = company.ToCompanyItem();
            pageModel.Parameters.Add(new NameValueItem("Company_ID", autoKey));
            return View(pageModel);
            
        }
        public ActionResult Searching()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Find(FormCollection form)
        {
            FindCompanyModel findCompany = new FindCompanyModel();
            //检索结果的demo值
            findCompany.Company = new List<org_Company>() { 
                new org_Company{Company_Name="无锡分公司",Company_ID=1,Is_Active=1},
                new org_Company{Company_Name="镇江分公司",Company_ID=2,Is_Active=0},
            };
         // string name=  Request["Company_Name"].ToString();Request.Form["Company_Name"].ToString()
            findCompany.Error = new ErrorModel();
            findCompany.Error.HasError = false;
            findCompany.Error.ErrorMessage = "已存在相同数据";
            return Json(findCompany.ToJson());
        }
        #endregion

        #region Import
        public ActionResult Import()
        {
            return View();
        }
        #endregion
        public ActionResult Index()
        {
            ViewModels viewModel=new ViewModels();
            viewModel.Config =new ConfigClass();
            viewModel.Columns=new List<Panasia.Gemini.Web.Models.Column>()
            {
                new Panasia.Gemini.Web.Models.Column{field="Company_ID",title="Company_ID",checkbox=true,width=80},
                new Panasia.Gemini.Web.Models.Column{field="Company_Name",title="公司名称",width=120},
                new Panasia.Gemini.Web.Models.Column{field="Is_Active",title="是否Active"},
                //new Panasia.Gemini.Web.Models.Column{field="operate",title="操作",align="center",formatOper},
            };
            viewModel.ColumnsToJson = viewModel.Columns.ToJson();
            
            using (var db = new PADBContext())
            {
                viewModel.Config.Company = db.org_Companies.ToList();
                return View(viewModel);
            }
        }
       
        public ActionResult Details()
        {
            
            string demo = Request["id"].ToString();
            DetailCompanyModel detail = new DetailCompanyModel();
            detail.Company = new org_Company();
            detail.Company.Company_ID = demo.ToInt32();
            detail.Company.Company_Name = "无锡分公司";
            detail.Company.Is_Active = 1;
            detail.Error=new ErrorModel();
            detail.Error.HasError = false;
            detail.Error.ErrorMessage = "出现错误";
            return View(detail);
        }

        //
        // GET: /org_Company/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /org_Company/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(org_Company org_company)
        {
            if (ModelState.IsValid)
            {
                db.org_Companies.Add(org_company);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(org_company);
        }


        //
        // GET: /org_Company/Edit/5
        
        public  ActionResult Editing()
        {
            string id = Request["id"].ToString();
            EditingCompanyModel edit = new EditingCompanyModel();
            edit.Company = new org_Company();
            edit.Company.Company_ID = id.ToInt32();
            edit.Company.Company_Name = "无锡分公司";
            edit.Company.Is_Active = 1;
            edit.Error = new ErrorModel();
            edit.Error.HasError = false;
            edit.Error.ErrorMessage = "出现错误";
            return View(edit);
        }

        [HttpPost]
        public JsonResult Edited()
        {
            string id = Request["id"].ToString();
            EditingCompanyModel edit = new EditingCompanyModel();
            edit.Company = new org_Company();
            edit.Company.Company_ID = 1;
            edit.Company.Company_Name = "无锡分公司";
            edit.Company.Is_Active = 1;
            edit.Error = new ErrorModel();
            edit.Error.HasError = false;
            edit.Error.ErrorMessage = "出现错误";
            return Json(edit.ToJson());
        }

        public ActionResult Delete(int id = 0)
        {
            org_Company org_company = db.org_Companies.Find(id);
            if (org_company == null)
            {
                return HttpNotFound();
            }
            return View(org_company);
        }

        //
        // POST: /org_Company/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            org_Company org_company = db.org_Companies.Find(id);
            db.org_Companies.Remove(org_company);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            org_Company org_company = db.org_Companies.Find(id);
            db.org_Companies.Remove(org_company);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}