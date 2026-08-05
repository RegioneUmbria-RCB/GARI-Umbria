Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaDomandaIrrigua.Agro_Pages_NS
Imports AgronicaCoreModello.ParametriAgenda_Temp

Public Class Reinnesco_Rilievi_Trappole
    Inherits Agro_Pages_NS.Agro_Page_Operazione_Colturale

    Dim objParametriAgenda As New ParametriAgenda


    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub



    'Variabile dell'operazione specifica che punta all'operazione generica (per evitare il cast),
    'RICORDARSI nel preinit di associare la variabile specifica all'Operazione_Colturale_Generica 
    Dim Operazione_Reinnesco_Trappole As AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.Aggiornamento_Trappole.Reinnesco_Trappole


#Region "Metodi Ordinati Per Flusso Di Controllo"


    Private Sub Reinnesco_Trappole_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit
        'Wrap dell'operazione generica nella variabile dell'operazione specifica  (per evitare il cast)
        Operazione_Reinnesco_Trappole = Operazione_Colturale_Generica
        AddHandler CType(Page.Master, Operazione).Property_ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto

    End Sub

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim link As String = ""

        Try
            Dim sitoorigine As Enum_SiteRedirector = objParametriAgenda.SitoOrigine
            Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine


            If sitoorigine = Enum_SiteRedirector.GiasNG Then
                AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                    Enum_SiteRedirector.GiasNG,
                                                                    objParametriAgenda.PaginaSitoOrigine,
                                                                    link,
                                                                    objParametri_Server)
            Else
                link = CType(Page.Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
            End If

        Catch ex As Exception
            link = CType(Page.Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
        End Try

        Response.Redirect(link)
    End Sub

    Private Sub Reinnesco_Trappole_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        Dim Script As New StringBuilder
        Script.AppendLine("     $('#chkSelezionaTutteLeTrappole').click(function (){ ")
        Script.AppendLine("         SelezionaDeselezionaTutteTrappole();")
        Script.AppendLine("     });")

        Script.AppendLine("     $('.ChkSelezionaTrappola').click(function (){ ")
        Script.AppendLine("         ChkSelezionaTrappola_Click();")
        Script.AppendLine("     });")

        ScriptManager.RegisterStartupScript(Master_Operazione.Property_UpdatePanelPerScript, Master_Operazione.Property_UpdatePanelPerScript.GetType(),
                                        String.Format("jQuery_{0}", Master_Operazione.Property_UpdatePanelPerScript.ClientID), Script.ToString, True)
    End Sub

    'genero l'operazione colturare specifica a partire dalla operazione generica I_Operazione_Colturale
    Protected Overrides Function Creo_Operazione_Specifica_da_Generica() As AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.I_Operazione_Colturale
        Dim Operazione_Temp As AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.I_Operazione_Colturale
        '------------------da togliereeeee'--------------------------DA MODIFICARE informaz<ione da mettere in operaz agenda generica----------------------
        Dim objParametriAgenda As New ParametriAgenda
        'objParametriAgenda.Leggi()
        '--------------------------DA MODIFICARE ----------------------

        Select Case CInt(objParametriAgenda.Lav_Cod)
            Case LAVCOD_REINNESCO_TRAPPOLE
                'modifica temporanea, per rilievo devo creare altro modo
                'modifica temporanea, per rilievo devo creare altro modo
                'modifica temporanea, per rilievo devo creare altro modo
                'modifica temporanea, per rilievo devo creare altro modo
                Operazione_Temp = New AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.Aggiornamento_Trappole.Reinnesco_Trappole(Operazione_Colturale_Generica, objParametri_Server, objParametriAgenda.Lav_Cod)
            Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                'modifica temporanea, per rilievo devo creare altro modo
                'modifica temporanea, per rilievo devo creare altro modo
                'modifica temporanea, per rilievo devo creare altro modo
                'modifica temporanea, per rilievo devo creare altro modo
                Operazione_Temp = New AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.Aggiornamento_Trappole.Reinnesco_Trappole(Operazione_Colturale_Generica, objParametri_Server, objParametriAgenda.Lav_Cod)
            Case Else
                Throw New NotImplementedException
        End Select

        Return Operazione_Temp
    End Function


    'Questo metodo viene chiamato solo se il parametro operazioneLettaCorrettamente è true.
    'Viene invocato dalla pagina principale nel load e non in postback, nel metodo caricaControlliGenerico che si esegue prima di caricaControlli della pagina
    Protected Overrides Sub CaricaControlli_Da_OperazioneColturale()
        Reinnesco_Trappoole_aspx_Utility.Carica_TabellaReinneschi_Da_OperazioneColturale(Me.GridView_Reinneschi,
                                                                           Operazione_Colturale_Generica,
                                                                           objParametri_Server)

        'CONTROLLO SE CI SONO DEI COSTI COLLEGATI
        Dim objParametriAgenda As New ParametriAgenda
        Dim objAgenda As New AgronicaCoreModello.OperazioneAgenda_Temp.Agenda_Operazione_Helper
        Dim Agenda As AgronicaCoreModello.OperazioneAgenda_Temp.Operazione_Agenda = objAgenda.Leggi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Id_Agenda, 0, objParametri_Server)

        For Each mdRif As AgronicaCoreModello.OperazioneAgenda_Temp.Movimento_Dettaglio_Riferimento In Agenda.Agenda_Riferimenti
            If mdRif.Lav_Cod_Rif = LAVCOD_COSTI_CDG Then
                'Messaggi.AgroMsgBuonFine("NB: Esistono costi collegati a questa operazione.", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                CType(Page.Master, Operazione).Property_hf_esistonoCostiCollegatiCDG = True
                Exit For
            End If
        Next

    End Sub


    'chiamato dopo gli altri abilitadisabilita controlli per.. 
    'disabilita e nasconde i controlli dell'operazione, per qualsiasi tipo di operazione (modifica, lettura o scrittura)
    Protected Overrides Sub AbilitaDisabilita_Controlli()

        Master_Operazione.Property_CBL_Consigli.Visible = True
        Master_Operazione.Property_GridView_Impianti.Visible = False

        Select Case CInt(Operazione_Colturale_Generica.Lav_Cod)

            Case LAVCOD_REINNESCO_TRAPPOLE

            Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                'per il rilievo avversità non serve il magazzino
                Master_Operazione.Property_ComboMagazzini.ddl_Magazzini.SelectedValue = "0"
                Master_Operazione.Property_ComboMagazzini.Enabled = False
                Master_Operazione.Property_Div_ProvenienzaRisorse.Visible = False

            Case Else
                Throw New NotImplementedException
        End Select


    End Sub


    Protected Overrides Sub AbilitaDisabilita_Controlli_PerScrittura()
        'nella scrittura visualizzo il pulsante per visualizzare le trappole in base al filtro.
        Me.ImageButton_MostraTrappole.Visible = True
        Me.ImageButton_MostraTrappole.Enabled = True
        Me.ImageButton_Sblocca.Visible = False
        Me.ImageButton_Sblocca.Enabled = False
        Me.sfondoverde.Visible = True
        Me.GridView_Reinneschi.Visible = False
        Me.GridView_Reinneschi.Enabled = False
        Me.GridView_TrappoleInstallate.Visible = True
        Me.GridView_TrappoleInstallate.Enabled = True
        Master_Operazione.flag_MostraBtnSalvaCDG = True
    End Sub


    Protected Overrides Sub AbilitaDisabilita_Controlli_PerModifica()
        'nella modifica e lettura non visualizzo il pulsante generare la tabella dele trappole ma la le genero 
        'automaticvamente in base all'ìoperazione dell'agenda.
        Me.ImageButton_MostraTrappole.Visible = False
        Me.ImageButton_MostraTrappole.Enabled = False
        Me.ImageButton_Sblocca.Visible = False
        Me.ImageButton_Sblocca.Enabled = False
        Me.sfondoverde.Visible = False
        Me.GridView_Reinneschi.Visible = True
        Me.GridView_Reinneschi.Enabled = True
        Me.GridView_TrappoleInstallate.Visible = False
        Me.GridView_TrappoleInstallate.Enabled = False
        Master_Operazione.flag_MostraBtnSalvaCDG = True
    End Sub


    Protected Overrides Sub AbilitaDisabilita_Controlli_PerLettura()
        'nella modifica e lettura non visualizzo il pulsante generare la tabella dele trappole ma la le genero 
        'automaticvamente in base all'ìoperazione dell'agenda.
        Me.ImageButton_MostraTrappole.Visible = False
        Me.ImageButton_MostraTrappole.Enabled = False
        Me.ImageButton_Sblocca.Visible = False
        Me.ImageButton_Sblocca.Enabled = False
        Me.sfondoverde.Visible = False
        Me.GridView_Reinneschi.Visible = True
        Me.GridView_Reinneschi.Enabled = False
        Me.GridView_TrappoleInstallate.Visible = False
        Me.GridView_TrappoleInstallate.Enabled = False
    End Sub

#End Region



#Region "Gestione Eventi Controlli"


    Private Sub ImageButton_Sblocca_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImageButton_Sblocca.Click
        sblocca()
    End Sub

    Private Sub sblocca()
        AbilitaDisabilita_ControlliMaster_PerSblocco_InScrittura_Standard()
        Me.ImageButton_MostraTrappole.Visible = True
        Me.ImageButton_MostraTrappole.Enabled = True
        Me.ImageButton_Sblocca.Visible = False
        Me.ImageButton_Sblocca.Enabled = False
        Me.LabelMostraSlblocca.Text = Resources.AgronicaAgenda_2010.MostraLeTrappole

        Me.GridView_TrappoleInstallate.DataSource = Nothing
        Me.GridView_TrappoleInstallate.DataBind()

        Me.DivNoteTabella.Visible = False
    End Sub

    Protected Sub ImageButton_MostraTrappole_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImageButton_MostraTrappole.Click
        mostra()
    End Sub

    Private Sub mostra()
        '--------------------CONTROLLO COERENZA PIVA SACOD-------------
        'Dato che il reinnesco non supporta il multicentro, e ancore
        'occorre gestirlo anche direttammente nell'oggetto operazione,
        'verifico che tutte le trappole e quindi l'operazione sia riferita allo stesso centro 
        'In questo caso recupero il sa_dalla combo, quindi i reinnesci appartenmgono per forza allo stesso centro
        'Comunque il controllo viene fatto anche quando:
        ' si preme il pulsante per generare l'impianto (che ci sia un centro nella combo selezionato): ImageButton_MostraTrappole_Click
        ' quando si genera la lista reinneschi per l'oggetto operazione da salvare: Genera_Lista_Reinneschi_Nella_OperazioneColturale_Da_TabellaTrappole_X_Reinnesco
        'quando si salva l'operazione tramite IO utility OperazioneColturale_ToFrom_AgendaDB: si controlla che operazione.sa_cod non sia 0 e che destinazioni piva e sacod corrispondano a quelli dell'operazione
        'sa_cod operazione è 0, viene assegnato quando si prepara l'operazione per il salvataggio
        Dim Sa_Cod_Temp = Master_Operazione.Property_ComboCentroAziendale.Valore_Combo
        If Sa_Cod_Temp = 0 Then
            'messaggiErrore.add(" Centro operazione e centro impianto destinazione non corrispondono, questa operazione non gestisce il multicentro ")
            Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.AlMomentoOccorreSelezionareAlmenoUnCentroA, Page, , _
                )
            Exit Sub
        End If
        '--------------------CONTROLLO FINE-------------


        Dim Veg_Cod As Integer = 0
        Dim comboSpecVal As String = Master_Operazione.Property_ComboSpecie.Valore_Combo
        If IsNumeric(comboSpecVal) AndAlso comboSpecVal > 0 Then
            Veg_Cod = CInt(comboSpecVal)
        End If
        Dim MagazPiva As String = ""
        Dim MagazSa_Cod As Integer = 0
        Dim MagazFabbricato As Integer = 0
        If Master_Operazione.Property_ComboMagazzini.ddl_Magazzini.SelectedValue.Contains("|") Then
            MagazPiva = Split(Master_Operazione.Property_ComboMagazzini.ddl_Magazzini.SelectedValue, "|")(2)
            MagazSa_Cod = CInt(Split(Master_Operazione.Property_ComboMagazzini.ddl_Magazzini.SelectedValue, "|")(1))
            MagazFabbricato = CInt(Split(Master_Operazione.Property_ComboMagazzini.ddl_Magazzini.SelectedValue, "|")(0))
        End If
        'devo ricordare che magazzino non è in operazione ma viene aggiunto dalla page_operazione
        'quando prepara operazione per il salvataggio, solo li posso modificare l'operazione, 
        'quindi per il resto uso i valori delle combo della master
        Reinnesco_Trappoole_aspx_Utility.caricaTabellaTrappole_X_Reinnesco(Me.GridView_TrappoleInstallate, _
                                                                   Operazione_Colturale_Generica.Piva, _
                                                                   Sa_Cod_Temp, _
                                                                   "", _
                                                                   "", _
                                                                   Veg_Cod, _
                                                                   0, _
                                                                   0, _
                                                                   CDate(Master_Operazione.Property_txt_DataOperazione.Text), _
                                                                   MagazPiva, _
                                                                   MagazSa_Cod, _
                                                                   MagazFabbricato, _
                                                                   Operazione_Colturale_Generica.Lav_Cod, _
                                                                   objParametri_Server)


        'coloro le righe con inneschi scaduti
        For Each row As GridViewRow In Me.GridView_TrappoleInstallate.Rows
            'CType(row.FindControl("Inneschi_Attivi"),
            If row.Cells(14).Text = "0" Then
                row.BackColor = Drawing.Color.Pink
            End If
        Next

        AbilitaDisabilita_ControlliMaster_PerInserimento_InScrittura_Standard()
        Me.ImageButton_MostraTrappole.Visible = False
        Me.ImageButton_MostraTrappole.Enabled = False
        Me.ImageButton_Sblocca.Enabled = True
        Me.ImageButton_Sblocca.Visible = True
        Me.LabelMostraSlblocca.Text = Resources.AgronicaAgenda_2010.Rimuovi

        Me.DivNoteTabella.Visible = True
        Me.Label5.Text = Resources.AgronicaAgenda_2010.NB
        Me.Label1.Text = Resources.AgronicaAgenda_2010.SituazioneTrappoleAl
        Me.Label2.Text = Master_Operazione.Property_txt_DataOperazione.Text
        Me.Label3.Text = Resources.AgronicaAgenda_2010.LaColonnaInneschiAttiviVisualizzaGliInnesc
        Me.Label4.Text = Resources.AgronicaAgenda_2010.SonoVisualizzatePerIlReinnescoLeSoleTrappo
        Me.Label6.Text = Resources.AgronicaAgenda_2010.LeRigheInRosaIndicanoCheGliInneschiSonoSca
        Me.Label6.BackColor = Drawing.Color.Pink
    End Sub


    Protected Overrides Function Prepara_Operazione_Per_Salvataggio(ByRef messaggiErrore As AgronicaCoreUtility.Messages_Str) As Boolean


        '--------------BLOCCO SALVATAGGIO SENZA MAGAZZINO-------------------
        If Not IsNothing(Session("UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO")) AndAlso Session("UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO") = True AndAlso Operazione_Colturale_Generica.Lav_Cod = LAVCOD_REINNESCO_TRAPPOLE Then
            If IsNothing(Operazione_Reinnesco_Trappole.Magazzino) Then
                messaggiErrore.add("In base alle impostazioni utente NON è possibile salvare l'operazione senza utilizzare il magazzino!")
                Return False
            End If

        End If
        '---------------------------------------------------------------------

        'attenzione, in alcuni casi salvava il magazzino anche per i rilievi,
        'forse rimaneva in sessione in alcuni casi,
        'per prevenire in quel caso lo tolgo
        If Operazione_Colturale_Generica.Lav_Cod = LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE Then
            Operazione_Colturale_Generica.Magazzino = Nothing
        End If

        Select Case Operazione_Colturale_Generica.Tipo_Operazione
            Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura

                'inserisco il centro aziendale (nella Genera_Lista_Reinneschi contrrollo anche che i centri corrispondano a questo,
                'comunque dovrei bloccare la combo dopo che ho creato la griglia per evitare incongruenze)
                Dim Sa_Cod_Temp = Master_Operazione.Property_ComboCentroAziendale.Valore_Combo
                If Sa_Cod_Temp = 0 Then
                    messaggiErrore.add(Resources.AgronicaAgenda_2010.OccorreSelezionareAlmenoUnCentroAziendaleP)
                    Return False
                Else
                    Operazione_Colturale_Generica.Sa_Cod = Sa_Cod_Temp
                End If
                Dim UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE As Boolean = False
                If Not IsNothing(Session("UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE")) Then
                    UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE = Session("UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE")
                Else
                    UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE = False
                End If

                Dim res As Boolean = Genera_Lista_Reinneschi_Nella_OperazioneColturale_Da_TabellaTrappole_X_Reinnesco(GridView_TrappoleInstallate, Operazione_Reinnesco_Trappole, CDate(Master_Operazione.Property_txt_DataOperazione.Text), objParametri_Server, messaggiErrore, Giacenza_SI_NO, Me, UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE)
                'controllo coerenza dati operazione ,
                'se ritorna false passa il messaggio alla pagina con l'errore che viene visualizzato,
                'anche nel caso res sia vuota ad esempio
                If Not res Then
                    Operazione_Reinnesco_Trappole.Reinneschi.Clear()
                    'messaggio passato alla pagina generica
                    Return False
                End If
            Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica
                'chiamo la funzione che mi aggiorna il numero di reinneschi nell'operazione,
                'utilizzo l'oggetto operazione in memoria, modifico solo il numero di reinneschi
                Dim res As Boolean = Aggiorna_Numero_Reinneschi_Nella_OperazioneColturale_Da_Tabella_Reinneschi(GridView_Reinneschi, Operazione_Reinnesco_Trappole, CDate(Master_Operazione.Property_txt_DataOperazione.Text), objParametri_Server, messaggiErrore, Giacenza_SI_NO, Me)
                'controllo coerenza dati operazione ,
                'se ritorna false passa il messaggio alla pagina con l'errore che viene visualizzato,
                'anche nel caso res sia vuota ad esempio
                If Not res Then
                    'Operazione_Reinnesco_Trappole.Reinneschi.Clear()  NO!!! altrimenti mi cancella i reineschi e quando ho giacenze negatrive e spingo continua mi salva opewrazione senza innneschi!!!
                    'messaggio passato alla pagina generica
                    Return False
                End If

            Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura
                Throw New NotImplementedException
            Case Else
                Throw New NotImplementedException
        End Select


        Return True

    End Function


    Protected Overrides Sub Gestisci_Fine_Salvataggio()
        Session("UtilizzataRicetta") = False
        Dim TipoSalvataggio As String = Master_Operazione.Property_RBL_Salva.SelectedValue + 1

        Select Case TipoSalvataggio
            Case enum_Tipo_Salvataggio.Salva_e_Esci

            Case enum_Tipo_Salvataggio.Salva_e_Nuovo

            Case enum_Tipo_Salvataggio.Salva_e_Duplica
                sblocca()
                mostra()

            Case enum_Tipo_Salvataggio.Salva_e_Vai_ai_Costi

        End Select

        'rirpistino variabile nascosta per messagebox sino  giacenze
        Giacenza_SI_NO.Value = "0"

    End Sub

