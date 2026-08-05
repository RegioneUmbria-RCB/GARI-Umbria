


Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class APP_Ricette_Operazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '#################################################################
    Public Function APP_MarcaOperazioneImportata(
        ByVal ID As String,
        ByVal Errori As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "APP_MarcaOperazioneImportata()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            Dim xErr As String = If(Errori.Length > 2000, Errori.Substring(0, 2000), Errori)

            Stb.Append(" UPDATE APP_Ricette_Operazioni ")
            Stb.Append(" SET ")
            Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
            Stb.Append("         ,Importato_Data= " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            Stb.Append("         ,Importato_Errore = '" & Agro_SQL_SaveText(xErr) & "' ")
            Stb.Append(" WHERE   ID = '" & ID & "' ")

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
