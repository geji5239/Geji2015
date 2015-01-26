using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panasia.Core.Auth
{
    public class Actions
    {
        /// <summary>
        /// 列表
        /// </summary>
        public const int Index = 1;
        /// <summary>
        /// 查询
        /// </summary>
        public const int Query = 2;
        /// <summary>
        /// 新增
        /// </summary>
        public const int Add = 4;
        /// <summary>
        /// 删除
        /// </summary>
        public const int Delete = 8;
        /// <summary>
        /// 编辑
        /// </summary>
        public const int Edit = 16;
        /// <summary>
        /// 导入
        /// </summary>
        public const int Import = 32;
        /// <summary>
        /// 导出
        /// </summary>
        public const int Export = 64;
    }
}