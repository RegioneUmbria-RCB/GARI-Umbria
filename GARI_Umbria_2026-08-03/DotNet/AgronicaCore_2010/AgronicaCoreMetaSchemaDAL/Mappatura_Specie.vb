
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Mappatura_Specie_R
    Inherits AgronicaCoreDataProvider.DataProvider





    '##############################################################################################
    Public Function Leggi(ByVal ID_Specie As Integer, _
                              ByVal xFiltroAggiuntivo As String, _
                              ByVal xOrderBy As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Mappatura_Specie_R.Leggi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            

            StrSQL.Length = 0
            StrSQL.Append(" SELECT     Mappatura_Specie.Veg_Cod, Mappatura_Specie.Grva_Cod, SpecieVegetali.Veg_Des, Mappatura_Specie.Hybrid, GruppoVarietale.Grva_Des")
            StrSQL.Append(" FROM         Mappatura_Specie INNER JOIN ")
            StrSQL.Append(" SpecieVegetali ON Mappatura_Specie.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN ")
            StrSQL.Append(" GruppoVarietale ON Mappatura_Specie.Grva_Cod = GruppoVarietale.Grva_Cod ")

            StrSQL.Append(" WHERE   1 =1 ")
            StrSQL.Append(" AND     ID_Specie =" & Agro_SQL_SaveNum(ID_Specie) & " ")
             

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
                    '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     GruppoVarietale.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     GruppoVarietale.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
                    '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Veg_Des, Grva_Des ")
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





'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
