Imports System.Transactions
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModello.Anagrafe
Imports System.Data.Entity
Imports System.Text
Imports Newtonsoft.Json

Public Class G2GAppezzamenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Nuovo_Appezzamenti_G2G(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String, ByVal To_Sa_Cod As Integer, ByVal To_Progressivo As Integer) As G2G_Appezzamenti

        Dim g2g As New G2G_Appezzamenti
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .To_Sa_Cod = To_Sa_Cod
            .To_Progressivo = To_Progressivo
            .AppezzamentiToInsert = New List(Of G2G_Appezzamento)
            .AppezzamentiToUpdate = New List(Of G2G_Appezzamento)
            .AppezzamentiToDelete = New List(Of G2G_Appezzamento)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Nuovo_Appezzamenti_G2GReverse(ByVal From_PivaSuperUser As String, ByVal To_PivaSuperUser As String, ByVal From_Piva As String, ByVal To_Piva As String, ByVal To_Sa_Cod As Integer, ByVal To_Progressivo As Integer) As G2G_Appezzamenti_Reverse

        Dim g2g As New G2G_Appezzamenti_Reverse
        With g2g
            .From_PivaSuperUser = From_PivaSuperUser
            .To_PivaSuperUser = To_PivaSuperUser
            .From_Piva = From_Piva
            .To_Piva = To_Piva
            .To_Sa_Cod = To_Sa_Cod
            .To_Progressivo = To_Progressivo
            .AppezzamentiToInsert = New List(Of G2G_Appezzamento)
            .AppezzamentiToUpdate = New List(Of G2G_Appezzamento)
            .AppezzamentiToDelete = New List(Of G2G_Appezzamento)
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        End With

        Return g2g

    End Function

    Public Function Leggi_Appezzamenti_G2G(ByVal Sa_Cod As Integer, ByRef Impianti As List(Of Impianto_Colturale), ByRef g2g As G2G_Appezzamenti, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef Log_Import As StringBuilder) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GAppezzamenti_R.Leggi_Appezzamenti_G2G()"
        Dim From_Piva = g2g.From_Piva
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Data = objParametri.FinestraTemporaleInizio
        Dim To_Data = objParametri.FinestraTemporaleFine

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            GiasContext.Database.CommandTimeout = 3600

            Dim listaAppezzamentiInsert = (
                From a In GiasContext.Appezzamento
                Where a.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse a.SA_COD = Sa_Cod) AndAlso
                    a.Validita_Inizio <= To_Data AndAlso a.Validita_Fine >= From_Data AndAlso
                    Not GiasContext.G2G_Recode_Appezzamenti.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = a.PIVA AndAlso
                        g.From_Sa_Cod = a.SA_COD AndAlso g.From_Appezza = a.APPEZZA)
                Select a).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Appezzamenti --> listaAppezzamentiInsert " & listaAppezzamentiInsert.Count)

            Dim UtentiXAppezzamenti As List(Of AgronicaCoreEntityFramework_POCO.UtentiXAppezzamenti) = (
                From a In GiasContext.UtentiXAppezzamenti
                Where a.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse a.SA_COD = Sa_Cod) AndAlso
                    a.Validita_Inizio <= To_Data AndAlso a.Validita_Fine >= From_Data AndAlso
                    Not GiasContext.G2G_Recode_Appezzamenti.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = a.PIVA AndAlso
                        g.From_Sa_Cod = a.SA_COD AndAlso g.From_Appezza = a.Appezza)
                Select a).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Appezzamenti --> UtentiXAppezzamenti " & UtentiXAppezzamenti.Count)

            Dim Appezzamento_Codici As List(Of AgronicaCoreEntityFramework_POCO.Appezzamento_Codici) = (
                From a In GiasContext.Appezzamento_Codici
                Where a.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse a.sa_cod = Sa_Cod) AndAlso
                    a.Validita_Inizio <= To_Data AndAlso a.Validita_Fine >= From_Data AndAlso
                    Not GiasContext.G2G_Recode_Appezzamenti.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = a.PIVA AndAlso
                        g.From_Sa_Cod = a.sa_cod AndAlso g.From_Appezza = a.appezza)
                Select a).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Appezzamenti --> Appezzamento_Codici " & Appezzamento_Codici.Count)

            Dim AppezzamentiXParticelle As List(Of AgronicaCoreEntityFramework_POCO.AppezzamentiXParticelle) = (
                From a In GiasContext.AppezzamentiXParticelle
                Where a.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse a.SA_COD = Sa_Cod) AndAlso
                    a.Validita_Inizio <= To_Data AndAlso a.Validita_Fine >= From_Data AndAlso
                    Not GiasContext.G2G_Recode_Appezzamenti.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = a.PIVA AndAlso
                        g.From_Sa_Cod = a.SA_COD AndAlso g.From_Appezza = a.APPEZZA)
                Select a).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Appezzamenti --> AppezzamentiXParticelle " & AppezzamentiXParticelle.Count)

            Dim AppezzamentiXParticellexMacrousi As List(Of AgronicaCoreEntityFramework_POCO.AppezzamentiXParticellexMacrousi) = (
                From a In GiasContext.AppezzamentiXParticellexMacrousi
                Where a.Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse a.Sa_cod = Sa_Cod) AndAlso
                    a.Validita_Inizio <= To_Data AndAlso a.Validita_Fine >= From_Data AndAlso
                    Not GiasContext.G2G_Recode_Appezzamenti.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = a.Piva AndAlso
                        g.From_Sa_Cod = a.Sa_cod AndAlso g.From_Appezza = a.Appezza)
                Select a).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Appezzamenti --> AppezzamentiXParticellexMacrousi " & AppezzamentiXParticellexMacrousi.Count)

            Dim AppezzamentiXParticellexMacrousixUtilizzo As List(Of AgronicaCoreEntityFramework_POCO.AppezzamentiXParticellexMacrousixUtilizzo) = (
                From a In GiasContext.AppezzamentiXParticellexMacrousixUtilizzo
                Where a.Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse a.Sa_cod = Sa_Cod) AndAlso
                    a.Validita_Inizio <= To_Data AndAlso a.Validita_Fine >= From_Data AndAlso
                    Not GiasContext.G2G_Recode_Appezzamenti.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = a.Piva AndAlso
                        g.From_Sa_Cod = a.Sa_cod AndAlso g.From_Appezza = a.Appezza)
                Select a).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Appezzamenti --> AppezzamentiXParticellexMacrousixUtilizzo " & AppezzamentiXParticellexMacrousixUtilizzo.Count)

            For Each appezzamento In listaAppezzamentiInsert

                Dim filtroImpianto As Boolean = (From i In Impianti Where i.Piva = appezzamento.PIVA And i.Sa_Cod = appezzamento.SA_COD And i.Appezza = appezzamento.APPEZZA).ToList.Count > 0

                If Impianti.Count = 0 OrElse filtroImpianto Then
                    Dim utente = (From u In UtentiXAppezzamenti Where u.USER = From_PivaSuperUser AndAlso u.PIVA = appezzamento.PIVA AndAlso u.SA_COD = appezzamento.SA_COD AndAlso u.Appezza = appezzamento.APPEZZA Select u).FirstOrDefault()
                    Dim codici = (From c In Appezzamento_Codici Where c.PIVA = appezzamento.PIVA AndAlso c.sa_cod = appezzamento.SA_COD AndAlso c.appezza = appezzamento.APPEZZA Select c).ToList()
                    Dim particelle = (From p In AppezzamentiXParticelle Where p.PIVA = appezzamento.PIVA AndAlso p.SA_COD = appezzamento.SA_COD AndAlso p.APPEZZA = appezzamento.APPEZZA Select p).ToList()
                    Dim macrousi = (From pm In AppezzamentiXParticellexMacrousi Where pm.Piva = appezzamento.PIVA AndAlso pm.Sa_cod = appezzamento.SA_COD AndAlso pm.Appezza = appezzamento.APPEZZA Select pm).ToList()
                    Dim utilizzi = (From pmu In AppezzamentiXParticellexMacrousixUtilizzo Where pmu.Piva = appezzamento.PIVA AndAlso pmu.Sa_cod = appezzamento.SA_COD AndAlso pmu.Appezza = appezzamento.APPEZZA Select pmu).ToList()
                    g2g.AppezzamentiToInsert.Add(New G2G_Appezzamento With {.Appezzamento = appezzamento, .Utente = utente, .Codici = codici, .Particelle = particelle, .Macrousi = macrousi, .Utilizzi = utilizzi})
                End If

            Next

            Dim listaAppezzamentiUpdate = (
                From a In GiasContext.Appezzamento
                Where a.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse a.SA_COD = Sa_Cod) AndAlso
                    a.Validita_Inizio <= To_Data AndAlso a.Validita_Fine >= From_Data AndAlso
                    (GiasContext.G2G_Recode_Appezzamenti.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = a.PIVA AndAlso
                        g.From_Sa_Cod = a.SA_COD AndAlso g.From_Appezza = a.APPEZZA AndAlso g.datainvio < a.Data_Modifica) OrElse
                    (From ac In GiasContext.Appezzamento_Codici
                     Where ac.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse ac.sa_cod = Sa_Cod) AndAlso
                     GiasContext.G2G_Recode_Appezzamenti.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = ac.PIVA AndAlso g.From_Sa_Cod = ac.sa_cod AndAlso g.From_Appezza = ac.appezza AndAlso g.datainvio < ac.Data_Modifica)
                     Select ac).Any(Function(x) x.PIVA = a.PIVA AndAlso x.sa_cod = a.SA_COD AndAlso x.appezza = a.APPEZZA))
                Select a).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Appezzamenti --> listaAppezzamentiUpdate " & listaAppezzamentiUpdate.Count)

            For Each appezzamento In listaAppezzamentiUpdate
                Dim utente = (From u In GiasContext.UtentiXAppezzamenti Where u.USER = From_PivaSuperUser AndAlso u.PIVA = appezzamento.PIVA AndAlso u.SA_COD = appezzamento.SA_COD AndAlso u.Appezza = appezzamento.APPEZZA Select u).FirstOrDefault()
                Dim codici = (From c In GiasContext.Appezzamento_Codici Where c.PIVA = appezzamento.PIVA AndAlso c.sa_cod = appezzamento.SA_COD AndAlso c.appezza = appezzamento.APPEZZA Select c).ToList()
                Dim particelle = (From p In GiasContext.AppezzamentiXParticelle Where p.PIVA = appezzamento.PIVA AndAlso p.SA_COD = appezzamento.SA_COD AndAlso p.APPEZZA = appezzamento.APPEZZA Select p).ToList()
                Dim macrousi = (From pm In GiasContext.AppezzamentiXParticellexMacrousi Where pm.Piva = appezzamento.PIVA AndAlso pm.Sa_cod = appezzamento.SA_COD AndAlso pm.Appezza = appezzamento.APPEZZA Select pm).ToList()
                Dim utilizzi = (From pmu In GiasContext.AppezzamentiXParticellexMacrousixUtilizzo Where pmu.Piva = appezzamento.PIVA AndAlso pmu.Sa_cod = appezzamento.SA_COD AndAlso pmu.Appezza = appezzamento.APPEZZA Select pmu).ToList()
                g2g.AppezzamentiToUpdate.Add(New G2G_Appezzamento With {.Appezzamento = appezzamento, .Utente = utente, .Codici = codici, .Particelle = particelle, .Macrousi = macrousi, .Utilizzi = utilizzi})
            Next

            g2g.Recode.G2GRecodeAppezzamentiToDelete = (
                From g In GiasContext.G2G_Recode_Appezzamenti
                Where g.From_PivaSuperUser = From_PivaSuperUser AndAlso g.To_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse g.From_Sa_Cod = Sa_Cod) AndAlso
                      Not GiasContext.Appezzamento.Any(Function(a) a.PIVA = g.From_Piva AndAlso a.SA_COD = g.From_Sa_Cod AndAlso a.APPEZZA = g.From_Appezza)
                Select g).ToList()

            G2GUtility.Log(Log_Import, "Trasferimento piano colturale: Appezzamenti --> G2GRecodeAppezzamentiToDelete " & g2g.Recode.G2GRecodeAppezzamentiToDelete.Count)

        End Using

        Return g2g.AppezzamentiToInsert.Count > 0 OrElse g2g.AppezzamentiToUpdate.Count > 0 OrElse g2g.Recode.G2GRecodeAppezzamentiToDelete.Count > 0

    End Function

    Public Function Leggi_Appezzamenti_G2GReverse(ByVal Sa_Cod As Integer, ByRef Impianti As List(Of Impianto_Colturale), ByRef g2g As G2G_Appezzamenti_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GAppezzamenti_R.Leggi_Appezzamenti_G2G()"
        Dim From_Piva = g2g.From_Piva
        Dim From_PivaSuperUser = g2g.From_PivaSuperUser
        Dim To_PivaSuperUser = g2g.To_PivaSuperUser
        Dim From_Data = objParametri.FinestraTemporaleInizio
        Dim To_Data = objParametri.FinestraTemporaleFine

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim listaAppezzamentiInsert = (
                From a In GiasContext.Appezzamento
                Where a.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse a.SA_COD = Sa_Cod) AndAlso
                    a.Validita_Inizio <= To_Data AndAlso a.Validita_Fine >= From_Data AndAlso
                    Not GiasContext.G2G_Recode_Appezzamenti.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = a.PIVA AndAlso
                        g.To_Sa_Cod = a.SA_COD AndAlso g.To_Appezza = a.APPEZZA)
                Select a).ToList()

            For Each appezzamento In listaAppezzamentiInsert

                Dim filtroImpianto As Boolean = (From i In Impianti Where i.Piva = appezzamento.PIVA And i.Sa_Cod = appezzamento.SA_COD And i.Appezza = appezzamento.APPEZZA).ToList.Count > 0

                If Impianti.Count = 0 OrElse filtroImpianto Then
                    Dim utente = (From u In GiasContext.UtentiXAppezzamenti Where u.USER = From_PivaSuperUser AndAlso u.PIVA = appezzamento.PIVA AndAlso u.SA_COD = appezzamento.SA_COD AndAlso u.Appezza = appezzamento.APPEZZA Select u).FirstOrDefault()
                    Dim codici = (From c In GiasContext.Appezzamento_Codici Where c.PIVA = appezzamento.PIVA AndAlso c.sa_cod = appezzamento.SA_COD AndAlso c.appezza = appezzamento.APPEZZA Select c).ToList()
                    Dim particelle = (From p In GiasContext.AppezzamentiXParticelle Where p.PIVA = appezzamento.PIVA AndAlso p.SA_COD = appezzamento.SA_COD AndAlso p.APPEZZA = appezzamento.APPEZZA Select p).ToList()
                    Dim macrousi = (From pm In GiasContext.AppezzamentiXParticellexMacrousi Where pm.Piva = appezzamento.PIVA AndAlso pm.Sa_cod = appezzamento.SA_COD AndAlso pm.Appezza = appezzamento.APPEZZA Select pm).ToList()
                    Dim utilizzi = (From pmu In GiasContext.AppezzamentiXParticellexMacrousixUtilizzo Where pmu.Piva = appezzamento.PIVA AndAlso pmu.Sa_cod = appezzamento.SA_COD AndAlso pmu.Appezza = appezzamento.APPEZZA Select pmu).ToList()
                    g2g.AppezzamentiToInsert.Add(New G2G_Appezzamento With {.Appezzamento = appezzamento, .Utente = utente, .Codici = codici, .Particelle = particelle, .Macrousi = macrousi, .Utilizzi = utilizzi})
                End If

            Next

            Dim listaAppezzamentiUpdate = (
                From a In GiasContext.Appezzamento
                Where a.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse a.SA_COD = Sa_Cod) AndAlso
                    a.Validita_Inizio <= To_Data AndAlso a.Validita_Fine >= From_Data AndAlso
                    (GiasContext.G2G_Recode_Appezzamenti.Any(Function(g) g.From_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = a.PIVA AndAlso
                        g.To_Sa_Cod = a.SA_COD AndAlso g.To_Appezza = a.APPEZZA AndAlso g.datainvio < a.Data_Modifica) OrElse
                    (From ac In GiasContext.Appezzamento_Codici
                     Where ac.PIVA = From_Piva AndAlso (Sa_Cod = 0 OrElse ac.sa_cod = Sa_Cod) AndAlso
                     GiasContext.G2G_Recode_Appezzamenti.Any(Function(g) g.To_PivaSuperUser = From_PivaSuperUser AndAlso
                        g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = ac.PIVA AndAlso g.To_Sa_Cod = ac.sa_cod AndAlso g.To_Appezza = ac.appezza AndAlso g.datainvio < ac.Data_Modifica)
                     Select ac).Any(Function(x) x.PIVA = a.PIVA AndAlso x.sa_cod = a.SA_COD AndAlso x.appezza = a.APPEZZA))
                Select a).ToList()

            For Each appezzamento In listaAppezzamentiUpdate
                Dim utente = (From u In GiasContext.UtentiXAppezzamenti Where u.USER = To_PivaSuperUser AndAlso u.PIVA = appezzamento.PIVA AndAlso u.SA_COD = appezzamento.SA_COD AndAlso u.Appezza = appezzamento.APPEZZA Select u).FirstOrDefault()
                Dim codici = (From c In GiasContext.Appezzamento_Codici Where c.PIVA = appezzamento.PIVA AndAlso c.sa_cod = appezzamento.SA_COD AndAlso c.appezza = appezzamento.APPEZZA Select c).ToList()
                Dim particelle = (From p In GiasContext.AppezzamentiXParticelle Where p.PIVA = appezzamento.PIVA AndAlso p.SA_COD = appezzamento.SA_COD AndAlso p.APPEZZA = appezzamento.APPEZZA Select p).ToList()
                Dim macrousi = (From pm In GiasContext.AppezzamentiXParticellexMacrousi Where pm.Piva = appezzamento.PIVA AndAlso pm.Sa_cod = appezzamento.SA_COD AndAlso pm.Appezza = appezzamento.APPEZZA Select pm).ToList()
                Dim utilizzi = (From pmu In GiasContext.AppezzamentiXParticellexMacrousixUtilizzo Where pmu.Piva = appezzamento.PIVA AndAlso pmu.Sa_cod = appezzamento.SA_COD AndAlso pmu.Appezza = appezzamento.APPEZZA Select pmu).ToList()
                g2g.AppezzamentiToUpdate.Add(New G2G_Appezzamento With {.Appezzamento = appezzamento, .Utente = utente, .Codici = codici, .Particelle = particelle, .Macrousi = macrousi, .Utilizzi = utilizzi})
            Next

            g2g.Recode.G2GRecodeAppezzamentiToDelete = (
                From g In GiasContext.G2G_Recode_Appezzamenti
                Where g.To_PivaSuperUser = From_PivaSuperUser AndAlso g.From_PivaSuperUser = To_PivaSuperUser AndAlso g.From_Piva = From_Piva AndAlso (Sa_Cod = 0 OrElse g.To_Sa_Cod = Sa_Cod) AndAlso
                      Not GiasContext.Appezzamento.Any(Function(a) a.PIVA = g.From_Piva AndAlso a.SA_COD = g.To_Sa_Cod AndAlso a.APPEZZA = g.To_Appezza)
                Select g).ToList()

        End Using

        Return g2g.AppezzamentiToInsert.Count > 0 OrElse g2g.AppezzamentiToUpdate.Count > 0 OrElse g2g.Recode.G2GRecodeAppezzamentiToDelete.Count > 0

    End Function

