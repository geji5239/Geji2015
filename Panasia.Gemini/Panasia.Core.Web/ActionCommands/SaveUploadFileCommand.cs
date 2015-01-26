using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Panasia.Core.App;

namespace Panasia.Core.Web
{
    public class SaveUploadFileResult : CommandResult
    {
        #region 属性 FileName
        public string FileName
        {
            get
            {
                return GetFieldValue<string>("FileName", string.Empty);
            }
            set
            {
                SetFieldValue("FileName", value);
            }
        }
        #endregion

        #region 属性 SourceName
        public string SourceName
        {
            get
            {
                return GetFieldValue<string>("SourceName", string.Empty);
            }
            set
            {
                SetFieldValue("SourceName", value);
            }
        }
        #endregion
    }

    public class SaveUploadFileCommand : CommandBase
    {
        public override CommandResult CreateResult()
        {
            return new SaveUploadFileResult();
        }
    }
}
