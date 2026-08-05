Imports System.Data.Entity.Core.Common.EntitySql
Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class TempChiaviMassivo
    Inherits AgronicaCoreDataProvider.DataProvider

    'RICORDARSI DI APRIRE E CHIUDERE LA CONNESSIONE ANCHE IN CASO DI EXCEPTION
    'APERTURA CONNESSIONE--> AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri)
    'CHIUSURA TRANSAZIONE OK --> AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

    'CHIUSURA TRANSAZIONE KO --> AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)

    'CHIUSURA CONNESSIONE --> AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri)


    Public Shared chunkSize As Integer = 1000
#Region "Filtro Piva"
    Public Shared Sub CreaTabellaTemp_FiltroPiva(chiaviList As List(Of String),
                                          nomeRoutine As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        If chiaviList Is Nothing OrElse Not chiaviList.Any Then
            Throw New Exception("chiaviList obbligatorio per le letture massive!")
        End If

        Dim AgronicaCoreDataProvider As New AgronicaCoreDataProvider.DataProvider
        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempPiva') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempPiva ( ")
        stb.AppendLine("        Piva varchar(50)COLLATE DATABASE_DEFAULT NULL ")
        stb.AppendLine("    )")
        stb.AppendLine(" END ")

        AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, stb.ToString(), nomeRoutine)

        stb.Length = 0
        If Not IsNothing(chiaviList) AndAlso chiaviList.Any Then
            Dim chunks = ChunkBy(Of String)(chiaviList, chunkSize)
            For Each chunk In chunks
                stb.AppendLine("INSERT INTO #TempPiva (Piva) VALUES ")
                For Each p As String In chunk
                    stb.AppendLine(String.Format("('{0}'),", p))
                Next
                Dim strSqlInsert As String = stb.ToString
                strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                stb.Clear()
                AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, strSqlInsert, nomeRoutine)
            Next
        End If
    End Sub
    Public Shared Sub EliminaTabellaTemp_FiltroPiva(nomeRoutine As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim AgronicaCoreDataProvider As New AgronicaCoreDataProvider.DataProvider
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempPiva') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempPiva  ")
        stb.AppendLine(" END ")

        AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, stb.ToString(), nomeRoutine)
    End Sub

#End Region

#Region "Filtro Appezzamento"
    Public Shared Sub CreaTabellaTemp_FiltroAppezzamenti(chiaviList As List(Of (String, Integer, Integer)),
                                                         nomeRoutine As String,
                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        If chiaviList Is Nothing OrElse Not chiaviList.Any Then
            Throw New Exception("chiaviList obbligatorio per le letture massive!")
        End If

        Dim AgronicaCoreDataProvider As New AgronicaCoreDataProvider.DataProvider

        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempAppezzamento') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempAppezzamento ( ")
        stb.AppendLine("        Piva varchar(50)COLLATE DATABASE_DEFAULT NULL ")
        stb.AppendLine("      , Sa_Cod INT NULL")
        stb.AppendLine("      , Appezza INT NULL")
        stb.AppendLine("    )")
        stb.AppendLine(" END ")

        AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, stb.ToString(), nomeRoutine)

        stb.Length = 0

        If Not IsNothing(chiaviList) AndAlso chiaviList.Any Then
            Dim chunks = ChunkBy(Of (String, Integer, Integer))(chiaviList, chunkSize)
            For Each chunk In chunks
                stb.AppendLine("INSERT INTO #TempAppezzamento (Piva, Sa_Cod, Appezza) VALUES ")
                For Each p As (String, Integer, Integer) In chunk
                    stb.AppendLine(String.Format("('{0}', {1}, {2}),", p.Item1, p.Item2, p.Item3))
                Next
                Dim strSqlInsert As String = stb.ToString
                strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                stb.Clear()
                AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, strSqlInsert, nomeRoutine)
            Next
        End If
    End Sub

    Public Shared Sub EliminaTabellaTemp_FiltroAppezzamenti(nomeRoutine As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim AgronicaCoreDataProvider As New AgronicaCoreDataProvider.DataProvider
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempAppezzamento') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempAppezzamento ")
        stb.AppendLine(" END ")

        AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, stb.ToString(), nomeRoutine)
    End Sub

#End Region

