Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PC_TriangoloTessitura_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '######################################################################################################################
    Public Function Leggi_IdTessitura(ByVal Sabbia As Int32,
                                      ByVal Argilla As Int32,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As Int32

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_TriangoloTessitura_R.Leggi_IdTessitura()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim IdTessitura As Integer

        Try

            '--------------------------------------------------------------------------
            'Verifica parametri obbligatori
            '

            If IsNothing(Sabbia) Then
                Throw New Exception("Parametro non corretto nella query (Sabbia obbligatorio)")
            End If

            If IsNothing(Argilla) Then
                Throw New Exception("Parametro non corretto nella query (Argilla obbligatorio)")
            End If

            '--------------------------------------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Id_Tessitura ")
            StrSQL.Append(" FROM  PC_TriangoloTessitura ")
            StrSQL.Append(" WHERE Sabbia = " & Agro_SQL_SaveNum(Sabbia) & " ")
            StrSQL.Append(" AND   Argilla = " & Agro_SQL_SaveNum(Argilla) & " ")



            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PC_TriangoloTessitura.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PC_TriangoloTessitura.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) Then
                If DT.Rows.Count <> 0 Then
                    IdTessitura = DT.Rows(0).Item("Id_Tessitura")
                Else
                    IdTessitura = 0
                End If
            Else
                IdTessitura = 0
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return IdTessitura


    End Function


End Class
