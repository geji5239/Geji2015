using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Panasia.Core
{
    class DataExtension
    {  
         
        private static bool CheckItemsCount(System.Data.Common.DbConnection db, AppModule module, SqlCommand sqlcommand, object[] paras, object expected)
        {
            bool flag = true;
            var table = module.GetTable(sqlcommand.Table);

            foreach (var logickey in table.LogicKeys)
            {
                var countCommand = module.GetCommand(string.Format("{0}.{1}.CountKey_{2}", module.Name, table.Name, logickey.ColumnName.Replace(",", "_")));
                if (countCommand != null)
                {
                    countCommand.Parameters = sqlcommand.Parameters;
                    var count = DBConn.ExecuteScalar(db, countCommand, paras);
                    if (!count.ToString().Equals(expected.ToString()))
                    {
                        return false;
                    }
                }
            }
            return flag;
        }
        public static object ExecuteScalar(AppModule module, SqlCommand sqlcommand, params object[] namevalues)
        {
            var db = module.OpenConnection();
            var result = DBConn.ExecuteScalar(db, sqlcommand, namevalues);
            db.CloseConnection();
            return result;
        }


        public static IEnumerable<T> ExecuteCommand<T>(string moduleName, string sqlName, params object[] namevalues) where T : IEntityBase, new()
        {
            var module = AppData.Current.Modules.FirstOrDefault(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
            var sqlcommand = module.GetCommand(sqlName);

            return ExecuteCommand<T>(module, sqlcommand, namevalues);
        }

        public static IEnumerable<T> ExecuteCommand<T>(AppModule module, string sqlName, params object[] namevalues) where T : IEntityBase, new()
        {
            var sqlcommand = module.GetCommand(sqlName);
            var db = module.OpenConnection();
            var result = DBConn.ExecuteQuery<T>(db, sqlcommand, namevalues).ToList();
            db.CloseConnection();
            return result;
        }

        public static IEnumerable<T> ExecuteCommand<T>(AppModule module, SqlCommand sqlcommand, params object[] namevalues) where T : IEntityBase, new()
        {
            var db = module.OpenConnection();
            var result = DBConn.ExecuteQuery<T>(db, sqlcommand, namevalues).ToList();
            db.CloseConnection();
            return result;
        }

        public static IEnumerable<T> ExecuteCommand<T>(DbConnection db, SqlCommand sqlcommand, params object[] namevalues) where T : IEntityBase, new()
        {
            var result = DBConn.ExecuteQuery<T>(db, sqlcommand, namevalues).ToList();
            return result;
        }

        public static int ExecuteNoneQuery(string moduleName, string sqlName, params object[] namevalues)
        {
            var module = AppData.Current.Modules.FirstOrDefault(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
            var sqlcommand = module.GetCommand(sqlName);
            var db = module.OpenConnection();
            var result = DBConn.ExecuteNoneQuery(db, sqlcommand, namevalues);
            db.CloseConnection();
            return result;
        }

        public static int ExecuteNoneQuery(AppModule module, SqlCommand sqlcommand, params object[] namevalues)
        {
            var db = module.OpenConnection();
            var result = DBConn.ExecuteNoneQuery(db, sqlcommand, namevalues);
            db.CloseConnection();
            return result;
        }

        public static int ExecuteNoneQuery(AppModule module, string sqlName, params object[] namevalues)
        {
            var sqlcommand = module.GetCommand(sqlName);
            var db = module.OpenConnection();
            var result = DBConn.ExecuteNoneQuery(db, sqlcommand, namevalues);
            db.CloseConnection();
            return result;
        }


        public static object ExecuteScalar(string moduleName, string sqlName, params object[] namevalues)
        {
            var module = AppData.Current.Modules.FirstOrDefault(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase));
            var sqlcommand = module.GetCommand(sqlName);
            var db = module.OpenConnection();
            var result = DBConn.ExecuteScalar(db, sqlcommand, namevalues);
            db.CloseConnection();
            return result;
        }

    }
}
