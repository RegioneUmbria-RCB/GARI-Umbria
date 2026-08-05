Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class ImportatoreMassivo

    Public Function Importa_DatiMassivo(ByVal Flag_ImportaAnagrafica As Boolean,
                                           ByVal Flag_ImportaMacchine As Boolean,
                                           ByVal Flag_ImportaCatasto As Boolean,
                                           ByVal Flag_ImportaPianoColturale As Boolean,
                                           ByVal StringaConnessione As String,
                                           ByVal Utente_Username As String,
                                           ByVal Utente_Password As String,
                                           ByVal ProgressivoGIAS As Integer,
                                           ByVal CodiceChiaveCliente As Integer,
                                           ByVal LinkWSImportaGIAS As String,
                                           ByVal LogDirectory As String,
                                           ByVal LogFileName As String,
                                           ByRef Messaggio As String,
                                           ByRef LogCodificheMancantiSpecie As String,
                                           ByRef LogCodificheMancantiVarieta As String,
                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                                           ByRef Piva_Padre As String) As Boolean

        Dim Dt_Imprese As New DataTable
        Dim Flag_Risultato As Boolean = False
        Dim NomeRoutine As String = "Importa_Dati"

        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = objParametri_Server.LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        Dim objLog As New AgronicaCoreDataProvider.LogProvider

        objLog.Scrivi_LOG(objParametri_Server,
                   NomeRoutine,
                   "Inizio importazione",
                   CustomLOGParams:=customLOGParams)

        'Leggo i File
        Crea_Dt_Imprese(StringaConnessione, Messaggio, Dt_Imprese)

        objLog.Scrivi_LOG(objParametri_Server,
                         NomeRoutine,
                         Messaggio,
                         CustomLOGParams:=customLOGParams)

        If Dt_Imprese.Rows.Count = 0 Then
            objLog.Scrivi_LOG(objParametri_Server,
                NomeRoutine,
                "Importazione arrestata a causa di errore.",
                CustomLOGParams:=customLOGParams)
            Messaggio = "Impossibile importare i dati: " & Messaggio
            Return False
        Else
            objLog.Scrivi_LOG(objParametri_Server,
             NomeRoutine,
             "Inizio lettura dei dati.",
             CustomLOGParams:=customLOGParams)
        End If

        If Dt_Imprese.Rows.Count > 0 Then

            For Each rowCuaa In Dt_Imprese.Rows
                If IsDBNull(rowCuaa(0)) Then
                    Continue For
                End If

                Dim messImportazione As String = ""

                Dim Cuaa As String = rowCuaa(0)

                Try


                    Cuaa = Cuaa.Trim
                    Cuaa = Cuaa.Replace("'", "")
                    If Cuaa.Length < 11 Then
                        Cuaa = Integer.Parse(Cuaa).ToString("D11")
                    End If

                    objLog.Scrivi_LOG(objParametri_Server,
                             NomeRoutine,
                             "Importo Impresa:" & Cuaa,
                             CustomLOGParams:=customLOGParams)

                    Dim mesgIniziale = "Importo Impresa:" & Cuaa & ": "

                    Importa_Impresa(Cuaa,
                                    Flag_ImportaAnagrafica,
                                    Flag_ImportaMacchine,
                                    Flag_ImportaCatasto,
                                    Flag_ImportaPianoColturale,
                                    StringaConnessione,
                                    Utente_Username,
                                    Utente_Password,
                                    ProgressivoGIAS,
                                    CodiceChiaveCliente,
                                    LinkWSImportaGIAS,
                                    LogDirectory,
                                    LogFileName,
                                    messImportazione,
                                    LogCodificheMancantiSpecie,
                                    LogCodificheMancantiVarieta,
                                    objParametri_Server,
                                    objParametri_Utenti,
                                    Piva_Padre)

                    objLog.Scrivi_LOG(objParametri_Server,
                             NomeRoutine,
                             messImportazione,
                             CustomLOGParams:=customLOGParams)

                    Messaggio &= "<br/> <p>" & mesgIniziale & messImportazione & "</p>"

                Catch ex As Exception

                    objLog.Scrivi_LOG(objParametri_Server,
                             NomeRoutine,
                             "Errore su :" & Cuaa & " - " & ex.Message,
                             CustomLOGParams:=customLOGParams)

                End Try

            Next

        End If

        objLog.Scrivi_LOG(objParametri_Server,
                             NomeRoutine,
                             "Importazione massiva terminata",
                             CustomLOGParams:=customLOGParams)

    End Function

    Private Sub Crea_Dt_Imprese(ByVal StringaConnessione As String, ByRef Messaggio As String, ByRef Dt As DataTable)

        Try

            Dim ds As New DataSet
            Dim MyConnection As New OleDb.OleDbConnection(StringaConnessione)
            MyConnection.Open()
            Dim dtSheet = MyConnection.GetSchema("Tables")
            Dim firstSheet = dtSheet.Rows(0)("TABLE_NAME").ToString()
            Dim da As New OleDb.OleDbDataAdapter("select * from [" & firstSheet & "]", MyConnection)
            da.Fill(ds, "fileXls")
            MyConnection.Close()

            Dt = ds.Tables(0)
        Catch ex As Exception
            Messaggio = "Errore all'apertura del file excel: " & ex.Message
        End Try


    End Sub

    Public Function Importa_Impresa(ByVal Cuaa As String,
                                           ByVal Flag_ImportaAnagrafica As Boolean,
                                           ByVal Flag_ImportaMacchine As Boolean,
                                           ByVal Flag_ImportaCatasto As Boolean,
                                           ByVal Flag_ImportaPianoColturale As Boolean,
                                           ByVal StringaConnessione As String,
                                           ByVal Utente_Username As String,
                                           ByVal Utente_Password As String,
                                           ByVal ProgressivoGIAS As Integer,
                                           ByVal CodiceChiaveCliente As Integer,
                                           ByVal LinkWSImportaGIAS As String,
                                           ByVal LogDirectory As String,
                                           ByVal LogFileName As String,
                                           ByRef Messaggio As String,
                                           ByRef LogCodificheMancantiSpecie As String,
                                           ByRef LogCodificheMancantiVarieta As String,
                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                                           ByRef Piva_Padre As String)

        Dim import_umbria As New Importatore_UMBRIA.AGEA_UMBRIA_Utility

        Dim Fascicolo_Umbria As String = ""
        Dim strErr As String = ""
        Dim statoWS As Boolean
        Dim messaggioWS As String = ""
        Dim piva As String = ""

        Dim Num_Scheda As String
        Dim DataValidazione As Date
        Dim OrigineOpr As String
        Dim allegatoDocumentoCod As Integer
        Dim xml_Response As String = ""

        Dim objImprese_Codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        piva = objImprese_Codici.Piva_from_CUAA(Cuaa, objParametri_Server)

        Dim esisteFascicolo As Boolean = import_umbria.FascicoloLeggiWSMemorizza_UMBRIA(
                            True,
                            0,
                            objParametri_Server,
                            Fascicolo_Umbria,
                            messaggioWS,
                            statoWS,
                            strErr,
                            piva,
                            Cuaa,
                            Num_Scheda,
                            DataValidazione,
                            OrigineOpr,
                            allegatoDocumentoCod,
                            xml_Response
                        )

        If Fascicolo_Umbria = "" Then
            Messaggio = "Fascicolo non presente"
            Return Messaggio
        End If

        Dim importatore As New Importatore

        If Flag_ImportaAnagrafica Then

            Dim ragSoc As String = ""
            Dim Indirizzo As String = ""
            Dim Cap As String = ""
            Dim Ista_Prov As String = ""
            Dim Istat_Com As String = ""
            Dim AziendaVisibile As Boolean



            Dim AziendaCreata = importatore.CreaAzienda(ProgressivoGIAS,
                                                        Utente_Password,
                                                        piva,
                                                        Cuaa,
                                                        Fascicolo_Umbria,
                                                        strErr,
                                                        Messaggio,
                                                        ragSoc, Indirizzo, Cap, Ista_Prov, Istat_Com, OrigineOpr,
                                                        objParametri_Server,
                                                        objParametri_Utenti,
                                                        AziendaVisibile,
                                                        Piva_Padre, Flag_ImportaMacchine, Flag_ImportaCatasto, False, False, False)
        End If

        If Flag_ImportaPianoColturale Then
            If piva = "" Then
                Throw New Exception("Partita Iva non trovata per il CUAA:" & Cuaa)
            End If

            Dim DtParticelle As New DataTable
            Dim dtPartielleAggregate As New DataTable

            Importatore.dtParticelleCreaStruttura(DtParticelle)
            Importatore.dtParticelleCreaStruttura(dtPartielleAggregate)

            Dim DataInizio As Date = AGRODATAINIZIO
            Dim DataFine As Date = AGRODATAFINE

            Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            objImpost.AnnataAgraria(DataValidazione, DataInizio, DataFine, objParametri_Utenti)

            importatore.PopolaDT_Appezzamenti(piva,
                                              Fascicolo_Umbria,
                                              False,
                                              False,
                                              DataInizio,
                                              DataFine,
                                              DtParticelle,
                                              objParametri_Server,
                                              objParametri_Utenti,
                                              Num_Scheda,
                                              Utente_Username,
                                              Utente_Password,
                                              ProgressivoGIAS,
                                              Flag_ImportaAnagrafica)

            importatore.dtParticelleAggrega(False, False, False, DtParticelle, dtPartielleAggregate)


            Dim strAppezzamenti = JSON_DataTableAppezzamenti_Tabella(dtPartielleAggregate, DataInizio, DataFine)

            Dim scriviConRibaltamento As New AgronicaCoreAnagrafeBIZ.ImportazionePcFast

            Dim programmazione_cod As Integer = 0

            scriviConRibaltamento.Aggiorna_PcFast_PianoColtuale(piva,
                                                                Cuaa,
                                                                programmazione_cod,
                                                                DataInizio,
                                                                DataFine,
                                                                Num_Scheda,
                                                                DataValidazione,
                                                                enum_Planning_Fonte.AGEA_Coordinamento,
                                                                ProgressivoGIAS,
                                                                allegatoDocumentoCod,
                                                                False,
                                                                False,
                                                                objParametri_Server,
                                                                objParametri_Utenti,
                                                                0, str_Appezzamenti:=strAppezzamenti,
                                                                importatoAutomaticamente:=False)

        End If

    End Function

    Private Shared Function JSON_DataTableAppezzamenti_Tabella(ByRef DT_Appezzamenti As DataTable,
                                                               ByVal DataInizio As String, ByVal DataFine As String,
                                                               Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("chiave", "chiave", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("piva", "piva", "string")
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("sa_cod", "sa_cod", "number")
        c._hidden = True
        c._Editabile = True
        l.Add(c)
        c = New ColonneNome("appezza", "appezza", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("programmazione_entita_cod", "Programmazione_Entita_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("datoGis", "Gis", "String")
        c._FormatoParticolare = "<span class='fa fa-globe fa-2x'></span>"
        'c._RemoveHtmlEncode = True
        c._Filtrabile = False
        c._width = "60px"
        l.Add(c)

        c = New ColonneNome("app_nome", "App.", "string")
        c._Editabile = True
        c._obbligatorio = True
        c._Filtrabile = False
        c._width = "78px"
        l.Add(c)

        c = New ColonneNome("utilizzo_sup", "Sup. Utilizzo [ha] (SAU)", "number")
        c._Editabile = True
        c._obbligatorio = True
        c._Filtrabile = False
        c._width = "120px"
        c._formatNr = "n4"
        c._sum = True
        l.Add(c)

        c = New ColonneNome("macrouso_cod", "Macrouso_Cod", "string")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("macrouso_sup", "Macrouso_Sup", "string")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("utilizzo", "Utilizzo (Specie Vegetale)", "string")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("varieta", "Varietà", "string")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("veg_des", "Specie Vegetale", "string")
        c._Editabile = True
        'c._hidden = True
        c._width = "200px"
        l.Add(c)

        c = New ColonneNome("cul_des", "Varietà", "string")
        c._Editabile = True
        'c._hidden = True
        c._width = "200px"
        l.Add(c)

        c = New ColonneNome("grfi_des", "Finalità Produttiva", "string")
        c._Editabile = True
        'c._hidden = True
        c._width = "200px"
        l.Add(c)

        c = New ColonneNome("macrouso", "Macrouso", "string")
        c._Editabile = True
        'c._hidden = True
        c._width = "200px"
        l.Add(c)

        c = New ColonneNome("grva_des", "Tipologia Varietale", "string")
        c._hidden = True
        l.Add(c)

        ''GESTIONE CATASTO IN LINEA
        c = New ColonneNome("PROV", "PROV", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("prov_des", "PR.", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("COM", "COM", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("com_des", "Comune", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "150px"
        l.Add(c)

        c = New ColonneNome("sezione", "Sez.", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("foglio", "Fgl.", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("numero", "Numero", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "100px"
        l.Add(c)

        c = New ColonneNome("subalterno", "Sub.", "String")
        'c._Editabile = True
        'c._hidden = True
        c._width = "80px"
        l.Add(c)

        c = New ColonneNome("sa_nome", "Centro", "string")
        c._Editabile = True
        c._Filtrabile = False
        c._width = "100px"
        c._Filtrabile = True
        l.Add(c)

        c = New ColonneNome("validita_inizio", "Inizio Gestione Appezzamento", "date")
        c._Editabile = True
        c._obbligatorio = True
        c._width = "133px"
        c._valueDefault = DataInizio
        c._Filtrabile = True
        l.Add(c)

        c = New ColonneNome("validita_fine", "Fine Gestione Appezzamento", "date")
        c._Editabile = True
        c._obbligatorio = True
        c._width = "123px"
        c._valueDefault = DataFine
        c._Filtrabile = True
        l.Add(c)

        c = New ColonneNome("prov", "PROV", "string")
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("com", "COM", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("catasto", "Catasto (Provincia-Comune-Sezione-Foglio-Numero-Subalterno)", "string")
        c._FormatoParticolare = "#= GetValoreParticella(chiave, catasto)#"
        c._RemoveHtmlEncode = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("catasto_key", "Catasto_Key", "String")
        c._RemoveHtmlEncode = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("id_cod", "Id_Cod", "String")
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("veg_cod", "Veg_Cod", "String")
        c._hidden = True
        c._Editabile = True
        l.Add(c)
        c = New ColonneNome("cul_cod", "Cul_Cod", "String")
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("grfi_cod", "Grfi_Cod", "String")
        c._hidden = True
        c._Editabile = True
        l.Add(c)
        c = New ColonneNome("grva_cod", "Grva_Cod", "String")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("veg_cod_agea", "Veg_Cod_Agea", "String")
        c._hidden = True
        c._Editabile = True
        l.Add(c)
        c = New ColonneNome("cul_cod_agea", "Cul_Cod_Agea", "String")
        c._hidden = True
        c._Editabile = True
        l.Add(c)

        c = New ColonneNome("ribaltato", "ribaltato", "String")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("movimentato", "movimentato", "String")
        c._hidden = True
        l.Add(c)


        c = New ColonneNome("Cop_Cod", "Cop_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Cop_Des", "Cop_Des", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Lotto", "Lotto", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Resa", "Resa", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Num_Piante", "Num_Piante", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("TRA_Fila", "TRA_Fila", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("SU_Fila", "SU_Fila", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Validita_Inizio_Impianto", "Validita_Inizio_Impianto", "Date")
        c._Editabile = True
        'c._valueDefault = DataInizio
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("TipoZona", "TipoZona", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("TipoZona_Des", "Tipo Zona", "String")
        c._Editabile = True
        'c._hidden = True
        l.Add(c)
        c = New ColonneNome("MetodoProduzione_Cod", "MetodoProduzione_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("MetodoProduzione_Des", "Metodo Produzione", "String")
        c._Editabile = True
        'c._hidden = True
        l.Add(c)
        c = New ColonneNome("Unita_Vitata", "Unita_Vitata", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Veg_Cod_Agea", "Veg_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Veg_Des_Agea", "Veg_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Cul_Cod_Agea", "Cul_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Cul_Des_Agea", "Cul_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Uso_Cod_Agea", "Uso_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Uso_Des_Agea", "Uso_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Occupazione_Cod_Agea", "Occupazione_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Occupazione_Des_Agea", "Occupazione_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Destinazione_Cod_Agea", "Destinazione_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Destinazione_Des_Agea", "Destinazione_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Qualita_Cod_Agea", "Qualita_Cod_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)
        c = New ColonneNome("Qualita_Des_Agea", "Qualita_Des_Agea", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Gru_Cod", "Gru_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Dpi_Cod", "Dpi_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Reg_Cod", "Reg_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Regolamento_Concimazione_Cod", "Regolamento_Concimazione_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Disciplinare", "Disciplinare", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Flag_PubblicoPrivato", "Flag_PubblicoPrivato", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("id_tr", "id_tr", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("StatoImpianto_Cod", "StatoImpianto_Cod", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("N", "N", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("P", "P", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("K", "K", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Data_Semina", "Data_Semina", "Date")
        c._Editabile = True
        c._hidden = True
        'c._valueDefault = DataInizio
        l.Add(c)

        c = New ColonneNome("Data_Raccolta", "Data_Raccolta", "Date")
        c._Editabile = True
        c._hidden = True
        'c._valueDefault = DataInizio
        l.Add(c)

        c = New ColonneNome("Data_Fioritura", "Data_Fioritura", "Date")
        c._Editabile = True
        c._hidden = True
        'c._valueDefault = DataInizio
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente", "Coltura_Precedente", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente2", "Coltura_Precedente2", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente3", "Coltura_Precedente3", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente4", "Coltura_Precedente4", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Piano_Semina", "Piano_Semina", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Codice_Contratto", "Codice_Contratto", "String")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("unito", "unito", "number")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("frazionato", "frazionato", "number")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Codice_Fiscale_Tecnico", "Codice_Fiscale_Tecnico", "String")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("stato_ribaltamento", "stato_ribaltamento", "number")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("provenienza_fascicolo", "provenienza_fascicolo", "String")
        'c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("IAF", "IAF", "String")
        'c._Editabile = True
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("DistBZ_CorpiIdrici", "DistBZ_CorpiIdrici", "number")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("DistBZ_AreeResPub", "DistBZ_AreeResPub", "number")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("DistBZ_Allevamenti", "DistBZ_Allevamenti", "number")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("DistBZ_VegNatNonColt", "DistBZ_VegNatNonColt", "number")
        c._Editabile = True
        c._hidden = True
        l.Add(c)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = False

        Dim risp As String

        If stringaKendoRow = "" Then
            Dim strKendoRow As New StringBuilder
            AgronicaCoreDataProvider.JSON_DataTable.kendo_Rows(DT_Appezzamenti, l, strKendoRow)
            risp = strKendoRow.ToString
        Else
            risp = js.JSON_DataTable_Kendo(DT_Appezzamenti, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu,
                                           stringaKendoRow:=stringaKendoRow) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '
        End If

        Return risp

    End Function

End Class
