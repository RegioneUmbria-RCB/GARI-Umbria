Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Analisi_CampionexDet_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '#########################################################################
    Public Function Scrivi(
                                ByVal Analisi_Testata_Cod As Integer,
                                ByVal Analisi_Dettaglio_Cod As Integer,
                                ByVal Analisi_Campione_Cod As Integer,
                                ByVal DataLock As Integer,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Analisi_CampionixDettagli( ")
            StrSQL.Append("            Analisi_SuperUser,     Analisi_Testata_Cod,         ")
            StrSQL.Append("            Analisi_Dettaglio_Cod,            Analisi_Campione_Cod,        ")
            StrSQL.Append("            Data_Agg,                         DataLock, ")

            StrSQL.Append("            Inviato, DataInvio, ")
            StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("            Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("            ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Dettaglio_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Campione_Cod) & "  ")


            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(DataLock) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , NULL  ")

            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

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


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Analisi_Testata_Cod"></param>
    ''' <param name="Analisi_Dettaglio_Cod"></param>
    ''' <param name="Analisi_Campione_Cod"></param>
    ''' <param name="DataLock"></param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	05/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    '#########################################################################
    Public Function Modifica(ByVal Analisi_Testata_Cod As Integer,
                             ByVal Analisi_Dettaglio_Cod As Integer,
                             ByVal Analisi_Campione_Cod As Integer,
                             ByVal DataLock As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '------------------------------

            'Query per la modifica dei dati                 ' #### CLASSE ####
            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Analisi_CampionixDettagli SET ")
            StrSQL.AppendLine("     Data_Agg =  " & Agro_SQL_SaveDate(Date.Now) & " ")
            StrSQL.AppendLine("   , DataLock =  " & Agro_SQL_SaveNum(DataLock))

            StrSQL.AppendLine("   , Inviato           =  0 ")
            StrSQL.AppendLine("   , DataInvio         =  Null ")
            StrSQL.AppendLine("   , Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("   , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   , Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   , Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.AppendLine(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")


            If Analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine(" AND Analisi_CampionixDettagli.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            End If

            If Analisi_Dettaglio_Cod <> 0 Then
                StrSQL.AppendLine(" AND Analisi_CampionixDettagli.Analisi_Dettaglio_Cod = " & Agro_SQL_SaveNum(Analisi_Dettaglio_Cod) & " ")
            End If

            If Analisi_Campione_Cod <> 0 Then
                StrSQL.AppendLine(" AND Analisi_CampionixDettagli.Analisi_Campione_Cod = " & Agro_SQL_SaveNum(Analisi_Campione_Cod) & " ")
            End If

            '------------------------------


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


    '#########################################################################
    Public Function Modifica2(
                                ByVal Analisi_Testata_Cod As Integer,
                                ByVal Analisi_Dettaglio_Cod As Integer,
                                ByVal Analisi_Campione_Cod As Integer,
                                ByVal DataLock As Integer,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_W.Modifica2()"

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

            StrSQL.Append("UPDATE Analisi_Testata SET ")

            StrSQL.Append("UPDATE Analisi_CampionixDettagli SET ")
            StrSQL.Append("     Analisi_Testata_Cod =  " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")

            StrSQL.Append("   , Data_Agg =  " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.Append("   , DataLock =  " & Agro_SQL_SaveNum(DataLock))
            StrSQL.Append("   , Inviato           =  0 ")
            StrSQL.Append("   , DataInvio         =  Null ")

            StrSQL.Append("   , Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   , Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   , Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If Analisi_Campione_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_CampionixDettagli.Analisi_Campione_Cod = " & Analisi_Campione_Cod & " ")
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



    '#########################################################################
    Public Function Cancella(
                            ByVal Analisi_Campione_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.Append(" UPDATE  Analisi_CampionixDettagli ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & objParametri.UsernameOperazione & "' ")
                StrSQL.Append("         ,Data_Modifica =  " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Data_Agg =  " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append("   AND Inviato >= 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Analisi_CampionixDettagli ")
                StrSQL.Append(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append("   AND Inviato = 0 ")
            End If

            If Analisi_Campione_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_CampionixDettagli.Analisi_Campione_Cod = " & Analisi_Campione_Cod & " ")
            End If


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

'########################################################################
'########################################################################
'########################################################################
'########################################################################
'########################################################################
'########################################################################

Public Class Analisi_CampionexDet_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Analisi_Testata_Cod As Integer,
                          ByVal Analisi_Dettaglio_Cod As Integer,
                          ByVal Analisi_Campione_Cod As Integer,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Analisi_CampionixDettagli.* ")
                    StrSQL.Append(" FROM   Analisi_CampionixDettagli ")
                    StrSQL.Append(" WHERE  Analisi_CampionixDettagli.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Analisi_CampionixDettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    Analisi_CampionixDettagli.Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")


                    If Analisi_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_CampionixDettagli.Analisi_Testata_Cod = " & Analisi_Testata_Cod & " ")
                    End If

                    If Analisi_Dettaglio_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_CampionixDettagli.Analisi_Dettaglio_Cod = " & Analisi_Dettaglio_Cod & " ")
                    End If

                    If Analisi_Campione_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_CampionixDettagli.Analisi_Campione_Cod = " & Analisi_Campione_Cod & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Analisi_CampionixDettagli.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Analisi_CampionixDettagli.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '

            End Select

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


    '##############################################################################################
    ''' <summary>
    ''' Da utilizzare con Selezione_JoinCompleta
    ''' </summary>
    ''' <param name="Analisi_Testata_Cod">Per non essere considerato -99999</param>
    ''' <param name="Analisi_Dettaglio_Cod">Per non essere considerato -99999</param>
    ''' <param name="Analisi_Campione_Cod">Per non essere considerato -99999</param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Leggi2(ByVal Analisi_Testata_Cod As Integer,
                                ByVal Analisi_Dettaglio_Cod As Integer,
                                ByVal Analisi_Campione_Cod As Integer,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_R.Leggi2()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  * ")

                    StrSQL.Append(" FROM    Analisi_CampionixDettagli INNER JOIN ")
                    StrSQL.Append("         Analisi_Campioni ON Analisi_CampionixDettagli.Analisi_SuperUser = Analisi_Campioni.Analisi_SuperUser AND  ")
                    StrSQL.Append("         Analisi_CampionixDettagli.Analisi_Campione_Cod = Analisi_Campioni.Analisi_Campione_Cod ")

                    StrSQL.Append(" WHERE   (Analisi_CampionixDettagli.Analisi_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "')  ")

                    '----- Condizioni

                    If Analisi_Testata_Cod <> -99999 Then
                        StrSQL.Append(" AND (Analisi_CampionixDettagli.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & ") ")
                    End If

                    If Analisi_Dettaglio_Cod <> -99999 Then
                        StrSQL.Append(" AND (Analisi_CampionixDettagli.Analisi_Dettaglio_Cod = " & Agro_SQL_SaveNum(Analisi_Dettaglio_Cod) & ")  ")
                    End If

                    If Analisi_Campione_Cod <> -99999 Then
                        StrSQL.Append(" AND (Analisi_CampionixDettagli.Analisi_Campione_Cod = " & Agro_SQL_SaveNum(Analisi_Campione_Cod) & ") ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Analisi_CampionixDettagli.Inviato >=0 ")
                            StrSQL.Append(" AND   Analisi_Campioni.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Analisi_CampionixDettagli.Inviato =-1 ")
                            StrSQL.Append(" AND   Analisi_Campioni.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

            End Select

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



    Public Function LeggixPrecaricaAlbero(ByVal Piva As String,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_R.LeggixPrecaricaAlbero()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT Analisi_CampionixDettagli.Analisi_Testata_Cod, Analisi_CampionixDettagli.Analisi_Dettaglio_Cod, Analisi_CampionixDettagli.Analisi_Campione_Cod, ")
            StrSQL.Append(" Analisi_Campioni.Analisi_Campione_Des , Analisi_EntitaxTestata.Piva as Piva ")
            StrSQL.Append(" FROM         Analisi_CampionixDettagli INNER JOIN ")
            StrSQL.Append("         Analisi_Campioni ON Analisi_CampionixDettagli.Analisi_SuperUser = Analisi_Campioni.Analisi_SuperUser AND ")
            StrSQL.Append("         Analisi_CampionixDettagli.Analisi_Campione_Cod = Analisi_Campioni.Analisi_Campione_Cod INNER JOIN ")
            StrSQL.Append("         Analisi_EntitaxTestata ON Analisi_CampionixDettagli.Analisi_Testata_Cod = Analisi_EntitaxTestata.Analisi_Testata_Cod ")


            StrSQL.Append(" WHERE   (Analisi_CampionixDettagli.Analisi_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')  ")
            StrSQL.Append(" AND    (Analisi_EntitaxTestata.Analisi_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')  ")

            If Piva <> "" Then
                StrSQL.Append(" AND     (Analisi_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
            End If


            '-------------------------------------------------------------------------- 
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '-------------------------------------------------------------------------- 

            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Analisi_EntitaxTestata.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Analisi_EntitaxTestata.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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