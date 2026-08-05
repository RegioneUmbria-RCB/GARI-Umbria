using AgronicaCoreAPI.converters;
using AgronicaCoreAPI.models;
using AgronicaCoreEntityFrameworkSTD_POCO.Models;
using AgronicaCoreModelloSTD;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fabbricato = AgronicaCoreModelsSTD.anagrafiche.Fabbricato;

namespace AgronicaCoreAPI.adapters
{
    public class AnagraficaAdapter
    {
        public static CentroAziendale leggiCentroAziendale(string piva, int sa_cod)
        {
            Impresa impresa = new Impresa
            {
                partitaIva = piva
            };

            CentroAziendale centro = new CentroAziendale(new CentroAziendale.PK(sa_cod, piva));

            return centro;
        }

        public static Fabbricato leggiFabbricato(string piva, int sa_cod, int codice, string descrizione)
        {
            return new Fabbricato()
            {
                primaryKey = new Fabbricato.PK()
                {
                    centroAziendalePK = leggiCentroAziendale(piva, sa_cod).primaryKey,
                    codice = codice
                },
                descrizione = descrizione
            };
        }

        public static Esercizio leggiEsercizio(Reg_Impianti imp)
        {
            return leggiEsercizio(imp.piva, imp.sa_cod, imp.appezza, imp.id_reg);
        }

        public static Reg_Impianti leggiImpianto(Esercizio esercizio)
        {
            var centroPK = esercizio.impiantoPK.appezzamentoPK.centroAziendalePK;
            var impianto = new Reg_Impianti
            {
                piva = centroPK.partitaIva,
                sa_cod = centroPK.codice,
                appezza = esercizio.impiantoPK.appezzamentoPK.codice,
                id_reg = esercizio.impiantoPK.codice
            };
            return impianto;
        }

        public static Esercizio leggiEsercizio(string piva, int sa_cod, int appezza, int id_reg)
        {
            CentroAziendale centro = leggiCentroAziendale(piva, sa_cod);
            Appezzamento.PK appezzamentoPK = new Appezzamento.PK(appezza, centro.primaryKey);

            Appezzamento appezzamento = new Appezzamento(appezzamentoPK);

            Impianto.PK impiantoPrimaryKey = new Impianto.PK(id_reg, appezzamentoPK);

            Impianto impianto = new Impianto(impiantoPrimaryKey);
            

            Esercizio esercizio = new Esercizio(0, "")
            {
                impiantoPK = impianto.primaryKey
            };

            return esercizio;
        }

        public static List<Fabbricato> leggiFabbricati(string dati)
        {
            var magazzini = JsonConvert.DeserializeObject<List<MagazziniEntity>>(dati);

            var fabbricati = (from m in magazzini select new Fabbricato { 
                primaryKey = new Fabbricato.PK() { codice = m.Id_Destinazione, centroAziendalePK = new CentroAziendale.PK(m.Sa_Cod, m.Piva) },
                descrizione = m.Ubic_Des 
            }).ToList();

            return fabbricati;
        }

        public static List<Esercizio> leggiEsercizi(string dati)
        {
            var regImpianti = JsonConvert.DeserializeObject<List<RegImpiantiEntity>>(dati);

            var esercizi = (from i in regImpianti
                            select new Esercizio(i.progetto_cod, i.progetto)
                              {
                                  codice = i.progetto_cod,
                                  descrizione = i.progetto,
                                  impiantoPK = new Impianto.PK(i.id_reg, new Appezzamento.PK(i.appezza, new CentroAziendale.PK(i.sa_cod, i.piva))),
                                  validita = new IntervalloTemporale(i.validita_inizio_distinta, i.validita_fine_distinta),
                                  regolamento = new Regolamenti(i.regolamento) { descrizione = i.reg_des}
                              }).ToList();

            return esercizi;
        }

