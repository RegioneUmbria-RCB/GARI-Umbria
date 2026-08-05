Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports System.Web.Script.Serialization
Imports AgronicaCorePannelloDiControlloBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreVarieBIZ

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
Public Class PannelloDiControllo_ScriptService_Categorie
    Inherits System.Web.Services.WebService

    'PER POPOLARE LA DDL DELLE AREE DI CATEGORIE 
    <WebMethod(EnableSession:=True)> _
    Public Function LeggiAree() As rispostaStandard(Of List(Of ListItem))
        Return LeggiAree_Generico(Nothing)
    End Function

    'PER POPOLARE LA DDL DELLE AREE DI CATEGORIE DATA LA CATEGORIA
    <WebMethod(EnableSession:=True)> _
    Public Function LeggiAree_ConSelezione(ID_Categoria As Integer) As rispostaStandard(Of List(Of ListItem))
        Return LeggiAree_Generico(ID_Categoria)
    End Function

    'PER POPOLARE LA DDL DELLE AREE DI CATEGORIE
    Public Function LeggiAree_Generico(ID_Categoria As Integer?) As rispostaStandard(Of List(Of ListItem))
        Dim r As New rispostaStandard(Of List(Of ListItem))

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim listaCat As List(Of PnlCtrl_Categorie) = HttpContext.Current.Session("PnlCtrl_ListaCategorie")

            'estraggo uttte le areee in ordine alfabetico
            Dim arrayAree As String() = (From x As PnlCtrl_Categorie In listaCat
                                         Order By x.Area
                                         Select x.Area).Distinct().ToArray()

            'Creo e popolo una lista di listItem per popolare il controllo
            Dim lista As New List(Of ListItem)
            lista.Add(New ListItem("Selezionare...", -1)) 'elemento vuoto iniziale

            If IsNothing(ID_Categoria) Then 'se niente deve essere selezionato carico la lista senza valori
                For Each x As String In arrayAree
                    lista.Add(New ListItem(x))
                Next
            Else 'se c'è una selezione imposto a ID_categoria l'elemento della mia selezione

                'salvo il nome dell'area che deve essere selezionata
                Dim nomeSelezione As String = (From x As PnlCtrl_Categorie In listaCat
                                               Where (x.ID = ID_Categoria)
                                               Select x.Area).First()

                Dim val As Integer
                For Each x As String In arrayAree
                    val = If(nomeSelezione = x, ID_Categoria, 0)
                    lista.Add(New ListItem(x, val))
                Next
            End If

            r.RispostaOK = True
            r.RispostaStringa = lista

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    'PER POPOLARE LA DDL DELLE TIPOLOGIE DI CATEGORIE
    <WebMethod(EnableSession:=True)> _
    Public Function LeggiTipologiePerArea(area As String) As rispostaStandard(Of List(Of ListItem))
        Dim r As New rispostaStandard(Of List(Of ListItem))

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            'recupero l'elenco delle categorie
            Dim tabCat As List(Of PnlCtrl_Categorie) = HttpContext.Current.Session("PnlCtrl_ListaCategorie")

            'filtro in base al nome dell'area ed ordino
            tabCat = (From x As PnlCtrl_Categorie In tabCat
                      Where x.Area = area
                      Order By x.Tipologia
                      Select x).ToList()

            Dim lista As New List(Of ListItem)
            lista.Add(New ListItem("Selezionare...", -1)) 'aggiungo un elemento vuoto
            For Each c As PnlCtrl_Categorie In tabCat
                lista.Add(New ListItem(c.Tipologia, c.ID))
            Next

            r.RispostaOK = True
            r.RispostaStringa = lista

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Function LeggiNomeArea(ID_Categoria As Integer) As rispostaStandard
        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim cat_R As New Categorie_R()
            Dim tabCat As List(Of PnlCtrl_Categorie) = cat_R.leggi_PnlCtrl_Categorie( _
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

End Class