using AgronicaCoreAPI.models;
using AgronicaCoreAPI.models.entities;
using AgronicaCoreEntityFrameworkSTD_POCO.Models;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda;
using AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiComuni;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Operazione.DAL.DataLayer.Agenda;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using static AgronicaCoreModelsSTD.GiasAPP.SincroWeb2App.DatiAzienda.DatiAziendaEntity;

namespace AgronicaCoreAPI.adapters
{
    public class ModelloAdapter
    {
        private List<ProdottiEntity> datiProdotti;

        public ModelloAdapter()
        {
            datiProdotti = new List<ProdottiEntity>();
        }

        public List<LavorazioneEntity> leggiLavorazioni(string dati)
        {
            var objects = JsonConvert.DeserializeObject<List<APP_Operazioni>>(dati);
            return (from x in objects
                    select new LavorazioneEntity()
                    {
                        codice = x.lav_cod,
                        descrizione = x.lav_des,
                        classType = "Lavorazione"
                    }).ToList();
        }

        public List<AttivitaCDGEntity> leggiAttivitaCDG(string dati)
        {
            var objects = JsonConvert.DeserializeObject<List<APP_Attivita>>(dati);
            return (from x in objects
                    select new AttivitaCDGEntity()
                    {
                        codice = x.Id_Attivita,
                        descrizione = x.Desc,
                        classType = "AttivitaCDG"
                    }).ToList();
        }

        public List<LavorazioneAttivitaCDGEntity> leggiLavorazioniAttivitaCDG(string dati)
        {
            var objects = JsonConvert.DeserializeObject<List<APP_AttivitaXOperazioni>>(dati);
            return (from x in objects
                    where x.Lav_Cod > 0
                    select new LavorazioneAttivitaCDGEntity()
                    {
                        lavorazioneId = x.Lav_Cod,
                        attivitaCDGId = x.Id_Attivita
                    }).ToList();
        }

        public List<OperazioniCombinazioniEntity> leggiOperazioniCombinazioni(string dati)
        {
            return JsonConvert.DeserializeObject<List<OperazioniCombinazioniEntity>>(dati);
        }

        public List<AvversitaEntity> leggiAvversita(string dati)
        {
            var objects = JsonConvert.DeserializeObject<List<APP_Avversita>>(dati);
            return (from x in objects
                    select new AvversitaEntity()
                    {
                        codice = x.Av_Cod,
                        descrizione = x.Av_Des_Vol,
                        gruppoCod = x.Av_Gru
                    }).ToList();
        }

        public List<GruppoAvversitaEntity> leggiGruppoAvversita(string dati)
        {
            var objects = JsonConvert.DeserializeObject<List<APP_GruppoAvversita>>(dati);
            return (from x in objects
                    select new GruppoAvversitaEntity()
                    {
                        codice = x.Av_Gru,
                        descrizione = x.Av_Gru_Des
                    }).ToList();
        }

        public List<GruppoAvversitaAttiveEntity> leggiGruppoAvversitaAttive(string dati)
        {
            var objects = JsonConvert.DeserializeObject<List<APP_GruppoAvversitaAttive>>(dati);
            return (from x in objects
                    select new GruppoAvversitaAttiveEntity()
                    {
                        codice = x.Av_Gru,
                        descrizione = x.Av_Gru_Des
                    }).ToList();
        }

        public List<InfestantiAttiveEntity> leggiInfestantiAttive(string dati)
        {
            var objects = JsonConvert.DeserializeObject<List<APP_InfestantiAttive>>(dati);
            return (from x in objects
                    select new InfestantiAttiveEntity()
                    {
                        codice = x.Av_Cod,
                        gruppo = x.Av_Gru,
                        descrizione = x.Av_Des_Vol
                    }).ToList();
        }

        public List<AvversitaSpecieEntity> leggiAvversitaSpecie(string dati)
        {
            var objects = JsonConvert.DeserializeObject<List<APP_AvversitaxSpecie>>(dati);
            return (from x in objects
                    select new AvversitaSpecieEntity()
                    {
                        avversitaCod = x.Av_Cod,
                        specieCod = x.Veg_Cod
                    }).ToList();
        }

        public List<CategorieUnitaMisuraEntity> leggiCategorieUnitaMisura(string dati)
        {
            var objects = JsonConvert.DeserializeObject<List<UnitaMisuraEntity>>(dati);
            return (from x in objects
                    select new CategorieUnitaMisuraEntity()
                    {
                        codice = x.Udm_Cod,
                        descrizione = x.Udm_des,
                        simbolo = x.Udm_Sim,
                        elemCod = x.Elem_Cod,
                        nomeComune = x.NomeComune,
                        tipoControlloCod = x.TipoControllo_Cod
                    }).ToList();
        }

        public List<SpecieVegetaliStadiCrescitaEntity> leggiSpecieVegetaliStadiCrescita(string dati)
        {
            var objects = JsonConvert.DeserializeObject<List<APP_SpecieVegetaliXStadiCrescita>>(dati);
            return (from x in objects
                    select new SpecieVegetaliStadiCrescitaEntity()
                    {
                        codice = x.Cod_SS,
                        codiceFF = x.FF_Cod,
                        descrizione = x.Descrizione,
                        fioritura = x.Flag_Fioritura != 0,
                        codiceMS = x.Cod_MS,
                        ID_BBCH = x.ID_BBCH,
                        progressivo = x.Progressivo,
                        visibile = x.Flag_Visibile != 0,
                        specieCod = x.Veg_Cod
                    }).ToList();
        }

        public void leggiMisureAvversita(string dati, DatiComuniEntity datiComuni)
        {
            var datiMisure = JsonConvert.DeserializeObject<DatiMisurevversita>(dati);

            datiComuni.misureAvversita = (
                from x in datiMisure.ListaAPP_MisuraxAvversita
                select new MisuraAvversitaEntity()
                {
                    avversitaCod = x.AV_COD,
                    codice = x.COD,
                    dataAggiornamento = x.DATA_AGG,
                    fondamentale = x.Fondamentale,
                    ff_Cod = x.ff_Cod,
                    gruppoAvversitaCod = x.AV_GRU,
                    ordine = x.ordine,
                    specieCod = x.VEG_COD,
                    udmCod = x.UDM_COD
                }).ToList();

            datiComuni.misureAvversitaAnagrafiche = (
                from x in datiMisure.ListaAPP_MisuraXAvversita_Anagrafiche
                select new MisuraAvversitaAnagraficheEntity()
                {
                    codice = x.MxAV_Cod,
                    descrizione = x.Anag_des,
                    valore = x.Anag_valore
                }).ToList();
            // anagrafeCod = x.Anag_cod
        }

