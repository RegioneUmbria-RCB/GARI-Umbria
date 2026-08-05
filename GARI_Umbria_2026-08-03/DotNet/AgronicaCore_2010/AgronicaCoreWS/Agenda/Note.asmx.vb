Imports System.ComponentModel
Imports System.Web.Services
Imports Newtonsoft.Json
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreModelsSTD.attivita.note_intervento

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Note
    Inherits System.Web.Services.WebService

    Private Sub ValorizeData(inData As Object, ByRef noteParams As LeggiNote, ByRef params As ObjParams)
        Dim datiRequest As String = JsonConvert.SerializeObject(inData)
        Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiNote))(datiRequest)
        If params Is Nothing Then
            params = New ObjParams()
        End If
        params.ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
        params.ObjParametri_Server = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
        params.ObjParametri_Utenti = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
        noteParams = objRequest.InData
    End Sub

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiNote(InData As Object) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.note_intervento.NoteIntervento))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.note_intervento.NoteIntervento))

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiNote))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_Note As LeggiNote = objRequest.InData

            Dim NoteBiz As New AgronicaControlli_2010.STD_Note
            Dim notesVisibility = 1 'show only visible notes

            If objParametri_Note.parametriAggiuntivi.Any Then
                Dim showHidden = objParametri_Note.parametriAggiuntivi.FirstOrDefault(Function(t) t.Item1 = "showHidden")
                If showHidden IsNot Nothing AndAlso showHidden.Item2 = "true" Then
                    notesVisibility = -1
                End If
            End If
            Dim noteList = NoteBiz.LeggiNote(objParametri_Note.tipoAttivita, notesVisibility, objParametri_Server)

            r.RispostaStringa = noteList
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiGruppi(InData As Object) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.note_intervento.NoteInterventoGruppi))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.note_intervento.NoteInterventoGruppi))

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiNote))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_Note As LeggiNote = objRequest.InData

            Dim NoteBiz As New AgronicaControlli_2010.STD_Note
            Dim notesVisibility = 1 'show only visible notes

            If objParametri_Note.parametriAggiuntivi.Any Then
                Dim showHidden = objParametri_Note.parametriAggiuntivi.FirstOrDefault(Function(t) t.Item1 = "showHidden")
                If showHidden IsNot Nothing AndAlso showHidden.Item2 = "true" Then
                    notesVisibility = -1
                End If
            End If

            Dim gruppiList = NoteBiz.LeggiGruppiConUtilizzo(objParametri_Note, notesVisibility, objParametri_Server)

            r.RispostaStringa = gruppiList
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function AggiornaDefaultNote(InData As Object) As rispostaStandard(Of Boolean)
        Dim r As New rispostaStandard(Of Boolean)
        Dim NoteBiz As New AgronicaControlli_2010.STD_Note
        Try
            Dim objParametri_Note As LeggiNote = Nothing
            Dim params As New ObjParams
            ValorizeData(InData, objParametri_Note, params)

            NoteBiz.ImpostaPresets(objParametri_Note, params)

            r.RispostaStringa = True
            r.RispostaOK = True
        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try
        Return r
    End Function

End Class