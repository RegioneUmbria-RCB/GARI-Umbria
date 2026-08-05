Imports System.Transactions
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModello.Anagrafe
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Data.Entity
Imports System.Text
Imports Newtonsoft.Json

Public Class G2GImpianti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Nuovo_Impianti_G2G(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String, ByVal To_Sa_Cod As Integer, ByVal To_Progressivo As Integer) As G2G_Impianti

        Dim g2g As New G2G_Impianti
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .To_Sa_Cod = To_Sa_Cod
            .To_Progressivo = To_Progressivo
            .ImpiantiToInsert = New List(Of G2G_Impianto)
            .ImpiantiToUpdate = New List(Of G2G_Impianto)
            .ImpiantiToDelete = New List(Of G2G_Impianto)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Nuovo_Impianti_G2GReverse(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String, ByVal To_Sa_Cod As Integer, ByVal To_Progressivo As Integer) As G2G_Impianti_Reverse

        Dim g2g As New G2G_Impianti_Reverse
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .To_Sa_Cod = To_Sa_Cod
            .To_Progressivo = To_Progressivo
            .ImpiantiToInsert = New List(Of G2G_Impianto)
            .ImpiantiToUpdate = New List(Of G2G_Impianto)
            .ImpiantiToDelete = New List(Of G2G_Impianto)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Leggi_Impianti_G2G(ByVal Sa_Cod As Integer, ByRef Impianti As List(Of Impianto_Colturale), ByRef g2g As G2G_Impianti, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef Log_Import As StringBuilder) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GImpianti_R.Leggi_Impianti_G2G()"
        Dim From_Piva = g2g.From_Piva
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Data = objParametri.FinestraTemporaleInizio
        Dim To_Data = objParametri.FinestraTemporaleFine

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaImpiantiInsert = (
                From i In GiasContext.Reg_Impianti
                Where i.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse i.SA_COD = Sa_Cod) AndAlso
                    i.Validita_Inizio <= To_Data AndAlso i.Validita_Fine >= From_Data AndAlso
                    Not GiasContext.G2G_Recode_Impianti.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = i.PIVA AndAlso g.From_Sa_Cod = i.SA_COD AndAlso
                        g.From_Appezza = i.APPEZZA AndAlso g.From_Id_Reg = i.ID_REG)
                Select i).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Impianti --> listaImpiantiInsert " & listaImpiantiInsert.Count)

            Dim Reg_Impianti_Codici As List(Of AgronicaCoreEntityFramework_POCO.Reg_Impianti_Codici) = (
                From i In GiasContext.Reg_Impianti_Codici
                Where i.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse i.sa_cod = Sa_Cod) AndAlso
                    i.Validita_Inizio <= To_Data AndAlso i.Validita_Fine >= From_Data AndAlso
                    Not GiasContext.G2G_Recode_Impianti.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = i.PIVA AndAlso g.From_Sa_Cod = i.sa_cod AndAlso
                        g.From_Appezza = i.appezza AndAlso g.From_Id_Reg = i.Id_Reg)
                Select i).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Impianti --> Reg_Impianti_Codici " & Reg_Impianti_Codici.Count)

            For Each impianto In listaImpiantiInsert
                Dim filtroImpianto As Boolean = (From i In Impianti Where i.Piva = impianto.PIVA And i.Sa_Cod = impianto.SA_COD And i.Appezza = impianto.APPEZZA And i.ID_Reg = impianto.ID_REG).ToList.Count > 0
                If Impianti.Count = 0 OrElse filtroImpianto Then
                    Dim codici = (From c In Reg_Impianti_Codici Where c.PIVA = impianto.PIVA AndAlso c.sa_cod = impianto.SA_COD AndAlso c.appezza = impianto.APPEZZA AndAlso c.Id_Reg = impianto.ID_REG AndAlso c.Progetto_Cod = 0 Select c).ToList()
                    g2g.ImpiantiToInsert.Add(New G2G_Impianto With {.Impianto = impianto, .Codici = codici})
                End If
            Next

            Dim listaImpiantiUpdate = (
                From i In GiasContext.Reg_Impianti
                Where i.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse i.SA_COD = Sa_Cod) AndAlso
                    i.Validita_Inizio <= To_Data AndAlso i.Validita_Fine >= From_Data AndAlso
                    (GiasContext.G2G_Recode_Impianti.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = i.PIVA AndAlso g.From_Sa_Cod = i.SA_COD AndAlso
                        g.From_Appezza = i.APPEZZA AndAlso g.From_Id_Reg = i.ID_REG AndAlso g.datainvio < i.Data_Modifica) OrElse
                    (From ic In GiasContext.Reg_Impianti_Codici
                     Where ic.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse ic.sa_cod = Sa_Cod) AndAlso ic.Progetto_Cod = 0 AndAlso
                     GiasContext.G2G_Recode_Impianti.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = ic.PIVA AndAlso g.From_Sa_Cod = ic.sa_cod AndAlso g.From_Appezza = ic.appezza AndAlso g.From_Id_Reg = ic.Id_Reg AndAlso g.datainvio < ic.Data_Modifica)
                     Select ic).Any(Function(x) x.PIVA = i.PIVA AndAlso x.sa_cod = i.SA_COD AndAlso x.appezza = i.APPEZZA AndAlso x.Id_Reg = i.ID_REG))
                Select i).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Impianti --> listaImpiantiUpdate " & listaImpiantiUpdate.Count)

            For Each impianto In listaImpiantiUpdate
                Dim codici = (From c In GiasContext.Reg_Impianti_Codici Where c.PIVA = impianto.PIVA AndAlso c.sa_cod = impianto.SA_COD AndAlso c.appezza = impianto.APPEZZA AndAlso c.Id_Reg = impianto.ID_REG AndAlso c.Progetto_Cod = 0 Select c).ToList()
                g2g.ImpiantiToUpdate.Add(New G2G_Impianto With {.Impianto = impianto, .Codici = codici})
            Next

            g2g.Recode.G2GRecodeImpiantiToDelete = (
                From g In GiasContext.G2G_Recode_Impianti
                Where g.From_PivaSuperUser = From_PivaSuperUser AndAlso g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse g.From_Sa_Cod = Sa_Cod) AndAlso
                      Not GiasContext.Reg_Impianti.Any(Function(i) i.PIVA = g.From_Piva AndAlso i.SA_COD = g.From_Sa_Cod AndAlso i.APPEZZA = g.From_Appezza AndAlso i.ID_REG = g.From_Id_Reg)
                Select g).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Impianti --> G2GRecodeImpiantiToDelete " & g2g.Recode.G2GRecodeImpiantiToDelete.Count)

        End Using

        Return g2g.ImpiantiToInsert.Count > 0 OrElse g2g.ImpiantiToUpdate.Count > 0 OrElse g2g.Recode.G2GRecodeImpiantiToDelete.Count > 0

    End Function

    Public Function Leggi_Impianti_G2GReverse(ByVal Sa_Cod As Integer, ByRef Impianti As List(Of Impianto_Colturale), ByRef g2g As G2G_Impianti_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GImpianti_R.Leggi_Impianti_G2G()"
        Dim From_Piva = g2g.From_Piva
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Data = objParametri.FinestraTemporaleInizio
        Dim To_Data = objParametri.FinestraTemporaleFine

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaImpiantiInsert = (
                From i In GiasContext.Reg_Impianti
                Where i.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse i.SA_COD = Sa_Cod) AndAlso
                    i.Validita_Inizio <= To_Data AndAlso i.Validita_Fine >= From_Data AndAlso
                    Not GiasContext.G2G_Recode_Impianti.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = i.PIVA AndAlso g.To_Sa_Cod = i.SA_COD AndAlso
                        g.To_Appezza = i.APPEZZA AndAlso g.To_Id_Reg = i.ID_REG)
                Select i).ToList()

            For Each impianto In listaImpiantiInsert
                Dim filtroImpianto As Boolean = (From i In Impianti Where i.Piva = impianto.PIVA And i.Sa_Cod = impianto.SA_COD And i.Appezza = impianto.APPEZZA And i.ID_Reg = impianto.ID_REG).ToList.Count > 0
                If Impianti.Count = 0 OrElse filtroImpianto Then
                    Dim codici = (From c In GiasContext.Reg_Impianti_Codici Where c.PIVA = impianto.PIVA AndAlso c.sa_cod = impianto.SA_COD AndAlso c.appezza = impianto.APPEZZA AndAlso c.Id_Reg = impianto.ID_REG AndAlso c.Progetto_Cod = 0 Select c).ToList()
                    g2g.ImpiantiToInsert.Add(New G2G_Impianto With {.Impianto = impianto, .Codici = codici})
                End If
            Next

            Dim listaImpiantiUpdate = (
                From i In GiasContext.Reg_Impianti
                Where i.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse i.SA_COD = Sa_Cod) AndAlso
                    i.Validita_Inizio <= To_Data AndAlso i.Validita_Fine >= From_Data AndAlso
                    (GiasContext.G2G_Recode_Impianti.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = i.PIVA AndAlso g.To_Sa_Cod = i.SA_COD AndAlso
                        g.To_Appezza = i.APPEZZA AndAlso g.To_Id_Reg = i.ID_REG AndAlso g.datainvio < i.Data_Modifica) OrElse
                    (From ic In GiasContext.Reg_Impianti_Codici
                     Where ic.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse ic.sa_cod = Sa_Cod) AndAlso ic.Progetto_Cod = 0 AndAlso
                     GiasContext.G2G_Recode_Impianti.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = ic.PIVA AndAlso g.To_Sa_Cod = ic.sa_cod AndAlso g.To_Appezza = ic.appezza AndAlso g.To_Id_Reg = ic.Id_Reg AndAlso g.datainvio < ic.Data_Modifica)
                     Select ic).Any(Function(x) x.PIVA = i.PIVA AndAlso x.sa_cod = i.SA_COD AndAlso x.appezza = i.APPEZZA AndAlso x.Id_Reg = i.ID_REG))
                Select i).ToList()

            For Each impianto In listaImpiantiUpdate
                Dim codici = (From c In GiasContext.Reg_Impianti_Codici Where c.PIVA = impianto.PIVA AndAlso c.sa_cod = impianto.SA_COD AndAlso c.appezza = impianto.APPEZZA AndAlso c.Id_Reg = impianto.ID_REG AndAlso c.Progetto_Cod = 0 Select c).ToList()
                g2g.ImpiantiToUpdate.Add(New G2G_Impianto With {.Impianto = impianto, .Codici = codici})
            Next

            g2g.Recode.G2GRecodeImpiantiToDelete = (
                From g In GiasContext.G2G_Recode_Impianti
                Where g.From_PivaSuperUser = From_PivaSuperUser AndAlso g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse g.To_Sa_Cod = Sa_Cod) AndAlso
                      Not GiasContext.Reg_Impianti.Any(Function(i) i.PIVA = g.From_Piva AndAlso i.SA_COD = g.To_Sa_Cod AndAlso i.APPEZZA = g.To_Appezza AndAlso i.ID_REG = g.To_Id_Reg)
                Select g).ToList()

        End Using

        Return g2g.ImpiantiToInsert.Count > 0 OrElse g2g.ImpiantiToUpdate.Count > 0 OrElse g2g.Recode.G2GRecodeImpiantiToDelete.Count > 0

    End Function


