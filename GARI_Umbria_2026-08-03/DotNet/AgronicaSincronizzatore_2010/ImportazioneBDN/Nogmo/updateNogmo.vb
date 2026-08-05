Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreMapper
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.attivita.centri_di_costo
Imports AgronicaCoreEntityFramework
Imports System.Transactions

Public Class updateNogmo
    Inherits wsNogmo

    Dim objParametriServer As AgronicaCoreParametri
    Dim objParametriUtenti As AgronicaCoreParametri

    Dim wsUpdate As ChiamawsAnagraficaCapoQry

    Dim GiasContext As Gias_DeveloperServer_Entities
    Dim sincronizzatoreAnimale As SincroBDNAnimale

    Dim logDirectory As String
    Dim logFileName As String
    Dim objLog As New AgronicaCoreDataProvider.LogProvider

    Dim customLOGParams As CustomLOGParams

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriUtenti As AgronicaCoreParametri)
        MyBase.New(objParametriServer, objParametriUtenti)

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

    End Sub
End Class
