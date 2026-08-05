
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeBIZ
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreContabDAL
Imports AgronicaCoreEFatturaDAL
Imports AgGateway.ADAPT.ApplicationDataModel.Documents
Imports AgronicaCoreEntityFramework_POCO

Public Class AssegnazioneLottoConferimentoUC
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Friend Shared Function LeggiTipologieConferimento(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim leggiAnagrafeLog As New OGenerazioni_Anagrafe_Moduli_Log_R
            Dim dtAnagrafeLog = leggiAnagrafeLog.Leggi(piva, 0, 0, 0, 0, -1, -1, -1, AGRODATAINIZIO, AGRODATAFINE,
                                                   "(Modulo_Generazione = 2 OR Modulo_Generazione = 5)", "",
                                                   objParametri_Server)

            DT.Columns.Add("Modulo_Cod", GetType(Integer))
            DT.Columns.Add("Modulo_Des", GetType(String))

            If Not IsNothing(dtAnagrafeLog) Then
                ModuloAttivo(dtAnagrafeLog, enum_Omni_Modulo_Generazione.FreshFood, "Trasformati Vegetali", DT)
                ModuloAttivo(dtAnagrafeLog, enum_Omni_Modulo_Generazione.Zoo, "Trasformati Animali", DT)
            End If

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

    Friend Shared Function LeggiConfigurazioneModulo(piva As String, generazioneModulo As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim configurazione As New Dictionary(Of String, Object)
            Dim leggiAnagrafeLog As New OGenerazioni_Anagrafe_Moduli_Log_R
            Dim dtAnagrafeLog = leggiAnagrafeLog.Leggi(piva, 0, generazioneModulo, 0, 0, -1, -1, -1, AGRODATAINIZIO, AGRODATAFINE,
                                                   "", "",
                                                   objParametri_Server)

            If Not IsNothing(dtAnagrafeLog) AndAlso dtAnagrafeLog.Rows.Count = 1 Then
                configurazione.Add("separatore", dtAnagrafeLog.Rows(0)("Separatore_Lotto"))
                configurazione.Add("parametriScelti", dtAnagrafeLog.Rows(0)("Lotto_Configurazione"))
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(configurazione, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Friend Shared Function SalvaConfigurazioneLotto(ByVal piva As String, generazioneModulo As Integer, separatore As String,
                                                    parametriScelti As Integer()) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim parametri As String = String.Empty
            If parametriScelti IsNot Nothing AndAlso parametriScelti.Any() Then
                parametri = $"{String.Join("|", parametriScelti)}|"
            End If
            Dim scriviAnagrafeLog As New OGenerazioni_Anagrafe_Moduli_Log_W
            Dim risp As Boolean = scriviAnagrafeLog.Scrivi(piva, 0, generazioneModulo, 0, 0, -1, -1, -1,
                                                           AGRODATAINIZIO, AGRODATAFINE, -1, -1, -1, -1, -1,
                                                           parametri, "", -1, separatore, -1, -1, -1, -1,
                                                           objParametri_Server)

            If Not risp Then
                Throw New Exception("Salvataggio della configurazioen in errore")
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Private Shared Sub ModuloAttivo(dtAnagrafeLog As DataTable, codiceModuloGenerazione As enum_Omni_Modulo_Generazione,
                                    descrizione As String, DT As DataTable)
        Dim result As Object() = Nothing

        Dim attivo As Boolean = dtAnagrafeLog.AsEnumerable() _
            .Any(Function(x) IsNumeric(x.Item("Modulo_Generazione")) AndAlso
                x.Item("Modulo_Generazione") = CInt(codiceModuloGenerazione))

        If attivo Then
            DT.Rows.Add(CInt(codiceModuloGenerazione), descrizione)
        End If

    End Sub

End Class