        public void leggiDanniRaccolte(string dati, DatiComuniEntity datiComuni)
        {
            var datiMisure = JsonConvert.DeserializeObject<DatiDanniRaccolte>(dati);

            datiComuni.misureDanni = (
                from x in datiMisure.ListaAPP_MisuraXDanniRaccolta
                select new MisuraDanniRaccoltaEntity()
                {
                    specieCod = x.Veg_Cod,
                    codice = x.Dr_Cod,
                    udmCod = x.Udm_Cod,
                    descrizione = x.Dr_Des,
                    udmDescrizione = x.Udm_Des,
                    udmSimbolo = x.Udm_Sim,
                    flag_visibile = x.Flag_Visibile
                }).ToList();
        }

        public void leggiIndiciMaturita(string dati, DatiComuniEntity datiComuni)
        {
            var datiIndici = JsonConvert.DeserializeObject<DatiIndiciMaturita>(dati);

            datiComuni.indiciMaturita = (
                from x in datiIndici.ListaAPP_IndiciMaturita
                select new IndiciMaturitaEntity()
                {
                    codice = x.IND_MAT_COD,
                    descrizione = x.IND_MAT_DES,
                    dataAggiornamento = x.DATA_AGG,
                    lavCod = x.LAV_COD
                }).ToList();

            datiComuni.indiciMaturitaSpecieVegetali = (
                from x in datiIndici.ListaAPP_IndiciMaturitaxSpecieVegetali
                select new IndiciMaturitaSpecieVegetaliEntity()
                {
                    indiceMaturitaCod = x.IND_MAT_COD,
                    classe = x.CLASSE,
                    raccolta = x.Flag_Raccolta != 0,
                    REG_COD = x.REG_COD,
                    specieCod = x.VEG_COD,
                    dataAggiornamento = x.DATA_AGG
                }).ToList();

            datiComuni.misureIndiciMaturita = (
                from x in datiIndici.ListaAPP_MisuraXIndiciMaturita
                select new MisuraIndiciMaturitaEntity()
                {
                    indiceMaturitaCod = x.IND_MAT_COD,
                    udmCod = x.UDM_COD,
                    dataAggiornamento = x.DATA_AGG
                }).ToList();

            datiComuni.misureIndiciMaturitaAnagrafiche = (
                from x in datiIndici.ListaAPP_MisuraXIndiciMaturita_Anagrafiche
                select new MisuraIndiciMaturitaAnagraficheEntity()
                {
                    indiceMaturitaCod = x.MxIn_Cod,
                    udmCod = x.UDM_COD,
                    descrizione = x.Anag_des,
                    valore = x.Anag_valore
                }).ToList();
        }

        public ImpresaEntity leggiImpresaModello(Impresa x)
        {
            string impresaPadre = null;
            if (x.impresaPadre != null && x.impresaPadre.Count > 0)
            {
                impresaPadre = x.impresaPadre.First().partitaIva;
            }

            bool agenzia = false;
            string chiaveCUAA = null;
            if (x.codici != null && x.codici.Count > 0)
            {
                foreach (var codice in x.codici)
                {
                    if (codice.codiceAnagrafe.codice == 1349 && codice.valore == "1")
                    {
                        agenzia = true;
                    }
                    if (codice.codiceAnagrafe.codice == 1365)
                    {
                        chiaveCUAA = codice.valore;
                    }
                }
            }

            return new ImpresaEntity()
            {
                partitaIva = x.partitaIva,
                ragioneSociale = x.ragioneSociale,
                CUAA = x.CUAA,
                tipoImpresa = x.tipo_Impresa,
                impresaPadre = impresaPadre,
                codici = JsonConvert.SerializeObject(x.codici),
                indirizzi = JsonConvert.SerializeObject(x.indirizzi),
                agenzia = agenzia,
                chiaveCUAA = chiaveCUAA,
                partitaIvaReale = x.partitaIvaReale
            };
        }

        public List<ImpresaEntity> leggiImprese(string dati)
        {
            var objects = JsonConvert.DeserializeObject<List<ImpreseEntity>>(dati);
            return (from x in objects
                    select new ImpresaEntity()
                    {
                        partitaIva = x.piva,
                        ragioneSociale = x.rag_soc,
                        CUAA = x.cuaa,
                        tipoImpresa = x.tipo,
                        impresaPadre = x.padre,
                        livello = x.livello,
                        provinciaCod = x.prov,
                        provincia = x.provincia,
                        regioneCod = x.reg,
                        regione = x.regione,
                        statoCod = x.stato
                    }).ToList();
        }

        public List<ImpresaEntity> leggiAziende(string dati)
        {
            return JsonConvert.DeserializeObject<List<ImpresaEntity>>(dati).OrderBy(o => o.ragioneSociale).ToList();
        }

        /// <summary>
        /// Returns a list containing exactly the one visible company when the authenticated user can see
        /// exactly one company; returns an empty list for zero or more-than-one visible companies.
        /// Implements the visibility-detection rule defined in FS002 and DS01-BL
        /// (RilevamentoVisibilitaAziendaSingola): count == 1 → AUTO_SYNC candidate; else → MANUAL_SELECT.
        /// </summary>
        /// <param name="dati">Serialised JSON array of companies returned by LeggiImprese_APP.</param>
        /// <returns>
        /// A list with 1 <see cref="ImpresaEntity"/> if exactly one company is visible; an empty list otherwise.
        /// </returns>
        public List<ImpresaEntity> leggiAziendeVisibili(string dati)
        {
            var aziende = JsonConvert.DeserializeObject<List<ImpresaEntity>>(dati);
            return aziende?.Count == 1 ? aziende : new List<ImpresaEntity>();
        }

