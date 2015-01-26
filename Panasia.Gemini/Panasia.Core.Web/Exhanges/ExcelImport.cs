using System;
using System.Collections.Generic;
using Panasia.Core.App;
using Panasia.Core;
using System.Xml.Serialization;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Panasia.Core.Web
{
    #region 导出结果
    public class ExcelImportResult : CommandResult, IActionCommandResult
    {
        #region 属性 InsertCount
        public int InsertCount
        {
            get
            {
                return GetFieldValue<int>("InsertCount", 0);
            }
            set
            {
                SetFieldValue("InsertCount", value);
            }
        }
        #endregion

        #region 属性 UpdateCount
        public int UpdateCount
        {
            get
            {
                return GetFieldValue<int>("UpdateCount", 0);
            }
            set
            {
                SetFieldValue("UpdateCount", value);
            }
        }
        #endregion

        #region 属性 CancelCount
        public int CancelCount
        {
            get
            {
                return GetFieldValue<int>("CancelCount", 0);
            }
            set
            {
                SetFieldValue("CancelCount", value);
            }
        }
        #endregion
        
        public System.Web.Mvc.ActionResult GetResult()
        {
            //TODO:这里要换成标准语句"导入记录：" + "导入成功" + result.Data.InsertCount + "条;导入更新" + result.Data.UpdateCount + "条;导入取消" + result.Data.CancelCount + "条"
            return new ContentResult { Content = "" };
        }
    }
    #endregion

    public class ExcelImportCommand:CommandBase
    {
        #region 属性 Config
        private ExcelToSql _Config = null;
        [XmlElement("Config")]
        public ExcelToSql Config
        {
            get
            {
                return _Config;
            }
            set
            {
                _Config = value;
                RaisePropertyChanged(() => Config);
            }
        }
        #endregion
        
        #region 导出
        [CommandConfig("ExcelImportCommand", "Excel导入")]
        public static CommandBase CreateCommand(string configDoc)
        {
            if (string.IsNullOrEmpty(configDoc))
            {
                return new ExcelImportCommand();
            }
            return configDoc.ToXObject<ExcelImportCommand>();
        }
        #endregion

        public override CommandResult CreateResult()
        {
            return new ExcelImportResult();
        }

        public override void OnExecute(ICommandContext context, CommandConfig config, CommandResult result)
        {
            var importResult = result as ExcelImportResult;
            //TODO:取文件，不考虑多个文件情况

            //TODO:存文件到上传临时文件夹 ~/tempfiles，考虑到文件可能重名，保存文件名应该唯一（可以用GUID）
            string fileName = "";

            fileName.ImportExcel(Config,importResult); 
        }
        
        public static ExcelImportResult OnExecute(string fileName)
        {
            var importResult =  new ExcelImportResult();
            var items = new List<object>();
            //手动加个数据测试
            ExcelToSql excelConfig = new ExcelToSql();
            ExcelSource source = new ExcelSource();
            source.SheetName = "用户清单";
            excelConfig.Source = source;
            SqlTarget target = new SqlTarget();
            SqlConfig sqlConfig = new SqlConfig();
            sqlConfig.CommandName = "ImportRecord";
            sqlConfig.GroupName = "系统安全";
            sqlConfig.TableName = "[dbo].[sys_User]";
            //SqlItem sqlItem1 = new SqlItem();
            sqlConfig.Parameters = null;
            target.SqlRowImport = sqlConfig;
            excelConfig.Target = target;

            ImportSource importSource = new ImportSource();
            importSource.RowFrom = 2;
            importSource.TitleRow = 1;
            CollectionBase<ImportField> fieldList = new CollectionBase<ImportField>();
            ImportField field1 = new ImportField();
            field1.Name = "UserID"; field1.IsRequired = true; field1.Title = "用户ID"; field1.AllowEmpty = false;
            //importField.Converter = string.Empty; importField.ConverterParameter = string.Empty;暂时不考虑转换
            ImportField field2 = new ImportField();
            field2.Name = "UserName"; field2.IsRequired = true; field2.Title = "登录名"; field2.AllowEmpty = false;
            ImportField field3 = new ImportField();
            field3.Name = "FullName"; field3.IsRequired = true; field3.Title = "用户名"; field3.AllowEmpty = false;
            ImportField field4 = new ImportField();
            field4.Name = "Email"; field4.IsRequired = true; field4.Title = "邮箱"; field4.AllowEmpty = false;
            ImportField field5 = new ImportField();
            field5.Name = "IsValid"; field5.IsRequired = true; field5.Title = "是否有效"; field5.AllowEmpty = false;
            fieldList.Add(field1); fieldList.Add(field2); fieldList.Add(field3); fieldList.Add(field4); fieldList.Add(field5);
            importSource.Fields = fieldList;
            

            Dictionary<string, ExcelToSql> directory = new Dictionary<string, ExcelToSql>();//根据sheetName判断调用哪个配置参数,系统初始读取不同配置加载到内存,静态全局
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            directory.Add(source.SheetName,excelConfig);
            //检查配置是否完整

            //读文件
            string strCon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=0'";
            using (OleDbConnection myConn = new OleDbConnection(strCon))
            {
                DataTable dtTable = new DataTable();
                try
                {
                    myConn.Open();
                    System.Data.DataTable dt = myConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    foreach (DataRow row in dt.Rows)//多少个sheets
                    {
                        string sheetName = row["TABLE_NAME"].ToString();//sheetName
                        string name = sheetName.TrimEnd(new char[] { '$' });
                        if (!directory.ContainsKey(name))
                        {
                            throw new Exception("上传文件工作表名和配置项不对应");
                        }
                        ExcelToSql config = directory[name];

                        var sqlItem = config.Target.SqlRowImport.SqlItem;
                        if (sqlItem == null)
                        {
                            throw new Exception("SQL配置不存在");
                        }
                        var conn = SqlData.Current.Connections[sqlItem.ConnectionName];
                        if (conn == null)
                        {
                            throw new Exception("数据连接不存在");
                        }
                        string commandText = "select * from [" + sheetName + "]";//SQL语句
                        OleDbDataAdapter odp = new OleDbDataAdapter(commandText, myConn);
                        odp.Fill(dtTable);
                        var rows = dtTable.Rows;
                        var columns = dtTable.Columns;
                        int fromIndex = config.Source.RowFrom;//table内容行
                        int titleRowNumber = config.Source.TitleRow;//table标题行
                        var requiredFields = config.Source.Fields.Where(x => x.IsRequired);
                        var allowEmptyFields = config.Source.Fields.Where(x =>x.AllowEmpty == false);//允许为空应该是判断不允许为空字段值
                        List<string> titles = new List<string>();
                        for (int i = 0; i < columns.Count; i++)
                        {
                            string titleName = rows[titleRowNumber][i].ToString();//读取标题行
                            titles.Add(titleName);
                        }
                        
                        foreach (var importField in requiredFields)//标题Title限制条件,不符合直接取消导入
                        {
                            if (!titles.Contains(importField.Title))
                            {
                                throw new Exception("上传文件工作表字段格式错误");
                            }
                        }

                        for (int i = fromIndex; i < rows.Count; i++)//内容是否允许不为空判断,逻辑todo
                        {
                            for (int j = 0; j < columns.Count; j++)
                            {
                                string value = rows[i][j].ToString();
                                foreach (var importField in allowEmptyFields)//判断每个Name限制条件,不符合直接取消导入
                                {
                                    if (!importField.AllowEmpty && value.IsNullOrEmpty())
                                    {

                                    }
                                }
                                parameter.Add(columns[j].ToString(), value);
                                items.Add(parameter);//添加sql参数
                            }
                        }  
                        
                        //开始导入：导入需要一个较长的时间
                        using (var db = conn.Open())
                        {
                            using (var trans = db.BeginTransaction())
                            {
                                try
                                {
                                    foreach (var item in items)
                                    {
                                        var count = (int)db.ExecuteScalar(sqlItem, items.ToArray());
                                        //0、不更新；1、新增成功；2、更新成功；
                                        switch (count)
                                        {
                                            case 0:
                                                importResult.CancelCount += 1;
                                                break;
                                            case 1:
                                                importResult.InsertCount += 1;
                                                break;
                                            case 2:
                                                importResult.UpdateCount += 1;
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
                        }//结束导入
                    }
                }
                catch (Exception ex)
                {
                    dtTable = new DataTable();
                }
            }
            return importResult;
        }
    }

    
}
