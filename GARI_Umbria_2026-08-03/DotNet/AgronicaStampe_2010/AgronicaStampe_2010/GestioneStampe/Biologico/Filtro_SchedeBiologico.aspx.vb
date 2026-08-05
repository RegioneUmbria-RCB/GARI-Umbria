Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports System.Xml
Imports AgronicaCoreUtility


Public Class Filtro_SchedeBiologico
    Inherits System.Web.UI.Page

    '----- Gestione Querystring
    'Dim Piva As String
    'Dim Sa_Cod As Integer
    'Dim Cod_Report As Integer
    'Dim Data_Inizio, Data_Fine As String
    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As Integer
    Dim Qs_Cod_Report As Integer
    Dim QS_Data_Inizio, QS_Data_Fine As String

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    '######################################################################################################
    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto

    End Sub

    Private Enum enum_Pannello
        Nessuno = 0
        Colturale = 1
        MateriePrime = 2
        Vendite = 3
        Preparati = 4
    End Enum


    ' ##################################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        '########################################################################
        '#####  Querystring  #####
        '########################################################################

        Qs_Cod_Report = Stringa_Decodifica(Request.QueryString("r").ToString, _
                           AgroKey_EncoderDecoder, _
                           Server)

        Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
                       AgroKey_EncoderDecoder, _
                       Server)

        If Not IsNothing(Request.QueryString("s")) AndAlso Stringa_Decodifica(Request.QueryString("s").ToString, _
                                    AgroKey_EncoderDecoder, _
                                    Server) <> "" Then
            Qs_Sa_Cod = CInt(Stringa_Decodifica(Request.QueryString("s").ToString, _
                    AgroKey_EncoderDecoder, _
                    Server))
        Else
            Qs_Sa_Cod = 0
        End If


        QS_Data_Inizio = Stringa_Decodifica(Request.QueryString("di").ToString, _
                       AgroKey_EncoderDecoder, _
                        Server)

        QS_Data_Fine = Stringa_Decodifica(Request.QueryString("df").ToString, _
                               AgroKey_EncoderDecoder, _
                               Server)

        '########################################################################
        '#####  Verifica se la pagina e' stata caricata per la prima volta  #####
        '########################################################################

        If Not Page.IsPostBack Then

            '//////////////////////////////////////////////////////
            '/// La pagina e' stata caricata per la prima volta ///
            '//////////////////////////////////////////////////////

            '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

            'Dim UtenteAbilitato As Boolean
            'Dim strDummy As String      'controllo accesso negato.....

            'UtenteAbilitato = Controlla_Permessi_Utente_2( _
            '                                Server, Session, Page, _
            '                                Session("ASG_Utente_Username"), _
            '                                Session("ASG_IdServizio"), _
            '                                 enum_Security_Attivita.Gest_Stampe, _
            '                                 enum_Security_Operazione.Lettura, _
            '                                strDummy)


            ''----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.

            'If UtenteAbilitato = False Then
            '    Dim strClose As String = "<script language='javascript'> window.close() </script>"
            '    Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))
            'End If

        Else

            Exit Sub

        End If

        '--------------------------------------------------------------------

        'Dim strXmlVariabilistampe As String
        'Dim htVariabiliStampe As System.Collections.Hashtable
        'Dim strErr As String
        'Dim XmlDoc As New System.Xml.XmlDocument
        'Dim XML_FiltroStampa As System.Xml.XmlElement

        'strXmlVariabilistampe = Session("strXmlVariabilistampe")

        'If strXmlVariabilistampe <> "" Then

        '    'Carico la stringa xml in un nuovo documento
        '    XmlDoc = New System.Xml.XmlDocument

        '    XmlDoc.LoadXml(strXmlVariabilistampe)

        '    XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")

        '    If Not IsNothing(XML_FiltroStampa) Then

        '        '<FiltroStampa username="…" report="21" user_profilo="…" > 
        '        '<VariabiliStampe piva="…" sa_cod="…" data_inizio ="…" data_fine ="…" />
        '        '</FiltroStampa>

        '        '§§§§§§§
        '        '  XML_EstraiVariabiliStampe(strXmlVariabilistampe, htVariabiliStampe, strErr)
        '        Dim objXmlStampe As New AgronicaCoreXML.XML_Stampe
        '        objXmlStampe.XML_EstraiVariabiliStampe(strXmlVariabilistampe, htVariabiliStampe, strErr)

        '        Piva = CStr(htVariabiliStampe("piva"))
        '        Sa_Cod = CStr(htVariabiliStampe("sa_cod"))
        '        Try
        '            Cod_Report = htVariabiliStampe("codice_report")

        '            If Cod_Report = 0 Then
        '                Cod_Report = htVariabiliStampe("report")
        '            End If
        '        Catch ex As Exception
        '            Cod_Report = 21
        '        End Try


        '        Data_Inizio = CStr(htVariabiliStampe("data_inizio"))
        '        Data_Fine = CStr(htVariabiliStampe("data_fine"))

        '    Else
        '        Dim VariabiliStampe As XmlElement
        '        VariabiliStampe = XmlDoc.SelectSingleNode("//VariabiliStampe")
        '        Piva = VariabiliStampe.GetAttribute("piva")
        '        Cod_Report = VariabiliStampe.GetAttribute("codice_report")
        '        Data_Inizio = ""
        '        Data_Fine = ""
        '    End If
        '    Session("ReportSelezionato") = Cod_Report
        'Else
        '    Piva = ""
        '    Data_Inizio = ""
        '    Data_Fine = ""
        '    Cod_Report = 0
        'End If


        '=====================================================
        '----- Inizializzo i controlli
        '=====================================================

        'DEFAULT SULLA STAMPA DEL LOTTO
        Dim Is_UtenteGiasLan As Boolean = False
        Dim objProfil As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Is_UtenteGiasLan = objProfil.Is_UtenteGiasLan("", objParametri_Utenti)
        If Is_UtenteGiasLan = True Then
            Me.Rbl_StampaLotto.SelectedValue = 1
        Else
            Me.Rbl_StampaLotto.SelectedValue = 0
        End If

        '-----------------------------------------------------------------

        Dim objHLP As New AgronicaCoreContabHLP.Contabilita
        Dim data_da As Date = AGRODATAINIZIO
        Dim data_a As Date = AGRODATAFINE

        objHLP.DataInizioFineMese_from_Data(data_da, _
                                              data_a, _
                                              Date.Today)

        Me.TxtDataDa.Text = data_da
        Me.TxtDataA.Text = data_a

        'visualizzo sempre il report delle preparazioni
        'Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        'Dim Flag_GiasLan As Boolean

        'Flag_GiasLan = objUtenti.Is_UtenteGiasLan("", objParametri_Utenti)
        'If Flag_GiasLan = True Then
        '    Me.OptionList_SchedaBiologico.Items.Add(New ListItem("Registro Preparazioni", enum_CodificaStampe.SchedaPreparati_Biologico))
        'End If

        If Qs_Cod_Report <> 0 Then
            Try
                Me.OptionList_SchedaBiologico.SelectedValue = Qs_Cod_Report
            Catch ex As Exception
                'il codice report non è presente
            End Try
        End If
        Cambia_Report()

        Dim REG As String = ""
        If Qs_Piva <> "" Then

            AgronicaCoreUtility.CaricaListControl.Imprese(Cmb_Impresa, False, "", "", Qs_Piva, "", _
                                                            "", objParametri_Server)

            Me.Lbl_NumImprese.Text = CStr(Me.Cmb_Impresa.Items.Count)

            REG = CaricaCentriAz(Qs_Sa_Cod)
            If REG = "" Then
                'default Emilia-Romagna
                REG = "008"
            End If
        End If

        AgronicaCoreUtility.CaricaListControl.Prodotto_LineeClassiProduzioni(Me.Cmb_ClassiProdotto, _
                                                                                True, "", "", _
                                                                                Qs_Piva, _
                                                                                0, 0, _
                                                                                "", "", _
                                                                                objParametri_Server)

        AgronicaCoreUtility.CaricaListControl.Regioni(Me.Cmb_Regioni, _
                                                          False, "", "", _
                                                          "", _
                                                          "", "", _
                                                          objParametri_Server)

        AgronicaCoreUtility.CaricaListControl.Regioni(Me.Cmb_Regioni2, _
                                                  False, "", "", _
                                                  "", _
                                                  "", "", _
                                                  objParametri_Server)

        Me.Cmb_Regioni.SelectedIndex = _
            Me.Cmb_Regioni.Items.IndexOf( _
                Me.Cmb_Regioni.Items.FindByValue( _
                    REG))

        Me.Cmb_Regioni2.SelectedIndex = _
    Me.Cmb_Regioni2.Items.IndexOf( _
        Me.Cmb_Regioni2.Items.FindByValue( _
            REG))

        CType(Page.Master.FindControl("Lbl_Rag_Soc"), Label).Text = ""
        CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = ""


    End Sub

    ' ##################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim strJS1 As New StringBuilder
        strJS1.AppendLine("$(document).ready(function () { ")
        strJS1.AppendLine("      window.close() ")
        strJS1.AppendLine(" });")

        ScriptManager.RegisterStartupScript(Me.Page, Me.Page.GetType(),
                                  String.Format("jQuery_{0}", Me.Page.ClientID), strJS1.ToString, True)

    End Sub


    '##########################################################################################################################################
    Private Sub Imposta_Pannelli(ByVal panel As enum_Pannello)

        '----- Disattivo tutti i pannelli
        'Me.Pannello_Report.Visible = True
        Me.Pannello_TipoReport.Visible = True
        Me.Pannello_Filtri.Visible = True

        Me.Pannello_ImpresaCentro.Visible = False
        Me.Pannello_Date.Visible = False
        Me.Pannello_Preparati.Visible = False
        Me.Pannello_Categorie.Visible = False
        Me.Pannello_xVendite.Visible = False
        Me.Riga_regione.Visible = False

        Select Case panel
            Case enum_Pannello.Colturale
                'rendi tutto invisibile

            Case enum_Pannello.MateriePrime
                'impresa
                'centro
                'date
                'filtro categorie
                'regione
                Me.Pannello_ImpresaCentro.Visible = True
                Me.Pannello_Date.Visible = True
                Me.Pannello_Categorie.Visible = True
                Me.Riga_regione.Visible = True

            Case enum_Pannello.Vendite
                'impresa
                'centro
                'date
                'filtro categorie
                'x vendite
                'regione
                Me.Pannello_ImpresaCentro.Visible = True
                Me.Pannello_Date.Visible = True
                Me.Pannello_Categorie.Visible = True
                Me.Pannello_xVendite.Visible = True
                Me.Riga_regione.Visible = True

            Case enum_Pannello.Preparati
                'impresa
                'centro
                'filtro pannello preparati
                Me.Pannello_Date.Visible = True
                Me.Pannello_ImpresaCentro.Visible = True
                Me.Pannello_Preparati.Visible = True
      
        End Select

        'Dim Dimensione As Unit

        'With Me.Pannello_Filtri
        '    .Height = Dimensione.Pixel(656)
        '    .Width = Dimensione.Pixel(974)
        '    '.Style.Item("Top") = 280
        '    '.Style.Item("Left") = 8
        'End With

        'With Me.Pannello_xVendite
        '    .Height = Dimensione.Pixel(170)
        '    .Width = Dimensione.Pixel(972)
        'End With

        'With Me.Pannello_Report
        '    .Height = Dimensione.Pixel(104)
        '    .Width = Dimensione.Pixel(972)
        '    '.Style.Item("Top") = 280
        '    '.Style.Item("Left") = 8
        'End With

        ''----- Imposto le dimensioni
        'With Me.Pannello_ImpresaCentro
        '    .Height = Dimensione.Pixel(112)
        '    .Width = Dimensione.Pixel(972)
        '    '.Style.Item("Top") = 280
        '    '.Style.Item("Left") = 8
        'End With

        ''----- Imposto le dimensioni
        'With Me.Pannello_Date
        '    .Height = Dimensione.Pixel(55)
        '    .Width = Dimensione.Pixel(972)
        '    '.Style.Item("Top") = 280
        '    '.Style.Item("Left") = 8
        'End With

        ''----- Imposto le dimensioni
        'With Me.Pannello_Categorie
        '    .Height = Dimensione.Pixel(136)
        '    .Width = Dimensione.Pixel(972)
        '    '.Style.Item("Top") = 280
        '    '.Style.Item("Left") = 8
        'End With

        ''----- Imposto le dimensioni
        'With Me.Pannello_Preparati
        '    .Height = Dimensione.Pixel(140)
        '    .Width = Dimensione.Pixel(972)
        '    .Style.Item("Top") = 192
        '    .Style.Item("Left") = 0
        'End With


    End Sub

    '##############################################################################
    Private Sub OptionList_SchedaBiologico_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OptionList_SchedaBiologico.SelectedIndexChanged
        Cambia_Report()
    End Sub

    '##########################################################################################################################################
    Private Sub Cambia_Report()

        Select Case Me.OptionList_SchedaBiologico.SelectedValue

            Case enum_CodificaStampe.SchedaColturale_Biologico
                'rendi tutto invisibile
                Imposta_Pannelli(enum_Pannello.Colturale)

            Case enum_CodificaStampe.SchedaMateriePrime_Biologico
                'impresa
                'centro
                'date
                'filtro categorie
                Imposta_Pannelli(enum_Pannello.MateriePrime)

                AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(Me.ChkList_Categorie, _
                                                            False, "", "", _
                                                            0, _
                                                            CAU_SCARICO, _
                                                            0, _
                                                             False, _
                                                              False, _
                                                              "", "", _
                                                            objParametri_Server)

                Dim i As Integer
                For i = 0 To Me.ChkList_Categorie.Items.Count - 1
                    Select Case Me.ChkList_Categorie.Items(i).Value
                        'seleziono come default
                        Case FERTILIZZANTI, FORMULATI, SEMENTI, MATERIE_VEGETALI
                            Me.ChkList_Categorie.Items(i).Selected = True
                    End Select
                Next

            Case enum_CodificaStampe.SchedaVendite_Biologico
                'impresa
                'centro
                'date
                'filtro categorie
                Imposta_Pannelli(enum_Pannello.Vendite)

                Dim Filtro_Cat As String
                Filtro_Cat = " (Tabella IN ('Materie_Prime','TipologieSementi') ) " + _
                                " AND  Elem_Cod NOT IN (" + CStr(FARMACI) + ", " + CStr(MANGIMI) + ")"

                AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(Me.ChkList_Categorie, _
                                                                        False, "", "", _
                                                                        0, _
                                                                        CAU_SCARICO, _
                                                                        0, _
                                                                        False, _
                                                                        False, _
                                                                        Filtro_Cat, "", _
                                                                        objParametri_Server)

                Dim i As Integer
                For i = 0 To Me.ChkList_Categorie.Items.Count - 1
                    Select Case Me.ChkList_Categorie.Items(i).Value
                        'seleziono come default
                        Case SEMILAVORATI_VEGETALI, TRASFORMATI_VEGETALI
                            Me.ChkList_Categorie.Items(i).Selected = True
                    End Select
                Next

            Case enum_CodificaStampe.SchedaPreparati_Biologico
                Imposta_Pannelli(enum_Pannello.Preparati)

        End Select

    End Sub

    '###########################################################################
    Private Sub Cmb_Impresa_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Impresa.SelectedIndexChanged

        CaricaCentriAz(0)

    End Sub


    ''' <summary>
    ''' Evento sul cambio del centro aziendale per caricare i Magazzini
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Cmb_CentroAziendale_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles Cmb_CentroAziendale.SelectedIndexChanged
        CaricaMagazzini(0)
    End Sub

    '###########################################################################
    Private Function CaricaCentriAz(ByVal Sa_Cod As Integer) As String

        Dim REG As String = ""
        Dim Piva As String

        Me.Cmb_CentroAziendale.Items.Clear()
        Me.Cmb_Prodotti.Items.Clear()

        If Me.Cmb_Impresa.SelectedValue <> "" Then

            Piva = Me.Cmb_Impresa.SelectedValue

            '---- Centro Aziendale
            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.Centri_Aziendali(
                                    Me.Cmb_CentroAziendale,
                                    True, "Tutti i Centri", "0",
                                    Piva, True, 2,
                                    "", "", objParametri_Server)

            'If Me.Cmb_CentroAziendale.Items.Count > 1 Then
            '    Me.Cmb_CentroAziendale.SelectedIndex = 1
            'End If
            If Sa_Cod <> 0 Then
                Me.Cmb_CentroAziendale.SelectedIndex = _
                    Me.Cmb_CentroAziendale.Items.IndexOf(Me.Cmb_CentroAziendale.Items.FindByValue(Sa_Cod))
            Else
                If Me.Cmb_CentroAziendale.Items.Count > 1 Then
                    Me.Cmb_CentroAziendale.SelectedIndex = 1
                Else
                    Me.Cmb_CentroAziendale.SelectedIndex = 0
                End If
            End If

            If Not IsNothing(Cmb_CentroAziendale.SelectedValue) AndAlso Cmb_CentroAziendale.SelectedValue <> "0" Then
                Dim objCentriInd As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
                REG = objCentriInd.Regione_from_PivaSaCod(Piva, Cmb_CentroAziendale.SelectedValue, objParametri_Server)
            End If


            'Inserisco tutti i Magazzini per Default:
            Me.Cmb_Magazzino.Items.Clear()
            Me.Cmb_Magazzino.Items.Add(New ListItem("Tutti i Magazzini", "0"))

            'carica menù a tendina dei prodotti

            Dim filtro As String
            filtro = " [Materie_Prime].Regolamento = 4 " + _
                        "AND [Materie_Prime].elem_cod IN (" + CStr(SEMILAVORATI_VEGETALI) + " ," + _
                        CStr(TRASFORMATI_VEGETALI) + " ," + CStr(SEMILAVORATI_ANIMALI) + " ," + CStr(TRASFORMATI_ANIMALI) + ") "

            'elenco semilavorati e trasformati vegetali/animali, biologici e abilitati per il reg preparazioni bio
            AgronicaCoreUtility.CaricaListControl.Materie_PrimeXReport(Me.Cmb_Prodotti, _
                                                                        False, "", "", _
                                                                        Piva, _
                                                                        0, 0, 0, _
                                                                        enum_AgroReportisticaTipi.RegistriBio, _
                                                                        2, _
                                                                        filtro, _
                                                                        "", _
                                                                        objParametri_Server)

            Cambia_Prodotto(Qs_Piva)

            '--------------------------

        End If

        Return REG

    End Function


    '######################################################################################################################################################################
    Private Sub Cmb_Prodotti_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Prodotti.SelectedIndexChanged
        Cambia_Prodotto(Me.Cmb_Impresa.SelectedValue)
    End Sub

    '######################################################################################################################################################################
    Private Sub Cambia_Prodotto(ByVal Piva As String)

        If Not IsNothing(Me.Cmb_Prodotti.SelectedItem) Then

            If Me.Cmb_Prodotti.SelectedValue <> "" Then

                Dim Mat_Cod As Integer

                Mat_Cod = Me.Cmb_Prodotti.SelectedValue.Split("|")(0)

                'linee e preparazioni del mat_cod selezionato
                AgronicaCoreUtility.CaricaListControl.Linee_Produzioni_Preparazioni(Me.Cmb_Preparazioni, _
                                                                                    False, "", "", _
                                                                                    Piva, _
                                                                                    enum_AgroReportistica.PreparazioniBio, _
                                                                                    Mat_Cod, _
                                                                                    "", "", _
                                                                                    objParametri_Server)

            End If

        End If

    End Sub

    '########################################################################################
    Private Sub ImgBtn_CercaProdotto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_CercaProdotto.Click
        CercaProdotto()
    End Sub

    '########################################################################################
    Private Sub CercaProdotto()

        Dim filtro As String = ""
        Dim str_ElemCod As String = ""

        str_ElemCod = Genera_str_ElemCod(Nothing)

        filtro = " Materie_Prime.Regolamento = 4 "

        If str_ElemCod <> "" Then
            filtro += " AND Materie_Prime.Elem_Cod  IN  " + str_ElemCod
        End If

        If Not IsNothing(Me.Cmb_ClassiProdotto.SelectedItem) AndAlso Me.Cmb_ClassiProdotto.SelectedValue <> "" Then
            'If filtro <> "" Then
            '    filtro += " AND "
            'End If
            filtro += " AND (Materie_Prime.Cat_Cod = " + Agro_SQL_SaveNum(Me.Cmb_ClassiProdotto.SelectedValue) + ") "
        End If

        Dim clc = New AgronicaCoreUtility.CaricaListControl
        clc.Materie_Prime(Me.Cmb_ProdottoxVendite,
                                                            True, "", "",
                                                            CAU_CARICO,
                                                            Qs_Piva,
                                                            0, 0, 0, False,
                                                            Me.Txt_MatDes.Text,
                                                            "",
                                                            Me.Txt_CodArticolo.Text,
                                                            "", 0, 0, 0, CODPROGETTO_NONDEFINITO, 0, LOTTO_NONDEFINITO,
                                                            0, 0, 0, 0, 0, 0, 0,
                                                            Date.Today,
                                                            filtro,
                                                            "",
                                                            objParametri_Server,
                                                            objParametri_Utenti)


    End Sub


    '########################################################################################
    Private Sub ImgBtn_CercaContatto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_CercaContatto.Click
        CercaContatto()
    End Sub


    '########################################################################################
    Private Sub CercaContatto()

        Me.Cmb_Contatti.Items.Clear()

        If Me.Txt_CodContatto.Text = "" And Me.Txt_ContattoDes.Text = "" Then
            Messaggi.AgroMsgBox("e' necessario specificare un filtro di ricerca per il caricamento dei contatti.", Me.Master.Page)
            Exit Sub
        End If

        Dim filtro_cont As String = "  (Rapporti_Contabili.Cliente = 1 OR Rapporti_Contabili.Terzista = 1) "

        If Me.Txt_CodContatto.Text <> "" Then
            filtro_cont += " AND  contatti.cod_contatto = '" & Agro_SQL_SaveText(Me.Txt_CodContatto.Text) & "'"
        End If

        If Me.Txt_ContattoDes.Text <> "" Then
            filtro_cont += " AND ( contatti.rag_soc LIKE '%" & Agro_SQL_SaveText(Me.Txt_ContattoDes.Text) & _
                                "%' OR Contatti.Nome LIKE '%" & Agro_SQL_SaveText(Me.Txt_ContattoDes.Text) & _
                                "%' OR Contatti.Cognome LIKE '%" & Agro_SQL_SaveText(Me.Txt_ContattoDes.Text) & "%' ) "
        End If

        Dim Piva As String = ""
        If Me.Cmb_Impresa.SelectedValue <> "" Then
            Piva = Me.Cmb_Impresa.SelectedValue
        End If

        AgronicaCoreUtility.CaricaListControl.Contatti(Me.Cmb_Contatti, _
                                                            True, "", "", _
                                                            Piva, "", _
                                                            0, 0, _
                                                            True, _
                                                            False, _
                                                            0, 0, False, 0, _
                                                            -99, _
                                                            filtro_cont, "", _
                                                            objParametri_Server)

        If Me.Cmb_Contatti.Items.Count > 1 Then
            Me.Cmb_Contatti.SelectedIndex = 1
        End If

    End Sub

    '########################################################################################
    Private Sub ImgBtn_Cerca_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Cerca.Click

        Me.Cmb_CentroAziendale.Items.Clear()

        If Me.Txt_Impresa.Text = "" Then
            Messaggi.AgroMsgBox("Impostare il filtro ragione sociale!", Me.Master.Page)
        Else
            Carica_Imprese(Me.Txt_Impresa.Text)
        End If

    End Sub


    '########################################################################################
    Private Sub Carica_Imprese(ByVal TestoCercaRagSoc As String)

        Dim strFiltro As String = ""
        Dim Num_Totale As Integer = 0

        '----------------------------------------------------------------
        '--- Filtro personalizzato 
        '----------------------------------------------------------------

        'If TestoCercaRagSoc <> "" Then
        strFiltro = " (Imprese.Rag_Soc like '%" & TestoCercaRagSoc & "%') "
        'End If

        AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(Me.Cmb_Impresa, _
                                            False, "", "", _
                                            strFiltro, _
                                            "ORDER BY Imprese.rag_soc ", _
                                             objParametri_Server, _
                                             objParametri_Utenti)

        'CaricaCombo_ImpreseUtente_Optimize(Server, Session, Page, _
        '                                    Me.Cmb_Impresa, _
        '                                    Num_Totale, _
        '                                    strFiltro, _
        '                                    "ORDER BY Imprese.rag_soc", _
        '                                    False, , )

        Num_Totale = Me.Cmb_Impresa.Items.Count

        Me.Lbl_NumImprese.Text = CStr(Num_Totale)

        Dim REG As String = ""
        REG = CaricaCentriAz(0)

        If REG = "" Then
            'default Emilia-Romagna
            REG = "008"
        End If

        Me.Cmb_Regioni.SelectedIndex = _
                                 Me.Cmb_Regioni.Items.IndexOf( _
                                     Me.Cmb_Regioni.Items.FindByValue( _
                                         REG))

        Me.Cmb_Regioni2.SelectedIndex = _
                                Me.Cmb_Regioni2.Items.IndexOf( _
                                    Me.Cmb_Regioni2.Items.FindByValue( _
                                        REG))

    End Sub


    '###########################################################################
    Private Sub ImgBtn_Stampa_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa.Click
        Stampa()
    End Sub

    '###########################################################################
    Private Function Genera_str_ElemCod(ByRef Messaggio As String) As String

        Dim Str_elem_cod As String = ""
        Dim AlmenoUno As Boolean = False

        Dim j As Integer
        For j = 0 To Me.ChkList_Categorie.Items.Count - 1
            If Me.ChkList_Categorie.Items(j).Selected = True Then
                Str_elem_cod += CStr(Me.ChkList_Categorie.Items(j).Value) + ","
                AlmenoUno = True
            End If
        Next
        If AlmenoUno = False Then
            Messaggio += "E' necessario selezionare almeno una categoria di magazzino!"
        Else
            Str_elem_cod = Left(Str_elem_cod, Str_elem_cod.Length - 1)
            Str_elem_cod = "(" + Str_elem_cod + ")"
        End If

        Return Str_elem_cod

    End Function

    '###########################################################################
    Private Sub Stampa()

        Dim TargetURL, Messaggio, Titolo As String
        Dim Piva, Str_elem_cod As String
        Dim Sa_Cod As Integer
        Dim Fabbricato_Cod As Integer
        Dim Data_Inizio, Data_Fine As Date
        Dim Elem_Cod, Mat_Cod, Cod_RisUm, Linea_Cod, Preparazione_Cod As Integer
        Dim Mat_Des As String = ""
        Dim Regione_Cod As String = ""
        Dim Flag_Regione As Integer = 0
        Dim Linea_Classe_Cod As Integer = 0
        Dim Flag_StampaConsistenzeVasca As Boolean = False

        If Not IsNothing(Me.Cmb_Impresa.SelectedItem) Then
            If Me.Cmb_Impresa.SelectedItem.Text = "" Then
                Messaggio += "Selezionare un'Impresa!"
            Else
                Piva = Me.Cmb_Impresa.SelectedValue
            End If
        Else
            Messaggio += "Non ci sono Imprese! Impossibile stampare."
        End If

        If Not IsNothing(Me.Cmb_CentroAziendale.SelectedItem) Then
            If Me.Cmb_CentroAziendale.SelectedItem.Text = "" Then
                Messaggio += "Selezionare un Centro Aziendale!"
            Else
                Sa_Cod = Me.Cmb_CentroAziendale.SelectedValue
            End If
        Else
            Messaggio += "Non ci sono Centri Aziendali! Impossibile stampare."
        End If

        If Not IsNothing(Me.Cmb_Magazzino.SelectedItem) Then
            If Me.Cmb_Magazzino.SelectedValue = "" Then
                Messaggi.AgroMsgBox("Selezionare un Magazzino!", Me.Master.Page)
                Exit Sub
            Else
                Fabbricato_Cod = Me.Cmb_Magazzino.SelectedValue
            End If
        Else
            Messaggi.AgroMsgBox("Non ci sono Magazzini! Impossibile stampare.", Me.Master.Page)
            Exit Sub
        End If

        If Me.Pannello_Date.Visible = True Then

            If Me.TxtDataDa.Text = "" Or Me.TxtDataA.Text = "" Then
                Messaggio += "E' necessario specificare l'intervallo temporale!"
            Else
                If CDate(Me.TxtDataDa.Text) > CDate(Me.TxtDataA.Text) Then
                    Messaggio += "La data di inizio dell'intervallo non può essere superiore alla data di fine!"
                Else
                    Data_Inizio = Me.TxtDataDa.Text
                    Data_Fine = Me.TxtDataA.Text
                End If
            End If
        End If

        If Me.Pannello_Categorie.Visible = True Then

            Str_elem_cod = Genera_str_ElemCod(Messaggio)

            If Not IsNothing(Me.Cmb_ClassiProdotto.SelectedItem) AndAlso Me.Cmb_ClassiProdotto.SelectedValue <> "" Then
                Linea_Classe_Cod = Me.Cmb_ClassiProdotto.SelectedValue
            End If

            Flag_StampaConsistenzeVasca = Me.Chk_ConsistenzeVasca.Checked

        End If

        If Me.Pannello_xVendite.Visible = True Then

            If Not IsNothing(Me.Cmb_ProdottoxVendite.SelectedItem) AndAlso Me.Cmb_ProdottoxVendite.SelectedValue <> "" Then
                Mat_Cod = Me.Cmb_ProdottoxVendite.SelectedValue
            End If

            If Not IsNothing(Me.Cmb_Contatti.SelectedItem) AndAlso Me.Cmb_Contatti.SelectedValue <> "" Then
                Cod_RisUm = Me.Cmb_Contatti.SelectedValue
            End If

        End If

        If Me.Pannello_Preparati.Visible = True Then

            If Not IsNothing(Me.Cmb_Prodotti.SelectedItem) AndAlso Me.Cmb_Prodotti.SelectedValue <> "" Then
                Mat_Des = Me.Cmb_Prodotti.SelectedItem.Text
                Mat_Cod = Me.Cmb_Prodotti.SelectedValue.Split("|")(0)
                Elem_Cod = Me.Cmb_Prodotti.SelectedValue.Split("|")(1)
            Else
                Messaggio += "Per stampare il Registro Preparazioni Bio è necessario selezionare il prodotto." + vbCrLf
            End If

            If Not IsNothing(Me.Cmb_Preparazioni.SelectedItem) AndAlso Me.Cmb_Preparazioni.SelectedValue <> "" Then
                Linea_Cod = Me.Cmb_Preparazioni.SelectedValue.Split("|")(0)
                Preparazione_Cod = Me.Cmb_Preparazioni.SelectedValue.Split("|")(1)
            Else
                Messaggio += "Per stampare il Registro Preparazioni Bio è necessario selezionare la Linea di Produzione/Preparazione." + vbCrLf
            End If

            If Me.Chk_LogoRegione.Checked = True Then
                If Me.Cmb_Regioni.SelectedValue <> "" Then
                    Regione_Cod = Me.Cmb_Regioni.SelectedValue
                    Flag_Regione = 1
                End If
            End If

        End If

        If Me.Riga_regione.Visible = True Then
            If Me.chk_Regione2.Checked = True Then
                If Me.Cmb_Regioni2.SelectedValue <> "" Then
                    Regione_Cod = Me.Cmb_Regioni2.SelectedValue
                End If
            End If
        End If

        If Messaggio <> "" Then
            Messaggi.AgroMsgBox(Messaggio, Me.Master.Page)
            Exit Sub
        End If

        ''================================================================

        Dim Querystring As String

        Querystring = "?p=" + Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server) +
                        "&s=" + Stringa_Codifica(Sa_Cod, AgroKey_EncoderDecoder, Server) &
                        "&f=" + Stringa_Codifica(Fabbricato_Cod, AgroKey_EncoderDecoder, Server) &
                        "&di=" + Stringa_Codifica(Data_Inizio.ToShortDateString, AgroKey_EncoderDecoder, Server) +
                        "&df=" + Stringa_Codifica(Data_Fine.ToShortDateString, AgroKey_EncoderDecoder, Server) &
                        "&arr=" & Stringa_Codifica(Rbl_Arrotondamento.SelectedValue, AgroKey_EncoderDecoder, Server)

        If Me.Chk_MostraDataOdiernaStampa.Checked = True Then
            Session("MostraDataOdiernaStampa") = True
        Else
            Session("MostraDataOdiernaStampa") = False
        End If

        Select Case Me.OptionList_SchedaBiologico.SelectedValue

            Case enum_CodificaStampe.SchedaColturale_Biologico
                'TargetURL = "../SchedaCampagna/Selezione_SchedaCampagna.aspx"
                'Titolo = "Scheda_Colturale_Bio"
                'no perchè occorre selezionare gli impianti dal filtrone

                '------------------------------------------------------------

            Case enum_CodificaStampe.SchedaMateriePrime_Biologico

                TargetURL = "SchedaMateriePrime/SchedaMateriePrimeBiologico.aspx"
                Titolo = "SchedaMateriePrimeBio"

                Querystring += "&fec=" + Stringa_Codifica(Str_elem_cod, AgroKey_EncoderDecoder, Server) & _
                                "&lcp=" + Stringa_Codifica(Linea_Classe_Cod, AgroKey_EncoderDecoder, Server) & _
                                "&vas=" + Stringa_Codifica(Flag_StampaConsistenzeVasca, AgroKey_EncoderDecoder, Server) & _
                                "&chkca=" + Stringa_Codifica(Me.Chk_StampaCodArticolo.Checked, AgroKey_EncoderDecoder, Server) & _
                                "&random=" & Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server) & _
                                "&rc=" & Stringa_Codifica(Regione_Cod, AgroKey_EncoderDecoder, Server)

                '------------------------------------------------------------

            Case enum_CodificaStampe.SchedaVendite_Biologico
                TargetURL = "SchedaVendite/SchedaVenditeBiologico.aspx"
                Titolo = "SchedaVenditeBio"

                Querystring += "&fec=" + Stringa_Codifica(Str_elem_cod, AgroKey_EncoderDecoder, Server) + _
                                "&lcp=" + Stringa_Codifica(Linea_Classe_Cod, AgroKey_EncoderDecoder, Server) + _
                                "&m=" + Stringa_Codifica(Mat_Cod, AgroKey_EncoderDecoder, Server) + _
                                "&ru=" + Stringa_Codifica(Cod_RisUm, AgroKey_EncoderDecoder, Server) + _
                                "&random=" & Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server) & _
                                "&sl=" + Stringa_Codifica(Me.Rbl_StampaLotto.SelectedValue, AgroKey_EncoderDecoder, Server) & _
                                "&chkca=" + Stringa_Codifica(Me.Chk_StampaCodArticolo.Checked, AgroKey_EncoderDecoder, Server) & _
                                "&rc=" & Stringa_Codifica(Regione_Cod, AgroKey_EncoderDecoder, Server)

                '------------------------------------------------------------

            Case enum_CodificaStampe.SchedaPreparati_Biologico

                'AgroMsgBox("Stampa in fase di sviluppo, sarà attivata prossimamente.", Page)
                'Exit Sub

                TargetURL = "../Biologico/RegistroPreparazioniBio/RegistroPreparazioniBio.aspx"
                Titolo = "RegistroPreparazioniBio"

                Querystring += "&lc=" & Stringa_Codifica(Linea_Cod, AgroKey_EncoderDecoder, Server) & _
                     "&pc=" & Stringa_Codifica(Preparazione_Cod, AgroKey_EncoderDecoder, Server) & _
                     "&rc=" & Stringa_Codifica(Regione_Cod, AgroKey_EncoderDecoder, Server) & _
                     "&fr=" & Stringa_Codifica(Flag_Regione, AgroKey_EncoderDecoder, Server) & _
                    "&mdes=" & Stringa_Codifica(Mat_Des, AgroKey_EncoderDecoder, Server) & _
                    "&m=" & Stringa_Codifica(Mat_Cod, AgroKey_EncoderDecoder, Server) & _
                    "&e=" & Stringa_Codifica(Elem_Cod, AgroKey_EncoderDecoder, Server) & _
                   "&rs=" & Stringa_Codifica(QS_SaveText(Me.Cmb_Impresa.SelectedItem.Text), AgroKey_EncoderDecoder, Server) + _
                   "&sn=" & Stringa_Codifica(QS_SaveText(Me.Cmb_CentroAziendale.SelectedItem.Text), AgroKey_EncoderDecoder, Server) + _
                    "&random=" & Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server) + _
                    "&seza=" & Stringa_Codifica(Me.ChkList_SezioniPrep.Items(0).Selected, AgroKey_EncoderDecoder, Server) + _
                    "&sezb=" & Stringa_Codifica(Me.ChkList_SezioniPrep.Items(1).Selected, AgroKey_EncoderDecoder, Server)

        End Select


        'Page_NewWindow(Page, TargetURL, Querystring, Titolo, , , , , , , , )

        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaScripPerPopup(TargetURL & Querystring, "Stampa")
        ScriptManager.RegisterStartupScript(Page, _
                                            Page.GetType(), _
                                            "jQuery_{0}", strJS, False)


    End Sub

    Private Sub CaricaMagazzini(ByVal Fabbricato_Cod As Integer)

        Dim Piva As String
        Dim Sa_Cod As Integer

        Me.Cmb_Magazzino.Items.Clear()
        'Me.Cmb_Prodotti.Items.Clear()

        If Me.Cmb_Impresa.SelectedValue <> "" Then

            Piva = Me.Cmb_Impresa.SelectedValue

            If Me.Cmb_CentroAziendale.SelectedValue <> "" Then

                Sa_Cod = Me.Cmb_CentroAziendale.SelectedValue

                If Sa_Cod = "0" Then

                    'Inserisco tutti i Magazzini per Default perchè ho selezionato Tutti i Centri:
                    Me.Cmb_Magazzino.Items.Clear()
                    Me.Cmb_Magazzino.Items.Add(New ListItem("Tutti i Magazzini", "0"))

                Else
                    '---- Magazzini
                    Dim clc = New AgronicaCoreUtility.CaricaListControl
                    clc.Fabbricati(Me.Cmb_Magazzino,
                                                    True, "Tutti i Magazzini", "0",
                                                    Piva,
                                                    Sa_Cod,
                                                    0,
                                                    FABBRICATI_NO_STALLE,
                                                     False,
                                                     "",
                                                     " Fabbricati.Fabbricato_Des ",
                                                    AGRODATAFINE,
                                                     HttpContext.Current.Session("ASG_objParametri_Server"))

                    If Fabbricato_Cod <> 0 Then
                        Me.Cmb_Magazzino.SelectedIndex = _
                            Me.Cmb_Magazzino.Items.IndexOf(Me.Cmb_Magazzino.Items.FindByValue(Fabbricato_Cod))
                    Else
                        Me.Cmb_Magazzino.SelectedIndex = 0
                    End If

                End If


            End If

        End If

    End Sub

End Class