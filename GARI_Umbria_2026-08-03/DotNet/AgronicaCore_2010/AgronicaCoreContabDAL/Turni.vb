Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Turni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(
                      ByVal Turno_Cod As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Turni_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0 
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
            StrSQL.Append(" FROM  Turni ")
            StrSQL.Append(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.Append(" AND   Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Turno_Cod <> 0 Then
                StrSQL.Append(" AND Turno_Cod = " & Agro_SQL_SaveNum(Turno_Cod) & " ")
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
                StrSQL.Append(" ORDER BY Turno_Des ASC ")
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


Public Class Turni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Turno_Cod As Integer,
                            ByVal Turno_Des As String,
                            ByVal Sigla As String,
                            ByVal Perc_Aumento_Tariffa As Decimal,
                            ByVal Perc_Aumento_Turno As Decimal,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Turni_W.Scrivi()"

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
            StrSQL.AppendLine("INSERT INTO Turni( ")
            StrSQL.AppendLine("                     Piva,     Turno_Cod, ")
            StrSQL.AppendLine("                     Turno_Des,     Sigla, ")
            StrSQL.AppendLine("                     Perc_Aumento_Tariffa,     Perc_Aumento_Turno, ")
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
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Turno_Cod) & " ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Turno_Des) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Sigla) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Perc_Aumento_Tariffa) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Perc_Aumento_Turno) & " ")

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

    Public Function Modifica(ByVal Turno_Cod As Integer,
                                ByVal Turno_Des As String,
                                ByVal Sigla As String,
                                ByVal Perc_Aumento_Tariffa As Decimal,
                                ByVal Perc_Aumento_Turno As Decimal,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Turni_W.Modifica()"

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


            StrSQL.AppendLine("UPDATE Turni SET ")
            StrSQL.AppendLine("       Turno_Des        = '" & Agro_SQL_SaveText(Turno_Des) & "'")
            StrSQL.AppendLine("      ,Sigla    =  '" & Agro_SQL_SaveText(Sigla) & "' ")
            StrSQL.AppendLine("      ,Perc_Aumento_Tariffa        = " & Agro_SQL_SaveNum(Perc_Aumento_Tariffa) & " ")
            StrSQL.AppendLine("      ,Perc_Aumento_Turno        = " & Agro_SQL_SaveNum(Perc_Aumento_Turno) & " ")

            StrSQL.AppendLine("      ,Inviato           =  0 ")
            StrSQL.AppendLine("      ,DataInvio         =  Null ")
            StrSQL.AppendLine("      ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.AppendLine("      ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("      ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("      ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.AppendLine(" WHERE   Piva = '" & Trim(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND   Turno_Cod = " & Agro_SQL_SaveNum(Turno_Cod) & " ")

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


    Public Function Cancella(ByVal Turno_Cod As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Turni_W.Cancella()"

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
            StrSQL.AppendLine(" FROM     Turni ")
            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND Turno_Cod = " & Agro_SQL_SaveNum(Turno_Cod) & "  ")

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
