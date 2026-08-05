Public Class Analisi_API_Richiesta_Analisi
    Public Property listaRichiestaAnalisi As List(Of Analisi_API_Richiesta)
End Class

Public Class Analisi_API_Richiesta
    Public Property richiesta As List(Of Analisi_API_ChiaveValore)
End Class

Public Class Analisi_API_ChiaveValore
    Public Property chiave As String
    Public Property valore As String
End Class

Public Class Analisi_API_Richiesta_Response
    Public Property esitoGlobale As Boolean
    Public Property message As String
    Public Property listaRichiestaElaborata As List(Of Analisi_API_Richiesta_Dettaglio_Response)
    Public Property listaRichiestaAnalisiElaborata As List(Of Analisi_API_Richiesta_Dettaglio_Response)
End Class

Public Class Analisi_API_Richiesta_Dettaglio_Response
    Public Property pivaSuperUser As String
    Public Property chiave As String
    Public Property messaggioErrore As String
End Class

'TODO: da oggetto nostra richiesta di analisi lo devo trasformare in JSON per invio PedonLab