Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class jDeereDataModel_Boundary
    Inherits jDeereDataModel_ItemGenerico
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property sourceType As String
    ''' <summary>
    ''' An ISO-8601 formatted timestamp of the last modification made to this boundary.
    ''' </summary>
    ''' <returns></returns>
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property modifiedTime As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property area As jDeereDataModel_MeasurementAsDouble
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property workableArea As jDeereDataModel_MeasurementAsDouble
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property multipolygons As List(Of jDeereDataModel_Polygon)
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property extent As jDeereDataModel_Extend
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property active As Boolean
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property irrigated As Boolean
    ''' <summary>
    ''' "True" indicates that the boundary can be crossed (Ex: a waterway). "False" indicates that the boundary cannot be crossed (ex: a boulder).
    ''' </summary>
    ''' <returns></returns>
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property passable As Boolean
    <JsonProperty("@type", NullValueHandling:=NullValueHandling.Ignore)>
    Public Property type As String
End Class

Public Class jDeereDataModel_Link
    Public Property uri As String
    Public Property rel As String

End Class

Public Class jDeereDataModel_Extend
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property topLeft As jDeereDataModel_Point
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property bottomRight As jDeereDataModel_Point
End Class

Public Class jDeereDataModel_MeasurementAsDouble
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property valueAsDouble As Double
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property unit As String

End Class
Public Class jDeereDataModel_Polygon
    <JsonProperty("@type", NullValueHandling:=NullValueHandling.Ignore)>
    Public Property Type As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property rings As List(Of jDeereDataModel_Ring)

    Public Sub New()
        Type = "Polygon"
        rings = New List(Of jDeereDataModel_Ring)
    End Sub

End Class
Public Class jDeereDataModel_Ring
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property points As List(Of jDeereDataModel_Point)
    <JsonProperty("@type", NullValueHandling:=NullValueHandling.Ignore)>
    Public Property objType As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property type As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property passable As Boolean

    Public Sub New()
        objType = "Ring"
        points = New List(Of jDeereDataModel_Point)
        type = "exterior"
        passable = False
    End Sub

End Class
Public Class jDeereDataModel_Point
    <JsonProperty("@type", NullValueHandling:=NullValueHandling.Ignore)>
    Public Property Type As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property lat As Double
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property lon As Double

    Public Sub New()
        Type = "Point"
        lat = 0
        lon = 0
    End Sub
End Class


Public Class jDeereDataModel_ApiCFG
    Public Property baseURL As String
    Public Property orgID As String
    Public Property mappaturaDatiMappaPrescrizione As List(Of DBFDataModel_MappaturaDati)
    Public Property JDeere As Agronica.Helpers.OAuth2.OAuth2DataModel_Config
    Public Property ForcedReFresh As Boolean
    Public Property JDeereWebHookCFG As jDeereDataModel_webhook_CFG

    Public Property ShapeType As enum_ShapeTypeMappaProduzioneJophnDeere
    Public Property Resolution As enum_ResolutionMappaProduzioneJophnDeere

    Public Sub New()
        baseURL = ""
        orgID = ""
        JDeere = Nothing
        JDeereWebHookCFG = Nothing
        ForcedReFresh = False
        ShapeType = 2
        Resolution = 3
    End Sub
End Class

Public Class jDeereDataModel_Field
    Inherits jDeereDataModel_ItemGenerico
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property archived As Boolean

    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property clients As jDeereDataModel_GenericList(Of jDeereDataModel_Client)
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property farms As jDeereDataModel_GenericList(Of jDeereDataModel_Farm)
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property boundaries As jDeereDataModel_GenericList(Of jDeereDataModel_Boundary)

    Public Sub New()
        archived = False
        clients = New jDeereDataModel_GenericList(Of jDeereDataModel_Client)
        farms = New jDeereDataModel_GenericList(Of jDeereDataModel_Farm)
        boundaries = New jDeereDataModel_GenericList(Of jDeereDataModel_Boundary)
    End Sub

End Class

Public Class jDeereDataModel_Farm
    Inherits jDeereDataModel_ItemGenerico

End Class


Public Class jDeereDataModel_Client
    Inherits jDeereDataModel_ItemGenerico
End Class

Public Class jDeereDataModel_ItemGenerico
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property name As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property id As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property guid As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property links As List(Of jDeereDataModel_Link)
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property OrganizationID As Integer?
End Class
Public Class jDeereDataModel_GenericList(Of T)

    Public Property total As Integer
    Public Property values As List(Of T)
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property links As List(Of jDeereDataModel_Link)

    Public Sub New()
        values = New List(Of T)
        links = New List(Of jDeereDataModel_Link)
    End Sub

End Class

Public Class jDeereDataModel_Organization
    Inherits jDeereDataModel_ItemGenerico
    Public Property type As String
    Public Property member As Boolean
    Public Property internal As Boolean
End Class

