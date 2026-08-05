Imports System.Web.Services
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class RaccolteXConferimentiUC
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Shared Function CercaRaccolte(ByVal piva As String, ByVal dataMovimento As Date, ByVal idMovDet As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        'Dim bizMovDet As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R
        'Dim dt = bizMovDet.BIZ_RaccolteCollegabiliConferimento(piva, dataMovimento, idMovDet, objParametriServer)
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim dalMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

        Dim fineValImp = dataMovimento
        Dim inizioValImp = dataMovimento.AddDays(-31)

        Dim dt = dalMovDet.RaccolteCollegabiliConferimento(piva, inizioValImp, fineValImp, idMovDet, objParametriServer)

        Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
        r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
        r.RispostaOK = True

        Return r
    End Function

    Public Shared Function LeggiImpostazioneProponiPesoRaccolta(ByVal piva As String, ByVal saCod As Integer, ByVal vegCod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        'L'impostazione dovrà essere creata nella tabella Imprese_ImpostazioniXSpecie, e come impostazione per il superuser.
        'Va letta con le seguenti priorità:
        'Imprese_ImpostazioniXSpecie {piva, saCod, vegCod}
        'Imprese_ImpostazioniXSpecie {piva, vegCod}
        'Imprese_ImpostazioniXSpecie {piva, saCod}
        'Imprese_ImpostazioniXSpecie {piva}
        'Impostazione Super User
        'Default No
        'La gestione indicata sarà inclusa nel core, quindi indipendente da questa funzione
        Dim imp As String = "0"


        r.RispostaStringa = imp
        r.RispostaOK = True

        Return r
    End Function

End Class