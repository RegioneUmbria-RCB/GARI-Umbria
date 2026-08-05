Imports System.Data.Entity
Imports System.Reflection
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class Vendita_Carburanti_DAL
    Inherits AgronicaCoreDataProvider.DataProvider
    Private _EFContext As Gias_DeveloperServer_Entities
    Private _coreDBContext As AgronicaCoreParametri
    Private _coreDBContext_user As AgronicaCoreParametri
    Public Sub New(efContext As Gias_DeveloperServer_Entities, dbContext As AgronicaCoreParametri)
        _EFContext = efContext
        _coreDBContext = dbContext
    End Sub

    Public Sub New(efContext As Gias_DeveloperServer_Entities, dbContext As AgronicaCoreParametri, dbContext_user As AgronicaCoreParametri)
        _EFContext = efContext
        _coreDBContext = dbContext
        _coreDBContext_user = dbContext_user
    End Sub

    Public Sub AggiungiNouvi(carburanti As List(Of VenditaCarburantiDto))
        For Each carb In carburanti
            carb.Lt = Math.Round(carb.Lt, MidpointRounding.AwayFromZero)
            Dim VenditeId = OttieneEntityFrameworkNuovoID()
            _EFContext.UMA_Vendite.Add(carb.ToVenditaCarburantePOCO(_coreDBContext, VenditeId))
        Next
        _EFContext.SaveChanges()
    End Sub

    Private Function OttieneEntityFrameworkNuovoID() As Integer
        Dim handleSequenze = New AgronicaCoreDataProvider.Agro_Sequenze()
        Return handleSequenze.NuovoId_Tabella_EF(_EFContext, "UMA_Vendite", 0, 2000000000, _coreDBContext)
    End Function

    Public Function OttienePIVADallePratiche(anno As Integer, cuaa As String) As String
        Dim imprese = _EFContext.Pratiche.Where(Function(s) s.Servizio_Cod = enum_Servizi.Gestione_UMA AndAlso s.Anno = anno AndAlso s.Cuaa = cuaa)
        If Not imprese.Any() Then
            Dim impreseCodici = _EFContext.Imprese_Codici.Where(Function(s) s.id_cod = enum_CodiciAnagrafe.CodiceCUAA AndAlso s.val_cod = cuaa)
            Return impreseCodici.FirstOrDefault().PIVA
        End If
        Return imprese.FirstOrDefault().Piva
    End Function
    Public Sub Aggiorna(carburanti As List(Of VenditaCarburantiDto))

        Dim now As Date = Date.Now
        For Each elem In carburanti
            Dim dbEntity As UMA_Vendite = _EFContext.UMA_Vendite.FirstOrDefault(Function(s) s.Id_Vendite = elem.Id_Vendite)

            If dbEntity IsNot Nothing Then
                dbEntity.PivaSuperUser = elem.PivaSuperUser
                dbEntity.PIVA_Venditore = elem.Azienda_Venditore_Cod
                dbEntity.PIVA_Cliente = elem.PIVA_Cliente
                dbEntity.Anno = elem.Anno_Cod
                dbEntity.Conto_Proprio_Terzi = elem.Conto_Proprio_Terzi_Cod
                dbEntity.Tipo_Carburante = elem.Tipo_Carburante_Cod
                dbEntity.Lt = Math.Round(elem.Lt, MidpointRounding.AwayFromZero)
                dbEntity.Tipo_Documento = elem.Tipo_Documento_Cod
                dbEntity.Data_Documento = elem.Data_Documento
                dbEntity.Nr_Documento = elem.Nr_Documento
                dbEntity.Note_Rivenditore = elem.Note_Rivenditore
                dbEntity.Data_Modifica = now
                dbEntity.Username_Modifica = _coreDBContext.UsernameOperazione
            End If
        Next
        _EFContext.SaveChanges()
    End Sub
    Public Sub Rimuovi(carburante As VenditaCarburantiDto)
        Dim record As UMA_Vendite = _EFContext.UMA_Vendite.Where(Function(s) s.Id_Vendite = carburante.Id_Vendite).FirstOrDefault()

        If Not record Is Nothing Then
            _EFContext.UMA_Vendite.Remove(record)
            _EFContext.SaveChanges()
        End If
    End Sub
    Public Function CaricaCarburantiVendita(filtri As FiltriCaricaRigheVenditaCarburanti,
                                            FiltroUtente As Boolean,
                                            FiltroGruppoUtente As Boolean,
                                            GruppoUtente As Integer,
                                            VisibilitaTotale As Boolean) As DataTable
        Dim StrSQL As New System.Text.StringBuilder()
        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim Filtro_Visibilita_Utente = Not objProfilo.HasFullVisibility(_coreDBContext_user.UtenteUsername, _coreDBContext_user)

        StrSQL.Append("SELECT Id_Vendite, Uma_Vendite.PivaSuperUser, PIVA_Venditore, PIVA_Cliente, Anno As Anno_Cod, Anno As Anno_Des, Conto_Proprio_Terzi As Conto_Proprio_Terzi_Cod,
                         CASE Conto_Proprio_Terzi
                           WHEN 0 THEN 'Conto Proprio'
                           WHEN -1 THEN 'Conto Terzi'
                         END as Conto_Proprio_Terzi_Des,
                         Tipo_Carburante as Tipo_Carburante_Cod,
                         CASE Tipo_Carburante
                           WHEN 2 THEN 'Gasolio'
                           WHEN 3 THEN 'Benzina'
                           WHEN 8 THEN 'Gasolio Serra'
                         END as Tipo_Carburante_Des,
                         Lt, Tipo_Documento as Tipo_Documento_Cod,
                         CASE Tipo_Documento
                           WHEN 1 THEN 'E-DAS'
                           WHEN 2 THEN 'Doc. cartaceo (causa di forza maggiore)'
                         END as Tipo_Documento_Des,
                         Data_Documento, Nr_Documento, Note_Rivenditore, venditore.piva as Azienda_Venditore_Cod, venditore.rag_soc as Azienda_Venditore_Des,
                         cuaa.val_cod as Cuaa, cliente.rag_soc as Ragione_Sociale,
						 Uma_Vendite.Data_Creazione as Data_Creazione,
						 Uma_Vendite.Data_Modifica as Data_Modifica,
                            Richiesta_Cod
                       FROM Uma_Vendite
                       JOIN Imprese_Codici cuaa ON Uma_Vendite.PIVA_Cliente = cuaa.PIVA And cuaa.id_cod = 1010
                       JOIN Imprese cliente ON Uma_Vendite.PIVA_Cliente = Cliente.PIVA
                       JOIN Imprese venditore ON Uma_Vendite.PIVA_Venditore = venditore.PIVA ")

        If Filtro_Visibilita_Utente AndAlso Not filtri.NuovaVisibilita Then
            StrSQL.AppendLine(" LEFT JOIN Utenti_Visibilita_Appoggio uv_venditore (NOLOCK) ON Uma_Vendite.PIVA_Venditore = uv_venditore.Piva AND uv_venditore.Sa_Cod = 0 AND uv_venditore.Username = '" & _coreDBContext_user.UtenteUsername & "' ")
            StrSQL.AppendLine(" LEFT JOIN Utenti_Visibilita_Appoggio uv_cliente (NOLOCK) ON Uma_Vendite.PIVA_Cliente = uv_cliente.Piva AND uv_cliente.Sa_Cod = 0 AND uv_cliente.Username = '" & _coreDBContext_user.UtenteUsername & "' ")
        End If

        If filtri.NuovaVisibilita Then
            If Not VisibilitaTotale Then
                If FiltroUtente And Not FiltroGruppoUtente Then
                    StrSQL.AppendLine(" LEFT JOIN " & _coreDBContext_user.Recupera_NomeDB & ".dbo.Utenti_Visibilita uv_venditore ON Uma_Vendite.PIVA_Venditore = uv_venditore.Piva_Azienda  AND uv_venditore.Username = '" & _coreDBContext_user.UtenteUsername & "' ")
                    StrSQL.AppendLine(" LEFT JOIN " & _coreDBContext_user.Recupera_NomeDB & ".dbo.Utenti_Visibilita uv_cliente ON Uma_Vendite.PIVA_Cliente = uv_cliente.Piva_Azienda AND uv_cliente.Username = '" & _coreDBContext_user.UtenteUsername & "' ")
                End If
                If Not FiltroUtente And FiltroGruppoUtente Then
                    StrSQL.AppendLine(" LEFT JOIN " & _coreDBContext_user.Recupera_NomeDB & ".dbo.Utenti_Visibilita uv_venditore ON Uma_Vendite.PIVA_Venditore = uv_venditore.Piva_Azienda  AND uv_venditore.Gruppo = " & GruppoUtente & " ")
                    StrSQL.AppendLine(" LEFT JOIN " & _coreDBContext_user.Recupera_NomeDB & ".dbo.Utenti_Visibilita uv_cliente ON Uma_Vendite.PIVA_Cliente = uv_cliente.Piva_Azienda AND uv_cliente.Gruppo = " & GruppoUtente & " ")
                End If
            End If
        End If

        StrSQL.Append("WHERE 1 = 1")

        If filtri.Anno <> 0 Then
            StrSQL.Append(" AND UMA_Vendite.Anno = " & filtri.Anno)
        End If

        If filtri.Piva <> "-1" AndAlso filtri.Piva <> "" Then
            StrSQL.Append(" AND (UMA_Vendite.PIVA_Venditore = '" & Agro_SQL_SaveText(filtri.Piva) & "' OR UMA_Vendite.PIVA_Cliente = '" & Agro_SQL_SaveText(filtri.Piva) & "' ) ")
        End If

        If Filtro_Visibilita_Utente AndAlso Not filtri.NuovaVisibilita Then
            StrSQL.Append(" AND NOT (uv_venditore.Piva IS NULL AND uv_cliente.Piva is NULL) ")
        End If

        If filtri.NuovaVisibilita And Not VisibilitaTotale And (FiltroUtente = True Or FiltroGruppoUtente = True) Then
            StrSQL.Append(" AND NOT (uv_venditore.Piva_Azienda IS NULL AND uv_cliente.Piva_Azienda is NULL) ")
        End If

        'If filtri.NuovaVisibilita AndAlso _coreDBContext.SuperUserUsername <> _coreDBContext.UtenteUsername AndAlso Not PiveVisibili.Contains(filtri.Piva) Then
        '    StrSQL.Append(" AND (UMA_Vendite.Piva_Cliente IN ( '" + PiveVisibili + "' ) OR UMA_Vendite.Piva_Venditore IN ( '" + PiveVisibili + "' ) ) ")
        'End If

        'Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        'Dim UtenteProfiloImpreseSql As String = ""

        'Dim DtImpreseVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", _coreDBContext)
        'If Not IsNothing(DtImpreseVisibili) AndAlso DtImpreseVisibili.Rows.Count > 0 Then
        '    For Each dr In DtImpreseVisibili.Rows
        '        UtenteProfiloImpreseSql &= "'" & dr.Item("Piva") & "',"
        '    Next
        '    StrSQL.AppendLine(" AND Venditore.Piva IN (" & UtenteProfiloImpreseSql.TrimEnd(",") & ") ")
        'End If

        StrSQL.AppendLine(" ORDER BY Id_Vendite DESC")

        Dim dt = EseguiQuery_Lettura(_coreDBContext, StrSQL.ToString(), MethodBase.GetCurrentMethod().Name)

        Return dt
    End Function

    Public Function VerificaValiditaCuaa(cuaa As String) As Boolean
        Dim imprese = _EFContext.Imprese_Codici.Where(Function(s) s.id_cod = enum_CodiciAnagrafe.CodiceCUAA AndAlso s.val_cod = cuaa)
        Return imprese.Any()
    End Function
    Public Function VerificarePraticaDiRichiestaCarburanteSiaAperta(filtri As FiltriPraticaRichiestaCarburante) As Boolean
        Dim imprese = _EFContext.Pratiche.Where(Function(s) s.Servizio_Cod = enum_Servizi.Gestione_UMA AndAlso s.Anno = filtri.Anno AndAlso s.Cuaa = filtri.Cuaa)
        If Not imprese.Any() Then
            Dim piva = _EFContext.Imprese_Codici.Where(Function(s) s.id_cod = enum_CodiciAnagrafe.CodiceCUAA AndAlso s.val_cod = filtri.Cuaa)
            If piva.Any() Then
                imprese = _EFContext.Pratiche.Where(Function(s) s.Servizio_Cod = enum_Servizi.Gestione_UMA AndAlso s.Anno = filtri.Anno AndAlso s.Piva = piva.FirstOrDefault.PIVA)
            End If
        End If
        Return imprese.Any()
    End Function

    Public Sub AggiornaCarburanteUMAPivaCliente(id_Vendite As Integer, pivaPraticaAperta As String)
        Dim UmaVendita = _EFContext.UMA_Vendite.FirstOrDefault(Function(s) s.Id_Vendite = id_Vendite)
        If UmaVendita IsNot Nothing Then
            UmaVendita.PIVA_Cliente = pivaPraticaAperta
        End If
        _EFContext.SaveChanges()
    End Sub


    Public Function CaricareRagioneSociale(cuaa As String) As String
        Dim impresePIVA = _EFContext.Imprese_Codici.Where(Function(s) s.id_cod = enum_CodiciAnagrafe.CodiceCUAA AndAlso s.val_cod = cuaa).Select(Function(s) s.PIVA)
        Dim rag_soc = _EFContext.Imprese.Where(Function(s) impresePIVA.Contains(s.PIVA)).Select(Function(s) s.rag_soc).FirstOrDefault()
        If rag_soc Is Nothing Then
            Throw New Exception("La Ragione Sociale non è stata trovata. E' possibile che il CUAA fornito non sia valido.")
        End If
        Return rag_soc
    End Function
    Public Function OttieneLtAssegnati(filtri As FiltriAggiornaLtAssegnati) As Integer
        Dim StrSQL As New System.Text.StringBuilder()
        Dim StrProprioTerzi As New System.Text.StringBuilder()

        If filtri.ContoProprioTerzi = 0 Then
            'Conto Proprio
            StrProprioTerzi.AppendLine(" Totali_Assegnati(Piva, Tipo_Carburante, Assegnato) AS (")
            StrProprioTerzi.AppendLine(" SELECT Z.Piva, Z.Tipo_Carburante, SUM(Z.Assegnato) as Assegnato")
            StrProprioTerzi.AppendLine(" FROM (")
            StrProprioTerzi.AppendLine("  SELECT UMA_Richieste_Testata.piva, rlav.Tipo_Carburante, SUM(rlav.Fabbisogno_Assegnato) as Assegnato")
            StrProprioTerzi.AppendLine("  FROM UMA_Richieste_Testata")
            StrProprioTerzi.AppendLine("  JOIN UMA_Richieste_Lavorazioni as rlav ON UMA_Richieste_Testata.Richiesta_Cod = rlav.Richiesta_Cod ")
            StrProprioTerzi.AppendLine("  JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod")
            StrProprioTerzi.AppendLine("  JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod")
            StrProprioTerzi.AppendLine("  WHERE Avanzamento_Richiesta = 0")
            StrProprioTerzi.AppendLine("  AND Pratiche.Anno = {0}")
            StrProprioTerzi.AppendLine("  AND UMA_Richieste_Testata.Piva = '{1}'")
            StrProprioTerzi.AppendLine("  AND UMA_Richieste_Testata.Tipo_Richiesta = {2}")
            StrProprioTerzi.AppendLine("  AND Pratiche_Stati_Attuali.Stato_Cod IN (2003, 2005, 2008)")
            StrProprioTerzi.AppendLine("  GROUP BY UMA_Richieste_Testata.piva, rlav.Tipo_Carburante ")
            StrProprioTerzi.AppendLine("  UNION ALL")
            StrProprioTerzi.AppendLine("  SELECT UMA_Richieste_Testata.piva, rlav_all.Tipo_Carburante, SUM(rlav_all.Carburante_Approvato) as Assegnato")
            StrProprioTerzi.AppendLine("  FROM UMA_Richieste_Testata")
            StrProprioTerzi.AppendLine("  JOIN UMA_Richieste_Allevamenti as rlav_all ON UMA_Richieste_Testata.Richiesta_Cod = rlav_all.Richiesta_Cod")
            StrProprioTerzi.AppendLine("  JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod")
            StrProprioTerzi.AppendLine("  JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod")
            StrProprioTerzi.AppendLine("  WHERE Avanzamento_Richiesta = 0")
            StrProprioTerzi.AppendLine("  AND Pratiche.Anno = {0}")
            StrProprioTerzi.AppendLine("  AND UMA_Richieste_Testata.Piva = '{1}'")
            StrProprioTerzi.AppendLine("  AND UMA_Richieste_Testata.Tipo_Richiesta = {2}")
            StrProprioTerzi.AppendLine("  AND Pratiche_Stati_Attuali.Stato_Cod IN (2003, 2005, 2008)")
            StrProprioTerzi.AppendLine("  GROUP BY UMA_Richieste_Testata.piva, rlav_all.Tipo_Carburante ")
            StrProprioTerzi.AppendLine(" ) Z")
            StrProprioTerzi.AppendLine(" GROUP BY Z.Piva, Z.Tipo_Carburante")
            StrProprioTerzi.AppendLine(" UNION ALL ")
            StrProprioTerzi.AppendLine(" SELECT UMA_Richieste_Testata.piva, rlav.Tipo_Carburante, SUM(rlav.Fabbisogno_Richiesto) as Assegnato ")
            StrProprioTerzi.AppendLine(" FROM UMA_Richieste_Testata ")
            StrProprioTerzi.AppendLine(" JOIN UMA_Richieste_Lavorazioni as rlav ON UMA_Richieste_Testata.Richiesta_Cod = rlav.Richiesta_Cod  ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" WHERE Avanzamento_Richiesta in (-1) ")
            StrProprioTerzi.AppendLine(" AND Pratiche.Anno = {0} ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Piva = '{1}' ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Tipo_Richiesta = {2} ")
            StrProprioTerzi.AppendLine(" AND Pratiche_Stati_Attuali.Stato_Cod IN (2003, 2005, 2008) ")
            StrProprioTerzi.AppendLine(" AND NOT EXISTS(SELECT UMA_Richieste_Testata.piva, rlav.Tipo_Carburante, SUM(rlav.Fabbisogno_Richiesto) as Assegnato ")
            StrProprioTerzi.AppendLine(" FROM UMA_Richieste_Testata ")
            StrProprioTerzi.AppendLine(" JOIN UMA_Richieste_Lavorazioni as rlav ON UMA_Richieste_Testata.Richiesta_Cod = rlav.Richiesta_Cod  ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" WHERE Avanzamento_Richiesta = 0 ")
            StrProprioTerzi.AppendLine(" AND Pratiche.Anno = {0} ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Piva = '{1}' ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Tipo_Richiesta = {2} ")
            StrProprioTerzi.AppendLine(" AND Pratiche_Stati_Attuali.Stato_Cod IN (2003, 2005, 2008) ")
            StrProprioTerzi.AppendLine(" GROUP BY UMA_Richieste_Testata.piva, rlav.Tipo_Carburante) ")
            StrProprioTerzi.AppendLine(" AND NOT EXISTS(")
            StrProprioTerzi.AppendLine("  SELECT UMA_Richieste_Testata.piva, rlav_all.Tipo_Carburante, SUM(rlav_all.Carburante_Approvato) as Assegnato")
            StrProprioTerzi.AppendLine("  FROM UMA_Richieste_Testata")
            StrProprioTerzi.AppendLine("  JOIN UMA_Richieste_Allevamenti as rlav_all ON UMA_Richieste_Testata.Richiesta_Cod = rlav_all.Richiesta_Cod")
            StrProprioTerzi.AppendLine("  JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod")
            StrProprioTerzi.AppendLine("  JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod")
            StrProprioTerzi.AppendLine("  WHERE Avanzamento_Richiesta = 0")
            StrProprioTerzi.AppendLine("  AND Pratiche.Anno = {0}")
            StrProprioTerzi.AppendLine("  AND UMA_Richieste_Testata.Piva = '{1}'")
            StrProprioTerzi.AppendLine("  AND UMA_Richieste_Testata.Tipo_Richiesta = {2}")
            StrProprioTerzi.AppendLine("  AND Pratiche_Stati_Attuali.Stato_Cod IN (2003, 2005, 2008)")
            StrProprioTerzi.AppendLine("  GROUP BY UMA_Richieste_Testata.piva, rlav_all.Tipo_Carburante) ")
            StrProprioTerzi.AppendLine(" GROUP BY UMA_Richieste_Testata.piva, rlav.Tipo_Carburante ")
            StrProprioTerzi.AppendLine(" )")
        Else
            'Conto Terzi
            StrProprioTerzi.AppendLine(" Totali_Assegnati(Piva, Richiesta_Cod, Tipo_Carburante, Assegnato) AS (")
            '1) Gasolio con carburante calcolato
            StrProprioTerzi.AppendLine(" -- 1) Gasolio con carburante calcolato ")
            StrProprioTerzi.AppendLine(" SELECT UMA_Richieste_Testata.piva, UMA_Richieste_Testata.richiesta_cod, 2 As Tipo_Carburante, ")
            StrProprioTerzi.AppendLine(" CASE WHEN SUM(ISNULL(UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, 0)) + SUM(ISNULL(UMA_Lavorazioni_Parziali.Fabbisogno_Assegnato, 0)) > ISNULL(MAX(UMA_Richieste_Testata.Approvazione_Iniziale_Gasolio),0) ")
            StrProprioTerzi.AppendLine("       AND SUM(ISNULL(UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, 0)) + SUM(ISNULL(UMA_Lavorazioni_Parziali.Fabbisogno_Assegnato, 0)) > 0 ")
            StrProprioTerzi.AppendLine(" THEN SUM(ISNULL(UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, 0)) + SUM(ISNULL(UMA_Lavorazioni_Parziali.Fabbisogno_Assegnato, 0)) ")
            StrProprioTerzi.AppendLine(" ELSE ISNULL(MAX(UMA_Richieste_Testata.Approvazione_Iniziale_Gasolio),0) END as Assegnato ")
            StrProprioTerzi.AppendLine(" FROM UMA_Richieste_Testata ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" LEFT JOIN UMA_Richieste_Lavorazioni ON UMA_Richieste_Lavorazioni.Richiesta_Cod = UMA_Richieste_Testata.Richiesta_Cod AND UMA_Richieste_Lavorazioni.Tipo_Carburante = 2 ")
            StrProprioTerzi.AppendLine(" LEFT JOIN UMA_Lavorazioni_Parziali ON UMA_Lavorazioni_Parziali.Richiesta_Cod = UMA_Richieste_Testata.Richiesta_Cod AND UMA_Lavorazioni_Parziali.Tipo_Carburante = 2 ")
            StrProprioTerzi.AppendLine(" WHERE UMA_Richieste_Testata.Avanzamento_Richiesta = 0 ")
            StrProprioTerzi.AppendLine(" AND Pratiche.Anno = {0} ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Piva = '{1}' ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Tipo_Richiesta = {2} ")
            StrProprioTerzi.AppendLine(" AND Pratiche_Stati_Attuali.Stato_Cod IN (2003, 2005, 2008) AND UMA_Richieste_Testata.Carburante_Calcolato > 0 ")
            StrProprioTerzi.AppendLine(" GROUP BY UMA_Richieste_Testata.piva, UMA_Richieste_Testata.richiesta_cod ")
            '2) Gasolio senza carburante calcolato
            StrProprioTerzi.AppendLine(" -- 2) Gasolio senza carburante calcolato ")
            StrProprioTerzi.AppendLine(" UNION ALL ")
            StrProprioTerzi.AppendLine(" SELECT UMA_Richieste_Testata.piva, UMA_Richieste_Testata.richiesta_cod, 2 As Tipo_Carburante, ")
            StrProprioTerzi.AppendLine(" CASE WHEN SUM(ISNULL(UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, 0)) > ISNULL(MAX(UMA_Richieste_Testata.Approvazione_Iniziale_Gasolio),0) ")
            StrProprioTerzi.AppendLine(" THEN SUM(ISNULL(UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, 0)) ")
            StrProprioTerzi.AppendLine(" ELSE ISNULL(MAX(UMA_Richieste_Testata.Approvazione_Iniziale_Gasolio),0) END as Assegnato ")
            StrProprioTerzi.AppendLine(" FROM UMA_Richieste_Testata ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" LEFT JOIN UMA_Richieste_Lavorazioni ON UMA_Richieste_Lavorazioni.Richiesta_Cod = UMA_Richieste_Testata.Richiesta_Cod AND UMA_Richieste_Lavorazioni.Tipo_Carburante = 2 ")
            StrProprioTerzi.AppendLine(" WHERE UMA_Richieste_Testata.Avanzamento_Richiesta = 0 ")
            StrProprioTerzi.AppendLine(" AND Pratiche.Anno = {0} ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Piva = '{1}' ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Tipo_Richiesta = {2} ")
            StrProprioTerzi.AppendLine(" AND Pratiche_Stati_Attuali.Stato_Cod IN (2003, 2005, 2008) ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Carburante_Calcolato = 0 ")
            StrProprioTerzi.AppendLine(" GROUP BY UMA_Richieste_Testata.piva, UMA_Richieste_Testata.richiesta_cod ")
            '3) Benzina con carburante calcolato
            StrProprioTerzi.AppendLine(" -- 3) Benzina con carburante calcolato ")
            StrProprioTerzi.AppendLine(" UNION ALL ")
            StrProprioTerzi.AppendLine(" SELECT UMA_Richieste_Testata.piva, UMA_Richieste_Testata.richiesta_cod, 3 As Tipo_Carburante, ")
            StrProprioTerzi.AppendLine(" CASE WHEN SUM(ISNULL(UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, 0)) + SUM(ISNULL(UMA_Lavorazioni_Parziali.Fabbisogno_Assegnato, 0)) > ISNULL(MAX(UMA_Richieste_Testata.Approvazione_Iniziale_Benzina),0) ")
            StrProprioTerzi.AppendLine("       AND SUM(ISNULL(UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, 0)) + SUM(ISNULL(UMA_Lavorazioni_Parziali.Fabbisogno_Assegnato, 0)) > 0 ")
            StrProprioTerzi.AppendLine(" THEN SUM(ISNULL(UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, 0)) + SUM(ISNULL(UMA_Lavorazioni_Parziali.Fabbisogno_Assegnato, 0)) ")
            StrProprioTerzi.AppendLine(" ELSE ISNULL(MAX(UMA_Richieste_Testata.Approvazione_Iniziale_Benzina),0) END as Assegnato ")
            StrProprioTerzi.AppendLine(" FROM UMA_Richieste_Testata ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" LEFT JOIN UMA_Richieste_Lavorazioni ON UMA_Richieste_Lavorazioni.Richiesta_Cod = UMA_Richieste_Testata.Richiesta_Cod AND UMA_Richieste_Lavorazioni.Tipo_Carburante = 3 ")
            StrProprioTerzi.AppendLine(" LEFT JOIN UMA_Lavorazioni_Parziali ON UMA_Lavorazioni_Parziali.Richiesta_Cod = UMA_Richieste_Testata.Richiesta_Cod AND UMA_Lavorazioni_Parziali.Tipo_Carburante = 3 ")
            StrProprioTerzi.AppendLine(" WHERE UMA_Richieste_Testata.Avanzamento_Richiesta = 0 ")
            StrProprioTerzi.AppendLine(" AND Pratiche.Anno = {0} ")
            StrProprioTerzi.AppendLine(" And UMA_Richieste_Testata.Piva = '{1}' ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Tipo_Richiesta = {2} ")
            StrProprioTerzi.AppendLine(" AND Pratiche_Stati_Attuali.Stato_Cod IN (2003, 2005, 2008) AND UMA_Richieste_Testata.Carburante_Calcolato > 0 ")
            StrProprioTerzi.AppendLine(" GROUP BY UMA_Richieste_Testata.piva, UMA_Richieste_Testata.richiesta_cod ")
            '4) Benzina senza carburante calcolato
            StrProprioTerzi.AppendLine(" -- 4) Benzina senza carburante calcolato ")
            StrProprioTerzi.AppendLine(" UNION ALL ")
            StrProprioTerzi.AppendLine(" SELECT UMA_Richieste_Testata.piva, UMA_Richieste_Testata.richiesta_cod, 3 As Tipo_Carburante, ")
            StrProprioTerzi.AppendLine(" CASE WHEN SUM(ISNULL(UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, 0)) > ISNULL(MAX(UMA_Richieste_Testata.Approvazione_Iniziale_Benzina),0) ")
            StrProprioTerzi.AppendLine(" THEN SUM(ISNULL(UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, 0)) ")
            StrProprioTerzi.AppendLine(" ELSE ISNULL(MAX(UMA_Richieste_Testata.Approvazione_Iniziale_Benzina),0) END as Assegnato ")
            StrProprioTerzi.AppendLine(" FROM UMA_Richieste_Testata ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" LEFT JOIN UMA_Richieste_Lavorazioni ON UMA_Richieste_Lavorazioni.Richiesta_Cod = UMA_Richieste_Testata.Richiesta_Cod AND UMA_Richieste_Lavorazioni.Tipo_Carburante = 3 ")
            StrProprioTerzi.AppendLine(" WHERE UMA_Richieste_Testata.Avanzamento_Richiesta = 0 ")
            StrProprioTerzi.AppendLine(" AND Pratiche.Anno = {0} ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Piva = '{1}' ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Tipo_Richiesta = {2} ")
            StrProprioTerzi.AppendLine(" AND Pratiche_Stati_Attuali.Stato_Cod IN (2003, 2005, 2008) ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Carburante_Calcolato = 0 ")
            StrProprioTerzi.AppendLine(" GROUP BY UMA_Richieste_Testata.piva, UMA_Richieste_Testata.richiesta_cod ")
            '5) Gasolio serra con carburante calcolato
            StrProprioTerzi.AppendLine(" -- 5) Gasolio serra con carburante calcolato ")
            StrProprioTerzi.AppendLine(" UNION ALL ")
            StrProprioTerzi.AppendLine(" SELECT UMA_Richieste_Testata.piva, UMA_Richieste_Testata.richiesta_cod, 8 As Tipo_Carburante, ")
            StrProprioTerzi.AppendLine(" CASE WHEN SUM(ISNULL(UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, 0)) + SUM(ISNULL(UMA_Lavorazioni_Parziali.Fabbisogno_Assegnato, 0)) > ISNULL(MAX(UMA_Richieste_Testata.Approvazione_Iniziale_Gasolio_Serra),0) ")
            StrProprioTerzi.AppendLine("       AND SUM(ISNULL(UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, 0)) + SUM(ISNULL(UMA_Lavorazioni_Parziali.Fabbisogno_Assegnato, 0)) > 0 ")
            StrProprioTerzi.AppendLine(" THEN SUM(ISNULL(UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, 0)) + SUM(ISNULL(UMA_Lavorazioni_Parziali.Fabbisogno_Assegnato, 0)) ")
            StrProprioTerzi.AppendLine(" ELSE ISNULL(MAX(UMA_Richieste_Testata.Approvazione_Iniziale_Gasolio_Serra),0) END as Assegnato ")
            StrProprioTerzi.AppendLine(" FROM UMA_Richieste_Testata ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" LEFT JOIN UMA_Richieste_Lavorazioni ON UMA_Richieste_Lavorazioni.Richiesta_Cod = UMA_Richieste_Testata.Richiesta_Cod AND UMA_Richieste_Lavorazioni.Tipo_Carburante = 8 ")
            StrProprioTerzi.AppendLine(" LEFT JOIN UMA_Lavorazioni_Parziali ON UMA_Lavorazioni_Parziali.Richiesta_Cod = UMA_Richieste_Testata.Richiesta_Cod AND UMA_Lavorazioni_Parziali.Tipo_Carburante = 8 ")
            StrProprioTerzi.AppendLine(" WHERE UMA_Richieste_Testata.Avanzamento_Richiesta = 0 ")
            StrProprioTerzi.AppendLine(" AND Pratiche.Anno = {0} ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Piva = '{1}' ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Tipo_Richiesta = {2} ")
            StrProprioTerzi.AppendLine(" AND Pratiche_Stati_Attuali.Stato_Cod IN (2003, 2005, 2008) AND UMA_Richieste_Testata.Carburante_Calcolato > 0 ")
            StrProprioTerzi.AppendLine(" GROUP BY UMA_Richieste_Testata.piva, UMA_Richieste_Testata.richiesta_cod ")
            '6) Gasolio serra senza carburante calcolato
            StrProprioTerzi.AppendLine(" -- 6) Gasolio serra senza carburante calcolato ")
            StrProprioTerzi.AppendLine(" UNION ALL ")
            StrProprioTerzi.AppendLine(" SELECT UMA_Richieste_Testata.piva, UMA_Richieste_Testata.richiesta_cod, 8 As Tipo_Carburante, ")
            StrProprioTerzi.AppendLine(" CASE WHEN SUM(ISNULL(UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, 0)) > ISNULL(MAX(UMA_Richieste_Testata.Approvazione_Iniziale_Gasolio_Serra),0) ")
            StrProprioTerzi.AppendLine(" THEN SUM(ISNULL(UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, 0)) ")
            StrProprioTerzi.AppendLine(" ELSE ISNULL(MAX(UMA_Richieste_Testata.Approvazione_Iniziale_Gasolio_Serra),0)  END as Assegnato ")
            StrProprioTerzi.AppendLine(" FROM UMA_Richieste_Testata ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" LEFT JOIN UMA_Richieste_Lavorazioni ON UMA_Richieste_Lavorazioni.Richiesta_Cod = UMA_Richieste_Testata.Richiesta_Cod AND UMA_Richieste_Lavorazioni.Tipo_Carburante = 8 ")
            StrProprioTerzi.AppendLine(" WHERE UMA_Richieste_Testata.Avanzamento_Richiesta = 0 ")
            StrProprioTerzi.AppendLine(" AND Pratiche.Anno = {0} ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Piva = '{1}' ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Tipo_Richiesta = {2} ")
            StrProprioTerzi.AppendLine(" AND Pratiche_Stati_Attuali.Stato_Cod IN (2003, 2005, 2008) ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Carburante_Calcolato = 0 ")
            StrProprioTerzi.AppendLine(" GROUP BY UMA_Richieste_Testata.piva, UMA_Richieste_Testata.richiesta_cod ")
            '7) Anticipi
            StrProprioTerzi.AppendLine(" -- 7) Anticipi ")
            StrProprioTerzi.AppendLine(" UNION ALL ")
            StrProprioTerzi.AppendLine(" SELECT UMA_Richieste_Testata.piva, 0, rlav.Tipo_Carburante, SUM(rlav.Fabbisogno_Richiesto) as Assegnato ")
            StrProprioTerzi.AppendLine(" FROM UMA_Richieste_Testata ")
            StrProprioTerzi.AppendLine(" JOIN UMA_Richieste_Lavorazioni as rlav ON UMA_Richieste_Testata.Richiesta_Cod = rlav.Richiesta_Cod  ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" WHERE Avanzamento_Richiesta in (-1) ")
            StrProprioTerzi.AppendLine(" AND Pratiche.Anno = {0} ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Piva = '{1}' ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Tipo_Richiesta = {2}  ")
            StrProprioTerzi.AppendLine(" AND Pratiche_Stati_Attuali.Stato_Cod IN (2003, 2005, 2008) ")
            StrProprioTerzi.AppendLine(" AND NOT EXISTS(SELECT UMA_Richieste_Testata.piva, rlav.Tipo_Carburante, SUM(rlav.Fabbisogno_Richiesto) as Assegnato ")
            StrProprioTerzi.AppendLine(" FROM UMA_Richieste_Testata ")
            StrProprioTerzi.AppendLine(" JOIN UMA_Richieste_Lavorazioni as rlav ON UMA_Richieste_Testata.Richiesta_Cod = rlav.Richiesta_Cod  ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" WHERE Avanzamento_Richiesta in (0) ")
            StrProprioTerzi.AppendLine(" AND Pratiche.Anno = {0} ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Piva = '{1}' ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Tipo_Richiesta = {2}  ")
            StrProprioTerzi.AppendLine(" AND Pratiche_Stati_Attuali.Stato_Cod IN (2003, 2005, 2008) ")
            StrProprioTerzi.AppendLine(" GROUP BY UMA_Richieste_Testata.piva, rlav.Tipo_Carburante) ")
            StrProprioTerzi.AppendLine(" AND NOT EXISTS(SELECT UMA_Richieste_Testata.piva, rlav.Tipo_Carburante, SUM(rlav.Fabbisogno_Richiesto) as Assegnato ")
            StrProprioTerzi.AppendLine(" FROM UMA_Richieste_Testata ")
            StrProprioTerzi.AppendLine(" JOIN UMA_Lavorazioni_Parziali as rlav ON UMA_Richieste_Testata.Richiesta_Cod = rlav.Richiesta_Cod  ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" WHERE Avanzamento_Richiesta in (0) ")
            StrProprioTerzi.AppendLine(" AND Pratiche.Anno = {0} ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Piva = '{1}' ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Tipo_Richiesta = {2}  ")
            StrProprioTerzi.AppendLine(" AND Pratiche_Stati_Attuali.Stato_Cod IN (2003, 2005, 2008) ")
            StrProprioTerzi.AppendLine(" GROUP BY UMA_Richieste_Testata.piva, rlav.Tipo_Carburante) ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.piva NOT IN (SELECT UMA_Richieste_Testata.piva ")
            StrProprioTerzi.AppendLine(" FROM UMA_Richieste_Testata ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod ")
            StrProprioTerzi.AppendLine(" WHERE Avanzamento_Richiesta in (0) ")
            StrProprioTerzi.AppendLine(" AND Pratiche.Anno = {0} ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Piva = '{1}' ")
            StrProprioTerzi.AppendLine(" AND UMA_Richieste_Testata.Tipo_Richiesta = {2} ")
            StrProprioTerzi.AppendLine(" AND Pratiche_Stati_Attuali.Stato_Cod IN (2003, 2005, 2008) ")
            StrProprioTerzi.AppendLine(" AND (UMA_Richieste_Testata.Approvazione_Iniziale_Benzina > 0 OR UMA_Richieste_Testata.Approvazione_Iniziale_Gasolio > 0 OR UMA_Richieste_Testata.Approvazione_Iniziale_Gasolio_Serra > 0)) ")
            StrProprioTerzi.AppendLine(" GROUP BY UMA_Richieste_Testata.piva, rlav.Tipo_Carburante ")
            '
            StrProprioTerzi.AppendLine(" )")
        End If

        StrSQL.AppendLine(" With prima_richiesta_anno (piva, rimanenza_benz, rimanenza_gas, rimanenza_ser) As ( ")
        StrSQL.AppendLine(" Select top 1 UMA_Richieste_Testata.Piva, UMA_Richieste_Testata.Rimanenza_Benzina, UMA_Richieste_Testata.Rimanenza_Gasolio, UMA_Richieste_Testata.Rimanenza_Gasolio_Serra ")
        StrSQL.AppendLine(" FROM UMA_Richieste_Testata")
        StrSQL.AppendLine(" JOIN Pratiche On UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod")
        StrSQL.AppendLine(" JOIN Pratiche_Stati_Attuali On Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod")
        StrSQL.AppendLine(" WHERE Avanzamento_Richiesta in (0)")
        StrSQL.AppendLine(" And Pratiche.Anno = {0}")
        StrSQL.AppendLine(" And UMA_Richieste_Testata.Piva = '{1}'")
        StrSQL.AppendLine(" AND UMA_Richieste_Testata.Tipo_Richiesta = {2}")
        StrSQL.AppendLine(" AND Pratiche_Stati_Attuali.Stato_Cod IN (2003, 2005, 2008)")
        StrSQL.AppendLine(" ORDER BY UMA_Richieste_Testata.Data_Creazione ASC")
        StrSQL.AppendLine(" ),")
        '
        StrSQL.AppendLine(StrProprioTerzi.ToString())
        '
        StrSQL.AppendLine(" SELECT Totali_Assegnati.Piva, MAX(Totali_Assegnati.Tipo_Carburante) As Tipo_Carburante, SUM(Totali_Assegnati.Assegnato) - MAX(ISNULL(prima_richiesta_anno.rimanenza_gas,0)) As Assegnato ")
        StrSQL.AppendLine(" FROM Totali_Assegnati ")
        StrSQL.AppendLine(" LEFT JOIN prima_richiesta_anno ON prima_richiesta_anno.piva = Totali_Assegnati.Piva ")
        StrSQL.AppendLine(" where Totali_Assegnati.Tipo_Carburante = 2 ")
        StrSQL.AppendLine(" Group By Totali_Assegnati.Piva ")
        StrSQL.AppendLine(" UNION ALL")
        StrSQL.AppendLine(" SELECT Totali_Assegnati.Piva, MAX(Totali_Assegnati.Tipo_Carburante) As Tipo_Carburante, SUM(Totali_Assegnati.Assegnato) - MAX(ISNULL(prima_richiesta_anno.rimanenza_benz,0)) As Assegnato ")
        StrSQL.AppendLine(" FROM Totali_Assegnati ")
        StrSQL.AppendLine(" LEFT JOIN prima_richiesta_anno ON prima_richiesta_anno.piva = Totali_Assegnati.Piva ")
        StrSQL.AppendLine(" where Totali_Assegnati.Tipo_Carburante = 3 ")
        StrSQL.AppendLine(" Group By Totali_Assegnati.Piva ")
        StrSQL.AppendLine(" UNION ALL")
        StrSQL.AppendLine(" SELECT Totali_Assegnati.Piva, MAX(Totali_Assegnati.Tipo_Carburante) As Tipo_Carburante, SUM(Totali_Assegnati.Assegnato) - MAX(ISNULL(prima_richiesta_anno.rimanenza_ser,0)) As Assegnato ")
        StrSQL.AppendLine(" FROM Totali_Assegnati ")
        StrSQL.AppendLine(" LEFT JOIN prima_richiesta_anno ON prima_richiesta_anno.piva = Totali_Assegnati.Piva ")
        StrSQL.AppendLine(" where Totali_Assegnati.Tipo_Carburante = 8 ")
        StrSQL.AppendLine(" Group By Totali_Assegnati.Piva ")

        Dim strSqlFormat = String.Format(StrSQL.ToString(), filtri.Anno, filtri.PivaCliente, filtri.ContoProprioTerzi)

        Dim dt = EseguiQuery_Lettura(_coreDBContext, strSqlFormat, MethodBase.GetCurrentMethod().Name)

        For Each dr As DataRow In dt.Rows
            Dim tipoCarburante = dr.Item("Tipo_Carburante")
            If tipoCarburante <> filtri.Tipo_Carburante Then
                Continue For
            End If
            Return dr.Item("Assegnato")
        Next
        Return 0
    End Function

    Public Function OttieneTotaleLtAcquistati(parametri As FiltriAggiornaTotaleLtAcquistati) As Integer
        Dim StrSQL As New System.Text.StringBuilder()
        StrSQL.Append("SELECT Anno, Tipo_Carburante, sum(lt) as TotaleLtAcquistati
                       FROM UMA_Vendite
                       Join Imprese_Codici ic on ic.PIVA = UMA_Vendite.PIVA_Cliente and ic.id_cod = 1010
                       WHERE ic.val_cod = '" & Agro_SQL_SaveText(parametri.Cuaa) & "' 
                       AND UMA_Vendite.Conto_Proprio_Terzi = " & Agro_SQL_SaveNum(parametri.ContoProprioContoTerzi) & "
                       GROUP BY UMA_Vendite.Anno, UMA_Vendite.tipo_Carburante")

        Dim dt = EseguiQuery_Lettura(_coreDBContext, StrSQL.ToString(), MethodBase.GetCurrentMethod().Name)

        For Each dr As DataRow In dt.Rows
            If dr.Item("Anno") = parametri.Anno AndAlso dr.Item("Tipo_Carburante") = parametri.TipoCarburante Then
                Return dr.Item("TotaleLtAcquistati")
            End If
        Next
        Return 0
    End Function
    Public Function VerificaElementoNonEsisteInDB(lav As VenditaCarburantiDto) As Boolean
        Dim exists = _EFContext.UMA_Vendite.Any(Function(s) s.Id_Vendite = lav.Id_Vendite)
        Return exists
    End Function

    Public Function LeggiCarburanteVendutoAlCliente(pivaCliente As String, anno As Integer, tipoCarburante As Integer, contoProprioTerzi As Integer) As List(Of UMA_Vendite)
        Dim elementi = _EFContext.UMA_Vendite.Where(Function(s) s.PIVA_Cliente = pivaCliente AndAlso s.Anno = anno AndAlso s.Tipo_Carburante = tipoCarburante AndAlso s.Conto_Proprio_Terzi = contoProprioTerzi).ToList()
        Return elementi
    End Function
End Class