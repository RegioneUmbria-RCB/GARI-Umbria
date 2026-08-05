Imports System.Web
Imports System.Web.Services

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreContabDAL

Imports AgronicaControlli_2010

Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreVarieBIZ


Public Class ManutenzioneMacchine
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda
    Dim TipoOperazioneAgenda As enum_Tipo_Operazione_Agenda

    '----- Gestione Querystring
    Dim Qs_Data As String
    Dim Qs_Operazione As String
    Dim Qs_Lav_Cod As String
    Dim Qs_Mac_Cod As String
    Dim Qs_IdAgenda As String

    Public jsMacchine As String

    Dim DT_mac As DataTable


    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Salva_Macchine_X_Salva_Tutto(ByVal dati As String, aggiorna_date As Boolean) As String

        Dim Dt_Macchine As DataTable
        Dim Dt_checked As DataTable

        Dt_Macchine = HttpContext.Current.Session("Dt_Macchine")

        Dt_checked = Dt_Macchine.Clone

        Dim app_chk As String() = dati.Split(New Char() {"|"c})

        ' Per ogni chiave di riga di Appezzamento checked
        For Each chiave As String In app_chk

            ' Ciclo su DT di tutti gli Appezzamenti disponibili per ritrovare la riga
            For i = 0 To Dt_Macchine.Rows.Count - 1

                If chiave <> "" Then

                    ' Controllo se le chiavi coincidono
                    If chiave = Dt_Macchine.Rows(i).Item("Mac_Cod") Then

                        ' copio la riga intera
                        Dt_checked.ImportRow(Dt_Macchine.Rows(i))
                    End If

                End If
            Next

        Next

        HttpContext.Current.Session("aggiornaDateMacchine") = aggiorna_date
        HttpContext.Current.Session("Macchine_checked") = Dt_checked

        Return "ok"

    End Function


    '#########################################################################################
    '  Galassi, 15/11/2016 13:07:16: Eliminato il caricamento della WATable tramite variabile e inserito come Webservice
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Macchine() As RispostaStandard

        Dim objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim objParametriAgenda As New ParametriAgenda


        Dim objMacc As New AgronicaCoreContabDAL.Parco_Macchine_R

        Dim risp As New RispostaStandard()


        Dim DT_mac = HttpContext.Current.Session("Dt_Macchine")

        DT_mac = objMacc.ParcoMacchine_Leggi(objParametriAgenda.Piva, _
                                                         objParametriAgenda.Mac_Cod, _
                                                         False, _
                                                         "", "", "", "", "", 0, _
                                                         "", False, 0, "", True, _
                                                         AGRODATAINIZIO, _
                                                         AGRODATAFINE, _
                                                         enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                         "", "", _
                                                         objParametri_Server)


        DT_mac.Columns.Add(New DataColumn("checked", GetType(Integer)))

        Select Case objParametriAgenda.Tipo_Operazione
            Case enum_TipoOperazioneDB.Scrittura
                If HttpContext.Current.Session("UtenteAbilitato_Modifica") = False Then
                    risp.RispostaOK = False
                    risp.Errore = "Utente non abilitato in Scrittura"
                End If
                ' @Paolo: setto il check di riga a 0 
                For i = 0 To DT_mac.Rows.Count - 1
                    DT_mac.Rows(i).Item("checked") = 0
                Next
            Case enum_TipoOperazioneDB.Lettura
                If HttpContext.Current.Session("UtenteAbilitato_Lettura") = False Then
                    risp.RispostaOK = False
                    risp.Errore = "Utente non abilitato in Lettura"
                End If

                If Not IsNothing(HttpContext.Current.Session("frm_MatCod")) Then

                    Dim frm_MatCod = HttpContext.Current.Session("frm_MatCod")


                    ' Ciclo le righe checked
                    For j = 0 To frm_MatCod.Count - 1

                        ' Ciclo le righe della tabella
                        For i = 0 To DT_mac.Rows.Count - 1

                            If frm_MatCod(j) = DT_mac.Rows(i).Item("Mac_Cod") Then
                                DT_mac.Rows(i).Item("checked") = 1
                            End If

                        Next

                    Next
                Else
                    risp.RispostaOK = False
                    risp.Errore = "Errore durante il caricamento della pagina. Uscire e riprovare."
                    Return risp
                End If
            Case enum_TipoOperazioneDB.Modifica, enum_TipoOperazioneDB.Copia

                If HttpContext.Current.Session("UtenteAbilitato_Modifica") = False Then
                    risp.RispostaOK = False
                    risp.Errore = "Utente non abilitato in Modifica"
                End If

                If Not IsNothing(HttpContext.Current.Session("frm_MatCod")) Then

                    Dim frm_MatCod = HttpContext.Current.Session("frm_MatCod")


                    ' Ciclo le righe checked
                    For j = 0 To frm_MatCod.Count - 1

                        ' Ciclo le righe della tabella
                        For i = 0 To DT_mac.Rows.Count - 1

                            If frm_MatCod(j) = DT_mac.Rows(i).Item("Mac_Cod") Then
                                DT_mac.Rows(i).Item("checked") = 1
                            End If

                        Next

                    Next
                Else
                    risp.RispostaOK = False
                    risp.Errore = "Errore durante il caricamento della pagina. Uscire e riprovare."
                    Return risp
                End If

            Case Else
                risp.RispostaOK = False
                risp.Errore = "Errore: Casistica non gestita."
                Return risp
        End Select


        risp.RispostaOK = True
        risp.RispostaStringa = DT_to_Json_Macchine(DT_mac)
        HttpContext.Current.Session("Dt_Macchine") = DT_mac
        Return risp
    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function DT_to_Json_Macchine(ByVal dt As DataTable) As String
        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)


        Dim cn As New ColonneNome("Mac_Cod", "chiave", "string")
        cn._Filtrabile = False
        cn._ColonnaDiSelezione = True
        cn._hidden = True
        cn._css = "pos_chiave"
        'cn._css = "prova"
        l.Add(cn)


        cn = New ColonneNome("Class_Desc", "Classe", "string")
        cn._Filtrabile = True
        cn._css = "pos_prov"
        l.Add(cn)

        cn = New ColonneNome("Mac_Des", "Macchina", "string")
        cn._Filtrabile = True
        cn._css = "pos_com"
        l.Add(cn)

        cn = New ColonneNome("Modello", "Modello", "string")
        cn._Filtrabile = True
        cn._css = "pos_sez"
        l.Add(cn)

        cn = New ColonneNome("Targa", "Targa", "string")
        cn._Filtrabile = True
        cn._css = "pos_fogl"
        'cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("Impresa", "Impresa Ref.", "string")
        cn._Filtrabile = True
        cn._css = "pos_num"
        'cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("checked", "checked", "string")
        'cn._Filtrabile = True
        cn._css = "checked"
        cn._hidden = True
        l.Add(cn)


        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master.flag_pag_Anagrafica = True

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---

        objParametriAgenda = New ParametriAgenda

        Dim id_agenda As String
        Dim op As Integer

        If InStr(Request.QueryString.ToString, "id_agenda") <> 0 AndAlso Request.QueryString("id_agenda").ToString <> "" Then

            id_agenda = Stringa_Decodifica(Request.QueryString("id_agenda"), AgroKey_EncoderDecoder, Server)

            If InStr(Request.QueryString.ToString, "op") <> 0 Then

                op = CInt(Stringa_Decodifica(Request.QueryString("op"), AgroKey_EncoderDecoder, Server))

                Select Case op
                    Case 2
                        TipoOperazioneAgenda = 2
                        objParametriAgenda.Tipo_Operazione = 2

                    Case 3
                        TipoOperazioneAgenda = 3
                        objParametriAgenda.Tipo_Operazione = 3

                End Select


            End If
            objParametriAgenda.Id_Agenda = id_agenda
        Else
            TipoOperazioneAgenda = objParametriAgenda.Tipo_Operazione
        End If

        'If InStr(Request.QueryString.ToString, "&d=") <> 0 Then

        '    Qs_Data = Stringa_Decodifica(Request.QueryString("d").ToString, _
        '                      AgroKey_EncoderDecoder, _
        '                      Server)
        'Else
        '    Qs_Data = CStr(Date.Today)
        'End If

        'If InStr(Request.QueryString.ToString, "&l=") <> 0 Then

        '    Qs_Lav_Cod = Stringa_Decodifica(Request.QueryString("l").ToString, _
        '                          AgroKey_EncoderDecoder, _
        '                          Server)

        'Else
        '    Qs_Lav_Cod = 0
        'End If

        'If InStr(Request.QueryString.ToString, "&m=") <> 0 Then

        '    Qs_Mac_Cod = Stringa_Decodifica(Request.QueryString("m").ToString, _
        '                 AgroKey_EncoderDecoder, _
        '                 Server)

        'Else
        '    Qs_Mac_Cod = 0
        'End If

        'If InStr(Request.QueryString.ToString, "&i=") <> 0 Then

        '    Qs_IdAgenda = Stringa_Decodifica(Request.QueryString("i").ToString, _
        '                 AgroKey_EncoderDecoder, _
        '                 Server)

        'Else
        '    Qs_IdAgenda = 0
        'End If




        If Not IsPostBack Then

            '##############################################################
            '#####  Inizializzo i controlli  ##############################
            '##############################################################

            Txt_DataIntervento.Text = objParametriAgenda.Data

            Select Case CInt(objParametriAgenda.Lav_Cod)

                Case LAVCOD_REVISIONE_MACCHINE

                    LblLavorazione.Text = "Revisione del:"
                    Master.Lbl_Titolo.Text = "Revisione Macchine / Attrezzature"
                    'lbl_TitoloGriglia.Text = "Macchine e Attrezzature Aziendali non dismesse:"
                    'lbl_Cerca_MacchinaImpianto.Text = "Cerca la Macchina/Attrezzatura:"
                    Lbl_CentroRevisione.Text = "Centro Revisione:"

                    'frm_Lavorazione_IdAttivita = CAU_MANUTENZIONE_PARCOMACCHINE
                    'frm_Lavorazione_ElementoGestito = 1

                Case LAVCOD_MANUTENZIONE_MACCHINE

                    LblLavorazione.Text = "Manutenzione del:"
                    Master.Lbl_Titolo.Text = "Manutezione Macchine / Attrezzature"
                    'lbl_TitoloGriglia.Text = "Macchine e Attrezzature Aziendali non dismesse:"
                    'lbl_Cerca_MacchinaImpianto.Text = "Cerca la Macchina/Attrezzatura:"
                    Lbl_CentroRevisione.Text = "Centro Manutenzione :"

                    'frm_Lavorazione_IdAttivita = CAU_MANUTENZIONE_PARCOMACCHINE
                    'frm_Lavorazione_ElementoGestito = 1

                Case LAVCOD_MANUTENZIONE_IMPIANTI

                    LblLavorazione.Text = "Manutezione Impianti del:"
                    Master.Lbl_Titolo.Text = "Manutezione Impianti"
                    'lbl_TitoloGriglia.Text = "Impianti Colturali:"
                    'lbl_Cerca_MacchinaImpianto.Text = "Cerca l'Impianto:"

                    'frm_Lavorazione_IdAttivita = CAU_MANUTENZIONE_impianti
                    'frm_Lavorazione_ElementoGestito = 1

            End Select

            '==========================================
            '===== Pagina caricata per la prima volta
            '==========================================

            '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina
            VerificaPermessi()

            'Dim objContatti As New AgronicaCoreUtility.CaricaListControl
            'objContatti.Contatti(Cmb_CentroRevisione, True, "Seleziona", "", objParametriAgenda.Piva, 0, 0, -7, True, False, 0, 0, False, 0, 0, "", "", objParametri_Utenti)



            'For i = 0 To DT.Rows.Count - 1
            '    Dim flag_str As String = ""

            '    flag_str = "Classe:'" & DT.Rows(i).Item("CLASS_DESC").ToUpper() & "'; Macchina:'" & DT.Rows(i).Item("Mac_Des").ToUpper() & "'; Modello:'" & DT.Rows(i).Item("Modello").ToUpper() & "'; Targa:'" & DT.Rows(i).Item("Targa").ToUpper() & "'; Impresa Ref.:'" & DT.Rows(i).Item("Impresa").ToUpper() & "'"

            '    Cmb_Macchine.Items.Add(New ListItem(flag_str, DT.Rows(i).Item("Mac_Cod")))
            'Next


        Else

            Exit Sub

        End If


        ''' INIZIALIZZO I CAMPI
        AgronicaCoreUtility.CaricaListControl.Contatti(Cmb_CentroRevisione, True, "SELEZIONA", "", _
                                                         objParametriAgenda.Piva, _
                                                         "", 0, -7, _
                                                         True, False, 0, 0, False, 0, ID_CF_NOFILTRO, _
                                                         "", _
                                                         "", _
                                                         objParametri_Server)



        ''''''''''''''''''''''''''''''''''''''''''''

        Select Case objParametriAgenda.Tipo_Operazione
            Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura

                'TO DO

            Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura, TipiEnumerativi.enum_TipoOperazioneDB.Modifica

                HttpContext.Current.Session("frm_MatCod") = ""

                Ripristina_Dati_nei_Controlli()

            Case TipiEnumerativi.enum_TipoOperazioneDB.Cancellazione
                'TO TEST
                Try
                    If Elimina_Manutenzione() Then
                        Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.OperazioneCancellata, Page)
                    End If

                Catch ex As Exception
                    Messaggi.AgroMsgBox(ex.Message, Page)
                Finally
                    AnnullaTutto(Me, Nothing)
                End Try

        End Select

    End Sub


    Private Sub VerificaPermessi()
        Dim UtenteAbilitato_Lettura As Boolean = False
        Dim UtenteAbilitato_Modifica As Boolean = False
        Dim objUtility As New AgronicaCoreModello.Utility_Operazioni

        objUtility.Verifica_Permessi_OperazioniAgenda_X_PagineAgronicaAgenda(objParametri_Server, objParametri_Utenti, UtenteAbilitato_Lettura, UtenteAbilitato_Modifica)

        Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura

        If Not UtenteAbilitato_Lettura Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
            Exit Sub
        End If

        Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica

        If objParametriAgenda.Tipo_Operazione <> enum_TipoOperazioneDB.Lettura AndAlso Not UtenteAbilitato_Modifica Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
            Exit Sub
        End If

    End Sub

    '########################################################################################
    Private Sub ImgBtn_SalvaTutto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SalvaTutto.Click
        If SalvaTutto() Then
            AnnullaTutto(Me, Nothing)
        End If

    End Sub


    '##########################################################
    Private Function SalvaTutto() As Boolean

        '------------------------------------------------
        '----- Definizione delle Variabili
        '------------------------------------------------
        Dim risp As Boolean = False

        Dim Messaggio As String

        Dim objAgendaScrivi As New Agenda_Operazione_Helper

        '=======================
        '===  Aggiornamento  ===
        '=======================

        'Dim Log_Errori As String = ""

        Try

            Select Case TipoOperazioneAgenda

                '------------------------------------------------
                '----- SCRITTURA
                '------------------------------------------------

                Case enum_TipoOperazioneDB.Scrittura


                    Dim Agenda As Operazione_Agenda = CreaOggettoAgenda()

                    If IsNothing(Agenda) Then
                        Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                        Exit Function
                    End If

                    Dim Id_Agenda As Integer = 0
                    'Agenda.Id_Agenda = 0

                    Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)
                    If Id_Agenda <> 0 Then

                    End If

                    If Not IsNothing(Session("aggiornaDateMacchine")) Then
                        If Session("aggiornaDateMacchine") Then
                            aggiornaDataMacchine(Agenda) 'PER FUTURO
                        End If
                        Session("aggiornaDateMacchine") = Nothing
                    End If

                    risp = True

                    '------------------------------------------------
                    '----- MODIFICA
                    '------------------------------------------------
                Case enum_TipoOperazioneDB.Modifica

                    ' Dim Agenda As Operazione_Agenda = HttpContext.Current.Session("Agenda")
                    Dim Agenda As Operazione_Agenda = CreaOggettoAgenda()

                    If IsNothing(Agenda) Then
                        Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                        Exit Function
                    End If

                    Dim CancellataOperazione As Boolean = False
                    CancellataOperazione = objAgendaScrivi.Cancella(Agenda.Piva,
                                                                     Agenda.Sa_Cod,
                                                                     Agenda.Id_Agenda, False,
                                                                     objParametri_Server, logCancellazione:=False)

                    Dim Id_Agenda As Integer
                    Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)


                    If Not IsNothing(Session("aggiornaDateMacchine")) Then
                        If Session("aggiornaDateMacchine") Then
                            aggiornaDataMacchine(Agenda) 'PER FUTURO
                        End If
                        Session("aggiornaDateMacchine") = Nothing
                    End If

                    risp = True

                    '###################################################################


                    '------------------------------------------------
                    '----- CANCELLAZIONE
                    '------------------------------------------------
                Case enum_TipoOperazioneDB.Cancellazione



                    '===============================================

            End Select

            '#########################################################################


            ''------------------------------------------------
            ''Se ho scelto SALVA E CONTINUA...
            'If Me.RBL_Salva.SelectedItem.Value = "1" Then
            '    SalvaContinua = True
            'End If

            'If Log_Errori = "" Then
            '    EseguitaOperazione = True

            '    AAA_GestioneUscitaPagina()
            'Else
            '    'Messaggio di errore
            '    Messaggio = "Si e' verificato un'errore : " & _
            '                Chr(13) & _
            '                Log_Errori

            '    'Visualizzo il messaggio di errore
            '    Call AgroMsgBox(Messaggio, Page)

            'End If



        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------
            'Messaggio di errore
            Messaggio = "Si e' verificato un'errore : " & _
                        Chr(13) & _
                        exc.Message.ToString()

            'Visualizzo il messaggio di errore
            Call Messaggi.AgroMsgBox(Messaggio, Page)

            '------------------------------------------------

        End Try

        Return risp

    End Function


    Private Function CreaOggettoAgenda() As Operazione_Agenda

        Dim Agenda As New Operazione_Agenda
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio As Movimento_Dettaglio

        Dim BaseCode As Integer
        Dim TopCode As Integer

        Dim Descrizione As String

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------
        Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))

        Select Case objParametriAgenda.Lav_Cod

            Case LAVCOD_MANUTENZIONE_MACCHINE
                Descrizione = "Manutenzione Macchine "
                objParametriAgenda.Cau_Mov = CAU_MANUTENZIONE_PARCOMACCHINE

            Case LAVCOD_REVISIONE_MACCHINE
                Descrizione = "Revisione Macchine "
                objParametriAgenda.Cau_Mov = CAU_MANUTENZIONE_PARCOMACCHINE

            Case LAVCOD_MANUTENZIONE_IMPIANTI

        End Select


        ' @Paolo
        ' Ciclo per tutte le Macchine che sono state selezionate
        ' ---> memorizzo per ciascuna la stessa operazione di manutenzione/revisione

        Dim dt As DataTable
        dt = HttpContext.Current.Session("Macchine_checked")


        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------

        Agenda.Tipo_Operazione = objParametriAgenda.Tipo_Operazione
        'Agenda.Id_Agenda = 0
        Agenda.Id_Agenda = objParametriAgenda.Id_Agenda
        Agenda.Data = CDate(Txt_DataIntervento.Text)
        Agenda.Piva = objParametriAgenda.Piva
        Agenda.Sa_Cod = objParametriAgenda.Sa_Cod
        Agenda.Lav_Cod = objParametriAgenda.Lav_Cod

        Agenda.Des_Lib = Descrizione  'LO VALORIZZO NEI MOVIMENTI DETTAGLI

        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode



        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO LAVORAZIONE
        '------------------------------------------------
        '------------------------------------------------
        Movimento = New Movimento
        Movimento.Scadenza = CDate(Txt_DataScadenza.Text)
        Movimento.Id_Agenda = objParametriAgenda.Id_Agenda
        Movimento.Piva = Agenda.Piva
        Movimento.Sa_Cod = Agenda.Sa_Cod
        Movimento.Data = CDate(Txt_DataIntervento.Text)
        Movimento.Lav_Cod = objParametriAgenda.Lav_Cod
        Movimento.Cau_Mov = objParametriAgenda.Cau_Mov
        Movimento.Mov_Desc = Txt_Note.Text
        Movimento.Doc_Numero = CDec(Txt_Certificato.Text)
        If Cmb_CentroRevisione.SelectedValue = "" Then
            Movimento.Cod_Risum = 0
        Else
            Movimento.Cod_Risum = Cmb_CentroRevisione.SelectedValue
        End If



        Movimento.BaseCode = BaseCode
        Movimento.TopCode = TopCode

        Agenda.Movimenti.Add(Movimento)
        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)


        For i = 0 To dt.Rows.Count - 1
            '------------------------------------------------
            '----- MOVIMENTI DETTAGLI 
            '------------------------------------------------

            'AGGIUNGO alla desc dell'operazione la macchina
            Agenda.Des_Lib &= " - " & dt.Rows(i).Item("Mac_Des") & " (" & dt.Rows(i).Item("Modello") & ") "


            Movimento_Dettaglio = New Movimento_Dettaglio

            Movimento_Dettaglio.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento_Dettaglio.Piva = Agenda.Piva
            Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio.Data = CDate(Txt_DataIntervento.Text)
            Movimento_Dettaglio.Lav_Cod = objParametriAgenda.Lav_Cod
            Movimento_Dettaglio.Cau_Mov = objParametriAgenda.Cau_Mov
            Movimento_Dettaglio.Mat_Cod = dt.Rows(i).Item("Mac_Cod")
            Movimento_Dettaglio.Elem_Cod = MACCHINE
            Movimento_Dettaglio.Contabilizzato = NONCONTABILE
            Movimento_Dettaglio.Pendente = enum_Pendenza.MovESENTE
            Movimento_Dettaglio.BaseCode = BaseCode
            Movimento_Dettaglio.TopCode = TopCode

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

        Next


        Return Agenda
    End Function


    Private Function Elimina_Manutenzione() As Boolean

        Dim objAgenda As New Agenda_Operazione_Helper

        Dim Agenda As New Operazione_Agenda

        Agenda = objAgenda.Leggi(objParametriAgenda.Piva, _
                                     objParametriAgenda.Sa_Cod, _
                                     CInt(objParametriAgenda.Id_Agenda), _
                                     0, _
                                     objParametri_Server)


        If Not IsNothing(Agenda) Then

            Dim CancellataOperazione As Boolean = False
            CancellataOperazione = objAgenda.Cancella(Agenda.Piva, _
                                                             Agenda.Sa_Cod, _
                                                             Agenda.Id_Agenda, False, _
                                                             objParametri_Server)
            Return CancellataOperazione

        End If

        Return False

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Carica_NuovoContatto() As RispostaStandard


        Dim r As New RispostaStandard
        Dim objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametriAgenda As New ParametriAgenda

        Dim QueryString As String
        QueryString = "?o=" & _
                        Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, objParametri_Server) & _
                        "&tipo_rapporto=" & _
                        Stringa_Codifica(0, AgroKey_EncoderDecoder, objParametri_Server) & _
                        "&lav_cod=" & _
                        Stringa_Codifica(0, AgroKey_EncoderDecoder, objParametri_Server) & _
                        "&piva=" & _
                        Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, objParametri_Server) & _
                        "&orig=" & _
                        Stringa_Codifica(enum_PagineAgenda_2010.Pagina_GestioneContatti, AgroKey_EncoderDecoder, objParametri_Server) & _
                        "&codcont=" & _
                        Stringa_Codifica("0", AgroKey_EncoderDecoder, objParametri_Server) & _
                        "&dialog=" & _
                        Stringa_Codifica("true", AgroKey_EncoderDecoder, objParametri_Server)

        Dim TargetUrl = "../GestioneContatti/Contatto.aspx"

        Dim Script As String
        Script = "<script language='javascript'>" & _
                    "           var win = window.open('" & TargetUrl & "" & QueryString & "' ," & _
                    "           'Nuovo Contatto', 'width=850, height=550, status=no, menubar=no, toolbar=no, scrollbars=no');" & _
                    "           win.focus(); " & _
                 " </script> "


        'Script = "<script language='javascript'>" & _
        '            "           apriFormDialog('" & TargetUrl & "" & QueryString & "' ," & _
        '            "           'Menu Stampe');" & _
        '         " </script> "


        'Dim StrWindowOpen As String = AgronicaCoreDataProvider.UtilityProvider.Page_NewWindow_RitornaJavascript( _
        '        Me, "Contatto.aspx", _
        '        QueryString, _
        '        "", _
        '        850, 550, 0, 0)

        'ScriptManager.RegisterClientScriptBlock(Content2, Content2.GetType(),
        '                        String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, True)

        'ScriptManager.RegisterClientScriptBlock(Page.FindControl("Content2"), Page.FindControl("Content2").GetType(), String.Format("jQuery_{0}", Page.FindControl("Content2").ClientID), StrWindowOpen, True)

        'UtilityProvider_2010.Page_NewWindow_2010(Page, _
        '        "Contatto.aspx", QueryString, , 850, 550, , , , , , "MainContent", True)

        r.RispostaOK = True
        r.ParametroDue_stringa = Script
        Return r
    End Function

    <script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function ApriScadenziario(dataFine As String) As RispostaStandard

        Dim objParametriAgenda As New ParametriAgenda

        Dim r As New RispostaStandard

        'Dim objXmlPassaggio As New AgronicaCoreGestioneRichieste.ScriviXml
        'ha già creato i parametri di sessione e i parametri del web config
        Dim ParametriScadenziario As New AgronicaCoreGestioneRichieste.ParametriScadenziario
        ParametriScadenziario.Pagina_Richiesta = enum_PagineGiasOnline_2010.Nuova_Scandenza
        ParametriScadenziario.Piva = objParametriAgenda.Piva

        ParametriScadenziario.Id_Area = enum_ID_Area_Alert.Macchine
        ParametriScadenziario.Id_Tipologia = enum_ID_Area_Tipologia.Taratura_ugelli
        If IsDate(dataFine) Then
            ParametriScadenziario.Data_Scadenza = dataFine
        Else
            ParametriScadenziario.Data_Scadenza = AGRODATAFINE
        End If

        Dim script As String
        script = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamente_ParametriScadenziario(Enum_SiteRedirector.Sito_AgronicaAnalisi_2010, ParametriScadenziario)

        'Dim objAgroWebconfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        'script = objXmlPassaggio.ApriIframeConSito(objAgroWebconfig.LinkGiasOnline_2010, Enum_SiteRedirector.Sito_AgronicaAnalisi_2010, Enum_SiteRedirector.Sito_GiasOnline_2010)

        Dim StrRet As String
        StrRet = "<script language='javascript'>" & _
                    "           var win = window.open('" & script & "' ," & _
                    "           'Scadenziario', 'width=850, height=550, status=no, menubar=no, toolbar=no, scrollbars=no');" & _
                    "           win.focus(); " & _
                 " </script> "

        r.RispostaOK = True
        r.ParametroDue_stringa = StrRet
        Return r

    End Function

    Private Sub Ripristina_Dati_nei_Controlli()

        '------------------------------------------
        '----- Recupero le informazioni
        '------------------------------------------

        Dim frm_MatCod As New List(Of Integer)

        Dim objAgenda As New Agenda_Operazione_Helper

        Dim Agenda As New Operazione_Agenda

        Agenda = objAgenda.Leggi(objParametriAgenda.Piva, _
                                     0, _
                                     CInt(objParametriAgenda.Id_Agenda), _
                                     0, _
                                     objParametri_Server)

        HttpContext.Current.Session("Agenda") = Agenda

        objAgenda = Nothing


        If Not IsNothing(Agenda) Then

            objParametriAgenda.Piva = Agenda.Piva
            objParametriAgenda.Sa_Cod = Agenda.Sa_Cod
            objParametriAgenda.Lav_Cod = Agenda.Lav_Cod

            'NOTE
            If Not IsNothing(Agenda.Note) Then
                For i = 0 To Agenda.Note.Count - 1
                    objParametriAgenda.Note.Add(Agenda.Note(i))
                Next
            End If
            objParametriAgenda.salva()

            ''''''''''''''''

            'MOVIMENTI
            If Not IsNothing(Agenda.Movimenti) Then

                For i = 0 To Agenda.Movimenti.Count - 1

                    Txt_Note.Text = Agenda.Movimenti(i).Mov_Desc

                    If Agenda.Movimenti(i).Doc_Numero <> 0 Then
                        Txt_Certificato.Text = Agenda.Movimenti(i).Doc_Numero
                    End If

                    If Not IsNothing(Agenda.Movimenti(i).Scadenza) Then
                        Txt_DataScadenza.Text = Agenda.Movimenti(i).Scadenza.ToShortDateString
                    End If

                    'seleziono centro revisione
                    If Agenda.Movimenti(i).Cod_Risum <> 0 Then
                        Cmb_CentroRevisione.SelectedIndex = Cmb_CentroRevisione.Items.IndexOf(Cmb_CentroRevisione.Items.FindByValue(Agenda.Movimenti(i).Cod_Risum))
                    End If

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case CAU_MANUTENZIONE_PARCOMACCHINE

                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                    ' ???
                                    'frm_MatCod(frm_MatCod.Count - 1) = Agenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod
                                    frm_MatCod.Add(CInt(Agenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod))
                                Next

                            End If



                    End Select

                Next

                HttpContext.Current.Session("frm_MatCod") = frm_MatCod

            End If

            ''''''''''''''''

        End If

    End Sub

    '##########################################################################################################################################
    Private Sub Appezzamento_Edit_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        ' VAnni: 2/10/2017: a seguito di AgroMasterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto

    End Sub

    '##########################################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim TargetUrl As String = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.Menu_principale, objParametriAgenda)
        Response.Redirect(TargetUrl)

    End Sub


    ' ----------------------------------------------
    ' Aggiorno in tutte le macchine la data di ultima revisione e ultima manutenzione nel caso in cui quella nella
    ' macchina sia minore dell'attuale
    ' ----------------------------------------------
    Private Sub aggiornaDataMacchine(agenda As Operazione_Agenda)
        Dim objMacchR = New AgronicaCoreContabDAL.Parco_Macchine_R
        Dim objMacchW = New AgronicaCoreContabDAL.Parco_Macchine_W
        Dim lav_cod = agenda.Lav_Cod
        Dim data = agenda.Data
        For Each dettaglio_movimento As Movimento_Dettaglio In agenda.Movimenti(0).Movimenti_Dettagli
            Dim dtMacch As DataTable = objMacchR.Leggi_daMacCod(agenda.Piva, dettaglio_movimento.Mat_Cod, "", "", objParametri_Server)
            Select Case lav_cod
                Case LAVCOD_MANUTENZIONE_MACCHINE
                    If (data > CDate(dtMacch(0).Item("Ultima_Manutenzione"))) Then
                        If objMacchW.Modifica_Data_Ultima_Manutenzione(dtMacch(0).Item("Mac_Cod"), data, objParametri_Server) Then
                        Else
                            Throw New Exception()
                        End If
                    End If
                Case LAVCOD_REVISIONE_MACCHINE
                    If (data > CDate(dtMacch(0).Item("Ultima_Revisione"))) Then
                        If objMacchW.Modifica_DataTaratura(dtMacch(0).Item("Mac_Cod"), data, objParametri_Server) Then
                        Else
                            Throw New Exception()
                        End If
                    End If
                Case Else
            End Select
        Next

    End Sub

End Class