using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using Luna.Common;
using Luna.Core.Extensions;
using Luna.WPF.ApplicationFramework.Extensions;

namespace Luna.WPF.ApplicationFramework.Helpers
{
    public static class DataRowExt
    {
        public static readonly string[] NullValues = new[] { "[null]", "[NULL]", "[Null]", "null", "", "NULL", "Null" };
        public static readonly string[] FalseValues = new[] { "False", "FALSE", "false", "0", "否", "NO", "no", "No" };
        public static readonly string[] TrueValues = new[] { "True", "TRUE", "true", "1", "是", "Yes", "YES", "yes" };

        public static string GetStringValue(this DataRow dr, string columnName)
        {
            if (!dr.Table.Columns.Contains(columnName))
                throw new Exception(string.Format("{0}表{1}列不存在", dr.Table.TableName, columnName));
            var result = dr[columnName].ToString().Trim();
            //if (result.IsNullOrEmpty())
            //    throw new Exception(string.Format("{0}表{1}列存在空字串或者格式不正確",dr.Table.TableName,columnName));
            return result;
        }

        public static bool IsNullValue<T>(this T value)
        {
            return NullValues.Contains(ReferenceEquals(value, default(T)) ? "" : value.ToString());
        }

        public static T GetValue<T>(this DataRow dr, string columnName)
        {
            var cellValue = dr.GetStringValue(columnName);

            if (typeof(T) == typeof(bool))
            {
                if (FalseValues.Contains(cellValue))
                    cellValue = "FALSE";
                else if (TrueValues.Contains(cellValue))
                    cellValue = "TRUE";
            }

            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(cellValue); ;
        }

        public static string GetStringValue(this DataRow dr, string format, params string[] columns)
        {
            var values = new string[columns.Length];

            for (int i = 0; i < values.Length; i++)
            {
                if (!dr.Table.Columns.Contains(columns[i]))
                    throw new Exception(string.Format("{0}表{1}列不存在", dr.Table.TableName, columns[i]));
                values[i] = dr[columns[i]].ToString().Trim();
            }
            return string.Format(format, values);
        }
    }

    public class ExcelReader
    {
        public static bool Is64BitOperatingSystem()
        {
            return IntPtr.Size == 8;
        }

        private readonly OleDbConnection _conn;
        private readonly OleDbConnection _connNoHeader;
        public ExcelReader(string filePath)
        {
            string provider;
            if (Is64BitOperatingSystem())
            {
                //提供64位汇入支持
                //需要安装Microsoft Access Database Engine 2010 Redistributable
                //下载地址：http://www.microsoft.com/downloads/details.aspx?displaylang=zh-cn&FamilyID=c06b8369-60dd-4b64-a44b-84b371ede16d
                //参考博客：http://blogs.msdn.com/b/farukcelik/archive/2010/06/04/accessing-excel-files-on-a-x64-machine.aspx
                provider = "Microsoft.ACE.OLEDB.12.0";
            }
            else
            {
                provider = "Microsoft.Jet.OLEDB.4.0";
            }
            var strCon = string.Format("Provider={0};Extended Properties=\"Excel 8.0;IMEX=1;HDR=Yes;\";Data Source={1}", provider, filePath);
            _conn = new OleDbConnection(strCon);
            _connNoHeader = new OleDbConnection(string.Format("Provider={0};Extended Properties=\"Excel 8.0;IMEX=1;HDR=No;\";Data Source={1}", provider, filePath));
        }

        public DataSet FillDataset(string textCommand, bool hasHeader)
        {
            var ds = new DataSet();
            var conn = new OleDbConnection();
            try
            {
                conn = hasHeader ? _conn : _connNoHeader;
                conn.Open();
                var cmd = new OleDbDataAdapter(textCommand, conn);
                cmd.Fill(ds, "ds1");
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
            return ds;
        }
        public DataSet FillDataset(string textCommand)
        {
            return FillDataset(textCommand, true);
        }
    }

    public static class ColumnNameMapping
    {
        static private Dictionary<string, string> _objectName2SheetColumnName;
        static private Dictionary<string, string> _sheetColumnName2ObjectName;
        static private Dictionary<string, string> _sheet2TableMapping;
        static private Dictionary<string, string> _table2SheetMapping;
        private static string _mappingFilePath;

