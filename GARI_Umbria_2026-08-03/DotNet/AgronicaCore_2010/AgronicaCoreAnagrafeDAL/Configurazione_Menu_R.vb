
Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Configurazione_Menu_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '============================================================================
    Public Function Leggi_Foglie(ByVal Piva_SuperUser As String,
                                   ByVal Servizio As Int32,
                                   ByVal Nodo_Codice As Int32,
                                   ByVal FinestraTemp_Inizio As Date,
                                   ByVal FinestraTemp_Fine As Date,
                                   ByRef objConnessione As DbConnection,
                                   ByVal StringaConnessione As String,
                                   ByVal FlagVisibilita As Int32,
                                   ByVal DirectoryLOG As String,
                                   ByVal FileLOG As String,
                                   ByVal IdentificatoreUtente As String
                                   ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Configurazione_Menu_R.Leggi_Foglie()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Configurazione_Menu_Foglie")
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

            Select Case FlagVisibilita
                Case 1  'Solo i NON CANCELLATI
                    StrSQL.Append(" AND     Configurazione_Menu_Foglie.Inviato >= 0 ")
                Case 2  'Solo i CANCELLATI
                    StrSQL.Append(" AND     Configurazione_Menu_Foglie.Inviato = -1 ")
                Case 3  'TUTTI
                    '
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If Piva_SuperUser <> "" Then
                StrSQL.Append(" AND Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
            End If

            If Servizio <> 0 Then
                StrSQL.Append(" AND Servizio = " & Agro_SQL_SaveNum(Servizio) & " ")
            End If

            If Nodo_Codice <> 0 Then

                StrSQL.Append(" AND Nodo_Codice = " & Agro_SQL_SaveNum(Nodo_Codice) & " ")

            End If


            StrSQL.Append(" ORDER BY Piva_SuperUser, Servizio, Nodo_Codice, Ordinamento ASC")

            '---------------------------------------------

            DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    '============================================================================
    Public Function Leggi_Nodo(ByVal Piva_SuperUser As String,
                                   ByVal Servizio As Int32,
                                   ByVal Nodo_Codice As Int32,
                                   ByVal FinestraTemp_Inizio As Date,
                                   ByVal FinestraTemp_Fine As Date,
                                   ByRef objConnessione As DbConnection,
                                   ByVal StringaConnessione As String,
                                   ByVal FlagVisibilita As Int32,
                                   ByVal DirectoryLOG As String,
                                   ByVal FileLOG As String,
                                   ByVal IdentificatoreUtente As String
                                   ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Configurazione_Menu_R.Leggi_Nodo()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Configurazione_Menu_Nodo")
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

            Select Case FlagVisibilita
                Case 1  'Solo i NON CANCELLATI
                    StrSQL.Append(" AND     Configurazione_Menu_Nodo.Inviato >= 0 ")
                Case 2  'Solo i CANCELLATI
                    StrSQL.Append(" AND     Configurazione_Menu_Nodo.Inviato = -1 ")
                Case 3  'TUTTI
                    '
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If Piva_SuperUser <> "" Then
                StrSQL.Append(" AND Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
            End If

            If Servizio <> 0 Then
                StrSQL.Append(" AND Servizio = " & Agro_SQL_SaveNum(Servizio) & " ")
            End If


            If Nodo_Codice <> 0 Then

                StrSQL.Append(" AND Nodo_Codice = " & Agro_SQL_SaveNum(Nodo_Codice) & " ")

            End If


            StrSQL.Append(" ORDER BY Piva_SuperUser, Servizio, Nodo_Codice, Ordinamento ASC")

            '---------------------------------------------

            DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

End Class
