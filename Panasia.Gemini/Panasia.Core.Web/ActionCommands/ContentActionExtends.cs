using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Panasia.Core.Commands;

namespace Panasia.Core.Web
{
    public class ContentActionExtends
    {
        /* ContentAction：在界面配置中，有时会有需要进一步处理的需要，这时候就可以写扩展代码。
         * 扩展代码的几个关键参数
         * 1、必须在方法的上面加上[ContentAction({名称},{显示名称})];
         * 2、方法必须符合object {方法名称}(Dictionary<string, object> paras);
         * 3、方法产生的是文本，可以是html、json等等;
         * 
         * 其它：可以作为变量的中间处理，后面继续执行其它的命令，这时候Content就无所谓了
         * 
         * 使用：方法写完之后，要将方法所在的项目的dll拷贝到配置工具目录，这样配置工具里就可以通过【扩展方法】命令来取这个方法；
         */
        [ContentAction("GetxxTableText", "xx表导出文本")]
        public object GetxxTableText(Dictionary<string, object> paras)
        {
            dynamic rows = paras.GetDictionaryValue("rows", null);
            foreach(var row in rows)
            {
                //如果row是RowEntity

                //如果row是DataTable的row

            }
            
             
            return "";
        }
    }
}
