Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Tariffe_R
    Inherits AgronicaCoreDataProvider.DataProvider



    Public Function Leggi(ByVal Tariffa_Cod As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Tariffe_R.Leggi()"

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
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Tariffe ")
            StrSQL.Append(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.Append(" AND   Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


            If Tariffa_Cod <> 0 Then
                StrSQL.Append(" AND Tariffa_Cod = " & Agro_SQL_SaveNum(Tariffa_Cod) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Tariffa_Des ASC ")
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

    Public Function LeggiTariffeXQualifica(ByVal Qualifica_Cod As Int32,
                                           ByVal DataDiValidita As DateTime,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Tariffe_R.LeggiTariffeXQualifica()"

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
            StrSQL.AppendLine(" SELECT T.* ")
            StrSQL.AppendLine(" FROM  Tariffe T")
            StrSQL.AppendLine(" INNER JOIN QualifichexTariffe QXT")
            StrSQL.AppendLine(" ON QXT.Piva=T.Piva and QXT.Tariffa_Cod=T.tariffa_cod And QXT.Id_Budget = 0 ")
            StrSQL.AppendLine(" WHERE QXT.Validita_Inizio <= " & Agro_SQL_SaveDate(DataDiValidita) & "  ")
            StrSQL.AppendLine(" AND   QXT.Validita_Fine >= " & Agro_SQL_SaveDate(DataDiValidita) & "  ")
            StrSQL.AppendLine(" AND   QXT.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Qualifica_Cod <> 0 Then
                StrSQL.AppendLine(" AND QXT.Qualifica_Cod = " & Agro_SQL_SaveNum(Qualifica_Cod) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   QXT.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   QXT.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY T.Tariffa_Des ASC ")
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

Public Class Tariffe_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Tariffa_Cod As Integer,
                            ByVal Tariffa_Des As String,
                            ByVal Sigla As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal Paga_Base As Decimal,
                            ByVal Contributi As Decimal,
                            ByVal Aumento_CCNL As Decimal,
                            ByVal Aumento_CIPL As Decimal,
                            ByVal Terzo_Elemento As Decimal,
                            ByVal TFR As Decimal,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Tariffe_W.Scrivi()"

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
            StrSQL.AppendLine("INSERT INTO Tariffe( ")
            StrSQL.AppendLine("                     Piva,     Tariffa_Cod, ")
            StrSQL.AppendLine("                     Tariffa_Des,     Sigla, ")
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
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Tariffa_Des) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Sigla) & "' ")
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

    Public Function Modifica(ByVal Tariffa_Cod As Integer,
                            ByVal Tariffa_Des As String,
                            ByVal Sigla As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal Paga_Base As Decimal,
                            ByVal Contributi As Decimal,
                            ByVal Aumento_CCNL As Decimal,
                            ByVal Aumento_CIPL As Decimal,
                            ByVal Terzo_Elemento As Decimal,
                            ByVal TFR As Decimal,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Tariffe_W.Modifica()"

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


            StrSQL.AppendLine("UPDATE Tariffe SET ")
            StrSQL.AppendLine("       Tariffa_Des        = '" & Agro_SQL_SaveText(Tariffa_Des) & "'")
            StrSQL.AppendLine("      ,Sigla    =  '" & Agro_SQL_SaveText(Sigla) & "' ")
            StrSQL.AppendLine("      ,Paga_Base    =  " & Agro_SQL_SaveNum(Paga_Base) & " ")
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

            'Anagrafiche CDG
            'Il LAN non differenzia tra aziende, i dati sono per installazione e non per azienda.
            'Quindi nella colonna PIVA scrivo la PivaSuperUser.
            StrSQL.AppendLine(" WHERE Piva = '" & Trim(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND   Tariffa_Cod = " & Agro_SQL_SaveNum(Tariffa_Cod) & " ")

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


    Public Function Cancella(ByVal Tariffa_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Tariffe_W.Cancella()"

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
            StrSQL.AppendLine(" FROM     Tariffe ")
            StrSQL.AppendLine(" WHERE  Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Tariffa_Cod = " & Agro_SQL_SaveNum(Tariffa_Cod) & "  ")

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
