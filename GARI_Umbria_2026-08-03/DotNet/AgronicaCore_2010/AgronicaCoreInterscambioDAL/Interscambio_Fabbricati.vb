Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Interscambio_Fabbricati_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(Sistema_Cod As Integer,
                          Piva As String,
                          Sa_Cod As Integer,
                          Fabbricato_Cod As Integer,
                          Codice_Esterno As String,
                          xFiltroAggiuntivo As String,
                          xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal dataValidita As Date = #2/1/1900#
                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreInterscambioDAL.Interscambio_Fabbricati_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM   Interscambio_Fabbricati ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If dataValidita <> #2/1/1900# Then
                StrSQL.AppendLine(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(dataValidita) & " ")
                StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(dataValidita) & " ")
            End If

            If Sistema_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sistema_Cod = " & Agro_SQL_SaveNum(Sistema_Cod) & "")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "")
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.AppendLine(" AND Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "")
            End If

            If Codice_Esterno <> "" Then
                StrSQL.AppendLine(" AND Codice_Esterno = '" & Agro_SQL_SaveText(Codice_Esterno) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Data_Creazione DESC")
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

Public Class Interscambio_Fabbricati_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(Sistema_Cod As Integer,
                           Piva As String,
                           Sa_Cod As Integer,
                           Fabbricato_Cod As Integer,
                           Codice_Gias As String,
                           Codice_Esterno As String,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                           Optional ByVal Validita_Fine As Date = AGRODATAFINE
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreInterscambioDAL.Interscambio_Fabbricati_W.Scrivi()"

        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO Interscambio_Fabbricati ( ")
            StrSQL.AppendLine("           Sistema_Cod")

            StrSQL.AppendLine("         , Piva")
            StrSQL.AppendLine("         , Sa_Cod")
            StrSQL.AppendLine("         , Fabbricato_Cod")

            StrSQL.AppendLine("         , Codice_Gias")
            StrSQL.AppendLine("         , Codice_Esterno")

            StrSQL.AppendLine("         , Data_Creazione")
            StrSQL.AppendLine("         , Data_Modifica")
            StrSQL.AppendLine("         , Username_Creazione")
            StrSQL.AppendLine("         , Username_Modifica")
            StrSQL.AppendLine("         , Validita_Inizio")
            StrSQL.AppendLine("         , Validita_Fine")
            StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine(" VALUES (")
            StrSQL.AppendLine("            " & Agro_SQL_SaveNum(Sistema_Cod) & " ")

            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")

            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(Codice_Gias) & "' ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(Codice_Esterno) & "' ")

            StrSQL.AppendLine("          , " & Agro_SQL_SaveDateTime(Now) & " ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDateTime(Now) & " ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp

    End Function

End Class
