Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports Newtonsoft.Json.Linq

Public Class G2GAttivita_R

    Public Function Leggi_Attivita_G2G(ByVal _Piva As String, ByVal PivaSuperUser_Destinazione As String, Configurazione As G2G_Configurazione_FiltriReq_Attivita, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef listaImpresePubbliche As List(Of String)) As G2G_Attivita

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GAttivita_R.Leggi_Attivita_G2G()"
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim rval As New G2G_Attivita

        'quanto sicuramente non assegnato ...
        With rval
            .From_Piva = _Piva
            .From_PivaSuperUser = objParametri.PivaSuperUser
        End With

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaAttivita As New List(Of Attivita)

            Dim attivita_insert = (From agenda In GiasContext.Agenda
                                   Join attivita In GiasContext.Attivita On agenda.Id_Attivita Equals attivita.ID_Attivita
                                   Where Configurazione.listaImprese.Contains(agenda.PIVA) _
                                      AndAlso agenda.Validita_Inizio >= Configurazione.dataValidita_Inizio _
                                      AndAlso agenda.Validita_Inizio <= Configurazione.dataValidita_Fine _
                                      AndAlso (Configurazione.ListaLav_Cod.Count > 0 And Configurazione.ListaLav_Cod.Contains(agenda.Lav_Cod) Or Configurazione.ListaLav_Cod.Count = 0) _
                                      AndAlso Not GiasContext.G2G_Recode_Attivita.Any(Function(g) g.From_PivaSuperUser = attivita.Piva_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.FromId_Attivita = attivita.ID_Attivita)
                                   Select attivita).Union(
                                  From agenda In GiasContext.Agenda
                                  Join movimenti_dettagli In GiasContext.Movimenti_dettagli On agenda.Id_Agenda Equals movimenti_dettagli.Id_Agenda
                                  Join attivita In GiasContext.Attivita On movimenti_dettagli.ID_Attivita Equals attivita.ID_Attivita
                                  Where Configurazione.listaImprese.Contains(agenda.PIVA) _
                                                  AndAlso agenda.Validita_Inizio >= Configurazione.dataValidita_Inizio _
                                                  AndAlso agenda.Validita_Inizio <= Configurazione.dataValidita_Fine _
                                                  AndAlso (Configurazione.ListaLav_Cod.Count > 0 And Configurazione.ListaLav_Cod.Contains(agenda.Lav_Cod) Or Configurazione.ListaLav_Cod.Count = 0) _
                                                  AndAlso Not GiasContext.G2G_Recode_Attivita.Any(Function(g) g.From_PivaSuperUser = attivita.Piva_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.FromId_Attivita = attivita.ID_Attivita)
                                  Select attivita).Distinct.ToList

            For Each att In attivita_insert

                If att.Piva = objParametri.PivaSuperUser Then
                    listaAttivita.Add(att)
                    Continue For
                ElseIf Configurazione.listaImprese.Contains(att.Piva) Then
                    listaAttivita.Add(att)
                    Continue For
                Else

                    Dim G2GAzienda = (From imp In GiasContext.Imprese
                                      Join g2g In GiasContext.G2G_Recode_Imprese On imp.PIVA Equals g2g.FROM_Piva
                                      Where g2g.From_PivaSuperUser = rval.From_PivaSuperUser And g2g.To_PivaSuperUser = PivaSuperUser_Destinazione Select g2g).FirstOrDefault

                    If G2GAzienda IsNot Nothing Then
                        att.Piva = G2GAzienda.To_Piva
                        listaAttivita.Add(att)
                        Continue For
                    End If

                End If

            Next

            rval.attivita_insert = listaAttivita

            rval.attivita_update = (From attivita In GiasContext.Attivita
                                    Join g2g In GiasContext.G2G_Recode_Attivita On attivita.Piva_SuperUser Equals g2g.From_PivaSuperUser And
                                                                    attivita.ID_Attivita Equals g2g.FromId_Attivita
                                    Where g2g.datainvio < attivita.Data_Modifica
                                    Select attivita).ToList


            rval.G2G_Attivita_Recode_delete = (
                        From r In GiasContext.G2G_Recode_Attivita
                        Where Not GiasContext.Attivita.Any(Function(p) p.ID_Attivita = r.FromId_Attivita And p.Piva_SuperUser = r.From_PivaSuperUser)
                        Select r).Distinct.ToList()



        End Using

        Return rval

    End Function

    Public Function Leggi_Attivita_G2GReverse(ByVal _Piva As String, ByVal PivaSuperUser_Destinazione As String, Configurazione As G2G_Configurazione_FiltriReq_Attivita, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef listaImpresePubbliche As List(Of String)) As G2G_Attivita_Reverse

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GAttivita_R.Leggi_Attivita_G2GReverse()"
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim rval As New G2G_Attivita_Reverse

        'quanto sicuramente non assegnato ...
        With rval
            .From_Piva = _Piva
            .From_PivaSuperUser = objParametri.PivaSuperUser
        End With

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaAttivita As New List(Of Attivita)

            Dim attivita_insert = (From agenda In GiasContext.Agenda
                                   Join attivita In GiasContext.Attivita On agenda.Id_Attivita Equals attivita.ID_Attivita
                                   Where Configurazione.listaImprese.Contains(agenda.PIVA) _
                                      AndAlso agenda.Validita_Inizio >= Configurazione.dataValidita_Inizio _
                                      AndAlso agenda.Validita_Inizio <= Configurazione.dataValidita_Fine _
                                      AndAlso (Configurazione.ListaLav_Cod.Count > 0 And Configurazione.ListaLav_Cod.Contains(agenda.Lav_Cod) Or Configurazione.ListaLav_Cod.Count = 0) _
                                      AndAlso Not GiasContext.G2G_Recode_Attivita.Any(Function(g) g.To_PivaSuperUser = attivita.Piva_SuperUser And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.ToId_Attivita = attivita.ID_Attivita)
                                   Select attivita).Union(
                                  From agenda In GiasContext.Agenda
                                  Join movimenti_dettagli In GiasContext.Movimenti_dettagli On agenda.Id_Agenda Equals movimenti_dettagli.Id_Agenda
                                  Join attivita In GiasContext.Attivita On movimenti_dettagli.ID_Attivita Equals attivita.ID_Attivita
                                  Where Configurazione.listaImprese.Contains(agenda.PIVA) _
                                                  AndAlso agenda.Validita_Inizio >= Configurazione.dataValidita_Inizio _
                                                  AndAlso agenda.Validita_Inizio <= Configurazione.dataValidita_Fine _
                                                  AndAlso (Configurazione.ListaLav_Cod.Count > 0 And Configurazione.ListaLav_Cod.Contains(agenda.Lav_Cod) Or Configurazione.ListaLav_Cod.Count = 0) _
                                                  AndAlso Not GiasContext.G2G_Recode_Attivita.Any(Function(g) g.To_PivaSuperUser = attivita.Piva_SuperUser And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.ToId_Attivita = attivita.ID_Attivita)
                                  Select attivita).Distinct.ToList

            For Each att In attivita_insert

                If att.Piva = objParametri.PivaSuperUser Then
                    listaAttivita.Add(att)
                    Continue For
                ElseIf Configurazione.listaImprese.Contains(att.Piva) Then
                    listaAttivita.Add(att)
                    Continue For
                Else

                    Dim G2GAzienda = (From imp In GiasContext.Imprese
                                      Join g2g In GiasContext.G2G_Recode_Imprese On imp.PIVA Equals g2g.FROM_Piva
                                      Where g2g.To_PivaSuperUser = rval.To_PivaSuperUser And g2g.From_PivaSuperUser = PivaSuperUser_Destinazione Select g2g).FirstOrDefault

                    If G2GAzienda IsNot Nothing Then
                        att.Piva = G2GAzienda.To_Piva
                        listaAttivita.Add(att)
                        Continue For
                    End If

                End If

            Next

            rval.attivita_insert = listaAttivita

            rval.attivita_update = (From attivita In GiasContext.Attivita
                                    Join g2g In GiasContext.G2G_Recode_Attivita On attivita.Piva_SuperUser Equals g2g.To_PivaSuperUser And
                                                                    attivita.ID_Attivita Equals g2g.ToId_Attivita
                                    Where g2g.datainvio < attivita.Data_Modifica
                                    Select attivita).ToList


            rval.G2G_Attivita_Recode_delete = (
                        From r In GiasContext.G2G_Recode_Attivita
                        Where Not GiasContext.Attivita.Any(Function(p) p.ID_Attivita = r.ToId_Attivita And p.Piva_SuperUser = r.To_PivaSuperUser)
                        Select r).Distinct.ToList()



        End Using

        Return rval

    End Function

