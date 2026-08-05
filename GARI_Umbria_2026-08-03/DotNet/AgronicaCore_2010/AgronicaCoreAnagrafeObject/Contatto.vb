Imports System.Runtime.Serialization

'Public Class Impresa
'    <DataMember()> _
'    Public Property StringValue() As String
'End Class



<DataContract()>
Public Class Contatto
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value>1 scrittura - 2 Modifica</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Tipo_Operazione() As Integer

    <DataMember()>
    Public Property Partita_Iva() As String

    <DataMember()>
    Public Property Ragione_Sociale() As String

    <DataMember()>
    Public Property Codice_Fiscale() As String

    <DataMember()>
    Public Property Nome() As String

    <DataMember()>
    Public Property Cognome() As String

    <DataMember()>
    Public Property Sesso() As String

    <DataMember()>
    Public Property Email() As String

    <DataMember()>
    Public Property Fax() As String

    <DataMember()>
    Public Property Telefono() As String

    <DataMember()>
    Public Property Nascita_data() As String

    <DataMember()>
    Public Property Nascita_Indirizzo() As Indirizzo

    <DataMember()>
    Public Property Residenza_Indirizzo() As Indirizzo

    <DataMember()>
    Public Property Validita_Inizio() As String

    <DataMember()>
    Public Property Validita_Fine() As String

    <DataMember()>
    Public Property Ruolo() As List(Of Ruolo)

End Class