        public List<ImpresaEntity> leggiAziende(string dati, int gerarchia, string tipologie)
        {
            List<int> listaTipi = tipologie?.Split(',')?.Select(int.Parse)?.ToList();
            var aziende = JsonConvert.DeserializeObject<List<ImpresaEntity>>(dati);
            if (listaTipi != null && listaTipi.Count > 0)
            {
                aziende = (from x in aziende where listaTipi.Contains(x.tipoImpresa) select x).ToList();
            }
            if (gerarchia != 0)
            {
                aziende = (from x in aziende where x.foglia == (gerarchia == 1 ? 0 : 1) select x).ToList();
            }
            return aziende;
        }

        public List<GerarchiaImpreseTipologiaEntity> leggiTipologieGerarchiaImprese(string dati)
        {
            return JsonConvert.DeserializeObject<List<GerarchiaImpreseTipologiaEntity>>(dati);
        }

        public List<CentroAziendaleEntity> leggiCentriAziendali(string dati)
        {
            var objects = JsonConvert.DeserializeObject<List<CentriAziendaliEntity>>(dati);
            return (from x in objects
                    select new CentroAziendaleEntity()
                    {
                        codice = x.sa_cod,
                        descrizione = x.sa_nome,
                        partitaIva = x.piva,
                        latitude = x.lat,
                        longitude = x.lng
                    }).ToList();
        }

        public List<CentroAziendaleEntity> leggiCentriAziendaliModello(List<CentroAziendale> objects)
        {
            return (
                from x in objects
                select new CentroAziendaleEntity()
                {
                    partitaIva = x.primaryKey.partitaIva,
                    codice = x.primaryKey.codice,
                    descrizione = x.nome,
                    latitude = (double)x.lat,
                    longitude = (double)x.lng,
                    codici = JsonConvert.SerializeObject(x.codici),
                    indirizzi = JsonConvert.SerializeObject(x.indirizzi)
                }).ToList();
        }

        public List<CampoEntity> leggiCampiModello(List<Campo> objects)
        {
            return (
                from x in objects
                select new CampoEntity()
                {
                    partitaIva = x.primaryKey.centroAziendalePK.partitaIva,
                    centroAziendaleCod = x.primaryKey.centroAziendalePK.codice,
                    codice = x.primaryKey.codice,
                    descrizione = x.descrizione
                }).ToList();
        }

        public List<DestinazioneUsoEntity> leggiDestinazioniUso(List<DestinazioneUso> objects)
        {
            return (
                from x in objects
                select new DestinazioneUsoEntity()
                {
                    codice = x.codice,
                    descrizione = x.descrizione
                }).ToList();
        }

        public List<SpecieEntity> leggiSpecie(List<Specie> objects, List<int> specie)
        {
            return (
                from x in objects
                where x.codice != 0 && (specie == null || specie.Contains(x.codice))
                select new SpecieEntity()
                {
                    codice = x.codice,
                    descrizione = x.descrizione
                }).ToList();
        }

        public List<VarietaEntity> leggiVarieta(List<Varieta> objects)
        {
            return (
                from x in objects
                select new VarietaEntity()
                {
                    codice = x.codice,
                    descrizione = x.descrizione,
                    specieCod = x.specie.codice
                }).ToList();
        }

        public List<GruppoFinalitaEntity> leggiFinalita(List<GruppoFinalita> objects)
        {
            return (
                from x in objects
                select new GruppoFinalitaEntity()
                {
                    codice = x.codice,
                    descrizione = x.descrizione,
                    specieCod = x.specieCod
                }).ToList();
        }

        public List<FabbricatoEntity> leggiMagazzini(string dati)
        {
            var objects = JsonConvert.DeserializeObject<List<APP_Magazzini>>(dati);

            return (
                from x in objects
                select new FabbricatoEntity()
                {
                    partitaIva = x.Piva,
                    centroaziendaleCod = x.Sa_Cod,
                    codice = x.Id_Destinazione,
                    descrizione = x.Ubic_Des,
                    tipoDestinazione = x.Tipo_Destinazione
                }).ToList();
        }

        public List<FabbricatoEntity> leggiMagazziniModello(List<Fabbricato> objects)
        {
            return (
                from x in objects
                select new FabbricatoEntity()
                {
                    partitaIva = x.primaryKey.centroAziendalePK.partitaIva,
                    centroaziendaleCod = x.primaryKey.centroAziendalePK.codice,
                    codice = x.primaryKey.codice,
                    descrizione = x.descrizione,
                    tipoDestinazione = x.tipo,
                    usoDaTerzi = x.usoDaTerzi,
                    indirizzo = JsonConvert.SerializeObject(x.indirizzo)
                }).ToList();
        }


