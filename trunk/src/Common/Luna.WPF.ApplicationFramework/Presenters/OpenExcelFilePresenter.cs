using System;
using System.Collections.Generic;
using System.Data;
using Caliburn.PresentationFramework.ApplicationModel;
using Luna.WPF.ApplicationFramework.Extensions;
using Luna.WPF.ApplicationFramework.Helpers;
using Luna.WPF.ApplicationFramework.Interfaces;

namespace Luna.WPF.ApplicationFramework.Presenters
{
    public abstract class OpenExcelFilePresenter<T> : Presenter, IOpenExcelFilePresenter
    {
        private readonly IOpenFileService _openFileService;
        protected IList<Exception> _exceptions;
        private string _filePath;
        private IList<T> _foundEntities;
        private bool _ignoreInvalidExcelRows;
        protected Dictionary<string, string[]> _nameOfSheets;
        private bool _overridExistData;
        private Dictionary<string, DataTable> _sheets;

        protected OpenExcelFilePresenter(IOpenFileService openFileService)
        {
            _openFileService = openFileService;
            _openFileService.Filter = "Excel File|*.xls";
        }

        public bool IgnoreInvalidExcelRows
        {
            get { return _ignoreInvalidExcelRows; }
            set
            {
                _ignoreInvalidExcelRows = value;
                NotifyOfPropertyChange(() => IgnoreInvalidExcelRows);
            }
        }

        public bool OverridExistData
        {
            get { return _overridExistData; }
            set
            {
                _overridExistData = value;
                NotifyOfPropertyChange(() => OverridExistData);
            }
        }

        #region IOpenExcelFilePresenter Members

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                NotifyOfPropertyChange(() => FilePath);
            }
        }

        #endregion

        public void OpenFile()
        {
            _foundEntities = new List<T>();
            _exceptions = new List<Exception>();


            if (_openFileService.ShowDialog(this) != true)
                return;

            FilePath = _openFileService.FileName;
            // 获取有效的Sheet表
            Dictionary<string, DataTable> foundSheets = ColumnNameMapping.GetValidExcelSheats(FilePath);
            _sheets = new Dictionary<string, DataTable>(_nameOfSheets.Count);

            foreach (var sheet in _nameOfSheets)
            {
                if (foundSheets.ContainsKey(sheet.Key))
                {
                    _sheets[sheet.Key] = foundSheets[sheet.Key];
                    _sheets[sheet.Key].TableName = sheet.Key;
                    _sheets[sheet.Key].CheckColumnExist(sheet.Value); //检查sheet表中列是否存在
                    _sheets[sheet.Key].BlankRowsRemove(); //除去数据都为空的行
                    _sheets[sheet.Key].AsEnumerable().DuplicateCheckAndExtract(Validation, ValidationRow, DuplicateRow,
                                                                               CatchException);
                }
                if (foundSheets.Count == 0)
                    _exceptions.Add(new Exception("档案格式有误!"));
            }
            ReadingCompleted(_foundEntities);
        }

        /// <summary>
        /// 显示错误信息
        /// </summary>
        /// <param name="ex"></param>
        private void CatchException(Exception ex)
        {
            _exceptions.Add(ex);
        }

        /// <summary>
        /// 显示重复的数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="row"></param>
        private void DuplicateRow(T obj, int row)
        {
            var error = new DuplicateException(new Exception(obj.ToString()), row);
            //bool iscontain = _exceptions.Select(e => e.Message).Contains(error.Message);
            //if (!iscontain)
            _exceptions.Add(error);
        }

        /// <summary>
        /// 显示有效的数据
        /// </summary>
        /// <param name="obj"></param>
        private void ValidationRow(T obj)
        {
            _foundEntities.Add(obj);
        }

        public T Validation(DataRow dataRow)
        {
            return ConvertToEntity((columnName, nullStringCheck) =>
                                       {
                                           string valueString = dataRow.GetStringValue(columnName);

                                           if (nullStringCheck && valueString.IsNullValue())
                                               throw new NoNullAllowedException();

                                           if (!nullStringCheck && valueString.IsNullValue())
                                               return string.Empty;

                                           return valueString;
                                       });
        }

        protected virtual T ConvertToEntity(Func<string, bool, string> getField)
        {
            return default(T);
        }

        protected abstract void ReadingCompleted(IList<T> foundEntities);
    }
}