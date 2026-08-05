Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ

Imports Newtonsoft.Json

Public Class OperazioneBootstrap_WS
    Inherits System.Web.UI.Page


    <WebMethod(EnableSession:=True)> _
    Public Shared Function Attivita_PopolaCombo() As RispostaStandard
        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse isNothing(objParametri_Server) Then
            r.Sessione = False
            return r
        End If


        Try

            'Inserire il codice QUI..
            Dim objCmbAttivita As new DropDownList
            AgronicaCoreUtility.CaricaListControl.Attivita(objCmbAttivita, True, "", "0", 0, "", "", objParametri_Server)
            
            

            r.RispostaOK = True
            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(objCmbAttivita.Items, "Id_attivita", "Attivita_Des")

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & _
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Turno_PopolaCombo() As RispostaStandard
        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse isNothing(objParametri_Server) Then
            r.Sessione = False
            return r
        End If


        Try

            'Inserire il codice QUI..
            Dim objCmbTurni As new DropDownList
            AgronicaCoreUtility.CaricaListControl.Turni(objCmbTurni, True, "", "0", 0, "", "", objParametri_Server)


            r.RispostaOK = True
            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(objCmbTurni.Items, "Turno_Cod", "Turno_Des")

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & _
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)> _
    Public Shared Function cmbCostiAccessori_AggiornaCostoUnitario(byval Turno_Cod As String, ByVal Attivita_Cod As String, byval DataAttivita As String  ) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..

            Dim objAttivita As New AgronicaCoreContabDAL.Attivita_R
            
            r.RispostaOK = True            
            r.RispostaStringa = Format( objAttivita.CostoOrario(Attivita_Cod, Turno_Cod, DataAttivita, objParametri_Server ), "0.00").Replace(",", ".")

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & _
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

End Class