        public PianoColturaleEntity leggiPianoColturale(List<Appezzamento> appezzamenti)
        {
            var pianoColturale = new PianoColturaleEntity();
            var destinazioniUso = new List<DestinazioneUsoEntity>();
            var specie = new List<SpecieEntity>();
            var varieta = new List<VarietaEntity>();
            var finalita = new List<GruppoFinalitaEntity>();

            foreach (var x in appezzamenti)
            {

                pianoColturale.appezzamenti.Add(new AppezzamentoEntity()
                {
                    partitaIva = x.primaryKey.centroAziendalePK.partitaIva,
                    centroAziendaleCod = x.primaryKey.centroAziendalePK.codice,
                    campoCod = (x.campoPK != null ? x.campoPK.codice : 0),
                    codice = x.primaryKey.codice,
                    nome = x.descrizione,
                    codiceAnagrafe = x.rif_Appezzamento,
                    superficie = x.superficie,
                    indirizzi = JsonConvert.SerializeObject(x.indirizzi),
                    codici = JsonConvert.SerializeObject(x.codici),
                    inizioValidita = x.validita.inizio,
                    fineValidita = x.validita.fine,
                    blkFlag = x.blkAppezzamento.blkFlag,
                    blkInizioData = x.blkAppezzamento.blkInizioData,
                    blkInizioUsername = x.blkAppezzamento.blkInizioUsername,
                    blkInizioNote = x.blkAppezzamento.blkInizioNote,
                    blkFineData = x.blkAppezzamento.blkFineData,
                    blkFineUsername = x.blkAppezzamento.blkFineUsername,
                    blkFineNote = x.blkAppezzamento.blkFineNote
                });

                foreach (var y in x.impianti)
                {
                    string codiceCatalogoAgea = string.Empty;
                    if (y.codici != null && y.codici.Count > 0)
                    {
                        foreach (var codice in y.codici)
                        {
                            if (codice.codiceAnagrafe.codice == (int)TipiEnumerativi.Enum_CodiciAnagrafe.CodiceCatalogoAgeaDemetra && codice.valore != string.Empty)
                            {
                                codiceCatalogoAgea = codice.valore;
                            }
                        }
                    }

                    pianoColturale.impianti.Add(new ImpiantoEntity()
                    {
                        codice = y.primaryKey.codice,
                        appezzamentoCod = y.primaryKey.appezzamentoPK.codice,
                        centroAziendaleCod = y.primaryKey.appezzamentoPK.centroAziendalePK.codice,
                        partitaIva = y.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva,
                        descrizione = y.descrizione,
                        utilizzoTerrenoClassType = y.utilizzoTerreno.classType,
                        utilizzoTerrenoCod = y.utilizzoTerreno.codice,
                        gruppoFinalitaCod = y.gruppoFinalita.codice,
                        superficie = y.superficie,
                        cartografia = y.cartografia,
                        StaticMapBase64String = y.immagineBase64,
                        inizioValidita = y.validita.inizio,
                        fineValidita = y.validita.fine,
                        coverCrops = y.cover_Crops,
                        codiceImpianto = y.codiceImpianto,
                        ageaIdColt = y.Agea_idColt,
                        codiceCatalogoAgea = codiceCatalogoAgea
                    });

                    foreach (var z in y.esercizi)
                    {
                        pianoColturale.esercizi.Add(new EsercizioEntity()
                        {
                            codice = z.codice,
                            impiantoCod = z.impiantoPK.codice,
                            appezzamentoCod = z.impiantoPK.appezzamentoPK.codice,
                            centroAziendaleCod = z.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                            partitaIva = z.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                            descrizione = z.descrizione,
                            inizioValidita = z.validita.inizio,
                            fineValidita = z.validita.fine,
                            resaPrevista = z.resa_prevista
                        });
                    }

                    if (y.utilizzoTerreno.classType == "Varieta")
                    {
                        var v = (Varieta)y.utilizzoTerreno;
                        varieta.Add(new VarietaEntity()
                        {
                            codice = v.codice,
                            descrizione = v.descrizione,
                            specieCod = v.specie.codice
                        });
                        specie.Add(new SpecieEntity()
                        {
                            codice = v.specie.codice,
                            descrizione = v.specie.descrizione
                        });
                        if (y.gruppoFinalita.codice != 0)
                        {
                            finalita.Add(new GruppoFinalitaEntity()
                            {
                                codice = y.gruppoFinalita.codice,
                                descrizione = y.gruppoFinalita.descrizione,
                                specieCod = v.specie.codice
                            });
                        }
                    }
                    else
                    {
                        destinazioniUso.Add(new DestinazioneUsoEntity()
                        {
                            codice = y.utilizzoTerreno.codice,
                            descrizione = y.utilizzoTerreno.descrizione
                        });
                    }
                }
            }

            pianoColturale.specie = specie.GroupBy(x => new { x.codice }).Select(g => g.First()).ToList();
            pianoColturale.varieta = varieta.GroupBy(x => new { x.codice }).Select(g => g.First()).ToList();
            pianoColturale.destinazioniUso = destinazioniUso.GroupBy(x => new { x.codice }).Select(g => g.First()).ToList();
            pianoColturale.finalita = finalita.GroupBy(x => new { x.codice, x.specieCod }).Select(g => g.First()).ToList();

            return pianoColturale;
        }

        public PianoColturaleEntity leggiPianoColturale(string dati)
        {
            var objects = JsonConvert.DeserializeObject<List<RegImpiantiEntity>>(dati);

            var pianoColturale = new PianoColturaleEntity();

            pianoColturale.centriAziendali = (from x in objects
                                              select new CentroAziendaleEntity()
                                              {
                                                  codice = x.sa_cod,
                                                  descrizione = x.sa_nome,
                                                  partitaIva = x.piva
                                              }).GroupBy(x => new { x.codice }).Select(g => g.First()).ToList();

            pianoColturale.destinazioniUso = (from x in objects
                                              where x.veg_cod == 0
                                              select new DestinazioneUsoEntity()
                                              {
                                                  codice = x.id_cod,
                                                  descrizione = x.codici_anagrafe_des
                                              }).GroupBy(x => new { x.codice }).Select(g => g.First()).ToList();

            pianoColturale.specie = (from x in objects
                                     where x.veg_cod != 0
                                     select new SpecieEntity()
                                     {
                                         codice = x.veg_cod,
                                         descrizione = x.veg_des
                                     }).GroupBy(x => new { x.codice }).Select(g => g.First()).ToList();

            pianoColturale.varieta = (from x in objects
                                      where x.veg_cod != 0
                                      select new VarietaEntity()
                                      {
                                          codice = x.cul_cod,
                                          descrizione = x.cul_des,
                                          specieCod = x.veg_cod
                                      }).GroupBy(x => new { x.codice }).Select(g => g.First()).ToList();

            pianoColturale.finalita = (from x in objects
                                       where x.veg_cod != 0
                                       select new GruppoFinalitaEntity()
                                       {
                                           codice = x.grfi_cod,
                                           descrizione = x.grfi_des,
                                           specieCod = x.veg_cod
                                       }).GroupBy(x => new { x.codice, x.specieCod }).Select(g => g.First()).ToList();

            pianoColturale.appezzamenti = (from x in objects
                                           select new AppezzamentoEntity()
                                           {
                                               codice = x.appezza,
                                               nome = x.app_nome,
                                               centroAziendaleCod = x.sa_cod,
                                               campoCod = x.campo_cod,
                                               partitaIva = x.piva,
                                               codiceAnagrafe = x.codici_anagrafe_appezzamento,
                                               superficie = x.sup_app,
                                               inizioValidita = x.validita_inizio_appezza,
                                               fineValidita = x.validita_fine_appezza,
                                               blkFlag = x.Blk_Flag,
                                               blkInizioData = x.Blk_Inizio_Data,
                                               blkInizioUsername = x.Blk_Inizio_Username,
                                               blkInizioNote = x.Blk_Inizio_Note,
                                               blkFineData = x.Blk_Fine_Data,
                                               blkFineUsername = x.Blk_Fine_Username,
                                               blkFineNote = x.Blk_Fine_Note

                                           }).GroupBy(x => new { x.codice, x.centroAziendaleCod }).Select(g => g.First()).ToList();

            pianoColturale.impianti = (from x in objects
                                       select new ImpiantoEntity()
                                       {
                                           codice = x.id_reg,
                                           appezzamentoCod = x.appezza,
                                           centroAziendaleCod = x.sa_cod,
                                           partitaIva = x.piva,
                                           descrizione = x.imp_des,
                                           utilizzoTerrenoClassType = x.veg_cod == 0 ? "DestinazioneUso" : "Varieta",
                                           utilizzoTerrenoCod = x.veg_cod == 0 ? x.id_cod : x.cul_cod,
                                           gruppoFinalitaCod = x.grfi_cod,
                                           superficie = x.sup_imp,
                                           StaticMapBase64String = x.StaticMapBase64String,
                                           inizioValidita = x.validita_inizio_impianto,
                                           fineValidita = x.validita_fine_impianto,
                                           coverCrops = x.cover == 1
                                       }).GroupBy(x => new { x.codice, x.appezzamentoCod, x.centroAziendaleCod }).Select(g => g.First()).ToList();

            pianoColturale.esercizi = (from x in objects
                                       select new EsercizioEntity()
                                       {
                                           codice = x.progetto_cod,
                                           impiantoCod = x.id_reg,
                                           appezzamentoCod = x.appezza,
                                           centroAziendaleCod = x.sa_cod,
                                           partitaIva = x.piva,
                                           descrizione = x.progetto,
                                           inizioValidita = x.validita_inizio_distinta,
                                           fineValidita = x.validita_fine_distinta
                                       }).GroupBy(x => new { x.codice }).Select(g => g.First()).ToList();

            return pianoColturale;
        }


