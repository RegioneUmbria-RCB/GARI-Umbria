using AgronicaCoreDTOStd.InData.Agenda.OperazioneAgenda_Temp;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.attivita.Info;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.costanti;
using AgronicaCoreModelsSTD.metaschema.avversita;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Operazione.BIZ.Resources;
using AgronicaNetCore.Operazione.BIZ.Services.Mov_Destinazioni.Factory;
using AgronicaNetCore.Operazione.DAL.Resources.UtilityAgenda;
using InData.Anagrafica;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;

namespace AgronicaNetCore.Operazione.BIZ.Services.Movimenti_Dettagli.Factory
{
    public class MovDettagliFactory : BaseServiceOperazioneBIZ, IMovDettagliFactory
    {
        private readonly IUtilityAgenda _utils;
        private readonly IMovDestinazioniFactory _movDestinazioniFactory;

        public MovDettagliFactory(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _utils = provider.GetRequiredService<IUtilityAgenda>();
            _movDestinazioniFactory = provider.GetRequiredService<IMovDestinazioniFactory>();
        }

        private Movimento_Dettaglio GetBaseMov(OperazioneAgenda agenda, InfoOperazione info)
        {
            Movimento_Dettaglio baseMov = new();
            baseMov.Id_Agenda = agenda.Id_Agenda;
            baseMov.Piva = agenda.Piva;
            baseMov.Sa_Cod = agenda.Sa_Cod;
            baseMov.Data = agenda.Data;
            baseMov.Lav_Cod = agenda.Lav_Cod;
            baseMov.Elem_Cod = info.Elem_Cod;
            baseMov.Contabilizzato = CostantiPersonalizzate.NONCONTABILE;
            baseMov.BaseCode = info.BaseCode;
            baseMov.TopCode = info.BaseCode;
            baseMov.Mezzo_Det = -1;
            return baseMov;
        }

        private bool HandleProductDetails(Attivita attivita, List<Movimento_Dettaglio> details, List<RisorsaProdotto> resources, InfoOperazione info)
        {
            if (!resources.Any()) return false;

            bool found = false;

            if (info.IsRaccolta && attivita.tipoRaccolta != Attivita.Tipo_Raccolta.Fast &&
                (((DettaglioRaccolta)resources.First()).Opzioni_Raccolta.GenerazioneLotto == Opzioni_Raccolta.enum_Generazione_Lotto_Raccolta.DA_ESERCIZO ||
                ((DettaglioRaccolta)resources.First()).Opzioni_Raccolta.GenerazioneLotto == Opzioni_Raccolta.enum_Generazione_Lotto_Raccolta.MANUALE))
            {
                throw new NotImplementedException();
            }

            IEnumerable<RisorsaProdotto> productResources = resources.Where(res => res.prodotto != null && res.prodotto.codice != 0);
            foreach (var resource in productResources)
            {
                throw new NotImplementedException();
            }
            return found;
        }

        private void HandleIrrigationDetails() { /* TODO */ }

        private void HandleVisitDetails() { /* TODO */ }

        private void HandleResourceWithoutProduct(
            List<Movimento_Dettaglio> movimentiDettagli, bool foundResouce,
            OperazioneAgenda agenda, InfoOperazione info)
        {
            Movimento_Dettaglio movDett = GetBaseMov(agenda, info);
            if (info.IsLavorazione || info.isNonUtilizzo || info.IsAbbattimento)
            {
                movimentiDettagli.Add(movDett);
            }
            if (info.IsSemina && !foundResouce)
            {
                throw new NotImplementedException();
            }
            if (info.IsRaccolta && !foundResouce)
            {
                movDett.Mov_Det_Des = _localizer.GetString("DettagliProdottiAziendaliRaccolta");
                movDett.Pendente = 3;
                movDett.Anno = 1900;
                movDett.Lotto = "";
                if (movDett.Cal_Cod == null)
                    movDett.Cal_Cod = 0;
                movimentiDettagli.Add(movDett);
            }
        }

