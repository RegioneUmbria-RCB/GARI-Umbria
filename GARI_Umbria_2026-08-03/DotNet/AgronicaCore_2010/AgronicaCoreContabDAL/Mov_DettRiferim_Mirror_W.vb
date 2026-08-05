
Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Mov_DettRiferim_Mirror_W
    Inherits AgronicaCoreDataProvider.DataProvider





    '============================================================================
    Public Function Scrivi( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Id_Agenda As Int32, _
                            ByVal Id_Mov As Int32, _
                            ByVal Id_Mov_Det As Int32, _
                            ByVal Lav_Cod As Int32, _
                            ByVal Cau_Mov As String, _
                            ByVal Piva_Rif As String, _
                            ByVal Sa_Cod_Rif As Int32, _
                            ByVal Id_Agenda_Rif As Int32, _
                            ByVal Id_Mov_Rif As Int32, _
                            ByVal Id_Mov_Det_Rif As Int32, _
                            ByVal Progressivo_Mirror As Int32, _
                            ByVal Lav_Cod_Rif As Int32, _
                            ByVal Cau_Mov_Rif As String, _
                            ByVal Qta As Decimal, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Mov_DettRiferim_Mirror_W.Scrivi()"

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

            StrSQL.Append(" INSERT INTO Mov_Dettagli_Riferimenti_Mirror ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          Piva,     Sa_Cod,     Id_Agenda,      Id_Mov,     Id_Mov_Det,     Lav_Cod,     Cau_Mov,  ")
            StrSQL.Append("          Piva_Rif, Sa_Cod_Rif, Id_Agenda_Rif , Id_Mov_Rif, Id_Mov_Det_Rif, Progressivo_Mirror, Lav_Cod_Rif, Cau_Mov_Rif, Qta, ")

            StrSQL.Append("          Inviato,            DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("         ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Mov) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Mov_Det) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Lav_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(UCase(Cau_Mov)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva_Rif) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod_Rif) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Agenda_Rif) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Mov_Rif) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Mov_Det_Rif) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Progressivo_Mirror) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Lav_Cod_Rif) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(UCase(Cau_Mov_Rif)) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Qta) & "  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append(") ")

            '---------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '============================================================================

    '                          VECCHIE FUNZIONI

    '============================================================================


    '============================================================================
    'Public Function Scrivi(
    '                        ByVal Piva As String,
    '                        ByVal Sa_Cod As Int32,
    '                        ByVal Id_Agenda As Int32,
    '                        ByVal Id_Mov As Int32,
    '                        ByVal Id_Mov_Det As Int32,
    '                        ByVal Lav_Cod As Int32,
    '                        ByVal Cau_Mov As String,
    '                        ByVal Piva_Rif As String,
    '                        ByVal Sa_Cod_Rif As Int32,
    '                        ByVal Id_Agenda_Rif As Int32,
    '                        ByVal Id_Mov_Rif As Int32,
    '                        ByVal Id_Mov_Det_Rif As Int32,
    '                        ByVal Progressivo_Mirror As Int32,
    '                        ByVal Lav_Cod_Rif As Int32,
    '                        ByVal Cau_Mov_Rif As String,
    '                        ByVal Qta As Decimal,
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

    '    Dim NomeRoutine As String = "AgronicaCoreContabDAL.Mov_DettRiferim_Mirror_W.Scrivi()"

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

    '        StrSQL.Append(" INSERT INTO Mov_Dettagli_Riferimenti_Mirror ")
    '        StrSQL.Append("         ( ")
    '        StrSQL.Append("          Piva,     Sa_Cod,     Id_Agenda,      Id_Mov,     Id_Mov_Det,     Lav_Cod,     Cau_Mov,  ")
    '        StrSQL.Append("          Piva_Rif, Sa_Cod_Rif, Id_Agenda_Rif , Id_Mov_Rif, Id_Mov_Det_Rif, Progressivo_Mirror, Lav_Cod_Rif, Cau_Mov_Rif, Qta, ")

    '        StrSQL.Append("          Inviato,            DataInvio, ")
    '        StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
    '        StrSQL.Append("         ) ")

    '        StrSQL.Append(" VALUES ( ")
    '        StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "'  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Mov) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Mov_Det) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Lav_Cod) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UCase(Cau_Mov)) & "'  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva_Rif) & "'  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod_Rif) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Agenda_Rif) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Mov_Rif) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Mov_Det_Rif) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Progressivo_Mirror) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Lav_Cod_Rif) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UCase(Cau_Mov_Rif)) & "'  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Qta) & "  ")

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









End Class
