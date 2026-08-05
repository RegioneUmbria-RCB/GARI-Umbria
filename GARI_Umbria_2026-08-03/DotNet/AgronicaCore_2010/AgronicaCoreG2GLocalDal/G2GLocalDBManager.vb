
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Text.RegularExpressions
Imports System.IO


'#################################################################
'#################################################################
'#################################################################

Public Class G2GLocalDBManager_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function getDBFolder(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As DataTable


        Try
            '---------------------------------------------
            Stb.Length = 0
            Stb.Append("SELECT SF.fileid, SF.name, SF.filename FROM sysfiles SF where fileid=1")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If xRisp.Rows.Count > 0 Then

                Dim lFullfilenamewithPath As String = xRisp(0)("filename")

                Return My.Computer.FileSystem.GetFileInfo(lFullfilenamewithPath).DirectoryName



            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return ""
    End Function

    '##############################################################################################
    Public Function AttachDB( _
                                ByVal filename As String _
                               , ByVal dbName As String _
                               , ByVal PercorsoCartella As String _
                               , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        Try
            '---------------------------------------------
            Stb.Length = 0
            Stb.Append("CREATE DATABASE " & dbName & " ON (FILENAME =  '" & PercorsoCartella & filename & ".mdf') FOR ATTACH;")

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



    '##############################################################################################
    Public Function CreaDBVuoto( _
                               ByVal dbName As String _
                               , ByVal PercorsoCartella As String _
                               , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        Try
            '---------------------------------------------
            Stb.Length = 0


            Stb.Append(" exec sp_executesql N' " & vbCrLf)
            Stb.Append(" USE [master]  " & vbCrLf)
            Stb.Append(" GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" /****** Object:  Database [" & dbName & "]    Script Date: 02/20/2013 10:20:08 ******/ " & vbCrLf)
            Stb.Append(" CREATE DATABASE [" & dbName & "] ON  PRIMARY  " & vbCrLf)
            Stb.Append(" ( NAME = N''" & dbName & "'', FILENAME = N''" & PercorsoCartella & dbName & ".mdf'' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ) " & vbCrLf)
            Stb.Append("  LOG ON  " & vbCrLf)
            Stb.Append(" ( NAME = N''" & dbName & "_log'', FILENAME = N''" & PercorsoCartella & dbName & "_log.ldf'' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%) " & vbCrLf)
            Stb.Append("             GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET COMPATIBILITY_LEVEL = 100 " & vbCrLf)
            Stb.Append("             GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" IF (1 = FULLTEXTSERVICEPROPERTY(''IsFullTextInstalled'')) " & vbCrLf)
            Stb.Append("                 begin " & vbCrLf)
            Stb.Append(" EXEC [" & dbName & "].[dbo].[sp_fulltext_database] @action = ''enable'' " & vbCrLf)
            Stb.Append("                 End " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET ANSI_NULL_DEFAULT OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET ANSI_NULLS OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET ANSI_PADDING OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET ANSI_WARNINGS OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET ARITHABORT OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET AUTO_CLOSE OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET AUTO_CREATE_STATISTICS ON  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET AUTO_SHRINK OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET AUTO_UPDATE_STATISTICS ON  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET CURSOR_CLOSE_ON_COMMIT OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET CURSOR_DEFAULT  GLOBAL  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET CONCAT_NULL_YIELDS_NULL OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET NUMERIC_ROUNDABORT OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET QUOTED_IDENTIFIER OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET RECURSIVE_TRIGGERS OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET  DISABLE_BROKER  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET AUTO_UPDATE_STATISTICS_ASYNC OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET DATE_CORRELATION_OPTIMIZATION OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET TRUSTWORTHY OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET ALLOW_SNAPSHOT_ISOLATION OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET PARAMETERIZATION SIMPLE  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET READ_COMMITTED_SNAPSHOT OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET HONOR_BROKER_PRIORITY OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET  READ_WRITE  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET RECOVERY SIMPLE  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET  MULTI_USER  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET PAGE_VERIFY CHECKSUM   " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ALTER DATABASE [" & dbName & "] SET DB_CHAINING OFF  " & vbCrLf)
            Stb.Append("                 GO " & vbCrLf)
            Stb.Append("'  " & vbCrLf)
            Stb.Append("  " & vbCrLf)


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
    Public Function Cancella(ByVal xFiltroAggiuntivo As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Append(" UPDATE ... ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   1=1 ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM ... ")
                StrSQL.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
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



