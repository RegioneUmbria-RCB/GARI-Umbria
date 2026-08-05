Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Progetti_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################

    Public Function Progetto_from_PivaSaCodAppezzaIdreg( _
                            ByVal PIVA As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Appezza As Int32, _
                            ByVal Id_Reg As Int32, _
                            ByVal Data As Date, _
                            ByRef ProgettoNome As String, _
                            ByRef ProgettoCod As Integer, _
                            ByRef Regolamento_Cod As Integer, _
                            ByRef Regolamento_Des As String, _
                            ByRef Disciplinare_Cod As Integer, _
                            ByRef Stato_Impianto_Cod As Integer, _
                            ByRef Stato_Impianto_Des As String, _
                            ByRef P_Ha As Decimal, _
                            ByRef Cau_Progetto As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Progetti.Progetto_from_PivaSaCodAppezzaIdreg()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Imprese_Progetti.*, ISNULL(Regolamenti.Reg_Des,'') AS Reg_Des, ISNULL(GruppoFinalita.Grfi_Des,'') AS Grfi_Des ")
            StrSQL.Append(" FROM   Imprese_Progetti LEFT OUTER JOIN ")
            StrSQL.Append(" Regolamenti ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod LEFT OUTER JOIN ")
            StrSQL.Append(" GruppoFinalita ON Imprese_Progetti.Stato_Impianto = GruppoFinalita.Grfi_Cod ")
            StrSQL.Append(" WHERE Imprese_Progetti.Validita_inizio <= " & Agro_SQL_SaveDate(Data) & " ")
            StrSQL.Append(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & " ")

            If PIVA <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If
            If Appezza <> 0 Then
                StrSQL.Append(" AND appezza = " & Agro_SQL_SaveNum(Appezza))
            End If
            If Id_Reg <> 0 Then
                StrSQL.Append(" AND id_reg = " & Agro_SQL_SaveNum(Id_Reg))
            End If
            If Cau_Progetto <> 0 Then
                StrSQL.Append(" AND cau_progetto = " & Agro_SQL_SaveNum(9100))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Imprese_Progetti.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Imprese_Progetti.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            '---------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        If Not IsNothing(DT) And DT.Rows.Count <> 0 Then
            ProgettoNome = DT.Rows(0).Item("progetto_nome")
            ProgettoCod = DT.Rows(0).Item("progetto_cod")
            Cau_Progetto = DT.Rows(0).Item("cau_progetto")
            Regolamento_Cod = DT.Rows(0).Item("Regolamento_Cod")
            Regolamento_Des = DT.Rows(0).Item("Reg_Des")
            Disciplinare_Cod = DT.Rows(0).Item("Disciplinare_Cod")
            Stato_Impianto_Cod = DT.Rows(0).Item("Stato_Impianto")
            Stato_Impianto_Des = DT.Rows(0).Item("Grfi_Des")


            If (Not IsNothing(DT.Rows(0).Item("p_ha"))) Then
                P_Ha = DT.Rows(0).Item("p_ha")
            Else
                P_Ha = 0
            End If

        Else
            ProgettoNome = ""
            ProgettoCod = -1
            Regolamento_Cod = 1
            Regolamento_Des = ""
            Disciplinare_Cod = 0
            Stato_Impianto_Cod = 0
            Stato_Impianto_Des = ""
            P_Ha = 0

        End If

        Return DT

    End Function



End Class



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
