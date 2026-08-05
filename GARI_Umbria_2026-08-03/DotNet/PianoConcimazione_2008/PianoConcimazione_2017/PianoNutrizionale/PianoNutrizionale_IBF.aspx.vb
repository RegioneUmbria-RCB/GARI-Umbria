Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports System.Web.Services
Imports Newtonsoft.Json.Linq
Imports AgronicaConversioneCartografiaGias.Agronica
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports Newtonsoft.Json
Imports AgronicaCorePianoConcimazioneBIZ

Public Class PianoNutrizionale_IBF
    Inherits System.Web.UI.Page

    Dim objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    'Dim Qs_AnalisiTestataCod As String
    Public Qs_Operazione As String
    Public Qs_Piva As String
    Public Qs_PCTestataCod As String
    'Dim Qs_RegolamentoCod As enum_PUARegolamenti
    Public Qs_Tipo As enum_PianoConcimazione_Tipo
    Public Qs_Blocco_Flag As Integer
    Dim objConcimazione As AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
    Dim objAnalisiModelloUtils As AgronicaCoreAnagrafeBIZ.Analisi_Modello_Utility
    Public Master_Concimaz As MasterConcimazione

    Private Enum TipoColtura
        Coltura_Principale = 0
        Precessione = 1
    End Enum

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub



    Private Function LeggiResa(ByVal Regolamento_Cod As Integer, ByVal Veg_Cod As Integer, ByVal Grfi_Cod As Integer, ByVal Stato_Cod As Integer,
                               ByRef Mas As Decimal, ByRef N_Fattore_Correttivo_Resa As Decimal, ByRef Mas_Dir_Nitrati As Decimal) As Decimal

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
        If objParametriUscitaMAS.N_Dir_Nitrati <> -1 Then
            Mas_Dir_Nitrati = objParametriUscitaMAS.N_Dir_Nitrati
        End If

        Return Resa

    End Function


    Private Sub Analisi_Valorizza(ByVal Sabbia As String, ByVal Limo As String, ByVal Argilla As String,
                               ByVal Ph As String, ByVal CalcTot As String, ByVal CalcAtt As String, ByVal SO As String,
                               ByVal N As String, ByVal P2O5 As String, ByVal K2O As String, ByVal CN As String, ByVal Mg As String, ByVal CSC As String,
                                  ByVal P As String, ByVal K As String, ByVal Flag_P As String, ByVal Flag_K As String)

        Txt_Sabbia.Text = Sabbia
        Txt_Limo.Text = Limo
        Txt_Argilla.Text = Argilla
        Txt_PH.Text = Ph
        Txt_CalcTot.Text = CalcTot
        Txt_CalcAtt.Text = CalcAtt
        Txt_SO.Text = SO
        Txt_N.Text = N
        Txt_P.Text = P2O5
        Txt_K.Text = K2O
        Txt_CN.Text = CN
        Txt_Mg.Text = Mg
        Txt_CSC.Text = CSC

        DDL_P2O5.SelectedIndex = DDL_P2O5.Items.IndexOf(DDL_P2O5.Items.FindByValue(Flag_P))
        If Flag_P = "1" Then
            Txt_P.Text = P
        End If

        DDL_K2O.SelectedIndex = DDL_K2O.Items.IndexOf(DDL_K2O.Items.FindByValue(Flag_K))
        If Flag_K = "1" Then
            Txt_K.Text = K
        End If

    End Sub

    Private Sub SettaImpianti()

        For Each row In GridView_Impianti.Rows
            CType(row.FindControl("ChkSelezionaImpianto"), CheckBox).Checked = False
        Next

        'verifco se è stata selezionata un'analisi
        'se l'analisi è legata a azienda,centro,appezzamento/impianto/particella
        'in tal caso preseleziono gli impianti
        If ddlAnalisi.SelectedValue <> "0" Then

            Dim objAnalisiE As New AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_R
            Dim DtAnalisiE As New DataTable
            DtAnalisiE = objAnalisiE.Leggi(CInt(ddlAnalisi.SelectedValue),
                                           0, "", 0, 0, 0, 0, 0, "", "", "", 0, 0, "", "",
                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           "", "", objParametri_Server)
            For Each da As DataRow In DtAnalisiE.Rows
                Select Case da("Analisi_Entita_Cod")
                    Case enum_Entita_Analisi.Impresa
                        For Each row In GridView_Impianti.Rows
                            If row.cells(1).text = da("piva") Then
                                CType(row.FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True
                            End If
                        Next
                    Case enum_Entita_Analisi.Centro
                        For Each row In GridView_Impianti.Rows
                            If row.cells(1).text = da("piva") And row.cells(2).text = da("sa_cod") Then
                                CType(row.FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True
                            End If
                        Next
                    Case enum_Entita_Analisi.Campo
                        For Each row In GridView_Impianti.Rows
                            If row.cells(1).text = da("piva") And row.cells(2).text = da("sa_cod") And row.cells(3).text = da("campo_cod") Then
                                CType(row.FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True
                            End If
                        Next
                    Case enum_Entita_Analisi.Appezzamento
                        For Each row In GridView_Impianti.Rows
                            If row.cells(1).text = da("piva") And row.cells(2).text = da("sa_cod") And row.cells(4).text = da("appezza") Then
                                CType(row.FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True
                            End If
                        Next
                    Case enum_Entita_Analisi.Impianto
                        For Each row In GridView_Impianti.Rows
                            If row.cells(1).text = da("piva") And row.cells(2).text = da("sa_cod") And row.cells(4).text = da("appezza") And row.cells(5).text = da("id_imp") Then
                                CType(row.FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True
                            End If
                        Next
                    Case enum_Entita_Analisi.Particella
                        For Each row In GridView_Impianti.Rows
                            Dim particella As String = da("prov") & ":" & da("com") & ":_" & da("sezione") & ":_" & da("foglio") & ":_" & da("numero") & ":_" & da("subalterno")
                            If InStr(row.cells(14).text, particella) > 0 Then
                                CType(row.FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True
                            End If
                        Next
                End Select
            Next

        End If

    End Sub

    Private Function ImpostaFattoriSelezionati(ByVal gv As GridView, ByVal IdCheck As String, ByRef Dt_Sel As DataTable) As Decimal

        Dim TotValori As Decimal = 0

        Dim i, j As Integer

        For i = 0 To Dt_Sel.Rows.Count - 1

            For j = 0 To gv.Rows.Count - 1

                If gv.DataKeys(j).Item("Fattore_Cod") = Dt_Sel.Rows(i).Item("Fattore_Cod") Then
                    CType(gv.Rows(j).FindControl(IdCheck), CheckBox).Checked = True
                    TotValori += gv.DataKeys(j).Item("Valore")
                    Exit For

                End If
            Next
        Next

        Return TotValori

    End Function

    Private Sub ImpostaFattoriSelezionati(ByRef ddl As DropDownList, ByRef txt As TextBox, ByRef Dt_Sel As DataTable)

        Dim i, j As Integer

        For i = 0 To Dt_Sel.Rows.Count - 1

            For j = 0 To ddl.Items.Count - 1

                If ddl.Items(j).Value.Split("|")(0) = Dt_Sel.Rows(i).Item("Fattore_Cod") Then

                    ddl.SelectedIndex = j
                    txt.Text = ddl.SelectedValue.Split("|")(1)

                    Exit For
                End If
            Next
        Next

    End Sub

    Private Function ImpostaFattoriDefault(ByVal gv As GridView, ByVal IdCheck As String) As Decimal

        Dim TotValori As Decimal = 0

        Dim j As Integer
        Dim Controllo_Valore As Decimal
        Dim Controllo_Funzione As String

        For j = 0 To gv.Rows.Count - 1

            Controllo_Valore = gv.DataKeys(j).Item("Controllo_Valore")
            Controllo_Funzione = gv.DataKeys(j).Item("Controllo_Funzione")

            Select Case Controllo_Funzione

                Case "Resa_Bassa"
                    If Resa_Bassa(Controllo_Valore) = True Then
                        CType(gv.Rows(j).FindControl(IdCheck), CheckBox).Checked = True
                        TotValori += gv.DataKeys(j).Item("Valore")
                    End If

                Case "Resa_Alta"
                    If Resa_Alta(Controllo_Valore) = True Then
                        CType(gv.Rows(j).FindControl(IdCheck), CheckBox).Checked = True
                        TotValori += gv.DataKeys(j).Item("Valore")
                    End If

                Case "SO_Bassa"
                    If SO_Bassa() = True Then
                        CType(gv.Rows(j).FindControl(IdCheck), CheckBox).Checked = True
                        TotValori += gv.DataKeys(j).Item("Valore")
                    End If

                Case "SO_Elevata"
                    If SO_Elevata() = True Then
                        CType(gv.Rows(j).FindControl(IdCheck), CheckBox).Checked = True
                        TotValori += gv.DataKeys(j).Item("Valore")
                    End If

                Case "SO_MoltoElevata"
                    If SO_MoltoElevata(Controllo_Valore) = True Then
                        CType(gv.Rows(j).FindControl(IdCheck), CheckBox).Checked = True
                        TotValori += gv.DataKeys(j).Item("Valore")
                    End If

                Case "Lisciviazione_Forte"
                    If Lisciviazione_Forte(Controllo_Valore) = True Then
                        CType(gv.Rows(j).FindControl(IdCheck), CheckBox).Checked = True
                        TotValori += gv.DataKeys(j).Item("Valore")
                    End If

                Case "CalcAtt_Elevato"
                    If CalcAtt_Elevato() = True Then
                        CType(gv.Rows(j).FindControl(IdCheck), CheckBox).Checked = True
                        TotValori += gv.DataKeys(j).Item("Valore")
                    End If

            End Select
        Next

        Return TotValori

    End Function

    Public Function Resa_Alta(ByVal Controllo_Valore As Decimal) As Boolean

        If IsNumeric(Txt_Resa.Text) AndAlso IsNumeric(Controllo_Valore) Then
            If CDec(Txt_Resa.Text.Replace(".", ",")) > CDec(Controllo_Valore) Then
                Return True
            End If
        End If

        Return False

    End Function

    Public Function Resa_Bassa(ByVal Controllo_Valore As Decimal) As Boolean

        If IsNumeric(Txt_Resa.Text) AndAlso IsNumeric(Controllo_Valore) Then
            If CDec(Txt_Resa.Text.Replace(".", ",")) < CDec(Controllo_Valore) Then
                Return True
            End If
        End If

        Return False

    End Function

    Public Function Lisciviazione_Forte(ByVal Controllo_Valore As Decimal) As Boolean

        Dim PioggiaTot As Decimal
        Dim PioggiaInv As Decimal = 0
        Dim PioggiaFeb As Decimal = 0

        If IsNumeric(Txt_Pioggia.Text) Then
            PioggiaInv = CDec(Txt_Pioggia.Text.Replace(".", ","))
        End If
        PioggiaTot = PioggiaInv + PioggiaFeb

        If IsNumeric(Controllo_Valore) Then
            If PioggiaTot > CDec(Controllo_Valore) Then
                Return True
            End If
        End If

        Return False

    End Function

    Public Function SO_Elevata() As Boolean

        Dim objParametriIngressoSuolo As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Suolo_input
        objParametriIngressoSuolo.Regolamento_Cod = CInt(ddlRegolamento.SelectedValue)
        objParametriIngressoSuolo.Sabbia = CDec(Txt_Sabbia.Text.Replace(".", ","))
        objParametriIngressoSuolo.Argilla = CDec(Txt_Argilla.Text.Replace(".", ","))
        objParametriIngressoSuolo.SO = CDec(Txt_SO.Text.Replace(".", ","))

        Dim objParametriUscitaSuolo As enum_PianoConcimazione_Dotazione
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscitaSuolo = objPC_WS.So_Dotazione(objParametriIngressoSuolo)

        Select Case objParametriUscitaSuolo
            Case enum_PianoConcimazione_Dotazione.Elevata
                Return True
        End Select

        Return False

    End Function

    Public Function SO_MoltoElevata(ByVal Controllo_Valore As Decimal) As Boolean

        If IsNumeric(Txt_SO.Text) AndAlso IsNumeric(Controllo_Valore) Then
            If CDec(Txt_SO.Text.Replace(".", ",")) > CDec(Controllo_Valore) Then
                Return True
            End If
        End If

        Return False

    End Function

    Public Function SO_Bassa() As Boolean

        Dim objParametriIngressoSuolo As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Suolo_input
        objParametriIngressoSuolo.Regolamento_Cod = CInt(ddlRegolamento.SelectedValue)
        objParametriIngressoSuolo.Sabbia = CDec(Txt_Sabbia.Text.Replace(".", ","))
        objParametriIngressoSuolo.Argilla = CDec(Txt_Argilla.Text.Replace(".", ","))
        objParametriIngressoSuolo.SO = CDec(Txt_SO.Text.Replace(".", ","))

        Dim objParametriUscitaSuolo As enum_PianoConcimazione_Dotazione
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscitaSuolo = objPC_WS.So_Dotazione(objParametriIngressoSuolo)

        Select Case objParametriUscitaSuolo
            Case enum_PianoConcimazione_Dotazione.Bassa, enum_PianoConcimazione_Dotazione.MoltoBassa
                Return True
        End Select

        Return False

    End Function


    Private Sub AnnullaTutto()

        Dim UrlTarget As String

        UrlTarget = "..\PianoConcimazione_MenuBS.aspx" &
                "?n=" &
                "&t=" &
                "&p=" &
                Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) +
                "&o=" &
                "&m=" & Stringa_Codifica(CStr(enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF), AgroKey_EncoderDecoder, Server)

        Response.Redirect(UrlTarget)

    End Sub

    Private Function GeneraStrutturaDTImpianti() As DataTable

        Dim Dt As New DataTable

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("campo_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("appezza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("id_reg", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("progetto_cod", GetType(Integer)))

        Dt.Columns.Add(New DataColumn("veg_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("cul_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("grfi_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("stato_impianto", GetType(Integer)))

        Dt.Columns.Add(New DataColumn("rag_soc", GetType(String)))
        Dt.Columns.Add(New DataColumn("sa_nome", GetType(String)))
        Dt.Columns.Add(New DataColumn("campo_nome", GetType(String)))
        Dt.Columns.Add(New DataColumn("app_nome", GetType(String)))
        Dt.Columns.Add(New DataColumn("catasto", GetType(String)))
        Dt.Columns.Add(New DataColumn("descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("sup_imp", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("validita_inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("validita_fine", GetType(String)))

        Dt.Columns.Add(New DataColumn("QtaMaxN", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("QtaMaxP2O5", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("QtaMaxK2O", GetType(Decimal)))

        Return Dt

    End Function

    Private Sub Salva()

        Dim strErr As String

#Region "Estrazione dati e controlli campi obbligatori"
        '--------------------
        '   TESTATA
        '--------------------
        Dim Regolamento As Integer
        Dim Descrizione As String

        Dim ValiditaInizio As Date = AGRODATAINIZIO
        Dim ValiditaFine As Date = AGRODATAFINE
        Dim Anno As Integer

        Dim Note As String

        Dim Flag_NonUtilizzo_Fertilizzanti As Integer


        Regolamento = CInt(ddlRegolamento.SelectedValue)
        Descrizione = Txt_Descrizione.Text

        Note = Txt_Note.Text
        Flag_NonUtilizzo_Fertilizzanti = IIf(Chk_NonUtilizzo_Fertilizzanti.Checked = True, 1, 0)

        If Txt_ValiditaInizio.Text = "day/month/year" Then
            Txt_ValiditaInizio.Text = ""
        End If
        If Txt_ValiditaInizio.Text <> "" Then
            ValiditaInizio = CDate(Txt_ValiditaInizio.Text)
        Else
            strErr &= Resources.PianoConcimazione_2017.InserireDataInizioPianoNutrizionale & vbCrLf
        End If

        If Txt_ValiditaFine.Text = "day/month/year" Then
            Txt_ValiditaFine.Text = ""
        End If
        If Txt_ValiditaFine.Text <> "" Then
            ValiditaFine = CDate(Txt_ValiditaFine.Text)
        Else
            strErr &= Resources.PianoConcimazione_2017.InserireDataFinePianoNutrizionale & vbCrLf
        End If

        If IsNumeric(Txt_Anno.Text) Then
            Anno = CInt(Txt_Anno.Text)
        Else
            strErr &= Resources.PianoConcimazione_2017.InserireAnnoPianoNutrizionale & vbCrLf
        End If

        If strErr <> "" Then
            Master.HiddenMessaggioErrore = strErr
            Exit Sub
        End If


        '--------------------
        '   DATI COLTURA E UBICAZIONE 
        '--------------------
        Dim Sa_Cod As Integer

        Dim Veg_Cod As Integer
        Dim Grfi_Cod As Integer

        Dim ResaDichiarata As Decimal
        Dim ResaStoricaPrecessione As Decimal

        Dim PeriodoSeminaColturaPrincipale As Integer
        Dim PeriodoRaccoltaColturaPrincipale As Integer


        Sa_Cod = CInt(Cmb_Centro.SelectedValue)
        Veg_Cod = Cmb_Specie_ColturaPrincipale.SelectedValue
        Grfi_Cod = ddlFinalitaRer_ColturaPrincipale.SelectedItem.Value

        If IsNumeric(Txt_Resa.Text) Then
            ResaDichiarata = CDec(Txt_Resa.Text.Replace(".", ","))
        Else
            strErr &= Resources.PianoConcimazione_2017.InserireResa & vbCrLf
        End If

        If IsNumeric(Txt_ResaStoricaPrecessione.Text) Then
            ResaStoricaPrecessione = CDec(Txt_ResaStoricaPrecessione.Text.Replace(".", ","))
        Else
            strErr &= Resources.PianoConcimazione_2017.InserireResaStoricaPrecessione & vbCrLf
        End If

        PeriodoSeminaColturaPrincipale = CInt(cmb_PeriodoSeminaColturaPrincipale.Value)
        PeriodoRaccoltaColturaPrincipale = CInt(cmb_PeriodoRaccoltaColturaPrincipale.Value)

        If PeriodoSeminaColturaPrincipale > PeriodoRaccoltaColturaPrincipale Then
            strErr &= "Il peridio semina non può precedere il periodo raccolta"
        End If

        If strErr <> "" Then
            Master.HiddenMessaggioErrore = strErr
            Exit Sub
        End If


        '--------------------
        '   CARATTERISTICHE SUOLO
        '--------------------
        Dim Sabbia As Decimal = 0
        Dim Limo As Decimal = 0
        Dim Argilla As Decimal = 0
        Dim Ph As Decimal = 0
        Dim CalcTot As Decimal = 0
        Dim CalcAtt As Decimal = 0
        Dim N As Decimal = 0
        Dim SO As Decimal = 0
        Dim CN As Decimal = 0
        Dim P2O5 As Decimal = 0
        Dim K2O As Decimal = 0
        Dim P As Decimal = 0
        Dim K As Decimal = 0
        Dim PC_Dettagli_Flag_P As Integer = 0
        Dim PC_Dettagli_Flag_K As Integer = 0
        Dim Mg As Decimal = 0
        Dim CSC As Decimal = 0

        If IsNumeric(Txt_Sabbia.Text) Then
            Sabbia = CDec(Txt_Sabbia.Text.Replace(".", ","))
        Else
            strErr &= Resources.PianoConcimazione_2017.InserireSabbia & vbCrLf
        End If

        If IsNumeric(Txt_Limo.Text) Then
            Limo = CDec(Txt_Limo.Text.Replace(".", ","))
        Else
            strErr &= Resources.PianoConcimazione_2017.InserireLimo & vbCrLf
        End If

        If IsNumeric(Txt_Argilla.Text) Then
            Argilla = CDec(Txt_Argilla.Text.Replace(".", ","))
        Else
            strErr &= Resources.PianoConcimazione_2017.InserireArgilla & vbCrLf
        End If

        If IsNumeric(Txt_PH.Text) Then
            Ph = CDec(Txt_PH.Text.Replace(".", ","))
        Else
            strErr &= Resources.PianoConcimazione_2017.InserirepH & vbCrLf
        End If

        If IsNumeric(Txt_CalcTot.Text) Then
            CalcTot = CDec(Txt_CalcTot.Text.Replace(".", ","))
        Else
            strErr &= Resources.PianoConcimazione_2017.InserireCalcareTotale & vbCrLf
        End If

        If IsNumeric(Txt_CalcAtt.Text) Then
            CalcAtt = CDec(Txt_CalcAtt.Text.Replace(".", ","))
        Else
            strErr &= Resources.PianoConcimazione_2017.InserireCalcareAttivo & vbCrLf
        End If

        If IsNumeric(Txt_N.Text) Then
            N = CDec(Txt_N.Text.Replace(".", ","))
        Else
            strErr &= Resources.PianoConcimazione_2017.InserireN & vbCrLf
        End If

        If IsNumeric(Txt_SO.Text) Then
            SO = CDec(Txt_SO.Text.Replace(".", ","))
        Else
            strErr &= Resources.PianoConcimazione_2017.InserireSO & vbCrLf
        End If

        If IsNumeric(Txt_CN.Text) Then
            CN = CDec(Txt_CN.Text.Replace(".", ","))
        Else
            strErr &= Resources.PianoConcimazione_2017.InserireCN & vbCrLf
        End If

        Select Case DDL_P2O5.SelectedValue
            Case "0"
                PC_Dettagli_Flag_P = 0
                If IsNumeric(Txt_P.Text) Then
                    P2O5 = CDec(Txt_P.Text.Replace(".", ","))
                    P = CDec(Txt_P.Text.Replace(".", ",")) * Fattore_Conversione_P2O5_P
                Else
                    strErr &= Resources.PianoConcimazione_2017.InserireP2O5 & vbCrLf
                End If
            Case Else
                PC_Dettagli_Flag_P = 1
                If IsNumeric(Txt_P.Text) Then
                    P2O5 = CDec(Txt_P.Text.Replace(".", ",")) * Fattore_Conversione_P_P2O5
                    P = CDec(Txt_P.Text.Replace(".", ","))
                Else
                    strErr &= Resources.PianoConcimazione_2017.InserireP & vbCrLf
                End If
        End Select

        Select Case DDL_K2O.SelectedValue
            Case "0"
                PC_Dettagli_Flag_K = 0
                If IsNumeric(Txt_K.Text) Then
                    K2O = CDec(Txt_K.Text.Replace(".", ","))
                    K = CDec(Txt_K.Text.Replace(".", ",")) * Fattore_Conversione_K2O_K
                Else
                    strErr &= Resources.PianoConcimazione_2017.InserireK2O & vbCrLf
                End If
            Case Else
                PC_Dettagli_Flag_K = 1
                If IsNumeric(Txt_K.Text) Then
                    K2O = CDec(Txt_K.Text.Replace(".", ",")) * Fattore_Conversione_K_K2O
                    K = CDec(Txt_K.Text.Replace(".", ","))
                Else
                    strErr &= Resources.PianoConcimazione_2017.InserireK & vbCrLf
                End If
        End Select

        If IsNumeric(Txt_Mg.Text) Then
            Mg = CDec(Txt_Mg.Text.Replace(".", ","))
        Else
            strErr &= Resources.PianoConcimazione_2017.InserireMg & vbCrLf
        End If

        If IsNumeric(Txt_CSC.Text) Then
            CSC = CDec(Txt_CSC.Text.Replace(".", ","))
        Else
            strErr &= Resources.PianoConcimazione_2017.InserireCSC & vbCrLf
        End If

        'x bilancio campi suolo obbligatori
        Select Case Qs_Tipo
            Case enum_PianoConcimazione_Tipo.Bilancio
                If strErr <> "" Then
                    Master.HiddenMessaggioErrore = strErr
                    Exit Sub
                End If
        End Select


        '--------------------
        '   PRATICHE AGRONOMICHE
        '--------------------
        Dim Precessione As Integer
        Dim Grfi_Cod_Precessione As Integer
        Dim Flag_ResiduiPrecessione As Integer
        Dim PeriodoInterramentoResiduiPrecessione As Integer

        Dim TipoFertilizzante As Integer
        Dim Frequenza As Integer
        Dim QtaN_FerPrec As Decimal

        Precessione = Cmb_Specie_Precessione.SelectedItem.Value
        Grfi_Cod_Precessione = ddlFinalitaRer_Precessione.SelectedItem.Value
        Flag_ResiduiPrecessione = IIf(Chk_ResiduiPrecessione.Checked = True, 1, 0)
        PeriodoInterramentoResiduiPrecessione = CInt(cmb_PeriodoInterramentoResiduiPrecessione.Value)

        TipoFertilizzante = ddlConcimeOrganico.SelectedItem.Value

        If IsNumeric(Txt_QtaN_KGHa.Text) Then
            QtaN_FerPrec = CDec(Txt_QtaN_KGHa.Text.Replace(".", ","))
        End If


        '--------------------
        '   METEO
        '--------------------
        Dim Piovosita As Decimal = 0
        Dim AvgTemperatura_ColturaInCampo As Decimal = 0
        Dim AvgTemperatura_MeseSemina_Febbraio As Decimal = 0
        Dim PercUmiditaColturaPrincipale As Decimal = 0
        Dim PercUmiditaRaccoltaPrecessione As Decimal = 0

        If IsNumeric(Txt_Pioggia.Text) Then
            Piovosita = CDec(Txt_Pioggia.Text.Replace(".", ","))
        End If
        If IsNumeric(Txt_AvgTemperatura_ColturaInCampo.Text) Then
            AvgTemperatura_ColturaInCampo = CDec(Txt_AvgTemperatura_ColturaInCampo.Text.Replace(".", ","))
        End If
        If IsNumeric(Txt_AvgTemperatura_MeseSemina_Febbraio.Text) Then
            AvgTemperatura_MeseSemina_Febbraio = CDec(Txt_AvgTemperatura_MeseSemina_Febbraio.Text.Replace(".", ","))
        End If
        If IsNumeric(Txt_PercUmiditaColturaPrincipale.Text) Then
            PercUmiditaColturaPrincipale = CDec(Txt_PercUmiditaColturaPrincipale.Text.Replace(".", ","))
        End If
        If IsNumeric(Txt_PercUmiditaRaccoltaPrecessione.Text) Then
            PercUmiditaRaccoltaPrecessione = CDec(Txt_PercUmiditaRaccoltaPrecessione.Text.Replace(".", ","))
        End If
#End Region


        Dim strErrore As String = Nothing
        Dim objLog As AgronicaCoreDataProvider.LogProvider
        Dim objSequenze As AgronicaCoreDataProvider.Agro_Sequenze

        Dim objPC_Testata As AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Testata_W
        Dim objPC_Dettagli As AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_W
        Dim objPC_Dettagli_R As AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_R
        Dim objPC_FattoriCorrettivi As AgronicaCorePianoConcimazioneDAL.PianoConcimazione_FattoriCorrettivi_W


        Dim BaseCode As Integer
        Dim TopCode As Integer

        Dim PC_Testata_Cod As Integer
        Dim PC_Dettagli_Cod As Integer

        Dim Allegati_Documenti_Cod As Integer = 0


        Try

            Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))

            ' istanzio le classi
            'objConnessione = New AgronicaCoreDataProvider.ConnessioniTransazioni
            objLog = New AgronicaCoreDataProvider.LogProvider

            ' apro la connessione e la transazione (primo parametri dell'interfaccia)
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            Dim bRet As Boolean

            objPC_Testata = New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Testata_W
            objPC_Dettagli = New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_W
            objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

            ' TESTATA PIANO

            If Qs_Operazione = enum_TipoOperazioneDB.Scrittura AndAlso Testata_Cod.Value = "0" Then
                '------------------
                '   TESTATA
                '------------------
                PC_Testata_Cod = objSequenze.NuovoId_Tabella("PianoConcimazione_Testata",
                                                             BaseCode,
                                                             TopCode,
                                                             objParametri_Server)

                Testata_Cod.Value = PC_Testata_Cod

                bRet = objPC_Testata.Scrivi(PC_Testata_Cod,
                                            Descrizione,
                                            Regolamento,
                                            Qs_Tipo,
                                            0,
                                            ValiditaInizio,
                                            ValiditaFine,
                                            Note,
                                            Flag_NonUtilizzo_Fertilizzanti,
                                            objParametri_Server)

                If Not bRet Then
                    objLog.Scrivi_LOG(objParametri_Server, "PCB_Inserimento.Salva", "Errore salvataggio testata")
                End If

                '------------------
                '   DETTAGLI
                '------------------
                PC_Dettagli_Cod = objSequenze.NuovoId_Tabella("PianoConcimazione_Dettagli",
                                                              BaseCode,
                                                              TopCode,
                                                              objParametri_Server)

            Else

                '------------------
                '   TESTATA
                '------------------
                PC_Testata_Cod = Testata_Cod.Value

                ' possono cambiare solo la descrizione, le date di validità, note e check dichiarazioni non utilizzo
                bRet = objPC_Testata.Modifica(PC_Testata_Cod,
                                              Descrizione,
                                              Regolamento,
                                              Qs_Tipo,
                                              ValiditaInizio,
                                              ValiditaFine,
                                              Note,
                                              Flag_NonUtilizzo_Fertilizzanti,
                                              "",
                                              objParametri_Server)


                '------------------
                '   DETTAGLI
                '------------------
                objPC_Dettagli_R = New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_R

                Dim DtDettagli As DataTable
                DtDettagli = objPC_Dettagli_R.Leggi(PC_Testata_Cod,
                                                    "", "",
                                                    objParametri_Server)

                PC_Dettagli_Cod = 0
                If Not IsNothing(DtDettagli) Then
                    If DtDettagli.Rows.Count > 0 Then
                        PC_Dettagli_Cod = DtDettagli.Rows(0).Item("PC_Dettagli_Cod")
                        '(04/09/2019 fede) recupero il codice allegato per eliminarlo
                        If IsNumeric(DtDettagli.Rows(0).Item("Allegati_Documenti_cod")) Then
                            Allegati_Documenti_Cod = DtDettagli.Rows(0).Item("Allegati_Documenti_cod")
                        End If
                    End If
                End If

                If PC_Dettagli_Cod <> 0 Then
                    objPC_Dettagli.Cancella(PC_Testata_Cod,
                                            PC_Dettagli_Cod,
                                            Qs_Piva,
                                            "",
                                            objParametri_Server)
                End If
                '(04/09/2019 fede) elimino l'allegato (dovrà essere rigenerato)
                If Allegati_Documenti_Cod <> 0 Then
                    Dim objAllegatiDocumenti As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
                    objAllegatiDocumenti.Cancella(Allegati_Documenti_Cod, "", objParametri_Server)
                    Dim objAlertEntita As New AgronicaCoreScadenziario.Alert_Entita_W
                    objAlertEntita.Cancella_xDocumento(Allegati_Documenti_Cod, objParametri_Server)
                End If
            End If


            '--------------------
            '   NPK MAX
            '--------------------
            Dim QtaMaxN As Decimal = 0
            Dim QtaMaxP As Decimal = 0
            Dim QtaMaxk As Decimal = 0

            If IsNumeric(N_Ammesso.Value) Then
                QtaMaxN = CDec(N_Ammesso.Value)
            End If
            If IsNumeric(P_Ammesso.Value) Then
                QtaMaxP = CDec(P_Ammesso.Value)
            End If
            If IsNumeric(K_Ammesso.Value) Then
                QtaMaxk = CDec(K_Ammesso.Value)
            End If

            Dim Analisi_Cod As Integer = CInt(ddlAnalisi.SelectedValue)

            Dim N_MAS As Decimal = -1
            Dim N_Mas_Regolamento_Tipo As Integer = 0

            Select Case Qs_Tipo
                Case enum_PianoConcimazione_Tipo.Bilancio

                    If IsNumeric(ddlMasB.SelectedItem.Value) Then
                        N_MAS = CDec(ddlMasB.SelectedItem.Value)
                    End If

                    Select Case ddlMasB.SelectedIndex
                        Case 0
                            N_Mas_Regolamento_Tipo = enum_PUARegolamenti_Tipo.PianoComcimazione
                        Case Else
                            N_Mas_Regolamento_Tipo = enum_PUARegolamenti_Tipo.PUA
                    End Select

                Case Else

                    If IsNumeric(ddlMasS.SelectedItem.Value) Then
                        N_MAS = CDec(ddlMasS.SelectedItem.Value)
                    End If

                    Select Case ddlMasS.SelectedIndex
                        Case 0
                            N_Mas_Regolamento_Tipo = enum_PUARegolamenti_Tipo.PianoComcimazione
                        Case Else
                            N_Mas_Regolamento_Tipo = enum_PUARegolamenti_Tipo.PUA
                    End Select
            End Select


            bRet = objPC_Dettagli.Scrivi(PC_Testata_Cod,
                                         PC_Dettagli_Cod,
                                         Qs_Piva,
                                         Txt_Descrizione.Text,
                                         ddlFinalitaRer_ColturaPrincipale.SelectedItem.Value,
                                         "",
                                         ddlEpocaModalitaDistribuzione.SelectedItem.Value,
                                         0, 0,
                                         ResaDichiarata,
                                         Precessione,
                                         Grfi_Cod_Precessione, "",
                                         0,
                                         Sabbia, Limo, Argilla, Ph, CalcTot, CalcAtt, CN, SO, N,
                                         P2O5, K2O,
                                         QtaN_FerPrec,
                                         Frequenza,
                                         0, 0, 0, 0, 0,
                                         0, 0, 0, 0,
                                         TipoFertilizzante,
                                         0,
                                         Piovosita,
                                         0,
                                         CInt(Anno),
                                         "", "", "",
                                         ValiditaInizio, ValiditaFine,
                                         "", 0, 0,
                                         CInt(Cmb_Specie_ColturaPrincipale.SelectedValue),
                                         0, 0, 0, 0,
                                         0,
                                         0,
                                         ValiditaInizio, ValiditaFine,
                                         QtaMaxN, QtaMaxP, QtaMaxk,
                                         Mg, CSC, Analisi_Cod,
                                         0, CInt(Cmb_Centro.SelectedValue),
                                         P, PC_Dettagli_Flag_P,
                                         K, PC_Dettagli_Flag_K,
                                         0,
                                         N_MAS, N_Mas_Regolamento_Tipo,
                                         0, 0, 0,
                                         0, 0, 0,
                                         objParametri_Server,
                                         ResaStoricaPrecessione,
                                         Flag_ResiduiPrecessione,
                                         PeriodoInterramentoResiduiPrecessione,
                                         PeriodoSeminaColturaPrincipale,
                                         PeriodoRaccoltaColturaPrincipale,
                                         AvgTemperatura_ColturaInCampo,
                                         AvgTemperatura_MeseSemina_Febbraio,
                                         PercUmiditaColturaPrincipale,
                                         PercUmiditaRaccoltaPrecessione)

            If Not bRet Then

                objLog.Scrivi_LOG(objParametri_Server, "PCB_Inserimento.Salva", "Errore salvataggio testata")

            Else

                ' FATTORI CORRETTIVI

                If Qs_Tipo = enum_PianoConcimazione_Tipo.Schede Then

                    objPC_FattoriCorrettivi = New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_FattoriCorrettivi_W

                    If Qs_Operazione = enum_TipoOperazioneDB.Modifica Then

                        objPC_FattoriCorrettivi.Cancella(0,
                                                         PC_Testata_Cod,
                                                         0,
                                                         "",
                                                         objParametri_Server)

                    End If

                    'Select Case ddlFaseCiclo.SelectedValue

                    '    Case enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto,
                    '         enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento,
                    '         enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento

                    '    Case Else

                    '        Dim vetFattori() As Integer

                    '        ' azoto
                    '        LeggiFattoriSelezionati(grdIncrementi, "Chk_SelIncrementi_N", vetFattori)
                    '        LeggiFattoriSelezionati(grdDecrementi, "Chk_SelDecrementi_N", vetFattori)

                    '        ' fosforo
                    '        LeggiFattoriSelezionati(grdIncrementiP, "Chk_SelIncrementi_P", vetFattori)
                    '        LeggiFattoriSelezionati(grdDecrementiP, "Chk_SelDecrementi_P", vetFattori)

                    '        ' potassio
                    '        LeggiFattoriSelezionati(grdIncrementiK, "Chk_SelIncrementi_K", vetFattori)
                    '        LeggiFattoriSelezionati(grdDecrementiK, "Chk_SelDecrementi_K", vetFattori)

                    '        ' salvo le dosi standard nella tabella dei FattoriCorrettivi
                    '        Dim lun As Integer = -1
                    '        If Not IsNothing(vetFattori) Then
                    '            lun = vetFattori.Length - 1
                    '        End If

                    '        If Not IsNothing(ddlDoseP.SelectedValue) AndAlso IsNumeric(ddlDoseP.SelectedValue.Split("|")(0)) Then
                    '            ReDim Preserve vetFattori(lun + 1)
                    '            vetFattori(lun + 1) = ddlDoseP.SelectedValue.Split("|")(0)
                    '            lun = lun + 1
                    '        End If

                    '        'If IsNumeric(ddlDoseK.SelectedValue) Then
                    '        If Not IsNothing(ddlDoseK.SelectedValue) AndAlso IsNumeric(ddlDoseK.SelectedValue.Split("|")(0)) Then
                    '            ReDim Preserve vetFattori(lun + 1)
                    '            vetFattori(lun + 1) = ddlDoseK.SelectedValue.Split("|")(0)
                    '        End If

                    '        If Not vetFattori Is Nothing Then
                    '            For i = 0 To vetFattori.Length - 1
                    '                bRet = objPC_FattoriCorrettivi.Scrivi(CInt(ddlRegolamento.SelectedValue),
                    '                                                      PC_Testata_Cod,
                    '                                                      vetFattori(i),
                    '                                                      AGRODATAINIZIO,
                    '                                                      AGRODATAFINE,
                    '                                                      objParametri_Server)
                    '            Next
                    '        End If
                    'End Select
                End If
            End If

            objLog = Nothing

            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)


        Catch ex As Exception

            Try
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            Catch

            End Try

            Master.HiddenMessaggioErrore = "Salvataggio non riuscito :" & ex.Message
            Exit Sub

        Finally

            If Not objParametri_Server.objConnessione Is Nothing Then AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        End Try

        Me.Div_BtnApplica.Visible = True
        Div_NPK_Calcolati.Visible = True

        Txt_N_Da_Applicare.Text = N_Ammesso.Value
        'Txt_P_Da_Applicare.Text = P_Ammesso.Value
        'Txt_K_Da_Applicare.Text = K_Ammesso.Value

        Select Case Qs_Tipo
            Case enum_PianoConcimazione_Tipo.Bilancio
                If IsNumeric(ddlMasB.SelectedItem.Value) And IsNumeric(N_Ammesso.Value) Then
                    If CDec(N_Ammesso.Value) > CDec(ddlMasB.SelectedItem.Value) Then
                        Txt_N_Da_Applicare.Text = ddlMasB.SelectedItem.Value
                    End If
                End If
            Case Else
                If IsNumeric(ddlMasS.SelectedItem.Value) And IsNumeric(N_Ammesso.Value) Then
                    If CDec(N_Ammesso.Value) > CDec(ddlMasS.SelectedItem.Value) Then
                        Txt_N_Da_Applicare.Text = ddlMasS.SelectedItem.Value
                    End If
                End If
        End Select


        Master.HiddenMessaggioOK = Resources.PianoConcimazione_2017.SalvataggioRiuscito

        If Qs_Operazione = enum_TipoOperazioneDB.Scrittura Then
            Testata_Cod.Value = PC_Testata_Cod
            Qs_PCTestataCod = PC_Testata_Cod

            Qs_Operazione = enum_TipoOperazioneDB.Modifica
        End If

    End Sub



    '####################################################################################################


    <WebMethod(EnableSession:=True)>
    Public Shared Function SportelloPCB(ByVal Piva As String) As RispostaStandard

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
            objPratiche.Data_Sportello_Da_Servizio(Piva, enum_Servizi.PianoConcimazione, DateTime.Now, SportelloAperto, dataMin, dataMax, objParametri_Server, objParametri_Utenti)

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

    Private Shared Function GetStatoImpiantoDes(ByVal listaStatiImpianti As List(Of Fase),
                                                ByVal statoImpiantoCod As Integer
                                                ) As String

        Dim descrizione As String = ""

        If Not listaStatiImpianti Is Nothing AndAlso listaStatiImpianti.Count > 0 Then
            Dim obj = (From l In listaStatiImpianti Where l.Codice = statoImpiantoCod Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                descrizione = obj.Descrizione
            End If
        End If

        Return descrizione

    End Function

#Region "CARICA DATI"
    Private Sub Carica_Dati()

        Dim Dt_Fattori_Sel As DataTable

        Dim Tipo As enum_PianoConcimazione_Tipo

        Dim Regolamento As Integer

        Dim ValiditaInizio As Date
        Dim ValiditaFine As Date

        Dim Veg_Cod_ColturaPrincipale As Integer
        Dim Grfi_Cod_ColturaPrincipale As Integer
        Dim FaseCiclo As Integer


        Dim Veg_Cod_Precessione As Integer
        Dim Grfi_Cod_Precessione As Integer
        Dim EpocaModalitaDistribuzione As Integer

        Dim Analisi_Cod As Integer = 0

        AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PUA_Regolamento_WS(ddlRegolamento, False, "", "", enum_PUARegolamenti_Tipo.PianoNutrizionale_IBF, 0, "", " Ordine desc ")

        CaricaListControl.Centri_Aziendali(Cmb_Centro, True, Resources.PianoConcimazione_2017.TuttiCentriAziendali, "0", Qs_Piva, True, 2, "", "", objParametri_Server)

        Meteo_TipoSorgente.Value = "0"
        Meteo_Sorgente.Value = "0"

        Dim sa_cod = CInt(Cmb_Centro.SelectedValue)
        CaricaListControl.Analisi_Terreno(ddlAnalisi, True, Resources.PianoConcimazione_2017.NessunaAnalisi, "0", Qs_Piva, "", "Analisi_Testata_Data_Inizio DESC", objParametri_Server, sa_cod)

        Select Case Qs_Operazione

            Case enum_TipoOperazioneDB.Scrittura
                ' preparo il filtro delle analisi
                Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

                Dim Anno As Integer = UtilityProvider.Numeri_from_StringaAlfaNumerica(ddlRegolamento.SelectedItem.Text)
                If Anno > 0 Then
                    Txt_Anno.Text = Anno.ToString
                    objImpost.AnnataAgraria(CDate("01/01/" & Anno.ToString), ValiditaInizio, ValiditaFine, objParametri_Utenti)
                    Txt_ValiditaInizio.Text = ValiditaInizio
                    Txt_ValiditaFine.Text = ValiditaFine
                End If

                Dim Regolamento_Def As String =
                    objImpost.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_Nitrati_RegolamentoPC_Default, objParametri_Utenti)

                If IsNumeric(Regolamento_Def) Then
                    ddlRegolamento.SelectedIndex = ddlRegolamento.Items.IndexOf(ddlRegolamento.Items.FindByValue(Regolamento_Def))
                End If

                Regolamento = CInt(ddlRegolamento.SelectedValue)

                Txt_Descrizione.Text = ddlRegolamento.SelectedItem.Text

                Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R

                Dim dataMin As Date = AGRODATAINIZIO
                Dim dataMax As Date = AGRODATAFINE
                Dim SportelloAperto As Boolean
                objPratiche.Data_Sportello_Da_Servizio(Qs_Piva, enum_Servizi.PianoConcimazione, DateTime.Now, SportelloAperto, dataMin, dataMax, objParametri_Server, objParametri_Utenti)

                If IsDate(Txt_ValiditaInizio.Text) AndAlso CDate(Txt_ValiditaInizio.Text) < dataMin Then
                    Txt_ValiditaInizio.Text = dataMin.ToShortDateString
                End If

                If IsDate(Txt_ValiditaFine.Text) AndAlso CDate(Txt_ValiditaFine.Text) > dataMax Then
                    Txt_ValiditaFine.Text = dataMax.ToShortDateString
                End If

                Carica_Dati_Regolamento(Regolamento)

                If ddlAnalisi.Items.Count > 1 Then
                    ddlAnalisi.SelectedIndex = 1
                End If
                ddlAnalisi_SelectedIndexChanged(Me, Nothing)

            Case Else

                If Qs_Operazione <> enum_TipoOperazioneDB.Lettura Then
                    Div_Bilancio.Visible = True
                    Div_Schede.Visible = True
                    Div_BTNSalva.Visible = True
                    Div_Appezzamenti.Visible = True
                End If

                '----------------
                ' DATI PIANO
                '----------------
                Dim Dt_Piano As DataTable
                Dim objPCD_DAL As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_R

                Dt_Piano = objPCD_DAL.Leggi(Qs_PCTestataCod,
                                           "", "", objParametri_Server)

                If Dt_Piano.Rows.Count > 0 Then
                    '--------------------
                    '   TESTATA
                    '--------------------
                    Tipo = Dt_Piano.Rows(0).Item("PC_Tipo")

                    Regolamento = Dt_Piano.Rows(0).Item("Regolamento_Cod")
                    ddlRegolamento.SelectedIndex = ddlRegolamento.Items.IndexOf(ddlRegolamento.Items.FindByValue(Regolamento))
                    Carica_Dati_Regolamento(Regolamento)

                    Txt_Descrizione.Text = Dt_Piano.Rows(0).Item("PC_Testata_Des")

                    ValiditaInizio = Dt_Piano.Rows(0).Item("validita_inizio_piano")
                    ValiditaFine = Dt_Piano.Rows(0).Item("Validita_Fine_piano")

                    If ValiditaInizio = AGRODATAINIZIO Then
                        Txt_ValiditaInizio.Text = ""
                    Else
                        Txt_ValiditaInizio.Text = ValiditaInizio.ToShortDateString
                    End If
                    If ValiditaFine = AGRODATAFINE Then
                        Txt_ValiditaFine.Text = ""
                    Else
                        Txt_ValiditaFine.Text = ValiditaFine.ToShortDateString
                    End If

                    Txt_Anno.Text = Dt_Piano.Rows(0).Item("PC_Dettagli_Anno")
                    Txt_Note.Text = Dt_Piano.Rows(0).Item("Note")

                    If Not IsDBNull(Dt_Piano.Rows(0).Item("Flag_NonUtilizzo_Fertilizzanti")) AndAlso CInt(Dt_Piano.Rows(0).Item("Flag_NonUtilizzo_Fertilizzanti")) > 0 Then
                        Chk_NonUtilizzo_Fertilizzanti.Checked = True
                    End If


                    '--------------------
                    '   DATI COLTURA E UBICAZIONE 
                    '--------------------
                    Cmb_Centro.SelectedValue = Dt_Piano.Rows(0).Item("PC_Dettagli_SaCod")

                    Veg_Cod_ColturaPrincipale = Dt_Piano.Rows(0).Item("PC_Dettagli_ColturaPrincipale_Veg_Cod")
                    Cmb_Specie_ColturaPrincipale.SelectedIndex = Cmb_Specie_ColturaPrincipale.Items.IndexOf(Cmb_Specie_ColturaPrincipale.Items.FindByValue(Veg_Cod_ColturaPrincipale))

                    Grfi_Cod_ColturaPrincipale = Dt_Piano.Rows(0).Item("PC_Dettagli_Finalita_GRFI_COD")
                    FaseCiclo = Dt_Piano.Rows(0).Item("PC_Dettagli_FaseCicloColturale_id_fase")

                    Carica_Dati_Specie(Regolamento, Veg_Cod_ColturaPrincipale, Grfi_Cod_ColturaPrincipale, FaseCiclo, TipoColtura.Coltura_Principale, ddlFinalitaRer_ColturaPrincipale)

                    Txt_Resa.Text = Dt_Piano.Rows(0).Item("PC_Dettagli_Resa")
                    Txt_ResaStoricaPrecessione.Text = Dt_Piano.Rows(0).Item("PC_Dettagli_Resa_Storica")

                    cmb_PeriodoSeminaColturaPrincipale.Value = Dt_Piano.Rows(0).Item("PC_Dettagli_Mese_Semina")
                    cmb_PeriodoRaccoltaColturaPrincipale.Value = Dt_Piano.Rows(0).Item("PC_Dettagli_Mese_Raccolta")


                    '--------------------
                    '   CARATTERISTICHE SUOLO
                    '--------------------
                    If Not IsDBNull(Dt_Piano.Rows(0).Item("PC_Dettagli_Analisi_Testata_Cod")) Then

                        Analisi_Cod = Dt_Piano.Rows(0).Item("PC_Dettagli_Analisi_Testata_Cod")
                        ddlAnalisi.SelectedIndex = ddlAnalisi.Items.IndexOf(ddlAnalisi.Items.FindByValue(Analisi_Cod))
                        Div_SalvaAnalisi.Visible = False

                        If Not IsDBNull(Dt_Piano.Rows(0).Item("Analisi_Testata_Data_Fine")) AndAlso
                                IsDate(Dt_Piano.Rows(0).Item("Analisi_Testata_Data_Fine")) AndAlso
                                CDate(Dt_Piano.Rows(0).Item("Analisi_Testata_Data_Fine")) < ValiditaInizio Then
                            Messaggi.AgroMsgBox(String.Format(Resources.PianoConcimazione_2017.AttenzioneAnalisiSelezionataScadutaIl, CDate(Dt_Piano.Rows(0).Item("Analisi_Testata_Data_Fine")).ToShortDateString), Page, , UpdatePanelPerScript)
                        End If
                    End If

                    Dim Mg As Decimal = 0
                    Dim CSC As Decimal = 0
                    If Not IsDBNull(Dt_Piano.Rows(0).Item("PC_Dettagli_Mg")) Then
                        Mg = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_Mg"))
                    End If
                    If Not IsDBNull(Dt_Piano.Rows(0).Item("PC_Dettagli_CSC")) Then
                        CSC = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_CSC"))
                    End If
                    Dim P As Decimal = 0
                    Dim K As Decimal = 0
                    Dim Flag_P As Integer = 0
                    Dim Flag_K As Integer = 0
                    If Not IsDBNull(Dt_Piano.Rows(0).Item("PC_Dettagli_P")) Then
                        P = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_P"))
                    End If
                    If Not IsDBNull(Dt_Piano.Rows(0).Item("PC_Dettagli_K")) Then
                        K = CDec(Dt_Piano.Rows(0).Item("PC_Dettagli_K"))
                    End If
                    If Not IsDBNull(Dt_Piano.Rows(0).Item("PC_Dettagli_Flag_P")) Then
                        Flag_P = Dt_Piano.Rows(0).Item("PC_Dettagli_Flag_P")
                    End If
                    If Not IsDBNull(Dt_Piano.Rows(0).Item("PC_Dettagli_Flag_K")) Then
                        Flag_K = Dt_Piano.Rows(0).Item("PC_Dettagli_Flag_K")
                    End If
                    Analisi_Valorizza(Dt_Piano.Rows(0).Item("PC_Dettagli_Sabbia"),
                                      Dt_Piano.Rows(0).Item("PC_Dettagli_Limo"),
                                      Dt_Piano.Rows(0).Item("PC_Dettagli_Argilla"),
                                      Dt_Piano.Rows(0).Item("PC_Dettagli_Ph"),
                                      Dt_Piano.Rows(0).Item("PC_Dettagli_Caco3"),
                                      Dt_Piano.Rows(0).Item("PC_Dettagli_Caco3_Attivo"),
                                      Dt_Piano.Rows(0).Item("PC_Dettagli_So"),
                                      Dt_Piano.Rows(0).Item("PC_Dettagli_ntot"),
                                      Dt_Piano.Rows(0).Item("PC_Dettagli_p2o5"),
                                      Dt_Piano.Rows(0).Item("PC_Dettagli_k2o"),
                                      Dt_Piano.Rows(0).Item("PC_Dettagli_CN"),
                                      Mg, CSC,
                                      P, K, Flag_P, Flag_K)

                    '--------------------
                    '   PRATICHE AGRONOMICHE
                    '--------------------
                    If Dt_Piano.Rows(0).Item("PC_Dettagli_Precessione_Veg_Cod") <> 0 Then
                        Veg_Cod_Precessione = Dt_Piano.Rows(0).Item("PC_Dettagli_Precessione_Veg_Cod")
                        Cmb_Specie_Precessione.SelectedIndex = Cmb_Specie_Precessione.Items.IndexOf(Cmb_Specie_Precessione.Items.FindByValue(Dt_Piano.Rows(0).Item("PC_Dettagli_Precessione_Veg_Cod")))
                    End If

                    If Dt_Piano.Rows(0).Item("PC_Dettagli_FinalitaPrecessione_Grfi_Cod") <> 0 Then
                        Grfi_Cod_Precessione = Dt_Piano.Rows(0).Item("PC_Dettagli_FinalitaPrecessione_Grfi_Cod")
                        ddlFinalitaRer_Precessione.SelectedIndex = ddlFinalitaRer_Precessione.Items.IndexOf(ddlFinalitaRer_Precessione.Items.FindByValue(Dt_Piano.Rows(0).Item("PC_Dettagli_FinalitaPrecessione_Grfi_Cod")))
                    End If

                    If Dt_Piano.Rows(0).Item("PC_Dettagli_FaseCicloColturale_id_fase") <> 0 Then
                        EpocaModalitaDistribuzione = Dt_Piano.Rows(0).Item("PC_Dettagli_FaseCicloColturale_id_fase")
                        ddlEpocaModalitaDistribuzione.SelectedIndex = ddlEpocaModalitaDistribuzione.Items.IndexOf(ddlEpocaModalitaDistribuzione.Items.FindByValue(Dt_Piano.Rows(0).Item("PC_Dettagli_FaseCicloColturale_id_fase")))
                    End If

                    Carica_Dati_Specie(Regolamento, Veg_Cod_Precessione, Grfi_Cod_Precessione, EpocaModalitaDistribuzione, TipoColtura.Precessione, ddlFinalitaRer_Precessione)

                    If Not IsDBNull(Dt_Piano.Rows(0).Item("PC_Dettagli_Flag_Residui_Precessione_Asportati")) AndAlso CInt(Dt_Piano.Rows(0).Item("PC_Dettagli_Flag_Residui_Precessione_Asportati")) > 0 Then
                        Chk_ResiduiPrecessione.Checked = True
                    End If

                    cmb_PeriodoInterramentoResiduiPrecessione.Value = Dt_Piano.Rows(0).Item("PC_Dettagli_Mese_Residui_Precessione_Interramento")

                    ' fertilizzazioni precedenti
                    If Dt_Piano.Rows(0).Item("PC_Dettagli_Fertilizzazione_id_tp_fer") <> 0 Then
                        ddlConcimeOrganico.SelectedIndex = ddlConcimeOrganico.Items.IndexOf(ddlConcimeOrganico.Items.FindByValue(Dt_Piano.Rows(0).Item("PC_Dettagli_Fertilizzazione_id_tp_fer")))
                    End If

                    Txt_QtaN_KGHa.Text = Format(Dt_Piano.Rows(0).Item("PC_Dettagli_Quantita"), "0.00")


                    '--------------------
                    '   METEO
                    '--------------------
                    'leggo se esiste una stazione associata al centro
                    If Dt_Piano.Rows(0).Item("PC_Dettagli_SaCod") > 0 Then
                        Dim reader = New AgronicaCoreMeteoDAL.DSS_Centri_Aziendali_Agronica_Stazioni_Meteo_R
                        Dim def_staz = reader.Leggi("Piva_SuperUser = '" & objParametri_Server.PivaSuperUser & "' AND piva = '" & Qs_Piva & "' AND sa_cod = " & Dt_Piano.Rows(0).Item("PC_Dettagli_SaCod").ToString, "", objParametri_Server)
                        If def_staz IsNot Nothing AndAlso def_staz.Rows.Count > 0 Then
                            Meteo_TipoSorgente.Value = def_staz.Rows(0)("tipo_sorgente")
                            Meteo_Sorgente.Value = def_staz.Rows(0)("Stazione_Cod")
                        End If
                    End If

                    Txt_Pioggia.Text = Dt_Piano.Rows(0).Item("PC_Dettagli_Piovosita")
                    Txt_AvgTemperatura_ColturaInCampo.Text = Dt_Piano.Rows(0).Item("PC_Dettagli_Temperatura_Media_ColturaInCampo")
                    Txt_AvgTemperatura_MeseSemina_Febbraio.Text = Dt_Piano.Rows(0).Item("PC_Dettagli_Temperatura_Media_MeseSemina_Febbraio")
                    Txt_PercUmiditaColturaPrincipale.Text = Dt_Piano.Rows(0).Item("PC_Dettagli_Perc_Umidita_Coltura_Principale")
                    Txt_PercUmiditaRaccoltaPrecessione.Text = Dt_Piano.Rows(0).Item("PC_Dettagli_Perc_Umidita_Raccolta_Precessione")


                    '--------------------
                    '   NPK MAX
                    '--------------------
                    If Not IsDBNull(Dt_Piano.Rows(0).Item("N_Ammesso")) Then
                        N_Ammesso.Value = CDec(Dt_Piano.Rows(0).Item("N_Ammesso"))
                    End If
                    If Not IsDBNull(Dt_Piano.Rows(0).Item("P_Ammesso")) Then
                        P_Ammesso.Value = CDec(Dt_Piano.Rows(0).Item("P_Ammesso"))
                    End If
                    If Not IsDBNull(Dt_Piano.Rows(0).Item("K_Ammesso")) Then
                        K_Ammesso.Value = CDec(Dt_Piano.Rows(0).Item("K_Ammesso"))
                    End If

                    If Qs_Tipo = enum_PianoConcimazione_Tipo.Schede Then
                        If Not IsDBNull(Dt_Piano.Rows(0).Item("N_Mas_Regolamento_Tipo")) AndAlso IsNumeric(Dt_Piano.Rows(0).Item("N_Mas_Regolamento_Tipo")) AndAlso CInt(Dt_Piano.Rows(0).Item("N_Mas_Regolamento_Tipo")) > 1 Then
                            ddlMasS.SelectedIndex = 1
                        End If
                    Else
                        If Not IsDBNull(Dt_Piano.Rows(0).Item("N_Mas_Regolamento_Tipo")) AndAlso IsNumeric(Dt_Piano.Rows(0).Item("N_Mas_Regolamento_Tipo")) AndAlso CInt(Dt_Piano.Rows(0).Item("N_Mas_Regolamento_Tipo")) > 1 Then
                            ddlMasB.SelectedIndex = 1
                        End If
                    End If
                End If


                '---------------------------
                ' IMPIANTI ASSOCIATI PIANO
                '---------------------------
                'lettura impianti a cui è associato il piano
                Dim Dt_Entita As DataTable
                Dim objPCE_DAL As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_EntitaxTestata_R

                Dt_Entita = objPCE_DAL.Leggi(Qs_PCTestataCod,
                                       "", "", objParametri_Server)

                If Dt_Entita.Rows.Count > 0 Then

                    For i = 0 To Dt_Entita.Rows.Count - 1

                        For j = 0 To GridView_Impianti.Rows.Count - 1

                            If Dt_Entita.Rows(i).Item("piva") = GridView_Impianti.DataKeys(j).Item("piva") And
                               Dt_Entita.Rows(i).Item("sa_cod") = GridView_Impianti.DataKeys(j).Item("sa_cod") And
                               Dt_Entita.Rows(i).Item("appezza") = GridView_Impianti.DataKeys(j).Item("appezza") And
                               Dt_Entita.Rows(i).Item("id_imp") = GridView_Impianti.DataKeys(j).Item("id_reg") And
                               Dt_Entita.Rows(i).Item("progetto_cod") = GridView_Impianti.DataKeys(j).Item("progetto_cod") Then

                                CType(GridView_Impianti.Rows(j).FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True

                            End If
                        Next
                    Next
                End If

                'Fattori Correttivi ASSOCIATI PIANO
                If Qs_Tipo = enum_PianoConcimazione_Tipo.Schede Then

                    Dim objFC_DAL As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_FattoriCorrettivi_R

                    Dt_Fattori_Sel = objFC_DAL.Leggi(0, Qs_PCTestataCod,
                                                 0, "", "", objParametri_Server)

                End If
        End Select

        Select Case Qs_Tipo
            Case enum_PianoConcimazione_Tipo.Bilancio
                CalcolaBilancio()
                Div_Bilancio.Visible = True
                Div_Schede.Visible = False
            Case enum_PianoConcimazione_Tipo.Schede
                CalcolaSchede(Dt_Fattori_Sel)
                Div_Bilancio.Visible = False
                Div_Schede.Visible = True
        End Select
    End Sub

    Private Sub Carica_Dati_Regolamento(ByVal Regolamento_Cod As Integer)

        '(27/02/2019 fede) caricate specie del regolamento
        Select Case RBL_Specie.SelectedValue
            Case "1"
                Dim clc As New CaricaListControl
                clc.TutteSpecieColtivate_SenzaControlloData(Cmb_Specie_ColturaPrincipale, False, "", "", Qs_Piva, 0, False, "", "", objParametri_Server, True)
            Case Else
                AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PC_SpecieConcimazione_WS(Cmb_Specie_ColturaPrincipale, False, "", "", Regolamento_Cod, "", "")
        End Select

        If IsNumeric(Cmb_Specie_ColturaPrincipale.SelectedValue) Then
            Carica_Dati_Specie(Regolamento_Cod, CInt(Cmb_Specie_ColturaPrincipale.SelectedValue), 0, 0, TipoColtura.Coltura_Principale, ddlFinalitaRer_ColturaPrincipale)
            Me.Div_BtnApplica.Visible = False
            Div_NPK_Calcolati.Visible = False
        Else
            Exit Sub
        End If

        AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PC_SpecieConcimazione_WS(Cmb_Specie_Precessione,
            True, "NESSUNA COLTURA", "0",
            Regolamento_Cod, "", "")
        'default Non definita
        Cmb_Specie_Precessione.SelectedIndex = Cmb_Specie_Precessione.Items.IndexOf(Cmb_Specie_Precessione.Items.FindByValue(0))

        AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PUA_Effluenti_WS(ddlConcimeOrganico,
            True, "NESSUNA SELEZIONE", "0",
            Regolamento_Cod, "", "", objParametri_Server, objParametri_Super_Server)
        'default nessuno
        ddlConcimeOrganico.SelectedIndex = ddlConcimeOrganico.Items.IndexOf(ddlConcimeOrganico.Items.FindByValue(0))

    End Sub

    Private Sub Carica_Dati_Specie(ByVal Regolamento_Cod As Integer, ByVal Veg_Cod As Integer, ByVal Grfi_Cod As Integer, ByVal FaseCiclo As Integer, TipoColtura As TipoColtura, ddlFinalita As DropDownList)

        AgronicaControlli_2010.ListControl_PianoConcimazione_WS.Finalita_Rer_WS(ddlFinalita, False, "", "", Regolamento_Cod, Veg_Cod, 0, "", "", "")

        If Grfi_Cod > 0 Then
            ddlFinalita.SelectedIndex = ddlFinalita.Items.IndexOf(ddlFinalita.Items.FindByValue(Grfi_Cod))
        Else
            If IsNumeric(ddlFinalita.SelectedValue) Then
                Grfi_Cod = CInt(ddlFinalita.SelectedValue)
            End If
        End If

        If TipoColtura = TipoColtura.Precessione Then
            AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PUA_EpocheModalita_WS(ddlEpocaModalitaDistribuzione, True, "NESSUNA SELEZIONE", "0", Regolamento_Cod, Veg_Cod, "", "")
            If FaseCiclo > 0 Then
                ddlEpocaModalitaDistribuzione.SelectedIndex = ddlEpocaModalitaDistribuzione.Items.IndexOf(ddlEpocaModalitaDistribuzione.Items.FindByValue(FaseCiclo))
            End If
        End If

        If TipoColtura = TipoColtura.Coltura_Principale Then
            Dim N_Fattore_Correttivo_Resa As Decimal
            Dim Mas As Decimal = -1
            Dim Mas_Dir_Nitrati As Decimal = -1

            Txt_Resa.Text = LeggiResa(Regolamento_Cod, Veg_Cod, Grfi_Cod, enum_Stato_Impianto.Impianto_Produzione, Mas, N_Fattore_Correttivo_Resa, Mas_Dir_Nitrati)

            Lbl_ResaRiferimento.InnerText = Txt_Resa.Text

            Fattore_Correttivo_N_Resa.Value = N_Fattore_Correttivo_Resa

            ddlMasS.Items.Clear()
            ddlMasB.Items.Clear()

            If Qs_Tipo = enum_PianoConcimazione_Tipo.Schede Then
                If Mas >= 0 Then
                    ddlMasS.Items.Add(New ListItem(Resources.PianoConcimazione_2017.PianoConcimazione & ": " & Mas.ToString, Mas))
                Else
                    ddlMasS.Items.Add(New ListItem(Resources.PianoConcimazione_2017.PianoConcimazione & ": n.d.", "n.d."))
                End If

                If Mas_Dir_Nitrati >= 0 Then
                    ddlMasS.Items.Add(New ListItem(Resources.PianoConcimazione_2017.DirettivaNitrati & ": " & Mas_Dir_Nitrati.ToString, Mas_Dir_Nitrati))
                Else
                    ddlMasS.Items.Add(New ListItem(Resources.PianoConcimazione_2017.DirettivaNitrati & ": n.d.", "n.d."))
                End If
            Else
                If Mas >= 0 Then
                    ddlMasB.Items.Add(New ListItem(Resources.PianoConcimazione_2017.LimiteMASPianoConcimazione & ": " & Mas.ToString & " Kg/Ha", Mas))
                Else
                    ddlMasB.Items.Add(New ListItem(Resources.PianoConcimazione_2017.LimiteMASPianoConcimazione & ": n.d.", "n.d."))
                End If

                If Mas_Dir_Nitrati >= 0 Then
                    ddlMasB.Items.Add(New ListItem(Resources.PianoConcimazione_2017.LimiteMASDirettivaNitrati & ": " & Mas_Dir_Nitrati.ToString & " Kg/Ha", Mas_Dir_Nitrati))
                Else
                    ddlMasB.Items.Add(New ListItem(Resources.PianoConcimazione_2017.LimiteMASDirettivaNitrati & ": n.d.", "n.d."))
                End If
            End If

            Dim sa_cod = CInt(Cmb_Centro.SelectedValue)
            Carica_Impianti(Veg_Cod, sa_cod)
        End If
    End Sub

    Private Sub Carica_Impianti(ByVal Veg_Cod As Integer, ByVal sa_cod As Integer)

        'lettura impianti dell'impresa corrente della specie scelta
        Dim Dt_Destinazioni As DataTable
        Dim Dr_Destinazioni As DataRow
        Dim i As Integer
        Dt_Destinazioni = GeneraStrutturaDTImpianti()
        GridView_Impianti.DataSource = Dt_Destinazioni
        GridView_Impianti.DataBind()

        Dim DataInizio As Date = AGRODATAINIZIO
        Dim DataFine As Date = AGRODATAFINE

        Dim strFiltroDate As String

        If IsDate(Txt_ValiditaInizio.Text) Then
            DataInizio = CDate(Txt_ValiditaInizio.Text)
        End If
        If IsDate(Txt_ValiditaFine.Text) Then
            DataFine = CDate(Txt_ValiditaFine.Text)
        End If
        strFiltroDate = " AND ip.validita_fine >=" & Agro_SQL_SaveDate(DataInizio)
        strFiltroDate &= "  AND ip.validita_inizio <=" & Agro_SQL_SaveDate(DataFine)


        Dim Dt_Impianti As DataTable
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dt_Impianti = objImpianti.Leggi_Dati_Impianti_per_PianoConcimazione(Qs_Piva,
                                         sa_cod, 0, 0, 0,
                                        " s.veg_cod=" & Veg_Cod.ToString & strFiltroDate, " ip.validita_inizio DESC ", objParametri_Server)

        Dim Dt_Catasto As DataTable
        Dt_Catasto = objImpianti.Leggi_Catasto_Impianti_per_PianoConcimazione(Qs_Piva,
                                         sa_cod, 0, 0, 0,
                                        " s.veg_cod=" & Veg_Cod.ToString & strFiltroDate, "", objParametri_Server)


        Dim listaStatiImpianti As List(Of Fase)

        If Dt_Impianti.Rows.Count > 0 Then
            listaStatiImpianti = GetListaStatiImpiantiDes(CInt(ddlRegolamento.SelectedValue), Veg_Cod)
        End If

        Dim HasHimp As New Hashtable

        For i = 0 To Dt_Impianti.Rows.Count - 1

            If Not HasHimp.ContainsKey(Dt_Impianti.Rows(i).Item("piva") & "_" & Dt_Impianti.Rows(i).Item("sa_cod") & "_" & Dt_Impianti.Rows(i).Item("appezza") & "_" & Dt_Impianti.Rows(i).Item("id_reg")) Then

                HasHimp.Add(Dt_Impianti.Rows(i).Item("piva") & "_" & Dt_Impianti.Rows(i).Item("sa_cod") & "_" & Dt_Impianti.Rows(i).Item("appezza") & "_" & Dt_Impianti.Rows(i).Item("id_reg"), "")

                Dr_Destinazioni = Dt_Destinazioni.NewRow

                Dr_Destinazioni.Item("piva") = Dt_Impianti.Rows(i).Item("piva")
                Dr_Destinazioni.Item("sa_cod") = Dt_Impianti.Rows(i).Item("sa_cod")
                Dr_Destinazioni.Item("campo_cod") = Dt_Impianti.Rows(i).Item("campo_cod")
                Dr_Destinazioni.Item("appezza") = Dt_Impianti.Rows(i).Item("appezza")
                Dr_Destinazioni.Item("id_reg") = Dt_Impianti.Rows(i).Item("id_reg")
                Dr_Destinazioni.Item("progetto_cod") = Dt_Impianti.Rows(i).Item("progetto_cod")

                Dr_Destinazioni.Item("veg_cod") = Dt_Impianti.Rows(i).Item("veg_Cod")

                'se non è valorizzato il campo (dati pregressi)
                'lo ricavo da un impianto
                If Veg_Cod = 0 Then
                    Veg_Cod = Dt_Impianti.Rows(i).Item("veg_Cod")
                End If

                Dr_Destinazioni.Item("cul_cod") = Dt_Impianti.Rows(i).Item("cul_Cod")
                Dr_Destinazioni.Item("grfi_cod") = Dt_Impianti.Rows(i).Item("Grfi_Cod")
                Dr_Destinazioni.Item("stato_impianto") = Dt_Impianti.Rows(i).Item("stato_impianto")

                Dr_Destinazioni.Item("rag_soc") = Dt_Impianti.Rows(i).Item("rag_soc")
                Dr_Destinazioni.Item("sa_nome") = Dt_Impianti.Rows(i).Item("sa_nome")
                Dr_Destinazioni.Item("app_nome") = Dt_Impianti.Rows(i).Item("app_nome")
                Dr_Destinazioni.Item("Campo_Nome") = Dt_Impianti.Rows(i).Item("campo_Des")
                Dr_Destinazioni.Item("descrizione") = Dt_Impianti.Rows(i).Item("veg_des") & " - " & Dt_Impianti.Rows(i).Item("cul_des") & " - " & Dt_Impianti.Rows(i).Item("grfi_des") & IIf(GetStatoImpiantoDes(listaStatiImpianti, Dt_Impianti.Rows(i).Item("stato_impianto")) <> "", " - " & GetStatoImpiantoDes(listaStatiImpianti, Dt_Impianti.Rows(i).Item("stato_impianto")), "")
                Dr_Destinazioni.Item("sup_imp") = Dt_Impianti.Rows(i).Item("sup_imp")
                Dr_Destinazioni.Item("validita_inizio") = IIf(Dt_Impianti.Rows(i).Item("Validita_inizio_Impianto").ToShortDateString <> "01/01/1900", Dt_Impianti.Rows(i).Item("Validita_inizio_Impianto").ToShortDateString, "...")
                Dr_Destinazioni.Item("validita_fine") = IIf(Dt_Impianti.Rows(i).Item("Validita_Fine_Impianto").ToShortDateString <> "31/12/2100", Dt_Impianti.Rows(i).Item("Validita_Fine_Impianto").ToShortDateString, "...")

                If Not IsNumeric(Dt_Impianti.Rows(i).Item("N_Massimo")) Then
                    Dr_Destinazioni.Item("QtaMaxN") = 0
                Else
                    Dr_Destinazioni.Item("QtaMaxN") = Dt_Impianti.Rows(i).Item("N_Massimo")
                End If

                If Not IsNumeric(Dt_Impianti.Rows(i).Item("P_Massimo")) Then
                    Dr_Destinazioni.Item("QtaMaxP2O5") = 0
                Else
                    Dr_Destinazioni.Item("QtaMaxP2O5") = Dt_Impianti.Rows(i).Item("P_Massimo")
                End If

                If Not IsNumeric(Dt_Impianti.Rows(i).Item("K_Massimo")) Then
                    Dr_Destinazioni.Item("QtaMaxK2O") = 0
                Else
                    Dr_Destinazioni.Item("QtaMaxK2O") = Dt_Impianti.Rows(i).Item("K_Massimo")
                End If

                Dim sCatasto As New Text.StringBuilder
                Dim separatore As String = "<br>"
                If Not Dt_Catasto Is Nothing Then
                    Dim DrCatasto() As DataRow = Dt_Catasto.Select("piva = '" & Dt_Impianti.Rows(i).Item("piva") & "' and sa_cod=" & Dt_Impianti.Rows(i).Item("sa_cod") & " and appezza=" & Dt_Impianti.Rows(i).Item("appezza"))
                    If Not DrCatasto Is Nothing Then
                        For Each d As DataRow In DrCatasto
                            sCatasto.Append(separatore + d("prov") & ":" & d("com") & ":_" & d("sezione") & ":_" & d("foglio") & ":_" & d("numero") & ":_" & d("subalterno"))
                        Next
                    End If
                End If
                If sCatasto.Length > separatore.Length Then
                    sCatasto = sCatasto.Remove(0, separatore.Length)
                End If

                Dr_Destinazioni.Item("catasto") = sCatasto.ToString

                Dt_Destinazioni.Rows.Add(Dr_Destinazioni)

            End If
        Next

        If Dt_Destinazioni.Rows.Count > 0 Then

            Dim DtKeys(5) As String
            DtKeys(0) = "piva"
            DtKeys(1) = "sa_cod"
            DtKeys(2) = "appezza"
            DtKeys(3) = "id_reg"
            DtKeys(4) = "progetto_cod"
            DtKeys(5) = "campo_cod"

            GridView_Impianti.DataSource = Dt_Destinazioni
            GridView_Impianti.DataKeyNames = DtKeys
            GridView_Impianti.DataBind()

            SettaImpianti()

        End If

    End Sub

#End Region


#Region "CALCOLA BILANCIO"
    Private Sub CalcolaBilancio()

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_IBF_INPUT

        '--------------------
        '   TESTATA
        '--------------------
        Dim Regolamento_Cod As Integer = CInt(ddlRegolamento.SelectedValue)
        objParametriIngresso.Regolamento_Cod = Regolamento_Cod


        '--------------------
        '   DATI COLTURA E UBICAZIONE 
        '--------------------
        If Not IsNumeric(Cmb_Specie_ColturaPrincipale.SelectedValue) Then
            Exit Sub
        End If
        objParametriIngresso.COLE_ColturaPrincipaleAnno = CInt(Cmb_Specie_ColturaPrincipale.SelectedValue)

        objParametriIngresso.Grfi_Cod_ColturaPrincipaleAnno = 0
        If IsNumeric(ddlFinalitaRer_ColturaPrincipale.SelectedValue) Then
            objParametriIngresso.Grfi_Cod_ColturaPrincipaleAnno = CInt(ddlFinalitaRer_ColturaPrincipale.SelectedValue)
        End If

        'If IsNumeric(ddlFaseCiclo.SelectedValue) Then
        '    objParametriIngresso.GRU = CInt(ddlFaseCiclo.SelectedValue)
        'Else
        '    objParametriIngresso.COLAA_GruppoCicloColturaleConcimato = 0
        'End If

        If IsNumeric(Txt_Resa.Text) Then
            objParametriIngresso.COLP_ResaUtilePrevistaColturaPrincipale_THa = CDec(Txt_Resa.Text.Replace(".", ","))
        Else
            objParametriIngresso.COLP_ResaUtilePrevistaColturaPrincipale_THa = 0
        End If

        If IsNumeric(Txt_ResaStoricaPrecessione.Text) Then
            objParametriIngresso.COLJ_ResaStoricaPrecessione_THa = CDec(Txt_ResaStoricaPrecessione.Text.Replace(".", ","))
        Else
            objParametriIngresso.COLJ_ResaStoricaPrecessione_THa = 0
        End If

        If IsNumeric(cmb_PeriodoSeminaColturaPrincipale.Value) Then
            objParametriIngresso.COLW_PeriodoSeminaColturaPrincipale = cmb_PeriodoSeminaColturaPrincipale.Value
        Else
            Exit Sub
        End If

        If IsNumeric(cmb_PeriodoRaccoltaColturaPrincipale.Value) Then
            objParametriIngresso.COLX_PeriodoRaccoltaColturaPrincipale = cmb_PeriodoRaccoltaColturaPrincipale.Value
        Else
            Exit Sub
        End If


        '--------------------
        '   CARATTERISTICHE SUOLO
        '--------------------
        If IsNumeric(Txt_Sabbia.Text) Then
            objParametriIngresso.SoilDataset_I_SabbiaPerc = CDec(Txt_Sabbia.Text.Replace(".", ","))
        Else
            objParametriIngresso.SoilDataset_I_SabbiaPerc = 0
        End If
        If IsNumeric(Txt_Argilla.Text) Then
            objParametriIngresso.SoilDataset_K_ArgillaPerc = CDec(Txt_Argilla.Text.Replace(".", ","))
        Else
            objParametriIngresso.SoilDataset_K_ArgillaPerc = 0
        End If
        If IsNumeric(Txt_Limo.Text) Then
            objParametriIngresso.SoilDataset_J_LimoPerc = CDec(Txt_Limo.Text.Replace(".", ","))
        Else
            objParametriIngresso.SoilDataset_J_LimoPerc = 0
        End If

        If IsNumeric(Txt_PH.Text) Then
            objParametriIngresso.SoilDataset_N_pH = CDec(Txt_PH.Text.Replace(".", ","))
        Else
            objParametriIngresso.SoilDataset_N_pH = 0
        End If

        If IsNumeric(Txt_N.Text) Then
            'LA FORMULA CONSIDERA N % --> NELLE ANALISI SI INSERISCE xMILLE
            objParametriIngresso.SoilDataset_T_NPerc = CDec(Txt_N.Text.Replace(".", ",")) / 10
        Else
            objParametriIngresso.SoilDataset_T_NPerc = 0
        End If
        If IsNumeric(Txt_SO.Text) Then
            objParametriIngresso.SoilDataset_S_SOPerc = CDec(Txt_SO.Text.Replace(".", ","))
        Else
            objParametriIngresso.SoilDataset_S_SOPerc = 0
        End If
        'If IsNumeric(Txt_CN.Text) Then
        '    objParametriIngresso.CN = CDec(Txt_CN.Text.Replace(".", ","))
        'Else
        '    objParametriIngresso.CN = 0
        'End If
        'If IsNumeric(Txt_P.Text) Then
        '    Select Case DDL_P2O5.SelectedValue
        '        Case "0"
        '            objParametriIngresso.P2O5 = CDec(Txt_P.Text.Replace(".", ","))
        '        Case Else
        '            objParametriIngresso.P2O5 = CDec(Txt_P.Text.Replace(".", ",")) * Fattore_Conversione_P_P2O5
        '    End Select
        'Else
        '    objParametriIngresso.P2O5 = 0
        'End If
        'If IsNumeric(Txt_K.Text) Then
        '    Select Case DDL_K2O.SelectedValue
        '        Case "0"
        '            objParametriIngresso.K2O = CDec(Txt_K.Text.Replace(".", ","))
        '        Case Else
        '            objParametriIngresso.K2O = CDec(Txt_K.Text.Replace(".", ",")) * Fattore_Conversione_K_K2O
        '    End Select
        'Else
        '    objParametriIngresso.K2O = 0
        'End If
        'If IsNumeric(Txt_Mg.Text) Then
        '    objParametriIngresso.Mg = CDec(Txt_Mg.Text.Replace(".", ","))
        'Else
        '    objParametriIngresso.Mg = 0
        'End If
        If IsNumeric(Txt_CSC.Text) Then
            objParametriIngresso.SoilDataset_Z_CSC = CDec(Txt_CSC.Text.Replace(".", ","))
        Else
            objParametriIngresso.SoilDataset_Z_CSC = 0
        End If

        'If IsNumeric(Txt_CalcTot.Text) Then
        '    objParametriIngresso.Caco3 = CDec(Txt_CalcTot.Text.Replace(".", ","))
        'Else
        '    objParametriIngresso.Caco3 = 0
        'End If

        'objParametriIngresso.DisponibilitaOssigeno_Cod = CInt(ddlDispOssigeno.SelectedValue)


        '--------------------
        '   PRATICHE AGRONOMICHE
        '--------------------
        objParametriIngresso.COLH_PrecessioneAnnoPrecedente = 0
        If IsNumeric(Cmb_Specie_Precessione.SelectedValue) Then
            objParametriIngresso.COLH_PrecessioneAnnoPrecedente = CInt(Cmb_Specie_Precessione.SelectedValue)
        End If
        objParametriIngresso.Grfi_Cod_PrecessioneAnnoPrecedente = 0
        If IsNumeric(ddlFinalitaRer_Precessione.SelectedValue) Then
            objParametriIngresso.Grfi_Cod_PrecessioneAnnoPrecedente = CInt(ddlFinalitaRer_Precessione.SelectedValue)
        End If

        objParametriIngresso.COLI_ResiduiPrecessioneAsportati = IIf(Chk_ResiduiPrecessione.Checked = True, True, False)

        If IsNumeric(cmb_PeriodoInterramentoResiduiPrecessione.Value) Then
            objParametriIngresso.COLV_PeriodoInterramentoResiduiPrecessione = cmb_PeriodoInterramentoResiduiPrecessione.Value
        Else
            Exit Sub
        End If

        objParametriIngresso.COLAD_TipoConcimeOrganico = CInt(ddlConcimeOrganico.SelectedValue)

        If IsNumeric(Txt_QtaN_KGHa.Text) Then
            objParametriIngresso.N_KgHa = CInt(Txt_QtaN_KGHa.Text)
        Else
            objParametriIngresso.N_KgHa = 0
        End If

        objParametriIngresso.COLAF_ModalitaDistribuzioneRelativoColturaEpoca = 0
        If IsNumeric(ddlEpocaModalitaDistribuzione.SelectedValue) Then
            objParametriIngresso.COLAF_ModalitaDistribuzioneRelativoColturaEpoca = CInt(ddlEpocaModalitaDistribuzione.SelectedValue)
        End If


        '--------------------
        '   METEO
        '--------------------
        If IsNumeric(Txt_Pioggia.Text) Then
            objParametriIngresso.PiovositaOttobreFebbraio = CDec(Txt_Pioggia.Text.Replace(".", ","))
        Else
            objParametriIngresso.PiovositaOttobreFebbraio = 0
        End If

        If IsNumeric(Txt_AvgTemperatura_ColturaInCampo.Text) Then
            objParametriIngresso.AVG_Temperatura_ColturaInCampo = CDec(Txt_AvgTemperatura_ColturaInCampo.Text.Replace(".", ","))
        Else
            objParametriIngresso.AVG_Temperatura_ColturaInCampo = 0
        End If

        If IsNumeric(Txt_AvgTemperatura_MeseSemina_Febbraio.Text) Then
            objParametriIngresso.AVG_Temperatura_MeseSemina_Febbraio = CDec(Txt_AvgTemperatura_MeseSemina_Febbraio.Text.Replace(".", ","))
        Else
            objParametriIngresso.AVG_Temperatura_MeseSemina_Febbraio = 0
        End If

        If IsNumeric(Txt_PercUmiditaColturaPrincipale.Text) Then
            objParametriIngresso.COLQ_PercUmiditaPrevistaColturaPrincipale = CDec(Txt_PercUmiditaColturaPrincipale.Text.Replace(".", ","))
        Else
            objParametriIngresso.COLQ_PercUmiditaPrevistaColturaPrincipale = 0
        End If

        If IsNumeric(Txt_PercUmiditaRaccoltaPrecessione.Text) Then
            objParametriIngresso.COLK_PercUmiditaRaccoltaPrecessione = CDec(Txt_PercUmiditaRaccoltaPrecessione.Text.Replace(".", ","))
        Else
            objParametriIngresso.COLK_PercUmiditaRaccoltaPrecessione = 0
        End If


        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_output
        Dim objBilancio As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objBilancio.CalcolaBilancio_IBF(objParametriIngresso)

        'necessita
        If objParametriUscita.ListaNecessita.Count > 0 Then

            Dim DT_Necessita As New DataTable
            Dim Dr_Necessita As DataRow
            DT_Necessita.Columns.Add(New DataColumn("Descrizione", GetType(String)))
            DT_Necessita.Columns.Add(New DataColumn("N", GetType(String)))
            'DT_Necessita.Columns.Add(New DataColumn("P", GetType(String)))
            'DT_Necessita.Columns.Add(New DataColumn("K", GetType(String)))

            For i = 0 To objParametriUscita.ListaNecessita.Count - 1
                Dr_Necessita = DT_Necessita.NewRow
                Dr_Necessita.Item("Descrizione") = objParametriUscita.ListaNecessita(i).Descrizione
                Dr_Necessita.Item("N") = Format(objParametriUscita.ListaNecessita(i).Valore_N, "0.00")
                'Dr_Necessita.Item("P") = Format(objParametriUscita.ListaNecessita(i).Valore_P, "0.00")
                'Dr_Necessita.Item("K") = Format(objParametriUscita.ListaNecessita(i).Valore_K, "0.00")
                DT_Necessita.Rows.Add(Dr_Necessita)
            Next

            Dr_Necessita = DT_Necessita.NewRow
            Dr_Necessita.Item("Descrizione") = Resources.PianoConcimazione_2017.TotaleNecessita
            Dr_Necessita.Item("N") = Format(objParametriUscita.N_Necessario, "0.00")
            'Dr_Necessita.Item("P") = Format(objParametriUscita.P_Necessario, "0.00")
            'Dr_Necessita.Item("K") = Format(objParametriUscita.K_Necessario, "0.00")
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
            'DT_Disponibilita.Columns.Add(New DataColumn("P", GetType(String)))
            'DT_Disponibilita.Columns.Add(New DataColumn("K", GetType(String)))

            For i = 0 To objParametriUscita.ListaDisponibilita.Count - 1
                Dr_Disponibilita = DT_Disponibilita.NewRow
                Dr_Disponibilita.Item("Descrizione") = objParametriUscita.ListaDisponibilita(i).Descrizione
                Dr_Disponibilita.Item("N") = Format(objParametriUscita.ListaDisponibilita(i).Valore_N, "0.00")
                'Dr_Disponibilita.Item("P") = Format(objParametriUscita.ListaDisponibilita(i).Valore_P, "0.00")
                'Dr_Disponibilita.Item("K") = Format(objParametriUscita.ListaDisponibilita(i).Valore_K, "0.00")
                DT_Disponibilita.Rows.Add(Dr_Disponibilita)
            Next

            Dr_Disponibilita = DT_Disponibilita.NewRow
            Dr_Disponibilita.Item("Descrizione") = Resources.PianoConcimazione_2017.TotaleDisponibita
            Dr_Disponibilita.Item("N") = Format(objParametriUscita.N_Disponibile, "0.00")
            'Dr_Disponibilita.Item("P") = Format(objParametriUscita.P_Disponibile, "0.00")
            'Dr_Disponibilita.Item("K") = Format(objParametriUscita.K_Disponibile, "0.00")
            DT_Disponibilita.Rows.Add(Dr_Disponibilita)


            Dr_Disponibilita = DT_Disponibilita.NewRow
            Dr_Disponibilita.Item("Descrizione") = Resources.PianoConcimazione_2017.BisognoCalcolato
            Dr_Disponibilita.Item("N") = Format(objParametriUscita.N_Ammesso, "0")
            'Dr_Disponibilita.Item("P") = Format(objParametriUscita.P_Calcolato, "0.00")
            'Dr_Disponibilita.Item("K") = Format(objParametriUscita.K_Calcolato, "0.00")
            DT_Disponibilita.Rows.Add(Dr_Disponibilita)


            'Dr_Disponibilita = DT_Disponibilita.NewRow
            'Dr_Disponibilita.Item("Descrizione") = Resources.PianoConcimazione_2017.ApportoAmmessoBilancio
            'Dr_Disponibilita.Item("N") = Format(objParametriUscita.N_Ammesso, "0.00")
            ''Dr_Disponibilita.Item("P") = Format(objParametriUscita.P_Ammesso, "0.00")
            ''Dr_Disponibilita.Item("K") = Format(objParametriUscita.K_Ammesso, "0.00")
            'DT_Disponibilita.Rows.Add(Dr_Disponibilita)

            GrdDisponibilita.DataSource = DT_Disponibilita
            GrdDisponibilita.DataBind()

            GrdDisponibilita.Rows(GrdDisponibilita.Rows.Count - 1).Font.Bold = True

        End If

        N_Ammesso.Value = Format(objParametriUscita.N_Ammesso, "0")
        'K_Ammesso.Value = objParametriUscita.K_Ammesso
        'P_Ammesso.Value = objParametriUscita.P_Ammesso


        '(09/03/2020 fede) aggiunta eventuale considerazione del fattore correttivo per resa maggiore
        Dim MAS As Decimal = -1 '0
        Dim strFattoreCorrettivo As String = ""

        'MAS = objParametriUscita.Limite_Mas
        'MAS_Dir_Nitati = objParametriUscita.Limite_Mas_Dir_Nitrati

        If IsNumeric(ddlMasB.SelectedItem.Value) = True Then
            MAS = CDec(ddlMasB.SelectedItem.Value)
        End If

        LblAttenzione.Text = ""

        If MAS >= 0 Then

            If objParametriUscita.FattoreCorrettivo_N > 0 AndAlso ddlMasB.SelectedIndex <= 0 Then
                If objParametriIngresso.COLP_ResaUtilePrevistaColturaPrincipale_THa > 0 AndAlso objParametriUscita.Resa_Rif > 0 Then
                    If objParametriIngresso.COLP_ResaUtilePrevistaColturaPrincipale_THa - objParametriUscita.Resa_Rif > 0 Then
                        MAS = MAS + ((objParametriIngresso.COLP_ResaUtilePrevistaColturaPrincipale_THa - objParametriUscita.Resa_Rif) * CDec(Format(objParametriUscita.FattoreCorrettivo_N, "###,###.###"))) 'objParametriUscita.FattoreCorrettivo_N)
                        strFattoreCorrettivo = " (" & String.Format(Resources.PianoConcimazione_2017.ConsiderandoFattoreCorrettivoKgNt, Format(objParametriUscita.FattoreCorrettivo_N, "###,###.###")) & ")"
                    End If
                End If
            End If

            If objParametriUscita.N_Ammesso > MAS Then
                LblAttenzione.Text = String.Format(Resources.PianoConcimazione_2017.ATTENZIONEApportoNNonPuoSuperareLimiteMAS, MAS.ToString) & " " & strFattoreCorrettivo
                N_Ammesso.Value = MAS
            End If

        End If

    End Sub

#End Region


#Region "CALCOLA SCHEDE"
    Private Sub CalcolaSchede(ByVal Dt_Fattori_Sel As DataTable)

        Dim Regolamento_Cod As Integer = CInt(ddlRegolamento.SelectedValue)

        If Not IsNumeric(Cmb_Specie_ColturaPrincipale.SelectedValue) Then
            Exit Sub
        End If

        Dim Resa As Decimal

        If Not IsNumeric(Txt_Resa.Text) Then
            AgroMsgBox(Resources.PianoConcimazione_2017.IndicareResa, Page, , True)
            Exit Sub
        Else
            Resa = CDec(Txt_Resa.Text.Replace(".", ","))
        End If

        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS

        ' MAS

        'Txt_MAS.Text = "n.d"

        Dim MAS As Decimal = 0

        LabelMAS.Text = Resources.PianoConcimazione_2017.LimiteMAS
        LabelMAS_Nota.Text = ""


        '(06/03/2020 fede) aggiunta eventuale considerazione del fattore correttivo per resa maggiore
        ' il fattore correttivo viene considerato solo se si sceglie il MAS del piano (non della direttiva notrati)
        If IsNumeric(ddlMasS.SelectedItem.Value) Then
            'If IsNumeric(N_MAS.Value) Then
            'MAS = CDec(N_MAS.Value)
            MAS = CDec(ddlMasS.SelectedItem.Value)
            If IsNumeric(Fattore_Correttivo_N_Resa.Value) AndAlso CDec(Fattore_Correttivo_N_Resa.Value) > 0 AndAlso ddlMasS.SelectedIndex <= 0 Then
                If IsNumeric(Txt_Resa.Text) AndAlso IsNumeric(Lbl_ResaRiferimento.InnerText) Then
                    If CDec(Txt_Resa.Text.Replace(".", ",")) - CDec(Lbl_ResaRiferimento.InnerText) > 0 Then
                        MAS = MAS + ((CDec(Txt_Resa.Text.Replace(".", ",")) - CDec(Lbl_ResaRiferimento.InnerText)) * CDec(Fattore_Correttivo_N_Resa.Value))
                        LabelMAS.Text = LabelMAS.Text & " *"
                        LabelMAS_Nota.Text = "* " & " (" & String.Format(Resources.PianoConcimazione_2017.ConsiderandoFattoreCorrettivoKgNt, Fattore_Correttivo_N_Resa.Value) & ")"
                    End If
                End If
            End If
            'Txt_MAS.Text = MAS
        End If


        ' FATTORI CORRETTIVI

        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input

        objParametriIngresso.Regolamento_Cod = Regolamento_Cod
        objParametriIngresso.Veg_Cod = CInt(Cmb_Specie_ColturaPrincipale.SelectedValue)
        objParametriIngresso.Grfi_Cod = 0
        If IsNumeric(ddlFinalitaRer_ColturaPrincipale.SelectedValue) Then
            objParametriIngresso.Grfi_Cod = CInt(ddlFinalitaRer_ColturaPrincipale.SelectedValue)
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

        ddlDoseP.Items.Clear()
        ddlDoseK.Items.Clear()

        Txt_DoseStandard.Text = 0
        Txt_DoseStandardP.Text = 0
        Txt_DoseStandardK.Text = 0
        Txt_DoseRicalcolata.Text = 0
        Txt_DoseRicalcolataP.Text = 0
        Txt_DoseRicalcolataK.Text = 0

        'necessita
        If objParametriUscita.ListaFattoriCorrettivi.Count > 0 Then

            ResaBassaDes.Value = ""
            ResaAltaDes.Value = ""

            Dim FattoreResaBassa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            FattoreResaBassa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.Resa_Bassa And x.Visibile = 1)(0)
            If Not FattoreResaBassa Is Nothing Then
                ResaBassaDes.Value = FattoreResaBassa.Descrizione '& " " & FattoreResaBassa.Valore
            End If

            Dim FattoreResaAlta As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            FattoreResaAlta = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.Resa_Alta And x.Visibile = 1)(0)
            If Not FattoreResaAlta Is Nothing Then
                ResaAltaDes.Value = FattoreResaAlta.Descrizione '& " " & FattoreResaAlta.Valore
                ResaAlta.Value = FattoreResaAlta.Valore
            End If

            'DOSI STANDARD

            'Select Case ddlFaseCiclo.SelectedValue

            '    Case enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto
            '        'rimangono = 0

            '    Case enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento

            '        Dim Fattore_N_I_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            '        Fattore_N_I_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_I_anno_allevamento)(0)
            '        If Not Fattore_N_I_Anno_Allevamento Is Nothing Then
            '            Txt_DoseStandard.Text = Fattore_N_I_Anno_Allevamento.Valore
            '        End If

            '        Dim Fattore_P_I_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            '        Fattore_P_I_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_I_anno_allevamento)(0)
            '        If Not Fattore_P_I_Anno_Allevamento Is Nothing Then
            '            Txt_DoseStandardP.Text = Fattore_P_I_Anno_Allevamento.Valore
            '        End If

            '        Dim Fattore_K_I_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            '        Fattore_K_I_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_I_anno_allevamento)(0)
            '        If Not Fattore_K_I_Anno_Allevamento Is Nothing Then
            '            Txt_DoseStandardK.Text = Fattore_K_I_Anno_Allevamento.Valore
            '        End If


            '    Case enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento

            '        Dim Fattore_N_II_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            '        Fattore_N_II_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_II_anno_allevamento)(0)
            '        If Not Fattore_N_II_Anno_Allevamento Is Nothing Then
            '            Txt_DoseStandard.Text = Fattore_N_II_Anno_Allevamento.Valore
            '        End If

            '        Dim Fattore_P_II_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            '        Fattore_P_II_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_II_anno_allevamento)(0)
            '        If Not Fattore_P_II_Anno_Allevamento Is Nothing Then
            '            Txt_DoseStandardP.Text = Fattore_P_II_Anno_Allevamento.Valore
            '        End If

            '        Dim Fattore_K_II_Anno_Allevamento As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            '        Fattore_K_II_Anno_Allevamento = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_II_anno_allevamento)(0)
            '        If Not Fattore_K_II_Anno_Allevamento Is Nothing Then
            '            Txt_DoseStandardK.Text = Fattore_K_II_Anno_Allevamento.Valore
            '        End If

            '    Case Else

            '        Dim Fattore_N_Dose_Standard As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            '        Fattore_N_Dose_Standard = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_Dose_Standard)(0)
            '        If Not Fattore_N_Dose_Standard Is Nothing Then
            '            Txt_DoseStandard.Text = Fattore_N_Dose_Standard.Valore
            '        End If

            '        Dim Lista_Dose_P As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            '        Lista_Dose_P = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "P" And UCase(x.Variazione) = "STANDARD" And x.Visibile = 1).ToList
            '        For i = 0 To Lista_Dose_P.Count - 1
            '            ddlDoseP.Items.Add(New ListItem(Lista_Dose_P(i).Descrizione, Lista_Dose_P(i).Codice & "|" & Lista_Dose_P(i).Valore))
            '        Next
            '        If Not ddlDoseP Is Nothing AndAlso ddlDoseP.Items.Count > 0 Then
            '            Txt_DoseStandardP.Text = ddlDoseP.SelectedValue.Split("|")(1)
            '        End If

            '        Dim Lista_Dose_K As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            '        Lista_Dose_K = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "K" And UCase(x.Variazione) = "STANDARD" And x.Visibile = 1).ToList
            '        For i = 0 To Lista_Dose_K.Count - 1
            '            ddlDoseK.Items.Add(New ListItem(Lista_Dose_K(i).Descrizione, Lista_Dose_K(i).Codice & "|" & Lista_Dose_K(i).Valore))
            '        Next
            '        If Not ddlDoseK Is Nothing AndAlso ddlDoseK.Items.Count > 0 Then
            '            Txt_DoseStandardK.Text = ddlDoseK.SelectedValue.Split("|")(1)
            '        End If

            'End Select

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
                InserisciRigaDt(ResaAltaDes.Value & " " & Fattore_N_Inc_Resa.Controllo_Valore, Fattore_N_Inc_Resa.Codice, Fattore_N_Inc_Resa.Valore, Fattore_N_Inc_Resa.Controllo_Funzione, Fattore_N_Inc_Resa.Controllo_Valore, Dt_Incrementi_N)
            End If

            Dim Lista_Incrementi_N As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Incrementi_N = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "N" And UCase(x.Variazione) = "INCREMENTO" And x.Visibile = 1).ToList
            For i = 0 To Lista_Incrementi_N.Count - 1
                InserisciRigaDt(Lista_Incrementi_N(i).Descrizione, Lista_Incrementi_N(i).Codice, Lista_Incrementi_N(i).Valore, Lista_Incrementi_N(i).Controllo_Funzione, Lista_Incrementi_N(i).Controllo_Valore, Dt_Incrementi_N)
            Next

            Dim Fattore_N_Dec_Resa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_N_Dec_Resa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_Dec_Resa)(0)
            If Not Fattore_N_Dec_Resa Is Nothing Then
                InserisciRigaDt(ResaBassaDes.Value & " " & Fattore_N_Dec_Resa.Controllo_Valore, Fattore_N_Dec_Resa.Codice, Fattore_N_Dec_Resa.Valore, Fattore_N_Dec_Resa.Controllo_Funzione, Fattore_N_Dec_Resa.Controllo_Valore, Dt_Decrementi_N)
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
                InserisciRigaDt(ResaAltaDes.Value & " " & Fattore_P_Inc_Resa.Controllo_Valore, Fattore_P_Inc_Resa.Codice, Fattore_P_Inc_Resa.Valore, Fattore_P_Inc_Resa.Controllo_Funzione, Fattore_P_Inc_Resa.Controllo_Valore, Dt_Incrementi_P)
            End If

            Dim Lista_Incrementi_P As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Incrementi_P = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "P" And UCase(x.Variazione) = "INCREMENTO" And x.Visibile = 1).ToList
            For i = 0 To Lista_Incrementi_P.Count - 1
                InserisciRigaDt(Lista_Incrementi_P(i).Descrizione, Lista_Incrementi_P(i).Codice, Lista_Incrementi_P(i).Valore, Lista_Incrementi_P(i).Controllo_Funzione, Lista_Incrementi_P(i).Controllo_Valore, Dt_Incrementi_P)
            Next

            Dim Fattore_P_Dec_Resa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_P_Dec_Resa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_Dec_Resa)(0)
            If Not Fattore_P_Dec_Resa Is Nothing Then
                InserisciRigaDt(ResaBassaDes.Value & " " & Fattore_P_Dec_Resa.Controllo_Valore, Fattore_P_Dec_Resa.Codice, Fattore_P_Dec_Resa.Valore, Fattore_P_Dec_Resa.Controllo_Funzione, Fattore_P_Dec_Resa.Controllo_Valore, Dt_Decrementi_P)
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
                InserisciRigaDt(ResaAltaDes.Value & " " & Fattore_K_Inc_Resa.Controllo_Valore, Fattore_K_Inc_Resa.Codice, Fattore_K_Inc_Resa.Valore, Fattore_K_Inc_Resa.Controllo_Funzione, Fattore_K_Inc_Resa.Controllo_Valore, Dt_Incrementi_K)
            End If

            Dim Lista_Incrementi_K As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Incrementi_K = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = "K" And UCase(x.Variazione) = "INCREMENTO" And x.Visibile = 1).ToList
            For i = 0 To Lista_Incrementi_K.Count - 1
                InserisciRigaDt(Lista_Incrementi_K(i).Descrizione, Lista_Incrementi_K(i).Codice, Lista_Incrementi_K(i).Valore, Lista_Incrementi_K(i).Controllo_Funzione, Lista_Incrementi_K(i).Controllo_Valore, Dt_Incrementi_K)
            Next

            Dim Fattore_K_Dec_Resa As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
            Fattore_K_Dec_Resa = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_Dec_Resa)(0)
            If Not Fattore_K_Dec_Resa Is Nothing Then
                InserisciRigaDt(ResaBassaDes.Value & " " & Fattore_K_Dec_Resa.Controllo_Valore, Fattore_K_Dec_Resa.Codice, Fattore_K_Dec_Resa.Valore, Fattore_K_Dec_Resa.Controllo_Funzione, Fattore_K_Dec_Resa.Controllo_Valore, Dt_Decrementi_K)
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


            'Select Case ddlFaseCiclo.SelectedValue

            '    Case enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto,
            '         enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento,
            '         enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento

            '    Case Else

            '        ' se sono in modifica seleziono i fattori salvati
            '        If Not Dt_Fattori_Sel Is Nothing Then

            '            ' azoto
            '            Dim totIncN As Decimal = ImpostaFattoriSelezionati(grdIncrementi, "Chk_SelIncrementi_N", Dt_Fattori_Sel)

            '            Dim totDecN As Decimal = ImpostaFattoriSelezionati(grdDecrementi, "Chk_SelDecrementi_N", Dt_Fattori_Sel)

            '            Txt_TotDecrementi.Text = totDecN.ToString
            '            Txt_TotIncrementi.Text = totIncN.ToString

            '            'piano nuovo NPK sono nella tabella dei dettagli (già letto sopra)
            '            If IsNumeric(N_Ammesso.Value) Then
            '                Txt_DoseRicalcolata.Text = CDec(N_Ammesso.Value)
            '            Else
            '                Dim doseRicN As Decimal
            '                Dim doseStd As Decimal = Txt_DoseStandard.Text
            '                'Dim doseMas As Decimal = IIf(IsNumeric(Txt_MAS.Text), Txt_MAS.Text, 0)
            '                Dim doseMas As Decimal = 0
            '                If IsNumeric(ddlMasS.SelectedItem.Value) Then
            '                    doseMas = CDec(ddlMasS.SelectedItem.Value)
            '                End If

            '                Dim maxIncr As Decimal = Txt_MaxIncrementi.Text

            '                doseRicN = doseStd + totIncN - totDecN

            '                If doseRicN > doseStd + maxIncr Then
            '                    doseRicN = doseStd + maxIncr
            '                End If

            '                If doseMas <> 0 Then
            '                    If doseRicN > doseMas Then
            '                        doseRicN = doseMas
            '                    End If
            '                End If

            '                Txt_DoseRicalcolata.Text = doseRicN.ToString
            '            End If

            '            ' fosforo
            '            Dim totIncP As Decimal = ImpostaFattoriSelezionati(grdIncrementiP, "Chk_SelIncrementi_P", Dt_Fattori_Sel)

            '            Dim totDecP As Decimal = ImpostaFattoriSelezionati(grdDecrementiP, "Chk_SelDecrementi_P", Dt_Fattori_Sel)

            '            Txt_TotDecrementiP.Text = totDecP.ToString
            '            Txt_TotIncrementiP.Text = totIncP.ToString

            '            ImpostaFattoriSelezionati(ddlDoseP, Txt_DoseStandardP, Dt_Fattori_Sel)

            '            If IsNumeric(P_Ammesso.Value) Then
            '                Txt_DoseRicalcolataP.Text = CDec(P_Ammesso.Value)
            '            Else
            '                Dim doseRicP As Decimal
            '                Dim doseStdP As Decimal = Txt_DoseStandardP.Text
            '                doseRicP = doseStdP + totIncP - totDecP
            '                Txt_DoseRicalcolataP.Text = doseRicP.ToString
            '            End If

            '            ' potassio
            '            Dim totIncK As Decimal = ImpostaFattoriSelezionati(grdIncrementiK, "Chk_SelIncrementi_K", Dt_Fattori_Sel)

            '            Dim totDecK As Decimal = ImpostaFattoriSelezionati(grdDecrementiK, "Chk_SelDecrementi_K", Dt_Fattori_Sel)

            '            Txt_TotDecrementiK.Text = totDecK.ToString
            '            Txt_TotIncrementiK.Text = totIncK.ToString

            '            ImpostaFattoriSelezionati(ddlDoseK, Txt_DoseStandardK, Dt_Fattori_Sel)
            '            If IsNumeric(K_Ammesso.Value) Then
            '                Txt_DoseRicalcolataK.Text = CDec(K_Ammesso.Value)
            '            Else
            '                Dim doseRicK As Decimal
            '                Dim doseStdK As Decimal = Txt_DoseStandardK.Text
            '                doseRicK = doseStdK + totIncK - totDecK
            '                Txt_DoseRicalcolataK.Text = doseRicK.ToString
            '            End If


            '        Else

            '            ' N

            '            Dim doseStd As Decimal = Txt_DoseStandard.Text

            '            Dim doseMas As Decimal = 0
            '            If IsNumeric(ddlMasS.SelectedItem.Value) Then
            '                doseMas = CDec(ddlMasS.SelectedItem.Value)
            '            End If


            '            Dim maxIncr As Decimal = Txt_MaxIncrementi.Text

            '            Dim totIncN As Decimal = ImpostaFattoriDefault(grdIncrementi, "Chk_SelIncrementi_N")
            '            Dim totDecN As Decimal = ImpostaFattoriDefault(grdDecrementi, "Chk_SelDecrementi_N")

            '            Txt_TotDecrementi.Text = totDecN.ToString
            '            Txt_TotIncrementi.Text = totIncN.ToString

            '            Dim doseRicN As Decimal
            '            doseRicN = doseStd + totIncN - totDecN

            '            If doseRicN > doseStd + maxIncr Then
            '                doseRicN = doseStd + maxIncr
            '            End If

            '            If doseMas <> 0 Then
            '                If doseRicN > doseMas Then
            '                    doseRicN = doseMas
            '                End If
            '            End If

            '            Txt_DoseRicalcolata.Text = doseRicN.ToString

            '            ' P
            '            ImpostaDoseStandard_P2O5_Default()
            '            Dim doseStdP As Decimal = Txt_DoseStandardP.Text

            '            Dim totIncP As Decimal = ImpostaFattoriDefault(grdIncrementiP, "Chk_SelIncrementi_P")
            '            Dim totDecP As Decimal = ImpostaFattoriDefault(grdDecrementiP, "Chk_SelDecrementi_P")

            '            Txt_TotDecrementiP.Text = totDecP.ToString
            '            Txt_TotIncrementiP.Text = totIncP.ToString

            '            Dim doseRicP As Decimal
            '            doseRicP = doseStdP + totIncP - totDecP
            '            Txt_DoseRicalcolataP.Text = doseRicP.ToString

            '            ' K
            '            ImpostaDoseStandard_K2O_Default()
            '            Dim doseStdk As Decimal = Txt_DoseStandardK.Text

            '            Dim totIncK As Decimal = ImpostaFattoriDefault(grdIncrementiK, "Chk_SelIncrementi_K")
            '            Dim totDecK As Decimal = ImpostaFattoriDefault(grdDecrementiK, "Chk_SelDecrementi_K")

            '            Txt_TotDecrementiK.Text = totDecK.ToString
            '            Txt_TotIncrementiK.Text = totIncK.ToString

            '            Dim doseRicK As Decimal
            '            doseRicK = doseStdk + totIncK - totDecK
            '            Txt_DoseRicalcolataK.Text = doseRicK.ToString

            '        End If
            'End Select
        End If

        'valorizare
        N_Ammesso.Value = Txt_DoseRicalcolata.Text
        P_Ammesso.Value = Txt_DoseRicalcolataP.Text
        K_Ammesso.Value = Txt_DoseRicalcolataK.Text

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

    Public Function CalcAtt_Elevato() As Boolean

        Dim objParametriIngressoSuolo As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Suolo_input
        objParametriIngressoSuolo.CalcAtt = CDec(Txt_CalcAtt.Text.Replace(".", ","))
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

    Public Function ImpostaDoseStandard_P2O5_Default()

        Dim objParametriIngressoSuolo As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Suolo_input

        objParametriIngressoSuolo.Regolamento_Cod = CInt(ddlRegolamento.SelectedValue)
        Select Case DDL_P2O5.SelectedValue
            Case "0"
                objParametriIngressoSuolo.P2O5 = CDec(Txt_P.Text.Replace(".", ","))
            Case Else
                objParametriIngressoSuolo.P2O5 = CDec(Txt_P.Text.Replace(".", ",")) * Fattore_Conversione_P_P2O5
        End Select

        Dim objParametriUscitaSuolo As enum_PianoConcimazione_Dotazione
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscitaSuolo = objPC_WS.P2O5_Dotazione(objParametriIngressoSuolo)

        Select Case objParametriUscitaSuolo

            Case enum_PianoConcimazione_Dotazione.MoltoBassa
                For j = 0 To ddlDoseP.Items.Count - 1
                    If ddlDoseP.Items(j).Value.Split("|")(0) = enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard_MoltoBassa Then
                        ddlDoseP.SelectedIndex = j
                        Txt_DoseStandardP.Text = ddlDoseP.SelectedValue.Split("|")(1)
                        Exit For
                    End If
                Next

            Case enum_PianoConcimazione_Dotazione.Bassa
                For j = 0 To ddlDoseP.Items.Count - 1
                    If ddlDoseP.Items(j).Value.Split("|")(0) = enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard_Bassa Then
                        ddlDoseP.SelectedIndex = j
                        Txt_DoseStandardP.Text = ddlDoseP.SelectedValue.Split("|")(1)
                        Exit For
                    End If
                Next

                '(05/03/2019 fede) spostato giudizio elevato su dotazione normale
            Case enum_PianoConcimazione_Dotazione.Media, enum_PianoConcimazione_Dotazione.Elevata
                For j = 0 To ddlDoseP.Items.Count - 1
                    If ddlDoseP.Items(j).Value.Split("|")(0) = enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard Then
                        ddlDoseP.SelectedIndex = j
                        Txt_DoseStandardP.Text = ddlDoseP.SelectedValue.Split("|")(1)
                        Exit For
                    End If
                Next

            Case enum_PianoConcimazione_Dotazione.MoltoElevata
                For j = 0 To ddlDoseP.Items.Count - 1
                    If ddlDoseP.Items(j).Value.Split("|")(0) = enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard_Elevata Then
                        ddlDoseP.SelectedIndex = j
                        Txt_DoseStandardP.Text = ddlDoseP.SelectedValue.Split("|")(1)
                        Exit For
                    End If
                Next

        End Select

    End Function

    Public Function ImpostaDoseStandard_K2O_Default()

        Dim objParametriIngressoSuolo As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Suolo_input
        objParametriIngressoSuolo.K2O = CDec(Txt_K.Text.Replace(".", ","))
        objParametriIngressoSuolo.Sabbia = CDec(Txt_Sabbia.Text.Replace(".", ","))
        objParametriIngressoSuolo.Argilla = CDec(Txt_Argilla.Text.Replace(".", ","))

        objParametriIngressoSuolo.Mg = 0
        objParametriIngressoSuolo.CSC = 0

        objParametriIngressoSuolo.Regolamento_Cod = CInt(ddlRegolamento.SelectedValue)

        Select Case CInt(ddlRegolamento.SelectedValue)
            Case Is >= enum_PUARegolamenti.PianoConcimazione_2017
                If IsNumeric(Txt_Mg.Text) Then
                    objParametriIngressoSuolo.Mg = CDec(Txt_Mg.Text.Replace(".", ","))
                End If
                If IsNumeric(Txt_CSC.Text) Then
                    objParametriIngressoSuolo.CSC = CDec(Txt_CSC.Text.Replace(".", ","))
                End If
        End Select

        Dim objParametriUscitaSuolo As enum_PianoConcimazione_Dotazione
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscitaSuolo = objPC_WS.K2O_Dotazione(objParametriIngressoSuolo)

        Select Case objParametriUscitaSuolo

            Case enum_PianoConcimazione_Dotazione.MoltoBassa, enum_PianoConcimazione_Dotazione.Bassa
                For j = 0 To ddlDoseK.Items.Count - 1
                    If ddlDoseK.Items(j).Value.Split("|")(0) = enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard_Bassa Then
                        ddlDoseK.SelectedIndex = j
                        Txt_DoseStandardK.Text = ddlDoseK.SelectedValue.Split("|")(1)
                        Exit For
                    End If
                Next


            Case enum_PianoConcimazione_Dotazione.Media
                For j = 0 To ddlDoseK.Items.Count - 1
                    If ddlDoseK.Items(j).Value.Split("|")(0) = enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard Then
                        ddlDoseK.SelectedIndex = j
                        Txt_DoseStandardK.Text = ddlDoseK.SelectedValue.Split("|")(1)
                        Exit For
                    End If
                Next

            Case enum_PianoConcimazione_Dotazione.Elevata, enum_PianoConcimazione_Dotazione.MoltoElevata
                For j = 0 To ddlDoseK.Items.Count - 1
                    If ddlDoseK.Items(j).Value.Split("|")(0) = enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard_Elevata Then
                        ddlDoseK.SelectedIndex = j
                        Txt_DoseStandardK.Text = ddlDoseK.SelectedValue.Split("|")(1)
                        Exit For
                    End If
                Next

        End Select

    End Function

