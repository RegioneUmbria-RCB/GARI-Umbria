Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Text

Public Class Cespiti
    Inherits AgronicaCoreDataProvider.DataProvider

    
    Public Function LeggiMovimentiCespiti(ByVal Piva As String, _
                        ByVal SaCod As Integer, _
                        ByVal BeneTipo As Integer, _
                        ByVal BeneCod As Integer, _
                        ByVal Anno As Integer, _
                        ByVal xFiltroAggiuntivo As String, _
                        ByVal xOrderBy As String, _
                        ByVal int_Configurazione_Moduli As enum_Omni_Modulo_Generazione, _
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cespiti.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * " + vbCrLf)
            StrSQL.Append(" FROM Cespiti C " + vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN Cespiti_Categorie CC on (CC.id_cod_categoria=C.id_cod_categoria) " + vbCrLf)
            StrSQL.Append(" INNER JOIN Cespiti_Movimenti CM on (C.id_cod_cespite=CM.id_cod_cespite) " + vbCrLf)
            StrSQL.Append(" WHERE C.id_cod_cespite in (SELECT id_cod_cespite FROM Cespiti_Movimenti WHERE esercizio<=" + Agro_SQL_SaveNum(CStr(Anno)) + " GROUP BY id_cod_cespite) ")
            StrSQL.Append(" AND CM.esercizio <= " + Agro_SQL_SaveNum(CStr(Anno)) + " " + vbCrLf)

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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



    Public Function LeggiNumRigheMovCespite( _
                        ByVal Anno As Integer, _
                        ByVal IdCodCespite As Integer, _
                        ByVal xFiltroAggiuntivo As String, _
                        ByVal xOrderBy As String, _
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cespiti.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT COUNT(c.id_cod_cespite) AS Conteggio,C.id_cod_cespite " + vbCrLf)
            StrSQL.Append(" FROM Cespiti C " + vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN Cespiti_Categorie CC on (CC.id_cod_categoria=C.id_cod_categoria) " + vbCrLf)
            StrSQL.Append(" INNER JOIN Cespiti_Movimenti CM on (C.id_cod_cespite=CM.id_cod_cespite) " + vbCrLf)
            StrSQL.Append(" WHERE C.id_cod_cespite in (SELECT id_cod_cespite FROM Cespiti_Movimenti WHERE esercizio<= " + Anno.ToString + " GROUP BY id_cod_cespite) " + vbCrLf)
            StrSQL.Append(" AND CM.esercizio <= " + Agro_SQL_SaveNum(CStr(Anno)) + " " + vbCrLf)
            StrSQL.Append(" AND C.id_cod_cespite =" + Agro_SQL_SaveNum(CStr(IdCodCespite)) + " " + vbCrLf)
            StrSQL.Append(" GROUP BY C.id_cod_cespite " + vbCrLf)

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
