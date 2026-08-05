Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class PUA_Apporti_DAL
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function RecuperaEffluentiTotali(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByVal nomeColonna As String,
                                            ByVal PUA_Cod As Integer,
                                            ByVal Regolamento_Cod As Integer,
                                            ByVal sEff_Cod As String) As Decimal

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePUA_DAL.PUA_Apporti_DAL.RecuperaEffluentiTotali()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim res As Decimal

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT ISNULL(SUM(" + nomeColonna + "),0) ")
            strSQL.AppendLine(" FROM PUA_Effluente AS F")

            strSQL.AppendLine(" WHERE F.Piva_SuperUser = '" & objParametri.PivaSuperUser & "'")

            strSQL.AppendLine(" AND F.PUA_Cod = " & Agro_SQL_SaveNum(PUA_Cod))
            strSQL.AppendLine(" AND F.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))
            strSQL.AppendLine(" AND F.Eff_Cod IN (" & Agro_SQL_Save_Clausola_IN(sEff_Cod) & ")")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            res = DT.Rows(0).Item(0)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            res = -1
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return res


    End Function

    Public Function ApportoDistribuitoxData(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByVal DataPartenza As Date,
                                            ByVal DataIntervento As Date,
                                            ByVal FerCod As String) As Decimal
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePUA_DAL.PUA_Apporti_DAL.RecuperaEffluentiTotali()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim res As Decimal = 0

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT ISNULL( SUM(rdt.ApportoxHa * rdst.Qta2), 0)")

            strSQL.AppendLine(" FROM Ricette r ")
            strSQL.AppendLine(" INNER JOIN Ricette_Operazioni ro ON r.Ricetta_SuperUser = ro.Ricetta_SuperUser AND r.Ricetta_Cod = ro.Ricetta_Cod")
            strSQL.AppendLine(" INNER JOIN Ricette_Dettagli rd ON ro.Ricetta_SuperUser = rd.Ricetta_SuperUser AND ro.Ricetta_Cod = rd.Ricetta_Cod AND ro.Ricetta_Operazione_Cod = rd.Ricetta_Operazione_Cod ")
            strSQL.AppendLine(" INNER JOIN Ricette_Dettaglio_Tecnico rdt ON rd.Ricetta_SuperUser = rdt.Ricetta_SuperUser AND ")
            strSQL.AppendLine(" rd.Ricetta_Cod = rdt.Ricetta_Cod AND rd.Ricetta_Operazione_Cod = rdt.Ricetta_Operazione_Cod AND rd.Ricetta_Dettaglio_Cod = rdt.Ricetta_Dettaglio_Cod")
            strSQL.AppendLine(" INNER JOIN Ricette_Destinazioni rdst ON rd.Ricetta_SuperUser = rdst.Ricetta_SuperUser AND ")
            strSQL.AppendLine(" rd.Ricetta_Cod = rdst.Ricetta_Cod AND rd.Ricetta_Operazione_Cod = rdst.Ricetta_Operazione_Cod AND rd.Ricetta_Dettaglio_Cod = rdst.Ricetta_Dettaglio_Cod")

            strSQL.AppendLine(" WHERE r.Ricetta_SuperUser = '" & objParametri.PivaSuperUser & "'")
            strSQL.AppendLine(" AND r.Tipo_Ricetta = " & enum_TipoRicetta.PianoDistribuzionePua)
            strSQL.AppendLine(" AND r.Validita_Inizio >= " & Agro_SQL_SaveDate(DataPartenza))
            strSQL.AppendLine(" AND r.Validita_Inizio <= " & Agro_SQL_SaveDate(DataIntervento))

            ' aggiunta per PUA 2012, quindi la tabella fertilizzazioni non è più utilizzata
            If FerCod <> "" Then
                strSQL.AppendLine(" AND rd.elem_cod = 3 ")
                strSQL.AppendLine(" AND rd.pro_cod = " & FerCod)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                res = DT.Rows(0).Item(0)
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            res = -1
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return res

    End Function

    Public Function FertilizzanteDistribuitoIntervallo(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByVal Piva As String, ByVal DataPartenza As Date, ByVal DataIntervento As Date,
                                                        ByVal FerCod As String) As Decimal
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePUA_DAL.PUA_Apporti_DAL.FertilizzanteDistribuitoIntervallo()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim res As Decimal = 0

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT ISNULL( SUM (   ")
            strSQL.AppendLine(" Case    ")
            strSQL.AppendLine(" WHEN rd.extra_int = 2 Then rd.qta   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 29 Then rd.qta   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 19 Then rd.qta*1000   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 304 Then rd.qta*1000   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 4 Then rd.qta*100   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 104 Then rd.qta/1000   ")
            strSQL.AppendLine("  WHEN rd.extra_int = 101 Then rd.qta/1000   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 3 Then rd.qta/1000   ")
            strSQL.AppendLine(" Else  0    ")
            strSQL.AppendLine(" End  ")
            strSQL.AppendLine("                 * rdst.Qta2), 0) as FertilizzanteDistribuito  ")

            strSQL.AppendLine(" FROM Ricette r ")
            strSQL.AppendLine(" INNER JOIN Ricette_Operazioni ro ON r.Ricetta_SuperUser = ro.Ricetta_SuperUser AND r.Ricetta_Cod = ro.Ricetta_Cod")
            strSQL.AppendLine(" INNER JOIN Ricette_Dettagli rd ON ro.Ricetta_SuperUser = rd.Ricetta_SuperUser AND ro.Ricetta_Cod = rd.Ricetta_Cod AND ro.Ricetta_Operazione_Cod = rd.Ricetta_Operazione_Cod ")
            strSQL.AppendLine(" INNER JOIN Ricette_Destinazioni rdst ON rd.Ricetta_SuperUser = rdst.Ricetta_SuperUser AND ")
            strSQL.AppendLine(" rd.Ricetta_Cod = rdst.Ricetta_Cod AND rd.Ricetta_Operazione_Cod = rdst.Ricetta_Operazione_Cod AND rd.Ricetta_Dettaglio_Cod = rdst.Ricetta_Dettaglio_Cod")

            strSQL.AppendLine(" WHERE r.Ricetta_SuperUser = '" & objParametri.PivaSuperUser & "'")
            strSQL.AppendLine(" AND r.piva = '" & Agro_SQL_SaveText(Piva) & "'")
            strSQL.AppendLine(" AND cau_mov = '" & Agro_SQL_SaveText(CAU_LAVORAZIONE) & "'")
            'strSQL.AppendLine(" AND r.Tipo_Ricetta = " & enum_TipoRicetta.PianoDistribuzionePua)
            strSQL.AppendLine(" AND r.Validita_Inizio >= " & Agro_SQL_SaveDate(DataPartenza))
            strSQL.AppendLine(" AND r.Validita_Inizio <= " & Agro_SQL_SaveDate(DataIntervento))

            ' aggiunta per PUA 2012, quindi la tabella fertilizzazioni non è più utilizzata
            If FerCod <> "" Then
                strSQL.AppendLine(" AND rd.elem_cod = 3 ")
                strSQL.AppendLine(" AND rd.pro_cod = " & FerCod)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                res = DT.Rows(0).Item(0)
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            res = -1
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return res

    End Function

    Public Function NDistribuitoIntervallo(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByVal Piva As String, ByVal DataPartenza As Date, ByVal DataIntervento As Date,
                                                        ByVal FerCod As String) As Decimal
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePUA_DAL.PUA_Apporti_DAL.NDistribuitoIntervallo()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim res As Decimal = 0

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT ISNULL( SUM (   ")
            strSQL.AppendLine(" Case    ")
            strSQL.AppendLine(" WHEN rd.extra_int = 2 Then rd.qta   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 29 Then rd.qta   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 19 Then rd.qta*1000   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 304 Then rd.qta*1000   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 4 Then rd.qta*100   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 104 Then rd.qta/1000   ")
            strSQL.AppendLine("  WHEN rd.extra_int = 101 Then rd.qta/1000   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 3 Then rd.qta/1000   ")
            strSQL.AppendLine(" Else  0    ")
            strSQL.AppendLine(" End  ")
            strSQL.AppendLine("        * rdt.N/100 * rdst.Qta2), 0) as NDistribuitoIntervallo  ")

            strSQL.AppendLine(" FROM Ricette r ")
            strSQL.AppendLine(" INNER JOIN Ricette_Operazioni ro ON r.Ricetta_SuperUser = ro.Ricetta_SuperUser AND r.Ricetta_Cod = ro.Ricetta_Cod")
            strSQL.AppendLine(" INNER JOIN Ricette_Dettagli rd ON ro.Ricetta_SuperUser = rd.Ricetta_SuperUser AND ro.Ricetta_Cod = rd.Ricetta_Cod AND ro.Ricetta_Operazione_Cod = rd.Ricetta_Operazione_Cod ")
            strSQL.AppendLine(" INNER JOIN Ricette_Dettaglio_Tecnico rdt ON rd.Ricetta_SuperUser = rdt.Ricetta_SuperUser AND ")
            strSQL.AppendLine(" rd.Ricetta_Cod = rdt.Ricetta_Cod AND rd.Ricetta_Operazione_Cod = rdt.Ricetta_Operazione_Cod AND rd.Ricetta_Dettaglio_Cod = rdt.Ricetta_Dettaglio_Cod")
            strSQL.AppendLine(" INNER JOIN Ricette_Destinazioni rdst ON rd.Ricetta_SuperUser = rdst.Ricetta_SuperUser AND ")
            strSQL.AppendLine(" rd.Ricetta_Cod = rdst.Ricetta_Cod AND rd.Ricetta_Operazione_Cod = rdst.Ricetta_Operazione_Cod AND rd.Ricetta_Dettaglio_Cod = rdst.Ricetta_Dettaglio_Cod")

            strSQL.AppendLine(" WHERE r.Ricetta_SuperUser = '" & objParametri.PivaSuperUser & "'")
            strSQL.AppendLine(" AND r.piva = '" & Agro_SQL_SaveText(Piva) & "'")
            strSQL.AppendLine(" AND cau_mov = '" & Agro_SQL_SaveText(CAU_LAVORAZIONE) & "'")
            'strSQL.AppendLine(" AND r.Tipo_Ricetta = " & enum_TipoRicetta.PianoDistribuzionePua)
            strSQL.AppendLine(" AND r.Validita_Inizio >= " & Agro_SQL_SaveDate(DataPartenza))
            strSQL.AppendLine(" AND r.Validita_Inizio <= " & Agro_SQL_SaveDate(DataIntervento))

            ' aggiunta per PUA 2012, quindi la tabella fertilizzazioni non è più utilizzata
            If FerCod <> "" Then
                strSQL.AppendLine(" AND rd.elem_cod = 3 ")
                strSQL.AppendLine(" AND rd.pro_cod = " & FerCod)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                res = DT.Rows(0).Item(0)
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            res = -1
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return res

    End Function

    Public Function EfficienzaPesataConNDistribuito(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByVal Piva As String, ByVal DataPartenza As Date, ByVal DataIntervento As Date,
                                                    ByVal FerCod As String) As Decimal
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePUA_DAL.PUA_Apporti_DAL.EfficienzaPesataConNDistribuito()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim res As Decimal = 0

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT ISNULL( SUM (   ")
            strSQL.AppendLine(" Case    ")
            strSQL.AppendLine(" WHEN rd.extra_int = 2 Then rd.qta   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 29 Then rd.qta   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 19 Then rd.qta*1000   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 304 Then rd.qta*1000   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 4 Then rd.qta*100   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 104 Then rd.qta/1000   ")
            strSQL.AppendLine("  WHEN rd.extra_int = 101 Then rd.qta/1000   ")
            strSQL.AppendLine(" WHEN rd.extra_int = 3 Then rd.qta/1000   ")
            strSQL.AppendLine(" Else  0    ")
            strSQL.AppendLine(" End  ")
            strSQL.AppendLine("              * rdt.N * rdt.efficienza * rdst.Qta2), 0) as EfficienzaPesata  ")

            strSQL.AppendLine(" FROM Ricette r ")
            strSQL.AppendLine(" INNER JOIN Ricette_Operazioni ro ON r.Ricetta_SuperUser = ro.Ricetta_SuperUser AND r.Ricetta_Cod = ro.Ricetta_Cod")
            strSQL.AppendLine(" INNER JOIN Ricette_Dettagli rd ON ro.Ricetta_SuperUser = rd.Ricetta_SuperUser AND ro.Ricetta_Cod = rd.Ricetta_Cod AND ro.Ricetta_Operazione_Cod = rd.Ricetta_Operazione_Cod ")
            strSQL.AppendLine(" INNER JOIN Ricette_Dettaglio_Tecnico rdt ON rd.Ricetta_SuperUser = rdt.Ricetta_SuperUser AND ")
            strSQL.AppendLine(" rd.Ricetta_Cod = rdt.Ricetta_Cod AND rd.Ricetta_Operazione_Cod = rdt.Ricetta_Operazione_Cod AND rd.Ricetta_Dettaglio_Cod = rdt.Ricetta_Dettaglio_Cod")
            strSQL.AppendLine(" INNER JOIN Ricette_Destinazioni rdst ON rd.Ricetta_SuperUser = rdst.Ricetta_SuperUser AND ")
            strSQL.AppendLine(" rd.Ricetta_Cod = rdst.Ricetta_Cod AND rd.Ricetta_Operazione_Cod = rdst.Ricetta_Operazione_Cod AND rd.Ricetta_Dettaglio_Cod = rdst.Ricetta_Dettaglio_Cod")

            strSQL.AppendLine(" WHERE r.Ricetta_SuperUser = '" & objParametri.PivaSuperUser & "'")
            strSQL.AppendLine(" AND r.piva = '" & Agro_SQL_SaveText(Piva) & "'")
            strSQL.AppendLine(" AND cau_mov = '" & Agro_SQL_SaveText(CAU_LAVORAZIONE) & "'")
            'strSQL.AppendLine(" AND r.Tipo_Ricetta = " & enum_TipoRicetta.PianoDistribuzionePua)
            strSQL.AppendLine(" AND r.Validita_Inizio >= " & Agro_SQL_SaveDate(DataPartenza))
            strSQL.AppendLine(" AND r.Validita_Inizio <= " & Agro_SQL_SaveDate(DataIntervento))

            ' aggiunta per PUA 2012, quindi la tabella fertilizzazioni non è più utilizzata
            If FerCod <> "" Then
                strSQL.AppendLine(" AND rd.elem_cod = 3 ")
                strSQL.AppendLine(" AND rd.pro_cod = " & FerCod)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                res = DT.Rows(0).Item(0)
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            res = -1
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return res

    End Function

    Public Function NUtile_Distribuito(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Appezza As Integer, ByVal Id_Reg As Integer, ByVal Progetto_Cod As Integer,
                                              ByVal Id_Fert_escluso As Integer) As Decimal
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePUA_DAL.PUA_Apporti_DAL.NSoddisfatto()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim res As Decimal = 0

        Try

            strSQL.Length = 0


            'strSQL.AppendLine("SELECT ISNULL( SUM(Ricette_Dettaglio_Tecnico.NutilexHa), 0) ")

            strSQL.AppendLine("SELECT ISNULL( SUM ( ")

            strSQL.Append("  CASE  " & vbCrLf)
            strSQL.Append("    WHEN Ricette_Dettagli.extra_int = 2 Then Ricette_Dettagli.qta " & vbCrLf)
            strSQL.Append("    WHEN Ricette_Dettagli.extra_int = 29 Then Ricette_Dettagli.qta " & vbCrLf)
            strSQL.Append("    WHEN Ricette_Dettagli.extra_int = 19 Then Ricette_Dettagli.qta*1000 " & vbCrLf)
            strSQL.Append("    WHEN Ricette_Dettagli.extra_int = 304 Then Ricette_Dettagli.qta*1000 " & vbCrLf)
            strSQL.Append("    WHEN Ricette_Dettagli.extra_int = 4 Then Ricette_Dettagli.qta*100 " & vbCrLf)
            strSQL.Append("    WHEN Ricette_Dettagli.extra_int = 104 Then Ricette_Dettagli.qta/1000 " & vbCrLf)
            strSQL.Append("    WHEN Ricette_Dettagli.extra_int = 101 Then Ricette_Dettagli.qta/1000 " & vbCrLf)
            strSQL.Append("    WHEN Ricette_Dettagli.extra_int = 3 Then Ricette_Dettagli.qta/1000 " & vbCrLf)
            strSQL.Append("    ELSE  0  " & vbCrLf)
            strSQL.Append("  END  " & vbCrLf)

            strSQL.AppendLine("                * Ricette_Dettaglio_Tecnico.N/100 * Ricette_Dettaglio_Tecnico.efficienza), 0) ")


            strSQL.AppendLine("FROM Ricette_Dettagli INNER JOIN ")
            strSQL.AppendLine("Ricette ON Ricette_Dettagli.Ricetta_SuperUser = Ricette.Ricetta_SuperUser AND Ricette_Dettagli.Ricetta_Cod = Ricette.Ricetta_Cod INNER JOIN ")
            strSQL.AppendLine("Ricette_Dettaglio_Tecnico ON Ricette_Dettagli.Ricetta_SuperUser = Ricette_Dettaglio_Tecnico.Ricetta_SuperUser AND ")
            strSQL.AppendLine("Ricette_Dettagli.Ricetta_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Cod AND ")
            strSQL.AppendLine("Ricette_Dettagli.Ricetta_Operazione_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Operazione_Cod AND ")
            strSQL.AppendLine("Ricette_Dettagli.Ricetta_Dettaglio_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod INNER JOIN ")
            strSQL.AppendLine("Ricette_Destinazioni ON Ricette_Dettagli.Ricetta_SuperUser = Ricette_Destinazioni.Ricetta_SuperUser AND ")
            strSQL.AppendLine("Ricette_Dettagli.Ricetta_Cod = Ricette_Destinazioni.Ricetta_Cod AND ")
            strSQL.AppendLine("Ricette_Dettagli.Ricetta_Operazione_Cod = Ricette_Destinazioni.Ricetta_Operazione_Cod AND ")
            strSQL.AppendLine("Ricette_Dettagli.Ricetta_Dettaglio_Cod = Ricette_Destinazioni.Ricetta_Dettaglio_Cod INNER JOIN ")
            strSQL.AppendLine("Imprese_Progetti  ON Ricette_Destinazioni.piva = Imprese_Progetti.Piva AND ")
            strSQL.AppendLine("Ricette_Destinazioni.sa_Cod = Imprese_Progetti.sa_Cod AND ")
            strSQL.AppendLine("Ricette_Destinazioni.appezza = Imprese_Progetti.appezza AND ")
            strSQL.AppendLine("Ricette_Destinazioni.id_reg = Imprese_Progetti.id_reg  ")

            strSQL.AppendLine("WHERE Ricette_Destinazioni.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSQL.AppendLine("AND Ricette_Destinazioni.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSQL.AppendLine("AND Ricette_Destinazioni.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSQL.AppendLine("AND Ricette_Destinazioni.appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            strSQL.AppendLine("AND Ricette_Destinazioni.id_reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            strSQL.AppendLine("AND Imprese_Progetti.progetto_cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " ")

            'strSQL.AppendLine("AND Ricette.Validita_Inizio <= PE.Validita_Fine ")

            If Id_Fert_escluso <> 0 Then
                strSQL.AppendLine("AND Ricette.Ricetta_Cod <> " & Id_Fert_escluso.ToString & " ")
            End If

            strSQL.AppendLine("GROUP BY Ricette_Destinazioni.Ricetta_SuperUser, Ricette_Destinazioni.Piva, Ricette_Destinazioni.sa_cod,Ricette_Destinazioni.appezza,Ricette_Destinazioni.id_reg,Imprese_Progetti.progetto_cod ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                res = DT.Rows(0).Item(0)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            res = -1
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return res

    End Function

    Public Function NTotale_Distribuito(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Appezza As Integer, ByVal Id_Reg As Integer, ByVal Progetto_Cod As Integer,
                                      Optional ByVal Ciclo As Integer = -1,
                                      Optional ByVal Insieme_Fert As String = "",
                                      Optional ByVal strFer_Cod As String = "") As Decimal



        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePUA_DAL.PUA_Apporti_DAL.NNettoDistribuito()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim res As Decimal = 0

        Try

            strSQL.Length = 0

            'strSQL.AppendLine("SELECT ISNULL( SUM(Ricette_Dettaglio_Tecnico.NnettoxHa * Reg_Impianti.sup_imp), 0) ")

            strSQL.AppendLine("SELECT ISNULL( SUM ( ")

            strSQL.Append("  CASE  " & vbCrLf)
            strSQL.Append("    WHEN Ricette_Dettagli.extra_int = 2 Then Ricette_Dettagli.qta " & vbCrLf)
            strSQL.Append("    WHEN Ricette_Dettagli.extra_int = 29 Then Ricette_Dettagli.qta " & vbCrLf)
            strSQL.Append("    WHEN Ricette_Dettagli.extra_int = 19 Then Ricette_Dettagli.qta*1000 " & vbCrLf)
            strSQL.Append("    WHEN Ricette_Dettagli.extra_int = 304 Then Ricette_Dettagli.qta*1000 " & vbCrLf)
            strSQL.Append("    WHEN Ricette_Dettagli.extra_int = 4 Then Ricette_Dettagli.qta*100 " & vbCrLf)
            strSQL.Append("    WHEN Ricette_Dettagli.extra_int = 104 Then Ricette_Dettagli.qta/1000 " & vbCrLf)
            strSQL.Append("    WHEN Ricette_Dettagli.extra_int = 101 Then Ricette_Dettagli.qta/1000 " & vbCrLf)
            strSQL.Append("    WHEN Ricette_Dettagli.extra_int = 3 Then Ricette_Dettagli.qta/1000 " & vbCrLf)
            strSQL.Append("    ELSE  0  " & vbCrLf)
            strSQL.Append("  END  " & vbCrLf)

            'strSQL.AppendLine("                * Ricette_Dettaglio_Tecnico.N * Reg_Impianti.sup_imp), 0) ")
            strSQL.AppendLine("                * Ricette_Dettaglio_Tecnico.N/100), 0) ")

            strSQL.AppendLine("FROM Ricette INNER JOIN ")
            strSQL.AppendLine("Ricette_Operazioni ON Ricette.Ricetta_SuperUser = Ricette_Operazioni.Ricetta_SuperUser AND ")
            strSQL.AppendLine("Ricette.Ricetta_Cod = Ricette_Operazioni.Ricetta_Cod INNER JOIN ")
            strSQL.AppendLine("Ricette_Dettagli ON Ricette_Operazioni.Ricetta_SuperUser = Ricette_Dettagli.Ricetta_SuperUser AND ")
            strSQL.AppendLine("Ricette_Operazioni.Ricetta_Cod = Ricette_Dettagli.Ricetta_Cod AND ")
            strSQL.AppendLine("Ricette_Operazioni.Ricetta_Operazione_Cod = Ricette_Dettagli.Ricetta_Operazione_Cod INNER JOIN ")
            strSQL.AppendLine("Ricette_Dettaglio_Tecnico ON Ricette_Dettagli.Ricetta_SuperUser = Ricette_Dettaglio_Tecnico.Ricetta_SuperUser AND ")
            strSQL.AppendLine("Ricette_Dettagli.Ricetta_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Cod AND ")
            strSQL.AppendLine("Ricette_Dettagli.Ricetta_Operazione_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Operazione_Cod AND  ")
            strSQL.AppendLine("Ricette_Dettagli.Ricetta_Dettaglio_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod INNER JOIN ")
            strSQL.AppendLine("Ricette_Destinazioni ON Ricette_Dettagli.Ricetta_SuperUser = Ricette_Destinazioni.Ricetta_SuperUser AND ")
            strSQL.AppendLine("Ricette_Dettagli.Ricetta_Cod = Ricette_Destinazioni.Ricetta_Cod AND ")
            strSQL.AppendLine("Ricette_Dettagli.Ricetta_Operazione_Cod = Ricette_Destinazioni.Ricetta_Operazione_Cod AND ")
            strSQL.AppendLine("Ricette_Dettagli.Ricetta_Dettaglio_Cod = Ricette_Destinazioni.Ricetta_Dettaglio_Cod INNER JOIN ")
            strSQL.AppendLine("Reg_Impianti  ON Ricette_Destinazioni.piva = Reg_Impianti.Piva AND ")
            strSQL.AppendLine("Ricette_Destinazioni.sa_Cod = Reg_Impianti.sa_Cod AND ")
            strSQL.AppendLine("Ricette_Destinazioni.appezza = Reg_Impianti.appezza AND ")
            strSQL.AppendLine("Ricette_Destinazioni.id_reg = Reg_Impianti.id_reg INNER JOIN ")
            strSQL.AppendLine("Imprese_Progetti  ON Imprese_Progetti.piva = Reg_Impianti.Piva AND ")
            strSQL.AppendLine("Imprese_Progetti.sa_Cod = Reg_Impianti.sa_Cod AND ")
            strSQL.AppendLine("Imprese_Progetti.appezza = Reg_Impianti.appezza AND ")
            strSQL.AppendLine("Imprese_Progetti.id_reg = Reg_Impianti.id_reg  ")

            strSQL.AppendLine("WHERE Ricette_Destinazioni.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSQL.AppendLine("AND Ricette_Destinazioni.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSQL.AppendLine("AND Ricette_Destinazioni.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSQL.AppendLine("AND Ricette_Destinazioni.appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            strSQL.AppendLine("AND Ricette_Destinazioni.id_reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            strSQL.AppendLine("AND Imprese_Progetti.progetto_cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " ")


            If Ciclo <> -1 Then
                strSQL.AppendLine("AND Imprese_Progetti.flagsecondoraccolto = " & Agro_SQL_SaveNum(Ciclo) & " ")
            End If

            If Insieme_Fert <> "" Then
                strSQL.AppendLine("AND Ricette_Operazioni.Id_Tp_Fer IN " & Agro_SQL_Save_Clausola_IN(Insieme_Fert) & " ")
            End If

            If strFer_Cod <> "" Then
                strSQL.AppendLine("AND Ricette_Dettagli.Elem_Cod =3 ")
                strSQL.AppendLine("AND Ricette_Dettagli.Pro_Cod IN " & Agro_SQL_Save_Clausola_IN(strFer_Cod) & " ")
            End If

            strSQL.AppendLine("GROUP BY Ricette_Destinazioni.Ricetta_SuperUser, Ricette_Destinazioni.Piva, Ricette_Destinazioni.sa_cod,Ricette_Destinazioni.appezza,Ricette_Destinazioni.id_reg,Imprese_Progetti.progetto_cod ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                res = DT.Rows(0).Item(0)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            res = -1
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return res

    End Function


End Class
