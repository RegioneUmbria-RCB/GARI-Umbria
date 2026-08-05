Imports System.Runtime.Serialization

'Public Class Impresa
'    <DataMember()> _
'    Public Property StringValue() As String
'End Class



<DataContract()>
Public Class Specie
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value>1 scrittura - 2 Modifica</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Codice_Specie() As Integer

    <DataMember()>
    Public Property Descrizione() As String

End Class
