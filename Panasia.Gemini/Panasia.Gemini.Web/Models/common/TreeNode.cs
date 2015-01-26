using System;
using System.Collections.Generic;
using System.Linq;
using Panasia.Core.Sys;

namespace Panasia.Gemini.Web.Models.Common
{
    /// <summary>
    /// 界面树
    /// </summary>
    public class TreeNode
    {
        public string id { get; set; }                  // 主键
        public string text { get; set; }                // 文本值
        public string state { get; set; }               // 是否展开
        public string attributes { get; set; }          // 级别
        public string iconCls { get; set; }             // 节点的图标
        public List<TreeNode> children { get; set; }    // 子节点

        /// <summary>
        /// 构造函数
        /// </summary>
        public TreeNode()
        {
            this.children = new List<TreeNode>();
        }
    }
}