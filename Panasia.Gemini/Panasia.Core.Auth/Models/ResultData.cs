using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Panasia.Core.Auth
{
    public class ResultData
    {
        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        public object Data { get; set; }
        

        public static ResultData CreateError(string error)
        {
            return new ResultData
            {
                HasError = true,
                ErrorMessage = error
            };
        }

        public static ResultData Create(object data)
        {
            return new ResultData
            {
                HasError = false,
                Data =data
            };
        }         
    }
}