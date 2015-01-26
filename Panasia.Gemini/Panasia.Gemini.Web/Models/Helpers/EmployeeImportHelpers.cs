using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Entity;
using System.Web.Security;
using System.Web.Mvc;
using System.Configuration;
using Excel;
using System.IO;
using Panasia.Core;
using Panasia.Core.Auth;

namespace Panasia.Gemini.Web.Models.Helpers
{
    public class EmployeeImportHelpers
    {
        ResultData result = new ResultData();
        void GetError(string errorMsg)
        {
            result.HasError = true;
            result.ErrorMessage = errorMsg;
        }
        public DataSet ReadExlToDt(string filePath)
        {
            try
            {
                FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader;
                if (filePath.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
                {
                    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);// Reading from a binary Excel file ('97-2003 format; *.xls)
                }
                else
                {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);// Reading from a OpenXml Excel file (2007 format; *.xlsx)
                }
                excelReader.IsFirstRowAsColumnNames = true;
                DataSet ds = excelReader.AsDataSet();
                excelReader.Close();
                return ds;
            }
            catch (Exception ex)
            {
                GetError(ex.Message);
                return null;
            }
        }
        public DataTable AddColumns(DataTable dt, string[] cols)
        {
            for (int i = 0; i < cols.Count(); i++)
            {
                if (!dt.Columns.Contains(cols[i]))
                    dt.Columns.Add(cols[i]);
            }
            return dt;
        }
        public DataTable MergeTable_RowAdd(DataTable fromTable, DataTable toTable)
        {
            object[] obj = new object[fromTable.Columns.Count];
            for (int i = 0; i < fromTable.Rows.Count; i++)
            {
                fromTable.Rows[i].ItemArray.CopyTo(obj, 0);
                toTable.Rows.Add(obj);
            }
            return toTable;
        }
        public DataTable SelectColsToTable(DataTable dt,string[] cols)
        {
            dt = dt.DefaultView.ToTable(false, cols);
            return dt;
        }
    }
}