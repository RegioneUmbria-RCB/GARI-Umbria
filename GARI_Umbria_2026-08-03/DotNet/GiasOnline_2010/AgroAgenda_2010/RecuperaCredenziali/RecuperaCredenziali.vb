Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreVarieBIZ

Public Class RecuperaCredenziali

    Public Shared Sub InizializzaCulturaPreLogin()

        If IsNumeric(HttpContext.Current.Session("LinguaCorrente")) Then
            Exit Sub
        End If

        Dim objLinguaSession As Lingua = HttpContext.Current.Session("LinguaCorrente")
        If objLinguaSession Is Nothing Then
            Dim linguaBrowserUtente = "it"
            Dim listLingue As String() = Web.HttpContext.Current.Request.UserLanguages
            If Not IsNothing(listLingue) Then
                linguaBrowserUtente = listLingue(0).Split("-")(0)
            End If
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaBrowserUtente)
            Dim lingua As New Lingua With {.CodiceISO = linguaBrowserUtente, .Lingua_cod = 0}
            lingua.Calcola_LinguaCod_da_CodiceISO()
            HttpContext.Current.Session("LinguaCorrente") = lingua
        Else
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(objLinguaSession.CodiceISO)
        End If
    End Sub

    Public Shared Sub InizializzaSitoGiasOnline()
        Dim inizializza As New Inizializzatore
        'crea connessioni e objparametrisuperserver o server a seconda della chiave nel webconfig
        inizializza.InizializzaSito_GiasOnLine(HttpContext.Current.Session)
    End Sub

    Public Shared Sub ValorizzaObjParametri(ByVal idSuperServer As Integer, ByRef outParametriServer As AgronicaCoreParametri, ByRef outParametriUtenti As AgronicaCoreParametri)
        Dim inizializza As New Inizializzatore

        If IsNothing(HttpContext.Current.Session("ASG_objParametri_Super_Server")) Then
            inizializza.InizializzaSito_GiasOnLine(HttpContext.Current.Session)
        End If

        InizializzaCulturaPreLogin()
        
        Dim objParametri_Super_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Super_Server"))

        Dim outStringaConnessioneServer, outStringaConnessioneUtenti, outIdUtenti, outPivaSuperUser, outNomeServer, outProgServer,
            outProgUtenti, outDescServer, outNoteServer, outDescUtenti, outNoteUtenti As String

        Inizializzatore.Ricavo_Parametri_Server_Utenti_Da_Id_Db_Server(objParametri_Super_Server, idSuperServer, outDescServer, outNoteServer,
                                                                       outDescUtenti, outNoteUtenti, outStringaConnessioneServer, outStringaConnessioneUtenti,
                                                                       outIdUtenti, outPivaSuperUser, outNomeServer, outProgServer, outProgUtenti)
        

        Dim objParametriWebConfig = inizializza.Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(
            outStringaConnessioneServer,
            objParametri_Super_Server.PivaSuperUser,
            Nothing,
            HttpContext.Current.Session)
        Dim AgroWebConfig As New AgroWebConfig(objParametri_Super_Server, objParametriWebConfig, True)

        outParametriServer = inizializza.Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(
            outStringaConnessioneServer,
            objParametri_Super_Server.PivaSuperUser,
            AgroWebConfig,
            HttpContext.Current.Session)

        outParametriServer.Lingua_Cod = objParametri_Super_Server.Lingua_Cod

        outParametriUtenti = inizializza.Crea_objParametri_Base_Da_Agrowebconfig_Session_Webconfig(
            outStringaConnessioneUtenti,
            objParametri_Super_Server.PivaSuperUser,
            AgroWebConfig,
            HttpContext.Current.Session)

        outParametriUtenti.Lingua_Cod = objParametri_Super_Server.Lingua_Cod


    End Sub

    Public Shared Function LinkPaginaLogin(ByVal valSuperServer As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim decoValSuperServer = Stringa_Decodifica(valSuperServer, AgroKey_EncoderDecoder)

        'Dim ID_DB_Sel As String = decoValSuperServer.Split("|")(0)
        'Dim TipoDB_Sel As String = decoValSuperServer.Split("|")(1)
        'Dim Server_Sel As String = decoValSuperServer.Split("|")(2)
        'Dim DB_Sel As String = decoValSuperServer.Split("|")(3)
        Dim PivaSuperUser_Sel As String = decoValSuperServer.Split("|")(4)
        'Dim Progressivo_Sel As String = decoValSuperServer.Split("|")(5)

        Dim dicQueryString As New Dictionary(Of String, String)
        dicQueryString.Add("pivasuperuser", PivaSuperUser_Sel)

        Dim linkBuilder As New UriBuilder
        linkBuilder.Scheme = HttpContext.Current.Request.Url.Scheme
        linkBuilder.Host = HttpContext.Current.Request.Url.Host
        linkBuilder.Port = HttpContext.Current.Request.Url.Port
        linkBuilder.Path = VirtualPathUtility.ToAbsolute("~/index.aspx") 'In sostituzione di ResolveUrl
        linkBuilder.Query = ComponiQueryStringDaDictionary(dicQueryString)

        r.RispostaOK = True
        r.RispostaStringa = linkBuilder.ToString()
        Return r
    End Function

    Public Shared Function ComponiQueryStringDaDictionary(ByVal dizionario As Dictionary(Of String, String)) As String
        Dim queryString As New List(Of String)

        For Each queryParameter As KeyValuePair(Of String, String) In dizionario
            queryString.Add(queryParameter.Key & "=" & queryParameter.Value)
        Next

        Return String.Join("&", queryString)
    End Function

End Class
