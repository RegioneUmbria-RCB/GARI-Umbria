Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text
Imports AgronicaCoreModelsSTD.exceptions

Public Class Gruppi_UtenteXGruppi_Merce_R
    Inherits DataProvider

    Private _objParametri_Server As AgronicaCoreParametri
    Private _objParametri_Utenti As AgronicaCoreParametri
    Private _giasContext As Gias_DeveloperServer_Entities

    Public Sub New(ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri)
        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(_objParametri_Server.StringaConnessione)
        _giasContext = New Gias_DeveloperServer_Entities(EFConnString)
    End Sub

    Public Function LeggiPermessiVisibiliAllUtenteConnesso(piva As String) As DataTable
        If piva <> "" Then
            Return LeggiDatiAssociatiAllImpresaSelezionata(piva)
        Else
            Return LeggiDatiAssociatiAllUtenteConesso()
        End If
    End Function

    Private Function LeggiDatiAssociatiAllUtenteConesso()
        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Dim filtroVisibilitaUtente = Not objProfilo.HasFullVisibility(_objParametri_Utenti.UtenteUsername, _objParametri_Utenti)
        Return LeggiConDescrizioni("", "", filtroVisibilitaUtente)
    End Function

    Private Function LeggiDatiAssociatiAllImpresaSelezionata(piva As String) As DataTable
        Dim filtroAggiuntivo = "UtentexMerce.Piva = '" & piva & "'"

        Return LeggiConDescrizioni(filtroAggiuntivo, "", False)
    End Function


    Private Function LeggiConDescrizioni(FiltroAggiuntivo As String,
                                        OrderBy As String,
                                        visibilitaUtenti As Boolean
                                        ) As DataTable

        Dim nomeRoutine = Reflection.MethodBase.GetCurrentMethod().Name
        Dim stb As New Text.StringBuilder
        Dim dt As DataTable
        Try

            Dim NomeDB_Utenti = _objParametri_Utenti.Recupera_NomeDB()

            stb.AppendLine("SELECT merce.Descrizione DescrizioneGruppoMerce, merce.Codice CodiceGruppoMerce, utenti.Gruppi_Utente_des, utenti.Gruppi_Utente_cod, ")
            stb.AppendLine("	   UtentexMerce.Id_Gruppo_Merce, UtentexMerce.Piva, Imprese.rag_soc")
            stb.AppendLine("FROM   Gruppi_UtenteXGruppi_Merce UtentexMerce")
            stb.AppendLine("INNER JOIN Gruppi_Merce merce on UtentexMerce.Id_Gruppo_Merce = merce.Id_Gruppo_Merce")
            stb.AppendLine("INNER JOIN " & NomeDB_Utenti & ".dbo.Gruppi_Utente utenti on utenti.Gruppi_Utente_cod = UtentexMerce.Gruppo_Utente")
            stb.AppendLine("INNER JOIN Imprese on UtentexMerce.piva = imprese.PIVA")

            If visibilitaUtenti Then
                stb.AppendLine("INNER JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON Imprese.Piva = Utenti_Visibilita_Appoggio.Piva 
									       AND Utenti_Visibilita_Appoggio.Entita_Cod = 1 
										   AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(_objParametri_Server.UtenteUsername) & "")
            End If

            stb.AppendLine("WHERE 1=1 ")

            If FiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo, , _objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            Select Case _objParametri_Server.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine("AND   UtentexMerce.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine("AND   UtentexMerce.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If OrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(OrderBy, _objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(_objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Scrivi_LOG(_objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    '##############################################################################################
    Public Function Leggi(ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String
                          ) As DataTable

        '----- Descrizione
        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Gruppi_UtenteXGruppi_Merce_R.Leggi()"

        '----- Variabili
        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Gruppi_UtenteXGruppi_Merce ")
            stb.AppendLine(" WHERE   1=1 ")

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , _objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            Select Case _objParametri_Server.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, _objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(_objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(_objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' <summary>
    ''' Verifica che tutti i dettagli del documento siano appartenenti a gruppi merce sui quali i gruppi di cui fa parte l'utente corrente hanno visibilità
    ''' </summary>
    ''' <param name="gruppiUtente"></param>
    ''' <param name="piva"></param>
    ''' <param name="idAgenda"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <returns></returns>
    Public Function DettagliDocumentoVisibili_X_GruppiUtente_X_GruppiMerce(ByVal piva As String, ByVal idAgenda As Integer, ByVal xFiltroAggiuntivo As String) As Boolean
        '----- Descrizione
        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Gruppi_UtenteXGruppi_Merce_R.DettagliDocumentoVisibili_X_GruppiUtente_X_GruppiMerce()"

        '----- Variabili
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Dim tuttiDettagliVisibili As Boolean

        Try

            If IsNothing(_objParametri_Utenti) Then
                Throw New GiasException("Impossibile determinare gruppo utente: Contattare assistenza.")
            End If

            Dim objGruppiMerce As New Gruppi_Merce_R

            StrSQL.Length = 0

            Dim sqlCreaTempDefault = objGruppiMerce.ComponiSql_CreaTempDefaultGruppiMerce(New List(Of String) From {piva}, _objParametri_Server, Nothing)
            StrSQL.AppendLine(sqlCreaTempDefault.ToString())
            StrSQL.AppendLine("")
            StrSQL.AppendLine(ComponiSql_ContaDettagli_X_GruppoUtente("", piva, idAgenda, False, xFiltroAggiuntivo))
            Dim sqlCancTempDefault = objGruppiMerce.ComponiSql_CancellaTempDefaultGruppiMerce()
            StrSQL.AppendLine("")
            StrSQL.AppendLine(sqlCancTempDefault.ToString())

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(_objParametri_Server, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            Dim numDettagliNonVisibili As Integer = dt.Rows(0).Item(0)

            tuttiDettagliVisibili = If(numDettagliNonVisibili = 0, True, False)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(_objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return tuttiDettagliVisibili

    End Function

    ''' <summary>
    ''' Crea una query "select count" dei dettagli di un documento che appartengono o no a gruppi merce che l'utente corrente ha in visibilità (vengono considerati i gruppi utente di quest'ultimo).
    ''' La query presuppone che sia stata creata la tabella temporanea dei default dei gruppi merce per la piva passata "#DefaultGruppiMerce".
    ''' </summary>
    ''' <param name="aliasTabAgenda">Se valorizzato, la query presuppone di essere usata come subquery ed utilizza questo alias per la tabella "Agenda" per effettuare la join con la tabella Movimenti_Dettagli. 
    ''' In questo caso vengono ignorati i parametri <paramref name="idAgenda"/> e <paramref name="piva"/>. Se non valorizzato, la query filtra i dati di un solo documento identificato dalla coppia di parametri <paramref name="piva"/> e <paramref name="idAgenda"/></param>
    ''' <param name="piva">Obbligatorio se il parametro <paramref name="aliasTabAgenda"/> è passato a vuoto, Piva dell'azienda a cui appartengono i documenti</param>
    ''' <param name="idAgenda">Obbligatorio se il parametro <paramref name="aliasTabAgenda"/> è passato a vuoto, altrimenti non viene considerato</param>
    ''' <param name="contaVisibili">Se true, il conteggio restituisce il numero di dettagli visibili all'utente, se false invece, il numero di dettagli non visualizzabili</param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <returns></returns>
    Public Function ComponiSql_ContaDettagli_X_GruppoUtente(ByVal aliasTabAgenda As String, ByVal piva As String, ByVal idAgenda As Integer, ByVal contaVisibili As Boolean, ByVal xFiltroAggiuntivo As String) As String

        Dim StrSQL As New StringBuilder

        StrSQL.AppendLine(" SELECT COUNT(*)")
        StrSQL.AppendLine(" FROM Movimenti_dettagli mov_det ")
        StrSQL.AppendLine(" LEFT JOIN Prodotti_Extra_Privata as prodExtraPriv WITH (nolock)")
        StrSQL.AppendLine("  ON prodExtraPriv.Piva_SuperUser = '" & _objParametri_Server.PivaSuperUser & "'")
        StrSQL.AppendLine("  AND prodExtraPriv.Elem_Cod = mov_det.Elem_Cod")
        StrSQL.AppendLine("  AND prodExtraPriv.Mat_Cod = mov_det.Mat_Cod")
        StrSQL.AppendLine("  AND prodExtraPriv.Pro_Cod = mov_det.Pro_Cod")
        StrSQL.AppendLine("  AND (prodExtraPriv.Piva = mov_det.Piva OR mov_det.Pro_Cod = 0)")
        StrSQL.AppendLine(" LEFT JOIN #DefaultGruppiMerce")
        StrSQL.AppendLine("  ON #DefaultGruppiMerce.Elem_Cod = mov_det.Elem_Cod ")
        StrSQL.Append(" WHERE ")

        If Not String.IsNullOrEmpty(aliasTabAgenda) Then
            StrSQL.AppendLine(String.Format("mov_det.Piva = {0}.Piva AND mov_det.Id_Agenda = {0}.Id_Agenda", aliasTabAgenda))
        Else
            StrSQL.AppendLine("mov_det.Piva = '" & Agro_SQL_SaveText(piva) & "' AND mov_det.Id_Agenda = " & Agro_SQL_SaveNum(idAgenda))
            'StrSQL.AppendLine(" AND  mov_det.Piva = '" & Agro_SQL_SaveText(piva) & "' AND mov_det.Id_Agenda = " & Agro_SQL_SaveNum(idAgenda))
        End If

        StrSQL.AppendLine(" AND COALESCE(prodExtraPriv.Id_Gruppo_Merce, #DefaultGruppiMerce.Id_Gruppo_Merce, 0) " & If(contaVisibili = False, "NOT", "") & " IN (")
        StrSQL.AppendLine(ComponiSql_DistinctGruppiMerce_X_GruppiUtente(aliasTabAgenda, piva))
        StrSQL.AppendLine(" )")

        If xFiltroAggiuntivo <> "" Then
            StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
            'StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , _objParametri_Server))
        End If

        Return StrSQL.ToString

    End Function

    ''' <summary>
    ''' Crea una query che restituisce l'elenco dei gruppi merce configurati per i gruppi utente a cui appartiene l'utente corrente.
    ''' </summary>
    ''' <param name="aliasTabAgenda">Se valorizzato, la query presuppone di essere usata come subquery ed utilizza questo alias per la tabella "Agenda" per filtrare l'impresa in Gruppi_UtenteXGruppi_Merce.
    ''' In questo caso viene ignorato il parametro <paramref name="piva"/></param>
    ''' <param name="piva">Se il parametro <paramref name="aliasTabAgenda"/> non è valorizzato ottengo i gruppi merce associati a questa impresa</param>
    ''' <returns></returns>
    Public Function ComponiSql_DistinctGruppiMerce_X_GruppiUtente(ByVal aliasTabAgenda As String, ByVal piva As String) As String

        Dim StrSQL As New StringBuilder

        Dim handleGruppiUtente As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

        Dim gruppiUtente = handleGruppiUtente.LeggiJoinGruppi(_objParametri_Utenti.UtenteUsername, 0, "", "", _objParametri_Utenti)
        Dim arrGruppiUtente = gruppiUtente.AsEnumerable().Select(Of Integer)(Function(gruppo) gruppo("Gruppi_Utente_Cod"))

        StrSQL.AppendLine("   SELECT DISTINCT Id_Gruppo_Merce")
        StrSQL.AppendLine("   FROM Gruppi_UtenteXGruppi_Merce WITH (nolock)")
        StrSQL.AppendLine("   WHERE Gruppi_UtenteXGruppi_Merce.Piva_SuperUser = '" & _objParametri_Utenti.PivaSuperUser & "'")
        If Not String.IsNullOrEmpty(aliasTabAgenda) Then
            StrSQL.AppendLine(String.Format("   AND Gruppi_UtenteXGruppi_Merce.Piva = {0}.Piva", aliasTabAgenda))
        Else
            StrSQL.AppendLine("   AND Gruppi_UtenteXGruppi_Merce.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            'StrSQL.AppendLine("   AND Gruppi_UtenteXGruppi_Merce.Piva = '" & Agro_SQL_SaveText(piva) & "'")
        End If

        StrSQL.AppendLine("   AND Gruppi_UtenteXGruppi_Merce.Gruppo_Utente IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrGruppiUtente)) & ")")
        'StrSQL.AppendLine("   AND Gruppi_UtenteXGruppi_Merce.Gruppo_Utente IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrGruppiUtente), False) & ")")

        '--------------------------------------------------------------------------
        Select Case _objParametri_Server.FlagVisibilita
            Case enumVisibilita.Visibilita_SoloNonCancellati
                StrSQL.AppendLine("   AND Gruppi_UtenteXGruppi_Merce.Inviato >=0 ")
            Case enumVisibilita.Visibilita_SoloCancellati
                StrSQL.AppendLine("   AND Gruppi_UtenteXGruppi_Merce.Inviato =-1 ")
            Case enumVisibilita.Visibilita_Tutti
                '...................................
            Case Else
                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
        End Select
        '--------------------------------------------------------------------------

        Return StrSQL.ToString

    End Function

End Class


'#################################################################
'#################################################################
'#################################################################

Public Class Gruppi_UtenteXGruppi_Merce_W
    Inherits DataProvider


    Private _objParametri_Server As AgronicaCoreParametri
    Private _objParametri_Utenti As AgronicaCoreParametri
    Private _giasContext As Gias_DeveloperServer_Entities

    Public Sub New(ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri)
        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(_objParametri_Server.StringaConnessione)
        _giasContext = New Gias_DeveloperServer_Entities(EFConnString)
    End Sub

    Public Function Scrivi(Gruppi_Utente_cod As Integer, Id_Gruppo_Merce As Integer, piva As String) As Boolean

        Dim toAdd As New AgronicaCoreEntityFramework_POCO.Gruppi_UtenteXGruppi_Merce
        Dim now = DateTime.Now

        toAdd.datainvio = Nothing
        toAdd.Data_Creazione = now
        toAdd.Data_Modifica = now
        toAdd.Gruppo_Utente = Gruppi_Utente_cod
        toAdd.Id_Gruppo_Merce = Id_Gruppo_Merce
        toAdd.inviato = 0
        toAdd.Piva = piva
        toAdd.Piva_SuperUser = _objParametri_Server.PivaSuperUser
        toAdd.Username_Creazione = _objParametri_Server.UtenteUsername
        toAdd.Username_Modifica = _objParametri_Server.UtenteUsername
        toAdd.Validita_Fine = AGRODATAFINE
        toAdd.Validita_Inizio = AGRODATAINIZIO

        _giasContext.Gruppi_UtenteXGruppi_Merce.Add(toAdd)
        _giasContext.SaveChanges()

        Return True
    End Function

    Public Function RecordExists(gruppoUtenteCod As Integer, gruppoMerceId As Integer, piva As String) As Boolean
        Return _giasContext.Gruppi_UtenteXGruppi_Merce.Any(Function(um) um.Piva_SuperUser = _objParametri_Server.PivaSuperUser AndAlso um.Gruppo_Utente = gruppoUtenteCod AndAlso um.Id_Gruppo_Merce = gruppoMerceId AndAlso um.Piva = piva)
    End Function



    '##############################################################################################
    Public Function Scrivi(Optional ByVal Data_creazione As DateTime = #2/1/1900# _
                          , Optional ByVal Data_modifica As DateTime = #2/1/1900# _
                          , Optional ByVal username_creazione As String = "" _
                          , Optional ByVal username_modifica As String = ""
                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Gruppi_UtenteXGruppi_Merce_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = _objParametri_Server.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = _objParametri_Server.UsernameOperazione
            End If



            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT Gruppi_UtenteXGruppi_Merce ")

            stb.AppendLine("              (")
            stb.AppendLine("              Inviato,            datainvio, ")
            stb.AppendLine("              Data_Creazione,     Data_Modifica, ")
            stb.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            stb.AppendLine("              Validita_Inizio,    Validita_Fine, ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")



            stb.AppendLine("         , 0  ")
            stb.AppendLine("         , Null  ")

            stb.AppendLine("            , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            stb.AppendLine("            , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            stb.AppendLine("            ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            stb.AppendLine("            ,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(_objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(_objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function



    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String) As Boolean

        '----- Descrizione
        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Gruppi_UtenteXGruppi_Merce_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            stb.Length = 0

            '---------------------------------------------
            If _objParametri_Server.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                stb.AppendLine(" UPDATE Gruppi_UtenteXGruppi_Merce ")
                stb.AppendLine(" SET ")
                stb.AppendLine("         Username_Modifica = '" & Agro_SQL_SaveText(_objParametri_Server.UsernameOperazione) & "' ")
                stb.AppendLine("         ,Data_Modifica= " & Agro_SQL_SaveDateTime(Date.Now) & " ")
                stb.AppendLine("         ,Inviato = -1 ")
                stb.AppendLine(" WHERE   1=1 ")
                stb.AppendLine(" AND     Inviato >= 0 ")
            Else
                stb.AppendLine(" DELETE FROM Gruppi_UtenteXGruppi_Merce ")
                stb.AppendLine(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , _objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(_objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(_objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Sub Cancella(gruppoUtente As Integer, Id_Gruppo_Merce As Integer, piva As String)
        Dim permesso = _giasContext.Gruppi_UtenteXGruppi_Merce.Where(
            Function(entity As Gruppi_UtenteXGruppi_Merce) entity.Gruppo_Utente = gruppoUtente AndAlso entity.Id_Gruppo_Merce = Id_Gruppo_Merce AndAlso entity.Piva = piva AndAlso entity.Piva_SuperUser = _objParametri_Server.PivaSuperUser).FirstOrDefault()

        If permesso Is Nothing Then
            Throw New Exception("Non è stato trovato nessun elemento da cancellare: ") ' nessun elemento da eliminare
        End If
        _giasContext.Gruppi_UtenteXGruppi_Merce.Remove(permesso)
        _giasContext.SaveChanges()
    End Sub
End Class
