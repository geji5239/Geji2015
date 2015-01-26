using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Panasia.Core.Web
{
    [XmlRoot("Form")]
    public class IndexForm:Form
    {
        #region 属性 ToolBar
        private ToolBar _ToolBar = null;
        [XmlIgnore()]
        public ToolBar ToolBar
        {
            get
            {
                return _ToolBar;
            }
            set
            {
                _ToolBar = value;
                RaisePropertyChanged(() => ToolBar);
            }
        }
        #endregion

        #region 属性 GridView
        private GridView _GridView = null;
        public GridView GridView
        {
            get
            {
                return _GridView;
            }
            set
            {
                _GridView = value;
                RaisePropertyChanged(() => GridView);
            }
        }
        #endregion
    }
}
