
Imports System.Web.Script.Serialization

Public Class KendoDataHelper


    Public Sub ListaFlatElementiCheck(ByVal StartHierarchicalDataSource As KendoHierarchicalDataSource, ByRef OutputList As List(Of String))

        If StartHierarchicalDataSource.checked AndAlso StartHierarchicalDataSource.Id <> "" Then
            OutputList.Add(StartHierarchicalDataSource.Id)
        End If

        If Not StartHierarchicalDataSource.items Is Nothing Then

            For Each Child In StartHierarchicalDataSource.items
                ListaFlatElementiCheck(Child, OutputList)
            Next
        End If

    End Sub

End Class

Public Class KendoHierarchicalDataSource


    Private _id As String

    Public text As String
    Public expanded As Boolean
    Public imageUrl As String
    <ScriptIgnore>
    Public checked As Boolean
    Public style As String
    Public items As List(Of KendoHierarchicalDataSource)
    Public type As String

    Public Veg_Cod As Integer
    Public Id_Cod As Integer

    Private _startDate As Date = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
    Private _endDate As Date = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE

    ''' <summary>
    ''' mantenere in minuscolo per compatibilità su oggetto serializzato lato client
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property value As String
        Get
            Return _id
        End Get
    End Property

    ''' <summary>
    ''' mantenere in minuscolo per compatibilità su oggetto serializzato lato client
    ''' </summary>
    ''' <returns></returns>
    Public Property id As String
        Get
            Return _id
        End Get
        Set(value As String)
            _id = value
        End Set
    End Property

    Public Property startDate() As Date
        Get
            Return _startDate
        End Get
        Set(ByVal value As Date)
            _startDate = value
        End Set
    End Property

    Public Property endDate() As Date
        Get
            Return _endDate
        End Get
        Set(ByVal value As Date)
            _endDate = value
        End Set
    End Property
End Class