End Class


Public Class G2GImpianti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Impianti_G2G(ByRef g2g As G2G_Impianti, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GImpianti_W.Scrivi_Impianti_G2G()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_Impianti(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_Impianti(g2g, objParametri)

    End Function

    Public Function Scrivi_Impianti_G2GReverse(ByRef g2g As G2G_Impianti_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GImpianti_W.Scrivi_Impianti_G2GReverse()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_ImpiantiReverse(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_ImpiantiReverse(g2g, objParametri)

    End Function

    Public Function Scrivi_Impianti(ByRef g2g As G2G_Impianti, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GImpianti_W.Scrivi_Impianti()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva
        Dim To_Sa_Cod = g2g.To_Sa_Cod

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim BaseCode As Long = 0
        Dim TopCode As Long = 2000000000
        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        objSequenze.Calcola_BaseCode(g2g.To_Progressivo, TopCode, BaseCode, objParametri)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' IMPIANTI DA INSERIRE
                If g2g.ImpiantiToInsert.Count > 0 Then

                    Dim index As Integer = 0
                    Dim listReg_Impianti As New List(Of AgronicaCoreEntityFramework_POCO.Reg_Impianti)
                    Dim listReg_Impianti_Codici As New List(Of AgronicaCoreEntityFramework_POCO.Reg_Impianti_Codici)
                    Dim listG2G_Recode_Impianti As New List(Of AgronicaCoreEntityFramework_POCO.G2G_Recode_Impianti)

                    Dim listG2G_Recode_Campo As List(Of AgronicaCoreEntityFramework_POCO.G2G_Recode_Campo)
                    Dim listG2G_Recode_Appezzamento As List(Of AgronicaCoreEntityFramework_POCO.G2G_Recode_Appezzamenti)
                    Dim prat As New AgronicaCoreG2GLocalDal.G2GPratiche_R

                    Dim listG2G_Recode_Impianti_GiaInseriti As List(Of AgronicaCoreEntityFramework_POCO.G2G_Recode_Impianti)

                    Dim firstFromPiva = g2g.ImpiantiToInsert(0).Impianto.PIVA
                    listG2G_Recode_Impianti_GiaInseriti = (From rr In GiasContext.G2G_Recode_Impianti
                                                           Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso
                                                               rr.To_PivaSuperUser = To_PivaSuperUser AndAlso
                                                               rr.From_Piva = firstFromPiva).ToList()

                    For Each i As G2G_Impianto In g2g.ImpiantiToInsert

                        If listG2G_Recode_Campo Is Nothing Then
                            listG2G_Recode_Campo = (From rr In GiasContext.G2G_Recode_Campo
                                                    Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso
                                                        rr.To_PivaSuperUser = To_PivaSuperUser AndAlso
                                                        rr.From_Piva = i.Impianto.PIVA).ToList
                        End If

                        If listG2G_Recode_Appezzamento Is Nothing Then
                            listG2G_Recode_Appezzamento = (From rr In GiasContext.G2G_Recode_Appezzamenti
                                                           Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso
                                                               rr.To_PivaSuperUser = To_PivaSuperUser AndAlso
                                                               rr.From_Piva = i.Impianto.PIVA).ToList()
                        End If

                        Dim To_Campo_Cod As Integer = 0
                        If i.Impianto.ID_CAMPO <> 0 Then
                            Dim list_recode_Campi = (From rr In listG2G_Recode_Campo Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = i.Impianto.PIVA AndAlso rr.From_Sa_Cod = i.Impianto.SA_COD AndAlso rr.From_Campo_cod = i.Impianto.ID_CAMPO Select (rr.To_Campo_cod)).ToList()
                            If list_recode_Campi.Count = 0 Then
                                Throw New Exception("Recode campo non trovato --> Piva:" & i.Impianto.PIVA & " Sa_Cod:" & i.Impianto.SA_COD & "  Campo_Cod:" & i.Impianto.ID_CAMPO)
                            End If
                            If list_recode_Campi.Count > 1 Then
                                Throw New Exception("Recode Multipli per campo --> Piva:" & i.Impianto.PIVA & " Sa_Cod:" & i.Impianto.SA_COD & "  Campo_Cod:" & i.Impianto.ID_CAMPO)
                            End If
                            To_Campo_Cod = list_recode_Campi(0)
                        End If

                        Dim list_recode_appezza = (From rr In listG2G_Recode_Appezzamento Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = i.Impianto.PIVA AndAlso rr.From_Sa_Cod = i.Impianto.SA_COD AndAlso rr.From_Appezza = i.Impianto.APPEZZA Select (rr.To_Appezza)).ToList()
                        If list_recode_appezza.Count = 0 Then
                            Throw New Exception("Appezzamento non presente in destinazione")
                        End If
                        If list_recode_appezza.Count > 1 Then
                            Throw New Exception("Presenti più recode in destinazione per lo stesso appezzamento")
                        End If
                        Dim To_Appezza As Integer = list_recode_appezza(0)

                        Dim recodeEsistenti = (From r In listG2G_Recode_Impianti_GiaInseriti Where r.From_Piva = i.Impianto.PIVA And
                                                                                              r.From_Sa_Cod = i.Impianto.SA_COD AndAlso
                                                                                              r.From_Appezza = i.Impianto.APPEZZA AndAlso
                                                                                              r.From_Id_Reg = i.Impianto.ID_REG).ToList

                        If recodeEsistenti.Count = 0 Then
                            ' nuovo impianto
                            Dim idSeq = objSequenze.NuovoId_Reg_Impianti(To_Piva, To_Sa_Cod, To_Appezza, BaseCode, TopCode, objParametri)
                            Dim impianto = Gias_EF_Utility.CopyEntity(GiasContext, i.Impianto, Nothing, username, data)
                            impianto.USER = To_PivaSuperUser
                            impianto.PIVA = To_Piva
                            impianto.SA_COD = To_Sa_Cod
                            impianto.APPEZZA = To_Appezza
                            impianto.ID_REG = idSeq
                            impianto.ID_CAMPO = To_Campo_Cod
                            'GiasContext.Reg_Impianti.Add(impianto)
                            listReg_Impianti.Add(impianto)

                            ' inserisce codici impianto
                            For Each cc As Reg_Impianti_Codici In i.Codici
                                Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                                codice.PIVA = impianto.PIVA
                                codice.sa_cod = impianto.SA_COD
                                codice.appezza = impianto.APPEZZA
                                codice.Id_Reg = impianto.ID_REG
                                'sistemazione codice 1298 che contiene le pratiche
                                If cc.id_cod = enum_CodiciAnagrafe.Programmazione_Impianto_Pratica_Cod Then
                                    Dim newStrPratiche As String = prat.ricodifica_Pratica_Cod(GiasContext, From_PivaSuperUser, cc.val_cod)
                                    If Not String.IsNullOrEmpty(newStrPratiche) Then
                                        codice.val_cod = newStrPratiche
                                    End If
                                End If
                                'GiasContext.Reg_Impianti_Codici.Add(codice)
                                listReg_Impianti_Codici.Add(codice)
                            Next

                            'nuovo recode impianto
                            Dim recode =
                        New G2G_Recode_Impianti With {
                            .From_PivaSuperUser = From_PivaSuperUser,
                            .To_PivaSuperUser = To_PivaSuperUser,
                            .From_Piva = i.Impianto.PIVA,
                            .To_Piva = impianto.PIVA,
                            .From_Sa_Cod = i.Impianto.SA_COD,
                            .To_Sa_Cod = impianto.SA_COD,
                            .From_Appezza = i.Impianto.APPEZZA,
                            .To_Appezza = impianto.APPEZZA,
                            .From_Id_Reg = i.Impianto.ID_REG,
                            .To_Id_Reg = impianto.ID_REG,
                            .Username_Creazione = username,
                            .Username_Modifica = username,
                            .Data_Creazione = Now(),
                            .Data_Modifica = Now(),
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .inviato = 0,
                            .datainvio = Now()
                        }
                            g2g.Recode.G2GRecodeImpiantiToInsert.Add(recode)
                            'GiasContext.G2G_Recode_Impianti.Add(recode)
                            listG2G_Recode_Impianti.Add(recode)

                            'G2GUtility.SaveChanges(GiasContext, index)
                        Else
                            g2g.Recode.G2GRecodeImpiantiToInsert.Add(recodeEsistenti(0))
                            Dim messaggio = "Impianto non importato: " & JsonConvert.SerializeObject(i.Impianto) & vbCrLf &
                                "Recode già presente"
                            Scrivi_LOG(objParametri, nomeRoutine, messaggio)
                        End If

                    Next

                    GiasContext.Reg_Impianti.AddRange(listReg_Impianti)
                    GiasContext.Reg_Impianti_Codici.AddRange(listReg_Impianti_Codici)
                    GiasContext.G2G_Recode_Impianti.AddRange(listG2G_Recode_Impianti)

                    GiasContext.SaveChanges()

                End If

                ' IMPIANTI DA MODIFICARE
                If g2g.ImpiantiToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    ' impianti da modificare
                    For Each i As G2G_Impianto In g2g.ImpiantiToUpdate

                        Dim To_Campo_Cod As Integer = 0
                        If i.Impianto.ID_CAMPO <> 0 Then
                            To_Campo_Cod = (From rr In GiasContext.G2G_Recode_Campo Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = i.Impianto.PIVA AndAlso rr.From_Sa_Cod = i.Impianto.SA_COD AndAlso rr.From_Campo_cod = i.Impianto.ID_CAMPO Select (rr.To_Campo_cod)).FirstOrDefault()
                        End If

                        Dim To_Appezza As Integer = (From rr In GiasContext.G2G_Recode_Appezzamenti Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = i.Impianto.PIVA AndAlso rr.From_Sa_Cod = i.Impianto.SA_COD AndAlso rr.From_Appezza = i.Impianto.APPEZZA Select (rr.To_Appezza)).FirstOrDefault()
                        If To_Appezza = 0 Then
                            Throw New Exception("Appezzamento non presente in destinazione")
                        End If

                        ' modifica recode impianto
                        Dim recode = (From rr In GiasContext.G2G_Recode_Impianti Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = i.Impianto.PIVA AndAlso rr.From_Sa_Cod = i.Impianto.SA_COD AndAlso rr.From_Appezza = i.Impianto.APPEZZA AndAlso rr.From_Id_Reg = i.Impianto.ID_REG).FirstOrDefault()
                        If recode Is Nothing Then
                            Throw New Exception("Recode non presente in destinazione")
                        End If
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodeImpiantiToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Impianti.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        ' modifica impianto
                        Dim impianto = (From ii In GiasContext.Reg_Impianti Where ii.PIVA = recode.To_Piva AndAlso ii.SA_COD = recode.To_Sa_Cod AndAlso ii.APPEZZA = recode.To_Appezza AndAlso ii.ID_REG = recode.To_Id_Reg).FirstOrDefault()
                        If impianto Is Nothing Then
                            Throw New Exception("Impianto non presente in destinazione")
                        End If
                        impianto = Gias_EF_Utility.CopyEntity(GiasContext, i.Impianto, impianto, username, data)
                        impianto.USER = To_PivaSuperUser
                        impianto.PIVA = recode.To_Piva
                        impianto.SA_COD = recode.To_Sa_Cod
                        impianto.APPEZZA = recode.To_Appezza
                        impianto.ID_REG = recode.To_Id_Reg
                        impianto.ID_CAMPO = To_Campo_Cod
                        GiasContext.Reg_Impianti.Attach(impianto)
                        GiasContext.Entry(impianto).State = EntityState.Modified

                        ' cancella codici impianto
                        Dim codici = (From cc In GiasContext.Reg_Impianti_Codici Where cc.PIVA = impianto.PIVA AndAlso cc.sa_cod = impianto.SA_COD AndAlso cc.appezza = impianto.APPEZZA AndAlso cc.Id_Reg = impianto.ID_REG AndAlso cc.Progetto_Cod = 0 Select cc).ToList()
                        For Each cc As Reg_Impianti_Codici In codici
                            GiasContext.Reg_Impianti_Codici.Attach(cc)
                            GiasContext.Reg_Impianti_Codici.Remove(cc)
                        Next

                        ' inserisce codici impianto
                        For Each cc As Reg_Impianti_Codici In i.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = impianto.PIVA
                            codice.sa_cod = impianto.SA_COD
                            codice.appezza = impianto.APPEZZA
                            codice.Id_Reg = impianto.ID_REG
                            'sistemazione codice 1298 che contiene le pratiche
                            If cc.id_cod = enum_CodiciAnagrafe.Programmazione_Impianto_Pratica_Cod Then
                                Dim prat As New AgronicaCoreG2GLocalDal.G2GPratiche_R
                                Dim newStrPratiche As String = prat.ricodifica_Pratica_Cod(GiasContext, From_PivaSuperUser, cc.val_cod)
                                If Not String.IsNullOrEmpty(newStrPratiche) Then
                                    codice.val_cod = newStrPratiche
                                End If
                            End If
                            GiasContext.Reg_Impianti_Codici.Add(codice)
                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If


                ' IMPIANTI DA CANCELLARE
                If g2g.Recode.G2GRecodeImpiantiToDelete.Count > 0 Then

                    Dim index As Integer = 0

                    For Each r As G2G_Recode_Impianti In g2g.Recode.G2GRecodeImpiantiToDelete

                        Dim impianto = (From i In GiasContext.Reg_Impianti Where i.PIVA = r.To_Piva AndAlso i.SA_COD = r.To_Sa_Cod AndAlso i.APPEZZA = r.To_Appezza AndAlso i.ID_REG = r.To_Id_Reg).FirstOrDefault()

                        'cancella impianto
                        If impianto IsNot Nothing Then

                            ' cancella codici impianto
                            Dim codici = (From cc In GiasContext.Reg_Impianti_Codici Where cc.PIVA = impianto.PIVA AndAlso cc.sa_cod = impianto.SA_COD AndAlso cc.appezza = impianto.APPEZZA AndAlso cc.Id_Reg = impianto.ID_REG AndAlso cc.Progetto_Cod = 0 Select cc).ToList()
                            For Each cc As Reg_Impianti_Codici In codici
                                Dim ccEntry = GiasContext.Entry(cc)
                                If ccEntry.State = EntityState.Detached Then
                                    GiasContext.Reg_Impianti_Codici.Attach(cc)
                                End If
                                If ccEntry.State <> EntityState.Deleted Then
                                    GiasContext.Reg_Impianti_Codici.Remove(cc)
                                End If
                            Next

                            'cancella impianto
                            Dim impiantoEntry = GiasContext.Entry(impianto)
                            If impiantoEntry.State = EntityState.Detached Then
                                GiasContext.Reg_Impianti.Attach(impianto)
                            End If
                            If impiantoEntry.State <> EntityState.Deleted Then
                                GiasContext.Reg_Impianti.Remove(impianto)
                            End If

                        End If

                        ' cancella recode impianto - trova l'entità tracciata se esiste
                        Dim recodeToDelete As G2G_Recode_Impianti = r
                        Dim trackedRecode = GiasContext.G2G_Recode_Impianti.Local.FirstOrDefault(Function(x) _
                            x.From_PivaSuperUser = r.From_PivaSuperUser AndAlso
                            x.To_PivaSuperUser = r.To_PivaSuperUser AndAlso
                            x.From_Piva = r.From_Piva AndAlso
                            x.To_Piva = r.To_Piva AndAlso
                            x.From_Sa_Cod = r.From_Sa_Cod AndAlso
                            x.To_Sa_Cod = r.To_Sa_Cod AndAlso
                            x.From_Appezza = r.From_Appezza AndAlso
                            x.To_Appezza = r.To_Appezza AndAlso
                            x.From_Id_Reg = r.From_Id_Reg AndAlso
                            x.To_Id_Reg = r.To_Id_Reg)

                        If trackedRecode IsNot Nothing Then
                            ' Usa l'entità già tracciata
                            recodeToDelete = trackedRecode
                        Else
                            ' Attach solo se non è già tracciata
                            Dim recodeEntry = GiasContext.Entry(r)
                            If recodeEntry.State = EntityState.Detached Then
                                Dim existsInDB = GiasContext.G2G_Recode_Impianti.Any(Function(x) _
                                    x.From_PivaSuperUser = r.From_PivaSuperUser AndAlso
                                    x.To_PivaSuperUser = r.To_PivaSuperUser AndAlso
                                    x.From_Piva = r.From_Piva AndAlso
                                    x.To_Piva = r.To_Piva AndAlso
                                    x.From_Sa_Cod = r.From_Sa_Cod AndAlso
                                    x.To_Sa_Cod = r.To_Sa_Cod AndAlso
                                    x.From_Appezza = r.From_Appezza AndAlso
                                    x.To_Appezza = r.To_Appezza AndAlso
                                    x.From_Id_Reg = r.From_Id_Reg AndAlso
                                    x.To_Id_Reg = r.To_Id_Reg)

                                If existsInDB Then
                                    GiasContext.G2G_Recode_Impianti.Attach(r)
                                Else
                                    ' Il record è già stato eliminato, salta questo elemento
                                    g2g.Recode.LogRecode &= objParametri.Recupera_NomeDB() & " - Recode Impianto già eliminato: " & r.From_PivaSuperUser & " -> " & r.To_PivaSuperUser & " - " & r.From_Piva & " -> " & r.To_Piva & " - " & r.From_Sa_Cod & " -> " & r.To_Sa_Cod & " - " & r.From_Appezza & " -> " & r.To_Appezza & " - " & r.From_Id_Reg & " -> " & r.To_Id_Reg & Environment.NewLine
                                    Continue For
                                End If


                            End If
                        End If

                        Dim finalEntry = GiasContext.Entry(recodeToDelete)
                        If finalEntry.State <> EntityState.Deleted Then
                            GiasContext.G2G_Recode_Impianti.Remove(recodeToDelete)
                        End If

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode &= " - impianti: " & g2g.ImpiantiToInsert.Count & " nuovi, " & g2g.ImpiantiToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeImpiantiToDelete.Count & " cancellati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.ToString
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_ImpiantiReverse(ByRef g2g As G2G_Impianti_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GImpianti_W.Scrivi_Impianti()"
        Dim messaggioErrore As String = ""

        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Piva = g2g.From_Piva
        Dim To_Piva = g2g.To_Piva
        Dim To_Sa_Cod = g2g.To_Sa_Cod

        Dim username = objParametri.UsernameOperazione
        Dim data = Date.Now

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim BaseCode As Long = 0
        Dim TopCode As Long = 2000000000
        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        objSequenze.Calcola_BaseCode(g2g.To_Progressivo, TopCode, BaseCode, objParametri)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' IMPIANTI DA INSERIRE
                If g2g.ImpiantiToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each i As G2G_Impianto In g2g.ImpiantiToInsert

                        Dim To_Campo_Cod As Integer = 0
                        If i.Impianto.ID_CAMPO <> 0 Then
                            To_Campo_Cod = (From rr In GiasContext.G2G_Recode_Campo Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = i.Impianto.PIVA AndAlso rr.To_Sa_Cod = i.Impianto.SA_COD AndAlso rr.To_Campo_cod = i.Impianto.ID_CAMPO Select (rr.From_Campo_cod)).FirstOrDefault()
                        End If

                        Dim To_Appezza As Integer = (From rr In GiasContext.G2G_Recode_Appezzamenti Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = i.Impianto.PIVA AndAlso rr.To_Sa_Cod = i.Impianto.SA_COD AndAlso rr.To_Appezza = i.Impianto.APPEZZA Select (rr.From_Appezza)).FirstOrDefault()
                        If To_Appezza = 0 Then
                            Throw New Exception("Appezzamento non presente in destinazione")
                        End If

                        ' nuovo impianto
                        Dim idSeq = objSequenze.NuovoId_Reg_Impianti(To_Piva, To_Sa_Cod, To_Appezza, BaseCode, TopCode, objParametri)
                        Dim impianto = Gias_EF_Utility.CopyEntity(GiasContext, i.Impianto, Nothing, username, data)
                        impianto.USER = To_PivaSuperUser
                        impianto.PIVA = To_Piva
                        impianto.SA_COD = To_Sa_Cod
                        impianto.APPEZZA = To_Appezza
                        impianto.ID_REG = idSeq
                        impianto.ID_CAMPO = To_Campo_Cod
                        GiasContext.Reg_Impianti.Add(impianto)

                        ' inserisce codici impianto
                        For Each cc As Reg_Impianti_Codici In i.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = impianto.PIVA
                            codice.sa_cod = impianto.SA_COD
                            codice.appezza = impianto.APPEZZA
                            codice.Id_Reg = impianto.ID_REG
                            'sistemazione codice 1298 che contiene le pratiche
                            If cc.id_cod = enum_CodiciAnagrafe.Programmazione_Impianto_Pratica_Cod Then
                                Dim prat As New AgronicaCoreG2GLocalDal.G2GPratiche_R
                                Dim newStrPratiche As String = prat.ricodifica_Pratica_CodReverse(GiasContext, From_PivaSuperUser, cc.val_cod)
                                If Not String.IsNullOrEmpty(newStrPratiche) Then
                                    codice.val_cod = newStrPratiche
                                End If
                            End If
                            GiasContext.Reg_Impianti_Codici.Add(codice)
                        Next

                        'nuovo recode impianto
                        Dim recode =
                        New G2G_Recode_Impianti With {
                            .From_PivaSuperUser = To_PivaSuperUser,
                            .To_PivaSuperUser = From_PivaSuperUser,
                            .From_Piva = impianto.PIVA,
                            .To_Piva = i.Impianto.PIVA,
                            .From_Sa_Cod = impianto.SA_COD,
                            .To_Sa_Cod = i.Impianto.SA_COD,
                            .From_Appezza = impianto.APPEZZA,
                            .To_Appezza = i.Impianto.APPEZZA,
                            .From_Id_Reg = impianto.ID_REG,
                            .To_Id_Reg = i.Impianto.ID_REG,
                            .Username_Creazione = username,
                            .Username_Modifica = username,
                            .Data_Creazione = Now(),
                            .Data_Modifica = Now(),
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .inviato = 0,
                            .datainvio = Now()
                        }
                        g2g.Recode.G2GRecodeImpiantiToInsert.Add(recode)
                        GiasContext.G2G_Recode_Impianti.Add(recode)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' IMPIANTI DA MODIFICARE
                If g2g.ImpiantiToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    ' impianti da modificare
                    For Each i As G2G_Impianto In g2g.ImpiantiToUpdate

                        Dim From_Campo_Cod As Integer = 0
                        If i.Impianto.ID_CAMPO <> 0 Then
                            From_Campo_Cod = (From rr In GiasContext.G2G_Recode_Campo Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = i.Impianto.PIVA AndAlso rr.To_Sa_Cod = i.Impianto.SA_COD AndAlso rr.To_Campo_cod = i.Impianto.ID_CAMPO Select (rr.From_Campo_cod)).FirstOrDefault()
                        End If

                        Dim To_Appezza As Integer = (From rr In GiasContext.G2G_Recode_Appezzamenti Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = i.Impianto.PIVA AndAlso rr.To_Sa_Cod = i.Impianto.SA_COD AndAlso rr.To_Appezza = i.Impianto.APPEZZA Select (rr.From_Appezza)).FirstOrDefault()
                        If To_Appezza = 0 Then
                            Throw New Exception("Appezzamento non presente in destinazione")
                        End If

                        ' modifica recode impianto
                        Dim recode = (From rr In GiasContext.G2G_Recode_Impianti Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = i.Impianto.PIVA AndAlso rr.To_Sa_Cod = i.Impianto.SA_COD AndAlso rr.To_Appezza = i.Impianto.APPEZZA AndAlso rr.To_Id_Reg = i.Impianto.ID_REG).FirstOrDefault()
                        If recode Is Nothing Then
                            Throw New Exception("Recode non presente in destinazione")
                        End If
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodeImpiantiToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Impianti.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        ' modifica impianto
                        Dim impianto = (From ii In GiasContext.Reg_Impianti Where ii.PIVA = recode.From_Piva AndAlso ii.SA_COD = recode.From_Sa_Cod AndAlso ii.APPEZZA = recode.From_Appezza AndAlso ii.ID_REG = recode.From_Id_Reg).FirstOrDefault()
                        If impianto Is Nothing Then
                            Throw New Exception("Impianto non presente in destinazione")
                        End If
                        impianto = Gias_EF_Utility.CopyEntity(GiasContext, i.Impianto, impianto, username, data)
                        impianto.USER = To_PivaSuperUser
                        impianto.PIVA = recode.From_Piva
                        impianto.SA_COD = recode.From_Sa_Cod
                        impianto.APPEZZA = recode.From_Appezza
                        impianto.ID_REG = recode.From_Id_Reg
                        impianto.ID_CAMPO = From_Campo_Cod
                        GiasContext.Reg_Impianti.Attach(impianto)
                        GiasContext.Entry(impianto).State = EntityState.Modified

                        ' cancella codici impianto
                        Dim codici = (From cc In GiasContext.Reg_Impianti_Codici Where cc.PIVA = impianto.PIVA AndAlso cc.sa_cod = impianto.SA_COD AndAlso cc.appezza = impianto.APPEZZA AndAlso cc.Id_Reg = impianto.ID_REG AndAlso cc.Progetto_Cod = 0 Select cc).ToList()
                        For Each cc As Reg_Impianti_Codici In codici
                            GiasContext.Reg_Impianti_Codici.Attach(cc)
                            GiasContext.Reg_Impianti_Codici.Remove(cc)
                        Next

                        ' inserisce codici impianto
                        For Each cc As Reg_Impianti_Codici In i.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = impianto.PIVA
                            codice.sa_cod = impianto.SA_COD
                            codice.appezza = impianto.APPEZZA
                            codice.Id_Reg = impianto.ID_REG
                            'sistemazione codice 1298 che contiene le pratiche
                            If cc.id_cod = enum_CodiciAnagrafe.Programmazione_Impianto_Pratica_Cod Then
                                Dim prat As New AgronicaCoreG2GLocalDal.G2GPratiche_R
                                Dim newStrPratiche As String = prat.ricodifica_Pratica_CodReverse(GiasContext, From_PivaSuperUser, cc.val_cod)
                                If Not String.IsNullOrEmpty(newStrPratiche) Then
                                    codice.val_cod = newStrPratiche
                                End If
                            End If
                            GiasContext.Reg_Impianti_Codici.Add(codice)
                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If


                ' IMPIANTI DA CANCELLARE
                If g2g.Recode.G2GRecodeImpiantiToDelete.Count > 0 Then

                    Dim index As Integer = 0

                    For Each r As G2G_Recode_Impianti In g2g.Recode.G2GRecodeImpiantiToDelete

                        Dim impianto = (From i In GiasContext.Reg_Impianti Where i.PIVA = r.From_Piva AndAlso i.SA_COD = r.From_Sa_Cod AndAlso i.APPEZZA = r.From_Appezza AndAlso i.ID_REG = r.From_Id_Reg).FirstOrDefault()

                        'cancella impianto
                        If impianto IsNot Nothing Then

                            ' cancella codici impianto
                            Dim codici = (From cc In GiasContext.Reg_Impianti_Codici Where cc.PIVA = impianto.PIVA AndAlso cc.sa_cod = impianto.SA_COD AndAlso cc.appezza = impianto.APPEZZA AndAlso cc.Id_Reg = impianto.ID_REG AndAlso cc.Progetto_Cod = 0 Select cc).ToList()
                            For Each cc As Reg_Impianti_Codici In codici
                                GiasContext.Reg_Impianti_Codici.Attach(cc)
                                GiasContext.Reg_Impianti_Codici.Remove(cc)
                            Next

                            'cancella impianto
                            GiasContext.Reg_Impianti.Attach(impianto)
                            GiasContext.Reg_Impianti.Remove(impianto)

                        End If

                        ' cancella recode impianto
                        GiasContext.G2G_Recode_Impianti.Attach(r)
                        GiasContext.G2G_Recode_Impianti.Remove(r)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode &= " - impianti: " & g2g.ImpiantiToInsert.Count & " nuovi, " & g2g.ImpiantiToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeImpiantiToDelete.Count & " cancellati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

End Class