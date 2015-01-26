using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Panasia.Core.Sys
{
    static class SerialCodes
    {
        public const string UserIDPrefix = "U";
        public const int UserIDLength = 6;

        public const string RoleIDPrefix = "R";
        public const int RoleIDLength = 6; 

        const string ZeroStrings = "0000000000"; 
        static object _CodeLock = new object();

        public static string GetNextUserID(this SysContext db)
        {
            return db.GetNextSerialCode("User", UserIDPrefix, UserIDLength);
        }

        public static string GetNextRoleID(this SysContext db)
        {
            return db.GetNextSerialCode("Role", RoleIDPrefix, RoleIDLength);
        }
        
        public static string GetNextSerialCode(this SysContext db, string typeCode, string prefix, int fixedLength)
        {
            lock (_CodeLock)
            {
                string code = string.Empty;

                var codeItem = db.SerialCodes.FirstOrDefault((s) => s.CodeType == typeCode && s.Prefix == prefix);

                int index = 0;
                if (codeItem == null)
                {
                    //创建
                    SerialCode newItem = new SerialCode
                    {
                        CodeType = typeCode,
                        Prefix = prefix,
                        NextIndex = 2,
                        FixedLength = fixedLength
                    };
                    newItem.ResetCreated();
                    db.SerialCodes.Add(newItem);
                    index = 1;
                }
                else
                {
                    index = codeItem.NextIndex;
                    codeItem.NextIndex += 1;
                    codeItem.ResetUpdated();
                }
                db.SaveChanges();

                int nIndex = index.ToString().Length;
                int zeroCount = fixedLength - nIndex - prefix.Length;
                return prefix + (zeroCount > 0 ? ZeroStrings.Substring(0, zeroCount) : string.Empty) + index.ToString();
            }
        }
    }
}
