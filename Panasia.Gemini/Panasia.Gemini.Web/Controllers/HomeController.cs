using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Core.Auth;
using System.IO;
using Panasia.Core.App;
using Panasia.Core;
using Panasia.Core.Web;
using Panasia.Core.Sys;
using System.Data.SqlClient;
using System.Configuration;

namespace Panasia.Gemini.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        [Auth]
        public ActionResult Main()
        {
            var user = SysService.GetCurrentUser();
            if (user == null)
            {
                return Redirect("Account/Login");
            }

            if(user.UserID.Equals("U00001"))
            {
                return View(AppConfig.Current.Menus);
            }

            var userPages = SysService.GetUserPages(user.UserID);
            Dictionary<string, Page> pages = new Dictionary<string, Page>();

            foreach (var group in AppConfig.Current.PageGroups)
            {
                foreach (var item in group.Pages)
                {
                    pages[item.PageID] = item;
                }
            }
            
            Func<Menu, bool> isUserPage = (m) =>
            {
                if (string.IsNullOrWhiteSpace(m.Src))
                {
                    return true;
                }

                if (!m.Src.StartsWith("Page", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (m.Src.Length < 11)
                {
                    return true;
                }
                var pageID = m.Src.Substring(5, 6);

                var userPage = userPages.FirstOrDefault(p => p.PageID.Equals(pageID, StringComparison.OrdinalIgnoreCase));
                if (userPage == null)
                {
                    return false;
                }

                Page page = pages.GetDictionaryValue(pageID, null);
                if (page == null)
                {
                    return false;
                }

                var index = m.Src.IndexOf('?');
                var actionName = m.Src.Length < 12 ? "Index" :
                    index > 0 ? m.Src.Substring(12, index - 12) :
                    m.Src.Substring(12);
                var action = page.Config.Actions.FirstOrDefault(a => a.Name.Equals(actionName, StringComparison.OrdinalIgnoreCase));
                if (action == null)
                {
                    return false;
                }

                if (action.ActionValue == 0)
                {
                    return true;
                }

                return (userPage.ActionValue & action.ActionValue) == action.ActionValue;
            };
            
            MenuCollection menus = new MenuCollection();
            Menu topmenus = new Menu();

            foreach (var topm in AppConfig.Current.Menus)
            {
                var menu = new Menu { Title=topm.Title,Name=topm.Name};
                foreach (var group in topm.Menus)
                { 
                    var smenus = group.Menus.Where(m => isUserPage(m)).ToList();
                    if (smenus.Count == 0)
                    {
                        continue;
                    }
                    var tmenu = new Menu { Title = group.Title, Name = group.Name };
                    smenus.AddToCollection(tmenu.Menus);
                    menu.Menus.Add(tmenu);
                    //menusAdd(tmenu);
                }
                menus.Add(menu);
            }

            //foreach (var group in topmenus.Menus)
            //{
               
                
            //    var menu = new Menu { Title = group.Title, Name= group.Name };
            //    smenus.AddToCollection(menu.Menus);
            //    menus.Add(menu);
            //}

            return View(menus);
        }

        public ActionResult ReloadConfig()
        {
            if (Request.IsLocal)
            {
                AppConfig.ReloadCurrent();
                SqlData.ReloadCurrent();
                UISettings.Reload();
                LangTexts.Reload();
                SystemCodes.Reload();
                return Content("刷新成功");
            }
            else
            {
                return Content("非法链接");
            }
        }
                
        [HttpPost]
        public ActionResult IsOnline()
        {
            var isonline= HttpContext.User!=null && HttpContext.User is UserPrincipal;
            return new JsonResult { Data = isonline.ToJson() };
        }

        public ActionResult NotAllowed()
        {
            return View();
        }

        public ActionResult TestPage()
        {
            return PartialView();
        }

        public ActionResult ControlSample()
        {
            return View();
        }

        public ActionResult TestPage2()
        {
            return View();
        }

        public ActionResult QuerySample()
        {
            return View();
        }

        public ActionResult NewItemSample()
        {
            return View();
        }

        public ActionResult EditItemSample()
        {
            return View();
        }

        /// <summary>
        /// 超时或者未登陆显示的界面
        /// </summary>
        /// <returns></returns>
        public ActionResult TimeOut()
        {
            return View();
        }

        public string Count() 
        {
            SqlCommand edate = new SqlCommand("select count(ContractEndDate) from hr_Employee where convert(varchar(7),ContractEndDate,126)=convert(varchar(7), getdate(), 126)"  );
            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["PASys"].ConnectionString);
            con.Open();
            SqlDataReader c1 = edate.ExecuteReader();
            return (c1[0].ToString());
        }
    }
}
