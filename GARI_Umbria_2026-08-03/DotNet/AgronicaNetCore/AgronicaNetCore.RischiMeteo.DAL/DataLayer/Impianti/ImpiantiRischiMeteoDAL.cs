using AgronicaNetCore.Base.Models;
using AgronicaNetCore.RischiMeteo.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.RischiMeteo.DAL.DataLayer.Impianti
{
    /// <summary>
    /// Recupera i dati di un impianto (specie, cultivar) direttamente da <c>Reg_Impianti</c>
    /// tramite join con <c>Cultivar</c> e <c>SpecieVegetali</c>.
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Mapping coltura.codiceSpecie e codiceCultivar.
    /// </summary>
    public class ImpiantiRischiMeteoDAL : BaseDALRischiMeteo, IImpiantiRischiMeteoDAL
    {
        public ImpiantiRischiMeteoDAL(IServiceProvider provider, IStringLocalizer<Messages> localizer)
            : base(provider, localizer) { }

        /// <inheritdoc/>
        public async Task<ImpiantoRischiMeteoEntity?> GetImpiantoAsync(
            string piva, int saCod, int appezza, int idReg,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(piva))    throw new ArgumentException("Specificare la partita IVA.",     nameof(piva));
            if (saCod <= 0)                         throw new ArgumentException("Specificare il codice Sa_Cod.",   nameof(saCod));
            if (appezza <= 0)                       throw new ArgumentException("Specificare il codice Appezza.", nameof(appezza));
            if (idReg <= 0)                         throw new ArgumentException("Specificare un Id_Reg valido.",   nameof(idReg));

            const string sql = @"
                SELECT TOP 1
                    ISNULL(sv.Veg_Cod, 0)   AS Veg_Cod,
                    ISNULL(ri.Cul_Cod, 0)   AS Cul_Cod
                FROM Reg_Impianti ri
                LEFT JOIN Cultivar       cu ON cu.Cul_Cod = ri.CUL_COD
                LEFT JOIN SpecieVegetali sv ON sv.Veg_Cod = cu.Veg_Cod
                WHERE ri.PIVA    = @piva
                  AND ri.SA_COD  = @saCod
                  AND ri.APPEZZA = @appezza
                  AND ri.ID_REG  = @idReg";

            var sqlParams = new Dictionary<string, object>
            {
                ["@piva"]    = piva,
                ["@saCod"]   = saCod,
                ["@appezza"] = appezza,
                ["@idReg"]   = idReg
            };

            try
            {
                var dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sql, sqlParams);
                if (dt.Rows.Count == 0) return null;

                var row    = dt.Rows[0];
                var vegCod = row["Veg_Cod"] != DBNull.Value ? Convert.ToInt32(row["Veg_Cod"]) : 0;
                var culCod = row["Cul_Cod"] != DBNull.Value ? Convert.ToInt32(row["Cul_Cod"]) : 0;

                return new ImpiantoRischiMeteoEntity
                {
                    VegCod = vegCod > 0 ? vegCod : null,
                    CulCod = culCod > 0 ? culCod : null
                };
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
