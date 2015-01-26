using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Panasia.Core;

namespace Panasia.Core.Web
{
    public static class InputTypes
    {
        private static object _LoadLock = new object();
        private static bool _InitDone = false;

        private static Dictionary<UIFieldType, InputTypeItem> _Items = new Dictionary<UIFieldType, InputTypeItem>();

        static InputTypes()
        {
            Init();
        }

        #region 初始化加载
        private static void Init()
        {
            if (!_InitDone)
            {
                lock (_LoadLock)
                {
                    if (!_InitDone)
                    {
                        //添加一个默认项
                        _Items[UIFieldType.None] = new InputTypeItem { TypeName = UIFieldType.None, Title = "无输入", Func = () => new UIInput() };
                        //文本框
                        _Items[UIFieldType.TextBox] = new InputTypeItem { TypeName = UIFieldType.TextBox, Title = "文本框", Func = () => new UIInput() };
                        //文本框
                        _Items[UIFieldType.PasswordBox] = new InputTypeItem { TypeName = UIFieldType.PasswordBox, Title = "密码框", Func = () => new UIInput { InputType="password" } };
                        //多行文本框
                        _Items[UIFieldType.TextArea] = new InputTypeItem { TypeName = UIFieldType.TextArea, Title = "多行文本框", Func = () => new UITextArea() };
                        //隐藏文本框
                        _Items[UIFieldType.Hidden] = new InputTypeItem { TypeName = UIFieldType.Hidden, Title = "隐藏文本框", Func = () => new UIInput { IsHidden = true } };
                        //单选框
                        _Items[UIFieldType.CheckBox] = new InputTypeItem { TypeName = UIFieldType.CheckBox, Title = "复选框", Func = () => new UICheckBox() };
                        //下拉框
                        _Items[UIFieldType.ComboBox] = new InputTypeItem { TypeName = UIFieldType.ComboBox, Title = "下拉框", Func = () => new UIComboBox() };
                        //下拉树
                        _Items[UIFieldType.ComboTree] = new InputTypeItem { TypeName = UIFieldType.ComboTree, Title = "下拉树", Func = () => new UIComboTree() };
                        //搜选框
                        _Items[UIFieldType.PickBox] = new InputTypeItem { TypeName = UIFieldType.PickBox, Title = "搜选框", Func = () => new UIPickBox() };
                        //日期框
                        _Items[UIFieldType.DateBox] = new InputTypeItem { TypeName = UIFieldType.DateBox, Title = "日期框", Func = () => new UIDateBox() };
                        //日期时间框
                        _Items[UIFieldType.DateTimeBox] = new InputTypeItem { TypeName = UIFieldType.DateTimeBox, Title = "日期时间框", Func = () => new UIDateTimeBox   () };
                        //数字框
                        _Items[UIFieldType.NumberBox] = new InputTypeItem { TypeName = UIFieldType.NumberBox, Title = "数字框", Func = () => new UINumericBox() };
                        //文件框
                        _Items[UIFieldType.FileBox] = new InputTypeItem { TypeName = UIFieldType.FileBox, Title = "文件框", Func = () => new UIFileBox() };
                        //图片框
                        _Items[UIFieldType.ImageBox] = new InputTypeItem { TypeName = UIFieldType.ImageBox, Title = "图片框", Func = () => new UIImageBox() };

                        _InitDone = true;
                    }
                }
            }
        }
        #endregion

        public static UIInput Create(UIFieldType typeName)
        {
            var item = _Items.GetDictionaryValue(typeName, null);
            var input = item == null ? null : item.Func();
            input.InitSettings();
            return input;
        }

        public static IEnumerable<InputTypeItem> GetItems()
        {
            return _Items.Values;
        }
    }

    public class InputTypeItem
    {
        public UIFieldType TypeName { get; internal set; }

        public string Title { get; internal set; }

        public Func<UIInput> Func { get; internal set; }
    }

    //[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false), MetadataAttribute]
    //public class InputTypeAttribute : ExportAttribute, IInputTypeMetadata
    //{
    //    internal const string InputTypeContract = "InputTypeContract";

    //    public string TypeName { get; private set; }

    //    public string Title { get; private set; }

    //    public InputTypeAttribute(string typeName, string title) :
    //        base(InputTypeContract, typeof(Func<object, object, object>))
    //    {
    //        TypeName = typeName;
    //        Title = title;
    //    }
    //}

    //public interface IInputTypeMetadata
    //{
    //    string TypeName { get; }

    //    string Title { get; }
    //}
}
