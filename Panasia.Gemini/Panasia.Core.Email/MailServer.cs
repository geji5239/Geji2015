using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panasia.Core.Email
{
    public class MailServer
    {
        #region 属性 Host
        private Host _Host = null;
        public Host Host
        {
            get
            { 
                return _Host;
            }
            set
            {
                _Host = value; 
            }
        }
        #endregion

        #region 属性 Sender
        private Credentials _Sender = null;
        public Credentials Sender
        {
            get
            {
                return _Sender;
            }
            set
            {
                _Sender = value; 
            }
        }
        #endregion
        
        public MailServer()
        {
        }

        /// <summary>
        /// 服务器信息
        /// </summary>
        /// <param name="serverAddress">服务地址</param>
        /// <param name="port">服务端口</param>
        /// <param name="enableSSL">是否启用SSL</param>
        public MailServer(string serverAddress,int port,bool enableSSL=true)
        {
            Host = new Host { Address = Host.Address, Port = port.ToString(), EnableSsl = enableSSL };
        }
        
        /// <summary>
        /// 服务器信息带发送者用户名密码
        /// </summary>
        /// <param name="serverAddress">服务地址</param>
        /// <param name="port">服务端口</param>
        /// <param name="enableSSL">是否启用SSL</param>
        /// <param name="user">用户名</param>
        /// <param name="password">用户密码</param>
        public MailServer(string serverAddress,int port,bool enableSSL,string user,string password)
        {
            Host = new Host { Address = serverAddress, Port = port.ToString(), EnableSsl = enableSSL };
            Sender = new Credentials { UserID = user, Password = password };
        }

        public void SendMail(string title,string content,string to)
        {
            if(Sender == null)
            {
                throw new Exception("请指定邮件服务器的身份验证信息Sender");
            }

            SendMail(title,content, to, Sender.UserID, Sender.Password);
        }

        public void SendMail(string title,string content,string to, string user,string password)
        {
            try
            {
                if(Host ==null)
                {
                    throw new Exception("邮件服务器信息Host没有设置");
                }

                if(string.IsNullOrWhiteSpace(title))
                {
                    throw new ArgumentException("请设置邮件标题",title);
                }
                if(string.IsNullOrWhiteSpace(content))
                {
                    throw new ArgumentException("请设置邮件内容",content);
                }
                if(string.IsNullOrWhiteSpace(to))
                {
                    throw new ArgumentException("邮件接受者不能为空",to);
                }
                if(string.IsNullOrWhiteSpace(user))
                {
                    throw new ArgumentException("邮件服务器的身份验证信息用户不能为空",user);
                }
                if(string.IsNullOrWhiteSpace(password))
                {
                    throw new ArgumentException("邮件服务器的身份验证信息密码不能为空",password);
                }

                var sendItem = new SendItem
                {
                    To = to.CreateItems().ToList(),
                    Host = Host,
                    Credentials = new Credentials { UserID = user.Trim(), Password = password },
                    From = new List<EmailItem> { new EmailItem { Email = user } },
                    Body = new Body { Content = content },
                    Subject = new Subject { Content = title.Trim() }
                };

                sendItem.Send();
                this.Log(string.Format("SendMail Server:{0},Port:{1},Sender:{2},To:{3},Title:{4}",
                    Host.Address, Host.Port, user, to, title));
            }
            catch (Exception ex)
            {
                this.Log(string.Format("SendMail Error Server:{0},Port:{1},Sender:{2},To:{3},Title:{4},Error:{5}",
                    Host.Address, Host.Port, user, to, title, ex.Message));
                throw;
            }
        }
    }
}