        private Dictionary<(int, string, string, string), QuantitaSuImpianto> RepartHarvestedQuantity(
            List<DettaglioRaccolta> products, Func<QuantitaSuImpianto, string> lottoMapper)
        {
            Dictionary<(int, string, string, string), QuantitaSuImpianto> dict = new();
            foreach (var resouce in products)
            {
                int productCode = resouce.prodotto.codice;
                decimal totalArea = resouce.QuantitaSuImpianti.Sum(q => q.esercizioCDC.superficieTrattata);
                decimal totalPlants = resouce.QuantitaSuImpianti.Sum(q => 
                    q.esercizioCDC.superficieTrattata * (decimal)q.esercizioCDC.esercizio.piante_Ha);
                string lastLot = "";
                decimal qta;
                foreach (var plant in resouce.QuantitaSuImpianti)
                {
                    string esercizioK = plant.esercizioCDC.esercizio.GetKey("_");
                    string magazzinoK = plant.Magazzino.GetKey("_");
                    string lotto = lottoMapper(plant) ?? lastLot;

                    if (resouce.Opzioni_Raccolta.Ripartizione == Opzioni_Raccolta.enum_Ripartizione_Raccolta.MANUALE)
                    {
                        qta = plant.Qta;
                    } else if (resouce.Opzioni_Raccolta.Ripartizione == Opzioni_Raccolta.enum_Ripartizione_Raccolta.AUTO_PIANTE && totalPlants > 0)
                    {
                        decimal plants = (decimal)plant.esercizioCDC.esercizio.piante_Ha * plant.esercizioCDC.superficieTrattata;
                        qta = resouce.quantitaTotaleReale * plants / totalPlants;
                    } else
                    {
                        qta = resouce.quantitaTotaleReale * plant.esercizioCDC.superficieTrattata / totalArea;
                    }

                    // Evito di aggiungere gli impianti per cui non è stata specificata una quantità
                    if (qta == 0 && productCode != 0) continue; 

                    if (dict.ContainsKey((productCode, esercizioK, magazzinoK, lotto.ToUpper())))
                    {
                        qta += dict[(productCode, esercizioK, magazzinoK, lotto.ToUpper())].Qta;
                        dict.Remove((productCode, esercizioK, magazzinoK, lotto.ToUpper()));
                    }

                    QuantitaSuImpianto newItem = new();
                    newItem.Qta = qta;
                    newItem.esercizioCDC = plant.esercizioCDC;
                    newItem.Magazzino = plant.Magazzino == null ? new("", 0, 0, "") : plant.Magazzino;
                    newItem.Lotto = lotto;
                    newItem.Prodotto = resouce.prodotto;
                    dict.Add((productCode, esercizioK, magazzinoK, lotto.ToUpper()), newItem);
                    lastLot = lotto;
                }
            }
            return dict;
        }

        private void AddMovDestinazione(Movimento_Dettaglio mov, Attivita attivita, OperazioneAgenda agenda, InfoOperazione info, decimal area, List<RisorsaProdotto> resources)
        {
            decimal doseHaTransformed = 0;
            if (info.IsSemina || info.IsRaccolta || info.Elem_Cod == ELEM_COD.TRAPPOLE)
            {
                doseHaTransformed = _utils.GetDoseTrasformata(mov.Qta, mov.Extra_Int) / area;
            } else
            {
                doseHaTransformed = _utils.GetDoseTrasformata(mov.Qta, mov.Extra_Int);
            }

            if (info.IsRaccolta)
            {
                mov.Movimenti_Destinazioni = _movDestinazioniFactory.GetMovDestinazioniRaccolta(mov, attivita, agenda, info, area, doseHaTransformed, resources);

            } else if (!info.IsIrrigazione && info.TipoCentroDiCosto != AgronicaCoreModelsSTD.attivita.centri_di_costo.Tipo.ProdottoDaTrattare)
            {
                mov.Movimenti_Destinazioni = _movDestinazioniFactory.GetMovDestinazioni(attivita, agenda, info, area, doseHaTransformed);
            }
        }

        private List<Movimento_Dettaglio_Riferimento> GetMovDettagliRiferiemnto(Attivita attivita, InfoOperazione info)
        {
            if (!info.IsVisita || attivita.attivitaCollegate == null)
                return new();
            throw new NotImplementedException();
        }

