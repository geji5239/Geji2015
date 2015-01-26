using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panasia.Gemini.Web.Progress
{
    public class FileProgress
    {
        public string FileName { get; set; }
        /// <summary>
        /// 导入文件数据的总数
        /// </summary>
        public int ImportTotalCount { get; set; }
        /// <summary>
        /// 当前导入执行进度
        /// </summary>
        public int CurrentCount { get; set; }
        /// <summary>
        /// 出勤的总数
        /// </summary>
        public int AttendanceTotalCount { get; set; }
        /// <summary>
        /// 当前出勤生成的进度
        /// </summary>
        public int CurrentMakeCount { get; set; }
    }
}