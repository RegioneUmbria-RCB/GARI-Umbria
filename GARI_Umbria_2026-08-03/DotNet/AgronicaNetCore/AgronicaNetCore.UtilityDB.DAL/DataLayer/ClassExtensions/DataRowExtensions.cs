
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace AgronicaNetCore.UtilityDB.DAL.DataLayer.ClassExtensions
{
    public static class DataRowExtensions
    {
        /// <summary>
        /// This method provides access to the values in each of the columns in a given row.
        /// This method makes casts unnecessary when accessing columns.
        /// Additionally, Field supports nullable types and maps automatically between DBNull and
        /// Nullable when the generic type is nullable.
        /// </summary>
        /// <param name="row">The input DataRow</param>
        /// <param name="columnName">The input column name specifying which row value to retrieve.</param>
        /// <returns>The DataRow value for the column specified.</returns>
        public static string GetFieldString(this DataRow row, string columnName, string defaultValue = "") 
        {
            object value = row[columnName];
            return value == DBNull.Value ? defaultValue : value.ToString();
        }
    }
}