#Region "Filtro Impianto"
    Public Shared Sub CreaTabellaTemp_FiltroImpianti(chiaviList As List(Of (String, Integer, Integer, Integer)),
                                                     nomeRoutine As String,
                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim AgronicaCoreDataProvider As New AgronicaCoreDataProvider.DataProvider

        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempImpianto') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempImpianto ( ")
        stb.AppendLine("        Piva varchar(50)COLLATE DATABASE_DEFAULT NULL ")
        stb.AppendLine("      , Sa_Cod INT NULL")
        stb.AppendLine("      , Appezza INT NULL")
        stb.AppendLine("      , Id_Reg INT NULL")
        stb.AppendLine("    )")
        stb.AppendLine(" END ")

        AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, stb.ToString(), nomeRoutine)

        stb.Length = 0

        If Not IsNothing(chiaviList) AndAlso chiaviList.Any Then
            Dim chunks = ChunkBy(Of (String, Integer, Integer, Integer))(chiaviList, chunkSize)
            For Each chunk In chunks
                stb.AppendLine("INSERT INTO #TempImpianto (Piva, Sa_Cod, Appezza, Id_Reg) VALUES ")
                For Each p As (String, Integer, Integer, Integer) In chunk
                    stb.AppendLine(String.Format("('{0}', {1}, {2}, {3}),", p.Item1, p.Item2, p.Item3, p.Item4))
                Next
                Dim strSqlInsert As String = stb.ToString
                strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                stb.Clear()
                AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, strSqlInsert, nomeRoutine)
            Next
        End If
    End Sub

    Public Shared Sub EliminaTabellaTemp_FiltroImpianti(nomeRoutine As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim AgronicaCoreDataProvider As New AgronicaCoreDataProvider.DataProvider
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempImpianto') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempImpianto ")
        stb.AppendLine(" END ")

        AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, stb.ToString(), nomeRoutine)
    End Sub

#End Region

#Region "Filtro Esercizio"
    Public Shared Sub CreaTabellaTemp_FiltroEsercizi(chiaviList As List(Of (String, Integer, Integer, Integer, Integer)),
                                                     nomeRoutine As String,
                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        If chiaviList Is Nothing OrElse Not chiaviList.Any Then
            Throw New Exception("chiaviList obbligatorio per le letture massive!")
        End If

        Dim AgronicaCoreDataProvider As New AgronicaCoreDataProvider.DataProvider

        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempEsercizio') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempEsercizio ( ")
        stb.AppendLine("        Piva varchar(50)COLLATE DATABASE_DEFAULT NULL ")
        stb.AppendLine("      , Sa_Cod INT NULL")
        stb.AppendLine("      , Appezza INT NULL")
        stb.AppendLine("      , Id_Reg INT NULL")
        stb.AppendLine("      , Progetto_Cod INT NULL")
        stb.AppendLine("    )")
        stb.AppendLine(" END ")

        AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, stb.ToString(), nomeRoutine)

        stb.Length = 0

        If Not IsNothing(chiaviList) AndAlso chiaviList.Any Then
            Dim chunks = ChunkBy(Of (String, Integer, Integer, Integer, Integer))(chiaviList, chunkSize)
            For Each chunk In chunks
                stb.AppendLine("INSERT INTO #TempEsercizio (Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod) VALUES ")
                For Each p As (String, Integer, Integer, Integer, Integer) In chunk
                    stb.AppendLine(String.Format("('{0}', {1}, {2}, {3}, {4}),", p.Item1, p.Item2, p.Item3, p.Item4, p.Item5))
                Next
                Dim strSqlInsert As String = stb.ToString
                strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                stb.Clear()
                AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, strSqlInsert, nomeRoutine)
            Next
        End If
    End Sub

    Public Shared Sub EliminaTabellaTemp_FiltroEsercizi(nomeRoutine As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim AgronicaCoreDataProvider As New AgronicaCoreDataProvider.DataProvider
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempEsercizio') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempEsercizio ")
        stb.AppendLine(" END ")

        AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, stb.ToString(), nomeRoutine)
    End Sub

    Public Shared Sub CreaTabellaTemp_FiltroProgetto(chiaviList As List(Of Integer),
                                                     nomeRoutine As String,
                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        If chiaviList Is Nothing OrElse Not chiaviList.Any Then
            Throw New Exception("chiaviList obbligatorio per le letture massive!")
        End If

        Dim AgronicaCoreDataProvider As New AgronicaCoreDataProvider.DataProvider

        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempProgetto') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempProgetto ( ")
        stb.AppendLine("        Progetto_Cod INT NULL")
        stb.AppendLine("    )")
        stb.AppendLine(" END ")

        AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, stb.ToString(), nomeRoutine)

        stb.Length = 0

        If Not IsNothing(chiaviList) AndAlso chiaviList.Any Then
            Dim chunks = ChunkBy(Of Integer)(chiaviList, chunkSize)
            For Each chunk In chunks
                stb.AppendLine("INSERT INTO #TempProgetto (Progetto_Cod) VALUES ")
                For Each p As Integer In chunk
                    stb.AppendLine(String.Format("({0}),", p))
                Next
                Dim strSqlInsert As String = stb.ToString
                strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                stb.Clear()
                AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, strSqlInsert, nomeRoutine)
            Next
        End If
    End Sub

    Public Shared Sub EliminaTabellaTemp_FiltroProgetto(nomeRoutine As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim AgronicaCoreDataProvider As New AgronicaCoreDataProvider.DataProvider
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempProgetto') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempProgetto ")
        stb.AppendLine(" END ")

        AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, stb.ToString(), nomeRoutine)
    End Sub

