using System.Data;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.CodiciAnagrafe;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Fabbricati;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Indirizzi;
using AgronicaNetCore.Anagrafe.DAL.HelpersSTD;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Magazzini
{
    public class Magazzini_APPService : BaseServiceAppBIZ, IMagazzini_APPService
    {
        private readonly IFabbricati_APP _fabbricati;
        private readonly ICodiciAnagrafe _codiciAnagrafeDal;
        private readonly IIndirizzi _indirizzi;

        public Magazzini_APPService(
            IServiceProvider provider,
            IFabbricati_APP fabbricati,
            ICodiciAnagrafe codiciAnagrafe,
            IIndirizzi indirizzi,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _fabbricati = fabbricati;
            _codiciAnagrafeDal = codiciAnagrafe;
            _indirizzi = indirizzi;
        }

        public async Task<List<FabbricatoEntity>> LeggiMagazziniAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            var fabbricati = new List<FabbricatoEntity>();
            try
            {
                var dtFabbricati = await _fabbricati.ReadAsync(piva, objParametriServer);

                var fabbricatiObj = dtFabbricati.AsEnumerable();
                if (fabbricatiObj.Count() == 0)
                    return fabbricati;

                var dtCodici = await _codiciAnagrafeDal.LeggiCodiciFabbricatiAsync(
                    new List<(string, int, int)>(
                        (
                            fabbricatiObj
                                .Select(x =>
                                    (
                                        x.Field<string>("piva"),
                                        x.Field<int>("sa_cod"),
                                        x.Field<int>("Fabbricato_Cod")
                                    )
                                )
                                .Distinct()
                                .ToList()
                        )!
                    ),
                    new List<int>(),
                    objParametriServer,
                    escludiCodiciCliente: true
                );

                var codici = new Dictionary<(string, int, int), List<CodiciAnagrafeValori>>();
                if (dtCodici?.Rows != null)
                    foreach (DataRow row in dtCodici.Rows)
                    {
                        var key = (
                            row.Field<string>("PIVA"),
                            row.Field<int>("SA_COD"),
                            row.Field<int>("Fabbricato_Cod")
                        );
                        if (!codici.ContainsKey(key!))
                        {
                            codici[key!] = new List<CodiciAnagrafeValori>();
                        }

                        codici[key!].Add(CodiciAnagrafeMapper.MapSTDFromRow(row));
                    }

                fabbricati = (
                    from x in fabbricatiObj
                    select new FabbricatoEntity
                    {
                        partitaIva = x.Field<string>("piva") ?? string.Empty,
                        centroaziendaleCod = x.Field<int>("sa_cod"),
                        codice = x.Field<int?>("Fabbricato_Cod") ?? 0,
                        descrizione = x.Field<string>("Fabbricato_Des") ?? string.Empty,
                        tipoDestinazione = x.Field<int?>("Tipo_Fabbricato_Cod") ?? 0,
                        indirizzo = JsonConvert.SerializeObject(
                            x.Field<int?>("Indirizzo_Cod") != 0
                                ? new Indirizzo(x.Field<int>("Indirizzo_Cod"))
                                {
                                    via = x.Field<string>("ind_des") ?? string.Empty,
                                    cap = x.Field<string>("CAP") ?? string.Empty,
                                    frazione = x.Field<string>("frz_des") ?? string.Empty,
                                    istatComune = new Istat()
                                    {
                                        com = x.Field<string>("com_cod_istat") ?? string.Empty,
                                        prov = x.Field<string>("pro_cod_istat") ?? string.Empty,
                                        localita = x.Field<string>("LOCALITA") ?? string.Empty,
                                        comuni_prov =
                                            x.Field<string>("COMUNI_PROV") ?? string.Empty,
                                    },
                                    stato = new CodiciNazioniISO3166(x.Field<string>("stato")),
                                    note = x.Field<string>("note") ?? string.Empty,
                                }
                                : null
                        ),
                        usoDaTerzi = Convert.ToInt32(x.Field<int>("usoDaTerzi")) != 0,
                        codici = JsonConvert.SerializeObject(
                            codici!.GetValueOrDefault(
                                (
                                    x.Field<string>("PIVA"),
                                    x.Field<int>("SA_COD"),
                                    x.Field<int>("Fabbricato_Cod")
                                ),
                                new List<CodiciAnagrafeValori>()
                            )
                        ),
                    }
                ).ToList();

                return fabbricati;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }
        }
    }
}