#End Region


#Region "ANALISI"
    Private Function XML_GeneraStringoneFinale() As String

        '------------------------------------------------
        '----- Definizione delle Variabili
        '------------------------------------------------

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlDoc1 As New System.Xml.XmlDocument
        Dim XmlDoc2 As New System.Xml.XmlDocument
        Dim XmlDoc3 As New System.Xml.XmlDocument
        Dim XmlDoc4 As New System.Xml.XmlDocument

        Dim XML_DatiTestate As System.Xml.XmlElement
        Dim XML_Testata As System.Xml.XmlElement
        Dim XML_DatiAnalisiEntitaxTestate As System.Xml.XmlElement
        Dim XML_DatiDettagli As System.Xml.XmlElement
        Dim XML_Dettaglio As System.Xml.XmlElement


        Dim str_DatiDettagli As String = ""
        Dim str_Dettaglio As String

        Dim Operazione As Integer
        Dim Dettaglio_Cod As String

        Dim DataInizio As Date
        Dim DataFine As Date

        Dim xPiva As String
        Dim xSa_Cod As Integer
        Dim xCampo_Cod As Integer
        Dim xAppezza As Integer
        Dim xID_Imp As Integer
        Dim xFabbricato_Cod As Integer

        Dim xCodProvincia As String
        Dim xCodComune As String
        Dim xSezione As String
        Dim xFoglio As Integer
        Dim xNumero As Integer
        Dim xSubalterno As String

        Dim xEntita_Cod As Integer

        Dim BaseCode As Integer
        Dim TopCode As Integer

        Dim DtDettagli As New DataTable
        Dim DtDettagliShort As New DataTable
        Dim DtCampioni As New DataTable

        Dim Analisi_Id_Agenda As Integer

        Dim Certificato_Cod As Integer


        '------------------------------------------------
        '----- Genero la struttura XML
        '------------------------------------------------

        XML_DatiTestate = XmlDoc.CreateElement("DatiTestate")

        If Me.Txt_ValiditaInizio.Text = "" Then
            DataInizio = CDate("01/01/1900")
        Else
            DataInizio = CDate(Me.Txt_ValiditaInizio.Text)
        End If

        If Me.Txt_ValiditaFine.Text = "" Then
            DataFine = CDate("31/12/2100")
        Else
            DataFine = CDate(Me.Txt_ValiditaFine.Text)
        End If

        Call Calcola_BaseCode_TopCode(BaseCode,
                                      TopCode,
                                      Session("ASG_ProgressivoGIAS"))

        Dim Analisi_Des As String = Txt_AnalisiDes.Text
        '------------------------------------------------
        '------------------------------------------------
        '----- NUOVA VERSIONE --> gestione dei CERTIFICATI
        '------------------------------------------------
        '------------------------------------------------
        '----- Verifico se è stato inserito :
        '----- 1. NUMERO del CERTIFICATO
        '----- 2. LABORATORIO DI ANALISI
        '----- In tal caso creo il record del certificato
        '----- nella tabella Analisi_Certificato
        '----- poi salvo in Analisi_testata il codice del certificato
        '------------------------------------------------
        '------------------------------------------------

        Dim Sabbia As Decimal = Txt_Sabbia.Text
        Dim Argilla As Decimal = Txt_Argilla.Text

        'Ricavo la classe tessitura
        Dim Id_ClasseTessitura As Integer = 0

        If Sabbia <> -99 AndAlso Argilla <> -99 Then
            'TODO: chiama WS per ricavare id_tessitura

            Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_input With {
                .Argilla = Argilla,
                .Sabbia = Sabbia
            }

            Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_output
            Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
            objParametriUscita = objPC_WS.ClassiTessituraDaAnalisi(objParametriIngresso)

            If Not objParametriUscita Is Nothing AndAlso
               Not objParametriUscita.ListaClassiTessitura Is Nothing AndAlso
               objParametriUscita.ListaClassiTessitura.Count = 1 Then
                Id_ClasseTessitura = objParametriUscita.ListaClassiTessitura(0).Id_ClasseTessitura
            End If

        End If


        '------------------------------------------------
        '----- Analisi Testata
        '------------------------------------------------

        'viene valorizzato solo nelle analisi del vino per la tracciabilità
        Analisi_Id_Agenda = 0
        '----- DatiTestate

        XML_DatiTestate.InnerXml = XML_Analisi_Testata(CInt(enum_TipoOperazioneDB.Scrittura),
                                                       Session("ASG_SuperUser_CodFiscale"),
                                                       CInt(0),
                                                       CInt(0),
                                                       Analisi_Des,
                                                       DataInizio,
                                                       DataFine,
                                                       ,
                                                       ,
                                                       "",
                                                      "",
                                                       , , ,
                                                       "",
                                                       "",
                                                       "",
                                                       "",
                                                       enum_AnalisiTipo.Analisi_Terreno,
                                                       Analisi_Id_Agenda,
                                                       Now.Today,
                                                        ,
                                                       BaseCode,
                                                       TopCode,
                                                       Id_ClasseTessitura)


        '---------------------------------------------
        '----- Certificato

        Dim strCertificato As String
        Dim Operazione_Certificato As Integer

        Operazione_Certificato = CInt(enum_TipoOperazioneDB.Scrittura)

        'Il certificato non è obbligatorio...
        'se sono in inserimento OK
        'se sono in modifica
        'se esisteva lo modifico
        'altr lo creo

        Certificato_Cod = 0

        Dim LaboratorioCod As Integer = 0
        Dim SchemaCod As String = "-1"

        strCertificato = XML_Analisi_Certificato(Operazione_Certificato,
                                        Session("ASG_SuperUser_CodFiscale"),
                                        Certificato_Cod,
                                        "",
                                        Now.Today,
                                        ,
                                        LaboratorioCod,
                                        SchemaCod,
                                            , , , , , , , , , , , , , ,
                                        BaseCode,
                                        TopCode)

        XML_DatiTestate.InnerXml &= strCertificato

        '---------------------------------------------

        XML_Testata = XML_DatiTestate.SelectSingleNode("Testata")

        '----- DatiAnalisi_EntitaxCertificati

        XML_DatiAnalisiEntitaxTestate = XmlDoc.CreateElement("DatiAnalisi_EntitaxTestata")

        XML_Testata.AppendChild(XML_DatiAnalisiEntitaxTestate)

        xPiva = Qs_Piva
        xSa_Cod = CInt(Cmb_Centro.SelectedValue)
        xCampo_Cod = 0
        xAppezza = 0
        xID_Imp = 0
        xFabbricato_Cod = 0
        xCodProvincia = ""
        xCodComune = ""
        xSezione = "0"
        xFoglio = 0
        xNumero = 0
        xSubalterno = "0"

        xEntita_Cod = IIf(xSa_Cod = 0, 1, 2) ' 1 = liv. aziendale 2 = liv. centro

        XML_DatiAnalisiEntitaxTestate.InnerXml = XML_Analisi_EntitaxTestata(TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                            Session("ASG_SuperUser_CodFiscale"),
                                                                            0,
                                                                            xEntita_Cod,
                                                                            xPiva,
                                                                            xSa_Cod,
                                                                            xCampo_Cod,
                                                                            xAppezza,
                                                                            xID_Imp,
                                                                            xFabbricato_Cod,
                                                                            xCodProvincia,
                                                                            xCodComune,
                                                                            xSezione,
                                                                            xFoglio,
                                                                            xNumero,
                                                                            xSubalterno,
                                                                                ,
                                                                            Now.Today)





        '----- DatiDettagli

        XML_DatiDettagli = XmlDoc.CreateElement("DatiDettagli")

        XML_Testata.AppendChild(XML_DatiDettagli)

        '    'Creo il nodo con Dettaglio_Cod = 0 per agganciare i Campioni direttamente alla testata
        '    'è stato convenuto di passare al componente Dettaglio_Cod=""
        '    '(lo creo solo se sono in scrittura)

        str_Dettaglio = XML_Analisi_Dettaglio(TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                              Session("ASG_SuperUser_CodFiscale"),
                                              CInt(0),
                                              "")

        XmlDoc1.LoadXml(str_Dettaglio)

        XML_Dettaglio = XmlDoc1.SelectSingleNode("Dettaglio")

        '----- Campioni  associati direttamente alla Testata (con Dettaglio_Cod = 0)

        Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
        'Dettaglio_Cod = 0
        'è stato modificato il componente, per gli inserimenti bisogna passare -1
        Dettaglio_Cod = -1


        'Sabbia
        str_Dettaglio = XML_Analisi_Dettaglio(Operazione,
                                              Session("ASG_SuperUser_CodFiscale"),
                                              CInt(0),
                                              Dettaglio_Cod,
                                              CInt(enum_AnalisiParametri.AnalisiParametri_Sabbia),
                                              CDbl(Sabbia),
                                              CDbl(0))

        XmlDoc3.LoadXml(str_Dettaglio)

        str_DatiDettagli = str_DatiDettagli + XmlDoc3.OuterXml

        Dim Limo As Decimal = Txt_Limo.Text
        str_Dettaglio = XML_Analisi_Dettaglio(Operazione,
                                              Session("ASG_SuperUser_CodFiscale"),
                                              CInt(0),
                                              Dettaglio_Cod,
                                              CInt(enum_AnalisiParametri.AnalisiParametri_Limo),
                                              CDbl(Limo),
                                              CDbl(0))

        XmlDoc3.LoadXml(str_Dettaglio)

        str_DatiDettagli = str_DatiDettagli + XmlDoc3.OuterXml

        'Argilla
        str_Dettaglio = XML_Analisi_Dettaglio(Operazione,
                                              Session("ASG_SuperUser_CodFiscale"),
                                              CInt(0),
                                              Dettaglio_Cod,
                                              CInt(enum_AnalisiParametri.AnalisiParametri_Argilla),
                                              CDbl(Argilla),
                                              CDbl(0))

        XmlDoc3.LoadXml(str_Dettaglio)

        str_DatiDettagli = str_DatiDettagli + XmlDoc3.OuterXml

        Dim Ph As Decimal = Txt_PH.Text
        str_Dettaglio = XML_Analisi_Dettaglio(Operazione,
                                              Session("ASG_SuperUser_CodFiscale"),
                                              CInt(0),
                                              Dettaglio_Cod,
                                              CInt(enum_AnalisiParametri.AnalisiParametri_pH),
                                              CDbl(Ph),
                                              CDbl(0))

        XmlDoc3.LoadXml(str_Dettaglio)

        str_DatiDettagli = str_DatiDettagli + XmlDoc3.OuterXml

        Dim CalcTot As Decimal = Txt_CalcTot.Text
        str_Dettaglio = XML_Analisi_Dettaglio(Operazione,
                                              Session("ASG_SuperUser_CodFiscale"),
                                              CInt(0),
                                              Dettaglio_Cod,
                                              CInt(enum_AnalisiParametri.AnalisiParametri_CaCO3),
                                              CDbl(CalcTot),
                                              CDbl(0))

        XmlDoc3.LoadXml(str_Dettaglio)

        str_DatiDettagli = str_DatiDettagli + XmlDoc3.OuterXml

        Dim CalcAtt As Decimal = Txt_CalcAtt.Text
        str_Dettaglio = XML_Analisi_Dettaglio(Operazione,
                                              Session("ASG_SuperUser_CodFiscale"),
                                              CInt(0),
                                              Dettaglio_Cod,
                                              CInt(enum_AnalisiParametri.AnalisiParametri_CaCO3_Attivo),
                                              CDbl(CalcAtt),
                                              CDbl(0))

        XmlDoc3.LoadXml(str_Dettaglio)

        str_DatiDettagli = str_DatiDettagli + XmlDoc3.OuterXml

        Dim SO As Decimal = Txt_SO.Text
        str_Dettaglio = XML_Analisi_Dettaglio(Operazione,
                                                Session("ASG_SuperUser_CodFiscale"),
                                                CInt(0),
                                                Dettaglio_Cod,
                                                CInt(enum_AnalisiParametri.AnalisiParametri_SostanzaOrganica),
                                                CDbl(SO),
                                                CDbl(0),
                                                , , , )

        XmlDoc3.LoadXml(str_Dettaglio)

        str_DatiDettagli = str_DatiDettagli + XmlDoc3.OuterXml

        Dim N_az As Decimal = Txt_N.Text
        str_Dettaglio = XML_Analisi_Dettaglio(Operazione,
                                              Session("ASG_SuperUser_CodFiscale"),
                                              CInt(0),
                                              Dettaglio_Cod,
                                              CInt(enum_AnalisiParametri.AnalisiParametri_Ntot),
                                              CDbl(N_az),
                                              CDbl(0))

        XmlDoc3.LoadXml(str_Dettaglio)

        str_DatiDettagli = str_DatiDettagli + XmlDoc3.OuterXml


        Dim P2O5 As Decimal = 0
        Dim P As Decimal = 0
        Dim PC_Dettagli_Flag_P As Integer = 0

        Select Case DDL_P2O5.SelectedValue
            Case "0"
                PC_Dettagli_Flag_P = 0
                If IsNumeric(Txt_P.Text) Then
                    P2O5 = CDec(Txt_P.Text.Replace(".", ","))
                    P = CDec(Txt_P.Text.Replace(".", ",")) * Fattore_Conversione_P2O5_P
                End If
            Case Else
                PC_Dettagli_Flag_P = 1
                If IsNumeric(Txt_P.Text) Then
                    P2O5 = CDec(Txt_P.Text.Replace(".", ",")) * Fattore_Conversione_P_P2O5
                    P = CDec(Txt_P.Text.Replace(".", ","))
                End If
        End Select

        str_Dettaglio = XML_Analisi_Dettaglio(Operazione,
                                              Session("ASG_SuperUser_CodFiscale"),
                                              CInt(0),
                                              Dettaglio_Cod,
                                              CInt(enum_AnalisiParametri.AnalisiParametri_P2O5_assimilabile),
                                              CDbl(P2O5),
                                              CDbl(0))

        XmlDoc3.LoadXml(str_Dettaglio)

        str_DatiDettagli = str_DatiDettagli + XmlDoc3.OuterXml

        str_Dettaglio = XML_Analisi_Dettaglio(Operazione,
                                              Session("ASG_SuperUser_CodFiscale"),
                                              CInt(0),
                                              Dettaglio_Cod,
                                              CInt(enum_AnalisiParametri.AnalisiParametri_P_assimilabile),
                                              CDbl(P),
                                              CDbl(0))

        XmlDoc3.LoadXml(str_Dettaglio)

        str_DatiDettagli = str_DatiDettagli + XmlDoc3.OuterXml



        Dim K2O As Decimal = 0
        Dim K As Decimal = 0
        Dim PC_Dettagli_Flag_K As Integer = 0

        Select Case DDL_K2O.SelectedValue
            Case "0"
                PC_Dettagli_Flag_K = 0
                If IsNumeric(Txt_K.Text) Then
                    K2O = CDec(Txt_K.Text.Replace(".", ","))
                    K = CDec(Txt_K.Text.Replace(".", ",")) * Fattore_Conversione_K2O_K
                End If
            Case Else
                PC_Dettagli_Flag_K = 1
                If IsNumeric(Txt_K.Text) Then
                    K2O = CDec(Txt_K.Text.Replace(".", ",")) * Fattore_Conversione_K_K2O
                    K = CDec(Txt_K.Text.Replace(".", ","))
                End If
        End Select

        str_Dettaglio = XML_Analisi_Dettaglio(Operazione,
                                              Session("ASG_SuperUser_CodFiscale"),
                                              CInt(0),
                                              Dettaglio_Cod,
                                              CInt(enum_AnalisiParametri.AnalisiParametri_K2O_scambiabile),
                                              CDbl(K2O),
                                              CDbl(0))

        XmlDoc3.LoadXml(str_Dettaglio)

        str_DatiDettagli = str_DatiDettagli + XmlDoc3.OuterXml

        str_Dettaglio = XML_Analisi_Dettaglio(Operazione,
                                              Session("ASG_SuperUser_CodFiscale"),
                                              CInt(0),
                                              Dettaglio_Cod,
                                              CInt(enum_AnalisiParametri.AnalisiParametri_K_scambiabile),
                                              CDbl(K),
                                              CDbl(0))

        XmlDoc3.LoadXml(str_Dettaglio)

        str_DatiDettagli = str_DatiDettagli + XmlDoc3.OuterXml

        Dim CN As Decimal = Txt_CN.Text
        str_Dettaglio = XML_Analisi_Dettaglio(Operazione,
                                              Session("ASG_SuperUser_CodFiscale"),
                                              CInt(0),
                                              Dettaglio_Cod,
                                              CInt(enum_AnalisiParametri.AnalisiParametri_RapportoCN),
                                              CDbl(CN),
                                              CDbl(0))

        XmlDoc3.LoadXml(str_Dettaglio)

        str_DatiDettagli = str_DatiDettagli + XmlDoc3.OuterXml

        Dim Mg As Decimal = Txt_Mg.Text
        str_Dettaglio = XML_Analisi_Dettaglio(Operazione,
                                              Session("ASG_SuperUser_CodFiscale"),
                                              CInt(0),
                                              Dettaglio_Cod,
                                              CInt(enum_AnalisiParametri.AnalisiParametri_Mg_assimilabile),
                                              CDbl(Mg),
                                              CDbl(0))

        XmlDoc3.LoadXml(str_Dettaglio)

        str_DatiDettagli = str_DatiDettagli + XmlDoc3.OuterXml

        Dim CSC As Decimal = Txt_CSC.Text
        str_Dettaglio = XML_Analisi_Dettaglio(Operazione,
                                              Session("ASG_SuperUser_CodFiscale"),
                                              CInt(0),
                                              Dettaglio_Cod,
                                              CInt(enum_AnalisiParametri.AnalisiParametri_CSC),
                                              CDbl(CSC),
                                              CDbl(0))

        XmlDoc3.LoadXml(str_Dettaglio)

        str_DatiDettagli = str_DatiDettagli + XmlDoc3.OuterXml


        XML_DatiDettagli.InnerXml = str_DatiDettagli

        '----- Assemblo la struttura

        XmlDoc.AppendChild(XML_DatiTestate)

        '----- Restituisco il risultato

        Return XmlDoc.OuterXml

    End Function

    Private Function ChiamaScriviAnalisiTerrenoModello() As Integer
        Dim OUTPUT_Testata_Cod As Integer = 0
        Dim objAnalisiTerrenoWrite As New AgronicaCoreAnagrafeBIZ.Analisi_Modello_W
        Dim objAnalisiTerrenoUtil As New AgronicaCoreAnagrafeBIZ.Analisi_Modello_Utility

        Dim DataInizio As Date
        Dim DataFine As Date

        If Me.Txt_ValiditaInizio.Text = "" Then
            DataInizio = AGRODATAINIZIO
        Else
            DataInizio = CDate(Me.Txt_ValiditaInizio.Text)
        End If
        If Me.Txt_ValiditaFine.Text = "" Then
            DataFine = AGRODATAFINE
        Else
            DataFine = CDate(Me.Txt_ValiditaFine.Text)
        End If

        Dim P2O5 As Decimal = 0
        Dim P As Decimal = 0
        Select Case DDL_P2O5.SelectedValue
            Case "0"
                If IsNumeric(Txt_P.Text) Then
                    P2O5 = CDec(Txt_P.Text.Replace(".", ","))
                    P = CDec(Txt_P.Text.Replace(".", ",")) * Fattore_Conversione_P2O5_P
                End If
            Case Else
                If IsNumeric(Txt_P.Text) Then
                    P2O5 = CDec(Txt_P.Text.Replace(".", ",")) * Fattore_Conversione_P_P2O5
                    P = CDec(Txt_P.Text.Replace(".", ","))
                End If
        End Select

        Dim K2O As Decimal = 0
        Dim K As Decimal = 0
        Select Case DDL_K2O.SelectedValue
            Case "0"
                If IsNumeric(Txt_K.Text) Then
                    K2O = CDec(Txt_K.Text.Replace(".", ","))
                    K = CDec(Txt_K.Text.Replace(".", ",")) * Fattore_Conversione_K2O_K
                End If
            Case Else
                If IsNumeric(Txt_K.Text) Then
                    K2O = CDec(Txt_K.Text.Replace(".", ",")) * Fattore_Conversione_K_K2O
                    K = CDec(Txt_K.Text.Replace(".", ","))
                End If
        End Select

        Dim analisiTerreno = objAnalisiTerrenoUtil.GeneraAnalisiTerrenoPerPianoConcimazione(Qs_Piva,
                                                                                            CInt(Cmb_Centro.SelectedValue),
                                                                                            DataInizio,
                                                                                            DataFine,
                                                                                            Txt_AnalisiDes.Text,
                                                                                            CDec(Txt_Sabbia.Text),
                                                                                            CDec(Txt_Argilla.Text),
                                                                                            CDec(Txt_Limo.Text),
                                                                                            CDec(Txt_PH.Text),
                                                                                            CDec(Txt_CalcTot.Text),
                                                                                            CDec(Txt_CalcAtt.Text),
                                                                                            CDec(Txt_SO.Text),
                                                                                            CDec(Txt_N.Text),
                                                                                            P2O5,
                                                                                            P,
                                                                                            K2O,
                                                                                            K,
                                                                                            CDec(Txt_CN.Text),
                                                                                            CDec(Txt_Mg.Text),
                                                                                            CDec(Txt_CSC.Text))

        objAnalisiTerrenoWrite.Scrivi_AnalisiTerreno_Modello(Qs_Piva,
                                                             OUTPUT_Testata_Cod,
                                                             analisiTerreno,
                                                             objParametri_Super_Server,
                                                             objParametri_Server,
                                                             objParametri_Utenti,
                                                             NoteLog:="PCB_Inserimento.aspx (modello)")
        Return OUTPUT_Testata_Cod

    End Function

    Private Function SalvaAnalisi(ByRef OUTPUT_Testata_Cod As Integer) As Boolean

        Dim msg As String
        Dim intDummy As Boolean
        Dim StrDummy As String
        Dim DataInizio As Date
        Dim DataFine As Date

        Dim DtDettagli As New DataTable
        Dim DtDettagliShort As New DataTable
        Dim DtCampioni As New DataTable
        Dim DettagliTemp As New DataTable

        Dim StringaXml As String

        Dim P2O5_assimilabile As String = ""
        Dim P_assimilabile As String = ""
        Dim K2O_assimilabile As String = ""
        Dim K_scambiabile As String = ""
        Dim k, p As Integer
        p = -1
        k = -1

        '------------------------------------------------------------
        '----- Verifico la correttezza delle informazioni inserite
        '------------------------------------------------------------

        msg = ""

        If Me.Txt_AnalisiDes.Text = "" Then
            msg += Resources.PianoConcimazione_2017.InserireDescrizioneAnalisi & vbCrLf
        End If

        If Me.Cmb_Centro.SelectedIndex < 0 Then
            msg += Resources.PianoConcimazione_2017.InserireCentrAziendale & vbCrLf
        End If
        '------------------------------------------------------------
        If Me.Txt_ValiditaInizio.Text = "" Then
            DataInizio = CDate("01/01/1900")
        Else
            DataInizio = CDate(Me.Txt_ValiditaInizio.Text)
        End If
        '------------------------------------------------------------

        If Me.Txt_ValiditaFine.Text = "" Then
            DataFine = CDate("31/12/2100")
        Else
            DataFine = CDate(Me.Txt_ValiditaFine.Text)
        End If

        '------------------------------------------------------------

        If msg <> "" Then
            AgronicaCoreUtility.Messaggi.AgroMsgBox(msg, Page, , UpdatePanelPerScript)
            Exit Function
        End If

        Try
            If hd_usaAnalisiModelloNG.Value = "1" Then
                OUTPUT_Testata_Cod = ChiamaScriviAnalisiTerrenoModello()
            Else
                'Creo la stringa di inserimento
                StringaXml = XML_GeneraStringoneFinale()

                Dim objTestataWrite As New AgronicaCoreAnagrafeBIZ.Analisi_Testata_W

                intDummy = objTestataWrite.Analisi_Testata_Scrivi(CStr(StringaXml),
                                                                  OUTPUT_Testata_Cod,
                                                                  objParametri_Server,
                                                                  NoteLog:="PianoNutrizionale_IBF.aspx")

                objTestataWrite = Nothing
            End If
            Return True

        Catch ex As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------
            StrDummy = ex.Message.ToString()

            AgronicaCoreUtility.Messaggi.AgroMsgBox("Si è verificato un errore durante la fase di salvataggio: " & StrDummy, Page, , UpdatePanelPerScript)

            Return False

        End Try

    End Function
