

Public Class Pratiche_Stati_Attuali

    '##############################################################################################
    Public Function LeggiListaPerImpresa(
        ByVal piva As String,
        ByVal WWorkflow_Cod As Integer,
        ByVal Servizio_Cod As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As String


        Dim lettura As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_R
        Dim dtL As DataTable =
            lettura.LeggiListaPerImpresa(piva, WWorkflow_Cod, Servizio_Cod, "", "", objParametri_server, objParametri_Utenti)

        Dim lista As List(Of String) = (From d In dtL.AsEnumerable
                                        Select "{" &
                                            " ""Codice"": """ & CStr(d("Codice")) & """," &
                                            " ""Descrizione"": """ & CStr(d("Descrizione")) & """, " &
                                            " ""Colore"": """ & CStr(d("Colore")) & """ " &
                                            " }").ToList()

        Return "[" & String.Join(",", lista.ToArray) & "]"
    End Function

End Class
