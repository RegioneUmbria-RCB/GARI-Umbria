Imports AgronicaCoreDataProvider.UtilityProvider
Public Class Ditte_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Ditta_Cod As Integer, _
                           ByVal Tipo As String, _
                           ByVal xFiltroAggiuntivo As String, _
                           ByVal xOrderBy As String, _
                           ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim StrSQL As New System.Text.StringBuilder
        Dim nomeroutine As String = "AgronicaCoreMetaSchemaDAL.Ditte.Leggi"

        Try
            '---------------------------------------------

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Ditte ")
            StrSQL.Append(" WHERE Validita_Inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Ditta_Cod <> 0 Then
                StrSQL.Append(" AND   Ditta_Cod = " & Agro_SQL_SaveNum(Ditta_Cod) & "   ")
            End If


            If Not Tipo.Equals("") Then
                StrSQL.Append(" AND Tipo = '" + Agro_SQL_SaveText(Tipo) + "'")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Ditta_Des ")
            End If


            '--------------------------------------------------------------------------
            Return MyBase.EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeroutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MyBase.Scrivi_LOG(objParametri, nomeroutine, ex.Message)
            Throw New Exception("[" & nomeroutine & "] : " & ex.Message)
            Return Nothing
        End Try


    End Function



    Public Function DittaDes_from_DittaCod(ByVal Ditta_Cod As Integer, _
                                                  ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String


        Dim dt As DataTable = Leggi(Ditta_Cod, "", "", "", objParametri)

        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Ditta_Des")
        End If

        Return ""

    End Function



End Class