#End Region

#Region "METEO"
    Private Function Carica_Dati_Meteo(ByVal tipoSorgente As enum_Meteo_Tiposorgente, ByVal sorgente As Integer, ByVal DataDa As Date, ByVal DataA As Date) As Decimal

        Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT

        Dim objParams As New JObject
        objParams("TipoSorgente") = tipoSorgente
        objParams("Sorgente") = sorgente
        objParams("DataInizio") = DataDa
        objParams("DataFine") = DataA

        Dim ss As String = objMeteoNT.DatiMeteoElaboraPiogge(objParams, objParametri_Server)
        Dim obj = JsonConvert.DeserializeObject(ss)

        If Not obj("RispostaOK") Then
            Return 0
        End If

        Dim datiGG = JArray.Parse(obj("RispostaStringa"))

        Dim Prec As Decimal = 0

        For Each dgg In datiGG

            Prec += CDec(dgg("Prec"))
        Next

        Return Prec
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Piogge_WM(ByVal PIVA As String, ByVal SaCod As Integer, ByVal Anno As Integer, ByVal leggiDaAgenda As Integer, ByVal tipoSorgente As Integer, ByVal sorgente As Integer, ByVal regolamento As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Try
            Dim TotMMInv As Decimal = -1
            Dim TotMMFeb As Decimal = -1

            'piogge invernali
            Dim DataDAInv As Date = CDate("01/10/" & Anno - 1)
            Dim DataAInv As Date = CDate("31/01/" & Anno)

            'piogge febbraio
            Dim DataDAFeb As Date = "01/02/" & Anno
            Dim DataAFeb As Date = CDate("28/02/" & Anno)
            If DateTime.IsLeapYear(Anno) Then
                DataAFeb = CDate("29/02/" & Anno)
            End If

            If leggiDaAgenda = 1 Then

                TotMMInv = 0
                TotMMFeb = 0

                ' carico i dati delle piogge leggendo dall'agenda

                Dim objMovDettTecnico As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
                Dim DtPiogge As DataTable = objMovDettTecnico.LeggiPiogge(PIVA, SaCod, 0, DataDAInv, DataAInv, "", objParametri_Server)

                For Each row In DtPiogge.Rows
                    TotMMInv += row("Qta_ril")
                Next

                Select Case regolamento
                    Case Is >= enum_PUARegolamenti.PianoConcimazione_2016
                        DtPiogge = objMovDettTecnico.LeggiPiogge(PIVA, SaCod, 0, DataDAFeb, DataAFeb, "", objParametri_Server)

                        For Each row In DtPiogge.Rows
                            TotMMFeb += row("Qta_ril")
                        Next
                End Select

                objMovDettTecnico = Nothing
                DtPiogge.Dispose()
                DtPiogge = Nothing

            Else
                ' carico i dati delle piogge leggendo dal meteo

                TotMMInv = Carica_Dati_Meteo2(tipoSorgente, sorgente, DataDAInv, DataAInv, objParametri_Server)

                Select Case regolamento
                    Case Is >= enum_PUARegolamenti.PianoConcimazione_2016
                        TotMMFeb = Carica_Dati_Meteo2(tipoSorgente, sorgente, DataDAFeb, DataAFeb, objParametri_Server)
                End Select
            End If

            Dim result As New JObject
            result("Pioggia") = Format(TotMMInv, "0.00")
            result("Pioggia_Febbraio") = Format(TotMMFeb, "0.00")

            r.RispostaOK = True
            r.RispostaStringa = result.ToString()

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    Private Shared Function Carica_Dati_Meteo2(ByVal tipoSorgente As enum_Meteo_Tiposorgente,
                                               ByVal sorgente As Integer,
                                               ByVal DataDa As Date,
                                               ByVal DataA As Date,
                                               ByVal objParametri_Server As AgronicaCoreParametri) As Decimal

        Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT

        Dim objParams As New JObject
        objParams("TipoSorgente") = tipoSorgente
        objParams("Sorgente") = sorgente
        objParams("DataInizio") = DataDa
        objParams("DataFine") = DataA

        Dim ss As String = objMeteoNT.DatiMeteoElaboraPiogge(objParams, objParametri_Server)
        Dim obj = JsonConvert.DeserializeObject(ss)

        If Not obj("RispostaOK") Then
            Return 0
        End If

        Dim datiGG = JArray.Parse(obj("RispostaStringa"))

        Dim Prec As Decimal = 0

        For Each dgg In datiGG

            Prec += CDec(dgg("Prec"))
        Next

        Return Prec
    End Function

#End Region

#Region "EVENT HANDLER"
    Private Sub PCB_InserimentoMultiplo_Init(sender As Object, e As EventArgs) Handles Me.Init
        Master_Concimaz = CType(Page.Master, MasterConcimazione)
        Master_Concimaz.flag_MostraBtnIndietro = True
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        AddHandler Master.ImgBtnAnnullaTutto.Click, AddressOf Me.AnnullaTutto

        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("Messaggi/AccessoNegato.htm")
        End If

        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        'ScriptxLoading()


        '######################################################################################################################
        '######################################################################################################################

        Session.Timeout = 180

        '##############################################################
        '#####  Recupero le variabili                        ##########
        '##############################################################

        objConcimazione = New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
        objConcimazione.Leggi()

        'If Not IsNothing(Request.QueryString("r")) Then

        '    Qs_RegolamentoCod = Stringa_Decodifica(Request.QueryString("r").ToString, _
        '                                              AgroKey_EncoderDecoder, _
        '                                              Server)
        'Else
        '    Qs_RegolamentoCod = enum_PUARegolamenti.PianoConcimazione_2012
        'End If

        If Not IsNothing(Request.QueryString("tipo")) Then
            Qs_Tipo = Stringa_Decodifica(Request.QueryString("tipo").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Tipo = objConcimazione.Tipo_Concimazione
        End If
        hd_Tipo.Value = Qs_Tipo

        Qs_Operazione = Stringa_Decodifica(Request.QueryString("o").ToString,
                            AgroKey_EncoderDecoder,
                            Server)
        hd_Operazione.Value = Qs_Operazione

        objAnalisiModelloUtils = New AgronicaCoreAnagrafeBIZ.Analisi_Modello_Utility
        hd_usaAnalisiModelloNG.Value = objAnalisiModelloUtils.usaAnalisiTerrenoNG(objParametri_Utenti)

        'TEMPORANEO
        If Not IsNothing(Request.QueryString("p")) Then
            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)
        Else
            Qs_Piva = objConcimazione.Piva
        End If
        hd_Piva.Value = Qs_Piva

        If Not IsNothing(Request.QueryString("q")) Then
            Qs_PCTestataCod = Stringa_Decodifica(Request.QueryString("q").ToString,
                                            AgroKey_EncoderDecoder,
                                            Server)
        Else
            Qs_PCTestataCod = objConcimazione.PianoConcimazione_Testata_Cod
        End If

        If Not IsNothing(Request.QueryString("blocco_flag")) Then
            Qs_Blocco_Flag = Stringa_Decodifica(Request.QueryString("blocco_flag").ToString,
                                            AgroKey_EncoderDecoder,
                                            Server)
        Else
            Qs_Blocco_Flag = 0
        End If

        If Qs_Blocco_Flag > 0 Then
            Div_BTNSalva.Visible = False
            Div_BtnApplica.Visible = False
            Div_NPK_Calcolati.Visible = False
        End If

        Dim UtenteAbilitato As Boolean
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Select Case Qs_Operazione

            Case enum_TipoOperazioneDB.Lettura
                UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"),
                                    Session("ASG_IdServizio"),
                                    enum_Security_Attivita.Piano_Nutrizionale,
                                    enum_Security_Operazione.Lettura,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)

            Case Else
                UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"),
                                    Session("ASG_IdServizio"),
                                    enum_Security_Attivita.Piano_Nutrizionale,
                                    enum_Security_Operazione.Modifica,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)
        End Select

        If UtenteAbilitato = False Then
            Response.Redirect("Messaggi/AccessoNegato.htm")
        End If

        If Not Page.IsPostBack Then

            Select Case Qs_Tipo
                Case enum_PianoConcimazione_Tipo.Bilancio
                    Master.Lbl_Titolo.Text = Resources.PianoConcimazione_2017.PianoNutrizionaleBilancio
                Case Else
                    Master.Lbl_Titolo.Text = Resources.PianoConcimazione_2017.PianoNutrizionaleSchede
            End Select

            If Qs_Piva <> "" Then
                Dim objRS As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Master.LblRag_Soc.Text = objRS.RagSoc_from_Piva(Qs_Piva, objParametri_Server)
            End If

            Testata_Cod.Value = Qs_PCTestataCod

            If Qs_Operazione = enum_TipoOperazioneDB.Lettura Then
                'GABRIELE Me.Btn_Meteo.Visible = False
                Me.Btn_Applica.Visible = False
                Me.Btn_Bilancio.Visible = False
                Me.Btn_Salva.Visible = False
                Me.Div_BtnVisualizza.Visible = False
                Me.Div_BTNSalva.Visible = False
                Me.Div_BtnApplica.Visible = False
                Div_NPK_Calcolati.Visible = False
                'GABRIELE Me.Rbl_Meteo.Visible = False
                Me.BtnAnalisi.Visible = False
                Me.BtnVisualizza.Visible = False
                Me.BtnRicerca.Visible = False
                Div_SalvaAnalisi.Visible = False

                bloccaPerLettura()
            Else
                If hd_usaAnalisiModelloNG.Value Then
                    Me.BtnAnalisi.Visible = False
                    Me.BtnVisualizza.Visible = True
                    Me.BtnRicerca.Visible = True
                Else
                    Me.BtnAnalisi.Visible = True
                    Me.BtnVisualizza.Visible = False
                    Me.BtnRicerca.Visible = False
                End If
            End If


            Carica_Dati()


        Else
            Exit Sub
        End If

        If ddlAnalisi.SelectedValue.Trim <> "0" Then
            BtnVisualizza.Text = PianoConcimazione_2017.Resources.PianoConcimazione_2017.VisualizzaCompleta
        Else
            BtnVisualizza.Text = PianoConcimazione_2017.Resources.PianoConcimazione_2017.CreazioneCompleta
        End If

    End Sub

    Private Sub bloccaPerLettura()
        ddlRegolamento.Enabled = False
        Txt_Descrizione.Enabled = False
        Txt_ValiditaInizio.Enabled = False
        Txt_ValiditaFine.Enabled = False
        Txt_Anno.Enabled = False
        Txt_Note.Enabled = False
        Chk_NonUtilizzo_Fertilizzanti.Enabled = False

        Cmb_Centro.Enabled= False
        RBL_Specie.Enabled= False
        Cmb_Specie_ColturaPrincipale.Enabled= False
        ddlFinalitaRer_ColturaPrincipale.Enabled= False
        Txt_Resa.Enabled= False

        ddlAnalisi.Enabled= False
        Txt_Sabbia.Enabled= False
        Txt_Argilla.Enabled= False
        Txt_Limo.Enabled= False
        Txt_PH.Enabled= False
        Txt_CalcAtt.Enabled= False
        Txt_CalcTot.Enabled= False
        Txt_SO.Enabled= False
        Txt_CN.Enabled= False
        Txt_N.Enabled= False
        Txt_P.Enabled= False
        Txt_K.Enabled= False
        Txt_Mg.Enabled= False
        Txt_CSC.Enabled= False

        Cmb_Specie_Precessione.Enabled= False
        ddlFinalitaRer_Precessione.Enabled= False
        Chk_ResiduiPrecessione.Enabled= False
        Txt_ResaStoricaPrecessione.Enabled= False
        ddlConcimeOrganico.Enabled= False
        Txt_QtaN_KGHa.Enabled= False
        ddlEpocaModalitaDistribuzione.Enabled= False

        Txt_Pioggia.Enabled= False
        Txt_AvgTemperatura_ColturaInCampo.Enabled= False
        Txt_AvgTemperatura_MeseSemina_Febbraio.Enabled= False
        Txt_PercUmiditaColturaPrincipale.Enabled= False
        Txt_PercUmiditaRaccoltaPrecessione.Enabled= False
    End Sub

    Private Sub Btn_Bilancio_Click(sender As Object, e As EventArgs) Handles Btn_Bilancio.Click

        Select Case Qs_Tipo
            Case enum_PianoConcimazione_Tipo.Bilancio
                CalcolaBilancio()
                Div_Bilancio.Visible = True
            Case enum_PianoConcimazione_Tipo.Schede
                CalcolaSchede(Nothing)
                Div_Schede.Visible = True
        End Select

    End Sub

    Private Sub Btn_Applica_Click(sender As Object, e As EventArgs) Handles Btn_Applica.Click

        Dim QtaMaxN As Decimal = 0
        Dim QtaMaxP As Decimal = 0
        Dim QtaMaxk As Decimal = 0

        Dim ValiditaInizio As Date = AGRODATAINIZIO
        Dim ValiditaFine As Date = AGRODATAFINE

        If IsNumeric(Txt_N_Da_Applicare.Text) Then
            QtaMaxN = CDec(Txt_N_Da_Applicare.Text.Replace(".", ","))
        End If
        'If IsNumeric(Txt_P_Da_Applicare.Text) Then
        '    QtaMaxP = CDec(Txt_P_Da_Applicare.Text.Replace(".", ","))
        'End If
        'If IsNumeric(Txt_K_Da_Applicare.Text) Then
        '    QtaMaxk = CDec(Txt_K_Da_Applicare.Text.Replace(".", ","))
        'End If


        If Txt_ValiditaInizio.Text <> "" Then
            ValiditaInizio = CDate(Txt_ValiditaInizio.Text)
        End If

        If Txt_ValiditaFine.Text <> "" Then
            ValiditaFine = CDate(Txt_ValiditaFine.Text)
        End If

        Dim objPC_EntitaxTestata As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_EntitaxTestata_W
        Dim objApporti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W

        Dim PC_Testata_Cod As Integer = CInt(Testata_Cod.Value)

        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)


            If Qs_Operazione = enum_TipoOperazioneDB.Modifica Then
                objPC_EntitaxTestata.Cancella(PC_Testata_Cod, 0, "", 0, 0, 0, 0, 0, "", "", "", 0, 0, "", 0, "", "", objParametri_Server)
            End If

            For i = 0 To GridView_Impianti.Rows.Count - 1

                If CType(GridView_Impianti.Rows(i).FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True Then

                    'record legame piano/esercizio
                    objPC_EntitaxTestata.Scrivi(PC_Testata_Cod,
                                                enum_EntitaAlberoImprese.Distinta,
                                                GridView_Impianti.DataKeys(i).Item("piva"),
                                                GridView_Impianti.DataKeys(i).Item("sa_cod"),
                                                GridView_Impianti.DataKeys(i).Item("appezza"),
                                                GridView_Impianti.DataKeys(i).Item("id_reg"),
                                                0,
                                                GridView_Impianti.DataKeys(i).Item("campo_cod"),
                                                "", "", "", 0, 0, "",
                                                GridView_Impianti.DataKeys(i).Item("progetto_cod"),
                                                "",
                                                "",
                                                QtaMaxN,
                                                QtaMaxP,
                                                QtaMaxk,
                                                ValiditaInizio,
                                                ValiditaFine,
                                                objParametri_Server)

                    'record apporti esercizio

                    objApporti.ModificaxProgetto2(GridView_Impianti.DataKeys(i).Item("piva"),
                                                  GridView_Impianti.DataKeys(i).Item("sa_cod"),
                                                  GridView_Impianti.DataKeys(i).Item("appezza"),
                                                  GridView_Impianti.DataKeys(i).Item("id_reg"),
                                                  GridView_Impianti.DataKeys(i).Item("progetto_cod"),
                                                  enum_CodiciAnagrafe.Impianto_LimiteN,
                                                  QtaMaxN,
                                                  AGRODATAINIZIO, AGRODATAFINE,
                                                  "",
                                                  objParametri_Server)

                    'objApporti.ModificaxProgetto2(GridView_Impianti.DataKeys(i).Item("piva"),
                    '                              GridView_Impianti.DataKeys(i).Item("sa_cod"),
                    '                              GridView_Impianti.DataKeys(i).Item("appezza"),
                    '                              GridView_Impianti.DataKeys(i).Item("id_reg"),
                    '                              GridView_Impianti.DataKeys(i).Item("progetto_cod"),
                    '                              enum_CodiciAnagrafe.Impianto_LimiteP,
                    '                              QtaMaxP,
                    '                              AGRODATAINIZIO, AGRODATAFINE,
                    '                              "",
                    '                              objParametri_Server)

                    'objApporti.ModificaxProgetto2(GridView_Impianti.DataKeys(i).Item("piva"),
                    '                              GridView_Impianti.DataKeys(i).Item("sa_cod"),
                    '                              GridView_Impianti.DataKeys(i).Item("appezza"),
                    '                              GridView_Impianti.DataKeys(i).Item("id_reg"),
                    '                              GridView_Impianti.DataKeys(i).Item("progetto_cod"),
                    '                              enum_CodiciAnagrafe.Impianto_LimiteK,
                    '                              QtaMaxk,
                    '                              AGRODATAINIZIO, AGRODATAFINE,
                    '                              "",
                    '                              objParametri_Server)

                End If

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

        Carica_Impianti(Cmb_Specie_ColturaPrincipale.SelectedValue, Cmb_Centro.SelectedValue)

        'AnnullaTutto()
        Master.HiddenMessaggioOK = Resources.PianoConcimazione_2017.SalvataggioRiuscito

    End Sub

    Private Sub Btn_Salva_Click(sender As Object, e As EventArgs) Handles Btn_Salva.Click

        Salva()

    End Sub
    Private Sub ddlRegolamento_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRegolamento.SelectedIndexChanged

        Dim Regolamento_Cod As Integer = CInt(ddlRegolamento.SelectedValue)
        Carica_Dati_Regolamento(Regolamento_Cod)

    End Sub

    Private Sub ddlAnalisi_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlAnalisi.SelectedIndexChanged

        Dim Sabbia As Decimal = 0
        Dim Limo As Decimal = 0
        Dim Argilla As Decimal = 0
        Dim Ph As Decimal = 0
        Dim CalcTot As Decimal = 0
        Dim CalcAtt As Decimal = 0
        Dim N As Decimal = 0
        Dim SO As Decimal = 0
        Dim CN As Decimal = 0
        Dim P2O5 As Decimal = 0
        Dim K2O As Decimal = 0
        Dim P As Decimal = 0
        Dim K As Decimal = 0
        Dim Mg As Decimal = 0
        Dim CSC As Decimal = 0
        Dim Flag_P As Integer = 0
        Dim Flag_K As Integer = 0

        Dim Data_Inizio As Date = AGRODATAINIZIO
        Dim Data_Fine As Date = AGRODATAFINE

        If ddlAnalisi.SelectedValue <> "0" Then

            Dim ObjDatiAnalisi As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_R
            ObjDatiAnalisi.Carica_DatiAnalisi_xPianoConcimazione(CInt(ddlAnalisi.SelectedValue), Sabbia, Limo, Argilla, Ph, CalcTot, CalcAtt, SO, N, P2O5, K2O, CN, Mg, CSC, P, K, Data_Inizio, Data_Fine, objParametri_Server)
            If P2O5 = 0 And P <> 0 Then
                Flag_P = 1
            End If
            If K2O = 0 And K <> 0 Then
                Flag_K = 1
            End If
            Analisi_Valorizza(Sabbia, Limo, Argilla, Ph, CalcTot, CalcAtt, SO, N, P2O5, K2O, CN, Mg, CSC, P, K, Flag_P, Flag_K)
            SettaImpianti()
            Div_SalvaAnalisi.Visible = False

            If IsDate(Txt_ValiditaInizio.Text) AndAlso Data_Fine < CDate(Txt_ValiditaInizio.Text) Then
                AgronicaCoreUtility.Messaggi.AgroMsgBox(String.Format(Resources.PianoConcimazione_2017.AttenzioneAnalisiSelezionataScadutaIl, Data_Fine.ToShortDateString), Page, , UpdatePanelPerScript)
            End If

        Else
            Analisi_Valorizza(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
            'Btn_SalvaAnalisi.Visible = True
            Div_SalvaAnalisi.Visible = True
            Txt_AnalisiDes.Text = ""
        End If

        If ddlAnalisi.SelectedValue.Trim <> "0" Then
            BtnVisualizza.Text = PianoConcimazione_2017.Resources.PianoConcimazione_2017.VisualizzaCompleta
        Else
            BtnVisualizza.Text = PianoConcimazione_2017.Resources.PianoConcimazione_2017.CreazioneCompleta
        End If

    End Sub

    Private Sub Cmb_Specie_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Cmb_Specie_ColturaPrincipale.SelectedIndexChanged

        Carica_Dati_Specie(CInt(ddlRegolamento.SelectedValue), CInt(Cmb_Specie_ColturaPrincipale.SelectedValue), 0, 0, TipoColtura.Coltura_Principale, ddlFinalitaRer_ColturaPrincipale)

    End Sub

    Private Sub Cmb_Specie_Precessione_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Cmb_Specie_Precessione.SelectedIndexChanged

        Carica_Dati_Specie(CInt(ddlRegolamento.SelectedValue), CInt(Cmb_Specie_Precessione.SelectedValue), 0, 0, TipoColtura.Precessione, ddlFinalitaRer_Precessione)

    End Sub

    Private Sub ddlFinalitaRer_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlFinalitaRer_ColturaPrincipale.SelectedIndexChanged

        Dim Grfi_Cod As Integer = 0
        If IsNumeric(ddlFinalitaRer_ColturaPrincipale.SelectedValue) Then
            Grfi_Cod = CInt(ddlFinalitaRer_ColturaPrincipale.SelectedValue)
        End If

        Dim N_Fattore_Correttivo_Resa As Decimal = 0
        Dim Mas As Decimal = -1
        Dim Mas_Dir_Nitrati As Decimal = -1

        Txt_Resa.Text = LeggiResa(ddlRegolamento.SelectedValue, Cmb_Specie_ColturaPrincipale.SelectedValue, Grfi_Cod, enum_Stato_Impianto.Impianto_Produzione, Mas, N_Fattore_Correttivo_Resa, Mas_Dir_Nitrati)
        Lbl_ResaRiferimento.InnerText = Txt_Resa.Text
        Fattore_Correttivo_N_Resa.Value = N_Fattore_Correttivo_Resa

        ddlMasS.Items.Clear()
        ddlMasB.Items.Clear()

        If Qs_Tipo = enum_PianoConcimazione_Tipo.Schede Then
            If Mas >= 0 Then
                ddlMasS.Items.Add(New ListItem(Resources.PianoConcimazione_2017.PianoConcimazione & ": " & Mas.ToString, Mas))
            Else
                ddlMasS.Items.Add(New ListItem(Resources.PianoConcimazione_2017.PianoConcimazione & ": n.d.", "n.d."))
            End If

            If Mas_Dir_Nitrati >= 0 Then
                ddlMasS.Items.Add(New ListItem(Resources.PianoConcimazione_2017.DirettivaNitrati & ": " & Mas_Dir_Nitrati.ToString, Mas_Dir_Nitrati))
            Else
                ddlMasS.Items.Add(New ListItem(Resources.PianoConcimazione_2017.DirettivaNitrati & ": n.d.", "n.d."))
            End If
        Else
            If Mas >= 0 Then
                ddlMasB.Items.Add(New ListItem(Resources.PianoConcimazione_2017.LimiteMASPianoConcimazione & ": " & Mas.ToString & " kg/ha", Mas))
            Else
                ddlMasB.Items.Add(New ListItem(Resources.PianoConcimazione_2017.LimiteMASPianoConcimazione & ": n.d.", "n.d."))
            End If

            If Mas_Dir_Nitrati >= 0 Then
                ddlMasB.Items.Add(New ListItem(Resources.PianoConcimazione_2017.LimiteMASDirettivaNitrati & ": " & Mas_Dir_Nitrati.ToString & " kg/ha", Mas_Dir_Nitrati))
            Else
                ddlMasB.Items.Add(New ListItem(Resources.PianoConcimazione_2017.LimiteMASDirettivaNitrati & ": n.d.", "n.d."))
            End If
        End If
    End Sub

    Private Sub Cmb_Centro_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Cmb_Centro.SelectedIndexChanged

        'GABRIELE
        Dim sa_cod = CInt(Cmb_Centro.SelectedValue)
        Dim Regolamento_Cod As Integer = CInt(ddlRegolamento.SelectedValue)

        If RBL_Specie.SelectedValue = "1" Then
            Dim clc As New AgronicaCoreUtility.CaricaListControl
            clc.TutteSpecieColtivate_SenzaControlloData(Cmb_Specie_ColturaPrincipale, False, "", "", Qs_Piva, sa_cod, True, "", "", objParametri_Server, True)
            If IsNumeric(Cmb_Specie_ColturaPrincipale.SelectedValue) Then
                Carica_Dati_Specie(Regolamento_Cod, CInt(Cmb_Specie_ColturaPrincipale.SelectedValue), 0, 0, TipoColtura.Coltura_Principale, ddlFinalitaRer_ColturaPrincipale)
            End If
        Else
            If IsNumeric(Cmb_Specie_ColturaPrincipale.SelectedValue) Then
                Carica_Impianti(Cmb_Specie_ColturaPrincipale.SelectedValue, sa_cod)
            End If
        End If

        'leggo se esiste una stazione associata al centro
        Meteo_TipoSorgente.Value = "0"
        Meteo_Sorgente.Value = "0"

        If sa_cod > 0 Then
            Dim reader = New AgronicaCoreMeteoDAL.DSS_Centri_Aziendali_Agronica_Stazioni_Meteo_R
            Dim def_staz = reader.Leggi("Piva_SuperUser = '" & objParametri_Server.PivaSuperUser & "' AND piva = '" & Qs_Piva & "' AND sa_cod = " & sa_cod.ToString, "", objParametri_Server)
            If def_staz IsNot Nothing AndAlso def_staz.Rows.Count > 0 Then
                Meteo_TipoSorgente.Value = def_staz.Rows(0)("tipo_sorgente")
                Meteo_Sorgente.Value = def_staz.Rows(0)("Stazione_Cod")
            End If
        End If

        'ricarico le analisi in base al centro aziendale
        Dim anal_cod = CInt(ddlAnalisi.SelectedValue)
        AgronicaCoreUtility.CaricaListControl.Analisi_Terreno(ddlAnalisi, True, Resources.PianoConcimazione_2017.NessunaAnalisi, "0", Qs_Piva, "", "Analisi_Testata_Data_Inizio DESC", objParametri_Server, sa_cod)
        If ddlAnalisi.Items.Count > 1 Then
            ddlAnalisi.SelectedIndex = ddlAnalisi.Items.IndexOf(ddlAnalisi.Items.FindByValue(anal_cod))
        End If
        ddlAnalisi_SelectedIndexChanged(Me, Nothing)

    End Sub

    Protected Sub BtnAnalisi_Click(sender As Object, e As EventArgs) Handles BtnAnalisi.Click

        Dim Permessi As PermessiUtente = New PermessiUtente()
        Dim UtenteAbilitato_Modifica As Boolean = Permessi.getPermesso(enum_Security_Attivita.Gest_Analisi_AccessoMenu).Scrittura

        Dim StrWindowOpen As String

        If UtenteAbilitato_Modifica Then

            Dim objAnalisi As New AgronicaCoreGestioneRichieste.ParametriAnalisi_2010
            objAnalisi.Pagina_SitoOrigine = enum_PaginePianoConcimazione_2017.PCB_Inserimento
            objAnalisi.SitoOrigine = Enum_SiteRedirector.Sito_PianoConcimazione_2017
            objAnalisi.Piva = Qs_Piva
            objAnalisi.Sa_Cod = CInt(Cmb_Centro.SelectedValue)
            objAnalisi.Tipo_Analisi = enum_AnalisiTipo.Analisi_Terreno
            objAnalisi.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
            objAnalisi.Pagina_Richiesta = enum_PagineAnalisi_2010.Pagina_Analisi

            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAnalisi_2010_PassandoDirettamente_ParametriAnalisi_2010(
                                      Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                      objAnalisi)

            StrWindowOpen = UtilityProvider.JqueryModalDialogScript(
                 strJS, "", UpdatePanelPerScript.ClientID,
                550, 850, 0, 0,
                , , , , , , NomeForm:="aspnetForm")

            Master.lbl_DescrGeneric.Text = "Analisi"
        Else
            StrWindowOpen = "alert('" & Resources.PianoConcimazione_2017.NoPermessoGestioneAnalisi & ".');"
        End If

        Dim tags As Boolean = True
        ScriptManager.RegisterStartupScript(UpdatePanelPerScript, UpdatePanelPerScript.GetType(),
                                            String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, tags)

    End Sub

    Private Sub Btn_hidden_CaricaAnalisi_Click(sender As Object, e As System.EventArgs) Handles Btn_hidden_CaricaAnalisi.Click

        ddlAnalisi.Items.Clear()
        Dim sa_cod = CInt(Cmb_Centro.SelectedValue)
        AgronicaCoreUtility.CaricaListControl.Analisi_Terreno(ddlAnalisi, True, Resources.PianoConcimazione_2017.NessunaAnalisi, "0", Qs_Piva, "", "Analisi_Testata_Data_Inizio DESC", objParametri_Server, sa_cod)
        If ddlAnalisi.Items.Count > 1 Then
            ddlAnalisi.SelectedIndex = 1
        End If
        ddlAnalisi_SelectedIndexChanged(Me, Nothing)

    End Sub

    Private Sub Btn_SalvaAnalisi_Click(sender As Object, e As System.EventArgs) Handles Btn_SalvaAnalisi.Click
        If Qs_Operazione = enum_TipoOperazioneDB.Lettura Then
            Master.HiddenMessaggioErrore = ""
            Exit Sub
        End If

        Dim Permessi As PermessiUtente = New PermessiUtente()
        Dim UtenteAbilitato_Modifica As Boolean = Permessi.getPermesso(enum_Security_Attivita.Gest_Analisi_AccessoMenu).Scrittura

        If UtenteAbilitato_Modifica Then
            Dim testata_analisi_cod As Integer = 0
            If SalvaAnalisi(testata_analisi_cod) Then
                ddlAnalisi.SelectedIndex = ddlAnalisi.Items.IndexOf(ddlAnalisi.Items.FindByValue(testata_analisi_cod.ToString))
                Btn_hidden_CaricaAnalisi_Click(Me, Nothing)
            End If
        Else
            AgronicaCoreUtility.Messaggi.AgroMsgBox(Resources.PianoConcimazione_2017.NoPermessoSalvataggioAnalisi, Page, , UpdatePanelPerScript)
        End If

    End Sub

    Protected Sub BtnVisualizza_Click(sender As Object, e As EventArgs) Handles BtnVisualizza.Click

        Dim Permessi As PermessiUtente = New PermessiUtente()

        Dim analisi_testata_cod As Integer = ddlAnalisi.SelectedValue

        Dim UtenteAbilitato As Boolean
        Dim TipoOperazione As Integer

        Select Case analisi_testata_cod
            Case 0
                UtenteAbilitato = Permessi.getPermesso(enum_Security_Attivita.Gest_Analisi_AccessoMenu).Scrittura
                TipoOperazione = enum_TipoOperazioneDB.Scrittura
            Case Else
                UtenteAbilitato = Permessi.getPermesso(enum_Security_Attivita.Gest_Analisi_AccessoMenu).Lettura
                TipoOperazione = enum_TipoOperazioneDB.Lettura
        End Select

        Dim StrWindowOpen As String

        If UtenteAbilitato Then

            Dim objAnalisiNG As New AgronicaCoreGestioneRichieste.ParametriAnalisiTerrenoNG
            objAnalisiNG.Pagina_SitoOrigine = enum_PaginePianoConcimazione_2017.Piano_Nutrizionale_IBF
            objAnalisiNG.SitoOrigine = Enum_SiteRedirector.Sito_PianoConcimazione_2017
            objAnalisiNG.Pagina_Richiesta = enum_PagineGiasNG.Pagina_Analisi_Terreno_Edit

            objAnalisiNG.Piva = Qs_Piva
            objAnalisiNG.Sa_Cod = CInt(Cmb_Centro.SelectedValue)

            objAnalisiNG.Tipo_Analisi = enum_AnalisiTipo.Analisi_Terreno
            objAnalisiNG.Analisi_Testata_Cod = analisi_testata_cod
            objAnalisiNG.Tipo_Operazione = TipoOperazione

            Dim strJS As String = objAnalisiModelloUtils.Link_Pagina_AnalisiTerrenoNG(Qs_Piva, objAnalisiNG)

            StrWindowOpen = UtilityProvider.JqueryModalDialogScript(
                 strJS, "", UpdatePanelPerScript.ClientID,
                550, 850, 0, 0,
                , , , , , , NomeForm:="aspnetForm")

            Master.lbl_DescrGeneric.Text = "Analisi"
        Else
            StrWindowOpen = "alert('Non si dispone del permesso richiesto per visualizzare le analisi.');"
        End If

        Dim tags As Boolean = True
        ScriptManager.RegisterStartupScript(UpdatePanelPerScript, UpdatePanelPerScript.GetType(),
                                            String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, tags)

    End Sub

    Protected Sub btnRicerca_Click(sender As Object, e As EventArgs) Handles BtnRicerca.Click
        Dim Permessi As PermessiUtente = New PermessiUtente()
        Dim UtenteAbilitato_Modifica As Boolean = Permessi.getPermesso(enum_Security_Attivita.Gest_Analisi_AccessoMenu).Lettura

        Dim StrWindowOpen As String

        If UtenteAbilitato_Modifica Then

            Dim objAnalisiNG As New AgronicaCoreGestioneRichieste.ParametriAnalisiTerrenoNG
            objAnalisiNG.Pagina_SitoOrigine = enum_PaginePianoConcimazione_2017.PCB_Inserimento
            objAnalisiNG.SitoOrigine = Enum_SiteRedirector.Sito_PianoConcimazione_2017
            objAnalisiNG.Piva = Qs_Piva
            objAnalisiNG.Sa_Cod = CInt(Cmb_Centro.SelectedValue)
            objAnalisiNG.Tipo_Analisi = enum_AnalisiTipo.Analisi_Terreno
            objAnalisiNG.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
            objAnalisiNG.Pagina_Richiesta = enum_PagineGiasNG.Pagina_Analisi_Terreno

            Dim strJS As String = objAnalisiModelloUtils.Link_Pagina_AnalisiTerrenoNG(Qs_Piva, objAnalisiNG)

            StrWindowOpen = UtilityProvider.JqueryModalDialogScript(
                 strJS, "", UpdatePanelPerScript.ClientID,
                550, 850, 0, 0,
                , , , , , , NomeForm:="aspnetForm")

            Master.lbl_DescrGeneric.Text = "Analisi"
        Else
            StrWindowOpen = "alert('Non si dispone del permesso richiesto per gestire le analisi.');"
        End If

        Dim tags As Boolean = True
        ScriptManager.RegisterStartupScript(UpdatePanelPerScript, UpdatePanelPerScript.GetType(),
                                            String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, tags)
    End Sub

    Private Sub RBL_Specie_SelectedIndexChanged(sender As Object, e As EventArgs) Handles RBL_Specie.SelectedIndexChanged

        Dim Regolamento_Cod As Integer = CInt(ddlRegolamento.SelectedValue)
        Dim sa_cod = CInt(Cmb_Centro.SelectedValue)

        Select Case RBL_Specie.SelectedValue
            Case "1"
                Dim clc As New AgronicaCoreUtility.CaricaListControl
                clc.TutteSpecieColtivate_SenzaControlloData(Cmb_Specie_ColturaPrincipale, False, "", "", Qs_Piva, sa_cod, False, "", "", objParametri_Server, True)
            Case Else
                AgronicaControlli_2010.ListControl_PianoConcimazione_WS.PC_SpecieConcimazione_WS(Cmb_Specie_ColturaPrincipale, False, "", "", Regolamento_Cod, "", "")
        End Select

        If IsNumeric(Cmb_Specie_ColturaPrincipale.SelectedValue) Then
            Carica_Dati_Specie(Regolamento_Cod, CInt(Cmb_Specie_ColturaPrincipale.SelectedValue), 0, 0, TipoColtura.Coltura_Principale, ddlFinalitaRer_ColturaPrincipale)
        End If

    End Sub

#End Region
End Class