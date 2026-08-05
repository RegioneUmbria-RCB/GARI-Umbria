Imports System.IO
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Gis
Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreUtentiDAL
Imports AgronicaSHPWrapper
Imports Newtonsoft.Json

Public Class Caricamento_Catasto
    Inherits LogProvider
    Implements IAlgoritmoProiezione

    Private ReadOnly LayerAnalysisConfig_Algorithm_Cod As Int32
    Private ReadOnly LayerAnalysisConfig_AlgorithmType_Cod As Int32
    Public Sub New(ByVal Algoritmo_Cod As Int32, ByVal TipoAlgoritmo_Cod As Int32)
        Me.LayerAnalysisConfig_Algorithm_Cod = Algoritmo_Cod
        Me.LayerAnalysisConfig_AlgorithmType_Cod = TipoAlgoritmo_Cod
    End Sub

    Public Sub LeggiLayerDaConfigurazioneAlgoritmo(LayerAnalysisConfig_Cod As Integer,
                                                   ByRef objParametri As AgronicaCoreParametri) Implements IAlgoritmoProiezione.LeggiLayerDaConfigurazioneAlgoritmo
        Throw New NotImplementedException()
    End Sub

    Public Sub LeggiLayerDaParametriEsecuzione(ParametriEsecuzione As String) Implements IAlgoritmoProiezione.LeggiLayerDaParametriEsecuzione
        Throw New NotImplementedException()
    End Sub

    Public Function Esegui(ByVal LayerAnalysisConfig_Cod As Integer,
                           ByVal Entita_cod_1 As Integer,
                           ByVal Entita_cod_2 As Integer,
                           ByVal Entita_cod_Risultato As Integer,
                           ByVal Esecuzione_cod As Integer,
                           ByVal Esecuzione_GUID As String,
                           ByVal ParametriEsecuzione As String,
                           ByRef objParametri_Server As AgronicaCoreParametri,
                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                           ByRef objParametri_Super_Server As AgronicaCoreParametri,
                           Optional override_transazione As Boolean = False) As Boolean Implements IAlgoritmoProiezione.Esegui

        Dim messaggistica As New AgronicaCoreMessaggisticaBIZ.Messaggi_Esecuzione_W

        Dim objCaricaCatasto As CaricaCatasto_IN = JsonConvert.DeserializeObject(Of CaricaCatasto_IN)(ParametriEsecuzione)

        Dim result As Boolean = True
        Dim fileList As String() = Nothing

        Dim _ObjParametriServer As AgronicaCoreParametri = objParametri_Server.CreateDeepCopy(objParametri_Server)
        Dim _ObjParametriUtenti As AgronicaCoreParametri = objParametri_Server.CreateDeepCopy(objParametri_Utenti)

        Try
            If objCaricaCatasto.InData.tipologiaShape_cod = Tipo_Importazione_FileShape.Importa_Raster Then
                messaggistica.AccodaMessaggioEsecuzione(objParametri_Server.UtenteUsername,
                    $"Tipologia shape cod {Tipo_Importazione_FileShape.Importa_Raster} non gestita",
                    objParametri_Server)
                Return False
            End If

            Dim catasto As TipoFileCatasto
            If objCaricaCatasto.InData.datiCatasto IsNot Nothing Then
                catasto = objCaricaCatasto.InData.datiCatasto.fileCatasto
            Else
                catasto = Nothing
            End If

            Dim fileZipName = objCaricaCatasto.InData.fileZipName
            Dim tipologiaShape_cod = objCaricaCatasto.InData.tipologiaShape_cod

            If Path.GetExtension(fileZipName) <> ".zip" Then
                messaggistica.AccodaMessaggioEsecuzione(objParametri_Server.UtenteUsername,
                    "Il file caricato non è in formato ZIP",
                    objParametri_Server)
                Return False
            End If

            If Not File.Exists(fileZipName) Then
                messaggistica.AccodaMessaggioEsecuzione(objParametri_Server.UtenteUsername,
                    $"Il file {fileZipName} non esiste",
                    objParametri_Server)
                Return False
            End If

            Dim dir = Path.GetDirectoryName(fileZipName)
            Dim workDir As String = Path.Combine(dir, Path.GetFileNameWithoutExtension(fileZipName))
            If Directory.Exists(workDir) Then
                Directory.Delete(workDir, True)
            End If
            Directory.CreateDirectory(workDir)

            AgronicaCoreUtility.AgroZip.UnZip(fileZipName, workDir)

            If tipologiaShape_cod = Tipo_Importazione_FileShape.Importa_KmlKmz Then
                Dim kmzFiles = Directory.GetFiles(workDir, "*.kmz", SearchOption.AllDirectories)

                For Each kmzipFile In kmzFiles
                    AgronicaCoreUtility.AgroZip.UnZip(kmzipFile, workDir)
                Next
            End If

            fileList = Directory.GetFiles(workDir, GetTipologiaFile(tipologiaShape_cod, catasto), SearchOption.AllDirectories)

            If fileList Is Nothing OrElse Not fileList.Any() Then

                messaggistica.AccodaMessaggioEsecuzione(objParametri_Server.UtenteUsername,
                    "Non è stato trovato nessun file idoneo all'importazione.",
                    objParametri_Server)
                Return False
            End If

        Catch ex As Exception
            messaggistica.AccodaMessaggioEsecuzione(objParametri_Server.UtenteUsername,
                ex.Message,
                objParametri_Server)
            Return False
        End Try

        If Not override_transazione Then

        End If

        For Each fileName In fileList
            Try
                Scrivi_LOG(_ObjParametriServer, "Caricamento_Catasto", $"Inizio importazione file {fileName}", False)
                Dim success = salvaElementiGraficiDaFileShape(fileName,
                                objCaricaCatasto.InData.layer_cod,
                                objCaricaCatasto.InData.datiImpianto,
                                objCaricaCatasto.InData.datiCatasto,
                                objCaricaCatasto.InData.tipologiaShape_cod,
                                objCaricaCatasto.InData.codice_sistemaRiferimento,
                                objCaricaCatasto.InData.progressivoGIAS,
                                _ObjParametriServer,
                                _ObjParametriUtenti,
                                objCaricaCatasto.InData.Validita_Inizio,
                                objCaricaCatasto.InData.Validita_Fine,
                                objCaricaCatasto.InData.Description,
                                objCaricaCatasto.InData.PixelSize)
                Scrivi_LOG(_ObjParametriServer, "Caricamento_Catasto", $"fine importazione file {fileName} con esito {IIf(success, "positivo", "negativo")}", False)
                result = result AndAlso success
            Catch ex As Exception
                messaggistica.AccodaMessaggioEsecuzione(objParametri_Server.UtenteUsername,
                    $"Un problema nel salvataggio dei file shape per il file {fileName}." + ex.Message,
                    objParametri_Server)
                result = False
            End Try

        Next

        Return result
    End Function

    Private Function DeserializzaInData(Of T)(ByVal InData As Object) As ObjParametri(Of T)

        Dim JsonSettings As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objInData As CoreWS_Generic(Of T) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of T))(JsonConvert.SerializeObject(InData), JsonSettings)

        Dim objParametri As New ObjParametri(Of T)

        objParametri.Super_Server = Utility.convertStringtoOBJparametri(objInData.objP.objP_super_server)
        objParametri.Server = Utility.convertStringtoOBJparametri(objInData.objP.objP_server)
        objParametri.Utenti = Utility.convertStringtoOBJparametri(objInData.objP.objP_utenti)

        objParametri.InData = objInData.InData

        Return objParametri

    End Function

    Private Shared Function GetTipologiaFile(ByVal tipologiaShape_cod As Integer, ByVal tipoFileCatasto As TipoFileCatasto) As String
        Dim filtro As String

        If tipologiaShape_cod = Tipo_Importazione_FileShape.importaCatasto_DXF Then
            If tipoFileCatasto = TipoFileCatasto.CatastoDXF Then
                filtro = "*.dxf"
            Else
                filtro = "*.shp"
            End If
        ElseIf tipologiaShape_cod = Tipo_Importazione_FileShape.Importa_KmlKmz Then
            filtro = "*.kml"
        Else
            filtro = "*.shp"
        End If

        Return filtro
    End Function

    Private Shared Function salvaElementiGraficiDaFileShape(ByVal fileName As String,
                                                           ByVal layer_cod As Int32,
                                                           ByVal datiImpianto As DatiImpianto_Importazione,
                                                           ByVal datiCatasto As Daticatasto_Importazione,
                                                           ByVal tipologiaShape_cod As Int32,
                                                           ByVal codice_sistemaRiferimento As Int32,
                                                           ByVal progressivoGIAS As Int32,
                                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                           Optional ByVal inizio_validita As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                                                           Optional ByVal fine_validita As Date = CostantiPersonalizzate.AGRODATAFINE,
                                                           Optional ByVal descrizione_file As String = "",
                                                           Optional ByVal pixelSize As Int32 = 0
                                                           ) As Boolean


        Dim objUtentiDAL As New Utenti_xGruppi_Utente_R

        Dim Codice_Fiscale_Tecnico =
            objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(objParametri_Server.UtenteUsername, objParametri_Utenti)

        Dim impostaRiproiezioneDaTipoImportazione As Boolean = False
        Dim riproiezioneRichiestaDaUtente As Boolean = (codice_sistemaRiferimento > 0)
        Dim layerPerCatasto = layer_cod

        If tipologiaShape_cod = Tipo_Importazione_FileShape.Importazione_Trimble Then
            'trimble
            layerPerCatasto = 1
            impostaRiproiezioneDaTipoImportazione = False
            codice_sistemaRiferimento = 1
        End If
        If tipologiaShape_cod = Tipo_Importazione_FileShape.importaCatasto_DXF Then
            layerPerCatasto = 3
            If datiCatasto.fileCatasto = TipoFileCatasto.CatastoDXF Then
                impostaRiproiezioneDaTipoImportazione = True
            Else
                impostaRiproiezioneDaTipoImportazione = False
                If Not (codice_sistemaRiferimento <> "-1") Then
                    codice_sistemaRiferimento = 1
                End If
            End If

        End If

        If tipologiaShape_cod = Tipo_Importazione_FileShape.importaGeneric_SHP Or
           tipologiaShape_cod = Tipo_Importazione_FileShape.Importa_KmlKmz Then

            If Not (codice_sistemaRiferimento <> "-1") Then
                codice_sistemaRiferimento = 1
            End If
        End If

        If tipologiaShape_cod = Tipo_Importazione_FileShape.Importazione_Trimble And datiImpianto.id_reg = 0 Then
            Throw New Exception("Nessun impianto selezionato.")
        End If

        Dim trasformaSistemaRiferimento As Boolean = (impostaRiproiezioneDaTipoImportazione Or riproiezioneRichiestaDaUtente)

        Select Case tipologiaShape_cod
            Case Tipo_Importazione_FileShape.importaCatasto_DXF
                Dim configurazioneImportazione As ConfigurazioneImportazione =
                        New ConfigurazioneImportazione(fileName,
                                                       objParametri_Server.PivaSuperUser,
                                                       "",
                                                       0,
                                                       datiImpianto.campo_cod,
                                                       datiImpianto.appezza,
                                                       datiImpianto.id_reg,
                                                       0,
                                                       0,
                                                       "",
                                                       0,
                                                       0,
                                                       layer_cod,
                                                       tipologiaShape_cod,
                                                       Now.Year,
                                                       "",
                                                       "",
                                                       progressivoGIAS,
                                                       trasformaSistemaRiferimento,
                                                       codice_sistemaRiferimento,
                                                       CInt(TipiEnumerativi.enum_GIS2012_TipoEntita.CATASTO).ToString,
                                                       Codice_Fiscale_Tecnico)

                configurazioneImportazione.DataInizioValidita = inizio_validita
                configurazioneImportazione.DataFineValidita = fine_validita

                configurazioneImportazione.ConfigurazioneImportazione_Catasto = New ConfigurazioneImportazione_Catasto
                configurazioneImportazione.ConfigurazioneImportazione_Catasto.AzioneSuDati_1Sovrascrive_2Ignora_3Aggiunge = datiCatasto.ComportamentoImportazione
                configurazioneImportazione.ConfigurazioneImportazione_Catasto.CreaLayerTestuale = datiCatasto.CreaLayerTestuale

                configurazioneImportazione.ConfigurazioneImportazione_Catasto.CodBelfiore = datiCatasto.CodBelfiore
                configurazioneImportazione.ConfigurazioneImportazione_Catasto.Prov = datiCatasto.Provincia
                configurazioneImportazione.ConfigurazioneImportazione_Catasto.Com = datiCatasto.Comune
                configurazioneImportazione.ConfigurazioneImportazione_Catasto.Sezione = datiCatasto.Sezione
                configurazioneImportazione.ConfigurazioneImportazione_Catasto.Foglio = datiCatasto.Foglio
                configurazioneImportazione.ConfigurazioneImportazione_Catasto.Particella = datiCatasto.Particella
                configurazioneImportazione.ConfigurazioneImportazione_Catasto.Subalterno = datiCatasto.Subalterno
                configurazioneImportazione.ConfigurazioneImportazione_Catasto.IdentificativoEsterno = datiCatasto.IdentificativoEsterno
                configurazioneImportazione.ConfigurazioneImportazione_Catasto.FiltroParticelleCatastali = datiCatasto.FiltroParticelleCatastali

                If datiCatasto.fileCatasto = TipoFileCatasto.CatastoDXF Then
                    Dim objImportDXF = New AgronicaSHPWrapper.DXF_ToAgronicaGIS2012
                    configurazioneImportazione.ConfigurazioneImportazione_Catasto.ListaLayersDXFAgenziaEntrate = datiCatasto.ListaLayersDXFAgenziaEntrate

                    objImportDXF.convert(configurazioneImportazione, objParametri_Server, objParametri_Utenti)
                Else

                    Dim objImportSHP = New AgronicaSHPWrapper.ShapeFileToAgronicaGis2012


                    objImportSHP.convert(configurazioneImportazione, objParametri_Server, objParametri_Utenti)

                End If
            Case Tipo_Importazione_FileShape.Importa_Raster
                Dim objEntitaWrite As New AgronicaSHPWrapper.Raster_ToAgronicaGIS2012

                Dim configurazioneFileRaster As New ConfigurazioneImportazione(fileName,
                                                                               objParametri_Server.PivaSuperUser,
                                                                               datiImpianto.piva,
                                                                               datiImpianto.sa_cod,
                                                                               datiImpianto.campo_cod,
                                                                               datiImpianto.appezza,
                                                                               datiImpianto.id_reg,
                                                                               0,
                                                                               0,
                                                                               "",
                                                                               0,
                                                                               0,
                                                                               layer_cod,
                                                                               tipologiaShape_cod,
                                                                               Now.Year,
                                                                               "",
                                                                               "",
                                                                               progressivoGIAS,
                                                                               trasformaSistemaRiferimento,
                                                                               codice_sistemaRiferimento,
                                                                               CInt(TipiEnumerativi.enum_GIS2012_TipoEntita.RASTER).ToString,
                                                                               Codice_Fiscale_Tecnico)
                configurazioneFileRaster.DataInizioValidita = inizio_validita
                configurazioneFileRaster.DataFineValidita = fine_validita
                configurazioneFileRaster.DescrizioneFile = descrizione_file
                configurazioneFileRaster.PixelSize = pixelSize

                objEntitaWrite.convert(configurazioneFileRaster, objParametri_Server, objParametri_Utenti)

            Case Tipo_Importazione_FileShape.Importa_KmlKmz

                Dim objImportKML = New AgronicaSHPWrapper.KML_ToAgronicaGIS2012

                Dim configurazioneKML = New ConfigurazioneImportazione(fileName,
                                                                    objParametri_Server.PivaSuperUser,
                                                                    datiImpianto.piva,
                                                                    datiImpianto.sa_cod,
                                                                    datiImpianto.campo_cod,
                                                                    datiImpianto.appezza,
                                                                    datiImpianto.id_reg,
                                                                    0,
                                                                    0,
                                                                    "",
                                                                    0,
                                                                    0,
                                                                    layer_cod,
                                                                    tipologiaShape_cod,
                                                                    Now.Year,
                                                                    "",
                                                                    "",
                                                                    progressivoGIAS,
                                                                    trasformaSistemaRiferimento,
                                                                    codice_sistemaRiferimento,
                                                                    CInt(TipiEnumerativi.enum_GIS2012_TipoEntita.DATI_IMPORTATI).ToString,
                                                                    Codice_Fiscale_Tecnico)

                configurazioneKML.DataInizioValidita = inizio_validita
                configurazioneKML.DataFineValidita = fine_validita

                objImportKML.convert(configurazioneKML, objParametri_Server, objParametri_Utenti)
            Case Else
                Dim objImportSHP = New ShapeFileToAgronicaGis2012

                Dim configurazioneShape = New ConfigurazioneImportazione(fileName,
                                                                         objParametri_Server.PivaSuperUser,
                                                                         datiImpianto.piva,
                                                                         datiImpianto.sa_cod,
                                                                         datiImpianto.campo_cod,
                                                                         datiImpianto.appezza,
                                                                         datiImpianto.id_reg,
                                                                         0,
                                                                         0,
                                                                         "",
                                                                         0,
                                                                         0,
                                                                         layer_cod,
                                                                         tipologiaShape_cod,
                                                                         Now.Year,
                                                                         "",
                                                                         "",
                                                                         progressivoGIAS,
                                                                         trasformaSistemaRiferimento,
                                                                         codice_sistemaRiferimento,
                                                                         CInt(TipiEnumerativi.enum_GIS2012_TipoEntita.DATI_IMPORTATI).ToString,
                                                                         Codice_Fiscale_Tecnico)

                configurazioneShape.DataInizioValidita = inizio_validita
                configurazioneShape.DataFineValidita = fine_validita

                objImportSHP.convert(configurazioneShape, objParametri_Server, objParametri_Utenti)

        End Select

        Return True

    End Function

    Private Class ObjParametri(Of T)

        Public Super_Server As AgronicaCoreParametri
        Public Server As AgronicaCoreParametri
        Public Utenti As AgronicaCoreParametri
        Public InData As T

    End Class
End Class