        static ColumnNameMapping()
        {
            _objectName2SheetColumnName = new Dictionary<string, string>();
            _sheetColumnName2ObjectName = new Dictionary<string, string>();
            _sheet2TableMapping = new Dictionary<string, string>();
            _table2SheetMapping = new Dictionary<string, string>();

            _mappingFilePath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles("Mapping.xls").First().FullName;

            var excelReader = new ExcelReader(_mappingFilePath);

            var ds = excelReader.FillDataset("Select * from [TableNameMapping$]");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                try
                {
                    //簡體中文 做完全形轉半型後匯有亂碼 先拿掉*********************************
                    //string ExcelSheetName =  Strings.StrConv( dr["ExcelSheetName"].ToString()  ,VbStrConv.Narrow, 0).ToLower().Trim();
                    //string TableName =  Strings.StrConv( dr["ObjectName"].ToString()  ,VbStrConv.Narrow, 0).ToLower().Trim();
                    //**************************************************************************
                    string ExcelSheetName = dr["ExcelSheetName"].ToString().ToLower().Trim();
                    string TableName = dr["ObjectName"].ToString().ToLower().Trim();

                    if (ExcelSheetName.Length == 0 || TableName.Length == 0)
                        continue;
                    _table2SheetMapping.Add(TableName, ExcelSheetName);
                    _sheet2TableMapping.Add(ExcelSheetName, TableName);
                }
                catch
                {
                    throw new Exception("資料表名稱重複!!");
                }
            }

            ds = excelReader.FillDataset("Select * from [ColumnNameMapping$]");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                try
                {
                    //簡體中文 做完全形轉半型後匯有亂碼 先拿掉*********************************
                    //string ExcelSheetName = Strings.StrConv(dr["ExcelSheetName"].ToString(), VbStrConv.Narrow, 0).Trim().ToLower();
                    //string ExcelColName = Strings.StrConv(dr["ExcelColName"].ToString(), VbStrConv.Narrow, 0).Trim().ToLower();
                    //string ObjectName = Strings.StrConv(dr["ObjectName"].ToString(), VbStrConv.Narrow, 0).Trim().ToLower();
                    //******************************************************************************
                    string ExcelSheetName = dr["ExcelSheetName"].ToString().Trim().ToLower();
                    string ExcelColName = dr["ExcelColName"].ToString().Trim().ToLower();
                    string ObjectName = dr["ObjectName"].ToString().Trim().ToLower();

                    if (ExcelSheetName.Length == 0 || ObjectName.Length == 0 || ExcelColName.Length == 0)
                        continue;
                    if (!_sheet2TableMapping.ContainsKey(ExcelSheetName))
                        throw new Exception("資料表不存在!!");
                    _objectName2SheetColumnName.Add(string.Format("{0}_{1}", _sheet2TableMapping[ExcelSheetName], ObjectName), ExcelColName);
                    _sheetColumnName2ObjectName.Add(string.Format("{0}_{1}", ExcelSheetName, ExcelColName), ObjectName);
                }
                catch
                {
                    throw new Exception("資料表欄位名稱重複!!");
                }
            }
        }

        public static void Initial()
        {

        }

        public static string CreateSqlString(string sheetName)
        {
            sheetName = sheetName.ToLower();
            if (!_table2SheetMapping.ContainsKey(sheetName))
                return null;

            var keyOfSheetName = string.Format("{0}_", sheetName);

            var columns = new StringBuilder();
            foreach (var item in _objectName2SheetColumnName)
            {
                var prefixIndex = item.Key.IndexOf(keyOfSheetName);
                if (prefixIndex == 0)
                    columns.AppendFormat("[{0}] as [{1}],", item.Value, item.Key.Substring(keyOfSheetName.Length));
            }

            return string.Format("Select {0} From [{1}$]", columns.Remove(columns.Length - 1, 1), _table2SheetMapping[sheetName]);
        }
        public static string SheetName2TableName(string SheetName)
        {
            if (_sheet2TableMapping.ContainsKey(SheetName))
                return _sheet2TableMapping[SheetName];
            return null;
        }
        public static string TableName2SheetName(string TableName)
        {
            if (_table2SheetMapping.ContainsKey(TableName))
                return _table2SheetMapping[TableName];
            return null;
        }
        public static string SheetColumnName2ObjectName(string SheetName, string ColumnName)
        {
            string s = string.Format("{0}_{1}", SheetName, ColumnName);
            if (_sheet2TableMapping.ContainsKey(s))
                return _sheet2TableMapping[s];
            return null;
        }
        public static string ObjectName2SheetColumnName(string ObjectName, string AttributeNAme)
        {
            String s = string.Format("{0}_{1}", ObjectName, AttributeNAme);
            if (_objectName2SheetColumnName.ContainsKey(s))
                return _objectName2SheetColumnName[s];
            return null;
        }

