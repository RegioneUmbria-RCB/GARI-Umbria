Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PC_CoefficenteMinimoSoN_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '######################################################################################################################
    Public Function Leggi_CoeffMinimo(ByVal Regolamento_Cod As Int32, _
                                      ByVal Id_CN As Int32, _
                                      ByVal Id_GruppoTessitura As Int32, _
                                         ByVal xFiltroAggiuntivo As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                      ) As Decimal

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PC_CoefficenteMinimoSoN_R.Leggi_CoeffMinimo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim dblCoeff As Decimal = 0

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM    PC_CoefficenteMinimoSoN ")
            StrSQL.Append(" WHERE   Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

            StrSQL.Append(" AND   Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND   Id_CN = " & Agro_SQL_SaveNum(Id_CN) & " ")
            StrSQL.Append(" AND   Id_GruppoTessitura = " & Agro_SQL_SaveNum(Id_GruppoTessitura) & " ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PC_CoefficenteMinimoSoN.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PC_CoefficenteMinimoSoN.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            If DT.Rows.Count = 1 Then
                dblCoeff = DT.Rows(0).Item("CoeffMin")
            End If
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dblCoeff


    End Function


End Class