        public List<Movimento_Dettaglio> GetMovDettagliList(Attivita attivita, OperazioneAgenda agenda, InfoOperazione info, decimal totalTreatedArea)
        {
            if (info.IsRilievo)
                return GetMovDettaglioRilievo(attivita, agenda, info);

            List<Movimento_Dettaglio> movDet = new();
            List<RisorsaProdotto> resources = attivita.risorse.Where(res =>
                res.classType == ClassType.DettaglioTrattamento || res.classType == ClassType.DettaglioFertilizzazione ||
                res.classType == ClassType.DettaglioSemina || res.classType == ClassType.DettaglioRaccolta
            ).Cast<RisorsaProdotto>().ToList();

            bool foundRisorsa = HandleProductDetails(attivita, movDet, resources, info);
            HandleIrrigationDetails();
            HandleVisitDetails();
            HandleResourceWithoutProduct(movDet, foundRisorsa, agenda, info);

            if (info.IsRaccolta && attivita.tipoRaccolta != Attivita.Tipo_Raccolta.Leggera_Con_Dettagli_Magazzino)
            {
                movDet.ForEach(mov => mov.Cal_Cod = 0);
            }
            if (totalTreatedArea > 0)
            {
                movDet.ForEach(mov => AddMovDestinazione(mov, attivita, agenda, info, totalTreatedArea, resources));
            }
            movDet.ForEach(mov => mov.Movimenti_Dettagli_Riferimenti = GetMovDettagliRiferiemnto(attivita, info));

            if (info.TipoCentroDiCosto == AgronicaCoreModelsSTD.attivita.centri_di_costo.Tipo.ProdottoDaTrattare)
            {
                throw new NotImplementedException();
            }

            return movDet;
        }

        #region Rilievo

        private List<Movimento_Dettaglio> GetMovDettaglioRilievo(Attivita attivita, OperazioneAgenda agenda, InfoOperazione info)
        {
            var dettagliRilievo = attivita.risorse
                .Where(r => r.classType == ClassType.DettaglioRilievo)
                .Cast<DettaglioRilievo>()
                .ToList();

            if (dettagliRilievo.Count == 0)
                return new List<Movimento_Dettaglio>();

            ValidaNoDuplicatiRilievo(dettagliRilievo, agenda.Lav_Cod);

            bool rilievoSenzaImpianti = IsRilievoSenzaImpianti(attivita);

            // Group by rilievo key → create one Movimento_Dettaglio per key
            var dict = new Dictionary<string, (Movimento_Dettaglio MovDettaglio, decimal SuperficieAccumulata, List<DettaglioRilievo> Dettagli)>();
            foreach (var det in dettagliRilievo)
            {
                string key = ComputeRilievoKey(agenda.Lav_Cod, det);
                if (!dict.ContainsKey(key))
                    dict[key] = (BuildBaseMovimentoDettaglio(agenda, info, det), 0m, new List<DettaglioRilievo>());

                decimal superficie = (!rilievoSenzaImpianti && det.esercizioCDC != null)
                    ? det.esercizioCDC.superficieTrattata : 0m;
                var (movDet, sup, lista) = dict[key];
                lista.Add(det);
                dict[key] = (movDet, sup + superficie, lista);
            }

            foreach (var entry in dict.Values)
            {
                if (!rilievoSenzaImpianti)
                    entry.MovDettaglio.Movimenti_Destinazioni =
                        _movDestinazioniFactory.GetMovDestinazioniRilievo(
                            entry.Dettagli, attivita, entry.SuperficieAccumulata, info);
            }

            return dict.Values.Select(e => e.MovDettaglio).ToList();
        }

        private void ValidaNoDuplicatiRilievo(List<DettaglioRilievo> dettagliRilievo, int lavCod)
        {
            var chiaviFull = new HashSet<string>();
            foreach (var det in dettagliRilievo)
            {
                string composita = $"{ComputeRilievoKey(lavCod, det)}|{GetImpiantoKey(det)}";
                if (!chiaviFull.Add(composita))
                    throw new InvalidOperationException($"RilievoEsistentePerImpianto: chiave duplicata '{composita}'.");
            }
        }

        private Movimento_Dettaglio BuildBaseMovimentoDettaglio(
            OperazioneAgenda agenda, InfoOperazione info, DettaglioRilievo det)
        {
            var (avCod, avGru) = EstrattiCodiciAvversita(agenda.Lav_Cod, det);
            int ffClasse = EstrattiFfClasse(agenda.Lav_Cod, det);
            int udmCod = det.unitaDiMisura?.codice ?? 0;

            var tecnico = new Movimento_Dettaglio_Tecnico
            {
                Qta_Ril = 0m,          // DS02B-BL: costante per rilievi
                dett_cod = udmCod,
                Av_Cod = avCod,
                Av_Gru = avGru,
                ff_classe = ffClasse,
                BaseCode = info.BaseCode,
                TopCode = info.TopCode
            };

            return new Movimento_Dettaglio
            {
                Id_Agenda = agenda.Id_Agenda,
                Piva = agenda.Piva,
                Sa_Cod = agenda.Sa_Cod,
                Data = agenda.Data,
                Lav_Cod = agenda.Lav_Cod,
                Cau_Mov = info.Cau_Mov,
                Elem_Cod = info.Elem_Cod,
                Contabilizzato = CostantiPersonalizzate.NONCONTABILE,
                Pendente = 3,           // DS02B-BL: costante per rilievi
                Anno = 1900,            // DS02B-BL: costante per rilievi
                Mezzo_Det = -1,         // DS02B-BL: costante per rilievi
                Udm_Cod = udmCod,
                BaseCode = info.BaseCode,
                TopCode = info.TopCode,
                Movimenti_Dettagli_Tecnici = new List<Movimento_Dettaglio_Tecnico> { tecnico },
                Movimenti_Destinazioni = new List<Movimento_Destinazione>()
            };
        }

