
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Public Class G2G_Recode_Agenda_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal FromPiva As String, ByVal FromSa_cod As Integer, ByVal FromId_Agenda As Integer, ByVal objParametri As AgronicaCoreParametri) As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.Append(" " & vbCrLf)
            Stb.Append(" select * " & vbCrLf)
            Stb.Append(" from G2G_Recode_Agenda rr " & vbCrLf)
            Stb.Append(" where 1=1 " & vbCrLf)

            If FromPiva <> "" Then
                Stb.Append(" and rr.FromPiva = " & FromPiva & " " & vbCrLf)

            End If

            If FromSa_cod <> 0 Then
                Stb.Append(" and rr.FromSa_cod = " & FromSa_cod & " " & vbCrLf)
            End If

            If FromId_Agenda <> 0 Then
                Stb.Append(" and  rr.FromId_Agenda = " & FromId_Agenda & " " & vbCrLf)
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


