Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Tariffe_Elementi_R
    Inherits AgronicaCoreDataProvider.DataProvider



    Public Function Leggi(ByVal Elemento_Cod As Integer,
                          ByVal Tariffa_Cod As Integer?,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Tariffe_Elementi_R.Leggi()"

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

            '------------------------------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Tariffe_Elementi ")
            StrSQL.AppendLine(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.AppendLine(" AND   Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Not Tariffa_Cod Is Nothing Then
                StrSQL.AppendLine(" AND Tariffa_Cod = " & Agro_SQL_SaveNum(Tariffa_Cod) & " ")
            End If

            If Elemento_Cod <> 0 Then
                StrSQL.AppendLine(" AND Elemento_Cod = " & Agro_SQL_SaveNum(Elemento_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

Public Class Tariffe_Elementi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Elemento_Cod As Integer,
                            ByVal Tariffa_Cod As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal Paga_Base As Decimal,
                            ByVal Contributi As Decimal,
                            ByVal Aumento_CCNL As Decimal,
                            ByVal Aumento_CIPL As Decimal,
                            ByVal Terzo_Elemento As Decimal,
                            ByVal TFR As Decimal,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Tariffe_Elementi_W.Scrivi()"

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
            StrSQL.AppendLine("INSERT INTO Tariffe_Elementi( ")
            StrSQL.AppendLine("                     Piva,     Tariffa_Cod, ")
            StrSQL.AppendLine("                     Elemento_Cod,         ")
            StrSQL.AppendLine("                     Paga_Base,     Contributi, ")
            StrSQL.AppendLine("                     Aumento_CCNL,     Aumento_CIPL, ")
            StrSQL.AppendLine("                     Terzo_Elemento,     TFR, ")
            StrSQL.AppendLine("                     Inviato,            DataInvio, ")
            StrSQL.AppendLine("                     Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                     UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                     Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                     ) ")


            StrSQL.AppendLine("VALUES ( ")

            'Anagrafiche CDG
            'Il LAN non differenzia tra aziende, i dati sono per installazione e non per azienda.
            'Quindi nella colonna PIVA scrivo la PivaSuperUser.

            StrSQL.AppendLine("         '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tariffa_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Elemento_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Paga_Base) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Contributi) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Aumento_CCNL) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Aumento_CIPL) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Terzo_Elemento) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(TFR) & " ")

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(" )")
            '---------------------------------------------

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

    Public Function Modifica(ByVal Elemento_Cod As Integer,
                            ByVal Tariffa_Cod As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal Paga_Base As Decimal,
                            ByVal Contributi As Decimal,
                            ByVal Aumento_CCNL As Decimal,
                            ByVal Aumento_CIPL As Decimal,
                            ByVal Terzo_Elemento As Decimal,
                            ByVal TFR As Decimal,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Tariffe_Elementi_W.Modifica()"

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


            StrSQL.AppendLine("UPDATE Tariffe_Elementi SET ")
            StrSQL.AppendLine("      Paga_Base    =  " & Agro_SQL_SaveNum(Paga_Base) & " ")
            StrSQL.AppendLine("      ,Contributi    =  " & Agro_SQL_SaveNum(Contributi) & " ")
            StrSQL.AppendLine("      ,Aumento_CCNL    =  " & Agro_SQL_SaveNum(Aumento_CCNL) & " ")
            StrSQL.AppendLine("      ,Aumento_CIPL    =  " & Agro_SQL_SaveNum(Aumento_CIPL) & " ")
            StrSQL.AppendLine("      ,Terzo_Elemento    =  " & Agro_SQL_SaveNum(Terzo_Elemento) & " ")
            StrSQL.AppendLine("      ,TFR    =  " & Agro_SQL_SaveNum(TFR) & " ")

            StrSQL.AppendLine("      ,Inviato           =  0 ")
            StrSQL.AppendLine("      ,DataInvio         =  Null ")
            StrSQL.AppendLine("      ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.AppendLine("      ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("      ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("      ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.AppendLine(" WHERE Piva = '" & Trim(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND   Elemento_Cod = " & Agro_SQL_SaveNum(Elemento_Cod) & " ")

            '---------------------------------------------

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


    Public Function Cancella(ByVal Elemento_Cod As Integer,
                             ByVal Tariffa_Cod As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Tariffe_Elementi_W.Cancella()"

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


            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM    Tariffe_Elementi ")
            StrSQL.AppendLine(" WHERE  Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Elemento_Cod <> 0 Then
                StrSQL.AppendLine(" AND Elemento_Cod = " & Agro_SQL_SaveNum(Elemento_Cod) & "  ")
            End If

            If Tariffa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Tariffa_Cod = " & Agro_SQL_SaveNum(Tariffa_Cod) & "  ")
            End If
            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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
