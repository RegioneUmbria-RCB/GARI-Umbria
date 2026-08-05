

Imports Importazione_Agea_DAL

Public Class AgeaAnagrafe

    ''' <summary>
    ''' Legge un xml da cache, restituisce un XDocument
    ''' </summary>
    ''' <param name="CUAA"></param>
    ''' <param name="Validazione_Numero"></param>    
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LeggiCache( _
        ByVal EnteValidatore_COD As Integer, _
        ByVal cuaa As String, _
        ByVal Validazione_Numero As String, _
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As String

        Dim XLeggiCache As New AgeaAnagrafe_R
        Dim XD As String = XLeggiCache.LeggiCache( _
            EnteValidatore_COD, _
            cuaa, _
            Validazione_Numero, _
            "", _
            "", _
            "", _
            objParametri _
        )

        Return XD


    End Function


End Class


