Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreVarieBIZ
Imports System.Web.Services
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports Newtonsoft.Json

Public Class UtilityModificaChiavi
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Private Enum enum_Utility
        Cambio_Piva_Impresa = 1
        Cambio_CF_Piva_Contatto = 2
        Cambio_CF_Utente = 3
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim Type As String = ""

        Dim Qs_Piva As String = ""

        If Not String.IsNullOrWhiteSpace(Request.QueryString("Type")) Then
            Type = CStr(Request.QueryString("Type"))
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("p")) Then
            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
        End If

        hf_Piva.Value = Qs_Piva

        inizializzoObjParametri()

        verificaPermessi(Type)

    End Sub

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
    End Sub

    Private Sub verificaPermessi(ByVal Type As String)

        '==================================
        '======= VERIFICA PERMESSI ========
        '==================================

        Dim UtenteAbilitatoLettura_Cambio_Piva_Impresa As Boolean = False

        Dim UtenteAbilitatoScrittura_Cambio_Piva_Impresa As Boolean = False

        Dim UtenteAbilitatoLettura_Cambio_CF_Piva_Contatto As Boolean = False

        Dim UtenteAbilitatoScrittura_Cambio_CF_Piva_Contatto As Boolean = False

        Dim UtenteAbilitatoLettura_Cambio_CF_Utente As Boolean = False

        Dim UtenteAbilitatoScrittura_Cambio_CF_Utente As Boolean = False

        'Controllo se l'utente ha i permessi per accedere
        If Not IsNothing(Type) AndAlso Not String.IsNullOrEmpty(Type) Then

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            Select Case CInt(Type)

                Case enum_Utility.Cambio_Piva_Impresa

                    UtenteAbilitatoLettura_Cambio_Piva_Impresa = objPermessi.Controlla_Permessi_Utente(
                                                                                                        Session("ASG_Utente_Username"),
                                                                                                        Session("ASG_IdServizio"),
                                                                                                        enum_Security_Attivita.ManutenzioneArchivi_ModificaPIVA,
                                                                                                        enum_Security_Operazione.Lettura,
                                                                                                        Date.Now,
                                                                                                        "",
                                                                                                        objParametri_Utenti)

                    UtenteAbilitatoScrittura_Cambio_Piva_Impresa = objPermessi.Controlla_Permessi_Utente(
                                                                                                           Session("ASG_Utente_Username"),
                                                                                                           Session("ASG_IdServizio"),
                                                                                                           enum_Security_Attivita.ManutenzioneArchivi_ModificaPIVA,
                                                                                                           enum_Security_Operazione.Modifica,
                                                                                                           Date.Now,
                                                                                                           "",
                                                                                                           objParametri_Utenti)

                Case enum_Utility.Cambio_CF_Piva_Contatto

                    UtenteAbilitatoLettura_Cambio_CF_Piva_Contatto = objPermessi.Controlla_Permessi_Utente(
                                                                                                            Session("ASG_Utente_Username"),
                                                                                                            Session("ASG_IdServizio"),
                                                                                                            enum_Security_Attivita.ManutenzioneArchivi_ModificaCodContatto,
                                                                                                            enum_Security_Operazione.Lettura,
                                                                                                            Date.Now,
                                                                                                            "",
                                                                                                            objParametri_Utenti)

                    UtenteAbilitatoScrittura_Cambio_CF_Piva_Contatto = objPermessi.Controlla_Permessi_Utente(
                                                                                                               Session("ASG_Utente_Username"),
                                                                                                               Session("ASG_IdServizio"),
                                                                                                               enum_Security_Attivita.ManutenzioneArchivi_ModificaCodContatto,
                                                                                                               enum_Security_Operazione.Modifica,
                                                                                                               Date.Now,
                                                                                                               "",
                                                                                                               objParametri_Utenti)

                Case enum_Utility.Cambio_CF_Utente

                    UtenteAbilitatoLettura_Cambio_CF_Utente = objPermessi.Controlla_Permessi_Utente(
                                                                                                        Session("ASG_Utente_Username"),
                                                                                                        Session("ASG_IdServizio"),
                                                                                                        enum_Security_Attivita.Utility_Cambio_CF_Utente,
                                                                                                        enum_Security_Operazione.Lettura,
                                                                                                        Date.Now,
                                                                                                        "",
                                                                                                        objParametri_Utenti)

                    UtenteAbilitatoScrittura_Cambio_CF_Utente = objPermessi.Controlla_Permessi_Utente(
                                                                                                       Session("ASG_Utente_Username"),
                                                                                                       Session("ASG_IdServizio"),
                                                                                                       enum_Security_Attivita.Utility_Cambio_CF_Utente,
                                                                                                       enum_Security_Operazione.Modifica,
                                                                                                       Date.Now,
                                                                                                       "",
                                                                                                       objParametri_Utenti)

            End Select

        End If


        If (UtenteAbilitatoScrittura_Cambio_Piva_Impresa = False AndAlso
            UtenteAbilitatoScrittura_Cambio_CF_Piva_Contatto = False AndAlso
            UtenteAbilitatoScrittura_Cambio_CF_Utente = False) OrElse
            (Type <> "1" AndAlso Type <> "2" AndAlso Type <> "3") Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        hf_Type.Value = Type

        hf_UtenteAbilitatoLettura_Cambio_Piva_Impresa.Value = UtenteAbilitatoLettura_Cambio_Piva_Impresa

        hf_UtenteAbilitatoScrittura_Cambio_Piva_Impresa.Value = UtenteAbilitatoScrittura_Cambio_Piva_Impresa

        hf_UtenteAbilitatoLettura_Cambio_CF_Piva_Contatto.Value = UtenteAbilitatoLettura_Cambio_CF_Piva_Contatto

        hf_UtenteAbilitatoScrittura_Cambio_CF_Piva_Contatto.Value = UtenteAbilitatoScrittura_Cambio_CF_Piva_Contatto

        hf_UtenteAbilitatoLettura_Cambio_CF_Utente.Value = UtenteAbilitatoLettura_Cambio_CF_Utente

        hf_UtenteAbilitatoScrittura_Cambio_CF_Utente.Value = UtenteAbilitatoScrittura_Cambio_CF_Utente

    End Sub

