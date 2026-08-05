


Imports System.Drawing
Imports System.Data
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports System.Configuration.ConfigurationManager
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class GST_Interferenze_Init
    Inherits System.Web.UI.Page


    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Dim Tabella_configurazione_Colori As Table


    '####################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If Session.IsNewSession Then
            Response.Redirect("../GST_Autenticazione/Autenticazione.aspx")
        End If

        Me.Master.ImpostaVisibilitaPulsantiMaster(TipiEnumerativiSementieri.enum_Pannelli.INTERFERENZE)

        '----- Dimensiono le variabili
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina



        Dim UtenteAbilitato As Boolean = False

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                    objParametri_Utenti.UtenteUsername,
                                    5,
                                    TipiEnumerativi.enum_Security_Attivita.Interferenze_Visualizzazione_Ridotta,
                                    TipiEnumerativi.enum_Security_Operazione.Lettura,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)


        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        If UtenteAbilitato = False Then

            UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                   objParametri_Utenti.UtenteUsername,
                                   5,
                                   TipiEnumerativi.enum_Security_Attivita.Interferenze_Visualizzazione_Estesa,
                                   TipiEnumerativi.enum_Security_Operazione.Lettura,
                                   Date.Now,
                                   "",
                                   objParametri_Utenti)
        End If

        '----- Se l'utente non ha il permesso per visualizzare la pagina ... 

        If (UtenteAbilitato = False) Then
            Response.Redirect("../GST_Menu/Menu.aspx")
        End If

        '##############################################################
        '#####  Verifico se sono in Post-Back  ########################
        '##############################################################

        If Not Page.IsPostBack Then
            'output.Write("Page has just been loaded")
            'If Not IsNothing(Request.QueryString("bietola")) Then
            '    ViewState("bietola") = Request.QueryString("bietola")
            'End If
        Else
            'output.Write("Postback has occured")
            Exit Sub
        End If

        '##############################################################
        '#####  Inizializzo la Pagina  ################################
        '##############################################################

        Call Imposta_Pannelli(enum_Pannelli.FILTRO)

        Lbl_InterferenzeTipo.Text = Lbl_InterferenzeTipo.Text.Replace("###", Session("ASG_Utente_Descrizione"))

        'Lbl_NumeroDistanze.Text = "N. distanze calcolate :  <b>" & Session("NumeroDistanzeCalcolate") & "</b>"

        '----- Estremi del UNID Impianto utilizzabile

        Dim NumeroImpianti As Integer
        Dim DT_Impianti As New DataTable

        DT_Impianti = Session("SuperDT_Impianti")

        NumeroImpianti = DT_Impianti.Rows.Count

        'Lbl_Estremi_UNID_A.Text = "[da 0  a " & (NumeroImpianti - 1) & "]"
        'Lbl_Estremi_UNID_B.Text = "[da 0  a " & (NumeroImpianti - 1) & "]"

        DT_Impianti.Dispose()


        '----- Se il numero di impianti e' <=20 allora consento di vedere le non-interferenze

        'If NumeroImpianti > 25 Then
        '    Chk_Flag_NonInterferenze.Checked = False
        '    Chk_Flag_NonInterferenze.Enabled = False
        'Else
        '    Chk_Flag_NonInterferenze.Checked = False
        '    Chk_Flag_NonInterferenze.Enabled = True
        'End If

        '----- Solo gli amministratori posso vedere le interferenze altrui ...

        UtenteAbilitato = False

        UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                   objParametri_Utenti.UtenteUsername,
                                   5,
                                   TipiEnumerativi.enum_Security_Attivita.Interferenze_Visualizzazione_Estesa,
                                   TipiEnumerativi.enum_Security_Operazione.Lettura,
                                   Date.Now,
                                   "",
                                   objParametri_Utenti)

        If (UtenteAbilitato = False) Then

            'Utente NORMALE
            'Visualizzo solo (interne alla propria) e (tra la propria e le altre)

            'Interne alla propria azienda -------------------   SI
            Rbl_FlagInterferenze_Interne.Visible = True
            Rbl_FlagInterferenze_Interne.Checked = False
            Lbl_AA.Visible = True

            'Tra la propria azienda e le altre --------------   SI
            Rbl_FlagInterferenze_Esterne.Visible = True
            Rbl_FlagInterferenze_Esterne.Checked = True
            Lbl_AX.Visible = True

            'Interne alle altre aziende ---------------------   NO
            'Rbl_FlagInterferenze_AA.Visible = False
            'Rbl_FlagInterferenze_AA.Checked = False
            'Lbl_XX.Visible = False

            'Tra le altre aziende ---------------------------   NO
            Rbl_FlagInterferenze_AB.Visible = False
            Rbl_FlagInterferenze_AB.Checked = False
            Lbl_XY.Visible = False

        Else

            'Utente AMMINISTRATORE
            'Visualizzo solo (tra le altre aziende)

            'Interne alla propria azienda -------------------   NO
            Rbl_FlagInterferenze_Interne.Visible = False
            Rbl_FlagInterferenze_Interne.Checked = False
            Lbl_AA.Visible = False

            'Tra la propria azienda e le altre --------------   NO
            Rbl_FlagInterferenze_Esterne.Visible = False
            Rbl_FlagInterferenze_Esterne.Checked = False
            Lbl_AX.Visible = False

            'Interne alle altre aziende ---------------------   NO
            'Rbl_FlagInterferenze_AA.Visible = False
            'Rbl_FlagInterferenze_AA.Checked = False
            'Lbl_XX.Visible = False

            'Tra le altre aziende ---------------------------   SI
            Rbl_FlagInterferenze_AB.Visible = True
            Rbl_FlagInterferenze_AB.Checked = True
            Lbl_XY.Visible = True

        End If




    End Sub





    '##################################################################################################
    Private Enum enum_Pannelli
        FILTRO = 1
        RISULTATO = 2
    End Enum


    '##################################################################################################
    Private Sub Imposta_Pannelli(ByVal Pannello As enum_Pannelli)

        'Pannello_Filtro.Visible = False
        Pannello_Risultato.Visible = False

        'Pannello_BTN_Filtro.BackColor = Color.WhiteSmoke
        'Pannello_BTN_Risultato.BackColor = Color.WhiteSmoke

        'Lbl_BTN_Filtro.ForeColor = Color.Blue
        'Lbl_BTN_Risultato.ForeColor = Color.Blue
        'Lbl_BTN_Stampa.ForeColor = Color.Blue

        'Pannello_Filtro.Height = Unit.Pixel(570)
        'Pannello_Filtro.Width = Unit.Pixel(958)
        'Pannello_Filtro.Style.Item("Top") = "82px"
        'Pannello_Filtro.Style.Item("Left") = "16px"

        'Pannello_Risultato.Height = Unit.Pixel(570)
        'Pannello_Risultato.Width = Unit.Pixel(958)
        'Pannello_Risultato.Style.Item("Top") = "82px"
        'Pannello_Risultato.Style.Item("Left") = "16px"

        'ImgBtn_Filtro.Enabled = True
        'ImgBtn_Risultato.Enabled = True
        'ImgBtn_Stampa.Enabled = True

        'Pannello_BTN_Filtro.Visible = True
        'Pannello_BTN_Risultato.Visible = True
        'Pannello_BTN_Stampa.Visible = True

        'Img_Freccia_Risultato.Visible = True
        'Img_Freccia_Stampa.Visible = True

        'Img_Freccia_Risultato.ImageUrl = "../App_Immagini/icone24/Freccia24VerdeDX.ico"



    End Sub



    ''#########################################################################################
    'Protected Sub ImgBtnAnnulla_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnAnnulla.Click

    '    Response.Redirect("../GST_Menu/Menu.aspx")

    'End Sub




    '#########################################################################################

    Protected Sub ImgBtn_Filtro_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        Response.Redirect("../GST_Filtro/GST_Filtro_Impianti.aspx")
    End Sub

    Protected Sub ImgBtn_Risultato_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)

        Response.Redirect("../GST_Filtro/GST_Filtro_Impianti.aspx")

        Exit Sub

        Dim DT_Impianti As New DataTable
        Dim DT_Interferenze As New DataTable
        Dim MatriceDistanze(,) As Double
        Dim DT_Distanze As New DataTable
        Dim DT_DistanzeBietola As New DataTable

        Dim NumeroInterferenze As Integer
        Dim MoltiplicatoreDistanze As Double
        Dim Flag_Interferenze_AA As Boolean
        Dim Flag_Interferenze_AX As Boolean
        Dim Flag_Interferenze_XX As Boolean
        Dim Flag_Interferenze_XY As Boolean
        Dim Flag_VisualizzaNonInterferenze As Boolean

        Dim DT_Risultato As New DataTable
        Dim i As Integer

        '--------------------------------------------------------------
        '--- Recupero le informazioni sulla pagina ...
        '--------------------------------------------------------------

        MoltiplicatoreDistanze = 1

        If Me.Txt_Moltiplicatore.Text = "" Then
            AgroMsgBox("Il MOLTIPLICATORE deve essere un valore numerico", Page)
            Exit Sub
        End If

        If Not IsNumeric(Me.Txt_Moltiplicatore.Text) Then
            AgroMsgBox("Il MOLTIPLICATORE deve essere un valore numerico", Page)
            Exit Sub
        End If

        Me.Txt_Moltiplicatore.Text = Me.Txt_Moltiplicatore.Text.Replace(".", ",")
        MoltiplicatoreDistanze = CDbl(Me.Txt_Moltiplicatore.Text)

        If MoltiplicatoreDistanze <= 0 Then
            AgroMsgBox("Il MOLTIPLICATORE deve essere un valore numerico positivo e non nullo", Page)
            Exit Sub
        End If

        '-----

        'Chk_FlagInterferenze_Interne

        If Rbl_FlagInterferenze_Interne.Checked = True Then
            Flag_Interferenze_AA = True
        Else
            Flag_Interferenze_AA = False
        End If

        If Rbl_FlagInterferenze_Esterne.Checked = True Then
            Flag_Interferenze_AX = True
        Else
            Flag_Interferenze_AX = False
        End If

        'If Rbl_FlagInterferenze_AA.Checked = True Then
        '    Flag_Interferenze_XX = True
        'Else
        '    Flag_Interferenze_XX = False
        'End If

        If Rbl_FlagInterferenze_AB.Checked = True Then
            Flag_Interferenze_XY = True
        Else
            Flag_Interferenze_XY = False
        End If

        '-----

        'If Chk_Flag_NonInterferenze.Checked = True Then
        '    Flag_VisualizzaNonInterferenze = True
        'Else
        '    Flag_VisualizzaNonInterferenze = False
        'End If

        '--------------------------------------------------------------
        '--- Recupero le informazioni dalla sessione ...
        '--------------------------------------------------------------

        DT_Impianti = Session("SuperDT_Impianti")
        MatriceDistanze = Session("MatriceDistanze")

        '--------------------------------------------------------------
        '--- Carico i datatable ausiliari
        '--------------------------------------------------------------

        Dim leggi_SpecieDistanze As New AgronicaCoreSementieriDAL.Mappatura_Specie_Distanze_R
        DT_Distanze = leggi_SpecieDistanze.Leggi("", objParametri_Server)
        'DT_Distanze = AD_SpecieDistanze_Leggi(objServer, objSession, objPage)

        Dim leggi_SpecieDistanzeBietola As New AgronicaCoreSementieriDAL.Mappatura_Specie_Distanze_Bietola_R
        DT_DistanzeBietola = leggi_SpecieDistanzeBietola.Leggi("", objParametri_Server)
        'DT_Bietola = AD_SpecieDistanzeBietola_Leggi(objServer, objSession, objPage)


        '--------------------------------------------------------------
        '--- Analizzo le interferenze
        '--------------------------------------------------------------

        'Verifico quale tipo di ricerca effettuare ...
        'Se viene richiesta una ricerca su un sottoinsieme, 
        'duplico il datatable e la matrice poi li ripulisco degli elementi non necessari

        '
        '
        '
        'TODO sostituire con tipo Enum


        Dim UtenteAmministratore As Boolean = False

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        UtenteAmministratore = objPermessi.Controlla_Permessi_Utente(
                                    objParametri_Utenti.UtenteUsername,
                                    5,
                                    TipiEnumerativi.enum_Security_Attivita.Interferenze_Visualizzazione_Estesa,
                                    TipiEnumerativi.enum_Security_Operazione.Lettura,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)

        'Per ora gestisco solo il caso principale ...
        Call AgronicaCoreSementieriBIZ.Interferenze.Analizza_Interferenze(
                                DT_Impianti,
                                DT_Distanze,
                                DT_DistanzeBietola,
                                MatriceDistanze,
                                DT_Interferenze,
                                NumeroInterferenze,
                                MoltiplicatoreDistanze,
                                Flag_Interferenze_AA,
                                Flag_Interferenze_AX,
                                Flag_Interferenze_XX,
                                Flag_Interferenze_XY,
                                Flag_VisualizzaNonInterferenze,
                                UtenteAmministratore,
                                Session,
                                objParametri_Server)

        ''--------------------------------------------------------------
        ''--- Carico la griglia dei risultati
        ''--------------------------------------------------------------

        ''Preparo il datatable dei risultati
        'Datatable_Risultato_Prepara(DT_Risultato)

        ''Riempio il datatable
        'Datatable_Risultato_Riempi( _
        '                        DT_Impianti, _
        '                        DT_Interferenze, _
        '                        DT_Risultato, _
        '                        MoltiplicatoreDistanze)

        'Session("SuperDT_Risultati") = DT_Risultato

        'Session("SuperDT_Interferenze") = DT_Interferenze

        'Rigenera_Griglia_Risultati("")

        ''--------------------------------------------------------------
        ''--- Numero di Interferenze (o altro)
        ''--------------------------------------------------------------

        'Dim NumeroRighe As Integer
        'NumeroRighe = DT_Risultato.Rows.Count
        'Lbl_NumeroInterferenze.Text = "N. Righe :  <b>" & NumeroRighe & "</b>"

        ''--------------------------------------------------------------
        ''--- Imposto il pannello corretto
        ''--------------------------------------------------------------

        'CaricaCombo_Ordinamento(Cmb_Ordinamento)

        'Call Imposta_Pannelli(enum_Pannelli.RISULTATO)



    End Sub

    Private Sub AgroMsgBox(v As String, page As Page)
        Throw New NotImplementedException()
    End Sub



    'Protected Sub ImgBtn_Filtro_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Filtro.Click

    '    'Imposto il pannello corretto
    '    Call Imposta_Pannelli(enum_Pannelli.FILTRO)

    'End Sub




    '#########################################################################################
    Protected Sub ImgBtn_Stampa_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)

        Dim dt As New DataTable("Interferenze")
        dt.Columns.Add("ElementoGrafico_Cod_1")
        dt.Columns.Add("ElementoGrafico_Cod_2")
        dt.Columns.Add("Impianto_A")
        dt.Columns.Add("Impianto_B")
        dt.Columns.Add("Descrizione")
        dt.Columns.Add("Distanza")
        dt.Columns.Add("Distanza_di_legge")
        dt.Columns.Add("Distanza_di_legge_moltiplicata")

        For i = 0 To GridViewInterferenze.Rows.Count - 1
            If CType(GridViewInterferenze.Rows(i).Cells(0).Controls(1), CheckBox).Checked = True Then
                Dim dr As DataRow = dt.NewRow
                dr.Item("ElementoGrafico_Cod_1") = GridViewInterferenze.DataKeys(i).Item("ElementoGrafico_Cod_1")
                dr.Item("ElementoGrafico_Cod_2") = GridViewInterferenze.DataKeys(i).Item("ElementoGrafico_Cod_2")
                dr.Item("Impianto_A") = GridViewInterferenze.Rows(i).Cells(1).Text
                dr.Item("Impianto_B") = GridViewInterferenze.Rows(i).Cells(2).Text
                dr.Item("Descrizione") = GridViewInterferenze.Rows(i).Cells(3).Text
                dr.Item("Distanza") = GridViewInterferenze.Rows(i).Cells(4).Text
                dr.Item("Distanza_di_legge") = GridViewInterferenze.Rows(i).Cells(5).Text
                dr.Item("Distanza_di_legge_moltiplicata") = GridViewInterferenze.Rows(i).Cells(6).Text
                dt.Rows.Add(dr)
            End If
        Next

        Session("SuperDT_Interferenze") = dt

        Response.Redirect("../GST_Stampe/GST_Stampe_Menu.aspx")

    End Sub


    Protected Sub btn_Elabora_Click(sender As Object, e As EventArgs) Handles btn_Elabora.Click

        CaricaCombo_Ordinamento(Cmb_Ordinamento)

        'Vettore di DataColumn
        Dim DtKeys(1) As String

        'Valorizzo le celle del vettore
        DtKeys(0) = "ElementoGrafico_Cod_1"
        DtKeys(1) = "ElementoGrafico_Cod_2"

        GridViewInterferenze.DataKeyNames = DtKeys

        Dim DT_Impianti As DataTable = Session("SuperDT_Impianti")

        Dim Impianti_Selezionati = DT_Impianti.Select("Selezionato = 1")

        Dim flagAggiuntivo As Boolean = Rbl_FlagInterferenze_TUTTI_TUTTI.Checked = False
        Dim SoloImpresa As Boolean = Rbl_FlagInterferenze_Interne.Checked = True

        Dim MiaPivaReferente As String = Session("Codice_Fiscale_Tecnico")
        MiaPivaReferente = Session("Codice_Fiscale_Tecnico_OK")

        Dim LIST_INT As New List(Of Integer)
        Dim LIST_EST As New List(Of Integer)

        For Each dr In Impianti_Selezionati

            If dr.Item("Referente_Piva") = MiaPivaReferente Then
                LIST_INT.Add(dr.Item("UNID_Impianto"))
            Else
                LIST_EST.Add(dr.Item("UNID_Impianto"))
            End If
        Next

        If Rbl_FlagInterferenze_Interne.Checked = True Then
            'Interferenze INTERNE della PROPRIA Azienda
            LIST_EST.Clear()
            LIST_EST.AddRange(LIST_INT)

        ElseIf Rbl_FlagInterferenze_Esterne.Checked = True Then
            'Interferenze tra la PROPRIA Azienda e le ALTRE.

        ElseIf Rbl_FlagInterferenze_AB.Checked = True Then
            'Interferenze TRA le Aziende.
            LIST_INT.AddRange(LIST_EST)
            LIST_EST.Clear()
            LIST_EST.AddRange(LIST_INT)

        ElseIf Rbl_FlagInterferenze_TUTTI_TUTTI.Checked = True Then
            'Tutte le Interferenze
            LIST_INT.AddRange(LIST_EST)
            LIST_EST.Clear()
            LIST_EST.AddRange(LIST_INT)

        End If

        'identifico la distanza massima tra gli appezzamenti
        Dim list_distinct_ID_Specie_sementieri As New List(Of Integer)
        For Each dr In DT_Impianti.DefaultView.ToTable(True, "ID_Specie_sementieri").Rows
            If Not IsDBNull(dr.Item("ID_Specie_sementieri")) Then
                list_distinct_ID_Specie_sementieri.Add(dr.Item("ID_Specie_sementieri"))
            End If
        Next

        Dim objInterferenze As New AgronicaCoreSementieriDAL.VerificaInterferenza(String.Join(", ", list_distinct_ID_Specie_sementieri), objParametri_Server)

        Dim DistanzaMassima As Double = objInterferenze.DistanzaMassima()

        Dim Moltiplicatore As Double = Txt_Moltiplicatore.Text.ToString.Replace(".", ",")
        Dim DT_Distanze As DataTable
        Dim objTrovaDistanze_R As New AgronicaCoreSementieriDAL.TrovaDistanze_R
        DT_Distanze = objTrovaDistanze_R.Leggi(DistanzaMassima * Moltiplicatore, String.Join(", ", LIST_INT), String.Join(", ", LIST_EST), flagAggiuntivo, SoloImpresa, objParametri_Server)

        DT_Distanze.Columns.Add("Impianto_A", Type.GetType("System.String"))
        DT_Distanze.Columns.Add("ID_A", Type.GetType("System.Int32"))
        DT_Distanze.Columns.Add("Referente_A", Type.GetType("System.String"))
        DT_Distanze.Columns.Add("AziendaA", Type.GetType("System.String"))

        DT_Distanze.Columns.Add("Specie", Type.GetType("System.String"))

        DT_Distanze.Columns.Add("Impianto_B", Type.GetType("System.String"))
        DT_Distanze.Columns.Add("ID_B", Type.GetType("System.Int32"))
        DT_Distanze.Columns.Add("Referente_B", Type.GetType("System.String"))
        DT_Distanze.Columns.Add("AziendaB", Type.GetType("System.String"))

        DT_Distanze.Columns.Add("Descrizione", Type.GetType("System.String"))

        DT_Distanze.Columns.Add("Distanza_di_legge", Type.GetType("System.Double"))
        DT_Distanze.Columns.Add("Distanza_di_legge_moltiplicata", Type.GetType("System.Double"))

        DT_Distanze.Columns.Add("Colore", Type.GetType("System.String"))

        DT_Distanze.Columns.Add("NomeScientifico", Type.GetType("System.String"))
        DT_Distanze.Columns.Add("NomeComune", Type.GetType("System.String"))
        DT_Distanze.Columns.Add("Regione", Type.GetType("System.String"))

        'identificate i possibili conflitti - scorro tutto e per ogni record controllo 
        Dim DT_Distanze_Row As DataRow

        'Dim inter As interferenza
        Dim idx As Integer = 0

        While idx < DT_Distanze.Rows.Count

            DT_Distanze_Row = DT_Distanze.Rows(idx)

            objInterferenze.Set_Specie_A(DT_Distanze_Row.Item("ID_Specie_A"))
            objInterferenze.Set_Sottospecie_A(DT_Distanze_Row.Item("ID_SottoSpecie_A"))
            objInterferenze.Set_Gruppo_A(DT_Distanze_Row.Item("ID_Gruppo_A"))
            objInterferenze.Set_Genotipo_A(DT_Distanze_Row.Item("ID_Genotipo_A"))
            objInterferenze.Set_Specie_B(DT_Distanze_Row.Item("ID_Specie_B"))
            objInterferenze.Set_Sottospecie_B(DT_Distanze_Row.Item("ID_SottoSpecie_B"))
            objInterferenze.Set_Gruppo_B(DT_Distanze_Row.Item("ID_Gruppo_B"))
            objInterferenze.Set_Genotipo_B(DT_Distanze_Row.Item("ID_Genotipo_B"))

            objInterferenze.Verifica(DT_Distanze_Row.Item("Distanza"), Moltiplicatore)

            If objInterferenze.TipoInterferenza = 0 Then

                DT_Distanze.Rows.RemoveAt(idx)

            Else

                Dim DR_Impianti() As DataRow

                DR_Impianti = DT_Impianti.Select("UNID_Impianto = " & DT_Distanze_Row.Item("ElementoGrafico_Cod_1"))
                DT_Distanze_Row.Item("Impianto_A") = generaTestoImpiantoGriglia(DR_Impianti(0))
                DT_Distanze_Row.Item("ID_A") = DR_Impianti(0).Item("UNID_Impianto")
                DT_Distanze_Row.Item("Referente_A") = DR_Impianti(0).Item("Referente_RagSoc")
                DT_Distanze_Row.Item("AziendaA") = DR_Impianti(0).Item("RagioneSociale")

                DT_Distanze_Row.Item("Specie") = DR_Impianti(0).Item("Veg_Des")

                DR_Impianti = DT_Impianti.Select("UNID_Impianto = " & DT_Distanze_Row.Item("ElementoGrafico_Cod_2"))
                DT_Distanze_Row.Item("Impianto_B") = generaTestoImpiantoGriglia(DR_Impianti(0))
                DT_Distanze_Row.Item("ID_B") = DR_Impianti(0).Item("UNID_Impianto")
                DT_Distanze_Row.Item("Referente_B") = DR_Impianti(0).Item("Referente_RagSoc")
                DT_Distanze_Row.Item("AziendaB") = DR_Impianti(0).Item("RagioneSociale")

                DT_Distanze_Row.Item("Descrizione") = objInterferenze.Motivazione_Des  'inter.Motivazione_Des

                DT_Distanze_Row.Item("NomeScientifico") = DR_Impianti(0).Item("NomeScientifico")
                DT_Distanze_Row.Item("NomeComune") = DR_Impianti(0).Item("Veg_Des")
                DT_Distanze_Row.Item("Regione") = DR_Impianti(0).Item("Regione_Des")

                Dim distanza As Double = Math.Round(DT_Distanze_Row.Item("Distanza"), 2)
                DT_Distanze_Row.Item("Distanza") = distanza
                DT_Distanze_Row.Item("Distanza_di_legge") = objInterferenze.DistanzaDiLegge        'inter.Distanza_di_legge
                DT_Distanze_Row.Item("Distanza_di_legge_moltiplicata") = objInterferenze.DistanzaDiLegge * Moltiplicatore   'inter.Distanza_di_legge * Moltiplicatore

                'Dim Percentuale As Integer = CInt((distanza / (inter.Distanza_di_legge * Moltiplicatore)) * 100)
                Dim Percentuale As Integer = CInt((distanza / (objInterferenze.DistanzaDiLegge * Moltiplicatore)) * 100)
                Dim clr As String = ""
                If Percentuale <= 20 Then
                    clr = "#FF0000"
                ElseIf Percentuale <= 40 Then
                    clr = "#FF4D4D"
                ElseIf Percentuale <= 60 Then
                    clr = "#FF8080"
                ElseIf Percentuale <= 80 Then
                    clr = "#FFB3B3"
                ElseIf Percentuale <= 100 Then
                    clr = "#FFD9D9"
                End If
                DT_Distanze_Row.Item("Colore") = clr

                idx += 1

            End If

        End While

        DT_Distanze.AcceptChanges()

        Session("DT_Distanze") = DT_Distanze

        finalizzaGrigliadistanze(DT_Distanze)

    End Sub

    Private Function generaTestoImpiantoGriglia(ByVal DR_Impianti As DataRow) As String

        Dim indirizzo As String

        If (Not IsDBNull(DR_Impianti.Item("via_stringa"))) AndAlso (DR_Impianti.Item("via_stringa") <> "") Then

            indirizzo = DR_Impianti.Item("via_stringa")
        Else

            indirizzo = DR_Impianti.Item("Indirizzo") & " " & DR_Impianti.Item("Comune_Des") & " - " & DR_Impianti.Item("Provincia_Sigla")
        End If

        'Dim rows As String(,) = {
        '    {"ID", DR_Impianti.Item("UNID_Impianto")},
        '    {"Referente", DR_Impianti.Item("Referente_RagSoc")},
        '    {"Specie", DR_Impianti.Item("Veg_Des")},
        '    {"Tipologia", DR_Impianti.Item("Tipologia_Des")},
        '    {"Azienda", DR_Impianti.Item("RagioneSociale")},
        '    {"Indirizzo", indirizzo},
        '    {"Validità", CDate(DR_Impianti.Item("Validita_Inizio")).ToShortDateString() & " - " & CDate(DR_Impianti.Item("Validita_Fine")).ToShortDateString()}
        '}

        Dim rows As String(,) = {
            {"ID", DR_Impianti.Item("UNID_Impianto")},
            {"Referente", DR_Impianti.Item("Referente_RagSoc")},
            {"Specie", DR_Impianti.Item("Veg_Des")},
            {"Tipologia", DR_Impianti.Item("Tipologia_Des")},
            {"Indirizzo", indirizzo},
            {"Validità", CDate(DR_Impianti.Item("Validita_Inizio")).ToShortDateString() & " - " & CDate(DR_Impianti.Item("Validita_Fine")).ToShortDateString()}
        }

        Dim str As String = "<table style='width:100%;'>"
        str &= "<tr><th style='width:1%;'></th><th></th></tr>"

        For r = 0 To rows.GetUpperBound(0)
            str &= "<tr>"
            str &= "<td style='text-align:right;'><b>" & rows(r, 0) & ":</b></td>"
            str &= "<td>" & rows(r, 1).Trim() & "</td>"
            str &= "</tr>"
        Next

        str &= "</table>"

        Return str

        'Dim Testo As String = ""

        'Dim col0 As New List(Of String) From {"ID", "Referente", "Specie", "Tipologia", "Azienda", "Indirizzo", "Validità"}
        'Dim col1 As New List(Of String) From {DR_Impianti.Item("UNID_Impianto"), DR_Impianti.Item("Referente_RagSoc"), DR_Impianti.Item("Veg_Des"), DR_Impianti.Item("Tipologia_Des"), DR_Impianti.Item("RagioneSociale")}

        'Dim indir As String = ""
        'If (Not IsDBNull(DR_Impianti.Item("via_stringa"))) AndAlso (DR_Impianti.Item("via_stringa") <> "") Then
        '    indir = DR_Impianti.Item("via_stringa")
        'Else
        '    indir = DR_Impianti.Item("Indirizzo") & " " & DR_Impianti.Item("Comune_Des") & " - " & DR_Impianti.Item("Provincia_Sigla")
        'End If

        'col1.Add(indir)

        'col1.Add(CDate(DR_Impianti.Item("Validita_Inizio")).ToShortDateString() & " - " & CDate(DR_Impianti.Item("Validita_Fine")).ToShortDateString())

        'Testo &= "<table style='width:100%;'>"
        'Testo &= "<tr><th style='width:1%;'></th><th></th></tr>"

        'For r = 0 To col0.Count - 1

        '    Testo &= "<tr>"
        '    Testo &= "<td style='text-align:right;'><b>" & col0(r) & ":</b></td>"
        '    Testo &= "<td>" & col1(r) & "</td>"
        '    Testo &= "</tr>"

        'Next
        'Testo &= "</table>"



        'Testo += "<b>ID :</b> " & DR_Impianti.Item("UNID_Impianto") & "<br>"
        'Testo += "<b>Referente : </b>" &
        '                 "<br>&nbsp;&nbsp;&nbsp;" &
        '                 DR_Impianti.Item("Referente_RagSoc") & "<br>"

        'Testo += "<b>Specie : </b>" &
        '                 "<br>&nbsp;&nbsp;&nbsp;" &
        '                 DR_Impianti.Item("Veg_Des") & "<br>"

        'Testo += "<b>Tipologia : </b>" &
        '                 "<br>&nbsp;&nbsp;&nbsp;" &
        '                 DR_Impianti.Item("Tipologia_Des") & "<br>"

        ''Testo += "<b>Indirizzo : </b>" & _
        ''         "<br>&nbsp;&nbsp;&nbsp;" & _
        ''         DR_Impianti.Item("Indirizzo") & _
        ''         "<br>&nbsp;&nbsp;&nbsp;" & _
        ''         DR_Impianti.Item("Comune_Des") & " - " & _
        ''         DR_Impianti.Item("Provincia_Sigla") & "<br>" &
        ''         DR_Impianti.Item("Via_Stringa") & "<br>"

        ''Testo += "<b>Indirizzo : </b>" & _
        ''        "<br>&nbsp;&nbsp;&nbsp;" & _
        ''        DR_Impianti.Item("via_stringa") & _
        ''        "<br>&nbsp;&nbsp;&nbsp;" 

        'If (Not IsDBNull(DR_Impianti.Item("via_stringa"))) AndAlso (DR_Impianti.Item("via_stringa") <> "") Then
        '    Testo += "<b>Indirizzo (App.): </b>" & "<br>&nbsp;&nbsp;&nbsp;" & DR_Impianti.Item("via_stringa") & "<br>"
        'Else
        '    Testo += "<b>Indirizzo (Centro): </b>" & "<br>&nbsp;&nbsp;&nbsp;" &
        '                     DR_Impianti.Item("Indirizzo") & "<br>&nbsp;&nbsp;&nbsp;" &
        '                     DR_Impianti.Item("Comune_Des") & " - " &
        '                     DR_Impianti.Item("Provincia_Sigla") & "<br>" &
        '                     DR_Impianti.Item("Via_Stringa") & "<br>"
        'End If

        'Testo += "<b>Validità: </b>" &
        '    CDate(DR_Impianti.Item("Validita_Inizio")).ToShortDateString() &
        '    " - " &
        '    CDate(DR_Impianti.Item("Validita_Fine")).ToShortDateString() &
        '    "<br>"

        'Return Testo
    End Function

    Private Sub finalizzaGrigliadistanze(ByVal DT_Distanze As DataTable)

        GridViewInterferenze.DataSource = DT_Distanze
        GridViewInterferenze.DataBind()

        For i = 0 To GridViewInterferenze.Rows.Count - 1
            If DT_Distanze.Rows(i).Item("Colore") <> "" Then
                GridViewInterferenze.Rows(i).Style.Add("background-color", DT_Distanze.Rows(i).Item("Colore"))
            Else
                GridViewInterferenze.Rows(i).Style.Remove("background-color")
            End If
        Next

        GridViewInterferenze.Visible = True
        Pannello_Risultato.Visible = True

        Lbl_NumeroInterferenze.Text = "N. Interferenze: " & DT_Distanze.Rows.Count

        'salvo in sessione
        'Session("DT_Distanze_2012") = DT_Distanze

        For i = 0 To GridViewInterferenze.Rows.Count - 1
            CType(GridViewInterferenze.Rows(i).Cells(0).Controls(1), CheckBox).Checked = True
        Next
    End Sub

    Protected Sub ImgBtn_Impianti_SelezionaTutti_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Impianti_SelezionaTutti.Click
        For i = 0 To GridViewInterferenze.Rows.Count - 1
            CType(GridViewInterferenze.Rows(i).Cells(0).Controls(1), CheckBox).Checked = True
        Next
    End Sub

    Protected Sub ImgBtn_Impianti_DeselezionaTutti_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Impianti_DeselezionaTutti.Click
        For i = 0 To GridViewInterferenze.Rows.Count - 1
            CType(GridViewInterferenze.Rows(i).Cells(0).Controls(1), CheckBox).Checked = False
        Next
    End Sub

    Protected Sub ImgBtn_Crescente_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Crescente.Click

        ordina(True)

    End Sub

    Protected Sub ImgBtn_Decrescente_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Decrescente.Click

        ordina(False)

    End Sub

    Protected Sub ordina(ByVal crescente As Boolean)

        Dim DT_Distanze As DataTable = Session("DT_Distanze")

        Dim ordinamento As String = Cmb_Ordinamento.SelectedValue

        Dim asc As String = IIf(crescente, " asc ", " desc ")

        Select Case ordinamento

            Case 0

            Case 1
                DT_Distanze.DefaultView.Sort = "Distanza" & asc
                DT_Distanze = DT_Distanze.DefaultView.ToTable()

            Case 2
                DT_Distanze.DefaultView.Sort = "Distanza" & asc
                DT_Distanze = DT_Distanze.DefaultView.ToTable()

            Case 3
                DT_Distanze.DefaultView.Sort = "Distanza" & asc
                DT_Distanze = DT_Distanze.DefaultView.ToTable()

            Case 4
                DT_Distanze.DefaultView.Sort = "ID_A" & asc
                DT_Distanze = DT_Distanze.DefaultView.ToTable()

            Case 5
                DT_Distanze.DefaultView.Sort = "Referente_A" & asc
                DT_Distanze = DT_Distanze.DefaultView.ToTable()

            Case 6
                DT_Distanze.DefaultView.Sort = "Distanza" & asc
                DT_Distanze = DT_Distanze.DefaultView.ToTable()

            Case 7
                DT_Distanze.DefaultView.Sort = "ID_B" & asc
                DT_Distanze = DT_Distanze.DefaultView.ToTable()

            Case 8
                DT_Distanze.DefaultView.Sort = "Referente_B" & asc
                DT_Distanze = DT_Distanze.DefaultView.ToTable()

            Case 9
                DT_Distanze.DefaultView.Sort = "Specie" & asc
                DT_Distanze = DT_Distanze.DefaultView.ToTable()


        End Select

        Session("DT_Distanze") = DT_Distanze
        finalizzaGrigliadistanze(DT_Distanze)

    End Sub

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function VerificaPermessiEstrazione() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        Try

            Dim permessiWMS As Boolean = ObjUtenti.Controlla_Permessi_Utente(
            objParametri_Utenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.Interferenze_ScaricoDati_Consolida,
            enum_Security_Operazione.Lettura,
            Date.Now,
            "",
            objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = permessiWMS.ToString

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaInterferenze() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim RisultatiInterferenze As DataTable = HttpContext.Current.Session("DT_Distanze")

        Dim Sportello As String = HttpContext.Current.Session("Sementi")
        Dim SportelloInt As Int32

        Dim FlagTransazioneLocale As Boolean
        Dim FlagConnessioneLocale As Boolean

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        Dim permessiWMS As Boolean

        Try

            permessiWMS = ObjUtenti.Controlla_Permessi_Utente(
                                        objParametri_Utenti.UtenteUsername,
                                        enum_Id_Servizio.GiasOnline,
                                        enum_Security_Attivita.Interferenze_ScaricoDati_Consolida,
                                        enum_Security_Operazione.Lettura,
                                        Date.Now,
                                        "",
                                        objParametri_Utenti)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
            Return r
        End Try

        If Not permessiWMS Then
            r.RispostaOK = False
            r.Errore = "L'utente non ha i permessi per effettuare l'operazione richiesta"
            Return r
        End If

        If Not Int32.TryParse(Sportello.Split("|")(4), SportelloInt) Then
            r.RispostaOK = False
            r.Errore = "Codice sportello non valido"
            Return r
        End If

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Dim xWrite As New AgronicaCoreSementieriDAL.Sementieri_EstrazioneInterferenze_W

            xWrite.Elimina(SportelloInt, 0, objParametri_Server)

            For Each row As DataRow In RisultatiInterferenze.Rows
                Dim tableAziendaParts = row("Impianto_A").ToString.Split(New String() {"<td>"}, StringSplitOptions.None)
                Dim DittaSementieraA = tableAziendaParts(2).Substring(0, tableAziendaParts(2).IndexOf("<"))
                Dim aziendaAIndirizzo = tableAziendaParts(5).Substring(0, tableAziendaParts(5).IndexOf("<"))
                Dim tipologia = tableAziendaParts(4).Substring(0, tableAziendaParts(4).IndexOf("<"))

                tableAziendaParts = row("Impianto_B").ToString.Split(New String() {"<td>"}, StringSplitOptions.None)
                Dim DittaSementieraB = tableAziendaParts(2).Substring(0, tableAziendaParts(2).IndexOf("<"))
                Dim aziendaBIndirizzo = tableAziendaParts(5).Substring(0, tableAziendaParts(5).IndexOf("<"))

                xWrite.Scrivi(SportelloInt,
                          row("Regione").ToString,
                          DittaSementieraA,
                          DittaSementieraB,
                          CInt(row("ID_A")),
                          CInt(row("ID_B")),
                          row("NomeComune").ToString,
                          row("NomeScientifico").ToString,
                          tipologia,
                          row("AziendaA").ToString,
                          aziendaAIndirizzo,
                          row("AziendaB").ToString,
                          aziendaBIndirizzo,
                          Decimal.Parse(row("Distanza_di_legge").ToString),
                          Decimal.Parse(row("Distanza_di_legge_moltiplicata").ToString),
                          0,
                          CostantiPersonalizzate.AGRODATAINIZIO,
                          objParametri_Server)

            Next

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = "Operazione completata con successo."

        Catch ex As Exception

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            r.RispostaOK = False
            r.Errore = ex.Message

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return r

    End Function

    Private Sub CaricaCombo_Ordinamento(dropDownList As DropDownList)
        dropDownList.Items.Clear()
        dropDownList.Items.Add(New ListItem("", "0"))
        dropDownList.Items.Add(New ListItem("Distanza", "1"))
        dropDownList.Items.Add(New ListItem("Distanza di Legge", "2"))
        dropDownList.Items.Add(New ListItem("Distanza Moltiplicata", "3"))
        dropDownList.Items.Add(New ListItem("ID 1", "4"))
        dropDownList.Items.Add(New ListItem("Referente 1", "5"))
        'dropDownList.Items.Add(New ListItem("Specie 1", "6"))
        dropDownList.Items.Add(New ListItem("ID 2", "7"))
        dropDownList.Items.Add(New ListItem("Referente 2", "8"))
        dropDownList.Items.Add(New ListItem("Specie", "9"))
    End Sub


    Private Function Informazioni_from_DT_Distanze_AUX(ByRef DT_Distanze_AUX As DataTable,
                                                       ByVal ID_Specie As Integer,
                                                       ByVal ID_Gruppo As Integer) As Integer
        Dim Risultato As DataRow()
        Dim StrSelect As String = " ID_Specie = " & ID_Specie & " AND ID_Gruppo = " & ID_Gruppo

        Risultato = DT_Distanze_AUX.Select(StrSelect)

        If Risultato.GetUpperBound(0) >= 0 Then
            Return Risultato(0)("Div_Varieta_in_Gruppo")
        End If

        Return 999999

    End Function



    Private Sub GST_Interferenze_Init_Init(sender As Object, e As EventArgs) Handles Me.Init

        AddHandler CType(Me.Master, GSTBootstrap).GSTBootstrap_ImgBtn_Filtro.Click, AddressOf ImgBtn_Filtro_Click
        AddHandler CType(Me.Master, GSTBootstrap).GSTBootstrap_ImgBtn_Stampa.Click, AddressOf ImgBtn_Stampa_Click

        AddHandler CType(Me.Master.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub


    Private Sub AnnullaTutto()
        Response.Redirect("../GST_Menu/GST_Menu.aspx")
    End Sub

End Class