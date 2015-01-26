using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Maticsoft.DBUtility;
using Panasia.Gemini.FL.Data.Models;
using System.Reflection;

namespace Panasia.Gemini.FL.Data.Common
{
    public class Common
    {
        public DataSet GetNumericColumnNameByTableName(string TableName)
        {
            string sql = string.Format(@"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = '{0}' and DATA_TYPE IN ('INT','DECIMAL','MONEY','NUMERIC','FLOAT') AND COLUMNPROPERTY(OBJECT_ID('{0}'),COLUMN_NAME,'IsIdentity')=0", TableName);
            return DbHelperSQL.Query(sql);
        }

        public string GetPKColumn(string TableName)
        {
            DataSet dsKeys = DbHelperSQL.RunProcedure("dbo.sp_pkeys", new[] { new System.Data.SqlClient.SqlParameter("@table_name", TableName) }, "pkeys");
            string key = "";
            if (dsKeys != null && dsKeys.Tables[0].Rows.Count > 0)
            {
                key = System.Data.DataTableExtensions.AsEnumerable(dsKeys.Tables[0]).Where(pkeys => pkeys["COLUMN_NAME"].ToString() != "Version")
                            .OrderBy(pkeys => Convert.ToInt32(pkeys["KEY_SEQ"]))
                            .Select(pkeys => pkeys["COLUMN_NAME"].ToString()).Take(1).First();
            }
            return key;
        }

        public Panasia.Gemini.FL.Data.Models.hr_Department GetDepartmentRootByDepartmentID(string DeptID)
        {
            hr_Department rootDepartment = new hr_Department();
            GetDepartmentRootByDepartmentID(new SysContext(), DeptID, ref rootDepartment);
            return rootDepartment;
        }

        private void GetDepartmentRootByDepartmentID(SysContext db, string DeptID, ref hr_Department rootDepartment)
        {
            hr_Department dept = db.hr_Department.Where(d => d.DeptID == DeptID).SingleOrDefault();
            if (dept != null)
            {
                if (dept.ParentID != "")
                {
                    GetDepartmentRootByDepartmentID(db, dept.ParentID, ref rootDepartment);
                }
                else
                {
                    rootDepartment = dept;
                }
            }
        }

        public string GetValueFromEntity(string PropertyName, object Entity)
        {
            Type t = Entity.GetType();
            PropertyInfo p = t.GetProperty(PropertyName);
            string value = "";
            if (p != null)
            {
                value = p.GetValue(Entity).ToString();
            }
            return value;
        }

        /// <summary>
        /// 把一个实例装进DataTable
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataTable EntityToDataTable(object entity)
        {
            Type entityType = entity.GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            //生成DataTable的structure
            DataTable dt = new DataTable();
            for (int i = 0; i < entityProperties.Length; i++)
            {
                Type t = entityProperties[i].PropertyType;
                if (entityProperties[i].PropertyType.IsGenericType && entityProperties[i].PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    t = entityProperties[i].PropertyType.GetGenericArguments()[0];
                }

                dt.Columns.Add(entityProperties[i].Name, t);
            }
            object[] entityValues = new object[entityProperties.Length];
            for (int i = 0; i < entityProperties.Length; i++)
            {
                entityValues[i] = entityProperties[i].GetValue(entity, null);
            }
            dt.Rows.Add(entityValues);
            return dt;
        }

        public DataSet Query(string tableName, string where, string sort, params string[] columns)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.AppendFormat("select {0} from {1}", string.Join(",", columns), tableName);
            if (!string.IsNullOrEmpty(where.Trim()))
            {
                strSql.AppendFormat(" where {0}", where);
            }
            if (!string.IsNullOrEmpty(sort.Trim()))
            {
                strSql.AppendFormat(" order by {0}", sort);
            }

            return DbHelperSQL.Query(strSql.ToString());
        }
    }
}
