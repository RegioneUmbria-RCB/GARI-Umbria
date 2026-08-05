Imports System.Web.Services
Imports AgronicaCorePannelloDiControlloBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreVarieBIZ

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class NC_ScriptService_Categorie
    Inherits System.Web.Services.WebService

    'PER POPOLARE LA DDL DELLE AREE DI CATEGORIE 
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiAree(objP_server As String) As RispostaStandard
        Return LeggiAree_Generico(objP_server, 0)
    End Function

    'PER POPOLARE LA DDL DELLE AREE DI CATEGORIE DATA LA CATEGORIA
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiAree_ConSelezione(objP_server As String, ID_Categoria As Integer) As RispostaStandard
        Return LeggiAree_Generico(objP_server, ID_Categoria)
    End Function

    'PER POPOLARE LA DDL DELLE AREE DI CATEGORIE
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiAree_Generico(objP_server As String, ID_Categoria As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            'Estraggo tutte le categorie
            Dim CatR As New NC_Categorie_R
            Dim listaCat As List(Of NC_Categorie) = CatR.leggi_NC_Categorie(objParametri_Server)

            'estraggo uttte le areee in ordine alfabetico
            Dim arrayAree As String() = (From x As NC_Categorie In listaCat
                                         Order By x.Area
                                         Select x.Area).Distinct().ToArray()

            'Creo e popolo una lista di listItem per popolare il controllo
            Dim lista As New JArray()

            If ID_Categoria = 0 Then 'se niente deve essere selezionato carico la lista senza valori
                For Each x As String In arrayAree
                    lista.Add(New JObject(New JProperty("area", x), New JProperty("id_area", x)))
                Next
            Else 'se c'è una selezione imposto a ID_categoria l'elemento della mia selezione

                'salvo il nome dell'area che deve essere selezionata
                Dim nomeSelezione As String = (From x As NC_Categorie In listaCat
                                               Where (x.ID_Categoria = ID_Categoria)
                                               Select x.Area).First()

                Dim val As Integer
                For Each x As String In arrayAree
                    val = If(nomeSelezione = x, ID_Categoria, 0)
                    lista.Add(New JObject(New JProperty("area", x), New JProperty("id_area", val)))
                Next
            End If

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(lista, Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    'PER POPOLARE LA DDL DELLE TIPOLOGIE DI CATEGORIE
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiCategorie(objP_server As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            'Estraggo tutte le categorie
            Dim CatR As New NC_Categorie_R
            Dim listaCat As List(Of NC_Categorie) = CatR.leggi_NC_Categorie(objParametri_Server)

            Dim listaJSON As New JArray()
            For Each c As NC_Categorie In listaCat
                listaJSON.Add(New JObject(New JProperty("area", c.Area), New JProperty("tipologia", c.Tipologia), New JProperty("id_categoria", c.ID_Categoria)))
            Next

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(listaJSON, Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiNomeArea(objP_server As String, ID_Categoria As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim cat_R As New NC_Categorie_R()
            Dim tabCat As List(Of NC_Categorie) = cat_R.leggi_NC_Categorie(
                                                            objParametri_Server, ID_Categoria)

            If tabCat.Count <= 0 Then
                r.RispostaOK = False
                r.Errore = "Errore durante l'operazione: Nessuna categoria trovata con ID " + ID_Categoria
                Exit Try
            End If

            r.RispostaOK = True
            r.RispostaStringa = tabCat.First().Area
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Modifica(objP_server As String, id_cat As Integer, nomeArea As String, nomeTipologia As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Dim NC_W As New NC_Categorie_W
        Dim err As String = NC_W.modifica(objParametri_Server, id_cat, nomeArea, nomeTipologia)

        If err = "" Then
            r = LeggiAree(objP_server)
        Else
            r.RispostaOK = False
            r.Errore = err
        End If

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Aggiungi(objP_server As String, nomeArea As String, nomeTipologia As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        'Salvo l'elemento
        Dim NC_W As New NC_Categorie_W
        Dim err As String = NC_W.aggiungi(objParametri_Server, nomeArea, nomeTipologia)

        If err = "" Then
            r = LeggiAree(objP_server)
        Else
            r.RispostaOK = False
            r.Errore = err
        End If

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Cancella(objP_server As String, id_Categoria As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Dim NC_W As New NC_Categorie_W
        Dim err As String = NC_W.cancella(objParametri_Server, id_Categoria)

        If err = "" Then
            r = LeggiAree(objP_server)
        Else
            r.RispostaOK = False
            r.Errore = err
        End If

        Return r

    End Function

End Class