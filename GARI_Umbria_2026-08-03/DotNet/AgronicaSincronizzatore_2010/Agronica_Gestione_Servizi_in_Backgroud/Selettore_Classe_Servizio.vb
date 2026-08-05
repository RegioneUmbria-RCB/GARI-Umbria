Imports System.IO
Imports System.Text
Imports System.Threading
Imports AgronicaCoreAcciseBIZ
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreIntegrazioneMacchine
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports ImportCertificatiPomodoro
Imports INDICODE_EDI_BMI_ImportExport
Imports Interscambio_Anagrafica
Imports SmartTractors_HubIot_ImpExp
Imports ProgrammazioneNotifichePush
Imports InvioNotifichePush
Imports StoricizzazioneMeteoNT
Imports AccodamentoScadenzaProdotti
Imports SincronizzazioneEsecuzioni
Imports SincronizzazioneEntitaInattive
Imports EsecuzioneAlgoritmoProiezione
Imports CaiInvioEmail
Imports GestioneTraduzionePOEditor
Imports DataPublish_Abaco
Imports Timesheet_Calendar
Imports Esportazione_Visite_Analisi_Commerciali
Imports Interscambio_BIOrogel
Imports GiasDuplicateResx
Imports PuliziaTabelleAPP
Imports VerificaConformitaMassivaBackground
Imports Export_Squadre

