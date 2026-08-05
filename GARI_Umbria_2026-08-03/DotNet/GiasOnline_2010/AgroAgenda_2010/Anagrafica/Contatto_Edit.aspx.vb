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
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports System.Xml


Public Class Contatto_Edit
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda
    Public permessi As PermessiUtente

    Dim xPiva As String
    Dim xSa_Cod As String
    Dim xCod_Contatto As String
    Dim Qs_Visibilita As Integer = 0

    Dim xValiditaInizio As Date
    Dim xValiditaFine As Date

    Dim BaseCode As Integer
    Dim TopCode As Integer

    Public Operazione As Integer

    Public jsIndirizzi As String
    Public jsCosti As String
    Public jsRapporti As String




    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Carica_Comuni(ByVal provincia As String) As Array


        Dim cmb_comuni2 As New DropDownList
        Dim rval(1) As String
        Dim options As String

        If provincia <> "" Then

            AgronicaCoreUtility.CaricaListControl.Comuni(cmb_comuni2, _
                                                             True, "", "", _
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


    <Script.Services.ScriptMethod()> _
<WebMethod(EnableSession:=True)> _
    Public Shared Function GetRandomPiva() As String
        Return GeneraRandom()
    End Function


    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function GetRandomCodFisc() As String
        Return GeneraRandom()
    End Function

    Private Shared Function GeneraRandom() As String

        Dim objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        objParametri.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

        Dim ok = False


        Dim objAgroSe As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim objCont As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim dt As DataTable

        Dim str_r As String
        While ok = False
            str_r = objAgroSe.NuovoId_Tabella("impresa", 0, 0, objParametri).ToString.Replace("-", "F")
            'controllo se è già usato 
            dt = objCont.LeggiContattoSpecifico("", str_r, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
            If dt.Rows.Count = 0 Then
                ok = True
            End If
        End While
        objParametri.ResettaFinestra()

        Return str_r

    End Function


    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Cambia_provincia_salva_codice(ByVal targa As String) As String
        Dim objSchemaDal As New AgronicaCoreMetaSchemaDAL.Istat_R
        Dim codice_prov As String

        codice_prov = objSchemaDal.CodIstat_from_Provincia(targa, HttpContext.Current.Session("ASG_objParametri_Server"))

        Return codice_prov

    End Function


    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Cambia_comune_salva_codice(ByVal provincia As String, ByVal nome_comune As String) As String
        Dim objSchemaDal As New AgronicaCoreMetaSchemaDAL.Istat_R
        Dim Comune As String
        Dim Pro_Cod_Istat As String
        Dim Com_Cod_Istat As String


        objSchemaDal.CodIstat_from_SiglaProvincia_and_StringaComune(provincia, nome_comune, Comune, Pro_Cod_Istat, Com_Cod_Istat, HttpContext.Current.Session("ASG_objParametri_Server"))

        Return Comune & "|" & Com_Cod_Istat

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Salva_Rapporti_In_Session(ByVal dati As String) As String

        Dim DtRapporti As DataTable

        DtRapporti = HttpContext.Current.Session("DT_Rapporti")

        ' Pulisco tutta la tabella (checked = 0)
        For i = 0 To DtRapporti.Rows.Count - 1
            DtRapporti.Rows(i).Item("checked") = 0
        Next

        Dim rap_chk As String() = dati.Split(New Char() {"|"c})

        ' Per ogni chiave di riga di Appezzamento checked
        For Each chiave As String In rap_chk
            If chiave <> "" Then

                ' Ciclo su DT di tutti gli Appezzamenti disponibili per ritrovare la riga
                For i = 0 To DtRapporti.Rows.Count - 1

                    ' Controllo se le chiavi coincidono
                    If CInt(chiave) = DtRapporti.Rows(i).Item("Cod_Rapporto") Then
                        DtRapporti.Rows(i).Item("checked") = 1
                    End If
                Next

            End If
        Next


        HttpContext.Current.Session("DT_Rapporti") = DtRapporti

        Return "ok"

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Controlla_MovimentiContabiliWS(ByVal Cod_Contatto As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim dtMov As DataTable
        Dim objParametriAgenda As New ParametriAgenda

        r.RispostaOK = True
        If objParametriAgenda.TipoOperazioneAgenda <> "1" Then
            Dim objContatto_R As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
            dtMov = objContatto_R.Contatto_ControllaMovimenti(CStr(objParametriAgenda.Piva), Cod_Contatto, "", HttpContext.Current.Session("ASG_objParametri_Server"))

            If dtMov.Rows.Count <> 0 Then
                r.RispostaOK = False
                r.Errore = "Esistono dei movimenti contabili per il contatto corrente. Impossibile cancellare il rapporto contabile selezionato."
            End If

        End If


        Return r
    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Set_Comune(ByVal comune As String, ByVal nome_comune As String)

        HttpContext.Current.Session("comune_settato") = comune
        HttpContext.Current.Session("nome_comune_settato") = nome_comune

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Aggiungi_Indirizzo(ByVal parametri As String) As RispostaStandard

        Dim DT_Indirizzo As DataTable
        Dim r As New RispostaStandard
        Dim trovato As Boolean = False


        DT_Indirizzo = HttpContext.Current.Session("DT_Indirizzo")


        Dim par As String() = parametri.Split(New Char() {"|"c})

        ' Controllo se il tipo di Indirizzo esiste gia
        For i = 0 To DT_Indirizzo.Rows.Count - 1

            If DT_Indirizzo.Rows(i).Item("Tipo_Indirizzo") = par(0) Then
                trovato = True

                r.RispostaOK = False
                r.Errore = "L'indirizzo della tipologia selezionata esiste già."

            End If

        Next

        If Not trovato Then
            Dim dr As DataRow = DT_Indirizzo.NewRow

            dr.Item("Cod_Indirizzo") = 0
            dr.Item("Tipo_Indirizzo") = par(0)
            dr.Item("Tipo_Indirizzo_Desc") = par(1)
            dr.Item("Via") = par(2)
            dr.Item("Sigla_Prov") = par(3)
            dr.Item("Provincia_des") = par(4)
            dr.Item("Provincia_cod") = par(5)
            dr.Item("Comune_cod") = par(6)
            dr.Item("Comune_des") = par(7)
            dr.Item("Frazione") = par(8)
            dr.Item("Cap") = par(9)
            dr.Item("Stato") = par(10)
            dr.Item("Note") = par(11)

            DT_Indirizzo.Rows.Add(dr)

            HttpContext.Current.Session("DT_Indirizzo") = DT_Indirizzo

            r.RispostaOK = True
            r.RispostaStringa = DT_to_Json_Indirizzi(DT_Indirizzo)

        End If



        Return r

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Modifica_Indirizzo(ByVal parametri As String) As String

        Dim DT_Indirizzi As DataTable
        Dim risp

        DT_Indirizzi = HttpContext.Current.Session("Dt_Indirizzi")

        Dim par As String() = parametri.Split(New Char() {"|"c})

        For i = 0 To DT_Indirizzi.Rows.Count - 1

            If DT_Indirizzi.Rows(i).Item("Tipo_Indirizzo") = par(0) Then

                DT_Indirizzi.Rows(i).Item("Tipo_Indirizzo_Desc") = par(1)
                DT_Indirizzi.Rows(i).Item("Via") = par(2)
                DT_Indirizzi.Rows(i).Item("Sigla_Prov") = par(3)
                DT_Indirizzi.Rows(i).Item("Provincia_des") = par(4)
                DT_Indirizzi.Rows(i).Item("Provincia_cod") = par(5)
                DT_Indirizzi.Rows(i).Item("Comune_cod") = par(6)
                DT_Indirizzi.Rows(i).Item("Comune_des") = par(7)
                DT_Indirizzi.Rows(i).Item("Frazione") = par(8)
                DT_Indirizzi.Rows(i).Item("Cap") = par(9)
                DT_Indirizzi.Rows(i).Item("Stato") = par(10)
                DT_Indirizzi.Rows(i).Item("Note") = par(11)

            End If

        Next


        risp = DT_to_Json_Indirizzi(DT_Indirizzi)

        HttpContext.Current.Session("Dt_Indirizzi") = DT_Indirizzi

        Return risp

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Aggiungi_Costo(ByVal parametri As String) As String

        Dim DT_Costi As DataTable
        Dim risp


        DT_Costi = HttpContext.Current.Session("DT_Costi")

        Dim dr As DataRow = DT_Costi.NewRow

        Dim par As String() = parametri.Split(New Char() {"|"c})

        'For i = 0 To DT_Costi.Rows.Count - 1

        '    If DT_Costi.Rows(i).Item("Tipo_Indirizzo") = par(0) Then

        '        DT_Indirizzi.Rows(i).Item("Tipo_Indirizzo_Desc") = par(1)
        '        DT_Indirizzi.Rows(i).Item("Via") = par(2)
        '        DT_Indirizzi.Rows(i).Item("Sigla_Prov") = par(3)
        '        DT_Indirizzi.Rows(i).Item("Provincia_des") = par(4)
        '        DT_Indirizzi.Rows(i).Item("Provincia_cod") = par(5)
        '        DT_Indirizzi.Rows(i).Item("Comune_cod") = par(6)
        '        DT_Indirizzi.Rows(i).Item("Comune_des") = par(7)
        '        DT_Indirizzi.Rows(i).Item("Frazione") = par(8)
        '        DT_Indirizzi.Rows(i).Item("Cap") = par(9)
        '        DT_Indirizzi.Rows(i).Item("Stato") = par(10)
        '        DT_Indirizzi.Rows(i).Item("Note") = par(11)

        '    End If

        'Next

        Dim id As Integer = 0 - (DT_Costi.Rows.Count + 1)

        dr("ID") = id
        dr("Udm_Cod") = par(0)

        If par(1) = "Ettaro" Then
            par(1) = "HA"
        End If

        If par(1) = "Ora" Then
            par(1) = "ORA"
        End If

        dr("Udm_Des") = par(1)
        dr("Validita_Inizio") = par(2)
        dr("Validita_Fine") = par(3)
        dr("Prezzo_Unitario") = par(4)

        DT_Costi.Rows.Add(dr)


        risp = DT_to_Json_Costi(DT_Costi)

        HttpContext.Current.Session("DT_Costi") = DT_Costi

        Return risp

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Controlla_Costo(ByVal parametri As String) As RispostaStandard

        Dim DT_Costi As DataTable
        Dim r As New RispostaStandard
        Dim interseca As Boolean = False

        Dim par As String() = parametri.Split(New Char() {"|"c})

        Dim dataI As Date = CDate(par(1))
        Dim dataF As Date = CDate(par(2))

        DT_Costi = HttpContext.Current.Session("DT_Costi")

        r.RispostaOK = True
        'r.RispostaStringa = str_Risposta

        For i = 0 To DT_Costi.Rows.Count - 1

            Dim unita As String = par(0).ToUpper()
            If unita = "ETTARO" Then
                unita = "HA"
            End If

            ' Controllo se hanno la stessa unità di misura
            If DT_Costi.Rows(i).Item("Udm_Des") = unita Then

                ' Controllo se le date sono intersecate
                If (dataI >= CDate(DT_Costi.Rows(i).Item("Validita_Inizio")) AndAlso dataI <= CDate(DT_Costi.Rows(i).Item("Validita_Fine"))) OrElse (dataF <= CDate(DT_Costi.Rows(i).Item("Validita_Inizio")) AndAlso dataF >= CDate(DT_Costi.Rows(i).Item("Validita_Fine"))) Then
                    interseca = True
                    r.RispostaOK = False
                    r.Errore = "Le date inserite si intersecano con alcune precedentemente inserite"
                    Exit For
                End If

            End If
        Next

        Return r

    End Function



    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function DT_to_Json_Costi(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)


        Dim cn As New ColonneNome("ID", "", "string")
        cn._FormatoParticolare = "<span></span>"
        cn._Filtrabile = False
        'cn._css = "prova"
        l.Add(cn)

        ''aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("ID", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            'listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaIndirizzo(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaCosto(this);"))
        End If

        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)

        ''Dim cn As New ColonneNome("Contatore", "Contatore", "string")
        ''l.Add(cn)

        ''Dim cn As New ColonneNome("Id_Cod", "Id_Cod", "string")
        ''cn._hidden = True
        ''l.Add(cn)


        cn = New ColonneNome("Udm_Des", "Unita di Misura", "string")
        l.Add(cn)

        cn = New ColonneNome("Validita_Inizio", "Data Inizio", "date")
        l.Add(cn)

        cn = New ColonneNome("Validita_Fine", "Data fine", "date")
        l.Add(cn)

        cn = New ColonneNome("Prezzo_Unitario", "Prezzo", "string")
        cn._css = "wacol_provincia"
        l.Add(cn)


        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function



    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function DT_to_Json_Indirizzi(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)


        Dim cn As New ColonneNome("Cod_Indirizzo", "", "string")
        cn._FormatoParticolare = "<span></span>"
        cn._Filtrabile = False
        'cn._css = "prova"
        l.Add(cn)

        ''aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Cod_Indirizzo", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaIndirizzo(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaIndirizzo(this);"))
        End If

        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)

        ''Dim cn As New ColonneNome("Contatore", "Contatore", "string")
        ''l.Add(cn)

        cn = New ColonneNome("Cod_Indirizzo", "Cod_Indirizzo", "string")
        cn._css = "wacol_cod"
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("Tipo_Indirizzo", "Tipo_Indirizzo", "string")
        cn._css = "wacol_tipologia_cod"
        cn._hidden = True
        l.Add(cn)


        cn = New ColonneNome("Tipo_Indirizzo_Desc", "Tipologia", "string")
        cn._css = "wacol_tipologia"
        l.Add(cn)

        cn = New ColonneNome("Via", "Indirizzo", "string")
        cn._css = "wacol_indirizzo"
        l.Add(cn)

        cn = New ColonneNome("Provincia_cod", "Provincia_cod", "string")
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("Sigla_Prov", "Prov", "string")
        cn._css = "wacol_provincia"
        l.Add(cn)

        cn = New ColonneNome("Comune_cod", "Comune_cod", "string")
        cn._hidden = True
        cn._css = "wacol_comune_cod"
        l.Add(cn)

        cn = New ColonneNome("Comune_des", "Comune", "string")
        cn._css = "wacol_comune"
        l.Add(cn)

        cn = New ColonneNome("Frazione", "Frazione", "string")
        cn._css = "wacol_frazione"
        l.Add(cn)

        cn = New ColonneNome("Cap", "Cap", "string")
        cn._css = "wacol_cap"
        l.Add(cn)

        cn = New ColonneNome("Stato", "Stato", "string")
        cn._css = "wacol_stato"
        l.Add(cn)

        cn = New ColonneNome("Note", "Note", "string")
        cn._css = "wacol_note"
        l.Add(cn)



        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function DT_to_Json_Rapporti(ByVal dt As DataTable) As String
        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)


        Dim cn As New ColonneNome("Cod_Rapporto", "Cod_Rapporto", "string")
        'cn._FormatoParticolare = "<span></span>"
        cn._Filtrabile = False
        cn._ColonnaDiSelezione = True
        cn._hidden = True
        'cn._css = "prova"
        l.Add(cn)

        ''aggiungo i pulsanti per modifica ed eliminazione
        'Dim c = New ColonneNome("Cod_RisUm", "Tool", "string")

        'Dim listaBtn = New List(Of btnAzioni)
        'listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaCodice(this);"))
        'listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaCodice(this);"))

        'Dim tool As New ToolStandard(listaBtn)
        'tool.cssColonna = "colmodmovimenti"
        'c._FormatoParticolare = tool.toString()
        'c._Filtrabile = False
        'c._ColonnaDiSelezione = True
        'l.Add(c)

        cn = New ColonneNome("Rapporto_Des", "Rapporto Contabile", "string")
        cn._css = "wacol_rapporto_desc"
        l.Add(cn)

        cn = New ColonneNome("Sa_Cod", "Sa_Cod", "string")
        cn._css = "wacol_sa_cod"
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("Cod_RisUm", "Cod_RisUm", "string")
        cn._css = "wacol_Cod_RisUm"
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("validita_inizio", "validita_inizio", "string")
        cn._css = "wacol_validita_inizio"
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("validita_fine", "validita_fine", "string")
        cn._css = "wacol_validita_fine"
        cn._hidden = True
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


    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Aggiorna_RapportiWS(ByVal tipo_persona As Integer) As String

        Dim DtRapporti As DataTable
        Dim DtRapportiFiltrati As DataTable
        Dim Dr() As DataRow
        Dim strFiltro As String = ""
        Dim i As Integer

        Dim risp As String = ""

        If HttpContext.Current.Session("Dt_Rapporti") IsNot Nothing Then

            DtRapporti = HttpContext.Current.Session("Dt_Rapporti")
            DtRapportiFiltrati = DtRapporti.Clone

            If tipo_persona = 0 Then
                'persona fisica
                strFiltro = " Sa_Cod in (0,2) "
            Else
                'giuridica
                strFiltro = " Sa_Cod in (0,1) "
            End If

            Dr = DtRapporti.Select(strFiltro)

            For i = 0 To Dr.Length - 1
                DtRapportiFiltrati.ImportRow(Dr(i))
            Next

            Dim DtKey(1) As String
            DtKey(0) = "Cod_Rapporto"
            DtKey(1) = "Cod_RisUm"

            'GridViewRapporti.DataSource = DtRapportiFiltrati
            'GridViewRapporti.DataKeyNames = DtKey
            'GridViewRapporti.DataBind()

            HttpContext.Current.Session("RBL_TipoUtente") = tipo_persona

            risp = DT_to_Json_Rapporti(DtRapportiFiltrati)

        End If

        Return risp

    End Function





    <Script.Services.ScriptMethod()> _
   <WebMethod(EnableSession:=True)> _
    Public Shared Function Aggiorna_Visibilita(ByVal tipo_persona As Integer) As String

        Dim Cmb_CentriAziendali As New DropDownList

        Select Case tipo_persona

            Case 0

                'solo le persone fisiche rendo possibile l'assegnazione ad un centro
                AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(Cmb_CentriAziendali, True, "Contatto Aziendale", "0", HttpContext.Current.Session("xPiva"), False, 2, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))
                Select Case HttpContext.Current.Session("UtenteAbilitato_Pubblico")
                    Case True
                        Cmb_CentriAziendali.Items.Add(New ListItem("Contatto movimentabile da tutte le imprese", "-1"))
                End Select

            Case 1

                Cmb_CentriAziendali.Items.Add(New ListItem("Contatto Aziendale", "0"))
                Cmb_CentriAziendali.Items.Add(New ListItem("Contatto movimentabile da tutte le imprese", "-1"))

        End Select

        Dim rval As String = ""
        For Each itm As ListItem In Cmb_CentriAziendali.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Elimina_Costo(ByVal id As String) As String

        Dim DT_Costi As DataTable
        Dim risp As String

        DT_Costi = HttpContext.Current.Session("DT_Costi")

        For i = 0 To DT_Costi.Rows.Count - 1

            If DT_Costi.Rows(i).Item("ID") = id Then

                DT_Costi.Rows(i).Delete()
                Exit For
            End If

        Next


        risp = DT_to_Json_Costi(DT_Costi)

        HttpContext.Current.Session("DT_Costi") = DT_Costi

        Return risp

    End Function



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

        objParametriAgenda = New ParametriAgenda
        'Dim xChiave As String
        xPiva = objParametriAgenda.Piva
        xSa_Cod = objParametriAgenda.Sa_Cod
        xCod_Contatto = objParametriAgenda.Cod_Contatto

        HttpContext.Current.Session("xPiva") = objParametriAgenda.Piva

        If Request.QueryString("visibilita") IsNot Nothing AndAlso IsNumeric(Request.QueryString("visibilita")) Then
            Qs_Visibilita = CInt(Request.QueryString("visibilita"))
        End If

        'Qs_Key = Stringa_Decodifica(Request.QueryString("k").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Qs_Operazione = Stringa_Decodifica(Request.QueryString("o").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Qs_PaginaRitorno = Stringa_Decodifica(Request.QueryString("r").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Recupero Chiave ed Operazione dalla querystring
        'xChiave = Qs_Key
        'Operazione = Qs_Operazione
        'xPiva = Qs_Piva

        Select Case objParametriAgenda.Tipo_Operazione
            Case enum_TipoOperazioneDB.Scrittura
                Master.Lbl_Titolo.Text = "Creazione Nuovo Contatto"
                objParametriAgenda.Sa_Cod = 0
            Case enum_TipoOperazioneDB.Lettura
                Master.Lbl_Titolo.Text = "Lettura Contatto"
            Case enum_TipoOperazioneDB.Modifica
                Master.Lbl_Titolo.Text = "Modifica Contatto"
        End Select

        Operazione = objParametriAgenda.Tipo_Operazione
        HttpContext.Current.Session("operazione") = Operazione

        'Call ChiaveAlbero_Decodifica_PartitaIVA(Qs_Key, Qs_PivaPadre)


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

        'Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica


        UtenteAbilitato_Lettura = permessi.getPermesso(enum_Security_Attivita.Anagrafica_Contatto).Lettura
        UtenteAbilitato_Modifica = permessi.getPermesso(enum_Security_Attivita.Anagrafica_Contatto).Scrittura


        Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica
        Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura


        If Not UtenteAbilitato_Modifica Then
            Response.Redirect("../MenuAnagrafica/Menubs_anagrafica.aspx")
            Exit Sub
        End If






        If Not Page.IsPostBack Then
            HttpContext.Current.Session("Dt_Costi") = Nothing
            HttpContext.Current.Session("Dt_Costi_Origine") = Nothing

            Dim UtenteAbilitato_Pubblico As Boolean = False
            UtenteAbilitato_Pubblico = objPermessi.Controlla_Permessi_Utente( _
                                       Session("ASG_Utente_Username"), _
                                       Session("ASG_IdServizio"), _
                                       enum_Security_Attivita.Modifica_Contatti_Pubblici, _
                                       enum_Security_Operazione.Modifica, _
                                       Date.Now, _
                                       "", _
                                       objParametri_Utenti)

            Session("UtenteAbilitato_Pubblico") = UtenteAbilitato_Pubblico


            GeneraDTIndirizzi()
            GeneraDTRapportiContabili()

            ViewState("nPrezzi") = -1
            GeneraDTCosti()

            CaricaTipologiaIndirizzo(0)


        Else

            '==========================================
            '===== Pagina ricaricata in POSTBACK
            '==========================================

            Exit Sub

        End If


        AgronicaCoreUtility.CaricaListControl.Provincie(ddl_provincia, True, "Seleziona la provincia", "-1", False, 1, "", "", "", "", "", "", "", objParametri_Server)


        HttpContext.Current.Session("RBL_TipoUtente") = 0


        ' Riempio la tendina delle nazioni
        Dim DT_Nazioni As DataTable
        Dim objNazioni As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
        DT_Nazioni = objNazioni.Leggi("", "", "", objParametri_Server)

        cmb_Stato.Items.Add(New ListItem("SELEZIONA", ""))

        For i = 0 To DT_Nazioni.Rows.Count - 1
            cmb_Stato.Items.Add(New ListItem(DT_Nazioni.Rows(i).Item("Descrizione"), DT_Nazioni.Rows(i).Item("Codice")))
        Next

        ' setto Italia come default
        cmb_Stato.SelectedIndex = cmb_Stato.Items.IndexOf(cmb_Stato.Items.FindByValue("IT"))




        Dim Dt_Rapporti As DataTable
        Dim Dt_Costi As DataTable
        Dim Dt_Indirizzi As DataTable

        '##############################################################
        '#####  Se sono in MODIFICA carico i dati  ####################
        '##############################################################

        If Operazione = enum_TipoOperazioneDB.Scrittura Then

            RBL_TipoUtente.SelectedValue = 0

            'solo le persone fisiche rendo possibile l'assegnazione ad un centro
            AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(Cmb_CentriAziendali, True, "Contatto Aziendale", "0", xPiva, False, 2, "", "", objParametri_Server)
            Select Case Session("UtenteAbilitato_Pubblico")
                Case True
                    Cmb_CentriAziendali.Items.Add(New ListItem("Contatto movimentabile da tutte le imprese", "-1"))
            End Select

            Dt_Rapporti = Nothing

            GeneraDTRapportiContabili()

            Dt_Rapporti = ViewState("Dt_Rapporti")
            Dt_Costi = ViewState("Dt_Costi")
            Dt_Indirizzi = ViewState("Dt_Indirizzi")

            HttpContext.Current.Session("Dt_Rapporti") = Dt_Rapporti
            HttpContext.Current.Session("Dt_Costi") = Dt_Costi
            HttpContext.Current.Session("Dt_Costi_Origine") = Dt_Costi
            HttpContext.Current.Session("Dt_Indirizzi") = Dt_Indirizzi

            rowModificaCF.Visible = False
            rowModificaPiva.Visible = False

        ElseIf Operazione = enum_TipoOperazioneDB.Modifica OrElse Operazione = enum_TipoOperazioneDB.Lettura Then

            If Operazione = enum_TipoOperazioneDB.Modifica Then
                rowModificaCF.Visible = True
                rowModificaPiva.Visible = True
            End If

            ' Leggo gli indirizzi del contatto
            Dim objContatto_R As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
            Dim Dati As String
            Dati = objContatto_R.Contatto_Leggi(xPiva, xCod_Contatto, "", False, objParametri_Server)

            'sono in info di un contatto legato ad un'altra azienda
            If Dati = "" Then
                Dati = objContatto_R.Contatto_Leggi("", xCod_Contatto, "", False, objParametri_Server)
            End If

            If Dati <> "" Then

                Dim doc As XDocument
                doc = XDocument.Parse(Dati)
                Dim risultato = (From c In doc.<DatiContatti>.<Contatto> _
                               Select c).FirstOrDefault


                doc = XDocument.Parse(Dati)
                Dim risultato_Indirizzi = (From c In doc.<DatiContatti>.<Contatto>.<Indirizzo> _
                               Select c).ToList

                Cmb_CentriAziendali.Items.Clear()

                ImgBtn_CF.Enabled = False

                If risultato.@id_cf = PERSONA_GIURIDICA Then
                    RBL_TipoUtente.SelectedValue = 1
                    Cmb_CentriAziendali.Items.Add(New ListItem("Contatto Aziendale", "0"))
                    Cmb_CentriAziendali.Items.Add(New ListItem("Contatto movimentabile da tutte le imprese", "-1"))

                    Txt_Piva.Text = risultato.@cod_contatto
                    Txt_Rag_Soc.Text = risultato.@rag_soc

                    Txt_Piva.Enabled = False

                Else
                    RBL_TipoUtente.SelectedValue = 0
                    Txt_CF.Text = risultato.@cod_contatto
                    Txt_Nome.Text = risultato.@nome
                    Txt_Cognome.Text = risultato.@cognome
                    If Txt_Nome.Text = "" AndAlso Txt_Cognome.Text = "" Then
                        Txt_Nome.Text = risultato.@rag_soc
                    End If
                    If risultato.@data_nascita <> AGRODATAINIZIO Then
                        Txt_DataNascita.Text = risultato.@data_nascita
                    End If
                    ddl_Sesso.SelectedValue = risultato.@sesso

                    Txt_CF.Enabled = False

                    'solo le persone fisiche rendo possibile l'assegnazione ad un centro
                    AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(Cmb_CentriAziendali, True, "Contatto Aziendale", "0", xPiva, False, 2, "", "", objParametri_Server)
                    Select Case Session("UtenteAbilitato_Pubblico")
                        Case True
                            Cmb_CentriAziendali.Items.Add(New ListItem("Contatto movimentabile da tutte le imprese", "-1"))
                    End Select

                End If

                Me.Cmb_CentriAziendali.SelectedIndex = _
                    Cmb_CentriAziendali.Items.IndexOf( _
                        Cmb_CentriAziendali.Items.FindByValue(risultato.@sa_cod))


                ' Contatti
                Dim risultato_contatti = (From c In doc.<DatiContatti>.<Contatto>.<Rubrica> _
                         Select descr = c.@descr, numero = c.@numero).ToList

                For i = 0 To risultato_contatti.Count - 1
                    Select Case risultato_contatti(i).descr.Trim.ToLower
                        Case "n. telefono:", "telefono", "tel.", "telefono:", "tel.:", "n.telefono", "tel"
                            Txt_Telefono.Text = risultato_contatti(i).numero
                        Case "cellulare:", "cellulare", "cell", "cell.", "cellure"
                            Txt_Cellulare.Text = risultato_contatti(i).numero
                        Case "fax:", "fax"
                            Txt_Fax.Text = risultato_contatti(i).numero
                        Case "e-mail:", "email", "e-mail", "e.mail", "email", "email.", "mail"
                            Txt_Mail.Text = risultato_contatti(i).numero
                    End Select
                Next


                ' indirizzi


                If ViewState("Dt_Indirizzi") IsNot Nothing Then

                    Dt_Indirizzi = ViewState("Dt_Indirizzi")

                    For i = 0 To risultato_Indirizzi.Count - 1

                        Dim drInd As DataRow = Dt_Indirizzi.NewRow

                        drInd.Item("Cod_Indirizzo") = risultato_Indirizzi(i).@cod_indirizzo
                        drInd.Item("Tipo_Indirizzo") = risultato_Indirizzi(i).@tipo_indirizzo

                        Select Case risultato_Indirizzi(i).@tipo_indirizzo
                            Case 3
                                drInd.Item("Tipo_Indirizzo_Desc") = "Residenza"
                            Case 5
                                drInd.Item("Tipo_Indirizzo_Desc") = "Luogo di Nascita"
                            Case 2
                                drInd.Item("Tipo_Indirizzo_Desc") = "Domicilio"
                            Case 4
                                drInd.Item("Tipo_Indirizzo_Desc") = "Residenza Estiva"
                            Case 1
                                drInd.Item("Tipo_Indirizzo_Desc") = "Sede Operativa"
                            Case 101
                                drInd.Item("Tipo_Indirizzo_Desc") = "Sede Legale"
                            Case 102
                                drInd.Item("Tipo_Indirizzo_Desc") = "Sede Aziendale"
                            Case 103
                                drInd.Item("Tipo_Indirizzo_Desc") = "Stabilimento"
                        End Select

                        drInd.Item("Via") = risultato_Indirizzi(i).@ind_des
                        drInd.Item("Frazione") = risultato_Indirizzi(i).@frz_des
                        drInd.Item("cap") = risultato_Indirizzi(i).@cap
                        drInd.Item("Provincia_des") = risultato_Indirizzi(i).@pro_des
                        drInd.Item("Provincia_cod") = risultato_Indirizzi(i).@pro_cod_istat
                        drInd.Item("Comune_des") = risultato_Indirizzi(i).@com_des
                        drInd.Item("Comune_cod") = risultato_Indirizzi(i).@com_cod_istat
                        drInd.Item("Sigla_Prov") = risultato_Indirizzi(i).@pro_cod
                        drInd.Item("Stato") = risultato_Indirizzi(i).@stato
                        drInd.Item("Note") = risultato_Indirizzi(i).@note

                        Dt_Indirizzi.Rows.Add(drInd)
                    Next

                End If

                ViewState("Dt_Indirizzi") = Dt_Indirizzi
                HttpContext.Current.Session("Dt_Indirizzi") = Dt_Indirizzi





                Dim Patentino As String = ""
                Dim Patentino_Rilascio As String = ""
                Dim Patentino_Scadenza As String = ""
                Dim Patentino_Ente As String = ""
                Dim nrBadge As String = ""


                Dim ID As Integer
                Dim ArrayID() As Integer
                Dim i_id As Integer = 0
                Dim UdmCod_Prezzo As Integer
                Dim UdmDes_Prezzo As String
                Dim InizioPrezzo As Date
                Dim FinePrezzo As Date
                Dim Prezzo As Decimal

                'controllo il rapporto contabile
                Dim risultato_Rapp = (From c In doc.<DatiContatti>.<Contatto>.<RapCon> _
                               Select c).ToList





                If ViewState("Dt_Rapporti") IsNot Nothing Then

                    Dt_Rapporti = ViewState("Dt_Rapporti")

                    For i = 0 To risultato_Rapp.Count - 1

                        For j = 0 To Dt_Rapporti.Rows.Count - 1
                            If risultato_Rapp(i).@cod_rapporto = Dt_Rapporti.Rows(j).Item("cod_rapporto") Then
                                Dt_Rapporti.Rows(j).Item("Cod_RisUm") = risultato_Rapp(i).@cod_risum
                                If risultato_Rapp(i).@validita_inizio = AGRODATAINIZIO Then
                                    Dt_Rapporti.Rows(j).Item("validita_inizio") = ""
                                Else
                                    Dt_Rapporti.Rows(j).Item("validita_inizio") = risultato_Rapp(i).@validita_inizio
                                End If
                                If risultato_Rapp(i).@validita_fine = AGRODATAFINE Then
                                    Dt_Rapporti.Rows(j).Item("validita_fine") = ""
                                Else
                                    Dt_Rapporti.Rows(j).Item("validita_fine") = risultato_Rapp(i).@validita_fine
                                End If
                                'Dt_Rapporti.Rows(j).Item("Settore_Des") = risultato_Rapp(i).@settore_des
                                'Dt_Rapporti.Rows(j).Item("attivita_des") = risultato_Rapp(i).@attivita_des

                                Dt_Rapporti.Rows(j).Item("checked") = 1

                                Txt_Progressivo.Text = risultato_Rapp(i).@settore_des
                                Txt_Attivita.Text = risultato_Rapp(i).@attivita_des

                                Dim risultato_Costi = (From c In risultato_Rapp(i).<DatiProdotti_Costi>.<Prodotto_Costo> _
                                               Select c).ToList

                                Dt_Costi = ViewState("Dt_Costi")

                                For x = 0 To risultato_Costi.Count - 1

                                    ID = risultato_Costi(x).@id
                                    ReDim Preserve ArrayID(i_id)
                                    ArrayID(i_id) = ID
                                    i_id += 1

                                    UdmCod_Prezzo = risultato_Costi(x).@mezzo
                                    Select Case UdmCod_Prezzo
                                        Case enum_TipoMezzo.Ettaro
                                            UdmDes_Prezzo = "HA"
                                        Case Else
                                            UdmDes_Prezzo = "ORA"
                                    End Select
                                    InizioPrezzo = risultato_Costi(x).@validita_inizio
                                    FinePrezzo = risultato_Costi(x).@validita_fine
                                    Prezzo = risultato_Costi(x).@prezzo_unitario

                                    'ID = 0
                                    InserisciRigaCosto(ID, UdmCod_Prezzo, UdmDes_Prezzo, InizioPrezzo, FinePrezzo, Prezzo)

                                    'Dim dr As DataRow = Dt_Costi.NewRow

                                    'dr.Item("ID") = ID
                                    'dr.Item("Udm_Cod") = UdmCod_Prezzo
                                    'dr.Item("Udm_Des") = UdmDes_Prezzo
                                    'dr.Item("Validita_Inizio") = InizioPrezzo
                                    'dr.Item("Validita_Fine") = FinePrezzo

                                    'If InizioPrezzo = AGRODATAINIZIO Then
                                    '    dr.Item("strValidita_Inizio") = "..."
                                    'Else
                                    '    dr.Item("strValidita_Inizio") = CDate(InizioPrezzo).ToShortDateString
                                    'End If

                                    'If FinePrezzo = AGRODATAFINE Then
                                    '    dr.Item("strValidita_Fine") = "..."
                                    'Else
                                    '    dr.Item("strValidita_Fine") = CDate(FinePrezzo).ToShortDateString
                                    'End If

                                    'dr.Item("Prezzo_Unitario") = Prezzo

                                    'Dt_Costi.Rows.Add(dr)


                                Next

                                Exit For

                            End If
                        Next

                        If risultato.@id_cf = PERSONA_FISICA Then
                            If Patentino = "" Then
                                Patentino = risultato_Rapp(i).@patentino
                                If risultato_Rapp(i).@data_rilascio_patentino <> AGRODATAINIZIO Then
                                    Patentino_Rilascio = risultato_Rapp(i).@data_rilascio_patentino
                                End If
                                If risultato_Rapp(i).@data_scadenza_patentino <> AGRODATAFINE Then
                                    Patentino_Scadenza = risultato_Rapp(i).@data_scadenza_patentino
                                End If
                                Patentino_Ente = risultato_Rapp(i).@ente_di_rilascio
                            End If
                        End If

                    Next

                End If

                nrBadge = risultato.@nrBadge

                Txt_Patentino.Text = Patentino
                Txt_Patentino_Rilascio.Text = Patentino_Rilascio
                Txt_Patentino_Scadenza.Text = Patentino_Scadenza
                Txt_Patentino_Ente.Text = Patentino_Ente
                Txt_Badge.Text = nrBadge


                ViewState("ArrayID") = ArrayID
                ViewState("Dt_Costi") = Dt_Costi
                ViewState("Dt_Rapporti") = Dt_Rapporti
                HttpContext.Current.Session("Dt_Rapporti") = Dt_Rapporti
                HttpContext.Current.Session("Dt_Costi") = Dt_Costi

                HttpContext.Current.Session("Dt_Costi_Origine") = Dt_Costi.Copy()

            End If
        End If

        jsRapporti = DT_to_Json_Rapporti(Dt_Rapporti)
        jsCosti = DT_to_Json_Costi(Dt_Costi)
        jsIndirizzi = DT_to_Json_Indirizzi(Dt_Indirizzi)



        ' Disabilito tutti i controlli se in Lettura
        If Operazione = enum_TipoOperazioneDB.Lettura Then

            RBL_TipoUtente.Enabled = False
            Cmb_CentriAziendali.Enabled = False
            Txt_CF.Enabled = False
            Txt_Cognome.Enabled = False
            Txt_Nome.Enabled = False
            Txt_DataNascita.Enabled = False
            ddl_Sesso.Enabled = False
            Txt_Telefono.Enabled = False
            Txt_Cellulare.Enabled = False
            Txt_Fax.Enabled = False
            Txt_Mail.Enabled = False
            Txt_Patentino.Enabled = False
            Txt_Patentino_Rilascio.Enabled = False
            Txt_Patentino_Scadenza.Enabled = False
            Txt_Patentino_Ente.Enabled = False
            Txt_Progressivo.Enabled = False
            Txt_Attivita.Enabled = False

            ddl_tipo_Indirizzo.Enabled = False
            txt_via.Enabled = False
            ddl_provincia.Enabled = False
            Txt_ComCodIstat.Enabled = False
            txt_cap.Enabled = False
            'txt_stato.Enabled = False
            cmb_Stato.Enabled = False
            Txt_Note_Indirizzo.Enabled = False
            txt_frazione.Enabled = False

            ImgBtn_Aggiungi_Indirizzo.Visible = False
            ImgBtn_CF.Visible = False

            ddl_UdmCod_Prezzo.Enabled = False
            Txt_Prezzo.Enabled = False
            Txt_Inizio_Prezzo.Enabled = False
            Txt_Fine_Prezzo.Enabled = False

            ImgBtn_Aggiungi_Costo.Visible = False

        End If



    End Sub





    Private Sub GeneraDTRapportiContabili()

        Dim DT As New DataTable
        Dim Dr As DataRow

        DT.Columns.Add(New DataColumn("Rapporto_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("Cod_Rapporto", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))

        DT.Columns.Add(New DataColumn("Cod_RisUm", GetType(Integer)))
        DT.Columns.Add(New DataColumn("validita_inizio", GetType(String)))
        DT.Columns.Add(New DataColumn("validita_fine", GetType(String)))
        DT.Columns.Add(New DataColumn("checked", GetType(Integer)))
        'DT.Columns.Add(New DataColumn("Settore_Des", GetType(String)))
        'DT.Columns.Add(New DataColumn("attivita_des", GetType(String)))

        Dim Dt_Rapporti As New DataTable
        Dim i As Integer

        Dim ObjRapp As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
        Dt_Rapporti = ObjRapp.Contatti_RapportiContabili_Leggi(SACOD_CONTATTO_NONDEFINITO, _
                                                  0, _
                                                  False, False, False, False, False, False, False, _
                                                   AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                   "", _
                                                   "", _
                                                   objParametri_Server)

        For i = 0 To Dt_Rapporti.Rows.Count - 1
            Dr = DT.NewRow
            Dr.Item("Cod_Rapporto") = Dt_Rapporti.Rows(i).Item("Cod_Rapporto")
            Dr.Item("Rapporto_Des") = Dt_Rapporti.Rows(i).Item("Rapporto_Des")
            Dr.Item("Sa_Cod") = Dt_Rapporti.Rows(i).Item("Sa_Cod")
            Dr.Item("Cod_RisUm") = 0
            Dr.Item("validita_inizio") = AGRODATAINIZIO
            Dr.Item("validita_fine") = AGRODATAFINE
            Dr.Item("checked") = 0
            'Dr.Item("Settore_Des") = ""
            'Dr.Item("attivita_des") = ""
            DT.Rows.Add(Dr)
        Next

        ViewState("Dt_Rapporti") = DT

    End Sub

    Private Sub GeneraDTIndirizzi()

        Dim DT As New DataTable
        DT.Columns.Add("Cod_Indirizzo", Type.GetType("System.Int32"))
        DT.Columns.Add("Tipo_Indirizzo", Type.GetType("System.Int32"))
        DT.Columns.Add("Tipo_Indirizzo_Desc", Type.GetType("System.String"))
        DT.Columns.Add("Via", Type.GetType("System.String"))
        DT.Columns.Add("Frazione", Type.GetType("System.String"))
        DT.Columns.Add("Provincia_des", Type.GetType("System.String"))
        DT.Columns.Add("Provincia_cod", Type.GetType("System.String"))
        DT.Columns.Add("Comune_des", Type.GetType("System.String"))
        DT.Columns.Add("Comune_cod", Type.GetType("System.String"))
        DT.Columns.Add("Cap", Type.GetType("System.String"))
        DT.Columns.Add("Sigla_Prov", Type.GetType("System.String"))
        DT.Columns.Add("Stato", Type.GetType("System.String"))
        DT.Columns.Add("Note", Type.GetType("System.String"))

        'Vettore di DataColumn
        Dim DtKeys(1) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = DT.Columns("Cod_Indirizzo")
        DtKeys(1) = DT.Columns("Tipo_Indirizzo")

        'Assegno il vettore delle chiavi al DataTable
        DT.PrimaryKey = DtKeys

        ViewState("Dt_Indirizzi") = DT
        HttpContext.Current.Session("DT_Indirizzo") = DT

    End Sub


    Private Sub GeneraDTCosti()

        Dim DT As New DataTable

        '----- Definisco la struttura del DataTable
        DT.Columns.Add(New DataColumn("ID", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("strValidita_Inizio", GetType(String)))
        DT.Columns.Add(New DataColumn("strValidita_Fine", GetType(String)))
        DT.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
        DT.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))
        DT.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(String)))

        'Vettore di DataColumn
        Dim DtKeys(0) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = DT.Columns("ID")

        'Assegno il vettore delle chiavi al DataTable
        DT.PrimaryKey = DtKeys

        ViewState("Dt_Costi") = DT
        HttpContext.Current.Session("Dt_Costi") = DT

    End Sub


    Private Sub InserisciRigaCosto(ByVal ID As Integer, _
                                 ByVal Udm_Cod As Integer, _
                                 ByVal Udm_Des As String, _
                                 ByVal Validita_Inizio As Date, _
                                 ByVal Validita_Fine As Date, _
                                 ByVal Prezzo_Unitario As String)


        Dim Dt_Costi As DataTable
        Dim i As Integer
        Dim PrezzoPresente As Boolean = False

        If ViewState("Dt_Costi") IsNot Nothing Then

            Dt_Costi = ViewState("Dt_Costi")

            For i = 0 To Dt_Costi.Rows.Count - 1
                If Dt_Costi.Rows(i).Item("Udm_Cod") = Udm_Cod AndAlso
                    Dt_Costi.Rows(i).Item("Validita_Inizio") = Validita_Inizio AndAlso
                    Dt_Costi.Rows(i).Item("Validita_Fine") = Validita_Fine AndAlso
                    Dt_Costi.Rows(i).Item("Prezzo_Unitario") = Prezzo_Unitario Then
                    PrezzoPresente = True
                    Exit For
                End If
            Next

            If Not PrezzoPresente Then

                Dim dr As DataRow = Dt_Costi.NewRow

                'ID = ViewState("nPrezzi")
                'ViewState("nPrezzi") -= 1

                dr.Item("ID") = ID
                dr.Item("Udm_Cod") = Udm_Cod
                dr.Item("Udm_Des") = Udm_Des

                Dim format As String = "dd/MM/yyyy"
                Dim val_ini As String = ""
                Dim val_fin As String = ""

                val_ini = Validita_Inizio.ToString(format)
                val_fin = Validita_Fine.ToString(format)

                Validita_Inizio = Date.ParseExact(val_ini, "dd/MM/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo)
                Validita_Fine = Date.ParseExact(val_fin, "dd/MM/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo)

                dr.Item("Validita_Inizio") = Validita_Inizio
                dr.Item("Validita_Fine") = Validita_Fine


                'If Validita_Inizio = AGRODATAINIZIO Then
                '    dr.Item("strValidita_Inizio") = "..."
                'Else
                '    dr.Item("strValidita_Inizio") = CDate(Validita_Inizio).ToShortDateString
                'End If

                'If Validita_Fine = AGRODATAFINE Then
                '    dr.Item("strValidita_Fine") = "..."
                'Else
                '    dr.Item("strValidita_Fine") = CDate(Validita_Fine).ToShortDateString
                'End If

                dr.Item("Prezzo_Unitario") = Prezzo_Unitario

                Dt_Costi.Rows.Add(dr)

                ViewState("Dt_Costi") = Dt_Costi

            End If

        End If



    End Sub

    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function CaricaTipologiaIndirizzo(ByVal tipo_persona As Integer) As String


        Dim ddl_tipo_Indirizzo As New DropDownList

        Select Case tipo_persona
            Case PERSONA_FISICA
                ddl_tipo_Indirizzo.Items.Add(New ListItem("Residenza", 3))
                ddl_tipo_Indirizzo.Items.Add(New ListItem("Luogo di Nascita", 5))
                ddl_tipo_Indirizzo.Items.Add(New ListItem("Domicilio", 2))
                ddl_tipo_Indirizzo.Items.Add(New ListItem("Residenza Estiva", 4))

                Dim lista As New List(Of String)
                lista.Add(3)
                lista.Add(5)
                lista.Add(2)
                lista.Add(4)
                HttpContext.Current.Session("lista_indirizzi") = lista
                'Session("lista_indirizzi") = lista
            Case PERSONA_GIURIDICA
                ddl_tipo_Indirizzo.Items.Add(New ListItem("Sede Operativa", 1))
                ddl_tipo_Indirizzo.Items.Add(New ListItem("Sede Legale", 101))
                ddl_tipo_Indirizzo.Items.Add(New ListItem("Sede Aziendale", 102))
                ddl_tipo_Indirizzo.Items.Add(New ListItem("Stabilimento", 103))

                Dim lista As New List(Of String)
                lista.Add(1)
                lista.Add(101)
                lista.Add(102)
                lista.Add(103)
                HttpContext.Current.Session("lista_indirizzi") = lista
                'Session("lista_indirizzi") = lista
        End Select


        Dim rval As String = ""
        For Each itm As ListItem In ddl_tipo_Indirizzo.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        Return rval

    End Function



    '########################################################################################
    Private Sub ImgBtn_SalvaTutto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SalvaTutto.Click

        salvacontatto()

    End Sub

    Protected Sub salvacontatto()

        Dim Base, Top As Integer

        Calcola_BaseCode_TopCode(Base, Top, Session("ASG_ProgressivoGIAS"))

        Dim streerore As String = ""
        'persona fisica

        Dim Nome As String = Txt_Nome.Text
        Dim Cognome As String = Txt_Cognome.Text
        'persona giuridica
        'Dim Piva_contatto As String = txt_piva.Text
        Dim Rag_Soc As String = Txt_Rag_Soc.Text
        Dim Cod_Fisc_giuridica As String = Txt_Piva.Text


        Dim telefono As String = Txt_Telefono.Text
        Dim fax As String = Txt_Fax.Text
        Dim cellulare As String = Txt_Cellulare.Text
        Dim email As String = Txt_Mail.Text
        Dim dataNascita As Date
        Dim sesso As String
        Dim nrBadge As String = ""
        nrBadge = Txt_Badge.Text

        Dim objContattoW As New AgronicaCoreAnagrafeBIZ.Contatti_W
        Dim xmlDoc As XmlDocument
        Dim xContatto As XmlElement

        Dim Cod_Contatto = ""

        Dim id_cf As Integer
        Dim isGIAS As Boolean

        Dim Errore As Boolean = False

        Dim Visibilita As Integer = Cmb_CentriAziendali.SelectedValue
        'If Chk_ContattoPubblico.Checked = True Then
        '    Visibilita = -1
        'Else
        '    Visibilita = 0
        'End If

        dataNascita = AGRODATAINIZIO

        'persona fisica
        If RBL_TipoUtente.SelectedValue = "0" Then

            id_cf = PERSONA_FISICA
            Rag_Soc = ""


            sesso = ddl_Sesso.SelectedValue
            Cod_Contatto = Txt_CF.Text.Trim

            If IsDate(Txt_DataNascita.Text) Then
                dataNascita = CDate(Txt_DataNascita.Text)
            End If

            If Cod_Contatto.Length = 11 AndAlso Cod_Contatto.Chars(0) = "F" AndAlso IsNumeric(Cod_Contatto.Substring(1)) Then
                'contatto generato automaticamente
            Else
                If Cod_Contatto.Length <> 16 Then
                    'streerore = "alert('il codice fiscale non è corretto. Inserire un valore alfanumerico di 16 cifre o generarne una con il pulsante apposito.');"
                    'Dim strJS112 As New StringBuilder
                    'strJS112.AppendLine("$(document).ready(function () { ")
                    'strJS112.AppendLine(streerore)
                    'strJS112.AppendLine(" });")
                    'ScriptManager.RegisterStartupScript(Script_Panel, Script_Panel.GetType, _
                    '                                                            String.Format("jQuery_{0}", Script_Panel), _
                    '                                                            strJS112.ToString, True)
                    'Exit Sub
                End If
            End If

            If Cod_Contatto = "" OrElse Nome = "" OrElse Cognome = "" Then
                'streerore = "alert('Indicare un codice fiscale, un nome e un cognome.');"
                'Dim strJS112 As New StringBuilder
                'strJS112.AppendLine("$(document).ready(function () { ")
                'strJS112.AppendLine(streerore)
                'strJS112.AppendLine(" });")
                'ScriptManager.RegisterStartupScript(Script_Panel, Script_Panel.GetType, _
                '                                                            String.Format("jQuery_{0}", Script_Panel), _
                '                                                            strJS112.ToString, True)
                'Exit Sub
            End If

        Else

            id_cf = PERSONA_GIURIDICA
            'controllo se è un impresa gias
            Dim objA As New AgronicaCoreAnagrafeDAL.Imprese_Read
            isGIAS = objA.VerificaEsistenza_PivaGIAS(Txt_Piva.Text, objParametri_Server) 'no, non devo controllare

            Nome = ""
            Cognome = ""
            sesso = ""

            Cod_Contatto = Txt_Piva.Text.Trim
            If Cod_Contatto.Length = 11 AndAlso Cod_Contatto.Chars(0) = "F" AndAlso IsNumeric(Cod_Contatto.Substring(1)) Then
                'contatto generato automaticamente
            Else
                If Cod_Contatto.Length <> 11 OrElse Not IsNumeric(Cod_Contatto) Then
                    'streerore = "alert('la partita iva non è corretta. Inserire un valore numerico di 11 cifre o generarne una con il pulsante apposito.');"
                    'Dim strJS112 As New StringBuilder
                    'strJS112.AppendLine("$(document).ready(function () { ")
                    'strJS112.AppendLine(streerore)
                    'strJS112.AppendLine(" });")
                    'ScriptManager.RegisterStartupScript(Script_Panel, Script_Panel.GetType, _
                    '                                                            String.Format("jQuery_{0}", Script_Panel), _
                    '                                                            strJS112.ToString, True)
                    'Exit Sub
                End If
            End If

            If Cod_Contatto = "" OrElse Rag_Soc = "" Then
                'streerore = "alert('Indicare una partita iva e la ragione sociale.');"
                'Dim strJS112 As New StringBuilder
                'strJS112.AppendLine("$(document).ready(function () { ")
                'strJS112.AppendLine(streerore)
                'strJS112.AppendLine(" });")
                'ScriptManager.RegisterStartupScript(Script_Panel, Script_Panel.GetType, _
                '                                                            String.Format("jQuery_{0}", Script_Panel), _
                '                                                            strJS112.ToString, True)
                'Exit Sub
            End If

            If xPiva = Cod_Contatto Then
                'streerore = "alert('Indicare una partita iva del contatto diversa da quella dell'impresa.');"
                'Dim strJS112 As New StringBuilder
                'strJS112.AppendLine("$(document).ready(function () { ")
                'strJS112.AppendLine(streerore)
                'strJS112.AppendLine(" });")
                'ScriptManager.RegisterStartupScript(Script_Panel, Script_Panel.GetType, _
                '                                                            String.Format("jQuery_{0}", Script_Panel), _
                '                                                            strJS112.ToString, True)
                'Exit Sub
            End If


        End If


        Dim tipoOperazioneDB_Contatto, tipoOperazioneDB_RapCont, tipoOperazioneDB_INDIRIZZI As Integer
        Dim cod_risum As Integer
        Dim cod_rapp As Integer
        'tipoOperazioneDB_Contatto = Qs_Operazione
        tipoOperazioneDB_Contatto = objParametriAgenda.Tipo_Operazione

        Dim objContatto_R As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility

        Dim Dt_Indirizzi As DataTable
        Dt_Indirizzi = objXML_Utility.CaricaGriglia_Indirizzi_for_XML()

        'Dim lista As List(Of String) = Session("lista_indirizzi")
        Dim lista As List(Of String) = HttpContext.Current.Session("lista_indirizzi")

        Dim Dt_Rubrica As DataTable
        Dt_Rubrica = objXML_Utility.CaricaGriglia_Rubrica_for_XML()

        Dim DT_RisUm As DataTable
        DT_RisUm = objXML_Utility.CaricaGriglia_RisUm_for_XML()

        Dim Dt_Prodotti As DataTable
        Dt_Prodotti = objXML_Utility.CaricaGriglia_ProdottiCosti_for_XML()



        'controllo se è già inserito
        If tipoOperazioneDB_Contatto = enum_TipoOperazioneDB.Scrittura Then

            Dim objCont As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dtConta As DataTable = objCont.ControllaSePresente(xPiva, Cod_Contatto, objParametri_Server)

            If dtConta.Rows.Count > 0 Then
                ''errore
                'Dim strejs = "alert('contatto già presente');"
                'Dim strJS112 As New StringBuilder
                'strJS112.AppendLine("$(document).ready(function () { ")
                'strJS112.AppendLine(streerore)
                'strJS112.AppendLine(" });")
                'ScriptManager.RegisterStartupScript(Script_Panel, Script_Panel.GetType, _
                '                                                            String.Format("jQuery_{0}", Script_Panel), _
                '                                                            strJS112.ToString, True)
                'Exit Sub
            End If

            '--------------------------------------------
            ' RUBRICA
            '--------------------------------------------
            If telefono <> "" Then
                objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Scrittura, 0, telefono, "N. Telefono:")
            End If
            If cellulare <> "" Then
                objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Scrittura, 0, cellulare, "Cellulare:")
            End If
            If fax <> "" Then
                objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Scrittura, 0, fax, "Fax:")
            End If
            If email <> "" Then
                objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Scrittura, 0, email, "E-Mail:")
            End If

            '--------------------------------------------
            ' INDIRIZZI
            '--------------------------------------------

            Dim GridViewIndirizzi As DataTable

            ' GridViewIndirizzi = ViewState("Dt_Indirizzi")
            GridViewIndirizzi = HttpContext.Current.Session("Dt_Indirizzi")

            For i = 0 To GridViewIndirizzi.Rows.Count - 1

                Dim Tipo_Indirizzo As String = GridViewIndirizzi.Rows(i).Item("Tipo_Indirizzo")
                Dim Cod_Indirizzo As String = GridViewIndirizzi.Rows(i).Item("Cod_Indirizzo")

                Dim Via As String = GridViewIndirizzi.Rows(i).Item("Via")
                Dim Frazione As String = GridViewIndirizzi.Rows(i).Item("Frazione")
                Dim Istat_Prov As String = GridViewIndirizzi.Rows(i).Item("Provincia_cod")
                Dim Istat_Com As String = GridViewIndirizzi.Rows(i).Item("Comune_cod").ToString.Substring(3, 3)
                Dim Cap As String = GridViewIndirizzi.Rows(i).Item("Cap")
                Dim Stato As String = GridViewIndirizzi.Rows(i).Item("Stato")
                Dim Note As String = GridViewIndirizzi.Rows(i).Item("Note")

                objXML_Utility.Inserisci_Riga_Dt_Indirizzi_for_XML(Dt_Indirizzi, enum_TipoOperazioneDB.Scrittura, xPiva, _
                                                  Cod_Contatto, _
                                                  Tipo_Indirizzo, _
                                                   Istat_Prov, _
                                                   Istat_Com, _
                                                   Cod_Indirizzo, _
                                                   Via, _
                                                   Frazione, _
                                                   Cap, _
                                                   Stato, _
                                                   Note)

                For k = 0 To lista.Count - 1
                    If lista(k) = Tipo_Indirizzo Then
                        lista.RemoveAt(k)
                        Exit For
                    End If
                Next

            Next

            'aggiongo quelli rimasti e non associati
            For i = 0 To lista.Count - 1
                objXML_Utility.Inserisci_Riga_Dt_Indirizzi_for_XML(Dt_Indirizzi, enum_TipoOperazioneDB.Scrittura, xPiva, Cod_Contatto, lista(i), "000", "000")
            Next


        Else

            Dim Dati As String
            Dati = objContatto_R.Contatto_Leggi(xPiva, xCod_Contatto, "", False, objParametri_Server)

            Dim doc As XDocument
            doc = XDocument.Parse(Dati)


            '--------------------------------------------
            ' RUBRICA
            '--------------------------------------------
            Dim risultato_Rubrica = (From c In doc.<DatiContatti>.<Contatto>.<Rubrica> _
                           Select _cod_rubrica = c.@cod_rubrica, descr = c.@descr).ToList


            Dim trovato As Boolean
            Dim cod_rubrica As Integer


            ''''TELEFONO
            trovato = False
            For i = 0 To risultato_Rubrica.Count - 1
                If risultato_Rubrica(i).descr.Trim.ToLower = "n.telefono:" OrElse risultato_Rubrica(i).descr.Trim.ToLower = "n. telefono:" OrElse
                            risultato_Rubrica(i).descr.Trim.ToLower = "telefono" OrElse risultato_Rubrica(i).descr.Trim.ToLower = "tel." OrElse
                            risultato_Rubrica(i).descr.Trim.ToLower = "telefono:" OrElse risultato_Rubrica(i).descr.Trim.ToLower = "tel.:" OrElse
                            risultato_Rubrica(i).descr.Trim.ToLower = "n.telefono" OrElse risultato_Rubrica(i).descr.Trim.ToLower = "tel" Then
                    trovato = True
                    cod_rubrica = risultato_Rubrica(i)._cod_rubrica
                    Exit For
                End If
            Next
            If telefono <> "" Then
                If Not trovato Then
                    'nuovo
                    objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Scrittura, 0, _
                                                                   telefono, "N. Telefono:")
                Else
                    objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Modifica, cod_rubrica, _
                                                                   telefono, "N. Telefono:")
                End If
            Else
                If trovato Then
                    objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Cancellazione, cod_rubrica, _
                                                               telefono, "N. Telefono:")
                End If
            End If


            ''''CELLULARE
            trovato = False
            For i = 0 To risultato_Rubrica.Count - 1
                If risultato_Rubrica(i).descr.Trim.ToLower = "cellulare:" OrElse risultato_Rubrica(i).descr.Trim.ToLower = "cellulare" OrElse
                    risultato_Rubrica(i).descr.Trim.ToLower = "cell" OrElse risultato_Rubrica(i).descr.Trim.ToLower = "cell." OrElse risultato_Rubrica(i).descr.Trim.ToLower = "cellure" Then
                    trovato = True
                    cod_rubrica = risultato_Rubrica(i)._cod_rubrica
                    Exit For
                End If
            Next
            If cellulare <> "" Then
                If Not trovato Then
                    'nuovo
                    objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Scrittura, 0, _
                                                                   cellulare, "Cellulare:")
                Else
                    objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Modifica, cod_rubrica, _
                                                                   cellulare, "Cellulare:")
                End If
            Else
                If trovato Then
                    objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Cancellazione, cod_rubrica, _
                                                               cellulare, "Cellulare:")
                End If
            End If


            ''''FAX
            trovato = False
            For i = 0 To risultato_Rubrica.Count - 1
                If risultato_Rubrica(i).descr.Trim.ToLower = "fax:" OrElse risultato_Rubrica(i).descr.Trim.ToLower = "fax" Then
                    trovato = True
                    cod_rubrica = risultato_Rubrica(i)._cod_rubrica
                    Exit For
                End If
            Next
            If fax <> "" Then
                If Not trovato Then
                    'nuovo
                    objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Scrittura, 0, _
                                                                   fax, "Fax:")
                Else
                    objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Modifica, cod_rubrica, _
                                                                   fax, "Fax:")
                End If
            Else
                If trovato = True Then
                    objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Cancellazione, cod_rubrica, _
                                                               fax, "Fax:")
                End If
            End If




            ''''Email
            trovato = False
            For i = 0 To risultato_Rubrica.Count - 1
                If risultato_Rubrica(i).descr.Trim.ToLower = "e-mail:" OrElse risultato_Rubrica(i).descr.Trim.ToLower = "email" OrElse
                    risultato_Rubrica(i).descr.Trim.ToLower = "e-mail" OrElse risultato_Rubrica(i).descr.Trim.ToLower = "e.mail" OrElse
                    risultato_Rubrica(i).descr.Trim.ToLower = "email" OrElse risultato_Rubrica(i).descr.Trim.ToLower = "email." OrElse
                    risultato_Rubrica(i).descr.Trim.ToLower = "mail" Then
                    trovato = True
                    cod_rubrica = risultato_Rubrica(i)._cod_rubrica
                    Exit For
                End If
            Next
            If email <> "" Then
                If trovato = False Then
                    'nuovo
                    objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Scrittura, 0, _
                                                                   email, "E-Mail:")
                Else
                    objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Modifica, cod_rubrica, _
                                                                   email, "E-Mail:")
                End If
            Else
                If trovato = True Then
                    objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Cancellazione, cod_rubrica, _
                                                               email, "E-Mail:")
                End If
            End If


            '--------------------------------------------
            ' INDIRIZZI
            '--------------------------------------------
            Dim GridViewIndirizzi As DataTable

            'GridViewIndirizzi = ViewState("Dt_Indirizzi")
            GridViewIndirizzi = HttpContext.Current.Session("Dt_Indirizzi")

            For i = 0 To GridViewIndirizzi.Rows.Count - 1

                Dim Tipo_Indirizzo As String = GridViewIndirizzi.Rows(i).Item("Tipo_Indirizzo")
                Dim Cod_Indirizzo As String = GridViewIndirizzi.Rows(i).Item("Cod_Indirizzo")

                Dim Via As String = GridViewIndirizzi.Rows(i).Item("Via")
                Dim Frazione As String = GridViewIndirizzi.Rows(i).Item("Frazione")
                Dim Istat_Prov As String = GridViewIndirizzi.Rows(i).Item("Provincia_cod")
                Dim Istat_Com As String = GridViewIndirizzi.Rows(i).Item("Comune_cod")
                Dim Cap As String = GridViewIndirizzi.Rows(i).Item("Cap")
                Dim Stato As String = GridViewIndirizzi.Rows(i).Item("Stato")
                Dim Note As String = GridViewIndirizzi.Rows(i).Item("Note")

                Select Case Cod_Indirizzo
                    Case 0
                        tipoOperazioneDB_INDIRIZZI = enum_TipoOperazioneDB.Scrittura
                    Case Else
                        tipoOperazioneDB_INDIRIZZI = enum_TipoOperazioneDB.Modifica
                End Select
                objXML_Utility.Inserisci_Riga_Dt_Indirizzi_for_XML(Dt_Indirizzi, tipoOperazioneDB_INDIRIZZI, xPiva, _
                                                  Cod_Contatto, _
                                                  Tipo_Indirizzo, _
                                                   Istat_Prov, _
                                                   Istat_Com, _
                                                   Cod_Indirizzo, _
                                                   Via, _
                                                   Frazione, _
                                                   Cap, _
                                                   Stato, _
                                                   Note)

                For k = 0 To lista.Count - 1
                    If lista(k) = Tipo_Indirizzo Then
                        lista.RemoveAt(k)
                        Exit For
                    End If
                Next

            Next

            'aggiongo quelli rimasti e non associati
            For i = 0 To lista.Count - 1
                objXML_Utility.Inserisci_Riga_Dt_Indirizzi_for_XML(Dt_Indirizzi, enum_TipoOperazioneDB.Scrittura, xPiva, Cod_Contatto, lista(i), "000", "000")
            Next


        End If


        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '''''''''''''''''''rapporti contabili '''''''''''''''''''''''''''


        Dim DTRapporti As DataTable
        'If ViewState("Dt_Rapporti") IsNot Nothing Then
        '    DTRapporti = ViewState("Dt_Rapporti")
        'End If

        If HttpContext.Current.Session("Dt_Rapporti") IsNot Nothing Then
            DTRapporti = HttpContext.Current.Session("Dt_Rapporti")
        End If


        Dim almenouno As Boolean = False

        Dim j As Integer
        Dim IndiceDt As Integer

        Dim ID As Integer
        Dim corrispettivo As Decimal = 0
        Dim tipo_corrispettivo As String = ""
        Dim InizioPrezzo As Date
        Dim FinePrezzo As Date



        For j = 0 To DTRapporti.Rows.Count - 1

            cod_rapp = DTRapporti.Rows(j).Item("Cod_Rapporto")
            cod_risum = DTRapporti.Rows(j).Item("Cod_RisUm")

            If cod_risum = 0 Then
                tipoOperazioneDB_RapCont = enum_TipoOperazioneDB.Scrittura
            Else
                tipoOperazioneDB_RapCont = enum_TipoOperazioneDB.Modifica
            End If

            'If CType(GridViewRapporti.Rows(j).FindControl("ChkSelezionaRapporto"), CheckBox).Checked = True Then
            If DTRapporti.Rows(j).Item("checked") = 1 Then

                almenouno = True

                For i = 0 To DTRapporti.Rows.Count - 1
                    If cod_rapp = DTRapporti.Rows(i).Item("cod_rapporto") Then
                        IndiceDt = i
                        Exit For
                    End If
                Next

                Dim dal As String = AGRODATAINIZIO.ToString
                If DTRapporti.Rows(j).Item("validita_inizio") <> "" Then
                    dal = DTRapporti.Rows(j).Item("validita_inizio")
                End If
                Dim al As String = AGRODATAFINE.ToString
                If DTRapporti.Rows(j).Item("validita_fine") <> "" Then
                    al = DTRapporti.Rows(j).Item("validita_fine")
                End If

                Dim progressivo As String = Txt_Progressivo.Text ' DTRapporti.Rows(IndiceDt).Item("Settore_Des")
                Dim attivita As String = Txt_Attivita.Text 'DTRapporti.Rows(IndiceDt).Item("attivita_des")



                Dim numero_patentino As String = ""
                Dim Ente_di_rilascio As String = ""
                Dim data_rilascio As String = AGRODATAINIZIO.ToString
                Dim data_scadenza As String = AGRODATAFINE.ToString

                'If RBL_TipoUtente.SelectedValue = PERSONA_FISICA Then
                If HttpContext.Current.Session("RBL_TipoUtente") = PERSONA_FISICA Then
                    numero_patentino = Txt_Patentino.Text
                    If IsDate(Txt_Patentino_Rilascio.Text) = True Then
                        data_rilascio = Txt_Patentino_Rilascio.Text
                    End If
                    If IsDate(Txt_Patentino_Scadenza.Text) = True Then
                        data_scadenza = Txt_Patentino_Scadenza.Text
                    End If
                    Ente_di_rilascio = Txt_Patentino_Ente.Text
                End If

                Dim ArrayID() As Integer
                If ViewState("ArrayID") IsNot Nothing Then
                    ArrayID = ViewState("ArrayID")
                End If


                If HttpContext.Current.Session("Dt_Costi_Origine") IsNot Nothing Then
                    Dim DtCosti2 As DataTable
                    DtCosti2 = HttpContext.Current.Session("Dt_Costi_Origine")
                    ' DtCosti2.Columns.Add(New DataColumn("TipoOperazioneDB", GetType(Integer)))

                    For c = 0 To DtCosti2.Rows.Count - 1

                        ID = DtCosti2.Rows(c).Item("id")
                        corrispettivo = DtCosti2.Rows(c).Item("Prezzo_Unitario")
                        tipo_corrispettivo = DtCosti2.Rows(c).Item("Udm_Cod")
                        InizioPrezzo = DtCosti2.Rows(c).Item("validita_inizio")
                        FinePrezzo = DtCosti2.Rows(c).Item("validita_fine")
                        'If ID < 0 Then
                        objXML_Utility.Inserisci_Riga_Dt_ProdottiCosti_for_XML(Dt_Prodotti, enum_TipoOperazioneDB.Cancellazione, ID, xPiva, "", 0, 0, cod_risum, 0, tipo_corrispettivo, corrispettivo, 0, 0, InizioPrezzo, FinePrezzo)
                        'End If

                        'objXML_Utility.Inserisci_Riga_Dt_ProdottiCosti_for_XML(Dt_Prodotti, enum_TipoOperazioneDB.Cancellazione, 0, xPiva, "", 0, 0, cod_risum, 0, 0, 0, 0, 0)
                    Next

                    If Dt_Prodotti.Rows.Count > 0 Then
                        objXML_Utility.Inserisci_Riga_Dt_RisUm_for_XML(DT_RisUm, tipoOperazioneDB_RapCont, xPiva, Cod_Contatto, , cod_risum, cod_rapp, progressivo, attivita, , _
                                                                        , , , , numero_patentino, data_rilascio, data_scadenza, , , dal, al, Dt_Prodotti, Ente_di_rilascio)
                    End If

                End If


                Dim DtCosti As DataTable
                'If Not ViewState("Dt_Costi") Is Nothing Then
                If HttpContext.Current.Session("Dt_Costi") IsNot Nothing Then
                    DtCosti = HttpContext.Current.Session("Dt_Costi")
                    'inserisco solo i negativi (=nuovi)
                    'i positivi non li tocco
                    For c = 0 To DtCosti.Rows.Count - 1
                        ID = DtCosti.Rows(c).Item("id")
                        corrispettivo = DtCosti.Rows(c).Item("Prezzo_Unitario")
                        tipo_corrispettivo = DtCosti.Rows(c).Item("Udm_Cod")
                        InizioPrezzo = DtCosti.Rows(c).Item("validita_inizio")
                        FinePrezzo = DtCosti.Rows(c).Item("validita_fine")
                        'If ID < 0 Then
                        objXML_Utility.Inserisci_Riga_Dt_ProdottiCosti_for_XML(Dt_Prodotti, enum_TipoOperazioneDB.Scrittura, ID, xPiva, "", 0, 0, cod_risum, 0, tipo_corrispettivo, corrispettivo, 0, 0, InizioPrezzo, FinePrezzo)
                        'End If
                    Next

                    'If Dt_Prodotti.Rows.Count > 0 Then
                    '    objXML_Utility.Inserisci_Riga_Dt_RisUm_for_XML(DT_RisUm, tipoOperazioneDB_RapCont, xPiva, Cod_Contatto, , cod_risum, cod_rapp, progressivo, attivita, , _
                    '                                                    , , , , numero_patentino, data_rilascio, data_scadenza, , , dal, al, Dt_Prodotti, Ente_di_rilascio)
                    'End If


                End If


                objXML_Utility.Inserisci_Riga_Dt_RisUm_for_XML(DT_RisUm, tipoOperazioneDB_RapCont, xPiva, Cod_Contatto, , cod_risum, cod_rapp, progressivo, attivita, , _
                                                                , , , , numero_patentino, data_rilascio, data_scadenza, , , dal, al, Dt_Prodotti, Ente_di_rilascio)

            Else

                'rapporto non piu selezionato ma salvato in precedenza ( = da cancellare)
                'verifico se è stato movimentato
                If cod_risum <> 0 Then

                    Dim dtMov As DataTable
                    dtMov = objContatto_R.Contatto_ControllaMovimenti(xPiva, Cod_Contatto, "", objParametri_Server)

                    If dtMov.Rows.Count = 0 Then

                        objXML_Utility.Inserisci_Riga_Dt_ProdottiCosti_for_XML(Dt_Prodotti, enum_TipoOperazioneDB.Cancellazione, 0, xPiva, "", 0, 0, cod_risum, 0, 0, 0, 0, 0)
                        objXML_Utility.Inserisci_Riga_Dt_RisUm_for_XML(DT_RisUm, enum_TipoOperazioneDB.Cancellazione, xPiva, Cod_Contatto, , cod_risum, cod_rapp, , , , _
                                                          , , , , , , , , , , , Dt_Prodotti, )

                    End If


                End If

            End If

        Next



        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '''''''''''''''''''END rapporti contabili '''''''''''''''''''''''
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        If Not almenouno Then
            'streerore = "alert('Scegliere almeno un rapporto contabile.');"
            'Dim strJS112 As New StringBuilder
            'strJS112.AppendLine("$(document).ready(function () { ")
            'strJS112.AppendLine(streerore)
            'strJS112.AppendLine(" });")
            'ScriptManager.RegisterStartupScript(Script_Panel, Script_Panel.GetType, _
            '                                                            String.Format("jQuery_{0}", Script_Panel), _
            '                                                            strJS112.ToString, True)
            'Exit Sub
        End If

        Dim objxml As New AgronicaCoreXML.XML_Anagrafe

        xContatto = objxml.XML_2_Contatti(streerore,
                              xmlDoc,
                              Base,
                              Top,
                          False,
                          tipoOperazioneDB_Contatto,
                          xPiva,
                          Cod_Contatto,
                          Dt_Indirizzi,
                          DT_RisUm,
                            Visibilita,
                            id_cf, Rag_Soc, ,
                                             Cod_Fisc_giuridica, , Nome, Cognome, dataNascita,
                                             sesso, , , , Dt_Rubrica, Nothing, nrBadge:=nrBadge)

        Dim risp_boolean As Boolean
        Dim output_piva, output_sa_cod As String

        streerore = ""


        Try
            risp_boolean = objContattoW.Contatto_Scrivi(xContatto.OuterXml, output_piva, output_sa_cod, objParametri_Server)

        Catch ex As Exception

            Errore = True

            streerore = ex.Message.ToString

            AgroMsgBox("Si è verificato un errore durante la fase di salvataggio: " & vbCrLf & streerore, Page)

        End Try

        Session("ModificatoCF") = Nothing

        'If streerore.Length > 0 Then
        '    streerore = "alert('" & streerore & "');"
        'End If

        'If streerore.Length Then
        '    streerore = "alert('" & streerore & "');"
        'Else

        '    'Session("ContattoCreatoPiva") = Cod_Contatto

        '    'Dim strJS1 As New StringBuilder
        '    'strJS1.AppendLine("$(document).ready(function () { ")
        '    'strJS1.AppendLine("      gestisciValore('" & Cod_Contatto & "') ")
        '    'strJS1.AppendLine("      window.close() ")
        '    'strJS1.AppendLine(" });")
        '    'ScriptManager.RegisterStartupScript(Script_Panel, Script_Panel.GetType, _
        '    '                                                            String.Format("jQuery_{0}", Script_Panel), _
        '    '                                                            strJS1.ToString, True)
        '    'Exit Sub


        'End If


        ''Dim strJS11 As New StringBuilder
        ''strJS11.AppendLine("$(document).ready(function () { ")
        ''strJS11.AppendLine(streerore)
        ''strJS11.AppendLine(" });")
        ''ScriptManager.RegisterStartupScript(Script_Panel, Script_Panel.GetType, _
        ''                                                            String.Format("jQuery_{0}", Script_Panel), _
        ''                                                            strJS11.ToString, True)

        ViewState("Dt_Costi") = Nothing

        '============================
        '===  Fine Aggiornamento  ===
        '============================
        If Errore = False Then
            'Dim Piva As String

            'Ritorno alla pagina AlberoImprese
            'Response.Redirect(Qs_PaginaRitorno & "?P=" & Piva)

            Dim Messaggio As String
            Messaggio = "CONTATTO salvato con successo." & vbCrLf & vbCrLf

            Dim TargetUrl As String
            Select Case tipo_salva.Value

                Case 1
                    Messaggio += "Verrà ricaricata la pagina per un ulteriore inserimento" & vbCrLf
                    AgroMsgBox(Messaggio, Page)

                    Page_Load(Nothing, EventArgs.Empty)
                    clear_form()
                Case Else
                    'Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanel_script, , True)

                    TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda)
                    'Response.Redirect(TargetUrl)
                    AgroMsgBox(Messaggio, Page, , , "window.location = '" & TargetUrl & "';")

            End Select

        End If

    End Sub


    Private Sub clear_form()

        Cmb_CentriAziendali.ClearSelection()
        Txt_CF.Text = ""
        Txt_Cognome.Text = ""
        Txt_Nome.Text = ""
        Txt_DataNascita.Text = ""
        ddl_Sesso.ClearSelection()
        Txt_Telefono.Text = ""
        Txt_Cellulare.Text = ""
        Txt_Fax.Text = ""
        Txt_Mail.Text = ""
        Txt_Patentino.Text = ""
        Txt_Patentino_Rilascio.Text = ""
        Txt_Patentino_Scadenza.Text = ""
        Txt_Patentino_Ente.Text = ""
        Txt_Progressivo.Text = ""
        Txt_Attivita.Text = ""

        ddl_tipo_Indirizzo.ClearSelection()
        txt_via.Text = ""
        ddl_provincia.ClearSelection()
        Txt_ComCodIstat.Text = ""
        txt_cap.Text = ""
        'txt_stato.Enabled = False
        cmb_Stato.Enabled = False
        Txt_Note_Indirizzo.Text = ""
        txt_frazione.Text = ""

        ddl_UdmCod_Prezzo.ClearSelection()
        Txt_Prezzo.Text = ""
        Txt_Inizio_Prezzo.Text = ""
        Txt_Fine_Prezzo.Text = ""

    End Sub


    '##########################################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)


        ' Setto nuovamente la PIVA del centro aziendale x filtro su menu
        If objParametriAgenda.Piva_Origine <> "" AndAlso objParametriAgenda.Piva_Origine <> objParametriAgenda.Piva Then
            objParametriAgenda.Piva = objParametriAgenda.Piva_Origine
        End If

        Dim TargetUrl As String
        TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda, , Qs_Visibilita)

        Response.Redirect(TargetUrl)

    End Sub


    Private Sub Contatto_Edit_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        ' VAnni: 2/10/2017: a seguito di AgroMaseterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        CType(Me.Master, AgendaBootstrap).flag_MostraBtnIndietro = True

    End Sub

    Private Sub RBL_TipoUtente_SelectedIndexChanged(sender As Object, e As EventArgs) Handles RBL_TipoUtente.SelectedIndexChanged

    End Sub

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CambiaCF(ByVal CF_Old As String, ByVal CF_Nuovo As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim objTabelle_R As New AgronicaCoreVarieDAL.System_Database_R
        Dim objTabelle_W As New AgronicaCoreVarieDAL.System_Database_W
        Dim ModificatoCampo As Boolean
        Dim RecordModificati As Integer = 0
        Dim DT_Tabelle As DataTable
        Dim i, j As Integer

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            DT_Tabelle = objTabelle_R.Leggi_ElencoNomiTabelle_2(objParametri_Server)

            If DT_Tabelle IsNot Nothing Then
                For i = 0 To DT_Tabelle.Rows.Count - 1
                    Dim Dt_Campi As DataTable
                    Dt_Campi = objTabelle_R.Leggi_ElencoNomiColonneTabella_3(DT_Tabelle.Rows(i).Item("TABLE_NAME"), objParametri_Server)
                    If Dt_Campi IsNot Nothing Then
                        For j = 0 To Dt_Campi.Rows.Count - 1
                            If LCase(Dt_Campi.Rows(j).Item("NAME")) = "cod_contatto" Then
                                ModificatoCampo = objTabelle_W.Modifica_Valore_Campo_Tabella(DT_Tabelle.Rows(i).Item("TABLE_NAME"), Dt_Campi.Rows(j).Item("NAME"), CF_Old, CF_Nuovo, "", objParametri_Server)
                                If ModificatoCampo = True Then
                                    RecordModificati += 1
                                End If
                                Select Case LCase(DT_Tabelle.Rows(i).Item("TABLE_NAME"))
                                    Case "liquidita"
                                        ModificatoCampo = objTabelle_W.Modifica_Valore_Campo_Tabella(DT_Tabelle.Rows(i).Item("TABLE_NAME"), "Riferimento", CF_Old, CF_Nuovo, "", objParametri_Server)
                                        If ModificatoCampo Then
                                            RecordModificati += 1
                                        End If
                                End Select

                            End If
                        Next
                    End If
                Next
            End If

            r.RispostaOK = True

            If RecordModificati > 0 Then
                r.RispostaConferma = True
            Else
                r.RispostaConferma = False
            End If

            r.RispostaStringa = "Operazione eseguita correttamente"

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception


            'Faccio il rollback della transazione
            If objParametri_Server.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If


            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return r

    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CambiaPiva(ByVal Piva_Old As String, ByVal Piva_Nuovo As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim objTabelle_R As New AgronicaCoreVarieDAL.System_Database_R
        Dim objTabelle_W As New AgronicaCoreVarieDAL.System_Database_W
        Dim ModificatoCampo As Boolean
        Dim RecordModificati As Integer = 0
        Dim DT_Tabelle As DataTable
        Dim i, j As Integer

        '------------------------------
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            DT_Tabelle = objTabelle_R.Leggi_ElencoNomiTabelle_2(objParametri_Server)

            If DT_Tabelle IsNot Nothing Then
                For i = 0 To DT_Tabelle.Rows.Count - 1
                    Dim Dt_Campi As DataTable
                    Dt_Campi = objTabelle_R.Leggi_ElencoNomiColonneTabella_3(DT_Tabelle.Rows(i).Item("TABLE_NAME"), objParametri_Server)
                    If Dt_Campi IsNot Nothing Then
                        For j = 0 To Dt_Campi.Rows.Count - 1
                            If LCase(Dt_Campi.Rows(j).Item("NAME")) = "cod_contatto" Then
                                ModificatoCampo = objTabelle_W.Modifica_Valore_Campo_Tabella(DT_Tabelle.Rows(i).Item("TABLE_NAME"), Dt_Campi.Rows(j).Item("NAME"), Piva_Old, Piva_Nuovo, "", objParametri_Server)
                                If ModificatoCampo Then
                                    RecordModificati += 1
                                End If
                                Select Case LCase(DT_Tabelle.Rows(i).Item("TABLE_NAME"))
                                    Case "liquidita"
                                        ModificatoCampo = objTabelle_W.Modifica_Valore_Campo_Tabella(DT_Tabelle.Rows(i).Item("TABLE_NAME"), "Riferimento", Piva_Old, Piva_Nuovo, "", objParametri_Server)
                                        If ModificatoCampo Then
                                            RecordModificati += 1
                                        End If
                                End Select
                            End If
                        Next
                    End If
                Next
            End If

            r.RispostaOK = True

            If RecordModificati > 0 Then
                r.RispostaConferma = True
            Else
                r.RispostaConferma = False
            End If

            r.RispostaStringa = "Operazione eseguita correttamente"

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

        Catch ex As Exception


            'Faccio il rollback della transazione
            If objParametri_Server.objTransazione IsNot Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return r

    End Function

End Class