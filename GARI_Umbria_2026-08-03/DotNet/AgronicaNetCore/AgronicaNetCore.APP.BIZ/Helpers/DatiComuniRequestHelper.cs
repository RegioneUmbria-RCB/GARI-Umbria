using System;
using System.Collections.Generic;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.UtentiImpostazioni;
using AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.AggregazioneJSONRisposta;
using AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.Enums;
using AgronicaNetCore.APP.BIZ.Services.ValidazioneParametriUtente;

namespace AgronicaNetCore.APP.BIZ.Helpers
{
    public static class DatiComuniRequestHelper
    {
        /// <summary>
        /// Normalizza un <see cref="DatiComuniFromFlutterRequest"/> in un dizionario chiave→valore stringa.
        /// I campi bool vengono serializzati come <c>"true"</c>/<c>"false"</c>;
        /// i campi stringa vengono mantenuti con il loro valore originale.
        /// </summary>
        public static Dictionary<string, string> ToParamRichiamoApi(DatiComuniFromFlutterRequest request)
        {
            return new Dictionary<string, string>
            {
                [nameof(request.AnagMetaschema)] = request.AnagMetaschema.ToString(),
                [nameof(request.AnagPersonalizzataRilievoFasiFenologiche)] = request.AnagPersonalizzataRilievoFasiFenologiche.ToString(),
                [nameof(request.AnagPersonalizzataRilievoIndiciMaturita)] = request.AnagPersonalizzataRilievoIndiciMaturita.ToString(),
                [nameof(request.AnagPersonalizzataRilievoAvversita)] = request.AnagPersonalizzataRilievoAvversita.ToString(),
                [nameof(request.AnagPersonalizzataRilievoDanniRaccolta)] = request.AnagPersonalizzataRilievoDanniRaccolta.ToString(),
                [nameof(request.AnagPersonalizzataRilievoErbeInfestanti)] = request.AnagPersonalizzataRilievoErbeInfestanti.ToString(),
                // Campi stringa: mantenuti come tali
                [nameof(request.AnagDisciplinari)] = request.AnagDisciplinari ?? string.Empty,
                [nameof(request.AnagNazioni)] = request.AnagNazioni ?? "IT",
                [nameof(request.AnagUtilizziTerreno)] = request.AnagUtilizziTerreno ?? string.Empty,
            };
        }

