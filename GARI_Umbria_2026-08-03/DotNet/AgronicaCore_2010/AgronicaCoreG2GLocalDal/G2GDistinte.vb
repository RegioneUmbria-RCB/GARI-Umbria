Imports System.Transactions
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModello.Anagrafe
Imports System.Data.Entity
Imports System.Text
Imports Newtonsoft.Json

Public Class G2GDistinte_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Nuovo_Distinte_G2G(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String, ByVal To_Sa_Cod As Integer, ByVal To_Progressivo As Integer) As G2G_Distinte

        Dim g2g As New G2G_Distinte
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .To_Sa_Cod = To_Sa_Cod
            .To_Progressivo = To_Progressivo
            .DistinteToInsert = New List(Of G2G_Distinta)
            .DistinteToUpdate = New List(Of G2G_Distinta)
            .DistinteToDelete = New List(Of G2G_Distinta)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Nuovo_Distinte_G2GReverse(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String, ByVal To_Sa_Cod As Integer, ByVal To_Progressivo As Integer) As G2G_Distinte_Reverse

        Dim g2g As New G2G_Distinte_Reverse
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .To_Sa_Cod = To_Sa_Cod
            .To_Progressivo = To_Progressivo
            .DistinteToInsert = New List(Of G2G_Distinta)
            .DistinteToUpdate = New List(Of G2G_Distinta)
            .DistinteToDelete = New List(Of G2G_Distinta)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Leggi_Distinte_G2G(ByVal Sa_Cod As Integer, ByRef Impianti As List(Of Impianto_Colturale), ByRef g2g As G2G_Distinte, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef Log_Import As StringBuilder) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GDistinte_R.Leggi_Distinte_G2G()"
        Dim From_Piva = g2g.From_Piva
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Data = objParametri.FinestraTemporaleInizio
        Dim To_Data = objParametri.FinestraTemporaleFine

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            GiasContext.Database.CommandTimeout = 3600

            Dim listaDistinteInsert = (
                From d In GiasContext.Imprese_Progetti
                Where d.Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse d.Sa_Cod = Sa_Cod) AndAlso
                    d.Validita_Inizio <= To_Data AndAlso d.Validita_Fine >= From_Data AndAlso
                    Not GiasContext.G2G_Recode_Distinta.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = d.Piva AndAlso g.From_Progetto_cod = d.Progetto_Cod)
                Select d).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Distinte --> listaDistinteInsert " & listaDistinteInsert.Count)

            Dim Reg_Impianti_Codici As List(Of AgronicaCoreEntityFramework_POCO.Reg_Impianti_Codici) = (
                From d In GiasContext.Reg_Impianti_Codici
                Where d.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse d.sa_cod = Sa_Cod) AndAlso
                    d.Validita_Inizio <= To_Data AndAlso d.Validita_Fine >= From_Data AndAlso
                    Not GiasContext.G2G_Recode_Distinta.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = d.PIVA AndAlso g.From_Progetto_cod = d.Progetto_Cod)
                Select d).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Distinte --> Reg_Impianti_Codici " & Reg_Impianti_Codici.Count)

            Dim Imprese_Progetto_Fasi As List(Of AgronicaCoreEntityFramework_POCO.Imprese_Progetto_Fasi) = (
                From d In GiasContext.Imprese_Progetto_Fasi
                Where d.Piva = From_Piva AndAlso
                    d.Validita_Inizio <= To_Data AndAlso d.Validita_Fine >= From_Data AndAlso
                    Not GiasContext.G2G_Recode_Distinta.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = d.Piva AndAlso g.From_Progetto_cod = d.Progetto_Cod)
                Select d).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Distinte --> Imprese_Progetto_Fasi " & Imprese_Progetto_Fasi.Count)

            For Each distinta In listaDistinteInsert
                Dim filtroImpianto As Boolean = (From i In Impianti Where i.Piva = distinta.Piva And i.Sa_Cod = distinta.Sa_Cod And i.Appezza = distinta.Appezza And i.ID_Reg = distinta.Id_Reg).ToList.Count > 0
                If Impianti.Count = 0 OrElse filtroImpianto Then
                    Dim codici = (From c In Reg_Impianti_Codici Where c.PIVA = distinta.Piva AndAlso c.sa_cod = distinta.Sa_Cod AndAlso c.appezza = distinta.Appezza AndAlso c.Id_Reg = distinta.Id_Reg AndAlso c.Progetto_Cod = distinta.Progetto_Cod Select c).ToList()
                    Dim fasi = (From f In Imprese_Progetto_Fasi Where f.Piva = distinta.Piva AndAlso f.Progetto_Cod = distinta.Progetto_Cod Select f).ToList()
                    g2g.DistinteToInsert.Add(New G2G_Distinta With {.Distinta = distinta, .Codici = codici, .Fasi = fasi})
                End If
            Next

            Dim listaDistinteUpdate = (
                From d In GiasContext.Imprese_Progetti
                Where d.Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse d.Sa_Cod = Sa_Cod) AndAlso
                    d.Validita_Inizio <= To_Data AndAlso d.Validita_Fine >= From_Data AndAlso
                    (GiasContext.G2G_Recode_Distinta.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = d.Piva AndAlso g.From_Progetto_cod = d.Progetto_Cod AndAlso g.datainvio < d.Data_Modifica) OrElse
                    (From dd In GiasContext.Reg_Impianti_Codici
                     Where dd.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse dd.sa_cod = Sa_Cod) AndAlso dd.Progetto_Cod <> 0 AndAlso
                     GiasContext.G2G_Recode_Distinta.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = dd.PIVA AndAlso g.From_Progetto_cod = dd.Progetto_Cod AndAlso g.datainvio < dd.Data_Modifica)
                     Select dd).Any(Function(x) x.PIVA = d.Piva AndAlso x.Progetto_Cod = d.Progetto_Cod))
                Select d).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Distinte --> listaDistinteUpdate " & listaDistinteUpdate.Count)

            For Each distinta In listaDistinteUpdate
                Dim codici = (From c In GiasContext.Reg_Impianti_Codici Where c.PIVA = distinta.Piva AndAlso c.sa_cod = distinta.Sa_Cod AndAlso c.appezza = distinta.Appezza AndAlso c.Id_Reg = distinta.Id_Reg AndAlso c.Progetto_Cod = distinta.Progetto_Cod Select c).ToList()
                Dim fasi = (From f In GiasContext.Imprese_Progetto_Fasi Where f.Piva = distinta.Piva AndAlso f.Progetto_Cod = distinta.Progetto_Cod Select f).ToList()
                g2g.DistinteToUpdate.Add(New G2G_Distinta With {.Distinta = distinta, .Codici = codici, .Fasi = fasi})
            Next

            g2g.Recode.G2GRecodeDistinteToDelete = (
                From g In GiasContext.G2G_Recode_Distinta
                Where g.From_PivaSuperUser = From_PivaSuperUser AndAlso g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = From_Piva AndAlso
                      Not GiasContext.Imprese_Progetti.Any(Function(d) d.Piva = g.From_Piva AndAlso d.Progetto_Cod = g.From_Progetto_cod)
                Select g).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Distinte --> G2GRecodeDistinteToDelete " & g2g.Recode.G2GRecodeDistinteToDelete.Count)

        End Using

        Return g2g.DistinteToInsert.Count > 0 OrElse g2g.DistinteToUpdate.Count > 0 OrElse g2g.Recode.G2GRecodeDistinteToDelete.Count > 0

    End Function

    Public Function Leggi_Distinte_G2GReverse(ByVal Sa_Cod As Integer, ByRef Impianti As List(Of Impianto_Colturale), ByRef g2g As G2G_Distinte_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GDistinte_R.Leggi_Distinte_G2G()"
        Dim From_Piva = g2g.From_Piva
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Data = objParametri.FinestraTemporaleInizio
        Dim To_Data = objParametri.FinestraTemporaleFine

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaDistinteInsert = (
                From d In GiasContext.Imprese_Progetti
                Where d.Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse d.Sa_Cod = Sa_Cod) AndAlso
                    d.Validita_Inizio <= To_Data AndAlso d.Validita_Fine >= From_Data AndAlso
                    Not GiasContext.G2G_Recode_Distinta.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = d.Piva AndAlso g.To_Progetto_cod = d.Progetto_Cod)
                Select d).ToList()

            For Each distinta In listaDistinteInsert
                Dim filtroImpianto As Boolean = (From i In Impianti Where i.Piva = distinta.Piva And i.Sa_Cod = distinta.Sa_Cod And i.Appezza = distinta.Appezza And i.ID_Reg = distinta.Id_Reg).ToList.Count > 0
                If Impianti.Count = 0 OrElse filtroImpianto Then
                    Dim codici = (From c In GiasContext.Reg_Impianti_Codici Where c.PIVA = distinta.Piva AndAlso c.sa_cod = distinta.Sa_Cod AndAlso c.appezza = distinta.Appezza AndAlso c.Id_Reg = distinta.Id_Reg AndAlso c.Progetto_Cod = distinta.Progetto_Cod Select c).ToList()
                    Dim fasi = (From f In GiasContext.Imprese_Progetto_Fasi Where f.Piva = distinta.Piva AndAlso f.Progetto_Cod = distinta.Progetto_Cod Select f).ToList()
                    g2g.DistinteToInsert.Add(New G2G_Distinta With {.Distinta = distinta, .Codici = codici, .Fasi = fasi})
                End If
            Next

            Dim listaDistinteUpdate = (
                From d In GiasContext.Imprese_Progetti
                Where d.Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse d.Sa_Cod = Sa_Cod) AndAlso
                    d.Validita_Inizio <= To_Data AndAlso d.Validita_Fine >= From_Data AndAlso
                    (GiasContext.G2G_Recode_Distinta.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = d.Piva AndAlso g.To_Progetto_cod = d.Progetto_Cod AndAlso g.datainvio < d.Data_Modifica) OrElse
                    (From dd In GiasContext.Reg_Impianti_Codici
                     Where dd.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse dd.sa_cod = Sa_Cod) AndAlso dd.Progetto_Cod <> 0 AndAlso
                     GiasContext.G2G_Recode_Distinta.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = dd.PIVA AndAlso g.To_Progetto_cod = dd.Progetto_Cod AndAlso g.datainvio < dd.Data_Modifica)
                     Select dd).Any(Function(x) x.PIVA = d.Piva AndAlso x.Progetto_Cod = d.Progetto_Cod))
                Select d).ToList()

            For Each distinta In listaDistinteUpdate
                Dim codici = (From c In GiasContext.Reg_Impianti_Codici Where c.PIVA = distinta.Piva AndAlso c.sa_cod = distinta.Sa_Cod AndAlso c.appezza = distinta.Appezza AndAlso c.Id_Reg = distinta.Id_Reg AndAlso c.Progetto_Cod = distinta.Progetto_Cod Select c).ToList()
                Dim fasi = (From f In GiasContext.Imprese_Progetto_Fasi Where f.Piva = distinta.Piva AndAlso f.Progetto_Cod = distinta.Progetto_Cod Select f).ToList()
                g2g.DistinteToUpdate.Add(New G2G_Distinta With {.Distinta = distinta, .Codici = codici, .Fasi = fasi})
            Next

            g2g.Recode.G2GRecodeDistinteToDelete = (
                From g In GiasContext.G2G_Recode_Distinta
                Where g.To_PivaSuperUser = From_PivaSuperUser AndAlso g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = From_Piva AndAlso
                      Not GiasContext.Imprese_Progetti.Any(Function(d) d.Piva = g.From_Piva AndAlso d.Progetto_Cod = g.To_Progetto_cod)
                Select g).ToList()

        End Using

        Return g2g.DistinteToInsert.Count > 0 OrElse g2g.DistinteToUpdate.Count > 0 OrElse g2g.Recode.G2GRecodeDistinteToDelete.Count > 0

    End Function

