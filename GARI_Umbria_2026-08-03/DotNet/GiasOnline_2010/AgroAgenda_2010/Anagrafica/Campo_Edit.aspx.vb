Imports System.Web
Imports System.Web.Services

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreUtentiDAL
Imports System.Xml
Imports AgroAgenda_2010.Resources
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreUtility


Partial Class Campo_Edit
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda

    Public Operazione As Integer
    Dim xPiva As String
    Dim xSa_Cod As String
    Dim xCampo_Cod As String
    Dim Qs_Piva As String
    'Dim Qs_PivaPadre As String
    Dim Qs_PivaNuova As String
    Dim Qs_CampoTipo As String
    Dim Qs_Visibilita As Integer = 0

    Public jsRubrica As String
    Public jsGestCat As String
    Public jsParticelle As String
    Public jsCodici As String
    Public cmb_Codici As String

    Public permessi As PermessiUtente
    Public OperazioneSuRubrica As String
    Dim Errore As String
    Dim Errore2 As Boolean = False

    Public visible_Campo_Cod As Boolean = True

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    'CaricaListControl.GruppoVegetale(CType(Me.Cmb_GruppoVegetale, ListControl), _
    '                                 True, "", "", _
    '                                 0, True, viewstate("GruCod_daModificare"), _
    '                                 "", "", objParametri_Server, objParametri_Utenti)

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_SpecieVegetali_In_Session(ByVal specie As String) As String
        HttpContext.Current.Session("Specie_Vegetale") = specie
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Specie_Vegetali() As String

        Dim cmb As New DropDownList

        'CaricaListControl.ImpreseConFiltroUtente(cmb_cultivar, True, "SELEZIONA", "", _
        '                                                                 " Imprese.rag_soc LIKE '%" & parametro & "%'", " ORDER BY Rag_Soc asc", _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Server"), _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Utenti"))
        CaricaListControl.SpecieVegetale_Optimize_x_PDC_Modificata2(cmb,
                                                                    True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim rval As String = ""
        For Each itm As ListItem In cmb.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        'Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        'HttpContext.Current.Session("prova") = "caio"

        Return rval

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiungi_Particella_WS(ByVal dati As String, ByVal tipo As Integer) As String

        Dim Dt_Particelle As DataTable
        Dim risp As String
        Dim app_chk As String() = dati.Split(New Char() {"|"c})

        Dt_Particelle = HttpContext.Current.Session("Dt_Particelle")

        For i = 0 To Dt_Particelle.Rows.Count - 1
            ' Controllo se le chiavi coincidono
            'For j = 0 To app_chk.Count - 1

            If Dt_Particelle.Rows(i).Item("chiave") = app_chk(0) Then

                Dt_Particelle.Rows(i).Item("checked") = 1
                Dt_Particelle.Rows(i).Item("SuperficieDisponibile") = app_chk(1)
                Dt_Particelle.Rows(i).Item("SuperficieIntersezione") = app_chk(2)

            End If
            'Next

        Next

        'HttpContext.Current.Session("Dt_Particelle") = Dt_Particelle
        If tipo = 1 Then
            risp = DT_to_Json_Particelle(Dt_Particelle)
        End If

        Return risp

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Elimina_Particella_WS(ByVal dati As String, ByVal tipo As Integer) As String

        Dim Dt_Particelle As DataTable
        Dim risp As String
        Dim app_chk As String() = dati.Split(New Char() {"|"c})

        Dt_Particelle = HttpContext.Current.Session("Dt_Particelle")

        For i = 0 To Dt_Particelle.Rows.Count - 1
            ' Controllo se le chiavi coincidono
            'For j = 0 To app_chk.Count - 1

            If Dt_Particelle.Rows(i).Item("chiave") = app_chk(0) Then

                Dt_Particelle.Rows(i).Item("checked") = 0
                Dt_Particelle.Rows(i).Item("SuperficieIntersezione") = 0
                Dt_Particelle.Rows(i).Item("SuperficieDisponibile") = app_chk(1)

            End If
            'Next

        Next

        'HttpContext.Current.Session("Dt_Particelle") = Dt_Particelle
        If tipo = 1 Then
            risp = DT_to_Json_Particelle(Dt_Particelle)
        End If

        Return risp

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Appezzamenti_X_Salva_Tutto(ByVal dati As String) As String

        Dim DtAppezzamenti As DataTable
        Dim Dt_checked As DataTable

        DtAppezzamenti = HttpContext.Current.Session("Dt_Appezzamenti")

        Dt_checked = DtAppezzamenti.Clone

        Dim app_chk As String() = dati.Split(New Char() {"|"c})

        ' Per ogni chiave di riga di Appezzamento checked
        For Each chiave As String In app_chk

            ' Ciclo su DT di tutti gli Appezzamenti disponibili per ritrovare la riga
            For i = 0 To DtAppezzamenti.Rows.Count - 1

                ' Controllo se le chiavi coincidono
                If chiave = DtAppezzamenti.Rows(i).Item("chiave") Then

                    ' copio la riga intera
                    Dt_checked.ImportRow(DtAppezzamenti.Rows(i))
                End If
            Next

        Next


        HttpContext.Current.Session("Appezzamenti_checked") = Dt_checked

        Return "ok"

    End Function



    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiorna_Appezzamenti(ByVal tipo As String, ByVal data_inizio As String, ByVal data_fine As String) As String

        Dim objParametriAgenda As New ParametriAgenda
        Dim DTRs As DataTable
        Dim Dt As New DataTable
        Dim Dt_checked As New DataTable
        Dim Dr As DataRow

        Dim DataInizio As Date
        Dim DataFine As Date
        Dim strDataInizio As String
        Dim strDataFine As String
        Dim SuperficieTotale As Double
        Dim AppezzaPresente As Boolean
        Dim ArrayAppezza(2, 0) As String
        Dim N_Appezza As Integer = 0


        Dim risp As String = ""
        Dim objAppezzamentoR As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        If data_inizio = "" Then
            data_inizio = "01/01/1900"
        End If

        If data_fine = "" Then
            data_fine = "31/12/2100"
        End If




        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("chiave", GetType(String)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("ID_Imp", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("App_Nome", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_App", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Appezzamento", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio_Appezzamento", GetType(Date)))
        Dt.Columns.Add(New DataColumn("Validita_Fine_Appezzamento", GetType(Date)))
        Dt.Columns.Add(New DataColumn("ColturaCorrente", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Coltura", GetType(String)))
        Dt.Columns.Add(New DataColumn("checked", GetType(Integer)))

        '----- Definisco l'insieme di colonne che costituiscono la chiave della tabella

        'Vettore di DataColumn
        Dim DtKeys(2) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = Dt.Columns("Piva")
        DtKeys(1) = Dt.Columns("Sa_Cod")
        DtKeys(2) = Dt.Columns("Appezza")

        'Assegno il vettore delle chiavi al DataTable
        Dt.PrimaryKey = DtKeys


        If tipo = "senza_catasto" Then


            DTRs = objAppezzamentoR.Recupera_Appezzamenti_Colture_del_Campo(
                                                     CStr(objParametriAgenda.Piva),
                                                     CInt(objParametriAgenda.Sa_Cod),
                                                     CInt(objParametriAgenda.Campo_Cod),
                                                     CDate(data_inizio),
                                                     CDate(data_fine),
                                                     True,
                                                      AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                     "",
                                                     "",
                                                     HttpContext.Current.Session("ASG_objParametri_Server"))

            Dt_checked.Columns.Add(New DataColumn("chiave", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Piva", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
            Dt_checked.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
            Dt_checked.Columns.Add(New DataColumn("ID_Imp", GetType(Integer)))
            Dt_checked.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
            Dt_checked.Columns.Add(New DataColumn("App_Nome", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Sup_App", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Validita_Appezzamento", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Validita_Inizio_Appezzamento", GetType(Date)))
            Dt_checked.Columns.Add(New DataColumn("Validita_Fine_Appezzamento", GetType(Date)))
            Dt_checked.Columns.Add(New DataColumn("ColturaCorrente", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Validita_Coltura", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("checked", GetType(Integer)))




        Else


            'se non sono in creazione...
            If CInt(objParametriAgenda.Campo_Cod) <> 0 Then

                DTRs = objAppezzamentoR.Recupera_Appezzamenti_Colture_del_Campo(
                                                         CStr(objParametriAgenda.Piva),
                                                     CInt(objParametriAgenda.Sa_Cod),
                                                     CInt(objParametriAgenda.Campo_Cod),
                                                     CDate(data_inizio),
                                                     CDate(data_fine),
                                                        False,
                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                        "",
                                                        "",
                                                        HttpContext.Current.Session("ASG_objParametri_Server"))
            End If

        End If


        '----- Carico la griglia

        Dim j As Integer
        If (Not IsNothing(DTRs)) AndAlso
              (DTRs.Rows.Count > 0) Then

            'Rs.Filter = "ID_REG = 0 OR (Impianto_Validita_Inizio<=" & DataValiditaFine & " AND Impianto_Validita_Fine>=" & DataValiditaInizio & ")"

            SuperficieTotale = 0
            For i = 0 To DTRs.Rows.Count - 1

                AppezzaPresente = False

                For j = 0 To UBound(ArrayAppezza, 2)

                    If ArrayAppezza(0, j) = DTRs.Rows(i).Item("Piva") And
                    ArrayAppezza(1, j) = DTRs.Rows(i).Item("Sa_Cod") And
                    ArrayAppezza(2, j) = DTRs.Rows(i).Item("Appezza") Then
                        AppezzaPresente = True
                        Exit For
                    End If

                Next

                If Not AppezzaPresente Then

                    ReDim Preserve ArrayAppezza(2, N_Appezza)
                    ArrayAppezza(0, N_Appezza) = DTRs.Rows(i).Item("Piva")
                    ArrayAppezza(1, N_Appezza) = DTRs.Rows(i).Item("Sa_Cod")
                    ArrayAppezza(2, N_Appezza) = DTRs.Rows(i).Item("Appezza")
                    N_Appezza += 1

                    'Creo una nuova riga
                    Dr = Dt.NewRow

                    '---

                    'Definisco i valori

                    Dr.Item("Piva") = DTRs.Rows(i).Item("Piva")
                    Dr.Item("Sa_Cod") = DTRs.Rows(i).Item("Sa_Cod")
                    Dr.Item("Appezza") = DTRs.Rows(i).Item("Appezza")
                    Dr.Item("ID_Imp") = DTRs.Rows(i).Item("ID_Reg")
                    Dr.Item("Campo_Cod") = DTRs.Rows(i).Item("Campo_Cod")

                    Dr.Item("App_Nome") = DTRs.Rows(i).Item("App_Nome")
                    Dr.Item("Sup_App") = Format(CDbl(DTRs.Rows(i).Item("Sup_App")), "0.0000")

                    '---

                    If DTRs.Rows(i).Item("Campo_Cod") <> 0 Then
                        Dr.Item("checked") = 1
                    Else
                        Dr.Item("checked") = 0
                    End If

                    DataInizio = CDate(DTRs.Rows(i).Item("Validita_Inizio"))
                    DataFine = CDate(DTRs.Rows(i).Item("Validita_Fine"))

                    Dr.Item("Validita_Inizio_Appezzamento") = DataInizio.ToShortDateString
                    Dr.Item("Validita_Fine_Appezzamento") = DataFine.ToShortDateString

                    If DataInizio = #1/1/1900# Then
                        strDataInizio = "...................."
                    Else
                        strDataInizio = DataInizio.ToShortDateString
                    End If

                    If DataFine = #12/31/2100# Then
                        strDataFine = "...................."
                    Else
                        strDataFine = DataFine.ToShortDateString
                    End If

                    Dr.Item("Validita_Appezzamento") = strDataInizio & " - " & strDataFine

                    '---

                    If Not IsDBNull(DTRs.Rows(i).Item("Cul_Cod")) Then

                        If DTRs.Rows(i).Item("Cul_Cod") <> 0 Then

                            If Not IsDBNull(DTRs.Rows(i).Item("Cul_des")) Then
                                Dr.Item("ColturaCorrente") = DTRs.Rows(i).Item("Veg_Des") & " - " & DTRs.Rows(i).Item("Cul_Des")
                            Else
                                Dr.Item("ColturaCorrente") = " ... "
                            End If

                        Else
                            Dr.Item("ColturaCorrente") = AgronicaAgenda_2010.TerrenoNudo
                        End If

                    Else
                        Dr.Item("ColturaCorrente") = " ... "
                    End If

                    '---

                    If Not IsDBNull(DTRs.Rows(i).Item("Impianto_Validita_Inizio")) Then
                        DataInizio = CDate(DTRs.Rows(i).Item("Impianto_Validita_Inizio"))
                    Else
                        DataInizio = #1/1/1900#
                    End If


                    If Not IsDBNull(DTRs.Rows(i).Item("Impianto_Validita_Fine")) Then
                        DataFine = CDate(DTRs.Rows(i).Item("Impianto_Validita_Fine"))
                    Else
                        DataFine = #12/31/2100#
                    End If

                    If DataInizio = #1/1/1900# Then
                        strDataInizio = "...................."
                    Else
                        strDataInizio = DataInizio.ToShortDateString
                    End If

                    If DataFine = #12/31/2100# Then
                        strDataFine = "...................."
                    Else
                        strDataFine = DataFine.ToShortDateString
                    End If

                    Dr.Item("Validita_Coltura") = strDataInizio & " - " & strDataFine

                    '---


                    'Associo alla tabella la nuova riga creata
                    Dt.Rows.Add(Dr)
                    'Dt.ImportRow(Dr)



                    'Aggiorno il totale della superficie
                    SuperficieTotale += CDbl(DTRs.Rows(i).Item("Sup_App"))

                    Dr.Item("Piva") = DTRs.Rows(i).Item("Piva")
                    Dr.Item("Sa_Cod") = DTRs.Rows(i).Item("Sa_Cod")
                    Dr.Item("Appezza") = DTRs.Rows(i).Item("Appezza")
                    Dr.Item("ID_Imp") = DTRs.Rows(i).Item("ID_Reg")
                    Dr.Item("Campo_Cod") = DTRs.Rows(i).Item("Campo_Cod")

                    Dr.Item("chiave") = DTRs.Rows(i).Item("Piva") & "_" & DTRs.Rows(i).Item("Sa_Cod") & "_" & DTRs.Rows(i).Item("Appezza") & "_" & DTRs.Rows(i).Item("ID_Reg") & "_" & DTRs.Rows(i).Item("Campo_Cod")


                    ' Aggiungo la stessa riga se è checked
                    If Dr.Item("checked") = 1 Then
                        'Dt_checked.Rows.Add(Dr)
                        Dt_checked.ImportRow(Dr)
                    End If

                End If

                'Prossimo record
            Next

        End If




        risp = DT_to_Json_GestCat(Dt)

        Return risp

    End Function



    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiorna_Appezzamenti_DaDateWS(ByVal data_inizio As String, ByVal data_fine As String, ByVal piva As String, ByVal sa_cod As Integer, ByVal campo_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        'Dim DtAppezzamenti As DataTable
        'Dim DtRapportiFiltrati As DataTable
        'Dim Dr() As DataRow
        'Dim strFiltro As String = ""
        'Dim i As Integer

        'Dim risp As String = ""
        Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Try

            If data_inizio = "" Then
                data_inizio = "01/01/1900"
            End If

            If data_fine = "" Then
                data_fine = "31/12/2100"
            End If

            Dim DTRs As DataTable
            Dim Dt As New DataTable
            Dim Dt_checked As New DataTable

            Dim objAppezzamentoR As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

            DTRs = objAppezzamentoR.Recupera_Appezzamenti_Colture_del_Campo(
                                                                 piva,
                                                                 sa_cod,
                                                                 campo_cod,
                                                                 data_inizio,
                                                                 data_fine,
                                                                 True,
                                                                  enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                 "",
                                                                 "",
                                                                 objParametriServer)


            Dim Dr As DataRow

            Dim DataInizio As Date
            Dim DataFine As Date
            Dim strDataInizio As String
            Dim strDataFine As String

            Dim ArrayAppezza(2, 0) As String
            Dim AppezzaPresente As Boolean = False
            Dim N_Appezza As Integer = 0
            Dim i As Integer

            Dim SuperficieTotale As Double

            Dt.Columns.Add(New DataColumn("chiave", GetType(String)))
            Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
            Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("ID_Imp", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("App_Nome", GetType(String)))
            Dt.Columns.Add(New DataColumn("Sup_App", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Appezzamento", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Inizio_Appezzamento", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Validita_Fine_Appezzamento", GetType(Date)))
            Dt.Columns.Add(New DataColumn("ColturaCorrente", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Coltura", GetType(String)))
            Dt.Columns.Add(New DataColumn("checked", GetType(Integer)))

            Dt_checked.Columns.Add(New DataColumn("chiave", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Piva", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
            Dt_checked.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
            Dt_checked.Columns.Add(New DataColumn("ID_Imp", GetType(Integer)))
            Dt_checked.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
            Dt_checked.Columns.Add(New DataColumn("App_Nome", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Sup_App", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Validita_Appezzamento", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Validita_Inizio_Appezzamento", GetType(Date)))
            Dt_checked.Columns.Add(New DataColumn("Validita_Fine_Appezzamento", GetType(Date)))
            Dt_checked.Columns.Add(New DataColumn("ColturaCorrente", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Validita_Coltura", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("checked", GetType(Integer)))



            '----- Carico la griglia

            Dim j As Integer
            If (Not IsNothing(DTRs)) AndAlso
               (DTRs.Rows.Count > 0) Then

                'Rs.Filter = "ID_REG = 0 OR (Impianto_Validita_Inizio<=" & DataValiditaFine & " AND Impianto_Validita_Fine>=" & DataValiditaInizio & ")"

                SuperficieTotale = 0
                For i = 0 To DTRs.Rows.Count - 1

                    AppezzaPresente = False

                    For j = 0 To UBound(ArrayAppezza, 2)

                        If ArrayAppezza(0, j) = DTRs.Rows(i).Item("Piva") And
                        ArrayAppezza(1, j) = DTRs.Rows(i).Item("Sa_Cod") And
                        ArrayAppezza(2, j) = DTRs.Rows(i).Item("Appezza") Then
                            AppezzaPresente = True
                            Exit For
                        End If

                    Next

                    If Not AppezzaPresente Then

                        ReDim Preserve ArrayAppezza(2, N_Appezza)
                        ArrayAppezza(0, N_Appezza) = DTRs.Rows(i).Item("Piva")
                        ArrayAppezza(1, N_Appezza) = DTRs.Rows(i).Item("Sa_Cod")
                        ArrayAppezza(2, N_Appezza) = DTRs.Rows(i).Item("Appezza")
                        N_Appezza += 1

                        'Creo una nuova riga
                        Dr = Dt.NewRow

                        '---

                        'Definisco i valori

                        Dr.Item("Piva") = DTRs.Rows(i).Item("Piva")
                        Dr.Item("Sa_Cod") = DTRs.Rows(i).Item("Sa_Cod")
                        Dr.Item("Appezza") = DTRs.Rows(i).Item("Appezza")
                        Dr.Item("ID_Imp") = DTRs.Rows(i).Item("ID_Reg")
                        Dr.Item("Campo_Cod") = DTRs.Rows(i).Item("Campo_Cod")

                        Dr.Item("App_Nome") = DTRs.Rows(i).Item("App_Nome")
                        Dr.Item("Sup_App") = Format(CDbl(DTRs.Rows(i).Item("Sup_App")), "0.0000")

                        '---

                        If DTRs.Rows(i).Item("Campo_Cod") <> 0 Then
                            Dr.Item("checked") = 1
                        Else
                            Dr.Item("checked") = 0
                        End If

                        DataInizio = CDate(DTRs.Rows(i).Item("Validita_Inizio"))
                        DataFine = CDate(DTRs.Rows(i).Item("Validita_Fine"))

                        Dr.Item("Validita_Inizio_Appezzamento") = DataInizio.ToShortDateString
                        Dr.Item("Validita_Fine_Appezzamento") = DataFine.ToShortDateString

                        If DataInizio = #1/1/1900# Then
                            strDataInizio = "...................."
                        Else
                            strDataInizio = DataInizio.ToShortDateString
                        End If

                        If DataFine = #12/31/2100# Then
                            strDataFine = "...................."
                        Else
                            strDataFine = DataFine.ToShortDateString
                        End If

                        Dr.Item("Validita_Appezzamento") = strDataInizio & " - " & strDataFine

                        '---

                        If Not IsDBNull(DTRs.Rows(i).Item("Cul_Cod")) Then

                            If DTRs.Rows(i).Item("Cul_Cod") <> 0 Then

                                If Not IsDBNull(DTRs.Rows(i).Item("Cul_des")) Then
                                    Dr.Item("ColturaCorrente") = DTRs.Rows(i).Item("Veg_Des") & " - " & DTRs.Rows(i).Item("Cul_Des")
                                Else
                                    Dr.Item("ColturaCorrente") = " ... "
                                End If

                            Else
                                Dr.Item("ColturaCorrente") = AgronicaAgenda_2010.TerrenoNudo
                            End If

                        Else
                            Dr.Item("ColturaCorrente") = " ... "
                        End If

                        '---

                        If Not IsDBNull(DTRs.Rows(i).Item("Impianto_Validita_Inizio")) Then
                            DataInizio = CDate(DTRs.Rows(i).Item("Impianto_Validita_Inizio"))
                        Else
                            DataInizio = #1/1/1900#
                        End If


                        If Not IsDBNull(DTRs.Rows(i).Item("Impianto_Validita_Fine")) Then
                            DataFine = CDate(DTRs.Rows(i).Item("Impianto_Validita_Fine"))
                        Else
                            DataFine = #12/31/2100#
                        End If

                        If DataInizio = #1/1/1900# Then
                            strDataInizio = "...................."
                        Else
                            strDataInizio = DataInizio.ToShortDateString
                        End If

                        If DataFine = #12/31/2100# Then
                            strDataFine = "...................."
                        Else
                            strDataFine = DataFine.ToShortDateString
                        End If

                        Dr.Item("Validita_Coltura") = strDataInizio & " - " & strDataFine

                        '---


                        'Associo alla tabella la nuova riga creata
                        Dt.Rows.Add(Dr)
                        'Dt.ImportRow(Dr)



                        'Aggiorno il totale della superficie
                        SuperficieTotale += CDbl(DTRs.Rows(i).Item("Sup_App"))

                        Dr.Item("Piva") = DTRs.Rows(i).Item("Piva")
                        Dr.Item("Sa_Cod") = DTRs.Rows(i).Item("Sa_Cod")
                        Dr.Item("Appezza") = DTRs.Rows(i).Item("Appezza")
                        Dr.Item("ID_Imp") = DTRs.Rows(i).Item("ID_Reg")
                        Dr.Item("Campo_Cod") = DTRs.Rows(i).Item("Campo_Cod")

                        Dr.Item("chiave") = DTRs.Rows(i).Item("Piva") & "_" & DTRs.Rows(i).Item("Sa_Cod") & "_" & DTRs.Rows(i).Item("Appezza") & "_" & DTRs.Rows(i).Item("ID_Reg") & "_" & DTRs.Rows(i).Item("Campo_Cod")


                        ' Aggiungo la stessa riga se è checked
                        If Dr.Item("checked") = 1 Then
                            'Dt_checked.Rows.Add(Dr)
                            Dt_checked.ImportRow(Dr)
                        End If

                    End If

                    'Prossimo record
                Next

                'lblSupUtil.Text = Format(SuperficieTotale, "0.0000")

                'If Opt_Con_Catasto.Checked = True Then
                '    lblRapporto.Text = Math.Round(CDbl(lblSupUtil.Text) / CDbl(lblSupCatasto.Text) * 100, 1)
                'End If

                'If Opt_Senza_Catasto.Checked = True Then
                '    lblRapporto.Text = 100
                'End If


            End If

            HttpContext.Current.Session("Dt_Appezzamenti") = Dt
            HttpContext.Current.Session("Appezzamenti_checked") = Dt_checked

            Dim jsGestCat = DT_to_Json_GestCat(Dt)

            'If Not HttpContext.Current.Session("Dt_Appezzamenti") Is Nothing Then

            '    DtAppezzamenti = HttpContext.Current.Session("Dt_Appezzamenti")
            '    DtRapportiFiltrati = DtAppezzamenti.Clone

            '    strFiltro = " Validita_Inizio_Appezzamento >= '" & CDate(data_inizio).ToShortDateString & "' AND Validita_Fine_Appezzamento <= '" & CDate(data_fine).ToShortDateString & "' "

            '    Dr = DtAppezzamenti.Select(strFiltro)

            '    For i = 0 To Dr.Length - 1
            '        DtRapportiFiltrati.ImportRow(Dr(i))
            '    Next

            '    Dim DtKey(1) As String
            '    DtKey(0) = "Cod_Rapporto"
            '    DtKey(1) = "Cod_RisUm"


            '    risp = DT_to_Json_GestCat(DtRapportiFiltrati)

            'End If

            r.RispostaOK = True
            r.RispostaStringa = jsGestCat

        Catch ex As Exception

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False

        End Try

        Return r

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_GestCat(ByVal dt As DataTable) As String
        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)


        'Dim cn As New ColonneNome("Campo_Cod", "", "string")
        'cn._FormatoParticolare = "<span></span>"
        'cn._Filtrabile = False
        ''cn._css = "prova"
        'l.Add(cn)

        ''aggiungo i pulsanti per modifica ed eliminazione
        'Dim c = New ColonneNome("Campo_Cod", "Tool", "string")

        'Dim listaBtn = New List(Of btnAzioni)
        'listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaCodice(this);"))
        'listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaCodice(this);"))

        'Dim tool As New ToolStandard(listaBtn)
        'tool.cssColonna = "colmodmovimenti"
        'c._FormatoParticolare = tool.toString()
        'c._Filtrabile = False
        'c._ColonnaDiSelezione = True
        'l.Add(c)


        Dim cn As New ColonneNome("chiave", "chiave", "string")
        cn._Filtrabile = False
        cn._ColonnaDiSelezione = True
        cn._hidden = True
        'cn._css = "prova"
        l.Add(cn)

        cn = New ColonneNome("Appezza", "Appezza", "string")
        cn._Filtrabile = True
        cn._hidden = True
        l.Add(cn)


        cn = New ColonneNome("App_Nome", AgronicaAgenda_2010.Appezzamento, "string")
        cn._Filtrabile = True
        l.Add(cn)

        cn = New ColonneNome("Sup_App", AgronicaAgenda_2010.SuperficieAbbr & " [ha]", "number")
        cn._Filtrabile = True
        l.Add(cn)

        cn = New ColonneNome("Validita_Appezzamento", AgronicaAgenda_2010.ValiditàAppezzamento, "string")
        cn._Filtrabile = True
        l.Add(cn)

        cn = New ColonneNome("Validita_Inizio_Appezzamento", "Validita_Inizio_Appezzamento", "string")
        cn._Filtrabile = True
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("Validita_Fine_Appezzamento", "Validita_Fine_Appezzamento", "string")
        cn._Filtrabile = True
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("ColturaCorrente", AgronicaAgenda_2010.Coltura, "string")
        cn._Filtrabile = True
        l.Add(cn)

        cn = New ColonneNome("Validita_Coltura", AgronicaAgenda_2010.ValiditàColtura, "string")
        cn._Filtrabile = True
        l.Add(cn)

        cn = New ColonneNome("checked", "checked", "string")
        cn._css = "wacol_checked"
        cn._hidden = True
        l.Add(cn)


        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Codici(ByVal dt As DataTable) As String
        Dim arr As New JArray
        For Each rowCodice In dt.Rows
            Dim obj As New JObject
            obj("Id_Cod") = CStr(rowCodice("Id_Cod"))
            obj("Descrizione") = CStr(rowCodice("Descrizione"))
            obj("Val_Cod") = CStr(rowCodice("Val_Cod"))
            arr.Add(obj)
        Next
        Return arr.ToString
    End Function

    'HttpContext.Current.Session("Dt_Codici")
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornajsCodici(strJsCodici As String) As RispostaStandard
        Dim r As New RispostaStandard
        Try
            HttpContext.Current.Session("Dt_Codici") = strJsCodici
            r.RispostaOK = True
            r.RispostaStringa = "true"
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try
        Return r
    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Particelle(ByVal dt As DataTable) As String

        Lingua.Gias_InizializzaCultura_DaSession()

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)


        'Dim cn As New ColonneNome("Campo_Cod", "", "string")
        'cn._FormatoParticolare = "<span></span>"
        'cn._Filtrabile = False
        ''cn._css = "prova"
        'l.Add(cn)

        ''aggiungo i pulsanti per modifica ed eliminazione
        'Dim c = New ColonneNome("Campo_Cod", "Tool", "string")

        'Dim listaBtn = New List(Of btnAzioni)
        'listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaCodice(this);"))
        'listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaCodice(this);"))

        'Dim tool As New ToolStandard(listaBtn)
        'tool.cssColonna = "colmodmovimenti"
        'c._FormatoParticolare = tool.toString()
        'c._Filtrabile = False
        'c._ColonnaDiSelezione = True
        'l.Add(c)


        Dim cn As New ColonneNome("chiave", AgronicaAgenda_2010.chiave, "string")
        cn._Filtrabile = False
        cn._ColonnaDiSelezione = True
        'cn._hidden = True
        cn._css = "pos_chiave"
        'cn._css = "prova"
        l.Add(cn)


        cn = New ColonneNome("Provincia", AgronicaAgenda_2010.Provincia, "string")
        cn._Filtrabile = True
        cn._css = "pos_prov"
        l.Add(cn)

        cn = New ColonneNome("Comune", AgronicaAgenda_2010.Comune, "string")
        cn._Filtrabile = True
        cn._css = "pos_com"
        l.Add(cn)

        cn = New ColonneNome("Sezione", AgronicaAgenda_2010.Sezione, "string")
        cn._Filtrabile = True
        cn._css = "pos_sez"
        l.Add(cn)

        cn = New ColonneNome("Foglio", AgronicaAgenda_2010.Foglio, "string")
        cn._Filtrabile = True
        cn._css = "pos_fogl"
        'cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("Numero", AgronicaAgenda_2010.Numero, "string")
        cn._Filtrabile = True
        cn._css = "pos_num"
        'cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("Subalterno", AgronicaAgenda_2010.Subalterno, "string")
        cn._Filtrabile = True
        cn._css = "pos_sub"
        l.Add(cn)

        cn = New ColonneNome("Superficie", AgronicaAgenda_2010.Superficie, "number")
        cn._Filtrabile = True
        cn._css = "pos_sup"
        l.Add(cn)

        cn = New ColonneNome("SuperficieDisponibile", AgronicaAgenda_2010.SuperficieDisponibile, "number")
        cn._Filtrabile = True
        cn._css = "pos_sup_disp"
        l.Add(cn)

        cn = New ColonneNome("SuperficieDisponibile2", AgronicaAgenda_2010.SuperficieDisponibile & "2", "number")
        cn._Filtrabile = True
        cn._css = "pos_sup_disp2"
        l.Add(cn)

        cn = New ColonneNome("Progressivo", "Progressivo", "number")
        cn._Filtrabile = True
        cn._hidden = True
        cn._css = "pos_prog"
        l.Add(cn)

        cn = New ColonneNome("Part_Cod", "Part_Cod", "string")
        cn._Filtrabile = True
        cn._hidden = True
        cn._css = "pos_part_cod"
        l.Add(cn)

        cn = New ColonneNome("SuperficieImpiegata", "SuperficieImpiegata", "number")
        cn._Filtrabile = True
        cn._hidden = True
        cn._css = "pos_sup_imp"
        l.Add(cn)

        cn = New ColonneNome("checked", "checked", "string") 'i18n
        cn._css = "wacol_checked"
        'cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("SuperficieIntersezione", DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Campo_Edit.aspx", "SuperficieIntersezioneAbbr"), String), "number")
        cn._Filtrabile = True
        cn._css = "pos_sup_int"
        'cn._hidden = True
        l.Add(cn)

        'Dim cn As New ColonneNome("Campo_Cod", "", "string")
        'cn._FormatoParticolare = "<span></span>"
        'cn._Filtrabile = False
        ''cn._css = "prova"
        'l.Add(cn)

        cn = New ColonneNome("SuperficieIntersezione", "+", "string")
        cn._FormatoParticolare = "<input type='textbox' name='add_particella_val' value='{0}' class='add_particella_val' />"
        cn._Filtrabile = False
        'l.Insert(1, cn)
        l.Add(cn)


        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function




    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        permessi = New PermessiUtente()

        OperazioneSuRubrica = ""
        Master.flag_pag_Anagrafica = True



        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If


        If Request.QueryString("visibilita") IsNot Nothing AndAlso IsNumeric(Request.QueryString("visibilita")) Then
            Qs_Visibilita = CInt(Request.QueryString("visibilita"))
        End If

        'Se Qs_Visibilita = 0 è stato aperto dal Menu Anagrafe generale e quindi utilizzo la versione standard della grafica
        If Qs_Visibilita = 0 Then
            Master.Master_versione = VERSIONE_MASTER_DEFAULT
            Master.Header_versione = VERSIONE_HEADER_DEFAULT
        End If
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---


        '==================================
        '======= VERIFICA PERMESSI ========
        '==================================

        Dim UtenteAbilitato_Lettura As Boolean = False

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitato_Modifica As Boolean = False

        UtenteAbilitato_Lettura = permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Lettura
        UtenteAbilitato_Modifica = permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Scrittura

        Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica
        Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura

        If Not UtenteAbilitato_Modifica Then
            Response.Redirect("../MenuAnagrafica/Menubs_anagrafica.aspx")
            Exit Sub
        End If



        objParametriAgenda = New ParametriAgenda



        xPiva = objParametriAgenda.Piva
        xSa_Cod = objParametriAgenda.Sa_Cod

        If objParametriAgenda.Campo_Cod = "" Then
            objParametriAgenda.Campo_Cod = 0
            xCampo_Cod = 0
        Else
            xCampo_Cod = objParametriAgenda.Campo_Cod
        End If




        'If InStr(Request.QueryString.ToString, "t=") <> 0 Then
        '    Qs_CampoTipo = Stringa_Decodifica(Request.QueryString("t").ToString, _
        '                           AgroKey_EncoderDecoder, _
        '                           Server)
        'Else
        '    Qs_CampoTipo = "0"
        'End If

        If Not IsPostBack AndAlso HttpContext.Current.Session("Serra") <> "" AndAlso Operazione = enum_TipoOperazioneDB.Scrittura Then
            Qs_CampoTipo = HttpContext.Current.Session("Serra")
        End If



        'Impostazioni Serra/Campo
        Select Case Qs_CampoTipo

            Case "0"

                chk_serra.Checked = False

                Select Case objParametriAgenda.Tipo_Operazione
                    Case enum_TipoOperazioneDB.Scrittura
                        Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("CreazioneNuovoCampo"), String)
                    Case enum_TipoOperazioneDB.Lettura
                        Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("LetturaCampo"), String)
                    Case enum_TipoOperazioneDB.Modifica
                        Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("ModificaCampo"), String)
                End Select


            Case "1"

                chk_serra.Checked = True

                Select Case objParametriAgenda.Tipo_Operazione
                    Case enum_TipoOperazioneDB.Scrittura
                        Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("CreazioneNuovaSerra"), String)
                    Case enum_TipoOperazioneDB.Lettura
                        Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("LetturaSerra"), String)
                    Case enum_TipoOperazioneDB.Modifica
                        Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("ModificaSerra"), String)
                End Select


        End Select

        Operazione = objParametriAgenda.Tipo_Operazione

        Dim obj_Codici As New JArray
        Dim StrCodiciCampo As String = " codice in (" & enum_CodiciAnagrafe.Sup_Contratto & "," & enum_CodiciAnagrafe.Filiera & ") "
        Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
        Dim DTCod = objCodiceAnagrafe.Leggi(0,
                                     "",
                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     StrCodiciCampo,
                                     "",
                                     objParametri_Server)
        For Each rowCod In DTCod.Rows
            Dim jCod As New JObject
            jCod.Add(New JProperty("value", rowCod("codice")))
            jCod.Add(New JProperty("text", rowCod("descrizione")))
            obj_Codici.Add(jCod)
        Next

        cmb_Codici = obj_Codici.ToString

        If Not IsPostBack Then

            ' Riempio select Orientamento Colturale
            Cmb_OrientamentoColturale.Items.Clear()
            'i18n Per la scritta "Non Impostato" ho una risorsa in maiuscolo omonima utilizzata in "operazioni\Irrigazione.aspx.vb" a righe 229 e 2209.
            'Appena possibile rinominarli come scritta qua ed aggiornare i precedenti richiami utilizzando la funzione ToUpper()
            Cmb_OrientamentoColturale.Items.Add(New ListItem("Non Impostato", "0"))
            Cmb_OrientamentoColturale.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("MonoSpecie"), String), "1"))
            Cmb_OrientamentoColturale.Items.Add(New ListItem(DirectCast(GetLocalResourceObject("MultiSpecie"), String), "2"))

            Txt_Superficie.Text = "0"
            Txt_SuperficieSAU.Text = "0"
            Txt_SuperficiePercentuale.Text = "0"
            Txt_SuperficieCatastale.Text = "0"
            Txt_SuperficieCoperta.Text = "0"

            HttpContext.Current.Session("Appezzamenti_checked") = Nothing
            HttpContext.Current.Session("Dt_Appezzamenti") = Nothing
            HttpContext.Current.Session("Dt_Particelle") = Nothing
            HttpContext.Current.Session("Dt_Codici") = Nothing

            HttpContext.Current.Session("Specie_Vegetale") = ""

        Else
            Exit Sub
        End If




        Dim objImpreseR As New AgronicaCoreAnagrafeDAL.Imprese_Read
        objImpreseR.Recupera_DateValidita_ElementiGerarchia(
                                         enum_GerarchiaImpresa_Elementi.Gerarchia_Centro,
                                         TxtValiditaInizio.Text,
                                         TxtValiditaFine.Text,
                                         Errore,
                                         xPiva,
                                         xSa_Cod,
                                         0,
                                         0,
                                         0,
                                         objParametri_Server)

        InizioCentro.Value = TxtValiditaInizio.Text
        FineCentro.Value = TxtValiditaFine.Text

        lbl_centro_data_inizio.Text = TxtValiditaInizio.Text
        lbl_centro_data_fine.Text = TxtValiditaFine.Text

        'If TxtValiditaInizio.Text = "01/01/1900" Then
        '    TxtValiditaInizio.Text = ""
        '    lbl_centro_data_inizio.Text = ""
        'Else
        '    lbl_centro_data_inizio.Text = TxtValiditaInizio.Text
        'End If

        'If TxtValiditaFine.Text = "31/12/2100" Then
        '    TxtValiditaFine.Text = ""
        '    lbl_centro_data_fine.Text = ""
        'Else
        '    lbl_centro_data_fine.Text = TxtValiditaFine.Text
        'End If


        'Catasto
        Dim DataValiditaInizio As Date
        Dim DataValiditaFine As Date

        If TxtValiditaInizio.Text = "" Then
            DataValiditaInizio = Session("ASG_FinestraTemporale_Inizio")
        Else
            DataValiditaInizio = CDate(TxtValiditaInizio.Text)
        End If

        If TxtValiditaFine.Text = "" Then
            DataValiditaFine = Session("ASG_FinestraTemporale_Fine")
        Else
            DataValiditaFine = CDate(TxtValiditaFine.Text)
        End If


        ' Setto la gestione del campo (select)
        Opt_Senza_Catasto.Checked = True
        Opt_Con_Catasto.Checked = False



        ' Carico e controllo se ci sono Particelle per il campo
        Call CaricaGriglia_Particelle(xPiva,
                                      xSa_Cod,
                                      xCampo_Cod,
                                      DataValiditaInizio,
                                      DataValiditaFine,
                                      Me.Txt_SuperficieCatastale.Text)

        '##############################################################
        '#####  Se sono in MODIFICA carico i dati  ####################
        '##############################################################

        Dim Dt As New DataTable
        Dim Dt_checked As New DataTable


        If Operazione = enum_TipoOperazioneDB.Scrittura Then


            'Dim DataValiditaInizio As Date
            'Dim DataValiditaFine As Date


            DataValiditaInizio = Session("ASG_FinestraTemporale_Inizio")
            DataValiditaFine = Session("ASG_FinestraTemporale_Fine")


            Dim NomeCampo As String = ""

            Dim NuovoCampoCod As Integer
            Dim NuovoCampoDes As String
            Dim BaseCode As Integer
            Dim TopCode As Integer



            'prendo la differenza tra l'ultimo codice e il basecode e lo unisco alla costante
            'del campo: l'idea è di generare in automatico una descrizione alfanumerica
            'composta da una costante + un la differenza prodotta come descritto sopra... 

            '------------------------------------------------
            '----- Calcolo i valori di BaseCode e TopCode
            '------------------------------------------------

            Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))

            Dim intDiff As Integer
            intDiff = NuovoCampoCod - BaseCode

            'Right di 3, mi servono cmq sempre con formato di 3 cifre es: 001, 099, 101
            If Qs_CampoTipo = "1" Then
                NuovoCampoDes = AgroLabel_Serra & " " & Right("000" + CStr(intDiff), 3)
            Else
                NuovoCampoDes = AgroLabel_Campo & " " & Right("000" + CStr(intDiff), 3)
            End If

            TxtDenominazione.Text = NuovoCampoDes



            Dim DTRs As DataTable
            Dim objAppezzamentoR As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

            DTRs = objAppezzamentoR.Recupera_Appezzamenti_Colture_del_Campo(
                                                     xPiva,
                                                     xSa_Cod,
                                                     0,
                                                     DataValiditaInizio,
                                                     DataValiditaFine,
                                                     False,
                                                      enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                     "",
                                                     "",
                                                     objParametri_Server)

            '----- Definisco la struttura del DataTable


            Dim Dr As DataRow
            Dim MessaggioErrore As String

            Dim DataInizio As Date
            Dim DataFine As Date
            Dim strDataInizio As String
            Dim strDataFine As String

            Dim ArrayAppezza(2, 0) As String
            Dim AppezzaPresente As Boolean = False
            Dim N_Appezza As Integer = 0
            Dim i As Integer

            Dim SuperficieTotale As Double

            Dt.Columns.Add(New DataColumn("chiave", GetType(String)))
            Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
            Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("ID_Imp", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))

            Dt.Columns.Add(New DataColumn("App_Nome", GetType(String)))
            Dt.Columns.Add(New DataColumn("Sup_App", GetType(String)))

            Dt.Columns.Add(New DataColumn("Validita_Appezzamento", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Inizio_Appezzamento", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Validita_Fine_Appezzamento", GetType(Date)))
            Dt.Columns.Add(New DataColumn("ColturaCorrente", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Coltura", GetType(String)))

            Dt.Columns.Add(New DataColumn("checked", GetType(Integer)))


            '----- Carico la griglia

            Dim j As Integer
            If (Not IsNothing(DTRs)) AndAlso
               (DTRs.Rows.Count > 0) Then

                'Rs.Filter = "ID_REG = 0 OR (Impianto_Validita_Inizio<=" & DataValiditaFine & " AND Impianto_Validita_Fine>=" & DataValiditaInizio & ")"

                SuperficieTotale = 0
                For i = 0 To DTRs.Rows.Count - 1

                    AppezzaPresente = False

                    For j = 0 To UBound(ArrayAppezza, 2)

                        If ArrayAppezza(0, j) = DTRs.Rows(i).Item("Piva") And
                        ArrayAppezza(1, j) = DTRs.Rows(i).Item("Sa_Cod") And
                        ArrayAppezza(2, j) = DTRs.Rows(i).Item("Appezza") Then
                            AppezzaPresente = True
                            Exit For
                        End If

                    Next

                    If Not AppezzaPresente Then

                        ReDim Preserve ArrayAppezza(2, N_Appezza)
                        ArrayAppezza(0, N_Appezza) = DTRs.Rows(i).Item("Piva")
                        ArrayAppezza(1, N_Appezza) = DTRs.Rows(i).Item("Sa_Cod")
                        ArrayAppezza(2, N_Appezza) = DTRs.Rows(i).Item("Appezza")
                        N_Appezza += 1

                        'Creo una nuova riga
                        Dr = Dt.NewRow

                        '---

                        'Definisco i valori

                        Dr.Item("Piva") = DTRs.Rows(i).Item("Piva")
                        Dr.Item("Sa_Cod") = DTRs.Rows(i).Item("Sa_Cod")
                        Dr.Item("Appezza") = DTRs.Rows(i).Item("Appezza")
                        Dr.Item("ID_Imp") = DTRs.Rows(i).Item("ID_Reg")
                        Dr.Item("Campo_Cod") = DTRs.Rows(i).Item("Campo_Cod")

                        Dr.Item("App_Nome") = DTRs.Rows(i).Item("App_Nome")
                        Dr.Item("Sup_App") = Format(CDbl(DTRs.Rows(i).Item("Sup_App")), "0.0000")

                        '---
                        Dr.Item("checked") = 0

                        'If DTRs.Rows(i).Item("Campo_Cod") <> 0 Then
                        '    Dr.Item("checked") = 1
                        'Else
                        '    Dr.Item("checked") = 0
                        'End If

                        DataInizio = CDate(DTRs.Rows(i).Item("Validita_Inizio"))
                        DataFine = CDate(DTRs.Rows(i).Item("Validita_Fine"))

                        Dr.Item("Validita_Inizio_Appezzamento") = DataInizio.ToShortDateString
                        Dr.Item("Validita_Fine_Appezzamento") = DataFine.ToShortDateString

                        If DataInizio = #1/1/1900# Then
                            strDataInizio = "...................."
                        Else
                            strDataInizio = DataInizio.ToShortDateString
                        End If

                        If DataFine = #12/31/2100# Then
                            strDataFine = "...................."
                        Else
                            strDataFine = DataFine.ToShortDateString
                        End If

                        Dr.Item("Validita_Appezzamento") = strDataInizio & " - " & strDataFine

                        '---

                        If Not IsDBNull(DTRs.Rows(i).Item("Cul_Cod")) Then

                            If DTRs.Rows(i).Item("Cul_Cod") <> 0 Then

                                If Not IsDBNull(DTRs.Rows(i).Item("Cul_des")) Then
                                    Dr.Item("ColturaCorrente") = DTRs.Rows(i).Item("Veg_Des") & " - " & DTRs.Rows(i).Item("Cul_Des")
                                Else
                                    Dr.Item("ColturaCorrente") = " ... "
                                End If

                            Else
                                Dr.Item("ColturaCorrente") = AgronicaAgenda_2010.TerrenoNudo
                            End If

                        Else
                            Dr.Item("ColturaCorrente") = " ... "
                        End If

                        '---

                        If Not IsDBNull(DTRs.Rows(i).Item("Impianto_Validita_Inizio")) Then
                            DataInizio = CDate(DTRs.Rows(i).Item("Impianto_Validita_Inizio"))
                        Else
                            DataInizio = #1/1/1900#
                        End If


                        If Not IsDBNull(DTRs.Rows(i).Item("Impianto_Validita_Fine")) Then
                            DataFine = CDate(DTRs.Rows(i).Item("Impianto_Validita_Fine"))
                        Else
                            DataFine = #12/31/2100#
                        End If

                        If DataInizio = #1/1/1900# Then
                            strDataInizio = "...................."
                        Else
                            strDataInizio = DataInizio.ToShortDateString
                        End If

                        If DataFine = #12/31/2100# Then
                            strDataFine = "...................."
                        Else
                            strDataFine = DataFine.ToShortDateString
                        End If

                        Dr.Item("Validita_Coltura") = strDataInizio & " - " & strDataFine

                        '---


                        'Associo alla tabella la nuova riga creata
                        Dt.Rows.Add(Dr)

                        'Aggiorno il totale della superficie
                        SuperficieTotale += CDbl(DTRs.Rows(i).Item("Sup_App"))


                        Dr.Item("chiave") = DTRs.Rows(i).Item("Piva") & "_" & DTRs.Rows(i).Item("Sa_Cod") & "_" & DTRs.Rows(i).Item("Appezza") & "_" & DTRs.Rows(i).Item("ID_Reg") & "_" & DTRs.Rows(i).Item("Campo_Cod")


                    End If

                    'Prossimo record
                Next

            End If



        ElseIf Operazione = enum_TipoOperazioneDB.Modifica OrElse Operazione = enum_TipoOperazioneDB.Lettura Then

            HttpContext.Current.Session("Serra") = Nothing



            'Creo gli oggetti COM+
            Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R          'Agro_Anagrafe_AD.Campi_R
            '   *   CreateCANCELLATOObject("Agro_Anagrafe_AD.Campi_R")
            Dim DTCampi As DataTable

            'Leggo le informazioni sul campo selezionato
            DTCampi = objCampi.Leggi(
                                    CStr(xPiva),
                                    CInt(xSa_Cod),
                                    CInt(xCampo_Cod),
                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "",
                                    "",
                                    objParametri_Server)

            TxtValiditaInizio.Text = DTCampi.Rows(0).Item("Validita_Inizio")
            TxtValiditaFine.Text = DTCampi.Rows(0).Item("Validita_Fine")

            TxtDenominazione.Text = DTCampi.Rows(0).Item("Campo_Des")

            ' Cerco se ci sono codici riferiti al campo
            Dim objCodCampo As New AgronicaCoreAnagrafeDAL.Campi_Codici_Read
            Dim dt2 As DataTable = objCodCampo.Leggi(CStr(xPiva), CInt(xSa_Cod), CInt(xCampo_Cod), enum_CodiciAnagrafe.Riferimento_Alfanumerico_Campo, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            TxtCodice.Text = ""
            If dt2.Rows.Count > 0 Then
                TxtCodice.Text = dt2.Rows(0).Item("Val_Cod")
            End If


            If Not IsDBNull(DTCampi.Rows(0).Item("Gru_Cod")) Then

                Select Case DTCampi.Rows(0).Item("Gru_Cod")

                    Case -2, 0      'Terreno Nudo / Non Impostato

                        'Imposto la posizione nella combo 
                        Cmb_OrientamentoColturale.SelectedIndex =
                            Cmb_OrientamentoColturale.Items.IndexOf(
                                Cmb_OrientamentoColturale.Items.FindByValue(
                                    "0"))


                        Me.Cmb_SpecieVegetale.Items.Clear()

                    Case -1         'Multi-Specie

                        'Imposto la posizione nella combo 
                        Cmb_OrientamentoColturale.SelectedIndex =
                            Cmb_OrientamentoColturale.Items.IndexOf(
                                Cmb_OrientamentoColturale.Items.FindByValue(
                                    "2"))


                        Me.Cmb_SpecieVegetale.Items.Clear()


                    Case 1, 2, 3    'Mono-Specie

                        'Imposto la posizione nella combo 
                        Cmb_OrientamentoColturale.SelectedIndex =
                            Cmb_OrientamentoColturale.Items.IndexOf(
                                Cmb_OrientamentoColturale.Items.FindByValue(
                                    "1"))

                        ViewState("VegCod_Da_Modificare") = DTCampi.Rows(0).Item("Veg_Cod")
                        ViewState("GruCod_daModificare") = DTCampi.Rows(0).Item("Gru_Cod")




                        ''CaricaCombo_SpecieVegetale( _
                        ''                        Server, Session, Page, _
                        ''                        Me.Cmb_SpecieVegetale, RsCampi("Gru_Cod").Value)

                        ''sono in modifica devo fare attenzione a visualizzare la specie vegetale
                        ''ANCHE SE non fa parte dell'eventuale filtro associato all'utente (per cui passo il veg_cod come VegCod_Da_Modificare).
                        'CaricaListControl.SpecieVegetale_Optimize(CType(Me.Cmb_SpecieVegetale, ListControl), _
                        '                                                              True, "", "", _
                        '                                                              DTCampi.Rows(0).Item("Gru_Cod"), _
                        '                                                              "", True, 0, _
                        '                                                              DTCampi.Rows(0).Item("Veg_Cod"), _
                        '                                                              DTCampi.Rows(0).Item("Gru_Cod"), _
                        '                                                              "", "", objParametri_Server, _
                        '                                                              objParametri_Utenti)


                        CaricaListControl.SpecieVegetale_Optimize_x_PDC_Modificata2(Cmb_SpecieVegetale,
                                                                                True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", HttpContext.Current.Session("ASG_objParametri_Server"))


                        'Imposto la posizione nella combo 
                        Cmb_SpecieVegetale.SelectedIndex =
                            Cmb_SpecieVegetale.Items.IndexOf(
                                Cmb_SpecieVegetale.Items.FindByValue(
                                   "" & DTCampi.Rows(0).Item("Veg_Cod").ToString & "|" & DTCampi.Rows(0).Item("Gru_Cod").ToString & ""))

                        HttpContext.Current.Session("Specie_Vegetale") = DTCampi.Rows(0).Item("Veg_Cod") & "|" & DTCampi.Rows(0).Item("Gru_Cod")

                End Select



            Else

                'Imposto la posizione nella combo 
                Cmb_OrientamentoColturale.SelectedIndex =
                    Cmb_OrientamentoColturale.Items.IndexOf(
                        Cmb_OrientamentoColturale.Items.FindByValue(
                            "0"))

                Me.Cmb_SpecieVegetale.Items.Clear()

            End If




            'Dim DataValiditaInizio As Date
            'Dim DataValiditaFine As Date

            If TxtValiditaInizio.Text = "" Then
                DataValiditaInizio = Session("ASG_FinestraTemporale_Inizio")
            Else
                DataValiditaInizio = CDate(TxtValiditaInizio.Text)
            End If

            If TxtValiditaFine.Text = "" Then
                DataValiditaFine = Session("ASG_FinestraTemporale_Fine")
            Else
                DataValiditaFine = CDate(TxtValiditaFine.Text)
            End If


            TxtDataVariazioneAppAggr.Text = TxtValiditaInizio.Text


            Dim DTRs As DataTable
            Dim objAppezzamentoR As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

            ' Setto i check CON/SENZA catasto
            Dim objCOM As New AgronicaCoreAnagrafeDAL.CampixParticelle_R
            Dim Dt_CXP As DataTable

            Dt_CXP = objCOM.Leggi(xPiva, xSa_Cod, xCampo_Cod, "", "", "", 0, 0, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            If Dt_CXP.Rows.Count > 0 Then
                Opt_Senza_Catasto.Checked = False
                Opt_Con_Catasto.Checked = True
            Else
                Opt_Senza_Catasto.Checked = True
                Opt_Con_Catasto.Checked = False
            End If

            If Opt_Senza_Catasto.Checked = True Then

                DTRs = objAppezzamentoR.Recupera_Appezzamenti_Colture_del_Campo(
                                                         xPiva,
                                                         xSa_Cod,
                                                         xCampo_Cod,
                                                         DataValiditaInizio,
                                                         DataValiditaFine,
                                                         True,
                                                          AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                         "",
                                                         "",
                                                         objParametri_Server)

            End If


            If Opt_Con_Catasto.Checked = True Then

                If xCampo_Cod <> 0 Then

                    DTRs = objAppezzamentoR.Recupera_Appezzamenti_Colture_del_Campo(
                                                             xPiva,
                                                             xSa_Cod,
                                                             xCampo_Cod,
                                                             DataValiditaInizio,
                                                             DataValiditaFine,
                                                            False,
                                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                            "",
                                                            "",
                                                            objParametri_Server)
                End If

            End If


            '----- Definisco la struttura del DataTable


            Dim Dr As DataRow
            Dim MessaggioErrore As String

            Dim DataInizio As Date
            Dim DataFine As Date
            Dim strDataInizio As String
            Dim strDataFine As String

            Dim ArrayAppezza(2, 0) As String
            Dim AppezzaPresente As Boolean = False
            Dim N_Appezza As Integer = 0
            Dim i As Integer

            Dim SuperficieTotale As Double

            Dt.Columns.Add(New DataColumn("chiave", GetType(String)))
            Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
            Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("ID_Imp", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("App_Nome", GetType(String)))
            Dt.Columns.Add(New DataColumn("Sup_App", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Appezzamento", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Inizio_Appezzamento", GetType(Date)))
            Dt.Columns.Add(New DataColumn("Validita_Fine_Appezzamento", GetType(Date)))
            Dt.Columns.Add(New DataColumn("ColturaCorrente", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Coltura", GetType(String)))
            Dt.Columns.Add(New DataColumn("checked", GetType(Integer)))

            Dt_checked.Columns.Add(New DataColumn("chiave", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Piva", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
            Dt_checked.Columns.Add(New DataColumn("Appezza", GetType(Integer)))
            Dt_checked.Columns.Add(New DataColumn("ID_Imp", GetType(Integer)))
            Dt_checked.Columns.Add(New DataColumn("Campo_Cod", GetType(Integer)))
            Dt_checked.Columns.Add(New DataColumn("App_Nome", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Sup_App", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Validita_Appezzamento", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Validita_Inizio_Appezzamento", GetType(Date)))
            Dt_checked.Columns.Add(New DataColumn("Validita_Fine_Appezzamento", GetType(Date)))
            Dt_checked.Columns.Add(New DataColumn("ColturaCorrente", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("Validita_Coltura", GetType(String)))
            Dt_checked.Columns.Add(New DataColumn("checked", GetType(Integer)))



            '----- Carico la griglia

            Dim j As Integer
            If (Not IsNothing(DTRs)) AndAlso
               (DTRs.Rows.Count > 0) Then

                'Rs.Filter = "ID_REG = 0 OR (Impianto_Validita_Inizio<=" & DataValiditaFine & " AND Impianto_Validita_Fine>=" & DataValiditaInizio & ")"

                SuperficieTotale = 0
                For i = 0 To DTRs.Rows.Count - 1

                    AppezzaPresente = False

                    For j = 0 To UBound(ArrayAppezza, 2)

                        If ArrayAppezza(0, j) = DTRs.Rows(i).Item("Piva") And
                        ArrayAppezza(1, j) = DTRs.Rows(i).Item("Sa_Cod") And
                        ArrayAppezza(2, j) = DTRs.Rows(i).Item("Appezza") Then
                            AppezzaPresente = True
                            Exit For
                        End If

                    Next

                    If Not AppezzaPresente Then

                        ReDim Preserve ArrayAppezza(2, N_Appezza)
                        ArrayAppezza(0, N_Appezza) = DTRs.Rows(i).Item("Piva")
                        ArrayAppezza(1, N_Appezza) = DTRs.Rows(i).Item("Sa_Cod")
                        ArrayAppezza(2, N_Appezza) = DTRs.Rows(i).Item("Appezza")
                        N_Appezza += 1

                        'Creo una nuova riga
                        Dr = Dt.NewRow

                        '---

                        'Definisco i valori

                        Dr.Item("Piva") = DTRs.Rows(i).Item("Piva")
                        Dr.Item("Sa_Cod") = DTRs.Rows(i).Item("Sa_Cod")
                        Dr.Item("Appezza") = DTRs.Rows(i).Item("Appezza")
                        Dr.Item("ID_Imp") = DTRs.Rows(i).Item("ID_Reg")
                        Dr.Item("Campo_Cod") = DTRs.Rows(i).Item("Campo_Cod")

                        Dr.Item("App_Nome") = DTRs.Rows(i).Item("App_Nome")
                        Dr.Item("Sup_App") = Format(CDbl(DTRs.Rows(i).Item("Sup_App")), "0.0000")

                        '---

                        If DTRs.Rows(i).Item("Campo_Cod") <> 0 Then
                            Dr.Item("checked") = 1
                        Else
                            Dr.Item("checked") = 0
                        End If

                        DataInizio = CDate(DTRs.Rows(i).Item("Validita_Inizio"))
                        DataFine = CDate(DTRs.Rows(i).Item("Validita_Fine"))

                        Dr.Item("Validita_Inizio_Appezzamento") = DataInizio.ToShortDateString
                        Dr.Item("Validita_Fine_Appezzamento") = DataFine.ToShortDateString

                        If DataInizio = #1/1/1900# Then
                            strDataInizio = "...................."
                        Else
                            strDataInizio = DataInizio.ToShortDateString
                        End If

                        If DataFine = #12/31/2100# Then
                            strDataFine = "...................."
                        Else
                            strDataFine = DataFine.ToShortDateString
                        End If

                        Dr.Item("Validita_Appezzamento") = strDataInizio & " - " & strDataFine

                        '---

                        If Not IsDBNull(DTRs.Rows(i).Item("Cul_Cod")) Then

                            If DTRs.Rows(i).Item("Cul_Cod") <> 0 Then

                                If Not IsDBNull(DTRs.Rows(i).Item("Cul_des")) Then
                                    Dr.Item("ColturaCorrente") = DTRs.Rows(i).Item("Veg_Des") & " - " & DTRs.Rows(i).Item("Cul_Des")
                                Else
                                    Dr.Item("ColturaCorrente") = " ... "
                                End If

                            Else
                                Dr.Item("ColturaCorrente") = AgronicaAgenda_2010.TerrenoNudo
                            End If

                        Else
                            Dr.Item("ColturaCorrente") = " ... "
                        End If

                        '---

                        If Not IsDBNull(DTRs.Rows(i).Item("Impianto_Validita_Inizio")) Then
                            DataInizio = CDate(DTRs.Rows(i).Item("Impianto_Validita_Inizio"))
                        Else
                            DataInizio = #1/1/1900#
                        End If


                        If Not IsDBNull(DTRs.Rows(i).Item("Impianto_Validita_Fine")) Then
                            DataFine = CDate(DTRs.Rows(i).Item("Impianto_Validita_Fine"))
                        Else
                            DataFine = #12/31/2100#
                        End If

                        If DataInizio = #1/1/1900# Then
                            strDataInizio = "...................."
                        Else
                            strDataInizio = DataInizio.ToShortDateString
                        End If

                        If DataFine = #12/31/2100# Then
                            strDataFine = "...................."
                        Else
                            strDataFine = DataFine.ToShortDateString
                        End If

                        Dr.Item("Validita_Coltura") = strDataInizio & " - " & strDataFine

                        '---


                        'Associo alla tabella la nuova riga creata
                        Dt.Rows.Add(Dr)
                        'Dt.ImportRow(Dr)



                        'Aggiorno il totale della superficie
                        SuperficieTotale += CDbl(DTRs.Rows(i).Item("Sup_App"))

                        Dr.Item("Piva") = DTRs.Rows(i).Item("Piva")
                        Dr.Item("Sa_Cod") = DTRs.Rows(i).Item("Sa_Cod")
                        Dr.Item("Appezza") = DTRs.Rows(i).Item("Appezza")
                        Dr.Item("ID_Imp") = DTRs.Rows(i).Item("ID_Reg")
                        Dr.Item("Campo_Cod") = DTRs.Rows(i).Item("Campo_Cod")

                        Dr.Item("chiave") = DTRs.Rows(i).Item("Piva") & "_" & DTRs.Rows(i).Item("Sa_Cod") & "_" & DTRs.Rows(i).Item("Appezza") & "_" & DTRs.Rows(i).Item("ID_Reg") & "_" & DTRs.Rows(i).Item("Campo_Cod")


                        ' Aggiungo la stessa riga se è checked
                        If Dr.Item("checked") = 1 Then
                            'Dt_checked.Rows.Add(Dr)
                            Dt_checked.ImportRow(Dr)
                        End If

                    End If

                    'Prossimo record
                Next

            End If

            lblSupUtil.Text = Format(SuperficieTotale, "0.0000")

            If Opt_Con_Catasto.Checked = True Then
                lblRapporto.Text = Math.Round(CDbl(lblSupUtil.Text) / CDbl(lblSupCatasto.Text) * 100, 1)
            End If

            If Opt_Senza_Catasto.Checked = True Then
                lblRapporto.Text = 100
            End If


        End If

        HttpContext.Current.Session("Dt_Appezzamenti") = Dt
        HttpContext.Current.Session("Appezzamenti_checked") = Dt_checked

        ' Controllo se ci sono iontersezioni fra papezzamenti (quindi gestione catastale)
        'visible_Campo_Cod = Controllo_Intersezioni_Appezzamenti()

        jsGestCat = DT_to_Json_GestCat(Dt)

        Dim dt_Codici As New DataTable

        Carica_CodiciCampo(dt_Codici)

        jsCodici = DT_to_Json_Codici(dt_Codici)

        HttpContext.Current.Session("Dt_Codici") = jsCodici


        If Operazione = enum_TipoOperazioneDB.Lettura Then

            chk_serra.Enabled = False
            TxtValiditaInizio.Enabled = False
            TxtValiditaFine.Enabled = False
            TxtDenominazione.Enabled = False
            Cmb_OrientamentoColturale.Enabled = False
            Cmb_SpecieVegetale.Enabled = False
            Txt_Superficie.Enabled = False
            Txt_SuperficieSAU.Enabled = False
            Txt_SuperficiePercentuale.Enabled = False
            Txt_SuperficieCoperta.Enabled = False
            TxtDataVariazioneAppAggr.Enabled = False


        End If


    End Sub

    Private Sub Carica_CodiciCampo(ByRef dt As DataTable)
        dt = Nothing
        dt = New DataTable
        Dim Operazione = objParametriAgenda.Tipo_Operazione

        dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))

        Select Case Operazione
            Case enum_TipoOperazioneDB.Modifica, enum_TipoOperazioneDB.Lettura
                Dim objCodici As New AgronicaCoreAnagrafeDAL.Campi_Codici_Read
                Dim dtCod = objCodici.Leggi(xPiva,
                                xSa_Cod,
                                xCampo_Cod, 0, "",
                                AGRODATAINIZIO, AGRODATAFINE,
                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                " id_cod <> " & enum_CodiciAnagrafe.Riferimento_Alfanumerico_Campo & " ", "", objParametri_Server)

                For Each codice In dtCod.Rows
                    Dim rowCodice = dt.NewRow()

                    rowCodice("Id_Cod") = codice("id_Cod")
                    rowCodice("Descrizione") = codice("descrizione")
                    rowCodice("Val_Cod") = codice("Val_Cod")

                    dt.Rows.Add(rowCodice)
                Next

        End Select

    End Sub


    '##########################################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim TargetUrl As String
        TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda, , Qs_Visibilita)
        Response.Redirect(TargetUrl)

    End Sub

    Private Sub Centro_Edit_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        ' VAnni: 2/10/2017: a seguito di AgroMasterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        Master.flag_MostraBtnIndietro = True
    End Sub




    '########################################################################################
    Private Sub ImgBtn_SalvaTutto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SalvaTutto.Click

        Salva_Tutto()

    End Sub




    Private Sub Salva_Tutto()

        Dim OK As Boolean = False

        Dim StrCampo As String
        Dim StrXmlInserisci As String

        Dim Operazione As enum_TipoOperazioneDB
        ''''Dim Indice As Integer

        Dim TipoOperazioneDB As enum_TipoOperazioneDB
        Dim Piva As String
        Dim Sa_Cod As Integer
        Dim Campo_Cod As Integer
        Dim Campo_Tipo As Integer
        Dim Campo_Des As String

        Dim Gru_Cod As Integer
        Dim Veg_Cod As Integer

        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date
        Dim BaseCode As Integer
        Dim TopCode As Integer

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlDatiCampi As System.Xml.XmlElement

        'Dim j As Integer
        'Dim AppezzaCorrente As Integer

        'Dim SommaSuperfici As Double = 0
        Dim SuperficieCampo As Double
        'Dim Percentuale As Double

        Dim i As Integer




        '------------------------------------------------
        '----- Verifico l'operazione richiesta (Inserimento / Modifica / Cancellazione)
        '------------------------------------------------

        'Recupero l'operazione richiesta, dalla querystring
        Operazione = objParametriAgenda.Tipo_Operazione


        '------------------------------------------------
        '----- Verifico la correttezza delle informazioni inserite
        '------------------------------------------------

        Dim MessaggioErrore As String = ""


        '--- Date di validita

        If Not IsDate(TxtValiditaInizio.Text) Then
            Validita_Inizio = AGRODATAINIZIO
        Else
            Validita_Inizio = CDate(TxtValiditaInizio.Text)
        End If

        If Not IsDate(TxtValiditaFine.Text) Then
            Validita_Fine = AGRODATAFINE
        Else
            Validita_Fine = CDate(TxtValiditaFine.Text)
        End If

        If Validita_Fine < Validita_Inizio Then
            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("FineDelCampoPrecedenteAinizio"), String) & vbCrLf
        End If

        If Validita_Inizio < CDate(InizioCentro.Value) Then
            MessaggioErrore += "   - " & AgronicaAgenda_2010.InizioAttivitaNonPuòPrecedereCreazioneCentroAziendale & ". (" & InizioCentro.Value & ")" & vbCrLf
        End If

        If Validita_Fine > CDate(FineCentro.Value) Then
            MessaggioErrore += "   - " & AgronicaAgenda_2010.FineAttivitaNonPuòSeguireCessazioneCentroAziendale & ". (" & FineCentro.Value & ")" & vbCrLf
        End If


        '----- Superficie Coperta


        If Not IsNumeric(Txt_SuperficieCoperta.Text) Then
            MessaggioErrore += DirectCast(GetLocalResourceObject("InserireUnValoreNumericoPerLaSuperficieCoperta"), String)
        End If


        If Txt_SuperficieCoperta.Text < 0 Then
            MessaggioErrore += DirectCast(GetLocalResourceObject("ImpossibileInserireUnaSuperficieCopertaNullaONegativa"), String)
        End If

        If InStr(Me.Txt_SuperficieCoperta.Text, ".") <> 0 Then
            MessaggioErrore += DirectCast(GetLocalResourceObject("PerInserireNellaSuperficieCopertaUnaCifraDecimaleUtilizzareLaVirgola"), String)
        End If


        'Se stò utilizzando il Wizard per creare Appezzamenti/Impianti effettuo i controlli
        'If DataGrid_NuoviAppezzamenti.Items.Count > 0 Then

        '    Call Controlla_FrazionamentoMultiplo(MessaggioErrore)

        'End If


        '--- RIEPILOGO

        Dim Messaggio As String

        If MessaggioErrore <> "" Then

            Messaggio = ""
            Messaggio += AgronicaAgenda_2010.SonoStatiRilevatiISeguentiErrori_ & vbCrLf
            Messaggio += "" & vbCrLf
            Messaggio += MessaggioErrore
            Messaggio += "" & vbCrLf
            Messaggio += AgronicaAgenda_2010.RitentareIlSalvataggioDopoLaCorrezione
            Throw New Exception(Messaggio)
            AgroMsgBox(Messaggio, Page)

            Exit Sub

        End If


        '------------------------------------------------
        '----- apro connessione e transazione
        '------------------------------------------------
        ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)


        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------

        Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))



        '------------------------------------------------
        '----- Costruisco la stringa XML del CAMPO
        '------------------------------------------------

        'Definisco il tipo di operazione da eseguire
        TipoOperazioneDB = Operazione

        '--SE TipoOperazioneDB è modifica prendo il campo_tipo
        'se no prendo il qs_campo_tipo

        'Prelevo le informazioni immediate
        TipoOperazioneDB = Operazione

        Select Case TipoOperazioneDB
            Case enum_TipoOperazioneDB.Scrittura
                xCampo_Cod = 0
            Case enum_TipoOperazioneDB.Modifica
                If xCampo_Cod = 0 Then
                    Throw New Exception("Il campo_cod non può essere 0 in una operazione di modifica")
                End If
        End Select

        Piva = xPiva
        Sa_Cod = xSa_Cod
        Campo_Cod = xCampo_Cod
        If chk_serra.Checked Then
            Qs_CampoTipo = "1"
        Else
            Qs_CampoTipo = "0"
        End If
        Campo_Tipo = CInt(Qs_CampoTipo)
        Campo_Des = TxtDenominazione.Text
        If Trim(Campo_Des) = "" AndAlso Campo_Tipo = 1 Then
            Campo_Des = "Serra 001"
        End If

        Select Case Qs_CampoTipo
            Case "1"
                SuperficieCampo = Txt_SuperficieCoperta.Text
            Case Else
                SuperficieCampo = 0
        End Select



        Select Case Me.Cmb_OrientamentoColturale.SelectedValue
            Case 0  '"Non Impostato"
                Gru_Cod = 0
                Veg_Cod = 0
            Case 1  '"Mono-Specie"

                'Dim spec_veg As String() = Cmb_SpecieVegetale.SelectedValue.Split(New Char() {"|"c})

                Dim spec_veg As String() = HttpContext.Current.Session("Specie_Vegetale").Split(New Char() {"|"c})
                Gru_Cod = spec_veg(1)
                Veg_Cod = spec_veg(0)

                'If Me.Cmb_GruppoVegetale.SelectedValue = "" Then
                '    Gru_Cod = 0
                '    Veg_Cod = 0
                'Else
                '    If Me.Cmb_SpecieVegetale.SelectedValue = "" Then
                '        Gru_Cod = 0
                '        Veg_Cod = 0
                '    Else
                '        Gru_Cod = Me.Cmb_GruppoVegetale.SelectedValue
                '        Veg_Cod = Me.Cmb_SpecieVegetale.SelectedValue
                '    End If
                'End If

            Case 2  '"Multi-Specie"
                Gru_Cod = -1
                Veg_Cod = 0

        End Select

        '---
        Dim objxml As New AgronicaCoreXML.XML_Anagrafe
        'Genero la stringa XML
        objxml.XML_Campo_3(enum_CodificaDecodifica.Codifica,
                        StrCampo,
                        TipoOperazioneDB,
                        Piva,
                        Sa_Cod,
                        Campo_Cod,
                        Campo_Tipo,
                        Campo_Des,
                        #1/1/1900#,
                        #1/1/1900#,
                        SuperficieCampo,
                        0,
                        0,
                        0,
                        "",
                        Gru_Cod,
                        Veg_Cod,
                        Validita_Inizio,
                        Validita_Fine,
                        BaseCode,
                        TopCode)



        Dim Id_Cod As String = enum_CodiciAnagrafe.Riferimento_Alfanumerico_Campo
        Dim Val_Cod As String = TxtCodice.Text
        Dim XmlCampoCodice As System.Xml.XmlElement
        Dim xmlCampiCodice As String = ""


        If Operazione = enum_TipoOperazioneDB.Modifica Then
            Dim objCodCampo As New AgronicaCoreAnagrafeDAL.Campi_Codici_Read
            Dim dt As DataTable = objCodCampo.Leggi(Piva,
                                                    Sa_Cod,
                                                    Campo_Cod,
                                                    0,
                                                    0,
                                                    AGRODATAINIZIO,
                                                    AGRODATAFINE,
                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    "",
                                                    "",
                                                    objParametri_Server)

            Dim Validita_Inizio_Codice As Date
            Dim Validita_Fine_Codice As Date
            Dim Log_Errori As String = ""
            Dim XmlDoc2 As New XmlDocument

            For Each rows In dt.Rows
                XmlCampoCodice = objxml.XML_Campo_Codice(Log_Errori,
                                                        CStr(enum_TipoOperazioneDB.Cancellazione),
                                                        Piva,
                                                        Sa_Cod,
                                                        rows("id_cod"),
                                                        rows("val_cod"),
                                                        BaseCode,
                                                        TopCode,
                                                        XmlDoc2,
                                                        0,
                                                        rows("xValidita_Inizio"),
                                                       rows("xValidita_Fine"))
                xmlCampiCodice &= XmlCampoCodice.OuterXml
            Next
        End If

        'Dim xmlD As New XmlDocument
        'xmlD.LoadXml(StrCampo)

        ' @Paolo
        ' Controllo se il campo codice non è vuoto ---> aggiungo nuovo codice
        If Val_Cod <> "" Then

            '#######################################################
            '#############   CodiceCampo    #################
            '#######################################################

            'possono essere tanti nodi codice

            ' If Not IsNothing(DT_Codici_Appezzamento) AndAlso DT_Codici_Appezzamento.Rows.Count <> 0 Then


            Dim Validita_Inizio_Codice As Date
            Dim Validita_Fine_Codice As Date
            Dim Log_Errori As String = ""
            Dim XmlDoc2 As New XmlDocument

            'Dim TipoOperazioneDB_Codice As enum_TipoOperazioneDB



            'For i = 0 To DT_Codici_Appezzamento.Rows.Count - 1

            'TipoOperazioneDB_Codice = DT_Codici_Appezzamento.Rows(i).Item("TipoOperazioneDB")

            ' Id_Cod = DT_Codici_Appezzamento.Rows(i).Item("Id_Cod")
            ' Val_Cod = DT_Codici_Appezzamento.Rows(i).Item("Val_Cod")
            Validita_Inizio_Codice = Validita_Inizio
            Validita_Fine_Codice = Validita_Fine




            ' Paolo
            ' Controllo se esiste già un record per il codice ---> imposto la tipologia di operazione
            Dim objCodCampo As New AgronicaCoreAnagrafeDAL.Campi_Codici_Read
            Dim dt As DataTable = objCodCampo.Leggi(Piva, Sa_Cod, Campo_Cod, Id_Cod, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Dim oper As enum_TipoOperazioneDB
            oper = enum_TipoOperazioneDB.Scrittura

            XmlCampoCodice = objxml.XML_Campo_Codice(Log_Errori,
                                                    oper,
                                                    Piva,
                                                    Sa_Cod,
                                                    Id_Cod,
                                                    Val_Cod,
                                                    BaseCode,
                                                    TopCode,
                                                    XmlDoc2,
                                                    0,
                                                    Validita_Inizio_Codice,
                                                   Validita_Fine_Codice)

            ''StrCampo = StrCampo & XmlCampoCodice.OuterXml

            'Dim results = StrCampo.Split(New String() {"</Campo>"}, StringSplitOptions.None)
            'StrCampo = results(0) & XmlCampoCodice.OuterXml & "</Campo>"

            ''xmlD.ImportNode(XmlCampoCodice, True)
            ''StrCampo = xmlD.InnerXml

            'Next
            xmlCampiCodice &= XmlCampoCodice.OuterXml

        End If

        jsCodici = HttpContext.Current.Session("Dt_Codici")

        If jsCodici <> "" Then
            Dim Validita_Inizio_Codice As Date
            Dim Validita_Fine_Codice As Date
            Dim Log_Errori As String = ""
            Dim XmlDoc2 As New XmlDocument

            Dim arrayCodici = JArray.Parse(jsCodici)

            Validita_Inizio_Codice = Validita_Inizio
            Validita_Fine_Codice = Validita_Fine

            For Each objCodice In arrayCodici
                Dim s_val_cod = objCodice("Val_Cod")
                Dim s_id_Cod = objCodice("Id_Cod")

                XmlCampoCodice = objxml.XML_Campo_Codice(Log_Errori,
                                                        CStr(enum_TipoOperazioneDB.Scrittura),
                                                        Piva,
                                                        Sa_Cod,
                                                        s_id_Cod,
                                                        s_val_cod,
                                                        BaseCode,
                                                        TopCode,
                                                        XmlDoc2,
                                                        0,
                                                        Validita_Inizio_Codice,
                                                       Validita_Fine_Codice)
                xmlCampiCodice &= XmlCampoCodice.OuterXml
            Next

        End If

        '------------------------------------------------
        '----- Costruisco la stringa XML complessiva di inserimento
        '------------------------------------------------

        'Creo il nodo "DatiImprese"
        XmlDatiCampi = XmlDoc.CreateElement("DatiCampi")

        'Inserisco il nodo "Campo"
        XmlDatiCampi.InnerXml = StrCampo

        'Rendo l'albero figlio del documento
        XmlDoc.AppendChild(XmlDatiCampi)

        If xmlCampiCodice <> "" Then
            ''seleziono il nodo campo e all'interno inserisco i nodi figli...sintassi xpath
            Dim xmlCampo As System.Xml.XmlElement = XmlDoc.SelectSingleNode("//Campo")
            ''aggiungo i codici creati in precedenza...

            xmlCampo.InnerXml = xmlCampiCodice
        End If

        'Estraggo la stringa XML complessiva
        StrXmlInserisci = XmlDoc.InnerXml

        '------------------------------------------------
        '----- 
        '------------------------------------------------

        'Distruggo gli oggetti
        XmlDatiCampi = Nothing
        XmlDoc = Nothing




        '=======================
        '===  Aggiornamento  ===
        '=======================

        Try


            '------------------------------------------------
            '-----
            '------------------------------------------------

            '    'Se sono in modifica cancello l'associazione degli appezzamenti con il campo

            '    Call Disconnetti_Appezzamenti(Piva, Sa_Cod, Campo_Cod, MessaggioErrore)


            '------------------------------------------------
            '-----
            '------------------------------------------------

            Dim NomeCampo As String = Campo_Des

            Dim NuovoCampoCod As Integer
            Dim NuovoCampoDes As String = Campo_Des


            '------------------------------------------------
            '----- Modifico o Inserisco il Campo
            '------------------------------------------------

            Dim objInsCampo As New AgronicaCoreAnagrafeBIZ.Campo_W 'New Agro_Anagrafe.Campo_W
            '   *   CreateCANCELLATOObject("Agro_Anagrafe.Campo_W")
            Dim Esito As Boolean

            'Eseguo i comandi XML
            Esito = objInsCampo.Campo_Scrivi(
                                            CStr(StrXmlInserisci),
                                            Nothing,
                                            Nothing,
                                                NuovoCampoCod,
                                                False,
                                                   objParametri_Server,
                                                   objParametri_Utenti)


            'NOTA
            'Controllo se l'operazione corrente è di scrittura e se il campo è vuoto..
            'In quel caso genero il nome del campo

            If (Operazione = enum_TipoOperazioneDB.Scrittura) Then

                Dim objModCampo As New AgronicaCoreAnagrafeDAL.Campi_W  'new Agro_Anagrafe_AD.Campi_W 
                '   *   CreateCANCELLATOObject("Agro_Anagrafe_AD.Campi_W") 

                objModCampo.Modifica(CStr(Piva),
                                     CInt(Sa_Cod),
                                     CInt(NuovoCampoCod),
                                     CStr(NuovoCampoDes),
                                     CInt(Gru_Cod),
                                     CInt(Veg_Cod),
                                     CDbl(0),
                                     CDbl(0),
                                     CDbl(0),
                                     CDbl(0),
                                     AGRODATAINIZIO,
                                     AGRODATAFINE,
                                     CStr(""),
                                     CInt(Qs_CampoTipo),
                                     Validita_Inizio,
                                        Validita_Fine,
                                        "",
                                        objParametri_Server)

                'NomeCampo = NuovoCampoDes

            End If


            '------------------------------------------------
            '----- Inserisco le associazioni Campo-Appezzamenti
            '------------------------------------------------

            'Dim Campo_Cod As Integer
            If Campo_Cod = 0 Then
                Campo_Cod = NuovoCampoCod
            End If




            Dim DataGridAppezzamenti_check As DataTable
            Dim DataGridAppezzamenti As DataTable

            If Not IsNothing(HttpContext.Current.Session("Appezzamenti_checked")) Then
                DataGridAppezzamenti_check = HttpContext.Current.Session("Appezzamenti_checked")
            End If

            If Not IsNothing(HttpContext.Current.Session("Dt_Appezzamenti")) Then
                DataGridAppezzamenti = HttpContext.Current.Session("Dt_Appezzamenti")
            End If


            'For i = 0 To DataGridAppezzamenti_check.Rows.Count - 1

            '    If CInt(DataGridAppezzamenti_check.Rows(i).Item("checked")) = 0 Then
            '        '----> aggrego
            '        Aggrega(xPiva, _
            '                xSa_Cod, _
            '                CInt(DataGridAppezzamenti_check.Rows(i).Item("Appezza")), _
            '                xCampo_Cod, _
            '                DataGridAppezzamenti_check.Rows(i).Item("Validita_Inizio_Appezzamento"), _
            '                DataGridAppezzamenti_check.Rows(i).Item("Validita_Fine_Appezzamento"), _
            '                TxtDataVariazioneAppAggr.Text)
            '    End If

            'Next


            'For i = 0 To DataGridAppezzamenti.Rows.Count - 1

            '    Dim trovato As Boolean = False

            '    ' Controllo se il campo checked = 1 
            '    If CInt(DataGridAppezzamenti.Rows(i).Item("checked")) = 1 Then

            '        Dim id_riga As String = DataGridAppezzamenti.Rows(i).Item("chiave")

            '        For j = 0 To DataGridAppezzamenti_check.Rows.Count - 1

            '            ' Controllo se l'appezzamento è stato checked...
            '            If id_riga = DataGridAppezzamenti_check.Rows(i).Item("chiave") Then
            '                trovato = True
            '            End If

            '        Next

            '    End If

            '    '... se non è stato trovato ---> disgrega
            '    If trovato Then
            '        Disaggrega(xPiva, _
            '                xSa_Cod, _
            '                CInt(DataGridAppezzamenti.Rows(i).Item("Appezza")), _
            '                xCampo_Cod, _
            '                DataGridAppezzamenti.Rows(i).Item("Validita_Inizio_Appezzamento"), _
            '                DataGridAppezzamenti.Rows(i).Item("Validita_Fine_Appezzamento"), _
            '                TxtDataVariazioneAppAggr.Text)

            '    End If

            'Next

            Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            ' Ciclo tutti gli Appezzamenti disponibili (base)
            For i = 0 To DataGridAppezzamenti.Rows.Count - 1

                Dim isChecked As Boolean = False



                ' Appezzamento base non checked (0)
                If DataGridAppezzamenti.Rows(i).Item("checked") = 0 Then

                    ' Ciclo su tutti gli Appezzamenti checked 
                    For j = 0 To DataGridAppezzamenti_check.Rows.Count - 1

                        ' Vedo se è stato checked il corrente...
                        If DataGridAppezzamenti_check.Rows(j).Item("chiave") = DataGridAppezzamenti.Rows(i).Item("chiave") Then
                            isChecked = True
                        End If

                    Next

                    If isChecked Then
                        '----> aggrego

                        Dim dtAppezza As DataTable = objAppezza.Leggi(xPiva, xSa_Cod, CInt(DataGridAppezzamenti.Rows(i).Item("Appezza")), enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                        If dtAppezza.Rows.Count > 0 Then

                            If dtAppezza.Rows(0)("Validita_Inizio") < Validita_Inizio Then

                                Throw New Exception(DirectCast(GetLocalResourceObject("LaValiditàInizioNonComprendeAlcuniAppezzamentiSelezionati"), String))

                            End If

                            If dtAppezza.Rows(0)("Validita_Fine") > Validita_Fine Then

                                Throw New Exception(DirectCast(GetLocalResourceObject("LaValiditàFinaleNonComprendeAlcuniAppezzamentiSelezionati"), String))

                            End If

                            Aggrega(xPiva,
                                xSa_Cod,
                                CInt(DataGridAppezzamenti.Rows(i).Item("Appezza")),
                                Campo_Cod,
                                DataGridAppezzamenti.Rows(i).Item("Validita_Inizio_Appezzamento"),
                                DataGridAppezzamenti.Rows(i).Item("Validita_Fine_Appezzamento"),
                                TxtDataVariazioneAppAggr.Text)

                        End If

                    End If


                    ' Appezzamento base checked (1)
                Else

                    ' Ciclo su tutti gli Appezzamenti checked 
                    For j = 0 To DataGridAppezzamenti_check.Rows.Count - 1

                        ' Vedo se è stato checked il corrente...
                        If DataGridAppezzamenti_check.Rows(j).Item("chiave") = DataGridAppezzamenti.Rows(i).Item("chiave") Then
                            isChecked = True
                        End If

                    Next


                    If isChecked Then
                        '----> aggrego

                        Dim dtAppezza As DataTable = objAppezza.Leggi(xPiva, xSa_Cod, CInt(DataGridAppezzamenti.Rows(i).Item("Appezza")), enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                        If dtAppezza.Rows.Count > 0 Then

                            If dtAppezza.Rows(0)("Validita_Inizio") < Validita_Inizio Then

                                Throw New Exception(DirectCast(GetLocalResourceObject("LaValiditàInizioNonComprendeAlcuniAppezzamentiSelezionati"), String))

                            End If

                            If dtAppezza.Rows(0)("Validita_Fine") > Validita_Fine Then

                                Throw New Exception(DirectCast(GetLocalResourceObject("LaValiditàFinaleNonComprendeAlcuniAppezzamentiSelezionati"), String))

                            End If

                            Aggrega(xPiva,
                                xSa_Cod,
                                CInt(DataGridAppezzamenti.Rows(i).Item("Appezza")),
                                Campo_Cod,
                                DataGridAppezzamenti.Rows(i).Item("Validita_Inizio_Appezzamento"),
                                DataGridAppezzamenti.Rows(i).Item("Validita_Fine_Appezzamento"),
                                TxtDataVariazioneAppAggr.Text)
                        End If

                    Else

                        '----> disaggrego
                        Disaggrega(xPiva,
                                xSa_Cod,
                                CInt(DataGridAppezzamenti.Rows(i).Item("Appezza")),
                                Campo_Cod,
                                DataGridAppezzamenti.Rows(i).Item("Validita_Inizio_Appezzamento"),
                                DataGridAppezzamenti.Rows(i).Item("Validita_Fine_Appezzamento"),
                                TxtDataVariazioneAppAggr.Text)

                    End If

                End If

            Next



            '------------------------------------------------
            '----- Inserisco le Intersezioni Campo / Particelle Catastali
            '------------------------------------------------

            Dim SuperficieIntersezione As Double
            Dim SuperficieDisponibile As Double
            Dim SuperficieImpiegata As Double

            Dim Prov As String
            Dim Com As String
            Dim Sezione As String
            Dim Foglio As Integer
            Dim Numero As Integer
            Dim Subalterno As String

            Dim Ettari As Integer
            Dim Are, Centiare As Integer

            Dim intDummy As Integer
            Dim Testo As String

            Dim ObjPartW As New AgronicaCoreAnagrafeDAL.CampixParticelle_W
            '   *   CreateCANCELLATOObject("Agro_Anagrafe_AD.CampixParticelle_W")


            '----- Se sono in modifica ... Cancello tutte le intersezioni

            If Operazione = enum_TipoOperazioneDB.Modifica Then

                ObjPartW.Cancella(
                                      CStr(xPiva),
                                      CInt(xSa_Cod),
                                      CInt(NuovoCampoCod),
                                      "", "", "", 0, 0, "",
                                      "",
                                      objParametri_Server)

            End If




            '-----  poi le reinserisco ....


            Dim DataGridParticelle As DataTable

            If Not IsNothing(HttpContext.Current.Session("Dt_Particelle")) Then
                DataGridParticelle = HttpContext.Current.Session("Dt_Particelle")
            End If


            'Se(e) ' attivata l'opzione SQUADRO ...
            If Opt_Con_Catasto.Checked = True Then

                'Se ci sono particelle ...
                If DataGridParticelle.Rows.Count > 0 Then

                    'Per ciascuna particella ...
                    For i = 0 To DataGridParticelle.Rows.Count - 1

                        'Se il check e' selezionato ...
                        'If CType(DataGridParticelle.Items(i).FindControl("ChkSelezionaParticella"), CheckBox).Checked = True Then
                        If DataGridParticelle.Rows(i).Item("checked") = 1 Then

                            'Verifico quanto e' stato inserito come intersezione (eventualmente uso tutta la qta' disponibile)

                            Prov = DataGridParticelle.Rows(i).Item("CodiceIstat_Provincia")
                            Com = DataGridParticelle.Rows(i).Item("CodiceIstat_Comune")
                            Sezione = DataGridParticelle.Rows(i).Item("Sezione")
                            Foglio = DataGridParticelle.Rows(i).Item("Foglio")
                            Numero = DataGridParticelle.Rows(i).Item("Numero")
                            Subalterno = DataGridParticelle.Rows(i).Item("Subalterno")

                            If Sezione = "&nbsp;" Then Sezione = "0"
                            If Subalterno = "&nbsp;" Then Subalterno = "0"


                            Testo = DataGridParticelle.Rows(i).Item("SuperficieIntersezione")

                            If Testo = "" Then
                                SuperficieIntersezione = 0
                            Else
                                If Not IsNumeric(Testo) Then
                                    SuperficieIntersezione = 0
                                Else
                                    Testo = Testo.Replace(".", ",")
                                    SuperficieIntersezione = CDbl(Testo)
                                End If
                            End If

                            SuperficieDisponibile = CDbl(DataGridParticelle.Rows(i).Item("SuperficieDisponibile"))
                            SuperficieImpiegata = CDbl(DataGridParticelle.Rows(i).Item("SuperficieImpiegata"))

                            ''Non consento di superare il limite ...
                            'If SuperficieIntersezione > SuperficieDisponibile + SuperficieImpiegata Then
                            '    SuperficieIntersezione = SuperficieDisponibile + SuperficieImpiegata
                            'End If

                            ''Verifico se sia il caso di salvare ...
                            ''If (SuperficieIntersezione <> 0) And _
                            'If (SuperficieIntersezione <= SuperficieDisponibile + SuperficieImpiegata) Then

                            'Salvo l'intersezione ...
                            Call EttariAreCentiare_from_Ettari(SuperficieIntersezione, Ettari, Are, Centiare)

                            intDummy = ObjPartW.Scrivi(
                                            CStr(Piva),
                                            CInt(Sa_Cod),
                                            CInt(NuovoCampoCod),
                                            CStr(Prov),
                                            CStr(Com),
                                            CStr(Sezione),
                                            CInt(Foglio),
                                            CInt(Numero),
                                            CStr(Subalterno),
                                            CDbl(Ettari),
                                            CInt(Are),
                                            CInt(Centiare),
                                            CDbl(0),
                                            CInt(0),
                                            CInt(0),
                                            CDbl(0),
                                            CInt(0),
                                            CInt(0),
                                            CDbl(SuperficieIntersezione),
                                            Validita_Inizio,
                                            Validita_Fine,
                                            objParametri_Server)

                            'End If

                        End If

                    Next

                End If

            End If


            ''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


            ''Se ho inserito nuovi appezzamenti col Wizard SERRA
            'If DataGrid_NuoviAppezzamenti.Items.Count > 0 Then

            '    If Qs_Operazione = enum_TipoOperazioneDB.Scrittura Then


            '        Genera_AppezzamentiImpianti_da_Campo(DataGrid_NuoviAppezzamenti, _
            '                                             NuovoCampoCod, _
            '                                             MessaggioErrore _
            '                                             )



            '    Else


            '        Genera_AppezzamentiImpianti_da_Campo(DataGrid_NuoviAppezzamenti, _
            '                                             0, _
            '                                             MessaggioErrore _
            '                                             )


            '    End If


            '    If MessaggioErrore <> "" Then

            '        Messaggio = ""
            '        Messaggio += AgronicaAgenda_2010.SonoStatiRilevatiISeguentiErrori_ & vbCrLf
            '        Messaggio += "" & vbCrLf
            '        Messaggio += MessaggioErrore
            '        Messaggio += "" & vbCrLf
            '        Messaggio += AgronicaAgenda_2010.RitentareIlSalvataggioDopoLaCorrezione
            '        Throw New Exception(Messaggio)
            '        AgroMsgBox(Messaggio, Page)

            '        Exit Sub

            '    End If

            'End If



            '------------------------------------------------
            '----- Conferma di aggiornamento del database
            '------------------------------------------------

            'chiudo la transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            'chiudo la connessione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

            OK = True

            '------------------------------------------------


        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            Errore2 = True
            Dim StrDummy As String

            'Messaggio di errore
            StrDummy = exc.Message.ToString()

            'chiudo la transazione con il rollback
            If objParametri_Server.objConnessione IsNot Nothing Then
                If objParametri_Server.objTransazione IsNot Nothing Then
                    'chiudo transazione
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                End If
                'chiudo la connessione
                ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
            End If

            'FACCIO APPARIRE UN ALERT......
            AgroMsgBox(AgronicaAgenda_2010.SiÈVerificatoUnErroreDuranteLaFaseDiSalvat & vbCrLf & StrDummy, Page)


            '------------------------------------------------

        End Try

        '============================
        '===  Fine Aggiornamento  ===
        '============================
        If Not Errore2 Then
            'Dim Piva As String

            'Ritorno alla pagina AlberoImprese
            'Response.Redirect(Qs_PaginaRitorno & "?P=" & Piva)

            Dim Messaggio2 As String
            Messaggio2 = DirectCast(GetLocalResourceObject("CAMPOSalvatoConsuccesso"), String) & vbCrLf & vbCrLf

            Dim TargetUrl As String
            Select Case tipo_salva.Value

                Case 1
                    Messaggio2 += AgronicaAgenda_2010.VerràRicaricataLaPaginaPerInserimento & vbCrLf
                    'Messaggi.AgroMsgBox(Messaggio2, Page, "aspnetForm", UpdatePanel_script, , True)
                    AgroMsgBox(Messaggio2, Page)

                    Page_Load(Nothing, EventArgs.Empty)
                    clear_form()

                Case 2

                    'TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineAgenda_2010.Pagina_Anagrafica_Appezzamento, objParametriAgenda)
                    objParametriAgenda.Piva = xPiva
                    objParametriAgenda.Sa_Cod = xSa_Cod
                    objParametriAgenda.Campo_Cod = Campo_Cod
                    TargetUrl = "../Anagrafica/Appezzamento_Nuovo.aspx"
                    'Response.Redirect(TargetUrl)
                    If Qs_Visibilita <> 0 Then
                        TargetUrl &= "?visibilita=" & CStr(Qs_Visibilita)
                    End If
                    AgroMsgBox(Messaggio2, Page, , , "window.location = '" & TargetUrl & "';")

                Case Else
                    'Messaggi.AgroMsgBox(Messaggio2, Page, "aspnetForm", UpdatePanel_script, , True)

                    TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda)
                    'Response.Redirect(TargetUrl)
                    If Qs_Visibilita <> 0 Then
                        TargetUrl &= "?visibilita=" & CStr(Qs_Visibilita)
                    End If
                    AgroMsgBox(Messaggio2, Page, , , "window.location = '" & TargetUrl & "';")

            End Select

        End If

    End Sub

    Private Sub clear_form()

        TxtValiditaInizio.Text = ""
        TxtValiditaFine.Text = ""
        TxtDenominazione.Text = ""
        Cmb_OrientamentoColturale.ClearSelection()
        Cmb_SpecieVegetale.ClearSelection()
        Txt_Superficie.Text = ""
        Txt_SuperficieSAU.Text = ""
        Txt_SuperficiePercentuale.Text = ""
        Txt_SuperficieCoperta.Text = ""
        TxtDataVariazioneAppAggr.Text = ""


    End Sub


    '########################################################################################
    Private Sub Aggrega(ByVal Piva As String,
                        ByVal Sa_Cod As Integer,
                        ByVal Appezza As Integer,
                        ByVal Campo_Cod As Integer,
                        ByVal Validita_Inizio_Appezzamento As String,
                        ByVal Validita_Fine_Appezzamento As String,
                        ByVal DataVariazione As String)


        Dim Validita_Inizio As String
        Dim Validita_Fine As String

        Dim Validita_Inizio_Campo As String
        Dim Validita_Fine_Campo As String

        '----- Verifico che sia stata impostata una data corretta

        'If (DataVariazione = "") Then
        '    'Throw New Exception("E' necessario impostare una DATA CORRETTA !!!")
        '    'AgroMsgBox("E' necessario impostare una DATA CORRETTA !!!", Page)
        '    'Exit Sub
        'End If

        ''----- La data deve essere compresa nella validita' dell'appezzamento e del campo

        'If (CDate(DataVariazione) < CDate(Validita_Inizio_Appezzamento)) Or _
        '   (CDate(DataVariazione) > CDate(Validita_Fine_Appezzamento)) Then
        '    ' Throw New Exception("La DATA deve essere compresa negli estremi di validita' dell'appezzamento selezionato !!!")
        '    ' AgroMsgBox("La DATA deve essere compresa negli estremi di validita' " & _
        '    '            "dell'appezzamento selezionato !!!", Page)
        '    ' Exit Sub
        'End If

        If IsDate(Me.TxtValiditaInizio.Text) Then
            Validita_Inizio_Campo = Me.TxtValiditaInizio.Text
        Else
            Validita_Inizio_Campo = AGRODATAINIZIO
        End If
        If IsDate(Me.TxtValiditaFine.Text) Then
            Validita_Fine_Campo = Me.TxtValiditaFine.Text
        Else
            Validita_Fine_Campo = AGRODATAFINE
        End If

        'If (CDate(DataVariazione) < CDate(Validita_Inizio_Campo)) Or _
        '   (CDate(DataVariazione) > CDate(Validita_Fine_Campo)) Then
        '    'Throw New Exception("La DATA deve essere compresa negli estremi di validita' del campo selezionato !!!")
        '    'AgroMsgBox("La DATA deve essere compresa negli estremi di validita' " & _
        '    '           "del campo selezionato !!!", Page)
        '    'Exit Sub
        'End If


        '=======================
        '===  Aggiornamento  ===
        '=======================




        Try

            'NOTA
            '   modifica del record nella tabella appezzamento (inserimento campo_cod)
            '   inserimento di un record in Campo_Storico 

            'Dim objSQL As New Codex_Utility.Sql
            'Dim StrSQL As String
            'Dim Messaggio As String
            'Dim NumeroRecordInteressati As Integer


            '---------------------------------------------------------------------------------------
            '----- 1.  Modifica del record nella tabella appezzamento (inserimento campo_cod)
            '---------------------------------------------------------------------------------------

            Dim objCampiW As New AgronicaCoreAnagrafeDAL.Campi_W
            Dim Esito As Boolean

            Validita_Inizio = Validita_Inizio_Campo

            If CDate(Validita_Fine_Appezzamento) < CDate(Validita_Fine_Campo) Then
                Validita_Fine = Validita_Fine_Appezzamento
            Else
                Validita_Fine = Validita_Fine_Campo
            End If

            Esito = objCampiW.Aggrega(Piva, Sa_Cod, Campo_Cod, Appezza, Validita_Inizio, Validita_Fine, objParametri_Server)

        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            Dim StrDummy As String

            'Messaggio di errore
            StrDummy = exc.Message.ToString()

            'Faccio abortire la transazione
            System.EnterpriseServices.ContextUtil.SetAbort()

            'FACCIO APPARIRE UN ALERT......
            Throw New Exception(AgronicaAgenda_2010.SiÈVerificatoUnErroreDuranteLaFaseDiSalvat & vbCrLf & StrDummy)
            AgroMsgBox(AgronicaAgenda_2010.SiÈVerificatoUnErroreDuranteLaFaseDiSalvat & vbCrLf & StrDummy, Page)

            '------------------------------------------------

        End Try

        '============================
        '===  Fine Aggiornamento  ===
        '============================

    End Sub


    '########################################################################################
    Private Sub Disaggrega(ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Appezza As Integer,
                            ByVal Campo_Cod As Integer,
                            ByVal Validita_Inizio_Appezzamento As String,
                            ByVal Validita_Fine_Appezzamento As String,
                            ByVal DataVariazione As String)


        Dim Validita_Inizio As String
        Dim Validita_Fine As String

        Dim Validita_Inizio_Campo As String
        Dim Validita_Fine_Campo As String

        '----- Verifico che sia stata impostata una data corretta

        If (DataVariazione = "") Then
            Throw New Exception(DirectCast(GetLocalResourceObject("ImpostareUnaDataCorretta"), String))
            AgroMsgBox("E' necessario impostare una DATA CORRETTA !!!", Page)
            Exit Sub
        End If

        '----- La data deve essere compresa nella validita' dell'appezzamento

        If CDate(DataVariazione) <> AGRODATAINIZIO Then

            ' If (CDate(DataVariazione) <= CDate(Validita_Inizio_Appezzamento)) Or
            '(CDate(DataVariazione) >= CDate(Validita_Fine_Appezzamento)) Then
            '     Throw New Exception("La DATA deve essere compresa negli estremi di validita' dell'appezzamento selezionato !!!")
            '     AgroMsgBox("La DATA deve essere compresa negli estremi di validita' " &
            '                "dell'appezzamento selezionato !!!", Page)
            '     Exit Sub
            ' End If

        End If

        If IsDate(Me.TxtValiditaInizio.Text) Then
            Validita_Inizio_Campo = Me.TxtValiditaInizio.Text
        Else
            Validita_Inizio_Campo = AGRODATAINIZIO
        End If
        If IsDate(Me.TxtValiditaFine.Text) Then
            Validita_Fine_Campo = Me.TxtValiditaFine.Text
        Else
            Validita_Fine_Campo = AGRODATAFINE
        End If

        '=======================
        '===  Aggiornamento  ===
        '=======================

        Try

            'NOTA
            '   modifica del record nella tabella appezzamento (eliminazione campo_cod)
            '   inserimento di un record in Campo_Storico (se non esiste lo creo...)

            'Dim objSQL As New Codex_Utility.Sql
            'Dim StrSQL As String
            'Dim Messaggio As String
            'Dim NumeroRecordInteressati As Integer



            'validita inizio = Max (validita inizio campo, validita inizio appezzamento)
            'validita fine = data scelta x l'aggregazione

            If CDate(Validita_Inizio_Appezzamento) > CDate(Validita_Inizio_Campo) Then
                Validita_Inizio = Validita_Inizio_Appezzamento
            Else
                Validita_Inizio = Validita_Inizio_Campo
            End If

            Validita_Fine = DataVariazione


            Dim objCmapiW As New AgronicaCoreAnagrafeDAL.Campi_W
            Dim Esito As Boolean
            Esito = objCmapiW.Disaggrega(Piva, Sa_Cod, Campo_Cod, Appezza, Validita_Inizio, Validita_Fine, objParametri_Server)
            ''------------------------------------------------
            ''----- Conferma di aggiornamento del database
            ''------------------------------------------------

            'EseguitaOperazione = True

            ''------------------------------------------------


        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            Dim StrDummy As String

            'Messaggio di errore
            StrDummy = exc.Message.ToString()

            'Faccio abortire la transazione
            System.EnterpriseServices.ContextUtil.SetAbort()

            'FACCIO APPARIRE UN ALERT......
            Throw New Exception(AgronicaAgenda_2010.SiÈVerificatoUnErroreDuranteLaFaseDiSalvat & vbCrLf & StrDummy)
            AgroMsgBox(AgronicaAgenda_2010.SiÈVerificatoUnErroreDuranteLaFaseDiSalvat & vbCrLf & StrDummy, Page)

            '------------------------------------------------

        End Try

        '============================
        '===  Fine Aggiornamento  ===
        '============================

    End Sub




    '########################################################################################
    Private Sub CaricaGriglia_Particelle(ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Campo_Cod As Integer,
                                         ByVal DataValiditaInizio As Date,
                                         ByVal DataValiditaFine As Date,
                                         ByRef SuperficieTotale As Double)

        '----- Definizione delle variabili

        Dim Dt_Particelle As New DataTable
        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dim Superficie As Double
        Dim Sup_Condotta As Double

        Dim SuperficieDisponibile As Double

        Dim ParticellaPresente As Boolean

        Dim Prov As String
        Dim Com As String
        Dim Sezione As String
        Dim Foglio As Integer
        Dim Numero As Integer
        Dim Subalterno As String

        Dim i, j As Integer

        Dim DrParticella() As DataRow

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("chiave", GetType(String)))

        Dt.Columns.Add(New DataColumn("CodiceIstat_Provincia", GetType(String)))
        Dt.Columns.Add(New DataColumn("CodiceIstat_Comune", GetType(String)))
        Dt.Columns.Add(New DataColumn("Provincia", GetType(String)))
        Dt.Columns.Add(New DataColumn("Comune", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sezione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Foglio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Numero", GetType(String)))
        Dt.Columns.Add(New DataColumn("Subalterno", GetType(String)))
        Dt.Columns.Add(New DataColumn("Superficie", GetType(String)))
        Dt.Columns.Add(New DataColumn("SuperficieDisponibile", GetType(String)))
        Dt.Columns.Add(New DataColumn("SuperficieDisponibile2", GetType(String)))
        Dt.Columns.Add(New DataColumn("Progressivo", GetType(String)))
        Dt.Columns.Add(New DataColumn("Part_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("SuperficieImpiegata", GetType(String)))

        Dt.Columns.Add(New DataColumn("checked", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("SuperficieIntersezione", GetType(String)))
        Dt.Columns.Add(New DataColumn("SuperficieIntersezioneInput", GetType(String)))

        '----- Definisco l'insieme di colonne che costituiscono la chiave della tabella

        'Vettore di DataColumn
        Dim DtKeys(5) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = Dt.Columns("Provincia")
        DtKeys(1) = Dt.Columns("Comune")
        DtKeys(2) = Dt.Columns("Sezione")
        DtKeys(3) = Dt.Columns("Foglio")
        DtKeys(4) = Dt.Columns("Numero")
        DtKeys(5) = Dt.Columns("Subalterno")

        'Assegno il vettore delle chiavi al DataTable
        Dt.PrimaryKey = DtKeys

        '----- Recupero l'elenco delle particelle 
        Dim objParticelleCatastali As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
        Dt_Particelle = objParticelleCatastali.Recupera_Particelle_per_Centro(
                                             Piva,
                                            Sa_Cod,
                                            DataValiditaInizio,
                                            DataValiditaFine,
                                                objParametri_Server)




        If Dt_Particelle.Rows.Count > 0 Then

            Opt_Con_Catasto.Checked = True
            Opt_Senza_Catasto.Checked = False

        End If



        '----- Carico la griglia

        If Not IsNothing(Dt_Particelle) Then

            SuperficieTotale = 0

            For i = 0 To Dt_Particelle.Rows.Count - 1

                ParticellaPresente = False

                Prov = Dt_Particelle.Rows(i).Item("prov")
                Com = Dt_Particelle.Rows(i).Item("com")
                Sezione = Dt_Particelle.Rows(i).Item("sezione")
                Foglio = Dt_Particelle.Rows(i).Item("foglio")
                Numero = Dt_Particelle.Rows(i).Item("numero")
                Subalterno = Dt_Particelle.Rows(i).Item("subalterno")

                For j = 0 To Dt.Rows.Count - 1

                    If Prov = Dt.Rows(j).Item("CodiceIstat_Provincia") And
                       Com = Dt.Rows(j).Item("CodiceIstat_Comune") And
                       Sezione = Dt.Rows(j).Item("sezione") And
                       Foglio = Dt.Rows(j).Item("foglio") And
                       Numero = Dt.Rows(j).Item("numero") And
                       Subalterno = Dt.Rows(j).Item("subalterno") Then

                        ParticellaPresente = True

                        Exit For

                    End If

                Next

                If Not ParticellaPresente Then

                    DrParticella = Dt_Particelle.Select("prov='" & Prov &
                                                        "' AND com='" & Com &
                                                        "' AND sezione='" & Sezione &
                                                        "' AND foglio='" & Foglio.ToString &
                                                        "' AND numero='" & Numero.ToString &
                                                        "' AND subalterno='" & Subalterno & "'")

                    Sup_Condotta = 0

                    For j = 0 To DrParticella.Length - 1
                        If j = 0 Then
                            Sup_Condotta = CDbl(DrParticella(j).Item("sup_condotta"))
                        Else
                            If CDbl(DrParticella(j).Item("sup_condotta")) <> 0 Then
                                If Sup_Condotta > CDbl(DrParticella(j).Item("sup_condotta")) Then
                                    Sup_Condotta = CDbl(DrParticella(j).Item("sup_condotta"))
                                End If
                            End If
                        End If
                    Next

                    'Creo una nuova riga
                    Dr = Dt.NewRow

                    '---

                    'Definisco i valori

                    Dr.Item("Progressivo") = "0"
                    Dr.Item("Part_Cod") = Dt_Particelle.Rows(i).Item("part_cod")

                    Dr.Item("CodiceIstat_Provincia") = Dt_Particelle.Rows(i).Item("prov")
                    Dr.Item("CodiceIstat_Comune") = Dt_Particelle.Rows(i).Item("com")

                    Dr.Item("Provincia") = Dt_Particelle.Rows(i).Item("COMUNI_PROV")
                    Dr.Item("Comune") = Dt_Particelle.Rows(i).Item("LOCALITA")
                    Dr.Item("Sezione") = IIf(Dt_Particelle.Rows(i).Item("Sezione") = "0", "", Dt_Particelle.Rows(i).Item("Sezione"))
                    Dr.Item("Foglio") = Dt_Particelle.Rows(i).Item("Foglio")
                    Dr.Item("Numero") = Dt_Particelle.Rows(i).Item("Numero")
                    Dr.Item("Subalterno") = IIf(Dt_Particelle.Rows(i).Item("Subalterno") = "0", "", Dt_Particelle.Rows(i).Item("Subalterno"))

                    Superficie = Dt_Particelle.Rows(i).Item("Superficie")
                    Sup_Condotta = Dt_Particelle.Rows(i).Item("Sup_condotta")

                    Dr.Item("Superficie") = Format(Sup_Condotta, "0.0000")

                    SuperficieDisponibile = CDbl(Dt_Particelle.Rows(i).Item("SuperficieDisponibile"))

                    Dr.Item("SuperficieDisponibile") = Format(SuperficieDisponibile, "0.0000")
                    Dr.Item("SuperficieDisponibile2") = Format(SuperficieDisponibile, "0.0000")

                    Dr.Item("SuperficieImpiegata") = 0

                    Dr.Item("checked") = 0
                    Dr.Item("SuperficieIntersezione") = 0
                    Dr.Item("chiave") = Prov & "_" & Com & "_" & Sezione & "_" & Foglio & "_" & Numero & "_" & Subalterno

                    '---

                    'Associo alla tabella la nuova riga creata
                    Dt.Rows.Add(Dr)


                End If


            Next


        End If




        ''----- Associo il DataTable con il DataGrid

        'DataGridParticelle.DataSource = Dt
        'DataGridParticelle.DataBind()

        '//////////////////////////////////////////////////////////////////////////////////////////////
        ' MODIFICA
        '----- Per ciascuna particella ripristino i check e le sup di intersezione
        '//////////////////////////////////////////////////////////////////////////////////////////////

        Dim objCOM As New AgronicaCoreAnagrafeDAL.CampixParticelle_R         'New Agro_Anagrafe_AD.CampixParticelle_R
        Dim DTRs2 As DataTable

        '   *   CreateCANCELLATOObject("Agro_Anagrafe_AD.CampixParticelle_R")

        Dim SuperficieIntersezione As Double
        Dim SuperficieIntersezioneTotale As Double

        '----- Per ciascuna particella vedo quanta ne ho usata per il campo corrente

        SuperficieIntersezioneTotale = 0
        SuperficieIntersezione = 0

        'Se c'e' almeno una particella ...
        If Dt.Rows.Count > 0 Then

            If Campo_Cod <> 0 Then

                'Cerco le intersezioni eventuali
                DTRs2 = objCOM.Leggi(
                                    CStr(Piva),
                                    CInt(Sa_Cod),
                                    CInt(Campo_Cod),
                                    "", "", "", 0, 0, "",
                                       AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "",
                                     "",
                                     objParametri_Server)


                objCOM = Nothing

                If (Not IsNothing(DTRs2)) AndAlso
                   (DTRs2.Rows.Count > 0) Then

                    For j = 0 To DTRs2.Rows.Count - 1

                        For i = 0 To Dt.Rows.Count - 1

                            ''Recupero la chiave
                            'Prov = Me.DataGridParticelle.Items(i).Cells(0).Text
                            'Com = Me.DataGridParticelle.Items(i).Cells(1).Text
                            'Sezione = Me.DataGridParticelle.Items(i).Cells(5).Text
                            'Foglio = Me.DataGridParticelle.Items(i).Cells(6).Text
                            'Numero = Me.DataGridParticelle.Items(i).Cells(7).Text
                            'Subalterno = Me.DataGridParticelle.Items(i).Cells(8).Text

                            Prov = Dt.Rows(i).Item("CodiceIstat_Provincia")
                            Com = Dt.Rows(i).Item("CodiceIstat_Comune")
                            Sezione = Dt.Rows(i).Item("Sezione")
                            Foglio = Dt.Rows(i).Item("Foglio")
                            Numero = Dt.Rows(i).Item("Numero")
                            Subalterno = Dt.Rows(i).Item("Subalterno")

                            If Sezione = "&nbsp;" OrElse Sezione = "" Then
                                Sezione = "0"
                            End If

                            If Subalterno = "&nbsp;" OrElse Subalterno = "" Then
                                Subalterno = "0"
                            End If

                            If Prov = DTRs2.Rows(j).Item("prov") AndAlso
                               Com = DTRs2.Rows(j).Item("com") AndAlso
                               Sezione = DTRs2.Rows(j).Item("sezione") AndAlso
                               Foglio = DTRs2.Rows(j).Item("foglio") AndAlso
                               Numero = DTRs2.Rows(j).Item("numero") AndAlso
                               Subalterno = DTRs2.Rows(j).Item("Subalterno") Then

                                SuperficieIntersezione = DTRs2.Rows(j).Item("area")

                                Dt.Rows(i).Item("checked") = 1
                                Dt.Rows(i).Item("SuperficieIntersezione") = SuperficieIntersezione


                                'CType(Me.DataGridParticelle.Items(i).FindControl("ChkSelezionaParticella"), CheckBox).Checked = True

                                'CType(Me.DataGridParticelle.Items(i).FindControl("TxtIntersezione"), TextBox).Text = _
                                'Format(SuperficieIntersezione, "0.0000")

                                'essendo in modifica risommo la sup assegnata a questo campo
                                'DataGridParticelle.Items(i).Cells(10).Text() = Format(CDbl(DataGridParticelle.Items(i).Cells(10).Text()) + SuperficieIntersezione, "0.0000")

                                Exit For

                            End If

                        Next

                        SuperficieIntersezioneTotale += SuperficieIntersezione

                    Next
                Else
                    'Nessun record di intersezione
                    SuperficieIntersezione = 0
                End If

            Else
                'Se il Campo_Cod=0
                SuperficieIntersezione = 0
            End If

        End If


        '----- Imposto il totale
        Me.Txt_SuperficieCatastale.Text = Format(SuperficieIntersezioneTotale, "0.0000")
        Me.Txt_Superficie.Text = Format(SuperficieIntersezioneTotale, "0.0000")

        lblSupCatasto.Text = Format(SuperficieIntersezioneTotale, "0.0000")

        ' Salvo il datatable delle particelle in session
        If Not IsNothing(Dt_Particelle) Then
            HttpContext.Current.Session("Dt_Particelle") = Dt
        End If

        jsParticelle = DT_to_Json_Particelle(Dt)


    End Sub

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function ControlloDataInizio(ByVal data As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametriUtenti As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Try

            Dim dataInizio As Date = Date.Parse(data)

            Dim dt_appezza As DataTable = HttpContext.Current.Session("Appezzamenti_checked")

            Dim ok = True
            For Each row In dt_appezza.Rows
                If row("checked") = 1 Then

                    If row("Validita_Inizio_Appezzamento") < dataInizio Then
                        ok = False
                        Exit For
                    End If

                End If

            Next

            r.RispostaOK = True
            r.RispostaStringa = ok

        Catch ex As Exception
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function ControlloDataFine(ByVal data As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametriUtenti As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Try

            Dim dataFine As Date = Date.Parse(data)

            Dim dt_appezza As DataTable = HttpContext.Current.Session("Appezzamenti_checked")

            Dim ok = True
            For Each row In dt_appezza.Rows
                If row("checked") = 1 Then

                    If row("Validita_Fine_Appezzamento") > dataFine Then
                        ok = False
                        Exit For
                    End If

                End If

            Next

            r.RispostaOK = True
            r.RispostaStringa = ok

        Catch ex As Exception
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    Private Sub chk_serra_CheckedChanged(sender As Object, e As EventArgs) Handles chk_serra.CheckedChanged
        If chk_serra.Checked Then
            HttpContext.Current.Session("Serra") = "1"
        Else
            HttpContext.Current.Session("Serra") = "0"
        End If
    End Sub

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Imposta_Serra(ByVal tipo As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametriUtenti As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Try

            HttpContext.Current.Session("Serra") = tipo
            r.RispostaOK = True
            r.RispostaStringa = ""

        Catch ex As Exception
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Check_PianoConcimazione_EntitaxTestata(Campo_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Try
            Dim DataGridAppezzamenti_check As DataTable
            Dim DataGridAppezzamenti As DataRow()

            If Not IsNothing(HttpContext.Current.Session("Appezzamenti_checked")) Then
                DataGridAppezzamenti_check = HttpContext.Current.Session("Appezzamenti_checked")
            End If

            If Not IsNothing(HttpContext.Current.Session("Dt_Appezzamenti")) Then
                Dim DT As DataTable = HttpContext.Current.Session("Dt_Appezzamenti")
                DataGridAppezzamenti = DT.Select("checked = 1")
            End If

            Dim messaggio As String = ""

            Dim listaAppErrori_Aggiunti As New List(Of String)
            Dim listaAppErrori_Rimossi As New List(Of String)

            Dim objPianoConcimazione_EntitaxTestata_R As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_EntitaxTestata_R

            Dim dic_AppOld As New Dictionary(Of (String, Integer, Integer), String)
            Dim dic_Inseriti As New Dictionary(Of (String, Integer, Integer), String)
            Dim dic_AppRimossi As New Dictionary(Of (String, Integer, Integer), String)

            'Mi salvo in un Dic tutti gli appezzamenti collegati prima della modifica
            For Each app_old In DataGridAppezzamenti
                Dim old_piva As String = app_old.Item("piva")
                Dim old_sa_cod As Integer = app_old.Item("sa_cod")
                Dim old_appezza As Integer = app_old.Item("appezza")
                Dim old_app_nome As String = app_old.Item("app_nome")

                If Not dic_AppOld.ContainsKey((old_piva, old_sa_cod, old_appezza)) Then
                    dic_AppOld.Add((old_piva, old_sa_cod, old_appezza), old_app_nome)
                End If
            Next

            'Mi salvo tutti gli appezzamenti appena scelti
            For Each app In DataGridAppezzamenti_check.Rows
                Dim piva As String = app.Item("piva")
                Dim sa_cod As Integer = app.Item("sa_cod")
                Dim appezza As Integer = app.Item("appezza")
                Dim app_nome As String = app.Item("app_nome")

                If Not dic_Inseriti.ContainsKey((piva, sa_cod, appezza)) Then
                    dic_Inseriti.Add((piva, sa_cod, appezza), app_nome)
                End If
            Next

            'Se sono stati rimossi tutti i collegamenti, aggiungo tutti gli app vecchi alla lista degli app rimossi
            If dic_Inseriti.Count = 0 AndAlso dic_AppOld.Count > 0 Then
                dic_AppRimossi = dic_AppOld
            Else
                'Confronto i vecchi con i nuovi, e mi salvo eventuali elementi mancanti
                For Each app_old In dic_AppOld
                    For Each app In dic_Inseriti
                        If Not dic_Inseriti.ContainsKey((app_old.Key.Item1, app_old.Key.Item2, app_old.Key.Item3)) AndAlso
                            Not dic_AppRimossi.ContainsKey((app_old.Key.Item1, app_old.Key.Item2, app_old.Key.Item3)) Then
                            dic_AppRimossi.Add((app_old.Key.Item1, app_old.Key.Item2, app_old.Key.Item3), app_old.Value)
                        End If
                    Next
                Next
            End If


            'CICLO SUGLI APPEZZAMENTI COLLEGATI APPENA INSERITI PER VEDERE SE ESITONO DEI COLLEGAMENTI INCONGRUENTI CON I PIANI DI CONCIMAZIONE
            For Each app In dic_Inseriti
                Dim piva As String = app.Key.Item1
                Dim sa_cod As Integer = app.Key.Item2
                Dim appezza As Integer = app.Key.Item3
                Dim app_nome As String = app.Value

                'CERCO SE ESISTE UN PIANO CONCIMAZIONE ASSOCIATO
                Dim PianoConcimazione_EntitaxTestata = objPianoConcimazione_EntitaxTestata_R.Leggi_Default(0, 0, piva, sa_cod, 0, appezza,
                                                                                                             0, 0, "", "", "", 0,
                                                                                                             0, "", 0, "",
                                                                                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                             "", "",
                                                                                                             objParametri_Server)
                For Each piano In PianoConcimazione_EntitaxTestata.Rows
                    'SE SUL PIANO CONCIMAZIONE L'APP E' COLLEGATO AD UN CAMPO_COD DIVERSO/NON E' COLLEGATO AD UN CAMPO, DO ERRORE
                    If piano.item("campo_cod") <> Campo_Cod AndAlso Not (listaAppErrori_Aggiunti.Contains(app_nome)) Then
                        listaAppErrori_Aggiunti.Add(app_nome)
                    End If
                Next
            Next

            'CERCO SE ESSITONO PIANI DI CONCIMAZIONE ASSOCIATI AGLI APPEZZAMENTI RIMOSSI
            For Each app In dic_AppRimossi
                Dim piva As String = app.Key.Item1
                Dim sa_cod As Integer = app.Key.Item2
                Dim appezza As Integer = app.Key.Item3
                Dim app_nome As String = app.Value

                'SE NEL PIANO CONCIMAZIONE è SALVATO IL CAMPO_COD, IMPEDISCO LA MODIFICA
                Dim PianoConcimazione_EntitaxTestata = objPianoConcimazione_EntitaxTestata_R.Leggi_Default(0, 0, piva, sa_cod, 0, appezza,
                                                                                                             0, 0, "", "", "", 0,
                                                                                                             0, "", 0, "",
                                                                                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                             "", "",
                                                                                                             objParametri_Server)
                For Each piano In PianoConcimazione_EntitaxTestata.Rows
                    If piano.item("campo_cod") = Campo_Cod AndAlso Not (listaAppErrori_Rimossi.Contains(app_nome)) Then
                        listaAppErrori_Rimossi.Add(app_nome)
                    End If
                Next
            Next

            If listaAppErrori_Aggiunti.Count > 0 Then
                messaggio += (Gias.ImpossibileAggiungereSeguentiAppezzamentiCampoRisultanoAssociatiPianoConcimazione & ": " & String.Join("<br>- ", listaAppErrori_Aggiunti))
                messaggio += "<br>"
            End If

            If listaAppErrori_Rimossi.Count > 0 Then
                messaggio += (Gias.ImpossibileRimuovereSeguentiAppezzamentiCampoRisultanoAssociatiPianoConcimazione & ": " & String.Join("<br>- ", listaAppErrori_Rimossi))
            End If

            r.RispostaOK = True
            r.RispostaStringa = messaggio

        Catch ex As Exception
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function
End Class