using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Panasia.Core.Email;

namespace Panasia.Core.Web.Test
{
    [TestClass]
    public class TestEmail
    {
        private Credentials Credentials = new Credentials { UserID = "xiangxiejin@panasia.cn", Password= ""};
        private Host Host = new Host { Address = "smtp.exmail.qq.com", EnableSsl = true, Port = "465" };

        [TestMethod]
        public void TestSendItem()
        {
            var sendItem = new SendItem
            {
                To = new List<EmailItem> { new EmailItem { Email = "83009867@qq.com" } },
                Host = Host,
                Credentials = Credentials,
                From = new List<EmailItem> { new EmailItem { Email = "xiangxiejin@panasia.cn" } },
                Body = new Body {  Content = "this is a test mail from panasia.cn"},
                Subject = new Subject {  Content = "测试邮件"}
            };

            sendItem.Send();

            Assert.IsNotNull(sendItem);
        }

        [TestMethod]
        public void TestMailServer()
        {
            var server = new MailServer(Host.Address,Convert.ToInt32(Host.Port), Host.EnableSsl, Credentials.UserID, Credentials.Password);
            server.SendMail("测试邮件", "this is a test mail from panasia.cn", "83009867@qq.com");
            Assert.IsNotNull(server);
        }

        [TestMethod]
        public void TestLocalhost()
        {
            var sendItem = new SendItem
            {
                To = new List<EmailItem> { new EmailItem { Email = "83009867@qq.com" } },
                Host = new Host { Type = HostType.Local, Address = "localhost" },
                From = new List<EmailItem> { new EmailItem { Email = "xiangxiejin@panasia.cn" } },
                Body = new Body { Content = "this is a test mail from panasia.cn" },
                Subject = new Subject { Content = "测试邮件" }
            };

            sendItem.Send();

            Assert.IsNotNull(sendItem);
        }

    }
}
