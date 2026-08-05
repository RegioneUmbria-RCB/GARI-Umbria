Imports System.IO
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports Newtonsoft.Json
Imports AgronicaCoreEFatturaDAL
Imports Newtonsoft.Json.Linq
Imports Gias2Gias_LIB
Imports AgronicaCoreModello
Imports AgronicaCoreUtility


Imports <xmlns="http://G2G">
Imports AgronicaCoreVarieDAL

Public Class MenuG2G
    Inherits System.Web.UI.Page

    Dim objParametriAgenda As ParametriAgenda
    Dim objParametri_Server, objParametri_Utenti, objParametri_Super_Server As AgronicaCoreParametri

    Public objparametri_server_string, objparametri_utenti_string As String


    Public PercorsoConnessioni As String = "c:\agroconnnesioni\connessioni.ini"

    Private Sub inizializzoObjParametri()
        objParametriAgenda = New ParametriAgenda

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))

        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)
        CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap).Lbl_Titolo.Text = "MENU G2G"

        inizializzoObjParametri()


        If objParametriAgenda.Piva <> "" Then
            hdPiva.Value = objParametriAgenda.Piva
        End If

        If Not IsPostBack Then

            popolaDDConfigurazioni()
            popolaDDConnessioneOrigine()
            popolaDDConnessioneDestinazione()


        End If

    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoConfigurazioniG2G() As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objG2G As New AgronicaCoreG2GLocalDal.G2GLocal_R
            Dim dtConfig As DataTable = objG2G.LeggiConfigurazioni(0, objParametri_Server)

            Dim jArrayConfig As New JArray()

            jArrayConfig.Add(New JObject(New JProperty("codice", -1), New JProperty("descrizione", "Seleziona ...")))
            'ddlConfigurazioni.Items.Add(New ListItem With {.Value = "-1", .Text = "Seleziona ..."})

            For Each dr As DataRow In dtConfig.Rows
                Dim codice As String = CStr(dr.Item("G2GLocalConfigurazioni_COD"))
                Dim descrizione As String = dr.Item("G2GLocalConfigurazioni_DES")
                jArrayConfig.Add(New JObject(New JProperty("codice", codice), New JProperty("descrizione", descrizione)))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayConfig, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Sub LeggiConfigurazioneG2G(ByVal ID_Cfg As Integer)

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        'If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
        '    r.Sessione = False
        '    Return r
        'End If

        Try

            Dim objG2G As New AgronicaCoreG2GLocalDal.G2GLocal_R
            Dim dtConfig As DataTable = objG2G.LeggiConfigurazioni(ID_Cfg, objParametri_Server)


            If dtConfig.Rows.Count > 0 Then
                'r.RispostaStringa = dtConfig.Rows(0).Item("G2GLocalConfigurazioni_CFG")

                RiempiControlli(dtConfig)

            End If

            LblErr.Visible = False

        Catch ex As Exception

            LblErr.Text = "Si è verificato un errore durante la lettura della configurazione"
            LblErr.Visible = True

            popolaDDConnessioneOrigine()
            popolaDDConnessioneDestinazione()

        End Try


    End Sub

    Public Sub RiempiControlli(dtConfig As DataTable)

        '  Marco Grilli, 12/06/2014 17:44:12: pulisco tutti i controlli
        svuotaControlliOrigine(True)
        svuotaControlliDestinazione(True)
        svuotacontrollitrasferimento()


        Dim albero As XDocument = XDocument.Parse(dtConfig.Rows(0)("G2GLocalConfigurazioni_CFG"))

        'Nome Config
        txtCfg.Text = dtConfig.Rows(0)("G2GLocalConfigurazioni_DES")

        'XML
        txtConfigurazione.Text = dtConfig.Rows(0)("G2GLocalConfigurazioni_CFG")

        Dim objOpzioni As Gias2Gias_LIB.clsOpzioni =
                       (From o In albero.<dati>.<configurazione>
                        Select New Gias2Gias_LIB.clsOpzioni With {
                           .wsimportaGiasURl = o.<urlWsimportaGias>.Value,
                           .Connessione_Server_GIAS_Origine = o.<connessioni>.<connessione>.<Connessione_SERVER_ORIGINE>.Value,
                           .Connessione_Server_GIAS_Destinazione = o.<connessioni>.<connessione>.<Connessione_SERVER_DESTINAZIONE>.Value,
                           .Connessione_Utenti_GIAS_Origine = o.<connessioni>.<connessione>.<Connessione_UTENTI_ORIGINE>.Value,
                           .Connessione_Utenti_GIAS_Destinazione = o.<connessioni>.<connessione>.<Connessione_UTENTI_DESTINAZIONE>.Value,
                           .ProgressivoGIAS_ORIGINE = o.<parametri>.<ProgressivoGIAS_ORIGINE>.Value,
                           .ProgressivoGIAS_DESTINAZIONE = o.<parametri>.<ProgressivoGIAS_DESTINAZIONE>.Value,
                           .PercorsoConnessioni = o.<parametri>.<percorsoConnessioni>.Value,
                           .SuperUser_CodFiscale_ORIGINE = o.<parametri>.<PivaSuperUser_ORIGINE>.Value,
                           .SuperUser_CodFiscale_DESTINAZIONE = o.<parametri>.<PivaSuperUser_DESTINAZIONE>.Value,
                           .SuperUser_Username_ORIGINE = o.<parametri>.<UsernameSuperUser_Origine>.Value,
                           .SuperUser_Username_DESTINAZIONE = o.<parametri>.<UsernameSuperUser_Destinazione>.Value,
                           .Import_CodFiscale_ORIGINE = o.<parametri>.<Import_CodFiscale_ORIGINE>.Value,
                           .Import_CodFiscale_DESTINAZIONE = o.<parametri>.<Import_CodFiscale_DESTINAZIONE>.Value,
                           .Import_Username_ORIGINE = o.<parametri>.<Import_Username_ORIGINE>.Value,
                           .Import_Username_DESTINAZIONE = o.<parametri>.<Import_Username_DESTINAZIONE>.Value,
                           .ImpostazioniTrasformazioni = o.<parametri>.<ImpostazioniTrasformazioni>.Value
                       }).FirstOrDefault

        txtConfigurazione.Text = dtConfig.Rows(0)("G2GLocalConfigurazioni_CFG")
        txtCfg.Text = dtConfig.Rows(0)("G2GLocalConfigurazioni_DES")
        txtProgressivoGIAS_ORIGINE.Text = objOpzioni.ProgressivoGIAS_ORIGINE
        txtProgressivoGIAS_DESTINAZIONE.Text = objOpzioni.ProgressivoGIAS_DESTINAZIONE
        txtPivaSuperUser_ORIGINE.Text = objOpzioni.SuperUser_CodFiscale_ORIGINE
        txtPivaSuperUser_DESTINAZIONE.Text = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        txtUsernameSuperUser_Origine.Text = objOpzioni.SuperUser_Username_ORIGINE
        txtUsernameSuperUser_Destinazione.Text = objOpzioni.SuperUser_Username_DESTINAZIONE
        txtImport_CodFiscale_ORIGINE.Text = objOpzioni.Import_CodFiscale_ORIGINE
        txtImport_CodFiscale_DESTINAZIONE.Text = objOpzioni.Import_CodFiscale_DESTINAZIONE
        txtImport_Username_ORIGINE.Text = objOpzioni.Import_Username_ORIGINE
        txtImport_Username_DESTINAZIONE.Text = objOpzioni.Import_Username_DESTINAZIONE
        txtImport_ImpostazioniTrasfromazioni.Text = objOpzioni.ImpostazioniTrasformazioni
        Txb_WebService.Text = objOpzioni.wsimportaGiasURl


        popolaDDConnessioneOrigine()
        popolaDDConnessioneDestinazione()


        '  Marco Grilli, 12/06/2014 17:25:12: per selezionare l'elemento giusto, dovevo prima popolare le ddb
        '                                     ma soprattutto sapere se è local2local (e quindi assegnare l'url)
        ddConnessione_SERVER_ORIGINE.Text = objOpzioni.Connessione_Server_GIAS_Origine
        ddConnessione_SERVER_DESTINAZIONE.Text = objOpzioni.Connessione_Server_GIAS_Destinazione
        ddConnessione_UTENTI_ORIGINE.Text = objOpzioni.Connessione_Utenti_GIAS_Origine
        ddConnessione_UTENTI_DESTINAZIONE.Text = objOpzioni.Connessione_Utenti_GIAS_Destinazione



        '#########################################################################################################################
        '################################## TRASFERIMENTO ########################################################################
        '#########################################################################################################################
        Dim objImprese As clsImportData =
            GIAS2GIAS_LOCALE.GIAS_2_GIAS.ListaDatiDaImportareLetturaDaXDoc(albero)



        'Riempimento Controlli IMPRESA

        'Audit
        If objImprese.DatiImpresa.FlagImporta_Audit Then
            FlagImporta_Audit.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_Audit) <> AGRODATAINIZIO Then
                ValiditaInizio_Audit.Text = objImprese.DatiImpresa.ValiditaInizio_Audit
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_Audit) <> AGRODATAFINE Then
                ValiditaFine_Audit.Text = objImprese.DatiImpresa.ValiditaFine_Audit
            End If
            Configurazione_Audit.Text = objImprese.DatiImpresa.configurazione_Audit
        End If

        'Audit_Interviste
        If objImprese.DatiImpresa.FlagImporta_Audit_Interviste Then
            FlagImporta_Audit_Interviste.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_Audit_Interviste) <> AGRODATAINIZIO Then
                ValiditaInizio_Audit_Interviste.Text = objImprese.DatiImpresa.ValiditaInizio_Audit_Interviste
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_Audit_Interviste) <> AGRODATAFINE Then
                ValiditaFine_Audit_Interviste.Text = objImprese.DatiImpresa.ValiditaFine_Audit_Interviste
            End If
            Configurazione_Audit_Interviste.Text = objImprese.DatiImpresa.configurazione_Audit_Interviste
        End If

        'agenda
        If objImprese.DatiImpresa.flagimporta_agenda Then
            flagimporta_agenda.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_agenda) <> AGRODATAINIZIO Then
                ValiditaInizio_agenda.Text = objImprese.DatiImpresa.ValiditaInizio_agenda
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_agenda) <> AGRODATAFINE Then
                ValiditaFine_agenda.Text = objImprese.DatiImpresa.ValiditaFine_agenda
            End If
            Configurazione_agenda.Text = objImprese.DatiImpresa.configurazione_agenda
        End If

        'pap
        If objImprese.DatiImpresa.flagimporta_pap Then
            flagimporta_pap.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_pap) <> AGRODATAINIZIO Then
                ValiditaInizio_pap.Text = objImprese.DatiImpresa.ValiditaInizio_pap
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_pap) <> AGRODATAFINE Then
                ValiditaFine_pap.Text = objImprese.DatiImpresa.ValiditaFine_pap
            End If
            Configurazione_pap.Text = objImprese.DatiImpresa.configurazione_pap
        End If

        'papz
        If objImprese.DatiImpresa.flagimporta_papz Then
            flagimporta_papz.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_papz) <> AGRODATAINIZIO Then
                ValiditaInizio_papz.Text = objImprese.DatiImpresa.ValiditaInizio_papz
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_papz) <> AGRODATAFINE Then
                ValiditaFine_papz.Text = objImprese.DatiImpresa.ValiditaFine_papz
            End If
            Configurazione_papz.Text = objImprese.DatiImpresa.configurazione_papz
        End If

        'notificabio
        If objImprese.DatiImpresa.flagimporta_notificabio Then
            flagimporta_notificabio.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_notificabio) <> AGRODATAINIZIO Then
                ValiditaInizio_notificabio.Text = objImprese.DatiImpresa.ValiditaInizio_notificabio
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_notificabio) <> AGRODATAFINE Then
                ValiditaFine_notificabio.Text = objImprese.DatiImpresa.ValiditaFine_notificabio
            End If
            Configurazione_notificabio.Text = objImprese.DatiImpresa.configurazione_notificabio
        End If

        'planning
        If objImprese.DatiImpresa.flagimporta_planning Then
            flagimporta_planning.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_planning) <> AGRODATAINIZIO Then
                ValiditaInizio_planning.Text = objImprese.DatiImpresa.ValiditaInizio_planning
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_planning) <> AGRODATAFINE Then
                ValiditaFine_planning.Text = objImprese.DatiImpresa.ValiditaFine_planning
            End If
            Configurazione_planning.Text = objImprese.DatiImpresa.configurazione_planning
        End If

        'distinta
        If objImprese.DatiImpresa.flagimporta_distinta Then
            flagimporta_distinta.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_distinta) <> AGRODATAINIZIO Then
                ValiditaInizio_distinta.Text = objImprese.DatiImpresa.ValiditaInizio_distinta
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_distinta) <> AGRODATAFINE Then
                ValiditaFine_distinta.Text = objImprese.DatiImpresa.ValiditaFine_distinta
            End If
            Configurazione_distinta.Text = objImprese.DatiImpresa.configurazione_distinta
        End If

        'ricette
        If objImprese.DatiImpresa.flagimporta_ricette Then
            flagimporta_ricette.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_ricette) <> AGRODATAINIZIO Then
                ValiditaInizio_ricette.Text = objImprese.DatiImpresa.ValiditaInizio_ricette
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_ricette) <> AGRODATAFINE Then
                ValiditaFine_ricette.Text = objImprese.DatiImpresa.ValiditaFine_ricette
            End If
            Configurazione_ricette.Text = objImprese.DatiImpresa.configurazione_ricette
        End If

        'materieprime
        If objImprese.DatiImpresa.Flagimporta_materieprime Then
            Flagimporta_materieprime.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_materieprime) <> AGRODATAINIZIO Then
                ValiditaInizio_materieprime.Text = objImprese.DatiImpresa.ValiditaInizio_materieprime
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_materieprime) <> AGRODATAFINE Then
                ValiditaFine_materieprime.Text = objImprese.DatiImpresa.ValiditaFine_materieprime
            End If
            Configurazione_materieprime.Text = objImprese.DatiImpresa.configurazione_materieprime
        End If

        'pianocolturale
        If objImprese.DatiImpresa.Flagimporta_pianocolturale Then
            Flagimporta_pianocolturale.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_pianocolturale) <> AGRODATAINIZIO Then
                ValiditaInizio_pianocolturale.Text = objImprese.DatiImpresa.ValiditaInizio_pianocolturale
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_pianocolturale) <> AGRODATAFINE Then
                ValiditaFine_pianocolturale.Text = objImprese.DatiImpresa.ValiditaFine_pianocolturale
            End If
            Configurazione_pianocolturale.Text = objImprese.DatiImpresa.configurazione_pianocolturale
        End If

        'gis
        If objImprese.DatiImpresa.Flagimporta_gis Then
            Flagimporta_gis.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_gis) <> AGRODATAINIZIO Then
                ValiditaInizio_gis.Text = objImprese.DatiImpresa.ValiditaInizio_gis
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_gis) <> AGRODATAFINE Then
                ValiditaFine_gis.Text = objImprese.DatiImpresa.ValiditaFine_gis
            End If
            Configurazione_gis.Text = objImprese.DatiImpresa.configurazione_gis
        End If

        'profilazione
        If objImprese.DatiImpresa.Flagimporta_profilazione Then
            Flagimporta_profilazione.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_profilazione) <> AGRODATAINIZIO Then
                ValiditaInizio_profilazione.Text = objImprese.DatiImpresa.ValiditaInizio_profilazione
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_profilazione) <> AGRODATAFINE Then
                ValiditaFine_profilazione.Text = objImprese.DatiImpresa.ValiditaFine_profilazione
            End If
            Configurazione_profilazione.Text = objImprese.DatiImpresa.configurazione_profilazione
        End If

        'catasto
        If objImprese.DatiImpresa.Flagimporta_catasto Then
            Flagimporta_catasto.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_catasto) <> AGRODATAINIZIO Then
                ValiditaInizio_catasto.Text = objImprese.DatiImpresa.ValiditaInizio_catasto
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_catasto) <> AGRODATAFINE Then
                ValiditaFine_catasto.Text = objImprese.DatiImpresa.ValiditaFine_catasto
            End If
            Configurazione_catasto.Text = objImprese.DatiImpresa.configurazione_catasto
        End If

        'pua
        If objImprese.DatiImpresa.flagimporta_pua Then
            flagimporta_pua.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_pua) <> AGRODATAINIZIO Then
                ValiditaInizio_pua.Text = objImprese.DatiImpresa.ValiditaInizio_pua
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_pua) <> AGRODATAFINE Then
                ValiditaFine_pua.Text = objImprese.DatiImpresa.ValiditaFine_pua
            End If
            Configurazione_pua.Text = objImprese.DatiImpresa.configurazione_pua
        End If

        'pratiche
        If objImprese.DatiImpresa.flagimporta_pratiche Then
            flagimporta_pratiche.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_pratiche) <> AGRODATAINIZIO Then
                ValiditaInizio_pratiche.Text = objImprese.DatiImpresa.ValiditaInizio_pratiche
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_pratiche) <> AGRODATAFINE Then
                ValiditaFine_pratiche.Text = objImprese.DatiImpresa.ValiditaFine_pratiche
            End If
            Configurazione_pratiche.Text = objImprese.DatiImpresa.configurazione_pratiche
        End If

        'pratiche_pull
        If objImprese.DatiImpresa.flagimporta_pratiche_pull Then
            flagimporta_pratiche_pull.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_pratiche_pull) <> AGRODATAINIZIO Then
                ValiditaInizio_pratiche_pull.Text = objImprese.DatiImpresa.ValiditaInizio_pratiche_pull
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_pratiche_pull) <> AGRODATAFINE Then
                ValiditaFine_pratiche_pull.Text = objImprese.DatiImpresa.ValiditaFine_pratiche_pull
            End If
            Configurazione_pratiche_pull.Text = objImprese.DatiImpresa.configurazione_pratiche_pull
        End If

        'piano_concimazione
        If objImprese.DatiImpresa.flagimporta_piano_concimazione Then
            flagimporta_piano_concimazione.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_piano_concimazione) <> AGRODATAINIZIO Then
                ValiditaInizio_piano_concimazione.Text = objImprese.DatiImpresa.ValiditaInizio_piano_concimazione
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_piano_concimazione) <> AGRODATAFINE Then
                ValiditaFine_piano_concimazione.Text = objImprese.DatiImpresa.ValiditaFine_piano_concimazione
            End If
            Configurazione_piano_concimazione.Text = objImprese.DatiImpresa.configurazione_piano_concimazione
        End If

        'LineeProduttive
        If objImprese.DatiImpresa.Flagimporta_LineeProduttive Then
            Flagimporta_LineeProduttive.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_LineeProduttive) <> AGRODATAINIZIO Then
                ValiditaInizio_LineeProduttive.Text = objImprese.DatiImpresa.ValiditaInizio_LineeProduttive
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_LineeProduttive) <> AGRODATAFINE Then
                ValiditaFine_LineeProduttive.Text = objImprese.DatiImpresa.ValiditaFine_LineeProduttive
            End If
            Configurazione_LineeProduttive.Text = objImprese.DatiImpresa.configurazione_LineeProduttive
        End If

        'piani_di_campionamento
        If objImprese.DatiImpresa.Flagimporta_piani_di_campionamento Then
            Flagimporta_piani_di_campionamento.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_piani_di_campionamento) <> AGRODATAINIZIO Then
                ValiditaInizio_piani_di_campionamento.Text = objImprese.DatiImpresa.ValiditaInizio_piani_di_campionamento
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_piani_di_campionamento) <> AGRODATAFINE Then
                ValiditaFine_piani_di_campionamento.Text = objImprese.DatiImpresa.ValiditaFine_piani_di_campionamento
            End If
            Configurazione_piani_di_campionamento.Text = objImprese.DatiImpresa.configurazione_piani_di_campionamento
        End If

        'analisi
        If objImprese.DatiImpresa.Flagimporta_analisi Then
            Flagimporta_analisi.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_analisi) <> AGRODATAINIZIO Then
                ValiditaInizio_analisi.Text = objImprese.DatiImpresa.ValiditaInizio_analisi
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_analisi) <> AGRODATAFINE Then
                ValiditaFine_analisi.Text = objImprese.DatiImpresa.ValiditaFine_analisi
            End If
            Configurazione_analisi.Text = objImprese.DatiImpresa.configurazione_analisi
        End If

        'Allegati
        If objImprese.DatiImpresa.flagimporta_allegati Then
            Flagimporta_allegati.Checked = True
            If CDate(objImprese.DatiImpresa.ValiditaInizio_allegati) <> AGRODATAINIZIO Then
                ValiditaInizio_allegati.Text = objImprese.DatiImpresa.ValiditaInizio_allegati
            End If
            If CDate(objImprese.DatiImpresa.ValiditaFine_Allegati) <> AGRODATAFINE Then
                ValiditaFine_allegati.Text = objImprese.DatiImpresa.ValiditaFine_Allegati
            End If
            Configurazione_allegati.Text = objImprese.DatiImpresa.configurazione_Allegati

        End If







        'Riempimento Controlli
        filtrone.Text = objImprese.filtrone
        filtronerisultato.Text = objImprese.filtronerisultato
        filtronerisultato_azienda.Text = objImprese.filtronerisultato_azienda




        'Riempimento Controlli Globale
        'Recodes
        If objImprese.DatiGlobali.Flagallinea_recodes Then
            FlagAllinea_Recodes.Checked = True
        End If

        'Note
        If objImprese.DatiGlobali.Flagimporta_note Then
            Flagimporta_Note.Checked = True
            If CDate(objImprese.DatiGlobali.Validita_Inizio_note) <> AGRODATAINIZIO Then
                ValiditaInizio_Note.Text = objImprese.DatiGlobali.Validita_Inizio_note
            End If
            If CDate(objImprese.DatiGlobali.Validita_Fine_note) <> AGRODATAFINE Then
                ValiditaFine_Note.Text = objImprese.DatiGlobali.Validita_Fine_note
            End If
            Configurazione_Note.Text = objImprese.DatiGlobali.configurazione_note

        End If

        'Profilazione_Globale
        If objImprese.DatiGlobali.Flagimporta_profilazione Then
            Flagimporta_Profilazione_Globale.Checked = True
            If CDate(objImprese.DatiGlobali.Validita_Inizio_profilazione) <> AGRODATAINIZIO Then
                ValiditaInizio_Profilazione_Globale.Text = objImprese.DatiGlobali.Validita_Inizio_profilazione
            End If
            If CDate(objImprese.DatiGlobali.Validita_Fine_profilazione) <> AGRODATAFINE Then
                ValiditaFine_Profilazione_Globale.Text = objImprese.DatiGlobali.Validita_Fine_profilazione
            End If
            Configurazione_Profilazione_Globale.Text = objImprese.DatiGlobali.configurazione_profilazione

        End If

        'Cac Codifica
        If objImprese.DatiGlobali.Flagimporta_cac_codifica Then
            Flagimporta_Cac_Codifica.Checked = True
            If CDate(objImprese.DatiGlobali.Validita_Inizio_cac_codifica) <> AGRODATAINIZIO Then
                ValiditaInizio_Cac_Codifica.Text = objImprese.DatiGlobali.Validita_Inizio_cac_codifica
            End If
            If CDate(objImprese.DatiGlobali.Validita_Fine_cac_codifica) <> AGRODATAFINE Then
                ValiditaFine_Cac_Codifica.Text = objImprese.DatiGlobali.Validita_Fine_cac_codifica
            End If
            Configurazione_Cac_Codifica.Text = objImprese.DatiGlobali.configurazione_cac_codifica

        End If

        'Piani Campionamento
        If objImprese.DatiGlobali.Flagimporta_pianicampionamento Then
            Flagimporta_PianiCampionamento.Checked = True
            If CDate(objImprese.DatiGlobali.Validita_Inizio_pianicampionamento) <> AGRODATAINIZIO Then
                ValiditaInizio_PianiCampionamento.Text = objImprese.DatiGlobali.Validita_Inizio_pianicampionamento
            End If
            If CDate(objImprese.DatiGlobali.Validita_Fine_pianicampionamento) <> AGRODATAFINE Then
                ValiditaFine_PianiCampionamento.Text = objImprese.DatiGlobali.Validita_Fine_pianicampionamento
            End If
            Configurazione_PianiCampionamento.Text = objImprese.DatiGlobali.configurazione_pianicampionamento

        End If

        'Contatti
        If objImprese.DatiGlobali.Flagimporta_contatti Then
            Flagimporta_Contatti.Checked = True
            If CDate(objImprese.DatiGlobali.Validita_Inizio_contatti) <> AGRODATAINIZIO Then
                ValiditaInizio_Contatti.Text = objImprese.DatiGlobali.Validita_Inizio_contatti
            End If
            If CDate(objImprese.DatiGlobali.Validita_Fine_contatti) <> AGRODATAFINE Then
                ValiditaFine_Contatti.Text = objImprese.DatiGlobali.Validita_Fine_contatti
            End If
            Configurazione_Contatti.Text = objImprese.DatiGlobali.configurazione_contatti

        End If

        'Macchine
        If objImprese.DatiGlobali.Flagimporta_macchine Then
            Flagimporta_Macchine.Checked = True
            If CDate(objImprese.DatiGlobali.Validita_Inizio_macchine) <> AGRODATAINIZIO Then
                ValiditaInizio_Macchine.Text = objImprese.DatiGlobali.Validita_Inizio_macchine
            End If
            If CDate(objImprese.DatiGlobali.Validita_Fine_macchine) <> AGRODATAFINE Then
                ValiditaFine_Macchine.Text = objImprese.DatiGlobali.Validita_Fine_macchine
            End If
            Configurazione_Macchine.Text = objImprese.DatiGlobali.configurazione_macchine

        End If

        'MateriePrimePubbliche
        If objImprese.DatiGlobali.Flagimporta_materieprime Then
            Flagimporta_MateriePrimePubbliche.Checked = True
            If CDate(objImprese.DatiGlobali.Validita_Inizio_materieprime) <> AGRODATAINIZIO Then
                ValiditaInizio_MateriePrimePubbliche.Text = objImprese.DatiGlobali.Validita_Inizio_materieprime
            End If
            If CDate(objImprese.DatiGlobali.Validita_Fine_materieprime) <> AGRODATAFINE Then
                ValiditaFine_MateriePrimePubbliche.Text = objImprese.DatiGlobali.Validita_Fine_materieprime
            End If
            Configurazione_MateriePrimePubbliche.Text = objImprese.DatiGlobali.configurazione_materieprime

        End If

        'Campionature
        If objImprese.DatiGlobali.Flagimporta_materieprimecampionature Then
            Flagimporta_Campionature.Checked = True
            If CDate(objImprese.DatiGlobali.Validita_Inizio_materieprimecampionature) <> AGRODATAINIZIO Then
                ValiditaInizio_Campionature.Text = objImprese.DatiGlobali.Validita_Inizio_materieprimecampionature
            End If
            If CDate(objImprese.DatiGlobali.Validita_Fine_materieprimecampionature) <> AGRODATAFINE Then
                ValiditaFine_Campionature.Text = objImprese.DatiGlobali.Validita_Fine_materieprimecampionature
            End If
            Configurazione_Campionature.Text = objImprese.DatiGlobali.configurazione_materieprimecampionature

        End If


    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function EsportaDatiG2G(ByVal ID_Cfg As Integer, ByVal XML_Cfg As String) As RispostaStandard
        Const NomeFunzione As String = "G2GEsportaDati"
        Dim r As New RispostaStandard

        Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim Messaggio_di_Ritorno_Opzionale As String = ""
        Dim cartellaLog As String = ""

        Dim Log_G2G As New StringBuilder
        Dim Log_Errori As New StringBuilder
        Dim Log_Riepilogo As New StringBuilder

        Dim messaggioErrori As String = ""

        Dim nomeFileLog As String = ".\" & CStr(DateTime.Now.Year) & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & "log_G2G.txt"
        Dim nomeFileErrori As String = ".\" & CStr(DateTime.Now.Year) & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & "log_Errori.txt"
        Dim nomeFileRiepilogo As String = ".\" & CStr(DateTime.Now.Year) & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & "log_Riepilogo.txt"

        Try

            cartellaLog = FileSystemHelper.AggiungiSlashSeNonEsiste(If(objParametri_Server.LogDirectory <> "", objParametri_Server.LogDirectory, "C:\GIASLan\Log")) & "G2G"
            If Not Directory.Exists(cartellaLog) Then
                Directory.CreateDirectory(cartellaLog)
            End If

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Inizio  Gias 2 Gias -->" & NomeFunzione)
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Leggo le configurazioni")

            If ID_Cfg <> 0 Then
                Dim objG2G As New AgronicaCoreG2GLocalDal.G2GLocal_R
                Dim dt As DataTable = objG2G.LeggiConfigurazioni(ID_Cfg, objParametri_Server)
                If dt.Rows.Count = 0 Then
                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: Nessuna configurazione trovata con l'id " & ID_Cfg)
                    Log_Errori.AppendLine(CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: Nessuna configurazione trovata con l'id " & ID_Cfg)
                    r.RispostaOK = False
                    r.Errore = "Nessuna configurazione trovata con l'id " & ID_Cfg
                    Return r
                Else
                    XML_Cfg = dt.Rows(0).Item("G2GLocalConfigurazioni_CFG")
                End If
            ElseIf String.IsNullOrEmpty(XML_Cfg) Then
                r.RispostaOK = False
                r.Errore = "Nessuna configurazione impostata"
                Return r
            Else
                XML_Cfg = Encoding.UTF8.GetString(Convert.FromBase64String(XML_Cfg))
            End If

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Eseguo il Gias 2 Gias.")

            Dim imprese As XDocument = XDocument.Parse(XML_Cfg)

            Dim G2G As New GIAS2GIAS_LOCALE.GIAS_2_GIAS
            G2G.GIAS_2_GIASCiclaImprese(Messaggio_di_Ritorno_Opzionale,
                                        Log_G2G, Log_Errori, Log_Riepilogo, cartellaLog, imprese,
                                        FromServizio:=False,
                                        nomeFileLog:=nomeFileLog,
                                        nomeFileErrori:=nomeFileErrori,
                                        nomeFileRiepilogo:=nomeFileRiepilogo)

            Log_G2G.AppendLine(vbCrLf & CStr(Date.Now) & " - " & "Fine Gias 2 Gias --> " & NomeFunzione)

            r.RispostaOK = True

        Catch ex As Exception

            messaggioErrori = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.ToString
            Log_Errori.AppendLine(messaggioErrori)

        Finally

            Messaggio_di_Ritorno_Opzionale &= Log_G2G.ToString

            My.Computer.FileSystem.WriteAllText(cartellaLog & nomeFileLog, messaggioErrori, True)

            Dim messaggioFinale As String = vbCrLf & CStr(Date.Now) & " - " & "Fine Gias 2 Gias --> " & NomeFunzione & vbCrLf
            My.Computer.FileSystem.WriteAllText(cartellaLog & nomeFileLog, messaggioFinale, True)

            My.Computer.FileSystem.WriteAllText(cartellaLog & nomeFileErrori, Log_Errori.ToString, True)
            My.Computer.FileSystem.WriteAllText(cartellaLog & nomeFileRiepilogo, Log_Riepilogo.ToString, True)

        End Try

        'Messaggio_di_Ritorno_Opzionale = GIAS2GIAS_LOCALE.GIAS_2_GIAS.ElaboraLog(Messaggio_di_Ritorno_Opzionale)
        Messaggio_di_Ritorno_Opzionale = Replace(Messaggio_di_Ritorno_Opzionale, vbCrLf, "<br>")

        If r.RispostaOK Then
            r.RispostaStringa = Messaggio_di_Ritorno_Opzionale
        Else
            r.Errore = Messaggio_di_Ritorno_Opzionale
        End If

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElaboraLogG2G(ByVal log As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim piva = ""
        Dim logErrori = ""
        Dim pivaErrori = ""
        Dim logImprese = ""
        Dim numImprese = 0
        Dim lines = log.Split(vbLf)

        For line = 0 To lines.Length - 1
            Dim errore = lines(line).ToLower().IndexOf("error") <> -1

            If lines(line).IndexOf("----- Impresa") <> -1 Then

                If logImprese.ToLower().IndexOf("error") <> -1 Then
                    logErrori += logImprese & vbLf
                    If piva <> "" Then pivaErrori += (If(pivaErrori <> "", ",", "")) & """" & piva & """"
                    piva = ""
                    logImprese = ""
                    numImprese += 1
                End If

                Dim index = lines(line).IndexOf(" - ")
                piva = If(index > 0, Mid(lines(line), index + 4, 11), "")
                logImprese = lines(line) & vbLf
            ElseIf errore Then
                logImprese += lines(line) & vbLf
            End If
        Next

        If logImprese.ToLower().IndexOf("error") <> -1 Then
            logErrori += logImprese & vbLf
            If piva <> "" Then pivaErrori += (If(pivaErrori <> "", ", ", "")) & """" & piva & """"
            numImprese += 1
        End If

        If pivaErrori <> "" Then
            logErrori = numImprese & " Imprese con errori: " & pivaErrori & vbLf & vbLf & logErrori
        End If

        r.RispostaOK = True
        r.RispostaStringa = logErrori

        Return r

    End Function


    Private Function CaricaCfgImprese() As XDocument
        Dim objParamentri_server As New AgronicaCoreDataProvider.AgronicaCoreParametri
        Dim albero As XDocument = Nothing
        Dim dt As DataTable

        Try
            objParamentri_server = HttpContext.Current.Session("ASG_objParametri_Server")

            Dim l As New AgronicaCoreG2GLocalDal.G2GLocal_R
            dt = l.LeggiConfigurazioni(
                ddlConfigurazioni.SelectedValue,
                objParamentri_server
            )

            If dt.Rows.Count > 0 Then
                txtCfg.Text = dt.Rows(0)("G2GLocalConfigurazioni_DES")
                albero = XDocument.Parse(dt.Rows(0)("G2GLocalConfigurazioni_CFG"))
            End If
        Catch
        End Try

        Return albero

    End Function

    'Private Sub CaricaCfg()

    '    Dim objParamentri_server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    '    objParamentri_server = HttpContext.Current.Session("ASG_objParametri_Server")

    '    '  Marco Grilli, 12/06/2014 17:44:12: pulisco tutti i controlli
    '    svuotaControlliOrigine(True)
    '    svuotaControlliDestinazione(True)

    '    Dim dt As DataTable
    '    Dim l As New AgronicaCoreG2GLocalDal.G2GLocal_R
    '    dt = l.LeggiConfigurazioni(
    '        ddlConfigurazioni.SelectedValue,
    '        objParamentri_server
    '    )

    '    txtCfg.Text = dt.Rows(0)("G2GLocalConfigurazioni_DES")

    '    If dt.Rows.Count > 0 Then
    '        Dim albero As XDocument = XDocument.Parse(dt.Rows(0)("G2GLocalConfigurazioni_CFG"))


    '        Dim objOpzioni As Gias2Gias_LIB.clsOpzioni =
    '                       (From o In albero.<dati>.<configurazione>
    '                        Select New Gias2Gias_LIB.clsOpzioni With {
    '                           .wsimportaGiasURl = o.<urlWsimportaGias>.Value,
    '                           .Connessione_Server_GIAS_Origine = o.<connessioni>.<connessione>.<Connessione_SERVER_ORIGINE>.Value,
    '                           .Connessione_Server_GIAS_Destinazione = o.<connessioni>.<connessione>.<Connessione_SERVER_DESTINAZIONE>.Value,
    '                           .Connessione_Utenti_GIAS_Origine = o.<connessioni>.<connessione>.<Connessione_UTENTI_ORIGINE>.Value,
    '                           .Connessione_Utenti_GIAS_Destinazione = o.<connessioni>.<connessione>.<Connessione_UTENTI_DESTINAZIONE>.Value,
    '                           .ProgressivoGIAS_ORIGINE = o.<parametri>.<ProgressivoGIAS_ORIGINE>.Value,
    '                           .ProgressivoGIAS_DESTINAZIONE = o.<parametri>.<ProgressivoGIAS_DESTINAZIONE>.Value,
    '                           .PercorsoConnessioni = o.<parametri>.<percorsoConnessioni>.Value,
    '                           .SuperUser_CodFiscale_ORIGINE = o.<parametri>.<PivaSuperUser_ORIGINE>.Value,
    '                           .SuperUser_CodFiscale_DESTINAZIONE = o.<parametri>.<PivaSuperUser_DESTINAZIONE>.Value,
    '                           .SuperUser_Username_ORIGINE = o.<parametri>.<UsernameSuperUser_Origine>.Value,
    '                           .SuperUser_Username_DESTINAZIONE = o.<parametri>.<UsernameSuperUser_Destinazione>.Value,
    '                           .Import_CodFiscale_ORIGINE = o.<parametri>.<Import_CodFiscale_ORIGINE>.Value,
    '                           .Import_CodFiscale_DESTINAZIONE = o.<parametri>.<Import_CodFiscale_DESTINAZIONE>.Value,
    '                           .Import_Username_ORIGINE = o.<parametri>.<Import_Username_ORIGINE>.Value,
    '                           .Import_Username_DESTINAZIONE = o.<parametri>.<Import_Username_DESTINAZIONE>.Value
    '                       }).FirstOrDefault


    '        txtProgressivoGIAS_ORIGINE.Text = objOpzioni.ProgressivoGIAS_ORIGINE
    '        txtProgressivoGIAS_DESTINAZIONE.Text = objOpzioni.ProgressivoGIAS_DESTINAZIONE
    '        txtPivaSuperUser_ORIGINE.Text = objOpzioni.SuperUser_CodFiscale_ORIGINE
    '        txtPivaSuperUser_DESTINAZIONE.Text = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
    '        txtUsernameSuperUser_Origine.Text = objOpzioni.SuperUser_Username_ORIGINE
    '        txtUsernameSuperUser_Destinazione.Text = objOpzioni.SuperUser_Username_DESTINAZIONE
    '        txtImport_CodFiscale_ORIGINE.Text = objOpzioni.Import_CodFiscale_ORIGINE
    '        txtImport_CodFiscale_DESTINAZIONE.Text = objOpzioni.Import_CodFiscale_DESTINAZIONE
    '        txtImport_Username_ORIGINE.Text = objOpzioni.Import_Username_ORIGINE
    '        txtImport_Username_DESTINAZIONE.Text = objOpzioni.Import_Username_DESTINAZIONE

    '        Txb_WebService.Text = objOpzioni.wsimportaGiasURl
    '        Ckb_G2GLocale.Checked = objOpzioni.isGias2Gias_local
    '        Txb_WebService.Enabled = Not objOpzioni.isGias2Gias_local


    '        popolaDDConnessioneOrigine()
    '        popolaDDConnessioneDestinazione()


    '        '  Marco Grilli, 12/06/2014 17:25:12: per selezionare l'elemento giusto, dovevo prima popolare le ddb
    '        '                                     ma soprattutto sapere se è local2local (e quindi assegnare l'url)
    '        ddConnessione_SERVER_ORIGINE.Text = objOpzioni.Connessione_Server_GIAS_Origine
    '        ddConnessione_SERVER_DESTINAZIONE.Text = objOpzioni.Connessione_Server_GIAS_Destinazione
    '        ddConnessione_UTENTI_ORIGINE.Text = objOpzioni.Connessione_Utenti_GIAS_Origine
    '        ddConnessione_UTENTI_DESTINAZIONE.Text = objOpzioni.Connessione_Utenti_GIAS_Destinazione

    '    End If

    'End Sub

    '  Marco Grilli, 12/06/2014 16:59:01: popola la drop down box con tutte le configurazioni G2G presenti sul DB
    Private Sub popolaDDConfigurazioni()

        '  Marco Grilli, 12/06/2014 16:59:09:  ottengo l'objparametri del server
        Dim objParametri_server As New AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")

        '  Marco Grilli, 12/06/2014 16:59:46: leggo le configurazioni
        Dim l As New AgronicaCoreG2GLocalDal.G2GLocal_R
        Dim dt As DataTable = l.LeggiConfigurazioni(
            0,
            objParametri_server
        )

        '  Marco Grilli, 12/06/2014 17:00:07: pulisco ed aggiungo le configruazioni alla ddb
        ddlConfigurazioni.Items.Clear()
        ddlConfigurazioni.Items.Add(New ListItem With {.Value = "-1", .Text = "Seleziona ..."})

        If dt.Rows.Count > 0 Then

            For Each dr In dt.Rows
                ddlConfigurazioni.Items.Add(New ListItem With {
                    .Value = dr("G2GLocalConfigurazioni_COD"),
                    .Text = dr("G2GLocalConfigurazioni_DES")
                })
            Next

        End If

    End Sub

    '  Marco Grilli, 12/06/2014 17:00:38: popola le ddb delle connesioni ai db di origine
    Private Sub popolaDDConnessioneOrigine()

        '  Marco Grilli, 12/06/2014 17:01:01: se c'è il superserver prendo da lì l'objparametri, altrimenti dal server
        Dim objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If objParametri_server Is Nothing Then
            objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
        End If

        popolaDDConnessione(TipiEnumerativi.enum_Tipo_DB.GIAS_SERVER, ddConnessione_SERVER_ORIGINE, objParametri_server)
        popolaDDConnessione(TipiEnumerativi.enum_Tipo_DB.GIAS_UTENTI, ddConnessione_UTENTI_ORIGINE, objParametri_server)
    End Sub

    '  Marco Grilli, 12/06/2014 17:00:38: popola le ddb delle connesioni ai db di destinazione
    Private Sub popolaDDConnessioneDestinazione()

        ''se il G2G è locale...
        'If Ckb_G2GLocale.Checked Then

        '    '  Marco Grilli, 12/06/2014 17:01:01: se c'è il superserver prendo da lì l'objparametri, altrimenti dal server
        Dim objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If objParametri_server Is Nothing Then
            objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
        End If
        popolaDDConnessione(TipiEnumerativi.enum_Tipo_DB.GIAS_SERVER, ddConnessione_SERVER_DESTINAZIONE, objParametri_server)
        popolaDDConnessione(TipiEnumerativi.enum_Tipo_DB.GIAS_UTENTI, ddConnessione_UTENTI_DESTINAZIONE, objParametri_server)
        'Else
        'se il G2G è da WS... simulo il click del pulsante "leggi da WS"
        If (Txb_WebService.Text <> "") Then
            BtnLeggiDestDaWS_Click(Nothing, Nothing)
        End If
        'End If
    End Sub

    Private Sub popolaDDConnessione(ByVal tipo As AgronicaCoreDataProvider.TipiEnumerativi.enum_Tipo_DB, ByRef dd As DropDownList, ByVal objParamentri_server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim leggiCN As New AgronicaCoreDataProvider.Connessioni
        Dim dtCN As DataTable =
        leggiCN.Leggi(
                0,
                tipo,
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                0,
                "",
                AGRODATAINIZIO,
                AGRODATAFINE,
                "",
                " Descrizione ",
                objParamentri_server
                )

        dd.Items.Clear()
        dd.Items.Add(New ListItem With {.Value = -1, .Text = "Seleziona un database"})
        For Each drCN In dtCN.Rows
            dd.Items.Add(New ListItem With {.Value = drCN("ID_DB"), .Text = drCN("Descrizione")})
        Next


    End Sub

    Private _g2g_set As clsOpzioni


    'Protected Sub start_Click(sender As Object, e As EventArgs) Handles start.Click

    '    Dim PathFileXMLOpzioniImport As String = AppSettings("PathFileXMLOpzioniImport")
    '    PathFileXMLOpzioniImport = Server.MapPath(".") & "\" & PathFileXMLOpzioniImport


    '    Dim objParamentri_server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    '    objParamentri_server = HttpContext.Current.Session("ASG_objParametri_Server")

    '    Dim log_G2G As New StringBuilder
    '    Dim log_Errori As New StringBuilder
    '    Dim log_Riepilogo As New StringBuilder


    '    'My.Computer.FileSystem.WriteAllText(".\log_G2G.txt", log_G2G.ToString, False)
    '    'My.Computer.FileSystem.WriteAllText(".\log_Errori.txt", log_Errori.ToString, False)
    '    'My.Computer.FileSystem.WriteAllText(".\log_Riepilogo.txt", log_Riepilogo.ToString, False)



    '    Dim G2G As New GIAS2GIAS_LOCALE.GIAS_2_GIAS

    '    Dim dirLog As String = Server.MapPath(".")

    '    Dim urlWS As String = ""


    '    G2G.GIAS_2_GIAS(
    '        dirLog,
    '        log_G2G,
    '        log_Errori,
    '        log_Riepilogo,
    '        urlWS,
    '        ddConnessione_SERVER_ORIGINE.SelectedValue,
    '        ddConnessione_SERVER_DESTINAZIONE.SelectedValue,
    '        ddConnessione_UTENTI_ORIGINE.SelectedValue,
    '        ddConnessione_UTENTI_DESTINAZIONE.SelectedValue,
    '        txtProgressivoGIAS_ORIGINE.Text,
    '        txtProgressivoGIAS_DESTINAZIONE.Text,
    '        txtPivaSuperUser_ORIGINE.Text,
    '        txtPivaSuperUser_DESTINAZIONE.Text,
    '        txtUsernameSuperUser_Origine.Text,
    '        txtUsernameSuperUser_Destinazione.Text,
    '        txtImport_CodFiscale_ORIGINE.Text,
    '        txtImport_CodFiscale_DESTINAZIONE.Text,
    '        txtImport_Username_ORIGINE.Text,
    '        txtImport_Username_DESTINAZIONE.Text,
    '        PercorsoConnessioni,
    '        hidTabella.Value
    '    )

    '    '  RenderProgressBar(totale, i, 1, 1)

    '    lblEsito.Text = "Importazione completata."

    'End Sub

    Protected Sub ddlConfigurazioni_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlConfigurazioni.SelectedIndexChanged

        If ddlConfigurazioni.SelectedIndex <= 0 Then

            svuotaControlliOrigine(True)
            svuotaControlliDestinazione(True)
        Else

            LeggiConfigurazioneG2G(ddlConfigurazioni.SelectedValue)


        End If

    End Sub

    Protected Sub ddConnessione_UTENTI_ORIGINE_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddConnessione_UTENTI_ORIGINE.SelectedIndexChanged

        If ddConnessione_UTENTI_ORIGINE.SelectedValue <> -1 Then
            genericoSelectChangeDaLocale(ddConnessione_UTENTI_ORIGINE.SelectedValue, txtImport_Username_ORIGINE, txtUsernameSuperUser_Origine, txtProgressivoGIAS_ORIGINE, txtPivaSuperUser_ORIGINE, txtImport_CodFiscale_ORIGINE)
        End If


    End Sub






    'Protected Sub getImprese_Click(sender As Object, e As EventArgs) Handles getImprese.Click
    '    Dim sImp As String = "true"

    '    Dim objParamentri_server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    '    objParamentri_server = HttpContext.Current.Session("ASG_objParametri_Server")

    '    'commentare appena pronto il superserver
    '    If IsNothing(Session("ASG_objParametri_Super_Server")) AndAlso String.IsNullOrEmpty(Session("ASG_objParametri_Super_Server")) Then
    '        Session("ASG_objParametri_Super_Server") = objParamentri_server
    '    End If

    '    Dim log_G2G As New StringBuilder
    '    Dim log_Errori As New StringBuilder
    '    Dim log_Riepilogo As New StringBuilder

    '    Dim dirLog As String = Server.MapPath(".")

    '    Dim urlWS As String = ""
    '    'If Not (Ckb_G2GLocale.Checked) Then
    '    '    urlWS = Txb_WebService.Text
    '    'End If

    '    Dim G2G As New GIAS2GIAS_LOCALE.GIAS_2_GIAS
    '    G2G.GIAS_2_GIAS(
    '      dirLog,
    '      log_G2G,
    '      log_Errori,
    '      log_Riepilogo,
    '      urlWS,
    '      ddConnessione_SERVER_ORIGINE.SelectedValue,
    '      ddConnessione_SERVER_DESTINAZIONE.SelectedValue,
    '      ddConnessione_UTENTI_ORIGINE.SelectedValue,
    '      ddConnessione_UTENTI_DESTINAZIONE.SelectedValue,
    '      txtProgressivoGIAS_ORIGINE.Text,
    '      txtProgressivoGIAS_DESTINAZIONE.Text,
    '      txtPivaSuperUser_ORIGINE.Text,
    '      txtPivaSuperUser_DESTINAZIONE.Text,
    '      txtUsernameSuperUser_Origine.Text,
    '      txtUsernameSuperUser_Destinazione.Text,
    '      txtImport_CodFiscale_ORIGINE.Text,
    '      txtImport_CodFiscale_DESTINAZIONE.Text,
    '      txtImport_Username_ORIGINE.Text,
    '      txtImport_Username_DESTINAZIONE.Text,
    '      PercorsoConnessioni,
    '      sImp)

    '    Dim xDocImp As XDocument = XDocument.Parse(sImp)

    '    tblImprese.ID = "tblScelta"
    '    Dim rigaImpresa As New TableRow
    '    Dim cellaImpresa As New TableCell


    '    predisponiHeaderTabella(xDocImp, tblImprese)

    '    Dim ns As XNamespace = "http://G2G"

    '    Dim xDocCfgImp As XDocument = CaricaCfgImprese()

    '    For Each curImp In (From i In xDocImp.Elements(ns + "dati").Elements(ns + "imprese").Elements(ns + "impresa"))
    '        rigaImpresa = New TableRow
    '        cellaImpresa = New TableCell

    '        '  Marco Grilli, 30/05/2014 18:20:30: assegno le variabili
    '        Dim curPiva As String = curImp.Elements(ns + "origine").Value
    '        Dim curSaCod As String = curImp.Elements(ns + "sa_cod").Value
    '        Dim lRagSoc As String = curImp.Elements(ns + "rag_soc").Value
    '        Dim lPivaDestinazione As String = curImp.Elements(ns + "destinazione").Value
    '        Dim lPivaPadre As String = curImp.Elements(ns + "padre").Elements(ns + "impresa").FirstOrDefault.Elements(ns + "piva").Value

    '        Dim curCod As String = curPiva & "|" & curSaCod

    '        '  Marco Grilli, 30/05/2014 18:18:44: Controllo se l'impresa è presente in config del G2G e quindi da checkare
    '        Dim isImpInCfg As Boolean = False
    '        Dim impInCfg As XElement
    '        '  Marco Grilli, 11/06/2014 15:21:48: se ha trovato una configurazione...
    '        If Not (xDocCfgImp Is Nothing) Then
    '            For Each curImpInCfg In (From i In xDocCfgImp.Elements(ns + "dati").Elements(ns + "imprese").Elements(ns + "impresa"))
    '                If (curPiva = curImpInCfg.Elements(ns + "origine").Value) Then
    '                    isImpInCfg = True
    '                    impInCfg = curImpInCfg
    '                    Exit For
    '                End If
    '            Next
    '        End If

    '        '  Marco Grilli, 03/06/2014 09:20:52: uso la mia var booleana per mettere il check
    '        cellaImpresa.Controls.Add(New CheckBox With {.ID = curCod, .Text = curPiva & "|" & lRagSoc & "|" & curSaCod, .Checked = isImpInCfg, .CssClass = "aziendaFiglia"})
    '        rigaImpresa.Cells.Add(cellaImpresa)

    '        cellaImpresa = New TableCell
    '        cellaImpresa.Controls.Add(New Label With {.ID = curCod & "_dest", .Text = lPivaDestinazione})
    '        rigaImpresa.Cells.Add(cellaImpresa)

    '        cellaImpresa = New TableCell
    '        cellaImpresa.Controls.Add(New Label With {.ID = curCod & "_padre", .Text = lPivaPadre})
    '        rigaImpresa.Cells.Add(cellaImpresa)

    '        cellaImpresa = New TableCell
    '        Dim ck As CheckBox
    '        Dim lNameToImport As String = ""

    '        For Each curDatiToImportImp In curImp.Descendants
    '            lNameToImport = curDatiToImportImp.Name.ToString.Replace("{http://G2G}", "")
    '            If lNameToImport.StartsWith("flagimporta_") Then

    '                '  Marco Grilli, 03/06/2014 10:00:15: verifico se il singolo dato (agenda, piano colturale, ecc sono salvati nella configurazione per l'impresa corrente)
    '                Dim isDatoInImpCfg As Boolean = False
    '                '  Marco Grilli, 03/06/2014 10:01:01: se l'impresa non è selezionata, neanche sto a cercare e lascio i check a false
    '                If isImpInCfg Then
    '                    For Each curDatoInImpCfg In impInCfg.Descendants
    '                        If (curDatoInImpCfg.Name.ToString.Replace("{http://G2G}", "") = lNameToImport) Then
    '                            isDatoInImpCfg = True
    '                            Exit For
    '                        End If
    '                    Next
    '                End If

    '                lNameToImport = lNameToImport.Replace("flagimporta_", "")
    '                ck = New CheckBox
    '                ck.ID = curCod & "|" & lNameToImport
    '                ck.Text = lNameToImport
    '                ck.Checked = isDatoInImpCfg
    '                ck.CssClass = "datiAzienda_" & lNameToImport & "Figlia"
    '                cellaImpresa.Controls.Add(ck)
    '                cellaImpresa.Controls.Add(New LiteralControl("<br/>"))
    '            End If
    '        Next

    '        rigaImpresa.Cells.Add(cellaImpresa)

    '        tblImprese.Rows.Add(rigaImpresa)

    '    Next

    '    start.Enabled = True
    'End Sub

    Private Sub predisponiHeaderTabella(ByVal xElem As XDocument, ByRef tblImprese As Table)

        tblImprese.CssClass = "ui-widget-content font8pt"

        Dim th As New TableHeaderRow
        Dim tc As TableHeaderCell

        th.CssClass = "ui-widget-header font8pt"

        Dim ckSelezionaTutteImprese As New CheckBox
        ckSelezionaTutteImprese.ID = "ckImprese"
        ckSelezionaTutteImprese.Text = "Imprese"
        ckSelezionaTutteImprese.CssClass = "azienda"
        ckSelezionaTutteImprese.Checked = True

        tc = New TableHeaderCell
        tc.Controls.Add(ckSelezionaTutteImprese)
        th.Cells.Add(tc)


        tc = New TableHeaderCell
        tc.Controls.Add(New LiteralControl("Nuova partita.iva in destinazione"))
        th.Cells.Add(tc)

        tc = New TableHeaderCell
        tc.Controls.Add(New LiteralControl("Partita.iva padre in gerarchia"))
        th.Cells.Add(tc)

        tc = New TableHeaderCell
        Dim ckSelezionaTuttiDatiImport As CheckBox
        Dim elemName As String = ""
        Dim ns As XNamespace = "http://G2G"

        For Each cEl In (From i In xElem.Elements(ns + "dati").Elements(ns + "imprese").Elements(ns + "impresa"))
            For Each curDatiToImportImp In cEl.Descendants

                elemName = curDatiToImportImp.Name.ToString.Replace("{http://G2G}", "")

                If elemName.StartsWith("flagimporta_") Then
                    elemName = elemName.Replace("flagimporta_", "")

                    If (tc.Controls.Count = 0) OrElse
                        IsNothing(AgronicaCoreUtility.Ricerca.FindControlIterative2(tc, "ckDati_impresa" & elemName)) Then

                        ckSelezionaTuttiDatiImport = New CheckBox
                        ckSelezionaTuttiDatiImport.ID = "ckDati_impresa" & elemName

                        ckSelezionaTuttiDatiImport.Text = elemName
                        ckSelezionaTuttiDatiImport.CssClass = "masterSeleziona datiAzienda_" & elemName
                        ckSelezionaTuttiDatiImport.Checked = True
                        tc.Controls.Add(ckSelezionaTuttiDatiImport)

                        tc.Controls.Add(New LiteralControl("<br />"))

                    End If

                End If
            Next
        Next

        th.Cells.Add(tc)

        tblImprese.Rows.Add(th)

    End Sub


    Protected Sub btn_DDConnessioni_Click(sender As Object, e As EventArgs) Handles btn_DDConnessioni.Click

        popolaDDConnessioneDestinazione()

    End Sub
    Protected Sub btn_salva_Click(sender As Object, e As EventArgs) Handles btn_salva.Click

        Dim log_G2G As New StringBuilder
        Dim log_Errori As New StringBuilder
        Dim log_Riepilogo As New StringBuilder

        Dim dirLog As String = Server.MapPath(".")

        Dim objParamentri_server As New AgronicaCoreDataProvider.AgronicaCoreParametri
        objParamentri_server = HttpContext.Current.Session("ASG_objParametri_Server")

        'commentare appena pronto il superserver
        If IsNothing(Session("ASG_objParametri_Super_Server")) AndAlso String.IsNullOrEmpty(Session("ASG_objParametri_Super_Server")) Then
            Session("ASG_objParametri_Super_Server") = objParamentri_server
        End If

        Dim urlWS As String = ""
        Dim Codice As Integer

        urlWS = Txb_WebService.Text

        LblErr.Visible = False

        If Trim(txtCfg.Text) <> "" And IsNumeric(txtProgressivoGIAS_ORIGINE.Text) And IsNumeric(txtProgressivoGIAS_DESTINAZIONE.Text) Then

            Dim cfgGlobali As List(Of clsDatiImpresaLista) = New List(Of clsDatiImpresaLista)
            Dim cfgImpresa As List(Of clsDatiImpresaLista) = New List(Of clsDatiImpresaLista)

            'Globali

            'Recodes
            If FlagAllinea_Recodes.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagallinea_recodes"
                cfgGlobali.Add(aggiungi)
            End If

            'Note
            If Flagimporta_Note.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_note"
                If IsDate(ValiditaInizio_Note.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_Note.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_Note.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_Note.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_Note.Text
                cfgGlobali.Add(aggiungi)
            End If

            'Profilazione
            If Flagimporta_Profilazione_Globale.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_profilazione"
                If IsDate(ValiditaInizio_Profilazione_Globale.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_Profilazione_Globale.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_Profilazione_Globale.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_Profilazione_Globale.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_Profilazione_Globale.Text
                cfgGlobali.Add(aggiungi)
            End If

            'cac_codifica
            If Flagimporta_Cac_Codifica.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_cac_codifica"
                If IsDate(ValiditaInizio_Cac_Codifica.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_Cac_Codifica.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_Cac_Codifica.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_Cac_Codifica.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_Cac_Codifica.Text
                cfgGlobali.Add(aggiungi)
            End If

            'pianicampionamento
            If Flagimporta_PianiCampionamento.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_pianicampionamento"
                If IsDate(ValiditaInizio_PianiCampionamento.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_PianiCampionamento.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_PianiCampionamento.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_PianiCampionamento.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_PianiCampionamento.Text
                cfgGlobali.Add(aggiungi)
            End If


            'contatti
            If Flagimporta_Contatti.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_contatti"
                If IsDate(ValiditaInizio_Contatti.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_Contatti.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_Contatti.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_Contatti.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_Contatti.Text
                cfgGlobali.Add(aggiungi)
            End If

            'macchine
            If Flagimporta_Macchine.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_macchine"
                If IsDate(ValiditaInizio_Macchine.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_Macchine.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_Macchine.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_Macchine.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_Macchine.Text
                cfgGlobali.Add(aggiungi)
            End If

            'MateriePrimePubbliche
            If Flagimporta_MateriePrimePubbliche.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_materieprime"
                If IsDate(ValiditaInizio_MateriePrimePubbliche.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_MateriePrimePubbliche.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_MateriePrimePubbliche.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_MateriePrimePubbliche.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_MateriePrimePubbliche.Text
                cfgGlobali.Add(aggiungi)
            End If

            'campionature
            If Flagimporta_Campionature.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_materieprimecampionature"
                If IsDate(ValiditaInizio_Campionature.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_Campionature.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_Campionature.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_Campionature.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_Campionature.Text
                cfgGlobali.Add(aggiungi)
            End If

            'fine globali
            '----------------------------------------------------------------------------


            'Imprese

            'Audit
            If FlagImporta_Audit.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_audit"
                If IsDate(ValiditaInizio_Audit.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_Audit.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_Audit.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_Audit.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_Audit.Text
                cfgImpresa.Add(aggiungi)
            End If

            'Audit_Interviste
            If FlagImporta_Audit_Interviste.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_audit_interviste"
                If IsDate(ValiditaInizio_Audit_Interviste.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_Audit_Interviste.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_Audit_Interviste.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_Audit_Interviste.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_Audit_Interviste.Text
                cfgImpresa.Add(aggiungi)
            End If

            'catasto
            If Flagimporta_catasto.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_catasto"
                If IsDate(ValiditaInizio_catasto.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_catasto.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_catasto.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_catasto.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_catasto.Text
                cfgImpresa.Add(aggiungi)
            End If

            'gis
            If Flagimporta_gis.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_gis"
                If IsDate(ValiditaInizio_gis.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_gis.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_gis.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_gis.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_gis.Text
                cfgImpresa.Add(aggiungi)
            End If

            'pianocolturale
            If Flagimporta_pianocolturale.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_pianocolturale"
                If IsDate(ValiditaInizio_pianocolturale.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_pianocolturale.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_pianocolturale.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_pianocolturale.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_pianocolturale.Text
                cfgImpresa.Add(aggiungi)
            End If

            'materieprime
            If Flagimporta_materieprime.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_materieprime"
                If IsDate(ValiditaInizio_materieprime.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_materieprime.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_materieprime.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_materieprime.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_materieprime.Text
                cfgImpresa.Add(aggiungi)
            End If

            'agenda
            If flagimporta_agenda.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_agenda"
                If IsDate(ValiditaInizio_agenda.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_agenda.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_agenda.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_agenda.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_agenda.Text
                cfgImpresa.Add(aggiungi)
            End If

            'pap
            If flagimporta_pap.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_pap"
                If IsDate(ValiditaInizio_pap.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_pap.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_pap.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_pap.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_pap.Text
                cfgImpresa.Add(aggiungi)
            End If

            'papz
            If flagimporta_papz.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_papz"
                If IsDate(ValiditaInizio_papz.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_papz.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_papz.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_papz.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_papz.Text
                cfgImpresa.Add(aggiungi)
            End If

            'notificabio
            If flagimporta_notificabio.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_notificabio"
                If IsDate(ValiditaInizio_notificabio.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_notificabio.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_notificabio.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_notificabio.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_notificabio.Text
                cfgImpresa.Add(aggiungi)
            End If

            'profilazione
            If Flagimporta_profilazione.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_profilazione"
                If IsDate(ValiditaInizio_profilazione.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_profilazione.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_profilazione.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_profilazione.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_profilazione.Text
                cfgImpresa.Add(aggiungi)
            End If

            'planning
            If flagimporta_planning.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_planning"
                If IsDate(ValiditaInizio_planning.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_planning.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_planning.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_planning.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_planning.Text
                cfgImpresa.Add(aggiungi)
            End If

            'distinta
            If flagimporta_distinta.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_distinta"
                If IsDate(ValiditaInizio_distinta.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_distinta.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_distinta.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_distinta.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_distinta.Text
                cfgImpresa.Add(aggiungi)
            End If

            'ricette
            If flagimporta_ricette.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_ricette"
                If IsDate(ValiditaInizio_ricette.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_ricette.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_ricette.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_ricette.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_ricette.Text
                cfgImpresa.Add(aggiungi)
            End If

            'pua
            If flagimporta_pua.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_pua"
                If IsDate(ValiditaInizio_pua.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_pua.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_pua.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_pua.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_pua.Text
                cfgImpresa.Add(aggiungi)
            End If

            'piano_concimazione
            If flagimporta_piano_concimazione.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_piano_concimazione"
                If IsDate(ValiditaInizio_piano_concimazione.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_piano_concimazione.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_piano_concimazione.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_piano_concimazione.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_piano_concimazione.Text
                cfgImpresa.Add(aggiungi)
            End If

            'pratiche
            If flagimporta_pratiche.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_pratiche"
                If IsDate(ValiditaInizio_pratiche.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_pratiche.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_pratiche.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_pratiche.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_pratiche.Text
                cfgImpresa.Add(aggiungi)
            End If

            'pratiche_pull
            If flagimporta_pratiche_pull.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_pratiche_pull"
                If IsDate(ValiditaInizio_pratiche_pull.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_pratiche_pull.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_pratiche_pull.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_pratiche_pull.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_pratiche_pull.Text
                cfgImpresa.Add(aggiungi)
            End If

            'LineeProduttive
            If Flagimporta_LineeProduttive.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_lineeproduttive"
                If IsDate(ValiditaInizio_LineeProduttive.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_LineeProduttive.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_LineeProduttive.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_LineeProduttive.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_LineeProduttive.Text
                cfgImpresa.Add(aggiungi)
            End If

            'piani_di_campionamento
            If Flagimporta_piani_di_campionamento.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_piani_di_campionamento"
                If IsDate(ValiditaInizio_piani_di_campionamento.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_piani_di_campionamento.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_piani_di_campionamento.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_piani_di_campionamento.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_piani_di_campionamento.Text
                cfgImpresa.Add(aggiungi)
            End If

            'analisi
            If Flagimporta_analisi.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_analisi"
                If IsDate(ValiditaInizio_analisi.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_analisi.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_analisi.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_analisi.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_analisi.Text
                cfgImpresa.Add(aggiungi)
            End If


            'allegati
            If Flagimporta_allegati.Checked Then
                Dim aggiungi As New clsDatiImpresaLista
                aggiungi.NomeElemento = "flagimporta_allegati"
                If IsDate(ValiditaInizio_allegati.Text) Then
                    aggiungi.ValiditaInizio = ValiditaInizio_allegati.Text
                Else
                    aggiungi.ValiditaInizio = AGRODATAINIZIO
                End If
                If IsDate(ValiditaFine_allegati.Text) Then
                    aggiungi.ValiditaFine = ValiditaFine_allegati.Text
                Else
                    aggiungi.ValiditaFine = AGRODATAFINE
                End If
                aggiungi.Configurazione = Configurazione_allegati.Text
                cfgImpresa.Add(aggiungi)
            End If




            'Rilettura Dati Fitrone per preservarne il contenuto iniziale
            If ddlConfigurazioni.SelectedValue > 0 Then

                Dim objG2G As New AgronicaCoreG2GLocalDal.G2GLocal_R
                Dim dtConfig As DataTable = objG2G.LeggiConfigurazioni(ddlConfigurazioni.SelectedValue, objParametri_Server)

                Dim albero As XDocument = XDocument.Parse(dtConfig.Rows(0)("G2GLocalConfigurazioni_CFG"))

                Dim objImprese As clsImportData = GIAS2GIAS_LOCALE.GIAS_2_GIAS.ListaDatiDaImportareLetturaDaXDoc(albero, True)


                'Riempimento Controlli
                filtrone.Text = objImprese.filtrone
                filtronerisultato.Text = objImprese.filtronerisultato
                filtronerisultato_azienda.Text = objImprese.filtronerisultato_azienda

            End If


            Dim G2G As New GIAS2GIAS_LOCALE.GIAS_2_GIAS
            Dim rvalDaSalvare As String = G2G.GIAS_2_GIAS_SalvaXMLxServizio(
              dirLog,
              log_G2G,
              log_Errori,
              log_Riepilogo,
              urlWS,
              ddConnessione_SERVER_ORIGINE.SelectedValue,
              ddConnessione_SERVER_DESTINAZIONE.SelectedValue,
              ddConnessione_UTENTI_ORIGINE.SelectedValue,
              ddConnessione_UTENTI_DESTINAZIONE.SelectedValue,
              txtProgressivoGIAS_ORIGINE.Text,
              txtProgressivoGIAS_DESTINAZIONE.Text,
              txtPivaSuperUser_ORIGINE.Text,
              txtPivaSuperUser_DESTINAZIONE.Text,
              txtUsernameSuperUser_Origine.Text,
              txtUsernameSuperUser_Destinazione.Text,
              txtImport_CodFiscale_ORIGINE.Text,
              txtImport_CodFiscale_DESTINAZIONE.Text,
              txtImport_Username_ORIGINE.Text,
              txtImport_Username_DESTINAZIONE.Text,
              txtImport_CodiceImpresa_ORIGINE.Text,
              txtImport_CodiceImpresa_DESTINAZIONE.Text,
              txtImport_ImpostazioniTrasfromazioni.Text,
              PercorsoConnessioni,
              cfgImpresa,
              cfgGlobali,
              filtrone.Text,
              filtronerisultato.Text,
              filtronerisultato_azienda.Text,
              hidTabella.Value
            )


            Dim salva As New AgronicaCoreG2GLocalDal.G2GLocal_W


            'Controllo Salvataggio/Modifica
            If ddlConfigurazioni.SelectedValue > 0 Then

                salva.Modifica(
                    ddlConfigurazioni.SelectedValue,
                    txtCfg.Text,
                    rvalDaSalvare,
                    "",
                    objParamentri_server
                    )

                Codice = ddlConfigurazioni.SelectedValue


            Else

                Dim agroSeq As New AgronicaCoreDataProvider.Agro_Sequenze
                Dim iG2GLocalConfigurazioni_COD As Integer
                'iG2GLocalConfigurazioni_COD = agroSeq.Agronica_SequenzaTabelle_NuovoID("G2GLocalConfigurazioni", objParamentri_server)
                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                iG2GLocalConfigurazioni_COD = agroSeq.NuovoId_Tabella("G2GLocalConfigurazioni", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)

                salva.Scrivi(
                iG2GLocalConfigurazioni_COD,
                txtCfg.Text,
                rvalDaSalvare,
                objParamentri_server
                )

                Codice = iG2GLocalConfigurazioni_COD

            End If

            'Ricarimento Configurazioni
            popolaDDConfigurazioni()

            'Rilettura
            Dim i As Integer
            For i = 0 To ddlConfigurazioni.Items.Count - 1

                If Codice = ddlConfigurazioni.Items(i).Value Then

                    ddlConfigurazioni.SelectedIndex = i
                    LeggiConfigurazioneG2G(ddlConfigurazioni.SelectedValue)
                    Exit For

                End If
            Next

        Else
            LblErr.Text = "Dati inseriti insufficienti."
            LblErr.Visible = True

        End If

    End Sub


    Private Sub genericoSelectChangeDaLocale(ByVal lIDDB As Integer, ByRef lImport_Username As TextBox,
                                     ByRef lUsernameSuperUser As TextBox, ByRef lProgressivoGIAS As TextBox, ByRef lPivaSuperUser As TextBox, ByRef lImport_CodFiscale As TextBox)

        Dim objParametri As AgronicaCoreParametri = GetOjParametriFromConnessioni(lIDDB)

        Dim dtLeggi As DataTable
        Dim leggi As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
        dtLeggi = leggi.Leggi(
            "",
            "",
            objParametri,
            RestituisciTutto:=True
            )

        If dtLeggi.Rows.Count > 0 Then
            lImport_Username.Text = dtLeggi.Rows(0)("UserName")
            lUsernameSuperUser.Text = dtLeggi.Rows(0)("UserName")
            lProgressivoGIAS.Text = dtLeggi.Rows(0)("ProgressivoGIAS")

            Dim dtLeggiSuperUser As DataTable
            Dim leggiPivaSuperUser As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
            dtLeggiSuperUser = leggiPivaSuperUser.Leggi(
                lImport_Username.Text,
                0,
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                "",
                "",
                objParametri,
                IgnoraProfilo:=True
            )

            If dtLeggiSuperUser.Rows.Count > 0 Then
                lPivaSuperUser.Text = dtLeggiSuperUser.Rows(0)("CodFisc")
                lImport_CodFiscale.Text = dtLeggiSuperUser.Rows(0)("CodFisc")
            End If

        End If
    End Sub

    Private Sub genericoSelectChangeDaWS(ByVal ID_DB As Integer, ByRef lImport_Username As TextBox,
                                         ByRef lUsernameSuperUser As TextBox, ByRef lProgressivoGIAS As TextBox,
                                         ByRef lPivaSuperUser As TextBox, ByRef lImport_CodFiscale As TextBox)

        ''saranno passati per parametro
        Dim dtLeggi, dtLeggiSuperUser As DataTable

        Dim ws As WS_Importa_GIAS_2014.ImportaWS

        Try
            ''chiamo il webservice che mi popola i dt
            '  Marco Grilli, 14/08/2014 15:00:01: Specificare SEMPRE qual'è l'URL di destinazione quando si istanza l'oggetto
            ws = New WS_Importa_GIAS_2014.ImportaWS()
            ws.Timeout = Integer.MaxValue
            ws.Url = Txb_WebService.Text
            ws.Importa_DettagliUtenteDaSuperServer(ID_DB, dtLeggi, dtLeggiSuperUser)

            If dtLeggi.Rows.Count > 0 Then
                lImport_Username.Text = dtLeggi.Rows(0)("UserName")
                lUsernameSuperUser.Text = dtLeggi.Rows(0)("UserName")
                lProgressivoGIAS.Text = dtLeggi.Rows(0)("ProgressivoGIAS")

                If dtLeggiSuperUser.Rows.Count > 0 Then
                    lPivaSuperUser.Text = dtLeggiSuperUser.Rows(0)("piva")
                    lImport_CodFiscale.Text = dtLeggiSuperUser.Rows(0)("piva")
                End If
            End If
        Catch
        End Try

    End Sub

    Private Function GetOjParametriFromConnessioni(ByVal idDB As Integer) As AgronicaCoreParametri

        Dim objParamentri_Superserver As New AgronicaCoreDataProvider.AgronicaCoreParametri
        Dim leggiStringaConnessioni As New AgronicaCoreDataProvider.Connessioni
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing

        Try
            objParamentri_Superserver = HttpContext.Current.Session("ASG_objParametri_Super_Server")

            Dim Stringa_Connessione_DB_Utenti_GIAS As String = Utility_Sicurezza.Leggi_Stringa_Connessione(idDB, objParamentri_Superserver)
            'leggiStringaConnessioni.Leggi_Stringa_Connessione(idDB, objParamentri_Superserver)

            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(
                AGRODATAINIZIO,
                AGRODATAFINE,
                AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica,
                AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati,
                "",
                "Transfert_GiacenzeDDT_daGias_aGias.txt",
                "",
                "",
                "",
                 "",
                Stringa_Connessione_DB_Utenti_GIAS
            )
        Catch
        End Try

        Return objParametri_Utenti
    End Function

    Protected Sub btnRefreshOrigine_Click(sender As Object, e As EventArgs) Handles btnRefreshOrigine.Click
        genericoSelectChangeDaLocale(ddConnessione_UTENTI_ORIGINE.SelectedValue, txtImport_Username_ORIGINE, txtUsernameSuperUser_Origine, txtProgressivoGIAS_ORIGINE, txtPivaSuperUser_ORIGINE, txtImport_CodFiscale_ORIGINE)
    End Sub

    Protected Sub btnRefreshDestinazione_Click(sender As Object, e As EventArgs) Handles btnRefreshDestinazione.Click

        genericoSelectChangeDaWS(ddConnessione_UTENTI_DESTINAZIONE.SelectedValue, txtImport_Username_DESTINAZIONE, txtUsernameSuperUser_Destinazione, txtProgressivoGIAS_DESTINAZIONE, txtPivaSuperUser_DESTINAZIONE, txtImport_CodFiscale_DESTINAZIONE)

    End Sub



    Protected Sub Passaverifiche_Click(sender As Object, e As EventArgs) Handles Passaverifiche.Click


        'panVerifiche.Visible = True

        'config.Visible = False
        'panListaImprese.Visible = False
        ' panGlobali.Visible = False
        'panSalvaCfg.Visible = False

    End Sub

    Protected Sub btnAvviaVerificheQualita_Click(sender As Object, e As EventArgs) Handles btnAvviaVerificheQualita.Click

        Dim verificatore As New AgronicaCoreG2GLocalDal.VerificheQuantitative_R

        Dim objParamentri_Superserver As New AgronicaCoreDataProvider.AgronicaCoreParametri
        objParamentri_Superserver = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim dtVerifica As DataTable
        'dtVerifica = verificatore.VerificaDB("zani_Server", "zani_destinazione_Server", "", "", objParamentri_Superserver)
        'dtVerifica = verificatore.VerificaDB("CAC_Server", "AgronicaSementi4_Server", "", "", objParamentri_Superserver)
        dtVerifica = verificatore.VerificaDB("Gias_server_matrice", "gias_metaschema", "", "", objParamentri_Superserver)


        Me.GridView_VerificheQta.DataSource = dtVerifica
        Me.GridView_VerificheQta.DataBind()


    End Sub


    Protected Sub BtnLeggiDestDaWS_Click(sender As Object, e As EventArgs) ' Handles BtnLeggiDestDaWS.Click
        '  Marco Grilli, 09/06/2014 12:10:43: Se non c'è scritto nulla, esco.
        If Txb_WebService.Text = "" Then
            Exit Sub
        End If

        Dim listaServer, listaUtenti, res As String
        Dim ws As New WS_Importa_GIAS_2014.ImportaWS()
        ws.Timeout = Integer.MaxValue

        'leggo l'elenco dei db
        Try
            ws.Url = Txb_WebService.Text
            res = ws.Importa_ListaServerUtenti(listaServer, listaUtenti)
            If (res <> "") Then
                LblErr.Text = res
                Exit Sub
            End If

            'popolo le DDBox dei server
            If (Not listaServer Is Nothing) Then
                ddConnessione_SERVER_DESTINAZIONE.Items.Add(New ListItem With {.Value = -1, .Text = "Seleziona un database"})
                For Each elem In listaServer.Split("|")
                    If elem.Contains("*") Then
                        ddConnessione_SERVER_DESTINAZIONE.Items.Add(
                            New ListItem With {.Value = elem.Split("*")(0), .Text = elem.Split("*")(1)})
                    End If
                Next
            Else
                LblErr.Text = "Nessun dato restituito dal WS come lista DB Server. "
            End If
            If (Not listaUtenti Is Nothing) Then
                ddConnessione_UTENTI_DESTINAZIONE.Items.Add(New ListItem With {.Value = -1, .Text = "Seleziona un database"})
                For Each elem In listaUtenti.Split("|")
                    If elem.Contains("*") Then
                        ddConnessione_UTENTI_DESTINAZIONE.Items.Add(
                            New ListItem With {.Value = elem.Split("*")(0), .Text = elem.Split("*")(1)})
                    End If
                Next
            Else
                LblErr.Text += "Nessun dato restituito dal WS come lista DB Utenti."
            End If
        Catch ex As Exception
            LblErr.Text = ex.Message
        End Try

    End Sub

    Private Sub svuotaControlliOrigine(svuotaDDBoxConnessioni As Boolean)
        If (svuotaDDBoxConnessioni) Then
            ddConnessione_SERVER_ORIGINE.Items.Clear()
            ddConnessione_UTENTI_ORIGINE.Items.Clear()
        End If
        txtProgressivoGIAS_ORIGINE.Text = ""
        txtPivaSuperUser_ORIGINE.Text = ""
        txtUsernameSuperUser_Origine.Text = ""
        txtImport_CodFiscale_ORIGINE.Text = ""
        txtImport_Username_ORIGINE.Text = ""
        txtImport_ImpostazioniTrasfromazioni.Text = ""
        txtImport_CodiceImpresa_ORIGINE.Text = ""
    End Sub

    Public Sub svuotaControlliDestinazione(svuotaDDBoxConnessioni As Boolean)
        If (svuotaDDBoxConnessioni) Then
            ddConnessione_SERVER_DESTINAZIONE.Items.Clear()
            ddConnessione_UTENTI_DESTINAZIONE.Items.Clear()
        End If
        txtProgressivoGIAS_DESTINAZIONE.Text = ""
        txtPivaSuperUser_DESTINAZIONE.Text = ""
        txtUsernameSuperUser_Destinazione.Text = ""
        txtImport_CodFiscale_DESTINAZIONE.Text = ""
        txtImport_Username_DESTINAZIONE.Text = ""
        txtImport_CodiceImpresa_DESTINAZIONE.Text = ""
    End Sub

    Private Sub btn_nuovo_Click(sender As Object, e As EventArgs) Handles btn_nuovo.Click

        'Reset Controlli
        svuotaControlliOrigine(False)
        svuotaControlliDestinazione(False)
        svuotacontrollitrasferimento()
        txtConfigurazione.Text = ""
        txtCfg.Text = ""
        LblErr.Visible = False
        ddlConfigurazioni.SelectedIndex = 0
        Txb_WebService.Text = ""





    End Sub
    Private Sub svuotacontrollitrasferimento()

        filtrone.Text = ""
        filtronerisultato.Text = ""
        filtronerisultato_azienda.Text = ""

        FlagImporta_Audit.Checked = False
        ValiditaInizio_Audit.Text = ""
        ValiditaFine_Audit.Text = ""
        Configurazione_Audit.Text = ""


        FlagImporta_Audit_Interviste.Checked = False
        ValiditaInizio_Audit_Interviste.Text = ""
        ValiditaFine_Audit_Interviste.Text = ""
        Configurazione_Audit_Interviste.Text = ""


        flagimporta_agenda.Checked = False
        ValiditaInizio_agenda.Text = ""
        ValiditaFine_agenda.Text = ""
        Configurazione_agenda.Text = ""


        flagimporta_pap.Checked = False
        ValiditaInizio_pap.Text = ""
        ValiditaFine_pap.Text = ""
        Configurazione_pap.Text = ""

        flagimporta_papz.Checked = False
        ValiditaInizio_papz.Text = ""
        ValiditaFine_papz.Text = ""
        Configurazione_papz.Text = ""

        flagimporta_notificabio.Checked = False
        ValiditaInizio_notificabio.Text = ""
        ValiditaFine_notificabio.Text = ""
        Configurazione_notificabio.Text = ""

        flagimporta_planning.Checked = False
        ValiditaInizio_planning.Text = ""
        ValiditaFine_planning.Text = ""
        Configurazione_planning.Text = ""

        flagimporta_distinta.Checked = False
        ValiditaInizio_distinta.Text = ""
        ValiditaFine_distinta.Text = ""
        Configurazione_distinta.Text = ""

        flagimporta_ricette.Checked = False
        ValiditaInizio_ricette.Text = ""
        ValiditaFine_ricette.Text = ""
        Configurazione_ricette.Text = ""

        Flagimporta_materieprime.Checked = False
        ValiditaInizio_materieprime.Text = ""
        ValiditaFine_materieprime.Text = ""
        Configurazione_materieprime.Text = ""

        Flagimporta_pianocolturale.Checked = False
        ValiditaInizio_pianocolturale.Text = ""
        ValiditaFine_pianocolturale.Text = ""
        Configurazione_pianocolturale.Text = ""

        Flagimporta_gis.Checked = False
        ValiditaInizio_gis.Text = ""
        ValiditaFine_gis.Text = ""
        Configurazione_gis.Text = ""

        Flagimporta_profilazione.Checked = False
        ValiditaInizio_profilazione.Text = ""
        ValiditaFine_profilazione.Text = ""
        Configurazione_profilazione.Text = ""

        Flagimporta_catasto.Checked = False
        ValiditaInizio_catasto.Text = ""
        ValiditaFine_catasto.Text = ""
        Configurazione_catasto.Text = ""

        flagimporta_pua.Checked = False
        ValiditaInizio_pua.Text = ""
        ValiditaFine_pua.Text = ""
        Configurazione_pua.Text = ""

        flagimporta_pratiche.Checked = False
        ValiditaInizio_pratiche.Text = ""
        ValiditaFine_pratiche.Text = ""
        Configurazione_pratiche.Text = ""

        flagimporta_piano_concimazione.Checked = False
        ValiditaInizio_piano_concimazione.Text = ""
        ValiditaFine_piano_concimazione.Text = ""
        Configurazione_piano_concimazione.Text = ""

        Flagimporta_LineeProduttive.Checked = False
        ValiditaInizio_LineeProduttive.Text = ""
        ValiditaFine_LineeProduttive.Text = ""
        Configurazione_LineeProduttive.Text = ""

        Flagimporta_piani_di_campionamento.Checked = False
        ValiditaInizio_piani_di_campionamento.Text = ""
        ValiditaFine_piani_di_campionamento.Text = ""
        Configurazione_piani_di_campionamento.Text = ""

        Flagimporta_allegati.Checked = False
        ValiditaInizio_allegati.Text = ""
        ValiditaFine_allegati.Text = ""
        Configurazione_allegati.Text = ""

        Flagimporta_Profilazione_Globale.Checked = False
        ValiditaInizio_Profilazione_Globale.Text = ""
        ValiditaFine_Profilazione_Globale.Text = ""
        Configurazione_Profilazione_Globale.Text = ""

        Flagimporta_Note.Checked = False
        ValiditaInizio_Note.Text = ""
        ValiditaFine_Note.Text = ""
        Configurazione_Note.Text = ""

        Flagimporta_Contatti.Checked = False
        ValiditaInizio_Contatti.Text = ""
        ValiditaFine_Contatti.Text = ""
        Configurazione_Contatti.Text = ""


        Flagimporta_Macchine.Checked = False
        ValiditaInizio_Macchine.Text = ""
        ValiditaFine_Macchine.Text = ""
        Configurazione_Macchine.Text = ""

        Flagimporta_Cac_Codifica.Checked = False
        ValiditaInizio_Cac_Codifica.Text = ""
        ValiditaFine_Cac_Codifica.Text = ""
        Configurazione_Cac_Codifica.Text = ""

        Flagimporta_analisi.Checked = False
        ValiditaInizio_analisi.Text = ""
        ValiditaFine_analisi.Text = ""
        Configurazione_analisi.Text = ""

        Flagimporta_PianiCampionamento.Checked = False
        ValiditaInizio_PianiCampionamento.Text = ""
        ValiditaFine_PianiCampionamento.Text = ""
        Configurazione_PianiCampionamento.Text = ""

        FlagAllinea_Recodes.Checked = False

    End Sub



    Protected Sub btn_XML_Click(sender As Object, e As EventArgs) Handles btn_XML.Click

        txtConfigurazione.Enabled = True

    End Sub

End Class