using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Panasia.Core.Sys;

namespace Panasia.Core.Auth
{
    public class ViewBase : WebViewPage
    {
        public virtual new UserPrincipal User
        {
            get { return base.User as UserPrincipal; }
        }

        public override void Execute()
        { 
        }
    }
    public abstract class ViewBase<TModel> : WebViewPage<TModel>
    {
        public virtual new UserPrincipal User
        {
            get { return base.User as UserPrincipal; }
        }
    }
}