End Class


Public Class G2GDistinte_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Distinte_G2G(ByRef g2g As G2G_Distinte, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GDistinte_W.Scrivi_Distinte_G2G()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_Distinte(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_Distinte(g2g, objParametri)

    End Function

    Public Function Scrivi_Distinte_G2GReverse(ByRef g2g As G2G_Distinte_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GDistinte_W.Scrivi_Distinte_G2GReverse()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_DistinteReverse(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_DistinteReverse(g2g, objParametri)

    End Function

    Public Function Scrivi_Distinte(ByRef g2g As G2G_Distinte, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GDistinte_W.Scrivi_Distinte()"
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
        'objSequenze.Calcola_BaseCode(g2g.To_Progressivo, TopCode, BaseCode, objParametri)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' DISTINTE DA INSERIRE
                If g2g.DistinteToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    Dim listImprese_Progetti As New List(Of AgronicaCoreEntityFramework_POCO.Imprese_Progetti)
                    Dim listReg_Impianti_Codici As New List(Of AgronicaCoreEntityFramework_POCO.Reg_Impianti_Codici)
                    Dim listImprese_Progetto_Fasi As New List(Of AgronicaCoreEntityFramework_POCO.Imprese_Progetto_Fasi)
                    Dim listG2G_Recode_Distinta As New List(Of AgronicaCoreEntityFramework_POCO.G2G_Recode_Distinta)

                    Dim listG2G_Recode_Appezzamento As List(Of AgronicaCoreEntityFramework_POCO.G2G_Recode_Appezzamenti)
                    Dim listG2G_Recode_Impianto As List(Of AgronicaCoreEntityFramework_POCO.G2G_Recode_Impianti)

                    Dim listG2G_Recode_Distinte_GiaEsistenti As List(Of AgronicaCoreEntityFramework_POCO.G2G_Recode_Distinta)
                    Dim firstFromPiva = g2g.DistinteToInsert(0).Distinta.Piva
                    listG2G_Recode_Distinte_GiaEsistenti = (From rr In GiasContext.G2G_Recode_Distinta
                                                            Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso
                                                               rr.To_PivaSuperUser = To_PivaSuperUser AndAlso
                                                               rr.From_Piva = firstFromPiva).ToList()

                    For Each d As G2G_Distinta In g2g.DistinteToInsert

                        If listG2G_Recode_Appezzamento Is Nothing Then
                            listG2G_Recode_Appezzamento = (From rr In GiasContext.G2G_Recode_Appezzamenti
                                                           Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso
                                                               rr.To_PivaSuperUser = To_PivaSuperUser AndAlso
                                                               rr.From_Piva = d.Distinta.Piva).ToList
                        End If

                        If listG2G_Recode_Impianto Is Nothing Then
                            listG2G_Recode_Impianto = (From rr In GiasContext.G2G_Recode_Impianti
                                                       Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso
                                                           rr.To_PivaSuperUser = To_PivaSuperUser AndAlso
                                                           rr.From_Piva = d.Distinta.Piva).ToList
                        End If

                        Dim list_recode_appezza = (From rr In listG2G_Recode_Appezzamento Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = d.Distinta.Piva AndAlso rr.From_Sa_Cod = d.Distinta.Sa_Cod AndAlso rr.From_Appezza = d.Distinta.Appezza Select (rr.To_Appezza)).ToList
                        If list_recode_appezza.Count = 0 Then
                            Throw New Exception("Appezzamento non presente in destinazione")
                        End If
                        If list_recode_appezza.Count > 1 Then
                            Throw New Exception("Presenti più recode in destinazione per lo stesso appezzamento")
                        End If
                        Dim To_Appezza As Integer = list_recode_appezza(0)

                        Dim list_Id_Reg = (From rr In listG2G_Recode_Impianto Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = d.Distinta.Piva AndAlso rr.From_Sa_Cod = d.Distinta.Sa_Cod AndAlso rr.From_Appezza = d.Distinta.Appezza AndAlso rr.From_Id_Reg = d.Distinta.Id_Reg Select (rr.To_Id_Reg)).ToList()
                        If list_Id_Reg.Count = 0 Then
                            Throw New Exception("Impianto non presente in destinazione")
                        End If
                        If list_Id_Reg.Count > 1 Then
                            Throw New Exception("Presenti più recode in destinazione per lo stesso Impianto")
                        End If
                        Dim To_Id_Reg = list_Id_Reg(0)

                        Dim recodeEsistenti = (From r In listG2G_Recode_Distinte_GiaEsistenti Where r.From_Piva = d.Distinta.Piva And
                                                                                              r.From_Progetto_cod = d.Distinta.Progetto_Cod).ToList

                        If recodeEsistenti.Count = 0 Then
                            ' nuovo distinta
                            Dim idSeq = objSequenze.NuovoId_Tabella("Impresa_Progetto", BaseCode, TopCode, objParametri)
                            Dim distinta = Gias_EF_Utility.CopyEntity(GiasContext, d.Distinta, Nothing, username, data)
                            distinta.Piva = To_Piva
                            distinta.Sa_Cod = To_Sa_Cod
                            distinta.Appezza = To_Appezza
                            distinta.Id_Reg = To_Id_Reg
                            distinta.Progetto_Cod = idSeq
                            'GiasContext.Imprese_Progetti.Add(distinta)
                            listImprese_Progetti.Add(distinta)

                            ' inserisce codici distinta
                            For Each cc As Reg_Impianti_Codici In d.Codici
                                Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                                codice.PIVA = distinta.Piva
                                codice.sa_cod = distinta.Sa_Cod
                                codice.appezza = distinta.Appezza
                                codice.Id_Reg = distinta.Id_Reg
                                codice.Progetto_Cod = distinta.Progetto_Cod
                                'GiasContext.Reg_Impianti_Codici.Add(codice)
                                listReg_Impianti_Codici.Add(codice)
                            Next

                            ' inserisce fasi distinta
                            For Each f As Imprese_Progetto_Fasi In d.Fasi
                                Dim idFase = objSequenze.NuovoId_Tabella("Impresa_Progetto_Fasi", BaseCode, TopCode, objParametri)
                                Dim fase = Gias_EF_Utility.CopyEntity(GiasContext, f, Nothing, username, data)
                                fase.Piva = distinta.Piva
                                fase.Progetto_Cod = distinta.Progetto_Cod
                                fase.Fase_Cod = idFase
                                'GiasContext.Imprese_Progetto_Fasi.Add(fase)
                                listImprese_Progetto_Fasi.Add(fase)
                            Next

                            'nuovo recode distinta
                            Dim recode =
                            New G2G_Recode_Distinta With {
                                .From_PivaSuperUser = From_PivaSuperUser,
                                .To_PivaSuperUser = To_PivaSuperUser,
                                .From_Piva = d.Distinta.Piva,
                                .To_Piva = distinta.Piva,
                                .From_Progetto_cod = d.Distinta.Progetto_Cod,
                                .To_Progetto_cod = distinta.Progetto_Cod,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = 0,
                                .datainvio = Now()
                            }
                            g2g.Recode.G2GRecodeDistinteToInsert.Add(recode)
                            'GiasContext.G2G_Recode_Distinta.Add(recode)
                            listG2G_Recode_Distinta.Add(recode)
                        Else
                            g2g.Recode.G2GRecodeDistinteToInsert.Add(recodeEsistenti(0))
                            Dim messaggio = "Distinta non importata: " & JsonConvert.SerializeObject(d.Distinta) & vbCrLf &
                                "Recode già presente"
                            Scrivi_LOG(objParametri, nomeRoutine, messaggio)
                        End If

                        'G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.Imprese_Progetti.AddRange(listImprese_Progetti)
                    GiasContext.Reg_Impianti_Codici.AddRange(listReg_Impianti_Codici)
                    GiasContext.Imprese_Progetto_Fasi.AddRange(listImprese_Progetto_Fasi)
                    GiasContext.G2G_Recode_Distinta.AddRange(listG2G_Recode_Distinta)

                    GiasContext.SaveChanges()

                End If

                ' DISTINTE DA MODIFICARE
                If g2g.DistinteToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    ' distinte da modificare
                    For Each d As G2G_Distinta In g2g.DistinteToUpdate

                        Dim To_Appezza As Integer = (From rr In GiasContext.G2G_Recode_Appezzamenti Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = d.Distinta.Piva AndAlso rr.From_Sa_Cod = d.Distinta.Sa_Cod AndAlso rr.From_Appezza = d.Distinta.Appezza Select (rr.To_Appezza)).FirstOrDefault()
                        If To_Appezza = 0 Then
                            Throw New Exception("Appezzamento non presente in destinazione")
                        End If

                        Dim To_Id_Reg As Integer = (From rr In GiasContext.G2G_Recode_Impianti Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = d.Distinta.Piva AndAlso rr.From_Sa_Cod = d.Distinta.Sa_Cod AndAlso rr.From_Appezza = d.Distinta.Appezza AndAlso rr.From_Id_Reg = d.Distinta.Id_Reg Select (rr.To_Id_Reg)).FirstOrDefault()
                        If To_Id_Reg = 0 Then
                            Throw New Exception("Impianto non presente in destinazione")
                        End If

                        ' modifica recode distinta
                        Dim recode = (From rr In GiasContext.G2G_Recode_Distinta Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = d.Distinta.Piva AndAlso rr.From_Progetto_cod = d.Distinta.Progetto_Cod).FirstOrDefault()
                        If recode Is Nothing Then
                            Throw New Exception("Recode non presente in destinazione")
                        End If
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodeDistinteToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Distinta.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        ' modifica distinta
                        Dim distinta = (From dd In GiasContext.Imprese_Progetti Where dd.Piva = recode.To_Piva AndAlso dd.Progetto_Cod = recode.To_Progetto_cod).FirstOrDefault()
                        If distinta Is Nothing Then
                            Throw New Exception("Distinta non presente in destinazione")
                        End If
                        distinta = Gias_EF_Utility.CopyEntity(GiasContext, d.Distinta, distinta, username, data)
                        distinta.Piva = To_Piva
                        distinta.Sa_Cod = To_Sa_Cod
                        distinta.Appezza = To_Appezza
                        distinta.Id_Reg = To_Id_Reg
                        distinta.Progetto_Cod = recode.To_Progetto_cod
                        GiasContext.Imprese_Progetti.Attach(distinta)
                        GiasContext.Entry(distinta).State = EntityState.Modified

                        ' cancella codici distinta
                        Dim codici = (From cc In GiasContext.Reg_Impianti_Codici Where cc.PIVA = distinta.Piva AndAlso cc.sa_cod = distinta.Sa_Cod AndAlso cc.appezza = distinta.Appezza AndAlso cc.Id_Reg = distinta.Id_Reg AndAlso cc.Progetto_Cod = distinta.Progetto_Cod Select cc).ToList()
                        For Each cc As Reg_Impianti_Codici In codici
                            GiasContext.Reg_Impianti_Codici.Attach(cc)
                            GiasContext.Reg_Impianti_Codici.Remove(cc)
                        Next

                        ' inserisce codici distinta
                        For Each cc As Reg_Impianti_Codici In d.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = distinta.Piva
                            codice.sa_cod = distinta.Sa_Cod
                            codice.appezza = distinta.Appezza
                            codice.Id_Reg = distinta.Id_Reg
                            codice.Progetto_Cod = distinta.Progetto_Cod
                            GiasContext.Reg_Impianti_Codici.Add(codice)
                        Next

                        ' cancella fasi distinta
                        'Dim fasi = (From f In GiasContext.Imprese_Progetto_Fasi Where f.Piva = distinta.Piva AndAlso f.Progetto_Cod = distinta.Progetto_Cod Select f).ToList()
                        'For Each f As Imprese_Progetto_Fasi In fasi
                        '    GiasContext.AttachTo("Imprese_Progetto_Fasi", f)
                        '    GiasContext.DeleteObject(f)
                        'Next

                        ' inserisce fasi distinta
                        'For Each f As Imprese_Progetto_Fasi In d.Fasi
                        '    Dim idFase = objSequenze.NuovoId_Tabella("Impresa_Progetto_Fasi", BaseCode, TopCode, objParametri)
                        '    Dim fase = Gias_EF_Utility.CopyEntity(GiasContext, f, Nothing, username, data)
                        '    fase.Piva = distinta.Piva
                        '    fase.Progetto_Cod = distinta.Progetto_Cod
                        '    fase.Fase_Cod = idFase
                        '    GiasContext.Imprese_Progetto_Fasi.Add(fase)
                        'Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' DISTINTE DA CANCELLARE
                If g2g.Recode.G2GRecodeDistinteToDelete.Count > 0 Then

                    Dim index As Integer = 0

                    For Each r As G2G_Recode_Distinta In g2g.Recode.G2GRecodeDistinteToDelete

                        Dim distinta = (From d In GiasContext.Imprese_Progetti Where d.Piva = r.To_Piva AndAlso d.Progetto_Cod = r.To_Progetto_cod).FirstOrDefault()

                        'cancella distinta
                        If distinta IsNot Nothing Then

                            ' cancella codici distinta
                            Dim codici = (From cc In GiasContext.Reg_Impianti_Codici Where cc.PIVA = distinta.Piva AndAlso cc.sa_cod = distinta.Sa_Cod AndAlso cc.appezza = distinta.Appezza AndAlso cc.Id_Reg = distinta.Id_Reg AndAlso cc.Progetto_Cod = distinta.Progetto_Cod Select cc).ToList()
                            For Each cc As Reg_Impianti_Codici In codici
                                GiasContext.Reg_Impianti_Codici.Attach(cc)
                                GiasContext.Reg_Impianti_Codici.Remove(cc)
                            Next

                            'cancella distinta
                            Dim distintaEntry = GiasContext.Entry(distinta)
                            If distintaEntry.State = EntityState.Detached Then
                                GiasContext.Imprese_Progetti.Attach(distinta)
                            End If
                            If distintaEntry.State <> EntityState.Deleted Then
                                GiasContext.Imprese_Progetti.Remove(distinta)
                            End If

                        End If

                        ' cancella recode distinta
                        Dim recodeToDelete As G2G_Recode_Distinta = r
                        Dim trackedRecode = GiasContext.G2G_Recode_Distinta.Local.FirstOrDefault(Function(x) _
                            x.From_PivaSuperUser = r.From_PivaSuperUser AndAlso
                            x.To_PivaSuperUser = r.To_PivaSuperUser AndAlso
                            x.From_Piva = r.From_Piva AndAlso
                            x.To_Piva = r.To_Piva AndAlso
                            x.From_Progetto_cod = r.From_Progetto_cod AndAlso
                            x.To_Progetto_cod = r.To_Progetto_cod)

                        If trackedRecode IsNot Nothing Then
                            recodeToDelete = trackedRecode
                        Else

                            Dim recodeEntry = GiasContext.Entry(r)
                            If recodeEntry.State = EntityState.Detached Then
                                Dim existsInDB = GiasContext.G2G_Recode_Distinta.Any(Function(x) _
                                    x.From_PivaSuperUser = r.From_PivaSuperUser AndAlso
                                    x.To_PivaSuperUser = r.To_PivaSuperUser AndAlso
                                    x.From_Piva = r.From_Piva AndAlso
                                    x.To_Piva = r.To_Piva AndAlso
                                    x.From_Progetto_cod = r.From_Progetto_cod AndAlso
                                    x.To_Progetto_cod = r.To_Progetto_cod)
                                If existsInDB Then
                                    GiasContext.G2G_Recode_Distinta.Attach(r)
                                Else
                                    g2g.Recode.LogRecode &= objParametri.Recupera_NomeDB() & " - Recode Distinta già eliminato: " & r.From_PivaSuperUser & " -> " & r.To_PivaSuperUser & " - " & r.From_Piva & " -> " & r.To_Piva & " - " & r.From_Progetto_cod & " -> " & r.To_Progetto_cod & Environment.NewLine
                                    Continue For
                                End If


                            End If
                        End If

                        Dim finalEntry = GiasContext.Entry(recodeToDelete)
                        If finalEntry.State <> EntityState.Deleted Then
                            GiasContext.G2G_Recode_Distinta.Remove(recodeToDelete)
                        End If

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode &= " - distinte: " & g2g.DistinteToInsert.Count & " nuovi, " & g2g.DistinteToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeDistinteToDelete.Count & " cancellati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.ToString
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_DistinteReverse(ByRef g2g As G2G_Distinte_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GDistinte_W.Scrivi_DistinteReverse()"
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
        'objSequenze.Calcola_BaseCode(g2g.To_Progressivo, TopCode, BaseCode, objParametri)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                ' DISTINTE DA INSERIRE
                If g2g.DistinteToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each d As G2G_Distinta In g2g.DistinteToInsert

                        Dim To_Appezza As Integer = (From rr In GiasContext.G2G_Recode_Appezzamenti Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = d.Distinta.Piva AndAlso rr.To_Sa_Cod = d.Distinta.Sa_Cod AndAlso rr.To_Appezza = d.Distinta.Appezza Select (rr.From_Appezza)).FirstOrDefault()
                        If To_Appezza = 0 Then
                            Throw New Exception("Appezzamento non presente in destinazione")
                        End If

                        Dim To_Id_Reg As Integer = (From rr In GiasContext.G2G_Recode_Impianti Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = d.Distinta.Piva AndAlso rr.To_Sa_Cod = d.Distinta.Sa_Cod AndAlso rr.To_Appezza = d.Distinta.Appezza AndAlso rr.To_Id_Reg = d.Distinta.Id_Reg Select (rr.From_Id_Reg)).FirstOrDefault()
                        If To_Id_Reg = 0 Then
                            Throw New Exception("Impianto non presente in destinazione")
                        End If

                        ' nuovo distinta
                        Dim idSeq = objSequenze.NuovoId_Tabella("Impresa_Progetto", BaseCode, TopCode, objParametri)
                        Dim distinta = Gias_EF_Utility.CopyEntity(GiasContext, d.Distinta, Nothing, username, data)
                        distinta.Piva = To_Piva
                        distinta.Sa_Cod = To_Sa_Cod
                        distinta.Appezza = To_Appezza
                        distinta.Id_Reg = To_Id_Reg
                        distinta.Progetto_Cod = idSeq
                        GiasContext.Imprese_Progetti.Add(distinta)

                        ' inserisce codici distinta
                        For Each cc As Reg_Impianti_Codici In d.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = distinta.Piva
                            codice.sa_cod = distinta.Sa_Cod
                            codice.appezza = distinta.Appezza
                            codice.Id_Reg = distinta.Id_Reg
                            codice.Progetto_Cod = distinta.Progetto_Cod
                            GiasContext.Reg_Impianti_Codici.Add(codice)
                        Next

                        ' inserisce fasi distinta
                        For Each f As Imprese_Progetto_Fasi In d.Fasi
                            Dim idFase = objSequenze.NuovoId_Tabella("Impresa_Progetto_Fasi", BaseCode, TopCode, objParametri)
                            Dim fase = Gias_EF_Utility.CopyEntity(GiasContext, f, Nothing, username, data)
                            fase.Piva = distinta.Piva
                            fase.Progetto_Cod = distinta.Progetto_Cod
                            fase.Fase_Cod = idFase
                            GiasContext.Imprese_Progetto_Fasi.Add(fase)
                        Next

                        'nuovo recode distinta
                        Dim recode =
                        New G2G_Recode_Distinta With {
                            .From_PivaSuperUser = To_PivaSuperUser,
                            .To_PivaSuperUser = From_PivaSuperUser,
                            .From_Piva = distinta.Piva,
                            .To_Piva = d.Distinta.Piva,
                            .From_Progetto_cod = distinta.Progetto_Cod,
                            .To_Progetto_cod = d.Distinta.Progetto_Cod,
                            .Username_Creazione = username,
                            .Username_Modifica = username,
                            .Data_Creazione = Now(),
                            .Data_Modifica = Now(),
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .inviato = 0,
                            .datainvio = Now()
                        }
                        g2g.Recode.G2GRecodeDistinteToInsert.Add(recode)
                        GiasContext.G2G_Recode_Distinta.Add(recode)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' DISTINTE DA MODIFICARE
                If g2g.DistinteToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    ' distinte da modificare
                    For Each d As G2G_Distinta In g2g.DistinteToUpdate

                        Dim From_Appezza As Integer = (From rr In GiasContext.G2G_Recode_Appezzamenti Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = d.Distinta.Piva AndAlso rr.To_Sa_Cod = d.Distinta.Sa_Cod AndAlso rr.To_Appezza = d.Distinta.Appezza Select (rr.From_Appezza)).FirstOrDefault()
                        If From_Appezza = 0 Then
                            Throw New Exception("Appezzamento non presente in destinazione")
                        End If

                        Dim From_Id_Reg As Integer = (From rr In GiasContext.G2G_Recode_Impianti Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = d.Distinta.Piva AndAlso rr.To_Sa_Cod = d.Distinta.Sa_Cod AndAlso rr.To_Appezza = d.Distinta.Appezza AndAlso rr.To_Id_Reg = d.Distinta.Id_Reg Select (rr.From_Id_Reg)).FirstOrDefault()
                        If From_Appezza = 0 Then
                            Throw New Exception("Impianto non presente in destinazione")
                        End If

                        ' modifica recode distinta
                        Dim recode = (From rr In GiasContext.G2G_Recode_Distinta Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = d.Distinta.Piva AndAlso rr.To_Progetto_cod = d.Distinta.Progetto_Cod).FirstOrDefault()
                        If recode Is Nothing Then
                            Throw New Exception("Recode non presente in destinazione")
                        End If
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodeDistinteToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Distinta.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        ' modifica distinta
                        Dim distinta = (From dd In GiasContext.Imprese_Progetti Where dd.Piva = recode.From_Piva AndAlso dd.Progetto_Cod = recode.From_Progetto_cod).FirstOrDefault()
                        If distinta Is Nothing Then
                            Throw New Exception("Distinta non presente in destinazione")
                        End If
                        distinta = Gias_EF_Utility.CopyEntity(GiasContext, d.Distinta, distinta, username, data)
                        distinta.Piva = From_Piva
                        distinta.Sa_Cod = To_Sa_Cod
                        distinta.Appezza = From_Appezza
                        distinta.Id_Reg = From_Id_Reg
                        distinta.Progetto_Cod = recode.From_Progetto_cod
                        GiasContext.Imprese_Progetti.Attach(distinta)
                        GiasContext.Entry(distinta).State = EntityState.Modified

                        ' cancella codici distinta
                        Dim codici = (From cc In GiasContext.Reg_Impianti_Codici Where cc.PIVA = distinta.Piva AndAlso cc.sa_cod = distinta.Sa_Cod AndAlso cc.appezza = distinta.Appezza AndAlso cc.Id_Reg = distinta.Id_Reg AndAlso cc.Progetto_Cod = distinta.Progetto_Cod Select cc).ToList()
                        For Each cc As Reg_Impianti_Codici In codici
                            GiasContext.Reg_Impianti_Codici.Attach(cc)
                            GiasContext.Reg_Impianti_Codici.Remove(cc)
                        Next

                        ' inserisce codici distinta
                        For Each cc As Reg_Impianti_Codici In d.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = distinta.Piva
                            codice.sa_cod = distinta.Sa_Cod
                            codice.appezza = distinta.Appezza
                            codice.Id_Reg = distinta.Id_Reg
                            codice.Progetto_Cod = distinta.Progetto_Cod
                            GiasContext.Reg_Impianti_Codici.Add(codice)
                        Next

                        ' cancella fasi distinta
                        'Dim fasi = (From f In GiasContext.Imprese_Progetto_Fasi Where f.Piva = distinta.Piva AndAlso f.Progetto_Cod = distinta.Progetto_Cod Select f).ToList()
                        'For Each f As Imprese_Progetto_Fasi In fasi
                        '    GiasContext.AttachTo("Imprese_Progetto_Fasi", f)
                        '    GiasContext.DeleteObject(f)
                        'Next

                        ' inserisce fasi distinta
                        'For Each f As Imprese_Progetto_Fasi In d.Fasi
                        '    Dim idFase = objSequenze.NuovoId_Tabella("Impresa_Progetto_Fasi", BaseCode, TopCode, objParametri)
                        '    Dim fase = Gias_EF_Utility.CopyEntity(GiasContext, f, Nothing, username, data)
                        '    fase.Piva = distinta.Piva
                        '    fase.Progetto_Cod = distinta.Progetto_Cod
                        '    fase.Fase_Cod = idFase
                        '    GiasContext.Imprese_Progetto_Fasi.Add(fase)
                        'Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' DISTINTE DA CANCELLARE
                If g2g.Recode.G2GRecodeDistinteToDelete.Count > 0 Then

                    Dim index As Integer = 0

                    For Each r As G2G_Recode_Distinta In g2g.Recode.G2GRecodeDistinteToDelete

                        Dim distinta = (From d In GiasContext.Imprese_Progetti Where d.Piva = r.To_Piva AndAlso d.Progetto_Cod = r.From_Progetto_cod).FirstOrDefault()

                        'cancella distinta
                        If distinta IsNot Nothing Then

                            ' cancella codici distinta
                            Dim codici = (From cc In GiasContext.Reg_Impianti_Codici Where cc.PIVA = distinta.Piva AndAlso cc.sa_cod = distinta.Sa_Cod AndAlso cc.appezza = distinta.Appezza AndAlso cc.Id_Reg = distinta.Id_Reg AndAlso cc.Progetto_Cod = distinta.Progetto_Cod Select cc).ToList()
                            For Each cc As Reg_Impianti_Codici In codici
                                GiasContext.Reg_Impianti_Codici.Attach(cc)
                                GiasContext.Reg_Impianti_Codici.Remove(cc)
                            Next

                            'cancella distinta
                            GiasContext.Imprese_Progetti.Attach(distinta)
                            GiasContext.Imprese_Progetti.Remove(distinta)

                        End If

                        ' cancella recode distinta
                        GiasContext.G2G_Recode_Distinta.Attach(r)
                        GiasContext.G2G_Recode_Distinta.Remove(r)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode &= " - distinte: " & g2g.DistinteToInsert.Count & " nuovi, " & g2g.DistinteToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeDistinteToDelete.Count & " cancellati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

End Class