Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PDC_MotivoCampionamento_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal ID_MotivoCampionamento As Int32,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCorePianisiCampionamentoDAL.PDC_MotivoCampionamento_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------
            StrSQL.AppendLine(" SELECT     *   ")
            StrSQL.AppendLine(" FROM         PDC_MotivoCampionamento  ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If ID_MotivoCampionamento <> 0 Then
                StrSQL.AppendLine(" AND PDC_MotivoCampionamento.ID_MotivoCampionamento = " & Agro_SQL_SaveNum(ID_MotivoCampionamento))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

End Class
