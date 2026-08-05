Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.Widgets
Imports Newtonsoft.Json
Imports System.Linq
Imports System.Net

Public Class WeatherWidget
    Public Function ReadLatLng(objParams As AgronicaCoreParametri,
                               piva As String) As Widget_MeteoImpresaLatLng

        Dim result As New Widget_MeteoImpresaLatLng With {.Piva = piva}

        Dim objImpreseCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim codes As DataTable = objImpreseCodici.LeggiJoinCompleto(piva, 0, "", "", objParams)

        Dim lat = codes.Select("id_cod = " & enum_CodiciAnagrafe.Latitudine)
        Dim lng = codes.Select("id_cod = " & enum_CodiciAnagrafe.Longitudine)

        If lat.Length > 0 Then
            result.Lat = lat(0)("val_cod")
        End If

        If lng.Length > 0 Then
            result.Lng = lng(0)("val_cod")
        End If

        Dim objImpresexIndirizzi As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
        Dim impresaIndirizzi As DataTable = objImpresexIndirizzi.Leggi(piva, 0, 0, enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParams)
        result.Location = impresaIndirizzi(0)("com_des")
        result.Country = impresaIndirizzi(0)("stato")

        If result.Lat = 0 AndAlso result.Lng = 0 Then
            Dim impresaBIZ As New AgronicaCoreAnagrafeBIZ.Impresa_W
            Dim location = impresaBIZ.GetCompanyGeolocation(objParams, piva, throwEx:=True)
            result.Lat = location.lat
            result.Lng = location.lng
        End If

        Return result
    End Function
End Class
