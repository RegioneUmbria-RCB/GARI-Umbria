Imports System.Web.Services
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility.CulturaHelper
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreAnagrafeDAL

Public Class GestioneMagazziniBS
    Inherits System.Web.UI.Page

    Dim objParametriAgenda As ParametriAgenda

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Public permessi As PermessiUtente
    Public Num_Magazzini As Integer

    Private qs_Visualizzazione_Mode As Integer '0=ricerca (default) 1=dettaglio
    Private qs_Tab_Richiesto As Integer '0=giacenze (default) 1=movimenti

    Private qs_Piva As String
    Private qs_Sa_Cod As Integer
    Private qs_Fabbricato_Cod As String

    Private qs_Lav_Cod As Integer
    Private qs_Elem_Cod As Integer
    Private qs_Pro_Cod As Integer
    Private qs_Mat_Cod As Integer

    Private qs_Data As Date

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub


#Region "Script Services"

    '<WebMethod(EnableSession:=True)>
    'Public Shared Function CaricaMagazzini_XTrasferimento(ByVal fabbricato_cod As String) As RispostaStandard
    '    Dim r As New RispostaStandard

    '    Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
    '    If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
    '        r.Sessione = False
    '    End If

    '    Try

    '        Dim objParametriAgenda As New ParametriAgenda
    '        Dim cmbMagazzino As New DropDownList

    '        Dim fabCod As String = fabbricato_cod.Split("|")(0)

    '        AgronicaCoreUtility.CaricaListControl.Fabbricati(cmbMagazzino, False, "", "",
    '                                                         objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, 0, MAGAZZINO, True,
    '                                                         " Fabbricati.Fabbricato_Cod <> " & fabCod & " AND ", "", AGRODATAFINE,
    '                                                         objParametriServer)

    '        Dim rVal As String = ""
    '        For Each itm As ListItem In cmbMagazzino.Items
    '            rVal &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
    '        Next

    '        r.RispostaOK = True
    '        r.RispostaStringa = rVal

    '    Catch ex As Exception
    '        r.RispostaOK = False
    '        r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
    '    End Try

    '    Return r

    'End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function TrasferimentoMovContabili(ByVal chiaveMagazzino As String,
                                                     ByVal listaChiaviMov As String()
                                                     ) As RispostaStandard

        Dim r As New RispostaStandard
        
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            Return New RispostaStandard With { .Sessione = False }
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
        
            Dim listaAg As New List(Of AgronicaCoreContabBIZ.obj_ID_Agenda_ID_Mov_ID_Mov_Det)

            '20/06/2014_00068870393_162924_0_1025_0_73400347_359212
            For Each chiaveMov In listaChiaviMov
                Dim o As New AgronicaCoreContabBIZ.obj_ID_Agenda_ID_Mov_ID_Mov_Det With {
                    .ID_Agenda = chiaveMov.Split("_")(2),
                    .ID_Mov = chiaveMov.Split("_")(7),
                    .ID_Mov_Det = chiaveMov.Split("_")(8)
                }

                listaAg.Add(o)
            Next

            Dim objAgenda As New AgronicaCoreContabBIZ.Agenda_solo_2010_W

            Dim nuovoSaCod As Integer = CInt(chiaveMagazzino.Split("_")(1))
            Dim nuovoFabb As Integer = CInt(chiaveMagazzino.Split("_")(2))

            r.RispostaOK = objAgenda.Sposta_Magazzino(nuovoSaCod, nuovoFabb,
                                               listaAg, objParametriServer, r.RispostaStringa)
            
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
        
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Calcola_Magazzini(ByVal valore As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            Return New RispostaStandard With { .Sessione = False }
        End If

        Dim objParametriAgenda As New ParametriAgenda
        Dim cmbMagazzino As New DropDownList

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim clc As New AgronicaCoreUtility.CaricaListControl

            If CInt(valore) = 0 Then
                clc.Fabbricati(cmbMagazzino, True, DirectCast(HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "TuttiIMagazzini"), String), "0", objParametriAgenda.Piva, valore, 0, MAGAZZINO, True, "", "", AGRODATAFINE,
                                                                 objParametriServer)
            Else
                clc.Fabbricati(cmbMagazzino, False, "", "", objParametriAgenda.Piva, valore, 0, MAGAZZINO, True, "", "", AGRODATAFINE,
                                                                 objParametriServer)
                ' Controllo se la select ha più di un magazzino
                If cmbMagazzino.Items.Count > 1 Then
                    cmbMagazzino.Items.Insert(0, New ListItem(DirectCast(HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "TuttiIMagazzini"), String), "0"))
                End If
            End If

            Dim rVal As String = ""
            For Each itm As ListItem In cmbMagazzino.Items
                rVal &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
            Next

            r.RispostaStringa = rval
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaMovimento(ByVal chiave As String) As RispostaStandard

        Dim r As New RispostaStandard

        '01/10/2013_01741130403_184361_0_1022_0_77070337
        Dim piva As String = chiave.Split("_")(1)
        Dim Sa_Cod As String = chiave.Split("_")(6)
        Dim Id_Agenda As String = chiave.Split("_")(2)
        Dim Data As String = chiave.Split("_")(0)
        Dim Lav_Cod As String = chiave.Split("_")(4)
        Dim Blocco_Flag As String = chiave.Split("_")(3)
        Dim Veg_Cod As String = 0

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametriAgenda As New ParametriAgenda

        Lingua.Gias_InizializzaCultura_DaSession()

        'verifico permesso op contabili e magazzino
        'permessi op contabili e magazzino
        'MenuAgenda.SelectedValue=3 è anche il codice dell'operazione in questo caso
        Dim permesso As Boolean = AgronicaCoreModello.Utility_Operazioni.PermessiOpContabiliEMagazzino(Lav_Cod, "3", objParametriServer, objParametriUtenti, HttpContext.Current.Session)
        If permesso = False Then
            r.RispostaStringa = AgronicaAgenda_2010.MancanzaPermessiOperazioneSceltaSuGruppoDiOperazioni
            r.RispostaOK = False
            Return r
        End If

        If Blocco_Flag <> 1 Then

            If Veg_Cod > 0 Then

                Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                Dim dt As DataTable = objSpecVeg.SpecieVegetali_GestioneFiltroUtente_Leggi(Veg_Cod,
                                                                                           0,
                                                                                           "",
                                                                                           "",
                                                                                           "",
                                                                                           "",
                                                                                           objParametriUtenti)
                If dt.Rows.Count = 0 Then
                    r.RispostaStringa = Resources.AgronicaAgenda_2010.NoModificaNoPermessoSpecie
                    r.RispostaOK = False
                    Return r
                End If

            End If

            objParametriAgenda.Piva = piva
            objParametriAgenda.Sa_Cod = Sa_Cod
            objParametriAgenda.Data = Data
            objParametriAgenda.Id_Agenda = Id_Agenda
            objParametriAgenda.Lav_Cod = Lav_Cod
            objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Cancellazione

            Try
                Dim messaggio As String = ""
                Dim matrice_delete(,) As String
                Dim res As Boolean = False
                Dim msg = ""
                If ControllaOperazione(matrice_delete, objParametriAgenda, objParametriServer, msg, False, objParametri_Utenti:=objParametriUtenti) Then
                    res = Operazione_Agenda_Utility.Cancella_Operazione_E_Collegate(objParametriAgenda,
                                                                                    objParametriServer,
                                                                                    messaggio,
                                                                                    True,
                                                                                    objParametri_Utenti:=objParametriUtenti)

                    If res Then
                        r.RispostaStringa = Resources.AgronicaAgenda_2010.OperazioneCancellata
                    Else
                        r.RispostaStringa = Resources.AgronicaAgenda_2010.OperazioneNonCancellataWarning
                    End If
                Else
                    r.Errore = msg
                End If

                r.RispostaOK = res
                Return r

                'Try

                'Catch ex As Exception
                '    Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.NonÈPossibileEliminareQuestaOperazioneBr & ex.Message, Page, , Nothing)
                'Finally
                '    'riaggiorno
                '    Esegui_Ricerca_giacenze()
                '    Esegui_Ricerca_Movimenti_Click()
                'End Try

            Catch ex As Exception

                r.RispostaStringa = ex.Message
                r.RispostaOK = False
                Return r

            End Try

        Else

            r.RispostaStringa = Resources.AgronicaAgenda_2010.ImpossibileModificareOperazioneBlocc
            r.RispostaOK = False
            Return r

        End If

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Gestione_Operazione_Menu(ByVal CodiceMenu As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Fabbricato_Cod As String,
                                                    ByVal Mat_Cod As String,
                                                    ByVal Pro_Cod As String
                                                    ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objParametriAgenda As New ParametriAgenda


            'If Sa_Cod <> "" Then
            objParametriAgenda.Sa_Cod = Sa_Cod
            ' End If

            'If Fabbricato_Cod <> "" Then
            objParametriAgenda.Fabbricato = Fabbricato_Cod
            'End If

            Dim targetUrl As String = ""

            Dim Id_Agenda As String = ""
            Dim Data As String = ""
            Dim Lav_Cod As String = ""
            Dim Blocco_Flag As String = ""

            Dim Veg_Cod As Integer = 0
            Dim gru_cod As String = ""
            Dim TIPO As String = ""

            If Pro_Cod = "" Then
                Pro_Cod = 0
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Select Case CodiceMenu

                '################################################################################################################
                '#####  CARICO SCARICO TRASFERIMENTO #####################################################################
                '################################################################################################################

                Case "50", "51", "52"
                    Lav_Cod = "0"
                    If CodiceMenu = "50" Then
                        Lav_Cod = CStr(LAVCOD_CARICO)
                    End If
                    If CodiceMenu = "51" Then
                        Lav_Cod = CStr(LAVCOD_SCARICO)
                    End If
                    If CodiceMenu = "52" Then
                        Lav_Cod = CStr(LAVCOD_TRASFERIMENTO)
                    End If
                    objParametriAgenda.Lav_Cod = Lav_Cod
                    'verifico permesso op contabili e magazzino
                    'permessi op contabili e magazzino
                    'CodiceMenu= non è anche il codice dell'operazione in questo caso, metto 1 per nuovo
                    Dim permesso As Boolean = AgronicaCoreModello.Utility_Operazioni.PermessiOpContabiliEMagazzino(Lav_Cod, "1", objParametriServer, objParametriUtenti, HttpContext.Current.Session)
                    If permesso = False Then
                        r.Errore = AgronicaAgenda_2010.MancanzaPermessiOperazioneSceltaSuGruppoDiOperazioni
                        Return r
                    End If

                    objParametriAgenda.Lav_Cod = Lav_Cod
                    objParametriAgenda.Data = Date.Now.ToShortDateString
                    objParametriAgenda.Id_Agenda = 0
                    objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                    Dim OpUtil As New AgronicaCoreModello.Utility_Operazioni
                    targetUrl = OpUtil.LinkPagina_from_LavCod_NEW(objParametriAgenda.Lav_Cod, objParametriAgenda, PaginaSitoAgendaOrigine:=enum_PagineAgenda_2010.Pagina_GestioneMagazziniBS)

                    r.RispostaOK = True
                    r.RispostaStringa = targetUrl
                    Return r

                    'Session("ParametriPaginaGestioneMagazzini") = ParametriPaginaGestioneMagazzini

                    '################################################################################################################
                    '#####  NUOVO PRODOTTO #####################################################################
                    '################################################################################################################
                Case "53"
                    targetUrl = AgronicaCoreModello.Utility_Operazioni.Link_GiasOnline_STR(enum_PagineGiasOnline.GestioneRisorse, objParametriAgenda)
                    r.RispostaOK = True
                    r.RispostaStringa = targetUrl
                    Return r

                    '################################################################################################################
                    '#####  SINCRO MAGAZZINO #####################################################################
                    '################################################################################################################

                'Case "54"

                    'Dim paginaSincro As enum_PagineAgronicaSincro

                    'Select Case HttpContext.Current.Session("ASG_ProgressivoGIAS")

                    '    Case enum_CodiceGIAS_Clienti.Agrisol,
                    '        enum_CodiceGIAS_Clienti.FruitModena
                    '        paginaSincro = enum_PagineAgronicaSincro.Import_DDT_Seled

                    '    Case Else
                    '        'importazione magazzino da altro archivio (cml, excel, webservice)
                    '        paginaSincro = enum_PagineAgronicaSincro.ImportazioneMagazzinoXMLPubblico

                    'End Select

                    'targetUrl = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sitosincronizzatore_PassandoDirettamenteIParametri(
                    '                                                    Enum_SiteRedirector.Sito_GiasOnline,
                    '                                                    paginaSincro,
                    '                                                    0,
                    '                                                    objParametriAgenda.Piva,
                    '                                                    0)

                    'r.RispostaOK = True
                    'r.RispostaStringa = targetUrl
                    'r.ParametroDue = True
                    'Return r


                    'Case "3"
                    '    'elimina
                    '    If OperazioniSelezionate(piva, sa_cod, Id_Agenda, Data, Lav_Cod, Blocco_Flag, Veg_Cod) <> 1 Then
                    '        'alert 1 sola
                    '        Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.EliminareUnOperazioneAllaVolta, Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
                    '        Exit Function
                    '    Else

                    '        'verifico permesso op contabili e magazzino
                    '        'permessi op contabili e magazzino
                    '        'CodiceMenu=3 è anche il codice dell'operazione in questo caso
                    '        Dim permesso As Boolean = Utility_NS.Utility_Operazioni.PermessiOpContabiliEMagazzino(Lav_Cod, CodiceMenu, objParametri_Server, objParametri_Utenti, Session)
                    '        If permesso = False Then
                    '            Messaggi.AgroMsgBox("Non si hanno i permessi per questa operazione su questo gruppo di operazioni", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
                    '            Exit Function
                    '        End If

                    '        If Blocco_Flag <> 1 Then

                    '            If Veg_Cod > 0 Then

                    '                Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                    '                Dim Dt As DataTable = objSpecVeg.SpecieVegetali_GestioneFiltroUtente_Leggi(Veg_Cod, _
                    '                                                                 0, _
                    '                                                                 "", _
                    '                                                                 "", _
                    '                                                                 "", _
                    '                                                                 "", _
                    '                                                                 objParametri_Utenti)
                    '                If Dt.Rows.Count = 0 Then
                    '                    Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.NoModificaNoPermessoSpecie, Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
                    '                    Exit Function
                    '                End If

                    '            End If

                    '            objParametriAgenda.Piva = piva
                    '            objParametriAgenda.Sa_Cod = sa_cod
                    '            objParametriAgenda.Data = Data
                    '            objParametriAgenda.Id_Agenda = Id_Agenda
                    '            objParametriAgenda.Lav_Cod = Lav_Cod


                    '            Try
                    '                Dim matrice_delete(,) As String
                    '                If ControllaOperazione(matrice_delete, objParametriAgenda, objParametri_Server, "", False) Then
                    '                End If
                    '            Catch ex As Exception
                    '                Messaggi.AgroMsgBox(ex.Message, Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
                    '                Exit Function
                    '            End Try


                    '            Messaggi.AgroEliminaSiNo("DEL", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))


                    '            Return
                    '        Else

                    '            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.ImpossibileModificareOperazioneBlocc, Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
                    '            Exit Function

                    '        End If
                    '    End If



                    '################################################################################################################
                    '#####  STAMPE #####################################################################
                    '################################################################################################################

                Case "7p", "7q", "7r", "7s", "7t"

                    Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
                    Dim strNodiVariabili As String = ""
                    Select Case CodiceMenu
                        Case "7p" 'giacenze
                            objAgronicaStampe.report = enum_CodificaStampe.SchedaMagazzinoGiacenze
                        Case "7q" 'movimenti
                            objAgronicaStampe.report = enum_CodificaStampe.SchedaMagazzinoMovimenti
                        Case "7r" 'fertilizzanti
                            objAgronicaStampe.report = enum_CodificaStampe.SchedaMagazzinoFertilizzanti
                        Case "7s" 'fito
                            objAgronicaStampe.report = enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari
                        Case "7t" 'riepilogo
                            objAgronicaStampe.report = enum_CodificaStampe.RiepilogoProdottiUtilizzati
                    End Select

                    strNodiVariabili = StrXmlParametri_Schede_Magazzino(objAgronicaStampe.report,
                                                                        objParametriAgenda.Piva,
                                                                        Sa_Cod,
                                                                        objParametriAgenda.Fabbricato,
                                                                        objParametriAgenda.Elem_Cod,
                                                                        Pro_Cod,
                                                                        Mat_Cod,
                                                                        AGRODATAINIZIO,
                                                                        AGRODATAINIZIO,
                                                                        AGRODATAFINE)

                    objAgronicaStampe.username = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
                    objAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
                    objAgronicaStampe.Xml_Generico.Length = 0
                    objAgronicaStampe.Xml_Generico.Append(strNodiVariabili)

                    Dim s = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                                                                                                                             objAgronicaStampe)

                    r.RispostaOK = True
                    r.RispostaStringa = s
                    r.ParametroDue = True
                    Return r

                    '################################################################################################################
                    '#####  AGENDA #####################################################################
                    '################################################################################################################

                Case "60"

                    Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim dtConfigSiti As DataTable
                    dtConfigSiti = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametriServer)
                    If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
                        targetUrl = "../menu/menubs_agenda_nuovo.aspx"
                    Else
                        targetUrl = "../Menu/Menu.aspx"
                    End If

                    r.RispostaOK = True
                    r.RispostaStringa = targetUrl
                    Return r


                    '################################################################################################################
                    '#####  Anagrafica azienda #####################################################################
                    '################################################################################################################
                Case "16"
                    targetUrl = AgronicaCoreModello.Utility_Operazioni.Link_GiasOnline_STR(enum_PagineGiasOnline.AlberoImprese, objParametriAgenda)
                    r.RispostaOK = True
                    r.RispostaStringa = targetUrl
                    Return r

                    '################################################################################################################
                    '#####  Contabilità#####################################################################
                    '################################################################################################################
                Case "17"

                    targetUrl = AgronicaCoreModello.Utility_Operazioni.Link_GiasOnline_STR(enum_PagineGiasOnline.MenuContab, objParametriAgenda)
                    r.RispostaOK = True
                    r.RispostaStringa = targetUrl
                    Return r

                    '################################################################################################################
                    '#####  Cambia Impresa #####################################################################
                    '################################################################################################################
                Case "20"
                    Dim origine As String
                    Dim destinazione As String

                    'azzero il filtro scelto x le operazioni multi-aziendali
                    HttpContext.Current.Session("VariabiliFiltro") = Nothing

                    'Costruisco il link
                    origine = Stringa_Codifica("../GestioneMagazzini/GestioneMagazziniBS.aspx",
                                               AgroKey_EncoderDecoder)

                    destinazione = Stringa_Codifica("../GestioneMagazzini/GestioneMagazziniBS.aspx",
                                                    AgroKey_EncoderDecoder)

                    targetUrl = "../Filtrino/FiltrinoImprese.aspx" &
                                "?o=" & origine &
                                "&d=" & destinazione
                    r.RispostaOK = True
                    r.RispostaStringa = targetUrl
                    Return r


                    '################################################################################################################
                    '#####  Impostazioni utente #####################################################################
                    '################################################################################################################
                Case "21"
                    Dim Utenti_Permessi_R As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                    Dim s = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoProfilazione_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                               enum_PagineProfilazione_2010.Pagina_Utenti_Impostazioni,
                               enum_PagineAgenda_2010.Pagina_GestioneMagazziniBS,
                               objParametriAgenda.Piva)

                    r.RispostaOK = True
                    r.RispostaStringa = s
                    r.ParametroDue = True
                    Return r

                    '    Exit Function

                    '################################################################################################################
                    '#####  Profilazione #####################################################################
                    '################################################################################################################
                Case "22"
                    Dim Utenti_Permessi_R As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                    Dim s = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoProfilazione_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                               enum_PagineProfilazione_2010.Pagina_Home,
                               enum_PagineAgenda_2010.Pagina_GestioneMagazziniBS,
                               objParametriAgenda.Piva)

                    r.RispostaOK = True
                    r.RispostaStringa = s
                    r.ParametroDue = True
                    Return r


                    '################################################################################################################
                    '#####  Contatti #####################################################################
                    '################################################################################################################
                Case "23"

                    'menu contatti
                    targetUrl = "../GestioneContatti/Contatti_Manager.aspx?piva=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                        "&origine=" & Stringa_Codifica("../GestioneMagazzini/GestioneMagazziniBS.aspx", AgroKey_EncoderDecoder)
                    r.RispostaOK = True
                    r.RispostaStringa = targetUrl
                    Return r

                    '################################################################################################################
                    '#####  Profitosan #####################################################################
                    '################################################################################################################
                Case "13"
                    Dim link As String = AgronicaCoreGestioneRichieste.profitosan.getLinkSimple(True, HttpContext.Current.Session("Request_Session"), New AgronicaCoreGestioneRichieste.AgroWebConfig, objParametriServer, objParametriUtenti)

                    r.RispostaOK = True
                    r.RispostaStringa = link
                    r.ParametroDue = True
                    Return r

                Case "1000"
                    'PER OPTA

                    Dim link As String = "./AnalisiRitiriTabacco.aspx"
                    r.RispostaOK = True
                    r.RispostaStringa = link
                    r.ParametroDue = True
                    Return r

                Case "1001"
                    'PER OPTA
                    Dim link As String = "./RintracciaLotto.aspx"
                    r.RispostaOK = True
                    r.RispostaStringa = link
                    r.ParametroDue = True
                    Return r

                Case "165" 'SMART AGENDA

                    Dim Utenti_Permessi_R As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                    Dim s = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                               Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                               enum_PagineGiasOnline_2010.RegistazioneSmart,
                               enum_PagineAgenda_2010.Pagina_GestioneMagazziniBS,
                               objParametriAgenda.Piva,
                               "", "", 0, "")

                    r.RispostaOK = True
                    r.RispostaStringa = s
                    r.ParametroDue = True
                    Return r

                Case Else

                    r.RispostaOK = False
                    r.Errore = String.Format("Codice menù {0} non gestito", CodiceMenu)
                    Return r

            End Select

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

