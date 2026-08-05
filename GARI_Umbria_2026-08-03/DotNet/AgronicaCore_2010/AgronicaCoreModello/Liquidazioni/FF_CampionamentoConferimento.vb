Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello.My.Resources

Public Class FF_CampionamentoConferimento

    Public Function Rendi_Definitivo_AnagAccontiLiquidazioni(ByVal piva As String,
                                                             ByVal IDAnag As Integer,
                                                             ByRef objParametri As AgronicaCoreParametri,
                                                             ByRef messaggioErrore As String
                                                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreModello.FF_CampionamentoConferimento.Rendi_Definitivo_AnagAccontiLiquidazioni()"
        Dim xRisp As Boolean = False

        Dim pivaSuperUser As String = objParametri.PivaSuperUser

        Try

            Dim gefutils As New Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Dim dal As Gias_DeveloperServer_Entities = Nothing

            Dim transOptions As New TransactionOptions With {.IsolationLevel = IsolationLevel.ReadCommitted}
            Using ts As New TransactionScope(TransactionScopeOption.Required, transOptions)

                Try

                    'Se arrivo qui, vuol dire che sono sicura di avere dei dati in Liquid_Mov_Dati_Generali_CampionamentoConferito

                    dal = New Gias_DeveloperServer_Entities(efConnString)

                    'dal.ContextOptions.UseLegacyPreserveChangesBehavior = False
                    dal.Database.Connection.Open()
                    dal.Database.ExecuteSqlCommand("SET ARITHABORT ON;")

                    'TODO: devo aprire anche la transazione (dipende se l'aggiornamento viene fatto con Sql oppure con EF)!!!





                    Dim anagAcconto = (From anag In dal.AnagAccontiLiquidazioni_CampionamentoConferito
                                       Where anag.Piva_SuperUser = pivaSuperUser AndAlso
                                             anag.PIVA = piva AndAlso anag.id_anagrafica = IDAnag
                                       Select anag).FirstOrDefault()

                    If anagAcconto Is Nothing Then
                        Throw New Exception("Non è stato possibile trovare l'elemento da modificare")
                    End If

                    'SOLO Se è di tipo liquidazione:
                    If anagAcconto.tipo_anagrafica = "L" Then

                        Dim listDettagli = (From camp In dal.Liquid_Mov_CampionamentoConferito
                                            Where camp.Piva_SuperUser = anagAcconto.Piva_SuperUser AndAlso
                                                  camp.PIVA = anagAcconto.PIVA AndAlso
                                                  camp.Id_acconto_liquidazione = anagAcconto.id_anagrafica
                                            Select camp).ToList()

                        If listDettagli IsNot Nothing AndAlso listDettagli.Count > 0 Then

                            'Per evitare di fare queste letture ad ogni righe le faccio solo una volta e (per l'iva) mi passo l'intera tabella
                            Dim oGenerazione = (From o In dal.OGenerazioni_Anagrafe_Moduli_Log
                                                Where o.Piva_SuperUser = pivaSuperUser AndAlso
                                                      o.Piva = piva
                                                Select o).FirstOrDefault()
                            Dim moduloGias As Integer = If(oGenerazione Is Nothing, 0, oGenerazione.Modulo_Generazione)

                            Dim listaIva As List(Of IVA_Aliquote) = (From iv In dal.IVA_Aliquote Select iv).ToList()

                            For Each dettaglio In listDettagli

                                'TODO: devo impostare il prezzo unitario sulle varie righe di movimenti dettagli (dove Prezzo_Unitario = 0) a partire da Liquid_Mov_CampionamentoConferito

                                Dim prezzoDaImpostare As Decimal = dettaglio.PrezzoTotaleAlKg

                                If dettaglio.perc_valore_acconto <> 100D Then
                                    prezzoDaImpostare = ArrotondaVal_6((dettaglio.PrezzoTotaleAlKg * dettaglio.perc_valore_acconto) / 100D)
                                End If

                                Dim prezzoDaRigaConferim As Integer = dettaglio.Prezzo_da_riga_conferim

                                Dim msgError As String = ""
                                msgError = ContabilitaHelper_Dettaglio.AggiornaDettagliEconomiciDaPrezzoKg(dettaglio.PIVA, dettaglio.Id_Mov_Det,
                                                                                                           prezzoDaImpostare, True,
                                                                                                           prezzoDaRigaConferim,
                                                                                                           objParametri, dal,
                                                                                                           dettaglio.Cod_Iva,
                                                                                                           moduloGias,
                                                                                                           listaIva)

                                If msgError <> "" Then
                                    Throw New Exception(msgError & " " & AgronicaCoreModelloRes.EseguiDiNuovoCalcoloLiquidazione)
                                End If

                                ' Controllo se la stessa riga di accettazione è presente in altre liquidazioni provvisorie o acconto provvisori collegati ad altre liquidazioni
                                ' Se sì cancello le altre (n.b. in acconti no perché è normale avere la stessa riga in più acconti / liquidazioni)
                                ' Si rende necessario perché si potrebbero avere pià simulazioni di acconto parallelo
                                Dim listAltri_FattVariaz_Liquid_ToDelete = (From camp2 In dal.Liquid_Mov_FattVariaz_CampionamentoConferito
                                                                            Join anag2 In dal.AnagAccontiLiquidazioni_CampionamentoConferito.Where(Function(a) a.definitivo <> 1 And (a.tipo_anagrafica = "L" Or a.id_liquidazione_riferimento <> IDAnag))
                                                                             On camp2.Piva_SuperUser Equals anag2.Piva_SuperUser And
                                                                                camp2.PIVA Equals anag2.PIVA And
                                                                                camp2.Id_acconto_liquidazione Equals anag2.id_anagrafica
                                                                            Where camp2.Piva_SuperUser = anagAcconto.Piva_SuperUser AndAlso
                                                                                        camp2.PIVA = anagAcconto.PIVA AndAlso
                                                                                        camp2.Id_Mov_Det = dettaglio.Id_Mov_Det AndAlso camp2.Id_acconto_liquidazione <> IDAnag
                                                                            Select camp2).ToList()
                                For Each toDel As Liquid_Mov_FattVariaz_CampionamentoConferito In listAltri_FattVariaz_Liquid_ToDelete
                                    dal.Liquid_Mov_FattVariaz_CampionamentoConferito.Attach(toDel)
                                    dal.Liquid_Mov_FattVariaz_CampionamentoConferito.Remove(toDel)
                                Next

                                Dim listAltri_PerCalibro_Liquid_ToDelete = (From camp2 In dal.Liquid_Mov_PerCalibro_CampionamentoConferito
                                                                            Join anag2 In dal.AnagAccontiLiquidazioni_CampionamentoConferito.Where(Function(a) a.definitivo <> 1 And (a.tipo_anagrafica = "L" Or a.id_liquidazione_riferimento <> IDAnag))
                                                                             On camp2.Piva_SuperUser Equals anag2.Piva_SuperUser And
                                                                                camp2.PIVA Equals anag2.PIVA And
                                                                                camp2.Id_acconto_liquidazione Equals anag2.id_anagrafica
                                                                            Where camp2.Piva_SuperUser = anagAcconto.Piva_SuperUser AndAlso
                                                                                        camp2.PIVA = anagAcconto.PIVA AndAlso
                                                                                        camp2.Id_Mov_Det = dettaglio.Id_Mov_Det AndAlso camp2.Id_acconto_liquidazione <> IDAnag
                                                                            Select camp2).ToList()
                                For Each toDel As Liquid_Mov_PerCalibro_CampionamentoConferito In listAltri_PerCalibro_Liquid_ToDelete
                                    dal.Liquid_Mov_PerCalibro_CampionamentoConferito.Attach(toDel)
                                    dal.Liquid_Mov_PerCalibro_CampionamentoConferito.Remove(toDel)
                                Next

                                Dim listAltri_Mov_Liquid_ToDelete = (From camp2 In dal.Liquid_Mov_CampionamentoConferito
                                                                     Join anag2 In dal.AnagAccontiLiquidazioni_CampionamentoConferito.Where(Function(a) a.definitivo <> 1 And (a.tipo_anagrafica = "L" Or a.id_liquidazione_riferimento <> IDAnag))
                                                                     On camp2.Piva_SuperUser Equals anag2.Piva_SuperUser And
                                                                        camp2.PIVA Equals anag2.PIVA And
                                                                        camp2.Id_acconto_liquidazione Equals anag2.id_anagrafica
                                                                     Where camp2.Piva_SuperUser = anagAcconto.Piva_SuperUser AndAlso
                                                                                        camp2.PIVA = anagAcconto.PIVA AndAlso
                                                                                        camp2.Id_Mov_Det = dettaglio.Id_Mov_Det AndAlso camp2.Id_acconto_liquidazione <> IDAnag
                                                                     Select camp2).ToList()
                                For Each toDel As Liquid_Mov_CampionamentoConferito In listAltri_Mov_Liquid_ToDelete
                                    dal.Liquid_Mov_CampionamentoConferito.Attach(toDel)
                                    dal.Liquid_Mov_CampionamentoConferito.Remove(toDel)
                                Next

                            Next

                        End If


                    End If

                    'In ogni caso: dopo aver fatto imposto a 1 il campo definitivo in AnagAccontiLiquidazioni

                    anagAcconto.definitivo = 1

                    anagAcconto.Username_Modifica = objParametri.UsernameOperazione
                    anagAcconto.Data_Modifica = Now


                    dal.AnagAccontiLiquidazioni_CampionamentoConferito.Attach(anagAcconto)
                    dal.Entry(anagAcconto).State = EntityState.Modified
                    dal.SaveChanges()
                    'EF6: AcceptAllChanges (Accepts the changes on all associated entries in the ObjectStateManager so their resultant state is either unchanged or detached.
                    '                       This method iterates all the ObjectStateEntry objects within the ObjectStateManager that are Added or Modified, and then sets the state of the entry to Unchanged. The Deleted items become detached.)
                    'Qui avrebbe senso fare questa operazione solo se è stato disabilitato il comportamento di default del SaveChanges(che internamente chiama AcceptAllChanges)
                    'ma visto che è stato chiamato il SaveChanges default, non ha senso farla perché SaveChanges() == SaveChanges(SaveOptions.DetectChangesBeforeSave | SaveOptions.AcceptAllChangesAfterSave)
                    'dal.AcceptAllChanges()

                    'If execution reaches here, it indicates the successful completion of all the save operation. hence commit the transaction.
                    ts.Complete()

                    xRisp = True

                Catch ex As Exception
                    'If any exception is caught, roll back the entire transaction and ends the transaction scope
                    ts.Dispose()
                    messaggioErrore = ex.Message & If(ex.InnerException Is Nothing, "", " [" & ex.InnerException.Message & "]")
                Finally
                    'Close the opened connection
                    If dal IsNot Nothing AndAlso dal.Database.Connection.State = ConnectionState.Open Then
                        dal.Database.Connection.Close()
                    End If
                End Try

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message & If(ex.InnerException Is Nothing, "", " [" & ex.InnerException.Message & "]")
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Riapri_AnagAccontiLiquidazioni(ByVal piva As String,
                                                             ByVal IDAnag As Integer,
                                                             ByRef objParametri As AgronicaCoreParametri,
                                                             ByRef messaggioErrore As String
                                                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreModello.FF_CampionamentoConferimento.Riapri_AnagAccontiLiquidazioni()"
        Dim xRisp As Boolean = False

        Dim pivaSuperUser As String = objParametri.PivaSuperUser

        Try

            Dim gefutils As New Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Dim dal As Gias_DeveloperServer_Entities = Nothing

            Dim transOptions As New TransactionOptions With {.IsolationLevel = IsolationLevel.ReadCommitted}
            Using ts As New TransactionScope(TransactionScopeOption.Required, transOptions)

                Try

                    dal = New Gias_DeveloperServer_Entities(efConnString)

                    'dal.ContextOptions.UseLegacyPreserveChangesBehavior = False
                    dal.Database.Connection.Open()
                    dal.Database.ExecuteSqlCommand("SET ARITHABORT ON;")

                    'TODO: devo aprire anche la transazione (dipende se l'aggiornamento viene fatto con Sql oppure con EF)!!!

                    Dim anagAcconto = (From anag In dal.AnagAccontiLiquidazioni_CampionamentoConferito
                                       Where anag.Piva_SuperUser = pivaSuperUser AndAlso
                                             anag.PIVA = piva AndAlso anag.id_anagrafica = IDAnag
                                       Select anag).FirstOrDefault()

                    If anagAcconto Is Nothing Then
                        Throw New Exception("Non è stato possibile trovare l'elemento da modificare")
                    End If

                    'SOLO Se è di tipo liquidazione:
                    If anagAcconto.tipo_anagrafica = "L" AndAlso
                        anagAcconto.definitivo = 1 Then

                        Dim listDettagli = (From camp In dal.Liquid_Mov_CampionamentoConferito
                                            Where camp.Piva_SuperUser = anagAcconto.Piva_SuperUser AndAlso
                                                  camp.PIVA = anagAcconto.PIVA AndAlso
                                                  camp.Id_acconto_liquidazione = anagAcconto.id_anagrafica AndAlso
                                                  camp.Prezzo_da_riga_conferim = 0
                                            Select camp).ToList()

                        If listDettagli IsNot Nothing AndAlso listDettagli.Count > 0 Then

                            'Per evitare di fare queste letture ad ogni righe le faccio solo una volta e (per l'iva) mi passo l'intera tabella
                            Dim oGenerazione = (From o In dal.OGenerazioni_Anagrafe_Moduli_Log
                                                Where o.Piva_SuperUser = pivaSuperUser AndAlso
                                                      o.Piva = piva
                                                Select o).FirstOrDefault()
                            Dim moduloGias As Integer = If(oGenerazione Is Nothing, 0, oGenerazione.Modulo_Generazione)

                            Dim listaIva As List(Of IVA_Aliquote) = (From iv In dal.IVA_Aliquote Select iv).ToList()

                            For Each dettaglio In listDettagli

                                'devo riportare a zero il prezzo unitario a meno che non fosse già presenti sulla riga di conferimento

                                Dim prezzoDaImpostare As Decimal = 0D

                                Dim prezzoDaRigaConferim As Integer = dettaglio.Prezzo_da_riga_conferim

                                Dim msgError As String = ""
                                msgError = ContabilitaHelper_Dettaglio.AggiornaDettagliEconomiciDaPrezzoKg(dettaglio.PIVA, dettaglio.Id_Mov_Det,
                                                                                                           prezzoDaImpostare, False,
                                                                                                           prezzoDaRigaConferim,
                                                                                                           objParametri, dal,
                                                                                                           dettaglio.Cod_Iva,
                                                                                                           moduloGias,
                                                                                                           listaIva, riaperturaLiquidazione:=True)

                                If msgError <> "" Then
                                    Throw New Exception(msgError & " " & AgronicaCoreModelloRes.EseguiDiNuovoCalcoloLiquidazione)
                                End If

                            Next

                        End If


                    End If

                    'In ogni caso: dopo aver fatto imposto a 0 il campo definitivo in AnagAccontiLiquidazioni
                    anagAcconto.definitivo = 0

                    anagAcconto.Username_Modifica = objParametri.UsernameOperazione
                    anagAcconto.Data_Modifica = Now


                    dal.AnagAccontiLiquidazioni_CampionamentoConferito.Attach(anagAcconto)
                    dal.Entry(anagAcconto).State = EntityState.Modified
                    dal.SaveChanges()
                    'EF6: AcceptAllChanges (Accepts the changes on all associated entries in the ObjectStateManager so their resultant state is either unchanged or detached.
                    '                       This method iterates all the ObjectStateEntry objects within the ObjectStateManager that are Added or Modified, and then sets the state of the entry to Unchanged. The Deleted items become detached.)
                    'Qui avrebbe senso fare questa operazione solo se è stato disabilitato il comportamento di default del SaveChanges(che internamente chiama AcceptAllChanges)
                    'ma visto che è stato chiamato il SaveChanges default, non ha senso farla perché SaveChanges() == SaveChanges(SaveOptions.DetectChangesBeforeSave | SaveOptions.AcceptAllChangesAfterSave)
                    'dal.AcceptAllChanges()

                    'If execution reaches here, it indicates the successful completion of all the save operation. hence commit the transaction.
                    ts.Complete()

                    xRisp = True

                Catch ex As Exception
                    'If any exception is caught, roll back the entire transaction and ends the transaction scope
                    ts.Dispose()
                    messaggioErrore = ex.Message & If(ex.InnerException Is Nothing, "", " [" & ex.InnerException.Message & "]")
                Finally
                    'Close the opened connection
                    If dal IsNot Nothing AndAlso dal.Database.Connection.State = ConnectionState.Open Then
                        dal.Database.Connection.Close()
                    End If
                End Try

            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message & If(ex.InnerException Is Nothing, "", " [" & ex.InnerException.Message & "]")
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