        private bool IsRilievoSenzaImpianti(Attivita attivita)
        {
            if (attivita.centriDiCosto.OfType<EsercizioCDC>().Any()) return false;
            var dets = attivita.risorse
                .Where(r => r.classType == ClassType.DettaglioRilievo)
                .Cast<DettaglioRilievo>().ToList();
            if (dets.Count == 0) return false;
            return dets.All(d =>
                d.esercizioCDC == null || (
                    d.esercizioCDC.esercizio.codice == 0 &&
                    d.esercizioCDC.esercizio.impiantoPK.codice == 0 &&
                    d.esercizioCDC.esercizio.impiantoPK.appezza == 0));
        }

        private string ComputeRilievoKey(int lavCod, DettaglioRilievo det)
        {
            return lavCod switch
            {
                LAV_COD.LAVCOD_RILIEVO_AVVERSITA_CAMPO => BuildAvversitaKey(det.avversitaGruppo, det.unitaDiMisura?.codice ?? 0, includeUdm: true),
                LAV_COD.LAVCOD_RILIEVO_ERBE_INFESTANTI => BuildAvversitaKey(det.erbaInfestante, det.unitaDiMisura?.codice ?? 0, includeUdm: false),
                LAV_COD.LAVCOD_RILIEVO_INDICI_MATURITA => det.IndiceMaturita.ToString(),
                LAV_COD.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA => det.IndiceResa.ToString(),
                LAV_COD.LAVCOD_DANNI_RACCOLTA => det.DannoRaccolta.ToString(),
                LAV_COD.LAVCOD_FASI_FENOLOGICHE => det.faseFenologica?.codice.ToString() ?? "0",
                _ => "0"
            };
        }

        private string BuildAvversitaKey(AvversitaGruppo? avversita, int udmCod, bool includeUdm)
        {
            var (avCod, avGru) = EstraiCodiciDaAvversita(avversita);
            return includeUdm ? $"{avCod}-{avGru}-{udmCod}" : $"{avCod}-{avGru}";
        }

        private (int AvCod, int AvGru) EstrattiCodiciAvversita(int lavCod, DettaglioRilievo det)
        {
            AvversitaGruppo? avversita = lavCod switch
            {
                LAV_COD.LAVCOD_RILIEVO_AVVERSITA_CAMPO => det.avversitaGruppo,
                LAV_COD.LAVCOD_RILIEVO_ERBE_INFESTANTI => det.erbaInfestante,
                _ => null
            };
            return EstraiCodiciDaAvversita(avversita);
        }

        private (int AvCod, int AvGru) EstraiCodiciDaAvversita(AvversitaGruppo? avversita)
        {
            if (avversita is null) return (0, 0);
            if (avversita.classType == ClassType.GruppoAvversita) return (0, avversita.codice);
            if (avversita.classType == ClassType.Avversita)
                return (avversita.codice, avversita.codice < 0 ? avversita.codice : 0);
            return (0, 0);
        }

        private int EstrattiFfClasse(int lavCod, DettaglioRilievo det)
        {
            return lavCod switch
            {
                LAV_COD.LAVCOD_RILIEVO_INDICI_MATURITA => det.IndiceMaturita,
                LAV_COD.LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA => det.IndiceResa,
                LAV_COD.LAVCOD_DANNI_RACCOLTA => det.DannoRaccolta,
                LAV_COD.LAVCOD_FASI_FENOLOGICHE => det.faseFenologica?.codice ?? 0,
                _ => 0
            };
        }

        private string GetImpiantoKey(DettaglioRilievo det)
        {
            if (det.esercizioCDC == null) return "no-impianto";
            var pk = det.esercizioCDC.esercizio.impiantoPK;
            return $"{pk.piva}_{pk.saCod}_{pk.appezza}_{pk.codice}";
        }

        #endregion
    }
}
