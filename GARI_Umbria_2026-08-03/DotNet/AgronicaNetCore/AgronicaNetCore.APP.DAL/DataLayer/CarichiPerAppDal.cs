using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita;
using AgronicaNetCore.APP.DAL.Resources;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Text;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.APP.DAL.DataLayer
{
    public class CarichiPerAppDal : BaseDalApp, ICarichiPerAppDal
    {
        public CarichiPerAppDal(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
        }

        public async Task<List<MovimentoDiMagazzino>> LeggiCarichiPerAppAsync(string piva, DateTime dataLavorazioneMin, DateTime dataUltimaSincro, AgronicaCoreParametriServer objParametriServer)
        {
            var sb = new StringBuilder();
            DataTable dt;
            var result = new List<MovimentoDiMagazzino>();

            sb.AppendLine(" SELECT ");
            sb.AppendLine("       Id, Versione, Tipo, ISNULL(TRY_CAST(Riferimento AS INT), 0) as Id_Agenda ");
            sb.AppendLine(" INTO ");
            sb.AppendLine("       #cteAppDati ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("       APP_Dati ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine($"     Tipo IN ('{(int)DatiApp.MovimentiDemetra}', '{(int)DatiApp.Movimenti}')");
            sb.AppendLine("     AND Piva = @piva ");

            sb.AppendLine("; ");

            LogAgendaHelper.AggiungiCreazioneLogAgendaUltimaOperazione(sb);

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     au.Id_Agenda, au.Lav_Cod, au.UltimaOperazione, au.Origine, ");
            sb.AppendLine("     a.Validita_Inizio as [Data], ");
            sb.AppendLine("     m.Mov_Desc, ");
            sb.AppendLine("     mdet.Elem_Cod, mdet.Pro_Cod, mdet.Mat_Cod, mdet.Mov_Det_Des, mdet.Lotto, mdet.Udm_Cod, mdet.Qta, mdet.Prezzo_Unitario, ");
            sb.AppendLine("     mdest.Piva as PivaMagazzino, mdest.Sa_Cod as SaCodMagazzino, mdest.Id_Destinazione, mdest.Tipo_Destinazione, ");
            sb.AppendLine("     um.Udm_Sim, um.Udm_Des, ");
            sb.AppendLine("     f.Fabbricato_Des, ");
            sb.AppendLine("     ad.Id, ad.Versione, ad.Tipo ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     #ultimoLogAgenda au");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     Agenda a");
            sb.AppendLine("     ON a.Id_Agenda = au.Id_Agenda ");
            sb.AppendLine("     AND a.Piva = au.Piva ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     Movimenti m");
            sb.AppendLine("     ON m.Id_Agenda = a.Id_Agenda ");
            sb.AppendLine($"     AND m.Cau_Mov IN ('{CAU_MOV.CAU_SCARICO}', '{CAU_MOV.CAU_CARICO}') ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     Movimenti_Dettagli mdet ");
            sb.AppendLine("     ON mdet.ID_Agenda = m.Id_Agenda ");
            sb.AppendLine("     AND mdet.Id_Mov = m.Id_Mov ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     Mov_Destinazioni mdest ");
            sb.AppendLine("     ON mdest.Id_Agenda = mdet.Id_Agenda ");
            sb.AppendLine("     AND mdest.Id_Mov = mdet.Id_Mov ");
            sb.AppendLine("     AND mdest.Id_Mov_Det = mdet.Id_Mov_Det ");
            sb.AppendLine($"     AND mdest.Tipo_Destinazione = {TIPO_DESTINAZIONE.TIPO_DESTINAZIONE_MAGAZZINO} ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     Fabbricati f");
            sb.AppendLine("     ON f.Piva = mdest.Piva ");
            sb.AppendLine("     AND f.Sa_Cod = mdest.Sa_Cod");
            sb.AppendLine("     AND f.Fabbricato_Cod = mdest.Id_Destinazione ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     UnitaMisura um ");
            sb.AppendLine("     ON um.UDM_COD = mdet.Udm_Cod ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     #cteAppDati ad");
            sb.AppendLine("     ON ad.Id_Agenda = au.Id_Agenda ");
            sb.AppendLine(" ORDER BY ");
            sb.AppendLine("     au.Data_Ora_Lavorazione, au.Id_Agenda ");

            sb.AppendLine(" ; ");

            sb.AppendLine(" DROP TABLE #cteAppDati; ");
            sb.AppendLine(" DROP TABLE #cteLogAgenda; ");
            sb.AppendLine(" DROP TABLE #ultimoLogAgenda; ");

            var parameters = new Dictionary<string, object>
            {
                { "@piva", piva },
                { "@dataLavorazioneMin", dataLavorazioneMin },
                { "@dataUltimaSincro", dataUltimaSincro }
            };

            Dictionary<string, Dictionary<Type, List<object>>> parametersIn = new()
            {
                {
                    "@lavCods",
                    FormatClauseIn(new List<int>()
                    {
                        LAV_COD.LAVCOD_CARICO,
                        LAV_COD.LAVCOD_SCARICO
                    })
                }
            };

            try
            {
                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sb.ToString(), parameters, parametersIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            if (dt is not null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    try
                    {
                        var movimentoDiMagazzino = new MovimentoDiMagazzino()
                        {
                            documenti = new List<AgronicaCoreModelsSTD.documenti.Documento>(),
                        };

                        var idAgenda = (int)row["Id_Agenda"];
                        movimentoDiMagazzino.Codice = idAgenda.ToString();

                        if (row["Id"] != System.DBNull.Value)
                        {
                            var guid = (string)row["Id"];
                            movimentoDiMagazzino.guid = guid;
                        }

                        if (row["Versione"] != System.DBNull.Value)
                        {
                            var versione = (string)row["Versione"];
                            movimentoDiMagazzino.versione = versione;
                        }

                        if (row["Tipo"] != System.DBNull.Value)
                        {
                            var tipo = (string)row["Tipo"];
                            movimentoDiMagazzino.origine = tipo;
                        }

                        var ultimaOperazione = (int)row["UltimaOperazione"];
                        if (ultimaOperazione == (int)enum_TipoOperazioneDB.Cancellazione)
                        {
                            movimentoDiMagazzino.cancellato = true;
                            result.Add(movimentoDiMagazzino);
                            continue;
                        }

                        var origineModifica = row.Field<int>("Origine");
                        movimentoDiMagazzino.definitivo = origineModifica == (int)Enum_CACSistemaCodEnum.Gias;

                        ValorizzaCampiMovimentoMagazzinoDaRigaTabella(row, movimentoDiMagazzino);

                        var note = (string)row["Mov_Desc"];
                        movimentoDiMagazzino.Note = note;

                        result.Add(movimentoDiMagazzino);
                    }
                    catch (Exception ex)
                    {
                        LogError(ex.Message, objParametriServer, ex);
                    }
                }
            }

            return result;
        }

        public async Task<List<Acquisto>> LeggiAcquistiPerAppAsync(string piva, DateTime dataLavorazioneMin, DateTime dataUltimaSincro, AgronicaCoreParametriServer objParametriServer)
        {
            var sb = new StringBuilder();
            DataTable dt;
            var result = new List<Acquisto>();

            sb.AppendLine(" SELECT ");
            sb.AppendLine("       Id, Versione, Tipo, ISNULL(TRY_CAST(Riferimento AS INT), 0) as Id_Agenda ");
            sb.AppendLine(" INTO ");
            sb.AppendLine("       #cteAppDati ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("       APP_Dati ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine($"     Tipo IN ('{(int)DatiApp.AcquistiDemetra}', '{(int)DatiApp.Acquisti}')");
            sb.AppendLine("     AND Piva = @piva");

            sb.AppendLine("; ");

            LogAgendaHelper.AggiungiCreazioneLogAgendaUltimaOperazione(sb);

            sb.AppendLine(" SELECT ");
            sb.AppendLine("     au.Id_Agenda, au.Lav_Cod, au.UltimaOperazione, au.Origine, ");
            sb.AppendLine("     a.Validita_Inizio as Data, ");
            sb.AppendLine("     m.Cod_RisUm, Cod_IndirizzoRisUm, m.Cau_Mov, ");
            sb.AppendLine("     m.Doc_Numero, ISNULL(m.Doc_Numero_Des, '') as Doc_Numero_Des, ISNULL(m.Doc_Numero_Sin, '') as Doc_Numero_Sin, ISNULL(m.Doc_Numero_Visualizzato, '') as Doc_Numero_Visualizzato, ");
            sb.AppendLine("     m.Ora as DataSpedizione, m.Extra_Str, ");
            sb.AppendLine("     mdet.Elem_Cod, mdet.Pro_Cod, mdet.Mat_Cod, mdet.Mov_Det_Des, mdet.Lotto, mdet.Udm_Cod, mdet.Qta, mdet.Prezzo_Unitario, ");
            sb.AppendLine("     mdest.Piva as PivaMagazzino, mdest.Sa_Cod as SaCodMagazzino, mdest.Id_Destinazione, mdest.Tipo_Destinazione, ");
            sb.AppendLine("     um.Udm_Sim, um.Udm_Des, ");
            sb.AppendLine("     f.Fabbricato_Des, ");
            sb.AppendLine("     ru.Settore_Des, ru.Attivita_Des, ");
            sb.AppendLine("     c.Rag_Soc as RagSocContatto, c.Piva as PivaContatto, c.Cod_Contatto, c.Codice_Fiscale, c.Nome, c.Cognome, ");
            sb.AppendLine("     rc.Cod_Rapporto, rc.Rapporto_Des,");
            sb.AppendLine("     ad.Id, ad.Versione, ad.Tipo ");
            sb.AppendLine(" FROM ");
            sb.AppendLine("     #ultimoLogAgenda au ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     Agenda a");
            sb.AppendLine("     ON a.Id_Agenda = au.Id_Agenda ");
            sb.AppendLine("     AND a.Piva = au.Piva ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     Movimenti m");
            sb.AppendLine("     ON m.Id_Agenda = a.Id_Agenda ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     Movimenti_Dettagli mdet ");
            sb.AppendLine("     ON mdet.ID_Agenda = m.Id_Agenda ");
            sb.AppendLine("     AND mdet.Id_Mov = m.Id_Mov ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     Mov_Destinazioni mdest ");
            sb.AppendLine("     ON mdest.Id_Agenda = mdet.Id_Agenda ");
            sb.AppendLine("     AND mdest.Id_Mov = mdet.Id_Mov ");
            sb.AppendLine("     AND mdest.Id_Mov_Det = mdet.Id_Mov_Det ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     Fabbricati f");
            sb.AppendLine("     ON f.Piva = mdest.Piva ");
            sb.AppendLine("     AND f.Sa_Cod = mdest.Sa_Cod");
            sb.AppendLine("     AND f.Fabbricato_Cod = mdest.Id_Destinazione ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     UnitaMisura um ");
            sb.AppendLine("     ON um.UDM_COD = mdet.Udm_Cod ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     Risorse_Umane ru ");
            sb.AppendLine("     ON ru.Cod_RisUm = m.Cod_RisUm ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     Contatti c ");
            sb.AppendLine("     ON c.Piva = ru.Piva ");
            sb.AppendLine("     AND c.Cod_Contatto = ru.Cod_Contatto ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     Rapporti_Contabili rc ");
            sb.AppendLine("     ON rc.Cod_Rapporto = ru.Cod_Rapporto ");
            sb.AppendLine(" LEFT JOIN ");
            sb.AppendLine("     #cteAppDati ad");
            sb.AppendLine("     ON ad.Id_Agenda = au.Id_Agenda ");
            sb.AppendLine(" ORDER BY ");
            sb.AppendLine("     au.Data_Ora_Lavorazione, au.Id_Agenda ");

            sb.AppendLine(" ; ");

            sb.AppendLine(" DROP TABLE #cteAppDati; ");
            sb.AppendLine(" DROP TABLE #cteLogAgenda; ");
            sb.AppendLine(" DROP TABLE #ultimoLogAgenda; ");

            var parameters = new Dictionary<string, object>
            {
                { "@piva", piva },
                { "@dataLavorazioneMin", dataLavorazioneMin },
                { "@dataUltimaSincro", dataUltimaSincro }
            };

            Dictionary<string, Dictionary<Type, List<object>>> parametersIn = new()
            {
                {
                    "@lavCods",
                    FormatClauseIn(new List<int>()
                    {
                        LAV_COD.LAVCOD_BOLLA_RICEVUTA,
                    })
                }
            };

            try
            {
                dt = await GetDataProvider(objParametriServer).ExecuteReadAsync(sb.ToString(), parameters, parametersIn);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, objParametriServer, ex);
                throw;
            }

            if (dt is not null && dt.Rows.Count > 0)
            {
                var rowsGroupedByIdAgenda = dt.AsEnumerable()
                    .GroupBy(row => row.Field<int>("Id_Agenda"))
                    .ToDictionary(group => group.Key, group => group.ToList());

                foreach (var rowGroup in rowsGroupedByIdAgenda)
                {
                    try
                    {
                        var id_agenda = rowGroup.Key;
                        var rows = rowGroup.Value;
                        var acquisto = new Acquisto();
                        acquisto.codice = id_agenda.ToString();

                        if (rows.Count == 1 && (int)rows[0]["UltimaOperazione"] == (int)enum_TipoOperazioneDB.Cancellazione) //cancellazione
                        {
                            acquisto.cancellato = true;
                            ValorizzaProprietaAcquistoDaAppDati(rows[0], acquisto);
                            result.Add(acquisto);
                            continue;
                        }

                        var rigaTestata = rows.FirstOrDefault(r => (string)r["Cau_Mov"] == CAU_MOV.CAU_REGISTRAZIONI); //ci dovrebbe essere solo una riga di testata
                        if (rigaTestata is null)
                        {
                            throw new Exception("Mancano i dati per la testata di un acquisto");
                        }

                        var righeMovimenti = rows.Where(r => (string)r["Cau_Mov"] == CAU_MOV.CAU_CARICO).ToList();
                        if (!righeMovimenti.Any())
                        {
                            throw new Exception("Mancano i dati per i movimenti dell'acquisto");
                        }

                        ValorizzaProprietaAcquistoDaAppDati(rigaTestata, acquisto);

                        var origineModifica = rigaTestata.Field<int>("Origine");
                        acquisto.definitivo = origineModifica == (int)Enum_CACSistemaCodEnum.Gias;

                        var dataMovimento = (DateTime)rigaTestata["Data"];
                        acquisto.dataDoc = dataMovimento;

                        var docNumeroVisualizzato = rigaTestata.Field<string>("Doc_Numero_Visualizzato");
                        if (string.IsNullOrEmpty(docNumeroVisualizzato))
                        {
                            var docNumeroSin = rigaTestata.Field<string>("Doc_Numero_Sin");
                            var docNumero = rigaTestata.Field<double?>("Doc_Numero");
                            var docNumeroDes = rigaTestata.Field<string>("Doc_Numero_Des");

                            var docNumeroStr = docNumero is null ? string.Empty : docNumero.ToString();

                            acquisto.numDoc = $"{docNumeroSin}{docNumeroStr}{docNumeroDes}";
                        }
                        else
                        {
                            acquisto.numDoc = docNumeroVisualizzato;
                        }

                        var cod_risum = rigaTestata.Field<int>("Cod_RisUm");
                        var settore_des = rigaTestata.Field<string>("Settore_Des");
                        var attivita_des = rigaTestata.Field<string>("Attivita_Des");

                        acquisto.fornitore = new RisorseUmane(cod_risum);
                        acquisto.fornitore.settore = settore_des;
                        acquisto.fornitore.attivita = attivita_des;

                        var ragSocContatto = rigaTestata.Field<string>("RagSocContatto");
                        var pivaContatto = rigaTestata.Field<string>("PivaContatto");
                        var codContatto = rigaTestata.Field<string>("Cod_Contatto");
                        var codiceFiscaleContatto = rigaTestata.Field<string>("Codice_Fiscale");
                        var nomeContatto = rigaTestata.Field<string>("Nome");
                        var cognomeContatto = rigaTestata.Field<string>("Cognome");

                        acquisto.fornitore.contatto = new Contatto()
                        {
                            primaryKey = new Contatto.PK(pivaContatto, codContatto),
                            codiceFiscale = codiceFiscaleContatto,
                            ragione_Sociale = ragSocContatto,
                            nome_Breve = ragSocContatto,
                            nome = nomeContatto,
                            cognome = cognomeContatto,
                        };

                        var codRapporto = rigaTestata.Field<int>("Cod_Rapporto");
                        var rapportoDes = rigaTestata.Field<string>("Rapporto_Des");

                        acquisto.fornitore.rapportoContabile = new RapportoContabile(codRapporto)
                        {
                            descrizione = rapportoDes
                        };

                        var dataSpedizione = rigaTestata.Field<DateTime>("DataSpedizione");
                        acquisto.data = dataSpedizione;

                        var note = rigaTestata.Field<string>("Extra_Str");
                        acquisto.note = note;

                        acquisto.movimenti = new();

                        foreach (var rigaMovimento in righeMovimenti)
                        {
                            var movimentoDiMagazzino = new MovimentoDiMagazzino()
                            {
                                documenti = new List<AgronicaCoreModelsSTD.documenti.Documento>(),
                            };
                            ValorizzaCampiMovimentoMagazzinoDaRigaTabella(rigaMovimento, movimentoDiMagazzino);
                            movimentoDiMagazzino.Note = "";

                            acquisto.movimenti.Add(movimentoDiMagazzino);
                        }

                        var codiceCentro = acquisto.movimenti.First().Magazzino.primaryKey.centroAziendalePK.codice;
                        var pivaCentro = acquisto.movimenti.First().Magazzino.primaryKey.centroAziendalePK.partitaIva;
                        acquisto.centroAziendale = new CentroAziendale(new CentroAziendale.PK(codiceCentro, pivaCentro));
                        acquisto.centroAziendale.nome = "";
                        acquisto.documenti = new List<AgronicaCoreModelsSTD.documenti.Documento>();

                        result.Add(acquisto);
                    }
                    catch (Exception ex)
                    {
                        LogError(ex.Message, objParametriServer, ex);
                    }
                }
            }

            return result;
        }

        private void ValorizzaProprietaAcquistoDaAppDati(DataRow dr, Acquisto acquisto)
        {
            if (dr["Id"] != System.DBNull.Value)
            {
                var guid = (string)dr["Id"];
                acquisto.guid = guid;
            }

            if (dr["Versione"] != System.DBNull.Value)
            {
                var versione = (string)dr["Versione"];
                acquisto.versione = versione;
            }

            if (dr["Tipo"] != System.DBNull.Value)
            {
                var tipo = (string)dr["Tipo"];
                acquisto.origine = tipo;
            }
        }

        private void ValorizzaCampiMovimentoMagazzinoDaRigaTabella(DataRow row, MovimentoDiMagazzino movimentoDiMagazzino)
        {
            var pivaMagazzino = (string)row["PivaMagazzino"];
            var saCodMagazzino = (int)row["SaCodMagazzino"];
            var fabbCod = (int)row["Id_Destinazione"];
            var fabbricato_des = (string)row["Fabbricato_Des"];
            var tipoDestinazione = row.Field<int>("Tipo_Destinazione");

            movimentoDiMagazzino.Magazzino = new AgronicaCoreModelsSTD.anagrafiche.Fabbricato(pivaMagazzino, saCodMagazzino, fabbCod, fabbricato_des) 
            {
                tipo = tipoDestinazione,
            };

            var data = (DateTime)row["Data"];
            movimentoDiMagazzino.Data = data;

            var lavCod = (int)row["Lav_Cod"];
            if (lavCod == LAV_COD.LAVCOD_CARICO)
            {
                movimentoDiMagazzino.Tipo = RilevamentoDiMagazzino.RilevamentoMagazzinoTipo.carico;
            }
            else if (lavCod == LAV_COD.LAVCOD_SCARICO)
            {
                movimentoDiMagazzino.Tipo = RilevamentoDiMagazzino.RilevamentoMagazzinoTipo.scarico;
            }
            else if (lavCod == LAV_COD.LAVCOD_BOLLA_RICEVUTA)
            {
                movimentoDiMagazzino.Tipo = RilevamentoDiMagazzino.RilevamentoMagazzinoTipo.prodotto;
            }

            var elem_cod = (int)row["Elem_Cod"];
            var pro_cod = (int)row["Pro_Cod"];
            var mat_cod = (int)row["Mat_Cod"];
            var descProdotto = (string)row["Mov_Det_Des"];

            var codiceProdotto = 0;
            if (pro_cod > 0)
            {
                codiceProdotto = pro_cod;
            }
            else if (mat_cod > 0)
            {
                codiceProdotto = -mat_cod;
            }

            var udm_cod = (int)row["Udm_Cod"];
            var udm_sim = row.Field<string>("Udm_Sim");
            var udm_des = row.Field<string>("Udm_Des");
            movimentoDiMagazzino.Prodotto = new AgronicaCoreModelsSTD.attivita.risorse.Prodotto(codiceProdotto, elem_cod)
            {
                descrizione = descProdotto,
                unitaDiMisura = new AgronicaCoreModelsSTD.metaschema.UnitaDiMisura(udm_cod)
                {
                    simbolo = udm_sim,
                    descrizione = udm_des,
                },
            };

            var lotto = (string)row["Lotto"];
            var qta = (decimal)(double)row["Qta"];
            var prezzo = (decimal)(double)row["Prezzo_Unitario"];

            movimentoDiMagazzino.Lotto = lotto;
            movimentoDiMagazzino.UdM = new AgronicaCoreModelsSTD.metaschema.UnitaDiMisura(udm_cod) 
            {
                simbolo = udm_sim,
                descrizione = udm_des,
            };
            movimentoDiMagazzino.Qta = qta;
            movimentoDiMagazzino.Prezzo = prezzo;
        }
    }
}
