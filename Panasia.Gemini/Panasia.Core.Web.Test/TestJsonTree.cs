using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panasia.Core;

namespace Panasia.Core.Web.Test
{
    [TestClass]
    public class TestJsonTree
    {
        [TestMethod]
        public void Test_JsonTree()
        {
            List<Entity> items = new List<Entity>();
            var item1 = new Entity();
            item1.SetFieldValue("ID", "");
        } 

        private Entity CreateEntity(params object[] nameValues)
        {
            var item = new Entity();
            for (int i = 0; i < nameValues.Length - 1;i++ )
            {
                item.SetFieldValue(nameValues[i].ToString(), nameValues[i + 1]);
            }
            return item;
        }
    }
     
}
