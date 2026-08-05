Imports System.Text
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions


Public Class DSS_Difesa_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Leggi(ByVal Stazione_Cod As Integer,
                           ByVal Tipo_Sorgente As Integer,
                           ByVal Av_Cod As Integer,
                           ByVal Veg_Cod As Integer,
                           ByVal Alg_Cod As Integer,
                           ByVal Provider_Consiglio As Integer,
                           ByVal Modello_Consiglio As Integer,
                           ByVal Data_Esecuzione As Date,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreMeteoDAL.DSS_Difesa_R.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" SELECT * ")
            Stb.AppendLine(" FROM DSS_Difesa  ")
            Stb.AppendLine(" WHERE 1 = 1  ")

            If Stazione_Cod <> 0 Then
                Stb.AppendLine(" AND Stazione_Cod = " & Agro_SQL_SaveNum(Stazione_Cod) & " ")
            End If

            If Tipo_Sorgente <> 0 Then
                Stb.AppendLine(" AND Tipo_Sorgente = " & Agro_SQL_SaveNum(Tipo_Sorgente) & " ")
            End If

            If Av_Cod <> 0 Then
                Stb.AppendLine(" AND Av_Cod = " & Agro_SQL_SaveNum(Av_Cod) & " ")
            End If

            If Veg_Cod <> 0 Then
                Stb.AppendLine(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If Alg_Cod <> 0 Then
                Stb.AppendLine(" AND Alg_Cod = " & Agro_SQL_SaveNum(Alg_Cod) & " ")
            End If

            If Provider_Consiglio <> 0 Then
                Stb.AppendLine(" AND Provider_Consiglio = " & Agro_SQL_SaveNum(Provider_Consiglio) & " ")
            End If

            If Modello_Consiglio <> 0 Then
                Stb.AppendLine(" AND Modello_Consiglio = " & Agro_SQL_SaveNum(Modello_Consiglio) & " ")
            End If

            If Data_Esecuzione <> AGRODATAINIZIO AndAlso Data_Esecuzione <> AGRODATAFINE Then
                Stb.AppendLine(" AND Data_Esecuzione = " & Agro_SQL_SaveDateTime(Data_Esecuzione))
            End If

            If Validita_Inizio <> AGRODATAINIZIO Then
                Stb.AppendLine(" AND Validita_Inizio >= " & Agro_SQL_SaveDateTime(Validita_Inizio))
            End If

            If Validita_Fine <> AGRODATAFINE Then
                Stb.AppendLine(" AND Validita_Fine <= " & Agro_SQL_SaveDateTime(Validita_Fine))
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            If xOrderBy <> "" Then
                Stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class

Public Class DSS_Difesa_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Scrivi(ByVal Stazione_Cod As Integer,
                           ByVal Tipo_Sorgente As Integer,
                           ByVal Av_Cod As Integer,
                           ByVal Veg_Cod As Integer,
                           ByVal Alg_Cod As Integer,
                           ByVal Provider_Consiglio As Integer,
                           ByVal Modello_Consiglio As Integer,
                           ByVal Data_Esecuzione As Date,
                           ByVal Risultato As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreMeteoDAL.DSS_Difesa_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO DSS_Difesa ")
            strSql.AppendLine("         ( ")
            strSql.AppendLine("          Stazione_Cod, Tipo_Sorgente, Av_Cod,")
            strSql.AppendLine("          Veg_Cod, Alg_Cod, Provider_Consiglio, Modello_Consiglio, Data_Esecuzione,")
            strSql.AppendLine("          Risultato, Username_Creazione, Username_Modifica,")
            strSql.AppendLine("          Validita_Inizio, Validita_Fine")
            strSql.AppendLine("         ) ")

            strSql.AppendLine(" VALUES ( ")
            strSql.AppendLine("          " & Agro_SQL_SaveNum(Stazione_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Sorgente))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Av_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Veg_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Alg_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Provider_Consiglio))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Modello_Consiglio))
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_Esecuzione))
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Risultato) & "'")
            strSql.AppendLine("         , '" & objParametri_Server.UtenteUsername & "'")
            strSql.AppendLine("         , '" & objParametri_Server.UtenteUsername & "'")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Validita_Inizio))
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Validita_Fine))
            strSql.AppendLine("        ) ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try



        Return xRisp

    End Function

End Class