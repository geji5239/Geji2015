using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Core.App;
using Panasia.Core.Sys;

namespace Panasia.Core.Auth.Controllers
{
    public class RoleController : Controller
    {
        [AuthAction("P00002",1)]
        public ActionResult Index()
        {

            return View();
        }

        #region 获取treegird数据
        public string GetTreegrid()
        {
            string jsonstr = "";
            string col = "";
            int pcount=0;
            foreach (var g in AppConfig.Current.PageGroups)
            {
                col += "{\"id\":\"" + g.Name + "\",\"name\":\"" + g.Name + "\"},";
                foreach (var p in g.Pages.Where(w => w.IsEnable))
                {
                    col += "{\"id\":\"" + p.PageID + "\",\"name\":\"" + p.Title + "\",\"_parentId\":\"" + g.Name + "\",\"action\":\"";
                    int prevalue = 0;
                    foreach (var a in p.Config.Actions.Where(a => a.IsEnable).GroupBy(a => a.Title))
                    {
                        if (prevalue != a.First().ActionValue)
                        {
                            if (a.First().ActionValue != 0)
                            {
                                col += a.First().ActionValue + ":" + a.Key;
                                if (a.Key != p.Config.Actions.Where(k => k.IsEnable).GroupBy(k => k.Title).Last().Key)
                                {
                                    col += ",";
                                }
                            }
                            prevalue = a.First().ActionValue;
                        }
                    }
                    col += "\"},"; 
                }
                pcount+=g.Pages.Where(w=>w.IsEnable).Count();
            }
           col= col.Substring(0, col.Length - 1);
            jsonstr+="{\"total\":"+(Convert.ToInt16(AppConfig.Current.PageGroups.Count())+pcount)+",\"rows\":["+col+"]}";
            return jsonstr.Replace("\\","");
        }
        #endregion

        #region 获取treegird带选中状态数据
        public string GetTreegridState(string ID)
        {
            string jsonstr = "";
            string col = "";
            int pcount = 0;
            var rolePages = SysService.GetRolePages(ID);
            foreach (var g in AppConfig.Current.PageGroups)
            {
                col += "{\"id\":\"" + g.Name + "\",\"name\":\"" + g.Name + "\"},";
                foreach (var p in g.Pages.Where(w => w.IsEnable))
                {
                    col += "{\"id\":\"" + p.PageID + "\",\"name\":\"" + p.Title + "\",\"_parentId\":\"" + g.Name + "\",\"action\":\"";
                    int prevalue = 0;
                    foreach (var a in p.Config.Actions.Where(a => a.IsEnable).GroupBy(a => a.Title))
                    {
                        if (prevalue != a.First().ActionValue)
                        {
                            if (a.First().ActionValue != 0)
                            {
                                bool state=rolePages.FirstOrDefault(f => f.PageID.Equals(p.PageID) &&(f.ActionValue & a.First().ActionValue) == a.First().ActionValue) != null;
                                col += a.First().ActionValue + ":" + a.Key + ":" + state.ToString();
                                if (a.Key != p.Config.Actions.Where(k => k.IsEnable).GroupBy(k => k.Title).Last().Key)
                                {
                                    col += ",";
                                }
                            }
                            prevalue = a.First().ActionValue;
                        }
                    }
                    col += "\"},";
                }
                pcount += g.Pages.Where(w => w.IsEnable).Count();
            }
            col = col.Substring(0, col.Length - 1);
            jsonstr += "{\"total\":" + (Convert.ToInt16(AppConfig.Current.PageGroups.Count()) + pcount) + ",\"rows\":[" + col + "]}";
            return jsonstr.Replace("\\", "");
        }
        #endregion

