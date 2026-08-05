Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.DataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class SDI_Log_Helper

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _logR As SDI_Log_R
    Private ReadOnly _lowW As SDI_Log_W
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)

        Dim giasContext As New Gias_DeveloperServer_Entities(efConnString)

        _logR = New SDI_Log_R(giasContext)
        _lowW = New SDI_Log_W(giasContext)

    End Sub

    Public Function ToString(ByVal entita As SDI_Log) As String
        Return String.Format("Id_Agenda {0} - Numero Documento {1} - Nome file XML {2}", entita.Id_Agenda, NumeroDocumento(entita), entita.NomeFileXML)
    End Function
    Private Function NumeroDocumento(ByVal entita As SDI_Log) As String
        Return String.Format("{0}{1}{2}", entita.Doc_Numero_Sin, entita.Doc_Numero, entita.Doc_Numero_Des)
    End Function

    Public Sub LoggaXmlNonGeneratoPerErrori(
        ByVal tipoDocumento As String,
        ByVal tipoTracciato As String,
        ByVal errorMessage As String, ByVal log As SDI_Log)
        _lowW.LoggaXmlNonGenerato(tipoDocumento, tipoTracciato, {errorMessage}.ToList(), StatoFattura_Gias.XMLNonGeneratoPerErrori, log)
    End Sub

    Public Sub LoggaXmlNonGenerabile(
        ByVal tipoDocumento As String,
        ByVal tipoTracciato As String,
        ByVal listaErrori As List(Of String), ByVal log As SDI_Log)
        _lowW.LoggaXmlNonGenerato(tipoDocumento, tipoTracciato, listaErrori, StatoFattura_Gias.XMLNonGenerabile, log)
    End Sub

    Public Sub Elimina(ByVal listaEntita As List(Of SDI_Log))
        If listaEntita Is Nothing OrElse Not listaEntita.Any Then Return
        listaEntita.ForEach(Sub(e) _lowW.Elimina(e))
    End Sub

    Public Sub Elimina(ByVal entita As SDI_Log)
        _lowW.Elimina(entita)
    End Sub
    Public Sub Aggiorna(ByVal entita As AgronicaCoreEntityFramework_POCO.SDI_Log)
        _lowW.Aggiorna(entita)
    End Sub

    Public Function Leggi(ByVal filtro As SDI_Log_Filter) As List(Of AgronicaCoreEntityFramework_POCO.SDI_Log)
        Return _logR.Leggi(filtro)
    End Function

    Public Function LeggiConAgenda(ByVal filtro As SDI_Log_Filter) As IEnumerable(Of Object)
        Return _logR.LeggiConAgenda(filtro)
    End Function

    Public Function LeggiPerSezionale(ByVal PIVA As String, ByVal Doc_numero_Sin As String, ByVal Doc_NUmero_Des As String, ByVal logIdPartenza As Integer) _
        As List(Of AgronicaCoreEntityFramework_POCO.SDI_Log)
        Return _logR.LeggiPerSezionale(PIVA, Doc_numero_Sin, Doc_NUmero_Des, logIdPartenza)
    End Function

    Public Function AcquisisciLock(ByVal filtro As SDI_Log_Filter, ByVal agenda As AgendaXLavCod, ByRef log As SDI_Log, ByRef statoPrecedente As StatoFattura_Gias) As Boolean

        Dim logEsistente = _logR.Leggi(filtro).FirstOrDefault()

        If logEsistente IsNot Nothing Then

            'log esiste già -> guardo se lo stato non è locked
            log = logEsistente
            statoPrecedente = logEsistente.Gias_Status
            If logEsistente.Gias_Status <> StatoFattura_Gias.InLock Then
                'lockko la riga
                logEsistente.Gias_Status = StatoFattura_Gias.InLock
                _lowW.Aggiorna(logEsistente)
                Return True
            Else
                ' se lo stato è locked non posso processare
                Return False
            End If
        Else
            ' log non esiste -> creo record di lock
            _lowW.LoggaLock(filtro.Id_Agenda, filtro.PIVA, agenda)
            log = _logR.Leggi(filtro).FirstOrDefault()
            statoPrecedente = StatoFattura_Gias.NonDefinito
            Return True
        End If

    End Function

End Class

