
Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Agenda_Mirror_R
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    'Public Function MaxProgressivo_from_IdAgenda(
    '                        ByVal Piva As String,
    '                        ByVal Sa_Cod As Int32,
    '                        ByVal Id_Agenda As Int32,
    '                            ByVal FinestraTemp_Inizio As Date,
    '                            ByVal FinestraTemp_Fine As Date,
    '                            ByRef objConnessione As DbConnection,
    '                            ByVal StringaConnessione As String,
    '                            ByVal FlagVisibilita As Int32,
    '                            ByVal DirectoryLOG As String,
    '                            ByVal FileLOG As String,
    '                            ByVal IdentificatoreUtente As String
    '                            ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreContabDAL.Agenda_Mirror_R.MaxProgressivo_from_IdAgenda()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Piva = ""
    '    '   Sa_Cod = 0            
    '    '   Id_Agenda = 0 
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try

    '        '------------------------------------------------------------------

    '        StrSQL.Length = 0

    '        StrSQL.Append(" SELECT MAX(Progressivo_Mirror) AS Progressivo ")
    '        StrSQL.Append(" FROM  Agenda_Mirror ")
    '        StrSQL.Append(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND   Validita_Inizio >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
    '        StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

    '        If Piva <> "" Then
    '            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
    '        End If

    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
    '        End If

    '        If Id_Agenda <> 0 Then
    '            StrSQL.Append(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
    '        End If


    '        Select Case FlagVisibilita
    '            Case 1  'Solo i NON CANCELLATI
    '                StrSQL.Append(" AND   Agenda_Mirror.Inviato >=0 ")
    '            Case 2  'Solo i CANCELLATI
    '                StrSQL.Append(" AND   Agenda_Mirror.Inviato =-1 ")
    '            Case 3  'TUTTI
    '                '
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select

    '        '------------------------------------------------------------------

    '        DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function




    Public Function MaxProgressivo_from_IdAgenda( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Id_Agenda As Int32, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Agenda_Mirror_R.MaxProgressivo_from_IdAgenda()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0            
        '   Id_Agenda = 0 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" SELECT MAX(Progressivo_Mirror) AS Progressivo ")
            StrSQL.Append(" FROM  Agenda_Mirror ")
            StrSQL.Append(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Inizio >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.Append(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Agenda_Mirror.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Agenda_Mirror.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return DT

    End Function


End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################



Public Class Agenda_Mirror_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '============================================================================
    'Public Function Scrivi(
    '                        ByVal Piva As String,
    '                        ByVal Sa_Cod As Int32,
    '                        ByVal Id_Agenda As Int32,
    '                        ByVal Progressivo_Mirror As Int32,
    '                        ByVal Lav_Cod As Int32,
    '                        ByVal Linea_Cod As Int32,
    '                        ByVal Preparazione_Cod As Int32,
    '                        ByVal Id_Trasformazione As Int32,
    '                        ByVal Des_Lib As String,
    '                            ByVal UserName_Creazione As String,
    '                            ByVal FinestraTemp_Inizio As Date,
    '                            ByVal FinestraTemp_Fine As Date,
    '                            ByRef objConnessione As DbConnection,
    '                            ByRef objTransazione As DbTransaction,
    '                            ByVal StringaConnessione As String,
    '                            ByVal DirectoryLOG As String,
    '                            ByVal FileLOG As String,
    '                            ByVal IdentificatoreUtente As String
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreContabDAL.Agenda_Mirror_W.Scrivi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------
    '        StrSQL.Length = 0

    '        StrSQL.Append(" INSERT INTO Agenda_Mirror ")
    '        StrSQL.Append("         ( ")
    '        StrSQL.Append("          Piva,    Sa_Cod,    Id_Agenda, Progressivo_Mirror, ")
    '        StrSQL.Append("          Lav_Cod, Linea_Cod, Preparazione_Cod,  Id_Trasformazione,   Des_Lib, ")

    '        StrSQL.Append("          Inviato,            DataInvio, ")
    '        StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
    '        StrSQL.Append("         ) ")

    '        StrSQL.Append(" VALUES ( ")
    '        StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "'  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Progressivo_Mirror) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Lav_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Linea_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Preparazione_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Trasformazione) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Des_Lib) & "'  ")

    '        StrSQL.Append("         , 0  ")
    '        StrSQL.Append("         , Null  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "  ")

    '        StrSQL.Append(") ")

    '        '---------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function




    Public Function Scrivi( _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal Id_Agenda As Int32, _
                                ByVal Progressivo_Mirror As Int32, _
                                ByVal Lav_Cod As Int32, _
                                ByVal Linea_Cod As Int32, _
                                ByVal Preparazione_Cod As Int32, _
                                ByVal Id_Trasformazione As Int32, _
                                ByVal Des_Lib As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Agenda_Mirror_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO Agenda_Mirror ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          Piva,    Sa_Cod,    Id_Agenda, Progressivo_Mirror, ")
            StrSQL.Append("          Lav_Cod, Linea_Cod, Preparazione_Cod,  Id_Trasformazione,   Des_Lib, ")

            StrSQL.Append("          Inviato,            DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("         ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Progressivo_Mirror) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Lav_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Linea_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Preparazione_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Trasformazione) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Des_Lib) & "'  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")

            StrSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


End Class









