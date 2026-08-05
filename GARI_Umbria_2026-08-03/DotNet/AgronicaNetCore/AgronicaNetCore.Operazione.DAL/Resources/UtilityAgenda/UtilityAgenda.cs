using AgronicaNetCore.Base.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Operazione.DAL.Resources.UtilityAgenda
{
    public class UtilityAgenda : IUtilityAgenda
    {
        public decimal GetDoseTrasformata(decimal dose, int udm)
        {
            return udm switch
            {
                (int)TipiEnumerativi.Enum_UnitaMisura.Grammi => dose / 1_000,
                (int)TipiEnumerativi.Enum_UnitaMisura.Milligrammi => dose / 1_000_000,
                (int)TipiEnumerativi.Enum_UnitaMisura.Quintali => dose * 100,
                (int)TipiEnumerativi.Enum_UnitaMisura.Tonnellate => dose * 1_000,
                (int)TipiEnumerativi.Enum_UnitaMisura.Millilitri => dose / 1_000,
                (int)TipiEnumerativi.Enum_UnitaMisura.CentimetriCubi => dose / 1_000,
                (int)TipiEnumerativi.Enum_UnitaMisura.Metri_Cubi => dose * 1_000,
                _ => dose // TipiEnumerativi.Enum_UnitaMisura.Litri, TipiEnumerativi.Enum_UnitaMisura.KG,
                          // TipiEnumerativi.Enum_UnitaMisura.Unita_Seme, TipiEnumerativi.Enum_UnitaMisura.Num_Piante,
                          // TipiEnumerativi.Enum_UnitaMisura.Confezioni, TipiEnumerativi.Enum_UnitaMisura.Numero,
                          // TipiEnumerativi.Enum_UnitaMisura.Numero_Trappole
            };
        }
    }
}
