using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Panasia.Core;

namespace Panasia.Core.Web
{
    public static class ImportExtensions
    {
        public static void ImportExcel(this string fileName, ExcelToSql config,ExcelImportResult importResult)
        {
            //读文件到Table
            var table = fileName.ReadToTable(config.Source.SheetName);
            //验证格式
            config.Source.CheckFormat(table);
            //转换参数，执行存储
            var result = config.Target.SqlRowImport.Save(config.Source.CreateParameters(table));

            importResult.CancelCount = result.CancelCount;
            importResult.InsertCount = result.InsertCount;
            importResult.UpdateCount = result.UpdateCount; 
        }

        #region 1、读文件到Table
        private static DataTable ReadToTable(this string fileName,string sheetName)
        {
            if (!System.IO.File.Exists(fileName))
            {
                //TODO:文件不存在，这里要取标准的错误提示
                throw new Exception();
            }
            //TODO:读文件到DataTable，如果需要指定读写方法，可以增加参数配置项
            
            return null;
        }
        #endregion

        #region 2、验证格式
        /*        */
        private static void CheckFormat(this ExcelSource excel, DataTable table)
        {
            if (table.Rows.Count < excel.TitleRow)
            {
                //TODO:这里要换成标准语句
                throw new Exception("数据太少");
            }

            var titleRow = table.Rows[excel.TitleRow];
            foreach (var field in excel.Fields)
            {
                //TODO:逐列判断是否存在
            } 
        }
        #endregion

        #region 3、将每行数据转换为SQL参数
        private static IEnumerable<object[]> CreateParameters(this ExcelSource excel, DataTable table)
        {
            for (int i = excel.RowFrom; i < table.Rows.Count; i++)
            {
                if(!excel.IsValidRow(table,i))
                {
                    continue;
                }

                List<object> paras = new List<object>();
                foreach(var f in excel.Fields)
                {
                    paras.Add(f.Name);
                    paras.Add(f.GetFieldValue(table, i));
                }
                yield return paras.ToArray();
            }
            yield break;
        }

        private static bool IsValidRow(this ExcelSource excel,DataTable table,int rowIndex)
        {
            //TODO:先要判断是否为有效数据
            return true;
        }

        private static object GetFieldValue(this ImportField field,DataTable table,int rowIndex)
        {
            //TODO:取每列的值，然后执行转换

            //TODO:如果需要转换，执行转换

            return null;
        }
        #endregion

        #region 4、执行导入
        private static ExcelImportResult Save(this SqlConfig sqlConfig,IEnumerable<object[]> paraItems)
        {
            ExcelImportResult result = new ExcelImportResult();
            //开始导入：导入需要一个较长的时间
            var sqlItem = sqlConfig.SqlItem;
            if (sqlItem == null)
            {
                throw new Exception("SQL配置不存在");
            }
            var conn = SqlData.Current.Connections[sqlItem.ConnectionName];
            if (conn == null)
            {
                throw new Exception("数据连接不存在");
            }

            using (var db = conn.Open())
            {
                using (var trans = db.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in paraItems)
                        {
                            var count = (int)db.ExecuteScalar(sqlItem, item);
                            //TODO:注意SQL执行后应该返回执行情况如下
                            //0、不更新；1、新增成功；2、更新成功；
                            switch (count)
                            {
                                case 0:
                                    result.CancelCount += 1;
                                    break;
                                case 1:
                                    result.InsertCount += 1;
                                    break;
                                case 2:
                                    result.UpdateCount += 1;
                                    break;
                            }
                        }
                        trans.Commit();
                    }
                    catch 
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            return result;
        }
        #endregion
    }
}