        #region 新增角色
        [AuthAction("P00002", 2)]
        public ActionResult Create()
        {
            RoleAuthModel model = new RoleAuthModel { RoleID = "", Pages = new List<AuthPageModel>() };
            model.Titles = new List<string>();
            foreach (var g in AppConfig.Current.PageGroups)
            {
               
                model.Titles.Add(g.Name);
                foreach (var p in g.Pages.Where(w => w.IsEnable))
                {
                    model.Pages.Add(new AuthPageModel
                    {
                        PageID = p.PageID,
                        Title = p.Title,
                        Name= g.Name,
                        Actions = p.Config.Actions.Where(a => a.IsEnable).GroupBy(a => a.Title)
                        .Select(m => new AuthActionModel
                        {
                            ActionTitle = m.Key,
                            ActionValue = m.First().ActionValue,
                            IsSelected = false
                        }).ToList()
                    });
                }
            }
            return View(model);
        }
        [AuthAction("P00002", 2)]
        [HttpPost]
        public ActionResult Add()
        {
            try
            {
                string Name = Request["Name"];
                string Description = Request["Description"];
                string CompanyIDs = Request["CompanyIDs"];
                string DeptIDs = Request["DeptIDs"];
                string BankIDs = Request["BankIDs"];

                var Role = Sys.SysService.CreateRole(Name,Description);
                
                

                var pageValues = Request.Params.AllKeys
                   .Where(k => k.StartsWith("P") && k.Length == 6)
                   .Select((r) => new RolePage
                   {
                       RoleID = Role.RoleID,
                       PageID = r,
                       DataFilter = (r == "P01001") ? CompanyIDs :(r=="P01002") ? DeptIDs:(r=="P01017")? BankIDs:"",
                       ActionValue = Convert.ToInt32(Request.Params[r])
                   }).ToList();
                SysService.UpdateRolePages(Role.RoleID, pageValues);
                return Json(ResultData.Create(Role));
            }
            catch (Exception ex)
            {
                return Json(ResultData.CreateError(ex.Message), "text/html");
            }
        }
        #endregion

        [AuthAction("P00002", 4)]
        #region 修改角色
        public ActionResult Edit(string ID)
        {
            var model = SysService.GetRole(ID);

            var rolePages = SysService.GetRolePages(ID);
            RoleAuthModel pagemodel = new RoleAuthModel { RoleID = ID, Name = model.Name, Description = model.Description,Pages = new List<AuthPageModel>() };
            pagemodel.DataFilter = null;
            pagemodel.Titles = new List<string>();
            foreach (var g in AppConfig.Current.PageGroups)
            {
                pagemodel.Titles.Add(g.Name);
                foreach (var p in g.Pages.Where(w => w.IsEnable))
                {
                    pagemodel.Pages.Add(new AuthPageModel
                    {
                        PageID = p.PageID,
                        Title = p.Title,
                        Name = g.Name,
                        Actions = p.Config.Actions.Where(a => a.IsEnable).GroupBy(a => a.Title)
                        .Select(m => new AuthActionModel
                        {
                            ActionTitle = m.Key,
                            ActionValue = m.First().ActionValue,
                            IsSelected = rolePages.FirstOrDefault(f => f.PageID.Equals(p.PageID) &&
                                (f.ActionValue & m.First().ActionValue) == m.First().ActionValue) != null
                        }).ToList()
                    });
                }
            }
            var companyids = (from rp in rolePages where rp.PageID.Equals("P01001") select rp.DataFilter.ToString()).ToArray();
            var deptids = (from rp in rolePages where rp.PageID.Equals("P01002") select rp.DataFilter.ToString()).ToArray(); ;
            var bankids = (from rp in rolePages where rp.PageID.Equals("P01017") select rp.DataFilter.ToString()).ToArray();
            foreach (var companyid in companyids)
            {
                pagemodel.DataFilter += companyid;
            }
            pagemodel.DataFilter += ":";
            foreach (var deptid in deptids)
            {
                pagemodel.DataFilter += deptid;
            }
            pagemodel.DataFilter+=":";
            foreach (var bankid in bankids)
            {
                pagemodel.DataFilter += bankid;
            }
            return View(pagemodel);
        }