#End Region












#Region "Overrides Non Implementati"



    Protected Overrides Sub LeggiImpostazioni()

        'Select Operazione_Colturale_Generica.Tipo_Operazione

        '    Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura

        '    Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica

        '    Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura

        '    Case Else
        '        Throw New NotImplementedException
        'End Select

    End Sub


    'La pagina Agro_page_Generica tramite il metodo caricaControlli_Generale() , nel caso di modifica e lettura richiama già
    'i metodi:
    ' Leggi_Operazione_Da_Agenda() che legge l'operazione coluturale e crea l'oggetto corrispondente in I_Operazione_Colturale
    'CaricaControlli_DaOperazioneColturale() che deve essere overrides da questa pagina e deve impostare i controlli in base all'oggetto operazione
    'Questo metodo viene chiamato solo se il parametro operazioneLettaCorrettamente è true
    Protected Overrides Sub caricaControlli()
        'ne qui ne nella  master, voglio costruire l'operazione solo qualdo creo effettivamente per il sslvataggio, in creo 
        'PreparaOperazionePerSalvataggioGenerica in modo da evitare srrori, ripetizioni, incongruenze. in tutti i rimanenti eventi
        'setto aol massimo delle variabili o faccio altre cose, orientate alla presentazione, non alla modifica del modello

        'Select Case Operazione_Colturale_Generica.Tipo_Operazione

        '    Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
        '        'nella scrittura non genero la tabella delle trappole che viene creata solo quando si preme il pulsante e dopo avere quindi 
        '        'impostato i parametri di filtro

        '    Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica, TipiEnumerativi.enum_TipoOperazioneDB.Lettura

        '    Case Else
        '        Throw New NotImplementedException
        'End Select

    End Sub


    Protected Overrides Sub CambioCentro()
        'ne qui ne nella  master, voglio costruire l'operazione solo qualdo creo effettivamente per il sslvataggio, in creo 
        'PreparaOperazionePerSalvataggioGenerica in modo da evitare srrori, ripetizioni, incongruenze. in tutti i rimanenti eventi
        'setto aol massimo delle variabili o faccio altre cose, orientate alla presentazione, non alla modifica del modello

        ''caricaTabellaTrappole_X_Reinnesco(GridView_TrappoleInstallate, "00", 0, "", "", 0, 0, 0, AGRODATAFINE, 0, objParametri_Server)
        'Dim str As New StringBuilder


        'str.AppendLine("$(document).ready(function () { ")


        'str.AppendLine("    $('#" & prova.ClientID & "').click();")

        'str.AppendLine("    });")

        'ScriptManager.RegisterStartupScript(Page, Page.GetType(),
        '                              String.Format("jQuery"), Page.ToString, True)

        ''Me.GridView_TrappoleInstallate.DataBind()
    End Sub


    Protected Overrides Sub CambioMagazzino()
        'ne qui ne nella  master, voglio costruire l'operazione solo qualdo creo effettivamente per il sslvataggio, in creo 
        'PreparaOperazionePerSalvataggioGenerica in modo da evitare srrori, ripetizioni, incongruenze. in tutti i rimanenti eventi
        'setto aol massimo delle variabili o faccio altre cose, orientate alla presentazione, non alla modifica del modello
        'Il magazzino lo setto quindi solo li, altrimenti uspo il valore della combo della master
        'Dim Fabbricato As Integer = CInt(Split(Master_Operazione.Property_ComboMagazzini.ddl_Magazzini.SelectedValue, "|")(0))
        'Dim SaCod_Fabbricato As Integer = CInt(Split(Master_Operazione.Property_ComboMagazzini.ddl_Magazzini.SelectedValue, "|")(1))

    End Sub


    Protected Overrides Sub CambioSpecie()
        ''da overrides
        'ne qui ne nella  master, voglio costruire l'operazione solo qualdo creo effettivamente per il sslvataggio, in creo 
        'PreparaOperazionePerSalvataggioGenerica in modo da evitare srrori, ripetizioni, incongruenze. in tutti i rimanenti eventi
        'setto aol massimo delle variabili o faccio altre cose, orientate alla presentazione, non alla modifica del modello
    End Sub


    Protected Overrides Sub MasterUnload()
        'ne qui ne nella  master, voglio costruire l'operazione solo qualdo creo effettivamente per il sslvataggio, in creo 
        'PreparaOperazionePerSalvataggioGenerica in modo da evitare srrori, ripetizioni, incongruenze. in tutti i rimanenti eventi
        'setto aol massimo delle variabili o faccio altre cose, orientate alla presentazione, non alla modifica del modello
        ''da overrides
        ''evento dopo caricamento master
    End Sub


    Protected Overrides Sub aggiornaOperazioniPerData()
        'ne qui ne nella  master, voglio costruire l'operazione solo qualdo creo effettivamente per il sslvataggio, in creo 
        'PreparaOperazionePerSalvataggioGenerica in modo da evitare srrori, ripetizioni, incongruenze. in tutti i rimanenti eventi
        'setto aol massimo delle variabili o faccio altre cose, orientate alla presentazione, non alla modifica del modello
    End Sub



    Protected Overrides Sub GetSpecie()
        'serve alla pagina generica per capire se la scoedie è permessa e cambiare tipo operzione
    End Sub

