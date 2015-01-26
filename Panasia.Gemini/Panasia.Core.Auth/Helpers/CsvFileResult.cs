﻿using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Panasia.Core.Auth.Helpers
{
    /// <summary>
    /// 使用时返回类型是CsvFileResult
    /// </summary>
    public class CsvFileResult : ActionResult
    {
        public string FileName { get; protected set; }
        public DataTable Table { get; protected set; }

        /// <summary>
        /// Converts the columns and rows from a data table into an Microsoft Excel compatible CSV file.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="fileName">The full file name including the extension.</param>
        public CsvFileResult(DataTable dataTable, string fileName)
        {
            Table = dataTable;
            FileName = fileName;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            StringBuilder csv = new StringBuilder(10 * Table.Rows.Count * Table.Columns.Count);

            for (int c = 0; c < Table.Columns.Count; c++)
            {
                if (c > 0)
                    csv.Append(",");
                DataColumn dc = Table.Columns[c];
                string columnTitleCleaned = CleanCSVString(dc.ColumnName);
                csv.Append(columnTitleCleaned);
            }
            csv.Append(Environment.NewLine);
            foreach (DataRow dr in Table.Rows)
            {
                StringBuilder csvRow = new StringBuilder();
                for (int c = 0; c < Table.Columns.Count; c++)
                {
                    if (c != 0)
                        csvRow.Append(",");

                    object columnValue = dr[c];
                    if (columnValue == null)
                        csvRow.Append("");
                    else
                    {
                        string columnStringValue = columnValue.ToString();

                        string cleanedColumnValue = CleanCSVString(columnStringValue);

                        if (columnValue.GetType() == typeof(string) && !columnStringValue.Contains(","))
                        {
                            cleanedColumnValue = "=" + cleanedColumnValue; // Prevents a number stored in a string from being shown as 8888E+24 in Excel. Example use is the AccountNum field in CI that looks like a number but is really a string.
                        }
                        csvRow.Append(cleanedColumnValue);
                    }
                }
                csv.AppendLine(csvRow.ToString());
            }

            HttpResponseBase response = context.HttpContext.Response;
            response.Clear();
            response.ContentType = "text/csv";
            response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(FileName, Encoding.GetEncoding("utf-8")));
            response.ContentEncoding = System.Text.Encoding.GetEncoding("gb2312");
            response.AddHeader("Pragma", "public");
            response.Write(csv.ToString());
            response.End();
            context.HttpContext.ApplicationInstance.CompleteRequest();
        }

        protected string CleanCSVString(string input)
        {
            string output = "\"" + input.Replace("\"", "\"\"").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", "") + "\"";
            return output;
        }
    }
}
