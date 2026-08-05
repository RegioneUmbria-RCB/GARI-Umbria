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
Imports AgroAgenda_2010.Resources
Imports Newtonsoft.Json

Public Class Catasto_Edit
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda

    Public Operazione As Integer
    Dim xPiva As String
    Dim xSa_Cod As String
    Dim Qs_Visibilita As Integer = 0

    Public permessi As PermessiUtente

    Public jsPossessi As String
    Public jsMacrousi As String
    Public jsZone As String
    Public jsClassamenti As String
    Dim GridView_Possessi As Object

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiungi_Possesso(ByVal codice As String, ByVal possesso As String, ByVal superficie As String, ByVal cod_particella As String, ByVal DataInizio As String, ByVal DataFine As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim Contatore As Integer
        Dim flag As Boolean = True

        Lingua.Gias_InizializzaCultura_DaSession()

        Dt = HttpContext.Current.Session("dt_Possessi")

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


        '----- Cerco il valore massimo del contatore

        Contatore = 0

        For i = 0 To Dt.Rows.Count - 1
            If Dt.Rows(i).Item("Contatore") > Contatore Then
                Contatore = Dt.Rows(i).Item("Contatore")
            End If
        Next

        Contatore = Contatore + 1

        '----- Inserisco la riga nel datagrid

        'Creo una nuova riga
        Dr = Dt.NewRow

        'Definisco i valori
        Dr.Item("Contatore") = Contatore
        Dr.Item("TitoloPossesso_Des") = possesso
        Dr.Item("TitoloPossesso_Cod") = codice
        Dr.Item("Sup_Condotta") = superficie
        Dr.Item("Cod_Particella") = cod_particella

        If DataInizio = #1/1/1900# Then
            Dr.Item("Validita_Inizio") = "..."
        Else
            ' Dr.Item("Validita_Inizio") = DataInizio.ToShortDateString
            Dr.Item("Validita_Inizio") = DataInizio
        End If

        If DataFine = #12/31/2100# Then
            Dr.Item("Validita_Fine") = "..."
        Else
            ' Dr.Item("Validita_Fine") = DataFine.ToShortDateString
            Dr.Item("Validita_Fine") = DataFine
        End If

        Dim MessaggioErrore As String = ""

        'Controllo se esiste già la voce che si sovrappone a quelle già inserite
        For i = 0 To Dt.Rows.Count - 1

            ''Recupero i valori della riga i-esima
            'If (Me.DataGrid_Possesso.Items(i).Cells(3).Text = "...") Then
            '    xDataInizio = #1/1/1900#
            'Else
            '    xDataInizio = CDate(Me.DataGrid_Possesso.Items(i).Cells(3).Text)
            'End If

            'If (Me.DataGrid_Possesso.Items(i).Cells(4).Text = "...") Then
            '    xDataFine = #12/31/2100#
            'Else
            '    xDataFine = CDate(Me.DataGrid_Possesso.Items(i).Cells(4).Text)
            'End If

            'Dim xDataInizio As Date = CDate(lbl_centro_az_data_inizio.text)
            'Dim xDataFine As Date = CDate(lbl_centro_az_data_fine.text)

            'Verifico
            Dim DataInizioWA As Date = AGRODATAINIZIO
            If IsDate(Dt.Rows(i).Item("Validita_Inizio")) Then
                DataInizioWA = CDate(Dt.Rows(i).Item("Validita_Inizio"))
            End If
            Dim DataFineWA As Date = AGRODATAFINE
            If IsDate(Dt.Rows(i).Item("Validita_Fine")) Then
                DataFineWA = Dt.Rows(i).Item("Validita_Fine")
            End If
            If (DataInizio >= DataInizioWA) And (DataInizio <= DataFineWA) Then
                'Intersezione
                flag = False
                MessaggioErrore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Catasto_Edit.aspx", "IntervalloImpostatoSiSovrapponeAiPrecedenti"), String)

                'Esco dal ciclo
                Exit For
            Else
                If (DataFine >= DataInizioWA) And (DataFine <= DataFineWA) Then
                    'Intersezione
                    flag = False
                    MessaggioErrore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Catasto_Edit.aspx", "IntervalloImpostatoSiSovrapponeAiPrecedenti"), String)

                    'Esco dal ciclo
                    Exit For
                Else

                    ' DA DECOMMENTARE!!!

                    'If DataInizio < CDate(lbl_centro_az_data_inizio.text) Then
                    '    flag = False
                    '    MessaggioErrore += "L'inizio del periodo non puo' precedere la creazione del Centro Aziendale. (" & lbl_centro_az_data_inizio.text & ")"

                    '    'Esco dal ciclo
                    '    Exit For
                    'End If

                    'If DataFine > CDate(lbl_centro_az_data_fine.text) Then
                    '    flag = False
                    '    MessaggioErrore += "La fine del periodo non puo' seguire la cessazione del Centro Aziendale. (" & lbl_centro_az_data_fine.text & ")"

                    '    'Esco dal ciclo
                    '    Exit For

                    'End If

                End If

            End If

        Next

        If (flag) Then
            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)
            HttpContext.Current.Session("dt_Possessi") = Dt
            Dim str_Risposta = DT_to_Json_Possessi(Dt)
            r.RispostaOK = True
            r.RispostaStringa = str_Risposta
        Else
            r.RispostaOK = False
            r.Errore = MessaggioErrore
        End If

        Return r

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Set_Comune(ByVal comune As String, ByVal nome_comune As String)

        HttpContext.Current.Session("cod_com") = comune
        HttpContext.Current.Session("nome_comune_settato") = nome_comune

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_ProvCom_Estero(ByVal cod_prov As String, ByVal cod_com As String)

        HttpContext.Current.Session("cod_prov") = cod_prov
        HttpContext.Current.Session("cod_com") = cod_com


    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiungi_Macrouso(ByVal codice As String, ByVal macrouso As String, ByVal superficie As String, ByVal DataInizio As String, ByVal DataFine As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim Contatore As Integer
        Dim flag As Boolean = True

        Lingua.Gias_InizializzaCultura_DaSession()

        Dt = HttpContext.Current.Session("dt_Macrousi")

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


        '----- Cerco il valore massimo del contatore

        Contatore = 0

        For i = 0 To Dt.Rows.Count - 1
            If Dt.Rows(i).Item("Contatore") > Contatore Then
                Contatore = Dt.Rows(i).Item("Contatore")
            End If
        Next

        Contatore = Contatore + 1

        '----- Inserisco la riga nel datagrid

        'Creo una nuova riga
        Dr = Dt.NewRow

        'Definisco i valori
        Dr.Item("Contatore") = Contatore
        Dr.Item("Descrizione") = macrouso
        Dr.Item("Macrouso_Cod") = codice
        Dr.Item("Superficie") = superficie

        If DataInizio = #1/1/1900# Then
            Dr.Item("Validita_Inizio") = "..."
        Else
            ' Dr.Item("Validita_Inizio") = DataInizio.ToShortDateString
            Dr.Item("Validita_Inizio") = DataInizio
        End If

        If DataFine = #12/31/2100# Then
            Dr.Item("Validita_Fine") = "..."
        Else
            ' Dr.Item("Validita_Fine") = DataFine.ToShortDateString
            Dr.Item("Validita_Fine") = DataFine
        End If


        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1

            If (Dt.Rows(i).Item("Macrouso_Cod") = codice) Then
                flag = False
            End If
        Next

        If (flag) Then
            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)
            HttpContext.Current.Session("dt_Macrousi") = Dt
            Dim str_Risposta = DT_to_Json_Macrousi(Dt)
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
    Public Shared Function Aggiungi_Zona(ByVal codice As String, ByVal zona As String, ByVal superficie As String, ByVal DataInizio As String, ByVal DataFine As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Dt As New DataTable
        Dim Dr As DataRow
        'Dim Contatore As Integer
        Dim flag As Boolean = True

        Lingua.Gias_InizializzaCultura_DaSession()

        Dt = HttpContext.Current.Session("dt_Zone")

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

        '----- Cerco il valore massimo del contatore

        'Contatore = 0

        'For i = 0 To Dt.Rows.Count - 1
        '    If Dt.Rows(i).Item("Contatore") > Contatore Then
        '        Contatore = Dt.Rows(i).Item("Contatore")
        '    End If
        'Next

        'Contatore = Contatore + 1

        '----- Inserisco la riga nel datagrid

        'Creo una nuova riga
        Dr = Dt.NewRow

        'Definisco i valori
        'Dr.Item("Contatore") = Contatore
        Dr.Item("Descrizione") = zona
        Dr.Item("Zona_Cod") = codice
        Dr.Item("Area") = superficie

        If DataInizio = #1/1/1900# Then
            Dr.Item("Validita_Inizio") = "..."
        Else
            ' Dr.Item("Validita_Inizio") = DataInizio.ToShortDateString
            Dr.Item("Validita_Inizio") = DataInizio
        End If

        If DataFine = #12/31/2100# Then
            Dr.Item("Validita_Fine") = "..."
        Else
            ' Dr.Item("Validita_Fine") = DataFine.ToShortDateString
            Dr.Item("Validita_Fine") = DataFine
        End If

        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1

            If (Dt.Rows(i).Item("Zona_Cod") = codice) Then
                flag = False
            End If
        Next

        If (flag) Then
            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)
            HttpContext.Current.Session("dt_Zone") = Dt
            Dim str_Risposta = DT_to_Json_Zone(Dt)
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
    Public Shared Function Aggiungi_Classamento(ByVal codice As String, ByVal qualita As String, ByVal superficie As String, ByVal porzione As String, ByVal classeCatasto As String, ByVal RedditoDom As String, ByVal RedditoAgr As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Dt As New DataTable
        Dim Dr As DataRow
        'Dim Contatore As Integer
        Dim flag As Boolean = True

        Lingua.Gias_InizializzaCultura_DaSession()

        Dt = HttpContext.Current.Session("dt_Classamenti")


        '----- Inserisco la riga nel datagrid

        'Creo una nuova riga
        Dr = Dt.NewRow

        'Definisco i valori
        Dr.Item("QUALITA_COD") = codice
        Dr.Item("QUALITA_Des") = qualita
        Dr.Item("Porzione") = porzione
        Dr.Item("CLASSE") = classeCatasto
        Dr.Item("Sup_Classe") = superficie
        Dr.Item("REDDITO_DOMINICALE") = RedditoDom
        Dr.Item("REDDITO_AGRARIO") = RedditoAgr



        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1

            If (Dt.Rows(i).Item("QUALITA_COD") = codice) Then
                flag = False
            End If
        Next

        If (flag) Then
            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)
            HttpContext.Current.Session("dt_Classamenti") = Dt
            Dim str_Risposta = DT_to_Json_Classamenti(Dt)
            r.RispostaOK = True
            r.RispostaStringa = str_Risposta
        Else
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.CodiceGiàInserito
        End If

        Return r

    End Function



    '##########################################################################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Possessi(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        Lingua.Gias_InizializzaCultura_DaSession()

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        Dim cn As New ColonneNome("TitoloPossesso_Cod", "", "string")
        cn._FormatoParticolare = "<span></span>"
        cn._Filtrabile = False
        l.Add(cn)


        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("TitoloPossesso_Cod", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            'listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaCodice(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaPossesso(this);"))

        End If

        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)


        cn = New ColonneNome("TitoloPossesso_Des", AgronicaAgenda_2010.Descrizione, "string")
        l.Add(cn)

        'cn = New ColonneNome("TitoloPossesso_Cod", "Codice", "string")
        'l.Add(cn)

        cn = New ColonneNome("Validita_Inizio", AgronicaAgenda_2010.Dal, "string")
        l.Add(cn)

        cn = New ColonneNome("Validita_Fine", AgronicaAgenda_2010.Al, "string")
        l.Add(cn)

        cn = New ColonneNome("Sup_Condotta", AgronicaAgenda_2010.Superficie, "string")
        l.Add(cn)

        cn = New ColonneNome("Cod_Particella", AgronicaAgenda_2010.CodiceParticellaAbbr, "string")
        cn = New ColonneNome("Cod_Particella", AgronicaAgenda_2010.CodiceParticellaAbbr, "string")
        l.Add(cn)


        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaPossesso(ByVal Id_Cod As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dt = HttpContext.Current.Session("dt_Possessi")

        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1
            If (Dt.Rows(i).Item("TitoloPossesso_Cod") = Id_Cod) Then
                Dt.Rows.RemoveAt(i)
                Exit For
            End If
        Next
        HttpContext.Current.Session("dt_Possessi") = Dt
        Dim str_Risposta = DT_to_Json_Possessi(Dt)
        r.RispostaOK = True
        r.RispostaStringa = str_Risposta
        Return r
    End Function


    '##########################################################################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Macrousi(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        Lingua.Gias_InizializzaCultura_DaSession()

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        Dim cn As New ColonneNome("Macrouso_Cod", "", "string")
        cn._FormatoParticolare = "<span></span>"
        cn._Filtrabile = False
        l.Add(cn)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Macrouso_Cod", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            'listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaCodice(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaMacrouso(this);"))

        End If

        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)


        cn = New ColonneNome("Descrizione", AgronicaAgenda_2010.Descrizione, "string")
        l.Add(cn)

        'cn = New ColonneNome("TitoloPossesso_Cod", "Codice", "string")
        'l.Add(cn)

        cn = New ColonneNome("Validita_Inizio", AgronicaAgenda_2010.Dal, "string")
        l.Add(cn)

        cn = New ColonneNome("Validita_Fine", AgronicaAgenda_2010.Al, "string")
        l.Add(cn)

        cn = New ColonneNome("Superficie", AgronicaAgenda_2010.Superficie, "string")
        l.Add(cn)


        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaMacrouso(ByVal Id_Cod As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dt = HttpContext.Current.Session("dt_Macrousi")

        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1
            If (Dt.Rows(i).Item("Macrouso_Cod") = Id_Cod) Then
                Dt.Rows.RemoveAt(i)
                Exit For
            End If
        Next
        HttpContext.Current.Session("dt_Macrousi") = Dt
        Dim str_Risposta = DT_to_Json_Macrousi(Dt)
        r.RispostaOK = True
        r.RispostaStringa = str_Risposta
        Return r
    End Function


    '##########################################################################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Classamenti(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        Lingua.Gias_InizializzaCultura_DaSession()

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        Dim cn As New ColonneNome("QUALITA_COD", "", "string")
        cn._FormatoParticolare = "<span></span>"
        cn._Filtrabile = False
        l.Add(cn)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("QUALITA_COD", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            'listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaCodice(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaClassamento(this);"))
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

        cn = New ColonneNome("QUALITA_Des", AgronicaAgenda_2010.Descrizione, "string")
        l.Add(cn)

        cn = New ColonneNome("Porzione", AgronicaAgenda_2010.Porzione, "string")
        l.Add(cn)

        cn = New ColonneNome("CLASSE", AgronicaAgenda_2010.Classe, "string")
        l.Add(cn)

        cn = New ColonneNome("Deduzione", AgronicaAgenda_2010.Deduzione, "string")
        l.Add(cn)

        cn = New ColonneNome("Sup_Classe", AgronicaAgenda_2010.SuperficieClasse, "string")
        l.Add(cn)

        cn = New ColonneNome("REDDITO_DOMINICALE", AgronicaAgenda_2010.RedditoDominicale, "string")
        l.Add(cn)

        cn = New ColonneNome("REDDITO_AGRARIO", AgronicaAgenda_2010.RedditoAgrario, "string")
        l.Add(cn)


        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaClassamento(ByVal Id_Cod As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dt = HttpContext.Current.Session("dt_Classamenti")

        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1
            If (Dt.Rows(i).Item("QUALITA_COD") = Id_Cod) Then
                Dt.Rows.RemoveAt(i)
                Exit For
            End If
        Next
        HttpContext.Current.Session("dt_Classamenti") = Dt
        Dim str_Risposta = DT_to_Json_Classamenti(Dt)
        r.RispostaOK = True
        r.RispostaStringa = str_Risposta
        Return r
    End Function


    '##########################################################################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Zone(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        Lingua.Gias_InizializzaCultura_DaSession()

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)


        Dim cn As New ColonneNome("Zona_Cod", "", "string")
        cn._FormatoParticolare = "<span></span>"
        cn._Filtrabile = False
        l.Add(cn)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Zona_Cod", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            'listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaCodice(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaZona(this);"))

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

        cn = New ColonneNome("Descrizione", AgronicaAgenda_2010.Descrizione, "string")
        l.Add(cn)

        'cn = New ColonneNome("TitoloPossesso_Cod", "Codice", "string")
        'l.Add(cn)

        cn = New ColonneNome("Area", AgronicaAgenda_2010.Area, "string")
        l.Add(cn)

        cn = New ColonneNome("Validita_Inizio", AgronicaAgenda_2010.ValiditàInizio, "date")
        l.Add(cn)

        cn = New ColonneNome("Validita_Fine", AgronicaAgenda_2010.ValiditàFine, "date")
        l.Add(cn)


        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaZona(ByVal Id_Cod As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dt = HttpContext.Current.Session("dt_Zone")

        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1
            If (Dt.Rows(i).Item("Zona_Cod") = Id_Cod) Then
                Dt.Rows.RemoveAt(i)
                Exit For
            End If
        Next
        HttpContext.Current.Session("dt_Zone") = Dt
        Dim str_Risposta = DT_to_Json_Zone(Dt)
        r.RispostaOK = True
        r.RispostaStringa = str_Risposta
        Return r
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Set_Comune(ByVal comune As String)

        HttpContext.Current.Session("comune_settato") = comune

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
                'If itm.Value <> "" Then
                '    options &= "<option value=""" & itm.Value & """>(" & itm.Value.Substring(3, 3) & ") : " & itm.Text.ToUpper & "</option>"
                'Else
                options &= "<option value=""" & itm.Value & """>" & itm.Text.ToUpper & "</option>"
                'End If


            Next

            rval(0) = options

            Dim objI As New AgronicaCoreMetaSchemaDAL.Istat_R
            'Imposto la textbox del CODICE ISTAT
            rval(1) = objI.CodIstat_from_Provincia(provincia, HttpContext.Current.Session("ASG_objParametri_Server"))


        End If

        Return rval

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function controllo_Catasto_Insert(prov As String, com As String, sez As String, foglio As Integer, numero As Integer, subalterno As String) As RispostaStandard
        Dim r As New RispostaStandard
        Lingua.Gias_InizializzaCultura_DaSession()
        Try
            Dim objParametriAgenda = New ParametriAgenda

            If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura OrElse objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
                Dim exists As Boolean = False
                Dim objParticelle As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
                Dim dt = objParticelle.Leggi(0, objParametriAgenda.Piva, 0, 0, prov, com, If(sez = "", "0", sez), foglio, numero, If(subalterno = "", "0", subalterno), enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    exists = True
                End If

                'escludo la particella che si sta modificando
                If exists AndAlso objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
                    If prov = objParametriAgenda.Particelle(0).Provincia AndAlso
                        com = objParametriAgenda.Particelle(0).Comune AndAlso
                        If(sez = "", "0", sez) = objParametriAgenda.Particelle(0).Sezione AndAlso
                        foglio = objParametriAgenda.Particelle(0).Foglio AndAlso
                        numero = objParametriAgenda.Particelle(0).Numero AndAlso
                        If(subalterno = "", "0", subalterno) = objParametriAgenda.Particelle(0).Subalterno Then
                        exists = False
                    End If
                End If

                r.RispostaOK = True
                r.RispostaStringa = exists

            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaMetodiProduzione(prov As String, com As String, sez As String, foglio As Integer, numero As Integer, subalterno As String) As RispostaStandard
        Dim r As New RispostaStandard()
        r.RispostaOK = True

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim ObjMetodoProduzione_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_MetodoProduzione_R
            Dim dtRisp = ObjMetodoProduzione_R.Leggi(
                    prov,
                    com,
                    sez,
                    foglio,
                    numero,
                    subalterno,
                    -1,
                    AGRODATAINIZIO,
                    AGRODATAFINE,
                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                    "",
                    "",
                    objParametri_Server)

            dtRisp.Columns.Add("Chiave", Type.GetType("System.Int32"))
            'dtRisp.Columns.Add("MetodoProduzione_Des", Type.GetType("System.String"))
            For i = 0 To dtRisp.Rows.Count - 1
                dtRisp.Rows(i).SetField(Of Integer)("Chiave", i)

                Select Case dtRisp.Rows(i).Field(Of Integer)("MetodoProduzione_Cod")

                    Case enum_TipoAgricoltura.Convenzionale
                        dtRisp.Rows(i).SetField(Of String)("MetodoProduzione_Des", AgronicaAgenda_2010.Convenzionale)

                    Case enum_TipoAgricoltura.InConversione
                        dtRisp.Rows(i).SetField(Of String)("MetodoProduzione_Des", AgronicaAgenda_2010.InConversione)

                    Case enum_TipoAgricoltura.Biologica
                        dtRisp.Rows(i).SetField(Of String)("MetodoProduzione_Des", AgronicaAgenda_2010.Biologico)

                End Select
            Next

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtRisp, Formatting.None, serializerSettings)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Sub SalvaGrigliaMetodiProduzione(ByVal righeGrigliaNonEliminate As String, ByVal modificheEffettuate As Boolean)
        'La routine viene eseguita sempre prima del salvataggio generale della Particella
        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                'In caso la sessione sia scaduta, interrompo l'esecuzione della routine il redirect alla login viene fatto dal page_load
                Exit Sub
            End If

            Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

            HttpContext.Current.Session("righeGrigliaPCatastaleMetodiProd") = JsonConvert.DeserializeObject(Of List(Of PCatastaleMetodoProduzioneModel))(righeGrigliaNonEliminate, settingLoc)
            HttpContext.Current.Session("grigliaPCatastaleMetodiProd_modificata") = modificheEffettuate

        Catch ex As Exception
            Throw New Exception(AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False))
        End Try
    End Sub

    Protected Overrides ReadOnly Property PageStatePersister As PageStatePersister
        Get
            Return New SessionPageStatePersister(Me)
        End Get
    End Property



    Private Sub Particella_Edit_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        '  Master_Operazione = CType(Page.Master, Operazione)
        ' VAnni: 2/10/2017: a seguito di AgroMaseterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        Master.flag_MostraBtnIndietro = True
        ' AddHandler CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Salva"), ImageButton).Click, AddressOf Me.SalvaTutto
        'AddHandler Master_Operazione.Property_Btn_Conferma_Ricetta.Click, AddressOf Me.Btn_Conferma_Ricetta


    End Sub

    '##########################################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim TargetUrl As String
        TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda, , Qs_Visibilita)
        Response.Redirect(TargetUrl)

    End Sub

    '##########################################################################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        permessi = New PermessiUtente()

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
        'UtenteAbilitato_Lettura = objPermessi.Controlla_Permessi_Utente( _
        '                            Session("ASG_Utente_Username"), _
        '                            Session("ASG_IdServizio"), _
        '                            enum_Security_Attivita.Anagrafica_ParticellaCatastale, _
        '                            enum_Security_Operazione.Lettura, _
        '                            Date.Now, _
        '                            "", _
        '                            objParametri_Utenti)

        'Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura

        Dim UtenteAbilitato_Modifica As Boolean = False
        'UtenteAbilitato_Modifica = objPermessi.Controlla_Permessi_Utente( _
        '                            Session("ASG_Utente_Username"), _
        '                            Session("ASG_IdServizio"), _
        '                            enum_Security_Attivita.Anagrafica_ParticellaCatastale, _
        '                            enum_Security_Operazione.Modifica, _
        '                            Date.Now, _
        '                            "", _
        '                            objParametri_Utenti)


        UtenteAbilitato_Lettura = permessi.getPermesso(enum_Security_Attivita.Anagrafica_ParticellaCatastale).Lettura
        UtenteAbilitato_Modifica = permessi.getPermesso(enum_Security_Attivita.Anagrafica_ParticellaCatastale).Scrittura


        Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica
        Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura


        If UtenteAbilitato_Modifica = False Then
            Response.Redirect("../MenuAnagrafica/Menubs_anagrafica.aspx")
            Exit Sub
        End If

        objParametriAgenda = New ParametriAgenda

        Select Case objParametriAgenda.Tipo_Operazione
            Case enum_TipoOperazioneDB.Scrittura
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("CreazioneNuovaParticellaAziendale"), String)
            Case enum_TipoOperazioneDB.Lettura
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("LetturaParticellaAziendale"), String)
            Case enum_TipoOperazioneDB.Modifica
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("ModificaParticellaAziendale"), String)
        End Select
        Operazione = objParametriAgenda.Tipo_Operazione

        HttpContext.Current.Session("operazione") = Operazione


        xPiva = objParametriAgenda.Piva
        xSa_Cod = objParametriAgenda.Sa_Cod


        If Not IsPostBack Then
            HttpContext.Current.Session("cod_prov") = ""
            HttpContext.Current.Session("cod_com") = ""
        Else
            Exit Sub
        End If

        'NB: Nel caso la pagina venga ricaricata in fase di postback va in errore sul js perché alcune variabili necessarie sono inizializzate all'interno di questa funzione.
        'Ciò si verifica per esempio se la pagina non supera dei controlli di validità nella funzione Salva_Tutto, pertanto aggiungo i nuovi controlli per la griglia dei metodi di produzione sul javascript
        'Non è possibile però far eseguire questa funzione anche in caso di postback perché si creano errori in fase di salvataggio (la particella non viene più trovata nella griglia generale).
        RipristinaDatiNeiControlli()


        If Operazione = enum_TipoOperazioneDB.Lettura Then

            Cmb_Provincia.Enabled = False
            Cmb_Comune.Enabled = False
            Txt_Sezione.Enabled = False
            Txt_Foglio.Enabled = False
            Txt_Numero.Enabled = False
            Txt_Subalterno.Enabled = False
            TxtSup_Ettari.Enabled = False
            TxtSup_Are.Enabled = False
            TxtSup_Centiare.Enabled = False
            Cmb_TitoloPossesso.Enabled = False
            Txt_SupCondotta.Enabled = False
            TxtValiditaInizio.Enabled = False
            TxtValiditaFine.Enabled = False

            Cmb_Macrousi.Enabled = False
            Txt_SupMacrouso.Enabled = False
            TxtValiditaInizioMacrouso.Enabled = False
            TxtValiditaFineMacrouso.Enabled = False
            TxtValiditaFine.Enabled = False

            Cmb_Zone.Enabled = False
            Txt_SupZona.Enabled = False
            TxtPorzione.Enabled = False
            TxtSupClass.Enabled = False
            Cmb_QualitaCatasto.Enabled = False
            TxtClasseCatasto.Enabled = False
            TxtRedditoDom.Enabled = False
            TxtRedditoAgr.Enabled = False

            btn_aggiungi_possesso.Visible = False
            btn_aggiungi_macrousi.Visible = False
            btn_aggiungi_zonizzazione.Visible = False

            btn_aggiungi_classamento.Visible = False

        ElseIf Operazione = enum_TipoOperazioneDB.Modifica Then

            Dim p = objParametriAgenda.Particelle(0)
            Dim objParticellaR As New AgronicaCoreAnagrafeBIZ.Particella_R
            Dim ModificaChiave = objParticellaR.CheckParticellaxModificaCancellazione(False,
                p.Provincia, p.Comune, p.Sezione, p.Foglio, p.Numero, p.Subalterno, objParametri_Server)

            Cmb_Provincia.Enabled = ModificaChiave
            Cmb_Comune.Enabled = ModificaChiave
            Txt_Sezione.Enabled = ModificaChiave
            Txt_Foglio.Enabled = ModificaChiave
            Txt_Numero.Enabled = ModificaChiave
            Txt_Subalterno.Enabled = ModificaChiave
            chk_estero.Enabled = False

        End If

    End Sub



    Private Sub RipristinaDatiNeiControlli()

        ' sicuramente c'è il dato della particella
        'LblRiferimenti.Text = objParametriAgenda.Piva & " " & objParametriAgenda.Sa_Cod






        Dim objImpresaR As New AgronicaCoreAnagrafeDAL.Imprese_Read
        'Lbl_Impresa.Text = objImpresaR.RagSoc_from_Piva(xPiva, objParametri_Server)

        'Centro Aziendale
        Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Lbl_CentroAziendale.Text = objCentriAz.SaNome_from_SaCod(xPiva, xSa_Cod, objParametri_Server)

        Dim Errore As String

        '#####################################################################################
        '#####################       POSSESSI        #########################################
        '#####################################################################################

        'Carico possesso
        AgronicaCoreUtility.CaricaListControl.TitoloPossesso(CType(Me.Cmb_TitoloPossesso, ListControl),
                                                             False, "", "",
                                                             "", "", objParametri_Server)

        'Imposto il POSSESSO come default
        Me.Cmb_TitoloPossesso.SelectedIndex =
                 Me.Cmb_TitoloPossesso.Items.IndexOf(
                     Me.Cmb_TitoloPossesso.Items.FindByValue(
                         "1"))


        CaricaGriglia_Possesso(True)


        '#####################################################################################
        '#####################       MACROUSI        #########################################
        '#####################################################################################

        AgronicaCoreUtility.CaricaListControl.Macrouso(Cmb_Macrousi, True, AgronicaAgenda_2010.Seleziona.ToUpper(), "0", "", "", objParametri_Server)

        CaricaGriglia_Macrousi(True)


        '#####################################################################################
        '#####################       ZONE            #########################################
        '#####################################################################################

        AgronicaCoreUtility.CaricaListControl.Zone(Cmb_Zone, True, AgronicaAgenda_2010.Seleziona.ToUpper(), "0", "", "", objParametri_Server)

        CaricaGriglia_Zone(True)


        '#####################################################################################
        '#####################       CLASSAMENTO     #########################################
        '#####################################################################################

        AgronicaCoreUtility.CaricaListControl.QualitaCatasto(CType(Cmb_QualitaCatasto, ListControl),
                                                             True, AgronicaAgenda_2010.Seleziona.ToUpper(), "",
                                                             "", "", objParametri_Server)


        CaricaGriglia_Classamento(True)



        '----- Suggerisco le date ...
        objImpresaR.Recupera_DateValidita_ElementiGerarchia(
                                     enum_GerarchiaImpresa_Elementi.Gerarchia_Centro,
                                     TxtValiditaInizio.Text,
                                     TxtValiditaFine.Text,
                                     Errore,
                                     xPiva,
                                     xSa_Cod, 0, 0, 0, objParametri_Server)

        InizioCentro.Value = TxtValiditaInizio.Text
        FineCentro.Value = TxtValiditaFine.Text


        TxtSupClass.Text = 0
        TxtClasseCatasto.Text = 0
        TxtRedditoDom.Text = 0
        TxtRedditoAgr.Text = 0


        ' DA DECOMMENTARE!

        ''@Paolo: Cerco l'azienda di riferimento
        'Dim DTCentro As DataTable
        'DTCentro = objCentriAz.Leggi_x_anagrafica(1, CStr(xPiva), CStr(xSa_Cod), "", "", objParametri_Server)


        '@Paolo: leggo le date di riferimento
        'If DTCentro.Rows.Count > 0 Then
        '    lbl_centro_az_data_inizio.Text = DTCentro.Rows(0).Item("Validita_Inizio")
        '    lbl_centro_az_data_fine.Text = DTCentro.Rows(0).Item("Validita_Fine")
        'End If



        If Not IsPostBack Then

            TxtValiditaInizio.Text = ""
            TxtValiditaFine.Text = ""

            TxtClasseCatasto.Text = ""
            TxtRedditoDom.Text = ""
            TxtRedditoAgr.Text = ""
            TxtPorzione.Text = ""
            TxtSupClass.Text = ""

            TxtSup_Ettari.Text = ""
            TxtSup_Are.Text = ""
            TxtSup_Centiare.Text = ""

            Txt_SupZona.Text = ""

            Txt_SupMacrouso.Text = ""
            TxtValiditaInizioMacrouso.Text = ""
            TxtValiditaFineMacrouso.Text = ""

            Me.Txt_SupCondotta.Text = ""

        Else

            Exit Sub
        End If



        If Operazione = enum_TipoOperazioneDB.Scrittura Then

            'NOTA
            'Se sono in fase di inserimento, 
            'suggerisco la provincia e il comune,
            'prendendoli dall'indirizzo del centro aziendale

            Dim CodiceIstat_Provincia As String
            Dim CodiceIstat_Comune As String
            Dim Provincia_Sigla As String

            ' su salva e continua usa dati in session
            If Not String.IsNullOrEmpty(HttpContext.Current.Session("default_particella")) Then
                CodiceIstat_Provincia = HttpContext.Current.Session("provincia_particella")
                CodiceIstat_Comune = HttpContext.Current.Session("comune_particella")
                Dim objIstatR As New AgronicaCoreMetaSchemaDAL.Istat_R
                Dim DescProv = objIstatR.Provincia_from_CodIstat(CodiceIstat_Provincia, Provincia_Sigla, objParametri_Server)
                If HttpContext.Current.Session("default_particella") = "2" Then
                    Txt_Sezione.Text = HttpContext.Current.Session("sezione_particella")
                    Txt_Foglio.Text = HttpContext.Current.Session("foglio_particella")
                End If
                HttpContext.Current.Session("default_particella") = ""
            Else
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
            End If

            Province_and_Comuni(Cmb_Provincia,
                                Cmb_Comune,
                                Provincia_Sigla,
                                CodiceIstat_Comune,
                                True,
                                1, objParametri_Server)

            TxtCodProvincia.Text = CodiceIstat_Provincia
            TxtCodComune.Text = CodiceIstat_Comune

            'Setto il check Particella Estera
            If Cmb_Provincia.SelectedValue = "00" Then
                chk_estero.Checked = True
            Else
                chk_estero.Checked = False
            End If

            chk_estero.Enabled = False

        ElseIf Operazione = enum_TipoOperazioneDB.Modifica Or Operazione = enum_TipoOperazioneDB.Lettura Then
            tipo_salva.Value = 0

            'LblRiferimenti.Text &= " " & objParametriAgenda.Particelle(0).Part_Cod

            'Imposto le combo di provincia e comune
            Dim objIstatR As New AgronicaCoreMetaSchemaDAL.Istat_R
            Dim xSiglaProvincia As String
            Dim DescProv As String
            DescProv = objIstatR.Provincia_from_CodIstat(objParametriAgenda.Particelle(0).Provincia, xSiglaProvincia, objParametri_Server)

            Province_and_Comuni(Cmb_Provincia,
                                Cmb_Comune,
                                xSiglaProvincia,
                                objParametriAgenda.Particelle(0).Comune,
                                True,
                                1, objParametri_Server)

            TxtCodProvincia.Text = objParametriAgenda.Particelle(0).Provincia
            TxtCodComune.Text = objParametriAgenda.Particelle(0).Comune

            'Setto il check Particella Estera
            If Cmb_Provincia.SelectedValue = "00" Then
                chk_estero.Checked = True
            Else
                chk_estero.Checked = False
            End If


            If objParametriAgenda.Particelle(0).Sezione = "0" Then
                Txt_Sezione.Text = ""
            Else
                Txt_Sezione.Text = objParametriAgenda.Particelle(0).Sezione
            End If

            Txt_Foglio.Text = objParametriAgenda.Particelle(0).Foglio
            Txt_Numero.Text = objParametriAgenda.Particelle(0).Numero

            If objParametriAgenda.Particelle(0).Subalterno = "0" Then
                Txt_Subalterno.Text = ""
            Else
                Txt_Subalterno.Text = objParametriAgenda.Particelle(0).Subalterno
            End If

            TxtSup_Ettari.Text = 0
            TxtSup_Are.Text = 0
            TxtSup_Centiare.Text = 0

            Dim objParticella As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
            objParticella.Recupera_Superfici_Particella(objParametriAgenda.Particelle(0).Provincia,
                                                        objParametriAgenda.Particelle(0).Comune,
                                                        objParametriAgenda.Particelle(0).Sezione,
                                                        objParametriAgenda.Particelle(0).Foglio,
                                                        objParametriAgenda.Particelle(0).Numero,
                                                        objParametriAgenda.Particelle(0).Subalterno,
                                                        TxtSup_Ettari.Text,
                                                        TxtSup_Are.Text,
                                                        TxtSup_Centiare.Text,
                                                        objParametri_Server)


            ' mi serve salvare nel viewstate per il salvataggio
            ViewState("provincia") = objParametriAgenda.Particelle(0).Provincia
            ViewState("comune") = objParametriAgenda.Particelle(0).Comune
            ViewState("sezione") = objParametriAgenda.Particelle(0).Sezione
            ViewState("foglio") = objParametriAgenda.Particelle(0).Foglio
            ViewState("numero") = objParametriAgenda.Particelle(0).Numero
            ViewState("subalterno") = objParametriAgenda.Particelle(0).Subalterno


            CaricaGriglia_Possesso(False,
                                    xPiva,
                                    xSa_Cod,
                                    objParametriAgenda.Particelle(0).Provincia,
                                    objParametriAgenda.Particelle(0).Comune,
                                    objParametriAgenda.Particelle(0).Sezione,
                                    objParametriAgenda.Particelle(0).Foglio,
                                    objParametriAgenda.Particelle(0).Numero,
                                    objParametriAgenda.Particelle(0).Subalterno)


            CaricaGriglia_Macrousi(False,
                                    objParametriAgenda.Particelle(0).Provincia,
                                    objParametriAgenda.Particelle(0).Comune,
                                    objParametriAgenda.Particelle(0).Sezione,
                                    objParametriAgenda.Particelle(0).Foglio,
                                    objParametriAgenda.Particelle(0).Numero,
                                    objParametriAgenda.Particelle(0).Subalterno)



            CaricaGriglia_Zone(False,
                               objParametriAgenda.Particelle(0).Provincia,
                               objParametriAgenda.Particelle(0).Comune,
                               objParametriAgenda.Particelle(0).Sezione,
                               objParametriAgenda.Particelle(0).Foglio,
                               objParametriAgenda.Particelle(0).Numero,
                               objParametriAgenda.Particelle(0).Subalterno)


            CaricaGriglia_Classamento(False,
                                      objParametriAgenda.Particelle(0).Provincia,
                                      objParametriAgenda.Particelle(0).Comune,
                                      objParametriAgenda.Particelle(0).Sezione,
                                      objParametriAgenda.Particelle(0).Foglio,
                                      objParametriAgenda.Particelle(0).Numero,
                                      objParametriAgenda.Particelle(0).Subalterno)


        End If

    End Sub


    '##################################################################################################
    Private Sub Cmb_Provincia_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Provincia.SelectedIndexChanged

        Dim Testo As String

        'Ricarico la combo dei comuni con il filtro sulla provincia
        AgronicaCoreUtility.CaricaListControl.Comuni(CType(Cmb_Comune, ListControl),
                                                     True, "", "",
                                                     Cmb_Provincia.SelectedItem.Value, True, 1,
                                                     "", "", objParametri_Server)

        'Inserisco il codice nella TextBox
        '---------------------------
        ' ComboBox : Provincia
        ' SIGLA(2)
        ' FC
        '---------------------------
        Testo = Cmb_Provincia.SelectedItem.Value
        Dim objIstR As New AgronicaCoreMetaSchemaDAL.Istat_R
        Testo = objIstR.CodIstat_from_Provincia(Testo, objParametri_Server)

        TxtCodProvincia.Text = Testo
        TxtCodComune.Text = ""

    End Sub

    '##################################################################################################
    Private Sub Cmb_Comune_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Comune.SelectedIndexChanged

        Dim Indice As Integer
        Dim Provincia As String

        TxtCodComune.Text = Mid(Cmb_Comune.SelectedItem.Value, 4, 3)

    End Sub



    '########################################################################################
    Private Sub CaricaGriglia_Possesso(ByVal Inizializza As Boolean,
                                       Optional ByVal Piva As String = "",
                                       Optional ByVal Sa_Cod As Integer = 0,
                                       Optional ByVal Prov As String = "",
                                       Optional ByVal Com As String = "",
                                       Optional ByVal Sezione As String = "",
                                       Optional ByVal Foglio As Integer = 0,
                                       Optional ByVal Numero As Integer = 0,
                                       Optional ByVal Subalterno As String = "")

        '----- Definizione delle variabili

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim Contatore As Integer

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Contatore", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("TitoloPossesso_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("TitoloPossesso_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Condotta", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cod_Particella", GetType(String)))


        '----- Recupero l'elenco delle particelle

        Dim objCOM As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim DTRS As DataTable

        If Inizializza = True Then
            DTRS = Nothing
        Else
            '   *   CreateCANCELLATOObject("Agro_Anagrafe_AD.ImpresexParticelle2_R")
            DTRS = objCOM.Leggi(
                             0,
                            CStr(Piva),
                            CInt(Sa_Cod),
                            0,
                            CStr(Prov),
                            CStr(Com),
                            CStr(Sezione),
                            CInt(Foglio),
                            CInt(Numero),
                            CStr(Subalterno),
                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "",
                             "", objParametri_Server)
            objCOM = Nothing
        End If

        '----- Carico la griglia

        'Se il recordset esiste ...
        If (Not IsNothing(DTRS)) AndAlso (DTRS.Rows.Count > 0) Then

            Dim objAnagrafeParCodDAL = New AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_R

            Contatore = 1
            Dim i As Integer
            Dim objTitoloPossessoR As New AgronicaCoreMetaSchemaDAL.Titolo_Possesso_R
            For i = 0 To DTRS.Rows.Count - 1

                'Creo una nuova riga
                Dr = Dt.NewRow

                'Definisco i valori

                Dr.Item("Contatore") = Contatore
                Contatore = Contatore + 1

                Dr.Item("TitoloPossesso_Des") = objTitoloPossessoR.TitoloPossessoDes_from_TitoloPossessoCod(DTRS.Rows(i).Item("xTitoloPossesso"))
                Dr.Item("TitoloPossesso_Cod") = DTRS.Rows(i).Item("xTitoloPossesso")

                If (CDate(DTRS.Rows(i).Item("xValidita_Inizio"))) = #1/1/1900# Then
                    Dr.Item("Validita_Inizio") = "..."
                Else
                    Dr.Item("Validita_Inizio") = CDate(DTRS.Rows(i).Item("xValidita_Inizio")).ToShortDateString
                End If

                If (CDate(DTRS.Rows(i).Item("xValidita_Fine"))) = #12/31/2100# Then
                    Dr.Item("Validita_Fine") = "..."
                Else
                    Dr.Item("Validita_Fine") = CDate(DTRS.Rows(i).Item("xValidita_Fine")).ToShortDateString
                End If

                Dr.Item("Sup_Condotta") = DTRS.Rows(i).Item("Sup_Condotta")

                Dim DtCodice = objAnagrafeParCodDAL.Leggi(
                    DTRS.Rows(i).Item("ID"), "", 0, "", "", "", 0, 0, "", enum_CodiciAnagrafe.CodiceParticella, "",
                    enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                If DtCodice.Rows.Count > 0 Then
                    Dr.Item("Cod_Particella") = DtCodice.Rows(0).Item("Val_Cod")
                End If

                'Associo alla tabella la nuova riga creata
                Dt.Rows.Add(Dr)

            Next





        End If

        HttpContext.Current.Session("dt_Possessi") = Dt

        '----- Associo il DataTable con il DataGrid
        Aggiorna_Griglia_Possessi(Dt)


    End Sub

    '#############################################################################################################################################################
    'Private Sub GridView_Possessi_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView_Possessi.RowCommand

    '    Dim IndiceRigaGriglia As Integer

    '    IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

    '    Select Case e.CommandName

    '        Case "Elimina"

    '            DataGrid_Possesso_Elimina(IndiceRigaGriglia)

    '    End Select
    'End Sub


    '#############################################################################################################################################################
    'Private Sub DataGrid_Possesso_Elimina(ByVal IndiceRigaGriglia As Integer)

    '    Dim Contatore As Integer
    '    Dim Dt As DataTable
    '    Dim Dr As DataRow

    '    'Recupero il codice da cancellare
    '    Contatore = GridView_Possessi.DataKeys(IndiceRigaGriglia).Item(0)

    '    'Recupero il datatable
    '    Dt = ViewState("vs_dtPossesso")

    '    Dim i As Integer
    '    For i = 0 To Dt.Rows.Count - 1
    '        If Dt.Rows(i).Item("Contatore") = Contatore Then
    '            Dr = Dt.Rows(i)
    '            Exit For
    '        End If
    '    Next

    '    'Elimino la riga
    '    Dr.Delete()

    '    Aggiorna_Griglia_Possessi(Dt)


    'End Sub


    '#####################################################################################################################################################
    'Private Sub ImgBtn_Inserisci_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Inserisci.Click

    '    Possesso_Inserisci()

    'End Sub

    '#####################################################################################################################################################
    Private Sub Possesso_Inserisci()


        Dim TitoloPossesso_Cod As Integer
        Dim TitoloPossesso_Des As String
        Dim DataInizio As Date
        Dim DataFine As Date
        Dim i As Integer
        Dim Errore As Boolean = False
        Dim xDataInizio As Date
        Dim xDataFine As Date
        Dim Dt As DataTable
        Dim Dr As DataRow
        Dim IndiceRiga As Integer
        Dim Contatore As Integer
        Dim MessaggioErrore As String
        Dim Sup_Particella As Double = 0
        Dim Sup_Condotta As Double = 0
        Dim Ettari As Double = 0
        Dim Are As Double = 0
        Dim Centiare As Double = 0
        Dim Cod_Particella As String = ""

        '----------------------------------------
        '----------------------------------------
        '----- Verifico le informazioni
        '----------------------------------------
        '----------------------------------------

        If Not IsDate(TxtValiditaInizio.Text) Then
            DataInizio = AGRODATAINIZIO
        Else
            DataInizio = CDate(TxtValiditaInizio.Text)
        End If

        If Not IsDate(TxtValiditaFine.Text) Then
            DataFine = AGRODATAFINE
        Else
            DataFine = CDate(TxtValiditaFine.Text)
        End If

        TitoloPossesso_Cod = Cmb_TitoloPossesso.SelectedValue
        TitoloPossesso_Des = Cmb_TitoloPossesso.SelectedItem.Text

        '----------------------------------------
        '----- Verifico che la data inizio non sia posteriore alla data fine
        If DataInizio > DataFine Then

            'Messaggio di errore
            Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("DataValiditàFinePossessoParticellaPrecedenteInizio"), String), Page, , UpdatePanel_script, , True)

            'Esco dalla subroutine
            Exit Sub

        End If


        '----------------------------------------
        '----- Verifico la Sup_Condotta

        If Me.TxtSup_Ettari.Text <> "" Then
            If IsNumeric(Me.TxtSup_Ettari.Text) Then
                Ettari = CDbl(Me.TxtSup_Ettari.Text)
            End If
        End If
        If Me.TxtSup_Are.Text <> "" Then
            If IsNumeric(Me.TxtSup_Are.Text) Then
                Are = CDbl(Me.TxtSup_Are.Text)
            End If
        End If
        If Me.TxtSup_Centiare.Text <> "" Then
            If IsNumeric(Me.TxtSup_Centiare.Text) Then
                Centiare = CDbl(Me.TxtSup_Centiare.Text)
            End If
        End If

        Sup_Particella = Ettari_from_EttariAreCentiare(Ettari, Are, Centiare)

        If Me.Txt_SupCondotta.Text <> "" Then
            If InStr(Txt_SupCondotta.Text, ".") <> 0 Then
                Txt_SupCondotta.Text = Replace(Txt_SupCondotta.Text, ".", ",")
            End If
            If IsNumeric(Me.Txt_SupCondotta.Text) Then
                Sup_Condotta = CDbl(Me.Txt_SupCondotta.Text)
            End If
        End If

        'Se la sup_condotta non è stata inserita 
        'o è stata inserita maggiore di quella della particella
        'viene inserita quella della particella
        If Sup_Particella <> 0 Then
            If Sup_Condotta = 0 Then
                Sup_Condotta = Sup_Particella
            Else
                If Sup_Condotta > Sup_Particella Then
                    Sup_Condotta = Sup_Particella
                End If
            End If
        Else
            Sup_Condotta = 0
        End If

        Cod_Particella = Me.Txt_CodParticella.Text

        '----- Recupero il datatable

        If Not IsNothing(ViewState("vs_dtPossesso")) Then
            'Recupero il datatable dal ViewState
            Dt = ViewState("vs_dtPossesso")
        Else
            Dt = New DataTable
        End If

        '----- Cerco il valore massimo del contatore

        Contatore = 0

        For i = 0 To Dt.Rows.Count - 1
            If Dt.Rows(i).Item("Contatore") > Contatore Then
                Contatore = Dt.Rows(i).Item("Contatore")
            End If
        Next

        Contatore = Contatore + 1

        '----- Inserisco la riga nel datagrid

        'Creo una nuova riga
        Dr = Dt.NewRow

        'Definisco i valori
        Dr.Item("Contatore") = Contatore

        Dr.Item("TitoloPossesso_Des") = TitoloPossesso_Des
        Dr.Item("TitoloPossesso_Cod") = TitoloPossesso_Cod

        If DataInizio = #1/1/1900# Then
            Dr.Item("Validita_Inizio") = "..."
        Else
            Dr.Item("Validita_Inizio") = DataInizio.ToShortDateString
        End If

        If DataFine = #12/31/2100# Then
            Dr.Item("Validita_Fine") = "..."
        Else
            Dr.Item("Validita_Fine") = DataFine.ToShortDateString
        End If

        Dr.Item("Sup_Condotta") = Sup_Condotta

        Dr.Item("Cod_Particella") = Cod_Particella


        'Associo alla tabella la nuova riga creata
        Dt.Rows.Add(Dr)

        '----- Associo il DataTable con il DataGrid
        Aggiorna_Griglia_Possessi(Dt)


        '----- Azzero i controlli di provenienza dei dati

        Me.TxtValiditaInizio.Text = ""
        Me.TxtValiditaFine.Text = ""
        Me.Txt_SupCondotta.Text = ""

    End Sub

    Private Sub Aggiorna_Griglia_Possessi(ByVal Dt As DataTable)

        ' Chiavi per recuperare le righe
        Dim DtKeys(5) As String
        DtKeys(0) = "Contatore"
        DtKeys(1) = "TitoloPossesso_Cod"
        DtKeys(2) = "Validita_Inizio"
        DtKeys(3) = "Validita_Fine"
        DtKeys(4) = "Sup_Condotta"
        DtKeys(5) = "Cod_Particella"

        jsPossessi = DT_to_Json_Possessi(Dt)
        HttpContext.Current.Session("dt_Possessi") = Dt


        '----- Associo il DataTable con la DataGrid
        'GridView_Possessi.DataSource = Dt
        'GridView_Possessi.DataKeyNames = DtKeys
        'GridView_Possessi.DataBind()

        '----- Salvo il DataTable dentro il viewstate
        ViewState("vs_dtPossesso") = Dt

    End Sub



    '########################################################################################
    Private Sub CaricaGriglia_Macrousi(ByVal Inizializza As Boolean,
                                       Optional ByVal Prov As String = "",
                                       Optional ByVal Com As String = "",
                                       Optional ByVal Sezione As String = "",
                                       Optional ByVal Foglio As Integer = 0,
                                       Optional ByVal Numero As Integer = 0,
                                       Optional ByVal Subalterno As String = "")

        '----- Definizione delle variabili

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim DTMacrousi As DataTable
        Dim Contatore As Integer = 1

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("Contatore", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Macrouso_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Superficie", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        '----------------------------------------------
        '----- Tabella ZonexParticelle
        '----------------------------------------------

        If Inizializza = True Then
            DTMacrousi = Nothing
        Else
            Dim objPartCatxMacro As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R
            DTMacrousi = objPartCatxMacro.Leggi(xPiva,
                                                CStr(Prov),
                                                CStr(Com),
                                                CStr(Sezione),
                                                CInt(Foglio),
                                                CInt(Numero),
                                                CStr(Subalterno),
                                                "",
                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                "", "", objParametri_Server)

            If Not DTMacrousi Is Nothing AndAlso DTMacrousi.Rows.Count > 0 Then
                Dim i As Integer
                For i = 0 To DTMacrousi.Rows.Count - 1

                    'Creo una nuova riga
                    Dr = Dt.NewRow

                    Dr.Item("Contatore") = Contatore
                    Contatore = Contatore + 1

                    Dr.Item("Macrouso_Cod") = DTMacrousi.Rows(i).Item("Macrouso_Cod")
                    Dr.Item("Descrizione") = DTMacrousi.Rows(i).Item("Macrouso_Des")
                    Dr.Item("Superficie") = DTMacrousi.Rows(i).Item("Superficie")

                    If (CDate(DTMacrousi.Rows(i).Item("Validita_Inizio"))) = #1/1/1900# Then
                        Dr.Item("Validita_Inizio") = "..."
                    Else
                        Dr.Item("Validita_Inizio") = CDate(DTMacrousi.Rows(i).Item("Validita_Inizio")).ToShortDateString
                    End If

                    If (CDate(DTMacrousi.Rows(i).Item("Validita_Fine"))) = #12/31/2100# Then
                        Dr.Item("Validita_Fine") = "..."
                    Else
                        Dr.Item("Validita_Fine") = CDate(DTMacrousi.Rows(i).Item("Validita_Fine")).ToShortDateString
                    End If
                    'Associo alla tabella la nuova riga creata
                    Dt.Rows.Add(Dr)
                Next

            End If

            HttpContext.Current.Session("dt_Macrousi") = Dt



        End If
        '----- Associo il DataTable con la DataGrid
        Aggiorna_Griglia_Macrousi(Dt)


    End Sub

    '########################################################################################
    Private Sub Aggiorna_Griglia_Macrousi(ByVal Dt As DataTable)

        ' Chiavi per recuperare le righe
        Dim DtKeys(4) As String
        DtKeys(0) = "Contatore"
        DtKeys(1) = "Macrouso_Cod"
        DtKeys(2) = "Validita_Inizio"
        DtKeys(3) = "Validita_Fine"
        DtKeys(4) = "Superficie"

        jsMacrousi = DT_to_Json_Macrousi(Dt)
        HttpContext.Current.Session("dt_Macrousi") = Dt

        '----- Associo il DataTable con la DataGrid
        'GridView_Macrousi.DataSource = Dt
        'GridView_Macrousi.DataKeyNames = DtKeys
        'GridView_Macrousi.DataBind()

        '----- Salvo il DataTable dentro il viewstate
        ViewState("vs_dtMacrousi") = Dt

    End Sub

    ''#######################################################################################################################################################
    'Private Sub GridView_Macrousi_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView_Macrousi.RowCommand

    '    Dim IndiceRigaGriglia As Integer

    '    IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

    '    Select Case e.CommandName

    '        Case "Elimina"

    '            Datagrid_Macrousi_Elimina(IndiceRigaGriglia)

    '    End Select
    'End Sub


    '#############################################################################################################################################################
    Private Sub Datagrid_Macrousi_Elimina(ByVal IndiceRigaGriglia As Integer)

        Dim Contatore As Integer
        Dim Dt As DataTable
        Dim Dr As DataRow

        'Recupero il codice da cancellare
        'Contatore = GridView_Macrousi.DataKeys(IndiceRigaGriglia).Item(0)

        'Recupero il datatable
        Dt = ViewState("vs_dtMacrousi")

        Dim i As Integer
        For i = 0 To Dt.Rows.Count - 1
            If Dt.Rows(i).Item("Contatore") = Contatore Then
                Dr = Dt.Rows(i)
            End If
        Next

        'Elimino la riga
        Dr.Delete()

        Aggiorna_Griglia_Macrousi(Dt)


    End Sub


    '#########################################################################################################################################
    'Private Sub ImgBtn_InserisciMacrouso_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_InserisciMacrouso.Click

    '    Macrouso_Inserisci()

    'End Sub

    '##############################################################################################################
    Private Sub Macrouso_Inserisci()

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim DrTmp As DataRow
        Dim MessaggioErrore As String
        Dim Sup As Double = 0
        Dim DataInizio As Date
        Dim DataFine As Date
        Dim xDataInizio As Date
        Dim xDataFine As Date
        Dim i As Integer
        Dim Errore As Boolean = False
        Dim Contatore As Integer

        '------------------------------------------------------
        '--- CONTROLLI
        '------------------------------------------------------

        If Not IsDate(Me.TxtValiditaInizioMacrouso.Text) Then
            DataInizio = AGRODATAINIZIO
        Else
            DataInizio = CDate(TxtValiditaInizioMacrouso.Text)
        End If

        If Not IsDate(Me.TxtValiditaFineMacrouso.Text) Then
            DataFine = AGRODATAFINE
        Else
            DataFine = CDate(TxtValiditaFineMacrouso.Text)
        End If


        If Me.Cmb_Macrousi.SelectedItem.Text = "" Then
            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IndicareIlMacrouso"), String) & vbCrLf
        End If

        If Me.Txt_SupMacrouso.Text <> "" Then
            If InStr(Txt_SupMacrouso.Text, ".") <> 0 Then
                Txt_SupMacrouso.Text = Replace(Txt_SupMacrouso.Text, ".", ",")
            End If
            If Not IsNumeric(Txt_SupMacrouso.Text) Then
                MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IndicareLaSuperficieConUnNumero"), String) & vbCrLf
            Else
                Sup = CDbl(Txt_SupMacrouso.Text)
            End If
        End If


        '--- RIEPILOGO

        Dim Messaggio As String

        If MessaggioErrore <> "" Then

            Messaggio = ""
            Messaggio += AgronicaAgenda_2010.SonoStatiRilevatiISeguentiErrori_ & vbCrLf
            Messaggio += "" & vbCrLf
            Messaggio += MessaggioErrore
            Messaggio += "" & vbCrLf
            Messaggio += AgronicaAgenda_2010.RitentareIlSalvataggioDopoLaCorrezione

            Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanel_script, , True)
            Exit Sub

        End If

        '----- Recupero il datatable

        If Not IsNothing(ViewState("vs_dtMacrousi")) Then
            'Recupero il datatable dal ViewState
            Dt = ViewState("vs_dtMacrousi")
        Else
            Dt = New DataTable
        End If

        'verifico che il macrouso non sia già presente
        'DrTmp = Dt.Rows.Find(Me.Cmb_Macrousi.SelectedValue)

        If Cmb_Macrousi.SelectedIndex > 0 Then
            For i = 0 To Dt.Rows.Count - 1
                'If Cmb_Macrousi.SelectedItem.Value = GridView_Macrousi.DataKeys(i).Item(1) Then
                '    DrTmp = Dt.Rows(i)
                '    Exit For
                'End If
            Next
        End If


        '----- Cerco il valore massimo del contatore

        Contatore = 0

        For i = 0 To Dt.Rows.Count - 1
            If Dt.Rows(i).Item("Contatore") > Contatore Then
                Contatore = Dt.Rows(i).Item("Contatore")
            End If
        Next

        Contatore = Contatore + 1

        If DrTmp Is Nothing Then


            '----- Inserisco la riga nel datagrid

            'Creo una nuova riga
            Dr = Dt.NewRow

            'Definisco i valori
            'Definisco i valori
            Dr.Item("Contatore") = Contatore

            If DataInizio = #1/1/1900# Then
                Dr.Item("Validita_Inizio") = "..."
            Else
                Dr.Item("Validita_Inizio") = DataInizio.ToShortDateString
            End If

            If DataFine = #12/31/2100# Then
                Dr.Item("Validita_Fine") = "..."
            Else
                Dr.Item("Validita_Fine") = DataFine.ToShortDateString
            End If


            Dr.Item("Macrouso_Cod") = CStr(Me.Cmb_Macrousi.SelectedValue)
            Dr.Item("Descrizione") = Me.Cmb_Macrousi.SelectedItem.Text
            Dr.Item("Superficie") = Sup

            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)

            '----- Associo il DataTable con il DataGrid
            Aggiorna_Griglia_Macrousi(Dt)

            '----- Azzero i controlli di provenienza dei dati
            Txt_SupMacrouso.Text = ""
            Cmb_Macrousi.SelectedIndex = -1
            TxtValiditaInizioMacrouso.Text = ""
            TxtValiditaFineMacrouso.Text = ""

        Else

            Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("MacrousoGiàPresente"), String), Page, , UpdatePanel_script, , True)

        End If

    End Sub


    '########################################################################################
    Private Sub CaricaGriglia_Zone(ByVal Inizializza As Boolean,
                                    Optional ByVal Prov As String = "",
                                    Optional ByVal Com As String = "",
                                    Optional ByVal Sezione As String = "",
                                    Optional ByVal Foglio As Integer = 0,
                                    Optional ByVal Numero As Integer = 0,
                                    Optional ByVal Subalterno As String = "")

        '----- Definizione delle variabili

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim DTZone As DataTable

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("Zona_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Area", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))


        '----------------------------------------------
        '----- Tabella ZonexParticelle
        '----------------------------------------------

        If Inizializza = True Then
            DTZone = Nothing
        Else
            Dim objZonepart As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
            DTZone = objZonepart.Leggi(0,
                                       CStr(Prov),
                                       CStr(Com),
                                       CStr(Sezione),
                                       CInt(Foglio),
                                       CInt(Numero),
                                       CStr(Subalterno),
                                       AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                       "", "", objParametri_Server)

            If Not DTZone Is Nothing AndAlso DTZone.Rows.Count > 0 Then
                Dim i As Integer
                For i = 0 To DTZone.Rows.Count - 1

                    'Creo una nuova riga
                    Dr = Dt.NewRow

                    'Definisco i valori
                    Dr.Item("Zona_Cod") = DTZone.Rows(i).Item("Zona_Cod")
                    Dr.Item("Descrizione") = DTZone.Rows(i).Item("Descrizione")
                    Dr.Item("Area") = DTZone.Rows(i).Item("Area")
                    Dr.Item("Validita_Inizio") = DTZone.Rows(i).Item("Validita_Inizio")
                    Dr.Item("Validita_Fine") = DTZone.Rows(i).Item("Validita_Fine")

                    'Associo alla tabella la nuova riga creata
                    Dt.Rows.Add(Dr)
                Next

            End If

            HttpContext.Current.Session("dt_Zone") = Dt


        End If
        '----- Associo il DataTable con la DataGrid
        Aggiorna_Griglia_Zone(Dt)



    End Sub

    '########################################################################################
    Private Sub Aggiorna_Griglia_Zone(ByVal Dt As DataTable)

        ' Chiavi per recuperare le righe
        Dim DtKeys(1) As String
        DtKeys(0) = "Zona_Cod"
        DtKeys(1) = "Area"

        jsZone = DT_to_Json_Zone(Dt)
        HttpContext.Current.Session("dt_Zone") = Dt


        '----- Associo il DataTable con la DataGrid
        'GridView_Zone.DataSource = Dt
        'GridView_Zone.DataKeyNames = DtKeys
        'GridView_Zone.DataBind()

        '----- Salvo il DataTable dentro il viewstate
        ViewState("vs_dtZone") = Dt

    End Sub

    '###############################################################################################################################################
    'Private Sub GridView_Zone_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView_Zone.RowCommand

    '    Dim IndiceRigaGriglia As Integer

    '    IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

    '    Select Case e.CommandName

    '        Case "Elimina"

    '            DataGrid_Zone_Elimina(IndiceRigaGriglia)

    '    End Select

    'End Sub

    '#####################################################################################################################################################
    Private Sub DataGrid_Zone_Elimina(ByVal IndiceRigaGriglia As Integer)

        Dim Zona_Cod As Integer
        Dim Dt As DataTable
        Dim Dr As DataRow


        'Recupero il codice da cancellare
        'Zona_Cod = GridView_Zone.DataKeys(IndiceRigaGriglia).Item(0)

        'Recupero il datatable
        Dt = ViewState("vs_dtZone")

        Dim i As Integer
        For i = 0 To Dt.Rows.Count - 1
            If Dt.Rows(i).Item("Zona_Cod") = Zona_Cod Then
                Dr = Dt.Rows(i)
                Exit For
            End If
        Next

        'Elimino la riga
        Dr.Delete()

        Aggiorna_Griglia_Zone(Dt)

    End Sub

    '#####################################################################################################################################
    'Private Sub ImgBtn_InserisciZona_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_InserisciZona.Click

    '    Zona_Inserisci()

    'End Sub

    '#####################################################################################################################################
    Private Sub Zona_Inserisci()

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim DrTmp As DataRow
        Dim MessaggioErrore As String
        Dim Sup As Double = 0

        '------------------------------------------------------
        '--- CONTROLLI
        '------------------------------------------------------

        If Me.Cmb_Zone.SelectedItem.Text = "" Then
            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IndicareLaZona"), String) & vbCrLf
        End If

        If Me.Txt_SupZona.Text <> "" Then
            If InStr(Txt_SupZona.Text, ".") <> 0 Then
                Txt_SupZona.Text = Replace(Txt_SupZona.Text, ".", ",")
            End If
            If Not IsNumeric(Me.Txt_SupZona.Text) Then
                MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IndicareLaSuperficieConUnNumero"), String) & vbCrLf
            Else
                Sup = CDbl(Me.Txt_SupZona.Text)
            End If
        End If


        '--- RIEPILOGO

        Dim Messaggio As String

        If MessaggioErrore <> "" Then

            Messaggio = ""
            Messaggio += AgronicaAgenda_2010.SonoStatiRilevatiISeguentiErrori_ & vbCrLf
            Messaggio += "" & vbCrLf
            Messaggio += MessaggioErrore
            Messaggio += "" & vbCrLf
            Messaggio += AgronicaAgenda_2010.RitentareIlSalvataggioDopoLaCorrezione

            Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanel_script, , True)

            Exit Sub

        End If

        '----- Recupero il datatable

        If Not IsNothing(ViewState("vs_dtZone")) Then
            'Recupero il datatable dal ViewState
            Dt = ViewState("vs_dtZone")
        Else
            Dt = New DataTable
        End If

        'verifico che la zona non sia già presente
        ' DrTmp = Dt.Rows.Find(Me.Cmb_Zone.SelectedValue)

        'If Cmb_Zone.SelectedIndex > 0 Then
        '    For i = 0 To Dt.Rows.Count - 1
        '        If Cmb_Zone.SelectedItem.Value = GridView_Zone.DataKeys(i).Item(0) Then
        '            DrTmp = Dt.Rows(i)
        '        End If
        '    Next
        'End If

        If DrTmp Is Nothing Then

            '----- Inserisco la riga nel datagrid

            'Creo una nuova riga
            Dr = Dt.NewRow

            'Definisco i valori

            Dr.Item("Zona_Cod") = CInt(Me.Cmb_Zone.SelectedValue)
            Dr.Item("Descrizione") = Me.Cmb_Zone.SelectedItem.Text
            Dr.Item("Area") = Sup

            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)

            Aggiorna_Griglia_Zone(Dt)

            '----- Azzero i controlli di provenienza dei dati

            Me.Txt_SupZona.Text = ""
            Me.Cmb_Zone.SelectedIndex = -1

        Else

            Messaggi.AgroMsgBox(DirectCast(GetLocalResourceObject("ZonaGiàPresente"), String), Page, , UpdatePanel_script, , True)

        End If

    End Sub


    '########################################################################################
    Private Sub CaricaGriglia_Classamento(ByVal Inizializza As Boolean,
                                          Optional ByVal Prov As String = "",
                                          Optional ByVal Com As String = "",
                                          Optional ByVal Sezione As String = "",
                                          Optional ByVal Foglio As Integer = 0,
                                          Optional ByVal Numero As Integer = 0,
                                          Optional ByVal Subalterno As String = "")

        '----- Definizione delle variabili

        Dim Dt As New DataTable
        Dim Dr As DataRow

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("QUALITA_COD", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("QUALITA_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Porzione", GetType(String)))
        Dt.Columns.Add(New DataColumn("CLASSE", GetType(String)))
        Dt.Columns.Add(New DataColumn("Deduzione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Classe", GetType(String)))
        Dt.Columns.Add(New DataColumn("REDDITO_DOMINICALE", GetType(String)))
        Dt.Columns.Add(New DataColumn("REDDITO_AGRARIO", GetType(String)))


        '----- Recupero l'elenco delle particelle

        Dim objCOM As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R     'New Agro_Anagrafe_AD.ParticelleCatastali_R
        Dim DTRs As DataTable

        If Inizializza = True Then
            DTRs = Nothing
        Else
            DTRs = objCOM.LeggixChiave_conClassamento(CStr(Prov),
                                CStr(Com),
                                CStr(Sezione),
                                CInt(Foglio),
                                CInt(Numero),
                                CStr(Subalterno),
                                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                 "", "", objParametri_Server)

            objCOM = Nothing

        End If

        '----- Carico la griglia

        'Se il recordset esiste ...
        If (Not IsNothing(DTRs)) AndAlso (DTRs.Rows.Count > 0) Then
            Dim i As Integer
            For i = 0 To DTRs.Rows.Count - 1

                If DTRs.Rows(i).Item("QUALITA_COD") <> 0 Then

                    'Creo una nuova riga
                    Dr = Dt.NewRow

                    'Definisco i valori
                    Dr.Item("QUALITA_COD") = DTRs.Rows(i).Item("QUALITA_COD")
                    Dr.Item("QUALITA_Des") = DTRs.Rows(i).Item("QUALITA_Des")
                    Dr.Item("Porzione") = DTRs.Rows(i).Item("Porzione")
                    Dr.Item("CLASSE") = DTRs.Rows(i).Item("CLASSE")
                    Dr.Item("Deduzione") = DTRs.Rows(i).Item("Deduzione")
                    Dr.Item("Sup_Classe") = DTRs.Rows(i).Item("Sup_Classe")
                    Dr.Item("REDDITO_DOMINICALE") = DTRs.Rows(i).Item("REDDITO_DOMINICALE")
                    Dr.Item("REDDITO_AGRARIO") = DTRs.Rows(i).Item("REDDITO_AGRARIO")

                    'Associo alla tabella la nuova riga creata
                    Dt.Rows.Add(Dr)

                End If

            Next

            HttpContext.Current.Session("dt_Classamenti") = Dt



        End If
        '----- Associo il DataTable con la DataGrid
        Aggiorna_Griglia_Classamento(Dt)


    End Sub


    '########################################################################################
    Private Sub Aggiorna_Griglia_Classamento(ByVal Dt As DataTable)

        ' Chiavi per recuperare le righe
        Dim DtKeys(5) As String
        DtKeys(0) = "Qualita_Cod"
        DtKeys(1) = "Sup_Classe"
        DtKeys(2) = "Porzione"
        DtKeys(3) = "Classe"
        DtKeys(4) = "REDDITO_DOMINICALE"
        DtKeys(5) = "REDDITO_AGRARIO"


        '----- Associo il DataTable con la DataGrid
        'GridView_Classamento.DataSource = Dt
        'GridView_Classamento.DataKeyNames = DtKeys
        'GridView_Classamento.DataBind()

        jsClassamenti = DT_to_Json_Classamenti(Dt)
        HttpContext.Current.Session("dt_Classamenti") = Dt

        '----- Salvo il DataTable dentro il viewstate
        ViewState("vs_dtClassamento") = Dt

    End Sub

    '###########################################################################################################################################################
    'Private Sub GridView_Classamento_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView_Classamento.RowCommand

    '    Dim IndiceRigaGriglia As Integer

    '    IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

    '    Select Case e.CommandName

    '        Case "Elimina"

    '            DataGrid_Classamento_Elimina(IndiceRigaGriglia)

    '    End Select

    'End Sub



    '########################################################################################
    Private Sub DataGrid_Classamento_Elimina(ByVal IndiceRigaGriglia As Integer)

        Dim Qualita_Cod As Integer
        Dim Dt As DataTable
        Dim Dr As DataRow

        'Recupero il codice da cancellare
        'Qualita_Cod = GridView_Classamento.DataKeys(IndiceRigaGriglia).Item(0)

        'Recupero il datatable
        Dt = ViewState("vs_dtClassamento")

        Dim i As Integer
        For i = 0 To Dt.Rows.Count - 1
            If Dt.Rows(i).Item("Qualita_Cod") = Qualita_Cod Then
                Dr = Dt.Rows(i)
            End If
        Next

        'Elimino la riga
        Dr.Delete()

        Aggiorna_Griglia_Classamento(Dt)

    End Sub


    '#######################################################################################################################################
    'Private Sub ImgBtn_InserisciClasse_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_InserisciClasse.Click

    '    Classe_Inserisci()

    'End Sub

    '#####################################################################
    Private Sub Classe_Inserisci()

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim MessaggioErrore As String

        '----------------------------------------
        '----- Qualità

        If Me.Cmb_QualitaCatasto.SelectedItem.Text = "" Then
            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IndicareLaQualità"), String) & vbCrLf
        End If

        '----------------------------------------
        '----- Redditi

        If Me.TxtRedditoDom.Text = "" Then
            Me.TxtRedditoDom.Text = 0
        Else
            If InStr(TxtRedditoDom.Text, ".") <> 0 Then
                TxtRedditoDom.Text = Replace(TxtRedditoDom.Text, ".", ",")
            End If
            If Not IsNumeric(TxtRedditoDom.Text) Then
                MessaggioErrore += "   - E' necessario indicare il REDDITO DOMINICALE in formato numerico." & vbCrLf
            End If
        End If

        If Me.TxtRedditoAgr.Text = "" Then
            Me.TxtRedditoAgr.Text = 0
        Else
            If InStr(TxtRedditoAgr.Text, ".") <> 0 Then
                TxtRedditoAgr.Text = Replace(TxtRedditoAgr.Text, ".", ",")
            End If
            If Not IsNumeric(TxtRedditoAgr.Text) Then
                MessaggioErrore += "   - E' necessario indicare il REDDITO AGRARIO in formato numerico." & vbCrLf
            End If
        End If

        '----------------------------------------
        '----- Sup

        If Me.TxtSupClass.Text = "" Then
            Me.TxtSupClass.Text = 0
        Else
            If InStr(TxtSupClass.Text, ".") <> 0 Then
                TxtSupClass.Text = Replace(TxtSupClass.Text, ".", ",")
            End If
            If Not IsNumeric(Me.TxtSupClass.Text) Then
                MessaggioErrore += "   - E' necessario indicare la Superficie di Classamento in formato numerico." & vbCrLf
            End If
        End If

        '--- RIEPILOGO

        Dim Messaggio As String

        If MessaggioErrore <> "" Then

            Messaggio = ""
            Messaggio += AgronicaAgenda_2010.SonoStatiRilevatiISeguentiErrori_ & vbCrLf
            Messaggio += "" & vbCrLf
            Messaggio += MessaggioErrore
            Messaggio += "" & vbCrLf
            Messaggio += AgronicaAgenda_2010.RitentareIlSalvataggioDopoLaCorrezione

            Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanel_script, , True)

            Exit Sub

        End If

        '----- Recupero il datatable

        If Not IsNothing(ViewState("vs_dtClassamento")) Then
            'Recupero il datatable dal ViewState
            Dt = ViewState("vs_dtClassamento")
        Else
            Dt = New DataTable
        End If


        '----- Inserisco la riga nel datagrid

        'Creo una nuova riga
        Dr = Dt.NewRow

        'Definisco i valori

        Dr.Item("QUALITA_COD") = Me.Cmb_QualitaCatasto.SelectedValue
        Dr.Item("QUALITA_Des") = Me.Cmb_QualitaCatasto.SelectedItem.Text
        Dr.Item("Porzione") = Me.TxtPorzione.Text
        Dr.Item("CLASSE") = Me.TxtClasseCatasto.Text
        Dr.Item("Deduzione") = ""
        Dr.Item("Sup_Classe") = Me.TxtSupClass.Text
        Dr.Item("REDDITO_DOMINICALE") = Me.TxtRedditoDom.Text
        Dr.Item("REDDITO_AGRARIO") = Me.TxtRedditoAgr.Text

        'Associo alla tabella la nuova riga creata
        Dt.Rows.Add(Dr)

        '----- Associo il DataTable con il DataGrid
        Aggiorna_Griglia_Classamento(Dt)

        '----- Azzero i controlli di provenienza dei dati

        Me.TxtPorzione.Text = ""
        Me.Cmb_QualitaCatasto.SelectedIndex = -1
        Me.TxtClasseCatasto.Text = ""
        Me.TxtSupClass.Text = ""
        Me.TxtRedditoDom.Text = ""
        Me.TxtRedditoAgr.Text = ""


    End Sub



    '##############################################################################################################################
    Private Sub ImgBtn_SalvaTutto_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SalvaTutto.Click

        Salva_Tutto()

    End Sub



    '####################################################################
    Private Sub Salva_Tutto()

        Dim TipoOperazioneDB As enum_TipoOperazioneDB

        Dim Piva As String
        Dim Sa_Cod As Integer

        Dim Part_Cod As Integer

        Dim Prov As String
        Dim Com As String
        Dim Sezione As String
        Dim Foglio As Integer
        Dim Numero As Integer
        Dim Subalterno As String

        Dim Prov_Old As String
        Dim Com_Old As String
        Dim Sezione_Old As String
        Dim Foglio_Old As Integer
        Dim Numero_Old As Integer
        Dim Subalterno_Old As String

        Dim Partita_Catastale As String

        Dim Ettari As Double
        Dim Are As Integer
        Dim Centiare As Integer
        Dim Sup_Particella As Double

        Dim Qualita_Cod As Integer
        Dim Classe As String
        Dim Reddito_Dominicale As Double
        Dim Reddito_Agrario As Double

        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date
        Dim Validita_Inizio_Centro As Date
        Dim Validita_Fine_Centro As Date

        Dim i, j As Integer

        Dim StrValidita_Inizio As String
        Dim StrValidita_Fine As String

        Dim TitoloPossesso As Integer

        Dim BaseCode As Integer
        Dim TopCode As Integer

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlDatiParticelle As System.Xml.XmlElement

        Dim StrDummy As String
        Dim IntDummy As Integer
        Dim strErr As String

        '  Dim objSQL As New Codex_Utility.Sql
        Dim StrSQL As String
        Dim NumeroRecordInteressati As Integer

        Dim xChiave As String
        Dim xTipoNodo As enum_TipoNodo

        '  Dim xPiva As String
        ' Dim xSa_Cod As Integer

        Dim xPart_Cod As Integer
        Dim xCodProvincia As String
        Dim xCodComune As String
        Dim xSezione As String
        Dim xFoglio As Integer
        Dim xNumero As Integer
        Dim xSubalterno As String

        Dim SupZona As Double = 0
        Dim TotSupZona As Double = 0
        Dim SupClasse As Double = 0
        Dim TotSupClasse As Double = 0
        Dim SupMacrouso As Double = 0
        Dim TotSupMacrouso As Double = 0


        Dim SalvaContinua As Boolean = False

        '------------------------------------------------
        '----- Verifico l'operazione richiesta (Inserimento / Modifica / Cancellazione)
        '------------------------------------------------

        'Recupero l'operazione richiesta, dalla querystring
        TipoOperazioneDB = Operazione

        '------------------------------------------------
        '----- Prelevo le informazioni immediate
        '------------------------------------------------

        Piva = xPiva
        Sa_Cod = xSa_Cod
        'Prov = TxtCodProvincia.Text
        'Com = TxtCodComune.Text
        Prov = HttpContext.Current.Session("cod_prov")
        If IsNothing(Prov) Or Prov = "" Then
            Prov = Cmb_Provincia.SelectedItem.Text.Substring(4, 3)
        End If

        Com = Replace(HttpContext.Current.Session("cod_com"), "|", "")
        If IsNothing(Com) Or Com = "" Then
            Com = Cmb_Comune.SelectedItem.Text.Substring(1, 3)
        End If
        'Exit Sub

        If Txt_Sezione.Text = "" Then
            Sezione = "0"
        Else
            Sezione = Txt_Sezione.Text
        End If

        Foglio = Txt_Foglio.Text
        Numero = Txt_Numero.Text

        If Txt_Subalterno.Text = "" Then
            Subalterno = "0"
        Else
            Subalterno = Txt_Subalterno.Text
        End If


        '------------------------------------------------
        '----- Verifico la correttezza delle informazioni inserite
        '------------------------------------------------

        Dim MessaggioErrore As String = ""


        '--- Date di validita
        'If GridView_Possessi.Rows.Count < 1 Then
        '    MessaggioErrore += "   - E' necessario definire il TITOLO di POSSESSO."
        'End If

        '----- Superfici

        If Me.TxtSup_Ettari.Text = "" Then
            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("GliEttariDiSuperficieDevonoEssereIndicatiConUnIntero"), String) & vbCrLf
        Else
            If Not IsNumeric(TxtSup_Ettari.Text) Then
                MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("GliEttariDiSuperficieDevonoEssereIndicatiConUnIntero"), String) & vbCrLf
            Else
                If CDbl(TxtSup_Ettari.Text) <> CDbl(Int(TxtSup_Ettari.Text)) Then
                    MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("GliEttariDiSuperficieDevonoEssereIndicatiConUnIntero"), String) & vbCrLf
                Else
                    If Int(TxtSup_Ettari.Text) < 0 Then
                        MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("GliEttariDiSuperficieDevonoEssereIndicatiConUnNumeroPositivo"), String) & vbCrLf
                    Else
                        Ettari = Int(TxtSup_Ettari.Text)
                    End If
                End If
            End If
        End If


        If Me.TxtSup_Are.Text = "" Then
            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LeAreDiSuperficieDevonoEssereIndicateConUnIntero"), String) & vbCrLf
        Else
            If Not IsNumeric(TxtSup_Are.Text) Then
                MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LeAreDiSuperficieDevonoEssereIndicateConUnIntero"), String) & vbCrLf
            Else
                If CDbl(TxtSup_Are.Text) <> CDbl(Int(TxtSup_Are.Text)) Then
                    MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LeAreDiSuperficieDevonoEssereIndicateConUnIntero"), String) & vbCrLf
                Else
                    If CInt(TxtSup_Are.Text) >= 100 Then
                        MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LeAreDiSuperficieDevonoEssereCompreseNelRange"), String) & vbCrLf
                    Else
                        If Int(TxtSup_Are.Text) < 0 Then
                            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LeAreDiSuperficieDevonoEssereIndicateConUnNumeroPositivo"), String) & vbCrLf
                        Else
                            Are = Int(TxtSup_Are.Text)
                        End If
                    End If
                End If
            End If
        End If


        If Me.TxtSup_Centiare.Text = "" Then
            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LeCentiareDiSuperficieDevonoEssereIndicateConUnIntero"), String) & vbCrLf
        Else
            If Not IsNumeric(TxtSup_Centiare.Text) Then
                MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LeCentiareDiSuperficieDevonoEssereIndicateConUnIntero"), String) & vbCrLf
            Else
                If CDbl(TxtSup_Centiare.Text) <> CDbl(Int(TxtSup_Centiare.Text)) Then
                    MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LeCentiareDiSuperficieDevonoEssereIndicateConUnIntero"), String) & vbCrLf
                Else
                    If CInt(TxtSup_Centiare.Text) >= 100 Then
                        MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LeCentiareDiSuperficieDevonoEssereCompreseNelRange"), String) & vbCrLf
                    Else
                        If CInt(TxtSup_Centiare.Text) < 0 Then
                            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LeCentiareDiSuperficieDevonoEssereIndicateConUnNumeroPositivo"), String) & vbCrLf
                        Else
                            Centiare = Int(TxtSup_Centiare.Text)
                        End If
                    End If
                End If
            End If
        End If

        Sup_Particella = Ettari_from_EttariAreCentiare(Ettari, Are, Centiare)

        '----- Superfici Zone

        'Verifico che la somma delle superfici assegnate alle zone 
        'nn superi quella della particella
        'For i = 0 To GridView_Zone.Rows.Count - 1

        '    SupZona = GridView_Zone.DataKeys(i).Item(1)
        '    TotSupZona += SupZona

        'Next

        ' commenatato Nicoletta 22/09/2014. Una particella può essere ZVN e Collinare allo stesso tempo, le zone non sono esclusive
        'If Sup_Particella <> 0 Then
        '    If TotSupZona > Sup_Particella Then
        '        MessaggioErrore += "   - La Superficie assegnata alle zone non può superare la superficie della particella." & vbCrLf
        '    End If
        'End If


        '----- Superfici MACROUSO

        'Verifico che la somma delle superfici assegnate ai macrousi
        'nn superi quella della particella

        'For i = 0 To GridView_Macrousi.Rows.Count - 1

        '    SupMacrouso = GridView_Macrousi.DataKeys(i).Item(4)
        '    TotSupMacrouso += SupMacrouso

        'Next


        'If Sup_Particella <> 0 Then
        '    If TotSupMacrouso > Sup_Particella Then
        '        MessaggioErrore += "   - La Superficie totale dei macrousi non può superare la superficie della particella." & vbCrLf
        '    End If
        'End If

        '----- Superfici Classamento

        'Verifico che la somma delle superfici assegnate alle zone 
        'nn superi quella della particella
        'For i = 0 To GridView_Classamento.Rows.Count - 1

        '    SupClasse = GridView_Classamento.DataKeys(i).Item(1)
        '    TotSupClasse += SupClasse

        'Next

        If Sup_Particella <> 0 Then
            If TotSupClasse > Sup_Particella Then
                MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("SuperficieDeiDatiClassamentoSuperioreASuperficieParticella"), String) & vbCrLf
            End If
        End If


        '----- Foglio e Numero

        If Txt_Foglio.Text = "" Then
            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("ImpostareIlFoglioCatastale"), String) & vbCrLf
        Else
            If Not IsNumeric(Txt_Foglio.Text) Or Trim(Txt_Foglio.Text) = "0" Then
                MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IlFoglioCatastaleDeveEssereIndicatoConUnInteroPositivo"), String) & vbCrLf
            Else
                If CDbl(Txt_Foglio.Text) <> CDbl(Int(Txt_Foglio.Text)) Then
                    MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IlFoglioCatastaleDeveEssereIndicatoConUnInteroPositivo"), String) & vbCrLf
                Else
                    If CInt(Txt_Foglio.Text) < 0 Then
                        MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IlFoglioCatastaleDeveEssereIndicatoConUnInteroPositivo"), String) & vbCrLf
                    End If
                End If
            End If
        End If

        If Txt_Numero.Text = "" Then
            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("ImpostareIlNumeroCatastale"), String) & vbCrLf
        Else
            If Not IsNumeric(Txt_Numero.Text) Or Trim(Txt_Numero.Text) = "0" Then
                MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IlNumeroCatastaleDeveEssereIndicatoConUnInteroPositivo"), String) & vbCrLf
            Else
                If CDbl(Txt_Numero.Text) <> CDbl(Int(Txt_Numero.Text)) Then
                    MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IlNumeroCatastaleDeveEssereIndicatoConUnInteroPositivo"), String) & vbCrLf
                Else
                    If CInt(Txt_Numero.Text) < 0 Then
                        MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IlNumeroCatastaleDeveEssereIndicatoConUnInteroPositivo"), String) & vbCrLf
                    End If
                End If
            End If
        End If

        '----- Provincia e Comune

        'If Cmb_Provincia.SelectedIndex = 0 Then
        '    MessaggioErrore += "   - E' necessario impostare la PROVINCIA di competenza." & vbCrLf
        'End If

        'If Cmb_Comune.SelectedIndex = 0 Then
        '    MessaggioErrore += "   - E' necessario impostare il COMUNE di competenza." & vbCrLf
        'End If



        Dim Messaggio As String

        If MessaggioErrore <> "" Then

            Messaggio = ""
            Messaggio += AgronicaAgenda_2010.SonoStatiRilevatiISeguentiErrori_ & vbCrLf
            Messaggio += "" & vbCrLf
            Messaggio += MessaggioErrore
            Messaggio += "" & vbCrLf
            Messaggio += AgronicaAgenda_2010.RitentareIlSalvataggioDopoLaCorrezione

            Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanel_script, , True)

            Exit Sub

        End If

        '=======================
        '===  Aggiornamento  ===
        '=======================

        Dim Errore As Boolean = False
        '------------------------------------------------
        '----- apro connessione e transazione
        '------------------------------------------------
        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)


        Try


            '------------------------------------------------
            '----- Recupero le informazioni
            '------------------------------------------------

            Ettari = TxtSup_Ettari.Text
            Are = TxtSup_Are.Text
            Centiare = TxtSup_Centiare.Text

            Partita_Catastale = ""

            If TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then
                '### MODIFICA ###
                Part_Cod = objParametriAgenda.Particelle(0).Part_Cod

            ElseIf TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura Then
                '### CREAZIONE ###
                Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
                'Part_Cod = objSeq.Agronica_SequenzaTabelle_NuovoID("particellecatastali",
                '                                            objParametri_Server)
                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                Part_Cod = objSeq.NuovoId_Tabella("particellecatastali", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
            End If


            Qualita_Cod = 0
            Classe = ""
            Reddito_Dominicale = 0
            Reddito_Agrario = 0

            Dim listaRigheMetodiProd As List(Of PCatastaleMetodoProduzioneModel) = Session("righeGrigliaPCatastaleMetodiProd")
            Dim grigliaMetodiProdModificata As Boolean = Session("grigliaPCatastaleMetodiProd_modificata")

            '------------------------------------------------
            '----- Inserisco o Modifico la PARTICELLA
            '------------------------------------------------

            Dim DTP As DataTable

            Dim ObjCOMP_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
            Dim ObjCOMP_W As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_W
            Dim ObjMetodoProduzione_W As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_MetodoProduzione_W
            Dim ObjCOMIP2_R As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
            Dim ObjCOMIP2_W As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_W
            Dim ObjAppxPart As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W
            Dim ObjCampixPart As New AgronicaCoreAnagrafeDAL.CampixParticelle_W
            Dim ObjProgettixPart As New AgronicaCoreAnagrafeDAL.ProgettixParticelle_W
            Dim ObjImpreseParticelleCodici As New AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_W

            Dim chiaveModificata As Boolean = False

            Select Case TipoOperazioneDB

                Case enum_TipoOperazioneDB.Scrittura

                    '----- Verifico se la particella esiste gia' ...

                    DTP = ObjCOMP_R.LeggixChiave(CStr(Prov),
                                               CStr(Com),
                                               CStr(Sezione),
                                               CInt(Foglio),
                                               CInt(Numero),
                                               CStr(Subalterno),
                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "", "",
                                                objParametri_Server)

                    If (Not IsNothing(DTP)) AndAlso (DTP.Rows.Count > 0) Then
                        '### ESISTE GIA' ###

                        '--- MODIFICA ---
                        ObjCOMP_W.Modifica_2(
                                            CStr(Prov),
                                            CStr(Com),
                                            CStr(Sezione),
                                            CInt(Foglio),
                                            CInt(Numero),
                                            CStr(Subalterno),
                                            CStr(Partita_Catastale),
                                            CDbl(Ettari),
                                            CDbl(Are),
                                            CDbl(Centiare),
                                            CInt(Qualita_Cod),
                                            CStr(Classe),
                                            CDbl(Reddito_Dominicale),
                                            CDbl(Reddito_Agrario),
                                            CInt(-1),
                                            AGRODATAINIZIO,
                                            AGRODATAFINE,
                                            "",
                                            objParametri_Server)

                    Else
                        '### NON ESISTE ###

                        '--- INSERIMENTO ---
                        IntDummy = ObjCOMP_W.Scrivi(
                                                    CInt(Part_Cod),
                                                    CStr(Prov),
                                                    CStr(Com),
                                                    CStr(Sezione),
                                                    CInt(Foglio),
                                                    CInt(Numero),
                                                    CStr(Subalterno),
                                                    CStr(Partita_Catastale),
                                                    CDbl(Ettari),
                                                    CDbl(Are),
                                                    CDbl(Centiare),
                                                    CInt(Qualita_Cod),
                                                    CStr(Classe),
                                                    CDbl(Reddito_Dominicale),
                                                    CDbl(Reddito_Agrario),
                                                    CInt(-1),
                                                    AGRODATAINIZIO,
                                                    AGRODATAFINE,
                                                    objParametri_Server)
                    End If



                Case enum_TipoOperazioneDB.Modifica

                    '-----------------------------------------------------
                    '----- controllo se è stata modificata la chiave
                    '-----------------------------------------------------

                    Prov_Old = ViewState("provincia")
                    Com_Old = ViewState("comune")
                    Sezione_Old = ViewState("sezione")
                    Foglio_Old = CInt(ViewState("foglio"))
                    Numero_Old = CInt(ViewState("numero"))
                    Subalterno_Old = ViewState("subalterno")

                    '------------------------------------------------------------------------------------------
                    '----- se è stata modificata la chiave
                    '----- modifico la particella (se NON esiste già)
                    '----- cancello i record di relazione della vecchia nella tabella ImpresexParticelle
                    '----- modifico i record di relazione della vecchia nella tabella AppezzamentiXParticelle
                    '----- modifico i record di relazione della vecchia nella tabella CampiXParticelle
                    '----- modifico i record di relazione della vecchia nella tabella ProgettiXParticelle
                    '------------------------------------------------------------------------------------------
                    If Prov_Old <> Prov Or
                        Com_Old <> Com Or
                        Sezione_Old <> Sezione Or
                        Foglio_Old <> Foglio Or
                        Numero_Old <> Numero Or
                        Subalterno_Old <> Subalterno Then

                        chiaveModificata = True

                        Dim xRisp As Boolean

                        '----- Verifico se la NUOVA particella esiste gia' ...

                        DTP = ObjCOMP_R.LeggixChiave(CStr(Prov),
                                                   CStr(Com),
                                                   CStr(Sezione),
                                                   CInt(Foglio),
                                                   CInt(Numero),
                                                   CStr(Subalterno),
                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    "", "",
                                                    objParametri_Server)

                        If (Not IsNothing(DTP)) AndAlso (DTP.Rows.Count > 0) Then
                            '### ESISTE GIA' ###
                            '--- NON FACCIO NULLA
                            xRisp = True
                        Else
                            '### NON ESISTE ###

                            '--- MODIFICO ---

                            xRisp = ObjCOMP_W.Modifica(Prov, Com, Sezione, Foglio, Numero, Subalterno,
                                                       Prov_Old, Com_Old, Sezione_Old, Foglio_Old, Numero_Old, Subalterno_Old,
                                                       Partita_Catastale,
                                                       Ettari, Are, Centiare,
                                                       Qualita_Cod, Classe, Reddito_Dominicale, Reddito_Agrario,
                                                       "", objParametri_Server)

                        End If


                        '------------------------------------------------------------------------------------------
                        '----- modifico la particella

                        'StrSQL = ""
                        'StrSQL += " UPDATE ParticelleCatastali "
                        'StrSQL += " SET  "
                        'StrSQL += " PROV = '" & Prov & "', "
                        'StrSQL += " COM = '" & Com & "', "
                        'StrSQL += " SEZIONE = '" & Sezione & "', "
                        'StrSQL += " FOGLIO =" & Foglio & ", "
                        'StrSQL += " NUMERO =" & Numero & ", "
                        'StrSQL += " Subalterno = '" & Subalterno & "',"
                        'StrSQL += " PARTITA_CATASTALE = '" & Partita_Catastale & "', "
                        'StrSQL += " ETTARI =" & Ettari & ", "
                        'StrSQL += " ARE =" & Are & ", "
                        'StrSQL += " CENTIARE = " & Centiare & ","
                        'StrSQL += " QUALITA_COD = " & Qualita_Cod & ", "
                        'StrSQL += " CLASSE ='" & Classe & "', "
                        'StrSQL += " REDDITO_DOMINICALE =" & Reddito_Dominicale & ", "
                        'StrSQL += " REDDITO_AGRARIO = " & Reddito_Agrario & ","
                        'StrSQL += " Data_Modifica =" & Agro_SQL_SaveDate(Now.Today) & ","
                        'StrSQL += " Username_Modifica = '" & Agro_SQL_SaveText(Session("ASG_Utente_CodFiscale").ToString) & "'"
                        'StrSQL += " WHERE PROV = '" & Prov_Old & "' "
                        'StrSQL += " AND COM = '" & Com_Old & "' "
                        'StrSQL += " AND SEZIONE ='" & Sezione_Old & "' "
                        'StrSQL += " AND FOGLIO =" & Foglio_Old & " "
                        'StrSQL += " AND NUMERO =" & Numero_Old & " "
                        'StrSQL += " AND Subalterno = '" & Subalterno_Old & "'"



                        'Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                        'Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                        'objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
                        'objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
                        'Dim dataprovider As New AgronicaCoreDataProvider.DataProvider
                        'xRisp = DataProvider.EseguiQuery_Scrittura(objParametri_Server, StrSQL.ToString, "GestioneCatasto")

                        'Verifico la presenza di errori
                        If xRisp Then

                            '--------------------------------------------------------
                            '----- cancello codice particella impresa
                            '--------------------------------------------------------
                            Dim dt = ObjCOMIP2_R.Leggi(0, Piva, Sa_Cod, 0, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                            For Each dr In dt.Rows
                                ObjImpreseParticelleCodici.Cancella(dr.Item("ID"), enum_CodiciAnagrafe.CodiceParticella, "", objParametri_Server)
                            Next

                            '------------------------------------------------------------------------------------------
                            '----- Cancello le relazioni della vecchia particella con l'impresa

                            ObjCOMIP2_W.Cancella(
                                        CStr(Piva),
                                        CInt(Sa_Cod),
                                        CStr(Prov_Old),
                                        CStr(Com_Old),
                                        CStr(Sezione_Old),
                                        CInt(Foglio_Old),
                                        CInt(Numero_Old),
                                        CStr(Subalterno_Old),
                                        "",
                                        objParametri_Server)


                            '------------------------------------------------------------------------------------------
                            '----- Modifico le relazioni della vecchia particella con eventuali appezzamenti

                            'StrSQL = ""
                            'StrSQL += " UPDATE AppezzamentiXParticelle "
                            'StrSQL += " SET  "
                            'StrSQL += " PROV = '" & Prov & "', "
                            'StrSQL += " COM = '" & Com & "', "
                            'StrSQL += " SEZIONE = '" & Sezione & "', "
                            'StrSQL += " FOGLIO =" & Foglio & ", "
                            'StrSQL += " NUMERO =" & Numero & ", "
                            'StrSQL += " Subalterno = '" & Subalterno & "',"
                            'StrSQL += " Data_Modifica =" & Agro_SQL_SaveDate(Now.Today) & ","
                            'StrSQL += " Username_Modifica = '" & Agro_SQL_SaveDate(Session("ASG_Utente_CodFiscale").ToString) & "'"
                            'StrSQL += " WHERE PROV = '" & Prov_Old & "' "
                            'StrSQL += " AND COM = '" & Com_Old & "' "
                            'StrSQL += " AND SEZIONE ='" & Sezione_Old & "' "
                            'StrSQL += " AND FOGLIO =" & Foglio_Old & " "
                            'StrSQL += " AND NUMERO =" & Numero_Old & " "
                            'StrSQL += " AND Subalterno = '" & Subalterno_Old & "'"

                            ''Recupero il recordset
                            'xRisp = False
                            'xRisp = DataProvider.EseguiQuery_Scrittura(objParametri_Server, StrSQL.ToString, "aaa")


                            xRisp = ObjAppxPart.ModificaChiave(Prov, Com, Sezione, Foglio, Numero, Subalterno,
                                                               Prov_Old, Com_Old, Sezione_Old, Foglio_Old, Numero_Old, Subalterno_Old,
                                                               "", objParametri_Server)

                            'Verifico la presenza di errori
                            If xRisp Then

                            End If

                            '------------------------------------------------------------------------------------------
                            '----- Modifico le relazioni della vecchia particella con eventuali campi

                            'StrSQL = ""
                            'StrSQL += " UPDATE CampiXParticelle "
                            'StrSQL += " SET  "
                            'StrSQL += " PROV = '" & Prov & "', "
                            'StrSQL += " COM = '" & Com & "', "
                            'StrSQL += " SEZIONE = '" & Sezione & "', "
                            'StrSQL += " FOGLIO =" & Foglio & ", "
                            'StrSQL += " NUMERO =" & Numero & ", "
                            'StrSQL += " Subalterno = '" & Subalterno & "',"
                            'StrSQL += " Data_Modifica =" & Agro_SQL_SaveDate(Now.Today) & ","
                            'StrSQL += " Username_Modifica = '" & Agro_SQL_SaveText(Session("ASG_Utente_CodFiscale").ToString) & "'"
                            'StrSQL += " WHERE PROV = '" & Prov_Old & "' "
                            'StrSQL += " AND COM = '" & Com_Old & "' "
                            'StrSQL += " AND SEZIONE ='" & Sezione_Old & "' "
                            'StrSQL += " AND FOGLIO =" & Foglio_Old & " "
                            'StrSQL += " AND NUMERO =" & Numero_Old & " "
                            'StrSQL += " AND Subalterno = '" & Subalterno_Old & "'"

                            'xRisp = False
                            'xRisp = dataprovider.EseguiQuery_Scrittura(objParametri_Server, StrSQL.ToString, "aaa")

                            xRisp = ObjCampixPart.ModificaChiave(Prov, Com, Sezione, Foglio, Numero, Subalterno,
                                                               Prov_Old, Com_Old, Sezione_Old, Foglio_Old, Numero_Old, Subalterno_Old,
                                                               "", objParametri_Server)

                            'Verifico la presenza di errori
                            If xRisp Then

                            End If

                            '------------------------------------------------------------------------------------------
                            '----- Modifico le relazioni della vecchia particella con eventuali distinte

                            'StrSQL = ""
                            'StrSQL += " UPDATE ProgettiXParticelle "
                            'StrSQL += " SET  "
                            'StrSQL += " PROV = '" & Prov & "', "
                            'StrSQL += " COM = '" & Com & "', "
                            'StrSQL += " SEZIONE = '" & Sezione & "', "
                            'StrSQL += " FOGLIO =" & Foglio & ", "
                            'StrSQL += " NUMERO =" & Numero & ", "
                            'StrSQL += " Subalterno = '" & Subalterno & "',"
                            'StrSQL += " Data_Modifica =" & Agro_SQL_SaveDate(Now.Today) & ","
                            'StrSQL += " Username_Modifica = '" & Agro_SQL_SaveText(Session("ASG_Utente_CodFiscale").ToString) & "'"
                            'StrSQL += " WHERE PROV = '" & Prov_Old & "' "
                            'StrSQL += " AND COM = '" & Com_Old & "' "
                            'StrSQL += " AND SEZIONE ='" & Sezione_Old & "' "
                            'StrSQL += " AND FOGLIO =" & Foglio_Old & " "
                            'StrSQL += " AND NUMERO =" & Numero_Old & " "
                            'StrSQL += " AND Subalterno = '" & Subalterno_Old & "'"

                            'xRisp = False
                            'xRisp = dataprovider.EseguiQuery_Scrittura(objParametri_Server, StrSQL.ToString, "aaa")

                            xRisp = ObjProgettixPart.ModificaChiave(Prov, Com, Sezione, Foglio, Numero, Subalterno,
                                                                    Prov_Old, Com_Old, Sezione_Old, Foglio_Old, Numero_Old, Subalterno_Old,
                                                                    "", objParametri_Server)

                            'Verifico la presenza di errori
                            If xRisp Then

                            End If

                            '--------------------------------------------------------
                            '----- cancello eventuali Particelle_Codici
                            '--------------------------------------------------------
                            xRisp = ObjCOMP_W.Cancella_ParticelleCatastali_Codici(CStr(Prov_Old),
                                                                                    CStr(Com_Old),
                                                                                    CStr(Sezione_Old),
                                                                                    CInt(Foglio_Old),
                                                                                    CInt(Numero_Old),
                                                                                    CStr(Subalterno_Old),
                                                                                    0, "", objParametri_Server)


                            '--------------------------------------------------------
                            '----- cancello eventuali METODI di PRODUZIONE
                            '--------------------------------------------------------
                            ObjMetodoProduzione_W.Elimina(
                            Prov_Old,
                            Com_Old,
                            Sezione_Old,
                            Foglio_Old,
                            Numero_Old,
                            Subalterno_Old,
                            -1,
                            objParametri_Server)


                            '--------------------------------------------------------
                            '----- cancello eventuali DATI di CLASSAMENTO
                            '--------------------------------------------------------

                            xRisp = ObjCOMP_W.Cancella_ParticelleCatastaliClassamento(CStr(Prov_Old),
                                                                                    CStr(Com_Old),
                                                                                    CStr(Sezione_Old),
                                                                                    CInt(Foglio_Old),
                                                                                    CInt(Numero_Old),
                                                                                    CStr(Subalterno_Old),
                                                                                    "", objParametri_Server)


                            '--------------------------------------------------------
                            '----- cancello eventuali ZONE
                            '--------------------------------------------------------
                            Dim objZonexPart As New AgronicaCoreAnagrafeDAL.ZonexParticelle_W
                            IntDummy = objZonexPart.Cancella(0,
                                                            CStr(Prov_Old),
                                                            CStr(Com_Old),
                                                            CStr(Sezione_Old),
                                                            CInt(Foglio_Old),
                                                            CInt(Numero_Old),
                                                            CStr(Subalterno_Old),
                                                            "",
                                                            objParametri_Server)

                            '--------------------------------------------------------
                            '----- cancello eventuali MACROUSO
                            '--------------------------------------------------------
                            Dim objMacrousoxPart As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_W
                            IntDummy = objMacrousoxPart.Cancella(Piva,
                                                                 CStr(Prov_Old),
                                                                 CStr(Com_Old),
                                                                 CStr(Sezione_Old),
                                                                 CInt(Foglio_Old),
                                                                 CInt(Numero_Old),
                                                                 CStr(Subalterno_Old),
                                                                 "",
                                                                 "", objParametri_Server)


                        End If




                    Else


                        '------------------------------------------------------------------------------------------
                        '----- se non è stata modificata la chiave
                        '----- modifico la particella
                        '----- cancello i record di relazione della particella nella tabella ImpresexParticelle
                        '----- cancello i record di relazione della particella nella tabella ParticelleCatastali_MetodoProduzione, perché ne effettuo di seguito l'inserimento
                        '------------------------------------------------------------------------------------------

                        '------------------------------------------------------------------------------------------
                        '----- modifico la particella

                        ObjCOMP_W.Modifica_2(
                                            CStr(Prov),
                                            CStr(Com),
                                            CStr(Sezione),
                                            CInt(Foglio),
                                            CInt(Numero),
                                            CStr(Subalterno),
                                            CStr(Partita_Catastale),
                                            CDbl(Ettari),
                                            CDbl(Are),
                                            CDbl(Centiare),
                                            CInt(Qualita_Cod),
                                            CStr(Classe),
                                            CDbl(Reddito_Dominicale),
                                            CDbl(Reddito_Agrario),
                                            CInt(-1),
                                            AGRODATAINIZIO,
                                            AGRODATAFINE,
                                            "", objParametri_Server)


                        '--------------------------------------------------------
                        '----- cancello codice particella impresa
                        '--------------------------------------------------------
                        Dim dt = ObjCOMIP2_R.Leggi(0, Piva, Sa_Cod, 0, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                        For Each dr In dt.Rows
                            ObjImpreseParticelleCodici.Cancella(dr.Item("ID"), enum_CodiciAnagrafe.CodiceParticella, "", objParametri_Server)
                        Next

                        '------------------------------------------------------------------------------------------
                        '----- Cancello le relazioni della particella con l'impresa

                        ObjCOMIP2_W.Cancella(
                                        CStr(Piva),
                                        CInt(Sa_Cod),
                                        CStr(Prov),
                                        CStr(Com),
                                        CStr(Sezione),
                                        CInt(Foglio),
                                        CInt(Numero),
                                        CStr(Subalterno),
                                        "",
                                        objParametri_Server)


                        '--------------------------------------------------------
                        '----- cancello eventuali METODI di PRODUZIONE
                        '--------------------------------------------------------
                        If grigliaMetodiProdModificata = True Then
                            ObjMetodoProduzione_W.Elimina(
                                Prov,
                                Com,
                                Sezione,
                                Foglio,
                                Numero,
                                Subalterno,
                                -1,
                                objParametri_Server)
                        End If


                        '--------------------------------------------------------
                        '----- cancello eventuali DATI di CLASSAMENTO
                        '--------------------------------------------------------

                        ObjCOMP_W.Cancella_ParticelleCatastaliClassamento(CStr(Prov),
                                                                                CStr(Com),
                                                                                CStr(Sezione),
                                                                                CInt(Foglio),
                                                                                CInt(Numero),
                                                                                CStr(Subalterno),
                                                                                "", objParametri_Server)



                        '--------------------------------------------------------
                        '----- cancello eventuali ZONE
                        '--------------------------------------------------------
                        Dim objzonePart As New AgronicaCoreAnagrafeDAL.ZonexParticelle_W
                        IntDummy = objzonePart.Cancella(0,
                                                                    CStr(Prov),
                                                                    CStr(Com),
                                                                    CStr(Sezione),
                                                                    CInt(Foglio),
                                                                    CInt(Numero),
                                                                    CStr(Subalterno),
                                                                    "",
                                                                    objParametri_Server)


                        '--------------------------------------------------------
                        '----- cancello eventuali Macrousi
                        '--------------------------------------------------------
                        Dim objmacPart As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_W
                        IntDummy = objmacPart.Cancella(Piva,
                                                       CStr(Prov),
                                                       CStr(Com),
                                                       CStr(Sezione),
                                                       CInt(Foglio),
                                                       CInt(Numero),
                                                       CStr(Subalterno),
                                                       "",
                                                       "", objParametri_Server)


                    End If





            End Select


            '------------------------------------------------
            '----- Inserisco : IMPRESE x PARTICELLE
            '------------------------------------------------

            '----- Inserisco le relazioni presenti nel datagrid

            Dim StrTitoloPossesso As String
            Dim StrSup_Condotta As String
            Dim StrCod_Particella As String

            Dim DT_Possessi As New DataTable
            Dim ImpresexParticelle_Codici_W As New AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_W

            DT_Possessi = HttpContext.Current.Session("dt_Possessi")

            If Not IsNothing(DT_Possessi) AndAlso DT_Possessi.Rows.Count > 0 Then

                'Per ciascuna riga del datagrid ...
                For i = 0 To DT_Possessi.Rows.Count - 1

                    '-----

                    'Dt.Columns.Add(New DataColumn("Contatore", GetType(Integer)))
                    'Dt.Columns.Add(New DataColumn("TitoloPossesso_Des", GetType(String)))
                    'Dt.Columns.Add(New DataColumn("TitoloPossesso_Cod", GetType(String)))
                    'Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
                    'Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
                    'Dt.Columns.Add(New DataColumn("Sup_Condotta", GetType(String)))

                    'Recupero le informazioni
                    StrValidita_Inizio = DT_Possessi.Rows(i).Item("Validita_Inizio")
                    StrValidita_Fine = DT_Possessi.Rows(i).Item("Validita_Fine")
                    StrTitoloPossesso = DT_Possessi.Rows(i).Item("TitoloPossesso_Cod")
                    StrSup_Condotta = DT_Possessi.Rows(i).Item("Sup_Condotta")

                    'StrValidita_Inizio = GridView_Possessi.DataKeys(i).Item(2)
                    'StrValidita_Fine = GridView_Possessi.DataKeys(i).Item(3)
                    'StrTitoloPossesso = GridView_Possessi.DataKeys(i).Item(1)
                    'StrSup_Condotta = GridView_Possessi.DataKeys(i).Item(4)

                    'Formatto correttamente
                    If StrValidita_Inizio = "..." Then
                        Validita_Inizio = #1/1/1900#
                    Else
                        Validita_Inizio = CDate(StrValidita_Inizio)
                    End If

                    If StrValidita_Fine = "..." Then
                        Validita_Fine = #12/31/2100#
                    Else
                        Validita_Fine = CDate(StrValidita_Fine)
                    End If

                    TitoloPossesso = CInt(StrTitoloPossesso)

                    If StrSup_Condotta = "..." Then
                        StrSup_Condotta = "0"
                    End If

                    StrSup_Condotta = StrSup_Condotta.Replace(".", ",")

                    '-----

                    IntDummy = ObjCOMIP2_W.Scrivi_2(
                                            CStr(Piva),
                                            CInt(Sa_Cod),
                                            CStr(Prov),
                                            CStr(Com),
                                            CStr(Sezione),
                                            CInt(Foglio),
                                            CInt(Numero),
                                            CStr(Subalterno),
                                            CStr(Partita_Catastale),
                                            CInt(TitoloPossesso),
                                            CDbl(StrSup_Condotta),
                                            CDate(Validita_Inizio),
                                            CDate(Validita_Fine),
                                            objParametri_Server)

                    '--------------------------------------------------------
                    '----- scrivo codice particella impresa
                    '--------------------------------------------------------
                    StrCod_Particella = DBNullToNothing(DT_Possessi.Rows(i).Item("Cod_Particella"))
                    If Not String.IsNullOrEmpty(StrCod_Particella) Then
                        Dim filtro = " ImpresexParticelle.TitoloPossesso = " & TitoloPossesso & " AND ImpresexParticelle.Validita_inizio = " & Agro_SQL_SaveDate(Validita_Inizio)
                        Dim dt = ObjCOMIP2_R.Leggi(0, Piva, Sa_Cod, 0, Prov, Com, Sezione, Foglio, Numero, Subalterno, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, filtro, "", objParametri_Server)
                        For Each dr In dt.Rows
                            ObjImpreseParticelleCodici.Scrivi(dr.Item("ID"), enum_CodiciAnagrafe.CodiceParticella, StrCod_Particella, Validita_Inizio, Validita_Fine, objParametri_Server)
                        Next
                    End If

                Next

            End If


            If chiaveModificata = True OrElse grigliaMetodiProdModificata = True Then
                '------------------------------------------------
                '----- Inserisco : ParticelleCatastali_MetodoProduzione
                '------------------------------------------------
                For Each rigaMetodoProd In listaRigheMetodiProd
                    IntDummy = ObjMetodoProduzione_W.Scrivi(
                        Prov,
                        Com,
                        Sezione,
                        Foglio,
                        Numero,
                        Subalterno,
                        rigaMetodoProd.MetodoProduzione_Cod,
                        rigaMetodoProd.Validita_Inizio,
                        rigaMetodoProd.Validita_Fine,
                        objParametri_Server)
                Next
            End If


            '------------------------------------------------
            '----- Inserisco : ParticelleCatastaliClassamento
            '------------------------------------------------

            Dim Porzione As String

            Dim DT_Classamenti As New DataTable

            DT_Classamenti = HttpContext.Current.Session("dt_Classamenti")

            'Per ciascuna riga del datagrid ...
            For i = 0 To DT_Classamenti.Rows.Count - 1

                'Dt.Columns.Add(New DataColumn("QUALITA_COD", GetType(Integer)))
                'Dt.Columns.Add(New DataColumn("QUALITA_Des", GetType(String)))
                'Dt.Columns.Add(New DataColumn("Porzione", GetType(String)))
                'Dt.Columns.Add(New DataColumn("CLASSE", GetType(String)))
                'Dt.Columns.Add(New DataColumn("Deduzione", GetType(String)))
                'Dt.Columns.Add(New DataColumn("Sup_Classe", GetType(String)))
                'Dt.Columns.Add(New DataColumn("REDDITO_DOMINICALE", GetType(String)))
                'Dt.Columns.Add(New DataColumn("REDDITO_AGRARIO", GetType(String)))

                Qualita_Cod = DT_Classamenti.Rows(i).Item("QUALITA_COD")
                SupClasse = DT_Classamenti.Rows(i).Item("Sup_Classe")
                Porzione = DT_Classamenti.Rows(i).Item("Porzione")
                Classe = DT_Classamenti.Rows(i).Item("CLASSE")
                Reddito_Dominicale = DT_Classamenti.Rows(i).Item("REDDITO_DOMINICALE")
                Reddito_Agrario = DT_Classamenti.Rows(i).Item("REDDITO_AGRARIO")

                'Qualita_Cod = GridView_Classamento.DataKeys(i).Item(0)
                'SupClasse = GridView_Classamento.DataKeys(i).Item(1)
                'Porzione = GridView_Classamento.DataKeys(i).Item(2)
                'Classe = GridView_Classamento.DataKeys(i).Item(3)
                'Reddito_Dominicale = GridView_Classamento.DataKeys(i).Item(4)
                'Reddito_Agrario = GridView_Classamento.DataKeys(i).Item(5)

                '-----
                IntDummy = ObjCOMP_W.Scrivi_ParticelleCatastaliClassamento(
                                        CStr(Prov),
                                        CStr(Com),
                                        CStr(Sezione),
                                        CInt(Foglio),
                                        CInt(Numero),
                                        CStr(Subalterno),
                                        CInt(Qualita_Cod),
                                        CStr(Porzione),
                                        CStr(Classe),
                                        CDbl(SupClasse),
                                        CDbl(Reddito_Dominicale),
                                        CDbl(Reddito_Agrario),
                                        CStr(""),
                                        CDate(Validita_Inizio),
                                        CDate(Validita_Fine),
                                        objParametri_Server)

            Next


            '------------------------------------------------
            '----- Inserisco : ZonexParticelle
            '------------------------------------------------

            Dim ZonaCod As Integer = 0

            Dim DT_Zone As New DataTable

            DT_Zone = HttpContext.Current.Session("dt_Zone")

            '----- Inserisco le relazioni presenti nel datagrid

            'Per ciascuna riga del datagrid ...
            For i = 0 To DT_Zone.Rows.Count - 1

                'Dt.Columns.Add(New DataColumn("Zona_Cod", GetType(Integer)))
                'Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
                'Dt.Columns.Add(New DataColumn("Area", GetType(String)))

                'Recupero le informazioni
                ZonaCod = DT_Zone.Rows(i).Item("Zona_Cod")
                SupZona = DT_Zone.Rows(i).Item("Area")

                'Recupero le informazioni
                StrValidita_Inizio = DT_Zone.Rows(i).Item("Validita_Inizio")
                StrValidita_Fine = DT_Zone.Rows(i).Item("Validita_Fine")

                'Formatto correttamente
                If StrValidita_Inizio = "..." Then
                    Validita_Inizio = #1/1/1900#
                Else
                    Validita_Inizio = CDate(StrValidita_Inizio)
                End If

                If StrValidita_Fine = "..." Then
                    Validita_Fine = #12/31/2100#
                Else
                    Validita_Fine = CDate(StrValidita_Fine)
                End If

                Dim objZonPar As New AgronicaCoreAnagrafeDAL.ZonexParticelle_W
                IntDummy = objZonPar.Scrivi(ZonaCod,
                                                        CStr(Prov),
                                                        CStr(Com),
                                                        CStr(Sezione),
                                                        CInt(Foglio),
                                                        CInt(Numero),
                                                        CStr(Subalterno),
                                                        SupZona,
                                                        CDate(Validita_Inizio),
                                                        CDate(Validita_Fine),
                                                        objParametri_Server)

            Next


            '------------------------------------------------
            '----- Inserisco : MacrousoxParticelle
            '------------------------------------------------

            Dim MacrousoCod As String

            Dim DT_Macrousi As New DataTable

            DT_Macrousi = HttpContext.Current.Session("dt_Macrousi")

            '----- Inserisco le relazioni presenti nel datagrid

            'Dt.Columns.Add(New DataColumn("Contatore", GetType(Integer)))
            'Dt.Columns.Add(New DataColumn("Macrouso_Cod", GetType(String)))
            'Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
            'Dt.Columns.Add(New DataColumn("Superficie", GetType(String)))
            'Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
            'Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

            'Per ciascuna riga del datagrid ...
            For i = 0 To DT_Macrousi.Rows.Count - 1

                'Recupero le informazioni
                MacrousoCod = DT_Macrousi.Rows(i).Item("Macrouso_Cod")
                SupMacrouso = DT_Macrousi.Rows(i).Item("Superficie")

                'Recupero le informazioni
                StrValidita_Inizio = DT_Macrousi.Rows(i).Item("Validita_Inizio")
                StrValidita_Fine = DT_Macrousi.Rows(i).Item("Validita_Fine")

                'Formatto correttamente
                If StrValidita_Inizio = "..." Then
                    Validita_Inizio = #1/1/1900#
                Else
                    Validita_Inizio = CDate(StrValidita_Inizio)
                End If

                If StrValidita_Fine = "..." Then
                    Validita_Fine = #12/31/2100#
                Else
                    Validita_Fine = CDate(StrValidita_Fine)
                End If

                '-----
                '' di default metto la superfice del macrouso a 0
                Dim objMacPar As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_W
                IntDummy = objMacPar.Scrivi(xPiva,
                                            CStr(Prov),
                                            CStr(Com),
                                            CStr(Sezione),
                                            CInt(Foglio),
                                            CInt(Numero),
                                            CStr(Subalterno),
                                            MacrousoCod,
                                            SupMacrouso,
                                            CDate(Validita_Inizio),
                                            CDate(Validita_Fine),
                                            objParametri_Server)

            Next





            '------------------------------------------------
            'distruggo gli oggetti 
            ObjCOMIP2_W = Nothing
            ObjCOMP_W = Nothing
            ObjCOMP_R = Nothing




            ''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


            ''------------------------------------------------
            ''----- Validazione dati inseriti
            ''------------------------------------------------

            ''NOTA
            ''La routine seguenti verifica se l'utente dispone dei permessi
            ''di validazione dei dati.
            ''In caso negativo imposta a (-1) il flag di validazione
            ''dell'impresa e invia un messaggio all'Ufficio Segreteria Tecnica

            'Call Gestione_Validazione_Dati(TipoOperazioneDB, _
            '                                Piva, _
            '                                AgroLabel_Particella & " ", _
            '                                1, _
            '                                "", _
            '                                Session, _
            '                                Server)


            ''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


            '------------------------------------------------
            '----- Conferma di aggiornamento del database
            '------------------------------------------------

            'EseguitaOperazione = True
            '============================
            '===  Fine Aggiornamento  ===
            '============================
            'chiudo la transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            'chiudo la connessione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)


            ''Se ho scelto SALVA E CONTINUA...
            'If Me.tipo_salva.Value = "1" Then

            '    SalvaContinua = True

            '    '---------------------------------------------------
            '    'svuoto alcuni controlli...

            '    Txt_Numero.Text = ""
            '    Txt_Subalterno.Text = ""

            '    TxtSup_Ettari.Text = ""
            '    TxtSup_Are.Text = ""
            '    TxtSup_Centiare.Text = ""

            '    ViewState("vs_dtClassamento") = Nothing
            '    ViewState("vs_dtPossesso") = Nothing
            '    ViewState("vs_dtZone") = Nothing

            '    'CaricaGriglia_Possesso(True)
            '    'CaricaGriglia_Classamento(True)
            '    'CaricaGriglia_Zone(True)

            '    '---------------------------------------------------
            '    'messaggio...........
            '    Messaggio = "Salvataggio avvenuto con successo!" & vbCrLf & vbCrLf & "ATTENZIONE!" & vbCrLf
            '    Messaggio += "E' stato selezionato il SALVA E CONTINUA, perciò rimarranno visualizzati a schermo i dati precedentemente inseriti." & vbCrLf
            '    Messaggio += "Modificare e/o cancellare i dati necessari per il nuovo inserimento." & vbCrLf
            '    Messaggio += "Se non si devono inserire ulteriori particelle catastali, uscire dalla pagina premendo il pulsante EXIT. "

            '    Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanel_script, , True)

            'End If


        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------
            Errore = True
            'chiudo la transazione con il rollback
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            'chiudo la connessione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

            'Messaggio di errore
            StrDummy = exc.Message.ToString()

            'Se la transazione no ha avuto esito positivo allora ...
            AgroMsgBox(AgronicaAgenda_2010.SiÈVerificatoUnErroreDuranteLaFaseDiSalvat & vbCrLf & StrDummy, Page)

            '------------------------------------------------

        End Try

        '============================
        '===  Fine Aggiornamento  ===
        '============================
        If Errore = False Then
            'Dim Piva As String

            'Ritorno alla pagina AlberoImprese
            'Response.Redirect(Qs_PaginaRitorno & "?P=" & Piva)

            Dim Messaggio2 As String
            Messaggio2 = DirectCast(GetLocalResourceObject("PARTICELLACATASTALESalvataConSuccesso"), String) & vbCrLf & vbCrLf

            Dim TargetUrl As String
            Select Case tipo_salva.Value

                Case 1
                    Messaggio2 += AgronicaAgenda_2010.VerràRicaricataLaPaginaPerInserimento & vbCrLf
                    AgroMsgBox(Messaggio2, Page)

                    ' sul salva e continua uso l'impostazione per il default su nuova particella
                    Dim objImpostazioni_Utenti As New Utenti_Impostazioni_Read
                    Dim DefaultParticella = objImpostazioni_Utenti.ImpostazioneValore1_from_ImpostazioneCod(
                        enum_Impostazioni_Utenti.UTENTE_NUOVA_PARTICELLA, objParametri_Utenti, 1)

                    If DefaultParticella <> "" Then
                        HttpContext.Current.Session("default_particella") = DefaultParticella
                        HttpContext.Current.Session("provincia_particella") = Prov
                        HttpContext.Current.Session("comune_particella") = Com
                        If DefaultParticella = "2" Then
                            HttpContext.Current.Session("sezione_particella") = Sezione
                            HttpContext.Current.Session("foglio_particella") = Foglio
                        End If
                    End If

                    Response.Redirect(HttpContext.Current.Request.Url.ToString(), True)
                    'Page_Load(Nothing, EventArgs.Empty)
                    'clear_form()
                Case Else
                    'Messaggi.AgroMsgBox(Messaggio2, Page, , UpdatePanel_script, , True)

                    TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda)
                    'Response.Redirect(TargetUrl)
                    If Qs_Visibilita <> 0 Then
                        TargetUrl &= "?visibilita=" & CStr(Qs_Visibilita)
                    End If
                    AgroMsgBox(Messaggio2, Page, , , "window.location = '" & TargetUrl & "';")

            End Select

        End If

        'If Errore = False Then

        '    'Dim Piva As String
        '    Dim Chiave As String

        '    If Not SalvaContinua Then

        '        ' '' 'Recupero  la partita IVA dalla querystring
        '        ' '' Piva = xPiva
        '        ' '' Chiave = xChiave

        '        ' '' objParametriAgenda.Particelle(0).

        '        ' '' 'Codifico la partita IVA
        '        ' '' Piva = Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server)
        '        ' '' Chiave = Stringa_Codifica(Chiave, AgroKey_EncoderDecoder, Server)

        '        ' '' 'Ritorno alla pagina AlberoImprese
        '        ' '' 'TO DO'
        '        ' '' ''''''''Response.Redirect(objParametriAgenda.pa & "?p=" & Piva & "&k=" & Chiave)

        '        Dim TargetUrl As String
        '        TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda)

        '        Response.Redirect(TargetUrl)

        '    End If

        'End If




    End Sub


    Private Sub clear_form()

        Cmb_Provincia.ClearSelection()
        Cmb_Comune.ClearSelection()
        Txt_Sezione.Text = ""
        Txt_Foglio.Text = ""
        Txt_Numero.Text = ""
        Txt_Subalterno.Text = ""
        TxtSup_Ettari.Text = ""
        TxtSup_Are.Text = ""
        TxtSup_Centiare.Text = ""
        Cmb_TitoloPossesso.ClearSelection()
        Txt_SupCondotta.Text = ""
        TxtValiditaInizio.Text = ""
        TxtValiditaFine.Text = ""

        Cmb_Macrousi.ClearSelection()
        Txt_SupMacrouso.Text = ""
        TxtValiditaInizioMacrouso.Text = ""
        TxtValiditaFineMacrouso.Text = ""
        TxtValiditaFine.Text = ""

        Cmb_Zone.ClearSelection()
        Txt_SupZona.Text = ""
        TxtPorzione.Text = ""
        TxtSupClass.Text = ""
        Cmb_QualitaCatasto.ClearSelection()
        TxtClasseCatasto.Text = ""
        TxtRedditoDom.Text = ""
        TxtRedditoAgr.Text = ""

    End Sub


#Region "TAB"

    '############################################################################################################
    '##########     Funzioni per TAB     ########################################################################
    '############################################################################################################

    '//la funzione controlla quale tab è stato selezionato prima del postback e ritorna l'indice
    '//salvato lato client in un campo nascosto prima del postback
    Public Function CalledFromClient_SetIndexTab() As String
        If (clickedTabUI.Value.Equals("")) Then
            Return "0"
        Else
            Return clickedTabUI.Value
        End If
    End Function


    'evento associato ad un btn non visualizzato, viene richiamato il click lato js che scatena quindi il postback
    Protected Sub Wrap_Client_PostedBack(ByVal sender As Object, ByVal e As EventArgs)
        If (Not fooName_PostedBack.Value.Equals("")) Then
            'nel campo hidden ho il nome di funzione da richiamare per il postback
            Dim metodo As System.Reflection.MethodInfo = Me.GetType().GetMethod(fooName_PostedBack.Value)
            If (Not IsNothing(metodo)) Then
                Dim objParam() As Object = {Me, EventArgs.Empty}
                metodo.Invoke(Me, objParam)
            End If
            'svuoto
            fooName_PostedBack.Value = ""
        End If
    End Sub

    Private Sub SelezionaTab(ByVal Controllo As System.Web.UI.Control, ByVal progressivoABaseZero As Integer)

        'If tabsCnt.Visible = False AndAlso progressivoABaseZero <> 0 Then
        '    progressivoABaseZero -= 1
        'End If


        'Dim stb As New StringBuilder

        'stb.AppendLine("$(function() { ")
        'stb.AppendLine("    $('#" & tabsCnt.ClientID & "').tabs({ ")
        'stb.AppendLine("        active: " & progressivoABaseZero)
        'stb.AppendLine("    }); ")
        'stb.AppendLine("}); ")

        'ScriptManager.RegisterClientScriptBlock(Controllo, Controllo.GetType(),
        '                                        String.Format("jQuery_{0}", "spostati"), stb.ToString, True)

    End Sub


#End Region

    Private Function GridView_Classamento() As Object
        Throw New NotImplementedException
    End Function



    Private Class PCatastaleMetodoProduzioneModel
        Public MetodoProduzione_Cod As Integer
        Public MetodoProduzione_Des As String
        Public Validita_Inizio As DateTime
        Public Validita_Fine As DateTime
    End Class

End Class