End Class

Public Class G2GAppezzamenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Appezzamenti_G2G(ByRef g2g As G2G_Appezzamenti, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GAppezzamenti_W.Scrivi_Appezzamenti_G2G()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_Appezzamenti(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_Appezzamenti(g2g, objParametri)

    End Function

    Public Function Scrivi_Appezzamenti_G2GReverse(ByRef g2g As G2G_Appezzamenti_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GAppezzamenti_W.Scrivi_Appezzamenti_G2GReverse()"
        Dim messaggioErrore As String = ""

        If G2GUtility.transactionScope Then

            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)
            Dim g2gRecode As G2G_Recode = g2g.Recode

            Try

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    g2gRecode = Scrivi_AppezzamentiReverse(g2g, objParametri)
                    scope.Complete()
                End Using

            Catch ex As Exception

                messaggioErrore = ex.Message
                g2gRecode.MessaggioErrore = messaggioErrore
                Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            End Try

            Return g2gRecode

        End If

        Return Scrivi_AppezzamentiReverse(g2g, objParametri)

    End Function

    Public Function Scrivi_Appezzamenti(ByRef g2g As G2G_Appezzamenti, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GAppezzamenti_W.Scrivi_Appezzamenti()"
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

                ' APPEZZAMENTI DA INSERIRE
                If g2g.AppezzamentiToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    Dim listAppezzamento As New List(Of AgronicaCoreEntityFramework_POCO.Appezzamento)
                    Dim listUtentiXAppezzamenti As New List(Of AgronicaCoreEntityFramework_POCO.UtentiXAppezzamenti)
                    Dim listAppezzamento_Codici As New List(Of AgronicaCoreEntityFramework_POCO.Appezzamento_Codici)
                    Dim listAppezzamentiXParticelle As New List(Of AgronicaCoreEntityFramework_POCO.AppezzamentiXParticelle)
                    Dim listAppezzamentiXParticellexMacrousi As New List(Of AgronicaCoreEntityFramework_POCO.AppezzamentiXParticellexMacrousi)
                    Dim listAppezzamentiXParticellexMacrousixUtilizzo As New List(Of AgronicaCoreEntityFramework_POCO.AppezzamentiXParticellexMacrousixUtilizzo)
                    Dim listG2G_Recode_Appezzamenti As New List(Of AgronicaCoreEntityFramework_POCO.G2G_Recode_Appezzamenti)

                    Dim listG2G_Recode_Campo As List(Of AgronicaCoreEntityFramework_POCO.G2G_Recode_Campo)

                    Dim listG2G_Recode_Appezzamenti_GiaInseriti As List(Of AgronicaCoreEntityFramework_POCO.G2G_Recode_Appezzamenti)

                    Dim firstFromPiva = g2g.AppezzamentiToInsert(0).Appezzamento.PIVA
                    listG2G_Recode_Appezzamenti_GiaInseriti = (From rr In GiasContext.G2G_Recode_Appezzamenti
                                                               Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso
                                                               rr.To_PivaSuperUser = To_PivaSuperUser AndAlso
                                                               rr.From_Piva = firstFromPiva).ToList()

                    For Each a As G2G_Appezzamento In g2g.AppezzamentiToInsert

                        If listG2G_Recode_Campo Is Nothing Then
                            listG2G_Recode_Campo = (From rr In GiasContext.G2G_Recode_Campo
                                                    Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso
                                                        rr.To_PivaSuperUser = To_PivaSuperUser AndAlso
                                                        rr.From_Piva = a.Appezzamento.PIVA).ToList
                        End If

                        Dim To_Campo_Cod As Integer = 0
                        If a.Appezzamento.Campo_Cod <> 0 Then
                            Dim list_recode_Campi = (From rr In listG2G_Recode_Campo Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = a.Appezzamento.PIVA AndAlso rr.From_Sa_Cod = a.Appezzamento.SA_COD AndAlso rr.From_Campo_cod = a.Appezzamento.Campo_Cod Select (rr.To_Campo_cod)).ToList()
                            If list_recode_Campi.Count = 0 Then
                                Throw New Exception("Recode campo non trovato --> Piva:" & a.Appezzamento.PIVA & " Sa_Cod:" & a.Appezzamento.SA_COD & "  Campo_Cod:" & a.Appezzamento.Campo_Cod)
                            End If
                            If list_recode_Campi.Count > 1 Then
                                Throw New Exception("Recode Multipli per campo --> Piva:" & a.Appezzamento.PIVA & " Sa_Cod:" & a.Appezzamento.SA_COD & "  Campo_Cod:" & a.Appezzamento.Campo_Cod)
                            End If
                            To_Campo_Cod = list_recode_Campi(0)
                        End If

                        Dim recodeEsistenti = (From r In listG2G_Recode_Appezzamenti_GiaInseriti Where r.From_Piva = a.Appezzamento.PIVA And
                                                                                              r.From_Sa_Cod = a.Appezzamento.SA_COD AndAlso
                                                                                              r.From_Appezza = a.Appezzamento.APPEZZA).ToList


                        If recodeEsistenti.Count = 0 Then
                            ' nuovo appezzamento
                            Dim idSeq = objSequenze.NuovoId_Appezzamento(To_Piva, To_Sa_Cod, BaseCode, TopCode, objParametri)
                            Dim appezzamento = Gias_EF_Utility.CopyEntity(GiasContext, a.Appezzamento, Nothing, username, data)
                            appezzamento.PIVA = To_Piva
                            appezzamento.SA_COD = To_Sa_Cod
                            appezzamento.Campo_Cod = To_Campo_Cod
                            appezzamento.APPEZZA = idSeq
                            appezzamento.Blk_Flag = -1
                            'GiasContext.Appezzamento.Add(appezzamento)
                            listAppezzamento.Add(appezzamento)

                            ' inserisce utente appezzamento
                            Dim utente = Gias_EF_Utility.CopyEntity(GiasContext, a.Utente, Nothing, username, data)
                            utente.USER = To_PivaSuperUser
                            utente.PIVA = appezzamento.PIVA
                            utente.SA_COD = appezzamento.SA_COD
                            utente.Appezza = appezzamento.APPEZZA
                            'GiasContext.UtentiXAppezzamenti.Add(utente)
                            listUtentiXAppezzamenti.Add(utente)

                            ' inserisce codici appezzamento
                            For Each cc As Appezzamento_Codici In a.Codici
                                Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                                codice.PIVA = appezzamento.PIVA
                                codice.sa_cod = appezzamento.SA_COD
                                codice.appezza = appezzamento.APPEZZA
                                'GiasContext.Appezzamento_Codici.Add(codice)
                                listAppezzamento_Codici.Add(codice)
                            Next

                            ' inserisce particelle appezzamento
                            For Each particella As AppezzamentiXParticelle In a.Particelle
                                Dim appezzamento_particella = Gias_EF_Utility.CopyEntity(GiasContext, particella, Nothing, username, data)
                                appezzamento_particella.PIVA = appezzamento.PIVA
                                appezzamento_particella.SA_COD = appezzamento.SA_COD
                                appezzamento_particella.APPEZZA = appezzamento.APPEZZA
                                'GiasContext.AppezzamentiXParticelle.Add(appezzamento_particella)
                                listAppezzamentiXParticelle.Add(appezzamento_particella)
                            Next

                            ' inserisce macrousi particelle
                            For Each macrouso As AppezzamentiXParticellexMacrousi In a.Macrousi
                                Dim macrouso_particella = Gias_EF_Utility.CopyEntity(GiasContext, macrouso, Nothing, username, data)
                                macrouso_particella.Piva = appezzamento.PIVA
                                macrouso_particella.Sa_cod = appezzamento.SA_COD
                                macrouso_particella.Appezza = appezzamento.APPEZZA
                                GiasContext.AppezzamentiXParticellexMacrousi.Add(macrouso_particella)
                                listAppezzamentiXParticellexMacrousi.Add(macrouso_particella)
                            Next

                            ' inserisce macrousi utilizzi particelle
                            For Each utilizzo As AppezzamentiXParticellexMacrousixUtilizzo In a.Utilizzi
                                Dim utilizzo_particella = Gias_EF_Utility.CopyEntity(GiasContext, utilizzo, Nothing, username, data)
                                utilizzo_particella.Piva = appezzamento.PIVA
                                utilizzo_particella.Sa_cod = appezzamento.SA_COD
                                utilizzo_particella.Appezza = appezzamento.APPEZZA
                                GiasContext.AppezzamentiXParticellexMacrousixUtilizzo.Add(utilizzo_particella)
                                listAppezzamentiXParticellexMacrousixUtilizzo.Add(utilizzo_particella)
                            Next

                            'nuovo recode appezzamento
                            Dim recode =
                        New G2G_Recode_Appezzamenti With {
                            .From_PivaSuperUser = From_PivaSuperUser,
                            .To_PivaSuperUser = To_PivaSuperUser,
                            .From_Piva = a.Appezzamento.PIVA,
                            .To_Piva = appezzamento.PIVA,
                            .From_Sa_Cod = a.Appezzamento.SA_COD,
                            .To_Sa_Cod = appezzamento.SA_COD,
                            .From_Appezza = a.Appezzamento.APPEZZA,
                            .To_Appezza = appezzamento.APPEZZA,
                            .Username_Creazione = username,
                            .Username_Modifica = username,
                            .Data_Creazione = Now(),
                            .Data_Modifica = Now(),
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .inviato = 0,
                            .datainvio = Now()
                        }
                            g2g.Recode.G2GRecodeAppezzamentiToInsert.Add(recode)
                            'GiasContext.G2G_Recode_Appezzamenti.Add(recode)
                            listG2G_Recode_Appezzamenti.Add(recode)

                            'G2GUtility.SaveChanges(GiasContext, index)

                        Else
                            g2g.Recode.G2GRecodeAppezzamentiToInsert.Add(recodeEsistenti(0))
                            Dim messaggio = "Appezzamento non importato: " & JsonConvert.SerializeObject(a.Appezzamento) & vbCrLf &
                                "Recode già presente"
                            Scrivi_LOG(objParametri, nomeRoutine, messaggio)

                        End If


                    Next

                    GiasContext.Appezzamento.AddRange(listAppezzamento)
                    GiasContext.UtentiXAppezzamenti.AddRange(listUtentiXAppezzamenti)
                    GiasContext.Appezzamento_Codici.AddRange(listAppezzamento_Codici)
                    GiasContext.AppezzamentiXParticelle.AddRange(listAppezzamentiXParticelle)
                    GiasContext.AppezzamentiXParticellexMacrousi.AddRange(listAppezzamentiXParticellexMacrousi)
                    GiasContext.AppezzamentiXParticellexMacrousixUtilizzo.AddRange(listAppezzamentiXParticellexMacrousixUtilizzo)
                    GiasContext.G2G_Recode_Appezzamenti.AddRange(listG2G_Recode_Appezzamenti)

                    GiasContext.SaveChanges()

                End If

                ' APPEZZAMENTI DA MODIFICARE
                If g2g.AppezzamentiToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    ' appezzamenti da modificare
                    For Each a As G2G_Appezzamento In g2g.AppezzamentiToUpdate

                        Dim To_Campo_Cod As Integer = 0
                        If a.Appezzamento.Campo_Cod <> 0 Then
                            To_Campo_Cod = (From rr In GiasContext.G2G_Recode_Campo Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = a.Appezzamento.PIVA AndAlso rr.From_Sa_Cod = a.Appezzamento.SA_COD AndAlso rr.From_Campo_cod = a.Appezzamento.Campo_Cod Select (rr.To_Campo_cod)).FirstOrDefault()
                        End If

                        ' modifica recode appezzamento
                        Dim recode = (From rr In GiasContext.G2G_Recode_Appezzamenti Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = a.Appezzamento.PIVA AndAlso rr.From_Sa_Cod = a.Appezzamento.SA_COD AndAlso rr.From_Appezza = a.Appezzamento.APPEZZA).FirstOrDefault()
                        If recode Is Nothing Then
                            Throw New Exception("Recode non presente in destinazione")
                        End If
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodeAppezzamentiToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Appezzamenti.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        ' modifica appezzamento
                        Dim appezzamento = (From aa In GiasContext.Appezzamento Where aa.PIVA = recode.To_Piva AndAlso aa.SA_COD = recode.To_Sa_Cod AndAlso aa.APPEZZA = recode.To_Appezza).FirstOrDefault()
                        If appezzamento Is Nothing Then
                            Throw New Exception("Appezzamento non presente in destinazione")
                        End If
                        appezzamento = Gias_EF_Utility.CopyEntity(GiasContext, a.Appezzamento, appezzamento, username, data)
                        appezzamento.PIVA = recode.To_Piva
                        appezzamento.SA_COD = recode.To_Sa_Cod
                        appezzamento.APPEZZA = recode.To_Appezza
                        appezzamento.Campo_Cod = To_Campo_Cod
                        appezzamento.Blk_Flag = -1
                        GiasContext.Appezzamento.Attach(appezzamento)
                        GiasContext.Entry(appezzamento).State = EntityState.Modified

                        ' cancella codici appezzamento
                        Dim codici = (From cc In GiasContext.Appezzamento_Codici Where cc.PIVA = appezzamento.PIVA AndAlso cc.sa_cod = appezzamento.SA_COD AndAlso cc.appezza = appezzamento.APPEZZA Select cc).ToList()
                        For Each cc As Appezzamento_Codici In codici
                            GiasContext.Appezzamento_Codici.Attach(cc)
                            GiasContext.Appezzamento_Codici.Remove(cc)
                        Next

                        ' inserisce codici appezzamento
                        For Each cc As Appezzamento_Codici In a.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = appezzamento.PIVA
                            codice.sa_cod = appezzamento.SA_COD
                            codice.appezza = appezzamento.APPEZZA
                            GiasContext.Appezzamento_Codici.Add(codice)
                        Next

                        Dim appezzamento_particelle = (From ap In GiasContext.AppezzamentiXParticelle Where ap.PIVA = appezzamento.PIVA And ap.SA_COD = appezzamento.SA_COD And ap.APPEZZA = appezzamento.APPEZZA).ToList()

                        ' cancella particelle appezzamento
                        For Each ap In appezzamento_particelle
                            GiasContext.AppezzamentiXParticelle.Attach(ap)
                            GiasContext.AppezzamentiXParticelle.Remove(ap)
                        Next

                        Dim macrousi_particelle = (From mp In GiasContext.AppezzamentiXParticellexMacrousi Where mp.Piva = appezzamento.PIVA And mp.Sa_cod = appezzamento.SA_COD And mp.Appezza = appezzamento.APPEZZA).ToList()

                        ' cancella macrousi particelle
                        For Each mp In macrousi_particelle
                            GiasContext.AppezzamentiXParticellexMacrousi.Attach(mp)
                            GiasContext.AppezzamentiXParticellexMacrousi.Remove(mp)
                        Next

                        Dim utilizzi_particelle = (From mup In GiasContext.AppezzamentiXParticellexMacrousixUtilizzo Where mup.Piva = appezzamento.PIVA And mup.Sa_cod = appezzamento.SA_COD And mup.Appezza = appezzamento.APPEZZA).ToList()

                        ' cancella macrousi utilizzi particelle
                        For Each up In utilizzi_particelle
                            GiasContext.AppezzamentiXParticellexMacrousixUtilizzo.Attach(up)
                            GiasContext.AppezzamentiXParticellexMacrousixUtilizzo.Remove(up)
                        Next

                        ' inserisce particelle appezzamento
                        For Each particella As AppezzamentiXParticelle In a.Particelle
                            Dim appezzamento_particella = Gias_EF_Utility.CopyEntity(GiasContext, particella, Nothing, username, data)
                            appezzamento_particella.PIVA = appezzamento.PIVA
                            appezzamento_particella.SA_COD = appezzamento.SA_COD
                            appezzamento_particella.APPEZZA = appezzamento.APPEZZA
                            GiasContext.AppezzamentiXParticelle.Add(appezzamento_particella)
                        Next

                        ' inserisce macrousi particelle appezzamento
                        For Each macrouso As AppezzamentiXParticellexMacrousi In a.Macrousi
                            Dim macrouso_particella = Gias_EF_Utility.CopyEntity(GiasContext, macrouso, Nothing, username, data)
                            macrouso_particella.Piva = appezzamento.PIVA
                            macrouso_particella.Sa_cod = appezzamento.SA_COD
                            macrouso_particella.Appezza = appezzamento.APPEZZA
                            GiasContext.AppezzamentiXParticellexMacrousi.Add(macrouso_particella)
                        Next

                        ' inserisce macrouso utilizzo particelle appezzamento
                        For Each utilizzo As AppezzamentiXParticellexMacrousixUtilizzo In a.Utilizzi
                            Dim utilizzo_particella = Gias_EF_Utility.CopyEntity(GiasContext, utilizzo, Nothing, username, data)
                            utilizzo_particella.Piva = appezzamento.PIVA
                            utilizzo_particella.Sa_cod = appezzamento.SA_COD
                            utilizzo_particella.Appezza = appezzamento.APPEZZA
                            GiasContext.AppezzamentiXParticellexMacrousixUtilizzo.Add(utilizzo_particella)
                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' APPEZZAMENTI DA CANCELLARE
                If g2g.Recode.G2GRecodeAppezzamentiToDelete.Count > 0 Then

                    Dim index As Integer = 0

                    For Each r As G2G_Recode_Appezzamenti In g2g.Recode.G2GRecodeAppezzamentiToDelete

                        Dim appezzamento = (From a In GiasContext.Appezzamento Where a.PIVA = r.To_Piva AndAlso a.SA_COD = r.To_Sa_Cod AndAlso a.APPEZZA = r.To_Appezza).FirstOrDefault()

                        'cancella appezzamento
                        If appezzamento IsNot Nothing Then

                            ' cancella codici appezzamento
                            Dim codici = (From cc In GiasContext.Appezzamento_Codici Where cc.PIVA = appezzamento.PIVA AndAlso cc.sa_cod = appezzamento.SA_COD AndAlso cc.appezza = appezzamento.APPEZZA Select cc).ToList()
                            For Each cc As Appezzamento_Codici In codici
                                Dim ccEntry = GiasContext.Entry(cc)
                                If ccEntry.State = EntityState.Detached Then
                                    GiasContext.Appezzamento_Codici.Attach(cc)
                                End If
                                If ccEntry.State <> EntityState.Deleted Then
                                    GiasContext.Appezzamento_Codici.Remove(cc)
                                End If
                            Next

                            Dim appezzamento_particelle = (From ap In GiasContext.AppezzamentiXParticelle Where ap.PIVA = appezzamento.PIVA And ap.SA_COD = appezzamento.SA_COD And ap.APPEZZA = appezzamento.APPEZZA).ToList()

                            ' cancella particelle appezzamento
                            For Each ap In appezzamento_particelle
                                Dim apEntry = GiasContext.Entry(ap)
                                If apEntry.State = EntityState.Detached Then
                                    GiasContext.AppezzamentiXParticelle.Attach(ap)
                                End If
                                If apEntry.State <> EntityState.Deleted Then
                                    GiasContext.AppezzamentiXParticelle.Remove(ap)
                                End If
                            Next

                            Dim macrousi_particelle = (From mp In GiasContext.AppezzamentiXParticellexMacrousi Where mp.Piva = appezzamento.PIVA And mp.Sa_cod = appezzamento.SA_COD And mp.Appezza = appezzamento.APPEZZA).ToList()

                            ' cancella macrousi particelle
                            For Each mp In macrousi_particelle
                                Dim mpEntry = GiasContext.Entry(mp)
                                If mpEntry.State = EntityState.Detached Then
                                    GiasContext.AppezzamentiXParticellexMacrousi.Attach(mp)
                                End If
                                If mpEntry.State <> EntityState.Deleted Then
                                    GiasContext.AppezzamentiXParticellexMacrousi.Remove(mp)
                                End If
                            Next

                            Dim utilizzi_particelle = (From mup In GiasContext.AppezzamentiXParticellexMacrousixUtilizzo Where mup.Piva = appezzamento.PIVA And mup.Sa_cod = appezzamento.SA_COD And mup.Appezza = appezzamento.APPEZZA).ToList()

                            ' cancella macrousi utilizzi particelle
                            For Each up In utilizzi_particelle
                                Dim upEntry = GiasContext.Entry(up)
                                If upEntry.State = EntityState.Detached Then
                                    GiasContext.AppezzamentiXParticellexMacrousixUtilizzo.Attach(up)
                                End If
                                If upEntry.State <> EntityState.Deleted Then
                                    GiasContext.AppezzamentiXParticellexMacrousixUtilizzo.Remove(up)
                                End If
                            Next

                            'cancella appezzamento
                            Dim appezzamentoEntry = GiasContext.Entry(appezzamento)
                            If appezzamentoEntry.State = EntityState.Detached Then
                                GiasContext.Appezzamento.Attach(appezzamento)
                            End If
                            If appezzamentoEntry.State <> EntityState.Deleted Then
                                GiasContext.Appezzamento.Remove(appezzamento)
                            End If

                        End If

                        ' cancella recode appezzamento
                        Dim recodeToDelete As G2G_Recode_Appezzamenti = r

                        Dim trackedRecode = GiasContext.G2G_Recode_Appezzamenti.Local.FirstOrDefault(Function(x) _
                            x.From_PivaSuperUser = recodeToDelete.From_PivaSuperUser AndAlso
                            x.To_PivaSuperUser = recodeToDelete.To_PivaSuperUser AndAlso
                            x.From_Piva = recodeToDelete.From_Piva AndAlso
                            x.To_Piva = recodeToDelete.To_Piva AndAlso
                            x.From_Sa_Cod = recodeToDelete.From_Sa_Cod AndAlso
                            x.To_Sa_Cod = recodeToDelete.To_Sa_Cod AndAlso
                            x.From_Appezza = recodeToDelete.From_Appezza AndAlso
                            x.To_Appezza = recodeToDelete.To_Appezza)

                        If trackedRecode IsNot Nothing Then
                            recodeToDelete = trackedRecode
                        Else
                            Dim recodeEntry = GiasContext.Entry(r)
                            If recodeEntry.State = EntityState.Detached Then
                                Dim existsInDB = GiasContext.G2G_Recode_Appezzamenti.Any(Function(x) _
                                    x.From_PivaSuperUser = r.From_PivaSuperUser AndAlso
                                    x.To_PivaSuperUser = r.To_PivaSuperUser AndAlso
                                    x.From_Piva = r.From_Piva AndAlso
                                    x.To_Piva = r.To_Piva AndAlso
                                    x.From_Sa_Cod = r.From_Sa_Cod AndAlso
                                    x.To_Sa_Cod = r.To_Sa_Cod AndAlso
                                    x.From_Appezza = r.From_Appezza AndAlso
                                    x.To_Appezza = r.To_Appezza)

                                If existsInDB Then
                                    GiasContext.G2G_Recode_Appezzamenti.Attach(r)
                                Else
                                    ' Il record è già stato eliminato, salta questo elemento
                                    g2g.Recode.LogRecode &= objParametri.Recupera_NomeDB() & " - Recode Appezzamento già eliminato: " & r.From_PivaSuperUser & " -> " & r.To_PivaSuperUser & " - " & r.From_Piva & " -> " & r.To_Piva & " - " & r.From_Sa_Cod & " -> " & r.To_Sa_Cod & " - " & r.From_Appezza & " -> " & r.To_Appezza & Environment.NewLine
                                    Continue For
                                End If
                            End If
                        End If

                        Dim finalEntry = GiasContext.Entry(recodeToDelete)
                        If finalEntry.State <> EntityState.Deleted Then
                            GiasContext.G2G_Recode_Appezzamenti.Remove(recodeToDelete)
                        End If

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode &= " - appezzamenti: " & g2g.AppezzamentiToInsert.Count & " nuovi, " & g2g.AppezzamentiToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeAppezzamentiToDelete.Count & " cancellati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.ToString
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_AppezzamentiReverse(ByRef g2g As G2G_Appezzamenti_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GAppezzamenti_W.Scrivi_Appezzamenti()"
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

                ' APPEZZAMENTI DA INSERIRE
                If g2g.AppezzamentiToInsert.Count > 0 Then

                    Dim index As Integer = 0

                    For Each a As G2G_Appezzamento In g2g.AppezzamentiToInsert

                        Dim To_Campo_Cod As Integer = 0
                        If a.Appezzamento.Campo_Cod <> 0 Then
                            To_Campo_Cod = (From rr In GiasContext.G2G_Recode_Campo Where rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_Piva = a.Appezzamento.PIVA AndAlso rr.To_Sa_Cod = a.Appezzamento.SA_COD AndAlso rr.To_Campo_cod = a.Appezzamento.Campo_Cod Select (rr.From_Campo_cod)).FirstOrDefault()
                        End If

                        ' nuovo appezzamento
                        Dim idSeq = objSequenze.NuovoId_Appezzamento(To_Piva, To_Sa_Cod, BaseCode, TopCode, objParametri)
                        Dim appezzamento = Gias_EF_Utility.CopyEntity(GiasContext, a.Appezzamento, Nothing, username, data)
                        appezzamento.PIVA = To_Piva
                        appezzamento.SA_COD = To_Sa_Cod
                        appezzamento.Campo_Cod = To_Campo_Cod
                        appezzamento.APPEZZA = idSeq
                        appezzamento.Blk_Flag = -1
                        GiasContext.Appezzamento.Add(appezzamento)

                        ' inserisce utente appezzamento
                        Dim utente = Gias_EF_Utility.CopyEntity(GiasContext, a.Utente, Nothing, username, data)
                        utente.USER = To_PivaSuperUser
                        utente.PIVA = appezzamento.PIVA
                        utente.SA_COD = appezzamento.SA_COD
                        utente.Appezza = appezzamento.APPEZZA
                        GiasContext.UtentiXAppezzamenti.Add(utente)

                        ' inserisce codici appezzamento
                        For Each cc As Appezzamento_Codici In a.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = appezzamento.PIVA
                            codice.sa_cod = appezzamento.SA_COD
                            codice.appezza = appezzamento.APPEZZA
                            GiasContext.Appezzamento_Codici.Add(codice)
                        Next

                        ' inserisce particelle appezzamento
                        For Each particella As AppezzamentiXParticelle In a.Particelle
                            Dim appezzamento_particella = Gias_EF_Utility.CopyEntity(GiasContext, particella, Nothing, username, data)
                            appezzamento_particella.PIVA = appezzamento.PIVA
                            appezzamento_particella.SA_COD = appezzamento.SA_COD
                            appezzamento_particella.APPEZZA = appezzamento.APPEZZA
                            GiasContext.AppezzamentiXParticelle.Add(appezzamento_particella)
                        Next

                        ' inserisce macrousi particelle
                        For Each macrouso As AppezzamentiXParticellexMacrousi In a.Macrousi
                            Dim macrouso_particella = Gias_EF_Utility.CopyEntity(GiasContext, macrouso, Nothing, username, data)
                            macrouso_particella.Piva = appezzamento.PIVA
                            macrouso_particella.Sa_cod = appezzamento.SA_COD
                            macrouso_particella.Appezza = appezzamento.APPEZZA
                            GiasContext.AppezzamentiXParticellexMacrousi.Add(macrouso_particella)
                        Next

                        ' inserisce macrousi utilizzi particelle
                        For Each utilizzo As AppezzamentiXParticellexMacrousixUtilizzo In a.Utilizzi
                            Dim utilizzo_particella = Gias_EF_Utility.CopyEntity(GiasContext, utilizzo, Nothing, username, data)
                            utilizzo_particella.Piva = appezzamento.PIVA
                            utilizzo_particella.Sa_cod = appezzamento.SA_COD
                            utilizzo_particella.Appezza = appezzamento.APPEZZA
                            GiasContext.AppezzamentiXParticellexMacrousixUtilizzo.Add(utilizzo_particella)
                        Next

                        'nuovo recode appezzamento
                        Dim recode =
                        New G2G_Recode_Appezzamenti With {
                            .From_PivaSuperUser = To_PivaSuperUser,
                            .To_PivaSuperUser = From_PivaSuperUser,
                            .From_Piva = a.Appezzamento.PIVA,
                            .To_Piva = appezzamento.PIVA,
                            .From_Sa_Cod = appezzamento.SA_COD,
                            .To_Sa_Cod = a.Appezzamento.SA_COD,
                            .From_Appezza = appezzamento.APPEZZA,
                            .To_Appezza = a.Appezzamento.APPEZZA,
                            .Username_Creazione = username,
                            .Username_Modifica = username,
                            .Data_Creazione = Now(),
                            .Data_Modifica = Now(),
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .inviato = 0,
                            .datainvio = Now()
                        }
                        g2g.Recode.G2GRecodeAppezzamentiToInsert.Add(recode)
                        GiasContext.G2G_Recode_Appezzamenti.Add(recode)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' APPEZZAMENTI DA MODIFICARE
                If g2g.AppezzamentiToUpdate.Count > 0 Then

                    Dim index As Integer = 0

                    ' appezzamenti da modificare
                    For Each a As G2G_Appezzamento In g2g.AppezzamentiToUpdate

                        Dim From_Campo_Cod As Integer = 0
                        If a.Appezzamento.Campo_Cod <> 0 Then
                            From_Campo_Cod = (From rr In GiasContext.G2G_Recode_Campo Where rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_Piva = a.Appezzamento.PIVA AndAlso rr.To_Sa_Cod = a.Appezzamento.SA_COD AndAlso rr.To_Campo_cod = a.Appezzamento.Campo_Cod Select (rr.From_Campo_cod)).FirstOrDefault()
                        End If

                        ' modifica recode appezzamento
                        Dim recode = (From rr In GiasContext.G2G_Recode_Appezzamenti Where rr.To_PivaSuperUser = From_PivaSuperUser AndAlso rr.From_PivaSuperUser = To_PivaSuperUser AndAlso rr.To_Piva = a.Appezzamento.PIVA AndAlso rr.To_Sa_Cod = a.Appezzamento.SA_COD AndAlso rr.To_Appezza = a.Appezzamento.APPEZZA).FirstOrDefault()
                        If recode Is Nothing Then
                            Throw New Exception("Recode non presente in destinazione")
                        End If
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.Recode.G2GRecodeAppezzamentiToUpdate.Add(recode)
                        GiasContext.G2G_Recode_Appezzamenti.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        ' modifica appezzamento
                        Dim appezzamento = (From aa In GiasContext.Appezzamento Where aa.PIVA = recode.From_Piva AndAlso aa.SA_COD = recode.From_Sa_Cod AndAlso aa.APPEZZA = recode.From_Appezza).FirstOrDefault()
                        If appezzamento Is Nothing Then
                            Throw New Exception("Appezzamento non presente in destinazione")
                        End If
                        appezzamento = Gias_EF_Utility.CopyEntity(GiasContext, a.Appezzamento, appezzamento, username, data)
                        appezzamento.PIVA = recode.From_Piva
                        appezzamento.SA_COD = recode.From_Sa_Cod
                        appezzamento.APPEZZA = recode.From_Appezza
                        appezzamento.Campo_Cod = From_Campo_Cod
                        appezzamento.Blk_Flag = -1
                        GiasContext.Appezzamento.Attach(appezzamento)
                        GiasContext.Entry(appezzamento).State = EntityState.Modified

                        ' cancella codici appezzamento
                        Dim codici = (From cc In GiasContext.Appezzamento_Codici Where cc.PIVA = appezzamento.PIVA AndAlso cc.sa_cod = appezzamento.SA_COD AndAlso cc.appezza = appezzamento.APPEZZA Select cc).ToList()
                        For Each cc As Appezzamento_Codici In codici
                            GiasContext.Appezzamento_Codici.Attach(cc)
                            GiasContext.Appezzamento_Codici.Remove(cc)
                        Next

                        ' inserisce codici appezzamento
                        For Each cc As Appezzamento_Codici In a.Codici
                            Dim codice = Gias_EF_Utility.CopyEntity(GiasContext, cc, Nothing, username, data)
                            codice.PIVA = appezzamento.PIVA
                            codice.sa_cod = appezzamento.SA_COD
                            codice.appezza = appezzamento.APPEZZA
                            GiasContext.Appezzamento_Codici.Add(codice)
                        Next

                        Dim appezzamento_particelle = (From ap In GiasContext.AppezzamentiXParticelle Where ap.PIVA = appezzamento.PIVA And ap.SA_COD = appezzamento.SA_COD And ap.APPEZZA = appezzamento.APPEZZA).ToList()

                        ' cancella particelle appezzamento
                        For Each ap In appezzamento_particelle
                            GiasContext.AppezzamentiXParticelle.Attach(ap)
                            GiasContext.AppezzamentiXParticelle.Remove(ap)
                        Next

                        Dim macrousi_particelle = (From mp In GiasContext.AppezzamentiXParticellexMacrousi Where mp.Piva = appezzamento.PIVA And mp.Sa_cod = appezzamento.SA_COD And mp.Appezza = appezzamento.APPEZZA).ToList()

                        ' cancella macrousi particelle
                        For Each mp In macrousi_particelle
                            GiasContext.AppezzamentiXParticellexMacrousi.Attach(mp)
                            GiasContext.AppezzamentiXParticellexMacrousi.Remove(mp)
                        Next

                        Dim utilizzi_particelle = (From mup In GiasContext.AppezzamentiXParticellexMacrousixUtilizzo Where mup.Piva = appezzamento.PIVA And mup.Sa_cod = appezzamento.SA_COD And mup.Appezza = appezzamento.APPEZZA).ToList()

                        ' cancella macrousi utilizzi particelle
                        For Each up In utilizzi_particelle
                            GiasContext.AppezzamentiXParticellexMacrousixUtilizzo.Attach(up)
                            GiasContext.AppezzamentiXParticellexMacrousixUtilizzo.Remove(up)
                        Next

                        ' inserisce particelle appezzamento
                        For Each particella As AppezzamentiXParticelle In a.Particelle
                            Dim appezzamento_particella = Gias_EF_Utility.CopyEntity(GiasContext, particella, Nothing, username, data)
                            appezzamento_particella.PIVA = appezzamento.PIVA
                            appezzamento_particella.SA_COD = appezzamento.SA_COD
                            appezzamento_particella.APPEZZA = appezzamento.APPEZZA
                            GiasContext.AppezzamentiXParticelle.Add(appezzamento_particella)
                        Next

                        ' inserisce macrousi particelle appezzamento
                        For Each macrouso As AppezzamentiXParticellexMacrousi In a.Macrousi
                            Dim macrouso_particella = Gias_EF_Utility.CopyEntity(GiasContext, macrouso, Nothing, username, data)
                            macrouso_particella.Piva = appezzamento.PIVA
                            macrouso_particella.Sa_cod = appezzamento.SA_COD
                            macrouso_particella.Appezza = appezzamento.APPEZZA
                            GiasContext.AppezzamentiXParticellexMacrousi.Add(macrouso_particella)
                        Next

                        ' inserisce macrouso utilizzo particelle appezzamento
                        For Each utilizzo As AppezzamentiXParticellexMacrousixUtilizzo In a.Utilizzi
                            Dim utilizzo_particella = Gias_EF_Utility.CopyEntity(GiasContext, utilizzo, Nothing, username, data)
                            utilizzo_particella.Piva = appezzamento.PIVA
                            utilizzo_particella.Sa_cod = appezzamento.SA_COD
                            utilizzo_particella.Appezza = appezzamento.APPEZZA
                            GiasContext.AppezzamentiXParticellexMacrousixUtilizzo.Add(utilizzo_particella)
                        Next

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' APPEZZAMENTI DA CANCELLARE
                If g2g.Recode.G2GRecodeAppezzamentiToDelete.Count > 0 Then

                    Dim index As Integer = 0

                    For Each r As G2G_Recode_Appezzamenti In g2g.Recode.G2GRecodeAppezzamentiToDelete

                        Dim appezzamento = (From a In GiasContext.Appezzamento Where a.PIVA = r.To_Piva AndAlso a.SA_COD = r.To_Sa_Cod AndAlso a.APPEZZA = r.To_Appezza).FirstOrDefault()

                        'cancella appezzamento
                        If appezzamento IsNot Nothing Then

                            ' cancella codici appezzamento
                            Dim codici = (From cc In GiasContext.Appezzamento_Codici Where cc.PIVA = appezzamento.PIVA AndAlso cc.sa_cod = appezzamento.SA_COD AndAlso cc.appezza = appezzamento.APPEZZA Select cc).ToList()
                            For Each cc As Appezzamento_Codici In codici
                                GiasContext.Appezzamento_Codici.Attach(cc)
                                GiasContext.Appezzamento_Codici.Remove(cc)
                            Next

                            Dim appezzamento_particelle = (From ap In GiasContext.AppezzamentiXParticelle Where ap.PIVA = appezzamento.PIVA And ap.SA_COD = appezzamento.SA_COD And ap.APPEZZA = appezzamento.APPEZZA).ToList()

                            ' cancella particelle appezzamento
                            For Each ap In appezzamento_particelle
                                GiasContext.AppezzamentiXParticelle.Attach(ap)
                                GiasContext.AppezzamentiXParticelle.Remove(ap)
                            Next

                            Dim macrousi_particelle = (From mp In GiasContext.AppezzamentiXParticellexMacrousi Where mp.Piva = appezzamento.PIVA And mp.Sa_cod = appezzamento.SA_COD And mp.Appezza = appezzamento.APPEZZA).ToList()

                            ' cancella macrousi particelle
                            For Each mp In macrousi_particelle
                                GiasContext.AppezzamentiXParticellexMacrousi.Attach(mp)
                                GiasContext.AppezzamentiXParticellexMacrousi.Remove(mp)
                            Next

                            Dim utilizzi_particelle = (From mup In GiasContext.AppezzamentiXParticellexMacrousixUtilizzo Where mup.Piva = appezzamento.PIVA And mup.Sa_cod = appezzamento.SA_COD And mup.Appezza = appezzamento.APPEZZA).ToList()

                            ' cancella macrousi utilizzi particelle
                            For Each up In utilizzi_particelle
                                GiasContext.AppezzamentiXParticellexMacrousixUtilizzo.Attach(up)
                                GiasContext.AppezzamentiXParticellexMacrousixUtilizzo.Remove(up)
                            Next

                            'cancella appezzamento
                            GiasContext.Appezzamento.Attach(appezzamento)
                            GiasContext.Appezzamento.Remove(appezzamento)

                        End If

                        ' cancella recode appezzamento
                        GiasContext.G2G_Recode_Appezzamenti.Attach(r)
                        GiasContext.G2G_Recode_Appezzamenti.Remove(r)

                        G2GUtility.SaveChanges(GiasContext, index)

                    Next

                    GiasContext.SaveChanges()

                End If

                ' log importazione
                g2g.Recode.LogRecode &= " - appezzamenti: " & g2g.AppezzamentiToInsert.Count & " nuovi, " & g2g.AppezzamentiToUpdate.Count & " modificati, " & g2g.Recode.G2GRecodeAppezzamentiToDelete.Count & " cancellati"

            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

        End Try

        Return g2g.Recode

    End Function

End Class