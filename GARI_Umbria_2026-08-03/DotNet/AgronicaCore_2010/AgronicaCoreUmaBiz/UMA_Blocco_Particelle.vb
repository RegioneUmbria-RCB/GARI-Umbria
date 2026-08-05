Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework_POCO
Imports System.Transactions
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreEntityFramework
Imports System.Linq
Imports System.Data.Entity
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUmaDal
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider

Public Class UMA_Blocco_Particelle

    Public Function Inserisci_Da_DataTable(ByVal Piva As String,
                                           ByVal Anno As Integer,
                                           ByVal dt As DataTable,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreUMABIZ.UMA_Blocco_Particelle.Inserisci_Da_DataTable()"
        Dim log As New AgronicaCoreDataProvider.LogProvider
        Dim bloccoW = New AgronicaCoreUmaDal.UMA_Blocco_Particelle_W
        Dim r As AgronicaCoreEntityFramework_POCO.UMA_Blocco_Particelle
        Dim EFArrayToInsert As New List(Of AgronicaCoreEntityFramework_POCO.UMA_Blocco_Particelle)
        Dim Risultato As Integer = 0

        Try
            Dim transactionOptions = New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                    '###########################################################################################################################################
                    '################################### NUOVI INSERIMENTI/MODIFICHE ###########################################################################
                    '###########################################################################################################################################


                    'creo i record da inserire
                    For Each row As DataRow In dt.Rows

                        If Not (IsDBNull(row.Item("COM")) OrElse
                                IsDBNull(row.Item("PROV")) OrElse
                                IsDBNull(row.Item("FOGLIO")) OrElse
                                IsDBNull(row.Item("SEZIONE")) OrElse
                                IsDBNull(row.Item("NUMERO")) OrElse
                                IsDBNull(row.Item("SUBALTERNO"))) Then

                            If EFArrayToInsert.Exists(Function(x) x.Piva = Piva AndAlso x.PROV = CStr(row.Item("PROV")) AndAlso
                                     x.COM = CStr(row.Item("COM")) AndAlso x.FOGLIO = CInt(row.Item("FOGLIO")) AndAlso
                                     x.SEZIONE = CStr(row.Item("SEZIONE")) AndAlso x.NUMERO = row.Item("NUMERO") AndAlso
                                     x.SUBALTERNO = CStr(row.Item("SUBALTERNO")) AndAlso x.Gruppo_Colturale_UMA = CStr(row.Item("Macrouso_UMA_Cod"))) Then

                                Dim found = EFArrayToInsert.Find(Function(x) x.Piva = Piva AndAlso x.PROV = CStr(row.Item("PROV")) AndAlso
                                     x.COM = CStr(row.Item("COM")) AndAlso x.FOGLIO = CInt(row.Item("FOGLIO")) AndAlso
                                     x.SEZIONE = CStr(row.Item("SEZIONE")) AndAlso x.NUMERO = row.Item("NUMERO") AndAlso
                                     x.SUBALTERNO = CStr(row.Item("SUBALTERNO")) AndAlso x.Gruppo_Colturale_UMA = CStr(row.Item("Macrouso_UMA_Cod")))
                                found.Sup_A += CDbl(row.Item("sup_A"))
                                found.Sup_B += CDbl(row.Item("sup_B"))

                            Else

                                r = New AgronicaCoreEntityFramework_POCO.UMA_Blocco_Particelle With {
                                .Piva = Piva,
                                .Piva_SuperUser = objParametri.PivaSuperUser,
                                .Programmazione_Cod = If(Anno < 2025, CStr(row.Item("Programmazione_Cod")), -4),
                                .Gruppo_Colturale_UMA = CStr(row.Item("Macrouso_UMA_Cod")),
                                .Sup_A = CDbl(row.Item("sup_A")),
                                .Sup_B = CDbl(row.Item("sup_B")),
                                .Anno = Anno,
                                .Bloccato = False,
                                .PROV = CStr(row.Item("PROV")),
                                .COM = CStr(row.Item("COM")),
                                .FOGLIO = CInt(row.Item("FOGLIO")),
                                .SEZIONE = CStr(row.Item("SEZIONE")),
                                .NUMERO = row.Item("NUMERO"),
                                .SUBALTERNO = CStr(row.Item("SUBALTERNO")),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .Data_Creazione = Date.Now,
                                .Data_Modifica = Date.Now,
                                .DataInvio = Nothing,
                                .Inviato = 0,
                                .Username_Creazione = objParametri.UtenteUsername,
                                .Username_Modifica = objParametri.UtenteUsername
                                }

                                EFArrayToInsert.Add(r)

                            End If

                        End If

                    Next


                    Risultato = bloccoW.Aggiungi(EFArrayToInsert, objParametri)

                    GiasContext.SaveChanges()

                    If (String.IsNullOrEmpty(MessaggioErrore)) Then
                        ' COMMIT Effettivo
                        scope.Complete()
                    Else
                        ' Rollback
                        scope.Dispose()
                    End If


                End Using
            End Using

        Catch ex As Exception

            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            log.Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception(MessaggioErrore)


        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return Risultato

    End Function

    Public Function Aggiorna_Blocchi_Da_Griglia(ByVal piva As String,
                                                ByVal righeModificate As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeBIZ.UMA_Blocco_Particelle.Aggiorna_Blocchi_Da_Griglia()"
        Dim log As New AgronicaCoreDataProvider.LogProvider
        Dim Jupdate As JArray
        Dim blocchiW = New AgronicaCoreUmaDal.UMA_Blocco_Particelle_W
        'Dim testata As DataobjRiga = testataR.LeggiDaRichiestaCod(Richiesta_Cod, objParametri).objRigas.Item(0)
        Dim r As AgronicaCoreEntityFramework_POCO.UMA_Blocco_Particelle
        Dim EFArrayToUpdate As New ArrayList
        Dim Risultato As Integer = 0

        Try
            Dim transactionOptions = New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                    '###########################################################################################################################################
                    '################################### NUOVI INSERIMENTI/MODIFICHE ###########################################################################
                    '###########################################################################################################################################

                    If righeModificate <> "" AndAlso righeModificate <> "[]" Then

                        Jupdate = JArray.Parse(righeModificate)

                        'creo i record da inserire
                        For Each objRiga As JObject In Jupdate

                            r = New AgronicaCoreEntityFramework_POCO.UMA_Blocco_Particelle With {
                                .Piva = piva,
                                .Piva_SuperUser = objParametri.PivaSuperUser,
                                .Programmazione_Cod = CStr(objRiga.Item("Programmazione_Cod")),
                                .Gruppo_Colturale_UMA = IIf(IsNothing(objRiga.Item("Gruppo_Colturale_UMA")), CStr(objRiga.Item("macrouso_UMA_Cod")), CStr(objRiga.Item("Gruppo_Colturale_UMA"))),
                                .Sup_A = CDbl(objRiga.Item("Sup_A")),
                                .Sup_B = CDbl(objRiga.Item("Sup_B")),
                                .Anno = CInt(objRiga.Item("anno")),
                                .Bloccato = CBool(objRiga.Item("Bloccato")),
                                .PROV = CStr(objRiga.Item("PROV")),
                                .COM = CStr(objRiga.Item("COM")),
                                .FOGLIO = CInt(objRiga.Item("FOGLIO")),
                                .SEZIONE = CStr(objRiga.Item("SEZIONE")),
                                .NUMERO = objRiga.Item("NUMERO"),
                                .SUBALTERNO = CStr(objRiga.Item("SUBALTERNO")),
                                .Validita_Inizio = Date.Now,
                                .Data_Modifica = Date.Now,
                                .Username_Modifica = objParametri.UtenteUsername,
                                .Inviato = 0,
                                .Data_Creazione = Date.Now,
                                .Username_Creazione = objParametri.UtenteUsername,
                                .Validita_Fine = AGRODATAFINE
                                }

                            EFArrayToUpdate.Add(r)

                        Next


                    End If
                    blocchiW.Aggiorna(EFArrayToUpdate, objParametri)

                    GiasContext.SaveChanges()

                    If (String.IsNullOrEmpty(MessaggioErrore)) Then
                        ' COMMIT Effettivo
                        scope.Complete()
                    Else
                        ' Rollback
                        scope.Dispose()
                    End If


                End Using
            End Using

        Catch ex As Exception

            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            log.Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception(MessaggioErrore)


        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return Risultato

    End Function


End Class
