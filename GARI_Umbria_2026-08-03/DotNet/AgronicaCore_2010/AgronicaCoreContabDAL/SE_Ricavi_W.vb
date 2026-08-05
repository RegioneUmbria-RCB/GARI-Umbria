Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class SE_Ricavi_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '============================================================================
    Public Function Scrivi(ByVal Piva_SuperUser As String,
                           ByVal Piva As String,
                           ByVal Veg_Cod As Decimal,
                           ByVal Superficie As Decimal,
                           ByVal Produzione_Totale As Decimal,
                           ByVal Ricavi As Decimal,
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

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.SE_Ricavi_W.Scrivi()"

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
            StrSQL.Append(" INSERT INTO SE_Ricavi ")
            StrSQL.Append("             (Piva_Superuser,     Piva,                  Veg_Cod, ")
            StrSQL.Append("              Superficie,         Produzione_Totale,     Ricavi, ")
            StrSQL.Append("              Inviato,            DataInvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva_SuperUser) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Superficie) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Produzione_Totale) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ricavi) & "  ")
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
                             ByVal Veg_Cod As Decimal,
                             ByVal Superficie As Decimal,
                             ByVal Produzione_Totale As Decimal,
                             ByVal Ricavi As Decimal,
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

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.SE_Ricavi_W.Modifica()"

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

            If Veg_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Veg_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE SE_Ricavi SET ")
            StrSQL.Append("    Superficie   =  " & Agro_SQL_SaveNum(Superficie) & "   ")
            StrSQL.Append("   ,Produzione_Totale =  " & Agro_SQL_SaveNum(Produzione_Totale) & "   ")
            StrSQL.Append("   ,Ricavi =  " & Agro_SQL_SaveNum(Ricavi) & "   ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(FinestraTemp_Fine))

            StrSQL.Append(" WHERE Piva_SuperUser      = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'  ")
            StrSQL.Append(" AND   Piva                = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append(" AND   Veg_Cod             =  " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
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
    Public Function Modifica_Campo(ByVal Piva_SuperUser As String,
                                   ByVal Piva As String,
                                   ByVal Veg_Cod As Decimal,
                                   ByVal Campo As String,
                                   ByVal Valore As Object,
                                   ByVal Tipo As String,
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

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.SE_Ricavi_W.Modifica_Campo()"

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

            If Veg_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Veg_Cod obbligatorio)")
            End If

            If Tipo <> "Numero" And Tipo <> "Data" And Tipo <> "Testo" Then
                Throw New Exception("Parametro non corretto nella query (Tipo obbligatorio)")
            End If

            Dim strValore As String

            Select Case Tipo
                Case "Numero"
                    strValore = " " & Agro_SQL_SaveNum(Valore.ToString) & " "
                Case "Data"
                    strValore = " " & Agro_SQL_SaveDate(CDate(Valore)) & " "
                Case "Testo"
                    strValore = " '" & Agro_SQL_SaveText(Valore.ToString) & "' "

                Case Else
                    strValore = " '" & Agro_SQL_SaveText(Valore.ToString) & "' "

            End Select


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE SE_Ricavi SET ")
            StrSQL.Append("    " & Campo & "   =  " & strValore & "   ")

            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")

            StrSQL.Append(" WHERE Piva_SuperUser      = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'  ")
            StrSQL.Append(" AND   Piva                = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append(" AND   Veg_Cod             =  " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
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
                             ByVal Veg_Cod As Int32,
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

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.SE_Ricavi_W.Cancella()"

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

            If Piva_SuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            If FlagCancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE SE_Ricavi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")

                'StrSQL.Append(" WHERE  Inviato > 0 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM SE_Ricavi ")

                'StrSQL.Append(" WHERE  Inviato = 0 ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "' ")
            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StrSQL.Append(" AND Validita_Inizio = " & Agro_SQL_SaveDate(FinestraTemp_Inizio))

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
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
