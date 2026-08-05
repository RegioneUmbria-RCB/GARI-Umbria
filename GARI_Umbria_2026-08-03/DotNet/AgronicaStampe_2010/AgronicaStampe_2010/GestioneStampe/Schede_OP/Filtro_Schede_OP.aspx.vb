Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreUtility

Public Class Filtro_Schede_OP
    Inherits System.Web.UI.Page

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        'Tolgo la pagina dalla cache
        Response.Expires = 0

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '#################################################################################
        '#####  Recupero i dati dalla QueryString 
        '#################################################################################

        'If Not (Request.QueryString("p")) Is Nothing Then
        '    QS_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
        '                                 AgroKey_EncoderDecoder, _
        '                                 Server)
        'End If
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '=====================================================
        '----- Inizializzo i controlli
        '=====================================================

        If Not Me.IsPostBack Then

            '==========================================
            '===== Pagina caricata per la prima volta
            '==========================================

        Else

            '==========================================
            '===== Pagina ricaricata in POSTBACK
            '==========================================

            'Evito di re-inizializzare i controlli
            Exit Sub


        End If

        'If Not QS_Piva Is Nothing Then
        '    ViewState("piva") = QS_Piva
        'End If

        Me.Txt_Anno.Text = Now.Year

        Me.TxtValiditaInizio.Text = "01/01/" & Now.Year
        Me.TxtValiditaFine.Text = "31/12/" & Now.Year

        AgronicaCoreUtility.CaricaListControl.Capitolato_Privato(ComboCapitolato, _
                                                                          True, "", "0", _
                                                                          "", _
                                                                          0, 0, _
                                                                          "", "", _
                                                                          objParametri_Server)

    End Sub

    '###########################################################################
    Private Sub Carica_Imprese()

        Dim FiltroSQL As String

        If Txt_RagioneSociale.Text <> "" Then
            FiltroSQL &= " AND Imprese.rag_soc LIKE '%" + Agro_SQL_SaveText(Txt_RagioneSociale.Text) + "%' "
        End If

        If Me.Txt_PIVA.Text <> "" And Me.Txt_PIVA.Text.Length = 11 Then
            FiltroSQL &= " AND Imprese.Piva = '" + Agro_SQL_SaveText(Me.Txt_PIVA.Text) + "' "
        End If

        If Me.Txt_Codice.Text <> "" Then
            FiltroSQL &= " AND (Imprese_Codici.id_cod = 1033"
            FiltroSQL &= " AND Imprese_Codici.val_cod = '" + Agro_SQL_SaveText(Txt_Codice.Text) + "') "
        End If

        Dim ClassJoin As New JoinFiltrone
        Dim classFiltrone As New AgronicaCoreUtility.Filtrone
        'se ha un filtro
        classFiltrone.ImpostaVariabiliJOIN_xFiltroUtente(FiltroSQL, ClassJoin)

        'Per il momento imposto sempre a true perchè può capitare
        'che il filtro associato all'utente vada a controllare il campo Padre e/o Foglia
        '(di GerarchiaImprese), ma non essendo specificata la tabella GerarchiaImprese
        'prima del nome del campo, la funzione ImpostaVariabiliJOIN_xFiltroUtente non la trova e non imposta il join
        ClassJoin.bGerarchiaImprese = True

        Dim Dt_Imprese As DataTable
        'TipoSelect = TipoSelect_FiltroneSuperNova_from_TipoFiltroImprese(enTipoFiltro)
        Dt_Imprese = classFiltrone.CreaDTFiltrone(objParametri_Server, _
                                                FiltroSQL, _
                                                2, _
                                                "ORDER BY Imprese.rag_soc", _
                                                ClassJoin)

        Dim NomiChiavi(0) As String
        NomiChiavi(0) = "piva"

        Me.DataGrid_Imprese.DataSource = Dt_Imprese
        Me.DataGrid_Imprese.DataKeyNames = NomiChiavi
        Me.DataGrid_Imprese.DataBind()

        Me.DataGrid_Imprese.Visible = True

    End Sub


    Protected Sub Btn_CercaImpresa_Click(sender As Object, e As EventArgs) Handles Btn_CercaImpresa.Click

        For i = 0 To DataGrid_Imprese.Rows.Count - 1
            DataGrid_Imprese.Rows(i).BackColor = Drawing.Color.Transparent
        Next

        Carica_Imprese()

    End Sub

    Sub DataGrid_Imprese_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)

        For i = 0 To DataGrid_Imprese.Rows.Count - 1
            DataGrid_Imprese.Rows(i).BackColor = Drawing.Color.Transparent
        Next

        Dim DataInizio As Date = AGRODATAINIZIO
        Dim DataFine As Date = AGRODATAFINE

        If IsDate(TxtValiditaInizio.Text) Then
            DataInizio = CDate(TxtValiditaInizio.Text)
        End If
        If IsDate(TxtValiditaFine.Text) Then
            DataFine = CDate(TxtValiditaFine.Text)
        End If

        If e.CommandName = "Select" Then

            Dim Piva As String

            Dim index As Integer = Convert.ToInt32(e.CommandArgument)
            Dim row As GridViewRow = DataGrid_Imprese.Rows(index)

            row.BackColor = Drawing.Color.Orange

            Piva = row.Cells(0).Text

            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.Centri_Aziendali(ComboCentri, True, "", "xxxxxxxxxxx/-1", Piva, True, 1, "", "", objParametri_Server)

            clc.TutteSpecieColtivate_3_Data_Da_A(ComboSpecie,
                                                      True, "", "X",
                                                      Piva,
                                                      0,
                                                      DataInizio, DataFine,
                                                      True,
                                                      "", "",
                                                      objParametri_Server,
                                                      True)

            Carica_Impianti(Piva)

            ''Salvo la Partita IVA nella variabile di sessione
            Session("PartitaIVA") = Piva

        End If

    End Sub

    '###########################################################################
    'questa query non legge i terreni nudi (è giusto per questo contesto di report)
    Private Sub Carica_Impianti(ByVal Piva As String)

        Dim stbQuery As New System.Text.StringBuilder
        Dim Dt As New DataTable

        Dim i As Integer

        Dim DataInizio As Date = AGRODATAINIZIO
        Dim DataFine As Date = AGRODATAFINE

        Dim ArrayCentro() As String

        Dt.Columns.Add(New DataColumn("piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("appezza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("id_reg", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("rag_soc", GetType(String)))
        Dt.Columns.Add(New DataColumn("sa_nome", GetType(String)))
        Dt.Columns.Add(New DataColumn("app_nome", GetType(String)))
        Dt.Columns.Add(New DataColumn("veg_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("cul_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("veg_des", GetType(String)))
        Dt.Columns.Add(New DataColumn("cul_des", GetType(String)))
        Dt.Columns.Add(New DataColumn("sup_imp", GetType(String)))

        Dt.Columns.Add(New DataColumn("grfi_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("grfi_des", GetType(String)))
        Dt.Columns.Add(New DataColumn("grva_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("grva_des", GetType(String)))

        Dt.Columns.Add(New DataColumn("Capitolato_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Capitolato_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dettaglio_Specie_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dettaglio_Specie_Des", GetType(String)))

        'controllo validità impianto.....

        If IsDate(TxtValiditaInizio.Text) Then
            DataInizio = CDate(TxtValiditaInizio.Text)
        End If
        If IsDate(TxtValiditaFine.Text) Then
            DataFine = CDate(TxtValiditaFine.Text)
        End If


        stbQuery.Length = 0
        stbQuery.Append(" SELECT  DISTINCT Reg_Impianti.PIVA, Reg_Impianti.SA_COD, Reg_Impianti.APPEZZA, Reg_Impianti.ID_REG,  ")
        stbQuery.Append(" Imprese.rag_soc, Centri_Aziendali.sa_nome, Appezzamento.APP_NOME, ")
        stbQuery.Append(" SpecieVegetali.Veg_Cod, Cultivar.Cul_Cod, SpecieVegetali.Veg_Des, Cultivar.Cul_Des, Reg_Impianti.Sup_Imp, ")
        stbQuery.Append(" GruppoFinalita.Grfi_Cod, GruppoFinalita.Grfi_Des, Reg_Impianti.GRVA_Cod_VEG AS Grva_Cod, ISNULL(GruppoVarietale.Grva_Des,'') AS Grva_Des, ")

        stbQuery.Append("ISNULL((SELECT TOP 1 val_cod FROM Reg_Impianti_Codici ")
        stbQuery.Append("        WHERE Reg_Impianti_Codici.Piva = Imprese_Progetti.PIVA ")
        stbQuery.Append("	     AND Reg_Impianti_Codici.sa_cod = Imprese_Progetti.sa_cod ")
        stbQuery.Append("	     AND Reg_Impianti_Codici.appezza = Imprese_Progetti.APPEZZA ")
        stbQuery.Append("	     AND Reg_Impianti_Codici.id_reg = Imprese_Progetti.ID_REG ")
        stbQuery.Append("	     AND Reg_Impianti_Codici.Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
        stbQuery.Append("	     AND Reg_Impianti_Codici.id_cod = 1093 ")
        stbQuery.Append("        AND Imprese_Progetti.validita_inizio <= " + Agro_SQL_SaveDate(DataFine) + " ")
        stbQuery.Append("        AND Imprese_Progetti.validita_fine >= " + Agro_SQL_SaveDate(DataInizio) + " ")
        stbQuery.Append("	     ), '') AS Capitolato_Cod, ")
        stbQuery.Append("'' AS Capitolato_Des, ")

        stbQuery.Append("ISNULL((SELECT TOP 1 val_cod FROM Reg_Impianti_Codici ")
        stbQuery.Append("        WHERE Reg_Impianti_Codici.Piva = Reg_Impianti.PIVA ")
        stbQuery.Append("	     AND Reg_Impianti_Codici.sa_cod = Reg_Impianti.sa_cod ")
        stbQuery.Append("	     AND Reg_Impianti_Codici.appezza = Reg_Impianti.APPEZZA ")
        stbQuery.Append("	     AND Reg_Impianti_Codici.id_reg = Reg_Impianti.ID_REG ")
        stbQuery.Append("	     AND Reg_Impianti_Codici.id_cod = 1108), '') AS Dettaglio_Specie_Cod, ")
        stbQuery.Append(" '' AS Dettaglio_Specie_Des ")

        stbQuery.Append(" , YEAR(Reg_Impianti.Validita_Inizio) as Anno_Imp ")

        stbQuery.Append(" FROM Imprese INNER JOIN ")
        stbQuery.Append(" Centri_Aziendali ON Imprese.PIVA = Centri_Aziendali.PIVA INNER JOIN")
        stbQuery.Append(" Appezzamento ON Centri_Aziendali.PIVA = Appezzamento.PIVA AND Centri_Aziendali.sa_cod = Appezzamento.SA_COD INNER JOIN")
        stbQuery.Append(" Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND Appezzamento.APPEZZA = Reg_Impianti.APPEZZA INNER JOIN ")
        stbQuery.Append(" Imprese_Progetti ON Imprese_Progetti.PIVA = Reg_Impianti.PIVA AND Imprese_Progetti.SA_COD = Reg_Impianti.SA_COD AND  Imprese_Progetti.APPEZZA = Reg_Impianti.APPEZZA AND  Imprese_Progetti.id_reg = Reg_Impianti.id_reg INNER JOIN ")
        stbQuery.Append(" Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod INNER JOIN")
        stbQuery.Append(" SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN")
        stbQuery.Append(" UtentiXImprese ON Imprese.PIVA = UtentiXImprese.PIVA INNER JOIN")
        stbQuery.Append(" GruppoFinalita ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN")
        stbQuery.Append(" GruppoVarietale ON Reg_Impianti.GRVA_Cod_VEG = GruppoVarietale.Grva_Cod LEFT OUTER JOIN")
        stbQuery.Append(" Reg_Impianti_Codici ON Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod AND ")
        stbQuery.Append(" Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza And Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg ")

        stbQuery.Append(" WHERE UtentiXImprese.[USER] = '" + Agro_SQL_SaveText(CStr(Session("ASG_SuperUser_CodFiscale"))) + "' ")
        stbQuery.Append(" AND Imprese.Piva = '" + Agro_SQL_SaveText(Piva) + "' ")


        If ComboCentri.Items.Count > 0 Then
            If ComboCentri.SelectedItem.Text <> "" Then
                ArrayCentro = Split(ComboCentri.SelectedItem.Value, "/")
                If Not ArrayCentro Is Nothing AndAlso ArrayCentro.Length > 0 Then
                    stbQuery.Append(" AND Reg_Impianti.Sa_Cod = " + Agro_SQL_SaveNum(ArrayCentro(1)) + " ")
                End If
            End If
        End If

        If ComboSpecie.Items.Count > 0 Then
            If ComboSpecie.SelectedItem.Text <> "" Then
                stbQuery.Append(" AND SpecieVegetali.Veg_Cod = " + Agro_SQL_SaveNum(ComboSpecie.SelectedItem.Value) + " ")
            End If
        End If

        If ComboVarieta.Items.Count > 0 Then
            If ComboVarieta.SelectedItem.Text <> "" Then
                stbQuery.Append(" AND Reg_Impianti.Cul_Cod = " + Agro_SQL_SaveNum(ComboVarieta.SelectedItem.Value) + " ")
            End If
        End If

        If ComboCapitolato.Items.Count > 0 Then
            If ComboCapitolato.SelectedItem.Text <> "" Then
                stbQuery.Append(" AND (Reg_Impianti_Codici.Id_Cod = 1093 ")
                stbQuery.Append(" AND Reg_Impianti_Codici.Val_Cod = '" + Agro_SQL_SaveText(ComboCapitolato.SelectedItem.Value) + "') ")
            End If
        End If

        stbQuery.Append(" AND Reg_Impianti.validita_inizio <= " + Agro_SQL_SaveDate(DataFine) + " ")
        stbQuery.Append(" AND Reg_Impianti.validita_fine >= " + Agro_SQL_SaveDate(DataInizio) + " ")

        stbQuery.Append(" AND Imprese_Progetti.validita_inizio <= " + Agro_SQL_SaveDate(DataFine) + " ")
        stbQuery.Append(" AND Imprese_Progetti.validita_fine >= " + Agro_SQL_SaveDate(DataInizio) + " ")

        'stbQuery.Append(" ORDER BY rag_soc, sa_nome, APP_NOME, Veg_Des, Cul_Des ")
        'ordinamento richiesto centro,specie,varietà,anno
        stbQuery.Append(" ORDER BY rag_soc , sa_nome , Veg_Des , Cul_Des ,anno_imp ")

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, _
                                stbQuery.ToString, _
                                 "Filtro_Schede_OP.Carica_Impianti")


        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then
            Dim objCac As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
            For i = 0 To Dt.Rows.Count - 1
                If Dt.Rows(i).Item("Dettaglio_Specie_Cod") <> "" Then
                    Dt.Rows(i).Item("Dettaglio_Specie_Des") = objCac.InfoAgg_Des_from_InfoAgg_Cod(Dt.Rows(i).Item("Dettaglio_Specie_Cod"), 2, 0, objParametri_Server)
                End If
                If Dt.Rows(i).Item("Capitolato_Cod") <> "" Then
                    Dt.Rows(i).Item("Capitolato_Des") = objCac.InfoAgg_Des_from_InfoAgg_Cod(Dt.Rows(i).Item("Capitolato_Cod"), 1, 0, objParametri_Server)
                End If
            Next
        End If

        DataGridImpianti.DataSource = Dt
        DataGridImpianti.DataBind()

        If DataGridImpianti.Rows.Count = 0 Then
            AgroMsgBox("Nessun impianto colturale trovato!", Page)
        End If



    End Sub



    Protected Sub Btn_CercaImpianti_Click(sender As Object, e As EventArgs) Handles Btn_CercaImpianti.Click

        CercaImpianti()

    End Sub

    Private Sub CercaImpianti()

        Dim Messaggio As String
        Dim N_Imprese As Integer = 0

        Dim Piva As String = ""
       
        '------------------
        'controlli
        '------------------
        'anno
        If Me.Txt_Anno.Text = "" Then
            Messaggio += "Inserire l'anno di riferimento!" & vbCrLf
        End If

        'impresa
        If Not Session("PartitaIVA") Is Nothing Then
            Piva = Session("PartitaIVA")
        End If

        If Piva = "" Then
            Messaggio += "Selezionare l'Impresa!" & vbCrLf
        End If
     
        If Messaggio <> "" Then
            AgroMsgBox(Messaggio, Page)
            Exit Sub
        End If

        Carica_Impianti(Piva)


    End Sub


    Protected Sub ComboSpecie_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboSpecie.SelectedIndexChanged

        Dim Piva As String = ""
        Dim DataInizio As Date = AGRODATAINIZIO
        Dim DataFine As Date = AGRODATAFINE

        'impresa
        If Not Session("PartitaIVA") Is Nothing Then
            Piva = Session("PartitaIVA")
        End If

        If Piva = "" Then
            AgroMsgBox("Selezionare l'Impresa!", Page)
            Exit Sub
        End If

        If IsDate(TxtValiditaInizio.Text) Then
            DataInizio = CDate(TxtValiditaInizio.Text)
        End If
        If IsDate(TxtValiditaFine.Text) Then
            DataFine = CDate(TxtValiditaFine.Text)
        End If

        If ComboSpecie.Items.Count > 0 Then

            If ComboSpecie.SelectedItem.Text <> "" Then

                AgronicaCoreUtility.CaricaListControl.TutteVarietaColtivate_5(ComboVarieta, _
                                                                True, "", "X", _
                                                                Piva, _
                                                                0, _
                                                                ComboSpecie.SelectedItem.Value, _
                                                                DataInizio, DataFine, _
                                                                "", "", _
                                                                objParametri_Server)
            End If

        End If
    End Sub

    Protected Sub ImgBtnAnnullaTutto_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnAnnullaTutto.Click

        'Dim strJS1 As New StringBuilder
        'strJS1.AppendLine("$(document).ready(function () { ")
        'strJS1.AppendLine("      window.close() ")
        'strJS1.AppendLine(" });")

        Dim strClose As String = "<script language='javascript'> window.close() </script>"
        Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))

    End Sub

    Private Sub Stampa()

        Dim TargetURL As String
        Dim NomePagina As String

        Dim Messaggio As String
        Dim i As Integer
        Dim N_Imprese As Integer = 0
     
        Dim Piva As String
        Dim Rag_Soc As String
        Dim Sa_Cod As String = 0
      
        Dim Descr_Tot As String = ""


        '------------------
        'controlli
        '------------------
        'anno
        If Me.Txt_Anno.Text = "" Then
            Messaggio += "Inserire l'anno di riferimento!" & vbCrLf
        End If

        'impresa
        If Not Session("PartitaIVA") Is Nothing Then
            Piva = Session("PartitaIVA")
        End If

        If Piva = "" Then
            Messaggio += "Selezionare almeno un'Impresa!" & vbCrLf
        End If

        If Messaggio <> "" Then
            AgroMsgBox(Messaggio, Page)
            Exit Sub
        End If
        '-------------------------------------

        Select Case CInt(Me.OptionList_Scheda.SelectedValue)

            Case enum_CodificaStampe.Atto_Notorio
                NomePagina = "AttoNotorio"
                TargetURL = "AttoNotorio/AttoNotorio.aspx"

                RecuperaChiaveImpianti(Messaggio, Sa_Cod, Descr_Tot, True)

                If Messaggio <> "" Then
                    AgroMsgBox(Messaggio, Page)
                    Exit Sub
                End If

            Case enum_CodificaStampe.Atto_Notorio * -1
                NomePagina = "SchedaAutoCertificazione"
                TargetURL = "AttoNotorio_2tipo/SchedaAutoCertificazione.aspx"

                RecuperaChiaveImpianti(Messaggio, Sa_Cod, Descr_Tot, True)

                If Messaggio <> "" Then
                    AgroMsgBox(Messaggio, Page)
                    Exit Sub
                End If

            Case enum_CodificaStampe.Adesione_Etico_Ambientale
                NomePagina = "AdesioneEticoAmbientale"
                TargetURL = "Scheda_OP_Tipo_1/Scheda_OP_Tipo_1.aspx"
            Case enum_CodificaStampe.Tenuta_Scheda_Campagna
                NomePagina = "TenutaSchedaCampagna"
                TargetURL = "Scheda_OP_Tipo_1/Scheda_OP_Tipo_1.aspx"
            Case enum_CodificaStampe.Codice_Condotta
                NomePagina = "CodiceCondotta"
                TargetURL = "Scheda_OP_Tipo_1/Scheda_OP_Tipo_1.aspx"
            Case enum_CodificaStampe.Adesione_DPI
                NomePagina = "AdesioneDPI"
                TargetURL = "Scheda_OP_Tipo_1/Scheda_OP_Tipo_1.aspx"
            Case enum_CodificaStampe.Impegnativa_Eurep
                NomePagina = "ImpegnativaEUREP"
                TargetURL = "Scheda_OP_Tipo_1/Scheda_OP_Tipo_1.aspx"
            Case enum_CodificaStampe.Impegnativa_QC
                NomePagina = "ImpegnativaQC"
                TargetURL = "Scheda_OP_Tipo_1/Scheda_OP_Tipo_1.aspx"
            Case enum_CodificaStampe.Impegnativa_Confusione_Sessuale
                NomePagina = "ImpegnativaConfusioneSessuale"
                TargetURL = "Scheda_OP_Tipo_1/Scheda_OP_Tipo_1.aspx"
            Case enum_CodificaStampe.Allegato_CatastoeValorizzazioni
                NomePagina = "AllegatoCatastoValorizzazioni"
                TargetURL = "Scheda_OP_Tipo_1/Scheda_OP_Tipo_1.aspx"

                RecuperaChiaveImpianti(Messaggio, Sa_Cod, Descr_Tot, True)

                If Messaggio <> "" Then
                    AgroMsgBox(Messaggio, Page)
                    Exit Sub
                End If

            Case enum_CodificaStampe.Mandato_Trasmissione_Telematica_Dati
                NomePagina = "MandatoTrasmissioneTelematicaDati"
                TargetURL = "MandatoTrasmissioneDati/MandatoTrasmissioneDati.aspx"

            Case enum_CodificaStampe.Impegnativa_Orticole_Gest_Annuale
                NomePagina = "ImpegnativaOrticoleGestAnnuale"
                TargetURL = "Scheda_OP_Tipo_2/Scheda_OP_Tipo_2.aspx"
            Case enum_CodificaStampe.Impegnativa_Orticole_Gest_Breve
                NomePagina = "ImpegnativaOrticoleGestBreve"
                TargetURL = "Scheda_OP_Tipo_2/Scheda_OP_Tipo_2.aspx"
            Case enum_CodificaStampe.Impegnativa_Fagiolino_Mercato_Fresco
                NomePagina = "ImpegnativaFagiolinoMercatoFresco"
                TargetURL = "Scheda_OP_Tipo_2/Scheda_OP_Tipo_2.aspx"
            Case enum_CodificaStampe.Impegnativa_Orticole_Industria
                NomePagina = "ImpegnativaOrticoleIndustria"
                TargetURL = "Scheda_OP_Tipo_2/Scheda_OP_Tipo_2.aspx"
            Case enum_CodificaStampe.Impegnativa_Pomodoro_Industria
                NomePagina = "ImpegnativaPomodoroIndustria"
                TargetURL = "Scheda_OP_Tipo_2/Scheda_OP_Tipo_2.aspx"

        End Select

        TargetURL &= "?p=" &
                    Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server) &
                    "&r=" &
                    Stringa_Codifica(Rag_Soc, AgroKey_EncoderDecoder, Server) &
                    "&a=" &
                    Stringa_Codifica(Me.Txt_Anno.Text, AgroKey_EncoderDecoder, Server)

        Select Case CInt(Me.OptionList_Scheda.SelectedValue)
            Case enum_CodificaStampe.Adesione_Etico_Ambientale,
                enum_CodificaStampe.Tenuta_Scheda_Campagna,
                enum_CodificaStampe.Codice_Condotta,
                enum_CodificaStampe.Adesione_DPI,
                enum_CodificaStampe.Impegnativa_Eurep,
                enum_CodificaStampe.Impegnativa_QC,
                enum_CodificaStampe.Impegnativa_Confusione_Sessuale,
                enum_CodificaStampe.Allegato_CatastoeValorizzazioni


                TargetURL &= "&t=" & _
                            Stringa_Codifica(Me.OptionList_Scheda.SelectedValue, AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.Impegnativa_Orticole_Gest_Annuale,
                enum_CodificaStampe.Impegnativa_Orticole_Gest_Breve,
                enum_CodificaStampe.Impegnativa_Fagiolino_Mercato_Fresco,
                enum_CodificaStampe.Impegnativa_Orticole_Industria,
                enum_CodificaStampe.Impegnativa_Pomodoro_Industria

                RecuperaChiaveImpianti(Messaggio, Sa_Cod, Descr_Tot, False)
                Session("DescrImpianti") = Descr_Tot

                If Messaggio <> "" Then
                    AgroMsgBox(Messaggio, Page)
                    Exit Sub
                End If


                TargetURL &= "&s=" &
                            Stringa_Codifica(Sa_Cod, AgroKey_EncoderDecoder, Server) &
                            "&t=" &
                            Stringa_Codifica(Me.OptionList_Scheda.SelectedValue, AgroKey_EncoderDecoder, Server)
                '& _
                '"&des=" & _
                'Stringa_Codifica(Descr_Tot, AgroKey_EncoderDecoder, Server)

        End Select


        'Select Case CInt(Me.OptionList_Scheda.SelectedValue)

        ''-------------------------------------
        ''--------- ATTO NOTORIO --------------
        ''-------------------------------------
        'Case enum_CodificaStampe.Atto_Notorio

        '    NomePagina = "AttoNotorio"

        '    RecuperaChiaveImpianti(Messaggio, Sa_Cod, Descr_Tot, True)

        '    If Messaggio <> "" Then
        '        AgroMsgBox(Messaggio, Page)
        '        Exit Sub
        '    End If

        '    TargetURL = "AttoNotorio/AttoNotorio.aspx" & _
        '                "?p=" & _
        '                Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server) & _
        '                "&r=" & _
        '                Stringa_Codifica(Rag_Soc, AgroKey_EncoderDecoder, Server) & _
        '                "&a=" & _
        '                Stringa_Codifica(Me.Txt_Anno.Text, AgroKey_EncoderDecoder, Server)


        ''-----------------------------------------------
        ''--------- Adesione_Etico_Ambientale -----------
        ''--------- Tenuta_Scheda_Campagna --------------
        ''--------- Codice_Condotta ---------------------
        ''-----------------------------------------------
        'Case enum_CodificaStampe.Adesione_Etico_Ambientale, _
        '     enum_CodificaStampe.Tenuta_Scheda_Campagna, _
        '     enum_CodificaStampe.Codice_Condotta

        '    Select Case CInt(Me.OptionList_Scheda.SelectedValue)
        '        Case enum_CodificaStampe.Adesione_Etico_Ambientale
        '            NomePagina = "AdesioneEticoAmbientale"
        '        Case enum_CodificaStampe.Tenuta_Scheda_Campagna
        '            NomePagina = "TenutaSchedaCampagna"
        '        Case enum_CodificaStampe.Codice_Condotta
        '            NomePagina = "CodiceCondotta"
        '    End Select

        '    TargetURL = "Scheda_OP_Tipo_1/Scheda_OP_Tipo_1.aspx" & _
        '                "?p=" & _
        '                Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server) & _
        '                "&r=" & _
        '                Stringa_Codifica(Rag_Soc, AgroKey_EncoderDecoder, Server) & _
        '                "&a=" & _
        '                Stringa_Codifica(Me.Txt_Anno.Text, AgroKey_EncoderDecoder, Server) & _
        '                "&t=" & _
        '                Stringa_Codifica(Me.OptionList_Scheda.SelectedValue, AgroKey_EncoderDecoder, Server)


        '    '-----------------------------------------------------
        '    '--------- Impegnativa_Confusione_Sessuale -----------
        '    '--------- Impegnativa_Eurep -------------------------
        '    '--------- Impegnativa_QC ----------------------------
        '    '--------- Adesione_DPI ------------------------------
        '    '-----------------------------------------------------
        'Case enum_CodificaStampe.Adesione_DPI, _
        '     enum_CodificaStampe.Impegnativa_Eurep, _
        '     enum_CodificaStampe.Impegnativa_QC, _
        '     enum_CodificaStampe.Impegnativa_Confusione_Sessuale

        '    Select Case CInt(Me.OptionList_Scheda.SelectedValue)
        '        Case enum_CodificaStampe.Adesione_DPI
        '            NomePagina = "AdesioneDPI"
        '        Case enum_CodificaStampe.Impegnativa_Eurep
        '            NomePagina = "ImpegnativaEUREP"
        '        Case enum_CodificaStampe.Impegnativa_QC
        '            NomePagina = "ImpegnativaQC"
        '        Case enum_CodificaStampe.Impegnativa_Confusione_Sessuale
        '            NomePagina = "ImpegnativaConfusioneSessuale"
        '    End Select

        '    RecuperaChiaveImpianti(Messaggio, Sa_Cod, Descr_Tot, True)

        '    If Messaggio <> "" Then
        '        AgroMsgBox(Messaggio, Page)
        '        Exit Sub
        '    End If


        '    TargetURL = "Scheda_OP_Tipo_1/Scheda_OP_Tipo_1.aspx" & _
        '                "?p=" & _
        '                Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server) & _
        '                "&r=" & _
        '                Stringa_Codifica(Rag_Soc, AgroKey_EncoderDecoder, Server) & _
        '                "&a=" & _
        '                Stringa_Codifica(Me.Txt_Anno.Text, AgroKey_EncoderDecoder, Server) & _
        '                "&t=" & _
        '                Stringa_Codifica(Me.OptionList_Scheda.SelectedValue, AgroKey_EncoderDecoder, Server)


        ''----------------------------------------------------------
        ''--------- Mandato_Trasmissione_Telematica_Dati -----------
        ''----------------------------------------------------------
        'Case enum_CodificaStampe.Mandato_Trasmissione_Telematica_Dati

        '    NomePagina = "MandatoTrasmissioneTelematicaDati"

        '    TargetURL = "MandatoTrasmissioneDati/MandatoTrasmissioneDati.aspx" & _
        '                "?p=" & _
        '                Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server) & _
        '                "&r=" & _
        '                Stringa_Codifica(Rag_Soc, AgroKey_EncoderDecoder, Server) & _
        '                "&a=" & _
        '                Stringa_Codifica(Me.Txt_Anno.Text, AgroKey_EncoderDecoder, Server)


        ''--------------------------------------------------------------
        ''--------- Impegnativa_Orticole_Gest_Annuale ------------------
        ''--------- Impegnativa_Orticole_Gest_Breve --------------------
        ''--------- Impegnativa_Fagiolino_Mercato_Fresco ---------------
        ''--------- Impegnativa_Orticole_Industria ---------------------
        ''--------- Impegnativa_Pomodoro_Industria ---------------------
        ''--------------------------------------------------------------
        'Case enum_CodificaStampe.Impegnativa_Orticole_Gest_Annuale, _
        '    enum_CodificaStampe.Impegnativa_Orticole_Gest_Breve, _
        '    enum_CodificaStampe.Impegnativa_Fagiolino_Mercato_Fresco, _
        '    enum_CodificaStampe.Impegnativa_Orticole_Industria, _
        '    enum_CodificaStampe.Impegnativa_Pomodoro_Industria

        '    Select Case CInt(Me.OptionList_Scheda.SelectedValue)
        '        Case enum_CodificaStampe.Impegnativa_Orticole_Gest_Annuale
        '            NomePagina = "ImpegnativaOrticoleGestAnnuale"
        '        Case enum_CodificaStampe.Impegnativa_Orticole_Gest_Breve
        '            NomePagina = "ImpegnativaOrticoleGestBreve"
        '        Case enum_CodificaStampe.Impegnativa_Fagiolino_Mercato_Fresco
        '            NomePagina = "ImpegnativaFagiolinoMercatoFresco"
        '        Case enum_CodificaStampe.Impegnativa_Orticole_Industria
        '            NomePagina = "ImpegnativaOrticoleIndustria"
        '        Case enum_CodificaStampe.Impegnativa_Pomodoro_Industria
        '            NomePagina = "ImpegnativaPomodoroIndustria"
        '    End Select

        '    RecuperaChiaveImpianti(Messaggio, Sa_Cod, Descr_Tot, False)

        '    If Messaggio <> "" Then
        '        AgroMsgBox(Messaggio, Page)
        '        Exit Sub
        '    End If


        '    TargetURL = "Scheda_OP_Tipo_2/Scheda_OP_Tipo_2.aspx" & _
        '                "?p=" & _
        '                Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server) & _
        '                "&s=" & _
        '                Stringa_Codifica(Sa_Cod, AgroKey_EncoderDecoder, Server) & _
        '                "&r=" & _
        '                Stringa_Codifica(Rag_Soc, AgroKey_EncoderDecoder, Server) & _
        '                "&a=" & _
        '                Stringa_Codifica(Me.Txt_Anno.Text, AgroKey_EncoderDecoder, Server) & _
        '                "&t=" & _
        '                Stringa_Codifica(Me.OptionList_Scheda.SelectedValue, AgroKey_EncoderDecoder, Server) & _
        '                "&des=" & _
        '                Stringa_Codifica(Descr_Tot, AgroKey_EncoderDecoder, Server)

        'End Select


        Dim strOpen As String = "<script language='javascript'>" & vbNewLine & _
               "window.open('" & TargetURL & "'," & _
               "'" & NomePagina & "','height=600,width=1000,menubar=yes,scrollbars=yes,top=0,left=0,resizable=yes');" & vbNewLine & _
               "</script>"

        'apro la finestra...
        Me.FindControl("Form1").Controls.Add(New LiteralControl(strOpen))



    End Sub

    '###########################################################################
    Private Sub RecuperaChiaveImpianti(ByRef Messaggio As String, _
                                        ByRef Sa_Cod As Integer, _
                                       ByRef Descr_Tot As String, _
                                       ByVal Flag_AggiungiAND As Boolean)

        Dim i As Integer
        Dim N_Impianti As Integer = 0
        Dim Piva As String
        Dim Appezza As String
        Dim Id_Reg As String
        Dim strFiltroImpianti As String = ""
        Dim strFiltroImpianto As String = ""
        Dim Descr As String

        'verifico di aver scelto almeno 1 impianto
        'preparo il filtro x la stampa
        For i = 0 To DataGridImpianti.Rows.Count - 1

            If CType(DataGridImpianti.Rows(i).FindControl("ChkSeleziona"), CheckBox).Checked Then

                Piva = Me.DataGridImpianti.Rows(i).Cells(1).Text
                Sa_Cod = Me.DataGridImpianti.Rows(i).Cells(2).Text
                Appezza = Me.DataGridImpianti.Rows(i).Cells(3).Text
                Id_Reg = Me.DataGridImpianti.Rows(i).Cells(4).Text

                strFiltroImpianto = " (Reg_Impianti.PIVA='" + Piva + "' " + _
                                    " AND Reg_Impianti.SA_COD=" + CStr(Sa_Cod) + _
                                    " AND Reg_Impianti.APPEZZA=" + CStr(Appezza) + _
                                    " AND Reg_Impianti.ID_REG=" + CStr(Id_Reg) + _
                                    " ) OR"

                strFiltroImpianti = strFiltroImpianti + strFiltroImpianto


                Select Case CInt(Me.OptionList_Scheda.SelectedValue)

                    Case enum_CodificaStampe.Impegnativa_Orticole_Gest_Annuale, _
                                enum_CodificaStampe.Impegnativa_Orticole_Gest_Breve, _
                                enum_CodificaStampe.Impegnativa_Pomodoro_Industria

                        Descr = DataGridImpianti.Rows(i).Cells(12).Text & " " & _
                                IIf(DataGridImpianti.Rows(i).Cells(14).Text = "&nbsp;", "", DataGridImpianti.Rows(i).Cells(14).Text) & " " & _
                                IIf(DataGridImpianti.Rows(i).Cells(17).Text = "&nbsp;", "", DataGridImpianti.Rows(i).Cells(17).Text) & " " & _
                                IIf(DataGridImpianti.Rows(i).Cells(18).Text = "&nbsp;", "", DataGridImpianti.Rows(i).Cells(18).Text)

                        If InStr(Descr_Tot, Descr) = 0 Then
                            Descr_Tot &= Descr & ", "
                        End If

                    Case enum_CodificaStampe.Impegnativa_Fagiolino_Mercato_Fresco, _
                            enum_CodificaStampe.Impegnativa_Orticole_Industria

                        Descr = DataGridImpianti.Rows(i).Cells(12).Text & " " & _
                                IIf(DataGridImpianti.Rows(i).Cells(14).Text = "&nbsp;", "", DataGridImpianti.Rows(i).Cells(14).Text) & " " & _
                                IIf(DataGridImpianti.Rows(i).Cells(15).Text = "&nbsp;", "", DataGridImpianti.Rows(i).Cells(15).Text) & " " & _
                                IIf(DataGridImpianti.Rows(i).Cells(17).Text = "&nbsp;", "", DataGridImpianti.Rows(i).Cells(17).Text) & " " & _
                                IIf(DataGridImpianti.Rows(i).Cells(18).Text = "&nbsp;", "", DataGridImpianti.Rows(i).Cells(18).Text)

                        If InStr(Descr_Tot, Descr) = 0 Then
                            Descr_Tot &= Descr & ", "
                        End If

                End Select


                N_Impianti += 1

            End If

        Next


        Select Case N_Impianti
            Case 0
                Messaggio += "Selezionare almeno un Impianto!" & vbCrLf

            Case Else

                If strFiltroImpianti <> "" Then
                    'tolgo l'ultimo OR
                    strFiltroImpianti = Left(strFiltroImpianti, strFiltroImpianti.Length - 2)
                    If Flag_AggiungiAND = True Then
                        'aggiungo l'AND
                        strFiltroImpianti = " AND (" + strFiltroImpianti + ")"
                    Else
                        strFiltroImpianti = " (" + strFiltroImpianti + ")"
                    End If
                End If

                If Descr_Tot <> "" Then
                    Descr_Tot = Left(Descr_Tot, Descr_Tot.Length - 2)
                End If

                Session("strParametri") = strFiltroImpianti

        End Select


    End Sub

    Protected Sub Btn_Stampa_Click(sender As Object, e As EventArgs) Handles Btn_Stampa.Click

        Stampa()

    End Sub

End Class