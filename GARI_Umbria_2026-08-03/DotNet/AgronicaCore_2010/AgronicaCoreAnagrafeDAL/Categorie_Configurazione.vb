Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Text

Public Class Categorie_Configurazione_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Elem_Cod As Integer,
                          ByVal Udm_Cod_Default As Integer,
                          ByVal Iva_Cod_Cat_Default As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AnagrafeDAL.Categorie_Configurazione_R.Leggi()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.AppendLine(" SELECT  * ")
            strSql.AppendLine(" FROM Categorie_Configurazione ")
            strSql.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            
            If Piva <> "" Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
            
            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Udm_Cod_Default <> 0 Then
                strSql.AppendLine(" AND Udm_Cod_Default = " & Agro_SQL_SaveNum(Udm_Cod_Default) & "   ")
            End If

            If Iva_Cod_Cat_Default <> 0 Then
                strSql.AppendLine(" AND Iva_Cod_Cat_Default = " & Agro_SQL_SaveNum(Iva_Cod_Cat_Default) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class
