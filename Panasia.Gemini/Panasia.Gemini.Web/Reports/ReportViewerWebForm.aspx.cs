using Microsoft.Reporting.WebForms;
using System;
using System.Web.UI.WebControls;
using Panasia.Core.Web;

namespace ReportViewerForMvc
{
    /// <summary>
    /// The Web Form used for rendering a ReportViewer control.
    /// </summary>
    public partial class ReportViewerWebForm : System.Web.UI.Page
    {
        private ReportViewerSetting _Setting = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if(IsPostBack)
            {
                return;
            }
            var id = Request["id"];
            _Setting = ReportViewers.GetReportViewer(id);
            if (_Setting != null)
            { 
                ReportViewer1.Reset();
                ReportViewer1.LocalReport.Dispose();
                ReportViewer1.LocalReport.DataSources.Clear();

                ReportViewer1.Width = Unit.Percentage(100);
                ReportViewer1.Height = Unit.Percentage(100);

                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/" + _Setting.ReportPath);
                _Setting.DataSources.ForEach(ds =>
                {
                    ReportViewer1.LocalReport.DataSources.Add(ds);
                });
                ReportViewer1.LocalReport.SetParameters(_Setting.Parameters);
                ReportViewer1.LocalReport.Refresh();
            }
            else
            {
                ReportViewer1.Reset();
                ReportViewer1.LocalReport.Dispose();
                ReportViewer1.LocalReport.DataSources.Clear();

                ReportViewer1.Width = Unit.Percentage(100);
                ReportViewer1.Height = Unit.Percentage(100);

                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Reports/Report1.rdlc");
            }
        }

        public string GetHeader()
        {
            return _Setting == null ? "" : _Setting.ContentHeader;
        }

        public string GetFooter()
        {
            return _Setting == null ? "" : _Setting.ContentFooter;
        }
    }
}