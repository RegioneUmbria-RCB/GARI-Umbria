using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.converters
{
    public class UtilizzoTerrenoComparer : IEqualityComparer<UtilizzoTerreno>
    {
        public bool Equals(UtilizzoTerreno? x, UtilizzoTerreno? y)
        {
            return x.codice == y.codice && x.classType == y.classType;
        }

        public int GetHashCode([DisallowNull] UtilizzoTerreno obj)
        {
            return HashCode.Combine(obj.codice, obj.classType);
        }
    }
}
