using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Data.SqlClient;
using SqlConnection = System.Data.SqlClient.SqlConnection;

namespace Panasia.Core.Auth.Helpers
{
    public static class DBHelper
    {
        public static IQueryable<T> DataSorting<T>(IQueryable<T> source, string sortExpression, string sortDirection)
        {
            string sortingDir = "OrderBy";
            if (sortDirection.ToUpper().Trim() == "ASC")
                sortingDir = "OrderBy";
            else if (sortDirection.ToUpper().Trim() == "DESC")
                sortingDir = "OrderByDescending";
            ParameterExpression param = Expression.Parameter(typeof(T), sortExpression);
            PropertyInfo pi = typeof(T).GetProperty(sortExpression);
            Type[] types = new Type[2];
            types[0] = typeof(T);
            types[1] = pi.PropertyType;
            Expression expr = Expression.Call(typeof(Queryable), sortingDir, types, source.Expression, Expression.Lambda(Expression.Property(param, sortExpression), param));
            IQueryable<T> query = source.AsQueryable().Provider.CreateQuery<T>(expr);
            return query;
        }

        public static IQueryable<T> DataPaging<T>(IQueryable<T> source, int pageNumber, int pageSize)
        {
            return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        public static IQueryable<T> SortingAndPaging<T>(IQueryable<T> source, string sortExpression, string sortDirection, int pageNumber, int pageSize)
        {
            IQueryable<T> query = DataSorting<T>(source, sortExpression, sortDirection);
            return DataPaging(query, pageNumber, pageSize);
        }

        /// <summary>
        /// 客户端数据导出
        /// </summary>
        /// <typeparam name="T">要导出的数据类型</typeparam>
        /// <param name="values">要导出的数据集</param>
        /// <param name="fields">“属性-标题”集</param>
        /// <param name="targetFile">目标文件</param>
        /// <returns></returns>
        /// <remarks>
        /// 1、除数据可通过“ToString()”转换外，尚不支持其他自定义转换，如将“bool”转换为“是/否”，或将数字转换为字符串值等
        /// 2、尚不支持内部属性（属性的内部属性）的导出
        /// </remarks>
        public static CsvFileResult Export<T>(this ICollection<T> values, IList<ExportField> fields, string targetFile)
        {
            int valueItemCount = fields.Count;
            IList<MethodInfo> methods = new MethodInfo[valueItemCount];
            DataTable table = new DataTable();
            Type type = typeof(T);
            for (int i = 0; i < valueItemCount; i++)
            {
                table.Columns.Add(fields[i].Title);
                methods[i] = type.GetProperty(fields[i].Property).GetGetMethod();
            }

            int index;
            DataRow row;
            foreach (T item in values)
            {
                row = table.NewRow();
                for (index = 0; index < valueItemCount; index++)
                {
                    row[index] = methods[index].Invoke(item, null);
                }
                table.Rows.Add(row);
            }
            return new CsvFileResult(table, targetFile);
        }

        /// <summary>
        /// 使用LinqToExcel实现读取上传的excel文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="Filepath"></param>
        /// <returns></returns>
        public static List<T> ReadExcel<T>(String Filepath)
        {
            List<T> list = null;
            //var excel = new ExcelQueryFactory(Filepath);
            //list = excel.Worksheet<T>(0).ToList();
            return list;
        }

        /// <summary>
        /// 通用导出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="fields"></param>
        /// <param name="targetFile"></param>
        /// <returns></returns>
        public static DataTable CommonExport<T>(this ICollection<T> values, IList<ExportField> fields, string targetFile)
        {
            int valueItemCount = fields.Count;
            IList<MethodInfo> methods = new MethodInfo[valueItemCount];
            DataTable table = new DataTable();
            Type type = typeof(T);
            for (int i = 0; i < valueItemCount; i++)
            {
                table.Columns.Add(fields[i].Title);
                methods[i] = type.GetProperty(fields[i].Property).GetGetMethod();
            }

            int index;
            DataRow row;
            foreach (T item in values)
            {
                row = table.NewRow();
                for (index = 0; index < valueItemCount; index++)
                {
                    row[index] = methods[index].Invoke(item, null);
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }

    public static class DBUtil
    {
        private static string connString = "server = 10.1.15.235; database = PA_Sys; uid =pguser; pwd =sa123456 ";
        private static System.Data.SqlClient.SqlConnection con = null;
        public static System.Data.SqlClient.SqlConnection DBOpen()
        {
            try
            {
                con = new System.Data.SqlClient.SqlConnection(connString);
                con.Open();
                return con;
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        public static void DBClose()
        {
            try
            {
                if (con != null)
                {
                    con.Close();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
