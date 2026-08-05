using System.Collections.Generic;

namespace AgronicaCoreVisibilitaStd.Models
{
    public sealed class BuildPraticheResult
    {
        public BuildPraticheResult(string sql, Dictionary<string, object> parametri)
        {
            Sql = sql;
            Parametri = parametri;
        }

        public string Sql { get; }

        public Dictionary<string, object> Parametri { get; }
    }
}
