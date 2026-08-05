using System.Data;
using AgronicaCoreDTOStd.OutData.Gis.Specie;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.CentriAziendali;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.CodiciAnagrafe;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Indirizzi;
using AgronicaNetCore.Anagrafe.DAL.HelpersSTD;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.CentriAziendali
{
    public class CentriAziendali_APPService : BaseServiceAppBIZ, ICentriAziendali_APPService
    {
        private readonly ICentriAziendali_APP _centriAziendaliApp;
        private readonly ICodiciAnagrafe _codiciAnagrafeDal;
        private readonly IIndirizzi _indirizzi;

        public CentriAziendali_APPService(
            ICentriAziendali_APP centriAziendali_APP,
            ICodiciAnagrafe codiciAnagrafe,
            IIndirizzi indirizzi,
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _centriAziendaliApp = centriAziendali_APP;
            _codiciAnagrafeDal = codiciAnagrafe;
            _indirizzi = indirizzi;
        }

        public async Task<List<CentroAziendaleEntity>> LeggiCentriAziendaliAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            try
            {
                var dt = await _centriAziendaliApp.ReadAsync(piva, objParametriServer);
                var chiavi = new List<(string, int)>();
                var chiaviSet = new HashSet<string>();

                foreach (DataRow row in dt.Rows)
                {
                    var partitaIva = row.Field<string>("piva") ?? string.Empty;
                    var codice = row.Field<int?>("sa_cod") ?? 0;
                    if (string.IsNullOrEmpty(partitaIva) || codice == 0)
                    {
                        continue;
                    }

                    var key = $"{partitaIva}|{codice}";
                    if (chiaviSet.Add(key))
                    {
                        chiavi.Add((partitaIva, codice));
                    }
                }

                var codici = await LeggiCodiciPerCentroAsync(chiavi, objParametriServer);
                var indirizzi = await LeggiIndirizziPerCentroAsync(chiavi, objParametriServer);
                var result = new List<CentroAziendaleEntity>();
                // var resultKeys = new HashSet<string>();

                foreach (DataRow row in dt.Rows)
                {
                    var partitaIva = row.Field<string>("piva") ?? string.Empty;
                    var codice = row.Field<int?>("sa_cod") ?? 0;
                    var chiave = $"{partitaIva}|{codice}";

                    var item = new CentroAziendaleEntity
                    {
                        partitaIva = partitaIva,
                        codice = codice,
                        descrizione = row.Field<string>("sa_nome") ?? string.Empty,
                        latitude = row.Field<double?>("lat") ?? 0,
                        longitude = row.Field<double?>("long") ?? 0,
                        codici = JsonConvert.SerializeObject(
                            codici!.GetValueOrDefault(
                                (partitaIva, codice),
                                new List<CodiciAnagrafeValori>()
                            )
                        ),
                        indirizzi = JsonConvert.SerializeObject(
                            indirizzi!.GetValueOrDefault(
                                (partitaIva, codice),
                                new List<IndirizzoAssociato>()
                            )
                        ),
                    };

                    result.Add(item);
                }

                return result;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }

        private async Task<
            Dictionary<(string, int), List<CodiciAnagrafeValori>>
        > LeggiCodiciPerCentroAsync(
            List<(string, int)> chiavi,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            var codici = new Dictionary<(string, int), List<CodiciAnagrafeValori>>();

            if (chiavi.Count == 0)
            {
                return new Dictionary<(string, int), List<CodiciAnagrafeValori>>();
            }

            var dtCodici = await _codiciAnagrafeDal.LeggiCodiciCentroAsync(
                chiavi,
                new List<int>(),
                objParametriServer,
                escludiCodiciCliente: true
            );
            foreach (DataRow row in dtCodici.Rows)
            {
                var key = (row.Field<string>("PIVA"), row.Field<int>("SA_COD"));
                if (!codici.ContainsKey(key!))
                {
                    codici[key!] = new List<CodiciAnagrafeValori>();
                }

                codici[key!].Add(CodiciAnagrafeMapper.MapSTDFromRow(row));
            }

            return codici;
        }

        private async Task<
            Dictionary<(string, int), List<IndirizzoAssociato>>
        > LeggiIndirizziPerCentroAsync(
            List<(string, int)> chiavi,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            var indirizzi = new Dictionary<(string, int), List<IndirizzoAssociato>>();

            if (chiavi.Count == 0)
            {
                return new Dictionary<(string, int), List<IndirizzoAssociato>>();
            }

            var dtIndirizzi = await _indirizzi.LeggiIndirizziCentroAsync(
                chiavi,
                new List<int>(),
                indirizzoCompleto: true,
                objParametriServer
            );
            if (dtIndirizzi?.Rows != null)
                foreach (DataRow row in dtIndirizzi.Rows)
                {
                    var key = (row.Field<string>("PIVA"), row.Field<int>("SA_COD"));
                    if (!indirizzi.ContainsKey(key!))
                    {
                        indirizzi[key!] = new List<IndirizzoAssociato>();
                    }

                    indirizzi[key!].Add(IndirizzoMapper.MapSTDFromRow(row, true));
                }

            return indirizzi;
        }
    }
}
