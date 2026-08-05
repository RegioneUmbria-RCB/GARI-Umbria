using AgronicaCoreDTOStd.InData.Agenda.OperazioneAgenda_Temp;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.centri_di_costo;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.attivita.Info;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.costanti;
using AgronicaNetCore.Operazione.BIZ.Resources;
using InData.Anagrafica;
using Microsoft.Extensions.Localization;
using AgronicaNetCore.Base.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Operazione.BIZ.Services.Mov_Destinazioni.Factory
{
    public class MovDestinazioniFactory : BaseServiceOperazioneBIZ, IMovDestinazioniFactory
    {
        public MovDestinazioniFactory(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
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
                    }
                    else if (resouce.Opzioni_Raccolta.Ripartizione == Opzioni_Raccolta.enum_Ripartizione_Raccolta.AUTO_PIANTE && totalPlants > 0)
                    {
                        decimal plants = (decimal)plant.esercizioCDC.esercizio.piante_Ha * plant.esercizioCDC.superficieTrattata;
                        qta = resouce.quantitaTotaleReale * plants / totalPlants;
                    }
                    else
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

        private Movimento_Destinazione GetBaseMovDest(OperazioneAgenda agenda, InfoOperazione info, EsercizioCDC esercizioCDC, decimal superficieTotale)
        {
            Movimento_Destinazione mov = new();
            mov.Id_Agenda = agenda.Id_Agenda;
            mov.Data = agenda.Data;
            mov.Piva = esercizioCDC.esercizio.impiantoPK.piva;
            mov.Sa_Cod = esercizioCDC.esercizio.impiantoPK.saCod;
            mov.Appezza = esercizioCDC.esercizio.impiantoPK.appezza;
            mov.Id_Destinazione = esercizioCDC.esercizio.impiantoPK.codice;
            mov.Tipo = 0;
            mov.Qta = 0;
            mov.Qta2 = esercizioCDC.superficieTrattata;
            mov.BaseCode = info.BaseCode;
            mov.TopCode = info.TopCode;
            mov.Programmazione_Entita_Cod = 0;
            mov.QuotaDistribuzione = superficieTotale != 0 ? esercizioCDC.superficieTrattata / superficieTotale : 0;
            return mov;
        }

        private List<Movimento_Destinazione>? GetMovDestinazioneFromQuantitaSuImpianti(
            List<QuantitaSuImpianto>  quantitaSuImpianto,
            OperazioneAgenda agenda, InfoOperazione info, decimal area)
        {
            if (quantitaSuImpianto == null || !quantitaSuImpianto.Any()) return null;

            List<Movimento_Destinazione> movimenti = new();
            foreach (var q in quantitaSuImpianto)
            {
                Movimento_Destinazione? mov = movimenti.Find(m => 
                    m.Piva == q.esercizioCDC.esercizio.impiantoPK.piva &&
                    m.Sa_Cod == q.esercizioCDC.esercizio.impiantoPK.saCod &&
                    m.Appezza == q.esercizioCDC.esercizio.impiantoPK.appezza &&
                    m.Id_Destinazione == q.esercizioCDC.esercizio.impiantoPK.codice);
                if (mov == null)
                {
                    mov = GetBaseMovDest(agenda, info, q.esercizioCDC, area);
                    movimenti.Add(mov);
                }
                mov.Qta += q.Qta;
            }
            return movimenti;
        }

        public List<Movimento_Destinazione>? GetMovDestinazioni(Attivita attivita, OperazioneAgenda agenda, InfoOperazione info, decimal area, decimal dose)
        {
            if (attivita.centriDiCosto == null) return null;

            List<Movimento_Destinazione> movimenti = new();
            IEnumerable<EsercizioCDC> cdcs = attivita.centriDiCosto.Where(c => c.classType == ClassType.EsercizioCDC).Cast<EsercizioCDC>();
            foreach (var cdc in cdcs)
            {
                Movimento_Destinazione mov = GetBaseMovDest(agenda, info, cdc, area);
                mov.Qta = dose * cdc.superficieTrattata;
                if (info.IsTrattamento || info.IsFertilizzazione)
                {
                    mov.Sup_Riduzione_BufferZone = cdc.superficieRiduzioneBufferZone;
                    mov.Perc_Riduzione_Deriva = cdc.percentualeRiduzioneDeriva;
                }
                movimenti.Add(mov);
            }
            return movimenti;
        }

        public List<Movimento_Destinazione>? GetMovDestinazioniRaccolta(Movimento_Dettaglio mov, Attivita attivita, OperazioneAgenda agenda, InfoOperazione info, decimal area, decimal dose, List<RisorsaProdotto> resources)
        {
            var products = resources.Cast<DettaglioRaccolta>()
                   .Where(res => res.prodotto != null && res.prodotto.codice == mov.Mat_Cod &&
                       res.QuantitaSuImpianti.Any() && res.QuantitaSuImpianti.First().Lotto.ToUpper() == mov.Lotto
                   ).ToList();
            var qtyOnPlant = RepartHarvestedQuantity(products, q => q.Lotto);
            if (qtyOnPlant != null && qtyOnPlant.Any())
            {
                return GetMovDestinazioneFromQuantitaSuImpianti(qtyOnPlant.Values.ToList(), agenda, info, area);
            }
            else
            {
                return GetMovDestinazioni(attivita, agenda, info, area, dose);
            }
        }

        public List<Movimento_Destinazione> GetMovDestinazioniRilievo(
            IEnumerable<DettaglioRilievo> dettagliPerChiave,
            Attivita attivita,
            decimal superficieAccumulata,
            InfoOperazione info)
        {
            var result = new List<Movimento_Destinazione>();
            foreach (var det in dettagliPerChiave)
            {
                if (det.esercizioCDC == null) continue;
                var pk = det.esercizioCDC.esercizio.impiantoPK;

                Movimento_Destinazione movimento_Destinazione = new()
                {
                    Piva = pk.piva,
                    Sa_Cod = pk.saCod,
                    Appezza = pk.appezza,
                    Id_Destinazione = pk.codice,
                    Qta = superficieAccumulata,
                    Qta2 = det.esercizioCDC.superficieTrattata,
                    BaseCode = info.BaseCode,
                    TopCode = info.TopCode,
                    Data = attivita.inizio
                };

                switch (attivita.job.getCodice())
                {
                    case LAV_COD.LAVCOD_FASI_FENOLOGICHE:
                        movimento_Destinazione.Data = det.DataOraRilievo;
                        break;
                    case LAV_COD.LAVCOD_RILIEVO_INDICI_MATURITA:
                        //TODO gestire salvataggio per i rilivi di indici di maturità
                        break;
                }

                result.Add(movimento_Destinazione);
            }
            return result;
        }
    }
}
