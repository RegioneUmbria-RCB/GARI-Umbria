Imports SincroAnagrafeBA


Public Class SchedaValidazione_R

    ''' <summary>
    ''' Legge un elenco di schede validazione da sigpa, restituisce una lista in formato BA
    ''' </summary>
    ''' <param name="CUAA"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SchedeFascicolo( _
        ByVal EnteValidatore_COD As Integer, _
        ByVal DataDa As Date, _
        ByVal DataA As Date, _
        ByVal CUAA As String, _
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        ) As List(Of SchedaValidazione)

    End Function

End Class
