using System;
using System.Data;

namespace InData
{
    public abstract class DataRowWrapper
    {
        public DataRow Row { get; }

        protected DataRowWrapper(DataRow row) => this.Row = row;

        public object GetValue(string columnName)
        {
            return Row.Table.Columns.Contains(columnName) && Row[columnName] != DBNull.Value
                ? Row[columnName]
                : null;
        }
    }
}
