Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Mov_Dettagli_Mirror_W
    Inherits AgronicaCoreDataProvider.DataProvider





    '============================================================================

    'Public Function Scrivi(
    '                        ByVal Piva As String,
    '                        ByVal Sa_Cod As Int32,
    '                        ByVal Id_Agenda As Int32,
    '                        ByVal Id_Mov As Int32,
    '                        ByVal Id_Mov_Det As Int32,
    '                        ByVal Progressivo_Mirror As Int32,
    '                        ByVal Elem_Cod As Int32,
    '                        ByVal Pro_Cod As Int32,
    '                        ByVal Mat_Cod As Int32,
    '                        ByVal Mov_Det_Des As String,
    '                        ByVal Qta As Decimal,
    '                        ByVal Udm_Cod As Int32,
    '                        ByVal Cod_Iva As Int32,
    '                        ByVal Jolly_Int As Int32,
    '                        ByVal Sconto As Decimal,
    '                        ByVal Prezzo_Unitario As Decimal,
    '                        ByVal Prezzo_Unitario_Netto As Decimal,
    '                        ByVal Cod_Conto As Int32,
    '                        ByVal Cal_Cod As Int32,
    '                        ByVal Cod_Progetto As Int32,
    '                        ByVal Fase_Cod As Int32,
    '                        ByVal Extra_Str As String,
    '                        ByVal Extra_Int As Int32,
    '                        ByVal Extra_Date As Date,
    '                        ByVal Ric_Cod As Int32,
    '                        ByVal Anno As Int32,
    '                        ByVal Imponibile As Decimal,
    '                        ByVal Imponibile_Netto As Decimal,
    '                        ByVal Iva As Decimal,
    '                        ByVal Contabilizzato As Integer,
    '                        ByVal Pendente As Integer,
    '                        ByVal Lotto As String,
    '                        ByVal Udm_Cod_Extra As Int32,
    '                        ByVal Qta_Extra As Decimal,
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

    '    Dim NomeRoutine As String = "AgronicaCoreContabDAL.Mov_Dettagli_Mirror_W.Scrivi()"

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

    '        StrSQL.Append(" INSERT INTO Movimenti_Dettagli_Mirror ")
    '        StrSQL.Append("         ( ")
    '        StrSQL.Append("          Piva,      Sa_Cod,      Id_Agenda,       Id_Mov,           Id_Mov_Det,              Progressivo_Mirror,            ")
    '        StrSQL.Append("          Elem_Cod,  Pro_Cod,     Mat_Cod,         Mov_Det_Des,      Qta,                     Udm_Cod,   ")
    '        StrSQL.Append("          Cod_Iva,   Jolly_Int,   Sconto,          Prezzo_Unitario,  Prezzo_Unitario_Netto,   Cod_Conto,       Cod_Progetto, ")
    '        StrSQL.Append("          Fase_Cod,  Cal_Cod,     Extra_Str,       Extra_Int,        Extra_Date,                         ")
    '        StrSQL.Append("          Ric_Cod,   Anno,        Imponibile,      Imponibile_Netto, Iva,                                ")
    '        StrSQL.Append("          Contabilizzato,         Pendente,        Lotto,            Udm_Cod_Extra,           Qta_Extra,  ")

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
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Progressivo_Mirror) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Mov_Det_Des) & "'  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Qta) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Iva) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Jolly_Int) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Sconto) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Prezzo_Unitario) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Prezzo_Unitario_Netto) & "  ")

    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Conto) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Progetto) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Fase_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Cal_Cod) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Extra_Str) & "'  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Extra_Int) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Extra_Date) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Ric_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Anno) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Imponibile) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Imponibile_Netto) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Iva) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Contabilizzato) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Pendente) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Lotto) & "'  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Udm_Cod_Extra) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Qta_Extra) & "  ")

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
                            ByVal Progressivo_Mirror As Int32, _
                            ByVal Elem_Cod As Int32, _
                            ByVal Pro_Cod As Int32, _
                            ByVal Mat_Cod As Int32, _
                            ByVal Mov_Det_Des As String, _
                            ByVal Qta As Decimal, _
                            ByVal Udm_Cod As Int32, _
                            ByVal Cod_Iva As Int32, _
                            ByVal Jolly_Int As Int32, _
                            ByVal Sconto As Decimal, _
                            ByVal Prezzo_Unitario As Decimal, _
                            ByVal Prezzo_Unitario_Netto As Decimal, _
                            ByVal Cod_Conto As Int32, _
                            ByVal Cal_Cod As Int32, _
                            ByVal Cod_Progetto As Int32, _
                            ByVal Fase_Cod As Int32, _
                            ByVal Extra_Str As String, _
                            ByVal Extra_Int As Int32, _
                            ByVal Extra_Date As Date, _
                            ByVal Ric_Cod As Int32, _
                            ByVal Anno As Int32, _
                            ByVal Imponibile As Decimal, _
                            ByVal Imponibile_Netto As Decimal, _
                            ByVal Iva As Decimal, _
                            ByVal Contabilizzato As Integer, _
                            ByVal Pendente As Integer, _
                            ByVal Lotto As String, _
                            ByVal Udm_Cod_Extra As Int32, _
                            ByVal Qta_Extra As Decimal, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Mov_Dettagli_Mirror_W.Scrivi()"

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

            StrSQL.Append(" INSERT INTO Movimenti_Dettagli_Mirror ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          Piva,      Sa_Cod,      Id_Agenda,       Id_Mov,           Id_Mov_Det,              Progressivo_Mirror,            ")
            StrSQL.Append("          Elem_Cod,  Pro_Cod,     Mat_Cod,         Mov_Det_Des,      Qta,                     Udm_Cod,   ")
            StrSQL.Append("          Cod_Iva,   Jolly_Int,   Sconto,          Prezzo_Unitario,  Prezzo_Unitario_Netto,   Cod_Conto,       Cod_Progetto, ")
            StrSQL.Append("          Fase_Cod,  Cal_Cod,     Extra_Str,       Extra_Int,        Extra_Date,                         ")
            StrSQL.Append("          Ric_Cod,   Anno,        Imponibile,      Imponibile_Netto, Iva,                                ")
            StrSQL.Append("          Contabilizzato,         Pendente,        Lotto,            Udm_Cod_Extra,           Qta_Extra,  ")

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
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Progressivo_Mirror) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Mov_Det_Des) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Qta) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Iva) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Jolly_Int) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sconto) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Prezzo_Unitario) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Prezzo_Unitario_Netto) & "  ")

            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Conto) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Progetto) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Fase_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cal_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Extra_Str) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Extra_Date) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ric_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Anno) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Imponibile) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Imponibile_Netto) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Iva) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Contabilizzato) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Pendente) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Lotto) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Udm_Cod_Extra) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Qta_Extra) & "  ")

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
