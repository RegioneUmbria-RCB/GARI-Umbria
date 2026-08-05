Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreMetaSchemaDAL
Imports Newtonsoft.Json
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreUmaDal
Imports System.Web

Public Class RichiestaAllevamenti
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Shared Function CercaRichiesteAllevamenti(ByVal piva As String, ByVal richiestaCod As Integer) As RispostaStandard
        Dim r As New RispostaStandard()
        r.RispostaOK = True

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim handleAllevamenti As New UMA_Richieste_Allevamenti_R()
            Dim dtAll = handleAllevamenti.Leggi("",
                                                "",
                                                richiestaCod,
                                                "",
                                                "",
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtAll, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    Public Shared Function AggiornaRichiesteAllevamenti(ByVal piva As String, ByVal richiestaCod As Integer, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String) As RispostaStandard
        Dim r As New RispostaStandard()
        r.RispostaOK = True

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

            Dim _listaInsert = JsonConvert.DeserializeObject(Of List(Of UMA_Richieste_Allevamenti))(righeInserite, settingLoc)
            Dim _listaUpdate = JsonConvert.DeserializeObject(Of List(Of UMA_Richieste_Allevamenti))(righeModificate, settingLoc)
            Dim _listaDelete = JsonConvert.DeserializeObject(Of List(Of UMA_Richieste_Allevamenti))(righeCancellate, settingLoc)

            Dim handleUmaBiz As New AgronicaCoreUmaBiz.UMA_Richieste()
            Dim result = handleUmaBiz.AggiornaRichiesteAllevamenti(piva, richiestaCod, _listaInsert, _listaUpdate, _listaDelete, objParametri_Server)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function
    Private Shared Function GetValiditaAnno(ByVal anno As String) As ValiditaAnno

        Dim validitaAnno As New ValiditaAnno

        validitaAnno.Inizio = AGRODATAINIZIO
        validitaAnno.Fine = AGRODATAFINE

        If IsNumeric(anno) Then
            validitaAnno.Inizio = New DateTime(anno, 1, 1)
            validitaAnno.Fine = New DateTime(anno, 12, 31)
        End If

        Return validitaAnno

    End Function
    Public Shared Function ElencoConfigurazioni(Anno As String) As RispostaStandard
        Dim r As New RispostaStandard()
        r.RispostaOK = True

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If
            Dim validitaAnno = GetValiditaAnno(Anno)
            Dim handleConfigAllevamenti As New UMA_Configurazione_Allevamenti_R()
            Dim dt = handleConfigAllevamenti.Leggi("010", "", "", "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server, validitaInizio:=ValiditaAnno.Inizio,
                                            validitaFine:=ValiditaAnno.Fine)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    Public Shared Function ElencoTipi(ByVal anno As String) As RispostaStandard
        Dim r As New RispostaStandard()
        r.RispostaOK = True

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim handleTipiAllevamenti As New AgronicaCoreMetaSchemaDAL.UMA_Allevamenti_R()

            Dim validitaInizio As Date = AGRODATAINIZIO
            Dim validitaFine As Date = AGRODATAFINE
            If IsNumeric(anno) Then
                validitaInizio = New DateTime(anno, 1, 1)
                validitaFine = New DateTime(anno, 12, 31)
            End If

            Dim dt = handleTipiAllevamenti.Leggi("010",
                                                 "",
                                                 objParametri_Server,
                                                 validitaInizio,
                                                 validitaFine)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

End Class