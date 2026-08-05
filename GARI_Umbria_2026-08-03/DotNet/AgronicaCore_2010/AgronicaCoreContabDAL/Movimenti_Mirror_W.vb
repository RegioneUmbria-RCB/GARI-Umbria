Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Movimenti_Mirror_W
    Inherits AgronicaCoreDataProvider.DataProvider




    '============================================================================
    'Public Function Scrivi(
    '                        ByVal Piva As String,
    '                        ByVal Sa_Cod As Int32,
    '                        ByVal Id_Agenda As Int32,
    '                        ByVal Id_Mov As Int32,
    '                        ByVal Progressivo_Mirror As Int32,
    '                        ByVal Cod_RisUm As Int32,
    '                        ByVal Cau_Mov As String,
    '                        ByVal Mov_Desc As String,
    '                        ByVal Data_Movimento As Date,
    '                        ByVal Scadenza As Date,
    '                        ByVal Doc_Numero As Int32,
    '                        ByVal Num_Protocollo As Decimal,
    '                        ByVal Cod_IndirizzoRisUm As Int32,
    '                        ByVal Cod_Destinazione As Int32,
    '                        ByVal Cod_IndirizzoDestinazione As Int32,
    '                        ByVal Mezzo As Int16,
    '                        ByVal Cod_Vettore As Int32,
    '                        ByVal Cod_IndirizzoVettore As Int32,
    '                        ByVal Causale_Trasporto As String,
    '                        ByVal Aspetto As String,
    '                        ByVal Peso As Decimal,
    '                        ByVal Ora As Date,
    '                        ByVal Colli As Int32,
    '                        ByVal Tipo_Sconto As Integer,
    '                        ByVal Extra_Str As String,
    '                        ByVal Extra_Int As Int32,
    '                        ByVal Extra_Date As Date,
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

    '    Dim NomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Mirror_W.Scrivi()"

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

    '        StrSQL.Append(" INSERT INTO Movimenti_Mirror ")
    '        StrSQL.Append("         ( ")
    '        StrSQL.Append("          Piva,         Sa_Cod,        Id_Agenda,   Id_Mov,   Progressivo_Mirror,           ")
    '        StrSQL.Append("          Cod_RisUm,    Cau_Mov,       Mov_Desc,      Data_Movimento,   ")
    '        StrSQL.Append("          Scadenza,     Doc_Numero,    Num_Protocollo,  ")

    '        StrSQL.Append("          Cod_IndirizzoRisUm, Cod_Destinazione,  Cod_IndirizzoDestinazione,  ")
    '        StrSQL.Append("          Mezzo,              Cod_Vettore,       Cod_IndirizzoVettore,       ")
    '        StrSQL.Append("          Causale_Trasporto,  Aspetto,           Peso,                       ")
    '        StrSQL.Append("          Ora,                Colli,             Tipo_Sconto                 ")
    '        StrSQL.Append("          Extra_Str,          Extra_Int,         Extra_Date,                 ")


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
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Progressivo_Mirror) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_RisUm) & "  ")
    '        StrSQL.Append("         , '" & Agro_SQL_SaveText(Cau_Mov) & "'  ")
    '        StrSQL.Append("         , '" & Agro_SQL_SaveText(Mov_Desc) & "'  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Movimento) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Scadenza) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Doc_Numero) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Num_Protocollo) & "  ")

    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_IndirizzoRisUm) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Destinazione) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_IndirizzoDestinazione) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Mezzo) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Vettore) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_IndirizzoVettore) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Causale_Trasporto) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Aspetto) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Peso) & "  ")
    '        StrSQL.Append("         , " & Replace(Agro_SQL_SaveDateTime(Data_Movimento, CStr(Ora)), ".", ":") & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Colli) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Sconto) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Extra_Str) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Extra_Int) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Extra_Date) & "  ")

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
                            ByVal Progressivo_Mirror As Int32, _
                            ByVal Cod_RisUm As Int32, _
                            ByVal Cau_Mov As String, _
                            ByVal Mov_Desc As String, _
                            ByVal Data_Movimento As Date, _
                            ByVal Scadenza As Date, _
                            ByVal Doc_Numero As Int32, _
                            ByVal Num_Protocollo As Decimal, _
                            ByVal Cod_IndirizzoRisUm As Int32, _
                            ByVal Cod_Destinazione As Int32, _
                            ByVal Cod_IndirizzoDestinazione As Int32, _
                            ByVal Mezzo As Int16, _
                            ByVal Cod_Vettore As Int32, _
                            ByVal Cod_IndirizzoVettore As Int32, _
                            ByVal Causale_Trasporto As String, _
                            ByVal Aspetto As String, _
                            ByVal Peso As Decimal, _
                            ByVal Ora As Date, _
                            ByVal Colli As Int32, _
                            ByVal Tipo_Sconto As Integer, _
                            ByVal Extra_Str As String, _
                            ByVal Extra_Int As Int32, _
                            ByVal Extra_Date As Date, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Mirror_W.Scrivi()"

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

            StrSQL.Append(" INSERT INTO Movimenti_Mirror ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          Piva,         Sa_Cod,        Id_Agenda,   Id_Mov,   Progressivo_Mirror,           ")
            StrSQL.Append("          Cod_RisUm,    Cau_Mov,       Mov_Desc,      Data_Movimento,   ")
            StrSQL.Append("          Scadenza,     Doc_Numero,    Num_Protocollo,  ")

            StrSQL.Append("          Cod_IndirizzoRisUm, Cod_Destinazione,  Cod_IndirizzoDestinazione,  ")
            StrSQL.Append("          Mezzo,              Cod_Vettore,       Cod_IndirizzoVettore,       ")
            StrSQL.Append("          Causale_Trasporto,  Aspetto,           Peso,                       ")
            StrSQL.Append("          Ora,                Colli,             Tipo_Sconto                 ")
            StrSQL.Append("          Extra_Str,          Extra_Int,         Extra_Date,                 ")


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
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Progressivo_Mirror) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_RisUm) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Cau_Mov) & "'  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Mov_Desc) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_Movimento) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Scadenza) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Doc_Numero) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Num_Protocollo) & "  ")

            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_IndirizzoRisUm) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Destinazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_IndirizzoDestinazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Mezzo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Vettore) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_IndirizzoVettore) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Causale_Trasporto) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Aspetto) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Peso) & "  ")
            StrSQL.Append("         , " & Replace(Agro_SQL_SaveDateTime(Data_Movimento, CStr(Ora)), ".", ":") & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Colli) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Sconto) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Extra_Str) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Extra_Date) & "  ")

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
