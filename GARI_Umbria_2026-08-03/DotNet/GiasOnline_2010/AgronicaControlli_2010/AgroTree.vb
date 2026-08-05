


Public Class AjaxTreeNodeJsonObjectConverter

    Public Shared Sub AjaxTreeNodeJsonObject_KendoHierarchical(ByVal t As List(Of AjaxTreeNode_FAST_JsonObject), ByRef kendoHierarchical As AgronicaCoreDataProvider.KendoHierarchicalDataSource)


        For Each curNodo As AjaxTreeNode_FAST_JsonObject In t
            AjaxTreeNodeJsonObject_KendoHierarchical(curNodo, kendoHierarchical, Nothing)
        Next

    End Sub
    Public Shared Sub AjaxTreeNodeJsonObject_KendoHierarchical(ByVal t As List(Of AjaxTreeNodeJsonObject), ByRef kendoHierarchical As AgronicaCoreDataProvider.KendoHierarchicalDataSource)


        For Each curNodo As AjaxTreeNodeJsonObject In t
            AjaxTreeNodeJsonObject_KendoHierarchical(curNodo, kendoHierarchical, Nothing)
        Next

    End Sub

    Public Shared Sub AjaxTreeNodeJsonObject_KendoHierarchical(
        ByVal t As AjaxTreeNode_FAST_JsonObject,
        ByRef kendoHierarchical As AgronicaCoreDataProvider.KendoHierarchicalDataSource,
        ByRef listaAppendi As List(Of AgronicaCoreDataProvider.KendoHierarchicalDataSource))



        CopiaTreenodeKendoHierarchical(t, kendoHierarchical)

        If Not listaAppendi Is Nothing Then
            listaAppendi.Add(kendoHierarchical)
        End If

        If Not t.children Is Nothing AndAlso t.children.Count > 0 Then

            If kendoHierarchical.items Is Nothing Then
                kendoHierarchical.items = New List(Of AgronicaCoreDataProvider.KendoHierarchicalDataSource)
            End If


            For Each curChildred In t.children
                AjaxTreeNodeJsonObject_KendoHierarchical(curChildred, New AgronicaCoreDataProvider.KendoHierarchicalDataSource, kendoHierarchical.items)
            Next

        End If

    End Sub
    Public Shared Sub AjaxTreeNodeJsonObject_KendoHierarchical(
        ByVal t As AjaxTreeNodeJsonObject,
        ByRef kendoHierarchical As AgronicaCoreDataProvider.KendoHierarchicalDataSource,
        ByRef listaAppendi As List(Of AgronicaCoreDataProvider.KendoHierarchicalDataSource))


        CopiaTreenodeKendoHierarchical(t, kendoHierarchical)

        If Not listaAppendi Is Nothing Then
            listaAppendi.Add(kendoHierarchical)
        End If

        If Not t.children Is Nothing AndAlso t.children.Count > 0 Then

            If kendoHierarchical.items Is Nothing Then
                kendoHierarchical.items = New List(Of AgronicaCoreDataProvider.KendoHierarchicalDataSource)
            End If


            For Each curChildred In t.children
                AjaxTreeNodeJsonObject_KendoHierarchical(curChildred, New AgronicaCoreDataProvider.KendoHierarchicalDataSource, kendoHierarchical.items)
            Next

        End If

    End Sub

    Public Shared Sub CopiaTreenodeKendoHierarchical(aTree As AjaxTreeNode_FAST_JsonObject, ByRef kHierarchical As AgronicaCoreDataProvider.KendoHierarchicalDataSource)

        kHierarchical.id = aTree.id
        kHierarchical.text = aTree.text
        kHierarchical.expanded = aTree.state.opened
        kHierarchical.imageUrl = aTree.icon
        kHierarchical.style = ""

    End Sub


    Public Shared Sub CopiaTreenodeKendoHierarchical(aTree As AjaxTreeNodeJsonObject, ByRef kHierarchical As AgronicaCoreDataProvider.KendoHierarchicalDataSource)

        kHierarchical.id = aTree.attr.id
        kHierarchical.text = aTree.attr.title
        kHierarchical.expanded = True
        kHierarchical.imageUrl = aTree.icon
        kHierarchical.style = aTree.attr.style
        kHierarchical.type = aTree.type
        kHierarchical.startDate = aTree.startDate
        kHierarchical.endDate = aTree.endDate

        kHierarchical.Veg_Cod = aTree.Veg_Cod
        kHierarchical.Id_Cod = aTree.Id_Cod

    End Sub

