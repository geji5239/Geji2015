using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Panasia.Core;
using Panasia.Core.Sys;

namespace Panasia.Core.Contents
{
    public class RazorTemplate : ContentFileBase
    {
        private Type _CompiledType = null;

        private static bool _RazorIgnoreCompile =
            ApplicationConfig.GetAppSettingValue("RazorIgnoreCompile", "").Equals("true", StringComparison.OrdinalIgnoreCase);

        private static bool _RazorIgnoreModelType =
            ApplicationConfig.GetAppSettingValue("RazorIgnoreModelType", "").Equals("true", StringComparison.OrdinalIgnoreCase);

        public static bool IsIgnoreComplile
        {
            get
            {
                return _RazorIgnoreCompile;
            }
        }
        public static bool IsIgnoreModelType
        {
            get
            {
                return _RazorIgnoreModelType;
            }
        }

        #region 属性 ModelTypeName
        private string _ModelTypeName = "";
        [XmlAttribute("ModelType"), DefaultValue("")]
        public string ModelTypeName
        {
            get
            {
                return _ModelTypeName;
            }
            set
            {
                _ModelTypeName = value;
                RaisePropertyChanged(() => ModelTypeName);
                ModelType = ModelTypeName.GetModelType();
            }
        }
        #endregion

        #region 属性 CompileMode
        private CompileMode _CompileMode = CompileMode.OnDemand;
        [XmlAttribute("CompileMode"), DefaultValue(CompileMode.OnDemand)]
        public CompileMode CompileMode
        {
            get
            {
                return _CompileMode;
            }
            set
            {
                _CompileMode = value;
                RaisePropertyChanged(() => CompileMode);
            }
        }
        #endregion

        #region 属性 IsCompiled
        private bool _IsCompiled = false;
        [XmlIgnore()]
        public bool IsCompiled
        {
            get
            {
                return _IsCompiled;
            }
            private set
            {
                _IsCompiled = value;
                RaisePropertyChanged(() => IsCompiled);
            }
        }
        #endregion

        #region 属性 ModelType
        private Type _ModelType = null;
        [XmlIgnore()]
        public Type ModelType
        {
            get
            {
                if (_ModelType == null)
                {
                    _ModelType = ModelTypeName.GetModelType();
                    if (_ModelType == null)
                    {
                        _ModelType = typeof(RazorContentViewModel);
                    }
                }
                return _ModelType;
            }
            private set
            {
                _ModelType = value;
                RaisePropertyChanged(() => ModelType);
            }
        }
        #endregion

        public override void Load()
        {
            ContentLib.AddRazorTemplate(this);
            base.Load();

            if (_RazorIgnoreCompile)
            {
                return;
            }

            if (CompileMode == Contents.CompileMode.OnLoaded)
            {
                Compile();
            }
        }

        public void Compile()
        {
            try
            { 
                IsCompiled = false;
                _CompiledType = null;
                if (!string.IsNullOrEmpty(Content))
                {
                    var modelType = ModelType;
                    if (_RazorIgnoreModelType || modelType == null)
                    {
                        Compile(typeof(DynamicObject)); 
                    }
                    else
                    {
                        RazorService.Compile(Content, modelType, FullName.ToLower());
                        _CompiledType = modelType;
                    }
                    IsCompiled = true;
                }
            }
            catch (Exception ex)
            {
                ex.Log(ex.ToString(), Category.Debug);
                throw LangTexts.Current.GetFormatLangText("9010", "模板[{0}]编译时出错:\r\n{1}", FullName, ex.Message)
                    .CreateException();
            }
        }

        private void Compile(Type type)
        {
            RemoveContentModel();
            RazorService.Compile(Content, type, FullName.ToLower());
            _CompiledType = type;
        }

        public string Render(object model)
        {
            var lastDate = LastChanged;
            if (!CheckExists())
            {
                throw LangTexts.Current.GetFormatLangText("9011", "模板文件[{0}]加载失败", FullName)
                    .CreateException();
            }

            if (_RazorIgnoreCompile)
            {
                if (lastDate.Ticks != LastChanged.Ticks)
                {
                    Content = LoadContent();
                }
                if (_RazorIgnoreModelType)
                {
                    RemoveContentModel();
                }

                return RazorService.Parse(Content, model);
                //return RazorService.Parse(;
            }

            if ((!IsCompiled) || lastDate.Ticks != LastChanged.Ticks)
            {
                Compile();
            }

            if (IsCompiled)
            {
                return RazorService.Run(FullName.ToLower(), model);
            }
            return null;
        }

        private void RemoveContentModel()
        {
            var index = Content.IndexOf("@model ");
            if (index >= 0)
            {
                var lineEnd = Content.IndexOf("\r\n", index);
                if (lineEnd > index)
                {
                    Content = index == 0 ? Content.Substring(lineEnd) :
                        Content.Substring(0, index) + Content.Substring(lineEnd);
                }
            }
        }
    }


}
