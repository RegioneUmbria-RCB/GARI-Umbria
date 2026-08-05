using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.CodiciAnagrafe;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Imprese;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Indirizzi;
using AgronicaNetCore.Anagrafe.DAL.HelpersSTD;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Imprese
{
    public class Imprese_APPService : BaseServiceAppBIZ, IImprese_APPService
    {
        private readonly IImprese_APP _impreseAPP;
        private readonly ICodiciAnagrafe _codiciAnagrafeDal;
        private readonly IIndirizzi _indirizziDal;

        public Imprese_APPService(
            IServiceProvider provider,
            IImprese_APP imprese_APP,
            ICodiciAnagrafe codiciAnagrafe,
            IIndirizzi indirizzi,
            IStringLocalizer<Messages> localizer
        )
            : base(provider, localizer)
        {
            _impreseAPP = imprese_APP;
            _codiciAnagrafeDal = codiciAnagrafe;
            _indirizziDal = indirizzi;
        }

        public async Task<ImpresaEntity?> LeggiImpresaAsync(
            string piva,
            AgronicaCoreParametriServer objParametriServer
        )
        {
            ImpresaEntity? impresa = null;
            try
            {
                var dt = await _impreseAPP.ReadAsync(piva, objParametriServer);

                if (dt.Rows.Count == 0)
                    return null;

                var impresaDr = dt.Rows[0];

                var dtCodici = await _codiciAnagrafeDal.LeggiCodiciImpresaAsync(
                    new List<string>() { piva },
                    new List<int>(),
                    objParametriServer,
                    escludiCodiciCliente: true
                );

                var codici = CodiciAnagrafeMapper.MapSTDFromDataTable(dtCodici);

                var dtIndirizzi = await _indirizziDal.LeggiIndirizziImpresaAsync(
                    new List<string>() { piva },
                    new List<int>(),
                    indirizzoCompleto: true,
                    objParametriServer
                );

                var indirizzi = IndirizzoMapper.MapSTDFromDataTable(dtIndirizzi, true);

                bool agenzia = codici.Any(c =>
                    c.codiceAnagrafe.codice
                        == (int)TipiEnumerativi.Enum_CodiciAnagrafe.CodiceAgenzia
                    && c.valore == "1"
                );
                string chiaveCUAA =
                    codici
                        .FirstOrDefault(c =>
                            c.codiceAnagrafe.codice
                            == (int)TipiEnumerativi.Enum_CodiciAnagrafe.Chiave_CUAA_Demetra
                        )
                        ?.valore
                    ?? "";
                string CUAA =
                    codici
                        .FirstOrDefault(c =>
                            c.codiceAnagrafe.codice
                            == (int)TipiEnumerativi.Enum_CodiciAnagrafe.CodiceCUAA
                        )
                        ?.valore
                    ?? "";

                impresa = new ImpresaEntity
                {
                    partitaIva = Convert.ToString(impresaDr["PIVA"]),
                    ragioneSociale = Convert.ToString(impresaDr["rag_soc"]),
                    CUAA = CUAA,
                    tipoImpresa =
                        impresaDr["TipoImpresaGerarchia"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(impresaDr["TipoImpresaGerarchia"]),
                    impresaPadre =
                        impresaDr["impresaPadre"] == DBNull.Value
                            ? null
                            : Convert.ToString(impresaDr["impresaPadre"]),
                    partitaIvaReale =
                        impresaDr["partitaIvaReale"] == DBNull.Value
                            ? null
                            : Convert.ToString(impresaDr["partitaIvaReale"]),
                    codici = JsonConvert.SerializeObject(codici),
                    indirizzi = JsonConvert.SerializeObject(indirizzi),
                    agenzia = agenzia,
                    chiaveCUAA = chiaveCUAA,
                };
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            return impresa;
        }
    }
}
