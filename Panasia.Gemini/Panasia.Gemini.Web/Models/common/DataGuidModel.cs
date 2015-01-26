using System.Collections.Generic;

namespace Panasia.Gemini.Web.Models.Common
{
    /// <summary>
    /// 所有的对象，通过这个转换为datagrid需要的json对象
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DataGuidModel<T> where T :class
    {
        public int total { get; set; }
        public List<T> rows { get; set; }

        public DataGuidModel()
        {
            this.total = 0;
            this.rows = new List<T>();
        }
    }
}