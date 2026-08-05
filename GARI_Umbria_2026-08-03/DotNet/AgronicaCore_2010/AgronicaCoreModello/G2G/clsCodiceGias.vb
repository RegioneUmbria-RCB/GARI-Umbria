'#################################################################
Public Class clsCodiceGias

    Private _ProgressivoGias As Integer
    Private _BaseCode As Integer
    Private _TopCode As Integer

#Region "Costruttori"

    Public Sub New()
        _ProgressivoGias = 0
        _BaseCode = 0
        _TopCode = 0
    End Sub

    Public Sub New(ByVal ProgressivoGias As Integer)
        _ProgressivoGias = ProgressivoGias
        _BaseCode = AgronicaCoreDataProvider.UtilityProvider.BaseCode_from_ProgressivoGias(ProgressivoGias)
        _TopCode = AgronicaCoreDataProvider.UtilityProvider.TopCode_from_ProgressivoGias(ProgressivoGias)
    End Sub

    Public Sub New(ByVal ProgressivoGias As Integer, ByVal BaseCode As Integer, ByVal TopCode As Integer)
        _ProgressivoGias = ProgressivoGias
        _BaseCode = BaseCode
        _TopCode = TopCode
    End Sub

#End Region

#Region "Proprieta"

    Public Property ProgressivoGias() As String
        Get
            Return _ProgressivoGias
        End Get
        Set(ByVal value As String)
            _ProgressivoGias = value
        End Set
    End Property

    Public Property BaseCode() As String
        Get
            Return _BaseCode
        End Get
        Set(ByVal value As String)
            _BaseCode = value
        End Set
    End Property

    Public Property TopCode() As String
        Get
            Return _TopCode
        End Get
        Set(ByVal value As String)
            _TopCode = value
        End Set
    End Property

#End Region

End Class
