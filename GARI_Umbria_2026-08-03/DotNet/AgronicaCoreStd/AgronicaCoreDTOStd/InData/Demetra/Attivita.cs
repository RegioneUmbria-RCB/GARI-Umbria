using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace AgronicaCoreDTOStd.InData.Demetra
{
    public class Attivita
    {
        public string codice { get; set; }
        public string codice_esterno { get; set; }
        public string subcodice { get; set; }
        public string subcodice_esterno { get; set; }
        public string versione { get; set; }
        public bool flag_cancellazione { get; set; }
        public bool flag_pianificata { get; set; }
        public PianificataCodici pianificata { get; set; }
        public string utente_ultima_modifica { get; set; }

        public DateTime data { get; set; }

        public int tipo_operazione { get; set; }

        public List<Impianto> impianti { get; set; }

        public string note { get; set; }

        public List<Macchina> macchine { get; set; }

        public List<Operatore> operatori { get; set; }

        public Acqua acqua { get; set; }

        public List<Prodotto> prodotti { get; set; }

        public bool flag_manuale { get; set; }

        public List<Raccolto> raccolti { get; set; }

        public List<Irrigazione> irrigazioni { get; set; }

        public int regolamento_cod { get; set; }

        public void RaggruppaRaccolti(bool creaPlot = false, bool lottoDistinti = false)
        {
            if (!flag_manuale)
            {
                var raccoltiRaggruppati = raccolti
                    .GroupBy(r => new
                    {
                        r.codice,
                        r.codice_esterno,
                        r.udm,
                        MagazzinoCodice = r.magazzino?.codice,
                        MagazzinoCodiceEsterno = r.magazzino?.codice_esterno,
                        MagazzinoLotto = r.magazzino?.lotto
                    })
                    .Select(gruppo => new
                    {
                        gruppo.Key,
                        quantitaTotale = gruppo.Sum(r => r.quantita)
                    })
                    .ToList();

                raccolti.Clear();

                decimal superficieTotale = impianti.Sum(i => i.superficie_trattata);
                var lottoCounter = new Dictionary<string, int>();

                foreach (var gruppo in raccoltiRaggruppati)
                {
                    if (creaPlot)
                    {
                        var lotto = lottoDistinti ? GetLottoConSuffisso(gruppo.Key.MagazzinoLotto, lottoCounter) : gruppo.Key.MagazzinoLotto;
                        foreach (var impianto in impianti)
                        {
                            var quantitaProporzionale = (gruppo.quantitaTotale / superficieTotale) * impianto.superficie_trattata;                            
                            raccolti.Add(new Raccolto
                            {
                                codice = gruppo.Key.codice,
                                codice_esterno = gruppo.Key.codice_esterno,
                                udm = gruppo.Key.udm,
                                magazzino = gruppo.Key.MagazzinoCodice != null ? new Magazzino
                                {
                                    codice = gruppo.Key.MagazzinoCodice,
                                    codice_esterno = gruppo.Key.MagazzinoCodiceEsterno,
                                    lotto = lotto
                                } : null,
                                quantita = quantitaProporzionale,
                                plot_id = impianto.plot_id
                            });
                        }
                    }
                    else
                    {
                        var lotto = lottoDistinti ? GetLottoConSuffisso(gruppo.Key.MagazzinoLotto, lottoCounter) : gruppo.Key.MagazzinoLotto;
                        raccolti.Add(new Raccolto
                        {
                            codice = gruppo.Key.codice,
                            codice_esterno = gruppo.Key.codice_esterno,
                            udm = gruppo.Key.udm,
                            magazzino = gruppo.Key.MagazzinoCodice != null ? new Magazzino
                            {
                                codice = gruppo.Key.MagazzinoCodice,
                                codice_esterno = gruppo.Key.MagazzinoCodiceEsterno,
                                lotto = lotto
                            } : null,
                            quantita = gruppo.quantitaTotale,
                            plot_id = string.Empty
                        });
                    }
                }
            }
        }

        private string GetLottoConSuffisso(string lotto, Dictionary<string, int> lottoCounter)
        {
            if (lotto != null)
            {
                if (lottoCounter.ContainsKey(lotto))
                {
                    lottoCounter[lotto]++;
                    lotto += $"_{lottoCounter[lotto]}";
                }
                else
                {
                    lottoCounter[lotto] = 1;
                }
            }
            return lotto;
        }

        public class PianificataCodici
        {
            public string codice { get; set; }
            public string codice_esterno { get; set; }
        }
    }
}