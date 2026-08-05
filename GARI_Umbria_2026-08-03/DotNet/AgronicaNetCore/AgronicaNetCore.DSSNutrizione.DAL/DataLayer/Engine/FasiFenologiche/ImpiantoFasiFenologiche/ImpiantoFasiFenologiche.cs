using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Threading.Tasks;
using AgronicaNetCore.Anagrafe.DAL.Base;
using InData.Engine.FasiFenologiche;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaCoreModelsSTD.Engine;
using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.FasiFenologiche.Models;

namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.FasiFenologiche.ImpiantoFasiFenologiche
{
    /// <summary>
    /// Accesso ai dati per la tabella Impianto_Fasi_Fenologiche_Engine.
    /// Implementa <see cref="IImpiantoFasiFenologiche"/>.
    /// </summary>
    public sealed class ImpiantoFasiFenologiche : DAL_Base, IImpiantoFasiFenologiche
    {
        private const string TableName = "Impianto_Fasi_Fenologiche_Engine";

        public ImpiantoFasiFenologiche(IServiceProvider provider) : base(provider)
        {
        }

        /// <inheritdoc/>
        public async Task InserisciFasiAsync(
            ImpiantoInput impianto,
            IReadOnlyList<FaseFenologicaEngine> fasi,
            MetadataAcquisizione metadata,
            AgronicaCoreParametriServer objParametriServer)
        {
            const string sqlInsert = @"
            INSERT INTO Impianto_Fasi_Fenologiche_Engine
                (PIVA, SA_COD, APPEZZA, ID_REG, codice_bbch, descrizione_fase,
                 data_stimata_raggiungimento, isForecast, isOverride, inviato, datainvio,
                 Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica,
                 Validita_Inizio, Validita_Fine)
            VALUES
                (@piva, @saCod, @appezza, @idReg, @codiceBbch, @descrizioneFase,
                 @dataStimata, @isForecast, @isOverride, @inviato, @datainvio,
                 @dataCreazione, @dataModifica, @usernameCreazione, @usernameModifica,
                 @validitaInizio, @validitaFine);";

            var now = DateTime.Now;
            var validitaInizio = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO);
            var validitaFine = DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE);

            foreach (var fase in fasi)
            {
                var expandoObj = new ExpandoObject();
                expandoObj.TryAdd("@piva", impianto.Piva);
                expandoObj.TryAdd("@saCod", impianto.SaCod);
                expandoObj.TryAdd("@appezza", impianto.Appezza);
                expandoObj.TryAdd("@idReg", impianto.IdReg);
                expandoObj.TryAdd("@codiceBbch", fase.CodiceBbch);
                expandoObj.TryAdd("@descrizioneFase", fase.DescrizioneFase);
                expandoObj.TryAdd("@dataStimata", fase.DataStimataRaggiungimento);
                expandoObj.TryAdd("@isForecast", fase.IsForecast);
                expandoObj.TryAdd("@isOverride", fase.IsOverride);
                expandoObj.TryAdd("@inviato", (short)0);
                expandoObj.TryAdd("@datainvio", DBNull.Value);
                expandoObj.TryAdd("@dataCreazione", now);
                expandoObj.TryAdd("@dataModifica", now);
                expandoObj.TryAdd("@usernameCreazione", metadata.UsernameCreazione);
                expandoObj.TryAdd("@usernameModifica", metadata.UsernameCreazione);
                expandoObj.TryAdd("@validitaInizio", validitaInizio);
                expandoObj.TryAdd("@validitaFine", validitaFine);

                try
                {
                    await GetDataProvider(objParametriServer).Execute_WriteAsync(sqlInsert, expandoObj);
                }
                catch (Exception ex)
                {
                    LogError(ex.Message, objParametriServer, ex);
                    throw;
                }
            }
        }

        /// <inheritdoc/>
        public async Task EseguiBulkInsertAsync(
            ImpiantoInput impianto,
            IReadOnlyList<FaseFenologicaEngine> fasi,
            MetadataAcquisizione metadata,
            AgronicaCoreParametriServer objParametriServer)
        {
            var now = DateTime.Now;
            var validitaInizio = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO);
            var validitaFine = DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE);

            var dataTable = BuildDataTable(impianto, fasi, metadata, now, validitaInizio, validitaFine);

            var columnMappings = new Dictionary<string, string>
            {
                ["PIVA"] = "PIVA",
                ["SA_COD"] = "SA_COD",
                ["APPEZZA"] = "APPEZZA",
                ["ID_REG"] = "ID_REG",
                ["codice_bbch"] = "codice_bbch",
                ["descrizione_fase"] = "descrizione_fase",
                ["data_stimata_raggiungimento"] = "data_stimata_raggiungimento",
                ["isForecast"] = "isForecast",
                ["isOverride"] = "isOverride",
                ["inviato"] = "inviato",
                ["datainvio"] = "datainvio",
                ["Data_Creazione"] = "Data_Creazione",
                ["Data_Modifica"] = "Data_Modifica",
                ["Username_Creazione"] = "Username_Creazione",
                ["Username_Modifica"] = "Username_Modifica",
                ["Validita_Inizio"] = "Validita_Inizio",
                ["Validita_Fine"] = "Validita_Fine"
            };

            try
            {
                await GetDataProvider(objParametriServer)
                    .ExecuteBulkInsertAsync(dataTable, TableName, columnMappings, 1000);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        private static DataTable BuildDataTable(
            ImpiantoInput impianto,
            IReadOnlyList<FaseFenologicaEngine> fasi,
            MetadataAcquisizione metadata,
            DateTime now,
            DateTime validitaInizio,
            DateTime validitaFine)
        {
            var dt = new DataTable();
            dt.Columns.Add("PIVA", typeof(string));
            dt.Columns.Add("SA_COD", typeof(int));
            dt.Columns.Add("APPEZZA", typeof(int));
            dt.Columns.Add("ID_REG", typeof(int));
            dt.Columns.Add("codice_bbch", typeof(string));
            dt.Columns.Add("descrizione_fase", typeof(string));
            dt.Columns.Add("data_stimata_raggiungimento", typeof(DateTime));
            dt.Columns.Add("isForecast", typeof(bool));
            dt.Columns.Add("isOverride", typeof(bool));
            dt.Columns.Add("inviato", typeof(short));
            dt.Columns.Add("datainvio", typeof(DateTime));
            dt.Columns.Add("Data_Creazione", typeof(DateTime));
            dt.Columns.Add("Data_Modifica", typeof(DateTime));
            dt.Columns.Add("Username_Creazione", typeof(string));
            dt.Columns.Add("Username_Modifica", typeof(string));
            dt.Columns.Add("Validita_Inizio", typeof(DateTime));
            dt.Columns.Add("Validita_Fine", typeof(DateTime));

            foreach (var fase in fasi)
            {
                dt.Rows.Add(
                    impianto.Piva,
                    impianto.SaCod,
                    impianto.Appezza,
                    impianto.IdReg,
                    fase.CodiceBbch,
                    fase.DescrizioneFase,
                    fase.DataStimataRaggiungimento,
                    fase.IsForecast,
                    false,
                    (short)0,
                    DBNull.Value,           // datainvio
                    now,
                    now,
                    metadata.UsernameCreazione,
                    metadata.UsernameCreazione,
                    validitaInizio,
                    validitaFine);
            }

            return dt;
        }
    }
}