        public void leggiProdotti(string dati, DatiAziendaEntity datiAzienda)
        {
            var prodotti = JsonConvert.DeserializeObject<List<ProdottiEntity>>(dati);
            if (datiAzienda.prodotti == null) datiAzienda.prodotti = new List<ProdottoEntity>();

            datiProdotti.AddRange(prodotti);
            datiAzienda.prodotti.AddRange((
                from x in prodotti
                select new ProdottoEntity()
                {
                    codice = x.Prodotto_Cod,
                    descrizione = x.Prodotto_Des,
                    nomeComune = x.NomeComune,
                    elemCod = x.Elem_Cod,
                    unitaDiMisuraCod = x.Udm_Cod,
                    specieCod = x.Veg_Cod,
                    N = x.N,
                    P2O5 = x.P2O5,
                    K20 = x.K2O,
                    Cu = x.Cu
                }).ToList());
        }

        public void leggiProdottiGiacenze(string dati, DatiAziendaEntity datiAzienda, bool magazzini = false)
        {
            var prodottiGiacenze = JsonConvert.DeserializeObject<List<APP_Prodotti_Giacenze>>(dati);
            if (datiAzienda.prodottiGiacenze == null) datiAzienda.prodottiGiacenze = new List<RilevamentoDiMagazzinoEntity>();

            datiAzienda.prodottiGiacenze = (
                from x in prodottiGiacenze
                select new RilevamentoDiMagazzinoEntity()
                {
                    partitaIva = x.Piva,
                    centroaziendaleCod = x.Sa_Cod,
                    elemCod = x.Elem_Cod,
                    prodottoCod = x.Prodotto_Cod,
                    fabbricatoCod = x.Fabbricato_Cod,
                    descrizione = x.Fabbricato_Des,
                    cod_progetto = x.Cod_Progetto,
                    cal_cod = x.Cal_Cod,
                    lotto = x.Lotto,
                    tipo = 0,
                    unitaDiMisuraCod = x.Udm_Cod,
                    quantita = (double)x.Giacenza,
                    LavCodCompatibiliFormulati = x.LavCodCompatibiliFormulati
                }).ToList(); // GroupBy(x => new { x.prodottoCod, x.fabbricatoCod }).Select(g => g.First())

            foreach (var prodotto in datiProdotti)
            {
                int unitaDiMisuraCod = 0;
                foreach (var giacenza in datiAzienda.prodottiGiacenze.Where(x => x.prodottoCod == prodotto.Prodotto_Cod && x.elemCod == prodotto.Elem_Cod))
                {
                    // imposto valori NPK
                    giacenza.N = (double)prodotto.N;
                    giacenza.K20 = (double)prodotto.K2O;
                    giacenza.P2O5 = (double)prodotto.P2O5;
                    giacenza.Cu = (double)prodotto.Cu;
                    unitaDiMisuraCod = giacenza.unitaDiMisuraCod;
                }
                // forzo unita di misura su prodotto
                if (unitaDiMisuraCod != 0)
                {
                    foreach (var p in datiAzienda.prodotti.Where(x => x.codice == prodotto.Prodotto_Cod))
                    {
                        p.unitaDiMisuraCod = unitaDiMisuraCod;
                    }
                }
            }

            if (magazzini)
            {
                datiAzienda.magazzini = (
                    from x in prodottiGiacenze
                    select new FabbricatoEntity()
                    {
                        centroaziendaleCod = x.Sa_Cod,
                        codice = x.Fabbricato_Cod,
                        descrizione = x.Fabbricato_Des,
                        partitaIva = x.Piva,
                        tipoDestinazione = x.Tipo_Destinazione
                    }).GroupBy(x => new { x.codice, x.centroaziendaleCod }).Select(g => g.First()).ToList();
            }
        }

        public List<RilevamentoDiMagazzinoEntity> leggiRilevamentiMagazzini(string dati)
        {
            var prodottiGiacenze = JsonConvert.DeserializeObject<List<APP_Prodotti_Giacenze>>(dati);

            return (
                from x in prodottiGiacenze
                select new RilevamentoDiMagazzinoEntity()
                {
                    partitaIva = x.Piva,
                    centroaziendaleCod = x.Sa_Cod,
                    elemCod = x.Elem_Cod,
                    prodottoCod = x.Prodotto_Cod,
                    fabbricatoCod = x.Fabbricato_Cod,
                    descrizione = x.Fabbricato_Des,
                    cod_progetto = x.Cod_Progetto,
                    cal_cod = x.Cal_Cod,
                    lotto = x.Lotto,
                    tipo = 0,
                    unitaDiMisuraCod = x.Udm_Cod,
                    quantita = (double)x.Giacenza,
                    LavCodCompatibiliFormulati = x.LavCodCompatibiliFormulati
                }).ToList();
        }

