

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class GDPR_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function OttieniGDPRValido(
        ByVal GDPR_Verifica As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" Select top 1 * ")
            Stb.AppendLine(" From GDPR ")

            If GDPR_Verifica <> "" Then
                Stb.AppendLine(" Where GDPR_COD Not In (" & Agro_SQL_Save_Clausola_IN(GDPR_Verifica) & ") ")
            End If

            Stb.AppendLine(" order by Validita_Inizio desc")



            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function


    '##############################################################################################
    Public Function Leggi(
        ByVal GDPR_Verifica As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" Select * ")
            Stb.AppendLine(" From GDPR ")

            If GDPR_Verifica <> 0 Then
                Stb.AppendLine(" Where GDPR_COD = " & GDPR_Verifica & "  ")
            End If



            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
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


Public Class GDPR_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(
                    GDPR_Cod As Integer,
                    TestoHtmlBreve As String,
                    TestoHtmlCompleto As String,
                    Validita_Inizio As Date,
                    Validita_fine As Date,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = ""
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" INSERT GDPR " + vbCrLf)

            Stb.Append("              (")
            Stb.AppendLine("         GDPR_Cod, ")
            Stb.AppendLine("         TestoHtmlBreve, ")
            Stb.AppendLine("         TestoHtmlCompleto, ")

            Stb.Append("              Inviato,            datainvio, ")
            Stb.Append("              Data_Creazione,     Data_Modifica, ")
            Stb.Append("              UserName_Creazione, UserName_Modifica, ")
            Stb.Append("              Validita_Inizio, Validita_Fine ")
            Stb.Append("              ) ")

            Stb.Append(" VALUES ( ")


            Stb.Append("           " & GDPR_Cod & "  " & vbCrLf)
            Stb.Append("         ,  '" & Agro_SQL_SaveText(TestoHtmlBreve) & "' " & vbCrLf)
            Stb.Append("			,'" & Agro_SQL_SaveText(TestoHtmlCompleto) & "' ")



            Stb.Append("         , 0  " & vbCrLf)
            Stb.Append("         , Null  " & vbCrLf)

            Stb.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            Stb.Append("			, " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Validita_fine) & "  ")

            Stb.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function







    '#################################################################
    Public Function modifica(
        GDPR_Cod As Integer,
        TestoHtmlBreve As String,
        TestoHtmlCompleto As String,
        Validita_Inizio As Date,
        Validita_fine As Date,
        ByVal xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "modifica()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            '---------------------------------------------

            Stb.Append(" UPDATE GDPR ")
            Stb.Append(" SET ")
            Stb.Append("   TestoHtmlBreve = '" & Agro_SQL_SaveText(TestoHtmlBreve) & "' ")
            Stb.Append(" , TestoHtmlCompleto = '" & Agro_SQL_SaveText(TestoHtmlCompleto) & "' ")
            Stb.Append(" , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            Stb.Append(" , Validita_Fine = " & Agro_SQL_SaveDate(Validita_fine) & " ")
            Stb.Append(" , Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

            Stb.Append(" WHERE GDPR_Cod = " & GDPR_Cod & "  ")
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function




End Class
