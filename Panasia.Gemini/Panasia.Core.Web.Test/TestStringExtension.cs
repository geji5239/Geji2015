using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Panasia.Core.Web.Test
{
    [TestClass]
    public class TestStringExtension
    {
        [TestMethod]
        public void Test_ReplaceParameter()
        {
            var source = "xx{@name}{@name}yy";
            var output = source.ReplaceParameters((name) => name + name.Length.ToString());
            Assert.AreEqual(output, "xxname4name4yy");


        }

        [TestMethod]
        public void TestJsonDynamic()
        {
            dynamic data = JsonConvert.DeserializeObject("{\"result\": \"Success\", \"data\": [{\"id\":\"1\"},{\"id\":\"1\"}]}");

            Assert.AreEqual(data.result.Value, "Success");
            Assert.AreEqual(data.data[0].id.Value, "1");
        }

    }
}
