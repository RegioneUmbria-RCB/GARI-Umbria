Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports Importazioni_OPTA
Imports AgronicaCoreXML.XML_Stampe
Imports ClosedXML.Excel
Imports System.Linq
Imports System.Data
Imports System.Data.DataSetExtensions
Imports AgronicaCoreVarieBIZ

Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class TabaccoStatistiche
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String

    Private Sub TabaccoStatistiche_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        'Per gestire il pulsante "Indietro"
        AddHandler Master.ImgBtnAnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        Master.Lbl_Titolo.Text = "Analisi Dati di Cura"

        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        objparametri_utenti_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Utenti)

        caricaFiltriReport()
    End Sub

    <Services.WebMethod(EnableSession:=True)> _
    Public Shared Function LeggiEsitoRicerca(ByVal TipoExport As String, _
                                            ByVal anno As String, ByVal piva As String, _
                                            ByVal cul_Cod As String, ByVal cod_Tecnico As String) As RispostaStandard

        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim dt As New DataTable
        Dim res As Boolean = False

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        'imposto il tipo di ricerca
        Dim uiC As Integer = -1 'lo inizializzo, altrimenti va sempre nel 1°
        Select Case TipoExport
            Case TipoTabelleRintraccioOpta.Rintraccio_X_Collo_OPTA
                uiC = enum_Impostazioni_Utenti.UTENTE_TABACCO_EXPORT_ALL
            Case TipoTabelleRintraccioOpta.Rintraccio_X_Appezzamento
                uiC = enum_Impostazioni_Utenti.UTENTE_TABACCO_EXPORT_APPEZZA
            Case TipoTabelleRintraccioOpta.Rintraccio_X_CoronaAppezzamento_OPTA
                uiC = enum_Impostazioni_Utenti.UTENTE_TABACCO_EXPORT_CORONA
            Case TipoTabelleRintraccioOpta.Rintraccio_X_Grado_Opta
                uiC = enum_Impostazioni_Utenti.UTENTE_TABACCO_EXPORT_GRADO
            Case TipoTabelleRintraccioOpta.Rintraccio_X_Buchi
                uiC = enum_Impostazioni_Utenti.UTENTE_TABACCO_EXPORT_BUCHI
        End Select

        Try
            If TipoExport <= 4 Then
                'leggo i dati da database
                res = Rintraccio.EsportaRintraccioOpta(TipoExport, dt, objParametri_Server)
            ElseIf TipoExport >= 5 AndAlso TipoExport <= 7 Then
                'è il report Crystal
                res = estraiDatiPerReport(dt, anno, piva, cul_Cod, cod_Tecnico, objParametri_Server)
            Else
                res = False
            End If

            If res = False Then
                r.RispostaOK = False
                r.Errore = "Errore: nessun dato estratto"
                Exit Try
            End If

            'estraggo dalle impostazioni utente i nomi delle colonne da visualizzare
            Dim lImposta As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtImposta1 As DataTable = _
                lImposta.Leggi_Utente_Poi_SuperUser(uiC, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim l1 As New List(Of String)
            Dim liCol As String()

            If dtImposta1.Rows.Count > 0 Then
                liCol = dtImposta1.Rows(0)("Impostazione_Valore_1").Split({"|"c, "*"c})
            Else
                For Each cc As DataColumn In dt.Columns
                    l1.Add(cc.ColumnName)
                    l1.Add(cc.ColumnName)
                Next
                liCol = l1.ToArray
            End If

            If TipoExport >= 5 AndAlso TipoExport <= 7 Then
                For Each cc As DataColumn In dt.Columns
                    l1.Add(cc.ColumnName)
                    l1.Add(cc.ColumnName)
                Next
                liCol = l1.ToArray
            End If

            'Aggiungo la tabella in sessione
            Dim nomeVarDtInSession As String = "WAExport_Rintraccio_X_Collo_OPTA"
            HttpContext.Current.Session.Add(nomeVarDtInSession, dt)

            'creo la lista di colonneNome
            Dim l As New List(Of ColonneNome)
            For i As Integer = 0 To liCol.Count - 1 Step 2
                For Each col As DataColumn In dt.Columns
                    If liCol(i).ToLower = col.ColumnName.ToLower Then
                        l.Add(New ColonneNome(col, liCol(i + 1)))
                    End If
                Next
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable(dt, l)
            r.opzioniWatable.nomeVarDtInSession = nomeVarDtInSession
            r.opzioniWatable.PrefissoNomeFileExport = "ExportDatiCura_"
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante la lettura. Dettagli: " & ex.Message
        End Try

        Return r

    End Function

    <Services.WebMethod(EnableSession:=True)> _
    Public Shared Function EsportaSuExcel(ByVal listaFiltriStr As String, _
                                          ByVal nomeVarSessionDt As String, _
                                          ByVal prefissoNomeFile As String) As rispostaStandard
        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim dt As DataTable = HttpContext.Current.Session(nomeVarSessionDt)
            'verifico se esiste la variabile di sessione con il dt
            If IsNothing(dt) Then
                r.RispostaOK = False
                r.Errore = "Errore durante l'operazione: datatable non trovato in sessione"
                Exit Try
            End If

            'Imposto il dt filtrandolo in base ai filtri passati come parametro
            Dim dtFiltrato As DataTable = AgronicaControlli_2010.jquery_watable_modificato.FiltraDTconFiltriWatable(dt, listaFiltriStr)
            dtFiltrato.TableName = "Esportazione"

            'dato che la funzione di esportazione legge il capiton del datacolum che io ho sfruttato per salvare altre info,
            'metto come caption il nome della colonna
            'For Each dc As DataColumn In dtFiltrato.Columns
            '    dc.Caption = dc.ColumnName
            'Next

            'percorso e nome del file da esportare
            Dim nomeFileUnivoco As String = prefissoNomeFile & "_" & AgronicaCoreUtility.FileSystemHelper.NomeFileUnivoco(".xlsx")
            Dim _AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            Dim path_file As String = _AgroWebConfig.GestioneAllegati_Repository & "\" & nomeFileUnivoco

            'Creo il file xlsx a partire dal DT
            Dim workbook = New ClosedXML.Excel.XLWorkbook()
            workbook.Worksheets.Add(dtFiltrato)
            workbook.SaveAs(path_file)

            'creo l'url dove si deve andare a prendere il file
            Dim url As String = _AgroWebConfig.LinkAgronicaStampe.ToLower().Replace("gestionerichieste.aspx", "") & "File_Allegati/" & nomeFileUnivoco

            r.RispostaOK = True
            r.RispostaStringa = url

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function


    Private Shared Function estraiDatiPerReport(ByRef dt As DataTable, ByVal anno As String, ByVal piva As String, _
                                                 ByVal cul_Cod As String, ByVal cod_Tecnico As String, ByRef objParametri As AgronicaCoreParametri) As Boolean
        Dim dp As New AgronicaCoreDataProvider.DataProvider
        dt = New DataTable
        Dim stb As New StringBuilder()

        stb.Append("SELECT         " & vbCrLf)
        stb.Append("    rr.ID, rr.Anno, rr.Centro, rr.ID_Partita, rr.Prog, rr.Data, rr.Grado, rgc.Colore, rgc.ClasseTargetNum, rr.Tara, rr.Kg, rr.Colli, rr.Collo, rr.QSup,  " & vbCrLf)
        stb.Append("    rr.Sostanza_E1, rr.Sostanza_E2, rr.Sostanza_E3, rr.Sostanza_E4, rr.Sostanza_E5, rr.Sostanza_E6, rr.Caratteristica_1, rr.Caratteristica_2,  " & vbCrLf)
        stb.Append("    rr.Barcode, rr.ID_Gruppo_Varietale, rr.Prog_Partita, rr.CODICE_CUAA, rr.QI,  " & vbCrLf)
        stb.Append("    rrir.PIVA, i.rag_soc, rrir.SA_COD, ca.sa_nome, rrir.APPEZZA, rrir.ID_REG, a.APP_NOME,  " & vbCrLf)
        stb.Append("    ac.val_cod AS NumeroAppezzamento, rrir.CUL_COD, rrir.Cul_Des, rrir.Sup_Imp, rrir.Mat_Cod, rrir.Mat_Des, a.SUP_APP, " & vbCrLf)
        stb.Append("    ic.val_cod as 'cod_Tecnico', iif (c.id_CF = 0, c.cognome + ' ' + c.nome,c.Rag_Soc) as 'Tecnico' " & vbCrLf)

        stb.Append("  ")
        stb.Append(" FROM __Rintraccio_Ritiro rr  " & vbCrLf)
        stb.Append(" LEFT OUTER JOIN __Rintraccio_RitiroXImpiantiRaccolti rrir  " & vbCrLf)
        stb.Append("    ON rr.ID = rrir.ID  " & vbCrLf)
        stb.Append(" LEFT OUTER JOIN __Rintraccio_GradoxColore rgc  " & vbCrLf)
        stb.Append("    ON rr.Grado = rgc.Grado  " & vbCrLf)
        stb.Append(" LEFT OUTER JOIN Imprese i  " & vbCrLf)
        stb.Append("    ON rrir.PIVA = i.PIVA  " & vbCrLf)
        stb.Append(" LEFT OUTER JOIN Centri_Aziendali ca  " & vbCrLf)
        stb.Append("    ON rrir.PIVA = ca.PIVA  " & vbCrLf)
        stb.Append("    AND rrir.SA_COD = ca.sa_cod  " & vbCrLf)
        stb.Append(" LEFT OUTER JOIN Appezzamento_Codici ac  " & vbCrLf)
        stb.Append("    ON rrir.PIVA = ac.PIVA  " & vbCrLf)
        stb.Append("    AND rrir.SA_COD = ac.sa_cod  " & vbCrLf)
        stb.Append("    AND rrir.APPEZZA = ac.appezza  " & vbCrLf)
        stb.Append("    AND ac.id_cod = 1018  " & vbCrLf)
        stb.Append(" LEFT OUTER JOIN Appezzamento a  " & vbCrLf)
        stb.Append("    ON rrir.PIVA = a.PIVA  " & vbCrLf)
        stb.Append("    AND rrir.SA_COD = a.SA_COD  " & vbCrLf)
        stb.Append("    AND rrir.APPEZZA = a.APPEZZA" & vbCrLf)
        stb.Append("  LEFT OUTER JOIN imprese_codici ic " & vbCrLf)
        stb.Append("     ON rrir.PIVA = ic.PIVA  " & vbCrLf)
        stb.Append("     AND ic.id_cod = 1088  " & vbCrLf)
        stb.Append("  LEFT OUTER JOIN Contatti c  " & vbCrLf)
        stb.Append("     ON ic.val_cod = c.Cod_Contatto" & vbCrLf)


        stb.Append(" WHERE 1=1 " & vbCrLf)
        If anno <> "null" Then
            stb.Append(" AND rr.Anno = " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveNum_NULL(anno) & vbCrLf)
        End If
        If piva <> "null" Then
            stb.Append(" AND rrir.PIVA = " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText_NULL(piva) & vbCrLf)
        End If
        If cul_Cod <> "null" Then
            stb.Append(" AND rrir.CUL_COD = " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveNum_NULL(cul_Cod) & vbCrLf)
        End If
        If cod_Tecnico <> "null" Then
            stb.Append(" AND ic.val_cod = " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText_NULL(cod_Tecnico) & vbCrLf)
        End If

        dt = dp.EseguiQuery_Lettura(objParametri, stb.ToString(), "OptaImport")
        Return True
    End Function

    '##########################################################################################################################################
    <Services.WebMethod(EnableSession:=True)> _
    Public Shared Function EsportaReport(ByVal TipoExport As String) As rispostastandard
        Dim r As New rispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim dt As DataTable = HttpContext.Current.Session("WAExport_Rintraccio_X_Collo_OPTA")

        '------------------------------------------------DEFINIZIONE DATASET-------------------------------------------
        Dim ds As New dsReportOPTA
        ds.DataSetName = "dsReportOPTA"

        '------------------------------------------------DICHIARAZIONE VARIABILI-------------------------------------------
        Dim htPercRaccolteApp As Hashtable
        Dim odPercColoriApp As OrderedDictionary
        Dim odTecnicaColturale As OrderedDictionary
        Dim qiMedioApp, produzioneKG, SuperficieHa, resaKgHa As Decimal
        Dim anno, azienda, varieta, nomeApp, nomeTec As String
        Dim drTemplate As DataRow 'datarow che fa da template con la chiave di riga

        '------------------------------------------------CALCOLO DATI-------------------------------------------
        Select Case TipoExport
            Case "5" 'report che per ogni appezzamento calcola i dati

                'ottengo la lista di tutti i piva / sacod / appezza che non sono null, e ne faccio il distinct
                Dim PivaSacodAppezzaIdReg = (From x In dt.AsEnumerable()
                                        Where Not IsNothing(x.Field(Of String)("Piva")) AndAlso Not IsNothing(x.Field(Of Integer)("Sa_cod")) AndAlso Not IsNothing(x.Field(Of Integer)("Appezza")) AndAlso Not IsNothing(x.Field(Of Integer)("Id_Reg"))
                                        Order By x.Field(Of String)("Piva"), x.Field(Of Integer)("Sa_cod"), x.Field(Of Integer)("Appezza"), x.Field(Of Integer)("Id_Reg")
                                        Select New With {Key .piva = x.Field(Of String)("Piva"), Key .saCod = x.Field(Of Integer)("Sa_cod"), Key .appezza = x.Field(Of Integer)("Appezza"), Key .Id_Reg = x.Field(Of Integer)("Id_Reg")} Distinct).ToArray()

                'per ogni piva / sacod / appezza...
                For Each elem In PivaSacodAppezzaIdReg
                    '..estraggo tutte le righe riguardanti l'appezzamento
                    Dim dtApp As DataTable = (From x In dt.AsEnumerable()
                                              Where x.Field(Of String)("Piva") = elem.piva AndAlso x.Field(Of Integer)("Sa_cod") = elem.saCod AndAlso x.Field(Of Integer)("Appezza") = elem.appezza AndAlso x.Field(Of Integer)("Id_Reg") = elem.Id_Reg
                                              Select x).CopyToDataTable()

                    'calcolo i dati per il report
                    azienda = calcolaAzienda(dtApp)
                    varieta = calcolaColtura(dtApp)
                    nomeApp = calcolaNomeApp(dtApp)
                    nomeTec = calcolaNomeTec(dtApp)
                    htPercRaccolteApp = calcolaPercRaccolte(dtApp)
                    odPercColoriApp = calcolaPercColori(dtApp)
                    qiMedioApp = calcolaQiMedio(dtApp)
                    produzioneKG = calcolaProduzione(dtApp)
                    SuperficieHa = calcolaSuperficie(dtApp)
                    resaKgHa = produzioneKG / SuperficieHa
                    anno = calcolaAnno(dtApp)

                    'imposto i valori sul datatable dei colori
                    drTemplate = ds.Colori.NewColoriRow()
                    drTemplate.Item("Piva") = elem.piva
                    drTemplate.Item("Sa_Cod") = elem.saCod
                    drTemplate.Item("Appezza") = elem.appezza
                    drTemplate.Item("id_reg") = elem.Id_Reg
                    drTemplate.Item("Cul_Cod") = 0
                    drTemplate.Item("cod_Tecnico") = 0
                    assegnaValoriSuDTColori(ds, drTemplate, odPercColoriApp)

                    'imposto i valori sul datatable delle raccolte
                    drTemplate = ds.Raccolte.NewRaccolteRow()
                    drTemplate.Item("Piva") = elem.piva
                    drTemplate.Item("Sa_Cod") = elem.saCod
                    drTemplate.Item("Appezza") = elem.appezza
                    drTemplate.Item("id_reg") = elem.Id_Reg
                    drTemplate.Item("Cul_Cod") = 0
                    drTemplate.Item("cod_Tecnico") = 0
                    assegnaValoriSuDTRaccolte(ds, drTemplate, htPercRaccolteApp)

                    'imposto i valori sul datatable delle varie
                    drTemplate = ds.Varie.NewVarieRow()
                    drTemplate.Item("Piva") = elem.piva
                    drTemplate.Item("Sa_Cod") = elem.saCod
                    drTemplate.Item("Appezza") = elem.appezza
                    drTemplate.Item("id_reg") = elem.Id_Reg
                    drTemplate.Item("Cul_Cod") = 0
                    drTemplate.Item("cod_Tecnico") = 0
                    assegnaValoriSuDTVarie(ds, drTemplate, TipoExport, _
                                           azienda, varieta, nomeApp, nomeTec, _
                                           qiMedioApp, produzioneKG, SuperficieHa, resaKgHa, anno)

                    '----------------------------------TECNICA COLTURALE--------------------------------------------------------
                    'calcolo i dati per la tecnica colturale
                    Dim dtTecCol As DataTable
                    estraiDatiTecnicaColturale(dtTecCol, elem.piva, elem.saCod, elem.appezza, elem.Id_Reg)
                    odTecnicaColturale = calcolaDatiTecnicaColturale(dtTecCol, dtApp)

                    'imposto i valori sul datatable delle tecniche colturale
                    drTemplate = ds.TecCol.NewTecColRow()
                    drTemplate.Item("Piva") = elem.piva
                    drTemplate.Item("Sa_Cod") = elem.saCod
                    drTemplate.Item("Appezza") = elem.appezza
                    drTemplate.Item("id_reg") = elem.Id_Reg
                    drTemplate.Item("Cul_Cod") = 0
                    drTemplate.Item("cod_Tecnico") = 0
                    assegnaValoriSuDTTecCol(ds, drTemplate, odTecnicaColturale)

                Next

            Case "6" 'report che per ogni varietà calcola i dati
                Dim listaVarieta As Integer() = (From x In dt.AsEnumerable()
                                       Where Not IsNothing(x.Field(Of Integer)("Cul_Cod"))
                                       Select x.Field(Of Integer)("Cul_Cod")).Distinct().ToArray()

                For Each var As Integer In listaVarieta

                    Dim dtVar As DataTable = (From x In dt.AsEnumerable()
                                               Where Not IsNothing(x.Field(Of Integer)("Cul_Cod")) AndAlso x.Field(Of Integer)("Cul_Cod") = var
                                               Select x).CopyToDataTable()

                    azienda = calcolaAzienda(dtVar)
                    varieta = calcolaColtura(dtVar)
                    nomeApp = calcolaNomeApp(dtVar)
                    nomeTec = calcolaNomeTec(dtVar)
                    htPercRaccolteApp = calcolaPercRaccolte(dtVar)
                    odPercColoriApp = calcolaPercColori(dtVar)
                    qiMedioApp = calcolaQiMedio(dtVar)
                    produzioneKG = calcolaProduzione(dtVar)
                    SuperficieHa = calcolaSuperficie(dtVar)
                    resaKgHa = produzioneKG / SuperficieHa
                    anno = calcolaAnno(dtVar)

                    'imposto i valori sul datatable dei colori
                    drTemplate = ds.Colori.NewColoriRow()
                    drTemplate.Item("Piva") = ""
                    drTemplate.Item("Sa_Cod") = 0
                    drTemplate.Item("Appezza") = 0
                    drTemplate.Item("id_reg") = 0
                    drTemplate.Item("Cul_Cod") = var
                    drTemplate.Item("cod_Tecnico") = 0
                    assegnaValoriSuDTColori(ds, drTemplate, odPercColoriApp)

                    'imposto i valori sul datatable delle raccolte
                    drTemplate = ds.Raccolte.NewRaccolteRow()
                    drTemplate.Item("Piva") = ""
                    drTemplate.Item("Sa_Cod") = 0
                    drTemplate.Item("Appezza") = 0
                    drTemplate.Item("id_reg") = 0
                    drTemplate.Item("Cul_Cod") = var
                    drTemplate.Item("cod_Tecnico") = 0
                    assegnaValoriSuDTRaccolte(ds, drTemplate, htPercRaccolteApp)

                    'imposto i valori sul datatable delle varie
                    drTemplate = ds.Varie.NewVarieRow()
                    drTemplate.Item("Piva") = ""
                    drTemplate.Item("Sa_Cod") = 0
                    drTemplate.Item("Appezza") = 0
                    drTemplate.Item("id_reg") = 0
                    drTemplate.Item("Cul_Cod") = var
                    drTemplate.Item("cod_Tecnico") = 0
                    assegnaValoriSuDTVarie(ds, drTemplate, TipoExport, _
                                           azienda, varieta, nomeApp, nomeTec, _
                                           qiMedioApp, produzioneKG, SuperficieHa, resaKgHa, anno)

                Next

            Case "7" 'report che per ogni tecnico e varietà calcola i dati
                'ottengo la lista di tutti i tecnici / varietà che non sono null, e ne faccio il distinct
                Dim listaTecniciVarieta = (From x In dt.AsEnumerable()
                                        Where Not IsNothing(x.Field(Of String)("cod_Tecnico")) AndAlso Not IsNothing(x.Field(Of Integer)("Cul_Cod"))
                                        Order By x.Field(Of String)("cod_Tecnico"), x.Field(Of Integer)("Cul_Cod")
                                        Select New With {Key .tecnico = x.Field(Of String)("cod_Tecnico"), Key .varieta = x.Field(Of Integer)("Cul_Cod")} Distinct).ToArray()

                For Each elem In listaTecniciVarieta

                    Dim dtTecVar As DataTable = (From x In dt.AsEnumerable()
                               Where x.Field(Of String)("cod_Tecnico") = elem.tecnico AndAlso x.Field(Of Integer)("Cul_Cod") = elem.varieta
                               Select x).CopyToDataTable()

                    azienda = calcolaAzienda(dtTecVar)
                    varieta = calcolaColtura(dtTecVar)
                    nomeApp = calcolaNomeApp(dtTecVar)
                    nomeTec = calcolaNomeTec(dtTecVar)
                    htPercRaccolteApp = calcolaPercRaccolte(dtTecVar)
                    odPercColoriApp = calcolaPercColori(dtTecVar)
                    qiMedioApp = calcolaQiMedio(dtTecVar)
                    produzioneKG = calcolaProduzione(dtTecVar)
                    SuperficieHa = calcolaSuperficie(dtTecVar)
                    resaKgHa = produzioneKG / SuperficieHa
                    anno = calcolaAnno(dtTecVar)

                    'imposto i valori sul datatable dei colori
                    drTemplate = ds.Colori.NewColoriRow()
                    drTemplate.Item("Piva") = ""
                    drTemplate.Item("Sa_Cod") = 0
                    drTemplate.Item("Appezza") = 0
                    drTemplate.Item("id_reg") = 0
                    drTemplate.Item("Cul_Cod") = elem.varieta
                    drTemplate.Item("cod_Tecnico") = elem.tecnico
                    assegnaValoriSuDTColori(ds, drTemplate, odPercColoriApp)

                    'imposto i valori sul datatable delle raccolte
                    drTemplate = ds.Raccolte.NewRaccolteRow()
                    drTemplate.Item("Piva") = ""
                    drTemplate.Item("Sa_Cod") = 0
                    drTemplate.Item("Appezza") = 0
                    drTemplate.Item("id_reg") = 0
                    drTemplate.Item("Cul_Cod") = elem.varieta
                    drTemplate.Item("cod_Tecnico") = elem.tecnico
                    assegnaValoriSuDTRaccolte(ds, drTemplate, htPercRaccolteApp)

                    'imposto i valori sul datatable delle varie
                    drTemplate = ds.Varie.NewVarieRow()
                    drTemplate.Item("Piva") = ""
                    drTemplate.Item("Sa_Cod") = 0
                    drTemplate.Item("Appezza") = 0
                    drTemplate.Item("id_reg") = 0
                    drTemplate.Item("Cul_Cod") = elem.varieta
                    drTemplate.Item("cod_Tecnico") = elem.tecnico
                    assegnaValoriSuDTVarie(ds, drTemplate, TipoExport, _
                                           azienda, varieta, nomeApp, nomeTec, _
                                           qiMedioApp, produzioneKG, SuperficieHa, resaKgHa, anno)
                Next
        End Select

        Dim rpt As New rptOPTA
        rpt.SetDataSource(ds)
        HttpContext.Current.Session("Report") = rpt

        r.RispostaOK = True
        r.RispostaStringa = "../Stampe/VisualizzatoreReport.aspx?anteprima=" & AgronicaCoreDataProvider.Sicurezza.Stringa_Codifica("1", AgroKey_EncoderDecoder, Nothing)

        Return r

    End Function

    Public Shared Sub assegnaValoriSuDTColori(ByRef ds As dsReportOPTA, drTemplate As DataRow, odPercColoriApp As OrderedDictionary)

        Dim i As Integer = 0
        Dim dr As dsReportOPTA.ColoriRow
        For Each elem As DictionaryEntry In odPercColoriApp
            dr = ds.Colori.NewColoriRow
            dr.ItemArray = drTemplate.ItemArray.Clone()
            dr("NomeColore") = elem.Key

            Dim dati As String() = elem.Value.ToString().Split("~")
            dr("PercColore") = If(IsDBNull(dati(0)), Nothing, dati(0))
            dr("Ordine") = i
            dr("Tipo") = dati(3)
            dr("TargetClasse") = dati(2)
            dr("SumColoreClasse") = dati(1)

            ds.Colori.AddColoriRow(dr)
            i += 1
        Next
    End Sub

    Public Shared Sub assegnaValoriSuDTRaccolte(ByRef ds As dsReportOPTA, drTemplate As DataRow, htPercRaccolteApp As Hashtable)
        Dim dr As dsReportOPTA.RaccolteRow

        dr = ds.Raccolte.NewRaccolteRow
        dr.ItemArray = drTemplate.ItemArray.Clone()
        dr.Item("NomeRaccolta") = "Lugs (%)"
        dr.Item("PercRaccolta") = 0 + htPercRaccolteApp.Item(36) 'Mat_Cod 36 --> 1° raccolta
        dr("Ordine") = 1
        ds.Raccolte.AddRaccolteRow(dr)

        dr = ds.Raccolte.NewRaccolteRow
        dr.ItemArray = drTemplate.ItemArray.Clone()
        dr.Item("NomeRaccolta") = "Cutters (%)"
        dr.Item("PercRaccolta") = 0 + htPercRaccolteApp.Item(37) 'Mat_Cod 37 --> 2° raccolta
        dr("Ordine") = 2
        ds.Raccolte.AddRaccolteRow(dr)

        dr = ds.Raccolte.NewRaccolteRow
        dr.ItemArray = drTemplate.ItemArray.Clone()
        dr.Item("NomeRaccolta") = "Leaf (%)"
        dr.Item("PercRaccolta") = 0 + htPercRaccolteApp.Item(38) 'Mat_Cod 38 --> 3° raccolta
        dr("Ordine") = 3
        ds.Raccolte.AddRaccolteRow(dr)

        dr = ds.Raccolte.NewRaccolteRow
        dr.ItemArray = drTemplate.ItemArray.Clone()
        dr.Item("NomeRaccolta") = "Tips (%)"
        dr.Item("PercRaccolta") = 0 + htPercRaccolteApp.Item(39) 'Mat_Cod 39 --> 4° raccolta
        dr("Ordine") = 4
        ds.Raccolte.AddRaccolteRow(dr)
    End Sub

    Public Shared Sub assegnaValoriSuDTVarie(ByRef ds As dsReportOPTA, drTemplate As DataRow, TipoExport As Decimal, azienda As String, varieta As String, nomeAppezzamento As String, nomeTecnico As String, _
                                             qiMedioApp As Decimal, produzioneKG As Decimal, SuperficieHa As Decimal, resaKgHa As Decimal, anno As String)
        Dim dr As dsReportOPTA.VarieRow
        dr = ds.Varie.NewVarieRow
        dr.ItemArray = drTemplate.ItemArray.Clone()

        dr.Item("Ragione_Sociale") = If(IsNothing(azienda), "", azienda)
        dr.Item("Varieta") = If(IsNothing(varieta), "", varieta)
        dr.Item("Descrizione_Appezzamento") = If(IsNothing(nomeAppezzamento), "", nomeAppezzamento)
        dr.Item("Tecnico") = If(IsNothing(nomeTecnico), "", nomeTecnico)
        dr.Item("QI") = 0 + qiMedioApp
        dr.Item("Produzione") = 0 + produzioneKG
        dr.Item("Superficie") = 0 + SuperficieHa
        dr.Item("Resa") = 0 + resaKgHa
        dr.Item("Anno") = If(IsNothing(anno), "", anno)
        ds.Varie.AddVarieRow(dr)
    End Sub

    Private Shared Sub assegnaValoriSuDTTecCol(ByRef ds As dsReportOPTA, drTemplate As DataRow, ByRef odTecnicaColturale As OrderedDictionary)
        Dim dr As dsReportOPTA.TecColRow
        Dim i As Integer = 1

        For Each elem As DictionaryEntry In odTecnicaColturale
            dr = ds.TecCol.NewTecColRow
            dr.ItemArray = drTemplate.ItemArray.Clone()
            dr.Item("Ordine") = i
            dr.Item("NomeCampo") = elem.Key
            dr.Item("ValoreCampo") = elem.Value
            ds.TecCol.AddTecColRow(dr)
            i += 1
        Next

    End Sub


    Public Shared Function calcolaPercRaccolte(ByRef dt As DataTable) As Hashtable
        Dim ht As New Hashtable()
        Dim listaRaccolte As Int32() = (From x In dt.AsEnumerable()
                                    Select x.Field(Of Int32)("Mat_Cod")).Distinct().ToArray()

        Dim sommaTaraApp As Single = dt.AsEnumerable().Sum(Function(x) x.Field(Of Single)("tara"))
        Dim sommaKgApp As Single = dt.AsEnumerable().Sum(Function(x) x.Field(Of Single)("kg"))
        Dim sommaNettoApp As Single = sommaKgApp - sommaTaraApp

        For Each r As Int32 In listaRaccolte
            Dim sommaTaraRaccolta As Single = Aggregate x In dt
                                                Where x.Item("Mat_Cod") = r
                                                Into Sum(x.Field(Of Single)("tara"))
            Dim sommaKgRaccolta As Single = Aggregate x In dt
                                                Where x.Item("Mat_Cod") = r
                                                Into Sum(x.Field(Of Single)("Kg"))

            ht.Add(r, ((sommaKgRaccolta - sommaTaraRaccolta) * 100 / sommaNettoApp))
        Next

        Return ht
    End Function

    Public Shared Function calcolaPercColori(ByRef dt As DataTable) As OrderedDictionary
        'calcolo le tare ed i Kg totali prodotti
        Dim sommaTaraApp As Single = dt.AsEnumerable().Sum(Function(x) x.Field(Of Single)("tara"))
        Dim sommaKgApp As Single = dt.AsEnumerable().Sum(Function(x) x.Field(Of Single)("kg"))
        Dim sommaNettoApp As Single = sommaKgApp - sommaTaraApp

        'calcolo le percentuali per ogni classe (top, medium, ecc.) e lo metto nel hashtable
        Dim ht As New Hashtable
        Dim listaClasseNum As Integer() = (From x In dt.AsEnumerable()
                            Select x.Field(Of Integer)("ClasseTargetNum")).Distinct().ToArray()

        For Each n As Integer In listaClasseNum
            Dim sommaTara As Single = Aggregate x In dt
                                                Where x.Item("ClasseTargetNum") = n
                                                Into Sum(x.Field(Of Single)("tara"))
            Dim sommaKg As Single = Aggregate x In dt
                                                Where x.Item("ClasseTargetNum") = n
                                                Into Sum(x.Field(Of Single)("Kg"))

            ht.Add(n, ((sommaKg - sommaTara) * 100 / sommaNettoApp))
        Next

        'estraggo tutti i colori e le relative info
        Dim dtTuttiColori As DataTable
        estraiDatiColori(dtTuttiColori)

        Dim od As New OrderedDictionary()
        'calcolo le percentuali per ogni colore e lo metto nel OrderedDictionary
        Dim listaColori As String() = (From x In dtTuttiColori.AsEnumerable()
                                              Select x.Field(Of String)("Colore")
                                             ).Distinct().ToArray()

        For Each c As String In listaColori

            Dim sommaTara As Single = Aggregate x In dt
                        Where x.Item("Colore") = c
                        Into Sum(x.Field(Of Single)("tara"))

            Dim sommaKg As Single = Aggregate x In dt
                        Where x.Item("Colore") = c
                        Into Sum(x.Field(Of Single)("Kg"))

            Dim classeTargetNum As Integer = (From x In dtTuttiColori
                                             Where x.Item("Colore") = c
                                             Select x.Field(Of Integer)("classeTargetNum")).First()

            Dim classeTargetStr As String = (From x In dtTuttiColori
                                             Where x.Item("Colore") = c
                                             Select x.Field(Of String)("classeTargetStr")).First()

            Dim classeTargetVal As String = (From x In dtTuttiColori
                                             Where x.Item("Colore") = c
                                             Select x.Field(Of String)("classeTargetVal")).First()

            Dim dati As String = String.Join("~", ((sommaKg - sommaTara) * 100 / sommaNettoApp), _
                                             0 + ht(classeTargetNum), classeTargetVal, classeTargetStr)
            od.Add(c, dati)
        Next

        Return od
    End Function

    Public Shared Function calcolaQiMedio(ByRef dt As DataTable) As Decimal
        Dim qiMedio As Single = 0
        Dim sommaTaraApp As Single = dt.AsEnumerable().Sum(Function(x) x.Field(Of Single)("tara"))
        Dim sommaKgApp As Single = dt.AsEnumerable().Sum(Function(x) x.Field(Of Single)("kg"))
        Dim sommaNettoApp As Single = sommaKgApp - sommaTaraApp

        For Each riga As DataRow In dt.Rows
            qiMedio += (riga.Field(Of Single)("kg") - riga.Field(Of Single)("tara")) * riga.Field(Of Single)("QI")
        Next

        qiMedio /= sommaNettoApp

        Return qiMedio
    End Function

    Public Shared Function calcolaProduzione(ByRef dt As DataTable) As Decimal
        Dim sommaTaraApp As Single = dt.AsEnumerable().Sum(Function(x) x.Field(Of Single)("tara"))
        Dim sommaKgApp As Single = dt.AsEnumerable().Sum(Function(x) x.Field(Of Single)("kg"))
        Dim sommaNettoApp As Single = sommaKgApp - sommaTaraApp

        Return sommaNettoApp
    End Function

    Public Shared Function calcolaSuperficie(ByRef dt As DataTable) As Decimal
        Dim sommaSupImp As Single = 0
        Dim listaFatti As New List(Of String)
        Dim codImp As String

        For Each dr As DataRow In dt.Rows
            codImp = dr.Item("PIVA") & "~" & dr.Item("SA_Cod") & "~" & dr.Item("APPEZZA") & "~" & dr.Item("ID_REG")
            If Not listaFatti.Contains(codImp) Then
                sommaSupImp += dr("Sup_Imp")
                listaFatti.Add(codImp)
            End If
        Next

        Return sommaSupImp
    End Function

    Public Shared Function calcolaAnno(ByRef dt As DataTable) As String
        Dim listaAnni As Integer() = dt.AsEnumerable().Select(Function(x) x.Field(Of Int32)("anno")).Distinct().ToArray
        Dim listaAnniStr As String = String.Join("-", listaAnni)

        Return listaAnniStr
    End Function

    Public Shared Function calcolaAzienda(ByRef dt As DataTable) As String
        Dim listaDistinct As String() = dt.AsEnumerable().Select(Function(x) x.Field(Of String)("rag_soc")).Distinct().ToArray
        'se c'è un solo elemento restituisco quello, altrimenti non ritorno nulla
        Return If(listaDistinct.Count = 1, listaDistinct(0), Nothing)
    End Function

    Public Shared Function calcolaColtura(ByRef dt As DataTable) As String
        Dim listaDistinct As String() = dt.AsEnumerable().Select(Function(x) x.Field(Of String)("Cul_Des")).Distinct().ToArray
        'se c'è un solo elemento restituisco quello, altrimenti non ritorno nulla
        Return If(listaDistinct.Count = 1, listaDistinct(0), Nothing)
    End Function

    Public Shared Function calcolaNomeApp(ByRef dt As DataTable) As String
        Dim listaDistinct As String() = dt.AsEnumerable().Select(Function(x) x.Field(Of String)("APP_NOME")).Distinct().ToArray
        'se c'è un solo elemento restituisco quello, altrimenti non ritorno nulla
        Return If(listaDistinct.Count = 1, listaDistinct(0), Nothing)
    End Function

    Public Shared Function calcolaNomeTec(ByRef dt As DataTable) As String
        Dim listaDistinct As String() = dt.AsEnumerable().Select(Function(x) x.Field(Of String)("Tecnico")).Distinct().ToArray
        'se c'è un solo elemento restituisco quello, altrimenti non ritorno nulla
        Return If(listaDistinct.Count = 1, listaDistinct(0), Nothing)
    End Function

    Private Sub caricaFiltriReport()
        Dim dp As New AgronicaCoreDataProvider.DataProvider
        Dim dt As DataTable = New DataTable
        Dim stb As New StringBuilder()

        stb.Append("SELECT DISTINCT         " & vbCrLf)
        stb.Append("     rr.Anno,  " & vbCrLf)
        stb.Append("     rrir.PIVA, i.rag_soc, " & vbCrLf)
        stb.Append("     rrir.CUL_COD, rrir.Cul_Des,  " & vbCrLf)
        stb.Append("     ic.val_cod as 'cod_Tecnico', iif (c.id_CF = 0, c.cognome + ' ' + c.nome,c.Rag_Soc) as 'Tecnico' " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append("  FROM __Rintraccio_Ritiro rr   " & vbCrLf)
        stb.Append("  LEFT OUTER JOIN __Rintraccio_RitiroXImpiantiRaccolti rrir   " & vbCrLf)
        stb.Append("     ON rr.ID = rrir.ID   " & vbCrLf)
        stb.Append("  LEFT OUTER JOIN Imprese i   " & vbCrLf)
        stb.Append("     ON rrir.PIVA = i.PIVA   " & vbCrLf)
        stb.Append("   LEFT OUTER JOIN imprese_codici ic " & vbCrLf)
        stb.Append("     ON rrir.PIVA = ic.PIVA  " & vbCrLf)
        stb.Append("     AND ic.id_cod = 1088  " & vbCrLf)
        stb.Append("   LEFT OUTER JOIN Contatti c  " & vbCrLf)
        stb.Append("     ON ic.val_cod = c.Cod_Contatto")

        dt = dp.EseguiQuery_Lettura(objParametri_Server, stb.ToString(), "OptaImport")

        Dim listaAnno As New List(Of ListItem)
        Dim listaVarieta As New List(Of ListItem)
        Dim listaTecnico As New List(Of ListItem)
        Dim listaAzienda As New List(Of ListItem)

        For Each dr As DataRow In dt.Rows
            If Not IsDBNull(dr("Anno")) Then
                listaAnno.Add(New ListItem(dr("Anno"), dr("Anno")))
            End If

            If Not IsDBNull(dr("Cul_Cod")) AndAlso Not IsDBNull(dr("Cul_Des")) Then
                listaVarieta.Add(New ListItem(dr("Cul_Des"), dr("Cul_Cod")))
            End If

            If Not IsDBNull(dr("cod_Tecnico")) AndAlso Not IsDBNull(dr("Tecnico")) Then
                listaTecnico.Add(New ListItem(dr("Tecnico"), dr("cod_Tecnico")))
            End If

            If Not IsDBNull(dr("PIVA")) AndAlso Not IsDBNull(dr("rag_soc")) Then
                listaAzienda.Add(New ListItem(dr("rag_soc"), dr("PIVA")))
            End If
        Next

        listaAnno = (From item In listaAnno Order By item.Text Select item).Distinct.ToList()
        listaVarieta = (From item In listaVarieta Order By item.Text Select item).Distinct.ToList()
        listaTecnico = (From item In listaTecnico Order By item.Text Select item).Distinct.ToList()
        listaAzienda = (From item In listaAzienda Order By item.Text Select item).Distinct.ToList()
        impostaDdlFiltro(ddlAnno, listaAnno)
        impostaDdlFiltro(ddlVarieta, listaVarieta)
        impostaDdlFiltro(ddlTecnico, listaTecnico)
        impostaDdlFiltro(ddlAzienda, listaAzienda)

    End Sub


    Private Function impostaDdlFiltro(ByRef ddl As DropDownList, ByRef lista As List(Of ListItem))
        Dim li As New ListItem("Selezionare...", "null")
        lista.Insert(0, li)
        ddl.DataTextField = "Text"
        ddl.DataValueField = "Value"
        ddl.DataSource = lista
        ddl.DataBind()
    End Function

    Private Shared Function estraiDatiTecnicaColturale(ByRef dt As DataTable, piva As String, sa_cod As Integer, appezza As Integer, id_reg As Integer) As Boolean
        Dim objParametri As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim dp As New AgronicaCoreDataProvider.DataProvider
        dt = New DataTable
        Dim stb As New StringBuilder()

        stb.Append(" SELECT a.piva,a.Sa_Cod,md.Appezza,md.Id_Destinazione,a.Id_Agenda,a.Lav_Cod,a.des_lib,m.Data_Movimento " & vbCrLf)
        stb.Append(" FROM Agenda a " & vbCrLf)
        stb.Append(" INNER JOIN Movimenti m " & vbCrLf)
        stb.Append("      ON a.piva=m.piva " & vbCrLf)
        stb.Append("      AND a.Sa_Cod=m.Sa_Cod " & vbCrLf)
        stb.Append("      AND a.Id_Agenda=m.Id_Agenda " & vbCrLf)
        stb.Append(" INNER JOIN Mov_Destinazioni md " & vbCrLf)
        stb.Append("      ON md.Piva=m.piva " & vbCrLf)
        stb.Append("      AND md.Sa_Cod=m.Sa_Cod " & vbCrLf)
        stb.Append("      AND md.Id_Agenda=m.Id_Agenda " & vbCrLf)
        stb.Append("      AND md.Id_Mov=m.Id_Mov " & vbCrLf)
        stb.Append(" WHERE a.piva = " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText_NULL(piva) & vbCrLf)
        stb.Append("      AND a.Sa_Cod = " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveNum_NULL(sa_cod) & vbCrLf)
        stb.Append("      AND md.Appezza = " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveNum_NULL(appezza) & vbCrLf)
        stb.Append("      AND md.Id_Destinazione = " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveNum_NULL(id_reg) & vbCrLf)
        stb.Append("      AND md.Tipo_Destinazione = 0 " & vbCrLf)
        stb.Append("      AND a.Lav_Cod in (12,71,125)")

        dt = dp.EseguiQuery_Lettura(objParametri, stb.ToString(), "OptaTecColt")
        Return True
    End Function

    Private Shared Function calcolaDatiTecnicaColturale(ByRef dtTabTecCol As DataTable, ByRef dtApp As DataTable) As OrderedDictionary
        Dim od As New OrderedDictionary()

        Dim contaE1 As Int32 = dtApp.AsEnumerable().Count(Function(x) x.Field(Of Boolean)("Sostanza_E1"))
        Dim contaE2 As Int32 = dtApp.AsEnumerable().Count(Function(x) x.Field(Of Boolean)("Sostanza_E2"))
        Dim contaE3 As Int32 = dtApp.AsEnumerable().Count(Function(x) x.Field(Of Boolean)("Sostanza_E3"))
        Dim contaE4 As Int32 = dtApp.AsEnumerable().Count(Function(x) x.Field(Of Boolean)("Sostanza_E4"))
        Dim contaE5 As Int32 = dtApp.AsEnumerable().Count(Function(x) x.Field(Of Boolean)("Sostanza_E5"))
        Dim contaE6 As Int32 = dtApp.AsEnumerable().Count(Function(x) x.Field(Of Boolean)("Sostanza_E6"))
        Dim contaC1 As Int32 = dtApp.AsEnumerable().Count(Function(x) x.Field(Of Boolean)("Caratteristica_1"))
        Dim contaC2 As Int32 = dtApp.AsEnumerable().Count(Function(x) x.Field(Of Boolean)("Caratteristica_2"))

        Dim contaScatole As Int32 = dtApp.Rows.Count

        od.Add("NTRM1 (%)", (contaE1 * 100 / contaScatole).ToString("F2"))
        od.Add("NTRM2 (%)", (contaE2 * 100 / contaScatole).ToString("F2"))
        od.Add("UMIDITA' 1 (%)", (contaE3 * 100 / contaScatole).ToString("F2"))
        od.Add("UMIDITA' 2 (%)", (contaE4 * 100 / contaScatole).ToString("F2"))
        od.Add("SCLEROTINIA 1 (%)", (contaE5 * 100 / contaScatole).ToString("F2"))
        od.Add("SCLEROTINIA 2 (%)", (contaE6 * 100 / contaScatole).ToString("F2"))
        od.Add("GRANDINE 1 (%)", (contaC1 * 100 / contaScatole).ToString("F2"))
        od.Add("GRANDINE 2 (%)", (contaC2 * 100 / contaScatole).ToString("F2"))

        Return od
    End Function

    Private Shared Function estraiDatiColori(ByRef dt As DataTable) As Boolean
        Dim objParametri As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim dp As New AgronicaCoreDataProvider.DataProvider
        dt = New DataTable
        Dim stb As New StringBuilder()

        stb.Append(" SELECT DISTINCT Colore, ClasseTargetNum, ClasseTargetStr, ClasseTargetVal, Ordine " & vbCrLf)
        stb.Append(" FROM __Rintraccio_GradoxColore " & vbCrLf)
        stb.Append(" ORDER BY Ordine " & vbCrLf)

        dt = dp.EseguiQuery_Lettura(objParametri, stb.ToString(), "OptaDatiColori")
        Return True
    End Function

    ''' <summary>
    ''' Bottone di annullamento
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri( _
                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010, _
                                       enum_PagineGiasOnline_2010.Menu, _
                                       enum_PagineAgenda_2010.Menu, "", "", "", 0, "")

        Response.Redirect(link)

    End Sub
End Class