        /// <summary>
        /// Determina la lista di <see cref="TabellaComune"/> da estrarre applicando la
        /// combinazione tra i parametri request normalizzati e i permessi utente.
        /// </summary>
        public static List<string> ResolveTabelleDaEstrarre(
            Dictionary<string, string> paramRichiamoApi,
            PermessiUtenteSincronizzazioneEntity permessi)
        {
            // Per i campi bool serializzati come "True"/"False" e per i campi stringa:
            // considera il parametro attivo se valorizzato e diverso da "false" (case-insensitive).
            bool Param(string key) =>
                paramRichiamoApi.TryGetValue(key, out var v) &&
                !string.IsNullOrEmpty(v) &&
                !v.Equals("False", StringComparison.OrdinalIgnoreCase);

            var tabelle = new List<string>();

            // Attivita || Rilievi || Visite
            if (permessi.Attivita || permessi.Rilievi || permessi.Visite)
            {
                tabelle.Add(TabellaComune.Lavorazioni.ToString());
                tabelle.Add(TabellaComune.OperazioniCombinazioni.ToString());
            }

            // Attivita || Rilievi
            if (Param(nameof(DatiComuniFromFlutterRequest.AnagMetaschema)))
            {
                if (permessi.Attivita || permessi.Rilievi)
                {
                    tabelle.Add(TabellaComune.Avversita.ToString());
                    tabelle.Add(TabellaComune.GruppiAvversita.ToString());
                    tabelle.Add(TabellaComune.GruppiAvversitaAttive.ToString());
                    tabelle.Add(TabellaComune.AvversitaSpecie.ToString());
                    tabelle.Add(TabellaComune.InfestantiAttive.ToString());
                    tabelle.Add(TabellaComune.CategorieUnitaMisura.ToString());
                }
            }

            // Rilievi
            if (Param(nameof(DatiComuniFromFlutterRequest.AnagMetaschema)) && permessi.Rilievi)
            {
                if (Param(nameof(DatiComuniFromFlutterRequest.AnagPersonalizzataRilievoFasiFenologiche)))
                    tabelle.Add(TabellaComune.SpecieVegetaliStadiCrescitaPersonalizzati.ToString());
                else
                    tabelle.Add(TabellaComune.SpecieVegetaliStadiCrescita.ToString());

                if (Param(nameof(DatiComuniFromFlutterRequest.AnagPersonalizzataRilievoIndiciMaturita)))
                {
                    tabelle.Add(TabellaComune.IndiciMaturitaPersonalizzate.ToString());
                    tabelle.Add(TabellaComune.IndiciMaturitaSpecieVegetaliPersonalizzate.ToString());
                    tabelle.Add(TabellaComune.MisureIndiciMaturitaPersonalizzate.ToString());
                }
                else
                {
                    tabelle.Add(TabellaComune.IndiciMaturita.ToString());
                    tabelle.Add(TabellaComune.IndiciMaturitaSpecieVegetali.ToString());
                    tabelle.Add(TabellaComune.MisureIndiciMaturita.ToString());
                }

                if (Param(nameof(DatiComuniFromFlutterRequest.AnagPersonalizzataRilievoAvversita)))
                    tabelle.Add(TabellaComune.MisureAvversitaPersonalizzate.ToString());
                else
                    tabelle.Add(TabellaComune.MisureAvversita.ToString());

                if (Param(nameof(DatiComuniFromFlutterRequest.AnagPersonalizzataRilievoDanniRaccolta)))
                    tabelle.Add(TabellaComune.MisureDanniPersonalizzate.ToString());
                else
                    tabelle.Add(TabellaComune.MisureDanni.ToString());
            }

            // Macchine
            if (Param(nameof(DatiComuniFromFlutterRequest.AnagMetaschema)))
            {
                if (permessi.Macchine)
                    tabelle.Add(TabellaComune.TipiMacchine.ToString());
            }

            // AnagDisciplinari — solo controllo request, senza permessi specifici
            if (Param(nameof(DatiComuniFromFlutterRequest.AnagDisciplinari)))
                tabelle.Add(TabellaComune.Disciplinari.ToString());

            // ImpiantiIrrigazioni — always included
            tabelle.Add(TabellaComune.ImpiantiIrrigazioni.ToString());

            // AnagMetaschema && AnagNazioni && (Aziende || PianoColturale)
            if (Param(nameof(DatiComuniFromFlutterRequest.AnagMetaschema)) &&
                paramRichiamoApi.GetValueOrDefault(nameof(DatiComuniFromFlutterRequest.AnagNazioni), "") != "" &&
                (permessi.Aziende || permessi.PianoColturale))
            {
                tabelle.Add(TabellaComune.Nazioni.ToString());
                tabelle.Add(TabellaComune.Regioni.ToString());
                tabelle.Add(TabellaComune.Province.ToString());
                tabelle.Add(TabellaComune.Comuni.ToString());
            }

            // AnagMetaschema && AnagUtilizziTerreno && (PianoColturale || Visite)
            if (Param(nameof(DatiComuniFromFlutterRequest.AnagMetaschema)) &&
                paramRichiamoApi.GetValueOrDefault(nameof(DatiComuniFromFlutterRequest.AnagUtilizziTerreno), "") != "" &&
                (permessi.PianoColturale || permessi.Visite))
            {
                //aggiungi destinazioni uso
                if (paramRichiamoApi.GetValueOrDefault(nameof(DatiComuniFromFlutterRequest.AnagUtilizziTerreno), "").Contains("0"))
                {
                    tabelle.Add(TabellaComune.DestinazioneUso.ToString());
                }

                tabelle.Add(TabellaComune.Specie.ToString());
                // Varieta e Finalita solo se PianoColturale
                if (permessi.PianoColturale)
                {
                    tabelle.Add(TabellaComune.Varieta.ToString());
                    tabelle.Add(TabellaComune.Finalita.ToString());
                }
            }

            // AnagMetaschema && Visite
            if (Param(nameof(DatiComuniFromFlutterRequest.AnagMetaschema)) && permessi.Visite)
                tabelle.Add(TabellaComune.SpecieZootecniche.ToString());

            tabelle.Add(TabellaComune.RapportiContabili.ToString());
            return tabelle;
        }

        public static FiltriRichiesti PopolaFiltriRichiesti(DatiComuniFromFlutterRequest request, ParamVisibilitaUtente visibilitaUtente)
        {
            var filtri = new FiltriRichiesti();

            // Nazioni
            if (!string.IsNullOrEmpty(request.AnagNazioni))
            {
                filtri.Nazioni = new List<string>(request.AnagNazioni.Split(','));
            }

            // CodiciSpecieVegetali (AnagUtilizziTerreno ha precedenza su visibilità specie)
            if (!string.IsNullOrEmpty(request.AnagUtilizziTerreno))
            {
                var utilizzi = request.AnagUtilizziTerreno.Split(',');
                filtri.Specie = new List<int>();
                foreach (var utilizzo in utilizzi)
                {
                    if (int.TryParse(utilizzo, out int idUtilizzo) && idUtilizzo != 0)
                    {
                        filtri.Specie.Add(idUtilizzo);
                    }
                }
            }
            else if (visibilitaUtente?.CodiciSpecieVegetali != null)
            {
                filtri.Specie = new List<int>(visibilitaUtente.CodiciSpecieVegetali);
            }

            // CodiciOperazioni
            if (visibilitaUtente?.CodiciOperazioni != null)
            {
                filtri.Operazioni = new List<int>(visibilitaUtente.CodiciOperazioni);
            }

            return filtri;
        }
    }
}
