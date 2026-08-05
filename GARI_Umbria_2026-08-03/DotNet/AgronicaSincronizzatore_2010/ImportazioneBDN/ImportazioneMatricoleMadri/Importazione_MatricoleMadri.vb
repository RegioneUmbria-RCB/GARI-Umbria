Imports System.Xml
Imports AgronicaCoreDataProvider.ListExtensions
Imports System.Text
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreMapper
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreEntityFramework
Imports System.Web.UI.WebControls
Imports AgronicaCoreUtility
Imports AgronicaCoreModelsSTD.exceptions

Public Class Importazione_MatricoleMadri
    Dim objParametriServer As AgronicaCoreParametri
    Dim objParametriUtenti As AgronicaCoreParametri
    Dim objStalla_R As New AgronicaCoreAnagrafeDAL.Stalla_R
    Dim objStalla_Raggruppamenti_R As New AgronicaCoreAnagrafeDAL.Stalla_Raggruppamenti_R
    Dim obj_AttivitaZooToAgenda_w As New AttivitaZootecnicaToAgenda

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriUtenti As AgronicaCoreParametri)
        Me.objParametriServer = objParametriServer
        Me.objParametriUtenti = objParametriUtenti
    End Sub
    Dim objLog As New AgronicaCoreDataProvider.LogProvider

    Public Function importaCapi(ByVal PivaSelezionata As String,
                                ByVal StringaConnessione As String,
                                         ByVal Utente_Username As String,
                                         ByVal Utente_Password As String,
                                         ByVal ProgressivoGIAS As Integer,
                                         ByVal CodiceChiaveCliente As Integer,
                                         ByVal LogDirectory As String,
                                         ByVal LogFileName As String,
                                         ByRef Messaggio As String
                                         ) As Boolean
        Dim Flag_Risultato As Boolean = False
        Dim NomeRoutine As String = "Importa_Dati"

        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = objParametriServer.LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        Try
            objLog.Scrivi_LOG(objParametriServer,
                   NomeRoutine,
                   "Inizio importazione",
                   CustomLOGParams:=customLOGParams)

            Dim DTCapiTotali As New DataTable

            CreaDT(StringaConnessione,
                   "",
                   DTCapiTotali)

            Dim DTStalle As DataTable = DTCapiTotali.DefaultView.ToTable(True, "Allevamento")
            Dim numCapiAggiornati = 0
            For Each rowStalla In DTStalle.Rows
                Try
                    Dim Bdn_codice_azienda = rowStalla("Allevamento").ToString
                    Messaggio &= " <h5>Stalla " & Bdn_codice_azienda & " </h5>"
                    Dim expandoCapi = DTCapiTotali.Select(" Allevamento = '" & Bdn_codice_azienda & "' ").CopyToDataTable.ToExpandoObject.Where(Function(capo)
                                                                                                                                                    Return (Not IsDBNull(capo("Madre"))) AndAlso (Not IsDBNull(capo("Matricola"))) AndAlso (Not IsDBNull(capo("Allevamento")))
                                                                                                                                                End Function)
                    'Lettura capi in DB 
                    Dim Raggruppamento_Cod As Integer = 0
                    Dim sa_cod As String = ""
                    Dim STA_NUM As String = ""
                    Dim piva As String = ""

                    GetDatiPerQuery(PivaSelezionata,
                                Bdn_codice_azienda,
                                piva,
                                sa_cod,
                                STA_NUM,
                                Raggruppamento_Cod)

                    Dim visibilita As Boolean = ControlloVisibilitaAziende(piva, sa_cod, STA_NUM)

                    If Not visibilita Then
                        Messaggio &= " Non è possibile sincronizzare la stalla perché non si dispone dei permessi necessari"

                    Else
                        Dim obj_ZooAnimali_R As New Zoo_Animali
                        Dim dictAnimaliPresenti = obj_ZooAnimali_R.Leggi_Giacenze(piva, sa_cod,
                                                                                  STA_NUM, 0,
                                                                                  0, Date.Now,
                                                                                  objParametriServer).ToExpandoObject.ToDictionary(Of String, String) _
                                                                                  (Function(row) row("Matricola"), Function(row) row("Cod_Progetto"))


                        Dim capiDaAggiornane = expandoCapi.Select(Function(capo)
                                                                      sistemaMatricola(capo("Matricola"))
                                                                      sistemaMatricola(capo("Madre"))
                                                                      Return capo
                                                                  End Function).Where(Function(row) dictAnimaliPresenti.ContainsKey(row("Matricola"))).
                                                                  ToDictionary(Of String, Object)(Function(row) dictAnimaliPresenti(row("Matricola").ToString).ToString, Function(row) row)


                        Dim matricoleAggiornate = AggiornaCapi(capiDaAggiornane,
                               sa_cod,
                               STA_NUM,
                               Raggruppamento_Cod,
                               piva,
                               objLog,
                               LogDirectory,
                               LogFileName,
                               Messaggio,
                               customLOGParams)

                        Messaggio &= "<p> Matricole nel Excel: " & expandoCapi.Count.ToString & "  Matricole Aggiornate " & matricoleAggiornate.Count.ToString
                        If capiDaAggiornane.Count < expandoCapi.Count Then
                            Messaggio &= "</br>  Matricole Non presenti in stalla(" & (expandoCapi.Count - capiDaAggiornane.Count) & "): <ul> "
                            Messaggio &= expandoCapi.Select(Of String)(Function(capo) capo("Matricola")) _
                                .Where(Function(matricola) Not matricoleAggiornate.Contains(matricola)) _
                                .Select(Of String)(Function(mat) "<il> " & mat & "</il></br>") _
                                .Aggregate(Function(mat1, mat2) mat1 & mat2)

                        End If
                        Messaggio &= "</p></br>"
                        numCapiAggiornati += matricoleAggiornate.Count
                    End If

                Catch ex As Exception
                    Messaggio &= "<p>" & ex.Message & "</p>"
                End Try
            Next

            Flag_Risultato = True

        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer,
                   NomeRoutine,
                   "Errore scrittura dati: " & ex.Message,
                   CustomLOGParams:=customLOGParams)
            Messaggio = ex.Message
            Flag_Risultato = False

        End Try

        objLog.Scrivi_LOG(objParametriServer,
                   NomeRoutine,
                   "Fine importazione",
                   CustomLOGParams:=customLOGParams)

        Return Flag_Risultato

    End Function

    Private Function sistemaMatricola(ByRef matricola As String) As String
        Const DEFAULT_PREFIX = "FR"
        If IsNumeric(matricola.Substring(0, 1)) Then
            matricola = DEFAULT_PREFIX & matricola
        End If
        Return matricola
    End Function

    'Estrazione del Raggruppamento_Cod
    Private Sub GetDatiPerQuery(ByVal PivaSelezionata As String,
                               ByVal BDN_Codice_Azienda As String,
                               ByRef piva As String,
                               ByRef sa_cod As String,
                               ByRef sta_num As String,
                               ByRef Raggruppamento_Cod As Integer)

        Dim DTStalla As New DataTable
        Dim DTRaggruppamenti As DataTable
        Dim PivaSuperUser As String = objParametriServer.PivaSuperUser

        'Estrazione codice azienda BDN dalla datatable dei capi che stiamo importando

        DTStalla = objStalla_R.Leggi("", 0, 0, 1, "Stalla.BDN_Codice_Azienda = '" & BDN_Codice_Azienda & "' ", "", Me.objParametriServer)

        If DTStalla.Rows.Count = 0 Then
            Throw New GiasException("Stalla " & BDN_Codice_Azienda & " non presente in GIAS")
        ElseIf DTStalla.Rows.Count > 1 Then
            Dim drStallaF = DTStalla.Select(" Piva = '" & PivaSelezionata & "' ")
            If drStallaF.Length = 0 Then
                Throw New GiasException("Al codice Azienda " & BDN_Codice_Azienda & " corrispondono più stalle su GIAS")
            ElseIf drStallaF.Length > 1 Then
                Throw New GiasException("Al codice Azienda " & BDN_Codice_Azienda & " corrispondono più stalle su GIAS")
            ElseIf drStallaF.Length = 1 Then
                DTStalla = drStallaF.CopyToDataTable
            End If
        End If
        'Estrazione Dati da DTStalla
        Dim DTStallaRow As DataRow = DTStalla.Select("").FirstOrDefault()
        piva = DTStallaRow.Item(0)
        sa_cod = Int(DTStallaRow.Item(1))
        sta_num = Int(DTStallaRow.Item(2))

        DTRaggruppamenti = objStalla_Raggruppamenti_R.Leggi_x_anagrafica(PivaSuperUser, piva, sa_cod, sta_num, Raggruppamento_Cod, " Stalla_Raggruppamenti.flag_bdn = 1 ", "", objParametriServer)
        If DTRaggruppamenti.Rows.Count = 0 Then
            Throw New GiasException("Stalla " & BDN_Codice_Azienda & " senza un raggruppamento correttamente configurato per l'importazione")
        End If
        Dim row = DTRaggruppamenti.Select("").FirstOrDefault()
        Raggruppamento_Cod = row.Item(7)

    End Sub

    ''' <summary>
    ''' Controllo della visibilità utente sulle stalle delle diverse aziende
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="saCod"></param>
    ''' <param name="staNum"></param>
    ''' <returns></returns>
    Private Function ControlloVisibilitaAziende(ByVal piva As String, ByVal saCod As Integer, ByVal staNum As Integer) As Boolean
        Dim visibilita As Boolean = False

        Dim classFiltrone As New AgronicaCoreUtility.Filtrone
        Dim classJoin As New JoinFiltrone
        classJoin.bCentriAziendali = True
        classJoin.bGerarchiaImprese = True
        Dim dtImpresexCentri As DataTable = classFiltrone.CreaDTFiltrone(objParametriServer, "",
                                                                         enum_TipoSelect_FiltroneSuperNova.CentriAziendali,
                                                                         "", classJoin)

        If Not IsNothing(dtImpresexCentri) AndAlso dtImpresexCentri.Rows.Count > 0 Then
            Dim listaImprese = dtImpresexCentri.ToExpandoObject.ToList
            Dim listaAziende As List(Of String) = listaImprese.Select(Of String)(Function(row) row("PIVA")).ToList

            'visibilità su Azienda
            If listaAziende.Contains(piva) Then
                Dim listaCentri As List(Of Integer) =
                    listaImprese.Where(Function(row) row("PIVA") = piva).Select(Of Integer)(Function(row) row("sa_cod")).ToList

                'visibilità su AziendaxCentro
                If listaCentri.Contains(saCod) Then
                    visibilita = True
                End If
            End If
        End If

        Return visibilita

    End Function

    'Crea DataTable importando i dati dal file excel
    Private Sub CreaDT(ByVal StringaConnessione As String,
                      ByRef Messaggio As String,
                      ByRef DTCapi As DataTable)
        Try
            Dim ds As New DataSet
            Dim MyConnection As New System.Data.OleDb.OleDbConnection(StringaConnessione)

            Dim counter As Integer = 0

            MyConnection.Open()

            Dim dtSheet = MyConnection.GetSchema("Tables")
            Dim firstSheet = dtSheet.Rows(0)("TABLE_NAME").ToString()
            Dim da As New System.Data.OleDb.OleDbDataAdapter("select * from [" + firstSheet + "]", MyConnection)

            da.Fill(ds, "fileXls")

            ds.Tables(0).AcceptChanges()
            ds.Tables(0).AcceptChanges()
            MyConnection.Close()
            DTCapi = ds.Tables(0)
            DTCapi.Columns(0).ColumnName = "Matricola"
            DTCapi.Columns(1).ColumnName = "Madre"
            DTCapi.Columns(2).ColumnName = "Allevamento"



        Catch ex As Exception
            Messaggio = "Errore all'apertura del file excel: " & ex.Message
        End Try

    End Sub

    ''Caricamento dei capi su DB
    'Private Sub CaricaCapi(ByVal DTCapiSenzaDuplicati As DataTable,
    '                      ByRef sa_cod As Integer,
    '                      ByRef sta_num As String,
    '                      ByRef Raggruppamento_Cod As Integer,
    '                      ByVal piva As String,
    '                      ByVal objLog As AgronicaCoreDataProvider.LogProvider,
    '                      LogDirectory As String,
    '                      LogFileName As String,
    '                      ByRef Messaggio As String)

    '    Dim LSTCapiSenzaDuplicati = DTCapiSenzaDuplicati.ToExpandoObject.ToList

    '    Dim LSTDateIngresso = From cp In LSTCapiSenzaDuplicati
    '                          Select cp.Item("F10") Distinct.ToList()

    '    Dim objAttivita As New attivita.Attivita
    '    Dim centroAzienda_Cod As Integer = 0 ' Controllare se effettivamente 0 è corretto
    '    objAttivita.fine = AGRODATAFINE
    '    objAttivita.job = New attivita.Zootecnia(LAVCOD_ACQUISTO_ANIMALI, "")
    '    objAttivita.centroAziendale = New anagrafiche.CentroAziendale With {
    '        .primaryKey = New anagrafiche.CentroAziendale.PK(centroAzienda_Cod, piva)
    '    }

    '    Dim LSTCapoAnimaleCDC As New List(Of attivita.centri_di_costo.CentroDiCosto)

    '    For Each ingresso In LSTDateIngresso
    '        Try
    '            Dim LSTCapiDaCaricareDCFiltrati = LSTCapiSenzaDuplicati.Where(Function(x)
    '                                                                              Return x.Item("F10") = ingresso
    '                                                                          End Function).ToList

    '            If LSTCapiDaCaricareDCFiltrati.Count = 0 Then
    '                Continue For
    '            End If

    '            objLog.Scrivi_LOG(LogDirectory,
    '                                  LogFileName,
    '                                  objParametriServer.LogDescrizioneUtente,
    '                                  System.Reflection.MethodBase.GetCurrentMethod().Name,
    '                                  "Inizio carico " & LSTCapiDaCaricareDCFiltrati.Count & " capi in data " & CDate(ingresso).ToShortDateString())

    '            For Each capo In LSTCapiDaCaricareDCFiltrati
    '                Dim codiceCapo As String = capo("Matricola")
    '                Try

    '                    Dim codAziendaNascita As String = capo("Stalla_Nascita")
    '                    Dim sesso As String = capo("F3")
    '                    Dim dataNascita As String = capo("F4")
    '                    Dim codRazza As Integer = RicavaRazzaCapoAnimale(capo("F5"))
    '                    Dim codRazzaPadre As Integer = RicavaRazzaCapoAnimale(capo("F6"))
    '                    Dim codRazzaMadre As Integer = RicavaRazzaCapoAnimale(capo("F7"))
    '                    Dim numCertificato As String = capo("F14")
    '                    Dim genere As Integer = 1 'Placeholder
    '                    Dim specie As Integer = 1 'Placeholder

    '                    Dim matricolaMadre As String = ""
    '                    If Not IsDBNull(capo("F20")) Then
    '                        Dim addFRPrefix As String = "FR"
    '                        If IsNumeric(CStr(capo("F20")).Substring(0, 2)) Then
    '                            matricolaMadre = addFRPrefix & capo("F20")
    '                        Else
    '                            matricolaMadre = capo("F20")
    '                        End If
    '                    End If



    '                    Dim OBJCapoAnimaleCDC = CreaCapoAnimale(piva,
    '                                                            sa_cod,
    '                                                            codiceCapo,
    '                                                            dataNascita,
    '                                                            sesso,
    '                                                            codRazza,
    '                                                            ingresso,
    '                                                            numCertificato,
    '                                                            Raggruppamento_Cod,
    '                                                            genere,
    '                                                            specie,
    '                                                            codRazzaMadre,
    '                                                            codRazzaPadre,
    '                                                            codAziendaNascita,
    '                                                            matricolaMadre)

    '                    LSTCapoAnimaleCDC.Add(OBJCapoAnimaleCDC)
    '                Catch ex As Exception
    '                    Messaggio &= "<p> Errore matricola " & codiceCapo & ": " & ex.Message & "</p>"
    '                End Try
    '            Next

    '            objAttivita.inizio = CDate(ingresso)
    '            objAttivita.centriDiCosto = LSTCapoAnimaleCDC

    '            Dim Id_Agenda As Integer = obj_AttivitaZooToAgenda_w.ScriviAttivitaZootecnicaToAgenda(objAttivita,
    '                                                                                                  Me.objParametriServer)

    '            Dim LSTMatricole = (From a As attivita.centri_di_costo.CapoAnimaleCDC In LSTCapoAnimaleCDC Select a.capoAnimale.matricola).ToList

    '            Messaggio &= "<p>" & String.Join(",", LSTMatricole) & "</p>"

    '            objLog.Scrivi_LOG(LogDirectory,
    '                                  LogFileName,
    '                                  objParametriServer.LogDescrizioneUtente,
    '                                  System.Reflection.MethodBase.GetCurrentMethod().Name,
    '                                  "Fine carico " & LSTCapiDaCaricareDCFiltrati.Count & " capi in data " & CDate(ingresso).ToShortDateString() & ": " & String.Join(",", LSTMatricole))

    '        Catch ex As Exception
    '            Dim msgEx = ex.Message
    '            If ex.InnerException IsNot Nothing Then
    '                msgEx &= " Inner Exception:" & ex.InnerException.Message
    '            End If
    '            Messaggio &= ex.Message & vbCrLf
    '            objLog.Scrivi_LOG(LogDirectory,
    '                              LogFileName,
    '                              objParametriServer.LogDescrizioneUtente,
    '                              System.Reflection.MethodBase.GetCurrentMethod().Name,
    '                              "Errore carico in data " & CDate(ingresso).ToShortDateString() & vbCrLf &
    '                              " Errore:" & msgEx)

    '        End Try
    '    Next

    'End Sub


    Private Function AggiornaCapi(ByVal capiDaAggiornare As Dictionary(Of String, Object),
                          ByRef sa_cod As Integer,
                          ByRef sta_num As String,
                          ByRef Raggruppamento_Cod As Integer,
                          ByVal piva As String,
                          ByVal objLog As AgronicaCoreDataProvider.LogProvider,
                          LogDirectory As String,
                          LogFileName As String,
                          ByRef Messaggio As String,
                          ByVal customLOGParams As CustomLOGParams) As List(Of String)


        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Dim listaMatricoleAggiornate As List(Of String) = New List(Of String)
        Try


            GiasContext.Zoo_Animali.Where(Function(row) capiDaAggiornare.Keys.Contains(row.Cod_Progetto)).ToList.ForEach(Sub(a)
                                                                                                                             listaMatricoleAggiornate.Add(a.Matricola)
                                                                                                                             a.MAT_MADRE = capiDaAggiornare(a.Cod_Progetto).Madre
                                                                                                                         End Sub)

            GiasContext.SaveChanges()
            GiasContext.Core.AcceptAllChanges()


        Catch ex As Exception
            Dim msgEx = ex.Message
            If ex.InnerException IsNot Nothing Then
                msgEx &= " Inner Exception:" & ex.InnerException.Message
            End If
            Messaggio &= ex.Message & vbCrLf
            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Errore carico in AggiornaCapi " & vbCrLf &
                              " Errore:" & msgEx,
                              CustomLOGParams:=customLOGParams)
        Finally
            GiasContext.Dispose()

        End Try
        Return listaMatricoleAggiornate
    End Function

    'Creazione di capo animale da caricare sul DB
    Private Function CreaCapoAnimale(ByVal piva As String,
                                    ByVal sa_cod As Integer,
                                    ByVal matricolaCapo As String,
                                    ByVal dataNascita As String,
                                    ByVal sesso As String,
                                    ByVal codRazza As Integer,
                                    ByVal ingresso As String,
                                    ByVal numCertificato As String,
                                    ByVal Raggruppamento_Cod As Integer,
                                    ByVal genere As Integer,
                                    ByVal specie As Integer,
                                    ByVal codRazzaMadre As Integer,
                                    ByVal codRazzaPadre As Integer,
                                    ByVal codAziendaNascita As String,
                                    ByVal MatricolaMadre As String) As attivita.centri_di_costo.CapoAnimaleCDC
        ' Togliere logdirectory e logfilename

        Dim CapoCDC As New attivita.centri_di_costo.CapoAnimaleCDC()

        Dim objImprese As New Imprese_Read
        Dim DTimprese As DataTable
        DTimprese = objImprese.Leggi_x_anagrafica(piva,
                                                "",
                                                "",
                                                Me.objParametriServer)

        Dim rowImprese As DataRow = DTimprese.Select("").FirstOrDefault

        Dim codDetentore As String = rowImprese.Item("Codice_Cuaa")

        Dim objCapo As New CapoAnimale With {
            .partitaIva = piva,
            .matricola = matricolaCapo,
            .sesso = sesso,
            .codice = 0,
            .nome = "",
            .collare = "",
            .lottoFornitore = "",
            .dataNascita = dataNascita,
            .numCertificato = numCertificato,
            .codiceFiscaleDetentore = codDetentore,
            .codiceFiscaleProprietario = "",
            .codiceAziendaNascita = codAziendaNascita,
            .indirizzoProd = New metaschema.IndirizzoProduttivo(0),
            .categoria = New metaschema.Categoria(0),
            .tipologia = New metaschema.TipologiaCapoAnimale(1),
            .metodoProduzione = New metaschema.MetodoProduzione(1),
            .idCapo_BDN = 0,
            .genere = New metaschema.Genere(genere),
            .specie = New metaschema.utilizzi.Specie(specie),
            .razza = New metaschema.Razza(codRazza),
            .esercizi = New List(Of AgronicaCoreModelsSTD.anagrafiche.EsercizioCapoAnimale),
            .statiAccrescimento = New List(Of AgronicaCoreModelsSTD.anagrafiche.StatoAccrescimento),
            .fornitore = New AgronicaCoreModelsSTD.anagrafiche.Contatto With {
                        .primaryKey = New AgronicaCoreModelsSTD.anagrafiche.Contatto.PK With {
                            .partitaIva = ""
                        }
                    },
            .validita = New anagrafiche.IntervalloTemporale With {
                .inizio = ingresso,
                .fine = AGRODATAFINE
                },
            .validitaConversione = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale With {
                    .inizio = AGRODATAINIZIO,
                    .fine = AGRODATAFINE
                },
            .madre = New CapoAnimale With {
                .razza = New metaschema.Razza(codRazzaMadre),
                .codice = 0,
                .matricola = MatricolaMadre
                },
            .padre = New CapoAnimale With {
                .razza = New metaschema.Razza(codRazzaPadre),
                .codice = 0,
                .matricola = ""
                }
        }
        CreaDistinte(objCapo)

        CreaStatiAccrescimento(objCapo)

        CapoCDC.codice = New attivita.centri_di_costo.CentroDiCosto.CodeType(sa_cod)
        CapoCDC.capoAnimale = objCapo
        CapoCDC.sottogruppoStalla_ingresso = New SottogruppoStalla
        CapoCDC.sottogruppoStalla_ingresso.codice = Raggruppamento_Cod

        CapoCDC.sottogruppoStalla_uscita = New SottogruppoStalla
        CapoCDC.sottogruppoStalla_uscita.codice = Raggruppamento_Cod

        If CapoCDC.capoAnimale.validita.inizio <> ingresso Then
            Dim a = 0
        End If

        Return CapoCDC
    End Function

    Private Function RicavaRazzaCapoAnimale(ByVal codRazza As Integer)
        Dim DTCodificaRazzeAnimaliFRtoBDN As DataTable

        '---------------Nuova-Query-StringBuilder---------------
        Dim DataProvider As New DataProvider
        Dim Stb As New System.Text.StringBuilder
        Dim NomeRoutine As String = "estrazioneCodRazzaBDNDaCodRazzaFR" ' Da nominare

        Stb.Length = 0
        Stb.AppendLine("SELECT *")
        Stb.AppendLine("FROM Cac_Codifica_InfoAggiuntive ")
        Stb.AppendLine("WHERE InfoAgg_Cod = 9 AND Argomento_Cod = " & codRazza & "")

        DTCodificaRazzeAnimaliFRtoBDN = DataProvider.EseguiQuery_Lettura(objParametriServer, Stb.ToString, NomeRoutine)
        '---------------Fine-Query-StringBuilder---------------

        Dim rowCodificaRazzeAnimliFRtoBDN As DataRow = DTCodificaRazzeAnimaliFRtoBDN.Select("").FirstOrDefault

        If IsNothing(rowCodificaRazzeAnimliFRtoBDN) Then
            Throw New GiasException("Errore! Razza ParmaFrance " & codRazza & " non mappata in GIAS.") 'INC non esiste nel db quindi ritorna nothinge e da errore
        End If

        Dim razzaBDN As String = rowCodificaRazzeAnimliFRtoBDN.Item("TestoAux_1")
        Dim obj_RazzeAnimali_R As New AgronicaCoreMetaSchemaDAL.Codifica_BDN_RazzeAnimali
        Dim DTCodificaRazzeAnimaliBDNtoAgronica As New DataTable

        DTCodificaRazzeAnimaliBDNtoAgronica = obj_RazzeAnimali_R.leggi(Me.objParametriServer,
                                                            "",
                                                            "",
                                                            "",
                                                            "",
                                                            "",
                                                            AgronicaCoreDataProvider.TipiEnumerativi.enum_Esportazioni_Sistema_Cod.BDN) 'BDN enum da cambiare? Creare?

        Dim rowCodificaRazzeAnimliBDNtoAgronica As DataRow = DTCodificaRazzeAnimaliBDNtoAgronica.Select("CODICE = '" + razzaBDN + "'").FirstOrDefault

        If IsNothing(rowCodificaRazzeAnimliBDNtoAgronica) Then
            Throw New GiasException("Errore! Razza BDN " & razzaBDN & " non mappata in GIAS.") 'INC non esiste nel db quindi ritorna nothinge e da errore
        End If

        Dim razzaAgronica = rowCodificaRazzeAnimliBDNtoAgronica.Item("RAZ_COD")

        Return razzaAgronica
    End Function

    Private Sub CreaDistinte(ByRef objCapo As CapoAnimale)

        Dim objEsercizio As New AgronicaCoreModelsSTD.anagrafiche.EsercizioCapoAnimale
        objEsercizio.codice_capo_animale = objCapo.codice
        objEsercizio.validita = New IntervalloTemporale

        objEsercizio.validita.inizio = objCapo.validita.inizio
        objEsercizio.validita.fine = objCapo.validita.fine

        objEsercizio.progettoNome = ""

        objCapo.esercizi.Add(objEsercizio)

    End Sub

    Private Sub CreaStatiAccrescimento(ByRef objCapo As CapoAnimale)
        Dim Piva As String = objCapo.partitaIva
        Dim Gen_Cod As Integer = objCapo.genere.codice
        Dim Spe_Cod As Integer = objCapo.specie.codice
        Dim Tipo_Cod As Integer = objCapo.tipologia.codice
        Dim Zoo_Animali_Lista_Stati_Accrescimento As List(Of AgronicaCoreEntityFramework_POCO.Zoo_Animali_Lista_Stati_Accrescimento)
        Dim GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(Me.objParametriServer.StringaConnessione)

        GiasContext = New Gias_DeveloperServer_Entities(EFConnString)

        Zoo_Animali_Lista_Stati_Accrescimento = (From a In GiasContext.Zoo_Animali_Lista_Stati_Accrescimento Select a).ToList()

        'Lettura degli stati di accrescimento da DB
        Dim StatiAccrescimento = From zan In Zoo_Animali_Lista_Stati_Accrescimento
                                 Where (zan.PIVA = Piva Or zan.Sa_Cod = -1) And zan.GEN_COD = Gen_Cod And zan.SPE_COD = Spe_Cod And zan.TIPO_COD = Tipo_Cod
                                 Select zan
                                 Order By zan.Giorno_Da

        If IsNothing(StatiAccrescimento) OrElse StatiAccrescimento.Count = 0 Then
            Throw New GiasException("Errore! Non esistono stati di accrescimento per questo capo.")
        End If

        For Each sa In StatiAccrescimento
            Dim objStatoAccr As New StatoAccrescimento

            '-------------------------------------------------------------------------------------------------------
            'Aggiunge alla data di nascita i giorni di durata del periodo dello stato di accrescimento
            '-------------------------------------------------------------------------------------------------------

            objStatoAccr.validita = New IntervalloTemporale

            'VALIDITA_INIZIO
            If IsNothing(sa.Giorno_Da) Then
                objStatoAccr.validita.inizio = objCapo.dataNascita
            Else
                objStatoAccr.validita.inizio = objCapo.dataNascita.AddDays(sa.Giorno_Da)
            End If

            'VALIDITA_FINE
            If IsNothing(sa.Giorno_A) Then
                objStatoAccr.validita.fine = AGRODATAFINE
            Else
                objStatoAccr.validita.fine = objCapo.dataNascita.AddDays(sa.Giorno_A)
            End If

            '-------------------------------------------------------------------------------------------------------

            objStatoAccr.codice = sa.STATO_COD
            objStatoAccr.descrizione = sa.Stato_Des

            objCapo.statiAccrescimento.Add(objStatoAccr)
        Next

    End Sub

End Class