using AgronicaCoreDTOStd.Identity;
using AgronicaCoreModelsSTD.attivita;
using AgronicaNetCore.APP.BIZ.Resources;
using AgronicaNetCore.APP.BIZ.Services.ImpostazioniApp;
using AgronicaNetCore.APP.DAL.DataLayer;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;

namespace AgronicaNetCore.APP.BIZ.Services.Carichi
{
    public class CarichiService : BaseServiceAppBIZ, ICarichiService
    {
        private readonly IImpostazioniAppService _impostazioniAppService;
        private readonly ICarichiPerAppDal _carichiPerAppDal;
        private readonly ChiamaCoreWS _coreWsClient;

        public CarichiService(IServiceProvider provider, IStringLocalizer<Messages> localizer, IImpostazioniAppService impostazioniAppService, ICarichiPerAppDal carichiPerAppDal, ChiamaCoreWS coreWsClient) : base(provider, localizer)
        {
            _impostazioniAppService = impostazioniAppService;
            _carichiPerAppDal = carichiPerAppDal;
            _coreWsClient = coreWsClient;
        }

        public async Task<(List<MovimentoDiMagazzino> movimenti, List<Acquisto> acquisti)> LeggiCarichiEAcquistiPerAppAsync(string piva, DateTime dataRiferimento, DateTime dataUltimaSincro, AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti)
        {
            var movimenti = await _carichiPerAppDal.LeggiCarichiPerAppAsync(piva, dataRiferimento, dataUltimaSincro, objParametriServer);

            var acquisti = await _carichiPerAppDal.LeggiAcquistiPerAppAsync(piva, dataRiferimento, dataUltimaSincro, objParametriServer);

            return (movimenti, acquisti);
        }

        public async Task<string> LeggiGiacenzeAsync(string piva, DateTime data, ObjParametri objParametri, AgronicaCoreParametriTriple tripleParams, string bearerToken, string coreWsUrl)
        {
            var url = coreWsUrl + "/Contab/Giacenze.asmx/Leggi_Giacenze";

            var corews_prodotti_giacenze = new CoreWS_Prodotti_Giacenze(objParametri.objP_super_server, objParametri.objP_server, objParametri.objP_utenti, piva, data.ToString("dd/MM/yyyy"));

            return await CallCoreWSAsync(tripleParams, bearerToken, url, corews_prodotti_giacenze);
        }

        public async Task<string> LeggiProdottiAsync(string piva, DateTime data, ObjParametri objParametri, AgronicaCoreParametriTriple tripleParams, string bearerToken, string coreWsUrl, EnumCategorieMagazzino categoriaMagazzino, bool metaschema, bool giacenza = true)
        {
            var url = coreWsUrl + "/Anagrafica/Prodotti.asmx/LeggiElencoCompletoProdotti_APP";

            var core_ws_prodotti = new CoreWS_Prodotti(objParametri.objP_super_server, objParametri.objP_server, objParametri.objP_utenti, piva, data.ToString("dd/MM/yyyy"), (int)categoriaMagazzino, metaschema ? "S" : "N", giacenza, false);

            return await CallCoreWSAsync(tripleParams, bearerToken, url, core_ws_prodotti);
        }


        private async Task<string> CallCoreWSAsync(AgronicaCoreParametriTriple tripleParams, string bearerToken, string url, object input)
        {
            try
            {
                var json = await _coreWsClient.ChiamaCoreWSAsync(url, input, tripleParams, bearerToken, false);

                if (string.IsNullOrEmpty(json))
                    throw new Exception("Risposta nulla da web service");

                JObject jObj = JObject.Parse(json);
                JToken d = jObj["d"];

                return d.ToString();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, tripleParams.ObjParametriServer, ex);
                throw;
            }
        }
    }

    public class CoreWS_Prodotti_Giacenze
    {
        public string piva { get; set; }
        public int tipo_aggregazione { get; set; }
        public bool soloCampiApp { get; set; }
        public int sa_cod { get; set; }
        public int tipo_fabbricato_cod { get; set; }
        public int fabbricato_cod { get; set; }
        public int elem_cod { get; set; }
        public int pro_cod { get; set; }
        public int mat_cod { get; set; }
        public string lotto { get; set; }
        public string Data_Movimento_Str { get; set; }
        public bool isFreshAndFood { get; set; }
        public bool flag_QtaNoZero {get; set; }
        public string objP_super_server { get; set; }
        public string objP_server {get; set; }
        public string objP_utenti {get; set; }
        public CoreWS_Prodotti_Giacenze(string objP_super_server, string objP_server, string objP_utenti, string piva, string data_movimento_str)
        {
            this.objP_super_server = objP_super_server;
            this.objP_server = objP_server;
            this.objP_utenti = objP_utenti;
            this.piva = piva;
            this.Data_Movimento_Str = data_movimento_str;
            this.tipo_aggregazione = 0;
            this.soloCampiApp = true;
            this.isFreshAndFood = false;
            this.flag_QtaNoZero = false;
        }
    }

    public class CoreWS_Prodotti
    {
        public string piva { get; set; }
        public int Elem_Cod { get; set; }
        public string Data_Movimento_Str { get; set; }
        public string metaschema { get; set; }
        public string soloInGiacenza { get; set; }
        public string Flag_QtaNoZero { get; set; }
        public string objP_super_server { get; set; }
        public string objP_server { get; set; }
        public string objP_utenti { get; set; }

        public CoreWS_Prodotti(string objP_super_server, string objP_server, string objP_utenti, string piva, string data_movimento_str, int Elem_Cod, string metaschema, bool soloInGiacenza, bool flagQtaNoZero)
        {
            this.objP_super_server = objP_super_server;
            this.objP_server = objP_server;
            this.objP_utenti = objP_utenti;
            this.piva = piva;
            this.Data_Movimento_Str = data_movimento_str;
            this.Elem_Cod = Elem_Cod;
            this.metaschema = metaschema;
            this.soloInGiacenza = soloInGiacenza.ToString();
            this.Flag_QtaNoZero = flagQtaNoZero.ToString();
        }
    }

    public class ProdottiEntity
    {
        public int Elem_Cod;
        public string NomeComune;
        public int Prodotto_Cod;
        public string Prodotto_Des;
        public double Prodotto_Giacenza;
        public double N;
        public double P2O5;
        public double K2O;
        public double Cu;
        public int Uso;
        public string Piva;
        public int Sa_Cod;
        public int Udm_Cod;
        public int Veg_Cod;
        public bool IsTrappolaFormulato;
    }
}
