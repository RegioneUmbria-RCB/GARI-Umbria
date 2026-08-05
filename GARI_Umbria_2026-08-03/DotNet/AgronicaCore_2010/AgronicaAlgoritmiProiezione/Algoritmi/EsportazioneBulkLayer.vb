Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDTOStd.InData.Gis
Imports AgronicaSHPWrapper
Imports Newtonsoft.Json.Linq

Public Class EsportazioneBulkLayer
    Implements IAlgoritmoProiezione

    Private ReadOnly LayerAnalysisConfig_Algorithm_Cod As Int32
    Private ReadOnly LayerAnalysisConfig_AlgorithmType_Cod As Int32

    Private Layer_1 As ProiezioneLayer
    Private Layer_2 As ProiezioneLayer
    Private Layer_Risultato As ProiezioneLayer

    Public Sub New(ByVal Algoritmo_Cod As Int32, ByVal TipoAlgoritmo_Cod As Int32)
        Me.LayerAnalysisConfig_Algorithm_Cod = Algoritmo_Cod
        Me.LayerAnalysisConfig_AlgorithmType_Cod = TipoAlgoritmo_Cod
    End Sub


    Public Sub LeggiLayerDaConfigurazioneAlgoritmo(ByVal LayerAnalysisConfig_Cod As Integer,
                                                   ByRef objParametri As AgronicaCoreParametri) Implements IAlgoritmoProiezione.LeggiLayerDaConfigurazioneAlgoritmo

    End Sub

    Public Sub LeggiLayerDaParametriEsecuzione(ByVal ParametriEsecuzione As String) Implements IAlgoritmoProiezione.LeggiLayerDaParametriEsecuzione

        Dim parametriObj As JObject = JObject.Parse(ParametriEsecuzione)

        Dim xRead As New AgronicaCoreGisBIZ.ProiezioniLayer_R

        Me.Layer_1 = New ProiezioneLayer With {
            .LayerElementiGrafici_Cod = CInt(parametriObj("layerElementiGrafici_Cod")),
            .TipologiaLayer_cod = CInt(parametriObj("tipologiaLayer_Cod"))
        }
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

        'TODO: verificare override_transazione

        LeggiLayerDaParametriEsecuzione(ParametriEsecuzione)

        Dim resp As Boolean
        Dim isImpianto As Boolean = False
        Dim username As String = ""

        Dim exporter As New AgronicaCoreGisDAL.GIS_Entita_R
        Dim messaggistica As New AgronicaCoreMessaggisticaBIZ.Messaggi_Esecuzione_W

        Dim codiceFiscaleTecnico As String = ""
        Dim xmlExportazione As XDocument
        Dim ParametriEsecuzioneObj As JObject = JObject.Parse(ParametriEsecuzione)
        Dim applicaFiltroTabellaUtentiVisibilitaAppoggio As Boolean = False

        Dim LeggiConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
        Dim Percorso As String = dt_Conf.Rows(0).Item("Valore")

        If Percorso Is Nothing OrElse Percorso.Equals("") OrElse Not System.IO.Directory.Exists(Percorso) Then
            Throw New Exception("Impossibile determinare il percorso remoto della cartella allegati.")
        End If

        Dim shapeFileName = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName())
        Dim basePath = System.IO.Path.Combine(Percorso, shapeFileName)
        Dim fileNameFullPath = System.IO.Path.Combine(basePath, String.Format("{0}.shp", shapeFileName))

        Dim usaCFTecnico As Boolean = False

        If Layer_1.LayerElementiGrafici_Cod = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.IMPIANTI Then
            isImpianto = True

            dt_Conf = LeggiConfSiti.Leggi(6, "GIS_EscludiFiltroCodiceFiscaleTecnico", "", "", objParametri_Server)

            If dt_Conf IsNot Nothing AndAlso dt_Conf.Rows.Count > 0 Then
                usaCFTecnico = Not CBool(dt_Conf.Rows(0).Item("Valore"))
            End If

            If usaCFTecnico Then
                Dim xReadUtenti As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

                codiceFiscaleTecnico = xReadUtenti.Leggi_IdentificativoGruppoUtenti_Singolo(objParametri_Utenti.UtenteUsername, objParametri_Utenti)
            End If
        End If

        If CBool(ParametriEsecuzioneObj("applicaFiltroTabellaUtentiVisibilitaAppoggio")) Then
            Dim xPLr As New AgronicaCoreGisDAL.ProiezioniLayer_R
            Dim usr As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R

            Dim cf = xPLr.LeggiEsecuzioneDaCodice(Esecuzione_cod, objParametri_Server).Rows(0)("Username_Modifica")
            username = usr.Username_From_CodFisc(cf, objParametri_Utenti)

            applicaFiltroTabellaUtentiVisibilitaAppoggio = AgronicaCoreGisDAL.GIS_Entita_R.LeggiGetQuery_DecidiFiltro_Utenti_Visibilita_Appoggio(objParametri_Server, username)
        End If

        Dim idEsp As Integer = CInt(ParametriEsecuzioneObj("idEsp"))

        xmlExportazione = exporter.LeggiXDocumentExportSHP_List(Layer_1.LayerElementiGrafici_Cod,
                                                                CBool(ParametriEsecuzioneObj("fullLayerExport")),
                                                                applicaFiltroTabellaUtentiVisibilitaAppoggio,
                                                                idEsp,
                                                                codiceFiscaleTecnico,
                                                                isImpianto,
                                                                usaCFTecnico,
                                                                objParametri_Server,
                                                                username)

        Dim nsGrafica As XNamespace = "http://www.agronica.it/grafica/"
        Dim nsGml As XNamespace = "http://www.opengis.net/gml"

        If xmlExportazione Is Nothing Then
            Throw New Exception("Nessuno shape da esportare.")
        End If

        If System.IO.Directory.Exists(basePath) Then
            System.IO.Directory.Delete(basePath, True)
        End If

        System.IO.Directory.CreateDirectory(basePath)

        Dim lRoot As XElement = xmlExportazione.Root
        lRoot.Name = nsGrafica + lRoot.Name.LocalName
        lRoot.Add(New XAttribute(XNamespace.Xmlns + "gml", nsGml))

        Dim target As AgronicaGis2012ToShapeFile = New AgronicaGis2012ToShapeFile()
        resp = target.Convert(xmlExportazione.ToString.Replace("xmlns=""""", ""), fileNameFullPath, False, New List(Of DBFDataModel_MappaturaDati))

        If Not resp Then
            Throw New Exception("Errore nell'esportazione degli shape.")
        End If

        Dim zipFile As String = fileNameFullPath.Replace(".shp", ".zip")
        AgronicaCoreUtility.AgroZip.AddFileToZip(zipFile, fileNameFullPath)
        AgronicaCoreUtility.AgroZip.AddFileToZip(zipFile, fileNameFullPath.Replace(".shp", ".dbf"))
        AgronicaCoreUtility.AgroZip.AddFileToZip(zipFile, fileNameFullPath.Replace(".shp", ".shx"))
        AgronicaCoreUtility.AgroZip.AddFileToZip(zipFile, fileNameFullPath.Replace(".shp", ".prj"))

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim overrideUsername = ""

        If ParametriEsecuzioneObj("UtenteOperazione") IsNot Nothing AndAlso
                Not ParametriEsecuzioneObj("UtenteOperazione").ToString.Equals("") Then
            overrideUsername = ParametriEsecuzioneObj("UtenteOperazione").ToString
        End If

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Dim xWrite As New AgronicaCoreGisBIZ.GIS_Entita_W



            Dim newAllegatoID = xWrite.salvaAllegatoEstrazione(zipFile,
                                                               ParametriEsecuzioneObj("descrizioneEsportazione").ToString,
                                                               CDate(ParametriEsecuzioneObj("inizioValidita")),
                                                               CDate(ParametriEsecuzioneObj("fineValidita")),
                                                               objParametri_Server,
                                                               overrideUsername)

            resp = xWrite.salvaAllegatoLayer(newAllegatoID, Layer_1.LayerElementiGrafici_Cod, Layer_1.TipologiaLayer_cod, objParametri_Server, overrideUsername)

            If Not resp Then
                Throw New Exception("Errore nell'aggiornamento dell'allegato nelle entità.")
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            messaggistica.AccodaMessaggioEsecuzione(overrideUsername,
                                                    String.Format("Estrazione {0} terminata con successo.", ParametriEsecuzioneObj("descrizioneEsportazione").ToString),
                                                    objParametri_Server)

        Catch ex As Exception
            If objParametri_Server.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            messaggistica.AccodaMessaggioEsecuzione(overrideUsername,
                                                    ex.Message,
                                                    objParametri_Server)

            Throw ex
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return resp

        Return True
    End Function
End Class
