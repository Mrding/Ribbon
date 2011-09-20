using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Luna.Core.Extensions;

namespace Luna.WPF.ApplicationFramework.Extensions
{
    public class RowException : Exception
    {
        public RowException(int rowIndex, Exception innerException)
            : base(innerException.Message, innerException)
        {
            Row = rowIndex;
        }

        public int Row { get; set; }

        public override string Message
        {
            get { return string.Format("{1} at row {0}", Row, base.Message); }
        }
    }

    public class DuplicateException : Exception
    {
        public int Row { get; set; }

        public DuplicateException(Exception innerException, int rowindex)
            : base(innerException.Message)
        {
            Row = rowindex;
        }

        public override string Message
        {
            get { return string.Format("Excel中第{0}行重复:{1}", Row, base.Message); }
        }
    }

    public static class ExcelRowProvingExt
    {
        //去空白行
        public static DataTable BlankRowsRemove(this DataTable dt)
        {

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int count = 1;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (dt.Rows[i][j] == DBNull.Value)
                    {

                        if (count == dt.Columns.Count)
                        {
                            dt.Rows.RemoveAt(i);
                            i--;

                        }
                        count++;
                    }
                }
            }
            return dt;
        }
        /// <summary>
        /// 移除Excel最后几行空白行
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataTable LastBlankRowsRemove(this DataTable dt)
        {
            var lastIndex = dt.Rows.Count - 1;
            while (lastIndex != -1)
            {
                if (dt.Rows[lastIndex].ItemArray.Count(o => o == DBNull.Value || string.IsNullOrEmpty(o.ToString().Trim())) == dt.Rows[lastIndex].ItemArray.Length)
                {
                    dt.Rows.RemoveAt(lastIndex);
                    lastIndex--;
                }
                else
                    break;
            }
          
            return dt;
        }
        /// <summary>
        /// 检查列名是否存在
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static void CheckColumnExist(this DataTable table, params string[] columnName)
        {
            foreach (var s in columnName)
            {
                if (!table.Columns.Contains(s))
                    throw new Exception(string.Format("{0}表{1}列不存在", table.TableName, s));
            }

        }

        /// <summary>
        /// 检查列名重复，根据特定的validation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rows"></param>
        /// <param name="validation"></param>
        /// <returns></returns>
        public static void DuplicateCheckAndExtract<T>(this IEnumerable<DataRow> rows, Func<DataRow, T> validation, Action<T> validRowFound, Action<T, int> DuplicateRowFound, Action<Exception> catchException)
        {
            var rowCount = 2;
            var all = rows.Select(r =>
                                      {
                                          try
                                          {
                                              return validation(r);
                                          }
                                          catch (Exception e)
                                          {
                                              catchException(new RowException(rowCount, e));
                                              return default(T);
                                          }
                                          finally
                                          {
                                              rowCount++;
                                          }

                                      }).Where(o => o.IsNotNull()).ToArray();

            for (var i = 0; i < all.Length; i++)
            {
                var item = all[i];

                if (all.Any(o => !ReferenceEquals(o, item) && o.Equals(item)))
                {
                    DuplicateRowFound(item, i + 1);
                }
                else
                {
                    validRowFound(item);
                }
            }
        }
    }

}
