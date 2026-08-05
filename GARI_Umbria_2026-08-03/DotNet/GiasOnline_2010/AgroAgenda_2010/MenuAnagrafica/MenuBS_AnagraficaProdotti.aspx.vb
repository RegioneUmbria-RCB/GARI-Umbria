Imports System.Web
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreUtility.CaricaListControl
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreEntityFramework
Imports System.Web.Script.Serialization
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreXML.XML_Stampe
Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreContabDAL
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabBIZ.Prodotti_Costi_W
Imports AgronicaCoreModelsSTD.exceptions

Public Class MenuBS_AnagraficaProdotti
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Public PageMode As String


    ''' <summary>
    ''' Per il caricamento dell'albero Anagrafico
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="PathRoot"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>


    Private Sub CambiaImpresa(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim TargetUrl As String
        Dim Origine As String
        Dim Destinazione As String

        'azzero il filtro scelto x le operazioni multi-aziendali
        Session("VariabiliFiltro") = Nothing

        'Costruisco il link
        Origine = Stringa_Codifica(
                        "../MenuAnagrafica/MenuBS_AnagraficaProdotti.aspx",
                        AgroKey_EncoderDecoder, Server)

        Destinazione = Stringa_Codifica(
                        "../MenuAnagrafica/MenuBS_AnagraficaProdotti.aspx",
                        AgroKey_EncoderDecoder, Server)


        TargetUrl = "../Filtrino/FiltrinoImprese.aspx" &
                    "?o=" & Origine &
                    "&d=" & Destinazione

        Response.Redirect(TargetUrl)

    End Sub

    ''' <summary>
    ''' Per il caricamento dell'albero Anagrafico
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="PathRoot"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod(EnableSession:=True)>
    Public Shared Function settaSa_Cod(ByVal sa_cod As String) As Boolean
        Dim objParametriAgenda As New ParametriAgenda
        objParametriAgenda.Sa_Cod = sa_cod
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function setta_modalita(ByVal modalita As String) As Boolean
        HttpContext.Current.Session("modalita") = modalita
    End Function

    ''' <summary>
    ''' Per il caricamento dell'albero Anagrafico
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="PathRoot"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim objParametriAgenda As New ParametriAgenda

        Dim objParametriAgenda_2010 As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
        objParametriAgenda_2010.Leggi()


        Dim TargetUrl As String = "../Menu/MenuBS_2017.aspx"
        Response.Redirect(TargetUrl)


    End Sub

    ' Per modifica multipla zoo
    Public objImpianto As New JObject()
    Dim xPiva As String
    Dim xSa_Cod As String
    Dim xAppezza As String
    Dim xId_Imp As String
    Public Shared listModuliAttivi_anagrafe_log As New List(Of Integer)


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        ' Elimino tutte le variabili in Sessione utilizzate nelle pagine interne
        HttpContext.Current.Session("dt_Codici") = Nothing
        HttpContext.Current.Session("dt_Padri") = Nothing
        HttpContext.Current.Session("dt_Rubrica") = Nothing

        ' Setto la visibilità dei bottoni in Master
        Master.flag_pag_Anagrafica = True

        ' Forzo titolo pagina in base ai parametri

        If Master.flag_MenuBS_2017 Then
            Dim IDSezione = 5000
            Master.SetTitoloPagina(IDSezione)

        End If

        Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        Dim objParametriAgenda As New ParametriAgenda




        Dim FiltroCentri As String = ""
        Dim DtCentriVisibili As DataTable
        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R

        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, "", "", objParametri_Server)

        If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
            For i = 0 To DtCentriVisibili.Rows.Count - 1
                FiltroCentri &= " (Centri_Aziendali.Piva='" & DtCentriVisibili.Rows(i).Item("Piva") & "' " &
                                " AND Centri_Aziendali.sa_cod=" & DtCentriVisibili.Rows(i).Item("Sa_Cod") & ") OR "
            Next
            If FiltroCentri <> "" Then
                FiltroCentri = " (" & Left(FiltroCentri, FiltroCentri.Length - 3) & ")"
            End If
        End If


        Master.flag_pag_MenuAgenda = True

        '@Paolo
        ' Setto la modalità di base del filtraggio dei contatti (0 = tutti appartenenti all'impresa)
        HttpContext.Current.Session("modalita") = "0"

        Dim Leggi_impostazioni As New Utenti_Impostazioni_Read
        Dim modificaMultipla As String

        If objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername Then
            modificaMultipla = "2"
        Else
            modificaMultipla = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SuperUser_Ereditatore, objParametri_Utenti, 2)  'to do...
        End If

        hidden_modificaMultipla.Value = modificaMultipla
        hidden_azienda.Value = objParametriAgenda.Piva
        hidden_sa_cod.Value = objParametriAgenda.Sa_Cod

        'Per nuovo albero anagrafico
        AlberoAnagrafica2017.AlberoAnagrafica2017_headerPlaceHeader = gisHeader

        'Impostazione SuperUser x gestione esercizi

        hidden_gest_esercizi.Value = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SuperUser_Gestione_Esercizi, objParametri_Utenti, 2)

        'Impostazione SuperUser x ereditatore
        If objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername Then
            hidden_ereditatore.Value = "2"
        Else
            hidden_ereditatore.Value = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SuperUser_Ereditatore, objParametri_Utenti, 2)
        End If

        ' Per modifica multipla zoo

        objParametriAgenda = New ParametriAgenda
        xPiva = objParametriAgenda.Piva
        xSa_Cod = objParametriAgenda.Sa_Cod
        xAppezza = objParametriAgenda.Appezza
        xId_Imp = objParametriAgenda.Id_Imp

        objImpianto.Add(New JProperty("piva", xPiva))
        objImpianto.Add(New JProperty("sa_cod", xSa_Cod))
        objImpianto.Add(New JProperty("appezza", xAppezza))
        objImpianto.Add(New JProperty("id_reg", xId_Imp))

        Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        objImpianto.Add(New JProperty("sa_nome", objCentriAz.SaNome_from_SaCod(xPiva, xSa_Cod, objParametri_Server)))

        AddHandler Master.ImgBtnAnnullaTutto.Click, AddressOf Me.AnnullaTutto

    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Function GetFiltrino() As String

        Dim Origine As String
        Dim Destinazione As String

        'azzero il filtro scelto x le operazioni multi-aziendali
        HttpContext.Current.Session("VariabiliFiltro") = Nothing

        'Costruisco il link
        Origine = Stringa_Codifica(
                        "../MenuAnagrafica/MenuBS_AnagraficaProdotti.aspx",
                        AgroKey_EncoderDecoder)

        Destinazione = Stringa_Codifica(
                        "../MenuAnagrafica/MenuBS_AnagraficaProdotti.aspx",
                        AgroKey_EncoderDecoder)


        Dim TargetUrl = "../Filtrino/FiltrinoImprese.aspx" &
                    "?o=" & Origine &
                    "&d=" & Destinazione

        Return TargetUrl
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function gotoNewElement(ByVal tipo As Integer, ByVal chiave As String) As RispostaStandard
        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim risp = New RispostaStandard


        'TODO controllo se sessione scaduta e 
        If objParametri_Server Is Nothing Then
            risp.Sessione = False
            risp.RispostaOK = vbFalse
            Return risp
        End If

        Dim permessi = New PermessiUtente()

        Try
            risp.RispostaOK = True
            If permessi.getPermesso(enum_Security_Attivita.Angrafica_Prodotti).Scrittura = True Then
                risp.RispostaStringa = NuovoProdotto(chiave)
            Else
                Throw New Exception(AgronicaAgenda_2010.MancanzaPermessiCreazioneRaggruppamentoStalla)
            End If
            objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
            objParametriAgenda.Particelle = New List(Of Particella)

        Catch ex As Exception
            risp.RispostaOK = False
            risp.Errore = ex.Message
        End Try


        Return risp
    End Function





    'Delete

    <WebMethod(EnableSession:=True)>
    Private Function PreparaperCancellazione(ByVal xTipoNodo As Integer, ByVal xChiave As String)
        Dim Testo As String
        Dim UrlTarget As String

        Dim objParametriAgenda = New ParametriAgenda

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        Dim r_msg As String = ""

        '----------------------------------------------------------------
        '------ MODIFICA x GIAS2GIAS (28/03/2008) -------
        '----------------------------------------------------------------
        'Verifico se l'Impresa è stata generata dal SuperUser corrente...
        'In caso contrario BLOCCO qualsiasi Operazione!!!


        ' Verifico se ESISTE la PivaSuperUser Origine Dato

        ' Se ESISTE
        '   Verifico se corrisponde al SuperUserCorrente 
        '       Se UGUALE proseguo
        '       Se DIVERSO ---> blocco l'operazione

        ' Se NON ESISTE
        '   Creo il record nella tabella Imprese_Codici
        Dim objCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        Dim SuperUser_Corrente As Boolean
        Dim PivaSuperUser_Origine As String

        If objCodici.EsistePivaSuperUser_OrigineDato(objParametriAgenda.Piva, Session("ASG_SuperUser_CodFiscale"), PivaSuperUser_Origine, SuperUser_Corrente,
                                                        "", objParametri_Server) = True Then


            If SuperUser_Corrente = True Then
                'OK
            Else
                'blocco
                If SuperUser_Corrente = False Then
                    r_msg = String.Format(AgronicaAgenda_2010.MenuBS_Anagrafica_PreparaperCancellazione_ArchivioDiverso, vbCrLf)
                    Exit Function
                End If

            End If

        Else
            ' SE NON esiste lo creo
            ' Inserisci_CodiceImpresa2(xPiva, enum_CodiciAnagrafe.PivaSuperUser_Origine_Dato, CStr(Session("ASG_SuperUser_CodFiscale")), objParametri_Server)

            Dim objImpreseCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Write

            'Inserisco
            Dim IntDummy As Integer = objImpreseCodici.Scrivi(objParametriAgenda.Piva,
                                               enum_CodiciAnagrafe.PivaSuperUser_Origine_Dato,
                                               CStr(Session("ASG_SuperUser_CodFiscale")),
                                               AGRODATAINIZIO,
                                               AGRODATAFINE,
                                               objParametri_Server)

            objImpreseCodici = Nothing

        End If

        '----------------------------------------------------------------
        '-------- Fine Modifica -----------------------------------------
        '----------------------------------------------------------------

        'Azzero la variabile di appoggio
        Testo = ""

        'Verifico il tipo del nodo selezionato
        Select Case xTipoNodo

            Case enum_TipoNodo.Utente

                '------------------------------------------------
            Case enum_TipoNodo.Impresa

                If objParametriAgenda.Piva <> Session("ASG_SuperUser_CodFiscale") Then

                    'Carico la pagina di cancellazione
                    UrlTarget = "Cancella_Elemento.aspx" &
                                    "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                                    "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) &
                                    "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                                    "&r=" & Stringa_Codifica("../AlberoImprese/AlberoImprese.aspx", AgroKey_EncoderDecoder, Server)

                    'Carico la pagina
                    Response.Redirect(UrlTarget)

                Else
                    r_msg = AgronicaAgenda_2010.ImpossibileEliminareImpresaSelezionata
                    Exit Function

                End If

                '------------------------------------------------
            Case enum_TipoNodo.Centro

                'Carico la pagina di cancellazione
                UrlTarget = "Cancella_Elemento.aspx" &
                                "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) &
                                "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                                "&r=" & Stringa_Codifica("AlberoImprese.aspx", AgroKey_EncoderDecoder, Server)

                'Carico la pagina
                Response.Redirect(UrlTarget)

                '------------------------------------------------
            Case enum_TipoNodo.Campo,
                 enum_TipoNodo.Serra

                'Carico la pagina di cancellazione
                UrlTarget = "Cancella_Elemento.aspx" &
                                "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) &
                                "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                                "&r=" & Stringa_Codifica("AlberoImprese.aspx", AgroKey_EncoderDecoder, Server)

                'Carico la pagina
                Response.Redirect(UrlTarget)

                '------------------------------------------------
            Case enum_TipoNodo.Appezzamento

                'Carico la pagina di cancellazione
                UrlTarget = "Cancella_Elemento.aspx" &
                                "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) &
                                "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                                "&r=" & Stringa_Codifica("AlberoImprese.aspx", AgroKey_EncoderDecoder, Server)

                'Carico la pagina
                Response.Redirect(UrlTarget)

                '------------------------------------------------
            Case enum_TipoNodo.ImpiantoNudo,
                 enum_TipoNodo.ImpiantoArborea,
                 enum_TipoNodo.ImpiantoErbacea,
                 enum_TipoNodo.ImpiantoOrticola

                'Carico la pagina di cancellazione
                UrlTarget = "Cancella_Elemento.aspx" &
                                "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) &
                                "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                                "&r=" & Stringa_Codifica("AlberoImprese.aspx", AgroKey_EncoderDecoder, Server)

                'Carico la pagina
                Response.Redirect(UrlTarget)

                '------------------------------------------------
            Case enum_TipoNodo.CatastoAziendale

                '------------------------------------------------
            Case enum_TipoNodo.Particella

                'Carico la pagina di cancellazione
                '''UrlTarget = "Cancella_Elemento.aspx" & _
                '''                "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) & _
                '''                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) & _
                '''                "&p=" & Stringa_Codifica(objParametriAgenda.piva, AgroKey_EncoderDecoder, Server)

                UrlTarget = "../GestioneCatasto/GestioneCatasto_Particella_Delete.aspx" &
                                 "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                                 "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) &
                                 "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                                 "&r=" & Stringa_Codifica("../AlberoImprese/AlberoImprese.aspx", AgroKey_EncoderDecoder, Server)

                'Carico la pagina
                Response.Redirect(UrlTarget)

                '------------------------------------------------
            Case enum_TipoNodo.Fabbricato_Generico,
                 enum_TipoNodo.f_Abitazione,
                 enum_TipoNodo.f_CellaFrigorifera,
                 enum_TipoNodo.f_ImpiantoLavorazione,
                 enum_TipoNodo.f_Magazzino,
                 enum_TipoNodo.f_Silos,
                 enum_TipoNodo.f_Stalla,
                 enum_TipoNodo.f_Fienile,
                 enum_TipoNodo.f_Essiccatoio

                'Carico la pagina di cancellazione
                UrlTarget = "Cancella_Elemento.aspx" &
                                "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) &
                                "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                                "&r=" & Stringa_Codifica("AlberoImprese.aspx", AgroKey_EncoderDecoder, Server)

                'Carico la pagina
                Response.Redirect(UrlTarget)

                '------------------------------------------------
            Case enum_TipoNodo.Persona

                'Carico la pagina di cancellazione
                UrlTarget = "Cancella_Elemento.aspx" &
                                "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder, Server) &
                                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) &
                                "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                                "&r=" & Stringa_Codifica("AlberoImprese.aspx", AgroKey_EncoderDecoder, Server)

                'Carico la pagina
                Response.Redirect(UrlTarget)

                '------------------------------------------------

            Case Else

                r_msg = AgronicaAgenda_2010.MenuBS_Anagrafica_PreparaperCancellazione_SelezionareNodo
                Exit Function

        End Select
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Permessi_Pagina() As rispostaStandard(Of MenuBS_Anagrafica_Permessi)
        Dim r As New rispostaStandard(Of MenuBS_Anagrafica_Permessi)
        r.RispostaStringa = New MenuBS_Anagrafica_Permessi

        Try
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            HttpContext.Current.Session("ASG_Utente_Username"),
                                            HttpContext.Current.Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Analisi_Dati_Meteo,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

            r.RispostaStringa.PermessiMeteoDSS = UtenteAbilitato

            UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                            HttpContext.Current.Session("ASG_Utente_Username"),
                                            HttpContext.Current.Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Gest_Stalle,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
            r.RispostaStringa.PermessiZoo = UtenteAbilitato

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function ModificaMultipla(ByVal parametri As String, ByVal dati As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            Dim objEreditatore As New AgronicaCoreAnagrafeBIZ.Ereditatore
            r = objEreditatore.ModificaMultipla(parametri, dati, objParametri_Server)

        Catch ex As GiasException
            'Errore gestito
            r.RispostaStringa = "Impossibile proseguire con il salvataggio: <br>" & ex.Message
            r.RispostaOK = False
        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function EditProdotto(ByVal piva As String,
                                        ByVal mat_cod As String,
                                        ByVal elem_cod As String,
                                        ByVal prodotto_des As String,
                                        ByVal pro_cod As String,
                                        ByVal duplica As Boolean,
                                        ByVal proprietario As Boolean,
                                        ByVal sa_cod As String,
                                        ByVal isalias As Boolean) As String


        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        'Salvo la PIVA del centro aziendale per poter risettare corretamente il filtro delle tabelle al torna indietro
        objParametriAgenda.Piva_Origine = ""
        objParametriAgenda.Piva_Origine = objParametriAgenda.Piva
        'objParametriAgenda.Sa_Cod = chiave.Split("_")(1)

        Dim operazione As String
        Dim url As String = "../Anagrafica/Prodotto_Edit.aspx"

        'Se duplica è true allora si è cliccata il pulsante Duplica
        If (duplica = True) Then
            operazione = "?o=" & Stringa_Codifica(enum_TipoOperazioneDB.Copia, AgroKey_EncoderDecoder, Nothing)
            'Altrimenti siamo solo in Modifica
        Else
            operazione = "?o=" & Stringa_Codifica(enum_TipoOperazioneDB.Modifica, AgroKey_EncoderDecoder, Nothing)
        End If
        Dim pivaParam As String = "&piva=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, Nothing)
        Dim matCodParam As String = "&mat_cod=" & Stringa_Codifica(mat_cod, AgroKey_EncoderDecoder, Nothing)
        Dim elemCodParam As String = "&elem_cod=" & Stringa_Codifica(elem_cod, AgroKey_EncoderDecoder, Nothing)
        Dim duplicaParam As String = "&duplica=" & duplica
        Dim tuttiTabParam As String = "&proprietario=" & Stringa_Codifica(proprietario, AgroKey_EncoderDecoder, Nothing)
        Dim proCodParam As String = "&pro_cod=" & Stringa_Codifica(pro_cod, AgroKey_EncoderDecoder, Nothing)
        Dim prodottoDesParam As String = "&prodotto_des=" & Stringa_Codifica(prodotto_des, AgroKey_EncoderDecoder, Nothing)
        Dim saCodParam As String = "&sa_cod=" & Stringa_Codifica(sa_cod, AgroKey_EncoderDecoder, Nothing)
        Dim isAliasParam As String = "&isalias=" & Stringa_Codifica(isalias, AgroKey_EncoderDecoder, Nothing)

        Dim queryString As String = operazione & pivaParam & matCodParam & elemCodParam & duplicaParam & tuttiTabParam & proCodParam & prodottoDesParam & saCodParam & isAliasParam
        Return url & queryString

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoProdotto(ByVal piva As String,
                                        ByVal mat_cod As String,
                                        ByVal elem_cod As String,
                                        ByVal prodotto_des As String,
                                        ByVal pro_cod As String,
                                        ByVal sa_cod As String,
                                        ByVal isalias As Boolean) As String

        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        'Salvo la PIVA del centro aziendale per poter risettare corretamente il filtro delle tabelle al torna indietro
        objParametriAgenda.Piva_Origine = ""
        objParametriAgenda.Piva_Origine = objParametriAgenda.Piva
        'objParametriAgenda.Sa_Cod = chiave.Split("_")(1)

        Dim url As String = "../Anagrafica/Prodotto_Edit.aspx"
        Dim operazione As String = "?o=" & Stringa_Codifica(enum_TipoOperazioneDB.Lettura, AgroKey_EncoderDecoder, Nothing)
        Dim pivaParam As String = "&piva=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, Nothing)
        Dim matCodParam As String = "&mat_cod=" & Stringa_Codifica(mat_cod, AgroKey_EncoderDecoder, Nothing)
        Dim elemCodParam As String = "&elem_cod=" & Stringa_Codifica(elem_cod, AgroKey_EncoderDecoder, Nothing)
        Dim proCodParam As String = "&pro_cod=" & Stringa_Codifica(pro_cod, AgroKey_EncoderDecoder, Nothing)
        Dim prodottoDesParam As String = "&prodotto_des=" & Stringa_Codifica(prodotto_des, AgroKey_EncoderDecoder, Nothing)
        Dim saCodParam As String = "&sa_cod=" & Stringa_Codifica(sa_cod, AgroKey_EncoderDecoder, Nothing)
        Dim isAliasParam As String = "&isalias=" & Stringa_Codifica(isalias, AgroKey_EncoderDecoder, Nothing)

        Dim queryString As String = operazione & pivaParam & matCodParam & elemCodParam & proCodParam & prodottoDesParam & saCodParam & isAliasParam
        Return url & queryString

    End Function

    Public Shared Function NuovoProdotto(ByVal elem_cod As String, Optional ByVal isalias As Boolean = False) As String

        Dim objParametriAgenda = New ParametriAgenda
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
        'Salvo la PIVA del centro aziendale per poter risettare corretamente il filtro delle tabelle al torna indietro
        objParametriAgenda.Piva_Origine = ""
        objParametriAgenda.Piva_Origine = objParametriAgenda.Piva


        Dim url As String = "../Anagrafica/Prodotto_Edit.aspx"

        Dim operazione As String = "?o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, Nothing)
        Dim pivaParam As String = "&piva=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Nothing)
        Dim matCodParam As String = "&mat_cod=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing)
        Dim elemCodParam As String = "&elem_cod=" & Stringa_Codifica(elem_cod, AgroKey_EncoderDecoder, Nothing)
        Dim proCodParam As String = "&pro_cod=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing)
        Dim tuttiTabParam As String = "&proprietario=" & Stringa_Codifica(True, AgroKey_EncoderDecoder, Nothing)
        Dim saCodParam As String = "&sa_cod=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing)
        Dim isAliasParam As String = "&isalias=" & Stringa_Codifica(isalias, AgroKey_EncoderDecoder, Nothing)

        Dim queryString As String = operazione & pivaParam & matCodParam & elemCodParam & proCodParam & tuttiTabParam & saCodParam & isAliasParam
        Return url & queryString

    End Function





    <WebMethod(EnableSession:=True)>
    Public Shared Function CreaOperazione(ByVal lav_cod As Integer, ByVal gru_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim gru_cod_list As New List(Of Integer)({1, 2, 3, 4, 5, 100})
            Dim veg_cod = 0
            Dim PaginaLink As String = MenuBS_Agenda_Nuovo.NuovaOperazioneAgenda(lav_cod, veg_cod)
            PaginaLink &= "?PaginaOrigine=" & CStr(enum_PagineGiasOnline.MenuAnagrafica)
            Dim objParametri_Agenda As New ParametriAgenda
            Dim objParametriAgenda = New ParametriAgenda
            Dim impiantiList = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

            Dim id_cod As Integer = 0

            Dim obj_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim obj_Impianti_codici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
            Dim dt_impianto As New DataTable

            If gru_cod_list.Contains(gru_cod) Then
                dt_impianto = obj_Impianti.Leggi(objParametriAgenda.Piva,
                                                 objParametriAgenda.Sa_Cod,
                                                 objParametriAgenda.Appezza,
                                                 objParametriAgenda.Id_Imp,
                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                 "", "", objParametri_Server)
            End If

            Dim data_Operazione = DateTime.Today

            If gru_cod_list.Contains(gru_cod) AndAlso dt_impianto.Rows.Count > 0 Then

                Dim validita_inizio = CDate(dt_impianto.Rows(0)("Validita_Inizio"))
                Dim validita_fine = CDate(dt_impianto.Rows(0)("Validita_Fine"))

                If validita_inizio > Date.Now Then
                    data_Operazione = validita_inizio
                ElseIf validita_fine < Date.Now Then
                    data_Operazione = validita_fine
                End If

                If lav_cod = LAVCOD_SEMINA Or
                    lav_cod = LAVCOD_TRAPIANTO Or
                    lav_cod = LAVCOD_TRAPIANTO_IN_SERRA Then
                    data_Operazione = validita_inizio
                End If

            End If

            objParametriAgenda.Data = data_Operazione

            Dim cul_cod = 0

            If gru_cod_list.Contains(gru_cod) Then
                cul_cod = obj_Impianti.CulCod_from_PivaSaCodAppezzaIdimp(objParametriAgenda.Piva,
                                                            objParametriAgenda.Sa_Cod,
                                                            objParametriAgenda.Appezza,
                                                            objParametriAgenda.Id_Imp,
                                                            objParametri_Server)

                If cul_cod = 0 Then
                    id_cod = obj_Impianti.Leggi_DestinazioneUso_Impianto(objParametriAgenda.Piva,
                                                                objParametriAgenda.Sa_Cod,
                                                                objParametriAgenda.Appezza,
                                                                objParametriAgenda.Id_Imp,
                                                                "", "", objParametri_Server)
                End If
            End If

            Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
            Dim veg_cod_s As String = ""

            If gru_cod_list.Contains(gru_cod) Then
                If cul_cod <> 0 Then
                    veg_cod_s = objCultivar.VegCod_from_CulCod(cul_cod, objParametri_Server)
                    veg_cod = veg_cod_s
                Else
                    veg_cod_s = "0/" & id_cod
                End If
            End If



            objParametriAgenda.Veg_Cod = veg_cod_s

            If gru_cod_list.Contains(gru_cod) Then
                Dim imp = New AgronicaCoreModello.ParametriAgenda_Temp.Impianto
                imp.Piva = objParametriAgenda.Piva
                imp.Sa_Cod = objParametriAgenda.Sa_Cod
                imp.Appezza = objParametriAgenda.Appezza
                imp.ID_Reg = objParametriAgenda.Id_Imp
                impiantiList.Add(imp)
                objParametriAgenda.Impianti = impiantiList
            End If

            objParametriAgenda.TipoOperazioneAgenda = "1"

            'objParametriAgenda.Piva = piva
            'objParametriAgenda.Sa_Cod = sa_cod
            'objParametriAgenda.Appezza = appezza
            'objParametriAgenda.Id_Imp = id_reg

            objParametriAgenda.Lav_Cod = lav_cod
            objParametriAgenda.salva()

            Dim objParametriAgenda_2010 As New ParametriAgenda_2010
            objParametriAgenda_2010.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_Lavorazioni
            objParametriAgenda_2010.PaginaProvenienza = enum_PagineAgenda_2010.Pagina_Anagrafica_Impianto
            'Dim PaginaLink As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(
            '    Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objParametriAgenda_2010
            ')

            r.RispostaOK = True
            r.RispostaStringa = PaginaLink

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function dateValiditaAgenda() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objRes As New JObject

            objRes("Validita_Inizio") = CDate(objParametri_Server.FinestraTemporaleInizio)
            objRes("Validita_Fine") = CDate(objParametri_Server.FinestraTemporaleFine)

            r.RispostaOK = True
            r.RispostaStringa = objRes.ToString

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function modificaMultiplaRedirect(ByVal obj_Impianto_str, ByVal capiSelezionatiModificaMultipla, Qs_Visibilita) As RispostaStandard

        Dim r As New RispostaStandard



        Try
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
            Dim obj_Impianto = Newtonsoft.Json.JsonConvert.DeserializeObject(obj_Impianto_str)
            Dim objImpostazioni_Utenti As New Utenti_Impostazioni_Read
            Dim piva As String = obj_Impianto.GetValue("piva").ToString()
            Dim sa_cod As Integer = CInt(obj_Impianto.GetValue("sa_cod"))
            Dim DefaultRegolamento = objImpostazioni_Utenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_COD_REGOLAMENTO, objParametri_Utenti)

            If IsNumeric(DefaultRegolamento) AndAlso CInt(DefaultRegolamento) < 1 Then
                DefaultRegolamento = 1
            End If

            Dim NoteLog As String = NOTELOG_ANAGRAFE_BOOTSTRAP

            r.ParametroDue = True
            r.ParametroDue_stringa = ""

            Dim TargetUrl = "../Anagrafica/Modifica_Multipla_zoo.aspx"
            If Qs_Visibilita <> 0 Then
                TargetUrl &= "?visibilita=" & CStr(Qs_Visibilita)
            End If

            Dim objParametriAgenda = New ParametriAgenda
            objParametriAgenda.Svuota_DatiOperazione()
            objParametriAgenda.Piva = piva
            objParametriAgenda.Raggruppamento_Cod = 0
            Dim ListaImpianti As New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
            Dim counter As Integer = 0

            'Loop creato perchè creando un loop con la lista impianti senza prima inizializzarla ogni Progetto_Cod precedente veniva sovrascritto con l'ultimo Progetto_Cod nel loop.
            For a = 1 To capiSelezionatiModificaMultipla.Length
                ListaImpianti.Add(New AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
            Next

            Dim Imp As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

            For Each i In capiSelezionatiModificaMultipla
                System.Diagnostics.Debug.WriteLine(i.item("Cod_Progetto"))
                Imp.Progetto_Cod = i.item("Cod_Progetto")
                ListaImpianti(counter).Progetto_Cod = Imp.Progetto_Cod
                counter += 1
            Next

            objParametriAgenda.Impianti = ListaImpianti
            objParametriAgenda.TipoOperazioneAgenda = "1"
            objParametriAgenda.Lav_Cod = 0
            objParametriAgenda.salva()

            r.ParametroDue_stringa = TargetUrl
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try


        Return r

    End Function




    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Specie() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim SpecieVegetali_R As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R()
            Dim dtSpecie = SpecieVegetali_R.Leggi(0, 0, "", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametriServer)

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dtSpecie, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            'i18N Errore durante l'operazione da tradurre
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Varieta(filtro_specie As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim filtro_aggiuntivo As String = ""
            If Trim(filtro_specie) <> "" Then
                filtro_aggiuntivo = " Cultivar.Veg_Cod IN (" & filtro_specie & ") "
            End If

            Dim Cultivar_R As New AgronicaCoreMetaSchemaDAL.Cultivar_R
            Dim dtVarieta = Cultivar_R.Leggi(0, 0, "", enumSelezioneVariabile.Selezione_JoinDescrizioni, filtro_aggiuntivo, "", objParametri_Server)

            Dim jArrayParamQual As New JArray()
            For Each dr As DataRow In dtVarieta.Rows
                jArrayParamQual.Add(New JObject(New JProperty("Cul_Cod", dr.Item("Cul_Cod")), New JProperty("Cul_Des", dr.Item("Veg_Des") & " - " & dr.Item("Cul_Des"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayParamQual, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            'i18N Errore durante l'operazione da tradurre
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Categorie_Commerciali(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objCC As New AgronicaCoreContabDAL.Linee_Classi_Produzioni_R
            Dim dtCategCommle = objCC.Leggi(piva, 0, "", "", objParametriServer)

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dtCategCommle, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            'i18N Errore durante l'operazione da tradurre
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Public Shared Function Leggi_Gruppi_Merce(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim GruppiMerceList As New List(Of Object)

            Dim objGM As New AgronicaCoreAnagrafeDAL.Gruppi_Merce_R
            Dim dtGruppiMerce = objGM.Leggi_con_Visibilita(piva, "", "", objParametriServer)


            For Each row As DataRow In dtGruppiMerce.Rows

                GruppiMerceList.Add(New With
                                {
                                     .Id_Gruppo_Merce = CInt(row("Id_Gruppo_Merce").ToString()),
                                     .Descrizione_Concatenata = row("Codice").ToString() & " " & row("Descrizione").ToString()
                                })

            Next

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(GruppiMerceList, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            'i18N Errore durante l'operazione da tradurre
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_XddlDittadiProvenienza() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim dt As DataTable
            Dim objDitte As New AgronicaCoreMetaSchemaDAL.Ditte_R
            dt = objDitte.Leggi(0, "", "", "", objParametriServer)


            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            'i18N Errore durante l'operazione da tradurre
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_XddlSpecieAnimali() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim List As New DropDownList
            AgronicaCoreUtility.CaricaListControl.Lista_Specie_Animali(List, True, "", 0, 0, "", "", objParametriServer)

            Dim AnimaliList As New List(Of Object)
            For i = 0 To List.Items.Count - 1

                AnimaliList.Add(New With
                                {
                                     .SPE_COD = List.Items(i).Value,
                                     .SPE_DES = List.Items(i).Text
                                })

            Next
            r.RispostaStringa = JsonConvert.SerializeObject(AnimaliList, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            'i18N Errore durante l'operazione da tradurre
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_XddlIndirizzoProduttivo(ByVal Gen_Cod As Integer, ByVal Spe_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim List As New DropDownList
            AgronicaCoreUtility.CaricaListControl.Lista_IndirizziProd_Animali(List, True, "Tutti gli Indirizzi Produttivi", -1, Gen_Cod, Spe_Cod, "", "", objParametriServer)

            Dim IndirizziList As New List(Of Object)
            For i = 0 To List.Items.Count - 1

                IndirizziList.Add(New With
                                {
                                     .IPRO_COD = List.Items(i).Value,
                                     .IPRO_DES = List.Items(i).Text
                                })

            Next
            r.RispostaStringa = JsonConvert.SerializeObject(IndirizziList, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            'i18N Errore durante l'operazione da tradurre
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function Leggi_XddlTipologieSementi() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim dt As DataTable
            Dim objTipoSem As New AgronicaCoreMetaSchemaDAL.TipologieSementi_R
            dt = objTipoSem.Leggi(0, "", "", "", objParametriServer)

            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            'i18N Errore durante l'operazione da tradurre
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function CaricaGrigliaStoricoPrezzi(ByVal piva As String, ByVal elem_cod As Integer, ByVal mat_cod As Integer, ByVal pro_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Dim dtProdotto As DataTable
            Dim obj As New Prodotti_Costi_R

            If piva <> "" Then
                Dim FiltroperPiva As String = "Prodotti_Costi.piva = '" & Agro_SQL_SaveText(piva) & "' And Prodotti_Costi.Id_Budget = 0 "

                dtProdotto = obj.Leggi(piva,
                                       elem_cod,
                                       "",
                                       pro_cod,
                                       mat_cod,
                                       0,
                                       0,
                                       0,
                                       AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       FiltroperPiva,
                                       "", objParametriServer)


                r.RispostaStringa = JsonConvert.SerializeObject(dtProdotto, Newtonsoft.Json.Formatting.None)
            End If


            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            'i18N Errore durante l'operazione da tradurre
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_GruppoVarietale(ByVal Veg_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim dt As DataTable
            Dim obj As New AgronicaCoreMetaSchemaDAL.GruppoVarietale_R

            dt = obj.Leggi(Veg_Cod,
                           0, "",
                           enumSelezioneVariabile.Selezione_JoinCompleta,
                           "", "",
                           objParametriServer)

            If Not IsNothing(dt) Then
                Dim xCod As Integer
                Dim xDes As String
                Dim udmList As New List(Of Object)
                For i = 0 To dt.Rows.Count - 1
                    'Non Ibrido
                    xCod = dt.Rows(i).Item("Grva_Cod")
                    xDes = CStr(dt.Rows(i).Item("Grva_Des"))

                    udmList.Add(New With
                             {
                                .Grva_Cod = xCod,
                                .Grva_Des = xDes
                            })

                    'Ibrido
                    xCod = -1 * dt.Rows(i).Item("Grva_Cod")
                    xDes = CStr(dt.Rows(i).Item("Grva_Des") & " -- Ibrido")

                    udmList.Add(New With
                                {
                                     .Grva_Cod = xCod,
                                     .Grva_Des = xDes
                                })

                Next

                r.RispostaStringa = JsonConvert.SerializeObject(udmList, Newtonsoft.Json.Formatting.None)
                r.RispostaOK = True

            End If


        Catch ex As Exception
            r.RispostaOK = False
            'i18N Errore durante l'operazione da tradurre
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Public Shared Function Leggi_VarietaColturale(ByVal piva As String, ByVal Veg_cod As Integer, ByVal Cul_cod_da_modificare As Integer)
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Try
            Dim List As New DropDownList

            AgronicaCoreUtility.CaricaListControl.Cultivar(List, False, "", "", Veg_cod, 0, "", True, Cul_cod_da_modificare, 0, "", "", objParametriServer, objParametriUtenti)

            Dim udmList As New List(Of Object)
            For i = 0 To List.Items.Count - 1

                udmList.Add(New With
                                {
                                     .Cul_Cod = List.Items(i).Value,
                                     .Cul_Des = List.Items(i).Text
                                })

            Next

            r.RispostaStringa = JsonConvert.SerializeObject(udmList, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            'i18N Errore durante l'operazione da tradurre
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function Leggi_Unita_Di_Misura(ByVal piva As String, ByVal Elem_Cod As Integer, ByVal Mat_Cod As Integer, ByVal Sa_Cod As Integer) As RispostaStandard


        Dim cmbUdm As New DropDownList
        Dim Num_Totale As Integer = 0
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            'il cau_mov è quello del carico, così va a leggere in CategorieXUnitaMisura
            AgronicaCoreUtility.CaricaListControl.Udm_Optimize(cmbUdm,
                                                                Num_Totale,
                                                                piva,
                                                                Sa_Cod,
                                                                0,
                                                                CAU_CARICO,
                                                                Elem_Cod,
                                                                False,
                                                                0,
                                                                Mat_Cod,
                                                                True, "", 0,
                                                                "", "", "",
                                                                objParametriServer, objParametriUtenti)

            Dim udmList As New List(Of Object)

            For Each i As ListItem In cmbUdm.Items

                udmList.Add(New With
                             {
                                .udm_cod = i.Value,
                                .udm_des = i.Text
                            })

            Next

            r.RispostaStringa = JsonConvert.SerializeObject(udmList, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            'i18N Errore durante l'operazione da tradurre
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function CaricaGrigliaCalibri(ByVal piva As String, ByVal mat_cod As Integer, ByVal sa_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Dim dt As DataTable
            Dim ObjPQ As New AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_R

            dt = ObjPQ.Leggi_Calibri(piva,
                                     sa_cod,
                                     mat_cod,
                                     0,
                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                     "", "", objParametriServer)


            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            'i18N Errore durante l'operazione da tradurre
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    Public Shared Function CaricaGrigliaIndici(ByVal piva As String, ByVal mat_cod As Integer, ByVal sa_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            Dim dt As DataTable
            Dim ObjPQ As New AgronicaCoreAnagrafeDAL.Materie_Prime_PQ_R

            dt = ObjPQ.Leggi_Indici(piva,
                                     sa_cod,
                                     mat_cod,
                                     0,
                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                     "", "", objParametriServer)


            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            'i18N Errore durante l'operazione da tradurre
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    Public Shared Function Leggi_IndiciMaturita(ByVal Veg_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If
        Try
            'Veg_Cod = 31
            Dim chk As New CheckBoxList
            AgronicaCoreUtility.CaricaListControl.IndiciMaturita(chk,
                                                                False, "", "",
                                                                Veg_Cod,
                                                                True,
                                                                "", "", objParametriServer)

            Dim udm As New List(Of Object)

            For Each i As ListItem In chk.Items
                Dim val As Integer = i.Value.Split("|")(0)
                udm.Add(New With
                             {
                                .IND_MAT_COD = val,
                                .IND_MAT_DES = i.Text
                              })

            Next

            r.RispostaStringa = JsonConvert.SerializeObject(udm, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            'i18N Errore durante l'operazione da tradurre
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function PermessoModificaProdotto(ByVal piva As String, ByVal sa_cod As Integer) As RispostaStandard

        Dim result As New RispostaStandard

        Dim objParametriAgenda As New ParametriAgenda
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim possoModificare As Boolean = False
        Dim strRes As String = ""
        If sa_cod <> -1 Then
            possoModificare = True
        Else
            If piva = objParametriAgenda.Piva Then
                possoModificare = True
            Else
                strRes = AgronicaAgenda_2010.ProdottoCreatoDaUnAltraImpresa
            End If
        End If

        result.RispostaOK = True
        result.RispostaConferma = possoModificare
        result.RispostaStringa = strRes

        Return result

    End Function

End Class