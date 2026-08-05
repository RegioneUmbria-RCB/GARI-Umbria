Imports System.Web
Imports System.Web.Services

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreUtility.CaricaListControl

Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreUtentiDAL
Imports AgronicaConversioneCartografiaGIAS.Agronica
Imports System.Xml
Imports AgronicaCoreGestioneRichieste
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreContabDAL

Partial Class Centro_Edit
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda

    Public Operazione As Integer
    Dim xPiva As String
    Dim xSa_Cod As String
    Dim Qs_Piva As String
    'Dim Qs_PivaPadre As String
    Dim Qs_PivaNuova As String
    Dim Qs_Visibilita As Integer = 0

    Public srv_gm As String

    Public jsRubrica As String
    Public jsCodici As String

    Public permessi As PermessiUtente

    Public OperazioneSuRubrica As String

    Dim Rubrica_count As Integer


    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    '##########################################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim TargetUrl As String
        TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda, , Qs_Visibilita)
        Response.Redirect(TargetUrl)

    End Sub

    Private Sub Centro_Edit_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        ' VAnni: 2/10/2017: a seguito di AgroMaseterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        Master.flag_MostraBtnIndietro = True
    End Sub


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Set_Comune(ByVal comune As String, ByVal nome_comune As String)

        Dim Arrayp() As String
        Dim Com As String = ""

        HttpContext.Current.Session("comune_settato") = comune
        HttpContext.Current.Session("nome_comune_settato") = nome_comune

        If Trim(comune) <> "" Then
            Arrayp = Split(comune, "|")
            Com = Arrayp(1)
        End If
        HttpContext.Current.Session("com") = Com

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CalcolaXYdaLatLong_webservice(ByVal Latitudine As Double, ByVal Longitudine As Double) As Integer()

        Dim res(1) As Integer
        Dim X As Integer
        Dim Y As Integer

        Dim CoordWktSudEst As String = "POINT (" & Longitudine.ToString.Replace(",", ".") & " " & Latitudine.ToString.Replace(",", ".") & ")"
        Const LatLongToXY As Integer = 4
        ' Const XYtoLatLong As Integer = 1
        Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
        Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(LatLongToXY, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim cconverter As New CoordinateConverter
        Dim ParametriCartograficiWGS84ED50 As New ParametriCoordinateConverter With {
            .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
            .CStoText = dtLeggiTrasformazione(0)("CSTo"),
            .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
            .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
            .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
            .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
        }
        Dim risultato As String = cconverter.WKTPolygonWGS84_from_WKTPolygonED50(CoordWktSudEst, True, ParametriCartograficiWGS84ED50)
        'risultato = risultato.Replace("(", "").Replace(")", "")
        'X = CInt(risultato.Split(",")(0))
        'Y = CInt(risultato.Split(",")(1))
        X = CDbl(risultato.Split(" ")(1).Replace("(", "").Replace(")", "").Replace(".", ","))
        Y = CDbl(risultato.Split(" ")(2).Replace("(", "").Replace(")", "").Replace(".", ","))

        res(0) = X
        res(1) = Y

        HttpContext.Current.Session("coord_x") = X
        HttpContext.Current.Session("coord_y") = Y

        HttpContext.Current.Session("lat") = Latitudine
        HttpContext.Current.Session("lng") = Longitudine


        Return res

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiungi_Rubrica(ByVal tipo As String, ByVal valore As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim flag As Boolean = True

        Lingua.Gias_InizializzaCultura_DaSession()

        'If Not IsDate(DataInizio) Then
        '    DataInizio = AGRODATAINIZIO
        'Else
        '    DataInizio = CDate(DataInizio)
        'End If

        'If Not IsDate(DataFine) Then
        '    DataFine = AGRODATAFINE
        'Else
        '    DataFine = CDate(DataFine)
        'End If


        If (IsNothing(HttpContext.Current.Session("dt_Rubrica"))) Then
            Dt.Columns.Add(New DataColumn("Cod_Rubrica", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("numero", GetType(String)))
            Dt.Columns.Add(New DataColumn("descr", GetType(String)))
            Dt.Columns.Add(New DataColumn("tipo", GetType(Integer)))
        Else

            Dt = HttpContext.Current.Session("dt_Rubrica")
        End If

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then
            For Each row In Dt.Rows
                If tipo = row("descr") Then
                    flag = False
                    Exit For
                End If
            Next
        End If

        If (flag) Then

            'Creo una nuova riga
            Dr = Dt.NewRow

            'Definisco i valori
            'Dr.Item("Cod_Rubrica") = 
            Dr.Item("numero") = valore
            Dr.Item("descr") = tipo

            Select Case tipo
                Case "Telefono"
                    Dr.Item("tipo") = 1
                Case "Cellulare"
                    Dr.Item("tipo") = 2
                Case "Fax"
                    Dr.Item("tipo") = 3
                Case "Email"
                    Dr.Item("tipo") = 4
                Case "Social"
                    Dr.Item("tipo") = 5
                Case "Web"
                    Dr.Item("tipo") = 6
            End Select


            'Dt.Rows.Add(Dr)

            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)
            HttpContext.Current.Session("dt_Rubrica") = Dt
            Dim str_Risposta = DT_to_Json_Rubrica(Dt)
            r.RispostaOK = True
            r.RispostaStringa = str_Risposta
        Else
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.CodiceGiàInserito
        End If

        Return r

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaRubrica(ByVal tipo As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dt = HttpContext.Current.Session("dt_Rubrica")

        'Controllo se esiste già la voce che si vuole inserire
        If tipo <> "" Then
            For i = 0 To Dt.Rows.Count - 1
                If (Dt.Rows(i).Item("tipo") = tipo) Then
                    Dt.Rows.RemoveAt(i)
                    Exit For
                End If
            Next
        End If

        HttpContext.Current.Session("dt_Rubrica") = Dt
        Dim str_Risposta = DT_to_Json_Rubrica(Dt)
        r.RispostaOK = True
        r.RispostaStringa = str_Risposta
        Return r

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Rubrica(ByVal dt As DataTable) As String
        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        Lingua.Gias_InizializzaCultura_DaSession()

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Tipo", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaRubrica(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaRubrica(this);"))

        End If

        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)


        Dim cn As New ColonneNome("descr", AgronicaAgenda_2010.Descrizione, "string")
        l.Add(cn)

        cn = New ColonneNome("numero", AgronicaAgenda_2010.Valore, "string")
        l.Add(cn)

        cn = New ColonneNome("tipo", "Tipo", "string")
        cn._hidden = True
        l.Add(cn)


        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiungi_Codice_Centro(ByVal codice As String, ByVal valore As String, ByVal codice_id As String, ByVal DataInizio As String, ByVal DataFine As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Dt As New DataTable
        Dim Dr As DataRow
        'Dim Contatore As Integer
        Dim flag As Boolean = True

        Lingua.Gias_InizializzaCultura_DaSession()


        If Not IsDate(DataInizio) Then
            DataInizio = AGRODATAINIZIO
        Else
            DataInizio = CDate(DataInizio)
        End If

        If Not IsDate(DataFine) Then
            DataFine = AGRODATAFINE
        Else
            DataFine = CDate(DataFine)
        End If

        If (IsNothing(HttpContext.Current.Session("dt_Codici"))) Then
            Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
            Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
        Else
            Dt = HttpContext.Current.Session("dt_Codici")
        End If


        'Contatore = 1
        'Dim i As Integer
        Dim objCodiceAnagrafeR As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

        'For i = 0 To Dt.Rows.Count - 1
        '    Contatore = Contatore + 1
        'Next




        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1

            If (Dt.Rows(i).Item("Id_Cod") = codice_id) Then
                flag = False
            End If
        Next


        If (flag) Then

            'Creo una nuova riga
            Dr = Dt.NewRow

            'Definisco i valori

            'Dr.Item("Contatore") = Contatore


            Dr.Item("Id_Cod") = codice_id
            'Dr.Item("Descrizione") = objCodiceAnagrafeR.CodiceAnagrafeDes_from_CodiceAnagrafeCod(codice_id, HttpContext.Current.Session("ASG_objParametri_Server"))
            Dr.Item("Descrizione") = codice
            Dr.Item("Val_Cod") = valore

            Dr.Item("Validita_Inizio") = DataInizio

            Dr.Item("Validita_Fine") = DataFine

            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)
            HttpContext.Current.Session("dt_Codici") = Dt
            Dim str_Risposta = DT_to_Json_Codici(Dt)
            r.RispostaOK = True
            r.RispostaStringa = str_Risposta
        Else
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.CodiceGiàInserito
        End If

        Return r

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Rimuovi_Codice_Centro(Id_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dt = HttpContext.Current.Session("dt_Codici")

        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1
            If (Dt.Rows(i).Item("Id_Cod") = Id_Cod) Then
                Dt.Rows.RemoveAt(i)
                Exit For
            End If
        Next
        HttpContext.Current.Session("dt_Codici") = Dt
        Dim str_Risposta = DT_to_Json_Codici(Dt)
        r.RispostaOK = True
        r.RispostaStringa = str_Risposta
        Return r
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Codici(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        Lingua.Gias_InizializzaCultura_DaSession()

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)


        Dim cn As New ColonneNome("Id_Cod", "", "string")
        cn._FormatoParticolare = "<span></span>"
        cn._Filtrabile = False
        'cn._css = "prova"
        l.Add(cn)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Id_Cod", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaCodice(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaCodice(this);"))
        End If

        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)

        'Dim cn As New ColonneNome("Contatore", "Contatore", "string")
        'l.Add(cn)

        'Dim cn As New ColonneNome("Id_Cod", "Id_Cod", "string")
        'cn._hidden = True
        'l.Add(cn)

        cn = New ColonneNome("descrizione", AgronicaAgenda_2010.Codice, "string")
        cn._css = "wacol_descrizione"
        l.Add(cn)

        cn = New ColonneNome("Val_Cod", AgronicaAgenda_2010.Valore, "string")
        cn._css = "wacol_valore"
        l.Add(cn)

        cn = New ColonneNome("Validita_Inizio", AgronicaAgenda_2010.Dal, "string")
        cn._css = "wacol_dal"
        l.Add(cn)

        cn = New ColonneNome("Validita_Fine", AgronicaAgenda_2010.Al, "string")
        cn._css = "wacol_al"
        l.Add(cn)



        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Set_Comune(ByVal comune As String)

        Dim Arrayp() As String
        Dim Com As String = ""

        HttpContext.Current.Session("comune_settato") = comune


        If Trim(comune) <> "" Then
            Arrayp = Split(comune, "|")
            Com = Arrayp(1)
        End If
        HttpContext.Current.Session("com") = Com

    End Function



    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Province(ByVal stato As String) As Array

        Dim dll_Provincia As New DropDownList
        Dim rval(0) As String
        Dim options As String


        AgronicaCoreUtility.CaricaListControl.Provincie(dll_Provincia,
                                                        True, "", "",
                                                        False, 1, "", "", "", "", "", "", "", HttpContext.Current.Session("ASG_objParametri_Server"),
                                                        stato)


        For Each itm As ListItem In dll_Provincia.Items
            options &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        rval(0) = options

        Return rval

    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Comuni(ByVal provincia As String) As Array


        Dim cmb_comuni2 As New DropDownList
        Dim rval(1) As String
        Dim options As String

        If provincia <> "" Then

            AgronicaCoreUtility.CaricaListControl.Comuni(cmb_comuni2,
                                                             True, "", "",
                                                            provincia, False, 1, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))


            For Each itm As ListItem In cmb_comuni2.Items
                options &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
            Next

            rval(0) = options

            Dim objI As New AgronicaCoreMetaSchemaDAL.Istat_R
            'Imposto la textbox del CODICE ISTAT
            rval(1) = objI.CodIstat_from_Provincia(provincia, HttpContext.Current.Session("ASG_objParametri_Server"))


        End If

        Return rval

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Riempi_cblOTE() As String

        Dim DTCodici As DataTable
        Dim objCodici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
        Dim objParametriAgenda As New ParametriAgenda
        Dim Testo As String
        Dim Testo2 As String

        Dim risp As String


        ''Leggo le informazioni sull'impresa selezionata			
        'DTCodici = objCodici.Leggi(CStr(objParametriAgenda.Piva), _
        '                            CInt(objParametriAgenda.Sa_Cod), _
        '                            0, "", "", _
        '                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
        '                            "id_cod < 2000 OR id_cod >= 3000", _
        '                            "", _
        '                            HttpContext.Current.Session("ASG_objParametri_Server"))

        ''Elimino l'oggetto COM+
        'objCodici = Nothing


        ''Se il recordset non e' nullo
        'If DTCodici.Rows.Count > 0 Then

        '    Dim i As Integer
        '    For i = 0 To DTCodici.Rows.Count - 1

        '        '###########################################################
        '        '### Orientamento Tecnico Economico
        '        '###########################################################

        '        If DTCodici.Rows(i).Item("Id_Cod") = _
        '           enum_CodiciAnagrafe.OTE Then

        '            Testo = DTCodici.Rows(i).Item("Val_Cod")
        '            Testo2 = DTCodici.Rows(i).Item("Id_Cod")

        '            'Imposto la posizione nella combo delle provincie
        '            'Indice = CmbOTE.Items.IndexOf(CmbOTE.Items.FindByValue(Testo))
        '            'CmbOTE.SelectedIndex = Indice


        '            'splitto con la pipe
        '            Dim jj As Integer
        '            Dim ote_app As String
        '            Dim OTE_COD_App As String
        '            Dim OTE_CODICI_SPLIT() As String
        '            OTE_CODICI_SPLIT = Testo.Split("|")

        '            Dim ObjOTE As New AgronicaCoreMetaSchemaDAL.OrientamentoTecnicoEconomico_R
        '            Dim DtOTE As DataTable

        '            For jj = 0 To OTE_CODICI_SPLIT.Length - 1
        '                ote_app = OTE_CODICI_SPLIT(jj)
        '                If Trim(ote_app) <> "" Then

        '                    DtOTE = ObjOTE.Leggi(ote_app, _
        '                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
        '                                    "", "", _
        '                                    HttpContext.Current.Session("ASG_objParametri_Server"))

        '                    If DtOTE.Rows.Count <> 0 Then

        '                        'aggiungo l'elemento all'interno della lista
        '                        'ListOTE.Items.Add(New ListItem(DtOTE.Rows(0).Item("OTE_des"), DtOTE.Rows(0).Item("Ote_cod")))

        '                        risp = risp & "-" & DtOTE.Rows(0).Item("Ote_cod")

        '                    End If

        '                End If
        '            Next
        '        End If


        '    Next

        'End If

        '###########################################################
        '### Orientamento Tecnico Economico
        '###########################################################
        Dim i As Integer
        Dim arr_ote As New List(Of String)
        Dim trovato As Boolean

        'Leggo le informazioni sull'impresa selezionata			
        DTCodici = objCodici.Leggi(CStr(objParametriAgenda.Piva),
                                    CInt(objParametriAgenda.Sa_Cod),
                                    0, "", "",
                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "id_cod < 2000 OR id_cod >= 3000",
                                    "",
                                    HttpContext.Current.Session("ASG_objParametri_Server"))

        For i = 0 To DTCodici.Rows.Count - 1

            If DTCodici.Rows(i).Item("Id_Cod") =
               enum_CodiciAnagrafe.OTE Then

                Testo = DTCodici.Rows(i).Item("Val_Cod")
                Testo2 = DTCodici.Rows(i).Item("Id_Cod")

                'Imposto la posizione nella combo delle provincie
                'Indice = CmbOTE.Items.IndexOf(CmbOTE.Items.FindByValue(Testo))
                'CmbOTE.SelectedIndex = Indice


                'splitto con la pipe
                Dim jj As Integer
                Dim ote_app As String
                'Dim OTE_COD_App As String
                Dim OTE_CODICI_SPLIT() As String
                OTE_CODICI_SPLIT = Testo.Split("|")

                Dim ObjOTE As New AgronicaCoreMetaSchemaDAL.OrientamentoTecnicoEconomico_R
                Dim DtOTE As DataTable

                For jj = 0 To OTE_CODICI_SPLIT.Length - 1
                    ote_app = OTE_CODICI_SPLIT(jj)
                    If Trim(ote_app) <> "" Then

                        DtOTE = ObjOTE.Leggi(ote_app,
                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "", "",
                                        HttpContext.Current.Session("ASG_objParametri_Server"))

                        If DtOTE.Rows.Count <> 0 Then

                            'aggiungo l'elemento all'interno della lista
                            'ListOTE.Items.Add(New ListItem(DtOTE.Rows(0).Item("OTE_des"), DtOTE.Rows(0).Item("Ote_cod")))

                            arr_ote.Add(DtOTE.Rows(0).Item("Ote_cod"))

                        End If
                    End If
                Next
            End If

        Next

        Dim DTRs As DataTable

        'Dim objCOM As New AgronicaCoreAnagrafeDAL.OTE_Read
        Dim objCOM As New AgronicaCoreMetaSchemaDAL.OrientamentoTecnicoEconomico_R

        'Leggo le imprese associate al profilo selezionato	
        DTRs = objCOM.Leggi("",
                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "", "OTE_ordine ASC", HttpContext.Current.Session("ASG_objParametri_Server"))

        objCOM = Nothing

        '----- Riempio il controllo con i dati del datatable

        'Se il datatable non è vuoto allora ...	
        If DTRs IsNot Nothing AndAlso DTRs.Rows.Count > 0 Then
            ' Itero sui valori base OTE
            For i = 0 To DTRs.Rows.Count - 1
                trovato = False

                'Itero sui valori OTE selezionati
                For Each check In arr_ote
                    If check = DTRs.Rows(i).Item("OTE_COD") Then
                        trovato = True
                    End If
                Next

                If trovato Then
                    risp &= "<option selected='selected' value=""" & DTRs.Rows(i).Item("OTE_COD") & """>" & DTRs.Rows(i).Item("OTE_DES") & "</option>"
                Else
                    risp &= "<option value=""" & DTRs.Rows(i).Item("OTE_COD") & """>" & DTRs.Rows(i).Item("OTE_DES") & "</option>"
                End If
            Next
        End If


        Return risp

    End Function



    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_cblOTE(ByVal valori As String)

        ' salvo i valori OTE in session (x salvataggio)
        valori = valori.Replace("-"c, "|"c)
        HttpContext.Current.Session("OTE") = valori



    End Function


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        permessi = New PermessiUtente()

        OperazioneSuRubrica = ""
        Master.flag_pag_Anagrafica = True

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---

        If Request.QueryString("visibilita") IsNot Nothing AndAlso IsNumeric(Request.QueryString("visibilita")) Then
            Qs_Visibilita = CInt(Request.QueryString("visibilita"))
        End If

        'Se Qs_Visibilita = 0 è stato aperto dal Menu Anagrafe generale e quindi utilizzo la versione standard della grafica
        If Qs_Visibilita = 0 Then
            Master.Master_versione = VERSIONE_MASTER_DEFAULT
            Master.Header_versione = VERSIONE_HEADER_DEFAULT
        End If

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

        If UtenteAbilitato_Modifica = False Then
            Response.Redirect("../MenuAnagrafica/Menubs_anagrafica.aspx")
            Exit Sub
        End If

        'Recupero da webconfig la connessione alternativa da usare
        Dim AgroWebC As New AgroWebConfig()

        srv_gm = "https://maps.googleapis.com/maps/api/js?key="

        If AgroWebC.GoogleMaps <> "" Then
            srv_gm = AgroWebC.GoogleMaps
            srv_gm = srv_gm.Replace("&sensor=false&libraries=drawing,geometry", "")
        End If


        objParametriAgenda = New ParametriAgenda

        Select Case objParametriAgenda.Tipo_Operazione
            Case enum_TipoOperazioneDB.Scrittura
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("CreazioneNuovoCentroAziendale"), String)
                objParametriAgenda.Sa_Cod = 0
            Case enum_TipoOperazioneDB.Lettura
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("LetturaCentroAziendale"), String)
            Case enum_TipoOperazioneDB.Modifica
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("ModificaCentroAziendale"), String)
        End Select
        Operazione = objParametriAgenda.Tipo_Operazione

        HttpContext.Current.Session("operazione") = Operazione


        xPiva = objParametriAgenda.Piva
        xSa_Cod = objParametriAgenda.Sa_Cod

        '#########################################################################################################
        '------------------- controllo per gestione visibilità della ddl Centro Esterno Collegato ----------------
        '------------------------------ visibilesolo se Zoo o F&F sono attivi ------------------------------------
        CentroEsterno_Div.Visible = False
        Dim moduli = ""
        Dim elencoModuli = ""
        Dim leggiAnagrafeLog As New OGenerazioni_Anagrafe_Moduli_Log_R
        Dim dtAnagrafeLog = leggiAnagrafeLog.Leggi(xPiva, 0, 0, 0, 0, -1, -1, -1, AGRODATAINIZIO, AGRODATAFINE,
                                                   "", "",
                                                   objParametri_Server)

        If dtAnagrafeLog IsNot Nothing AndAlso dtAnagrafeLog.Rows.Count > 0 Then
            For Each rAnagrafeLog In dtAnagrafeLog.Rows
                If rAnagrafeLog.Item("Modulo_Generazione") = 2 OrElse rAnagrafeLog.Item("Modulo_Generazione") = 5 Then
                    CentroEsterno_Div.Visible = True
                End If
            Next
        End If
        moduli = elencoModuli
        dtAnagrafeLog = Nothing
        '#########################################################################################################



        If Not IsPostBack Then

            HttpContext.Current.Session("OTE") = Nothing

        Else
            Exit Sub
        End If

        RipristinaDatiNeiControlli()


        If Operazione = enum_TipoOperazioneDB.Lettura Then

            Cmb_CentroEsterno.Enabled = False
            TxtDenominazione.Enabled = False
            Cmb_Tipologia.Enabled = False
            Cmb_TitoloPossesso.Enabled = False
            Txt_Via.Enabled = False
            Txt_Frazione.Enabled = False
            Cmb_Provincia.Enabled = False
            Cmb_Comune.Enabled = False
            Txt_CAP.Enabled = False
            'Txt_Stato.Enabled = False
            cmb_Stato.Enabled = False
            Txt_Note.Enabled = False
            TxtRubrica_Numero.Enabled = False
            Cmb_Rubrica_Descrizione.Enabled = False
            btn_geolocalizza.Visible = False

            TxtValiditaInizio.Enabled = False
            TxtValiditaFine.Enabled = False
            CmbCodice.Enabled = False
            TxtCodiceValore.Enabled = False
            TxtValiditaInizioCodice.Enabled = False
            TxtValiditaFineCodice.Enabled = False


            TxtCodiceImpresa.Enabled = False
            chkLast.Enabled = False
            CmbTipoAttivita.Enabled = False
            CmbOrganismoControllo.Enabled = False
            cblOTE.Disabled = True

            TxtSup_Totale.Enabled = False
            TxtSup_Tare.Enabled = False
            TxtSup_SAU.Enabled = False
            txtSup_SAU_Convenzionale.Enabled = False
            txtSup_SAU_Conversione.Enabled = False
            txtSup_SAU_Biologico.Enabled = False
            TxtSup_Bosco.Enabled = False
            TxtSup_Prati.Enabled = False

        End If

    End Sub

    ' Ripristina combo stato per codice e in seconda battuta per descrizione (per retocompatibiltà)
    Private Sub Ripristina_Cmb_Stato(ByVal stato As String)
        Dim index = cmb_Stato.Items.IndexOf(cmb_Stato.Items.FindByValue(stato))
        If index = -1 Then
            stato = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stato.ToLower())
            index = cmb_Stato.Items.IndexOf(cmb_Stato.Items.FindByText(stato))
        End If
        cmb_Stato.SelectedIndex = If(index = -1, cmb_Stato.Items.IndexOf(cmb_Stato.Items.FindByValue("IT")), index)
    End Sub

    Private Sub RipristinaDatiNeiControlli()


        TxtDenominazione.Text = ""
        LblRiferimenti.Text = xPiva & " " & xSa_Cod


        Dim objImpresaR As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Dim Errore As String
        Dim StrCodiciAzienda As String

        Dim objCodici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read  'Agro_Anagrafe_AD.Centri_Codici_Read
        Dim DTCodici As DataTable

        Dim Testo As String
        Dim Testo2 As String

        Dim TipoOperazioneDB As enum_TipoOperazioneDB
        Dim Id_Cod As Integer
        Dim Val_Cod As String
        Dim StrCodice As String
        Dim StrCodici As String
        Dim BaseCode As Integer
        Dim TopCode As Integer
        Dim Stato As String = "IT"


        Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

        'StrCodiciAzienda = objCodiceAnagrafe.Filtro_Codici_Anagrafe(1, 3, 2, objParametri_Server)

        ''Elimino codice CUAA, Titolo Possesso e tecnico perchè già presenti nella form
        'StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1010", "")
        'StrCodiciAzienda = Replace(StrCodiciAzienda, " Codice = 1010", "")
        'StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1016", "")
        'StrCodiciAzienda = Replace(StrCodiciAzienda, "Codice = 1016 Or ", "")
        'StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1088", "")
        'StrCodiciAzienda = Replace(StrCodiciAzienda, " Codice = 1088", "")

        StrCodiciAzienda = " UPPER(gruppo) = 'CENTRO' OR UPPER(creatore) = 'CRPA' "

        AgronicaCoreUtility.CaricaListControl.Codici(CType(Me.CmbCodice, ListControl),
                                                     True, "", "",
                                                     0, "",
                                                     StrCodiciAzienda, "", objParametri_Server)

        AgronicaCoreUtility.CaricaListControl.TipoAttivita(CType(Me.CmbTipoAttivita, ListControl),
                                                           True, " ", " ",
                                                           "", "", objParametri_Server)

        'AgronicaCoreUtility.CaricaListControl.OTE(CType(Me.CmbOTE, ListControl), _
        '                                           True, " ", " ", _
        '                                           "", "", objParametri_Server)

        AgronicaCoreUtility.CaricaListControl.BIO_Organismi_Controllo(Me.CmbOrganismoControllo,
                                                                  True, "", "0",
                                                                  0, "", "", 2, 0, "", "", objParametri_Server)

        AgronicaCoreUtility.CaricaListControl.TitoloPossesso(Me.Cmb_TitoloPossesso, False, "", "", "", "", objParametri_Server)

        AgronicaCoreUtility.CaricaListControl.CentroEsterno(Me.Cmb_CentroEsterno, True, "Nessun Centro Aziendale Esterno Collegato", "", xPiva, False, objParametri_Server)

        TxtSup_Totale.Text = 0
        TxtSup_Tare.Text = 0
        TxtSup_SAU.Text = 0
        txtSup_SAU_Biologico.Text = 0
        txtSup_SAU_Conversione.Text = 0
        txtSup_SAU_Convenzionale.Text = 0
        TxtSup_Bosco.Text = 0
        TxtSup_Prati.Text = 0
        Txt_CodIndirizzo.Text = 0

        TxtSup_Totale.Enabled = False
        TxtSup_Tare.Enabled = False
        TxtSup_SAU.Enabled = False
        txtSup_SAU_Biologico.Enabled = False
        txtSup_SAU_Conversione.Enabled = False
        txtSup_SAU_Convenzionale.Enabled = False


        ' Riempio la tendina delle nazioni
        Dim DT_Nazioni As DataTable
        Dim objNazioni As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
        DT_Nazioni = objNazioni.Leggi("", "", "Descrizione", objParametri_Server)

        For i = 0 To DT_Nazioni.Rows.Count - 1
            cmb_Stato.Items.Add(New ListItem(DT_Nazioni.Rows(i).Item("Descrizione"), DT_Nazioni.Rows(i).Item("Codice")))
            cmb_Stato.Items(i).Attributes.Add("Gestione_Gerarchia_Geografica", DT_Nazioni.Rows(i).Item("Gestione_Gerarchia_Geografica"))
        Next

        ' setto Italia come default
        cmb_Stato.SelectedIndex = cmb_Stato.Items.IndexOf(cmb_Stato.Items.FindByValue("IT"))



        If Operazione = enum_TipoOperazioneDB.Scrittura Then
            'carico tutti i controlli a null
            TxtDenominazione.Text = ""
            'Cmb_Tipologia.SelectedValue = 101
            TxtPiva.Text = xPiva
            TxtSaCod.Text = xSa_Cod

            'NOTA
            'Se sono in fase di inserimento, 
            'suggerisco la provincia e il comune,
            'prendendoli dall'indirizzo del centro aziendale

            Dim CodiceIstat_Provincia As String = ""
            Dim CodiceIstat_Comune As String = ""
            Dim Provincia_Sigla As String = ""
            'Dim objCentrixIndirizziR As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
            'Call objCentrixIndirizziR.Recupera_Indirizzo_ElementiGerarchia( _
            '                                enum_GerarchiaImpresa_Elementi.Gerarchia_Centro, _
            '                                Provincia_Sigla, _
            '                                CodiceIstat_Provincia, _
            '                                CodiceIstat_Comune, _
            '                                Errore, _
            '                                xPiva, _
            '                                xSa_Cod, _
            '                                0, 0, objParametri_Server)

            Province_and_Comuni(Cmb_Provincia,
                                Cmb_Comune,
                                Provincia_Sigla,
                                CodiceIstat_Comune,
                                False,
                                1, objParametri_Server)

            Txt_ProCodIstat.Text = CodiceIstat_Provincia
            Txt_ProvinciaSigla.Text = Provincia_Sigla
            Txt_ComCodIstat.Text = CodiceIstat_Comune

            HttpContext.Current.Session("comune_settato") = Cmb_Comune.SelectedValue

            AgronicaCoreUtility.CaricaListControl.TipoCentro(CType(Cmb_Tipologia, ListControl),
                                                                     True, AgronicaAgenda_2010.Seleziona.ToUpper(), "",
                                                                     "", "", objParametri_Server)

            Dim DTRubrica As New DataTable

            DTRubrica.Columns.Add(New DataColumn("Cod_Rubrica", GetType(String)))
            DTRubrica.Columns.Add(New DataColumn("descr", GetType(String)))
            DTRubrica.Columns.Add(New DataColumn("numero", GetType(String)))
            DTRubrica.Columns.Add(New DataColumn("tipo", GetType(Integer)))


            HttpContext.Current.Session("dt_Rubrica") = DTRubrica
            jsRubrica = DT_to_Json_Rubrica(DTRubrica)
            'initRubrica.Value = jsRubrica

            If (IsNothing(HttpContext.Current.Session("dt_Codici"))) Then
                CaricaGriglia_Codici(True)
            End If



        ElseIf Operazione = enum_TipoOperazioneDB.Modifica OrElse Operazione = enum_TipoOperazioneDB.Lettura Then
            'se sono qui inizializzo i controlli con quello che ho da db
            'carico denominazione



            If IsPostBack Then
                Exit Sub
            End If

            '@Paolo: Cerco l'azienda di riferimento
            Dim DTAzienda As DataTable
            DTAzienda = objImpresaR.Leggi(CStr(xPiva),
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                        objParametri_Server)

            '@Paolo: leggo le date di riferimento
            If DTAzienda.Rows.Count > 0 Then
                lbl_azienda_data_inizio.Text = DTAzienda.Rows(0).Item("Validita_Inizio")
                lbl_azienda_data_fine.Text = DTAzienda.Rows(0).Item("Validita_Fine")
            End If


            Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            Dim DTCentri As DataTable
            DTCentri = objCentriAz.Leggi(CStr(xPiva),
                                        CInt(xSa_Cod),
                                         AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                         "",
                                        "",
                                        objParametri_Server)

            If DTCentri.Rows.Count > 0 Then
                TxtDenominazione.Text = DTCentri.Rows(0).Item("sa_nome").ToString

                Txt_Via.Text = DTCentri.Rows(0).Item("ind_des").ToString
                Txt_Frazione.Text = DTCentri.Rows(0).Item("frz_des").ToString
                Txt_CAP.Text = DTCentri.Rows(0).Item("CAP")

                'Txt_Stato.Text = DTCentri.Rows(0).Item("stato")
                Txt_Note.Text = DTCentri.Rows(0).Item("note")

                Dim dtTitoloPossesso = objCodici.Leggi(CStr(xPiva), CInt(xSa_Cod), 1016, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                If dtTitoloPossesso.Rows.Count > 0 Then
                    Cmb_TitoloPossesso.SelectedValue = dtTitoloPossesso.Rows(0)("val_cod")
                End If


                If Trim(DTCentri.Rows(0).Item("Stato")) <> "" Then
                    Stato = UCase(Trim(DTCentri.Rows(0).Item("Stato")))
                End If

                Ripristina_Cmb_Stato(Stato)

                Txt_Latitude.Text = DTCentri.Rows(0).Item("lat")
                Txt_Latitude.ReadOnly = True

                Txt_Longitude.Text = DTCentri.Rows(0).Item("long")
                Txt_Longitude.ReadOnly = True

                AgronicaCoreUtility.CaricaListControl.TipoCentro(CType(Cmb_Tipologia, ListControl),
                                                         True, AgronicaAgenda_2010.Seleziona.ToUpper(), "",
                                                         "", "", objParametri_Server)


                If DTCentri.Rows(0).Item("Validita_Inizio") = "01/01/1900" Then
                    TxtValiditaInizio.Text = ""
                Else
                    TxtValiditaInizio.Text = DTCentri.Rows(0).Item("Validita_Inizio")
                End If

                If DTCentri.Rows(0).Item("Validita_Fine") = "31/12/2100" Then
                    TxtValiditaFine.Text = ""
                Else
                    TxtValiditaFine.Text = DTCentri.Rows(0).Item("Validita_Fine")
                End If

                ATPrevalenteText.Value = DTCentri.Rows(0).Item("AT_Prevalente")
                ' Leggo la Rubrica

                Dim objRubrica As New AgronicaCoreAnagrafeDAL.CentrixRubrica_Read
                Dim DTRubrica As DataTable


                'Leggo le informazioni sull'impresa selezionata
                DTRubrica = objRubrica.Leggi(CStr(xPiva),
                                             CInt(xSa_Cod),
                                             0,
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                "",
                                                "",
                                                objParametri_Server)


                Rubrica_count = DTRubrica.Rows.Count
                DTRubrica.Columns.Add(New DataColumn("tipo", GetType(Integer)))

                For Each row In DTRubrica.Rows

                    Select Case row("descr")
                        Case "Telefono"
                            row("tipo") = 1
                        Case "Cellulare"
                            row("tipo") = 2
                        Case "Fax"
                            row("tipo") = 3
                        Case "Email"
                            row("tipo") = 4
                        Case "Social"
                            row("tipo") = 5
                        Case "Web"
                            row("tipo") = 6
                    End Select

                Next

                HttpContext.Current.Session("dt_Rubrica") = DTRubrica


                jsRubrica = DT_to_Json_Rubrica(DTRubrica)
                'initRubrica.Value = jsRubrica


                'GridView_Rubrica.DataSource = DTRubrica
                'GridView_Rubrica.DataBind()

                '----- Salvo il DataTable dentro il viewstate
                Session("vs_dtRubrica") = DTRubrica


                ''''''''

                CaricaGriglia_Codici(False, xPiva, xSa_Cod)



                ''''''''

                'Leggo le informazioni sull'impresa selezionata			
                DTCodici = objCodici.Leggi(CStr(xPiva),
                                            CInt(xSa_Cod),
                                            0, "", "",
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "id_cod < 2000 OR id_cod >= 3000",
                                            "",
                                            objParametri_Server)

                'Elimino l'oggetto COM+
                objCodici = Nothing


                'Se il recordset non e' nullo
                If DTCodici.Rows.Count > 0 Then



                    Dim i As Integer
                    For i = 0 To DTCodici.Rows.Count - 1


                        '###########################################################
                        '### CODICE OPERATORE   ---   Codice Impresa     
                        '###########################################################

                        If DTCodici.Rows(i).Item("Id_Cod") = enum_CodiciAnagrafe.CodiceCentro_Attuale Then

                            Me.TxtCodiceImpresa.Text = DTCodici.Rows(i).Item("Val_Cod")

                        End If



                        '###########################################################
                        '### Conformita' / Certificazioni / Regolamenti
                        '###########################################################

                        If (Not IsDBNull(DTCodici.Rows(i).Item("creatore"))) AndAlso
                           (Not IsDBNull(DTCodici.Rows(i).Item("gruppo"))) Then

                            If DTCodici.Rows(i).Item("creatore") = "AGRONICA" AndAlso
                               DTCodici.Rows(i).Item("gruppo") = "CONFORMITA" Then

                                Testo = DTCodici.Rows(i).Item("Descrizione")
                                'ListCertificazioni.Items.Add(New ListItem(Testo, DTCodici.Rows(i).Item("codice")))

                            End If

                        End If


                        '###########################################################
                        '### Tipo di Centro Aziendale
                        '###########################################################

                        If (Not IsDBNull(DTCodici.Rows(i).Item("creatore"))) AndAlso
                           (Not IsDBNull(DTCodici.Rows(i).Item("gruppo"))) Then

                            If DTCodici.Rows(i).Item("creatore") = "CSA" AndAlso
                               DTCodici.Rows(i).Item("gruppo") = "TIPO_CA" Then

                                '-----------------------------------
                                ' Text = Descrizione
                                ' Value = Codice Anagrafe
                                '-----------------------------------

                                'Imposto la posizione nella combo 
                                Cmb_Tipologia.SelectedIndex =
                                    Cmb_Tipologia.Items.IndexOf(
                                        Cmb_Tipologia.Items.FindByValue(
                                            DTCodici.Rows(i).Item("id_cod")))

                            End If

                        End If

                        '###########################################################
                        '### Tipo di Attivita'
                        '###########################################################

                        If DTCodici.Rows(i).Item("Id_Cod") =
                           enum_CodiciAnagrafe.TipoAttivita Then

                            Testo = DTCodici.Rows(i).Item("Val_Cod")
                            Testo2 = DTCodici.Rows(i).Item("Id_Cod")
                            'controllo se è stato inserito un valore usando il textbox
                            'If Testo.StartsWith("@") Then 'rendo visibile il text box
                            '    txtAltro.Style.Item("visibility") = "visible"
                            '    txtAltro.Text = Testo.Substring(1)
                            '    Testo = "@"
                            'End If
                            'Imposto la posizione nella combo 
                            CmbTipoAttivita.SelectedIndex =
                                CmbTipoAttivita.Items.IndexOf(
                                    CmbTipoAttivita.Items.FindByValue(
                                        Testo))

                        End If

                        '###########################################################
                        '### Associazione con Organismi di Controllo
                        '###########################################################
                        '-------------------------------------------------------------
                        'modificato il 09 04 2013
                        'usare il codice anagrafe  ORGANISMO_DI_CONTROLLO_BIO = 1263 
                        'e un solo organismo letto da tabell BIO_Dati_OrganismiControllo
                        If DTCodici.Rows(i).Item("Id_Cod") = enum_CodiciAnagrafe.ORGANISMO_DI_CONTROLLO_BIO Then
                            Dim valcod As String = DTCodici.Rows(i).Item("Val_Cod")
                            CmbOrganismoControllo.SelectedIndex =
                                CmbOrganismoControllo.Items.IndexOf(
                                    CmbOrganismoControllo.Items.FindByValue(
                                        valcod))

                        End If

                        '###########################################################
                        '### Centro Aziendale Esterno Associato
                        '###########################################################
                        '-------------------------------------------------------------
                        'modificato il 09 04 2013
                        'usare il codice anagrafe  ORGANISMO_DI_CONTROLLO_BIO = 1263 
                        'e un solo organismo letto da tabell BIO_Dati_OrganismiControllo
                        If DTCodici.Rows(i).Item("Id_Cod") = enum_CodiciAnagrafe.Centro_Aziendale_Esterno_Collegato Then
                            Dim valcod As String = DTCodici.Rows(i).Item("Val_Cod")
                            Cmb_CentroEsterno.SelectedIndex =
                                Cmb_CentroEsterno.Items.IndexOf(
                                    Cmb_CentroEsterno.Items.FindByValue(
                                        valcod))

                        End If

                        '###########################################################
                        '### Orientamento Tecnico Economico
                        '###########################################################

                        If DTCodici.Rows(i).Item("Id_Cod") =
                           enum_CodiciAnagrafe.OTE Then

                            Testo = DTCodici.Rows(i).Item("Val_Cod")
                            Testo2 = DTCodici.Rows(i).Item("Id_Cod")

                            'Imposto la posizione nella combo delle provincie
                            'Indice = CmbOTE.Items.IndexOf(CmbOTE.Items.FindByValue(Testo))
                            'CmbOTE.SelectedIndex = Indice


                            'splitto con la pipe
                            Dim jj As Integer
                            Dim ote_app As String
                            Dim OTE_CODICI_SPLIT() As String
                            OTE_CODICI_SPLIT = Testo.Split("|")

                            Dim ObjOTE As New AgronicaCoreMetaSchemaDAL.OrientamentoTecnicoEconomico_R
                            Dim DtOTE As DataTable

                            For jj = 0 To OTE_CODICI_SPLIT.Length - 1
                                ote_app = OTE_CODICI_SPLIT(jj)
                                If Trim(ote_app) <> "" Then

                                    DtOTE = ObjOTE.Leggi(ote_app,
                                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    "", "",
                                                    objParametri_Server)

                                    If DtOTE.Rows.Count <> 0 Then

                                        'aggiungo l'elemento all'interno della lista
                                        'ListOTE.Items.Add(New ListItem(DtOTE.Rows(0).Item("OTE_des"), DtOTE.Rows(0).Item("Ote_cod")))

                                    End If
                                End If
                            Next
                        End If



                    Next

                End If



                ' Compilo SUPERFICI
                Dim Sup_Totale As Double = 0
                Dim Sup_Bosco As Double = 0
                Dim Sup_Prati As Double = 0
                Dim Sup_Tare As Double = 0
                Dim SAU_Totale As Double = 0
                Dim SAU_Biologico As Double = 0
                Dim SAU_Conversione As Double = 0
                Dim SAU_Convenzionale As Double = 0

                Dim objCentriAzR As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                Call objCentriAzR.Recupera_Superfici_CentroAziendale(
                                            DTCentri.Rows(0).Item("Piva"),
                                            DTCentri.Rows(0).Item("Sa_Cod"),
                                            Sup_Totale,
                                            Sup_Bosco,
                                            Sup_Prati,
                                            Sup_Tare,
                                            SAU_Totale,
                                            SAU_Biologico,
                                            SAU_Conversione,
                                            SAU_Convenzionale,
                                            Date.Today,
                                             objParametri_Server)


                TxtSup_Totale.Text = Format(Sup_Totale, "0.####")
                TxtSup_Tare.Text = Format(Sup_Tare, "0.####")
                TxtSup_SAU.Text = Format(SAU_Totale, "0.####")
                Me.txtSup_SAU_Biologico.Text = Format(SAU_Biologico, "0.####")
                Me.txtSup_SAU_Conversione.Text = Format(SAU_Conversione, "0.####")
                Me.txtSup_SAU_Convenzionale.Text = Format(SAU_Convenzionale, "0.####")

                TxtSup_Bosco.Text = DTCentri.Rows(0).Item("Sup_Bosco")
                TxtSup_Prati.Text = DTCentri.Rows(0).Item("Sup_Prati")






            End If

            'Leggo le informazioni sull'impresa selezionata			
            Dim objIndirizzi As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
            Dim DTIndirizzi As DataTable
            DTIndirizzi = objIndirizzi.Leggi(CStr(xPiva),
                                                CInt(xSa_Cod),
                                                0, 0,
                                                 AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                                  "",
                                                "",
                                                objParametri_Server)

            'Elimino l'oggetto COM+
            objIndirizzi = Nothing

            'Se il recordset non e' nullo
            If DTIndirizzi.Rows.Count > 0 Then
                Dim i As Integer

                For i = 0 To DTIndirizzi.Rows.Count - 1

                    If (DTIndirizzi.Rows(i).Item("tipo_indirizzo") = 1) Then

                        Txt_CodIndirizzo.Text = DTIndirizzi.Rows(i).Item("cod_indirizzo")

                        Txt_Via.Text = DTIndirizzi.Rows(i).Item("ind_des")
                        Txt_Frazione.Text = DTIndirizzi.Rows(i).Item("frz_des")
                        Txt_CAP.Text = DTIndirizzi.Rows(i).Item("cap")
                        'Txt_Stato.Text = DTIndirizzi.Rows(i).Item("stato")
                        Txt_Note.Text = DTIndirizzi.Rows(i).Item("note")

                        Ripristina_Cmb_Stato(DTIndirizzi.Rows(i).Item("stato"))

                        Me.Txt_ProvinciaSigla.Text = CStr(DTIndirizzi.Rows(i).Item("pro_cod"))

                        Me.Txt_ProCodIstat.Text = CStr(DTIndirizzi.Rows(i).Item("pro_cod_istat"))
                        Me.Txt_ComCodIstat.Text = CStr(DTIndirizzi.Rows(i).Item("com_cod_istat"))

                        'Me.Txt_Provincia.Text = Provincia_from_CodIstat(Server, Session, Page, CStr(RsIndirizzi.Fields("pro_cod_istat").Value), Nothing)

                    End If

                Next



            End If






            tipo_salva.Value = 0

            'LblRiferimenti.Text &= " " & objParametriAgenda.Particelle(0).Part_Cod

            'Imposto le combo di provincia e comune
            Dim objIstatR As New AgronicaCoreMetaSchemaDAL.Istat_R
            Dim xSiglaProvincia As String = ""
            Dim DescProv As String = ""

            Dim CodiceIstat_Provincia As String
            Dim CodiceIstat_Comune As String
            Dim Provincia_Sigla As String

            Dim objCentrixIndirizziR As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
            Call objCentrixIndirizziR.Recupera_Indirizzo_ElementiGerarchia(
                                            enum_GerarchiaImpresa_Elementi.Gerarchia_Centro,
                                            Provincia_Sigla,
                                            CodiceIstat_Provincia,
                                            CodiceIstat_Comune,
                                            Errore,
                                            xPiva,
                                            xSa_Cod,
                                            0, 0, objParametri_Server)

            If Not String.IsNullOrEmpty(CodiceIstat_Provincia) Then
                DescProv = objIstatR.Provincia_from_CodIstat(CodiceIstat_Provincia, xSiglaProvincia, objParametri_Server)
            End If

            Province_and_Comuni(Cmb_Provincia,
                                Cmb_Comune,
                                xSiglaProvincia,
                                CodiceIstat_Comune,
                                False,
                                1, objParametri_Server, Stato)

            Txt_ProCodIstat.Text = CodiceIstat_Provincia
            Txt_ProvinciaSigla.Text = Provincia_Sigla
            Txt_ComCodIstat.Text = CodiceIstat_Comune

            If Cmb_Comune.Items.Count > 0 Then
                HttpContext.Current.Session("comune_settato") = Cmb_Comune.SelectedValue
            End If


            '-------------------------------------------------------------------
            '----- Leggo l'elenco dei CODICI del CENTRO AZIENDALE
            '-------------------------------------------------------------------

            'Controllo se e' checked la checkbox chkLast
            'In caso positivo genero un nuovo codice e lo inserisco
            'altrimenti considero il codice presente nella textbox
            'P.S. Ora deve essere presente un solo codice e non hanno
            'piu' importanza le date ...




            Dim intUdc As Integer = Me.UltimoIdCodiceOperatore()


            'Recupero le informazioni
            '-----------------------------------
            ' Value
            ' 01/01/1900.31/12/2100.01234567
            '-----------------------------------


            If Me.chkLast.Checked = True Then

                TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
                Id_Cod = enum_CodiciAnagrafe.CodiceCentro_Attuale
                Val_Cod = UltimoIdCodiceOperatore().ToString

                'Genero l'XML del singolo nodo
                Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                StrCodice,
                                TipoOperazioneDB,
                                Id_Cod,
                                Val_Cod,
                               AGRODATAINIZIO,
                               AGRODATAFINE,
                                BaseCode,
                                TopCode)

                'Inserisco l'XML nella stringa complessiva
                StrCodici &= StrCodice

            Else


                If Me.TxtCodiceImpresa.Text <> "" Then

                    TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
                    Id_Cod = enum_CodiciAnagrafe.CodiceCentro_Attuale
                    Val_Cod = Me.TxtCodiceImpresa.Text

                    'Genero l'XML del singolo nodo
                    Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                    StrCodice,
                                    TipoOperazioneDB,
                                    Id_Cod,
                                    Val_Cod,
                               AGRODATAINIZIO,
                               AGRODATAFINE,
                                    BaseCode,
                                    TopCode)

                    'Inserisco l'XML nella stringa complessiva
                    StrCodici &= StrCodice

                End If

            End If

            '------------------------
            '------------------------



        End If

    End Sub

    '########################################################################################
    Private Function UltimoIdCodiceOperatore() As Integer
        'funzione che mi ritorna l'ultimo id del codice anagrafe, sfrutta il componente COM

        Dim BaseCode As Integer
        Dim TopCode As Integer

        Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))
        'dichiaro l'oggetto com+ che mi server
        Dim objCodiceOperatore As New Agro_Sequenze

        'Leggo le informazioni sul centro selezionato
        Return objCodiceOperatore.NuovoId_Tabella(CStr("CodiceOperatore"),
                                                    BaseCode,
                                                    TopCode,
                                                    objParametri_Server)
    End Function


    '########################################################################################
    Private Sub CaricaGriglia_Codici(ByVal Inizializza As Boolean,
                                       Optional ByVal Piva As String = "",
                                       Optional ByVal SaCod As Integer = 0)

        '----- Definizione delle variabili

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim Contatore As Integer

        Dim StrXmlCodiciAttuali As String = ""
        Dim StrXmlCodice As String = ""

        Dim BaseCode As Integer
        Dim TopCode As Integer

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Contatore", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))


        '----- Recupero l'elenco delle particelle

        'Dim objImpreseCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim objCentriCodici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
        Dim DTCodici As DataTable

        If Inizializza = True Then
            DTCodici = Nothing
        Else

            DTCodici = objCentriCodici.Leggi(CStr(Piva), SaCod, 0, "", "",
                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            " ((id_cod < 2000 AND id_cod <> 1107) OR (id_cod >= 3000 AND id_cod <> 1107)) AND ID_COD NOT IN (101, 102, 103, 1000, 1003, 93, 1009, 1263, 1016, 1337) ",
                                            "",
                                            objParametri_Server)
        End If



        HttpContext.Current.Session("dt_Codici") = DTCodici



        If (Not IsNothing(DTCodici)) AndAlso (DTCodici.Rows.Count > 0) Then

            Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))

            Contatore = 1
            Dim i As Integer
            Dim objCodiceAnagrafeR As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

            For i = 0 To DTCodici.Rows.Count - 1

                Select Case DTCodici.Rows(i).Item("Id_Cod")

                    Case enum_CodiciAnagrafe.CodiceCUAA, enum_CodiciAnagrafe.Tecnico

                    Case Else

                        'Creo una nuova riga
                        Dr = Dt.NewRow

                        'Definisco i valori

                        Dr.Item("Contatore") = Contatore
                        Contatore += 1

                        Dr.Item("Id_Cod") = DTCodici.Rows(i).Item("id_cod")
                        Dr.Item("Descrizione") = objCodiceAnagrafeR.CodiceAnagrafeDes_from_CodiceAnagrafeCod(CInt(DTCodici.Rows(i).Item("id_cod")), objParametri_Server)
                        Dr.Item("Val_Cod") = DTCodici.Rows(i).Item("val_cod")

                        If (CDate(DTCodici.Rows(i).Item("Validita_Inizio"))) = AGRODATAINIZIO Then
                            Dr.Item("Validita_Inizio") = "..."
                        Else
                            Dr.Item("Validita_Inizio") = CDate(DTCodici.Rows(i).Item("Validita_Inizio")).ToShortDateString
                        End If

                        If (CDate(DTCodici.Rows(i).Item("Validita_Fine"))) = AGRODATAFINE Then
                            Dr.Item("Validita_Fine") = "..."
                        Else
                            Dr.Item("Validita_Fine") = CDate(DTCodici.Rows(i).Item("Validita_Fine")).ToShortDateString
                        End If

                        Dt.Rows.Add(Dr)

                End Select

                'Genero l'XML del singolo nodo
                Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                StrXmlCodice,
                                enum_TipoOperazioneDB.Cancellazione,
                                DTCodici.Rows(i).Item("Id_Cod"),
                                DTCodici.Rows(i).Item("Val_cod"),
                                DTCodici.Rows(i).Item("xValidita_Inizio"),
                                DTCodici.Rows(i).Item("xValidita_Fine"),
                                BaseCode,
                                TopCode)

                StrXmlCodiciAttuali &= StrXmlCodice

            Next

        End If


        '----- Salvo la StrXmlCodiciAttuali 

        Session("StrXmlCodiciAttuali") = StrXmlCodiciAttuali


        '----- Associo il DataTable con il DataGrid
        Aggiorna_Griglia_Codici(Dt)


    End Sub

    '#####################################################################################################################################################
    Private Sub Aggiorna_Griglia_Codici(ByVal Dt As DataTable)

        ' Chiavi per recuperare le righe
        Dim DtKeys(4) As String
        DtKeys(0) = "Contatore"
        DtKeys(1) = "Id_Cod"
        DtKeys(2) = "Val_Cod"
        DtKeys(3) = "Validita_Inizio"
        DtKeys(4) = "Validita_Fine"


        jsCodici = DT_to_Json_Codici(Dt)
        'initCodici.Value = jsCodici
        '----- Associo il DataTable con la DataGrid
        'GridView_Codici.DataSource = Dt
        'GridView_Codici.DataKeyNames = DtKeys
        'GridView_Codici.DataBind()

        '----- Salvo il DataTable dentro il viewstate
        ViewState("vs_dtCodici") = Dt

    End Sub

    Private Sub CalcolaLatLongDaXY(ByRef X As Integer, ByRef Y As Integer, ByRef Txt_Latitudine As Double, ByRef Txt_Longitudine As Double)
        Dim CoordWktSudEst As String = "(" & X & "," & Y & ")"
        ' Const LatLongToXY As Integer = 4
        Const XYtoLatLong As Integer = 1
        Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
        Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(XYtoLatLong, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        Dim cconverter As New CoordinateConverter
        Dim ParametriCartograficiWGS84ED50 As New ParametriCoordinateConverter With {
            .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
            .CStoText = dtLeggiTrasformazione(0)("CSTo"),
            .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
            .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
            .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
            .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
        }
        Dim risultato As String = cconverter.WKTPolygonWGS84_from_WKTPolygonED50(CoordWktSudEst, True, ParametriCartograficiWGS84ED50)
        'risultato = risultato.Replace("(", "").Replace(")", "")
        Txt_Longitudine = CDbl(risultato.Split(" ")(1).Replace("(", "").Replace(")", "").Replace(".", ","))
        Txt_Latitudine = CDbl(risultato.Split(" ")(2).Replace("(", "").Replace(")", "").Replace(".", ","))
    End Sub

    Private Sub CalcolaXYdaLatLong(ByRef X As Integer, ByRef Y As Integer, ByRef Latitudine As Double, ByRef Longitudine As Double)
        Dim CoordWktSudEst As String = "(" & Longitudine.ToString.Replace(",", ".") & "," & Latitudine.ToString.Replace(",", ".") & ")"
        Const LatLongToXY As Integer = 4
        ' Const XYtoLatLong As Integer = 1
        Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
        Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(LatLongToXY, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        Dim cconverter As New CoordinateConverter
        Dim ParametriCartograficiWGS84ED50 As New ParametriCoordinateConverter With {
            .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
            .CStoText = dtLeggiTrasformazione(0)("CSTo"),
            .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
            .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
            .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
            .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
        }
        Dim risultato As String = cconverter.WKTPolygonWGS84_from_WKTPolygonED50(CoordWktSudEst, True, ParametriCartograficiWGS84ED50)
        'risultato = risultato.Replace("(", "").Replace(")", "")
        'X = CInt(risultato.Split(",")(0))
        'Y = CInt(risultato.Split(",")(1))
        X = CDbl(risultato.Split(" ")(1).Replace("(", "").Replace(")", "").Replace(".", ","))
        Y = CDbl(risultato.Split(" ")(2).Replace("(", "").Replace(")", "").Replace(".", ","))
    End Sub


    '########################################################################################
    Private Sub ImgBtn_SalvaTutto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SalvaTutto.Click

        Salva_Tutto()

    End Sub


    '########################################################################################
    Private Sub Salva_Tutto()

        Dim StrIndirizzo As String
        Dim StrCodice As String
        Dim StrCodici As String
        Dim StrCentroAziendale As String
        Dim StrXmlInserisci As String
        Dim StrXmlCancella As String
        Dim StrRubricaSingola As String
        Dim StrRubricaGruppo As String
        Dim StrCodiceSingolo As String
        Dim StrCodiceGruppo As String

        Dim Operazione As enum_TipoOperazioneDB
        Dim Indice As Integer
        Dim i As Integer
        'Dim Testo As String
        'Dim Carattere As String

        Dim BaseCode As Integer
        Dim TopCode As Integer

        Dim TipoOperazioneDB As enum_TipoOperazioneDB
        Dim Tipo_Indirizzo As Integer
        Dim Cod_Indirizzo As Integer
        Dim Ind_Des As String
        Dim Frz_Des As String
        Dim CAP As String
        Dim Com_Des As String
        Dim Pro_Cod As String
        Dim Stato As String
        Dim Note As String
        Dim Pro_Cod_Istat As String
        Dim Com_Cod_Istat As String
        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date
        Dim CoordinataX As Integer
        Dim CoordinataY As Integer

        'Dim Cod_Rubrica As Integer
        Dim RubricaCodice As Integer
        Dim RubricaNumero As String
        Dim RubricaDescrizione As String

        Dim CodiceID_Cod As Integer
        Dim CodiceVal_Cod As String

        Dim Id_Cod As Integer
        Dim Val_Cod As String

        Dim Piva As String
        Dim Sa_Cod As Integer
        Dim Sa_Nome As String
        'Dim X As Double
        'Dim Y As Double
        Dim Zslm As Double
        Dim Longitudine As Double
        Dim Latitudine As Double
        Dim Area As Double
        Dim CA_Sipi As String
        Dim AT_Prevalente As String
        Dim Forma_Possesso As String

        Dim TipoCentroCodice As Integer
        Dim TipoCentroDescrizione As String
        Dim TitoloPossesso As Integer

        Dim Sup_Totale As Double
        Dim Sup_Bosco As Double
        Dim Sup_Tare As Double
        Dim Sup_SAU As Double
        Dim Sup_Prati As Double
        Dim Sup_SAU_Convenzionale As Double
        Dim Sup_SAU_Conversione As Double
        Dim Sup_SAU_Biologico As Double


        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlDoc2 As New System.Xml.XmlDocument
        Dim XmlDatiCentriAziendali As System.Xml.XmlElement
        Dim XmlCentroAziendale As System.Xml.XmlElement
        Dim XmlInd As XmlElement

        Dim Errore As Boolean = False

        Dim MessaggioErrore As String

        Dim NoteLog As String = NOTELOG_ANAGRAFE_BOOTSTRAP

        Dim objCentroAziendale As New AgronicaCoreAnagrafeBIZ.CentroAziendale_W
        Dim StrDummy As String


        '------------------------------------------------
        '----- Verifico l'operazione richiesta (Inserimento / Modifica / Cancellazione)
        '------------------------------------------------

        'Recupero l'operazione richiesta, dalla querystring
        Operazione = objParametriAgenda.Tipo_Operazione 'CInt(Qs_Operazione)



        '------------------------------------------------
        '----- Verifico la correttezza delle informazioni inserite
        '------------------------------------------------

        MessaggioErrore = ""


        '--- Denominazione del Centro Aziendale

        If TxtDenominazione.Text = "" Then
            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LaDenominazioneDelCentroNonPuòEssereNulla"), String) & vbCrLf
        End If


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
            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LaFineDelCentroNonPuòPrecedereInizio"), String) & vbCrLf
        End If

        'If Validita_Inizio < CDate(InizioImpresa.Value) Then
        '    MessaggioErrore += "   - L'inizio dell'attivita' non puo' precedere la creazione dell'Impresa. (" & InizioImpresa.Value & ")" & vbCrLf
        'End If

        'If Validita_Fine > CDate(FineImpresa.Value) Then
        '    MessaggioErrore += "   - La fine dell'attivita' non puo' seguire la cessazione dell'Impresa. (" & FineImpresa.Value & ")" & vbCrLf
        'End If


        '--- Tipo di Centro

        If Cmb_Tipologia.SelectedValue = "" Then
            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IndicareIlTipoDiCentro"), String) & vbCrLf
        End If

        '--- Indirizzo

        If Txt_Via.Text = "" Then
            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IndirizzoNecessitaVIA"), String) & vbCrLf
        End If


        'Lettura Gestione_Gerarchia_Geografica
        Dim DT_Nazioni As DataTable
        Dim objNazioni As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
        DT_Nazioni = objNazioni.Leggi(cmb_Stato.SelectedItem.Value, "", "Descrizione", objParametri_Server)

        If CInt(DT_Nazioni(0).Item("Gestione_Gerarchia_Geografica")) = 1 Then


            If Txt_ProvinciaSigla.Text = "" Then
                MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IndirizzoNecessitaPROVINCIA"), String) & vbCrLf
            End If

            If Txt_ComCodIstat.Text = "" Then
                MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IndirizzoNecessitaCOMUNE"), String) & vbCrLf
            End If

            If Me.Txt_CAP.Text = "" Then
                MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IndirizzoNecessitaCAP"), String) & vbCrLf
            Else
                If Not IsNumeric(Me.Txt_CAP.Text) Then
                    MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IlCAPDeveEssereCostituitoDaCinqueCifre"), String) & vbCrLf
                Else
                    If Microsoft.VisualBasic.Strings.Len(Me.Txt_CAP.Text) <> 5 Then
                        MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IlCAPDeveEssereCostituitoDaCinqueCifre"), String) & vbCrLf
                    End If
                End If
            End If

        End If

        'If Me.Txt_Stato.Text = "" Then
        '    MessaggioErrore += "   - " & "Nell'indirizzo e' necessario indicare lo STATO." & vbCrLf
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

            AgroMsgBox(Messaggio, Page)

            Exit Sub

        End If


        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------

        Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))


        '------------------------------------------------
        '----- Costruisco la stringa XML del INDIRIZZO
        '------------------------------------------------

        'Definisco il tipo di operazione da eseguire
        TipoOperazioneDB = Operazione

        'Prelevo le informazioni immediate
        Tipo_Indirizzo = 1
        Cod_Indirizzo = Txt_CodIndirizzo.Text
        Ind_Des = Txt_Via.Text
        Frz_Des = Txt_Frazione.Text
        CAP = Txt_CAP.Text
        Com_Des = Cmb_Comune.SelectedValue                    'Cmb_Comune.SelectedItem.Text
        'Com_Des = Me.Txt_Comune.Text                    'Cmb_Comune.SelectedItem.Text
        Pro_Cod = Txt_ProvinciaSigla.Text            'Cmb_Provincia.SelectedItem.Value

        If CInt(DT_Nazioni(0).Item("Gestione_Gerarchia_Geografica")) = 1 Then

            Frz_Des = Txt_Frazione.Text
            CAP = Txt_CAP.Text
            'Com_Des = ddl_comune.SelectedItem.Text

            Pro_Cod = Me.Txt_ProvinciaSigla.Text
            Pro_Cod_Istat = Me.Txt_ProCodIstat.Text


            If HttpContext.Current.Session("com") = Nothing Then
                Set_Comune(Cmb_Comune.SelectedItem.Value, Cmb_Comune.SelectedItem.Text)
            End If

            Dim com_codice As String
            com_codice = HttpContext.Current.Session("com")
            Com_Des = HttpContext.Current.Session("nome_comune_settato")
            Com_Cod_Istat = com_codice



        Else
            Frz_Des = Txt_Frazione.Text
            CAP = Txt_CAP.Text
            'Com_Des = ddl_comune.SelectedItem.Text
            Com_Des = ""
            Pro_Cod = "00"
            Pro_Cod_Istat = "000"
            Com_Cod_Istat = "000"
        End If

        'Stato = Txt_Stato.Text
        Stato = cmb_Stato.SelectedItem.Value


        Note = Txt_Note.Text


        'Genero la stringa XML
        Call XML_Indirizzo(enum_CodificaDecodifica.Codifica,
                           StrIndirizzo,
                           TipoOperazioneDB,
                           Tipo_Indirizzo,
                           Cod_Indirizzo,
                           Ind_Des,
                           Frz_Des,
                           CAP,
                           Com_Des,
                           Pro_Cod,
                           Stato,
                           Note,
                           Pro_Cod_Istat,
                           Com_Cod_Istat,
                           AGRODATAINIZIO,
                           AGRODATAFINE,
                           BaseCode,
                           TopCode)


        ''----------------------------------------------------------
        ''----- ELIMINO I NUMERI IN RUBRICA SE SONO IM MODIFICA
        ''----------------------------------------------------------


        StrRubricaGruppo = ""
        StrCodiceGruppo = ""

        Dim objCentriRubrica As New AgronicaCoreAnagrafeDAL.CentrixRubrica_Read
        Dim objCodiciCentri As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read

        'Dim strSql As String

        If Operazione = enum_TipoOperazioneDB.Modifica Then

            ''compongo la query di cancellazione..
            'strSql = ""
            'strSql += " SELECT CentrixRubrica.PIVA, CentrixRubrica.sa_cod, CentrixRubrica.cod_rubrica, Rubrica.numero, Rubrica.descr"
            'strSql += " FROM CentrixRubrica INNER JOIN"
            'strSql += " Rubrica ON CentrixRubrica.cod_rubrica = Rubrica.cod_rubrica"
            'strSql += " WHERE CentrixRubrica.PIVA = '" & Qs_Piva & "' "
            'strSql += " AND CentrixRubrica.sa_cod = " & Me.TxtSaCod.Text

            ''eseguo la query...
            ''Recupero il recordset
            'Rs = objSQL.SqlSelect(objParametri_Server.StringaConnessione, _
            '                     strSql, _
            '                    0, _
            '                    Messaggio)

            'Elimino l'oggetto
            Dim Rs As DataTable = objCentriRubrica.Leggi(xPiva, xSa_Cod, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            For i = 0 To Rs.Rows.Count - 1

                Call XML_Rubrica(enum_CodificaDecodifica.Codifica,
                                 StrRubricaSingola,
                                 enum_TipoOperazioneDB.Cancellazione,
                                 CInt(Rs.Rows(i).Item("cod_rubrica")),
                                 CStr(Rs.Rows(i).Item("numero")),
                                 CStr(Rs.Rows(i).Item("descr")),
                       AGRODATAINIZIO,
                       AGRODATAFINE,
                                 BaseCode,
                                 TopCode)

                'Inserimento nella stringa complessiva
                StrRubricaGruppo &= StrRubricaSingola


            Next


            Dim Cs As DataTable = objCodiciCentri.Leggi(xPiva, xSa_Cod, 0, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            For i = 0 To Cs.Rows.Count - 1
                If CInt(Cs.Rows(i).Item("ID_Cod")) <> 101 AndAlso CInt(Cs.Rows(i).Item("ID_Cod")) <> 102 AndAlso CInt(Cs.Rows(i).Item("ID_Cod")) <> 103 AndAlso CInt(Cs.Rows(i).Item("ID_Cod")) < 2000 Then

                    Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                StrCodiceSingolo,
                                enum_TipoOperazioneDB.Cancellazione,
                                CInt(Cs.Rows(i).Item("ID_Cod")),
                                CStr(Cs.Rows(i).Item("Val_cod")),
                                AGRODATAINIZIO,
                                AGRODATAFINE,
                                BaseCode,
                                TopCode)

                    'Inserimento nella stringa complessiva
                    StrCodici &= StrCodiceSingolo

                End If


            Next


        End If


        If Me.chkLast.Checked = True Then

            TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
            Id_Cod = enum_CodiciAnagrafe.CodiceCentro_Attuale
            Val_Cod = UltimoIdCodiceOperatore().ToString

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            TipoOperazioneDB,
                            Id_Cod,
                            Val_Cod,
                           AGRODATAINIZIO,
                           AGRODATAFINE,
                            BaseCode,
                            TopCode)

            'Inserisco l'XML nella stringa complessiva
            StrCodici &= StrCodice

        Else


            If Me.TxtCodiceImpresa.Text <> "" Then

                TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
                Id_Cod = enum_CodiciAnagrafe.CodiceCentro_Attuale
                Val_Cod = Me.TxtCodiceImpresa.Text

                'Genero l'XML del singolo nodo
                Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                StrCodice,
                                TipoOperazioneDB,
                                Id_Cod,
                                Val_Cod,
                           AGRODATAINIZIO,
                           AGRODATAFINE,
                                BaseCode,
                                TopCode)

                'Inserisco l'XML nella stringa complessiva
                StrCodici &= StrCodice

            End If

        End If





        '----- TIPO di ATTIVITA'

        'Recupero le informazioni

        'Se non ho selezionato nulla allora non creo il codice
        If CmbTipoAttivita.SelectedIndex >= 1 Then
            '-----------------------------------
            ' Text = Descrizione
            ' Value = "P", "T" , "X"
            '-----------------------------------
            TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
            Id_Cod = enum_CodiciAnagrafe.TipoAttivita
            'controllo che non sia stato selezionato l'opzione "Altro", in questo caso
            'devo aggiungere al valore il testo del controllo me.txtAltro
            Val_Cod = CmbTipoAttivita.SelectedItem.Value
            'If Val_Cod.StartsWith("@") Then 'il valore che sta dietro alla voce Altro è @
            '    'aggiungo il testo del controllo 
            '    Val_Cod &= Me.txtAltro.Text.Trim
            'End If


            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            TipoOperazioneDB,
                            Id_Cod,
                            Val_Cod,
                           AGRODATAINIZIO,
                           AGRODATAFINE,
                            BaseCode,
                            TopCode)

            'Inserisco l'XML nella stringa complessiva
            StrCodici &= StrCodice

        End If


        If Not IsNothing(CmbOrganismoControllo) AndAlso
           CmbOrganismoControllo.SelectedItem.Value <> "0" AndAlso
           CmbOrganismoControllo.SelectedItem.Value <> "" AndAlso
           IsNumeric(CmbOrganismoControllo.SelectedItem.Value) Then
            Id_Cod = enum_CodiciAnagrafe.ORGANISMO_DI_CONTROLLO_BIO
            Val_Cod = CStr(CmbOrganismoControllo.SelectedItem.Value)
            TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            TipoOperazioneDB,
                            Id_Cod,
                            Val_Cod,
                           AGRODATAINIZIO,
                           AGRODATAFINE,
                            BaseCode,
                            TopCode)

            'Inserisco l'XML nella stringa complessiva
            StrCodici &= StrCodice
        End If


        Dim OTE_values As String

        OTE_values = HttpContext.Current.Session("OTE")

        If (OTE_values IsNot Nothing) AndAlso (OTE_values.Length > 0) Then
            '-----------------------------------
            ' Text = Descrizione
            ' Value = codice OTE
            '-----------------------------------
            TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
            Id_Cod = enum_CodiciAnagrafe.OTE
            'Val_Cod = CStr(CmbOTE.SelectedItem.Value)
            Val_Cod = OTE_values


            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            TipoOperazioneDB,
                            Id_Cod,
                            Val_Cod,
                           AGRODATAINIZIO,
                           AGRODATAFINE,
                            BaseCode,
                            TopCode)

            'Inserisco l'XML nella stringa complessiva
            StrCodici &= StrCodice

        End If



        '------------------------------------------------
        '----- Costruisco la stringa XML della RUBRICA
        '------------------------------------------------

        Dim DT_Rub As DataTable
        DT_Rub = HttpContext.Current.Session("dt_Rubrica")

        'Azzero la stringa complessiva delle voci di rubrica
        'StrRubricaGruppo = ""

        'Inizializzo i valori
        'TipoOperazioneDB = enum_TipoOperazioneDB.Lettura
        If Rubrica_count > 0 Then
            TipoOperazioneDB = enum_TipoOperazioneDB.Modifica
        Else
            TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
        End If

        RubricaCodice = 0
        RubricaNumero = "."
        RubricaDescrizione = "."


        'Verifico tutte le voci della lista ...
        For Indice = 0 To DT_Rub.Rows.Count - 1

            '----- Ricavo il CODICE Rubrica

            'Cod_Rubrica = DT_Rub.Items(Indice).Value
            RubricaDescrizione = DT_Rub.Rows(Indice).Item("descr")
            RubricaNumero = DT_Rub.Rows(Indice).Item("numero")
            If Not IsDBNull(DT_Rub.Rows(Indice).Item("Cod_Rubrica")) AndAlso IsNumeric(DT_Rub.Rows(Indice).Item("Cod_Rubrica")) Then
                RubricaCodice = DT_Rub.Rows(Indice).Item("Cod_Rubrica")
            Else
                RubricaCodice = 0
            End If
            '----- Ricavo il NUMERO

            'If Cod_Rubrica >= 0 Then
            '    '-------------------------------------------------------------
            '    '(0547) 33.02.84.......... : Vecchio Numero Telefonico
            '    '                        |   |
            '    '                       25   28
            '    '-------------------------------------------------------------
            ''Testo = Left(ListRubrica.Items(Indice).Text, 25)
            'Testo = Left(DT_Rub.Rows(Indice).Item("numero"), 25)

            '    'Pulisco il numero dai puntini residui
            '    Carattere = Right(Testo, 1)

            '    Do While Carattere = "."
            '        Testo = Left(Testo, Len(Testo) - 1)
            '        Carattere = Right(Testo, 1)
            '    Loop

            '    RubricaNumero = Testo

            'Else
            '    'Non ha importanza ...
            '    RubricaNumero = "."
            'End If

            ''----- Ricavo la DESCRIZIONE

            'If Cod_Rubrica >= 0 Then
            'RubricaNumero = Mid(DT_Rub.Rows(Indice).Item("numero"), 29)
            'Else
            '    'Non ha importanza ...
            '    RubricaDescrizione = "."
            'End If



            'Creazione della stringa XML della singola voce
            Call XML_Rubrica(enum_CodificaDecodifica.Codifica,
                             StrRubricaSingola,
                             TipoOperazioneDB,
                             RubricaCodice,
                             RubricaNumero,
                             RubricaDescrizione,
                             AGRODATAINIZIO,
                             AGRODATAFINE,
                             BaseCode,
                             TopCode)


            'Inserimento nella stringa complessiva
            StrRubricaGruppo &= StrRubricaSingola

        Next

        '------------------------------------------------
        '----- Costruisco la stringa XML della RUBRICA
        '------------------------------------------------

        Dim DT_Cod As DataTable
        DT_Cod = HttpContext.Current.Session("dt_Codici")


        'Verifico tutte le voci della lista ...
        If DT_Cod IsNot Nothing Then
            For Indice = 0 To DT_Cod.Rows.Count - 1

                '----- Ricavo il CODICE Rubrica

                'Cod_Rubrica = DT_Rub.Items(Indice).Value
                CodiceID_Cod = DT_Cod.Rows(Indice).Item("ID_Cod")
                CodiceVal_Cod = DT_Cod.Rows(Indice).Item("val_Cod")

                '----- Ricavo il NUMERO

                'If Cod_Rubrica >= 0 Then
                '    '-------------------------------------------------------------
                '    '(0547) 33.02.84.......... : Vecchio Numero Telefonico
                '    '                        |   |
                '    '                       25   28
                '    '-------------------------------------------------------------
                ''Testo = Left(ListRubrica.Items(Indice).Text, 25)
                'Testo = Left(DT_Rub.Rows(Indice).Item("numero"), 25)

                '    'Pulisco il numero dai puntini residui
                '    Carattere = Right(Testo, 1)

                '    Do While Carattere = "."
                '        Testo = Left(Testo, Len(Testo) - 1)
                '        Carattere = Right(Testo, 1)
                '    Loop

                '    RubricaNumero = Testo

                'Else
                '    'Non ha importanza ...
                '    RubricaNumero = "."
                'End If

                ''----- Ricavo la DESCRIZIONE

                'If Cod_Rubrica >= 0 Then
                'RubricaNumero = Mid(DT_Rub.Rows(Indice).Item("numero"), 29)
                'Else
                '    'Non ha importanza ...
                '    RubricaDescrizione = "."
                'End If



                'Creazione della stringa XML della singola voce
                Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                 StrCodiceSingolo,
                                 enum_TipoOperazioneDB.Scrittura,
                                 CodiceID_Cod,
                                 CodiceVal_Cod,
                                 AGRODATAINIZIO,
                                 AGRODATAFINE,
                                 BaseCode,
                                 TopCode)


                'Inserimento nella stringa complessiva
                StrCodici &= StrCodiceSingolo

            Next
        End If

        TitoloPossesso = Me.Cmb_TitoloPossesso.SelectedItem.Value

        If Me.Cmb_TitoloPossesso.SelectedItem.Value <> "0" Then

            TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
            Id_Cod = enum_CodiciAnagrafe.TitoloPossesso
            Val_Cod = Me.Cmb_TitoloPossesso.SelectedItem.Value

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            TipoOperazioneDB,
                            Id_Cod,
                            Val_Cod,
                           AGRODATAINIZIO,
                           AGRODATAFINE,
                            BaseCode,
                            TopCode)

            'Inserisco l'XML nella stringa complessiva
            StrCodici &= StrCodice

        End If
        ''----- ORIENTAMENTO TECNICO ECONOMICO

        ''Recupero le informazioni

        ''Se non ho selezionato nulla allora non creo il codice
        'If ListOTE.Items.Count > 0 Then
        '    '-----------------------------------
        '    ' Text = Descrizione
        '    ' Value = codice OTE
        '    '-----------------------------------
        '    TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
        '    Id_Cod = enum_CodiciAnagrafe.OTE
        '    'Val_Cod = CStr(CmbOTE.SelectedItem.Value)
        '    Val_Cod = ""
        '    'scorro tutta la lista 
        '    For Indice = 0 To ListOTE.Items.Count - 1
        '        Val_Cod += ListOTE.Items(Indice).Value + "|"
        '    Next

        '    'trimmo l ultima pipe
        '    If Val_Cod.Length > 0 Then
        '        Val_Cod = Val_Cod.Substring(0, Val_Cod.Length - 1)
        '    End If

        '    'Genero l'XML del singolo nodo
        '    Call XML_Codice(enum_CodificaDecodifica.Codifica, _
        '                    StrCodice, _
        '                    TipoOperazioneDB, _
        '                    Id_Cod, _
        '                    Val_Cod, _
        '                   AGRODATAINIZIO, _
        '                   AGRODATAFINE, _
        '                    BaseCode, _
        '                    TopCode)

        '    'Inserisco l'XML nella stringa complessiva
        '    StrCodici = StrCodici & StrCodice

        'End If

        If Not IsNothing(Cmb_CentroEsterno) AndAlso
            Not IsNothing(Cmb_CentroEsterno.SelectedItem.Value) Then

            Id_Cod = enum_CodiciAnagrafe.Centro_Aziendale_Esterno_Collegato
            Val_Cod = CStr(Cmb_CentroEsterno.SelectedItem.Value)
            TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            TipoOperazioneDB,
                            Id_Cod,
                            Val_Cod,
                           AGRODATAINIZIO,
                           AGRODATAFINE,
                            BaseCode,
                            TopCode)

            'Inserisco l'XML nella stringa complessiva
            StrCodici &= StrCodice
        End If


        ''------------------------------------------------
        ''----- Costruisco la stringa XML del CENTRO AZIENDALE
        ''------------------------------------------------

        ''Definisco il tipo di operazione da eseguire
        TipoOperazioneDB = Operazione

        ''Recupero le altre informazioni

        'Piva = TxtPiva.Text
        'Sa_Cod = TxtSaCod.Text
        Piva = xPiva
        Sa_Cod = xSa_Cod
        Sa_Nome = TxtDenominazione.Text
        If HttpContext.Current.Session("coord_x") <> 0 Then
            CoordinataX = HttpContext.Current.Session("coord_x")
        Else
            CoordinataX = 0
        End If
        If HttpContext.Current.Session("coord_y") <> 0 Then
            CoordinataY = HttpContext.Current.Session("coord_y")
        Else
            CoordinataY = 0
        End If
        Zslm = "0"


        If HttpContext.Current.Session("lat") <> 0 Then
            Latitudine = HttpContext.Current.Session("lat")
        Else
            Latitudine = 0
        End If
        If HttpContext.Current.Session("lng") <> 0 Then
            Longitudine = HttpContext.Current.Session("lng")
        Else
            Longitudine = 0
        End If


        Area = "0"
        CA_Sipi = ""
        AT_Prevalente = If(ATPrevalenteText.Value, "")
        Forma_Possesso = ""

        TipoCentroCodice = Cmb_Tipologia.SelectedValue
        TipoCentroDescrizione = Cmb_Tipologia.SelectedItem.Text
        'TitoloPossesso = 0

        Sup_Totale = 0      'IIf(TxtSup_Totale.Text <> "", TxtSup_Totale.Text, 0)
        Sup_Tare = 0        'IIf(TxtSup_Totale.Text <> "", TxtSup_Totale.Text, 0)IIf(TxtSup_Tare.Text <> "", TxtSup_Tare.Text, 0)
        Sup_SAU = 0         'IIf(TxtSup_Totale.Text <> "", TxtSup_Totale.Text, 0)IIf(TxtSup_SAU.Text <> "", TxtSup_SAU.Text, 0)
        Sup_SAU_Convenzionale = 0       'IIf(TxtSup_Totale.Text <> "", TxtSup_Totale.Text, 0)IIf(Me.txtSup_SAU_Convenzionale.Text <> "", txtSup_SAU_Convenzionale.Text, 0)
        Sup_SAU_Conversione = 0         'IIf(TxtSup_Totale.Text <> "", TxtSup_Totale.Text, 0)IIf(Me.txtSup_SAU_Conversione.Text <> "", txtSup_SAU_Conversione.Text, 0)
        Sup_SAU_Biologico = 0           'IIf(TxtSup_Totale.Text <> "", TxtSup_Totale.Text, 0)IIf(Me.txtSup_SAU_Biologico.Text <> "", txtSup_SAU_Biologico.Text, 0)

        '---
        Sup_Totale = TxtSup_Totale.Text
        Sup_Tare = TxtSup_Tare.Text
        Sup_SAU = TxtSup_SAU.Text
        Sup_SAU_Convenzionale = txtSup_SAU_Convenzionale.Text
        Sup_SAU_Conversione = txtSup_SAU_Conversione.Text
        Sup_SAU_Biologico = txtSup_SAU_Biologico.Text


        If TxtSup_Bosco.Text = "" Then
            Sup_Bosco = 0
        Else
            If Not IsNumeric(TxtSup_Bosco.Text) Then
                Sup_Bosco = 0
            Else
                Sup_Bosco = TxtSup_Bosco.Text
            End If
        End If

        If TxtSup_Prati.Text = "" Then
            Sup_Prati = 0
        Else
            If Not IsNumeric(TxtSup_Prati.Text) Then
                Sup_Prati = 0
            Else
                Sup_Prati = TxtSup_Prati.Text
            End If
        End If

        '---
        If TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then
            'Vado a recuperare 
        End If

        'Genero la stringa XML del centro aziendale
        Call XML_CentroAziendale(
                        enum_CodificaDecodifica.Codifica,
                        StrCentroAziendale,
                        TipoOperazioneDB,
                        Piva,
                        Sa_Cod,
                        Sa_Nome,
                        CoordinataX,
                        CoordinataY,
                        Zslm,
                        Longitudine,
                        Latitudine,
                        Area,
                        CA_Sipi,
                        AT_Prevalente,
                        Forma_Possesso,
                        TipoCentroCodice,
                        Sup_Totale,
                        Sup_Bosco,
                        Sup_Tare,
                        Sup_SAU,
                        Sup_Prati,
                        Validita_Inizio,
                        Validita_Fine,
                        BaseCode,
                        TopCode, TitoloPossesso, Sup_SAU_Convenzionale, Sup_SAU_Conversione, Sup_SAU_Biologico)


        '------------------------------------------------
        '----- Costruisco la stringa XML complessiva di inserimento
        '------------------------------------------------

        'Creo il nodo "DatiCentriAziendali"
        XmlDatiCentriAziendali = XmlDoc.CreateElement("DatiCentriAziendali")

        'Inserisco il nodo "CentroAziendale"
        XmlDatiCentriAziendali.InnerXml = StrCentroAziendale

        'Rendo l'albero figlio del documento
        XmlDoc.AppendChild(XmlDatiCentriAziendali)

        'Faccio una copia del documento XML
        XmlDoc2 = XmlDoc

        'Seleziono il nodo "CentroAziendale"
        XmlCentroAziendale = XmlDoc.SelectSingleNode("//CentroAziendale")

        StrCodici &= StrCodiceGruppo

        'Inserisco gli elementi "Indirizzo" e "Codice"  e "Rubrica" come figli del nodo "CentroAziendale"
        XmlCentroAziendale.InnerXml = StrIndirizzo & StrCodici & StrRubricaGruppo

        'Estraggo la stringa XML complessiva
        StrXmlInserisci = XmlDoc.InnerXml

        '------------------------------------------------
        '----- Costruisco la stringa XML complessiva di cancellazione
        '------------------------------------------------
        If Operazione = enum_TipoOperazioneDB.Modifica Then

            If Session("StrXmlCodiciAttuali").ToString <> "" Then


                'Seleziono il nodo "CentroAziendale"
                XmlCentroAziendale = XmlDoc2.SelectSingleNode("//CentroAziendale")

                'Rendo il centro in lettura
                XmlCentroAziendale.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Lettura))

                'commento del 09/08/2011
                'prima c'era questo codice:

                ''Inserisco gli elementi "Codice" da cancellare
                'XmlCentroAziendale.InnerXml = Session("StrXmlCodiciAttuali")

                'poi a maggio 2011 piero ha modificato così per via di un bug in modifica di un cnetro che avesse dei codici:

                While XmlCentroAziendale.SelectSingleNode("Codice") IsNot Nothing
                    XmlCentroAziendale.RemoveChild(XmlCentroAziendale.SelectSingleNode("Codice"))
                End While



                'Inserisco gli elementi "Codice" da cancellare
                XmlCentroAziendale.InnerXml = String.Format("{0}{1}", XmlCentroAziendale.InnerXml,
                                                                        Session("StrXmlCodiciAttuali"))

                'il problema è che viene sdoppiata la rubrica
                'rimuovo dall'xml la rubrica e metto l'indirizzo in lettura

                While XmlCentroAziendale.SelectSingleNode("Rubrica") IsNot Nothing
                    XmlCentroAziendale.RemoveChild(XmlCentroAziendale.SelectSingleNode("Rubrica"))
                    'XmlCentroAziendale.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Cancellazione))
                End While



                'Seleziono il nodo "CentroAziendale"
                XmlInd = XmlDoc2.SelectSingleNode("//Indirizzo")

                'Rendo il centro in lettura
                XmlInd.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Lettura))


                'Estraggo la stringa XML complessiva
                StrXmlCancella = XmlDoc2.InnerXml

            End If

        End If


        'Distruggo gli oggetti
        XmlDatiCentriAziendali = Nothing
        XmlCentroAziendale = Nothing
        XmlDoc = Nothing
        XmlDoc2 = Nothing


        '=======================
        '===  Aggiornamento  ===
        '=======================
        '------------------------------------------------
        '----- apro connessione e transazione
        '------------------------------------------------
        ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
        Errore = False

        Dim OUT_Piva As String
        Dim OUT_SaCod As Integer

        Try

            '' Stefania
            'ParoleChiave_Salvatutto(Piva, objParametri_Server)

            '------------------------------------------------
            '----- Se sono in MODIFICA cancello i codici attuali
            '------------------------------------------------

            If Operazione = enum_TipoOperazioneDB.Modifica Then

                If Session("StrXmlCodiciAttuali") <> "" Then

                    'Creo gli oggetti COM+
                    '   *   CreateCANCELLATOObject("Agro_Anagrafe.CentroAziendale_W")

                    'Eseguo i comandi XML
                    StrDummy = objCentroAziendale.CentroAziendale_Scrivi(
                                    CStr(StrXmlCancella),
                                     OUT_Piva,
                                     OUT_SaCod,
                                      objParametri_Server,
                                      objParametri_Utenti,
                                      NoteLog:=NoteLog)

                    'DEBUG !!!!!
                    '''Throw New Exception("Errore generato da me medesimo !!!")

                End If

            End If


            '------------------------------------------------
            '----- Modifico o Inserisco l'azienda
            '------------------------------------------------

            'Creo gli oggetti COM+
            '   *   CreateCANCELLATOObject("Agro_Anagrafe.CentroAziendale_W")
            Dim Cod_Centro As Integer

            'Eseguo i comandi XML
            StrDummy = objCentroAziendale.CentroAziendale_Scrivi(
                                CStr(StrXmlInserisci),
                                OUT_Piva,
                                     OUT_SaCod,
                                      objParametri_Server,
                                      objParametri_Utenti,
                                      NoteLog:=NoteLog)

            Cod_Centro = CInt(OUT_SaCod)

            'Elimino gli oggetti COM+
            objCentroAziendale = Nothing



            '------------------------------------------------
            '----- Modifica Campi non gestiti dall'XML
            '------------------------------------------------

            'NOTA
            'Le ultime indicazioni prevedono che i due campi 'Classe di Rischio' e 'Fasi Critiche'
            'siano solo visualizzati e non editabili.
            'Il valore di tali campi si ricava dalla Cartella Aziendale - Periodi di Controllo
            'Dei vari record presenti per un centro aziendale, recupero il piu' recente ...


            'Dim objNewCampi As New object 'Agro_Anagrafe2_AD.CentriAziendali_Write()
            '   *   CreateCANCELLATOObject("Agro_Anagrafe2_AD.CentriAziendali_Write")

            'Dim ClasseRischio As Integer
            'Dim FasiCritiche As String

            'FasiCritiche = Me.Txt_FasiCritiche.Text

            'If Not IsNumeric(Me.Txt_ClasseRischio.Text) Then
            '    ClasseRischio = 0
            'Else
            '    ClasseRischio = CInt(Me.Txt_ClasseRischio.Text)
            'End If

            'objNewCampi.Modifica_NuoviCampi( _
            '                            Piva, _
            '                            Sa_Cod, _
            '                            FasiCritiche, _
            '                            ClasseRischio, _
            '                            , _
            '                            Session(ASG_.con..._server))

            'objNewCampi = Nothing





            '------------------------------------------------
            '----- Inserimento Magazzino se richiesto
            '------------------------------------------------

            If Operazione = enum_TipoOperazioneDB.Scrittura Then

                If Me.Chk_Magazzino.Checked = True Then

                    Dim objSequenza As New Agro_Sequenze 'New Agro_Anagrafe_AD.Agro_Sequenze
                    Dim objIndirizzo As New AgronicaCoreAnagrafeDAL.Indirizzi_Write 'New Agro_Anagrafe_AD.Indirizzi_Write
                    'Dim Tipo_Indirizzo As Integer

                    '----- Creo un nuovo record indirizzo

                    'Creo gli oggetti COM+
                    '   *   CreateCANCELLATOObject("Agro_Anagrafe_AD.Agro_Sequenze")
                    '   *   CreateCANCELLATOObject("Agro_Anagrafe_AD.Indirizzi_Write")

                    'Recupero un nuovo indice
                    Cod_Indirizzo = objSequenza.NuovoId_Tabella(
                                                            CStr("Indirizzi"),
                                                            CInt(BaseCode),
                                                            CInt(TopCode),
                                                            objParametri_Server)

                    'Memorizzo il nuovo codice indirizzo
                    Txt_CodIndirizzo.Text = Cod_Indirizzo

                    'Scrivo il nuovo record
                    Indice = objIndirizzo.Scrivi(
                                            CInt(Cod_Indirizzo),
                                            CStr(Ind_Des),
                                            CStr(Frz_Des),
                                            CStr(CAP),
                                            CStr(Com_Des),
                                            CStr(Pro_Cod),
                                            CStr(Stato),
                                            CStr(Note),
                                            CStr(Pro_Cod_Istat),
                                            CStr(Com_Cod_Istat),
                                            objParametri_Server.FinestraTemporaleInizio,
                                            objParametri_Server.FinestraTemporaleFine,
                                            objParametri_Server)

                    'Distruggo gli oggetti COM+
                    objSequenza = Nothing
                    objIndirizzo = Nothing



                    '------------------------------------------------
                    '-----  FABBRICATO  -----------------------------
                    '------------------------------------------------

                    Dim objSeqFabbricati As New Agro_Sequenze
                    Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_W

                    Dim Fabbricato_Cod As Integer
                    Dim Fabbricato_Des As String
                    Dim Tipo_Fabbricato_Cod As Integer
                    Dim Regolamento_Cod As Integer
                    Dim mc_Convenzionale As Double
                    Dim mc_Conversione As Double
                    Dim mc_Biologico As Double
                    Dim Conversione_Inizio As Date
                    Dim Conversione_Fine As Date

                    Dim p_Prov As String
                    Dim p_Com As String
                    Dim p_Sezione As String
                    Dim p_Foglio As Integer
                    Dim p_Numero As Integer
                    Dim p_Subalterno As String


                    Fabbricato_Des = "Magazzino " & Sa_Nome
                    Tipo_Fabbricato_Cod = 20
                    TitoloPossesso = 0
                    mc_Convenzionale = 0
                    mc_Conversione = 0
                    mc_Biologico = 0
                    Regolamento_Cod = 1
                    Conversione_Inizio = #1/1/1900#
                    Conversione_Fine = #1/1/1900#

                    p_Prov = "000"
                    p_Com = "000"
                    p_Sezione = ""
                    p_Foglio = 0
                    p_Numero = 0
                    p_Subalterno = ""


                    '----- Creo un nuovo record

                    'Creo gli oggetti COM+
                    '   *   CreateCANCELLATOObject("Agro_Anagrafe2_AD.Agro_Sequenze")
                    '   *   CreateCANCELLATOObject("Agro_Anagrafe2_AD.Fabbricati_W")

                    'Recupero un nuovo indice
                    Fabbricato_Cod = objSeqFabbricati.NuovoId_xPiva_xSaCod(
                                            CStr("SeqMagazzino"),
                                            CStr("Mag_Cod"),
                                            CStr(Piva),
                                            CInt(Cod_Centro),
                                            CInt(BaseCode),
                                            CInt(TopCode),
                                            objParametri_Server)

                    'Scrivo il nuovo record
                    Indice = objFabbricati.Scrivi(
                                            CStr(Piva),
                                            CInt(Cod_Centro),
                                            CInt(Fabbricato_Cod),
                                            CStr(Fabbricato_Des),
                                            CInt(Cod_Indirizzo),
                                            CInt(Tipo_Fabbricato_Cod),
                                            CStr(p_Prov),
                                            CStr(p_Com),
                                            CStr(p_Sezione),
                                            CInt(p_Foglio),
                                            CInt(p_Numero),
                                            CStr(p_Subalterno),
                                            CDbl(mc_Convenzionale),
                                            CDbl(mc_Conversione),
                                            CDbl(mc_Biologico),
                                            CInt(Regolamento_Cod),
                                            CInt(TitoloPossesso),
                                            CInt(0),
                                            CInt(0),
                                            CInt(0),
                                            CInt(0),
                                            CInt(0),
                                            CInt(0),
                                            CInt(0),
                                            CInt(0),
                                            CInt(0),
                                            CInt(0),
                                            CInt(0),
                                            CInt(0),
                                            CDate(Conversione_Inizio),
                                            CDate(Conversione_Fine),
                                            0, 0, 0, 0, 0, 0, 0, 0, "",
                                            CostantiPersonalizzate.AGRODATAINIZIO,
                                            0,
                                            CDate(Validita_Inizio),
                                            CDate(Validita_Fine),
                                            objParametri_Server)

                    'Distruggo gli oggetti COM+
                    objSeqFabbricati = Nothing
                    objFabbricati = Nothing


                End If

            End If




            ''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

            ''------------------------------------------------
            ''----- Validazione dati inseriti
            ''------------------------------------------------

            ''NOTA
            ''La routine seguenti verifica se l'utente dispone dei permessi
            ''di validazione dei dati.
            ''In caso negativo imposta a (-1) il flag di validazione
            ''dell'impresa e invia un messaggio all'Ufficio Segreteria Tecnica

            'Call Gestione_Validazione_Dati(Operazione, _
            '                                Piva, _
            '                                AgroLabel_CentroAziendale & _
            '                                    " : " & Sa_Nome & " (" & Piva & ") ", _
            '                                1, _
            '                                "", _
            '                                Session, _
            '                                Server)

            ''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


            If Operazione = enum_TipoOperazioneDB.Scrittura AndAlso OUT_Piva <> "" AndAlso OUT_SaCod <> 0 Then
                Dim objUtentiVisibilitaR As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim DtImpreseVisibili As DataTable
                DtImpreseVisibili = objUtentiVisibilitaR.Leggi(enum_TipoEntita.Impresa, "", "", objParametri_Server)
                If DtImpreseVisibili IsNot Nothing AndAlso DtImpreseVisibili.Rows.Count > 0 Then
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
                    objUtentiVisibilita.Scrivi(enum_TipoEntita.Centro, OUT_Piva, OUT_SaCod, 0, 0, objParametri_Server)
                End If
            End If


            '------------------------------------------------
            '----- Conferma di aggiornamento del database
            '------------------------------------------------

            'EseguitaOperazione = True


            '------------------------------------------------
            'chiudo la transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            'chiudo la connessione
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

            HttpContext.Current.Session("dt_Rubrica") = Nothing


        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------
            Errore = True

            'chiudo la transazione con il rollback
            If objParametri_Server.objConnessione IsNot Nothing Then
                If objParametri_Server.objTransazione IsNot Nothing Then
                    'chiudo transazione
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                End If
                'chiudo la connessione
                ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
            End If

            'Messaggio di errore
            StrDummy = exc.Message.ToString()

            'Faccio abortire la transazione
            'System.EnterpriseServices.ContextUtil.SetAbort()

            'FACCIO APPARIRE UN ALERT......
            AgroMsgBox("Si e' verificato un errore durante la fase di salvataggio : " & Chr(13) & StrDummy, Page)

            '------------------------------------------------

        End Try

        '============================
        '===  Fine Aggiornamento  ===
        '============================
        If Errore = False Then
            'Dim Piva As String

            'Se la transazione ha avuto esito positivo allora ...
            If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
                'impresa appena inserita
                Piva = Qs_PivaNuova
            Else
                If Qs_Piva <> "" Then
                    Piva = Qs_Piva
                End If
            End If
            'Piva = Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server)

            'Ritorno alla pagina AlberoImprese
            'Response.Redirect(Qs_PaginaRitorno & "?P=" & Piva)

            Dim Messaggio2 As String
            Messaggio2 = DirectCast(GetLocalResourceObject("CENTROAZIENDALESalvatoConSuccesso"), String) & vbCrLf & vbCrLf

            Dim TargetUrl As String
            Select Case tipo_salva.Value

                Case 1
                    Messaggio2 += AgronicaAgenda_2010.VerràRicaricataLaPaginaPerInserimento & vbCrLf
                    'Messaggi.AgroMsgBox(Messaggio2, Page, , UpdatePanel_script, , True)

                    AgroMsgBox(Messaggio2, Page)

                    Page_Load(Nothing, EventArgs.Empty)
                    clear_form()
                Case Else
                    'Messaggi.AgroMsgBox(Messaggio2, Page, , UpdatePanel_script, , True)

                    TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda, , Qs_Visibilita)
                    'Response.Redirect(TargetUrl)
                    AgroMsgBox(Messaggio2, Page, , , "window.location = '" & TargetUrl & "';")

            End Select
        End If



    End Sub

    Private Sub clear_form()

        Cmb_CentroEsterno.ClearSelection()
        TxtDenominazione.Text = ""
        Cmb_Tipologia.ClearSelection()
        Txt_Via.Text = ""
        Txt_Frazione.Text = ""
        Cmb_Provincia.ClearSelection()
        Cmb_Comune.ClearSelection()
        Txt_CAP.Text = ""
        'cmb_Stato.ClearSelection()
        cmb_Stato.SelectedValue = "IT"
        Txt_Note.Text = ""
        TxtRubrica_Numero.Text = ""
        Cmb_Rubrica_Descrizione.ClearSelection()


        TxtValiditaInizio.Text = ""
        TxtValiditaFine.Text = ""
        CmbCodice.ClearSelection()
        TxtCodiceValore.Text = ""
        TxtValiditaInizioCodice.Text = ""
        TxtValiditaFineCodice.Text = ""


        TxtCodiceImpresa.Text = ""
        chkLast.Checked = False
        CmbTipoAttivita.ClearSelection()
        CmbOrganismoControllo.ClearSelection()

        TxtSup_Totale.Text = ""
        TxtSup_Tare.Text = ""
        TxtSup_SAU.Text = ""
        txtSup_SAU_Convenzionale.Text = ""
        txtSup_SAU_Conversione.Text = ""
        txtSup_SAU_Biologico.Text = ""
        TxtSup_Bosco.Text = ""
        TxtSup_Prati.Text = ""

    End Sub

    Private Sub Centro_Edit_PreLoad(sender As Object, e As EventArgs) Handles Me.PreLoad

    End Sub

    'Protected Sub GridView_Rubrica_RowComand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView_Rubrica.RowCommand

    '    ' Setto la tipologia di operazione su rubrica per il controllo lato-pagina
    '    OperazioneSuRubrica = e.CommandName

    '    Select Case e.CommandName

    '        ' Modifica della riga
    '        Case "Modifica"

    '            Dim IndiceRigaGriglia As Integer = 0

    '            'Recupero l'indice di riga del datagrid
    '            IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

    '            'Recupero il datatable
    '            Dim Dt_Rubrica As DataTable

    '            Dt_Rubrica = Session("vs_dtRubrica")


    '            If Not Dt_Rubrica Is Nothing Then
    '                TxtRubrica_Numero.Text = Dt_Rubrica.Rows(IndiceRigaGriglia).Item("numero")

    '                ' Gestione della tipologia di contatto
    '                Select Case Dt_Rubrica.Rows(IndiceRigaGriglia).Item("descr")
    '                    Case "Telefono"
    '                        Cmb_Rubrica_Descrizione.SelectedValue = "Telefono"

    '                    Case "Telef"
    '                        Cmb_Rubrica_Descrizione.SelectedValue = "Telefono"

    '                    Case "Cellulare"
    '                        Cmb_Rubrica_Descrizione.SelectedValue = "Cellulare"

    '                    Case "Fax"
    '                        Cmb_Rubrica_Descrizione.SelectedValue = "Fax"

    '                    Case "Email"
    '                        Cmb_Rubrica_Descrizione.SelectedValue = "Email"

    '                End Select


    '            End If

    '            ' Cancellazione della riga
    '        Case "Cancella"
    '            Dim i As Integer
    '            i = 2

    '    End Select

    'End Sub

End Class