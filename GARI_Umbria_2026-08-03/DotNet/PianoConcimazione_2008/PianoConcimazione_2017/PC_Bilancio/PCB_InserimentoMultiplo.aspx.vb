Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreUtility.CaricaListControl
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreVarieBIZ
Imports AgronicaCorePianoConcimazioneBIZ

Public Class PCB_InserimentoMultiplo
    Inherits System.Web.UI.Page

    '----- objParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Dim Qs_Tipo As enum_PianoConcimazione_Tipo
    'Dim Qs_Tipo_Aggregazione As enum_Tipo_Aggregazione

    Dim Log_Errori As String = ""
    Dim Nome_Documento As String = "PianoConcimazione_Massivo"

    Public Master_Concimaz As MasterConcimazione

    Private Enum enum_Tipo_Aggregazione
        Azienda_Centro_Specie_Finalita = 0
        Azienda_Centro_Specie_Finalita_Vulnerabilita = 1
        Azienda_Centro_Specie_Finalita_Vulnerabilita_Analisi = 2
    End Enum

    Public Class StatisticaAnalisi
        Public Property Analisi As Integer
        Public Property Ricorrenza As Integer
        Public Property Righe As List(Of Integer)
    End Class

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        AddHandler Master.ImgBtnAnnullaTutto.Click, AddressOf Me.AnnullaTutto

        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("Messaggi/AccessoNegato.htm")
        End If

        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                            Session("ASG_Utente_Username"),
                            Session("ASG_IdServizio"),
                            enum_Security_Attivita.SupportoDecisioni_PianoConcimazioneMassivo,
                            enum_Security_Operazione.Modifica,
                            Date.Now,
                            "",
                            objParametri_Utenti)

        If UtenteAbilitato = False Then
            Response.Redirect("Messaggi/AccessoNegato.htm")
        End If

        Session.Timeout = 180


        If Not IsNothing(Request.QueryString("tipo")) Then
            Qs_Tipo = Stringa_Decodifica(Request.QueryString("tipo").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Tipo = enum_PianoConcimazione_Tipo.Bilancio
        End If

        ''default true x compatibilita col precedente (multi-multi)
        'If Not IsNothing(Request.QueryString("tipo_aggregazione")) Then
        '    Qs_Tipo_Aggregazione = Stringa_Decodifica(Request.QueryString("tipo_aggregazione").ToString,
        '                                        AgroKey_EncoderDecoder,
        '                                        Server)
        'Else
        '    Qs_Tipo_Aggregazione = enum_Tipo_Aggregazione.Azienda_Centro_Specie_Finalita
        'End If

        If Not Page.IsPostBack Then

            Select Case Qs_Tipo
                Case enum_PianoConcimazione_Tipo.Bilancio
                    Master.Lbl_Titolo.Text = "PIANO CONCIMAZIONE (Metodo Bilancio)"
                Case Else
                    Master.Lbl_Titolo.Text = "PIANO CONCIMAZIONE (Metodo Schede)"
            End Select

            Txt_Descrizione.Text = "Piano Concimazione " & Today.Year.ToString
            Txt_ValiditaInizio.Text = "01/01/" & Today.Year.ToString
            Txt_ValiditaFine.Text = "31/12/" & Today.Year.ToString
            Txt_Anno.Text = Today.Year.ToString

        Else
            'output.Write("Postback has occured")
            Exit Sub
        End If

        AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PUA_Regolamento_WS(ddlRegolamento, False, "", "", enum_PUARegolamenti_Tipo.PianoComcimazione, 0, "", " Ordine desc ")

        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim Regolamento_Def As String = ""
        Regolamento_Def = objImpost.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_Nitrati_RegolamentoPC_Default,
                                             objParametri_Utenti)

        If IsNumeric(Regolamento_Def) Then
            ddlRegolamento.SelectedIndex = ddlRegolamento.Items.IndexOf(ddlRegolamento.Items.FindByValue(Regolamento_Def))
        End If

        Dim Regolamento_Cod As Integer = 0
        Regolamento_Cod = CInt(ddlRegolamento.SelectedValue)

        Txt_Descrizione.Text = ddlRegolamento.SelectedItem.Text

        Dim DataInizio, DataFine As Date
        Dim Anno As Decimal = 0
        Anno = AgronicaCoreDataProvider.UtilityProvider.Numeri_from_StringaAlfaNumerica(ddlRegolamento.SelectedItem.Text)
        If Anno > 0 Then
            Txt_Anno.Text = Anno.ToString
            objImpost.AnnataAgraria(CDate("01/01/" & Anno.ToString), DataInizio, DataFine, objParametri_Utenti)
            Txt_ValiditaInizio.Text = DataInizio
            Txt_ValiditaFine.Text = DataFine
        End If

        'CmbAggregazione.SelectedIndex = CmbAggregazione.Items.IndexOf(CmbAggregazione.Items.FindByValue(Qs_Tipo_Aggregazione))

        Crea_Dt_Impianti()

        Dim objParametri As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
        objParametri.Leggi()

        Dim ListaPive As New List(Of String)

        'caso multi azienda multi specie non visualizzo la scelta di aggregare ne il catasto per ora per fretta (voluto da fabrizio)
        If Not IsNothing(objParametri) AndAlso Not IsNothing(objParametri.ListaImpianti) Then
            Carica_Impianti_Sessione(objParametri.ListaImpianti)
            Div_Aggregazione.Attributes.Add("style", "display:none")
            grdSpecie.Columns(3).HeaderStyle.CssClass = "displaynone"
            grdSpecie.Columns(3).ItemStyle.CssClass = "displaynone"
            For Each impianto In objParametri.ListaImpianti
                If Not ListaPive.Contains(impianto.Piva) Then
                    ListaPive.Add(impianto.Piva)
                End If
            Next
            'Crea_Dt_Analisi(ListaPive)
        Else
            ListaPive.Add(objParametri.Piva)
            'Crea_Dt_Analisi(ListaPive)
            'Carica_Impianti_Impresa(objParametri.Piva, 0, 0)
        End If

        Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R

        Dim dataMin As Date = AGRODATAINIZIO
        Dim dataMax As Date = AGRODATAFINE
        Dim SportelloAperto As Boolean
        objPratiche.Data_Sportello_Da_Servizio_Pive_Multiple(ListaPive, enum_Servizi.PianoConcimazione, DateTime.Now, SportelloAperto, dataMin, dataMax, objParametri_Server, objParametri_Utenti)

        If IsDate(Txt_ValiditaInizio.Text) AndAlso CDate(Txt_ValiditaInizio.Text) < dataMin Then
            Txt_ValiditaInizio.Text = dataMin.ToShortDateString
        End If

        If IsDate(Txt_ValiditaFine.Text) AndAlso CDate(Txt_ValiditaFine.Text) > dataMax Then
            Txt_ValiditaFine.Text = dataMax.ToShortDateString
        End If

        '(18/08/2020 fede) escluse analisi scadute
        Dim DataDa As Date = AGRODATAINIZIO
        Dim DataA As Date = AGRODATAFINE
        If IsDate(Txt_ValiditaInizio.Text) Then
            DataDa = CDate(Txt_ValiditaInizio.Text)
        End If
        If IsDate(Txt_ValiditaFine.Text) Then
            DataA = CDate(Txt_ValiditaFine.Text)
        End If

        Crea_Dt_Analisi(ListaPive, DataDa, DataA)

        If IsNothing(objParametri.ListaImpianti) Then
            Carica_Impianti_Impresa(objParametri.Piva, 0, 0)
        End If

        Aggrega_Griglia_Impianti()

        Select Case Qs_Tipo
            Case enum_PianoConcimazione_Tipo.Bilancio
                Btn_Bilancio.Visible = True
                Btn_Schede.Visible = False
            Case Else
                Btn_Bilancio.Visible = False
                Btn_Schede.Visible = True
        End Select


    End Sub

    Private Sub AnnullaTutto()

        Dim UrlTarget As String

        Dim objParametri As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
        objParametri.Leggi()

        UrlTarget = "..\PianoConcimazione_MenuBS.aspx" &
                "?n=" &
                "&t=" +
                "&p=" +
                Stringa_Codifica(objParametri.Piva, AgroKey_EncoderDecoder, Server) +
                "&o=" &
                "&m=" & Stringa_Codifica(CStr(enum_PUARegolamenti_Tipo.PianoComcimazione), AgroKey_EncoderDecoder, Server)

        Response.Redirect(UrlTarget)

    End Sub

    Private Sub Crea_Dt_Impianti()

        Dim Dt As New DataTable

        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Centro", GetType(String)))

        Dt.Columns.Add(New DataColumn("Impianti", GetType(String)))
        Dt.Columns.Add(New DataColumn("AppezzaIdimp", GetType(String)))

        Dt.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Veg_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Grfi_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("stato_impianto", GetType(Integer)))  'id_fase tabella PC_FasiCicloColturale DB Gias_Piano_Concimazione
        'Dt.Columns.Add(New DataColumn("stato_impianto_des", GetType(String)))       'fase_des tabella PC_FasiCicloColturale DB Gias_Piano_Concimazione

        Dt.Columns.Add(New DataColumn("Resa", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("sup_imp", GetType(Decimal)))

        Dt.Columns.Add(New DataColumn("strcatasto", GetType(String)))
        Dt.Columns.Add(New DataColumn("catasto", GetType(String)))
        Dt.Columns.Add(New DataColumn("zvn", GetType(String)))
        Dt.Columns.Add(New DataColumn("stranalisi", GetType(String)))

        Session("Dt_Tutti_Impianti") = Dt

    End Sub

    Private Sub Crea_Dt_Analisi(ByVal ListaPive As List(Of String), ByVal DataInizio As Date, ByVal DataFine As Date)

        Dim objAnalisi As New AgronicaCoreAnagrafeDAL.Analisi_Testata_R
        Dim DtAnalisi As New DataTable

        Dim filtroPiva As String = ""
        For Each piva In ListaPive
            filtroPiva &= "'" & piva & "',"
        Next
        If filtroPiva <> "" Then
            DtAnalisi = objAnalisi.LeggiConCertificati_Entita("", 0, 0, 0, DataInizio, DataFine, " piva IN (" & Left(filtroPiva, filtroPiva.Length - 1) & ")", "", objParametri_Server, defaultSezioneSubalterno:="0")
        End If

        Session("Dt_Tutte_Analisi") = DtAnalisi

    End Sub

    Private Function CaricaGriglia_Bilanci() As DataTable

        Dim Dt As New DataTable

        Dt.Columns.Add(New DataColumn("Contatore", GetType(Integer)))

        Dt.Columns.Add(New DataColumn("Piano_Des", GetType(String)))

        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Centro", GetType(String)))

        Dt.Columns.Add(New DataColumn("Impianti", GetType(String)))

        Dt.Columns.Add(New DataColumn("AppezzaIdimp", GetType(String)))


        Dt.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Veg_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Grfi_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Grfi_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Fase_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Fase_Des", GetType(String)))

        Dt.Columns.Add(New DataColumn("Resa", GetType(Decimal)))

        Dt.Columns.Add(New DataColumn("Precessione", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("DispOssigeno", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Sabbia", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Limo", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Argilla", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Ph", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("CalcTot", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("CalcAtt", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("CN", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("SO", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("N", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("P", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("K", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Mg", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("CSC", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("QtaN_FerPrec", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("IdFrequenza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("TipoFertilizzante", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Piovosita", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("PiovositaFeb", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Zvn", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Anno", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Area", GetType(String)))
        Dt.Columns.Add(New DataColumn("RegimeIrriguo", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Anticipazioni", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("AnticipazioniAnni", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("UbicazioneCod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("PercNFissazione", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Copertura", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Analisi_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("SeminaSodo", GetType(Integer)))

        Dt.Columns.Add(New DataColumn("Fattori", GetType(String)))

        Dt.Columns.Add(New DataColumn("N_Ammesso", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("P_Ammesso", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("K_Ammesso", GetType(Decimal)))

        'chiave (x modifiche)
        Dim DtKeys(0) As DataColumn
        DtKeys(0) = Dt.Columns("Contatore")
        Dt.PrimaryKey = DtKeys

        Return Dt

    End Function

    Private Sub Carica_Impianti_Sessione(ByVal ListaImpianti As AgronicaCoreGestioneRichieste.Impianto())

        Dim strImpianti As String = String.Empty
        Dim strAppezzaIdimp As String = String.Empty

        Dim DtImpianti As DataTable
        Dim InserisciImpianto As Boolean

        Dim i As Integer
        Dim Dr As DataRow
        Dim Dt As DataTable
        Dt = Session("Dt_Tutti_Impianti")

        Dim objImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        'ordino per fare poi il raggruppamento sotto
        ListaImpianti = ListaImpianti.OrderBy(Function(x) x.Piva).ThenBy(Function(x) x.Sa_Cod).ThenBy(Function(x) x.Veg_Cod).ThenBy(Function(x) x.Grfi_Cod).ToArray()

        Dim dictStatoImpianto As New Dictionary(Of Integer, List(Of Fase))

        For i = 0 To ListaImpianti.GetLength(0) - 1

            DtImpianti = objImpianto.Leggi_Dati_Impianti_per_PianoConcimazione(ListaImpianti(i).Piva,
                                                                   ListaImpianti(i).Sa_Cod,
                                                                   ListaImpianti(i).Appezza,
                                                                   ListaImpianti(i).Id_Reg, ListaImpianti(i).Progetto_Cod,
                                                                   "", " ip.validita_inizio DESC ", objParametri_Server)

            Dim strGrfiDes As String
            Dim StatoImpiantoDes As String
            Dim CampoCod As Integer
            Dim RagSoc As String
            Dim Centro As String
            Dim ResaTot As Decimal
            Dim ResaImpianto As Decimal
            Dim nImpiantiResa As Integer
            Dim Sup_Imp As Decimal

            InserisciImpianto = True

            If DtImpianti.Rows.Count > 0 Then

                strGrfiDes = DtImpianti.Rows(0).Item("Grfi_Des")
                CampoCod = DtImpianti.Rows(0).Item("Campo_Cod")
                RagSoc = DtImpianti.Rows(0).Item("rag_soc")
                Centro = DtImpianti.Rows(0).Item("Sa_Nome")
                StatoImpiantoDes = GetStatoImpiantoDes(dictStatoImpianto,
                                                        CInt(ddlRegolamento.SelectedValue), ListaImpianti(i).Veg_Cod,
                                                             DtImpianti.Rows(0).Item("stato_impianto"))
                ResaImpianto = 0
                If Not IsDBNull(DtImpianti.Rows(0).Item("Resa_Effettiva")) AndAlso DtImpianti.Rows(0).Item("Resa_Effettiva") <> 0 Then
                    ResaImpianto = DtImpianti.Rows(0).Item("Resa_Effettiva") / DtImpianti.Rows(0).Item("Sup_Imp")
                    nImpiantiResa += 1
                ElseIf Not IsDBNull(DtImpianti.Rows(0).Item("Resa_Prevista")) AndAlso DtImpianti.Rows(0).Item("Resa_Prevista") <> 0 Then
                    ResaImpianto = DtImpianti.Rows(0).Item("Resa_Prevista") / DtImpianti.Rows(0).Item("Sup_Imp")
                    nImpiantiResa += 1
                End If
                ResaTot += ResaImpianto

            End If

            strImpianti &= IIf(strImpianti = String.Empty, "", " - ") &
                          ListaImpianti(i).App_Nome & " (" & ListaImpianti(i).Sup_Imp & "Ha )"

            strAppezzaIdimp &= IIf(strAppezzaIdimp = String.Empty, "", "$") &
                                   CampoCod & "|" & ListaImpianti(i).Appezza & "|" & ListaImpianti(i).Id_Reg & "|" & ListaImpianti(i).Progetto_Cod

            'scludo le destinazioni uso
            If InserisciImpianto = True And ListaImpianti(i).Veg_Cod > 0 Then

                Dr = Dt.NewRow

                Dr.Item("Piva") = ListaImpianti(i).Piva
                Dr.Item("Sa_Cod") = ListaImpianti(i).Sa_Cod

                Dr.Item("Centro") = RagSoc & "<BR>" & Centro

                Dr.Item("Veg_Cod") = ListaImpianti(i).Veg_Cod
                Dr.Item("Grfi_Cod") = ListaImpianti(i).Grfi_Cod
                Dr.Item("stato_impianto") = DtImpianti.Rows(0).Item("stato_impianto")

                Dr.Item("Veg_Des") = ListaImpianti(i).Veg_Des & "<BR>" & strGrfiDes & IIf(StatoImpiantoDes <> "", "<br>" & StatoImpiantoDes, "")

                Dr.Item("Impianti") = strImpianti
                Dr.Item("AppezzaIdimp") = strAppezzaIdimp

                Dr.Item("Sup_Imp") = ListaImpianti(i).Sup_Imp

                ' resa media
                If nImpiantiResa > 0 Then
                    Dr.Item("Resa") = ResaTot / nImpiantiResa
                Else
                    Dr.Item("Resa") = 0
                End If

                Dr.Item("catasto") = ""
                Dr.Item("strcatasto") = ""
                Dr.Item("zvn") = ""
                Dr.Item("stranalisi") = ""

                ResaTot = 0
                nImpiantiResa = 0

                Dt.Rows.Add(Dr)

                strImpianti = String.Empty
                strAppezzaIdimp = String.Empty

            End If

        Next

        Session("Dt_Tutti_Impianti") = Dt

    End Sub
    Private Sub Carica_Impianti_Impresa(ByVal Piva As String, ByVal Veg_Cod As Integer, ByVal sa_cod As Integer)

        'lettura impianti dell'impresa corrente della specie scelta
        Dim Dr As DataRow
        Dim i As Integer

        Dim Dt As DataTable
        Dt = Session("Dt_Tutti_Impianti")

        Dt.Rows.Clear()

        Dim DtTutteAnalisi As DataTable
        DtTutteAnalisi = Session("Dt_Tutte_Analisi")

        Dim DataInizio As Date = AGRODATAINIZIO
        Dim DataFine As Date = AGRODATAFINE

        Dim strImpianti As String = String.Empty
        Dim strAppezzaIdimp As String = String.Empty

        Dim strFiltroDate As String

        If IsDate(Txt_ValiditaInizio.Text) Then
            DataInizio = CDate(Txt_ValiditaInizio.Text)
        End If
        If IsDate(Txt_ValiditaFine.Text) Then
            DataFine = CDate(Txt_ValiditaFine.Text)
        End If
        strFiltroDate = " ip.validita_fine >=" & Agro_SQL_SaveDate(DataInizio)
        strFiltroDate &= "  AND ip.validita_inizio <=" & Agro_SQL_SaveDate(DataFine)

        Dim Dt_Impianti As DataTable
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        Dim FiltroImpianti As String = strFiltroDate
        If Veg_Cod <> 0 Then
            FiltroImpianti &= " and  s.veg_cod=" & Veg_Cod.ToString
        Else
            'escludo destinazioni uso
            FiltroImpianti &= " and  s.veg_cod<>0 "
        End If
        Dt_Impianti = objImpianti.Leggi_Dati_Impianti_per_PianoConcimazione(Piva,
                                         sa_cod, 0, 0, 0,
                                        FiltroImpianti, " r.Piva, r.sa_cod, s.Veg_des, r.Grfi_Cod ", objParametri_Server)

        Dim Dt_Catasto As DataTable
        Dt_Catasto = objImpianti.Leggi_Catasto_Impianti_per_PianoConcimazione(Piva,
                                         sa_cod, 0, 0, 0,
                                        FiltroImpianti, "", objParametri_Server,
                                        defaultSezioneSubalterno:="0")


        '----------------------------------
        'lettura zone vulnerabili
        Dim DtPV As DataTable
        Dim objPV As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
        DtPV = objPV.Leggi(-17,
                           "", "", "", 0, 0, "",
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "",
                            "", objParametri_Server)

        '----------------------------------------
        'lettura fasce (la metto in un try catch in caso non esista la tabella nel DB PianoConcimazione_Pua e di conseguenza la vista)
        Dim DtPVF As DataTable
        Try
            Dim objPVF As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Vulnerabili_R
            DtPVF = objPVF.Leggi("", "", "", 0, 0, "", 0,
                                 "",
                                 "", objParametri_Server)
        Catch ex As Exception

        End Try

        'Dim listaFinalita As List(Of Finalita)
        'listaFinalita = GetListaFinalitaRer(CInt(ddlRegolamento.SelectedValue))

        Dim dictStatoImpianto As New Dictionary(Of Integer, List(Of Fase))

        Dim HasHimp As New Hashtable

        For i = 0 To Dt_Impianti.Rows.Count - 1

            If Not HasHimp.ContainsKey(Dt_Impianti.Rows(i).Item("piva") & "_" & Dt_Impianti.Rows(i).Item("sa_cod") & "_" & Dt_Impianti.Rows(i).Item("appezza") & "_" & Dt_Impianti.Rows(i).Item("id_reg")) Then

                HasHimp.Add(Dt_Impianti.Rows(i).Item("piva") & "_" & Dt_Impianti.Rows(i).Item("sa_cod") & "_" & Dt_Impianti.Rows(i).Item("appezza") & "_" & Dt_Impianti.Rows(i).Item("id_reg"), "")

                Dr = Dt.NewRow

                Dr.Item("piva") = Dt_Impianti.Rows(i).Item("piva")
                Dr.Item("sa_cod") = Dt_Impianti.Rows(i).Item("sa_cod")
                Dr.Item("Centro") = Dt_Impianti.Rows(i).Item("rag_soc") & "<BR>" & Dt_Impianti.Rows(i).Item("sa_nome")

                Dr.Item("Impianti") = Dt_Impianti.Rows(i).Item("app_nome") & " (" & Dt_Impianti.Rows(i).Item("sup_imp") & "Ha )"
                Dr.Item("AppezzaIdimp") = Dt_Impianti.Rows(i).Item("campo_cod") & "|" & Dt_Impianti.Rows(i).Item("appezza") & "|" & Dt_Impianti.Rows(i).Item("id_reg") & "|" & Dt_Impianti.Rows(i).Item("progetto_cod")

                Dim ResaImpianto As Decimal = 0
                If Not IsDBNull(Dt_Impianti.Rows(i).Item("Resa_Effettiva")) AndAlso Dt_Impianti.Rows(i).Item("Resa_Effettiva") <> 0 Then
                    ResaImpianto = Dt_Impianti.Rows(i).Item("Resa_Effettiva") / Dt_Impianti.Rows(i).Item("Sup_Imp")
                ElseIf Not IsDBNull(Dt_Impianti.Rows(i).Item("Resa_Prevista")) AndAlso Dt_Impianti.Rows(i).Item("Resa_Prevista") <> 0 Then
                    ResaImpianto = Dt_Impianti.Rows(i).Item("Resa_Prevista") / Dt_Impianti.Rows(i).Item("sup_imp")
                End If
                Dr.Item("Resa") = ResaImpianto


                Dr.Item("stato_impianto") = Dt_Impianti.Rows(i).Item("stato_impianto")
                Dim StatoImpiantoDes As String = GetStatoImpiantoDes(dictStatoImpianto,
                                                        CInt(ddlRegolamento.SelectedValue), Dt_Impianti.Rows(i).Item("veg_Cod"),
                                                             Dt_Impianti.Rows(i).Item("stato_impianto"))

                Dr.Item("veg_cod") = Dt_Impianti.Rows(i).Item("veg_Cod")

                Dr.Item("veg_des") = Dt_Impianti.Rows(i).Item("veg_des") & "<BR>" & Dt_Impianti.Rows(i).Item("grfi_des") & IIf(StatoImpiantoDes <> "", "<br>" & StatoImpiantoDes, "")
                Dr.Item("grfi_cod") = Dt_Impianti.Rows(i).Item("Grfi_Cod")
                Dr.Item("sup_imp") = Dt_Impianti.Rows(i).Item("sup_imp")



                Dim sCatasto As New Text.StringBuilder
                Dim catasto As New Text.StringBuilder
                Dim strAnalisiCatasto As String = ""
                Dim separatore As String = "<br>"
                Dim DrPV() As DataRow = Nothing
                Dim DrPVFA() As DataRow = Nothing
                Dim DrPVFB() As DataRow = Nothing
                If Dt_Catasto IsNot Nothing Then
                    Dim DrCatasto() As DataRow = Dt_Catasto.Select("piva='" & Dt_Impianti.Rows(i).Item("piva") & "' and sa_cod=" & Dt_Impianti.Rows(i).Item("sa_cod") & " and appezza=" & Dt_Impianti.Rows(i).Item("appezza"))
                    If DrCatasto IsNot Nothing Then
                        For Each d As DataRow In DrCatasto
                            sCatasto.Append(d("prov") & "_" & d("com") & "_" & d("sezione") & "_" & d("foglio") & "_" & d("numero") & "_" & d("subalterno"))
                            catasto.Append(d("prov") & "_" & d("com") & "_" & d("sezione") & "_" & d("foglio") & "_" & d("numero") & "_" & d("subalterno") & "|")
                            strAnalisiCatasto &= IIf(strAnalisiCatasto = "", "", " OR ") & " (Piva = '" & Dt_Impianti.Rows(i).Item("piva") & "' AND prov = '" & d("prov") & "' AND com = '" & d("com") & "' AND sezione = '" & d("sezione") & "' AND numero = " & d("numero") & " AND foglio = " & d("foglio") & " AND subalterno = '" & d("subalterno") & "')"
                            If DtPV IsNot Nothing AndAlso DtPV.Rows.Count > 0 Then
                                DrPV = DtPV.Select("prov='" & d("prov") & "' AND Com='" & d("com") & "' AND Sezione='" & d("sezione") & "' AND Foglio=" & d("foglio") & " AND Numero=" & d("numero") & " AND subalterno='" & d("subalterno") & "'")
                                If DrPV IsNot Nothing AndAlso DrPV.Length > 0 Then
                                    sCatasto.Append("_<b>(V)</b>")
                                End If
                            End If
                            If DtPVF IsNot Nothing AndAlso DtPVF.Rows.Count > 0 Then
                                DrPVFA = DtPVF.Select("prov='" & d("prov") & "' AND Com='" & d("com") & "' AND Sezione='" & d("sezione") & "' AND Foglio=" & d("foglio") & " AND Fascia_Cod=1")
                                If DrPVFA IsNot Nothing AndAlso DrPVFA.Length > 0 Then
                                    sCatasto.Append("_<b>(V-Fascia A)</b>")
                                End If
                                DrPVFB = DtPVF.Select("prov='" & d("prov") & "' AND Com='" & d("com") & "' AND Sezione='" & d("sezione") & "' AND Foglio=" & d("foglio") & " AND Fascia_Cod=2")
                                If DrPVFB IsNot Nothing AndAlso DrPVFB.Length > 0 Then
                                    sCatasto.Append("_<b>(V-Fascia B)</b>")
                                End If
                            End If
                            sCatasto.Append(separatore)
                        Next
                    End If
                End If


                If sCatasto.ToString.Length > 0 Then
                    Dr.Item("strcatasto") = Left(sCatasto.ToString, sCatasto.ToString.Length - 4)
                Else
                    Dr.Item("strcatasto") = ""
                End If

                If catasto.ToString.Length > 0 Then
                    Dr.Item("catasto") = Left(catasto.ToString, catasto.ToString.Length - 1)
                Else
                    Dr.Item("catasto") = ""
                End If

                If sCatasto.ToString.Contains("V") = True Then
                    Dr.Item("zvn") = "v"
                Else
                    Dr.Item("zvn") = ""
                End If

                '---------------------------------------------------------------------
                ' ANALISI
                Dim strPiva As String = ""
                Dim strSaCod As String = ""
                Dim strCampi As String = ""
                Dim strAppezzamenti As String = ""
                Dim strRegImpianti As String = ""

                strPiva = Dt_Impianti.Rows(i).Item("piva")
                strSaCod = Dt_Impianti.Rows(i).Item("sa_cod")
                strCampi = " (Piva = '" & strPiva & "' AND Sa_Cod = " & strSaCod & " AND Campo_Cod = " & Dt_Impianti.Rows(i).Item("campo_cod") & ")"
                strAppezzamenti = " (Piva = '" & strPiva & "' AND Sa_Cod = " & strSaCod & " AND Campo_Cod = " & Dt_Impianti.Rows(i).Item("campo_cod") & " AND Appezza= " & Dt_Impianti.Rows(i).Item("appezza") & ")"
                strRegImpianti = " (Piva = '" & strPiva & "' AND Sa_Cod = " & strSaCod & " AND Campo_Cod = " & Dt_Impianti.Rows(i).Item("campo_cod") & " AND Appezza= " & Dt_Impianti.Rows(i).Item("appezza") & " AND Id_Imp = " & Dt_Impianti.Rows(i).Item("id_reg") & ")"

                Dim drAnalisi() As DataRow = Nothing
                If strAnalisiCatasto <> "" Then
                    drAnalisi = DtTutteAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Particella & " and (" & strAnalisiCatasto & ")")
                End If
                If Not (drAnalisi IsNot Nothing AndAlso drAnalisi.Length > 0) Then
                    If strRegImpianti <> "" Then
                        drAnalisi = DtTutteAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Impianto & " and (" & strRegImpianti & ")")
                    End If
                    If Not (drAnalisi IsNot Nothing AndAlso drAnalisi.Length > 0) Then
                        If strAppezzamenti <> "" Then
                            drAnalisi = DtTutteAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Appezzamento & " and (" & strAppezzamenti & ")")
                        End If
                        If Not (drAnalisi IsNot Nothing AndAlso drAnalisi.Length > 0) Then
                            If strCampi <> "" Then
                                drAnalisi = DtTutteAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Campo & " and (" & strCampi & ")")
                            End If
                            If Not (drAnalisi IsNot Nothing AndAlso drAnalisi.Length > 0) Then
                                drAnalisi = DtTutteAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Centro & " and Piva = '" & strPiva & "' AND Sa_Cod = " & strSaCod)
                                If Not (drAnalisi IsNot Nothing AndAlso drAnalisi.Length > 0) Then
                                    drAnalisi = DtTutteAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Impresa & " and Piva = '" & strPiva & "'")
                                End If
                            End If
                        End If
                    End If
                End If

                Dim strAnalisi As String = ""
                If drAnalisi IsNot Nothing Then
                    For Each drA As DataRow In drAnalisi
                        If Not strAnalisi.Contains(drA.Item("analisi_testata_cod") & ",") Then
                            strAnalisi &= drA.Item("analisi_testata_cod") & ","
                        End If
                    Next
                End If
                If strAnalisi <> "" Then
                    Dr.Item("stranalisi") = Left(strAnalisi, strAnalisi.Length - 1)
                Else
                    Dr.Item("stranalisi") = ""
                End If

                '----------------------------------------------------------------------

                Dt.Rows.Add(Dr)

            End If

        Next

        Session("Dt_Tutti_Impianti") = Dt

    End Sub

    Private Sub Aggrega_Griglia_Impianti()

        Dim Dt As New DataTable
        Dt = Session("Dt_Tutti_Impianti")

        Dim DtTutteAnalisi As DataTable
        DtTutteAnalisi = Session("Dt_Tutte_Analisi")

        Dim Dt_Impianti As New DataTable
        Dim Dr As DataRow

        Dim InserisciImpianto As Boolean

        Dim strImpianti As String = String.Empty
        Dim strAppezzaIdimp As String = String.Empty
        Dim strCatasto As String = String.Empty
        Dim Catasto As String = String.Empty
        Dim strAnalisi As String = String.Empty

        If Not IsNothing(Dt) Then

            Dt_Impianti = Dt.Clone

            Dim dtNew As DataTable

            Dim Dv As New DataView
            Dt.TableName = "Impianti"
            Dv.Table = Dt

            'ordinamento utile all'aggregazione sotto
            Select Case CmbAggregazione.SelectedValue
                Case enum_Tipo_Aggregazione.Azienda_Centro_Specie_Finalita
                    Dv.Sort = "Piva, centro, Veg_des, Grfi_Cod, stato_impianto"
                Case enum_Tipo_Aggregazione.Azienda_Centro_Specie_Finalita_Vulnerabilita
                    Dv.Sort = "Piva, centro, Veg_des, Grfi_Cod, stato_impianto, zvn"
                Case enum_Tipo_Aggregazione.Azienda_Centro_Specie_Finalita_Vulnerabilita_Analisi
                    Dv.Sort = "Piva, centro, Veg_des, Grfi_Cod, stato_impianto, zvn"
                    'Dv.Sort = "Piva, centro, Veg_des, Grfi_Cod, zvn, stranalisi"
            End Select

            dtNew = Dv.ToTable


            If dtNew IsNot Nothing AndAlso dtNew.Rows.Count > 0 AndAlso
               CmbAggregazione.SelectedValue = enum_Tipo_Aggregazione.Azienda_Centro_Specie_Finalita_Vulnerabilita_Analisi Then

                '''' Spiegazione Algoritmo:
                ' sort come per group by fino a zvn
                ' capire quante righe consecutive devo prendere in considerazione
                ' il mio blocco dovrebbe essere solo le righe che hanno stessi valori di Piva, centro, Veg_des, Grfi_Cod e zvn

                ' passare queste righe ad una funzione
                ' estrapolare l'array di analisi per ogni riga

                ' devo trovare quella che è (sono) l'analisi maggiormente presente
                ' quella diventerà una riga, quindi lascio come valida solo quella/e analisi

                ' sul blocco che rimane trova quella che è l'analisi maggiormente presente
                ' quella diventerà una riga, quindi lascio come valida solo quell'analisi

                ' dopo aver fatto tutte queste sostituzioni sulla colonna delle analisi, ri-ordino per tutte e 6 le colonne
                ' a questo punto segue la procedura normale, avendo già le righe ordinate e le raggruppa per le 6 colonne

                Dim dtTemp As New DataTable
                dtTemp = dtNew.Clone()
                Dim primaRigaCambioChiave As Integer = 0

                'Ciclo il totale del dt
                For i As Integer = 0 To dtNew.Rows.Count - 1

                    If Not (i = dtNew.Rows.Count - 1 OrElse
                            (dtNew.Rows(i).Item("Piva") <> dtNew.Rows(i + 1).Item("Piva") Or
                             dtNew.Rows(i).Item("sa_cod") <> dtNew.Rows(i + 1).Item("sa_cod") Or
                             dtNew.Rows(i).Item("veg_cod") <> dtNew.Rows(i + 1).Item("veg_cod") Or
                             dtNew.Rows(i).Item("grfi_cod") <> dtNew.Rows(i + 1).Item("grfi_cod") Or
                             dtNew.Rows(i).Item("stato_impianto") <> dtNew.Rows(i + 1).Item("stato_impianto") Or
                             dtNew.Rows(i).Item("zvn") <> dtNew.Rows(i + 1).Item("zvn"))) Then

                        'sono in un blocco in cui gli elementi chiave sono uguali alla riga successiva,
                        'quindi mi copio la riga nel dt temporaneo
                        dtTemp.ImportRow(dtNew.Rows(i))

                    Else

                        'è l'ultima riga del blocco (o dt), quindi devo fare il ragionamento, ma devo cmq copiarla nel dtTemp in ogni caso
                        dtTemp.ImportRow(dtNew.Rows(i))


                        'è la prima riga del nuovo blocco, quindi devo fare le statistiche ed i cambi nel blocco appena concluso


                        'TODO: faccio il mio ragionamento su dtTemp per cambiare i valori

                        'Mi mantengo un dizionario con l'analisi, la lista di indici di righe che ce l'hanno e per comodità un contatore di queste ultime
                        Dim statistica As List(Of StatisticaAnalisi) = GeneraStatisticheAnalisi(dtTemp)

                        'Ora che ho generato le statistiche devo capire qual è l'analisi più frequente
                        'Mi copio la lista ordinata da un'altra parte
                        Dim statistichePerRicorrenza As List(Of StatisticaAnalisi) = statistica.OrderByDescending(Function(x) x.Ricorrenza).ToList()

                        'Devo verificare se ho un'analisi 0
                        Dim noAnalisi As StatisticaAnalisi = statistica.Find(Function(x) x.Analisi = 0)
                        Dim strAnalisiUlteriori As String = ""
                        If Not noAnalisi Is Nothing Then
                            'devo tenere da conto quali sono le altre analisi di questo blocco perché le devo aggiungere nella strAnalisi
                            strAnalisiUlteriori = String.Join(",",
                                                              statistica.OrderBy(Function(y) y.Analisi).Select(Function(x) x.Analisi.ToString()).ToArray())
                        End If

                        'Devo mantenere un elenco con gli id di riga che ho già sistemato
                        Dim listIdRigaSistemati As New List(Of Integer)

                        'Esegui (eliminando la statistica mano a mano all'interno della funzione) finché non si esaurisce la lista
                        While listIdRigaSistemati.Count < dtTemp.Rows.Count AndAlso statistichePerRicorrenza.Count > 0
                            SistemaStringaAnalisi(dtTemp, statistichePerRicorrenza, listIdRigaSistemati, strAnalisiUlteriori)
                        End While



                        'Switcho la stringa di analisi in tutte le righe del blocco appena passato,
                        'partendo da primaRigaCambioChiave e per la lunghezza dell righe "parcheggiate" e manipolate su dtTemp
                        Dim k As Integer = 0
                        For j As Integer = primaRigaCambioChiave To primaRigaCambioChiave + (dtTemp.Rows.Count - 1)
                            dtNew.Rows(j).Item("stranalisi") = dtTemp.Rows(k).Item("stranalisi")
                            k += 1
                        Next

                        'Dalla prossima riga in poi c'è un nuovo blocco, quindi ri-azzero il mio dt e le variabili ed imposto come prima riga del blocco la prossima
                        primaRigaCambioChiave = i + 1
                        dtTemp.Clear()

                        'inserisco la riga nel dt temporaneo (solo se non sono sull'ultima riga)
                        'If i < dtNew.Rows.Count - 1 Then
                        '    dtTemp.ImportRow(dtNew.Rows(i))
                        'End If

                    End If

                Next

                dtTemp = Nothing

                'Ora che ho sistemato la colonna analisi lasciando solo i valori su cui raggruppare, posso riordinare il DataTable secondo il criterio giusto
                '(ORRIBILE) per farlo ripasso dal DataView e ricreo di nuovo il DataTable?!?, farlo meglio!
                Dim dvTemp As New DataView
                dvTemp.Table = dtNew
                dvTemp.Sort = "Piva, centro, Veg_des, Grfi_Cod, stato_impianto, zvn, stranalisi"

                Dim dtAnalisi As DataTable = dvTemp.ToTable
                dtNew = dtAnalisi
            End If




            For i = 0 To dtNew.Rows.Count - 1

                Dim ResaTot As Decimal
                Dim ResaImpianto As Decimal
                Dim nImpiantiResa As Integer

                InserisciImpianto = True

                ResaImpianto = 0
                If Not IsDBNull(dtNew.Rows(i).Item("Resa")) AndAlso dtNew.Rows(i).Item("Resa") <> 0 Then
                    ResaImpianto = dtNew.Rows(i).Item("Resa") / dtNew.Rows(i).Item("sup_imp")
                    nImpiantiResa += 1
                End If
                ResaTot += ResaImpianto

                strImpianti &= IIf(strImpianti = String.Empty, "", " - ") & dtNew.Rows(i).Item("impianti")

                strAppezzaIdimp &= IIf(strAppezzaIdimp = String.Empty, "", "$") & dtNew.Rows(i).Item("AppezzaIdimp")

                'strCatasto &= dtNew.Rows(i).Item("strcatasto")
                strCatasto &= IIf(strCatasto = String.Empty, "", "<br>") & dtNew.Rows(i).Item("strcatasto")
                Catasto &= IIf(strAppezzaIdimp = String.Empty, "", "$") & dtNew.Rows(i).Item("catasto")
                strAnalisi &= IIf(strAnalisi = String.Empty, "", "$") & dtNew.Rows(i).Item("stranalisi")

                Select Case CmbAggregazione.SelectedValue

                    Case enum_Tipo_Aggregazione.Azienda_Centro_Specie_Finalita

                        If Not (i = dtNew.Rows.Count - 1 OrElse
                                 (dtNew.Rows(i).Item("Piva") <> dtNew.Rows(i + 1).Item("Piva") Or
                                  dtNew.Rows(i).Item("sa_cod") <> dtNew.Rows(i + 1).Item("sa_cod") Or
                                  dtNew.Rows(i).Item("veg_cod") <> dtNew.Rows(i + 1).Item("veg_cod") Or
                                  dtNew.Rows(i).Item("grfi_cod") <> dtNew.Rows(i + 1).Item("grfi_cod") Or
                                  dtNew.Rows(i).Item("stato_impianto") <> dtNew.Rows(i + 1).Item("stato_impianto"))) Then

                            InserisciImpianto = False

                        End If

                    Case enum_Tipo_Aggregazione.Azienda_Centro_Specie_Finalita_Vulnerabilita

                        If Not (i = dtNew.Rows.Count - 1 OrElse
                             (dtNew.Rows(i).Item("Piva") <> dtNew.Rows(i + 1).Item("Piva") Or
                              dtNew.Rows(i).Item("sa_cod") <> dtNew.Rows(i + 1).Item("sa_cod") Or
                              dtNew.Rows(i).Item("veg_cod") <> dtNew.Rows(i + 1).Item("veg_cod") Or
                              dtNew.Rows(i).Item("grfi_cod") <> dtNew.Rows(i + 1).Item("grfi_cod") Or
                              dtNew.Rows(i).Item("stato_impianto") <> dtNew.Rows(i + 1).Item("stato_impianto") Or
                              dtNew.Rows(i).Item("zvn") <> dtNew.Rows(i + 1).Item("zvn"))) Then

                            InserisciImpianto = False

                        End If

                    Case enum_Tipo_Aggregazione.Azienda_Centro_Specie_Finalita_Vulnerabilita_Analisi

                        If Not (i = dtNew.Rows.Count - 1 OrElse
                          (dtNew.Rows(i).Item("Piva") <> dtNew.Rows(i + 1).Item("Piva") Or
                           dtNew.Rows(i).Item("sa_cod") <> dtNew.Rows(i + 1).Item("sa_cod") Or
                           dtNew.Rows(i).Item("veg_cod") <> dtNew.Rows(i + 1).Item("veg_cod") Or
                           dtNew.Rows(i).Item("grfi_cod") <> dtNew.Rows(i + 1).Item("grfi_cod") Or
                            dtNew.Rows(i).Item("stato_impianto") <> dtNew.Rows(i + 1).Item("stato_impianto") Or
                           dtNew.Rows(i).Item("zvn") <> dtNew.Rows(i + 1).Item("zvn") Or
                           dtNew.Rows(i).Item("stranalisi") <> dtNew.Rows(i + 1).Item("stranalisi"))) Then

                            InserisciImpianto = False

                        End If
                End Select

                If InserisciImpianto = True Then

                    Dr = Dt_Impianti.NewRow

                    Dr.Item("Piva") = dtNew.Rows(i).Item("Piva")
                    Dr.Item("Sa_Cod") = dtNew.Rows(i).Item("sa_cod")

                    Dr.Item("Centro") = dtNew.Rows(i).Item("centro")

                    Dr.Item("Veg_Cod") = dtNew.Rows(i).Item("veg_cod")
                    Dr.Item("Grfi_Cod") = dtNew.Rows(i).Item("grfi_cod")
                    Dr.Item("stato_impianto") = dtNew.Rows(i).Item("stato_impianto")

                    Dr.Item("Veg_Des") = dtNew.Rows(i).Item("veg_des")

                    Dr.Item("Impianti") = strImpianti
                    Dr.Item("strCatasto") = strCatasto
                    Dr.Item("Catasto") = Catasto
                    Dr.Item("zvn") = dtNew.Rows(i).Item("zvn")

                    Dr.Item("AppezzaIdimp") = strAppezzaIdimp

                    Dr.Item("strAnalisi") = strAnalisi

                    ' resa media
                    If nImpiantiResa > 0 Then
                        Dr.Item("Resa") = ResaTot / nImpiantiResa
                    Else
                        Dr.Item("Resa") = 0
                    End If

                    ResaTot = 0
                    nImpiantiResa = 0

                    Dt_Impianti.Rows.Add(Dr)

                    strImpianti = String.Empty
                    strAppezzaIdimp = String.Empty
                    strCatasto = String.Empty
                    Catasto = String.Empty
                    strAnalisi = String.Empty

                End If

            Next

        End If


        Dim NomiChiavi(11) As String
        NomiChiavi(0) = "Piva"
        NomiChiavi(1) = "Sa_Cod"
        NomiChiavi(2) = "Veg_Cod"
        NomiChiavi(3) = "Grfi_Cod"
        NomiChiavi(4) = "AppezzaIdimp"
        NomiChiavi(5) = "Veg_Des"
        NomiChiavi(6) = "Centro"
        NomiChiavi(7) = "Impianti"
        NomiChiavi(8) = "Resa"
        NomiChiavi(9) = "zvn"
        NomiChiavi(10) = "stranalisi"
        NomiChiavi(11) = "stato_impianto"

        grdSpecie.DataSource = Dt_Impianti ' dtSorted
        grdSpecie.DataKeyNames = NomiChiavi
        grdSpecie.DataBind()

        Dim objAnalisi As New AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_R
        Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        Dim objSpecieConc As New AgronicaCoreMetaSchemaDAL.PC_SpecieConcimazione_R
        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        Dim ddlFinalitaRer As DropDownList
        Dim ddlFasi As DropDownList
        Dim ddlAnalisi As DropDownList

        Dim UnaSpecie As Boolean = True
        Dim PrimaSpecie As Integer
        If grdSpecie.Rows.Count > 0 Then
            PrimaSpecie = grdSpecie.DataKeys(0).Item(2).ToString
        End If

        Dim Regolamento_Cod As Integer = CInt(ddlRegolamento.SelectedValue)


        ' preparo il filtro delle analisi
        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Dt_Impost As DataTable
        Dt_Impost = objImpost.Leggi2(1,
                                     objParametri_Utenti.UtenteUsername,
                                     enum_Impostazioni_Utenti.UTENTE_Nitrati_PC_Analisi,
                                     "", "",
                                     objParametri_Utenti)

        Dim strEntitaAnalisi As String
        If Dt_Impost.Rows.Count > 0 Then
            strEntitaAnalisi = Dt_Impost.Rows(0).Item("Impostazione_Valore_1")
        End If

        Dim VetEntita() As String
        VetEntita = Split(strEntitaAnalisi, "|")

        For i = 0 To grdSpecie.Rows.Count - 1

            If grdSpecie.DataKeys(i).Item(9) <> "" Then
                CType(grdSpecie.Rows(i).FindControl("ChkZVN"), CheckBox).Checked = True
            End If

            If Math.Round(grdSpecie.DataKeys(i).Item(8) / 1000, MidpointRounding.AwayFromZero) > 0 Then
                CType(grdSpecie.Rows(i).FindControl("txtResa"), TextBox).Text = Math.Round(grdSpecie.DataKeys(i).Item(8) / 1000, MidpointRounding.AwayFromZero)
            End If

            ddlFinalitaRer = CType(grdSpecie.Rows(i).FindControl("ddlFinalita"), DropDownList)
            If Not IsNothing(ddlFinalitaRer) Then
                AgronicaControlli_2010.ListControl_PianoConcimazione_WS.Finalita_Rer_WS(ddlFinalitaRer, False, "", "", Regolamento_Cod, grdSpecie.DataKeys(i).Item(2).ToString, 0, "", "", "")
            End If

            ddlFasi = CType(grdSpecie.Rows(i).FindControl("ddlFasi"), DropDownList)

            If PrimaSpecie <> grdSpecie.DataKeys(i).Item(2).ToString Then
                UnaSpecie = False
            End If

            If Not IsNothing(ddlFasi) Then

                AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PC_FasiCicloColturale_WS(ddlFasi, False, "", "", Regolamento_Cod,
                                               grdSpecie.DataKeys(i).Item(2).ToString, "", "")

                If ddlFasi.Items.Count > 1 Then

                    If grdSpecie.DataKeys(i).Item(11) <> 102 Then
                        ddlFasi.SelectedIndex =
                          ddlFasi.Items.IndexOf(
                               ddlFasi.Items.FindByValue(grdSpecie.DataKeys(i).Item(11)))
                    Else

                        If Not IsNothing(ddlFasi.Items.FindByText("piena produzione")) Then

                            ddlFasi.SelectedIndex =
                                ddlFasi.Items.IndexOf(
                                     ddlFasi.Items.FindByText("piena produzione"))
                        End If
                    End If

                End If

                'setto resa default se non valorizzata
                If Not IsNumeric(CType(grdSpecie.Rows(i).FindControl("txtResa"), TextBox).Text) Then

                    Dim Grfi_Cod As Integer = 0
                    Dim Resa As Decimal = 0
                    If IsNumeric(ddlFinalitaRer.SelectedValue) Then
                        Grfi_Cod = CInt(CType(grdSpecie.Rows(i).FindControl("ddlFinalita"), DropDownList).SelectedValue)
                    End If
                    Dim N_Fattore_Correttivo_Resa As Decimal
                    Dim Mas As Decimal = -1
                    Resa = LeggiResa(Regolamento_Cod, grdSpecie.DataKeys(i).Item(2), Grfi_Cod, enum_Stato_Impianto.Impianto_Produzione, Mas, N_Fattore_Correttivo_Resa)
                    If Resa > 0 Then
                        CType(grdSpecie.Rows(i).FindControl("txtResa"), TextBox).Text = Resa
                    End If
                End If

            End If


            ddlAnalisi = CType(grdSpecie.Rows(i).FindControl("ddlAnalisi"), DropDownList)

            If Not IsNothing(ddlAnalisi) Then

                'se ho già caricato le analisi ...
                If grdSpecie.DataKeys(i).Item(10) <> "" AndAlso Not DtTutteAnalisi Is Nothing Then

                    Dim HashAnalisi As New Hashtable
                    Dim strAI As String = grdSpecie.DataKeys(i).Item(10).ToString
                    Dim vetAImp() As String = strAI.Split(New Char() {"$"}, StringSplitOptions.RemoveEmptyEntries)
                    If Not vetAImp Is Nothing Then
                        For Each aImp In vetAImp
                            Dim analisi_cod() As String = aImp.Split(",")
                            For Each analisic In analisi_cod

                                If Not HashAnalisi.ContainsKey(analisic) Then
                                    Dim descAnalisi As String = ""

                                    If analisic = "0" Then
                                        descAnalisi = "Nessuna Analisi"
                                    Else
                                        Dim analisi As DataRow = DtTutteAnalisi.Select("analisi_testata_cod=" & analisic.ToString).FirstOrDefault()
                                        If Not IsNothing(analisi) Then
                                            descAnalisi = analisi.Item("Analisi_Testata_Des") & " - " & CDate(analisi.Item("Analisi_Testata_Data_Inizio")).ToShortDateString
                                        End If
                                    End If

                                    If descAnalisi <> "" Then
                                        'ddlAnalisi.Items.Add(New ListItem(descAnalisi, analisic.ToString))
                                        ddlAnalisi.Items.Insert(0, New ListItem(descAnalisi, analisic.ToString))
                                        HashAnalisi.Add(analisic, "")
                                    End If
                                End If

                            Next
                        Next
                    End If

                Else

                    Dim strFiltroEntita As String = ""
                    Dim strTemp As String = ""

                    Dim strPiva As String = ""
                    Dim strSaCod As String = ""
                    Dim strCampo As String = ""
                    Dim strAppezza As String = ""
                    Dim strIdReg As String = ""
                    Dim vetImp() As String


                    Dim strCampi As String = ""
                    Dim strAppezzamenti As String = ""
                    Dim strRegImpianti As String = ""

                    Dim str As String
                    str = grdSpecie.DataKeys(i).Item(4).ToString
                    vetImp = str.Split("$")

                    strPiva = grdSpecie.DataKeys(i).Item(0).ToString
                    strSaCod = grdSpecie.DataKeys(i).Item(1).ToString

                    For e = 0 To vetImp.Length - 1
                        strCampi &= IIf(strCampi = "", "", " OR ") & " (Analisi_EntitaxTestata.Piva = '" & strPiva & "' AND Analisi_EntitaxTestata.Sa_Cod = " & strSaCod &
                            " AND Analisi_EntitaxTestata.Campo_Cod = " & vetImp(e).Split("|")(0) & ")"
                        strAppezzamenti &= IIf(strAppezzamenti = "", "", " OR ") & " (Analisi_EntitaxTestata.Piva = '" & strPiva & "' AND Analisi_EntitaxTestata.Sa_Cod = " & strSaCod &
                            " AND Analisi_EntitaxTestata.Campo_Cod = " & vetImp(e).Split("|")(0) & " AND Analisi_EntitaxTestata.Appezza= " & vetImp(e).Split("|")(1) & ")"
                        strRegImpianti &= IIf(strRegImpianti = "", "", " OR ") & " (Analisi_EntitaxTestata.Piva = '" & strPiva & "' AND Analisi_EntitaxTestata.Sa_Cod = " & strSaCod &
                            " AND Analisi_EntitaxTestata.Campo_Cod = " & vetImp(e).Split("|")(0) & " AND Analisi_EntitaxTestata.Appezza= " & vetImp(e).Split("|")(1) & " AND Analisi_EntitaxTestata.Id_Imp = " & vetImp(e).Split("|")(2) & ")"
                    Next


                    For e = 0 To VetEntita.Length - 1
                        Select Case VetEntita(e)
                            Case enum_Entita_Analisi.Impresa
                            Case enum_Entita_Analisi.Centro
                                strTemp = " (Analisi_EntitaxTestata.Piva = '" & strPiva & "' AND Analisi_EntitaxTestata.Sa_Cod = " & strSaCod & " AND Analisi_EntitaxTestata.Appezza=0 AND Analisi_EntitaxTestata.Id_Imp=0) "
                            Case enum_Entita_Analisi.Campo
                                strTemp = "(" & strCampi & ")"
                            Case enum_Entita_Analisi.Appezzamento
                                strTemp = "(" & strAppezzamenti & ")"
                            Case enum_Entita_Analisi.Impianto
                                strTemp = "(" & strRegImpianti & ")"
                        End Select

                        If strTemp <> "" Then
                            strFiltroEntita &= IIf(strFiltroEntita = "", "", " OR ") & strTemp
                        End If

                    Next

                    If strFiltroEntita <> "" Then
                        strFiltroEntita = "(" & strFiltroEntita & ")"
                    End If

                    Analisi_Terreno(ddlAnalisi, False, "Nessuna Analisi", "0", grdSpecie.DataKeys(i).Item(0).ToString, strFiltroEntita, "Analisi_Testata_Data_Inizio DESC", objParametri_Server)

                    Dim dtAnalisi As DataTable
                    dtAnalisi = objAnalisi.Leggi(0, 0, grdSpecie.DataKeys(i).Item(0).ToString, 0, 0, 0, 0, 0, "", "", "", 0, 0, "", "",
                                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                             "", "Analisi_Testata_Data_Inizio DESC",
                                             objParametri_Server)

                    Dim j As Integer
                    Dim DrAnalisi() As DataRow

                    Dim TestataCod As Integer

                    For j = 0 To dtAnalisi.Rows.Count - 1
                        str = grdSpecie.DataKeys(i).Item(4).ToString
                        vetImp = str.Split("$")

                        ' c'è solo un impianto per riga, quindi cerco le analisi associate all'impianto
                        If vetImp.Length = 1 Then
                            strCampo = vetImp(0).Split("|")(0)
                            strAppezza = vetImp(0).Split("|")(1)
                            strIdReg = vetImp(0).Split("|")(2)

                            DrAnalisi = dtAnalisi.Select("Piva='" & strPiva & "' AND Sa_Cod = " & strSaCod & " AND Campo_Cod = " & strCampo & " AND Appezza = " & strAppezza & " AND Id_Imp =" & strIdReg)

                            If DrAnalisi.Length = 0 Then

                                DrAnalisi = dtAnalisi.Select("Piva='" & strPiva & "' AND Sa_Cod = " & strSaCod & " AND Campo_Cod = " & strCampo & " AND Appezza = " & strAppezza)

                                If DrAnalisi.Length = 0 Then

                                    DrAnalisi = dtAnalisi.Select("Piva='" & strPiva & "' AND Sa_Cod = " & strSaCod & " AND Campo_Cod = " & strCampo)

                                    If DrAnalisi.Length = 0 Then

                                        DrAnalisi = dtAnalisi.Select("Piva='" & strPiva & "' AND Sa_Cod = " & strSaCod)

                                    End If
                                End If
                            End If
                        Else
                            ' ci sono più impianti per riga, quindi cerco le analisi associate al centro aziendale
                            strPiva = grdSpecie.DataKeys(i).Item(0).ToString
                            strSaCod = grdSpecie.DataKeys(i).Item(1).ToString
                            DrAnalisi = dtAnalisi.Select("Piva='" & strPiva & "' AND Sa_Cod=" & strSaCod)

                        End If

                        If DrAnalisi.Length <> 0 Then
                            TestataCod = DrAnalisi(0).Item("Analisi_Testata_Cod")
                            ddlAnalisi.SelectedIndex = ddlAnalisi.Items.IndexOf(ddlAnalisi.Items.FindByValue(TestataCod))
                        Else
                            ddlAnalisi.Items.Add(New ListItem("Nessuna Analisi", "0"))
                        End If


                    Next


                End If

                If ddlAnalisi.Items.Count = 0 Then
                    ddlAnalisi.Items.Add(New ListItem("Nessuna Analisi", "0"))
                End If

            End If


            AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PC_PrecessioneColturale_WS(CType(grdSpecie.Rows(i).FindControl("ddlPrecessione"), DropDownList), False, "", "", Regolamento_Cod, 0, "", "")
            CType(grdSpecie.Rows(i).FindControl("ddlPrecessione"), DropDownList).SelectedIndex = CType(grdSpecie.Rows(i).FindControl("ddlPrecessione"), DropDownList).Items.IndexOf(CType(grdSpecie.Rows(i).FindControl("ddlPrecessione"), DropDownList).Items.FindByText("Non definita"))

            AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PC_Ubicazione_WS(CType(grdSpecie.Rows(i).FindControl("ddlUbicazione"), DropDownList), False, "", "", Regolamento_Cod, "", "")

            AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PC_DisponibilitaOssigeno_WS(CType(grdSpecie.Rows(i).FindControl("ddlDispOss"), DropDownList), False, "", "", Regolamento_Cod, "", "")

            AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PC_Frequenza_WS(CType(grdSpecie.Rows(i).FindControl("ddlFrequenza"), DropDownList), False, "", "", Regolamento_Cod, "", "")

            AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PC_MatriciOrganiche_WS(CType(grdSpecie.Rows(i).FindControl("ddlFertilizzante"), DropDownList), False, "", "", Regolamento_Cod, "", "")
            CType(grdSpecie.Rows(i).FindControl("ddlFertilizzante"), DropDownList).SelectedIndex = CType(grdSpecie.Rows(i).FindControl("ddlFertilizzante"), DropDownList).Items.IndexOf(CType(grdSpecie.Rows(i).FindControl("ddlFertilizzante"), DropDownList).Items.FindByText("Nessuno"))

            ' carico i dati delle piogge leggendo dal meteo
            Dim DtCentri As DataTable
            objCentri = New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            DtCentri = objCentri.Leggi_Coordinate(grdSpecie.DataKeys(i).Item(0).ToString, grdSpecie.DataKeys(i).Item(1), "", "", objParametri_Server)

            Dim X As Integer
            Dim Y As Integer
            If DtCentri.Rows.Count = 1 Then
                X = DtCentri.Rows(0).Item("X")
                Y = DtCentri.Rows(0).Item("Y")
            End If

            Dim strMsg As String
            Dim TotMMInv As Decimal = 0
            Dim TotMMFeb As Decimal = 0

            Dim DataDAInv As Date
            Dim DataAInv As Date
            Dim DataDAFeb As Date
            Dim DataAFeb As Date

            'piogge invernali
            DataDAInv = CDate("01/10/" & CInt(Txt_Anno.Text) - 1)
            DataAInv = CDate("31/01/" & CInt(Txt_Anno.Text))

            'piogge febbraio
            DataDAFeb = "01/02/" & CInt(Txt_Anno.Text)
            If DateTime.IsLeapYear(CInt(Txt_Anno.Text)) Then
                DataAFeb = CDate("29/02/" & CInt(Txt_Anno.Text))
            Else
                DataAFeb = CDate("28/02/" & CInt(Txt_Anno.Text))
            End If

            TotMMInv = Carica_Dati_Meteo(X, Y, DataDAInv, DataAInv, 0, 0, strMsg)

            Select Case CInt(ddlRegolamento.SelectedValue)
                Case Is >= enum_PUARegolamenti.PianoConcimazione_2016
                    TotMMFeb = Carica_Dati_Meteo(X, Y, DataDAFeb, DataAFeb, 0, 0, strMsg)
            End Select

            CType(grdSpecie.Rows(i).FindControl("Txt_Pioggia"), TextBox).Text = TotMMInv
            CType(grdSpecie.Rows(i).FindControl("Txt_Pioggia_Febbraio"), TextBox).Text = TotMMFeb

        Next

        '-------------------------------------------------------------------------
        ' HEADER

        If grdSpecie.Rows.Count > 0 Then

            If UnaSpecie Then
                AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PC_FasiCicloColturale_WS(CType(grdSpecie.HeaderRow.FindControl("ddlFasiHeader"), DropDownList),
                                  False, "", "", Regolamento_Cod,
                                  PrimaSpecie, "", "")

            Else
                CType(grdSpecie.HeaderRow.FindControl("ddlFasiHeader"), DropDownList).Visible = False
            End If


            AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PC_Ubicazione_WS(CType(grdSpecie.HeaderRow.FindControl("ddlUbicazioneHeader"), DropDownList),
                   False, "", "", Regolamento_Cod, "", "")

            AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PC_DisponibilitaOssigeno_WS(CType(grdSpecie.HeaderRow.FindControl("ddlDispOssHeader"), DropDownList),
                   False, "", "", Regolamento_Cod, "", "")

            AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PC_Frequenza_WS(CType(grdSpecie.HeaderRow.FindControl("ddlFrequenzaHeader"), DropDownList),
                   False, "", "", Regolamento_Cod, "", "")

            AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PC_PrecessioneColturale_WS(CType(grdSpecie.HeaderRow.FindControl("ddlPrecessioneHeader"), DropDownList),
                   False, "", "", Regolamento_Cod, 0, "", "")

            CType(grdSpecie.HeaderRow.FindControl("ddlPrecessioneHeader"), DropDownList).SelectedIndex = CType(grdSpecie.HeaderRow.FindControl("ddlPrecessioneHeader"), DropDownList).Items.IndexOf(CType(grdSpecie.HeaderRow.FindControl("ddlPrecessioneHeader"), DropDownList).Items.FindByText("Non definita"))


            AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PC_MatriciOrganiche_WS(CType(grdSpecie.HeaderRow.FindControl("ddlFertilizzanteHeader"), DropDownList),
                   False, "", "", Regolamento_Cod, "", "")

            CType(grdSpecie.HeaderRow.FindControl("ddlFertilizzanteHeader"), DropDownList).SelectedIndex = CType(grdSpecie.HeaderRow.FindControl("ddlFertilizzanteHeader"), DropDownList).Items.IndexOf(CType(grdSpecie.HeaderRow.FindControl("ddlFertilizzanteHeader"), DropDownList).Items.FindByText("Nessuno"))

        End If

    End Sub

    Private Function GeneraStatisticheAnalisi(ByRef dt As DataTable) As List(Of StatisticaAnalisi)

        Dim statistica As New List(Of StatisticaAnalisi)

        'Mi ciclo tutto il blocco per ottenere le "statistiche" sulle analisi usate
        For i As Integer = 0 To dt.Rows.Count - 1

            Dim analisi_cod() As String = dt.Rows(i).Item("stranalisi").split(",")

            'TODO: non credo funzioni bene nel caso di zero analisi
            If analisi_cod.Length > 0 Then

                For Each analisiStr In analisi_cod

                    Dim analisi As Integer = 0
                    If analisiStr <> "" AndAlso IsNumeric(analisiStr) Then
                        analisi = CInt(analisiStr)
                    End If

                    Dim idx As Integer = statistica.FindIndex(Function(x) x.Analisi = analisi)
                    If idx <> -1 Then
                        statistica(idx).Righe.Add(i)
                        statistica(idx).Ricorrenza += 1
                    Else
                        statistica.Add(New StatisticaAnalisi With {
                                          .Analisi = analisi,
                                          .Righe = New List(Of Integer) From {i},
                                          .Ricorrenza = 1
                                       })
                    End If
                Next

            Else
                'Non ho analisi, quindi aggiungo un record al dizionario con chiave analisi = 0
                Dim idx As Integer = statistica.FindIndex(Function(x) x.Analisi = 0)
                If idx <> -1 Then
                    statistica(idx).Righe.Add(i)
                    statistica(idx).Ricorrenza += 1
                Else
                    statistica.Add(New StatisticaAnalisi With {
                                      .Analisi = 0,
                                      .Righe = New List(Of Integer) From {i},
                                      .Ricorrenza = 1
                                   })
                End If
            End If

        Next

        'sto raggruppando per analisi, quindi non posso lasciare una riga con analisi = 0,
        'quindi prendo il tutte quelle righe faccio risultare che avessero l'analisi già più ricorrente
        'Dim noAnalisi As StatisticaAnalisi = statistica.Find(Function(x) x.Analisi = 0)


        'If Not noAnalisi Is Nothing AndAlso statistica.Count > 1 Then
        '    'C'è un analisi? sposto questi id sull'analisi più ricorrente
        '    Dim analisiPiuPresente = statistica.Where(Function(y) y.Analisi <> 0).OrderByDescending(Function(x) x.Ricorrenza).FirstOrDefault()
        '    Dim idx2 As Integer = statistica.FindIndex(Function(x) x.Analisi = analisiPiuPresente.Analisi)
        '    If idx2 <> -1 Then
        '        statistica(idx2).Righe.AddRange(noAnalisi.Righe)
        '        statistica(idx2).Righe.Sort()
        '        statistica(idx2).Ricorrenza = statistica(idx2).Righe.Count

        '        'Avendo spostato, ora elimino la voce dell'Analisi 0
        '        statistica.Remove(noAnalisi)
        '    End If
        'End If

        Return statistica

    End Function

    Private Sub SistemaStringaAnalisi(ByRef dt As DataTable,
                                      ByRef statistichePerRicorrenza As List(Of StatisticaAnalisi),
                                      ByRef listIdRigaSistemati As List(Of Integer),
                                      ByVal strAnalisiUlteriori As String)

        'Considero la prima analisi dell'elenco
        Dim i As Integer = 0
        Dim analisiPiuFrequente As StatisticaAnalisi = statistichePerRicorrenza(i)

        Dim stringaAnalisi As String = CStr(analisiPiuFrequente.Analisi)
        Dim analisiGestite As New List(Of Integer) From {analisiPiuFrequente.Analisi}

        'Solo se mi sono rimaste almeno 2 righe
        If statistichePerRicorrenza.Count >= 2 Then

            'Devo verificare se la/e riga/he successiva/e sono a parimerito
            While statistichePerRicorrenza(i).Ricorrenza = statistichePerRicorrenza(i + 1).Ricorrenza

                'L'analisi successiva ha la stessa ricorrenza, ma coinvolge anche gli stessi idRiga?!?

                Dim differentiIdRiga As Boolean = False
                'Avendo generato le statistiche scorrendo in ordina il dt originale,
                'gli idRiga dentro a Righe sono già ordinati in maniera crescente 
                For j As Integer = 0 To analisiPiuFrequente.Righe.Count - 1
                    If analisiPiuFrequente.Righe(j) <> statistichePerRicorrenza(i + 1).Righe(j) Then
                        differentiIdRiga = True
                        Exit For
                    End If
                Next

                'Stessi idRiga, quindi gestisco in un colpo solo anche questa analisi
                If differentiIdRiga = False Then
                    analisiGestite.Add(statistichePerRicorrenza(i + 1).Analisi)
                    stringaAnalisi &= "," & CStr(statistichePerRicorrenza(i + 1).Analisi)
                End If

                i += 1

                'Devo accertarmi che i prossimi due elementi che confronterò esistano (non devo aver esaurito la lista)
                If i >= statistichePerRicorrenza.Count OrElse i + 1 >= statistichePerRicorrenza.Count Then
                    Exit While
                End If

            End While

        End If

        'mi serve anche una lista per i soli idRiga sistemati in questo giro della funzione
        Dim listIdRigaSistematiLoc As New List(Of Integer)

        For Each idRiga In analisiPiuFrequente.Righe

            'Se ho già sistemato tutte le righe del set iniziale, è inutile proseguire con i cicli
            If listIdRigaSistemati.Count = dt.Rows.Count Then
                Exit For
            End If

            'Devo agire solo se si tratta di una riga che non ho ancora sistemato
            If Not listIdRigaSistemati.Contains(idRiga) Then
                'Dalla tabella originale devo eliminare, per le righe che mi servono, tutte le altre analisi che non siano la/le più frequente/i
                If analisiPiuFrequente.Analisi = 0 Then
                    'Sono nella riga senza analisi, quindi come stringa analisi uso quella del parametro
                    'che include già lo zero più tutte le altre voci della stessa riga
                    dt.Rows(idRiga).Item("stranalisi") = strAnalisiUlteriori
                Else
                    dt.Rows(idRiga).Item("stranalisi") = stringaAnalisi
                End If

                listIdRigaSistemati.Add(idRiga)
                listIdRigaSistematiLoc.Add(idRiga)

                'TODO: devo anche eliminare questo idRiga dalle voci successive ed aggiornare il conteggio
                'TODO: non va bene fatto con questo ciclo, perché in realtà ogni volta che tolgo dal mezzo una riga
                'potrebbero cambiare i contatori dell'ordinamento
            End If

        Next

        'Devo eliminare dalla lista la/le analisi che ho appena sistemato
        statistichePerRicorrenza.RemoveAll(Function(x) analisiGestite.Contains(x.Analisi))

        'Dalle statistiche che rimangono devo eliminare anche gli idRiga già gestiti e ricalcolare la ricorrenza
        statistichePerRicorrenza.ForEach(Sub(x)
                                             x.Righe.RemoveAll(Function(y) listIdRigaSistematiLoc.Contains(y))
                                             x.Ricorrenza = x.Righe.Count
                                         End Sub)

        'Se ora ho delle ricorrenze a zero le elimino
        statistichePerRicorrenza.RemoveAll(Function(x) x.Ricorrenza = 0)

        'Riordino la lista se ci sono elementi
        If statistichePerRicorrenza.Count > 0 Then
            statistichePerRicorrenza = statistichePerRicorrenza.OrderByDescending(Function(x) x.Ricorrenza).ToList()
        End If

        'TODO: esegui ricorsivamente (eliminando la statistica mano a mano) finché non si esaurisce la lista


    End Sub

    Private Function LeggiResa(ByVal Regolamento_Cod As Integer, ByVal Veg_Cod As Integer, ByVal Grfi_Cod As Integer, ByVal Stato_Cod As Integer,
                               ByRef Mas As Decimal, ByRef N_Fattore_Correttivo_Resa As Decimal) As Decimal

        Dim Resa As Decimal = 0
        Dim objParametriIngressoMAS As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_input
        objParametriIngressoMAS.Regolamento_Cod = Regolamento_Cod
        objParametriIngressoMAS.Veg_Cod = Veg_Cod
        objParametriIngressoMAS.Grfi_Cod = Grfi_Cod
        objParametriIngressoMAS.Stato_Cod = Stato_Cod
        objParametriIngressoMAS.ValoreMax = True
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objParametriUscitaMAS As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_output
        objParametriUscitaMAS = objPC_WS.LimiteMAS(objParametriIngressoMAS)

        If objParametriUscitaMAS.Resa <> -1 Then
            Resa = objParametriUscitaMAS.Resa
        End If
        If objParametriUscitaMAS.FattoreCorrettivo_N > 0 Then
            N_Fattore_Correttivo_Resa = objParametriUscitaMAS.FattoreCorrettivo_N
        End If
        If objParametriUscitaMAS.N <> -1 Then
            Mas = objParametriUscitaMAS.N
        End If

        Return Resa

    End Function

    Private Function Carica_Dati_Meteo(ByVal Coordinata_X As Integer, ByVal Coordinata_Y As Integer,
                                       ByVal DataDa As Date, ByVal DataA As Date,
                                       ByVal Numero_Quadrante As Integer, ByRef quadranteReturn As Integer,
                                       ByRef messaggioErrore As String) As Decimal


        'Recupera i filtri
        Dim Ora As String = "00"
        Dim SogliaMM As Decimal = 0.01

        Dim Flag_Spazio As String

        If Coordinata_X <> 0 And Coordinata_Y <> 0 Then
            Flag_Spazio = "C"
        End If

        'crea xml x ws
        Dim objMeteo As New Meteo
        Dim XML_Parametri As System.Xml.XmlElement
        Dim Str_XML_Parametri As String = ""
        Dim Doorkey As String = "Y4h8u3B5w2"
        Dim Security_Token As String = ""
        Dim Flag_Dettagli As String = "L" 'L=info minime; H=dettagli aggiuntivi
        Dim Flag_Aggrega As String = "G" ' G=giornaliero; H= orario; I=intervallo
        Dim Numero_Stazione As Integer = 0

        Dim dblRet As Decimal = 0

        Try

            XML_Parametri = objMeteo.Crea_XMLParametri_DatiPrecipitazioni(Nothing,
                                                                        Doorkey,
                                                                        Session("ASG_Utente_Username_Crypt"),
                                                                        Session("ASG_Utente_Password_Crypt"),
                                                                        Security_Token,
                                                                        Flag_Dettagli,
                                                                        Flag_Aggrega,
                                                                        Flag_Spazio,
                                                                        SogliaMM,
                                                                        Ora,
                                                                        Numero_Quadrante,
                                                                        Numero_Stazione,
                                                                        Coordinata_X,
                                                                        Coordinata_Y,
                                                                        DataDa,
                                                                        DataA)

            Str_XML_Parametri = XML_Parametri.OuterXml

            Str_XML_Parametri = objMeteo.Cripta_XML_Parametri(Str_XML_Parametri)


        Catch ex As Exception
            messaggioErrore = "Si è verificato il seguente errore: " +
            vbCrLf + ex.Message
            Return False
        End Try

        If Str_XML_Parametri = "" Then
            messaggioErrore = "Impossibile chiamare il Web Service"
            Return False
        Else

            Dim Str_XML_Risultato As String
            Try
                Str_XML_Risultato = objMeteo.Chiama_WS_Meteo_DatiPrecipitazioni(Str_XML_Parametri)
            Catch ex As Exception
                messaggioErrore = "Non è stato recuperato alcun risultato dal Web Service: " & ex.Message
                Return False
            End Try


            If Str_XML_Risultato = "" Then
                messaggioErrore = "Non è stato recuperato alcun risultato dal Web Service"
                Return False
            Else
                Dim Num_Quadrante As Integer
                Dim Numero_Nodi_Dato As Integer
                Dim Codice_Errore As Integer
                Dim Messaggio_Errore As String
                Dim XMLs_NodiDato As System.Xml.XmlNodeList

                Try

                    'Elabora la risposta del WS
                    objMeteo.Leggi_XML_Risultato(Str_XML_Risultato,
                                                    Num_Quadrante,
                                                    Numero_Nodi_Dato,
                                                    Codice_Errore,
                                                    Messaggio_Errore,
                                                    XMLs_NodiDato)


                    quadranteReturn = Num_Quadrante

                Catch ex As Exception
                    messaggioErrore = "Si è verificato il seguente errore: " +
                    vbCrLf & ex.Message
                    Return False
                End Try

                If Codice_Errore <> 0 Then
                    messaggioErrore = "Si è verificato il seguente errore: " & vbCrLf & Messaggio_Errore
                    Return False
                Else


                    'Leggi i nodi dato e Carica il datagrid
                    If Not IsNothing(XMLs_NodiDato) AndAlso XMLs_NodiDato.Count > 0 Then

                        Try

                            Dim i As Integer
                            Dim Data_Rilievo As Date
                            Dim mm_pioggia As Decimal
                            Dim XML_Dato As System.Xml.XmlElement

                            '-------------------------------------------------------------------

                            For i = 0 To XMLs_NodiDato.Count - 1

                                XML_Dato = XMLs_NodiDato.Item(i)

                                Data_Rilievo = XML_Dato.GetAttribute("gg")
                                mm_pioggia = XML_Dato.GetAttribute("mm")

                                ' ulteriore controllo sull'intervallo temporale
                                If Data_Rilievo >= DataDa And Data_Rilievo <= DataA Then
                                    dblRet += mm_pioggia
                                End If


                            Next


                        Catch ex As Exception
                            messaggioErrore = "Si è verificato il seguente errore durante la lettura dei dati: " & vbCrLf & ex.Message
                            Return False
                        End Try

                    Else
                        messaggioErrore = "Non sono presenti dati per l'intervallo selezionato"
                    End If

                End If

            End If 'Str_XML_Risultato

        End If 'Str_XML_Parametri

        objMeteo = Nothing

        Return dblRet

    End Function

    Private Function CalcolaBilancio(ByVal IndiceRiga As Integer,
                                     ByRef N_Ammesso As Decimal,
                                     ByRef P_Ammesso As Decimal,
                                     ByRef K_Ammesso As Decimal,
                                     ByRef GrfiCod_Rer As Integer,
                                     ByRef GrfiDes_Rer As String,
                                     ByRef FaseCod As Integer,
                                     ByRef FaseDes As String,
                                     ByRef Resa As Decimal,
                                     ByRef Precessione As Integer,
                                     ByRef DispOssigeno As Integer,
                                     ByRef Sabbia As Decimal,
                                     ByRef Limo As Decimal,
                                     ByRef Argilla As Decimal,
                                     ByRef PH As Decimal,
                                     ByRef CalcTot As Decimal,
                                     ByRef CalcAtt As Decimal,
                                     ByRef CN As Decimal,
                                     ByRef SO As Decimal,
                                     ByRef N As Decimal,
                                     ByRef P As Decimal,
                                     ByRef K As Decimal,
                                     ByRef P2O5 As Decimal,
                                     ByRef K2O As Decimal,
                                     ByRef Mg As Decimal,
                                     ByRef CSC As Decimal,
                                     ByRef QtaN_FerPrec As Decimal,
                                     ByRef IdFrequenza As Integer,
                                     ByRef TipoFertilizzante As Integer,
                                     ByRef Piovosita As Decimal,
                                     ByRef ZVN As Integer,
                                     ByRef Area As String,
                                     ByRef Veg_Cod As Integer,
                                     ByRef RegimeIrriguo As Integer,
                                     ByRef Anticipazioni As Integer,
                                     ByRef AnticipazioniAnni As Integer,
                                     ByRef UbicazioneCod As Integer,
                                     ByRef PercNFissazione As Decimal,
                                     ByRef Copertura As Integer,
                                     ByRef Analisi_Cod As Integer,
                                     ByRef SeminaSodo As Integer,
                                     ByRef PiovositaFeb As Decimal,
                                        ByRef strAlert As String
                                     ) As Boolean


        Veg_Cod = grdSpecie.DataKeys(IndiceRiga).Item(2)

        If IsNumeric(CType(grdSpecie.Rows(IndiceRiga).FindControl("txtResa"), TextBox).Text) Then
            Resa = CDbl(CType(grdSpecie.Rows(IndiceRiga).FindControl("txtResa"), TextBox).Text)
        End If

        Dim ddlFinalitaRer As DropDownList
        ddlFinalitaRer = CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlFinalita"), DropDownList)
        If Not IsNothing(ddlFinalitaRer) AndAlso Not IsNothing(ddlFinalitaRer.SelectedItem) Then
            GrfiCod_Rer = ddlFinalitaRer.SelectedItem.Value
            GrfiDes_Rer = ddlFinalitaRer.SelectedItem.Text
        End If


        Dim ddlFaseCiclo As DropDownList
        ddlFaseCiclo = CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlFasi"), DropDownList)
        If Not IsNothing(ddlFaseCiclo) AndAlso Not IsNothing(ddlFaseCiclo.SelectedItem) Then
            FaseCod = ddlFaseCiclo.SelectedItem.Value
            FaseDes = ddlFaseCiclo.SelectedItem.Text
        End If


        Dim ddlAnalisi As DropDownList
        ddlAnalisi = CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlAnalisi"), DropDownList)
        If Not IsNothing(ddlAnalisi) AndAlso Not IsNothing(ddlAnalisi.SelectedItem) Then
            Analisi_Cod = ddlAnalisi.SelectedItem.Value
            If Analisi_Cod <> 0 Then
                Dim Data_Inizio_Analisi As Date = AGRODATAINIZIO
                Dim Data_Fine_Analisi As Date = AGRODATAFINE
                Dim ObjDatiAnalisi As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_R
                ObjDatiAnalisi.Carica_DatiAnalisi_xPianoConcimazione(Analisi_Cod, Sabbia, Limo, Argilla, PH, CalcTot, CalcAtt, SO, N, P2O5, K2O, CN, Mg, CSC, P, K, Data_Inizio_Analisi, Data_Fine_Analisi, objParametri_Server)
                'Carica_Analisi(Analisi_Cod, Sabbia, Limo, Argilla, PH, CalcTot, CalcAtt, SO, N, P, K, CN, Mg, CSC)
                If IsDate(Txt_ValiditaInizio.Text) AndAlso Data_Fine_Analisi < CDate(Txt_ValiditaInizio.Text) Then
                    strAlert &= "L'analisi selezionata su " & grdSpecie.Rows(IndiceRiga).Cells(1).Text &
                         " - " & grdSpecie.Rows(IndiceRiga).Cells(7).Text &
                        " è scaduta il " & Data_Fine_Analisi.ToShortDateString & "<br>"
                End If
            End If
        End If

        If IsNumeric(CType(grdSpecie.Rows(IndiceRiga).FindControl("Txt_Pioggia"), TextBox).Text) Then
            Piovosita = CDec(CType(grdSpecie.Rows(IndiceRiga).FindControl("Txt_Pioggia"), TextBox).Text)
        End If
        If IsNumeric(CType(grdSpecie.Rows(IndiceRiga).FindControl("Txt_Pioggia_Febbraio"), TextBox).Text) Then
            PiovositaFeb = CDec(CType(grdSpecie.Rows(IndiceRiga).FindControl("Txt_Pioggia_Febbraio"), TextBox).Text)
        End If

        If IsNumeric(CType(grdSpecie.Rows(IndiceRiga).FindControl("Txt_NFissazione"), TextBox).Text) Then
            PercNFissazione = CDec(CType(grdSpecie.Rows(IndiceRiga).FindControl("Txt_NFissazione"), TextBox).Text)
        End If

        If CType(grdSpecie.Rows(IndiceRiga).FindControl("ChkIrriguo"), CheckBox).Checked Then
            RegimeIrriguo = 1
        End If

        If CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlPrecessione"), DropDownList).SelectedValue Then
            Precessione = CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlPrecessione"), DropDownList).SelectedValue
        End If

        If CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlDispOss"), DropDownList).SelectedValue Then
            DispOssigeno = CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlDispOss"), DropDownList).SelectedValue
        End If

        If CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlFrequenza"), DropDownList).SelectedValue Then
            IdFrequenza = CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlFrequenza"), DropDownList).SelectedValue
        End If

        If CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlFertilizzante"), DropDownList).SelectedValue Then
            TipoFertilizzante = CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlFertilizzante"), DropDownList).SelectedValue
        End If

        If IsNumeric(CType(grdSpecie.Rows(IndiceRiga).FindControl("txtQtaNFert"), TextBox).Text) Then
            QtaN_FerPrec = CDec(CType(grdSpecie.Rows(IndiceRiga).FindControl("txtQtaNFert"), TextBox).Text)
        End If

        If CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlUbicazione"), DropDownList).SelectedValue Then
            UbicazioneCod = CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlUbicazione"), DropDownList).SelectedValue
        End If

        Dim Chk_ZVN As CheckBox
        Chk_ZVN = CType(grdSpecie.Rows(IndiceRiga).FindControl("ChkZVN"), CheckBox)
        If Not IsNothing(Chk_ZVN) Then
            ZVN = IIf(Chk_ZVN.Checked = True, 1, 0)
        End If

        Dim Chk_Serra As CheckBox
        Chk_Serra = CType(grdSpecie.Rows(IndiceRiga).FindControl("ChkSerra"), CheckBox)
        If Not IsNothing(Chk_Serra) Then
            Copertura = IIf(Chk_Serra.Checked = True, 1, 0)
        End If

        Dim Chk_Irriguo As CheckBox
        Chk_Irriguo = CType(grdSpecie.Rows(IndiceRiga).FindControl("ChkIrriguo"), CheckBox)
        If Not IsNothing(Chk_Irriguo) Then
            RegimeIrriguo = IIf(Chk_Irriguo.Checked = True, 1, 0)
        End If

        Dim Chk_SeminaSodo As CheckBox
        Chk_SeminaSodo = CType(grdSpecie.Rows(IndiceRiga).FindControl("ChkSeminaSodo"), CheckBox)
        If Not IsNothing(Chk_SeminaSodo) Then
            SeminaSodo = IIf(Chk_SeminaSodo.Checked = True, 1, 0)
        End If

        Area = CType(grdSpecie.Rows(IndiceRiga).FindControl("txtAreaOmogenea"), TextBox).Text


        Dim Txt_AnticipazioniAnni As TextBox
        Txt_AnticipazioniAnni = CType(grdSpecie.Rows(IndiceRiga).FindControl("txtAnticipazioni"), TextBox)
        If IsNumeric(Txt_AnticipazioniAnni.Text) Then
            AnticipazioniAnni = CDbl(Txt_AnticipazioniAnni.Text)
            Anticipazioni = 1
        End If


        'chiamata web service

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_input
        Dim Regolamento_Cod As Integer = CInt(ddlRegolamento.SelectedValue)
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod
        objParametriIngresso.Veg_Cod = Veg_Cod
        objParametriIngresso.Grfi_Cod = GrfiCod_Rer

        objParametriIngresso.Resa = CDec(Resa)
        objParametriIngresso.FaseCicloColturale_Cod = FaseCod
        objParametriIngresso.AnticipazioniAnni = AnticipazioniAnni

        objParametriIngresso.ColturaProtetta = Chk_Serra.Checked
        objParametriIngresso.Ubicazione_Cod = UbicazioneCod
        objParametriIngresso.FissazioneN_Perc = PercNFissazione

        objParametriIngresso.Sabbia = Sabbia
        objParametriIngresso.Argilla = Argilla
        objParametriIngresso.NTot = N
        objParametriIngresso.SO = SO
        objParametriIngresso.CN = CN
        objParametriIngresso.P2O5 = P2O5
        objParametriIngresso.K2O = K2O
        objParametriIngresso.Mg = Mg
        objParametriIngresso.CSC = CSC
        objParametriIngresso.Caco3 = CalcTot
        objParametriIngresso.DisponibilitaOssigeno_Cod = DispOssigeno
        objParametriIngresso.PioggiaMM = Piovosita
        objParametriIngresso.PioggiaMM_Febbraio = PiovositaFeb
        objParametriIngresso.Precessione_Veg_Cod = Precessione

        objParametriIngresso.FertilizzanteOrganico_ColturePrecedenti_Tipo = TipoFertilizzante
        objParametriIngresso.FertilizzanteOrganico_ColturePrecedenti_Frequenza = IdFrequenza
        objParametriIngresso.FertilizzanteOrganico_ColturePrecedenti_Qta = QtaN_FerPrec

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_output
        Dim objBilancio As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objBilancio.CalcolaBilancio(objParametriIngresso)

        N_Ammesso = objParametriUscita.N_Ammesso
        P_Ammesso = objParametriUscita.P_Ammesso
        K_Ammesso = objParametriUscita.K_Ammesso

        '(09/03/2020 fede) aggiunta eventuale considerazione del fattore correttivo per resa maggiore
        Dim MAS As Decimal
        MAS = objParametriUscita.Limite_Mas
        Dim strFattoreCorrettivo As String = ""

        If MAS >= 0 Then

            If objParametriUscita.FattoreCorrettivo_N > 0 Then
                If objParametriIngresso.Resa > 0 AndAlso objParametriUscita.Resa_Rif > 0 Then
                    If objParametriIngresso.Resa - objParametriUscita.Resa_Rif > 0 Then
                        MAS = MAS + ((objParametriIngresso.Resa - objParametriUscita.Resa_Rif) * objParametriUscita.FattoreCorrettivo_N)
                        strFattoreCorrettivo = " (" & "considerando il Fattore Correttivo di " & objParametriUscita.FattoreCorrettivo_N & " Kg N/t)"
                    End If
                End If
            End If

            If objParametriUscita.N_Ammesso > MAS Then
                LblAttenzione.Text = "ATTENZIONE: l'apporto di N non può superare il limite MAS di " & MAS.ToString & " kg/ha" & strFattoreCorrettivo
                N_Ammesso = MAS
            Else
                LblAttenzione.Text = "Limite MAS: " & MAS.ToString & " kg/ha" & strFattoreCorrettivo
            End If

        End If




        Return True

    End Function

    Private Function CalcolaSchede(ByVal IndiceRiga As Integer,
                                     ByRef N_Ammesso As Decimal,
                                     ByRef P_Ammesso As Decimal,
                                     ByRef K_Ammesso As Decimal,
                                     ByRef GrfiCod_Rer As Integer,
                                     ByRef GrfiDes_Rer As String,
                                     ByRef FaseCod As Integer,
                                     ByRef FaseDes As String,
                                     ByRef Resa As Decimal,
                                     ByRef Precessione As Integer,
                                     ByRef DispOssigeno As Integer,
                                     ByRef Sabbia As Decimal,
                                     ByRef Limo As Decimal,
                                     ByRef Argilla As Decimal,
                                     ByRef PH As Decimal,
                                     ByRef CalcTot As Decimal,
                                     ByRef CalcAtt As Decimal,
                                     ByRef CN As Decimal,
                                     ByRef SO As Decimal,
                                     ByRef N As Decimal,
                                     ByRef P As Decimal,
                                     ByRef K As Decimal,
                                     ByRef P2O5 As Decimal,
                                     ByRef K2O As Decimal,
                                     ByRef Mg As Decimal,
                                     ByRef CSC As Decimal,
                                     ByRef QtaN_FerPrec As Decimal,
                                     ByRef IdFrequenza As Integer,
                                     ByRef TipoFertilizzante As Integer,
                                     ByRef Piovosita As Decimal,
                                     ByRef ZVN As Integer,
                                     ByRef Area As String,
                                     ByRef Veg_Cod As Integer,
                                     ByRef RegimeIrriguo As Integer,
                                     ByRef Anticipazioni As Integer,
                                     ByRef AnticipazioniAnni As Integer,
                                     ByRef UbicazioneCod As Integer,
                                     ByRef PercNFissazione As Decimal,
                                     ByRef Copertura As Integer,
                                     ByRef Analisi_Cod As Integer,
                                     ByRef SeminaSodo As Integer,
                                     ByRef Fattori As String,
                                     ByRef PiovositaFeb As Decimal,
                                        ByRef strAlert As String
                                     ) As Boolean

        Veg_Cod = grdSpecie.DataKeys(IndiceRiga).Item(2)

        If IsNumeric(CType(grdSpecie.Rows(IndiceRiga).FindControl("txtResa"), TextBox).Text) Then
            Resa = CDbl(CType(grdSpecie.Rows(IndiceRiga).FindControl("txtResa"), TextBox).Text)
        End If

        Dim ddlFinalitaRer As DropDownList
        ddlFinalitaRer = CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlFinalita"), DropDownList)
        If Not IsNothing(ddlFinalitaRer) AndAlso Not IsNothing(ddlFinalitaRer.SelectedItem) Then
            GrfiCod_Rer = ddlFinalitaRer.SelectedItem.Value
            GrfiDes_Rer = ddlFinalitaRer.SelectedItem.Text
        End If


        Dim ddlFaseCiclo As DropDownList
        ddlFaseCiclo = CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlFasi"), DropDownList)
        If Not IsNothing(ddlFaseCiclo) AndAlso Not IsNothing(ddlFaseCiclo.SelectedItem) Then
            FaseCod = ddlFaseCiclo.SelectedItem.Value
            FaseDes = ddlFaseCiclo.SelectedItem.Text
        End If


        Dim ddlAnalisi As DropDownList
        ddlAnalisi = CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlAnalisi"), DropDownList)
        If Not IsNothing(ddlAnalisi) AndAlso Not IsNothing(ddlAnalisi.SelectedItem) Then
            Analisi_Cod = ddlAnalisi.SelectedItem.Value
            If Analisi_Cod <> 0 Then
                Dim Data_Inizio_Analisi As Date = AGRODATAINIZIO
                Dim Data_Fine_Analisi As Date = AGRODATAFINE
                Dim ObjDatiAnalisi As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_R
                ObjDatiAnalisi.Carica_DatiAnalisi_xPianoConcimazione(Analisi_Cod, Sabbia, Limo, Argilla, PH, CalcTot, CalcAtt, SO, N, P2O5, K2O, CN, Mg, CSC, P, K, Data_Inizio_Analisi, Data_Fine_Analisi, objParametri_Server)
                'Carica_Analisi(Analisi_Cod, Sabbia, Limo, Argilla, PH, CalcTot, CalcAtt, SO, N, P, K, CN, Mg, CSC)
                If IsDate(Txt_ValiditaInizio.Text) AndAlso Data_Fine_Analisi < CDate(Txt_ValiditaInizio.Text) Then
                    strAlert &= "L'analisi selezionata su " & grdSpecie.Rows(IndiceRiga).Cells(1).Text &
                        " - " & grdSpecie.Rows(IndiceRiga).Cells(7).Text &
                        " è scaduta il " & Data_Fine_Analisi.ToShortDateString & "<br>"
                End If

            End If
        End If

        If IsNumeric(CType(grdSpecie.Rows(IndiceRiga).FindControl("Txt_Pioggia"), TextBox).Text) Then
            Piovosita = CDbl(CType(grdSpecie.Rows(IndiceRiga).FindControl("Txt_Pioggia"), TextBox).Text)
        End If
        If IsNumeric(CType(grdSpecie.Rows(IndiceRiga).FindControl("Txt_Pioggia_Febbraio"), TextBox).Text) Then
            PiovositaFeb = CDbl(CType(grdSpecie.Rows(IndiceRiga).FindControl("Txt_Pioggia_Febbraio"), TextBox).Text)
        End If

        If IsNumeric(CType(grdSpecie.Rows(IndiceRiga).FindControl("Txt_NFissazione"), TextBox).Text) Then
            PercNFissazione = CDbl(CType(grdSpecie.Rows(IndiceRiga).FindControl("Txt_NFissazione"), TextBox).Text)
        End If

        If CType(grdSpecie.Rows(IndiceRiga).FindControl("ChkIrriguo"), CheckBox).Checked Then
            RegimeIrriguo = 1
        End If

        If CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlPrecessione"), DropDownList).SelectedValue Then
            Precessione = CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlPrecessione"), DropDownList).SelectedValue
        End If

        If CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlDispOss"), DropDownList).SelectedValue Then
            DispOssigeno = CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlDispOss"), DropDownList).SelectedValue
        End If

        If CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlFrequenza"), DropDownList).SelectedValue Then
            IdFrequenza = CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlFrequenza"), DropDownList).SelectedValue
        End If

        If CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlFertilizzante"), DropDownList).SelectedValue Then
            TipoFertilizzante = CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlFertilizzante"), DropDownList).SelectedValue
        End If

        If IsNumeric(CType(grdSpecie.Rows(IndiceRiga).FindControl("txtQtaNFert"), TextBox).Text) Then
            QtaN_FerPrec = CDbl(CType(grdSpecie.Rows(IndiceRiga).FindControl("txtQtaNFert"), TextBox).Text)
        End If

        If CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlUbicazione"), DropDownList).SelectedValue Then
            UbicazioneCod = CType(grdSpecie.Rows(IndiceRiga).FindControl("ddlUbicazione"), DropDownList).SelectedValue
        End If

        Dim Chk_ZVN As CheckBox
        Chk_ZVN = CType(grdSpecie.Rows(IndiceRiga).FindControl("ChkZVN"), CheckBox)
        If Not IsNothing(Chk_ZVN) Then
            ZVN = IIf(Chk_ZVN.Checked = True, 1, 0)
        End If

        Dim Chk_Serra As CheckBox
        Chk_Serra = CType(grdSpecie.Rows(IndiceRiga).FindControl("ChkSerra"), CheckBox)
        If Not IsNothing(Chk_Serra) Then
            Copertura = IIf(Chk_Serra.Checked = True, 1, 0)
        End If

        Dim Chk_Irriguo As CheckBox
        Chk_Irriguo = CType(grdSpecie.Rows(IndiceRiga).FindControl("ChkIrriguo"), CheckBox)
        If Not IsNothing(Chk_Irriguo) Then
            RegimeIrriguo = IIf(Chk_Irriguo.Checked = True, 1, 0)
        End If

        Dim Chk_SeminaSodo As CheckBox
        Chk_SeminaSodo = CType(grdSpecie.Rows(IndiceRiga).FindControl("ChkSeminaSodo"), CheckBox)
        If Not IsNothing(Chk_SeminaSodo) Then
            SeminaSodo = IIf(Chk_SeminaSodo.Checked = True, 1, 0)
        End If

        Area = CType(grdSpecie.Rows(IndiceRiga).FindControl("txtAreaOmogenea"), TextBox).Text

        Dim Txt_AnticipazioniAnni As TextBox
        Txt_AnticipazioniAnni = CType(grdSpecie.Rows(IndiceRiga).FindControl("txtAnticipazioni"), TextBox)
        If IsNumeric(Txt_AnticipazioniAnni.Text) Then
            AnticipazioniAnni = CDbl(Txt_AnticipazioniAnni.Text)
            Anticipazioni = 1
        End If


        'chiamata web service

        Dim Regolamento_Cod As Integer = CInt(ddlRegolamento.SelectedValue)

        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS

        ' MAS

        Dim LimiteMas As Decimal = 0
        Dim objParametriIngressoMAS As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_input
        objParametriIngressoMAS.Regolamento_Cod = Regolamento_Cod
        objParametriIngressoMAS.Veg_Cod = Veg_Cod
        objParametriIngressoMAS.Grfi_Cod = 0
        If IsNumeric(GrfiCod_Rer) Then
            objParametriIngressoMAS.Grfi_Cod = GrfiCod_Rer
        End If
        objParametriIngressoMAS.Stato_Cod = enum_Stato_Impianto.Impianto_Produzione
        objParametriIngressoMAS.ValoreMax = True

        Dim ResaRif As Decimal = 0
        Dim Mas As Decimal = -1
        Dim N_Fattore_Correttivo_Resa As Decimal = 0

        Dim objParametriUscitaMAS As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_output
        objParametriUscitaMAS = objPC_WS.LimiteMAS(objParametriIngressoMAS)

        '(09/03/2020 fede) aggiunta eventuale considerazione del fattore correttivo per resa maggiore
        If objParametriUscitaMAS.N >= 0 Then
            LimiteMas = objParametriUscitaMAS.N
        End If
        If objParametriUscitaMAS.Resa >= 0 Then
            ResaRif = objParametriUscitaMAS.Resa
        End If
        If objParametriUscitaMAS.FattoreCorrettivo_N > 0 Then
            N_Fattore_Correttivo_Resa = objParametriUscitaMAS.FattoreCorrettivo_N
        End If

        If LimiteMas > 0 Then
            If N_Fattore_Correttivo_Resa > 0 Then
                If Resa AndAlso ResaRif > 0 Then
                    If Resa - ResaRif > 0 Then
                        LimiteMas = LimiteMas + (Resa - ResaRif * N_Fattore_Correttivo_Resa)
                    End If
                End If
            End If
        End If



        ' FATTORI

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input

        objParametriIngresso.Regolamento_Cod = Regolamento_Cod
        objParametriIngresso.Veg_Cod = Veg_Cod
        objParametriIngresso.Grfi_Cod = 0
        If IsNumeric(GrfiCod_Rer) Then
            objParametriIngresso.Grfi_Cod = GrfiCod_Rer
        End If
        objParametriIngresso.SoloValorizzati = True
        objParametriIngresso.SoloVisibili = False

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output
        objParametriUscita = objPC_WS.FattoriCorrettivi_Leggi(objParametriIngresso)

        Dim Dt_Incrementi_N As New DataTable
        Dim Dt_Decrementi_N As New DataTable
        Dim Dt_Incrementi_P As New DataTable
        Dim Dt_Decrementi_P As New DataTable
        Dim Dt_Incrementi_K As New DataTable
        Dim Dt_Decrementi_K As New DataTable

        Dt_Incrementi_N.Columns.Add(New DataColumn("Fattore_Cod", GetType(Integer)))
        Dt_Incrementi_N.Columns.Add(New DataColumn("Fattore_Des", GetType(String)))
        Dt_Incrementi_N.Columns.Add(New DataColumn("Valore", GetType(String)))
        Dt_Incrementi_N.Columns.Add(New DataColumn("Controllo_Valore", GetType(Decimal)))
        Dt_Incrementi_N.Columns.Add(New DataColumn("Controllo_Funzione", GetType(String)))
        Dt_Decrementi_N.Columns.Add(New DataColumn("Fattore_Cod", GetType(Integer)))
        Dt_Decrementi_N.Columns.Add(New DataColumn("Fattore_Des", GetType(String)))
        Dt_Decrementi_N.Columns.Add(New DataColumn("Valore", GetType(String)))
        Dt_Decrementi_N.Columns.Add(New DataColumn("Controllo_Valore", GetType(Decimal)))
        Dt_Decrementi_N.Columns.Add(New DataColumn("Controllo_Funzione", GetType(String)))

        Dt_Incrementi_P.Columns.Add(New DataColumn("Fattore_Cod", GetType(Integer)))
        Dt_Incrementi_P.Columns.Add(New DataColumn("Fattore_Des", GetType(String)))
        Dt_Incrementi_P.Columns.Add(New DataColumn("Valore", GetType(String)))
        Dt_Incrementi_P.Columns.Add(New DataColumn("Controllo_Valore", GetType(Decimal)))
        Dt_Incrementi_P.Columns.Add(New DataColumn("Controllo_Funzione", GetType(String)))
        Dt_Decrementi_P.Columns.Add(New DataColumn("Fattore_Cod", GetType(Integer)))
        Dt_Decrementi_P.Columns.Add(New DataColumn("Fattore_Des", GetType(String)))
        Dt_Decrementi_P.Columns.Add(New DataColumn("Valore", GetType(String)))
        Dt_Decrementi_P.Columns.Add(New DataColumn("Controllo_Valore", GetType(Decimal)))
        Dt_Decrementi_P.Columns.Add(New DataColumn("Controllo_Funzione", GetType(String)))

        Dt_Incrementi_K.Columns.Add(New DataColumn("Fattore_Cod", GetType(Integer)))
        Dt_Incrementi_K.Columns.Add(New DataColumn("Fattore_Des", GetType(String)))
        Dt_Incrementi_K.Columns.Add(New DataColumn("Valore", GetType(String)))
        Dt_Incrementi_K.Columns.Add(New DataColumn("Controllo_Valore", GetType(Decimal)))
        Dt_Incrementi_K.Columns.Add(New DataColumn("Controllo_Funzione", GetType(String)))
        Dt_Decrementi_K.Columns.Add(New DataColumn("Fattore_Cod", GetType(Integer)))
        Dt_Decrementi_K.Columns.Add(New DataColumn("Fattore_Des", GetType(String)))
        Dt_Decrementi_K.Columns.Add(New DataColumn("Valore", GetType(String)))
        Dt_Decrementi_K.Columns.Add(New DataColumn("Controllo_Valore", GetType(Decimal)))
        Dt_Decrementi_K.Columns.Add(New DataColumn("Controllo_Funzione", GetType(String)))


        Dim ResaBassa As Decimal = 0
        Dim ResaAlta As Decimal = 0
        Dim ResaBassaDes As String = ""
        Dim ResaAltaDes As String = ""

        Dim N_Max_Incrementi As Decimal = 0
        Dim N_Dose_Standard As Decimal = 0
        Dim P_Dose_Standard As Decimal = 0
        Dim K_Dose_Standard As Decimal = 0


        If objParametriUscita.ListaFattoriCorrettivi.Count > 0 Then

            Dim FattoreResaBassa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            FattoreResaBassa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.Resa_Bassa And x.Visibile = 1)(0)
            If Not FattoreResaBassa Is Nothing Then
                ResaBassa = FattoreResaBassa.Valore
                ResaBassaDes = FattoreResaBassa.Descrizione '& " " & FattoreResaBassa.Valore
            End If


            Dim FattoreResaAlta As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            FattoreResaAlta = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.Resa_Alta And x.Visibile = 1)(0)
            If Not FattoreResaAlta Is Nothing Then
                ResaAlta = FattoreResaAlta.Valore
                ResaAltaDes = FattoreResaAlta.Descrizione '& " " & FattoreResaAlta.Valore
            End If

            ' AZOTO

            Dim Fattore_N_Inc_Max As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_N_Inc_Max = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_Inc_Max)(0)
            If Not Fattore_N_Inc_Max Is Nothing Then
                N_Max_Incrementi = Fattore_N_Inc_Max.Valore
            End If

            Dim Fattore_N_Dose_Standard As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_N_Dose_Standard = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_Dose_Standard)(0)
            If Not Fattore_N_Dose_Standard Is Nothing Then
                N_Dose_Standard = Fattore_N_Dose_Standard.Valore
            End If

            Dim Fattore_N_Inc_Resa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_N_Inc_Resa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_Inc_Resa)(0)
            If Not Fattore_N_Inc_Resa Is Nothing Then
                InserisciRigaDt(ResaAltaDes & " " & Fattore_N_Inc_Resa.Controllo_Valore, Fattore_N_Inc_Resa.Codice, Fattore_N_Inc_Resa.Valore, Fattore_N_Inc_Resa.Controllo_Funzione, Fattore_N_Inc_Resa.Controllo_Valore, Dt_Incrementi_N)
            End If

            Dim Lista_Incrementi_N As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Incrementi_N = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "N" And UCase(x.Variazione) = "INCREMENTO" And x.Visibile = 1).ToList
            For i = 0 To Lista_Incrementi_N.Count - 1
                InserisciRigaDt(Lista_Incrementi_N(i).Descrizione, Lista_Incrementi_N(i).Codice, Lista_Incrementi_N(i).Valore, Lista_Incrementi_N(i).Controllo_Funzione, Lista_Incrementi_N(i).Controllo_Valore, Dt_Incrementi_N)
            Next

            Dim Fattore_N_Dec_Resa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_N_Dec_Resa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_Dec_Resa)(0)
            If Not Fattore_N_Dec_Resa Is Nothing Then
                InserisciRigaDt(ResaBassaDes & " " & Fattore_N_Dec_Resa.Controllo_Valore, Fattore_N_Dec_Resa.Codice, Fattore_N_Dec_Resa.Valore, Fattore_N_Dec_Resa.Controllo_Funzione, Fattore_N_Dec_Resa.Controllo_Valore, Dt_Decrementi_N)
            End If

            Dim Lista_Decrementi_N As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Decrementi_N = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "N" And UCase(x.Variazione) = "DECREMENTO" And x.Visibile = 1).ToList
            For i = 0 To Lista_Decrementi_N.Count - 1
                InserisciRigaDt(Lista_Decrementi_N(i).Descrizione, Lista_Decrementi_N(i).Codice, Lista_Decrementi_N(i).Valore, Lista_Decrementi_N(i).Controllo_Funzione, Lista_Decrementi_N(i).Controllo_Valore, Dt_Decrementi_N)
            Next

            ' FOSFORO

            Dim Fattore_P_Inc_Resa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_P_Inc_Resa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_Inc_Resa)(0)
            If Not Fattore_P_Inc_Resa Is Nothing Then
                InserisciRigaDt(ResaAltaDes & " " & Fattore_P_Inc_Resa.Controllo_Valore, Fattore_P_Inc_Resa.Codice, Fattore_P_Inc_Resa.Valore, Fattore_P_Inc_Resa.Controllo_Funzione, Fattore_P_Inc_Resa.Controllo_Valore, Dt_Incrementi_P)
            End If

            Dim Lista_Incrementi_P As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Incrementi_P = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "P" And UCase(x.Variazione) = "INCREMENTO" And x.Visibile = 1).ToList
            For i = 0 To Lista_Incrementi_P.Count - 1
                InserisciRigaDt(Lista_Incrementi_P(i).Descrizione, Lista_Incrementi_P(i).Codice, Lista_Incrementi_P(i).Valore, Lista_Incrementi_P(i).Controllo_Funzione, Lista_Incrementi_P(i).Controllo_Valore, Dt_Incrementi_P)
            Next

            Dim Fattore_P_Dec_Resa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_P_Dec_Resa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_Dec_Resa)(0)
            If Not Fattore_P_Dec_Resa Is Nothing Then
                InserisciRigaDt(ResaBassaDes & " " & Fattore_P_Dec_Resa.Controllo_Valore, Fattore_P_Dec_Resa.Codice, Fattore_P_Dec_Resa.Valore, Fattore_P_Dec_Resa.Controllo_Funzione, Fattore_P_Dec_Resa.Controllo_Valore, Dt_Decrementi_P)
            End If

            Dim Lista_Decrementi_P As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Decrementi_P = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "P" And UCase(x.Variazione) = "DECREMENTO" And x.Visibile = 1).ToList
            For i = 0 To Lista_Decrementi_P.Count - 1
                InserisciRigaDt(Lista_Decrementi_P(i).Descrizione, Lista_Decrementi_P(i).Codice, Lista_Decrementi_P(i).Valore, Lista_Decrementi_P(i).Controllo_Funzione, Lista_Decrementi_P(i).Controllo_Valore, Dt_Decrementi_P)
            Next

            Dim Lista_Dose_P As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Dose_P = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "P" And UCase(x.Variazione) = "STANDARD" And x.Visibile = 1).ToList
            ddlDoseP.Items.Clear()
            For i = 0 To Lista_Dose_P.Count - 1
                ddlDoseP.Items.Add(New ListItem(Lista_Dose_P(i).Descrizione, Lista_Dose_P(i).Codice & "|" & Lista_Dose_P(i).Valore))
            Next
            If Not ddlDoseP Is Nothing AndAlso ddlDoseP.Items.Count > 0 Then
                P_Dose_Standard = ddlDoseP.SelectedValue.Split("|")(1)
            End If

            'POTASSIO

            Dim Fattore_K_Inc_Resa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_K_Inc_Resa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_Inc_Resa)(0)
            If Not Fattore_K_Inc_Resa Is Nothing Then
                InserisciRigaDt(ResaAltaDes & " " & Fattore_K_Inc_Resa.Controllo_Valore, Fattore_K_Inc_Resa.Codice, Fattore_K_Inc_Resa.Valore, Fattore_K_Inc_Resa.Controllo_Funzione, Fattore_K_Inc_Resa.Controllo_Valore, Dt_Incrementi_K)
            End If

            Dim Lista_Incrementi_K As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Incrementi_K = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "K" And UCase(x.Variazione) = "INCREMENTO" And x.Visibile = 1).ToList
            For i = 0 To Lista_Incrementi_K.Count - 1
                InserisciRigaDt(Lista_Incrementi_K(i).Descrizione, Lista_Incrementi_K(i).Codice, Lista_Incrementi_K(i).Valore, Lista_Incrementi_K(i).Controllo_Funzione, Lista_Incrementi_K(i).Controllo_Valore, Dt_Incrementi_K)
            Next

            Dim Fattore_K_Dec_Resa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_K_Dec_Resa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_Dec_Resa)(0)
            If Not Fattore_K_Dec_Resa Is Nothing Then
                InserisciRigaDt(ResaBassaDes & " " & Fattore_K_Dec_Resa.Controllo_Valore, Fattore_K_Dec_Resa.Codice, Fattore_K_Dec_Resa.Valore, Fattore_K_Dec_Resa.Controllo_Funzione, Fattore_K_Dec_Resa.Controllo_Valore, Dt_Decrementi_K)
            End If

            Dim Lista_Decrementi_K As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Decrementi_K = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "K" And UCase(x.Variazione) = "DECREMENTO" And x.Visibile = 1).ToList
            For i = 0 To Lista_Decrementi_K.Count - 1
                InserisciRigaDt(Lista_Decrementi_K(i).Descrizione, Lista_Decrementi_K(i).Codice, Lista_Decrementi_K(i).Valore, Lista_Decrementi_K(i).Controllo_Funzione, Lista_Decrementi_K(i).Controllo_Valore, Dt_Decrementi_K)
            Next

            Dim Lista_Dose_K As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Dose_K = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "K" And UCase(x.Variazione) = "STANDARD" And x.Visibile = 1).ToList
            ddlDoseK.Items.Clear()
            For i = 0 To Lista_Dose_K.Count - 1
                ddlDoseK.Items.Add(New ListItem(Lista_Dose_K(i).Descrizione, Lista_Dose_K(i).Codice & "|" & Lista_Dose_K(i).Valore))
            Next
            If Not ddlDoseK Is Nothing AndAlso ddlDoseK.Items.Count > 0 Then
                K_Dose_Standard = ddlDoseK.SelectedValue.Split("|")(1)
            End If

            Dim DtKeys(3) As String
            DtKeys(0) = "Fattore_Cod"
            DtKeys(1) = "Valore"
            DtKeys(2) = "Controllo_Funzione"
            DtKeys(3) = "Controllo_Valore"


            grdIncrementi.DataSource = Dt_Incrementi_N
            grdIncrementi.DataKeyNames = DtKeys
            grdIncrementi.DataBind()
            grdDecrementi.DataSource = Dt_Decrementi_N
            grdDecrementi.DataKeyNames = DtKeys
            grdDecrementi.DataBind()
            grdIncrementiP.DataSource = Dt_Incrementi_P
            grdIncrementiP.DataKeyNames = DtKeys
            grdIncrementiP.DataBind()
            grdDecrementiP.DataSource = Dt_Decrementi_P
            grdDecrementiP.DataKeyNames = DtKeys
            grdDecrementiP.DataBind()
            grdIncrementiK.DataSource = Dt_Incrementi_K
            grdIncrementiK.DataKeyNames = DtKeys
            grdIncrementiK.DataBind()
            grdDecrementiK.DataSource = Dt_Decrementi_K
            grdDecrementiK.DataKeyNames = DtKeys
            grdDecrementiK.DataBind()

            Select Case FaseCod

                Case enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto

                    N_Ammesso = 0
                    P_Ammesso = 0
                    K_Ammesso = 0

                Case enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento

                    Dim Fattore_N_I_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Fattore_N_I_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_I_anno_allevamento)(0)
                    If Not Fattore_N_I_Anno_Allevamento Is Nothing Then
                        N_Dose_Standard = Fattore_N_I_Anno_Allevamento.Valore
                    End If

                    Dim Fattore_P_I_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Fattore_P_I_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_I_anno_allevamento)(0)
                    If Not Fattore_P_I_Anno_Allevamento Is Nothing Then
                        P_Dose_Standard = Fattore_P_I_Anno_Allevamento.Valore
                    End If

                    Dim Fattore_K_I_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Fattore_K_I_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_I_anno_allevamento)(0)
                    If Not Fattore_K_I_Anno_Allevamento Is Nothing Then
                        K_Dose_Standard = Fattore_K_I_Anno_Allevamento.Valore
                    End If

                    N_Ammesso = N_Dose_Standard
                    P_Ammesso = P_Dose_Standard
                    K_Ammesso = K_Dose_Standard

                Case enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento

                    Dim Fattore_N_II_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Fattore_N_II_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_II_anno_allevamento)(0)
                    If Not Fattore_N_II_Anno_Allevamento Is Nothing Then
                        N_Dose_Standard = Fattore_N_II_Anno_Allevamento.Valore
                    End If

                    Dim Fattore_P_II_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Fattore_P_II_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_II_anno_allevamento)(0)
                    If Not Fattore_P_II_Anno_Allevamento Is Nothing Then
                        P_Dose_Standard = Fattore_P_II_Anno_Allevamento.Valore
                    End If

                    Dim Fattore_K_II_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Fattore_K_II_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_II_anno_allevamento)(0)
                    If Not Fattore_K_II_Anno_Allevamento Is Nothing Then
                        K_Dose_Standard = Fattore_K_II_Anno_Allevamento.Valore
                    End If

                    N_Ammesso = N_Dose_Standard
                    P_Ammesso = P_Dose_Standard
                    K_Ammesso = K_Dose_Standard

                Case Else

                    ' N

                    Dim doseStd As Decimal = N_Dose_Standard
                    Dim doseMas As Decimal = LimiteMas
                    Dim maxIncr As Decimal = N_Max_Incrementi

                    Dim totIncN As Decimal = ImpostaFattoriDefault(grdIncrementi, "Chk_SelIncrementi_N", Resa, Argilla, Sabbia, SO, CalcAtt, Piovosita, PiovositaFeb, Fattori)
                    Dim totDecN As Decimal = ImpostaFattoriDefault(grdDecrementi, "Chk_SelDecrementi_N", Resa, Argilla, Sabbia, SO, CalcAtt, Piovosita, PiovositaFeb, Fattori)

                    Dim doseRicN As Decimal
                    doseRicN = doseStd + totIncN - totDecN

                    If doseRicN > doseStd + maxIncr Then
                        doseRicN = doseStd + maxIncr
                    End If

                    If doseMas <> 0 Then
                        If doseRicN > doseMas Then
                            doseRicN = doseMas
                        End If
                    End If

                    N_Ammesso = doseRicN

                    ' P
                    Dim doseStdP As Decimal = 0
                    ImpostaDoseStandard_P2O5_Default(P2O5, Fattori, doseStdP)

                    Dim totIncP As Decimal = ImpostaFattoriDefault(grdIncrementiP, "Chk_SelIncrementi_P", Resa, Argilla, Sabbia, SO, CalcAtt, Piovosita, PiovositaFeb, Fattori)
                    Dim totDecP As Decimal = ImpostaFattoriDefault(grdDecrementiP, "Chk_SelDecrementi_P", Resa, Argilla, Sabbia, SO, CalcAtt, Piovosita, PiovositaFeb, Fattori)

                    Dim doseRicP As Decimal
                    doseRicP = doseStdP + totIncP - totDecP
                    P_Ammesso = doseRicP

                    ' K
                    Dim doseStdk As Decimal = 0
                    ImpostaDoseStandard_K2O_Default(Argilla, Sabbia, K2O, Mg, CSC, Fattori, doseStdk)

                    Dim totIncK As Decimal = ImpostaFattoriDefault(grdIncrementiK, "Chk_SelIncrementi_K", Resa, Argilla, Sabbia, SO, CalcAtt, Piovosita, PiovositaFeb, Fattori)
                    Dim totDecK As Decimal = ImpostaFattoriDefault(grdDecrementiK, "Chk_SelDecrementi_K", Resa, Argilla, Sabbia, SO, CalcAtt, Piovosita, PiovositaFeb, Fattori)

                    Dim doseRicK As Decimal
                    doseRicK = doseStdk + totIncK - totDecK
                    K_Ammesso = doseRicK

            End Select

        End If

        Return True

    End Function

    Private Sub InserisciRigaDt(ByVal Fattore_Des As String, ByVal Fattore_Cod As Integer,
                                ByVal Valore As Decimal,
                                ByVal Controllo_Funzione As String, ByVal Controllo_Valore As Decimal,
                                ByRef Dt As DataTable)

        Dim Dr As DataRow
        Dr = Dt.NewRow()
        Dr.Item("Fattore_Des") = Fattore_Des
        Dr.Item("Fattore_Cod") = Fattore_Cod
        Dr.Item("Valore") = Valore
        Dr.Item("Controllo_Funzione") = Controllo_Funzione
        Dr.Item("Controllo_Valore") = Controllo_Valore
        Dt.Rows.Add(Dr)

    End Sub
    Private Function ImpostaFattoriDefault(ByVal gv As GridView, ByVal IdCheck As String,
                                           ByVal Resa As Decimal,
                                           ByVal Argilla As Decimal, ByVal Sabbia As Decimal,
                                           ByVal SO As Decimal, ByVal CalcAtt As Decimal,
                                           ByVal PioggiaInv As Decimal, ByVal PioggiaFeb As Decimal,
                                           ByRef Fattori As String) As Decimal

        Dim TotValori As Decimal = 0

        Dim j As Integer
        Dim Controllo_Valore As Decimal
        Dim Controllo_Funzione As String

        For j = 0 To gv.Rows.Count - 1

            Controllo_Valore = gv.DataKeys(j).Item("Controllo_Valore")
            Controllo_Funzione = gv.DataKeys(j).Item("Controllo_Funzione")

            Select Case Controllo_Funzione

                Case "Resa_Bassa"
                    If Resa_Bassa(Controllo_Valore, Resa) = True Then
                        CType(gv.Rows(j).FindControl(IdCheck), CheckBox).Checked = True
                        Fattori &= gv.DataKeys(j).Item("Fattore_Cod") & ","
                        TotValori += gv.DataKeys(j).Item("Valore")
                    End If

                Case "Resa_Alta"
                    If Resa_Alta(Controllo_Valore, Resa) = True Then
                        CType(gv.Rows(j).FindControl(IdCheck), CheckBox).Checked = True
                        Fattori &= gv.DataKeys(j).Item("Fattore_Cod") & ","
                        TotValori += gv.DataKeys(j).Item("Valore")
                    End If

                Case "SO_Bassa"
                    If SO_Bassa(Argilla, Sabbia, SO) = True Then
                        CType(gv.Rows(j).FindControl(IdCheck), CheckBox).Checked = True
                        Fattori &= gv.DataKeys(j).Item("Fattore_Cod") & ","
                        TotValori += gv.DataKeys(j).Item("Valore")
                    End If

                Case "SO_Elevata"
                    If SO_Elevata(Argilla, Sabbia, SO) = True Then
                        CType(gv.Rows(j).FindControl(IdCheck), CheckBox).Checked = True
                        Fattori &= gv.DataKeys(j).Item("Fattore_Cod") & ","
                        TotValori += gv.DataKeys(j).Item("Valore")
                    End If

                Case "SO_MoltoElevata"
                    If SO_MoltoElevata(Controllo_Valore, SO) = True Then
                        CType(gv.Rows(j).FindControl(IdCheck), CheckBox).Checked = True
                        Fattori &= gv.DataKeys(j).Item("Fattore_Cod") & ","
                        TotValori += gv.DataKeys(j).Item("Valore")
                    End If

                Case "Lisciviazione_Forte"
                    If Lisciviazione_Forte(Controllo_Valore, PioggiaInv, PioggiaFeb) = True Then
                        CType(gv.Rows(j).FindControl(IdCheck), CheckBox).Checked = True
                        Fattori &= gv.DataKeys(j).Item("Fattore_Cod") & ","
                        TotValori += gv.DataKeys(j).Item("Valore")
                    End If

                Case "CalcAtt_Elevato"
                    If CalcAtt_Elevato(CalcAtt) = True Then
                        CType(gv.Rows(j).FindControl(IdCheck), CheckBox).Checked = True
                        Fattori &= gv.DataKeys(j).Item("Fattore_Cod") & ","
                        TotValori += gv.DataKeys(j).Item("Valore")
                    End If

            End Select


        Next


        Return TotValori

    End Function


    Public Function Resa_Alta(ByVal Controllo_Valore As Decimal, ByVal Resa As Decimal) As Boolean

        If IsNumeric(Resa) AndAlso IsNumeric(Controllo_Valore) Then
            If CDec(Resa) > CDec(Controllo_Valore) Then
                Return True
            End If
        End If

        Return False

    End Function

    Public Function Resa_Bassa(ByVal Controllo_Valore As Decimal, ByVal Resa As Decimal) As Boolean

        If IsNumeric(Resa) AndAlso IsNumeric(Controllo_Valore) Then
            If CDec(Resa) < CDec(Controllo_Valore) Then
                Return True
            End If
        End If

        Return False

    End Function

    Public Function Lisciviazione_Forte(ByVal Controllo_Valore As Decimal, ByVal PioggiaInv As Decimal, ByVal PioggiaFeb As Decimal) As Boolean

        Dim PioggiaTot As Decimal

        PioggiaTot = PioggiaInv + PioggiaFeb

        If IsNumeric(Controllo_Valore) Then
            If PioggiaTot > CDec(Controllo_Valore) Then
                Return True
            End If
        End If

        Return False

    End Function

    Public Function SO_Elevata(ByVal Argilla As Decimal, ByVal Sabbia As Decimal, ByVal SO As Decimal) As Boolean

        Dim objParametriIngressoSuolo As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Suolo_input
        objParametriIngressoSuolo.Sabbia = CDec(Sabbia)
        objParametriIngressoSuolo.Argilla = CDec(Argilla)
        objParametriIngressoSuolo.SO = CDec(SO)
        objParametriIngressoSuolo.Regolamento_Cod = CInt(ddlRegolamento.SelectedValue)

        Dim objParametriUscitaSuolo As enum_PianoConcimazione_Dotazione
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscitaSuolo = objPC_WS.So_Dotazione(objParametriIngressoSuolo)

        Select Case objParametriUscitaSuolo
            Case enum_PianoConcimazione_Dotazione.Elevata
                Return True
        End Select

        Return False

    End Function

    Public Function SO_MoltoElevata(ByVal Controllo_Valore As Decimal, ByVal SO As Decimal) As Boolean

        If IsNumeric(SO) AndAlso IsNumeric(Controllo_Valore) Then
            If CDec(SO) > CDec(Controllo_Valore) Then
                Return True
            End If
        End If

        Return False

    End Function

    Public Function SO_Bassa(ByVal Argilla As Decimal, ByVal Sabbia As Decimal, ByVal SO As Decimal) As Boolean

        Dim objParametriIngressoSuolo As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Suolo_input
        objParametriIngressoSuolo.Sabbia = CDec(Sabbia)
        objParametriIngressoSuolo.Argilla = CDec(Argilla)
        objParametriIngressoSuolo.SO = CDec(SO)
        objParametriIngressoSuolo.Regolamento_Cod = CInt(ddlRegolamento.SelectedValue)

        Dim objParametriUscitaSuolo As enum_PianoConcimazione_Dotazione
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscitaSuolo = objPC_WS.So_Dotazione(objParametriIngressoSuolo)

        Select Case objParametriUscitaSuolo
            Case enum_PianoConcimazione_Dotazione.Bassa, enum_PianoConcimazione_Dotazione.MoltoBassa
                Return True
        End Select

        Return False

    End Function

    Public Function CalcAtt_Elevato(ByVal CalcAtt As Decimal) As Boolean

        Dim objParametriIngressoSuolo As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Suolo_input
        objParametriIngressoSuolo.CalcAtt = CDec(CalcAtt)
        objParametriIngressoSuolo.Regolamento_Cod = CInt(ddlRegolamento.SelectedValue)

        Dim objParametriUscitaSuolo As enum_PianoConcimazione_Dotazione
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscitaSuolo = objPC_WS.CalcAtt_Dotazione(objParametriIngressoSuolo)

        Select Case objParametriUscitaSuolo
            Case enum_PianoConcimazione_Dotazione.MoltoElevata
                Return True
        End Select

        Return False

    End Function

    Public Function ImpostaDoseStandard_P2O5_Default(ByVal P As Decimal, ByRef Fattori As String, ByRef doseStdP As Decimal)

        Dim objParametriIngressoSuolo As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Suolo_input
        objParametriIngressoSuolo.P2O5 = P
        objParametriIngressoSuolo.Regolamento_Cod = CInt(ddlRegolamento.SelectedValue)

        Dim objParametriUscitaSuolo As enum_PianoConcimazione_Dotazione
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscitaSuolo = objPC_WS.P2O5_Dotazione(objParametriIngressoSuolo)

        Select Case objParametriUscitaSuolo

            Case enum_PianoConcimazione_Dotazione.MoltoBassa
                For j = 0 To ddlDoseP.Items.Count - 1
                    If ddlDoseP.Items(j).Value.Split("|")(0) = enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard_MoltoBassa Then
                        ddlDoseP.SelectedIndex = j
                        doseStdP = ddlDoseP.SelectedValue.Split("|")(1)
                        Fattori &= ddlDoseP.SelectedValue.Split("|")(0) & ","
                        Exit For
                    End If
                Next

            Case enum_PianoConcimazione_Dotazione.Bassa
                For j = 0 To ddlDoseP.Items.Count - 1
                    If ddlDoseP.Items(j).Value.Split("|")(0) = enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard_Bassa Then
                        ddlDoseP.SelectedIndex = j
                        doseStdP = ddlDoseP.SelectedValue.Split("|")(1)
                        Fattori &= ddlDoseP.SelectedValue.Split("|")(0) & ","
                        Exit For
                    End If
                Next

                '(05/03/2019 fede) spostato giudizio elevato su dotazione normale
            Case enum_PianoConcimazione_Dotazione.Media, enum_PianoConcimazione_Dotazione.Elevata
                For j = 0 To ddlDoseP.Items.Count - 1
                    If ddlDoseP.Items(j).Value.Split("|")(0) = enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard Then
                        ddlDoseP.SelectedIndex = j
                        doseStdP = ddlDoseP.SelectedValue.Split("|")(1)
                        Fattori &= ddlDoseP.SelectedValue.Split("|")(0) & ","
                        Exit For
                    End If
                Next

            Case enum_PianoConcimazione_Dotazione.MoltoElevata
                For j = 0 To ddlDoseP.Items.Count - 1
                    If ddlDoseP.Items(j).Value.Split("|")(0) = enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard_Elevata Then
                        ddlDoseP.SelectedIndex = j
                        doseStdP = ddlDoseP.SelectedValue.Split("|")(1)
                        Fattori &= ddlDoseP.SelectedValue.Split("|")(0) & ","
                        Exit For
                    End If
                Next

        End Select

    End Function

    Public Function ImpostaDoseStandard_K2O_Default(ByVal Argilla As Decimal, ByVal Sabbia As Decimal,
                                                    ByVal K As Decimal, ByVal Mg As Decimal, ByVal CSC As Decimal,
                                                    ByRef Fattori As String,
                                                    ByRef doseStdk As Decimal)

        Dim objParametriIngressoSuolo As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Suolo_input
        objParametriIngressoSuolo.K2O = K
        objParametriIngressoSuolo.Sabbia = Sabbia
        objParametriIngressoSuolo.Argilla = Argilla

        objParametriIngressoSuolo.Regolamento_Cod = CInt(ddlRegolamento.SelectedValue)

        Select Case CInt(ddlRegolamento.SelectedValue)
            Case Is >= enum_PUARegolamenti.PianoConcimazione_2017
                objParametriIngressoSuolo.Mg = Mg
                objParametriIngressoSuolo.CSC = CSC
        End Select

        Dim objParametriUscitaSuolo As enum_PianoConcimazione_Dotazione
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscitaSuolo = objPC_WS.K2O_Dotazione(objParametriIngressoSuolo)

        Select Case objParametriUscitaSuolo

            Case enum_PianoConcimazione_Dotazione.MoltoBassa, enum_PianoConcimazione_Dotazione.Bassa
                For j = 0 To ddlDoseK.Items.Count - 1
                    If ddlDoseK.Items(j).Value.Split("|")(0) = enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard_Bassa Then
                        ddlDoseK.SelectedIndex = j
                        doseStdk = ddlDoseK.SelectedValue.Split("|")(1)
                        Fattori &= ddlDoseK.SelectedValue.Split("|")(0) & ","
                        Exit For
                    End If
                Next


            Case enum_PianoConcimazione_Dotazione.Media
                For j = 0 To ddlDoseK.Items.Count - 1
                    If ddlDoseK.Items(j).Value.Split("|")(0) = enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard Then
                        ddlDoseK.SelectedIndex = j
                        doseStdk = ddlDoseK.SelectedValue.Split("|")(1)
                        Fattori &= ddlDoseK.SelectedValue.Split("|")(0) & ","
                        Exit For
                    End If
                Next

            Case enum_PianoConcimazione_Dotazione.Elevata, enum_PianoConcimazione_Dotazione.MoltoElevata
                For j = 0 To ddlDoseK.Items.Count - 1
                    If ddlDoseK.Items(j).Value.Split("|")(0) = enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard_Elevata Then
                        ddlDoseK.SelectedIndex = j
                        doseStdk = ddlDoseK.SelectedValue.Split("|")(1)
                        Fattori &= ddlDoseK.SelectedValue.Split("|")(0) & ","
                        Exit For
                    End If
                Next

        End Select

    End Function

    'Private Sub Carica_Analisi(ByVal Analisi_Testata_Cod As Integer, ByRef Sabbia As String, ByRef Limo As String, ByRef Argilla As String,
    '                           ByRef Ph As String, ByRef CalcTot As String, ByRef CalcAtt As String, ByRef SO As String,
    '                           ByRef N As String, ByRef P As String, ByRef K As String, ByRef CN As String, ByRef Mg As String, ByRef CSC As String)


    '    Dim Parametro_Cod As Integer
    '    Dim Dettaglio_Cod As Integer
    '    Dim Valore As Decimal

    '    'Leggo Analisi_Dettagli se ho selezionato un' "Analisi_Testata_Cod"
    '    Dim objAnalisiD As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_R
    '    Dim DtAnalisiD As New DataTable
    '    DtAnalisiD = objAnalisiD.Leggi(Analisi_Testata_Cod,
    '                                   0, 0,
    '                                   AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
    '                                   "Analisi_Parametro_Cod IN (3,4,5,2,6,8,7,9,12,18,20,23,11,84,85)", "", objParametri_Server)

    '    If Not IsNothing(DtAnalisiD) AndAlso DtAnalisiD.Rows.Count > 0 Then

    '        Dim i As Integer
    '        For i = 0 To DtAnalisiD.Rows.Count - 1

    '            Parametro_Cod = DtAnalisiD.Rows(i).Item("analisi_parametro_cod")
    '            Dettaglio_Cod = DtAnalisiD.Rows(i).Item("analisi_dettaglio_cod")
    '            If Not IsDBNull(DtAnalisiD.Rows(i).Item("analisi_dettaglio_valore_1")) Then
    '                Valore = DtAnalisiD.Rows(i).Item("analisi_dettaglio_valore_1")
    '            Else
    '                Valore = 0
    '            End If

    '            If Valore = 0 Then
    '                Valore = objAnalisiD.Leggi_ValoreParametro_AnalisiPrecedente(Analisi_Testata_Cod, Parametro_Cod, "Analisi_Testata.Analisi_Testata_Tipo = " & enum_AnalisiTipo.Analisi_Terreno, "", objParametri_Server)
    '            End If

    '            Select Case Parametro_Cod

    '                Case enum_AnalisiParametri.AnalisiParametri_Sabbia

    '                    Sabbia = CStr(Valore)

    '                Case enum_AnalisiParametri.AnalisiParametri_Limo

    '                    Limo = CStr(Valore)

    '                Case enum_AnalisiParametri.AnalisiParametri_Argilla

    '                    Argilla = CStr(Valore)

    '                Case enum_AnalisiParametri.AnalisiParametri_pH

    '                    Ph = CStr(Valore)

    '                Case enum_AnalisiParametri.AnalisiParametri_CaCO3

    '                    CalcTot = CStr(Valore)

    '                Case enum_AnalisiParametri.AnalisiParametri_CaCO3_Attivo

    '                    CalcAtt = CStr(Valore)

    '                Case enum_AnalisiParametri.AnalisiParametri_SostanzaOrganica

    '                    SO = CStr(Valore)

    '                Case enum_AnalisiParametri.AnalisiParametri_Ntot

    '                    N = CStr(Valore)

    '                Case enum_AnalisiParametri.AnalisiParametri_P2O5_assimilabile

    '                    P = CStr(Valore)

    '                Case enum_AnalisiParametri.AnalisiParametri_K2O_scambiabile

    '                    K = CStr(Valore)

    '                Case enum_AnalisiParametri.AnalisiParametri_K2O_assimilabile

    '                    If K = 0 Then
    '                        K = CStr(Valore)
    '                    End If


    '                Case enum_AnalisiParametri.AnalisiParametri_Mg_assimilabile

    '                    Mg = CStr(Valore)

    '                Case enum_AnalisiParametri.AnalisiParametri_CSC

    '                    CSC = CStr(Valore)

    '                Case enum_AnalisiParametri.AnalisiParametri_RapportoCN

    '                    CN = CStr(Valore)

    '                Case enum_AnalisiParametri.AnalisiParametri_CarbonatiTotali

    '                    If CalcTot = 0 Then
    '                        CalcTot = CStr(Valore)
    '                    End If

    '            End Select
    '        Next


    '    End If



    'End Sub

    Private Sub Btn_Bilancio_Click(sender As Object, e As EventArgs) Handles Btn_Bilancio.Click

        Dim Dt As New DataTable
        Dim Dr As DataRow
        GridViewBilanci.DataSource = Dt
        GridViewBilanci.DataBind()

        Dt = CaricaGriglia_Bilanci()

        Dim Contatore As Integer = 0

        Dim N_Ammesso As Decimal
        Dim P_Ammesso As Decimal
        Dim K_Ammesso As Decimal

        Dim Veg_Cod As Integer = 0
        Dim Anno As Integer = 0
        Dim Resa As Decimal = 0
        Dim Analisi_Cod As Integer = 0
        Dim Sabbia As Decimal = 0
        Dim Limo As Decimal = 0
        Dim Argilla As Decimal = 0
        Dim Ph As Decimal = 0
        Dim CalcTot As Decimal = 0
        Dim CalcAtt As Decimal = 0
        Dim N As Decimal = 0
        Dim SO As Decimal = 0
        Dim CN As Decimal = 0
        Dim P As Decimal = 0
        Dim K As Decimal = 0
        Dim P2O5 As Decimal = 0
        Dim K2O As Decimal = 0
        Dim Mg As Decimal = 0
        Dim CSC As Decimal = 0
        Dim Piovosita As Decimal = 0
        Dim PiovositaFeb As Decimal = 0
        Dim AnticipazioniAnni As Integer = 0
        Dim Anticipazioni As Integer = 0
        Dim Precessione As Integer
        Dim PercNFissazione As Decimal
        Dim DispOssigeno As Integer
        Dim TipoFertilizzante As Integer
        Dim IdFrequenza As Integer
        Dim UbicazioneCod As Integer = 0
        Dim Copertura As Integer = 0
        Dim Zvn As Integer = 0
        Dim GrfiDes_Rer As String = ""
        Dim GrfiCod_Rer As Integer = 0
        Dim RegimeIrriguo As Integer = 0
        Dim FaseDes As String = ""
        Dim FaseCod As Integer = 0
        Dim QtaN_FerPrec As Decimal = 0
        Dim Area As String = ""
        Dim SeminaSodo As Integer = 0

        Dim strAlert As String = ""

        For i = 0 To grdSpecie.Rows.Count - 1

            If CType(grdSpecie.Rows(i).FindControl("ChkSeleziona"), CheckBox).Checked = True Then

                N_Ammesso = 0
                P_Ammesso = 0
                K_Ammesso = 0

                CalcolaBilancio(i, N_Ammesso, P_Ammesso, K_Ammesso,
                                GrfiCod_Rer, GrfiDes_Rer, FaseCod, FaseDes, Resa, Precessione, DispOssigeno,
                                Sabbia, Limo, Argilla, Ph, CalcTot, CalcAtt, CN, SO, N, P, K, P2O5, K2O, Mg, CSC,
                                QtaN_FerPrec, IdFrequenza, TipoFertilizzante,
                                Piovosita, Zvn, Area, Veg_Cod, RegimeIrriguo, Anticipazioni, AnticipazioniAnni,
                                UbicazioneCod, PercNFissazione, Copertura, Analisi_Cod, SeminaSodo, PiovositaFeb, strAlert)

                Dr = Dt.NewRow

                Dr.Item("Contatore") = Contatore
                Contatore += 1

                Dr.Item("Piano_Des") = Txt_Descrizione.Text

                Dr.Item("Piva") = grdSpecie.DataKeys(i).Item(0).ToString
                Dr.Item("Sa_Cod") = grdSpecie.DataKeys(i).Item(1).ToString
                Dr.Item("Centro") = grdSpecie.DataKeys(i).Item(6).ToString
                Dr.Item("Impianti") = grdSpecie.DataKeys(i).Item(7).ToString
                Dr.Item("AppezzaIdimp") = grdSpecie.DataKeys(i).Item(4).ToString

                Dr.Item("Veg_Cod") = Veg_Cod
                Dr.Item("Veg_Des") = grdSpecie.DataKeys(i).Item(5).ToString
                Dr.Item("Grfi_Cod") = GrfiCod_Rer
                Dr.Item("Grfi_Des") = GrfiDes_Rer
                Dr.Item("Fase_Cod") = FaseCod
                Dr.Item("Fase_Des") = FaseDes

                Dr.Item("Resa") = Resa

                Dr.Item("Precessione") = Precessione
                Dr.Item("DispOssigeno") = DispOssigeno
                Dr.Item("Sabbia") = Sabbia
                Dr.Item("Limo") = Limo
                Dr.Item("Argilla") = Argilla
                Dr.Item("Ph") = Ph
                Dr.Item("CalcTot") = CalcTot
                Dr.Item("CalcAtt") = CalcAtt
                Dr.Item("CN") = CN
                Dr.Item("SO") = SO
                Dr.Item("N") = N
                Dr.Item("P") = P
                Dr.Item("K") = K
                Dr.Item("Mg") = Mg
                Dr.Item("CSC") = CSC
                Dr.Item("QtaN_FerPrec") = QtaN_FerPrec
                Dr.Item("IdFrequenza") = IdFrequenza
                Dr.Item("TipoFertilizzante") = TipoFertilizzante
                Dr.Item("Piovosita") = Piovosita
                Dr.Item("PiovositaFeb") = PiovositaFeb
                Dr.Item("Zvn") = Zvn
                Dr.Item("Anno") = Anno
                Dr.Item("Area") = Area
                Dr.Item("RegimeIrriguo") = RegimeIrriguo
                Dr.Item("Anticipazioni") = Anticipazioni
                Dr.Item("AnticipazioniAnni") = AnticipazioniAnni
                Dr.Item("UbicazioneCod") = UbicazioneCod
                Dr.Item("PercNFissazione") = PercNFissazione
                Dr.Item("Copertura") = Copertura
                Dr.Item("Analisi_Cod") = Analisi_Cod
                Dr.Item("SeminaSodo") = SeminaSodo

                Dr.Item("N_Ammesso") = N_Ammesso
                Dr.Item("P_Ammesso") = P_Ammesso
                Dr.Item("K_Ammesso") = K_Ammesso

                Dr.Item("Fattori") = ""

                Dt.Rows.Add(Dr)

            End If

        Next


        Dim NomiChiavi(42) As String
        NomiChiavi(0) = "Piva"
        NomiChiavi(1) = "Sa_Cod"
        NomiChiavi(2) = "AppezzaIdimp"
        NomiChiavi(3) = "N_Ammesso"
        NomiChiavi(4) = "P_Ammesso"
        NomiChiavi(5) = "K_Ammesso"
        NomiChiavi(6) = "Grfi_Cod"
        NomiChiavi(7) = "Fase_Cod"
        NomiChiavi(8) = "Fase_Des"
        NomiChiavi(9) = "Resa"
        NomiChiavi(10) = "Precessione"
        NomiChiavi(11) = "DispOssigeno"
        NomiChiavi(12) = "Sabbia"
        NomiChiavi(13) = "Limo"
        NomiChiavi(14) = "Argilla"
        NomiChiavi(15) = "PH"
        NomiChiavi(16) = "CalcTot"
        NomiChiavi(17) = "CalcAtt"
        NomiChiavi(18) = "CN"
        NomiChiavi(19) = "SO"
        NomiChiavi(20) = "N"
        NomiChiavi(21) = "P"
        NomiChiavi(22) = "K"
        NomiChiavi(23) = "Mg"
        NomiChiavi(24) = "CSC"
        NomiChiavi(25) = "QtaN_FerPrec"
        NomiChiavi(26) = "IdFrequenza"
        NomiChiavi(27) = "TipoFertilizzante"
        NomiChiavi(28) = "Piovosita"
        NomiChiavi(29) = "ZVN"
        NomiChiavi(30) = "Area"
        NomiChiavi(31) = "Veg_Cod"
        NomiChiavi(32) = "RegimeIrriguo"
        NomiChiavi(33) = "Anticipazioni"
        NomiChiavi(34) = "AnticipazioniAnni"
        NomiChiavi(35) = "UbicazioneCod"
        NomiChiavi(36) = "PercNFissazione"
        NomiChiavi(37) = "Copertura"
        NomiChiavi(38) = "Analisi_Cod"
        NomiChiavi(39) = "SeminaSodo"
        NomiChiavi(40) = "Contatore"
        NomiChiavi(41) = "Fattori"
        NomiChiavi(42) = "PiovositaFeb"

        GridViewBilanci.DataSource = Dt
        GridViewBilanci.DataKeyNames = NomiChiavi
        GridViewBilanci.DataBind()

        Session("DT_Bilanci") = Dt


        For i = 0 To GridViewBilanci.Rows.Count - 1
            If IsNumeric(GridViewBilanci.DataKeys(i).Item(3)) Then
                CType(GridViewBilanci.Rows(i).FindControl("txtN_Ammesso"), TextBox).Text = Math.Round(GridViewBilanci.DataKeys(i).Item(3))
            End If
            If IsNumeric(GridViewBilanci.DataKeys(i).Item(4)) Then
                CType(GridViewBilanci.Rows(i).FindControl("txtP_Ammesso"), TextBox).Text = Math.Round(GridViewBilanci.DataKeys(i).Item(4))
            End If
            If IsNumeric(GridViewBilanci.DataKeys(i).Item(5)) Then
                CType(GridViewBilanci.Rows(i).FindControl("txtK_Ammesso"), TextBox).Text = Math.Round(GridViewBilanci.DataKeys(i).Item(5))
            End If
        Next


        If strAlert <> "" Then
            Master.HiddenMessaggioErrore = "ATTENZIONE!!<br>" & strAlert
        End If


    End Sub

    Private Sub PCB_InserimentoMultiplo_Init(sender As Object, e As EventArgs) Handles Me.Init
        Master_Concimaz = CType(Page.Master, MasterConcimazione)
        Master_Concimaz.flag_MostraBtnIndietro = True
    End Sub

    Private Sub Btn_Schede_Click(sender As Object, e As EventArgs) Handles Btn_Schede.Click

        Dim Dt As New DataTable
        Dim Dr As DataRow
        GridViewBilanci.DataSource = Dt
        GridViewBilanci.DataBind()

        Dt = CaricaGriglia_Bilanci()

        Dim Contatore As Integer = 0

        Dim N_Ammesso As Decimal
        Dim P_Ammesso As Decimal
        Dim K_Ammesso As Decimal

        Dim Veg_Cod As Integer = 0
        Dim Anno As Integer = 0
        Dim Resa As Decimal = 0
        Dim Analisi_Cod As Integer = 0
        Dim Sabbia As Decimal = 0
        Dim Limo As Decimal = 0
        Dim Argilla As Decimal = 0
        Dim Ph As Decimal = 0
        Dim CalcTot As Decimal = 0
        Dim CalcAtt As Decimal = 0
        Dim N As Decimal = 0
        Dim SO As Decimal = 0
        Dim CN As Decimal = 0
        Dim P As Decimal = 0
        Dim K As Decimal = 0
        Dim P2O5 As Decimal = 0
        Dim K2O As Decimal = 0
        Dim Mg As Decimal = 0
        Dim CSC As Decimal = 0
        Dim Piovosita As Decimal = 0
        Dim PiovositaFeb As Decimal = 0
        Dim AnticipazioniAnni As Integer = 0
        Dim Anticipazioni As Integer = 0
        Dim Precessione As Integer
        Dim PercNFissazione As Decimal
        Dim DispOssigeno As Integer
        Dim TipoFertilizzante As Integer
        Dim IdFrequenza As Integer
        Dim UbicazioneCod As Integer = 0
        Dim Copertura As Integer = 0
        Dim Zvn As Integer = 0
        Dim GrfiDes_Rer As String = ""
        Dim GrfiCod_Rer As Integer = 0
        Dim RegimeIrriguo As Integer = 0
        Dim FaseDes As String = ""
        Dim FaseCod As Integer = 0
        Dim QtaN_FerPrec As Decimal = 0
        Dim Area As String = ""
        Dim SeminaSodo As Integer = 0

        Dim Fattori As String = ""

        Dim strAlert As String = ""

        For i = 0 To grdSpecie.Rows.Count - 1

            If CType(grdSpecie.Rows(i).FindControl("ChkSeleziona"), CheckBox).Checked = True Then

                N_Ammesso = 0
                P_Ammesso = 0
                K_Ammesso = 0
                Fattori = ""

                CalcolaSchede(i, N_Ammesso, P_Ammesso, K_Ammesso,
                                GrfiCod_Rer, GrfiDes_Rer, FaseCod, FaseDes, Resa, Precessione, DispOssigeno,
                                Sabbia, Limo, Argilla, Ph, CalcTot, CalcAtt, CN, SO, N, P, K, P2O5, K2O, Mg, CSC,
                                QtaN_FerPrec, IdFrequenza, TipoFertilizzante,
                                Piovosita, Zvn, Area, Veg_Cod, RegimeIrriguo, Anticipazioni, AnticipazioniAnni,
                                UbicazioneCod, PercNFissazione, Copertura, Analisi_Cod, SeminaSodo, Fattori, PiovositaFeb, strAlert)

                'aggiungo riga bilancio
                Dr = Dt.NewRow

                Dr.Item("Contatore") = Contatore
                Contatore += 1

                Dr.Item("Piano_Des") = Txt_Descrizione.Text

                Dr.Item("Piva") = grdSpecie.DataKeys(i).Item(0).ToString
                Dr.Item("Sa_Cod") = grdSpecie.DataKeys(i).Item(1).ToString
                Dr.Item("Centro") = grdSpecie.DataKeys(i).Item(6).ToString
                Dr.Item("Impianti") = grdSpecie.DataKeys(i).Item(7).ToString
                Dr.Item("AppezzaIdimp") = grdSpecie.DataKeys(i).Item(4).ToString

                Dr.Item("Veg_Cod") = Veg_Cod
                Dr.Item("Veg_Des") = grdSpecie.DataKeys(i).Item(5).ToString
                Dr.Item("Grfi_Cod") = GrfiCod_Rer
                Dr.Item("Grfi_Des") = GrfiDes_Rer
                Dr.Item("Fase_Cod") = FaseCod
                Dr.Item("Fase_Des") = FaseDes

                Dr.Item("Resa") = Resa

                Dr.Item("Precessione") = Precessione
                Dr.Item("DispOssigeno") = DispOssigeno
                Dr.Item("Sabbia") = Sabbia
                Dr.Item("Limo") = Limo
                Dr.Item("Argilla") = Argilla
                Dr.Item("Ph") = Ph
                Dr.Item("CalcTot") = CalcTot
                Dr.Item("CalcAtt") = CalcAtt
                Dr.Item("CN") = CN
                Dr.Item("SO") = SO
                Dr.Item("N") = N
                Dr.Item("P") = P
                Dr.Item("K") = K
                Dr.Item("Mg") = Mg
                Dr.Item("CSC") = CSC
                Dr.Item("QtaN_FerPrec") = QtaN_FerPrec
                Dr.Item("IdFrequenza") = IdFrequenza
                Dr.Item("TipoFertilizzante") = TipoFertilizzante
                Dr.Item("Piovosita") = Piovosita
                Dr.Item("PiovositaFeb") = PiovositaFeb
                Dr.Item("Zvn") = Zvn
                Dr.Item("Anno") = Anno
                Dr.Item("Area") = Area
                Dr.Item("RegimeIrriguo") = RegimeIrriguo
                Dr.Item("Anticipazioni") = Anticipazioni
                Dr.Item("AnticipazioniAnni") = AnticipazioniAnni
                Dr.Item("UbicazioneCod") = UbicazioneCod
                Dr.Item("PercNFissazione") = PercNFissazione
                Dr.Item("Copertura") = Copertura
                Dr.Item("Analisi_Cod") = Analisi_Cod
                Dr.Item("SeminaSodo") = SeminaSodo

                Dr.Item("N_Ammesso") = N_Ammesso
                Dr.Item("P_Ammesso") = P_Ammesso
                Dr.Item("K_Ammesso") = K_Ammesso

                Dr.Item("Fattori") = Fattori

                Dt.Rows.Add(Dr)

            End If

        Next

        Dim NomiChiavi(42) As String
        NomiChiavi(0) = "Piva"
        NomiChiavi(1) = "Sa_Cod"
        NomiChiavi(2) = "AppezzaIdimp"
        NomiChiavi(3) = "N_Ammesso"
        NomiChiavi(4) = "P_Ammesso"
        NomiChiavi(5) = "K_Ammesso"
        NomiChiavi(6) = "Grfi_Cod"
        NomiChiavi(7) = "Fase_Cod"
        NomiChiavi(8) = "Fase_Des"
        NomiChiavi(9) = "Resa"
        NomiChiavi(10) = "Precessione"
        NomiChiavi(11) = "DispOssigeno"
        NomiChiavi(12) = "Sabbia"
        NomiChiavi(13) = "Limo"
        NomiChiavi(14) = "Argilla"
        NomiChiavi(15) = "PH"
        NomiChiavi(16) = "CalcTot"
        NomiChiavi(17) = "CalcAtt"
        NomiChiavi(18) = "CN"
        NomiChiavi(19) = "SO"
        NomiChiavi(20) = "N"
        NomiChiavi(21) = "P"
        NomiChiavi(22) = "K"
        NomiChiavi(23) = "Mg"
        NomiChiavi(24) = "CSC"
        NomiChiavi(25) = "QtaN_FerPrec"
        NomiChiavi(26) = "IdFrequenza"
        NomiChiavi(27) = "TipoFertilizzante"
        NomiChiavi(28) = "Piovosita"
        NomiChiavi(29) = "ZVN"
        NomiChiavi(30) = "Area"
        NomiChiavi(31) = "Veg_Cod"
        NomiChiavi(32) = "RegimeIrriguo"
        NomiChiavi(33) = "Anticipazioni"
        NomiChiavi(34) = "AnticipazioniAnni"
        NomiChiavi(35) = "UbicazioneCod"
        NomiChiavi(36) = "PercNFissazione"
        NomiChiavi(37) = "Copertura"
        NomiChiavi(38) = "Analisi_Cod"
        NomiChiavi(39) = "SeminaSodo"
        NomiChiavi(40) = "Contatore"
        NomiChiavi(41) = "Fattori"
        NomiChiavi(42) = "PiovositaFeb"

        GridViewBilanci.DataSource = Dt
        GridViewBilanci.DataKeyNames = NomiChiavi
        GridViewBilanci.DataBind()

        Session("DT_Bilanci") = Dt

        For i = 0 To GridViewBilanci.Rows.Count - 1
            If IsNumeric(GridViewBilanci.DataKeys(i).Item(3)) Then
                CType(GridViewBilanci.Rows(i).FindControl("txtN_Ammesso"), TextBox).Text = Math.Round(GridViewBilanci.DataKeys(i).Item(3))
            End If
            If IsNumeric(GridViewBilanci.DataKeys(i).Item(4)) Then
                CType(GridViewBilanci.Rows(i).FindControl("txtP_Ammesso"), TextBox).Text = Math.Round(GridViewBilanci.DataKeys(i).Item(4))
            End If
            If IsNumeric(GridViewBilanci.DataKeys(i).Item(5)) Then
                CType(GridViewBilanci.Rows(i).FindControl("txtK_Ammesso"), TextBox).Text = Math.Round(GridViewBilanci.DataKeys(i).Item(5))
            End If
        Next


        If strAlert <> "" Then
            Master.HiddenMessaggioErrore = "ATTENZIONE!!<br>" & strAlert
        End If

    End Sub

    Private Sub Btn_Salva_Click(sender As Object, e As EventArgs) Handles Btn_Salva.Click

        Dim PC_Testata_Cod As Integer = 0
        Dim PianoSalvato As Boolean

        Dim ValiditaInizio As Date = AGRODATAINIZIO
        Dim ValiditaFine As Date = AGRODATAFINE
        Dim Anno As Integer = 0

        If Txt_ValiditaInizio.Text <> "" Then
            ValiditaInizio = CDate(Txt_ValiditaInizio.Text)
        End If

        If Txt_ValiditaFine.Text <> "" Then
            ValiditaFine = CDate(Txt_ValiditaFine.Text)
        End If

        If IsNumeric(Txt_Anno.Text) Then
            Anno = CInt(Txt_Anno.Text)
        End If

        Try

            For i = 0 To GridViewBilanci.Rows.Count - 1

                If CType(GridViewBilanci.Rows(i).FindControl("ChkSelezionaPiano"), CheckBox).Checked = True Then

                    PC_Testata_Cod = 0

                    PianoSalvato = SalvaPiano(ValiditaInizio, ValiditaFine, Anno,
                               GridViewBilanci.DataKeys(i).Item("Piva"),
                               GridViewBilanci.DataKeys(i).Item("N_Ammesso"),
                               GridViewBilanci.DataKeys(i).Item("P_Ammesso"),
                               GridViewBilanci.DataKeys(i).Item("K_Ammesso"),
                               GridViewBilanci.DataKeys(i).Item("Grfi_Cod"),
                               GridViewBilanci.DataKeys(i).Item("Fase_Cod"),
                               GridViewBilanci.DataKeys(i).Item("Fase_Des"),
                               GridViewBilanci.DataKeys(i).Item("Resa"),
                               GridViewBilanci.DataKeys(i).Item("Precessione"),
                               GridViewBilanci.DataKeys(i).Item("DispOssigeno"),
                               GridViewBilanci.DataKeys(i).Item("Sabbia"),
                               GridViewBilanci.DataKeys(i).Item("Limo"),
                               GridViewBilanci.DataKeys(i).Item("Argilla"),
                               GridViewBilanci.DataKeys(i).Item("PH"),
                               GridViewBilanci.DataKeys(i).Item("CalcTot"),
                               GridViewBilanci.DataKeys(i).Item("CalcAtt"),
                               GridViewBilanci.DataKeys(i).Item("CN"),
                               GridViewBilanci.DataKeys(i).Item("SO"),
                               GridViewBilanci.DataKeys(i).Item("N"),
                               GridViewBilanci.DataKeys(i).Item("P"),
                               GridViewBilanci.DataKeys(i).Item("K"),
                               GridViewBilanci.DataKeys(i).Item("Mg"),
                               GridViewBilanci.DataKeys(i).Item("CSC"),
                               GridViewBilanci.DataKeys(i).Item("QtaN_FerPrec"),
                               GridViewBilanci.DataKeys(i).Item("IdFrequenza"),
                               GridViewBilanci.DataKeys(i).Item("TipoFertilizzante"),
                               GridViewBilanci.DataKeys(i).Item("Piovosita"),
                               GridViewBilanci.DataKeys(i).Item("ZVN"),
                               GridViewBilanci.DataKeys(i).Item("Area"),
                               GridViewBilanci.DataKeys(i).Item("Veg_Cod"),
                               GridViewBilanci.DataKeys(i).Item("RegimeIrriguo"),
                               GridViewBilanci.DataKeys(i).Item("Anticipazioni"),
                               GridViewBilanci.DataKeys(i).Item("AnticipazioniAnni"),
                               GridViewBilanci.DataKeys(i).Item("UbicazioneCod"),
                               GridViewBilanci.DataKeys(i).Item("PercNFissazione"),
                               GridViewBilanci.DataKeys(i).Item("Copertura"),
                               GridViewBilanci.DataKeys(i).Item("Analisi_Cod"),
                               GridViewBilanci.DataKeys(i).Item("SeminaSodo"),
                               GridViewBilanci.DataKeys(i).Item("Fattori"),
                               GridViewBilanci.DataKeys(i).Item("PiovositaFeb"),
                               PC_Testata_Cod, CInt(GridViewBilanci.DataKeys(i).Item("Sa_Cod")))


                    If PianoSalvato = True Then

                        Dim N_Ammesso As Decimal = GridViewBilanci.DataKeys(i).Item("N_Ammesso")
                        Dim P_Ammesso As Decimal = GridViewBilanci.DataKeys(i).Item("P_Ammesso")
                        Dim K_Ammesso As Decimal = GridViewBilanci.DataKeys(i).Item("K_Ammesso")

                        If IsNumeric(CType(GridViewBilanci.Rows(i).FindControl("txtN_Ammesso"), TextBox).Text.Replace(".", ",")) Then
                            N_Ammesso = CDec(CType(GridViewBilanci.Rows(i).FindControl("txtN_Ammesso"), TextBox).Text.Replace(".", ","))
                        End If
                        If IsNumeric(CType(GridViewBilanci.Rows(i).FindControl("txtP_Ammesso"), TextBox).Text.Replace(".", ",")) Then
                            P_Ammesso = CDec(CType(GridViewBilanci.Rows(i).FindControl("txtP_Ammesso"), TextBox).Text.Replace(".", ","))
                        End If
                        If IsNumeric(CType(GridViewBilanci.Rows(i).FindControl("txtK_Ammesso"), TextBox).Text.Replace(".", ",")) Then
                            K_Ammesso = CDec(CType(GridViewBilanci.Rows(i).FindControl("txtK_Ammesso"), TextBox).Text.Replace(".", ","))
                        End If

                        Applica(PC_Testata_Cod, ValiditaInizio, ValiditaFine,
                             GridViewBilanci.DataKeys(i).Item("Piva"),
                             GridViewBilanci.DataKeys(i).Item("Sa_Cod"),
                             GridViewBilanci.DataKeys(i).Item("AppezzaIdimp"),
                             N_Ammesso,
                             P_Ammesso,
                             K_Ammesso)

                        Try
                            Select Case Qs_Tipo
                                Case enum_PianoConcimazione_Tipo.Bilancio
                                    CreaReportBilancio(i, PC_Testata_Cod, Anno)
                                Case enum_PianoConcimazione_Tipo.Schede
                                    CreaReportSchede(i, PC_Testata_Cod, Anno, ddlRegolamento.SelectedValue)
                                Case Else
                            End Select
                        Catch ex As Exception
                            Log_Errori += "- SalvaPdf (riga i=" & i & ") : " + vbCrLf + ex.Message + vbCrLf
                        End Try

                    End If

                End If

            Next


        Catch ex As Exception

            Try
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            Catch

            End Try

            Master.HiddenMessaggioErrore = "Salvataggio non riuscito :" & ex.Message
            Exit Sub

        Finally

            If Not objParametri_Server.objConnessione Is Nothing Then AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        End Try


        '-----------------------------------------------------------------
        '---- Salvataggio Log Errori 
        '---- caso in cui non sia stato creato e salvato il report
        '-----------------------------------------------------------------
        If Log_Errori <> "" Then

            Dim Nome_File As String = "Log_Errori_" + Nome_Documento

            Dim objLog As New AgronicaCoreDataProvider.LogProvider
            objLog.Gestione_LogErrori(objParametri_Server,
                                      "PianoConcimazione",
                                       Nome_File & ".txt",
                                       Session("ASG_Utente_Username"),
                                        "PianoConcimazioneMassivo",
                                         Log_Errori)

        End If


        Master.HiddenMessaggioOK = "Salvataggio riuscito!"
        Btn_Salva.Visible = False


    End Sub

    Private Function SalvaPiano(ByVal ValiditaInizio As Date,
                                ByVal ValiditaFine As Date,
                                ByVal Anno As Integer,
                                ByVal Piva As String,
                            ByVal N_Ammesso As Decimal,
                            ByVal P_Ammesso As Decimal,
                            ByVal K_Ammesso As Decimal,
                            ByVal GrfiCod_Rer As Integer,
                            ByVal FaseCod As Integer,
                            ByVal FaseDes As String,
                            ByVal Resa As Decimal,
                            ByVal Precessione As Integer,
                            ByVal DispOssigeno As Integer,
                            ByVal Sabbia As Decimal,
                            ByVal Limo As Decimal,
                            ByVal Argilla As Decimal,
                            ByVal PH As Decimal,
                            ByVal CalcTot As Decimal,
                            ByVal CalcAtt As Decimal,
                            ByVal CN As Decimal,
                            ByVal SO As Decimal,
                            ByVal N As Decimal,
                            ByVal P2O5 As Decimal,
                            ByVal K2O As Decimal,
                            ByVal Mg As Decimal,
                            ByVal CSC As Decimal,
                            ByVal QtaN_FerPrec As Decimal,
                            ByVal IdFrequenza As Integer,
                            ByVal TipoFertilizzante As Integer,
                            ByVal Piovosita As Decimal,
                            ByVal ZVN As Integer,
                            ByVal Area As String,
                            ByVal Veg_Cod As Integer,
                            ByVal RegimeIrriguo As Integer,
                            ByVal Anticipazioni As Integer,
                            ByVal AnticipazioniAnni As Integer,
                            ByVal UbicazioneCod As Integer,
                            ByVal PercNFissazione As Decimal,
                            ByVal Copertura As Integer,
                            ByVal Analisi_Cod As Integer,
                            ByVal SeminaSodo As Integer,
                                ByVal Fattori As String,
                                ByVal PiovositaFeb As Decimal,
                                ByRef PC_Testata_Cod As Integer,
                                ByVal sa_cod As Integer
                        ) As Boolean


        'Azoto
        'fosforo
        'potassio

        Dim strErrore As String = Nothing
        Dim objLog As AgronicaCoreDataProvider.LogProvider
        Dim objSequenze As AgronicaCoreDataProvider.Agro_Sequenze

        Dim objPC_Testata As AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Testata_W
        Dim objPC_Dettagli As AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_W
        Dim objPC_FattoriCorrettivi As AgronicaCorePianoConcimazioneDAL.PianoConcimazione_FattoriCorrettivi_W
        Dim objElaborazioni As AgronicaCoreAnagrafeDAL.PianoConcimazione_Elaborazioni_W

        Dim BaseCode As Integer
        Dim TopCode As Integer


        Dim PC_Dettagli_Cod As Integer

        Dim ElaborazioneCod As Integer


        Try

            Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))

            ' istanzio le classi
            'objConnessione = New AgronicaCoreDataProvider.ConnessioniTransazioni
            objLog = New AgronicaCoreDataProvider.LogProvider

            ' apro la connessione e la transazione (primo parametri dell'interfaccia)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            Dim bRet As Boolean

            objPC_Testata = New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Testata_W
            objPC_Dettagli = New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_W
            objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

            ' TESTATA PIANO

            PC_Testata_Cod = objSequenze.NuovoId_Tabella("PianoConcimazione_Testata",
                                                             BaseCode,
                                                             TopCode,
                                                             objParametri_Server)

            ElaborazioneCod = objSequenze.NuovoId_Tabella("PianoConcimazione_Elaborazioni",
                                                          BaseCode,
                                                          TopCode,
                                                          objParametri_Server)

            objElaborazioni = New AgronicaCoreAnagrafeDAL.PianoConcimazione_Elaborazioni_W

            objElaborazioni.Scrivi(ElaborazioneCod, Txt_Descrizione.Text, Now, AGRODATAFINE, objParametri_Server)


            bRet = objPC_Testata.Scrivi(PC_Testata_Cod,
                                            Txt_Descrizione.Text,
                                            CInt(ddlRegolamento.SelectedValue),
                                            Qs_Tipo,
                                            ElaborazioneCod,
                                            ValiditaInizio,
                                            ValiditaFine,
                                            "", 0,
                                            objParametri_Server)

            If Not bRet Then
                objLog.Scrivi_LOG(objParametri_Server, "PCB_Inserimento.Salva", "Errore salvataggio testata")
            End If

            ' DETTAGLIO PIANO

            PC_Dettagli_Cod = objSequenze.NuovoId_Tabella("PianoConcimazione_Dettagli",
                                                              BaseCode,
                                                              TopCode,
                                                              objParametri_Server)


            Dim N_MAS As Decimal = -1
            Dim N_Mas_Regolamento_Tipo As Integer = 0

            bRet = objPC_Dettagli.Scrivi(PC_Testata_Cod,
                                          PC_Dettagli_Cod,
                                          Piva,
                                          Txt_Descrizione.Text,
                                          GrfiCod_Rer,
                                          FaseDes,
                                          FaseCod,
                                          0, 0,
                                          Resa,
                                          Precessione,
                                          0, "",
                                          DispOssigeno,
                                          Sabbia, Limo, Argilla, PH, CalcTot, CalcAtt, CN, SO, N, P2O5, K2O,
                                          QtaN_FerPrec,
                                          IdFrequenza,
                                          0, 0, 0, 0, 0, 0, 0, 0, 0,
                                          TipoFertilizzante,
                                          0,
                                          Piovosita,
                                          ZVN,
                                          CInt(Anno),
                                          Area,
                                          "", "",
                                          ValiditaInizio,
                                          ValiditaFine,
                                          "", 0, 0,
                                          Veg_Cod,
                                          RegimeIrriguo,
                                          Anticipazioni,
                                          AnticipazioniAnni,
                                          UbicazioneCod,
                                          PercNFissazione,
                                          Copertura,
                                          ValiditaInizio,
                                          ValiditaFine,
                                          N_Ammesso, P_Ammesso, K_Ammesso,
                                          Mg, CSC, Analisi_Cod,
                                          PiovositaFeb, sa_cod,
                                          0, 0, 0, 0, 0,
                                          Nothing, Nothing,
                                          0, 0, 0, 0, 0, 0,
                                          objParametri_Server)

            If Not bRet Then

                objLog.Scrivi_LOG(objParametri_Server, "PCB_Inserimento.Salva", "Errore salvataggio testata")

            Else

                ' FATTORI CORRETTIVI

                If Qs_Tipo = enum_PianoConcimazione_Tipo.Schede Then

                    Select Case FaseCod

                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto,
                             enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento,
                             enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento

                        Case Else

                            objPC_FattoriCorrettivi = New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_FattoriCorrettivi_W

                            Dim vetFattori() As String
                            If Fattori <> "" Then
                                vetFattori = Fattori.Split(",")
                            End If

                            For i = 0 To vetFattori.Length - 1
                                If IsNumeric(vetFattori(i)) Then
                                    bRet = objPC_FattoriCorrettivi.Scrivi(CInt(ddlRegolamento.SelectedValue),
                                                                        PC_Testata_Cod,
                                                                        vetFattori(i),
                                                                        AGRODATAINIZIO,
                                                                        AGRODATAFINE,
                                                                        objParametri_Server)
                                End If
                            Next

                    End Select

                End If

            End If

            objLog = Nothing

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)


        Catch ex As Exception

            Try
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            Catch

            End Try

            Return False

        Finally

            If Not objParametri_Server.objConnessione Is Nothing Then AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        End Try

        Return True

    End Function

    Private Sub CreaReportBilancio(ByVal IndiceGriglia As Integer, ByVal PC_Testata_Cod As Integer, ByVal AnnoSalvato As Integer)

        Dim objPagReport As New Bilancio_NPK

        'Dim Chiave As String = ""

        Dim Campo, Appezza, IdImp As Integer

        Dim StrImpianto As String = GridViewBilanci.DataKeys(IndiceGriglia).Item("AppezzaIdimp")

        '                        GridViewBilanci.DataKeys(IndiceGriglia).Item("Piva"),
        '                        GridViewBilanci.DataKeys(IndiceGriglia).Item("Sa_Cod"),
        '                        GridViewBilanci.DataKeys(IndiceGriglia).Item("AppezzaIdimp"),
        '                        GridViewBilanci.DataKeys(IndiceGriglia).Item("N_Ammesso"),
        '                        GridViewBilanci.DataKeys(IndiceGriglia).Item("P_Ammesso"),
        'GridViewBilanci.DataKeys(IndiceGriglia).Item("K_Ammesso")

        Dim strPrimoImpianto As String = StrImpianto.Split("$")(0)

        If strPrimoImpianto <> String.Empty Then
            Campo = strPrimoImpianto.Split("|")(0)
            Appezza = strPrimoImpianto.Split("|")(1)
            IdImp = strPrimoImpianto.Split("|")(2)
        End If

        'ChiaveAlbero_Codifica(Chiave, TipiEnumerativi.enum_TipoNodo.DistintaDiProduzione,
        '                      grdSpecie.DataKeys(IndiceGriglia).Item(0).ToString, _
        '                      grdSpecie.DataKeys(IndiceGriglia).Item(1).ToString, _
        '                      Campo, Appezza, IdImp, , , , , , , , , , , , , , , , , ViewState("ProgettoCodSalvato"))

        Dim rptBilancioNPK As New RPT_Bilancio_NPK

        Dim Regolamento_Cod As Integer = 0
        Dim Sa_Nome As String = " "
        Dim Sup_Tot As Decimal = 0
        Dim Mas As Decimal = -1
        Dim MasSalvato As Boolean = False

        objPagReport.Dati_Piano(GridViewBilanci.DataKeys(IndiceGriglia).Item("Piva"),
                                  GridViewBilanci.DataKeys(IndiceGriglia).Item("Sa_Cod"),
                                  Appezza,
                                  IdImp,
                                  PC_Testata_Cod, Sa_Nome,
                                  Sup_Tot, Regolamento_Cod,
                                  Mas, MasSalvato,
                                  objParametri_Server,
                                  rptBilancioNPK)

        Dim DsNecessita As New DS_Fattori
        Dim DsDisponibilita As New DS_Fattori

        objPagReport.Carica_Dati(DsDisponibilita, DsNecessita, PC_Testata_Cod, GridViewBilanci.DataKeys(IndiceGriglia).Item("Piva"),
                                  Sup_Tot, Mas, MasSalvato, objParametri_Server, rptBilancioNPK)

        rptBilancioNPK.OpenSubreport("RPT_Necessita.rpt").SetDataSource(DsNecessita)
        rptBilancioNPK.OpenSubreport("RPT_Disponibilita.rpt").SetDataSource(DsDisponibilita)

        Dim AmmessoN, AmmessoP, AmmessoK As Double ',LimiteMas
        AmmessoN = IIf(IsNumeric(GridViewBilanci.DataKeys(IndiceGriglia).Item("N_Ammesso")), CDbl(GridViewBilanci.DataKeys(IndiceGriglia).Item("N_Ammesso")), 0)
        AmmessoP = IIf(IsNumeric(GridViewBilanci.DataKeys(IndiceGriglia).Item("P_Ammesso")), CDbl(GridViewBilanci.DataKeys(IndiceGriglia).Item("P_Ammesso")), 0)
        AmmessoK = IIf(IsNumeric(GridViewBilanci.DataKeys(IndiceGriglia).Item("K_Ammesso")), CDbl(GridViewBilanci.DataKeys(IndiceGriglia).Item("K_Ammesso")), 0)

        CType(rptBilancioNPK.Section3.ReportObjects("TextAttenzione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""

        CType(rptBilancioNPK.Section3.ReportObjects("TextQtaN"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(AmmessoN, "0.00")
        CType(rptBilancioNPK.Section3.ReportObjects("TextQtaP"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(AmmessoP, "0.00")
        CType(rptBilancioNPK.Section3.ReportObjects("TextQtaK"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(AmmessoK, "0.00")

        Dim strCentro As String = Sa_Nome.Replace("<BR>", "")
        Dim objSpec As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        Dim dtSpec As DataTable = objSpec.Leggi(CInt(GridViewBilanci.DataKeys(IndiceGriglia).Item("Veg_Cod")), 0, "", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        Dim strSpecie As String = " "
        If Not IsNothing(dtSpec) AndAlso dtSpec.Rows.Count > 0 Then
            strSpecie = dtSpec.Rows(0).Item("veg_des").ToString
        End If


        strCentro = AgronicaCoreUtility.Stringhe.EliminaCaratteriSpecialiFile(strCentro)
        strSpecie = AgronicaCoreUtility.Stringhe.EliminaCaratteriSpecialiFile(strSpecie)


        Dim CodSocio As String
        Dim CentroConferimento As String

        Dim objAnagrafe As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        CodSocio = objAnagrafe.Leggi_Codice_from_Imprese_Codici(GridViewBilanci.DataKeys(IndiceGriglia).Item("Piva").ToString, enum_CodiciAnagrafe.Codice_Socio, objParametri_Server)

        CentroConferimento = objAnagrafe.Leggi_Codice_from_Imprese_Codici(GridViewBilanci.DataKeys(IndiceGriglia).Item("Piva").ToString, enum_CodiciAnagrafe.V01CON00_VCCOIS, objParametri_Server)
        objAnagrafe = Nothing

        'Dim NomeFile As String = "PC" & ViewState("TestataCodSalvata") & "_" & strCentro.Substring(0, 15) & "_" & strSpecie.Substring(0, 15) & ".pdf"

        Dim NomeFile As String = "PCB" &
                                 IIf(CentroConferimento <> "", "_" & CentroConferimento, "") &
                                 "_" & strCentro.Substring(0, Math.Min(15, strCentro.Length - 1)) &
                                 IIf(CodSocio <> "", "_" & CodSocio, "") &
                                 "_" & strSpecie.Substring(0, Math.Min(15, strSpecie.Length - 1)) &
                                 "_" & PC_Testata_Cod & ".pdf"

        Dim strFile As String = objPagReport.SalvaPdf(objParametri_Server, rptBilancioNPK, NomeFile)

        'salvo il record dell'allegato se ha salvato il pdf
        If strFile <> "" Then

            Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
            Dim AllegatiDocumentiCod As Integer

            Dim anno As Integer
            If Not IsNothing(AnnoSalvato) Then
                anno = AnnoSalvato
            Else
                anno = Now.Year
            End If

            AllegatiDocumentiCod = objAllegati.SalvaAllegato(GridViewBilanci.DataKeys(IndiceGriglia).Item("Piva").ToString,
                                                             enum_CategorieDocumenti.PianoConcimazione,
                                                             "Piano Concimazione",
                                                             NomeFile,
                                                             Session("Sottocartella"),
                                                             PC_Testata_Cod, GridViewBilanci.DataKeys(IndiceGriglia).Item("Piva").ToString, "", "",
                                                             CDate("01/01/" & anno.ToString),
                                                             CDate("31/12/" & anno.ToString),
                                                             objParametri_Server)

            Dim objPC_dettagli_W As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_W
            'INSERISCO IL CODICE ALLEGATO NELLA TABELLA Piano_Concimazione_Dettagli del PC salvato
            If Not IsNothing(AllegatiDocumentiCod) AndAlso AllegatiDocumentiCod <> 0 Then
                If Not objPC_dettagli_W.UpdateCodAllegato(PC_Testata_Cod, 0, GridViewBilanci.DataKeys(IndiceGriglia).Item("Piva").ToString, AllegatiDocumentiCod, objParametri_Server) Then
                    Throw New Exception("Non sono riuscito ad associare l'allegato al corrente Piano di Concimanzione. PC_cod =" & PC_Testata_Cod.ToString)
                End If
            End If

            objAllegati = Nothing

        End If

        rptBilancioNPK.Close()
        rptBilancioNPK.Dispose()

        'rptBilancioNPK = Nothing

    End Sub


    Private Function Applica(ByVal PC_Testata_Cod As Integer, ByVal Data_Inizio As Date, ByVal Data_Fine As Date,
                             ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal strAppezzaIdimp As String,
                             ByVal QtaMaxN As Decimal, ByVal QtaMaxP As Decimal, ByVal QtaMaxK As Decimal)


        Dim objPC_EntitaxTestata As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_EntitaxTestata_W
        Dim objApporti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W


        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            Dim vetImp() As String
            vetImp = strAppezzaIdimp.Split("$")

            Dim strCampo As String
            Dim strAppezza As String
            Dim strIdReg As String
            Dim strProgettoCod As String

            Dim i As Integer

            For i = 0 To vetImp.Length - 1

                strCampo = vetImp(i).Split("|")(0)
                strAppezza = vetImp(i).Split("|")(1)
                strIdReg = vetImp(i).Split("|")(2)
                strProgettoCod = vetImp(i).Split("|")(3)

                objPC_EntitaxTestata.Scrivi(PC_Testata_Cod,
                                                   AgronicaCoreDataProvider.TipiEnumerativi.enum_EntitaAlberoImprese.Distinta,
                                                   Piva,
                                                   Sa_Cod,
                                                   strAppezza,
                                                   strIdReg,
                                                   0,
                                                   strCampo,
                                                   "", "", "", 0, 0, "",
                                                   strProgettoCod,
                                                   "",
                                                   "",
                                                   QtaMaxN,
                                                   QtaMaxP,
                                                   QtaMaxK,
                                                   Data_Inizio,
                                                   Data_Fine,
                                                   objParametri_Server)

                'record apporti esercizio

                objApporti.ModificaxProgetto2(Piva,
                                                   Sa_Cod,
                                                   strAppezza,
                                                   strIdReg,
                                                    strProgettoCod,
                                                    enum_CodiciAnagrafe.Impianto_LimiteN,
                                                    QtaMaxN,
                                                    AGRODATAINIZIO, AGRODATAFINE,
                                                    "",
                                                    objParametri_Server)

                objApporti.ModificaxProgetto2(Piva,
                                                   Sa_Cod,
                                                   strAppezza,
                                                   strIdReg,
                                                    strProgettoCod,
                                                    enum_CodiciAnagrafe.Impianto_LimiteP,
                                                    QtaMaxP,
                                                    AGRODATAINIZIO, AGRODATAFINE,
                                                    "",
                                                    objParametri_Server)

                objApporti.ModificaxProgetto2(Piva,
                                                   Sa_Cod,
                                                   strAppezza,
                                                   strIdReg,
                                                    strProgettoCod,
                                                    enum_CodiciAnagrafe.Impianto_LimiteK,
                                                     QtaMaxK,
                                                    AGRODATAINIZIO, AGRODATAFINE,
                                                    "",
                                                    objParametri_Server)


            Next

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        Catch ex As Exception

            Try
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            Catch

            End Try

        Finally

            If Not objParametri_Server.objConnessione Is Nothing Then AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        End Try


    End Function

    Private Sub GridViewBilanci_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewBilanci.RowCommand

        Dim Selezione_IndiceRiga As Integer
        Dim Selezione_Comando As String

        Selezione_IndiceRiga = Int(e.CommandArgument)
        Selezione_Comando = e.CommandName

        Dim Contatore As Integer = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Contatore")
        IndiceBilancio.Value = Contatore

        Dim N_Ammesso As Decimal = 0
        Dim P_Ammesso As Decimal = 0
        Dim K_Ammesso As Decimal = 0

        Select Case Selezione_Comando

            Case "Dettaglio"

                Txt_Sabbia.Text = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Sabbia")
                Txt_Limo.Text = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Limo")
                Txt_Argilla.Text = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Argilla")
                Txt_PH.Text = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("PH")

                Txt_CalcTot.Text = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("CalcTot")
                Txt_CalcAtt.Text = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("CalcAtt")
                Txt_SO.Text = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("SO")
                Txt_CN.Text = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("CN")

                Txt_N.Text = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("N")
                Txt_P.Text = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("P")
                Txt_K.Text = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("K")
                Txt_Mg.Text = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Mg")

                Txt_CSC.Text = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("CSC")

                Hidden_Veg_Cod.Value = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Veg_Cod")
                Hidden_Grfi_Cod.Value = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Grfi_Cod")
                Hidden_Fase_Cod.Value = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Fase_Cod")
                Hidden_Resa.Value = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Resa")
                Hidden_AnticipazioniAnni.Value = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("AnticipazioniAnni")
                Hidden_Copertura.Value = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Copertura")
                Hidden_UbicazioneCod.Value = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("UbicazioneCod")
                Hidden_PercNFissazione.Value = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("PercNFissazione")
                Hidden_DispOssigeno.Value = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("DispOssigeno")
                Hidden_Piovosita.Value = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Piovosita")
                Hidden_PiovositaFeb.Value = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("PiovositaFeb")
                Hidden_Precessione.Value = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Precessione")
                Hidden_TipoFertilizzante.Value = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("TipoFertilizzante")
                Hidden_IdFrequenza.Value = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("IdFrequenza")
                Hidden_QtaN_FerPrec.Value = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("QtaN_FerPrec")
                Hidden_Fattori.Value = GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Fattori")


                Select Case Qs_Tipo
                    Case enum_PianoConcimazione_Tipo.Bilancio
                        Div_Bilancio.Visible = True
                        Div_Schede.Visible = False
                        CalcolaBilancio(GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Veg_Cod"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Grfi_Cod"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Fase_Cod"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Resa"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("AnticipazioniAnni"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Copertura"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("UbicazioneCod"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("PercNFissazione"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("DispOssigeno"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Piovosita"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Precessione"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("TipoFertilizzante"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("IdFrequenza"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("QtaN_FerPrec"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("PiovositaFeb"),
                                        N_Ammesso, P_Ammesso, K_Ammesso)
                    Case enum_PianoConcimazione_Tipo.Schede
                        Div_Bilancio.Visible = False
                        Div_Schede.Visible = True
                        CalcolaSchede(GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Veg_Cod"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Grfi_Cod"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Fase_Cod"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Resa"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Piovosita"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("PiovositaFeb"),
                                        GridViewBilanci.DataKeys(Selezione_IndiceRiga).Item("Fattori"),
                                        N_Ammesso, P_Ammesso, K_Ammesso)
                End Select


                Dim script As New StringBuilder
                script.AppendLine("<script type='text/javascript' >$(document).ready(function () { ")
                script.AppendLine("             $('#dialogDettaglioBilancio').modal();")
                script.AppendLine("     });</script>")

                Page.RegisterClientScriptBlock("", script.ToString)


        End Select

    End Sub

    Private Sub BtnAggiornaBilancio_Click(sender As Object, e As EventArgs) Handles BtnAggiornaBilancio.Click


        Dim Contatore As Integer = IndiceBilancio.Value

        Dim DT_Bilanci As DataTable = Session("DT_Bilanci")
        Dim Dr As DataRow
        Dr = DT_Bilanci.Rows.Find(Contatore)

        Dr.Item("Sabbia") = Txt_Sabbia.Text
        Dr.Item("Limo") = Txt_Limo.Text
        Dr.Item("Argilla") = Txt_Argilla.Text
        Dr.Item("PH") = Txt_PH.Text
        Dr.Item("CalcTot") = Txt_CalcTot.Text
        Dr.Item("CalcAtt") = Txt_CalcAtt.Text
        Dr.Item("SO") = Txt_SO.Text
        Dr.Item("CN") = Txt_CN.Text
        Dr.Item("N") = Txt_N.Text
        Dr.Item("P") = Txt_P.Text
        Dr.Item("K") = Txt_K.Text
        Dr.Item("Mg") = Txt_Mg.Text
        Dr.Item("CSC") = Txt_CSC.Text

        Hidden_Fattori.Value = ""

        Dim vetFattori() As Integer

        ' azoto
        LeggiFattoriSelezionati(grdIncrementi, "Chk_SelIncrementi_N", vetFattori)
        LeggiFattoriSelezionati(grdDecrementi, "Chk_SelDecrementi_N", vetFattori)

        ' fosforo
        LeggiFattoriSelezionati(grdIncrementiP, "Chk_SelIncrementi_P", vetFattori)
        LeggiFattoriSelezionati(grdDecrementiP, "Chk_SelDecrementi_P", vetFattori)

        ' potassio
        LeggiFattoriSelezionati(grdIncrementiK, "Chk_SelIncrementi_K", vetFattori)
        LeggiFattoriSelezionati(grdDecrementiK, "Chk_SelDecrementi_K", vetFattori)

        ' salvo le dosi standard nella tabella dei FattoriCorrettivi
        Dim lun As Integer = -1
        If Not IsNothing(vetFattori) Then
            lun = vetFattori.Length - 1
        End If

        If Not IsNothing(ddlDoseP.SelectedValue) AndAlso IsNumeric(ddlDoseP.SelectedValue.Split("|")(0)) Then
            ReDim Preserve vetFattori(lun + 1)
            vetFattori(lun + 1) = ddlDoseP.SelectedValue.Split("|")(0)
            lun = lun + 1
        End If

        'If IsNumeric(ddlDoseK.SelectedValue) Then
        If Not IsNothing(ddlDoseK.SelectedValue) AndAlso IsNumeric(ddlDoseK.SelectedValue.Split("|")(0)) Then
            ReDim Preserve vetFattori(lun + 1)
            vetFattori(lun + 1) = ddlDoseK.SelectedValue.Split("|")(0)
        End If

        If Not vetFattori Is Nothing Then
            For i = 0 To vetFattori.Length - 1
                Hidden_Fattori.Value &= vetFattori(i) & ","
            Next
        End If

        If Hidden_Fattori.Value <> "" Then
            Dr.Item("Fattori") = Left(Hidden_Fattori.Value, Hidden_Fattori.Value.Length - 1)
        End If

        Dr.Item("N_Ammesso") = Txt_DoseRicalcolata.Text
        Dr.Item("P_Ammesso") = Txt_DoseRicalcolataP.Text
        Dr.Item("K_Ammesso") = Txt_DoseRicalcolataK.Text

        Dim NomiChiavi(42) As String
        NomiChiavi(0) = "Piva"
        NomiChiavi(1) = "Sa_Cod"
        NomiChiavi(2) = "AppezzaIdimp"
        NomiChiavi(3) = "N_Ammesso"
        NomiChiavi(4) = "P_Ammesso"
        NomiChiavi(5) = "K_Ammesso"
        NomiChiavi(6) = "Grfi_Cod"
        NomiChiavi(7) = "Fase_Cod"
        NomiChiavi(8) = "Fase_Des"
        NomiChiavi(9) = "Resa"
        NomiChiavi(10) = "Precessione"
        NomiChiavi(11) = "DispOssigeno"
        NomiChiavi(12) = "Sabbia"
        NomiChiavi(13) = "Limo"
        NomiChiavi(14) = "Argilla"
        NomiChiavi(15) = "PH"
        NomiChiavi(16) = "CalcTot"
        NomiChiavi(17) = "CalcAtt"
        NomiChiavi(18) = "CN"
        NomiChiavi(19) = "SO"
        NomiChiavi(20) = "N"
        NomiChiavi(21) = "P"
        NomiChiavi(22) = "K"
        NomiChiavi(23) = "Mg"
        NomiChiavi(24) = "CSC"
        NomiChiavi(25) = "QtaN_FerPrec"
        NomiChiavi(26) = "IdFrequenza"
        NomiChiavi(27) = "TipoFertilizzante"
        NomiChiavi(28) = "Piovosita"
        NomiChiavi(29) = "ZVN"
        NomiChiavi(30) = "Area"
        NomiChiavi(31) = "Veg_Cod"
        NomiChiavi(32) = "RegimeIrriguo"
        NomiChiavi(33) = "Anticipazioni"
        NomiChiavi(34) = "AnticipazioniAnni"
        NomiChiavi(35) = "UbicazioneCod"
        NomiChiavi(36) = "PercNFissazione"
        NomiChiavi(37) = "Copertura"
        NomiChiavi(38) = "Analisi_Cod"
        NomiChiavi(39) = "SeminaSodo"
        NomiChiavi(40) = "Contatore"
        NomiChiavi(41) = "Fattori"
        NomiChiavi(42) = "PiovositaFeb"

        GridViewBilanci.DataSource = DT_Bilanci
        GridViewBilanci.DataKeyNames = NomiChiavi
        GridViewBilanci.DataBind()

        Session("DT_Bilanci") = DT_Bilanci

    End Sub
    Private Sub LeggiFattoriSelezionati(ByVal gv As GridView, ByVal IdCheck As String, ByRef vetFattori() As Integer)

        Dim i As Integer
        For i = 0 To gv.Rows.Count - 1

            Dim lun As Integer = -1

            If CType(gv.Rows(i).FindControl(IdCheck), CheckBox).Checked = True Then

                If Not IsNothing(vetFattori) Then
                    lun = vetFattori.Length - 1
                End If

                ReDim Preserve vetFattori(lun + 1)
                vetFattori(lun + 1) = gv.DataKeys(i).Item("Fattore_Cod")

            End If

        Next

    End Sub
    Private Sub CalcolaBilancio(ByVal Veg_Cod As Integer, ByVal Grfi_Cod As Integer, ByVal FaseCod As Integer, ByVal Resa As Decimal,
                                ByVal AnticipazioniAnni As Integer, ByVal Copertura As Integer, ByVal Ubicazione As Integer, ByVal PercNFiss As Decimal,
                                ByVal DispOss As Integer, ByVal Piovosita As Decimal,
                                ByVal Precessione As Integer, ByVal Fertilizzante As Integer, ByVal Frequenza As Integer, ByVal QtaN As Decimal, ByVal PiovositaFeb As Decimal,
                                ByRef N_Ammesso As Decimal, ByRef P_Ammesso As Decimal, ByRef K_Ammesso As Decimal)

        Dim Regolamento_Cod As Integer = CInt(ddlRegolamento.SelectedValue)

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_input

        objParametriIngresso.Regolamento_Cod = Regolamento_Cod
        objParametriIngresso.Veg_Cod = Veg_Cod
        objParametriIngresso.Grfi_Cod = Grfi_Cod
        objParametriIngresso.FaseCicloColturale_Cod = FaseCod
        objParametriIngresso.Resa = Resa

        objParametriIngresso.AnticipazioniAnni = AnticipazioniAnni

        If Copertura = 0 Then
            objParametriIngresso.ColturaProtetta = False
        Else
            objParametriIngresso.ColturaProtetta = True
        End If

        objParametriIngresso.Ubicazione_Cod = Ubicazione
        objParametriIngresso.FissazioneN_Perc = PercNFiss

        If IsNumeric(Txt_Sabbia.Text) Then
            objParametriIngresso.Sabbia = CDec(Txt_Sabbia.Text)
        Else
            objParametriIngresso.Sabbia = 0
        End If
        If IsNumeric(Txt_Argilla.Text) Then
            objParametriIngresso.Argilla = CDec(Txt_Argilla.Text)
        Else
            objParametriIngresso.Argilla = 0
        End If
        If IsNumeric(Txt_N.Text) Then
            objParametriIngresso.NTot = CDec(Txt_N.Text)
        Else
            objParametriIngresso.NTot = 0
        End If
        If IsNumeric(Txt_SO.Text) Then
            objParametriIngresso.SO = CDec(Txt_SO.Text)
        Else
            objParametriIngresso.SO = 0
        End If
        If IsNumeric(Txt_CN.Text) Then
            objParametriIngresso.CN = CDec(Txt_CN.Text)
        Else
            objParametriIngresso.CN = 0
        End If
        If IsNumeric(Txt_P.Text) Then
            objParametriIngresso.P2O5 = CDec(Txt_P.Text)
        Else
            objParametriIngresso.P2O5 = 0
        End If
        If IsNumeric(Txt_K.Text) Then
            objParametriIngresso.K2O = CDec(Txt_K.Text)
        Else
            objParametriIngresso.K2O = 0
        End If
        If IsNumeric(Txt_Mg.Text) Then
            objParametriIngresso.Mg = CDec(Txt_Mg.Text)
        Else
            objParametriIngresso.Mg = 0
        End If
        If IsNumeric(Txt_CSC.Text) Then
            objParametriIngresso.CSC = CDec(Txt_CSC.Text)
        Else
            objParametriIngresso.CSC = 0
        End If

        If IsNumeric(Txt_CalcTot.Text) Then
            objParametriIngresso.Caco3 = CDec(Txt_CalcTot.Text)
        Else
            objParametriIngresso.Caco3 = 0
        End If

        objParametriIngresso.DisponibilitaOssigeno_Cod = DispOss

        objParametriIngresso.PioggiaMM = Piovosita
        objParametriIngresso.PioggiaMM_Febbraio = PiovositaFeb

        objParametriIngresso.Precessione_Veg_Cod = Precessione
        objParametriIngresso.FertilizzanteOrganico_ColturePrecedenti_Tipo = Fertilizzante
        objParametriIngresso.FertilizzanteOrganico_ColturePrecedenti_Frequenza = Frequenza

        objParametriIngresso.FertilizzanteOrganico_ColturePrecedenti_Qta = QtaN

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_output
        Dim objBilancio As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objBilancio.CalcolaBilancio(objParametriIngresso)

        'necessita
        If objParametriUscita.ListaNecessita.Count > 0 Then

            Dim DT_Necessita As New DataTable
            Dim Dr_Necessita As DataRow
            DT_Necessita.Columns.Add(New DataColumn("Descrizione", GetType(String)))
            DT_Necessita.Columns.Add(New DataColumn("N", GetType(String)))
            DT_Necessita.Columns.Add(New DataColumn("P", GetType(String)))
            DT_Necessita.Columns.Add(New DataColumn("K", GetType(String)))

            For i = 0 To objParametriUscita.ListaNecessita.Count - 1
                Dr_Necessita = DT_Necessita.NewRow
                Dr_Necessita.Item("Descrizione") = objParametriUscita.ListaNecessita(i).Descrizione
                Dr_Necessita.Item("N") = Format(objParametriUscita.ListaNecessita(i).Valore_N, "0.00")
                Dr_Necessita.Item("P") = Format(objParametriUscita.ListaNecessita(i).Valore_P, "0.00")
                Dr_Necessita.Item("K") = Format(objParametriUscita.ListaNecessita(i).Valore_K, "0.00")
                DT_Necessita.Rows.Add(Dr_Necessita)
            Next

            Dr_Necessita = DT_Necessita.NewRow
            Dr_Necessita.Item("Descrizione") = "Totale Necessità"
            Dr_Necessita.Item("N") = Format(objParametriUscita.N_Necessario, "0.00")
            Dr_Necessita.Item("P") = Format(objParametriUscita.P_Necessario, "0.00")
            Dr_Necessita.Item("K") = Format(objParametriUscita.K_Necessario, "0.00")
            DT_Necessita.Rows.Add(Dr_Necessita)

            grdNecessita.DataSource = DT_Necessita
            grdNecessita.DataBind()


        End If



        'disponibilita
        If objParametriUscita.ListaDisponibilita.Count > 0 Then

            Dim DT_Disponibilita As New DataTable
            Dim Dr_Disponibilita As DataRow
            DT_Disponibilita.Columns.Add(New DataColumn("Descrizione", GetType(String)))
            DT_Disponibilita.Columns.Add(New DataColumn("N", GetType(String)))
            DT_Disponibilita.Columns.Add(New DataColumn("P", GetType(String)))
            DT_Disponibilita.Columns.Add(New DataColumn("K", GetType(String)))

            For i = 0 To objParametriUscita.ListaDisponibilita.Count - 1
                Dr_Disponibilita = DT_Disponibilita.NewRow
                Dr_Disponibilita.Item("Descrizione") = objParametriUscita.ListaDisponibilita(i).Descrizione
                Dr_Disponibilita.Item("N") = Format(objParametriUscita.ListaDisponibilita(i).Valore_N, "0.00")
                Dr_Disponibilita.Item("P") = Format(objParametriUscita.ListaDisponibilita(i).Valore_P, "0.00")
                Dr_Disponibilita.Item("K") = Format(objParametriUscita.ListaDisponibilita(i).Valore_K, "0.00")
                DT_Disponibilita.Rows.Add(Dr_Disponibilita)
            Next

            Dr_Disponibilita = DT_Disponibilita.NewRow
            Dr_Disponibilita.Item("Descrizione") = "Totale Disponibità"
            Dr_Disponibilita.Item("N") = Format(objParametriUscita.N_Disponibile, "0.00")
            Dr_Disponibilita.Item("P") = Format(objParametriUscita.P_Disponibile, "0.00")
            Dr_Disponibilita.Item("K") = Format(objParametriUscita.K_Disponibile, "0.00")
            DT_Disponibilita.Rows.Add(Dr_Disponibilita)

            Dr_Disponibilita = DT_Disponibilita.NewRow
            Dr_Disponibilita.Item("Descrizione") = "Bisogno Calcolato"
            Dr_Disponibilita.Item("N") = Format(objParametriUscita.N_Calcolato, "0.00")
            Dr_Disponibilita.Item("P") = Format(objParametriUscita.P_Calcolato, "0.00")
            Dr_Disponibilita.Item("K") = Format(objParametriUscita.K_Calcolato, "0.00")
            DT_Disponibilita.Rows.Add(Dr_Disponibilita)

            Dr_Disponibilita = DT_Disponibilita.NewRow
            Dr_Disponibilita.Item("Descrizione") = "Apporto ammesso col bilancio"
            Dr_Disponibilita.Item("N") = Format(objParametriUscita.N_Ammesso, "0.00")
            Dr_Disponibilita.Item("P") = Format(objParametriUscita.P_Ammesso, "0.00")
            Dr_Disponibilita.Item("K") = Format(objParametriUscita.K_Ammesso, "0.00")
            DT_Disponibilita.Rows.Add(Dr_Disponibilita)

            GrdDisponibilita.DataSource = DT_Disponibilita
            GrdDisponibilita.DataBind()

            GrdDisponibilita.Rows(GrdDisponibilita.Rows.Count - 1).Font.Bold = True

        End If

        N_Ammesso = objParametriUscita.N_Ammesso
        K_Ammesso = objParametriUscita.K_Ammesso
        P_Ammesso = objParametriUscita.P_Ammesso

        '(09/03/2020 fede) aggiunta eventuale considerazione del fattore correttivo per resa maggiore
        Dim MAS As Decimal
        MAS = objParametriUscita.Limite_Mas
        Dim strFattoreCorrettivo As String = ""

        If MAS >= 0 Then

            If objParametriUscita.FattoreCorrettivo_N > 0 Then
                If objParametriIngresso.Resa > 0 AndAlso objParametriUscita.Resa_Rif > 0 Then
                    If objParametriIngresso.Resa - objParametriUscita.Resa_Rif > 0 Then
                        MAS = MAS + ((objParametriIngresso.Resa - objParametriUscita.Resa_Rif) * objParametriUscita.FattoreCorrettivo_N)
                        strFattoreCorrettivo = " (" & "considerando il Fattore Correttivo di " & objParametriUscita.FattoreCorrettivo_N & " Kg N/t)"
                    End If
                End If
            End If

            If objParametriUscita.N_Ammesso > MAS Then
                LblAttenzione.Text = "ATTENZIONE: l'apporto di N non può superare il limite MAS di " & MAS.ToString & " kg/ha" & strFattoreCorrettivo
                N_Ammesso = MAS
            Else
                LblAttenzione.Text = "Limite MAS: " & MAS.ToString & " kg/ha" & strFattoreCorrettivo
            End If

        End If

    End Sub

    Private Sub CalcolaSchede(ByVal Veg_Cod As Integer, ByVal Grfi_Cod As Integer, ByVal Fase_Cod As Integer, ByVal Resa As Decimal,
                              ByVal Piovosita As Decimal, ByVal PiovositaFeb As Decimal, ByVal Fattori As String,
                              ByRef N_Ammesso As Decimal, ByRef P_Ammesso As Decimal, ByRef K_Ammesso As Decimal)


        Txt_DoseStandard.Text = 0
        Txt_DoseStandardP.Text = 0
        Txt_DoseStandardK.Text = 0

        Txt_MAS.Text = 0
        Txt_MAS.Text = "n.d"


        Txt_TotDecrementi.Text = 0
        Txt_TotIncrementi.Text = 0
        Txt_TotDecrementiP.Text = 0
        Txt_TotIncrementiP.Text = 0
        Txt_TotDecrementiK.Text = 0
        Txt_TotIncrementiK.Text = 0

        Txt_DoseRicalcolata.Text = 0
        Txt_DoseRicalcolataP.Text = 0
        Txt_DoseRicalcolataK.Text = 0

        Dim Regolamento_Cod As Integer = CInt(ddlRegolamento.SelectedValue)

        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS

        ' MAS

        Dim ResaRif As Decimal = 0
        Dim Mas As Decimal = -1
        Dim N_Fattore_Correttivo_Resa As Decimal = 0

        Dim objParametriIngressoMAS As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_input
        objParametriIngressoMAS.Regolamento_Cod = Regolamento_Cod
        objParametriIngressoMAS.Veg_Cod = Veg_Cod
        objParametriIngressoMAS.Grfi_Cod = Grfi_Cod
        objParametriIngressoMAS.Stato_Cod = enum_Stato_Impianto.Impianto_Produzione
        objParametriIngressoMAS.ValoreMax = True

        Dim objParametriUscitaMAS As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_output
        objParametriUscitaMAS = objPC_WS.LimiteMAS(objParametriIngressoMAS)

        '(09/03/2020 fede) aggiunta eventuale considerazione del fattore correttivo per resa maggiore
        If objParametriUscitaMAS.Resa >= 0 Then
            ResaRif = objParametriUscitaMAS.Resa
        End If
        If objParametriUscitaMAS.FattoreCorrettivo_N > 0 Then
            N_Fattore_Correttivo_Resa = objParametriUscitaMAS.FattoreCorrettivo_N
        End If
        If objParametriUscitaMAS.N >= 0 Then
            Txt_MAS.Text = objParametriUscitaMAS.N
        End If

        If IsNumeric(Txt_MAS.Text) Then
            Mas = CDec(Txt_MAS.Text)
            If N_Fattore_Correttivo_Resa > 0 Then
                If Resa AndAlso ResaRif > 0 Then
                    If Resa - ResaRif > 0 Then
                        Mas = Mas + (Resa - ResaRif * N_Fattore_Correttivo_Resa)
                    End If
                End If
            End If
            Txt_MAS.Text = Mas
        End If


        ' FATTORI CORRETTIVI

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input

        objParametriIngresso.Regolamento_Cod = Regolamento_Cod
        objParametriIngresso.Veg_Cod = Veg_Cod
        objParametriIngresso.Grfi_Cod = Grfi_Cod
        objParametriIngresso.SoloValorizzati = True
        objParametriIngresso.SoloVisibili = False

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output
        objParametriUscita = objPC_WS.FattoriCorrettivi_Leggi(objParametriIngresso)

        Dim Dt_Incrementi_N As New DataTable
        Dim Dt_Decrementi_N As New DataTable
        Dim Dt_Incrementi_P As New DataTable
        Dim Dt_Decrementi_P As New DataTable
        Dim Dt_Incrementi_K As New DataTable
        Dim Dt_Decrementi_K As New DataTable

        Dt_Incrementi_N.Columns.Add(New DataColumn("Fattore_Cod", GetType(Integer)))
        Dt_Incrementi_N.Columns.Add(New DataColumn("Fattore_Des", GetType(String)))
        Dt_Incrementi_N.Columns.Add(New DataColumn("Valore", GetType(String)))
        Dt_Incrementi_N.Columns.Add(New DataColumn("Controllo_Valore", GetType(Decimal)))
        Dt_Incrementi_N.Columns.Add(New DataColumn("Controllo_Funzione", GetType(String)))
        Dt_Decrementi_N.Columns.Add(New DataColumn("Fattore_Cod", GetType(Integer)))
        Dt_Decrementi_N.Columns.Add(New DataColumn("Fattore_Des", GetType(String)))
        Dt_Decrementi_N.Columns.Add(New DataColumn("Valore", GetType(String)))
        Dt_Decrementi_N.Columns.Add(New DataColumn("Controllo_Valore", GetType(Decimal)))
        Dt_Decrementi_N.Columns.Add(New DataColumn("Controllo_Funzione", GetType(String)))

        Dt_Incrementi_P.Columns.Add(New DataColumn("Fattore_Cod", GetType(Integer)))
        Dt_Incrementi_P.Columns.Add(New DataColumn("Fattore_Des", GetType(String)))
        Dt_Incrementi_P.Columns.Add(New DataColumn("Valore", GetType(String)))
        Dt_Incrementi_P.Columns.Add(New DataColumn("Controllo_Valore", GetType(Decimal)))
        Dt_Incrementi_P.Columns.Add(New DataColumn("Controllo_Funzione", GetType(String)))
        Dt_Decrementi_P.Columns.Add(New DataColumn("Fattore_Cod", GetType(Integer)))
        Dt_Decrementi_P.Columns.Add(New DataColumn("Fattore_Des", GetType(String)))
        Dt_Decrementi_P.Columns.Add(New DataColumn("Valore", GetType(String)))
        Dt_Decrementi_P.Columns.Add(New DataColumn("Controllo_Valore", GetType(Decimal)))
        Dt_Decrementi_P.Columns.Add(New DataColumn("Controllo_Funzione", GetType(String)))

        Dt_Incrementi_K.Columns.Add(New DataColumn("Fattore_Cod", GetType(Integer)))
        Dt_Incrementi_K.Columns.Add(New DataColumn("Fattore_Des", GetType(String)))
        Dt_Incrementi_K.Columns.Add(New DataColumn("Valore", GetType(String)))
        Dt_Incrementi_K.Columns.Add(New DataColumn("Controllo_Valore", GetType(Decimal)))
        Dt_Incrementi_K.Columns.Add(New DataColumn("Controllo_Funzione", GetType(String)))
        Dt_Decrementi_K.Columns.Add(New DataColumn("Fattore_Cod", GetType(Integer)))
        Dt_Decrementi_K.Columns.Add(New DataColumn("Fattore_Des", GetType(String)))
        Dt_Decrementi_K.Columns.Add(New DataColumn("Valore", GetType(String)))
        Dt_Decrementi_K.Columns.Add(New DataColumn("Controllo_Valore", GetType(Decimal)))
        Dt_Decrementi_K.Columns.Add(New DataColumn("Controllo_Funzione", GetType(String)))

        ddlDoseP.Items.Clear()
        ddlDoseK.Items.Clear()


        Dim ResaBassa As Decimal = 0
        Dim ResaAlta As Decimal = 0
        Dim ResaBassaDes As String = ""
        Dim ResaAltaDes As String = ""
        Dim N_Max_Incrementi As Decimal = 0
        Dim N_Dose_Standard As Decimal = 0
        Dim P_Dose_Standard As Decimal = 0
        Dim K_Dose_Standard As Decimal = 0



        'necessita
        If objParametriUscita.ListaFattoriCorrettivi.Count > 0 Then

            Dim FattoreResaBassa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            FattoreResaBassa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.Resa_Bassa And x.Visibile = 1)(0)
            If Not FattoreResaBassa Is Nothing Then
                ResaBassa = FattoreResaBassa.Valore
                ResaBassaDes = FattoreResaBassa.Descrizione ' & " " & FattoreResaBassa.Valore
            End If

            Dim FattoreResaAlta As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            FattoreResaAlta = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.Resa_Alta And x.Visibile = 1)(0)
            If Not FattoreResaAlta Is Nothing Then
                ResaAlta = FattoreResaAlta.Valore
                ResaAltaDes = FattoreResaAlta.Descrizione '& " " & FattoreResaAlta.Valore
            End If


            'DOSI STANDARD

            Select Case Fase_Cod

                Case enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto
                    'rimangono = 0

                Case enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento

                    Dim Fattore_N_I_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Fattore_N_I_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_I_anno_allevamento)(0)
                    If Not Fattore_N_I_Anno_Allevamento Is Nothing Then
                        N_Dose_Standard = Fattore_N_I_Anno_Allevamento.Valore
                    End If
                    Txt_DoseStandard.Text = N_Dose_Standard

                    Dim Fattore_P_I_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Fattore_P_I_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_I_anno_allevamento)(0)
                    If Not Fattore_P_I_Anno_Allevamento Is Nothing Then
                        P_Dose_Standard = Fattore_P_I_Anno_Allevamento.Valore
                    End If
                    Txt_DoseStandardP.Text = P_Dose_Standard

                    Dim Fattore_K_I_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Fattore_K_I_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_I_anno_allevamento)(0)
                    If Not Fattore_K_I_Anno_Allevamento Is Nothing Then
                        K_Dose_Standard = Fattore_K_I_Anno_Allevamento.Valore
                    End If
                    Txt_DoseStandardK.Text = K_Dose_Standard

                    N_Ammesso = N_Dose_Standard
                    P_Ammesso = P_Dose_Standard
                    K_Ammesso = K_Dose_Standard


                Case enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento

                    Dim Fattore_N_II_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Fattore_N_II_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_II_anno_allevamento)(0)
                    If Not Fattore_N_II_Anno_Allevamento Is Nothing Then
                        N_Dose_Standard = Fattore_N_II_Anno_Allevamento.Valore
                    End If
                    Txt_DoseStandard.Text = N_Dose_Standard

                    Dim Fattore_P_II_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Fattore_P_II_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_II_anno_allevamento)(0)
                    If Not Fattore_P_II_Anno_Allevamento Is Nothing Then
                        P_Dose_Standard = Fattore_P_II_Anno_Allevamento.Valore
                    End If
                    Txt_DoseStandardP.Text = P_Dose_Standard

                    Dim Fattore_K_II_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Fattore_K_II_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_II_anno_allevamento)(0)
                    If Not Fattore_K_II_Anno_Allevamento Is Nothing Then
                        K_Dose_Standard = Fattore_K_II_Anno_Allevamento.Valore
                    End If
                    Txt_DoseStandardK.Text = K_Dose_Standard

                    N_Ammesso = N_Dose_Standard
                    P_Ammesso = P_Dose_Standard
                    K_Ammesso = K_Dose_Standard

                Case Else

                    Dim Fattore_N_Dose_Standard As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Fattore_N_Dose_Standard = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_Dose_Standard)(0)
                    If Not Fattore_N_Dose_Standard Is Nothing Then
                        Txt_DoseStandard.Text = Fattore_N_Dose_Standard.Valore
                    End If

                    Dim Lista_Dose_P As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
                    Lista_Dose_P = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "P" And UCase(x.Variazione) = "STANDARD" And x.Visibile = 1).ToList
                    ddlDoseP.Items.Clear()
                    For i = 0 To Lista_Dose_P.Count - 1
                        ddlDoseP.Items.Add(New ListItem(Lista_Dose_P(i).Descrizione, Lista_Dose_P(i).Codice & "|" & Lista_Dose_P(i).Valore))
                    Next

                    If Not ddlDoseP Is Nothing AndAlso ddlDoseP.Items.Count > 0 Then
                        Txt_DoseStandardP.Text = ddlDoseP.SelectedValue.Split("|")(1)
                    End If

                    Dim Lista_Dose_K As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
                    Lista_Dose_K = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "K" And UCase(x.Variazione) = "STANDARD" And x.Visibile = 1).ToList
                    ddlDoseK.Items.Clear()
                    For i = 0 To Lista_Dose_K.Count - 1
                        ddlDoseK.Items.Add(New ListItem(Lista_Dose_K(i).Descrizione, Lista_Dose_K(i).Codice & "|" & Lista_Dose_K(i).Valore))
                    Next

                    If Not ddlDoseK Is Nothing AndAlso ddlDoseK.Items.Count > 0 Then
                        Txt_DoseStandardK.Text = ddlDoseK.SelectedValue.Split("|")(1)
                    End If

            End Select

            Txt_DoseRicalcolata.Text = Txt_DoseStandard.Text
            Txt_DoseRicalcolataP.Text = Txt_DoseStandardP.Text
            Txt_DoseRicalcolataK.Text = Txt_DoseStandardK.Text


            ' AZOTO

            Dim Fattore_N_Inc_Max As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_N_Inc_Max = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_Inc_Max)(0)
            If Not Fattore_N_Inc_Max Is Nothing Then
                Txt_MaxIncrementi.Text = Fattore_N_Inc_Max.Valore
            Else
                Txt_MaxIncrementi.Text = 0
            End If


            Dim Fattore_N_Inc_Resa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_N_Inc_Resa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_Inc_Resa)(0)
            If Not Fattore_N_Inc_Resa Is Nothing Then
                InserisciRigaDt(ResaAltaDes & " " & Fattore_N_Inc_Resa.Controllo_Valore, Fattore_N_Inc_Resa.Codice, Fattore_N_Inc_Resa.Valore, Fattore_N_Inc_Resa.Controllo_Funzione, Fattore_N_Inc_Resa.Controllo_Valore, Dt_Incrementi_N)
            End If

            Dim Lista_Incrementi_N As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Incrementi_N = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "N" And UCase(x.Variazione) = "INCREMENTO" And x.Visibile = 1).ToList
            For i = 0 To Lista_Incrementi_N.Count - 1
                InserisciRigaDt(Lista_Incrementi_N(i).Descrizione, Lista_Incrementi_N(i).Codice, Lista_Incrementi_N(i).Valore, Lista_Incrementi_N(i).Controllo_Funzione, Lista_Incrementi_N(i).Controllo_Valore, Dt_Incrementi_N)
            Next

            Dim Fattore_N_Dec_Resa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_N_Dec_Resa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_Dec_Resa)(0)
            If Not Fattore_N_Dec_Resa Is Nothing Then
                InserisciRigaDt(ResaBassaDes & " " & Fattore_N_Dec_Resa.Controllo_Valore, Fattore_N_Dec_Resa.Codice, Fattore_N_Dec_Resa.Valore, Fattore_N_Dec_Resa.Controllo_Funzione, Fattore_N_Dec_Resa.Controllo_Valore, Dt_Decrementi_N)
            End If

            Dim Lista_Decrementi_N As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Decrementi_N = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "N" And UCase(x.Variazione) = "DECREMENTO" And x.Visibile = 1).ToList
            For i = 0 To Lista_Decrementi_N.Count - 1
                InserisciRigaDt(Lista_Decrementi_N(i).Descrizione, Lista_Decrementi_N(i).Codice, Lista_Decrementi_N(i).Valore, Lista_Decrementi_N(i).Controllo_Funzione, Lista_Decrementi_N(i).Controllo_Valore, Dt_Decrementi_N)
            Next

            ' FOSFORO

            Dim Fattore_P_Inc_Resa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_P_Inc_Resa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_Inc_Resa)(0)
            If Not Fattore_P_Inc_Resa Is Nothing Then
                InserisciRigaDt(ResaAltaDes & " " & Fattore_P_Inc_Resa.Controllo_Valore, Fattore_P_Inc_Resa.Codice, Fattore_P_Inc_Resa.Valore, Fattore_P_Inc_Resa.Controllo_Funzione, Fattore_P_Inc_Resa.Controllo_Valore, Dt_Incrementi_P)
            End If

            Dim Lista_Incrementi_P As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Incrementi_P = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "P" And UCase(x.Variazione) = "INCREMENTO" And x.Visibile = 1).ToList
            For i = 0 To Lista_Incrementi_P.Count - 1
                InserisciRigaDt(Lista_Incrementi_P(i).Descrizione, Lista_Incrementi_P(i).Codice, Lista_Incrementi_P(i).Valore, Lista_Incrementi_P(i).Controllo_Funzione, Lista_Incrementi_P(i).Controllo_Valore, Dt_Incrementi_P)
            Next

            Dim Fattore_P_Dec_Resa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_P_Dec_Resa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_Dec_Resa)(0)
            If Not Fattore_P_Dec_Resa Is Nothing Then
                InserisciRigaDt(ResaBassaDes & " " & Fattore_P_Dec_Resa.Controllo_Valore, Fattore_P_Dec_Resa.Codice, Fattore_P_Dec_Resa.Valore, Fattore_P_Dec_Resa.Controllo_Funzione, Fattore_P_Dec_Resa.Controllo_Valore, Dt_Decrementi_P)
            End If

            Dim Lista_Decrementi_P As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Decrementi_P = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "P" And UCase(x.Variazione) = "DECREMENTO" And x.Visibile = 1).ToList
            For i = 0 To Lista_Decrementi_P.Count - 1
                InserisciRigaDt(Lista_Decrementi_P(i).Descrizione, Lista_Decrementi_P(i).Codice, Lista_Decrementi_P(i).Valore, Lista_Decrementi_P(i).Controllo_Funzione, Lista_Decrementi_P(i).Controllo_Valore, Dt_Decrementi_P)
            Next


            'POTASSIO

            Dim Fattore_K_Inc_Resa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_K_Inc_Resa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_Inc_Resa)(0)
            If Not Fattore_K_Inc_Resa Is Nothing Then
                InserisciRigaDt(ResaAltaDes & " " & Fattore_K_Inc_Resa.Controllo_Valore, Fattore_K_Inc_Resa.Codice, Fattore_K_Inc_Resa.Valore, Fattore_K_Inc_Resa.Controllo_Funzione, Fattore_K_Inc_Resa.Controllo_Valore, Dt_Incrementi_K)
            End If

            Dim Lista_Incrementi_K As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Incrementi_K = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "K" And UCase(x.Variazione) = "INCREMENTO" And x.Visibile = 1).ToList
            For i = 0 To Lista_Incrementi_K.Count - 1
                InserisciRigaDt(Lista_Incrementi_K(i).Descrizione, Lista_Incrementi_K(i).Codice, Lista_Incrementi_K(i).Valore, Lista_Incrementi_K(i).Controllo_Funzione, Lista_Incrementi_K(i).Controllo_Valore, Dt_Incrementi_K)
            Next

            Dim Fattore_K_Dec_Resa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_K_Dec_Resa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_Dec_Resa)(0)
            If Not Fattore_K_Dec_Resa Is Nothing Then
                InserisciRigaDt(ResaBassaDes & " " & Fattore_K_Dec_Resa.Controllo_Valore, Fattore_K_Dec_Resa.Codice, Fattore_K_Dec_Resa.Valore, Fattore_K_Dec_Resa.Controllo_Funzione, Fattore_K_Dec_Resa.Controllo_Valore, Dt_Decrementi_K)
            End If

            Dim Lista_Decrementi_K As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Decrementi_K = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "K" And UCase(x.Variazione) = "DECREMENTO" And x.Visibile = 1).ToList
            For i = 0 To Lista_Decrementi_K.Count - 1
                InserisciRigaDt(Lista_Decrementi_K(i).Descrizione, Lista_Decrementi_K(i).Codice, Lista_Decrementi_K(i).Valore, Lista_Decrementi_K(i).Controllo_Funzione, Lista_Decrementi_K(i).Controllo_Valore, Dt_Decrementi_K)
            Next


            Dim DtKeys(3) As String
            DtKeys(0) = "Fattore_Cod"
            DtKeys(1) = "Valore"
            DtKeys(2) = "Controllo_Funzione"
            DtKeys(3) = "Controllo_Valore"


            grdIncrementi.DataSource = Dt_Incrementi_N
            grdIncrementi.DataKeyNames = DtKeys
            grdIncrementi.DataBind()
            grdDecrementi.DataSource = Dt_Decrementi_N
            grdDecrementi.DataKeyNames = DtKeys
            grdDecrementi.DataBind()
            grdIncrementiP.DataSource = Dt_Incrementi_P
            grdIncrementiP.DataKeyNames = DtKeys
            grdIncrementiP.DataBind()
            grdDecrementiP.DataSource = Dt_Decrementi_P
            grdDecrementiP.DataKeyNames = DtKeys
            grdDecrementiP.DataBind()
            grdIncrementiK.DataSource = Dt_Incrementi_K
            grdIncrementiK.DataKeyNames = DtKeys
            grdIncrementiK.DataBind()
            grdDecrementiK.DataSource = Dt_Decrementi_K
            grdDecrementiK.DataKeyNames = DtKeys
            grdDecrementiK.DataBind()

            Select Case Fase_Cod

                Case enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto,
                     enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento,
                     enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento

                Case Else

                    ' se sono in modifica seleziono i fattori salvati
                    If Fattori <> "" Then

                        Dim ArrayFattori() As String = Fattori.Split(",")

                        ' azoto
                        Dim totIncN As Decimal = ImpostaFattoriSelezionati(grdIncrementi, "Chk_SelIncrementi_N", ArrayFattori)
                        Dim totDecN As Decimal = ImpostaFattoriSelezionati(grdDecrementi, "Chk_SelDecrementi_N", ArrayFattori)

                        Txt_TotDecrementi.Text = totDecN.ToString
                        Txt_TotIncrementi.Text = totIncN.ToString

                        'piano nuovo NPK sono nella tabella dei dettagli (già letto sopra)
                        'If IsNumeric(N_Ammesso.Value) Then
                        '    Txt_DoseRicalcolata.Text = CDec(N_Ammesso.Value)
                        'Else
                        Dim doseRicN As Decimal
                        Dim doseStd As Decimal = Txt_DoseStandard.Text
                        Dim doseMas As Decimal = IIf(IsNumeric(Txt_MAS.Text), Txt_MAS.Text, 0)
                        Dim maxIncr As Decimal = Txt_MaxIncrementi.Text

                        doseRicN = doseStd + totIncN - totDecN

                        If doseRicN > doseStd + maxIncr Then
                            doseRicN = doseStd + maxIncr
                        End If

                        If doseMas <> 0 Then
                            If doseRicN > doseMas Then
                                doseRicN = doseMas
                            End If
                        End If

                        Txt_DoseRicalcolata.Text = doseRicN.ToString
                        'End If

                        ' fosforo
                        Dim totIncP As Decimal = ImpostaFattoriSelezionati(grdIncrementiP, "Chk_SelIncrementi_P", ArrayFattori)
                        Dim totDecP As Decimal = ImpostaFattoriSelezionati(grdDecrementiP, "Chk_SelDecrementi_P", ArrayFattori)

                        Txt_TotDecrementiP.Text = totDecP.ToString
                        Txt_TotIncrementiP.Text = totIncP.ToString

                        ImpostaFattoriSelezionati(ddlDoseP, Txt_DoseStandardP, ArrayFattori)

                        'If IsNumeric(P_Ammesso.Value) Then
                        '    Txt_DoseRicalcolataP.Text = CDec(P_Ammesso.Value)
                        'Else
                        Dim doseRicP As Decimal
                        Dim doseStdP As Decimal = Txt_DoseStandardP.Text
                        doseRicP = doseStdP + totIncP - totDecP
                        Txt_DoseRicalcolataP.Text = doseRicP.ToString
                        'End If

                        ' potassio
                        Dim totIncK As Decimal = ImpostaFattoriSelezionati(grdIncrementiK, "Chk_SelIncrementi_K", ArrayFattori)
                        Dim totDecK As Decimal = ImpostaFattoriSelezionati(grdDecrementiK, "Chk_SelDecrementi_K", ArrayFattori)

                        Txt_TotDecrementiK.Text = totDecK.ToString
                        Txt_TotIncrementiK.Text = totIncK.ToString

                        ImpostaFattoriSelezionati(ddlDoseK, Txt_DoseStandardK, ArrayFattori)

                        'If IsNumeric(K_Ammesso.Value) Then
                        '    Txt_DoseRicalcolataK.Text = CDec(K_Ammesso.Value)
                        'Else
                        Dim doseRicK As Decimal
                        Dim doseStdK As Decimal = Txt_DoseStandardK.Text
                        doseRicK = doseStdK + totIncK - totDecK
                        Txt_DoseRicalcolataK.Text = doseRicK.ToString
                        'End If


                    Else

                        ' N

                        Dim Sabbia As Decimal = 0
                        If IsNumeric(Txt_Sabbia.Text) Then
                            Sabbia = CDec(Txt_Sabbia.Text)
                        End If
                        Dim Argilla As Decimal = 0
                        If IsNumeric(Txt_Argilla.Text) Then
                            Argilla = CDec(Txt_Argilla.Text)
                        End If
                        Dim SO As Decimal = 0
                        If IsNumeric(Txt_SO.Text) Then
                            SO = CDec(Txt_SO.Text)
                        End If
                        Dim CalcAtt As Decimal = 0
                        If IsNumeric(Txt_CalcAtt.Text) Then
                            CalcAtt = CDec(Txt_CalcAtt.Text)
                        End If
                        Dim P As Decimal = 0
                        If IsNumeric(Txt_P.Text) Then
                            P = CDec(Txt_P.Text)
                        End If
                        Dim K As Decimal = 0
                        If IsNumeric(Txt_K.Text) Then
                            K = CDec(Txt_K.Text)
                        End If
                        Dim Mg As Decimal = 0
                        If IsNumeric(Txt_Mg.Text) Then
                            Mg = CDec(Txt_Mg.Text)
                        End If
                        Dim CSC As Decimal = 0
                        If IsNumeric(Txt_CSC.Text) Then
                            CSC = CDec(Txt_CSC.Text)
                        End If

                        Dim doseStd As Decimal = N_Dose_Standard
                        Dim doseMas As Decimal = CDec(Txt_MAS.Text)
                        Dim maxIncr As Decimal = N_Max_Incrementi

                        Dim totIncN As Decimal = ImpostaFattoriDefault(grdIncrementi, "Chk_SelIncrementi_N", Resa, Argilla, Sabbia, SO, CalcAtt, Piovosita, PiovositaFeb, Fattori)
                        Dim totDecN As Decimal = ImpostaFattoriDefault(grdDecrementi, "Chk_SelDecrementi_N", Resa, Argilla, Sabbia, SO, CalcAtt, Piovosita, PiovositaFeb, Fattori)

                        Txt_TotDecrementi.Text = totDecN.ToString
                        Txt_TotIncrementi.Text = totIncN.ToString

                        Dim doseRicN As Decimal
                        doseRicN = doseStd + totIncN - totDecN

                        If doseRicN > doseStd + maxIncr Then
                            doseRicN = doseStd + maxIncr
                        End If

                        If doseMas <> 0 Then
                            If doseRicN > doseMas Then
                                doseRicN = doseMas
                            End If
                        End If

                        Txt_DoseRicalcolata.Text = doseRicN.ToString
                        N_Ammesso = doseRicN

                        ' P
                        Dim doseStdP As Decimal = 0
                        ImpostaDoseStandard_P2O5_Default(P, Fattori, doseStdP)
                        Txt_DoseStandardP.Text = doseStdP

                        Dim totIncP As Decimal = ImpostaFattoriDefault(grdIncrementiP, "Chk_SelIncrementi_P", Resa, Argilla, Sabbia, SO, CalcAtt, Piovosita, PiovositaFeb, Fattori)
                        Dim totDecP As Decimal = ImpostaFattoriDefault(grdDecrementiP, "Chk_SelDecrementi_P", Resa, Argilla, Sabbia, SO, CalcAtt, Piovosita, PiovositaFeb, Fattori)

                        Txt_TotDecrementiP.Text = totDecP.ToString
                        Txt_TotIncrementiP.Text = totIncP.ToString

                        Dim doseRicP As Decimal
                        doseRicP = doseStdP + totIncP - totDecP
                        Txt_DoseRicalcolataP.Text = doseRicP.ToString
                        P_Ammesso = doseRicP

                        ' K
                        Dim doseStdk As Decimal = 0
                        ImpostaDoseStandard_K2O_Default(Argilla, Sabbia, K, Mg, CSC, Fattori, doseStdk)
                        Txt_DoseStandardK.Text = doseStdk

                        Dim totIncK As Decimal = ImpostaFattoriDefault(grdIncrementiK, "Chk_SelIncrementi_K", Resa, Argilla, Sabbia, SO, CalcAtt, Piovosita, PiovositaFeb, Fattori)
                        Dim totDecK As Decimal = ImpostaFattoriDefault(grdDecrementiK, "Chk_SelDecrementi_K", Resa, Argilla, Sabbia, SO, CalcAtt, Piovosita, PiovositaFeb, Fattori)

                        Txt_TotDecrementiK.Text = totDecK.ToString
                        Txt_TotIncrementiK.Text = totIncK.ToString

                        Dim doseRicK As Decimal
                        doseRicK = doseStdk + totIncK - totDecK
                        Txt_DoseRicalcolataK.Text = doseRicK.ToString
                        K_Ammesso = doseRicK

                    End If

            End Select

        End If

        'valorizare
        N_Ammesso = Txt_DoseRicalcolata.Text
        P_Ammesso = Txt_DoseRicalcolataP.Text
        K_Ammesso = Txt_DoseRicalcolataK.Text

        Hidden_Fattori.Value = Fattori


    End Sub

    Private Function ImpostaFattoriSelezionati(ByVal gv As GridView, ByVal IdCheck As String, ByVal Fattori() As String) As Decimal

        Dim TotValori As Decimal = 0

        Dim i, j As Integer

        For i = 0 To Fattori.Length - 1
            If IsNumeric(Fattori(i)) Then
                For j = 0 To gv.Rows.Count - 1
                    If gv.DataKeys(j).Item("Fattore_Cod") = Fattori(i) Then
                        CType(gv.Rows(j).FindControl(IdCheck), CheckBox).Checked = True
                        TotValori += gv.DataKeys(j).Item("Valore")
                        Exit For
                    End If
                Next
            End If
        Next

        Return TotValori

    End Function

    Private Sub ImpostaFattoriSelezionati(ByRef ddl As DropDownList, ByRef txt As TextBox, ByVal Fattori() As String)

        Dim i, j As Integer

        For i = 0 To Fattori.Length - 1
            If IsNumeric(Fattori(i)) Then
                For j = 0 To ddl.Items.Count - 1
                    If ddl.Items(j).Value.Split("|")(0) = CInt(Fattori(i)) Then
                        ddl.SelectedIndex = j
                        txt.Text = ddl.SelectedValue.Split("|")(1)
                        Exit For
                    End If
                Next
            End If
        Next


    End Sub

    Private Sub BtnCalcolaBilancio_Click(sender As Object, e As EventArgs) Handles BtnCalcolaBilancio.Click

        Dim N_Ammesso As Decimal = 0
        Dim P_Ammesso As Decimal = 0
        Dim K_Ammesso As Decimal = 0

        Select Case Qs_Tipo
            Case enum_PianoConcimazione_Tipo.Bilancio
                CalcolaBilancio(Hidden_Veg_Cod.Value,
                                Hidden_Grfi_Cod.Value,
                                Hidden_Fase_Cod.Value,
                                Hidden_Resa.Value,
                                Hidden_AnticipazioniAnni.Value,
                                Hidden_Copertura.Value,
                                Hidden_UbicazioneCod.Value,
                                Hidden_PercNFissazione.Value,
                                Hidden_DispOssigeno.Value,
                                Hidden_Piovosita.Value,
                                Hidden_Precessione.Value,
                                Hidden_TipoFertilizzante.Value,
                                Hidden_IdFrequenza.Value,
                                Hidden_QtaN_FerPrec.Value,
                                Hidden_PiovositaFeb.Value,
                                        N_Ammesso, P_Ammesso, K_Ammesso)
            Case Else
                CalcolaSchede(Hidden_Veg_Cod.Value,
                              Hidden_Grfi_Cod.Value,
                              Hidden_Fase_Cod.Value,
                              Hidden_Resa.Value,
                              Hidden_Piovosita.Value,
                              Hidden_PiovositaFeb.Value,
                              Hidden_Fattori.Value,
                                        N_Ammesso, P_Ammesso, K_Ammesso)
        End Select


        Dim script As New StringBuilder
        script.AppendLine("<script type='text/javascript' >$(document).ready(function () { ")
        script.AppendLine("             $('#dialogDettaglioBilancio').modal();")
        script.AppendLine("     });</script>")

        Page.RegisterClientScriptBlock("", script.ToString)

    End Sub

    Private Sub ddlRegolamento_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRegolamento.SelectedIndexChanged

        Dim objParametri As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
        objParametri.Leggi()

        '(18/08/2020 fede) escluse analisi scadute
        Dim DataDa As Date = AGRODATAINIZIO
        Dim DataA As Date = AGRODATAFINE
        If IsDate(Txt_ValiditaInizio.Text) Then
            DataDa = CDate(Txt_ValiditaInizio.Text)
        End If
        If IsDate(Txt_ValiditaFine.Text) Then
            DataA = CDate(Txt_ValiditaFine.Text)
        End If

        Dim ListaPive As New List(Of String)
        ListaPive.Add(objParametri.Piva)
        Crea_Dt_Analisi(ListaPive, DataDa, DataA)

        Carica_Impianti_Impresa(objParametri.Piva, 0, 0)

        Aggrega_Griglia_Impianti()

        Select Case Qs_Tipo
            Case enum_PianoConcimazione_Tipo.Bilancio
                Btn_Bilancio.Visible = True
                Btn_Schede.Visible = False
            Case Else
                Btn_Bilancio.Visible = False
                Btn_Schede.Visible = True
        End Select

    End Sub

    Private Sub CreaReportSchede(IndiceGriglia As Integer, PC_Testata_Cod As Integer, AnnoSalvato As Integer, ByVal Regolamento As enum_PUARegolamenti)

        Dim objPagReport As New Scheda_NPK

        Dim Campo, Appezza, IdImp As Integer
        Dim Sa_Nome As String = ""
        Dim Veg_Cod As Integer = 0
        Dim Grfi_Cod As Integer = 0
        Dim Fase_Cod As Integer = 0

        Dim StrImpianto As String = GridViewBilanci.DataKeys(IndiceGriglia).Item("AppezzaIdimp")

        Dim strPrimoImpianto As String = StrImpianto.Split("$")(0)

        If strPrimoImpianto <> String.Empty Then
            Campo = strPrimoImpianto.Split("|")(0)
            Appezza = strPrimoImpianto.Split("|")(1)
            IdImp = strPrimoImpianto.Split("|")(2)
        End If

        Dim rptSchedaNPK As New RPT_Scheda_NPK
        Dim Sup_Tot As Double = 0
        Dim N_Ammesso As Decimal = 0
        Dim P_Ammesso As Decimal = 0
        Dim K_Ammesso As Decimal = 0
        Dim Resa As Decimal = 0

        Dim Mas As Decimal = -1
        Dim MasSalvato As Boolean = False

        objPagReport.Dati_Piano(GridViewBilanci.DataKeys(IndiceGriglia).Item("Piva"),
                                  GridViewBilanci.DataKeys(IndiceGriglia).Item("Sa_Cod"),
                                  PC_Testata_Cod,
                                  Regolamento,
                                  Veg_Cod, Grfi_Cod, Fase_Cod, Resa,
                                  Sup_Tot, Sa_Nome,
                                  N_Ammesso, P_Ammesso, K_Ammesso,
                                   Mas, MasSalvato,
                                  objParametri_Server,
                                  rptSchedaNPK)


        objPagReport.public_Dati_Contenuto(Veg_Cod, Grfi_Cod, Fase_Cod, Resa, N_Ammesso, P_Ammesso, K_Ammesso, Sup_Tot, PC_Testata_Cod,
                                          GridViewBilanci.DataKeys(IndiceGriglia).Item("Piva"), Regolamento,
                                          Mas, MasSalvato,
                                          rptSchedaNPK, objParametri_Server)

        'Dim strCentro As String = GridViewBilanci.DataKeys(IndiceGriglia).Item("Centro").ToString.Replace("<BR>", "")
        Dim strCentro As String = Sa_Nome.Replace("<BR>", "")
        Dim objSpec As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        'Dim strSpecie As String = AgronicaCoreUtility.Stringhe.EliminaCaratteriSpecialiFile(GridViewBilanci.DataKeys(IndiceGriglia).Item("Veg_Des").ToString.Replace("<BR>", ""))
        Dim dtSpec As DataTable = objSpec.Leggi(CInt(GridViewBilanci.DataKeys(IndiceGriglia).Item("Veg_Cod")), 0, "", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        Dim strSpecie As String = " "
        If Not IsNothing(dtSpec) AndAlso dtSpec.Rows.Count > 0 Then
            strSpecie = dtSpec.Rows(0).Item("veg_des").ToString
        End If
        'strCentro = strCentro.Replace(" ", "")
        'strCentro = strCentro.Replace("'", "")
        'strCentro = strCentro.Replace("""", "")
        'strSpecie = strSpecie.Replace(" ", "")
        'strSpecie = strSpecie.Replace("'", "")

        strCentro = AgronicaCoreUtility.Stringhe.EliminaCaratteriSpecialiFile(strCentro)


        Dim CodSocio As String
        Dim CentroConferimento As String

        Dim objAnagrafe As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        CodSocio = objAnagrafe.Leggi_Codice_from_Imprese_Codici(GridViewBilanci.DataKeys(IndiceGriglia).Item("Piva"), enum_CodiciAnagrafe.Codice_Socio, objParametri_Server)

        CentroConferimento = objAnagrafe.Leggi_Codice_from_Imprese_Codici(GridViewBilanci.DataKeys(IndiceGriglia).Item("Piva"), enum_CodiciAnagrafe.V01CON00_VCCOIS, objParametri_Server)
        objAnagrafe = Nothing

        'Dim NomeFile As String = "PC" & ViewState("TestataCodSalvata") & "_" & strCentro.Substring(0, 15) & "_" & strSpecie.Substring(0, 15) & ".pdf"

        Dim NomeFile As String = "PCS" &
                                 IIf(CentroConferimento <> "", "_" & CentroConferimento, "") &
                                 "_" & strCentro.Substring(0, Math.Min(15, strCentro.Length - 1)) &
                                 IIf(CodSocio <> "", "_" & CodSocio, "") &
                                 "_" & strSpecie.Substring(0, Math.Min(15, strSpecie.Length - 1)) &
                                 "_" & PC_Testata_Cod & ".pdf"

        Dim strFile As String = objPagReport.public_SalvaPdf(objParametri_Server, rptSchedaNPK, NomeFile)

        'salvo il record dell'allegato se ha salvato il pdf
        If strFile <> "" Then

            Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
            Dim AllegatiDocumentiCod As Integer

            Dim anno As Integer
            If Not IsNothing(AnnoSalvato) Then
                anno = AnnoSalvato
            Else
                anno = Now.Year
            End If

            AllegatiDocumentiCod = objAllegati.SalvaAllegato(GridViewBilanci.DataKeys(IndiceGriglia).Item("Piva").ToString,
                                                             enum_CategorieDocumenti.PianoConcimazione,
                                                             "Piano Concimazione",
                                                             NomeFile,
                                                             Session("Sottocartella"),
                                                             PC_Testata_Cod, GridViewBilanci.DataKeys(IndiceGriglia).Item("Piva").ToString, "", "",
                                                             CDate("01/01/" & anno.ToString),
                                                             CDate("31/12/" & anno.ToString),
                                                             objParametri_Server)

            Dim objPC_dettagli_W As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_W
            'INSERISCO IL CODICE ALLEGATO NELLA TABELLA Piano_Concimazione_Dettagli del PC salvato
            If Not IsNothing(AllegatiDocumentiCod) AndAlso AllegatiDocumentiCod <> 0 Then
                If Not objPC_dettagli_W.UpdateCodAllegato(PC_Testata_Cod, 0, GridViewBilanci.DataKeys(IndiceGriglia).Item("Piva").ToString, AllegatiDocumentiCod, objParametri_Server) Then
                    Throw New Exception("Non sono riuscito ad associare l'allegato al corrente Piano di Concimanzione. PC_cod =" & PC_Testata_Cod.ToString)
                End If
            End If

            objAllegati = Nothing

        End If


        rptSchedaNPK.Close()
        rptSchedaNPK.Dispose()

        'rptBilancioNPK = Nothing
    End Sub

    Protected Sub CmbAggregazione_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbAggregazione.SelectedIndexChanged

        Aggrega_Griglia_Impianti()

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function SportelloPCB_Multiplo() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Try

            Dim objParametri As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
            objParametri.Leggi()

            Dim ListaPive As New List(Of String)

            'caso multi azienda multi specie non visualizzo la scelta di aggregare ne il catasto per ora per fretta (voluto da fabrizio)
            If Not IsNothing(objParametri) AndAlso Not IsNothing(objParametri.ListaImpianti) Then
                For Each impianto In objParametri.ListaImpianti
                    If Not ListaPive.Contains(impianto.Piva) Then
                        ListaPive.Add(impianto.Piva)
                    End If
                Next
            Else
                ListaPive.Add(objParametri.Piva)
            End If

            Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R

            Dim dataMin As Date = AGRODATAINIZIO
            Dim dataMax As Date = AGRODATAFINE
            Dim SportelloAperto As Boolean
            objPratiche.Data_Sportello_Da_Servizio_Pive_Multiple(ListaPive, enum_Servizi.PianoConcimazione, DateTime.Now, SportelloAperto, dataMin, dataMax, objParametri_Server, objParametri_Utenti)

            Dim objSportello As New JObject

            objSportello("Validita_Inizio") = CDate(dataMin)
            objSportello("Validita_Fine") = CDate(dataMax)
            objSportello("Sportello_Aperto") = CBool(SportelloAperto)

            r.RispostaOK = True
            r.RispostaStringa = objSportello.ToString

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r

    End Function


    'Private Shared Function GetListaFinalitaRer(ByVal regolamentoCod As Integer) As List(Of Finalita)

    '    Dim objParametriIngresso As New PianoConcimazione_FinalitaRER_input With {
    '        .Regolamento_Cod = regolamentoCod
    '    }

    '    Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
    '    Dim objParametriUscita As PianoConcimazione_FinalitaRER_output = objPC_WS.FinalitaRER(objParametriIngresso)

    '    Return objParametriUscita.ListaFinalita

    'End Function

    'Private Shared Function GetFinalitaRerDes(ByVal listaFinalita As List(Of Finalita), ByVal Grfi_Cod_Rer As Integer) As String

    '    Dim descrizione As String = ""

    '    If Not listaFinalita Is Nothing AndAlso listaFinalita.Count > 0 Then
    '        Dim obj = (From l In listaFinalita Where l.Codice = Grfi_Cod_Rer Select l).FirstOrDefault()

    '        If Not obj Is Nothing Then
    '            descrizione = obj.Descrizione
    '        End If
    '    End If

    '    Return descrizione

    'End Function


    Private Shared Function GetListaStatiImpiantiDes(ByVal regolamentoCod As Integer, ByVal vegCod As Integer) As List(Of Fase)

        Dim objParametriIngresso As New PianoConcimazione_FasiCicloColturale_input With {
            .Regolamento_Cod = regolamentoCod,
            .Veg_Cod = vegCod,
            .Url = ""
        }

        Dim objParametriUscita As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.FasiCicloColturale(objParametriIngresso)

        Return objParametriUscita.ListaFasi

    End Function


    Private Shared Function GetStatoImpiantoDes(ByRef dictStatoImpianto As Dictionary(Of Integer, List(Of Fase)),
                                              ByVal regolamentoCod As Integer,
                                              ByVal vegCod As Integer,
                                              ByVal statoImpiantoCod As Integer
                                              ) As String

        Dim descrizione As String = ""

        Dim listaStatiImpianti As List(Of Fase)
        If dictStatoImpianto.ContainsKey(vegCod) Then
            'avevo già letto gli stati, li recupero dal dizionario senza rileggere
            listaStatiImpianti = dictStatoImpianto(vegCod)
        Else
            'non avevo ancora letto gli stati per questo veg_cod, quindi li leggo
            listaStatiImpianti = GetListaStatiImpiantiDes(regolamentoCod, vegCod)
            dictStatoImpianto.Add(vegCod, listaStatiImpianti)
        End If

        If Not listaStatiImpianti Is Nothing AndAlso listaStatiImpianti.Count > 0 Then
            Dim obj = (From l In listaStatiImpianti Where l.Codice = statoImpiantoCod Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                descrizione = obj.Descrizione
            End If
        End If

        Return descrizione

    End Function


End Class