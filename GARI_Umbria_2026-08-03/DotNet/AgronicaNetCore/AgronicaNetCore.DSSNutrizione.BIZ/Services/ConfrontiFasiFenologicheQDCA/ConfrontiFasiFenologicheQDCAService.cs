using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaCoreModelsSTD.costanti;
using AgronicaNetCore.Agenda.DAL.DataLayer.Agenda;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Resources;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.ConfrontiFasiFenologicheQDCA.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.SpecieVegetaliXStadiCrescita;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.ConfrontiFasiFenologicheQDCA
{
    /// <summary>
    /// Implementazione del servizio di confronto e sincronizzazione fasi fenologiche QDCA.
    /// Legge le fasi storiche registrate nel QDCA tramite <see cref="IAgenda.LeggiImpiantiAsync"/>,
    /// identifica le fasi nuove (non duplicate) in base al codice BBCH e costruisce
    /// gli oggetti <see cref="Attivita"/> pronti per la persistenza nel Quaderno di Campagna.
    /// </summary>
    /// <remarks>
    /// Design Specification: DS02-BL ConfrontiFasiFenologicheQDCA — Descrizione e Regole di Business.
    /// FR006 — Verifica e sincronizzazione delle fasi fenologiche BBCH.
    /// </remarks>
    public sealed class ConfrontiFasiFenologicheQDCAService : BaseService, IConfrontiFasiFenologicheQDCAService
    {
        private readonly IAgenda _agendaDal;
        private readonly ISpecieVegetaliXStadiCrescita _specieVegetaliXStadiCrescitaDal;
        private readonly IStringLocalizer<Messages> _localizer;

        public ConfrontiFasiFenologicheQDCAService(IServiceProvider provider) : base(provider)
        {
            _agendaDal = provider.GetRequiredService<IAgenda>();
            _specieVegetaliXStadiCrescitaDal = provider.GetRequiredService<ISpecieVegetaliXStadiCrescita>();
            _localizer = provider.GetRequiredService<IStringLocalizer<Messages>>();
        }

        /// <inheritdoc />
        public async Task<ConfrontiFasiFenologicheQDCAResult> ConfrontaAsync(
            ConfrontiFasiFenologicheQDCAInput input,
            AgronicaCoreParametriServer objParametriServer,
            CancellationToken cancellationToken = default)
        {
            // Fail fast — tutti i parametri identificativi sono obbligatori.
            if (input is null) throw new ArgumentNullException(nameof(input));
            if (string.IsNullOrWhiteSpace(input.Piva))
                throw new ArgumentException("La PIVA è obbligatoria.", nameof(input));
            if (input.ProgettoCod <= 0)
                throw new ArgumentException("Il ProgettoCod deve essere maggiore di zero.", nameof(input));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));

            cancellationToken.ThrowIfCancellationRequested();

            var fasiNuove = new List<FaseFenologicaQDCAInput>();
            var fasiDuplicate = new List<FaseFenologicaQDCAInput>();
            var fasiNonRiconosciute = new List<FaseFenologicaQDCAInput>();
            var listaAttivita = new List<Attivita>();

            // ── Step 4: Data operazione QDCA = MAX(data_fase) ─────────────────
            // Include tutte le fasi ricevute (storiche e predittive).
            // Riferimento: DS02-BL — Regole di Business — Data operazione QDCA.
            DateTime dataOperazione = input.ElencoFasi.Count > 0
                ? input.ElencoFasi.Max(f => f.DataFase)
                : DateTime.UtcNow;

            DataTable dtSpecieVarietaXStadiCrescita = await _specieVegetaliXStadiCrescitaDal.LeggiAsync(input.VegCod,0,0,
                                                                                                        false,false,false, objParametriServer);

            if(dtSpecieVarietaXStadiCrescita is not null && dtSpecieVarietaXStadiCrescita.Rows.Count > 0)
            {
                // ── Step 2a: Costruzione set di codici BBCH riconosciuti dalla tabella ──
                // Iterazione su TUTTE le righe di SpecieVegetaliXStadiCrescita per la specie
                // richiesta. Solo le fasi il cui BbchCod compare in questo set possono essere
                // salvate come Attivita; quelle assenti confluiranno in FasiNonRiconosciute.
                // Riferimento: DS02-BL — Regole di Business — Confronto per unicità.
                var bbchValidi = new HashSet<string>(StringComparer.Ordinal);
                foreach (DataRow rowVal in dtSpecieVarietaXStadiCrescita.Rows)
                {
                    if (rowVal["Stadio_Principale"] != DBNull.Value)
                    {
                        var bbchStr = rowVal["Stadio_Principale"].ToString()!;
                        var sc = rowVal["Seconda_Cifra"] != DBNull.Value ? rowVal["Seconda_Cifra"].ToString() : null;
                        var tc = rowVal["Terza_Cifra"]   != DBNull.Value ? rowVal["Terza_Cifra"].ToString()   : null;

                        if (!string.IsNullOrEmpty(sc)) bbchStr += sc;
                        if (!string.IsNullOrEmpty(tc)) bbchStr += tc;

                        bbchValidi.Add(bbchStr);
                    }
                }
                // ── Step 1: Lettura fasi storiche dal QDCA ────────────────────────
                // Riferimento: DS02-BL — Regole di Business — Lettura da QDCA delle fasi storiche.
                // Usa LeggiImpiantiAsync con:
                //   - Id_Agenda:         lista vuota (non filtriamo per agenda)
                //   - Id_Esercizi:       progettoCod (filtra per l'esercizio corrente)
                //   - Lav_cod:           79 (LAVCOD_FASI_FENOLOGICHE = Rilievo Fioritura / Fasi Fenologiche)
                //   - leggiDettagli_Tecnici: true → include la colonna Mov_Dettaglio_Tecnico.FF_Classe (codice BBCH)
                DataTable dtQDCA = await _agendaDal.LeggiImpiantiAsync(
                    Id_Agenda: new List<int>(),
                    objParametriServer: objParametriServer,
                    leggiDettagli_Tecnici: true,
                    Id_Esercizi: new List<int> { input.ProgettoCod },
                    Lav_cod: new List<int> { LAV_COD.LAVCOD_FASI_FENOLOGICHE });

                // ── Step 2: Raccolta dei Cod_SS (FF_Classe) già presenti nel QDCA ─────
                // Sul QDCA viene salvato solo il Cod_SS della tabella SpecieVegetaliXStadiCrescita
                // nella colonna FF_Classe di Mov_Dettaglio_Tecnico.
                // Riferimento: DS02-BL — Regole di Business — Confronto per unicità.
                var codSsList = new List<int>();
                foreach (DataRow row in dtQDCA.Rows)
                {
                    if (row["FF_Classe"] != DBNull.Value)
                    {
                        int codSs = Convert.ToInt32(row["FF_Classe"]);
                        if (codSs > 0)
                            codSsList.Add(codSs);
                    }
                }

                // ── Step 2b: Risoluzione Cod_SS → codice BBCH tramite SpecieVegetaliXStadiCrescita ──
                // Per ogni Cod_SS già presente nel QDCA, ricostruisce il codice BBCH come stringa
                // dalla scomposizione per cifre (Stadio_Principale, Seconda_Cifra, Terza_Cifra).
                // Il confronto avviene sulle stesse colonne usate in Step 5, evitando l'uso di
                // ID_BBCH che è una chiave interna e non il codice BBCH numerico completo.
                // Riferimento: DS02-BL — Regole di Business — Confronto per unicità.
                var bbchEsistenti = new HashSet<string>(StringComparer.Ordinal);
                if (codSsList.Count > 0)
                {
                    DataRow[] filteredRows = dtSpecieVarietaXStadiCrescita.Select($"Cod_SS IN ({string.Join(",", codSsList)})");

                    if (filteredRows is not null && filteredRows.Length > 0)
                    {
                        foreach (DataRow row in filteredRows)
                        {
                            if (row["Stadio_Principale"] != DBNull.Value)
                            {
                                var bbch = row["Stadio_Principale"].ToString()!;
                                var secondaCifra = row["Seconda_Cifra"] != DBNull.Value ? row["Seconda_Cifra"].ToString() : null;
                                var terzaCifra   = row["Terza_Cifra"]  != DBNull.Value ? row["Terza_Cifra"].ToString()  : null;

                                if (!string.IsNullOrEmpty(secondaCifra))
                                    bbch += secondaCifra;
                                if (!string.IsNullOrEmpty(terzaCifra))
                                    bbch += terzaCifra;

                                bbchEsistenti.Add(bbch);
                            }
                        }
                    }
                }

                // ── Step 3: Confronto per unicità — codice BBCH come chiave ───────
                // Il confronto avviene in tre stadi:
                //   1. Il codice è già nel QDCA → duplicato (non salvare).
                //   2. Il codice non è nel QDCA ma è nella tabella → nuovo (da salvare).
                //   3. Il codice non è riconosciuto nella tabella → non riconosciuto (non salvare).
                // Riferimento: DS02-BL — Regole di Business — Confronto per unicità.

                foreach (var fase in input.ElencoFasi)
                {
                    if (!string.IsNullOrEmpty(fase.BbchCod) && bbchEsistenti.Contains(fase.BbchCod))
                        fasiDuplicate.Add(fase);
                    else if (string.IsNullOrEmpty(fase.BbchCod) || !bbchValidi.Contains(fase.BbchCod))
                        fasiNonRiconosciute.Add(fase);
                    else
                        fasiNuove.Add(fase);
                }


                // ── Step 5: Costruzione oggetti Attivita per le fasi nuove ────────
                // Un singolo oggetto Attivita per impianto con lav_cod = 79 (Rilievo Fasi Fenologiche).
                // Riferimento: DS02-BL — Output lista_attivita.
                // Framework.md — Procedura di Salvataggio Fasi Fenologiche.

                if (fasiNuove.Count > 0)
                {
                    var EsercizioCDC = new AgronicaCoreModelsSTD.attivita.centri_di_costo.EsercizioCDC()
                    {
                        esercizio = new Esercizio(input.ProgettoCod, "")
                        {
                            impiantoPK = new Impianto.PK(input.IdReg, input.Appezza, input.SaCod, input.Piva)
                        },
                        superficieTrattata = input.SuperficieImpianto
                    };

                    // NOTA: attivita.risorse dovrebbe contenere un DettaglioRilievo per ogni fase nuova
                    // (Framework.md — Gestione Dati Fenologici - Struttura Attività Rilievo).
                    var attivita = new Attivita
                    {
                        tipo = Attivita.Tipo_Attivita.QuadernoDiCampagna,
                        job = new Lavorazione(LAV_COD.LAVCOD_FASI_FENOLOGICHE),
                        inizio = dataOperazione,
                        codice = "0",
                        raccoglitore = 0,
                        centroAziendale = new CentroAziendale(
                            new CentroAziendale.PK(input.SaCod, input.Piva)),
                        risorse = new List<Risorsa>(),
                        centriDiCosto = new List<AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto>()
                        {
                            EsercizioCDC
                        },
                        note = _localizer.GetString("NotaFaseFenoDSSNutrizione")
                    };

                    foreach (var fase in fasiNuove)
                    {
                        //Fase che può essere composta al massimo da 3 elementi (più comune che siano 2),
                        //in questo caso devo fare il count degli elementi per capire quali colonne filtrare
                        
                        if(fase.BbchCod != "" && fase.BbchCod.Length > 0)
                        {
                            string FilterExpression = $"Stadio_Principale = {fase.BbchCod[0]}";

                            if(fase.BbchCod.Length == 2)
                                FilterExpression += $" AND Seconda_Cifra = {fase.BbchCod[1]}";

                            if (fase.BbchCod.Length == 3)
                                FilterExpression += $" AND Terza_Cifra = {fase.BbchCod[2]}";

                            DataRow[] filteredRows = dtSpecieVarietaXStadiCrescita.Select(FilterExpression);

                            if (filteredRows.Length == 1)
                            {
                                var bbchCodeInt = int.TryParse(fase.BbchCod, out var bbch) ? bbch : 0;

                                var Cod_SS = filteredRows[0]["Cod_SS"] != DBNull.Value ? Convert.ToInt32(filteredRows[0]["Cod_SS"]) : 0;

                                attivita.risorse.Add(new DettaglioRilievo
                                {
                                    unitaDiMisura = new UnitaDiMisura(0),
                                    QtaRilevata = 0,
                                    QtaRilevataString = fase.DataFase.ToShortDateString(),
                                    esercizioCDC = new AgronicaCoreModelsSTD.attivita.centri_di_costo.EsercizioRilievoCDC()
                                    {
                                        esercizio = EsercizioCDC.esercizio,
                                        superficieTrattata = EsercizioCDC.superficieTrattata,
                                        dataRiferimento = fase.DataFase
                                    },
                                    faseFenologica = new FaseFenologica
                                    {
                                        codice = Cod_SS,
                                        descrizione = "",
                                        stadioCrescitaBBCH = new BaseCodeDescr
                                        {
                                            codice = bbchCodeInt,
                                            descrizione = fase.BbchDescrizione
                                        },
                                        specieVegetale = new Specie
                                        {
                                            codice = input.VegCod,
                                            descrizione = ""
                                        }
                                    },
                                    DataOraRilievo = fase.DataFase
                                });
                            }
                        }

                    }

                    if(attivita.risorse is not null && attivita.risorse.FindIndex(r=>r.classType == ClassType.DettaglioRilievo) >= 0)
                        listaAttivita.Add(attivita);
                }
            }
            else
            {
                // Se la tabella SpecieVegetaliXStadiCrescita non restituisce dati per la specie,
                // nessuna fase può essere riconosciuta né salvata come Attivita.
                fasiNonRiconosciute.AddRange(input.ElencoFasi);
            }

            return new ConfrontiFasiFenologicheQDCAResult
            {
                FasiNuove            = fasiNuove.AsReadOnly(),
                FasiDuplicate        = fasiDuplicate.AsReadOnly(),
                FasiNonRiconosciute  = fasiNonRiconosciute.AsReadOnly(),
                DataOperazioneQDCA   = dataOperazione,
                ListaAttivita     = listaAttivita.AsReadOnly()
            };
        }
    }
}
