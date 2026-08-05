Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreBiologicoBIZ
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json.Linq

Public Class Filtro_ReportBiologico
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim UtenteAbilitatoLettura As Boolean = False
    Dim UtenteAbilitatoScrittura As Boolean = False
    Public Mostra_Firma_ODC As Boolean = False
    Public Default_Data_Odierna As Boolean = False
    Public Report_Selezionato As Integer = 0

    Public objparametri_server_string, objparametri_utenti_string As String

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

    Private Sub inizializzoParametriPagina()
        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        Dim dalConfigSiti As New Configurazione_Siti_R
        hdMailTo.Value = dalConfigSiti.Leggi_Valore(6, "MailBancheDati", "", "", objParametri_Server)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        inizializzoObjParametri()
        ControlloPermessiUtente()

        If Not IsPostBack Then
            inizializzoParametriPagina()
        End If

        If Not IsNothing(Request.QueryString("r")) Then
            Report_Selezionato = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("r")),
                                           AgroKey_EncoderDecoder,
                                           Server)
        End If
        CType(MyBase.Master, StampeBootstrap).SetTitoloPagina(204)

    End Sub
     
    Private Sub ControlloPermessiUtente()

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        ' Lettura
        UtenteAbilitatoLettura = objPermessi.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                                       Session("ASG_IdServizio"),
                                                                       enum_Security_Attivita.Biologico_ReportBio,
                                                                       enum_Security_Operazione.Lettura,
                                                                       Date.Now,
                                                                       "",
                                                                       objParametri_Utenti)

        If Not UtenteAbilitatoLettura Then
            'TODO Verificare il redirect per le stampe
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
            Exit Sub
        End If
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura

        ' Scrittura
        UtenteAbilitatoScrittura = objPermessi.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                                         Session("ASG_IdServizio"),
                                                                         enum_Security_Attivita.Biologico_ReportBio,
                                                                         enum_Security_Operazione.Modifica,
                                                                         Date.Now,
                                                                         "",
                                                                         objParametri_Utenti)
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Default_Data_Odierna = Integer.Parse(objImpostazioni.LeggiConDefault(
                enum_Impostazioni_Utenti.Utente_Cod_DefaultMostraDataBio, 2,
                "0", objParametri_Utenti)) = 1

        Mostra_Firma_ODC = Integer.Parse(objImpostazioni.LeggiConDefault(
                enum_Impostazioni_Utenti.UTENTE_Cod_MostraFirmaODCBio, 2,
                "0", objParametri_Utenti)) = 1

    End Sub

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function LeggiRegioneXCentroAziendale(ByVal piva As String, ByVal sa_Cod As Integer
                                            ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Dim DT As DataTable = Nothing

        Try

            Dim obj_CentroInd As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read

            DT = obj_CentroInd.Leggi(piva, sa_Cod, 0, enum_TipiIndirizzi.SedeOperativa, enumSelezioneVariabile.Selezione_JoinCompleta,
                                     "", "", objParametriServer)

            If Not IsNothing(DT) Then
                r.RispostaOK = True
                If DT.Rows.Count > 0 Then
                    r.RispostaStringa = DT.Rows(0).Item("REG")
                Else
                    'Equivale a "regione non definita"
                    r.RispostaStringa = "000"
                End If
            End If


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function LeggiFornitori(ByVal piva As String,
                                          ByVal dataInizio As Date,
                                          ByVal dataFine As Date,
                                          ByVal centro As String,
                                          ByVal prodotti As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim filtroAggiuntivo As String = ""
            prodotti = prodotti.Replace("-", "").Replace("|", ", ")
            Dim objreportBIO As New AgronicaCoreBiologicoDAL.BIO_ReportBio
            Dim Dt = objreportBIO.leggiFornitori(piva, dataInizio, dataFine, centro, prodotti, objParametriServer)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function LeggiClassiProdotto(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Dim DT As DataTable = Nothing
        Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim lineeClassiProduzioniR As New AgronicaCoreContabDAL.Linee_Classi_Produzioni_R

            DT = lineeClassiProduzioniR.LeggiClassiProdotto(piva, 0, 0, "", "", objParametriServer)

            If Not IsNothing(DT) Then
                r.RispostaOK = True
                r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)
            End If


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function LeggiMateriePrimeReport(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try


            Dim filtro As String
            filtro = " [Materie_Prime].Regolamento = 4 " + _
                     "AND [Materie_Prime].elem_cod IN (" + CStr(SEMILAVORATI_VEGETALI) + " ," + _
                     CStr(TRASFORMATI_VEGETALI) + " ," + CStr(SEMILAVORATI_ANIMALI) + " ," + CStr(TRASFORMATI_ANIMALI) + ") "

            'elenco semilavorati e trasformati vegetali/animali, biologici e abilitati per il reg preparazioni bio
            Dim ddl As New DropDownList
            AgronicaCoreUtility.CaricaListControl.Materie_PrimeXReport(ddl, _
                                                                       False, "", "", _
                                                                       piva, _
                                                                       0, 0, 0, _
                                                                       enum_AgroReportisticaTipi.RegistriBio, _
                                                                       2, _
                                                                       filtro, _
                                                                       "", _
                                                                       objParametriServer)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                jArrayListaOp.Add(New JObject(New JProperty("Desc", i.Text), New JProperty("Value", i.Value)))
            Next

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function LeggiLineeProduzioniPreparazioni(ByVal piva As String, ByVal matCod As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            'linee e preparazioni del mat_cod selezionato
            Dim ddl As New DropDownList
            AgronicaCoreUtility.CaricaListControl.Linee_Produzioni_Preparazioni(ddl, _
                                                                                False, "", "", _
                                                                                piva, _
                                                                                enum_AgroReportistica.PreparazioniBio, _
                                                                                matCod, _
                                                                                "", "", _
                                                                                objParametriServer)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                jArrayListaOp.Add(New JObject(New JProperty("Desc", i.Text), New JProperty("Value", i.Value)))
            Next

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function LeggiCategorieMagazzinoBIO(ByVal estrazioneScelta As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim filtroAggiuntivo As String = ""

            Select Case estrazioneScelta
                Case enum_Tipo_Report_BIO.Scheda_Materie_Prime
                    Dim elemCodEsclusi = New String() {MACCHINE, ZOO_CONSISTENZA}
                    filtroAggiuntivo = "Elem_Cod NOT IN (" & Join(elemCodEsclusi, ", ") & ")"

                Case enum_Tipo_Report_BIO.Scheda_Vendite
                    Dim elemCodEsclusi = New String() {MACCHINE, ZOO_CONSISTENZA, FARMACI, MANGIMI}
                    filtroAggiuntivo = "Tabella IN ('Materie_Prime','TipologieSementi') AND Elem_Cod NOT IN (" & Join(elemCodEsclusi, ", ") & ")"
            End Select


            Dim objCategorie_Magazzino As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
            Dim Dt = objCategorie_Magazzino.Leggi(0, CAU_SCARICO, False, filtroAggiuntivo, "", objParametriServer)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function EseguiEstrazione(params As String) As rispostaStandard(Of RispostaReportBio)
        Dim r As New rispostaStandard(Of RispostaReportBio)

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim userName As String = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
            Dim prgGias As String = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))

            Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim objFiltriEstrazioni As FiltroReportBIO = JsonConvert.DeserializeObject(Of FiltroReportBIO)(params, settingLoc)
            Dim objReportBio As New AgronicaCoreBiologicoBIZ.ReportBIO

            r = objReportBio.ReportBio(objFiltriEstrazioni, userName, prgGias, objParametriServer, objParametriUtenti)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)

        End Try

        Return r


    End Function
End Class