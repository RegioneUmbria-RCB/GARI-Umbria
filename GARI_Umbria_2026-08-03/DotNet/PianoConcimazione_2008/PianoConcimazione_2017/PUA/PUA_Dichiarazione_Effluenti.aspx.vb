Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class PUA_Dichiarazione_Effluenti
    Inherits System.Web.UI.Page

    Dim objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    'Dim Qs_AnalisiTestataCod As String
    Public Qs_Operazione As String
    Public Qs_Piva As String
    Public Qs_Sa_Cod As Integer
    Public Qs_PUA_Cod As Integer
    Public Qs_Regolamento_Cod As Integer
    Public Qs_Tipo As enum_PUA_Tipo
    Public Qs_Blocco_Flag As Integer
    Dim objPua As AgronicaCoreGestioneRichieste.ParametriPUA
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        AddHandler Master.ImgBtnAnnullaTutto.Click, AddressOf Me.AnnullaTutto
        Master.flag_MostraBtnIndietro = True

        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("Messaggi/AccessoNegato.htm")
        End If

        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Session.Timeout = 180


        '##############################################################
        '#####  Recupero le variabili                        ##########
        '##############################################################

        objPua = New AgronicaCoreGestioneRichieste.ParametriPUA
        objPua.Leggi()


        If Not IsNothing(Request.QueryString("tipo")) Then
            Qs_Tipo = Stringa_Decodifica(Request.QueryString("tipo").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Tipo = objPua.Tipo_Pua
        End If

        Qs_Operazione = Stringa_Decodifica(Request.QueryString("o").ToString,
                            AgroKey_EncoderDecoder,
                            Server)
        hd_Operazione.Value = Qs_Operazione

        'TEMPORANEO

        If Not IsNothing(Request.QueryString("p")) Then
            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)
        Else
            Qs_Piva = objPua.Piva
        End If
        hd_Piva.Value = Qs_Piva

        If Not IsNothing(Request.QueryString("sa_cod")) Then
            Qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("sa_cod").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)
        Else
            Qs_Sa_Cod = objPua.Sa_Cod
        End If
        hd_Sa_Cod.Value = Qs_Sa_Cod

        If Not IsNothing(Request.QueryString("q")) Then
            Qs_PUA_Cod = Stringa_Decodifica(Request.QueryString("q").ToString,
                                            AgroKey_EncoderDecoder,
                                            Server)
        Else
            Qs_PUA_Cod = objPua.PUA_Testata_Cod
        End If
        hd_PUA_Cod.Value = Qs_PUA_Cod

        If Not IsNothing(Request.QueryString("r")) Then
            Qs_Regolamento_Cod = Stringa_Decodifica(Request.QueryString("r").ToString,
                                            AgroKey_EncoderDecoder,
                                            Server)
        Else
            Qs_Regolamento_Cod = objPua.Regolamento_Cod
        End If
        hd_Regolamento_Cod.Value = Qs_Regolamento_Cod

        If Not IsNothing(Request.QueryString("blocco_flag")) Then
            Qs_Blocco_Flag = Stringa_Decodifica(Request.QueryString("blocco_flag").ToString,
                                            AgroKey_EncoderDecoder,
                                            Server)
        Else
            Qs_Blocco_Flag = 0
        End If

        If Qs_Blocco_Flag > 0 Then
            Div_BTNSalva.Visible = False
        End If


        If Qs_Operazione = enum_TipoOperazioneDB.Modifica Then
            Txt_ValiditaInizio.Enabled = False
            Txt_ValiditaFine.Enabled = False
            Txt_Anno.Enabled = False
            ddlRegolamento.Enabled = False
            ddlCentriAziendali.Enabled = False
            ddlMetodo.Enabled = False
        End If


        Dim objUtentiPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim PermessoScrittura = objUtentiPermessi.Controlla_Permessi_Utente(Session("ASG_Utente_Username"), Session("ASG_IdServizio"), enum_Security_Attivita.Gest_PUA_2, enum_Security_Operazione.Modifica, DateTime.Now, "", objParametri_Utenti)
        Dim PermessoLettura = objUtentiPermessi.Controlla_Permessi_Utente(Session("ASG_Utente_Username"), Session("ASG_IdServizio"), enum_Security_Attivita.Gest_PUA_2, enum_Security_Operazione.Lettura, DateTime.Now, "", objParametri_Utenti)

        If PermessoScrittura = False And PermessoLettura = True Then
            Div_BTNSalva.Visible = False
        End If

        If Not Page.IsPostBack Then

            Select Case Qs_Tipo
                Case enum_PianoConcimazione_Tipo.Bilancio
                    Master.Lbl_Titolo.Text = "PUA (Metodo Bilancio)"
                Case Else
                    Master.Lbl_Titolo.Text = "PUA (Metodo MAS)"
            End Select

            If Qs_Piva <> "" Then
                Dim objRS As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Master.LblRag_Soc.Text = objRS.RagSoc_from_Piva(Qs_Piva, objParametri_Server)
            End If

            Txt_Anno.Text = Today.Year

            Dim DataInizio, DataFine As Date
            Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            objImpost.AnnataAgraria(Today, DataInizio, DataFine, objParametri_Utenti)
            Txt_ValiditaInizio.Text = DataInizio
            Txt_ValiditaFine.Text = DataFine

            'caricaRegolamenti(DataInizio, DataFine)
            'impostaRegolamento_Default(DataInizio, DataFine)

            AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(ddlCentriAziendali, True, "Tutti i Centri Aziendali", "0", Qs_Piva, True, 2, "", "", objParametri_Server)



            Select Case Qs_Operazione

                Case enum_TipoOperazioneDB.Scrittura

                    Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R

                    Dim dataMin As Date = AGRODATAINIZIO
                    Dim dataMax As Date = AGRODATAFINE
                    Dim SportelloAperto As Boolean
                    objPratiche.Data_Sportello_Da_Servizio(Qs_Piva, enum_Servizi.PUA, DateTime.Now, SportelloAperto, dataMin, dataMax, objParametri_Server, objParametri_Utenti)

                    If DataFine > dataMax Then
                        DataFine = dataMax
                        Txt_ValiditaFine.Text = DataFine
                    End If

                    If DataInizio < dataMin Then
                        DataInizio = dataMin
                        Txt_ValiditaInizio.Text = DataInizio
                    End If

                    caricaRegolamenti(DataInizio, DataFine)
                    impostaRegolamento_Default(DataInizio, DataFine)

                    ddlRegolamento_SelectedIndexChanged(Me, Nothing)


                    BtnSalva.Visible = False

                Case enum_TipoOperazioneDB.Modifica

                    Dim objTest As New AgronicaCorePUA_DAL.PUA_Testata_R
                    Dim dtTest As DataTable = objTest.Leggi(Qs_Regolamento_Cod, Qs_PUA_Cod, Qs_Piva, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

                    If Not dtTest Is Nothing AndAlso dtTest.Rows.Count > 0 Then

                        Txt_ValiditaInizio.Text = dtTest.Rows(0).Item("Validita_inizio")
                        Txt_ValiditaFine.Text = dtTest.Rows(0).Item("Validita_Fine")
                        Txt_Anno.Text = dtTest.Rows(0).Item("Pua_anno")

                        caricaRegolamenti(dtTest.Rows(0).Item("Validita_inizio"), dtTest.Rows(0).Item("Validita_Fine"))
                        impostaRegolamento_Default(dtTest.Rows(0).Item("Validita_inizio"), dtTest.Rows(0).Item("Validita_Fine"))

                        ddlRegolamento.SelectedIndex = ddlRegolamento.Items.IndexOf(ddlRegolamento.Items.FindByValue(Qs_Regolamento_Cod))

                        'AF 30/11/23 - In modifca la testata è bloccata, non serve sovrascrivere invocare il change
                        'ddlRegolamento_SelectedIndexChanged(Me, Nothing)

                        If Not IsDBNull(dtTest.Rows(0).Item("pua_tipo")) AndAlso CInt(dtTest.Rows(0).Item("pua_tipo")) > 0 Then
                            Select Case CInt(dtTest.Rows(0).Item("pua_tipo"))
                                Case enum_PUA_Tipo.Completo
                                    ddlMetodo.Items.Add(New ListItem("Bilancio", enum_PUA_Tipo.Completo))
                                Case enum_PUA_Tipo.Semplificato
                                    ddlMetodo.Items.Add(New ListItem("Semplificato", enum_PUA_Tipo.Semplificato))
                            End Select
                        End If

                        If Not IsDBNull(dtTest.Rows(0).Item("sa_cod")) AndAlso CInt(dtTest.Rows(0).Item("sa_cod")) > 0 Then
                            ddlCentriAziendali.SelectedIndex = ddlCentriAziendali.Items.IndexOf(ddlCentriAziendali.Items.FindByValue(dtTest.Rows(0).Item("sa_cod")))
                        End If

                        Txt_Note.Text = dtTest.Rows(0).Item("Note")

                        If Not IsDBNull(dtTest.Rows(0).Item("Flag_NonUtilizzo_Fertilizzanti")) AndAlso CInt(dtTest.Rows(0).Item("Flag_NonUtilizzo_Fertilizzanti")) > 0 Then
                            Chk_NonUtilizzo_Fertilizzanti.Checked = True
                        End If

                    End If

            End Select


            Exit Sub

        End If

    End Sub

    Private Shared Function Leggi_Pua_Effluenti(pua_cod As Integer, regolamento_cod As Integer,
                                                        objParametri_Server As AgronicaCoreParametri,
                                                        objParametri_Super_Server As AgronicaCoreParametri) As String

        Dim strEffluenti As String = ""

        Try

            Dim strErr As String = ""

            Dim Perc_Zootecnico_Testata As Decimal = 0

            'Se non c'è il pua cod mi serve solo lo schema della tabella
            Dim objEff As New AgronicaCorePUA_DAL.Pua_Effluente_R
            Dim DtEffluenti As DataTable = objEff.Leggi(regolamento_cod, pua_cod, 0, " azoto_qta >0 " & If(pua_cod = 0, " AND 1=0", ""), "", objParametri_Server)

            Dim objEffDiv As New AgronicaCorePUA_DAL.Pua_Effluente_PeriodoDivieto_R

            Dim objTest As New AgronicaCorePUA_DAL.PUA_Testata_R
            Dim dtTest As DataTable = objTest.Leggi(regolamento_cod, pua_cod, "", AGRODATAINIZIO, AGRODATAFINE, If(pua_cod = 0, " 1=0", ""), "", objParametri_Server)

            If Not dtTest Is Nothing AndAlso dtTest.Rows.Count > 0 Then

                If Not IsDBNull(dtTest.Rows(0).Item("Perc_Zootecnico")) Then
                    Perc_Zootecnico_Testata = dtTest.Rows(0).Item("Perc_Zootecnico")
                End If

            End If

            Dim objEffluentiOutput As AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_output

            If regolamento_cod <> 0 Then

                Dim objPC As New AgronicaCoreWebService.PianoConcimazione_WS

                Dim objEffluentiInput As New AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_input
                objEffluentiInput.Regolamento_Cod = regolamento_cod
                objEffluentiOutput = objPC.Effluenti(objEffluentiInput, objParametri_Server, objParametri_Super_Server)

            End If

            DtEffluenti.Columns.Add(New DataColumn("eff_des", GetType(String)))
            DtEffluenti.Columns.Add(New DataColumn("tipo_eff_cod", GetType(Integer)))
            DtEffluenti.Columns.Add(New DataColumn("tipo_eff_des", GetType(String)))
            DtEffluenti.Columns.Add(New DataColumn("udm_cod", GetType(Integer)))
            DtEffluenti.Columns.Add(New DataColumn("udm_sim", GetType(String)))
            DtEffluenti.Columns.Add(New DataColumn("flag_tipo_allevamento", GetType(Integer)))
            DtEffluenti.Columns.Add(New DataColumn("flag_matrice_prevalente", GetType(Integer)))
            DtEffluenti.Columns.Add(New DataColumn("flag_provenienzaesterna_des", GetType(String)))

            DtEffluenti.Columns.Add(New DataColumn("strPeriodoDivieto", GetType(String)))
            DtEffluenti.Columns.Add(New DataColumn("jSonDivieti", GetType(String)))




            ''sistemo le date
            'If Not DtEffluentiDivieti Is Nothing Then
            '    For Each drDiv As DataRow In DtEffluentiDivieti.Rows
            '        If Not IsDBNull(drDiv.Item("Data_Divieto_Da")) Then
            '            If CDate(drDiv.Item("Data_Divieto_Da")) = AGRODATAINIZIO Or CDate(drDiv.Item("Data_Divieto_Da")) = AGRODATAFINE Then
            '                drDiv.Item("Data_Divieto_Da") = DBNull.Value
            '            End If
            '        End If
            '        If Not IsDBNull(drDiv.Item("Data_Divieto_A")) Then
            '            If CDate(drDiv.Item("Data_Divieto_A")) = AGRODATAINIZIO Or CDate(drDiv.Item("Data_Divieto_A")) = AGRODATAFINE Then
            '                drDiv.Item("Data_Divieto_A") = DBNull.Value
            '            End If
            '        End If
            '    Next
            'End If

            'Dim strEffluentiDivieti As String = JSON_DataTableEffluentiDivieti_Tabella(DtEffluentiDivieti)



            Dim Eff_Cod As Integer
            Dim Eff_Des As String
            Dim Tipo_Eff_Cod As Integer
            Dim Tipo_Eff_Des As String
            Dim Udm_Cod As Integer
            Dim Udm_Sim As String
            Dim Flag_Tipo_Allevamento As Integer
            Dim Flag_Matrice_Prevalente As Integer
            Dim Flag_ProvenienzaEsterna_Des As String

            If Not DtEffluenti Is Nothing Then

                For i = 0 To DtEffluenti.Rows.Count - 1

                    Eff_Cod = DtEffluenti.Rows(i).Item("eff_cod")
                    Eff_Des = ""
                    Tipo_Eff_Cod = 0
                    Tipo_Eff_Des = ""
                    Udm_Cod = 0
                    Udm_Sim = ""
                    Flag_Tipo_Allevamento = 0
                    Flag_Matrice_Prevalente = 0
                    Flag_ProvenienzaEsterna_Des = "No"



                    If Not IsNothing(objEffluentiOutput) Then
                        Dim Effluente As New AgronicaCorePianoConcimazioneBIZ.PUA_Effluente
                        Effluente = objEffluentiOutput.ListaEffluenti.Where(Function(x) x.Eff_Cod = Eff_Cod)(0)
                        If Not Effluente Is Nothing Then
                            Eff_Des = Effluente.Eff_Des
                            Tipo_Eff_Cod = Effluente.Tipo_Eff_Cod
                            Tipo_Eff_Des = Effluente.Tipo_Eff_Des
                            Udm_Cod = Effluente.Udm_Cod
                            Udm_Sim = Effluente.Udm_Sim
                            Flag_Tipo_Allevamento = Effluente.SpecieAllevamento
                            Flag_Matrice_Prevalente = Effluente.MatricePrevalente
                        End If
                    End If



                    ' perc_zootecnico
                    ' se non presenti sull'effluente (versione nuova)
                    ' prendo quelli della testata pua
                    If Not IsNumeric(DtEffluenti.Rows(i).Item("perc_zootecnico")) Then
                        DtEffluenti.Rows(i).Item("perc_zootecnico") = 0
                    End If
                    If DtEffluenti.Rows(i).Item("perc_zootecnico") = 0 And Flag_Matrice_Prevalente = 1 Then
                        DtEffluenti.Rows(i).Item("perc_zootecnico") = Perc_Zootecnico_Testata
                    End If

                    If IsDBNull(DtEffluenti.Rows(i).Item("Flag_ProvenienzaEsterna")) Then
                        DtEffluenti.Rows(i).Item("Flag_ProvenienzaEsterna") = 0
                    End If

                    Flag_ProvenienzaEsterna_Des = If(DtEffluenti.Rows(i).Item("Flag_ProvenienzaEsterna") = 1, "Sì", "No")

                    DtEffluenti.Rows(i).Item("Eff_Des") = Eff_Des
                    DtEffluenti.Rows(i).Item("tipo_eff_cod") = Tipo_Eff_Cod
                    DtEffluenti.Rows(i).Item("tipo_eff_des") = Tipo_Eff_Des
                    DtEffluenti.Rows(i).Item("Udm_Cod") = Udm_Cod
                    DtEffluenti.Rows(i).Item("Udm_Sim") = Udm_Sim
                    DtEffluenti.Rows(i).Item("flag_tipo_allevamento") = Flag_Tipo_Allevamento
                    DtEffluenti.Rows(i).Item("flag_matrice_prevalente") = Flag_Matrice_Prevalente
                    DtEffluenti.Rows(i).Item("flag_provenienzaEsterna_des") = Flag_ProvenienzaEsterna_Des

                    If Not IsDBNull(DtEffluenti.Rows(i).Item("Data_Inizio_Divieto")) Then
                        If CDate(DtEffluenti.Rows(i).Item("Data_Inizio_Divieto")) = AGRODATAINIZIO Or CDate(DtEffluenti.Rows(i).Item("Data_Inizio_Divieto")) = AGRODATAFINE Then
                            DtEffluenti.Rows(i).Item("Data_Inizio_Divieto") = DBNull.Value
                        End If
                    End If
                    If Not IsDBNull(DtEffluenti.Rows(i).Item("Data_Fine_Divieto")) Then
                        If CDate(DtEffluenti.Rows(i).Item("Data_Fine_Divieto")) = AGRODATAINIZIO Or CDate(DtEffluenti.Rows(i).Item("Data_Fine_Divieto")) = AGRODATAFINE Then
                            DtEffluenti.Rows(i).Item("Data_Fine_Divieto") = DBNull.Value
                        End If
                    End If

                    Dim DtEffluenteDivieti As DataTable = objEffDiv.Leggi(regolamento_cod, pua_cod, 0, DtEffluenti.Rows(i).Item("id"), "", "", objParametri_Server)
                    Dim strPeriodoDivieto As String = ""

                    If Not DtEffluenteDivieti Is Nothing AndAlso DtEffluenteDivieti.Rows.Count > 0 Then
                        'If Not DtEffluenteDivieti Is Nothing Then
                        Dim strEffluenteDivieti As String = JSON_DataTableEffluentiDivieti_Tabella(DtEffluenteDivieti)
                        DtEffluenti.Rows(i).Item("jSonDivieti") = strEffluenteDivieti
                        For Each dr As DataRow In DtEffluenteDivieti.Rows
                            Dim strDivietoInizio As String = "..."
                            Dim strDivietoFine As String = "..."
                            If Not IsDBNull(dr.Item("Data_Divieto_DA")) Then
                                If Not (CDate(dr.Item("Data_Divieto_DA")) = AGRODATAINIZIO Or CDate(dr.Item("Data_Divieto_DA")) = AGRODATAFINE) Then
                                    strDivietoInizio = CDate(dr.Item("Data_Divieto_DA"))
                                End If
                            End If
                            If Not IsDBNull(dr.Item("Data_Divieto_A")) Then
                                If Not (CDate(dr.Item("Data_Divieto_A")) = AGRODATAINIZIO Or CDate(dr.Item("Data_Divieto_A")) = AGRODATAFINE) Then
                                    strDivietoFine = CDate(dr.Item("Data_Divieto_A"))
                                End If
                            End If
                            strPeriodoDivieto &= strDivietoInizio & "-" & strDivietoFine & vbCrLf '& "<br>"
                        Next
                    End If
                    'If strPeriodoDivieto <> "" Then
                    '    strPeriodoDivieto = Left(strPeriodoDivieto, strPeriodoDivieto.Length - 4)
                    'End If
                    DtEffluenti.Rows(i).Item("strPeriodoDivieto") = strPeriodoDivieto

                Next

            End If

            strEffluenti = JSON_DataTableEffluenti_Tabella(DtEffluenti)

        Catch ex As Exception

            Return ex.Message

        End Try

        Return strEffluenti

    End Function

    Private Shared Function JSON_DataTableEffluenti_Tabella(ByRef DT_Effluenti As DataTable,
                                                               Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("id", "id", "number") With {._hidden = True})
        l.Add(New ColonneNome("tipo_eff_cod", "tipo_eff_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("tipo_eff_des", "tipo_eff_des", "string") With {._hidden = True})
        l.Add(New ColonneNome("flag_tipo_allevamento", "flag_tipo_allevamento", "number") With {._hidden = True})
        l.Add(New ColonneNome("flag_matrice_prevalente", "flag_matrice_prevalente", "number") With {._hidden = True})
        l.Add(New ColonneNome("perc_zootecnico", "% Zootecnico", "number") With {._cssHeader = "allineadestra", ._css = "allineadestra"})
        l.Add(New ColonneNome("eff_cod", "eff_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("eff_des", "eff_des", "string") With {._hidden = True})
        l.Add(New ColonneNome("udm_cod", "udm_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("udm_sim", "Unita Misura", "string") With {._css = "allineadestra", ._cssHeader = "allineadestra", ._FiltrabileConCheck = True})
        l.Add(New ColonneNome("carico", "Carico", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra", ._Filtrabile = True})
        'l.Add(New ColonneNome("data_inizio_divieto", "Data Inizio Divieto", "date"))
        'l.Add(New ColonneNome("data_fine_divieto", "Data Fine Divieto", "date"))
        l.Add(New ColonneNome("giorni_stoccaggio", "Stoccaggio [gg]", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("capacita_stoccaggio", "Capacità Stoccaggio", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("riempimento", "Riempimento a Fine Divieto", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("azoto_qta", "Azoto [kg]", "number") With {._formatNr = "n2", ._sum = True, ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("azoto_titoli", "Titolo [kg/q]", "number") With {._formatNr = "n3", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("flag_provenienzaesterna", "flag_provenienzaesterna", "number") With {._hidden = True})
        l.Add(New ColonneNome("flag_provenienzaesterna_des", "flag_provenienzaesterna_des", "string") With {._hidden = True})

        l.Add(New ColonneNome("strPeriodoDivieto", "Periodo Divieto", "string"))
        l.Add(New ColonneNome("jSonDivieti", "jSonDivieti", "string") With {._hidden = True})

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = True

        Dim risp As String = js.JSON_DataTable_Kendo(DT_Effluenti, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu,
                                           stringaKendoRow:=stringaKendoRow) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '

        Return risp

    End Function

    Private Shared Function JSON_DataTableEffluentiDivieti_Tabella(ByRef DT_Effluenti As DataTable,
                                                                Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("id_pua_effluente", "id_pua_effluente", "number") With {._hidden = True, ._Editabile = True})
        l.Add(New ColonneNome("id", "id", "number") With {._hidden = True})
        l.Add(New ColonneNome("data_divieto_da", "Data Inizio Divieto", "date"))
        l.Add(New ColonneNome("data_divieto_a", "Data Fine Divieto", "date"))

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = True

        'Dim risp As String = js.JSON_DataTable_Kendo(DT_Effluenti, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu,
        '                                   stringaKendoRow:=stringaKendoRow) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '

        Dim strKendoRow As New StringBuilder
        js.kendo_Rows(DT_Effluenti, l, strKendoRow)
        'risp = strKendoRow.ToString

        Return strKendoRow.ToString

    End Function



    Private Sub AnnullaTutto()

        Dim UrlTarget As String = "..\PianoConcimazione_MenuBS.aspx" &
                                    "?n=" &
                                    "&t=" &
                                    "&p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                                    "&o=" &
                                    "&m=" & Stringa_Codifica(CStr(enum_PUARegolamenti_Tipo.PUA), AgroKey_EncoderDecoder, Server)

        Response.Redirect(UrlTarget)

    End Sub



    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiEffluenti(
        ByVal pua_cod As String,
        ByVal regolamento_cod As String
    ) As RispostaStandard

        Dim r As New RispostaStandard


        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim strEffluenti As String = Leggi_Pua_Effluenti(pua_cod, regolamento_cod, objParametri_Server, objParametri_Super_Server)

            Dim objEffDiv As New AgronicaCorePUA_DAL.Pua_Effluente_PeriodoDivieto_R
            Dim DtEffluentiDivieti As DataTable = objEffDiv.Leggi(regolamento_cod, pua_cod, 0, 0, "", "", objParametri_Server)

            'sistemo le date
            If Not DtEffluentiDivieti Is Nothing Then
                For Each drDiv As DataRow In DtEffluentiDivieti.Rows
                    If Not IsDBNull(drDiv.Item("Data_Divieto_Da")) Then
                        If CDate(drDiv.Item("Data_Divieto_Da")) = AGRODATAINIZIO Or CDate(drDiv.Item("Data_Divieto_Da")) = AGRODATAFINE Then
                            drDiv.Item("Data_Divieto_Da") = DBNull.Value
                        End If
                    End If
                    If Not IsDBNull(drDiv.Item("Data_Divieto_A")) Then
                        If CDate(drDiv.Item("Data_Divieto_A")) = AGRODATAINIZIO Or CDate(drDiv.Item("Data_Divieto_A")) = AGRODATAFINE Then
                            drDiv.Item("Data_Divieto_A") = DBNull.Value
                        End If
                    End If
                Next
            End If

            Dim strEffluentiDivieti As String = JSON_DataTableEffluentiDivieti_Tabella(DtEffluentiDivieti)

            r.RispostaOK = True
            r.RispostaStringa = strEffluenti


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiTipoEff(regolamento_cod As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objPC As New AgronicaCoreWebService.PianoConcimazione_WS

        Dim objEffluentiInput As New AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_input
        objEffluentiInput.Regolamento_Cod = regolamento_cod
        Dim objEffluentiOutput As AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_output = objPC.Effluenti(objEffluentiInput, objParametri_Server, objParametri_Super_Server)

        Dim lista As New List(Of String)
        For Each e As AgronicaCorePianoConcimazioneBIZ.PUA_Effluente In objEffluentiOutput.ListaEffluenti

            Dim strElem As String = "{""tipo_eff_cod"":" & e.Tipo_Eff_Cod & ", ""tipo_eff_des"":""" & e.Tipo_Eff_Des & """ , ""flag_tipo_allevamento"":" & e.SpecieAllevamento & ", ""flag_matrice_prevalente"":" & e.MatricePrevalente & "}"
            If Not lista.Contains(strElem) Then
                lista.Add(strElem)
            End If

        Next

        r.RispostaStringa = "[" & String.Join(",", lista) & "]"
        r.RispostaOK = True

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiEff(regolamento_cod As String, tipo_eff_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Super_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objEffluentiInput As New AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_input
        objEffluentiInput.Regolamento_Cod = regolamento_cod
        'objEffluentiInput.Tipo_Eff_Cod = tipo_eff_cod

        Dim objPC As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objEffluentiOutput As AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_output = objPC.Effluenti(objEffluentiInput, objParametri_Server, objParametri_Super_Server)

        Dim jArrayListaOp As New JArray()
        For Each e As AgronicaCorePianoConcimazioneBIZ.PUA_Effluente In objEffluentiOutput.ListaEffluenti
            If tipo_eff_cod <> 0 Then
                If e.Tipo_Eff_Cod = tipo_eff_cod Then
                    Dim elem As JObject = New JObject(New JProperty("eff_cod", e.Eff_Cod),
                                                      New JProperty("eff_des", e.Eff_Des),
                                                      New JProperty("udm_cod", e.Udm_Cod),
                                                      New JProperty("udm_sim", e.Udm_Sim),
                                                      New JProperty("n", e.N))
                    If Not jArrayListaOp.Contains(elem) Then
                        jArrayListaOp.Add(elem)
                    End If
                End If
            Else
                Dim elem As JObject = New JObject(New JProperty("eff_cod", e.Eff_Cod),
                                  New JProperty("eff_des", e.Eff_Des),
                                  New JProperty("udm_cod", e.Udm_Cod),
                                  New JProperty("udm_sim", e.Udm_Sim),
                                  New JProperty("n", e.N))
                If Not jArrayListaOp.Contains(elem) Then
                    jArrayListaOp.Add(elem)
                End If
            End If

        Next

        r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
        r.RispostaOK = True

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ApriNuovoDdtRicevuto(ByVal piva As String, ByVal regolamento_cod As String, ByVal data As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim parametriAgenda As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010()
        parametriAgenda.PaginaProvenienza = enum_PaginePianoConcimazione_2017.MenuBS_PUA
        parametriAgenda.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_DocumentoContabileGenerico
        parametriAgenda.Operazione = enum_TipoOperazioneDB.Scrittura
        parametriAgenda.Lavorazione = LAVCOD_BOLLA_RICEVUTA
        parametriAgenda.Piva = piva
        parametriAgenda.DataSelezionata = data

        parametriAgenda.QueryStringFiltrino = "&e=" & Stringa_Codifica(CostantiPersonalizzate.FERTILIZZANTI, AgroKey_EncoderDecoder, objParametri_Server) &
                                              "&pr=" & Stringa_Codifica(regolamento_cod & "/2", AgroKey_EncoderDecoder, objParametri_Server) &
                                              "&exit=true"

        r.RispostaOK = True
        r.RispostaStringa = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(
                                                                    Enum_SiteRedirector.Sito_PianoConcimazione_2017, parametriAgenda).Replace("<script language='javascript'>", "").Replace("</script>", "")

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function VerificaDisponibiltaConferimentiEsterni(ByVal piva As String, ByVal regolamento_cod As String, ByVal data_da As String, ByVal data_a As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Utenti) OrElse IsNothing(objParametri_Super_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objEffluentiInput As New AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_input
        objEffluentiInput.Regolamento_Cod = regolamento_cod

        Dim objPC As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objEffluentiOutput As AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_output = objPC.Effluenti(objEffluentiInput, objParametri_Server, objParametri_Super_Server)

        Dim jsonRes As New JArray()

        For Each eff As AgronicaCorePianoConcimazioneBIZ.PUA_Effluente In objEffluentiOutput.ListaEffluenti

            'Dato che bolle e fatture caricano in Kg e L  prendo l'unità di misura radice dell'effluente e cerco quello, poi la qta la divido per 1000 per fare la conversione a m3 o t

            Dim dataInizio As Date = AGRODATAINIZIO
            Dim dataFine As Date = AGRODATAFINE
            If IsDate(data_da) Then
                dataInizio = CDate(data_da)
            End If
            If IsDate(data_a) Then
                dataFine = CDate(data_a)
            End If

            Dim pro_cod As Integer = eff.Fer_Cod
            Dim lavs_cod As Integer() = {LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA}
            Dim udm_cod As Integer = 0

            Select Case eff.Udm_Cod
                Case enum_UnitaMisura.Tonnellate, enum_UnitaMisura.Quintali
                    udm_cod = enum_UnitaMisura.KG
                Case enum_UnitaMisura.Metri_Cubi
                    udm_cod = enum_UnitaMisura.Litri
                Case Else
                    Throw New Exception("unità di misura dell'effluente " & eff.Eff_Des & " non gestita")
            End Select


            Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R
            Dim dtMovimenti As DataTable = objMovimenti.LeggiMovimentixPUA(piva, lavs_cod, FERTILIZZANTI, pro_cod, enum_Agenda_Causali.CARICO,
                                                                           dataInizio, dataFine, udm_cod, "", "", objParametri_Server)

            If dtMovimenti.Rows.Count = 0 Then
                Continue For
            End If

            Dim sumCarico As Decimal = 0
            Dim mediaPesataN As Decimal = 0
            Dim divisoreMediaPesataN As Decimal = 0
            Dim Azoto_Qta As Decimal = 0

            For Each dr As DataRow In dtMovimenti.Rows

                Dim qta As Decimal =0
                Select Case eff.Udm_Cod
                    Case enum_UnitaMisura.Tonnellate, enum_UnitaMisura.Quintali
                        qta = CDec(dr.Item("qta")) / CDec(100)
                    Case enum_UnitaMisura.Metri_Cubi
                        qta = CDec(dr.Item("qta")) / CDec(1000)
                End Select

                Dim N As Decimal = CDec(dr.Item("N"))
                sumCarico += qta
                If N > 0 Then
                    mediaPesataN += (N * qta)
                    divisoreMediaPesataN += qta
                End If
            Next

            If divisoreMediaPesataN <> 0 Then
                mediaPesataN = mediaPesataN / divisoreMediaPesataN
            End If

            Azoto_Qta = sumCarico * mediaPesataN

            'trasformo l'azoto in kh/q (=%) nel caso di carico in m3
            Select Case eff.Udm_Cod
                Case enum_UnitaMisura.Metri_Cubi
                    Azoto_Qta = Azoto_Qta * 10
            End Select

            jsonRes.Add(New JObject(
                                        New JProperty("id", 0),
                                        New JProperty("tipo_eff_cod", eff.Tipo_Eff_Cod),
                                        New JProperty("tipo_eff_des", eff.Tipo_Eff_Des),
                                        New JProperty("flag_tipo_allevamento", eff.SpecieAllevamento),
                                        New JProperty("flag_matrice_prevalente", eff.MatricePrevalente),
                                        New JProperty("perc_zootecnico", 100),
                                        New JProperty("eff_cod", eff.Eff_Cod),
                                        New JProperty("eff_des", eff.Eff_Des),
                                        New JProperty("udm_cod", eff.Udm_Cod),
                                        New JProperty("udm_sim", eff.Udm_Sim),
                                        New JProperty("carico", Math.Round(sumCarico, 2)),
                                        New JProperty("giorni_stoccaggio", Nothing),
                                        New JProperty("capacita_stoccaggio", Nothing),
                                        New JProperty("riempimento", Nothing),
                                        New JProperty("azoto_qta", Math.Round(Azoto_Qta, 2)),
                                        New JProperty("azoto_titoli", Math.Round(mediaPesataN, 2)),
                                        New JProperty("flag_provenienzaesterna", 1),
                                        New JProperty("flag_provenienzaesterna_des", "Sì"),
                                        New JProperty("strPeriodoDivieto", ""),
                                        New JProperty("jSonDivieti", "")
                                     ))

        Next




        r.RispostaOK = True
        r.RispostaStringa = JsonConvert.SerializeObject(jsonRes, Formatting.None)

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva(piva As String, sa_cod As Integer, pua_cod As Integer,
                                 regolamento_cod As Integer, pua_tipo As Integer,
                                 data_da As String, data_a As String, anno As Integer,
                                 note As String, flag_nonutilizzo_fertilizzanti As Boolean,
                                 dati As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim Flag_Connessione, Flag_Transazione As Boolean

        Try

            Utility.VerificaApriTransazione(objParametri_Server, Flag_Connessione, Flag_Transazione)

            'se ho un pua_cod sono in modifica per cui prima elimino gli effluenti già salvati 
            '(questo per eliminare eventuali righe non valorizzate che erano sempre salvate nel pua 1.0)
            Dim objPUA_Test_W As New AgronicaCorePUA_DAL.PUA_Testata_W
            Dim objPUA_Eff_W As New AgronicaCorePUA_DAL.Pua_Effluente_W
            Dim objPUA_EffDiv_W As New AgronicaCorePUA_DAL.Pua_Effluente_PeriodoDivieto_W
            Dim objPUALog As New AgronicaCorePUA_DAL.AgronicaLogPua_W
            Dim bRet As Boolean

            Dim dataInizio As Date = AGRODATAINIZIO
            Dim dataFine As Date = AGRODATAFINE
            If IsDate(data_da) Then
                dataInizio = CDate(data_da)
            End If
            If IsDate(data_a) Then
                dataFine = CDate(data_a)
            End If

            Dim TipoOperazioneT As enum_TipoOperazioneDB
            Dim TipoOperazioneE As enum_TipoOperazioneDB

            Dim HashEffMod As New Hashtable

            Dim DtEffOLD As DataTable

            If IsNumeric(pua_cod) AndAlso CInt(pua_cod) > 0 Then

                'leggo per il log successivo
                Dim objEff As New AgronicaCorePUA_DAL.Pua_Effluente_R
                DtEffOLD = objEff.Leggi(regolamento_cod, pua_cod, 0, "", "", objParametri_Server)

                'aggiorno solo il campo note
                objPUA_Test_W.Aggiorna_Note_e_Flag_NonUtilizzo_Fertilizzanti(piva, pua_cod, regolamento_cod, note, IIf(flag_nonutilizzo_fertilizzanti = True, 1, 0), "", objParametri_Server)

                bRet = objPUA_Eff_W.Cancella(CInt(pua_cod), 0, regolamento_cod, "", objParametri_Server)
                bRet = objPUA_EffDiv_W.Cancella(CInt(pua_cod), regolamento_cod, 0, 0, "", objParametri_Server)


                TipoOperazioneT = enum_TipoOperazioneDB.Modifica

            Else

                bRet = objPUA_Test_W.Scrivi(pua_cod, piva, sa_cod, anno, pua_tipo, 0, 0, 0, 0, note, regolamento_cod, 0, 0, 0, dataFine, dataInizio, IIf(flag_nonutilizzo_fertilizzanti = True, 1, 0), objParametri_Server)

                TipoOperazioneT = enum_TipoOperazioneDB.Scrittura

            End If

            If bRet = True Then
                objPUALog.Scrivi(TipoOperazioneT, "PUA_Testata", pua_cod, regolamento_cod, piva, sa_cod, anno, pua_tipo, Nothing, Nothing, Nothing, Nothing, "", objParametri_Server)
            End If


            If bRet = True Then

                Dim rows = JArray.Parse(dati)

                AgronicaCoreUtility.DataOra.JarrayAggiustaDate(rows)

                For Each row In rows

                    Dim capacita_stoccaggio As Decimal = 0
                    Dim carico As Decimal = 0
                    Dim riempimento As Decimal = 0
                    Dim giorni_stoccaggio As Integer = 0
                    Dim perc_zootecnico As Decimal = 0

                    If Not row("capacita_stoccaggio") Is Nothing AndAlso IsNumeric(row("capacita_stoccaggio")) Then
                        capacita_stoccaggio = row("capacita_stoccaggio")
                    End If
                    If Not row("carico") Is Nothing AndAlso IsNumeric(row("carico")) Then
                        carico = row("carico")
                    End If
                    If Not row("riempimento") Is Nothing AndAlso IsNumeric(row("riempimento")) Then
                        riempimento = row("riempimento")
                    End If
                    If Not row("giorni_stoccaggio") Is Nothing AndAlso IsNumeric(row("giorni_stoccaggio")) Then
                        giorni_stoccaggio = row("giorni_stoccaggio")
                    End If

                    'Dim data_inizio_divieto As Date = AGRODATAINIZIO
                    'Dim data_fine_divieto As Date = AGRODATAFINE

                    'If Not row("data_inizio_divieto") Is Nothing AndAlso IsDate(row("data_inizio_divieto").ToString) Then
                    '    data_inizio_divieto = CDate(row("data_inizio_divieto"))
                    'End If
                    'If Not row("data_fine_divieto") Is Nothing AndAlso IsDate(row("data_fine_divieto").ToString) Then
                    '    data_fine_divieto = CDate(row("data_fine_divieto"))
                    'End If

                    If Not row("flag_tipo_allevamento") Is Nothing AndAlso IsNumeric(row("flag_tipo_allevamento")) AndAlso CInt(row("flag_tipo_allevamento")) = 1 Then
                        perc_zootecnico = 100
                    Else
                        If Not row("perc_zootecnico") Is Nothing AndAlso IsNumeric(row("perc_zootecnico")) Then
                            perc_zootecnico = CDec(row("perc_zootecnico").ToString)
                        End If
                    End If

                    Dim Id As Integer = row("id")

                    Select Case Id
                        Case <= 0
                            TipoOperazioneE = enum_TipoOperazioneDB.Scrittura
                            Id = 0
                        Case Else
                            TipoOperazioneE = enum_TipoOperazioneDB.Modifica
                            HashEffMod.Add(Id, "")
                    End Select

                    bRet = objPUA_Eff_W.Scrivi(Id,
                                               pua_cod, row("eff_cod"),
                                               row("azoto_qta"),
                                               row("azoto_titoli"),
                                               capacita_stoccaggio,
                                               carico,
                                               giorni_stoccaggio,
                                               riempimento,
                                               regolamento_cod,
                                               0,
                                               perc_zootecnico,
                                               0,
                                               row("flag_provenienzaesterna"),
                                               dataFine, dataInizio,
                                               AGRODATAINIZIO, AGRODATAFINE,
                                               objParametri_Server)

                    'divieti
                    If Not row("jSonDivieti") Is Nothing AndAlso row("jSonDivieti") <> "" Then

                        Dim rows_divieti = JArray.Parse(row("jSonDivieti"))

                        AgronicaCoreUtility.DataOra.JarrayAggiustaDate(rows_divieti)

                        For Each row_divieti In rows_divieti

                            Dim data_inizio_divieto As Date = AGRODATAINIZIO
                            Dim data_fine_divieto As Date = AGRODATAFINE

                            If Not row_divieti("data_divieto_da") Is Nothing AndAlso IsDate(row_divieti("data_divieto_da").ToString) Then
                                data_inizio_divieto = CDate(row_divieti("data_divieto_da").ToString)
                            End If
                            If Not row_divieti("data_divieto_a") Is Nothing AndAlso IsDate(row_divieti("data_divieto_a").ToString) Then
                                data_fine_divieto = CDate(row_divieti("data_divieto_a").ToString)
                            End If

                            bRet = objPUA_EffDiv_W.Scrivi(row_divieti("id"),
                                                            pua_cod, regolamento_cod,
                                                            Id,
                                                            data_inizio_divieto, data_fine_divieto,
                                                            dataFine, dataInizio,
                                                            objParametri_Server)
                        Next

                    End If

                    If bRet = True Then
                        objPUALog.Scrivi(TipoOperazioneE, "PUA_Effluente", pua_cod, regolamento_cod, Id, row("eff_cod"), row("azoto_titoli"), row("flag_provenienzaesterna"), Nothing, Nothing, Nothing, Nothing, "", objParametri_Server)
                    End If

                Next

            End If

            'loggo gli effluenti cancellati
            If Not DtEffOLD Is Nothing Then
                For Each drC As DataRow In DtEffOLD.Rows
                    If Not HashEffMod.ContainsKey(drC.Item("id")) Then
                        objPUALog.Scrivi(enum_TipoOperazioneDB.Cancellazione, "PUA_Effluente", pua_cod, regolamento_cod, drC.Item("id"), drC.Item("eff_cod"), drC.Item("azoto_titoli"), drC.Item("flag_provenienzaesterna"), Nothing, Nothing, Nothing, Nothing, "", objParametri_Server)
                    End If
                Next
            End If

            Utility.VerificaChiudiTransazione(objParametri_Server, Flag_Transazione)

            r.RispostaOK = True
            r.RispostaStringa = "Salvataggio avvenuto con successo."
            r.ParametroDue_stringa = pua_cod

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri_Server, Flag_Transazione)

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        Finally

            Utility.VerificaChiudiConnessione(objParametri_Server, Flag_Connessione)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiChimici(
        ByVal piva As String, ByVal data As String
    ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If


        Try




            Dim Dt_Giacenze As DataTable
            Dim objG As New AgronicaCoreStampeDAL.Magazzino
            Dt_Giacenze = objG.SchedaGiacenzeMagazzino(data,
                                             piva,
                                             0,
                                             0,
                                             FERTILIZZANTI,
                                             0,
                                             0,
                                             0, 0, 0, 0,
                                             LOTTO_NONDEFINITO,
                                             False,
                                             "", "", "", "", "", "", "", "", "", "", "",
                                             "Movimenti_dettagli.mat_Cod=0 and Movimenti_dettagli.pro_cod<>0",
                                             "",
                                             objParametri_Server, objParametri_Utenti,
                                                Flag_QtaMaggioreZero:=True
                                                )

            Dt_Giacenze.Columns.Add(New DataColumn("N", GetType(Decimal)))
            Dt_Giacenze.Columns.Add(New DataColumn("N_Tot", GetType(Decimal)))

            Dim strFiltroFerCod As String = ""
            For Each dr In Dt_Giacenze.Rows
                strFiltroFerCod &= " Fertilizzanti.Fer_Cod=" & dr.Item("Pro_Cod").ToString & " OR "
            Next

            If strFiltroFerCod.Length > 0 Then
                strFiltroFerCod = Left(strFiltroFerCod, strFiltroFerCod.Length - 3)
                strFiltroFerCod = "(" & strFiltroFerCod & ")"

                Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.Fertilizzanti_input
                objParametriIngresso.DataInizio = AGRODATAINIZIO
                objParametriIngresso.DataFine = data
                objParametriIngresso.Tipo = 0 '_TipoRichiesto
                objParametriIngresso.IncludiApporti = True
                objParametriIngresso.IncludiTipologia = True
                'escludo gli organici per eliminarli poi
                objParametriIngresso.strFiltro = strFiltroFerCod & " AND Tipologie.TP_COD < 6 "

                Dim objFert_WS As New AgronicaCoreWebService.Fertilizzanti_WS
                Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.Fertilizzanti_output = objFert_WS.Fertilizzanti(objParametriIngresso)

                For Each dr In Dt_Giacenze.Rows
                    If Not IsNothing(objParametriUscita) Then
                        Dim Fertilizzante As New AgronicaCoreMetaSchemaBIZ.Fertilizzante
                        Fertilizzante = objParametriUscita.ListaFertilizzanti.Where(Function(x) x.Codice = dr.item("pro_cod"))(0)
                        If Not Fertilizzante Is Nothing Then
                            dr.item("N") = Fertilizzante.N
                            dr.item("N_Tot") = dr.item("Giacenza") * Fertilizzante.N / 100
                            'escludo gli organici per eliminarli poi
                        Else
                            dr.item("N") = -1
                        End If
                    End If
                Next

            End If

            'elimino gli organici 
            For i = 0 To Dt_Giacenze.Rows.Count - 1
                If Dt_Giacenze.Rows(i).Item("N") = -1 Then
                    Dt_Giacenze.Rows(i).Delete()
                End If
            Next
            Dt_Giacenze.AcceptChanges()

            Dim strEffluenti As String = JSON_DataTableChimici_Tabella(Dt_Giacenze)

            r.RispostaOK = True
            r.RispostaStringa = strEffluenti

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r

    End Function


    Private Shared Function JSON_DataTableChimici_Tabella(ByRef DT_Chimici As DataTable,
                                                               Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("pro_cod", "Pro_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("descrizione_prodotto", "Fertilizzante", "string"))
        l.Add(New ColonneNome("n", "Titolo [%]", "number") With {._formatNr = "n2", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("udm_sim", "Unita Misura", "string") With {._cssHeader = "allineadestra", ._css = "allineadestra"})
        l.Add(New ColonneNome("giacenza", "Giacenza alla data", "number") With {._formatNr = "n3", ._css = "allineadestra", ._cssHeader = "allineadestra"})
        l.Add(New ColonneNome("n_tot", "Azoto [kg]", "number") With {._formatNr = "n2", ._sum = True, ._cssHeader = "allineadestra", ._css = "allineadestra"})

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = True

        Dim risp As String = js.JSON_DataTable_Kendo(DT_Chimici, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu,
                                           stringaKendoRow:=stringaKendoRow)

        Return risp

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function SportelloPUA(
        ByVal Piva As String
    ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If
        Try

            Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R

            Dim dataMin As Date = AGRODATAINIZIO
            Dim dataMax As Date = AGRODATAFINE
            Dim SportelloAperto As Boolean
            objPratiche.Data_Sportello_Da_Servizio(Piva, enum_Servizi.PUA, DateTime.Now, SportelloAperto, dataMin, dataMax, objParametri_Server, objParametri_Utenti)

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

    Private Sub ddlRegolamento_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRegolamento.SelectedIndexChanged

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input
        objParametriIngresso.Tipo = enum_PUARegolamenti_Tipo.PUA
        objParametriIngresso.Regolamento_Cod = ddlRegolamento.SelectedValue

        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.Regolamenti(objParametriIngresso)

        ddlMetodo.Items.Clear()

        Dim gg_i As String
        Dim mm_i As String
        Dim gg_f As String
        Dim mm_f As String

        For i = 0 To objParametriUscita.ListaRegolamenti.Count - 1
            If Not objParametriUscita.ListaRegolamenti(i).ListaParametri Is Nothing AndAlso objParametriUscita.ListaRegolamenti(i).ListaParametri.Count > 0 Then
                For j = 0 To objParametriUscita.ListaRegolamenti(i).ListaParametri.Count - 1
                    Select Case objParametriUscita.ListaRegolamenti(i).ListaParametri(j).Codice
                        Case enum_PUAParametri.Metodo_Completo
                            If objParametriUscita.ListaRegolamenti(i).ListaParametri(j).Valore = "1" Then
                                ddlMetodo.Items.Add(New ListItem("Bilancio", enum_PUA_Tipo.Completo))
                            End If
                        Case enum_PUAParametri.Metodo_Semplificato
                            If objParametriUscita.ListaRegolamenti(i).ListaParametri(j).Valore = "1" Then
                                ddlMetodo.Items.Add(New ListItem("Semplificato", enum_PUA_Tipo.Semplificato))
                            End If
                        Case enum_PUAParametri.Competenza_Inizio_Giorno
                            gg_i = Right("00" & objParametriUscita.ListaRegolamenti(i).ListaParametri(j).Valore, 2)
                        Case enum_PUAParametri.Competenza_Inizio_Mese
                            mm_i = Right("00" & objParametriUscita.ListaRegolamenti(i).ListaParametri(j).Valore, 2)
                        Case enum_PUAParametri.Competenza_Fine_Giorno
                            gg_f = Right("00" & objParametriUscita.ListaRegolamenti(i).ListaParametri(j).Valore, 2)
                        Case enum_PUAParametri.Competenza_Fine_Mese
                            mm_f = Right("00" & objParametriUscita.ListaRegolamenti(i).ListaParametri(j).Valore, 2)
                    End Select
                Next
            End If
        Next

        Dim anno As String = Mid(Txt_ValiditaFine.Text, 7)
        'If gg_i.Length = 2 And mm_i.Length = 2 Then
        '    Txt_ValiditaInizio.Text = gg_i & "/" & mm_i & "/" & anno
        'End If
        If gg_f.Length = 2 And mm_f.Length = 2 Then
            Txt_ValiditaFine.Text = gg_f & "/" & mm_f & "/" & anno
            Txt_ValiditaInizio.Text = CDate(Txt_ValiditaFine.Text).AddYears(-1).AddDays(1)
        End If


    End Sub

    Private Sub Txt_ValiditaInizio_TextChanged(sender As Object, e As EventArgs) Handles Txt_ValiditaInizio.TextChanged
        Dim dataInizio As Date = AGRODATAINIZIO
        Dim dataFine As Date = AGRODATAFINE

        If IsDate(Txt_ValiditaInizio.Text) Then
            dataInizio = CDate(Txt_ValiditaInizio.Text)
        End If

        If IsDate(Txt_ValiditaFine.Text) Then
            dataFine = CDate(Txt_ValiditaFine.Text)
        End If

        caricaRegolamenti(dataInizio, dataFine)
        impostaRegolamento_Default(dataInizio, dataFine)
    End Sub

    Private Sub Txt_ValiditaFine_TextChanged(sender As Object, e As EventArgs) Handles Txt_ValiditaFine.TextChanged
        Dim dataInizio As Date = AGRODATAINIZIO
        Dim dataFine As Date = AGRODATAFINE

        If IsDate(Txt_ValiditaInizio.Text) Then
            dataInizio = CDate(Txt_ValiditaInizio.Text)
        End If

        If IsDate(Txt_ValiditaFine.Text) Then
            dataFine = CDate(Txt_ValiditaFine.Text)
        End If

        caricaRegolamenti(dataInizio, dataFine)
        impostaRegolamento_Default(dataInizio, dataFine)
    End Sub

    Private Function caricaRegolamenti(dataInizio As Date, dataFine As Date)

        AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PUA_Regolamento_WS(ddlRegolamento,
                                                                                       False,
                                                                                       "",
                                                                                       "",
                                                                                       enum_PUARegolamenti_Tipo.PUA,
                                                                                       Qs_Tipo,
                                                                                       " PUA_Regolamenti.regolamento_cod>=78",
                                                                                       " Ordine desc ",
                                                                                        dataInizio, dataFine)

    End Function


    Private Function impostaRegolamento_Default(dataInizio As Date, dataFine As Date)
        Select Case Qs_Operazione
            Case enum_TipoOperazioneDB.Scrittura

                Dim Regolamento_Def As String = ""

                Regolamento_Def = getRegolamento_Imprese_Codici(dataInizio, dataFine)

                If Regolamento_Def = "" Then

                    Regolamento_Def = getRegolamento_ImpostazioneUtente(dataInizio, dataFine)
                End If

                If IsNumeric(Regolamento_Def) Then
                    ddlRegolamento.SelectedIndex = ddlRegolamento.Items.IndexOf(ddlRegolamento.Items.FindByValue(Regolamento_Def))
                End If

        End Select
    End Function

    Private Function getRegolamento_ImpostazioneUtente(DataInizio As Date, DataFine As Date) As String
        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim disciplinare = objImpost.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_DPI_PREDEFINITO, objParametri_Utenti, 1)
        Dim a As String = ""
        If disciplinare <> "" Then
            a = getRegolamento_da_Disciplinare(disciplinare, DataInizio, DataFine)
        End If
        Return a
    End Function

    Private Function getRegolamento_Imprese_Codici(DataInizio As Date, DataFine As Date) As String
        Dim objImprese_Codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim a As String = ""
        Dim disciplinare = objImprese_Codici.Leggi_Codice_from_Imprese_Codici(Qs_Piva, enum_CodiciAnagrafe.Disciplinare_Aziendale_Default, objParametri_Server)
        If disciplinare <> "" Then
            a = getRegolamento_da_Disciplinare(disciplinare, DataInizio, DataFine)
        End If
        Return a
    End Function

    Private Function getRegolamento_da_Disciplinare(disciplinare As String, DataInizio As Date, DataFine As Date) As String
        Dim x As New AgronicaCoreDpiBIZ.CaricaListControl
        Dim regolamento_cod As String = ""
        If disciplinare.Contains("e:") Then
            'nuovo metodo
            If disciplinare.Split("/")(1).Split(":")(1) = "1" Then
                regolamento_cod = AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PUA_Regolamento_Da_Ente(disciplinare.Split("/")(0).Split(":")(1),
                                                                                                enum_PUARegolamenti_Tipo.PUA,
                                                                                                Qs_Tipo,
                                                                                                "",
                                                                                                " Ordine desc ",
                                                                                                DataInizio, DataFine)
            End If

        Else

            Dim ente_cod As String = ""
            Dim fp As String = ""

            x.Trova_Ente_Disciplinare(disciplinare, False, "", "",
                                           Session, objParametri_Server, objParametri_Utenti,
                                           0, 0, 0, 0, True, True, False,
                                           New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = True, .Flag_DisciplinareAttivo = True},
                                           ente_cod,
                                           fp)

            If ente_cod <> "" AndAlso fp = "1" Then
                regolamento_cod = AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PUA_Regolamento_Da_Ente(ente_cod,
                                                                                                enum_PUARegolamenti_Tipo.PUA,
                                                                                                Qs_Tipo,
                                                                                                "",
                                                                                                " Ordine desc ",
                                                                                                DataInizio, DataFine)
            End If
        End If

        Return regolamento_cod

    End Function

End Class