        public static Dictionary<string, DataTable> GetForecastExcelSheet(string filePath)
        {
            var excelReader = new ExcelReader(filePath);
            var items = new Dictionary<string, DataTable>();

            var ds0 = excelReader.FillDataset("Select * from [CSSQSLSecond$]");
            if (ds0 != null)
            {
                ds0.Tables[0].TableName = TableNames.CSSQSLSecond;
                items[TableNames.CSSQSLSecond] = ds0.Tables[0];
            }

            var ds = excelReader.FillDataset("Select * from [CSSQForeCastSL$] order by TargetDate");
            if (ds != null)
            {
                ds.Tables[0].TableName = TableNames.ForecastServiceLevel;
                items[TableNames.ForecastServiceLevel] = ds.Tables[0];
            }

            var ds1 = excelReader.FillDataset("Select * from [CSSQForeCastTraffic$] order by TrafficDate");
            if (ds1 != null)
            {
                ds1.Tables[0].TableName = TableNames.ForecastTraffic;
                items[TableNames.ForecastTraffic] = ds1.Tables[0];
            }

            return items;
        }

        public static bool ReadAssignmentExcelSheets(string filePath,
            Action<DateTime> importDate, Action<int> dateRangeReadCompleted, Action<object[], string[], int, int> readRow, Action<string, int, IList<int>, object> duplicationRow,
            Func<int, string, Core.Tuple<object, int[]>> duplicationCheck)
        {
            var excelReader = new ExcelReader(filePath);
            var items = new Dictionary<string, DataTable>();

            //离线,假日
            /*foreach (var item in _sheet2TableMapping.Where(o => o.Value == TableNames.ShiftSubEvnet || o.Value == TableNames.ShiftAbsentEvent || o.Value == "seatoccupation"))
            {
                var ds = excelReader.FillDataset(CreateSqlString(item.Value));
                if (ds == null) continue;
                items.Add(item.Value, ds.Tables[0]);
            }*/
            var sheetsFound = false;

            excelReader.FillDataset("Select * from [Shift$]", false).SaftyInvoke(ds =>
            {
                var table = ds.Tables[0].LastBlankRowsRemove();

                var convertedDate = default(DateTime?);
                var shiftColumnStartIndex = default(int?);

                for (var index = 0; index < table.Rows[0].ItemArray.Length; index++)
                {
                    var cellValue = table.Rows[0].ItemArray[index];
                    if (cellValue.IsNull()) continue;

                    DateTime date;

                    if (!DateTime.TryParse(cellValue.ToString(), out date))
                        continue;

                    shiftColumnStartIndex = shiftColumnStartIndex ?? index;

                    if (convertedDate != null)
                    {
                        //日期连续性侦测
                        if (1 < date.Subtract(convertedDate.Value).TotalDays)
                            break;
                    }
                    importDate(date);
                    convertedDate = date;
                }

                if (shiftColumnStartIndex == null) return;

                dateRangeReadCompleted(table.Rows.Count-1);

                var readedHeaders = new List<string>(table.Rows.Count - 1);

                for (var i = 1; i < table.Rows.Count; i++)
                {
                    var itemArray = table.Rows[i].ItemArray;
                    var excelRowIndex = i + 1;

                    var queryParams = new string[shiftColumnStartIndex.Value];

                    for (var j = 0; j < shiftColumnStartIndex.Value; j++)
                        queryParams[j] = itemArray[j].IsNullValue() ? string.Empty : itemArray[j].ToString().Trim();

                    var header = string.Join(",", queryParams);
                    var blankHeader = header == ",";

                    var duplicatedWith = default(Core.Tuple<object, int[]>);

                    if (blankHeader)
                       header = string.Empty;
                    else
                        readedHeaders.ForEach((h, k) =>
                        {
                            if (h == header)
                            {
                                duplicatedWith = duplicationCheck(k + 2, header);
                                return;
                            }
                        });

                    if (blankHeader)
                        duplicationRow(header, excelRowIndex, null, null);
                    else if (duplicatedWith != null)
                        duplicationRow(header, excelRowIndex, duplicatedWith.Item2, duplicatedWith.Item1);
                    else
                        readRow(itemArray, queryParams, excelRowIndex, shiftColumnStartIndex.Value);

                    readedHeaders.Add(header);
                }
                sheetsFound = true;
            });

            return sheetsFound;
        }




        public static Dictionary<string, DataTable> GetValidExcelSheats(string filePath)
        {
            var excelReader = new ExcelReader(filePath);
            var items = new Dictionary<string, DataTable>();
            foreach (var item in _sheet2TableMapping)
            {
                var ds = excelReader.FillDataset(CreateSqlString(item.Value));
                if (ds == null || ds.Tables.Count == 0) continue;
                items.Add(item.Value, ds.Tables[0]);
            }
            return items;
        }
    }
}
