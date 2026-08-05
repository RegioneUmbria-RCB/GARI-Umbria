Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class DocumentoPerScarico

    Public Property Piva_Superuser As String
    Public Property Piva As String
    Public Property Documento_Cod As Integer
    Public Property ID_Tipologia As Integer
    Public Property Data_Scadenza As Date
    Public Property Descrizione As String
    Public Property Note As String
    Public Property Allegati As List(Of DocumentoAllegato)
    Public Property Visita_Cod As Integer
    Public Property Attivita_Cod As String
    Public Property guid As String
    Public Property cancellato As Boolean
    Public Property Ricetta_Operazione_Cod As Integer
    Public Property Ricetta_Destinazione_Cod As Integer
End Class




Public Class DocumentoPerImport

    Public Property CUAA As String
    Public Property dati As String

End Class


Public Class DatiDocumentoAllegatoPerImport

    Public Property Documenti As List(Of DocumentoAllegatoPerImport)

End Class


