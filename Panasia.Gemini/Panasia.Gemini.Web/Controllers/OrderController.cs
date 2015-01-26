using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Core.Auth;
using System.IO;

namespace Panasia.Gemini.Web.Controllers
{
    public class OrderController : Controller
    {
        //
        // GET: /Order/
        public ActionResult UploadAttach()
        {
            int iTotal = Request.Files.Count;
            ResultData r = new ResultData();
            if (iTotal == 0)
            {
                r = ResultData.CreateError("没有数据");
            }
            else
            {
                string orderID = Request.Form["OrderID"];
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif", "bmp" };
                HttpPostedFileBase file = Request.Files[0];
                string fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);

                string fileName = Guid.NewGuid().ToString("N") + "." + fileExt;

                if (!supportedTypes.Contains(fileExt.ToLower()))
                {
                    r = ResultData.CreateError("文件类型错误");
                }
                if (file.ContentLength > 0 || !string.IsNullOrEmpty(file.FileName))
                {
                    //保存文件
                    string filePath = Server.MapPath(string.Format("~/Upload/OrderAttachment/{0}/", orderID));
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    string savepath = string.Format(@"{0}\{1}", filePath, fileName);
                    file.SaveAs(savepath);
                    r = ResultData.Create(fileName);

                    string originFileUrl = Request.Form.AllKeys.Contains("OriginAttFileUrl") ? Request.Form["OriginAttFileUrl"] : "";
                    if (originFileUrl != "")
                    {
                        string originFilePath =Server.MapPath(string.Format("~/Upload/OrderAttachment/{0}/{1}", orderID,originFileUrl));
                        if (System.IO.File.Exists(originFilePath))
                        {
                            System.IO.File.Delete(originFilePath);
                        }
                    }
                }
            }
            return Json(r, JsonRequestBehavior.AllowGet); ;
        }
	}
}