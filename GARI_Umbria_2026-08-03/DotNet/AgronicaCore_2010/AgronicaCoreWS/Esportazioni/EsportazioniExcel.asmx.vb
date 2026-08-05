Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreGestioneRichieste

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class EsportazioniExcel
    Inherits System.Web.Services.WebService

    <WebMethod()> <Script.Services.ScriptMethod()> _
    Public Function HelloWorld(ByVal objParametri As String) As String
        Dim oS As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParametri)
        Return "Hello World"
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()> _
    Public Function EsportaSuExcel(ByVal listaFiltriStr As String, _
                                          ByVal nomeVarSessionDt As String, _
                                          ByVal prefissoNomeFile As String, _
                                          ByVal objP_server As String) As rispostaStandard
        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try
            Dim dt As DataTable = HttpContext.Current.Session(nomeVarSessionDt)
            'verifico se esiste la variabile di sessione con il dt
            If IsNothing(dt) Then
                r.RispostaOK = False
                r.Errore = "Errore durante l'operazione: datatable non trovato in sessione"
                Exit Try
            End If

            'Imposto il dt filtrandolo in base ai filtri passati come parametro
            Dim dtFiltrato As DataTable = AgronicaControlli_2010.jquery_watable_modificato.FiltraDTconFiltriWatable(dt, listaFiltriStr)
            dtFiltrato.TableName = "Esportazione"

            'dato che la funzione di esportazione legge il capiton del datacolum che io ho sfruttato per salvare altre info,
            'metto come caption il nome della colonna
            'For Each dc As DataColumn In dtFiltrato.Columns
            '    dc.Caption = dc.ColumnName
            'Next

            'percorso e nome del file da esportare
            Dim nomeFileUnivoco As String = prefissoNomeFile & "_" & AgronicaCoreUtility.FileSystemHelper.NomeFileUnivoco(".xlsx")
            Dim _AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            Dim path_file As String = _AgroWebConfig.GestioneAllegati_Repository & "\" & nomeFileUnivoco

            'Creo il file xlsx a partire dal DT
            Dim workbook = New ClosedXML.Excel.XLWorkbook()
            workbook.Worksheets.Add(dtFiltrato)
            workbook.SaveAs(path_file)

            'creo l'url dove si deve andare a prendere il file
            Dim url As String = _AgroWebConfig.LinkAgronicaStampe.ToLower().Replace("gestionerichieste.aspx", "") & "File_Allegati/" & nomeFileUnivoco

            r.RispostaOK = True
            r.RispostaStringa = url

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function


End Class