#End Region

    Private Sub GestioneMagazziniBS_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        ' VAnni: 2/10/2017: a seguito di AgroMasterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtnFiltro.Click, AddressOf Me.CambiaImpresa

        Master.flag_pag_GestioneMagazziniBS = True
    End Sub

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        objParametriAgenda.Svuota_DatiOperazione()

        Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline AndAlso
           paginaOnLineRitorno = enum_PagineGiasOnline.MenuMagazzini Then

            Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(True, paginaOnLineRitorno, objParametriAgenda))

        End If

        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso
           paginaOnLineRitorno = enum_PagineGiasOnline_2010.Menu Then

            Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(False, paginaOnLineRitorno, objParametriAgenda))

        End If

        If paginaOnLineRitorno = 0 Then
            If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline_2010 Then
                paginaOnLineRitorno = enum_PagineGiasOnline_2010.Menu
            Else
                paginaOnLineRitorno = enum_PagineAgenda_2010.Menu
            End If
            Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(False, paginaOnLineRitorno, objParametriAgenda))
        End If

        If Master.flag_MenuBS_2017 Then
            Response.Redirect("../Menu/MenuBS_2017.aspx")
        End If

        Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineAgenda_2010.Menu, objParametriAgenda))

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("GestioneMagazzino"), String)
        Master.flag_pag_Anagrafica = True
        Master.SetTitoloPagina(46)

        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If
        '---

        permessi = New PermessiUtente() 'leggo i permessi che serviranno anche lato client

        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
        objParametriAgenda = New ParametriAgenda

        If Not IsNothing(Request.QueryString("visualizzazione_mode")) Then
            qs_Visualizzazione_Mode = Stringa_Decodifica(Request.QueryString("visualizzazione_mode").ToString, AgroKey_EncoderDecoder)
        Else
            qs_Visualizzazione_Mode = 0
        End If
        If Not IsNothing(Request.QueryString("piva")) Then
            qs_Piva = Stringa_Decodifica(Request.QueryString("piva").ToString, AgroKey_EncoderDecoder)
        Else
            qs_Piva = ""
        End If
        If Not IsNothing(Request.QueryString("sa_cod")) Then
            qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("sa_cod").ToString, AgroKey_EncoderDecoder)
        Else
            qs_Sa_Cod = 0
        End If

    
        If Not IsNothing(Request.QueryString("fabbricato_cod")) Then
            Dim strFabb As String = Stringa_Decodifica(Request.QueryString("fabbricato_cod").ToString, AgroKey_EncoderDecoder)
            qs_Fabbricato_Cod = ConvertiChiaveMagazzinoKendo(strFabb)
        Else
            qs_Fabbricato_Cod = ""
        End If

        If Not IsNothing(Request.QueryString("lav_cod")) Then
            qs_Lav_Cod = Stringa_Decodifica(Request.QueryString("lav_cod").ToString, AgroKey_EncoderDecoder)
        Else
            qs_Lav_Cod = 0
        End If
        If Not IsNothing(Request.QueryString("elem_cod")) Then
            qs_Elem_Cod = Stringa_Decodifica(Request.QueryString("elem_cod").ToString, AgroKey_EncoderDecoder)
        Else
            qs_Elem_Cod = 0
        End If
        If Not IsNothing(Request.QueryString("pro_cod")) Then
            qs_Pro_Cod = Stringa_Decodifica(Request.QueryString("pro_cod").ToString, AgroKey_EncoderDecoder)
        Else
            qs_Pro_Cod = 0
        End If
        If Not IsNothing(Request.QueryString("mat_cod")) Then
            qs_Mat_Cod = Stringa_Decodifica(Request.QueryString("mat_cod").ToString, AgroKey_EncoderDecoder)
        Else
            qs_Mat_Cod = 0
        End If
        If Not IsNothing(Request.QueryString("tab_richiesto")) Then
            qs_Tab_Richiesto = Stringa_Decodifica(Request.QueryString("tab_richiesto").ToString, AgroKey_EncoderDecoder)
        Else
            qs_Tab_Richiesto = 0
        End If
        If Not IsNothing(Request.QueryString("data")) Then
            qs_Data = Stringa_Decodifica(Request.QueryString("data").ToString, AgroKey_EncoderDecoder)
        Else
            qs_Data = AGRODATAINIZIO
        End If

        Session("Request_Session") = Request

        Dim isPostBackScript As String = "var isPostBack = false;"

        hdVisualizzazioneMode.Value = qs_Visualizzazione_Mode
        hdTabRichiesto.Value = qs_Tab_Richiesto

        'If qs_Data <> AGRODATAINIZIO Then
        '    txt_DataOperazioneAlGiorno.Text = qs_Data.ToShortDateString
        '    Dim leggiAnnataAgraria As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        '    Dim d1, d2 As Date
        '    leggiAnnataAgraria.AnnataAgraria(qs_Data, d1, d2, objParametri_Utenti)
        '    txt_DataOperazioneDa.Text = d1.ToShortDateString
        '    txt_DataOperazioneA.Text = d2.ToShortDateString
        'End If

        If qs_Visualizzazione_Mode > 0 Then
            Select Case qs_Tab_Richiesto
                Case 0
                    Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("DettaglioGiacenze"), String)
                Case Else
                    Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("DettaglioMovimenti"), String)
            End Select
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False

        End If


        If Not IsPostBack Then



            Dim objM As New AgronicaCoreAnagrafeDAL.Fabbricati_R
            Num_Magazzini = objM.Numero_Fabbricati(objParametriAgenda.Piva, objParametri_Server)

            DistruggiSessionVecchie()

            '==================================
            '======= VERIFICA PERMESSI ========
            '==================================

            Dim UtenteAbilitato_Lettura As Boolean = False
            ViewState("UtenteAbilitato_Lettura") = permessi.getPermesso(enum_Security_Attivita.Gest_Magazzino).Lettura
            ViewState("UtenteAbilitato_Modifica") = permessi.getPermesso(enum_Security_Attivita.Gest_Magazzino).Scrittura

            Dim UtenteAbilitato_Lettura_GestPrezzi As Boolean = permessi.getPermesso(enum_Security_Attivita.Gestione_Prezzi).Lettura
            hf_UtenteAbilitatoGestionePrezziLettura.Value = UtenteAbilitato_Lettura_GestPrezzi

            ImpostaPermessi()

            If qs_Visualizzazione_Mode = 0 Then
                objParametriAgenda.Svuota_DatiOperazione()
            End If

            If objParametriAgenda.Piva = "" Then
                Throw New Exception("objParametriAgenda.Piva = '' ")
            End If
            hdPiva.Value = objParametriAgenda.Piva

            'Centro Aziendale
            'CaricaCentriAziendali()

            'Magazzini
            'CaricaMagazzini(ComboMagazzini)    'TODO: GIULIA
            'CaricaMagazzini(ComboMagazzini_1)

            'AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(Me.cmb_Categoria,
            '                                                         True, AgronicaAgenda_2010.TutteLeCategorie, "0",
            '                                                         0, "", 0, False, False,
            '                                                         "Elem_Cod NOT IN (" &
            '                                                         COADIUVANTI.ToString & "," &
            '                                                         MACCHINE & "," & RIFIUTI & "," & ZOO_CONSISTENZA & "," &
            '                                                         ALTRI_BENI_AMMORTIZZABILI & "," & ALTRI_BENI & ")",
            '                                                         "", objParametri_Server)

            'cmb_Categoria.SelectedValue = "0"

            'pre-setto se arrivata dalla singola operazione
            'If qs_Sa_Cod <> 0 Then
            '    ComboCentroAziendale.ddl_CentroAziendale.SelectedIndex = ComboCentroAziendale.ddl_CentroAziendale.Items.IndexOf(ComboCentroAziendale.ddl_CentroAziendale.Items.FindByValue(qs_Sa_Cod))
            'End If
            'If qs_Fabbricato_Cod <> "" Then 'TODO: GIULIA
            '    ComboMagazzini.ddl_Magazzini.SelectedIndex = ComboMagazzini.ddl_Magazzini.Items.IndexOf(ComboMagazzini.ddl_Magazzini.Items.FindByValue(qs_Fabbricato_Cod))
            'End If
            'If qs_Elem_Cod <> 0 Then
            '    cmb_Categoria.SelectedIndex = cmb_Categoria.Items.IndexOf(cmb_Categoria.Items.FindByValue(qs_Elem_Cod))
            'End If
            'If qs_Pro_Cod <> 0 Then
            '    TxtCodProdotto.Text = qs_Pro_Cod
            'End If

            Dim objDefaults As New Defaults With {
                .Piva = hdPiva.Value,
                .Sa_Cod = qs_Sa_Cod,
                .Chiave_Magazzino = qs_Fabbricato_Cod,
                .Elem_Cod = qs_Elem_Cod,
                .Pro_Cod = qs_Pro_Cod,
                .Prodotto = If(qs_Pro_Cod <> 0, qs_Pro_Cod, "")
            }

            'Data
            If qs_Data = AGRODATAINIZIO Then
                objDefaults.DataGiacenza = Today
                objDefaults.DataOpDa = Nothing
                objDefaults.DataOpA = Nothing '= objParametriAgenda.Data no, la giacenza la faccio di default alla AgroDataFine e i movimenti nel mese
                'txt_DataOperazioneAlGiorno.Text = Today.ToShortDateString
                'txt_DataOperazioneDa.Text = ""
                'txt_DataOperazioneA.Text = "" '= objParametriAgenda.Data no, la giacenza la faccio di default alla AgroDataFine e i movimenti nel mese
            Else
                Dim d1, d2 As Date
                Dim leggiAnnataAgraria As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                leggiAnnataAgraria.AnnataAgraria(qs_Data, d1, d2, objParametri_Utenti)

                objDefaults.DataGiacenza = qs_Data
                objDefaults.DataOpDa = d1
                objDefaults.DataOpA = d2

                'txt_DataOperazioneAlGiorno.Text = qs_Data.ToShortDateString
                'txt_DataOperazioneDa.Text = d1.ToShortDateString
                'txt_DataOperazioneA.Text = d2.ToShortDateString

            End If


            hdDefaults.Value = JsonConvert.SerializeObject(objDefaults, Formatting.None)


            'Controllo se F&F / Cantine / Tabacco / Zoo
            Dim anagrafeLogR As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
            Dim elencoModuli As List(Of Integer) = anagrafeLogR.Recupera_Moduli_Cliente(hdPiva.Value, objParametri_Server)
            hf_Moduli_Anagrafe_Log.Value = JsonConvert.SerializeObject(elencoModuli, Formatting.None)

        Else

            If objParametriAgenda.Piva = "" Then
                Throw New Exception("objParametriAgenda.Piva = '' ")
            End If

            isPostBackScript = "var isPostBack = true;"

        End If

        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "IsPostBack", isPostBackScript, True)

    End Sub

    Private Function ConvertiChiaveMagazzinoKendo(strFabb As String) As String

        If strFabb.Contains("|") Then
            'Mi è arrivato il codice magazzino nel vecchio formato (Combo Server), quindi lo devo convertire nella nuova chiave Kendo

            Dim fabCod As String = strFabb.Split("|")(0)
            Dim saCod As String = strFabb.Split("|")(1)
            Dim piva As String = ""
            If strFabb.Split("|").Count() = 3 Then
                piva = strFabb.Split("|")(2)
            End If

            'Dò per scontato che mi sia arrivato un magazzino 20?!?
            Dim nuovaChiave As String = TIPO_DESTINAZIONE_MAGAZZINO & "_" & saCod & "_" & fabCod

            Return nuovaChiave
        Else If IsNumeric(strFabb) AndAlso CInt(strFabb) = 0 Then
            Return ""
        Else
            Return strFabb
        End If
    End Function

    Private Sub DistruggiSessionVecchie()
        Session.Remove("UtenteAbilitato_Lettura")
        Session.Remove("UtenteAbilitato_Modifica")
    End Sub

    Public Sub ImpostaPermessi()
        'Controllo se ha il permesso di lettura
        If ViewState("UtenteAbilitato_Lettura") = False Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

    End Sub

    'Private Sub CaricaCentriAziendali()
    '    ComboCentroAziendale.Piva = objParametriAgenda.Piva
    '    ComboCentroAziendale.CaricaComboCentroAziendale(True)
    '    ComboCentroAziendale.Valore_Combo = objParametriAgenda.Sa_Cod
    '    ComboCentroAziendale.ddl_CentroAziendale.SelectedIndex = ComboCentroAziendale.ddl_CentroAziendale.Items.IndexOf(ComboCentroAziendale.ddl_CentroAziendale.Items.FindByValue(objParametriAgenda.Sa_Cod))
    'End Sub

    Private Sub CaricaMagazzini(ByRef comboMag As AgronicaControlli_2010.ComboMagazzini)
        comboMag.Piva = objParametriAgenda.Piva
        comboMag.Sa_Cod = objParametriAgenda.Sa_Cod
        comboMag.Flag_CodCentroFabbricato = True
        comboMag.TipoMagazzino = MAGAZZINO
        comboMag.Flag_GestioneMagazziniImpresaPadre = False
        comboMag.TestoRiga0 = DirectCast(GetLocalResourceObject("TuttiIMagazzini"), String)
        comboMag.CaricaComboMagazzini()
        'imposto il valore
        If objParametriAgenda.Fabbricato <> "0" AndAlso Split(objParametriAgenda.Fabbricato, "|").Count = 3 Then
            'ok
        Else
            If Split(objParametriAgenda.Fabbricato, "|").Count = 2 Then
                'manca la piva
                objParametriAgenda.Fabbricato = objParametriAgenda.Fabbricato & "|" & objParametriAgenda.Piva
            Else
                objParametriAgenda.Fabbricato = "0"
            End If
        End If
        comboMag.Valore_Combo = objParametriAgenda.Fabbricato

        If comboMag.ddl_Magazzini.Items.Count > 2 Then
            comboMag.ddl_Magazzini.SelectedIndex = comboMag.ddl_Magazzini.Items.IndexOf(comboMag.ddl_Magazzini.Items.FindByValue(objParametriAgenda.Fabbricato))
        Else
            comboMag.ddl_Magazzini.SelectedIndex = 1
        End If

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function AlienaRiga(ByVal piva As String, ByVal Sa_Cod As Integer, ByVal Fabbricato_Cod As Integer, ByVal Tipo_Destinazione As Integer,
                                      ByVal Elem_Cod As Integer, ByVal Pro_Cod As Integer, ByVal Prodotto_Des As String, ByVal Mat_Cod As Integer,
                                      ByVal Lotto As String, ByVal Cal_Cod As String, ByVal Cod_Progetto As String, ByVal Fase_Cod As String,
                                      ByVal Udm_Cod As Integer, ByVal Qta_No_Arrotondamenti As Decimal,
                                      ByVal Data As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim idAgenda As Integer = 0
        Dim objAgendaScrivi As New Agenda_Operazione_Helper


        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            'Dim Lotto As String = Lotto_Acc      '"Lotto_Int OR Lotto_Acc"
            Dim dataAlienazione As Date

            If Data = "" Then
                dataAlienazione = Now
            Else
                dataAlienazione = CDate(Data)
            End If

            If dataAlienazione > Now Then
                dataAlienazione = Now
            End If


            Dim opAgenda As New Operazione_Agenda With {
                .Tipo_Operazione = enum_TipoOperazioneDB.Scrittura,
                .Piva = piva,
                .Sa_Cod = Sa_Cod,
                .Id_Agenda = 0,
                .Lav_Cod = LAVCOD_SCARICO,
                .Des_Lib = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "Alienazione"), String) & " " & Prodotto_Des,
                .Data = dataAlienazione
            }

            Dim lMovDettaglio As New Movimento With {
                .Piva = piva,
                .Sa_Cod = Sa_Cod,
                .Id_Agenda = opAgenda.Id_Agenda,
                .Id_Mov = 0,
                .Cod_Risum = 0,
                .Cau_Mov = CAU_SCARICO,
                .Doc_Numero = 0,
                .Doc_Numero_Sin = "",
                .Doc_Numero_Des = "",
                .Ora = dataAlienazione,
                .Mov_Desc = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "ScaricoPerAlienazioneGiacenze"), String),
                .Data = dataAlienazione,
                .Scadenza = AGRODATAFINE,
                .Num_Protocollo_Decimal = 0,
                .Sezionale_Cod = 0,
                .Progr_Registrazione = 0,
                .Data_Registrazione = dataAlienazione
            }

            Dim lMovimentiDettaglio As New Movimento_Dettaglio With {
                .Piva = piva,
                .Sa_Cod = Sa_Cod,
                .Id_Agenda = opAgenda.Id_Agenda,
                .Id_Mov = lMovDettaglio.Id_Mov,
                .Id_Mov_Det = 0,
                .Elem_Cod = Elem_Cod,
                .Pro_Cod = Pro_Cod,
                .Mat_Cod = Mat_Cod,
                .Udm_Cod = Udm_Cod,
                .Cod_Progetto = CInt(Cod_Progetto),
                .Cal_Cod = CInt(Cal_Cod),
                .Fase_Cod = CInt(Fase_Cod),
                .Lotto = Lotto,
                .Mov_Det_Des = "",
                .Qta = Qta_No_Arrotondamenti,
                .Qta_Dettaglio1 = 0,
                .Qta_Dettaglio2 = 0,
                .Prezzo_Unitario = 0,
                .Prezzo_Unitario_Netto = 0,
                .Prezzo_Effettivo = 0,
                .Tara = 0,
                .Qta_Extra_Totale = 0,
                .Variazione = 0,
                .Listino_Cod = 0,
                .Sconto_Listino = 0,
                .Sconto_Testo = 0,
                .Sconto_Modalita = 0,
                .Cod_Iva = 0,
                .Iva_Deto_Cod = 0,
                .ChkIva_Manuale = 0,
                .Jolly_Int = enum_TipoMovimentazioneMagazzino.MagazzinoMovimentato,
                .Sconto = 0,
                .ChkLayOut_Hide = 0,
                .Contabilizzato = CONTABILE,
                .Imponibile = 0,
                .Imponibile_Netto = 0,
                .Iva = 0,
                .Cod_IvaIndetraibile = 0,
                .Iva_Indetraibile_Perc = 0,
                .Iva_Indetraibile = 0,
                .TempoCarenza = 0,
                .Cau_Mov = CAU_SCARICO,
                .Pendente = enum_Pendenza.Alienazione_Pendenza,
                .Data = dataAlienazione,
                .Validita_Inizio = dataAlienazione,
                .Validita_Fine = AGRODATAFINE,
                .Ric_Cod = 2,
                .Ric_Cod_Pat = 2,
                .Anno = Year(dataAlienazione),
                .Cod_Conto = 0,
                .Cod_Conto_Pat = 0,
                .Extra_Int = 0,
                .Extra_Date = AGRODATAFINE
            }

            '.Udm_Cod_Extra = Udm_Cod_Extra,
            '.Qta_Extra = Qta_Extra,

            Dim lMovDestinazione As New Movimento_Destinazione With {
                .Piva = piva,
                .Sa_Cod = Sa_Cod,
                .Id_Agenda = opAgenda.Id_Agenda,
                .Id_Mov = lMovDettaglio.Id_Mov,
                .Id_Mov_Det = lMovimentiDettaglio.Id_Mov_Det,
                .Appezza = 0,
                .Id_Destinazione = Fabbricato_Cod,
                .Tipo = Tipo_Destinazione,
                .Qta = Qta_No_Arrotondamenti,
                .Qta2 = 0,
                .Data = dataAlienazione
            }

            lMovimentiDettaglio.Movimenti_Destinazioni.Add(lMovDestinazione)

            lMovDettaglio.Movimenti_Dettagli.Add(lMovimentiDettaglio)

            opAgenda.Movimenti.Add(lMovDettaglio)

            idAgenda = objAgendaScrivi.Scrivi(opAgenda, objParametriServer, True)

            If idAgenda <> 0 Then

                r.RispostaOK = True
                r.RispostaStringa = String.Format(
                    DirectCast(HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "AlienaRiga_CreazioneOk"), String),
                    idAgenda)
            Else
                r.RispostaOK = False
                r.Errore = String.Format(
                    DirectCast(HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "AlienaRiga_NonRiuscita"), String),
                    Prodotto_Des)
            End If


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

