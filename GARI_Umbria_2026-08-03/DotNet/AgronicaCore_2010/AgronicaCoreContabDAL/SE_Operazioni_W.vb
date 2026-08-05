Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class SE_Operazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '============================================================================
    Public Function Scrivi(ByVal Piva_SuperUser As String,
                           ByVal Piva As String,
                           ByVal Macrovoce_Cod As Int32,
                           ByVal Operazione_Cod As Int32,
                           ByVal Lav_Cod As Int32,
                           ByVal Veg_Cod As Int32,
                           ByVal Noleggi_Passivi As Int32,
                           ByVal Noleggi_Passivi_Costo As Int32,
                           ByVal Ore_Operazione As Decimal,
                           ByVal Num_Ripetizioni As Int32,
                           ByVal UserName_Creazione As String,
                           ByVal FinestraTemp_Inizio As Date,
                           ByVal FinestraTemp_Fine As Date,
                           ByRef objConnessione As DbConnection,
                           ByRef objTransazione As DbTransaction,
                           ByVal StringaConnessione As String,
                           ByVal DirectoryLOG As String,
                           ByVal FileLOG As String,
                           ByVal IdentificatoreUtente As String
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.SE_Operazioni_W.Scrivi()"

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
            StrSQL.Append(" INSERT INTO SE_Operazioni ")
            StrSQL.Append("             (Piva_Superuser, Piva,    Macrovoce_Cod, Operazione_Cod, ")
            StrSQL.Append("              Lav_Cod, Veg_Cod,    Noleggi_Passivi, Noleggi_Passivi_Costo, Ore_Operazione, Num_Ripetizioni, ")
            StrSQL.Append("              Inviato,            DataInvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva_SuperUser) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Macrovoce_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Operazione_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Lav_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Noleggi_Passivi) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Noleggi_Passivi_Costo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ore_Operazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Num_Ripetizioni) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "  ")


            StrSQL.Append(") ")

            '---------------------------------------------
            xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function




    '============================================================================
    Public Function Modifica(ByVal Piva_SuperUser As String,
                             ByVal Piva As String,
                             ByVal Macrovoce_Cod As Int32,
                             ByVal Veg_Cod As Int32,
                             ByVal Operazione_Cod As Int32,
                             ByVal Lav_Cod As Int32,
                             ByVal Noleggi_Passivi As Int32,
                             ByVal Noleggi_Passivi_Costo As Decimal,
                             ByVal Ore_Operazione As Decimal,
                             ByVal Num_Ripetizioni As Int32,
                             ByVal UserName_Modifica As String,
                             ByVal FinestraTemp_Inizio As Date,
                             ByVal FinestraTemp_Fine As Date,
                             ByRef objConnessione As DbConnection,
                             ByRef objTransazione As DbTransaction,
                             ByVal StringaConnessione As String,
                             ByVal DirectoryLOG As String,
                             ByVal FileLOG As String,
                             ByVal IdentificatoreUtente As String
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.SE_Operazioni_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva_SuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Macrovoce_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE SE_Operazioni SET ")
            StrSQL.Append("    Lav_Cod           =  " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            StrSQL.Append("   ,Veg_Cod           =  " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            StrSQL.Append("   ,Noleggi_Passivi   =  " & Agro_SQL_SaveNum(Noleggi_Passivi) & "   ")
            StrSQL.Append("   ,Noleggi_Passivi_Costo =  " & Agro_SQL_SaveNum(Noleggi_Passivi_Costo) & "   ")
            StrSQL.Append("   ,Ore_Operazione =  " & Agro_SQL_SaveNum(Ore_Operazione) & "   ")
            StrSQL.Append("   ,Num_Ripetizioni =  " & Agro_SQL_SaveNum(Num_Ripetizioni) & "   ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(FinestraTemp_Fine))

            StrSQL.Append(" WHERE Piva_SuperUser      = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'  ")
            StrSQL.Append(" AND   Piva                = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append(" AND   Macrovoce_Cod       =  " & Agro_SQL_SaveNum(Macrovoce_Cod) & "   ")
            StrSQL.Append(" AND   Veg_Cod             =  " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            StrSQL.Append(" AND   Operazione_Cod      =  " & Agro_SQL_SaveNum(Operazione_Cod) & "   ")
            StrSQL.Append(" AND   Lav_Cod             =  " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            StrSQL.Append(" AND   Validita_Inizio     =  " & Agro_SQL_SaveDate(FinestraTemp_Inizio))

            '---------------------------------------------
            xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp


    End Function




    '============================================================================
    Public Function Cancella(ByVal Piva_SuperUser As String,
                             ByVal Piva As String,
                             ByVal Macrovoce_Cod As Int32,
                             ByVal Veg_Cod As Int32,
                             ByVal Operazione_Cod As Int32,
                             ByVal Lav_Cod As Int32,
                             ByVal FinestraTemp_Inizio As Date,
                             ByVal UserName_Modifica As String,
                             ByVal FlagCancellazioneLogica As Int32,
                             ByRef objConnessione As DbConnection,
                             ByRef objTransazione As DbTransaction,
                             ByVal StringaConnessione As String,
                             ByVal DirectoryLOG As String,
                             ByVal FileLOG As String,
                             ByVal IdentificatoreUtente As String
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.SE_Operazioni_W.Cancella()"

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

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            If FlagCancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE SE_Operazioni ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")

                'StrSQL.Append(" WHERE  Inviato > 0 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM SE_Operazioni ")

                'StrSQL.Append(" WHERE  Inviato = 0 ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "' ")
            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StrSQL.Append(" AND Validita_Inizio = " & Agro_SQL_SaveDate(FinestraTemp_Inizio))

            If Macrovoce_Cod <> 0 Then
                StrSQL.Append(" AND Macrovoce_Cod = " & Agro_SQL_SaveNum(Macrovoce_Cod) & "   ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            End If

            If Operazione_Cod <> 0 Then
                StrSQL.Append(" AND Operazione_Cod = " & Agro_SQL_SaveNum(Operazione_Cod) & "   ")
            End If

            If Lav_Cod <> 0 Then
                StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            End If

            '---------------------------------------------
            xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

End Class
