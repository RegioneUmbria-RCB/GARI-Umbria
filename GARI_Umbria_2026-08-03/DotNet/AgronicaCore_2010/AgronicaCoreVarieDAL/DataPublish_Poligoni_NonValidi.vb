Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports System.Text

Public Class DataPublish_Poligoni_NonValidi_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Leggi(ByVal id As Integer,
                          ByVal Id_Notifica_SistemaEsterno As Integer,
                          ByVal CUAA As String,
                          ByVal Campagna As Integer,
                          ByVal Id_Appezzamento_Esterno As Long,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreVarieDAL.DataPublish_Poligoni_NonValidi_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT ")
            stb.AppendLine("    Id, ")
            stb.AppendLine("    Id_Notifica_SistemaEsterno, ")
            stb.AppendLine("    CUAA, ")
            stb.AppendLine("    Campagna, ")
            stb.AppendLine("    Id_Appezzamento_Esterno, ")
            stb.AppendLine("    SRID, ")
            stb.AppendLine("    Poligono_WKT, ")
            stb.AppendLine("    Data_Creazione, ")
            stb.AppendLine("    Username_Creazione ")
            stb.AppendLine(" FROM DataPublish_Poligoni_NonValidi ")
            stb.AppendLine(" WHERE 1=1 ")

            If id <> 0 Then
                stb.AppendLine(" AND ID = " & Agro_SQL_SaveNum(id) & " ")
            End If

            If Id_Notifica_SistemaEsterno <> 0 Then
                stb.AppendLine(" AND Id_Notifica_SistemaEsterno = " & Agro_SQL_SaveNum(Id_Notifica_SistemaEsterno) & " ")
            End If

            If CUAA <> "" Then
                stb.AppendLine(" AND CUAA = '" & Agro_SQL_SaveText(CUAA) & "' ")
            End If

            If Campagna <> 0 Then
                stb.AppendLine(" AND Campagna = " & Agro_SQL_SaveNum(Campagna) & " ")
            End If

            If Id_Appezzamento_Esterno <> 0 Then
                stb.AppendLine(" AND Id_Appezzamento_Esterno = " & Agro_SQL_SaveNum(Id_Appezzamento_Esterno) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
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

Public Class DataPublish_Poligoni_NonValidi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Id_Notifica_SistemaEsterno As Integer,
                           ByVal CUAA As String,
                           ByVal Campagna As Integer,
                           ByVal Id_Appezzamento_Esterno As Long,
                           ByVal srid As Integer,
                           ByVal Poligono_WKT As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.DataPublish_Poligoni_NonValidi_W.Scrivi()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim isResp As Boolean

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO DataPublish_Poligoni_NonValidi ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine("     Id_Notifica_SistemaEsterno, ")
            StrSQL.AppendLine("     CUAA, ")
            StrSQL.AppendLine("     Campagna, ")
            StrSQL.AppendLine(" 	Id_Appezzamento_Esterno, ")
            StrSQL.AppendLine(" 	SRID, ")
            StrSQL.AppendLine(" 	Poligono_WKT, ")
            StrSQL.AppendLine(" 	Inviato, ")
            StrSQL.AppendLine(" 	Data_Creazione, ")
            StrSQL.AppendLine(" 	Data_Modifica, ")
            StrSQL.AppendLine(" 	Username_Creazione, ")
            StrSQL.AppendLine(" 	Username_Modifica, ")
            StrSQL.AppendLine(" 	Validita_Inizio, ")
            StrSQL.AppendLine(" 	Validita_Fine ")
            StrSQL.AppendLine(" ) VALUES ( ")
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveNum(Id_Notifica_SistemaEsterno)))
            StrSQL.AppendLine(String.Format(" '{0}', ", Agro_SQL_SaveText(CUAA)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveNum(Campagna)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveNum(Id_Appezzamento_Esterno)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveNum(srid)))
            StrSQL.AppendLine(String.Format(" '{0}', ", Agro_SQL_SaveText(Poligono_WKT)))
            StrSQL.AppendLine(" 0, ")
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveDateTime(DateTime.Now)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveDateTime(DateTime.Now)))
            StrSQL.AppendLine(String.Format(" '{0}', ", Agro_SQL_SaveText(objParametri.UtenteUsername)))
            StrSQL.AppendLine(String.Format(" '{0}', ", Agro_SQL_SaveText(objParametri.UtenteUsername)))
            StrSQL.AppendLine(String.Format(" {0}, ", Agro_SQL_SaveDate(AGRODATAINIZIO)))
            StrSQL.AppendLine(String.Format(" {0} ", Agro_SQL_SaveDate(AGRODATAFINE)))
            StrSQL.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            isResp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            isResp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return isResp

    End Function

End Class
