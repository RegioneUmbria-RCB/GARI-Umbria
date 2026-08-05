Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json.Linq

Public Class IndiciXTipologie_UC
    Inherits System.Web.UI.UserControl

    '----- Gestione della pagina transazionale
    Dim EseguitaOperazione As Boolean
    Dim PremutoAnnulla As Boolean

    '----- variabili globali
    Dim Operazione As Integer
    Dim Messaggio As String = ""

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objparametri_server_string As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        inizializzoObjParametri()
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            ' Nothing


        Catch ex As Exception

            'Messaggio di errore
            Messaggio = AgronicaAgenda_2010.SiEVerificatoUnErrore & Chr(13) & ex.Message.ToString()

            'Visualizzo il messaggio di errore
            Call Messaggi.AgroMsgBox(Messaggio, Page, "Form1")
            '------------------------------------------------

        End Try

    End Sub

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub


    Private Shared Function gettimesep() As String
        Return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.TimeSeparator
    End Function


    Public Shared Function Ricerca_Alert_Indici_UC() As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim leggi As New AgronicaCoreScadenziario.Alert_Indice_R
            DT = leggi.Leggi("", 0, objParametri_Server, enumSelezioneVariabile.Selezione_TabellaDatiMinimi)


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Public Shared Function Insert_Alert_IndicexTipologia_UC(ByVal Id_Indice As Integer, ByVal ChkObbligatorio As Integer, ByVal ID_Tipologia As Integer, ByVal ID_Area As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim Scrittura_OK As New Boolean

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim Scrivi As New AgronicaCoreScadenziario.Alert_Indice_W
            Scrittura_OK = Scrivi.Scrivi_Alert_IndicexTipologie(Id_Indice, ChkObbligatorio, ID_Tipologia, ID_Area, objParametri_Server)

            If (Scrittura_OK) Then
                r.RispostaOK = True
            Else
                r.RispostaOK = False
            End If
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function LeggiIndicexTipologia_UC(ByVal ID_Tipologia As Integer, ByVal ID_Area As Integer, ByVal ID_IndiceXTipo As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim leggi As New AgronicaCoreScadenziario.Alert_Indice_R
            'Controllo se viene cliccata la riga della listbox IndiceXTipologia
            DT = leggi.LeggiIndicixTipologia("", ID_Area, ID_Tipologia, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, ID_IndiceXTipo, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function Modifica_Ordine_Alert_IndicexTipologia_UC(ByVal RigheAggiornate As String) As RispostaStandard

        Dim righeAggiornateArray As JArray = JArray.Parse(RigheAggiornate)

        Dim r As New RispostaStandard
        Dim Scrittura_OK As New Boolean

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then

            r.Sessione = False
            Return r
        End If

        Try
            Dim Scrivi As New AgronicaCoreScadenziario.Alert_Indice_W
            Scrittura_OK = Scrivi.Modifica_Ordine_Alert_IndicexTipologia(objParametri_Server.PivaSuperUser, objParametri_Server, righeAggiornateArray)
            If (Scrittura_OK) Then
                r.RispostaOK = True
            Else
                r.RispostaOK = False
            End If
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function Modifica_Obbligo_Alert_IndicexTipologia(ByVal Id_Indice As Integer, ByVal ID_Tipologia As Integer, ByVal ID_Area As Integer, ByVal Obbligatorio As Double) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Scrittura_OK As New Boolean

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then

            r.Sessione = False
            Return r
        End If

        Try
            Dim Scrivi As New AgronicaCoreScadenziario.Alert_Indice_W
            Scrittura_OK = Scrivi.Modifica_Obbligo_Alert_IndicexTipologia(objParametri_Server.PivaSuperUser, Id_Indice, ID_Tipologia, ID_Area, Obbligatorio, objParametri_Server)
            If (Scrittura_OK) Then
                r.RispostaOK = True
            Else
                r.RispostaOK = False
            End If
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function Cancella_Alert_IndicexTipologia_UC(ByVal Id_Indice As Integer, ByVal ID_Tipologia As Integer, ByVal ID_Area As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim Scrittura_OK As New Boolean

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim Scrivi As New AgronicaCoreScadenziario.Alert_Indice_W
            Scrittura_OK = Scrivi.Cancella_Alert_IndicexTipologie(objParametri_Server.PivaSuperUser, Id_Indice, ID_Tipologia, ID_Area, objParametri_Server)

            If (Scrittura_OK) Then
                r.RispostaOK = True
            Else
                r.RispostaOK = False
            End If
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function



    Public Shared Function Carica_griglia_IndiciXTipologie_UC(ByVal ID_Area As Integer, ByVal ID_Tipologia As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim leggi As New AgronicaCoreScadenziario.Alert_Indice_R
            DT = leggi.LeggiIndicixTipologiaxGriglia("", ID_Area, ID_Tipologia, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function
End Class