#End Region

#Region "Filtro Generico Chiave Stringa"
    Public Shared Sub CreaTabellaTemp_FiltroChiaveStringa(chiaviList As List(Of String),
                                                          nomeRoutine As String,
                                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                            Optional varcharSize As Integer = 50)

        If chiaviList Is Nothing OrElse Not chiaviList.Any Then
            Throw New Exception("chiaviList obbligatorio per le letture massive!")
        End If

        Dim AgronicaCoreDataProvider As New AgronicaCoreDataProvider.DataProvider
        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempChiave') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempChiave ( ")
        stb.AppendLine($"        chiave varchar({varcharSize}) COLLATE DATABASE_DEFAULT NULL ")
        stb.AppendLine("    )")
        stb.AppendLine(" END ")

        AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, stb.ToString(), nomeRoutine)

        stb.Length = 0

        If Not IsNothing(chiaviList) AndAlso chiaviList.Any Then
            Dim chunks = ChunkBy(Of String)(chiaviList, chunkSize)
            For Each chunk In chunks
                stb.AppendLine("INSERT INTO #TempChiave (chiave) VALUES ")
                For Each p As String In chunk
                    stb.AppendLine(String.Format("('{0}'),", p))
                Next
                Dim strSqlInsert As String = stb.ToString
                strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                stb.Clear()
                AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, strSqlInsert, nomeRoutine)
            Next
        End If
    End Sub

    Public Shared Sub EliminaTabellaTemp_FiltroChiaveStringa(nomeRoutine As String,
                                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim AgronicaCoreDataProvider As New AgronicaCoreDataProvider.DataProvider
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempChiave') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempChiave ")
        stb.AppendLine(" END ")

        AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, stb.ToString(), nomeRoutine)
    End Sub

#End Region


#Region "Filtro Centro"
    Public Shared Sub CreaTabellaTemp_FiltroCentro(chiaviList As List(Of (String, Integer)),
                                                     nomeRoutine As String,
                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
                                                     
        If chiaviList Is Nothing OrElse Not chiaviList.Any Then
            Throw New Exception("chiaviList obbligatorio per le letture massive!")
        End If


        Dim AgronicaCoreDataProvider As New AgronicaCoreDataProvider.DataProvider

        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempCentro') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempCentro ( ")
        stb.AppendLine("        Piva varchar(50)COLLATE DATABASE_DEFAULT NULL ")
        stb.AppendLine("      , Sa_Cod INT NULL")
        stb.AppendLine("    )")
        stb.AppendLine(" END ")

        AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, stb.ToString(), nomeRoutine)

        stb.Length = 0

        If Not IsNothing(chiaviList) AndAlso chiaviList.Any Then
            Dim chunks = ChunkBy(Of (String, Integer))(chiaviList, chunkSize)
            For Each chunk In chunks
                stb.AppendLine("INSERT INTO #TempCentro (Piva, Sa_Cod) VALUES ")
                For Each p As (String, Integer) In chunk
                    stb.AppendLine(String.Format("('{0}', {1}),", p.Item1, p.Item2))
                Next
                Dim strSqlInsert As String = stb.ToString
                strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                stb.Clear()
                AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, strSqlInsert, nomeRoutine)
            Next
        End If
    End Sub

    Public Shared Sub EliminaTabellaTemp_FiltroCentro(nomeRoutine As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim AgronicaCoreDataProvider As New AgronicaCoreDataProvider.DataProvider
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempCentro') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempCentro ")
        stb.AppendLine(" END ")

        AgronicaCoreDataProvider.EseguiQuery_Scrittura(objParametri, stb.ToString(), nomeRoutine)
    End Sub
#End Region


End Class