        public List<CodificaProdottoEntity> leggiCodificaProdotti(string dati)
        {
            var prodotti = JsonConvert.DeserializeObject<List<CodificaProdottiEntity>>(dati);

            return (
                from x in prodotti
                select new CodificaProdottoEntity()
                {
                    elemCod = x.Elem_Cod,
                    codice = x.Codice_GIAS,
                    descrizione = x.Desc_GIAS,
                    codProdotto = x.Cod_Prodotto_Cliente,
                    descProdotto = x.Desc_Prodotto_Cliente,
                    catProdotto = x.Categoria_Prodotto_Cliente,
                    partitaIva = x.Piva,
                    codArticolo = x.Cod_Articolo,
                    tipoCodifica = x.Tipo_Codifica
                }).ToList();
        }

        public void leggiDisciplinari(List<Disciplinare> dati, DatiComuniEntity datiComuni)
        {
            datiComuni.disciplinari = (
                from x in dati
                select new DisciplinareEntity()
                {
                    codice = x.codice,
                    descrizione = x.descrizione,
                    regConcimazioneCod = x.regolamentoConcimazione.codice,
                    regConcimazioneDesc = x.regolamentoConcimazione.descrizione,
                    disciplinarePubblicoPrivato = x.disciplinarePubblicoPrivato,
                    flagProtetto = x.flagProtetto,
                    idTr = x.idTr
                }).ToList();
        }

        public List<ProdottoEntity> leggiProdottiOnline(string dati, string piva)
        {
            var prodotti = JsonConvert.DeserializeObject<List<ProdottiEntity>>(dati);

            return (
                from x in prodotti
                where string.IsNullOrEmpty(piva) || x.Piva == piva
                select new ProdottoEntity()
                {
                    //Lorenzo C. Per ora commentato da rivedere quando si svilupperanno le Trappole 
                    //IsTrappolaFormulato = x.IsTrappolaFormulato,
                    codice = x.Prodotto_Cod,
                    descrizione = x.Prodotto_Des,
                    nomeComune = x.NomeComune,
                    elemCod = x.Elem_Cod,
                    unitaDiMisuraCod = x.Udm_Cod,
                    specieCod = x.Veg_Cod,
                    N = x.N,
                    P2O5 = x.P2O5,
                    K20 = x.K2O,
                    Cu = x.Cu
                }).GroupBy(x => new { x.codice, x.elemCod }).Select(g => g.First()).ToList();
        }

        public void leggiContatti(string dati, DatiComuniEntity datiComuni)
        {
            var contatti = JsonConvert.DeserializeObject<List<APP_Contatti>>(dati);
            datiComuni.risorseUmane = leggiRisorseUmane(contatti);
            datiComuni.contatti = leggiContatti(contatti);
        }

        public void leggiFornitori(string dati, DatiComuniEntity datiComuni)
        {
            var fornitori = JsonConvert.DeserializeObject<List<APP_Contatti>>(dati);
            datiComuni.risorseFornitori = leggiRisorseUmane(fornitori);
            datiComuni.fornitori = leggiContatti(fornitori);
        }

        public void leggiContatti(string dati, DatiAziendaEntity datiAzienda)
        {
            var contatti = JsonConvert.DeserializeObject<List<APP_Contatti>>(dati);
            datiAzienda.risorseUmane = leggiRisorseUmane(contatti);
            datiAzienda.contatti = leggiContatti(contatti);
        }

        public void leggiFornitori(string dati, DatiAziendaEntity datiAzienda)
        {
            var fornitori = JsonConvert.DeserializeObject<List<APP_Contatti>>(dati);
            if (datiAzienda.risorseFornitori.Any())
                datiAzienda.risorseFornitori.AddRange(leggiRisorseUmane(fornitori));
            else
                datiAzienda.risorseFornitori = leggiRisorseUmane(fornitori);

            if (datiAzienda.fornitori.Any())
                datiAzienda.fornitori.AddRange(leggiContatti(fornitori));
            else
                datiAzienda.fornitori = leggiContatti(fornitori);
        }

        public void leggiContattiStazioniMeteo(string dati, DatiAziendaEntity datiAzienda)
        {
            if (dati != string.Empty)
            {
                var fornitori = JsonConvert.DeserializeObject<List<APP_Contatti>>(dati);
                if (datiAzienda.risorseFornitori.Any())
                    datiAzienda.risorseFornitori.AddRange(leggiRisorseUmane(fornitori));
                else
                    datiAzienda.risorseFornitori = leggiRisorseUmane(fornitori);

                if (datiAzienda.fornitori.Any())
                    datiAzienda.fornitori.AddRange(leggiContatti(fornitori));
                else
                    datiAzienda.fornitori = leggiContatti(fornitori);
            }
        }

        public List<RisorseUmaneEntity> leggiRisorseUmane(List<APP_Contatti> contatti)
        {
            return (
                from x in contatti
                select new RisorseUmaneEntity()
                {
                    codice = x.Cod_RisUm,
                    validitaFrom = x.Validita_Inizio,
                    validitaTo = x.Validita_Fine,
                    settore = "",
                    attivita = "",
                    rapportoContabileCod = x.Cod_Rapporto,
                    codiceContatto = x.Cod_Contatto,
                    partitaIvaContatto = x.Piva
                }).ToList();
        }

