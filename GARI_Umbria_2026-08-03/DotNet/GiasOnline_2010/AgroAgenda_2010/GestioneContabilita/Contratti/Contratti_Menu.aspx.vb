

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreXML.XML_Stampe
Imports AgronicaCoreVarieBIZ

Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports Newtonsoft.Json

Public Class Contratti_Menu
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub



#Region "script services leggi CDG"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Trova_Impianti(ByVal piva As String, ByVal filtro_centro As String, ByVal filtro_specie As String, ByVal filtro_terrenonudo As String, ByVal filtro_varieta As String, ByVal id_agenda_cdg As Integer, ByVal data_movimento As String, ByVal split As Integer, ByVal filtro_codice_appezzamento As String, ByVal filtro_codice_impianto As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim leggi As New CDG_DAL_R


        Try

            'Formattazione Filtri
            If Trim(filtro_centro) = "" Then
                filtro_centro = "0"
            Else
                If Right(" " & filtro_centro, 1) = "," Then
                    filtro_centro = Left(filtro_centro, Len(filtro_centro) - 1)
                End If

            End If

            If Trim(filtro_specie) = "" Then
                filtro_specie = "0"
            Else
                If Right(" " & filtro_specie, 1) = "," Then
                    filtro_specie = Left(filtro_specie, Len(filtro_specie) - 1)
                End If

            End If

            If Trim(filtro_terrenonudo) = "" Then
                filtro_terrenonudo = "0"
            Else
                If Right(" " & filtro_terrenonudo, 1) = "," Then
                    filtro_terrenonudo = Left(filtro_terrenonudo, Len(filtro_terrenonudo) - 1)
                End If

            End If

            If Trim(filtro_varieta) = "" Then
                filtro_varieta = "0"
            Else
                If Right(" " & filtro_varieta, 1) = "," Then
                    filtro_varieta = Left(filtro_varieta, Len(filtro_varieta) - 1)
                End If

            End If

            If Trim(filtro_codice_appezzamento) = "" Then
                filtro_codice_appezzamento = "0"
            Else
                If Right(" " & filtro_codice_appezzamento, 1) = "," Then
                    filtro_codice_appezzamento = Left(filtro_codice_appezzamento, Len(filtro_codice_appezzamento) - 1)
                End If

            End If

            If Trim(filtro_codice_impianto) = "" Then
                filtro_codice_impianto = "0"
            Else
                If Right(" " & filtro_codice_impianto, 1) = "," Then
                    filtro_codice_impianto = Left(filtro_codice_impianto, Len(filtro_codice_impianto) - 1)
                End If

            End If

            If data_movimento = "" Then
                data_movimento = AGRODATAINIZIO
            End If

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dt = leggi.Trova_Impianti(piva, filtro_centro, filtro_specie, filtro_terrenonudo, filtro_varieta, id_agenda_cdg, data_movimento, split, filtro_codice_appezzamento, filtro_codice_impianto, objParametri_Server)


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r


    End Function





    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Contratti_Anno(ByVal piva As String, ByVal cau_contratto As String, ByVal contratto_numero As String, ByVal cod_contatto As String, ByVal veg_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim leggi As New AgronicaCoreContabDAL.Imprese_Contratti_R
        Dim Filtro_Aggiuntivo As String = ""


        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dt = leggi.Leggi_Anno(piva, 0, contratto_numero, cau_contratto, cod_contatto, 0, veg_cod, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, Filtro_Aggiuntivo, "Anno Desc", objParametri_Server)


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r


    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Contratti_Conferenti(ByVal piva As String, ByVal tipo_rapporto As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim leggi As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
        Dim Filtro_Aggiuntivo As String = ""


        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If


            Dt = leggi.LeggiRapportoSpecificoxDocumenti(piva, False, tipo_rapporto, True, "", Filtro_Aggiuntivo, "Rag_Soc_Contatto", objParametri_Server)

            If Dt.Rows.Count > 0 Then

                For Each dr As DataRow In Dt.Rows

                    If dr("Settore_Des") <> "" And dr("Settore_Des") <> "0" Then

                        dr("Rag_Soc_Contatto") = dr("Rag_Soc_Contatto") & " --  " & dr("Settore_Des")

                    End If


                Next

            End If


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r


    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Contratti_Prodotti(ByVal piva As String, ByVal veg_cod As Integer, ByVal elem_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim leggi As New AgronicaCoreContabDAL.Imprese_Contratti_R
        Dim Filtro_Aggiuntivo As String = ""
        Dim Dt_Filtro As DataTable
        Dim leggi_filtro As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R


        Try

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

            Dt_Filtro = leggi_filtro.Leggi(enum_Impostazioni_Utenti.SuperUser_FiltroSQL_MateriePrime, enum_AreaGIAS.Conferimento, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            If Dt_Filtro.Rows.Count > 0 Then

                Filtro_Aggiuntivo = Dt_Filtro(0)("Str_0")

            End If



            Dt = leggi.Leggi_Prodotti(piva, "", 0, veg_cod, elem_cod, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, Filtro_Aggiuntivo, "", objParametri_Server)


            If Dt.Rows.Count > 0 Then

                For Each dr As DataRow In Dt.Rows

                    If dr("Cod_Articolo") <> "" Then

                        dr("Mat_Des") = dr("Cod_Articolo") & " " & dr("Mat_Des")

                    End If

                Next

            End If


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r


    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Contratti(ByVal piva As String, ByVal cau_contratto As String, ByVal contratto_numero As String, ByVal filtro_anno As String, ByVal filtro_conferente As String, ByVal filtro_prodotto As String, ByVal veg_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim leggi As New AgronicaCoreContabDAL.Imprese_Contratti_R
        Dim Filtro_Aggiuntivo As String = ""
        Dim Arrayp() As String
        Dim i As Integer
        Dim Validita_Inizio As Date = AGRODATAINIZIO
        Dim Validita_Fine As Date = AGRODATAFINE


        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            'Impostazione Filtro Aggiuntivo
            If Trim(filtro_anno) <> "" Then
                Arrayp = Split(filtro_anno, ",")
                
                For indice = 0 To UBound(Arrayp)

                    Validita_Inizio = "01/01/" & Arrayp(indice)
                    Validita_Fine = "31/12/" & Arrayp(indice)

                    Filtro_Aggiuntivo = Filtro_Aggiuntivo & IIf(Filtro_Aggiuntivo <> "", " Or ", "") & "(Imprese_Contratti.Validita_Inizio >= " & Agro_SQL_SaveDate(Validita_Inizio) & " And Imprese_Contratti.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & ")"

                Next

                Filtro_Aggiuntivo = "(" & Filtro_Aggiuntivo & ")"

            End If

            If Trim(filtro_conferente) <> "" Then
                Filtro_Aggiuntivo = Filtro_Aggiuntivo & IIf(Filtro_Aggiuntivo <> "", " And ", "") & " (Imprese_Contratti.Cod_Risum In (" & filtro_conferente & "))"
            End If

            If Trim(filtro_prodotto) <> "" Then
                Filtro_Aggiuntivo = Filtro_Aggiuntivo & IIf(Filtro_Aggiuntivo <> "", " And ", "") & " (Imprese_Contratto_Fasi.Mat_Cod In (" & filtro_prodotto & "))"
            End If



            Dt = leggi.Leggi(piva, -1, contratto_numero, cau_contratto, "", 0, veg_cod, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, Filtro_Aggiuntivo, "", objParametri_Server)


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r


    End Function




    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Clausole(ByVal piva As String, ByVal cau_contratto As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim leggi As New AgronicaCoreContabDAL.Imprese_Contratti_Clausole_R
        Dim Filtro_Aggiuntivo As String = ""

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dt = leggi.Leggi(piva, 0, cau_contratto, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, Filtro_Aggiuntivo, "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r


    End Function

#End Region



#Region "script services aggiorna CDG"

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaClausole(ByVal piva As String,
                                           ByVal righeInseriteGrid_Clausole As String,
                                           ByVal righeModificateGrid_Clausole As String,
                                           ByVal righeCancellateGrid_Clausole As String
                                           ) As RispostaStandard

        '      



        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try


            Dim scrivi As New AgronicaCoreContabBIZ.Imprese_Contratti_BIZ_W
            Dim Dummy As Integer


            Dummy = scrivi.AggiornaClausole(piva,
                                            AGRODATAINIZIO,
                                            AGRODATAFINE,
                                            righeInseriteGrid_Clausole,
                                            righeModificateGrid_Clausole,
                                            righeCancellateGrid_Clausole,
                                            objParametri_Server)


            r.RispostaStringa = CStr(Dummy)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=False)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Delete_Contratto(ByVal piva As String,
                                            ByVal contratto_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim xRisp As Boolean = False
        Dim msgError As String = ""
        Dim DT As New DataTable

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim objContratto As New AgronicaCoreContabBIZ.Imprese_Contratti_BIZ_W

            'Cancellazione
            contratto_cod = objContratto.Cancella("", contratto_cod, "", objParametriServer)


            'Cancellazione Ok
            r.Errore = ""



            '    Case Else

            '        'Cancellazione Negata
            '        r.Errore = "-1"

            'End Select

            r.RispostaStringa = CStr(contratto_cod)
            r.RispostaOK = True



        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = 0

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=False)
        End Try

        Return r

    End Function


#End Region


#Region "Caricamento"

    Private Sub caricaControlli()


        'End Select
    End Sub

#End Region




    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master().Lbl_Titolo.Text = "Gestione Contratti Conferimento"
        'Master.flag_pag_GestioneCompletaCdG = True

        inizializzoObjParametri()
        inizializzoParametriPagina()

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Gestione_Contratti_Conferimento,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Gestione_Contratti_Conferimento,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        If UtenteAbilitatoLettura = False Then
            Response.Redirect("~/Menu/MenuBS_Agenda_Nuovo.aspx")
        End If

        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                          AgroKey_EncoderDecoder,
                                          Server)


        hdPiva_Codificata.Value = Stringa_Codifica(hdPiva.Value, AgroKey_EncoderDecoder, HttpContext.Current.Session)

        If Not Request.QueryString("tipo") Is Nothing Then
            hdCau_Contratto.Value = "9300" ' Request.QueryString("tipo")
        End If


        Dim PaginaRedirect As String = ""

        hdPaginaRedirect.Value = ""
        If Not Request.QueryString("origine") Is Nothing Then

            hdPaginaRedirect.Value = Stringa_Decodifica(CStr(Request.QueryString("origine")),
                                            AgroKey_EncoderDecoder,
                                            Server)
        Else
            hdPaginaRedirect.Value = "../Menu/MenuBS_Agenda_Nuovo.aspx"
        End If

        'If Not Page.IsPostBack Then
        '    'caricaControlli()
        'End If

        If Not IsNothing(Session("ParametriAgenda_2010")) Then

            Dim objParametriAgenda_2010 As New ParametriAgenda_2010
            objParametriAgenda_2010.Leggi()

        End If


        Master.flag_MostraBtnIndietro = False

        Master.flag_MostraBtnEsci = False






    End Sub


    Private Sub inizializzoParametriPagina()


    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

End Class