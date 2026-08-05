using System.Data;
using Microsoft.VisualBasic;
using Microsoft.Extensions.DependencyInjection;
using InData.Zoo;
using InData.Anagrafica;
using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.attivita.risorse;
using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Magazzino.DAL.DataLayer;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.Farmaci;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiImpostazioni;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Imprese_Impostazioni;

using static AgronicaCoreModelsSTD.attivita.Attivita;
using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Magazzino.BIZ.Services
{
    public class MagazzinoService : BaseService, IMagazzinoService
    {
        private readonly IUtentiImpostazioni _utentiImpostazioni;
        private readonly IImprese_Impostazioni _impreseImpostazioni;
        private readonly IMagazzino _magazzinoDAL;
        private readonly IFarmaci _farmaciDAL;

        public MagazzinoService(IServiceProvider provider) : base(provider)
        {
            _utentiImpostazioni = provider.GetRequiredService<IUtentiImpostazioni>();
            _impreseImpostazioni = provider.GetRequiredService<IImprese_Impostazioni>();
            _magazzinoDAL = provider.GetRequiredService<IMagazzino>();
            _farmaciDAL = provider.GetRequiredService<IFarmaci>();
        }

        private async Task<int> getCompanySettings_Magazzini(string piva, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            string defaultValue = "1";
            string val = await _impreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(
                piva, 
                null, 
                Enum_Impostazioni_Utenti.SUPERUSER_GESTIONE_MAGAZZINO_ABILITATA, 
                ELEM_COD.FARMACI,
                defaultValue, 
                objP_Utenti, 
                objP_Server
            );
            return int.Parse(val);
        }

        private async Task<enum_Gestione_Giacenze> getCompanySettings_Giacenze(string piva, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            string defaultValue = ((int)enum_Gestione_Giacenze.TuttiProdotti).ToString();
            string val = await _impreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(
                piva, 
                null,
                Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE, 
                ELEM_COD.FARMACI,
                defaultValue, 
                objP_Utenti, 
                objP_Server
            );
            return (enum_Gestione_Giacenze)int.Parse(val);
        }
        
        private async Task<enum_Gestione_Lotti> getCompanySettings_Lotti(string piva, AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            string defaultValue = ((int)enum_Gestione_Lotti.Nessuna).ToString();
            string val = await _impreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser_ElemCod(
                piva, 
                null,
                Enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI, 
                ELEM_COD.FARMACI,
                defaultValue, 
                objP_Utenti, 
                objP_Server
            );
            return (enum_Gestione_Lotti)int.Parse(val);
        }
        
        private async Task<string> getUserSettings_BlockGiacenze(AgronicaCoreParametriServer objP_Server, AgronicaCoreParametriUtenti objP_Utenti)
        {
            int usernameUtente = 1;
            string defaultValue = "0";
            return await _utentiImpostazioni.LeggiConDefault(
                Enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE,
                usernameUtente,
                defaultValue,
                objP_Utenti, 
                objP_Server
            );
        }

        private static enum_Tipo_Operazione_Agenda getTipoOperazione(Tipo_Attivita tipoAttivita, Stati statoAttivita)
        {
            var opAgendaType = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna;
            switch (tipoAttivita)
            {
                case Tipo_Attivita.QuadernoDiCampagna:
                    opAgendaType = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna;
                    break;
                case Tipo_Attivita.Ricetta:
                    opAgendaType = (statoAttivita == Stati.Eseguita) ? enum_Tipo_Operazione_Agenda.RicettaBrogliaccio : enum_Tipo_Operazione_Agenda.Ricetta;
                    //if (statoAttivita == Stati.Eseguita)
                    //    opAgendaType = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio;
                    // else
                    //    opAgendaType = enum_Tipo_Operazione_Agenda.Ricetta;
                    break;
            }
            return opAgendaType;
        }

        private static void GetAssettoMagazzino(
            Tipo_Attivita tipoAttivita,
            Stati statoAttivita,
            int gestioneMagazzino,
            enum_Gestione_Giacenze gestioneGiacenze,
            enum_Gestione_Lotti gestioneLotto,
            string blockGiacenza_User,
            bool ignoreEmptyGiacenze,
            ref bool useLotto,
            ref bool useMagazzino,
            ref bool useAnagrafica,
            ref bool fQtaGreaterZero
        ) {
            //useLotto = false;
            //useMagazzino = false;
            //useAnagrafica = false;
            //fQtaGreaterZero = false;

            useLotto = (gestioneLotto != enum_Gestione_Lotti.Nessuna);

            //switch (gestioneLotto)
            //{
            //    case enum_Gestione_Lotti.Nessuna:
            //        useLotto = false;
            //        break;
            //    default:
            //        useLotto = true;
            //        break;
            //}

            var tipoOperazione = getTipoOperazione(tipoAttivita, statoAttivita);

            switch (tipoOperazione)
            {
                case enum_Tipo_Operazione_Agenda.QuadernoDiCampagna:
                case enum_Tipo_Operazione_Agenda.RicettaBrogliaccio:
                    if (gestioneMagazzino == 1)
                    {
                        useMagazzino = true;

                        switch (gestioneGiacenze)
                        {
                            case enum_Gestione_Giacenze.SoloMovimentati:
                                useAnagrafica = false;
                                fQtaGreaterZero = false;
                                if (ignoreEmptyGiacenze) fQtaGreaterZero = true;
                                break;
                            case enum_Gestione_Giacenze.SoloPresenti:
                                useAnagrafica = false;
                                fQtaGreaterZero = true;
                                break;
                            case enum_Gestione_Giacenze.TuttiProdotti:
                                useAnagrafica = (!ignoreEmptyGiacenze);
                                fQtaGreaterZero = (ignoreEmptyGiacenze);
                                break;
                        }
                        
                        if (blockGiacenza_User == "1") fQtaGreaterZero = true;
                    }
                    else
                    {
                        useMagazzino = false;
                        useAnagrafica = true;
                        fQtaGreaterZero = false;
                    }
                    break;

                case enum_Tipo_Operazione_Agenda.Ricetta:
                    if (gestioneMagazzino == 1)
                    {
                        useMagazzino = true;
                        useAnagrafica = (!ignoreEmptyGiacenze);
                        fQtaGreaterZero = (ignoreEmptyGiacenze);
                    }
                    else
                    {
                        useMagazzino = false;
                        useAnagrafica = true;
                        fQtaGreaterZero = false;
                    }
                    break;
            }
        }

        public async Task<List<DettaglioRegistroSomministrazioni>> Leggi_Giacenze_Farmaci(
            LeggiGiacenzaFarmaci paramsFarmaci,
            Tipo_Attivita tipoAttivita,
            Stati statoAttivita,
            bool escludiGiacenzeZero,
            List<string> codiciAIC,
            AgronicaCoreParametriSuperServer objP_SuperServer,
            AgronicaCoreParametriServer objP_Server,
            AgronicaCoreParametriUtenti objP_Utenti
        ) {
            var farmaciList = new List<DettaglioRegistroSomministrazioni>();

            // DT: bisognerebbe prendere i centri dei fabbricati, non quelli degli impianti,
            // ma si è valutato di fermarsi a livello di super user o azienda,
            // quindi per il momento è sufficiente passare la PIVA 
            int gestioneMagazzino = await getCompanySettings_Magazzini(paramsFarmaci.Piva, objP_Server, objP_Utenti);
            var gestioneGiacenza = await getCompanySettings_Giacenze(paramsFarmaci.Piva, objP_Server, objP_Utenti);
            var gestioneLotto = await getCompanySettings_Lotti(paramsFarmaci.Piva, objP_Server, objP_Utenti);
            string blockGiacenze_User = await getUserSettings_BlockGiacenze(objP_Server, objP_Utenti);

            bool useLotto = false, useMagazzino = false, useAnagrafica = false, fQtaGreaterZero = false;
            GetAssettoMagazzino(
                tipoAttivita, 
                statoAttivita,
                gestioneMagazzino,
                gestioneGiacenza,
                gestioneLotto,
                blockGiacenze_User, 
                escludiGiacenzeZero, 
                ref useLotto, 
                ref useMagazzino, 
                ref useAnagrafica, 
                ref fQtaGreaterZero
            );

            string filtroFarmCod = "";

            var dtGiacenze = new DataTable();
            var dtGiacenzeTot = new DataTable();

            // Se utilizzo il magazzino, leggo prima i prodotti in giacenza
            if (useMagazzino)
            {
                //string xFiltroAggiuntivo_MagazzinoAttivoAllaData = " AND (Fabbricati.Validita_Inizio <= " + Agro_SQL_SaveDate(validitaFine) + " AND Fabbricati.Validita_Fine >= " + Agro_SQL_SaveDate(validitaFine) + ") " + " AND Fabbricati.CodiceBDN = '" + Agro_SQL_SaveText(codiceBDN) + "' AND Fabbricati.ChkMagazzinoFarmaci = 1 AND Fabbricati.ProprietarioCapi = '" + Agro_SQL_SaveText(codFiscaleProprietario) + "' ";
                var xFiltroAggiuntivo_MagazzinoAttivoAllaData = new FiltroAggiuntivo()
                {
                    defaultBooleanOp = Boolean_Operators.And,
                    filters = {
                        new Filter("Fabbricati.CodiceBDN", "@CodiceBDN", Comparison_Operators.Equal, Boolean_Operators.And, paramsFarmaci.codiceBDN, typeof(string)),
                        new Filter("Fabbricati.ChkMagazzinoFarmaci", "@chkMagazzinoFarmaci", Comparison_Operators.Equal, Boolean_Operators.And, 1, typeof(int))
                    }
                };

                if (paramsFarmaci.UdmCod != 0)
                    xFiltroAggiuntivo_MagazzinoAttivoAllaData.filters
                        .Add(new Filter("Movimenti_dettagli.Udm_Cod", "@UdmCod", Comparison_Operators.Equal, Boolean_Operators.And, paramsFarmaci.UdmCod, typeof(int)));

                if (paramsFarmaci.codFiscaleProprietario != null && paramsFarmaci.codFiscaleProprietario != "")
                    xFiltroAggiuntivo_MagazzinoAttivoAllaData.filters.Add(new Filter("Fabbricati.ProprietarioCapi", "@codFiscaleProprietario", Comparison_Operators.Equal, Boolean_Operators.And, paramsFarmaci.codFiscaleProprietario, typeof(string)));

                dtGiacenze = await _magazzinoDAL.SchedaGiacenzeMagazzino(paramsFarmaci.ValiditaFine, paramsFarmaci.Piva, paramsFarmaci.SaCod, 0, ELEM_COD.FARMACI, paramsFarmaci.ProCod, 0, 0, 0, 0, 0, COSTANTI_GENERALI.LOTTO_NONDEFINITO, Flag_QtaNoZero: false, null, null, null, null, null, null, null, null, null, null, null, null, "", objP_Server, objP_Utenti, xFiltroAggiuntivo_16: xFiltroAggiuntivo_MagazzinoAttivoAllaData, Flag_QtaMaggioreZero: fQtaGreaterZero);

                dtGiacenzeTot = await _magazzinoDAL.SchedaGiacenzeMagazzino(DateTime.Parse(CostantiPersonalizzate.AGRODATAFINE), paramsFarmaci.Piva, paramsFarmaci.SaCod, 0, ELEM_COD.FARMACI, 0, 0, 0, 0, 0, 0, COSTANTI_GENERALI.LOTTO_NONDEFINITO, Flag_QtaNoZero: false, null, null, null, null, null, null, null, null, null, null, null, null, "", objP_Server, objP_Utenti, xFiltroAggiuntivo_16: xFiltroAggiuntivo_MagazzinoAttivoAllaData, Flag_QtaMaggioreZero: fQtaGreaterZero);

                for (var i = 0; i <= dtGiacenze.Rows.Count - 1; i++)
                    filtroFarmCod += " Farmaci.Farm_Cod = " + dtGiacenze.Rows[i]["Pro_Cod"].ToString() + " OR ";

                if (filtroFarmCod != "")
                {
                    filtroFarmCod = $" AND ({Strings.Left(filtroFarmCod, filtroFarmCod.Length - 3)})";
                }
                else
                    // se non ho alcun farmaco in magazzino e non si vogliono prodotti presenti solo in anagrafica, si esce
                    if (!useAnagrafica) return farmaciList;
            }

            var dtResult = await _farmaciDAL.LeggiFarmaciListAICAsync(0, null, codiciAIC, objP_Server);

            var dtOrderedDrugs = new DataTable();
            dtOrderedDrugs.Columns.Add(new DataColumn("FarmDes", typeof(string)));
            dtOrderedDrugs.Columns.Add(new DataColumn("DettaglioSomministrazione", typeof(DettaglioRegistroSomministrazioni)));
            dtOrderedDrugs.Columns.Add(new DataColumn("ConGiacenza", typeof(int)));

            DataRow drow;
            var rowGiacenze = Array.Empty<DataRow>();
            var rowGiacenzeTot = Array.Empty<DataRow>();

            foreach (var rowFarmaco in dtResult.AsEnumerable())
            {
                int farmCod = (int)rowFarmaco["Farm_Cod"];
                string farmDes = (string)rowFarmaco["Denominazione"] + " - " + rowFarmaco["Confezione"];
                string codAIC = (string)rowFarmaco["AIC"];

                var dettSomm = new DettaglioRegistroSomministrazioni()
                {
                    codiceAIC = codAIC,
                    MagazziniMovimentazioni = new List<RilevamentoDiMagazzino>(),
                    prodotto = new Prodotto(farmCod, ELEM_COD.FARMACI)
                    {
                        descrizione = farmDes
                    },
                    dataPrescrizione = CostantiPersonalizzate.AGRODATAINIZIO_DATE
                };

                bool giacenzaFound = false;
                if (useMagazzino)
                {
                    if (dtGiacenze != null && dtGiacenze.Rows.Count > 0) 
                        rowGiacenze = dtGiacenze.Select("Pro_Cod = " + farmCod);

                    if (rowGiacenze != null && rowGiacenze.Length > 0)
                    {
                        for (var g = 0; g <= rowGiacenze.Length - 1; g++)
                        {
                            var rilevamentoMagazzino = new RilevamentoDiMagazzino() {
                                Qta = Math.Round((decimal)rowGiacenze[g]["Giacenza"], 4),
                                udm = new UnitaDiMisura((int)rowGiacenze[g]["Udm_Cod"])
                                {
                                    simbolo = (string)rowGiacenze[g]["Udm_Sim"]
                                },
                                Lotto = (string)rowGiacenze[g]["lotto"],
                                Magazzino = new Fabbricato() 
                                {
                                    primaryKey = new Fabbricato.PK()
                                    {
                                        centroAziendalePK = new CentroAziendale.PK() 
                                        {
                                            partitaIva = (string)rowGiacenze[g]["Piva"],
                                            codice = (int)rowGiacenze[g]["Sa_Cod"]
                                        },
                                        codice = (int)rowGiacenze[g]["Id_Destinazione"]
                                    },
                                    descrizione = (string)rowGiacenze[g]["Fabbricato_Des"] + " (" + (string)rowGiacenze[g]["Sa_Nome"] + ")",
                                    tipo = (int)rowGiacenze[g]["Tipo_Destinazione"]
                                }
                            };

                            dettSomm.unitaDiMisura = rilevamentoMagazzino.udm;

                            rowGiacenzeTot = Array.Empty<DataRow>();
                            if (dtGiacenzeTot != null && dtGiacenzeTot.Rows.Count > 0)
                            {
                                rowGiacenzeTot = dtGiacenzeTot
                                    .Select(" Piva = '" + rowGiacenze[g]["Piva"] + "' AND Sa_Cod =" + (int)rowGiacenze[g]["Sa_Cod"] + " AND Id_Destinazione=" + (int)rowGiacenze[g]["Id_Destinazione"] + " AND lotto='" + (string)rowGiacenze[g]["lotto"] + "'");
                                if (rowGiacenzeTot != null && rowGiacenzeTot.Length > 0)
                                    rilevamentoMagazzino.QtaTot = Math.Round((decimal)rowGiacenzeTot[0]["Giacenza"], 4);
                            }

                            // Se fQtaGreaterZero è attivo e il lotto non ha giacenza residua nel lungo periodo
                            // (es. protocollo retrodatato che esaurisce un lotto già consumato in futuro),
                            // il lotto non va mostrato per evitare scarichi sotto giacenza.
                            if (fQtaGreaterZero && rowGiacenzeTot.Length == 0) continue;

                            var dttTrattGiacenza = dettSomm.Clona();
                            dttTrattGiacenza.MagazziniMovimentazioni.Add(rilevamentoMagazzino);

                            drow = dtOrderedDrugs.NewRow();
                            drow["FarmDes"] = farmDes;
                            drow["DettaglioSomministrazione"] = dttTrattGiacenza;
                            drow["ConGiacenza"] = 1;
                            dtOrderedDrugs.Rows.Add(drow);

                            giacenzaFound = true;
                        }
                    }
                }

                // DT: il prodotto va restituito anche se presente solo in anagrafica se "usaAnagrafica=true"
                // DT: se si sta usando anche il magazzino, si restituisce il prodotto (senza indicazioni di giacenza) solo se non è stato trovato in magazzino (trovataGiacenza=False)
                if (useAnagrafica && !giacenzaFound)
                {
                    drow = dtOrderedDrugs.NewRow();
                    drow["FarmDes"] = farmDes;
                    drow["DettaglioSomministrazione"] = dettSomm;
                    drow["ConGiacenza"] = 0;
                    dtOrderedDrugs.Rows.Add(drow);
                }
            }

            if (dtOrderedDrugs != null)
            {
                var dv = new DataView();
                dtOrderedDrugs.TableName = "Farmaci";
                dv.Table = dtOrderedDrugs;
                dv.Sort = "ConGiacenza DESC, FarmDes ASC";

                for (var i = 0; i <= dv.Count - 1; i++)
                    farmaciList.Add((DettaglioRegistroSomministrazioni)dv[i]["DettaglioSomministrazione"]);
            }

            return farmaciList;
        }

    }
}