        [AuthAction("P00002", 4)]
        [HttpPost]
        public ActionResult Update()
        {
             try
            {
                string RoleID = Request["RoleID"];
                string Name = Request["Name"];
                string Description = Request["Description"];
                string CompanyIDs = Request["CompanyIDs"];
                string DeptIDs = Request["DeptIDs"];
                string BankIDs = Request["BankIDs"];

                SysService.UpdateRole(RoleID, Name, Description);

                var pageValues = Request.Params.AllKeys
                    .Where(k => k.StartsWith("P") && k.Length == 6)
                    .Select((r) => new RolePage
                    {
                        RoleID = RoleID,
                        PageID = r,
                        DataFilter = (r == "P01001") ? CompanyIDs : (r == "P01002") ? DeptIDs : (r == "P01017") ? BankIDs : "",
                        ActionValue = Convert.ToInt32(Request.Params[r])
                    }).ToList();
                
                SysService.UpdateRolePages(RoleID, pageValues);

                Role role = new Role { RoleID = RoleID, Name = Name, Description = Description};              
                return Json(ResultData.Create(role),"text/html");
            }
            catch (Exception ex)
            {
                return Json(ResultData.CreateError(ex.Message), "text/html");
            }
        }
        #endregion

        [AuthAction("P00002", 8)]
        #region 查看角色
        public ActionResult Detail(string ID)
        {
            var model = SysService.GetRole(ID);
            return View(model);
        }
        #endregion

        [AuthAction("P00002", 16)]
        #region 删除角色
        public ActionResult Delete(string keys)
        {
            if(string.IsNullOrEmpty(keys))
            {
                return Json(ResultData.CreateError("主键为空，删除取消"));
            }
            if(keys.Contains(','))
            {
                //throw new Exception("角色不支持批量删除");
                return Json(ResultData.CreateError("角色不支持批量删除"));
            }
            else
            {
                try
                {
                    SysService.DeleteRole(keys);
                    return Json(ResultData.Create(true), "text/html"); 
                }
                catch (Exception ex)
                {
                    return Json(ResultData.CreateError(ex.Message));
                }
            }
            
        }
        #endregion 

        [AuthAction("P00002", 32)]
        #region 检索
        public ActionResult Search()
        {
            return View();
        }

        [AuthAction("P00002", 32)]
        [HttpPost]
        public ActionResult Query()
        {
            string name = Request["Name"];
            string description = Request["Description"];
            string page = Request["page"];
            string rows = Request["rows"];
            return Json(SysService.GetRolesWithPage(name,description,page,rows));
        }
        #endregion
        
        #region 修改权限
        public ActionResult Auth(string ID)
        {
            var rolePages = SysService.GetRolePages(ID);
            RoleAuthModel model = new RoleAuthModel { RoleID = ID, Pages = new List<AuthPageModel>() };
            foreach (var g in AppConfig.Current.PageGroups)
            {
                foreach(var p in g.Pages.Where(w=>w.IsEnable))
                {
                    model.Pages.Add(new AuthPageModel
                    {
                        PageID = p.PageID,
                        Title = p.Title,
                        Actions = p.Config.Actions.Where(a => a.IsEnable).GroupBy(a => a.Title)
                        .Select(m => new AuthActionModel
                        {
                            ActionTitle = m.Key,
                            ActionValue = m.First().ActionValue,
                            IsSelected = rolePages.FirstOrDefault(f => f.PageID.Equals(p.PageID) &&
                                (f.ActionValue &  m.First().ActionValue) == m.First().ActionValue)!=null
                        }).ToList()
                    });
                }
            } 
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateAuth()
        {
            try
            {
                var roleID = Request["RoleID"];
                var pageValues = Request.Params.AllKeys
                    .Where(k => k.StartsWith("P") && k.Length == 6)
                    .Select((r) => new RolePage
                    {
                        RoleID = roleID,
                        PageID = r,
                        ActionValue = Convert.ToInt32(Request.Params[r])
                    }).ToList();
                SysService.UpdateRolePages(roleID, pageValues);

                return Json(ResultData.Create(true), "textml");
            }
            catch (Exception ex)
            {
                return Json(ResultData.CreateError(ex.Message), "text/html");
            }
        }
        #endregion
    }
}
