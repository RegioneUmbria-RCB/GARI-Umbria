Imports Newtonsoft.Json

Public Class Anagrafica
    Public Property cuaa As String
    Public Property subject_id As Integer?
    Public Property tipo_anagrafica As String
    Public Property denominazione As String
    Public Property codice_fiscale As String
    Public Property partita_iva As String
    Public Property sede As Indirizzo
    Public Property centro_aziendale As Coordinate
    Public Property codice_op As String
    Public Property mandato As Mandato
    Public Property ultimo_aggiornamento As String
End Class

Public Class Mandato
    Public Property codice_detentore As String
    Public Property data_inizio_validita As String
    Public Property data_fine_validita As String
End Class

Public Class Indirizzo
    Public Property codice_belfiore As String
    Public Property comune As String
    Public Property indirizzo As String
    Public Property cap As String
End Class

Public Class Coordinate
    Public Property lat As Decimal
    <JsonProperty(PropertyName:="long")>
    Public Property lng As Decimal
End Class

Public Class AnagraficaChangeLog
    Public Property nextKey As String
    Public Property records As List(Of Anagrafica)
    Public Property count As Integer

    Public Sub New()
        records = New List(Of Anagrafica)
    End Sub
End Class