Public Class Selettore_Classe_Servizio

    Function Avvia(ByVal Configurazione_Servizio As Configurazione_Servizio,
                   ByRef Messaggio_di_Ritorno_Opzionale As String,
                   ByVal ObjParametri_SuperServer As AgronicaCoreParametri,
                   ByVal ObjParametri_Server As AgronicaCoreParametri,
                   ByVal ObjParametri_Utenti As AgronicaCoreParametri
                   ) As Boolean

        'sempre questo
        If Configurazione_Servizio.Id_Servizio = enum_Id_Servizio.GiasOnline Then

            Select Case Configurazione_Servizio.Tipo_Sincro

                Case enum_Tipi_Servizi_Background.StoricizzazioneMeteo_NT
                    Dim servizio As New Storicizzazione_DatiOrari(Configurazione_Servizio)
                    Dim result As Boolean = servizio.Esegui()
                    Messaggio_di_Ritorno_Opzionale = servizio.GetMessaggio()
                    Return result

                Case enum_Tipi_Servizi_Background.ImportazioneMassivaDaAnagrafeBA

                    'invio tutti o solo i parametri necessari, ma ritorno true se ok o false se problemi,
                    'il log dettagliato lo produrrà il sincro specifico
                    'La funzione controlla periodicamente se il task è ancora attivo o 
                    'se l'ora di fine esecuzione è passata
                    Dim ImportazioneAnagrafeBA As New SincroAnagrafeBA.ImportazioneAnagrafeBA(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return ImportazioneAnagrafeBA.ImportazioneMassivaDaAnagrafeBA(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Sincronizzatore_Web
                    Dim sincroWeb As New GIAS2GIAS_LOCALE.GIAS_2_GIAS(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return sincroWeb.GIAS_2_GIAS_DaIDConf(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Sincronizzatore_Apofruit

                    Dim Impor_Sincro_Apofruit As New Sincronizzatore_Apofruit.Impor_Sincro_Apofruit(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return Impor_Sincro_Apofruit.Avvia_Importazione_Sincronizzazione_Siagr_Gias(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Importatore_UNIFORMA

                    Dim ImportatoreUniforma As New ImportatoreUniforma.Importatore(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return ImportatoreUniforma.Avvia_Importazione_Sincronizzazione_Uniforma_Gias(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_Metos

                    Dim ImportatoreMetos As New Sincronizzazione_Dati_Stazioni_Meteo.Sincro_Meteo(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return ImportatoreMetos.Avvia_Importazione_Sincronizzazione_Metos(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_CoDiMa

                    Dim importatoreWinNet As New Sincronizzazione_Dati_Stazioni_Meteo.Sincro_Meteo_CoDiMa(Configurazione_Servizio)
                    Return importatoreWinNet.Avvia_Importazione_Sincronizzazione_CoDiMa(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_NT_CoDiMa

                    Dim servizio As New ImportatoreMeteoNT.ImportatoreMeteo_CoDiMa(Configurazione_Servizio)
                    Dim result As Boolean = servizio.Esegui()
                    Messaggio_di_Ritorno_Opzionale = servizio.Messaggio
                    Return result

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_WiNet

                    Dim importatoreWinNet As New Sincronizzazione_Dati_Stazioni_Meteo.Sincro_Meteo_WiNet(Configurazione_Servizio)
                    Return importatoreWinNet.Avvia_Importazione_Sincronizzazione_WinNet(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_AgriculturalSupport

                    Dim importatoreAs As New Sincronizzazione_Dati_Stazioni_Meteo.Sincro_Meteo_AS(Configurazione_Servizio)
                    Return importatoreAs.Avvia_Importazione_Sincronizzazione_AS(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_NT_WiNet

                    Dim servizio As New ImportatoreMeteoNT.ImportatoreMeteo_WiNet(Configurazione_Servizio)
                    Dim result As Boolean = servizio.Esegui()
                    Messaggio_di_Ritorno_Opzionale = servizio.Messaggio
                    Return result

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_NT_AS

                    Dim servizio As New ImportatoreMeteoNT.ImportatoreMeteo_AS(Configurazione_Servizio)
                    Dim result As Boolean = servizio.Esegui()
                    Messaggio_di_Ritorno_Opzionale = servizio.Messaggio
                    Return result

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_NT_NetSens

                    Dim servizio As New ImportatoreMeteoNT.ImportatoreMeteo_NetSens(Configurazione_Servizio)
                    Dim result As Boolean = servizio.Esegui()
                    Messaggio_di_Ritorno_Opzionale = servizio.Messaggio
                    Return result

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_NT_Pessl

                    Dim servizio As New ImportatoreMeteoNT.ImportatoreMeteo_Pessl(Configurazione_Servizio)
                    Dim result As Boolean = servizio.Esegui()
                    Messaggio_di_Ritorno_Opzionale = servizio.Messaggio
                    Return result

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_NT_GreenPlanet

                    Dim servizio As New ImportatoreMeteoNT.ImportatoreMeteo_GreenPlanet(Configurazione_Servizio)
                    Dim result As Boolean = servizio.Esegui()
                    Messaggio_di_Ritorno_Opzionale = servizio.Messaggio
                    Return result

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_NT_A2ASmartCity

                    Dim servizio As New ImportatoreMeteoNT.ImportatoreMeteo_A2ASmartCity(Configurazione_Servizio)
                    Dim result As Boolean = servizio.Esegui()
                    Messaggio_di_Ritorno_Opzionale = servizio.Messaggio
                    Return result

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_NT_DigiFarm

                    Dim servizio As New ImportatoreMeteoNT.ImportatoreMeteo_DigiFarm(Configurazione_Servizio)
                    Dim result As Boolean = servizio.Esegui()
                    Messaggio_di_Ritorno_Opzionale = servizio.Messaggio
                    Return result

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_NT_Agrismart

                    Dim servizio As New ImportatoreMeteoNT.ImportatoreMeteo_Agrismart(Configurazione_Servizio)
                    Dim result As Boolean = servizio.Esegui()
                    Messaggio_di_Ritorno_Opzionale = servizio.Messaggio
                    Return result

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_NT_Horta

                    Dim servizio As New ImportatoreMeteoNT.ImportatoreMeteo_Horta(Configurazione_Servizio)
                    Dim result As Boolean = servizio.Esegui()
                    Messaggio_di_Ritorno_Opzionale = servizio.Messaggio
                    Return result

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_NT_Hypermeteo
                    Dim servizio As New ImportatoreMeteoNT.ImportatoreMeteo_Hypermeteo(Configurazione_Servizio)
                    Dim result As Boolean = servizio.Esegui()
                    Messaggio_di_Ritorno_Opzionale = servizio.Messaggio
                    Return result

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_NT_HypermeteoPrevisionale
                    Dim servizio As New ImportatoreMeteoNT.ImportatoreMeteo_HypermeteoPrevisionale(Configurazione_Servizio)
                    Dim result As Boolean = servizio.Esegui()
                    Messaggio_di_Ritorno_Opzionale = servizio.Messaggio
                    Return result

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_NT_RadarMeteoFTP

                    Dim servizio As New ImportatoreMeteoNT.ImportatoreMeteo_RadarMeteoFTP(Configurazione_Servizio)
                    Dim result As Boolean = servizio.Esegui()
                    Messaggio_di_Ritorno_Opzionale = servizio.Messaggio
                    Return result

                Case enum_Tipi_Servizi_Background.Importazione_Risposta_Analisi_Laboratorio

                    'ANALISI LABORATORI
                    Dim ImportatoreAnalisi_Fito As New Importazione_Analisi_Pesticidi.Importazione_analisiPesticidi(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return ImportatoreAnalisi_Fito.Avvia_Importazione_Analisi_Pesticidi(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Importazione_Risposta_Analisi_Laboratorio_EmailAllegati

                    'analisi laboratori email allegati
                    Dim ImportatoreAnalisi_Fito As New Importazione_Analisi_Pesticidi.Importazione_analisiPesticidi(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return ImportatoreAnalisi_Fito.Avvia_Importazione_Analisi_Pesticidi_EmailAllegati(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.OPTA_ImportaRicevimenti_TracciaColli

                    Return Importazioni_OPTA.Rintraccio.ImportaRicevimenti_e_RintracciaColli(Messaggio_di_Ritorno_Opzionale, Configurazione_Servizio, ObjParametri_Server)
                    'Dim servizioRintraccio As New Importazioni_OPTA.Rintraccio(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    'Return servizioRintraccio.ImportaRicevimenti_e_RintracciaColli(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Indicode_EDI_Euritmo

                    Dim ediGestore As New INDICODE_EDI_Euritmo_Importazione.INDICODE_EDI_Euritmo_Importazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return ediGestore.Avvia_Importazione(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Indicode_EDI_BMI_Esportazione

                    Dim ediGestore As New INDICODE_EDI_BMI_ImportExport.INDICODE_EDI_BMI_Esportazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return ediGestore.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Indicode_EDI_BMI_CMag

                    Dim ediGestore As New INDICODE_EDI_BMI_ImportExport.INDICODE_EDI_BMI_Esportazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return ediGestore.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Indicode_EDI_BMI_SMag

                    Dim ediGestore As New INDICODE_EDI_BMI_ImportExport.INDICODE_EDI_BMI_Esportazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return ediGestore.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Indicode_EDI_BMI_Importazione '39

                    Dim ediGestore As New INDICODE_EDI_BMI_ImportExport.INDICODE_EDI_BMI_Importazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return ediGestore.Avvia_Importazione(Messaggio_di_Ritorno_Opzionale, "")

                Case enum_Tipi_Servizi_Background.Indicode_EDI_BMI_Importazione_RIPROCESSA

                    Dim ediGestore As New INDICODE_EDI_BMI_ImportExport.INDICODE_EDI_BMI_Importazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return ediGestore.Avvia_Importazione(Messaggio_di_Ritorno_Opzionale, "")

                Case enum_Tipi_Servizi_Background.ImportazioneMassivaDaAnagrafeAVEPA

                    Dim avepa As New Importazione_Avepa.AvepaToXmlAgea
                    Return avepa.ImportazioneFascicoliAVEPA(Configurazione_Servizio.Parametri_Extra, ObjParametri_Server)

                Case enum_Tipi_Servizi_Background.ImportazioneMagazzinoXMLPubblico

                    'Test-Esempio
                    Threading.Thread.Sleep(2000)
                    Messaggio_di_Ritorno_Opzionale = "SIMULAZIONE ImportazioneMagazzinoXMLPubblico eseguita con successo"
                    Return True

                Case enum_Tipi_Servizi_Background.esportazioneSpesometro

                    'Test-Esempio
                    Threading.Thread.Sleep(2000)
                    Messaggio_di_Ritorno_Opzionale = "SIMULAZIONE esportazioneSpesometro eseguita con successo"
                    Return True

                Case enum_Tipi_Servizi_Background.Teleregistri

                    Dim espTel As New Esportazione_Teleregistri.Esportatore_Teleregistri(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return espTel.Avvia_Esportazione()

                Case enum_Tipi_Servizi_Background.SQNPI

                    Dim espSQPNI As New Esportatore_SQNPI.Esportatore_SQNPI(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Dim rs As RispostaStandard = espSQPNI.Avvia_Esportazione()

                    Messaggio_di_Ritorno_Opzionale = rs.RispostaStringa & " - " & rs.Errore
                    Return rs.RispostaOK

                Case enum_Tipi_Servizi_Background.iMotion

                    Dim iMot As New iMotion.iMotion_import(Configurazione_Servizio, ObjParametri_Server)
                    Dim rs As RispostaStandard = iMot.GestioneImportazione()

                    Messaggio_di_Ritorno_Opzionale = rs.RispostaStringa & " - " & rs.Errore
                    Return rs.RispostaOK

                Case enum_Tipi_Servizi_Background.ImportazioneCalibri

                    Dim impoCali As New Importatore_Calibrature.Importatore_Calibrature(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return impoCali.Avvia_Esportazione()

                Case enum_Tipi_Servizi_Background.ImportazioneFascicoli

                    Dim impoFasc As New AgroFascicoloBA_BIZ.ImportatoreFascicoli(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Dim returnStr As String = ""
                    Dim returnStatus = impoFasc.avviaImportazione(returnStr)
                    Messaggio_di_Ritorno_Opzionale = returnStr
                    Return returnStatus


                Case enum_Tipi_Servizi_Background.GestoreFascicoli

                    Dim gestore As New AgroFascicoloBA_BIZ.GestoreFascicoli(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return gestore.avviaImportazione()

                Case enum_Tipi_Servizi_Background.InvioMail

                    Dim invio As New Sincronizzatore_InvioMail.InvioMailProgrammate(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return invio.inviaMailProgrammate()

                Case enum_Tipi_Servizi_Background.InvioSMS

                    Dim invio As New Sincronizzatore_InvioMail.InvioSMSProgrammati(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return invio.inviaSMSProgrammati()

                Case enum_Tipi_Servizi_Background.AccodamentoEmailInterferenzeXConferma

                    Dim accoda As New AccodamentoNotifiche.AccodamentoInterferenze(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)

                    Dim rs As RispostaStandard = accoda.Accoda()

                    Messaggio_di_Ritorno_Opzionale = rs.RispostaStringa & " - " & rs.Errore
                    Return rs.RispostaOK

                Case enum_Tipi_Servizi_Background.Interscambio_Anagrafica_Importazione

                    Dim interGestore As New Interscambio_Anagrafica.Anagrafica_Importazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    'Aggiungo il riferimento al progetto Interscambio_Util per il parametro opzionale "DetFile" richiesto dalla funzione
                    Return interGestore.Avvia_Importazione(Messaggio_di_Ritorno_Opzionale, "")

                Case enum_Tipi_Servizi_Background.Interscambio_Prodotti_Importazione

                    Dim interGestore As New Interscambio_Prodotti.Prodotti_Importazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return interGestore.Avvia_Importazione(Messaggio_di_Ritorno_Opzionale, "")

                Case enum_Tipi_Servizi_Background.Interscambio_Progetti_Esportazione

                    Dim interGestore As New Interscambio_Entita.Entita_Esportazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return interGestore.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale, "")

                Case enum_Tipi_Servizi_Background.Interscambio_OP_Esportazione

                    Dim interGestore As New Interscambio_Entita.Entita_Esportazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return interGestore.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale, "")

                Case enum_Tipi_Servizi_Background.Interscambio_Utilizzo_Ore_Esportazione

                    Dim interGestore As New Interscambio_Ore.Ore_Esportazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return interGestore.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale, "")

                Case enum_Tipi_Servizi_Background.Interscambio_Consumo_Materiali_Esportazione

                    Dim interGestore As New INDICODE_EDI_BMI_ImportExport.ConsumoMateriali_Esportazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return interGestore.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale, "")

                Case enum_Tipi_Servizi_Background.Interscambio_OP_Chiuso_Esportazione

                    Dim interGestore As New Interscambio_Entita.Entita_Esportazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return interGestore.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale, "")

                Case enum_Tipi_Servizi_Background.Interscambio_SuperTask_Importazione

                    Dim interGestore As New Interscambio_SuperTask.SuperTask_Importazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return interGestore.Avvia_Importazione(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Interscambio_SuperTask_Esportazione

                    Dim interGestore As New Interscambio_SuperTask.SuperTask_Esportazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return interGestore.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Interscambio_Anagrafica_Esportazione

                    Dim interGestore As New Interscambio_Anagrafica.Anagrafica_Esportazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return interGestore.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale, "")

                Case enum_Tipi_Servizi_Background.Interscambio_Prodotti_Esportazione

                    Dim interGestore As New Interscambio_Prodotti.Prodotti_Esportazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return interGestore.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale, "")

                Case enum_Tipi_Servizi_Background.Interscambio_Invoice_Esportazione

                    Dim interGestore As New INDICODE_EDI_BMI_ImportExport.Invoice_Esportazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return interGestore.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale, "")

                Case enum_Tipi_Servizi_Background.CreazioneAutomaticaMateriePrimeVeg

                    Dim objImportaGias As New AgronicaCoreAnagrafeBIZ.Importa_GIAS
                    Return objImportaGias.GSB_Chiama_CreaMateriePrimeVegetali(Configurazione_Servizio, ObjParametri_Server, ObjParametri_Utenti, Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.DSS_ModelliPrevisionali_Elaborazione,
                     enum_Tipi_Servizi_Background.Elaborazione_Indicatori_DSS

                    Dim objCalcolatore As New AgronicaCoreModelliPrevisionaliBIZ.ModelliPrevisionaliCalcolatore_GSB(Configurazione_Servizio)
                    Dim result = objCalcolatore.Avvia_Importazione_Sincronizzazione_Modelli()
                    Messaggio_di_Ritorno_Opzionale = objCalcolatore.MessaggioRitorno
                    Return result

                Case enum_Tipi_Servizi_Background.ImportatoreFascicoli_AGEA_UMBRIA

                    Dim importUmbria As New Importatore_UMBRIA.Importatore_AGEA_UMBRIA(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    importUmbria.avviaImportazione()

                Case enum_Tipi_Servizi_Background.EFattura_Generazione_XML

                    Dim objEFattura As New AgronicaCoreEFatturaBIZ.FatturaElettronicaService(ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer, Configurazione_Servizio)
                    objEFattura.Esegui()
                    Return True

                Case enum_Tipi_Servizi_Background.EFattura_Invio_XML_Attivi

                    Dim objEFattura As New AgronicaCoreEFatturaBIZ.FatturaElettronicaService(ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer, Configurazione_Servizio)
                    objEFattura.Esegui()
                    Return True

                Case enum_Tipi_Servizi_Background.EFattura_Verifica_Esiti

                    Dim objEFattura As New AgronicaCoreEFatturaBIZ.FatturaElettronicaService(ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer, Configurazione_Servizio)
                    objEFattura.Esegui()
                    Return True

                Case enum_Tipi_Servizi_Background.EFattura_Ricevi_XML_Passivi

                    Dim objEFattura As New AgronicaCoreEFatturaBIZ.FatturaElettronicaService(ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer, Configurazione_Servizio)
                    objEFattura.Esegui()
                    Return True

                Case enum_Tipi_Servizi_Background.EAccise_Spedizione_DAA_Creati
                    Dim objAccise As New AcciseService(ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer, Configurazione_Servizio)

                    If objAccise.ControllaServizioAttivo() Then
                        objAccise.InviaMessaggiCreati()
                        Thread.Sleep(New TimeSpan(0, 0, 0, 5))
                        objAccise.LeggiMessaggiRisposte()
                        Thread.Sleep(New TimeSpan(0, 0, 0, 5))
                        objAccise.LeggiEsiti()
                        Return True
                    End If

                Case enum_Tipi_Servizi_Background.Sentinel2Sincro

                    Dim objSentinel2Sincro As New Sentinel2Sincro.Sentinel2Sincro(Configurazione_Servizio)
                    objSentinel2Sincro.Avvia_Importazione_Sincronizzazione_Sentinel2(Messaggio_di_Ritorno_Opzionale)
                    Return True

                Case enum_Tipi_Servizi_Background.Sentinel2Elaborazioni

                    Dim objSentinel2Sincro As New Sentinel2Sincro.Sentinel2Elaborazioni(Configurazione_Servizio)
                    objSentinel2Sincro.Avvia_Elaborazioni_Sentinel2(Messaggio_di_Ritorno_Opzionale)
                    Return True

                Case enum_Tipi_Servizi_Background.Aggiornamento_CDG_DW

                    Dim objAggCDG As New DW_CDG_Costi_Ricavi_BIZ(ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer, Configurazione_Servizio)
                    ObjParametri_Server.objTransazione = Nothing
                    Dim allOk = objAggCDG.Esegui_Aggiorna_DW_Costi(Messaggio_di_Ritorno_Opzionale)
                    Return allOk

                Case enum_Tipi_Servizi_Background.CDG_Motorino_Visite

                    Dim objAggCDG As New DW_CDG_Costi_Ricavi_BIZ(ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer, Configurazione_Servizio)
                    Dim allOk = objAggCDG.Esegui_Aggiorna_Motorino_Visite(Messaggio_di_Ritorno_Opzionale)
                    Return allOk


                Case enum_Tipi_Servizi_Background.Creazione_Dati_BI_Aboca

                    Dim objAggCDG As New DW_CDG_Costi_Ricavi_BIZ(ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer, Configurazione_Servizio)
                    Dim allOk = objAggCDG.Esegui_Aggiorna_CdG_BI_Esterna(Messaggio_di_Ritorno_Opzionale)
                    Return allOk

                Case enum_Tipi_Servizi_Background.Importazione_RMA_UE
                    Dim importatoreRMA_UE As New ImportazioneRMA_UE.ImportaRMA_UE_xml(Configurazione_Servizio)
                    Return importatoreRMA_UE.Avvia_Importazione_RMA_UE(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Importazione_RMA_Homologa
                    Dim importatoreRmaHomologa As New ImportazioneRMA_UE.ImportaRmaHomologa(Configurazione_Servizio)
                    Return importatoreRmaHomologa.Avvia_Importazione_RMA_Hologa(Messaggio_di_Ritorno_Opzionale)


                Case enum_Tipi_Servizi_Background.Importazione_RMA_Zespri
                    Dim importatoreRmaZespri As New ImportazioneRMA_UE.ImportaRMA_Zespri(Configurazione_Servizio)
                    Return importatoreRmaZespri.Avvia_Importazione_RMA_Zespri(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Importazione_COOPSole

                    Dim importatore As New CooperativaSOLE_Importatore.ImportatoreCoopSOLE(Configurazione_Servizio, ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)
                    Return importatore.Avvia(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Servizio_ImportExport_Riunite

                    Dim importExport As New Riunite_ImportExport.RiuniteServiceManager(Configurazione_Servizio, ObjParametri_Server, ObjParametri_Utenti)
                    importExport.Avvia()
                    Return True

                Case enum_Tipi_Servizi_Background.DSS_PreElaborazioneImpianti

                    Dim servizio As New AgronicaCoreModelliPrevisionaliBIZ.GSB_PreElaborazioneModelli(Configurazione_Servizio, ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer)
                    Dim result = servizio.Avvia()
                    Messaggio_di_Ritorno_Opzionale = servizio.Messaggio()
                    Return result

                Case enum_Tipi_Servizi_Background.myticoAllineamentoTimbrature
                    Dim allinea As New Mytico.AllineamentoDatiTimbrature(Configurazione_Servizio, ObjParametri_Server, ObjParametri_Utenti)
                    Dim result As RispostaStandard
                    result = allinea.InvioTimbrature()
                    Messaggio_di_Ritorno_Opzionale = result.RispostaStringa & vbCrLf & result.Errore
                    Return result.RispostaOK

                Case enum_Tipi_Servizi_Background.SincronizzazioneDatiGisDaServerAgronica
                    Dim sincroWeb As New GIAS2GIAS_LOCALE.GIAS_2_GIAS(ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Dim rval As RispostaStandard =
                        sincroWeb.GIAS_2_GIAS_PULL_GIS_SRVAgronica(Configurazione_Servizio)
                    Messaggio_di_Ritorno_Opzionale = rval.RispostaStringa

                    Return rval.RispostaOK

                Case enum_Tipi_Servizi_Background.ImportazioneAnagraficheJDE

                    Dim SincroJDE As New Importazione_Articoli_Fruttagel_JDE.SincronizzazioneAutomaticaAnagrafiche(Configurazione_Servizio, ObjParametri_Server, ObjParametri_Utenti)
                    SincroJDE.Avvia_Sincro_JDE(Messaggio_di_Ritorno_Opzionale)
                    Return True

                Case enum_Tipi_Servizi_Background.JDeere_Download_ShapeFile_produzione

                    Return True


                Case enum_Tipi_Servizi_Background.SincronizzazionePerformaTFS

                    Dim sincroPerforma As New PerformaSincronizzatori.SincroPerformaTFS(Configurazione_Servizio)

                    Dim rval As RispostaStandard =
                        sincroPerforma.SincronizzaPerformaTFS()

                    Dim stbMsg As New StringBuilder

                    stbMsg.Append(rval.RispostaStringa)
                    stbMsg.Append(rval.Errore)
                    Messaggio_di_Ritorno_Opzionale = stbMsg.ToString()

                    Return rval.RispostaOK

                Case enum_Tipi_Servizi_Background.ImportAnagraficheCOPROB

                    Dim imp As New ImportAnagrafiche_COPROB.ImportAnagraficheCOPROB_Service(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Dim rval As RispostaStandard =
                        imp.Avvia_Import_Anagrafiche_COPROB(Messaggio_di_Ritorno_Opzionale)

                    If Not rval.RispostaOK Then
                        rval.RispostaStringa = Messaggio_di_Ritorno_Opzionale
                    End If

                    Return rval.RispostaOK

                Case enum_Tipi_Servizi_Background.ImportConferimentiPomodoro

                    Dim imp As New ImportConferimentiPomodoro_Service(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Dim rval As RispostaStandard = imp.Avvia_Import_Conferimenti_Pomodoro()

                    If rval.RispostaOK Then
                        Messaggio_di_Ritorno_Opzionale = rval.RispostaStringa
                    Else
                        Messaggio_di_Ritorno_Opzionale = rval.Errore
                    End If

                    Return rval.RispostaOK


                Case enum_Tipi_Servizi_Background.ImportConferimentiAgribologna

                    Dim imp As New Import_Conferimenti.ImportConferimentiAgribologna(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Dim rval As RispostaStandard = imp.Avvia_Import_Conferimenti_Agribologna()

                    If rval.RispostaOK Then
                        Messaggio_di_Ritorno_Opzionale = rval.RispostaStringa
                    Else
                        Messaggio_di_Ritorno_Opzionale = rval.Errore
                    End If

                    Return rval.RispostaOK



                Case enum_Tipi_Servizi_Background.ImportQDC_Documentale

                    Dim imp As New Import_Conferimenti.ImportConferimentiAgribologna(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Dim rval As RispostaStandard = imp.Avvia_Import_QDC_Agribologna("", True)

                    If rval.RispostaOK Then
                        Messaggio_di_Ritorno_Opzionale = rval.RispostaStringa
                    Else
                        Messaggio_di_Ritorno_Opzionale = rval.Errore
                    End If

                    Return rval.RispostaOK


                Case enum_Tipi_Servizi_Background.Interscambio_ExportMovimenti

                    Dim exp As New INDICODE_EDI_BMI_ImportExport.Movimenti_Esportazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return exp.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale)


                Case enum_Tipi_Servizi_Background.Integrazione_Macchine_Lavorazione

                    If Not Log4NetConfigurazioneFactory.Instance.ConfigurazioneInizializzata Then

                        Dim strPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly.Location)
                        Dim log4netConfigFullPath = Path.Combine(strPath, "Log4net.config")

                        If File.Exists(log4netConfigFullPath) Then

                            'Imposta proprietà previste dal file di configurazione
                            Log4NetConfigurazioneFactory.Instance.ImpostaProprieta(Configurazione_Servizio.Parametri_Extra)

                            'Apertura file configurazione e inizializzazione log4net
                            Using fs As New FileStream(log4netConfigFullPath, FileMode.Open)
                                log4net.Config.XmlConfigurator.Configure(fs)
                                fs.Close()
                            End Using

                        End If

                        Log4NetConfigurazioneFactory.Instance.ConfigurazioneInizializzata = True

                    End If

                    Dim objParametri As New ObjParametri With {.SuperServer = ObjParametri_SuperServer, .Server = ObjParametri_Server, .Utenti = ObjParametri_Utenti}
                    Dim sai As New ServizioAttivatoreImportazioni(Configurazione_Servizio, objParametri)
                    Dim statoInizializzato = sai.Inizializza()
                    If statoInizializzato = String.Empty Then
                        Return sai.AvviaTutto()
                    Else
                        Return False
                    End If

                Case enum_Tipi_Servizi_Background.ImportazioneMassivaPlanningAGEA

                    Dim importatorePlanningMassivo =
                        New ImportatoreMassivoPlanning.ImportatoreMassivoPlanning(Configurazione_Servizio,
                                                                                  ObjParametri_SuperServer,
                                                                                  ObjParametri_Server,
                                                                                  ObjParametri_Utenti)

                    importatorePlanningMassivo.AvviaImportazione()

                Case enum_Tipi_Servizi_Background.Export_SDS

                    Dim interGestore As New SchedeSicurezza.SDS_Esportazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return interGestore.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale, "", True)

                Case enum_Tipi_Servizi_Background.ImportazioneTaskData_SDF

                    Dim service As New ImportatoreTaskDataSDF.ImportTaskData_WindowsService(Configurazione_Servizio,
                                                                                            ObjParametri_SuperServer,
                                                                                              ObjParametri_Server,
                                                                                              ObjParametri_Utenti)
                    Dim rval As RispostaStandard = service.RecuperaInformazioniTaskDataSDF(Messaggio_di_Ritorno_Opzionale)

                    If Not rval.RispostaOK Then
                        rval.RispostaStringa = Messaggio_di_Ritorno_Opzionale
                    End If

                    Return rval.RispostaOK

                Case enum_Tipi_Servizi_Background.Interscambio_FatturePdf_Importazione

                    Dim interGestore As New Interscambio_Fatture_Documentale.Fatture_Importazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return interGestore.Avvia_Importazione(Messaggio_di_Ritorno_Opzionale, "")

                Case enum_Tipi_Servizi_Background.Interscambio_CdcWbs_Importazione

                    Dim interGestore As New Interscambio_AnagraficaCosti.CDC_Importazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return interGestore.Avvia_Importazione(Messaggio_di_Ritorno_Opzionale, "")

                Case enum_Tipi_Servizi_Background.Interscambio_VerificaStatoPagamentoFatture

                    Dim interGestore As New Interscambio_Fatture_Pagamenti.Richiesta_Stato_Pagamento(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return interGestore.Esegui_Verifica(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.SincronizzazioneDatiApp

                    Dim Interscambio As New SincroAttivita.AttivitaSincronizzate(Configurazione_Servizio)
                    Return Interscambio.SincronizzaDatiApp(ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti, Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.ImportazioneDatiStazioniMeteo_NT_Acmotec


                    Dim servizio As New ImportatoreMeteoNT.ImportatoreMeteo_Acmotec(Configurazione_Servizio)
                    Dim result As Boolean = servizio.Esegui()
                    Messaggio_di_Ritorno_Opzionale = servizio.Messaggio
                    Return result

                Case enum_Tipi_Servizi_Background.ImportazioneDati_rete_acqua_Acmotec
                    Dim servizio As New Importatore_IoT_Data.ImportatoreIoT_Acmotec(ObjParametri_Server, Configurazione_Servizio)
                    Dim result As Boolean = servizio.Esegui()
                    Messaggio_di_Ritorno_Opzionale = servizio.Messaggio
                    Return result

                Case enum_Tipi_Servizi_Background.AggiornaChecklistCOPROB

                    Dim servizio As New UpdateChecklist_COPROB.UpdateChecklistCOPROB_Service(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return servizio.Avvia_Update_Checklist_COPROB(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.SincronizzatoreBDN
                    Dim servizio As New ImportazioneBDN.ServizioSincroStalla(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return servizio.Avvia()

                Case enum_Tipi_Servizi_Background.EsportazioneWMSOnPlant

                    Dim orogel = New Orogel()
                    orogel.avviaEsportazioneEsercizi(Configurazione_Servizio, ObjParametri_Server)
                    Return True

                Case enum_Tipi_Servizi_Background.EsportazioneENI_SAP

                    Dim sapEni As New Agende_Esportazione_ENI_SAP(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return sapEni.AvviaEsportazione(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.ProgrammazioneNotifichePush

                    Dim programmazioneNotifiche As New ProgrammazioneNotifiche(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)

                    Return programmazioneNotifiche.Accoda(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.InvioNotifichePush

                    Dim invioNotifichePush As New InvioNotifiche(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)

                    Return invioNotifichePush.inviaNotifiche(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.HubIot_InvioRicette

                    Dim expManager As New HubIoT_ExporterServiceManager(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return expManager.EseguiEsportazione()

                Case enum_Tipi_Servizi_Background.HubIot_RecuperaDatiProduzione
                    Return True

                Case enum_Tipi_Servizi_Background.ImportazioneCAI_Giacenze
                    Dim caiGiacenze = New ImportatoreCAI_Giacenze(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return caiGiacenze.ConvertiFileCAIGiacenzeInXML(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.ImportazioneCAI_Agenzie

                    Dim caiAgenzie = New Import_CAI_AgenzieAziende(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return caiAgenzie.importaAgenzie()

                Case enum_Tipi_Servizi_Background.ImportazioneCAI_Aziende
                    Dim caiAziende As New Import_CAI_AgenzieAziende(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return caiAziende.importaAziende()

                Case enum_Tipi_Servizi_Background.ImportazioneCAI_Acquisti_Imprese
                    Dim caiGiacenze = New ImportatoreCAI_Acquisti_Imprese(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return caiGiacenze.ConvertiFileCAIAcquistiInXML(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.EsportazioneCAI_Aziende
                    Dim caiExportAziendeQdC As New Export_CAI_Aziende(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return caiExportAziendeQdC.AvviaEsportazione(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.CalcoloGHG
                    Dim sincroGHG = New SincroGHG.SincroGHG(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return sincroGHG.AvviaCalcoloGHG()

                Case enum_Tipi_Servizi_Background.SincronizzazioneEsecuzioniConfigurazioneProiezione

                    Dim sincronizzatoreEsecuzioni As New SincronizzatoreEsecuzioni(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)

                    Return sincronizzatoreEsecuzioni.Sincronizza(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.SincronizzazioneEntitaInattive

                    Dim sincronizzatoreEntitaInattive As New SincronizzatoreEntitaInattive(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)

                    Return sincronizzatoreEntitaInattive.Sincronizza(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.EsecuzioneAlgoritmiProiezione

                    Dim esecutoreAlgoritmoProiezione As New EsecutoreAlgoritmoProiezione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)

                    ' With SAT we don't have the possibility for bulk inserts
                    ' Return esecutoreAlgoritmoProiezione.EseguiAlgoritmoBulk(Messaggio_di_Ritorno_Opzionale, False)
                    Return esecutoreAlgoritmoProiezione.EseguiAlgoritmo(Messaggio_di_Ritorno_Opzionale, False)

                Case enum_Tipi_Servizi_Background.EsecuzioneAlgoritmiSenzaConfigurazione

                    Dim esecutoreAlgoritmoProiezione As New EsecutoreAlgoritmoProiezione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)

                    Return esecutoreAlgoritmoProiezione.EseguiAlgoritmo(Messaggio_di_Ritorno_Opzionale, True)

                Case enum_Tipi_Servizi_Background.SincroBDN_Mail

                    Dim servizio As New ImportazioneBDN.ServizioSincroStalla_Mail(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    servizio.Avvia()
                    Return True

                Case enum_Tipi_Servizi_Background.AccodamentoServizioInScandenza

                    Dim AccodamentoServizioInScandenza As New InvioScadenzaProdotti(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return AccodamentoServizioInScandenza.InviaScadenzaProdotti(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Cai_Invio_Email

                    Dim cai_invio_email = New Cai_Invio_Email(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    cai_invio_email.Avvia()

                Case enum_Tipi_Servizi_Background.Demetra_Abaco_Import_Creazione
                    Dim importer As New Import(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    importer.Avvia(1, Messaggio_di_Ritorno_Opzionale)
                    importer.Dispose()
                    importer = Nothing
                    Return True

                Case enum_Tipi_Servizi_Background.Demetra_Abaco_Import_Aggiornamento

                    Dim importer As New Import(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    importer.Avvia(2, Messaggio_di_Ritorno_Opzionale)
                    importer.Dispose()
                    importer = Nothing
                    Return True

                Case enum_Tipi_Servizi_Background.Sincro_Timesheet_Calendar

                    Dim sincroTimesheetCalendar As New Sincro_Timesheet_Calendar(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)

                    Return sincroTimesheetCalendar.Sincronizza(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Sincronizza_Utenti_Visibilita
                    Dim objUtenti As New AgronicaCoreUtentiBIZ.SincronizzatoreUtentiVisibilitaService(ObjParametri_Server, ObjParametri_Utenti, ObjParametri_SuperServer, Configurazione_Servizio)

                    Dim retVal As Boolean = objUtenti.SincronizzaTuttiGliUtenti_Parallel()
                    If Not retVal Then
                        objUtenti.SincronizzaTuttiGliUtenti()
                    End If

                    Return True

                Case enum_Tipi_Servizi_Background.GestioneTraduzione
                    Dim GestioneTraduzione As New GestioneTraduzione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return GestioneTraduzione.GestioneTraduzione(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Esportazione_Visite_Analisi_Commerciali

                    Dim exportVisite As New ExportVisite(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)

                    Return exportVisite.Esporta(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Interscambio_BIOrogel
                    Dim Interscambio_BIOrogel As New InterscambioDati_BIOrogel(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return Interscambio_BIOrogel.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.EsportazioneMaterialeVivaisticoOrdini
                    Dim handleEsportazioneOrdiniMV As New MaterialeVivaistico.EsportazioneOrdiniMV(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handleEsportazioneOrdiniMV.EsportaOrdini(Messaggio_di_Ritorno_Opzionale, False)

                Case enum_Tipi_Servizi_Background.EsportazioneMaterialeVivaisticoOrdiniVerificaStato
                    Dim handleEsportazioneOrdiniMV As New MaterialeVivaistico.EsportazioneOrdiniMV(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handleEsportazioneOrdiniMV.OttieniStatoOrdini(Messaggio_di_Ritorno_Opzionale, False)

                Case enum_Tipi_Servizi_Background.Importazione_Risposta_Analisi_Zootecnia
                    Dim ImportatoreAnalisi_Zootecnia As New Importazione_Analisi_Zootecnia.Importazione_AnalisiZootecnia(Configurazione_Servizio, ObjParametri_Server, ObjParametri_Utenti)
                    Return ImportatoreAnalisi_Zootecnia.Avvia_Importazione_Analisi_Zootecnia(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Esportazione_To_Demetra_Fabbricati
                    Dim handleEsportazioneFabbricatiDemetra As New Export_Fabbricati.Export_Fabbricati(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handleEsportazioneFabbricatiDemetra.EsportaFabbricati(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Esportazione_To_Demetra_AnalisiTerreno
                    Dim handleEsportazioneAnalisiTerrenoDemetra As New Export_AnalisiTerreno.Export_AnalisiTerreno(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handleEsportazioneAnalisiTerrenoDemetra.EsportaAnalisiTerreno(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.SuperTask_Esportazione_To_Demetra
                    Dim interGestore As New SuperTask_Esportazione_To_Demetra.SuperTask_Esportazione(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return interGestore.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Esportazione_To_Demetra_Attivita
                    Dim handleEsportazioneAttivitaDemetra As New Export_Attivita.Export_Attivita(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handleEsportazioneAttivitaDemetra.EsportaAttivita(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Esportazione_To_Demetra_Contatti
                    Dim handleEsportazioneContattiDemetra As New Export_Contatti.Export_Contatti(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handleEsportazioneContattiDemetra.EsportaContatti(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Esportazione_To_Demetra_Macchine
                    Dim handleEsportazioneAttivitaDemetra As New Export_Macchine.Export_Macchine(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handleEsportazioneAttivitaDemetra.EsportaMacchine(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Gias_Duplicate_Resx
                    Dim CheckDuplicateResx As New VerificazioneDoppioniResx(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return CheckDuplicateResx.Esegui_Verifica(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Esportazione_MovimentiMag_SistemiEsterni
                    Dim handleEsportazioneAttivitaDemetra As New Movimenti_Esportazione_SistemiEsterni(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handleEsportazioneAttivitaDemetra.Avvia_Esportazione(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Esportazione_LogInterscambio_ElasticSearch
                    Dim handler As New Esportazione_ElasticSearch.Esportazione_ElasticSearch(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handler.Esporta(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Servizio_IsAlive
                    Dim handler As New IsAlive.IsAliveService(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handler.Avvia()

                Case enum_Tipi_Servizi_Background.Aggiornamento_ISCC_Conferimenti

                    Dim objISCC As New Aggiornamento_ISCC.Aggiornamento_ISCC(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return objISCC.AvviaAggiornamentoISCC()

                Case enum_Tipi_Servizi_Background.ElaborazioneComplianceISCC_Async
                    Dim objISCC As New Compliance_ISCC_Async.Compliance_ISCC_Async(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return objISCC.Avvia()


                Case enum_Tipi_Servizi_Background.Import_DPI_Abaco

                    Dim impDPI As New Abaco_Import_DPI.ImportDpiAbaco(Configurazione_Servizio)
                    Return impDPI.Avvia_Import_DPI_Abaco(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.PuliziaTabelleLog '166
                    Dim handlePuliziaTabelleAgronicaLog As New PuliziaTabelleAgronicaLog.Pulizia(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handlePuliziaTabelleAgronicaLog.Esegui(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.ImportazionePendenzeAGEA '167

                    Dim importPendenze As New PianoColturale_Pendenze.ImportPendenze(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    importPendenze.ImportaPendenze()
                    importPendenze.Dispose()
                    importPendenze = Nothing
                    Return True

                Case enum_Tipi_Servizi_Background.ImportGAP_Documentale

                    Dim imp As New ImportGapZani.ImportGlobalGAPZani(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Dim rval As RispostaStandard = imp.Avvia_Import_Global_GAP_Zani("", True)

                    If rval.RispostaOK Then
                        Messaggio_di_Ritorno_Opzionale = rval.RispostaStringa
                    Else
                        Messaggio_di_Ritorno_Opzionale = rval.Errore
                    End If

                    Return rval.RispostaOK

                Case enum_Tipi_Servizi_Background.PuliziaTabelleAPP '169
                    Dim handlePuliziaTabelle As New PuliziaTabelle(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handlePuliziaTabelle.Esegui(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.ImportazioneAcquistiFarmacie '170
                    Dim handleImport = New ImportatoreInalcaAcquistiFarmaci(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handleImport.ConvertiFileFarmacieInXML(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.ExportStazioniMeteoInfragri   '171
                    Dim handleEsportazioneInfragri As New Export_Stazioni.Export_Stazioni(Configurazione_Servizio, enum_Esportazioni_Sistema_Cod.Infragri_Export_Stazioni, enum_SistemiEsterni.Infragri, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handleEsportazioneInfragri.EsportaStazioni(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.VerificaConformitaMassiva '172
                    Dim handleVerificaConformitaMassiva As New VerificaConformitaMassivaBackground.Verifica_Conformita_Massiva(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handleVerificaConformitaMassiva.EseguiVerifica(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Esportazione_To_Demetra_Attivita_V2 '173
                    Dim handleEsportazioneAttivitaDemetra As New Export_Attivita.Export_Attivita(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handleEsportazioneAttivitaDemetra.EsportaAttivita_V2(Messaggio_di_Ritorno_Opzionale)

                Case enum_Tipi_Servizi_Background.Esportazione_To_Demetra_Squadre '174
                    Dim handleEsportazioneSquadreDemetra As New Export_Squadre.Export_Squadre(Configurazione_Servizio, ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti)
                    Return handleEsportazioneSquadreDemetra.EsportaSquadre(Messaggio_di_Ritorno_Opzionale)

                Case Else

                    'oppure
                    'throw new Exception("Selettore_Classe_Servizio: Configurazione_Servizio.Tipo_Sincro non presente nella scelta")
                    Messaggio_di_Ritorno_Opzionale = "Selettore_Classe_Servizio: Configurazione_Servizio.Tipo_Sincro non presente nella scelta"
                    Return False

            End Select

        Else

            'oppure
            'throw new Exception("Selettore_Classe_Servizio: Configurazione_Servizio.Id_Servizio <> enum_Id_Servizio.GiasOnline")
            Messaggio_di_Ritorno_Opzionale = "Selettore_Classe_Servizio: Configurazione_Servizio.Id_Servizio <> enum_Id_Servizio.GiasOnline"
            Return False

        End If

        Return False

    End Function

End Class
