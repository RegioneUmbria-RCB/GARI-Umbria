Public Class GruppoAppezzamentoWS
    Public Property nextKey As String
    Public Property count As Integer
    Public Property records As List(Of GruppoAppezzamentoIntestazione)

    Public Sub New()
        records = New List(Of GruppoAppezzamentoIntestazione)
    End Sub
End Class

Public Class GruppoAppezzamentoIntestazione
    Public Property codice As String
    Public Property codice_esterno As String
    Public Property tipo_modifica As String
    Public Property elemento_anagrafico As GruppoAppezzamentoDettagli
End Class

Public Class GruppoAppezzamentoDettagli
    Public Property descrizione As String
    Public Property flag_cancellazione As Boolean?
    Public Property data_eliminazione As Date?
    Public Property ultimo_aggiornamento As Date?
    Public Property appezzamenti As List(Of AppezzamentoInGruppo)
End Class

Public Class AppezzamentoInGruppo
    Public Property id_appezzamento As String
    'Classe fatta per eventuali implementazioni future, al momento sarebbe bastata la sola proprietà come List(of String) sulla classe GruppoAppezzamentoDettagli
End Class