Public Class SDI_Log_W : Inherits EFatturaBaseDAL

    Private ReadOnly _giasContext As Gias_DeveloperServer_Entities
    Public Sub New(ByVal giasContext As Gias_DeveloperServer_Entities)
        _giasContext = giasContext
    End Sub

    Public Sub LoggaLock(ByVal idAgenda As Integer, ByVal piva As String, ByVal agenda As AgendaXLavCod)

        Try

            Dim entita = New SDI_Log With
           {
               .Piva = piva,
               .Id_Agenda = idAgenda,
               .Gias_Status = StatoFattura_Gias.InLock,
               .Data_Creazione = DateTime.Now,
               .Data_Modifica = DateTime.Now,
               .Doc_Numero = agenda.Doc_numero,
               .Doc_Numero_Des = agenda.Doc_NUmero_Des,
               .Doc_Numero_Sin = agenda.Doc_NUmero_Sin
           }

            _giasContext.SDI_Log.Add(entita)
            _giasContext.SaveChanges()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "SDI_Log_W.LoggaLock")
        End Try

    End Sub
    Public Sub LoggaXmlNonGenerato(
            ByVal tipoDocumento As String,
            ByVal tipoTracciato As String,
            ByVal listaErrori As List(Of String),
            ByVal stato As StatoFattura_Gias,
            ByVal log As SDI_Log)

        Try

            log.TipoFattura = tipoDocumento
            log.TipoTracciato = tipoTracciato
            log.Gias_Status = stato
            log.Data_Creazione = DateTime.Now
            log.Data_Modifica = DateTime.Now

            If listaErrori IsNot Nothing AndAlso listaErrori.Any Then
                listaErrori.ForEach(Sub(e)
                                        Dim dettaglio = New SDI_Log_Dettaglio With
                                           {
                                               .Messaggio = e,
                                               .Data_Creazione = DateTime.Now,
                                               .TipoErrore = If(stato = StatoFattura_Gias.XMLNonGeneratoPerErrori, TipoErroreEFattura.Eccezzione, TipoErroreEFattura.VerificheFallite)
                                           }
                                        log.SDI_Log_Dettaglio.Add(dettaglio)

                                    End Sub)
            End If

            Aggiorna(log)

        Catch ex As Exception
            Throw RaiseDAlException(ex, "SDI_Log_W.LoggaXmlNonGenerato")
        End Try

    End Sub

    Public Sub Aggiorna(ByVal entita As AgronicaCoreEntityFramework_POCO.SDI_Log)

        Try
            entita.Data_Modifica = DateTime.Now
            _giasContext.Entry(entita).State = EntityState.Modified
            _giasContext.SaveChanges()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "SDI_Log_W.Aggiorna")
        End Try

    End Sub

    Public Sub Elimina(ByVal entita As AgronicaCoreEntityFramework_POCO.SDI_Log)

        Try
            _giasContext.SDI_Log.Remove(entita)
            _giasContext.SaveChanges()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "SDI_Log_W.Elimina")
        End Try

    End Sub

End Class

