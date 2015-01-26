using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panasia.Core;

namespace Panasia.Core.Sys
{
    public static class DataHelper
    {
        public static string CreateJsonTree(this IEnumerable<IEntityBase> items,
            string rootID, string pidName, string idName, string textName,string checkedID="")
        {
            Func<IEntityBase, string, string> getValue = (item, name) =>
            {
                var v = item.GetValue(name, null);
                return v == null ? "" : v.ToString();
            };
            var groups = items.GroupBy(g => getValue(g, pidName))
                .ToDictionary((g) => g.Key);

            var root = groups.GetDictionaryValue(rootID, null);
            if (root == null)
            {
                return "[]";
            }
            var result = items.CreateItems(rootID, pidName, idName, textName);
            if(result==null)
            {
                return "[]";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                result.CreateJsonTree(sb, checkedID);
                return sb.ToString();
            } 
        } 
    }

    public static class JsonTreeExtension
    {
        public static void CreateJsonTree(this IEnumerable<JsonTreeItem> items, StringBuilder sb, string checkedID = "")
        {
            sb.Append("["); 
            bool first = true;
            foreach(var item in items)
            {
                if(first)
                {
                    first = false;
                }
                else
                {                     
                    sb.Append(",");
                }
                sb.AppendFormat("{{\"id\":\"{0}\",\"text\":\"{1}\"", item.id, item.text); 
                if(item.id.Equals(checkedID, StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(",\"checked\":\"true\"");
                }
                if(item.children==null)
                {
                    sb.Append(",\"children\":[]}");
                }
                else
                {
                    sb.Append(",\"children\":");
                    item.children.CreateJsonTree(sb, checkedID);
                    sb.Append("}");
                }
            }
            sb.Append("]");
        }

        public static IEnumerable<JsonTreeItem> CreateItems(this IEnumerable<IEntityBase> items, string rootID, string pid, string id, string text)
        {
            Func<IEntityBase, string, string> getValue = (item, name) =>
            {
                var v = item.GetValue(name, null);
                return v == null ? "" : v.ToString();
            };

            var groups = items.GroupBy(g => getValue(g, pid))
                .ToDictionary((g) => g.Key);

            return CreateItems(rootID, pid, id, text, groups, getValue);
        }

        private static IEnumerable<JsonTreeItem> CreateItems(string rootid, string pid, string id, string text,
            Dictionary<string, IGrouping<string, IEntityBase>> groups,
            Func<IEntityBase, string, string> getValue)
        {
            var root = groups.GetDictionaryValue(rootid, null);
            if (root == null)
            {
                yield break;
            }
            foreach (var item in root)
            {
                var idValue = getValue(item, id);
                if (idValue.Equals(rootid))
                {
                    yield break;
                }
                var jitem = new JsonTreeItem
                {
                    id = idValue,
                    text = getValue(item, text),
                    children = CreateItems(idValue, pid, id, text, groups, getValue).ToList()
                };
                yield return jitem;
            }
        }

    }

    public class JsonTreeItem
    {
        public string id { get; set; }

        public string text { get; set; }
        
        public List<JsonTreeItem> children { get; set; }
    }
}