#Region "Gestore Carica Giacenze"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGiacenze(ByVal Sa_Cod As String,
                                          ByVal strFabbricato_Cod As String,
                                          ByVal Tipo_Fabbricato As Integer,
                                          ByVal Elem_Cod As String,
                                          ByVal NomeProdotto As String,
                                          ByVal CodArticolo As String,
                                          ByVal CodiceProdotto As String,
                                          ByVal Lotto As String,
                                          ByVal Data As String,
                                          ByVal VisualizzaGiacenzeZero As String,
                                          ByVal CifreArrotondamento As String,
                                          ByVal Kendo As Boolean,
                                          ByVal ValorizzaProdotto As Boolean
                                          ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametriAgenda As New ParametriAgenda
        Dim Piva As String = objParametriAgenda.Piva

        Try

            If Piva = "" Then
                Throw New Exception("Piva non impostata")
            End If

            Dim str_Risposta As String
            Dim dt As New DataTable

            Dim Fabbricato_Cod As Integer = strFabbricato_Cod.Split("|")(0)

            Dim Mat_Cod As Integer = 0
            Dim Cal_Cod As Integer = 0
            Dim Cod_Progetto As Integer = 0
            Dim Fase_Cod As Integer = 0
            Dim Udm_Cod As Integer = 0
            Dim Pro_Cod As Integer

            Dim stringaFilter As String = ""


            If Lotto Is Nothing Then
                Lotto = LOTTO_NONDEFINITO
            End If

            If stringaFilter <> "" Then
                stringaFilter = String.Format("AND {0}", stringaFilter)
            End If

            Dim dataRicerca As Date

            If Data = "" Then
                dataRicerca = AGRODATAFINE
            Else
                dataRicerca = CDate(Data)
            End If

            If Elem_Cod = "" Then
                Elem_Cod = 0
            End If

            If CodiceProdotto <> "" AndAlso IsNumeric(CodiceProdotto) Then
                Pro_Cod = CodiceProdotto
            End If

            Dim leggiAnagrafeLog As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
            Dim isFreshAndFood As Boolean = leggiAnagrafeLog.IsFreshAndFood(Piva, Elem_Cod, objParametriServer)

            Dim dtParamQual As New DataTable
            Dim htParamQual As New Hashtable
            If isFreshAndFood Then
                Dim objConfigDettagli As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
                dtParamQual = objConfigDettagli.Leggi(Piva, 0, False, "Tipo = 1", "", objParametriServer)
            End If


            Dim objGruppiMerce As New Gruppi_Merce_R
            Dim ListaGruppiMercePerCategoria As List(Of ImpostazioneDefault_GruppiMerce) = objGruppiMerce.GetListGruppiMerceDefault(Piva, SACOD_NOFILTRO, objParametriUtenti, objParametriServer)
            Dim gestioneGruppiMerce As Boolean = ListaGruppiMercePerCategoria.Count > 0


            Dim objGiacenze As New AgronicaCoreContabDAL.Giacenze_R
            Dim dtGiacenze As DataTable = objGiacenze.SchedaGiacenzeMagazzino(dataRicerca,
                                                                              Piva,
                                                                              Sa_Cod,
                                                                              Fabbricato_Cod,
                                                                              Elem_Cod,
                                                                              Pro_Cod, Mat_Cod, Cal_Cod, Cod_Progetto, Fase_Cod, Udm_Cod, Lotto,
                                                                              Not CBool(VisualizzaGiacenzeZero),
                                                                              stringaFilter, "", "", "", "", "", "", "", "",
                                                                              "", "",
                                                                              "",
                                                                              objParametriServer, objParametriUtenti,
                                                                              isFreshAndFood:=isFreshAndFood,
                                                                              flagRecuperaCodArticolo:=True,
                                                                              codArticolo:=If(CodArticolo, ""),
                                                                              cercaCodArticoloPerLike:=True,
                                                                              gruppiMerceDefaultPerCategoria:=ListaGruppiMercePerCategoria)


            Dim numProdotti As Integer = 0
            Dim numRecord As Integer = 0

            Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim objCalibri As New AgronicaCoreMetaSchemaDAL.CalibriFrutti_R
            Dim objCampionature As New AgronicaCoreContabDAL.Materie_Prime_Campionature_R
            Dim objAnimaliDistinte As New AgronicaCoreAnagrafeDAL.Zoo_Animali_Distinte

            '----- Definisco la struttura del DataTable

            dt.Columns.Add(New DataColumn("piva", GetType(String)))
            dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
            dt.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))
            dt.Columns.Add(New DataColumn("Tipo_Destinazione", GetType(Integer)))
            dt.Columns.Add(New DataColumn("Fabbricato_Cod", GetType(Integer)))
            dt.Columns.Add(New DataColumn("Fabbricato_Des", GetType(String)))

            dt.Columns.Add(New DataColumn("Cat_Cod", GetType(Integer)))
            dt.Columns.Add(New DataColumn("Cat_Des", GetType(String)))
            dt.Columns.Add(New DataColumn("Pro_Cod", GetType(Integer)))

            dt.Columns.Add(New DataColumn("Cod_Articolo", GetType(String)))

            dt.Columns.Add(New DataColumn("Pro_Des", GetType(String)))
            dt.Columns.Add(New DataColumn("Mat_Cod", GetType(Integer)))

            dt.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
            dt.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
            dt.Columns.Add(New DataColumn("Qta", GetType(Decimal)))
            dt.Columns.Add(New DataColumn("Qta_Mag", GetType(Decimal)))

            dt.Columns.Add(New DataColumn("Lotto_Int", GetType(String)))
            dt.Columns.Add(New DataColumn("Lotto_Acc", GetType(String)))
            dt.Columns.Add(New DataColumn("Param_Des", GetType(String)))
            dt.Columns.Add(New DataColumn("Cal_Cod", GetType(Integer)))
            dt.Columns.Add(New DataColumn("Cal_Des", GetType(String)))
            dt.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(Decimal)))
            dt.Columns.Add(New DataColumn("Totale", GetType(Decimal)))
            dt.Columns.Add(New DataColumn("Cod_Progetto", GetType(String)))
            dt.Columns.Add(New DataColumn("Fase_Cod", GetType(String)))

            ' calcola valore prodotti in base alla tipologia impostata
            Dim Tipo_Valorizzazione As String() = {}
            Dim Valore_Medio_Ponderato As New Dictionary(Of String, Decimal)
            Dim Valore_Anagrafica_Prodotti As New Dictionary(Of String, Decimal)
            Dim Valore_Ultimo_Prodotto As New Dictionary(Of String, Decimal)
            If Not CBool(VisualizzaGiacenzeZero) AndAlso CBool(ValorizzaProdotto) Then
                Tipo_Valorizzazione = objGiacenze.Calcola_ValorizzazioneProdotti(
                    enum_Impostazioni_Utenti.SUPERUSER_TipoValorizzazioneCostiCdG,
                    Piva, dataRicerca, Elem_Cod, 0, 0, 0, "***Bypass***",
                    Valore_Medio_Ponderato, Valore_Anagrafica_Prodotti, Valore_Ultimo_Prodotto,
                    objParametriServer, objParametriUtenti)
                dt.Columns.Add(New DataColumn("Valore_Unitario", GetType(String)))
                dt.Columns.Add(New DataColumn("Valore_Globale", GetType(String)))
            End If


            If isFreshAndFood Then

                dt.Columns.Add(New DataColumn("Referenza", GetType(String)))
                dt.Columns.Add(New DataColumn("Fornitore", GetType(String)))

                For Each paramQual In dtParamQual.Rows

                    Select Case paramQual("Tipo")
                        Case 3
                            dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Val_Cod", GetType(Decimal)))
                        Case 4
                            dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Val_Cod", GetType(String)))
                        Case 5
                            dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Val_Cod", GetType(Date)))
                        Case Else
                            dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Tipo_Cod", GetType(Integer)))
                            If Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                                dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Tara_Campionatura", GetType(Decimal)))
                                dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Sigla", GetType(String)))
                                dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Descrizione", GetType(String)))
                                dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Codice_Generazione_Link", GetType(Integer)))
                                dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Mat_Cod_Generazione_Link", GetType(Integer)))
                            End If
                    End Select

                Next

                'Note
                dt.Columns.Add(New DataColumn("FF_ONote_Descrizione", GetType(String)))
            End If

            If gestioneGruppiMerce Then
                dt.Columns.Add(New DataColumn("Id_Gruppo_Merce", GetType(Integer)))
                dt.Columns.Add(New DataColumn("Des_Gruppo_Merce", GetType(String)))
            End If

            dt.Columns.Add(New DataColumn("chiave_giacenze", GetType(String)))

            If dtGiacenze IsNot Nothing AndAlso dtGiacenze.Rows.Count > 0 Then

                For i = 0 To dtGiacenze.Rows.Count - 1

                    Dim giacenza As Decimal = 0
                    If Not IsDBNull(dtGiacenze.Rows(i).Item("Giacenza")) AndAlso IsNumeric(dtGiacenze.Rows(i).Item("Giacenza")) Then
                        giacenza = CDec(dtGiacenze.Rows(i).Item("Giacenza"))
                    End If

                    'oltre al filtro nella query, devo fare anche il filtro su codice
                    'perché molti Decimal vengono salvati come valori infinitamente piccoli
                    'ad esempio 0.00003680000000017003
                    If CBool(VisualizzaGiacenzeZero) = True OrElse (giacenza <> 0 AndAlso Not (giacenza < QTA_GiancenzeVisualizzate AndAlso giacenza > -QTA_GiancenzeVisualizzate)) Then

                        numRecord += 1

                        Dim dr As DataRow

                        ' Giulia: 25/3/2019:Semplificata sezione seguente per gestione di presenza o meno di filtro su Nome Prodotto per evitare 300 righe duplicate

                        Dim elemCodRiga As Integer = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Elem_Cod")), dtGiacenze.Rows(i).Item("Elem_Cod"), 0)
                        Dim matCodRiga As Integer = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Mat_Cod")), dtGiacenze.Rows(i).Item("Mat_Cod"), 0)
                        Dim proCodRiga As Integer = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Pro_Cod")), dtGiacenze.Rows(i).Item("Pro_Cod"), 0)

                        Dim descrizione As String = dtGiacenze.Rows(i).Item("Descrizione_Prodotto")

                        If NomeProdotto = "" OrElse (NomeProdotto <> "" AndAlso InStr(descrizione, NomeProdotto, CompareMethod.Text) <> 0) Then
                            'Se non ho passato filtro nome prodotto oppure l'ho passato e la descrizione lo contiene

                            numProdotti += 1

                            'Creo una nuova riga
                            dr = dt.NewRow

                            'magazzino o cella
                            dr.Item("Piva") = dtGiacenze.Rows(i).Item("Piva")
                            dr.Item("Sa_Cod") = CInt(dtGiacenze.Rows(i).Item("Sa_Cod"))
                            dr.Item("Sa_Nome") = dtGiacenze.Rows(i).Item("Sa_Nome")

                            dr.Item("Tipo_Destinazione") = CInt(dtGiacenze.Rows(i).Item("Tipo_Destinazione"))
                            dr.Item("Fabbricato_Cod") = CInt(dtGiacenze.Rows(i).Item("Id_Destinazione"))

                            If isFreshAndFood = True AndAlso dr.Item("Tipo_Destinazione") = CELLA_FRIGORIFERA Then
                                dr.Item("Fabbricato_Des") = dtGiacenze.Rows(i).Item("Identificativo")
                            Else
                                dr.Item("Fabbricato_Des") = dtGiacenze.Rows(i).Item("Fabbricato_Des")
                            End If

                            'Definisco i valori
                            dr.Item("Cat_Cod") = elemCodRiga
                            dr.Item("Cat_Des") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("NomeComune")), dtGiacenze.Rows(i).Item("NomeComune"), "")
                            dr.Item("Pro_Cod") = proCodRiga

                            dr.Item("Mat_Cod") = matCodRiga

                            dr.Item("Cod_Articolo") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Cod_Articolo")), dtGiacenze.Rows(i).Item("Cod_Articolo"), "")

                            'Select Case matCodRiga
                            '    Case 0
                            '        dr.Item("Pro_Des") = descrizione ' + " (Cod:" + CStr(proCodRiga) + ")"
                            '    Case Else
                            dr.Item("Pro_Des") = descrizione '+ " (Cod:" + CStr(dr.Item("Cod_Articolo")) + ")"
                            'End Select

                            Dim codProgettoRiga As Integer = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Cod_Progetto")), dtGiacenze.Rows(i).Item("Cod_Progetto"), 0)
                            dr.Item("Cod_Progetto") = codProgettoRiga

                            'lo inizializzo in ogni caso
                            dr.Item("Lotto_Int") = ""

                            Select Case elemCodRiga

                                Case SEMILAVORATI_VEGETALI

                                    If codProgettoRiga = 0 Then
                                        dr.Item("Lotto_Int") = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "DaTerzi"), String)
                                    Else
                                        ' dr.Item("Lotto_Int") = objProgetto.ProgettoNome_from_ProgettoCod(codProgettoRiga, Nothing, objParametriServer)
                                        dr.Item("Lotto_Int") = dtGiacenze.Rows(i).Item("Lotto_Interno")

                                    End If



                                Case TRASFORMATI_VEGETALI
                                    'dr.Item("Lotto_Int") = objProgetto.ProgettoNome_from_ProgettoCod(codProgettoRiga, Nothing, objParametriServer)

                                    dr.Item("Lotto_Int") = dtGiacenze.Rows(i).Item("Lotto_Interno")


                                Case MATERIE_ANIMALI, SEMILAVORATI_ANIMALI, TRASFORMATI_ANIMALI

                                    If codProgettoRiga <> 0 Then
                                        Dim objAnimale = objAnimaliDistinte.Leggi(codProgettoRiga, objParametriServer)

                                        If objAnimale IsNot Nothing Then
                                            'dr.Item("Progetto") = objAnimale.Progetto
                                            dr.Item("Lotto_Int") = objAnimale.Codice_Distinta
                                        End If
                                    End If

                            End Select

                            dr.Item("Lotto_Acc") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Lotto")), dtGiacenze.Rows(i).Item("Lotto"), "")
                            dr.Item("Cal_Cod") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Cal_Cod")), dtGiacenze.Rows(i).Item("Cal_Cod"), 0)

                            'faccio il controllo altrimenti mi carica sempre calibro A per tutte le categorie di prodotto
                            Select Case dr.Item("Cal_Cod")

                                Case Is > 0
                                    dr.Item("Param_Des") = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "Calibro"), String)
                                    dr.Item("Cal_Des") = objCalibri.CalDes_from_CalCod(dr.Item("Cal_Cod"), objParametriServer)

                                Case Is < 0

                                    Dim dtCampionature As DataTable
                                    dtCampionature = objCampionature.Leggi(dr.Item("Cal_Cod"), "", 0, 0,
                                                                           0, "", True,
                                                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                           "", "", objParametriServer)

                                    If dtCampionature IsNot Nothing AndAlso dtCampionature.Rows.Count > 0 Then
                                        dr.Item("Param_Des") = ""
                                        dr.Item("Cal_Des") = dtCampionature.Rows(0).Item("Descrizione")
                                    End If
                                    dtCampionature = Nothing

                                Case 0
                                    dr.Item("Param_Des") = ""
                                    dr.Item("Cal_Des") = ""
                            End Select

                            dr.Item("Udm_Cod") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Udm_Cod")), dtGiacenze.Rows(i).Item("Udm_Cod"), 0)
                            dr.Item("Udm_Des") = dtGiacenze.Rows(i).Item("Udm_Sim")

                            '  Giulia, 05/04/2017 12:49:08: mi serve anche il valore di giacenza non arrotondato, per poter fare l'alienazione
                            dr.Item("Qta") = giacenza

                            Dim giacenza0 As Boolean = False

                            If CInt(CifreArrotondamento) >= 0 Then
                                giacenza = Agro_Math.ArrotondaVal(giacenza, CInt(CifreArrotondamento))
                            End If

                            If giacenza = 0 Then
                                giacenza0 = True
                            End If

                            dr.Item("Qta_Mag") = giacenza

                            dr.Item("Fase_Cod") = If(Not IsDBNull(dtGiacenze.Rows(i).Item("Fase_Cod")), dtGiacenze.Rows(i).Item("Fase_Cod"), 0)

                            If Tipo_Valorizzazione.Length > 0 AndAlso Not CBool(VisualizzaGiacenzeZero) AndAlso CBool(ValorizzaProdotto) Then

                                Dim prezzo_unitario As Decimal = 0

                                If dr.Item("Cat_Cod") <> 0 AndAlso dr.Item("Cat_Cod") <> 501 Then

                                    Dim chiave As String = dr.Item("Piva") & "_" & dr.Item("Cat_Cod") & "_" & dr.Item("Pro_Cod") & "_" & dr.Item("Mat_Cod") & "_" & dr.Item("Udm_Cod")

                                    Dim chiave_lotto As String = ""
                                    If Tipo_Valorizzazione.Contains("10") OrElse Tipo_Valorizzazione.Contains("40") Then
                                        chiave_lotto = dr.Item("Piva") & "_" & dr.Item("Cat_Cod") & "_" & dr.Item("Pro_Cod") & "_" & dr.Item("Mat_Cod") & "_" & dr.Item("Udm_Cod") & "_" & dr.Item("Lotto_Acc")
                                    End If

                                    For Each tipo In Tipo_Valorizzazione

                                            ' calcolo valore medio ponderato
                                            If tipo = "1" AndAlso Valore_Medio_Ponderato.ContainsKey(chiave) Then
                                                prezzo_unitario = Valore_Medio_Ponderato(chiave)
                                            End If

                                        ' calcolo valore medio ponderato con lotto valorizzato
                                        If tipo = "10" AndAlso Valore_Medio_Ponderato.ContainsKey(chiave_lotto) Then
                                            prezzo_unitario = Valore_Medio_Ponderato(chiave_lotto)
                                        End If

                                        ' calcolo valore da tabella costi
                                        If tipo = "3" AndAlso Valore_Anagrafica_Prodotti.ContainsKey(chiave) Then
                                                prezzo_unitario = Valore_Anagrafica_Prodotti(chiave)
                                            End If

                                            ' calcolo valore da ultimo prodotto
                                            If tipo = "4" AndAlso Valore_Ultimo_Prodotto.ContainsKey(chiave) Then
                                                prezzo_unitario = Valore_Ultimo_Prodotto(chiave)
                                            End If

                                        ' calcolo valore da ultimo prodotto con lotto valorizzato
                                        If tipo = "40" AndAlso Valore_Ultimo_Prodotto.ContainsKey(chiave_lotto) Then
                                            prezzo_unitario = Valore_Ultimo_Prodotto(chiave_lotto)
                                        End If

                                        If prezzo_unitario <> 0 Then
                                                Exit For
                                            End If

                                        Next

                                    End If

                                    dr.Item("Valore_Unitario") = prezzo_unitario
                                dr.Item("Valore_Globale") = Format(prezzo_unitario * giacenza, "###,###,##0.00")

                            End If

                            If isFreshAndFood Then
                                For Each paramQual In dtParamQual.Rows
                                    'Parametri qualitativi inseriti dall'utente

                                    Select Case paramQual("Tipo")

                                        Case 3

                                            If dtGiacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod") <> "" Then
                                                dr.Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod") = CDec(dtGiacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod").Replace(".", SeparatoreDecimaleVB))
                                            End If

                                        Case 4

                                            dr.Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod") = dtGiacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod")

                                        Case 5

                                            If IsDate(dtGiacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod")) Then
                                                dr.Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod") = CDate(dtGiacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod")).ToShortDateString
                                            End If

                                        Case Else

                                            'Parametri qualitativi da DDL
                                            If Not ({"cliente"}).Contains(paramQual("Tabella_Key")) AndAlso
                                               CInt(dtGiacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Tipo_Cod")) <> 0 AndAlso
                                               Not htParamQual.ContainsKey(paramQual("Tabella_Key")) Then
                                                htParamQual.Add(paramQual("Tabella_Key"), True)
                                            End If

                                            dr.Item("FF_" & paramQual("Tabella_Key") & "_Tipo_Cod") = dtGiacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Tipo_Cod")
                                            If Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                                                dr.Item("FF_" & paramQual("Tabella_Key") & "_Tara_Campionatura") = dtGiacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Tara_Campionatura")
                                                dr.Item("FF_" & paramQual("Tabella_Key") & "_Sigla") = dtGiacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Sigla")
                                                dr.Item("FF_" & paramQual("Tabella_Key") & "_Descrizione") = dtGiacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Descrizione")
                                                dr.Item("FF_" & paramQual("Tabella_Key") & "_Codice_Generazione_Link") = dtGiacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Codice_Generazione_Link")
                                                dr.Item("FF_" & paramQual("Tabella_Key") & "_Mat_Cod_Generazione_Link") = dtGiacenze.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Mat_Cod_Generazione_Link")

                                                'Valorizzazione colonna Referenza
                                                If Not ({"imballaggio", "contenitore", "confezione"}).Contains(paramQual("Tabella_Key")) Then
                                                    'If Not String.IsNullOrEmpty(dr.Item("FF_" & paramQual("Tabella_Key") & "_Sigla")) Then
                                                    '    dr.Item("Referenza") &= "<br/>" & dr.Item("FF_" & paramQual("Tabella_Key") & "_Sigla")
                                                    'Else
                                                    If Not String.IsNullOrEmpty(dr.Item("FF_" & paramQual("Tabella_Key") & "_Descrizione")) Then
                                                        dr.Item("Referenza") &= "<br/>" & dr.Item("FF_" & paramQual("Tabella_Key") & "_Descrizione")
                                                    End If
                                                    'End If
                                                    'Imballaggio, Contenitore e Descrizione vengono mostrati in colonne a parte
                                                    'Else
                                                    '    If Not String.IsNullOrEmpty(dr.Item("FF_" & paramQual("Tabella_Key") & "_Descrizione")) Then
                                                    '        dr.Item("Referenza") &= " " & dr.Item("FF_" & paramQual("Tabella_Key") & "_Descrizione")
                                                    '    End If
                                                End If
                                            End If

                                            If ({"fornitore"}).Contains(paramQual("Tabella_Key")) Then
                                                Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R
                                                Dim codRisUmFornitore As String = dtGiacenze.Rows(i).Item("FF_fornitore_Tipo_Cod").ToString
                                                If Not String.IsNullOrEmpty(codRisUmFornitore) Then
                                                    Dim objContatto As DataTable = objContattiR.RagSoc_Nome_Cognome_RapportoDes_from_Cod_Risum(codRisUmFornitore, objParametriServer)
                                                    If objContatto.Rows.Count > 0 Then
                                                        dr.Item("Fornitore") = objContatto.Rows(0)("rag_soc")
                                                        'Fornitore viene mostrato in colonne a parte
                                                        'dr.Item("Referenza") &= " " & dr.Item("Fornitore")
                                                    End If
                                                End If
                                            End If

                                    End Select

                                Next

                                'TODO: Note --> Non c'è?!?
                                'dr.Item("FF_ONote_Descrizione") = dtGiacenze.Rows(i).Item("FF_ONote_Descrizione")
                            End If

                            If gestioneGruppiMerce Then
                                dr.Item("Id_Gruppo_Merce") = dtGiacenze.Rows(i).Item("Id_Gruppo_Merce")
                                dr.Item("Des_Gruppo_Merce") = dtGiacenze.Rows(i).Item("Des_Gruppo_Merce")
                            End If


                            dr.Item("chiave_giacenze") = dr.Item("piva") & "_" &
                                                         dr.Item("sa_cod") & "_" &
                                                         dr.Item("fabbricato_cod") & "_" &
                                                         dr.Item("cat_cod") & "_" &
                                                         dr.Item("pro_cod") & "_" &
                                                         dr.Item("mat_cod") & "_" &
                                                         If(Kendo, StringToHex(dr.Item("Lotto_Acc")), dr.Item("Lotto_Acc")) & "_" &
                                                         dr.Item("cal_cod") & "_" &
                                                         dr.Item("cod_progetto") & "_" &
                                                         dr.Item("udm_cod")

                            If Not giacenza0 OrElse CBool(VisualizzaGiacenzeZero) Then
                                'Associo alla tabella la nuova riga creata
                                dt.Rows.Add(dr)
                            End If

                        End If 'ricerca prodotto o no

                    End If 'Filtro arrotondamento giacenza

                Next 'ciclo giacenze

            End If

            'If Kendo Then
            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            str_Risposta = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            'Else
            '    str_Risposta = DT_to_Json_Giacenze(dt)
            'End If

            r.RispostaOK = True
            r.RispostaStringa = str_Risposta

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    Public Shared Function StringToHex(ByVal text As String) As String
        Dim text_byte = Encoding.Unicode.GetBytes(text)
        Dim text_hex = BitConverter.ToString(text_byte)
        Return Replace(text_hex, "-", "")
    End Function

    Public Shared Function HexToString(ByVal hex As String) As String
        Dim NumberChars As Integer = hex.Length
        Dim bytes As Byte() = New Byte(NumberChars / 2 - 1) {}
        For i As Integer = 0 To NumberChars - 1 Step 2
            bytes(i / 2) = Convert.ToByte(hex.Substring(i, 2), 16)
        Next
        Return Encoding.Unicode.GetString(bytes)
    End Function

#End Region


#Region "Gestore Carica Movimenti"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaMovimenti(ByVal Sa_Cod As String,
                                           ByVal strFabbricato_Cod As String,
                                           ByVal Tipo_Fabbricato As Integer,
                                           ByVal Elem_Cod As String,
                                           ByVal NomeProdotto As String,
                                           ByVal Cod_Articolo As String,
                                           ByVal Pro_Cod As String,
                                           ByVal Mat_Cod As String,
                                           ByVal Lotto As String,
                                           ByVal Cal_Cod As String,
                                           ByVal Cod_Progetto As String,
                                           ByVal Udm_Cod As String,
                                           ByVal DataInizio As String,
                                           ByVal DataFine As String,
                                           ByVal VisualizzaCarichi As String,
                                           ByVal VisualizzaScarichi As String,
                                           ByVal CifreArrotondamento As String,
                                           ByVal Kendo As Boolean
                                           ) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Dim listaCauCarico As New List(Of String) From {CAU_CARICO, CAU_CONFERIMENTO, CAU_ACCETTAZIONE_BENI_DA_DIVERSI}
        Dim listaCauScarico As New List(Of String) From {CAU_SCARICO, CAU_CONFERIMENTO_DIVERSI, CAU_ACCETTAZIONE_BENI}

        Dim objParametriAgenda As New ParametriAgenda
        Dim Piva As String = objParametriAgenda.Piva

        Try

            If String.IsNullOrWhiteSpace(Piva) Then
                Throw New Exception("Impresa (p.iva) non specificata")
            End If

            Dim str_Risposta As String
            Dim dt As New DataTable

            Dim Fabbricato_Cod As Integer = strFabbricato_Cod.Split("|")(0)
            Dim Fase_Cod As Integer = 0

            Dim stringaFilter As String = ""

            '20/06/2018: patch su filtro lotto (se è indefinito deve filtrare esattamente indefinito)
            '21/10/2022: rimuovendo lotto indefinito, occorre sempre filtrare per il lotto fornito, eccetto quando passato come nothing
            If Lotto Is Nothing Then
                Lotto = LOTTO_NONDEFINITO
            End If
            'If Lotto.Trim <> "" And Lotto.ToLower.Trim <> "indefinito" Then
            'Else
            '    Lotto = CStr(CInt(LOTTO_NONDEFINITO))
            'End If


            If stringaFilter <> "" Then
                stringaFilter = String.Format("AND {0}", stringaFilter)
            End If

            If DataInizio = "" Then
                DataInizio = AGRODATAINIZIO.ToShortDateString
            End If

            If DataFine = "" Then
                DataFine = AGRODATAFINE.ToShortDateString
            End If


            If Elem_Cod = "" Then
                Elem_Cod = 0
            End If

            If Not (Pro_Cod <> "" AndAlso IsNumeric(Pro_Cod)) Then
                Pro_Cod = 0
            End If
            If Not (Mat_Cod <> "" AndAlso IsNumeric(Mat_Cod)) Then
                Mat_Cod = 0
            End If
            If Not (Cal_Cod <> "" AndAlso IsNumeric(Cal_Cod)) Then
                Cal_Cod = 0
            End If
            If Not (Cod_Progetto <> "" AndAlso IsNumeric(Cod_Progetto)) Then
                Cod_Progetto = 0
            End If
            If Not (Udm_Cod <> "" AndAlso IsNumeric(Udm_Cod)) Then
                Udm_Cod = 0
            End If

            'Carico e Scarico
            Dim filterCarichi As String = ""
            Dim filterScarichi As String = ""

            If CBool(VisualizzaCarichi) = True Then
                filterCarichi = " Cau_Mov IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "') "
            End If

            If CBool(VisualizzaScarichi) = True Then
                filterScarichi = " Cau_Mov IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "','" & CAU_ACCETTAZIONE_BENI & "') "
            End If

            If filterCarichi <> "" AndAlso filterScarichi <> "" Then
                stringaFilter &= " AND ( " & filterCarichi & " OR " & filterScarichi & " ) "
            ElseIf filterCarichi <> "" Then
                stringaFilter &= " AND ( " & filterCarichi & " ) "
            ElseIf filterScarichi <> "" Then
                stringaFilter &= " AND ( " & filterScarichi & " ) "
            End If

            Dim leggiAnagrafeLog As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
            Dim isFreshAndFood As Boolean = leggiAnagrafeLog.IsFreshAndFood(Piva, Elem_Cod, objParametriServer)

            Dim dtParamQual As New DataTable
            Dim htParamQual As New Hashtable
            If isFreshAndFood Then
                Dim objConfigDettagli As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
                dtParamQual = objConfigDettagli.Leggi(Piva, 0, False, "Tipo = 1", "", objParametriServer)
            End If


            Dim objGruppiMerce As New Gruppi_Merce_R
            Dim ListaGruppiMercePerCategoria As List(Of ImpostazioneDefault_GruppiMerce) = objGruppiMerce.GetListGruppiMerceDefault(Piva, SACOD_NOFILTRO, objParametriUtenti, objParametriServer)
            Dim gestioneGruppiMerce As Boolean = ListaGruppiMercePerCategoria.Count > 0


            Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R
            Dim dtMovimenti As DataTable = objMovimenti.SchedaMovimentiMagazzino(CDate(DataInizio),
                                                                                 CDate(DataFine),
                                                                                 Piva,
                                                                                 CInt(Sa_Cod),
                                                                                 Fabbricato_Cod,
                                                                                 CInt(Elem_Cod),
                                                                                 CInt(Pro_Cod),
                                                                                 CInt(Mat_Cod),
                                                                                 CInt(Cal_Cod),
                                                                                 CInt(Cod_Progetto),
                                                                                 Fase_Cod,
                                                                                 CInt(Udm_Cod),
                                                                                 Lotto,
                                                                                 stringaFilter, "", "", "", "", "", "", "", "", "", "",
                                                                                 "Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Cod_Progetto, Movimenti_dettagli.Lotto, Movimenti_dettagli.Cal_Cod, Movimenti_dettagli.Udm_Cod, Movimenti.Data_Movimento",
                                                                                 objParametriServer, objParametriUtenti,
                                                                                 flagRecuperaCodArticolo:=True,
                                                                                 codArticolo:=If(Cod_Articolo, ""),
                                                                                 cercaCodArticoloPerLike:=True,
                                                                                 isFreshAndFood:=isFreshAndFood,
                                                                                 gruppiMerceDefaultPerCategoria:=ListaGruppiMercePerCategoria)


            Dim numMovimenti As Integer = 0
            Dim numRecord As Integer = 0

            Dim objMat As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            Dim objCalibri As New AgronicaCoreMetaSchemaDAL.CalibriFrutti_R
            Dim objIndice As New AgronicaCoreMetaSchemaDAL.IndiciMaturitaxSpecie_R
            Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim objMovDetRif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R

            '----- Definisco la struttura del DataTable

            dt.Columns.Add(New DataColumn("piva", GetType(String)))
            dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
            dt.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))
            dt.Columns.Add(New DataColumn("Tipo_Destinazione", GetType(Integer)))
            dt.Columns.Add(New DataColumn("Fabbricato_Cod", GetType(Integer)))
            dt.Columns.Add(New DataColumn("Fabbricato_Des", GetType(String)))

            dt.Columns.Add(New DataColumn("Cat_Cod", GetType(Integer)))
            dt.Columns.Add(New DataColumn("Cat_Des", GetType(String)))
            dt.Columns.Add(New DataColumn("Pro_Cod", GetType(Integer)))

            dt.Columns.Add(New DataColumn("Cod_Articolo", GetType(String)))

            dt.Columns.Add(New DataColumn("Pro_Des", GetType(String)))
            dt.Columns.Add(New DataColumn("Mat_Cod", GetType(Integer)))

            dt.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
            dt.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
            dt.Columns.Add(New DataColumn("Qta", GetType(Decimal)))
            dt.Columns.Add(New DataColumn("Qta_Dest", GetType(Decimal)))
            dt.Columns.Add(New DataColumn("Qta_Mag", GetType(Decimal)))

            dt.Columns.Add(New DataColumn("Data", GetType(Date)))
            dt.Columns.Add(New DataColumn("Lav_Des", GetType(String)))

            dt.Columns.Add(New DataColumn("Dettagli", GetType(String)))

            dt.Columns.Add(New DataColumn("Doc_Numero", GetType(String)))
            dt.Columns.Add(New DataColumn("Contatto", GetType(String)))

            dt.Columns.Add(New DataColumn("Lotto_Int", GetType(String)))
            dt.Columns.Add(New DataColumn("Lotto_Acc", GetType(String)))
            dt.Columns.Add(New DataColumn("Mov_Det_Extra_Str", GetType(String)))
            dt.Columns.Add(New DataColumn("Param_Des", GetType(String)))
            dt.Columns.Add(New DataColumn("Cal_Cod", GetType(Integer)))
            dt.Columns.Add(New DataColumn("Cal_Des", GetType(String)))
            dt.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(Decimal)))
            dt.Columns.Add(New DataColumn("Totale", GetType(Decimal)))
            dt.Columns.Add(New DataColumn("Cod_Progetto", GetType(String)))
            dt.Columns.Add(New DataColumn("Fase_Cod", GetType(String)))

            dt.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
            dt.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))

            dt.Columns.Add(New DataColumn("Cau_Mov", GetType(String)))
            dt.Columns.Add(New DataColumn("Cau_Des", GetType(String)))
            dt.Columns.Add(New DataColumn("Blocco_Flag", GetType(String)))
            dt.Columns.Add(New DataColumn("Info", GetType(String)))

            dt.Columns.Add(New DataColumn("DescrizioneOpearazione", GetType(String)))

            dt.Columns.Add(New DataColumn("Modificabile", GetType(String)))
            dt.Columns.Add(New DataColumn("Eliminabile", GetType(String)))

            If isFreshAndFood Then

                dt.Columns.Add(New DataColumn("Referenza", GetType(String)))
                dt.Columns.Add(New DataColumn("Fornitore", GetType(String)))

                For Each paramQual In dtParamQual.Rows

                    Select Case paramQual("Tipo")
                        Case 3
                            dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Val_Cod", GetType(Decimal)))
                        Case 4
                            dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Val_Cod", GetType(String)))
                        Case 5
                            dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Val_Cod", GetType(Date)))
                        Case Else
                            dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Tipo_Cod", GetType(Integer)))
                            If Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                                dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Tara_Campionatura", GetType(Decimal)))
                                dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Sigla", GetType(String)))
                                dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Descrizione", GetType(String)))
                                dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Codice_Generazione_Link", GetType(Integer)))
                                dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Mat_Cod_Generazione_Link", GetType(Integer)))
                            End If
                    End Select

                Next

                'Note
                dt.Columns.Add(New DataColumn("FF_ONote_Descrizione", GetType(String)))
            End If

            If gestioneGruppiMerce Then
                dt.Columns.Add(New DataColumn("Id_Gruppo_Merce", GetType(Integer)))
                dt.Columns.Add(New DataColumn("Des_Gruppo_Merce", GetType(String)))
            End If

            dt.Columns.Add(New DataColumn("chiave_movimenti", GetType(String)))


            Dim totCarico As Decimal = 0D
            Dim totScarico As Decimal = 0D

            If dtMovimenti IsNot Nothing AndAlso dtMovimenti.Rows.Count > 0 Then

                'estraggo i dati dei movimenti contabili (x num doc e contatto)
                Dim objSqlDis As New AgronicaCoreDataProvider.DatatableUtility
                Dim strId_Agenda() As String = objSqlDis.SelectDistinct(dtMovimenti, "id_agenda", False)

                Dim strIDAgenda As String = ""
                Dim dtMov As DataTable = Nothing

                If strId_Agenda IsNot Nothing AndAlso strId_Agenda.Length > 0 Then
                    strIDAgenda = String.Join(",", strId_Agenda)
                    If strIDAgenda <> "" Then
                        Dim objMov As New AgronicaCoreContabDAL.Movimenti_R
                        dtMov = objMov.MovimentiContabili_Contatto(0, "", 0, 0, 0, 0, CAU_REGISTRAZIONI, 0, AGRODATAINIZIO, AGRODATAFINE, "XYZ", 0, "XYZ", 0, 0, AGRODATAINIZIO, " agenda.id_agenda IN (" & strIDAgenda & ") ", "", objParametriServer)
                    End If
                End If

                Dim dr As DataRow

                For i = 0 To dtMovimenti.Rows.Count - 1

                    Dim qtaDest As Decimal = 0
                    If Not IsDBNull(dtMovimenti.Rows(i).Item("Qta_Dest")) AndAlso IsNumeric(Not IsDBNull(dtMovimenti.Rows(i).Item("Qta_Dest"))) Then
                        qtaDest = CDec(dtMovimenti.Rows(i).Item("Qta_Dest"))
                        If CInt(CifreArrotondamento) >= 0 Then
                            qtaDest = Agro_Math.ArrotondaVal(qtaDest, CifreArrotondamento)
                        End If
                    End If

                    ' Giulia: 25/3/2019:Semplificata sezione seguente per gestione di presenza o meno di filtro su Nome Prodotto per evitare 300 righe duplicate

                    Dim elemCodRiga As Integer = If(Not IsDBNull(dtMovimenti.Rows(i).Item("Elem_Cod")), dtMovimenti.Rows(i).Item("Elem_Cod"), 0)
                    Dim matCodRiga As Integer = If(Not IsDBNull(dtMovimenti.Rows(i).Item("Mat_Cod")), dtMovimenti.Rows(i).Item("Mat_Cod"), 0)
                    Dim proCodRiga As Integer = If(Not IsDBNull(dtMovimenti.Rows(i).Item("Pro_Cod")), dtMovimenti.Rows(i).Item("Pro_Cod"), 0)

                    Dim descrizione As String = dtMovimenti.Rows(i).Item("Descrizione_Prodotto")

                    If NomeProdotto = "" OrElse (NomeProdotto <> "" AndAlso InStr(descrizione, NomeProdotto, CompareMethod.Text) <> 0) Then
                        'Se non ho passato filtro nome prodotto oppure l'ho passato e la descrizione lo contiene

                        dr = dt.NewRow

                        dr.Item("Piva") = dtMovimenti.Rows(i).Item("Piva")
                        dr.Item("Sa_Cod") = dtMovimenti.Rows(i).Item("Sa_Cod")
                        dr.Item("Sa_Nome") = dtMovimenti.Rows(i).Item("Sa_Nome")

                        dr.Item("Tipo_Destinazione") = CInt(dtMovimenti.Rows(i).Item("Tipo_Destinazione"))
                        dr.Item("Fabbricato_Cod") = CInt(dtMovimenti.Rows(i).Item("Id_Destinazione"))

                        If isFreshAndFood = True AndAlso dr.Item("Tipo_Destinazione") = CELLA_FRIGORIFERA Then
                            dr.Item("Fabbricato_Des") = dtMovimenti.Rows(i).Item("Identificativo")
                        Else
                            dr.Item("Fabbricato_Des") = dtMovimenti.Rows(i).Item("Fabbricato_Des")
                        End If

                        dr.Item("Cat_Cod") = elemCodRiga
                        dr.Item("Cat_Des") = dtMovimenti.Rows(i).Item("Cat_Des")
                        dr.Item("Pro_Cod") = proCodRiga
                        dr.Item("Mat_Cod") = matCodRiga

                        dr.Item("Cod_Articolo") = If(Not IsDBNull(dtMovimenti.Rows(i).Item("Cod_Articolo")), dtMovimenti.Rows(i).Item("Cod_Articolo"), "")

                        'Select Case matCodRiga
                        '    Case 0
                        '        dr.Item("Pro_Des") = descrizione '+ " (Cod:" + CStr(proCodRiga) + ")"
                        '    Case Else
                        dr.Item("Pro_Des") = descrizione '+ " (Cod:" + CStr(dr.Item("Cod_Articolo")) + ")"
                        'End Select

                        Dim codProgettoRiga As Integer = If(Not IsDBNull(dtMovimenti.Rows(i).Item("Cod_Progetto")), dtMovimenti.Rows(i).Item("Cod_Progetto"), 0)
                        dr.Item("Cod_Progetto") = codProgettoRiga

                        'lo inizializzo in ogni caso
                        dr.Item("Lotto_Int") = ""

                        Select Case elemCodRiga

                            Case SEMILAVORATI_VEGETALI

                                If codProgettoRiga = 0 Then
                                    dr.Item("Lotto_Int") = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "DaTerzi"), String)
                                Else
                                    'dr.Item("Lotto_Int") = objProgetto.ProgettoNome_from_ProgettoCod(codProgettoRiga, Nothing, objParametriServer)
                                    dr.Item("Lotto_Int") = dtMovimenti.Rows(i).Item("Lotto_Interno")
                                End If

                            Case TRASFORMATI_VEGETALI
                                'dr.Item("Lotto_Int") = objProgetto.ProgettoNome_from_ProgettoCod(codProgettoRiga, Nothing, objParametriServer)

                                dr.Item("Lotto_Int") = dtMovimenti.Rows(i).Item("Lotto_Interno")

                            Case MATERIE_ANIMALI, SEMILAVORATI_ANIMALI, TRASFORMATI_ANIMALI

                                If codProgettoRiga <> 0 Then
                                    Dim objAnimaliDistinte As New AgronicaCoreAnagrafeDAL.Zoo_Animali_Distinte
                                    Dim objAnimale = objAnimaliDistinte.Leggi(codProgettoRiga, objParametriServer)

                                    If objAnimale IsNot Nothing Then
                                        'dr.Item("Progetto") = objAnimale.Progetto
                                        dr.Item("Lotto_Int") = objAnimale.Codice_Distinta
                                    End If
                                End If

                        End Select

                        dr.Item("Lotto_Acc") = If(Not IsDBNull(dtMovimenti.Rows(i).Item("Lotto")), dtMovimenti.Rows(i).Item("Lotto"), "")
                        dr.Item("Cal_Cod") = If(Not IsDBNull(dtMovimenti.Rows(i).Item("Cal_Cod")), dtMovimenti.Rows(i).Item("Cal_Cod"), 0)

                        'altrimenti mi carica il calibro A per tutte le categorie di magazzino
                        Select Case dr.Item("Cal_Cod")

                            Case Is > 0
                                dr.Item("Param_Des") = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "Calibro"), String)
                                dr.Item("Cal_Des") = objCalibri.CalDes_from_CalCod(dr.Item("Cal_Cod"), objParametriServer)

                            Case Is < 0

                                'leggo il parametro di qualità
                                Dim vegCod As Integer = 0
                                objMat.VegCod_CulCod_from_MatCod(Piva, elemCodRiga, matCodRiga, vegCod, 0, objParametriServer)
                                Dim dtIndici = objIndice.Leggi_ParametroQualita(vegCod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)

                                If dtIndici IsNot Nothing AndAlso dtIndici.Rows.Count > 0 Then
                                    dr.Item("Param_Des") = dtIndici.Rows(0).Item("Ind_Mat_Des")
                                    dr.Item("Cal_Des") = -dr.Item("Cal_Cod")
                                End If
                                dtIndici = Nothing

                            Case 0
                                dr.Item("Param_Des") = ""
                                dr.Item("Cal_Des") = ""
                        End Select

                        dr.Item("Udm_Cod") = If(Not IsDBNull(dtMovimenti.Rows(i).Item("Udm_Cod")), dtMovimenti.Rows(i).Item("Udm_Cod"), 0)
                        dr.Item("Udm_Des") = dtMovimenti.Rows(i).Item("Udm_Sim")
                        dr.Item("Qta_Dest") = qtaDest
                        dr.Item("Data") = If(Not IsDBNull(dtMovimenti.Rows(i).Item("Data_Movimento")), CDate(dtMovimenti.Rows(i).Item("Data_Movimento")).ToShortDateString, "")
                        'TODO: GIULIA: Why cau_mov = 0?!?
                        dr.Item("Cau_Mov") = If(Not IsDBNull(dtMovimenti.Rows(i).Item("Cau_Mov")), dtMovimenti.Rows(i).Item("Cau_Mov"), 0)
                        dr.Item("Cau_Des") = dr.Item("Cau_Mov")
                        dr.Item("Info") = If(Not IsDBNull(dtMovimenti.Rows(i).Item("Mov_Desc")), dtMovimenti.Rows(i).Item("Mov_Desc"), "")
                        dr.Item("Mov_Det_Extra_Str") = If(Not IsDBNull(dtMovimenti.Rows(i).Item("Mov_Det_Extra_Str")), dtMovimenti.Rows(i).Item("Mov_Det_Extra_Str"), "")

                        'dr.Item("Prezzo_Unitario") = If(Not IsDBNull(dtMovimenti.Rows(i).Item("Prezzo_Unitario")), dtMovimenti.Rows(i).Item("Prezzo_Unitario"), 0)
                        dr.Item("Id_Agenda") = CInt(dtMovimenti.Rows(i).Item("Id_Agenda"))
                        dr.Item("Blocco_Flag") = dtMovimenti.Rows(i).Item("Blocco_Flag")

                        dr.Item("Fase_Cod") = dtMovimenti.Rows(i).Item("Fase_Cod")

                        If listaCauCarico.Contains(dr.Item("Cau_Mov")) Then
                            dr.Item("Qta") = qtaDest
                            totCarico += CDec(dr.Item("Qta_Dest"))
                        Else
                            dr.Item("Qta") = -qtaDest
                            totScarico += CDec(dr.Item("Qta_Dest"))
                        End If

                        Dim lavCod As Integer = CInt(dtMovimenti.Rows(i).Item("Lav_Cod"))
                        dr.Item("Lav_Cod") = lavCod

                        dr.Item("Lav_Des") = dtMovimenti.Rows(i).Item("Lav_Des")
                        dr.Item("Dettagli") = dtMovimenti.Rows(i).Item("Des_Lib")

                        Select Case lavCod
                            Case LAVCOD_CARICO, LAVCOD_SCARICO
                                If Not IsDBNull(dtMovimenti.Rows(i).Item("pendente")) AndAlso IsNumeric(dtMovimenti.Rows(i).Item("pendente")) Then
                                    Select Case CInt(dtMovimenti.Rows(i).Item("pendente")) 'i18n
                                        Case enum_Pendenza.GiacenzeIniziali
                                            dr.Item("Lav_Des") = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "RilievoGiacenzeIniziali"), String) & " - " & dr.Item("Lav_Des")
                                        Case enum_Pendenza.AutoProduzione
                                            dr.Item("Lav_Des") = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "BeniAutoprodotti"), String) & " - " & dr.Item("Lav_Des")
                                        Case enum_Pendenza.AutoConsumo
                                            dr.Item("Lav_Des") = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "Autoconsumo"), String) & " - " & dr.Item("Lav_Des")
                                        Case enum_Pendenza.Smaltimento
                                            dr.Item("Lav_Des") = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "Smaltimento"), String) & " - " & dr.Item("Lav_Des")
                                        Case enum_Pendenza.Furto
                                            dr.Item("Lav_Des") = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "Furto"), String) & " - " & dr.Item("Lav_Des")
                                        Case enum_Pendenza.Trasferimento
                                            dr.Item("Lav_Des") = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneMagazzini/GestioneMagazziniBS.aspx", "TrasferimentoMerci"), String) & " - " & dr.Item("Lav_Des")
                                    End Select
                                End If
                        End Select

                        dr.Item("Modificabile") = "1"
                        dr.Item("Eliminabile") = "1"

                        Dim Prezzo_Unitario As Decimal = 0
                        Dim Prezzo_Unitario_Netto As Decimal = 0
                        Dim Prezzo_Unitario_DocAllegato As Decimal = 0
                        Dim Prezzo_Unitario_Netto_DocAllegato As Decimal = 0

                        Select Case lavCod

                            Case LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO

                                Dim lavCodFattura As Integer
                                Dim cauMovimento As String

                                If lavCod = LAVCOD_BOLLA_RICEVUTA Then
                                    lavCodFattura = LAVCOD_FATTURA_RICEVUTA
                                    cauMovimento = CAU_CARICO
                                Else
                                    lavCodFattura = LAVCOD_FATTURA_EMESSA
                                    cauMovimento = CAU_SCARICO
                                End If

                                objMovDetRif.Recupera_PrezzoUnitario_FatturaAllegata(objParametriServer,
                                                                                     Prezzo_Unitario_DocAllegato,
                                                                                     Prezzo_Unitario_Netto_DocAllegato,
                                                                                     CStr(Piva),
                                                                                     0,
                                                                                     dtMovimenti.Rows(i).Item("Id_Agenda"),
                                                                                     0,
                                                                                     dtMovimenti.Rows(i).Item("Id_Mov_Det"),
                                                                                     lavCod,
                                                                                     CAU_REGISTRAZIONI,
                                                                                     lavCodFattura,
                                                                                     cauMovimento)

                                If lavCod = LAVCOD_BOLLA_EMESSA Then

                                    Dim dtRif As DataTable = objMovDetRif.Recupera_DT_Rif_Unificato(dtMovimenti.Rows(i).Item("Piva"),
                                                                                                    0,
                                                                                                    dtMovimenti.Rows(i).Item("Id_Agenda"),
                                                                                                    0, 0, 0, "", objParametriServer)

                                    If dtRif.Rows.Count <> 0 Then
                                        For Each drRif As DataRow In dtRif.Rows
                                            Select Case drRif.Item("Lav_Cod_Risultato")
                                                Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME,
                                                    LAVCOD_GEODISINFESTAZIONE, LAVCOD_DISSECCAMENTO, LAVCOD_DISERBO
                                                    dr.Item("Modificabile") = "0"
                                                    dr.Item("Eliminabile") = "0"
                                                    Exit For
                                            End Select
                                        Next
                                    End If

                                End If

                            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME,
                                LAVCOD_GEODISINFESTAZIONE, LAVCOD_DISSECCAMENTO, LAVCOD_DISERBO
                                Dim dtRif As DataTable = objMovDetRif.Recupera_DT_Rif_Unificato(dtMovimenti.Rows(i).Item("Piva"),
                                                                                                0,
                                                                                                dtMovimenti.Rows(i).Item("Id_Agenda"),
                                                                                                0, 0, 0, "", objParametriServer)

                                If dtRif.Rows.Count <> 0 Then
                                    For Each drRif As DataRow In dtRif.Rows
                                        Select Case drRif.Item("Lav_Cod_Risultato")
                                            Case LAVCOD_BOLLA_EMESSA
                                                dr.Item("Modificabile") = "0"
                                                dr.Item("Eliminabile") = "1"
                                                Exit For
                                        End Select
                                    Next
                                End If

                        End Select

                        If Prezzo_Unitario_DocAllegato <> 0 Then
                            Prezzo_Unitario = Prezzo_Unitario_DocAllegato
                        Else
                            Prezzo_Unitario = dtMovimenti.Rows(i).Item("Prezzo_Unitario")
                        End If

                        If Prezzo_Unitario_Netto_DocAllegato <> 0 Then
                            Prezzo_Unitario_Netto = Prezzo_Unitario_Netto_DocAllegato
                        Else
                            Prezzo_Unitario_Netto = dtMovimenti.Rows(i).Item("Prezzo_Unitario_Netto")
                        End If

                        If Prezzo_Unitario_Netto <> 0 Then
                            dr.Item("Prezzo_Unitario") = Prezzo_Unitario_Netto
                        Else
                            dr.Item("Prezzo_Unitario") = Prezzo_Unitario
                        End If

                        dr.Item("Totale") = CDec(dr.Item("Prezzo_Unitario")) * Agro_Math.ArrotondaVal(CDec(dtMovimenti.Rows(i).Item("Qta_Dest")), 4)

                        'num documento
                        If lavCod >= 1000 AndAlso lavCod <= 1999 Then

                            Dim docNumero As String = ""
                            Dim contatto As String = ""
                            Dim codContatto As String = ""
                            Dim causaleTrasporto As String = ""
                            If Not IsDBNull(dtMovimenti.Rows(i).Item("id_agenda")) Then
                                Dim drMov() As DataRow = dtMov.Select("id_agenda=" & dtMovimenti.Rows(i).Item("id_agenda"))
                                If drMov IsNot Nothing AndAlso drMov.Length > 0 Then
                                    docNumero = drMov(0).Item("doc_numero_Sin") & drMov(0).Item("doc_numero") & drMov(0).Item("doc_numero_des")
                                    codContatto = drMov(0).Item("cod_contatto")
                                    If Not IsDBNull(drMov(0).Item("Causale_Trasporto")) Then
                                        causaleTrasporto = drMov(0).Item("Causale_Trasporto")
                                    End If
                                    If Not IsDBNull(drMov(0).Item("rag_soc")) AndAlso drMov(0).Item("rag_soc") <> "" Then
                                        contatto = drMov(0).Item("rag_soc") '& " (" & drMov(0).Item("cod_contatto") & ")"
                                    ElseIf Not IsDBNull(drMov(0).Item("cognome")) AndAlso drMov(0).Item("cognome") <> "" Then
                                        contatto = drMov(0).Item("cognome") & " " & drMov(0).Item("nome") '& " (" & drMov(0).Item("cod_contatto") & ")"
                                    End If
                                End If
                            End If

                            dr.Item("Doc_Numero") = docNumero
                            dr.Item("Contatto") = contatto
                        Else
                            dr.Item("Doc_Numero") = ""
                            dr.Item("Contatto") = ""
                        End If


                        If isFreshAndFood Then
                            For Each paramQual In dtParamQual.Rows
                                'Parametri qualitativi inseriti dall'utente

                                Select Case paramQual("Tipo")

                                    Case 3

                                        If dtMovimenti.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod") <> "" Then
                                            dr.Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod") = CDec(dtMovimenti.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod").Replace(".", SeparatoreDecimaleVB))
                                        End If

                                    Case 4

                                        dr.Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod") = dtMovimenti.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod")

                                    Case 5

                                        If IsDate(dtMovimenti.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod")) Then
                                            dr.Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod") = CDate(dtMovimenti.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Val_Cod")).ToShortDateString
                                        End If

                                    Case Else

                                        'Parametri qualitativi da DDL
                                        If Not ({"cliente"}).Contains(paramQual("Tabella_Key")) AndAlso
                                           CInt(dtMovimenti.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Tipo_Cod")) <> 0 AndAlso
                                           Not htParamQual.ContainsKey(paramQual("Tabella_Key")) Then
                                            htParamQual.Add(paramQual("Tabella_Key"), True)
                                        End If

                                        dr.Item("FF_" & paramQual("Tabella_Key") & "_Tipo_Cod") = dtMovimenti.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Tipo_Cod")
                                        If Not ({"cliente", "fornitore"}).Contains(paramQual("Tabella_Key")) Then
                                            dr.Item("FF_" & paramQual("Tabella_Key") & "_Tara_Campionatura") = dtMovimenti.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Tara_Campionatura")
                                            dr.Item("FF_" & paramQual("Tabella_Key") & "_Sigla") = dtMovimenti.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Sigla")
                                            dr.Item("FF_" & paramQual("Tabella_Key") & "_Descrizione") = dtMovimenti.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Descrizione")
                                            dr.Item("FF_" & paramQual("Tabella_Key") & "_Codice_Generazione_Link") = dtMovimenti.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Codice_Generazione_Link")
                                            dr.Item("FF_" & paramQual("Tabella_Key") & "_Mat_Cod_Generazione_Link") = dtMovimenti.Rows(i).Item("FF_" & paramQual("Tabella_Key") & "_Mat_Cod_Generazione_Link")

                                            'Valorizzazione colonna Referenza
                                            If Not ({"imballaggio", "contenitore", "confezione"}).Contains(paramQual("Tabella_Key")) Then
                                                'If Not String.IsNullOrEmpty(dr.Item("FF_" & paramQual("Tabella_Key") & "_Sigla")) Then
                                                '    dr.Item("Referenza") &= "<br/>" & dr.Item("FF_" & paramQual("Tabella_Key") & "_Sigla")
                                                'Else
                                                If Not String.IsNullOrEmpty(dr.Item("FF_" & paramQual("Tabella_Key") & "_Descrizione")) Then
                                                    dr.Item("Referenza") &= "<br/>" & dr.Item("FF_" & paramQual("Tabella_Key") & "_Descrizione")
                                                End If
                                                'End If
                                                'Imballaggio, Contenitore e Descrizione vengono mostrati in colonne a parte
                                                'Else
                                                '    If Not String.IsNullOrEmpty(dr.Item("FF_" & paramQual("Tabella_Key") & "_Descrizione")) Then
                                                '        dr.Item("Referenza") &= " " & dr.Item("FF_" & paramQual("Tabella_Key") & "_Descrizione")
                                                '    End If
                                            End If
                                        End If

                                        If ({"fornitore"}).Contains(paramQual("Tabella_Key")) Then
                                            Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R
                                            Dim codRisUmFornitore As String = dtMovimenti.Rows(i).Item("FF_fornitore_Tipo_Cod").ToString
                                            If Not String.IsNullOrEmpty(codRisUmFornitore) Then
                                                Dim objContatto As DataTable = objContattiR.RagSoc_Nome_Cognome_RapportoDes_from_Cod_Risum(codRisUmFornitore, objParametriServer)
                                                If objContatto.Rows.Count > 0 Then
                                                    dr.Item("Fornitore") = objContatto.Rows(0)("rag_soc")
                                                    'Fornitore viene mostrato in colonne a parte
                                                    'dr.Item("Referenza") &= " " & dr.Item("Fornitore")
                                                End If
                                            End If
                                        End If

                                End Select

                            Next

                            'TODO: Note --> Non c'è?!?
                            'dr.Item("FF_ONote_Descrizione") = dtGiacenze.Rows(i).Item("FF_ONote_Descrizione")
                        End If

                        If gestioneGruppiMerce Then
                            dr.Item("Id_Gruppo_Merce") = dtMovimenti.Rows(i).Item("Id_Gruppo_Merce")
                            dr.Item("Des_Gruppo_Merce") = dtMovimenti.Rows(i).Item("Des_Gruppo_Merce")
                        End If

                        Dim data_chiave = If(Kendo, Replace(dr.Item("data"), "/", "-"), dr.Item("data"))
                        dr.Item("chiave_movimenti") = data_chiave & "_" &
                                                      dr.Item("piva") & "_" &
                                                      dr.Item("id_agenda") & "_" &
                                                      dr.Item("Blocco_Flag") & "_" &
                                                      dr.Item("lav_cod") & "_" &
                                                      "0_" &
                                                      dr.Item("sa_cod") & "_" &
                                                      dtMovimenti.Rows(i).Item("id_mov") & "_" &
                                                      dtMovimenti.Rows(i).Item("id_mov_det")


                        dt.Rows.Add(dr)

                    End If

                Next

            End If

            'If Kendo Then
                Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                str_Risposta = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            'Else
            '    str_Risposta = DT_to_Json_Movimenti(dt)
            'End If

            r.RispostaOK = True
            r.RispostaStringa = str_Risposta

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

#End Region

    Private Sub CambiaImpresa(ByVal sender As Object, ByVal e As System.EventArgs)

        'azzero il filtro scelto x le operazioni multi-aziendali
        Session("VariabiliFiltro") = Nothing

        'Costruisco il link
        Dim origine As String = Stringa_Codifica("../GestioneMagazzini/GestioneMagazziniBS.aspx", AgroKey_EncoderDecoder, Server)

        Dim destinazione As String = Stringa_Codifica("../GestioneMagazzini/GestioneMagazziniBS.aspx", AgroKey_EncoderDecoder, Server)

        Dim targetUrl As String = "../Filtrino/FiltrinoImprese.aspx" &
                                    "?o=" & origine &
                                    "&d=" & destinazione

        Response.Redirect(targetUrl)

    End Sub

    Public Class Defaults
        Public Property Piva As String
        Public Property Sa_Cod As Integer
        Public Property Chiave_Magazzino As String
        Public Property Mag As Integer
        Public Property Elem_Cod As Integer
        Public Property Pro_Cod As Integer
        Public Property Prodotto As String
        Public Property DataGiacenza As Date?
        Public Property DataOpDa As Date?
        Public Property DataOpA As Date?
    End Class

End Class
