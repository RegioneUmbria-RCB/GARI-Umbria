Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.Services
Imports AgroAgenda_2010.Resources
Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModello
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreUtility.CaricaListControl
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq

Partial Class Appezzamento_Edit
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda

    Dim Qs_Visibilita As Integer = 0

    Public Operazione As Integer
    Dim xPiva As String
    Dim xSa_Cod As String
    Dim xAppezza As String
    Dim xCampo_Cod As String

    Public permessi As PermessiUtente

    Public jsCodici As String
    Public jsUtilizzo As String
    Public jsParticelle As String
    Public jsIndirizzi As String


    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    ' ----------------------------------------------
    ' se la voce non è già inserita la aggiungo al DT degli utilizzi e lo invio come risposta Json
    ' 
    ' ----------------------------------------------
    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiungi_Utilizzo(ByVal codice_id As String, ByVal testo As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Dt As New DataTable
        Dim Dr As DataRow
        'Dim Contatore As Integer
        Dim flag As Boolean = True


        If (IsNothing(HttpContext.Current.Session("dt_Utilizzo"))) Then
            Dt.Columns.Add(New DataColumn("id_cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("val_cod", GetType(String)))
        Else
            Dt = HttpContext.Current.Session("dt_Utilizzo")
        End If


        'Contatore = 1
        'Dim i As Integer
        'Dim objCodiceAnagrafeR As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

        'For i = 0 To Dt.Rows.Count - 1
        '    Contatore = Contatore + 1
        'Next




        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1

            If (Dt.Rows(i).Item("id_cod") = codice_id) Then
                flag = False
            End If
        Next


        If (flag) Then

            'Creo una nuova riga
            Dr = Dt.NewRow

            'Definisco i valori

            'Dr.Item("Contatore") = Contatore


            Dr.Item("id_cod") = codice_id
            'Dr.Item("Descrizione") = objCodiceAnagrafeR.CodiceAnagrafeDes_from_CodiceAnagrafeCod(codice_id, HttpContext.Current.Session("ASG_objParametri_Server"))
            Dr.Item("val_cod") = testo
            'Dr.Item("Val_Cod") = valore


            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)
            HttpContext.Current.Session("dt_Utilizzo") = Dt
            Dim str_Risposta = DT_to_Json_Utilizzo(Dt)
            r.RispostaOK = True
            r.RispostaStringa = str_Risposta
        Else
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.CodiceGiàInserito
        End If

        Return r

    End Function

    ' ----------------------------------------------
    ' tolgo la voce dal DT degli utilizzo se presente, la risposta è sempre true
    ' 
    ' ----------------------------------------------
    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaUtilizzo(ByVal Id_Cod As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dt = HttpContext.Current.Session("dt_Utilizzo")

        'Controllo se esiste già la voce che si vuole togliere
        For i = 0 To Dt.Rows.Count - 1
            If (Dt.Rows(i).Item("Id_Cod") = Id_Cod) Then
                Dt.Rows.RemoveAt(i)
                Exit For
            End If
        Next
        HttpContext.Current.Session("dt_Utilizzo") = Dt
        Dim str_Risposta = DT_to_Json_Utilizzo(Dt)
        r.RispostaOK = True
        r.RispostaStringa = str_Risposta
        Return r
    End Function


    ' ----------------------------------------------
    ' se la voce non è già inserita la aggiungo al DT dei codici_anagrafe e lo invio come risposta Json
    ' 
    ' ----------------------------------------------
    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiungi_Codice(ByVal codice As String, ByVal valore As String, ByVal codice_id As String, ByVal DataInizio As String, ByVal DataFine As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Dt As New DataTable
        Dim Dr As DataRow
        'Dim Contatore As Integer
        Dim flag As Boolean = True




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
        'Dim objCodiceAnagrafeR As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

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

    ' ----------------------------------------------
    ' Elimino la voce all'interno del DT dei codici_anagrafe se presente, la risposta è sempre TRUE
    ' 
    ' ----------------------------------------------
    '##########################################################################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaCodice(ByVal Id_Cod As String) As RispostaStandard
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



    ' ----------------------------------------------
    ' Trasformo il DT dei codici anagrafe associati alla particella in un JSON valido per il caricamento lato client con le modifiche necessarie
    ' per poter essere interpretato correttamente all'interno della pagina
    ' ----------------------------------------------
    '##########################################################################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Codici(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

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
            'TODO da reinserire
            'listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaCodice(this);"))
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

        cn = New ColonneNome("descrizione", AgronicaAgenda_2010.Codice, "String")
        l.Add(cn)

        cn = New ColonneNome("Val_Cod", AgronicaAgenda_2010.Valore, "String")
        l.Add(cn)

        cn = New ColonneNome("Validita_Inizio", AgronicaAgenda_2010.Dal, "String")
        l.Add(cn)

        cn = New ColonneNome("Validita_Fine", AgronicaAgenda_2010.Al, "String")
        l.Add(cn)



        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    ' ----------------------------------------------
    ' Trasformo il DT dei codici anagrafe associati alla particella in un JSON valido per il caricamento lato client con le modifiche necessarie
    ' per poter essere interpretato correttamente all'interno della pagina
    ' ----------------------------------------------
    '##########################################################################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Indirizzi(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)


        Dim c = New ColonneNome("chiave", "chiave", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("cod_indirizzo", "cod_indirizzo", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Tipo_Indirizzo", "Tipo_Indirizzo", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Tipo_Indirizzo_Des", "Tipo Indirizzo", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("ind_des", AgronicaAgenda_2010.Indirizzo, "string")
        c._Editabile = True
        c._obbligatorio = True
        l.Add(c)

        c = New ColonneNome("frz_des", AgronicaAgenda_2010.Frazione, "string")
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("CAP", AgronicaAgenda_2010.CAP, "string")
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("pro_cod", "Prov.", "string")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("stato", "stato", "string")
        c._Editabile = True
        c._obbligatorio = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("stato_des", "stato_des", "string")
        c._Editabile = True
        c._obbligatorio = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("note", AgronicaAgenda_2010.Note, "string")
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("pro_cod_istat", "ISTAT Prov", "string")
        'c._hidden = True
        c._Editabile = True
        c._obbligatorio = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("com_des", "Comune", "string")
        c._Editabile = True
        c._obbligatorio = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("com_cod_istat", "ISTAT Com", "string")
        c._hidden = True
        c._obbligatorio = True
        c._Editabile = True
        l.Add(c)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l)
        Return risp
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaIndirizzi(kendoGrid As String) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            Dim kendoRows = JArray.Parse(kendoGrid)
            Dim dtIndirizzi As New DataTable
            Crea_Dt_Indirizzi(dtIndirizzi)
            For Each row In kendoRows
                Dim dr = dtIndirizzi.NewRow()
                dr("chiave") = row("chiave")
                dr("cod_indirizzo") = row("cod_indirizzo")
                dr("Tipo_Indirizzo") = row("Tipo_Indirizzo")
                dr("ind_des") = row("ind_des")
                dr("frz_des") = row("frz_des")
                dr("com_des") = row("com_des")
                dr("pro_cod") = row("pro_cod")
                dr("stato") = row("stato")
                dr("note") = row("note")
                dr("stato_des") = row("stato_des")
                dr("pro_cod_istat") = row("pro_cod_istat")
                dr("com_cod_istat") = row("com_cod_istat")
                dr("CAP") = row("CAP")


                dtIndirizzi.Rows.Add(dr)
            Next

            HttpContext.Current.Session("dtAppezzamentixIndirizzi") = dtIndirizzi

            r.RispostaStringa = DT_to_Json_Indirizzi(dtIndirizzi)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                        Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    Public Shared Sub Crea_Dt_Indirizzi(ByRef DT As DataTable)

        DT.Columns.Add(New DataColumn("chiave", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Cod_Indirizzo", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Tipo_Indirizzo", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Tipo_Indirizzo_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("ind_des", GetType(String)))
        DT.Columns.Add(New DataColumn("frz_des", GetType(String)))
        DT.Columns.Add(New DataColumn("com_des", GetType(String)))
        DT.Columns.Add(New DataColumn("pro_cod", GetType(String)))
        DT.Columns.Add(New DataColumn("stato", GetType(String)))
        DT.Columns.Add(New DataColumn("stato_des", GetType(String)))
        DT.Columns.Add(New DataColumn("note", GetType(String)))
        DT.Columns.Add(New DataColumn("pro_cod_istat", GetType(String)))
        DT.Columns.Add(New DataColumn("com_cod_istat", GetType(String)))
        DT.Columns.Add(New DataColumn("CAP", GetType(String)))

    End Sub


    Public Sub Popola_Dt_Indirizzi(Piva As String, Sa_Cod As Integer, Appezza As Integer)
        Dim objAppezzaxIndirizzi As New AgronicaCoreAnagrafeDAL.AppezzamentixIndirizzi_Read

        Dim dt = objAppezzaxIndirizzi.Leggi(Piva, Sa_Cod, Appezza, 0, "", "", objParametri_Server)
        dt.Columns.Add(New DataColumn("chiave", GetType(Integer)))

        Dim chiave = 1
        For Each row In dt.Rows
            row("chiave") = chiave
            chiave += 1
        Next

        Session("dtAppezzamentixIndirizzi") = dt

        jsIndirizzi = DT_to_Json_Indirizzi(dt)

    End Sub


    ' ----------------------------------------------
    ' Trasformo il DT degli utilizzi in un JSON valido per il caricamento lato client con le modifiche necessarie
    ' per poter essere interpretato correttamente all'interno della pagina
    ' ----------------------------------------------
    '##########################################################################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Utilizzo(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Id_Cod", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaUtilizzo(this);"))
        End If

        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)

        Dim cn As New ColonneNome("Id_Cod", AgronicaAgenda_2010.Codice, "string")
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("Val_Cod", AgronicaAgenda_2010.Valore, "string")
        l.Add(cn)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    Public Shared Function CaricaGriglia_Particelle_xJSON(ByVal dtParam As DataTable, ByVal flag_Macrouso As Boolean, ByVal flag_Utilizzo As Boolean, ByVal flag_Varieta As Boolean) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("chiave", "kendoKey", "String")
        c._hidden = True
        l.Add(c)


        c = New ColonneNome("ChkSelezionaParticella", "check", "bool")
        c._Display = False
        l.Add(c)

        c = New ColonneNome("CodiceIstat_Provincia", "ISTAT PROV", "String")
        c._Display = False
        l.Add(c)

        c = New ColonneNome("CodiceIstat_Comune", "ISTAT COM", "String")
        c._Display = False
        l.Add(c)
        c = New ColonneNome("Part_Cod", "cod part", "number")
        c._Display = False
        l.Add(c)
        c = New ColonneNome("Progressivo", "progressivo", "number")
        c._Display = False
        l.Add(c)

        c = New ColonneNome("SuperficieImpiegata", "Superficie Impiegata", "number")
        c._Editabile = True
        'c._css = "Sup_Imp"
        c._formatNr = "n5"
        c._hidden = True 'la colonna viene costruita lato client
        l.Add(c)

        c = New ColonneNome("Provincia", AgronicaAgenda_2010.Provincia, "String")
        l.Add(c)
        c = New ColonneNome("Comune", AgronicaAgenda_2010.Comune, "String")
        l.Add(c)
        c = New ColonneNome("Sezione", AgronicaAgenda_2010.Sezione, "String")
        l.Add(c)
        c = New ColonneNome("Foglio", AgronicaAgenda_2010.Foglio, "String")
        l.Add(c)
        c = New ColonneNome("Numero", AgronicaAgenda_2010.Numero, "number")
        l.Add(c)
        c = New ColonneNome("Subalterno", AgronicaAgenda_2010.Subalterno, "String")
        l.Add(c)
        c = New ColonneNome("SuperficieCondottaDisponibile", AgronicaAgenda_2010.SuperficieCondottaDisponibileAbbr, "number")
        c._css = "Sup_Disponibile"
        c._formatNr = "n5"
        l.Add(c)
        c = New ColonneNome("SuperficieLorda", AgronicaAgenda_2010.SuperficieLorda, "number")
        l.Add(c)
        c = New ColonneNome("Superficie", AgronicaAgenda_2010.Superficie, "number")
        l.Add(c)

        If flag_Macrouso Then

            c = New ColonneNome("Macrouso_Cod", "Macrouso_Cod", "String")
            c._Display = False
            l.Add(c)
            c = New ColonneNome("Macrouso_Des", AgronicaAgenda_2010.Macrouso, "String")
            l.Add(c)
            c = New ColonneNome("Sup_Macrouso", AgronicaAgenda_2010.SuperficieMacrousoAbbr, "number")
            l.Add(c)
            c = New ColonneNome("SuperficieMacrousoDisponibile", AgronicaAgenda_2010.SuperficieMacrousoDisponibileAbbr, "number") 'TOCHECK
            l.Add(c)

        End If

        If flag_Utilizzo Then

            c = New ColonneNome("Veg_Cod_Agea", "Veg_Cod_Agea", "String")
            c._Display = False
            l.Add(c)
            c = New ColonneNome("Veg_Des_Agea", AgronicaAgenda_2010.SpecieAgea, "String")
            l.Add(c)
            If flag_Varieta Then

                c = New ColonneNome("Cul_Cod_Agea", "Cul_Cod_Agea", "String")
                c._Display = False
                l.Add(c)
                c = New ColonneNome("Cul_Des_Agea", AgronicaAgenda_2010.VarietàAgea, "String")
                l.Add(c)
            End If
            c = New ColonneNome("Sup_Utilizzo", AgronicaAgenda_2010.SuperficieUtilizzoAbbr, "number")
            l.Add(c)
            c = New ColonneNome("SuperficieUtilizzoDisponibile", AgronicaAgenda_2010.SuperficieUtilizzoDisponibileAbbr, "number")
            l.Add(c)
        End If


        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dtParam, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=False, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa)


        Return risp

    End Function


    Public Sub Salva_Particelle(ByVal type As Integer, ByVal dataInizio As Date, ByVal dataFine As Date)

        Dim objParametriAgenda As New ParametriAgenda

        Dim chiaveP_hash As String
        Dim Particelle_hash As New Hashtable
        Dim chiaveM_hash As String
        Dim Macrousi_hash As New Hashtable
        Dim chiaveU_hash As String
        Dim Utilizzi_hash As New Hashtable


        Dim Dt_Particelle As New DataTable

        Dim DataValiditaInizio As Date = dataInizio
        Dim DataValiditaFine As Date = dataFine


        Dim Macrouso_Cod As String
        Dim Sup_Macrouso As String
        Dim Veg_Cod_Agea As String
        Dim Cul_Cod_Agea As String
        Dim Sup_Utilizzo As String

        Dim SuperficieIntersezione As Double
        Dim SuperficieIntersezioneMacrouso As Double

        Dim Ettari As Integer
        Dim Are, Centiare As Integer

        Dim intDummy As Integer

        Dim ObjPartW As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W
        Dim ObjPartMacrW As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousi_W
        Dim ObjPartMacrUtW As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_W

        Dim list_part As List(Of Particella_Anagrafica) = GetParticelleKendo()


        If list_part.Count > 0 Then
            ' divido le chiavi per ogni particella selezionata per il salvataggio 

            'arr_chiave = Split(chiave, "-")

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(dataInizio, dataFine)

            ' scarico tutte le particelle a disposizione (a seconda se ci sono macrousi e utilizzi o no)
            If objParametriAgenda.Campo_Cod = "0" Then

                Dim objParticelleCatastali_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R

                Select Case type
                    Case 0 ' nessun check selezionato
                        'Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), DataValiditaInizio, DataValiditaFine, HttpContext.Current.Session("ASG_objParametri_Server"))
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), DataValiditaInizio, DataValiditaFine, objParametri_Server)
                    Case 1 ' macrousi ok
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_Macrousi(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), DataValiditaInizio, DataValiditaFine, objParametri_Server)
                    Case 2, 3, 4 ' utilizzi ok ' entrambi ok ' entrambi ok + varietà
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), DataValiditaInizio, DataValiditaFine, objParametri_Server)
                    Case Else
                        Throw New Exception(AgronicaAgenda_2010.TipoRecuperoParticelleNonGestito)
                End Select
            Else

                Dim objCampixParticelleR As New AgronicaCoreAnagrafeDAL.CampixParticelle_R

                If objCampixParticelleR.Campo_Definito_Come_Squadro(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), CInt(objParametriAgenda.Campo_Cod), objParametri_Server) = True Then

                    Dim objParticelleCatastali_R As New AgronicaCoreAnagrafeDAL.CampixParticelle_R

                    Select Case type
                        Case 0 ' nessun check selezionato
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), objParametriAgenda.Campo_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
                        Case 1 ' macrousi ok
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro_con_Macrousi(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), objParametriAgenda.Campo_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
                        Case 2, 3, 4 ' utilizzi ok ' entrambi ok  ' entrambi ok + varietà
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro_con_MacrousiUtilizzo(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), objParametriAgenda.Campo_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
                        Case Else
                            Throw New Exception(AgronicaAgenda_2010.TipoRecuperoParticelleNonGestito)
                    End Select
                Else
                    Dim objParticelleCatastali_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R

                    Select Case type
                        Case 0 ' nessun check selezionato
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), DataValiditaInizio, DataValiditaFine, objParametri_Server)
                        Case 1 ' macrousi ok
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_Macrousi(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), DataValiditaInizio, DataValiditaFine, objParametri_Server)
                        Case 2, 3, 4 ' utilizzi ok ' entrambi ok ' entrambi ok + varietà
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), DataValiditaInizio, DataValiditaFine, objParametri_Server)
                    End Select
                End If
            End If

            Dim list_part2 = list_part
            ' ciclo su righe particelle
            For Each particella In list_part


                'creo la stringa CHIAVE
                chiaveP_hash = particella.CodiceIstat_Provincia & "|" & particella.CodiceIstat_Comune & "|" & particella.Sezione & "|" & particella.Foglio.ToString & "|" & particella.Numero.ToString & "|" & particella.Subalterno

                If Not Particelle_hash.ContainsKey(chiaveP_hash) Then

                    For Each particella2 In list_part2

                        If particella.CodiceIstat_Provincia = particella2.CodiceIstat_Provincia AndAlso
                            particella.CodiceIstat_Comune = particella2.CodiceIstat_Comune AndAlso
                            particella.Sezione = particella2.Sezione AndAlso
                            particella.Foglio = particella2.Foglio AndAlso
                            particella.Numero = particella2.Numero AndAlso
                            particella.Subalterno = particella2.Subalterno Then

                            If IsNumeric(particella2.SuperficieImpiegata) Then

                                SuperficieIntersezioneMacrouso = particella2.SuperficieImpiegata
                                SuperficieIntersezione += SuperficieIntersezioneMacrouso


                                ' Se è abilitato Macrousi
                                If (type = 1) OrElse (type = 3) OrElse (type = 4) Then
                                    Macrouso_Cod = particella2.Macrouso_Cod.ToString
                                    Sup_Macrouso = particella2.Sup_Macrouso.ToString
                                End If

                                ' Se è abilitato Utilizzi
                                If (type = 2) OrElse (type = 3) OrElse (type = 4) Then
                                    Cul_Cod_Agea = particella2.Cul_Cod_Agea.ToString
                                    Sup_Utilizzo = particella2.Sup_Utilizzo.ToString
                                End If

                                If (type = 4) Then
                                    Veg_Cod_Agea = particella2.Veg_Cod_Agea.ToString
                                End If


                                If type <> 0 Then

                                    chiaveM_hash = particella2.CodiceIstat_Provincia & "|" & particella2.CodiceIstat_Comune & "|" & particella2.Sezione & "|" & particella2.Foglio.ToString & "|" & particella2.Numero.ToString & "|" & particella2.Subalterno & "|" & Macrouso_Cod & "|" & Sup_Macrouso '& "|" & SuperficieIntersezioneMacrouso
                                    chiaveU_hash = particella2.CodiceIstat_Provincia & "|" & particella2.CodiceIstat_Comune & "|" & particella2.Sezione & "|" & particella2.Foglio.ToString & "|" & particella2.Numero.ToString & "|" & particella2.Subalterno & "|" & Macrouso_Cod & "|" & Sup_Macrouso & "|" & Veg_Cod_Agea & "|" & Cul_Cod_Agea & "|" & Sup_Utilizzo

                                    If Macrouso_Cod <> "" AndAlso Macrouso_Cod <> "&nbsp;" AndAlso Not Macrousi_hash.ContainsKey(chiaveM_hash) Then

                                        Macrousi_hash.Add(chiaveM_hash, "")

                                        intDummy = ObjPartMacrW.Scrivi(
                                                                CStr(objParametriAgenda.Piva),
                                                                CInt(objParametriAgenda.Sa_Cod),
                                                                CInt(objParametriAgenda.Appezza),
                                                                CStr(particella2.CodiceIstat_Provincia),
                                                                CStr(particella2.CodiceIstat_Comune),
                                                                CStr(particella2.Sezione),
                                                                CInt(particella2.Foglio),
                                                                CInt(particella2.Numero),
                                                                CStr(particella2.Subalterno),
                                                                Macrouso_Cod,
                                                                CDbl(SuperficieIntersezioneMacrouso),
                                                                DataValiditaInizio,
                                                                DataValiditaFine,
                                                                objParametri_Server)
                                    End If

                                    If Veg_Cod_Agea <> "" AndAlso Veg_Cod_Agea <> "&nbsp;" AndAlso Not Utilizzi_hash.ContainsKey(chiaveU_hash) Then

                                        Utilizzi_hash.Add(chiaveU_hash, "")

                                        intDummy = ObjPartMacrUtW.Scrivi(
                                                                CStr(objParametriAgenda.Piva),
                                                                CInt(objParametriAgenda.Sa_Cod),
                                                                CInt(objParametriAgenda.Appezza),
                                                                CStr(particella2.CodiceIstat_Provincia),
                                                                CStr(particella2.CodiceIstat_Comune),
                                                                CStr(particella2.Sezione),
                                                                CInt(particella2.Foglio),
                                                                CInt(particella2.Numero),
                                                                CStr(particella2.Subalterno),
                                                                Macrouso_Cod,
                                                                Veg_Cod_Agea,
                                                                Cul_Cod_Agea,
                                                                CDbl(SuperficieIntersezioneMacrouso),
                                                                DataValiditaInizio,
                                                                DataValiditaFine,
                                                                objParametri_Server)
                                    End If

                                End If

                            End If
                        End If

                    Next

                    If SuperficieIntersezione <> 0 Then

                        Call EttariAreCentiare_from_Ettari(SuperficieIntersezione, Ettari, Are, Centiare)

                        intDummy = ObjPartW.Scrivi(
                                        CStr(objParametriAgenda.Piva),
                                        CInt(objParametriAgenda.Sa_Cod),
                                        CInt(objParametriAgenda.Appezza),
                                        CStr(particella.CodiceIstat_Provincia),
                                        CStr(particella.CodiceIstat_Comune),
                                        CStr(particella.Sezione),
                                        CInt(particella.Foglio),
                                        CInt(particella.Numero),
                                        CStr(particella.Subalterno),
                                        CDbl(SuperficieIntersezione),
                                        CDbl(Ettari),
                                        CInt(Are),
                                        CInt(Centiare),
                                        CDbl(0),
                                        CInt(0),
                                        CInt(0),
                                        CDbl(0),
                                        CInt(0),
                                        CInt(0),
                                        DataValiditaInizio,
                                        DataValiditaFine,
                                        objParametri_Server)
                        SuperficieIntersezione = 0
                    End If

                End If

            Next


            objParametri_Server.ResettaFinestra()

        End If ' end chiave <> 0

    End Sub


    Public Function GetParticelleKendo() As List(Of Particella_Anagrafica)
        Dim jss = New JavaScriptSerializer()
        Return jss.Deserialize(Of List(Of Particella_Anagrafica))(hidden_pushedParticelleDaSalvare.Value)
    End Function


    '########################################################################################
    ''' <summary>
    '''     Carico tutte le particelle e se sono in modifica anche quelle selezionate con le superfici associate.
    '''     Metto tutto in un JSon per la rappresentazione
    '''     Oltre ai parametri dell'appezzamento mi interessa sapere la datainizio e fine appezza per filtrare correttamente le particelle e poi anche la combo marcousi ed utilizzi
    ''' </summary>
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaKendo_Particelle(ByVal paramPart As String) As RispostaStandard
        Dim r As New RispostaStandard

        'VAriabili da gestire
        'Dim DataGridParticelle As DataGrid
        'Dim Cmb_Macrousi As ListBox
        'Dim Cmb_Utilizzo1 As ListBox

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")


        Dim Campo_Cod As Integer
        Dim Appezza As Integer
        Dim flag_Macrousi As Boolean
        Dim flag_Utilizzi As Boolean
        Dim flag_Varieta As Boolean
        Dim DataValiditaInizio As String
        Dim DataValiditaFine As String
        Dim flag_Modifica As Boolean

        Dim jSonParametri As JObject = JObject.Parse(paramPart)
        Try

            Campo_Cod = jSonParametri("Campo_Cod").ToString
            Appezza = jSonParametri("Appezza").ToString
            flag_Macrousi = jSonParametri("flag_Macrousi").ToString
            flag_Utilizzi = jSonParametri("flag_Utilizzi").ToString
            flag_Varieta = jSonParametri("flag_Varieta").ToString
            DataValiditaInizio = jSonParametri("DataValiditaInizio").ToString
            DataValiditaFine = jSonParametri("DataValiditaFine").ToString
            flag_Modifica = jSonParametri("flag_Modifica").ToString
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Appezzamento_Edit.aspx",
                "ErroreDuranteLaLetturaDeiParametri"), String) & ": " & ex.Message.ToString
            Return r
        End Try

        Dim ValiditInizio As Date = CDate(DataValiditaInizio)
        Dim validitFine As Date = CDate(DataValiditaFine)

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Dim objParametriAgenda As New ParametriAgenda

        If IsNothing(objParametriAgenda) Then
            r.RispostaOK = False
            r.Errore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Appezzamento_Edit.aspx", "ErroreDuranteLaLetturaDeiParametri"), String)
            Return r
        End If
        Try

            r.RispostaOK = True

            'modifico la data dell oggetto objparametri
            Dim appDInizio As New Date
            appDInizio = objParametri_Server.FinestraTemporaleInizio
            Dim appDFine As New Date
            appDFine = objParametri_Server.FinestraTemporaleFine

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(ValiditInizio, validitFine)

            '----- Definizione delle variabili

            Dim Dt As New DataTable
            Dim Dr As DataRow
            Dim Progressivo As Integer

            Dim SuperficieTotaleUsata As Double

            Dim Sup_Condotta_Min As Double
            Dim SuperficieIntersezione As Double
            Dim SuperficieDisponibile As Double

            Dim Prov As String
            Dim Com As String
            Dim Sezione As String
            Dim Foglio As Integer
            Dim Numero As Integer
            Dim Subalterno As String
            Dim Macrouso_Cod As String
            Dim Macrouso_Des As String
            Dim Sup_Macrouso As String

            Dim Veg_Cod_Agea As String
            Dim Veg_Des_Agea As String
            Dim Cul_Cod_Agea As String
            Dim Cul_Des_Agea As String
            Dim Sup_Utilizzo As String

            Dim i, j As Integer

            Dim SuperficieIntersezioneTotale As Double

            Dim Squadro As Boolean = False

            '----- Definisco la struttura del DataTable
            Dt.Columns.Add(New DataColumn("chiave", GetType(String)))
            Dt.Columns.Add(New DataColumn("ChkSelezionaParticella", GetType(Boolean)))
            Dt.Columns.Add(New DataColumn("CodiceIstat_Provincia", GetType(String)))
            Dt.Columns.Add(New DataColumn("CodiceIstat_Comune", GetType(String)))
            Dt.Columns.Add(New DataColumn("Part_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Progressivo", GetType(String)))
            Dt.Columns.Add(New DataColumn("Provincia", GetType(String)))
            Dt.Columns.Add(New DataColumn("Comune", GetType(String)))
            Dt.Columns.Add(New DataColumn("Sezione", GetType(String)))
            Dt.Columns.Add(New DataColumn("Foglio", GetType(String)))
            Dt.Columns.Add(New DataColumn("Numero", GetType(String)))
            Dt.Columns.Add(New DataColumn("Subalterno", GetType(String)))
            Dt.Columns.Add(New DataColumn("SuperficieLorda", GetType(String)))
            Dt.Columns.Add(New DataColumn("Superficie", GetType(String)))
            Dt.Columns.Add(New DataColumn("SuperficieCondottaDisponibile", GetType(String)))
            Dt.Columns.Add(New DataColumn("SuperficieImpiegata", GetType(String)))

            Dt.Columns.Add(New DataColumn("Macrouso_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Macrouso_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("Sup_Macrouso", GetType(String)))
            Dt.Columns.Add(New DataColumn("SuperficieMacrousoDisponibile", GetType(String)))

            Dt.Columns.Add(New DataColumn("Veg_Cod_Agea", GetType(String)))
            Dt.Columns.Add(New DataColumn("Veg_Des_Agea", GetType(String)))
            Dt.Columns.Add(New DataColumn("Cul_Cod_Agea", GetType(String)))
            Dt.Columns.Add(New DataColumn("Cul_Des_Agea", GetType(String)))
            Dt.Columns.Add(New DataColumn("Sup_Utilizzo", GetType(String)))
            Dt.Columns.Add(New DataColumn("SuperficieUtilizzoDisponibile", GetType(String)))

            '----- Definisco l'insieme di colonne che costituiscono la chiave della tabella

            'Vettore di DataColumn
            Dim DtKeys(10) As DataColumn

            'Valorizzo le celle del vettore
            DtKeys(0) = Dt.Columns("Provincia")
            DtKeys(1) = Dt.Columns("Comune")
            DtKeys(2) = Dt.Columns("Sezione")
            DtKeys(3) = Dt.Columns("Foglio")
            DtKeys(4) = Dt.Columns("Numero")
            DtKeys(5) = Dt.Columns("Subalterno")
            DtKeys(6) = Dt.Columns("Macrouso_Cod")
            DtKeys(7) = Dt.Columns("Sup_Macrouso")
            DtKeys(8) = Dt.Columns("Veg_Cod_Agea")
            DtKeys(9) = Dt.Columns("Cul_Cod_Agea")
            DtKeys(10) = Dt.Columns("Sup_Utilizzo")


            'Assegno il vettore delle chiavi al DataTable
            Dt.PrimaryKey = DtKeys

            '----- Verifico quali particelle devo caricare
            Dim Dt_Particelle As New DataTable

            Dim Dt_Particella As New DataTable
            Dim DrParticella As DataRow()


            ' MACROUSI
            'Leggo i record della tabella AppezzamentoxParticellexMacrousi:
            '   - se contiene dei record metto il check sui macrousi e carico la combo;
            '   - se non contiene alcun record tolgo il check dai macrousi.

            Dim objAppxPartxMacr As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousi_R

            ' UTILIZZI
            'Leggo i record della tabella AppezzamentoxParticellexMacrousixUtilizzo:
            '   - se contiene dei record metto il check utilizzi e carico le combo;
            '   - se non contiene alcun record tolgo il check utilizzi.

            Dim objAppxPartxMacrxUtil As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_R

            'NOTA
            'Se l'appezzamento appartiene ad un campo allora verifico se il campo ha delle 
            'relazioni CAMPIxPARTICELLE valorizzate.
            'In caso affermativo ho una gestione del terreno a SQUADRI e presento solo le 
            'particelle che sono state relazionate con il campo.

            'In caso negativo il campo e' gestito solo come AGGREGATO di APPEZZAMENTI allora
            'visualizzo tutte le particelle del Centro Aziendale.

            'Se l'appezzamento non appartiene ad un campo, allora opero come nel caso dello
            'aggregato e visualizzo tutte le particelle del Centro Aziendale.

            'NOTA
            'Se APPEZZA <> 0 allora ricarico anche i valori attuali

            If Campo_Cod = 0 Then

                '### Tutte le particelle del Centro Aziendale ###
                '  Galassi, 21/04/2017 12.09.55: TODO
                'Lbl_NoteCatastoAppezzamento.Text = ""

                Dim objParticelleCatastali_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R

                If flag_Macrousi Then
                    If Not flag_Utilizzi Then
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_Macrousi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, ValiditInizio, validitFine, objParametri_Server)
                    Else
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, ValiditInizio, validitFine, objParametri_Server)
                    End If
                Else
                    If Not flag_Utilizzi Then
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, ValiditInizio, validitFine, objParametri_Server)
                    Else
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, ValiditInizio, validitFine, objParametri_Server)
                    End If
                End If

            Else

                Dim objCampixParticelleR As New AgronicaCoreAnagrafeDAL.CampixParticelle_R
                If objCampixParticelleR.Campo_Definito_Come_Squadro(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, Campo_Cod, objParametri_Server) = True Then

                    'Imposto la variabile Squadro 
                    Squadro = True

                    '### Particelle associate al Campo
                    'Me.Lbl_NoteCatastoAppezzamento.Text = _
                    '                "L'Appezzamento appartiene ad un Campo." & _
                    '                "<br>Per tale Campo e' stata definita una superficie catastale " & _
                    '                "percio' le particelle disponibili sono quelle assegnate al Campo !!!"
                    '  Galassi, 21/04/2017 12.10.18: 
                    'Lbl_NoteCatastoAppezzamento.Text = _
                    '                "L'Appezzamento appartiene ad un Campo per il quale e' stata definita una superficie catastale. Le particelle disponibili sono quelle assegnate al Campo !!!"
                    Dim objParticelleCatastali_R As New AgronicaCoreAnagrafeDAL.CampixParticelle_R
                    'If Chk_Macrousi.Checked Then
                    '    Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro_con_Macrousi(Piva, Sa_Cod, Campo_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
                    'Else
                    '    Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro(Piva, Sa_Cod, Campo_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
                    'End If

                    If flag_Macrousi Then
                        If Not flag_Utilizzi Then
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro_con_Macrousi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, Campo_Cod, ValiditInizio, validitFine, objParametri_Server)
                        Else
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro_con_MacrousiUtilizzo(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, Campo_Cod, ValiditInizio, validitFine, objParametri_Server)
                        End If
                    Else
                        If Not flag_Utilizzi Then
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, Campo_Cod, ValiditInizio, validitFine, objParametri_Server)
                        Else
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro_con_MacrousiUtilizzo(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, Campo_Cod, ValiditInizio, validitFine, objParametri_Server)
                        End If
                    End If
                Else

                    '### Tutte le particelle del Centro Aziendale ###
                    'Me.Lbl_NoteCatastoAppezzamento.Text = _
                    '            "L'Appezzamento appartiene ad un Campo." & _
                    '              "<br>Per tale Campo e' NON stata definita una superficie catastale " & _
                    '              "percio' le particelle disponibili sono quelle legate al Centro Aziendale !!!"
                    '  Galassi, 21/04/2017 12.10.32: 
                    'Lbl_NoteCatastoAppezzamento.Text = _
                    '                "L'Appezzamento appartiene ad un Campo per il quale NON e' stata definita una superficie catastale. Le particelle disponibili sono quelle assegnate al Centro Aziendale !!!"

                    Dim objParticelleCatastali_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R

                    If flag_Macrousi Then
                        If Not flag_Utilizzi Then
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_Macrousi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, ValiditInizio, validitFine, objParametri_Server)
                        Else
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, ValiditInizio, validitFine, objParametri_Server)
                        End If
                    Else
                        If Not flag_Utilizzi Then
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, ValiditInizio, validitFine, objParametri_Server)
                        Else
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, ValiditInizio, validitFine, objParametri_Server)
                        End If
                    End If


                End If

            End If


            '--------------------------------------------------------------------
            '--- HO TUTTE LE PARTICELLE --> CALCOLO X OGNUNA LA SUP.IMPIEGATA ---
            '--------------------------------------------------------------------

            Dim chiave_hash As String
            Dim table_hash As New Hashtable

            'Dim chiave_hash_macrousi As String
            'Dim table_hash_macrousi As New Hashtable

            'Dim chiave_hash_vegcod_agea As String
            'Dim table_hash_vegcod_agea As New Hashtable
            'Dim chiave_hash_culcod_agea As String

            'Cmb_Macrousi.Items.Clear()
            'Cmb_Utilizzo1.Items.Clear()

            'Se il recordset esiste ...
            If Not IsNothing(Dt_Particelle) Then

                Progressivo = 1

                For i = 0 To Dt_Particelle.Rows.Count - 1

                    Prov = Dt_Particelle.Rows(i).Item("prov")
                    Com = Dt_Particelle.Rows(i).Item("com")

                    If Dt_Particelle.Rows(i).Item("sezione") = "0" Then
                        Sezione = ""
                    Else
                        Sezione = Dt_Particelle.Rows(i).Item("sezione")
                    End If

                    Foglio = Dt_Particelle.Rows(i).Item("foglio")
                    Numero = Dt_Particelle.Rows(i).Item("numero")

                    If Dt_Particelle.Rows(i).Item("Subalterno") = "0" Then
                        Subalterno = ""
                    Else
                        Subalterno = Dt_Particelle.Rows(i).Item("subalterno")
                    End If



                    'RIEMPIO LA COMBO MACROUSI CON QUELLI GESTITI
                    If flag_Macrousi Then

                        'Dim cmb_index_m As Integer
                        'Dim cmb_flag_m As Boolean

                        Macrouso_Cod = Dt_Particelle.Rows(i).Item("Macrouso_Cod")
                        Macrouso_Des = Dt_Particelle.Rows(i).Item("Macrouso_Des")
                        Sup_Macrouso = Dt_Particelle.Rows(i).Item("Sup_Macrouso")

                        ''creo la stringa CHIAVE
                        'chiave_hash_macrousi = Macrouso_Cod

                        'If Not table_hash_macrousi.ContainsKey(chiave_hash_macrousi) Then
                        '    table_hash_macrousi.Add(Macrouso_Cod, Macrouso_Des)
                        '    If Macrouso_Cod <> "" Then

                        '        cmb_flag_m = False
                        '        For cmb_index_m = 0 To Cmb_Macrousi.Items.Count - 1

                        '            If Macrouso_Des < Cmb_Macrousi.Items(cmb_index_m).Text Then
                        '                Cmb_Macrousi.Items.Insert(cmb_index_m, New ListItem(Macrouso_Des, Macrouso_Cod))
                        '                cmb_flag_m = True
                        '                Exit For
                        '            End If

                        '        Next

                        '        If Not cmb_flag_m Then
                        '            Cmb_Macrousi.Items.Add(New ListItem(Macrouso_Des, Macrouso_Cod))
                        '        End If

                        '    Else
                        '        Cmb_Macrousi.Items.Insert(0, New ListItem(Macrouso_Des, Macrouso_Cod))
                        '    End If
                        'End If

                    Else
                        Macrouso_Cod = ""
                        Macrouso_Des = ""
                        Sup_Macrouso = ""
                    End If


                    'RIEMPIO LE COMBO UTILIZZI
                    If flag_Utilizzi Then

                        'Dim cmb_index_u As Integer
                        'Dim cmb_flag_u As Boolean

                        Macrouso_Cod = Dt_Particelle.Rows(i).Item("Macrouso_Cod")
                        Macrouso_Des = Dt_Particelle.Rows(i).Item("Macrouso_Des")
                        Sup_Macrouso = Dt_Particelle.Rows(i).Item("Sup_Macrouso")

                        Veg_Cod_Agea = Dt_Particelle.Rows(i).Item("Veg_Cod_agea")
                        Veg_Des_Agea = Dt_Particelle.Rows(i).Item("Veg_Des_agea")
                        Cul_Cod_Agea = Dt_Particelle.Rows(i).Item("Cul_Cod_agea")
                        Cul_Des_Agea = Dt_Particelle.Rows(i).Item("Cul_Des_agea")
                        Sup_Utilizzo = Dt_Particelle.Rows(i).Item("Sup_Utilizzo")

                        ''creo la stringa CHIAVE
                        'chiave_hash_vegcod_agea = Veg_Cod_Agea
                        'chiave_hash_culcod_agea = Cul_Cod_Agea

                        ''specie
                        'If Not table_hash_vegcod_agea.ContainsKey(chiave_hash_vegcod_agea) Then
                        '    table_hash_vegcod_agea.Add(Veg_Cod_Agea, Veg_Des_Agea)
                        '    If Veg_Cod_Agea <> "0" Then
                        '        cmb_flag_u = False
                        '        For cmb_index_u = 0 To Cmb_Utilizzo1.Items.Count - 1
                        '            If Veg_Des_Agea < Cmb_Utilizzo1.Items(cmb_index_u).Text Then
                        '                Cmb_Utilizzo1.Items.Insert(cmb_index_u, New ListItem(Veg_Des_Agea, Veg_Cod_Agea))
                        '                cmb_flag_u = True
                        '                Exit For
                        '            End If
                        '        Next
                        '        If Not cmb_flag_u Then
                        '            Cmb_Utilizzo1.Items.Add(New ListItem(Veg_Des_Agea, Veg_Cod_Agea))
                        '        End If
                        '    Else
                        '        Cmb_Utilizzo1.Items.Insert(0, New ListItem(Veg_Des_Agea, Veg_Cod_Agea))
                        '    End If
                        'End If

                    Else
                        Veg_Cod_Agea = ""
                        Veg_Des_Agea = ""
                        Cul_Cod_Agea = ""
                        Cul_Des_Agea = ""
                        Sup_Utilizzo = ""
                    End If



                    'creo la stringa CHIAVE
                    'chiave_hash = Prov & "|" & Com & "|" & Sezione & "|" & Foglio.ToString & "|" & Numero.ToString & "|" & Subalterno & "|" & Macrouso_Cod
                    'chiave_hash = Prov & "|" & Com & "|" & Sezione & "|" & Foglio.ToString & "|" & Numero.ToString & "|" & Subalterno & "|" & Macrouso_Cod & "|" & Veg_Cod_Agea & "|" & Cul_Cod_Agea
                    chiave_hash = Prov & "§" & Com & "§" & Sezione & "§" & Foglio.ToString & "§" & Numero.ToString & "§" & Subalterno & "§" & Macrouso_Cod & "§" & Veg_Cod_Agea & "§" & Cul_Cod_Agea

                    If Not table_hash.ContainsKey(chiave_hash) Then

                        table_hash.Add(chiave_hash, "")
                        DrParticella = Dt_Particelle.Select("prov='" & Dt_Particelle.Rows(i).Item("prov").ToString &
                                        "' AND com='" & Dt_Particelle.Rows(i).Item("com").ToString &
                                        "' AND sezione='" & Dt_Particelle.Rows(i).Item("sezione").ToString &
                                        "' AND foglio='" & Dt_Particelle.Rows(i).Item("foglio").ToString &
                                        "' AND numero='" & Dt_Particelle.Rows(i).Item("numero").ToString &
                                        "' AND subalterno='" & Dt_Particelle.Rows(i).Item("subalterno").ToString & "'")

                        Sup_Condotta_Min = 0

                        For j = 0 To DrParticella.Length - 1
                            If j = 0 Then
                                Sup_Condotta_Min = CDbl(DrParticella(j).Item("sup_condotta"))
                            Else
                                If CDbl(DrParticella(j).Item("sup_condotta")) <> 0 Then
                                    If Sup_Condotta_Min > CDbl(DrParticella(j).Item("sup_condotta")) Then
                                        Sup_Condotta_Min = CDbl(DrParticella(j).Item("sup_condotta"))
                                        ' SuperficieTotaleUsata = CDbl(DrParticella(j).Item("sup_condotta"))
                                    End If
                                End If
                            End If
                        Next

                        'Creo una nuova riga
                        Dr = Dt.NewRow

                        'Definisco i valori

                        '---
                        Dr.Item("chiave") = chiave_hash
                        Dr.Item("CodiceIstat_Provincia") = Dt_Particelle.Rows(i).Item("prov")
                        Dr.Item("CodiceIstat_Comune") = Dt_Particelle.Rows(i).Item("com")
                        Dr.Item("Part_Cod") = Dt_Particelle.Rows(i).Item("part_cod")

                        Dr.Item("Progressivo") = Progressivo

                        '-----

                        Dr.Item("Provincia") = Dt_Particelle.Rows(i).Item("COMUNI_PROV")
                        Dr.Item("Comune") = Dt_Particelle.Rows(i).Item("LOCALITA")

                        '-----

                        Dr.Item("Sezione") = IIf(Dt_Particelle.Rows(i).Item("Sezione") = "0", "", Dt_Particelle.Rows(i).Item("Sezione"))
                        Dr.Item("Foglio") = Dt_Particelle.Rows(i).Item("Foglio")
                        Dr.Item("Numero") = Dt_Particelle.Rows(i).Item("Numero")
                        Dr.Item("Subalterno") = IIf(Dt_Particelle.Rows(i).Item("Subalterno") = "0", "", Dt_Particelle.Rows(i).Item("Subalterno"))

                        '-----

                        Dr.Item("Superficie") = Format(Sup_Condotta_Min, "0.0000")

                        Dr.Item("SuperficieLorda") = Format(UtilityProvider.Ettari_from_EttariAreCentiare(CDbl(Dt_Particelle.Rows(i).Item("particella_ettari")),
                                                                                              CDbl(Dt_Particelle.Rows(i).Item("particella_are")),
                                                                                              CDbl(Dt_Particelle.Rows(i).Item("particella_centiare"))),
                                                                                              "0.0000")


                        '--------------------------------------------------------------------------
                        ' Se l'Appezzamento appartiene ad un campo SQUADRO
                        ' l'area della particella utilizzata è
                        ' la somma delle intersezioni degli altri appezzamenti aggregati al campo!
                        '--------------------------------------------------------------------------

                        '--------------------------------------------------------------------------
                        ' Se l'Appezzamento appartiene ad un campo NON SQUADRO
                        ' o NON appartiene a campi
                        ' l'area della particella utilizzata è
                        ' la somma delle intersezioni con gli altri campi squadri + 
                        ' l'area di intersezione con gli appezzamenti non aggregati in campo!
                        '--------------------------------------------------------------------------

                        Dim sezioneMacr As String
                        Dim subMacr As String

                        If Dt_Particelle.Rows(i).Item("sezione").ToString = "" Then
                            sezioneMacr = "0"
                        Else
                            sezioneMacr = Dt_Particelle.Rows(i).Item("sezione").ToString
                        End If

                        If Dt_Particelle.Rows(i).Item("subalterno").ToString = "" Then
                            subMacr = "0"
                        Else
                            subMacr = Dt_Particelle.Rows(i).Item("subalterno").ToString
                        End If

                        If Macrouso_Cod <> "" Then

                            Dim DtSuperficie As DataTable

                            DtSuperficie = objAppxPartxMacr.Leggi_SuperficieMacrousoUtilizzata(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, 0,
                                                                                               Dt_Particelle.Rows(i).Item("PROV"),
                                                                                               Dt_Particelle.Rows(i).Item("COM"),
                                                                                               sezioneMacr,
                                                                                               CInt(Dt_Particelle.Rows(i).Item("Foglio")),
                                                                                               CInt(Dt_Particelle.Rows(i).Item("Numero")),
                                                                                               subMacr,
                                                                                               Macrouso_Cod,
                                                                                               "", "", objParametri_Server)

                            SuperficieDisponibile = Format(Sup_Macrouso, "0.0000")

                            If Not IsDBNull(DtSuperficie.Rows(0).Item("Superficie")) Then
                                SuperficieDisponibile -= DtSuperficie.Rows(0).Item("Superficie")
                            End If

                            Dr.Item("SuperficieMacrousoDisponibile") = Format(SuperficieDisponibile, "0.0000")

                        End If

                        If Veg_Des_Agea <> "" Then

                            Dim DtSuperficieUt As DataTable

                            DtSuperficieUt = objAppxPartxMacrxUtil.Leggi_SuperficieMacrousoxUtilizzo(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, 0,
                                                                                                Dt_Particelle.Rows(i).Item("PROV"),
                                                                                                Dt_Particelle.Rows(i).Item("COM"),
                                                                                                sezioneMacr,
                                                                                                CInt(Dt_Particelle.Rows(i).Item("Foglio")),
                                                                                                CInt(Dt_Particelle.Rows(i).Item("Numero")),
                                                                                                subMacr,
                                                                                                Macrouso_Cod,
                                                                                                Veg_Cod_Agea,
                                                                                                Cul_Cod_Agea,
                                                                                                "", "", objParametri_Server)

                            SuperficieDisponibile = Format(Sup_Utilizzo, "0.0000")

                            If Not IsDBNull(DtSuperficieUt.Rows(0).Item("Superficie")) Then
                                SuperficieDisponibile -= DtSuperficieUt.Rows(0).Item("Superficie")
                            End If

                            Dr.Item("SuperficieUtilizzoDisponibile") = Format(SuperficieDisponibile, "0.0000")

                        Else
                            Dr.Item("SuperficieUtilizzoDisponibile") = "0"
                        End If

                        If Squadro AndAlso Campo_Cod <> 0 Then

                            'Appezzamenti Aggregati allo squadro
                            SuperficieTotaleUsata = CDbl(Dt_Particelle.Rows(i).Item("SuperficieAppSquadro"))

                            'Calcolo quella disponibile
                            SuperficieDisponibile = CDbl(Sup_Condotta_Min) - SuperficieTotaleUsata

                            Dr.Item("SuperficieCondottaDisponibile") = Format(SuperficieDisponibile, "0.0000")

                        Else

                            SuperficieDisponibile = CDbl(Dt_Particelle.Rows(i).Item("SuperficieDisponibile"))

                            Dr.Item("SuperficieCondottaDisponibile") = Format(SuperficieDisponibile, "0.0000")

                        End If


                        'End If


                        '--------------------------------------------------------
                        '--------------------------------------------------------
                        '--------------------------------------------------------
                        '--------------------------------------------------------
                        '--------------------------------------------------------

                        Dr.Item("SuperficieImpiegata") = 0

                        Dr.Item("Macrouso_Cod") = Macrouso_Cod
                        Dr.Item("Macrouso_Des") = Macrouso_Des
                        Dr.Item("Sup_Macrouso") = Format(Sup_Macrouso, "0.0000")

                        Dr.Item("Veg_Cod_Agea") = Veg_Cod_Agea
                        If Cul_Des_Agea = "" Then
                            Dr.Item("Veg_Des_Agea") = Veg_Des_Agea
                        Else
                            Dr.Item("Veg_Des_Agea") = Veg_Des_Agea & " - " & Cul_Des_Agea
                        End If

                        Dr.Item("Cul_Cod_Agea") = Cul_Cod_Agea
                        Dr.Item("Cul_Des_Agea") = Cul_Des_Agea
                        Dr.Item("Sup_Utilizzo") = Format(Sup_Utilizzo, "0.0000")


                        '-----

                        'Associo alla tabella la nuova riga creata
                        Dt.Rows.Add(Dr)

                        'Aggiorno il contatore
                        Progressivo += 1

                    End If

                Next

                'Nel caso in cui non fosse già presente, aggiungo alla combo l'elemento per la selezione di
                'tutti i macrousi
                'If Cmb_Macrousi.Items.FindByValue("") Is Nothing Then
                '    Cmb_Macrousi.Items.Insert(0, New ListItem("", ""))
                'End If

            End If

            'MI INSERISCO QUI PER LA GENERAZIONE DELLE COLONNE DELLA KENDO



            ''----- Associo il DataTable con il DataGrid

            'DataGridParticelle.DataSource = Dt
            'DataGridParticelle.DataBind()

            'If flag_Macrousi = True Then

            '    Dim macrouso_cod_selezionato As String = ""
            '    If Not ViewState("macrouso_cod") Is Nothing Then
            '        macrouso_cod_selezionato = ViewState("macrouso_cod").ToString
            '        Select Case macrouso_cod_selezionato
            '            Case ""
            '            Case Else
            '                Dim Dt_Particelle_Filtrato As New DataTable
            '                Dim Dr_Filtrato() As DataRow
            '                Dt_Particelle_Filtrato = Dt.Clone
            '                Dr_Filtrato = Dt.Select("macrouso_cod='" & macrouso_cod_selezionato & "'")
            '                If Not Dr_Filtrato Is Nothing Then
            '                    For i = 0 To Dr_Filtrato.Length - 1
            '                        Dt_Particelle_Filtrato.ImportRow(Dr_Filtrato(i))
            '                    Next
            '                End If
            '                DataGridParticelle.DataSource = Dt_Particelle_Filtrato
            '                DataGridParticelle.DataBind()

            '                Cmb_Macrousi.SelectedIndex = _
            '                    Cmb_Macrousi.Items.IndexOf(Cmb_Macrousi.Items.FindByValue( _
            '                        macrouso_cod_selezionato))
            '        End Select
            '    End If

            '    DataGridParticelle.Columns(12).Visible = True
            '    DataGridParticelle.Columns(13).Visible = True
            '    DataGridParticelle.Columns(14).Visible = True
            '    Cmb_Macrousi.Enabled = True

            'Else

            '    DataGridParticelle.Columns(12).Visible = False
            '    DataGridParticelle.Columns(13).Visible = False
            '    DataGridParticelle.Columns(14).Visible = False
            '    Cmb_Macrousi.Enabled = False

            'End If



            'If flag_Utilizzi = True Then

            '    Dim veg_cod_agea_selezionato As String = ""
            '    Dim cul_cod_agea_selezionato As String = ""
            '    If Not ViewState("veg_cod_agea") Is Nothing Then
            '        veg_cod_agea_selezionato = ViewState("veg_cod_agea").ToString
            '        Select Case veg_cod_agea_selezionato
            '            Case ""
            '            Case Else
            '                Dim Dt_Particelle_Filtrato As New DataTable
            '                Dim Dr_Filtrato() As DataRow
            '                Dt_Particelle_Filtrato = Dt.Clone
            '                Dr_Filtrato = Dt.Select("veg_cod_agea='" & veg_cod_agea_selezionato & "'")
            '                If Not Dr_Filtrato Is Nothing Then
            '                    For i = 0 To Dr_Filtrato.Length - 1
            '                        Dt_Particelle_Filtrato.ImportRow(Dr_Filtrato(i))
            '                    Next
            '                End If
            '                DataGridParticelle.DataSource = Dt_Particelle_Filtrato
            '                DataGridParticelle.DataBind()

            '                Cmb_Utilizzo1.SelectedIndex = _
            '                    Cmb_Utilizzo1.Items.IndexOf(Cmb_Utilizzo1.Items.FindByValue( _
            '                        veg_cod_agea_selezionato))
            '        End Select
            '    End If

            '    DataGridParticelle.Columns(15).Visible = True
            '    DataGridParticelle.Columns(17).Visible = True
            '    DataGridParticelle.Columns(18).Visible = True
            '    Cmb_Utilizzo1.Enabled = True

            '    If flag_Varieta = True Then
            '        DataGridParticelle.Columns(16).Visible = True
            '    Else
            '        DataGridParticelle.Columns(16).Visible = False
            '    End If

            'Else

            '    DataGridParticelle.Columns(15).Visible = False
            '    DataGridParticelle.Columns(16).Visible = False
            '    DataGridParticelle.Columns(17).Visible = False
            '    DataGridParticelle.Columns(18).Visible = False
            '    Cmb_Utilizzo1.Enabled = False

            'End If



            'In questo momento la "SuperficieDisponibile" è solo parziale
            Dim Dt_SuperfInter As DataTable = Dt

            '//////////////////////////////////////////////////////////////////////////////////////////////
            ' MODIFICA
            '----- Per ciascuna particella ripristino i check e le sup di intersezione
            '//////////////////////////////////////////////////////////////////////////////////////////////

            If CInt(Appezza) <> 0 Then

                Dim objCOMAP As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R 'Object New Agro_Anagrafe_AD.AppezzaxParticelle_R
                Dim DTRsAP As DataTable
                Dim xy As Integer
                Dim HTAppezzamenti As New Hashtable()

                Dim DtMacr As New DataTable
                DtMacr = objAppxPartxMacr.Leggi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, Appezza, "", "", "", 0, 0, "", "", "", "", objParametri_Server)

                Dim DtUtil As New DataTable
                DtUtil = objAppxPartxMacrxUtil.Leggi(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, Appezza, "", "", "", 0, 0, "", "", "", "", "", "", objParametri_Server)

                'Cerco le intersezioni eventuali
                DTRsAP = objCOMAP.LeggiParticelle_Da_Appezzamento(
                                                        CStr(objParametriAgenda.Piva),
                                                        CInt(objParametriAgenda.Sa_Cod),
                                                        CInt(Appezza),
                                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                        "",
                                                        "", objParametri_Server)

                objCOMAP = Nothing

                'SE CI SONO UTILIZZI
                If flag_Utilizzi AndAlso DtUtil.Rows.Count > 0 Then

                    For xy = 0 To DtUtil.Rows.Count - 1

                        For i = 0 To Dt_SuperfInter.Rows.Count - 1

                            'Recupero la chiave
                            Prov = Dt_SuperfInter.Rows(i).Item("CodiceIstat_Provincia")
                            Com = Dt_SuperfInter.Rows(i).Item("CodiceIstat_Comune")
                            Sezione = Dt_SuperfInter.Rows(i).Item("Sezione")
                            Foglio = Dt_SuperfInter.Rows(i).Item("Foglio")
                            Numero = Dt_SuperfInter.Rows(i).Item("Numero")
                            Subalterno = Dt_SuperfInter.Rows(i).Item("Subalterno")
                            Macrouso_Cod = Dt_SuperfInter.Rows(i).Item("Macrouso_Cod")
                            Veg_Cod_Agea = Dt_SuperfInter.Rows(i).Item("Veg_Cod_agea")
                            Cul_Cod_Agea = Dt_SuperfInter.Rows(i).Item("Cul_Cod_agea")

                            If Sezione = "&nbsp;" OrElse Sezione = "" Then
                                Sezione = "0"
                            End If

                            If Subalterno = "&nbsp;" OrElse Subalterno = "" Then
                                Subalterno = "0"
                            End If

                            If (Not HTAppezzamenti.Contains(String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}", Prov, Com, Sezione, Foglio, Numero, Subalterno, Macrouso_Cod, Veg_Cod_Agea, Cul_Cod_Agea))) And
                                   Prov = DtUtil.Rows(xy).Item("prov") And
                                   Com = DtUtil.Rows(xy).Item("com") And
                                   Sezione = DtUtil.Rows(xy).Item("sezione") And
                                   Foglio = DtUtil.Rows(xy).Item("foglio") And
                                   Numero = DtUtil.Rows(xy).Item("numero") And
                                   Subalterno = DtUtil.Rows(xy).Item("Subalterno") And
                                   Macrouso_Cod = DtUtil.Rows(xy).Item("Macrouso_Cod") And
                                   Veg_Cod_Agea = DtUtil.Rows(xy).Item("Veg_Cod_Agea") And
                                   Cul_Cod_Agea = DtUtil.Rows(xy).Item("Cul_Cod_Agea") Then

                                SuperficieIntersezione = DtUtil.Rows(xy).Item("Superficie")
                                'SuperficieIntersezione = DTRsAP.Rows(xy).Item("area")

                                'TO VERIFY colonna del check
                                Dt_SuperfInter.Rows(i).Item("ChkSelezionaParticella") = True

                                'TO VERIFY colonna intersezione
                                Dt_SuperfInter.Rows(i).Item("SuperficieImpiegata") = Format(SuperficieIntersezione, "0.0000")

                                'essendo in modifica risommo la sup del macrouso assegnata a questo campo

                                Dt_SuperfInter.Rows(i).Item("SuperficieCondottaDisponibile") = Format(CDbl(Dt_SuperfInter.Rows(i).Item("SuperficieCondottaDisponibile")) + SuperficieIntersezione, "0.0000")
                                Dt_SuperfInter.Rows(i).Item("SuperficieMacrousoDisponibile") = Format(CDbl(Dt_SuperfInter.Rows(i).Item("SuperficieMacrousoDisponibile")) + SuperficieIntersezione, "0.0000")
                                Dt_SuperfInter.Rows(i).Item("SuperficieUtilizzoDisponibile") = Format(CDbl(Dt_SuperfInter.Rows(i).Item("SuperficieUtilizzoDisponibile")) + SuperficieIntersezione, "0.0000")

                                HTAppezzamenti.Add(String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}", Prov, Com, Sezione, Foglio, Numero, Subalterno, Macrouso_Cod, Veg_Cod_Agea, Cul_Cod_Agea), 1)

                            End If

                            'If Not HTAppezzamenti2.Contains(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", Prov, Com, Sezione, Foglio, Numero, Subalterno)) Then
                            '    HTAppezzamenti2.Add(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", Prov, Com, Sezione, Foglio, Numero, Subalterno), 1)
                            'End If

                        Next


                    Next

                Else

                    'SE CI SONO MACROUSI
                    If flag_Macrousi AndAlso DtMacr.Rows.Count > 0 Then

                        For xy = 0 To DtMacr.Rows.Count - 1

                            For i = 0 To Dt_SuperfInter.Rows.Count - 1

                                'Recupero la chiave
                                Prov = Dt_SuperfInter.Rows(i).Item("CodiceIstat_Provincia")
                                Com = Dt_SuperfInter.Rows(i).Item("CodiceIstat_Comune")
                                Sezione = Dt_SuperfInter.Rows(i).Item("Sezione")
                                Foglio = Dt_SuperfInter.Rows(i).Item("Foglio")
                                Numero = Dt_SuperfInter.Rows(i).Item("Numero")
                                Subalterno = Dt_SuperfInter.Rows(i).Item("Subalterno")
                                Macrouso_Cod = Dt_SuperfInter.Rows(i).Item("Macrouso_Cod")

                                If Sezione = "&nbsp;" OrElse Sezione = "" Then
                                    Sezione = "0"
                                End If

                                If Subalterno = "&nbsp;" OrElse Subalterno = "" Then
                                    Subalterno = "0"
                                End If

                                If (Not HTAppezzamenti.Contains(String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", Prov, Com, Sezione, Foglio, Numero, Subalterno, Macrouso_Cod))) AndAlso
                                       Prov = DtMacr.Rows(xy).Item("prov") AndAlso
                                       Com = DtMacr.Rows(xy).Item("com") AndAlso
                                       Sezione = DtMacr.Rows(xy).Item("sezione") AndAlso
                                       Foglio = DtMacr.Rows(xy).Item("foglio") AndAlso
                                       Numero = DtMacr.Rows(xy).Item("numero") AndAlso
                                       Subalterno = DtMacr.Rows(xy).Item("Subalterno") AndAlso
                                       Macrouso_Cod = DtMacr.Rows(xy).Item("Macrouso_Cod") Then


                                    SuperficieIntersezione = DtMacr.Rows(xy).Item("Superficie")
                                    'SuperficieIntersezione = DTRsAP.Rows(xy).Item("area")

                                    'colonna del check
                                    Dt_SuperfInter.Rows(i).Item("ChkSelezionaParticella") = True

                                    'colonna intersezione
                                    Dt_SuperfInter.Rows(i).Item("SuperficieImpiegata") = Format(SuperficieIntersezione, "0.0000")


                                    'essendo in modifica risommo la sup del macrouso assegnata a questo campo
                                    Dt_SuperfInter.Rows(i).Item("SuperficieCondottaDisponibile") = Format(CDbl(Dt_SuperfInter.Rows(i).Item("SuperficieCondottaDisponibile")) + SuperficieIntersezione, "0.0000")
                                    Dt_SuperfInter.Rows(i).Item("SuperficieMacrousoDisponibile") = Format(CDbl(Dt_SuperfInter.Rows(i).Item("SuperficieMacrousoDisponibile")) + SuperficieIntersezione, "0.0000")

                                    HTAppezzamenti.Add(String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", Prov, Com, Sezione, Foglio, Numero, Subalterno, Macrouso_Cod), 1)

                                End If

                                'If Not HTAppezzamenti2.Contains(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", Prov, Com, Sezione, Foglio, Numero, Subalterno)) Then
                                '    HTAppezzamenti2.Add(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", Prov, Com, Sezione, Foglio, Numero, Subalterno), 1)
                                'End If

                            Next

                        Next

                    Else

                        For xy = 0 To DTRsAP.Rows.Count - 1

                            For i = 0 To Dt_SuperfInter.Rows.Count - 1

                                'Recupero la chiave
                                Prov = Dt_SuperfInter.Rows(i).Item("CodiceIstat_Provincia")
                                Com = Dt_SuperfInter.Rows(i).Item("CodiceIstat_Comune")
                                Sezione = Dt_SuperfInter.Rows(i).Item("Sezione")
                                Foglio = Dt_SuperfInter.Rows(i).Item("Foglio")
                                Numero = Dt_SuperfInter.Rows(i).Item("Numero")
                                Subalterno = Dt_SuperfInter.Rows(i).Item("Subalterno")

                                If Sezione = "&nbsp;" OrElse Sezione = "" Then
                                    Sezione = "0"
                                End If

                                If Subalterno = "&nbsp;" OrElse Subalterno = "" Then
                                    Subalterno = "0"
                                End If

                                If (Not HTAppezzamenti.Contains(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", Prov, Com, Sezione, Foglio, Numero, Subalterno))) AndAlso
                                       Prov = DTRsAP.Rows(xy).Item("prov") AndAlso
                                       Com = DTRsAP.Rows(xy).Item("com") AndALso
                                       Sezione = DTRsAP.Rows(xy).Item("sezione") AndAlso
                                       Foglio = DTRsAP.Rows(xy).Item("foglio") AndAlso
                                       Numero = DTRsAP.Rows(xy).Item("numero") AndAlso
                                       Subalterno = DTRsAP.Rows(xy).Item("Subalterno") Then

                                    SuperficieIntersezione = DTRsAP.Rows(xy).Item("area")


                                    If (objParametriAgenda.Tipo_Operazione <> "1") Then
                                        'colonna del check
                                        Dt_SuperfInter.Rows(i).Item("ChkSelezionaParticella") = True

                                        'colonna intersezione
                                        Dt_SuperfInter.Rows(i).Item("SuperficieImpiegata") = Format(SuperficieIntersezione, "0.0000")

                                        'essendo in modifica risommo la sup assegnata a questo campo
                                        Dt_SuperfInter.Rows(i).Item("SuperficieCondottaDisponibile") = Format(CDbl(Dt_SuperfInter.Rows(i).Item("SuperficieCondottaDisponibile")) + SuperficieIntersezione, "0.0000")
                                    Else
                                        'colonna del check
                                        Dt_SuperfInter.Rows(i).Item("ChkSelezionaParticella") = False

                                        'colonna intersezione
                                        Dt_SuperfInter.Rows(i).Item("SuperficieImpiegata") = Format(SuperficieIntersezione, "0.0000")

                                        'essendo in modifica risommo la sup assegnata a questo campo
                                        Dt_SuperfInter.Rows(i).Item("SuperficieCondottaDisponibile") = Format(CDbl(Dt_SuperfInter.Rows(i).Item("SuperficieCondottaDisponibile")) + SuperficieIntersezione, "0.0000")
                                    End If

                                    Exit For

                                End If

                            Next

                            SuperficieIntersezioneTotale += SuperficieIntersezione

                        Next

                    End If

                End If

            End If

            r.RispostaStringa = CaricaGriglia_Particelle_xJSON(Dt_SuperfInter, flag_Macrousi, flag_Utilizzi, flag_Varieta)
            r.ParametroDue_stringa = SuperficieIntersezioneTotale

            'resetto l oggetto objparametri
            objParametri_Server.FinestraTemporaleInizio = appDInizio
            objParametri_Server.FinestraTemporaleFine = appDFine

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                        Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function




    '##########################################################################################################################################
    Private Sub Appezzamento_Edit_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        ' VAnni: 2/10/2017: a seguito di nuovo agroMasterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        CType(Me.Master, AgendaBootstrap).flag_MostraBtnIndietro = True

    End Sub

    '##########################################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim TargetUrl As String
        TargetUrl = Master.TrovaRedirectCorretto(False, objParametriAgenda.PaginaSitoOrigine, objParametriAgenda, True, Qs_Visibilita)
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
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
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
        UtenteAbilitato_Lettura = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"),
                                    Session("ASG_IdServizio"),
                                    enum_Security_Attivita.Anagrafica_Appezzamento,
                                    enum_Security_Operazione.Lettura,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)

        'Serve?
        'ViewState("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura

        Dim UtenteAbilitato_Modifica As Boolean = False
        UtenteAbilitato_Modifica = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"),
                                    Session("ASG_IdServizio"),
                                    enum_Security_Attivita.Anagrafica_Appezzamento,
                                    enum_Security_Operazione.Modifica,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)

        'Serve?
        'ViewState("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica

        If Not UtenteAbilitato_Modifica Then
            AnnullaTutto(Me, Nothing)
            Exit Sub
        End If


        objParametriAgenda = New ParametriAgenda

        xPiva = objParametriAgenda.Piva
        xSa_Cod = objParametriAgenda.Sa_Cod
        xAppezza = objParametriAgenda.Appezza
        xCampo_Cod = objParametriAgenda.Campo_Cod
        hidden_campo_cod.Value = xCampo_Cod

        Select Case objParametriAgenda.Tipo_Operazione
            Case enum_TipoOperazioneDB.Scrittura
                Master.Lbl_Titolo.Text = "Creazione Nuovo Appezzamento"
            Case enum_TipoOperazioneDB.Lettura
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("LetturaAppezzamento"), String)
            Case enum_TipoOperazioneDB.Modifica
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("ModificaAppezzamento"), String)
        End Select

        Operazione = objParametriAgenda.Tipo_Operazione

        HttpContext.Current.Session("operazione") = Operazione 'TODO è fatto così e fa schifo.. usare i parametri ag. veri

        If Not IsPostBack Then
            HttpContext.Current.Session("dt_Possessi") = Nothing
            HttpContext.Current.Session("dt_Utilizzo") = Nothing
        Else
            Exit Sub
        End If

        RipristinaDatiNeiControlli()


        If Operazione = enum_TipoOperazioneDB.Lettura Then

            RBL_Catasto_Con.Enabled = False
            RBL_Catasto_Senza.Enabled = False
            TxtSuperficie_SenzaCatasto.Enabled = False
            Opt_Convenzionale.Enabled = False
            Opt_InConversione.Enabled = False
            Opt_Biologico.Enabled = False
            TxtAppNome.Enabled = False
            Cmb_campo_ass.Enabled = False
            TxtValiditaInizio.Enabled = False
            TxtValiditaFine.Enabled = False
            Txt_Rif_Alfanum_App.Enabled = False
            Txt_Isola.Enabled = False
            TxtPendenza.Enabled = False
            Cmb_Esposizione.Enabled = False
            Cmb_Ubicazione.Enabled = False
            TxtCoordinataX.Enabled = False
            TxtCoordinataY.Enabled = False
            TxtCoordinataZ.Enabled = False
            CmbCodice.Enabled = False
            TxtCodiceValore2.Enabled = False
            TxtValiditaInizioCodice.Enabled = False
            TxtValiditaFineCodice.Enabled = False

            btn_Aggiungi_Codice.Visible = False
            btn_aggiungi_utilizzo.Visible = False

            Chk_Macrousi.Enabled = False
            Chk_Utilizzi.Enabled = False
            Chk_Varieta.Enabled = False


        End If

    End Sub

    Private Sub RipristinaDatiNeiControlli()
        Dim campoSquadro As Boolean = False
        Dim Sup_App As Double

        '##############################################################
        '#####  Inizializzo i controlli  ##############################
        '##############################################################



        ' Inserisco i dati nelle label in testata
        'Centro Aziendale
        Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        LblCentro.Text = objCentriAz.SaNome_from_SaCod(xPiva, xSa_Cod, objParametri_Server)

        'Appezzamento
        Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        'LblAppezza.Text = objAppezza.AppezzamentoNome_from_Appezza(xPiva, xSa_Cod, xAppezza, objParametri_Server)

        If xAppezza = "" Then
            xAppezza = "0"
        Else
            appezza.Value = xAppezza
        End If


        ' Campo
        Dim Dt_Appezzamento As New DataTable
        Dt_Appezzamento = objAppezza.Recupera_Dati_Appezzamento(xPiva, CInt(xSa_Cod), CInt(xAppezza), "", "", objParametri_Server)
        If (Dt_Appezzamento IsNot Nothing AndAlso Dt_Appezzamento.Rows.Count > 0) Then
            xCampo_Cod = Dt_Appezzamento.Rows(0).Item("Campo_Cod")
        End If


        If (Not IsNothing(xCampo_Cod) AndAlso IsNumeric(xCampo_Cod) AndAlso xCampo_Cod <> 0) Then
            Dim objCampo As New AgronicaCoreAnagrafeDAL.Campi_R
            LblCampo.Text = objCampo.CampoDes_from_CampoCod(xPiva, xSa_Cod, xCampo_Cod, objParametri_Server)
            objCampo = Nothing
            If controlloSeCampoSquadro(xPiva, xSa_Cod, xCampo_Cod) Then
                campoSquadro = True
            End If
        End If


        '----------------------------------------------
        ' macrousi
        Dim objAppxPartxMacr As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousi_R
        Dim DtMacr As DataTable

        DtMacr = objAppxPartxMacr.Leggi(xPiva, xSa_Cod, xAppezza, "", "", "", 0, 0, "", "", "", "", objParametri_Server)

        If DtMacr IsNot Nothing AndAlso DtMacr.Rows.Count > 0 Then
            Chk_Macrousi.Checked = True
        Else
            Chk_Macrousi.Checked = False
        End If

        ' utilizzi
        Dim objAppxPartxMacrxUtil As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_R
        Dim DtUtil As DataTable

        DtUtil = objAppxPartxMacrxUtil.Leggi(xPiva, xSa_Cod, xAppezza, "", "", "", 0, 0, "", "", "", "", "", "", objParametri_Server)

        If DtUtil IsNot Nothing AndAlso DtUtil.Rows.Count > 0 Then
            Chk_Utilizzi.Checked = True
        Else
            Chk_Utilizzi.Checked = False
        End If



        If Not IsPostBack Then

            Session("dtAppezzamentixIndirizzi") = Nothing

            'Aggiungo nel viewstate il valore x gestire prossimo nel caso ci sia una variazione
            'del campo...   ??????????????????????? cioè????
            Me.ViewState("Next_Val_4_Appezza") = 0

            '----- Disattivo le modifiche di Superficie

            TxtSuperficie.Text = "0"

            'TxtSupCorrezione.Text = "0"

            TxtSuperficie.ReadOnly = False
            ' TxtSuperficie.CssClass = "Testo_08_Blue"
            ' TxtSuperficie.BackColor = AgroColor_BluChiaro

            'RigaCorrezione.Visible = False

            'BtnCorrezione.Visible = False
            'Lbl_SupCorrezione.Visible = False
            'Lbl_SupCorrezione2.Visible = False

            'TxtSupCorrezione.Visible = False

            '----- Pulisco le varie Textbox

            TxtAppNome.Text = ""

            TxtPendenza.Text = "0"
            'TxtSupCampoSpia.Text = "0"

            TxtCoordinataX.Text = ""
            TxtCoordinataY.Text = ""
            TxtCoordinataZ.Text = ""


        Else
            Exit Sub
        End If


        AgronicaCoreUtility.CaricaListControl.OrientamentoProduttivo(CType(Me.Cmb_Utilizzo, ListControl),
                                                                     False, "", "",
                                                                     "", "", objParametri_Server)

        AgronicaCoreUtility.CaricaListControl.Esposizione(CType(Cmb_Esposizione, ListControl),
                                                          True, AgronicaAgenda_2010.Seleziona.ToUpper(), "",
                                                          "", "", objParametri_Server)

        AgronicaCoreUtility.CaricaListControl.Ubicazione(CType(Cmb_Ubicazione, ListControl),
                                                         True, AgronicaAgenda_2010.Seleziona.ToUpper(), "",
                                                         "", "", objParametri_Server)

        'combo precessioni colturali
        AgronicaCoreUtility.CaricaListControl.SpecieVegetale_Optimize_x_PDC_Modificata2(ddl_Coltura_1,
                                                                                True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", objParametri_Server)
        AgronicaCoreUtility.CaricaListControl.SpecieVegetale_Optimize_x_PDC_Modificata2(ddl_Coltura_2,
                                                                                True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", objParametri_Server)
        AgronicaCoreUtility.CaricaListControl.SpecieVegetale_Optimize_x_PDC_Modificata2(ddl_Coltura_3,
                                                                                True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", objParametri_Server)
        AgronicaCoreUtility.CaricaListControl.SpecieVegetale_Optimize_x_PDC_Modificata2(ddl_Coltura_4,
                                                                                True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", objParametri_Server)


        'carico la combo dei codici
        Dim StrCodiciAppezzamento As String
        Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
        StrCodiciAppezzamento = objCodiceAnagrafe.Filtro_Codici_Anagrafe(3, 3, 2, objParametri_Server) ' 3 è l'appezza solo in questo core

        'Dim objCodice As Object 'Agro_Anagrafe_AD.Codici_Anagrafe_Read
        '   *   CreateCANCELLATOObject("Agro_Anagrafe_AD.Codici_Anagrafe_Read")

        'StrCodiciAppezzamento = objCodice.Filtro_Codici_Anagrafe(3, 3, 2, , , , , _
        '                                                        CStr(Session(ASG_.con..._server)))

        'Elimino i codici Titolo Possesso, Metodo Produzione  e fine impiego perchè già presenti
        StrCodiciAppezzamento = Replace(StrCodiciAppezzamento, " Or Codice = 1016", "")
        StrCodiciAppezzamento = Replace(StrCodiciAppezzamento, "Codice = 1016 Or ", "")
        StrCodiciAppezzamento = Replace(StrCodiciAppezzamento, " Or Codice = 1018", "")
        StrCodiciAppezzamento = Replace(StrCodiciAppezzamento, "Codice = 1018 Or ", "")
        StrCodiciAppezzamento = Replace(StrCodiciAppezzamento, " Or Codice = 1014", "")
        StrCodiciAppezzamento = Replace(StrCodiciAppezzamento, "Codice = 1014 Or ", "")

        AgronicaCoreUtility.CaricaListControl.Codici(CType(Me.CmbCodice, ListControl),
                                                     True, "", "",
                                                     0, "",
                                                     StrCodiciAppezzamento, "", objParametri_Server)

        'metto la stinga restituita dal componente nel formato "cod,cod,cod.."
        'per utilizzarla quando devo caricare i dati
        StrCodiciAppezzamento = Replace(StrCodiciAppezzamento, " Or Codice = ", ",")
        StrCodiciAppezzamento = Right(StrCodiciAppezzamento, StrCodiciAppezzamento.Length - 9)

        ' Carico combo Campi aggregatori relativi all'azienda (se sono in un campo squdro non posso cambiare il campo dalla dropdown)
        Dim dt_campi As DataTable
        Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R
        'dt_campi = objCampi.Leggi_soloAggregatori(objParametriAgenda.Piva, xSa_Cod, "", "", objParametri_Server)
        dt_campi = objCampi.Leggi_x_anagrafica(objParametriAgenda.Piva, xSa_Cod, 0, "", "", objParametri_Server)

        Cmb_campo_ass.Items.Add(New ListItem("NESSUNO", "0"))

        For i = 0 To dt_campi.Rows.Count - 1
            Cmb_campo_ass.Items.Add(New ListItem(dt_campi.Rows(i).Item("Campo_des"), dt_campi.Rows(i).Item("campo_cod")))
        Next


        Dim Dt_Centro As New DataTable

        Dim objImpresaR As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dt_Centro = objImpresaR.Recupera_Dati_CentroAziendale(xPiva, xSa_Cod, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)


        If Dt_Centro IsNot Nothing AndAlso Dt_Centro.Rows.Count > 0 Then

            'non visualizzo l'impresa perché è nella master
            ' Me.Lbl_Impresa.Text = Dt_Centro.Rows(0).Item("Rag_Soc")
            'Centro Aziendale
            Me.LblCentro.Text = Dt_Centro.Rows(0).Item("Sa_Nome")

            '----- Suggerisco le date del padre (Centro Az)
            Dim ValiditaInizioCentroAz As String
            Dim ValiditaFineCentroAz As String

            ValiditaInizioCentroAz = Dt_Centro.Rows(0).Item("Validita_Inizio")
            ValiditaFineCentroAz = Dt_Centro.Rows(0).Item("Validita_Fine")

            InizioCentro.Value = Dt_Centro.Rows(0).Item("Validita_Inizio")
            FineCentro.Value = Dt_Centro.Rows(0).Item("Validita_Fine")

            lbl_centro_data_inizio.Text = Dt_Centro.Rows(0).Item("Validita_Inizio")
            lbl_centro_data_fine.Text = Dt_Centro.Rows(0).Item("Validita_Fine")

            TxtValiditaInizio.Text = IIf(ValiditaInizioCentroAz <> AGRODATAINIZIO, ValiditaInizioCentroAz, "")
            TxtValiditaFine.Text = IIf(ValiditaFineCentroAz <> AGRODATAFINE, ValiditaFineCentroAz, "")

        End If

        Select Case Operazione
            Case enum_TipoOperazioneDB.Scrittura

                'faccio le selezioni di base per la scrittura
                Opt_Convenzionale.Checked = True
                RBL_Catasto_Senza.Checked = True

                Dim dt_Indirizzi = New DataTable
                Crea_Dt_Indirizzi(dt_Indirizzi)
                jsIndirizzi = DT_to_Json_Indirizzi(dt_Indirizzi)

                '--------------------------------------------------
                ' solo per nuovo appezzamento:
                '
                ' - dato che sono in un nuovo appezzamento eredito le colture precedenti dal campo (SE PRESENTE)
                '--------------------------------------------------

                If xCampo_Cod <> 0 Then
                    Dim DTCodiciCampo As DataTable
                    Dim objCampiCodici As New AgronicaCoreAnagrafeDAL.Campi_Codici_Read

                    Dim Campo_Id_Cod As Integer
                    Dim Campo_Val_Cod As String = ""

                    DTCodiciCampo = objCampiCodici.Leggi(CStr(xPiva),
                                                CInt(xSa_Cod),
                                             CInt(xCampo_Cod),
                                            CInt(Campo_Id_Cod),
                                           CStr(Campo_Val_Cod),
                                           AGRODATAINIZIO,
                                            AGRODATAFINE,
                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "id_cod < 2000 OR id_cod >= 3000",
                                            "",
                                            objParametri_Server)

                    If DTCodiciCampo.Rows.Count > 0 Then
                        For i = 0 To DTCodiciCampo.Rows.Count - 1

                            If (Not IsDBNull(DTCodiciCampo.Rows(i).Item("val_cod"))) Then

                                Campo_Id_Cod = DTCodiciCampo.Rows(i).Item("id_cod")
                                Campo_Val_Cod = DTCodiciCampo.Rows(i).Item("val_cod")

                                Select Case DTCodiciCampo.Rows(i).Item("id_cod")

                                    Case enum_CodiciAnagrafe.Coltura_Campo_Precedente_1
                                        ddl_Coltura_1.SelectedValue = DTCodiciCampo.Rows(i).Item("val_cod")
                                    Case enum_CodiciAnagrafe.Coltura_Campo_Precedente_2
                                        ddl_Coltura_2.SelectedValue = DTCodiciCampo.Rows(i).Item("val_cod")
                                    Case enum_CodiciAnagrafe.Coltura_Campo_Precedente_3
                                        ddl_Coltura_3.SelectedValue = DTCodiciCampo.Rows(i).Item("val_cod")
                                    Case enum_CodiciAnagrafe.Coltura_Campo_Precedente_4
                                        ddl_Coltura_4.SelectedValue = DTCodiciCampo.Rows(i).Item("val_cod")
                                End Select
                            End If
                        Next
                    End If
                End If

            Case enum_TipoOperazioneDB.Modifica, enum_TipoOperazioneDB.Lettura


                '##############################################################
                '#####  Se sono in MODIFICA carico i dati  ####################
                '##############################################################


                '----- Tabella APPEZZAMENTO
                Dim objAppezzamenti As New AgronicaCoreAnagrafeDAL.Appezzamento_Read  'Agro_Anagrafe_AD.Appezzamento_Read
                Dim DTAppezzamenti As DataTable

                '   *   CreateCANCELLATOObject("Agro_Anagrafe_AD.Appezzamento_Read")
                Dim appInizio As Date = objParametri_Server.FinestraTemporaleInizio
                Dim appFine As Date = objParametri_Server.FinestraTemporaleFine
                objParametri_Server.FinestraTemporaleInizio = CDate(AGRODATAINIZIO)
                objParametri_Server.FinestraTemporaleFine = CDate(AGRODATAFINE)
                DTAppezzamenti = objAppezzamenti.Leggi(CStr(xPiva),
                                                       CInt(xSa_Cod),
                                                       CInt(xAppezza),
                                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                       "",
                                                       "",
                                                       objParametri_Server)

                objParametri_Server.FinestraTemporaleInizio = CDate(appInizio)
                objParametri_Server.FinestraTemporaleFine = CDate(appFine)

                'Se il recordset non è nullo
                If DTAppezzamenti.Rows.Count > 0 Then

                    TxtAppNome.Text = DTAppezzamenti.Rows(0).Item("App_Nome")
                    Dim AppNome As String
                    Dim Array As Char()

                    AppNome = DTAppezzamenti.Rows(0).Item("App_Nome")
                    Array = AppNome.ToCharArray()

                    TxtAppNome.Text = DTAppezzamenti.Rows(0).Item("App_Nome")

                    xCampo_Cod = DTAppezzamenti.Rows(0).Item("Campo_Cod")
                    TxtValiditaInizio.Text = DTAppezzamenti.Rows(0).Item("Validita_Inizio")

                    LblRiferimenti.Text = xPiva & " " & xSa_Cod & " " & DTAppezzamenti.Rows(0).Item("Campo_Cod") & " " & DTAppezzamenti.Rows(0).Item("Appezza")

                    Sup_App = DTAppezzamenti.Rows(0).Item("sup_app")

                    ViewState("Validita_Inizio_Appezzamento") = TxtValiditaInizio.Text

                    'controllo che non sia inserita la data di default...
                    If TxtValiditaInizio.Text = "01/01/1900" Then
                        TxtValiditaInizio.Text = ""
                    End If

                    TxtValiditaFine.Text = DTAppezzamenti.Rows(0).Item("Validita_Fine")
                    If TxtValiditaFine.Text = "31/12/2100" Then
                        TxtValiditaFine.Text = ""
                    End If

                    TxtSuperficie.Text = DTAppezzamenti.Rows(0).Item("sup_app")
                    TxtPendenza.Text = DTAppezzamenti.Rows(0).Item("pende")

                    Dim Indice As Integer

                    'Esposizione
                    Indice = Cmb_Esposizione.Items.IndexOf(Cmb_Esposizione.Items.FindByValue(DTAppezzamenti.Rows(0).Item("esposiz")))
                    Cmb_Esposizione.SelectedIndex = Indice


                    'Ubicazione
                    Indice = Cmb_Ubicazione.Items.IndexOf(Cmb_Ubicazione.Items.FindByValue(DTAppezzamenti.Rows(0).Item("ubicazione")))
                    Cmb_Ubicazione.SelectedIndex = Indice

                    'buffer zone
                    txtDistBZ_Allevamenti.Text = DTAppezzamenti.Rows(0).Item("DistBZ_Allevamenti")
                    txtDistBZ_AreeResPub.Text = DTAppezzamenti.Rows(0).Item("DistBZ_AreeResPub")
                    txtDistBZ_CorpiIdrici.Text = DTAppezzamenti.Rows(0).Item("DistBZ_CorpiIdrici")
                    txtDistBZ_VegNatNonColt.Text = DTAppezzamenti.Rows(0).Item("DistBZ_VegNatNonColt")
                    txtSupBZ_Riduzione.Text = DTAppezzamenti.Rows(0).Item("SupBZ_Riduzione")

                    'Coordinate
                    TxtCoordinataX.Text = DTAppezzamenti.Rows(0).Item("x")
                    TxtCoordinataY.Text = DTAppezzamenti.Rows(0).Item("y")
                    TxtCoordinataZ.Text = DTAppezzamenti.Rows(0).Item("zslm")


                    'verifico se ci sono particelle legate
                    '----------------------------------------------
                    Dim objAppxPart As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
                    Dim DtAppxPart As DataTable

                    DtAppxPart = objAppxPart.LeggiParticelle_Da_Appezzamento(xPiva, xSa_Cod, xAppezza, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

                    If DtAppxPart IsNot Nothing AndAlso DtAppxPart.Rows.Count > 0 Then
                        RBL_Catasto_Con.Checked = True
                        RBL_Catasto_Senza.Checked = False
                        LblSuperficie_Con_Catasto.Text = Sup_App
                    Else
                        TxtSuperficie_SenzaCatasto.Text = Sup_App
                        RBL_Catasto_Con.Checked = False
                        RBL_Catasto_Senza.Checked = True
                    End If


                    Popola_Dt_Indirizzi(CStr(xPiva), CInt(xSa_Cod), CInt(xAppezza))

                End If


                If ricaricaCodiciPresenti(StrCodiciAppezzamento) Then
                    If IsNothing(HttpContext.Current.Session("dt_Utilizzo")) Then
                        Dim Dt As New DataTable

                        Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
                        Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))

                        jsUtilizzo = DT_to_Json_Utilizzo(Dt)
                    Else
                        jsUtilizzo = DT_to_Json_Utilizzo(HttpContext.Current.Session("dt_Utilizzo"))
                    End If
                End If



        End Select

        '  Galassi, 30/05/2017 10.45.21: definisco il comportamento se campo squadro: se squadro non faccio modificare il campo associato.
        If Not campoSquadro Then
            Cmb_campo_ass.SelectedValue = xCampo_Cod
            hidden_tipoCampo.Value = 2
        Else
            Cmb_campo_ass.SelectedValue = xCampo_Cod
            hidden_tipoCampo.Value = 1
        End If

        'Imposto il viewstate utilizzato per le validità catasto
        ViewState("Validita_Inizio_Appezzamento") = TxtValiditaInizio.Text
        ViewState("Validita_Fine_Appezzamento") = TxtValiditaFine.Text


    End Sub



    ''########################################################################################
    'Private Sub CaricaGriglia_Particelle(ByVal Piva As String, _
    '                                     ByVal Sa_Cod As Integer, _
    '                                     ByVal Campo_Cod As Integer, _
    '                                     ByVal Appezza As Integer, _
    '                                     ByVal DataValiditaInizio As Date, _
    '                                     ByVal DataValiditaFine As Date, _
    '                                     ByVal Operazione As enum_TipoOperazioneDB)

    '    'modifico la data dell oggetto objparametri
    '    Dim appDInizio As New Date
    '    appDInizio = objParametri_Server.FinestraTemporaleInizio
    '    Dim appDFine As New Date
    '    appDFine = objParametri_Server.FinestraTemporaleFine

    '    objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(DataValiditaInizio, DataValiditaFine)

    '    '----- Definizione delle variabili

    '    Dim Dt As New DataTable
    '    Dim Dr As DataRow
    '    Dim MessaggioErrore As String
    '    Dim Progressivo As Integer

    '    Dim Ettari As Integer
    '    Dim Are As Integer
    '    Dim Centiare As Integer

    '    Dim SuperficieAppezzaAggregati As Double
    '    Dim SuperficieTotaleUsata As Double

    '    Dim Superficie As Double
    '    Dim Sup_Condotta As Double
    '    Dim Sup_Condotta_Min As Double
    '    Dim Sup_Lorda As Double
    '    Dim SuperficieIntersezione As Double
    '    Dim SuperficieDisponibile As Double

    '    Dim Prov As String
    '    Dim Com As String
    '    Dim Sezione As String
    '    Dim Foglio As Integer
    '    Dim Numero As Integer
    '    Dim Subalterno As String
    '    Dim Macrouso_Cod As String
    '    Dim Macrouso_Des As String
    '    Dim Sup_Macrouso As String

    '    Dim Veg_Cod_Agea As String
    '    Dim Veg_Des_Agea As String
    '    Dim Cul_Cod_Agea As String
    '    Dim Cul_Des_Agea As String
    '    Dim Sup_Utilizzo As String

    '    Dim i, j As Integer

    '    Dim SuperficieIntersezioneTotale As Double

    '    Dim Squadro As Boolean = False

    '    Dim ParticellaPresente As Boolean


    '    '----- Definisco la struttura del DataTable

    '    Dt.Columns.Add(New DataColumn("CodiceIstat_Provincia", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("CodiceIstat_Comune", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Part_Cod", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Progressivo", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Provincia", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Comune", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Sezione", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Foglio", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Numero", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Subalterno", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("SuperficieLorda", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Superficie", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("SuperficieCondottaDisponibile", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("SuperficieCondottaDisponibile2", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("SuperficieImpiegata", GetType(String)))

    '    Dt.Columns.Add(New DataColumn("Macrouso_Cod", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Macrouso_Des", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Sup_Macrouso", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("SuperficieMacrousoDisponibile", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("SuperficieMacrousoDisponibile2", GetType(String)))

    '    Dt.Columns.Add(New DataColumn("Veg_Cod_Agea", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Veg_Des_Agea", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Cul_Cod_Agea", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Cul_Des_Agea", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Sup_Utilizzo", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("SuperficieUtilizzoDisponibile", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("SuperficieUtilizzoDisponibile2", GetType(String)))

    '    '----- Definisco l'insieme di colonne che costituiscono la chiave della tabella

    '    'Vettore di DataColumn
    '    Dim DtKeys(8) As DataColumn

    '    'Valorizzo le celle del vettore
    '    DtKeys(0) = Dt.Columns("Provincia")
    '    DtKeys(1) = Dt.Columns("Comune")
    '    DtKeys(2) = Dt.Columns("Sezione")
    '    DtKeys(3) = Dt.Columns("Foglio")
    '    DtKeys(4) = Dt.Columns("Numero")
    '    DtKeys(5) = Dt.Columns("Subalterno")
    '    DtKeys(6) = Dt.Columns("Macrouso_Cod")
    '    DtKeys(7) = Dt.Columns("Veg_Cod_Agea")
    '    DtKeys(8) = Dt.Columns("Cul_Cod_Agea")

    '    'Assegno il vettore delle chiavi al DataTable
    '    Dt.PrimaryKey = DtKeys

    '    '----- Verifico quali particelle devo caricare
    '    Dim Dt_Particelle As New DataTable

    '    Dim Dt_Particella As New DataTable
    '    Dim DrParticella() As DataRow


    '    ' MACROUSI
    '    'Leggo i record della tabella AppezzamentoxParticellexMacrousi:
    '    '   - se contiene dei record metto il check sui macrousi e carico la combo;
    '    '   - se non contiene alcun record tolgo il check dai macrousi.

    '    Dim objAppxPartxMacr As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousi_R
    '    Dim DtMacr As DataTable

    '    DtMacr = objAppxPartxMacr.Leggi(Piva, Sa_Cod, Appezza, "", "", "", 0, 0, "", "", "", "", objParametri_Server)

    '    'If Not DtMacr Is Nothing AndAlso DtMacr.Rows.Count > 0 Then
    '    '    Chk_Macrousi.Checked = True
    '    'Else
    '    '    Chk_Macrousi.Checked = False
    '    'End If

    '    ' UTILIZZI
    '    'Leggo i record della tabella AppezzamentoxParticellexMacrousixUtilizzo:
    '    '   - se contiene dei record metto il check utilizzi e carico le combo;
    '    '   - se non contiene alcun record tolgo il check utilizzi.

    '    Dim objAppxPartxMacrxUtil As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_R
    '    Dim DtUtil As DataTable

    '    DtUtil = objAppxPartxMacrxUtil.Leggi(Piva, Sa_Cod, Appezza, "", "", "", 0, 0, "", "", "", "", "", "", objParametri_Server)

    '    'If Not DtUtil Is Nothing AndAlso DtUtil.Rows.Count > 0 Then
    '    '    Chk_Utilizzi.Checked = True
    '    'Else
    '    '    Chk_Utilizzi.Checked = False
    '    'End If

    '    'NOTA
    '    'Se l'appezzamento appartiene ad un campo allora verifico se il campo ha delle 
    '    'relazioni CAMPIxPARTICELLE valorizzate.
    '    'In caso affermativo ho una gestione del terreno a SQUADRI e presento solo le 
    '    'particelle che sono state relazionate con il campo.

    '    'In caso negativo il campo e' gestito solo come AGGREGATO di APPEZZAMENTI allora
    '    'visualizzo tutte le particelle del Centro Aziendale.

    '    'Se l'appezzamento non appartiene ad un campo, allora opero come nel caso dello
    '    'aggregato e visualizzo tutte le particelle del Centro Aziendale.

    '    'NOTA
    '    'Se APPEZZA <> 0 allora ricarico anche i valori attuali



    '    If Campo_Cod = 0 Then

    '        '### Tutte le particelle del Centro Aziendale ###
    '        'Me.Lbl_NoteCatastoAppezzamento.Text = _
    '        '                    "L'Appezzamento non appartiene a nessun Campo." & _
    '        '                    "<br>Le particelle disponibili sono quelle legate al Centro Aziendale !!!"
    '        Lbl_NoteCatastoAppezzamento.Text = ""


    '        Dim objParticelleCatastali_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R

    '        If Chk_Macrousi.Checked = True Then
    '            If Chk_Utilizzi.Checked = False Then
    '                Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_Macrousi(Piva, Sa_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
    '            Else
    '                Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(Piva, Sa_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
    '            End If
    '        Else
    '            If Chk_Utilizzi.Checked = False Then
    '                Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro(Piva, Sa_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
    '            Else
    '                Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(Piva, Sa_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
    '            End If
    '        End If

    '    Else

    '        Dim objCampixParticelleR As New AgronicaCoreAnagrafeDAL.CampixParticelle_R
    '        If objCampixParticelleR.Campo_Definito_Come_Squadro(Piva, Sa_Cod, Campo_Cod, objParametri_Server) = True Then

    '            'Imposto la variabile Squadro 
    '            Squadro = True

    '            '### Particelle associate al Campo
    '            'Me.Lbl_NoteCatastoAppezzamento.Text = _
    '            '                "L'Appezzamento appartiene ad un Campo." & _
    '            '                "<br>Per tale Campo e' stata definita una superficie catastale " & _
    '            '                "percio' le particelle disponibili sono quelle assegnate al Campo !!!"
    '            Lbl_NoteCatastoAppezzamento.Text = "L'Appezzamento appartiene ad un Campo per il quale e' stata definita una superficie catastale. Le particelle disponibili sono quelle assegnate al Campo !!!"

    '            Dim objParticelleCatastali_R As New AgronicaCoreAnagrafeDAL.CampixParticelle_R
    '            'If Chk_Macrousi.Checked = True Then
    '            '    Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro_con_Macrousi(Piva, Sa_Cod, Campo_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
    '            'Else
    '            '    Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro(Piva, Sa_Cod, Campo_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
    '            'End If

    '            If Chk_Macrousi.Checked = True Then
    '                If Chk_Utilizzi.Checked = False Then
    '                    Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro_con_Macrousi(Piva, Sa_Cod, Campo_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
    '                Else
    '                    Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro_con_MacrousiUtilizzo(Piva, Sa_Cod, Campo_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
    '                End If
    '            Else
    '                If Chk_Utilizzi.Checked = False Then
    '                    Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro(Piva, Sa_Cod, Campo_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
    '                Else
    '                    Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro_con_MacrousiUtilizzo(Piva, Sa_Cod, Campo_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
    '                End If
    '            End If
    '        Else

    '            '### Tutte le particelle del Centro Aziendale ###
    '            'Me.Lbl_NoteCatastoAppezzamento.Text = _
    '            '            "L'Appezzamento appartiene ad un Campo." & _
    '            '              "<br>Per tale Campo e' NON stata definita una superficie catastale " & _
    '            '              "percio' le particelle disponibili sono quelle legate al Centro Aziendale !!!"
    '            Lbl_NoteCatastoAppezzamento.Text = "L'Appezzamento appartiene ad un Campo per il quale NON e' stata definita una superficie catastale. Le particelle disponibili sono quelle assegnate al Centro Aziendale !!!"

    '            Dim objParticelleCatastali_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R

    '            If Chk_Macrousi.Checked = True Then
    '                If Chk_Utilizzi.Checked = False Then
    '                    Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_Macrousi(Piva, Sa_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
    '                Else
    '                    Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(Piva, Sa_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
    '                End If
    '            Else
    '                If Chk_Utilizzi.Checked = False Then
    '                    Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro(Piva, Sa_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
    '                Else
    '                    Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(Piva, Sa_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
    '                End If
    '            End If


    '        End If

    '    End If

    '    ' Salvo le particelle in sessione
    '    HttpContext.Current.Session("dt_Possessi") = Dt_Particelle


    '    '--------------------------------------------------------------------
    '    '--- HO TUTTE LE PARTICELLE --> CALCOLO X OGNUNA LA SUP.IMPIEGATA ---
    '    '--------------------------------------------------------------------

    '    '----- Carico la griglia
    '    'If Campo_Cod <> 0 Then

    '    Dim chiave_hash As String
    '    Dim table_hash As New Hashtable

    '    Dim chiave_hash_macrousi As String
    '    Dim table_hash_macrousi As New Hashtable

    '    Dim chiave_hash_vegcod_agea As String
    '    Dim table_hash_vegcod_agea As New Hashtable
    '    Dim chiave_hash_culcod_agea As String
    '    Dim table_hash_culcod_agea As New Hashtable

    '    Dim l As New List(Of ColonneNome)

    '    'Cmb_Macrousi.Items.Clear()
    '    'Cmb_Utilizzo1.Items.Clear()

    '    'Se il recordset esiste ...
    '    If Not IsNothing(Dt_Particelle) Then

    '        Progressivo = 1

    '        For i = 0 To Dt_Particelle.Rows.Count - 1

    '            Prov = Dt_Particelle.Rows(i).Item("prov")

    '            Com = Dt_Particelle.Rows(i).Item("com")


    '            If Dt_Particelle.Rows(i).Item("sezione") = "0" Then
    '                Sezione = ""
    '            Else
    '                Sezione = Dt_Particelle.Rows(i).Item("sezione")
    '            End If



    '            Foglio = Dt_Particelle.Rows(i).Item("foglio")

    '            Numero = Dt_Particelle.Rows(i).Item("numero")


    '            If Dt_Particelle.Rows(i).Item("Subalterno") = "0" Then
    '                Subalterno = ""
    '            Else
    '                Subalterno = Dt_Particelle.Rows(i).Item("subalterno")
    '            End If



    '            'RIEMPIO LA COMBO MACROUSI CON QUELLI GESTITI
    '            If Chk_Macrousi.Checked = True Then

    '                Dim cmb_index_m As Integer
    '                Dim cmb_flag_m As Boolean

    '                Macrouso_Cod = Dt_Particelle.Rows(i).Item("Macrouso_Cod")

    '                Macrouso_Des = Dt_Particelle.Rows(i).Item("Macrouso_Des")

    '                Sup_Macrouso = Dt_Particelle.Rows(i).Item("Sup_Macrouso")


    '                'creo la stringa CHIAVE
    '                chiave_hash_macrousi = Macrouso_Cod

    '                If Not table_hash_macrousi.ContainsKey(chiave_hash_macrousi) Then
    '                    table_hash_macrousi.Add(Macrouso_Cod, Macrouso_Des)
    '                    If Macrouso_Cod <> "" Then

    '                        cmb_flag_m = False
    '                        For cmb_index_m = 0 To Cmb_Macrousi.Items.Count - 1

    '                            If Macrouso_Des < Cmb_Macrousi.Items(cmb_index_m).Text Then
    '                                Cmb_Macrousi.Items.Insert(cmb_index_m, New ListItem(Macrouso_Des, Macrouso_Cod))
    '                                cmb_flag_m = True
    '                                Exit For
    '                            End If

    '                        Next

    '                        If Not cmb_flag_m Then
    '                            Cmb_Macrousi.Items.Add(New ListItem(Macrouso_Des, Macrouso_Cod))
    '                        End If

    '                    Else
    '                        Cmb_Macrousi.Items.Insert(0, New ListItem(Macrouso_Des, Macrouso_Cod))
    '                    End If
    '                End If

    '            Else
    '                Macrouso_Cod = ""
    '                Macrouso_Des = ""
    '                Sup_Macrouso = ""
    '            End If


    '            'RIEMPIO LE COMBO UTILIZZI
    '            If Chk_Utilizzi.Checked = True Then

    '                Dim cmb_index_u As Integer
    '                Dim cmb_flag_u As Boolean

    '                Macrouso_Cod = Dt_Particelle.Rows(i).Item("Macrouso_Cod")

    '                Macrouso_Des = Dt_Particelle.Rows(i).Item("Macrouso_Des")

    '                Sup_Macrouso = Dt_Particelle.Rows(i).Item("Sup_Macrouso")


    '                Veg_Cod_Agea = Dt_Particelle.Rows(i).Item("Veg_Cod_agea")
    '                Veg_Des_Agea = Dt_Particelle.Rows(i).Item("Veg_Des_agea")
    '                Cul_Cod_Agea = Dt_Particelle.Rows(i).Item("Cul_Cod_agea")
    '                Cul_Des_Agea = Dt_Particelle.Rows(i).Item("Cul_Des_agea")

    '                Sup_Utilizzo = Dt_Particelle.Rows(i).Item("Sup_Utilizzo")


    '                'creo la stringa CHIAVE
    '                chiave_hash_vegcod_agea = Veg_Cod_Agea
    '                chiave_hash_culcod_agea = Cul_Cod_Agea

    '                'specie
    '                If Not table_hash_vegcod_agea.ContainsKey(chiave_hash_vegcod_agea) Then
    '                    table_hash_vegcod_agea.Add(Veg_Cod_Agea, Veg_Des_Agea)
    '                    If Veg_Cod_Agea <> "0" Then
    '                        cmb_flag_u = False
    '                        For cmb_index_u = 0 To Cmb_Utilizzo1.Items.Count - 1
    '                            If Veg_Des_Agea < Cmb_Utilizzo1.Items(cmb_index_u).Text Then
    '                                Cmb_Utilizzo1.Items.Insert(cmb_index_u, New ListItem(Veg_Des_Agea, Veg_Cod_Agea))
    '                                cmb_flag_u = True
    '                                Exit For
    '                            End If
    '                        Next
    '                        If Not cmb_flag_u Then
    '                            Cmb_Utilizzo1.Items.Add(New ListItem(Veg_Des_Agea, Veg_Cod_Agea))
    '                        End If
    '                    Else
    '                        Cmb_Utilizzo1.Items.Insert(0, New ListItem(Veg_Des_Agea, Veg_Cod_Agea))
    '                    End If
    '                End If

    '            Else
    '                Veg_Cod_Agea = ""
    '                Veg_Des_Agea = ""
    '                Cul_Cod_Agea = ""
    '                Cul_Des_Agea = ""
    '                Sup_Utilizzo = ""
    '            End If











    '            'creo la stringa CHIAVE
    '            'chiave_hash = Prov + "|" + Com + "|" + Sezione + "|" + Foglio.ToString + "|" + Numero.ToString + "|" + Subalterno + "|" + Macrouso_Cod
    '            chiave_hash = Prov + "|" + Com + "|" + Sezione + "|" + Foglio.ToString + "|" + Numero.ToString + "|" + Subalterno + "|" + Macrouso_Cod + "|" + Veg_Cod_Agea + "|" + Cul_Cod_Agea

    '            If Not table_hash.ContainsKey(chiave_hash) Then

    '                table_hash.Add(chiave_hash, "")
    '                DrParticella = Dt_Particelle.Select("prov='" & Dt_Particelle.Rows(i).Item("prov").ToString & _
    '                                "' AND com='" & Dt_Particelle.Rows(i).Item("com").ToString & _
    '                                "' AND sezione='" & Dt_Particelle.Rows(i).Item("sezione").ToString & _
    '                                "' AND foglio='" & Dt_Particelle.Rows(i).Item("foglio").ToString & _
    '                                "' AND numero='" & Dt_Particelle.Rows(i).Item("numero").ToString & _
    '                                "' AND subalterno='" & Dt_Particelle.Rows(i).Item("subalterno").ToString & "'")

    '                Sup_Condotta_Min = 0

    '                For j = 0 To DrParticella.Length - 1
    '                    If j = 0 Then
    '                        Sup_Condotta_Min = CDbl(DrParticella(j).Item("sup_condotta"))
    '                    Else
    '                        If CDbl(DrParticella(j).Item("sup_condotta")) <> 0 Then
    '                            If Sup_Condotta_Min > CDbl(DrParticella(j).Item("sup_condotta")) Then
    '                                Sup_Condotta_Min = CDbl(DrParticella(j).Item("sup_condotta"))
    '                                ' SuperficieTotaleUsata = CDbl(DrParticella(j).Item("sup_condotta"))
    '                            End If
    '                        End If
    '                    End If
    '                Next

    '                'Creo una nuova riga
    '                Dr = Dt.NewRow

    '                'Definisco i valori

    '                '---

    '                Dr.Item("CodiceIstat_Provincia") = Dt_Particelle.Rows(i).Item("prov")
    '                Dr.Item("CodiceIstat_Comune") = Dt_Particelle.Rows(i).Item("com")
    '                Dr.Item("Part_Cod") = Dt_Particelle.Rows(i).Item("part_cod")

    '                Dr.Item("Progressivo") = Progressivo

    '                '-----

    '                Dr.Item("Provincia") = Dt_Particelle.Rows(i).Item("COMUNI_PROV")
    '                Dr.Item("Comune") = Dt_Particelle.Rows(i).Item("LOCALITA")

    '                '-----

    '                Dr.Item("Sezione") = IIf(Dt_Particelle.Rows(i).Item("Sezione") = "0", "", Dt_Particelle.Rows(i).Item("Sezione"))
    '                Dr.Item("Foglio") = Dt_Particelle.Rows(i).Item("Foglio")
    '                Dr.Item("Numero") = Dt_Particelle.Rows(i).Item("Numero")
    '                Dr.Item("Subalterno") = IIf(Dt_Particelle.Rows(i).Item("Subalterno") = "0", "", Dt_Particelle.Rows(i).Item("Subalterno"))

    '                '-----

    '                Dr.Item("Superficie") = Format(Sup_Condotta_Min, "0.0000")

    '                Dim objUtility As New AgronicaCoreDataProvider.UtilityProvider
    '                Dr.Item("SuperficieLorda") = Format(objUtility.Ettari_from_EttariAreCentiare(CDbl(Dt_Particelle.Rows(i).Item("particella_ettari")), _
    '                                                                                      CDbl(Dt_Particelle.Rows(i).Item("particella_are")), _
    '                                                                                      CDbl(Dt_Particelle.Rows(i).Item("particella_centiare"))), _
    '                                                                                      "0.0000")


    '                '--------------------------------------------------------------------------
    '                ' Se l'Appezzamento appartiene ad un campo SQUADRO
    '                ' l'area della particella utilizzata è
    '                ' la somma delle intersezioni degli altri appezzamenti aggregati al campo!
    '                '--------------------------------------------------------------------------

    '                '--------------------------------------------------------------------------
    '                ' Se l'Appezzamento appartiene ad un campo NON SQUADRO
    '                ' o NON appartiene a campi
    '                ' l'area della particella utilizzata è
    '                ' la somma delle intersezioni con gli altri campi squadri + 
    '                ' l'area di intersezione con gli appezzamenti non aggregati in campo!
    '                '--------------------------------------------------------------------------

    '                Dim sezioneMacr As String
    '                Dim subMacr As String

    '                If Dt_Particelle.Rows(i).Item("sezione").ToString = "" Then
    '                    sezioneMacr = "0"
    '                Else
    '                    sezioneMacr = Dt_Particelle.Rows(i).Item("sezione").ToString
    '                End If

    '                If Dt_Particelle.Rows(i).Item("subalterno").ToString = "" Then
    '                    subMacr = "0"
    '                Else
    '                    subMacr = Dt_Particelle.Rows(i).Item("subalterno").ToString
    '                End If

    '                If Macrouso_Cod <> "" Then

    '                    Dim DtSuperficie As DataTable

    '                    DtSuperficie = objAppxPartxMacr.Leggi_SuperficieMacrousoUtilizzata(Piva, Sa_Cod, 0, _
    '                                                                                       Dt_Particelle.Rows(i).Item("PROV"), _
    '                                                                                       Dt_Particelle.Rows(i).Item("COM"), _
    '                                                                                       sezioneMacr, _
    '                                                                                       CInt(Dt_Particelle.Rows(i).Item("Foglio")), _
    '                                                                                       CInt(Dt_Particelle.Rows(i).Item("Numero")), _
    '                                                                                       subMacr, _
    '                                                                                       Macrouso_Cod, _
    '                                                                                       "", "", objParametri_Server)

    '                    SuperficieDisponibile = Sup_Macrouso

    '                    If Not IsDBNull(DtSuperficie.Rows(0).Item("Superficie")) Then
    '                        SuperficieDisponibile -= DtSuperficie.Rows(0).Item("Superficie")
    '                    End If

    '                    Dr.Item("SuperficieMacrousoDisponibile") = Format(SuperficieDisponibile, "0.0000")

    '                End If

    '                If Veg_Des_Agea <> "" Then

    '                    Dim DtSuperficieUt As DataTable

    '                    DtSuperficieUt = objAppxPartxMacrxUtil.Leggi_SuperficieMacrousoxUtilizzo(Piva, Sa_Cod, 0, _
    '                                                                                        Dt_Particelle.Rows(i).Item("PROV"), _
    '                                                                                        Dt_Particelle.Rows(i).Item("COM"), _
    '                                                                                        sezioneMacr, _
    '                                                                                        CInt(Dt_Particelle.Rows(i).Item("Foglio")), _
    '                                                                                        CInt(Dt_Particelle.Rows(i).Item("Numero")), _
    '                                                                                        subMacr, _
    '                                                                                        Macrouso_Cod, _
    '                                                                                        Veg_Cod_Agea, _
    '                                                                                        Cul_Cod_Agea, _
    '                                                                                        "", "", objParametri_Server)

    '                    SuperficieDisponibile = Sup_Utilizzo

    '                    If Not IsDBNull(DtSuperficieUt.Rows(0).Item("Superficie")) Then
    '                        SuperficieDisponibile -= DtSuperficieUt.Rows(0).Item("Superficie")
    '                    End If

    '                    Dr.Item("SuperficieUtilizzoDisponibile") = Format(SuperficieDisponibile, "0.0000")

    '                Else
    '                    Dr.Item("SuperficieUtilizzoDisponibile") = "0"
    '                End If

    '                If Squadro = True And Campo_Cod <> 0 Then

    '                    'Appezzamenti Aggregati allo squadro
    '                    SuperficieTotaleUsata = CDbl(Dt_Particelle.Rows(i).Item("SuperficieAppSquadro"))

    '                    'Calcolo quella disponibile
    '                    SuperficieDisponibile = CDbl(Sup_Condotta_Min) - SuperficieTotaleUsata

    '                    Dr.Item("SuperficieCondottaDisponibile") = Format(SuperficieDisponibile, "0.0000")
    '                    Dr.Item("SuperficieCondottaDisponibile2") = Format(SuperficieDisponibile, "0.0000")

    '                Else

    '                    SuperficieDisponibile = CDbl(Dt_Particelle.Rows(i).Item("SuperficieDisponibile"))

    '                    Dr.Item("SuperficieCondottaDisponibile") = Format(SuperficieDisponibile, "0.0000")
    '                    Dr.Item("SuperficieCondottaDisponibile2") = Format(SuperficieDisponibile, "0.0000")

    '                End If


    '                'End If


    '                '--------------------------------------------------------
    '                '--------------------------------------------------------
    '                '--------------------------------------------------------
    '                '--------------------------------------------------------
    '                '--------------------------------------------------------

    '                Dr.Item("SuperficieImpiegata") = 0

    '                Dr.Item("Macrouso_Cod") = Macrouso_Cod
    '                Dr.Item("Macrouso_Des") = Macrouso_Des
    '                Dr.Item("Sup_Macrouso") = Sup_Macrouso

    '                Dr.Item("Veg_Cod_Agea") = Veg_Cod_Agea
    '                Dr.Item("Veg_Des_Agea") = Veg_Des_Agea
    '                Dr.Item("Cul_Cod_Agea") = Cul_Cod_Agea
    '                Dr.Item("Cul_Des_Agea") = Cul_Des_Agea
    '                Dr.Item("Sup_Utilizzo") = Sup_Utilizzo


    '                '-----

    '                'Associo alla tabella la nuova riga creata
    '                Dt.Rows.Add(Dr)

    '                'Aggiorno il contatore
    '                Progressivo += 1

    '            End If

    '        Next


    '        ' Gestione colonne tabella Dati Catastali
    '        Dim cn As New ColonneNome("Provincia", "Provincia", "string")
    '        cn._css = "pos_prov"
    '        l.Add(cn)
    '        cn = New ColonneNome("Comune", "Comune", "string")
    '        cn._css = "pos_com"
    '        l.Add(cn)
    '        cn = New ColonneNome("CodiceIstat_Provincia", "Cod Prov", "string")
    '        l.Add(cn)
    '        cn = New ColonneNome("CodiceIstat_Comune", "Cod Com", "string")
    '        l.Add(cn)
    '        cn = New ColonneNome("Sezione", "Sezione", "string")
    '        cn._css = "pos_sez"
    '        l.Add(cn)
    '        cn = New ColonneNome("Foglio", "Foglio", "string")
    '        cn._css = "pos_fogl"
    '        l.Add(cn)
    '        cn = New ColonneNome("Numero", "Numero", "string")
    '        cn._css = "pos_num"
    '        l.Add(cn)
    '        cn = New ColonneNome("Subalterno", "Subalterno", "string")
    '        cn._css = "pos_sub"
    '        l.Add(cn)
    '        cn = New ColonneNome("SuperficieLorda", "Sup. Catastale Lorda [ha]", "string")
    '        cn._css = "pos_sup_lor"
    '        l.Add(cn)
    '        cn = New ColonneNome("Superficie", "Sup. Condotta [ha]", "string")
    '        cn._css = "pos_sup_con"
    '        l.Add(cn)
    '        cn = New ColonneNome("SuperficieCondottaDisponibile", "Sup. Condotta Disponibile [ha]", "string")
    '        cn._css = "pos_sup_dis"
    '        l.Add(cn)

    '        'Aggiungo una colonna di supporto per il calcolo CLIENT della superficie disponibile
    '        cn = New ColonneNome("SuperficieCondottaDisponibile2", "Sup. Condotta Disponibile [ha] 2", "string")
    '        l.Add(cn)


    '        ' Carico la Watable all'avvio
    '        'jsParticelle = DT_to_Json_Particelle(Dt_Particelle, l)
    '        jsParticelle = DT_to_Json_Particelle(Dt, l)

    '        'Nel caso in cui non fosse già presente, aggiungo alla combo l'elemento per la selezione di
    '        'tutti i macrousi
    '        If Cmb_Macrousi.Items.FindByValue("") Is Nothing Then
    '            Cmb_Macrousi.Items.Insert(0, New ListItem("", ""))
    '        End If

    '    End If


    '    '----- Associo il DataTable con il DataGrid

    '    DataGridParticelle.DataSource = Dt
    '    DataGridParticelle.DataBind()

    '    If Chk_Macrousi.Checked = True Then

    '        Dim macrouso_cod_selezionato As String = ""
    '        If Not ViewState("macrouso_cod") Is Nothing Then
    '            macrouso_cod_selezionato = ViewState("macrouso_cod").ToString
    '            Select Case macrouso_cod_selezionato
    '                Case ""
    '                Case Else
    '                    Dim Dt_Particelle_Filtrato As New DataTable
    '                    Dim Dr_Filtrato() As DataRow
    '                    Dt_Particelle_Filtrato = Dt.Clone
    '                    Dr_Filtrato = Dt.Select("macrouso_cod='" & macrouso_cod_selezionato & "'")
    '                    If Not Dr_Filtrato Is Nothing Then
    '                        For i = 0 To Dr_Filtrato.Length - 1
    '                            Dt_Particelle_Filtrato.ImportRow(Dr_Filtrato(i))
    '                        Next
    '                    End If
    '                    DataGridParticelle.DataSource = Dt_Particelle_Filtrato
    '                    DataGridParticelle.DataBind()

    '                    Cmb_Macrousi.SelectedIndex = _
    '                        Cmb_Macrousi.Items.IndexOf(Cmb_Macrousi.Items.FindByValue( _
    '                            macrouso_cod_selezionato))
    '            End Select
    '        End If

    '        DataGridParticelle.Columns(12).HeaderStyle.CssClass = ""
    '        DataGridParticelle.Columns(12).ItemStyle.CssClass = ""

    '        DataGridParticelle.Columns(13).HeaderStyle.CssClass = ""
    '        DataGridParticelle.Columns(13).ItemStyle.CssClass = ""

    '        DataGridParticelle.Columns(14).HeaderStyle.CssClass = ""
    '        DataGridParticelle.Columns(14).ItemStyle.CssClass = ""

    '        Cmb_Macrousi.Enabled = True

    '    Else

    '        DataGridParticelle.Columns(12).HeaderStyle.CssClass = "displaynone"
    '        DataGridParticelle.Columns(12).ItemStyle.CssClass = "displaynone"

    '        DataGridParticelle.Columns(13).HeaderStyle.CssClass = "displaynone"
    '        DataGridParticelle.Columns(13).ItemStyle.CssClass = "displaynone"

    '        DataGridParticelle.Columns(14).HeaderStyle.CssClass = "displaynone"
    '        DataGridParticelle.Columns(14).ItemStyle.CssClass = "displaynone"

    '        Cmb_Macrousi.Enabled = False

    '    End If



    '    If Chk_Utilizzi.Checked = True Then

    '        Dim veg_cod_agea_selezionato As String = ""
    '        Dim cul_cod_agea_selezionato As String = ""
    '        If Not ViewState("veg_cod_agea") Is Nothing Then
    '            veg_cod_agea_selezionato = ViewState("veg_cod_agea").ToString
    '            Select Case veg_cod_agea_selezionato
    '                Case ""
    '                Case Else
    '                    Dim Dt_Particelle_Filtrato As New DataTable
    '                    Dim Dr_Filtrato() As DataRow
    '                    Dt_Particelle_Filtrato = Dt.Clone
    '                    Dr_Filtrato = Dt.Select("veg_cod_agea='" & veg_cod_agea_selezionato & "'")
    '                    If Not Dr_Filtrato Is Nothing Then
    '                        For i = 0 To Dr_Filtrato.Length - 1
    '                            Dt_Particelle_Filtrato.ImportRow(Dr_Filtrato(i))
    '                        Next
    '                    End If
    '                    DataGridParticelle.DataSource = Dt_Particelle_Filtrato
    '                    DataGridParticelle.DataBind()

    '                    Cmb_Utilizzo1.SelectedIndex = _
    '                        Cmb_Utilizzo1.Items.IndexOf(Cmb_Utilizzo1.Items.FindByValue( _
    '                            veg_cod_agea_selezionato))
    '            End Select
    '        End If

    '        DataGridParticelle.Columns(15).HeaderStyle.CssClass = ""
    '        DataGridParticelle.Columns(15).ItemStyle.CssClass = ""

    '        If Chk_Varieta.Checked = True Then
    '            ' DataGridParticelle.Columns(16).Visible = True
    '            DataGridParticelle.Columns(16).HeaderStyle.CssClass = ""
    '            DataGridParticelle.Columns(16).ItemStyle.CssClass = ""
    '        Else
    '            'DataGridParticelle.Columns(16).Visible = False
    '            DataGridParticelle.Columns(16).HeaderStyle.CssClass = "displaynone"
    '            DataGridParticelle.Columns(16).ItemStyle.CssClass = "displaynone"
    '        End If

    '        DataGridParticelle.Columns(17).HeaderStyle.CssClass = ""
    '        DataGridParticelle.Columns(17).ItemStyle.CssClass = ""

    '        DataGridParticelle.Columns(18).HeaderStyle.CssClass = ""
    '        DataGridParticelle.Columns(18).ItemStyle.CssClass = ""

    '        Cmb_Utilizzo1.Enabled = True

    '    Else

    '        DataGridParticelle.Columns(15).HeaderStyle.CssClass = "displaynone"
    '        DataGridParticelle.Columns(15).ItemStyle.CssClass = "displaynone"

    '        DataGridParticelle.Columns(16).HeaderStyle.CssClass = "displaynone"
    '        DataGridParticelle.Columns(16).ItemStyle.CssClass = "displaynone"

    '        DataGridParticelle.Columns(17).HeaderStyle.CssClass = "displaynone"
    '        DataGridParticelle.Columns(17).ItemStyle.CssClass = "displaynone"

    '        DataGridParticelle.Columns(18).HeaderStyle.CssClass = "displaynone"
    '        DataGridParticelle.Columns(18).ItemStyle.CssClass = "displaynone"

    '        Cmb_Utilizzo1.Enabled = False

    '    End If



    '    'In questo momento la "SuperficieDisponibile" è solo parziale

    '    '//////////////////////////////////////////////////////////////////////////////////////////////
    '    ' MODIFICA
    '    '----- Per ciascuna particella ripristino i check e le sup di intersezione
    '    '//////////////////////////////////////////////////////////////////////////////////////////////

    '    If Operazione = enum_TipoOperazioneDB.Modifica Then

    '        Dim objCOMAP As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R 'Object New Agro_Anagrafe_AD.AppezzaxParticelle_R
    '        Dim DTRsAP As DataTable

    '        'Cerco le intersezioni eventuali
    '        DTRsAP = objCOMAP.LeggiParticelle_Da_Appezzamento( _
    '                                                CStr(Piva), _
    '                                                CInt(Sa_Cod), _
    '                                                CInt(Appezza), _
    '                                                 enumSelezioneVariabile.Selezione_JoinDescrizioni, _
    '                                                "", _
    '                                                "", objParametri_Server)

    '        objCOMAP = Nothing

    '        If DTRsAP.Rows.Count > 0 Then

    '            Dim xy As Integer

    '            Dim HTAppezzamenti As New Hashtable
    '            Dim HTAppezzamenti2 As New Hashtable


    '            'Cerco le intersezioni eventuali con gli utilizzi
    '            If DtUtil.Rows.Count > 0 Then

    '                For xy = 0 To DtUtil.Rows.Count - 1

    '                    For i = 0 To Me.DataGridParticelle.Rows.Count - 1

    '                        'Recupero la chiave
    '                        Prov = DataGridParticelle.Rows(i).Cells(0).Text
    '                        Com = DataGridParticelle.Rows(i).Cells(1).Text
    '                        Sezione = DataGridParticelle.Rows(i).Cells(5).Text
    '                        Foglio = DataGridParticelle.Rows(i).Cells(6).Text
    '                        Numero = DataGridParticelle.Rows(i).Cells(7).Text
    '                        Subalterno = DataGridParticelle.Rows(i).Cells(8).Text
    '                        Macrouso_Cod = DataGridParticelle.Rows(i).Cells(22).Text
    '                        Veg_Cod_Agea = DataGridParticelle.Rows(i).Cells(23).Text
    '                        Cul_Cod_Agea = DataGridParticelle.Rows(i).Cells(24).Text

    '                        If Sezione = "&nbsp;" Then
    '                            Sezione = "0"
    '                        End If

    '                        If Subalterno = "&nbsp;" Then
    '                            Subalterno = "0"
    '                        End If

    '                        If Not HTAppezzamenti.Contains(String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}", Prov, Com, Sezione, Foglio, Numero, Subalterno, Macrouso_Cod, Veg_Cod_Agea, Cul_Cod_Agea)) And _
    '                           Prov = DtUtil.Rows(xy).Item("prov") And _
    '                           Com = DtUtil.Rows(xy).Item("com") And _
    '                           Sezione = DtUtil.Rows(xy).Item("sezione") And _
    '                           Foglio = DtUtil.Rows(xy).Item("foglio") And _
    '                           Numero = DtUtil.Rows(xy).Item("numero") And _
    '                           Subalterno = DtUtil.Rows(xy).Item("Subalterno") And _
    '                           Macrouso_Cod = DtUtil.Rows(xy).Item("Macrouso_Cod") And _
    '                           Veg_Cod_Agea = DtUtil.Rows(xy).Item("Veg_Cod_Agea") And _
    '                           Cul_Cod_Agea = DtUtil.Rows(xy).Item("Cul_Cod_Agea") Then

    '                            SuperficieIntersezione = DtUtil.Rows(xy).Item("Superficie")
    '                            'SuperficieIntersezione = DTRsAP.Rows(xy).Item("area")

    '                            CType(Me.DataGridParticelle.Rows(i).FindControl("ChkSelezionaParticella"), CheckBox).Checked = True

    '                            CType(Me.DataGridParticelle.Rows(i).FindControl("TxtIntersezione"), TextBox).Text = _
    '                                                                                  Format(SuperficieIntersezione, "0.0000")

    '                            'essendo in modifica risommo la sup del macrouso assegnata a questo campo
    '                            DataGridParticelle.Rows(i).Cells(11).Text() = Format(CDbl(DataGridParticelle.Rows(i).Cells(11).Text()) + SuperficieIntersezione, "0.0000")
    '                            DataGridParticelle.Rows(i).Cells(14).Text() = Format(CDbl(DataGridParticelle.Rows(i).Cells(14).Text()) + SuperficieIntersezione, "0.0000")
    '                            DataGridParticelle.Rows(i).Cells(18).Text() = Format(CDbl(DataGridParticelle.Rows(i).Cells(18).Text()) + SuperficieIntersezione, "0.0000")

    '                            HTAppezzamenti.Add(String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}-{8}", Prov, Com, Sezione, Foglio, Numero, Subalterno, Macrouso_Cod, Veg_Cod_Agea, Cul_Cod_Agea), 1)

    '                        End If

    '                        If Not HTAppezzamenti2.Contains(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", Prov, Com, Sezione, Foglio, Numero, Subalterno)) Then
    '                            HTAppezzamenti2.Add(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", Prov, Com, Sezione, Foglio, Numero, Subalterno), 1)
    '                        End If

    '                    Next

    '                Next

    '            Else

    '                'Cerco le intersezioni eventuali
    '                If DtMacr.Rows.Count > 0 Then

    '                    For xy = 0 To DtMacr.Rows.Count - 1

    '                        For i = 0 To Me.DataGridParticelle.Rows.Count - 1

    '                            'Recupero la chiave
    '                            Prov = DataGridParticelle.Rows(i).Cells(0).Text
    '                            Com = DataGridParticelle.Rows(i).Cells(1).Text
    '                            Sezione = DataGridParticelle.Rows(i).Cells(5).Text
    '                            Foglio = DataGridParticelle.Rows(i).Cells(6).Text
    '                            Numero = DataGridParticelle.Rows(i).Cells(7).Text
    '                            Subalterno = DataGridParticelle.Rows(i).Cells(8).Text
    '                            Macrouso_Cod = DataGridParticelle.Rows(i).Cells(22).Text

    '                            If Sezione = "&nbsp;" Then
    '                                Sezione = "0"
    '                            End If

    '                            If Subalterno = "&nbsp;" Then
    '                                Subalterno = "0"
    '                            End If

    '                            If Not HTAppezzamenti.Contains(String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", Prov, Com, Sezione, Foglio, Numero, Subalterno, Macrouso_Cod)) And _
    '                               Prov = DtMacr.Rows(xy).Item("prov") And _
    '                               Com = DtMacr.Rows(xy).Item("com") And _
    '                               Sezione = DtMacr.Rows(xy).Item("sezione") And _
    '                               Foglio = DtMacr.Rows(xy).Item("foglio") And _
    '                               Numero = DtMacr.Rows(xy).Item("numero") And _
    '                               Subalterno = DtMacr.Rows(xy).Item("Subalterno") And _
    '                               Macrouso_Cod = DtMacr.Rows(xy).Item("Macrouso_Cod") Then


    '                                SuperficieIntersezione = DtMacr.Rows(xy).Item("Superficie")
    '                                'SuperficieIntersezione = DTRsAP.Rows(xy).Item("area")

    '                                CType(DataGridParticelle.Rows(i).FindControl("ChkSelezionaParticella"), CheckBox).Checked = True

    '                                CType(DataGridParticelle.Rows(i).FindControl("TxtIntersezione"), TextBox).Text = _
    '                                                                                      Format(SuperficieIntersezione, "0.0000")

    '                                'essendo in modifica risommo la sup del macrouso assegnata a questo campo
    '                                DataGridParticelle.Rows(i).Cells(11).Text() = Format(CDbl(DataGridParticelle.Rows(i).Cells(11).Text()) + SuperficieIntersezione, "0.0000")
    '                                DataGridParticelle.Rows(i).Cells(14).Text() = Format(CDbl(DataGridParticelle.Rows(i).Cells(14).Text()) + SuperficieIntersezione, "0.0000")

    '                                HTAppezzamenti.Add(String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", Prov, Com, Sezione, Foglio, Numero, Subalterno, Macrouso_Cod), 1)

    '                            End If

    '                            If Not HTAppezzamenti2.Contains(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", Prov, Com, Sezione, Foglio, Numero, Subalterno)) Then
    '                                HTAppezzamenti2.Add(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", Prov, Com, Sezione, Foglio, Numero, Subalterno), 1)
    '                            End If

    '                        Next

    '                    Next

    '                End If

    '            End If

    '            For xy = 0 To DTRsAP.Rows.Count - 1

    '                For i = 0 To Me.DataGridParticelle.Rows.Count - 1

    '                    'Recupero la chiave
    '                    Prov = DataGridParticelle.Rows(i).Cells(0).Text
    '                    Com = DataGridParticelle.Rows(i).Cells(1).Text
    '                    Sezione = DataGridParticelle.Rows(i).Cells(5).Text
    '                    Foglio = DataGridParticelle.Rows(i).Cells(6).Text
    '                    Numero = DataGridParticelle.Rows(i).Cells(7).Text
    '                    Subalterno = DataGridParticelle.Rows(i).Cells(8).Text

    '                    Macrouso_Cod = Me.DataGridParticelle.Rows(i).Cells(22).Text

    '                    If Sezione = "&nbsp;" Then
    '                        Sezione = "0"
    '                    End If

    '                    If Subalterno = "&nbsp;" Then
    '                        Subalterno = "0"
    '                    End If

    '                    If Not HTAppezzamenti2.Contains(String.Format("{0}-{1}-{2}-{3}-{4}-{5}", Prov, Com, Sezione, Foglio, Numero, Subalterno)) And _
    '                       Prov = DTRsAP.Rows(xy).Item("prov") And _
    '                       Com = DTRsAP.Rows(xy).Item("com") And _
    '                       Sezione = DTRsAP.Rows(xy).Item("sezione") And _
    '                       Foglio = DTRsAP.Rows(xy).Item("foglio") And _
    '                       Numero = DTRsAP.Rows(xy).Item("numero") And _
    '                       Subalterno = DTRsAP.Rows(xy).Item("Subalterno") Then

    '                        SuperficieIntersezione = DTRsAP.Rows(xy).Item("area")
    '                        'SuperficieIntersezione = DTRsAP.Rows(xy).Item("area")

    '                        CType(DataGridParticelle.Rows(i).FindControl("ChkSelezionaParticella"), CheckBox).Checked = True

    '                        CType(DataGridParticelle.Rows(i).FindControl("TxtIntersezione"), TextBox).Text = _
    '                                                                              Format(SuperficieIntersezione, "0.0000")

    '                        'essendo in modifica risommo la sup assegnata a questo campo
    '                        DataGridParticelle.Rows(i).Cells(11).Text() = Format(CDbl(DataGridParticelle.Rows(i).Cells(11).Text()) + SuperficieIntersezione, "0.0000")


    '                        Exit For

    '                    End If

    '                Next

    '                SuperficieIntersezioneTotale += SuperficieIntersezione

    '            Next

    '        End If


    '    End If 
    '    'resetto l oggetto objparametri
    '    objParametri_Server.FinestraTemporaleInizio = appDInizio
    '    objParametri_Server.FinestraTemporaleFine = appDFine

    'End Sub

    '########################################################################################
    Private Sub ImgBtn_SalvaTutto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SalvaTutto.Click

        If ControlliSalvataggio() Then
            Salva_Tutto()
        End If

    End Sub


    ''########################################################################################
    'Private Sub ImgBtn_CalcolaSupCatastale_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs)

    '    Dim Prov As String = ""
    '    Dim Com As String = ""
    '    Dim Sezione As String = ""
    '    Dim Foglio As String = ""
    '    Dim Numero As String = ""
    '    Dim Subalterno As String = ""

    '    Dim i As Integer
    '    Dim SuperficieTotale As Double = 0

    '    Dim MessaggioErrore As String = ""

    '    'Se c'e' almeno una particella ...
    '    If DataGridParticelle.Rows.Count > 0 Then

    '        For i = 0 To DataGridParticelle.Rows.Count - 1

    '            If CType(DataGridParticelle.Rows(i).FindControl("ChkSelezionaParticella"), CheckBox).Checked = True Then

    '                If IsNumeric(CType(DataGridParticelle.Rows(i).FindControl("TxtIntersezione"), TextBox).Text) Then

    '                    SuperficieTotale += CDbl(CType(DataGridParticelle.Rows(i).FindControl("TxtIntersezione"), TextBox).Text)

    '                    If CDbl(CType(DataGridParticelle.Rows(i).FindControl("TxtIntersezione"), TextBox).Text) > DataGridParticelle.Rows(i).Cells(11).Text Then

    '                        If DataGridParticelle.Rows(i).Cells(3).Text <> "&nbsp;" Then
    '                            Prov = DataGridParticelle.Rows(i).Cells(3).Text
    '                        End If
    '                        If DataGridParticelle.Rows(i).Cells(4).Text <> "&nbsp;" Then
    '                            Com = DataGridParticelle.Rows(i).Cells(4).Text
    '                        End If
    '                        If DataGridParticelle.Rows(i).Cells(5).Text <> "&nbsp;" Then
    '                            Sezione = DataGridParticelle.Rows(i).Cells(5).Text
    '                        End If
    '                        If DataGridParticelle.Rows(i).Cells(6).Text <> "&nbsp;" Then
    '                            Foglio = DataGridParticelle.Rows(i).Cells(6).Text
    '                        End If
    '                        If DataGridParticelle.Rows(i).Cells(7).Text <> "&nbsp;" Then
    '                            Numero = DataGridParticelle.Rows(i).Cells(7).Text
    '                        End If
    '                        If DataGridParticelle.Rows(i).Cells(8).Text <> "&nbsp;" Then
    '                            Subalterno = DataGridParticelle.Rows(i).Cells(8).Text
    '                        End If

    '                        MessaggioErrore += " - La Sup. di Intersezione con la particella " & _
    '                                            Prov & " - " & _
    '                                            Com & " - " & _
    '                                            Sezione & " - " & _
    '                                            Foglio & " - " & _
    '                                            Numero & " - " & _
    '                                            Subalterno & " - " & _
    '                                            " supera la Sup. Disponibile!" & vbCrLf
    '                    End If

    '                Else

    '                    MessaggioErrore += " - Nella riga " + CStr(i + 1) + " è stato inserito un dato non numerico!" + vbCrLf

    '                End If

    '            End If

    '        Next

    '    End If

    '    'Me.Txt_SuperficieCatastale.Text = Format(SuperficieTotale, "0.0000")
    '    Input_SupTot.Value = Format(SuperficieTotale, "0.0000")

    '    If Chk_SuperficieCatastale.Checked = True And SuperficieTotale <> 0 Then
    '        LblSuperficie_Con_Catasto.Text = Format(SuperficieTotale, "0.0000")

    '        TxtSuperficie.Text = Format(SuperficieTotale, "0.0000")
    '    End If

    '    If MessaggioErrore <> "" Then
    '        'Throw New Exception(MessaggioErrore)
    '        Messaggi.AgroMsgBox(MessaggioErrore, Page)
    '        Exit Sub
    '    End If

    'End Sub


    '##########################################################################################################################
    'Private Sub Chk_Macrousi_CheckedChanged(sender As Object, e As System.EventArgs) Handles Chk_Macrousi.CheckedChanged


    '    Dim DataValiditaInizio As Date
    '    Dim DataValiditaFine As Date

    '    '------ Definisco le date di validità del catasto/appezzamento

    '    If TxtValiditaInizio.Text = "" Then
    '        DataValiditaInizio = Session("ASG_FinestraTemporale_Inizio")
    '    Else
    '        DataValiditaInizio = CDate(TxtValiditaInizio.Text)
    '    End If

    '    If TxtValiditaFine.Text = "" Then
    '        DataValiditaFine = Session("ASG_FinestraTemporale_Fine")
    '    Else
    '        DataValiditaFine = CDate(TxtValiditaFine.Text)
    '    End If

    '    'CaricaGriglia_Particelle( _
    '    '                      xPiva, _
    '    '                      xSa_Cod, _
    '    '                      xCampo_Cod, _
    '    '                      xAppezza, _
    '    '                      DataValiditaInizio, _
    '    '                      DataValiditaFine, _
    '    '                      Operazione)


    'End Sub

    '##########################################################################################################################
    'Private Sub Chk_Utilizzi_CheckedChanged(sender As Object, e As System.EventArgs) Handles Chk_Utilizzi.CheckedChanged


    '    Dim DataValiditaInizio As Date
    '    Dim DataValiditaFine As Date

    '    '------ Definisco le date di validità del catasto/appezzamento
    '    If TxtValiditaInizio.Text = "" Then
    '        DataValiditaInizio = Session("ASG_FinestraTemporale_Inizio")
    '    Else
    '        DataValiditaInizio = CDate(TxtValiditaInizio.Text)
    '    End If

    '    If TxtValiditaFine.Text = "" Then
    '        DataValiditaFine = Session("ASG_FinestraTemporale_Fine")
    '    Else
    '        DataValiditaFine = CDate(TxtValiditaFine.Text)
    '    End If

    '    ''CaricaGriglia_Particelle( _
    '    '                      xPiva, _
    '    '                      xSa_Cod, _
    '    '                      xCampo_Cod, _
    '    '                      xAppezza, _
    '    '                      DataValiditaInizio, _
    '    '                      DataValiditaFine, _
    '    '                      Operazione)
    'End Sub

    ''##########################################################################################################################
    'Private Sub Cmb_Macrousi_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles Cmb_Macrousi.SelectedIndexChanged

    '    ViewState("macrouso_cod") = Cmb_Macrousi.SelectedValue

    '    Dim DataValiditaInizio As Date
    '    Dim DataValiditaFine As Date


    '    '------ Definisco le date di validità del catasto/appezzamento

    '    If TxtValiditaInizio.Text = "" Then
    '        DataValiditaInizio = Session("ASG_FinestraTemporale_Inizio")
    '    Else
    '        DataValiditaInizio = CDate(TxtValiditaInizio.Text)
    '    End If

    '    If TxtValiditaFine.Text = "" Then
    '        DataValiditaFine = Session("ASG_FinestraTemporale_Fine")
    '    Else
    '        DataValiditaFine = CDate(TxtValiditaFine.Text)
    '    End If

    '    CaricaGriglia_Particelle( _
    '                          xPiva, _
    '                          xSa_Cod, _
    '                          xCampo_Cod, _
    '                          xAppezza, _
    '                          DataValiditaInizio, _
    '                          DataValiditaFine, _
    '                          Operazione)


    'End Sub

    '##########################################################################################################################
    'Private Sub Cmb_Utilizzo1_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles Cmb_Utilizzo1.SelectedIndexChanged

    '    ViewState("veg_cod_agea") = Cmb_Utilizzo1.SelectedValue

    '    Dim DataValiditaInizio As Date
    '    Dim DataValiditaFine As Date

    '    '------ Definisco le date di validità del catasto/appezzamento

    '    If TxtValiditaInizio.Text = "" Then
    '        DataValiditaInizio = Session("ASG_FinestraTemporale_Inizio")
    '    Else
    '        DataValiditaInizio = CDate(TxtValiditaInizio.Text)
    '    End If

    '    If TxtValiditaFine.Text = "" Then
    '        DataValiditaFine = Session("ASG_FinestraTemporale_Fine")
    '    Else
    '        DataValiditaFine = CDate(TxtValiditaFine.Text)
    '    End If

    '    CaricaGriglia_Particelle( _
    '                          xPiva, _
    '                          xSa_Cod, _
    '                          xCampo_Cod, _
    '                          xAppezza, _
    '                          DataValiditaInizio, _
    '                          DataValiditaFine, _
    '                          Operazione)

    'End Sub


    '##########################################################################################################################
    'Private Sub Chk_Varieta_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chk_Varieta.CheckedChanged

    '    If Chk_Utilizzi.Checked = True Then
    '        If Chk_Varieta.Checked = True Then

    '            DataGridParticelle.Columns(16).HeaderStyle.CssClass = ""
    '            DataGridParticelle.Columns(16).ItemStyle.CssClass = ""
    '        Else
    '            DataGridParticelle.Columns(16).HeaderStyle.CssClass = "displaynone"
    '            DataGridParticelle.Columns(16).ItemStyle.CssClass = "displaynone"
    '        End If
    '    Else
    '        DataGridParticelle.Columns(16).HeaderStyle.CssClass = "displaynone"
    '        DataGridParticelle.Columns(16).ItemStyle.CssClass = "displaynone"
    '    End If

    'End Sub


    '###################################################################################################
    'Private Sub Chk_SuperficieCatastale_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chk_SuperficieCatastale.CheckedChanged

    '    Dim SuperficieCatastale As Double

    '    If Me.Chk_SuperficieCatastale.Checked = True Then

    '        Me.TxtSuperficie.Enabled = False

    '        'Calcola la superficie catastale impostata nel datagrid

    '        ' ora è calcolata con jquery lato client
    '        ' Me.ImgBtn_CalcolaSupCatastale_Click(Me, Nothing)

    '        If IsNumeric(Input_SupTot.Value) AndAlso CDbl(Input_SupTot.Value) <> 0 Then

    '            'Imposta la textbox al nuovo valore
    '            Me.TxtSuperficie.Text = Format(CDbl(Input_SupTot.Value), "0.0000")

    '        End If

    '    Else

    '        Me.TxtSuperficie.Enabled = True

    '    End If

    'End Sub




    ''####################################################################################################################################
    'Private Sub ImgBtn_Codice_Ins_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Codice_Ins.Click
    '    Dim Testo As String
    '    Dim Testo2 As String
    '    Dim Testo3 As String
    '    Dim Codice As Integer
    '    Dim Indice As Integer
    '    Dim Valore As String

    '    If CmbCodice.SelectedIndex < 1 Then
    '        'Throw New Exception("Selezionare un codice !!!")
    '        Messaggi.AgroMsgBox("Selezionare un codice !!!", Page)
    '        Exit Sub
    '    End If

    '    If TxtCodiceValore.Text = "" Then
    '        'Throw New Exception("Non e' ammesso un valore nullo !!!")
    '        Messaggi.AgroMsgBox("Non e' ammesso un valore nullo !!!", Page)
    '        Exit Sub
    '    End If

    '    'Verifico che non esista gia' l'elemento
    '    For Indice = 0 To ListCodici.Items.Count - 1

    '        Testo = ListCodici.Items(Indice).Value

    '        If CmbCodice.SelectedItem.Value = Testo Then
    '            'Throw New Exception("Il codice è già stato risulta essere già presente !!!")
    '            Messaggi.AgroMsgBox("Il codice è già stato risulta essere già presente !!!", Page)
    '            Exit Sub
    '        End If
    '    Next

    '    'Inserisco l'elemento nella Listbox
    '    Testo = CmbCodice.SelectedItem.Text
    '    Testo2 = Testo & " = " & TxtCodiceValore.Text

    '    Valore = CmbCodice.SelectedItem.Value

    '    ListCodici.Items.Add(New ListItem(Testo2, Valore))

    '    CmbCodice.SelectedIndex = 0
    '    TxtCodiceValore.Text = ""

    'End Sub

    ''####################################################################################################################################
    'Private Sub ImgBtn_Codice_Canc_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Codice_Canc.Click

    '    Dim Indice As Integer

    '    If ListCodici.SelectedIndex > -1 Then

    '        'Recupero l'indice dell'elemento selezionato
    '        Indice = ListCodici.SelectedIndex

    '        'Elimino l'elemento selezionato
    '        ListCodici.Items.RemoveAt(Indice)

    '    End If

    'End Sub


    '########################################################################################
    Private Sub OrientamentoProduttivo_GestioneValCod(ByVal Val_Cod As String)

        Dim i As Integer
        Dim Vet_Orient As String()
        Dim Cod, Val As String
        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dim objBio As New AgronicaCoreMetaSchemaDAL.BIO_Dati_OrientamentoProduttivo_R


        If (IsNothing(HttpContext.Current.Session("dt_Utilizzo"))) Then
            Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
        Else
            Dt = HttpContext.Current.Session("dt_Utilizzo")
        End If

        Vet_Orient = Val_Cod.Split(",")

        For i = 0 To Vet_Orient.Length - 1
            Cod = Vet_Orient(i)
            Val = objBio.OrientProduttivoDes_from_OrientProduttivoCod(Cod, objParametri_Server)
            'ListOrientamento.Items.Add(New ListItem(Val, Cod))

            'Creo una nuova riga
            Dr = Dt.NewRow

            Dr.Item("Id_Cod") = Cod
            Dr.Item("Val_Cod") = Val

            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)


        Next

        HttpContext.Current.Session("dt_Utilizzo") = Dt
        jsUtilizzo = DT_to_Json_Utilizzo(Dt)

    End Sub






    '#################################################################################
    'Protected Sub Btn_Salva_Click(sender As Object, e As EventArgs) Handles Btn_Salva.Click

    '    If ControlliSalvataggio(CType(Operazione, enum_TipoOperazioneDB)) Then

    '        'Recupero l'operazione richiesta, dalla querystring e richiamo la sub x l'inserimento..
    '        SalvaTutto(CType(Operazione, enum_TipoOperazioneDB))

    '    End If

    'End Sub


    '#################################################################################
    'Private Sub Btn_SalvaTutto_Click(sender As Object, e As System.EventArgs) Handles Btn_SalvaTutto.Click

    '    'Recupero l'operazione richiesta, dalla querystring e richiamo la sub x l'inserimento..
    '    SalvaTutto(CType(Operazione, enum_TipoOperazioneDB))

    'End Sub

    '#################################################################################
    Private Function ControlliSalvataggio(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                          Optional ByVal blnValidaImpresa As Boolean = True) As Boolean

        Dim bRet As Boolean = True

        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date
        Dim AlmenoUna As Boolean = False


        '------------------------------------------------
        '----- Verifico la correttezza delle informazioni inserite
        '------------------------------------------------


        Dim MessaggioErrore As String = ""
        Dim MessaggioWarning As String = ""


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
            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("FineAppezzamentoPrecedenteCreazione"), String) & vbCrLf
        End If

        If Validita_Inizio < CDate(InizioCentro.Value) Then
            MessaggioErrore += "   - " & AgronicaAgenda_2010.InizioAttivitaNonPuòPrecedereCreazioneCentroAziendale & ". (" & InizioCentro.Value & ")" & vbCrLf
        End If

        If Validita_Fine > CDate(FineCentro.Value) Then
            MessaggioErrore += "   - " & AgronicaAgenda_2010.FineAttivitaNonPuòSeguireCessazioneCentroAziendale & ". (" & FineCentro.Value & ")" & vbCrLf
        End If


        '--- Nome dell'appezzamento

        'NOTA
        'ora viene assegnato in automatico

        'If TxtAppNome.Text = "" Then
        '    TxtAppNome.Text = AgroLabel_Appezzamento
        'End If


        '--- Pendenza

        If Me.TxtPendenza.Text = "" Then
            Me.TxtPendenza.Text = 0
        Else
            If Not IsNumeric(TxtPendenza.Text) Then
                Me.TxtPendenza.Text = 0

                MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LaPENDENZADeveEssereUnValoreNumerico"), String) & vbCrLf
            Else
                If InStr(Me.TxtPendenza.Text, ".") <> 0 Then
                    MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("ComeSeparatoreDecimalePerLaPENDENZAUtilizzareLaVirgola"), String) & vbCrLf

                Else
                    If (Me.TxtPendenza.Text > 100) Then

                        MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LaPendenzaPuòEssereAlMassimoCentoPerCento"), String) & vbCrLf

                    End If
                End If
            End If
        End If


        '--- Coordinate UTM
        If Me.TxtCoordinataX.Text = "" Then
            Me.TxtCoordinataX.Text = 0
        Else
            If Not IsNumeric(Me.TxtCoordinataX.Text) Then
                MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LaCoordinataXDeveEssereEspressaConUnValoreNumerico"), String) & vbCrLf
            End If
        End If

        If Me.TxtCoordinataY.Text = "" Then
            Me.TxtCoordinataY.Text = 0
        Else
            If Not IsNumeric(Me.TxtCoordinataY.Text) Then
                MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LaCoordinataYDeveEssereEspressaConUnValoreNumerico"), String) & vbCrLf
            End If
        End If

        If Me.TxtCoordinataZ.Text = "" Then
            Me.TxtCoordinataZ.Text = 0
        Else
            If Not IsNumeric(Me.TxtCoordinataZ.Text) Then
                MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LALTITUDINEDeveEssereEspressaConUnValoreNumerico"), String) & vbCrLf
            End If
        End If


        '--- Superficie

        'Superficie nulla ...
        If Me.TxtSuperficie.Text = "" Then
            MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LaSUPERFICIENonPuòEssereNulla"), String) & vbCrLf
        Else
            'Valore non numerico ...
            If Not IsNumeric(Me.TxtSuperficie.Text) Then
                MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("IlValoreDellaSUPERFICIEDeveEssereNumerico"), String) & vbCrLf
            Else
                'Presenza del punto come separatore decimale ...
                If InStr(Me.TxtSuperficie.Text, ".") <> 0 Then
                    MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("ComeSeparatoreDecimalePerLaSUPERFICIEUtilizzareLaVirgola"), String) & vbCrLf
                Else
                    If Me.TxtSuperficie.Text = 0 Then
                        MessaggioErrore += "   - " & DirectCast(GetLocalResourceObject("LaSUPERFICIENonPuòEssereNulla"), String) & vbCrLf
                    End If
                End If
            End If
        End If



        '--- Superficie Campo Spia

        'If Me.TxtSupCampoSpia.Text = "" Then
        '    MessaggioErrore += "   - La SUPERFICIE del CAMPO SPIA non puo' essere nulla." & vbCrLf
        'Else
        '    If Not IsNumeric(Me.TxtSupCampoSpia.Text) Then
        '        MessaggioErrore += "   - Il valore della SUPERFICIE del CAMPO SPIA deve essere numerico." & vbCrLf
        '    Else
        '        If (Me.TxtSupCampoSpia.Text = 0) And _
        '           (Me.ChkCampoSpia.Checked = True) Then

        '            MessaggioErrore += "   - La SUPERFICIE del CAMPO SPIA non puo' essere nulla." & vbCrLf

        '        End If
        '    End If
        'End If

        '--- Validità inizio appezzamento --> catasto
        '--- Validità fine appezzamento --> catasto

        'se esistono particelle...
        If DataGridParticelle.Rows.Count > 0 Then

            AlmenoUna = False

            'se qualche particella è selezionata...
            For i = 0 To DataGridParticelle.Rows.Count - 1
                If CType(DataGridParticelle.Rows(i).FindControl("ChkSelezionaParticella"), CheckBox).Checked Then
                    AlmenoUna = True
                    Exit For
                End If
            Next

            'se qualche particella è selezionata...
            If AlmenoUna Then

                If ViewState("Validita_Inizio_Appezzamento") <> TxtValiditaInizio.Text Then
                    MessaggioErrore &= "   - " & DirectCast(GetLocalResourceObject("AttenzioneRiverificareCatastoPerCambioDataInizio"), String) & vbCrLf
                End If

                If ViewState("Validita_Fine_Appezzamento") <> TxtValiditaFine.Text Then
                    MessaggioErrore &= "   - " & DirectCast(GetLocalResourceObject("AttenzioneRiverificareCatastoPerCambioDataFine"), String) & vbCrLf
                End If

            End If

        End If




        '--- La superficie delle Intersez. con le particelle 
        '--- non deve superare la sup.delle disponibile delle singole particelle
        'Select Case Qs_Operazione

        '    Case enum_TipoOperazioneDB.Scrittura

        If DataGridParticelle.Rows.Count > 0 Then

            For i = 0 To DataGridParticelle.Rows.Count - 1

                If CType(DataGridParticelle.Rows(i).FindControl("ChkSelezionaParticella"), CheckBox).Checked Then

                    If CType(DataGridParticelle.Rows(i).FindControl("TxtIntersezione"), TextBox).Text <> "" Then

                        If CDbl(CType(DataGridParticelle.Rows(i).FindControl("TxtIntersezione"), TextBox).Text) > DataGridParticelle.Rows(i).Cells(11).Text Then
                            MessaggioWarning &= " - Nella riga " & CStr(i + 1) & " Particella: " & DataGridParticelle.Rows(i).Cells(3).Text &
                            "-" & DataGridParticelle.Rows(i).Cells(4).Text

                            If DataGridParticelle.Rows(i).Cells(5).Text <> "" Then
                                MessaggioWarning &= "-" & DataGridParticelle.Rows(i).Cells(5).Text
                            End If

                            MessaggioWarning &= "-" & DataGridParticelle.Rows(i).Cells(6).Text &
                                                "-" & DataGridParticelle.Rows(i).Cells(7).Text

                            If DataGridParticelle.Rows(i).Cells(8).Text <> "" Then
                                MessaggioWarning &= "-" & DataGridParticelle.Rows(i).Cells(8).Text
                            End If

                            MessaggioWarning &= " " & DirectCast(GetLocalResourceObject("SuperficieIntersezioneSuperaLaSuperficieCondottaDisponibile"), String) & vbCrLf & vbCrLf
                        End If

                        If DataGridParticelle.Columns(14).Visible Then

                            If CDbl(CType(DataGridParticelle.Rows(i).FindControl("TxtIntersezione"), TextBox).Text) > DataGridParticelle.Rows(i).Cells(14).Text Then
                                MessaggioWarning &= " - Nella riga " & CStr(i + 1) & " Particella: " & DataGridParticelle.Rows(i).Cells(3).Text &
                                "-" & DataGridParticelle.Rows(i).Cells(4).Text 'i18n

                                If DataGridParticelle.Rows(i).Cells(5).Text <> "" Then
                                    MessaggioWarning &= "-" & DataGridParticelle.Rows(i).Cells(5).Text
                                End If

                                MessaggioWarning &= "-" & DataGridParticelle.Rows(i).Cells(6).Text &
                                                    "-" & DataGridParticelle.Rows(i).Cells(7).Text

                                If DataGridParticelle.Rows(i).Cells(8).Text <> "" Then
                                    MessaggioWarning &= "-" & DataGridParticelle.Rows(i).Cells(8).Text
                                End If

                                MessaggioWarning &= " " & DirectCast(GetLocalResourceObject("SuperficieIntersezioneSuperaLaSuperficieMacrousoDisponibile"), String) & vbCrLf & vbCrLf
                            End If

                        End If


                    Else
                        MessaggioErrore &= " - Nella riga " & CStr(i + 1) & " Particella: " & DataGridParticelle.Rows(i).Cells(3).Text &
                            "-" & DataGridParticelle.Rows(i).Cells(4).Text 'i18n

                        If DataGridParticelle.Rows(i).Cells(5).Text <> "" Then
                            MessaggioErrore &= "-" & DataGridParticelle.Rows(i).Cells(5).Text
                        End If

                        MessaggioErrore &= "-" & DataGridParticelle.Rows(i).Cells(6).Text &
                                            "-" & DataGridParticelle.Rows(i).Cells(7).Text

                        If DataGridParticelle.Rows(i).Cells(8).Text <> "" Then
                            MessaggioErrore &= "-" & DataGridParticelle.Rows(i).Cells(8).Text
                        End If

                        MessaggioErrore &= " " & DirectCast(GetLocalResourceObject("SuperficieDiIntersezioneNonInserita"), String) & vbCrLf & vbCrLf
                    End If

                End If

            Next
        End If

        ' TODO
        ' Aggiungere i controlli della coerenza delle particelle se ci sono degli squadri

        '--- RIEPILOGO

        Dim Messaggio As String

        If MessaggioWarning <> "" Then

            Messaggio = String.Format("{0}{1}<br><b><i>" & AgronicaAgenda_2010.ProcedereUgualmente & "</i></b>", MessaggioWarning, vbCr)

            'impedisco di procedere
            Messaggi.AgroSiNo(Messaggio, "Btn_Salva", Page)

            bRet = False

        End If

        If MessaggioErrore <> "" Then

            Messaggio = ""
            Messaggio += AgronicaAgenda_2010.SonoStatiRilevatiISeguentiErrori_ & vbCrLf
            Messaggio += "" & vbCrLf
            Messaggio += MessaggioErrore
            Messaggio += "" & vbCrLf
            Messaggio += AgronicaAgenda_2010.RitentareIlSalvataggioDopoLaCorrezione

            Messaggi.AgroMsgBox(Messaggio, Page)

            ' Exit Function

            bRet = False

        End If

        Return bRet

    End Function

    Private Function ControlliSalvataggio() As Boolean

        Dim bRet As Boolean = True

        '------------------------------------------------
        '----- Verifico la correttezza delle informazioni inserite
        '------------------------------------------------

        Dim MessaggioErrore As String = ""
        Dim MessaggioWarning As String = ""

        Dim objImpostazioni_Utenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim DTRiferimentoAppezzamento = objImpostazioni_Utenti.Leggi(enum_Impostazioni_Utenti.Codice_Univoco_Appezzamento, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

        If DTRiferimentoAppezzamento.Rows.Count > 0 AndAlso DTRiferimentoAppezzamento.Rows(0)("Impostazione_Valore_1").trim = "1" Then
            If Not String.IsNullOrEmpty(Txt_Rif_Alfanum_App.Text) Then
                Dim objAppezzamentoCodici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R
                Dim filtro As String = "(Appezzamento_Codici.Sa_Cod <> " & xSa_Cod & " OR Appezzamento_Codici.Appezza <> " & xAppezza & ")"
                Dim dtCodAppezzamento = objAppezzamentoCodici.Leggi(xPiva, 0, 0, enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento, Txt_Rif_Alfanum_App.Text, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, filtro, "", objParametri_Server)
                If dtCodAppezzamento.Rows.Count > 0 Then
                    MessaggioErrore &= DirectCast(GetLocalResourceObject("RiferimentoAppezzamentoGiàUtilizzatoSuUnAltro"), String)
                End If
            End If
        End If


        '--- RIEPILOGO

        Dim Messaggio As String

        If MessaggioWarning <> "" Then

            Messaggio = String.Format("{0}{1}<br><b><i>" & AgronicaAgenda_2010.ProcedereUgualmente & "</i></b>", MessaggioWarning, vbCr)

            'impedisco di procedere
            Messaggi.AgroSiNo(Messaggio, "Btn_Salva", Page)

            bRet = False

        End If

        If MessaggioErrore <> "" Then

            Messaggio = ""
            Messaggio += AgronicaAgenda_2010.SonoStatiRilevatiISeguentiErrori_ & vbCrLf
            Messaggio += "" & vbCrLf
            Messaggio += MessaggioErrore
            Messaggio += "" & vbCrLf
            Messaggio += AgronicaAgenda_2010.RitentareIlSalvataggioDopoLaCorrezione

            Messaggi.AgroMsgBox(Messaggio, Page)

            bRet = False

        End If

        Return bRet

    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function controllaMovimenti_CdG_xModificaValidita(piva As String, sa_cod As Integer, appezza As Integer,
                                                                    validita_inizio As String, validita_fine As String
                                                                    ) As RispostaStandard
        Dim r As New RispostaStandard


        Try
            Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objRisposta = New JObject()

            Dim xFiltroAggiuntivo As String
            Dim messaggio = ""
            Dim messaggioSpecifico As Boolean = False 'Per sapere se specificare la data del movimento solo se DTAgenda.Rows.Count = 1


            'CONTROLLO MOVIMENTI DI AGENDA
            Dim MovimentiPresenti = False
            Dim Validita_Inizio_Agenda As Date
            Dim Validita_Fine_Agenda As Date
            Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R
            xFiltroAggiuntivo = "( Data_Movimento < " & Agro_SQL_SaveDate(validita_inizio) & " OR Data_Movimento > " & Agro_SQL_SaveDate(validita_fine) & " )"
            Dim DTAgenda As DataTable
            'ricavo il recordset dei movimenti di produzione associati all'impianto
            DTAgenda = ObjAgenda.LeggiCronologiaMovimenti(piva, sa_cod, appezza, 0,
                                                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                          xFiltroAggiuntivo, "",
                                                          objParametri_Server)


            ObjAgenda = Nothing

            If DTAgenda.Rows.Count > 0 Then
                MovimentiPresenti = True
                Validita_Inizio_Agenda = DTAgenda.Rows(0).Item("Data_Movimento")
                Validita_Fine_Agenda = DTAgenda.Rows(DTAgenda.Rows.Count - 1).Item("Data_Movimento")

                If DTAgenda.Rows.Count = 1 Then
                    messaggioSpecifico = True
                End If
            End If

            If MovimentiPresenti Then
                If Validita_Inizio_Agenda < validita_inizio Then
                    If Not messaggioSpecifico Then
                        messaggio = "Impossibile modificare la data inizio dell'appezzamento, perché esistono registrazioni collegate ad un impianto"
                    Else
                        messaggio = "Impossibile modificare la data inizio dell'appezzamento, perché esistono registrazioni collegate ad un impianto in data " & Validita_Inizio_Agenda
                    End If
                End If

                If Validita_Fine_Agenda > validita_fine Then

                    If Not messaggioSpecifico Then
                        messaggio = "Impossibile modificare la data fine dell'appezzamento, perché esistono registrazioni collegate ad un impianto"
                    Else
                        messaggio = "Impossibile modificare la data fine dell'appezzamento, perché esistono registrazioni collegate ad un impianto in data " & Validita_Fine_Agenda
                    End If
                End If
            End If

            If messaggio <> "" Then
                objRisposta.Add(New JProperty("Prosegui", "false"))
                objRisposta.Add(New JProperty("TipoMessaggioRitorno", "alert"))
                objRisposta.Add(New JProperty("TipoControllo", "0"))
                objRisposta.Add(New JProperty("Messaggio", messaggio))

                r.RispostaStringa = objRisposta.ToString

                r.RispostaOK = True
                Return r
            End If



            'CONTROLLO CdG
            Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
            Dim controllo = objControllo.controllo_CdG(Nothing, piva, sa_cod, appezza, 0, 0, validita_inizio, validita_fine, objParametri_Server)

            If controllo.errore Then
                Dim MessaggioErroreCdG As String = "Impossibile modificare le date dell'appezzamento, perché esistono Costi di Gestione collegati ad un esercizio"

                objRisposta.Add(New JProperty("Prosegui", "false"))
                objRisposta.Add(New JProperty("TipoMessaggioRitorno", "alert"))
                objRisposta.Add(New JProperty("TipoControllo", "0"))
                objRisposta.Add(New JProperty("Messaggio", MessaggioErroreCdG))
                r.RispostaStringa = objRisposta.ToString
                r.RispostaOK = True
                Return r

            End If

            'TUTTO OK NO ERRORI
            If messaggio = "" Then

                objRisposta.Add(New JProperty("Prosegui", "true"))
                objRisposta.Add(New JProperty("TipoMessaggioRitorno", "procedi"))
                objRisposta.Add(New JProperty("TipoControllo", "0"))
                objRisposta.Add(New JProperty("Messaggio", ""))
                r.RispostaStringa = objRisposta.ToString

                r.RispostaOK = True
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                        Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    Function controlloCdG(validita_inizio As Date,
                          validita_fine As Date,
                          objParametri_Server As AgronicaCoreParametri) As Boolean

    End Function

    '#################################################################################
    'Private Sub SalvaTutto(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
    '                       Optional ByVal blnValidaImpresa As Boolean = True)

    Private Sub Salva_Tutto()

        'scorporate dal click del btnSalavaTutto...passo il tipo di operazione da compiere.......

        Dim StrAppezzamento As String = ""
        Dim StrXmlInserisci As String
        Dim StrXmlCancella As String = ""

        'Dim Operazione As enum_TipoOperazioneDB

        'Imposto le variabili dummy
        Const ZeroData As String = "0"
        Const ZeroInt As Integer = 0
        Const ZeroString As String = "0"
        Const ZeroDouble As Double = 0
        Const NullString As String = ""

        '  Galassi, 23/06/2017 16.13.48: Tipo_Salvataggio_Particelle contiene la tipologia di salvataggio lavata lato client nella variabile hidden
        Dim Tipo_Salvataggio_Particelle As Integer

        'Dim TipoOperazioneDB As enum_TipoOperazioneDB
        Dim Piva As String
        Dim Sa_Cod As Integer
        Dim Appezza As Integer
        Dim Sup_App As Double
        ' Geolocalizz
        Dim X, Y, Zslm As Double
        ' BufferZone
        Dim DistBZ_AreeResPub, DistBZ_CorpiIdrici, DistBZ_Allevamenti, SupBZ_Riduzione, DistBZ_VegNatNonColt As Double
        Dim Esposizione As String = ""
        Dim Ubicazione As String = ""
        Dim App_Nome As String = ""
        Dim lastPiva As String = ""
        Dim Pendenza As Double
        Dim Campo_Spia As Integer
        Dim Campo_Spia_Area As Double
        Dim Campo_Cod As Integer
        Dim Prossimo As Integer
        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date
        Dim BaseCode As Integer
        Dim TopCode As Integer

        'Dim lastSa_Cod As Integer
        'Dim lastAppezza As Integer

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlDoc2 As New System.Xml.XmlDocument
        Dim XmlDatiAppezzamenti As System.Xml.XmlElement
        Dim XmlAppezzamento As System.Xml.XmlElement

        Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_W

        Dim NoteLog As String = NOTELOG_ANAGRAFE_BOOTSTRAP

        Dim StrDummy As String


        'Dim i, j As Integer

        'Dim Chiave As String

        Dim TipoOperazioneDB As enum_TipoOperazioneDB
        Dim Operazione As enum_TipoOperazioneDB

        'Recupero l'operazione richiesta
        Operazione = objParametriAgenda.Tipo_Operazione

        '------------------------------------------------
        '----- Verifico l'operazione richiesta (Inserimento / Modifica / Cancellazione)
        '------------------------------------------------

        'Recupero l'operazione richiesta, dalla querystring
        'Operazione = CInt(Qs_Operazione)

        Dim app_fine As Date
        Dim app_inizio As Date
        app_fine = objParametri_Server.FinestraTemporaleFine
        app_inizio = objParametri_Server.FinestraTemporaleInizio
        objParametri_Server.FinestraTemporaleFine = Validita_Fine
        objParametri_Server.FinestraTemporaleInizio = Validita_Inizio

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------

        Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))


        '------------------------------------------------
        '----- Costruisco la stringa XML dell'APPEZZAMENTO
        '------------------------------------------------

        'Definisco il tipo di operazione da eseguire
        'TipoOperazioneDB = Operazione


        'Prelevo le informazioni immediate
        Piva = xPiva    'TxtPiva.Text
        Sa_Cod = xSa_Cod        ' TxtSaCod.Text

        'Definisco il tipo di operazione da eseguire
        TipoOperazioneDB = Operazione


        'Test
        'Call ImgBtn_CalcolaSupCatastale_Click(Me, Nothing)

        'controllo se sono in inserimento o modifica, se inserisco il codice appezza lo crea lui in automatico
        If TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura Then
            Appezza = 0
        Else
            Appezza = xAppezza        ' TxtAppezza.Text
        End If


        Dim contr = New SalvataggioWebControl()

        contr.Controlla(TxtCoordinataX, X, 0)
        contr.Controlla(TxtCoordinataY, Y, 0)
        contr.Controlla(TxtCoordinataZ, Zslm, 0)
        contr.Controlla(Cmb_Esposizione, Esposizione, "")
        contr.Controlla(TxtPendenza, Pendenza, 0)
        contr.Controlla(Cmb_Ubicazione, Ubicazione, "")
        contr.Controlla(TxtAppNome, App_Nome, "")
        contr.Controlla(txtDistBZ_Allevamenti, DistBZ_Allevamenti, 0)
        contr.Controlla(txtDistBZ_AreeResPub, DistBZ_AreeResPub, 0)
        contr.Controlla(txtDistBZ_CorpiIdrici, DistBZ_CorpiIdrici, 0)
        contr.Controlla(txtDistBZ_VegNatNonColt, DistBZ_VegNatNonColt, 0)
        contr.Controlla(txtSupBZ_Riduzione, SupBZ_Riduzione, 0)


        If RBL_Catasto_Con.Checked Then
            contr.Controlla(TxtSuperficie, Sup_App, 0)
            contr.Controlla(hidden_tipoParticelleDaSalvare, Tipo_Salvataggio_Particelle, 0)
        End If

        If RBL_Catasto_Senza.Checked Then
            contr.Controlla(TxtSuperficie_SenzaCatasto, Sup_App, 0)
        End If

        'If ChkCampoSpia.Checked = True Then
        '    Campo_Spia = 1
        '    Campo_Spia_Area = CDbl(TxtSupCampoSpia.Text)
        'Else
        '    Campo_Spia = 0
        '    Campo_Spia_Area = 0
        'End If

        ' Prelevo il valore del campo selezionato dalla combo
        ' lo squadro prendo quello che arriva dalla chiamata.. altrimenti quello della combo
        'TODO
        contr.Controlla(Cmb_campo_ass, Campo_Cod, 0)

        contr.Controlla(TxtValiditaInizio, Validita_Inizio, AGRODATAINIZIO)
        contr.Controlla(TxtValiditaFine, Validita_Fine, AGRODATAFINE)

        'prendo il valore dal viewstate....
        'TODO - va??
        Prossimo = ViewState("Next_Val_4_Appezza")


        'Genero la stringa XML
        Call XML_Appezzamento(enum_CodificaDecodifica.Codifica,
                                StrAppezzamento,
                                TipoOperazioneDB,
                                Piva,
                                Sa_Cod,
                                Appezza,
                                Sup_App,
                                ZeroData,
                                ZeroData,
                                X,
                                Y,
                                Zslm,
                                Esposizione,
                                Pendenza,
                                Ubicazione,
                                ZeroInt,
                                NullString,
                                ZeroDouble,
                                ZeroDouble,
                                ZeroDouble,
                                ZeroDouble,
                                ZeroDouble,
                                ZeroDouble,
                                ZeroDouble,
                                ZeroDouble,
                                ZeroDouble,
                                ZeroDouble,
                                ZeroDouble,
                                ZeroDouble,
                                ZeroString,
                                ZeroInt,
                                ZeroDouble,
                                ZeroData,
                                ZeroDouble,
                                ZeroData,
                                ZeroData,
                                ZeroDouble,
                                ZeroData,
                                NullString,
                                App_Nome,
                                Campo_Spia,
                                Campo_Spia_Area,
                                NullString,
                                Campo_Cod,
                                Prossimo,
                                ZeroData,
                                ZeroData,
                                Validita_Inizio,
                                Validita_Fine,
                                BaseCode,
                                TopCode,
                                DistBZ_CorpiIdrici,
                                DistBZ_AreeResPub,
                                DistBZ_Allevamenti,
                                DistBZ_VegNatNonColt,
                                SupBZ_Riduzione)



        '------------------------------------------------
        '----- Costruisco la stringa XML dei CODICI
        '------------------------------------------------

        Dim Indice As Integer
        Dim Id_Cod As Integer
        Dim Val_Cod As String

        Dim StrCodice As String = ""
        Dim StrCodici As String = ""


        Dim DT_Codici As New DataTable

        DT_Codici = HttpContext.Current.Session("dt_Codici")

        If Not IsNothing(DT_Codici) AndAlso DT_Codici.Rows.Count > 0 Then
            For Indice = 0 To DT_Codici.Rows.Count - 1

                'Recupero le informazioni
                Id_Cod = CInt(DT_Codici.Rows(Indice).Item("id_cod"))
                Val_Cod = DT_Codici.Rows(Indice).Item("val_Cod")

                'Genero l'XML del singolo nodo
                Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                StrCodice,
                                enum_TipoOperazioneDB.Scrittura,
                                Id_Cod,
                                Val_Cod,
                                CDate("01/01/1900"),
                                CDate("31/12/2100"),
                                BaseCode,
                                TopCode,
                                "Appezzamento")

                'Inserisco l'XML nella stringa complessiva
                StrCodici = StrCodici & StrCodice

            Next
        End If




        'Riferimento AlfanumericoAppezzamento (utilizzato come riferimento nella scheda di campagna)
        If Txt_Rif_Alfanum_App.Text <> "" Then

            Id_Cod = enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento
            Val_Cod = Txt_Rif_Alfanum_App.Text

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            enum_TipoOperazioneDB.Scrittura,
                            Id_Cod,
                            Val_Cod,
                            CDate("01/01/1900"),
                            CDate("31/12/2100"),
                            BaseCode,
                            TopCode,
                            "Appezzamento")

            'Inserisco l'XML nella stringa complessiva
            StrCodici = StrCodici & StrCodice

        End If

        If Txt_Isola.Text <> "" Then

            Id_Cod = enum_CodiciAnagrafe.Isola
            Val_Cod = Txt_Isola.Text

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            enum_TipoOperazioneDB.Scrittura,
                            Id_Cod,
                            Val_Cod,
                            CDate("01/01/1900"),
                            CDate("31/12/2100"),
                            BaseCode,
                            TopCode,
                            "Appezzamento")

            'Inserisco l'XML nella stringa complessiva
            StrCodici = StrCodici & StrCodice

        End If

        'TODO
        ''TITOLO POSSESSO
        'If Me.cmbTitoloPossesso.SelectedItem.Value <> "0" Then

        '    Id_Cod = enum_CodiciAnagrafe.TitoloPossesso
        '    Val_Cod = Me.cmbTitoloPossesso.SelectedItem.Value

        '    'Genero l'XML del singolo nodo
        '    Call XML_Codice(enum_CodificaDecodifica.Codifica, _
        '                    StrCodice, _
        '                    enum_TipoOperazioneDB.Scrittura, _
        '                    Id_Cod, _
        '                    Val_Cod, _
        '                    CDate("01/01/1900"), _
        '                    CDate("31/12/2100"), _
        '                    BaseCode, _
        '                    TopCode, _
        '                    "Appezzamento")

        '    'Inserisco l'XML nella stringa complessiva
        '    StrCodici = StrCodici & StrCodice

        'End If


        'METODO DI PRODUZIONE
        Dim metProd As String
        If Opt_Convenzionale.Checked Then
            metProd = "1"
        ElseIf Opt_InConversione.Checked Then
            metProd = "2"
        ElseIf Opt_Biologico.Checked Then
            metProd = "3"
        Else
            metProd = "1"
        End If

        Call XML_Codice(enum_CodificaDecodifica.Codifica,
                        StrCodice,
                        enum_TipoOperazioneDB.Scrittura,
                        enum_CodiciAnagrafe.MetodoDiProduzione,
                        metProd,
                        CDate("01/01/1900"),
                        CDate("31/12/2100"),
                        BaseCode,
                        TopCode,
                        "Appezzamento")

        'Inserisco l'XML nella stringa complessiva
        StrCodici = StrCodici & StrCodice

        'Numero Appezzamento Biologico 
        If Txt_NumeroAppBio.Text <> "" Then

            Id_Cod = enum_CodiciAnagrafe.Codice_Appezza_Biologico
            Val_Cod = Txt_NumeroAppBio.Text

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            enum_TipoOperazioneDB.Scrittura,
                            Id_Cod,
                            Val_Cod,
                            CDate("01/01/1900"),
                            CDate("31/12/2100"),
                            BaseCode,
                            TopCode,
                            "Appezzamento")

            'Inserisco l'XML nella stringa complessiva
            StrCodici = StrCodici & StrCodice

        End If



        'DATA FINE IMPIEGO 
        If Me.Txt_DataFineImpiego.Text <> "" Then

            Id_Cod = enum_CodiciAnagrafe.DataFineImpiegoPNC
            Val_Cod = Me.Txt_DataFineImpiego.Text

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            enum_TipoOperazioneDB.Scrittura,
                            Id_Cod,
                            Val_Cod,
                            CDate("01/01/1900"),
                            CDate("31/12/2100"),
                            BaseCode,
                            TopCode,
                            "Appezzamento")

            'Inserisco l'XML nella stringa complessiva
            StrCodici = StrCodici & StrCodice

        End If


        'ORIENTAMENTO

        'If Me.ListOrientamento.Items.Count > 0 Then
        '    Val_Cod = ""
        '    'Recupero le informazioni
        '    Id_Cod = enum_CodiciAnagrafe.OrientamentoProduttivo

        '    For Indice = 0 To Me.ListOrientamento.Items.Count - 1
        '        Val_Cod &= ListOrientamento.Items(Indice).Value & ","
        '    Next

        '    Val_Cod = Left(Val_Cod, Val_Cod.Length - 1)

        '    Call XML_Codice(enum_CodificaDecodifica.Codifica, _
        '                    StrCodice, _
        '                    enum_TipoOperazioneDB.Scrittura, _
        '                    Id_Cod, _
        '                    Val_Cod, _
        '                    CDate("01/01/1900"), _
        '                    CDate("31/12/2100"), _
        '                    BaseCode, _
        '                    TopCode, _
        '                    "Appezzamento")

        '    'Inserisco l'XML nella stringa complessiva
        '    StrCodici = StrCodici & StrCodice

        'End If

        If Not IsNothing(HttpContext.Current.Session("dt_Utilizzo")) Then

            Dim DT_Orientamento As New DataTable

            DT_Orientamento = HttpContext.Current.Session("dt_Utilizzo")
            If DT_Orientamento.Rows.Count > 0 Then
                Dim val_cods As String = ""
                For Indice = 0 To DT_Orientamento.Rows.Count - 1

                    Val_Cod = DT_Orientamento.Rows(Indice).Item("id_cod")
                    val_cods &= Val_Cod & ","

                Next
                val_cods = val_cods.Substring(0, val_cods.Length - 1)

                'Genero l'XML del singolo nodo
                Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                StrCodice,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_CodiciAnagrafe.OrientamentoProduttivo,
                                val_cods,
                                CDate("01/01/1900"),
                                CDate("31/12/2100"),
                                BaseCode,
                                TopCode,
                                "Appezzamento")

                'Inserisco l'XML nella stringa complessiva
                StrCodici = StrCodici & StrCodice

            End If

        End If


        'CONFINI A RISCHIO
        If Me.Txt_ConfiniRischio.Text <> "" Then

            Id_Cod = enum_CodiciAnagrafe.Appezzamento_ConfiniRischio
            Val_Cod = Me.Txt_ConfiniRischio.Text

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            enum_TipoOperazioneDB.Scrittura,
                            Id_Cod,
                            Val_Cod,
                            CDate("01/01/1900"),
                            CDate("31/12/2100"),
                            BaseCode,
                            TopCode,
                            "Appezzamento")

            'Inserisco l'XML nella stringa complessiva
            StrCodici = StrCodici & StrCodice

        End If

        'Coltura Precedente 1 Anno
        If Me.ddl_Coltura_1.SelectedValue <> "" Then

            Id_Cod = enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1
            Val_Cod = Me.ddl_Coltura_1.SelectedValue

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            enum_TipoOperazioneDB.Scrittura,
                            Id_Cod,
                            Val_Cod,
                            CDate("01/01/1900"),
                            CDate("31/12/2100"),
                            BaseCode,
                            TopCode,
                            "Appezzamento")

            'Inserisco l'XML nella stringa complessiva
            StrCodici = StrCodici & StrCodice

        End If


        'Coltura Precedente 2 Anno
        If Me.ddl_Coltura_2.SelectedValue <> "" Then

            Id_Cod = enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2
            Val_Cod = Me.ddl_Coltura_2.SelectedValue

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            enum_TipoOperazioneDB.Scrittura,
                            Id_Cod,
                            Val_Cod,
                            CDate("01/01/1900"),
                            CDate("31/12/2100"),
                            BaseCode,
                            TopCode,
                            "Appezzamento")

            'Inserisco l'XML nella stringa complessiva
            StrCodici = StrCodici & StrCodice

        End If

        'Coltura Precedente 3 Anno
        If Me.ddl_Coltura_3.SelectedValue <> "" Then

            Id_Cod = enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3
            Val_Cod = Me.ddl_Coltura_3.SelectedValue

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            enum_TipoOperazioneDB.Scrittura,
                            Id_Cod,
                            Val_Cod,
                            CDate("01/01/1900"),
                            CDate("31/12/2100"),
                            BaseCode,
                            TopCode,
                            "Appezzamento")

            'Inserisco l'XML nella stringa complessiva
            StrCodici = StrCodici & StrCodice

        End If

        'Coltura Precedente 4 Anno
        If Me.ddl_Coltura_4.SelectedValue <> "" Then

            Id_Cod = enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4
            Val_Cod = Me.ddl_Coltura_4.SelectedValue

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            enum_TipoOperazioneDB.Scrittura,
                            Id_Cod,
                            Val_Cod,
                            CDate("01/01/1900"),
                            CDate("31/12/2100"),
                            BaseCode,
                            TopCode,
                            "Appezzamento")

            'Inserisco l'XML nella stringa complessiva
            StrCodici = StrCodici & StrCodice

        End If

        Dim strIndirizzi As String = ""
        'AppezzamentixIndirizzi
        Dim dtIndirizzi = Session("dtAppezzamentixIndirizzi")
        Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R

        For Each row In dtIndirizzi.rows

            Dim TipoOperazioneIndirizzo = enum_TipoOperazioneDB.Scrittura
            If Not IsDBNull(row("cod_indirizzo")) AndAlso row("cod_indirizzo") <> 0 Then
                TipoOperazioneIndirizzo = enum_TipoOperazioneDB.Modifica
            End If

            Dim siglaProv = ""


            'Lettura Gestione_Gerarchia_Geografica
            Dim DT_Nazioni As DataTable
            Dim objNazioni As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
            DT_Nazioni = objNazioni.Leggi(row("stato"), "", "Descrizione", objParametri_Server)

            Dim pro_cod_istat As String = "000"
            Dim com_cod_istat As String = "000"


            If CInt(DT_Nazioni(0).Item("Gestione_Gerarchia_Geografica")) = 1 Then
                objIstat.Provincia_from_CodIstat(row("pro_cod_istat"), siglaProv, objParametri_Server)

                pro_cod_istat = row("pro_cod_istat")
                com_cod_istat = row("com_cod_istat")

            End If


            Dim strIndirizzo = ""
            Call XML_Indirizzo(enum_CodificaDecodifica.Codifica,
                                strIndirizzo,
                                TipoOperazioneIndirizzo,
                                row("Tipo_Indirizzo"),
                                row("Cod_Indirizzo"),
                                row("ind_des"),
                                row("frz_des"),
                                row("CAP"),
                                row("com_des"),
                                siglaProv,
                                row("stato"),
                                row("note"),
                                pro_cod_istat,
                                com_cod_istat,
                                AGRODATAINIZIO,
                                AGRODATAFINE,
                                BaseCode,
                                TopCode)

            strIndirizzi &= strIndirizzo

        Next

        '------------------------------------------------
        '----- Costruisco la stringa XML complessiva di inserimento
        '------------------------------------------------

        'Creo il nodo "DatiAppezzamenti"
        XmlDatiAppezzamenti = XmlDoc.CreateElement("DatiAppezzamenti")

        'Inserisco il nodo "Appezzamento"
        XmlDatiAppezzamenti.InnerXml = StrAppezzamento

        'Rendo l'albero figlio del documento
        XmlDoc.AppendChild(XmlDatiAppezzamenti)

        'Faccio una copia del documento XML
        XmlDoc2 = XmlDoc

        'Seleziono il nodo Appezzamento e all'interno inserisco i nodi figli...sintassi xpath
        XmlAppezzamento = XmlDoc.SelectSingleNode("//Appezzamento")

        'Aaggiungo i codici creati in precedenza...
        'xmlAppezzamento.InnerXml = StrMetodoProduzione
        XmlAppezzamento.InnerXml = StrCodici & strIndirizzi

        'Estraggo la stringa XML complessiva
        StrXmlInserisci = XmlDoc.InnerXml


        '------------------------------------------------
        '----- Costruisco la stringa XML complessiva di cancellazione
        '------------------------------------------------

        If TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then

            If Session("StrXmlCodiciAttuali").ToString <> "" Then

                'Seleziono il nodo "Appezzamento"
                XmlAppezzamento = XmlDoc2.SelectSingleNode("//Appezzamento")

                'Rendo l'Appezzamento in lettura
                XmlAppezzamento.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Lettura))

                'Inserisco gli elementi "Codice" da cancellare
                XmlAppezzamento.InnerXml = Session("StrXmlCodiciAttuali")

                'Estraggo la stringa XML complessiva
                StrXmlCancella = XmlDoc2.InnerXml

            End If

            Dim objAppezzaxIndirizzi As New AgronicaCoreAnagrafeDAL.AppezzamentixIndirizzi_Read
            Dim dtIndirizzi2 As DataTable = Session("dtAppezzamentixIndirizzi")
            Dim strFiltro = ""
            If dtIndirizzi2.Select(" Cod_Indirizzo <> 0 ").Count > 0 Then
                strFiltro = " Indirizzi.Cod_Indirizzo NOT IN ("
                For Each row In dtIndirizzi2.Rows
                    If Not IsDBNull(row("cod_indirizzo")) AndAlso row("cod_indirizzo") <> 0 Then
                        strFiltro &= CStr(row("cod_indirizzo")) & ", "
                    End If
                Next
                strFiltro = strFiltro.Substring(0, strFiltro.Length - 2)
                strFiltro &= ")"
            End If

            Dim dtIndirizziDelete = objAppezzaxIndirizzi.Leggi(Piva, Sa_Cod, Appezza, 0, strFiltro, "", objParametri_Server)
            Dim strIndirizziDel = ""
            Dim strIndirizzoDel = ""
            If dtIndirizziDelete.Rows.Count > 0 Then
                For Each rowDel In dtIndirizziDelete.Rows
                    Call XML_Indirizzo(enum_CodificaDecodifica.Codifica,
                                       strIndirizzoDel,
                                       enum_TipoOperazioneDB.Cancellazione,
                                       1, rowDel("cod_indirizzo"), "", "", "", "", "", "", "", "", "", AGRODATAINIZIO, AGRODATAFINE, BaseCode, TopCode)
                    strIndirizziDel &= strIndirizzoDel
                Next

                XmlAppezzamento.InnerXml = XmlAppezzamento.InnerXml & strIndirizziDel

                'Estraggo la stringa XML complessiva
                StrXmlCancella = XmlDoc2.InnerXml

            End If

        End If

        'Distruggo gli oggetti
        XmlDatiAppezzamenti = Nothing
        XmlDoc = Nothing
        XmlAppezzamento = Nothing
        XmlDoc2 = Nothing

        If contr.Messaggi <> "" Then
            AgroMsgBox(AgronicaAgenda_2010.SiÈVerificatoUnErroreDuranteLaFaseDiSalvat & vbCrLf & contr.Messaggi, Page)
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

            Dim NomeAppezzamento As String = App_Nome
            Dim intLastAppe As Integer

            '------------------------------------------------
            '----- Se sono in MODIFICA cancello i codici attuali
            '------------------------------------------------

            If TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then

                If Session("StrXmlCodiciAttuali") <> "" Then

                    intLastAppe = objAppezzamento.Appezzamento_Scrivi(
                                CStr(StrXmlCancella),
                                Piva,
                                Sa_Cod,
                                Appezza,
                                objParametri_Server,
                                objParametri_Utenti,
                                NoteLog:=NoteLog)


                End If

            End If




            '------------------------------------------------
            '----- Modifico o Inserisco l'APPEZZAMENTO
            '------------------------------------------------

            intLastAppe = objAppezzamento.Appezzamento_Scrivi(
                                            CStr(StrXmlInserisci),
                                            Piva,
                                            Sa_Cod,
                                            Appezza,
                                            objParametri_Server,
                                            objParametri_Utenti,
                                            NoteLog:=NoteLog)


            '------------------------------------------------
            '----- Se appezzamento biologico setto regolamento bio su esercizio se non impostato
            '------------------------------------------------

            If TipoOperazioneDB = enum_TipoOperazioneDB.Modifica AndAlso metProd = "3" Then

                Dim objImpreseProgetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                Dim dtDistinta = objImpreseProgetti.LeggiDistinta_Attiva_inData(
                    Piva, Sa_Cod, Appezza, 0, Date.Now, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                    "", "", objParametri_Server)

                If dtDistinta IsNot Nothing AndAlso dtDistinta.Rows.Count > 0 Then
                    If dtDistinta.Rows(0).Item("Regolamento_Cod") = enum_Cod_Regolamento.Regolamento_Nessuno Then
                        Dim objImpiantoPro As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_W
                        objImpiantoPro.ModificaSingolo_CampoNumerico(
                            Piva, Sa_Cod, Appezza, dtDistinta.Rows(0).Item("id_reg"), dtDistinta.Rows(0).Item("progetto_cod"),
                            "Regolamento_Cod", enum_Cod_Regolamento.Regolamento_bio, "", objParametri_Server)
                    End If
                End If

            End If

            '------------------------------------------------
            '----- Inserisco le Intersezioni Appezzamento / Particelle Catastali
            '------------------------------------------------


            'Dim SuperficieIntersezione As Double
            'Dim SuperficieIntersezioneMacrouso As Double

            'Dim SuperficieDisponibile As Double
            'Dim SuperficieImpiegata As Double

            'Dim Prov As String
            'Dim Com As String
            'Dim Sezione As String
            'Dim Foglio As Integer
            'Dim Numero As Integer
            'Dim Subalterno As String

            'Dim Macrouso_Cod As String
            'Dim Veg_Cod_Agea As String
            'Dim Cul_Cod_Agea As String

            'Dim Ettari As Integer
            'Dim Are, Centiare As Integer

            'Dim intDummy As Integer

            Dim ObjPartW As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W
            Dim ObjPartMacrW As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousi_W
            Dim ObjPartMacrUtW As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_W

            '----- Se sono in modifica ... Cancello tutte le intersezioni

            If TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then

                ObjPartW.Cancella(
                                        CStr(Piva),
                                        CInt(Sa_Cod),
                                        CInt(Appezza),
                                        "", "", "", 0, 0, "",
                                        "",
                                        objParametri_Server)

                ObjPartMacrW.Cancella(
                                        CStr(Piva),
                                        CInt(Sa_Cod),
                                        CInt(Appezza),
                                        "", "", "", 0, 0, "",
                                        "",
                                        "",
                                        objParametri_Server)

                ObjPartMacrUtW.Cancella(
                                        CStr(Piva),
                                        CInt(Sa_Cod),
                                        CInt(Appezza),
                                        "", "", "", 0, 0, "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        objParametri_Server)

            End If


            ''-----  poi le reinserisco ....
            ' @Paolo: ...SOLO SE E' STATO SELEZIONATO IL CHECK "CON CATASTO"

            If RBL_Catasto_Con.Checked Then
                'richiamo la variabile dove ho salvato il push delle particelle e il tipo di salvataggio e salvo
                Salva_Particelle(Tipo_Salvataggio_Particelle, Validita_Inizio, Validita_Fine)
            End If


            ''Se ci sono particelle ...
            'If DataGridParticelle.Rows.Count > 0 Then

            '    Dim chiave_hash As String
            '    Dim table_hash As New Hashtable
            '    Dim DrParticella() As DataRow
            '    Dim Dt_Particelle As DataTable
            '    Dt_Particelle = DataGridParticelle.DataSource

            '    'Per ciascuna particella ...
            '    For i = 0 To DataGridParticelle.Rows.Count - 1

            '        'Se il check e' selezionato ...
            '        If CType(DataGridParticelle.Rows(i).FindControl("ChkSelezionaParticella"), CheckBox).Checked = True Then

            '            SuperficieIntersezione = 0
            '            'Verifico quanto e' stato inserito come intersezione (eventualmente uso tutta la qta' disponibile)

            '            Prov = DataGridParticelle.Rows(i).Cells(0).Text
            '            Com = DataGridParticelle.Rows(i).Cells(1).Text
            '            Sezione = DataGridParticelle.Rows(i).Cells(5).Text
            '            Foglio = DataGridParticelle.Rows(i).Cells(6).Text
            '            Numero = DataGridParticelle.Rows(i).Cells(7).Text
            '            Subalterno = DataGridParticelle.Rows(i).Cells(8).Text

            '            If Sezione = "&nbsp;" Then Sezione = "0"
            '            If Subalterno = "&nbsp;" Then Subalterno = "0"

            '            'creo la stringa CHIAVE
            '            chiave_hash = Prov & "|" & Com & "|" & Sezione & "|" & Foglio.ToString & "|" & Numero.ToString & "|" & Subalterno

            '            If Not table_hash.ContainsKey(chiave_hash) Then

            '                table_hash.Add(chiave_hash, "")

            '                'cerco nella griglia se la particella è ripetuta
            '                For j = 0 To DataGridParticelle.Rows.Count - 1
            '                    If Prov = DataGridParticelle.Rows(j).Cells(0).Text And _
            '                        Com = DataGridParticelle.Rows(j).Cells(1).Text And _
            '                        Sezione = IIf(DataGridParticelle.Rows(j).Cells(5).Text = "&nbsp;", "0", DataGridParticelle.Rows(j).Cells(5).Text) And _
            '                        Foglio = DataGridParticelle.Rows(j).Cells(6).Text And _
            '                        Numero = DataGridParticelle.Rows(j).Cells(7).Text And _
            '                        Subalterno = IIf(Me.DataGridParticelle.Rows(j).Cells(8).Text = "&nbsp;", "0", DataGridParticelle.Rows(j).Cells(8).Text) And _
            '                        CType(Me.DataGridParticelle.Rows(j).FindControl("ChkSelezionaParticella"), CheckBox).Checked = True Then

            '                        If IsNumeric(CType(DataGridParticelle.Rows(j).FindControl("TxtIntersezione"), TextBox).Text) Then

            '                            SuperficieIntersezioneMacrouso = CDbl(CType(DataGridParticelle.Rows(j).FindControl("TxtIntersezione"), TextBox).Text)
            '                            SuperficieIntersezione += SuperficieIntersezioneMacrouso

            '                            Macrouso_Cod = DataGridParticelle.Rows(j).Cells(22).Text
            '                            Veg_Cod_Agea = DataGridParticelle.Rows(j).Cells(23).Text
            '                            Cul_Cod_Agea = DataGridParticelle.Rows(j).Cells(24).Text

            '                            If Macrouso_Cod <> "" And Macrouso_Cod <> "&nbsp;" Then

            '                                intDummy = ObjPartMacrW.Scrivi( _
            '                                                        CStr(lastPiva), _
            '                                                        CInt(lastSa_Cod), _
            '                                                        CInt(lastAppezza), _
            '                                                        CStr(Prov), _
            '                                                        CStr(Com), _
            '                                                        CStr(Sezione), _
            '                                                        CInt(Foglio), _
            '                                                        CInt(Numero), _
            '                                                        CStr(Subalterno), _
            '                                                        Macrouso_Cod, _
            '                                                        CDbl(SuperficieIntersezioneMacrouso), _
            '                                                        CDate(Validita_Inizio), _
            '                                                        CDate(Validita_Fine), _
            '                                                        objParametri_Server)
            '                            End If

            '                            If Veg_Cod_Agea <> "" And Veg_Cod_Agea <> "&nbsp;" Then

            '                                intDummy = ObjPartMacrUtW.Scrivi( _
            '                                                        CStr(lastPiva), _
            '                                                        CInt(lastSa_Cod), _
            '                                                        CInt(lastAppezza), _
            '                                                        CStr(Prov), _
            '                                                        CStr(Com), _
            '                                                        CStr(Sezione), _
            '                                                        CInt(Foglio), _
            '                                                        CInt(Numero), _
            '                                                        CStr(Subalterno), _
            '                                                        Macrouso_Cod, _
            '                                                        Veg_Cod_Agea, _
            '                                                        Cul_Cod_Agea, _
            '                                                        CDbl(SuperficieIntersezioneMacrouso), _
            '                                                        CDate(Validita_Inizio), _
            '                                                        CDate(Validita_Fine), _
            '                                                        objParametri_Server)
            '                            End If

            '                        End If
            '                    End If
            '                Next

            '                If SuperficieIntersezione <> 0 Then

            '                    Call EttariAreCentiare_from_Ettari(SuperficieIntersezione, Ettari, Are, Centiare)

            '                    intDummy = ObjPartW.Scrivi( _
            '                                        CStr(lastPiva), _
            '                                    CInt(lastSa_Cod), _
            '                                    CInt(lastAppezza), _
            '                                    CStr(Prov), _
            '                                    CStr(Com), _
            '                                    CStr(Sezione), _
            '                                    CInt(Foglio), _
            '                                    CInt(Numero), _
            '                                    CStr(Subalterno), _
            '                                    CDbl(SuperficieIntersezione), _
            '                                    CDbl(Ettari), _
            '                                    CInt(Are), _
            '                                    CInt(Centiare), _
            '                                    CDbl(0), _
            '                                    CInt(0), _
            '                                    CInt(0), _
            '                                    CDbl(0), _
            '                                    CInt(0), _
            '                                    CInt(0), _
            '                                    CDate(Validita_Inizio), _
            '                                    CDate(Validita_Fine), _
            '                                    objParametri_Server)

            '                End If

            '            End If


            '            SuperficieDisponibile = CDbl(DataGridParticelle.Rows(i).Cells(11).Text)
            '            SuperficieImpiegata = CDbl(DataGridParticelle.Rows(i).Cells(21).Text)

            '        End If

            '    Next

            'End If

            '-----------------------------------------------------------------------------
            '----- Preparo il link alla pagina Edit_Impianto x crearlo subito
            '----- in caso si inserimento solo
            '-----------------------------------------------------------------------------

            '  TODO
            ' ''If (TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura) Then

            ' ''    ChiaveAlbero_Codifica(Chiave, _
            ' ''                            enum_TipoNodo.Impianto_Generico, _
            ' ''                            lastPiva, _
            ' ''                            lastSa_Cod, _
            ' ''                            Campo_Cod, _
            ' ''                            lastAppezza, _
            ' ''                            , , , , , , , , , , , , , , , )

            ' ''    'Carico la pagina di edit
            ' ''    Destinazione = "Edit_Impianto_2.aspx" & _
            ' ''                    "?k=" & Stringa_Codifica(Chiave, AgroKey_EncoderDecoder, Server) & _
            ' ''                    "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, Server) & _
            ' ''                    "&p=" & Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server)


            ' ''End If


            '------------------------------------------------

            objParametri_Server.FinestraTemporaleFine = app_fine
            objParametri_Server.FinestraTemporaleInizio = app_inizio


            '------------------------------------------------
            '----- Conferma di aggiornamento del database
            '------------------------------------------------
            '============================
            '===  Fine Aggiornamento  ===
            '============================
            'chiudo la transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            'chiudo la connessione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        Catch exc As Exception

            Errore = True

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------
            'chiudo la transazione con il rollback
            If objParametri_Server.objConnessione IsNot Nothing Then
                If objParametri_Server.objTransazione IsNot Nothing Then
                    'chiudo transazione
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                End If
                'chiudo la connessione
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
            End If

            'Messaggio di errore
            StrDummy = exc.Message.ToString()


            'FACCIO APPARIRE UN ALERT......
            AgroMsgBox(AgronicaAgenda_2010.SiÈVerificatoUnErroreDuranteLaFaseDiSalvat & vbCrLf & StrDummy, Page)

            '------------------------------------------------

        End Try

        '============================
        '===  Fine Aggiornamento  ===
        '============================
        If Not Errore Then
            'Dim Piva As String

            'Ritorno alla pagina AlberoImprese
            'Response.Redirect(Qs_PaginaRitorno & "?P=" & Piva)
            Dim Messaggio As String
            Messaggio = DirectCast(GetLocalResourceObject("APPEZZAMENTOSalvatoConSuccesso"), String) & vbCrLf & vbCrLf

            Dim TargetUrl As String
            Select Case tipo_salva.Value

                Case 1
                    Messaggio &= "Verrà ricaricata la pagina per un ulteriore inserimento" & vbCrLf
                    AgroMsgBox(Messaggio, Page)

                    Page_Load(Nothing, EventArgs.Empty)
                    clear_form()
                Case Else
                    'Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanel_script, , True)

                    TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda)
                    'Response.Redirect(TargetUrl)
                    If Qs_Visibilita <> 0 Then
                        TargetUrl &= "?visibilita=" & CStr(Qs_Visibilita)
                    End If

                    AgroMsgBox(Messaggio, Page, , , "window.location = '" & TargetUrl & "';")

            End Select




        End If

        '============================
        '===  Fine Aggiornamento  ===
        '============================

        ' TODO
        ' ''If Errore = False Then

        ' ''    If Destinazione = "" Then

        ' ''        Piva = Qs_Piva
        ' ''        Chiave = Qs_Key

        ' ''        'Codifico la partita IVA
        ' ''        Piva = Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server)
        ' ''        Chiave = Stringa_Codifica(Chiave, AgroKey_EncoderDecoder, Server)

        ' ''        'Ritorno alla pagina AlberoImprese
        ' ''        Response.Redirect(Qs_PaginaRitorno & "?p=" & Piva & "&k=" & Chiave)

        ' ''    Else

        ' ''        Response.Redirect(Destinazione)

        ' ''    End If

        ' ''End If

    End Sub


    Private Sub clear_form()

        TxtSuperficie_SenzaCatasto.Text = ""
        TxtAppNome.Text = ""
        Cmb_campo_ass.ClearSelection()
        TxtValiditaInizio.Text = ""
        TxtValiditaFine.Text = ""
        Txt_Rif_Alfanum_App.Text = ""
        Txt_Isola.Text = ""
        TxtPendenza.Text = ""
        Cmb_Esposizione.ClearSelection()
        Cmb_Ubicazione.ClearSelection()
        TxtCoordinataX.Text = ""
        TxtCoordinataY.Text = ""
        TxtCoordinataZ.Text = ""
        CmbCodice.ClearSelection()
        TxtCodiceValore2.Text = ""
        TxtValiditaInizioCodice.Text = ""
        TxtValiditaFineCodice.Text = ""

    End Sub

    Private Function controlloSeCampoSquadro(xPiva As String, xSa_Cod As Integer, xCampo_Cod As Integer) As Boolean
        Dim objCampoPart As New AgronicaCoreAnagrafeDAL.CampixParticelle_R
        Dim dt_campi = objCampoPart.Leggi(xPiva, xSa_Cod, xCampo_Cod, "", "", "", 0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        If Not IsNothing(dt_campi) AndAlso dt_campi.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

#Region "TAB"

    ''############################################################################################################
    ''##########     Funzioni per TAB     ########################################################################
    ''############################################################################################################

    ''//la funzione controlla quale tab è stato selezionato prima del postback e ritorna l'indice
    ''//salvato lato client in un campo nascosto prima del postback
    'Public Function CalledFromClient_SetIndexTab() As String
    '    If (clickedTabUI.Value.Equals("")) Then
    '        Return "0"
    '    Else
    '        Return clickedTabUI.Value
    '    End If
    'End Function


    ''evento associato ad un btn non visualizzato, viene richiamato il click lato js che scatena quindi il postback
    'Protected Sub Wrap_Client_PostedBack(ByVal sender As Object, ByVal e As EventArgs)
    '    If (Not fooName_PostedBack.Value.Equals("")) Then
    '        'nel campo hidden ho il nome di funzione da richiamare per il postback
    '        Dim metodo As System.Reflection.MethodInfo = Me.GetType().GetMethod(fooName_PostedBack.Value)
    '        If (Not IsNothing(metodo)) Then
    '            Dim objParam() As Object = {Me, EventArgs.Empty}
    '            metodo.Invoke(Me, objParam)
    '        End If
    '        'svuoto
    '        fooName_PostedBack.Value = ""
    '    End If
    'End Sub

    'Private Sub SelezionaTab(ByVal Controllo As System.Web.UI.Control, ByVal progressivoABaseZero As Integer)

    '    If tabsCnt.Visible = False AndAlso progressivoABaseZero <> 0 Then
    '        progressivoABaseZero -= 1
    '    End If


    '    Dim stb As New StringBuilder

    '    stb.AppendLine("$(function() { ")
    '    stb.AppendLine("    $('#" & tabsCnt.ClientID & "').tabs({ ")
    '    stb.AppendLine("        active: " & progressivoABaseZero)
    '    stb.AppendLine("    }); ")
    '    stb.AppendLine("}); ")

    '    ScriptManager.RegisterClientScriptBlock(Controllo, Controllo.GetType(),
    '                                            String.Format("jQuery_{0}", "spostati"), stb.ToString, True)

    'End Sub


#End Region

    Private Function ricaricaCodiciPresenti(StrCodiciAppezzamento As String) As Boolean

        '------ Ricaricamento codici presenti ----'


        '----- Tabella Appezzamento_Codici

        Dim DTCodici As DataTable
        Dim DTUtilizzi As DataTable

        Dim objCodici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R

        Dim Id_Cod As Integer
        Dim Val_Cod As String = ""
        Dim StrCodice As String = ""
        Dim StrXmlCodiciAttuali As String
        Dim BaseCode As Integer
        Dim TopCode As Integer
        'Dim CodiceAnagrafeDes As String


        DTCodici = objCodici.Leggi(CStr(xPiva),
                                   CInt(xSa_Cod),
                                   CInt(xAppezza),
                                   CInt(Id_Cod),
                                   CStr(Val_Cod),
                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                   "id_cod < 2000 OR id_cod >= 3000",
                                   "",
                                   objParametri_Server)

        '@Paolo: aggiunto per gestione codici di utilizzo
        DTUtilizzi = objCodici.Leggi_Utilizzo_Terreno(CStr(xPiva),
                                   CInt(xSa_Cod),
                                   CInt(xAppezza),
                                   CInt(Id_Cod),
                                   CStr(Val_Cod),
                                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                   "(id_cod between 10 and 99)",
                                   "",
                                   objParametri_Server)

        'Dim objBio As New AgronicaCoreMetaSchemaDAL.BIO_Dati_OrientamentoProduttivo_R
        'DTUtilizzi = objBio.Leggi(0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

        StrXmlCodiciAttuali = ""
        Dim Dtcodici_tab = DTCodici.Clone

        If DTCodici.Rows.Count > 0 Then



            '--------------------------------------------------
            'FILTRO i codici CHIAVE DEI CLIENTI!!!!!!!!!!!!!!!
            '--------------------------------------------------

            For i = 0 To DTCodici.Rows.Count - 1

                If (Not IsDBNull(DTCodici.Rows(i).Item("val_cod"))) Then

                    Id_Cod = DTCodici.Rows(i).Item("id_cod")
                    Val_Cod = DTCodici.Rows(i).Item("val_cod")

                    'Genero l'XML del singolo nodo
                    Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                    StrCodice,
                                    enum_TipoOperazioneDB.Cancellazione,
                                    Id_Cod,
                                    Val_Cod,
                                    CDate("01/01/1900"),
                                    CDate("31/12/2100"),
                                    BaseCode,
                                    TopCode,
                                    "Appezzamento")

                    'Inserisco l'XML nella stringa complessiva
                    StrXmlCodiciAttuali = StrXmlCodiciAttuali & StrCodice

                    Select Case DTCodici.Rows(i).Item("id_cod")

                        Case enum_CodiciAnagrafe.MetodoDiProduzione
                            Select Case DTCodici.Rows(i).Item("val_cod")
                                Case 1
                                    Opt_Convenzionale.Checked = True
                                    Opt_InConversione.Checked = False
                                    Opt_Biologico.Checked = False
                                Case 2
                                    Opt_Convenzionale.Checked = False
                                    Opt_InConversione.Checked = True
                                    Opt_Biologico.Checked = False
                                Case 3
                                    Opt_Convenzionale.Checked = False
                                    Opt_InConversione.Checked = False
                                    Opt_Biologico.Checked = True
                                Case Else
                                    Opt_Convenzionale.Checked = True
                                    Opt_InConversione.Checked = False
                                    Opt_Biologico.Checked = False
                            End Select

                            'If DTCodici.Rows(i).Item("val_cod") = 0 Then
                            '    Me.OptionList_MetodoProduzione.SelectedValue = 1
                            'Else
                            '    Me.OptionList_MetodoProduzione.SelectedValue = DTCodici.Rows(i).Item("val_cod")
                            'End If

                            'Case enum_CodiciAnagrafe.TitoloPossesso
                            '    cmbTitoloPossesso.SelectedIndex = _
                            '        cmbTitoloPossesso.Items.IndexOf(cmbTitoloPossesso.Items.FindByValue( _
                            '            DTCodici.Rows(i).Item("val_cod")))

                        Case enum_CodiciAnagrafe.DataFineImpiegoPNC
                            Me.Txt_DataFineImpiego.Text = DTCodici.Rows(i).Item("val_cod")

                        Case enum_CodiciAnagrafe.Appezzamento_ConfiniRischio
                            Me.Txt_ConfiniRischio.Text = DTCodici.Rows(i).Item("val_cod")

                        Case enum_CodiciAnagrafe.OrientamentoProduttivo
                            OrientamentoProduttivo_GestioneValCod(DTCodici.Rows(i).Item("val_cod"))

                        Case enum_CodiciAnagrafe.Codice_Appezza_Biologico
                            Txt_NumeroAppBio.Text = DTCodici.Rows(i).Item("val_cod")

                        Case enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento
                            Txt_Rif_Alfanum_App.Text = DTCodici.Rows(i).Item("val_cod")

                        Case enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1
                            ddl_Coltura_1.SelectedValue = DTCodici.Rows(i).Item("val_cod")
                        Case enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2
                            ddl_Coltura_2.SelectedValue = DTCodici.Rows(i).Item("val_cod")
                        Case enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3
                            ddl_Coltura_3.SelectedValue = DTCodici.Rows(i).Item("val_cod")
                        Case enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4
                            ddl_Coltura_4.SelectedValue = DTCodici.Rows(i).Item("val_cod")
                        Case enum_CodiciAnagrafe.Isola
                            Txt_Isola.Text = DTCodici.Rows(i).Item("val_cod")
                            'codici inseriti nella listbox
                        Case Else

                            'se nella stringa contenente i codici restituita dal componente è presente..
                            If InStr(StrCodiciAppezzamento, DTCodici.Rows(i).Item("id_cod")) <> 0 Then
                                Dtcodici_tab.ImportRow(DTCodici.Rows(i))
                                'Dim objCodiceAnagrafeR As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
                                'CodiceAnagrafeDes = objCodiceAnagrafeR.CodiceAnagrafeDes_from_CodiceAnagrafeCod(CInt(DTCodici.Rows(i).Item("id_cod")), objParametri_Server)
                                'CmbCodice.Items.Add(New ListItem(CodiceAnagrafeDes & " = " & DTCodici.Rows(i).Item("val_cod"), _
                                '                                  DTCodici.Rows(i).Item("id_cod")))
                                'objCodiceAnagrafeR = Nothing
                            End If

                    End Select

                End If

            Next



        End If

        HttpContext.Current.Session("dt_Codici") = Dtcodici_tab
        jsCodici = DT_to_Json_Codici(Dtcodici_tab)
        Session("StrXmlCodiciAttuali") = StrXmlCodiciAttuali


        '@Paolo: aggiungo i codici degli utilizzi al XML
        StrXmlCodiciAttuali = ""
        'If DTUtilizzi.Rows.Count > 0 Then



        '    '--------------------------------------------------
        '    'FILTRO i codici CHIAVE DEI CLIENTI!!!!!!!!!!!!!!!
        '    '--------------------------------------------------

        '    For i = 0 To DTUtilizzi.Rows.Count - 1

        '        If (Not IsDBNull(DTUtilizzi.Rows(i).Item("val_cod"))) Then

        '            Id_Cod = DTUtilizzi.Rows(i).Item("id_cod")
        '            Val_Cod = DTUtilizzi.Rows(i).Item("val_cod")

        '            'Genero l'XML del singolo nodo
        '            Call XML_Codice(enum_CodificaDecodifica.Codifica, _
        '                            StrCodice, _
        '                            enum_TipoOperazioneDB.Cancellazione, _
        '                            Id_Cod, _
        '                            Val_Cod, _
        '                            CDate("01/01/1900"), _
        '                            CDate("31/12/2100"), _
        '                            BaseCode, _
        '                            TopCode, _
        '                            "Appezzamento")

        '            'Inserisco l'XML nella stringa complessiva
        '            StrXmlCodiciAttuali = StrXmlCodiciAttuali & StrCodice



        '            'se nella stringa contenente i codici restituita dal componente è presente..
        '            If InStr(StrCodiciAppezzamento, DTUtilizzi.Rows(i).Item("id_cod")) <> 0 Then
        '                Dim objCodiceAnagrafeR As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
        '                CodiceAnagrafeDes = objCodiceAnagrafeR.CodiceAnagrafeDes_from_CodiceAnagrafeCod(CInt(DTUtilizzi.Rows(i).Item("id_cod")), objParametri_Server)
        '                CmbCodice.Items.Add(New ListItem(CodiceAnagrafeDes & " = " & DTUtilizzi.Rows(i).Item("val_cod"), _
        '                                                    DTUtilizzi.Rows(i).Item("id_cod")))
        '                objCodiceAnagrafeR = Nothing
        '            End If



        '        End If

        '    Next



        'End If


        'HttpContext.Current.Session("dt_Utilizzo") = DTUtilizzi
        'jsUtilizzo = DT_to_Json_Utilizzo(DTUtilizzi)
        Session("StrXmlCodiciAttuali") &= StrXmlCodiciAttuali
    End Function


End Class