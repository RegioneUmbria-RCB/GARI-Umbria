Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.attivita.Attivita

Public Class Visite_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
                        ByVal Piva As String,
                        ByVal Sa_Cod As Integer,
                        ByVal Id_Agenda As Integer,
                        ByVal xFiltroAggiuntivo As String,
                        ByVal xOrderBy As String,
                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        Optional numeroRigheDaEstrattare As Integer? = Nothing
                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreVisiteDAL.Visite_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT ")
            If numeroRigheDaEstrattare.HasValue Then
                StrSQL.AppendLine("TOP " & numeroRigheDaEstrattare.Value)
            End If

            StrSQL.AppendLine(" m.Data_Movimento, RIGHT('0' + CAST(DATEPART(hh,m.Ora) AS varchar),2) + ':' + RIGHT('0' + CAST(DATEPART(mi,m.Ora) AS varchar),2) AS Ora_Movimento, ")
            StrSQL.AppendLine(" a.Piva, i.rag_soc, a.Sa_Cod, CASE a.Sa_Cod WHEN -1 THEN '' ELSE ca.Sa_Nome END AS 'Centro_Aziendale', ")
            StrSQL.AppendLine(" a.Id_Agenda, a.Lav_Cod, a.Des_Lib AS Lav_Des, m.Mov_Desc as Note, a.Username_Creazione, CASE ud.Flag_Azienda_Persona WHEN 1 THEN ud.Rag_Soc WHEN 2 THEN ud.Cognome + ' - ' + ud.Nome END AS 'Operatore', ")

            'Concateno il nome delle sotto-operazioni
            StrSQL.AppendLine(" 	 STUFF((SELECT DISTINCT ', ' + ISNULL(att.[desc],o.LAV_DES) AS [text()] ")
            StrSQL.AppendLine(" 	 from Mov_Dettagli_Riferimenti rif ")
            StrSQL.AppendLine(" 	 INNER JOIN Agenda a2 ON a2.piva=rif.Piva_Rif AND  a2.Sa_Cod=rif.Sa_Cod_Rif AND  a2.id_agenda=rif.id_agenda_rif ")
            StrSQL.AppendLine(" 	 INNER JOIN Operazioni o ON a2.lav_cod=o.lav_cod ")
            StrSQL.AppendLine(" 	 LEFT  JOIN Attivita att ON a2.id_Attivita=att.ID_Attivita ")
            StrSQL.AppendLine(" 	 WHERE a.piva=rif.piva AND a.Sa_Cod=rif.Sa_Cod AND a.Id_Agenda=rif.Id_Agenda ")
            StrSQL.AppendLine(" 	 FOR XML PATH('')), 1, 1, '' ) as 'AttivitaSvolte', ")
            StrSQL.AppendLine(" 	 a.Data_Modifica ")
            StrSQL.AppendLine(" FROM Agenda a")
            StrSQL.AppendLine(" INNER JOIN Movimenti m ON a.PIVA = m.PIVA AND a.Sa_Cod = m.Sa_Cod AND a.Id_Agenda = m.Id_Agenda")
            StrSQL.AppendLine(" INNER JOIN Imprese i ON i.PIVA = a.PIVA ")
            StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali ca ON ca.PIVA = a.PIVA and ca.sa_cod = a.sa_cod ")
            StrSQL.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_dettagli ud ON a.Username_Creazione = ud.CodFisc ")
            'StrSQL.AppendLine(" LEFT JOIN Mov_Dettagli_Riferimenti rif ON a.Username_Creazione = ud.CodFisc ")

            StrSQL.AppendLine(" WHERE a.Lav_Cod = " & LAVCOD_VISITA)
            'StrSQL.AppendLine(" WHERE a.Lav_Cod In (" & LAVCOD_VISITA.ToString() & ", " & LAVCOD_VISITA_GENERICA.ToString() & ", " & LAVCOD_MONITORAGGIO_TEMPI_RIENTRO.ToString() & ")")

            If Piva <> "" Then
                StrSQL.AppendLine(" And a.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND a.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString()))
            End If

            If Id_Agenda <> 0 Then
                StrSQL.AppendLine(" AND a.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda.ToString()))
            End If

            If objParametri_Server.FinestraTemporaleInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND m.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            End If

            If objParametri_Server.FinestraTemporaleFine <> AGRODATAFINE Then
                StrSQL.AppendLine(" AND m.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                StrSQL.AppendLine(" ORDER BY a.PIVA, m.Data_Movimento DESC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiDettagli(
                    ByVal Piva As String,
                    ByVal Sa_Cod As Integer,
                    ByVal Id_Agenda As Integer,
                    ByVal xFiltroAggiuntivo As String,
                    ByVal xOrderBy As String,
                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                    Optional ByVal topNRighe As Integer? = Nothing
                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreVisiteDAL.Visite_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            StrSQL.Length = 0

            If Not IsNothing(topNRighe) AndAlso topNRighe.HasValue Then
                StrSQL.AppendLine(" SELECT DISTINCT TOP " + topNRighe.Value.ToString + " ")
            Else
                StrSQL.AppendLine(" SELECT ")
            End If

            StrSQL.AppendLine(" m.Data_Movimento, RIGHT('0' + CAST(DATEPART(hh,m.Ora) AS varchar),2) + ':' + RIGHT('0' + CAST(DATEPART(mi,m.Ora) AS varchar),2) AS Ora_Movimento, ")
            StrSQL.AppendLine(" a.Piva, i.rag_soc, a_Rif.Sa_Cod, CASE a_Rif.Sa_Cod WHEN -1 THEN '' ELSE ca.Sa_Nome END AS 'Sa_Nome', ")
            StrSQL.AppendLine(" a.Id_Agenda, a.Lav_Cod, a.Des_Lib AS Lav_Des, m.Mov_Desc as Note, a.Username_Creazione, CASE ud.Flag_Azienda_Persona WHEN 1 THEN ud.Rag_Soc WHEN 2 THEN ud.Cognome + ' - ' + ud.Nome END AS 'Operatore', ")
            StrSQL.AppendLine(" a_Rif.lav_Cod AS Lav_Cod_Rif, a_Rif.id_attivita As id_Attivita_Rif, ISNULL(att.[Desc], o.LAV_DES) AS 'Operazione' , md_Rif.Mov_Det_Des AS 'Descrizione'")
            StrSQL.AppendLine(" FROM Agenda a")
            StrSQL.AppendLine(" INNER JOIN Movimenti m ON a.PIVA = m.PIVA AND a.Sa_Cod = m.Sa_Cod AND a.Id_Agenda = m.Id_Agenda")
            StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli md ON m.PIVA = md.PIVA And m.Id_Agenda = md.Id_Agenda AND m.Id_mov = md.Id_mov")
            StrSQL.AppendLine(" INNER JOIN Mov_Dettagli_Riferimenti mdr ON md.PIVA = mdr.PIVA AND md.Id_Agenda = mdr.Id_Agenda AND md.Id_mov = mdr.Id_mov AND md.Id_mov_det = mdr.Id_mov_det")
            StrSQL.AppendLine(" INNER JOIN Agenda a_Rif ON mdr.Piva_Rif=a_Rif.Piva AND mdr.Sa_Cod_Rif=a_Rif.Sa_Cod AND mdr.id_Agenda_Rif=a_Rif.id_Agenda")
            StrSQL.AppendLine(" INNER JOIN Movimenti m_Rif ON m_Rif.Piva=a_Rif.Piva AND m_Rif.Sa_Cod=a_Rif.Sa_Cod AND m_Rif.id_Agenda=a_Rif.id_Agenda")
            StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli md_Rif ON m_Rif.Piva=md_Rif.Piva AND m_Rif.Sa_Cod=md_Rif.Sa_Cod AND m_Rif.id_Agenda=md_Rif.id_Agenda AND m_Rif.id_mov=md_Rif.id_mov")
            StrSQL.AppendLine(" INNER JOIN Operazioni o ON o.lav_cod = a_Rif.Lav_Cod")
            StrSQL.AppendLine(" LEFT  JOIN Attivita att ON att.id_attivita = a_Rif.id_attivita")
            StrSQL.AppendLine(" INNER JOIN Imprese i ON i.PIVA = a.PIVA ")
            StrSQL.AppendLine(" LEFT  JOIN Centri_Aziendali ca ON ca.PIVA = a_Rif.PIVA AND ca.sa_cod = a_Rif.sa_cod ")
            StrSQL.AppendLine(" LEFT  JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_dettagli ud ON a.Username_Creazione = ud.CodFisc ")

            StrSQL.AppendLine(" WHERE a.Lav_Cod = " & LAVCOD_VISITA)
            'StrSQL.AppendLine(" WHERE a.Lav_Cod In (" & LAVCOD_VISITA.ToString() & ", " & LAVCOD_VISITA_GENERICA.ToString() & ", " & LAVCOD_MONITORAGGIO_TEMPI_RIENTRO.ToString() & ")")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND a.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND a.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString()))
            End If

            If Id_Agenda <> 0 Then
                StrSQL.AppendLine(" AND a.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda.ToString()))
            End If

            If objParametri_Server.FinestraTemporaleInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND m.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            End If

            If objParametri_Server.FinestraTemporaleFine <> AGRODATAFINE Then
                StrSQL.AppendLine(" AND m.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                StrSQL.AppendLine(" ORDER BY m.Data_Movimento DESC, rag_soc ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiDettagliVisite(
                ByVal Sa_Cod As Integer,
                ByVal Id_Agenda As Integer,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                ByVal usernames As String(),
                Optional ByVal topNRighe As Integer? = Nothing
                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreVisiteDAL.Visite_R.LeggiDettagliVisite()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim isSuperUser As Boolean = (objParametri_Utenti.SuperUserUsername = objParametri_Utenti.UtenteUsername)

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            StrSQL.Length = 0

            If Not IsNothing(topNRighe) AndAlso topNRighe.HasValue Then
                StrSQL.AppendLine(" SELECT DISTINCT TOP " + topNRighe.Value.ToString + " ")
            Else
                StrSQL.AppendLine(" SELECT ")
            End If

            StrSQL.AppendLine(" m.Data_Movimento, RIGHT('0' + CAST(DATEPART(hh,m.Ora) AS varchar),2) + ':' + RIGHT('0' + CAST(DATEPART(mi,m.Ora) AS varchar),2) AS Ora_Movimento, ")
            StrSQL.AppendLine(" a.Piva, i.rag_soc, a_Rif.Sa_Cod, CASE a_Rif.Sa_Cod WHEN -1 THEN '' ELSE ca.Sa_Nome END AS 'Sa_Nome', ")
            StrSQL.AppendLine(" a.Id_Agenda, a.Lav_Cod, a.Des_Lib AS Lav_Des, m.Mov_Desc as Note, c.Cognome + ' - ' + c.Nome AS 'Operatore', ")
            StrSQL.AppendLine(" a_Rif.lav_Cod AS Lav_Cod_Rif, a_Rif.id_attivita As id_Attivita_Rif, ISNULL(att.[Desc], o.LAV_DES) AS 'Operazione' , md_Rif.Mov_Det_Des AS 'Descrizione'")
            StrSQL.AppendLine(" FROM Agenda a")
            StrSQL.AppendLine(" INNER JOIN Movimenti m ON a.PIVA = m.PIVA AND a.Sa_Cod = m.Sa_Cod AND a.Id_Agenda = m.Id_Agenda")
            StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli md ON m.PIVA = md.PIVA And m.Id_Agenda = md.Id_Agenda AND m.Id_mov = md.Id_mov")
            StrSQL.AppendLine(" INNER JOIN Mov_Dettagli_Riferimenti mdr ON md.PIVA = mdr.PIVA AND md.Id_Agenda = mdr.Id_Agenda AND md.Id_mov = mdr.Id_mov AND md.Id_mov_det = mdr.Id_mov_det")
            StrSQL.AppendLine(" INNER JOIN Agenda a_Rif ON mdr.Piva_Rif=a_Rif.Piva AND mdr.Sa_Cod_Rif=a_Rif.Sa_Cod AND mdr.id_Agenda_Rif=a_Rif.id_Agenda")
            StrSQL.AppendLine(" INNER JOIN Movimenti m_Rif ON m_Rif.Piva=a_Rif.Piva AND m_Rif.Sa_Cod=a_Rif.Sa_Cod AND m_Rif.id_Agenda=a_Rif.id_Agenda")
            StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli md_Rif ON m_Rif.Piva=md_Rif.Piva AND m_Rif.Sa_Cod=md_Rif.Sa_Cod AND m_Rif.id_Agenda=md_Rif.id_Agenda AND m_Rif.id_mov=md_Rif.id_mov")
            StrSQL.AppendLine(" INNER JOIN Operazioni o ON o.lav_cod = a_Rif.Lav_Cod")
            StrSQL.AppendLine(" LEFT  JOIN Attivita att ON att.id_attivita = a_Rif.id_attivita")
            StrSQL.AppendLine(" INNER JOIN Imprese i ON i.PIVA = a.PIVA ")
            StrSQL.AppendLine(" LEFT  JOIN Centri_Aziendali ca ON ca.PIVA = a_Rif.PIVA AND ca.sa_cod = a_Rif.sa_cod ")
            StrSQL.AppendLine(" INNER JOIN Movimenti m_assegnatario ON m_assegnatario.Piva=a.Piva AND m_assegnatario.Sa_Cod=a.Sa_Cod AND m_assegnatario.id_Agenda=a.id_Agenda AND m_assegnatario.Cau_Mov=6852 ")
            StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli md_assegnatario ON m_assegnatario.Piva=md_assegnatario.Piva AND m_assegnatario.Sa_Cod=md_assegnatario.Sa_Cod AND m_assegnatario.id_Agenda=md_assegnatario.id_Agenda AND m_assegnatario.id_mov=md_assegnatario.id_mov ")
            StrSQL.AppendLine(" INNER JOIN Risorse_Umane ru ON ru.Cod_RisUm = md_assegnatario.mat_cod ")
            StrSQL.AppendLine(" INNER JOIN Contatti c ON c.Cod_Contatto = ru.Cod_Contatto AND c.Piva = ru.Piva ")

            If isSuperUser = False Then
                StrSQL.AppendLine(" INNER JOIN ContattiXUtentiGias cug ON cug.piva = c.piva AND cug.cod_contatto = c.Cod_contatto AND (cug.username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & " OR cug.username IN( " & Agro_SQL_Save_Clausola_IN("'" & String.Join("','", usernames) & "'") & " )) ")
            End If

            StrSQL.AppendLine(" WHERE a.Lav_Cod = " & LAVCOD_VISITA)
            'StrSQL.AppendLine(" WHERE a.Lav_Cod In (" & LAVCOD_VISITA.ToString() & ", " & LAVCOD_VISITA_GENERICA.ToString() & ", " & LAVCOD_MONITORAGGIO_TEMPI_RIENTRO.ToString() & ")")

            'If Piva <> "" Then
            '    StrSQL.AppendLine(" AND a.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            'End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND a.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString()))
            End If

            If Id_Agenda <> 0 Then
                StrSQL.AppendLine(" AND a.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda.ToString()))
            End If

            If objParametri_Server.FinestraTemporaleInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND m.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            End If

            If objParametri_Server.FinestraTemporaleFine <> AGRODATAFINE Then
                StrSQL.AppendLine(" AND m.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                StrSQL.AppendLine(" ORDER BY m.Data_Movimento DESC, rag_soc ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiDettagliVisiteNew(
                ByVal veg_cod As Integer,
                ByVal id_cod As Integer,
                ByVal usernameFilter As String,
                ByVal pivaAzienda As String,
                ByVal tipoVisita As Integer,
                ByVal dataDa As Date,
                ByVal dataA As Date,
                ByVal pivaCentro As String,
                ByVal saCodCentro As String,
                ByVal impiantiSelezionati As String(),
                ByVal operazioniFilter As Integer(),
                ByVal Gen_Cod As Integer,
                ByVal Spe_Cod As Integer,
                ByVal IPro_Cod As Integer,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                ByVal usernames As String(),
                ByVal existsImpresa As Boolean,
                Optional ByVal topNRighe As Integer? = Nothing
                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreVisiteDAL.Visite_R.LeggiDettagliVisiteNew()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim isSuperUser As Boolean = (objParametri_Utenti.SuperUserUsername = objParametri_Utenti.UtenteUsername)
        Dim isSpecieVegetale As Boolean = True

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            StrSQL.Length = 0

            'Creo la CTE che contiene i rilievi collegati alle Visite
            StrSQL.AppendLine(" with Rilievi_con_visite_cte as  ( ")
            StrSQL.AppendLine("     select distinct ")
            StrSQL.AppendLine("     Mov_Dettagli_Riferimenti.Id_Agenda as IdAgenda_Visita , Agenda.Validita_Inizio as Validita_Inizio_Rilievo,")
            StrSQL.AppendLine("     MAX(Agenda.Id_Agenda) as IdAgenda_Rilievo, STRING_AGG(Appezzamento.app_nome, ', ') AS Nome_Impianto, Agenda.Blocco_Flag As Blocco_Flag_Rilievo, Agenda.Lav_Cod As Lav_Cod_Rilievo, Operazioni.LAV_DES as Lav_Des_Rilievo, Agenda.Sa_Cod as Sa_Cod_Rilievo, Agenda.Piva as Piva_Rilievo,")
            StrSQL.AppendLine("     SV.Veg_Cod, SV.Veg_Des, Codici_Anagrafe.codice as Cod_DestUso, Codici_Anagrafe.descrizione as Des_DestUso ")
            StrSQL.AppendLine("     from Agenda ")
            StrSQL.AppendLine("     inner join Operazioni on Operazioni.Lav_Cod = Agenda.Lav_Cod")
            StrSQL.AppendLine("     inner join Movimenti on  Agenda.PIVA = Movimenti.PIVA and  Agenda.Sa_Cod = Movimenti.Sa_Cod and Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            StrSQL.AppendLine("     inner join Movimenti_dettagli on  Agenda.PIVA = Movimenti_dettagli. PIVA and  Agenda.Sa_Cod = Movimenti_dettagli.Sa_Cod and ")
            StrSQL.AppendLine("     Agenda.Id_Agenda = Movimenti_dettagli.Id_Agenda and Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov")
            StrSQL.AppendLine("     inner join Mov_Dettagli_Riferimenti on  Agenda.PIVA = Mov_Dettagli_Riferimenti.PIVA and")
            StrSQL.AppendLine("           Agenda.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda_Rif and Mov_Dettagli_Riferimenti.Lav_Cod = " & LAVCOD_VISITA & " and Mov_Dettagli_Riferimenti.Cau_Mov = '" & CAU_VISITE_ISPETTIVE & "'")
            StrSQL.AppendLine("     LEFT OUTER JOIN Mov_Destinazioni mdest ON Agenda.Piva = mdest.Piva AND Agenda.id_Agenda = mdest.Id_Agenda And mdest.Sa_Cod = Agenda.Sa_Cod and Movimenti_dettagli.id_mov = mdest.id_mov and Movimenti_dettagli.Id_Mov_Det = mdest.Id_Mov_Det and mdest.Tipo_Destinazione = 0")
            StrSQL.AppendLine("     LEFT OUTER JOIN Reg_Impianti ON Reg_Impianti.PIVA = mdest.Piva AND Reg_Impianti.SA_COD = mdest.Sa_Cod AND Reg_Impianti.APPEZZA = mdest.Appezza AND Reg_Impianti.ID_REG = mdest.Id_Destinazione")
            StrSQL.AppendLine("     LEFT OUTER JOIN Reg_Impianti_Codici ric ON ric.PIVA = mdest.Piva AND ric.SA_COD = mdest.Sa_Cod AND ric.APPEZZA = mdest.Appezza AND ric.ID_REG = mdest.Id_Destinazione AND ric.id_cod between 3000 and 3999 ")
            StrSQL.AppendLine("     LEFT OUTER JOIN cultivar c ON c.Cul_Cod = Reg_Impianti.CUL_COD ")
            StrSQL.AppendLine("     LEFT OUTER JOIN SpecieVegetali SV on ((Movimenti_dettagli.Dettaglio_VegCod <> 0 AND SV.Veg_Cod = Movimenti_dettagli.Dettaglio_VegCod) OR SV.Veg_Cod = c.veg_cod) ")
            StrSQL.AppendLine("     LEFT OUTER JOIN Codici_Anagrafe on ((Movimenti_dettagli.Dettaglio_IdCod <> 0 AND Codici_Anagrafe.codice = Movimenti_dettagli.Dettaglio_IdCod) OR Codici_Anagrafe.codice = ric.id_cod) ")
            StrSQL.AppendLine("     LEFT OUTER JOIN Appezzamento ON Reg_Impianti.PIVA = Appezzamento.Piva AND Reg_Impianti.SA_COD = Appezzamento.Sa_Cod AND Reg_Impianti.APPEZZA = Appezzamento.Appezza")
            StrSQL.AppendLine("           where Agenda.Lav_Cod IN (" & STR_OP_COLLEGABILI_A_VISITE_NG & ")")
            StrSQL.AppendLine("              and Movimenti.Cau_Mov in (" & CAU_RILIEVO_CAMPO & ", " & CAU_RILIEVO_RACCOLTA & ")")
            StrSQL.AppendLine("     group by Mov_Dettagli_Riferimenti.Id_Agenda, Agenda.Validita_Inizio, Blocco_Flag, Agenda.Lav_Cod, LAV_DES, Agenda.Sa_Cod, Agenda.Piva, SV.Veg_Cod, SV.Veg_Des, Codici_Anagrafe.codice, Codici_Anagrafe.descrizione")
            StrSQL.AppendLine(")")

            If Not IsNothing(topNRighe) AndAlso topNRighe.HasValue Then
                StrSQL.AppendLine(" SELECT DISTINCT TOP " + topNRighe.Value.ToString + " ")
            Else
                StrSQL.AppendLine(" SELECT DISTINCT ")
            End If

            StrSQL.AppendLine(" m.Data_Movimento, RIGHT('0' + CAST(DATEPART(hh,m.Ora) AS varchar),2) + ':' + RIGHT('0' + CAST(DATEPART(mi,m.Ora) AS varchar),2) AS Ora_Movimento, ")
            StrSQL.AppendLine(" a.Piva, i.rag_soc, a.Sa_Cod, CASE a.Sa_Cod WHEN -1 THEN '' ELSE ca.Sa_Nome END AS 'Sa_Nome', ")
            StrSQL.AppendLine(" a.Id_Agenda, a.Lav_Cod, a.Des_Lib AS Lav_Des, m.Mov_Desc as Note, c.Cognome + ' - ' + c.Nome AS 'Operatore', ")
            StrSQL.AppendLine(" a.id_attivita As id_Attivita_Rif, ISNULL(att.[Desc], o.LAV_DES) AS 'Operazione' , md.Mov_Det_Des AS 'Descrizione', ")
            StrSQL.AppendLine(" CASE a.Stato_Cod WHEN " & StatiWorkflowQdC.Eseguito & " THEN '" & AgronicaCoreDataProvider.My.Resources.Gias.Eseguita & "' ELSE '" & AgronicaCoreDataProvider.My.Resources.Gias.DaEseguire & "' END AS 'Stato_Visita', a.DaRemoto, ")
            StrSQL.AppendLine(" STRING_AGG(Appezzamento.app_nome, ', ') AS Appezza_Visite, ")
            StrSQL.AppendLine(" MAX(ISNULL(SpecieVegetali.Veg_Cod, 0)) AS Veg_Cod, ")
            StrSQL.AppendLine(" MAX(ISNULL(SpecieVegetali.Veg_Des, ISNULL(Rilievi_con_visite_cte.Veg_Des, ''))) AS Veg_Des, ")
            StrSQL.AppendLine(" MAX(ISNULL(Codici_Anagrafe.codice, ISNULL(Rilievi_con_visite_cte.Cod_DestUso, 0))) AS Id_Cod, ")
            StrSQL.AppendLine(" MAX(ISNULL(Codici_Anagrafe.descrizione, ISNULL(Rilievi_con_visite_cte.Des_DestUso, ''))) AS Id_Des, ")
            StrSQL.AppendLine(" MAX(ISNULL(Lista_IndirizziProd_Animali.GEN_COD, 0)) AS Gen_Cod, ")
            StrSQL.AppendLine(" MAX(ISNULL(Lista_IndirizziProd_Animali.SPE_COD, 0)) AS Spe_Cod, ")
            StrSQL.AppendLine(" MAX(ISNULL(Lista_IndirizziProd_Animali.IPRO_COD, 0)) AS IPro_Cod, ")
            StrSQL.AppendLine(" MAX(ISNULL(Lista_IndirizziProd_Animali.IPRO_DES, '')) AS IPro_Des, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.IdAgenda_Rilievo, 0) AS Id_Agenda_Rif, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Validita_Inizio_Rilievo AS Validita_Inizio_Rif, ")
            StrSQL.AppendLine(" ISNULL(CoordinateGIS.Latitude, 0) AS Latitude, ")
            StrSQL.AppendLine(" ISNULL(CoordinateGIS.Longitude, 0) AS Longitude, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.Blocco_Flag_Rilievo, 0) AS Blocco_Flag_Rif, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.Lav_Cod_Rilievo, 0) AS Lav_Cod_Rif, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.Lav_Des_Rilievo, '') AS Lav_Des_Rif, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.Sa_Cod_Rilievo, 0) AS Sa_Cod_Rif, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.Nome_Impianto, '') AS Nome_Impianto ")


            StrSQL.AppendLine(" FROM Agenda a ")
            StrSQL.AppendLine(" INNER JOIN Movimenti m ON a.PIVA = m.PIVA AND a.Sa_Cod = m.Sa_Cod AND a.Id_Agenda = m.Id_Agenda AND m.Cau_Mov <> 6852 ")
            StrSQL.AppendLine(" LEFT JOIN Movimenti_dettagli md ON m.PIVA = md.PIVA And m.Id_Agenda = md.Id_Agenda AND m.Id_mov = md.Id_mov ")
            StrSQL.AppendLine(" INNER JOIN Operazioni o ON o.lav_cod = a.Lav_Cod ")
            StrSQL.AppendLine(" LEFT  JOIN Attivita att ON att.id_attivita = a.id_attivita ")
            StrSQL.AppendLine(" INNER JOIN Imprese i ON i.PIVA = a.PIVA ")
            StrSQL.AppendLine(" LEFT  JOIN Centri_Aziendali ca ON ca.PIVA = a.PIVA AND ca.sa_cod = a.sa_cod ")
            StrSQL.AppendLine(" INNER JOIN Movimenti m_assegnatario ON m_assegnatario.Piva=a.Piva AND m_assegnatario.Sa_Cod=a.Sa_Cod AND m_assegnatario.id_Agenda=a.id_Agenda AND m_assegnatario.Cau_Mov=6852  ")
            StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli md_assegnatario ON m_assegnatario.Piva=md_assegnatario.Piva AND m_assegnatario.Sa_Cod=md_assegnatario.Sa_Cod AND m_assegnatario.id_Agenda=md_assegnatario.id_Agenda AND m_assegnatario.id_mov=md_assegnatario.id_mov ")
            StrSQL.AppendLine(" INNER JOIN Risorse_Umane ru ON ru.Cod_RisUm = md_assegnatario.mat_cod ")
            StrSQL.AppendLine(" INNER JOIN Contatti c ON c.Cod_Contatto = ru.Cod_Contatto AND c.Piva = ru.Piva ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Mov_Destinazioni mdest ON a.Piva = mdest.Piva AND a.id_Agenda = mdest.Id_Agenda And mdest.Sa_Cod = a.Sa_Cod and md.id_mov = mdest.id_mov and md.Id_Mov_Det = mdest.Id_Mov_Det and mdest.Tipo_Destinazione = 0 ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Reg_Impianti ON Reg_Impianti.PIVA = mdest.Piva AND Reg_Impianti.SA_COD = mdest.Sa_Cod AND Reg_Impianti.APPEZZA = mdest.Appezza AND Reg_Impianti.ID_REG = mdest.Id_Destinazione ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Reg_Impianti_Codici ric ON ric.PIVA = mdest.Piva AND ric.SA_COD = mdest.Sa_Cod AND ric.APPEZZA = mdest.Appezza AND ric.ID_REG = mdest.Id_Destinazione AND ric.id_cod between 3000 and 3999 ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Appezzamento ON Reg_Impianti.PIVA = Appezzamento.Piva AND Reg_Impianti.SA_COD = Appezzamento.Sa_Cod AND Reg_Impianti.APPEZZA = Appezzamento.Appezza ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Cultivar on Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
            StrSQL.AppendLine(" LEFT OUTER JOIN SpecieVegetali on ((md.Dettaglio_VegCod <> 0 AND SpecieVegetali.Veg_Cod = md.Dettaglio_VegCod) OR SpecieVegetali.Veg_Cod = Cultivar.veg_cod) ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Codici_Anagrafe on ((md.Dettaglio_IdCod <> 0 AND Codici_Anagrafe.codice = md.Dettaglio_IdCod) OR Codici_Anagrafe.codice = ric.id_cod) ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Lista_IndirizziProd_Animali on (md.Dettaglio_GenCod <> 0 AND Lista_IndirizziProd_Animali.GEN_COD = md.Dettaglio_GenCod AND Lista_IndirizziProd_Animali.SPE_COD = md.Dettaglio_SpeCod AND Lista_IndirizziProd_Animali.IPRO_COD = md.Dettaglio_IProCod) ")
            StrSQL.AppendLine(" LEFT OUTER JOIN (	SELECT DISTINCT GIS_Entita.Piva, GIS_Entita.ID_Agenda, GIS_ElementiGrafici.Poligono_GeoEntity.Lat AS Latitude, GIS_ElementiGrafici.Poligono_GeoEntity.Long AS Longitude ")
            StrSQL.AppendLine("                      FROM GIS_Entita ")
            StrSQL.AppendLine("                     LEFT OUTER JOIN GIS_ElementiGrafici ON GIS_Entita.Entita_Cod = GIS_ElementiGrafici.Entita_Cod AND GIS_ElementiGrafici.LayerElementiGrafici_Cod = " & enum_Gis_LayerElementiGrafici_std.Op_Agenda & " ")
            StrSQL.AppendLine("                     WHERE ID_Agenda > 0 ")
            StrSQL.AppendLine("  ) CoordinateGIS On CoordinateGIS.Piva = a.PIVA And CoordinateGIS.ID_Agenda = a.ID_Agenda ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Rilievi_con_visite_cte On Rilievi_con_visite_cte.IdAgenda_Visita = a.Id_Agenda And Rilievi_con_visite_cte.Piva_Rilievo = a.Piva")

            If isSuperUser = False Or usernameFilter <> "" Then
                StrSQL.AppendLine(" INNER JOIN ContattiXUtentiGias cug On cug.piva = c.piva And cug.cod_contatto = c.Cod_contatto ")
                If usernameFilter <> "" Then
                    StrSQL.AppendLine(" And (cug.username = " & Agro_SQL_SaveText_NULL(usernameFilter) & " ")
                Else
                    StrSQL.AppendLine(" And (cug.username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & " ")
                    If usernames IsNot Nothing AndAlso usernames.Count > 0 Then
                        StrSQL.AppendLine(" OR cug.username IN( " & Agro_SQL_Save_Clausola_IN(String.Join(",", usernames), True) & " ) ")
                    End If
                End If
                StrSQL.AppendLine(" ) ")
            End If

            StrSQL.AppendLine(" WHERE a.Lav_Cod = 5007 ")

            If isSuperUser = False AndAlso existsImpresa = True Then
                StrSQL.AppendLine(" AND i.PIVA IN (SELECT DISTINCT piva from Utenti_Visibilita_Appoggio WHERE Entita_Cod = 1 AND Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & " ) ")
            End If

            If veg_cod <> -1 Then
                If veg_cod <> 0 Then
                    StrSQL.AppendLine(" AND (SpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(veg_cod.ToString()) & " OR Rilievi_con_visite_cte.Veg_Cod = " & Agro_SQL_SaveNum(veg_cod.ToString()) & ") ")
                Else
                    StrSQL.AppendLine(" AND SpecieVegetali.Veg_Cod IS NULL AND Rilievi_con_visite_cte.Veg_Cod IS NULL AND Codici_Anagrafe.codice IS NULL AND Rilievi_con_visite_cte.Cod_DestUso IS NULL ")
                End If
            Else
                If id_cod <> -1 Then
                    StrSQL.AppendLine(" AND (Codici_Anagrafe.codice = " & Agro_SQL_SaveNum(id_cod.ToString()) & " OR Rilievi_con_visite_cte.Cod_DestUso = " & Agro_SQL_SaveNum(id_cod.ToString()) & ") ")
                End If
            End If

            If pivaAzienda <> "" Then
                StrSQL.AppendLine(" AND a.piva = " & Agro_SQL_SaveText_NULL(pivaAzienda))
            End If

            If tipoVisita <> 0 Then
                StrSQL.AppendLine(" AND a.Stato_Cod = " & Agro_SQL_SaveNum(tipoVisita.ToString()))
            End If

            If (pivaCentro <> "" And saCodCentro <> "") AndAlso (pivaCentro <> "-1" And saCodCentro <> "0") Then
                StrSQL.AppendLine(" AND ca.PIVA = " & Agro_SQL_SaveText_NULL(pivaCentro) & " AND ca.sa_cod = " & Agro_SQL_SaveText_NULL(saCodCentro))
            End If

            If operazioniFilter IsNot Nothing AndAlso operazioniFilter.Count > 0 Then
                StrSQL.AppendLine(" AND a.id_attivita IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", operazioniFilter)) & ") ")
            End If

            If Gen_Cod <> -1 AndAlso Spe_Cod <> -1 AndAlso IPro_Cod <> -1 Then
                StrSQL.AppendLine(" AND Lista_IndirizziProd_Animali.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod.ToString()) & " ")
                StrSQL.AppendLine(" AND Lista_IndirizziProd_Animali.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod.ToString()) & " ")
                StrSQL.AppendLine(" AND Lista_IndirizziProd_Animali.IPro_Cod = " & Agro_SQL_SaveNum(IPro_Cod.ToString()) & " ")
            End If

            If impiantiSelezionati IsNot Nothing AndAlso impiantiSelezionati.Count > 0 Then
                StrSQL.AppendLine(" AND (")
                For ctr As Integer = 0 To (impiantiSelezionati.Count - 1)
                    Dim chiavi = impiantiSelezionati(ctr).Split(CChar("_"))
                    StrSQL.AppendLine("( ")
                    StrSQL.AppendLine(" Reg_Impianti.PIVA = " & chiavi(0) & " AND Reg_Impianti.SA_COD = " & chiavi(1) & " AND Reg_Impianti.APPEZZA = " & chiavi(2))
                    StrSQL.AppendLine(" )")
                    If ctr < (impiantiSelezionati.Count - 1) Then
                        StrSQL.AppendLine(" OR ")
                    End If
                Next
                StrSQL.AppendLine(" )")
            End If

            StrSQL.AppendLine(" AND m.Data_Movimento between " + Agro_SQL_SaveDate(dataDa) + " AND " + Agro_SQL_SaveDate(dataA) + " ")

            StrSQL.AppendLine(" GROUP BY m.Data_Movimento, ")
            StrSQL.AppendLine(" RIGHT('0' + CAST(DATEPART(hh, m.Ora) AS varchar), 2) + ':' + RIGHT('0' + CAST(DATEPART(mi, m.Ora) AS varchar), 2), ")
            StrSQL.AppendLine(" a.Piva, ")
            StrSQL.AppendLine(" i.rag_soc, ")
            StrSQL.AppendLine(" a.Sa_Cod, ")
            StrSQL.AppendLine(" CASE a.Sa_Cod WHEN -1 THEN '' ELSE ca.Sa_Nome END, ")
            StrSQL.AppendLine(" a.Id_Agenda, ")
            StrSQL.AppendLine(" a.Lav_Cod, ")
            StrSQL.AppendLine(" a.Des_Lib, ")
            StrSQL.AppendLine(" m.Mov_Desc, ")
            StrSQL.AppendLine(" c.Cognome + ' - ' + c.Nome, ")
            StrSQL.AppendLine(" a.lav_Cod, ")
            StrSQL.AppendLine(" a.id_attivita, ")
            StrSQL.AppendLine(" ISNULL(att.[Desc], o.LAV_DES), ")
            StrSQL.AppendLine(" md.Mov_Det_Des, ")
            StrSQL.AppendLine(" CASE a.Stato_Cod WHEN " & StatiWorkflowQdC.Eseguito & " THEN '" & AgronicaCoreDataProvider.My.Resources.Gias.Eseguita & "' ELSE '" & AgronicaCoreDataProvider.My.Resources.Gias.DaEseguire & "' END, ")
            StrSQL.AppendLine(" a.DaRemoto, ")
            'StrSQL.AppendLine(" Appezzamento.app_nome, ")
            StrSQL.AppendLine(" CoordinateGIS.Latitude, ")
            StrSQL.AppendLine(" CoordinateGIS.Longitude, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.IdAgenda_Rilievo, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Validita_Inizio_Rilievo, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Blocco_Flag_Rilievo, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Lav_Cod_Rilievo, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Lav_Des_Rilievo, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Sa_Cod_Rilievo, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Nome_Impianto ")


            'StrSQL.AppendLine(" ORDER BY m.Data_Movimento DESC, Ora_Movimento DESC, rag_soc ")
            StrSQL.AppendLine(" ORDER BY m.Data_Movimento DESC, Ora_Movimento DESC ")

            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiDettagliVisiteNew_DettaglioRilievo(
                ByVal veg_cod As Integer,
                ByVal id_cod As Integer,
                ByVal usernameFilter As String,
                ByVal pivaAzienda As String,
                ByVal tipoVisita As Integer,
                ByVal dataDa As Date,
                ByVal dataA As Date,
                ByVal pivaCentro As String,
                ByVal saCodCentro As String,
                ByVal impiantiSelezionati As String(),
                ByVal operazioniFilter As Integer(),
                ByVal Gen_Cod As Integer,
                ByVal Spe_Cod As Integer,
                ByVal IPro_Cod As Integer,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                ByVal usernames As String(),
                ByVal existsImpresa As Boolean,
                Optional ByVal topNRighe As Integer? = Nothing
                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreVisiteDAL.Visite_R.LeggiDettagliVisiteNew_DettaglioRilievo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim isSuperUser As Boolean = (objParametri_Utenti.SuperUserUsername = objParametri_Utenti.UtenteUsername)
        Dim isSpecieVegetale As Boolean = True

        Dim RilieviDal As New AgronicaCoreContabDAL.Rilievi_R

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            StrSQL.Length = 0

            'Creo la CTE che contiene i rilievi collegati alle Visite
            StrSQL.AppendLine(" with Rilievi_con_visite_cte as  ( ")

            'nuova query 
            StrSQL.AppendLine("     select distinct ")
            StrSQL.AppendLine("         Mov_Dettagli_Riferimenti.Id_Agenda as IdAgenda_Visita, Rilievi.* ")
            StrSQL.AppendLine("     FROM Mov_Dettagli_Riferimenti ")
            StrSQL.AppendLine("         INNER JOIN ( " & RilieviDal.getRilieviQueryString(objParametri_Utenti).ToString & " AND Agenda.Lav_Cod IN (" & STR_OP_COLLEGABILI_A_VISITE_NG & ")) Rilievi ON Rilievi.Piva_Rilievo = Mov_Dettagli_Riferimenti.PIVA And ")
            StrSQL.AppendLine("           Rilievi.IdAgenda_Rilievo = Mov_Dettagli_Riferimenti.Id_Agenda_Rif And Mov_Dettagli_Riferimenti.Lav_Cod = " & LAVCOD_VISITA & " And Mov_Dettagli_Riferimenti.Cau_Mov = '" & CAU_VISITE_ISPETTIVE & "'")
            StrSQL.AppendLine(")")

            If Not IsNothing(topNRighe) AndAlso topNRighe.HasValue Then
                StrSQL.AppendLine(" SELECT DISTINCT TOP " + topNRighe.Value.ToString + " ")
            Else
                StrSQL.AppendLine(" SELECT DISTINCT ")
            End If

            StrSQL.AppendLine(" m.Data_Movimento, RIGHT('0' + CAST(DATEPART(hh,m.Ora) AS varchar),2) + ':' + RIGHT('0' + CAST(DATEPART(mi,m.Ora) AS varchar),2) AS Ora_Movimento, ")
            StrSQL.AppendLine(" a.Piva, i.rag_soc, a.Sa_Cod, CASE a.Sa_Cod WHEN -1 THEN '' ELSE ca.Sa_Nome END AS 'Sa_Nome', ")
            StrSQL.AppendLine(" a.Id_Agenda, a.Lav_Cod, a.Des_Lib AS Lav_Des, m.Mov_Desc as Note, c.Cognome + ' - ' + c.Nome AS 'Operatore', ")
            StrSQL.AppendLine(" a.id_attivita As id_Attivita_Rif, ISNULL(att.[Desc], o.LAV_DES) AS 'Operazione' , md.Mov_Det_Des AS 'Descrizione', ")
            StrSQL.AppendLine(" CASE a.Stato_Cod WHEN " & StatiWorkflowQdC.Eseguito & " THEN '" & AgronicaCoreDataProvider.My.Resources.Gias.Eseguita & "' ELSE '" & AgronicaCoreDataProvider.My.Resources.Gias.DaEseguire & "' END AS 'Stato_Visita', a.DaRemoto, ")
            StrSQL.AppendLine(" STRING_AGG(Appezzamento.app_nome, ', ') AS Appezza_Visite, ")
            StrSQL.AppendLine(" MAX(ISNULL(SpecieVegetali.Veg_Cod, ISNULL(Rilievi_con_visite_cte.Veg_Cod, 0))) AS Veg_Cod, ")
            StrSQL.AppendLine(" MAX(ISNULL(SpecieVegetali.Veg_Des, ISNULL(Rilievi_con_visite_cte.Veg_Des, ''))) AS Veg_Des, ")
            StrSQL.AppendLine(" MAX(ISNULL(Codici_Anagrafe.codice, ISNULL(Rilievi_con_visite_cte.Cod_DestUso, 0))) AS Id_Cod, ")
            StrSQL.AppendLine(" MAX(ISNULL(Codici_Anagrafe.descrizione, ISNULL(Rilievi_con_visite_cte.Des_DestUso, ''))) AS Id_Des, ")
            StrSQL.AppendLine(" MAX(ISNULL(Lista_IndirizziProd_Animali.GEN_COD, 0)) AS Gen_Cod, ")
            StrSQL.AppendLine(" MAX(ISNULL(Lista_IndirizziProd_Animali.SPE_COD, 0)) AS Spe_Cod, ")
            StrSQL.AppendLine(" MAX(ISNULL(Lista_IndirizziProd_Animali.IPRO_COD, 0)) AS IPro_Cod, ")
            StrSQL.AppendLine(" MAX(ISNULL(Lista_IndirizziProd_Animali.IPRO_DES, '')) AS IPro_Des, ")
            StrSQL.AppendLine(" ISNULL(CoordinateGIS.Latitude, 0) AS Latitude, ")
            StrSQL.AppendLine(" ISNULL(CoordinateGIS.Longitude, 0) AS Longitude, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.IdAgenda_Rilievo, 0) AS Id_Agenda_Rif, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Validita_Inizio_Rilievo AS Validita_Inizio_Rif, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.Blocco_Flag_Rilievo, 0) AS Blocco_Flag_Rif, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.Lav_Cod_Rilievo, 0) AS Lav_Cod_Rif, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.Lav_Des_Rilievo, '') AS Lav_Des_Rif, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.Sa_Cod_Rilievo, 0) AS Sa_Cod_Rif, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.APPEZZA, 0) AS APPEZZA, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.APP_NOME, '') AS Appezza_Rilievo, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.Latitude, '') AS LatitudeRilievo, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.Longitude, '') AS LongitudeRilievo, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.Rilievo, '') AS Rilievo, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.Valore_Rilievo, '') AS Valore_Rilievo, ")
            StrSQL.AppendLine(" ISNULL(Rilievi_con_visite_cte.Causali_Rilievo, '') AS Causali_Rilievo ")

            StrSQL.AppendLine(" FROM Agenda a ")
            StrSQL.AppendLine(" INNER JOIN Movimenti m ON a.PIVA = m.PIVA AND a.Sa_Cod = m.Sa_Cod AND a.Id_Agenda = m.Id_Agenda AND m.Cau_Mov <> 6852 ")
            StrSQL.AppendLine(" LEFT JOIN Movimenti_dettagli md ON m.PIVA = md.PIVA And m.Id_Agenda = md.Id_Agenda AND m.Id_mov = md.Id_mov ")
            StrSQL.AppendLine(" INNER JOIN Operazioni o ON o.lav_cod = a.Lav_Cod ")
            StrSQL.AppendLine(" LEFT  JOIN Attivita att ON att.id_attivita = a.id_attivita ")
            StrSQL.AppendLine(" INNER JOIN Imprese i ON i.PIVA = a.PIVA ")
            StrSQL.AppendLine(" LEFT  JOIN Centri_Aziendali ca ON ca.PIVA = a.PIVA AND ca.sa_cod = a.sa_cod ")
            StrSQL.AppendLine(" INNER JOIN Movimenti m_assegnatario ON m_assegnatario.Piva=a.Piva AND m_assegnatario.Sa_Cod=a.Sa_Cod AND m_assegnatario.id_Agenda=a.id_Agenda AND m_assegnatario.Cau_Mov=6852  ")
            StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli md_assegnatario ON m_assegnatario.Piva=md_assegnatario.Piva AND m_assegnatario.Sa_Cod=md_assegnatario.Sa_Cod AND m_assegnatario.id_Agenda=md_assegnatario.id_Agenda AND m_assegnatario.id_mov=md_assegnatario.id_mov ")
            StrSQL.AppendLine(" INNER JOIN Risorse_Umane ru ON ru.Cod_RisUm = md_assegnatario.mat_cod ")
            StrSQL.AppendLine(" INNER JOIN Contatti c ON c.Cod_Contatto = ru.Cod_Contatto AND c.Piva = ru.Piva ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Mov_Destinazioni mdest ON a.Piva = mdest.Piva AND a.id_Agenda = mdest.Id_Agenda And mdest.Sa_Cod = a.Sa_Cod and md.id_mov = mdest.id_mov and md.Id_Mov_Det = mdest.Id_Mov_Det and mdest.Tipo_Destinazione = 0 ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Reg_Impianti ON Reg_Impianti.PIVA = mdest.Piva AND Reg_Impianti.SA_COD = mdest.Sa_Cod AND Reg_Impianti.APPEZZA = mdest.Appezza AND Reg_Impianti.ID_REG = mdest.Id_Destinazione ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Reg_Impianti_Codici ric ON ric.PIVA = mdest.Piva AND ric.SA_COD = mdest.Sa_Cod AND ric.APPEZZA = mdest.Appezza AND ric.ID_REG = mdest.Id_Destinazione AND ric.id_cod between 3000 and 3999 ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Appezzamento ON Reg_Impianti.PIVA = Appezzamento.Piva AND Reg_Impianti.SA_COD = Appezzamento.Sa_Cod AND Reg_Impianti.APPEZZA = Appezzamento.Appezza ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Cultivar on Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
            StrSQL.AppendLine(" LEFT OUTER JOIN SpecieVegetali on ((md.Dettaglio_VegCod <> 0 AND SpecieVegetali.Veg_Cod = md.Dettaglio_VegCod) OR SpecieVegetali.Veg_Cod = Cultivar.veg_cod) ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Codici_Anagrafe on ((md.Dettaglio_IdCod <> 0 AND Codici_Anagrafe.codice = md.Dettaglio_IdCod) OR Codici_Anagrafe.codice = ric.id_cod) ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Lista_IndirizziProd_Animali on (md.Dettaglio_GenCod <> 0 AND Lista_IndirizziProd_Animali.GEN_COD = md.Dettaglio_GenCod AND Lista_IndirizziProd_Animali.SPE_COD = md.Dettaglio_SpeCod AND Lista_IndirizziProd_Animali.IPRO_COD = md.Dettaglio_IProCod) ")
            StrSQL.AppendLine(" LEFT OUTER JOIN (	SELECT DISTINCT GIS_Entita.Piva, GIS_Entita.ID_Agenda, GIS_ElementiGrafici.Poligono_GeoEntity.Lat AS Latitude, GIS_ElementiGrafici.Poligono_GeoEntity.Long AS Longitude ")
            StrSQL.AppendLine("                      FROM GIS_Entita ")
            StrSQL.AppendLine("                     LEFT OUTER JOIN GIS_ElementiGrafici ON GIS_Entita.Entita_Cod = GIS_ElementiGrafici.Entita_Cod AND GIS_ElementiGrafici.LayerElementiGrafici_Cod = " & enum_Gis_LayerElementiGrafici_std.Op_Agenda & " ")
            StrSQL.AppendLine("                     WHERE ID_Agenda > 0 ")
            StrSQL.AppendLine("  ) CoordinateGIS On CoordinateGIS.Piva = a.PIVA And CoordinateGIS.ID_Agenda = a.ID_Agenda ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Rilievi_con_visite_cte on Rilievi_con_visite_cte.IdAgenda_Visita = a.Id_Agenda and Rilievi_con_visite_cte.Piva_Rilievo = a.Piva")

            If isSuperUser = False Or usernameFilter <> "" Then
                StrSQL.AppendLine(" INNER JOIN ContattiXUtentiGias cug ON cug.piva = c.piva AND cug.cod_contatto = c.Cod_contatto ")
                If usernameFilter <> "" Then
                    StrSQL.AppendLine(" AND (cug.username = " & Agro_SQL_SaveText_NULL(usernameFilter) & " ")
                Else
                    StrSQL.AppendLine(" AND (cug.username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & " ")
                    If usernames IsNot Nothing AndAlso usernames.Count > 0 Then
                        StrSQL.AppendLine(" OR cug.username IN( " & Agro_SQL_Save_Clausola_IN(String.Join(",", usernames), True) & " ) ")
                    End If
                End If
                StrSQL.AppendLine(" ) ")
            End If

            StrSQL.AppendLine(" WHERE a.Lav_Cod = 5007 ")

            If isSuperUser = False AndAlso existsImpresa = True Then
                StrSQL.AppendLine(" AND i.PIVA IN (SELECT DISTINCT piva from Utenti_Visibilita_Appoggio WHERE Entita_Cod = 1 AND Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & " ) ")
            End If

            If veg_cod <> -1 Then
                If veg_cod <> 0 Then
                    StrSQL.AppendLine(" AND (SpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(veg_cod.ToString()) & " OR Rilievi_con_visite_cte.Veg_Cod = " & Agro_SQL_SaveNum(veg_cod.ToString()) & ") ")
                Else
                    StrSQL.AppendLine(" AND SpecieVegetali.Veg_Cod IS NULL AND Rilievi_con_visite_cte.Veg_Cod IS NULL AND Codici_Anagrafe.codice IS NULL AND Rilievi_con_visite_cte.Cod_DestUso IS NULL ")
                End If
            Else
                If id_cod <> -1 Then
                    StrSQL.AppendLine(" AND (Codici_Anagrafe.codice = " & Agro_SQL_SaveNum(id_cod.ToString()) & " OR Rilievi_con_visite_cte.Cod_DestUso = " & Agro_SQL_SaveNum(id_cod.ToString()) & ") ")
                End If
            End If

            If pivaAzienda <> "" Then
                StrSQL.AppendLine(" AND a.piva = " & Agro_SQL_SaveText_NULL(pivaAzienda))
            End If

            If tipoVisita <> 0 Then
                StrSQL.AppendLine(" AND a.Stato_Cod = " & Agro_SQL_SaveNum(tipoVisita.ToString()))
            End If

            If (pivaCentro <> "" And saCodCentro <> "") AndAlso (pivaCentro <> "-1" And saCodCentro <> "0") Then
                StrSQL.AppendLine(" AND ca.piva = " & Agro_SQL_SaveText_NULL(pivaCentro) & " AND ca.sa_cod = " & Agro_SQL_SaveText_NULL(saCodCentro))
            End If

            If operazioniFilter IsNot Nothing AndAlso operazioniFilter.Count > 0 Then
                StrSQL.AppendLine(" AND a.id_attivita IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", operazioniFilter)) & ") ")
            End If

            If Gen_Cod <> -1 AndAlso Spe_Cod <> -1 AndAlso IPro_Cod <> -1 Then
                StrSQL.AppendLine(" AND Lista_IndirizziProd_Animali.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod.ToString()) & " ")
                StrSQL.AppendLine(" AND Lista_IndirizziProd_Animali.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod.ToString()) & " ")
                StrSQL.AppendLine(" AND Lista_IndirizziProd_Animali.IPro_Cod = " & Agro_SQL_SaveNum(IPro_Cod.ToString()) & " ")
            End If

            If impiantiSelezionati IsNot Nothing AndAlso impiantiSelezionati.Count > 0 Then
                StrSQL.AppendLine(" AND (")
                For ctr As Integer = 0 To (impiantiSelezionati.Count - 1)
                    Dim chiavi = impiantiSelezionati(ctr).Split(CChar("_"))
                    StrSQL.AppendLine("( ")
                    StrSQL.AppendLine(" Reg_Impianti.PIVA = " & chiavi(0) & " AND Reg_Impianti.SA_COD = " & chiavi(1) & " AND Reg_Impianti.APPEZZA = " & chiavi(2))
                    StrSQL.AppendLine(" )")
                    If ctr < (impiantiSelezionati.Count - 1) Then
                        StrSQL.AppendLine(" OR ")
                    End If
                Next
                StrSQL.AppendLine(" )")
            End If

            StrSQL.AppendLine(" AND m.Data_Movimento between " + Agro_SQL_SaveDate(dataDa) + " AND " + Agro_SQL_SaveDate(dataA) + " ")

            StrSQL.AppendLine(" GROUP BY m.Data_Movimento, ")
            StrSQL.AppendLine(" RIGHT('0' + CAST(DATEPART(hh, m.Ora) AS varchar), 2) + ':' + RIGHT('0' + CAST(DATEPART(mi, m.Ora) AS varchar), 2), ")
            StrSQL.AppendLine(" a.Piva, ")
            StrSQL.AppendLine(" i.rag_soc, ")
            StrSQL.AppendLine(" a.Sa_Cod, ")
            StrSQL.AppendLine(" CASE a.Sa_Cod WHEN -1 THEN '' ELSE ca.Sa_Nome END, ")
            StrSQL.AppendLine(" a.Id_Agenda, ")
            StrSQL.AppendLine(" a.Lav_Cod, ")
            StrSQL.AppendLine(" a.Des_Lib, ")
            StrSQL.AppendLine(" m.Mov_Desc, ")
            StrSQL.AppendLine(" c.Cognome + ' - ' + c.Nome, ")
            StrSQL.AppendLine(" a.lav_Cod, ")
            StrSQL.AppendLine(" a.id_attivita, ")
            StrSQL.AppendLine(" ISNULL(att.[Desc], o.LAV_DES), ")
            StrSQL.AppendLine(" md.Mov_Det_Des, ")
            StrSQL.AppendLine(" CASE a.Stato_Cod WHEN " & StatiWorkflowQdC.Eseguito & " THEN '" & AgronicaCoreDataProvider.My.Resources.Gias.Eseguita & "' ELSE '" & AgronicaCoreDataProvider.My.Resources.Gias.DaEseguire & "' END, ")
            StrSQL.AppendLine(" a.DaRemoto, ")
            'StrSQL.AppendLine(" Appezzamento.app_nome, ")
            StrSQL.AppendLine(" CoordinateGIS.Latitude, ")
            StrSQL.AppendLine(" CoordinateGIS.Longitude, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.IdAgenda_Rilievo, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Validita_Inizio_Rilievo, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Blocco_Flag_Rilievo, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Lav_Cod_Rilievo, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Lav_Des_Rilievo, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Sa_Cod_Rilievo, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.APPEZZA, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.APP_NOME, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Latitude, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Longitude, ")
            'StrSQL.AppendLine(" Rilievi_con_visite_cte.DescrizioneAvv, ")
            'StrSQL.AppendLine(" Rilievi_con_visite_cte.DescrizioneAvvGru, ")
            'StrSQL.AppendLine(" Rilievi_con_visite_cte.FF_Classe, ")
            'StrSQL.AppendLine(" Rilievi_con_visite_cte.Indice, ")
            'StrSQL.AppendLine(" Rilievi_con_visite_cte.DescrizioneDanni, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Rilievo, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Valore_Rilievo, ")
            'StrSQL.AppendLine(" Rilievi_con_visite_cte.TipoControllo, ")
            'StrSQL.AppendLine(" Rilievi_con_visite_cte.DataFasiFenologiche, ")
            StrSQL.AppendLine(" Rilievi_con_visite_cte.Causali_Rilievo ")


            'StrSQL.AppendLine(" ORDER BY m.Data_Movimento DESC, Ora_Movimento DESC, rag_soc ")
            StrSQL.AppendLine(" ORDER BY m.Data_Movimento DESC, Ora_Movimento DESC ")

            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_Agenzie_Visibilita_Utente_Visite(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                           ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                           ByVal UsernameTecnico As String,
                                                           ByVal Filtro_Visibilita_Utente As Boolean,
                                                           ByVal typeUserLogged As enum_TipoOperatoreVisita,
                                                           ByVal existsImpresaForUserLogged As Boolean,
                                                           ByVal existsImpresaForTecnico As Boolean) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Read.Leggi_Agenzie_Visibilita_Utente_Visite()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim isSuperUser As Boolean = (objParametri_Utenti.SuperUserUsername = objParametri_Utenti.UtenteUsername) AndAlso (objParametri_Utenti.UtenteUsername = UsernameTecnico)

        Try

            stb.Length = 0
            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            stb.AppendLine(" SELECT DISTINCT I.PIVA, I.rag_soc ")
            stb.AppendLine(" FROM  Imprese I ")

            If isSuperUser = False AndAlso existsImpresaForTecnico Then
                stb.AppendLine(" inner join Utenti_Visibilita_Appoggio U (NOLOCK) ")
                stb.AppendLine(" on I.Piva = U.Piva ")
            End If

            If existsImpresaForUserLogged AndAlso (typeUserLogged = enum_TipoOperatoreVisita.CapoTecnico OrElse typeUserLogged = enum_TipoOperatoreVisita.Capo) Then
                stb.AppendLine(" inner join Utenti_Visibilita_Appoggio U_Capo (NOLOCK) ")
                stb.AppendLine(" on I.Piva = U_Capo.Piva and U_Capo.username ='" & Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) & "' ")
            End If

            stb.AppendLine(" WHERE 1 = 1 ")

            If isSuperUser = False AndAlso existsImpresaForTecnico Then
                stb.AppendLine(" AND U.PivaSuperUser='" & Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) & "'")
                stb.AppendLine(" AND (U.Entita_Cod = 1) ")
            End If

            stb.AppendLine(" AND I.Piva IN (select IC.Piva from Imprese_Codici IC where IC.id_cod = " & enum_CodiciAnagrafe.CodiceAgenzia & " )")

            If isSuperUser = False AndAlso existsImpresaForTecnico AndAlso Filtro_Visibilita_Utente Then
                stb.AppendLine(" AND U.Username = " & Agro_SQL_SaveText_NULL(UsernameTecnico) & "  ")
            End If

            stb.AppendLine(" AND   I.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & " ")
            stb.AppendLine(" AND   I.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & " ")

            stb.AppendLine(" ORDER BY I.rag_soc ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function LeggiVisiteAPP(
                        ByVal Piva As String,
                        ByVal Sa_Cod As Integer,
                        ByVal Id_Agenda As Integer,
                        ByVal Stato_Cod As Integer,
                        ByVal Username As String,
                        ByVal xFiltroAggiuntivo As String,
                        ByVal xOrderBy As String,
                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                        Optional ByVal Rilievi As Boolean = False
                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreVisiteDAL.Visite_R.LeggiVisite()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            StrSQL.AppendLine("SELECT DISTINCT a.Piva, a.Sa_Cod, a.Id_Agenda, a.Id_Attivita, a.Stato_Cod, m.Data_Movimento, cug.username ")
            StrSQL.AppendLine("FROM Agenda a ")
            StrSQL.AppendLine(" INNER JOIN Movimenti m ON a.PIVA = m.PIVA AND a.Sa_Cod = m.Sa_Cod AND a.Id_Agenda = m.Id_Agenda AND m.Cau_Mov <> 6852 ")
            StrSQL.AppendLine(" INNER JOIN Movimenti m_assegnatario ON m_assegnatario.Piva=a.Piva AND m_assegnatario.Sa_Cod=a.Sa_Cod AND m_assegnatario.id_Agenda=a.id_Agenda AND m_assegnatario.Cau_Mov = 6852 ")
            StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli md_assegnatario ON m_assegnatario.Piva=md_assegnatario.Piva AND m_assegnatario.Sa_Cod=md_assegnatario.Sa_Cod AND m_assegnatario.id_Agenda=md_assegnatario.id_Agenda AND m_assegnatario.id_mov=md_assegnatario.id_mov ")
            StrSQL.AppendLine(" INNER JOIN Risorse_Umane ru ON ru.Cod_RisUm = md_assegnatario.mat_cod ")
            StrSQL.AppendLine(" INNER JOIN Contatti c ON c.Cod_Contatto = ru.Cod_Contatto AND c.Piva = ru.Piva ")
            StrSQL.AppendLine(" INNER JOIN ContattiXUtentiGias cug ON cug.piva = c.piva AND cug.cod_contatto = c.Cod_contatto AND cug.username = '" & Agro_SQL_SaveText(Username) & "' ")

            StrSQL.AppendLine(" WHERE a.Lav_Cod = " & LAVCOD_VISITA)
            StrSQL.AppendLine(" AND a.id_attivita IN (SELECT id_attivita FROM AttivitaXOperazioni ")
            StrSQL.AppendLine(" WHERE Lav_Cod IN (" & LAVCOD_VISITA & If(Rilievi, ",79,109,113", "") & ")) ")

            If Piva <> "" Then
                StrSQL.AppendLine(" And a.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND a.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString()))
            End If

            If Id_Agenda <> 0 Then
                StrSQL.AppendLine(" AND a.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda.ToString()))
            End If

            If Stato_Cod <> 0 Then
                StrSQL.AppendLine(" AND a.Stato_Cod = " & Agro_SQL_SaveNum(Stato_Cod.ToString()))
            End If

            If objParametri_Server.FinestraTemporaleInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND m.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            End If

            If objParametri_Server.FinestraTemporaleFine <> AGRODATAFINE Then
                StrSQL.AppendLine(" AND m.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                StrSQL.AppendLine(" ORDER BY a.PIVA, m.Data_Movimento DESC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class
