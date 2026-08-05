Imports System.Runtime.Serialization

'Public Class Impresa
'    <DataMember()> _
'    Public Property StringValue() As String
'End Class



<DataContract()>
Public Class Indirizzo
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value>1 scrittura - 2 Modifica</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Tipo_Operazione() As Integer

    <DataMember()>
    Public Property Flag_Fornitore_Fatturazione() As Boolean

    <DataMember()>
    Public Property Codice_Navgreen() As String

    <DataMember()>
    Public Property Tipo_Indirizzo() As String

    <DataMember()>
    Public Property Cod_Indirizzo() As Integer

    <DataMember()>
    Public Property Via() As String

    <DataMember()>
    Public Property Frazione() As String

    <DataMember()>
    Public Property Cap() As String

    <DataMember()>
    Public Property Comune() As String

    <DataMember()>
    Public Property Provincia() As String

    <DataMember()>
    Public Property Stato() As String

    <DataMember()>
    Public Property Note_Indirizzo() As String

    <DataMember()>
    Public Property Codice_istat_Provincia() As String

    <DataMember()>
    Public Property Codice_istat_Comune() As String

    Public Sub New()

    End Sub
    Public Sub New(ByVal _Flag_Fornitore_Fatturazione As Boolean, _
                   ByVal _Codice_Navgreen As String, _
                   ByVal _Via As String, _
                         ByVal _Cap As String, _
                         ByVal _Comune As String, _
                         ByVal _Provincia As String, _
                         ByVal _Codice_Istat_Provincia As String, _
                         ByVal _Codice_Istat_Comune As String, _
                         ByVal _Stato As String
                         )
        Codice_Navgreen = _Codice_Navgreen
        Flag_Fornitore_Fatturazione = _Flag_Fornitore_Fatturazione

        Tipo_Indirizzo = 1
        Cod_Indirizzo = 0
        Via = _Via
        Cap = _Cap
        Comune = _Comune
        Provincia = _Provincia
        Codice_istat_Comune = _Codice_Istat_Comune
        Codice_istat_Provincia = _Codice_Istat_Provincia
        Stato = _Stato
    End Sub

End Class
