Imports System.Net
Imports System.Text
Imports System.Web
Imports System.Web.Caching
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreScadenziario
Imports AgronicaCoreUmaBiz
Imports AgronicaCoreUmaDal
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json


Public Class ConfigurazioneUMA
    Inherits System.Web.UI.Page
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub


    <WebMethod(EnableSession:=True)>
    Protected Sub LeggiDataTabellaLavorazioni()
        Dim lavorazioniBiz As New UMA_Configurazione_MacrousixLavorazioni_BIZ

        If Not Page.IsPostBack Then
            Dim dt = lavorazioniBiz.AgronicaCoreDataProvider_Leggi(objParametri_Server)
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            inizializzoObjParametri()

            'Controllo se l'utente ha i permessi per accedere
            'TODO enum security carburanti?
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Configurazione_UMA,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

            Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Configurazione_UMA,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)


            'Pagina di origine
            hfPaginaRedirect.Value = ""
            hfPaginaRedirect_Codificata.Value = ""

            'Imposto le variabili di ponte con il client
            hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
            hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

            If Not UtenteAbilitatoLettura Then
                If String.IsNullOrWhiteSpace(CStr(hfPaginaRedirect_Codificata.Value)) Then
                    Response.Redirect("~/Menu/MenuBS_Agenda_Nuovo.aspx")
                Else
                    Response.Redirect(CStr(hfPaginaRedirect.Value) & "?p=" & Request.QueryString("p"))
                End If
            End If

        Catch ex As Exception

        End Try
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_GrigliaLavorazioniUMA(ByVal righeCancellate As String,
                                                       ByVal righeInserite As String,
                                                       ByVal righeModificate As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            ''' Salva le righe nel DB '''
            Dim biz As New UMA_Configurazione_MacrousixLavorazioni_BIZ
            SvuotaCacheUMA(HttpContext.Current.Cache)
            Return biz.Salva_GrigliaLavorazioniUMA(objParametriServer, righeInserite, righeModificate, righeCancellate)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LavorazioniAlternative_SalvaGriglia(ByVal righeCancellate As String,
                                                               ByVal righeInserite As String,
                                                               ByVal righeModificate As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            ''' Salva le righe nel DB '''
            Dim biz As New UMALavorazioniAlternative
            SvuotaCacheUMA(HttpContext.Current.Cache)
            Return biz.LavorazioniAlternative_SalvaGriglia(objParametriServer, righeInserite, righeModificate, righeCancellate)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function leggiConfigurazione() As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim ricette_operazioni_W As New AgronicaCoreContabBIZ.Ricette_Operazioni_W

        'ricette_operazioni_W.Test("02961820541", objParametri_Server, objParametri_Utenti)

        r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LavorazioniAlternative_CaricaElenco(InizioValidita As String, FineValidita As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim biz As New UMALavorazioniAlternative
            Dim UMALavorazioni As DataTable = biz.LavorazionAlternative_LeggiTabella(objParametri_Server, InizioValidita, FineValidita)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(UMALavorazioni, Formatting.None, serializerSettings)
            r.RispostaOK = True
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Setup_CaricaElenco() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim biz As New UMASetup
            Dim Setup As DataTable = biz.Setup_LeggiTabella(objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Setup, Formatting.None, serializerSettings)
            r.RispostaOK = True
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function UF_CaricaElenco(InizioValidita As String, FineValidita As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim biz As New UMA_UF
            Dim UF As DataTable = biz.UF_LeggiTabella(InizioValidita, FineValidita, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(UF, Formatting.None, serializerSettings)
            r.RispostaOK = True
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoMacrousi_Caricale(InizioValidita As String, FineValidita As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim biz As New UMA_Macrousi
            Dim UF As DataTable = biz.Macrousi_LeggiTabella(InizioValidita, FineValidita, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(UF, Formatting.None, serializerSettings)
            r.RispostaOK = True
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoLavorazioni_Caricale(InizioValidita As String, FineValidita As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim biz As New UMA_Lavorazioni
            Dim UF As DataTable = biz.Lavorazioni_LeggiTabella(InizioValidita, FineValidita, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(UF, Formatting.None, serializerSettings)
            r.RispostaOK = True
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoAllevamenti_Caricale(InizioValidita As String, FineValidita As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim biz As New UMA_Allevamenti
            Dim UF As DataTable = biz.Allevamenti_LeggiTabella(InizioValidita, FineValidita, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(UF, Formatting.None, serializerSettings)
            r.RispostaOK = True
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoAssociazioniMacrousi_Caricale(InizioValidita As String, FineValidita As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim biz As New UMA_Macrousi
            Dim UF As DataTable = biz.AssociazioniMacrousi_LeggiTabella(InizioValidita, FineValidita, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(UF, Formatting.None, serializerSettings)
            r.RispostaOK = True
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function UF_CaricaElencoAgea(colonna As Integer, mostraDescrizioniVuote As Boolean) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim dt As DataTable

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim ufdal As New UMA_UF_Colture_R

            Select Case colonna
                Case 1
                    dt = ufdal.leggiOccupazione(objParametri_Server, mostraDescrizioniVuote)
                Case 2
                    dt = ufdal.leggiDestinazione(objParametri_Server, mostraDescrizioniVuote)
                Case 3
                    dt = ufdal.leggiUso(objParametri_Server, mostraDescrizioniVuote)
                Case 4
                    dt = ufdal.leggiQualita(objParametri_Server, mostraDescrizioniVuote)
                Case Else
                    dt = Nothing
            End Select

            Dim x = dt.NewRow
            x.Item(0) = " "
            x.Item(1) = "NON SELEZIONATO"
            dt.Rows.InsertAt(x, 0)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            r.RispostaOK = True
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function DateRendicontazione_CaricaElenco() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim biz As New UMAConfigurazioneDateRendicontazioni
            Dim Setup As DataTable = biz.DateRendicontazioni_LeggiTabella(objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Setup, Formatting.None, serializerSettings)
            r.RispostaOK = True
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaConfigurazioniUMA(InizioValidita As String, FineValidita As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim biz As New UMA_Configurazione_MacrousixLavorazioni_BIZ
        Dim UMALavorazioni = biz.AgronicaCoreDataProvider_Leggi(objParametri_Server, InizioValidita, FineValidita)

        Dim Dt As New DataTable
        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dt.Columns.Add(New DataColumn("Operazioni_LavDeS", GetType(String)))
        Dt.Columns.Add(New DataColumn("UMAMacrousi_MacrousoUMADes", GetType(String)))
        Dt.Columns.Add(New DataColumn("UMALavorazioni_LavUmaDes", GetType(String)))
        Dt.Columns.Add(New DataColumn("Attivita_Desc", GetType(String)))

        Dt.Columns.Add(New DataColumn("RegioneCod", GetType(String)))
        Dt.Columns.Add(New DataColumn("MacrousoUMACod", GetType(String)))
        Dt.Columns.Add(New DataColumn("LavUMACod", GetType(String)))
        Dt.Columns.Add(New DataColumn("LavCod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("IdAttivita", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("TipoOperazioneCod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("TipoOperazioneDes", GetType(String)))
        Dt.Columns.Add(New DataColumn("GasolioLt", GetType(Double)))
        Dt.Columns.Add(New DataColumn("BenzinaLt", GetType(Double)))
        Dt.Columns.Add(New DataColumn("Ordinamento", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("NMaxOperazioni", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Default", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Inviato", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("DataInvio", GetType(DateTime)))
        Dt.Columns.Add(New DataColumn("DataCreazione", GetType(DateTime)))
        Dt.Columns.Add(New DataColumn("DataModifica", GetType(DateTime)))
        Dt.Columns.Add(New DataColumn("UsernameCreazione", GetType(String)))
        Dt.Columns.Add(New DataColumn("UsernameModifica", GetType(String)))
        Dt.Columns.Add(New DataColumn("ValiditaInizio", GetType(DateTime)))
        Dt.Columns.Add(New DataColumn("ValiditaFine", GetType(DateTime)))
        Dt.Columns.Add(New DataColumn("UDMAlternativa", GetType(String)))
        Dt.Columns.Add(New DataColumn("GasolinoLTxBiologico", GetType(Double)))
        Dt.Columns.Add(New DataColumn("BenzinaLTxBiologico", GetType(Double)))
        Dt.Columns.Add(New DataColumn("LimiteMax", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("MaxxHa", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("ID", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Modificabile", GetType(Boolean)))
        Dt.Columns.Add(New DataColumn("Regolamento_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Regolamento_CodDes", GetType(String)))
        Dt.Columns.Add(New DataColumn("Coefficiente_Distribuzione_Acqua", GetType(Double)))
        Dt.Columns.Add(New DataColumn("FlagNoteCompObbl", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("FlagNoteCompObblDes", GetType(String)))

        For Each row As DataRow In UMALavorazioni.Rows
            Dim d = Dt.NewRow
            d("Operazioni_LavDeS") = row.Item("LAV_DES")
            d("UMAMacrousi_MacrousoUMADes") = row.Item("Macrouso_UMA_Des")
            d("UMALavorazioni_LavUmaDes") = row.Item("Lav_Uma_Des")
            d("Attivita_Desc") = row.Item("Desc")

            d("RegioneCod") = row.Item("Regione_Cod")
            d("MacrousoUMACod") = row.Item("Macrouso_UMA_Cod")
            d("LavUMACod") = row.Item("Lav_UMA_Cod")
            d("LavCod") = row.Item("Lav_Cod")
            d("IdAttivita") = row.Item("Id_Attivita")
            Dim tipoOpCod As Integer = row.Item("Tipo_Operazione")
            Dim tipoOpDes As String
            If tipoOpCod = 1 Then
                tipoOpDes = "Ordinaria"
            Else
                tipoOpDes = "Straordinaria"
            End If

            d("TipoOperazioneDes") = tipoOpDes
            d("TipoOperazioneCod") = tipoOpCod
            d("GasolioLt") = row.Item("Gasolio_Lt")
            d("BenzinaLt") = row.Item("Benzina_Lt")
            d("Ordinamento") = row.Item("Ordinamento")
            d("NMaxOperazioni") = row.Item("N_Max_Operazioni")
            d("Default") = row.Item("Default")
            d("Inviato") = row.Item("inviato")
            d("DataInvio") = row.Item("datainvio")
            d("DataCreazione") = row.Item("Data_Creazione")
            d("DataModifica") = row.Item("Data_Modifica")
            d("UsernameCreazione") = row.Item("Username_Creazione")
            d("UsernameModifica") = row.Item("Username_Modifica")
            d("ValiditaInizio") = row.Item("Validita_Inizio")
            d("ValiditaFine") = row.Item("Validita_Fine")
            d("UDMAlternativa") = row.Item("Udm_Alternativa")
            d("GasolinoLTxBiologico") = row.Item("Gasolio_LtxBiologico")
            d("BenzinaLTxBiologico") = row.Item("Benzina_LtxBiologico")
            d("LimiteMax") = row.Item("Limite_Max")
            d("MaxxHa") = row.Item("Max_xHa")
            d("ID") = row.Item("ID")
            d("Regolamento_Cod") = row.Item("Regolamento_Cod")
            Dim tipoRegolamento_Cod As Integer = row.Item("Regolamento_Cod")
            Dim Regolamento_CodDes As String
            If tipoRegolamento_Cod = 1 Then
                Regolamento_CodDes = "Convenzionale"
            ElseIf tipoRegolamento_Cod = 4 Then
                Regolamento_CodDes = "Biologico"
            Else
                Regolamento_CodDes = "Entrambi"
            End If
            d("Regolamento_CodDes") = Regolamento_CodDes
            d("Modificabile") = False
            d("Coefficiente_Distribuzione_Acqua") = If(IsDBNull(row.Item("Coeff_acq_distr")), 0, row.Item("Coeff_acq_distr"))
            d("FlagNoteCompObbl") = If(IsDBNull(row.Item("FlagNoteCompObbl")), 0, row.Item("FlagNoteCompObbl"))
            d("FlagNoteCompObblDes") = If(IsDBNull(row.Item("FlagNoteCompObbl")), "NO", If(row.Item("FlagNoteCompObbl") = 1, "SI", "NO"))

            Dt.Rows.Add(d)
        Next
        r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LavorazioniAlternative_CaricaDropdown(elencoRichiesto As String) As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As New DataTable
        Try
            Dim biz As New UMALavorazioniAlternative
            r.RispostaOK = True ' Assumiamo argomento valido

            Select Case elencoRichiesto
                Case "Macrouso_UMA_Des"
                    dt = biz.Dropdown_Gruppo_Colturale_UMA(objParametri_Server)
                Case "LavUMA_Lav_UMA_Des"
                    dt = biz.Dropdown_LavUMA_Lav_UMA_Des(objParametri_Server)
                Case "LavUMAAlt_Lav_UMA_Des"
                    dt = biz.Dropdown_LavUMAAlt_Lav_UMA_Des(objParametri_Server)
                Case Else
                    r.RispostaOK = False
                    r.Errore = "Argomente non valido: " & elencoRichiesto
            End Select

            ' ****************************************** tutto ok, risposta ******************************************
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LavorazioniAlternative_LeggiElencoDropdown(elencoRichiesto As String) As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As New DataTable
        Try
            Dim biz As New UMALavorazioniAlternative
            r.RispostaOK = True ' Assumiamo argomento valido

            Select Case elencoRichiesto
                Case "Gruppo_Colturale_UMA"
                    dt = biz.Dropdown_Gruppo_Colturale_UMA(objParametri_Server)
                Case "Lavorazione_UMA"
                    dt = biz.Dropdown_LavUMA_Lav_UMA_Des(objParametri_Server)
                Case "Lavorazione_UMA_Alt"
                    dt = biz.Dropdown_LavUMAAlt_Lav_UMA_Des(objParametri_Server)
                Case Else
                    r.RispostaOK = False
                    r.Errore = "Argomente non valido: " & elencoRichiesto
            End Select
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try

        ' ****************************************** tutto ok, risposta ******************************************
        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiElenco_MacrousoUMACod(elencoRichiesto As String) As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As New DataTable
        Try
            Dim biz As New UMA_Configurazione_MacrousixLavorazioni_BIZ
            r.RispostaOK = True ' Assumiamo argomento valido

            Select Case elencoRichiesto
                Case "uma_macrousi"
                    dt = biz.LeggiElenco_UMA_Macrousi(objParametri_Server)
                Case "uma_lavorazioni"
                    dt = biz.LeggiElenco_UMA_Lavorazioni(objParametri_Server)
                Case "operazioni"
                    dt = biz.LeggiElenco_Operazioni(objParametri_Server)
                Case "attivita"
                    dt = biz.LeggiElenco_Attivita(objParametri_Server)
                Case Else
                    r.RispostaOK = False
                    r.Errore = "Argomente non valido: " & elencoRichiesto
            End Select
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try

        ' ****************************************** tutto ok, risposta ******************************************
        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Protected Sub Setup_LeggiTabella()
        Dim setup As New UMASetup

        If Not Page.IsPostBack Then
            Dim dt = setup.Setup_LeggiTabella(objParametri_Server)
        End If
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function Setup_SalvaGriglia(ByVal righeCancellate As String,
                                                               ByVal righeInserite As String,
                                                               ByVal righeModificate As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            ''' Salva le righe nel DB '''
            Dim biz As New UMASetup
            SvuotaCacheUMA(HttpContext.Current.Cache)
            Return biz.Setup_SalvaGriglia(objParametriServer, righeInserite, righeModificate, righeCancellate)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function UF_SalvaGriglia(ByVal righeCancellate As String,
                                           ByVal righeInserite As String,
                                           ByVal righeModificate As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            ''' Salva le righe nel DB '''
            Dim biz As New UMA_UF
            SvuotaCacheUMA(HttpContext.Current.Cache)
            Return biz.UF_SalvaGriglia(objParametriServer, righeInserite, righeModificate, righeCancellate)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoMacrousi_SalvaGriglia(ByVal righeCancellate As String,
                                           ByVal righeInserite As String,
                                           ByVal righeModificate As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            ''' Salva le righe nel DB '''
            Dim biz As New UMA_Macrousi
            SvuotaCacheUMA(HttpContext.Current.Cache)
            Return biz.Macrousi_SalvaGriglia(objParametriServer, righeInserite, righeModificate, righeCancellate)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoLavorazioni_SalvaGriglia(ByVal righeCancellate As String,
                                           ByVal righeInserite As String,
                                           ByVal righeModificate As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            ''' Salva le righe nel DB '''
            Dim biz As New UMA_Lavorazioni
            SvuotaCacheUMA(HttpContext.Current.Cache)
            Return biz.Lavorazioni_SalvaGriglia(objParametriServer, righeInserite, righeModificate, righeCancellate)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoAllevamenti_SalvaGriglia(ByVal righeCancellate As String,
                                           ByVal righeInserite As String,
                                           ByVal righeModificate As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            ''' Salva le righe nel DB '''
            Dim biz As New UMA_Allevamenti
            SvuotaCacheUMA(HttpContext.Current.Cache)
            Return biz.Allevamenti_SalvaGriglia(objParametriServer, righeInserite, righeModificate, righeCancellate)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoAssociazioniMacrousi_SalvaGriglia(ByVal righeCancellate As String,
                                           ByVal righeInserite As String,
                                           ByVal righeModificate As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Flag_Connessione, Flag_Transazione As Boolean

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            AgronicaCoreDataProvider.Utility.VerificaApriTransazione(objParametriServer,
                                         Flag_Connessione,
                                         Flag_Transazione)

            ''' Salva le righe nel DB '''
            Dim modifiche = JsonConvert.DeserializeObject(Of List(Of AgronicaCoreModello.modelMacrousiUMA))(righeModificate)
            Dim LeggiConfSiti As New Configurazione_Siti_R
            Dim urlWS As String = LeggiConfSiti.Leggi_Valore(6, "GiasOnline_WS_UMA_AgroWS_Codifica_Specie_Vegetali_Agea_2015_2020", "", "", objParametriServer)

            Dim utility As New AgronicaCoreUtility.Http
            Dim payload = "{ jsonDati : """ & JsonConvert.SerializeObject(modifiche).Replace("'", "@&").Replace("""", "'") & """} "

            Dim response As String = utility.RestPostBasicAuth($"{urlWS}/AggiornaMacrousiUMA", payload, String.Empty, String.Empty, "d", True)
            Dim risposta As RispostaStandard = JsonConvert.DeserializeObject(Of RispostaStandard)(response)

            If risposta.RispostaOK Then
                Dim rCapCli = New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_W()
                For Each item As AgronicaCoreModello.modelMacrousiUMA In modifiche
                    Dim objRispIns = rCapCli.AggiornaMacrousiUMA(objParametriServer,
                                                                 item.Cul_Cod_Agea,
                                                                 item.Uso_Cod,
                                                                 item.Macrouso_Cod,
                                                                 item.Occupazione_Cod,
                                                                 item.Destinazione_Cod,
                                                                 item.Qualita_Cod,
                                                                 item.Macrouso_UMA_Cod,
                                                                 item.Macrouso_UMA_Des)

                    If (Not objRispIns) Then
                        Dim messaggio As New StringBuilder($"Aggiornamento fallito del record: ")
                        messaggio.AppendLine($"Cul_Cod_Agea = {item.Cul_Cod_Agea}, ")
                        messaggio.AppendLine($"Uso_Cod = {item.Uso_Cod}, ")
                        messaggio.AppendLine($"Macrouso_Cod = {item.Macrouso_Cod}, ")
                        messaggio.AppendLine($"Occupazione_Cod = {item.Occupazione_Cod}, ")
                        messaggio.AppendLine($"Destinazione_Cod = {item.Destinazione_Cod} ")
                        messaggio.AppendLine($"Nuovi valori: Macrouso_UMA_Cod = {item.Macrouso_UMA_Cod}, ")
                        messaggio.AppendLine($"Macrouso_UMA_Des = {item.Macrouso_UMA_Des}")

                        Throw New Exception(messaggio.ToString())
                    End If
                Next

                AgronicaCoreDataProvider.Utility.VerificaChiudiTransazione(objParametriServer,
                                            Flag_Transazione)

                r.RispostaOK = True
                r.RispostaStringa = "L'operazione è stata completata con successo."

            Else
                Throw New Exception(risposta.RispostaStringa)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametriServer,
                                               Flag_Transazione)
            r.RispostaOK = False
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally
            Utility.VerificaChiudiConnessione(objParametriServer,
                                              Flag_Connessione)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Setup_LeggiTipologieReport() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim dt As DataTable

        Try

            Dim tipologie As New Alert_Tipologia_R

            r.RispostaOK = True

            dt = tipologie.Leggi(enum_ID_Area_Alert.UMA_Carburanti, 0, "", False, objParametri_Server)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function DateRendicontazioni_SalvaGriglia(ByVal righeCancellate As String,
                                                               ByVal righeInserite As String,
                                                               ByVal righeModificate As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            ''' Salva le righe nel DB '''
            Dim biz As New UMAConfigurazioneDateRendicontazioni
            SvuotaCacheUMA(HttpContext.Current.Cache)
            Return biz.SetupDateRendicontazione_SalvaGriglia(objParametriServer, righeInserite, righeModificate, righeCancellate)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    Public Shared Sub SvuotaCacheUMA(cache As Cache)

        If cache IsNot Nothing Then

            Dim keysToClear = (From dict As System.Collections.DictionaryEntry In cache
                               Let key = dict.Key.ToString()
                               Where key.Contains("UMA_")
                               Select key).ToList()


            For Each key In keysToClear
                cache.Remove(key)
            Next

        End If

    End Sub




    '*******************************************UMAConfigurazioneAllevamenti******************************************'

    <WebMethod(EnableSession:=True)>
    Public Shared Function UMAConfigurazioneAllevamenti_SalvaGriglia(ByVal righeCancellate As String,
                                                                     ByVal righeInserite As String,
                                                                     ByVal righeModificate As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            ''' Salva le righe nel DB '''
            Dim biz As New AgronicaCoreUmaBiz.UMAConfigurazioneAllevamenti
            Return biz.UMAConfigurazioneAllevamenti_SalvaGriglia(objParametriServer, righeInserite, righeModificate, righeCancellate)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Protected Sub LeggiDataUMAConfigurazioneAllevamenti()
        Dim configurazioneAllevamentiBiz As New AgronicaCoreUmaBiz.UMAConfigurazioneAllevamenti


        If Not Page.IsPostBack Then
            Dim dt = configurazioneAllevamentiBiz.AgronicaCoreDataProvider_Leggi(objParametri_Server)
        End If
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function leggiConfigurazioneAllevamenti() As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim ricette_operazioni_W As New AgronicaCoreContabBIZ.Ricette_Operazioni_W

        'ricette_operazioni_W.Test("02961820541", objParametri_Server, objParametri_Utenti)

        r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function UMAConfigurazioneAllevamenti_CaricaElenco(InizioValidita As String, FineValidita As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim biz As New UMAConfigurazioneAllevamenti
            Dim configurazioneAllevamenti As DataTable = biz.UMAConfigurazioneAllevamenti_LeggiTabella(objParametri_Server, InizioValidita, FineValidita)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(configurazioneAllevamenti, Formatting.None, serializerSettings)
            r.RispostaOK = True
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    Public Shared Function CaricaConfigurazioneAllevamentiUMA() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim biz As New AgronicaCoreUmaBiz.UMAConfigurazioneAllevamenti
        Dim dal As New AgronicaCoreUmaDal.UMAConfigurazioneAllevamenti_R

        Dim configurazioneAllevamenti = dal.LeggiUMAConfigurazioneAllevamenti(objParametri_Server)

        Dim Dt As New DataTable
        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore



        Dim x As New List(Of Object)

        Dim configurazioniAllevamentiUMASD = (From row In configurazioneAllevamenti.AsEnumerable() Select New With
                                                         {
                                                           .Reione_Cod = CInt(row.Item("Reione_Cod")),
                                                           .UMA_All_Cod = CInt(row.Item("Id_Schema_Template")),
                                                           .Tipo_Operazione = If(row.Item("Tipo_Operazione") Is DBNull.Value, 0, CInt(row.Item("Tipo_Operazione"))),
                                                           .Gasolio_Lt = If(row.Item("Gasolio_Lt") Is DBNull.Value, 0, CInt(row.Item("Gasolio_Lt"))),
                                                           .Benzina_Lt = If(row.Item("Benzina_Lt") Is DBNull.Value, 0, CInt(row.Item("Benzina_Lt"))),
                                                           .Qta_Aggiuntiva_Carro = If(row.Item("Qta_Aggiuntiva_Carro") Is DBNull.Value, 0, CInt(row.Item("Qta_Aggiuntiva_Carro"))),
                                                           .N_Max_Allevamenti = If(row.Item("N_Max_Allevamenti") Is DBNull.Value, 0, CInt(row.Item("N_Max_Allevamenti"))),
                                                           .inviato = If(row.Item("inviato") Is DBNull.Value, 0, CInt(row.Item("inviato"))),
                                                           .datainvio = If(row.Item("datainvio") Is DBNull.Value, AGRODATAINIZIO, CDate(row.Item("datainvio"))),
                                                           .Data_Creazione = If(row.Item("Data_Creazione") Is DBNull.Value, DateTime.Now, CDate(row.Item("Data_Creazione"))),
                                                           .Data_Modifica = If(row.Item("Data_Creazione") Is DBNull.Value, DateTime.Now, CDate(row.Item("Data_Creazione"))),
                                                           .Username_Creazione = If(row.Item("Username_Creazione") Is DBNull.Value, objParametri_Server.UtenteUsername, row.Item("Username_Creazione")),
                                                           .Username_modifica = If(row.Item("Username_Modifica") Is DBNull.Value, objParametri_Server.UtenteUsername, row.Item("Username_Modifica")),
                                                           .Validita_Inizio = If(row.Item("Validita_Inizio") Is DBNull.Value, AGRODATAINIZIO, CDate(row.Item("Validita_Inizio"))),
                                                           .Validita_Fine = If(row.Item("Validita_Fine") Is DBNull.Value, AGRODATAFINE, CDate(row.Item("Validita_Fine")))
                                                         }).ToList()



        r.RispostaStringa = JsonConvert.SerializeObject(configurazioniAllevamentiUMASD, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function UMAConfigurazioneAllevamenti_CaricaDropdown(elencoRichiesto As String) As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As New DataTable
        Try
            Dim biz As New UMAConfigurazioneAllevamenti
            r.RispostaOK = True ' Assumiamo argomento valido

            Select Case elencoRichiesto
                Case "Operazione"
                    dt = biz.Dropdown_Operazione(objParametri_Server)
                    'Case "UMA_All_Des"
                    '    dt = biz.ElencoAllevamenti()
                Case Else
                    r.RispostaOK = False
                    r.Errore = "Argomente non valido: " & elencoRichiesto
            End Select

            ' ****************************************** tutto ok, risposta ******************************************
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoAllevamenti() As RispostaStandard
        Return UMAConfigurazioneAllevamenti.ElencoAllevamenti()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function getElencoTipoMacchine() As RispostaStandard
        Dim r As New RispostaStandard
        Dim temp As Integer
        Dim cont As Integer
        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As DataTable
        Try
            Dim macchine As New AgronicaCoreMetaSchemaDAL.Macchine_R
            r.RispostaOK = True ' Assumiamo argomento valido

            dt = macchine.Leggi("", objParametri_Server)

            Dim dataView = New DataView(dt) With {
                .Sort = "CLASS_CODE ASC"
            }
            dt = dataView.ToTable(False, "CLASS_CODE", "CLASS_DESC")

            For Each row As DataRow In dt.Rows
                temp = CStr(row.Item("CLASS_CODE")).Split(".").Count
                Do While cont < temp - 1
                    row.Item("CLASS_DESC") = " . " + CStr(row.Item("CLASS_DESC"))
                    cont += 1
                Loop
                cont = 0
                temp = 0
            Next

            dt.Columns("CLASS_CODE").ColumnName = "Macchine_Targa_Obbligatoria"
            dt.Columns("CLASS_DESC").ColumnName = "Macchine_Targa_ObbligatoriaDes"

            ' ****************************************** tutto ok, risposta ******************************************
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function getElencoStati() As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As DataTable
        Try
            Dim praticheStati As New AgronicaCoreProfilazioneDAL.Pratiche_R
            r.RispostaOK = True ' Assumiamo argomento valido

            dt = praticheStati.Leggi_StatiServizio("2007", objParametri_Server)

            dt.Columns("WAnagraficaStati_Cod").ColumnName = "Stati_Invio_Mail"
            dt.Columns("WAnagraficaStati_Des").ColumnName = "Stati_Invio_MailDes"

            ' ****************************************** tutto ok, risposta ******************************************
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function getElencoGruppiUtente() As RispostaStandard
        Dim r As New RispostaStandard
        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As DataTable
        Try
            Dim gruppi As New AgronicaCoreUtentiDAL.Gruppi_Utente_R
            r.RispostaOK = True ' Assumiamo argomento valido

            dt = gruppi.Leggi(0, "", "", objParametri_Utenti)

            Dim dataView = New DataView(dt) With {
             .Sort = "Gruppi_Utente_cod ASC"
            }

            dt = dataView.ToTable(False, "Gruppi_Utente_cod", "Gruppi_Utente_des")

            dt.Columns("Gruppi_Utente_cod").ColumnName = "Gruppi_Utenti_Invio_Mail"
            dt.Columns("Gruppi_Utente_des").ColumnName = "Gruppi_Utenti_Invio_MailDes"

            dt = dataView.ToTable(False, "Gruppi_Utente_cod", "Gruppi_Utente_des")

            dt.Columns("Gruppi_Utente_cod").ColumnName = "Gruppi_Utenti_Invio_Mail"
            dt.Columns("Gruppi_Utente_des").ColumnName = "Gruppi_Utenti_Invio_MailDes"

            ' ****************************************** tutto ok, risposta ******************************************
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function


    <WebMethod(EnableSession:=True)>
    Protected Sub UMAConfigurazioneAllevamenti_LeggiTabella()
        Dim configurazioneAllevamenti As New UMAConfigurazioneAllevamenti

        If Not Page.IsPostBack Then
            Dim dt = configurazioneAllevamenti.UMAConfigurazioneAllevamenti_LeggiTabella(objParametri_Server)
        End If
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiDataTabellaGruppiAllevamento() As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As DataTable
        Try
            Dim gruppiAllevamento As New UMA_Allevamenti_R
            r.RispostaOK = True ' Assumiamo argomento valido

            dt = gruppiAllevamento.Gruppi_Allevamento_Leggi(objParametri_Server)

            ' ****************************************** tutto ok, risposta ******************************************
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try






    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiDataTabellaMacrousiUMA() As RispostaStandard
        Dim r As New RispostaStandard

        ' ***************************************** Verifiche *****************************************
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        ' ****************************************** Carica l'elenco ******************************************
        Dim dt As DataTable
        Try
            Dim macrousi As New UMA_Macrousi_R
            r.RispostaOK = True ' Assumiamo argomento valido

            dt = macrousi.Elenco(objParametri_Server)

            ' ****************************************** tutto ok, risposta ******************************************
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try

    End Function











    '*****************************************UMAConfigurazioniAlllevamenti********************************************'








    Enum Tipo_Operazione
        Ordinaria = 1
        Straordinaria = 2
    End Enum

End Class