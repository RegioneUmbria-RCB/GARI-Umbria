Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreModelsSTD.anagrafiche
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.TipiEnumerativi

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Rapporti_Contabili
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetRapportiContab(objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_server As AgronicaCoreParametri
            If objP_server = "" Then
                objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
            Else
                objParametri_server = Utility.convertStringtoOBJparametri(objP_server)
            End If


            Dim objR As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

            Dim dt As DataTable = objR.Contatti_RapportiContabili_Leggi(CostantiPersonalizzate.SACOD_CONTATTO_NONDEFINITO,
                                                                        0, False, False, False, False, False, False, False,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        " (Sa_Cod=0 OR Sa_Cod=2) ", "", objParametri_server)


            Dim lista As New List(Of String)

            For Each dr As DataRow In dt.Rows
                lista.Add("{""Cod_Rapporto"":""" & jSon.Escape(dr.Item("Cod_Rapporto")) & """, ""Rapporto_Des"":""" & dr.Item("Rapporto_Des") & """}")
            Next


            Dim strRisp As String = "[" & String.Join(",", lista.Distinct.ToList) & "]"


            r.RispostaOK = True
            r.RispostaStringa = strRisp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetRapportiContabCodDescr(InData As CoreWS_Generic(Of Integer)) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim filtro As String = ""
            Select Case InData.InData
                Case enum_RapportiContabili_SaCod.PersoneGiuridiche
                    filtro = " (Sa_Cod=0 OR Sa_Cod=1) "
                Case enum_RapportiContabili_SaCod.PersoneFisiche
                    filtro = " (Sa_Cod=0 OR Sa_Cod=2) "
            End Select
            Dim objR As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

            Dim rapportiContabili As IEnumerable(Of AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr) =
                objR.Contatti_RapportiContabili_Leggi(
                    CostantiPersonalizzate.SACOD_CONTATTO_NONDEFINITO,
                    0, False, False, False, False, False, False, False,
                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                   filtro, "", objParametri_server
                ).Select.Select(Function(row) New AgronicaCoreModelsSTD.baseClass.BaseCodeDescrStr(
                    jSon.Escape(row.Item("Cod_Rapporto")), row.Item("Rapporto_Des")
                ))

            r.RispostaStringa = JsonConvert.SerializeObject(rapportiContabili)
            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = MessaggioErrore
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LeggiDropdownContattiImprese(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of RapportoContabile))

        Dim r As New rispostaStandard(Of List(Of RapportoContabile))

        Dim rList As New List(Of RapportoContabile)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objR As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

            Dim dt As DataTable = objR.Contatti_RapportiContabili_Leggi(CostantiPersonalizzate.SACOD_CONTATTO_NONDEFINITO,
                                                                        0, False, False, False, False, False, False, False,
                                                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                        " (Sa_Cod=0 OR Sa_Cod=1) ", "", objParametri_Server)

            For Each rapCont In dt.Rows
                Dim nuovaRiga As New RapportoContabile
                nuovaRiga.codice = rapCont.Item("Cod_Rapporto")
                nuovaRiga.descrizione = rapCont.Item("Rapporto_Des")
                rList.Add(nuovaRiga)
            Next

            r.RispostaOK = True
            r.RispostaStringa = rList

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function RimuoviElementoContattiImprese_NG(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objR As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W

            objR.Cancella(InData.InData, "", objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function RimuoviElementoContattiImprese(InData As CoreWS_Generic(Of Integer)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objR As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W

            objR.Cancella(InData.InData, "", objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ModificaElementoContattiImprese(InData As CoreWS_Generic(Of RapportoContabile)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objR As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_W

            objR.Modifica(InData.InData.codice, InData.InData.consulente, InData.InData.descrizione, InData.InData.cliente, InData.InData.fornitore, InData.InData.dipendente, InData.InData.terzista, InData.InData.legale, InData.InData.agente, InData.InData.consulente, New Date(), New Date(), objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function

End Class
