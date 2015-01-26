using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Panasia.Gemini.Web.Models.Helpers
{
    public static class SessionExtends
    {
        public static bool IsExist(string controllername, string actionname)
        {
            // org_EmployeesController employees = new org_EmployeesController();
            // Models.
            //if()
            return true;
        }
        public static org_Employees GetUserInfo(this HttpSessionStateBase session)
        {
            if (CurrentSession == null)
            {
                return new org_Employees();
            }

            var userInfo = CurrentSession.GetValue("UserInfo");
            if (userInfo == null)
            {
              //  userInfo = new org_Employees() { SessionID = session.SessionID };
                CurrentSession.AddValue("UserInfo", userInfo);
            }

            return (org_Employees)userInfo;
        }

        public static HttpSessionStateBase CurrentSession = HttpContext.Current.Request.RequestContext.HttpContext.Session;

        //public static void SetUserInfo(this HttpSessionStateBase session,org_Employees userInfo)
        //{
        //    if (CurrentSession != null)
        //    {
        //        userInfo.SessionID = CurrentSession.SessionID;
        //        CurrentSession.AddValue("UserInfo", userInfo);
        //    }
        //}

        public static string GetString(this FormCollection collection, string key)
        {
            string value = string.Empty;
            var fvalue = collection[key];
            if (!string.IsNullOrEmpty(fvalue))
            {
                value = fvalue;
            }
            return value;
        }

        public static object GetValue(this HttpSessionStateBase session, string name)
        {
            return session[name];
        }

        public static void AddValue(this HttpSessionStateBase session, string name, object value)
        {
            session[name] = value;
        }


        public static List<string> GetValidationErrors(this object model)
        {
            List<string> errors = new List<string>();
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, context, results, true);
            foreach (var result in results)
            {
                var name = result.MemberNames.First();
                errors.Add(name + " " + result.ErrorMessage);
            }
            return errors;
        }
        
    }
}