using AgronicaCoreAPI.converters;
using AgronicaCoreDataProviderSTD;
using AgronicaCoreDTOStd.InData;
using AgronicaCoreDTOStd.InData.Agea;
using AgronicaCoreDTOStd.InData.Agenda;
using AgronicaCoreDTOStd.InData.AgronicaChatGPT;
using AgronicaCoreDTOStd.InData.AgronicaCoreUtentiBIZ;
using AgronicaCoreDTOStd.InData.AgronicaCoreUtility;
using AgronicaCoreDTOStd.InData.Anagrafica;
using AgronicaCoreDTOStd.InData.Analisi;
using AgronicaCoreDTOStd.InData.Audit;
using AgronicaCoreDTOStd.InData.AuthDispatcher;
using AgronicaCoreDTOStd.InData.Budget;
using AgronicaCoreDTOStd.InData.FiltroRicerca;
using AgronicaCoreDTOStd.InData.GiasApp;
using AgronicaCoreDTOStd.InData.Gis;
using AgronicaCoreDTOStd.InData.Gis.MUZ;
using AgronicaCoreDTOStd.InData.IoT;
using AgronicaCoreDTOStd.InData.IsAlive;
using AgronicaCoreDTOStd.InData.Menu;
using AgronicaCoreDTOStd.InData.Metaschema;
using AgronicaCoreDTOStd.InData.Notifiche;
using AgronicaCoreDTOStd.InData.Profilazione;
using AgronicaCoreDTOStd.InData.Provisioning;
using AgronicaCoreDTOStd.InData.Provisioning.Retail;
using AgronicaCoreDTOStd.InData.ReadDettaglioAziendale;
using AgronicaCoreDTOStd.InData.ReadRequisitiStabilimento;
using AgronicaCoreDTOStd.InData.SaveRequisitiStabilimento;
using AgronicaCoreDTOStd.InData.Utility;
using AgronicaCoreDTOStd.InData.Valutazioni;
using AgronicaCoreDTOStd.InData.Visite;
using AgronicaCoreDTOStd.InData.Widgets;
using AgronicaCoreDTOStd.SmartTractors_HubIoT;
using AgronicaCoreModelloSTD;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.Audit;
using AgronicaCoreModelsSTD.AuthDispatcher;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.documenti;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreModelsSTD.GiasAPP;
using AgronicaCoreModelsSTD.Gis;
using AgronicaCoreModelsSTD.Gis.MUZ;
using AgronicaCoreModelsSTD.Gis.PermessiLayer;
using AgronicaCoreModelsSTD.IsAlive;
using AgronicaCoreModelsSTD.Menu;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaCoreModelsSTD.meteo;
using AgronicaCoreModelsSTD.pianiDiCampionamento;
using AgronicaCoreModelsSTD.profilazione;
using AgronicaCoreModelsSTD.provisioning;
using AgronicaCoreModelsSTD.Utility;
using AgronicaCoreModelsSTD.valutazioni;
using AgronicaCoreModelsSTD.Widgets;
using AgronicaCoreModelsSTD.Zoo;
using AgronicaCoreVarieBizSTD;
using AgronicaCoreWebServiceSTD;
using InData;
using InData.Agea;
using InData.Agenda;
using InData.Anagrafica;
using InData.Analisi;
using InData.DatiPrevisionaliColture;
using InData.Metaschema;
using InData.Meteo;
using InData.Operazione;
using InData.Statistiche;
using InData.Utility;
using InData.WidgetManager;
using InData.Zoo;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AgronicaCoreDTOStd.OutData.Gis;
using InData.Gis;
using static AgronicaCoreDataProviderSTD.TipiEnumerativi;
using static AgronicaCoreDTOStd.InData.Shared.GridDto;
using Fabbricato = AgronicaCoreModelsSTD.anagrafiche.Fabbricato;
using GruppoRaccolta = AgronicaCoreModelsSTD.anagrafiche.GruppoRaccolta;
using AgronicaCoreModelsSTD.metaschema.avversita;
using AgronicaCoreModelsSTD.attivita.dettagli;

namespace AgronicaCoreAPI.Controllers
{
    public class CoreWSController
    {
        private readonly string _baseAddr;
        private readonly string _bearerToken;
        private readonly bool _compressione;
        private readonly string _clientTimeZoneInfo = string.Empty;
        private HttpClient _hc;
        protected ILog _log;
        private HttpRequest _request;
        private readonly bool _isRabbitMQ;

