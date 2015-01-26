using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panasia.Core.Web
{
    public enum UIFieldType
    {
        //空
        None,
        //文本框
        TextBox,
        //密码框
        PasswordBox,
        //多行文本框
        TextArea,
        //隐藏文本框
        Hidden,
        //单选框
        CheckBox,
        //下拉框
        ComboBox,
        //下拉树
        ComboTree,
        //搜选框
        PickBox,
        //日期框
        DateBox,
        //日期时间框
        DateTimeBox,
        //数字框
        NumberBox,
        //文件框
        FileBox,
        //图片框
        ImageBox
    }
}