        public List<ContattoEntity> leggiContatti(List<APP_Contatti> contatti)
        {
            return (
                from x in contatti
                let datiPatentino = x.DatiPatentino?.Split('|')
                select new ContattoEntity()
                {
                    codice = x.Cod_Contatto,
                    partitaIva = x.Piva,
                    centroAziendaleCod = x.Sa_Cod,
                    codiceFiscale = x.Cod_Contatto,
                    nome = x.Nome,
                    cognome = x.Cognome,
                    nomeBreve = string.IsNullOrEmpty(x.Nome) || string.IsNullOrEmpty(x.Cognome) ? x.rag_soc : x.Nome + " " + x.Cognome,
                    ragioneSociale = x.rag_soc,
                    isPublic = x.Sa_Cod == -1,
                    badge = x.NrBadge,
                    validitaFrom = AgronicaCoreDataProviderSTD.CostantiPersonalizzate.AGRODATAINIZIO,
                    validitaTo = AgronicaCoreDataProviderSTD.CostantiPersonalizzate.AGRODATAFINE,
                    //validitaFrom = x.Validita_Inizio,
                    //validitaTo = x.Validita_Fine,
                    nrPatentino = datiPatentino != null && datiPatentino.Length > 0 ? datiPatentino[0] : null,
                    dataRilascioPatentino = datiPatentino != null && datiPatentino.Length > 1 && DateTime.TryParse(datiPatentino[1], out var dataRilascio) ? dataRilascio : (DateTime?)null,
                    dataScadenzaPatentino = datiPatentino != null && datiPatentino.Length > 2 && DateTime.TryParse(datiPatentino[2], out var dataScadenza) ? dataScadenza : (DateTime?)null,
                    dataNascita = x.Data_Nascita,
                    sesso = x.Sesso
                }).ToList();
        }

        public void leggiMacchine(string dati, DatiComuniEntity datiComuni)
        {
            var macchine = JsonConvert.DeserializeObject<List<MacchineEntity>>(dati);
            // datiComuni.macchine = leggiMacchine(macchine);
            datiComuni.parcoMacchine = leggiParcoMacchine(macchine);
        }

        public void leggiMacchine(string dati, DatiAziendaEntity datiAzienda)
        {
            var macchine = JsonConvert.DeserializeObject<List<MacchineEntity>>(dati);
            // datiAzienda.macchine = leggiMacchine(macchine);
            datiAzienda.parcoMacchine = leggiParcoMacchine(macchine);
        }

        public List<MacchinaEntity> leggiMacchine(List<MacchineEntity> macchine)
        {
            var data = DateTime.Now.Date;
            return (
                from x in macchine
                where x.Validita_Inizio <= data && x.Validita_Fine >= data
                select new MacchinaEntity()
                {
                    codice = x.Mac_Cod,
                    descrizione = x.Mac_Des
                }).ToList();
        }

        public List<ParcoMacchineEntity> leggiParcoMacchine(string dati)
        {
            var macchine = JsonConvert.DeserializeObject<List<MacchineEntity>>(dati);
            return leggiParcoMacchine(macchine);
        }

        public List<ParcoMacchineEntity> leggiParcoMacchine(List<MacchineEntity> macchine)
        {
            var data = DateTime.Now.Date;
            return (
                from x in macchine
                where x.Validita_Inizio <= data && x.Validita_Fine >= data
                select new ParcoMacchineEntity()
                {
                    partitaIva = x.Piva,
                    centroAziendaleCod = x.Sa_Cod,
                    codice = x.Mac_Cod,
                    descrizione = x.Mac_Des,
                    modello = x.Modello,
                    macchinaCod = x.Mac_Cod,
                    validitaFrom = x.Validita_Inizio,
                    validitaTo = x.Validita_Fine,
                    codiceAnagrafe = x.Codice,
                    tipoMacchinaCod = x.Class_Code,
                    titoloPossessoCod = x.TitoloPossesso,
                    finalitaCod = x.Tipo,
                    proprietario = x.Denominazione_Proprietario,
                    targa = x.Targa,
                    numImmatricolazione = x.N_Immatricolazione,
                    dataImmatricolazione = x.Data_Immatricolazione,
                    BTM_Serial = x.BTM_Serial,
                    VIN = x.VIN,
                    Img_Thumbnail = x.Img_Thumbnail,
                    Img_Thumbnail_FileName = x.Img_Thumbnail_FileName,
                    Img_Thumbnail_Extension = x.Img_Thumbnail_Extension,
                    Img_Large = x.Img_Large,
                    Img_Large_FileName = x.Img_Large_FileName,
                    Img_Large_Extension = x.Img_Large_Extension,
                    Distinta_Installazione = x.Distinta_Installazione,
                    Contratto_Installazione = x.Contratto_Installazione,
                    Tipologia_Installazione = x.Tipologia_Installazione,
                    Data_Inizio_Installazione = x.Data_Inizio_Installazione,
                    Data_Fine_Installazione = x.Data_Fine_Installazione,
                    Stato_Installazione = x.Stato_Installazione,
                    Provincia_Istat_Installazione = x.Provincia_Istat_Installazione,
                    Comune_Istat_Installazione = x.Comune_Istat_Installazione,
                    Indirizzo_Installazione = x.Indirizzo_Installazione,
                    Latitudine_Installazione = x.Latitudine_Installazione,
                    Longitudine_Installazione = x.Longitudine_Installazione,
                    contattoCod = x.Cod_Contatto,
                    visibileCtrlGestione = x.Visibile_ctrl_gestione
                }).ToList();
        }

        public void leggiTipologieDocumento(string dati, DatiComuniEntity datiComuni)
        {
            var tipologie = JsonConvert.DeserializeObject<List<APP_Tipologie>>(dati);

            datiComuni.areeTipologie = (
                from x in tipologie
                select new AreaTipologieDocumentoEntity()
                {
                    codice = x.ID_Area,
                    descrizione = x.Nome_Area
                }).GroupBy(x => new { x.codice }).Select(g => g.First()).ToList();

            datiComuni.tipologie = (
                from x in tipologie
                select new TipologiaDocumentoEntity()
                {
                    codice = x.ID_Tipologia,
                    descrizione = x.Nome_Tipologia,
                    areaCod = x.ID_Area
                }).ToList();
        }

        public List<ProgettoEntity> leggiProgetti(string dati, string piva)
        {
            var objects = JsonConvert.DeserializeObject<List<APP_Imputazioni_Fasi>>(dati);

            return (
                from x in objects
                select new ProgettoEntity()
                {
                    partitaIva = piva,
                    codice = x.Imputazione_Cod,
                    descrizione = x.Imputazione_Nome,
                    attivitaCDGCod = x.Id_Attivita
                }).ToList();
        }

