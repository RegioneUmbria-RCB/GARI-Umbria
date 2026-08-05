Imports System.Text
Imports AgronicaCoreDataProvider

Public Class Configurazioni_Interscambio
    Inherits DataProvider

    Protected objParametriServer As AgronicaCoreParametri
    Protected objParametriUtenti As AgronicaCoreParametri

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriUtenti As AgronicaCoreParametri)
        Me.objParametriServer = objParametriServer
        Me.objParametriUtenti = objParametriUtenti
    End Sub

    Function LeggiConfigurazioneGuida(ByVal idGuida As Integer, ByVal idConfig As Integer, ByVal xFiltroAggiuntivo As String) As DataTable

        Const nomeRoutine = "AgronicaCoreVarieDAL.Configurazioni_Interscambio.LeggiConfigurazioneGuida()"

        Dim messaggioErrore As String
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0

            strSql.AppendLine(" SELECT Configurazioni_Interscambio.*")
            strSql.AppendLine(" FROM  Configurazioni_Interscambio")
            strSql.AppendLine(" INNER JOIN Configurazione_Servizi_Guida ON Configurazione_Servizi_Guida.Id_Config = Configurazioni_Interscambio.Id_Config")
            strSql.AppendLine(" WHERE 1=1  ")

            If idGuida <> 0 Then
                strSql.AppendLine(" AND Configurazione_Servizi_Guida.ID = " & Agro_SQL_SaveNum(idGuida))
            End If

            If idConfig <> 0 Then
                strSql.AppendLine(" AND Configurazioni_Interscambio.Id_Config = " & Agro_SQL_SaveNum(idConfig))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametriServer))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Function LeggiConfigurazione(ByVal idConfig As Integer, ByVal xFiltroAggiuntivo As String) As DataTable

        Const nomeRoutine = "AgronicaCoreVarieDAL.Configurazioni_Interscambio.LeggiConfigurazione()"

        Dim messaggioErrore As String
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0

            strSql.AppendLine(" SELECT Configurazioni_Interscambio.*")
            strSql.AppendLine(" FROM  Configurazioni_Interscambio")
            strSql.AppendLine(" WHERE 1=1  ")

            If idConfig <> 0 Then
                strSql.AppendLine(" AND Configurazioni_Interscambio.Id_Config = " & Agro_SQL_SaveNum(idConfig))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametriServer))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Function AggiornaConfigurazione(ByVal idConfig As Integer,
                                    ByVal sigla As String,
                                    ByVal parametriStandard As String,
                                    ByVal parametriRuntime As String,
                                    ByVal tipizzazioneParametriRuntime As String) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Configurazioni_Interscambio.AggiornaConfigurazione()"

        Dim MessaggioErrore As String
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE Configurazioni_Interscambio SET ")
            StrSQL.AppendLine("       Data_Modifica                  =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.AppendLine("       ,UserName_Modifica              = '" & Agro_SQL_SaveText(objParametriServer.UsernameOperazione) & "'")

            If parametriStandard <> "" Then
                StrSQL.AppendLine("       ,Parametri_Standard              = '" & Agro_SQL_SaveText(parametriStandard) & "'")
            End If

            If parametriRuntime <> "" Then
                StrSQL.AppendLine("       ,Parametri_Runtime              = '" & Agro_SQL_SaveText(parametriRuntime) & "'")
            End If

            If tipizzazioneParametriRuntime <> "" Then
                StrSQL.AppendLine("       ,Tipizzazione_Parametri_Runtime = '" & Agro_SQL_SaveText(tipizzazioneParametriRuntime) & "'")
            End If

            StrSQL.AppendLine(" WHERE    Id_Config        = " & Agro_SQL_SaveNum(idConfig))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametriServer, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

End Class
