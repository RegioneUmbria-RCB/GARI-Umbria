Imports System.Xml
Imports AgGateway.ADAPT.ApplicationDataModel.ADM
Imports AgGateway.ADAPT.ApplicationDataModel.Common
Imports AgGateway.ADAPT.ApplicationDataModel.Logistics
Imports UniqueId = AgGateway.ADAPT.ApplicationDataModel.Common.UniqueId


Public Class SetupDataCreator
    Private _applicationDataModel As ApplicationDataModel

    Public Sub New()
        _applicationDataModel = New ApplicationDataModel With {
            .Catalog = New Catalog()
        }
    End Sub

    Public Function GetDataModel() As ApplicationDataModel
        Return _applicationDataModel
    End Function

    Public Sub AddClientFarmField(GrowerName As String, GrowerUUID As String, FarmDesc As String, FarmUUID As String, FieldName As String, FieldUUID As String)
        Dim _g = AddNewClient(GrowerName, GrowerUUID)
        Dim _f = AddNewFarm(FarmDesc, FarmUUID, _g)

        AddNewField(FieldName, FieldUUID, _g, _f)
    End Sub


    Private Sub AddNewField(Name As String, UUID As String, client As Grower, farm As Farm)
        Try
            Dim _field As New Field() With {
                .FarmId = farm.Id.ReferenceId,
                .GrowerId = client.Id.ReferenceId,
                .Description = Name
            }
            _field.Id.UniqueIds.Add(New UniqueId() With {.Id = UUID, .IdType = IdTypeEnum.UUID})
            _applicationDataModel.Catalog.Fields.Add(_field)
        Catch ex As Exception

        End Try
    End Sub

    Private Function AddNewClient(Name As String, UUID As String) As Grower
        Dim _grower As New Grower()
        Try
            _grower.Name = Name
            _grower.Id.UniqueIds.Add(New UniqueId() With {.Id = UUID, .IdType = IdTypeEnum.UUID})
            _applicationDataModel.Catalog.Growers.Add(_grower)
        Catch ex As Exception

        End Try
        Return _grower
    End Function

    Private Function AddNewFarm(Name As String, UUID As String, client As Grower) As Farm
        Dim _farm As New Farm()
        Try
            _farm.Description = Name
            _farm.GrowerId = client.Id.ReferenceId
            _farm.Id.UniqueIds.Add(New UniqueId() With {.Id = UUID, .IdType = IdTypeEnum.UUID})
            _applicationDataModel.Catalog.Farms.Add(_farm)
        Catch ex As Exception

        End Try
        Return _farm
    End Function
End Class
