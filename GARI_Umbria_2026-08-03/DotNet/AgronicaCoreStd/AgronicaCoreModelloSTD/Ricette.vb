
Imports AgronicaCoreDataProviderSTD.TipiEnumerativi

Public Class Ricette

    Public Property TipoOperazioneDB As enum_TipoOperazioneDB

    Public Property ricetta_cod As Integer
    Public Property ricetta_des As String
    Public Property ricetta_numero As String
    Public Property note As String
    Public Property Tipo_Ricetta As enum_TipoRicetta_DB

    Public Property RicetteOperazioni As List(Of Ricette_Operazioni)

End Class
