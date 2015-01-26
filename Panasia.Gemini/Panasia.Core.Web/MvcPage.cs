using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml.Serialization;
using Panasia.Core;
using Panasia.Core.App;

namespace Panasia.Core.Web
{
    [PageConfig("MvcPage","一般MVC页面",typeof(MvcPage))]
    public class MvcPage:PageConfig
    {
        #region 属性 Controller
        private string _Controller = string.Empty;
        [XmlAttribute]
        public string Controller
        {
            get
            {
                return _Controller;
            }
            set
            {
                _Controller = value;
                RaisePropertyChanged(() => Controller);
            }
        }
        #endregion

        #region 属性 DefaultAction
        private string _DefaultAction = "Index";
        [XmlAttribute]
        public string DefaultAction
        {
            get
            {
                return _DefaultAction;
            }
            set
            {
                _DefaultAction = value;
                RaisePropertyChanged(() => DefaultAction);
            }
        }
        #endregion

        #region 属性 IsRedirect
        private bool _IsRedirect = false;
        [XmlAttribute("IsRedirect"), DefaultValue(false)]
        public bool IsRedirect
        {
            get
            {
                return _IsRedirect;
            }
            set
            {
                _IsRedirect = value;
                RaisePropertyChanged(() => IsRedirect);
            }
        }
        #endregion
               

        public virtual ActionResult GetResult(ICommandContext context,string actionName)
        {
            return null;
        }

        protected override ActionCollection CreateDefaultActions()
        {
            var actions = new ActionCollection();

            return actions;
        }
    } 

}
