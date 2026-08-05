Imports AgronicaSHPWrapper
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieBIZ

Public Class EsportazioneBordoMacchina

    Public Function Esportazione(inData As EsportazioneBordoMacchina_EsportazioneIn, ByRef idFileToUpload As Integer) As RispostaStandard


        Dim rval As New RispostaStandard

        Try

            inData.exportfileBasePath = inData.exportfileBasePath & "\" & inData.NomeFileFinale.Split(".")(0)

            Dim folderExists As Boolean
            folderExists = My.Computer.FileSystem.DirectoryExists(inData.exportfileBasePath)
            If Not folderExists Then
                My.Computer.FileSystem.CreateDirectory(inData.exportfileBasePath)
            End If

            Select Case inData.TipoEsportazioneBordoMacchina

                Case enum_BordoMacchinaFormati.TrimbleEZGuide_250_500_750
                    'da gestire, al momento si trova il tutto in libreria: AgronicaEZGuide_500_250Exporter

                Case Else

                    'tutti gli altri formati seguono al momento questa procedura (deriva da John Deere)

                    'da: https://developer-portal.deere.com/#/myJohnDeereAPI/%2Fdocumentation%2Fmyjohndeere%2FfilesGuide.htm
                    'The following outlines the suggested naming pattern for compressed zip files.
                    '<Client>_<Farm>_<Field>_<Product>.zip
                    'Rx (folder)
                    '<Client>_<Farm>_<Field>_<Product>.shp
                    '<Client>_<Farm>_<Field>_<Product>.shx
                    '<Client>_<Farm>_<Field>_<Product>.dbf

                    Dim A2JD As New Agronica2JohnDeere.JDeereFilesWrapper


                    ''lettura dei dati GIAS in base a fitri passati
                    'LetturaDatiGiasSuFiltriPassati(inData)

                    Dim dt As DataTable = RecuperaElencoMacchineOperatori(inData)

                    'Riporto dei dati GIAS in strutture Dati John Deere
                    RiportoDatiGiasSuStruttureJohnDeere(dt, inData, A2JD)

                    ''Lettura dei dati John Deere
                    'LetturaDatiJD()

                    'Scrittura dei file di Setup via Framework Adapt
                    ScritturaFileSetupViaAdapt(inData, A2JD)

                    'scrittura dello shapefile a corredo
                    ScritturaShapeMappaPrescrizione(dt, inData)

                    'Scrittura del file Zip
                    ScriviFileZipFinale(inData)


                    'Verificare se ciò è sufficiente.
                    inData.Piva = dt.Rows(0)("piva")

                    'Riporto del file Zip su Database (decommentare x dev)
                    RiportoFileZipSuDB("", inData, idFileToUpload)

                    rval.RispostaOK = True
            End Select


        Catch ex As Exception
            rval.RispostaOK = False
            rval.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return rval

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="inData"></param>
    ''' <returns>True se si scrive su db</returns>
    Private Function RiportoFileZipSuDBLeggiImpostazioneSalvataggioFiles(inData As EsportazioneBordoMacchina_EsportazioneIn) As Boolean

        Dim xLetturaImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim dt As DataTable =
            xLetturaImpostazioni.Leggi(enum_Impostazioni_Utenti.SUPERUSER_DOCUMENTALE_SALVA_ALLEGATO_SU_DB, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", inData.objParametri_Utenti)

        Dim rval As Boolean
        If dt.Rows.Count > 0 Then
            rval = dt.Rows(0)("Impostazione_Valore_1")
        Else
            rval = True
        End If

        Return rval

    End Function
    Private Sub RiportoFileZipSuDB(jdFileName As String, inData As EsportazioneBordoMacchina_EsportazioneIn, ByRef idFileToUpload As Integer)


        Dim messaggioErrore As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim rval As New AgronicaCoreVarieBIZ.RispostaStandard

        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                FlagConnessioneLocale,
                FlagTransazioneLocale,
                inData.objParametri_Server)


            'scrivo l'allegato in GIAS.
            Dim FullFileNameZip As String =
                inData.exportfileBasePath & ".zip"

            Dim Allegati_Documenti_NomeFile As String = FullFileNameZip

            Dim fileContents As Byte() = Nothing

            Dim ScriviFileSuDB As Boolean =
                RiportoFileZipSuDBLeggiImpostazioneSalvataggioFiles(inData)

            If ScriviFileSuDB Then
                fileContents = My.Computer.FileSystem.ReadAllBytes(FullFileNameZip)
                Dim vFile1 As String() = Allegati_Documenti_NomeFile.Split("\")
                Allegati_Documenti_NomeFile = vFile1(vFile1.Length - 1)
            End If

            Dim ScriviAllegatoDB As New AgronicaCoreAnagrafeBIZ.Allegati_Documenti_W
            Dim OUTPUT_Allegati_Documenti_Cod As Integer

            Dim Desc As String = "File Bordo Macchina"

            Dim rvalAllegati As RispostaStandard =
            ScriviAllegatoDB.PrecisionFarmingScriviSuAllegati(
                enum_CategorieDocumenti.PrecisionFarming_FileBordoMacchina,
                Desc,
                Allegati_Documenti_NomeFile,
                inData.objParametri_Server,
                inData.Piva,
                inData.Sa_Cod,
                inData.Appezza,
                inData.Ricetta_Operazione_Cod,
                inData.ID_Imp,
                "",
                fileContents,
                "",
                OUTPUT_Allegati_Documenti_Cod
             )


            If Not rvalAllegati.RispostaOK Then
                Throw New Exception(rvalAllegati.Errore)
            End If

            'creo un legame sulle tabelle di frontiera JD


            Dim curjDeereDataModel_Files As New AgronicaCoreEntityFramework_POCO.jDeereDataModel_Files

            Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

            Dim idSeqFile As Integer =
                ObjSequenze.NuovoId_Tabella("jDeereDataModel_Files", 0, 2000000000, inData.objParametri_Server)

            Dim anagDAl As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Organization_R
            Dim orgIDGias As Integer = anagDAl.LeggiViaGUID(inData.jDeereDataModelCFG.orgID, inData.objParametri_Server).ID


            curjDeereDataModel_Files.ID = idSeqFile
            curjDeereDataModel_Files.Name = inData.NomeFileFinale
            curjDeereDataModel_Files.delayProcessing = 0
            curjDeereDataModel_Files.guid = "AGR-Allegato-" & OUTPUT_Allegati_Documenti_Cod
            curjDeereDataModel_Files.OrganizationID = orgIDGias

            curjDeereDataModel_Files.Validita_Inizio = AGRODATAINIZIO
            curjDeereDataModel_Files.Validita_Fine = AGRODATAFINE

            curjDeereDataModel_Files.Data_Creazione = Date.Now
            curjDeereDataModel_Files.Username_Creazione = inData.objParametri_Server.UsernameOperazione
            curjDeereDataModel_Files.Data_Modifica = Date.Now
            curjDeereDataModel_Files.Username_Modifica = inData.objParametri_Server.UsernameOperazione
            curjDeereDataModel_Files.inviato = 0

            Dim EFArrayToInsert As New ArrayList
            EFArrayToInsert.Add(curjDeereDataModel_Files)

            Dim jDFileCtr As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_Files_W

            Dim rvalFileJD As String =
                jDFileCtr.Aggiorna_jDeereDataModel_Files(EFArrayToInsert, New ArrayList, New ArrayList, inData.objParametri_Server)

            If Not String.IsNullOrEmpty(rvalFileJD) Then
                Throw New Exception(rvalFileJD)
            End If

            Dim idSedEntita As Integer =
                ObjSequenze.NuovoId_Tabella("jDeereDataModel_EntitaGIAS", 0, 2000000000, inData.objParametri_Server)

            Dim associazioneJD As New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGIAS_W
            associazioneJD.ScriviAssociazioneGIASJD(
                enum_TipoEntitaJohnDeere.File,
                idSedEntita,
                "",
                0,
                0,
                OUTPUT_Allegati_Documenti_Cod,
                idSeqFile,
                AGRODATAINIZIO,
                AGRODATAFINE,
                inData.objParametri_Server
            )

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la transazione e la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, inData.objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            idFileToUpload = idSeqFile

            rval.RispostaOK = True
            rval.RispostaStringa = ""

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not inData.objParametri_Server.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, inData.objParametri_Server)

            End If

            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)


            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then


                Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio += "" & vbCrLf
                Messaggio += messaggioErrore
                Messaggio += "" & vbCrLf
                Messaggio += "Ritentare il salvataggio dopo la correzione ..."

            End If

            rval.RispostaOK = False
            rval.Errore = Messaggio

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, inData.objParametri_Server)

        End Try


    End Sub

    Private Sub ScriviFileZipFinale(inData As EsportazioneBordoMacchina_EsportazioneIn)
        AgronicaCoreUtility.AgroZip.ZipAFolder(inData.exportfileBasePath & ".zip", inData.exportfileBasePath)
    End Sub

    Private Sub ScritturaShapeMappaPrescrizione(dtOperazioniRicette As DataTable, inData As EsportazioneBordoMacchina_EsportazioneIn)

        Dim LeggiGIS As New AgronicaCoreGisDAL.GIS_Entita_R
        Dim iFilePrescrizione As Integer = 0


        'scorrendo i record otterò un raggruppamento di: "Linea guida - Punto A, Linea guida - Punto B - Linea guida (retta che congiunge), poligono di confini 
        'se non sono presenti le linee guida allora otterrò il solo  poligono di confine.

        Dim folderExists As Boolean
        folderExists = My.Computer.FileSystem.DirectoryExists(inData.exportfileBasePath & "\Rx")
        If Not folderExists Then
            My.Computer.FileSystem.CreateDirectory(inData.exportfileBasePath & "\Rx")
        End If

        For Each lineaGuida As DataRow In dtOperazioniRicette.Rows

            Dim curEntita_Cod As Integer = lineaGuida("Entita_cod")
            Dim iRicettaOperazione_Cod As Integer = lineaGuida("Ricetta_Operazione_Cod")

            Dim lSoloNomeFilePrescrizione As String = iRicettaOperazione_Cod & "_" & iFilePrescrizione
            lSoloNomeFilePrescrizione = Right("PPPPPPPP" & lSoloNomeFilePrescrizione, 8)

            Dim lFilePrescription As String = inData.exportfileBasePath & "\Rx\" & lSoloNomeFilePrescrizione & ".shp"

            If Not My.Computer.FileSystem.FileExists(lFilePrescription) And lineaGuida("LayerElementiGrafici_COD") = inData.LayerDoveLeggereConfini Then


                Dim shpWrap As New AgronicaSHPWrapper.AgronicaGis2012ToShapeVarie
                shpWrap.PredisponiPassoLineaGuidaMappaPrescrizione(
                    inData.LayerDoveLeggereConfini,
                    inData.objParametri_Server,
                    inData.objParametri_Utenti,
                    curEntita_Cod,
                    LeggiGIS,
                    iFilePrescrizione,
                    inData.Codice_Fiscale_Tecnico,
                    iRicettaOperazione_Cod,
                    lFilePrescription,
                    inData.jDeereDataModelCFG.mappaturaDatiMappaPrescrizione,
                    True)

            End If

        Next 'Linea Guida

    End Sub

    Private Sub ScritturaFileSetupViaAdapt(inData As EsportazioneBordoMacchina_EsportazioneIn, A2JD As Agronica2JohnDeere.JDeereFilesWrapper)

        A2JD.ScriviFileSetup(PluginNameStringFromTipoEnum(inData.TipoEsportazioneBordoMacchina), 0, inData.exportfileBasePath)

    End Sub

    Private Sub LetturaDatiJD()

    End Sub

    Private Sub RiportoDatiGiasSuStruttureJohnDeere(dt As DataTable, indata As EsportazioneBordoMacchina_EsportazioneIn, ByRef A2JD As Agronica2JohnDeere.JDeereFilesWrapper)
        'leggere i dati da GIAS per creare le strutture dati da mandare ad ADAPT per i documenti relativi alla lavorazione (client\farm\field\boundary\etc..)
        'dal datatable prendere l'elemento con layercod=19 , da lì prendere piva\sa_cod\appezza e rileggere la entità_gis per tipo_entita_cod=1 
        'in questo modo ho sia il riferimento al campo (piva\sa_cod\appezza) che il codice del boundary

        Dim rObjGIAS = New AgronicaCoreAnagrafeDAL.jDeereDataModelDAL_EntitaGias_R
        Dim dtf As DataTable

        For Each ele In dt.Select("LayerElementiGrafici_cod=19")
            'verfico che l'oggetto del field sia mappato sulle strutture dati JD e che sia stato inviato al cloud JD
            dtf = rObjGIAS.GetJDEntityGIASSyncro(ele("piva"), ele("sa_cod"), ele("appezza"), "", "", indata.objParametri_Server)
            If dtf Is Nothing Then
                Throw New Exception("Appezzamento (" + indata.Appezza.ToString() + ") non sincronizzato con John Deere Operation Center, eseguire sincronizzazione prima di procedere")
            Else
                For Each row As DataRow In dtf.Rows
                    If InStr(row("FieldUUID"), "AGR-") > 0 Then
                        Throw New Exception("Appezzamento (" + indata.Appezza.ToString() + ") non sincronizzato con John Deere Operation Center, eseguire sincronizzazione prima di procedere")
                    Else
                        A2JD.setupDataCreator.AddClientFarmField(row("ClientName"), row("ClientUUID"), row("FarmName"), row("FarmUUID"), row("FieldName"), row("FieldUUID"))
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub LetturaDatiGiasSuFiltriPassati(inData As EsportazioneBordoMacchina_EsportazioneIn)

    End Sub

    ''' <summary>
    ''' Leggi Elenco di Macchine ed operatori
    ''' </summary>
    ''' <param name="layerElementiPF"></param>
    ''' <param name="inData"></param>
    ''' <returns></returns>
    Public Function RecuperaElencoMacchineOperatori(
                            inData As EsportazioneBordoMacchina_EsportazioneIn
                        ) As DataTable

        Return GetDtOperazioniRicette(inData.LayerDoveLeggereConfini, inData)

    End Function

    Private Shared Function GetDtOperazioniRicette(layerElementiPF As Integer, inData As EsportazioneBordoMacchina_EsportazioneIn) As DataTable
        Dim curEntita_Cod As Int32 = 0
        Dim leggiOperazioniRicette As New AgronicaCoreGisDAL.PrecisionFarming

        ' VAnni: 3/3/2017: seleziono i layer per le linee guida ed il layer per i confini (appezzamenti, impianti oppure planning)
        Dim xLeggiTipoEntita As New AgronicaCoreGisDAL.GIS_TipoEntita_R
        Dim dtLEggiTipoEntita As DataTable =
        xLeggiTipoEntita.Leggi(0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, " LayerElementiGrafici_cod = " & layerElementiPF, "", inData.objParametri_Server)
        Dim FiltroTipi As String = String.Join(",",
            (
            From d In dtLEggiTipoEntita.AsEnumerable
            Select d("TipoEntita_Cod")).ToArray
       )

        Return leggiOperazioniRicette.LeggiLineeGuidaAB(
            inData.objParametri_Server.PivaSuperUser,
            inData.Entita_Cod,
            inData.TipoEntita_Cod,
            inData.PivaPadre,
            inData.Piva,
            inData.Sa_Cod,
            inData.Appezza,
            inData.Campo_Cod,
            inData.ID_Imp,
            inData.Prov,
            inData.Com,
            inData.Sezione,
            inData.Foglio,
            inData.Numero,
            inData.Subalterno,
            inData.ID_Agenda,
            0,
            inData.Programmazione_Entita_cod,
            0,
            inData.ListaContatti,
            inData.LeggiContattiMacchineOppureDatoFinale,
            inData.datada,
            inData.dataa,
            layerElementiPF,
            inData.Codice_Fiscale_Tecnico,
            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
            " Entita.TipoEntita_Cod in (" & FiltroTipi & ", 55) ",
            "",
            inData.objParametri_Server)

    End Function

    Private Shared Function PluginNameStringFromTipoEnum(EnumFormato As enum_BordoMacchinaFormati) As String

        Select Case EnumFormato
            Case enum_BordoMacchinaFormati.AgGatewayIsoXml
                Return CostantiPersonalizzate.AdaptFrameworkPluginNames.AgGatewayIsoXml
            Case enum_BordoMacchinaFormati.AgGatewayApplicationDataModel
                Return CostantiPersonalizzate.AdaptFrameworkPluginNames.AgGatewayApplicationDataModel
            Case enum_BordoMacchinaFormati.Deere1800
                Return CostantiPersonalizzate.AdaptFrameworkPluginNames.Deere1800
            Case enum_BordoMacchinaFormati.Deere2600
                Return CostantiPersonalizzate.AdaptFrameworkPluginNames.Deere2600
            Case enum_BordoMacchinaFormati.Deere2630
                Return CostantiPersonalizzate.AdaptFrameworkPluginNames.Deere2630
            Case enum_BordoMacchinaFormati.DeereGen4
                Return CostantiPersonalizzate.AdaptFrameworkPluginNames.DeereGen4
            Case enum_BordoMacchinaFormati.DeereGen2_CommandCenter
                Return CostantiPersonalizzate.AdaptFrameworkPluginNames.DeereGen2_CommandCenter
        End Select

    End Function

End Class



Public Class EsportazioneBordoMacchina_EsportazioneIn

    Public TipoEsportazioneBordoMacchina As enum_BordoMacchinaFormati
    Public Property PivaSuperUser As String
    Public Property Entita_Cod As Int32
    Public Property TipoEntita_Cod As Int32
    Public Property PivaPadre As String
    Public Property Piva As String
    Public Property Sa_Cod As Int32
    Public Property Appezza As Int32
    Public Property Campo_Cod As Int32
    Public Property ID_Imp As Int32
    Public Property Prov As String
    Public Property Com As String
    Public Property Sezione As String
    Public Property Foglio As Int32
    Public Property Numero As Int32
    Public Property Subalterno As String
    Public Property ID_Agenda As Int32
    Public Property Ricetta_Operazione_Cod As Integer
    Public Property Programmazione_Entita_cod As Integer
    Public Property exportfileBasePath As String
    Public Property NomeFileFinale As String
    Public Property ListaContatti As String
    Public Property LeggiContattiMacchineOppureDatoFinale As Integer
    Public Property datada As DateTime
    Public Property dataa As DateTime
    Public Property LayerDoveLeggereConfini As Integer
    Public Property Codice_Fiscale_Tecnico As String
    Public Property objParametri_Server As AgronicaCoreParametri
    Public Property objParametri_Utenti As AgronicaCoreParametri
    Public Property jDeereDataModelCFG As jDeereDataModel_ApiCFG

End Class