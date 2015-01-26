using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panasia.Core.Sys
{
    public class Messages
    {
        //[{0}] , 欢迎您使用本系统！
        public static readonly string System_Welcome = LangTexts.Current.GetLangText("1001", "[{0}] , 欢迎您使用本系统！");
        //请求的页面不存在！
        public static readonly string System_PageNotFound=LangTexts.Current.GetLangText("1004", "请求的页面不存在");

        public static readonly string System_ActionNotFound=LangTexts.Current.GetLangText("1005", "Action[{0}]不存在");

        public static readonly string System_ActionNotAllowed=LangTexts.Current.GetLangText("1006", "无权访问此页面");
        
        //请检查您的用户名和密码是否输入正确。
        public static readonly string System_InvalidUserNameOrPassword = LangTexts.Current.GetLangText("2001", "请检查您的用户名和密码是否输入正确。");
        //密码修改成功！
        public static readonly string System_PasswordChangedSuccess = LangTexts.Current.GetLangText("2002", "密码修改成功！");
        //密码修改失败，请重新输入！
        public static readonly string System_PasswordChangedFailure = LangTexts.Current.GetLangText("2003", "密码修改失败，请重新输入！");
        //[{0}] + 成功！
        public static readonly string System_ActionSuccess = LangTexts.Current.GetLangText("2004", "[{0}] + 成功！");
        //[{0}] + 失败！[{1}]
        public static readonly string System_ActionFailure = LangTexts.Current.GetLangText("2005", "[{0}] + 失败！[{1}]");
        //读取数据失败，请重试！
        public static readonly string System_LoadFailure = LangTexts.Current.GetLangText("2006", "读取数据失败，请重试！");
        //创建对象失败，请检查网络并重试！ 
        public static readonly string System_CreateObjectError = LangTexts.Current.GetLangText("2007", "创建对象失败，请检查网络并重试！ ");
        //成功 文档上传成功！ 
        public static readonly string Files_UploadSuccess = LangTexts.Current.GetLangText("2008", "成功 文档上传成功！ ");
        //失败 文档上传时出现异常，请重试！
        public static readonly string Files_UploadFailure = LangTexts.Current.GetLangText("2009", "失败 文档上传时出现异常，请重试！");
        //文档下载时出现异常，请重试！
        public static readonly string Files_DownloadFailure = LangTexts.Current.GetLangText("2010", "文档下载时出现异常，请重试！");
        //[{0}] 不能为空，请填写完整！
        public static readonly string Validates_InputFieldNotNull = LangTexts.Current.GetLangText("2011", "[{0}] 不能为空，请填写完整！");
        //[{0}] 不能为空，请选择！ 
        public static readonly string Validates_SelectFieldNotNull = LangTexts.Current.GetLangText("2012", "[{0}] 不能为空，请选择！ ");
        //[{0}] 不能为 [{1}]
        public static readonly string Validates_FieldValueError = LangTexts.Current.GetLangText("2013", "[{0}] 不能为 [{1}]");
        //[{0}] 不能大于 [{1}] 或 [{0}] 不能小于 [{1}]
        public static readonly string Validates_FieldValueMustInRange = LangTexts.Current.GetLangText("2014", "[{0}] 不能大于 [{1}] 或 [{0}] 不能小于 [{1}]");
        //[{0}], 超出业务允许范围区间！
        public static readonly string Validates_FieldValueOutRange = LangTexts.Current.GetLangText("2015", "[{0}], 超出业务允许范围区间！");
        //[{0}] 输入有误 [ 应该是：[{1}] ] 请重新输入！
        public static readonly string Validates_DataFormatError = LangTexts.Current.GetLangText("2016", "[{0}] 输入有误 [ 应该是：[{1}] ] 请重新输入！");
        // 请选择[{0}]！
        public static readonly string Validates_PleaseSelect = LangTexts.Current.GetLangText("2017", " 请选择[{0}]！");
        //确认要 + [{0}] + 吗？
        public static readonly string Confirm_Action = LangTexts.Current.GetLangText("2018", "确认要 + [{0}] + 吗？");
        //[{0}] 已经存在 , 确定要覆盖吗？
        public static readonly string Confirm_Replace = LangTexts.Current.GetLangText("2019", "[{0}] 已经存在 , 确定要覆盖吗？");
        //- 没有查询到符合条件的记录！
        public static readonly string Records_NotFound = LangTexts.Current.GetLangText("2020", "- 没有查询到符合条件的记录！");
        //- 登录用户已经达到最大连接数！
        public static readonly string Records_ReachMaxUserConnections = LangTexts.Current.GetLangText("2021", "- 登录用户已经达到最大连接数！");
        //- 登录超时，请重新操作！
        public static readonly string Records_LoginTimeout = LangTexts.Current.GetLangText("2022", "- 登录超时，请重新操作！");
        //- 对不起，您没有操作权限！
        public static readonly string Records_NotAllowed = LangTexts.Current.GetLangText("2023", "- 对不起，您没有操作权限！");
        //- 已经存在相同信息，无法保存！
        public static readonly string Records_AlreadyExists = LangTexts.Current.GetLangText("2024", "- 已经存在相同信息，无法保存！");

    }
}
