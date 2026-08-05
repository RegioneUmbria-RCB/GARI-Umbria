Imports System.Runtime.Serialization

'Public Class Impresa
'    <DataMember()> _
'    Public Property StringVale() As String
'End Class



<DataContract()>
Public Class Particelle

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value>1 scrittura - 2 Modifica</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Provincia() As String

    <DataMember()>
    Public Property Comune() As String

    <DataMember()>
    Public Property Sezione() As String

    <DataMember()>
    Public Property Foglio() As String

    <DataMember()>
    Public Property Numero() As Decimal

    <DataMember()>
    Public Property SubAlterno() As String

    <DataMember()>
    Public Property Area_Condotta() As Decimal



End Class
