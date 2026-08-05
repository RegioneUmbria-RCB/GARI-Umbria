using System.Data;
using System.Text;
using AgronicaNetCore.Anagrafe.DAL.Resources;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.Anagrafe.DAL.DataLayer.CodificaProdotti
{
    public class CodificaProdotti_APP : BaseDALAnagrafe, ICodificaProdotti_APP
    {
        public CodificaProdotti_APP(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        public async Task<DataTable> ReadAsync(string piva, Enum_Tipo_CAC_Codifica_ProdottiAziendali tipoCodifica, int elemCod, AgronicaCoreParametriServer objParametriServer, bool soloMappati = false)
        {
            var stbQuery = new StringBuilder();
            var parSql = new Dictionary<string, object>();

            stbQuery
                .AppendLine("SELECT")
                .AppendLine(" Elem_Cod,")
                .AppendLine(" Codice_GIAS,")
                .AppendLine(" Desc_GIAS,")
                .AppendLine(" Cod_Prodotto_Cliente,")
                .AppendLine(" Desc_Prodotto_Cliente,")
                .AppendLine(" Categoria_Prodotto_Cliente,")
                .AppendLine(" Piva,")
                .AppendLine(" Cod_Articolo,")
                .AppendLine(" Tipo_Codifica")
                .AppendLine("FROM CAC_Codifica_ProdottiAziendali")
                .AppendLine("WHERE Piva_SuperUser = @pivaSuperuser");

            if (elemCod != 0)
                stbQuery.AppendLine("AND Elem_Cod = @elemCod");
            if (piva != string.Empty)
                stbQuery.AppendLine("AND Piva = @piva");
            if (tipoCodifica != Enum_Tipo_CAC_Codifica_ProdottiAziendali.NessunFiltro)
                stbQuery.AppendLine("AND Tipo_Codifica = @tipoCodifica");
            if (soloMappati)
                stbQuery.AppendLine("AND Codice_Gias <> 0");

            parSql.Add("@pivaSuperuser", objParametriServer.PivaSuperUser);
            parSql.Add("@elemCod", elemCod);
            parSql.Add("@piva", piva);
            parSql.Add("@tipoCodifica", (int)tipoCodifica);

            try
            {
                return await GetDataProvider(objParametriServer)
                    .ExecuteReadAsync(stbQuery.ToString(), parSql);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
