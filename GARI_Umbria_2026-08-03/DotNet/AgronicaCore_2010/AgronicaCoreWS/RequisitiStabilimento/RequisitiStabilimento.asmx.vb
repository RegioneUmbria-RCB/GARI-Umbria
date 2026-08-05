Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class RequisitiStabilimento
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function readRequisitiStabilimento(
                                             ByVal InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.ReadRequisitiStabilimento.ReadRequisitiStabilimento)
                                             ) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As DataTable

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

            Dim impreseContrattiR As New Imprese_Contratti_BIZ_R

            DT = impreseContrattiR.readRequisitiStabilimento(InData.InData.risUm, InData.InData.validita.inizio, InData.InData.validita.fine, InData.InData.matCod, objParametri_Server, InData.InData.idBudget)

            Dim risp As String = JsonConvert.SerializeObject(DT, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True
            r.RispostaStringa = risp
        Catch ex As Exception
            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function saveRequisitiStabilimento(
                                             ByVal InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.SaveRequisitiStabilimento.SaveRequisitiStabilimento)
                                             ) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As DataTable

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

            Dim impreseContrattiW As New Imprese_Contratti_BIZ_W

            impreseContrattiW.saveRequisitiStabilimento(InData.InData.rows, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = ""
        Catch ex As Exception
            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function readPianoColturale(
                                      ByVal InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.ReadRequisitiStabilimento.ReadRequisitiStabilimento)
                                      ) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As DataTable

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

            Dim impreseContrattiR As New Imprese_Contratti_BIZ_R

            DT = impreseContrattiR.readPianoColturale(InData.InData.risUm, InData.InData.validita.inizio, InData.InData.validita.fine, InData.InData.matCod, objParametri_Server, InData.InData.idBudget)

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("PIVA", "PIVA", "string") With {._hidden = True})
            l.Add(New ColonneNome("rag_soc", "rag_soc", "string") With {._hidden = True})
            l.Add(New ColonneNome("cuaa", "cuaa", "string") With {._hidden = True})
            l.Add(New ColonneNome("cuaaSocio", "cuaaSocio", "string"))
            l.Add(New ColonneNome("Piva_Padre", "PivaPadre", "string"))
            l.Add(New ColonneNome("Rag_Soc_Padre", "Rag_Soc_Padre", "string"))
            l.Add(New ColonneNome("GruppoRaccolta_Cod", "GruppoRaccolta_Cod", "number"))
            l.Add(New ColonneNome("GruppoRaccolta_Des", "GruppoRaccolta_Des", "string"))
            l.Add(New ColonneNome("Veg_Cod", "Veg_Cod", "number"))
            l.Add(New ColonneNome("Veg_Des", "Veg_Des", "string"))
            l.Add(New ColonneNome("Cul_Cod", "Cul_Cod", "number"))
            l.Add(New ColonneNome("Cul_Des", "Cul_Des", "string"))
            l.Add(New ColonneNome("Mat_Cod", "Mat_Cod", "number"))
            l.Add(New ColonneNome("Mat_Des", "Mat_Des", "string"))
            l.Add(New ColonneNome("Sup", "Sup", "number"))
            l.Add(New ColonneNome("ResaPrevista", "ResaPrevista", "number"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(DT, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaOK = True
            r.RispostaStringa = risp
        Catch ex As Exception
            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function readContracts(
                                 ByVal InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.ReadRequisitiStabilimento.ReadRequisitiStabilimento)
                                 ) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As DataTable

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

            Dim impreseContrattiR As New Imprese_Contratti_BIZ_R
            Dim mostraAssegnazioni = True
            DT = impreseContrattiR.readContracts(InData.InData.risUm, InData.InData.validita.inizio, InData.InData.validita.fine, InData.InData.matCod, mostraAssegnazioni, objParametri_Server, InData.InData.idBudget)

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Piva", "PIVA", "string") With {._hidden = True})
            l.Add(New ColonneNome("rag_soc", "rag_soc", "string") With {._hidden = True})
            l.Add(New ColonneNome("Contratto_Nome", "Contratto_Nome", "string"))
            l.Add(New ColonneNome("Contratto_Cod", "Contratto_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Fase_Cod", "Fase_Cod", "number") With {._hidden = True})
            'l.Add(New ColonneNome("Cod_Risum", "Cod_Risum", "number") With {._hidden = True})
            'l.Add(New ColonneNome("Rag_Soc", "Rag_Soc", "string"))
            l.Add(New ColonneNome("Mat_Cod", "Mat_Cod", "number"))
            l.Add(New ColonneNome("Mat_Des", "Mat_Des", "string"))
            l.Add(New ColonneNome("Superficie", "Superficie", "number"))
            l.Add(New ColonneNome("QtaPrevista", "QtaPrevista", "number"))
            l.Add(New ColonneNome("ResaPrevista", "ResaPrevista", "number"))
            If mostraAssegnazioni Then
                l.Add(New ColonneNome("Sup_Assegnata", "Sup_Assegnata", "number"))
                l.Add(New ColonneNome("Qta_Assegnata", "Qta_Assegnata", "number"))
                l.Add(New ColonneNome("Percentuale_Sup_Assegnata", "Percentuale_Sup_Assegnata", "number"))
                l.Add(New ColonneNome("Percentuale_Qta_Assegnata", "Percentuale_Qta_Assegnata", "number"))
            End If
            l.Add(New ColonneNome("partitaIvaReale", "partitaIvaReale", "string") With {._hidden = True})

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(DT, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaOK = True
            r.RispostaStringa = risp
        Catch ex As Exception
            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function readDettaglioAziendale(
                                 ByVal InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.ReadDettaglioAziendale.ReadDettaglioAziendale)
                                 ) As RispostaStandard
        Dim r As New RispostaStandard
        Dim DT As DataTable

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

            Dim impreseContrattiR As New Imprese_Contratti_BIZ_R

            DT = impreseContrattiR.readDettaglioAziendale(InData.InData.piva, InData.InData.matCod, InData.InData.idBudget, InData.InData.validita.inizio, InData.InData.validita.fine, objParametri_Server)

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = JsonConvert.SerializeObject(DT)
            r.RispostaOK = True
            r.RispostaStringa = risp
        Catch ex As Exception
            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return r
    End Function

End Class