End Class


Public Class G2GAttivita_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Attivita_G2G(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_Attivita, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDal.G2GAttivita_W.Scrivi_Attivita_G2G()"
        Dim messaggioErrore As String = ""
        g2g.Recode = New G2G_Recode

        Dim username = objParametri.UsernameOperazione
        Dim piva = g2g.From_Piva
        Dim data = Date.Now

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

        Try

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    '
                    'DELETE
                    '

                    For Each r As G2G_Recode_Attivita In g2g.G2G_Attivita_Recode_delete

                        ''cancella entita
                        Dim attivita = (From m In GiasContext.Attivita Where m.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso m.ID_Attivita = r.ToId_Attivita).FirstOrDefault()
                        GiasContext.Attivita.Attach(attivita)
                        GiasContext.Attivita.Remove(attivita)

                        '' cancella recode entita
                        Dim recode = (From rr In GiasContext.G2G_Recode_Attivita Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.FromId_Attivita = r.FromId_Attivita).FirstOrDefault()
                        GiasContext.G2G_Recode_Attivita.Attach(recode)
                        GiasContext.G2G_Recode_Attivita.Remove(recode)
                        GiasContext.SaveChanges()

                    Next

                    g2g.G2G_Attivita_Recode_update = New List(Of G2G_Recode_Attivita)

                    'TESTATA
                    For Each m As Attivita In g2g.attivita_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Attivita Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.FromId_Attivita = m.ID_Attivita).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Attivita_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Attivita.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim attivita = (From mm In GiasContext.Attivita Where mm.ID_Attivita = recode.ToId_Attivita).FirstOrDefault()
                        attivita = Gias_EF_Utility.CopyEntity(GiasContext, m, attivita, username, data)
                        attivita.Piva_SuperUser = Destinazione_Piva_SuperUser
                        attivita.ID_Attivita = recode.ToId_Attivita
                        GiasContext.Attivita.Attach(attivita)
                        GiasContext.Entry(attivita).State = EntityState.Modified

                        GiasContext.SaveChanges()

                    Next

                    g2g.G2G_Attivita_Recode_insert = New List(Of G2G_Recode_Attivita)

                    If g2g.attivita_insert.Count > 0 Then

                        For Each s As Attivita In g2g.attivita_insert

                            Dim idSeq As Integer = 0


                            'Richiedo un nuovo id sequenza
                            idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "Attivita", 0, 2000000000, objParametri)
                            Dim idLibero As Boolean = False
                            While idLibero = False
                                Dim libero = (From att In GiasContext.Attivita Where att.ID_Attivita = idSeq).FirstOrDefault
                                If libero Is Nothing Then
                                    idLibero = True
                                Else
                                    idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "Attivita", 0, 2000000000, objParametri)
                                End If
                            End While



                            Dim attivita = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                            attivita.ID_Attivita = idSeq
                            attivita.Piva_SuperUser = Destinazione_Piva_SuperUser


                            GiasContext.Attivita.Add(attivita)
                            GiasContext.SaveChanges()

                            Dim recode =
                                    New G2G_Recode_Attivita With {
                                        .From_PivaSuperUser = Origine_Piva_SuperUser,
                                        .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                        .FromId_Attivita = s.ID_Attivita,
                                        .ToId_Attivita = attivita.ID_Attivita,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                            g2g.G2G_Attivita_Recode_insert.Add(recode)
                            GiasContext.G2G_Recode_Attivita.Add(recode)

                            GiasContext.SaveChanges()

                        Next

                    End If


                    g2g.Recode.G2GRecodeAttivitaToInsert = g2g.G2G_Attivita_Recode_insert
                    g2g.Recode.G2GRecodeAttivitaToUpdate = g2g.G2G_Attivita_Recode_update
                    g2g.Recode.G2GRecodeAttivitaToDelete = g2g.G2G_Attivita_Recode_delete


                    ' COMIT Effettivo
                    scope.Complete()

                End Using

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            'Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_Attivita_G2GReverse(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_Attivita_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDal.G2GAttivita_W.Scrivi_Attivita_G2G()"
        Dim messaggioErrore As String = ""
        g2g.Recode = New G2G_Recode

        Dim username = objParametri.UsernameOperazione
        Dim piva = g2g.From_Piva
        Dim data = Date.Now

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

        Try

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    '
                    'DELETE
                    '

                    For Each r As G2G_Recode_Attivita In g2g.G2G_Attivita_Recode_delete

                        ''cancella entita
                        Dim attivita = (From m In GiasContext.Attivita Where m.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso m.ID_Attivita = r.FromId_Attivita).FirstOrDefault()
                        GiasContext.Attivita.Attach(attivita)
                        GiasContext.Attivita.Remove(attivita)

                        '' cancella recode entita
                        Dim recode = (From rr In GiasContext.G2G_Recode_Attivita Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.ToId_Attivita = r.FromId_Attivita).FirstOrDefault()
                        GiasContext.G2G_Recode_Attivita.Attach(recode)
                        GiasContext.G2G_Recode_Attivita.Remove(recode)
                        GiasContext.SaveChanges()

                    Next

                    g2g.G2G_Attivita_Recode_update = New List(Of G2G_Recode_Attivita)

                    'TESTATA
                    For Each m As Attivita In g2g.attivita_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Attivita Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.ToId_Attivita = m.ID_Attivita).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Attivita_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Attivita.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim attivita = (From mm In GiasContext.Attivita Where mm.ID_Attivita = recode.FromId_Attivita).FirstOrDefault()
                        attivita = Gias_EF_Utility.CopyEntity(GiasContext, m, attivita, username, data)
                        attivita.Piva_SuperUser = Destinazione_Piva_SuperUser
                        attivita.ID_Attivita = recode.FromId_Attivita
                        GiasContext.Attivita.Attach(attivita)
                        GiasContext.Entry(attivita).State = EntityState.Modified

                        GiasContext.SaveChanges()

                    Next

                    g2g.G2G_Attivita_Recode_insert = New List(Of G2G_Recode_Attivita)

                    If g2g.attivita_insert.Count > 0 Then

                        For Each s As Attivita In g2g.attivita_insert

                            Dim idSeq As Integer = 0


                            'Richiedo un nuovo id sequenza
                            idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "Attivita", 0, 2000000000, objParametri)
                            Dim idLibero As Boolean = False
                            While idLibero = False
                                Dim libero = (From att In GiasContext.Attivita Where att.ID_Attivita = idSeq).FirstOrDefault
                                If libero Is Nothing Then
                                    idLibero = True
                                Else
                                    idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "Attivita", 0, 2000000000, objParametri)
                                End If
                            End While



                            Dim attivita = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                            attivita.ID_Attivita = idSeq
                            attivita.Piva_SuperUser = Destinazione_Piva_SuperUser


                            GiasContext.Attivita.Add(attivita)
                            GiasContext.SaveChanges()

                            Dim recode =
                                    New G2G_Recode_Attivita With {
                                        .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                        .To_PivaSuperUser = Origine_Piva_SuperUser,
                                        .FromId_Attivita = attivita.ID_Attivita,
                                        .ToId_Attivita = s.ID_Attivita,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                            g2g.G2G_Attivita_Recode_insert.Add(recode)
                            GiasContext.G2G_Recode_Attivita.Add(recode)

                            GiasContext.SaveChanges()

                        Next

                    End If


                    g2g.Recode.G2GRecodeAttivitaToInsert = g2g.G2G_Attivita_Recode_insert
                    g2g.Recode.G2GRecodeAttivitaToUpdate = g2g.G2G_Attivita_Recode_update
                    g2g.Recode.G2GRecodeAttivitaToDelete = g2g.G2G_Attivita_Recode_delete


                    ' COMIT Effettivo
                    scope.Complete()

                End Using

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            'Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        End Try

        Return g2g.Recode

    End Function

End Class