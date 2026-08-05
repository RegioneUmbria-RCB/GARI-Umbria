using System.Data;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.ParcoMacchine;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.ParcoMacchine
{
    public class ParcoMacchine_APPService : BaseServiceAppBIZ, IParcoMacchine_APPService
    {
        private readonly IParcoMacchine_APP _parcoMacchineApp;
        public ParcoMacchine_APPService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _parcoMacchineApp = _serviceProvider.GetRequiredService<IParcoMacchine_APP>();
        }

        public async Task<List<ParcoMacchineEntity>> LeggiParcoMacchineAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            var dt = await ReadAsync(piva, objParametriServer);
            var result = new List<ParcoMacchineEntity>();
            // var keys = new HashSet<string>();

            foreach (DataRow row in dt.Rows)
            {
                var item = new ParcoMacchineEntity
                {
                    partitaIva = row.Field<string>("Piva") ?? string.Empty,
                    centroAziendaleCod = row.Field<int?>("Sa_Cod") ?? 0,
                    codice = row.Field<int?>("Mac_Cod") ?? 0,
                    descrizione = row.Field<string>("Mac_Des") ?? string.Empty,
                    modello = row.Field<string>("Modello") ?? string.Empty,
                    macchinaCod = row.Field<int?>("Mac_Cod") ?? 0,
                    validitaFrom = row.Field<DateTime?>("Validita_Inizio") ?? DateTime.MinValue,
                    validitaTo = row.Field<DateTime?>("Validita_Fine") ?? DateTime.MinValue,
                    tipoMacchinaCod = row.Field<string>("Class_Code") ?? string.Empty,
                    titoloPossessoCod = row.Field<int?>("TitoloPossesso") ?? 0,
                    finalitaCod = row.Field<int?>("Tipo") ?? 0,
                    proprietario = row.Field<string>("Denominazione_Proprietario") ?? string.Empty,
                    targa = row.Field<string>("Targa") ?? string.Empty,
                    numImmatricolazione = row.Field<string>("N_Immatricolazione") ?? string.Empty,
                    dataImmatricolazione =
                        row.Field<DateTime?>("Data_Immatricolazione") ?? DateTime.MinValue,
                    codiceAnagrafe = row.Field<string>("Codice") ?? string.Empty,
                    BTM_Serial = row.Field<string>("BTM_Serial") ?? string.Empty,
                    VIN = row.Field<string>("VIN") ?? string.Empty,
                    Img_Thumbnail = row.Field<string>("Img_Thumbnail") ?? string.Empty,
                    Img_Thumbnail_FileName =
                        row.Field<string>("Img_Thumbnail_FileName") ?? string.Empty,
                    Img_Thumbnail_Extension =
                        row.Field<string>("Img_Thumbnail_Extension") ?? string.Empty,
                    Img_Large = row.Field<string>("Img_Large") ?? string.Empty,
                    Img_Large_FileName = row.Field<string>("Img_Large_FileName") ?? string.Empty,
                    Img_Large_Extension = row.Field<string>("Img_Large_Extension") ?? string.Empty,
                    Distinta_Installazione =
                        row.Field<string>("Distinta_Installazione") ?? string.Empty,
                    Contratto_Installazione =
                        row.Field<string>("Contratto_Installazione") ?? string.Empty,
                    Tipologia_Installazione =
                        row.Field<string>("Tipologia_Installazione") ?? string.Empty,
                    Data_Inizio_Installazione = row.Field<DateTime?>("Data_Inizio_Installazione"),
                    Data_Fine_Installazione = row.Field<DateTime?>("Data_Fine_Installazione"),
                    Stato_Installazione = row.Field<string>("Stato_Installazione") ?? string.Empty,
                    Provincia_Istat_Installazione =
                        row.Field<string>("Provincia_Istat_Installazione") ?? string.Empty,
                    Comune_Istat_Installazione =
                        row.Field<string>("Comune_Istat_Installazione") ?? string.Empty,
                    Indirizzo_Installazione =
                        row.Field<string>("Indirizzo_Installazione") ?? string.Empty,
                    Latitudine_Installazione = (float?)row.Field<double?>("Latitudine_Installazione"),
                    Longitudine_Installazione = (float?)row.Field<double?>("Longitudine_Installazione"),
                    contattoCod = row.Field<string>("Cod_Contatto") ?? string.Empty,
                    visibileCtrlGestione = row.Field<int?>("Visibile_ctrl_gestione") ?? 0,
                };

                if (item.codice == 0)
                {
                    continue;
                }

                // var key = $"{item.partitaIva}|{item.centroAziendaleCod}|{item.codice}";
                // if (keys.Add(key))
                // {
                //     result.Add(item);
                // }
                result.Add(item);
            }

            return result;
        }

        public async Task<List<MacchinaEntity>> LeggiMacchineAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            var dt = await ReadAsync(piva, objParametriServer);
            var result = new List<MacchinaEntity>();
            // var keys = new HashSet<int>();

            foreach (DataRow row in dt.Rows)
            {
                var validitaFrom = row.Field<DateTime?>("Validita_Inizio") ?? DateTime.MinValue;
                var validitaTo = row.Field<DateTime?>("Validita_Fine") ?? DateTime.MinValue;
                if (validitaFrom > DateTime.Now.Date || validitaTo < DateTime.Now.Date)
                {
                    continue;
                }

                var item = new MacchinaEntity
                {
                    codice = row.Field<int?>("Mac_Cod") ?? 0,
                    descrizione = row.Field<string>("Mac_Des") ?? string.Empty,
                };

                if (item.codice == 0)
                {
                    continue;
                }

                // if (keys.Add(item.codice))
                // {
                //     result.Add(item);
                // }
                result.Add(item);
            }

            return result;
        }

        private async Task<DataTable> ReadAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            try
            {
                return await _parcoMacchineApp.ReadAsync(piva, objParametriServer);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
