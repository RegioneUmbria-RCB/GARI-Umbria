Imports System.Web
Imports System.Web.Services

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreXML.XML_Stampe
Imports System.Drawing



Public Class RintracciaLotto
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public Master_Operazione As Agenda


    Private Sub Menu_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        Master_Operazione = CType(Page.Master, Agenda)
        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto

        'AlberoAnagrafica.Flag_Singola_Selezione = False
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        LabelRes.Text = ""

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---

        CType(Master.FindControl("Lbl_Titolo"), Label).Text = "Tracciabilità (pagina di test)"



        InizializzaVarie()

        Dim ispostbackscript As String = "var isPostBack = false;"

        If Not IsPostBack Then

            DistruggiSessionVecchie()



            '==================================
            '======= VERIFICA PERMESSI ========
            '==================================

            Dim UtenteAbilitato_Lettura As Boolean = False

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            UtenteAbilitato_Lettura = objPermessi.Controlla_Permessi_Utente( _
                                        Session("ASG_Utente_Username"), _
                                        Session("ASG_IdServizio"), _
                                        enum_Security_Attivita.Gest_Magazzino, _
                                        enum_Security_Operazione.Lettura, _
                                        Date.Now, _
                                        "", _
                                        objParametri_Utenti)

            ViewState("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura

            Dim UtenteAbilitato_Modifica As Boolean = False
            UtenteAbilitato_Modifica = objPermessi.Controlla_Permessi_Utente( _
                                       Session("ASG_Utente_Username"), _
                                       Session("ASG_IdServizio"), _
                                       enum_Security_Attivita.Gest_Magazzino, _
                                       enum_Security_Operazione.Modifica, _
                                       Date.Now, _
                                       "", _
                                       objParametri_Utenti)

            ViewState("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica


            '##############################################################
            '#####  Inizializzo i controlli  ##############################
            '##############################################################
            ImpostaPermessi()






            ' CaricaCentriAziendali()
            ' CaricaMagazzini()

            AgronicaCoreUtility.CaricaListControl.CategorieMagazzino( _
                                                Me.cmb_Categoria, _
                                                True, "", "", _
                                                0, _
                                                  "", _
                                                 0, _
                                                 False, _
                                                 True, _
                                                 "Elem_Cod <> " & COADIUVANTI.ToString, _
                                                 "", objParametri_Server)


            cmb_Categoria.SelectedValue = TRASFORMATI_VEGETALI


            txt_DataOperazioneAlGiorno.Text = Date.Now.ToShortDateString

            'If ParametriPaginaGestioneMagazzini.Pro_Cod <> 0 Then
            '    TxtCodProdotto.Text = CStr(ParametriPaginaGestioneMagazzini.Pro_Cod)
            'End If
            'TxtProdotto.Text = ParametriPaginaGestioneMagazzini.Prodotto
            'TxtLotto.Text = ParametriPaginaGestioneMagazzini.Lotto

            If TxtCodProdotto.Text <> "" Then ' AndAlso (tipoOperazione = "0" Or tipoOperazione = "2") Then
                Esegui_Ricerca_Movimenti_Click()
            End If




        End If




        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "IsPostBack", ispostbackscript, True)

    End Sub


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


    Private Sub InizializzaVarie()

        'script Centri
        ScriptManager.RegisterStartupScript(UpdatePanel_Menu, UpdatePanel_Menu.GetType(),
                                         String.Format("jQuery_{0}", ComboCentroAziendale.ClientID), ComboCentroAziendale.GetJS(), True)

        'script Magazzini
        ScriptManager.RegisterStartupScript(UpdatePanel_Menu, UpdatePanel_Menu.GetType(),
                                         String.Format("jQuery_{0}", ComboMagazzini.ClientID), ComboMagazzini.GetJS(), True)


        'script categoria
        Dim StrSelect As New StringBuilder
        StrSelect.AppendLine("$(document).ready(function () { ")
        StrSelect.AppendLine("   $('#" & cmb_Categoria.ClientID & "').combobox();")
        StrSelect.AppendLine("$('#" + txt_DataOperazioneAlGiorno.ClientID + "').datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });")
        'StrSelect.AppendLine("  $('#" & txt_DataOperazioneAlGiorno.ClientID & "').change(function () {  $('#" & BTN_ChangeData_AlGiorno.ClientID & "').click(); });")
        StrSelect.AppendLine("       ('#WaitFrame').hide();   ")
        StrSelect.AppendLine("});")
        ScriptManager.RegisterStartupScript(UpdatePanel_Menu, UpdatePanel_Menu.GetType(),
                                     String.Format("jQuery_{0}", cmb_Categoria.ClientID), StrSelect.ToString, True)


    End Sub


    Private Sub CaricaCentriAziendali()
        ComboCentroAziendale.Piva = ""
        ComboCentroAziendale.CaricaComboCentroAziendale(True)
        ComboCentroAziendale.Valore_Combo = 0
        'ComboCentroAziendale.ddl_CentroAziendale.SelectedIndex = ComboCentroAziendale.ddl_CentroAziendale.Items.IndexOf(ComboCentroAziendale.ddl_CentroAziendale.Items.FindByValue(objParametriAgenda.Sa_Cod))
    End Sub


    Protected Sub BTN_ComboCentroAziendale_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ComboCentroAziendale.Click
        CaricaMagazzini()
    End Sub


    Private Sub CaricaMagazzini()
        'ComboMagazzini.Sa_Cod = objParametriAgenda.Sa_Cod
        ComboMagazzini.Piva = ComboCentroAziendale.Piva
        ComboMagazzini.Sa_Cod = 0 'ComboCentroAziendale.Sa_Cod
        ComboMagazzini.Flag_CodCentroFabbricato = True
        ComboMagazzini.TipoMagazzino = MAGAZZINO
        ComboMagazzini.Flag_GestioneMagazziniImpresaPadre = False
        ComboMagazzini.TestoRiga0 = "Tutti i Magazzini"
        ComboMagazzini.CaricaComboMagazzini()
        'imposto il valore

        ComboMagazzini.Valore_Combo = 0 'objParametriAgenda.Fabbricato
        'ComboMagazzini.ddl_Magazzini.SelectedIndex = ComboMagazzini.ddl_Magazzini.Items.IndexOf(ComboMagazzini.ddl_Magazzini.Items.FindByValue(objParametriAgenda.Fabbricato))

    End Sub


    Protected Sub BTN_Magazzini_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_Magazzini.Click

        ' ComboCentroAziendale.ddl_CentroAziendale.SelectedIndex = ComboCentroAziendale.ddl_CentroAziendale.Items.IndexOf(ComboCentroAziendale.ddl_CentroAziendale.Items.FindByValue(objParametriAgenda.Sa_Cod))

    End Sub


    Protected Sub BTN_ComboCategoria_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ComboCategoria.Click
        TxtCodProdotto.Text = ""
        TxtLotto.Text = ""
        TxtProdotto.Text = ""
    End Sub


    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)
        'Response.Redirect(CType(Master, Agenda).TrovaRedirectCorretto(False, enum_PagineAgenda_2010.Menu, objParametriAgenda))

        Dim strJS As New StringBuilder
        strJS.AppendLine("$(document).ready(function () { ")
        strJS.AppendLine("      window.close(); ")
        strJS.AppendLine(" });")
        ScriptManager.RegisterClientScriptBlock( _
           UpdatePanelscript, _
           UpdatePanelscript.GetType(), _
               String.Format("jQuery_{0}", UpdatePanelscript.ClientID), strJS.ToString, True)
        Exit Sub

    End Sub


    Private Sub LeggiValori(ByRef Mat_Cod As Integer, ByRef Cal_Cod As Integer, ByRef Cod_Progetto As Integer, ByRef Fase_Cod As Integer, ByRef Udm_Cod As Integer, ByRef Elem_Cod As Integer, ByRef Pro_Cod As Integer, ByRef Lotto As String, ByRef piva As String, ByRef sa_cod As Integer, ByRef Fabbricato_Cod As Integer)
        Mat_Cod = 0
        Cal_Cod = 0
        Cod_Progetto = 0
        Fase_Cod = 0
        Udm_Cod = 0
        'Dim Lotto As String = LOTTO_NONDEFINITO 'usato come filtro con like

        Elem_Cod = 0
        Pro_Cod = 0
        Lotto = ""

        piva = ""
        ' Dim sa_cod As Integer = objParametriAgenda.Sa_Cod 'no, potrebbe essere 0=tutti
        sa_cod = 0




        If Me.cmb_Categoria.SelectedIndex = 0 Then
            Elem_Cod = 0
        Else
            Elem_Cod = Me.cmb_Categoria.SelectedValue
        End If

        If TxtCodProdotto.Text <> "" AndAlso IsNumeric(TxtCodProdotto.Text) Then
            Pro_Cod = TxtCodProdotto.Text
        End If

        If Me.TxtLotto.Text <> "" Then
            Lotto = Me.TxtLotto.Text
        End If
    End Sub


    Private Sub ImgBtn_Cerca_Movimenti_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Cerca_Movimenti.Click

        Esegui_Ricerca_Movimenti_Click()

    End Sub


    Private Sub Esegui_Ricerca_Movimenti_Click()
        Dim Mat_Cod As Integer
        Dim Cal_Cod As Integer
        Dim Cod_Progetto As Integer
        Dim Fase_Cod As Integer
        Dim Udm_Cod As Integer
        Dim Elem_Cod As Integer
        Dim Pro_Cod As Integer
        Dim Lotto As String
        Dim piva As String
        Dim sa_cod As Integer
        Dim Fabbricato_Cod As Integer

        Try
            LeggiValori(Mat_Cod, Cal_Cod, Cod_Progetto, Fase_Cod, Udm_Cod, Elem_Cod, Pro_Cod, Lotto, piva, sa_cod, Fabbricato_Cod)

            Esegui_Ricerca_Movimenti(Mat_Cod, Cal_Cod, Cod_Progetto, Fase_Cod, Udm_Cod, Elem_Cod, Pro_Cod, Lotto, piva, sa_cod, Fabbricato_Cod)

        Catch ex As Exception

            LabelRes.Text = ex.Message

        End Try

    End Sub



    Private Sub Esegui_Ricerca_Movimenti(ByRef Mat_Cod As Integer, ByRef Cal_Cod As Integer, ByRef Cod_Progetto As Integer, ByRef Fase_Cod As Integer, ByRef Udm_Cod As Integer, ByRef Elem_Cod As Integer, ByRef Pro_Cod As Integer, ByRef Lotto As String, ByRef piva As String, ByRef sa_cod As Integer, ByRef Fabbricato_Cod As Integer)

        Dim ListaImpRaccolti As List(Of Impianto)
        Dim Mat_Cod_Raccolto As Integer
        Dim Mat_Des_Raccolto As String

        Dim DtMovimenti As New DataTable
        'la piva non è obbligatoria ma consigliata

        Dim forno_piva As String = ""
        Dim forno_sa_cod As Integer = 0
        Dim forno_fabbricato_cod As Integer = 0
        Dim data_inizio_cura As DateTime = AGRODATAINIZIO

        Dim Rintraccio As Importazioni_OPTA.Rintraccio
        Dim res As Boolean = Rintraccio.Esegui_Ricerca_Movimenti(Mat_Cod, Cal_Cod, Cod_Progetto, Fase_Cod, Udm_Cod, Elem_Cod,
                                                                           Pro_Cod, Lotto, piva, sa_cod, Fabbricato_Cod,
                                                                           ListaImpRaccolti, Mat_Cod_Raccolto, Mat_Des_Raccolto,
                                                                       True, DtMovimenti, objParametri_Server,
                                                                       forno_piva, forno_sa_cod, forno_fabbricato_cod, data_inizio_cura)

        GridView_Movimenti.DataSource = FinalizzaDtMovimenti(DtMovimenti)
        GridView_Movimenti.DataBind()

        If IsNothing(DtMovimenti) Or DtMovimenti.Rows.Count = 0 Then
            LabelRes.Text = "Nessun movimento"
        End If
        'For i = 0 To Me.GridView_Movimenti.Rows().Count - 1
        '    If Me.GridView_Movimenti.Rows(i).Cells(30).Text = enum_Agenda_Causali.CARICO Then
        '        Me.GridView_Movimenti.Rows(i).Cells(19).BackColor = Color.MediumSpringGreen
        '    Else
        '        Me.GridView_Movimenti.Rows(i).Cells(19).BackColor = Color.Tomato
        '    End If
        'Next


    End Sub

    Public Function FinalizzaDtMovimenti(ByRef DtMovimenti As DataTable) As DataTable

        Dim Dt As DataTable = inizializzaDtMovimenti()
        Dim dr As DataRow
        Dim objCatMag As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
        Dim objMat As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim objUdm As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
        Dim Descrizione As String
        If Not IsNothing(DtMovimenti) Then
            For i = 0 To DtMovimenti.Rows.Count - 1



                dr = Dt.NewRow

                dr.Item("DescrizioneMovimento") = DtMovimenti.Rows(i).Item("DescrizioneMovimento")

                dr.Item("Piva") = DtMovimenti.Rows(i).Item("Piva")
                dr.Item("Sa_Cod") = DtMovimenti.Rows(i).Item("Sa_Cod")
                dr.Item("Sa_Nome") = New AgronicaCoreAnagrafeDAL.CentriAziendali_Read().SaNome_from_SaCod(DtMovimenti.Rows(i).Item("Piva"), DtMovimenti.Rows(i).Item("Sa_Cod"), objParametri_Server)
                dr.Item("Fabbricato_Cod") = DtMovimenti.Rows(i).Item("Id_Destinazione")
                dr.Item("Fabbricato_Des") = New AgronicaCoreAnagrafeDAL.Fabbricati_R().FabbricatoDes_from_FabbricatoCod(DtMovimenti.Rows(i).Item("Piva"), DtMovimenti.Rows(i).Item("Sa_Cod"), DtMovimenti.Rows(i).Item("Id_Destinazione"), objParametri_Server)

                dr.Item("Cat_Cod") = IIf(Not IsDBNull(DtMovimenti.Rows(i).Item("Elem_Cod")), DtMovimenti.Rows(i).Item("Elem_Cod"), 0)
                dr.Item("Cat_Des") = objCatMag.NomeComune_from_ElemCod(DtMovimenti.Rows(i).Item("Elem_Cod"), objParametri_Server)
                dr.Item("Pro_Cod") = IIf(Not IsDBNull(DtMovimenti.Rows(i).Item("Pro_Cod")), DtMovimenti.Rows(i).Item("Pro_Cod"), 0)
                dr.Item("Mat_Cod") = IIf(Not IsDBNull(DtMovimenti.Rows(i).Item("Mat_Cod")), DtMovimenti.Rows(i).Item("Mat_Cod"), 0)

                Dim Cod_Articolo_ As String
                Select Case dr.Item("Mat_Cod")
                    Case 0
                        dr.Item("Pro_Des") = objCatMag.ProDes_from_ProCod(DtMovimenti.Rows(i).Item("Elem_Cod"), DtMovimenti.Rows(i).Item("Pro_Cod"), objParametri_Server)
                    Case Else
                        dr.Item("Pro_Des") = objMat.MatDes_from_MatCod("", DtMovimenti.Rows(i).Item("Elem_Cod"), DtMovimenti.Rows(i).Item("Mat_Cod"), "", Cod_Articolo_, "", objParametri_Server)
                End Select

                'leggo il parametro di qualità
                Dim objIndice As New AgronicaCoreMetaSchemaDAL.IndiciMaturitaxSpecie_R
                Dim veg_cod As Integer = 0
                Dim o As New AgronicaCoreAnagrafeDAL.Materie_Prime_R()
                o.VegCod_CulCod_from_MatCod(CStr(""), DtMovimenti.Rows(i).Item("Elem_Cod"), DtMovimenti.Rows(i).Item("Mat_Cod"), veg_cod, 0, objParametri_Server)
                Dim DtIndici = objIndice.Leggi_ParametroQualita(veg_cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                dr.Item("Lotto_Int") = objProgetto.ProgettoNome_from_ProgettoCod(DtMovimenti.Rows(i).Item("Cod_Progetto"), Nothing, objParametri_Server)
                dr.Item("Lotto_Acc") = IIf(Not IsDBNull(DtMovimenti.Rows(i).Item("Lotto")), DtMovimenti.Rows(i).Item("Lotto"), "")
                dr.Item("Cal_Cod") = IIf(Not IsDBNull(DtMovimenti.Rows(i).Item("Cal_Cod")), DtMovimenti.Rows(i).Item("Cal_Cod"), 0)
                'altrimenti mi carica il calibro A per tutte le categorie di magazzino
                If dr.Item("Cal_Cod") > 0 Then
                    dr.Item("Param_Des") = "Calibro"
                    Dim objCalibri As New AgronicaCoreMetaSchemaDAL.CalibriFrutti_R
                    dr.Item("Cal_Des") = objCalibri.CalDes_from_CalCod(dr.Item("Cal_Cod"), objParametri_Server)
                    objCalibri = Nothing
                ElseIf dr.Item("Cal_Cod") < 0 Then
                    If Not DtIndici Is Nothing AndAlso DtIndici.Rows.Count > 0 Then
                        dr.Item("Param_Des") = DtIndici.Rows(0).Item("Ind_Mat_Des")
                        dr.Item("Cal_Des") = -dr.Item("Cal_Cod")
                        DtIndici = Nothing
                    End If
                End If
                Dim simbolo As String = ""
                dr.Item("Udm_Cod") = IIf(Not IsDBNull(DtMovimenti.Rows(i).Item("Udm_Cod")), DtMovimenti.Rows(i).Item("Udm_Cod"), 0)
                dr.Item("Udm_Des") = objUdm.UdmDes_from_UdmCod(DtMovimenti.Rows(i).Item("Udm_Cod"), simbolo, objParametri_Server)
                Dim Qta_Dest As Decimal = 0
                If Not IsDBNull(DtMovimenti.Rows(i).Item("Qta")) AndAlso IsNumeric(Not IsDBNull(DtMovimenti.Rows(i).Item("Qta"))) Then
                    Qta_Dest = CDbl(DtMovimenti.Rows(i).Item("Qta"))
                    Qta_Dest = Math.Round(Qta_Dest, 4, MidpointRounding.AwayFromZero)
                End If
                dr.Item("Qta_Dest") = Qta_Dest
                dr.Item("Qta") = Qta_Dest
                dr.Item("Data") = IIf(Not IsDBNull(DtMovimenti.Rows(i).Item("Data_Movimento")), CDate(DtMovimenti.Rows(i).Item("Data_Movimento")).ToShortDateString, "")
                dr.Item("Ora") = IIf(Not IsDBNull(DtMovimenti.Rows(i).Item("Ora")), CDate(DtMovimenti.Rows(i).Item("Ora")).ToShortTimeString, "")
                dr.Item("Cau_Mov") = IIf(Not IsDBNull(DtMovimenti.Rows(i).Item("Cau_Mov")), DtMovimenti.Rows(i).Item("Cau_Mov"), 0)
                dr.Item("Cau_Des") = dr.Item("Cau_Mov")
                dr.Item("Id_Agenda") = DtMovimenti.Rows(i).Item("Id_Agenda")
                dr.Item("Blocco_Flag") = DtMovimenti.Rows(i).Item("Blocco_Flag")

                dr.Item("Fase_Cod") = DtMovimenti.Rows(i).Item("Fase_Cod")


                Dim Lav_Cod As Integer = CInt(DtMovimenti.Rows(i).Item("Lav_Cod"))
                dr.Item("Lav_Cod") = Lav_Cod
                dr.Item("Lav_Des") = New AgronicaCoreMetaSchemaDAL.Operazioni_R().LavorazioneDes_from_LavorazioneCod(Lav_Cod, objParametri_Server)
                dr.Item("Dettagli") = DtMovimenti.Rows(i).Item("Des_Lib")

                Dt.Rows.Add(dr)

            Next
        End If

        Return Dt
    End Function

    Public Function inizializzaDtMovimenti() As DataTable
        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))
        Dt.Columns.Add(New DataColumn("Fabbricato_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Fabbricato_Des", GetType(String)))


        Dt.Columns.Add(New DataColumn("Cat_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cat_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Pro_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Pro_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Mat_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Lotto_Int", GetType(String)))
        Dt.Columns.Add(New DataColumn("Lotto_Acc", GetType(String)))
        Dt.Columns.Add(New DataColumn("Param_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cal_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cal_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Qta", GetType(String)))
        Dt.Columns.Add(New DataColumn("Qta_Dest", GetType(String)))
        Dt.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(String)))
        Dt.Columns.Add(New DataColumn("Totale", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cod_Progetto", GetType(String)))
        Dt.Columns.Add(New DataColumn("Qta_Mag", GetType(String)))
        Dt.Columns.Add(New DataColumn("Fase_Cod", GetType(String)))

        Dt.Columns.Add(New DataColumn("Data", GetType(String)))
        Dt.Columns.Add(New DataColumn("Ora", GetType(String)))
        Dt.Columns.Add(New DataColumn("Lav_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dettagli", GetType(String)))

        Dt.Columns.Add(New DataColumn("Id_Agenda", GetType(String)))
        Dt.Columns.Add(New DataColumn("Lav_Cod", GetType(String)))

        Dt.Columns.Add(New DataColumn("Cau_Mov", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cau_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Blocco_Flag", GetType(String)))
        Dt.Columns.Add(New DataColumn("Info", GetType(String)))

        Dt.Columns.Add(New DataColumn("DescrizioneMovimento", GetType(String)))

        Return Dt

    End Function


    Private Sub GridView_Movimenti_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView_Movimenti.RowCommand

    End Sub




End Class