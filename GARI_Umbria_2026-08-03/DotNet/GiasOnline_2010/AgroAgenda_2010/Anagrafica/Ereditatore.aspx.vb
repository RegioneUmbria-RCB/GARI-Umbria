Imports AgronicaCoreVarieBIZ
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports System.Collections.Generic
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModello.ParametriAgenda_Temp
Public Class Ereditatore
    Inherits System.Web.UI.Page

    Dim objParametri_Utenti As AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreParametri

    Public permessi As PermessiUtente
    Dim objParametri_Agenda As ParametriAgenda_2010
    Dim objAgenda As ParametriAgenda

    Private Sub Ereditatore_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        ' VAnni: 2/10/2017: a seguito di AgroMaseterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        Master.Lbl_Titolo.Text = "Ereditatore"
        Master.flag_pag_Anagrafica = True
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Load
        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        permessi = New PermessiUtente()
        objParametri_Agenda = New ParametriAgenda_2010
        objParametri_Agenda.Leggi()
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        If Not Page.IsPostBack Then
            objAgenda = New ParametriAgenda
            Dim listImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto) = objAgenda.Impianti
            Dim filtroImpianti As String
            Dim Query1_TempTableCreazione As String = ""
            Dim Query2_TempTableIndice As String = ""
            Dim Query3_TempTableFill As String = ""
            'Dim Query4_TempTableJoin As String = ""
            If Not IsNothing(listImpianti) Then
                For Each impianto In listImpianti
                    Query3_TempTableFill += " INSERT INTO #tempimpianti (Piva, Sa_Cod, Appezza, Id_Reg)  " & vbCrLf
                    Query3_TempTableFill += " VALUES     ('" + Agro_SQL_SaveText(impianto.Piva) + "'," + Agro_SQL_SaveNum(impianto.Sa_Cod) + "," + Agro_SQL_SaveNum(impianto.Appezza) + "," + Agro_SQL_SaveNum(impianto.ID_Reg) + ")  " & vbCrLf

                Next

                Dim TabelleTemp_Mode As Integer
                Dim Str_TabelleTemp_RegolaConfronto As String

                TabelleTemp_Mode = Recupera_TabelleTemp_Mode()

                '1: CREAZIONE TABELLA TEMPORANEA TRAMITE SELECT INSERT INTO 
                '2: TRAMITE CREATE TABLE
                Select Case TabelleTemp_Mode

                    Case 1
                        Query1_TempTableCreazione += " SELECT Piva, Sa_Cod, Appezza, Id_Reg "
                        Query1_TempTableCreazione += "   INTO #tempimpianti   "
                        Query1_TempTableCreazione += "       FROM Reg_Impianti "
                        Query1_TempTableCreazione += "           WHERE 1 = 0   "
                        Query1_TempTableCreazione += vbCrLf

                    Case 2

                        'Recupera regola di Confronto: collate sql_LATIN1_GENERAL_cp850_ci_as NOT NULL
                        Str_TabelleTemp_RegolaConfronto = Recupera_Str_TabelleTemp_RegolaConfronto()

                        Query1_TempTableCreazione += "   CREATE TABLE #tempimpianti (	"
                        Query1_TempTableCreazione += " [Piva]       [varchar] (11) " + Str_TabelleTemp_RegolaConfronto + " NOT NULL ,"
                        Query1_TempTableCreazione += " [Sa_Cod]     [int] 		    NOT NULL ,"
                        Query1_TempTableCreazione += " [Appezza]    [int] 	        NOT NULL ,"
                        Query1_TempTableCreazione += " [Id_Reg]     [int] 		    NOT NULL ,"
                        Query1_TempTableCreazione += " ) ON [PRIMARY]"
                        Query1_TempTableCreazione += vbCrLf
                End Select

                Query2_TempTableIndice = " CREATE UNIQUE INDEX [#AgroIndextempimpianti] ON [dbo].[#tempimpianti]([Piva], [Sa_Cod], [Appezza], [Id_Reg]) "

                Session("ereditatore_Query1_TempTableCreazione") = Query1_TempTableCreazione
                Session("ereditatore_Query2_TempTableIndice") = Query2_TempTableIndice
                Session("ereditatore_Query3_TempTableFill") = Query3_TempTableFill
                Session("ereditatore_filtroImpianti") = listImpianti
            End If



        End If


    End Sub

    Private Sub AnnullaTutto(sender As Object, e As ImageClickEventArgs)
        Throw New NotImplementedException
    End Sub


    <WebMethod(EnableSession:=True)> _
    Public Shared Function LeggiElencoProprieta() As RispostaStandard
        Dim r As New rispostaStandard

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            r.RispostaOK = True

            Dim Cmb_Proprieta As ListControl = New ListBox

            'CaricaCombo_Proprieta()
            AgronicaCoreUtility.CaricaListControl.MultiModificaImpianti_Proprieta( _
                                Cmb_Proprieta, _
                                False, "", "-1", _
                                1, _
                                False, _
                                "", _
                                objParametri_Utenti)

            'Dim dt As DataTable = AgronicaCoreUtility.CaricaListControl.itemsCollection_toDataTable(Cmb_Proprieta.Items, "text", "value")
            ''dt = Cmb_Proprieta.DataSource

            'r.RispostaStringa = CaricaLista_Proprieta_xJSON(dt)

            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(Cmb_Proprieta.Items, "value", "text")

        Catch ex As Exception

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)> _
    Public Shared Function LeggiImpiantiConProprieta(ByVal valori As Integer(), ByVal filtroDistinte As Integer, ByVal dataDistinta As String) As RispostaStandard
        Dim r As New rispostaStandard

        'TODO
        Dim dtCodAnagr As DataTable = New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        'Dim objParamentriConcimazione As AgronicaCoreGestioneRichieste.ParametriConcimazione_2017 = New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
        'objParamentriConcimazione.Leggi()
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            'Inserire il codice QUI..

            r.RispostaOK = True



            Dim Query1_TempTableCreazione = CType(HttpContext.Current.Session("ereditatore_Query1_TempTableCreazione"), String)
            Dim Query2_TempTableIndice = CType(HttpContext.Current.Session("ereditatore_Query2_TempTableIndice"), String)
            Dim Query3_TempTableFill = CType(HttpContext.Current.Session("ereditatore_Query3_TempTableFill"), String)

            Dim dt As DataTable

            dt = RegImpianti_ImpreseProgetti_Leggi(Query1_TempTableCreazione, Query2_TempTableIndice, Query3_TempTableFill, valori, _
                                                   dtCodAnagr, filtroDistinte, dataDistinta, objParametri_Server)

            r.RispostaStringa = CaricaGriglia_Impianti_xJSON(dt, valori)
            r.ParametroDue_stringa = CStr(dt.Rows.Count)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & _
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function

    Public Shared Function CaricaGriglia_Impianti_xJSON(ByVal dt As DataTable, valori As Integer()) As String

        'TODO

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome


        c = New ColonneNome("Piva", "P. IVA", "string")
        'c._OperatoreFiltro_Kendo = "contains"
        c._Display = False
        l.Add(c)

        c = New ColonneNome("Sa_Cod", "Sa_Cod", "number")
        c._Display = False
        l.Add(c)

        c = New ColonneNome("Appezza", "Appezza", "number")
        c._Display = False
        l.Add(c)

        c = New ColonneNome("Id_Reg", "Id_Reg", "number")
        c._Display = False
        l.Add(c)

        c = New ColonneNome("Veg_Cod", "Veg_Cod", "number")
        c._Display = False
        l.Add(c)

        c = New ColonneNome("Cul_Cod", "Cul_Cod", "number")
        c._Display = False
        l.Add(c)

        c = New ColonneNome("Grfi_Cod", "Grfi_Cod", "number")
        c._Display = False
        l.Add(c)

        c = New ColonneNome("Stato_Impianto", "Stato_Impianto", "number")
        c._Display = False
        l.Add(c)

        c = New ColonneNome("Rag_Soc", "Azienda", "string")
        'c._OperatoreFiltro_Kendo = "contains"
        l.Add(c)

        c = New ColonneNome("Sa_Nome", "Centro", "string")
        l.Add(c)

        c = New ColonneNome("Veg_Des", "Specie", "string")
        l.Add(c)

        c = New ColonneNome("App_Nome", "Appezzamento", "string")
        l.Add(c)

        c = New ColonneNome("Progetto_Nome", "Impianto/Distinta", "string")
        l.Add(c)

        For Each codice In valori

            Try
                Dim enCodice = CType(codice, enum_MultiModificaImpianti_Proprieta)

                'c = New ColonneNome([Enum].GetName(GetType(enum_MultiModificaImpianti_Proprieta), enCodice), TipiEnumerativi.MultiModificaImpiantiProprieta_Des_from_Cod(enCodice), "string")
                'c._Editabile = True
                ''c._FormatoParticolare = "templateFunction"
                'l.Add(c)
                Select Case enCodice
                    'Case enum_MultiModificaImpianti_Proprieta.Capitolato_Privato


                    '    'Case enum_MultiModificaImpianti_Proprieta.Org_Referente

                    '    'c = New ColonneNome("Organismo_referente", TipiEnumerativi.MultiModificaImpiantiProprieta_Des_from_Cod(enCodice), "string")
                    '    'l.Add(c)


                    'Case enum_MultiModificaImpianti_Proprieta.Magazzino_Conferimento

                    '    c = New ColonneNome("Mag_Conferimento", TipiEnumerativi.MultiModificaImpiantiProprieta_Des_from_Cod(enCodice), "string")
                    '    l.Add(c)



                    'Case enum_MultiModificaImpianti_Proprieta.Certificazione

                    '    c = New ColonneNome("Certificazione", TipiEnumerativi.MultiModificaImpiantiProprieta_Des_from_Cod(enCodice), "string")
                    '    l.Add(c)

                    Case enum_MultiModificaImpianti_Proprieta.Regolamento
                        c = New ColonneNome([Enum].GetName(GetType(enum_MultiModificaImpianti_Proprieta), enCodice), TipiEnumerativi.MultiModificaImpiantiProprieta_Des_from_Cod(enCodice), "string")
                        c._Editabile = True
                        c._FormatoParticolare = "templateRegolamento"
                        l.Add(c)
                    Case Else
                        c = New ColonneNome([Enum].GetName(GetType(enum_MultiModificaImpianti_Proprieta), enCodice), TipiEnumerativi.MultiModificaImpiantiProprieta_Des_from_Cod(enCodice), "string")
                        c._Editabile = True
                        'c._FormatoParticolare = "templateFunction"
                        l.Add(c)


                End Select
            Catch ex As Exception

            End Try

        Next

        'Stato_Cod = .Item("Stato_Impianto")


        'If CDate(.Item("Validita_Fine_Impianto")) = AGRODATAINIZIO Then
        '    Fine_Impianto = "..."
        'Else
        '    Fine_Impianto = CStr(.Item("Validita_Fine_Impianto"))
        'End If
        'If CDate(.Item("Validita_Fine_Distinta")) = AGRODATAFINE Then
        '    Fine_Distinta = "..."
        'Else
        '    Fine_Distinta = CStr(.Item("Validita_Fine_Distinta"))
        'End If

        'Piva = .Item("Piva")
        'Rag_Soc = .Item("Rag_Soc")
        'Sa_Cod = .Item("Sa_Cod")
        'Sa_Nome = .Item("Sa_Nome")
        'Appezza = .Item("Appezza")
        'App_Nome = .Item("App_Nome")
        'Id_Reg = .Item("Id_Reg")
        'Veg_Cod = .Item("Veg_Cod")
        'Cul_Cod = .Item("Cul_Cod")
        'Grfi_Cod = .Item("Grfi_Cod")
        'Stato_Cod = .Item("Stato_Impianto")
        'Progetto_Cod = .Item("Progetto_Cod")
        'Distinta = .Item("Progetto_Nome") + ", Validità: " + CStr(.Item("Validita_Inizio_Distinta")) + " - " + Fine_Distinta
        'Organismo_Referente = .Item("Organismo_Referente")
        'Mag_Conferimento = .Item("Mag_Conferimento")
        'Capitolato_Privato = .Item("Capitolato_Privato")
        'Sup_Imp = .Item("Sup_Imp")
        'Disciplinare_Cod = .Item("Disciplinare_Cod")
        'Disciplinare_PubblicoPrivato = .Item("Disciplinare_PubblicoPrivato")
        'Regolamento_Concimazioni_Cod = .Item("Regolamento_Concimazioni_Cod")
        'Regolamento_Cod = .Item("Regolamento_Cod")
        'Data_Semina = CDate(.Item("Data_Inizio_Prevista")).ToShortDateString
        'Data_Fioritura = CDate(.Item("Data_Fioritura_Prevista")).ToShortDateString
        'Data_Raccolta = CDate(.Item("Data_Fine_Prevista")).ToShortDateString
        'Certificazione = .Item("Certificazione")

        'Resa = .Item("Produzione_Prevista")

        'Validita_Inizio_Distinta = CStr(.Item("Validita_Inizio_Distinta"))
        'Validita_Fine_Distinta = CStr(.Item("Validita_Fine_Distinta"))
        'Validita_Inizio_Impianto = CStr(.Item("Validita_Inizio_Impianto"))
        'Validita_Fine_Impianto = CStr(.Item("Validita_Fine_Impianto"))

        'N = CStr(.Item("N"))
        'P = CStr(.Item("P"))
        'K = CStr(.Item("K"))
        'M = CStr(.Item("M"))

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = False
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '

        Return risp


    End Function

    Private Shared Function CaricaLista_Proprieta_xJSON(dt As DataTable) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("text", "text", "string")
        l.Add(c)

        c = New ColonneNome("value", "value", "string")
        l.Add(c)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = False
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=False) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '

        Return risp

    End Function


    '###############################################################################################
    Private Shared Function RegImpianti_ImpreseProgetti_Leggi(ByVal Query1_TempTableCreazione As String, _
                                                        ByVal Query2_TempTableIndice As String, _
                                                        ByVal Query3_TempTableFill As String, valori As Integer(), _
                                                        ByVal dtCodAnagr As DataTable, _
                                                        ByVal filtroDistinte As Integer, dataDistinta As String, _
                                                        ByRef objServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim DescrizioneFunzione As String = "RegImpianti_ImpreseProgetti_Leggi"

        Dim Messaggio As String = ""
        Dim DT As DataTable
        Dim stbQuery As New System.Text.StringBuilder

        Dim Query4_TempTableJoin As String = ""
        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------

        stbQuery.Length = 0
        stbQuery.Append(" SELECT  Imprese.Rag_Soc, Centri_Aziendali.Sa_Nome, Appezzamento.App_Nome, " & vbCrLf)
        stbQuery.Append(" Reg_Impianti.*, Reg_Impianti.Validita_Inizio AS Validita_Inizio_Impianto, Reg_Impianti.Validita_Fine AS Validita_Fine_Impianto, " & vbCrLf)
        stbQuery.Append(" Imprese_Progetti.Piva, Imprese_Progetti.Progetto_Cod, Imprese_Progetti.Progetto_Nome," & vbCrLf)
        stbQuery.Append(" Imprese_Progetti.Progetto_Des, Imprese_Progetti.Ricavi_Previsti, Imprese_Progetti.Produzione_Prevista," & vbCrLf)
        stbQuery.Append(" Imprese_Progetti.Cau_Progetto, Imprese_Progetti.Sa_Cod, Imprese_Progetti.Appezza," & vbCrLf)
        stbQuery.Append(" Imprese_Progetti.Id_Reg, Imprese_Progetti.Data_Inizio_Prevista, Imprese_Progetti.Data_Fioritura_Prevista, Imprese_Progetti.Data_Fine_Prevista," & vbCrLf)
        stbQuery.Append(" Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Distinta, Imprese_Progetti.Validita_Fine AS Validita_Fine_Distinta, Imprese_Progetti.Stato_Impianto," & vbCrLf)
        stbQuery.Append(" Imprese_Progetti.Regolamento_Cod, Imprese_Progetti.Disciplinare_Cod, Imprese_Progetti.Disciplinare_PubblicoPrivato, Imprese_Progetti.Regolamento_Concimazioni_Cod, Imprese_Progetti.P_HA," & vbCrLf)
        stbQuery.Append(" Cultivar.Cul_Des, SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des " & vbCrLf)

        'NEL CASO SIA SELEZIONATO IL DPI, NON è NECESSARIO ALCUN IF, PERCHè IL DISCIPLINARE_COD è IN IMPRESE_PROGETTI
        'NEL CASO SIA SELEZIONATO  Data_Semina, Data_Raccolta o Resa, NON è NECESSARIO ALCUN IF, PERCHè SONO DATI IN IMPRESE_PROGETTI
        For Each codice In valori

            Try
                Dim enCodice = CType(codice, enum_MultiModificaImpianti_Proprieta)

                Select Case enCodice
                    Case enum_MultiModificaImpianti_Proprieta.Capitolato_Privato

                        stbQuery.Append(", ISNULL(( SELECT  TOP 1 Reg_Impianti_Codici.val_cod " & vbCrLf)
                        stbQuery.Append("           FROM    Reg_Impianti_Codici  " & vbCrLf)
                        stbQuery.Append("           WHERE   Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza   " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod " & vbCrLf)
                        stbQuery.Append("           AND     (Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Capitolato_Privato & ") " & vbCrLf)
                        stbQuery.Append(" ) , '') AS " & enum_MultiModificaImpianti_Proprieta.Capitolato_Privato.ToString & vbCrLf)

                    Case enum_MultiModificaImpianti_Proprieta.Org_Referente


                        stbQuery.Append(", ISNULL(( SELECT  TOP 1 Reg_Impianti_Codici.val_cod " & vbCrLf)
                        stbQuery.Append("           FROM    Reg_Impianti_Codici  " & vbCrLf)
                        stbQuery.Append("           WHERE   Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza   " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod " & vbCrLf)
                        stbQuery.Append("           AND     Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Organismo_Referente & "" & vbCrLf)
                        stbQuery.Append("       ), '') AS " & enum_MultiModificaImpianti_Proprieta.Org_Referente.ToString & vbCrLf)

                    Case enum_MultiModificaImpianti_Proprieta.Magazzino_Conferimento

                        stbQuery.Append(", ISNULL(( SELECT  TOP 1 Reg_Impianti_Codici.val_cod " & vbCrLf)
                        stbQuery.Append("           FROM    Reg_Impianti_Codici  " & vbCrLf)
                        stbQuery.Append("           WHERE   Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza   " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod " & vbCrLf)
                        stbQuery.Append("           AND     Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Magazzino_Conferimento & vbCrLf)
                        stbQuery.Append("       ), '') AS " & enum_MultiModificaImpianti_Proprieta.Magazzino_Conferimento.ToString & vbCrLf)
                        'If ViewState("Magazzino_Conferimento") = True Then
                        'Else
                        '    stbQuery.Append(" , '' AS Mag_Conferimento " & vbCrLf)
                        'End If

                    Case enum_MultiModificaImpianti_Proprieta.Pro_N

                        stbQuery.Append(", ISNULL(( SELECT  TOP 1 Reg_Impianti_Codici.val_cod " & vbCrLf)
                        stbQuery.Append("           FROM    Reg_Impianti_Codici  " & vbCrLf)
                        stbQuery.Append("           WHERE   Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza   " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod " & vbCrLf)
                        stbQuery.Append("           AND     Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Impianto_LimiteN & vbCrLf)
                        stbQuery.Append("       ), '') AS " & enum_MultiModificaImpianti_Proprieta.Pro_N.ToString & vbCrLf)

                        'If ViewState("N") = True Then
                        'Else
                        '    stbQuery.Append(" , '' AS N " & vbCrLf)
                        'End If

                    Case enum_MultiModificaImpianti_Proprieta.Pro_P2O5

                        stbQuery.Append(", ISNULL(( SELECT  TOP 1 Reg_Impianti_Codici.val_cod " & vbCrLf)
                        stbQuery.Append("           FROM    Reg_Impianti_Codici  " & vbCrLf)
                        stbQuery.Append("           WHERE   Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza   " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod " & vbCrLf)
                        stbQuery.Append("           AND     Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Impianto_LimiteP & vbCrLf)
                        stbQuery.Append("       ), '') AS " & enum_MultiModificaImpianti_Proprieta.Pro_P2O5.ToString & vbCrLf)

                        'If ViewState("P") = True Then
                        'Else
                        '    stbQuery.Append(" , '' AS P " & vbCrLf)
                        'End If

                    Case enum_MultiModificaImpianti_Proprieta.Sup_Impianto

                        stbQuery.Append(", ISNULL(( Sup_Imp), '') AS " & enum_MultiModificaImpianti_Proprieta.Sup_Impianto.ToString & vbCrLf)

                        'If ViewState("P") = True Then
                        'Else
                        '    stbQuery.Append(" , '' AS P " & vbCrLf)
                        'End If

                    Case enum_MultiModificaImpianti_Proprieta.Pro_K2O

                        stbQuery.Append(", ISNULL(( SELECT  TOP 1 Reg_Impianti_Codici.val_cod " & vbCrLf)
                        stbQuery.Append("           FROM    Reg_Impianti_Codici  " & vbCrLf)
                        stbQuery.Append("           WHERE   Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza   " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod " & vbCrLf)
                        stbQuery.Append("           AND     Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Impianto_LimiteK & vbCrLf)
                        stbQuery.Append("       ), '') AS " & enum_MultiModificaImpianti_Proprieta.Pro_K2O.ToString & vbCrLf)
                        'If ViewState("K") = True Then
                        'Else
                        '    stbQuery.Append(" , '' AS K " & vbCrLf)
                        'End If

                    Case enum_MultiModificaImpianti_Proprieta.Pro_MgO

                        stbQuery.Append(", ISNULL(( SELECT  TOP 1 Reg_Impianti_Codici.val_cod " & vbCrLf)
                        stbQuery.Append("           FROM    Reg_Impianti_Codici  " & vbCrLf)
                        stbQuery.Append("           WHERE   Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza   " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod " & vbCrLf)
                        stbQuery.Append("           AND     Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Impianto_LimiteMg & vbCrLf)
                        stbQuery.Append("       ), '') AS " & enum_MultiModificaImpianti_Proprieta.Pro_MgO.ToString & vbCrLf)

                        'If ViewState("M") = True Then
                        'Else
                        '    stbQuery.Append(" , '' AS M " & vbCrLf)
                        'End If

                    Case enum_MultiModificaImpianti_Proprieta.Certificazione

                        stbQuery.Append(", ISNULL(( SELECT  TOP 1 Reg_Impianti_Codici.val_cod " & vbCrLf)
                        stbQuery.Append("           FROM    Reg_Impianti_Codici  " & vbCrLf)
                        stbQuery.Append("           WHERE   Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza   " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod " & vbCrLf)
                        stbQuery.Append("           AND     Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Certificazione & vbCrLf)
                        stbQuery.Append("       ), '') AS " & enum_MultiModificaImpianti_Proprieta.Certificazione.ToString & vbCrLf)

                        'If ViewState("Certificazione") = True Then
                        'Else
                        '    stbQuery.Append(" , '' AS Certificazione " & vbCrLf)
                        'End If
                    Case Else

                        stbQuery.Append(", ISNULL(( SELECT  TOP 1 Reg_Impianti_Codici.val_cod " & vbCrLf)
                        stbQuery.Append("           FROM    Reg_Impianti_Codici  " & vbCrLf)
                        stbQuery.Append("           WHERE   Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza   " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg  " & vbCrLf)
                        stbQuery.Append("           AND     Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod " & vbCrLf)
                        stbQuery.Append("           AND     Reg_Impianti_Codici.id_cod = " & enCodice & vbCrLf)
                        stbQuery.Append("       ), '') AS " & [Enum].GetName(GetType(enum_MultiModificaImpianti_Proprieta), enCodice) & vbCrLf)

                End Select
            Catch ex As Exception

            End Try

        Next


        stbQuery.Append(" FROM  Imprese ")
        stbQuery.Append(" INNER JOIN Centri_Aziendali ON Imprese.Piva = Centri_Aziendali.Piva  ")
        stbQuery.Append(" INNER JOIN Appezzamento ON Appezzamento.PIVA = Centri_Aziendali.PIVA ")
        stbQuery.Append(" AND Appezzamento.sa_cod = Centri_Aziendali.sa_cod ")
        stbQuery.Append(" INNER JOIN Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA ")
        stbQuery.Append(" AND Appezzamento.sa_cod = Reg_Impianti.sa_cod AND Appezzamento.appezza = Reg_Impianti.appezza ")
        stbQuery.Append(" INNER JOIN Imprese_Progetti ON Imprese_Progetti.PIVA = Reg_Impianti.PIVA ")
        stbQuery.Append(" AND Imprese_Progetti.sa_cod = Reg_Impianti.sa_cod AND Imprese_Progetti.appezza = Reg_Impianti.appezza AND Imprese_Progetti.id_reg = Reg_Impianti.id_reg ")

        stbQuery.Append(" INNER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod " & vbCrLf)
        stbQuery.Append(" INNER JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod " & vbCrLf)

        '/**************************************************
        '         NUOVO METODO CON TABELLA TEMPORANEA

        '           --> AGGIUNGO QUESTA PARTE:

        stbQuery.Append(" INNER JOIN  #tempimpianti " & vbCrLf)
        stbQuery.Append(" ON #tempimpianti.Piva = Reg_Impianti.Piva AND Reg_Impianti.Sa_Cod = #tempimpianti.Sa_Cod AND  Reg_Impianti.Appezza = #tempimpianti.Appezza AND  Reg_Impianti.Id_Reg = #tempimpianti.Id_Reg " & vbCrLf)
        '/**************************************************

        Select Case CInt(filtroDistinte) 'TODO ComboDistinta finire le date

            Case 0 ' distinta attiva alla data odierna
                stbQuery.Append(" WHERE     (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Date.Today) & ") " & vbCrLf)
                stbQuery.Append(" AND     (Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Date.Today) & ") " & vbCrLf)

            Case 1 ' tutte le distinte

            Case 2 ' distinta attiva alla data specificata
                stbQuery.Append(" WHERE     (Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(CDate(dataDistinta)) & ") " & vbCrLf)
                stbQuery.Append(" AND     (Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(CDate(dataDistinta)) & ") " & vbCrLf)
        End Select

        stbQuery.Append(" ORDER BY Rag_Soc, Sa_Nome, App_nome, Veg_Des, Cul_des ")


        '/**************************************************
        Query4_TempTableJoin = stbQuery.ToString

        stbQuery = Nothing
        '/**************************************************


        '----------------------------------------------------
        '--- Recupero il datatable --------------------------
        '----------------------------------------------------

        '/**************************************************
        '         NUOVO METODO CON TABELLA TEMPORANEA  
        Dim obj_MultiQuery As New AgronicaCoreDataProvider.AccessoMultiQuery

        'DT = obj_MultiQuery.MLT_SelectFiltrataConTabellaTemporanea(Query1_TempTableCreazione, _
        '                                                            Query2_TempTableIndice, _
        '                                                            Query3_TempTableFill, _
        '                                                            Query4_TempTableJoin, _
        '                                                            1, _
        '                                                            objSession("ASG_Connessione_Se..."), _
        '                                                            objSession("ASG_PathFi..."), _
        '                                                            "", _
        '                                                            Messaggio)


        DT = obj_MultiQuery.MLT_SelectFiltrataConTabellaTemporanea_2013(Query1_TempTableCreazione, _
                                                                    Query2_TempTableIndice, _
                                                                    Query3_TempTableFill, _
                                                                    Query4_TempTableJoin, _
                                                                    objServer.StringaConnessione, _
                                                                    Messaggio)



        'Verifico la presenza di errori
        If Not IsNothing(Messaggio) AndAlso Messaggio <> "" Then
            Throw New Exception("Modulo: " & DescrizioneFunzione & " : " & Messaggio)
        Else
            Return DT
        End If


    End Function

    '###############################################################################################
    'legge la chiave del web.config
    'se non c'è imposta il valore di default
    Public Function Recupera_TabelleTemp_Mode() As Integer

        '<add key="TabelleTemp_Mode" value="1"/> <!-- 1: CREAZIONE TABELLA TEMPORANEA TRAMITE SELECT INSERT INTO | 2: TRAMITE CREATE TABLE -->

        '1: CREAZIONE TABELLA TEMPORANEA TRAMITE SELECT - INSERT INTO
        '2: CREAZIONE TABELLA TEMPORANEA TRAMITE CREATE TABLE

        Dim TabelleTemp_Mode As Integer

        If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("TabelleTemp_Mode")) And _
                System.Configuration.ConfigurationManager.AppSettings("TabelleTemp_Mode") <> "" Then

            TabelleTemp_Mode = System.Configuration.ConfigurationManager.AppSettings("TabelleTemp_Mode")

            If TabelleTemp_Mode <> 1 And TabelleTemp_Mode <> 2 Then
                TabelleTemp_Mode = 1
            End If

        Else
            TabelleTemp_Mode = 1

        End If


        Return TabelleTemp_Mode


    End Function

    '###############################################################################################
    'legge la chiave del web.config
    'se non c'è imposta il valore di default
    Public Function Recupera_Str_TabelleTemp_RegolaConfronto() As String

        '<add key="Str_TabelleTemp_RegolaConfronto" value=""/> <!-- Esempio: COLLATE SQL_Latin1_General_CP850_CI_AS -->

        'Recupera la regola di confronto

        Dim Str_TabelleTemp_RegolaConfronto As String

        If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("Str_TabelleTemp_RegolaConfronto")) And _
               System.Configuration.ConfigurationManager.AppSettings("Str_TabelleTemp_RegolaConfronto") <> "" Then

            Str_TabelleTemp_RegolaConfronto = System.Configuration.ConfigurationManager.AppSettings("Str_TabelleTemp_RegolaConfronto")

        Else
            Str_TabelleTemp_RegolaConfronto = " COLLATE SQL_Latin1_General_CP850_CI_AS "

        End If


        Return Str_TabelleTemp_RegolaConfronto


    End Function

End Class