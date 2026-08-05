Imports System.Runtime.Serialization

'Public Class Impresa
'    <DataMember()> _
'    Public Property StringValue() As String
'End Class



<DataContract()>
Public Class Legale_Rappresentante
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value>1 scrittura - 2 Modifica</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Tipo_Operazione() As Integer

    <DataMember()>
    Public Property Nome_Cognome() As String

    <DataMember()>
    Public Property Codice_Fiscale() As String

    <DataMember()>
    Public Property Sesso() As String

    <DataMember()>
    Public Property Validita_Inizio() As String

    <DataMember()>
    Public Property Validita_Fine() As String

    <DataMember()>
    Public Property Indirizzo() As Indirizzo

    <DataMember()>
    Public Property Documenti() As String

    <DataMember()>
    Public Property Rubrica() As String

    <DataMember()>
    Public Property Rubrica_1() As String

    <DataMember()>
    Public Property Rubrica_2() As String

    <DataMember()>
    Public Property Rubrica_3() As String

    <DataMember()>
    Public Property Rubrica_4() As String

    <DataMember()>
    Public Property Rubrica_5() As String

End Class
