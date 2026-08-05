Imports NetTopologySuite.Features

Public Class AttributeRecord
    Implements IDisposable
    Private _attributes As List(Of AttributeField)

    Public ReadOnly Attributes As List(Of AttributeField)

    Public Sub New()
        _attributes = New List(Of AttributeField)
    End Sub

    Public Sub AddColumn(ByVal key As String, ByVal type As AttributeFieldType, ByVal value As Object, Optional ByVal lunghezza As Integer = 19, Optional ByVal precisione As Integer = 17)
        If (_attributes.Where(Function(x) x.Nome = key).FirstOrDefault() IsNot Nothing) Then
            Throw New Exception("Esiste già una colonna con lo stesso nome")
        End If
        _attributes.Add(New AttributeField(key, type, lunghezza, precisione) With {.Value = value})
    End Sub

    Public Function GetValueAsString(ByVal key As String) As String
        Return CType(getColumnValue(key), String)
    End Function
    Public Function GetValueAsDate(ByVal key As String) As Date
        Return CType(getColumnValue(key), Date)
    End Function
    Public Function GetValueAsBoolean(ByVal key As String) As Boolean
        Return CType(getColumnValue(key), Boolean)
    End Function
    Public Function GetValueAsInteger(ByVal key As String) As Int32
        Return CType(getColumnValue(key), Int32)
    End Function
    Public Function GetValueAsInt64(ByVal key As String) As Int64
        Return CType(getColumnValue(key), Int64)
    End Function
    Public Function GetValueAsDouble(ByVal key As String) As Double
        Return CType(getColumnValue(key), Double)
    End Function
    Public Function GetValueAsDecimal(ByVal key As String) As Decimal
        Return CType(getColumnValue(key), Decimal)
    End Function

    Public Sub UpdateColumn(ByVal key As String, ByVal value As Object)
        Dim colref = _attributes.Where(Function(x) x.Nome = key).FirstOrDefault()
        If colref Is Nothing Then
            Throw New Exception("Colonna " + key + " non trovata")
        End If
        colref.Value = value
    End Sub

    Public Sub DeleteColumn(ByVal key As String)
        Dim colref = _attributes.Where(Function(x) x.Nome = key).FirstOrDefault()
        If colref Is Nothing Then
            Throw New Exception("Colonna " + key + " non trovata")
        End If

        _attributes.Remove(colref)
    End Sub

    Public Function AsAttributeTable() As AttributesTable
        Dim ret As New AttributesTable
        Try
            For Each att In _attributes
                ret.Add(att.Nome, att.Value)
            Next
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        _attributes.Clear()
        _attributes = Nothing
    End Sub


#Region "Internal"
    Private Function getColumnValue(ByVal key As String) As Object
        Dim colref = _attributes.Where(Function(x) x.Nome = key).FirstOrDefault()
        If colref Is Nothing Then
            Throw New Exception("Colonna " + key + " non trovata")
        End If

        Return colref.Value
    End Function

#End Region
End Class

Public Class AttributeField
    Private _value As Object

    Public ReadOnly Nome As String
    Public ReadOnly Type As AttributeFieldType
    Public ReadOnly Length As Integer
    Public ReadOnly Precision As Integer

    Public Property Value
        Set(value As Object)
            Try
                Select Case Type
                    Case AttributeFieldType._Character
                        _value = CType(value, String)
                    Case AttributeFieldType._Date
                        _value = CType(value, Date)
                    Case AttributeFieldType._Double
                        _value = Math.Round(CType(value, Double), Precision)
                    Case AttributeFieldType._Float
                        _value = Math.Round(CType(value, Decimal), Precision)
                    Case AttributeFieldType._Int32
                        _value = CType(value, Int32)
                    Case AttributeFieldType._Int64
                        _value = CType(value, Int64)
                    Case AttributeFieldType._Logical
                        _value = CType(value, Boolean)
                End Select
            Catch ex As Exception
                Throw New Exception(ex.Message, ex)
            End Try
        End Set
        Get
            Try
                Select Case Type
                    Case AttributeFieldType._Character
                        Return CType(_value, String)
                    Case AttributeFieldType._Date
                        Return CType(_value, Date)
                    Case AttributeFieldType._Double
                        Return Math.Round(CType(_value, Double), Precision)
                    Case AttributeFieldType._Float
                        Return Math.Round(CType(_value, Decimal), Precision)
                    Case AttributeFieldType._Int32
                        Return CType(_value, Int32)
                    Case AttributeFieldType._Int64
                        Return CType(_value, Int64)
                    Case AttributeFieldType._Logical
                        Return CType(_value, Boolean)
                End Select
            Catch ex As Exception
                Throw New Exception(ex.Message, ex)
            End Try
        End Get
    End Property

    Public Sub New(ByVal nome As String,
                   ByVal tipo As AttributeFieldType,
                   ByVal lunghezza As Integer,
                   ByVal decimali As Integer)
        Me.Nome = nome
        Me.Type = tipo
        If tipo <> AttributeFieldType._Date AndAlso lunghezza <= 0 Then
            Throw New Exception("La lunghezza del campo deve essere maggiore di zero.")
        End If
        If {AttributeFieldType._Float, AttributeFieldType._Double, AttributeFieldType._Int32, AttributeFieldType._Int64}.Contains(tipo) And (lunghezza > 19 Or decimali > 17 Or decimali < 0) Then
            Throw New Exception("I campi di tipo numerico possono avere una lunghezza massima di 19 ed una precisione massima di 17")
        End If
        Select Case Type
            Case AttributeFieldType._Character
                If lunghezza > 254 Then
                    Throw New Exception("La lunghezza del campo deve essere compresa tra 1 e 254")
                End If
                Me.Length = lunghezza
                Me.Precision = 0
            Case AttributeFieldType._Date
                Me.Length = 8
                Me.Precision = 0
            Case AttributeFieldType._Int32
                Me.Length = lunghezza
                Me.Precision = 0
            Case AttributeFieldType._Int64
                Me.Length = lunghezza
                Me.Precision = 0
            Case AttributeFieldType._Double
                Me.Length = lunghezza
                Me.Precision = decimali
            Case AttributeFieldType._Float
                Me.Length = lunghezza
                Me.Precision = decimali
            Case AttributeFieldType._Logical
                Me.Length = 1
                Me.Precision = 0
        End Select
    End Sub
End Class

Public Enum AttributeFieldType
    _Character = 0
    _Date = 1
    _Int32 = 2
    _Int64 = 3
    _Double = 4
    _Float = 5
    _Logical = 6
End Enum