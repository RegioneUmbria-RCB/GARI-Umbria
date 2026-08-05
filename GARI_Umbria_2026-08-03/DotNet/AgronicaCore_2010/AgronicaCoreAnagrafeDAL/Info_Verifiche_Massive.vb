Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider

Public Class Info_Verifiche_Massive_W
    Inherits AgronicaCoreDataProvider.DataProvider

End Class

'########################################################################
'########################################################################
'########################################################################
'########################################################################
'########################################################################
'########################################################################

Public Class Info_Verifiche_Massive_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function LeggiImpresexSpeciexOperazioniVerificaConformita(dataDa As Date,
                          dataA As Date,
                          piva As List(Of String),
                          vegCod As List(Of Integer),
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PS_Zone_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT distinct r.piva, sv.veg_cod  ")
            StrSQL.Append(" FROM reg_impianti r ")
            StrSQL.Append(" JOIN cultivar c on c.cul_Cod = r.CUL_COD ")
            StrSQL.Append(" JOIN SpecieVegetali sv on sv.Veg_Cod = c.veg_cod ")
            StrSQL.Append($" JOIN Mov_Destinazioni md ON r.piva = md.piva AND r.SA_COD = md.Sa_Cod AND r.APPEZZA = md.Appezza AND r.ID_REG = md.Id_Destinazione AND Tipo_Destinazione = {CostantiPersonalizzate.TIPO_DESTINAZIONE_IMPIANTO} ")
            StrSQL.AppendLine(" JOIN Movimenti m ON m.piva = md.Piva AND m.Sa_Cod = md.Sa_Cod AND m.Id_Agenda = md.Id_Agenda AND m.Id_Mov = md.Id_Mov ")
            StrSQL.Append(" WHERE  Data_Movimento <= " & Agro_SQL_SaveDate(dataA) & " ")
            StrSQL.Append(" AND    Data_Movimento >= " & Agro_SQL_SaveDate(dataDa) & " ")

            If (piva IsNot Nothing AndAlso piva.Any()) Then
                StrSQL.Append(" AND r.piva IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", piva), True) & ")")
            End If

            If (vegCod IsNot Nothing AndAlso vegCod.Any()) Then
                StrSQL.Append(" AND sv.veg_cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", vegCod), True) & ") ")
            End If

            '--------------------------------------------------------------------------
            StrSQL.Append(" order by r.piva, sv.veg_cod ")

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

    ' Restituisce la testata per id
    Public Function GetTestataById(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, idTestata As Integer) As DataTable
        Dim sql As New System.Text.StringBuilder
        sql.Length = 0
        sql.Append("SELECT Id_Testata ,Piva,Sa_Cod,Veg_Cod,Intervallo_Inizio,Intervallo_Fine,Dpi_Cod,Flag_Solo_Controlli_Utente,Flag_Verifica_Magazzino,Flag_Verifica_IAF,Tempo_Impiegato_ss,Numero_Operazioni_Verificate,Origine,Data_Modifica FROM Verifica_Conformita_Testata WHERE id_testata = " & Agro_SQL_SaveNum(idTestata))
        Return EseguiQuery_Lettura(objParametri, sql.ToString(), "Info_Verifiche_Massive_R.GetTestataById")
    End Function

    ' Restituisce i risultati per id testata
    Public Function GetGiacenzeMagazzinoById(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, idTestata As Integer) As DataTable
        Dim sql As New System.Text.StringBuilder
        sql.Length = 0
        sql.Append("SELECT * FROM Verifica_Conformita_Esiti_Giacenze WHERE id_testata = " & Agro_SQL_SaveNum(idTestata) & " order by Id_Testata, id_prodotto, lotto, unita_misura ")
        Return EseguiQuery_Lettura(objParametri, sql.ToString(), "Info_Verifiche_Massive_R.GetRisultatiById")
    End Function

    Public Function GetRisultatiById(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, idTestata As Integer) As DataTable
        Dim sql As New System.Text.StringBuilder
        sql.Length = 0
        sql.Append("SELECT Id_Risultato, Id_Testata, Piva, Sa_Cod, Id_Agenda, Lav_cod, Data_Operazione, Dpi_Cod, Sup_Trattata, Conforme, Conforme_Magazzino FROM Verifica_Conformita_Risultati WHERE id_testata = " & Agro_SQL_SaveNum(idTestata) & " order by Id_Agenda,Id_Testata ")
        Return EseguiQuery_Lettura(objParametri, sql.ToString(), "Info_Verifiche_Massive_R.GetRisultatiById")
    End Function

    ' Restituisce il confronto esiti raggruppati per Id_Agenda per una migliore presentazione
    Public Function GetEsitiConfrontoRaggruppati(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, idTestataNuova As Integer, idTestataVecchia As Integer) As DataTable

        Dim sql As String = "WITH EsitiConfronti AS ( " &
                            "SELECT r.Id_Agenda, " &
                            $" CASE WHEN e.Err_Code IN ({CInt(enTipoErrCode_Verifica.Engine_AcquaObbligatoria)},{CInt(enTipoErrCode_Verifica.Engine_QuantitaAcquaEccessiva)})" &
                            $" THEN {CInt(enTipoErrCode_Verifica.AcquaNonCorretta)}" &
                            $" WHEN e.Err_Code IN ({CInt(enTipoErrCode_Verifica.Engine_CarenzaNonRispettataRaccolta)})" &
                            $" THEN {CInt(enTipoErrCode_Verifica.CarenzaNonRispettata)} " &
                            $" WHEN e.Err_Code IN ({CInt(enTipoErrCode_Verifica.FitofarmacoNonBiologico)})" &
                            $" THEN {CInt(enTipoErrCode_Verifica.ProdottoNonBiologico)} " &
                            $" WHEN e.Err_Code IN ({CInt(enTipoErrCode_Verifica.DoseEccessivaRameDpi)})" &
                            $" THEN {CInt(enTipoErrCode_Verifica.DoseEccessivaRame)} " &
                            "Else e.Err_Code End As Err_Code , " &
                            "COALESCE(MIN(Case When e.Id_Testata = " & Agro_SQL_SaveNum(idTestataNuova) &
                            " Then CAST(e.Conforme As INT) End), -1) As Engine, " &
                            "COALESCE(MIN(Case When e.Id_Testata = " & Agro_SQL_SaveNum(idTestataVecchia) &
                            " Then CAST(e.Conforme As INT) End), -1) As WebService " &
                            "FROM Verifica_Conformita_Esiti e " &
                            "INNER JOIN Verifica_Conformita_Risultati r On r.Id_Risultato = e.Id_Risultato " &
                            "WHERE e.Id_Testata In (" & Agro_SQL_SaveNum(idTestataNuova) & "," & Agro_SQL_SaveNum(idTestataVecchia) & ") " &
                            "GROUP BY r.Id_Agenda, e.Err_Code " &
                            "), " &
                            "AgendaGroups As ( " &
                            "Select DISTINCT Id_Agenda FROM EsitiConfronti " &
                            ") " &
                            "Select ag.Id_Agenda, " &
                            "'Tabella: Verifica_Conformita_Esiti (Differenze Id_Agenda ' + CAST(ag.Id_Agenda AS VARCHAR) + ')' AS SectionHeader, " &
                            "ec.Err_Code, c.Descrizione, CASE WHEN MIN(ec.Engine) = 1 THEN 1 ELSE 0 END AS Engine, MAX(ec.WebService) as WebService " &
                            "FROM AgendaGroups ag " &
                            "LEFT JOIN EsitiConfronti ec ON ec.Id_Agenda = ag.Id_Agenda " &
                            "INNER JOIN CodiciErroreVerificaConformita c On c.CodiceErrore = ec.Err_Code Group by ag.Id_Agenda, ec.Err_Code, c.Descrizione " &
                            "ORDER BY ag.Id_Agenda, ec.Err_Code, c.Descrizione"
        Return EseguiQuery_Lettura(objParametri, sql, "Info_Verifiche_Massive_R.GetEsitiConfrontoRaggruppati")
    End Function

    ' Restituisce gli esiti magazzino raggruppati per Id_Agenda per una migliore presentazione
    Public Function GetEsitiMagazzinoById(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, idTestata As Integer) As DataTable
        Dim sql As String = "WITH EsitiMagazzino AS ( " &
                            "SELECT r.Id_Agenda, " &
                            "em.Id_Esito_Magazzino, em.Id_Testata, em.Id_Risultato, " &
                            "em.Piva_Magazzino, em.Sa_Cod_Magazzino, em.Fabbricato_Cod_Magazzino, " &
                            "em.Lotto, em.Conforme, em.Id_Prodotto, em.Categoria_Prodotto, " &
                            "em.Qta_Scaricata, em.Giacenza_Magazzino, em.Unita_Misura, em.Dettagli, em.Inviato " &
                            "FROM Verifica_Conformita_Esiti_Magazzino em " &
                            "INNER JOIN Verifica_Conformita_Risultati r ON r.Id_Risultato = em.Id_Risultato " &
                            "WHERE em.Id_Testata = " & Agro_SQL_SaveNum(idTestata) & " " &
                            "), " &
                            "AgendaGroups AS ( " &
                            "SELECT DISTINCT Id_Agenda FROM EsitiMagazzino " &
                            ") " &
                            "SELECT ag.Id_Agenda, " &
                            "'Tabella: Verifica_Conformita_Esiti_Magazzino (Id_Agenda ' + CAST(ag.Id_Agenda AS VARCHAR) + ')' AS SectionHeader, " &
                            "em.Id_Esito_Magazzino, em.Id_Testata, em.Id_Risultato, " &
                            "em.Piva_Magazzino, em.Sa_Cod_Magazzino, em.Fabbricato_Cod_Magazzino, " &
                            "em.Lotto, em.Conforme, em.Id_Prodotto, em.Categoria_Prodotto, " &
                            "em.Qta_Scaricata, em.Giacenza_Magazzino, em.Unita_Misura, em.Dettagli, em.Inviato " &
                            "FROM AgendaGroups ag " &
                            "LEFT JOIN EsitiMagazzino em ON em.Id_Agenda = ag.Id_Agenda " &
                            "ORDER BY ag.Id_Agenda, em.Id_Prodotto, em.Id_Testata"
        Return EseguiQuery_Lettura(objParametri, sql, "Info_Verifiche_Massive_R.GetEsitiMagazzinoById")
    End Function

End Class