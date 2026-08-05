Public Class ValidationAttribute : Inherits Attribute

    Private ReadOnly _validatorType As Type
    Private ReadOnly _objectType As Type
    Private ReadOnly _order As Integer
    Private ReadOnly _riferimento As String
    Private ReadOnly _soggetto As EnuValidatoreSoggetto

    Public Property ObjectToValidate() As Object
    Public Property Mapper() As IDecodificheMapper
    Public ReadOnly Property Order() As Integer
        Get
            Return _order
        End Get
    End Property

    Public ReadOnly Property Name() As String
        Get
            Return _validatorType.Name
        End Get
    End Property

    Public Sub New()
        MyBase.New()
    End Sub
    Public Sub New(ByVal order As Integer,
                   ByVal validatorType As Type,
                   ByVal objectType As Type,
                   ByVal riferimento As String,
                   ByVal soggetto As EnuValidatoreSoggetto)

        _validatorType = validatorType
        _objectType = objectType
        _order = order
        _riferimento = riferimento
        _soggetto = soggetto
    End Sub
    Public Function Validate(ByVal oggettoDaValidare As Object, ByVal fattura As FatturaGias, ByRef listaErrori As List(Of String)) As Boolean

        Dim val = DirectCast(Activator.CreateInstance(_validatorType, _objectType, _riferimento, _soggetto, Mapper), IFatturaValidator)
        Return val.Validate(oggettoDaValidare, fattura, listaErrori)

    End Function

    Public Function Validate(ByRef listaErrori As List(Of String), ByVal fattura As FatturaGias) As Boolean

        Dim val = DirectCast(Activator.CreateInstance(_validatorType, _objectType, _riferimento, _soggetto, Mapper), IFatturaValidator)
        If val.AllowNullObject Then
            Return val.Validate(ObjectToValidate, fattura, listaErrori)
        Else
            If ObjectToValidate Is Nothing Then
                Return False
            Else
                Return val.Validate(ObjectToValidate, fattura, listaErrori)
            End If
        End If

    End Function

End Class
