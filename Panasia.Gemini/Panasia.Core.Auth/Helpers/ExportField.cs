
namespace Panasia.Core.Auth.Helpers
{
    /// <summary>
    /// 导出字段
    /// </summary>
    public class ExportField
    {
        /// <summary>
        /// 获取或设置要导出的字段名
        /// </summary>
        public string Property { get;private set; }
        /// <summary>
        /// 获取或设置导出时的标题
        /// </summary>
        public string Title { get;private set; }

        public ExportField(string property, string title)
        {
            this.Property = property;
            this.Title = title;
        }
    }
}
