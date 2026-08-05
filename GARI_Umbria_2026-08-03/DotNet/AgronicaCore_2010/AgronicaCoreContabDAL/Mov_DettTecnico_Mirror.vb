Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Mov_DettTecnico_Mirror_W
    Inherits AgronicaCoreDataProvider.DataProvider





    '============================================================================
    'Public Function Scrivi(
    '                        ByVal Piva As String,
    '                        ByVal Sa_Cod As Int32,
    '                        ByVal Id_Agenda As Int32,
    '                        ByVal Id_Mov As Int32,
    '                        ByVal Id_Mov_Det As Int32,
    '                        ByVal Id_Reg_Dettaglio As Int32,
    '                        ByVal Progressivo_Mirror As Int32,
    '                        ByVal Av_Cod As Int32,
    '                        ByVal Av_Gru As Int32,
    '                        ByVal Sigla_AV As String,
    '                        ByVal Data_Ril As Date,
    '                        ByVal Qta_Ril As Decimal,
    '                        ByVal Dose As Decimal,
    '                        ByVal Ditta_Cod As Int32,
    '                        ByVal Dett_Cod As Int32,
    '                        ByVal Id_Insetto As Int32,
    '                        ByVal FF_Classe As Int32,
    '                        ByVal Mg As Decimal,
    '                        ByVal N As Decimal,
    '                        ByVal K As Decimal,
    '                        ByVal P As Decimal,
    '                        ByVal Parziale As Int16,
    '                        ByVal Nitrati As Int16,
    '                        ByVal Freatimetro As Decimal,
    '                        ByVal Piezo1 As Decimal,
    '                        ByVal Piezo2 As Decimal,
    '                        ByVal Piezo3 As Decimal,
    '                        ByVal Piezo4 As Decimal,
    '                        ByVal Trap_Num As Int32,
    '                        ByVal Inn1_Data As Date,
    '                        ByVal Inn2_Data As Date,
    '                        ByVal Inn3_Data As Date,
    '                        ByVal Inn4_Data As Date,
    '                        ByVal Lotto As String,
    '                        ByVal Extra_Int As Int32,
    '                        ByVal Extra_Str As String,
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

    '    Dim NomeRoutine As String = "AgronicaCoreContabDAL.Mov_DettTecnico_Mirror_W.Scrivi()"

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

    '        StrSQL.Append(" INSERT INTO Mov_Dettaglio_Tecnico_Mirror ")
    '        StrSQL.Append("         ( ")
    '        StrSQL.Append("          Piva, Sa_Cod, Id_Agenda, Id_Mov, ")
    '        StrSQL.Append("          Id_Mov_Det, Id_Reg_Dettaglio, Progressivo_Mirror, ")

    '        StrSQL.Append("          Av_Cod, Av_Gru, Sigla_AV, Data_Ril, Qta_Ril, Dose, ")
    '        StrSQL.Append("          Ditta_Cod, Dett_Cod, Id_Insetto, FF_Classe, Mg,  ")
    '        StrSQL.Append("          N, K, P, Parziale, Nitrati, Freatimetro, Piezo1,   ")
    '        StrSQL.Append("          Piezo2, Piezo3, Piezo4, Trap_Num, ")
    '        StrSQL.Append("          Inn1_Data, Inn2_Data, Inn3_Data, Inn4_Data, ")
    '        StrSQL.Append("          Lotto, Extra_Int, Extra_Str, Extra_Date, ")

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
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Progressivo_Mirror) & "  ")


    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Av_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Av_Gru) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Sigla_AV) & "'  ")
    '        StrSQL.Append("         , " & IIf(Data_Ril = New Date, "Null", Agro_SQL_SaveDate(Data_Ril)) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Qta_Ril) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Dose) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Ditta_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Dett_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Insetto) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(FF_Classe) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Mg) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(N) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(K) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(P) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Parziale) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Nitrati) & "  ")


    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Freatimetro) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Piezo1) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Piezo2) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Piezo3) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Piezo4) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Trap_Num) & "  ")
    '        StrSQL.Append("         , " & IIf(Inn1_Data = New Date, "Null", Agro_SQL_SaveDate(Inn1_Data)) & "  ")
    '        StrSQL.Append("         , " & IIf(Inn2_Data = New Date, "Null", Agro_SQL_SaveDate(Inn2_Data)) & "  ")
    '        StrSQL.Append("         , " & IIf(Inn3_Data = New Date, "Null", Agro_SQL_SaveDate(Inn3_Data)) & "  ")
    '        StrSQL.Append("         , " & IIf(Inn4_Data = New Date, "Null", Agro_SQL_SaveDate(Inn4_Data)) & "  ")
    '        StrSQL.Append("         , '" & Agro_SQL_SaveText(Lotto) & "'  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Extra_Int) & "  ")
    '        StrSQL.Append("         , '" & Agro_SQL_SaveText(Extra_Str) & "'  ")
    '        StrSQL.Append("         , " & IIf(Extra_Date = New Date, "Null", Agro_SQL_SaveDate(Extra_Date)) & "  ")


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
                            ByVal Id_Reg_Dettaglio As Int32, _
                            ByVal Progressivo_Mirror As Int32, _
                            ByVal Av_Cod As Int32, _
                            ByVal Av_Gru As Int32, _
                            ByVal Sigla_AV As String, _
                            ByVal Data_Ril As Date, _
                            ByVal Qta_Ril As Decimal, _
                            ByVal Dose As Decimal, _
                            ByVal Ditta_Cod As Int32, _
                            ByVal Dett_Cod As Int32, _
                            ByVal Id_Insetto As Int32, _
                            ByVal FF_Classe As Int32, _
                            ByVal Mg As Decimal, _
                            ByVal N As Decimal, _
                            ByVal K As Decimal, _
                            ByVal P As Decimal, _
                            ByVal Parziale As Int16, _
                            ByVal Nitrati As Int16, _
                            ByVal Freatimetro As Decimal, _
                            ByVal Piezo1 As Decimal, _
                            ByVal Piezo2 As Decimal, _
                            ByVal Piezo3 As Decimal, _
                            ByVal Piezo4 As Decimal, _
                            ByVal Trap_Num As Int32, _
                            ByVal Inn1_Data As Date, _
                            ByVal Inn2_Data As Date, _
                            ByVal Inn3_Data As Date, _
                            ByVal Inn4_Data As Date, _
                            ByVal Lotto As String, _
                            ByVal Extra_Int As Int32, _
                            ByVal Extra_Str As String, _
                            ByVal Extra_Date As Date, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Mov_DettTecnico_Mirror_W.Scrivi()"

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

            StrSQL.Append(" INSERT INTO Mov_Dettaglio_Tecnico_Mirror ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          Piva, Sa_Cod, Id_Agenda, Id_Mov, ")
            StrSQL.Append("          Id_Mov_Det, Id_Reg_Dettaglio, Progressivo_Mirror, ")

            StrSQL.Append("          Av_Cod, Av_Gru, Sigla_AV, Data_Ril, Qta_Ril, Dose, ")
            StrSQL.Append("          Ditta_Cod, Dett_Cod, Id_Insetto, FF_Classe, Mg,  ")
            StrSQL.Append("          N, K, P, Parziale, Nitrati, Freatimetro, Piezo1,   ")
            StrSQL.Append("          Piezo2, Piezo3, Piezo4, Trap_Num, ")
            StrSQL.Append("          Inn1_Data, Inn2_Data, Inn3_Data, Inn4_Data, ")
            StrSQL.Append("          Lotto, Extra_Int, Extra_Str, Extra_Date, ")

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
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Progressivo_Mirror) & "  ")


            StrSQL.Append("         , " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Av_Gru) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Sigla_AV) & "'  ")
            StrSQL.Append("         , " & IIf(Data_Ril = New Date, "Null", Agro_SQL_SaveDate(Data_Ril)) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Qta_Ril) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Dose) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ditta_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Dett_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Insetto) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(FF_Classe) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Mg) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(N) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(K) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(P) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Parziale) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Nitrati) & "  ")


            StrSQL.Append("         , " & Agro_SQL_SaveNum(Freatimetro) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Piezo1) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Piezo2) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Piezo3) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Piezo4) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Trap_Num) & "  ")
            StrSQL.Append("         , " & IIf(Inn1_Data = New Date, "Null", Agro_SQL_SaveDate(Inn1_Data)) & "  ")
            StrSQL.Append("         , " & IIf(Inn2_Data = New Date, "Null", Agro_SQL_SaveDate(Inn2_Data)) & "  ")
            StrSQL.Append("         , " & IIf(Inn3_Data = New Date, "Null", Agro_SQL_SaveDate(Inn3_Data)) & "  ")
            StrSQL.Append("         , " & IIf(Inn4_Data = New Date, "Null", Agro_SQL_SaveDate(Inn4_Data)) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Lotto) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Extra_Str) & "'  ")
            StrSQL.Append("         , " & IIf(Extra_Date = New Date, "Null", Agro_SQL_SaveDate(Extra_Date)) & "  ")


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