#Region "TAB Utility Cambio Piva"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Impresa_con_PIVA_cambio_piva_UtilityModificaChiavi() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim List As New DropDownList
            AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(List, False,
                                                                         "", "", "", "",
                                                                         objParametriServer, objParametriUtenti)

            Dim ImpreseList As New List(Of Object)
            For i = 0 To List.Items.Count - 1

                Dim Piva As String = List.Items(i).Value

                ImpreseList.Add(New With
                                    {
                                         .Piva = List.Items(i).Value,
                                         .Rag_Soc_Piva = List.Items(i).Text.ToString() '& " ( Partita IVA : " & List.Items(i).Value.ToString() & " )"
                                    })

            Next
            r.RispostaStringa = JsonConvert.SerializeObject(ImpreseList, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function ControlliPreliminari_cambio_piva_UtilityModificaChiavi(ByVal Piva_Old As String, ByVal Piva_New As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim PivaPresente As Boolean

            Dim IsG2G As Boolean

            If Piva_New <> "" Then

                If Piva_New = Piva_Old Then
                    r.RispostaOK = False
                    r.Errore = "Impostare una Partita IVA diversa da quella attuale!"
                    Return r
                End If

                'PivaPresente = VerificaEsistenza_PivaGIAS(objParametriServer, Piva_New)

                'Controllo in tutte le tabelle se è già presente la PIVA

                Dim ObjAgroDatabaseUtility = New AgronicaCoreVarieDAL.System_Database_R

                Dim List_ObjParametri As New List(Of AgronicaCoreParametri) From {objParametriServer, objParametriUtenti}

                Dim NomeColonnaDaCercare As String = "Piva"

                Dim NomeTabellaPresenteValoreStringa As String = ""

                PivaPresente = ObjAgroDatabaseUtility.TrovaValoreStringa_FiltroNomeColonna(NomeColonnaDaCercare,
                                                                                            Piva_New,
                                                                                            NomeTabellaPresenteValoreStringa,
                                                                                            List_ObjParametri)

                If PivaPresente = True Then
                    r.RispostaOK = False
                    r.Errore = "La Partita IVA indicata è già presente ( " & NomeTabellaPresenteValoreStringa & " )!"
                    Return r
                End If

                Dim rgx As New Regex("^[a-zA-Z0-9-.]{1,25}$", RegexOptions.None, TimeSpan.FromSeconds(3))
                If rgx.IsMatch(Piva_New) = False Then
                    r.RispostaOK = False
                    r.Errore = "Occorre specificare solo lettere maiuscole, minuscole e numeri"
                    Return r
                End If

                Dim objimprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                IsG2G = objimprese.Azienda_G2G(Piva_Old, "", "", objParametriServer)

                If IsG2G = False Then
                    r.RispostaOK = True
                Else
                    If HttpContext.Current.Session("ASG_Utente_Username") = HttpContext.Current.Session("ASG_SuperUser_Username") Then
                        r.RispostaOK = True
                        r.RispostaStringa = "L'Azienda è oggetto di GIAS2GIAS! <br> Valutare se procedere con la modifica della partita iva. <br>"
                    Else
                        r.RispostaOK = False
                        r.Errore = "L'azienda è oggetto di Gias2Gias, pertanto non è possibile modificare la partita IVA."
                        Return r
                    End If
                End If

            Else
                r.RispostaOK = False
                r.Errore = "Inserire la Partita IVA nuova!"
                Return r
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_cambio_piva_UtilityModificaChiavi(ByVal Piva_Old As String, ByVal Piva_New As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objUtility_KeyTranslator As New AgronicaCoreVarieBIZ.Utility_KeyTranslator_W
        Dim risp As Boolean = False
        Dim flag_SuperUser As Boolean = False

        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If


        Try

            If Piva_Old = objParametriServer.PivaSuperUser Then

                flag_SuperUser = True

            End If

            risp = objUtility_KeyTranslator.KeyTranslator_PartitaIVA(Piva_Old,
                                                                    Piva_New,
                                                                    flag_SuperUser,
                                                                    objParametriServer,
                                                                    objParametriUtenti)

            If risp Then

                'Spostata la riassegnazione della PivaSuperUser per poter aggiornare anche i campi con il cod_contatto
                If flag_SuperUser Then

                    objParametriServer.PivaSuperUser = Piva_New
                    objParametriServer.UtenteCodFiscale = Piva_New

                    HttpContext.Current.Session("ASG_SuperUser_CodFiscale") = Piva_New

                    HttpContext.Current.Session("ASG_objParametri_Server") = objParametriServer

                    objParametriUtenti.PivaSuperUser = Piva_New
                    objParametriUtenti.UtenteCodFiscale = Piva_New

                    HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametriUtenti

                End If

                r.RispostaStringa = "Modifica effettuata con successo! <br> Sono stati sostituiti tutti i record!"

            End If

            r.RispostaOK = risp

            '------------------------------------------------
            '------------------------------------------------
            '------------------------------------------------

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

#End Region

#Region "TAB Utility Cambio CF/Piva Contatto"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Impresa_con_contatto_cambio_CF_piva_contatto_UtilityModificaChiavi() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim SuperUser_CodFiscale = HttpContext.Current.Session("ASG_SuperUser_CodFiscale")

            Dim dt As DataTable
            Dim obj As New AgronicaCoreAnagrafeDAL.Contatti_R

            dt = obj.Contatti_Imprese_Leggi(SuperUser_CodFiscale,
                                           "", "",
                                           objParametriServer)

            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Contatto_cambio_CF_piva_contatto_UtilityModificaChiavi(ByVal Piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            'Leggo solo la tabella Contatti,
            'perchè se utilizzo CaricaListControl.Contatti_2 ci potrebbero essere più righe per ogni contatto in base al rapporto contabile.

            Dim DT_Contatti As DataTable

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

            DT_Contatti = objContatti.LeggiDatiMinimi(Piva,
                                                        "", "", objParametriServer)


            For Each row In DT_Contatti.Rows


                Select Case CStr(row("Cod_Contatto")).Length

                    Case 11
                        row("Rag_Soc") += " ( Partita IVA : " + row("Cod_Contatto").ToString() + " )"

                    Case 16
                        row("Rag_Soc") += " ( Codice Fiscale : " + row("Cod_Contatto").ToString() + " )"

                End Select


            Next

            r.RispostaStringa = JsonConvert.SerializeObject(DT_Contatti, Newtonsoft.Json.Formatting.None)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ControlliPreliminari_cambio_CF_piva_contatto_UtilityModificaChiavi(ByVal Piva As String, ByVal CodContatto_Old As String, ByVal CodContatto_New As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim Cod_Contatto_Presente As Boolean
            Dim Piva_GIAS As Boolean

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

            Cod_Contatto_Presente = objContatti.VerificaEsistenza_CodContatto(Piva,
                                                                              CodContatto_New,
                                                                              objParametriServer)

            If Cod_Contatto_Presente = True Then
                r.RispostaOK = False
                r.Errore = "Impossibile procedere con la modifica, la nuova Partita IVA/Codice Fiscale è già presente in archivio!"
                Return r
            End If

            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read

            Piva_GIAS = objImprese.VerificaEsistenza_PivaGIAS(CodContatto_Old,
                                                              objParametriServer)

            If Piva_GIAS = True Then
                r.RispostaOK = False
                r.Errore = "Questo contatto è un'impresa GIAS, per modificare la Partita IVA utilizzare l'Utility Cambio PIVA."
                Return r
            End If

            Dim CodContattoNew As String = CodContatto_New

            Dim rgx As New Regex("^[a-zA-Z0-9-.]{1,25}$", RegexOptions.None, TimeSpan.FromSeconds(3))
            If rgx.IsMatch(CodContattoNew) = False Then
                r.RispostaOK = False
                r.Errore = "Occorre specificare solo lettere maiuscole, minuscole e numeri"
                Return r
            End If


            Select Case CStr(CodContatto_Old).Length

            '######################################################

                Case 11

                    If CodContatto_New <> "" Then

                        'VERIFICA IL NUOVO COD_CONTATTO IMMESSO
                        Select Case CStr(CodContatto_New).Length

                            Case 11

                                r.RispostaOK = True
                                r.RispostaStringa = "Sei sicuro di voler modificare la Partita IVA?"

                            Case 16

                                If CStr(CodContatto_Old).StartsWith("-") Or (CStr(CodContatto_Old).StartsWith("F") And IsNumeric(Mid(CodContatto_Old, 2, CodContatto_Old.Length)) = True) Then

                                    r.RispostaOK = True
                                    r.RispostaStringa = "Sei sicuro di voler modificare la Partita IVA?"

                                Else
                                    r.RispostaOK = True
                                    r.RispostaStringa = "Sei sicuro di voler modificare la Partita IVA lunga 11 caratteri con una lunga 16?"

                                End If


                            Case Else
                                r.RispostaOK = True
                                r.RispostaStringa = "Sei sicuro di voler modificare la Partita IVA lunga 11 caratteri con una lunga " & CStr(CodContattoNew).Length.ToString & "?"

                        End Select


                    Else
                        r.RispostaOK = False
                        r.Errore = "Occorre inserire la nuova Partita IVA/Codifica Fiscale"
                        Return r
                    End If


                '######################################################

                Case 16
                    '---- CODICE FISCALE


                    If CodContatto_New <> "" Then

                        Select Case CStr(CodContatto_New).Length

                            Case 11

                                r.RispostaOK = True
                                r.RispostaStringa = "Sei sicuro di voler modificare un CF lungo 16 chr con uno lungo 11 chr?"

                            Case 16

                                If VerificaEspressioneRegolare(CodContatto_New, "", Input_Controllato.enum_EspressioniRegolari.RegExp_CodiceFiscale) = True Then

                                    r.RispostaOK = True
                                    r.RispostaStringa = "Sei sicuro di voler modificare il Codice Fiscale?"

                                Else

                                    r.RispostaOK = True
                                    r.RispostaStringa = "Sei sicuro di voler modificare il CF con uno che non è valido? (non rispetta le caratteristiche di un CF)"

                                End If


                            Case Else

                                r.RispostaOK = True
                                r.RispostaStringa = "Sei sicuro di voler modificare un CF lungo 16 chr con uno lungo " & CStr(CodContatto_New).Length.ToString & " chr?"

                        End Select

                    Else
                        r.RispostaOK = False
                        r.Errore = "Inserire il nuovo Codice Fiscale!"
                        Return r
                    End If


                '######################################################

                Case 0
                    '----- ERRORE
                    r.RispostaOK = False
                    r.Errore = "Selezionare un Contatto!"
                    Return r


                Case Else
                    r.RispostaOK = True
                    r.RispostaStringa = "Sei sicuro di voler modificare il codice contatto?"

            End Select


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_cambio_CF_piva_contatto_UtilityModificaChiavi(ByVal Piva As String, ByVal CodContatto_Old As String, ByVal CodContatto_New As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objUtility_KeyTranslator As New AgronicaCoreVarieBIZ.Utility_KeyTranslator_W
            Dim risp As Boolean

            risp = objUtility_KeyTranslator.KeyTranslator_CodContatto2(Piva,
                                                                        CodContatto_Old,
                                                                        CodContatto_New,
                                                                        objParametriServer)

            If risp Then
                r.RispostaStringa = "Modifica effettuata con successo! <br> Sono stati sostituiti tutti i record!"
            End If

            r.RispostaOK = risp

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

#End Region

#Region "TAB Utility Cambio CF Utente"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Username_con_Codice_Fiscale_cambio_CF_utente_UtilityModificaChiavi() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim dt As DataTable
            Dim obj As New AgronicaCoreUtentiDAL.Utenti_Read

            dt = obj.Leggi2("",
                           "", "",
                           objParametriUtenti)

            dt.Columns.Add("UserName_CodFisc", GetType(String))

            For Each row As DataRow In dt.Rows

                '----- Persona Giuridica
                If CInt(row("Flag_Azienda_Persona").ToString()) = 1 Then

                    row("UserName_CodFisc") = row("Username").ToString() & " ( Partita IVA : " & row("CodFisc").ToString() & " )"

                    '----- Persona Fisica
                ElseIf CInt(row("Flag_Azienda_Persona").ToString()) = 2 Then

                    row("UserName_CodFisc") = row("Username").ToString() & " ( Codice Fiscale : " & row("CodFisc").ToString() & " )"

                End If

            Next


            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function ControlliPreliminari_cambio_CF_utente_UtilityModificaChiavi(ByVal Codice_Fiscale_Old As String, ByVal Codice_Fiscale_New As String, ByVal Flag_Azienda_Persona As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objUtenti_Dettagli_R As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R

            If Codice_Fiscale_Old = objParametri_Server.PivaSuperUser Then
                r.RispostaOK = False
                r.Errore = "Non è possibile modificare la Partita IVA del superuser!"
                Return r
            End If

            If Codice_Fiscale_New.Contains(" ") Or Codice_Fiscale_New = "" Then
                r.RispostaOK = False
                If Flag_Azienda_Persona = 1 Then
                    r.Errore = "Partita IVA nuova non valida!"
                ElseIf Flag_Azienda_Persona = 2 Then
                    r.Errore = "Codice Fiscale nuovo non valido!"
                End If
                Return r
            End If


            Select Case Flag_Azienda_Persona
                Case 1
                    '----- Persona Giuridica

                    If VerificaEspressioneRegolare(Codice_Fiscale_New, "", enum_EspressioniRegolari.RegExp_PartitaIVA) = False Then
                        r.RispostaOK = False
                        r.Errore = "Partita IVA nuova ha un formato non corretto!"
                        Return r
                    End If

                Case 2
                    '----- Persona Fisica

                    If VerificaEspressioneRegolare(Codice_Fiscale_New, "", enum_EspressioniRegolari.RegExp_CodiceFiscale) = False Then
                        r.RispostaOK = False
                        r.Errore = "Codice Fiscale nuovo ha un formato non corretto!"
                        Return r
                    End If

            End Select


            Dim dt = objUtenti_Dettagli_R.Utenti_Dettagli_from_CF(Codice_Fiscale_New, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            If dt.Rows.Count > 0 Then
                r.RispostaOK = False
                If Flag_Azienda_Persona = 1 Then
                    r.Errore = "Partita IVA nuova già presente in archivio!"
                ElseIf Flag_Azienda_Persona = 2 Then
                    r.Errore = "Codice Fiscale nuovo già presente in archivio!"
                End If
                Return r
            End If

            dt = objUtenti_Dettagli_R.Utenti_Dettagli_from_CF(Codice_Fiscale_Old, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            If dt.Rows.Count = 0 Then
                r.RispostaOK = False
                If Flag_Azienda_Persona = 1 Then
                    r.Errore = "Partita IVA vecchia non presente In archivio!"
                ElseIf Flag_Azienda_Persona = 2 Then
                    r.Errore = "Codice Fiscale vecchio non presente In archivio!"
                End If
                Return r
            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_cambio_CF_utente_UtilityModificaChiavi(ByVal Codice_Fiscale_Old As String, ByVal Codice_Fiscale_New As String, ByVal Flag_Azienda_Persona As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objUtility_KeyTranslator As New AgronicaCoreVarieBIZ.Utility_KeyTranslator_W

            r = objUtility_KeyTranslator.UtilityCambioCFUtente(Codice_Fiscale_Old, Codice_Fiscale_New, Flag_Azienda_Persona,
                                                                objParametriServer, objParametriUtenti)

            'Se si sta cercando di cambiare il Codice Fiscale dell'utente con cui si è entrati allora aggiorno il nuovo codice fiscale anche 
            'nella sessione
            If r.RispostaOK AndAlso
                objParametriServer.UsernameOperazione = Codice_Fiscale_Old AndAlso
                objParametriUtenti.UsernameOperazione = Codice_Fiscale_Old AndAlso
                objParametriServer.UtenteCodFiscale = Codice_Fiscale_Old AndAlso
                objParametriUtenti.UtenteCodFiscale = Codice_Fiscale_Old Then

                objParametriServer.UsernameOperazione = Codice_Fiscale_New
                objParametriServer.UtenteCodFiscale = Codice_Fiscale_New

                HttpContext.Current.Session("ASG_objParametri_Server") = objParametriServer

                objParametriUtenti.UsernameOperazione = Codice_Fiscale_New
                objParametriUtenti.UtenteCodFiscale = Codice_Fiscale_New

                HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametriUtenti

                r.ParametroDue = True

                r.ParametroDue_stringa = "Uscire e rientrare dal GIAS per completare l'operazione."

            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

#End Region

End Class