using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Text;
using AgronicaNetCore.Anagrafe.DAL.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.CodificheCAC.DAL.Model;
using AgronicaNetCore.CodificheCAC.DAL.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.CodificheCAC.DAL.DataLayer
{
    public class CodificheCAC : DAL_Base, ICodificheCAC
    {
        private readonly string _ID = "ID";
        private readonly string _sistemaCod = "Sistema_Cod";
        private readonly string _codiceEsterno = "Codice_Esterno";
        private readonly string _descrizioneEsterno = "Descrizione_Esterno";
        private readonly string _tabellaGias = "Tabella_Gias";
        private readonly string _codiceGias = "Codice_Gias";
        private readonly string _dataCreazione = "Data_Creazione";
        private readonly string _dataModifica = "Data_Modifica";
        private readonly string _usernameCreazione = "Username_Creazione";
        private readonly string _usernameModifica = "Username_Modifica";
        private readonly string _validitaInizio = "Validita_Inizio";
        private readonly string _validitaFine = "Validita_Fine";
        private readonly string _dataValiditaInizio = "1900-01-01 00:00:00.000";
        private readonly string _dataValiditaFine = "2100-12-31 00:00:00.000";

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CodificheCAC> _logger;
        private readonly IStringLocalizer<Messages> _localizer;

        public CodificheCAC(IServiceProvider serviceProvider, ILogger<CodificheCAC> logger, IStringLocalizer<Messages> localizer) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _localizer = localizer;
        }
        public async Task<DataTable> GetCodificaCACAsync(AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            DataTable dt;

            // Select all columns
            stbQuery.AppendLine("SELECT * FROM CAC_Codifica_Dati_SistemiEsterni");

            try
            {
                var result = new List<CodificaCACModel>();
                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(stbQuery.ToString());

                foreach (DataRow row in dt.Rows)
                {
                    result.Add(new CodificaCACModel
                    {
                        ID = row.Field<int>(_ID),
                        Sistema_Cod = row.Field<int>(_sistemaCod),
                        Codice_Esterno = row.Field<string?>(_codiceEsterno),
                        Descrizione_Esterno = row.Field<string?>(_descrizioneEsterno),
                        Tabella_Gias = row.Field<string?>(_tabellaGias),
                        Codice_Gias = row.Field<string?>(_codiceGias),
                        Data_Creazione = row.Field<DateTime>(_dataCreazione),
                        Data_Modifica = row.Field<DateTime>(_dataModifica),
                        Username_Creazione = row.Field<string?>(_usernameCreazione),
                        Username_Modifica = row.Field<string?>(_usernameModifica),
                        Validita_Inizio = row.Field<DateTime>(_validitaInizio),
                        Validita_Fine = row.Field<DateTime>(_validitaFine),
                        Sistema = ConvertSistemaCodToEnum(row.Field<int>(_sistemaCod).ToString()),
                    });
                }
                return ConvertToDataTable(result);
            }
            catch (Exception ex)
            {
                throw new Exception(_localizer["ErrorRetrievingData"], ex);
            }
        }

        public async Task<bool> InsertCodificaCACAsync(CodificaCACModel model, AgronicaCoreParametriServer objParametriServer)
        {
            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("INSERT INTO CAC_Codifica_Dati_SistemiEsterni");
            stbQuery.AppendLine("(Sistema_Cod, Codice_Esterno, Descrizione_Esterno, Tabella_Gias, Codice_Gias,");
            stbQuery.AppendLine(" Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine)");
            stbQuery.AppendLine("VALUES");
            stbQuery.AppendLine("(@sistemaCod, @codiceEsterno, @descrizioneEsterno, @tabellaGias, @codiceGias,");
            stbQuery.AppendLine(" GETDATE(), GETDATE(), @usernameCreazione, @usernameModifica, @validitaInizio, @validitaFine)");

            try
            {
                string displayName = await GetUserFromCF(objParametriServer);

                DateTime? validitaInizio = model.Validita_Inizio;
                DateTime? validitaFine = model.Validita_Fine;

                if (model.Validita_Inizio == null)
                {
                    validitaInizio = DateTime.ParseExact(_dataValiditaInizio, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                }

                if (model.Validita_Fine == null)
                {
                    validitaInizio = DateTime.ParseExact(_dataValiditaFine, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                }

                var expandoObj = new ExpandoObject();

                expandoObj.TryAdd("@sistemaCod", model.Sistema_Cod);
                expandoObj.TryAdd("@codiceEsterno", model.Codice_Esterno);
                expandoObj.TryAdd("@descrizioneEsterno", model.Descrizione_Esterno);
                expandoObj.TryAdd("@tabellaGias", model.Tabella_Gias);
                expandoObj.TryAdd("@codiceGias", model.Codice_Gias);
                expandoObj.TryAdd("@usernameCreazione", displayName);
                expandoObj.TryAdd("@usernameModifica", displayName);
                expandoObj.TryAdd("@validitaInizio", validitaInizio);
                expandoObj.TryAdd("@validitaFine", validitaFine);

                return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                throw new Exception(_localizer["ErrorInsertingData"], ex);
            }
        }

        public async Task<bool> UpdateCodificaCACAsync(CodificaCACModel model, AgronicaCoreParametriServer objParametriServer)
        {
            if (model.ID <= 0)
                throw new Exception(_localizer["InvalidID"]);

            string displayName = await GetUserFromCF(objParametriServer);

            var stbQuery = new StringBuilder();
            stbQuery.AppendLine("UPDATE CAC_Codifica_Dati_SistemiEsterni SET");
            stbQuery.AppendLine(" Sistema_Cod = @sistemaCod,");
            stbQuery.AppendLine(" Codice_Esterno = @codiceEsterno,");
            stbQuery.AppendLine(" Descrizione_Esterno = @descrizioneEsterno,");
            stbQuery.AppendLine(" Tabella_Gias = @tabellaGias,");
            stbQuery.AppendLine(" Codice_Gias = @codiceGias,");
            stbQuery.AppendLine(" Data_Modifica = GETDATE(),");
            stbQuery.AppendLine(" Username_Modifica = @usernameModifica,");
            stbQuery.AppendLine(" Validita_Inizio = @validitaInizio,");
            stbQuery.AppendLine(" Validita_Fine = @validitaFine");
            stbQuery.AppendLine("WHERE ID = @id");

            try
            {
                var expandoObj = new ExpandoObject();
                expandoObj.TryAdd("@id", model.ID);
                expandoObj.TryAdd("@sistemaCod", model.Sistema_Cod);
                expandoObj.TryAdd("@codiceEsterno", model.Codice_Esterno);
                expandoObj.TryAdd("@descrizioneEsterno", model.Descrizione_Esterno);
                expandoObj.TryAdd("@tabellaGias", model.Tabella_Gias);
                expandoObj.TryAdd("@codiceGias", model.Codice_Gias);
                expandoObj.TryAdd("@usernameModifica", displayName);
                expandoObj.TryAdd("@validitaInizio", model.Validita_Inizio);
                expandoObj.TryAdd("@validitaFine", model.Validita_Fine);

                return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                throw new Exception(_localizer["ErrorUpdateData"], ex);
            }
        }

        public async Task<bool> DeleteCodificaCACAsync(CodificaCACModel objCAC, AgronicaCoreParametriServer objParametriServer)
        {
            if (objCAC.ID <= 0)
                throw new Exception(_localizer["InvalidID"]);

            var stbQuery = new StringBuilder();
            var expandoObj = new ExpandoObject();
            expandoObj.TryAdd("@id", objCAC.ID);
            stbQuery.AppendLine("DELETE FROM CAC_Codifica_Dati_SistemiEsterni");
            stbQuery.AppendLine("WHERE ID = @id");

            try
            {
                return await GetDataProvider(objParametriServer).Execute_WriteAsync(stbQuery.ToString(), expandoObj);
            }
            catch (Exception ex)
            {
                throw new Exception(_localizer["ErrorDeleteData"], ex);
            }
        }


        #region Utility
        private async Task<string> GetUserFromCF(AgronicaCoreParametriServer objParametriServer)
        {
            var codiceFiscale = objParametriServer.UsernameOperazione;

            string userLookupQuery = "SELECT [USER] FROM Utenti WHERE CODICE_FISCALE = @codiceFiscale";

            var param = new Dictionary<string, object>();
            param.TryAdd("@codiceFiscale", codiceFiscale);

            string displayName = codiceFiscale;

            var result = await GetDataProvider(objParametriServer).ExecuteReadAsync(userLookupQuery, param);

            if (result != null && result.Rows.Count > 0 && result.Columns.Contains("USER"))
            {
                displayName = result.Rows[0]["USER"]?.ToString() ?? codiceFiscale;
            }

            return displayName;
        }
        private DataTable ConvertToDataTable<T>(List<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);

            // Get all the properties
            var props = typeof(T).GetProperties();

            foreach (var prop in props)
            {
                Type propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                dataTable.Columns.Add(prop.Name, propType);
            }

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
        private string ConvertSistemaCodToEnum(string sistemaCOD)
        {
            if (int.TryParse(sistemaCOD, out int codiceInt))
            {
                if (Enum.IsDefined(typeof(Enum_CACSistemaCodEnum), codiceInt))
                {
                    return Enum.GetName(typeof(Enum_CACSistemaCodEnum), codiceInt) ?? string.Empty;
                }
            }

            return string.Empty;
        }
        #endregion Utility
    }
}
