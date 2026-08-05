Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider

Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp

Namespace Agro_Pages_NS

    Public MustInherit Class Agro_Page_Operazione_Colturale
        Inherits Agro_Page_Generica

        Protected Master_Operazione As Operazione 'Agro_Pages_NS.Master_Pages_NS.Operazione_MP
        'I_Operazione_Colturale_Handler è usato per creare una nuova operazione e salvarla in sessione, deve rimanere privato .. fprse
        ' Protected Handler_Operazione As Agro_Modello.Operazioni_Colturali_NS.Handler_Operazione
        'operazione colturale, utilizzata dalle pagine che specializzano l'Agro_Page_Operazione_Colturale
        Protected Operazione_Colturale_Generica As AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.I_Operazione_Colturale

        Protected UtenteAbilitato_Lettura As Boolean = False
        Protected UtenteAbilitato_Modifica As Boolean = False

#Region "PreInit"


        Private Sub Page_PreInit1(sender As Object, e As System.EventArgs) Handles Me.PreInit

            Master_Operazione = CType(Me.Master, Operazione) ' Agro_Pages_NS.Master_Pages_NS.Operazione_MP)


            If Not IsPostBack Then

                If Genera_Operazione_Colturale() Then
                    'i permessi utilizzano l'operazione, non spostare prima
                    VerificaPermessi()
                Else
                    Throw New NotImplementedException
                End If



            Else
                'se non sono in postback leggo l'operazione da sessione
                Operazione_Colturale_Generica = AgronicaCoreModello.Agenda.Operazioni_Colturali.Handler_Operazione.Leggi_Operazione_Da_Sessione()

                UtenteAbilitato_Lettura = Session("UtenteAbilitato_Lettura")
                UtenteAbilitato_Modifica = Session("UtenteAbilitato_Modifica")
            End If
            'da questo momento in avanti l'operazione è disponibile tramite la variabile I_Operazione_Colturale 
            'per le pagine che specializzano Agro_Page_Operazione_Colturale


            If Not IsNothing(Operazione_Colturale_Generica) Then
                'la nuova operazione Operazione_Colturale_Genericata è stata creata correttamente
            Else
                Throw New NotImplementedException
            End If

        End Sub

        Private Function Genera_Operazione_Colturale() As Boolean
            Dim OK As Boolean = True

            '--------------------------DA MODIFICARE E GESTIRE NELL APAGINA MENU----------------------
            'Se è la prima volta che carico la pagina creo la nuova operazione, altrimenti la leggo dalla sessione
            ''''devo togliere parametri agenda onasconderlo meglio
            Dim objParametriAgenda As New ParametriAgenda
            'objParametriAgenda.Leggi()
            'da cambiare, demando creazione alla classe specializzante, 
            'potrei togliere l'handler e fare gestire tutto dalla pagina specifica, ma quando dovrò
            'smettere di utilizzare objParametriAgenda lì'operazione colturale generica mio servirà per portarmi dietro 
            'i parametri dalla pagina dei menu, per cui l'operazione di creazione dell'handler dovrà essere effettuata
            'dalla pagina menu
            Operazione_Colturale_Generica = AgronicaCoreModello.Agenda.Operazioni_Colturali.Handler_Operazione.Crea_Nuova_OperazioneColturale(objParametriAgenda.Piva, objParametriAgenda.Data, objParametriAgenda.Tipo_Operazione, objParametriAgenda.Id_Agenda, objParametri_Server)
            'I_Operazione_Colturale.Sa_Cod = objParametriAgenda.Sa_Cod NOOO, gli arriva 0 dal menu, sacod lo leggo dopo dall'oggettto agenda
            '-------------------------------------------FINE--------------------------------------------------

            'creo l'operazione specifica, il metodo viene implementato dalla pagina cpecifica, dato che 
            'occorre avere impostati alcuni parametri come il codice lavorazione e questi parametri non devono essere a conoscenza della pagina
            'generica. Se non venissero impostati non si potrebbe chiamare successivamente Leggi_Operazione_Da_Agenda()
            'che ha bisogno naturalmente di conoscere il tipo di operazione per riuscire a leggere
            Operazione_Colturale_Generica = Creo_Operazione_Specifica_da_Generica()
            If IsNothing(Operazione_Colturale_Generica) Then
                Return False
            End If


            Select Case Operazione_Colturale_Generica.Tipo_Operazione

                Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica, TipiEnumerativi.enum_TipoOperazioneDB.Lettura
                    'In modifica e lettura leggo l'operazione agenda da db e la wrappo nell'oggetto I_Operazione_Colturale
                    'specifico dell'operazione, definito dalla pagina che specializza l'operazione.
                    'in tutte le pagine operazioni infatti in lettura e modifica occorre leggere l'operazione da db e avere l'oggetto corrispondente I_Operazione_C. in memoria
                    OK = AgronicaCoreIO.Agenda.Handler_IO_Operazione.Leggi_Operazione_Da_Agenda(Operazione_Colturale_Generica, objParametri_Server)
                    If Not OK Then
                        Return False
                    End If
            End Select

            Return True

        End Function

        Private Sub VerificaPermessi()

            Dim UtenteAbilitato_Lettura_Temp As Boolean = False

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            UtenteAbilitato_Lettura_Temp = objPermessi.Controlla_Permessi_Utente( _
                                        Session("ASG_Utente_Username"), _
                                        Session("ASG_IdServizio"), _
                                        enum_Security_Attivita.Agenda_AccessoMenu, _
                                        enum_Security_Operazione.Lettura, _
                                        Date.Now, _
                                        "", _
                                        objParametri_Utenti)

            Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura_Temp
            UtenteAbilitato_Lettura = UtenteAbilitato_Lettura_Temp

            If UtenteAbilitato_Lettura = False Then
                Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
                Exit Sub
            End If

            Dim UtenteAbilitato_Modifica_Temp As Boolean = False
            UtenteAbilitato_Modifica_Temp = objPermessi.Controlla_Permessi_Utente( _
                                       Session("ASG_Utente_Username"), _
                                       Session("ASG_IdServizio"), _
                                       enum_Security_Attivita.Agenda_AccessoMenu, _
                                       enum_Security_Operazione.Modifica, _
                                       Date.Now, _
                                       "", _
                                       objParametri_Utenti)

            Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica_Temp
            UtenteAbilitato_Modifica = UtenteAbilitato_Modifica_Temp

            If (Not (Operazione_Colturale_Generica.Tipo_Operazione = enum_TipoOperazioneDB.Lettura)) AndAlso UtenteAbilitato_Modifica = False Then
                Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
                Exit Sub
            End If


        End Sub

#End Region

#Region "Init"

        Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
            AddHandler Master_Operazione.Property_BTN_ChangeData.Click, AddressOf Me.AggiornaOperazioniPerData
            AddHandler Master_Operazione.Property_ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
            AddHandler Master_Operazione.Property_BTN_CentroAziendale.Click, AddressOf Me.CambioCentro
            AddHandler Master_Operazione.Property_BTN_Magazzini.Click, AddressOf Me.CambioMagazzino
            AddHandler Master_Operazione.Property_BTN_ComboSpecie.Click, AddressOf Me.CambioSpecie
            AddHandler Master_Operazione.Property_ImgBtn_Salva.Click, AddressOf Me.SalvaTutto
            AddHandler CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Carico"), ImageButton).Click, AddressOf Me.BTN_CaricoMagazzino
            AddHandler CType(Page.Master, MasterPage).PreRender, AddressOf Me.MasterUnload
        End Sub

#End Region

#Region "Load"

        Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
            If Not IsPostBack Then
                LeggiImpostazioni_Generale()
                Carica_ControlliMaster_Generale()
                AbilitaDisabilita_ControlliMaster_Generale()
            End If
        End Sub


        'leggo le eventuali IMPOSTAZIONI UTENTE
        Private Sub LeggiImpostazioni_Generale()
            LeggiImpostazioni()
        End Sub


        Private Sub Carica_ControlliMaster_Generale()

            Master_Operazione.Property_Lbl_Titolo.Text = Operazione_Colturale_Generica.Lav_Des

            Select Case Operazione_Colturale_Generica.Tipo_Operazione

                Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                    'nella scrittura non faccio operazioni generiche per il caricamento dei controlli 

                Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica, TipiEnumerativi.enum_TipoOperazioneDB.Lettura
                    'In modifica e lettura leggo l'operazione agenda da db e la wrappo nell'oggetto I_Operazione_Colturale
                    'specifico dell'operazione, definito dalla pagina che specializza l'operazione.
                    'in tutte le pagine operazioni infatti in lettura e modifica occorre leggere l'operazione da db e avere l'oggetto corrispondente I_Operazione_C. in memoria


                    'carico le note e la nota
                    '-----------------------------------------------------
                    'se ho note selezionate le checkko!!!
                    '-----------------------------------------------------
                    For i = 0 To Operazione_Colturale_Generica.Note_Codificate.Count - 1

                        For j = 0 To Master_Operazione.Property_CBL_Consigli.Items.Count - 1
                            If Operazione_Colturale_Generica.Note_Codificate(i) = _
                                         Master_Operazione.Property_CBL_Consigli.Items(j).Value Then
                                Master_Operazione.Property_CBL_Consigli.Items(j).Selected = True
                            End If
                        Next

                    Next
                    Master_Operazione.Property_txt_Note.Text = Operazione_Colturale_Generica.Nota
                    'se ho il magazzino lo seleziono
                    Dim fabbricato As String = "0"
                    If Not IsNothing(Operazione_Colturale_Generica.Magazzino) Then
                        fabbricato = Operazione_Colturale_Generica.Magazzino.Fabbricato_Cod & "|" & Operazione_Colturale_Generica.Magazzino.Sa_Cod & "|" & Operazione_Colturale_Generica.Piva
                    End If
                    ImpostaFabbricatoDelMagazzinoEsternoSePresente(fabbricato)
                    Master_Operazione.Property_ComboMagazzini.ddl_Magazzini.SelectedIndex = Master_Operazione.Property_ComboMagazzini.ddl_Magazzini.Items.IndexOf(Master_Operazione.Property_ComboMagazzini.ddl_Magazzini.Items.FindByValue(fabbricato))

                    CaricaControlli_Da_OperazioneColturale()


                    'sarà da cambiare la gestione dei costi access, fare un modello e caricarli da questa pagiuna usandonoperazione_colturale generica
                    'creando un aproprietà migliore
                    Master_Operazione.CaricaCostiAccessori()

                Case Else
                    Throw New NotImplementedException

            End Select

            'Richiamo il metodo mustOverrides CaricaControlli, che è presente in tutte le pagine specifiche e che verrà utilizzato se necessario
            'per caricare i controlli specifici dell'operazione
            caricaControlli()

        End Sub

        Public Sub ImpostaFabbricatoDelMagazzinoEsternoSePresente(ByRef fabbricato As String)
            '----------------------------------------------------------------------------------------------------
            'gestione combo magazzino esterno
            'se l'operazione aveva utilizzato il magazzino del'azienda padre devo preselezionare quello e non quello del'operazione
            Dim objRif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
            Dim seminaOldConAltraImpresa As Boolean
            Dim Id_Agenda_Old As Integer = Operazione_Colturale_Generica.ID_Agenda
            seminaOldConAltraImpresa = objRif.Verifica_Operazione_ConMagazzinoAltraimpresa(Operazione_Colturale_Generica.Piva, _
                                                                                        Operazione_Colturale_Generica.Sa_Cod, _
                                                                                        Id_Agenda_Old, _
                                                                                        Operazione_Colturale_Generica.Lav_Cod, _
                                                                                        "", _
                                                                                        objParametri_Server)
            If seminaOldConAltraImpresa Then

                'recupero la piva, sacod e id fabbricato per il confronto
                Dim Magazzino_Piva_Operazione_Old As String = ""
                Dim Magazzino_Sa_Cod_Operazione_Old As Integer = 0
                Dim Magazzino_Fabbricato_Cod_Operazione_Old As Integer = 0

                objRif.Recupera_ChiaveMagazzinoAltraImpresa_Operazione(Operazione_Colturale_Generica.Piva, _
                                                                    Operazione_Colturale_Generica.Sa_Cod, _
                                                                    Id_Agenda_Old, _
                                                                    Operazione_Colturale_Generica.Lav_Cod, _
                                                                    "", _
                                                                    Magazzino_Piva_Operazione_Old, _
                                                                    0, _
                                                                    Magazzino_Sa_Cod_Operazione_Old, _
                                                                    Magazzino_Fabbricato_Cod_Operazione_Old, _
                                                                    objParametri_Server)

                If Magazzino_Piva_Operazione_Old = "" Or Magazzino_Sa_Cod_Operazione_Old = 0 Or Magazzino_Fabbricato_Cod_Operazione_Old = 0 Then
                    Throw New Exception("Non ho recupoerato il magazzino dell'azienda padre")
                End If

                fabbricato = Magazzino_Fabbricato_Cod_Operazione_Old & "|" & Magazzino_Sa_Cod_Operazione_Old & "|" & Magazzino_Piva_Operazione_Old

            End If
            '----------------------------------------------------------------------------------------------------
        End Sub

        Private Sub AbilitaDisabilita_ControlliMaster_Generale()

            'controlo il permesso sulla specie, se non ce l'ho metto operazione in lettura
            If Operazione_Colturale_Generica.Tipo_Operazione = CStr(TipiEnumerativi.enum_TipoOperazioneDB.Modifica) Then
                Try
                    'Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                    'Dim Dt As DataTable = objSpecVeg.SpecieVegetali_GestioneFiltroUtente_Leggi(getSpecie(), _
                    '                                                 0, _
                    '                                                 "", _
                    '                                                 "", _
                    '                                                 "", _
                    '                                                 "", _
                    '                                                 objParametri_Utenti)
                    'If Dt.Rows.Count = 0 Then
                    '    Operazione_Colturale_Generica.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
                    'End If
                Catch ex As Exception
                    Operazione_Colturale_Generica.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
                End Try
            End If


            'impostazioni in base operazione di creazione/modifica..
            Select Case Operazione_Colturale_Generica.Tipo_Operazione

                Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                    AbilitaDisabilita_ControlliMaster_PerScrittura_Generale()

                Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica
                    AbilitaDisabilita_ControlliMaster_PerModifica_Generale()

                Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura
                    AbilitaDisabilita_ControlliMaster_PerLettura_Generale()

            End Select

            'Richiamo il metodo mustOverrides , che è presente in tutte le pagine specifiche e che verrà utilizzato se necessario
            'per disabilitare i controlli specifici dell'operazione
            AbilitaDisabilita_Controlli()

        End Sub


        Private Sub AbilitaDisabilita_ControlliMaster_PerScrittura_Generale()
            BloccaSblocca_Tutti_ControlliMaster(True)
            AbilitaDisabilita_Controlli_PerScrittura()
        End Sub


        Private Sub AbilitaDisabilita_ControlliMaster_PerModifica_Generale()

            BloccaSblocca_Tutti_ControlliMaster(False)
            'true per modifica:
            Master_Operazione.Property_ImgBtn_Salva.Enabled = True
            Master_Operazione.Property_RBL_Salva.Visible = True
            Master_Operazione.Property_Box_Salva.Enabled = True
            Master_Operazione.Property_Box_Salva.Visible = True
            Master_Operazione.Property_CBL_Consigli.Enabled = True
            Master_Operazione.Property_txt_Note.Enabled = True

            'Per il salva e vai ai costi, abilito il controllo
            Master_Operazione.Property_RBL_Salva.Enabled = True
            Master_Operazione.Property_RBL_Salva.Items(1).Enabled = False
            Master_Operazione.Property_RBL_Salva.Items(2).Enabled = False

            AbilitaDisabilita_Controlli_PerModifica()

        End Sub


        Private Sub AbilitaDisabilita_ControlliMaster_PerLettura_Generale()
            BloccaSblocca_Tutti_ControlliMaster(False)
            AbilitaDisabilita_Controlli_PerLettura()
        End Sub

        'funzione che blocca tutti tranne quelli per salvataggio e note e consigli
        'e soprattutto non blocca la scelta del tipo salvataggio
        'utilie perchè di solito le operazioni possono avere un poulsante di 
        'iun serimento o creazioe tabella, che blocca i controlli della master ma lascia la scelta del tipo di salvataggio
        Protected Sub AbilitaDisabilita_ControlliMaster_PerInserimento_InScrittura_Standard()
            BloccaSblocca_Tutti_ControlliMaster(False)
            'true per modifica:
            Master_Operazione.Property_ImgBtn_Salva.Enabled = True
            Master_Operazione.Property_RBL_Salva.Enabled = True
            Master_Operazione.Property_RBL_Salva.Visible = True
            Master_Operazione.Property_Box_Salva.Enabled = True
            Master_Operazione.Property_Box_Salva.Visible = True
            Master_Operazione.Property_CBL_Consigli.Enabled = True
            Master_Operazione.Property_txt_Note.Enabled = True

        End Sub

        'funzione che sblocca tutti 
        'utilie perchè di solito le operazioni possono avere un poulsante di 
        'iun serimento o creazioe tabella
        Protected Sub AbilitaDisabilita_ControlliMaster_PerSblocco_InScrittura_Standard()
            BloccaSblocca_Tutti_ControlliMaster(True)
        End Sub


        Protected Sub BloccaSblocca_Tutti_ControlliMaster(ByVal Abilita As Boolean)
            'blocco via javascript pulsanti combo
            'Sblocco via javascript pulsanti combo
            'Si sbloccano da soli con il ricaricamento
            AbilitaDisabilita_ComboMaster(Abilita, Abilita, Abilita, Abilita)

            Master_Operazione.Property_ImgBtn_Salva.Enabled = Abilita 'true per modifica

            Master_Operazione.Property_RBL_Salva.Enabled = Abilita
            Master_Operazione.Property_RBL_Salva.Visible = Abilita

            Master_Operazione.Property_Box_Salva.Enabled = Abilita
            Master_Operazione.Property_Box_Salva.Visible = Abilita

            Master_Operazione.Property_CBL_Consigli.Enabled = Abilita
            Master_Operazione.Property_GridView_Impianti.Enabled = Abilita

            Master_Operazione.Property_txt_DataOperazione.Enabled = Abilita
            Master_Operazione.Property_txt_Note.Enabled = Abilita
        End Sub

        Protected Sub AbilitaDisabilita_ComboMaster(ByVal abilita_centri As Boolean, ByVal abilita_operazioni As Boolean, ByVal abilita_specie As Boolean, ByVal abilita_magazzini As Boolean)
            'blocco via javascript pulsanti combo
            Dim script As New StringBuilder
            If abilita_centri Then
                Master_Operazione.Property_BTN_CentroAziendale.Enabled = True
                Master_Operazione.Property_ComboCentroAziendale.Enabled = True
            Else
                Master_Operazione.Property_BTN_CentroAziendale.Enabled = False
                Master_Operazione.Property_ComboCentroAziendale.Enabled = False
                script.AppendLine("$(document).ready(function () { ")
                script.AppendLine("     BloccaCombo_CentroAziendale();")
                script.AppendLine("}); ")
            End If
            If abilita_operazioni Then
                'Master_Operazione.Property_BTN_Operazione.Enabled = True
                Master_Operazione.Property_ComboOperazione.Enabled = True
            Else
                'Master_Operazione.Property_BTN_Operazione.Enabled = True
                Master_Operazione.Property_ComboOperazione.Enabled = False
                script.AppendLine("$(document).ready(function () { ")
                script.AppendLine("     BloccaCombo_Operazioni();")
                script.AppendLine("}); ")
            End If
            If abilita_specie Then
                Master_Operazione.Property_BTN_ComboSpecie.Enabled = True
                Master_Operazione.Property_ComboSpecie.Enabled = True
            Else
                Master_Operazione.Property_BTN_ComboSpecie.Enabled = False
                Master_Operazione.Property_ComboSpecie.Enabled = False
                script.AppendLine("$(document).ready(function () { ")
                script.AppendLine("     BloccaCombo_Specie();")
                script.AppendLine("}); ")
            End If
            If abilita_magazzini Then
                Master_Operazione.Property_BTN_Magazzini.Enabled = True
                Master_Operazione.Property_ComboMagazzini.Enabled = True
            Else
                Master_Operazione.Property_BTN_Magazzini.Enabled = False
                Master_Operazione.Property_ComboMagazzini.Enabled = False
                script.AppendLine("$(document).ready(function () { ")
                script.AppendLine("     BloccaCombo_Magazzini();")
                script.AppendLine("}); ")
            End If


            If script.ToString <> "" Then
                ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelCentroAziendale"), UpdatePanel), _
                                        CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelCentroAziendale"), UpdatePanel).GetType(), _
                                        "jQuery_{0}", script.ToString, True)
            End If

        End Sub


#End Region

#Region "UnLoad"

        Private Sub Page_Unload(sender As Object, e As System.EventArgs) Handles Me.Unload

            'salvo in sessione l'operazione colturale per il prossimo caricamento
            AgronicaCoreModello.Agenda.Operazioni_Colturali.Handler_Operazione.Salva_Operazione_In_Sessione(Operazione_Colturale_Generica)

        End Sub

#End Region


        Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)
            RipristinaSessione()

            Dim objParametriAgenda As New ParametriAgenda
            Dim link As String = ""
            Try
                Dim sitoorigine As Enum_SiteRedirector = HttpContext.Current.Session("Sito_Origine")
                Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

                If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 And paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                    link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri( _
                                           Enum_SiteRedirector.Sito_AgronicaAgenda_2010, _
                                           enum_PagineGiasOnline_2010.RegistazioneSmart, _
                                           enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

                Else
                    link = CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                End If

            Catch ex As Exception
                link = CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
            End Try

            Response.Redirect(link)


        End Sub

        Private Sub RipristinaSessione()
            Session.Remove("UtenteAbilitato_Lettura")
            Session.Remove("UtenteAbilitato_Modifica")

            Dim res As Boolean = AgronicaCoreModello.Agenda.Operazioni_Colturali.Handler_Operazione.Elimina_Operazione_Da_Sessione()

            'DA ELIMINARE QUESTO PARAMETRO IN FUTURO E GESTIRE TUTTO DA OPERAZIONE
            Dim objParametriAgenda As New ParametriAgenda

            objParametriAgenda.Svuota_DatiOperazione()
            objParametriAgenda.OperazioneMulticentro = True
            '--------

        End Sub

        Private Sub SalvaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

            '-----------------------------------------------------------------------------------------------------
            '-----------------------TEMPORANERO PER EVITARE SALVATAGGIO MAGAZZINI ESTERNI------------------------
            Dim objParametriAgenda As New ParametriAgenda
            If objParametriAgenda.Fabbricato <> "0" AndAlso Split(objParametriAgenda.Fabbricato, "|").Count = 3 AndAlso Split(objParametriAgenda.Fabbricato, "|")(2) <> objParametriAgenda.Piva Then
                Dim MErrore As String
                MErrore = "ATTENZIONE!<br> al momento non è permesso il salvataggio con il magazzino di un'azienda diversa <b>"
                Messaggi.AgroMsgBox("Attenzione, l'operazione non è stata registrata! <br> " & MErrore, Page, , _
                    CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                Exit Sub
            End If
            '-----------------------------------------------------------------------------------------------------
            '-----------------------------------------------------------------------------------------------------

            Dim messaggiErrore As New AgronicaCoreUtility.Messages_Str

            If Prepara_Operazione_Per_Salvataggio_Generica(messaggiErrore) Then

                If salva_Operazione(messaggiErrore) Then

                    Gestisci_Fine_Salvataggio_Generica()
                    Exit Sub

                End If

            End If

            Messaggi.AgroMsgBox("Attenzione, l'operazione non è stata registrata! </br> " & messaggiErrore.getAllHtml, Page, , _
                  Master_Operazione.Property_UpdatePanelToolBar)
        End Sub

        Private Function Prepara_Operazione_Per_Salvataggio_Generica(ByRef messaggiErrore As AgronicaCoreUtility.Messages_Str) As Boolean

            'inserisco DATA operazione
            Operazione_Colturale_Generica.DataOperazione = CDate(Master_Operazione.Property_txt_DataOperazione.Text)

            'inserisco il magazzino
            If Master_Operazione.Property_ComboMagazzini.ddl_Magazzini.SelectedValue.Contains("|") Then
                Dim Fabbricato As Integer = CInt(Split(Master_Operazione.Property_ComboMagazzini.ddl_Magazzini.SelectedValue, "|")(0))
                Dim SaCod_Fabbricato As Integer = CInt(Split(Master_Operazione.Property_ComboMagazzini.ddl_Magazzini.SelectedValue, "|")(1))
                Dim Piva_Fabbricato As String = Split(Master_Operazione.Property_ComboMagazzini.ddl_Magazzini.SelectedValue, "|")(2)
                Operazione_Colturale_Generica.Magazzino = New AgronicaCoreModello.Anagrafe.Magazzino(Piva_Fabbricato, SaCod_Fabbricato, Fabbricato)
            Else
                Operazione_Colturale_Generica.Magazzino = Nothing
            End If

            'inserisco la nota
            Operazione_Colturale_Generica.Nota = Master_Operazione.Property_txt_Note.Text

            'Inserisco le note (i consigli)
            Dim ListaConsigli As List(Of Nota)
            ListaConsigli = CType(Master, Operazione).GetConsigli()
            Operazione_Colturale_Generica.Note_Codificate.Clear()
            If ListaConsigli.Count > 0 Then
                For i = 0 To ListaConsigli.Count - 1
                    Operazione_Colturale_Generica.Note_Codificate.Add(ListaConsigli(i).Nota_Cod)
                Next
            End If

            '-----------------------------------------
            'da modificare e creare oggetti appositi per costi accessori
            Dim objParametriAgenda As New ParametriAgenda
            'objParametriAgenda.Leggi()
            Operazione_Colturale_Generica.Costi_Accessori_Temporaneo = objParametriAgenda.Movimenti
            '--------------------------------------------


            'inserisco il resto specifico per l'operazione
            Return Prepara_Operazione_Per_Salvataggio(messaggiErrore)

        End Function


        'Questo metodo vale per tutte le operazioni, a patto ch esi sia inserito il codice specifico nella IO_Operazioni,
        'nel caso sia necessario modificare il salvataggio oggorre agire su IO_Operazioni (vedere Handler_Operazione.Salva_Operazione_Su_Agenda)
        ' Se occorre preparare loperazione per il salvataggio usare il metodo MustOverride Prepara_Operazione_Per_Salvataggio,
        'Implementato nella classe che specializza e che quindi ha libero accesso ai parametri della pagina.
        'Se occorre avere dei parametri in piu aggiungerli all'oggetto Operazione_Colturale e gestire il tutto nella
        'IO_Operazione, non sovrascrivere questo metodo
        Private Function salva_Operazione(ByRef messaggiErrore As AgronicaCoreUtility.Messages_Str) As Boolean
            Dim objParametriAgenda As New ParametriAgenda
            If AgronicaCoreIO.Agenda.Handler_IO_Operazione.Salva_Operazione_Su_Agenda(Operazione_Colturale_Generica, objParametri_Server, messaggiErrore, objParametriAgenda.Id_Agenda) Then
                Operazione_Colturale_Generica.ID_Agenda = objParametriAgenda.Id_Agenda
                Return True
            End If
            Return False
        End Function

        Private Sub Gestisci_Fine_Salvataggio_Generica()
            Dim TipoSalvataggio As String = Master_Operazione.Property_RBL_Salva.SelectedValue + 1
            Select Case TipoSalvataggio
                Case enum_Tipo_Salvataggio.Salva_e_Esci
                    'elimino i permessi e l'operazione
                    RipristinaSessione()
                    'Response.Redirect("~/menu/menu.aspx")
                    Dim objParametriAgenda As New ParametriAgenda

                    Dim link As String = ""
                    Try
                        Dim sitoorigine As Enum_SiteRedirector = HttpContext.Current.Session("Sito_Origine")
                        Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

                        If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 And paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                            link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri( _
                                                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010, _
                                                   enum_PagineGiasOnline_2010.RegistazioneSmart, _
                                                   enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

                        ElseIf sitoorigine = Enum_SiteRedirector.GiasNG Then
                            AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                        Enum_SiteRedirector.GiasNG,
                                                                        objParametriAgenda.PaginaSitoOrigine,
                                                                        link,
                                                                        objParametri_Server,
                                                                        SitoOrigine:=Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua)
                        Else
                            link = CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                        End If

                    Catch ex As Exception
                        If HttpContext.Current.Session("Sito_Origine") IsNot Nothing AndAlso
                           CType(HttpContext.Current.Session("Sito_Origine"), Enum_SiteRedirector) = Enum_SiteRedirector.GiasNG Then
                            AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                        Enum_SiteRedirector.GiasNG,
                                                                        objParametriAgenda.PaginaSitoOrigine,
                                                                        link,
                                                                        objParametri_Server,
                                                                        SitoOrigine:=Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua)
                        Else
                            link = CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                        End If
                    End Try


                    Dim strJS As New StringBuilder
                    strJS.AppendLine("$(document).ready(function () { ")
                    strJS.AppendLine("      ChiamataParent_Id_Ageda(" & Operazione_Colturale_Generica.ID_Agenda & "); ")
                    strJS.AppendLine("      window.location = '" & link & "'; ")
                    strJS.AppendLine(" });")
                    ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                                  String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)





                    'objParametriAgenda.Leggi()
                    objParametriAgenda.Svuota_DatiOperazione()
                    'Response.Redirect(CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda))

                Case enum_Tipo_Salvataggio.Salva_e_Nuovo
                    'Ripristino l'operazione
                    'Trappole_Disposed()

                    '----------------------------------
                    '--------------------------
                    'QUESTA PARTE FA SCHIFO; DA SISTEMARE QUANDO SI TOGLIE objParametriAgenda
                    'DA ELIMINARE QUESTO PARAMETRO IN FUTURO E GESTIRE TUTTO DA OPERAZIONE
                    'DA ELIMINARE QUESTO PARAMETRO IN FUTURO E GESTIRE TUTTO DA OPERAZIONE
                    'Operazione.REINIT()
                    Dim objParametriAgenda As New ParametriAgenda
                    objParametriAgenda.Id_Agenda = "0"

                    'rigenero operazione per ulirla, come rientrassi nella pagina
                    If Not Genera_Operazione_Colturale() Then
                        'se errori per sicurezza torno al menu
                        RipristinaSessione()
                        'Response.Redirect("~/menu/menu.aspx")
                        'objParametriAgenda.Leggi()
                        objParametriAgenda.Svuota_DatiOperazione()
                        Response.Redirect(CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda))

                    Else

                        'Operazione.REINIT()
                        'objParametriAgenda.Leggi()
                        objParametriAgenda.Impianti = New List(Of Impianto)
                        objParametriAgenda.Note = New List(Of Nota)
                        objParametriAgenda.Movimenti = New List(Of Movimento)
                        Operazione_Colturale_Generica.Costi_Accessori_Temporaneo.Clear()
                        Operazione_Colturale_Generica.Note_Codificate.Clear()

                        Dim strJS As New StringBuilder
                        strJS.AppendLine("$(document).ready(function () { ")
                        'strJS.AppendLine("      BloccaSbloccaTotale(); ")
                        strJS.AppendLine("      window.location = '" & (New AgronicaCoreModello.Utility_Operazioni).LinkPagina_from_LavCod(Operazione_Colturale_Generica) & "'; ")
                        strJS.AppendLine(" });")
                        ScriptManager.RegisterStartupScript(Master_Operazione.Property_UpdatePanelToolBar1, Master_Operazione.Property_UpdatePanelToolBar1.GetType(),
                                                      String.Format("jQuery_{0}", Master_Operazione.Property_UpdatePanelToolBar1.ClientID), strJS.ToString, True)

                    End If
                    '----------------------------------
                    '--------------------------
                    '----------------------------------
                    '----------------------------------
                    '--------------------------
                    '----------------------------------
                    '--------------------------
                    '--------------------------


                    Messaggi.AgroMsgBuonFine("Registrazione Effettuata con Successo!", Page, , _
                                      Master_Operazione.Property_UpdatePanelToolBar)

                Case enum_Tipo_Salvataggio.Salva_e_Duplica

                    Dim objParametriAgenda As New ParametriAgenda
                    objParametriAgenda.Id_Agenda = "0"

                    'Dim strJS As New StringBuilder
                    'strJS.AppendLine("$(document).ready(function () { ")
                    'strJS.AppendLine("      BloccaSbloccaTotale(); ")
                    'strJS.AppendLine(" });")
                    'ScriptManager.RegisterStartupScript(Master_Operazione.Property_UpdatePanelToolBar1, Master_Operazione.Property_UpdatePanelToolBar1.GetType(),
                    '                              String.Format("jQuery_{0}", Master_Operazione.Property_UpdatePanelToolBar1.ClientID), strJS.ToString, True)

                    Messaggi.AgroMsgBuonFine("Registrazione Effettuata con Successo!", Page, , _
                                      Master_Operazione.Property_UpdatePanelToolBar)

                Case enum_Tipo_Salvataggio.Salva_e_Vai_ai_Costi

                    'Response.Redirect("~/menu/menu.aspx")
                    Dim objParametriAgenda As New ParametriAgenda

                    Dim PaginaLink As String = "../AnalisiCostiProduzione/GestioneCosti.aspx"

                    Dim link As String = ""
                    Try
                        Dim sitoorigine As Enum_SiteRedirector = HttpContext.Current.Session("Sito_Origine")
                        Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

                        If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 And paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                            link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                                   enum_PagineGiasOnline_2010.RegistazioneSmart,
                                                   enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

                        Else
                            link = CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                        End If

                    Catch ex As Exception
                        link = CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                    End Try

                    PaginaLink &= "?p=" & Sicurezza.Stringa_Codifica(objParametriAgenda.Piva, CostantiPersonalizzate.AgroKey_EncoderDecoder) &
                              "&id_agenda=" & Sicurezza.Stringa_Codifica(objParametriAgenda.Id_Agenda, CostantiPersonalizzate.AgroKey_EncoderDecoder) &
                              "&origine=" & Sicurezza.Stringa_Codifica(link, CostantiPersonalizzate.AgroKey_EncoderDecoder) &
                              "&entrata_diretta=" & Sicurezza.Stringa_Codifica(0, CostantiPersonalizzate.AgroKey_EncoderDecoder, Nothing)

                    Dim strJS As New StringBuilder
                    strJS.AppendLine("$(document).ready(function () { ")
                    strJS.AppendLine("      ChiamataParent_Id_Ageda(" & Operazione_Colturale_Generica.ID_Agenda & "); ")
                    strJS.AppendLine("      window.location = '" & PaginaLink & "'; ")
                    strJS.AppendLine(" });")
                    ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                                  String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)


                    'elimino i permessi e l'operazione
                    RipristinaSessione()

            End Select

            Gestisci_Fine_Salvataggio()

        End Sub


#Region "MustOverride"

        Protected MustOverride Function Creo_Operazione_Specifica_da_Generica() As AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.I_Operazione_Colturale

        Protected MustOverride Function Prepara_Operazione_Per_Salvataggio(ByRef messaggiErrore As AgronicaCoreUtility.Messages_Str) As Boolean

        Protected MustOverride Sub Gestisci_Fine_Salvataggio()

        Protected MustOverride Sub CaricaControlli_Da_OperazioneColturale()

        Protected MustOverride Sub LeggiImpostazioni()

        Protected MustOverride Sub caricaControlli()

        Protected MustOverride Sub AbilitaDisabilita_Controlli()

        Protected MustOverride Sub AbilitaDisabilita_Controlli_PerScrittura()

        Protected MustOverride Sub AbilitaDisabilita_Controlli_PerModifica()

        Protected MustOverride Sub AbilitaDisabilita_Controlli_PerLettura()

        Protected MustOverride Sub AggiornaOperazioniPerData()

        Protected MustOverride Sub CambioCentro()

        Protected MustOverride Sub CambioMagazzino()

        Protected MustOverride Sub CambioSpecie()

        Protected MustOverride Sub MasterUnload()

        Protected MustOverride Sub GetSpecie()
        ''evento dopo caricamento master

        'rifare e gestire meglio, fatta al volo
        Protected Sub BTN_CaricoMagazzino(ByVal sender As Object, ByVal e As System.EventArgs)


            Dim PaginaLink As String
            'Case LAVCOD_SCARICO, LAVCOD_CARICO, LAVCOD_VENDITA, LAVCOD_ACQUISTO, LAVCOD_TRASFERIMENTO
            PaginaLink = "../GestioneMagazzini/FormProdotto.aspx"

            Dim xChiave As String = ""
            Dim mode As String = ""
            Dim Carico_Scarico As String = ""
            Call Albero.ChiaveAlbero_Codifica(xChiave, _
                                                   enum_TipoNodo.p_PortafoglioProdotti, _
                                                   Operazione_Colturale_Generica.Piva, _
                                                    Master_Operazione.Property_ComboMagazzini.Sa_Cod, , , , , , , , , , , , _
                                                   Master_Operazione.Property_ComboMagazzini.Fabbricato_Cod)

            Dim Lav_Cod As Integer = CostantiPersonalizzate.LAVCOD_CARICO
            Carico_Scarico = "C"
            mode = "magazzino"


            PaginaLink = PaginaLink & _
                            "?k=" & Sicurezza.Stringa_Codifica(xChiave, CostantiPersonalizzate.AgroKey_EncoderDecoder) & _
                            "&c=" & Sicurezza.Stringa_Codifica(Carico_Scarico, CostantiPersonalizzate.AgroKey_EncoderDecoder) & _
                            "&o=" & Sicurezza.Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, CostantiPersonalizzate.AgroKey_EncoderDecoder) & _
                            "&orig=" & Sicurezza.Stringa_Codifica(enum_PagineAgenda_2010.Menu, CostantiPersonalizzate.AgroKey_EncoderDecoder) & _
                            "&mode=" & Sicurezza.Stringa_Codifica(mode, CostantiPersonalizzate.AgroKey_EncoderDecoder) & _
                             "&l=" & Sicurezza.Stringa_Codifica(Operazione_Colturale_Generica.Lav_Cod, CostantiPersonalizzate.AgroKey_EncoderDecoder) & _
                            "&d=" & Sicurezza.Stringa_Codifica(CStr(Operazione_Colturale_Generica.DataOperazione), CostantiPersonalizzate.AgroKey_EncoderDecoder) & _
                            "&s=" & Sicurezza.Stringa_Codifica(Operazione_Colturale_Generica.Sa_Cod, CostantiPersonalizzate.AgroKey_EncoderDecoder) & _
                            "&a=" & Sicurezza.Stringa_Codifica(Operazione_Colturale_Generica.ID_Agenda, CostantiPersonalizzate.AgroKey_EncoderDecoder) & _
                            "&exit=true"


            Dim strJS As String
            strJS = "<script language='javascript'>" & _
                "           window.open('" & PaginaLink & "' ," & _
                "           'stampe'," & _
                "           'height=700," & _
                "           width=1000," & _
                "           menubar=yes," & _
                "           resizable=yes," & _
                "           scrollbars=yes," & _
                "           top=0,left=0');" & _
                " </script> "

            ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel), _
                                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel).GetType(), _
                                                "jQuery_{0}", strJS, False)
        End Sub

#End Region

    End Class

End Namespace

