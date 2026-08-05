Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json

Public Class BundleState
    Public Function GetBundleState(ByVal Piva As String, ByVal CUAA As String, ByVal year As Integer, ByVal skip As Integer, ByVal top As Integer, ByVal onlyLast As Boolean, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Super_Server As AgronicaCoreParametri) As String
        Dim utility As New Utility

        If String.IsNullOrEmpty(CUAA) Then
            Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            CUAA = xImpCodR.Leggi_CUAA(Piva, objParametri_Server)
        End If

        Dim endpoint As String = "/api/bundle/state"

        If onlyLast Then
            endpoint &= "/latest"
        End If

        endpoint &= "?farmId=" & CUAA & "&year=" & year

        If Not onlyLast Then
            endpoint &= "&skip=" & skip & "&top=" & top
        End If

        Return utility.CallHubAgeaGET("", endpoint, objParametri_Super_Server)
    End Function

    Public Function CountBundleState(ByVal Piva As String, ByVal CUAA As String, ByVal year As Integer, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Super_Server As AgronicaCoreParametri) As String
        Dim utility As New Utility

        If String.IsNullOrEmpty(CUAA) Then
            Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            CUAA = xImpCodR.Leggi_CUAA(Piva, objParametri_Server)
        End If

        Dim endpoint As String = "/api/bundle/state/count" & "?farmId=" & CUAA & "&year=" & year
        Return utility.CallHubAgeaGET("", endpoint, objParametri_Super_Server)
    End Function

    Public Function GetAvailableFarms(ByVal Filters As InData.Agenda.FarmFilters, ByVal objParametri_Super_Server As AgronicaCoreParametri) As String
        Dim utility As New Utility
        Dim endpoint As String = "/api/farms/filter"
        Dim str_filters As String = JsonConvert.SerializeObject(Filters)

        Return utility.CallHubAgeaPOST(str_filters, endpoint, objParametri_Super_Server)
    End Function

    Public Function GetBundleData(ByVal BundleId As Integer, ByVal objParametri_Super_Server As AgronicaCoreParametri) As String
        Dim utility As New Utility

        Dim endpoint As String = "/api/bundle/" & BundleId & "/data"
        Return utility.CallHubAgeaGET("", endpoint, objParametri_Super_Server)
    End Function

    Public Function ReadBundleLog(ByVal instanceId As String, objParametri_Super_Server As AgronicaCoreParametri) As String
        Dim utility As New Utility

        Dim endpoint As String = "/api/quartz/ReadLogFile/" & instanceId
        Return utility.CallHubAgeaGET("", endpoint, objParametri_Super_Server)
    End Function
End Class
