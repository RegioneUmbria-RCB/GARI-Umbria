Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreVarieBIZ
Imports InData.Anagrafica
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class GruppiRaccolta
    Inherits System.Web.Services.WebService

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiGruppiRaccoltaValidi(
        InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)
    ) As rispostaStandard(Of List(Of GruppoRaccolta))

        Dim r As New rispostaStandard(Of List(Of GruppoRaccolta))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objGruppiRaccolta_R As New AgronicaCoreAnagrafeDAL.Gruppi_Raccolta_Read

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim gruppiRaccolta = objGruppiRaccolta_R.LeggiGruppiRaccoltaValidi(objParametri_Server, "", "", InData.InData.Data)

            Dim list As List(Of GruppoRaccolta) = New List(Of GruppoRaccolta)

            If gruppiRaccolta.Rows.Count <> 0 Then
                For Each gr As DataRow In gruppiRaccolta.Rows
                    Dim grupporaccolta = New GruppoRaccolta(code:=CInt(gr.Item("GruppoRaccolta_Cod")), descr:=gr.Item("GruppoRaccolta_Des"))
                    grupporaccolta.validita = New IntervalloTemporale(CDate(gr.Item("Validita_Inizio")), CDate(gr.Item("Validita_Fine")))
                    list.Add(grupporaccolta)
                Next
            End If

            r.RispostaOK = True
            r.RispostaStringa = list
        Catch ex As Exception
            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiGruppiRaccolta(
        InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)
    ) As rispostaStandard(Of List(Of GruppoRaccolta))

        Dim r As New rispostaStandard(Of List(Of GruppoRaccolta))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objGruppiRaccolta_R As New AgronicaCoreAnagrafeDAL.Gruppi_Raccolta_Read

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim gruppiRaccolta = objGruppiRaccolta_R.LeggiGruppiRaccolta(objParametri_Server)

            Dim list As List(Of GruppoRaccolta) = New List(Of GruppoRaccolta)

            If gruppiRaccolta.Rows.Count <> 0 Then
                For Each gr As DataRow In gruppiRaccolta.Rows
                    Dim grupporaccolta = New GruppoRaccolta(code:=CInt(gr.Item("GruppoRaccolta_Cod")), descr:=gr.Item("GruppoRaccolta_Des"))
                    grupporaccolta.validita = New IntervalloTemporale(CDate(gr.Item("Validita_Inizio")), CDate(gr.Item("Validita_Fine")))
                    list.Add(grupporaccolta)
                Next
            End If

            r.RispostaOK = True
            r.RispostaStringa = list
        Catch ex As Exception
            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiGruppoRaccoltaImpresa(
        InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)
    ) As rispostaStandard(Of BaseCodeDescr)

        Dim r As New rispostaStandard(Of BaseCodeDescr)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objGruppiRaccolta_R As New AgronicaCoreAnagrafeDAL.Gruppi_Raccolta_Read

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim gr = objGruppiRaccolta_R.LeggiGruppoRaccoltaImpresa(InData.InData.Piva, objParametri_Server, "")

            Dim gruppoRaccolta As BaseCodeDescr = New BaseCodeDescr(0, "")

            If gr.Rows.Count <> 0 Then
                gruppoRaccolta = New BaseCodeDescr(code:=CInt(gr.Rows(0).Item("GruppoRaccolta_Cod")), descr:=gr.Rows(0).Item("GruppoRaccolta_Des"))
            End If

            r.RispostaOK = True
            r.RispostaStringa = gruppoRaccolta
        Catch ex As Exception
            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:

            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function ScriviModificaCancella_GruppoRaccolta(ByVal InData As CoreWS_Generic(Of ScriviGruppoRaccolta)) As rispostaStandard(Of GruppoRaccolta)
        Dim r As New rispostaStandard(Of GruppoRaccolta)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objGruppoRaccoltaBIZ As New AgronicaCoreAnagrafeBIZ.Gruppi_Raccolta

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objGruppoRaccolta As AgronicaCoreModelsSTD.anagrafiche.GruppoRaccolta = InData.InData.gruppoRaccolta
            Dim risp = objGruppoRaccoltaBIZ.ScriviModificaCancella_GruppiRaccolta(objGruppoRaccolta,
                                                            InData.InData.tipoOperazione,
                                                            objParametri_Server,
                                                            objParametri_Utenti
                                                            )

            r.RispostaOK = True
            r.RispostaStringa = InData.InData.gruppoRaccolta

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = InData.InData.gruppoRaccolta
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}

        End Try

        Return r
    End Function

End Class