End Class

Public Class AjaxTreeNodeJsonObject

    Private _attr As New AjaxTreeNodeJsonObjectAttribute
    Private _state As String = String.Empty
    Private _icon As String = String.Empty
    Private _metadata As String = String.Empty
    Private _data As New AjaxTreeNodeJsonObjectDataLinkAttribute
    Private _children As List(Of AjaxTreeNodeJsonObject)
    Private _type As String = String.Empty
    Private _startDate As Date = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
    Private _endDate As Date = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE
    Private _veg_cod As Integer = 0
    Private _id_cod As Integer = 0

    Public ReadOnly Property attr() As AjaxTreeNodeJsonObjectAttribute
        Get
            Return _attr
        End Get
    End Property

    Public ReadOnly Property data() As AjaxTreeNodeJsonObjectDataLinkAttribute
        Get
            Return _data
        End Get
    End Property

    Public Property state() As String
        Get
            Return _state
        End Get
        Set(ByVal value As String)
            _state = value
        End Set
    End Property

    Public Property icon() As String
        Get
            Return _icon
        End Get
        Set(ByVal value As String)
            _icon = value
        End Set
    End Property

    Public Property metadata() As String
        Get
            Return _metadata
        End Get
        Set(ByVal value As String)
            _metadata = value
        End Set
    End Property

    Public Property children() As List(Of AjaxTreeNodeJsonObject)
        Get
            Return _children
        End Get
        Set(ByVal value As List(Of AjaxTreeNodeJsonObject))
            _children = value
        End Set
    End Property

    Public Property type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = value
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

    Public Property Veg_Cod() As String
        Get
            Return _veg_cod
        End Get
        Set(ByVal value As String)
            _veg_cod = value
        End Set
    End Property

    Public Property Id_Cod() As String
        Get
            Return _id_cod
        End Get
        Set(ByVal value As String)
            _id_cod = value
        End Set
    End Property

    Public Sub New()
    End Sub

    Public Sub New(ByVal id As String, ByVal title As String, ByVal href As String)
        Me.New(id, title, href, "", "", False)
    End Sub




    Public Sub New(ByVal id As String,
                   ByVal title As String,
                   ByVal classCss As String,
                   ByVal style As String,
                   ByVal href As String,
                   ByVal oggetto_generico As Object
                   )

        Dim b As New Boolean
        If oggetto_generico.GetType.ToString = b.GetType.ToString Then
            inizializza(id, title, classCss, style, href, CType(oggetto_generico, Boolean))
        Else
            inizializza(id, title, classCss, style, href, CType(oggetto_generico, List(Of AjaxTreeNodeJsonObject)))
        End If

    End Sub

    Public Sub New(ByVal id As String,
                   ByVal title As String,
                   ByVal classCss As String,
                   ByVal style As String,
                   ByVal href As String,
                   ByVal oggetto_generico As Object,
                   ByVal startDate As Date,
                   ByVal endDate As Date
                   )
        Dim b As New Boolean
        If oggetto_generico.GetType.ToString = b.GetType.ToString Then
            inizializza(id, title, classCss, style, href, CType(oggetto_generico, Boolean), startDate, endDate)
        Else
            inizializza(id, title, classCss, style, href, CType(oggetto_generico, List(Of AjaxTreeNodeJsonObject)), startDate, endDate)
        End If

    End Sub

    Public Sub inizializza(
                          ByVal id As String,
                          ByVal title As String,
                          ByVal [class] As String,
                          ByVal style As String,
                          ByVal href As String,
                          ByVal hasChildren As Boolean,
                          Optional startDate As Date = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO,
                          Optional endDate As Date = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE
                          )
        'li values
        Me.attr.id = id
        Me.attr.title = title
        Me.attr.[class] = [class]
        Me.attr.style = style
        'a values
        Me.data.attr.href = href
        Me.data.attr.id = "a" & Me.attr.id
        Me.data.title = Me.attr.title

        Me.startDate = startDate
        Me.endDate = endDate

        If hasChildren Then
            Me.state = "closed"
        End If

        Me._veg_cod = 0
        Me._id_cod = 0

    End Sub


    Public Overridable Sub inizializza(
                          ByVal id As String,
                          ByVal title As String,
                          ByVal [class] As String,
                          ByVal style As String,
                          ByVal href As String,
                          ByRef children As List(Of AjaxTreeNodeJsonObject),
                          Optional startDate As Date = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO,
                          Optional endDate As Date = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE
                          )
        'li values
        Me.attr.id = id
        Me.attr.title = title
        Me.attr.[class] = [class]
        Me.attr.style = style
        'a values
        Me.data.attr.href = href
        Me.data.attr.id = "a" & Me.attr.id
        Me.data.title = Me.attr.title

        If children IsNot Nothing Then
            Me.state = "closed"
            Me.children = children
        End If

        Me.startDate = startDate
        Me.endDate = endDate

    End Sub

