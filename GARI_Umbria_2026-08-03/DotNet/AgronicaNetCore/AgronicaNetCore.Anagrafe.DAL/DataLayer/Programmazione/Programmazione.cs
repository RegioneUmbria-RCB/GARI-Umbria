using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.Programmazione
{
    public class Programmazione : BaseDALAnagrafe, IProgrammazione
    {
        public Programmazione(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer) { }

        public async Task<DataTable?> GetPlanningPerConfrontoCatastoAsync(string partitaIva, int programmazioneCod, AgronicaCoreParametriServer objParametriServer, bool origine = true, bool ShowCatasto = false, bool showVarieta = false)
        {
            var strSql = "";
            var parSql = new Dictionary<string, object>();
            DataTable? result = null;

            try
            {
                if (string.IsNullOrEmpty(partitaIva))
                    throw new Exception("Specificare la partita iva");

                if (programmazioneCod == 0) throw new Exception("Valorizzare il numero della scheda di planning");

                strSql += "select \n";
                if (ShowCatasto)
                    strSql += "     Prov, Prov_Des, Com, Com_Des, Sezione, Foglio, Numero, Subalterno, \n";
                strSql += "     Veg_cod, Veg_Des,\n";
                if (showVarieta)
                    strSql += "     Cul_Cod, Cul_Des,\n";
                strSql += "     id_Cod, \n";
                if (origine)
                {
                    strSql += "sum(Superficie) as Superficie_Ori \n";
                }
                else
                {
                    strSql += "sum(Superficie) as Superficie_Act \n";
                }
                strSql += "\n";
                strSql += "from( \n";
                strSql += "     select \n";
                strSql += "     b.Id_Cod, \n";
                strSql += "     b.Veg_Cod, \n";
                strSql += "     COALESCE(sv.Veg_Des, ca.descrizione) as Veg_Des, \n";
                strSql += "     b.Cul_Cod, \n";
                strSql += "     coalesce(cc.Cul_Des,'') as Cul_Des, \n";
                strSql += "     c.Prov, \n";
                strSql += "     coalesce(lp.COMUNI_PROV, '') as Prov_Des, \n";
                strSql += "     c.Com, \n";
                strSql += "     coalesce(lp.LOCALITA, '') as Com_Des, \n";
                strSql += "     c.Sezione, \n";
                strSql += "     c.Foglio, \n";
                strSql += "     c.Numero, \n";
                strSql += "     c.Subalterno, \n";
                strSql += "     coalesce(c.Superficie, b.Superficie) as Superficie \n";
                strSql += "\n";
                strSql += "     from Programmazione_Testata a \n";
                strSql += "\n";
                strSql += "     left join programmazione_entita b \n";
                strSql += "     on(a.Piva_SuperUser = b.Piva_SuperUser and a.Programmazione_Cod = b.Programmazione_Cod) \n";
                strSql += "\n";
                strSql += "     left join Programmazione_Particelle c \n";
                strSql += "     on(b.Piva_SuperUser = c.Piva_SuperUser and b.Programmazione_Entita_Cod = c.Programmazione_Entita_Cod) \n";
                strSql += "\n";
                strSql += "     left join Cultivar cc \n";
                strSql += "     on(cc.cul_cod = b.cul_cod) \n";
                strSql += "\n";
                strSql += "     left join SpecieVegetali sv \n";
                strSql += "    on(sv.Veg_Cod = cc.Veg_Cod) \n";
                strSql += "\n";
                strSql += "     left join codici_anagrafe ca ON (b.id_cod = ca.codice) \n";
                strSql += "\n";
                strSql += "     left join ISTAT lp on(lp.PROV = c.Prov AND lp.COM = c.Com) \n";
                strSql += "\n";
                strSql += "     where a.piva = @piva \n";
                if (programmazioneCod > 0)
                    strSql += "     and a.Programmazione_Cod = @programmazioneCod \n";
                if (ShowCatasto)
                    strSql += "     and c.Prov is not null \n";
                strSql += "     ) src \n";
                strSql += "group by \n";
                if (ShowCatasto)
                    strSql += " Prov, Prov_Des, Com, Com_Des, Sezione, Foglio, Numero, Subalterno, \n";
                strSql += " Veg_Cod, Veg_Des,\n";
                if (showVarieta)
                    strSql += " Cul_Cod, Cul_des,\n";
                strSql += " id_Cod \n";

                parSql.Add("@piva", partitaIva);
                if (programmazioneCod > 0)
                    parSql.Add("@programmazioneCod", programmazioneCod);

                result = await GetDataProvider(objParametriServer).ExecuteReadAsync(strSql, parSql);

                if (ShowCatasto && showVarieta)
                {
                    var pk = new List<DataColumn>();
                    foreach (DataColumn col in result.Columns)
                    {
                        switch (col.ColumnName)
                        {
                            case "Prov":
                            case "Com":
                            case "Sezione":
                            case "Foglio":
                            case "Numero":
                            case "Subalterno":
                            case "Veg_cod":
                            case "Cul_Cod":
                            case "id_Cod":
                                pk.Add(col);
                                break;

                        }
                    }
                    result.PrimaryKey = pk.ToArray();
                }
                else
                {
                    if (ShowCatasto)
                    {
                        var pk = new List<DataColumn>();
                        foreach (DataColumn col in result.Columns)
                        {
                            switch (col.ColumnName)
                            {
                                case "Prov":
                                case "Com":
                                case "Sezione":
                                case "Foglio":
                                case "Numero":
                                case "Subalterno":
                                case "Veg_cod":
                                case "id_Cod":
                                    pk.Add(col);
                                    break;

                            }
                        }
                        result.PrimaryKey = pk.ToArray();
                    }
                    else
                    {
                        if (showVarieta)
                        {
                            var pk = new List<DataColumn>();
                            foreach (DataColumn col in result.Columns)
                            {
                                switch (col.ColumnName)
                                {
                                    case "Cul_Cod":
                                    case "Veg_cod":
                                    case "id_Cod":
                                        pk.Add(col);
                                        break;

                                }
                            }
                            result.PrimaryKey = pk.ToArray();
                        }
                        else
                        {
                            var pk = new List<DataColumn>();
                            foreach (DataColumn col in result.Columns)
                            {
                                switch (col.ColumnName)
                                {
                                    case "Veg_cod":
                                    case "id_Cod":
                                        pk.Add(col);
                                        break;

                                }
                            }
                            result.PrimaryKey = pk.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                result = null;
            }
            return result;
        }
    }
}
