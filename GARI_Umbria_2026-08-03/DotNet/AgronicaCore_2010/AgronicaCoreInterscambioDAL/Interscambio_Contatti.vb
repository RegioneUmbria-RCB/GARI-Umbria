Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Interscambio_Contatti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(Sistema_Cod As Integer,
                          Piva As String,
                          Cod_Contatto As String,
                          Cod_Risum As Integer,
                          Codice_Esterno As String,
                          Validita_Inizio As Date,
                          Validita_Fine As Date,
                          xFiltroAggiuntivo As String,
                          xOrderBy As String,
                              ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreInterscambioDAL.Interscambio_Contatti_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM   Interscambio_Contatti ")

            StrSQL.AppendLine(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Sistema_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sistema_Cod = " & Agro_SQL_SaveNum(Sistema_Cod) & "")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Cod_Contatto <> "" Then
                StrSQL.AppendLine(" AND Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'")
            End If

            If Cod_Risum <> 0 Then
                StrSQL.AppendLine(" AND Cod_Risum = " & Agro_SQL_SaveNum(Cod_Risum) & "")
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

Public Class Interscambio_Contatti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(Sistema_Cod As Integer,
                           Piva As String,
                           Cod_Contatto As String,
                           Cod_Risum As Integer,
                           Codice_Gias As String,
                           Codice_Esterno As String,
                           Validita_Inizio As Date,
                           Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreInterscambioDAL.Interscambio_Contatti_W.Scrivi()"

        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO Interscambio_Contatti ( ")
            StrSQL.AppendLine("           Sistema_Cod")

            StrSQL.AppendLine("         , Piva")
            StrSQL.AppendLine("         , Cod_Contatto")
            StrSQL.AppendLine("         , Cod_Risum")

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
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveNum(Cod_Risum) & " ")

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

    Public Function Cancella(Sistema_Cod As Integer,
                             Codice_Esterno As String,
                            ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreInterscambioDAL.Interscambio_Contatti_W.Cancella()"

        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" Delete FROM Interscambio_Contatti ")
            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(" Sistema_Cod = " & Agro_SQL_SaveNum(Sistema_Cod))
            StrSQL.AppendLine(" AND Codice_Esterno = '" & Agro_SQL_SaveText(Codice_Esterno) & "' ")

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