End Class

Public Class AjaxTreeNodeJsonObjectAttribute
    Private _id As String = String.Empty
    Private _title As String = String.Empty
    Private _style As String = String.Empty
    Private _class As String = String.Empty

    Public Property style() As String
        Get
            Return Me._style
        End Get
        Set(ByVal value As String)
            Me._style = value
        End Set
    End Property
    Public Property [class]() As String
        Get
            Return Me._class
        End Get
        Set(ByVal value As String)
            Me._class = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the Id value
    ''' </summary>
    Public Property id() As String
        Get
            Return Me._id
        End Get
        Set(ByVal value As String)
            Me._id = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the Title value
    ''' </summary>
    Public Property title() As String
        Get
            Return Me._title
        End Get
        Set(ByVal value As String)
            Me._title = value
        End Set
    End Property

End Class

Public Class AjaxTreeNodeJsonObjectDataLinkAttribute

    Private _title As String = String.Empty
    Private _attr As New AjaxTreeNodeJsonObjectLinkAttribute

    ''' <summary>
    ''' Gets or sets the Title value
    ''' </summary>
    Public Property title() As String
        Get
            Return Me._title
        End Get
        Set(ByVal value As String)
            Me._title = value
        End Set
    End Property

    Public Property attr() As AjaxTreeNodeJsonObjectLinkAttribute
        Get
            Return _attr
        End Get
        Set(ByVal value As AjaxTreeNodeJsonObjectLinkAttribute)
            _attr = value
        End Set
    End Property

End Class

Public Class AjaxTreeNodeJsonObjectLinkAttribute
    Private _id As String = String.Empty
    Private _href As String = String.Empty
    Private _target As String = String.Empty

    ''' <summary>
    ''' Gets or sets the Id value
    ''' </summary>
    Public Property id() As String
        Get
            Return Me._id
        End Get
        Set(ByVal value As String)
            Me._id = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the Href value
    ''' </summary>
    Public Property href() As String
        Get
            Return Me._href
        End Get
        Set(ByVal value As String)
            Me._href = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the Target value
    ''' </summary>
    Public Property target() As String
        Get
            Return Me._target
        End Get
        Set(ByVal value As String)
            Me._target = value
        End Set
    End Property

End Class
