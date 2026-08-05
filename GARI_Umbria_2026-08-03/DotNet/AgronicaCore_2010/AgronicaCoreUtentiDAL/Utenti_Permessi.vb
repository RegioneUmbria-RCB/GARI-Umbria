Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.exceptions

<CachedDataProviderAttribute("Utenti_Permessi_R")>
Public Class Utenti_Permessi_R
    Inherits AgronicaCoreDataProvider.CachedDataProvider

    <Cacheable(True)>
    Public Function Controlla_Permessi_Utente(ByVal UserName As String,
                                              ByVal Id_Servizio As Integer,
                                              ByVal Id_Attivita As enum_Security_Attivita,
                                              ByVal Id_Operazione As enum_Security_Operazione,
                                              ByVal DataOraControllo As Date,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As Boolean

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Permessi_R.Controlla_Permessi_Utente()"

        Dim minutiTrascorsiGiornata As Integer
        minutiTrascorsiGiornata = DataOraControllo.Hour * 60 + DataOraControllo.Minute

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.AppendLine("SELECT *")
            StrSQL.AppendLine("FROM Utenti_Permessi ")
            StrSQL.AppendLine($"WHERE UserName = '{Agro_SQL_SaveText(Trim(UserName))}'")
            StrSQL.AppendLine($"    AND Id_Servizio = '{Agro_SQL_SaveText(Trim(Id_Servizio))}'")
            StrSQL.AppendLine($"    AND Id_Attivita = '{Agro_SQL_SaveText(Trim(Id_Attivita))}'")
            StrSQL.AppendLine($"    AND Id_Operazione = '{Agro_SQL_SaveText(Trim(Id_Operazione))}'")
            StrSQL.AppendLine($"    AND Validita_inizio <= {Agro_SQL_SaveDate(DataOraControllo.Date)}")
            StrSQL.AppendLine($"    AND Validita_Fine >= {Agro_SQL_SaveDate(DataOraControllo.Date)}")
            'StrSQL.AppendLine($"    AND (((Inizio_Ore*60)+Inizio_Minuti) <= {minutiTrascorsiGiornata})")
            'StrSQL.AppendLine($"    AND (((Fine_Ore*60)+Fine_Minuti) >= {minutiTrascorsiGiornata})")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
            Return True
        End If

        Return False

    End Function

    <Cacheable(False)>
    Public Function Controlla_Permessi_Utente_Tutti(ByVal UserName As String,
                                                    ByVal Id_Servizio As Integer,
                                                    ByVal Id_Attivita As enum_Security_Attivita,
                                                    ByVal Id_Operazione As enum_Security_Operazione,
                                                    ByVal DataOraControllo As Date,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Permessi_R.Controlla_Permessi_Utente_Tutti()"

        Dim minutiTrascorsiGiornata As Integer
        minutiTrascorsiGiornata = DataOraControllo.Hour * 60 + DataOraControllo.Minute

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Utenti_Permessi ")
            StrSQL.Append(" WHERE  UserName = '" & Agro_SQL_SaveText(Trim(UserName)) & "'")
            StrSQL.Append(" AND  Id_Servizio = '" & Agro_SQL_SaveText(Trim(Id_Servizio)) & "'")
            'StrSQL.Append(" AND  Id_Attivita = '" & Agro_SQL_SaveText(Trim(Id_Attivita)) & "'")

            If Id_Attivita <> 0 Then
                StrSQL.Append(" AND Id_Attivita = " & Agro_SQL_SaveText(Trim(Id_Attivita)) & " ")
            End If

            StrSQL.Append(" AND  Id_Operazione = '" & Agro_SQL_SaveText(Trim(Id_Operazione)) & "'")
            StrSQL.Append(" AND  Validita_inizio <= " & Agro_SQL_SaveDate(DataOraControllo) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(DataOraControllo) & " ")
            StrSQL.Append(" AND (((Inizio_Ore*60)+Inizio_Minuti) <= " & minutiTrascorsiGiornata & ")")
            StrSQL.Append(" AND (((Fine_Ore*60)+Fine_Minuti) >= " & minutiTrascorsiGiornata & ")")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Il campo Id_Operazione deve avere come valore 9999 per evitare di filtrare i
    ''' record per un suo valore. Non si é utilizzato il valore 0 perché nella
    ''' tabella dove sono definite le operazioni l'operazione Lettura ha codice 0
    ''' </summary>
    ''' <param name="UserName"></param>
    ''' <param name="Id_Servizio"></param>
    ''' <param name="Id_Attivita"></param>
    ''' <param name="Id_Operazione"></param>
    ''' <param name="ID"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	16/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi(ByVal UserName As String,
                          ByVal Id_Servizio As Integer,
                          ByVal Id_Attivita As enum_Security_Attivita,
                          ByVal Id_Operazione As enum_Security_Operazione,
                          ByVal ID As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Permessi_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Utenti_Permessi  (NOLOCK) ")
            StrSQL.Append(" WHERE UserName = '" & Agro_SQL_SaveText(UserName) & "' ")

            If Id_Servizio <> 0 Then
                StrSQL.Append(" AND Id_Servizio = " & Id_Servizio & " ")
            End If

            If Id_Attivita <> 0 Then
                StrSQL.Append(" AND Id_Attivita = " & Id_Attivita & " ")
            End If

            If Id_Operazione <> 9999 Then
                StrSQL.Append(" AND Id_Operazione = " & Id_Operazione & " ")
            End If

            If ID <> 0 Then
                StrSQL.Append(" AND ID = " & ID & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Utenti_Permessi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Utenti_Permessi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Inizio_Ore ASC")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    <Cacheable(True)>
    Public Function LeggiCached(ByVal UserName As String,
                          ByVal Id_Servizio As Integer,
                          ByVal Id_Attivita As enum_Security_Attivita,
                          ByVal Id_Operazione As enum_Security_Operazione,
                          ByVal ID As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Permessi_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Utenti_Permessi  (NOLOCK) ")
            StrSQL.Append(" WHERE UserName = '" & Agro_SQL_SaveText(UserName) & "' ")

            If Id_Servizio <> 0 Then
                StrSQL.Append(" AND Id_Servizio = " & Id_Servizio & " ")
            End If

            If Id_Attivita <> 0 Then
                StrSQL.Append(" AND Id_Attivita = " & Id_Attivita & " ")
            End If

            If Id_Operazione <> 9999 Then
                StrSQL.Append(" AND Id_Operazione = " & Id_Operazione & " ")
            End If

            If ID <> 0 Then
                StrSQL.Append(" AND ID = " & ID & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Utenti_Permessi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Utenti_Permessi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Inizio_Ore ASC")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' <summary>
    ''' Verifica l'esistenza della tabella Cliente_Permessi usata per la gestione delle funzionalità attive a livello SU.
    ''' </summary>
    ''' <param name="objParametri">objParametri_Utenti</param>
    ''' <returns>True se la tabella esiste, False altrimenti</returns>
    Public Function VerificaEsistenzaTabellaClientePermessi(objParametri As AgronicaCoreParametri)
        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Permessi_R.VerificaEsistenzaTabellaClientePermessi()"
        Dim StrSQL As New StringBuilder With {.Length = 0}
        Try
            StrSQL.AppendLine(" SELECT 1 FROM  Cliente_Permessi ")
            Dim dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Public Function LeggiCliente_Permessi(
        ByVal UserName As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable
        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Permessi_R.LeggiCliente_Permessi()"
        Dim StrSQL As New StringBuilder With {.Length = 0}
        Dim dt As DataTable
        Try
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Cliente_Permessi ")
            StrSQL.AppendLine(" WHERE Id_Attivita != 0 ")
            If Not String.IsNullOrWhiteSpace(UserName) Then
                StrSQL.AppendLine(" AND UserName = '" & Agro_SQL_SaveText(UserName) & "' ")
            End If
            If Not String.IsNullOrWhiteSpace(xFiltroAggiuntivo) Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Cliente_Permessi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Cliente_Permessi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
        Return dt
    End Function

    Public Function LeggixOggetto(ByVal UserName As String,
                                  ByVal DataOraControllo As Date,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Permessi_R.LeggixOggetto()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Utenti_Permessi ")
            StrSQL.Append(" WHERE UserName = '" & Agro_SQL_SaveText(UserName) & "' ")

            StrSQL.Append(" AND  Validita_inizio <= " & Agro_SQL_SaveDate(DataOraControllo) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(DataOraControllo) & " ")
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Utenti_Permessi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Utenti_Permessi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Permessi_APP(ByVal UserName As String,
                                       ByVal DataOraControllo As Date,
                                       ByRef objParametri As AgronicaCoreParametri,
                                       ByRef gestionePermessiGiasAPP As String
                                       ) As DataTable

        ' mappatura permessi utente / impostazioni APP
        Dim mappaPermessi As New Dictionary(Of Integer, Integer) From {
            {enum_Security_Attivita.GiasAPP_NUOVA_RICETTA, enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_NUOVA_RICETTA},
            {enum_Security_Attivita.GiasAPP_NUOVO_INTERVENTO, enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_NUOVO_INTERVENTO},
            {enum_Security_Attivita.GiasAPP_INTERVENTI_DA_FARE, enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_INTERVENTI_DA_FARE},
            {enum_Security_Attivita.GiasAPP_SCARICO_ORE, enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_SCARICO_ORE},
            {enum_Security_Attivita.GiasAPP_ENTRATAUSCITA, enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_ENTRATAUSCITA},
            {enum_Security_Attivita.GiasAPP_LAMIAPOSIZIONE, enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_LAMIAPOSIZIONE},
            {enum_Security_Attivita.GiasAPP_VISITE, enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_VISITE},
            {enum_Security_Attivita.GiasAPP_DOCUMENTI, enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_DOCUMENTI},
            {enum_Security_Attivita.GiasAPP_RILIEVI, enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_RILIEVI},
            {enum_Security_Attivita.GiasAPP_GIS, enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_GIS},
            {enum_Security_Attivita.GiasAPP_InCab, enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_InCab}
        }

        Dim dtImpostazioni As New DataTable
        dtImpostazioni.Columns.Add(New DataColumn("Impostazione_Cod", GetType(Integer)))
        dtImpostazioni.Columns.Add(New DataColumn("Impostazione_Valore_1", GetType(String)))

        For Each permesso In mappaPermessi

            Dim lettura As Boolean = Controlla_Permessi_Utente(UserName, enum_Id_Servizio.GiasOnline, permesso.Key, enum_Security_Operazione.Lettura, DataOraControllo, "", objParametri)
            Dim scrittura As Boolean = Controlla_Permessi_Utente(UserName, enum_Id_Servizio.GiasOnline, permesso.Key, enum_Security_Operazione.Modifica, DataOraControllo, "", objParametri)

            Dim drImpo = dtImpostazioni.NewRow
            drImpo.Item("Impostazione_Cod") = permesso.Value
            drImpo.Item("Impostazione_Valore_1") = If(scrittura, "1", If(lettura, "2", "0"))
            dtImpostazioni.Rows.Add(drImpo)

        Next

        ' aggiungo nuovi permessi gestiti dall'app
        Dim altriPermessi As New List(Of Integer) From {
            enum_Security_Attivita.GiasAPP_PianoColturale,
            enum_Security_Attivita.GiasAPP_Magazzini,
            enum_Security_Attivita.GiasAPP_Macchine,
            enum_Security_Attivita.GiasAPP_Manutenzioni,
            enum_Security_Attivita.GiasAPP_DDT_Movimenti,
            enum_Security_Attivita.GiasAPP_Gias,
            enum_Security_Attivita.GiasAPP_Aziende,
            enum_Security_Attivita.GiasAPP_Centri,
            enum_Security_Attivita.GiasAPP_Isolamenti,
            enum_Security_Attivita.GiasAPP_DSS_Difesa,
            enum_Security_Attivita.GiasAPP_Consiglio_Irriguo,
            enum_Security_Attivita.GiasAPP_Consiglio_Fertirriguo,
            enum_Security_Attivita.GiasAPP_Precision_Farming,
            enum_Security_Attivita.GiasAPP_Monitoraggio_Meteo,
            enum_Security_Attivita.GiasAPP_Lavoratori,
            enum_Security_Attivita.GiasAPP_Widget_Rischi_Meteo,
            enum_Security_Attivita.GiasAPP_Widget_Rischi_Difesa,
            enum_Security_Attivita.GiasAPP_Widget_Consiglio_Semina,
            enum_Security_Attivita.GiasAPP_Widget_Consiglio_Nutrizione,
            enum_Security_Attivita.GiasAPP_Widget_Consiglio_Irriguo,
            enum_Security_Attivita.GiasAPP_Widget_Consiglio_Raccolta,
            enum_Security_Attivita.GiasAPP_Chatbot,
            enum_Security_Attivita.GiasAPP_Allarmi_Widget,
            enum_Security_Attivita.GiasAPP_Dati_Meteo_Storici,
            enum_Security_Attivita.GiasAPP_Indici_Satellitari,
            enum_Security_Attivita.GiasAPP_Geofoto,
            enum_Security_Attivita.GiasAPP_Consultazione_Dati_Sensori,
            enum_Security_Attivita.GiasAPP_Squadre_Lavoratori,
            enum_Security_Attivita.GiasAPP_Creazione_Fornitore,
            enum_Security_Attivita.GiasAPP_Gestione_Tracce,
            enum_Security_Attivita.GiasAPP_Gestione_Fasi_Fenologiche,
            enum_Security_Attivita.GiasAPP_Gestione_Trappole,
            enum_Security_Attivita.GiasAPP_Notizie_Coldiretti,
            enum_Security_Attivita.GiasAPP_Messaggi,
            enum_Security_Attivita.GiasAPP_Promemoria,
            enum_Security_Attivita.GiasAPP_Registrazione_Rilievi_Pedologici,
            enum_Security_Attivita.GiasAPP_Modulo_BeLeaf
        }
        For Each permesso In altriPermessi
            Dim lettura As Boolean = Controlla_Permessi_Utente(UserName, enum_Id_Servizio.GiasOnline, permesso, enum_Security_Operazione.Lettura, DataOraControllo, "", objParametri)
            Dim scrittura As Boolean = Controlla_Permessi_Utente(UserName, enum_Id_Servizio.GiasOnline, permesso, enum_Security_Operazione.Modifica, DataOraControllo, "", objParametri)
            gestionePermessiGiasAPP &= "|" & If(scrittura, "1", If(lettura, "2", "0"))
        Next

        Return dtImpostazioni

    End Function

    Public Function Leggi_tutti_utenti(ByVal Id_Servizio As Integer,
                                       ByVal Id_Attivita As enum_Security_Attivita,
                                       ByVal Id_Operazione As enum_Security_Operazione,
                                       ByVal ID As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Permessi_R.Leggi_tutti_utenti()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Utenti_Permessi ")
            StrSQL.Append(" WHERE 1= 1 ")

            If Id_Servizio <> 0 Then
                StrSQL.Append(" AND Id_Servizio = " & Id_Servizio & " ")
            End If

            If Id_Attivita <> 0 Then
                StrSQL.Append(" AND Id_Attivita = " & Id_Attivita & " ")
            End If

            If Id_Operazione <> 9999 Then
                StrSQL.Append(" AND Id_Operazione = " & Id_Operazione & " ")
            End If

            If ID <> 0 Then
                StrSQL.Append(" AND ID = " & ID & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Utenti_Permessi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Utenti_Permessi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Inizio_Ore ASC")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function LeggiJoinDettagli(ByVal Username_Utente As String,
                                      ByVal CodFisc_Utente As String,
                                      ByVal Id_Servizio As Integer,
                                      ByVal Id_Attivita As Integer,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal Id_Operazione As Integer = -1
                                      ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_Permessi_R.LeggiJoinDettagli()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT  ")
            strSql.AppendLine("         Utenti_Dettagli.UserName, Utenti_Dettagli.Flag_Azienda_Persona, Utenti_Dettagli.Cognome, Utenti_Dettagli.Nome, Utenti_Dettagli.PIVA, Utenti_Dettagli.CodFisc, Utenti_Dettagli.Rag_Soc, ")
            strSql.AppendLine("         Utenti_Dettagli.Data_Creazione, Utenti_Dettagli.Data_Modifica, Utenti_Dettagli.UserNameCommerciale, ")
            strSql.AppendLine("         Utenti_dettagli.email, Utenti_dettagli.Tel, ")
            '01/01/1900' su default Validita_Fine così si può capire che è scaduto o non ha il permesso
            strSql.AppendLine("         ISNULL(Utenti_Permessi.Validita_Inizio, '01/01/1900' ) AS Validita_Inizio, ISNULL(Utenti_Permessi.Validita_Fine, '01/01/1900') AS Validita_Fine, ")
            strSql.AppendLine("         Utenti_Permessi.Id_Attivita, Utenti_Permessi.Id_Servizio ")

            strSql.AppendLine(" FROM  Utenti_Dettagli ")
            strSql.AppendLine(" LEFT OUTER JOIN Utenti_Permessi ON Utenti_Dettagli.UserName = Utenti_Permessi.UserName ")

            strSql.AppendLine(" WHERE   1=1 ")

            If Id_Servizio <> 0 Then
                strSql.AppendLine(" AND   Utenti_Permessi.Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio))
            End If
            If Id_Attivita <> 0 Then
                strSql.AppendLine(" AND     Utenti_Permessi.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita))
            End If
            If Id_Operazione <> -1 Then
                strSql.AppendLine(" AND     Utenti_Permessi.Id_Operazione = " & Agro_SQL_SaveNum(Id_Operazione))
            End If
            If Username_Utente <> "" Then
                strSql.AppendLine(" AND     Utenti_Dettagli.UserName = '" & Agro_SQL_SaveText(Username_Utente) & "'")
            End If
            If CodFisc_Utente <> "" Then
                strSql.AppendLine(" AND     Utenti_Dettagli.CodFisc = '" & Agro_SQL_SaveText(CodFisc_Utente) & "'")
            End If
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Utenti_Dettagli.Nome, Utenti_Dettagli.Cognome ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§



Public Class Utenti_Permessi_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Cancella(ByVal UserName As String,
                             ByVal Id_Servizio As Integer,
                             ByVal Id_Attivita As Integer,
                             ByVal Id_Operazione As Integer,
                             ByVal ID As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AnagrafeCoreUtentiDAL.Utenti_Permessi_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Utenti_Permessi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE    UserName = '" & Agro_SQL_SaveText(UserName) & "' ")
                StrSQL.Append(" AND Inviato >= 0")

                If Id_Servizio <> 0 Then
                    StrSQL.Append(" AND Id_Servizio = " & Id_Servizio & " ")
                End If

                If Id_Attivita <> 0 Then
                    StrSQL.Append(" AND Id_Attivita = " & Id_Attivita & " ")
                End If

                If Id_Operazione <> 0 Then
                    StrSQL.Append(" AND Id_Operazione = " & Id_Operazione & " ")
                End If

                If ID <> 0 Then
                    StrSQL.Append(" AND ID = " & ID & " ")
                End If

            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Utenti_Permessi ")
                StrSQL.Append(" WHERE    UserName = '" & Agro_SQL_SaveText(UserName) & "' ")

                If Id_Servizio <> 0 Then
                    StrSQL.Append(" AND Id_Servizio = " & Id_Servizio & " ")
                End If

                If Id_Attivita <> 0 Then
                    StrSQL.Append(" AND Id_Attivita = " & Id_Attivita & " ")
                End If

                If Id_Operazione <> 0 Then
                    StrSQL.Append(" AND Id_Operazione = " & Id_Operazione & " ")
                End If

                If ID <> 0 Then
                    StrSQL.Append(" AND ID = " & ID & " ")
                End If

            End If
            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    '##############################################################################################
    ''' <summary>
    ''' Cancella permessi su più utenti.
    ''' </summary>
    ''' <param name="Id_Servizio">0: nessun filtro</param>
    ''' <param name="Id_Attivita">Codice del permesso da eliminare. 0: nessun filtro</param>
    ''' <param name="Id_Operazione">0: solo lettura, 1: nessun filtro, 2: scrittura</param>
    ''' <param name="objParametri">ObjParametri_Utenti</param>
    ''' <returns></returns>
    Public Function CancellaMultiplo(
        ByVal Id_Servizio As Integer, ByVal Id_Attivita As IEnumerable(Of Integer),
        ByVal Id_Operazione As Integer, ByVal ID As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As Boolean
        Const nomeRoutine = "AnagrafeCoreUtentiDAL.Utenti_Permessi_W.CancellaTutti()"
        Dim StrSQL As New StringBuilder With {.Length = 0}
        Dim xRisp As Boolean

        If Id_Servizio = 0 AndAlso Id_Attivita.Count = 0 AndAlso Id_Operazione = 1 AndAlso
            ID = 0 AndAlso String.IsNullOrWhiteSpace(xFiltroAggiuntivo) Then
            Throw New UnauthorizedAccessException("Operation aborted: risk of deleting everything on the table.")
        End If
        Dim xIn = Id_Attivita.Select(Function(cod) cod.ToString).
            Aggregate(Function(acc, cod) acc & ", " & cod)
        Try
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.AppendLine(" UPDATE Utenti_Permessi ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE 1=1 ")
                StrSQL.AppendLine(" AND Inviato >= 0")
            Else
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM     Utenti_Permessi ")
                StrSQL.AppendLine(" WHERE 1=1 ")
            End If

            If Id_Servizio <> 0 Then
                StrSQL.AppendLine(" AND Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & " ")
            End If
            If Id_Attivita.Any Then
                StrSQL.AppendLine(" AND Id_Attivita in ( " & Agro_SQL_Save_Clausola_IN(xIn) & ") ")
            End If
            If Id_Operazione <> 1 Then
                StrSQL.AppendLine(" AND Id_Operazione = " & Agro_SQL_SaveNum(Id_Operazione) & " ")
            End If
            If ID <> 0 Then
                StrSQL.AppendLine(" AND ID = " & Agro_SQL_SaveNum(ID) & " ")
            End If
            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
        Return xRisp
    End Function

    '##############################################################################################
    Public Function Scrivi(ByVal UserName As String,
                           ByVal Id_Servizio As Integer,
                           ByVal Id_Attivita As Integer,
                           ByVal Id_Operazione As Integer,
                           ByVal ID As Integer,
                           ByVal Inizio_Ore As Integer,
                           ByVal Inizio_Minuti As Integer,
                           ByVal Fine_Ore As Integer,
                           ByVal Fine_Minuti As Integer,
                           ByVal UserName_Creazione As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AnagrafeCoreUtentiDAL.Utenti_Permessi_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Utenti_Permessi (")
            StrSQL.Append("                    UserName, ")
            StrSQL.Append("                    Id_Servizio, ")
            StrSQL.Append("                    Id_Attivita, ")
            StrSQL.Append("                    Id_Operazione, ")
            StrSQL.Append("                    ID, ")
            StrSQL.Append("                    Inizio_Ore, ")
            StrSQL.Append("                    Inizio_Minuti, ")
            StrSQL.Append("                    Fine_Ore, ")
            StrSQL.Append("                    Fine_Minuti, ")
            StrSQL.Append("                    Inviato, ")
            StrSQL.Append("                    DataInvio, ")
            StrSQL.Append("                    Data_Creazione, ")
            StrSQL.Append("                    Data_Modifica, ")
            StrSQL.Append("                    Username_Creazione, ")
            StrSQL.Append("                    Username_Modifica, ")
            StrSQL.Append("                    Validita_Inizio, ")
            StrSQL.Append("                    Validita_Fine ")
            StrSQL.Append("                    ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(UserName) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Servizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Attivita) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Operazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Inizio_Ore) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Inizio_Minuti) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Fine_Ore) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Fine_Minuti) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function ScriviMultiplo(
        ByVal UserName As String, ByVal Id_Attivita As IEnumerable(Of Integer), Id_Operazione As enum_TipoPermesso,
        ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date, objParametri As AgronicaCoreParametri
    )
        Const nomeRoutine = "AnagrafeCoreUtentiDAL.Utenti_Permessi_W.ScriviMultiplo()"
        Dim StrSQL As New StringBuilder With {.Length = 0}
        Dim xRisp As Boolean
        If Id_Attivita Is Nothing OrElse Id_Attivita.Count = 0 Then
            Return True
        End If
        Dim xin = Id_Attivita.Select(Function(cod) cod.ToString).Aggregate(Function(acc, cod) acc & ", " & cod)
        Try
            StrSQL.AppendLine("; WITH Attivita_cte as (")
            StrSQL.AppendLine("    SELECT Id_Servizio, Id_Attivita FROM TB_Attivita ")
            StrSQL.AppendLine("    WHERE Id_Attivita in ( ")
            StrSQL.AppendLine("    " & Agro_SQL_Save_Clausola_IN(xin) & " ")
            StrSQL.AppendLine("    ) ")
            StrSQL.AppendLine(") ")

            StrSQL.AppendLine("INSERT INTO Utenti_Permessi (")
            StrSQL.AppendLine("    UserName, Id_Servizio, Id_Attivita, Id_Operazione, ID, Inviato, ")
            StrSQL.AppendLine("    Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine ")
            StrSQL.AppendLine(") ")
            StrSQL.AppendLine("SELECT ")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(UserName) & "' as UserName ")
            StrSQL.AppendLine("         , Id_Servizio, Id_Attivita ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_Operazione) & " as Id_Operazione  ")
            StrSQL.AppendLine("         , 1 as ID, 0 as Inviato  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' as Username_Creazione ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' as Username_Modifica ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & " as Validita_Inizio ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & " as Validita_Fine ")
            StrSQL.AppendLine("FROM Attivita_cte")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
        Return xRisp
    End Function

    Public Function ScriviLetturaScrittura(
        ByVal UserName As String, ByVal Id_Servizio As Integer, ByVal Id_Attivita As Integer,
        ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date, objParametri As AgronicaCoreParametri
    )
        Dim rOk = Scrivi(
            UserName, Id_Servizio, Id_Attivita, enum_TipoPermesso.LETTURA,
            1, 0, 0, 23, 59, objParametri.UsernameOperazione,
            Validita_Inizio, Validita_Fine, objParametri
        )
        If rOk Then
            Dim wOk = Scrivi(
                UserName, Id_Servizio, Id_Attivita, enum_TipoPermesso.LETTURA_SCRITTURA,
                1, 0, 0, 23, 59, objParametri.UsernameOperazione,
                Validita_Inizio, Validita_Fine, objParametri
            )
            Return rOk AndAlso wOk
        End If
        Return False
    End Function

    '##############################################################################################
    Public Function Scrivi_Cliente_Permessi(ByVal UserName As String,
                                                        ByVal Id_Servizio As Integer,
                                                        ByVal Id_Attivita As Integer,
                                                        ByVal Id_Operazione As Integer,
                                                        ByVal Validita_Inizio As Date,
                                                        ByVal Validita_Fine As Date,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                        ) As Boolean

        Const nomeRoutine = "AnagrafeCoreUtentiDAL.Utenti_Permessi_W.AggiornaCliente_Permessi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("INSERT INTO Cliente_Permessi (")
            StrSQL.AppendLine("                    PivaSuperUser, ")
            StrSQL.AppendLine("                    UserName, ")
            StrSQL.AppendLine("                    Id_Servizio, ")
            StrSQL.AppendLine("                    Id_Attivita, ")
            StrSQL.AppendLine("                    Id_Operazione, ")
            StrSQL.AppendLine("                    Inviato, ")
            StrSQL.AppendLine("                    DataInvio, ")
            StrSQL.AppendLine("                    Data_Creazione, ")
            StrSQL.AppendLine("                    Data_Modifica, ")
            StrSQL.AppendLine("                    Username_Creazione, ")
            StrSQL.AppendLine("                    Username_Modifica, ")
            StrSQL.AppendLine("                    Validita_Inizio, ")
            StrSQL.AppendLine("                    Validita_Fine ")
            StrSQL.AppendLine("                    ) ")

            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(UserName) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_Servizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_Attivita) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_Operazione) & "  ")
            StrSQL.AppendLine("         , 1  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(UserName) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(UserName) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function
    '##############################################################################################
    ''' <param name="Id_Attivita">Collezione di Id_Attivita da eliminare</param>
    ''' <param name="Id_Operazione">Impostare a 1 (enum_TipoPermesso.DISABILITATO) per evitare il filtro</param>
    Public Function Cancella_Cliente_Permessi(
        ByVal UserName As String,
        Id_Attivita As IEnumerable(Of Integer),
        Id_Operazione As enum_TipoPermesso,
        xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As Boolean
        Const nomeRoutine = "AnagrafeCoreUtentiDAL.Utenti_Permessi_W.CancellaCliente_Permessi()"
        Dim StrSQL As New StringBuilder With {.Length = 0}
        Dim xRisp As Boolean

        If Id_Attivita.Count > 0 AndAlso Id_Attivita.Any(Function(perm) perm = enum_Security_Attivita.Gest_Menu) Then
            Throw New GiasException("Impossibile cancellare il permesso 'Accesso al menu principale' per il superuser!")
        End If

        Dim xIn As String = Id_Attivita.Select(Function(id) id.ToString).
            Aggregate(Function(acc, id) acc & "," & id)

        Try
            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM  Cliente_Permessi ")
            StrSQL.AppendLine(" WHERE UserName = '" & Agro_SQL_SaveText(UserName) & "'")
            StrSQL.AppendLine(" AND  PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            If Id_Operazione <> enum_TipoPermesso.DISABILITATO Then
                StrSQL.AppendLine(" AND  Id_Operazione = " & Agro_SQL_SaveNum(Id_Operazione))
            End If
            If Id_Attivita.Any Then
                StrSQL.AppendLine(" AND Id_Attivita in (" & Agro_SQL_Save_Clausola_IN(xIn) & ") ")
            End If
            If Not String.IsNullOrWhiteSpace(xFiltroAggiuntivo) Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
        Return xRisp
    End Function
    '##############################################################################################
    'ATTENZIONE!!!
    ' se vengono passate le date 01/01/1900 e 31/12/2100 i valori non verranno modificati!!!
    Public Function Modifica_Validita(ByVal UserName As String,
                                      ByVal Id_Servizio As Integer,
                                      ByVal Validita_Inizio As Date,
                                      ByVal Validita_Fine As Date,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As Boolean

        Const nomeRoutine = "AnagrafeCoreUtentiDAL.Utenti_Permessi_W.Modifica_Validita()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE Utenti_Permessi SET ")
            StrSQL.Append("       Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("       ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            If Validita_Inizio <> AGRODATAINIZIO Then
                StrSQL.Append("       ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            End If
            If Validita_Fine <> AGRODATAFINE Then
                StrSQL.Append("       ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            End If

            StrSQL.Append(" WHERE UserName = '" & Agro_SQL_SaveText(UserName) & "' ")
            StrSQL.Append(" AND   Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & " ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    '##############################################################################################
    'Blocco Id_Operazione per Gestione Validità Chiave  
    Public Function Blocca_Permessi(ByVal Id_Servizio As Integer,
                                    ByVal Err_Log As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        Const nomeRoutine = "AnagrafeCoreUtentiDAL.Utenti_Permessi_W.Blocca_Permessi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE Utenti_Permessi SET ")
            StrSQL.Append("       Id_Operazione =  9 ")

            StrSQL.Append(" WHERE UserName <> '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
            StrSQL.Append(" AND   Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & " ")
            StrSQL.Append(" AND   Id_Operazione = 2 ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("UPDATE Utenti_Impostazioni SET ")
            StrSQL.Append("       Impostazione_Valore_2 =  1 ")
            StrSQL.Append("      ,Impostazione_Valore_3 =  '" & Agro_SQL_SaveText(Err_Log) & "' ")

            StrSQL.Append(" WHERE UserName = '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
            StrSQL.Append(" AND   Impostazione_Cod = " & enum_Impostazioni_Utenti.SUPERUSER_Licenza_Giorni_Franchigia & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    '##############################################################################################
    'Ripristino Id_Operazione per Gestione Validità Chiave  
    Public Function Ripristino_Permessi(ByVal Id_Servizio As Integer,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Boolean

        Const nomeRoutine = "AnagrafeCoreUtentiDAL.Utenti_Permessi_W.Ripristino_Permessi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE Utenti_Permessi SET ")
            StrSQL.Append("       Id_Operazione =  2 ")

            StrSQL.Append(" WHERE UserName <> '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
            StrSQL.Append(" AND   Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & " ")
            StrSQL.Append(" AND   Id_Operazione = 9 ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("UPDATE Utenti_Impostazioni SET ")
            StrSQL.Append("       Impostazione_Valore_2 =  0 ")

            StrSQL.Append(" WHERE UserName = '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
            StrSQL.Append(" AND   Impostazione_Cod = " & enum_Impostazioni_Utenti.SUPERUSER_Licenza_Giorni_Franchigia & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    ''' <summary>
    ''' Scrive i permessi di uno o più utenti copiandoli da quelli posseduti dalla sua tipologia.
    ''' </summary>
    ''' <param name="username">Username o lista di username degli utenti su cui eseguire l'operazione. Formato per la lista di username: <tt>'user1', 'user2', 'user3', ...</tt> </param>
    ''' <param name="xFiltroAggiuntivo">Filtro eseguibile sulla tabella utenti</param>
    ''' <remarks>
    ''' Errori noti:
    ''' - Tira errore se la tipologia a cui è collegato l'utente non possiede permessi
    ''' </remarks>
    Public Function ScriviDaTipologia(username As String,
                                      xFiltroAggiuntivo As String,
                                      objParametri As AgronicaCoreParametri,
                                      Optional gestisciInTransazione As Boolean = True)

        Const nomeRoutine = "AnagrafeCoreUtentiDAL.Utenti_Permessi_W.ScriviDaTipologia()"

        Dim StrSQL As New StringBuilder With {.Length = 0}
        Dim xRisp As Boolean

        If Not String.IsNullOrWhiteSpace(username) Then
            username = username.Trim
            If Not username.StartsWith("'") OrElse username.Contains(")") Then
                username = Agro_SQL_SaveText(username)
            End If
        Else
            Throw New ArgumentNullException("Nessun username specificato")
        End If
        Try
            If gestisciInTransazione Then
                ConnessioniTransazioni.ApriConnessione(gestisciInTransazione, objParametri)
            End If

            ' Creo le tabelle temporanee
            StrSQL.AppendLine(" ; WITH Utenti_Da_Processare AS ( ")
            StrSQL.AppendLine("   SELECT UserName FROM Utenti ")
            StrSQL.AppendLine("   WHERE UserName IN ")
            StrSQL.AppendLine("   ( ")
            StrSQL.AppendLine(Agro_SQL_Save_Clausola_IN(username, True))
            StrSQL.AppendLine("   ) ")
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine("   AND " & Agro_SQL_SaveText(xFiltroAggiuntivo))
            End If
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" select * into #utenti ")
            StrSQL.AppendLine(" from Utenti_Da_Processare ")
            StrSQL.AppendLine()
            'Leggo i permessi associati alla nuova tipologia
            StrSQL.AppendLine(" ; WITH utp AS ( ")
            StrSQL.AppendLine("   SELECT DISTINCT UserName, Validita_Inizio, Validita_Fine FROM Utenti_Permessi ")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("   utenti.UserName, Utenti_TipologiexPermessi.Id_Servizio, Utenti_TipologiexPermessi.Id_Attivita, Utenti_TipologiexPermessi.Id_Operazione, ")
            StrSQL.AppendLine("   1 as ID, 0 as Inizio_Ore, 0 as Inizio_Minuti, 23 as Fine_Ore, 59 as Fine_Minuti, 0 as inviato, ")
            StrSQL.AppendLine("   utenti.Tipologia_Cod , ")
            StrSQL.AppendLine("   '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' as Username_Creazione, ")
            StrSQL.AppendLine("   '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' as Username_Modifica, ")
            StrSQL.AppendLine("   coalesce(utp.Validita_Inizio, CAST('01/01/1900' as date)) as Validita_Inizio, ") ' Sostituire con date validità profilo?
            StrSQL.AppendLine("   coalesce(utp.Validita_Fine , CAST('31/12/2100' as date)) as Validita_Fine ")     ' Sostituire con date validità profilo?
            StrSQL.AppendLine(" INTO #Utenti_Permessi ")
            StrSQL.AppendLine(" FROM Utenti ")
            StrSQL.AppendLine("   LEFT JOIN Utenti_TipologiexPermessi ON Utenti.Tipologia_Cod = Utenti_TipologiexPermessi.Tipologia_Cod ")
            StrSQL.AppendLine("   LEFT JOIN utp ON utp.UserName = Utenti.UserName ")
            StrSQL.AppendLine(" WHERE utenti.UserName in ")
            StrSQL.AppendLine(" (    select UserName from #utenti    ) ")
            StrSQL.AppendLine()
            ' Cancello vecchi record in Utenti_Permessi
            StrSQL.AppendLine(" DELETE FROM Utenti_Permessi WHERE UserName in ")
            StrSQL.AppendLine(" (    SELECT UserName FROM #utenti    ) ")
            StrSQL.AppendLine()
            ' Inserisco nuovi record in Utenti_Permessi
            StrSQL.AppendLine(" IF EXISTS (SELECT TOP(1) * FROM #Utenti_Permessi WHERE Id_Operazione >= 0) ")
            StrSQL.AppendLine(" BEGIN ")
            StrSQL.AppendLine(" INSERT INTO Utenti_Permessi ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine("     UserName, Id_Servizio, Id_Attivita, Id_Operazione, ID, ")
            StrSQL.AppendLine("     Inizio_Ore, Inizio_Minuti, Fine_Ore, Fine_Minuti, inviato, ")
            StrSQL.AppendLine("     Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine, Tipologia_Cod ")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("     UserName, Id_Servizio, Id_Attivita, Id_Operazione, ID, ")
            StrSQL.AppendLine("     Inizio_Ore, Inizio_Minuti, Fine_Ore, Fine_Minuti, inviato, ")
            StrSQL.AppendLine("     Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine, Tipologia_Cod ")
            StrSQL.AppendLine(" FROM #Utenti_Permessi ")
            StrSQL.AppendLine(" END ")
            StrSQL.AppendLine()

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
            If gestisciInTransazione Then
                ConnessioniTransazioni.ChiudiTransazione(1, objParametri)
                ConnessioniTransazioni.ChiudiConnessione(objParametri)
            End If
        Catch ex As Exception
            If gestisciInTransazione Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
                ConnessioniTransazioni.ChiudiConnessione(objParametri)
            End If
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp
    End Function

    ''' <summary>
    ''' Scrive i permessi di uno o più utenti copiandoli da quelli posseduti dalla sua tipologia.
    ''' </summary>
    ''' <param name="xFiltroAggiuntivo">Filtro eseguibile sulla tabella utenti</param>
    ''' <remarks>
    ''' Errori noti:
    ''' - Tira errore se la tipologia a cui è collegato l'utente non possiede permessi
    ''' </remarks>
    Public Function ScriviDaTipologiaMassivo(
        users As IEnumerable(Of AgronicaCoreModelsSTD.profilazione.IUtente),
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri,
        Optional gestisciInTransazione As Boolean = True
    )

        Const nomeRoutine = "AnagrafeCoreUtentiDAL.Utenti_Permessi_W.ScriviDaTipologia()"

        Dim StrSQL As New StringBuilder With {.Length = 0}
        Dim xRisp As Boolean

        If users.Count = 0 Then
            Return True
        End If
        Dim prepareFilter = Function(acc, u)
                                If u.index Mod 100 = 0 Then
                                    acc.name &= ", --" & u.index & vbNewLine & u.name
                                ElseIf u.index Mod 10 = 0 Then
                                    acc.name &= ", " & vbNewLine & u.name
                                Else
                                    acc.name &= ", " & u.name
                                End If
                                Return acc
                            End Function
        Dim usersFilter = users.AsParallel.
            Select(Function(u, i) New With {.name = "'" & u.UserName & "'", .index = i}).
            Aggregate(prepareFilter).name
        Try
            If gestisciInTransazione Then
                ConnessioniTransazioni.ApriConnessione(gestisciInTransazione, objParametri)
            End If

            ' Creo le tabelle temporanee
            StrSQL.AppendLine(" ; WITH Utenti_Da_Processare AS ( ")
            StrSQL.AppendLine("   SELECT UserName FROM Utenti ")
            StrSQL.AppendLine("   WHERE UserName IN ")
            StrSQL.AppendLine("   ( " & usersFilter & " ) ")
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine("   AND " & Agro_SQL_SaveText(xFiltroAggiuntivo))
            End If
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" select * into #utenti ")
            StrSQL.AppendLine(" from Utenti_Da_Processare ")
            StrSQL.AppendLine()
            'Leggo i permessi associati alla nuova tipologia
            StrSQL.AppendLine(" ; WITH utp AS ( ")
            StrSQL.AppendLine("   SELECT DISTINCT UserName, Validita_Inizio, Validita_Fine FROM Utenti_Permessi ")
            StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("   utenti.UserName, Utenti_TipologiexPermessi.Id_Servizio, Utenti_TipologiexPermessi.Id_Attivita, Utenti_TipologiexPermessi.Id_Operazione, ")
            StrSQL.AppendLine("   1 as ID, 0 as Inizio_Ore, 0 as Inizio_Minuti, 23 as Fine_Ore, 59 as Fine_Minuti, 0 as inviato, ")
            StrSQL.AppendLine("   utenti.Tipologia_Cod , ")
            StrSQL.AppendLine("   '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' as Username_Creazione, ")
            StrSQL.AppendLine("   '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' as Username_Modifica, ")
            StrSQL.AppendLine("   coalesce(utp.Validita_Inizio, CAST('01/01/1900' as date)) as Validita_Inizio, ") ' Sostituire con date validità profilo?
            StrSQL.AppendLine("   coalesce(utp.Validita_Fine , CAST('31/12/2100' as date)) as Validita_Fine ")     ' Sostituire con date validità profilo?
            StrSQL.AppendLine(" INTO #Utenti_Permessi ")
            StrSQL.AppendLine(" FROM Utenti ")
            StrSQL.AppendLine("   LEFT JOIN Utenti_TipologiexPermessi ON Utenti.Tipologia_Cod = Utenti_TipologiexPermessi.Tipologia_Cod ")
            StrSQL.AppendLine("   LEFT JOIN utp ON utp.UserName = Utenti.UserName ")
            StrSQL.AppendLine(" WHERE utenti.UserName in ")
            StrSQL.AppendLine(" (    select UserName from #utenti    ) ")
            StrSQL.AppendLine()
            ' Cancello vecchi record in Utenti_Permessi
            StrSQL.AppendLine(" DELETE FROM Utenti_Permessi WHERE UserName in ")
            StrSQL.AppendLine(" (    SELECT UserName FROM #utenti    ) ")
            StrSQL.AppendLine()
            ' Inserisco nuovi record in Utenti_Permessi
            StrSQL.AppendLine(" IF EXISTS (SELECT TOP(1) * FROM #Utenti_Permessi WHERE Id_Operazione >= 0) ")
            StrSQL.AppendLine(" BEGIN ")
            StrSQL.AppendLine(" INSERT INTO Utenti_Permessi ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine("     UserName, Id_Servizio, Id_Attivita, Id_Operazione, ID, ")
            StrSQL.AppendLine("     Inizio_Ore, Inizio_Minuti, Fine_Ore, Fine_Minuti, inviato, ")
            StrSQL.AppendLine("     Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine, Tipologia_Cod ")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" SELECT DISTINCT ")
            StrSQL.AppendLine("     UserName, Id_Servizio, Id_Attivita, Id_Operazione, ID, ")
            StrSQL.AppendLine("     Inizio_Ore, Inizio_Minuti, Fine_Ore, Fine_Minuti, inviato, ")
            StrSQL.AppendLine("     Username_Creazione, Username_Modifica, MAX(Validita_Inizio), MAX(Validita_Fine), Tipologia_Cod ")
            StrSQL.AppendLine(" FROM #Utenti_Permessi ")
            StrSQL.AppendLine(" GROUP BY ")
            StrSQL.AppendLine("     UserName, Id_Servizio, Id_Attivita, Id_Operazione, ID, ")
            StrSQL.AppendLine("     Inizio_Ore, Inizio_Minuti, Fine_Ore, Fine_Minuti, inviato, ")
            StrSQL.AppendLine("     Username_Creazione, Username_Modifica, Tipologia_Cod ")
            StrSQL.AppendLine(" END ")
            StrSQL.AppendLine()

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
            If gestisciInTransazione Then
                ConnessioniTransazioni.ChiudiTransazione(1, objParametri)
                ConnessioniTransazioni.ChiudiConnessione(objParametri)
            End If
        Catch ex As Exception
            If gestisciInTransazione Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
                ConnessioniTransazioni.ChiudiConnessione(objParametri)
            End If
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
        Return xRisp
    End Function

End Class
