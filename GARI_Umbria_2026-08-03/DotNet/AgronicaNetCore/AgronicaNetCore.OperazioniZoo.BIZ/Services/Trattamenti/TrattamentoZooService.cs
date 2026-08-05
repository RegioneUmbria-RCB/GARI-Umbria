using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.exceptions;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Fabbricati;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impresa;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Farmaci;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Operazione;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.UnitaMisura;
using AgronicaNetCore.Operazione.BIZ.Services.Agenda;
using AgronicaNetCore.Operazione.DAL.DataLayer.Agenda;
using AgronicaNetCore.Operazione.DAL.DataLayer.Mov_Destinazioni;
using AgronicaNetCore.Operazione.DAL.DataLayer.Mov_Dettaglio_Tecnico;
using AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti;
using AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti_Dettagli;
using AgronicaNetCore.Operazione.DAL.DataLayer.Movimenti_Zoo;
using AgronicaNetCore.OperazioniZoo.BIZ.Resources;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.Models;
using AgronicaNetCore.OperazioniZoo.DAL.DataLayer.OperazioniZoo;
using AgronicaNetCore.Zoo.DAL.DataLayer.Prescrizioni;
using AgronicaNetCore.Zoo.DAL.DataLayer.Ricette_Zoo;
using AgronicaNetCore.Zoo.DAL.DataLayer.Ricette_Zoo_Agenda;
using AgronicaNetCore.Zoo.DAL.DataLayer.Ricette_Zoo_Destinazioni;
using AgronicaNetCore.Zoo.DAL.DataLayer.Ricette_Zoo_Dettagli;
using AgronicaNetCore.Zoo.DAL.DataLayer.Ricette_Zoo_Dettaglio_Tecnico;
using AgronicaNetCore.Zoo.DAL.DataLayer.Ricette_Zoo_Movimenti;
using AgronicaNetCore.Zoo.DAL.DataLayer.Ricette_ZooxAgenda;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using InData.Agenda;
using InData.Zoo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using OutData.Kendo;
using OutData.Zoo;
using System.Data;
using static AgronicaCoreModelsSTD.attivita.Attivita;
using static AgronicaNetCore.OperazioniZoo.BIZ.Services.Trattamenti.ITrattamentoZooService;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.Trattamenti
{
    public class TrattamentoZooService : BaseServiceOperazioniZooBIZ, ITrattamentoZooService
    {
        private static readonly DateTime AGRODATAINIZIO = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO);
        private static readonly DateTime AGRODATAFINE = DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE);

        private IMapper _mapper;

        private readonly IFarmaci _farmaciDal;
        private readonly IUnitaMisura _unitaMisuraDal;
        private readonly IOperazione _operazioneDal;
        private readonly IOperazioniZoo _operazioniZooDal;
        private readonly IPrescrizioni _prescrizioniDal;

        private readonly IFabbricati _fabbricatiDal;

        private readonly IRicette_Zoo _ricetteZooDal;
        private readonly IRicette_Zoo_Agenda _ricetteZooAgDal;
        private readonly IRicette_ZooxAgenda _ricetteZooxAgDal;
        private readonly IRicette_Zoo_Movimenti _ricetteZooMovDal;
        private readonly IRicette_Zoo_Dettagli _ricetteZooDettDal;
        private readonly IRicette_Zoo_Destinazioni _ricetteZooDestDal;
        private readonly IRicette_Zoo_Dettaglio_Tecnico _ricetteZooDettTecDal;

        private readonly IAgenda _agendaDal;
        private readonly IAgendaService _agendaBiz;
        private readonly IMovimenti _movimentiDal;
        private readonly IMovimenti_Zoo _movimentiZooDal;
        private readonly IMovimenti_Dettagli _movDettagliDal;
        private readonly IMov_Dettaglio_Tecnico _movDettTecnicoDal;
        private readonly IMov_Destinazioni _movDestinazioniDal;

        public TrattamentoZooService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _mapper = provider.GetRequiredService<IMapper>();

            _farmaciDal = provider.GetRequiredService<IFarmaci>();
            _unitaMisuraDal = provider.GetRequiredService<IUnitaMisura>();
            _operazioneDal = provider.GetRequiredService<IOperazione>();
            _operazioniZooDal = provider.GetRequiredService<IOperazioniZoo>();

            _fabbricatiDal = provider.GetRequiredService<IFabbricati>();

            _ricetteZooDal = provider.GetRequiredService<IRicette_Zoo>();
            _ricetteZooAgDal = provider.GetRequiredService<IRicette_Zoo_Agenda>();
            _ricetteZooMovDal = provider.GetRequiredService<IRicette_Zoo_Movimenti>();
            _ricetteZooDettDal = provider.GetRequiredService<IRicette_Zoo_Dettagli>();
            _ricetteZooDestDal = provider.GetRequiredService<IRicette_Zoo_Destinazioni>();
            _ricetteZooDettTecDal = provider.GetRequiredService<IRicette_Zoo_Dettaglio_Tecnico>();

            _prescrizioniDal = provider.GetRequiredService<IPrescrizioni>();

            _ricetteZooxAgDal = provider.GetRequiredService<IRicette_ZooxAgenda>();
            _agendaDal = provider.GetRequiredService<IAgenda>();
            _agendaBiz = provider.GetRequiredService<IAgendaService>();
            _movimentiDal = provider.GetRequiredService<IMovimenti>();
            _movimentiZooDal = provider.GetRequiredService<IMovimenti_Zoo>();
            _movDettagliDal = provider.GetRequiredService<IMovimenti_Dettagli>();
            _movDettTecnicoDal = provider.GetRequiredService<IMov_Dettaglio_Tecnico>();
            _movDestinazioniDal = provider.GetRequiredService< IMov_Destinazioni> ();
        }

        #region Utilities

        public class CreaTrattamentoDaProtocollo_Resp : ITrattamentoResult
        {
            /// <summary>
            /// Gruppo di Prescrizioni creato dal ribaltamento del Protocollo
            /// </summary>
            public int Gruppo_Prescrizione { get; set; }
            /// <summary>
            /// Prima testata di Prescrizione 
            /// </summary>
            public int Id_Prescrizione { get; set; }
            /// <summary>
            /// Lista delle Somministrazioni create post-ribaltamento
            /// </summary>
            public List<int> Somministrazioni { get; set; }

            public CreaTrattamentoDaProtocollo_Resp()
            {
                Gruppo_Prescrizione = 0;
                Id_Prescrizione = 0;
                Somministrazioni = new List<int>();
            }
        }

        public class CreaTrattamentoDaPrescrizione_Resp : ITrattamentoResult
        {
            /// <summary>
            /// Testata di Prescrizione 
            /// </summary>
            public int Id_Prescrizione { get; set; }
            /// <summary>
            /// Lista delle Somministrazioni create
            /// </summary>
            public List<int> Somministrazioni { get; set; }

            public CreaTrattamentoDaPrescrizione_Resp()
            {
                Id_Prescrizione = 0;
                Somministrazioni = new List<int>();
            }
        }

        public class ScriviModificaSomministrazione_Resp : ITrattamentoResult
        {
            /// <summary>
            /// Gruppo di Prescrizioni (Ricette_Zoo.Gruppo_Ricetta)
            /// </summary>
            public int Gruppo_Prescrizione { get; set; }
            /// <summary>
            /// Id della testata di Prescrizione (Ricette_Zoo.IdRicetta)
            /// </summary>
            public int Id_Prescrizione { get; set; }
            /// <summary>
            /// Id della riga di Prescrizione (Ricette_Zoo_Agenda.IdAgenda)
            /// </summary>
            public int Id_Riga_Prescrizione { get; set; }
            /// <summary>
            /// Id_Agenda della Somministrazione (Agenda.Id_Agenda)
            /// </summary>
            public int Id_Somministrazione { get; set; }

            public ScriviModificaSomministrazione_Resp()
            {
                Gruppo_Prescrizione = 0;
                Id_Prescrizione = 0;
                Id_Riga_Prescrizione = 0;
                Id_Somministrazione = 0;
            }
        }

        /// <summary>
        ///  Utility per Parse di campi string to int
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static int ParseRequiredInt(string objectName, string fieldName, string fieldValue, string errorMessage = "")
        {
            if (string.IsNullOrWhiteSpace(fieldValue))
                throw new GiasException($"Il campo '{fieldName}' dell'oggetto '{objectName}' non è stato valorizzato.{errorMessage}");
            if (!int.TryParse(fieldValue, out int result))
                throw new GiasException($"Il campo '{fieldName}' dell'oggetto '{objectName}' deve essere numerico.{errorMessage}");
            return result;
        }

        private static string GetAlimentoDesFromCod(int alimentoCod)
        {
            return alimentoCod switch
            {
                1 => "CARNE",
                2 => "LATTE",
                _ => string.Empty,
            };
        }

        private static FiltroAggiuntivo GetFiltroAggiuntivoGruppoRic(int Gruppo_Ricetta)
        {
            if (Gruppo_Ricetta == 0)
                throw new ArgumentException($"'{nameof(Gruppo_Ricetta)}' non può essere 0.", nameof(Gruppo_Ricetta));

            FiltroAggiuntivo filtroAggiuntivo = new();
            filtroAggiuntivo.AddFilter("Gruppo_Ricetta", "@gruRic", Comparison_Operators.Equal, Boolean_Operators.First, Gruppo_Ricetta, typeof(int));
            return filtroAggiuntivo;
        }

        private static FiltroAggiuntivo GetFiltroAggiuntivoCauMov(string Cau_Mov) 
        {
            if (string.IsNullOrEmpty(Cau_Mov))
                throw new ArgumentException($"'{nameof(Cau_Mov)}' non può essere null o vuoto.", nameof(Cau_Mov));

            FiltroAggiuntivo filtroAggiuntivo = new();
            filtroAggiuntivo.AddFilter("Cau_Mov", "@cauMov", Comparison_Operators.Equal, Boolean_Operators.First, Cau_Mov, typeof(string));
            return filtroAggiuntivo;
        }
        
        private static FiltroAggiuntivo GetFiltroAggiuntivoProCod(int Pro_Cod, string lotto) 
        {
            if (Pro_Cod == 0)
                throw new ArgumentException($"'{nameof(Pro_Cod)}' non può essere 0.", nameof(Pro_Cod));

            FiltroAggiuntivo filtroAggiuntivo = new();
            filtroAggiuntivo.defaultBooleanOp = Boolean_Operators.And;
            filtroAggiuntivo.AddFilter("Pro_Cod", "@proCod", Comparison_Operators.Equal, Boolean_Operators.And, Pro_Cod, typeof(int));
            if (lotto != null && lotto != "")
            {
                filtroAggiuntivo.AddFilter("Lotto", "@lotto", Comparison_Operators.Equal, Boolean_Operators.And, lotto, typeof(int));
            }
            return filtroAggiuntivo;
        }
        
        private static FiltroAggiuntivo GetFiltroAggiuntivoDettCod(int Dett_Cod) 
        {
            if (Dett_Cod == 0)
                throw new ArgumentException($"'{nameof(Dett_Cod)}' non può essere 0.", nameof(Dett_Cod));

            FiltroAggiuntivo filtroAggiuntivo = new();
            filtroAggiuntivo.AddFilter("Dett_Cod", "@dettcod", Comparison_Operators.Equal, Boolean_Operators.First, Dett_Cod, typeof(int));
            return filtroAggiuntivo;
        }

        #endregion

        #region Leggi Attivita

        /// <summary>
        /// 
        /// </summary>
        /// <param name="somministrazione"></param>
        /// <param name="listCodAnimale"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task<List<CapoAnimaleCDC>> GetListaCapoAnimaleCdc(Attivita somministrazione, List<int> listCodAnimale, AgronicaCoreParametriServer objP)
        {
            List<CapoAnimaleCDC> animali = new();

            string piva = somministrazione.centroAziendale.primaryKey.partitaIva;
            int saCod = somministrazione.centroAziendale.primaryKey.codice;
            int staNum = somministrazione.fabbricatoCod;

            var dtGiacenze = await _operazioniZooDal.Leggi_GiacenzeAsync(piva, saCod, staNum, 0, 0, somministrazione.inizio, objP, listCod_Animali: listCodAnimale, mostraPesate: true);
            if (dtGiacenze != null && dtGiacenze.Rows.Count > 0)
                foreach (var row in dtGiacenze.AsEnumerable())
                {
                    int codAnimale = (int)row["Cod_Animale"];
                    string matricola = (string)row["Matricola"];
                    string sesso = (string)row["Sesso"];
                    int razCod = (int)row["RAZ_COD"];
                    string razDes = (string)row["RAZ_DES"];
                    int raggrCod = (int)row["Raggruppamento_Cod"];
                    string raggrDes = (string)row["Raggruppamento_Des"];
                    int statoCod = (int)row["Stato_Cod"];
                    string statoDes = (string)row["Stato_Des"];
                    double pesoStimato = row["Incremento_Teorico_Calcolato"] is DBNull ? 0 : Convert.ToDouble(row["Incremento_Teorico_Calcolato"]);
                    DateTime inizio = (DateTime)row["Validita_Inizio"];
                    DateTime fine = (DateTime)row["Validita_Fine"];

                    animali.Add(new CapoAnimaleCDC()
                    {
                        capoAnimale = new(piva, codAnimale, matricola)
                        {
                            sesso = sesso,
                            pesoStimato = pesoStimato,
                            razza = new(razCod, razDes),
                            validita = new(inizio, fine),
                            statiAccrescimento = new List<StatoAccrescimento>()
                            {
                                new(statoCod, statoDes),
                            },
                        },
                        sottogruppoStalla_ingresso = new()
                        {
                            codice = raggrCod,
                            nome = raggrDes,
                        }
                    });
                }

            return animali;
        }

        private async Task<int> GetArrotondamentoPesoFromRZDettagli(int idRigaRicetta, string aic, AgronicaCoreParametriServer objP)
        {
            var dtRicZM = await _ricetteZooMovDal.ReadAsync("", 0, 0, idRigaRicetta, 0, objP);
            if (dtRicZM == null || dtRicZM.Rows.Count == 0)
                return 0;
            var dtosRZM = dtRicZM.AsEnumerable().Select(row => _mapper.Map<WriteRicetteZooMovimenti>(new RicetteZooMovimentiRow(row)));

            var dtosRZDtt = new List<WriteRicetteZooDettagli>();
            foreach (var dtoRZM in dtosRZM)
            {
                var dtRicZDtt = await _ricetteZooDettDal.ReadAsync("", 0, dtoRZM.IdRicetta, dtoRZM.IdAgenda, dtoRZM.IdMov, 0, objP);
                if (dtRicZDtt == null || dtRicZDtt.Rows.Count == 0)
                    continue;
                dtosRZDtt.AddRange(dtRicZDtt.AsEnumerable().Select(row => _mapper.Map<WriteRicetteZooDettagli>(new RicetteZooDettagliRow(row))));
            }
            
            var dtoRZD = dtosRZDtt.FirstOrDefault(x => x.ProdottoAic.Contains(aic));

            return dtoRZD?.Arrotondamento_Peso ?? 0;
        }

        /// <summary>
        /// Verifica se esistono somministrazioni successive alla prima già confermate.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> HasSommSuccessiveConfermate(int idFirstSomm, AgronicaCoreParametriServer objP)
        {
            bool resp = false;

            var dtoFirstSomm = await _ricetteZooDal.GetFirstSomministrazione(idFirstSomm, "", objP)
                ?? throw new GiasException($"Prescrizione {idFirstSomm} legata alla prima Somministrazione non trovata.");
            int gruppoRicetta = dtoFirstSomm.Gruppo_Ricetta ?? 0;

            // Ottiene gli IdRicetta delle Prescrizioni future legate alla prima Somministrazione
            var dtRicZoo = await _ricetteZooDal.ReadByGruppoPresAsync(gruppoRicetta, objP);
            if (dtRicZoo.Rows.Count == 1) return false; // Protocollo con una sola somministrazione

            var idRicettaList = dtRicZoo
                .AsEnumerable()
                .Select(row => (int)row["IdRicetta"])
                .Except(new int[] { idFirstSomm })
                .ToArray();

            if (idRicettaList.Length > 0)
            {
                foreach (int idPres in idRicettaList)
                {
                    var dtRicZooAg = await _ricetteZooAgDal.ReadAsync("", 0, idPres, 0, "", objP);
                    if (dtRicZooAg == null || dtRicZooAg.Rows.Count == 0)
                        throw new GiasException($"Riga della Prescrizione {idPres} (legata alla prima Somministrazione) non trovata.");
                    var dtoRicZA = _mapper.Map<WriteRicetteZooAgenda>(new RicetteZooAgendaRow(dtRicZooAg.Rows[0]));
                    bool isConfirmed = await IsSommConfirmed(idPres, dtoRicZA.IdAgenda, objP);

                    if (isConfirmed) { resp = true; break; }
                }
            }
            else throw new GiasException($"Prescrizioni del gruppo {gruppoRicetta} legate alla prima Somministrazione non trovate.");

            return resp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoAgenda"></param>
        /// <param name="somministrazione"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task PopolaDettagliSomministrazione(WriteAgenda dtoAgenda, Attivita somministrazione, AgronicaCoreParametriServer objP)
        {
            DataTable? dtMovimenti = await _movimentiDal.ReadAsync(dtoAgenda.Piva, dtoAgenda.Id_Agenda, 0, objP, GetFiltroAggiuntivoCauMov(CAU_MOV.CAU_TRATTAMENTO_ZOO));
            if (dtMovimenti == null || dtMovimenti.Rows.Count == 0) return;
            var dtoMovimenti = _mapper.Map<WriteMovimenti>(new MovimentiRow(dtMovimenti.Rows[0]));
            
            DataTable? dtMovZoo = await _movimentiZooDal.ReadAsync(dtoMovimenti.Piva, dtoMovimenti.Id_Agenda, dtoMovimenti.Id_Mov, objP);
            if (dtMovZoo != null && dtMovZoo.Rows.Count > 0)
            {
                var dtoMovZoo = _mapper.Map<WriteMovimentiZoo>(new MovimentiZooRow(dtMovZoo.Rows[0]));
                somministrazione.note = dtoMovZoo.Note ?? string.Empty;
            }

            DataTable? dtMovDett = await _movDettagliDal.ReadAsync(dtoMovimenti.Piva, dtoMovimenti.Id_Agenda, dtoMovimenti.Id_Mov, 0, objP);
            if (dtMovDett == null || dtMovDett.Rows.Count == 0) return;
            var dtosMovDett = dtMovDett.AsEnumerable().Select(row => _mapper.Map<WriteMovDettagli>(new MovDettagliRow(row)));
            
            bool withDiffAICs = dtosMovDett.Select(mdtt => mdtt.Pro_Cod).Distinct().Count() > 1;

            foreach (WriteMovDettagli dtoMovDett in dtosMovDett)
            {
                // Aggiunta dettagli Prodotto della Riga di Prescrizione
                int farmCod = dtoMovDett.Pro_Cod ?? 0;
                string aic = "";
                string farmDes = "";

                if (farmCod != 0)
                {
                    DataTable? dtFarmaco = await _farmaciDal.LeggiFarmaciAsync(farmCod, "", objP);
                    if (dtFarmaco != null && dtFarmaco.Rows.Count > 0)
                    {
                        aic = (string)dtFarmaco.Rows[0]["AIC"];
                        farmDes = $"{(string)dtFarmaco.Rows[0]["Denominazione"]} - {(string)dtFarmaco.Rows[0]["Confezione"]}";
                    }
                }

                FarmacoCategoriaSemplificata[] farmCatSem = Array.Empty<FarmacoCategoriaSemplificata>();
                var dtFarmCatS = await _farmaciDal.ReadFarmacixCategoriaSemplificataAsync(0, aic, 0, 0, objP);
                if (dtFarmCatS != null && dtFarmCatS.Rows.Count > 0)
                {
                    farmCatSem = dtFarmCatS.AsEnumerable()
                        .Select(row => new Tuple<string, string>(row["FarmCatS_Id"].ToString()!, row["FarmCatS_Des"].ToString()!))
                        .Distinct()
                        .Select(tup => new FarmacoCategoriaSemplificata(int.Parse(tup.Item1), tup.Item2))
                        .ToArray();
                }

                int udmCod = dtoMovDett.Udm_Cod ?? 0;
                string udmDes = "";
                string udmSim = "";
                if (udmCod != 0)
                {
                    switch ((enum_UnitaMisura)udmCod)
                    {
                        case enum_UnitaMisura.Grammi:
                            udmDes = "Grammi";
                            udmSim = "g";
                            break;
                        case enum_UnitaMisura.Millilitri:
                            udmDes = "Millilitri";
                            udmSim = "ml";
                            break;
                        case enum_UnitaMisura.KG:
                            udmDes = "Chilogrammi";
                            udmSim = "kg";
                            break;
                        case enum_UnitaMisura.Litri:
                            udmDes = "Litri";
                            udmSim = "l";
                            break;
                    }
                }

                string trattNumero = dtoMovDett.Rif_Esterno;
                string sommNumero = dtoMovDett.Extra_Str;
                string regScoNum = dtoMovDett.RegSco_Numero;
                int elemCod = dtoMovDett.Elem_Cod ?? 0;
                double qta = dtoMovDett.Qta_Extra_Totale ?? 0;

                var dettaglioSomm = new DettaglioRegistroSomministrazioni()
                {
                    codice = sommNumero,
                    prodotto = new(farmCod, elemCod)
                    {
                        descrizione = farmDes,
                        codice_alfanumerico = aic,
                    },
                    farmacoCatSem = farmCatSem,
                    codiceAIC = aic,
                    numTrattamento = trattNumero,
                    dataPrescrizione = DateTime.Parse(CostantiPersonalizzate.AGRODATAINIZIO),
                    quantitaTotaleReale = (decimal)qta,
                    regSco_Numero = regScoNum,                    
                    unitaDiMisura = new(udmCod)
                    {
                        tipoControllo = new BaseCodeDescr(udmCod, udmDes),
                        simbolo = udmSim,
                    },
                    validita = new(somministrazione.inizio, somministrazione.fine),
                    durataTrattamento = (int)(somministrazione.fine - somministrazione.inizio).TotalDays + 1,
                };

                DataTable? dtoMovDettTec = await _movDettTecnicoDal.ReadAsync(dtoMovDett.Piva, dtoMovDett.Id_Agenda, dtoMovDett.Id_Mov, dtoMovDett.Id_Mov_Det, 0, objP);
                if (dtoMovDettTec != null && dtoMovDettTec.Rows.Count > 0)
                {
                    List<TempiSospensione> tempiSospensione = new();
                    foreach (var dtoMovDte in dtoMovDettTec.AsEnumerable().Select(row => _mapper.Map<WriteMovDettTecnico>(new MovDettTecnicoRow(row))))
                    {
                        int alimCod = dtoMovDte.Dett_Cod ?? 0;
                        int durataSosp = dtoMovDte.Extra_Int ?? 0;
                        tempiSospensione.Add(new TempiSospensione()
                        {
                            Alimento = new BaseCodeDescr(alimCod, GetAlimentoDesFromCod(alimCod)),
                            tempoSospensione = durataSosp,
                        });
                    }
                    dettaglioSomm.sospensione = tempiSospensione.ToArray();
                }

                somministrazione.risorse.Add(dettaglioSomm);

                await AddDettagliProtocolloToSomministrazione(dtoAgenda.Id_Agenda, somministrazione, aic, objP);

                DataTable? dtMovDest = await _movDestinazioniDal.ReadAsync(dtoMovDett.Piva, dtoMovDett.Id_Agenda, dtoMovDett.Id_Mov, dtoMovDett.Id_Mov_Det, 0, objP);
                if (dtMovDest != null && dtMovDest.Rows.Count > 0)
                {
                    List<int> listaCodAnimale = dtMovDest.AsEnumerable()
                        .Select(row => _mapper.Map<WriteMovDestinazioni>(new MovDestinazioniRow(row)))
                        .Select(dto => dto.Id_Destinazione).ToList();

                    // Lettura capi da Zoo_Animali
                    var capiAnimale = await GetListaCapoAnimaleCdc(somministrazione, listaCodAnimale, objP);
                    foreach (var dtoDest in dtMovDest.AsEnumerable().Select(row => _mapper.Map<WriteMovDestinazioni>(new MovDestinazioniRow(row))))
                    {
                        if (withDiffAICs 
                            && somministrazione.centriDiCosto
                                .Where(cdc => cdc is CapoAnimaleCDC)
                                .Cast<CapoAnimaleCDC>()
                                .Any(c => c.capoAnimale.codice == dtoDest.Id_Destinazione))
                        {
                            var capoCdc = somministrazione.centriDiCosto
                                .Where(cdc => cdc is CapoAnimaleCDC)
                                .Cast<CapoAnimaleCDC>().FirstOrDefault(c => c.capoAnimale.codice == dtoDest.Id_Destinazione);
                            if (capoCdc != null) capoCdc.qtaSomministrata += dtoDest.Qta ?? 0;
                        }
                        else
                        {
                            var capoCdc = capiAnimale.Find(capoCdc => capoCdc.capoAnimale.codice == dtoDest.Id_Destinazione);
                            if (capoCdc != null) capoCdc.qtaSomministrata = dtoDest.Qta ?? 0;
                            somministrazione.centriDiCosto.Add(capoCdc);
                        }
                    }

                    //somministrazione.centriDiCosto.AddRange(capiAnimale);
                }
            }
        }

        private async Task AddDettagliProtocolloToSomministrazione(int idAgenda, Attivita somministrazione, string aic, AgronicaCoreParametriServer objP)
        {
            DettaglioRegistroSomministrazioni? dettSomm = somministrazione.risorse
                .OfType<DettaglioRegistroSomministrazioni>().FirstOrDefault()
                ?? throw new GiasException("Errore nell'ottenere DettaglioRegistroSomministrazioni all'interno delle Risorse nell'oggetto Attivita.");

            DataTable? dtRicetteZxAgenda = await _ricetteZooxAgDal.ReadAsync(0, 0, idAgenda, objP);
            if (dtRicetteZxAgenda == null || dtRicetteZxAgenda.Rows.Count != 1) return;

            int idRicetta = (int)dtRicetteZxAgenda.Rows[0]["Id_Ricetta"];
            int idRigaRicetta = (int)dtRicetteZxAgenda.Rows[0]["Id_RigaRicetta"];

            var dtRicZoo = await _ricetteZooDal.ReadAsync("", 0, 0, idRicetta, "", objP);
            if (dtRicZoo == null || dtRicZoo.Rows.Count == 0)
                throw new GiasException($"Testata della Prescrizione {idRicetta} non trovata.");
            else if (dtRicZoo.Rows.Count > 1)
                throw new GiasException($"Trovate più di una Testata di Prescrizione con IdRicetta {idRicetta}.");
            var dtoRicZoo = _mapper.Map<WriteRicetteZoo>(new RicetteZooRow(dtRicZoo.Rows[0]));

            somministrazione.codiceOperazioneRicetta = idRigaRicetta.ToString();
            
            var tipoPres = (enum_TipoPrescrizione)(dtoRicZoo.TipoCodice ?? 0);
            dettSomm.prescrizioneOrigine = (int)tipoPres;
            
            bool isProtocollo = (tipoPres == enum_TipoPrescrizione.Protocollo_Terapeutico || tipoPres == enum_TipoPrescrizione.Da_Protocollo_GIAS);
            dettSomm.isFirstSomm = isProtocollo && await _ricetteZooAgDal.IsFirstSomm(idRicetta, idRigaRicetta, "", objP);
            dettSomm.hasSuccessiveConfermate = isProtocollo && await HasSommSuccessiveConfermate(idRicetta, objP);

            var dtRzM = await _ricetteZooMovDal.ReadAsync("", 0, idRicetta, idRigaRicetta, 0, objP);
            if (dtRzM == null || dtRzM.Rows.Count == 0) throw new GiasException("Record di Ricette_Zoo_Movimenti legato alla Somministrazione non trovato.");
            var dtosRzM = dtRzM.AsEnumerable().Select(row => _mapper.Map<WriteRicetteZooMovimenti>(new RicetteZooMovimentiRow(row)));

            var dtosRZDtt = new List<WriteRicetteZooDettagli>();
            foreach (var dtoRZM in dtosRzM)
            {
                var dtRicZDtt = await _ricetteZooDettDal.ReadAsync("", 0, dtoRZM.IdRicetta, dtoRZM.IdAgenda, dtoRZM.IdMov, 0, objP);
                if (dtRicZDtt == null || dtRicZDtt.Rows.Count == 0) continue;
                dtosRZDtt.AddRange(dtRicZDtt.AsEnumerable().Select(row => _mapper.Map<WriteRicetteZooDettagli>(new RicetteZooDettagliRow(row))));
            }

            var dtoRZD = dtosRZDtt.FirstOrDefault(x => x.ProdottoAic.Contains(aic.Substring(0,6))) ?? throw new GiasException("Record di Ricette_Zoo_Dettagli legato alla Somministrazione non trovato.");

            dettSomm.massivo = (dtoRZD.Massivo ?? 0) == 1;
            dettSomm.qtaDose = dtoRZD.Qta_Dose ?? 0;
            dettSomm.udmDose = dtoRZD.Udm_Dose ?? 0;
            dettSomm.arrotondamentoPeso = dtoRZD.Arrotondamento_Peso ?? 0;
        }

        private async Task PopolaDettagliScaricoProdotti(string piva, int idAgenda, Attivita somministrazione, AgronicaCoreParametriServer objP)
        {
            DataTable? dtMovimenti = await _movimentiDal.ReadAsync(piva, idAgenda, 0, objP, GetFiltroAggiuntivoCauMov(CAU_MOV.CAU_SCARICO));
            if (dtMovimenti == null || dtMovimenti.Rows.Count == 0)
                return;

            int idMov = (int)dtMovimenti.Rows[0]["Id_Mov"];

            DataTable? dtMovDett = await _movDettagliDal.ReadAsync(piva, idAgenda, idMov, 0, objP);
            if (dtMovDett == null || dtMovDett.Rows.Count == 0)
                return;

            foreach (var dtoMovDett in dtMovDett.AsEnumerable().Select(row => _mapper.Map<WriteMovDettagli>(new MovDettagliRow(row))))
            {
                int idMovDet = dtoMovDett.Id_Mov_Det;

                DataTable? dtMovDest = await _movDestinazioniDal.ReadAsync(piva, idAgenda, idMov, idMovDet, 0, objP);
                if (dtMovDest == null || dtMovDest.Rows.Count == 0)
                    return;

                string Piva_Magazzino = "";
                int Sa_Cod_Magazzino = 0;
                int Fabbricato_Cod_Magazzino = 0;
                string Fabbricato_Des_Magazzino = "";

                if ((int)dtMovDest.Rows[0]["Tipo_Destinazione"] == TIPO_DESTINAZIONE.TIPO_DESTINAZIONE_MAGAZZINO)
                {
                    Piva_Magazzino = (string)dtMovDest.Rows[0]["Piva"];
                    Sa_Cod_Magazzino = (int)dtMovDest.Rows[0]["Sa_Cod"];
                    Fabbricato_Cod_Magazzino = (int)dtMovDest.Rows[0]["Id_Destinazione"];
                    var dtMagazzino = await _fabbricatiDal.ReadAsync(Piva_Magazzino, Sa_Cod_Magazzino, Fabbricato_Cod_Magazzino, objP);
                    if (dtMagazzino == null || dtMagazzino.Rows.Count > 0)
                        Fabbricato_Des_Magazzino = (string)dtMagazzino.Rows[0]["Fabbricato_Des"];
                }

                // Aggiunta dettagli Prodotto della Riga di Prescrizione
                int farmCod = dtoMovDett.Pro_Cod ?? 0;
                string aic = "";
                DataTable? dtFarmaco = await _farmaciDal.LeggiFarmaciAsync(farmCod, "", objP);
                if(dtFarmaco != null && dtFarmaco.Rows.Count > 0)
                    aic = (string)dtFarmaco.Rows[0]["AIC"];

                string trattNumero = dtoMovDett.Rif_Esterno;
                string sommNumero = dtoMovDett.Extra_Str;
                string regScoNum = dtoMovDett.RegSco_Numero;
                int udmCod = dtoMovDett.Udm_Cod ?? 0;
                string udmDes = "";
                string udmSim = "";
                DataTable? Udm = await _unitaMisuraDal.LeggiAsync(udmCod, 0, objP);
                if (Udm != null && Udm.Rows.Count > 0)
                {
                    udmDes = (string)Udm.Rows[0]["UDM_DES"];
                    udmSim = (string)Udm.Rows[0]["UDM_SIM"];
                }

                int elemCod = dtoMovDett.Elem_Cod ?? 0;
                decimal qta = decimal.Parse(dtoMovDett.Qta_Extra_Totale?.ToString() ?? throw new ArgumentNullException("Qta_Extra_Totale is null"));

                var scaricoProd = new RisorsaProdotto()
                {
                    prodotto = new(farmCod, elemCod)
                    {
                        tipo = new TipoRisorsa(elemCod),
                        codice_alfanumerico = aic,
                    },
                    quantitaTotaleReale = qta,
                    unitaDiMisura = new UnitaDiMisura(udmCod)
                    {
                        descrizione = udmDes,
                        simbolo = udmSim
                    },
                    MagazziniMovimentazioni = new()
                    {
                        new()
                        {
                            Magazzino = new()
                            {
                                primaryKey = new Fabbricato.PK(Piva_Magazzino, Sa_Cod_Magazzino, Fabbricato_Cod_Magazzino),
                                tipo = (int)enum_TipoFabbricato.Magazzino_Aziendale,                                
                                descrizione = Fabbricato_Des_Magazzino
                            },
                            Qta = qta,
                            QtaTot = qta,
                            Lotto = dtoMovDett.Lotto,
                        }
                    },
                };
                somministrazione.risorse.Add(scaricoProd);
            }
        }

        /// <summary>
        /// Converte una riga di Agenda (Somministrazione) in un oggetto Attivita Zoo 
        /// </summary>
        /// <param name="idAgenda"></param>
        /// <param name="dtoAgenda"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task<Attivita> DtAgendaToAttivita(int idAgenda, WriteAgenda dtoAgenda, AgronicaCoreParametriServer objP)
        {
            string piva = dtoAgenda.Piva;
            DateTime dataInizio = dtoAgenda.Validita_Inizio;
            DateTime dataFine = dtoAgenda.Validita_Fine;

            Attivita somministrazione = new()
            {
                codice = idAgenda.ToString(),
                tipo = Tipo_Attivita.QuadernoDiCampagna,
                job = new Zootecnia(LAV_COD.LAVCOD_CUREMEDICAMENTI_ANIMALI, "Cure e Medicamenti"),
                centroAziendale = new CentroAziendale(new CentroAziendale.PK(dtoAgenda.Sa_Cod ?? 0, piva)),
                fabbricatoCod = dtoAgenda.Sta_Num ?? 0,
                inizio = dataInizio,
                fine = dataFine,
                risorse = new List<Risorsa>(),
                centriDiCosto = new List<CentroDiCosto>(),
            };

            await PopolaDettagliSomministrazione(dtoAgenda, somministrazione, objP);
            await PopolaDettagliScaricoProdotti(piva, idAgenda, somministrazione, objP);

            return somministrazione;
        }

        /// <summary>
        /// Popola l'oggetto Attivita a partire da un Trattamento Zoo
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="idAgenda">Agenda.Id_Agenda</param>
        /// <param name="objP"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<Attivita> PopulateAttivitaZooFromAgenda(string piva, int idAgenda, AgronicaCoreParametriServer objP)
        {
            DataTable? dtAgenda = await _agendaDal.ReadAsync(piva, idAgenda, objP);

            if (dtAgenda == null || dtAgenda.Rows.Count == 0)
                throw new GiasException("Operazione non trovata. Impossibile generare l'oggetto Attivita.");
            if (dtAgenda.Rows.Count != 1)
                throw new GiasException("Piu' di un'Operazione trovata. Impossibile generare l'oggetto Attivita.");

            Attivita attivita = await DtAgendaToAttivita(idAgenda, _mapper.Map<WriteAgenda>(new AgendaRow(dtAgenda.Rows[0])), objP);

            return attivita;
        }

        public async Task<Attivita?> GetAttivitaFromAgenda(string Piva, int Id_Agenda, AgronicaCoreParametriServer objP)
        {
            Attivita? res = null;

            try
            {
                res = await PopulateAttivitaZooFromAgenda(Piva, Id_Agenda, objP);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
            }

            return res;
        }

        #endregion

        #region Scrittura Attivita

        #region Crea Prescrizione GIAS

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoProtRZDS"></param>
        /// <param name="dtoIndTerapRZD"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        //private async Task<WriteRicetteZooDestinazioni> CreateRicZooDestinazioniFromProtocollo(WriteRicetteZooDestinazioni dtoProtRZDS, WriteRicetteZooDettagli dtoIndTerapRZD, AgronicaCoreParametri objP)
        //{
        //    var dtoIndTerapRZDS = _mapper.Map<WriteRicetteZooDestinazioni>(dtoProtRZDS);

        //    dtoIndTerapRZDS.IdRicetta = dtoIndTerapRZD.IdRicetta;
        //    dtoIndTerapRZDS.IdAgenda = dtoIndTerapRZD.IdAgenda;
        //    dtoIndTerapRZDS.IdMov = dtoIndTerapRZD.IdMov;
        //    dtoIndTerapRZDS.IdDettaglio = dtoIndTerapRZD.IdDettaglio;
        //    dtoIndTerapRZDS.IdDestinazione = 0;

        //    dtoIndTerapRZDS.IdDestinazione = await _ricetteZooDestBiz.ScriviModificaAsync(dtoIndTerapRZDS, objP);

        //    return dtoIndTerapRZDS;
        //}
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoProtRZDTT"></param>
        /// <param name="dtoIndTerapRZD"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task<WriteRicetteZooDettaglioTecnico> CreateRicZooDettaglioTecnicoFromProtocollo(WriteRicetteZooDettaglioTecnico dtoProtRZDTT, WriteRicetteZooDettagli dtoIndTerapRZD, AgronicaCoreParametriServer objP)
        {
            var dtoIndTerapRZDTT = _mapper.Map<WriteRicetteZooDettaglioTecnico>(dtoProtRZDTT);

            dtoIndTerapRZDTT.Id_Ricetta = dtoIndTerapRZD.IdRicetta;
            dtoIndTerapRZDTT.Id_Agenda = dtoIndTerapRZD.IdAgenda;
            dtoIndTerapRZDTT.Id_Mov = dtoIndTerapRZD.IdMov;
            dtoIndTerapRZDTT.Id_Mov_Det = dtoIndTerapRZD.IdDettaglio;
            dtoIndTerapRZDTT.Id_Reg_Dettaglio = 0;

            dtoIndTerapRZDTT.Id_Reg_Dettaglio = await _ricetteZooDettTecDal.ScriviModificaAsync(dtoIndTerapRZDTT, objP);

            return dtoIndTerapRZDTT;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoProtRZD"></param>   
        /// <param name="dtoIndTerapRZM"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task<WriteRicetteZooDettagli> CreateRicZooDettagliFromProtocollo(WriteRicetteZooDettagli dtoProtRZD, WriteRicetteZooMovimenti dtoIndTerapRZM, AgronicaCoreParametriServer objP)
        {
            var dtoIndTerapRZD = _mapper.Map<WriteRicetteZooDettagli>(dtoProtRZD);

            dtoIndTerapRZD.IdRicetta = dtoIndTerapRZM.IdRicetta;
            dtoIndTerapRZD.IdAgenda = dtoIndTerapRZM.IdAgenda;
            dtoIndTerapRZD.IdMov = dtoIndTerapRZM.IdMov;
            dtoIndTerapRZD.IdDettaglio = 0;

            dtoIndTerapRZD.IdDettaglio = await _ricetteZooDettDal.ScriviModificaAsync(dtoIndTerapRZD, objP);

            //var dtRicZooDest = await _ricetteZooDestDal.ReadAsync("", 0, dtoProtRZD.IdRicetta, dtoProtRZD.IdAgenda, dtoProtRZD.IdMov, dtoProtRZD.IdDettaglio, 0, objP);
            //if (dtRicZooDest != null && dtRicZooDest.Rows.Count > 0)
            //    foreach (DataRow rowRZDS in dtRicZooDest.Rows)
            //    {
            //        var dtoProtRZDS = _mapper.Map<WriteRicetteZooDestinazioni>(new RicetteZooDestinazioniRow(rowRZDS));
            //        var dtoIndTerapRZDS = await CreateRicZooDestinazioniFromProtocollo(dtoProtRZDS, dtoIndTerapRZD, objP);
            //    }

            var dtRicZooDettTec = await _ricetteZooDettTecDal.ReadAsync("", 0, dtoProtRZD.IdRicetta, dtoProtRZD.IdAgenda, dtoProtRZD.IdMov, dtoProtRZD.IdDettaglio, 0, objP);
            if (dtRicZooDettTec != null && dtRicZooDettTec.Rows.Count > 0)
                foreach (DataRow rowRZDTT in dtRicZooDettTec.Rows)
                {
                    var dtoProtRZDTT = _mapper.Map<WriteRicetteZooDettaglioTecnico>(new RicetteZooDettTecnicoRow(rowRZDTT));
                    var dtoIndTerapRZDTT = await CreateRicZooDettaglioTecnicoFromProtocollo(dtoProtRZDTT, dtoIndTerapRZD, objP);
                }

            return dtoIndTerapRZD;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoProtRZM"></param>
        /// <param name="dtoIndTerapRZA"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task<WriteRicetteZooMovimenti> CreateRicZooMovimentiFromProtocollo(WriteRicetteZooMovimenti dtoProtRZM, WriteRicetteZooAgenda dtoIndTerapRZA, AgronicaCoreParametriServer objP)
        {
            var dtoIndTerapRZM = _mapper.Map<WriteRicetteZooMovimenti>(dtoProtRZM);

            dtoIndTerapRZM.IdRicetta = dtoIndTerapRZA.IdRicetta;
            dtoIndTerapRZM.IdAgenda = dtoIndTerapRZA.IdAgenda;
            dtoIndTerapRZM.IdMov = 0;

            dtoIndTerapRZM.IdMov = await _ricetteZooMovDal.ScriviModificaAsync(dtoIndTerapRZM, objP);

            var dtRicZooDett = await _ricetteZooDettDal.ReadAsync("", 0, dtoProtRZM.IdRicetta, dtoProtRZM.IdAgenda, dtoProtRZM.IdMov, 0, objP);
            if (dtRicZooDett != null && dtRicZooDett.Rows.Count > 0)
                foreach (DataRow rowRZD in dtRicZooDett.Rows)
                {
                    var dtoProtRZD = _mapper.Map<WriteRicetteZooDettagli>(new RicetteZooDettagliRow(rowRZD));
                    var dtoIndTerapRZD = await CreateRicZooDettagliFromProtocollo(dtoProtRZD, dtoIndTerapRZM, objP);
                }   

            return dtoIndTerapRZM;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoProtRZA"></param>
        /// <param name="idIndTerap"></param>
        /// <param name="sommNum">Somministrazione Numero x (in base al numero di Somministrazioni impostate nella Riga di Protocollo)</param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task<WriteRicetteZooAgenda> CreateRicZooAgendaFromProtocollo(WriteRicetteZooAgenda dtoProtRZA, int idIndTerap, int sommNum, AgronicaCoreParametriServer objP)
        {
            var dtoIndTerapRZA = _mapper.Map<WriteRicetteZooAgenda>(dtoProtRZA);
            dtoIndTerapRZA.IdRicetta = idIndTerap;
            dtoIndTerapRZA.IdAgenda = 0;
            dtoIndTerapRZA.Numero = "";
            dtoIndTerapRZA.Note = sommNum.ToString() + " di " + dtoProtRZA.Numero_Somm.ToString();
            dtoIndTerapRZA.Numero_Somm = sommNum;

            dtoIndTerapRZA.IdAgenda = await _ricetteZooAgDal.ScriviModificaAsync(dtoIndTerapRZA, objP);

            var dtRicZooMov = await _ricetteZooMovDal.ReadAsync("", 0, dtoProtRZA.IdRicetta, dtoProtRZA.IdAgenda, 0, objP);
            if (dtRicZooMov != null && dtRicZooMov.Rows.Count > 0)
                foreach (DataRow rowRZM in dtRicZooMov.Rows)
                {
                    var dtoProtRZM = _mapper.Map<WriteRicetteZooMovimenti>(new RicetteZooMovimentiRow(rowRZM));
                    var dtoIndTerapRZM = await CreateRicZooMovimentiFromProtocollo(dtoProtRZM, dtoIndTerapRZA, objP);
                }

            return dtoIndTerapRZA;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoPresRZA"></param>
        /// <param name="idPres"></param>
        /// <param name="sommNum">Somministrazione Numero x (in base al numero di Somministrazioni impostate nella Riga di Prescrizione)</param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task<WriteRicetteZooAgenda> CreateRicZooAgendaFromPrescrizione(WriteRicetteZooAgenda dtoPresRZA, int idPres, int sommNum, AgronicaCoreParametriServer objP)
        {
            var dtoRZA = _mapper.Map<WriteRicetteZooAgenda>(dtoPresRZA);
            dtoRZA.IdRicetta = idPres;
            dtoRZA.IdAgenda = 0;
            dtoRZA.Numero = dtoPresRZA.Numero;
            dtoRZA.Note = dtoRZA.Note != ""
                ? $"{dtoRZA.Note} ({sommNum} di {dtoPresRZA.Numero_Somm})"
                : $"{sommNum} di {dtoPresRZA.Numero_Somm}";
            dtoRZA.Numero_Somm = sommNum;

            dtoRZA.IdAgenda = await _ricetteZooAgDal.ScriviModificaAsync(dtoRZA, objP);

            var dtRicZooMov = await _ricetteZooMovDal.ReadAsync("", 0, dtoPresRZA.IdRicetta, dtoPresRZA.IdAgenda, 0, objP);
            if (dtRicZooMov != null && dtRicZooMov.Rows.Count > 0)
                foreach (DataRow rowRZM in dtRicZooMov.Rows)
                {
                    var dtoPresRZM = _mapper.Map<WriteRicetteZooMovimenti>(new RicetteZooMovimentiRow(rowRZM));
                    var dtoRZM = await CreateRicZooMovimentiFromProtocollo(dtoPresRZM, dtoRZA, objP);
                }

            return dtoRZA;
        }

        /// <summary>
        /// Scrive una nuova riga di Ricette_Zoo (Indicazione Terapeutica Da Procollo GIAS) partendo da una riga di Ricette_Zoo (Protocollo)
        /// </summary>
        /// <param name="dtoProtocollo"></param>
        /// <param name="idGruppoPres"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<WriteRicetteZoo> CreateRicetteZooFromProtocollo(WriteRicetteZoo dtoProtocollo, int idGruppoPres, AgronicaCoreParametriServer objP)
        {
            var dtoIndTerap = _mapper.Map<WriteRicetteZoo>(dtoProtocollo);

            dtoIndTerap.IdRicetta = 0;
            dtoIndTerap.Numero = "";
            dtoIndTerap.Pin = "";
            dtoIndTerap.StatoCodice = 0;
            dtoIndTerap.TipoCodice = (int)enum_TipoPrescrizione.Da_Protocollo_GIAS;
            dtoIndTerap.Id_Protocollo = dtoProtocollo.IdRicetta;
            dtoIndTerap.ProtocolloCodice = dtoProtocollo.Numero;
            dtoIndTerap.Validita_Inizio = DateTime.Now;
            dtoIndTerap.Gruppo_Ricetta = idGruppoPres;

            dtoIndTerap.IdRicetta = await _ricetteZooDal.ScriviModificaAsync(dtoIndTerap, objP);

            return dtoIndTerap;
        }

        /// <summary>
        /// Scrive una nuova riga di Ricette_Zoo partendo da una riga di Ricette_Zoo (Prescrizione)
        /// </summary>
        /// <param name="dtoPrescrizione"></param>
        /// <param name="idGruppoPres"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<WriteRicetteZoo> CreateRicetteZooFromPrescrizione(WriteRicetteZoo dtoPrescrizione, int idGruppoPres, AgronicaCoreParametriServer objP)
        {
            var dtoIndTerap = _mapper.Map<WriteRicetteZoo>(dtoPrescrizione);

            dtoIndTerap.IdRicetta = 0;
            dtoIndTerap.StatoCodice = (int)enum_StatoTrattamento.Aperto;
            dtoIndTerap.Validita_Inizio = DateTime.Now;
            dtoIndTerap.Gruppo_Ricetta = idGruppoPres;

            dtoIndTerap.IdRicetta = await _ricetteZooDal.ScriviModificaAsync(dtoIndTerap, objP);

            return dtoIndTerap;
        }

        /// <summary>
        /// Ribalta un Protocollo su una nuova Ricetta_Zoo (Tipo = Da Protocollo GIAS)
        /// </summary>
        /// <param name="idProtocollo"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task<WriteRicetteZoo> ProjectProtocolloToIndTerapeutica(int idProtocollo, AgronicaCoreParametriServer objP)
        {
            // IdRicetta della prima Prescrizione da associare alla Somministrazione in creazione
            WriteRicetteZoo dtoFirstPres = new();

            try
            {
                var dtProt = await _ricetteZooDal.ReadAsync("", 0, 0, idProtocollo, "", objP);
                if (dtProt == null || dtProt.Rows.Count == 0)
                    throw new GiasException($"Protocollo {idProtocollo} non trovato.");

                var dtoProtocollo = _mapper.Map<WriteRicetteZoo>(new RicetteZooRow(dtProt.Rows[0]));

                int idGruppoPres = await _ricetteZooDal.NuovoId_GruppoRicettaZoo(objP);

                var dtProtRows = await _ricetteZooAgDal.ReadAsync("", 0, dtoProtocollo.IdRicetta, 0, "", objP);
                if (dtProtRows != null && dtProtRows.Rows.Count == 1)
                    foreach (var dtoRowProt in dtProtRows.AsEnumerable().Select(row => _mapper.Map<WriteRicetteZooAgenda>(new RicetteZooAgendaRow(row))))
                    {
                        for (int sommN = 1; sommN <= dtoRowProt.Numero_Somm; sommN++)
                        {
                            var dtoIndTerap = await CreateRicetteZooFromProtocollo(dtoProtocollo, idGruppoPres, objP);                            
                            if (sommN == 1)
                                dtoFirstPres = _mapper.Map<WriteRicetteZoo>(dtoIndTerap);

                            var dtoRicZooAg = await CreateRicZooAgendaFromProtocollo(dtoRowProt, dtoIndTerap.IdRicetta, sommN, objP);
                        }
                    }
            }
            catch (Exception)
            {
                throw;
            }

            return dtoFirstPres;
        }

        /// <summary>
        /// Gestione divisione qta totale in caso di Indicazione Terapeutica con più Somministrazioni
        /// </summary>
        /// <param name="dtoFirstPres"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task FixQtaSomministrazioniFuture_IndTerap(WriteRicetteZoo dtoFirstPres, AgronicaCoreParametriServer objP)
        {
            // (Di Varano) Per ora si suppone che l'indicazione terapeutica avrà sempre e solo un prodotto

            int idFirstPres = dtoFirstPres.IdRicetta;
            int gruppoPres = dtoFirstPres.Gruppo_Ricetta!.Value;
            
            var dtFirstPresRow = await _ricetteZooAgDal.ReadAsync("", 0, idFirstPres, 0, "", objP);
            if (dtFirstPresRow != null && dtFirstPresRow.Rows.Count > 0) {
                var qtaTot = 0.0f;
                var qtaxSomm = 0.0f;

                var dtoFirstPresRow = _mapper.Map<WriteRicetteZooAgenda>(new RicetteZooAgendaRow(dtFirstPresRow.Rows[0]));
                int idRowFirstPres = dtoFirstPresRow.IdAgenda;
                int numSomm = dtoFirstPresRow.Numero_Somm!.Value;

                var dtFirstPresDett = await _ricetteZooDettDal.ReadAsync("", 0, idFirstPres, idRowFirstPres, 0, 0, objP);
                if (dtFirstPresDett != null && dtFirstPresDett.Rows.Count == 1) {
                    var dtoFirstPresDett = _mapper.Map<WriteRicetteZooDettagli>(new RicetteZooDettagliRow(dtFirstPresDett.Rows[0]));
                    qtaTot = dtoFirstPresDett.Quantitativo!.Value;
                    qtaxSomm = (float)Math.Round(qtaTot / numSomm, MidpointRounding.ToZero);
                }

                double qtaTotFromQtaxSomm = qtaxSomm * numSomm;
                double diffQtaxLastCapo = qtaTotFromQtaxSomm < qtaTot ? qtaTot - qtaTotFromQtaxSomm : 0.0;

                var dtGruppoPres = await _ricetteZooDal.ReadByGruppoPresAsync(gruppoPres, objP);
                if (dtGruppoPres != null && dtGruppoPres.Rows.Count > 1) {
                    var dtosGruppoPres = dtGruppoPres.AsEnumerable()
                        .Select(row => _mapper.Map<WriteRicetteZoo>(new RicetteZooRow(row)))
                        .OrderBy(pres => pres.IdRicetta);
                    int idLastPres = dtosGruppoPres.Last().IdRicetta;
                    
                    foreach (var dtoPres in dtosGruppoPres) 
                    {
                        var dtPresRow = await _ricetteZooAgDal.ReadAsync("", 0, dtoPres.IdRicetta, 0, "", objP);
                        var idPres = dtoPres.IdRicetta;
                        if (dtPresRow != null && dtPresRow.Rows.Count == 1) {
                            var dtoPresRow = _mapper.Map<WriteRicetteZooAgenda>(new RicetteZooAgendaRow(dtPresRow.Rows[0]));
                            int idRowPres = dtoPresRow.IdAgenda;

                            var dtPresDett = await _ricetteZooDettDal.ReadAsync("", 0, idPres, idRowPres, 0, 0, objP);
                            if (dtPresDett != null && dtPresDett.Rows.Count == 1) {
                                var dtoPresDettRZD = _mapper.Map<WriteRicetteZooDettagli>(new RicetteZooDettagliRow(dtPresDett.Rows[0]));
                                
                                dtoPresDettRZD.Udm_Dose = (dtoPresDettRZD.Udm_Cod == (int)enum_UnitaMisura.Millilitri)
                                    ? (int)enum_UnitaMisura.ml_su_Capo : 0;
                                
                                // Se la quantità non può essere equamente distribuita, aggiungo la differenza nell'ultima Somministrazione
                                dtoPresDettRZD.Quantitativo = (dtoPres.IdRicetta == idLastPres) 
                                    ? (float)(qtaxSomm + diffQtaxLastCapo) 
                                    : dtoPresDettRZD.Quantitativo = (float)qtaxSomm;
                                
                                await _ricetteZooDettDal.ScriviModificaAsync(dtoPresDettRZD, objP);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Modifica una Prescrizione per generare il gruppo di Somministrazioni programmate
        /// (la Prescrizione modificata diventa la Prescrizione legata alla prima Somministrazione del gruppo)
        /// </summary>
        /// <param name="idPrescrizione"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task<WriteRicetteZoo> ProjectPrescrizioneForProtocolliInCorso(int idPrescrizione, AgronicaCoreParametriServer objP)
        {
            WriteRicetteZoo dtoFirstPres = new();

            try
            {
                var dtPres = await _ricetteZooDal.ReadAsync("", 0, 0, idPrescrizione, "", objP);
                if (dtPres == null || dtPres.Rows.Count == 0)
                    throw new GiasException($"Protocollo {idPrescrizione} non trovato.");

                dtoFirstPres = _mapper.Map<WriteRicetteZoo>(new RicetteZooRow(dtPres.Rows[0]));

                int idFirstPres = dtoFirstPres.IdRicetta;
                int idGruppoPres = await _ricetteZooDal.NuovoId_GruppoRicettaZoo(objP);

                var dtPresRows = await _ricetteZooAgDal.ReadAsync("", 0, dtoFirstPres.IdRicetta, 0, "", objP);
                if (dtPresRows != null && dtPresRows.Rows.Count == 1)
                {
                    var dtoRowProt = _mapper.Map<WriteRicetteZooAgenda>(new RicetteZooAgendaRow(dtPresRows.Rows[0]));

                    int idRowFirstPres = dtoRowProt.IdAgenda;
                    int numSomm = dtoRowProt.Numero_Somm!.Value;

                    for (int sommN = 2; sommN <= numSomm; sommN++)
                    {
                        var dtoIndTerap = await CreateRicetteZooFromPrescrizione(dtoFirstPres, idGruppoPres, objP);
                        var dtoRicZooAg = await CreateRicZooAgendaFromPrescrizione(dtoRowProt, dtoIndTerap.IdRicetta, sommN, objP);
                    }

                    dtoFirstPres.Gruppo_Ricetta = idGruppoPres;
                    await _ricetteZooDal.ScriviModificaAsync(dtoFirstPres, objP);

                    await FixQtaSomministrazioniFuture_IndTerap(dtoFirstPres, objP);

                    dtoRowProt.Numero_Somm = 1;
                    dtoRowProt.Note = $"1 di {numSomm}";
                    await _ricetteZooAgDal.ScriviModificaAsync(dtoRowProt, objP);
                }

            }
            catch (Exception)
            {
                throw;
            }

            return dtoFirstPres;
        }

        #endregion

        #region Creazione/Modifica Somministrazione

        /// <summary>
        /// 
        /// </summary>
        /// <param name="somministrazione"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task<WriteAgenda> GetDtoAgendaFromAttivitaZoo(Attivita somministrazione, AgronicaCoreParametriServer objP)
        {
            int lavCod = !string.IsNullOrEmpty(somministrazione.job?.primaryKey?.codice) ? int.Parse(somministrazione.job.primaryKey.codice) : 0;
            WriteAgenda dto = new(somministrazione.centroAziendale.primaryKey.partitaIva, somministrazione.centroAziendale.primaryKey.codice, 0)
            {
                Lav_Cod = lavCod,
                Sta_Num = somministrazione.fabbricatoCod,
                Des_Lib = await _operazioneDal.LavorazioneDesFromLavorazioneCodAsync(lavCod, objP),
                Blocco_Data = AGRODATAINIZIO,
                Validita_Inizio = somministrazione.inizio.Date,
                Validita_Fine = somministrazione.inizio.Date,
                Origine = "",
                Inviato = 0
            };

            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="somministrazione">Attivita collegata all'Attivita di tipo Ricetta</param>
        /// <param name="sommNum"></param>
        /// <param name="idAgenda"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task<List<WriteRicetteZooAgenda>> WriteRighePrescrizioneFromSomministrazione(Attivita somministrazione, int gruppoRicetta, int sommNum, int idAgenda, AgronicaCoreParametriServer objP)
        {
            List<WriteRicetteZooAgenda> result = new();

            if (gruppoRicetta == 0)
                throw new GiasException($"Non è stato passato il Gruppo Ricetta. Impossibile proseguire nell'aggiornamento delle Righe di Prescrizione.");

            // Lettura delle testate di Ricetta_Zoo
            var dtRicZoo = await _ricetteZooDal.ReadByGruppoPresAsync(gruppoRicetta, objP);
            if (dtRicZoo == null || dtRicZoo.Rows.Count == 0)
                throw new GiasException($"Testate di Prescrizione non trovate. Impossibile proseguire nell'aggiornamento delle Righe di Prescrizione.");

            List<WriteRicetteZooAgenda> righePrescrizione = new();
            foreach (DataRow row in dtRicZoo.Rows)
            {
                // Lettura Righe della Testata di Prescrizione
                int idRicetta = (int)row["IdRicetta"];
                var dtRicZooAg = await _ricetteZooAgDal.ReadAsync("", 0, idRicetta, 0, "", objP);

                if (dtRicZooAg == null || dtRicZooAg.Rows.Count == 0)
                    throw new GiasException($"Non sono state trovate le Righe della Prescrizione {idRicetta} (Id Gruppo {gruppoRicetta}) da associare alla Somministrazione.");

                righePrescrizione.AddRange(dtRicZooAg.AsEnumerable().Select(row => _mapper.Map<WriteRicetteZooAgenda>(new RicetteZooAgendaRow(row))));
            }

            DateTime lastInsertedDate = somministrazione.inizio.Date;
            // Ordina le Righe di Prescrizione (i possibili gruppi) per Numero_Somm
            foreach (var rowRza in righePrescrizione.OrderBy(row => row.Numero_Somm))
            {
                bool isFirst = rowRza.Numero_Somm == 1;

                int intervalloSomm = rowRza.Intervallo_Somm ?? 0;
                rowRza.DataInizioTrattamento = lastInsertedDate;
                rowRza.DataFineTrattamento = lastInsertedDate;
                rowRza.DurataTrattamento = intervalloSomm;

                int idRigaPres = await _ricetteZooAgDal.ScriviModificaAsync(rowRza, objP);
                rowRza.IdAgenda = idRigaPres;

                lastInsertedDate = lastInsertedDate.AddDays(intervalloSomm);

                // Scrittura del link tra Riga di Prescrizione e Somministrazione per la prima somministrazione (operazione di Agenda) 
                if (isFirst)
                {
                    WriteRicetteZooxAgenda dtoRZxA = new(rowRza.IdRicetta, rowRza.IdAgenda, idAgenda)
                    {
                        Validita_Inizio = somministrazione.inizio,
                        Validita_Fine = somministrazione.fine,
                    };
                    await _ricetteZooxAgDal.ScriviModificaAsync(dtoRZxA, objP);
                }

                result.Add(rowRza);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="somministrazione">Attivita collegata all'Attivita di tipo Ricetta</param>
        /// <param name="dtoAgenda"></param>
        /// <param name="cauMov">In base a questo compone un Movimenti per lo Scarico del Farmaco su Animale o da Magazzino</param>
        /// <returns></returns>
        private static WriteMovimenti GetDtoMovimenti(Attivita somministrazione, WriteAgenda dtoAgenda, int cauMov, string sommNumDes = "")
        {
            WriteMovimenti dto = new(somministrazione.centroAziendale.primaryKey.partitaIva, dtoAgenda.Id_Agenda, 0)
            {
                Mov_Desc = dtoAgenda.Des_Lib,
                Cau_Mov = cauMov,
                Data_Movimento = somministrazione.inizio.Date,
                Scadenza = AGRODATAFINE,
                Data_Registrazione = DateTime.Now, // TODO Sostituire con Data_Movimento??
                Validita_Inizio = somministrazione.inizio,
                Validita_Fine = AGRODATAFINE,
                Extra_Str = sommNumDes
            };

            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="somministrazione">Attivita collegata all'Attivita di tipo Ricetta</param>
        /// <param name="agenda"></param>
        /// <param name="cauMov">In base a questo scrive una riga di Movimenti per lo Scarico del Farmaco su Animale o da Magazzino</param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task<WriteMovimenti> WriteMovimentiFromAttivitaZoo(Attivita somministrazione, WriteAgenda agenda, string cauMov, AgronicaCoreParametriServer objP, string sommNumDes = "")
        {
            WriteMovimenti dtoMov = GetDtoMovimenti(somministrazione, agenda, int.Parse(cauMov), sommNumDes);

            var dtMov = await _movimentiDal.ReadAsync(agenda.Piva, agenda.Id_Agenda, 0, objP, GetFiltroAggiuntivoCauMov(cauMov));
            if(dtMov != null && dtMov.Rows.Count == 1)
                dtoMov.Id_Mov = (int)dtMov.Rows[0]["Id_Mov"];

            int idMov = await _movimentiDal.ScriviModificaAsync(dtoMov, objP);
            dtoMov.Id_Mov = idMov;

            return dtoMov;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRicetta">Ricette_Zoo.IdRicetta</param>
        /// <param name="somministrazione">Attivita collegata all'Attivita di tipo Ricetta</param>
        /// <param name="dtoMov">Movimenti legato allo Scarico del Farmaco su Animale</param>
        /// <param name="objP_Server"></param>
        /// <param name="objP_Utenti"></param>
        /// <returns></returns>
        private async Task<WriteMovimentiZoo> GetDtoMovimentiZooFromAttivitaZoo(int idRicetta, Attivita somministrazione, WriteMovimenti dtoMov, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            int presTipo = 0;
            string presNum = "";
            string presRigaNum = "";

            int idAgenda = dtoMov.Id_Agenda;
            int idRigaRic = int.Parse(somministrazione.codiceOperazioneRicetta);
            string noteSomm = somministrazione.note ?? string.Empty;

            var dtRz = await _prescrizioniDal.ReadPrescrizioniAsync(new(idRicetta), objP_Server, objP_Utenti);
            if (dtRz != null && dtRz.Rows.Count > 0)
            {
                presNum = (string)dtRz.Rows[0]["Numero"];
                presTipo = (int)dtRz.Rows[0]["TipoCodice"];

                var dtRzA = await _prescrizioniDal.ReadRighePrescrizioneAsync(idRicetta, objP_Server);
                if (dtRzA != null && dtRzA.Rows.Count > 0)
                    foreach (var row in dtRzA.Rows)
                        if ((int)dtRzA.Rows[0]["IdAgenda"] == idRigaRic)
                        {
                            presRigaNum = (string)dtRzA.Rows[0]["Numero"];
                            break;
                        }
            }

            WriteMovimentiZoo dto = new(somministrazione.centroAziendale.primaryKey.partitaIva, idAgenda, dtoMov.Id_Mov)
            {
                Pres_Numero = presNum,
                PresRiga_Numero = presRigaNum,
                Tipo_Trattamento = presTipo, 
                Stato_Trattamento = 0, // TODO Aggiungere enum?
                Note = noteSomm,
                Validita_Inizio = somministrazione.inizio,
                Validita_Fine = AGRODATAFINE,
            };

            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRicetta">Ricette_Zoo.IdRicetta</param>
        /// <param name="somministrazione">Attivita collegata all'Attivita di tipo Ricetta</param>
        /// <param name="dtoMov">Movimenti legato allo Scarico del Farmaco su Animale</param>
        /// <param name="objP_Server"></param>
        /// <param name="objP_Utenti"></param>
        /// <returns></returns>
        private async Task<bool> WriteMovimentiZooFromAttivitaZoo(int idRicetta, Attivita somministrazione, WriteMovimenti dtoMov, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            WriteMovimentiZoo dtoMovZ = await GetDtoMovimentiZooFromAttivitaZoo(idRicetta, somministrazione, dtoMov, objP_Server, objP_Utenti);
            return await _movimentiZooDal.ScriviModificaAsync(dtoMovZ, objP_Server);
        }

        /// <summary>
        /// Elimina Movimenti e collegate
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="idAgenda"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task DeleteOldMovimenti(string piva, int idAgenda, AgronicaCoreParametriServer objP)
        {
            var dtMov = await _movimentiDal.ReadAsync(piva, idAgenda, 0, objP);
            var movimentiToDelete = dtMov.AsEnumerable().Select(row => _mapper.Map<WriteMovimenti>(new MovimentiRow(row))).ToList();

            foreach (var dtoMov in movimentiToDelete)
            {
                // Delete Movimenti_Zoo associati
                await DeleteOldMovimentiZoo(piva, idAgenda, dtoMov.Id_Mov, objP);

                // Delete Movimenti_Dettagli associati
                await DeleteOldMovDettagli(piva, idAgenda, dtoMov.Id_Mov, objP);
            }

            if (movimentiToDelete.Any())
                await _movimentiDal.EliminaAsync(movimentiToDelete, objP);
        }

        /// <summary>
        /// Elimina Movimenti_Zoo
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="idAgenda"></param>
        /// <param name="idMov"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task DeleteOldMovimentiZoo(string piva, int idAgenda, int idMov, AgronicaCoreParametriServer objP)
        {
            var dtMovZ = await _movimentiZooDal.ReadAsync(piva, idAgenda, idMov, objP);
            var movimentiZooToDelete = dtMovZ.AsEnumerable().Select(row => _mapper.Map<WriteMovimentiZoo>(new MovimentiZooRow(row))).ToList();
            
            if (movimentiZooToDelete.Any())
                await _movimentiZooDal.EliminaAsync(movimentiZooToDelete, objP);
        }

        /// <summary>
        /// Elimina Movimenti_dettagli e collegate
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="idAgenda"></param>
        /// <param name="idMov"></param>
        /// <param name="objP"></param>
        private async Task DeleteOldMovDettagli(string piva, int idAgenda, int idMov, AgronicaCoreParametriServer objP)            
        {
            var dtMovDett = await _movDettagliDal.ReadAsync(piva, idAgenda, idMov, 0, objP);
            var movDettagliToDelete = dtMovDett.AsEnumerable().Select(row => _mapper.Map<WriteMovDettagli>(new MovDettagliRow(row))).ToList();
            
            foreach (var dtoDett in movDettagliToDelete)
            {
                // Delete Mov_Dettaglio_Tecnico associati
                await DeleteOldMovDettTecnico(piva, idAgenda, idMov, dtoDett.Id_Mov_Det, objP);

                // Delete Mov_Destinazioni associati
                await DeleteOldMovDestinazioni(piva, idAgenda, idMov, dtoDett.Id_Mov_Det, objP);
            }

            if (movDettagliToDelete.Any())
                await _movDettagliDal.EliminaAsync(movDettagliToDelete, objP);
        }

        /// <summary>
        /// Elimina Mov_Destinazioni
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="idAgenda"></param>
        /// <param name="idMov"></param>
        /// <param name="idMovDet"></param>
        /// <param name="objP"></param>
        private async Task DeleteOldMovDestinazioni(string piva, int idAgenda, int idMov, int idMovDet, AgronicaCoreParametriServer objP)
        {
            var dtMovDest = await _movDestinazioniDal.ReadAsync(piva, idAgenda, idMov, idMovDet, 0, objP);
            var movDesToDelete = dtMovDest.AsEnumerable().Select(row => _mapper.Map<WriteMovDestinazioni>(new MovDestinazioniRow(row))).ToList();

            if (movDesToDelete.Count > 0)
                await _movDestinazioniDal.EliminaAsync(movDesToDelete, objP);
        }

        /// <summary>
        /// Elimina Mov_Dettaglio_Tecnici 
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="idAgenda"></param>
        /// <param name="idMov"></param>
        /// <param name="idMovDet"></param>
        /// <param name="objP"></param>
        private async Task DeleteOldMovDettTecnico(string piva, int idAgenda, int idMov, int idMovDet, AgronicaCoreParametriServer objP)
        {
            var dtMovDettTec = await _movDettTecnicoDal.ReadAsync(piva, idAgenda, idMov, idMovDet, 0, objP);
            var movDetTecToDelete = dtMovDettTec.AsEnumerable().Select(row => _mapper.Map<WriteMovDettTecnico>(new MovDettTecnicoRow(row))).ToList();

            if (movDetTecToDelete.Count > 0)
                await _movDettTecnicoDal.EliminaAsync(movDetTecToDelete, objP);
        }

        /// <summary>
        /// Aggiunge i dati mancanti dei Dettagli della Somministrazione alla riga di Ricette_Zoo_Dettagli corrispondente
        /// </summary>
        /// <param name="dettagliSomm"></param>
        /// <param name="dtoRza"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task<WriteRicetteZooDettagli> UpdateRicetteZooFromDettagliSomm(DettaglioRegistroSomministrazioni dettagliSomm, WriteRicetteZooAgenda dtoRza, AgronicaCoreParametriServer objP)
        {
            WriteRicetteZooDettagli? dtoRzdtt = null;

            try
            {
                var dtRzm = await _ricetteZooMovDal.ReadAsync("", 0, dtoRza.IdRicetta, dtoRza.IdAgenda, 0, objP);
                if (dtRzm != null && dtRzm.Rows.Count > 0)
                    foreach (DataRow row in dtRzm.Rows)
                    {
                        var rowRzm = _mapper.Map<WriteRicetteZooMovimenti>(new RicetteZooMovimentiRow(row));

                        var dtRzdtt = await _ricetteZooDettDal.ReadAsync("", 0, rowRzm.IdRicetta, rowRzm.IdAgenda, rowRzm.IdMov, 0, objP);
                        if (dtRzdtt != null && dtRzdtt.Rows.Count > 0)
                        {
                            dtoRzdtt = dtRzdtt.AsEnumerable()
                                    .Select(row => _mapper.Map<WriteRicetteZooDettagli>(new RicetteZooDettagliRow(row)))
                                    .Where(row => row.ProdottoAic != "")
                                    .FirstOrDefault(row => dettagliSomm.codiceAIC.StartsWith(row.ProdottoAic));
                            
                            if (dtoRzdtt != null) break;
                        }
                    }

                if (dtoRzdtt == null)
                    throw new GiasException($"Non è stata trovata il Dettaglio della Riga {dtoRza.IdAgenda} (Prescrizione {dtoRza.IdRicetta}) da associare alla al Dettaglio della Somministrazione.");

                //Scrivo la famiglia del farmaco, non il farmaco specifico
                dtoRzdtt.Pro_Cod = 0;
                dtoRzdtt.ProdottoAic = dettagliSomm.codiceAIC.Substring(0, 6);

                await _ricetteZooDettDal.ScriviModificaAsync(dtoRzdtt, objP);
            }
            catch (Exception)
            {
                throw;
            }

            return dtoRzdtt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="somministrazione">Attivita collegata all'Attivita di tipo Ricetta</param>
        /// <param name="dtoMov">Movimenti legato allo Scarico del Farmaco su Animale</param>
        /// <param name="dettagliSomm"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<WriteMovDettagli> GetDtoMovDettagliFromSomministrazione(Attivita somministrazione, WriteMovimenti dtoMov, DettaglioRegistroSomministrazioni dettagliSomm, AgronicaCoreParametriServer objP)
        {
            int farmCod = dettagliSomm.prodotto.codice;

            var dtFarmaco = await _farmaciDal.LeggiFarmaciAsync(farmCod, "", objP);
            if (dtFarmaco != null && dtFarmaco.Rows.Count > 0)
            {
                //int selUdm;
                //decimal qtaConv;
                int udmCod = dettagliSomm.unitaDiMisura.codice;

                //switch ((enum_UnitaMisura)udmCod)
                //{
                //    case enum_UnitaMisura.Grammi:
                //        selUdm = udmCod;
                //        udmCod = (int)enum_UnitaMisura.KG; 
                //        qtaConv = dettagliSomm.quantitaTotaleReale / 1000;
                //        break;

                //    case enum_UnitaMisura.Millilitri:
                //        selUdm = udmCod;
                //        udmCod = (int)enum_UnitaMisura.Litri;
                //        qtaConv = dettagliSomm.quantitaTotaleReale / 1000;
                //        break;

                //    default:
                //        selUdm = udmCod;
                //        qtaConv = dettagliSomm.quantitaTotaleReale;
                //        break;
                //}

                WriteMovDettagli dto = new(somministrazione.centroAziendale.primaryKey.partitaIva, dtoMov.Id_Agenda, dtoMov.Id_Mov, 0)
                {
                    Pro_Cod = farmCod,
                    Elem_Cod = ELEM_COD.FARMACI,
                    Mov_Det_Des = $"{dtFarmaco.Rows[0]["Denominazione"]} ({dtFarmaco.Rows[0]["Confezione"]} - {dettagliSomm.quantitaTotaleReale})",
                    Udm_Cod = udmCod,
                    Qta = (double)dettagliSomm.quantitaTotaleReale,
                    Qta_Extra_Totale = (double)dettagliSomm.quantitaTotaleReale,
                    Contabilizzato = DETTAGLI_CONTABILI.NON_CONTABILE,
                    RegSco_Numero = dettagliSomm.regSco_Numero,
                    Rif_Esterno = dettagliSomm.numTrattamento, // Tratt_Numero
                    Extra_Str = dettagliSomm.codice, // Somm_Numero
                    Extra_Int = udmCod,
                    Extra_Date = dettagliSomm.dataPrescrizione,
                    Validita_Inizio = AGRODATAINIZIO,
                    Validita_Fine = AGRODATAFINE,
                };

                return dto;
            }
            else
            {
                throw new GiasException("Farmaco non configurato.");
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="somministrazione">Attivita collegata all'Attivita di tipo Ricetta</param>
        /// <param name="dtoMov">Movimenti legato allo Scarico del Farmaco su Animale</param>
        /// <param name="dettagliSomm"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task<WriteMovDettagli> WriteMovDettagliFromSomministrazione(Attivita somministrazione, WriteMovimenti dtoMov, DettaglioRegistroSomministrazioni dettagliSomm, AgronicaCoreParametriServer objP)
        {
            WriteMovDettagli dtoMovDett = await GetDtoMovDettagliFromSomministrazione(somministrazione, dtoMov, dettagliSomm, objP);

            var dtMovDett = await _movDettagliDal.ReadAsync(dtoMovDett.Piva, dtoMovDett.Id_Agenda, dtoMovDett.Id_Mov, 0, objP, 
                GetFiltroAggiuntivoProCod(dtoMovDett.Pro_Cod.GetValueOrDefault(0), dtoMovDett.Lotto));
            if (dtMovDett != null && dtMovDett.Rows.Count == 1)
                dtoMovDett.Id_Mov_Det = (int)dtMovDett.Rows[0]["Id_Mov_Det"];

            int idMovDett = await _movDettagliDal.ScriviModificaAsync(dtoMovDett, objP);
            dtoMovDett.Id_Mov_Det = idMovDett;

            return dtoMovDett;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoMovDett">Movimenti_dettagli legato allo Scarico del Farmaco su Animale</param>
        /// <param name="sospensione"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task<int> WriteMovDettTecnicoFromSospensione(WriteMovDettagli dtoMovDett, TempiSospensione sospensione, AgronicaCoreParametriServer objP)
        {
            WriteMovDettTecnico dtoMovDettTec = new(dtoMovDett.Piva, dtoMovDett.Id_Agenda, dtoMovDett.Id_Mov, dtoMovDett.Id_Mov_Det, 0)
            {
                Dett_Cod = sospensione.Alimento.codice,
                Extra_Int = sospensione.tempoSospensione,
                Validita_Inizio = AGRODATAINIZIO,
                Validita_Fine = AGRODATAFINE
            };

            var dtMovDettTec = await _movDettTecnicoDal.ReadAsync(dtoMovDettTec.Piva, dtoMovDettTec.Id_Agenda, dtoMovDettTec.Id_Mov, dtoMovDettTec.Id_Mov_Det, 0, objP, GetFiltroAggiuntivoDettCod(dtoMovDettTec.Dett_Cod.GetValueOrDefault(0)));
            if (dtMovDettTec != null && dtMovDettTec.Rows.Count == 1)
                dtoMovDettTec.Id_Reg_Dettaglio = (int)dtMovDettTec.Rows[0]["Id_Reg_Dettaglio"];

            int idMovRegDet = await _movDettTecnicoDal.ScriviModificaAsync(dtoMovDettTec, objP);

            return idMovRegDet;
        }

        /// <summary>
        /// Corregge la Quantità Somministrata per Capo Animale in base alla Quantità Totale della Somministrazione e al numero di Capi coinvolti.
        /// Nel caso si utilizzino diversi prodotti (AIC) per lo stesso trattamento, la quantità viene divisa equamente tra i capi.
        /// </summary>
        /// <param name="qtaTot"></param>
        /// <param name="listaCapiCdc"></param>
        /// <param name="withDiffAICs"></param>
        private static void FixQtaSomministrataxCapo(double qtaTot, List<CapoAnimaleCDC> listaCapiCdc, bool withDiffAICs = false)
        {
            if (withDiffAICs)
            {
                double qtaxCapo = Math.Round((qtaTot / listaCapiCdc.Count) * 100 / 100, 4);
                listaCapiCdc.ForEach(c => c.qtaSomministrata = qtaxCapo);
            }

            double qtaTotFromQtaxCapo = Math.Round(listaCapiCdc.Sum(c => c.qtaSomministrata), 2);
            double diffQtaxLastCapo = qtaTotFromQtaxCapo < qtaTot ? Math.Round(qtaTot - qtaTotFromQtaxCapo, 2) : 0.0;
            if (diffQtaxLastCapo > 0)
            {
                var lastCapo = listaCapiCdc.Last();
                lastCapo.qtaSomministrata = Math.Round(lastCapo.qtaSomministrata + diffQtaxLastCapo, 2);
            }
        }

        private async Task<bool> CheckExistingRZDest(WriteRicetteZooDettagli dtoRzdtt, CapoAnimale capo, AgronicaCoreParametriServer objP)
        {
            var dtRzdst = await _ricetteZooDestDal.ReadAsync("", 0, dtoRzdtt.IdRicetta, dtoRzdtt.IdAgenda, dtoRzdtt.IdMov, dtoRzdtt.IdDettaglio, 0, objP);
            if (dtRzdst != null && dtRzdst.Rows.Count > 0) {
                return dtRzdst
                    .AsEnumerable()
                    .Select(row => _mapper.Map<WriteRicetteZooDestinazioni>(new RicetteZooDestinazioniRow(row)))
                    .ToList()
                    .Any(row => row.CodAnimale == capo.codice || row.Matricola == capo.matricola);
            }
         
            return false;
        }

        /// <summary>
        /// Scrive un record di Ricette_Zoo_Destinazioni con i dati del Capo coinvolto per ogni Riga della Prescrizione 
        /// creata (riferimenti alla prima somministrazione e programmate) poiche' al ribaltamento da Protocollo non sono create
        /// </summary>
        /// <param name="dtosRicZooAg"></param>
        /// <param name="capo"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task WriteRicetteZooDestFromCapo(List<WriteRicetteZooAgenda> dtosRicZooAg, CapoAnimale capo, AgronicaCoreParametriServer objP)
        {
            // Righe della Prescrizione ribaltata dal Protocollo
            foreach (WriteRicetteZooAgenda dtoRza in dtosRicZooAg)
            {
                var dtRzm = await _ricetteZooMovDal.ReadAsync(dtoRza.Piva, dtoRza.Sa_Cod ?? 0, dtoRza.IdRicetta, dtoRza.IdAgenda, 0, objP);
                if (dtRzm == null || dtRzm.Rows.Count == 0)
                    throw new GiasException($"Non sono state trovate i record di Ricette_Zoo_Movimenti della Prescrizione {dtoRza.IdRicetta} (Prescrizione ribaltata dal Protocollo di origine).");

                foreach (var dtoRzm in dtRzm.AsEnumerable().Select(row => _mapper.Map<WriteRicetteZooMovimenti>(new RicetteZooMovimentiRow(row))))
                {
                    var dtRzdtt = await _ricetteZooDettDal.ReadAsync(dtoRzm.Piva, dtoRzm.Sa_Cod ?? 0, dtoRzm.IdRicetta, dtoRzm.IdAgenda, dtoRzm.IdMov, 0, objP);
                    if (dtRzdtt == null || dtRzdtt.Rows.Count == 0)
                        throw new GiasException($"Non sono state trovate le righe di Dettagli della Prescrizione {dtoRza.IdRicetta} (Prescrizione ribaltata dal Protocollo di origine).");

                    foreach (var dtoRzdtt in dtRzdtt.AsEnumerable().Select(row => _mapper.Map<WriteRicetteZooDettagli>(new RicetteZooDettagliRow(row))))
                    {
                        // Nel caso di Prescrizioni Veterinarie e Indicazioni Terapeutiche
                        // sono già presenti i record di Ricette_Zoo_Destinazioni se si tratta della prima somministrazione
                        if (await CheckExistingRZDest(dtoRzdtt, capo, objP)) continue;

                        WriteRicetteZooDestinazioni dtoRzdest = new(dtoRzdtt.Piva, dtoRzdtt.Sa_Cod ?? 0, dtoRzdtt.IdRicetta, dtoRzdtt.IdAgenda, dtoRzdtt.IdMov, dtoRzdtt.IdDettaglio)
                        {
                            IdDestinazione = 0,
                            CodAnimale = capo.codice,
                            Matricola = capo.matricola,
                            Sesso = capo.sesso,
                            Validita_Fine = AGRODATAFINE
                        };
                        int idRzdst = await _ricetteZooDestDal.ScriviModificaAsync(dtoRzdest, objP);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="somministrazione">Attivita collegata all'Attivita di tipo Ricetta</param>
        /// <param name="dettagliSomm"></param>
        /// <param name="capoCDC"></param>
        /// <param name="dtoMovDett">Movimenti_dettagli legato allo Scarico del Farmaco su Animale</param>
        /// <param name="dtosRicZooAg"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        private async Task<int> WriteMovDestinazioneFromCapo(Attivita somministrazione, 
            DettaglioRegistroSomministrazioni dettagliSomm, 
            CapoAnimaleCDC capoCDC, 
            WriteMovDettagli dtoMovDett, 
            List<WriteRicetteZooAgenda>? dtosRicZooAg, 
            AgronicaCoreParametriServer objP, 
            bool generateRzDst = true)
        {
            CapoAnimale capo = capoCDC.capoAnimale;
            double sommPercentage = Math.Round(((double)((decimal)capoCDC.qtaSomministrata / (decimal)dtoMovDett.Qta.GetValueOrDefault(0))), 10);

            WriteMovDestinazioni dtoMovDest = new(dtoMovDett.Piva, dtoMovDett.Id_Agenda, dtoMovDett.Id_Mov, dtoMovDett.Id_Mov_Det, capo.codice)
            {
                Tipo_Destinazione = TIPO_DESTINAZIONE.TIPO_DESTINAZIONE_ANIMALE,
                Qta = capoCDC.qtaSomministrata,
                Validita_Inizio = somministrazione.inizio,
                Validita_Fine = AGRODATAFINE,
                Mov_Destinazioni_GraphicKey = capo.matricola,
                QuotaDistribuzione = sommPercentage
            };

            await _movDestinazioniDal.ScriviModificaAsync(dtoMovDest, objP);

            // Caso scrittura prima Somministrazione da Protocollo: necessario aggiungere le righe di Ricette_Zoo_Destinazioni poichè mancanti
            if (generateRzDst && dtosRicZooAg != null)
                await WriteRicetteZooDestFromCapo(dtosRicZooAg, capo, objP);

            return dtoMovDest.Id_Destinazione;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="somministrazione">Attivita collegata all'Attivita di tipo Ricetta</param>
        /// <param name="dtoMov">Movimenti legato allo Scarico del Farmaco da Magazzino</param>
        /// <param name="scaricoProd"></param>
        /// <param name="dettagliSomm">Dettagli della Somministrazioni legati al Farmaco</param>
        /// <param name="movMagazzino">Movimento di Magazzino</param>
        /// <param name="objP_Server"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<WriteMovDettagli> GetDtoMovDettagliFromScaricoProd(Attivita somministrazione, WriteMovimenti dtoMov, RisorsaProdotto scaricoProd, DettaglioRegistroSomministrazioni dettagliSomm, RilevamentoDiMagazzino movMagazzino, AgronicaCoreParametriServer objP_Server)
        {
            int farmCod = scaricoProd.prodotto.codice;
            var dtFarmaco = await _farmaciDal.LeggiFarmaciAsync(farmCod, "", objP_Server);
            if (dtFarmaco != null && dtFarmaco.Rows.Count > 0)
            {
                WriteMovDettagli dto = new(somministrazione.centroAziendale.primaryKey.partitaIva, dtoMov.Id_Agenda, dtoMov.Id_Mov, 0)
                {
                    Sa_Cod = somministrazione.centroAziendale.primaryKey.codice,
                    Pro_Cod = farmCod,
                    Elem_Cod = ELEM_COD.FARMACI,
                    Mov_Det_Des = $"{dtFarmaco.Rows[0]["Denominazione"]} ({dtFarmaco.Rows[0]["Confezione"]} - {movMagazzino.Qta:F2})",
                    Udm_Cod = (int)movMagazzino.udm.codice,
                    Qta = Math.Round((double)movMagazzino.Qta, 0, MidpointRounding.AwayFromZero),
                    Qta_Extra_Totale = Math.Round((double)movMagazzino.QtaTot, 0, MidpointRounding.AwayFromZero),
                    Contabilizzato = DETTAGLI_CONTABILI.NON_CONTABILE,
                    Rif_Esterno = dettagliSomm.numTrattamento, // Tratt_Numero
                    Extra_Str = scaricoProd.prodotto.codice_alfanumerico, // AIC
                    Extra_Int = (int)movMagazzino.QtaTot, // (int)somministrazione.quantitaTotaleReale,
                    Lotto = movMagazzino.Lotto,
                    Validita_Inizio = AGRODATAINIZIO,
                    Validita_Fine = AGRODATAFINE,
                };

                return dto;
            }
            else
            {
                throw new GiasException("Farmaco non configurato.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="somministrazione">Attivita collegata all'Attivita di tipo Ricetta</param>
        /// <param name="dtoMov">Movimenti legato allo Scarico del Farmaco da Magazzino</param>
        /// <param name="scaricoProd">Dettagli della Somministrazioni legati al Farmaco</param>
        /// <param name="dettagliSomm"></param>
        /// <param name="movMagazzino">Movimento di Magazzino</param>
        /// <param name="objP_Server"></param>
        /// <returns></returns>
        private async Task<WriteMovDettagli> WriteMovDettagliFromScaricoProd(Attivita somministrazione, WriteMovimenti dtoMov, RisorsaProdotto scaricoProd, RilevamentoDiMagazzino movMagazzino, DettaglioRegistroSomministrazioni dettagliSomm, AgronicaCoreParametriServer objP_Server)
        {
            WriteMovDettagli dtoMovDett = await GetDtoMovDettagliFromScaricoProd(somministrazione, dtoMov, scaricoProd, dettagliSomm, movMagazzino, objP_Server);

            var dtMovDett = await _movDettagliDal.ReadAsync(dtoMovDett.Piva, dtoMovDett.Id_Agenda, dtoMovDett.Id_Mov, 0, objP_Server, GetFiltroAggiuntivoProCod((int)dtoMovDett.Pro_Cod.GetValueOrDefault(0), (string)dtoMovDett.Lotto));
            if (dtMovDett != null && dtMovDett.Rows.Count == 1)
                dtoMovDett.Id_Mov_Det = (int)dtMovDett.Rows[0]["Id_Mov_Det"];

            int idMovDett = await _movDettagliDal.ScriviModificaAsync(dtoMovDett, objP_Server);
            dtoMovDett.Id_Mov_Det = idMovDett;

            return dtoMovDett;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attivita">Attivita collegata all'Attivita di tipo Ricetta</param>
        /// <param name="dtoMovDett">Movimenti_dettagli legato allo Scarico del Farmaco da Magazzino</param>
        /// <param name="movMagazzino">Movimento di Magazzino</param>
        /// <returns></returns>
        private static WriteMovDestinazioni GetDtoMovDestinazioneFromScaricoProd(Attivita attivita, WriteMovDettagli dtoMovDett, RilevamentoDiMagazzino movMagazzino)
        {
            Fabbricato magazzino = movMagazzino.Magazzino;

            WriteMovDestinazioni dto = new(dtoMovDett.Piva, dtoMovDett.Id_Agenda, dtoMovDett.Id_Mov, dtoMovDett.Id_Mov_Det, magazzino.primaryKey.codice)
            {
                Sa_Cod = magazzino.primaryKey.centroAziendalePK.codice,
                Tipo_Destinazione = magazzino.tipo,
                Qta = Math.Round((double)dtoMovDett.Qta!, 0, MidpointRounding.AwayFromZero),
                Validita_Inizio = attivita.inizio,
                Validita_Fine = AGRODATAFINE,
            };

            return dto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id_Ricetta"></param>
        /// <param name="Id_Riga"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> IsSommConfirmed(int Id_Ricetta, int Id_Riga, AgronicaCoreParametriServer objP)
        {
            bool isConfirmed = false;

            if (Id_Ricetta == 0 || Id_Riga == 0)
                throw new GiasException($"Valorizzare i parametri {nameof(Id_Ricetta)} e {nameof(Id_Riga)}.");

            try
            {
                var dt = await _ricetteZooAgDal.ReadAsync("", 0, Id_Ricetta, Id_Riga, "", objP);

                if (dt == null || dt.Rows.Count == 0)
                    throw new GiasException($"Nessun record trovato con i parametri forniti: {nameof(Id_Ricetta)}={Id_Ricetta}, {nameof(Id_Riga)}={Id_Riga}");
                else if (dt.Rows.Count > 1)
                    throw new GiasException($"Trovate più di una Ricetta con i parametri forniti: {nameof(Id_Ricetta)}={Id_Ricetta}, {nameof(Id_Riga)}={Id_Riga}");

                var dtRicZXA = await _ricetteZooxAgDal.ReadAsync(Id_Ricetta, Id_Riga, 0, objP);
                isConfirmed = (dtRicZXA != null && dtRicZXA.Rows.Count > 0);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }

            return isConfirmed;
        }

        /// <summary>
        /// Verifica se è consentito l'update dei Capi da trattare (aggiunta di Capi).
        /// Al momento è consentito l'aggiornamento dei Capi di Somministrazioni non confermate solo dall'ultima Somministrazione confermata.
        /// </summary>
        /// <param name="idRicetta"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<bool> BloccaUpdateCapiTrattati(int idRicetta, AgronicaCoreParametriServer objP)
        {
            var dtRicZoo = await _ricetteZooDal.ReadAsync("", 0, 0, idRicetta, "", objP)
                ?? throw new GiasException($"Prescrizione {idRicetta} non trovata.");
            int gruppoRicetta = (int)dtRicZoo.Rows[0]["Gruppo_Ricetta"];

            // Ottiene gli IdRicetta delle successive Prescrizioni future legate alla Somministrazione passata
            var dtGruppoPres = await _ricetteZooDal.ReadByGruppoPresAsync(gruppoRicetta, objP) ??
                throw new GiasException($"Prescrizioni del gruppo {gruppoRicetta} legate alla prima Somministrazione non trovate.");
            if (dtGruppoPres.Rows.Count == 1) return false; // Protocollo con una sola somministrazione

            var idRicettaList = dtGruppoPres
                .AsEnumerable()
                .Select(row => (int)row["IdRicetta"])
                .OrderBy(x => x)
                .SkipWhile(x => x <= idRicetta).ToArray();

            if (idRicettaList.Length > 0)
                foreach (int idPres in idRicettaList)
                {
                    var dtRicZooAg = await _ricetteZooAgDal.ReadAsync("", 0, idPres, 0, "", objP);
                    if (dtRicZooAg == null || dtRicZooAg.Rows.Count == 0)
                        throw new GiasException($"Riga della Prescrizione {idPres} (legata alla prima Somministrazione) non trovata.");
                    
                    int idRigaPres = (int)dtRicZooAg.Rows[0]["IdAgenda"];
                    bool isConfirmed = await IsSommConfirmed(idPres, idRigaPres, objP);

                    if (isConfirmed) return true;
                }

            return false;
        }

        /// <summary>
        /// Ottiene i Capi trattati della prima Somministrazione
        /// </summary>
        /// <param name="idRic"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<int[]> GetCapiFromFirstSomm(int idRic, AgronicaCoreParametriServer objP)
        {
            var dtoFirstSomm = await _ricetteZooDal.GetFirstSomministrazione(idRic, "", objP) 
                ?? throw new GiasException($"Non sono stati trovati i Capi della prima Somministrazione (Id_Ricetta di partenza = {idRic}).");

            var dtRicZA = await _ricetteZooAgDal.ReadAsync("", 0, dtoFirstSomm.IdRicetta, 0, "", objP);
            if (dtRicZA == null || dtRicZA.Rows.Count == 0)
                throw new GiasException($"Non sono stati trovati i Capi della prima Somministrazione (Id_Ricetta di partenza = {idRic})."); 
            int idRigaPres = (int)dtRicZA.Rows[0]["IdAgenda"];

            var dtRicZDst = await _ricetteZooDestDal.ReadAsync("", 0, dtoFirstSomm.IdRicetta, idRigaPres, 0, 0, 0, objP);
            if (dtRicZDst == null || dtRicZDst.Rows.Count == 0)
                throw new GiasException($"Non sono stati trovati i Capi della prima Somministrazione (Id_Ricetta di partenza = {idRic}).");

            return dtRicZDst.AsEnumerable().Select(row => (int)row["CodAnimale"]).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRicetta"></param>
        /// <param name="capiTrattati"></param>
        /// <param name="objP"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task UpdateCapiTrattatiSommSuccessive(int idRicetta, List<CapoAnimaleCDC> capiTrattati, AgronicaCoreParametriServer objP)
        {
            var dtRicZoo = await _ricetteZooDal.ReadAsync("", 0, 0, idRicetta, "", objP)
                ?? throw new GiasException($"Prescrizione {idRicetta} non trovata.");
            int gruppoRicetta = (int)dtRicZoo.Rows[0]["Gruppo_Ricetta"];

            // Ottiene gli IdRicetta delle successive Prescrizioni future legate alla Somministrazione passata
            var dtGruppoPres = await _ricetteZooDal.ReadByGruppoPresAsync(gruppoRicetta, objP) ??
                throw new GiasException($"Prescrizioni del gruppo {gruppoRicetta} legate alla Somministrazione passata non trovate.");
            if (dtGruppoPres.Rows.Count == 1) return; // Protocollo con una sola somministrazione

            var idRicettaList = dtGruppoPres
                .AsEnumerable()
                .Select(row => (int)row["IdRicetta"])
                .OrderBy(x => x)
                .SkipWhile(x => x <= idRicetta).ToArray();

            var dtosRicZA = new List<WriteRicetteZooAgenda>();

            if (idRicettaList.Length > 0)
            {
                foreach (int idPres in idRicettaList)
                {
                    var dtRicZooAg = await _ricetteZooAgDal.ReadAsync("", 0, idPres, 0, "", objP);
                    if (dtRicZooAg == null || dtRicZooAg.Rows.Count == 0)
                        throw new GiasException($"Riga della Prescrizione {idPres} (legata alla Somministrazione passata) non trovata.");
                    var dtoRicZA = _mapper.Map<WriteRicetteZooAgenda>(new RicetteZooAgendaRow(dtRicZooAg.Rows[0]));
                    int idRiga = dtoRicZA.IdAgenda;

                    // Elimina i record precedenti di Ricette_Zoo_Destinazioni
                    var dtosRicZDst = (await _ricetteZooDestDal.ReadAsync("", 0, idPres, idRiga, 0, 0, 0, objP)
                        ?? throw new GiasException($"Destinazioni della Riga di Prescrizione {idPres} - {idRiga} (legata alla Somministrazione passata) non trovate."))
                        .AsEnumerable().Select(row => _mapper.Map<WriteRicetteZooDestinazioni>(new RicetteZooDestinazioniRow(row))).ToList();
                    await _ricetteZooDestDal.EliminaAsync(dtosRicZDst, objP);

                    dtosRicZA.Add(dtoRicZA);
                }

                foreach (CapoAnimale capo in capiTrattati.Select(cdc => cdc.capoAnimale))
                    await WriteRicetteZooDestFromCapo(dtosRicZA, capo, objP);
            }
        }

        private async Task<bool> IsFirstSomm(int idPres, int idRigaPres, AgronicaCoreParametri objP)
        {
            var dtRicZoo = await _ricetteZooDal.ReadAsync("", 0, 0, idPres, "", objP)
                ?? throw new GiasException($"Prescrizione {idPres} non trovata.");
            if (dtRicZoo.Rows.Count == 0) throw new GiasException($"Prescrizione {idPres} non trovata.");
            var dtoRicZoo = _mapper.Map<WriteRicetteZoo>(new RicetteZooRow(dtRicZoo.Rows[0]));
            
            int tipoCod = dtoRicZoo.TipoCodice ?? 0;
            if (tipoCod != (int)enum_TipoPrescrizione.Da_Protocollo_GIAS) return false;
                // throw new GiasException($"La prima Somministrazione non è gestita per il Tipo {tipoCod}: Id_Ricetta={idPres}, Id_Riga_Ricetta={idRigaPres}, TipoCodice={tipoCod}");

            return await _ricetteZooAgDal.IsFirstSomm(idPres, idRigaPres, "", objP);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="somministrazione"></param>
        /// <param name="objP_Server"></param>
        /// <param name="objP_Utenti"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<ScriviModificaSomministrazione_Resp> ConfermaSomministrazioneFutura(Attivita somministrazione, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            var resp = new ScriviModificaSomministrazione_Resp();

            int idPres = ParseRequiredInt(nameof(Attivita), nameof(somministrazione.codiceOperazioneRicetta), somministrazione.codiceOperazioneRicetta, " Impossibile creare/modificare la Somministrazione.");
           
            // Lettura Riga della Prescrizione legata alla Somministrazione futura
            var dtRicZooAg = await _ricetteZooAgDal.ReadAsync("", 0, idPres, 0, "", objP_Server);
            if (dtRicZooAg == null || dtRicZooAg.Rows.Count == 0)
                throw new GiasException($"Non è stata trovata la Riga di Prescrizione legata alla Somministrazione {idPres}.");
            else if (dtRicZooAg.Rows.Count > 1)
                throw new GiasException($"Sono state trovate più Righe di Prescrizione legate alla Somministrazione {idPres}.");

            //
            bool ricettaRibaltata = await IsRicettaRibaltata(idPres, objP_Server);
            if (ricettaRibaltata)
            {
                throw new GiasException($"Somministrazione già confermata per la prescrizione {idPres}.");
            }
            var dtoRicZA = _mapper.Map<WriteRicetteZooAgenda>(new RicetteZooAgendaRow(dtRicZooAg.Rows[0]));

            int idRigaPres = dtoRicZA.IdAgenda;
            bool isFirst = dtoRicZA.Numero_Somm == 1;
            bool updateCapiTrattati = !await BloccaUpdateCapiTrattati(idPres, objP_Server);

            // Ottiene le Famiglia AIC dei prodotti che possono essere prescritti per questa Somministrazione
            string[] eligibleAicFamilies = await _ricetteZooDettDal.GetFamigliaAicFromDettagli(idPres, idRigaPres, 0, 0, objP_Server);

            // Scrittura Agenda
            WriteAgenda dtoSommAgenda = await GetDtoAgendaFromAttivitaZoo(somministrazione, objP_Server);
            dtoSommAgenda.Id_Agenda = await _agendaBiz.ScriviModificaAsync(dtoSommAgenda, objP_Server, JsonConvert.SerializeObject(somministrazione, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            // Scrittura link Ricette_ZooxAgenda
            WriteRicetteZooxAgenda dtoRZxA = new(dtoRicZA.IdRicetta, dtoRicZA.IdAgenda, dtoSommAgenda.Id_Agenda)
            {
                Validita_Inizio = somministrazione.inizio,
                Validita_Fine = somministrazione.fine,
            };
            await _ricetteZooxAgDal.ScriviModificaAsync(dtoRZxA, objP_Server);

            // Scrittura Movimenti - Scarico del Farmaco sul Capo
            WriteMovimenti dtoMov = await WriteMovimentiFromAttivitaZoo(somministrazione, dtoSommAgenda, CAU_MOV.CAU_TRATTAMENTO_ZOO, objP_Server, dtoRicZA.Note);

            // Scrittura testata (Mov Zoo) per dati della Prescrizione
            bool resMovZ = await WriteMovimentiZooFromAttivitaZoo(idPres, somministrazione, dtoMov, objP_Server, objP_Utenti);

            // Verifica se i Capi selezionati sono appartenenti allo stesso elenco di quelli trattati della prima Somministrazione
            // NB: Possono essere anche meno di quelli della prima Somministrazione, ma quelli selezionati devono rientrare nello stesso elenco
            int[] animaliToCheck = isFirst ? Array.Empty<int>() : await GetCapiFromFirstSomm(dtoRicZA.IdRicetta, objP_Server);

            bool withDiffAICs = IsSommWithDiffAICs(somministrazione);

            foreach (var risorsa in somministrazione.risorse)
            {
                /*** Dati relativi allo scarico del farmaco sul capo ***/
                if (risorsa is DettaglioRegistroSomministrazioni dettaglioSomm)
                {
                    if (!eligibleAicFamilies.Any(substring => dettaglioSomm.codiceAIC.Contains(substring)))
                        throw new GiasException($"Famiglia AIC errata. Il farmaco selezionato (AIC {dettaglioSomm.codiceAIC}) non può essere prescritto.");

                    // Scrittura riga di Movimenti_dettagli (dettagli del farmaco)
                    WriteMovDettagli dtoMovDett = await WriteMovDettagliFromSomministrazione(somministrazione, dtoMov, dettaglioSomm, objP_Server);

                    // Scrittura Mov_Dettaglio_Tecnico (tempi sospensione per alimento)  
                    if (dettaglioSomm.sospensione != null)
                        foreach (TempiSospensione sosp in dettaglioSomm.sospensione)
                            await WriteMovDettTecnicoFromSospensione(dtoMovDett, sosp, objP_Server);
                    
                    var capiTrattati = somministrazione.centriDiCosto.Where(cdc => cdc.classType == nameof(CapoAnimaleCDC)).OfType<CapoAnimaleCDC>().ToList();
                    FixQtaSomministrataxCapo((double)dtoMovDett.Qta!, capiTrattati, withDiffAICs);

                    // Scrittura Mov_Destinazioni e Ricette_Zoo_Destinazioni (dati del capo)
                    foreach (CapoAnimaleCDC capoCDC in capiTrattati)
                    {
                        if (!isFirst && !animaliToCheck.Contains(capoCDC.capoAnimale.codice))
                            throw new GiasException($"Il Capo {capoCDC.capoAnimale.matricola} non è associato alla prima Somministrazione. Non è possibile inserirlo tra i Capi da trattare.");

                        await WriteMovDestinazioneFromCapo(somministrazione, dettaglioSomm, capoCDC, dtoMovDett, null, objP_Server);
                    }

                    // Se è la prima Somministrazione e non ne è stato bloccato l'update, aggiorna i Capi trattati delle Somministrazioni successive
                    if (updateCapiTrattati) await UpdateCapiTrattatiSommSuccessive(idPres, capiTrattati, objP_Server);
                }
                /*** Dati relativi allo scarico del farmaco dalle giacenze di magazzino ***/
                else if (risorsa is RisorsaProdotto scaricoProd)
                {
                    // Verifica se esiste la somministrazione (scarico) animale da associare allo scarico di magazzino
                    if (somministrazione.risorse.Find(r => r is DettaglioRegistroSomministrazioni somm && somm.prodotto.codice == scaricoProd.prodotto.codice)
                        is DettaglioRegistroSomministrazioni dettSomm)
                    {
                        if (!eligibleAicFamilies.Any(substring => dettSomm.codiceAIC.Contains(substring)))
                            throw new GiasException($"Famiglia AIC errata. Il farmaco selezionato (AIC {dettSomm.prodotto.codice_alfanumerico}) non può essere prescritto.");

                        // Scrittura Movimenti Scarico 
                        WriteMovimenti dtoMovScarico = await WriteMovimentiFromAttivitaZoo(somministrazione, dtoSommAgenda, CAU_MOV.CAU_SCARICO, objP_Server);

                        foreach (RilevamentoDiMagazzino movMagazzino in scaricoProd.MagazziniMovimentazioni)
                        {
                            // Scrittura Movimenti_dettagli Scarico
                            WriteMovDettagli dtoMovDettScarico = await WriteMovDettagliFromScaricoProd(somministrazione, dtoMovScarico, scaricoProd, movMagazzino, dettSomm, objP_Server);

                            // Scrittura Mov_Destinazioni Scarico
                            WriteMovDestinazioni dtoMovDestScarico = GetDtoMovDestinazioneFromScaricoProd(somministrazione, dtoMovDettScarico, movMagazzino);
                            int idDestScarico = await _movDestinazioniDal.ScriviModificaAsync(dtoMovDestScarico, objP_Server);
                        }
                    }
                }
            }

            resp.Id_Somministrazione = dtoSommAgenda.Id_Agenda;
            resp.Id_Prescrizione = idPres;
            resp.Id_Riga_Prescrizione = idRigaPres;

            return resp;
        }

        private async Task<bool> IsRicettaRibaltata(int idPres, AgronicaCoreParametriServer objP_Server)
        {
            DataTable dt = await _ricetteZooxAgDal.ReadAsync(idPres, 0, 0, objP_Server);
            if (dt != null && dt.Rows.Count > 0)
                return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="somministrazione"></param>
        /// <param name="idAgenda"></param>
        /// <param name="objP_Server"></param>
        /// <param name="objP_Utenti"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<ScriviModificaSomministrazione_Resp> UpdateSomministrazione(Attivita somministrazione, int idAgenda, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            var resp = new ScriviModificaSomministrazione_Resp();

            // Modifica Agenda
            var dtAgenda = await _agendaDal.ReadAsync("", idAgenda, objP_Server);
            if (dtAgenda == null || dtAgenda.Rows.Count == 0)
                throw new GiasException($"Operazione di Agenda {idAgenda} non trovata.");
            if (dtAgenda.Rows.Count != 1)
                throw new GiasException($"Piu' di un'Operazione trovata per Id_Agenda = {idAgenda}.");

            var dtoAgenda = _mapper.Map<WriteAgenda>(new AgendaRow(dtAgenda.Rows[0]));
            dtoAgenda.Validita_Inizio = somministrazione.inizio.Date;
            dtoAgenda.Validita_Fine = somministrazione.fine.Date;
            await _agendaBiz.ScriviModificaAsync(dtoAgenda, objP_Server, JsonConvert.SerializeObject(somministrazione, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            // Verifica se la somministrazione è già stata trasmessa al sistema VetInfo:
            // numTrattamento è valorizzato in DettaglioRegistroSomministrazioni se il trattamento ha già un Rif_Esterno (già inviato a VetInfo)
            string? numTrattamentoVetInfo = somministrazione.risorse
                .OfType<DettaglioRegistroSomministrazioni>()
                .Select(d => d.numTrattamento)
                .FirstOrDefault(n => !string.IsNullOrEmpty(n));
            if (!string.IsNullOrEmpty(numTrattamentoVetInfo))
                throw new GiasException($"La somministrazione è già stata trasmessa al sistema VetInfo (n. trattamento: {numTrattamentoVetInfo}). Non è possibile modificare i capi trattati per una somministrazione già inviata.");

            // TODO Valutare se necessario: caso di prima somministrazione potrebbe essere necessario
            // Lettura legame Somministrazione - Prescrizione
            var dtRicZxA = await _ricetteZooxAgDal.ReadAsync(0, 0, idAgenda, objP_Server);
            if (dtRicZxA == null || dtRicZxA.Rows.Count == 0)
                throw new GiasException($"Non è stata trovata la Riga di Prescrizione legata alla Somministrazione {idAgenda}.");
            else if (dtRicZxA.Rows.Count > 1)
                throw new GiasException($"Sono state trovate più Righe di Prescrizione legate alla Somministrazione {idAgenda}.");

            var dtoRicZxA = _mapper.Map<WriteRicetteZooxAgenda>(new RicetteZooxAgendaRow(dtRicZxA.Rows[0]));
            int idPres = dtoRicZxA.Id_Ricetta;
            int idRigaPres = dtoRicZxA.Id_RigaRicetta;

            var dtRicZoo = await _ricetteZooDal.ReadAsync("", 0, 0, idPres, "", objP_Server);
            if (dtRicZoo == null || dtRicZoo.Rows.Count == 0)
                throw new GiasException($"Testata di Prescrizione {idPres} non trovata.");
            else if (dtRicZoo.Rows.Count > 1)
                throw new GiasException($"Sono state trovate più Testate di Prescrizione con Id_Ricetta = {idPres}.");
            var dtoRicZoo = _mapper.Map<WriteRicetteZoo>(new RicetteZooRow(dtRicZoo.Rows[0]));

            var tipoPres = (enum_TipoPrescrizione)(dtoRicZoo.TipoCodice ?? 0);
            bool isProtocollo = (tipoPres == enum_TipoPrescrizione.Protocollo_Terapeutico || tipoPres == enum_TipoPrescrizione.Da_Protocollo_GIAS);

            bool isFirst = isProtocollo && await IsFirstSomm(idPres, idRigaPres, objP_Server);
            bool updateCapiTrattati = isProtocollo && !(await BloccaUpdateCapiTrattati(idPres, objP_Server));

            var dtRZAg = await _ricetteZooAgDal.ReadAsync("", 0, idPres, idRigaPres, "", objP_Server);
            if (dtRZAg == null || dtRZAg.Rows.Count == 0)
                throw new GiasException($"Riga di Prescrizione {idRigaPres} non trovata.");
            else if (dtRZAg.Rows.Count > 1)
                throw new GiasException($"Sono state trovate più Righe di Prescrizione con Id_Riga_Ricetta = {idRigaPres}.");
            var dtoRZAg = _mapper.Map<WriteRicetteZooAgenda>(new RicetteZooAgendaRow(dtRZAg.Rows[0]));

            // Ottiene le Famiglia AIC dei prodotti che possono essere prescritti per questa Somministrazione
            string[] eligibleAicFamilies = await _ricetteZooDettDal.GetFamigliaAicFromDettagli(idPres, idRigaPres, 0, 0, objP_Server);

            /** Logica di Modifica: 
                Elimina vecchi record da Movimenti in poi e crea i nuovi record come per la creazione .
                Se si sta modificando una prima Somministrazione, è consentita la modifica del Farmaco (restando nella stessa Famiglia AIC) 
                    e dei Capi da trattare (diversi, più o meno Capi degli originali).
                Se non si tratta della prima Somministrazione del gruppo, solo del Farmaco (sempre restando nella stessa Famiglia AIC) 
                    e dei Capi (stessi o meno degli originali).
                In caso di modifica della prima Somministrazione, vengono aggiornati anche i Capi per le successive; 
                    nel caso di somministrazioni appartenenti allo stesso gruppo della prima già confermata, viene bloccata la modifica (solo per modifica dei Capi).
            **/

            // Elimina le righe di tabelle Agenda collegate (Movimenti, Movimenti_dettagli, Mov_Destinazioni e Mov_Dettaglio_Tecnico) 
            await DeleteOldMovimenti(dtoAgenda.Piva, dtoAgenda.Id_Agenda, objP_Server);

            // Scrittura Movimenti - Scarico del Farmaco sul Capo
            WriteMovimenti dtoMov = await WriteMovimentiFromAttivitaZoo(somministrazione, dtoAgenda, CAU_MOV.CAU_TRATTAMENTO_ZOO, objP_Server, dtoRZAg.Note);

            // Scrittura testata (Mov Zoo) per dati della Prescrizione
            bool resMovZ = await WriteMovimentiZooFromAttivitaZoo(idPres, somministrazione, dtoMov, objP_Server, objP_Utenti);

            // Verifica se i Capi selezionati sono appartenenti allo stesso elenco di quelli trattati della prima Somministrazione
            int[] animaliToCheck = Array.Empty<int>();
            if (isProtocollo && !isFirst)
                animaliToCheck = await GetCapiFromFirstSomm(idPres, objP_Server);

            bool withDiffAICs = IsSommWithDiffAICs(somministrazione);

            foreach (var risorsa in somministrazione.risorse)
            {
                /*** Dati relativi allo scarico del farmaco sul capo ***/
                if (risorsa is DettaglioRegistroSomministrazioni dettaglioSomm)
                {
                    if (!eligibleAicFamilies.Any(substring => dettaglioSomm.codiceAIC.Contains(substring)))
                        throw new GiasException($"Famiglia AIC errata. Il farmaco selezionato (AIC {dettaglioSomm.prodotto.codice_alfanumerico}) non può essere prescritto.");

                    // Scrittura riga di Movimenti_dettagli (dettagli del farmaco)
                    WriteMovDettagli dtoMovDett = await WriteMovDettagliFromSomministrazione(somministrazione, dtoMov, dettaglioSomm, objP_Server);

                    // Scrittura Mov_Dettaglio_Tecnico (tempi sospensione per alimento)  
                    if (dettaglioSomm.sospensione != null)
                        foreach (TempiSospensione sosp in dettaglioSomm.sospensione)
                            await WriteMovDettTecnicoFromSospensione(dtoMovDett, sosp, objP_Server);

                    var capiTrattati = somministrazione.centriDiCosto.Where(cdc => cdc.classType == nameof(CapoAnimaleCDC)).OfType<CapoAnimaleCDC>().ToList();
                    FixQtaSomministrataxCapo((double)dtoMovDett.Qta!, capiTrattati, withDiffAICs);

                    // Scrittura Mov_Destinazioni e Ricette_Zoo_Destinazioni (dati del capo)
                    foreach (CapoAnimaleCDC capoCDC in capiTrattati)
                    {
                        if (isProtocollo && !isFirst)
                            if (!animaliToCheck.Contains(capoCDC.capoAnimale.codice))
                                throw new GiasException($"Il Capo {capoCDC.capoAnimale.matricola} non è associato alla prima Somministrazione. Non è possibile inserirlo tra i Capi da trattare.");

                        await WriteMovDestinazioneFromCapo(somministrazione, dettaglioSomm, capoCDC, dtoMovDett, null, objP_Server);
                    }

                    // Se è la prima Somministrazione e non ne è stato bloccato l'update, aggiorna i Capi trattati delle Somministrazioni successive
                    if (updateCapiTrattati) await UpdateCapiTrattatiSommSuccessive(dtoRicZxA.Id_Ricetta, capiTrattati, objP_Server);

                }
                /*** Dati relativi allo scarico del farmaco dalle giacenze di magazzino ***/
                else if (risorsa is RisorsaProdotto scaricoProd)
                {
                    // Verifica se esiste la somministrazione (scarico) animale da associare allo scarico di magazzino
                    if (somministrazione.risorse.Find(r => r is DettaglioRegistroSomministrazioni somm && somm.prodotto.codice == scaricoProd.prodotto.codice)
                        is DettaglioRegistroSomministrazioni dettSomm)
                    {
                        if (!eligibleAicFamilies.Any(substring => dettSomm.codiceAIC.Contains(substring)))
                            throw new GiasException($"Famiglia AIC errata. Il farmaco selezionato (AIC {dettSomm.prodotto.codice_alfanumerico}) non può essere prescritto.");

                        // Scrittura Movimenti Scarico 
                        WriteMovimenti dtoMovScarico = await WriteMovimentiFromAttivitaZoo(somministrazione, dtoAgenda, CAU_MOV.CAU_SCARICO, objP_Server);

                        foreach (RilevamentoDiMagazzino movMagazzino in scaricoProd.MagazziniMovimentazioni)
                        {
                            // Scrittura Movimenti_dettagli Scarico
                            WriteMovDettagli dtoMovDettScarico = await WriteMovDettagliFromScaricoProd(somministrazione, dtoMovScarico, scaricoProd, movMagazzino, dettSomm, objP_Server);

                            // Scrittura Mov_Destinazioni Scarico
                            WriteMovDestinazioni dtoMovDestScarico = GetDtoMovDestinazioneFromScaricoProd(somministrazione, dtoMovDettScarico, movMagazzino);
                            int idDestScarico = await _movDestinazioniDal.ScriviModificaAsync(dtoMovDestScarico, objP_Server);
                        }
                    }
                }
            }

            resp.Id_Somministrazione = dtoAgenda.Id_Agenda;
            resp.Id_Prescrizione = idPres;
            resp.Id_Riga_Prescrizione = idRigaPres;

            return resp;
        }

        #endregion

        public async Task<SomministrazioneProdotti> LeggiSomministrazioneProdottiAsync(string sommNumero, AgronicaCoreParametriServer objP)
        {
            SomministrazioneProdotti somministrazioneProdotti = new SomministrazioneProdotti();

            try
            {
                if (sommNumero == "0")
                {
                    somministrazioneProdotti.result = await _farmaciDal.LeggiFarmaciAsync(0, "", objP);
                    somministrazioneProdotti.kendoColumns = new KendoColumn[]
                    {
                        new KendoColumn { Field = "AIC", Hidden = false, Title = "AIC", DataType = "string" },
                        new KendoColumn { Field = "Confezione", Hidden = false, Title = "Confezione", DataType = "string" },
                        new KendoColumn { Field = "Denominazione", Hidden = false, Title = "Denominazione", DataType = "string" },
                        new KendoColumn { Field = "Codice_GTIN", Hidden = false, Title = "CodiceGTIN", DataType = "string" },
                        new KendoColumn { Field = "ModalitaPrescrizione", Hidden = false, Title = "ModalitaPresc", DataType = "string" },
                        new KendoColumn { Field = "InfoAggiuntive", Hidden = false, Title = "InfoAggiuntive", DataType = "string" }
                    };
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objP, ex);
                throw;
            }
            return somministrazioneProdotti;
        }

        private bool IsSommWithDiffAICs(Attivita somministrazione)
        {
            return somministrazione.risorse
                .Where(ris => ris is DettaglioRegistroSomministrazioni)
                .Select(ris => (ris as DettaglioRegistroSomministrazioni)!.codiceAIC)
                .Distinct()
                .Count() > 1;
        }

        public async Task<ITrattamentoResult> CreaTrattamentoDaProtocollo(int Id_Protocollo, List<Attivita> somministrazioni, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            var resp = new CreaTrattamentoDaProtocollo_Resp();
            var nomeRoutine = "CreaTrattamentoDaProtocollo";

            var jsonSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                MaxDepth = 32
            };

            LogInformation("Request [" + nomeRoutine + "]:" + JsonConvert.SerializeObject(somministrazioni, jsonSettings), objP_Server);

            try
            {
                await OpenConnectionAsync(objP_Server, true, IsolationLevel.ReadUncommitted);
                //await OpenTransactionAsync(objP);

                // Ribalta il Protocollo in x (Ricette_Zoo_Agenda.Numero_Somm) nuove testate di Prescrizione (Ricette_Zoo);
                // la prima viene legata alla nuova Somministrazione
                var dtoRicZoo = await ProjectProtocolloToIndTerapeutica(Id_Protocollo, objP_Server);

                int idPrescrizione = dtoRicZoo.IdRicetta;
                int gruppoPrescrizione = dtoRicZoo.Gruppo_Ricetta ?? 0;

                // Per ora si suppone che il Protocollo abbia sempre e solo un Prodotto
                // Scrive per Data Inizio crescente le Somministrazioni passate
                int sommNum = 1;
                foreach (Attivita somministrazione in somministrazioni.OrderBy(act => act.inizio))
                {
                    // Scrittura Agenda
                    WriteAgenda dtoSommAgenda = await GetDtoAgendaFromAttivitaZoo(somministrazione, objP_Server);
                    dtoSommAgenda.Id_Agenda = await _agendaBiz.ScriviModificaAsync(dtoSommAgenda, objP_Server, JsonConvert.SerializeObject(somministrazione, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                    // Aggiorna le date di inizio e fine di ogni Riga di Prescrizione (Ricette_Zoo_Agenda), in base ai campi Intervallo_Somm e Numero_Somm
                    var dtoRicZooAgs = await WriteRighePrescrizioneFromSomministrazione(somministrazione, gruppoPrescrizione, sommNum, dtoSommAgenda.Id_Agenda, objP_Server);
                    somministrazione.codiceOperazioneRicetta = dtoRicZooAgs[0].IdAgenda.ToString();
                    var sommNumDes = dtoRicZooAgs[0].Note;

                    // Scrittura Movimenti - Scarico del Farmaco sul Capo
                    WriteMovimenti dtoMov = await WriteMovimentiFromAttivitaZoo(somministrazione, dtoSommAgenda, CAU_MOV.CAU_TRATTAMENTO_ZOO, objP_Server, sommNumDes);

                    bool withDiffAICs = IsSommWithDiffAICs(somministrazione);

                    // Scrittura testata (Mov Zoo) per dati della Prescrizione
                    bool resMovZ = await WriteMovimentiZooFromAttivitaZoo(dtoRicZoo.IdRicetta, somministrazione, dtoMov, objP_Server, objP_Utenti);

                    // OLD Elimina le righe di tabelle Agenda collegate (Movimenti, Movimenti_dettagli, Mov_Destinazioni e Mov_Dettaglio_Tecnico) 
                    //await DeleteOldMovimenti(dtoSommAgenda.Piva, dtoSommAgenda.Id_Agenda, objP);

                    var aicScaricati = new List<string>();
                    foreach (var risorsa in somministrazione.risorse)
                    {
                        /*** Dati relativi allo scarico del farmaco sul capo ***/
                        if (risorsa is DettaglioRegistroSomministrazioni dettaglioSomm)
                        {
                            // Scrittura Movimenti_dettagli (dettagli del farmaco) 
                            WriteMovDettagli dtoMovDett = await WriteMovDettagliFromSomministrazione(somministrazione, dtoMov, dettaglioSomm, objP_Server);

                            // Scrittura Mov_Dettaglio_Tecnico (tempi sospensione per alimento)  
                            if (dettaglioSomm.sospensione != null)
                                foreach (TempiSospensione sosp in dettaglioSomm.sospensione)
                                    await WriteMovDettTecnicoFromSospensione(dtoMovDett, sosp, objP_Server);

                            // Clona la lista per mantenere la quantità per capo da distribuire
                            var listaCapiCdc = new List<CapoAnimaleCDC>();
                            listaCapiCdc.AddRange(
                                somministrazione.centriDiCosto.Where(cdc => cdc.classType == nameof(CapoAnimaleCDC)).OfType<CapoAnimaleCDC>().ToList()
                            );
                            
                            FixQtaSomministrataxCapo((double)dtoMovDett.Qta!, listaCapiCdc, withDiffAICs);

                            // Scrittura Mov_Destinazioni e Ricette_Zoo_Destinazioni (dati del capo)
                            bool generateRzDst = !aicScaricati.Contains(dettaglioSomm.codiceAIC[..6]);
                            foreach (CapoAnimaleCDC capoCDC in listaCapiCdc)
                                await WriteMovDestinazioneFromCapo(somministrazione, dettaglioSomm, capoCDC, dtoMovDett, dtoRicZooAgs, objP_Server, generateRzDst);

                            if (!aicScaricati.Contains(dettaglioSomm.codiceAIC[..6])) aicScaricati.Add(dettaglioSomm.codiceAIC[..6]);
                        }
                        /*** Dati relativi allo scarico del farmaco dalle giacenze di magazzino ***/
                        else if (risorsa is RisorsaProdotto scaricoProd)
                        {
                            // Verifica se esiste la somministrazione (scarico) animale da associare allo scarico di magazzino
                            if (somministrazione.risorse.Find(r => r is DettaglioRegistroSomministrazioni somm && somm.prodotto.codice == scaricoProd.prodotto.codice) 
                                is DettaglioRegistroSomministrazioni dettSomm)
                            {
                                // Scrittura Movimenti Scarico 
                                WriteMovimenti dtoMovScarico = await WriteMovimentiFromAttivitaZoo(somministrazione, dtoSommAgenda, CAU_MOV.CAU_SCARICO, objP_Server);

                                foreach (RilevamentoDiMagazzino movMagazzino in scaricoProd.MagazziniMovimentazioni)
                                {
                                    // Scrittura Movimenti_dettagli Scarico
                                    WriteMovDettagli dtoMovDettScarico = await WriteMovDettagliFromScaricoProd(somministrazione, dtoMovScarico, scaricoProd, movMagazzino, dettSomm, objP_Server);

                                    // Scrittura Mov_Destinazioni Scarico
                                    WriteMovDestinazioni dtoMovDestScarico = GetDtoMovDestinazioneFromScaricoProd(somministrazione, dtoMovDettScarico, movMagazzino);
                                    int idDestScarico = await _movDestinazioniDal.ScriviModificaAsync(dtoMovDestScarico, objP_Server);
                                }
                            }
                        }
                    }

                    sommNum++;
                    resp.Somministrazioni.Add(dtoSommAgenda.Id_Agenda);
                }

                resp.Gruppo_Prescrizione = gruppoPrescrizione;
                resp.Id_Prescrizione = idPrescrizione;
            }
            catch (Exception ex)
            {
                CloseTransaction(objP_Server, true);
                LogError(ex.Message, objP_Server, ex);
                throw;
            }
            finally
            {
                CloseConnection(objP_Server);
            }
            LogInformation("Response [" + nomeRoutine + "]:" + JsonConvert.SerializeObject(resp), objP_Server);
            return resp;
        }
        
        public async Task<ITrattamentoResult> ScriviModificaSomministrazione(Attivita somministrazione, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            var resp = new ScriviModificaSomministrazione_Resp();

            try
            {
                await OpenConnectionAsync(objP_Server, true, IsolationLevel.ReadUncommitted);

                int idAgenda = ParseRequiredInt(nameof(Attivita), nameof(somministrazione.codice), somministrazione.codice, " Impossibile creare/modificare la Somministrazione.");
                
                if (idAgenda == 0)
                    /*** Conferma di una Somministrazione futura ***/
                    resp = await ConfermaSomministrazioneFutura(somministrazione, objP_Server, objP_Utenti);
                else
                    /*** Modifica di una Somministrazione esistente ***/
                    resp = await UpdateSomministrazione(somministrazione, idAgenda, objP_Server, objP_Utenti);
            }
            catch (Exception ex)
            {
                CloseTransaction(objP_Server, true);
                LogError(ex.Message, objP_Server, ex);
                throw;
            }
            finally
            {
                CloseConnection(objP_Server);
            }

            return resp;
        }

        public async Task<bool> EliminaSomministrazione(DeleteSomministrazione dtoDelete, AgronicaCoreParametriServer objP)
        {
            bool result = false;

            // Somministrazione futura
            if (dtoDelete.Programmata)
            {
                if (dtoDelete.Id_Ricetta == null || dtoDelete.Id_Ricetta == 0)
                    throw new GiasException($"La Somministrazione da eliminare è programmata, è necessario valorizzare Id_Ricetta.");
                if (dtoDelete.Id_Riga_Ricetta == null || dtoDelete.Id_Riga_Ricetta == 0)
                    throw new GiasException($"La Somministrazione da eliminare è programmata, è necessario valorizzare Id_Riga_Ricetta.");

                int idRicetta = dtoDelete.Id_Ricetta ?? 0;
                int idRigaRicetta = dtoDelete.Id_Riga_Ricetta ?? 0;

                var dtRicZooxAg = await _ricetteZooxAgDal.ReadAsync(idRicetta, idRigaRicetta, 0, objP);
                if (dtRicZooxAg != null && dtRicZooxAg.Rows.Count > 0)
                    throw new GiasException($"La Somministrazione selezionata è già stata confermata.");

                var dtRicZoo = await _ricetteZooDal.ReadAsync("", 0, 0, idRicetta, "", objP);
                if (dtRicZoo == null || dtRicZoo.Rows.Count == 0)
                    throw new GiasException($"Testata di Prescrizione non trovata (Id_Ricetta = {idRicetta}).");
                var dtoRicZoo = _mapper.Map<WriteRicetteZoo>(new RicetteZooRow(dtRicZoo.Rows[0]));

                // Elimina i record di Ricette_Zoo e associate che rappresentano la Somministrazione futura
                result = await _ricetteZooDal.EliminaAssociateAsync(dtoRicZoo, objP);

            }
            // Somministrazione confermata
            else
            {
                throw new NotImplementedException("Questa funzione non è più da utilizzare per l'eliminazione di Somministrazioni confermate. " +
                    "Utilizzare elimina_operazione_multipla presente su AgronicaCoreWS.");
                //if (dtoDelete.Id_Agenda == null || dtoDelete.Id_Agenda == 0)
                //    throw new GiasException($"La Somministrazione da eliminare è confermata, è necessario valorizzare Id_Agenda.");

                //var dtAgenda = await _agendaDal.ReadAsync("", dtoDelete.Id_Agenda ?? 0, objP);
                //if (dtAgenda == null || dtAgenda.Rows.Count == 0)
                //    throw new GiasException($"Somminstrazione non trovata (Id_Agenda = {dtoDelete.Id_Agenda}).");
                //var dtoAgenda = _mapper.Map<WriteAgenda>(new AgendaRow(dtAgenda.Rows[0]));

                //result = await _agendaDal.EliminaAssociateAsync(dtoAgenda, objP);
            }

            return result;
        }

        public async Task<ITrattamentoResult> CreaTrattamentoDaPrescrizione(int Id_Prescrizione, List<Attivita> somministrazioni, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            var resp = new CreaTrattamentoDaProtocollo_Resp();

            try
            {
                await OpenConnectionAsync(objP_Server, true, IsolationLevel.ReadUncommitted);

                var dtRicZoo = await _ricetteZooDal.ReadAsync("", 0, 0, Id_Prescrizione, "", objP_Server);
                if (dtRicZoo == null || dtRicZoo.Rows.Count == 0)
                    throw new GiasException($"Testata di Prescrizione non trovata (Id_Prescrizione = {Id_Prescrizione}).");
                else if (dtRicZoo.Rows.Count > 1)
                    throw new GiasException($"Più Testate di Prescrizione trovate (Id_Prescrizione = {Id_Prescrizione}).");
                var dtoRicZoo = _mapper.Map<WriteRicetteZoo>(new RicetteZooRow(dtRicZoo.Rows[0]));

                var dtRZAg = await _ricetteZooAgDal.ReadAsync("", 0, Id_Prescrizione, 0, "", objP_Server);
                if (dtRZAg == null || dtRZAg.Rows.Count == 0)
                    throw new GiasException($"Riga di Prescrizione non trovata (Id_Prescrizione = {Id_Prescrizione}).");
                else if (dtRZAg.Rows.Count > 1)
                    throw new GiasException($"Più Righe di Prescrizione trovate (Id_Prescrizione = {Id_Prescrizione}), al momento non vengono gestite.");
                var dtoRZAg = _mapper.Map<WriteRicetteZooAgenda>(new RicetteZooAgendaRow(dtRZAg.Rows[0]));

                // Ribalta il Protocollo in x (Ricette_Zoo_Agenda.Numero_Somm) nuove testate di Prescrizione (Ricette_Zoo)
                // nel caso in cui la durata del trattamento è maggiore di un giorno: la prima viene legata alla nuova Somministrazione.
                if (dtoRZAg.Numero_Somm > 1)
                {
                    dtoRicZoo = await ProjectPrescrizioneForProtocolliInCorso(Id_Prescrizione, objP_Server);
                    dtoRZAg.Note = $"1 di {dtoRZAg.Numero_Somm}";
                }

                int idPrescrizione = dtoRicZoo.IdRicetta;
                int gruppoPrescrizione = dtoRicZoo.Gruppo_Ricetta ?? 0;

                // Per ora si suppone che la Prescrizione abbia sempre e solo un Prodotto
                int sommNum = 1;
                foreach (Attivita somministrazione in somministrazioni.OrderBy(act => act.inizio))
                {
                    // Scrittura Agenda
                    WriteAgenda dtoSommAgenda = await GetDtoAgendaFromAttivitaZoo(somministrazione, objP_Server);
                    dtoSommAgenda.Id_Agenda = await _agendaBiz.ScriviModificaAsync(dtoSommAgenda, objP_Server, JsonConvert.SerializeObject(somministrazione, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                    // Aggiorna le date di inizio e fine di ogni Riga di Prescrizione (Ricette_Zoo_Agenda), in base ai campi Intervallo_Somm e Numero_Somm
                    List<WriteRicetteZooAgenda> dtoRicZooAgs;
                    if (dtoRZAg.Numero_Somm > 1)
                        dtoRicZooAgs = await WriteRighePrescrizioneFromSomministrazione(somministrazione, gruppoPrescrizione, sommNum, dtoSommAgenda.Id_Agenda, objP_Server);
                    else
                        dtoRicZooAgs = new List<WriteRicetteZooAgenda>() { dtoRZAg };
                    somministrazione.codiceOperazioneRicetta = dtoRicZooAgs[0].IdAgenda.ToString();

                    // Scrittura link Ricette_Zoo x Agenda
                    WriteRicetteZooxAgenda dtoRZxA = new(Id_Prescrizione, dtoRZAg.IdAgenda, dtoSommAgenda.Id_Agenda)
                    {
                        Validita_Inizio = somministrazione.inizio,
                        Validita_Fine = somministrazione.fine,
                    };
                    await _ricetteZooxAgDal.ScriviModificaAsync(dtoRZxA, objP_Server);
                     
                    // Scrittura Movimenti - Scarico del Farmaco sul Capo
                    WriteMovimenti dtoMov = await WriteMovimentiFromAttivitaZoo(somministrazione, dtoSommAgenda, CAU_MOV.CAU_TRATTAMENTO_ZOO, objP_Server, dtoRZAg.Note);

                    // Scrittura testata (Mov Zoo) per dati della Prescrizione
                    bool resMovZ = await WriteMovimentiZooFromAttivitaZoo(dtoRicZoo.IdRicetta, somministrazione, dtoMov, objP_Server, objP_Utenti);

                    foreach (var risorsa in somministrazione.risorse)
                    {
                        /*** Dati relativi allo scarico del farmaco sul capo ***/
                        if (risorsa is DettaglioRegistroSomministrazioni dettaglioSomm)
                        {
                            // Se si tratta di una Somministrazione appartenente a un gruppo, la quantità totale reale viene letta dalla prima Riga di Prescrizione del gruppo
                            // poiché è stata calcolata in fase di proiezione della Prescrizione nel gruppo di Somministrazioni (fn ProjectPrescrizioneForProtocolliInCorso) 
                            //if (gruppoPrescrizione != 0)
                            //{
                            //    var dtRzDett = await _ricetteZooDettDal.ReadAsync("", 0, dtoRZAg.IdRicetta, dtoRZAg.IdAgenda, 0, 0, objP_Server);
                            //    if (dtRzDett != null && dtRzDett.Rows.Count == 1)
                            //    {
                            //        var dtoRzDett = _mapper.Map<WriteRicetteZooDettagli>(new RicetteZooDettagliRow(dtRzDett.Rows[0]));
                            //        dettaglioSomm.quantitaTotaleReale = (decimal)dtoRzDett.Quantitativo!;
                                    
                            //        var qtaxCapo = Math.Round((decimal)(dettaglioSomm.quantitaTotaleReale! / dtoRZAg.Numero_Somm!), MidpointRounding.ToZero);
                            //        listaCapiCdc.ForEach(capo => capo.qtaSomministrata = (double)qtaxCapo!);
                            //    }
                            //}

                            // Scrittura Movimenti_dettagli (dettagli del farmaco) 
                            WriteMovDettagli dtoMovDett = await WriteMovDettagliFromSomministrazione(somministrazione, dtoMov, dettaglioSomm, objP_Server);

                            // Scrittura Mov_Dettaglio_Tecnico (tempi sospensione per alimento)  
                            if (dettaglioSomm.sospensione != null)
                                foreach (TempiSospensione sosp in dettaglioSomm.sospensione)
                                    await WriteMovDettTecnicoFromSospensione(dtoMovDett, sosp, objP_Server);

                            // Clona la lista per mantenere la quantità per capo da distribuire
                            var listaCapiCdc = new List<CapoAnimaleCDC>();
                            listaCapiCdc.AddRange(
                                somministrazione.centriDiCosto.Where(cdc => cdc.classType == nameof(CapoAnimaleCDC)).OfType<CapoAnimaleCDC>().ToList()
                            );
                            FixQtaSomministrataxCapo((double)dtoMovDett.Qta!, listaCapiCdc);

                            // Scrittura Mov_Destinazioni e Ricette_Zoo_Destinazioni (dati del capo)
                            foreach (CapoAnimaleCDC capoCDC in listaCapiCdc)
                                await WriteMovDestinazioneFromCapo(somministrazione, dettaglioSomm, capoCDC, dtoMovDett, dtoRicZooAgs, objP_Server);

                        }
                        /*** Dati relativi allo scarico del farmaco dalle giacenze di magazzino ***/
                        else if (risorsa is RisorsaProdotto scaricoProd)
                        {
                            // Verifica se esiste la somministrazione (scarico) animale da associare allo scarico di magazzino
                            if (somministrazione.risorse.Find(r => r is DettaglioRegistroSomministrazioni somm && somm.prodotto.codice == scaricoProd.prodotto.codice)
                                is DettaglioRegistroSomministrazioni dettSomm)
                            {
                                // Scrittura Movimenti Scarico 
                                WriteMovimenti dtoMovScarico = await WriteMovimentiFromAttivitaZoo(somministrazione, dtoSommAgenda, CAU_MOV.CAU_SCARICO, objP_Server);

                                foreach (RilevamentoDiMagazzino movMagazzino in scaricoProd.MagazziniMovimentazioni)
                                {
                                    // Scrittura Movimenti_dettagli Scarico
                                    WriteMovDettagli dtoMovDettScarico = await WriteMovDettagliFromScaricoProd(somministrazione, dtoMovScarico, scaricoProd, movMagazzino, dettSomm, objP_Server);

                                    // Scrittura Mov_Destinazioni Scarico
                                    WriteMovDestinazioni dtoMovDestScarico = GetDtoMovDestinazioneFromScaricoProd(somministrazione, dtoMovDettScarico, movMagazzino);
                                    int idDestScarico = await _movDestinazioniDal.ScriviModificaAsync(dtoMovDestScarico, objP_Server);
                                }
                            }
                        }
                    }

                    sommNum++;
                    resp.Somministrazioni.Add(dtoSommAgenda.Id_Agenda);
                }

                resp.Id_Prescrizione = Id_Prescrizione;
            }
            catch (Exception ex)
            {
                CloseTransaction(objP_Server, true);
                LogError(ex.Message, objP_Server, ex);
                throw;
            }
            finally
            {
                CloseConnection(objP_Server);
            }

            return resp;
        }

        public async Task<ITrattamentoResult> ProgrammaTrattamentoDaProtocollo(int Id_Protocollo, List<Attivita> somministrazioni, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            var resp = new CreaTrattamentoDaProtocollo_Resp();
            var nomeRoutine = "ProgrammaTrattamentoDaProtocollo";
            LogInformation("Request [" + nomeRoutine + "]:" + JsonConvert.SerializeObject(somministrazioni), objP_Server);
            try
            {
                await OpenConnectionAsync(objP_Server, true, IsolationLevel.ReadUncommitted);
                //await OpenTransactionAsync(objP);

                // Ribalta il Protocollo in x (Ricette_Zoo_Agenda.Numero_Somm) nuove testate di Prescrizione (Ricette_Zoo);
                // la prima viene legata alla nuova Somministrazione
                var dtoRicZoo = await ProjectProtocolloToIndTerapeutica(Id_Protocollo, objP_Server);

                int idPrescrizione = dtoRicZoo.IdRicetta;
                int gruppoPrescrizione = dtoRicZoo.Gruppo_Ricetta ?? 0;

                // Aggiorna le date di inizio e fine di ogni Riga di Prescrizione (Ricette_Zoo_Agenda), in base ai campi Intervallo_Somm e Numero_Somm
                var dtRicZoo = await _ricetteZooDal.ReadByGruppoPresAsync(gruppoPrescrizione, objP_Server);
                if (dtRicZoo == null || dtRicZoo.Rows.Count == 0)
                    throw new GiasException($"Testate di Prescrizione non trovate. Impossibile proseguire nell'aggiornamento delle Righe di Prescrizione.");

                List<WriteRicetteZooAgenda> righePrescrizione = new();
                foreach (DataRow row in dtRicZoo.Rows)
                {
                    int idRicetta = (int)row["IdRicetta"];
                    var dtRicZooAg = await _ricetteZooAgDal.ReadAsync("", 0, idRicetta, 0, "", objP_Server);

                    if (dtRicZooAg == null || dtRicZooAg.Rows.Count == 0)
                        throw new GiasException($"Non sono state trovate le Righe della Prescrizione {idRicetta} (Id Gruppo {gruppoPrescrizione}) da associare alla Somministrazione.");

                    righePrescrizione.AddRange(dtRicZooAg.AsEnumerable().Select(row => _mapper.Map<WriteRicetteZooAgenda>(new RicetteZooAgendaRow(row))));
                }

                var somministrazione = somministrazioni.First();
                var dtosRZA = new List<WriteRicetteZooAgenda>();
                DateTime lastInsertedDate = somministrazione.inizio.Date;
                foreach (var rowRza in righePrescrizione.OrderBy(row => row.Numero_Somm))
                {
                    bool isFirst = rowRza.Numero_Somm == 1;

                    int intervalloSomm = rowRza.Intervallo_Somm ?? 0;
                    rowRza.DataInizioTrattamento = lastInsertedDate;
                    rowRza.DataFineTrattamento = lastInsertedDate;
                    rowRza.DurataTrattamento = intervalloSomm;

                    int idRigaPres = await _ricetteZooAgDal.ScriviModificaAsync(rowRza, objP_Server);
                    rowRza.IdAgenda = idRigaPres;

                    lastInsertedDate = lastInsertedDate.AddDays(intervalloSomm);

                    dtosRZA.Add(rowRza);
                }

                somministrazione.codiceOperazioneRicetta = dtosRZA[0].IdAgenda.ToString();


                // Caso scrittura prima Somministrazione da Protocollo: necessario aggiungere le righe di Ricette_Zoo_Destinazioni poichè mancanti
                if (dtosRZA != null)
                    foreach (CapoAnimale capo in somministrazione.centriDiCosto
                        .Where(cdc => cdc.classType == nameof(CapoAnimaleCDC))
                        .OfType<CapoAnimaleCDC>()
                        .Select(cdc => cdc.capoAnimale)
                        .ToList())
                    {
                        await WriteRicetteZooDestFromCapo(dtosRZA, capo, objP_Server);
                    }

                resp.Gruppo_Prescrizione = gruppoPrescrizione;
                resp.Id_Prescrizione = idPrescrizione;
            }
            catch (Exception ex)
            {
                CloseTransaction(objP_Server, true);
                LogError(ex.Message, objP_Server, ex);
                throw;
            }
            finally
            {
                CloseConnection(objP_Server);
            }
            LogInformation("Response [" + nomeRoutine + "]:" + JsonConvert.SerializeObject(resp), objP_Server);
            return resp;
        }

        #endregion
    }
}
