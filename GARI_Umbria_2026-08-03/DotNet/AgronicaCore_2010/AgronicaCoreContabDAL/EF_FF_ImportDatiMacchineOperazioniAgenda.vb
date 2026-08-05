Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL
Imports System.Data.Entity
Imports System.Net

Public Class EF_FF_ImportDatiMacchineOperazioniAgenda
    Inherits AgronicaCoreDataProvider.DataProvider

    Private Shared Function CreateImportDatiMacchineOperazioniAgenda(ByRef dal As Gias_DeveloperServer_Entities,
                                                                     ByRef pivasuperuser As String,
                                                                     ByRef piva As String,
                                                                     ByRef lav_cod As Integer,
                                                                     ByRef idReg As Integer,
                                                                     ByRef id_agenda As Integer,
                                                                     ByRef username As String,
                                                                     ByRef objParametri As AgronicaCoreParametri,
                                                                     ByRef objParametriUtenti As AgronicaCoreParametri
                                                                    ) As FF_ImportDatiMacchineOperazioniAgenda

        Dim datimacchine As New FF_ImportDatiMacchineOperazioniAgenda

        Dim idGen As New AgronicaCoreDataProvider.Agro_Sequenze
        'datimacchine.ID = idGen.NuovoId_Tabella_EF(dal, "FF_ImportDatiMacchineOperazioniAgenda", 0, 2000000, objParametri)
        datimacchine.ID = -1

        datimacchine.pivasuperuser = ""
        datimacchine.piva = ""
        datimacchine.cod_operazione = 0
        datimacchine.cod_impianto = 0
        datimacchine.id_agenda = 0
        datimacchine.payload = ""
        datimacchine.stato = 0

        datimacchine.inviato = 0

        datimacchine.Data_Creazione = DateTime.Now
        datimacchine.Data_Modifica = DateTime.Now

        datimacchine.Username_Creazione = username
        datimacchine.Username_Modifica = username

        datimacchine.Validita_Inizio = AGRODATAINIZIO
        datimacchine.Validita_Fine = AGRODATAFINE

        Return datimacchine
    End Function

    Private Shared Function ImportDatiMacchineOperazioniAgenda_Scrivi_EF(ByVal DatiMacchina As AgronicaCoreModelsSTD.scambiodati.ImportDatiMacchineOperazionAgenda,
                                                 ByRef objParametriServer As AgronicaCoreParametri,
                                                 ByRef objParametriUtenti As AgronicaCoreParametri,
                                                 ByVal username As String,
                                                 Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                 Optional ByVal NewTransaction As Boolean = True
                                                 ) As FF_ImportDatiMacchineOperazioniAgenda

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EF_FF_ImportDatiMacchineOperazioniAgenda.ImportDatiMacchineOperazioniAgenda_Scrivi_EF"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        If EFImprese.ImpresaExist(GiasContext, DatiMacchina.pivasuperuser) = False Then
            Throw New WebException("Partita Iva (" + DatiMacchina.pivasuperuser + ") non anagrafica imprese. Operazione annullata")
        End If

        If EFImprese.ImpresaExist(GiasContext, DatiMacchina.piva) = False Then
            Throw New WebException("Partita Iva (" + DatiMacchina.piva + ") non anagrafica imprese. Operazione annullata")
        End If

        Dim DatiMacchineOperazioniAgenda = CreateImportDatiMacchineOperazioniAgenda(GiasContext,
                                                                    DatiMacchina.pivasuperuser,
                                                                    DatiMacchina.piva,
                                                                    DatiMacchina.cod_operazione,
                                                                    DatiMacchina.cod_impianto,
                                                                    DatiMacchina.id_agenda,
                                                                    username,
                                                                    objParametriServer,
                                                                    objParametriUtenti)

        Try
            DatiMacchineOperazioniAgenda.ID = DatiMacchina.ID
            DatiMacchineOperazioniAgenda.pivasuperuser = DatiMacchina.pivasuperuser
            DatiMacchineOperazioniAgenda.piva = DatiMacchina.piva
            DatiMacchineOperazioniAgenda.cod_operazione = DatiMacchina.cod_operazione
            DatiMacchineOperazioniAgenda.cod_impianto = DatiMacchina.cod_impianto
            DatiMacchineOperazioniAgenda.id_agenda = DatiMacchina.id_agenda
            DatiMacchineOperazioniAgenda.payload = DatiMacchina.payload
            DatiMacchineOperazioniAgenda.stato = 1

            GiasContext.FF_ImportDatiMacchineOperazioniAgenda.Add(DatiMacchineOperazioniAgenda)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch wex As WebException
            DatiMacchineOperazioniAgenda = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw New Exception(wex.Message)
        Catch ex As Exception
            DatiMacchineOperazioniAgenda = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return DatiMacchineOperazioniAgenda
    End Function

    Private Shared Function ImportDatiMacchineOperazioniAgenda_Modifica_EF(ByVal DatiMacchina As AgronicaCoreModelsSTD.scambiodati.ImportDatiMacchineOperazionAgenda,
                                                                     ByRef objParametriServer As AgronicaCoreParametri,
                                                                     ByRef objParametriUtenti As AgronicaCoreParametri,
                                                                     ByVal username As String,
                                                                     Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                     Optional ByVal NewTransaction As Boolean = True
                                                                     ) As FF_ImportDatiMacchineOperazioniAgenda

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EF_FF_ImportDatiMacchineOperazioniAgenda.ImportDatiMacchineOperazioniAgenda_Modifica_EF"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        Dim DatiMacchineOperazioniAgendaList = From DatiMacchineOperazioniAgenda In GiasContext.FF_ImportDatiMacchineOperazioniAgenda
                                               Where DatiMacchineOperazioniAgenda.ID = DatiMacchina.ID
                                               Select DatiMacchineOperazioniAgenda

        Dim oDatiMacchineOperazioniAgenda = DatiMacchineOperazioniAgendaList.FirstOrDefault()

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        If EFImprese.ImpresaExist(GiasContext, DatiMacchina.pivasuperuser) = False Then
            Throw New WebException("Partita Iva (" + DatiMacchina.pivasuperuser + ") non anagrafica imprese. Operazione annullata")
        End If

        If EFImprese.ImpresaExist(GiasContext, DatiMacchina.piva) = False Then
            Throw New WebException("Partita Iva (" + DatiMacchina.piva + ") non anagrafica imprese. Operazione annullata")
        End If

        If oDatiMacchineOperazioniAgenda Is Nothing Then
            Throw New WebException("Record DatiMacchina (" + DatiMacchina.ID.ToString() +
                                               ") non trovato in anagrafica. Impossibile proseguire")
        End If

        Try
            If (oDatiMacchineOperazioniAgenda.pivasuperuser <> DatiMacchina.pivasuperuser) Or
                    (oDatiMacchineOperazioniAgenda.piva <> DatiMacchina.piva) Or
                    (oDatiMacchineOperazioniAgenda.cod_operazione <> DatiMacchina.cod_operazione) Or
                    (oDatiMacchineOperazioniAgenda.cod_impianto <> DatiMacchina.cod_impianto) Or
                    ((oDatiMacchineOperazioniAgenda.id_agenda <> DatiMacchina.id_agenda) And DatiMacchina.id_agenda <> 0) Then

                Throw New WebException("ID Task " + DatiMacchina.ID.ToString() + " già presente per altro tipo operazione/azienda/impianto, modifica non permessa. Verificare i dati inviati.")
            End If

            oDatiMacchineOperazioniAgenda.pivasuperuser = DatiMacchina.pivasuperuser
            oDatiMacchineOperazioniAgenda.piva = DatiMacchina.piva
            oDatiMacchineOperazioniAgenda.cod_operazione = DatiMacchina.cod_operazione
            oDatiMacchineOperazioniAgenda.cod_impianto = DatiMacchina.cod_impianto
            ' l'ide agenda lo aggiorno solo se quest'ultimo è ancora 0 suula tabella di confine (questo per evitare di perdere il legame in tracciabilità)
            If (oDatiMacchineOperazioniAgenda.id_agenda = 0) Then
                oDatiMacchineOperazioniAgenda.id_agenda = DatiMacchina.id_agenda
            End If
            oDatiMacchineOperazioniAgenda.payload = DatiMacchina.payload
            If oDatiMacchineOperazioniAgenda.stato > 1 Then
                oDatiMacchineOperazioniAgenda.stato = 1
            Else
                oDatiMacchineOperazioniAgenda.stato = DatiMacchina.status
            End If

            oDatiMacchineOperazioniAgenda.Data_Modifica = DateTime.Now

            GiasContext.FF_ImportDatiMacchineOperazioniAgenda.Attach(oDatiMacchineOperazioniAgenda)
            GiasContext.Entry(oDatiMacchineOperazioniAgenda).State = EntityState.Modified
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch wex As WebException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw New WebException(wex.Message)
        Catch ex As Exception
            oDatiMacchineOperazioniAgenda = Nothing
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return oDatiMacchineOperazioniAgenda
    End Function

    Public Shared Sub ImportDatiMacchineOperazioniAgenda_Cancella_EF(ByVal DatiMacchina As AgronicaCoreModelsSTD.scambiodati.ImportDatiMacchineOperazionAgenda,
                                                                     ByRef objParametriServer As AgronicaCoreParametri,
                                                                     ByRef objParametriUtenti As AgronicaCoreParametri,
                                                                     ByVal username As String,
                                                                     Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                                                     Optional ByVal NewTransaction As Boolean = True
                                                                     )

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EF_FF_ImportDatiMacchineOperazioniAgenda.ImportDatiMacchineOperazioniAgenda_Cancella_EF"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametriServer.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Try
            Dim DatiMacchineOperazioniAgendaList = From DatiMacchineOperazioniAgenda In GiasContext.FF_ImportDatiMacchineOperazioniAgenda
                                                   Where DatiMacchineOperazioniAgenda.ID = DatiMacchina.ID
                                                   Select DatiMacchineOperazioniAgenda

            Dim oDatiMacchineOperazioniAgenda = DatiMacchineOperazioniAgendaList.FirstOrDefault()

            If oDatiMacchineOperazioniAgenda Is Nothing Then
                Throw New WebException("Record DatiMacchina (" + DatiMacchina.ID.ToString() +
                                               ") non trovato in anagrafica. Impossibile proseguire")
            End If

            GiasContext.FF_ImportDatiMacchineOperazioniAgenda.Attach(oDatiMacchineOperazioniAgenda)
            GiasContext.FF_ImportDatiMacchineOperazioniAgenda.Remove(oDatiMacchineOperazioniAgenda)
            GiasContext.SaveChanges()

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

    End Sub

    Public Shared Function Internal_Scrivi_DatiMacchinaOperazioneAgenda(ByVal data As AgronicaCoreModelsSTD.scambiodati.ImportDatiMacchineOperazionAgenda,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByVal username As String,
                                         Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                         Optional ByVal NewTransaction As Boolean = True
                                         ) As AgronicaCoreEntityFramework_POCO.FF_ImportDatiMacchineOperazioniAgenda

        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.FF_ImportDatiMacchineOperazioniAgenda_W.Internal_Scrivi_DatiMacchinaOperazioneAgenda()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim ret As AgronicaCoreEntityFramework_POCO.FF_ImportDatiMacchineOperazioniAgenda = Nothing

        Try
            ret = AgronicaCoreContabDAL.EF_FF_ImportDatiMacchineOperazioniAgenda.ImportDatiMacchineOperazioniAgenda_Scrivi_EF(data,
                                                                                objParametri,
                                                                                objParametri_utenti,
                                                                                username,
                                                                                GiasContext,
                                                                                NewTransaction)

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch wex As WebException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw New Exception(wex.Message)
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return ret
    End Function

    Public Shared Function Internal_Modifica_DatiMacchinaOperazioneAgenda(ByVal data As AgronicaCoreModelsSTD.scambiodati.ImportDatiMacchineOperazionAgenda,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByVal username As String,
                                         Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                         Optional ByVal NewTransaction As Boolean = True
                                         ) As AgronicaCoreEntityFramework_POCO.FF_ImportDatiMacchineOperazioniAgenda

        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.FF_ImportDatiMacchineOperazioniAgenda_W.Internal_Modifica_DatiMacchinaOperazioneAgenda()"
        Dim messaggioErrore As String = ""
        Dim scope As TransactionScope = Nothing
        Dim bCloseContext As Boolean = False

        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If
        If NewTransaction Then
            scope = New TransactionScope()
        End If

        Dim ret As FF_ImportDatiMacchineOperazioniAgenda = Nothing

        Try
            ret = ImportDatiMacchineOperazioniAgenda_Modifica_EF(data,
                                                                objParametri,
                                                                objParametri_utenti,
                                                                username,
                                                                GiasContext,
                                                                NewTransaction)

            If NewTransaction Then
                scope.Complete()
                scope.Dispose()
            End If
        Catch wex As WebException
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            Throw New WebException(wex.Message)
        Catch ex As Exception
            If scope IsNot Nothing Then
                scope.Dispose()
            End If
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If bCloseContext Then
            GiasContext.Dispose()
        End If

        Return ret
    End Function

End Class

