Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PC_Liscivazione_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '######################################################################################################################
    Public Function Leggi_Perdita(ByVal Regolamento_Cod As Int32, _
                                  ByVal Argilla As Int32, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                  ) As Decimal

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_Liscivazione_R.Leggi_Perdita()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim dblPerdita As Decimal = 0

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT TOP 1 * ")
            StrSQL.Append(" FROM PC_Liscivazione ")
            StrSQL.Append(" WHERE   Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

            StrSQL.Append(" AND   Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND   percentuale <= " & Agro_SQL_SaveNum(Argilla) & " ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PC_Liscivazione.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PC_Liscivazione.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            StrSQL.Append(" ORDER BY percentuale DESC ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            '--------------------------------------------------------------------------
            If DT.Rows.Count = 1 Then
                dblPerdita = CDbl(DT.Rows(0).Item("perdita"))
            End If
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            dblPerdita = 0
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dblPerdita


    End Function
End Class
