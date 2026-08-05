Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Web.Script.Serialization
Imports AgronicaCorePannelloDiControlloBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
Public Class PannelloDiControllo_ScriptService
    Inherits System.Web.Services.WebService

    'PER POPOLARE LA DDL DELLE AZIENDE
    <WebMethod(EnableSession:=True)> _
    Public Function LeggiAziende() As rispostaStandard(Of List(Of ListItem))
        Dim r As New rispostaStandard(Of List(Of ListItem))

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        'estraggo i dati sulle imprese visibili dall'utente che ha fatto login
        Dim ddlTmp As New DropDownList()
        AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente( _
                    ddlTmp, True, "Selezionare...", -1, "", " ORDER BY Rag_Soc asc", _
                    objParametri_Server, objParametri_Utenti)

        'converto la listitemcollection in list of listitem
        Dim lista As New List(Of ListItem)
        For Each x As ListItem In ddlTmp.Items
            lista.Add(x)
        Next

        r.RispostaOK = True
        r.RispostaStringa = lista

        Return r
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function Leggi_Utenti() As rispostaStandard(Of List(Of ListItem))
        Dim r As New rispostaStandard(Of List(Of ListItem))

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim UtentiR As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim dtUtenti As DataTable = UtentiR.Leggi("", 0, _
                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                "", "", objParametri_Utenti)

        Dim lista As New List(Of ListItem)
        lista.Add(New ListItem("Nessuna Selezione", -1))
        For Each dr As DataRow In dtUtenti.Rows
            If dr("Flag_Azienda_Persona") = "2" Then '2 significa che è una persona fisica e non giuridica
                lista.Add(New ListItem(dr("Cognome") & " " & dr("Nome"), dr("CodFisc")))
            End If
        Next

        r.RispostaOK = True
        r.RispostaStringa = lista

        Return r
    End Function

    <Services.WebMethod(EnableSession:=True)> _
    Public Function EsportaSuExcel(ByVal listaFiltriStr As String, _
                                          ByVal listaColonneVisibili As String, _
                                          ByVal nomeVarSessionDt As String, _
                                          ByVal prefissoNomeFile As String) As rispostaStandard(Of Byte())
        Dim r As New rispostaStandard(Of Byte())

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

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
            'Imposto il dt filtrandolo in base alle sole colonne visualizzate ed ordino
            dtFiltrato = AgronicaControlli_2010.jquery_watable_modificato.FiltraDTconSoloColonneOrdinateVisibili(dt, listaColonneVisibili)
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
            'r.RispostaStringa = url

            Dim binReader As New System.IO.BinaryReader(System.IO.File.Open(path_file, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            binReader.BaseStream.Position = 0
            Dim binFile As Byte() = binReader.ReadBytes(Convert.ToInt32(binReader.BaseStream.Length))
            binReader.Close()
            r.RispostaStringa = binFile

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function


    <Services.WebMethod(EnableSession:=True)> _
    Public Function EsportaSuExcel2(ByVal data As String, ByVal colonne As String) As rispostaStandard(Of Byte())

        Dim r As New rispostaStandard(Of Byte())
        ' Mappo i dati da json ad vb array
        data = data.Replace("<span class=""filter"">", "<span class='filter'>")

        Dim result = JsonConvert.DeserializeObject(data)
        colonne = colonne.Substring(1, colonne.Length - 2)
        Dim res_colonne As String() = colonne.Split(New Char() {","c})


        ' Creo il datatable che conterrà i dati
        Dim DT As New DataTable
        '' Mappo le colonne
        For i = 0 To res_colonne.Length - 1
            DT.Columns.Add(res_colonne(i), Type.GetType("System.String"))
        Next



        For Each row In result

            Dim dr As DataRow = DT.NewRow

            For i = 0 To res_colonne.Length - 1
                dr(res_colonne(i)) = row(res_colonne(i))
            Next

            DT.Rows.Add(dr)

        Next

        'percorso e nome del file da esportare
        Dim nomeFileUnivoco As String = "PnlCtrl_Scad_" & AgronicaCoreUtility.FileSystemHelper.NomeFileUnivoco(".xlsx")
        Dim _AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim path_file As String = _AgroWebConfig.GestioneAllegati_Repository & "\" & nomeFileUnivoco

        DT.TableName = "Esportazione"

        'Creo il file xlsx a partire dal DT
        Dim workbook = New ClosedXML.Excel.XLWorkbook()
        workbook.Worksheets.Add(DT)
        workbook.SaveAs(path_file)

        'creo l'url dove si deve andare a prendere il file
        Dim url As String = _AgroWebConfig.LinkAgronicaStampe.ToLower().Replace("gestionerichieste.aspx", "") & "File_Allegati/" & nomeFileUnivoco

        r.RispostaOK = True
        'r.RispostaStringa = url

        Dim binReader As New System.IO.BinaryReader(System.IO.File.Open(path_file, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        binReader.BaseStream.Position = 0
        Dim binFile As Byte() = binReader.ReadBytes(Convert.ToInt32(binReader.BaseStream.Length))
        binReader.Close()
        r.RispostaStringa = binFile


        Return r
    End Function

End Class