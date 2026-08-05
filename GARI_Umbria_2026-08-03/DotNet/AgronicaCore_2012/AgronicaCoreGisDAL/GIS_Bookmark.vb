Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.anagrafiche

Public Class GIS_Bookmark_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Leggi(ByVal bookmark_Cod As Int32, objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.GIS_Bookmark_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * FROM GIS_Bookmark ")
            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" Utente = '{0}' ", Agro_SQL_SaveText(objParametri.UtenteUsername)))
            StrSQL.AppendLine(String.Format(" AND PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))

            If bookmark_Cod <> 0 Then
                StrSQL.AppendLine(String.Format(" AND Bookmark_Cod = {0} ", Agro_SQL_SaveNum(bookmark_Cod)))
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
Public Class GIS_Bookmark_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function SalvaNuovoBookmark(ByVal newIDBookmark As Int32,
                                       ByVal bookmark As Bookmark,
                                       ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Bookmark_W.SalvaNuovoBookmark()"

        Dim MessaggioErrore As String = ""
        Dim StrSql As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            '---------------------------------------------
            StrSql.Length = 0

            StrSql.AppendLine(" INSERT INTO GIS_Bookmark ( ")
            StrSql.AppendLine(" PivaSuperUser ")
            StrSql.AppendLine(" , Bookmark_Cod ")
            StrSql.AppendLine(" , Bookmark_Des ")
            StrSql.AppendLine(" , Utente ")
            StrSql.AppendLine(" , Center_Lat ")
            StrSql.AppendLine(" , Center_Lng ")
            StrSql.AppendLine(" , Zoom ")
            StrSql.AppendLine(" , Posizioni_Speciali ")
            StrSql.AppendLine(" , Username_Creazione ")
            StrSql.AppendLine(" , Username_Modifica ")

            StrSql.AppendLine(" ) VALUES ( ")
            StrSql.AppendLine(String.Format(" '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(newIDBookmark)))
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(bookmark.Bookmark_Des)))
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UtenteUsername)))
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(bookmark.Center_Lat)))
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(bookmark.Center_Lng)))
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(bookmark.Zoom)))
            StrSql.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(bookmark.Posizioni_Speciali)))
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSql.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSql.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

    Public Function EliminaBookmark(ByVal bookmark_Cod As Int32,
                                    ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Bookmark_W.EliminaBookmark()"

        Dim MessaggioErrore As String = ""
        Dim StrSql As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            '---------------------------------------------
            StrSql.Length = 0

            StrSql.AppendLine(" DELETE FROM GIS_Bookmark ")

            StrSql.AppendLine(" WHERE ")

            StrSql.AppendLine(String.Format(" Bookmark_Cod = {0} ", Agro_SQL_SaveNum(bookmark_Cod)))
            StrSql.AppendLine(String.Format(" AND Utente = '{0}' ", Agro_SQL_SaveText(objParametri.UtenteUsername)))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

    Public Function AggiornaBookmark(ByVal bookmark As Bookmark,
                                     ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_Bookmark_W.AggiornaBookmark()"

        Dim MessaggioErrore As String = ""
        Dim StrSql As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try

            '---------------------------------------------
            StrSql.Length = 0

            StrSql.AppendLine(" UPDATE GIS_Bookmark SET ")
            StrSql.AppendLine(String.Format(" Bookmark_Des = '{0}' ", Agro_SQL_SaveText(bookmark.Bookmark_Des)))
            StrSql.AppendLine(String.Format(" , Center_Lat = {0} ", Agro_SQL_SaveNum(bookmark.Center_Lat)))
            StrSql.AppendLine(String.Format(" , Center_Lng = {0} ", Agro_SQL_SaveNum(bookmark.Center_Lng)))
            StrSql.AppendLine(String.Format(" , Zoom = {0} ", Agro_SQL_SaveNum(bookmark.Zoom)))
            StrSql.AppendLine(String.Format(" , Posizioni_Speciali = {0} ", Agro_SQL_SaveNum(bookmark.Posizioni_Speciali)))
            StrSql.AppendLine(" , Data_Modifica = GETDATE() ")
            StrSql.AppendLine(String.Format(" , Username_Modifica = '{0}' ", Agro_SQL_SaveText(objParametri.UtenteUsername)))

            StrSql.AppendLine(" WHERE ")

            StrSql.AppendLine(String.Format(" Bookmark_Cod = {0} ", Agro_SQL_SaveNum(bookmark.Bookmark_Cod)))
            StrSql.AppendLine(String.Format(" AND Utente = '{0}' ", Agro_SQL_SaveText(objParametri.UtenteUsername)))

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(objParametri, StrSql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp

    End Function
End Class