Public Class jDeereDataModel_FieldOperation
    Inherits jDeereDataModel_ItemGenerico

    Public Property fieldOperationType As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property adaptMachineType As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property cropSeason As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property startDate As DateTime
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property endDate As DateTime

    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property measurement As jDeereDataModel_GenericList(Of jDeereDataModel_FieldOperationMeasurement)

    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property Allegati_Documenti_Cod As Integer

    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property FieldID As Integer
End Class

Public Class jDeereDataModel_FieldOperationMeasurement
    Inherits jDeereDataModel_ItemGenerico

    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property measurementName As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property measurementCategory As String
    Public Property area As Decimal
    Public Property area_unit As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property totalMaterial As Decimal
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property totalMaterial_unit As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property averageMaterial As Decimal
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property averageMaterial_unit As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property yield As Decimal
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property yield_unit As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property averageYield As Decimal
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property averageYield_unit As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property averageMistoure As Decimal
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property averageMistoure_unit As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property wetMass As Decimal
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property wetMass_unit As String
    Public Property averageWetMass As Decimal
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property averageWetMass_unit As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property averageSpeed As Decimal
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property averageSpeed_unit As String

    <JsonIgnore()>
    Public Property FieldOperationID As Integer

End Class

Public Class jDeereDataModel_webhook_CFG

    Public Property RefreshOrganization As Boolean
    Public Property RefreshClient As Boolean
    Public Property RefreshFarm As Boolean
    Public Property RefreshField As Boolean
    Public Property RefreshOperation As Boolean
    Public Property RefreshBoundary As Boolean
    Public Property RefreshAssett As Boolean
    Public Property RefreshMachine As Boolean

End Class

Public Class jDeereDataModel_FieldOperationFileRequest
    Public Property ID As Integer
    Public Property FieldOperationID As Integer
    Public Property FieldOperationGUID As String
    Public Property Allegati_Documenti_Cod As Integer
    Public Property RequestState As Integer
    Public Property OAuth_Token As String
    Public Property Request_Parameters As String
    Public Property Esito As String

    Public Sub New()
        ID = 0
        FieldOperationID = 0
        FieldOperationGUID = ""
        Allegati_Documenti_Cod = 0
        RequestState = 0
        OAuth_Token = ""
        Request_Parameters = ""
        Esito = ""
    End Sub
End Class

Public Class jDeereDataModel_FieldOperationFileRequest_Parameter
    Public Property oauth_cfg As jDeereDataModel_ApiCFG
    Public Property fieldOpGuid As String

    Public Sub New()
        oauth_cfg = Nothing
        fieldOpGuid = ""
    End Sub
End Class

#Region "Classi per richieste POST/PUT/DELETE"
Public Class jDeereDataModel_Field_REQ
    Inherits jDeereDataModel_ItemGenerico
    Public Property archived As Boolean
    Public Property clients As jDeereDataModel_ClientList_REQ
    Public Property farms As jDeereDataModel_FarmList_REQ

    <JsonIgnore()>
    Public Property action As jDeereDataModel_RequestType

    Public Sub New()
        archived = False
        clients = New jDeereDataModel_ClientList_REQ()
        farms = New jDeereDataModel_FarmList_REQ()
        action = jDeereDataModel_RequestType.Insert
    End Sub

End Class

Public Class jDeereDataModel_ClientList_REQ
    Public Property clients As List(Of jDeereDataModel_Client_REQ)

    Public Sub New()
        clients = New List(Of jDeereDataModel_Client_REQ)
    End Sub

End Class

Public Class jDeereDataModel_FarmList_REQ
    Public Property farms As List(Of jDeereDataModel_Farm_REQ)

    Public Sub New()
        farms = New List(Of jDeereDataModel_Farm_REQ)
    End Sub
End Class

Public Class jDeereDataModel_Boundary_REQ
    Inherits jDeereDataModel_ItemGenerico
    <JsonProperty("@type", NullValueHandling:=NullValueHandling.Ignore)>
    Public Property Type As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property sourceType As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property multipolygons As List(Of jDeereDataModel_Polygon)
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property active As Boolean
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property irrigated As Boolean

    <JsonIgnore()>
    Public Property action As jDeereDataModel_RequestType

    Public Sub New()
        Type = "Boundary"
        sourceType = ""
        active = True
        irrigated = False
        multipolygons = New List(Of jDeereDataModel_Polygon)
        action = jDeereDataModel_RequestType.Insert
    End Sub
End Class



Public Class jDeereDataModel_Farm_REQ
    Inherits jDeereDataModel_ItemGenerico

End Class

Public Class jDeereDataModel_Client_REQ
    Inherits jDeereDataModel_ItemGenerico
End Class

Public Class jDeereDataModel_File
    Inherits jDeereDataModel_ItemGenerico

    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property delayProcessing As Boolean
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property archived As Boolean
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property StatoInvio As Boolean
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property EsitoInvio As String

    Public Sub New()
        name = ""
        delayProcessing = Nothing
        archived = Nothing
        StatoInvio = Nothing
        EsitoInvio = Nothing
    End Sub
End Class

#End Region