Imports SincroAnagrafeBA
Imports AgroFascicoloBA_SIGPA_DAL

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

        Dim lRval As New List(Of SchedaValidazione)

        Dim sigpa As New AgroBA_Fascicolo_R
        Dim dtFA As DataTable = sigpa.getSchedeFascicolo( _
            CUAA, _
            DataDa, _
            DataA, _
            "", _
            "", _
            objParametri _
        )

        For Each rFA As DataRow In dtFA.Rows
            lRval.Add(New SchedaValidazione With { _
                       .numeroScheda = rFA("num_validazione"), _
                       .dataScheda = AgronicaCoreDataProvider.Conversioni.DateTime_To_DataintYYYYMMGG(rFA("data_validazione")), _
                       .dataSchedaSpecified = True _
                   })
        Next

        Return lRval

    End Function

End Class
