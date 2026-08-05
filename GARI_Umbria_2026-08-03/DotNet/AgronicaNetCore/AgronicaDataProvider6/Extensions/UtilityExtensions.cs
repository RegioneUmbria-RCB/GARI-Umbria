using System.Data;
using System.Dynamic;

namespace AgronicaDataProvider6.Extensions
{
    public static class UtilityExtensions
    {

        public static List<ExpandoObject> ToExpandoObjectList(this DataTable dt)
        {

            var result = new List<ExpandoObject>();

            var colCount = dt.Columns.Count;
            foreach (DataRow r in dt.Rows)
            {
                var expando = new ExpandoObject();
                var expandoDic = (IDictionary<string, object>)expando!;

                for (int i = 0; i <= colCount - 1; i++)
                {
                    var key = r.Table.Columns[i].ColumnName.ToString();
                    var val = r[key];
                    expandoDic[key] = val;
                }
                result.Add(expando);
            }

            return result;
        }


        public static List<IDictionary<string, object>> ToDictionaryList(this DataTable dt)
        {
            List<IDictionary<string, object>> listaFinale = new List<IDictionary<string, object>>();

            var colCount = dt.Columns.Count;

            foreach (DataRow r in dt.Rows)
            {
                var objExpando = new System.Dynamic.ExpandoObject();
                IDictionary<string, object> obj = objExpando!;

                for (int i = 0; i <= colCount - 1; i++)
                {
                    var key = r.Table.Columns[i].ColumnName.ToString();
                    var val = r[key];
                    obj[key] = val;
                }

                listaFinale.Add(obj);
            }

            return listaFinale;
        }
    }
}