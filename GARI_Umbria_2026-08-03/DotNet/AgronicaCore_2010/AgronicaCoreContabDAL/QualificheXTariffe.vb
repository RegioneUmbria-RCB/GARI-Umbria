Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class QualificheXTariffe_R
    Inherits AgronicaCoreDataProvider.DataProvider



    Public Function Leggi(ByVal Piva As String,
                            ByVal Tipo As String,
                            ByVal Qualifica_Cod As Int32,
                            ByVal Tariffa_Cod As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal xSelezioneVariabile As enumSelezioneVariabile? = enumSelezioneVariabile.Selezione_TabellaCompleta,
                            Optional ByVal IdBudget As Integer = 0
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.QualificheXTariffe_R.Leggi()"

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
            Select Case xSelezioneVariabile
                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM  QualificheXTariffe ")
                    StrSQL.AppendLine(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                    StrSQL.AppendLine(" AND   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.AppendLine(" AND Id_Budget = " & Agro_SQL_SaveNum(IdBudget) & " ")

                    '29/11/2017 Grilli: commentato perché sul lan la tabella non differenzia tra aziende, i dati sono per installazione e non per azienda.
                    'If Piva <> "" Then
                    '    StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    'End If

                    If Tipo <> 0 Then
                        StrSQL.AppendLine(" AND Tipo = " & Agro_SQL_SaveNum(Tipo) & " ")
                    End If

                    If Qualifica_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Qualifica_Cod = " & Agro_SQL_SaveNum(Qualifica_Cod) & " ")
                    End If

                    If Tariffa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Tariffa_Cod = " & Agro_SQL_SaveNum(Tariffa_Cod) & " ")
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

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT QxT.ID , QxT.Piva_SuperUser , QxT.Piva, QxT.Qualifica_Cod, Q.Qualifica_Des, ")
                    StrSQL.AppendLine(" QxT.Tariffa_Cod, T.Tariffa_Des, QxT.Tipo, QxT.Valore, ")
                    StrSQL.AppendLine(" QxT.Validita_Inizio, QxT.Validita_Fine")
                    StrSQL.AppendLine(" FROM  QualificheXTariffe QxT")
                    StrSQL.AppendLine(" LEFT JOIN Qualifiche AS Q")
                    StrSQL.AppendLine(" ON  Q.Qualifica_Cod = QxT.Qualifica_Cod")
                    StrSQL.AppendLine(" LEFT JOIN Tariffe AS T")
                    StrSQL.AppendLine(" ON  T.Tariffa_Cod = QxT.Tariffa_Cod")
                    StrSQL.AppendLine(" WHERE QxT.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.AppendLine(" AND   QxT.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                    StrSQL.AppendLine(" AND   QxT.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.AppendLine(" AND Id_Budget = " & Agro_SQL_SaveNum(IdBudget) & " ")
                    '29/11/2017 Grilli: commentato perché sul lan la tabella non differenzia tra aziende, i dati sono per installazione e non per azienda.
                    'If Piva <> "" Then
                    '    StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    'End If

                    If Tipo <> 0 Then
                        StrSQL.AppendLine(" AND QxT.Tipo = " & Agro_SQL_SaveNum(Tipo) & " ")
                    End If

                    If Qualifica_Cod <> 0 Then
                        StrSQL.AppendLine(" AND QxT.Qualifica_Cod = " & Agro_SQL_SaveNum(Qualifica_Cod) & " ")
                    End If

                    If Tariffa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND QxT.Tariffa_Cod = " & Agro_SQL_SaveNum(Tariffa_Cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   QxT.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   QxT.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

End Class


Public Class QualificheXTariffe_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Qualifica_Cod As Integer,
                           ByVal Tariffa_Cod As Integer,
                           ByVal Tipo As Integer,
                           ByVal Valore As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal IdBudget As Integer = 0
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.QualificheXTariffe_W.Scrivi()"

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
            StrSQL.AppendLine("INSERT INTO QualificheXTariffe( ")
            StrSQL.AppendLine("                                 Piva_SuperUser, Piva, ")
            StrSQL.AppendLine("                                 Qualifica_Cod,     Tariffa_Cod, ")
            StrSQL.AppendLine("                                 Tipo,     Valore, ")
            StrSQL.AppendLine("                                 Inviato,            DataInvio, ")
            StrSQL.AppendLine("                                 Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                                 UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                                 Validita_Inizio,    Validita_Fine, Id_Budget ")
            StrSQL.AppendLine("                              ) ")


            StrSQL.AppendLine("VALUES ( ")
            StrSQL.AppendLine("         '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            'Anagrafiche CDG
            'Il LAN non differenzia tra aziende, i dati sono per installazione e non per azienda.
            'Quindi nella colonna PIVA scrivo la PivaSuperUser.

            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Qualifica_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tariffa_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipo) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Valore) & " ")

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(IdBudget) & " ")
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

    Public Function Modifica(ByVal ID As Integer,
                               ByVal Qualifica_Cod As Integer,
                               ByVal Tariffa_Cod As Integer,
                               ByVal Tipo As Integer,
                               ByVal Valore As Decimal,
                               ByVal Validita_Inizio As Date,
                               ByVal Validita_Fine As Date,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               Optional ByVal Id_Budget As Int32 = 0
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.QualificheXTariffe_W.Modifica()"

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


            StrSQL.AppendLine("UPDATE QualificheXTariffe SET ")
            StrSQL.AppendLine("       Qualifica_Cod        = " & Agro_SQL_SaveNum(Qualifica_Cod) & " ")
            StrSQL.AppendLine("      ,Tariffa_Cod    =  " & Agro_SQL_SaveNum(Tariffa_Cod) & " ")
            StrSQL.AppendLine("      ,Tipo        = " & Agro_SQL_SaveNum(Tipo) & " ")
            StrSQL.AppendLine("      ,Valore        = " & Agro_SQL_SaveNum(Valore) & " ")

            StrSQL.AppendLine("      ,Inviato           =  0 ")
            StrSQL.AppendLine("      ,DataInvio         =  Null ")
            StrSQL.AppendLine("      ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.AppendLine("      ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("      ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("      ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' And Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")
            StrSQL.AppendLine(" AND ID = " & Agro_SQL_SaveNum(ID) & "  ")

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


    Public Function Cancella(ByVal ID As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.QualificheXTariffe_W.Cancella()"

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
            StrSQL.AppendLine(" FROM   QualificheXTariffe ")
            StrSQL.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND ID = " & Agro_SQL_SaveNum(ID) & "  ")

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

