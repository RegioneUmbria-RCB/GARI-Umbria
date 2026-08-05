Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider
Imports System.Text

''' <summary>
''' DAL di sola lettura per la tabella <c>Utenti_Profili_Pratiche</c>.
''' Usata dall'orchestrator combinato gerarchia+pratiche (DS01-BL) durante il login
''' per recuperare le pratiche selezionate dal profilo utente.
''' Equivalente VB di <c>AgronicaNetCore.Utenti.DAL.UtentiProfiliPratiche.LeggiAsync</c>.
''' </summary>
Public Class Utenti_Profili_Pratiche
    Inherits AgronicaCoreDataProvider.DataProvider

        Private Function StringListAggregator(utenti As IEnumerable(Of String), mettiApici As Boolean) As List(Of String)
        Dim prepareFilter = Function(acc, u)
                                If u.index Mod 10 = 0 Then
                                    acc.name &= ", " & vbNewLine & u.name
                                Else
                                    acc.name &= ", " & u.name
                                End If
                                Return acc
                            End Function
        Dim uu = utenti.AsParallel.
            DefaultIfEmpty(String.Empty).
            Select(Function(str) If(mettiApici, "'" & str & "'", str)).
            Select(Function(str, i) New With {.name = str, .index = i}).
            GroupBy(Function(u) u.index \ 10_000).
            Select(Function(batch) batch.Aggregate(prepareFilter).name).
            ToList ' lista di stringhe aggregate
        Return uu
    End Function

    ''' <summary>
    ''' Restituisce <c>(Servizio_Cod, Considera_Validita_Temporale)</c> per l'utente specificato.
    ''' </summary>
    Public Function Leggi(
        ByVal idUtente As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Profili_Pratiche.Leggi()"
        Dim StrSQL As New StringBuilder
        Try
            StrSQL.AppendLine(" SELECT Servizio_Cod, Considera_Validita_Temporale ")
            StrSQL.AppendLine(" FROM   Utenti_Profili_Pratiche (NOLOCK) ")
            StrSQL.AppendLine(" WHERE  IdUtente = '" & Agro_SQL_SaveText(idUtente) & "' ")
            Return EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function Copia(
        template As String, targets As IEnumerable(Of String),
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Profili_Pratiche.Copia()"
        Dim conApici = targets.Count > 1
        Dim xIn = StringListAggregator(targets, conApici).FirstOrDefault
        Dim StrSQL As New StringBuilder        
        Try
            StrSQL.AppendLine($"DELETE FROM utenti_profili_pratiche")
            StrSQL.AppendLine($"WHERE IdUtente IN ({Agro_SQL_Save_Clausola_IN(xIn, valoriStringa:=Not conApici, creaParametriSql:=Not conApici)})")
            EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            StrSQL.Clear()
            StrSQL.AppendLine("INSERT INTO utenti_profili_pratiche")
            StrSQL.AppendLine("  (IdUtente, Servizio_Cod, Considera_Validita_Temporale, Data_Creazione, Data_Modifica)")
            StrSQL.AppendLine("SELECT")
            StrSQL.AppendLine("  Utenti.UserName, TEMPLATE.Servizio_Cod, TEMPLATE.Considera_Validita_Temporale, GETDATE(), GETDATE()")
            StrSQL.AppendLine("FROM utenti_profili_pratiche TEMPLATE, Utenti")
            StrSQL.AppendLine($"WHERE TEMPLATE.IdUtente = '{Agro_SQL_SaveText(template)}'")
            StrSQL.AppendLine($"  AND Utenti.UserName IN ({Agro_SQL_Save_Clausola_IN(xIn, valoriStringa:=Not conApici, creaParametriSql:=Not conApici)})")
            Return EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function LeggiImpreseCentriDaPratiche(username As String, objParametri_utenti As AgronicaCoreParametri, objParametri_server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Profili_Pratiche.LeggiImpreseDaPratiche()"
        Dim StrSQL As New StringBuilder
        Try
            Dim pars As New Dictionary(Of String, Object) From {
                {"@username", username}
            }

            StrSQL.AppendLine(" SELECT DISTINCT Centri_Aziendali.PIVA, COALESCE(Centri_Aziendali.sa_cod, 0) AS Sa_Cod")
            StrSQL.AppendLine($" FROM   [{objParametri_utenti.Recupera_NomeDB()}].[dbo].[utenti_profili_pratiche] utentiPratiche (NOLOCK) ")
            StrSQL.AppendLine(" LEFT JOIN Pratiche ON Pratiche.Servizio_Cod = utentiPratiche.Servizio_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali ON Centri_Aziendali.Piva = Pratiche.Piva ")
            StrSQL.AppendLine(" WHERE  utentiPratiche.IdUtente = @username ")
            StrSQL.AppendLine("   AND  Pratiche.Piva IS NOT NULL")
            StrSQL.AppendLine("   AND Centri_Aziendali.Validita_Inizio <= GETDATE() AND Centri_Aziendali.Validita_Fine >= GETDATE() ")
            StrSQL.AppendLine("   AND  (")
            StrSQL.AppendLine("         utentiPratiche.Considera_Validita_Temporale = 0 ")
            StrSQL.AppendLine("         OR  ( Pratiche.Validita_Inizio <= GETDATE() AND Pratiche.Validita_Fine >= GETDATE() )")
            StrSQL.AppendLine("   )")

            Return EseguiQuery_Lettura(objParametri_server, StrSQL.ToString(), pars, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri_server, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Invalida la cache di visibilita' appoggio (DataUltimoRiportoUtentiVisibilitaAppoggio = NULL)
    ''' per tutti gli utenti che hanno un filtro pratiche sul servizio indicato, cosi' che il
    ''' sincronizzatore batch della visibilita' li rielabori al ciclo successivo.
    ''' Sostituisce il trigger DB <c>tr_up_cascade_update</c>: va invocata dai writer di Pratiche
    ''' (in inserimento/eliminazione) passando i parametri dell'UtentiDB.
    ''' Mirror in codice del pattern gia' usato per gerarchia/centri
    ''' (GerarchiaImprese_W.AggiornaUtentiProfili, CentriAziendali).
    ''' Non solleva eccezioni: un fallimento dell'invalidazione non deve interrompere la
    ''' scrittura della pratica (la cache si riallinea comunque al login interattivo).
    ''' </summary>
    Public Function InvalidaVisibilitaAppoggioXServizio(
        ByVal Servizio_Cod As Integer,
        ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Profili_Pratiche.InvalidaVisibilitaAppoggioXServizio()"
        Dim StrSQL As New StringBuilder
        Try
            StrSQL.AppendLine(" UPDATE up WITH (ROWLOCK) SET ")
            StrSQL.AppendLine("    up.DataUltimoRiportoUtentiVisibilitaAppoggio = NULL ")
            StrSQL.AppendLine(" FROM Utenti_Profili up ")
            StrSQL.AppendLine(" INNER JOIN Utenti_Profili_Pratiche upp ON up.Utente = upp.IdUtente ")
            StrSQL.AppendLine(" WHERE upp.Servizio_Cod = " & Servizio_Cod & " ")
            StrSQL.AppendLine("   AND up.DataUltimoRiportoUtentiVisibilitaAppoggio IS NOT NULL ")
            Return EseguiQuery_Scrittura(objParametri_utenti, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri_utenti, NomeRoutine, ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Cancella tutti i filtri pratiche associati all'utente indicato.
    ''' Sostituisce la FK <c>FK_upp_IdUtente ... ON DELETE CASCADE</c>: va invocata alla
    ''' cancellazione del profilo/utente per ripulire <c>utenti_profili_pratiche</c>.
    ''' </summary>
    Public Function EliminaPerUtente(
        ByVal idUtente As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Profili_Pratiche.EliminaPerUtente()"
        Dim StrSQL As New StringBuilder
        Try
            StrSQL.AppendLine(" DELETE FROM Utenti_Profili_Pratiche ")
            StrSQL.AppendLine(" WHERE IdUtente = '" & Agro_SQL_SaveText(idUtente) & "' ")
            Return EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

End Class
