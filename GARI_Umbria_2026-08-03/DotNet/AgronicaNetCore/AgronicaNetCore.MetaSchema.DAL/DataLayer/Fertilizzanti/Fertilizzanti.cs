using System.Data;
using System.Text;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.Resources;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.Fertilizzanti;

public class Fertilizzanti : BaseDALMetaschema, IFertilizzanti
{
    public Fertilizzanti(IServiceProvider provider, IStringLocalizer<Messages> localizer)
        : base(provider, localizer) { }

    public async Task<DataTable> LeggiAsync(
        int ferCod,
        string ferDes,
        int tipoRichiesto,
        int regolamentoCod,
        DateTime validitaInizio,
        DateTime validitaFine,
        bool includiTipologia,
        string statoCod,
        AgronicaCoreParametriServer objParametriServer
    )
    {
        var sqlParams = new Dictionary<string, object>();
        var stb = new StringBuilder();

        stb.AppendLine(" SELECT Fertilizzanti.Fer_Cod, Fertilizzanti.Fer_Des,");
        stb.AppendLine("        Fertilizzanti.Denominazione,");
        stb.AppendLine("        ISNULL(Fertilizzanti.N, 0) AS N,");
        stb.AppendLine("        ISNULL(Fertilizzanti.P2O5, 0) AS P2O5,");
        stb.AppendLine("        ISNULL(Fertilizzanti.K2O, 0) AS K2O,");
        stb.AppendLine("        ISNULL(Fertilizzanti.MgO, 0) AS MgO,");
        stb.AppendLine("        ISNULL(Fertilizzanti.Cu, 0) AS Cu,");
        stb.AppendLine("        ISNULL((SELECT 1 FROM RegolamentixFertilizzanti rf");
        stb.AppendLine(
            "                WHERE rf.fer_cod = Fertilizzanti.fer_cod AND rf.REG_COD = 4), 0) AS Bio"
        );

        if (includiTipologia)
        {
            stb.AppendLine("       ,Tipologie.TP_COD, Tipologie.TP_DES");
        }

        if (tipoRichiesto >= 6)
        {
            stb.AppendLine("       ,TipoFertilizzante.descrizione, TipoFertilizzante.id_tp_fer");
            stb.AppendLine("       ,ISNULL(Effluenti.udm_cod, 0) AS udm_cod");
            stb.AppendLine("       ,ISNULL(Effluenti.eff_cod, 0) AS eff_cod");
        }

        stb.AppendLine(" FROM Fertilizzanti");
        stb.AppendLine(
            " INNER JOIN FertilizzantiXTipologie ON Fertilizzanti.FER_COD = FertilizzantiXTipologie.FER_COD"
        );

        if (!string.IsNullOrEmpty(statoCod))
        {
            stb.AppendLine(" INNER JOIN FertilizzantixAmbitoEstero");
            stb.AppendLine("     ON Fertilizzanti.Fer_Cod = FertilizzantixAmbitoEstero.Fer_Cod");
            stb.AppendLine("     AND FertilizzantixAmbitoEstero.Stato_Cod = @statoCod");
            sqlParams.Add("@statoCod", statoCod);
        }

        if (includiTipologia)
        {
            stb.AppendLine(
                " INNER JOIN Tipologie ON FertilizzantiXTipologie.TP_COD = Tipologie.TP_COD"
            );
        }

        // Bio: regolamentoCod = -2 means filter only biological fertilizers (REG_COD = 4)
        if (regolamentoCod == -2)
        {
            stb.AppendLine(
                " INNER JOIN RegolamentixFertilizzanti ON Fertilizzanti.FER_COD = RegolamentixFertilizzanti.FER_COD"
            );
        }

        if (tipoRichiesto >= 6)
        {
            stb.AppendLine(
                " INNER JOIN FertilizzantixTipoOrganici ON Fertilizzanti.Fer_Cod = FertilizzantixTipoOrganici.FR_COD"
            );
            stb.AppendLine(
                " INNER JOIN TipoFertilizzante ON FertilizzantixTipoOrganici.id_tp_fer = TipoFertilizzante.id_tp_fer"
            );
            stb.AppendLine(
                " LEFT JOIN EffluentixFertilizzanti ON Fertilizzanti.fer_cod = EffluentixFertilizzanti.fer_cod"
            );
            stb.AppendLine(
                "           AND EffluentixFertilizzanti.Regolamento_Cod = @regolamentoCod"
            );
            stb.AppendLine(
                " LEFT JOIN Effluenti ON EffluentixFertilizzanti.Eff_Cod = Effluenti.Eff_Cod"
            );
            stb.AppendLine(
                "           AND EffluentixFertilizzanti.Regolamento_Cod = Effluenti.Regolamento_Cod"
            );
            // @regolamentoCod is referenced by both the JOIN above and the WHERE switch cases below
            sqlParams.Add("@regolamentoCod", regolamentoCod);
        }

        stb.AppendLine(" WHERE Fertilizzanti.Validita_inizio <= @validitaFine");
        stb.AppendLine("   AND Fertilizzanti.Validita_Fine >= @validitaInizio");
        sqlParams.Add("@validitaFine", validitaFine);
        sqlParams.Add("@validitaInizio", validitaInizio);

        if (ferCod != 0)
        {
            stb.AppendLine("   AND Fertilizzanti.FER_Cod = @ferCod");
            sqlParams.Add("@ferCod", ferCod);
        }

        if (!string.IsNullOrEmpty(ferDes))
        {
            stb.AppendLine("   AND Fertilizzanti.FER_DES LIKE @ferDes");
            sqlParams.Add("@ferDes", $"%{ferDes}%");
        }

        if (regolamentoCod == -2)
        {
            stb.AppendLine("   AND RegolamentixFertilizzanti.REG_COD = 4");
        }

        switch (tipoRichiesto)
        {
            case 0: // Tutti i fertilizzanti
                break;
            case 1: // Trattamenti Antibutteratura
                if (includiTipologia)
                    stb.AppendLine("   AND Tipologie.TP_COD = 1");
                break;
            case 2: // Concimazione Fogliare
                if (includiTipologia)
                    stb.AppendLine("   AND Tipologie.TP_COD IN (2, 3)");
                break;
            case 3: // Fertirrigazione
                if (includiTipologia)
                    stb.AppendLine("   AND Tipologie.TP_COD IN (3, 5)");
                break;
            case 4: // Concimazione Organica
                if (includiTipologia)
                    stb.AppendLine("   AND Tipologie.TP_COD IN (6, 7, 8)");
                break;
            case 5: // Concimazione pieno Campo
                if (includiTipologia)
                    stb.AppendLine("   AND Tipologie.TP_COD = 4");
                break;
            case 6: // Ammendanti + Palabili + Liquami del PUA 2007
                stb.AppendLine("   AND FertilizzantixTipoOrganici.id_tp_fer IN (2, 3, 4, 5)");
                stb.AppendLine(
                    "   AND FertilizzantixTipoOrganici.Regolamento_Cod = @regolamentoCod"
                );
                break;
            default: // Ammendanti + Palabili + Liquami del PAN 2012+
                stb.AppendLine("   AND FertilizzantixTipoOrganici.Regolamento_Cod = @regolamentoCod");
                break;
        }

        stb.AppendLine(" ORDER BY Fertilizzanti.Fer_Des");

        try
        {
            return await GetDataProvider(objParametriServer)
                .ExecuteReadAsync(stb.ToString(), sqlParams, new());
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }

    public async Task<DataTable> LeggiMacroElementiAsync(
        List<int> ferCodList,
        AgronicaCoreParametriServer objParametriServer
    )
    {
        var stb = new StringBuilder();
        stb.AppendLine(" SELECT Fer_Cod,");
        stb.AppendLine("        ISNULL(N, 0) AS N,");
        stb.AppendLine("        ISNULL(P2O5, 0) AS P2O5,");
        stb.AppendLine("        ISNULL(K2O, 0) AS K2O,");
        stb.AppendLine("        ISNULL(Cu, 0) AS Cu");
        stb.AppendLine(" FROM Fertilizzanti");
        stb.AppendLine(" WHERE Fer_Cod IN (@ferCodList)");

        var parSqlIn = new Dictionary<string, Dictionary<Type, List<object>>>
        {
            { "@ferCodList", FormatClauseIn(ferCodList) },
        };

        try
        {
            return await GetDataProvider(objParametriServer)
                .ExecuteReadAsync(stb.ToString(), new(), parSqlIn);
        }
        catch (Exception ex)
        {
            LogError(ex.Message, objParametriServer, ex);
            throw;
        }
    }
}
