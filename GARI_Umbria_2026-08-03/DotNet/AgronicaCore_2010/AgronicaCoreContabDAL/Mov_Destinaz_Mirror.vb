
Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Mov_Destinaz_Mirror_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '============================================================================
    'Public Function Scrivi(
    '                        ByVal Piva As String,
    '                        ByVal Sa_Cod As Int32,
    '                        ByVal Id_Agenda As Int32,
    '                        ByVal Id_Mov As Int32,
    '                        ByVal Id_Mov_Det As Int32,
    '                        ByVal Appezza As Int32,
    '                        ByVal Id_Destinazione As Int32,
    '                        ByVal Progressivo_Mirror As Int32,
    '                        ByVal Tipo_Destinazione As Int32,
    '                        ByVal Qta As Decimal,
    '                        ByVal Qta2 As Decimal,
    '                        ByVal Tipo_Scorta As Int16,
    '                        ByVal Scorta_Min As Decimal,
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

    '    Dim NomeRoutine As String = "AgronicaCoreContabDAL.Mov_Destinaz_Mirror_W.Scrivi()"

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

    '        StrSQL.Append(" INSERT INTO Mov_Destinazioni_Mirror ")
    '        StrSQL.Append("         ( ")
    '        StrSQL.Append("          Piva, Sa_Cod, Id_Agenda, Id_Mov, Id_Mov_Det, ")
    '        StrSQL.Append("          Appezza, Id_Destinazione, Tipo_Destinazione, Qta,    Qta2, ")
    '        StrSQL.Append("          Tipo_Scorta, Scorta_Min,   ")

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
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Destinazione) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Progressivo_Mirror) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Destinazione) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Qta) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Qta2) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Scorta) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Scorta_Min) & "  ")

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
                            ByVal Id_Mov As Int32, _
                            ByVal Id_Mov_Det As Int32, _
                            ByVal Appezza As Int32, _
                            ByVal Id_Destinazione As Int32, _
                            ByVal Progressivo_Mirror As Int32, _
                            ByVal Tipo_Destinazione As Int32, _
                            ByVal Qta As Decimal, _
                            ByVal Qta2 As Decimal, _
                            ByVal Tipo_Scorta As Int16, _
                            ByVal Scorta_Min As Decimal, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Mov_Destinaz_Mirror_W.Scrivi()"

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

            StrSQL.Append(" INSERT INTO Mov_Destinazioni_Mirror ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          Piva, Sa_Cod, Id_Agenda, Id_Mov, Id_Mov_Det, ")
            StrSQL.Append("          Appezza, Id_Destinazione, Tipo_Destinazione, Qta,    Qta2, ")
            StrSQL.Append("          Tipo_Scorta, Scorta_Min,   ")

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
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Destinazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Progressivo_Mirror) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Destinazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Qta) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Qta2) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Scorta) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Scorta_Min) & "  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

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