        public static List<Impianto> leggiImpianti(string dati)
        {
            var regImpianti = JsonConvert.DeserializeObject<List<RegImpiantiEntity>>(dati);

            var impianti = (from i in regImpianti
                            select new Impianto(new Impianto.PK(i.id_reg, new Appezzamento.PK(i.appezza, new CentroAziendale.PK(i.sa_cod, i.piva))))
                            {
                                descrizione = i.progetto,
                                utilizzoTerreno = i.veg_cod == 0 ? new DestinazioneUso(i.id_cod) { descrizione = i.codici_anagrafe_des } : new Varieta(i.cul_cod) { descrizione = i.cul_des, specie = new Specie(i.veg_cod) { descrizione = i.veg_des } },
                                copertura = new Copertura(i.cop_cod) { descrizione = i.copertura},
                                irrigazione = new Irrigazione(i.imp_cod) { descrizione = i.imp_des },
                                codiceImpianto = i.Codici_Anagrafe_Impianto,
                                superficie = i.sup_imp
                            }).ToList();

            return impianti;
        }

        public static List<Appezzamento> leggiAppezzamenti(string dati)
        {
            var regImpianti = JsonConvert.DeserializeObject<List<RegImpiantiEntity>>(dati);

            var appezzamenti = (from i in regImpianti
                            select new Appezzamento(new Appezzamento.PK(i.appezza, new CentroAziendale.PK(i.sa_cod, i.piva)))
                            {
                                descrizione = i.app_nome,
                                campoPK = i.campo_cod != 0 ? new Campo.PK(i.campo_cod, new CentroAziendale.PK(i.sa_cod, i.piva)) : null,
                                // codiceAppezzamento = new CodiciAnagrafeValori() { valore = i.codici_anagrafe_appezzamento },
                                rif_Appezzamento = i.codici_anagrafe_appezzamento,
                                superficie = i.sup_app
                            }).ToList();

            return appezzamenti;
        }

        public static List<CentroAziendale> leggiCentriAziendali(string dati)
        {
            var regImpianti = JsonConvert.DeserializeObject<List<RegImpiantiEntity>>(dati);

            var centriAziendali = (from i in regImpianti
                                   select new CentroAziendale(new CentroAziendale.PK(i.sa_cod, i.piva)) { nome = i.sa_nome })
                                   .GroupBy(i => new { i.primaryKey.codice }).Select(g => g.First())
                                   .ToList();

            return centriAziendali;
        }

        public static List<UtilizzoTerreno> leggiUtilizziTerreno(string dati)
        {
            var regImpianti = JsonConvert.DeserializeObject<List<RegImpiantiEntity>>(dati);
            var utilizzi = new List<UtilizzoTerreno>();                
            foreach (var i in regImpianti)
            {
                if (i.veg_cod == 0) {
                    utilizzi.Add(new DestinazioneUso(i.id_cod) { descrizione = i.codici_anagrafe_des });
                } else {
                    utilizzi.Add(new Varieta(i.cul_cod) { descrizione = i.cul_des, specie = new Specie(i.veg_cod) { descrizione = i.veg_des } });
                }
            }

            return utilizzi.GroupBy(i => new { i.classType, i.codice }).Select(g => g.First()).ToList();
            // return utilizzi.Distinct(new UtilizzoTerrenoComparer()).ToList();
        }

        /* 
        public static List<DestinazioneUso> leggiDestinazioniUso(string dati)
        {
            return JsonConvert.DeserializeObject<List<DestinazioneUso>>(dati);
        }

        public static List<Specie> leggiSpecie(string dati)
        {
            var entity = JsonConvert.DeserializeObject<List<SpecieEntity>>(dati);
            var specie = (from i in entity select new Specie(i.veg_cod) { descrizione = i.veg_des }).ToList();
            return specie;
        }

        public static List<Varieta> leggiVarieta(string dati)
        {
            var entity = JsonConvert.DeserializeObject<List<VarietaEntity>>(dati);
            var varieta = (from i in entity select new Varieta(i.cul_cod) { descrizione = i.cul_des }).ToList();
            return varieta;
        }

        public static List<GruppoFinalita> leggiGruppoFinalita(string dati)
        {
            var entity = JsonConvert.DeserializeObject<List<GruppoFinalitaEntity>>(dati);
            var finalita = (from i in entity select new GruppoFinalita(i.grfi_cod) { descrizione = i.grfi_des }).ToList();
            return finalita;
        }
        */
    }
}
