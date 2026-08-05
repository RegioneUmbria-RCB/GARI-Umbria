



Public Class AjaxTreeNode_FAST_JsonObject
    Private _id As String
    Private _text As String
    Private _icon As String = String.Empty
    Private _state As New AjaxTreeNode_FAST_JsonObjectState
    Private _children As List(Of AjaxTreeNode_FAST_JsonObject)

    Private _attr_li As New AjaxTreeNode_FAST_JsonObjectAttribute
    Private _attr_a As New AjaxTreeNode_FAST_JsonObjectAttribute


    Public Property id() As String
        Get
            Return _id
        End Get
        Set(ByVal value As String)
            _id = value
        End Set
    End Property

    Public Property text() As String
        Get
            Return _text
        End Get
        Set(ByVal value As String)
            _text = value
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

    Public ReadOnly Property state() As AjaxTreeNode_FAST_JsonObjectState
        Get
            Return _state
        End Get
    End Property
      
    Public Property children() As List(Of AjaxTreeNode_FAST_JsonObject)
        Get
            Return _children
        End Get
        Set(ByVal value As List(Of AjaxTreeNode_FAST_JsonObject))
            _children = value
        End Set
    End Property





    Public Sub New()
    End Sub

    Public Sub New(ByVal id As String, ByVal title As String, ByVal href As String)
        Me.New(id, title, href, "", "", False)
    End Sub




    Public Sub New(ByVal id As String, ByVal title As String, ByVal classCss As String, ByVal style As String, ByVal href As String, ByVal oggetto_generico As Object)
        Dim b As New Boolean
        If oggetto_generico.GetType.ToString = b.GetType.ToString Then
            inizializza(id, title, classCss, style, href, CType(oggetto_generico, Boolean))
        Else
            inizializza(id, title, classCss, style, href, CType(oggetto_generico, List(Of AjaxTreeNode_FAST_JsonObject)))
        End If

    End Sub

    Public Sub inizializza(ByVal id As String, ByVal text As String, ByVal [class] As String, ByVal style As String, ByVal href As String, ByVal hasChildren As Boolean)
        'li values
        Me.id = id
        Me.text = text
        Me._attr_li.[class] = [class]
        Me._attr_li.style = style
        'a values
        Me._attr_a.href = href
        Me._attr_a.id = "a" & Me.id
        'Me._attr_a.text =text 

        If hasChildren Then
            Me.state.opened = "false"
        End If

    End Sub


    Public Sub inizializza(ByVal id As String, ByVal text As String, ByVal [class] As String, ByVal style As String, ByVal href As String, ByRef children As List(Of AjaxTreeNode_FAST_JsonObject))
        'li values
        Me.id = id
        Me.text = text
        Me._attr_li.[class] = [class]
        Me._attr_li.style = style
        'a values
        Me._attr_a.href = href
        Me._attr_a.id = "a" & Me.id
        'Me.data.title = Me.attr.title

        If children IsNot Nothing Then
            Me.state.opened = "false"
            Me.children = children
        End If

    End Sub

End Class



Public Class AjaxTreeNode_FAST_JsonObjectState
    Private _opened As String = "false"
    Private _disable As String = "false"
    Private _selected As String = "false"

    Public Property opened() As String
        Get
            Return Me._opened
        End Get
        Set(ByVal value As String)
            Me._opened = value
        End Set
    End Property
    Public Property disable() As String
        Get
            Return Me._disable
        End Get
        Set(ByVal value As String)
            Me._disable = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the Id value
    ''' </summary>
    Public Property selected() As String
        Get
            Return Me._selected
        End Get
        Set(ByVal value As String)
            Me._selected = value
        End Set
    End Property
     
End Class





Public Class AjaxTreeNode_FAST_JsonObjectAttribute
    Private _id As String = String.Empty
    Private _title As String = String.Empty
    Private _style As String = String.Empty
    Private _class As String = String.Empty
    Private _href As String = String.Empty

    Public Property href() As String
        Get
            Return Me._href
        End Get
        Set(ByVal value As String)
            Me._href = value
        End Set
    End Property

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

Public Class AjaxTreeNode_FAST_JsonObjectDataLinkAttribute

    Private _title As String = String.Empty
    Private _attr As New AjaxTreeNode_FAST_JsonObjectLinkAttribute

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

    Public Property attr() As AjaxTreeNode_FAST_JsonObjectLinkAttribute
        Get
            Return _attr
        End Get
        Set(ByVal value As AjaxTreeNode_FAST_JsonObjectLinkAttribute)
            _attr = value
        End Set
    End Property

End Class

Public Class AjaxTreeNode_FAST_JsonObjectLinkAttribute
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