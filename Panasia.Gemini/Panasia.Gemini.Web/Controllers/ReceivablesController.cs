using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Panasia.Core.Auth;
using Panasia.Gemini.Web.Models.Helpers;

namespace Panasia.Gemini.Web.Controllers
{
    public class ReceivablesController : Controller
    {
        //
        // GET: /Interview/
        [HttpPost]
        public ActionResult Import(FormCollection form)
        {
            //string JCID = form["JCID"];
            ReceivablesHelpers th = new ReceivablesHelpers();
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection FileCollect = request.Files;
            if (FileCollect.Count > 0)
            {
                foreach (string str in FileCollect)
                {
                    HttpPostedFile FileSave = FileCollect[str];  //用key获取单个文件对象HttpPostedFile
                    string fileName = FileSave.FileName.Substring(FileSave.FileName.LastIndexOf("\\") + 1);
                    string filePath = string.Format(HttpRuntime.AppDomainAppPath.ToString() + @"Upload");
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    string Path = string.Format("{0}\\{1}_{2}", filePath, DateTime.Now.ToString("yyyyMMddHHmmss"), fileName);
                    FileSave.SaveAs(Path);              //将上传的文件保存
                    //return Json(ResultData.CreateError("维护中，暂不能用!"));
                    ResultData result = th.ImportResult(Path);
                    if (result.HasError)
                    {
                        return Json(ResultData.CreateError("导入失败!错误信息:" + result.ErrorMessage), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                    }
                    return Json(ResultData.CreateError("导入成功!"), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                }
            }
            return View(form);
        }

        public ActionResult UploadFile(FormCollection form)
        {
            HttpRequestBase request = Request;
            string fileNewName = form["CustomerID"] + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"); //新文件名
            string document = @"\Upload\CustomerAtt";                                       //上传文件夹
            string[] fileTypes = new[] { "jpg", "jpeg", "png", "gif", "bmp" }; //文件格式设定

            int iTotal = request.Files.Count;
            if (iTotal == 0)
            {
                return Json(ResultData.CreateError("没有数据"));
            }
            else
            {
                for (int i = 0; i < iTotal; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    string fileType = System.IO.Path.GetExtension(file.FileName).Substring(1).ToLower(); //扩展名（包含‘.’）
                    if (!fileTypes.Contains(fileType))
                    {
                        return Json(ResultData.CreateError("文件类型错误"), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                    }
                    else if (file.ContentLength>1024*1024*2)
                    {
                        return Json(ResultData.CreateError("文件大小不得超过2M"), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        string fileName = file.FileName.Substring(file.FileName.LastIndexOf("\\") + 1);//原文件名（包含扩展名）
                        fileNewName = fileNewName + "." + fileType; //新文件名（包含扩展名）
                        string filePath = string.Format(HttpRuntime.AppDomainAppPath.ToString() + document);//绝对路径文件夹
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        string relativePath = string.Format("{0}\\{1}", document, fileNewName);//相对路径
                        string Path = string.Format("{0}\\{1}", filePath, fileNewName);//绝对路径
                        file.SaveAs(Path);   //保存文件到绝对路径
                        object data = (object)relativePath.Replace("\\", "/");
                        return Json(ResultData.Create(data), "text/html;charset=utf-8", JsonRequestBehavior.AllowGet);//返回相对路径
                    }
                }
            }
            return null;
        }

    }
}
