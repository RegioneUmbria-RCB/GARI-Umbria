Imports System.Data.SqlClient

Public Class ConfigurazioneBase

    Public Shared Function EsisteTabella(ByVal nomeTabella As String, ByVal objParametri As AgronicaCoreParametri) As Boolean

        If DataProviderFactory.Instance.TipoProvider = TipiEnumerativi.enum_DataProvidersType.OleDbProvider Then
            Return False
        End If

        Dim retVal As Boolean
        Dim cb2 As New ConfigurazioneBase2
        retVal = cb2.EsisteTabellaQry(nomeTabella, objParametri)
        Return retVal

    End Function

End Class

Public Class ConfigurazioneBase2
    Inherits DataProvider

    Public Function EsisteTabellaQry(ByVal nomeTabella As String, ByVal objParametri As AgronicaCoreParametri) As Boolean


        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Widgets_R.EsisteTabella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append("select count(1) from sys.tables tt where tt.name = '" & Agro_SQL_SaveText(nomeTabella) & "'")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt.Rows.Count > 0 AndAlso CInt(dt.Rows(0)(0)) > 0


    End Function

End Class
