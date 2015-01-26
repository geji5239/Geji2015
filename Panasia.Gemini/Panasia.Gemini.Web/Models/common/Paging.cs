using System.Collections.Generic;
using System.Linq;

namespace Panasia.Gemini.Web.Models.Common
{
    /// <summary>
    /// 分页信息以及分页处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Paging<T> where T : class
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 一页几行
        /// </summary>
        public int rows { get; set; }
        /// <summary>
        /// 升降
        /// </summary>
        public string sort { get; set; }
        /// <summary>
        /// 按照哪个来排
        /// </summary>
        public string order { get; set; }

        /// <summary>
        /// 无参构
        /// </summary>
        public Paging()
        {
        }

        /// <summary>
        /// 分页过滤处理
        /// </summary>
        /// <param name="list"></param>
        /// <remarks>
        /// 默认是根据主键来排序的，其他的排序处理逻辑见“EDS>Serve.Common.DBHelper”
        /// </remarks>
        public void pagingProc(ref IEnumerable<T> list)
        {
            //if (list == null)
            //{
            //    return;
            //}
            //else
            //{
            //    list = list.Select(x => x).OrderBy(x => x.key).Skip((this.page - 1) * this.rows).Take(this.rows);
            //}
        }
    }
}