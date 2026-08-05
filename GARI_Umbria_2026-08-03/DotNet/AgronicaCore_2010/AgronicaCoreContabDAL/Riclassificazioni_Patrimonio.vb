Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class Riclassificazioni_Patrimonio_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal Piva As String, _
                        ByVal Ric_Cod_Pat As Integer, _
                        ByVal Piva_Riferimento As String, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Riclassificazioni_Patrimonio_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        '------------------------------

        Try

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Riclassificazioni_Patrimonio  ")

            StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            StrSQL.Append(" AND     Piva      = '" & Agro_SQL_SaveText(Piva) & "'   ")

            If (Ric_Cod_Pat <> 0) Then
                StrSQL.Append(" AND Ric_Cod_Pat   =  " & Agro_SQL_SaveNum(Ric_Cod_Pat) & "   ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Riclassificazioni_Patrimonio.Inviato >=0 ")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Riclassificazioni_Patrimonio.Inviato =-1 ")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dt


    End Function



    '###################################################################################
    Public Function Leggi_PivaRiferimento(ByVal Piva As String, _
                                             ByVal Ric_Cod_Pat As Integer, _
                                            ByVal xFiltroAggiuntivo As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As String

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Riclassificazioni_Patrimonio.Leggi_PivaRiferimento()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim PivaRiferimento As String = ""

        Try

            DT = Leggi(Piva, Ric_Cod_Pat, "", _
                        xFiltroAggiuntivo, "", _
                        objParametri)

            If Not DT Is Nothing Then
                Select Case DT.Rows.Count
                    Case 0
                    Case 1
                        PivaRiferimento = DT.Rows(0).Item("Piva_Riferimento")
                    Case Is > 1
                        Throw New Exception("Sono stati recuperati più record di Riclassificazioni_Patrimonio per codice=" & Str(Ric_Cod_Pat))
                End Select

            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return PivaRiferimento

    End Function




End Class