Public Class SDI_Log_R : Inherits EFatturaBaseDAL

    Private ReadOnly _giasContext As Gias_DeveloperServer_Entities
    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal giasContext As Gias_DeveloperServer_Entities)
        _giasContext = giasContext
    End Sub

    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)

        _giasContext = New Gias_DeveloperServer_Entities(efConnString)

    End Sub

    Public Function LeggiAgendeErrateDopoXGiorni(ByVal piva As String, ByVal periodoControlloGG As Integer) As ControlloPreliminare

        Try

            Dim controlloPreliminare As New ControlloPreliminare

            Dim agende As List(Of AgendaXLavCod) = New List(Of AgendaXLavCod)

            Dim qry = From logs In _giasContext.SDI_Log
                      Group Join agenda In _giasContext.Agenda
                      On logs.Id_Agenda Equals agenda.Id_Agenda And logs.Piva Equals agenda.PIVA Into agenda_group = Group
                      From _agenda_group In agenda_group.DefaultIfEmpty()
                      Group Join movimenti In _giasContext.Movimenti
                      On _agenda_group.PIVA Equals movimenti.PIVA And _agenda_group.Id_Agenda Equals movimenti.Id_Agenda Into movi_group = Group
                      From _movi_group In movi_group.DefaultIfEmpty()
                      Where logs.Piva.Trim.Equals(piva) AndAlso
                            (logs.Gias_Status.Value.Equals(StatoFattura_Gias.XMLGenerato) OrElse
                            logs.Gias_Status.Value.Equals(StatoFattura_Gias.InviatoSDI_KO)) _
                      AndAlso _movi_group.Cau_Mov.Trim().Equals(CAU_REGISTRAZIONI)
                      Select New With
                            {
                                .Log = logs,
                                .Agenda = agenda_group.FirstOrDefault(),
                                .Movimento = _movi_group
                            }

            For Each obj As Object In qry.ToList()
                Dim a = New AgendaXLavCod
                a.Blocco_Flag = If(obj.Agenda Is Nothing, 0, obj.Agenda.Blocco_Flag)
                a.Doc_numero = obj.Log.Doc_Numero
                a.Doc_NUmero_Des = obj.Log.Doc_Numero_Des
                a.Doc_NUmero_Sin = obj.Log.Doc_Numero_Sin
                a.IdAgenda = If(obj.Agenda Is Nothing, Int32.MinValue, obj.Agenda.Id_Agenda)
                a.NomeFIleXml = obj.Log.NomeFileXML
                'a.DataMovimento = If(obj.Agenda Is Nothing, DateTime.MinValue, obj.Agenda.Data_Creazione)
                a.DataMovimento = If(obj.Movimento Is Nothing, DateTime.MinValue, obj.Movimento.Validita_Inizio)
                a.StatoGias = CInt(obj.Log.Gias_Status)
                agende.Add(a)
            Next

            ' Eliminate
            controlloPreliminare.PresentiInSdiLogMaEliminate = agende.Where(Function(s) s.IdAgenda = Int32.MinValue).ToList()

            Dim dataRiferifento As DateTime = DateTime.Now.AddDays(periodoControlloGG * -1)
            Dim agendeNonEliminate = agende.Where(Function(s) s.IdAgenda <> Int32.MinValue).ToList()
            Dim agendeDaControllare As List(Of AgendaXLavCod) = New List(Of AgendaXLavCod)
            For Each a As AgendaXLavCod In agendeNonEliminate
                If a.DataMovimento.Value.Date < dataRiferifento.Date.Date Then
                    agendeDaControllare.Add(a)
                End If
            Next
            controlloPreliminare.Generate = agendeDaControllare.Where(Function(s) s.StatoGias = StatoFattura_Gias.XMLGenerato).OrderByDescending(Function(s) s.DataMovimento).ToList()
            controlloPreliminare.InviatoSDI_KO = agendeDaControllare.Where(Function(s) s.StatoGias = StatoFattura_Gias.InviatoSDI_KO).OrderByDescending(Function(s) s.DataMovimento).ToList()

            Return controlloPreliminare

        Catch ex As Exception
            Throw RaiseDAlException(ex, "SDI_Log_R.LeggiAgenteErrateDopoXGiorni")
        End Try

    End Function

    Public Function LeggiPerSezionale(ByVal PIVA As String, ByVal Doc_numero_Sin As String, ByVal Doc_NUmero_Des As String, ByVal logIdPartenza As Integer) _
        As List(Of AgronicaCoreEntityFramework_POCO.SDI_Log)

        Try

            Dim qry = From logs In _giasContext.SDI_Log
                      Where logs.Id_Log > logIdPartenza AndAlso
                      logs.Doc_Numero_Sin.Equals(Doc_numero_Sin) AndAlso
                      logs.Doc_Numero_Des.Equals(Doc_NUmero_Des) AndAlso
                      logs.Piva.Equals(PIVA)
            Return qry.ToList()

        Catch ex As Exception
            Throw RaiseDAlException(ex, "SDI_Log_R.LeggiPerSezionale")
        End Try

    End Function
    Public Function Leggi(ByVal filtro As SDI_Log_Filter) As List(Of AgronicaCoreEntityFramework_POCO.SDI_Log)

        Try

            If filtro.NomeFileXML Is Nothing Then filtro.NomeFileXML = ""

            Dim qry = From logs In _giasContext.SDI_Log
                      Where (filtro.NomeFileXML = "" OrElse logs.NomeFileXML.Equals(filtro.NomeFileXML)) _
                      AndAlso (filtro.Id_Agenda = 0 OrElse logs.Id_Agenda.Value.Equals(filtro.Id_Agenda)) _
                      AndAlso (filtro.PIVA = "" OrElse logs.Piva.Trim.Equals(filtro.PIVA.Trim())) _
                      AndAlso (filtro.StatoGias = StatoFattura_Gias.NonDefinito OrElse logs.Gias_Status.Value.Equals(filtro.StatoGias)) _
                      AndAlso (filtro.StatoSDI = StatoFattura_SDI.NonDefinito OrElse logs.SDI_Status.Value.Equals(filtro.StatoSDI) OrElse logs.SDI_Status.Value.Equals(StatoFattura_SDI.NonDefinito))
                      Select logs


            Return qry.ToList()

        Catch ex As Exception
            Throw RaiseDAlException(ex, "SDI_Log_R.Leggi")
        End Try

    End Function


    Public Function LeggixCancellazione(ByRef objParametri As AgronicaCoreParametri,
                                        ByVal Id_Agenda As Integer
                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreEFattura_DAL.SDI_Log_R.LeggixCancellazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim DP As New AgronicaCoreDataProvider.DataProvider

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  SDI_Log ")
            StrSQL.AppendLine(" Where Id_Agenda = " & Id_Agenda & " ")
            StrSQL.AppendLine(" AND Gias_Status = 500 ")


            '--------------------------------------------------------------------------
            DT = DP.EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            DP.Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function




    Public Function LeggiConAgenda(ByVal filtro As SDI_Log_Filter) As IEnumerable(Of Object)

        Try

            If filtro.NomeFileXML Is Nothing Then filtro.NomeFileXML = ""

            Dim qry = From logs In _giasContext.SDI_Log
                      Group Join agenda In _giasContext.Agenda
                      On logs.Id_Agenda Equals agenda.Id_Agenda And logs.Piva Equals agenda.PIVA Into agenda_group = Group
                      From _agenda_group In agenda_group.DefaultIfEmpty()
                      Group Join movimenti In _giasContext.Movimenti
                      On _agenda_group.PIVA Equals movimenti.PIVA And _agenda_group.Id_Agenda Equals movimenti.Id_Agenda Into movi_group = Group
                      From _movi_group In movi_group.DefaultIfEmpty()
                      Where (filtro.NomeFileXML = "" OrElse logs.NomeFileXML.Equals(filtro.NomeFileXML)) _
                      AndAlso (filtro.Id_Agenda = 0 OrElse logs.Id_Agenda.Value.Equals(filtro.Id_Agenda)) _
                      AndAlso (filtro.PIVA = "" OrElse logs.Piva.Trim.Equals(filtro.PIVA.Trim())) _
                      AndAlso (filtro.StatoGias = StatoFattura_Gias.NonDefinito OrElse logs.Gias_Status.Value.Equals(filtro.StatoGias)) _
                      AndAlso (filtro.StatoSDI = StatoFattura_SDI.NonDefinito OrElse logs.SDI_Status.Value.Equals(filtro.StatoSDI) OrElse logs.SDI_Status.Value.Equals(StatoFattura_SDI.NonDefinito)) _
                      AndAlso _movi_group.Cau_Mov.Trim().Equals(CAU_REGISTRAZIONI)
                      Select New With
                          {
                               .Log = logs,
                               .Agenda = New AgendaXLavCod With
                                {
                                    .Blocco_Flag = _agenda_group.Blocco_Flag,
                                    .Doc_numero = logs.Doc_Numero,
                                    .Doc_NUmero_Des = logs.Doc_Numero_Des,
                                    .Doc_NUmero_Sin = logs.Doc_Numero_Sin,
                                    .IdAgenda = logs.Id_Agenda,
                                    .NomeFIleXml = logs.NomeFileXML,
                                    .DataMovimento = _movi_group.Validita_Inizio
                                }
                          }

            'Dim qry = From logs In _giasContext.SDI_Log
            '          Group Join agenda In _giasContext.Agenda
            '          On logs.Id_Agenda Equals agenda.Id_Agenda And logs.Piva Equals agenda.PIVA Into agenda_group = Group
            '          From _agenda_group In agenda_group.DefaultIfEmpty()
            '          Where (filtro.NomeFileXML = "" OrElse logs.NomeFileXML.Equals(filtro.NomeFileXML)) _
            '          AndAlso (filtro.Id_Agenda = 0 OrElse logs.Id_Agenda.Value.Equals(filtro.Id_Agenda)) _
            '          AndAlso (filtro.PIVA = "" OrElse logs.Piva.Trim.Equals(filtro.PIVA.Trim())) _
            '          AndAlso (filtro.StatoGias = StatoFattura_Gias.NonDefinito OrElse logs.Gias_Status.Value.Equals(filtro.StatoGias)) _
            '          AndAlso (filtro.StatoSDI = StatoFattura_SDI.NonDefinito OrElse logs.SDI_Status.Value.Equals(filtro.StatoSDI) OrElse logs.SDI_Status.Value.Equals(StatoFattura_SDI.NonDefinito))
            '          Select New With
            '              {
            '                   .Log = logs,
            '                   .Agenda = New AgendaXLavCod With
            '                    {
            '                        .Blocco_Flag = _agenda_group.Blocco_Flag,
            '                        .Doc_numero = logs.Doc_Numero,
            '                        .Doc_NUmero_Des = logs.Doc_Numero_Des,
            '                        .Doc_NUmero_Sin = logs.Doc_Numero_Sin,
            '                        .IdAgenda = logs.Id_Agenda,
            '                        .NomeFIleXml = logs.NomeFileXML,
            '                        .DataMovimento = _agenda_group.Data_Creazione
            '                    }
            '              }

            Return qry.ToList()

        Catch ex As Exception
            Throw RaiseDAlException(ex, "SDI_Log_R.LeggiConAgenda")
        End Try

    End Function

    Public Function LeggiOK() As List(Of SDI_Log)

        Dim statiSDI As Integer() = {StatoFattura_SDI.RicevutaConsegna, StatoFattura_SDI.RicevutaMancataConsegna}

        Try
            Dim qry = From logs In _giasContext.SDI_Log
                      Where statiSDI.Contains(logs.SDI_Status)
                      Select logs

            Return qry.ToList()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "SDI_Log_R.LeggiOK")
        End Try

    End Function
    Public Function LeggiKO() As List(Of SDI_Log)

        Dim statiGias As Integer() = {StatoFattura_Gias.XMLNonGenerabile, StatoFattura_Gias.XMLNonGeneratoPerErrori, StatoFattura_Gias.InviatoSDI_KO}

        Try
            Dim qry = From logs In _giasContext.SDI_Log
                      Where statiGias.Contains(logs.Gias_Status.Value) _
                      OrElse (logs.Gias_Status.Value.Equals(StatoFattura_Gias.InviatoSDI_OK) AndAlso logs.SDI_Status.Value.Equals(StatoFattura_SDI.RicevutaScato))
                      Select logs
            Return qry.ToList()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "SDI_Log_R.LeggiKO")
        End Try

    End Function

End Class

' TODO Spostare
Public Enum StatoFattura_Gias

    NonDefinito = 0
    InLock = 1
    XMLNonGeneratoPerErrori = 300
    XMLNonGenerabile = 301
    XMLGenerato = 400
    XMLEsportato = 401
    XMLDaRigenerare = 402
    InviatoSDI_OK = 500
    InviatoSDI_KO = 403

End Enum

Public Enum StatoFattura_SDI
    NonDefinito = 0
    EsitoNonAncoraDisponibile = 100
    EsitiMultipli = 200
    RicevutaScato = 300
    RicevutaConsegna = 400
    RicevutaMancataConsegna = 500
End Enum

Public Enum TipoErroreEFattura
    NonDefinito = 0
    Validazione2C = 1
    ValidazioneSDI = 2
    Eccezzione = 3
    VerificheFallite = 4
End Enum

'AT: Perpetua Mancata Consegna - Solo PA
'DT: Decorrenza Termini - Solo PA
'MC: Mancata Consegna
'NS: Notifica di Scarto
'RC: Ricevuta di Consegna
'EC_ACCETTAZIONE: Esito Committente Accettazione - Solo PA
'EC_RIFIUTO: Esito Committente Rifiuto - Solo PA