        private JObject Request(HttpMethod method, string request, string content)
        {
            JObject obj = null;

            //using (var hc = new HttpClient())
            //{

            if (!string.IsNullOrEmpty(_bearerToken))
            {
                _hc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
            }

            if (_compressione)
            {
                _hc.DefaultRequestHeaders.Add("x-compressione", "true");
            }

            if (!String.IsNullOrEmpty(_clientTimeZoneInfo))
            {
                _hc.DefaultRequestHeaders.Add("x-timezone", _clientTimeZoneInfo);
            }

            if (_isRabbitMQ)
            {
                _hc.DefaultRequestHeaders.Add("X-RabbitMQ", "true");
            }

            string authCookieValue = string.Empty;
            if (_request!=null && _request.Headers.TryGetValue("cookie", out var headerValues))
            {
                var authCookie = headerValues.FirstOrDefault(s => s.Contains("auth_cookie="));
                if (authCookie is not null)
                {
                    var authCookieContent = authCookie.Split(";").First(s => s.Contains("auth_cookie")); //per problemi a runtime (in modalità release)
                    authCookieContent = authCookieContent.Split("=")[1];
                    if (authCookieContent.Contains("chunks"))
                    {
                        var chunksNr = Convert.ToInt32(authCookieContent.Split("-")[1]);
                        chunksNr = Math.Max(chunksNr, 3); //limit the nr of chunks to avoid malicious attacks (SonarQube)
                        for (int i = 1; i <= chunksNr; i++)
                        {
                            var chunkAuthCookie = headerValues.FirstOrDefault(s => s.Contains($"auth_cookieC{i}"));

                            if (chunkAuthCookie is not null)
                            {
                                chunkAuthCookie = chunkAuthCookie.Split(";").First(s => s.Contains($"auth_cookieC{i}")).Split("=")[1];
                                authCookieValue = string.Format("{0}{1}", authCookieValue, chunkAuthCookie);
                            }
                        }
                    }
                    else
                    {
                        authCookieValue = authCookieContent;
                    }
                    if (_hc.DefaultRequestHeaders.Contains("auth_cookie"))
                    {
                        _hc.DefaultRequestHeaders.Remove("auth_cookie");
                    }
                    _hc.DefaultRequestHeaders.Add("auth_cookie", authCookieValue);
                }
            }

            Task<HttpResponseMessage> trm = null;
            HttpResponseMessage rm = null;
            try
            {

                if (method == HttpMethod.Post)
                {
                    HttpContent cont = null;
                    if (!string.IsNullOrEmpty(content))
                    {
                        cont = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
                    }

                    trm = _hc.PostAsync(_baseAddr + request, cont);
                }
                else
                {
                    if (method == HttpMethod.Get)
                    {
                        trm = _hc.GetAsync(_baseAddr + request);
                    }
                }

                if (trm != null)
                {
                    //var timer = new Stopwatch();
                    // timer.Start();
                    rm = trm.Result;
                    // timer.Stop();
                    // Console.WriteLine(request + ": " + timer.Elapsed.ToString(@"m\:ss\.fff"));

                    if (rm.IsSuccessStatusCode)
                    {
                        Task<String> ts = rm.Content.ReadAsStringAsync();
                        obj = JObject.Parse(ts.Result);
                        if (obj.ContainsKey("d"))
                        {
                            obj = (JObject)obj.GetValue("d");
                        }
                    }
                    else if (rm.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        throw new CoreWSNotAuthenticatedException("CoreWS responded with status 401");
                    }
                    else
                    {
                        CoreAPIException ex = new CoreAPIException();
                        ex.request = request;
                        ex.requestContent = content;
                        ex.StatusCode = (int)rm.StatusCode;
                        ex.ReasonPhrase = rm.ReasonPhrase;
                        String contentError = "";
                        contentError = rm.Content?.ReadAsStringAsync().Result;
                        ex.contentError = contentError;
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                // handle exception
                // string errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex);
                // LogManager.GetLogger("CoreAPI").Error(errore);
                string requestStr = "";
                try
                {
                    requestStr = JsonConvert.SerializeObject(request);
                }
                catch (Exception ex1)
                {

                }

                string responseStr = "";
                if (rm != null)
                {
                    try
                    {
                        responseStr = JsonConvert.SerializeObject(rm);
                    }
                    catch (Exception ex1)
                    {

                    }
                }

                string errore = "ERRORE: " + ex.Message + " Request:" + requestStr + " Response:" + responseStr;

                JsonConvert.SerializeObject(rm);
                _log.Error(errore, ex);
                obj = null;
                throw ex;
            }
            finally
            {
                rm?.Dispose();
                trm?.Dispose();
            }
            //}

            return obj;
        }

        public CoreWSController(HttpClient hc, string baseAddr, string bearerToken, HttpRequest request, bool isRabbitMQ = false)
        {
            _log = LogManager.GetLogger("CoreAPI");
            _hc = hc;
            _baseAddr = baseAddr;
            if (!_baseAddr.EndsWith("/"))
            {
                _baseAddr += "/";
            }
            _bearerToken = bearerToken;
            _request = request;
            _isRabbitMQ = isRabbitMQ;

            // Recupero i custom headers, se presenti nella request
            // (non disponibili quando invocato dal consumer RabbitMQ)

            StringValues headerValues;
            _compressione = false;
            _clientTimeZoneInfo = null;

            if (!isRabbitMQ && request != null)
            {
                if (request.Headers.TryGetValue("x-compressione", out headerValues))
                {
                    if (headerValues.FirstOrDefault() == "true")
                    {
                        _compressione = true;
                    }
                }

                if (request.Headers.TryGetValue("x-timezone", out headerValues))
                {
                    if (headerValues.Any() && !String.IsNullOrEmpty(headerValues.First()))
                    {
                        _clientTimeZoneInfo = headerValues.First();
                    }
                }
            }
        }

        protected void logInfo(string info) { _log.Info(info); }

        protected void logErrore(string errore) { _log.Error(errore); }

        public class CoreWSUtilizziTerreno : APICallsBasic
        {
            public string LetteraIniziale;
            public string StringaCerca;
            public string FiltroAggiuntivo;
            public string Ordinamento;

            public CoreWSUtilizziTerreno(string objP_super_server, string objP_server, string objP_utenti) : base(objP_super_server, objP_server, objP_utenti)
            {
                LetteraIniziale = "";
                StringaCerca = "";
                FiltroAggiuntivo = "";
                Ordinamento = "";
            }

        }

        public class CoreWSSpecie : CoreWSUtilizziTerreno
        {

            public int Gru_Cod;

            public CoreWSSpecie(string objP_super_server, string objP_server, string objP_utenti, int gruppo) : base(objP_super_server, objP_server, objP_utenti)
            {
                Gru_Cod = gruppo;
            }

        }

        public class CoreWSVarieta : CoreWSUtilizziTerreno
        {

            public int Veg_Cod;

            public CoreWSVarieta(string objP_super_server, string objP_server, string objP_utenti, int specie) : base(objP_super_server, objP_server, objP_utenti)
            {
                Veg_Cod = specie;
            }

        }

        public class CoreWSDisciplinari : APICallsBasic
        {

            public string veg_cod;
            public string data;
            public bool flag_disciplinareprivato;
            public int reg_cod;


            public CoreWSDisciplinari(string objP_super_server, string objP_server, string objP_utenti, string specie, bool flag) : base(objP_super_server, objP_server, objP_utenti)
            {
                veg_cod = specie;
                data = "";
                flag_disciplinareprivato = flag;
                reg_cod = 0;
            }

        }

        public class CoreWSLeggiMagazzini : APICallsBasic
        {

            public string piva;
            public bool filtroMagazziniAPP;

            public CoreWSLeggiMagazzini(string objP_super_server, string objP_server, string objP_utenti, string piva, bool filtro) : base(objP_super_server, objP_server, objP_utenti)
            {
                this.piva = piva;
                this.filtroMagazziniAPP = filtro;
            }

        }

        public class CoreWSScriviMagazzini : APICallsBasic
        {

            public List<Fabbricato> magazzini;

            public CoreWSScriviMagazzini(string objP_super_server, string objP_server, string objP_utenti, List<Fabbricato> magazzini) : base(objP_super_server, objP_server, objP_utenti)
            {
                this.magazzini = magazzini;
            }

        }

        public class CoreWSLeggiAppezzamenti : APICallsBasic
        {
            public string piva;
            public int sa_cod;
            public int appezza;

            public CoreWSLeggiAppezzamenti(string objP_super_server, string objP_server, string objP_utenti, string piva, int sa_cod, int appezza) : base(objP_super_server, objP_server, objP_utenti)
            {
                this.piva = piva;
                this.sa_cod = sa_cod;
                this.appezza = appezza;
            }

        }

        public class CoreWSScriviAppezzamenti : APICallsBasic
        {
            public List<Appezzamento> appezzamenti;

            public CoreWSScriviAppezzamenti(string objP_super_server, string objP_server, string objP_utenti, List<Appezzamento> appezzamenti) : base(objP_super_server, objP_server, objP_utenti)
            {
                this.appezzamenti = appezzamenti;
            }
        }

        public RispostaStandard LeggiRilieviProduzione(CoreWSRequest<CoreWS_Generic<LeggiRilieviProduzione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/StatisticheApp.asmx/LeggiRilieviProduzione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiStimeProduzione(CoreWSRequest<CoreWS_Generic<LeggiStimeProduzione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/StatisticheApp.asmx/LeggiStimeProduzione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiRicette(CoreWS_RicettePerScarico request)
        {
            JObject result = Request(HttpMethod.Post, "/Contab/Ricette.asmx/RicettaLeggi_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ScriviRicette(CoreWS_RicettaScrivi_APP request)
        {
            JObject result = Request(HttpMethod.Post, "/Contab/Ricette.asmx/RicettaScrivi_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.attivita.Attivita>> LeggiVisite(CoreWS_LeggiVisite request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/LeggiVisite", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.attivita.Attivita>>>(result.ToString());
        }

        public RispostaStandard ScriviVisite(CoreWS_VisiteScrivi_APP request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreVisite/Visite.asmx/VisiteScrivi_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ScriviDocumento(CoreWS_DocumentiScrivi_APP request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreScadenziario/Alert.asmx/DocumentiScrivi_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ScriviCoordinate(CoreWS_CoordinateScrivi_APP request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/EntrateUscite.asmx/Coordinate_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ScriviEntrateUscite(CoreWS_LogEventiScrivi_APP request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/EntrateUscite.asmx/EntrateUscite_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiOperazioni(CoreWS_Operazioni request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Operazioni.asmx/Leggi_Operazioni_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiOperazioniCombinazioni(CoreWS_Operazioni request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Operazioni.asmx/Leggi_Operazioni_Combinazioni_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiAvversita(CoreWS_Avversita request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Avversita.asmx/Leggi_Avversita_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiAvversitaSpecie(CoreWS_Avversita_Specie request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/SpecieVegetalixAvversita.asmx/Leggi_SpecieVegetalixAvversita_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiGruppoAvversitaAttive(CoreWS_GruppoAvversitaAttive request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/GruppoAvversita.asmx/Leggi_GruppoAvversitaAttive_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiErbeInfestantiAttive(CoreWS_Infestanti_Attive request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Avversita.asmx/Leggi_InfestantiAttive_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiInfestantiAttive(CoreWS_Avversita_Specie request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/SpecieVegetalixAvversita.asmx/Leggi_SpecieVegetalixAvversita_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiGruppoAvversita(CoreWS_GruppoAvversita request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/GruppoAvversita.asmx/Leggi_GruppoAvversita_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiCategorieUnitaMisura(CoreWS_CategorieXUnitaMisura request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/CategorieXUnitaMisura.asmx/Leggi_CategorieXUnitaMisura", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<UnitaDiMisura>> LeggiUnitaDiMisura(CoreWSRequest<CoreWS_Generic<LeggiUnitaDiMisura>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/UnitaDiMisura.asmx/Leggi_UnitaMisura", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<UnitaDiMisura>>>(result.ToString());
        }

        public RispostaStandard LeggiEpoche(CoreWS_Epoche request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Epoche.asmx/Leggi_Epoche_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard readRequisitiStabilimento(CoreWSRequest<CoreWS_Generic<ReadRequisitiStabilimento>> request)
        {
            JObject result = Request(HttpMethod.Post, "/RequisitiStabilimento/RequisitiStabilimento.asmx/readRequisitiStabilimento", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard saveRequisitiStabilimento(CoreWSRequest<CoreWS_Generic<SaveRequisitiStabilimento>> request)
        {
            JObject result = Request(HttpMethod.Post, "/RequisitiStabilimento/RequisitiStabilimento.asmx/saveRequisitiStabilimento", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard readPianoColturaleRequisitiStabilimento(CoreWSRequest<CoreWS_Generic<ReadRequisitiStabilimento>> request)
        {
            JObject result = Request(HttpMethod.Post, "/RequisitiStabilimento/RequisitiStabilimento.asmx/readPianoColturale", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard readContracts(CoreWSRequest<CoreWS_Generic<ReadRequisitiStabilimento>> request)
        {
            JObject result = Request(HttpMethod.Post, "/RequisitiStabilimento/RequisitiStabilimento.asmx/readContracts", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard readSurfaceForProducts(CoreWSRequest<CoreWS_Generic<ReadRequisitiStabilimento>> request)
        {
            JObject result = Request(HttpMethod.Post, "/RequisitiStabilimento/RequisitiStabilimento.asmx/readSurfaceForProducts", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard readDettaglioAziendale(CoreWSRequest<CoreWS_Generic<ReadDettaglioAziendale>> request)
        {
            JObject result = Request(HttpMethod.Post, "/RequisitiStabilimento/RequisitiStabilimento.asmx/readDettaglioAziendale", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard Leggi_ParticelleCampo_NG(CoreWSRequest<CoreWS_Generic<BudgetAnagrafica<Parametri_ObjParametriAgenda_NG_GestioneRichieste>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Budget/Campi.asmx/Leggi_ParticelleCampo_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ScriviCampiAnagrafica_NG(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Budget.ScriviCampiAnagrafica>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Budget/Campi.asmx/ScriviCampiAnagrafica_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CaricaDatiCatastali_NG(CoreWSRequest<CoreWS_Generic<BudgetAnagrafica<CaricaDatiCatastali>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Budget/Reg_Impianto.asmx/CaricaDatiCatastali_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard Leggi_Esercizi_Anagrafica_Bdg(CoreWSRequest<CoreWS_Generic<BudgetAnagrafica<Parametri_ObjParametriAgenda_NG>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Budget/Reg_Impianto.asmx/Leggi_Esercizi_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento> Leggi_Appezzamento_Anagrafica(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Budget/Appezzamento.asmx/Leggi_Appezzamento_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento>>(result.ToString());
        }
        public RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento> Scrivi_Appezzamento_Anagrafica(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Budget/Appezzamento.asmx/Scrivi_Appezzamento_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento>>(result.ToString());
        }
        public RispostaStandard<AgronicaCoreDTOStd.InData.Data> Leggi_Max_DataModifica(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Budget/Appezzamento.asmx/Leggi_Max_DataModifica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreDTOStd.InData.Data>>(result.ToString());
        }
        public RispostaStandard<AgronicaCoreDTOStd.InData.Budget.Budget_Testata> Leggi_Budget_Attivo(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Budget/Budget.asmx/Leggi_Budget_Attivo", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreDTOStd.InData.Budget.Budget_Testata>>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreDTOStd.InData.Budget.Budget_Testata>> LeggiElencoBudgetTestata(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Budget/Budget.asmx/LeggiElencoBudgetTestata", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreDTOStd.InData.Budget.Budget_Testata>>>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Campo>> LeggiCampiBudget(CoreWSRequest<CoreWS_Generic<BudgetAnagrafica<AgronicaCoreDTOStd.InData.Metaschema.LeggiCampi>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Budget/Campi.asmx/LeggiCampi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Campo>>>(result.ToString());
        }
        public RispostaStandard Leggi_Campi_Anagrafica(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Budget/Campi.asmx/Leggi_Campi_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Campo> Leggi_Campo_Anagrafica(CoreWSRequest<CoreWS_Generic<BudgetAnagrafica<InData.Parametri_ObjParametriAgenda_NG>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Budget/Campi.asmx/Leggi_Campo_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Campo>>(result.ToString());
        }
        public RispostaStandard Leggi_AppezzamentiCampo_NG(CoreWSRequest<CoreWS_Generic<BudgetAnagrafica<InData.Parametri_ObjParametriAgenda_NG>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Budget/Campi.asmx/Leggi_AppezzamentiCampo_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<String> LeggiInvestimentoCatastaleBdg(CoreWSRequest<CoreWS_Generic<BudgetAnagrafica<AgronicaCoreDTOStd.InData.Metaschema.LeggiInvestimentoCatastale>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Budget/Catasto.asmx/Leggi_Investimento_Catastale", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<String>>(result.ToString());
        }

        public RispostaStandard<string> LeggiInvestimentoCatastaleCampoBdg(CoreWSRequest<CoreWS_Generic<BudgetAnagrafica<AgronicaCoreDTOStd.InData.Metaschema.LeggiInvestimentoCatastaleCampo>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Budget/Catasto.asmx/Leggi_Investimento_Catastale_Campo", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }
        public RispostaStandard Ribalta_BudgetReale(CoreWSRequest<CoreWS_Generic<List<BudgetAnagrafica<Appezzamento>>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Budget/Appezzamento.asmx/Ribalta_BudgetReale", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.DettaglioVarietaPersonalizzato>> LeggiDettaglioVarietaPersonalizzato(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Codifiche/CAC_Codifica_InfoAggiuntive.asmx/LeggiDettaglioVarietaPersonalizzato", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.DettaglioVarietaPersonalizzato>>>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>> LeggiCapitolatoPrivato(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Codifiche/CAC_Codifica_InfoAggiuntive.asmx/LeggiCapitolatoPrivato", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>>>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>> LeggiResiduiDisponibili(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Codifiche/CAC_Codifica_InfoAggiuntive.asmx/LeggiResiduiDisponibili", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>>>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>> LeggiCertProdDisponibili(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Codifiche/CAC_Codifica_InfoAggiuntive.asmx/LeggiCertProdDisponibili", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>>>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>> LeggiPianiSemina(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Codifiche/CAC_Codifica_InfoAggiuntive.asmx/LeggiPianiSemina", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>>>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>> LeggiCAC_Codifica_InfoAggiuntive(CoreWSRequest<CoreWS_Generic<LeggiCAC_Codifica_InfoAggiuntive>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Codifiche/CAC_Codifica_InfoAggiuntive.asmx/LeggiCAC_Codifica_InfoAggiuntive_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.attivita.AttivitaPersonalizzata>> Leggi_AttivitaPersonalizzata_Modello(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Contab/Attivita.asmx/Leggi_AttivitaPersonalizzata_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.attivita.AttivitaPersonalizzata>>>(result.ToString());
        }
        public RispostaStandard ImportaAgendaAziendaDaTabelleAPP_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "Contab/Ricette.asmx/ImportaAgendaAziendaDaTabelleAPP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ImportaAgendaDaTabelleAPP(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "Contab/Ricette.asmx/ImportaAgendaDaTabelleAPP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ImportaRicetteAziendaDaTabelleAPP_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "Contab/Ricette.asmx/ImportaRicetteAziendaDaTabelleAPP_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ImportaRicetteDaTabelleAPP(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "Contab/Ricette.asmx/ImportaRicetteDaTabelleAPP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiAttivita(CoreWS_Attivita request)
        {
            JObject result = Request(HttpMethod.Post, "/Contab/Attivita.asmx/Leggi_Attivita_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiAttivitaOperazioni(CoreWS_AttivitaXOperazioni request)
        {
            JObject result = Request(HttpMethod.Post, "/Contab/AttivitaXOperazioni.asmx/Leggi_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiAttivitaCentriAziendali(CoreWS_AttivitaXCentri_Aziendali request)
        {
            JObject result = Request(HttpMethod.Post, "/Contab/AttivitaXCentri_Aziendali.asmx/Leggi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiProgetti(CoreWS_Imputazione_Fasi request)
        {
            JObject result = Request(HttpMethod.Post, "/Contab/Imputazioni_Fasi.asmx/Leggi_Imputazioni_Fasi_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiTipologie(CoreWS_Tipologie request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreScadenziario/Alert_Tipologia.asmx/LeggiTipologie_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaComboStampePreferite(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpostazioniUtente.asmx/CaricaComboStampePreferite", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard getStampePreferite(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpostazioniUtente.asmx/LeggiStampePreferite", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiPermessiAPP(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_Permessi_R.asmx/Get_Permessi_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiImpostazioni(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/Get_Impostazioni_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<BaseCodeDescr>> LeggiValoriParametriQualitativi_Modello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Anagrafica.FiltroValoriParametriQualitativi>> request)
        {
            JObject result = Request(HttpMethod.Post, "/FreshAndFood/FreshAndFood.asmx/LeggiValoriParametriQualitativi_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<BaseCodeDescr>>>(result.ToString());
        }
        public RispostaStandard CaricaDatiApp_NG(CoreWSRequest<CoreWS_Generic<CaricaDatiApp>> request)
        {
            JObject result = Request(HttpMethod.Post, "GiasApp/SincroDatiApp.asmx/CaricaDatiApp_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiImpreseImpostazioni(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Imprese_Impostazioni_R.asmx/Get_Impostazioni_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiDatiUtente(CoreWS_Operatore request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/LeggiDatiUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ListaUtenti(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/ListaUtenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CreateRandomCF(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/CreateRandomCF", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard GetUsersCount(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/GetUsersCount", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<ListaUtenti> ListaUtentiDatiBase(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/ListaUtentiDatiBase", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ListaUtenti>>(result.ToString());
        }
        public RispostaStandard<ListaGruppiUtente> ListaGruppiUtente(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/ListaGruppiUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ListaGruppiUtente>>(result.ToString());
        }
        public RispostaStandard LeggiImprese(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/LeggiImpreseConFiltroUtente_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiImpreseAPP(CoreWSRequest<CoreWS_Generic<FiltroAziendeAPP>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/LeggiImprese_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ContaImpreseVisibiliAPP(CoreWSRequest<CoreWS_Generic<FiltroAziendeAPP>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/ContaImpreseVisibili_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiImpreseAPP_GIS(CoreWSRequest<CoreWS_Generic<FiltroAziendeMappaAPP>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/LeggiImprese_APP_GIS", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiTipologieGerarchiaImprese(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/LeggiTipologieGerarchiaImprese_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiImpreseNG(CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Leggi_Imprese_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<AgronicaCoreDTOStd.InData.Data> LeggiImpreseMaxDataModifica(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Leggi_Max_DataModifica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreDTOStd.InData.Data>>(result.ToString());
        }
        public RispostaStandard<Impresa> LeggiImpresaNG(CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Leggi_Impresa_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Impresa>>(result.ToString());
        }
        public RispostaStandard<Impresa> ScriviImpresaNG(CoreWSRequest<CoreWS_Generic<Impresa>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Scrivi_Impresa_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Impresa>>(result.ToString());
        }
        public RispostaStandard LeggiCentriAziendali(CoreWS_Centri_Aziendali request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/CentriAziendali.asmx/getCentriAziendali_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiCentriNG(CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/CentroAziendale.asmx/Leggi_Centri_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<IndirizzoAssociato>> readCentreAddresses(CoreWSRequest<CoreWS_Generic<LeggiIndirizziCentro>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/CentroAziendale.asmx/readCentreAddresses", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<IndirizzoAssociato>>>(result.ToString());
        }
        public RispostaStandard<CentroAziendale> LeggiCentroNG(CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/CentroAziendale.asmx/Leggi_Centro_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<CentroAziendale>>(result.ToString());
        }
        public RispostaStandard<CentroAziendale> ScriviCentroNG(CoreWSRequest<CoreWS_Generic<CentroAziendale>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/CentroAziendale.asmx/Scrivi_Centro_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<CentroAziendale>>(result.ToString());
        }
        public RispostaStandard LeggiImpianti(CoreWS_Impianti request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Impianti.asmx/getListaImpianti_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiContatti(CoreWS_Contatti request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/Leggi_Contatti_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard EliminaContatto(CoreWSRequest<CoreWS_Generic<Contatto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/EliminaContatto", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiFornitori(CoreWS_Contatti request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/Leggi_Fornitori_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ContattoAssociaUtente(CoreWSRequest<CoreWS_Generic<AssociaUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/AssociaUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiUtentiDaAssociare(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/LeggiUtentiDaAssociare", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiUtenteAssociato(CoreWSRequest<CoreWS_Generic<AssociaUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/LeggiUtenteAssociato", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiMacchine(CoreWS_Parco_Macchine request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/Leggi_Macchine_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ScriviMacchina(CoreWS_Scrivi_Macchina request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/Scrivi_Macchina_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiTipiMacchine(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Macchine.asmx/Leggi_TutteMacchine", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiMagazzini(CoreWS_Magazzini request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Fabbricati.asmx/LeggiMagazzini_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiProdotti(string objP_super_server, string objP_server, string objP_utenti, string piva, string data, enum_CategorieMagazzino categoria, bool metaschema, bool giacenza = true)
        {
            return LeggiProdotti(new CoreWS_Prodotti(objP_super_server, objP_server, objP_utenti, piva, data, (int)categoria, metaschema ? "S" : "N", giacenza, false));
        }
        public RispostaStandard LeggiCodificaProdotti(CoreWS_Codifica_Prodotti request)
        {
            JObject result = Request(HttpMethod.Post, "/Codifiche/CAC_Codifica_ProdottiAziendali.asmx/LeggiCodificaProdotti", JsonConvert.SerializeObject(request));
            return result != null ? JsonConvert.DeserializeObject<RispostaStandard>(result.ToString()) : null;
        }
        public RispostaStandard LeggiProdotti(CoreWS_Prodotti request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/LeggiElencoCompletoProdotti_APP", JsonConvert.SerializeObject(request));
            return result != null ? JsonConvert.DeserializeObject<RispostaStandard>(result.ToString()) : null;
        }
        public RispostaStandard LeggiProdottiGias(CoreWS_Prodotti_GIAS request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/RicercaProdottiSenzaGiacenza_APP", JsonConvert.SerializeObject(request));
            return result != null ? JsonConvert.DeserializeObject<RispostaStandard>(result.ToString()) : null;
        }
        public RispostaStandard LeggiProdottiSenzaGiacenza(CoreWS_ProdottiSenzaGiacenza request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/RicercaProdottiSenzaGiacenza", JsonConvert.SerializeObject(request));
            return result != null ? JsonConvert.DeserializeObject<RispostaStandard>(result.ToString()) : null;
        }
        public RispostaStandard LeggiProdottiGiacenze(CoreWS_Prodotti_Giacenze request)
        {
            JObject result = Request(HttpMethod.Post, "/Contab/Giacenze.asmx/Leggi_Giacenze", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiMovimentiMagazzini(CoreWS_Movimenti_Magazzini request)
        {
            JObject result = Request(HttpMethod.Post, "/Contab/Giacenze.asmx/Leggi_Movimenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ScriviMovimentiMagazzini(CoreWSRequest<CoreWS_Generic<List<MovimentoDiMagazzino>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Contab/Giacenze.asmx/Scrivi_Movimenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ScriviAttivita(CoreWSRequest<CoreWS_Generic<RicettePerScarico>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/ScriviAttivita", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ScriviVisite(CoreWSRequest<CoreWS_Generic<VisitePerScarico>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/ScriviVisite", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ScriviDocumenti(CoreWSRequest<CoreWS_Generic<DocumentoPerScarico>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/ScriviDocumenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ScriviDocumentiImport(CoreWSRequest<CoreWS_Generic<DocumentoPerImport>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/ScriviDocumentiImport", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CheckHasAttachedDocs(CoreWSRequest<CoreWS_Generic<AttachmentCheckParams>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Contab/RicercaDocumenti.asmx/HasAttachment", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<Impresa> ScriviImpresa(CoreWSRequest<CoreWS_Generic<Impresa>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/ScriviImpresa", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Impresa>>(result.ToString());
        }
        public RispostaStandard<CentroAziendale> ScriviCentro(CoreWSRequest<CoreWS_Generic<CentroAziendale>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/ScriviCentro", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<CentroAziendale>>(result.ToString(), new JsonSerializerSettings { Converters = { new StringEnumConverter() } });
        }
        public RispostaStandard<SquadraAttivita> ScriviSquadra(CoreWSRequest<CoreWS_Generic<SquadraAttivita>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/ScriviSquadra", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<SquadraAttivita>>(result.ToString(), new JsonSerializerSettings { Converters = { new StringEnumConverter() } });
        }
        public RispostaStandard<RisorseUmane> ScriviContatto(CoreWSRequest<CoreWS_Generic<RisorseUmane>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/ScriviContatto", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<RisorseUmane>>(result.ToString(), new JsonSerializerSettings { Converters = { new StringEnumConverter() } });
        }
        public RispostaStandard<Appezzamento> ScriviAppezzamento(CoreWSRequest<CoreWS_Generic<Appezzamento>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/ScriviAppezzamento", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Appezzamento>>(result.ToString(), new JsonSerializerSettings { Converters = { new StringEnumConverter() } });
        }
        public RispostaStandard ScriviMagazzini(CoreWSRequest<CoreWS_Generic<List<Fabbricato>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/ScriviMagazzini", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ScriviRilievo(CoreWSRequest<CoreWS_Generic<AgronicaCoreModelsSTD.attivita.Attivita>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/ScriviRilievo", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ScriviMovimenti(CoreWSRequest<CoreWS_Generic<List<MovimentoDiMagazzino>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/ScriviMovimenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ScriviAcquisto(CoreWSRequest<CoreWS_Generic<Acquisto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/ScriviAcquisto", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ScriviManutenzioni(CoreWSRequest<CoreWS_Generic<List<Manutenzione>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/ScriviManutenzioni", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiConfigurazioneSiti(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/LeggiConfigurazioneSiti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard warmUpEFGiasNG(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/warmUpEFGiasNG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<warmUpGiasNG_ResponseDto> warmUpGiasNG_NG(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/warmUpGiasNG_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<warmUpGiasNG_ResponseDto>>(result.ToString());
        }
        public RispostaStandard PassaggioSitoAgenda_NG(CoreWSRequest<CoreWS_Generic<PassaggioSitoAgenda>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoAgenda_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PassaggioSitoSincronizzatore_NG(CoreWSRequest<CoreWS_Generic<InDataUtility>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoSincronizzatore_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PassaggioSitoAnalisi_NG(CoreWSRequest<CoreWS_Generic<InDataUtility>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoAnalisi_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PassaggioSitoPua_NG(CoreWSRequest<CoreWS_Generic<InDataUtility>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoPua_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PassaggioSitoPlanning_NG(CoreWSRequest<CoreWS_Generic<InDataUtility>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoPlanning_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PassaggioSitoGiasOnline_NG(CoreWSRequest<CoreWS_Generic<InDataUtility>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoGiasOnline_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PassaggioSitoProfilazione_NG(CoreWSRequest<CoreWS_Generic<InDataUtility>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoProfilazione_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PassaggioSitoAudit_NG(CoreWSRequest<CoreWS_Generic<InDataUtility>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoAudit_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PassaggioSitoAgronicaCheckCOOP_NG(CoreWSRequest<CoreWS_Generic<InDataUtility>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoAgronicaCheckCOOP_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PassaggioSitoAgronicaLabQualita_NG(CoreWSRequest<CoreWS_Generic<InDataUtility>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoAgronicaLabQualita_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PassaggioSitoPianiCampionamento_NG(CoreWSRequest<CoreWS_Generic<InDataUtility>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoPianiCampionamento_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PassaggioSitoPianiSemina_NG(CoreWSRequest<CoreWS_Generic<InDataUtility>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoPianiSemina_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PassaggioSitoPianoConcimazione_NG(CoreWSRequest<CoreWS_Generic<PassaggioSitoPianoConcimazione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoPianoConcimazione_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PassaggioSitoStampe_2010_NG(CoreWSRequest<CoreWS_Generic<InDataUtility>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoStampe_2010_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PassaggioSitoAgronicaUma_NG(CoreWSRequest<CoreWS_Generic<InDataUtility>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoAgronicaUma_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PassaggioSitoAgronicaDomandaIrrigua_NG(CoreWSRequest<CoreWS_Generic<InDataUtility>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoAgronicaDomandaIrrigua_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard PassaggioSitoGiasOnlineVecchio_NG(CoreWSRequest<CoreWS_Generic<InDataUtility>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/PassaggioSitoGiasOnlineVecchio_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<Imprese_Impostazioni>> Get_Imprese_Impostazioni_NG(CoreWSRequest<CoreWS_Generic<Get_Imprese_Impostazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/Get_Imprese_Impostazioni_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Imprese_Impostazioni>>>(result.ToString());
        }
        public RispostaStandard Redirect_NG(CoreWSRequest<CoreWS_Generic<InDataUtility>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/Redirect_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard GestioneStampe_NG(CoreWSRequest<CoreWS_Generic<GestioneStampe>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/GestioneStampe_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard getLinkProfitosan(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/getLinkProfitosan", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<Utente_Permessi> getUtente(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/getUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Utente_Permessi>>(result.ToString());
        }
        public RispostaStandard<ModuliGias> getModuli(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/getModuli", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ModuliGias>>(result.ToString());
        }

        public RispostaStandard ScriviMacchine(CoreWSRequest<CoreWS_Generic<List<ParcoMacchine>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/ScriviMacchine", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard VerificaAssociazioneBTM(CoreWS_Parco_Macchine_BTM request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/VerificaAssiciazioneBTM", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiRilievi(string objP_super_server, string objP_server, string objP_utenti, string operazione, bool personalizzate)
        {
            bool avversita = operazione == CostantiPersonalizzate.LAVCOD_RILIEVO_AVVERSITA_CAMPO.ToString();
            string dpiCod = avversita ? "0" : "";
            string idRcdpi = avversita ? "0" : "";
            string dpiPubblicoPrivato = avversita ? "1" : "";
            bool filtraSpecie = true;

            if (operazione == CostantiPersonalizzate.LAVCOD_RILIEVO_INDICI_MATURITA.ToString() ||
                operazione == CostantiPersonalizzate.LAVCOD_DANNI_RACCOLTA.ToString())
            {
                filtraSpecie = false;
            }

            return LeggiRilievi(new CoreWS_Rilievi(objP_super_server, objP_server, objP_utenti, operazione, "0", dpiCod, idRcdpi, dpiPubblicoPrivato, personalizzate, filtraSpecie));
        }

        public RispostaStandard LeggiRilievi(CoreWS_Rilievi request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Rilievi.asmx/PopolaRilievo_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PopolaRilievo_NG(CoreWSRequest<CoreWS_Generic<PopolaRilievo>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Rilievi.asmx/PopolaRilievo_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public string Leggi_ListaRilievi_New(CoreWSRequest<CoreWS_Generic<LeggiRilievi>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Rilievi.asmx/Leggi_ListaRilievi_ToKendoGrid_new", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString()).RispostaStringa;
        }

        public RispostaStandard<GisDataReadRval<GeoJSONAgroGisProp>> GisTestData(CoreWS_Gis<GisDataReadParam> request)
        {
            JObject result = Request(HttpMethod.Post, "/GIS/GISWS.asmx/TestData", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<GisDataReadRval<GeoJSONAgroGisProp>>>(result.ToString());
        }

        public RispostaStandard<GisDataReadRval_New<GeoJSONAgroGisProp>> GisLeggiElencoEntitaGeoJson(CoreWS_Gis<GisDataReadParam> request)
        {
            JObject result = Request(HttpMethod.Post, "/GIS/GISWS.asmx/LeggiElencoEntitaGeoJson", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<GisDataReadRval_New<GeoJSONAgroGisProp>>>(result.ToString());
        }

        public RispostaStandard<STBufferGeoJsonPolygonOutData> GisLeggiPoligono(CoreWS_Gis<STBufferGeoJsonPolygonInData> request)
        {
            JObject result = Request(HttpMethod.Post, "/GIS/GISWS.asmx/STBufferGeoJsonPolygon", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<STBufferGeoJsonPolygonOutData>>(result.ToString());
        }

        public RispostaStandard<CoreWSGisEndPoints_SalvaNuovoElementoGraficoDaChiaveAlberoOutData> GisScriviElementoGrafico(CoreWS_Gis<CoreWSGisEndPoints_SalvaNuovoElementoGraficoDaChiaveAlberoInData> request)
        {
            JObject result = Request(HttpMethod.Post, "/GIS/GISWS.asmx/SalvaNuovoElementoGraficoDaChiaveAlbero", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<CoreWSGisEndPoints_SalvaNuovoElementoGraficoDaChiaveAlberoOutData>>(result.ToString());
        }

        public RispostaStandard<CoreWSGisEndPoints_ModificaImpianto2019OutData> GisModificaImpianto(CoreWS_Gis<CoreWSGisEndPoints_ModificaImpianto2019InData> request)
        {
            JObject result = Request(HttpMethod.Post, "/GIS/GISWS.asmx/ModificaImpianto2019", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<CoreWSGisEndPoints_ModificaImpianto2019OutData>>(result.ToString());
        }

        public RispostaStandard<List<DestinazioneUso>> LeggiDestinazioniUso(CoreWSRequest<CoreWS_Generic<LeggiDestinazioniUso>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/SpecieVegetali.asmx/LeggiDestinazioniUso", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<DestinazioneUso>>>(result.ToString());
        }

        public RispostaStandard<List<Specie>> LeggiSpecie(CoreWSRequest<CoreWS_Generic<LeggiSpecie>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/SpecieVegetali.asmx/LeggiFiltroUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Specie>>>(result.ToString());
        }

        public RispostaStandard<List<Varieta>> LeggiVarieta(CoreWSRequest<CoreWS_Generic<LeggiCultivar>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Cultivar.asmx/LeggiFiltroUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Varieta>>>(result.ToString());
        }

        public RispostaStandard<List<GruppoFinalita>> LeggiGruppoFinalita(CoreWSRequest<CoreWS_Generic<LeggiFinalita>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/GruppoFinalita.asmx/Leggi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<GruppoFinalita>>>(result.ToString());
        }
        public RispostaStandard LeggiComboGruppoFinalitaNuovoImpianto(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/GruppoFinalita.asmx/LeggiComboGruppoFinalitaNuovoImpianto", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<Disciplinare>> LeggiDisciplinariEnte(CoreWSRequest<CoreWS_Generic<LeggiDisciplinari>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreDPI/DPI.asmx/CaricaComboDisciplinare_Ente_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Disciplinare>>>(result.ToString());
        }

        public RispostaStandard<List<Disciplinare>> LeggiDisciplinari(CoreWSRequest<CoreWS_Generic<LeggiDisciplinari>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreDPI/DPI.asmx/CaricaComboDisciplinare_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Disciplinare>>>(result.ToString());
        }

        public RispostaStandard<List<Disciplinare>> Leggi_Disciplinari_Testata_conRegolamentoConcimazione(CoreWSRequest<CoreWS_Generic<LeggiDisciplinari>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreDPI/DPI.asmx/Leggi_Disciplinari_Testata_conRegolamentoConcimazione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Disciplinare>>>(result.ToString());
        }

        public RispostaStandard<List<Fabbricato>> LeggiMagazziniModello(CoreWSRequest<CoreWS_Generic<LeggiMagazzini>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Fabbricati.asmx/LeggiMagazziniAPP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Fabbricato>>>(result.ToString());
        }

        public RispostaStandard ScriviMagazziniModello(CoreWSRequest<CoreWS_Generic<List<Fabbricato>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Fabbricati.asmx/ScriviMagazziniAPP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<Appezzamento> LeggiAppezzamentoModello(CoreWSRequest<CoreWS_Generic<LeggiAppezzamento>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/Leggi_Appezzamento_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Appezzamento>>(result.ToString(), new UtilizzoTerrenoConverter());
        }

        public RispostaStandard<Appezzamento> ScriviAppezzamentiModello(CoreWSRequest<CoreWS_Generic<Appezzamento>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/Scrivi_Appezzamento_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Appezzamento>>(result.ToString(), new UtilizzoTerrenoConverter());
        }

        public RispostaStandard<List<Provincia>> LeggiProvince(CoreWSRequest<CoreWS_Generic<GetProvincie>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ISTAT.asmx/GetProvince_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Provincia>>>(result.ToString());
        }

        public RispostaStandard<List<Comune>> LeggiComuni(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ISTAT.asmx/GetComuni_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Comune>>>(result.ToString());
        }

        public RispostaStandard<List<CodiciNazioniISO3166>> LeggiNazioni(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ISTAT.asmx/GetStati_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<CodiciNazioniISO3166>>>(result.ToString());
        }

        public RispostaStandard<object> LeggiAttivitaDaRicettaOperazione(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiAttivitaDaRicettaOperazione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }

        public RispostaStandard VerificaAttivita(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/VerificaAttivita", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        // Relativa alla nuova gestione di verifica conformita 2025
        public RispostaStandard VerificaConformitaAttivita(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Agenda.ScriviListaAttivita>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/VerificaConformitaAttivita", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PostAttivita(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/ScriviAttivita", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<PresenzaMovimentazioni> VerificaMovimentiXImpianto(CoreWSRequest<CoreWS_Generic<Impianto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/Verifica_OperazioniAgenda", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<PresenzaMovimentazioni>>(result.ToString());
        }

        public RispostaStandard LeggiDatiApp(CoreWS_Dati_App request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/LeggiDatiApp", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiDatiAppStorico(CoreWS_Dati_App request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/LeggiDatiAppStorico", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ConsultaSincroDatiApp(CoreWSRequest<CoreWS_Generic<ConsultaSincroDatiApp>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/ConsultaSincroDatiApp", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<OutData.Logging.MonitorLogInterscambio_OUT> ConsultaLogInterscambio(CoreWSRequest<CoreWS_Generic<ConsultaSincroDatiApp>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Logging/LogInterscambio.asmx/ConsultaLogInterscambio", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<OutData.Logging.MonitorLogInterscambio_OUT>>(result.ToString());
        }

        public RispostaStandard ControllaModalitaDemetra(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Logging/LogInterscambio.asmx/Get_isModalitaDemetra", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<Impresa> LeggiImpresaModello(CoreWSRequest<CoreWS_Generic<LeggiImpresa>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Leggi_Impresa_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Impresa>>(result.ToString());
        }

        public RispostaStandard<List<Impresa>> LeggiImpreseModello(CoreWS_Imprese request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/LeggiImprese", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Impresa>>>(result.ToString());
        }

        public RispostaStandard<List<CentroAziendale>> LeggiCentriAziendaliModello(CoreWS_Centri_Aziendali request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/LeggiCentri", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<CentroAziendale>>>(result.ToString());
        }

        public RispostaStandard<List<Campo>> LeggiCampiModello(CoreWS_Campi request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/LeggiCampi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Campo>>>(result.ToString());
        }

        public RispostaStandard<List<Appezzamento>> LeggiAppezzamentiModello(CoreWS_Appezzamenti request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/SincroDatiApp.asmx/LeggiAppezzamenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Appezzamento>>>(result.ToString());
        }

        public RispostaStandard CreaProdotti(CoreWSRequest<CoreWS_Generic<CreaProdotti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/Crea_Prodotti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        #region Funzioni Agenda

        public RispostaStandard<UtilizzoTerreno> LeggiUtilizzoTerrenoImpianto(CoreWSRequest<CoreWS_Generic<ChiaveImpianto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Impianti.asmx/LeggiUtilizzoTerrenoImpianto", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<UtilizzoTerreno>>(result.ToString());
        }

        public RispostaStandard<ModificaListaRicetteQdC> GestioneFlagRicettaInviaApp(CoreWSRequest<CoreWS_Generic<ModificaListaRicetteQdC>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/GestioneFlagRicettaInviaApp", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ModificaListaRicetteQdC>>(result.ToString());
        }

        public RispostaStandard CaricaListaPersone(CoreWSRequest<CoreWS_Generic<ModificaMultipla_Attivita>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/CaricaListaPersone", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CaricaListaMacchine(CoreWSRequest<CoreWS_Generic<ModificaMultipla_Attivita>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/CaricaListaMacchine", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard InfomodificaOperazioneSingola(CoreWSRequest<CoreWS_Generic<InfomodificaOperazioneSingola>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/infomodifica_operazione_singola_new", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<ListaMisurePerAvversitaAnagrafica> LeggiMisurePerAvversitaAnagrafiche(CoreWSRequest<CoreWS_Generic<LeggiMisuraPerAvversitaAnagrafica_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiMisurePerAvversitaAnagrafiche", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ListaMisurePerAvversitaAnagrafica>>(result.ToString());
        }

        public RispostaStandard<ListaMisurePerAvversitaAnagraficaExtended> LeggiMisurePerAvversitaAnagraficheExtended(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiMisurePerAvversitaAnagraficheExtended", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ListaMisurePerAvversitaAnagraficaExtended>>(result.ToString());
        }

        public RispostaStandard MisurePerAvversitaAnagrafiche(CoreWSRequest<CoreWS_Generic<MisuraPerAvversitaAnagrafica_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/MisurePerAvversitaAnagrafiche", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard indiciMaturitaAnagrafiche(CoreWSRequest<CoreWS_Generic<LeggiMisureAvversita>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiMisureIndiciMaturitaAnagrafiche", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<ListaMisuraPerIndiciMaturitaAnagrafica> LeggiMisureIndiciMaturitaAnagrafiche2(CoreWSRequest<CoreWS_Generic<LeggiMisuraPerIndiciMaturitaAnagrafiche_IN>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiMisureIndiciMaturitaAnagrafiche2", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ListaMisuraPerIndiciMaturitaAnagrafica>>(result.ToString());
        }

        public RispostaStandard MisurePerIndiciMaturita(CoreWSRequest<CoreWS_Generic<MisuraPerIndiciMaturita_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/MisurePerIndiciMaturita", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<ListaMisuraPerIndiciMaturita> LeggiMisureIndiciMaturita(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiMisureIndiciMaturita", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ListaMisuraPerIndiciMaturita>>(result.ToString());
        }

        #endregion

        #region Funzioni QdC

        public RispostaStandard PopolaRilievo(CoreWSRequest<CoreWS_Generic<PopolaRilievo>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Rilievi.asmx/PopolaRilievo_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiAgendaStatistiche(CoreWSRequest<CoreWS_Generic<InData.Agenda.LeggiAgendaStatistiche>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Contab/AgendaStatistiche.asmx/LeggiAgendaStatistiche", JsonConvert.SerializeObject(request)); //Contab/AgendaStatistiche.asmx/LeggiAgendaStatistiche'
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        #endregion

        #region Funzioni Profilazione

        public RispostaStandard AggiungiGuidaImpostazione(CoreWSRequest<CoreWS_Generic<AgronicaCoreModelsSTD.profilazione.Impostazione[]>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpostazioniUtente.asmx/AggiungiInTabellaGuida", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiImpostazioniSezioni(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpostazioniUtente.asmx/LeggiSezioniGuida", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiGuideImpostazioniUtenti(CoreWSRequest<CoreWS_Generic<Leggi_Impostazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpostazioniUtente.asmx/Carica_ImpostazioniUtente_Sezioni", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiControlliImpostazioniUtenti(CoreWSRequest<CoreWS_Generic<Leggi_Impostazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpostazioniUtente.asmx/Carica_ImpostazioniTemplate", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard SalvaImpostazioniUtenti(CoreWSRequest<CoreWS_Generic<AgronicaCoreModelsSTD.utente.Utente[]>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpostazioniUtente.asmx/Salva_ImpostazioniUtenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ResetImpostazioniUtentiDefault(CoreWSRequest<CoreWS_Generic<AgronicaCoreModelsSTD.utente.Utente[]>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpostazioniUtente.asmx/ResetDefault", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CopiaImpostazioniUtenti(CoreWSRequest<CoreWS_Generic<CopiaImpostazioniObj>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpostazioniUtente.asmx/Copia_ImpostazioniUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard Leggi_GruppiOperazioni_FiltroImpostazioni(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Operazioni.asmx/Leggi_GruppiOperazioni_FiltroImpostazioni", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CreaUtenteConVisibilita(CoreWSRequest<CoreWS_Generic<LeggiScriviVisibilitaUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Visibilita.asmx/CreaUtenteConVisibilita", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard RimuoviVisibilitaPerUtente(CoreWSRequest<CoreWS_Generic<RimuoviVisibilitaUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Visibilita.asmx/RimuoviVisibilitaPerUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ModificaVisibilitaUtente(CoreWSRequest<CoreWS_Generic<LeggiScriviVisibilitaUtenti>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Visibilita.asmx/ModificaVisibilitaUtenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard RimuoviVisibilitaUtente(CoreWSRequest<CoreWS_Generic<LeggiScriviVisibilitaUtenti>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Visibilita.asmx/RimuoviVisibilitaUtenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CopiaVisibilitaUtenti(CoreWSRequest<CoreWS_Generic<CopiaVisibilitaObj>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Visibilita.asmx/CopiaVisibilitaUtenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiAziendeVisibilita(CoreWSRequest<CoreWS_Generic<LeggiScriviVisibilitaUtenti>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Visibilita.asmx/LeggiAziendeVisibilita", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CheckFathersInList(CoreWSRequest<CoreWS_Generic<AgronicaCoreModelsSTD.anagrafiche.ImpresaDto[]>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Visibilita.asmx/CheckFathersInList", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiVisibilitaUtenti(CoreWSRequest<CoreWS_Generic<LeggiScriviVisibilitaUtenti>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Visibilita.asmx/LeggiVisibilitaUtenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiVisibilitaGruppi(CoreWSRequest<CoreWS_Generic<LeggiScriviVisibilitaUtenti>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Visibilita.asmx/LeggiVisibilitaGruppi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard HaVisibilitaTotale(CoreWSRequest<CoreWS_Generic<List<string>>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Visibilita.asmx/HaVisibilitaTotale", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ControllaStessaVisibilita(CoreWSRequest<CoreWS_Generic<List<string>>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Visibilita.asmx/ControllaStessaVisibilita", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ConfrontaVisibilitaUtenti(CoreWSRequest<CoreWS_Generic<ConfrontaVisibilitaUtenti>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Visibilita.asmx/ConfrontaVisibilitaUtenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ImpostaLingua(CoreWSRequest<CoreWS_Generic<CambioLinguaObj>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_W.asmx/ImpostaLingua", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard AssociaProfilo(CoreWSRequest<CoreWS_Generic<AssociaProfiloObj>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_W.asmx/AssociaProfilo", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ModificaValiditaPermessi(CoreWSRequest<CoreWS_Generic<AssociaProfiloObj>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_W.asmx/ModificaValiditaPermessi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ModificaFinestraTemporale(CoreWSRequest<CoreWS_Generic<UtenteFinestraTemp[]>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_W.asmx/ModificaFinestraTemporale", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard AggiornaPermessiUtentiTipologia(CoreWSRequest<CoreWS_Generic<TipologiaUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Tipologie.asmx/AggiornaPermessiUtentiTipologia", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard AggiornaImpostazioniUtentiTipologia(CoreWSRequest<CoreWS_Generic<AssociaProfiloObj>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Tipologie.asmx/AggiornaImpostazioniUtentiTipologia", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard SalvaGruppoUtente(CoreWSRequest<CoreWS_Generic<GruppoUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Gruppi_Utente.asmx/SalvaGruppoUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        #endregion

        public RispostaStandard LeggiClientValidation(CoreWS_ClientValidation request)
        {
            JObject result = Request(HttpMethod.Post, "/Provisioning/Provisioning.asmx/LeggiClientValidation", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiClientValidationWS2010(CoreWS_ClientValidation request)
        {
            JObject result = Request(HttpMethod.Post, "/WS_Autenticazione.asmx/LeggiClientValidation", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public widgetManagerResponse LeggiWidgetManagerWS2010(CoreWS_WidgetManager request)
        {
            JObject result = Request(HttpMethod.Post, "/ProfilatoreUtenze.svc/widgetManager", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<widgetManagerResponse>(result.ToString());
        }
        public RispostaStandard LeggiApiValidationWS2010(CoreWS_ApiValidation request)
        {
            JObject result = Request(HttpMethod.Post, "/WS_Autenticazione.asmx/LeggiApiValidation", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<DatiServer> DatiServer(CoreWSRequest<CoreWS_Generic<DatiServerRequest>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Provisioning/Provisioning.asmx/DatiServer", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<DatiServer>>(result.ToString());
        }

        public RispostaStandard<List<Vista>> VisteGriglia_CaricaViste(CoreWSRequest<CoreWS_Generic<ChiaveVista>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Shared/VisteGriglia.asmx/CaricaViste", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Vista>>>(result.ToString());
        }

        public RispostaStandard VisteGriglia_CancellaVista(CoreWSRequest<CoreWS_Generic<ChiaveVista>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Shared/VisteGriglia.asmx/CancellaVista", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CreaNuovoToken(CoreWSRequest<CoreWS_Generic<CreaToken_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Provisioning/Provisioning.asmx/CreaNuovoToken", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard VerificaEsistenzaToken(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Provisioning/Provisioning.asmx/VerificaEsistenzaToken", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard VisteGriglia_ScriviViste(CoreWSRequest<CoreWS_Generic<SalvaVisteWrapper>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Shared/VisteGriglia.asmx/ScriviViste", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        #region FunzioniGIS_Importate

        public RispostaStandard<List<TipoEntita_Out>> GIS_TipoEntita_Popola(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/GIS_TipoEntita_Popola", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<TipoEntita_Out>>>(result.ToString());
        }

        public RispostaStandard<ApriSitoAnalisi_Out> ApriSitoAnalisixVisualizzazione(CoreWSRequest<CoreWS_Generic<LeggiEntita_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/ApriSitoAnalisixVisualizzazione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ApriSitoAnalisi_Out>>(result.ToString());
        }

        public RispostaStandard<SingoloObjImpianto_Out> CaricaSingoloOggetto_Impianto(CoreWSRequest<CoreWS_Generic<ChiaveImpianto_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CaricaSingoloOggetto_Impianto", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<SingoloObjImpianto_Out>>(result.ToString());
        }

        public RispostaStandard<AnalisiMeteo_Out> AnalisiMeteoBs(CoreWSRequest<CoreWS_Generic<AnalisiMeteo_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/AnalisiMeteoBs", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AnalisiMeteo_Out>>(result.ToString());
        }

        public RispostaStandard BufferZone_Aggiorna(CoreWSRequest<CoreWS_Generic<BufferZone_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/BufferZone_Aggiorna", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<LeggiBufferZone_Out> BufferZone_Leggi(CoreWSRequest<CoreWS_Generic<BufferZone_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/BufferZone_Leggi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<LeggiBufferZone_Out>>(result.ToString());
        }

        public RispostaStandard CaricaImpreseCodici(CoreWSRequest<CoreWS_Generic<CaricaImpreseCod_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CaricaImpreseCodici", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ColorazioneAutomatica(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/ColorazioneAutomatica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard Get_CodiceFiscaleTecnico(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/Get_CodiceFiscaleTecnico", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<ChiaveAlbero> GetChiave_ID_Agenda_Selezionato(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/GetChiave_ID_Agenda_Selezionato", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ChiaveAlbero>>(result.ToString());
        }

        public RispostaStandard<List<ObjInputHTML_Out>> ElencoOperazioniAgendaGraficabili(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/ElencoOperazioniAgendaGraficabili", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<ObjInputHTML_Out>>>(result.ToString());
        }

        public RispostaStandard<SalvaNuovoMultiPoint_Out> SalvaNuovoMultipoint(CoreWSRequest<CoreWS_Generic<SalvaNuovoMultiPoint_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/SalvaNuovoMultipoint", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<SalvaNuovoMultiPoint_Out>>(result.ToString());
        }

        public RispostaStandard CaricaAppNomeProgrammazione_des(CoreWSRequest<CoreWS_Generic<ChiaveAlbero>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CaricaAppNomeProgrammazione_des", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiGisPurpose(CoreWSRequest<CoreWS_Generic<LeggiGisPurpose_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiGisPurpose", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard VerificaCancella(CoreWSRequest<CoreWS_Generic<LeggiEntita_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/VerificaCancella", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard pfRateoSrv(CoreWSRequest<CoreWS_Generic<PfRateoSrv_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/pfRateoSrv", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard pfIndiceRischioSrv(CoreWSRequest<CoreWS_Generic<pfIndiciRischio_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/pfIndiceRischioSrv", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard pfPrescriptionUpload(CoreWSRequest<CoreWS_Generic<pfPrescriptionUploadCoreWS_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/pfPrescriptionUpload", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard pfPrescriptionUpdate(CoreWSRequest<CoreWS_Generic<pfPrescriptionUpdate_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/pfPrescriptionUpdate", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<string> MappaPrescrizioneImage(CoreWSRequest<CoreWS_Generic<MappaPrescrizioneImage_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GIS/GISWS.asmx/MappaPrescrizioneImage", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }

        public RispostaStandard<MappaPrescrizioneGroundOverlay_Out> MappaPrescrizioneGroundOverlay(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GIS/GISWS.asmx/MappaPrescrizioneGroundOverlay", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<MappaPrescrizioneGroundOverlay_Out>>(result.ToString());
        }

        public RispostaStandard<CoordsFromViaCentro_Out> CoordinateFromViaCentro(CoreWSRequest<CoreWS_Generic<CoordinateFromImpresa_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CoordinateFromViaCentro", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<CoordsFromViaCentro_Out>>(result.ToString());
        }

        public RispostaStandard ControllaSeEsisteCatasto(CoreWSRequest<CoreWS_Generic<LeggiEntita_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/ControllaSeEsisteCatasto", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        //public RispostaStandard<Caricaplace_TabHTML_Out> Caricaplace_tabella_Dettagli(CoreWSRequest<CoreWS_Generic<int>> request)
        //{
        //    JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/Caricaplace_tabella_Dettagli", JsonConvert.SerializeObject(request));
        //    return JsonConvert.DeserializeObject<RispostaStandard<Caricaplace_TabHTML_Out>>(result.ToString());
        //}

        //public RispostaStandard Caricaplace_tabella_Dettagli2(CoreWSRequest<CoreWS_Generic<int>> request)
        //{
        //    JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/Caricaplace_tabella_Dettagli2", JsonConvert.SerializeObject(request));
        //    return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        //}

        public RispostaStandard<List<ObjOptionHTML_Out>> Carica_ddlTipologiaLayer(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/Carica_ddlTipologiaLayer", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<ObjOptionHTML_Out>>>(result.ToString());
        }

        public RispostaStandard ControllaSeEsistonoOperazioni(CoreWSRequest<CoreWS_Generic<ChiaveAlbero>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/ControllaSeEsistonoOperazioni", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<GetProprieta_Out> GetProprieta(CoreWSRequest<CoreWS_Generic<GetProprieta_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/GetProprieta", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<GetProprieta_Out>>(result.ToString());
        }

        public RispostaStandard<GisDataReadRval_New<GeoJSONAgroGisProp>> CaricaDatiPrecisionXmlDaAllegati(CoreWSRequest<CoreWS_Generic<CaricaDatiPrecision_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CaricaDatiPrecisionXmlDaAllegati", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<GisDataReadRval_New<GeoJSONAgroGisProp>>>(result.ToString());
        }

        public RispostaStandard<VerificaInterferenze_Out> VerificaInterferenze(CoreWSRequest<CoreWS_Generic<VerificaInterferenze_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/VerificaInterferenze", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<VerificaInterferenze_Out>>(result.ToString());
        }

        public RispostaStandard<VerificaVicini_Out> VerificaVicini(CoreWSRequest<CoreWS_Generic<VerificaVicini_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/VerificaVicini", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<VerificaVicini_Out>>(result.ToString());
        }

        public RispostaStandard ElencoEntitaCodXRecuperoStaticMaps(CoreWSRequest<CoreWS_Generic<ElencoEntitaCodXRecuperoStaticMaps>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/ElencoEntitaCodXRecuperoStaticMaps", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<Entita_Info_Out>> InfoInterferenze(CoreWSRequest<CoreWS_Generic<InfoInterferenze_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/InfoInterferenze", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Entita_Info_Out>>>(result.ToString());
        }

        public RispostaStandard<Cancella_Out> Cancella(CoreWSRequest<CoreWS_Generic<Cancella_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/Cancella", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Cancella_Out>>(result.ToString());
        }

        public RispostaStandard<Cancella_EntitaGraf_Out> CancellaEntitaGrafiche(CoreWSRequest<CoreWS_Generic<List<string>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CancellaEntitaGrafiche", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Cancella_EntitaGraf_Out>>(result.ToString());
        }

        public RispostaStandard CancellaMultipoint(CoreWSRequest<CoreWS_Generic<List<string>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CancellaMultipoint", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard VerificaPoligono(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/VerificaPoligono", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<SalvaNuovoAB_Out> SalvaNuovoAB(CoreWSRequest<CoreWS_Generic<SalvaNuovoAB_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/SalvaNuovoAB", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<SalvaNuovoAB_Out>>(result.ToString());
        }

        public RispostaStandard<DateDefault_Out> CaricaDateDefault(CoreWSRequest<CoreWS_Generic<CaricaDate_Default_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CaricaDateDefault", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<DateDefault_Out>>(result.ToString());
        }

        public RispostaStandard SementiMappaturaLiberaSpecieVegetalePermessa(CoreWSRequest<CoreWS_Generic<SementiMappaturaLibera_SpecieVegPermessa_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/SementiMappaturaLiberaSpecieVegetalePermessa", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard SementiSportelloPossoDisegnare(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/SementiSportelloPossoDisegnare", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<Lista_GisSat_SentinelOverlay_Out> CustomMapOverlayBase_InizializzaCalendario(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CustomMapOverlayBase_InizializzaCalendario", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Lista_GisSat_SentinelOverlay_Out>>(result.ToString());
        }

        public RispostaStandard<Lista_GisSat_SentinelOverlay_Out> CustomMapOverlayBaseEntitaGIS_InizializzaCalendario(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CustomMapOverlayBaseEntitaGIS_InizializzaCalendario", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Lista_GisSat_SentinelOverlay_Out>>(result.ToString());
        }

        //public RispostaStandard<ElencoTipologieLayer> AggiornaElencoTipologie3(CoreWSRequest<CoreWS_Generic<AggiornaElencoTipologie_In>> request)
        //{
        //    JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/AggiornaElencoTipologie3", JsonConvert.SerializeObject(request));
        //    return JsonConvert.DeserializeObject<RispostaStandard<ElencoTipologieLayer>>(result.ToString());
        //}

        public RispostaStandard<ElencoTipologieLayer> AggiornaElencoTipologie4(CoreWSRequest<CoreWS_Generic<AggiornaElencoTipologie_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/AggiornaElencoTipologie4", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ElencoTipologieLayer>>(result.ToString());
        }

        public RispostaStandard SalvaColoriLayer2(CoreWSRequest<CoreWS_Generic<SalvaColoriLayer2_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/SalvaColoriLayer2", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard SalvaVisibilitaLayer(CoreWSRequest<CoreWS_Generic<SalvaVisibilitaLayer_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/SalvaVisibilitaLayer", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard SalvaFlagVisibilitaLayer(CoreWSRequest<CoreWS_Generic<SalvaFlagVisibilitaLayer_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/SalvaFlagVisibilitaLayer", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<ScriviNuovoLayerPersonalizzato_Out> ScriviNuovoLayerPersonalizzato(CoreWSRequest<CoreWS_Generic<ScriviNuovoLayerPersonalizzato_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/ScriviNuovoLayerPersonalizzato", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ScriviNuovoLayerPersonalizzato_Out>>(result.ToString());
        }
        
        public RispostaStandard<bool> DeleteCustomLayer(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/DeleteCustomLayer", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }

        public RispostaStandard<Obj_SalvaGrafica> SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza(CoreWSRequest<CoreWS_Generic<SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza_2022", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Obj_SalvaGrafica>>(result.ToString());
        }

        public RispostaStandard WmsGetFeature(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/WmsGetFeature", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<DatiStrutturaAttributiLayer> LeggiElencoStrutturaAttributiLayer(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiElencoStrutturaAttributiLayer", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<DatiStrutturaAttributiLayer>>(result.ToString());
        }

        public RispostaStandard<SalvaEntitaConAttributi_Out> SalvaEntitaConAttributi(CoreWSRequest<CoreWS_Generic<SalvaEntitaConAttributi_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/SalvaEntitaConAttributi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<SalvaEntitaConAttributi_Out>>(result.ToString());
        }

        public RispostaStandard<VerificaEsistenzaEntitaPerImpianti_Out> VerificaEsistenzaEntitaPerImpianti(CoreWSRequest<CoreWS_Generic<VerificaEsistenzaEntitaPerImpianti_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/VerificaEsistenzaEntitaPerImpianti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<VerificaEsistenzaEntitaPerImpianti_Out>>(result.ToString());
        }

        public RispostaStandard<AggiornaFiltroImpianti_Out> AggiornaFiltroImpianti(CoreWSRequest<CoreWS_Generic<AggiornaFiltroImpianti_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/AggiornaFiltroImpianti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AggiornaFiltroImpianti_Out>>(result.ToString());
        }

        public RispostaStandard<LeggiImpostazioniAvanzateLayer> LeggiImpostazioniAvanzateLayer(CoreWSRequest<CoreWS_Generic<LeggiImpostazioniAvanzateLayer_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiImpostazioniAvanzateLayer", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<LeggiImpostazioniAvanzateLayer>>(result.ToString());
        }

        public RispostaStandard AttributoLayer(CoreWSRequest<CoreWS_Generic<AttributoLayer_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/AttributoLayer", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ImpostaVisualizzazioneEtichetta(CoreWSRequest<CoreWS_Generic<ImpostaVisualizzazioneEtichetta_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/ImpostaVisualizzazioneEtichetta", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ImpostaCampoChiaveLayer(CoreWSRequest<CoreWS_Generic<ImpostaCampoChiaveLayer_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/ImpostaCampoChiaveLayer", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ImpostaAttivazioneAttributoLayer(CoreWSRequest<CoreWS_Generic<AttivaAttributoLayer_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/ImpostaAttivazioneAttributoLayer", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard AggiornaElementoGraficoPerTipoOggetto(CoreWSRequest<CoreWS_Generic<AggiornaElementoGraficoPerTipoOggetto_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/AggiornaElementoGraficoPerTipoOggetto", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<LeggiTipiOggettoPerElementoGrafico> LeggiTipiOggettoPerElementoGrafico(CoreWSRequest<CoreWS_Generic<ElementoGraficoPerTipoOggetto_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiTipiOggettoPerElementoGrafico", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<LeggiTipiOggettoPerElementoGrafico>>(result.ToString());
        }
        public RispostaStandard<LeggiAllegatiDaRicettaDestinazione> LeggiAllegatiDaRicettaDestinazione(CoreWSRequest<CoreWS_Generic<LeggiAllegatiDaRicettaDestinazione_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiAllegatiDaRicettaDestinazione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<LeggiAllegatiDaRicettaDestinazione>>(result.ToString());
        }
        public RispostaStandard<VerificaEsistenzaEntitaAnagrafiche> VerificaEsistenzaEntitaAnagrafiche(CoreWSRequest<CoreWS_Generic<VerificaEsistenzaEntitaAnagrafiche_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/VerificaEsistenzaEntitaAnagrafiche", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<VerificaEsistenzaEntitaAnagrafiche>>(result.ToString());
        }
        public RispostaStandard<GisDataReadRval_New<GeoJSONAgroGisProp>> PosizioniRilevate(CoreWSRequest<CoreWS_Generic<LettureTecniciInCampo_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/PosizioniRilevate", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<GisDataReadRval_New<GeoJSONAgroGisProp>>>(result.ToString());
        }
        public RispostaStandard<GisDataReadRval_New<GeoJSONAgroGisProp>> UltimaPosizione(CoreWSRequest<CoreWS_Generic<LettureTecniciInCampo_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/UltimaPosizione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<GisDataReadRval_New<GeoJSONAgroGisProp>>>(result.ToString());
        }
        public RispostaStandard LetturaDatiElaboratiSuSensoreListaValori(CoreWSRequest<CoreWS_Generic<LetturaDatiElaboratiSuSensoreListaValori_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LetturaDatiElaboratiSuSensoreListaValori", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LayerTilesDescrizione(CoreWSRequest<CoreWS_Generic<LayerTilesDescrizione_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LayerTilesDescrizione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<TipologiaLabel>> LeggiLayerTilesDescrizione(CoreWSRequest<CoreWS_Generic<LeggiLayerTilesDescrizione_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiLayerTilesDescrizione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<TipologiaLabel>>>(result.ToString());
        }
        public RispostaStandard<string> LeggiSistemiRiferimento(CoreWSRequest<CoreWS_Generic<LeggiSistemiRiferimento_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiSistemiRiferimento", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }
        public RispostaStandard CaricaFileShape(CoreWSRequest<CoreWS_Generic<CaricaFileCompletoCoreWS_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CaricaFileShape", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<ElencoAlgoritmi> LeggiAlgoritmiProiezione(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiAlgoritmiProiezione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ElencoAlgoritmi>>(result.ToString());
        }
        public RispostaStandard SalvaConfigurazioneProiezione(CoreWSRequest<CoreWS_Generic<ConfigurazioneProiezione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/SalvaConfigurazioneProiezione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ActivateAlgorithm(CoreWSRequest<CoreWS_Generic<ConfigurazioneProiezione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/ActivateAlgorithm", JsonConvert.SerializeObject(request));
            var risposta = JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
            var rStringa = risposta.RispostaStringa;
            return new RispostaStandard()
            {
                RispostaOK = risposta.RispostaOK,
                RispostaStringa = rStringa,
                Errore = risposta.Errore
            };
        }

        public RispostaStandard SaveAlgorithmConfigurationCfg(CoreWSRequest<CoreWS_Generic<ConfigurazioneProiezione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/SaveAlgorithmConfigurationCfg", JsonConvert.SerializeObject(request));
            var risposta = JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
            var rStringa = risposta.RispostaStringa;
            return new RispostaStandard()
            {
                RispostaOK = risposta.RispostaOK,
                RispostaStringa = rStringa,
                Errore = risposta.Errore
            };
        }

        public RispostaStandard<ElencoConfigurazioniProiezione> LeggiConfigurazioniProiezione(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiConfigurazioniProiezione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ElencoConfigurazioniProiezione>>(result.ToString());
        }
        public RispostaStandard<ElencoConfigurazioniProiezione> LeggiConfigurazioniProiezioneFiltrati(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiConfigurazioniProiezioneFiltrati", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ElencoConfigurazioniProiezione>>(result.ToString());
        }
        public RispostaStandard<ElencoConfigurazioniSuLayer> LeggiConfigurazioniProiezioneSuLayer(CoreWSRequest<CoreWS_Generic<LeggiElencoConfigurazioni_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiConfigurazioniProiezioneSuLayer", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ElencoConfigurazioniSuLayer>>(result.ToString());
        }
        public RispostaStandard<ElencoPermessiConfigurazione> LeggiPermessiUtenteConfigurazioni(CoreWSRequest<CoreWS_Generic<Int32>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiPermessiUtenteConfigurazioni", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ElencoPermessiConfigurazione>>(result.ToString());
        }
        public RispostaStandard<ElencoPermessiConfigurazione> LeggiPermessiGruppiUtenteConfigurazioni(CoreWSRequest<CoreWS_Generic<Int32>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiPermessiGruppiUtenteConfigurazioni", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ElencoPermessiConfigurazione>>(result.ToString());
        }
        public RispostaStandard OperazioniPermessiProiezioni(CoreWSRequest<CoreWS_Generic<ModifichePermessiConfigurazione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/OperazioniPermessiProiezioni", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<ElencoPermessiUtente> LeggiPermessiConfigurazioniDaUtente(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiPermessiConfigurazioniDaUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ElencoPermessiUtente>>(result.ToString());
        }
        public RispostaStandard<AttivaDisattivaConfigurazione_Out> AttivaDisattivaConfigurazione(CoreWSRequest<CoreWS_Generic<AttivazioneConfigurazioneAlgoritmiCartografici>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/AttivaDisattivaConfigurazione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AttivaDisattivaConfigurazione_Out>>(result.ToString());
        }
        public RispostaStandard<ElencoLogEsecuzioniConfigurazioniProiezione> LeggiLogEsecuzioniConfigurazione(CoreWSRequest<CoreWS_Generic<Int32>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiLogEsecuzioniConfigurazione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ElencoLogEsecuzioniConfigurazioniProiezione>>(result.ToString());
        }
        public RispostaStandard<ElencoMaschereLayerRaster> LeggiMaschereLayerRaster(CoreWSRequest<CoreWS_Generic<MaschereLayerFiltro_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiMaschereLayerRaster", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ElencoMaschereLayerRaster>>(result.ToString());
        }
        public RispostaStandard OperazioniMaschereLayerRaster(CoreWSRequest<CoreWS_Generic<OperazioneMascheraLayerRaster_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/OperazioniMaschereLayerRaster", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<ElencoPermessiMaschera> LeggiPermessiUtenteMaschera(CoreWSRequest<CoreWS_Generic<Int32>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiPermessiUtenteMaschera", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ElencoPermessiMaschera>>(result.ToString());
        }
        public RispostaStandard<ElencoPermessiMaschera> LeggiPermessiGruppiUtenteMaschera(CoreWSRequest<CoreWS_Generic<Int32>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiPermessiGruppiUtenteMaschera", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ElencoPermessiMaschera>>(result.ToString());
        }
        public RispostaStandard OperazioniPermessiMaschera(CoreWSRequest<CoreWS_Generic<ModifichePermessiMaschera>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/OperazioniPermessiMaschera", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard AttivaDisattivaMaschera(CoreWSRequest<CoreWS_Generic<AttivazioneMascheraLayerRaster>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/AttivaDisattivaMaschera", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard NuovaElaborazionePianoConcimazioneGEE(CoreWSRequest<CoreWS_Generic<PianoConcimazioneGEE>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/NuovaElaborazionePianoConcimazioneGEE", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard NuovaElaborazionePiattaformaGEE(CoreWSRequest<CoreWS_Generic<NuovaElaborazioneGEE>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/NuovaElaborazionePiattaformaGEE", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard NuovaElaborazionePiattaformaSAT(CoreWSRequest<CoreWS_Generic<SatCloudEvent>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/NuovaElaborazionePiattaformaSAT", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard NuovaElaborazionePiattaformaMappePrescrizione(CoreWSRequest<CoreWS_Generic<MappePrescrizioneCloudEvent>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/NuovaElaborazioneMappePrescrizione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<ElencoAllegatiLayer_Out> LeggiAllegatiLayer(CoreWSRequest<CoreWS_Generic<LeggiDatiLayer_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiAllegatiLayer", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ElencoAllegatiLayer_Out>>(result.ToString());
        }
        public RispostaStandard<AllegatoFile> LeggiAllegatoDocumento(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiAllegatoDocumento", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AllegatoFile>>(result.ToString());
        }
        public RispostaStandard EsportaShapeEntita(CoreWSRequest<CoreWS_Generic<EsportaShapeEntita_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/EsportaShapeEntita", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ModificaEstrazioneShape(CoreWSRequest<CoreWS_Generic<AllegatoLayerModifica>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/ModificaEstrazioneShape", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<RasterInfoClick_Out>> RasterInfoClick(CoreWSRequest<CoreWS_Generic<RasterInfoClick_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/RasterInfoClick", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<RasterInfoClick_Out>>>(result.ToString());
        }
        public RispostaStandard<string> LeggiChiaveAlbero(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiChiaveAlbero", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }
        #endregion

        #region GIASApp

        public RispostaStandard PushNotification(CoreWSRequest<CoreWS_Generic<Notifica_Utente_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/GiasAppWS.asmx/PushNotification", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard PopNotification(CoreWSRequest<CoreWS_Generic<Notifica_Utente_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/GiasAppWS.asmx/PopNotification", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<ServiziSottoscrivibili_Out> ServiziNotificheSottoscrivibili(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GiasApp/GiasAppWS.asmx/ServiziNotificheSottoscrivibili", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ServiziSottoscrivibili_Out>>(result.ToString());
        }
        #endregion

        #region RETAIL

        public RispostaStandard NuovoUtenteRetail(CoreWSRequest<CoreWS_Generic<JObject>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Provisioning/Provisioning.asmx/NuovoUtenteRetail", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard RiportaNuovoUtente(CoreWSRequest<CoreWS_Generic<JObject>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Provisioning/Provisioning.asmx/RiportaNuovoUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard RinnovaUtente(CoreWSRequest<CoreWS_Generic<JObject>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Provisioning/Provisioning.asmx/RinnovaUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CambiaEmailUtente(CoreWSRequest<CoreWS_Generic<NuovaEmail_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Provisioning/Provisioning.asmx/CambiaEmailUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        #endregion

        #region MUZ
        public RispostaStandard<List<DatiMUZVisibili_Out>> LeggiDatiMUZVisibili(CoreWSRequest<CoreWS_Generic<LeggiDatiMUZVisibili_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiDatiMUZVisibili", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<DatiMUZVisibili_Out>>>(result.ToString());
        }

        public RispostaStandard<List<MUZ>> LeggiMUZ(CoreWSRequest<CoreWS_Generic<LeggiMUZ_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiMUZ", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<MUZ>>>(result.ToString());
        }

        public RispostaStandard<List<GruppoAreaOmogenea>> LeggiListaGruppiMUZ(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiListaGruppiMUZ", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<GruppoAreaOmogenea>>>(result.ToString());
        }

        public RispostaStandard OperazioniGruppiMUZ(CoreWSRequest<CoreWS_Generic<OperazioniGruppiMUZ_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/OperazioniGruppiMUZ", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard OperazioniMUZ(CoreWSRequest<CoreWS_Generic<MUZ>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/OperazioniMUZ", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        #endregion

        #region MESSAGGISTICA_ESECUZIONI
        public RispostaStandard AccodaMessaggioEsecuzione(CoreWSRequest<CoreWS_Generic<MessaggioEsecuzione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Messaggistica/MessaggisticaWS.asmx/AccodaMessaggioEsecuzione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<MessaggioEsecuzione>> LeggiMessaggiEsecuzioneNonLetti(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Messaggistica/MessaggisticaWS.asmx/LeggiMessaggiEsecuzioneNonLetti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<MessaggioEsecuzione>>>(result.ToString());
        }

        public RispostaStandard<MessaggioEsecuzione> LeggiMessaggioEsecuzione(CoreWSRequest<CoreWS_Generic<Int32>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Messaggistica/MessaggisticaWS.asmx/LeggiMessaggioEsecuzione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<MessaggioEsecuzione>>(result.ToString());
        }

        public RispostaStandard ModificaMessaggioEsecuzione(CoreWSRequest<CoreWS_Generic<MessaggioEsecuzione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Messaggistica/MessaggisticaWS.asmx/ModificaMessaggioEsecuzione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        #endregion

        #region AUTHDISPATCHER
        public RispostaStandard<ElencoUrlFirmati> RichiediSignedURL(CoreWSRequest<CoreWS_Generic<RichiestaSignedUrl_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AuthDispatcher/AuthDispatcher.asmx/RichiediSignedURL", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ElencoUrlFirmati>>(result.ToString());
        }
        public RispostaStandard<ElencoUrlFirmati> RichiediAbacoURL(CoreWSRequest<CoreWS_Generic<RichiestaAbacoUrl_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AuthDispatcher/AuthDispatcher.asmx/RichiediAbacoURL", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ElencoUrlFirmati>>(result.ToString());
        }
        public RispostaStandard<ElencoUrlFirmati> RichiediSatUrl(CoreWSRequest<CoreWS_Generic<RichiestaSatUrl_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AuthDispatcher/AuthDispatcher.asmx/RichiediSatUrl", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ElencoUrlFirmati>>(result.ToString());
        }

        public RispostaStandard AutenticaGEE(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AuthDispatcher/AuthDispatcher.asmx/AutenticaGEE", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard AutenticaGoogleCloud(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AuthDispatcher/AuthDispatcher.asmx/AutenticaGoogleCloud", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        #endregion

        #region FunzioniGIS_Configurazione

        public RispostaStandard<CfgAlbero_CfgGisUtente> ConfiguraAlbero(CoreWSRequest<CoreWS_Generic<ImpostaConfigurazioneAlbero>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/ConfiguraAlbero", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<CfgAlbero_CfgGisUtente>>(result.ToString());
        }

        public RispostaStandard<ConfigurazioneGisUtente> CfgGIS_Leggi(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CfgGIS_Leggi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ConfigurazioneGisUtente>>(result.ToString());
        }
        public RispostaStandard<ConfigurazioneGisUtente> CfgGIS_Leggi_Default(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CfgGIS_Leggi_Default", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ConfigurazioneGisUtente>>(result.ToString());
        }

        public RispostaStandard CfgGIS_Salva(CoreWSRequest<CoreWS_Generic<ScriviConfigurazioneGisUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CfgGIS_Salva", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<ConfigurazioniGisGenerali> CfgGIS_Leggi_Generali(CoreWSRequest<CoreWS_Generic<LeggiConfigurazioniGeneraliGis>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CfgGIS_Leggi_Generali", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ConfigurazioniGisGenerali>>(result.ToString());
        }
        public RispostaStandard<Cfg_GestioneSistemaRif_Out> CfgGestioneSistemaDiRiferimentoPredefinito(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CfgGestioneSistemaDiRiferimentoPredefinito", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Cfg_GestioneSistemaRif_Out>>(result.ToString());
        }
        public RispostaStandard<LeggiPermessiLayerUtenti_Out> LeggiElencoPermessiLayerUtenti(CoreWSRequest<CoreWS_Generic<LeggiPermessiLayerUtenti_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiElencoPermessiLayerUtenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<LeggiPermessiLayerUtenti_Out>>(result.ToString());
        }
        public RispostaStandard<LeggiPermessiLayerUtenti_Out> PermessiUtenteSuSingoloLayer(CoreWSRequest<CoreWS_Generic<PermessiUtenteSuSingoloLayer_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/PermessiUtenteSuSingoloLayer", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<LeggiPermessiLayerUtenti_Out>>(result.ToString());
        }
        public RispostaStandard SalvaPermessiLayerUtenti(CoreWSRequest<CoreWS_Generic<SalvaPermessiLayerUtenti_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/SalvaPermessiLayerUtenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<LeggiPermessiLayerGruppiUtente_Out> LeggiElencoPermessiLayerGruppiUtente(CoreWSRequest<CoreWS_Generic<LeggiPermessiLayerGruppiUtente_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiElencoPermessiLayerGruppiUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<LeggiPermessiLayerGruppiUtente_Out>>(result.ToString());
        }
        public RispostaStandard SalvaPermessiLayerGruppiUtente(CoreWSRequest<CoreWS_Generic<SalvaPermessiLayerGruppiUtente_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/SalvaPermessiLayerGruppiUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard EliminazioneTotaleDatiLayer(CoreWSRequest<CoreWS_Generic<Int32>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/EliminazioneTotaleDatiLayer", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<EndpointMappeSatellitari> LeggiBaseUrlMappeSatellitari(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiBaseUrlMappeSatellitari", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<EndpointMappeSatellitari>>(result.ToString());
        }

        //public RispostaStandard TestElaborazioneAlgoritmo(CoreWSRequest<CoreWS_Generic<string>> request)
        //{
        //    JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/TestElaborazioneAlgoritmo", JsonConvert.SerializeObject(request));
        //    return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        //}

        public RispostaStandard OperazioniBookmark(CoreWSRequest<CoreWS_Generic<OperazioniBookmark_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/OperazioniBookmark", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<Bookmark>> LeggiBookmark(CoreWSRequest<CoreWS_Generic<Int32>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiBookmark", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Bookmark>>>(result.ToString());
        }

        public RispostaStandard CaricaPaletteSLD(CoreWSRequest<CoreWS_Generic<CaricaPaletteSLD_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/CaricaPaletteSLD", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<bool> VerificaEsistenzaPalette(CoreWSRequest<CoreWS_Generic<VerificaEsistenzaPalette_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/VerificaEsistenzaPalette", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }

        public RispostaStandard<string> getEnvelopeWKT(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/getEnvelopeWKT", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }

        public RispostaStandard<bool> IncludiEsludiImpresaDaControlloComplianceISCC(CoreWSRequest<CoreWS_Generic<IncludiEsludiImpresaISCC_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/IncludiEsludiImpresaDaControlloComplianceISCC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }

        public RispostaStandard<bool> SottomettiElaborazioneMassivaGISCheckList(CoreWSRequest<CoreWS_Generic<SottomettiElaborazioneMassivaGISCheckList_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/SottomettiElaborazioneMassivaGISCheckList", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }

        public RispostaStandard<bool> VerificaAziendaAbilitataISCC(CoreWSRequest<CoreWS_Generic<VerificaAziendaAbilitataISCC_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/VerificaAziendaAbilitataISCC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }

        public RispostaStandard<LeggiElencoElaborazioniMassive_Out> LeggiElencoElaborazioniMassiveGISCheckList(CoreWSRequest<CoreWS_Generic<LeggiElencoElaborazioniMassive_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiElencoElaborazioniMassiveGISCheckList", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<LeggiElencoElaborazioniMassive_Out>>(result.ToString());
        }
        
        public RispostaStandard<DateTime> GisLeggiDateImpiantiFiltroTemporale(CoreWS_Gis<LeggiDateImpiantiPerFiltroTemporale_In> request)
        {
            JObject result = Request(HttpMethod.Post, "/Gis/GisWS.asmx/LeggiDateImpiantiFiltroTemporale", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<DateTime>>(result.ToString());
        }

        #endregion

        public RispostaStandard<ParcoMacchine> LeggiMacchinaAnagrafica(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/Leggi_Macchina_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ParcoMacchine>>(result.ToString());
        }

        public RispostaStandard LeggiMacchineAnagrafica(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/Leggi_Macchine_Anagrafica_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard Leggi_Macchine_Per_Tipo_NG(CoreWSRequest<CoreWS_Generic<Leggi_Macchine_Per_Tipo>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/Leggi_Macchine_Per_Tipo_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard Leggi_Macchine_Irrigazione_Impianto_NG(CoreWSRequest<CoreWS_Generic<Leggi_Macchine_Irrigazione_Impianto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/Leggi_Macchine_Irrigazione_Impianto_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiMacchinaIrrigazioneDefault(CoreWSRequest<CoreWS_Generic<Leggi_Macchine_Irrigazione_Impianto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/LeggiMacchinaIrrigazioneDefault", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiImpiantoIrrigazione(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/LeggiImpiantoIrrigazione",JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ReadMachinesByClassCode(CoreWSRequest<CoreWS_Generic<InData.Anagrafica.MachinesXTypeReadParams>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/ReadMachinesByClassCode", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ScriviMacchina(CoreWSRequest<CoreWS_Generic<Scrivi_Macchina_Anagrafica>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/Scrivi_Macchina_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<bool> IsMacchinaMovimentata(CoreWSRequest<CoreWS_Generic<ParcoMacchine>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/IsMacchinaMovimentata", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }

        public RispostaStandard<bool> IsEditAllowed(CoreWSRequest<CoreWS_Generic<ParcoMacchine>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/IsEditAllowed", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }

        public RispostaStandard<List<int>> CentresOnWhichIsUsed(CoreWSRequest<CoreWS_Generic<ParcoMacchine>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/CentresOnWhichIsUsed", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<int>>>(result.ToString());
        }

        public RispostaStandard<List<string>> CompaniesByWichIsUsed(CoreWSRequest<CoreWS_Generic<ParcoMacchine>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/CompaniesByWichIsUsed", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<string>>>(result.ToString());
        }

        public RispostaStandard LeggiCatastoAnagrafica(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Catasto.asmx/Leggi_Catasto_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<CatastoCentroAziendale> LeggiCatastoAnagrafica(CoreWSRequest<CoreWS_Generic<CatastoCentroAziendale>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Catasto.asmx/Leggi_Particella_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<CatastoCentroAziendale>>(result.ToString());
        }

        public RispostaStandard ScriviCatasto(CoreWSRequest<CoreWS_Generic<ScriviCatasto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Catasto.asmx/Scrivi_Particella_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CheckPossessi(CoreWSRequest<CoreWS_Generic<CatastoCentroAziendale>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Catasto.asmx/CheckPossessi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Campo>> LeggiCampi(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiCampi>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Campi.asmx/LeggiCampi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Campo>>>(result.ToString());
        }

        public RispostaStandard LeggiCampiAnagrafica(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Campi.asmx/Leggi_Campi_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<Campo> LeggiCampoAnagrafica(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Campi.asmx/Leggi_Campo_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Campo>>(result.ToString());
        }

        public RispostaStandard LeggiAppezzamentiCampo(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Campi.asmx/Leggi_AppezzamentiCampo_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ScriviCampiAnagrafica(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Anagrafica.ScriviCampiAnagrafica>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Campi.asmx/ScriviCampiAnagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ScriviCampiAnagraficaInLine(CoreWSRequest<CoreWS_Generic<ScriviCampiAnagraficaInLine>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Campi.asmx/ScriviCampiAnagrafica_inLine", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CaricaGridImpianti(CoreWSRequest<CoreWS_Generic<LeggiImpianto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/CaricaGridImpianti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<UtilizzoTerreno>> LeggiSpecieVegetaliAttiveImpianti(CoreWSRequest<CoreWS_Generic<LeggiImpianto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/Leggi_SpecieVegetali_Attive_Impianti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<UtilizzoTerreno>>>(result.ToString());
        }

        public RispostaStandard LeggiAppezzamentiAnagrafica(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/Leggi_Appezzamenti_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento> LeggiAppezzamentoGlobalAnagrafica(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/Leggi_Appezzamento_Global_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento>>(result.ToString());
        }

        public RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento> LeggiAppezzamentoAnagrafica(CoreWSRequest<CoreWS_Generic<LeggiAppezzamento>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/Leggi_Appezzamento_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento>>(result.ToString());
        }

        public RispostaStandard<UtilizzoTerreno> LeggiAppezzamentoUtilizzoTerreno(CoreWSRequest<CoreWS_Generic<AgronicaCoreModelsSTD.anagrafiche.Impianto.PK>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/Leggi_Appezzamento_UtilizzoTerreno", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<UtilizzoTerreno>>(result.ToString());
        }

        public RispostaStandard CaricaDatiCatastali_NG(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Anagrafica.CaricaDatiCatastali>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/CaricaDatiCatastali_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiParticellePerAppezzamento(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/LeggiParticelle_Per_Appezzamento", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ControllaValiditaAppezzamenti(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/Controlla_Validita_Appezzamenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento> ScriviAppezzamentoAnagrafica(CoreWSRequest<CoreWS_Generic<Appezzamento>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/Scrivi_Appezzamento_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Appezzamento>>(result.ToString());
        }

        public RispostaStandard<Appezzamento> ScriviAppezzamentoAnagraficaFiltroTemporale(CoreWSRequest<CoreWS_Generic<AppezzamentoFiltroTemporale>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/Scrivi_Appezzamento_Anagrafica_Filtro_Temporale", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Appezzamento>>(result.ToString());
        }

        public RispostaStandard<string> BloccaSbloccaAppezzamenti(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Anagrafica.BloccaSbloccaAppezzamenti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/BloccaSblocca_Appezzamenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }

        public RispostaStandard VerificaSuperficie(CoreWSRequest<CoreWS_Generic<AgronicaCoreModelsSTD.anagrafiche.Impianto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/Verifica_Superficie", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<AgronicaCoreDTOStd.InData.Agenda.PresenzaMovimentazioni> VerificaOperazioniAgenda(CoreWSRequest<CoreWS_Generic<AgronicaCoreModelsSTD.anagrafiche.Impianto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/Verifica_OperazioniAgenda", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreDTOStd.InData.Agenda.PresenzaMovimentazioni>>(result.ToString());
        }

        public RispostaStandard Leggi_Esercizi_Anagrafica(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/Leggi_Esercizi_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<AgronicaCoreDTOStd.InData.Data> LeggiMaxDataModifica(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.LeggiFiltro>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/Leggi_Max_DataModifica_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreDTOStd.InData.Data>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Contatto>> CaricaComboCmbOrganismoReferenteModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Anagrafica.FiltroImpresa>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/CaricaComboCmb_OrganismoReferente_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Contatto>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Contatto>> CaricaComboCmbRiferimentoTrasferimentoDati_Modello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Anagrafica.FiltroImpresa>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/CaricaComboCmb_Riferimento_Trasferimento_Dati_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Contatto>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Fabbricato>> CaricaComboCmbMagazzinoConferimentoModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Anagrafica.FiltroImpresa>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/CaricaComboCmb_MagazzinoConferimento_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Fabbricato>>>(result.ToString());
        }

        public RispostaStandard CaricaComboCmbTecnici(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.LeggiFiltro>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/CaricaComboCmb_Tecnici_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Contatto>> CaricaComboCmbTecniciModello(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/CaricaComboCmb_Tecnici_Modello_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Contatto>>>(result.ToString());
        }

        public RispostaStandard CaricaComboCmbOrganismiODC(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/CaricaComboCmb_OrganismiODC_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.RisorseUmane>> CaricaComboCmbOrganismiODCModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.LeggiFiltro>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/CaricaComboCmb_OrganismiODC_Modello_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.RisorseUmane>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Copertura>> CaricaComboCoperturaModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiFormaAllevamento>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Copertura.asmx/CaricaComboCopertura_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Copertura>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.ImpegniAggiuntiviFacoltativi>> CaricaComboIAFModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiIAF>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreDPI/DPI.asmx/CaricaComboIAF_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.ImpegniAggiuntiviFacoltativi>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.attivita.Lavorazione>> CaricaComboLavorazioniModel(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiOperazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Operazioni.asmx/CaricaComboLavorazioni_Model", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.attivita.Lavorazione>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.DensitaImpianto.Portinnesto>> CaricaComboPortinnestiModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiPortinnesto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Portinnesti.asmx/CaricaComboPortinnesti_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.DensitaImpianto.Portinnesto>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Regolamenti>> CaricaComboRegolamentiModello(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Regolamenti.asmx/CaricaComboRegolamenti_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Regolamenti>>>(result.ToString());
        }

        public RispostaStandard CaricaComboSpecieVegetaliJoinGruppoVegetale(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/SpecieVegetali.asmx/CaricaComboSpecieVegetali_Join_GruppoVegetale", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CaricaClassiSpecieSementieri(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/SpecieVegetali.asmx/LeggiClassiSpecieSementieri", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CaricaClassiSpecieXUtentiSementieri(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/SpecieVegetali.asmx/LeggiClassiSpecieXUtentiSementieri", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard SalvaClassiSpecieXUtentiSementieri(CoreWSRequest<CoreWS_Generic<ClassiSpeciePerUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/SpecieVegetali.asmx/SalvaClassiSpecieXUtentiSementieri", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<BaseCodeDescr>> LeggiGruppoVegetale(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/SpecieVegetali.asmx/LeggiGruppoVegetale", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<BaseCodeDescr>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe>> LeggiDropDownAppezzamentiCodici(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/Leggi_DropDown_Appezzamenti_Codici", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe>>>(result.ToString());
        }


        public RispostaStandard LeggiParticellePerCentro(CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Campi.asmx/LeggiParticellePerCentro", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }


        public RispostaStandard<String> LeggiInvestimentoCatastale(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiInvestimentoCatastale>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Catasto.asmx/Leggi_Investimento_Catastale", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<String>>(result.ToString());
        }


        public RispostaStandard<String> LeggiInvestimentoCatastaleCampo(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiInvestimentoCatastaleCampo>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Catasto.asmx/Leggi_Investimento_Catastale_Campo", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<String>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.CentroAziendale>> LeggiCentriAziendaliModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiCentriAziendali>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/CentroAziendale.asmx/LeggiCentriAziendaliModello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.CentroAziendale>>>(result.ToString());
        }

        public RispostaStandard LeggiContattiAnagrafica1(CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/Leggi_Contatti_Anagrafica1", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard Leggi_Contatti_Anagrafica_NG(CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/Leggi_Contatti_Anagrafica_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }


        public RispostaStandard<List<Fabbricato>> LeggiMagazziniQdC(CoreWSRequest<CoreWS_Generic<InData.Anagrafica.LeggiMagazzini_QdC>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Fabbricati.asmx/Leggi_Magazzini_QdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Fabbricato>>>(result.ToString());
        }

        public RispostaStandard ControlloPresenzaPiva(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Controllo_Presenza_Piva", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<Boolean> ImpresaBiologica(CoreWSRequest<CoreWS_Generic<AgronicaCoreModelsSTD.anagrafiche.Impresa>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/ImpresaBiologica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Boolean>>(result.ToString());
        }

        public RispostaStandard LeggiImpreseCodici(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Leggi_Imprese_Codici_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Impresa>> LeggiImpreseFiltro(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.LeggiFiltro>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Leggi_Imprese_Filtro_StringaRicerca", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Impresa>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Impresa>> LeggiImpresePadri(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Anagrafica.Leggi_Padri>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Leggi_Imprese_Padri", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Impresa>>>(result.ToString());
        }

        public RispostaStandard<List<BaseCodeDescrStr>> LeggiImpresePadriBaseCodeDescr(CoreWSRequest<CoreWS_Generic<Leggi_Padri>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Leggi_Imprese_Padri_BaseCodeDescrStr", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<BaseCodeDescrStr>>>(result.ToString());
        }

        public RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Impresa> LeggiImpresaPadreSementieri(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Anagrafica.Leggi_Padri>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Leggi_Impresa_Padre_Sementieri", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreModelsSTD.anagrafiche.Impresa>>(result.ToString());
        }

        #region DatiPrevisionaliColture

        public RispostaStandard<DatiPrevisionaliColture> readDatiPrevisionaliColture(CoreWSRequest<CoreWS_Generic<DatiPrevisionaliColtureRequest>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/DatiPrevisionaliColture.asmx/readDatiPrevisionaliColture", JsonConvert.SerializeObject(request));
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<DatiPrevisionaliColture>>(result.ToString());
            var rStringa = risposta.RispostaStringa;
            return new RispostaStandard<DatiPrevisionaliColture>()
            {
                RispostaOK = risposta.RispostaOK,
                RispostaStringa = rStringa,
                Errore = risposta.Errore
            };
        }

        public RispostaStandard readDatiPrevisionaliColtureDT(CoreWSRequest<CoreWS_Generic<DatiPrevisionaliColtureRequest>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/DatiPrevisionaliColture.asmx/readDatiPrevisionaliColtureDT", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard editDatiPrevisionaliColture(CoreWSRequest<CoreWS_Generic<DatiPrevisionaliColtureComplete>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/DatiPrevisionaliColture.asmx/editDatiPrevisionaliColture", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard createDatiPrevisionaliColture(CoreWSRequest<CoreWS_Generic<DatiPrevisionaliColtureComplete>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/DatiPrevisionaliColture.asmx/createDatiPrevisionaliColture", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard deleteDatiPrevisionaliColture(CoreWSRequest<CoreWS_Generic<DatiPrevisionaliColtureComplete>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/DatiPrevisionaliColture.asmx/deleteDatiPrevisionaliColture", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        #endregion

        public RispostaStandard<List<AgronicaCoreModelsSTD.attivita.dettagli.DettaglioFertilizzazione>> LeggiFertilizzantiQdC(CoreWSRequest<CoreWS_Generic<InData.Anagrafica.LeggiProdotti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/Leggi_Fertilizzanti_QdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.attivita.dettagli.DettaglioFertilizzazione>>>(result.ToString());
        }

        public RispostaStandard<Object> LeggiFormulatiQdC(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/Leggi_Formulati_QdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Object>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.attivita.dettagli.DettaglioTrattamento>> LeggiInsettiUtiliQdC(CoreWSRequest<CoreWS_Generic<InData.Anagrafica.LeggiProdotti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/Leggi_InsettiUtilili_QdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.attivita.dettagli.DettaglioTrattamento>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRegistroSomministrazioni>> LeggiGiacenzaFarmaci(CoreWSRequest<CoreWS_Generic<InData.Zoo.LeggiGiacenzaFarmaci>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/Leggi_Giacenze_Farmaci", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRegistroSomministrazioni>>>(result.ToString());
        }

        public RispostaStandard<List<MovimentoDiMagazzino>> LeggiProdottiDaTrattareQdC(CoreWSRequest<CoreWS_Generic<InData.Anagrafica.LeggiProdotti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/Leggi_ProdottiDaTrattare_QdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<MovimentoDiMagazzino>>>(result.ToString());
        }

        public RispostaStandard<List<UtilizzoTerreno>> LeggiSpecieVegetaliQdC(CoreWSRequest<CoreWS_Generic<InData.Agenda.LeggiSpecieQdC>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiSpecieVegetaliQdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<UtilizzoTerreno>>>(result.ToString());
        }

        public RispostaStandard<List<Consiglio_Irrigazione>> LeggiConsigliIrrigazione(CoreWSRequest<CoreWS_Generic<LeggiConsigliIrrigazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiConsigliIrrigazione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Consiglio_Irrigazione>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.avversita.AvversitaGruppo>> Leggi_AvversitaInsettiUtili_QdC(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiAvversita>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Avversita.asmx/Leggi_AvversitaInsettiUtili_QdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.avversita.AvversitaGruppo>>>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.attivita.risorse.RisorsaProdotto>> LeggiProdottiQdC(CoreWSRequest<CoreWS_Generic<InData.Anagrafica.LeggiProdotti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/Leggi_Prodotti_QdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.attivita.risorse.RisorsaProdotto>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.attivita.dettagli.DettaglioSemina>> LeggiSementiQdC(CoreWSRequest<CoreWS_Generic<InData.Anagrafica.LeggiProdotti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/Leggi_Sementi_QdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.attivita.dettagli.DettaglioSemina>>>(result.ToString());
        }


        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe>> LeggiDropDownEserciziCodici(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/Leggi_DropDown_Esercizi_Codici", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe>>>(result.ToString());
        }

        public RispostaStandard<Object> CaricaCentriImpresa(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.CaricaCentriImpresa>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/CentroAziendale.asmx/Carica_Centri_Impresa_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Object>>(result.ToString());
        }


        public RispostaStandard LeggiFabbricatiOmni(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Anagrafica.LeggiFabbricatiOmni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Fabbricati.asmx/LeggiFabbricatiOmni_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRaccolta>> LeggiTrasformatiVegetaliQdC(CoreWSRequest<CoreWS_Generic<InData.Anagrafica.LeggiProdotti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/Leggi_TrasformatiVegetali_QdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRaccolta>>>(result.ToString());
        }

        public RispostaStandard<AgronicaCoreModelsSTD.metaschema.ApportoMacroelementi> CalcoloNPKModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.FiltroCalcoloNPK>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreDPI/PianoConcimazione.asmx/CalcoloNPK_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreModelsSTD.metaschema.ApportoMacroelementi>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.FinalitaPianoConcimazione>> PCFinalitaRerWSModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.FiltroPC_Finalita_Rer>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreDPI/PianoConcimazione.asmx/PC_Finalita_Rer_WS_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.FinalitaPianoConcimazione>>>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Disciplinare>> Leggi_Disciplinari_Testata_conRegolamentoConcimazione(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreDPI/DPI.asmx/Leggi_Disciplinari_Testata_conRegolamentoConcimazione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Disciplinare>>>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Disciplinare>> Leggi_Disciplinari_Testata_DirettivaNitrati(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreDPI/DPI.asmx/Leggi_Disciplinari_Testata_DirettivaNitrati", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Disciplinare>>>(result.ToString());
        }

        #region profilazione
        public RispostaStandard SalvaUtenti_NG(CoreWSRequest<CoreWS_Generic<ScriviUtenti>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_W.asmx/SalvaUtenti_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard DisattivaUtenti_NG(CoreWSRequest<CoreWS_Generic<string[]>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_W.asmx/DisattivaUtenti_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ImportaUtentiDaExcel(CoreWSRequest<CoreWS_Generic<FileWrapperAlt>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/ImportazioneUtenti.asmx/ImportaUtentiDaExcel", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<TipologiaUtente>> ListaTipologie(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Tipologie.asmx/ListaTipologie", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<TipologiaUtente>>>(result.ToString());
        }
        public RispostaStandard ScriviTipologie_NG(CoreWSRequest<CoreWS_Generic<TipologiaUtente[]>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Tipologie.asmx/ScriviTipologie_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CopiaTipologia_NG(CoreWSRequest<CoreWS_Generic<CopyProfileObj>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Tipologie.asmx/CopiaTipologia_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ListaTipologiexPermessi_NG(CoreWSRequest<CoreWS_Generic<Boolean>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Tipologie.asmx/ListaTipologiexPermessi_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard TipologiaHaImpostazioniPermessiCollegati(CoreWSRequest<CoreWS_Generic<TipologiaUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Tipologie.asmx/HaImpostazioniPermessiCollegati", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard TipologiaLeggiUtentiCollegati(CoreWSRequest<CoreWS_Generic<TipologiaUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Tipologie.asmx/LeggiUtentiCollegati", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CancellaTipologia_NG(CoreWSRequest<CoreWS_Generic<TipologiaUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Tipologie.asmx/CancellaTipologia_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard AssegnaPermesso_NG(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Tipologie.asmx/AssegnaPermesso_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Carica_Gerarchia_Permessi_NG(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Permessi_R.asmx/Carica_Gerarchia_Permessi_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Carica_Gerarchia_Permessi_Attivi(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Permessi_R.asmx/Carica_Gerarchia_Permessi_Attivi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard AggiornaCliente_Permessi(CoreWSRequest<CoreWS_Generic<Cliente_PermessiScrivi>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Permessi_W.asmx/AggiornaCliente_Permessi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<Cliente_Permesso>> LeggiCliente_Permessi(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Permessi_R.asmx/LeggiCliente_Permessi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Cliente_Permesso>>>(result.ToString());
        }
        public RispostaStandard<Boolean> ControllaEsistenzaTabellaCliente_Permessi(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Permessi_R.asmx/ControllaEsistenzaTabellaCliente_Permessi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Boolean>>(result.ToString());
        }

        public RispostaStandard LeggiImpostazioneImpresaCentro(CoreWSRequest<CoreWS_Generic<LeggiValoriImpostazioni_AziendeCentri>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Imprese_Impostazioni_R.asmx/LeggiImpostazioneImpresaCentro", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiSezioniImpostazioni_AziendeCentri(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Imprese_Impostazioni_R.asmx/LeggiSezioniImpostazioni_AziendeCentri", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard SalvaImpostazioni_AziendeCentri_NG(CoreWSRequest<CoreWS_Generic<SalvaImpostazioni_AziendeCentri>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Imprese_Impostazioni_W.asmx/SalvaImpostazioni_AziendeCentri_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CopiaImpostazioniAziende(CoreWSRequest<CoreWS_Generic<CopiaImpostazioniObj>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Imprese_Impostazioni_W.asmx/CopiaImpostazioniAziende", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CancellaImpostazioni_AziendeCentri_NG(CoreWSRequest<CoreWS_Generic<Imprese_Impostazioni[]>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Imprese_Impostazioni_W.asmx/CancellaImpostazioni_AziendeCentri_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CancellaImpostazione_AziendeCentri_NG(CoreWSRequest<CoreWS_Generic<CancellaImpostazione_AziendeCentri>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Imprese_Impostazioni_W.asmx/CancellaImpostazione_AziendeCentri_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiValoriImpostazioni_AziendeCentri_NG(CoreWSRequest<CoreWS_Generic<LeggiValoriImpostazioni_AziendeCentri>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Imprese_Impostazioni_R.asmx/LeggiValoriImpostazioni_AziendeCentri_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        #endregion

        public RispostaStandard LeggiGruppiMerceAnagrafica_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_R.asmx/LeggiGruppiMerceAnagrafica_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard AggiungiOModificaGruppoMerce_NG(CoreWSRequest<CoreWS_Generic<GruppoMerceDto>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_R.asmx/AggiungiOModificaGruppoMerce_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard TentativoCancellazioneGruppoMerce_NG(CoreWSRequest<CoreWS_Generic<TentativoCancellazioneGruppoMerce>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_R.asmx/TentativoCancellazioneGruppoMerce_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiGruppiUtenti(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_R.asmx/LeggiGruppiUtenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiGruppiMerce_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_R.asmx/LeggiGruppiMerce_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ScriviGruppiUtentiPerGruppiMerce_NG(CoreWSRequest<CoreWS_Generic<ScriviGruppiUtentiPerGruppiMerce>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_R.asmx/ScriviGruppiUtentiPerGruppiMerce_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        #region "Dati rete Acqua"
        public RispostaStandard LeggiTipologiaDispositivi(CoreWSRequest<CoreWS_Generic<TipologiaDispositivi>> request)
        {
            JObject result = Request(HttpMethod.Post, "/IOT/DatiReteAcqua.asmx/LeggiTipologiaDispositivi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiDispositiviXSorgente(CoreWSRequest<CoreWS_Generic<DispositiviXSorgente>> request)
        {
            JObject result = Request(HttpMethod.Post, "/IOT/DatiReteAcqua.asmx/LeggiDispositiviXSorgente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiAnagraficaDispositivi(CoreWSRequest<CoreWS_Generic<LeggiAnagraficaDispositivi>> request)
        {
            JObject result = Request(HttpMethod.Post, "/IOT/DatiReteAcqua.asmx/LeggiAnagraficaDispositivi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<RisultatoIOT> DatiIOTElabora(CoreWSRequest<CoreWS_Generic<RichiediDatiIOT>> request)
        {
            JObject result = Request(HttpMethod.Post, "/IOT/DatiReteAcqua.asmx/DatiIOTElabora", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<RisultatoIOT>>(result.ToString());
        }
        #endregion
        public RispostaStandard Get_OperazioniPreferite(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/Get_OperazioniPreferite", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Get_OperazioniPreferite_PerRicette(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtentiBIZ/Utenti_Impostazioni_R.asmx/Get_OperazioniPreferite_PerRicette", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiSpecieColtivate_NG(CoreWSRequest<CoreWS_Generic<LeggiSpecieColtivate>> request)
        {
            JObject result = Request(HttpMethod.Post, "AgronicaCoreUtility/CaricaListControl.asmx/LeggiSpecieColtivate_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ControllaSogliaAvversitaSoddisfattaQdC(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Avversita.asmx/Controlla_Soglia_Avversita_Soddisfatta_QdC_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<Object> LeggiAvversitaQdC(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Avversita.asmx/Leggi_Avversita_QdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Object>>(result.ToString());
        }


        public RispostaStandard<Object> LeggiSoglieAvversitaQdC(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Avversita.asmx/Leggi_SoglieAvversita_QdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Object>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescr>> LeggiBIODatiOrientamentoProduttivo(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Biologico.asmx/Leggi_BIO_Dati_OrientamentoProduttivo", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescr>>>(result.ToString());
        }

        public RispostaStandard LeggiCategorieMagazzinoImpostazioni(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiCategorieMagazzinoImpostazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Categorie_Magazzino.asmx/Leggi_Categorie_Magazzino_Impostazioni_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }


        public RispostaStandard LeggiGenericoCategorieMagazzino(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiGenericoCategorieMagazzino>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Categorie_Magazzino.asmx/Leggi_Generico_Categorie_Magazzino_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }


        public RispostaStandard<Object> LeggiUnitaMisuraCategoria(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiUnitaMisuraCategoria>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/CategoriexUnitaMisura.asmx/Leggi_UnitaMisura_Categoria_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Object>>(result.ToString());
        }


        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneSuFila>> CaricaTecnicaConduzioneSuFilaModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiConduzione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Conduzione.asmx/CaricaTecnicaConduzioneSuFila_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneSuFila>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneTraFila>> CaricaTecnicaConduzioneTraFilaModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiConduzione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Conduzione.asmx/CaricaTecnicaConduzioneTraFila_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneTraFila>>>(result.ToString());
        }

        public RispostaStandard<Object> LeggiDosiEtichettaQdC(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/DosiEtichetta.asmx/Leggi_DosiEtichetta_QdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Object>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Epoca>> LeggiEpocheDPIQdC(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiEpoche>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Epoche.asmx/Leggi_EpocheDPI_QdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Epoca>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Epoca>> LeggiEpocheFertilizzazioneQdC(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiEpoche>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Epoche.asmx/Leggi_EpocheFertilizzazione_QdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Epoca>>>(result.ToString());
        }

        public RispostaStandard<Decimal> LeggiEfficienza(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiEfficienza>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Fertilizzazione.asmx/LeggiEfficienza", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Decimal>>(result.ToString());
        }

        public RispostaStandard<Decimal> LeggiEfficienzaPUA2007(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiEfficienza>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Fertilizzazione.asmx/LeggiEfficienza_PUA_2007", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Decimal>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.DensitaImpianto.FormaAllevamento>> CaricaFormeAllevamentoModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiFormaAllevamento>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/FormeAllevamento.asmx/CaricaFormeAllevamento_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.DensitaImpianto.FormaAllevamento>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.FormeGiuridiche>> LeggiModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiModello>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/FormeGiuridiche.asmx/Leggi_Modello_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.FormeGiuridiche>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.FaseCicloColturale>> LeggiFaseCicloColturale(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.FiltroFinalita2>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/GruppoFinalita.asmx/LeggiFaseCicloColturale", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.FaseCicloColturale>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoVarietale>> LeggiGruppoVarietale(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiGruppoVarietale>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/GruppoVarietale.asmx/Leggi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoVarietale>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Irrigazione>> CaricaImpiantiIrrigazioniModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiPortinnesto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpiantiIrrigazioni.asmx/CaricaImpiantiIrrigazioni_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Irrigazione>>>(result.ToString());
        }

        public RispostaStandard CaricaAreeGIAS(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpostazioniUtente.asmx/Carica_AreeGIAS_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }


        public RispostaStandard CaricaDatiDDL(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.CaricaDatiDDL>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpostazioniUtente.asmx/Carica_DatiDDL_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard SalvaImpostazioniUtente(CoreWSRequest<CoreWS_Generic<AgronicaCoreModelsSTD.profilazione.Impostazione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpostazioniUtente.asmx/Salva_ImpostazioniUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard GetCAP(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.GetCAP>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ISTAT.asmx/GetCAP_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard GetComuni(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.GetComuni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ISTAT.asmx/GetComuni_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard GetProvincie(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ISTAT.asmx/GetProvincie", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Regione>> readRegioni(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ISTAT.asmx/readRegioni", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Regione>>>(result.ToString());
        }

        public RispostaStandard<AgronicaCoreModelsSTD.metaschema.Regione> readRegionsByProvince(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ISTAT.asmx/readRegionsByProvince", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreModelsSTD.metaschema.Regione>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166>> GetStatiModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.GetStatiModello>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ISTAT.asmx/GetStati_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166>>>(result.ToString());
        }

        public RispostaStandard<Object> LeggiLocalizzazioniModello(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Localizzazioni.asmx/LeggiLocalizzazioni_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Object>>(result.ToString());
        }

        public RispostaStandard LeggiDitte(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Macchine.asmx/Leggi_Ditte_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiTipo(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiTipo>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Macchine.asmx/Leggi_Tipo_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiTutteMacchine(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Macchine.asmx/Leggi_TutteMacchine_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Macrouso>> LeggiMacrousi(CoreWSRequest<CoreWS_Generic<AgronicaCoreModelsSTD.metaschema.Macrouso>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Macrousi.asmx/Leggi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.Macrouso>>>(result.ToString());
        }


        public RispostaStandard<List<AgronicaCoreModelsSTD.attivita.Lavorazione>> LeggiOperazioniModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiOperazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Operazioni.asmx/Leggi_Operazioni_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.attivita.Lavorazione>>>(result.ToString());
        }

        public RispostaStandard SalvaOperazioniPreferite(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.SalvaOperazioniPreferite>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Operazioni.asmx/SalvaOperazioniPreferite", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.RegolamentoConcimazione>> LeggiPUARegolamentixEditImpiantoModello(CoreWSRequest<CoreWS_Generic<AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/PUA_Regolamenti.asmx/Leggi_PUA_RegolamentixEditImpianto_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.RegolamentoConcimazione>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescr>> LeggiQualitaCatasto(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/QualitaCatasto.asmx/Leggi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescr>>>(result.ToString());
        }
        public RispostaStandard<Object> LeggiUnitaDiMisuraQdC(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/UnitaDiMisura.asmx/Leggi_UnitaDiMisura_QdC_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Object>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.RapportoContabile>> LeggiDropdownContattiImprese(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Rapporti_Contabili.asmx/LeggiDropdownContattiImprese", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.RapportoContabile>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.utilizzi.Specie>> LeggiSpecieVegetali(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiSpecie>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/SpecieVegetali.asmx/Leggi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.utilizzi.Specie>>>(result.ToString());
        }

        public RispostaStandard<AgronicaCoreModelsSTD.metaschema.UnitaDiMisura> RecuperaUdMdaFrCod(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/UnitaDiMisura.asmx/Recupera_UdM_da_FrCod", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreModelsSTD.metaschema.UnitaDiMisura>>(result.ToString());
        }
        public RispostaStandard CaricaComboLavorazioni_ConFiltro_NG(CoreWSRequest<CoreWS_Generic<CaricaComboLavorazioni_ConFiltro>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Operazioni.asmx/CaricaComboLavorazioni_ConFiltro_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaComboLavorazioni_ConFiltro_PerBrogliaccio_NG(CoreWSRequest<CoreWS_Generic<CaricaComboLavorazioni_ConFiltro>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Operazioni.asmx/CaricaComboLavorazioni_ConFiltro_PerBrogliaccio_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescr>> RecuperaUdMdaFrCod(CoreWSRequest<CoreWS_Generic<AgronicaCoreModelsSTD.baseClass.BaseCodeDescr>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Zone.asmx/Leggi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.baseClass.BaseCodeDescr>>>(result.ToString());
        }

        public RispostaStandard CaricaComboLavorazioni_NG(CoreWSRequest<CoreWS_Generic<CaricaComboLavorazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Operazioni.asmx/CaricaComboLavorazioni_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CaricaComboLavorazioni_ConFiltro_PerRicette_NG(CoreWSRequest<CoreWS_Generic<CaricaComboLavorazioni_ConFiltro>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Operazioni.asmx/CaricaComboLavorazioni_ConFiltro_PerRicette_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaComboLavorazioni_PerBrogliaccio_NG(CoreWSRequest<CoreWS_Generic<CaricaComboLavorazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Operazioni.asmx/CaricaComboLavorazioni_PerBrogliaccio_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaComboLavorazioni_PerRicette_NG(CoreWSRequest<CoreWS_Generic<CaricaComboLavorazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Operazioni.asmx/CaricaComboLavorazioni_PerRicette_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.attivita.Lavorazione>> changeVisualizzaOperazioni(CoreWSRequest<CoreWS_Generic<LeggiOperazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Operazioni.asmx/changeVisualizzaOperazioni", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.attivita.Lavorazione>>>(result.ToString());
        }
        public RispostaStandard Leggi_GruppoOperazioni_NG(CoreWSRequest<CoreWS_Generic<Leggi_GruppoOperazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Operazioni.asmx/Leggi_GruppoOperazioni_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Leggi_Operazioni_APP(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Operazioni.asmx/Leggi_Operazioni_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Leggi_TabellaCostoUnitario_NG(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Macchine.asmx/Leggi_TabellaCostoUnitario_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CaricaComboCultivar_conFiltroUtente_NG(CoreWSRequest<CoreWS_Generic<CaricaComboCultivar_conFiltroUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Cultivar.asmx/CaricaComboCultivar_conFiltroUtente_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard GetVarietaAGEA_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Cultivar.asmx/GetVarietaAGEA_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta>> Leggi(CoreWSRequest<CoreWS_Generic<LeggiCultivar>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Cultivar.asmx/Leggi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta>>>(result.ToString());
        }

        public RispostaStandard CaricaComboFinalita_NG(CoreWSRequest<CoreWS_Generic<CaricaComboFinalita>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/GruppoFinalita.asmx/CaricaComboFinalita_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaComboFinalita2_NG(CoreWSRequest<CoreWS_Generic<CaricaComboFinalita2>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/GruppoFinalita.asmx/CaricaComboFinalita2_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard GetStati(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ISTAT.asmx/GetStati", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Leggi_PUA_Regolamenti_NG(CoreWSRequest<CoreWS_Generic<Leggi_PUA_Regolamenti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/PUA_Regolamenti.asmx/Leggi_PUA_Regolamenti_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Leggi_PUA_RegolamentixEditImpianto_NG(CoreWSRequest<CoreWS_Generic<Leggi_PUA_Regolamenti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/PUA_Regolamenti.asmx/Leggi_PUA_RegolamentixEditImpianto_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Leggi_PUA_RegolamentixImpostazioniNitrati(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/PUA_Regolamenti.asmx/Leggi_PUA_RegolamentixImpostazioniNitrati", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        #region "Funzioni SmartTractors-HubIoT"
        public RispostaStandard InviaRicetta(CoreWSRequest<CoreWS_Generic<List<RicettaOperazione2WorkOrderKey>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/SmartTractor_HubIoT.asmx/InviaRicetta", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<ParametriConnessioni_Out> LeggiParametriConnessioni(CoreWSRequest<CoreWS_Generic<List<ParametriConnessioni_In>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/SmartTractor_HubIoT.asmx/LeggiParametriConnessioni", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ParametriConnessioni_Out>>(result.ToString());
        }

        public RispostaStandard SalvaParametriConnessioni(CoreWSRequest<CoreWS_Generic<SalvaParametriConnessioni_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/SmartTractor_HubIoT.asmx/SalvaParametriConnessioni", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        #endregion

        public RispostaStandard GetImputazioni(CoreWS_Imputazione_Fasi request)
        {
            JObject result = Request(HttpMethod.Post, "/Contab/Imputazioni_Fasi.asmx/Leggi_Imputazioni_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard GetTipiImputazioni(CoreWS_Imputazione_Fasi request)
        {
            JObject result = Request(HttpMethod.Post, "/Contab/Imputazioni_Fasi.asmx/Leggi_TipiImputazioni_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard GetRapportiContab(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Rapporti_Contabili.asmx/GetRapportiContab", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard GetRapportiContabCodDescr(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Rapporti_Contabili.asmx/GetRapportiContabCodDescr", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ModificaElementoContattiImprese(CoreWSRequest<CoreWS_Generic<RapportoContabile>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Rapporti_Contabili.asmx/ModificaElementoContattiImprese", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard RimuoviElementoContattiImprese_NG(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Rapporti_Contabili.asmx/RimuoviElementoContattiImprese_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaComboCodici_Terreno_NG(CoreWSRequest<CoreWS_Generic<CaricaComboCodici_Terreno>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/SpecieVegetali.asmx/CaricaComboCodici_Terreno_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaComboSpecieVegetali_conFiltroUtente_NG(CoreWSRequest<CoreWS_Generic<CaricaComboSpecieVegetali_conFiltroUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/SpecieVegetali.asmx/CaricaComboSpecieVegetali_conFiltroUtente_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaComboSpecieVegetaliDestinazioneUso_conFiltroUtente_NG(CoreWSRequest<CoreWS_Generic<CaricaComboSpecieVegetali_conFiltroUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/SpecieVegetali.asmx/CaricaComboSpecieVegetaliDestinazioneUso_conFiltroUtente_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard GetUtilizzo_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/SpecieVegetali.asmx/GetUtilizzo_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<AgronicaCoreModelsSTD.metaschema.utilizzi.Specie> Leggi_Da_Cultivar(CoreWSRequest<CoreWS_Generic<AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/SpecieVegetali.asmx/Leggi_Da_Cultivar", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreModelsSTD.metaschema.utilizzi.Specie>>(result.ToString());
        }
        public RispostaStandard CaricaComboCopertura_NG(CoreWSRequest<CoreWS_Generic<CaricaComboCopertura>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Copertura.asmx/CaricaComboCopertura_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaComboRegolamenti_NuovoImpianto(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Regolamenti.asmx/CaricaComboRegolamenti_NuovoImpianto", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaComboRegolamenti_NG(CoreWSRequest<CoreWS_Generic<CaricaComboRegolamenti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Regolamenti.asmx/CaricaComboRegolamenti_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CategorieMagazzino_NG(CoreWSRequest<CoreWS_Generic<CategorieMagazzino>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Categorie_Magazzino.asmx/CategorieMagazzino_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Leggi_Categorie_Magazzino_NG(CoreWSRequest<CoreWS_Generic<Leggi_Categorie_Magazzino>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Categorie_Magazzino.asmx/Leggi_Categorie_Magazzino_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaFormeAllevamento_NG(CoreWSRequest<CoreWS_Generic<CaricaFormeAllevamento>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/FormeAllevamento.asmx/CaricaFormeAllevamento_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard FormeGiuridiche_Leggi_NG(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/FormeGiuridiche.asmx/Leggi_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaComboGruppoVarietale_NG(CoreWSRequest<CoreWS_Generic<CaricaComboGruppoVarietale>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/GruppoVarietale.asmx/CaricaComboGruppoVarietale_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CaricaImpiantiIrrigazioni_NG(CoreWSRequest<CoreWS_Generic<CaricaImpiantiIrrigazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpiantiIrrigazioni.asmx/CaricaImpiantiIrrigazioni_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Leggi_ImpiantiIrrigazioni_APP(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpiantiIrrigazioni.asmx/Leggi_ImpiantiIrrigazioni_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Carica_ImpostazioniTemplate(CoreWSRequest<CoreWS_Generic<Leggi_Impostazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpostazioniUtente.asmx/Carica_ImpostazioniTemplate", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Carica_ImpostazioniUtente_Sezioni(CoreWSRequest<CoreWS_Generic<Leggi_Impostazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/ImpostazioniUtente.asmx/Carica_ImpostazioniUtente_Sezioni", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard GetMacroUsi(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Macrousi.asmx/GetMacroUsi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiModalitaTrasporto_NG(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Accise.asmx/LeggiUnitaTrasporto_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CaricaComboStatoImpianto_NG(CoreWSRequest<CoreWS_Generic<CaricaComboStatoImpianto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/FasiCicloColturale.asmx/CaricaComboStatoImpianto_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard Leggi_FasiCicloColturalexSpecie(CoreWSRequest<CoreWS_Generic<Leggi_FasiCicloColturalexSpecie>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/FasiCicloColturale.asmx/Leggi_FasiCicloColturalexSpecie", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<Contribute[]> ReadContributes(CoreWSRequest<CoreWS_Generic<Contribute>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Contribute.asmx/Read", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Contribute[]>>(result.ToString());
        }

        public RispostaStandard CaricaServiziPratiche(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/TransizioniDiStato.asmx/CaricaServiziPratiche", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CaricaTransizioniServizio(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/TransizioniDiStato.asmx/CaricaTransizioniServizio", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CaricaTransizioniXGruppoUtente(CoreWSRequest<CoreWS_Generic<GruppoUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/TransizioniDiStato.asmx/CaricaTransizioniXGruppoUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard AggiungiTrasizioniStatoXGruppoUtente(CoreWSRequest<CoreWS_Generic<ScriviGruppoUtentexTransizioniStato>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/TransizioniDiStato.asmx/AggiungiTrasizioniStatoXGruppoUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard RimuoviTrasizioniStatoXGruppoUtente(CoreWSRequest<CoreWS_Generic<ScriviGruppoUtentexTransizioniStato>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/TransizioniDiStato.asmx/RimuoviTrasizioniStatoXGruppoUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ScriviGruppoUtente(CoreWSRequest<CoreWS_Generic<ScriviGruppoUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/GruppiUtente.asmx/ScriviGruppoUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard LeggiIVA_Aliquote_NG(CoreWSRequest<CoreWS_Generic<LeggiIVA_Aliquote>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/IVA_Aliquote.asmx/LeggiIVA_Aliquote_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard get_Lista_Categorie_Animali_NG(CoreWSRequest<CoreWS_Generic<get_Lista_Categorie_Animali>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Lista_Categorie_Animali.asmx/get_Lista_Categorie_Animali_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard get_Lista_Generi_Animali(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Lista_Generi_Animali.asmx/get_Lista_Generi_Animali", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard get_Lista_IndirizziProd_Animali_NG(CoreWSRequest<CoreWS_Generic<get_Lista_IndirizziProd_Animali>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Lista_IndirizziProd_Animali.asmx/get_Lista_IndirizziProd_Animali_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard get_Lista_Razze_Animali_NG(CoreWSRequest<CoreWS_Generic<get_Lista_Razze_Animali>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Lista_Razze_Animali.asmx/get_Lista_Razze_Animali_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard get_Lista_Specie_Animali_NG(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Lista_Specie_Animali.asmx/get_Lista_Specie_Animali_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.MetodoProduzione>> MetodoProduzione_Leggi(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/MetodoProduzione.asmx/Leggi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.MetodoProduzione>>>(result.ToString());
        }
        public RispostaStandard Nazioni_Leggi(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Nazioni.asmx/Leggi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CaricaComboPortinnesti_NG(CoreWSRequest<CoreWS_Generic<CaricaComboPortinnesti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Portinnesti.asmx/CaricaComboPortinnesti_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.DensitaImpianto.Portinnesto>> CaricaComboPortinnesti_Modello(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiPortinnesto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Portinnesti.asmx/CaricaComboPortinnesti_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.metaschema.DensitaImpianto.Portinnesto>>>(result.ToString());
        }
        public RispostaStandard Regioni_Leggi(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Regioni.asmx/Leggi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<BaseCodeDescr>> LeggiLingue(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Zone.asmx/LeggiLingue", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<BaseCodeDescr>>>(result.ToString());
        }

        public RispostaStandard CaricaCombo_SpecieVegetale_Semente_New_NG(CoreWSRequest<CoreWS_Generic<CaricaCombo_SpecieVegetale_Semente_New>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/TipologieSementi.asmx/CaricaCombo_SpecieVegetale_Semente_New_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaComboTipologieSementi_NG(CoreWSRequest<CoreWS_Generic<CaricaComboTipologieSementi>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/TipologieSementi.asmx/CaricaComboTipologieSementi_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CaricaComboTipologieSementiByVeg_Cod_NG(CoreWSRequest<CoreWS_Generic<CaricaComboTipologieSementiByVeg_Cod>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/TipologieSementi.asmx/CaricaComboTipologieSementiByVeg_Cod_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiUMA_Lavorazioni_NG(CoreWSRequest<CoreWS_Generic<LeggiUMA_Lavorazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/UMA.asmx/LeggiUMA_Lavorazioni_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiUMA_Macrousi_NG(CoreWSRequest<CoreWS_Generic<LeggiUMA_Macrousi>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/UMA.asmx/LeggiUMA_Macrousi_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<ClasseTessitura> Leggi_ClasseTessitura(CoreWSRequest<CoreWS_Generic<LeggiClasseTessitura>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Tessiture.asmx/Leggi_ClasseTessitura", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ClasseTessitura>>(result.ToString());
        }
        public RispostaStandard CaricaImpiantiEsistenti_GIS_NG(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Anagrafica.CaricaImpiantiEsistenti_GIS>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/CaricaImpiantiEsistenti_GIS_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CaricaCombo_SpecieColtivate_NG(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Anagrafica.CaricaCombo_SpecieColtivate>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/CaricaCombo_SpecieColtivate_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard GetImpresexAppezza_Movimentati_NG(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Anagrafica.GetImpresexAppezza_Movimentati>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/GetImpresexAppezza_Movimentati_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard Leggi_Appezzamento_Global_Anagrafica_G_NG(CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/Leggi_Appezzamento_Global_Anagrafica_G_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard Leggi_Impianti_Anagrafica_NG(CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/Leggi_Impianti_Anagrafica_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<AppezzamentoJoinDescrizioni> ReadAgriculturalPlotLight(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/ReadAgriculturalPlotLight", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AppezzamentoJoinDescrizioni>>(result.ToString());
        }
        
        public RispostaStandard<LinkedMachine<Appezzamento.PK>[]> ReadAppezzamentiXParcoMacchine(CoreWSRequest<CoreWS_Generic<InData.Anagrafica.AppezzamentoXParcoMacchine>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/ReadAppezzamentiXParcoMacchine", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<LinkedMachine<Appezzamento.PK>[]>>(result.ToString());
        }
        
        public RispostaStandard<bool> WriteAppezzamentiXParcoMacchine(CoreWSRequest<CoreWS_Generic<Appezzamento[]>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/WriteAppezzamentiXParcoMacchine", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }
        
        public RispostaStandard<bool> EditAppezzamentiXParcoMacchine(CoreWSRequest<CoreWS_Generic<LinkedMachine<Appezzamento.PK>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/EditAppezzamentiXParcoMacchine", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }
        
        public RispostaStandard<bool> DeleteAppezzamentiXParcoMacchine(CoreWSRequest<CoreWS_Generic<AppezzamentoXParcoMacchine>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/DeleteAppezzamentiXParcoMacchine", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }
        
        public RispostaStandard<bool> DeleteAppezzamentiXParcoMacchineRecords(CoreWSRequest<CoreWS_Generic<List<Appezzamento>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/DeleteAppezzamentiXParcoMacchineRecords", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }

        public RispostaStandard<bool> UpdatePlotsWeaving(CoreWSRequest<CoreWS_Generic<Appezzamento[]>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/UpdatePlotsWeaving", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }
        
        public RispostaStandard<bool> UpdatePlotsSlope(CoreWSRequest<CoreWS_Generic<Appezzamento[]>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/UpdatePlotsSlope", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }
        
        public RispostaStandard<bool> UpdatePlotsConstrain(CoreWSRequest<CoreWS_Generic<Appezzamento[]>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/UpdatePlotsConstrain", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }

        public RispostaStandard<bool> WriteProjectsXContributesRecords(CoreWSRequest<CoreWS_Generic<List<Esercizio>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/WriteProjectsXContributesRecords", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }

        public RispostaStandard CaricaGrigliaOperazioni_NG(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/CaricaGrigliaOperazioni_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<Ricetta_Operazione>> CaricaRicetteOperazioni(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/CaricaRicetteOperazioni", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Ricetta_Operazione>>>(result.ToString());
        }
        public RispostaStandard<List<MenuRicette>> CaricaMenuRicette(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/CaricaMenuRicette", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<MenuRicette>>>(result.ToString());
        }
        public RispostaStandard CheckConformita(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/CheckConformita", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<ControllaMassimali_QdC> ControllaMassimali(CoreWSRequest<CoreWS_Generic<Controllo_Inserimento_Dose_Prodotto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/ControllaMassimali", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ControllaMassimali_QdC>>(result.ToString());
        }
        public RispostaStandard Controllo_Inserimento_Dose_Prodotto(CoreWSRequest<CoreWS_Generic<Controllo_Inserimento_Dose_Prodotto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/Controllo_Inserimento_Dose_Prodotto", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<object> Controllo_Sportello(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/Controllo_Sportello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        public RispostaStandard<AgronicaCoreModelsSTD.metaschema.Disciplinare> Default_DPI_QdC_NG(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/Default_DPI_QdC_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreModelsSTD.metaschema.Disciplinare>>(result.ToString());
        }
        public RispostaStandard<List<Nota_Operazione>> getGiustificazione_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/getGiustificazione_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Nota_Operazione>>>(result.ToString());
        }
        public RispostaStandard<AgronicaCoreDTOStd.InData.Anagrafica.Operazione> getNewAgenda_NG(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Agenda.getNewAgenda>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/getNewAgenda_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreDTOStd.InData.Anagrafica.Operazione>>(result.ToString());
        }
        public RispostaStandard<DoseConsentitaDiserbo> Imposta_DoseConsentitaDiserbo(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/Imposta_DoseConsentitaDiserbo", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<DoseConsentitaDiserbo>>(result.ToString());
        }
        public RispostaStandard<Inizializza_QdC> Inizializza_QdC_NG(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/Inizializza_QdC_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Inizializza_QdC>>(result.ToString());
        }
        public RispostaStandard<object> LeggiAgendaAttivita_toKendoGrid_NG(CoreWSRequest<CoreWS_Generic<LeggiAgendaAttivita_toKendoGrid>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiAgendaAttivita_toKendoGrid_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        public RispostaStandard<object> LeggiAgendaDDT_NG(CoreWSRequest<CoreWS_Generic<LeggiAgendaDDT>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiAgendaDDT_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        public RispostaStandard<object> LeggiAgendaDDT_toKendoGrid(CoreWSRequest<CoreWS_Generic<LeggiAgendaDDT_toKendoGrid>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiAgendaDDT_toKendoGrid_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        public RispostaStandard<object> LeggiAgendaGenerica_toKendoGrid_NG(CoreWSRequest<CoreWS_Generic<LeggiAgendaGenerica_toKendoGrid>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiAgendaGenerica_toKendoGrid_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        public RispostaStandard<object> LeggiAgendaVisite_toKendoGrid_NG(CoreWSRequest<CoreWS_Generic<LeggiAgendaVisite_toKendoGrid>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiAgendaVisite_toKendoGrid_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        public RispostaStandard<object> LeggiRicetteBrogliaccio_toKendoGrid_NG(CoreWSRequest<CoreWS_Generic<LeggiRicetteBrogliaccio_toKendoGrid>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiRicetteBrogliaccio_toKendoGrid_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        public RispostaStandard OperazioneAgenda_NG(CoreWSRequest<CoreWS_Generic<OperazioneAgenda>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/OperazioneAgenda_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ScriviListaAttivitaToRaccoglitore(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Agenda.ScriviListaAttivita>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/ScriviListaAttivitaToRaccoglitore", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<object> LeggiAttivitaDaAgendaNonVerbose(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiAttivitaDaAgendaNonVerbose", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        public RispostaStandard ModificaMultiplaListaAttivita(CoreWSRequest<CoreWS_Generic<ModificaMultipla_Attivita>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/ModificaMultiplaListaAttivita", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Redirect_In_Base_Al_Lav_Cod(CoreWSRequest<CoreWS_Generic<LeggiLink_Operazione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/Redirect_In_Base_Al_Lav_Cod", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaDefaultMacchine(CoreWSRequest<CoreWS_Generic<LeggiProfilazione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Profilazione.asmx/CaricaDefaultMacchine", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaDefaultOperatori(CoreWSRequest<CoreWS_Generic<LeggiProfilazione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Profilazione.asmx/CaricaDefaultOperatori", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaRisorseOperatori(CoreWSRequest<CoreWS_Generic<LeggiProfilazione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Profilazione.asmx/CaricaRisorseOperatori", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Scrivi_Profilazione_Dati(CoreWSRequest<CoreWS_Generic<LeggiProfilazione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Profilazione.asmx/Scrivi_Profilazione_Dati", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaRisorseMacchine(CoreWSRequest<CoreWS_Generic<LeggiProfilazione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Profilazione.asmx/CaricaRisorseMacchine", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiAziendexUtenti(CoreWSRequest<CoreWS_Generic<LeggiAziendexUtenti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Profilazione.asmx/LeggiAziendexUtenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PaginaLinkGestioneMagazziniQueryStringDto_NG(CoreWSRequest<CoreWS_Generic<PaginaLinkGestioneMagazziniQueryStringDto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/PaginaLinkGestioneMagazziniQueryStringDto_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaOperazioni_NG(CoreWSRequest<CoreWS_Generic<CaricaOperazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/CaricaOperazioni_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiImpianti_NG(CoreWSRequest<CoreWS_Generic<LeggiImpianti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Impianti.asmx/LeggiImpianti_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ricetta_numero_default_NG(CoreWSRequest<CoreWS_Generic<ricetta_numero_default>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/ricetta_numero_default_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard crea_ricetta_NG(CoreWSRequest<CoreWS_Generic<crea_ricetta>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/crea_ricetta_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CopiaOperazioneSingola_NG(CoreWSRequest<CoreWS_Generic<CopiaOperazioniDto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/CopiaOperazioneSingola_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard BloccaAttivitaAgenda_NG(CoreWSRequest<CoreWS_Generic<List<BloccaAttivitaAgenda>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/BloccaAttivitaAgenda_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard SbloccaAttivitaAgenda_NG(CoreWSRequest<CoreWS_Generic<List<BloccaAttivitaAgenda>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/SbloccaAttivitaAgenda_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaImpostazioniApp(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/CaricaImpostazioniApp_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Ricette_VerificaSeCostiCollegatiECancella_NG(CoreWSRequest<CoreWS_Generic<Elimina_Ricetta_Brogliaccio>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/Ricette_VerificaSeCostiCollegatiECancella_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Ricette_CancellaRicettaCancellaCosti_NG(CoreWSRequest<CoreWS_Generic<Elimina_Ricetta_Brogliaccio>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/Ricette_CancellaRicettaCancellaCosti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Ricette_Cancella_Multi(CoreWSRequest<CoreWS_Generic<Elimina_Ricetta_Brogliaccio>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/Ricette_Cancella_Multi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Ricette_CancellaRicettaConvertiCosti_NG(CoreWSRequest<CoreWS_Generic<Elimina_Ricetta_Brogliaccio>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/Ricette_CancellaRicettaConvertiCosti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaRicette_NG(CoreWSRequest<CoreWS_Generic<CaricaRicette>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/CaricaRicette_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard Ricette_Copia_NG(CoreWSRequest<CoreWS_Generic<Ricetta_Operazione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/Ricette_Copia_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard infomodifica_operazione_singola_new(CoreWSRequest<CoreWS_Generic<InfomodificaOperazioneSingola>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/infomodifica_operazione_singola_new", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }


        public RispostaStandard CaricaZoo_NG(CoreWSRequest<CoreWS_Generic<CaricaZoo>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/CaricaZoo_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard CaricaAlberoCentriDropdown_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaControlli_2010/AlberoAnagrafica2017.aspx/CaricaAlberoCentriDropdown_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard GetNodesAlberoAnagrafeNG(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaControlli_2010/AlberoAnagrafica2017.aspx/GetNodesAlberoAnagrafeNG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        #region Note
        public RispostaStandard<List<AgronicaCoreModelsSTD.attivita.note_intervento.NoteInterventoGruppi>> LeggiGruppi(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Note.asmx/LeggiGruppi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.attivita.note_intervento.NoteInterventoGruppi>>>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.attivita.note_intervento.NoteIntervento>> LeggiNote(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Note.asmx/LeggiNote", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.attivita.note_intervento.NoteIntervento>>>(result.ToString());
        }
        public RispostaStandard<bool> AggiornaDefaultNote(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Note.asmx/AggiornaDefaultNote", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }
        #endregion

        public RispostaStandard<object> LeggiListaAttivitaDaAgenda(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiListaAttivitaDaAgenda", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }

        public RispostaStandard<object> LeggiListaAttivitaDaRicettaOperazionePerAgenda(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiListaAttivitaDaRicettaOperazionePerAgenda", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        public RispostaStandard<object> LeggiListaAttivitaDaRicettaOperazionePerRicetta(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiListaAttivitaDaRicettaOperazionePerRicetta", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        public RispostaStandard<object> LeggiListaAttivitaDaRicettaOperazionePerBrogliaccio(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiListaAttivitaDaRicettaOperazionePerBrogliaccio", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }


        public RispostaStandard GetCampi_NG(CoreWSRequest<CoreWS_Generic<GetCampi>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Campi.asmx/GetCampi_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Leggi_Campi_Codici(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Campi.asmx/Leggi_Campi_Codici", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Leggi_SpecieVegetali(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Campi.asmx/Leggi_SpecieVegetali", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Campo>> LeggiCampi_perSpecieImpianti(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Campi.asmx/LeggiCampi_perSpecieImpianti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Campo>>>(result.ToString());
        }
        public RispostaStandard<Boolean> Test_Campi_Archivio_Lettura(CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Campi.asmx/Test_Campi_Archivio_Lettura", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Boolean>>(result.ToString());
        }
        public RispostaStandard<Boolean> Test_Campi_Archivio_Scrittura(CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Campi.asmx/Test_Campi_Archivio_Scrittura", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Boolean>>(result.ToString());
        }
        public RispostaStandard Carica_Cmb_Imprese(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Carica_Cmb_Imprese_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard Carica_Cmb_Imprese_Area_NG(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Carica_Cmb_Imprese_Area_Carica_Cmb_Imprese_UMANG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Carica_Cmb_Imprese_UMA_NG(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Carica_Cmb_Imprese_UMA_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaAzienda_GIS_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/CaricaAzienda_GIS_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<BaseCodeDescr>> Leggi_Certificazioni(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Leggi_Certificazioni", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<BaseCodeDescr>>>(result.ToString());
        }

        public RispostaStandard<List<GruppoRaccolta>> LeggiGruppiRaccoltaValidi(CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GruppiRaccolta/GruppiRaccolta.asmx/LeggiGruppiRaccoltaValidi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<GruppoRaccolta>>>(result.ToString());
        }

        public RispostaStandard<List<GruppoRaccolta>> LeggiGruppiRaccolta(CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GruppiRaccolta/GruppiRaccolta.asmx/LeggiGruppiRaccolta", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<GruppoRaccolta>>>(result.ToString());
        }

        public RispostaStandard<GruppoRaccolta> LeggiGruppoRaccoltaImpresa(CoreWSRequest<CoreWS_Generic<InData.Parametri_ObjParametriAgenda_NG>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GruppiRaccolta/GruppiRaccolta.asmx/LeggiGruppoRaccoltaImpresa", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<GruppoRaccolta>>(result.ToString());
        }

        internal RispostaStandard<string> LeggiDisciplinariCombo(CoreWSRequest<CoreWS_Generic<LeggiDisciplinariCombo>> request)
        {
            var payload = JsonConvert.SerializeObject(new { request.InData.InData.veg_cod, request.InData.InData.data, request.InData.InData.flag_disciplinareprivato, request.InData.objP.objP_server, request.InData.objP.objP_utenti });
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreDPI/DPI.asmx/CaricaComboDPI", payload);
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }

        public RispostaStandard<GruppoRaccolta> ScriviModificaCancella_GruppoRaccolta(CoreWSRequest<CoreWS_Generic<ScriviGruppoRaccolta>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GruppiRaccolta/GruppiRaccolta.asmx/ScriviModificaCancella_GruppoRaccolta", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<GruppoRaccolta>>(result.ToString());
        }

        public RispostaStandard LeggiImpreseConFiltroUtente(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/LeggiImpreseConFiltroUtente", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Impresa>> LeggiImpreseConFiltroUtente_Modello(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/LeggiImpreseConFiltroUtente_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Impresa>>>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Impresa>> LeggiImpreseConFiltroUtenteHubAgea_Modello(CoreWSRequest<CoreWS_Generic<FarmFilters>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/LeggiImpreseConFiltroUtenteHubAgea_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Impresa>>>(result.ToString());
        }
        public RispostaStandard<ImpresaAgeaExcel> ReportImpreseExcelFiltroAgea(CoreWSRequest<CoreWS_Generic<FarmFilters>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/ExportImpreseAgeaExcel", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ImpresaAgeaExcel>>(result.ToString());
        }
        public RispostaStandard LeggiImpreseConFiltroUtenteCodiceSocio_NG(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/LeggiImpreseConFiltroUtenteCodiceSocio_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<Boolean> Test_Imprese_Archivio_Lettura(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Test_Imprese_Archivio_Lettura", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Boolean>>(result.ToString());
        }
        public RispostaStandard<Boolean> Test_Imprese_Archivio_Scrittura(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Test_Imprese_Archivio_Scrittura", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Boolean>>(result.ToString());
        }

        public RispostaStandard<ParcoMacchineDto> cancellaMacchina_NG(CoreWSRequest<CoreWS_Generic<CancellaMacchina>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/cancellaMacchina_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ParcoMacchineDto>>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValore_IntTesto>> getAlimentazioneMacchine(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/getAlimentazioneMacchine", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValore_IntTesto>>>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValore_IntTesto>> getCentriVisibilita_NG(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/getCentriVisibilita_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValore_IntTesto>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValoreTesto>> getElencoDettaglio1_NG(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/getElencoDettaglio1_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValoreTesto>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValoreTesto>> getElencoDettaglio2_NG(CoreWSRequest<CoreWS_Generic<getElencoDettaglio2>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/getElencoDettaglio2_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValoreTesto>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValore_IntTesto>> getElencoMarche_NG(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/getElencoMarche_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValore_IntTesto>>>(result.ToString());
        }
        public RispostaStandard getElencoMarcheWS_NG(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/getElencoMarcheWS_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValoreTesto>> getElencoTipo_NG(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/getElencoTipo_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValoreTesto>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValore_IntTesto>> getFinalita(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/getFinalita", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValore_IntTesto>>>(result.ToString());
        }
        public RispostaStandard<string> getJSON_ParcoMacchine_js(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/getJSON_ParcoMacchine_js", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }
        public RispostaStandard<AgronicaCoreDTOStd.InData.Anagrafica.ParcoMacchineDto> getMacchina_NG(CoreWSRequest<CoreWS_Generic<getMacchina>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/getMacchina_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreDTOStd.InData.Anagrafica.ParcoMacchineDto>>(result.ToString());
        }
        public RispostaStandard<AgronicaCoreDTOStd.InData.Anagrafica.ParcoMacchineDto> getNewMacchina_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/getNewMacchina_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreDTOStd.InData.Anagrafica.ParcoMacchineDto>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValore_IntTesto>> getPotenzaUdm(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/getPotenzaUdm", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValore_IntTesto>>>(result.ToString());
        }

        public RispostaStandard GetTipoMacchine(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/GetTipoMacchine", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValore_IntTesto>> getTitoloPossesso(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/getTitoloPossesso", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValore_IntTesto>>>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValore_IntTesto>> getTipoTarga(CoreWSRequest<CoreWS_Generic<String>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/getTipoTarga", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreDTOStd.InData.Anagrafica.rispostaValore_IntTesto>>>(result.ToString());
        }

        public RispostaStandard Leggi_Macchine_Per_Contatto_NG(CoreWSRequest<CoreWS_Generic<Leggi_Macchine_Per_Contatto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/Leggi_Macchine_Per_Contatto_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Leggi_Macchine_Per_Piva_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/Leggi_Macchine_Per_Piva_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<BaseCodeDescr>> Leggi_Caratteristiche_Macchina_Anagrafica(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/Leggi_Caratteristiche_Macchina_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<BaseCodeDescr>>>(result.ToString());
        }

        public RispostaStandard scriviMacchina_NG(CoreWSRequest<CoreWS_Generic<scriviMacchina>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/scriviMacchina_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<Boolean> Test_Macchine_Archivio_Lettura(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/Test_Macchine_Archivio_Lettura", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Boolean>>(result.ToString());
        }
        public RispostaStandard Test_Macchine_Archivio_Lettura(CoreWSRequest<CoreWS_Generic<scriviMacchina>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/Test_Macchine_Archivio_Lettura", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<Boolean> Test_Macchine_Archivio_Scrittura(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Macchine.asmx/Test_Macchine_Archivio_Scrittura", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Boolean>>(result.ToString());
        }
        public RispostaStandard<Object> LeggiParticelleCatastali_toKendoGrid(CoreWSRequest<CoreWS_Generic<LeggiParticelleCatastali_toKendoGrid>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Catasto.asmx/LeggiParticelleCatastali_toKendoGrid", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Object>>(result.ToString());
        }
        public RispostaStandard<Boolean> Test_Catasto_Archivio_Lettura(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Catasto.asmx/Test_Catasto_Archivio_Lettura", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Boolean>>(result.ToString());
        }
        public RispostaStandard<Boolean> Test_Catasto_Archivio_Scrittura(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Catasto.asmx/Test_Catasto_Archivio_Scrittura", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Boolean>>(result.ToString());
        }
        public RispostaStandard Carica_Comuni(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/CentroAziendale.asmx/Carica_Comuni", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaCentroAziendale_GIS_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/CentroAziendale.asmx/CaricaCentroAziendale_GIS_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard GetCentri_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/CentroAziendale.asmx/GetCentri_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.CentroAziendale>> Leggi_Centri_Aziendali_perSpecieImpianti(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiCentriAziendali>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/CentroAziendale.asmx/Leggi_Centri_Aziendali_perSpecieImpianti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.CentroAziendale>>>(result.ToString());
        }
        public RispostaStandard<List<ImpresaParametri>> LeggiImpreseParametri(CoreWSRequest<CoreWS_Generic<LeggiImpreseParametri>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GHG/GHG_WS.asmx/LeggiImpreseParametri_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<ImpresaParametri>>>(result.ToString());
        }
        public RispostaStandard ScriviImpreseParametri(CoreWSRequest<CoreWS_Generic<ScriviModificaImpreseParametri>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GHG/GHG_WS.asmx/ScriviImpreseParametri_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ModificaImpreseParametri(CoreWSRequest<CoreWS_Generic<ScriviModificaImpreseParametri>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GHG/GHG_WS.asmx/ModificaImpreseParametri_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CancellaImpreseParametri(CoreWSRequest<CoreWS_Generic<ScriviModificaImpreseParametri>> request)
        {
            JObject result = Request(HttpMethod.Post, "/GHG/GHG_WS.asmx/CancellaImpreseParametri_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiCentriConFiltroUtente_NG(CoreWSRequest<CoreWS_Generic<LeggiCentriConFiltroUtente>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/CentroAziendale.asmx/LeggiCentriConFiltroUtente_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<Boolean> Test_Centri_Archivio_Lettura(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/CentroAziendale.asmx/Test_Centri_Archivio_Lettura", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Boolean>>(result.ToString());
        }
        public RispostaStandard<Boolean> Test_Centri_Archivio_Scrittura(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/CentroAziendale.asmx/Test_Centri_Archivio_Scrittura", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Boolean>>(result.ToString());
        }

        public RispostaStandard LeggiElencoCompletoProdotti_NG(CoreWSRequest<CoreWS_Generic<LeggiElencoCompletoProdotti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/LeggiElencoCompletoProdotti_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiElencoCompletoProdottiMultiCategoria_NG(CoreWSRequest<CoreWS_Generic<LeggiElencoCompletoProdottiMultiCategoria>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/LeggiElencoCompletoProdottiMultiCategoria_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiElencoProdotti_APP_NG(CoreWSRequest<CoreWS_Generic<LeggiElencoProdotti_APP>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/LeggiElencoProdotti_APP_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<List<BaseCodeDescrStr>> Leggi_Contributi(CoreWSRequest<CoreWS_Generic<AgronicaCoreModelsSTD.anagrafiche.Impianto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/Leggi_Contributi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<BaseCodeDescrStr>>>(result.ToString());
        }
        public RispostaStandard<AgronicaCoreDTOStd.InData.Data> Leggi_Max_DataModifica(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.LeggiFiltro>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/Leggi_Max_DataModifica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreDTOStd.InData.Data>>(result.ToString());
        }

        public RispostaStandard<string> leggiGenerazionePoligoniDefaultValue(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/leggiGenerazionePoligoniDefaultValue", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }

        public RispostaStandard Carica_Stalle_NG(CoreWSRequest<CoreWS_Generic<Carica_Stalle>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Fabbricati.asmx/Carica_Stalle_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Leggi_Fabbricati_Anagrafica(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Fabbricati.asmx/Leggi_Fabbricati_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Leggi_Fabbricati_Anagrafica_CodDescr(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Fabbricati.asmx/Leggi_Fabbricati_Anagrafica_CodDescr", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<Fabbricato> LeggiUltimo_Magazzino_Prodotto_Movimentato(CoreWSRequest<CoreWS_Generic<LeggiUltimo_Magazzino_Prodotto_Movimentato>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Fabbricati.asmx/Ultimo_Magazzino_Prodotto_Movimentato", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Fabbricato>>(result.ToString());
        }

        public RispostaStandard Leggi_Magazzini_Organismoreferente_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Fabbricati.asmx/Leggi_Magazzini_Organismoreferente_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiFabbricati_NG(CoreWSRequest<CoreWS_Generic<LeggiFabbricati>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Fabbricati.asmx/LeggiFabbricati_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaComboCmb_MagazzinoConferimento_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/CaricaComboCmb_MagazzinoConferimento_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaComboCmb_OrganismoReferente_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/CaricaComboCmb_OrganismoReferente_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard CaricaComboCmb_Riferimento_Trasferimento_Dati_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/CaricaComboCmb_Riferimento_Trasferimento_Dati_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Leggi_Contatti_Anagrafica_NG(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/Leggi_Contatti_Anagrafica_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Leggi_Contatti_Per_Piva_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/Leggi_Contatti_Per_Piva_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiFornitori_FF_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/LeggiFornitori_FF_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiRapportoDocumenti_NG(CoreWSRequest<CoreWS_Generic<LeggiRapportoDocumenti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/LeggiRapportoDocumenti_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiRapportoSpecifico_NG(CoreWSRequest<CoreWS_Generic<LeggiRapportoSpecifico>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/LeggiRapportoSpecifico_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Leggi_Analisi_Testata_Per_Piva_NG(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Analisi_Testata.asmx/Leggi_Analisi_Testata_Per_Piva_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiAree_NG(CoreWSRequest<CoreWS_Generic<LeggiAree>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Area_Omogenea.asmx/LeggiAree", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard PadriGerarchia2_NG(CoreWSRequest<CoreWS_Generic<PadriGerarchia2>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/GerarchiaImprese.asmx/PadriGerarchia", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard PadriGerarchia(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/GerarchiaImprese.asmx/PadriGerarchia", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiIndirizziContatto_NG(CoreWSRequest<CoreWS_Generic<LeggiIndirizziContatto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Indirizzi.asmx/LeggiIndirizziContatto_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard LeggiIndirizzoAziendaSuperUser(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Indirizzi.asmx/LeggiIndirizziAziendaSuperUser", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard GetImpresexParticelle_NG(CoreWSRequest<CoreWS_Generic<GetImpresexParticelle>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/ParticelleCatastali.asmx/GetImpresexParticelle", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard GetImpresexParticelle_Movimentate_NG(CoreWSRequest<CoreWS_Generic<GetImpresexParticelle_Movimentate>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/ParticelleCatastali.asmx/GetImpresexParticelle_Movimentate_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<CentroDropdownLists> Leggi_Centro_Dropdowns(CoreWSRequest<CoreWS_Generic<Parametri_ObjParametriAgenda_NG_GestioneRichieste>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/CentroAziendale.asmx/Leggi_Centro_Dropdowns", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<CentroDropdownLists>>(result.ToString());
        }
        public RispostaStandard<object> Carica_Centri_Impresa(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.CaricaCentriImpresa>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/CentroAziendale.asmx/Carica_Centri_Impresa_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        public RispostaStandard DatiRelativiPercorsoBreadcrumbs_NG(CoreWSRequest<CoreWS_Generic<DatiRelativiPercorsoBreadcrumbs>> request)
        {
            JObject result = Request(HttpMethod.Post, "Anagrafica/AnagraficaSharedFns.asmx/DatiRelativiPercorsoBreadcrumbs_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<AgronicaCoreDTOStd.InData.Anagrafica.Operazione> getNewAgenda(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Agenda.getNewAgenda>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/getNewAgenda", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreDTOStd.InData.Anagrafica.Operazione>>(result.ToString());
        }

        public RispostaStandard<object> LeggiAgendaAttivita_toKendoGrid(CoreWSRequest<CoreWS_Generic<LeggiAgendaAttivita_toKendoGrid>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiAgendaAttivita_toKendoGrid", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        public RispostaStandard<object> LeggiAgendaDDT(CoreWSRequest<CoreWS_Generic<LeggiAgendaDDT>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiAgendaDDT", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        public RispostaStandard<object> LeggiAgendaGenerica_toKendoGrid(CoreWSRequest<CoreWS_Generic<LeggiAgendaGenerica_toKendoGrid>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiAgendaGenerica_toKendoGrid", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        public RispostaStandard<object> LeggiAgendaVisite_toKendoGrid(CoreWSRequest<CoreWS_Generic<LeggiAgendaVisite_toKendoGrid>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiAgendaVisite_toKendoGrid", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        public RispostaStandard<object> LeggiRicetteBrogliaccio_toKendoGrid(CoreWSRequest<CoreWS_Generic<LeggiRicetteBrogliaccio_toKendoGrid>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/LeggiRicetteBrogliaccio_toKendoGrid", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        public RispostaStandard OperazioneAgenda(CoreWSRequest<CoreWS_Generic<OperazioneAgenda>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/OperazioneAgenda", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard GetCampi(CoreWSRequest<CoreWS_Generic<GetCampi>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Campi.asmx/GetCampi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<bool> CheckUtenteTipologiaAccessoQDC(CoreWSRequest<CoreWS_Generic<UtenteTipologiaAccessoQDC>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Pratiche/Pratiche_W.asmx/CheckUtenteTipologiaAccessoQDC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }

        #region Funzioni Menu

        public RispostaStandard<AlberoMenu> LeggiAlberoMenu(CoreWSRequest<CoreWS_Generic<object>> request)
        {

            JObject ripostaAlberoMenu = Request(HttpMethod.Post, "/Menu/Menu.asmx/LeggiAlberoMenu", JsonConvert.SerializeObject(request));
            var alberoMenu = JsonConvert.DeserializeObject<RispostaStandard<AlberoMenu>>(ripostaAlberoMenu.ToString());

            return alberoMenu;
        }
        
        public RispostaStandard<AlberoMenu> LeggiAlberoMenu_APP(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject ripostaAlberoMenu = Request(HttpMethod.Post, "/Menu/Menu.asmx/LeggiAlberoMenu_APP", JsonConvert.SerializeObject(request));
            var alberoMenu = JsonConvert.DeserializeObject<RispostaStandard<AlberoMenu>>(ripostaAlberoMenu.ToString());
            return alberoMenu;
        }

        public RispostaStandard<BreadcrumbsInfo> ottieniIconaDaTesto(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject risposta = Request(HttpMethod.Post, "/Menu/Menu.asmx/ottieniIconaDaTesto", JsonConvert.SerializeObject(request));
            RispostaStandard<BreadcrumbsInfo> risp = JsonConvert.DeserializeObject<RispostaStandard<BreadcrumbsInfo>>(risposta.ToString());
            return risp;
        }


        public RispostaStandard<string> AggiornaPreferiti(CoreWSRequest<CoreWS_Generic<preferiti_in>> request)
        {


            JObject risposta = Request(HttpMethod.Post, "/Menu/Menu.asmx/aggiornaPreferiti", JsonConvert.SerializeObject(request));
            RispostaStandard<string> risp = JsonConvert.DeserializeObject<RispostaStandard<string>>(risposta.ToString());
            return risp;


        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.anagrafiche.Impresa>> ricercaAzienda(CoreWSRequest<CoreWS_Generic<string>> request)
        {


            JObject risposta = Request(HttpMethod.Post, "/Anagrafica/Imprese.asmx/Leggi_Imprese_Filtro", JsonConvert.SerializeObject(request));
            RispostaStandard<List<Impresa>> risp = JsonConvert.DeserializeObject<RispostaStandard<List<Impresa>>>(risposta.ToString());
            return risp;


        }

        public RispostaStandard<string> aggiornaAttivitaMenuUtente(CoreWSRequest<CoreWS_Generic<attivitaNavigazioneAziende_in>> request)
        {


            JObject risposta = Request(HttpMethod.Post, "/Menu/Menu.asmx/aggiornaAttivitaNavigazioneAziende", JsonConvert.SerializeObject(request));
            RispostaStandard<string> risp = JsonConvert.DeserializeObject<RispostaStandard<string>>(risposta.ToString());
            return risp;


        }
        public RispostaStandard<Utente> ottieniInformazioneUtente(CoreWSRequest<CoreWS_Generic<object>> request)
        {


            JObject risposta = Request(HttpMethod.Post, "/Menu/Menu.asmx/ottieniInformazioneUtente", JsonConvert.SerializeObject(request));
            RispostaStandard<Utente> risp = JsonConvert.DeserializeObject<RispostaStandard<Utente>>(risposta.ToString());
            return risp;


        }

        public RispostaStandard<InformazioniAssistenza> ottieniInformazioniAssistenza(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject risposta = Request(HttpMethod.Post, "/Menu/Menu.asmx/ottieniInformazioniAssistenza", JsonConvert.SerializeObject(request));
            RispostaStandard<InformazioniAssistenza> risp = JsonConvert.DeserializeObject<RispostaStandard<InformazioniAssistenza>>(risposta.ToString());
            return risp;
        }


        public RispostaStandard<List<Impresa>> ottieniUltimeAziendeSelezionate(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject risposta = Request(HttpMethod.Post, "/Menu/Menu.asmx/ultimeAziendeSelezionate", JsonConvert.SerializeObject(request));
            RispostaStandard<List<Impresa>> risp = JsonConvert.DeserializeObject<RispostaStandard<List<Impresa>>>(risposta.ToString());
            return risp;
        }

        public RispostaStandard<List<Impresa>> ottieniUltimeAziendeSelezionate2(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            RispostaStandard<List<Impresa>> r = new RispostaStandard<List<Impresa>>() { RispostaOK = true };
            JObject risposta = Request(HttpMethod.Post, "/Menu/Menu.asmx/ultimeAziendeSelezionate2", JsonConvert.SerializeObject(request));
            //RispostaStandard<List<Impresa>> risp = JsonConvert.DeserializeObject<RispostaStandard<List<Impresa>>>(risposta.ToString());
            return r;
        }

        public RispostaStandard<BreadcrumbsInfo> OttieniBreadcrumbs(CoreWSRequest<CoreWS_Generic<int>> request)
        {
            JObject risposta = Request(HttpMethod.Post, "/Menu/Menu.asmx/OttieniBreadcrumbs", JsonConvert.SerializeObject(request));
            RispostaStandard<BreadcrumbsInfo> risp = JsonConvert.DeserializeObject<RispostaStandard<BreadcrumbsInfo>>(risposta.ToString());
            return risp;
        }

        public RispostaStandard LeggiVersioneHeader(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject risposta = Request(HttpMethod.Post, "/Menu/Menu.asmx/LeggiVersioneHeader", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(risposta.ToString());
        }

        #endregion

        #region Funzioni Widgets
        public RispostaStandard<List<Widget>> ElencoWidgets(CoreWSRequest<CoreWS_Generic<Widgets_In>> request)
        {
            var results = new List<Widget>();
            JObject risposta = null;

            risposta = Request(HttpMethod.Post, "/Metaschema/Widgets.asmx/ElencoWidgets", JsonConvert.SerializeObject(request));
            var widgetsTutti = JsonConvert.DeserializeObject<RispostaStandard<List<Widget>>>(risposta.ToString()).RispostaStringa;

            if (widgetsTutti == null || !widgetsTutti.Any())
                return new RispostaStandard<List<Widget>> { RispostaOK = true, RispostaStringa = results };

            risposta = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_Widgets.asmx/ElencoWidgets", JsonConvert.SerializeObject(request));
            var widgetsUtenti = JsonConvert.DeserializeObject<RispostaStandard<List<Widget>>>(risposta.ToString()).RispostaStringa;

            widgetsUtenti.ForEach(wu =>
            {
                var widget = widgetsTutti.FirstOrDefault(w => w.IdWidget.Equals(wu.IdWidget));
                if (widget != null)
                    results.Add(wu.UserWidgetToCompleteWidget(widget));
            }
            );

            var filteredWidgets = results.Where(w => w.Abilitato.Value.Equals(request.InData.InData.Abilitato.Value));
            filteredWidgets = filteredWidgets.Where(w => w.Visibile.Equals(request.InData.InData.Visibile.Value));
            if (!String.IsNullOrEmpty(request.InData.InData.Descrizione))
                filteredWidgets = filteredWidgets.Where(w => w.Descrizione.ToLower().Contains(request.InData.InData.Descrizione.ToLower()));
            if (!String.IsNullOrEmpty(request.InData.InData.Codice))
                filteredWidgets = filteredWidgets.Where(w => w.Codice.ToLower().Contains(request.InData.InData.Codice.ToLower()));

            return new RispostaStandard<List<Widget>> { RispostaOK = true, RispostaStringa = filteredWidgets.ToList() };
        }

        public RispostaStandard<string> UserWidgetsReset(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            var risposta = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_Widgets.asmx/UserWidgetsReset", JsonConvert.SerializeObject(request));
            return new RispostaStandard<string>
            {
                RispostaOK = true,
                RispostaStringa = risposta.ToString()
            };
        }

        public RispostaStandard<string> AggiornaConfigurazioneWidget(CoreWSRequest<CoreWS_Generic<Widget_Configuration>> request)
        {
            var risposta = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_Widgets.asmx/AggiornaConfigurazioneWidget", JsonConvert.SerializeObject(request));
            return new RispostaStandard<string>
            {
                RispostaOK = true,
                RispostaStringa = risposta.ToString()
            };
        }

        public RispostaStandard<Widget_Complete_Configuration> ElencoWidgetsXConfigurazione(CoreWSRequest<CoreWS_Generic<Widgets_In>> request)
        {
            var results = new Widget_Complete_Configuration
            {
                PresetIniziale = new List<Widget_Configuration>(),
                UserWidgets = new List<Widget_Configuration>()
            };
            JObject risposta = null;

            risposta = Request(HttpMethod.Post, "/Metaschema/Widgets.asmx/ElencoWidgets", JsonConvert.SerializeObject(request));
            var widgetsPresetIniziale = JsonConvert.DeserializeObject<RispostaStandard<List<Widget>>>(risposta.ToString()).RispostaStringa;

            if (!widgetsPresetIniziale.Any())
                return new RispostaStandard<Widget_Complete_Configuration> { RispostaOK = true, RispostaStringa = results };
            widgetsPresetIniziale.ForEach(w =>
                results.PresetIniziale.Add(new Widget_Configuration(w)));

            risposta = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_Widgets.asmx/ElencoWidgets", JsonConvert.SerializeObject(request));
            var widgetsUtenti = JsonConvert.DeserializeObject<RispostaStandard<List<Widget>>>(risposta.ToString()).RispostaStringa;

            //DCA: leggo se eventualmente c'è una configurazione per azienda
            request.InData.InData.UserName = CostantiPersonalizzate.UserDefaultWidgets;
            risposta = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_Widgets.asmx/ElencoWidgets", JsonConvert.SerializeObject(request));
            var widgetsConfAzienda = JsonConvert.DeserializeObject<RispostaStandard<List<Widget>>>(risposta.ToString()).RispostaStringa;

            if (widgetsConfAzienda.Any())
            {
                //sovrascrivo il Preset
                results.PresetIniziale.Clear();
                widgetsConfAzienda.ForEach(wu => 
                {
                    var widget = widgetsPresetIniziale.FirstOrDefault(w => w.IdWidget.Equals(wu.IdWidget));
                    if (widget != null)
                    {
                        var widgetCompleto = wu.UserWidgetToCompleteWidget(widget);
                        results.PresetIniziale.Add(new Widget_Configuration(widgetCompleto));
                    }
                });
            }

            if (widgetsUtenti.Any())
            {
                widgetsUtenti.ForEach(wu =>
                {
                    var widget = widgetsPresetIniziale.FirstOrDefault(w => w.IdWidget.Equals(wu.IdWidget));
                    if (widget != null)
                    {
                        var widgetCompleto = wu.UserWidgetToCompleteWidget(widget);
                        results.UserWidgets.Add(new Widget_Configuration(widgetCompleto));
                    }
                });
            }

            return new RispostaStandard<Widget_Complete_Configuration> { RispostaOK = true, RispostaStringa = results };
        }

        public RispostaStandard<List<string>> AggiornaWidgetsUtente(CoreWSRequest<CoreWS_Generic<Aggiorna_Widgets_In>> request)
        {

            var risposta = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_Widgets.asmx/AggiornaWidgetsUtente", JsonConvert.SerializeObject(request));
            return new RispostaStandard<List<string>>
            {
                RispostaOK = true,
                RispostaStringa = JsonConvert.DeserializeObject<RispostaStandard<List<string>>>(risposta.ToString()).RispostaStringa
            };
        }

        public RispostaStandard<List<string>> ScriviAggiornaWidgetsUtente(CoreWSRequest<CoreWS_Generic<Aggiorna_Widgets_In>> request)
        {

            var risposta = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_Widgets.asmx/ScriviAggiornaWidgetsUtente", JsonConvert.SerializeObject(request));
            return new RispostaStandard<List<string>>
            {
                RispostaOK = true,
                RispostaStringa = JsonConvert.DeserializeObject<RispostaStandard<List<string>>>(risposta.ToString()).RispostaStringa
            };
        }

        public RispostaStandard<List<Widget_MovimentoMagazzino>> Widget_LeggiUltimiMovimentiMagazzino(CoreWSRequest<CoreWS_Generic<Widget_MovimentiMagazziono_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Widgets/Widgets_Dati.asmx/LeggiMovimentiMagazzino", JsonConvert.SerializeObject(request));
            return new RispostaStandard<List<Widget_MovimentoMagazzino>>
            {
                RispostaOK = true,
                RispostaStringa = JsonConvert.DeserializeObject<RispostaStandard<List<Widget_MovimentoMagazzino>>>(result.ToString()).RispostaStringa
            };
        }

        public RispostaStandard<List<Widget_Operazione>> LeggiUltimiRilievi(CoreWSRequest<CoreWS_Generic<Widget_Operazioni_IN>> request)
        {

            JObject operazioni = Request(HttpMethod.Post, "/Widgets/Widgets_Dati.asmx/LeggiUltimiRilievi", JsonConvert.SerializeObject(request));

            var settings = new JsonSerializerSettings() { DateTimeZoneHandling = DateTimeZoneHandling.Local };
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<List<Widget_Operazione>>>(operazioni.ToString(), settings).RispostaStringa;

            return new RispostaStandard<List<Widget_Operazione>>()
            {
                RispostaOK = true,
                RispostaStringa = risposta
            };

        }

        public RispostaStandard<List<Widget_Operazione>> LeggiUltimeAttivita(CoreWSRequest<CoreWS_Generic<Widget_Operazioni_IN>> request)
        {

            JObject operazioni = Request(HttpMethod.Post, "/Widgets/Widgets_Dati.asmx/LeggiUltimeAttivita", JsonConvert.SerializeObject(request));

            var settings = new JsonSerializerSettings() { DateTimeZoneHandling = DateTimeZoneHandling.Local };
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<List<Widget_Operazione>>>(operazioni.ToString(), settings).RispostaStringa;

            return new RispostaStandard<List<Widget_Operazione>>()
            {
                RispostaOK = true,
                RispostaStringa = risposta
            };

        }

        public RispostaStandard<List<Widget_Operazione>> LeggiUltimeVisite(CoreWSRequest<CoreWS_Generic<Widget_Operazioni_IN>> request)
        {
            JObject visite = Request(HttpMethod.Post, "/Widgets/Widgets_Dati.asmx/LeggiUltimeVisite", JsonConvert.SerializeObject(request));

            var settings = new JsonSerializerSettings() { DateTimeZoneHandling = DateTimeZoneHandling.Local };
            RispostaStandard<List<Widget_Operazione>> risp = JsonConvert.DeserializeObject<RispostaStandard<List<Widget_Operazione>>>(visite.ToString(), settings);
            return risp;
        }

        public RispostaStandard<List<Widget_Coltura>> LeggiColture(CoreWSRequest<CoreWS_Generic<Widget_Culture_IN>> request)
        {

            JObject operazioni = Request(HttpMethod.Post, "/Widgets/Widgets_Dati.asmx/LeggiColture", JsonConvert.SerializeObject(request));

            var settings = new JsonSerializerSettings() { DateTimeZoneHandling = DateTimeZoneHandling.Local };
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<List<Widget_Coltura>>>(operazioni.ToString(), settings).RispostaStringa;

            return new RispostaStandard<List<Widget_Coltura>>()
            {
                RispostaOK = true,
                RispostaStringa = risposta
            };

        }

        public RispostaStandard<List<Widget_GHGColture>> Leggi_GHGColture(CoreWSRequest<CoreWS_Generic<Widget_GHGColture_IN>> request)
        {

            JObject operazioni = Request(HttpMethod.Post, "/Widgets/Widgets_Dati.asmx/Leggi_GHGColture", JsonConvert.SerializeObject(request));

            var settings = new JsonSerializerSettings() { DateTimeZoneHandling = DateTimeZoneHandling.Local };
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<List<Widget_GHGColture>>>(operazioni.ToString(), settings).RispostaStringa;

            return new RispostaStandard<List<Widget_GHGColture>>()
            {
                RispostaOK = true,
                RispostaStringa = risposta
            };

        }

        public RispostaStandard<List<Widget_StimeProduzioneColture>> Leggi_StimeProduzioneColture(CoreWSRequest<CoreWS_Generic<Widget_StimeProduzioneColture_IN>> request)
        {

            JObject operazioni = Request(HttpMethod.Post, "/Widgets/Widgets_Dati.asmx/Leggi_StimeProduzioneColture", JsonConvert.SerializeObject(request));
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<List<Widget_StimeProduzioneColture>>>(operazioni.ToString()).RispostaStringa;

            return new RispostaStandard<List<Widget_StimeProduzioneColture>>()
            {
                RispostaOK = true,
                RispostaStringa = risposta
            };

        }

        public RispostaStandard<WidgetIndiciProduttivitaGlobal> readWidgetIndiciProduttivita(CoreWSRequest<CoreWS_Generic<WidgetRequestIndiciProduttivita>> request)
        {
            JObject operazioni = Request(HttpMethod.Post, "/Widgets/WidgetIndiciProduttivitaWS.asmx/readWidgetIndiciProduttivita", JsonConvert.SerializeObject(request));
            string str = operazioni.ToString();
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<WidgetIndiciProduttivitaGlobal>>(str).RispostaStringa;

            return new RispostaStandard<WidgetIndiciProduttivitaGlobal>()
            {
                RispostaOK = true,
                RispostaStringa = risposta
            };
        }

        public RispostaStandard<List<int>> readAvailableYearsIndiciProduttivita(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject operazioni = Request(HttpMethod.Post, "/Widgets/WidgetIndiciProduttivitaWS.asmx/readAvailableYearsIndiciProduttivita", JsonConvert.SerializeObject(request));
            string str = operazioni.ToString();
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<List<int>>>(str).RispostaStringa;

            return new RispostaStandard<List<int>>()
            {
                RispostaOK = true,
                RispostaStringa = risposta
            };
        }

        public RispostaStandard<List<WidgetKPI>> readWidgetKPI(CoreWSRequest<CoreWS_Generic<WidgetRequestKpi>> request)
        {
            JObject operazioni = Request(HttpMethod.Post, "/Widgets/WidgetIndiciProduttivitaWS.asmx/readKPI", JsonConvert.SerializeObject(request));
            string str = operazioni.ToString();
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<List<WidgetKPI>>>(str).RispostaStringa;

            return new RispostaStandard<List<WidgetKPI>>()
            {
                RispostaOK = true,
                RispostaStringa = risposta
            };
        }

        public RispostaStandard<Widget_AgroMeteo> ConfigurazioneAgroMeteo(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject operazioni = Request(HttpMethod.Post, "/Widgets/Widgets_Dati.asmx/ConfigurazioneAgroMeteo", JsonConvert.SerializeObject(request));
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<Widget_AgroMeteo>>(operazioni.ToString()).RispostaStringa;

            return new RispostaStandard<Widget_AgroMeteo>()
            {
                RispostaOK = true,
                RispostaStringa = risposta
            };

        }

        public RispostaStandard<Widget_MeteoImpresaLatLng> WeatherLatLng(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            var operazioni = Request(HttpMethod.Post, "/Widgets/Widgets_Dati.asmx/WeatherLatLng", JsonConvert.SerializeObject(request));
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<Widget_MeteoImpresaLatLng>>(operazioni.ToString()).RispostaStringa;

            return new RispostaStandard<Widget_MeteoImpresaLatLng>()
            {
                RispostaOK = true,
                RispostaStringa = risposta
            };
        }

        public RispostaStandard<List<Widget_ProduzioneColtura>> LeggiProduzioneColture(CoreWSRequest<CoreWS_Generic<Widget_Culture_IN>> request)
        {

            JObject operazioni = Request(HttpMethod.Post, "/Widgets/Widgets_Dati.asmx/LeggiProduzioneColture", JsonConvert.SerializeObject(request));
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<List<Widget_ProduzioneColtura>>>(operazioni.ToString()).RispostaStringa;

            return new RispostaStandard<List<Widget_ProduzioneColtura>>()
            {
                RispostaOK = true,
                RispostaStringa = risposta
            };

        }

        public RispostaStandard<string> Widget_LinkGestioneCompleta(CoreWSRequest<CoreWS_Generic<Widget_LinkGestione_IN>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Widgets/Widgets_Dati.asmx/LinkGestioneCompleta", JsonConvert.SerializeObject(request));
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString()).RispostaStringa;

            return new RispostaStandard<string>()
            {
                RispostaOK = true,
                RispostaStringa = risposta
            };

        }

        public RispostaStandard<string> Logout(CoreWSRequest<CoreWS_Generic<List<string>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Utility.asmx/LogoutGias", JsonConvert.SerializeObject(request));
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString()).RispostaStringa;
            return new RispostaStandard<string>()
            {
                RispostaOK = true,
                RispostaStringa = risposta
            };
        }

        public RispostaStandard<object> ModelliPrevisionali_ElaboraIndicatori(CoreWSRequest<CoreWS_Generic<Widget_Modelli_Previsionali_Indicatori_IN>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Meteo/Meteo.asmx/ModelliPrevisionali_ElaboraIndicatori", JsonConvert.SerializeObject(request));
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString()).RispostaStringa;
            return new RispostaStandard<object>()
            {
                RispostaOK = true,
                RispostaStringa = risposta
            };
        }

        public RispostaStandard<List<TurnoConsiglioIrrigazione>> Leggi_Turni_Salvati(CoreWSRequest<CoreWS_Generic<int>> InData)
        {
            JObject result = Request(HttpMethod.Post, "/Meteo/Meteo.asmx/Leggi_Turni_Salvati", JsonConvert.SerializeObject(InData));
            return JsonConvert.DeserializeObject<RispostaStandard<List<TurnoConsiglioIrrigazione>>>(result.ToString());
        }

        public RispostaStandard CercaPioggeIrrigazione(CoreWSRequest<CoreWS_Generic<LeggiRilieviPiogge>> InData)
        {
            JObject result = Request(HttpMethod.Post, "/Meteo/Meteo.asmx/CercaPioggeIrrigazione", JsonConvert.SerializeObject(InData));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard<object> ElaboraMonitoraggioSuolo(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Meteo/Meteo.asmx/ElaboraMonitoraggioSuolo", JsonConvert.SerializeObject(request));
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString()).RispostaStringa;
            return new RispostaStandard<object>()
            {
                RispostaOK = true,
                RispostaStringa = risposta
            };
        }

        public RispostaStandard<object> DatiMeteo_ElaboraRiepilogo(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Meteo/Meteo.asmx/DatiMeteo_ElaboraRiepilogo", JsonConvert.SerializeObject(request));
            var risposta = JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString()).RispostaStringa;
            return new RispostaStandard<object>()
            {
                RispostaOK = true,
                RispostaStringa = risposta
            };
        }

        public RispostaStandard<List<WidgetAcquisto>> LeggiUltimiAcquisti(CoreWSRequest<CoreWS_Generic<Widget_Acquisti_IN>> request)
        {


            JObject visite = Request(HttpMethod.Post, "/Contab/RicercaDocumenti.asmx/Leggi_Top_N_Documenti", JsonConvert.SerializeObject(request));

            var settings = new JsonSerializerSettings() { DateTimeZoneHandling = DateTimeZoneHandling.Local };
            RispostaStandard<List<WidgetAcquisto>> risp = JsonConvert.DeserializeObject<RispostaStandard<List<WidgetAcquisto>>>(visite.ToString(), settings);
            return risp;
        }

        public RispostaStandard<List<Widget_PrevisioniAI>> readStatistichePrevisioniAI(CoreWSRequest<CoreWS_Generic<Widget_PrevisioniAI_IN>> request)
        {

            JObject stats = Request(HttpMethod.Post, "/Widgets/Widgets_Dati.asmx/LeggiStatistichePrevisioniAI", JsonConvert.SerializeObject(request));
            RispostaStandard<List<Widget_PrevisioniAI>> risposta = JsonConvert.DeserializeObject<RispostaStandard<List<Widget_PrevisioniAI>>>(stats.ToString());
            return risposta;

        }

        #endregion

        public RispostaStandard CopiaSpostaAppezzamenti(CoreWSRequest<CoreWS_Generic<CopiaSpostaAppezzamenti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Appezzamento.asmx/CopiaSposta_Appezzamenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard Prodotti_NG(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Anagrafica.Prodotti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/Prodotti_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard Prodotti_x_CAC_NG(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Anagrafica.Prodotti_x_CAC>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/Prodotti_x_CAC_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        #region Visite

        public string Leggi_ListaVisiteDettagli(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Visite.LeggiVisite>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreVisite/Visite.asmx/Leggi_ListaVisiteDettagli_ToKendoGrid_new", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString()).RispostaStringa;
        }

        public RispostaStandard<List<Lavorazione>> getListaVisiteOperazioni(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreVisite/Visite.asmx/LeggiCategorieVisiteOperazioni", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Lavorazione>>>(result.ToString());
        }

        public RispostaStandard<List<AttivitaPersonalizzata>> getListaVisiteAttivita(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreVisite/Visite.asmx/LeggiCategorieVisiteAttivita", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AttivitaPersonalizzata>>>(result.ToString());
        }

        public string LeggiUtenteTecnicoOCapo(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreVisite/Visite.asmx/LeggiUserTecnico", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString()).RispostaStringa;
        }

        public string LeggiListaTecnici(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreVisite/Visite.asmx/LeggiListaTecnici", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString()).RispostaStringa;
        }

        public string LeggiListaAziende(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Visite.LeggiAziende>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreVisite/Visite.asmx/LeggiListaAziende", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString()).RispostaStringa;
        }

        public string LeggiListaAgenzie(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Visite.LeggiAziende>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreVisite/Visite.asmx/LeggiListaAgenzie", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString()).RispostaStringa;
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.attivita.risorse.RisorsaZootecnica>> LeggiRisorseZootecniche(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreVisite/Visite.asmx/LeggiRisorseZootecniche", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.attivita.risorse.RisorsaZootecnica>>>(result.ToString());
        }

        public RispostaStandard EliminaRilieviVisite(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Visite.Elimina_Rilievi_Visita>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreVisite/Visite.asmx/EliminaRilieviVisite", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        #endregion

        public RispostaStandard<List<Vincolo>> LeggiVincoli(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiVincoli>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreDPI/DPI.asmx/CaricaComboVincoli_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Vincolo>>>(result.ToString());
        }

        public RispostaStandard<List<Vincolo>> LeggiVincoliOrdered(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Metaschema.LeggiVincoli>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreDPI/DPI.asmx/CaricaComboVincoliOrdered_Modello", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Vincolo>>>(result.ToString());
        }

        public RispostaStandard<List<AgronicaCoreModelsSTD.attivita.risorse.Prodotto>> Leggi_TrasformatiVegetali_Anagrafica(CoreWSRequest<CoreWS_Generic<InData.Anagrafica.LeggiProdotti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Prodotti.asmx/Leggi_TrasformatiVegetali_Anagrafica", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AgronicaCoreModelsSTD.attivita.risorse.Prodotto>>>(result.ToString());
        }

        public RispostaStandard<List<Contatto>> LeggiContattiMacchina(CoreWSRequest<CoreWS_Generic<LeggiContattiMacchina>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/Leggi_Contatti_Macchine", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Contatto>>>(result.ToString());
        }

        public RispostaStandard<List<Contatto>> LeggiContattiStazioniMeteo(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/Leggi_Contatti_Stazioni_Meteo", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Contatto>>>(result.ToString());
        }

        public RispostaStandard LeggiContattiStazioniMeteoAPP(CoreWS_Contatti request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Contatti.asmx/Leggi_Contatti_Stazioni_Meteo_APP", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard Aggiungi_Al_PUA(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Agenda.AggiungiRicettaAlPUA>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/Aggiungi_Al_PUA", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }


        #region "Valutazioni"

        public RispostaStandard<List<Valutazione_Testata>> getListaValutazioniTestata(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Valutazioni.LeggiTestata>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Leggi_Testata", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Valutazione_Testata>>>(result.ToString());
        }

        public RispostaStandard getListaValutazioniTestataGriglia(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Valutazioni.LeggiTestata>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Leggi_Testata_x_Griglia", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<Valutazione_Testata> ScriviValutazioneTestata(CoreWSRequest<CoreWS_Generic<Valutazione_Testata>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Scrivi_Valutazione_Testata", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Valutazione_Testata>>(result.ToString());
        }

        public RispostaStandard<Valutazione_Dettaglio_Specifico> ScriviValutazioneDettaglioSpecifico(CoreWSRequest<CoreWS_Generic<Valutazione_Dettaglio_Specifico>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Scrivi_Valutazione_Dettaglio_Specifico", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Valutazione_Dettaglio_Specifico>>(result.ToString());
        }

        public RispostaStandard<Valutazione_Scrivi_Griglia> ScriviValutazioneDettaglioGriglia(CoreWSRequest<CoreWS_Generic<LeggiGriglia>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Modifica_Valutazione_Dettaglio_x_Griglia", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Valutazione_Scrivi_Griglia>>(result.ToString());
        }


        public RispostaStandard<Valutazione_Scrivi_Griglia> ScriviValutazioneDettaglioSpecificoGriglia(CoreWSRequest<CoreWS_Generic<LeggiGriglia>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Modifica_Valutazione_Dettaglio_Specifico_x_Griglia", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Valutazione_Scrivi_Griglia>>(result.ToString());
        }




        public RispostaStandard getListaValutazioniDettaglioEcoGriglia(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Valutazioni.LeggiDettaglio>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Leggi_Dettaglio_Eco_x_Griglia", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard getListaValutazioniDettaglioPatGriglia(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Valutazioni.LeggiDettaglio>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Leggi_Dettaglio_Pat_x_Griglia", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<Valutazione_Dettaglio> ScriviValutazioneDettaglio(CoreWSRequest<CoreWS_Generic<Valutazione_Dettaglio>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Scrivi_Valutazione_Dettaglio", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Valutazione_Dettaglio>>(result.ToString());
        }

        public RispostaStandard getListaValutazioniDettaglioSpecificoGriglia(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Valutazioni.LeggiDettaglio>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Leggi_Dettaglio_Specifico_x_Griglia", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<ValutazioneTipoAnno>> LeggiElencoTipoAnnoValutazione(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/LeggiElencoTipoAnno", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<ValutazioneTipoAnno>>>(result.ToString());
        }

        public RispostaStandard<List<Valutazione_Piano_Conti>> getListaValutazioniPianoConti(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Valutazioni.LeggiPianoConti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Leggi_Piano_Conti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Valutazione_Piano_Conti>>>(result.ToString());
        }
        public RispostaStandard getListaValutazioniPianoContiGriglia(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Valutazioni.LeggiPianoConti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Leggi_Piano_Conti_x_Griglia", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<Valutazione_Piano_Conti> ScriviValutazionePianoConti(CoreWSRequest<CoreWS_Generic<Valutazione_Piano_Conti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Scrivi_Piano_Conti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Valutazione_Piano_Conti>>(result.ToString());
        }

        public RispostaStandard<List<TreeValutazionePianoContixConti>> getListaValutazioniPianoContixConti(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Valutazioni.LeggiPianoContixConti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Leggi_Piano_ContixConti_x_Tree", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<TreeValutazionePianoContixConti>>>(result.ToString());
        }
        public RispostaStandard<TreeValutazionePianoContixConti> ScriviValutazionePianoContixConti(CoreWSRequest<CoreWS_Generic<TreeValutazionePianoContixConti>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Scrivi_Piano_ContixConti_x_Tree", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<TreeValutazionePianoContixConti>>(result.ToString());
        }


        public RispostaStandard<Valutazione_Testata> AggiornaValutazioneDettaglio(CoreWSRequest<CoreWS_Generic<LeggiTestata>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Aggiorna_Valutazione_Dettaglio", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Valutazione_Testata>>(result.ToString());
        }

        public RispostaStandard<Valutazione_Testata> AggiornaValutazioneDettaglioSpecifico(CoreWSRequest<CoreWS_Generic<LeggiTestata>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Aggiorna_Valutazione_Dettaglio_Specifico", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Valutazione_Testata>>(result.ToString());
        }

        public RispostaStandard<Valutazione_Testata> AggiornaValutazioneDettaglioSingolo(CoreWSRequest<CoreWS_Generic<LeggiDettaglio>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Aggiorna_Valutazione_Dettaglio_Singolo_Conto", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Valutazione_Testata>>(result.ToString());
        }

        public RispostaStandard<Valutazione_Testata> AggiornaValutazioneDettaglioArete(CoreWSRequest<CoreWS_Generic<LeggiDettaglioSpecificoArete>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Aggiorna_Valutazione_Dettaglio_Arete", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Valutazione_Testata>>(result.ToString());
        }


        public RispostaStandard<List<Valutazione_Conto>> getListaValutazioniConto(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Valutazioni.LeggiConto>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/Leggi_Conti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Valutazione_Conto>>>(result.ToString());
        }

        public RispostaStandard<ValutazioneExcel> ReportExcelValutazione(CoreWSRequest<CoreWS_Generic<LeggiTestata>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Valutazioni/Valutazioni.asmx/ReportExcel", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ValutazioneExcel>>(result.ToString());
        }

        #endregion

        public RispostaStandard<List<UnitaDiMisura_Alternativa>> LeggiConversioniAlt(CoreWS_UnitaMisuraAlternativeLeggi request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/UnitaMisuraAlternativa.asmx/CaricaComboUdmAlternativa", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<UnitaDiMisura_Alternativa>>>(result.ToString());
        }

        public RispostaStandard GetTassoConversioneAlt(CoreWS_ConversioneAltUdmLeggi.LeggiTassoConversione request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/UnitaMisuraAlternativa.asmx/LeggiTassoConversione", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard GetSuperificieEttari(CoreWS_ConversioneAltUdmLeggi.GetSuperficieEttari request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/UnitaMisuraAlternativa.asmx/GetSuperficie_Ettari", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard elimina_operazione_multipla(CoreWSRequest<CoreWS_Generic<elimina_operazione_multipla>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/elimina_operazione_multipla_NG", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<BackgroundLoginModel> LeggiParametriConnessioneDaAccessToken(LoginAccessTokenModel request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/LeggiParametriConnessioneDaAccessToken", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<BackgroundLoginModel>>(result.ToString());
        }

        public RispostaStandard<int> getMinutiValiditaLoginMemorizzato(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaCoreUtentiBIZ/Utenti_R.asmx/getMinutiValiditaLoginMemorizzato", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<int>>(result.ToString());
        }

        public RispostaStandard RegistraNotificheCUAA(CoreWSRequest<CoreWS_Generic<NotificaCUAA>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Notifiche/Notifiche.asmx/NotificaCUAA", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        #region Funzioni Zoo

        public RispostaStandard<List<Stalla>> LeggiStalle(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Zoo.LeggiStalle>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Zoo/ZooAnimali.asmx/CaricaElencoStalle", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<Stalla>>>(result.ToString());
        }

        public RispostaStandard<List<SottogruppoStalla>> LeggiSottogruppiStalla(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Zoo.LeggiSottogruppiStalla>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Zoo/ZooAnimali.asmx/CaricaElencoRaggruppamenti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<SottogruppoStalla>>>(result.ToString());
        }

        public RispostaStandard<List<GiacenzaZoo>> LeggiGiacenzeZoo(CoreWSRequest<CoreWS_Generic<AgronicaCoreDTOStd.InData.Zoo.LeggiGiacenzeZoo>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Zoo/ZooAnimali.asmx/CaricaGiacenzeZoo", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<GiacenzaZoo>>>(result.ToString());
        }

        public RispostaStandard LeggiAnomalieZoo(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Lista_Anomalie.asmx/get_Lista_Anomalie", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard ScriviCapoAnimale(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Zoo/ZooAnimali.asmx/ScriviCapoAnimale", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ScriviOperazioneZoo(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Zoo/ZooAnimali.asmx/ScriviOperazioneZoo", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<PianoDiCampionamento>> LeggiPianiCampionamento(CoreWSRequest<CoreWS_Generic<LeggiPianiCampionamento>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Zoo/ZooAnimali.asmx/LeggiPianiCampionamento", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<PianoDiCampionamento>>>(result.ToString());
        }

        public RispostaStandard ScriviPianoCampionamento(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Zoo/ZooAnimali.asmx/ScriviPianoCampionamento", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard ScriviPianoCampionamentoConSpostamento(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Zoo/ZooAnimali.asmx/ScriviPianoCampionamentoConSpostamento", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard SpostamentoGruppi(CoreWSRequest<CoreWS_Generic<SpostamentoGruppi>> request)
        {
            _hc.Timeout = TimeSpan.FromSeconds(900);
            JObject result = Request(HttpMethod.Post, "/Zoo/ZooAnimali.asmx/SpostamentoGruppi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        #endregion

        public RispostaStandard<AgronicaCoreModelsSTD.metaschema.Pua> Recupera_Pua(CoreWSRequest<CoreWS_Generic<LeggiPUA>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/Recupera_Pua", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreModelsSTD.metaschema.Pua>>(result.ToString());
        }

        public RispostaStandard<decimal> CaricaDisponibilitaAttualeFertilizzante(CoreWSRequest<CoreWS_Generic<LeggiDisponibilitaAttualeFertilizzante>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/CaricaDisponibilitaAttualeFertilizzante", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<decimal>>(result.ToString());
        }

        #region Importazioni
        public Api_Response ImportAttivitaDemetra(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Importazioni/ImportAttivita.asmx/ImportAttivitaDemetra", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<Api_Response>(result.ToString());
        }
        public Api_Response ImportAnalisiTerrenoDemetra(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Importazioni/ImportAnalisiTerreno.asmx/ImportAnalisiTerrenoDemetra", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<Api_Response>(result.ToString());
        }
        public Api_Response ImportFabbricatiDemetra(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Importazioni/ImportFabbricati.asmx/ImportFabbricatiDemetra", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<Api_Response>(result.ToString());
        }

        public Api_Response ImportContattiDemetra(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Importazioni/ImportContatti.asmx/ImportLavoratoriQDCDemetra", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<Api_Response>(result.ToString());
        }

        public Api_Response ImportSquadreDemetra(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Importazioni/ImportSquadre.asmx/ImportSquadreDemetra", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<Api_Response>(result.ToString());
        }

        public Api_Response ImportAttivitaNewAgri(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Importazioni/ImportAttivita.asmx/ImportAttivitaNewAgri", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<Api_Response>(result.ToString());
        }

        #endregion

        public RispostaStandard creaTokenJWT(CoreWSRequest<CoreWS_Generic<CreaTokenJWT_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Provisioning/Provisioning.asmx/CreaTokenJWT", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<CreaTokenJWT_In> refreshTokenJWT(CoreWSRequest<CoreWS_Generic<RefreshTokenJWT_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Provisioning/Provisioning.asmx/RefreshTokenJWT", JsonConvert.SerializeObject(request));
            logInfo("refreshTokenJWT request:" + JsonConvert.SerializeObject(result));
            var response = JsonConvert.DeserializeObject<RispostaStandard<CreaTokenJWT_In>>(result.ToString());
            logInfo("refreshTokenJWT response:" + JsonConvert.SerializeObject(response));
            return response;
        }

        public RispostaStandard<CreaTokenJWT_In> leggiTokenJWT(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Provisioning/Provisioning.asmx/LeggiTokenJWT", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<CreaTokenJWT_In>>(result.ToString());
        }

        #region Codifiche Agea Macchine
        public RispostaStandard<List<BaseCodeDescrStr>> ReadAgeaMachineCodes(CoreWSRequest<CoreWS_Generic<CodificaMacchineAgeaRequest>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Agea.asmx/ReadAgeaMachineCodes", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<BaseCodeDescrStr>>>(result.ToString());
        }
        #endregion

        public RispostaStandard<List<ReachableDBsOUT>> reachableDB(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/IsAlive/IsAlive.asmx/ReachableDB", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<ReachableDBsOUT>>>(result.ToString());
        }

        public RispostaStandard<List<ReachableSiteIN>> reachableSites(CoreWSRequest<CoreWS_Generic<List<string>>> request)
        {
            JObject result = Request(HttpMethod.Post, "/IsAlive/IsAlive.asmx/ReachableSites", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<ReachableSiteIN>>>(result.ToString());
        }

        public RispostaStandard<string> GetAllBundleState(CoreWSRequest<CoreWS_Generic<ExportQdCtoAgeaPaginated>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Agea.asmx/GetAllBundleState", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }

        public RispostaStandard<string> CountBundleState(CoreWSRequest<CoreWS_Generic<ExportQdCtoAgea>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Agea.asmx/CountBundleState", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }

        public RispostaStandard<string> GetLastBundleState(CoreWSRequest<CoreWS_Generic<ExportQdCtoAgea>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Agea.asmx/GetLastBundleState", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }

        public RispostaStandard<string> MapAttivitaToAgea(CoreWSRequest<CoreWS_Generic<ExportQdCtoAgea>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Agea.asmx/MapAttivitaToAgea", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }
        
        public RispostaStandard<string> SendAttivitaToAgea(CoreWSRequest<CoreWS_Generic<ExportQdCtoAgea>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Agea.asmx/SendAttivitaToAgea", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }

        public RispostaStandard<string> ExtractQdCToAgea(CoreWSRequest<CoreWS_Generic<ExportQdCtoAgea>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Agea.asmx/ExtractQdCToAgea", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }        

        public RispostaStandard<string> GetBundleData(CoreWSRequest<CoreWS_Generic<GetBundleData_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Agea.asmx/GetBundleData", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }

        public RispostaStandard<string> ReadBundleLog(CoreWSRequest<CoreWS_Generic<ReadBundleLog_In>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Agea.asmx/ReadBundleLog", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }

        public RispostaStandard<string> CheckTrattamentoFromChatGPT(CoreWSRequest<CoreWS_Generic<CheckTrattamento>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaChatGPT/ChatGPT.asmx/CheckTrattamento", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }

        public RispostaStandard<string> PrevisioniAIFromChatGPT(CoreWSRequest<CoreWS_Generic<PrevisioniChatGPT_IN>> request)
        {
            JObject result = Request(HttpMethod.Post, "/AgronicaChatGPT/ChatGPT.asmx/PrevisioniCostiRicavi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }

        public string ProseguiSelezionati(CoreWSRequest<CoreWS_Generic<ProseguiSelezionati>> request)
        {
            JObject result = Request(HttpMethod.Post, "/FiltroRicerca/FiltroRicerca.asmx/ProseguiSelezionati", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString()).RispostaStringa;
        }

        public RispostaStandard<object> GetCodiceImpianto(CoreWSRequest<CoreWS_Generic<object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/GetCodiceImpianto", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<object>>(result.ToString());
        }
        
        public RispostaStandard<List<object>> GenerateDescriptions(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/GenerateDescriptions", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<object>>>(result.ToString());
        }
        
        public RispostaStandard<string> ClosePlant(CoreWSRequest<CoreWS_Generic<Esercizio>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Anagrafica/Reg_Impianto.asmx/ClosePlant", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }

        #region "Analisi Terreno"
        public RispostaStandard<string> LeggiListaAnalisiTerreno(CoreWSRequest<CoreWS_Generic<LeggiListaAnalisiTerreno>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Analisi/Analisi_Modello.asmx/LeggiListaAnalisiTerreno", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }
        public RispostaStandard<AgronicaCoreModelsSTD.analisi.AnalisiTerreno> LeggiAnalisiTerreno(CoreWSRequest<CoreWS_Generic<LeggiAnalisiTerreno>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Analisi/Analisi_Modello.asmx/LeggiAnalisiTerreno", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<AgronicaCoreModelsSTD.analisi.AnalisiTerreno>>(result.ToString());
        }
        public RispostaStandard<string> ScriviAnalisiTerreno(CoreWSRequest<CoreWS_Generic<ScriviAnalisiTerreno>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Analisi/Analisi_Modello.asmx/ScriviAnalisiTerreno", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }
        public RispostaStandard<bool> ModificaAnalisiTerrenoInLine(CoreWSRequest<CoreWS_Generic<ScriviAnalisiTerreno>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Analisi/Analisi_Modello.asmx/ModificaAnalisiTerrenoInLine", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<bool>>(result.ToString());
        }
        public RispostaStandard<String> CancellaAnalisiTerreno(CoreWSRequest<CoreWS_Generic<ScriviAnalisiTerreno>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Analisi/Analisi_Modello.asmx/CancellaAnalisiTerreno", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }
        public RispostaStandard<String> CancellaListaAnalisiTerreno(CoreWSRequest<CoreWS_Generic<ScriviListaAnalisiTerreno>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Analisi/Analisi_Modello.asmx/CancellaListaAnalisiTerreno", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }
        public RispostaStandard<String> LeggiLaboratori(APICallsBasic request)
        {
            JObject result = Request(HttpMethod.Post, "/Analisi/Analisi_Modello.asmx/LeggiLaboratori", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }
        public RispostaStandard<String> LeggiSchemiAnalisi(CoreWSRequest<CoreWS_Generic<LeggiSchemiAnalisi>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Analisi/Analisi_Modello.asmx/LeggiSchemiAnalisi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }
        /*public RispostaStandard<String> LeggiParametriAnalisi(CoreWSRequest<CoreWS_Generic<LeggiParametriAnalisi>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Analisi/Analisi_Modello.asmx/LeggiParametriAnalisi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }*/
        public RispostaStandard<String> LeggiParametriAnalisi(CoreWSRequest<CoreWS_Generic<LeggiParametriAnalisiDettagli>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Analisi/Analisi_Modello.asmx/LeggiParametriAnalisi", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }
        #endregion

        public RispostaStandard<List<BaseCodeDescr>> Leggi_Modalita_Applicazione_QdC(CoreWSRequest<CoreWS_Generic<Leggi_Modalita_Applicazione>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/Leggi_Modalita_Applicazione_QdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<BaseCodeDescr>>>(result.ToString());
        }

        public RispostaStandard<Object> LeggiAvversitaInnescoQdC(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Metaschema/Avversita.asmx/Leggi_AvversitaInnesco_QdC", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<Object>>(result.ToString());
        }

        public RispostaStandard<string> Controlla_Giacenza_Fertilizzanti(CoreWSRequest<CoreWS_Generic<ScriviListaAttivita>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/Controlla_Giacenza_Fertilizzanti", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<string>>(result.ToString());
        }

        public RispostaStandard CaricaTrappole(CoreWSRequest<CoreWS_Generic<CaricaOperazioni>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/MenuBS_WS.asmx/CaricaTrappole", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard<List<AvversitaTrappole>> Ottieni_Numero_Trappole_Registrate(CoreWSRequest<CoreWS_Generic<Leggi_Numero_Trappole>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Agenda/Agenda.asmx/Ottieni_Numero_Trappole_Registrate", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AvversitaTrappole>>>(result.ToString());
        }

        #region "NewAgri"
        public Api_Response N_Distribuito(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/NewAgri/NewAgri.asmx/N_Distribuito", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<Api_Response>(result.ToString());
        }

        public Api_Response Aggiorna_Impianti_N_Pua(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/NewAgri/NewAgri.asmx/Aggiorna_Impianti_N_Pua", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<Api_Response>(result.ToString());
        }

        public Api_Response ImportUtenteNewAgri(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/NewAgri/NewAgri.asmx/ImportUtenteNewAgri", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<Api_Response>(result.ToString());
        }

        public Api_Response ImportVisibilitaUtenteNewAgri(CoreWSRequest<CoreWS_Generic<Object>> request)
        {
            JObject result = Request(HttpMethod.Post, "/NewAgri/NewAgri.asmx/ImportVisibilitaUtenteNewAgri", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<Api_Response>(result.ToString());
        }

        #endregion

        #region Audit
        public RispostaStandard<ChecklistEUDR> GetChecklistEUDR(CoreWSRequest<CoreWS_Generic<LeggiChecklistEUDR>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Audit/Audit.asmx/GetChecklistEUDR", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ChecklistEUDR>>(result.ToString());
        }
        public RispostaStandard<ChecklistEUDR> PostChecklistEUDR(CoreWSRequest<CoreWS_Generic<ScriviChecklistEUDR>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Audit/Audit.asmx/PostChecklistEUDR", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<ChecklistEUDR>>(result.ToString());
        }
        public RispostaStandard<List<AuditCompletamentoModel>> GetCompletamentoCheckList(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Audit/Audit.asmx/GetCompletamentoCheckList", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard<List<AuditCompletamentoModel>>>(result.ToString());
        }

        public RispostaStandard GetCheckListManagementData(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Audit/Audit.asmx/GetCheckListManagementData", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard SaveChecklistManagement(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Audit/Audit.asmx/SaveChecklistManagement", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }

        public RispostaStandard GetWorkflowManagementData(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Audit/Audit.asmx/GetWorkflowManagementData", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        public RispostaStandard SaveWorkflowManagement(CoreWSRequest<CoreWS_Generic<string>> request)
        {
            JObject result = Request(HttpMethod.Post, "/Audit/Audit.asmx/SaveWorkflowManagement", JsonConvert.SerializeObject(request));
            return JsonConvert.DeserializeObject<RispostaStandard>(result.ToString());
        }
        #endregion
    }
}
