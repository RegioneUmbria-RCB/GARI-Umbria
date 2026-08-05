Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreMapper
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.attivita.centri_di_costo
Imports AgronicaCoreEntityFramework
Imports System.Transactions
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModello.Nogmo
Imports RestSharp.Serialization.Json
Imports AgronicaCoreUtility

Public Class uploadNogmo

    Dim objParametriServer As AgronicaCoreParametri
    Dim objParametriUtenti As AgronicaCoreParametri

    Dim wsUpdate As ChiamawsAnagraficaCapoQry

    Dim GiasContext As Gias_DeveloperServer_Entities
    Dim sincronizzatoreAnimale As SincroBDNAnimale
    Dim agroZip As AgroZip
    Dim logDirectory As String
    Dim logFileName As String
    Dim objLog As New AgronicaCoreDataProvider.LogProvider
    Dim logInvioChiamate As AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W = New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W
    Dim logInvioAnagrafe As AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_W = New AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_W
    Dim ChiamateUpload As wsUpload
    Public exporterID As Integer
    Dim customLOGParams As CustomLOGParams

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriUtenti As AgronicaCoreParametri)

        Me.objParametriServer = objParametriServer
        Me.objParametriUtenti = objParametriUtenti
        logDirectory = objParametriServer.LogDirectory & "\NOGMO\"

        customLOGParams = New CustomLOGParams With {
            .LogDescrizioneUtente = objParametriServer.LogDescrizioneUtente,
            .LogDirectory = logDirectory,
            .LogFileName = "logNOGMO.txt"
        }

        objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Inizio NOGMO",
                          CustomLOGParams:=customLOGParams)

        GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
        ChiamateUpload = New wsUpload(objParametriServer, objParametriUtenti)
        agroZip = New AgroZip()
        exporterID = ChiamateUpload.exporterID
    End Sub

    Public Function checkBovine(dati As CheckBovine, idInvioChiamataAdd As Integer) As JObject
        Dim risp = ChiamateUpload.CheckBovine(dati)
        Dim data As Date = Date.Now
        Dim esito = IIf(risp.IsSuccessful, 1, 0)

        Dim rispostaCompressa = AgroZip.CompressioneBase64(1, risp.Content)

        logInvioChiamate.Create_Agronica_Log_Invio_Chiamate(TipiEnumerativi.enum_Esportazioni_Sistema_Cod.Nogmo, JsonConvert.SerializeObject(dati), data, esito, rispostaCompressa, 0,
                                                                "POST", objParametriServer, GiasContext, idInvioChiamataAdd)

        Return JsonConvert.DeserializeObject(risp.Content)
    End Function



    Public Function add(dati As DatiNogmo) As List(Of String)
        Dim ChiamateUpload = New wsUpload(objParametriServer, objParametriUtenti)

        Dim risp = ChiamateUpload.Add(dati)
        Dim data As Date = Date.Now


        Dim jsonRisposta = JsonConvert.DeserializeObject(risp.Content)
        Dim matricoleAnimaliFalliti = New List(Of String)


        If risp.IsSuccessful Then
            Dim idInvioChiamate = logInvioChiamate.Create_Agronica_Log_Invio_Chiamate(TipiEnumerativi.enum_Esportazioni_Sistema_Cod.Nogmo, JsonConvert.SerializeObject(dati), data, 1, risp.Content, 0,
                                                                "POST", objParametriServer, GiasContext).ID
            Dim upload_guid = jsonRisposta("upload_guid")

            Dim dictAnimaliFalliti As Dictionary(Of String, String) = New Dictionary(Of String, String)

            If jsonRisposta("count_total") <> jsonRisposta("count_compliant") Then 'Se alcuni animali sono falliti, faccio una chiamata per controllare quali sono falliti e per sapere il motivo
                Dim check = checkBovine(New CheckBovine With {.Identification = dati.BovineList.Select(Function(animale) animale.Identification).ToList}, idInvioChiamate)
                dictAnimaliFalliti = check.Item("BovineImportFail").Where(Function(item)
                                                                              Return item("Guid").ToString.ToLower = upload_guid.ToString.ToLower 'Controllo che gl animali falliti siano appartenti a questo carico
                                                                          End Function).ToDictionary(Of String, String)(Function(item) item("Identification"), Function(item) "Errori: " & item("EmptyFields").ToString & item("Errors").ToString) 'Converto a dizionario per velocizzare 
            End If

            dati.BovineList.ForEach(Sub(animale)
                                        Dim stato = 1
                                        If dictAnimaliFalliti.ContainsKey(animale.Identification) Then
                                            animale.Errori = dictAnimaliFalliti(animale.Identification)
                                            matricoleAnimaliFalliti.Add(animale.Identification)
                                            stato = 0
                                        End If
                                        logInvioAnagrafe.Scrivi(TipiEnumerativi.enum_Esportazioni_Sistema_Cod.Nogmo, idInvioChiamate, stato, "NOGMO", animale.chiave, animale.Piva, animale.Sa_Cod, 0 _
                                                                , 0, animale.Cod_Progetto, animale.Errori, data, objParametriServer)
                                    End Sub)

        Else
            Dim idInvioChiamate = logInvioChiamate.Create_Agronica_Log_Invio_Chiamate(TipiEnumerativi.enum_Esportazioni_Sistema_Cod.Nogmo, JsonConvert.SerializeObject(dati), data, 0, risp.Content, 0,
                                                    "POST", objParametriServer, GiasContext
                                                    ).ID
            dati.BovineList.ForEach(Sub(animale)
                                        matricoleAnimaliFalliti.Add(animale.Identification)
                                        logInvioAnagrafe.Scrivi(TipiEnumerativi.enum_Esportazioni_Sistema_Cod.Nogmo, idInvioChiamate, 0, "NOGMO", animale.chiave, animale.Piva, animale.Sa_Cod, 0 _
                                                                , 0, animale.Cod_Progetto, risp.Content, data, objParametriServer)
                                    End Sub)
        End If

        Return matricoleAnimaliFalliti
    End Function

End Class