#End Region



#Region "Non Implementati e Commentati"

    'Private Sub inizializzoParametriPagina()

    '    'Select Case CInt(Operazione_Colturale_Generica.Lav_Cod)

    '    '    Case LAVCOD_REINNESCO_TRAPPOLE

    '    '    Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE

    '    '    Case Else
    '    '        Throw New NotImplementedException
    '    'End Select

    'End Sub


    Private Sub GridView_Reinneschi_Sorting(sender As Object, e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridView_Reinneschi.Sorting

    End Sub

    Private Sub GridView_TrappoleInstallate_Sorting(sender As Object, e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridView_TrappoleInstallate.Sorting
        '    Dim dt As DataTable = GridView_TrappoleInstallate.DataSource
        '    If Not IsNothing(dt) Then
        '        Dim dv As New DataView(dt)
        '        dv.Sort = String.Format("{0} {1}", e.SortExpression, ConvertSort(e.SortDirection))
        '        GridView_TrappoleInstallate.DataSource = dv
        '        GridView_TrappoleInstallate.DataBind()
        '    End If
        'End Sub

        'Private Function ConvertSort(ByRef sortDirection As SortDirection) As String
        '    Dim m_SortDirection As String = ""
        '    Select Case sortDirection
        '        Case sortDirection.Ascending
        '            m_SortDirection = "ASC"
        '        Case sortDirection.Descending
        '            m_SortDirection = "DESC"
        '    End Select
        '    Return m_SortDirection
    End Sub


#End Region




End Class


Module Reinnesco_Trappoole_aspx_Utility



    Sub caricaTabellaTrappole_X_Reinnesco(ByRef gridView As GridView, _
                                          ByVal Piva As String, _
                                          ByVal Sa_cod As Integer, _
                                          ByVal Sigla_AV As String, _
                                          ByVal Appezza As String, _
                                          ByVal Veg_Cod As Integer, _
                                          ByVal Cul_Cod As Integer, _
                                          ByVal Gru_Cod As Integer, _
                                          ByVal Data_Di_Controllo As Date, _
                                          ByVal Magazzino_Piva As String, _
                                          ByVal Magazzino_Sa_Cod As Integer, _
                                          ByVal Magazzino_Fabbricato_Cod As Integer, _
                                          ByVal Lav_Cod As Integer, _
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim DT_Completo As System.Data.DataTable
        Dim objMvDetTecn As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
        DT_Completo = objMvDetTecn.Trappole_Installate_X_Reinneschi(Piva, _
                                                                   Sa_cod, _
                                                                   Sigla_AV, _
                                                                   Appezza, _
                                                                   Veg_Cod, _
                                                                   Cul_Cod, _
                                                                   Gru_Cod, _
                                                                   Data_Di_Controllo, _
                                                                   "", _
                                                                   objParametri)



        'ora devo filtrare le righe duplicate della trappola (piva-cacod-trapnum-siglaav) che sono state duplicate avendo piu reinneschi.
        'Seleziono la riga con reinnesco con data piu alta e nel caso di data identica seleziono quella con id_agenda piu alto
        Dim temp_piva As String = ""
        Dim temp_Sa_cod As String = ""
        Dim temp_Sigla_AV As String = ""
        Dim temp_Trap_Num As String = ""
        Dim temp_Data_Reinnesco As String = ""
        Dim temp_Reinnesco_Id_Agenda As String = ""
        Dim temp2_piva As String = ""
        Dim temp2_Sa_cod As String = ""
        Dim temp2_Sigla_AV As String = ""
        Dim temp2_Trap_Num As String = ""
        Dim temp2_Data_Reinnesco As String = ""
        Dim temp2_Reinnesco_Id_Agenda As String = ""
        Dim DT_Scremato As New System.Data.DataTable
        Dim riga As System.Data.DataRow
        Dim riga2 As System.Data.DataRow

        DT_Scremato = DT_Completo.Clone()

        'scorro la lista delle righe
        For i = 0 To DT_Completo.Rows.Count - 1

            riga = DT_Completo.Rows(i)
            If IsDBNull(riga.Item("Data_Reinnesco")) Then
                'se la riga non ha reinnesco allora è ricuramente unica e la aggiungo alla dt scremata
                'aggiungo inoltre 
                DT_Scremato.ImportRow(riga)
            Else
                'se la riga ha un reinnesco controllo che sia il piu recente
                temp_piva = riga.Item("Piva")
                temp_Sa_cod = riga.Item("Sa_cod")
                temp_Sigla_AV = riga.Item("Sigla_AV")
                temp_Trap_Num = riga.Item("Trap_Num")
                temp_Data_Reinnesco = riga.Item("Data_Reinnesco")
                temp_Reinnesco_Id_Agenda = riga.Item("Reinnesco_Id_Agenda")


                'scorro tutta la datatable per vedere le righe che riguardano la stessa trappola
                For j = 0 To DT_Completo.Rows.Count - 1

                    riga2 = DT_Completo.Rows(j)
                    If IsDBNull(riga2.Item("Data_Reinnesco")) Then
                        'salto le righe senza reinnesco, non riguardano sicuramente la trappola che sto considerando

                    Else
                        'riga con reinnesco, ricavo l'identificativo della trappola, la data di reinnesco e l'id agenda
                        temp2_piva = riga2.Item("Piva")
                        temp2_Sa_cod = riga2.Item("Sa_cod")
                        temp2_Sigla_AV = riga2.Item("Sigla_AV")
                        temp2_Trap_Num = riga2.Item("Trap_Num")
                        temp2_Data_Reinnesco = riga2.Item("Data_Reinnesco")
                        temp2_Reinnesco_Id_Agenda = riga2.Item("Reinnesco_Id_Agenda")

                        'controllo l'id trappola
                        If temp_Trap_Num = temp2_Trap_Num AndAlso temp_piva = temp2_piva _
                            AndAlso temp_Sa_cod = temp2_Sa_cod AndAlso temp_Sigla_AV = temp2_Sigla_AV Then

                            Dim Datediff As Integer = DateTime.Compare(CDate(temp_Data_Reinnesco), CDate(temp2_Data_Reinnesco))
                            'se la riga con reinnesco riguarda la trappola che sto considerando, controllo la data
                            If Datediff < 0 Then
                                'se trovo una riga con la stessa trappola e con reinnesco piu recente  posso uscire,
                                'questa riga non sarà aggiunta perchè ne esiste una con reinnesco piu recente
                                Exit For
                            End If

                            If Datediff = 0 Then
                                'Se trovo una riga con la stessa data controllo l'id agenda,
                                If temp_Reinnesco_Id_Agenda < temp2_Reinnesco_Id_Agenda Then
                                    'se la riga che sto controllando ha idagenda piu basso allora la considero meno recente e come nel punto precedente è
                                    'allora da non aggiungere
                                    Exit For
                                End If
                            End If
                            If Datediff > 0 Then
                                'se trovo una riga con la stessa trappola e con reinnesco meno recente la riga 
                                'potrebbe essere da aggiungere se non ne trovo con reinneschi piu recenti

                            End If
                        End If
                    End If

                    If j = DT_Completo.Rows.Count - 1 Then
                        'se sono arrivato all'ultima riga senza uscire allora aggiungo la riga che stavo controllando dato
                        'che non ne ho trovate altre con reinnesco piu recente
                        DT_Scremato.ImportRow(riga)
                    End If

                Next


            End If

        Next

        'aggiungo le colonne Uso_Desc, Scadenza e Inneschi_Attivi 
        DT_Scremato.Columns.Add("Uso_Desc", GetType(String))
        DT_Scremato.Columns.Add("Scadenza", GetType(String))
        DT_Scremato.Columns.Add("Inneschi_Attivi", GetType(Integer))
        DT_Scremato.Columns.Add("Giacenza", GetType(Integer))
        'popolo le colonne aggiunte

        For i = 0 To DT_Scremato.Rows.Count - 1
            Dim data_Riferimento As Date
            Dim UltimiInneschi As Integer = 0
            riga = DT_Scremato.Rows(i)
            If IsDBNull(riga.Item("Data_Reinnesco")) Then
                'se la riga non ha reinnesco allora la data di riferimento per la scadenza è la data di installazione trappola
                data_Riferimento = CDate(riga.Item("Validita_Inizio"))
                'e gli ultimi inneschi sono quelli dell'installazione
                UltimiInneschi = CInt(riga.Item("Dose"))
            Else
                'se la riga ha reinnesco allora la data di riferimento per la scadenza è la data di reinnesco
                data_Riferimento = CDate(riga.Item("Data_Reinnesco"))
                'e gli ultimi inneschi sono quelli dell'installazione
                UltimiInneschi = CInt(riga.Item("Reinnesco_Dose"))
            End If
            'giorni durata trappola
            Dim Giorni As Integer = riga.Item("Trap_Dur")

            'imposto la scadenza
            Dim data_scadenza As Date
            data_scadenza = DateAdd(DateInterval.Day, Giorni, data_Riferimento)
            riga.Item("Scadenza") = data_scadenza.ToShortDateString

            'se la data di controllo, cioè quella dell'operazione è precedente la data di scadenza 
            ' allora ho inneschi attivi,  quelli dell'ultimo reinnesco o della installazione

            If DateTime.Compare(Data_Di_Controllo, data_scadenza) < 0 Then
                riga.Item("Inneschi_Attivi") = UltimiInneschi
            Else
                riga.Item("Inneschi_Attivi") = 0
            End If

            'Imposto la descrizione dell'uso
            riga.Item("Uso_Desc") = (New AgronicaCoreMetaSchemaDAL.Trappole_R()).usoDesc_FromUso(CInt(riga.Item("Uso")))

            'Imposto la Qta2 se non è presente già in db perché salvato nel vecchio modo
            If riga.Item("Qta2") = 0 Then
                riga.Item("Qta2") = riga.Item("Sup_Imp")
            End If

            Select Case CInt(Lav_Cod)
                Case LAVCOD_REINNESCO_TRAPPOLE
                    If Magazzino_Fabbricato_Cod <> 0 Then
                        Dim objGiacenze As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                        'l'ultimo parametro, id_agenda, indica l'id_agenda da non considerare, ma qui sono in scrittura, 
                        'non devo non considerare l'idagenda della trappola corrente, non avrebbe senso, metto 0 
                        Dim idagendareinnesco = 0
                        'If Not IsDBNull(riga.Item("Reinnesco_Id_Agenda")) Then
                        '    If IsNumeric(riga.Item("Reinnesco_Id_Agenda")) Then
                        '        idagendareinnesco = riga.Item("Reinnesco_Id_Agenda")
                        '    End If
                        'End If
                        Dim GiacenzaInneschi As Integer = objGiacenze.Verifica_Giacenze_Con_Magazzino_Esterno(Magazzino_Piva, _
                                                                    CInt(Magazzino_Sa_Cod), _
                                                                    CInt(Magazzino_Fabbricato_Cod), _
                                                                    CInt(198), _
                                                                    CInt(riga.Item("Av_Cod")), _
                                                                    0, _
                                                                    0, 0, AgronicaCoreDataProvider.CostantiPersonalizzate.LOTTO_NONDEFINITO, _
                                                                    0, 0, _
                                                                    AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, _
                                                                    Data_Di_Controllo, _
                                                                    Piva, _
                                                                    Sa_cod, _
                                                                    idagendareinnesco, _
                                                                    objParametri)

                        riga.Item("Giacenza") = GiacenzaInneschi

                    Else
                        riga.Item("Giacenza") = 0
                    End If
                Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                    'per il rilievo avversità non serve il magazzino
                Case Else
                    Throw New NotImplementedException
            End Select




        Next

        'Nascondo alcune colonnee modifico intestazione se sono in rilievo avversita
        Select Case CInt(Lav_Cod)

            Case LAVCOD_REINNESCO_TRAPPOLE

            Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE

                'cambio intestazione di 'reinneschi ' in 'avversità rilevate'
                gridView.Columns(16).HeaderText = Resources.AgronicaAgenda_2010.NumeroAvversitàRilevate
                gridView.Caption = Resources.AgronicaAgenda_2010.IndicareIlNumeroDiAvversitàRilevateNelleTr
                'Nascondo colonne inneschi alla installazione e al reinnesco e giacenza,
                gridView.Columns(10).Visible = False
                gridView.Columns(12).Visible = False
                gridView.Columns(17).Visible = False
            Case Else
                Throw New NotImplementedException
        End Select

        SettaTabellaGridview(DT_Scremato, gridView, Lav_Cod)

    End Sub

    Sub Carica_TabellaReinneschi_Da_OperazioneColturale(ByRef gridView As GridView, ByRef I_Operazione_Colturale As AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.Aggiornamento_Trappole.Reinnesco_Trappole, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim Dt As New DataTable("Reinneschi")

        Dt.Columns.Add("PIVA", GetType(String))
        Dt.Columns.Add("Rag_Soc", GetType(String))
        Dt.Columns.Add("Sa_Cod", GetType(Integer))
        Dt.Columns.Add("Sa_nome", GetType(String))
        Dt.Columns.Add("Appezza", GetType(Integer))
        Dt.Columns.Add("ID_Reg", GetType(Integer))
        Dt.Columns.Add("App_nome", GetType(String))
        Dt.Columns.Add("Veg_Cod", GetType(Integer))
        Dt.Columns.Add("Veg_Des", GetType(String))
        Dt.Columns.Add("Cul_Cod", GetType(Integer))
        Dt.Columns.Add("Cul_Des", GetType(String))
        Dt.Columns.Add("Trap_Num", GetType(Integer))
        Dt.Columns.Add("Freatimetro", GetType(Integer))
        Dt.Columns.Add("Ditta_Cod", GetType(Integer))
        Dt.Columns.Add("TRAP_COD", GetType(Integer))
        Dt.Columns.Add("Trap_Des", GetType(String))
        Dt.Columns.Add("Sigla_AV", GetType(String))
        Dt.Columns.Add("Av_Cod", GetType(Integer))
        Dt.Columns.Add("Uso", GetType(Integer))
        Dt.Columns.Add("Uso_Desc", GetType(String))
        Dt.Columns.Add("Validita_Inizio", GetType(String))
        'Dt.Columns.Add("Dose", GetType(Integer))
        Dt.Columns.Add("Data_Reinnesco", GetType(String))
        'Dt.Columns.Add("Reinnesco_Dose", GetType(Integer))
        Dt.Columns.Add("Scadenza", GetType(String))
        'Dt.Columns.Add("Inneschi_Attivi", GetType(Integer))
        Dt.Columns.Add("Giacenza", GetType(Integer))
        Dt.Columns.Add("Reinneschi", GetType(Integer))
        Dt.Columns.Add("Qta2", GetType(Decimal))

        For i = 0 To I_Operazione_Colturale.Reinneschi.Count - 1

            Dim row As DataRow = Dt.NewRow

            row.Item("PIVA") = I_Operazione_Colturale.Piva
            row.Item("Rag_Soc") = I_Operazione_Colturale.RagioneSociale
            row.Item("Sa_Cod") = I_Operazione_Colturale.Sa_Cod
            row.Item("Sa_nome") = (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(I_Operazione_Colturale.Piva, I_Operazione_Colturale.Sa_Cod, objParametri_Server)
            row.Item("Appezza") = I_Operazione_Colturale.Reinneschi(i).Trappola.Impianto_Di_Installazione.Appezza
            row.Item("ID_Reg") = I_Operazione_Colturale.Reinneschi(i).Trappola.Impianto_Di_Installazione.ID_Reg
            row.Item("App_nome") = (New AgronicaCoreAnagrafeDAL.Appezzamento_Read()).AppezzamentoNome_from_Appezza(I_Operazione_Colturale.Piva, I_Operazione_Colturale.Sa_Cod, I_Operazione_Colturale.Reinneschi(i).Trappola.Impianto_Di_Installazione.Appezza, objParametri_Server)
            row.Item("Veg_Cod") = (New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read()).VegCod_from_PivaSaCodAppezzaIdimp(I_Operazione_Colturale.Piva, I_Operazione_Colturale.Sa_Cod, I_Operazione_Colturale.Reinneschi(i).Trappola.Impianto_Di_Installazione.Appezza, I_Operazione_Colturale.Reinneschi(i).Trappola.Impianto_Di_Installazione.ID_Reg, "", "", objParametri_Server)
            row.Item("Veg_Des") = (New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R()).VegDes_from_VegCod(CInt(row.Item("Veg_Cod")), objParametri_Server)
            row.Item("Cul_Cod") = (New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read()).CulCod_from_PivaSaCodAppezzaIdimp(I_Operazione_Colturale.Piva, I_Operazione_Colturale.Sa_Cod, I_Operazione_Colturale.Reinneschi(i).Trappola.Impianto_Di_Installazione.Appezza, I_Operazione_Colturale.Reinneschi(i).Trappola.Impianto_Di_Installazione.ID_Reg, objParametri_Server)
            row.Item("Cul_Des") = (New AgronicaCoreMetaSchemaDAL.Cultivar_R()).CulDes_from_CulCod(CInt(row.Item("Cul_Cod")), objParametri_Server)
            row.Item("Trap_Num") = I_Operazione_Colturale.Reinneschi(i).Trappola.ID
            row.Item("Freatimetro") = I_Operazione_Colturale.Reinneschi(i).Trappola.ID_Personalizzato
            row.Item("Ditta_Cod") = I_Operazione_Colturale.Reinneschi(i).Trappola.Ditta_ID
            row.Item("TRAP_COD") = I_Operazione_Colturale.Reinneschi(i).Trappola.Prodotto_ID
            row.Item("Trap_Des") = (New AgronicaCoreMetaSchemaDAL.Trappole_R()).TrapDes_from_TrapCod(CInt(row.Item("TRAP_COD")), objParametri_Server)
            row.Item("Sigla_AV") = I_Operazione_Colturale.Reinneschi(i).Trappola.Avversita_Sigla
            row.Item("Av_Cod") = I_Operazione_Colturale.Reinneschi(i).Trappola.Avversita_ID
            row.Item("Uso") = (New AgronicaCoreMetaSchemaDAL.Trappole_R()).Uso_from_TrapCod(CInt(row.Item("TRAP_COD")), objParametri_Server)
            row.Item("Uso_Desc") = (New AgronicaCoreMetaSchemaDAL.Trappole_R()).usoDesc_FromUso(CInt(row.Item("Uso")))
            row.Item("Validita_Inizio") = ((New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R).DataInstallazioneTrappola(I_Operazione_Colturale.Piva, I_Operazione_Colturale.Sa_Cod, I_Operazione_Colturale.Reinneschi(i).Trappola.Avversita_Sigla, I_Operazione_Colturale.Reinneschi(i).Trappola.ID, objParametri_Server)).ToShortDateString
            'probabile cancellazione di trappola
            If row.Item("Validita_Inizio") = AGRODATAINIZIO Then
                row.Item("Validita_Inizio") = "ATTENZIONE! La trappola probabilmente è stata cancellata"
            End If

            If I_Operazione_Colturale.Reinneschi(i).Trappola.Impianto_Di_Installazione.Qta2 > 0 Then
                row.Item("Qta2") = I_Operazione_Colturale.Reinneschi(i).Trappola.Impianto_Di_Installazione.Qta2
            Else
                row.Item("Qta2") = (New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read()).LeggiSuperficie(row.Item("PIVA"), row.Item("Sa_Cod"), row.Item("Appezza"), row.Item("ID_Reg"), objParametri_Server)
            End If

            'row.Item("Dose") = 0
            row.Item("Data_Reinnesco") = (I_Operazione_Colturale.DataOperazione).ToShortDateString
            'row.Item("Reinnesco_Dose") = 0

            'Scadenza è solo per il reinnesco, per il rilievo non ha senso
            Select Case CInt(I_Operazione_Colturale.Lav_Cod)

                Case LAVCOD_REINNESCO_TRAPPOLE
                    'imposto la scadenza
                    Dim Giorni As Integer = (New AgronicaCoreMetaSchemaDAL.Trappole_R()).Trap_Dur_from_TrapCod(CInt(row.Item("TRAP_COD")), objParametri_Server)
                    Dim data_scadenza As Date
                    data_scadenza = DateAdd(DateInterval.Day, Giorni, I_Operazione_Colturale.DataOperazione)
                    row.Item("Scadenza") = data_scadenza.ToShortDateString
                Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                    'la scadenza è solo per il reinnesco, per il rilievo non ha senso
                    row.Item("Scadenza") = (I_Operazione_Colturale.DataOperazione).ToShortDateString
                Case Else
                    Throw New NotImplementedException
            End Select



            'row.Item("Inneschi_Attivi") = 0
            row.Item("Reinneschi") = I_Operazione_Colturale.Reinneschi(i).NumeroReinneschi

            'Giacenza  è solo per il reinnesco, per il rilievo non ha senso
            Select Case CInt(I_Operazione_Colturale.Lav_Cod)

                Case LAVCOD_REINNESCO_TRAPPOLE
                    If Not IsNothing(I_Operazione_Colturale.Magazzino) Then
                        Dim objGiacenze As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                        'l'ultimo parametro, id_agenda, indica l'id_agenda da non considerare, ma qui sono in scrittura, 
                        'non devo non considerare l'idagenda della trappola corrente, non avrebbe senso, metto 0 
                        Dim GiacenzaInneschi As Integer = objGiacenze.Verifica_Giacenze_Con_Magazzino_Esterno(I_Operazione_Colturale.Magazzino.Piva, _
                                                                    I_Operazione_Colturale.Magazzino.Sa_Cod, _
                                                                    I_Operazione_Colturale.Magazzino.Fabbricato_Cod, _
                                                                    CInt(198), _
                                                                    I_Operazione_Colturale.Reinneschi(i).Trappola.Avversita_ID, _
                                                                    0, _
                                                                    0, 0, AgronicaCoreDataProvider.CostantiPersonalizzate.LOTTO_NONDEFINITO, _
                                                                    0, 0, _
                                                                    AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, _
                                                                    I_Operazione_Colturale.DataOperazione, _
                                                                    I_Operazione_Colturale.Piva, _
                                                                    I_Operazione_Colturale.Sa_Cod, _
                                                                    I_Operazione_Colturale.ID_Agenda, _
                                                                    objParametri_Server)
                        row.Item("Giacenza") = GiacenzaInneschi
                    Else
                        row.Item("Giacenza") = 0
                    End If
                Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                    'per il rilievo avversità non serve il magazzino
                    row.Item("Giacenza") = 0
                Case Else
                    Throw New NotImplementedException
            End Select

            Dt.Rows.Add(row)

        Next

        'Nascondo alcune colonnee modifico intestazione se sono in rilievo avversita
        Select Case CInt(I_Operazione_Colturale.Lav_Cod)

            Case LAVCOD_REINNESCO_TRAPPOLE

            Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE

                'cambio intestazione di 'reinneschi ' in 'avversità rilevate'
                gridView.Columns(12).HeaderText = Resources.AgronicaAgenda_2010.NumeroAvversitàRilevate
                gridView.Caption = Resources.AgronicaAgenda_2010.IndicareIlNumeroDiAvversitàRilevateNelleTr
                'Nascondo colonne inneschi alla installazione e al reinnesco e giacenza,
                gridView.Columns(10).Visible = False
                gridView.Columns(11).Visible = False
                gridView.Columns(13).Visible = False
            Case Else
                Throw New NotImplementedException
        End Select

        SettaTabellaGridview(Dt, gridView, I_Operazione_Colturale.Lav_Cod)

    End Sub

    Private Sub SettaTabellaGridview(Dt As DataTable, gridView As GridView, ByVal Lav_Cod As Integer)

        ''Vettore di DataColumn
        Dim DtKeys(11) As String

        'Valorizzo le celle del vettore
        DtKeys(0) = "PIVA"
        'DtKeys(0) = "Rag_Soc"
        DtKeys(1) = "Sa_Cod"
        'DtKeys(0) = "Sa_nome"
        DtKeys(2) = "Appezza"
        DtKeys(3) = "ID_Reg"
        'DtKeys(0) = "App_nome"
        'DtKeys(0) = "Veg_Cod"
        'DtKeys(0) = "Veg_Des"
        'DtKeys(0) = "Cul_Cod"
        'DtKeys(0) = "Cul_Des"
        DtKeys(4) = "Trap_Num"
        DtKeys(5) = "Freatimetro"
        DtKeys(6) = "Ditta_Cod"
        DtKeys(7) = "TRAP_COD"
        'DtKeys(0) = "Trap_Des"
        DtKeys(8) = "Sigla_AV"
        DtKeys(9) = "Av_Cod"
        DtKeys(10) = "Uso"
        'DtKeys(0) = "Uso_Desc"
        'DtKeys(0) = "Validita_Inizio"
        'DtKeys(0) = "Dose"
        'DtKeys(0) = "Data_Reinnesco"
        'DtKeys(0) = "Reinnesco_Dose"
        'DtKeys(0) = "Scadenza"
        'DtKeys(0) = "Inneschi_Attivi"
        'DtKeys(0) = "Giacenza"
        DtKeys(11) = "Qta2"



        gridView.DataKeyNames = DtKeys

        gridView.DataSource = Dt
        gridView.DataBind()

    End Sub



    Public Function Genera_Lista_Reinneschi_Nella_OperazioneColturale_Da_TabellaTrappole_X_Reinnesco(ByRef gridView As GridView, _
                                                                                        ByRef I_Operazione_Colturale As AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.Aggiornamento_Trappole.Reinnesco_Trappole, _
                                                                                        ByVal DataOperazione As Date, _
                                                                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                                        ByRef messaggiErrore As AgronicaCoreUtility.Messages_Str, _
                                                                                        ByVal Giacenza_SI_NO As Global.System.Web.UI.WebControls.HiddenField, _
                                                                                        ByRef pagina As Reinnesco_Rilievi_Trappole, _
                                                                                        ByRef UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE As Boolean) As Boolean

        'pulisco l'operazione dagli inneschi
        I_Operazione_Colturale.Reinneschi.Clear()

        'dato che i nserisco piu reinneschi contemporaneamente e questi possono prelevare della stessa guiacenza devo controllare la coerenza rispetto alal somma delle
        'giacenze, per queste utilizzo una hashtable dove memorizzare inneschi controllate da sommare se si riferiscono 
        'alla stessa avversita
        Dim ReinneschiPerAvvesita As New Hashtable

        For i = 0 To gridView.Rows.Count - 1

            If (CType(gridView.Rows(i).FindControl("ChkSelezionaTrappola"), CheckBox).Checked = True) Then

                Dim Piva = CStr(gridView.DataKeys(i).Item("PIVA"))
                Dim Sa_Cod = CInt(gridView.DataKeys(i).Item("Sa_Cod"))
                '--------------------CONTROLLO COERENZA PIVA SACOD-------------
                'Dato che il reinnesco non supporta il multicentro, e ancore
                'occorre gestirlo anche direttammente nell'oggetto operazione,
                'verifico che tutte le trappole e quindi l'operazione sia riferita allo stesso centro 
                'Verifico anche piva per ulteriore controllo
                'Comunque il controllo viene fatto anche quando:
                ' si preme il pulsante per generare l'impianto (che ci sia un centro nella combo selezionato): ImageButton_MostraTrappole_Click
                ' quando si genera la lista reinneschi per l'oggetto operazione da salvare: Genera_Lista_Reinneschi_Nella_OperazioneColturale_Da_TabellaTrappole_X_Reinnesco
                'quando si salva l'operazione tramite IO utility OperazioneColturale_ToFrom_AgendaDB: si controlla che operazione.sa_cod non sia 0 e che destinazioni piva e sacod corrispondano a quelli dell'operazione
                If Piva <> I_Operazione_Colturale.Piva Then
                    messaggiErrore.add(Resources.AgronicaAgenda_2010.PivaOperazioneEPivaImpiantoNonCorrispondon)
                    Return False
                End If
                If Sa_Cod <> I_Operazione_Colturale.Sa_Cod Then
                    messaggiErrore.add(Resources.AgronicaAgenda_2010.CentroOperazioneECentroImpiantoNonCorrispo)
                    Return False
                End If
                '--------------------CONTROLLO FINE-------------

                'occhio, case sensitive i nomi degli item !!!

                Dim Appezza = CInt(gridView.DataKeys(i).Item("Appezza"))
                Dim ID_Reg = CInt(gridView.DataKeys(i).Item("ID_Reg"))
                Dim Qta2 = CDec(gridView.DataKeys(i).Item("Qta2"))
                Dim Impianto As New AgronicaCoreModello.Anagrafe.Impianto_Colturale(Piva, Sa_Cod, Appezza, ID_Reg, 0, AGRODATAINIZIO, AGRODATAFINE, Qta2)

                Dim ID As Integer = CInt(gridView.DataKeys(i).Item("Trap_Num"))
                Dim ID_Personalizzato As Integer = CInt(gridView.DataKeys(i).Item("Freatimetro"))

                Dim Prodotto_ID As Integer = CInt(gridView.DataKeys(i).Item("TRAP_COD"))
                Dim Ditta_ID As Integer = CInt(gridView.DataKeys(i).Item("Ditta_Cod"))
                Dim Avversita_ID As Integer = CInt(gridView.DataKeys(i).Item("Av_Cod"))
                Dim Avversita_Sigla As String = CStr(gridView.DataKeys(i).Item("Sigla_AV"))

                Dim Trappola As New AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.Installazione_Trappole.Con_Inneschi.Trappola(ID, ID_Personalizzato, Impianto)
                Trappola.Prodotto_ID = Prodotto_ID
                Trappola.Ditta_ID = Ditta_ID
                Trappola.Avversita_ID = Avversita_ID
                Trappola.Avversita_Sigla = Avversita_Sigla

                Dim NumeroInneschi As String = CType(gridView.Rows(i).FindControl("Txt_Reinnesco"), TextBox).Text
                If Not (IsNumeric(NumeroInneschi) AndAlso CInt(NumeroInneschi) >= 0) Then
                    messaggiErrore.add(Resources.AgronicaAgenda_2010.IlNumeroIdicatoNonÈValidoDeveEssereInteroE & ID)
                    Return False
                End If

                Select Case CInt(I_Operazione_Colturale.Lav_Cod)

                    Case LAVCOD_REINNESCO_TRAPPOLE

                        If Not (CInt(NumeroInneschi) > 0) Then
                            messaggiErrore.add(String.Format(Resources.AgronicaAgenda_2010.IlNumeroDeiReinneschiDeveEssereMaggioreDi0, ID))
                            Return False
                        End If

                        'Il magazzino è già stato letto dalla combo e impostato nel parametro dell'operazione
                        'prima di chiamare questa funzione
                        If Not IsNothing(I_Operazione_Colturale.Magazzino) Then

                            'gestire magazzino
                            'qui o nella IO salvataggio operazione
                            'controllo la giacenza tenendo conto anche degli inneschi controllati in precedenza e della stessa avversità
                            Dim numeroinneschidacontrollare As Integer = 0
                            If ReinneschiPerAvvesita.Contains(Avversita_ID) Then
                                numeroinneschidacontrollare = NumeroInneschi + CInt(ReinneschiPerAvvesita(Avversita_ID))
                                ReinneschiPerAvvesita.Item(Avversita_ID) = numeroinneschidacontrollare
                            Else
                                numeroinneschidacontrollare = NumeroInneschi
                                ReinneschiPerAvvesita.Add(Avversita_ID, numeroinneschidacontrollare)
                            End If
                            Dim GiacenzaInneschiAllaData As Integer
                            Dim GiacenzaInneschiAttuale As Integer
                            Dim messaggio As String = ""
                            Dim res As Boolean = New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R().controllaGiacenza_Trappole_Inneschi( _
                                                                                                    I_Operazione_Colturale.Magazzino.Piva, _
                                                                                                    I_Operazione_Colturale.Magazzino.Sa_Cod, _
                                                                                                    I_Operazione_Colturale.Magazzino.Fabbricato_Cod, _
                                                                                                    I_Operazione_Colturale.ID_Agenda, _
                                                                                                    0, _
                                                                                                    Avversita_ID, _
                                                                                                    0, _
                                                                                                    numeroinneschidacontrollare, _
                                                                                                    0, _
                                                                                                    GiacenzaInneschiAllaData, _
                                                                                                    0, _
                                                                                                    GiacenzaInneschiAttuale, _
                                                                                                    DataOperazione, _
                                                                                                    messaggio, _
                                                                                                    objParametri_Server)

                            If Not res Then
                                'messaggiErrore.add(messaggio)
                                'Return False

                                '--------------BLOCCO SALVATAGGIO SE_SUPERA_GIACENZE-------------------
                                If UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE AndAlso I_Operazione_Colturale.Lav_Cod = LAVCOD_REINNESCO_TRAPPOLE Then

                                    Dim MErrore As String
                                    MErrore = "In base alle impostazioni utente NON è possibile usare un prodotto con giacenza non sufficiente! " & vbCr & messaggio
                                    Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & MErrore, pagina.Page, , CType(pagina.Master, Operazione).Property_UpdatePanelPerScript)
                                    Return False

                                End If
                                '---------------------------------------------------------------------

                                'controllo se ho acconsentito precedenrtemente al salvataggio senza giacenze
                                If Giacenza_SI_NO.Value = "0" Then

                                    'se non ho acconsentito genere agrosino che mi rilancera il salvataggio via jscript
                                    messaggio = messaggio & vbCr & Resources.AgronicaAgenda_2010.BrBprocediignoragiacTrapp
                                    'AgroSiNo
                                    'Messaggi.AgroSiNo(messaggio, "Salva", Page, , UpdatePanelGridImpianti)
                                    'impedisco di procedere per ora
                                    ' VAnni: 28/2/2020: Non usare il resx sulla stringa "Giacenze"
                                    Messaggi.AgroSiNo(messaggio & vbCr & Resources.AgronicaAgenda_2010.BrBIAltrimentiInserireUnValoreValidoOTogli, _
                                                       "Giacenze", pagina.Page, , CType(pagina.Master, Operazione).Property_UpdatePanelPerScript)

                                    Return False

                                Else
                                    'se ho già cliccato  ok vado avanti

                                End If

                            End If

                        End If
                    Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                        'per il rilievo avversità non serve il magazzino
                    Case Else
                        Throw New NotImplementedException
                End Select



                Dim Reinnesco As New AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.Aggiornamento_Trappole.Reinnesco(Trappola, CInt(NumeroInneschi))
                I_Operazione_Colturale.Reinneschi.Add(Reinnesco)


            End If

        Next

        'controllo
        If I_Operazione_Colturale.Reinneschi.Count < 1 Then
            messaggiErrore.add(Resources.AgronicaAgenda_2010.DeveEssereSelezionatoAlmenoUnReinnescoPerS)
            Return False
        End If

        Return True
    End Function

    Public Function Aggiorna_Numero_Reinneschi_Nella_OperazioneColturale_Da_Tabella_Reinneschi(ByRef gridView As GridView, _
                                                                                        ByRef I_Operazione_Colturale As AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.Aggiornamento_Trappole.Reinnesco_Trappole, _
                                                                                        ByVal DataOperazione As Date, _
                                                                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                                        ByRef messaggiErrore As AgronicaCoreUtility.Messages_Str, _
                                                                                        ByVal Giacenza_SI_NO As Global.System.Web.UI.WebControls.HiddenField, _
                                                                                        ByRef pagina As Reinnesco_Rilievi_Trappole) As Boolean

        'dato che i nserisco piu reinneschi contemporaneamente e questi possono prelevare della stessa guiacenza devo controllare la coerenza rispetto alal somma delle
        'giacenze, per queste utilizzo una hashtable dove memorizzare inneschi controllate da sommare se si riferiscono 
        'alla stessa avversita
        Dim ReinneschiPerAvvesita As New Hashtable

        For i = 0 To gridView.Rows.Count - 1



            Dim Piva = CStr(gridView.DataKeys(i).Item("PIVA"))
            Dim Sa_Cod = CInt(gridView.DataKeys(i).Item("Sa_Cod"))
            '--------------------CONTROLLO COERENZA PIVA SACOD-------------
            'Dato che il reinnesco non supporta il multicentro, e ancore
            'occorre gestirlo anche direttammente nell'oggetto operazione,
            'verifico che tutte le trappole e quindi l'operazione sia riferita allo stesso centro 
            'Verifico anche piva per ulteriore controllo
            'Comunque il controllo viene fatto anche quando:
            ' si preme il pulsante per generare l'impianto (che ci sia un centro nella combo selezionato): ImageButton_MostraTrappole_Click
            ' quando si genera la lista reinneschi per l'oggetto operazione da salvare: Genera_Lista_Reinneschi_Nella_OperazioneColturale_Da_TabellaTrappole_X_Reinnesco
            'quando si salva l'operazione tramite IO utility OperazioneColturale_ToFrom_AgendaDB: si controlla che operazione.sa_cod non sia 0 e che destinazioni piva e sacod corrispondano a quelli dell'operazione
            If Piva <> I_Operazione_Colturale.Piva Then
                messaggiErrore.add(Resources.AgronicaAgenda_2010.PivaOperazioneEPivaImpiantoNonCorrispondon)
                Return False
            End If
            If Sa_Cod <> I_Operazione_Colturale.Sa_Cod Then
                messaggiErrore.add(Resources.AgronicaAgenda_2010.CentroOperazioneECentroImpiantoNonCorrispo)
                Return False
            End If
            '--------------------CONTROLLO FINE-------------

            'occhio, case sensitive i nomi degli item !!!

            Dim Appezza = CInt(gridView.DataKeys(i).Item("Appezza"))
            Dim ID_Reg = CInt(gridView.DataKeys(i).Item("ID_Reg"))
            Dim Qta2 As Decimal = CDec(gridView.DataKeys(i).Item("Qta2"))
            Dim Impianto As New AgronicaCoreModello.Anagrafe.Impianto_Colturale(Piva, Sa_Cod, Appezza, ID_Reg, 0, AGRODATAINIZIO, AGRODATAFINE, Qta2)

            Dim ID As Integer = CInt(gridView.DataKeys(i).Item("Trap_Num"))
            Dim ID_Personalizzato As Integer = CInt(gridView.DataKeys(i).Item("Freatimetro"))

            Dim Prodotto_ID As Integer = CInt(gridView.DataKeys(i).Item("TRAP_COD"))
            Dim Ditta_ID As Integer = CInt(gridView.DataKeys(i).Item("Ditta_Cod"))
            Dim Avversita_ID As Integer = CInt(gridView.DataKeys(i).Item("Av_Cod"))
            Dim Avversita_Sigla As String = CStr(gridView.DataKeys(i).Item("Sigla_AV"))
            Dim Trappola As New AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.Installazione_Trappole.Con_Inneschi.Trappola(ID, ID_Personalizzato, Impianto)
            Trappola.Prodotto_ID = Prodotto_ID
            Trappola.Ditta_ID = Ditta_ID
            Trappola.Avversita_ID = Avversita_ID
            Trappola.Avversita_Sigla = Avversita_Sigla

            Dim NumeroInneschi As String = CType(gridView.Rows(i).FindControl("Txt_Reinnesco"), TextBox).Text
            If Not (IsNumeric(NumeroInneschi) AndAlso CInt(NumeroInneschi) >= 0) Then
                messaggiErrore.add(String.Format(Resources.AgronicaAgenda_2010.IlNumeroIdicatoNonÈValidoDeveEssereInteroE, ID))
                Return False
            End If

            Aggiorna_Numero_Reinneschi(I_Operazione_Colturale, Trappola, CInt(NumeroInneschi))

            Select Case CInt(I_Operazione_Colturale.Lav_Cod)

                Case LAVCOD_REINNESCO_TRAPPOLE

                    If Not (CInt(NumeroInneschi) > 0) Then
                        messaggiErrore.add(String.Format(Resources.AgronicaAgenda_2010.IlNumeroDeiReinneschiDeveEssereMaggioreDi0, ID))
                        Return False
                    End If

                    If Not IsNothing(I_Operazione_Colturale.Magazzino) Then

                        'gestire magazzino
                        'qui o nella IO salvataggio operazione
                        'controllo la giacenza tenendo conto anche degli inneschi controllati in precedenza e della stessa avversità
                        Dim numeroinneschidacontrollare As Integer = 0
                        If ReinneschiPerAvvesita.Contains(Avversita_ID) Then
                            numeroinneschidacontrollare = NumeroInneschi + CInt(ReinneschiPerAvvesita(Avversita_ID))
                            ReinneschiPerAvvesita.Item(Avversita_ID) = numeroinneschidacontrollare
                        Else
                            numeroinneschidacontrollare = NumeroInneschi
                            ReinneschiPerAvvesita.Add(Avversita_ID, numeroinneschidacontrollare)
                        End If
                        Dim GiacenzaInneschiAllaData As Integer
                        Dim GiacenzaInneschiAttuale As Integer
                        Dim messaggio As String = ""
                        Dim res As Boolean = New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R().controllaGiacenza_Trappole_Inneschi( _
                                                                                                I_Operazione_Colturale.Magazzino.Piva, _
                                                                                                I_Operazione_Colturale.Magazzino.Sa_Cod, _
                                                                                                I_Operazione_Colturale.Magazzino.Fabbricato_Cod, _
                                                                                                I_Operazione_Colturale.ID_Agenda, _
                                                                                                0, _
                                                                                                Avversita_ID, _
                                                                                                0, _
                                                                                                numeroinneschidacontrollare, _
                                                                                                0, _
                                                                                                GiacenzaInneschiAllaData, _
                                                                                                0, _
                                                                                                GiacenzaInneschiAttuale, _
                                                                                                DataOperazione, _
                                                                                                messaggio, _
                                                                                                objParametri_Server)

                        If Not res Then
                            'messaggiErrore.add(messaggio)
                            'Return False

                            'controllo se ho acconsentito precedenrtemente al salvataggio senza giacenze
                            If Giacenza_SI_NO.Value = "0" Then

                                'se non ho acconsentito genere agrosino che mi rilancera il salvataggio via jscript
                                messaggio = messaggio & vbCr & Resources.AgronicaAgenda_2010.BrBIProcedereUgualmenteSenzaGestireLeGiace
                                'AgroSiNo
                                'Messaggi.AgroSiNo(messaggio, "Salva", Page, , UpdatePanelGridImpianti)
                                'impedisco di procedere per ora
                                ' VAnni: 28/2/2020: Non usare il resx sulla stringa "Giacenze"
                                Messaggi.AgroSiNo(messaggio & vbCr & Resources.AgronicaAgenda_2010.BrBIAltrimentiInserireUnValoreValidoOTogli, "Giacenze", pagina.Page, , pagina.updatepanelTrappoleMostra)

                                Return False

                            Else
                                'se ho già cliccato  ok vado avanti

                            End If

                        End If

                    End If
                Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
                    'per il rilievo avversità non serve il magazzino
                Case Else
                    Throw New NotImplementedException
            End Select



        Next


        Return True
    End Function

    Private Function Aggiorna_Numero_Reinneschi(I_Operazione_Colturale As AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.Aggiornamento_Trappole.Reinnesco_Trappole, Trappola As AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.Installazione_Trappole.Con_Inneschi.Trappola, NumeroInneschi As Integer) As Boolean
        For i = 0 To I_Operazione_Colturale.Reinneschi.Count - 1
            If I_Operazione_Colturale.Reinneschi(i).Trappola.ID = Trappola.ID AndAlso _
               I_Operazione_Colturale.Reinneschi(i).Trappola.ID_Personalizzato = Trappola.ID_Personalizzato AndAlso _
               I_Operazione_Colturale.Reinneschi(i).Trappola.Avversita_Sigla = Trappola.Avversita_Sigla _
                Then

                I_Operazione_Colturale.Reinneschi(i).NumeroReinneschi = NumeroInneschi

            End If
        Next
        Return True
    End Function




End Module

