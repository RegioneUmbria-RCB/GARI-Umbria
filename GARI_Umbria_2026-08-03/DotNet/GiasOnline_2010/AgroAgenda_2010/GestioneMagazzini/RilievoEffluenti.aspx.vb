Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCorePianoConcimazioneBIZ
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp

Public Class RilievoEffluenti
    Inherits System.Web.UI.Page

    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Qs_Piva As String
    Public Qs_Sa_Cod As Integer
    Public Qs_Fabbricato_Cod As Integer

    Public Qs_PUA_Cod As Integer
    Public Qs_Regolamento_Cod As Integer

    Public QS_Data As Date
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        If Not IsNothing(Request.QueryString("piva")) Then
            Qs_Piva = Stringa_Decodifica(Request.QueryString("piva").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)
        End If

        If Not IsNothing(Request.QueryString("sa_cod")) Then
            Qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("sa_cod").ToString,
                                            AgroKey_EncoderDecoder,
                                            Server)
        End If

        If Not IsNothing(Request.QueryString("fabbricato_cod")) Then
            Qs_Fabbricato_Cod = Stringa_Decodifica(Request.QueryString("fabbricato_cod").ToString,
                                            AgroKey_EncoderDecoder,
                                            Server)
        End If



        If Not IsNothing(Request.QueryString("pua_cod")) Then
            Qs_PUA_Cod = Stringa_Decodifica(Request.QueryString("pua_cod").ToString,
                                            AgroKey_EncoderDecoder,
                                            Server)
        End If

        If Not IsNothing(Request.QueryString("regolamento_cod")) Then
            Qs_Regolamento_Cod = Stringa_Decodifica(Request.QueryString("regolamento_cod").ToString,
                                            AgroKey_EncoderDecoder,
                                            Server)
            'Else
            '    Qs_Regolamento_Cod = objPua.Regolamento_Cod
        End If

        If Not IsNothing(Request.QueryString("data")) Then
            QS_Data = Stringa_Decodifica(Request.QueryString("data").ToString,
                                            AgroKey_EncoderDecoder,
                                            Server)
        End If

        Dim _aperturadaIFrame As Integer = 0

        If Not IsNothing(Request.QueryString("Ifr")) Then
            _aperturadaIFrame = CInt(Stringa_Decodifica(Request.QueryString("Ifr").ToString, AgroKey_EncoderDecoder))
        End If


        hdApertodaGiasNG.Value = "False"

        If _aperturadaIFrame = 1 Then

            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False

            Dim objParametriAgenda As New ParametriAgenda

            If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG Then
                hdApertodaGiasNG.Value = "True"
            End If
        End If


        hdPiva.Value = Qs_Piva
        hdSaCod.Value = Qs_Sa_Cod
        hdFabbricatoCod.Value = Qs_Fabbricato_Cod
        hdPuaCod.Value = Qs_PUA_Cod
        hdRegCod.Value = Qs_Regolamento_Cod
        hdData.Value = QS_Data

        Master.flag_MostraHeader = False
        Master.flag_MostraFooter = False

        If Not Page.IsPostBack Then




        End If

    End Sub




    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiGiacenzeEffluenti(
        ByVal piva As String, ByVal sacod As Integer, ByVal fabbricatocod As Integer, ByVal data As String, ByVal puacod As Integer, ByVal regolamentocod As Integer
    ) As rispostaStandard(Of Verifica_DisciplinareBS_LeggiOperazioni)

        'Dim r As New RispostaStandard

        Dim r As New rispostaStandard(Of Verifica_DisciplinareBS_LeggiOperazioni)
        r.RispostaStringa = New Verifica_DisciplinareBS_LeggiOperazioni

        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Utenti) OrElse IsNothing(objParametri_Super_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objPC As New AgronicaCoreWebService.PianoConcimazione_WS
            Dim objEffluentiInput As New PUA_Effluenti_input With {
                    .Regolamento_Cod = regolamentocod
                }
            Dim objEffluentiOutput As New PUA_Effluenti_output
            objEffluentiOutput = objPC.Effluenti(objEffluentiInput, objParametri_Server, objParametri_Super_Server)

            Dim objEff As New AgronicaCorePUA_DAL.Pua_Effluente_R
            Dim DtEffluenti As DataTable = objEff.Leggi(regolamentocod, puacod, 0, " azoto_qta >0 AND Flag_ProvenienzaEsterna = 0 ", "", objParametri_Server)

            Dim ListaEffluenti As New List(Of PUA_Effluente_Aziendale)
            Dim Effluente As PUA_Effluente_Aziendale
            Dim Filtro_Fertilizzanti As String = ""

            Dim objG As New AgronicaCoreStampeDAL.Magazzino

            Dim DT_Effluente As New DataTable
            Dim DT_Giacenze As New DataTable

            Crea_DT_EffluenteMagazzino(DT_Effluente)
            Crea_DT_GiacenzeEffluentiMagazzino(DT_Giacenze)

            Dim Dt_Giacenza_Data As DataTable

            Dim DataGiacenza As Date

            Dim Qta_Dichiarata As Decimal
            Dim Udm_Cod_Dichiarata As Integer
            Dim Udm_Sim_Dichiarata As String

            Dim Qta_Data As Decimal
            Dim Udm_Cod_Data As Integer
            Dim Udm_Sim_Data As String

            Dim Qta_Data_Stimata As Decimal
            Dim Udm_Cod_Data_Stimata As Integer
            Dim Udm_Sim_Data_Stimata As String

            Dim Qta_Data_Stimata_mese As Decimal
            Dim Udm_Cod_Data_Stimata_mese As Integer
            Dim Udm_Sim_Data_Stimata_mese As String

            Dim objApporti As New AgronicaCorePUA_BIZ.PUA_Apporti_BIZ

            Dim Data_Inizio As Date
            Dim Data_Fine As Date

            If DtEffluenti IsNot Nothing AndAlso DtEffluenti.Rows.Count > 0 Then

                'Leggo i Periodi di Divieto
                Dim objPeriodoDivieto As New AgronicaCorePUA_DAL.Pua_Effluente_PeriodoDivieto_R

                Dim xOrderBy As String = "Data_Divieto_A DESC"

                Dim DT_PeriodiDivieto As DataTable = objPeriodoDivieto.Leggi(regolamentocod, puacod, 0, 0, "", xOrderBy, objParametri_Server)

                For Each dr As DataRow In DtEffluenti.Rows

                    Effluente = New PUA_Effluente_Aziendale

                    Effluente.Eff_Cod = dr.Item("eff_cod")

                    If Not IsNothing(objEffluentiOutput) Then
                        Dim Fertilizzante As New PUA_Effluente
                        Fertilizzante = objEffluentiOutput.ListaEffluenti.Where(Function(x) x.Eff_Cod = Effluente.Eff_Cod)(0)
                        If Fertilizzante IsNot Nothing Then
                            Effluente.Fer_Cod = Fertilizzante.Fer_Cod
                            Effluente.Fer_Des = Fertilizzante.Fer_Des
                            Effluente.Eff_Des = Fertilizzante.Eff_Des
                            Effluente.Udm_Cod = Fertilizzante.Udm_Cod
                            Effluente.Udm_Sim = Fertilizzante.Udm_Sim
                            Filtro_Fertilizzanti &= Effluente.Fer_Cod & ","
                        End If
                    End If

                    If Not IsDBNull(dr.Item("Carico")) Then
                        Effluente.Carico = dr.Item("Carico")
                    End If
                    If Not IsDBNull(dr.Item("Capacita_stoccaggio")) Then
                        Effluente.Capacita_stoccaggio = dr.Item("Capacita_stoccaggio")
                    End If
                    If Not IsDBNull(dr.Item("Giorni_stoccaggio")) Then
                        Effluente.Giorni_stoccaggio = dr.Item("Giorni_stoccaggio")
                    End If
                    If Not IsDBNull(dr.Item("Riempimento")) Then
                        Effluente.Riempimento = dr.Item("Riempimento")
                    End If
                    If Not IsDBNull(dr.Item("Azoto_Qta")) Then
                        Effluente.Azoto_Qta = dr.Item("Azoto_Qta")
                    End If
                    If Not IsDBNull(dr.Item("Azoto_Titoli")) Then
                        Effluente.Azoto_Titoli = dr.Item("Azoto_Titoli")
                    End If
                    If Not IsDBNull(dr.Item("Tipo_Allevamento")) Then
                        Effluente.Tipo_Allevamento = dr.Item("Tipo_Allevamento")
                    End If
                    If Not IsDBNull(dr.Item("Perc_Zootecnico")) Then
                        Effluente.Perc_Zootecnico = dr.Item("Perc_Zootecnico")
                    End If
                    If Not IsDBNull(dr.Item("Matrice_Prevalente")) Then
                        Effluente.Matrice_Prevalente = dr.Item("Matrice_Prevalente")
                    End If
                    If Not IsDBNull(dr.Item("Flag_ProvenienzaEsterna")) Then
                        Effluente.Flag_ProvenienzaEsterna = dr.Item("Flag_ProvenienzaEsterna")
                    End If

                    'Ora i periodi di divieto vengono salvati nella tabella Pua_Effluente_PeriodiDivieto.
                    'Imposto come perido di divieto nella griglia quello in cui ricade la data dell'operazione,
                    'altrimenti quello più recente

                    If Not IsNothing(DT_PeriodiDivieto) AndAlso DT_PeriodiDivieto.Rows.Count > 0 Then
                        Dim DrPeriodiXPua_Effluente = DT_PeriodiDivieto.Select("ID_Pua_Effluente = " & dr.Item("ID"))

                        If Not IsNothing(DrPeriodiXPua_Effluente) AndAlso DrPeriodiXPua_Effluente.Length > 0 Then

                            Dim SoloData As String = CDate(data).ToString("MM/dd/yyyy")

                            Dim DrPeriodoDataOperazioneVietata = DrPeriodiXPua_Effluente.CopyToDataTable().Select("Data_Divieto_A <= #" & SoloData & "# AND Data_Divieto_DA >= #" & SoloData & "#")

                            If Not IsNothing(DrPeriodoDataOperazioneVietata) AndAlso DrPeriodoDataOperazioneVietata.Length > 0 Then
                                Effluente.Data_Inizio_Divieto = DrPeriodoDataOperazioneVietata(0).Item("Data_Divieto_DA")
                                Effluente.Data_Fine_Divieto = DrPeriodoDataOperazioneVietata(0).Item("Data_Divieto_A")
                            Else
                                Effluente.Data_Inizio_Divieto = DrPeriodiXPua_Effluente(0).Item("Data_Divieto_DA")
                                Effluente.Data_Fine_Divieto = DrPeriodiXPua_Effluente(0).Item("Data_Divieto_A")
                            End If
                        End If
                    End If

                    If Not IsDBNull(dr.Item("Regolamento_Cod")) Then
                        Effluente.Regolamento_Cod = dr.Item("Regolamento_Cod")
                    End If

                    If Not IsDBNull(dr.Item("validita_inizio")) Then
                        Data_Inizio = dr.Item("validita_inizio")
                    End If
                    If Not IsDBNull(dr.Item("validita_fine")) Then
                        Data_Fine = dr.Item("validita_fine")
                    End If

                    ListaEffluenti.Add(Effluente)

                    Udm_Cod_Dichiarata = Effluente.Udm_Cod
                    Udm_Sim_Dichiarata = Effluente.Udm_Sim
                    Udm_Cod_Data = Effluente.Udm_Cod
                    Udm_Sim_Data = Effluente.Udm_Sim
                    Udm_Cod_Data_Stimata = Effluente.Udm_Cod
                    Udm_Sim_Data_Stimata = Effluente.Udm_Sim
                    Udm_Cod_Data_Stimata_mese = Effluente.Udm_Cod
                    Udm_Sim_Data_Stimata_mese = Effluente.Udm_Sim

                    Qta_Dichiarata = Effluente.Carico
                    Qta_Data = 0
                    Qta_Data_Stimata = 0

                    DataGiacenza = CDate(data)

                    Dt_Giacenza_Data = objG.SchedaGiacenzeMagazzino(DataGiacenza,
                                             piva, sacod, fabbricatocod,
                                             FERTILIZZANTI,
                                             Effluente.Fer_Cod, 0, 0, 0, 0, 0,
                                             LOTTO_NONDEFINITO,
                                             False,
                                             "", "", "", "", "", "", "", "", "", "", "",
                                             "", "", objParametri_Server, objParametri_Utenti)



                    If Dt_Giacenza_Data IsNot Nothing AndAlso Dt_Giacenza_Data.Rows.Count > 0 Then
                        Select Case Effluente.Udm_Cod
                            Case enum_UnitaMisura.Quintali
                                Qta_Data = Dt_Giacenza_Data.Rows(0).Item("giacenza") / 100
                            Case enum_UnitaMisura.Metri_Cubi
                                Qta_Data = Dt_Giacenza_Data.Rows(0).Item("giacenza") / 1000
                        End Select
                    End If

                    Qta_Data_Stimata = objApporti.StoccaggioDisponibileAllaDataEffluente(
                                                objParametri_Server,
                                                puacod,
                                                regolamentocod,
                                                Effluente.Eff_Cod,
                                                Data_Inizio,
                                                CDate(data))

                    InserisciRiga_DT_EffluenteMagazzino(DT_Effluente,
                                                        Effluente.Eff_Cod, Effluente.Eff_Des,
                                                        Effluente.Fer_Cod, Effluente.Fer_Des,
                                                        Effluente.Data_Inizio_Divieto, Effluente.Data_Fine_Divieto,
                                                        Qta_Dichiarata, Udm_Cod_Dichiarata, Udm_Sim_Dichiarata,
                                                        DataGiacenza, Qta_Data, Udm_Cod_Data, Udm_Sim_Data,
                                                        Qta_Data_Stimata, Effluente.Udm_Cod, Effluente.Udm_Sim,
                                                        Effluente.Azoto_Titoli, Effluente.Regolamento_Cod,
                                                        IIf(Qta_Data >= 0, True, False),
                                                        IIf(Qta_Data >= 0, "Giacenza Positiva", "Giacenza Negativa"))


                Next

            End If


            For Each dr As DataRow In DT_Effluente.Rows

                Dim DaysInFirstMonth As Integer = Date.DaysInMonth(Data_Inizio.Year, Data_Inizio.Month)
                Dim LastDayInFirtsMonthDate As Date = New Date(Data_Inizio.Year, Data_Inizio.Month, DaysInFirstMonth)

                'comincio i carichi dall'ultimo giorno del mese della data inizio pua
                DataGiacenza = LastDayInFirtsMonthDate

                'aggiungo le righe fino al mese precedente la data in cui sto registrando l'operazione
                Do While DataGiacenza < CDate(data)

                    Qta_Data = 0
                    Udm_Cod_Data = 0
                    Udm_Sim_Data = ""
                    Qta_Data_Stimata = 0
                    Udm_Cod_Data_Stimata = 0
                    Udm_Sim_Data_Stimata = ""
                    Qta_Data_Stimata_mese = 0
                    Udm_Cod_Data_Stimata_mese = 0
                    Udm_Sim_Data_Stimata_mese = ""

                    Dt_Giacenza_Data = objG.SchedaGiacenzeMagazzino(DataGiacenza,
                                         piva, sacod, fabbricatocod,
                                         FERTILIZZANTI,
                                         dr.Item("fer_cod"), 0, 0, 0, 0, 0,
                                         LOTTO_NONDEFINITO,
                                         False,
                                         "", "", "", "", "", "", "", "", "", "", "",
                                         "", "", objParametri_Server, objParametri_Utenti)


                    Udm_Cod_Data = dr.Item("udm_cod_data_stimata")
                    Udm_Sim_Data = dr.Item("udm_sim_data_stimata")

                    Udm_Cod_Data_Stimata_mese = dr.Item("udm_cod_data_stimata")
                    Udm_Sim_Data_Stimata_mese = dr.Item("udm_sim_data_stimata")

                    If Dt_Giacenza_Data IsNot Nothing AndAlso Dt_Giacenza_Data.Rows.Count > 0 Then
                        Select Case Udm_Cod_Data
                            Case enum_UnitaMisura.Quintali
                                Qta_Data = Dt_Giacenza_Data.Rows(0).Item("giacenza") / 100
                            Case enum_UnitaMisura.Metri_Cubi
                                Qta_Data = Dt_Giacenza_Data.Rows(0).Item("giacenza") / 1000
                        End Select
                    End If

                    Dim Capacita As Decimal = 0
                    Dim CapacitaAlTerminePeriodoDivieto As Decimal = 0
                    Dim RicaricaGiornaliera As Decimal = 0
                    Dim NrGiorni As Integer = 0

                    Qta_Data_Stimata = objApporti.StoccaggioDisponibileAllaDataEffluente(
                                            objParametri_Server,
                                            puacod,
                                            regolamentocod,
                                            dr.Item("eff_cod"),
                                            Data_Inizio,
                                            CDate(DataGiacenza),
                                            Capacita, CapacitaAlTerminePeriodoDivieto, RicaricaGiornaliera, NrGiorni)

                    Dim DaysInMonth As Integer = Date.DaysInMonth(DataGiacenza.Year, DataGiacenza.Month)

                    Qta_Data_Stimata_mese = DaysInMonth * RicaricaGiornaliera

                    InserisciRiga_DT_GiacenzeEffluentiMagazzino(DT_Giacenze,
                                                dr.Item("fer_cod"), DataGiacenza,
                                                   Qta_Data, Udm_Cod_Data, Udm_Sim_Data,
                                                        Qta_Data_Stimata, dr.Item("udm_cod_data_stimata"), dr.Item("udm_sim_data_stimata"),
                                                            Qta_Data_Stimata_mese, Udm_Cod_Data_Stimata_mese, Udm_Sim_Data_Stimata_mese,
                                                          If(Qta_Data >= 0, True, False),
                                                          If(Qta_Data >= 0, "Giacenza Positiva", "Giacenza Negativa"))

                    'incremento di un mese
                    DataGiacenza = DateAdd(DateInterval.Month, 1, DataGiacenza)
                    DaysInMonth = Date.DaysInMonth(DataGiacenza.Year, DataGiacenza.Month)
                    DataGiacenza = New Date(DataGiacenza.Year, DataGiacenza.Month, DaysInMonth)

                Loop

            Next

            r.RispostaOK = True

            r.RispostaStringa.KendoGrid = JSON_DataTable_Tabella_Effluenti(DT_Effluente)
            r.RispostaStringa.KendoGridFiglia = JSON_DataTable_Tabella_Effluenti_Giacenze(DT_Giacenze)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r

    End Function


    Private Shared Function JSON_DataTable_Tabella_Effluenti(ByRef DT As DataTable,
                                                               Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("fer_cod", "fer_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("fer_des", "Effluente", "string"))
        l.Add(New ColonneNome("data_inizio_divieto", "Data Inizio Divieto", "date"))
        l.Add(New ColonneNome("data_fine_divieto", "Data Fine Divieto", "date"))
        l.Add(New ColonneNome("udm_cod_dichiarata", "udm_cod_dichiarata", "number") With {._hidden = True})
        l.Add(New ColonneNome("udm_sim_dichiarata", "Unita Misura", "string") With {._cssHeader = "allineadestra", ._css = "allineadestra"})
        l.Add(New ColonneNome("qta_dichiarata", "Giacenza dichiarata", "number") With {._formatNr = "n3", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("udm_cod_data", "udm_cod_data", "number") With {._hidden = True})
        l.Add(New ColonneNome("udm_sim_data", "Unita Misura", "string") With {._cssHeader = "allineadestra", ._css = "allineadestra"})
        l.Add(New ColonneNome("qta_data", "Giacenza alla data della fertilizzazione", "number") With {._formatNr = "n3", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("udm_cod_data_stimata", "udm_cod_data_stimata", "number") With {._hidden = True})
        l.Add(New ColonneNome("udm_sim_data_stimata", "Unita Misura", "string") With {._cssHeader = "allineadestra", ._css = "allineadestra"})
        l.Add(New ColonneNome("qta_data_stimata", "Giacenza Stimata alla data", "number") With {._formatNr = "n3", ._css = "allineadestra", ._cssHeader = "allineadestra"})

        l.Add(New ColonneNome("n_titolo", "n_titolo", "number") With {._hidden = True})
        l.Add(New ColonneNome("pua_regolamento", "pua_regolamento", "number") With {._hidden = True})

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = True

        Dim risp As String = js.JSON_DataTable_Kendo(DT, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu,
                                           stringaKendoRow:=stringaKendoRow)

        Return risp

    End Function

    Private Shared Function JSON_DataTable_Tabella_Effluenti_Giacenze(ByRef DT As DataTable,
                                                               Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("id", "id", "number") With {._hidden = True})
        l.Add(New ColonneNome("data_movimento", "Data", "date"))
        l.Add(New ColonneNome("fer_cod", "fer_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("udm_cod_data", "udm_cod_data", "number") With {._hidden = True})
        l.Add(New ColonneNome("udm_sim_data", "Unita Misura", "string") With {._cssHeader = "allineadestra", ._css = "allineadestra"})
        l.Add(New ColonneNome("qta_data", "Giacenza di Magazzino", "number") With {._formatNr = "n3", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("udm_cod_data_stimata", "udm_cod_data_stimata", "number") With {._hidden = True})
        l.Add(New ColonneNome("udm_sim_data_stimata", "Unita Misura", "string") With {._cssHeader = "allineadestra", ._css = "allineadestra"})
        l.Add(New ColonneNome("qta_data_stimata", "Giacenza Stimata", "number") With {._formatNr = "n3", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("udm_cod_data_stimata_mese", "udm_cod_data_stimata__mese", "number") With {._hidden = True})
        l.Add(New ColonneNome("udm_sim_data_stimata_mese", "Unita Misura", "string") With {._cssHeader = "allineadestra", ._css = "allineadestra"})
        l.Add(New ColonneNome("qta_data_stimata_mese", "Giacenza Mensile Stimata", "number") With {._formatNr = "n3", ._css = "allineadestra", ._cssHeader = "allineadestra"})

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = True

        Dim risp As String = js.JSON_DataTable_Kendo(DT, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu,
                                           stringaKendoRow:=stringaKendoRow)

        Return risp

    End Function

    Private Shared Sub Crea_DT_EffluenteMagazzino(ByRef DT_Effluenti As DataTable)

        DT_Effluenti.Columns.Add(New DataColumn("eff_cod", GetType(Integer)))
        DT_Effluenti.Columns.Add(New DataColumn("eff_des", GetType(String)))
        DT_Effluenti.Columns.Add(New DataColumn("fer_cod", GetType(Integer)))
        DT_Effluenti.Columns.Add(New DataColumn("fer_des", GetType(String)))
        DT_Effluenti.Columns.Add(New DataColumn("qta_dichiarata", GetType(Decimal)))
        DT_Effluenti.Columns.Add(New DataColumn("udm_cod_dichiarata", GetType(Integer)))
        DT_Effluenti.Columns.Add(New DataColumn("udm_sim_dichiarata", GetType(String)))
        DT_Effluenti.Columns.Add(New DataColumn("data_movimento", GetType(Date)))
        DT_Effluenti.Columns.Add(New DataColumn("data_inizio_divieto", GetType(Date)))
        DT_Effluenti.Columns.Add(New DataColumn("data_fine_divieto", GetType(Date)))
        DT_Effluenti.Columns.Add(New DataColumn("qta_data", GetType(Decimal)))
        DT_Effluenti.Columns.Add(New DataColumn("udm_cod_data", GetType(Integer)))
        DT_Effluenti.Columns.Add(New DataColumn("udm_sim_data", GetType(String)))
        DT_Effluenti.Columns.Add(New DataColumn("qta_data_stimata", GetType(Decimal)))
        DT_Effluenti.Columns.Add(New DataColumn("udm_cod_data_stimata", GetType(Integer)))
        DT_Effluenti.Columns.Add(New DataColumn("udm_sim_data_stimata", GetType(String)))

        DT_Effluenti.Columns.Add(New DataColumn("n_titolo", GetType(Decimal)))
        DT_Effluenti.Columns.Add(New DataColumn("pua_regolamento", GetType(Decimal)))

        DT_Effluenti.Columns.Add(New DataColumn("BooleanRisVer", GetType(String)))
        DT_Effluenti.Columns.Add(New DataColumn("RisultatoVerifica", GetType(String)))

    End Sub

    Private Shared Sub Crea_DT_GiacenzeEffluentiMagazzino(ByRef DT_Giacenze_Effluenti As DataTable)

        DT_Giacenze_Effluenti.Columns.Add(New DataColumn("id", GetType(Integer)))
        DT_Giacenze_Effluenti.Columns.Add(New DataColumn("fer_cod", GetType(Integer)))
        DT_Giacenze_Effluenti.Columns.Add(New DataColumn("data_movimento", GetType(Date)))
        DT_Giacenze_Effluenti.Columns.Add(New DataColumn("qta_data", GetType(Decimal)))
        DT_Giacenze_Effluenti.Columns.Add(New DataColumn("udm_cod_data", GetType(Integer)))
        DT_Giacenze_Effluenti.Columns.Add(New DataColumn("udm_sim_data", GetType(String)))
        DT_Giacenze_Effluenti.Columns.Add(New DataColumn("qta_data_stimata", GetType(Decimal)))
        DT_Giacenze_Effluenti.Columns.Add(New DataColumn("udm_cod_data_stimata", GetType(Integer)))
        DT_Giacenze_Effluenti.Columns.Add(New DataColumn("udm_sim_data_stimata", GetType(String)))
        DT_Giacenze_Effluenti.Columns.Add(New DataColumn("qta_data_stimata_mese", GetType(Decimal)))
        DT_Giacenze_Effluenti.Columns.Add(New DataColumn("udm_cod_data_stimata_mese", GetType(Integer)))
        DT_Giacenze_Effluenti.Columns.Add(New DataColumn("udm_sim_data_stimata_mese", GetType(String)))

        DT_Giacenze_Effluenti.Columns.Add(New DataColumn("BooleanRisVer", GetType(String)))
        DT_Giacenze_Effluenti.Columns.Add(New DataColumn("RisultatoVerifica", GetType(String)))

    End Sub

    Private Shared Sub InserisciRiga_DT_EffluenteMagazzino(ByRef DT_Effluenti As DataTable,
                                                           ByVal eff_cod As Integer, ByVal eff_des As String,
                                                           ByVal fer_cod As Integer, ByVal fer_des As String,
                                                           ByVal data_inizio_divieto As Date, ByVal data_fine_divieto As Date,
                                                           ByVal qta_dichiarata As Decimal, ByVal udm_cod_dichiarata As Integer, ByVal udm_sim_dichiarata As String,
                                                           ByVal data_movimento As Date, ByVal qta_data As Decimal, ByVal udm_cod_data As Integer, ByVal udm_sim_data As String,
                                                           ByVal qta_data_stimata As Decimal, ByVal udm_cod_data_stimata As Integer, ByVal udm_sim_data_stimata As String,
                                                           ByVal n_titolo As Decimal, ByVal pua_regolamento As Integer,
                                                           ByVal BooleanRisVer As String, ByVal RisultatoVerifica As String)
        Dim Dr As DataRow
        Dr = DT_Effluenti.NewRow
        Dr.Item("eff_cod") = eff_cod
        Dr.Item("eff_des") = eff_des
        Dr.Item("fer_cod") = fer_cod
        Dr.Item("fer_des") = fer_des
        If CDate(data_inizio_divieto) = AGRODATAINIZIO OrElse CDate(data_inizio_divieto) = AGRODATAFINE Then
            Dr.Item("data_inizio_divieto") = DBNull.Value
        Else
            Dr.Item("data_inizio_divieto") = data_inizio_divieto
        End If
        If CDate(data_fine_divieto) = AGRODATAINIZIO OrElse CDate(data_fine_divieto) = AGRODATAFINE Then
            Dr.Item("data_fine_divieto") = DBNull.Value
        Else
            Dr.Item("data_fine_divieto") = data_fine_divieto
        End If
        Dr.Item("qta_dichiarata") = qta_dichiarata
        Dr.Item("udm_cod_dichiarata") = udm_cod_dichiarata
        Dr.Item("udm_sim_dichiarata") = udm_sim_dichiarata
        Dr.Item("data_movimento") = data_movimento
        Dr.Item("qta_data") = qta_data
        Dr.Item("udm_cod_data") = udm_cod_data
        Dr.Item("udm_sim_data") = udm_sim_data
        Dr.Item("qta_data_stimata") = qta_data_stimata
        Dr.Item("udm_cod_data_stimata") = udm_cod_data_stimata
        Dr.Item("udm_sim_data_stimata") = udm_sim_data_stimata

        Dr.Item("n_titolo") = n_titolo
        Dr.Item("pua_regolamento") = pua_regolamento

        Dr.Item("RisultatoVerifica") = RisultatoVerifica
        Dr.Item("BooleanRisVer") = BooleanRisVer
        DT_Effluenti.Rows.Add(Dr)

    End Sub

    Private Shared Sub InserisciRiga_DT_GiacenzeEffluentiMagazzino(ByRef DT_Giacenze_Effluenti As DataTable,
                                                                        ByVal fer_cod As Integer, ByVal data_movimento As Date,
                                                                             ByVal qta_data As Decimal, ByVal udm_cod_data As Integer, ByVal udm_sim_data As String,
                                                                             ByVal qta_data_stimata As Decimal, ByVal udm_cod_data_stimata As Integer, ByVal udm_sim_data_stimata As String,
                                                                             ByVal qta_data_stimata_mese As Decimal, ByVal udm_cod_data_stimata_mese As Integer, ByVal udm_sim_data_stimata_mese As String,
                                                                                    ByVal BooleanRisVer As String, ByVal RisultatoVerifica As String)
        Dim Dr As DataRow
        Dr = DT_Giacenze_Effluenti.NewRow
        Dr.Item("id") = DT_Giacenze_Effluenti.Rows.Count + 1
        Dr.Item("fer_cod") = fer_cod
        Dr.Item("data_movimento") = data_movimento
        Dr.Item("qta_data") = qta_data
        Dr.Item("udm_cod_data") = udm_cod_data
        Dr.Item("udm_sim_data") = udm_sim_data
        Dr.Item("qta_data_stimata") = qta_data_stimata
        Dr.Item("udm_cod_data_stimata") = udm_cod_data_stimata
        Dr.Item("udm_sim_data_stimata") = udm_sim_data_stimata
        Dr.Item("qta_data_stimata_mese") = qta_data_stimata_mese
        Dr.Item("udm_cod_data_stimata_mese") = udm_cod_data_stimata_mese
        Dr.Item("udm_sim_data_stimata_mese") = udm_sim_data_stimata_mese
        Dr.Item("BooleanRisVer") = BooleanRisVer
        Dr.Item("RisultatoVerifica") = RisultatoVerifica
        DT_Giacenze_Effluenti.Rows.Add(Dr)

    End Sub



    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva(ByVal piva As String, ByVal sacod As Integer, ByVal fabbricatocod As Integer,
                                 ByVal strKendoEffluenti As String, ByVal strKendoGiacenze As String, dati As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim Flag_Connessione, Flag_Transazione As Boolean

        Try

            Utility.VerificaApriTransazione(objParametri_Server, Flag_Connessione, Flag_Transazione)

            Dim Effluenti = JArray.Parse(strKendoEffluenti)
            Dim Giacenze = JArray.Parse(strKendoGiacenze)

            AgronicaCoreUtility.DataOra.JarrayAggiustaDate(Effluenti)
            AgronicaCoreUtility.DataOra.JarrayAggiustaDate(Giacenze)

            ' registro tutti i rilievi consistenze
            For Each effluente In Effluenti

                Dim GiacenzeEffluente = (
                         From g In Giacenze Where CInt(g("fer_cod")) = CInt(effluente("fer_cod"))
                         )

                For Each giacenza In GiacenzeEffluente

                    Dim DataCarico As Date = CDate(giacenza("data_movimento").ToString)
                    Dim FerCod As Integer = CInt(giacenza("fer_cod"))
                    Dim FerDes As String = giacenza("fer_des")
                    Dim qta As Decimal = CDec(giacenza("qta_data_stimata_mese").ToString)
                    Dim udmcod As Integer = CInt(giacenza("udm_cod_data_stimata_mese").ToString)

                    Dim N_Titolo As Decimal = CDec(effluente("n_titolo").ToString)
                    Dim PuaRegolamento As Integer = CInt(effluente("pua_regolamento").ToString)

                    Dim Id_Agenda As Integer = 0

                    'verifico se esiste gia
                    Dim FiltroFert As String = " (Movimenti_Dettagli.elem_cod = 3 and Movimenti_Dettagli.pro_cod=" & FerCod.ToString & ") "
                    Dim objAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                    Dim DtCarico As DataTable
                    DtCarico = objAgenda.CarichiScarichi(LAVCOD_CARICO, piva, sacod, fabbricatocod,
                                                         0, 0, "", DataCarico,
                                                         FiltroFert, "", objParametri_Server)

                    If Not (DtCarico IsNot Nothing AndAlso DtCarico.Rows.Count > 0) Then
                        Dim objAgendaScrivi As New Agenda_Operazione_Helper
                        Dim Agenda As New Operazione_Agenda
                        Agenda = Crea_Carico(DataCarico, FerCod, FerDes, qta, udmcod, N_Titolo, PuaRegolamento, piva, sacod, fabbricatocod)
                        Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)
                    End If

                Next

            Next

            Utility.VerificaChiudiTransazione(objParametri_Server, Flag_Transazione)

            r.RispostaOK = True
            r.RispostaStringa = "Salvataggio avvenuto con successo."

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri_Server, Flag_Transazione)

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        Finally

            Utility.VerificaChiudiConnessione(objParametri_Server, Flag_Connessione)

        End Try

        Return r

    End Function

    Public Shared Function Crea_Carico(ByVal data As Date, ByVal fercod As Integer, ByVal ferdes As String,
                                       ByVal qta As Decimal, ByVal udmcodindicata As Integer,
                                       ByVal N_Titolo As Decimal, ByVal PuaRegolamento As Integer,
                                       ByVal piva As String, ByVal sacod As Integer, ByVal fabbricatocod As Integer
                                       ) As Operazione_Agenda

        Dim Agenda As New Operazione_Agenda
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Destinazione As Movimento_Destinazione
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico

        Dim BaseCode As Integer
        Dim TopCode As Integer

        UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode,
                                                 HttpContext.Current.Session("ASG_ProgressivoGIAS"))

        Dim Id_Agenda As Integer = 0

        '------------------------
        '   AGENDA
        '------------------------
        Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        Agenda.Id_Agenda = Id_Agenda
        Agenda.Data = data
        Agenda.Piva = piva
        Agenda.Sa_Cod = sacod
        Agenda.Lav_Cod = LAVCOD_CARICO
        Agenda.Des_Lib = "Carico Effluenti"

        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode


        '------------------------
        '   MOVIMENTO
        '------------------------
        Agenda.Movimenti = New List(Of Movimento)

        Movimento = New Movimento

        Movimento.Id_Agenda = Id_Agenda
        Movimento.Piva = piva
        Movimento.Sa_Cod = sacod
        Movimento.Data = data
        Movimento.Lav_Cod = LAVCOD_CARICO
        Movimento.Cau_Mov = CAU_CARICO

        Movimento.BaseCode = BaseCode
        Movimento.TopCode = TopCode

        Agenda.Movimenti.Add(Movimento)


        '------------------------
        '   MOVIMENTO DETTAGLIO
        '------------------------
        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        Movimento_Dettaglio = New Movimento_Dettaglio

        Dim Udm_Cod As Integer
        Select Case udmcodindicata
            Case enum_UnitaMisura.Quintali
                Udm_Cod = enum_UnitaMisura.KG
                qta = qta * 100
            Case enum_UnitaMisura.Metri_Cubi
                Udm_Cod = enum_UnitaMisura.Litri
                qta = qta * 1000
        End Select

        Movimento_Dettaglio.Id_Agenda = Id_Agenda
        Movimento_Dettaglio.Piva = piva
        Movimento_Dettaglio.Sa_Cod = sacod
        Movimento_Dettaglio.Data = data
        Movimento_Dettaglio.Lav_Cod = LAVCOD_CARICO
        Movimento_Dettaglio.Cau_Mov = CAU_CARICO
        Movimento_Dettaglio.Mov_Det_Des = ferdes
        Movimento_Dettaglio.Elem_Cod = FERTILIZZANTI
        Movimento_Dettaglio.Pro_Cod = fercod
        Movimento_Dettaglio.Udm_Cod = Udm_Cod
        Movimento_Dettaglio.Qta = qta
        Movimento_Dettaglio.Pendente = enum_Pendenza.AutoProduzione
        Movimento_Dettaglio.Extra_Int = udmcodindicata
        Movimento_Dettaglio.Contabilizzato = NONCONTABILE

        Movimento_Dettaglio.BaseCode = BaseCode
        Movimento_Dettaglio.TopCode = TopCode

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)


        '------------------------
        '   MOVIMENTO DETTAGLIO TECNICO
        '------------------------
        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)

        Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico

        Movimento_Dettaglio_Tecnico.Id_Agenda = Id_Agenda
        Movimento_Dettaglio_Tecnico.Piva = piva
        Movimento_Dettaglio_Tecnico.Sa_Cod = sacod
        Movimento_Dettaglio_Tecnico.Data = data
        Movimento_Dettaglio_Tecnico.N = N_Titolo
        Movimento_Dettaglio_Tecnico.Extra_Int = PuaRegolamento

        Movimento_Dettaglio_Tecnico.BaseCode = BaseCode
        Movimento_Dettaglio_Tecnico.TopCode = TopCode

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)


        '------------------------
        '   MOVIMENTO DESTINAZIONE
        '------------------------
        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

        Movimento_Destinazione = New Movimento_Destinazione

        Movimento_Destinazione.Id_Agenda = Id_Agenda
        Movimento_Destinazione.Piva = piva
        Movimento_Destinazione.Sa_Cod = sacod
        Movimento_Destinazione.Appezza = 0
        Movimento_Destinazione.Id_Destinazione = fabbricatocod
        Movimento_Destinazione.Tipo = MAGAZZINO
        Movimento_Destinazione.Qta = qta
        Movimento_Destinazione.Data = data
        Movimento_Destinazione.BaseCode = BaseCode
        Movimento_Destinazione.TopCode = TopCode

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1).Movimenti_Destinazioni.Add(Movimento_Destinazione)


        Return Agenda

    End Function

End Class