        public List<CentroAziendaleAttivitaCDGEntity> leggiCentriAziendaliAttivitaCDG(string dati, string piva)
        {
            var objects = JsonConvert.DeserializeObject<List<APP_AttivitaXCentri_Aziendali>>(dati);

            return (
                from x in objects
                select new CentroAziendaleAttivitaCDGEntity()
                {
                    partitaIva = piva,
                    centroAziendaleCod = x.Sa_Cod,
                    attivitaCDGCod = x.ID_Attivita,
                    inclusa = x.Inclusa
                }).ToList();
        }

        public List<ImpostazioneEntity> leggiImpostazioni(string dati, string piva)
        {
            var objects = JsonConvert.DeserializeObject<List<ImpostazioniEntity>>(dati);

            return (
                from x in objects
                select new ImpostazioneEntity()
                {
                    partitaIva = x.Piva,
                    centroAziendaleCod = x.Sa_Cod,
                    codice = x.Impostazione_Cod,
                    valore = x.Impostazione_Valore
                }).ToList();
        }

        public List<TipoMacchinaEntity> leggiTipiMacchine(string dati)
        {
            var objects = JsonConvert.DeserializeObject<List<TipoMacchineEntity>>(dati);

            return (
                from x in objects
                select new TipoMacchinaEntity()
                {
                    codice = x.CLASS_CODE,
                    descrizione = x.CLASS_DESC
                }).GroupBy(x => new { x.codice }).Select(g => g.First()).ToList();
        }

        public List<NazioneEntity> leggiNazioni(List<CodiciNazioniISO3166> objects, List<string> nazioni)
        {
            return (
                from x in objects
                where nazioni == null || nazioni.Contains(x.codice)
                select new NazioneEntity()
                {
                    codice = x.codice,
                    descrizione = x.descrizione
                }).ToList();
        }

        public List<RegioneEntity> leggiRegioni(List<Provincia> objects)
        {
            return (
                from x in objects
                select new RegioneEntity()
                {
                    codice = x.regione.codice,
                    descrizione = x.regione.descrizione,
                    statoCod = x.stato.codice
                }).GroupBy(x => new { x.codice, x.statoCod }).Select(g => g.First()).ToList();
        }

        public List<ProvinciaEntity> leggiProvince(List<Provincia> objects)
        {
            return (
                from x in objects
                select new ProvinciaEntity()
                {
                    codice = x.codice,
                    descrizione = x.descrizione,
                    sigla = x.sigla,
                    regioneCod = x.regione.codice,
                    regioneDes = x.regione.descrizione,
                    statoCod = x.stato.codice
                }).ToList();
        }

        public List<ComuneEntity> leggiComuni(List<Comune> objects)
        {
            return (
                from x in objects
                select new ComuneEntity()
                {
                    codice = x.codice,
                    descrizione = x.descrizione,
                    cap = x.cap,
                    provinciaCod = x.provincia.codice,
                    statoCod = x.provincia.stato.codice
                }).ToList();
        }

        public List<SpecieZootecnicaEntity> leggiSpecieZootecniche(List<RisorsaZootecnica> objects)
        {
            return (
                from x in objects
                select new SpecieZootecnicaEntity()
                {
                    genereCod = x.genere != null ? x.genere.codice : 0,
                    specieCod = x.specie != null ? x.specie.codice : 0,
                    indirizzoProdCod = x.indirizzoProd != null ? x.indirizzoProd.codice : 0,
                    descrizione = x.descrizione
                }).ToList();
        }

        public List<OperazioneCausaleEntity> leggiOperazioniCausali(DataTable dtRisorse)
        {
            return (
                from d in dtRisorse.AsEnumerable()
                select new OperazioneCausaleEntity()
                {
                    id = int.Parse(d["Id"].ToString()),
                    causale = d["Causale"].ToString(),
                    lavCod = int.Parse(d["Lav_Cod"].ToString()),
                    Validita_Inizio = DateTime.Parse(d["Validita_Inizio"].ToString()),
                    Validita_Fine = DateTime.Parse(d["Validita_Fine"].ToString())

                }).ToList();
        }


        public AgronicaCoreModelloSTD.Coordinate convertiCoordinate(List<TrackingGPSEntity> objects, string username)
        {
            int id = 1;
            var coordinate = new AgronicaCoreModelloSTD.Coordinate
            {
                APPGISList = (
                from x in objects
                select new APP_GIS()
                {
                    Id = id++,
                    DataOraRilevata = x.dataOraRilevata,
                    GisTxt = "POINT (" + string.Format(new CultureInfo("en-US"), "{0:0.000000}", x.longitude) + " " + string.Format(new CultureInfo("en-US"), "{0:0.000000}", x.latitude) + ")",
                    Nome = x.name != null ? x.name : "",
                    Cognome = x.surname != null ? x.surname : "",
                    NrBadge = x.badge != null ? x.badge : "",
                    Identif_Dispositivo = x.phone != null ? x.phone : "",
                    Data_Creazione = DateTime.Now,
                    Data_Modifica = DateTime.Now,
                    Username_Creazione = username,
                    Username_Modifica = username,
                    Validita_Inizio = AgronicaCoreDataProviderSTD.CostantiPersonalizzate.AGRODATAINIZIO,
                    Validita_Fine = AgronicaCoreDataProviderSTD.CostantiPersonalizzate.AGRODATAFINE
                }).ToList()
            };

            return coordinate;
        }

        public AgronicaCoreModelloSTD.EntrateUscite convertiEventi(List<LogEventiEntity> objects, string username)
        {
            int id = 1;
            var eventi = new AgronicaCoreModelloSTD.EntrateUscite
            {
                APPLogEventiList = (
                from x in objects
                select new APP_LogEventi()
                {
                    Id = id++,
                    DataOraRilevata = x.dataOraRilevata,
                    Evento = x.evento,
                    Nome = x.nome != null ? x.nome : "",
                    Cognome = x.cognome != null ? x.cognome : "",
                    NrBadge = x.badge != null ? x.badge : "",
                    Identif_Dispositivo = x.telefono != null ? x.telefono : "",
                    Data_Creazione = DateTime.Now,
                    Data_Modifica = DateTime.Now,
                    Username_Creazione = username,
                    Username_Modifica = username,
                    Validita_Inizio = AgronicaCoreDataProviderSTD.CostantiPersonalizzate.AGRODATAINIZIO,
                    Validita_Fine = AgronicaCoreDataProviderSTD.CostantiPersonalizzate.AGRODATAFINE
                }).ToList()
            };

            return eventi;
        }

    }
}
