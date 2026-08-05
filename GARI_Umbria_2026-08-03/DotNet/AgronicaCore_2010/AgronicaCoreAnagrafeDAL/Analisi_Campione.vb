Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Analisi_Campione_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '#########################################################################
    Public Function Scrivi(ByVal Analisi_Campione_Cod As Integer,
                           ByVal Analisi_Campione_Des As String,
                           ByVal Analisi_Campione_Coord_X As Decimal,
                           ByVal Analisi_Campione_Coord_Y As Decimal,
                           ByVal Analisi_Campione_Quantita As Decimal,
                           ByVal Analisi_Campione_UdM As Integer,
                           ByVal Analisi_Campione_Profondita As Decimal,
                           ByVal Analisi_Campione_Profondita_Min As Decimal,
                           ByVal Analisi_Campione_Profondita_Max As Decimal,
                           ByVal Analisi_Campione_Riferimento_1 As String,
                           ByVal Analisi_Campione_Riferimento_2 As String,
                           ByVal Analisi_Campione_Riferimento_3 As String,
                           ByVal Analisi_Campione_Riferimento_4 As String,
                           ByVal Analisi_Campione_Riferimento_5 As String, ByVal Analisi_Campione_Note As String,
                           ByVal Analisi_Campione_Key_Piva As String, ByVal Analisi_Campione_Key_SaCod As Integer,
                           ByVal Analisi_Campione_Key_IDGrafica As String,
                           ByVal DataLock As Integer,
                           ByVal Analisi_Campione_Prov As String,
                           ByVal Analisi_Campione_Com As String,
                           ByVal Analisi_Campione_Sezione As String,
                           ByVal Analisi_Campione_Foglio As Integer,
                           ByVal Analisi_Campione_Numero As Integer,
                           ByVal Analisi_Campione_Subalterno As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Campione_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Analisi_Campioni( ")
            StrSQL.Append("            Analisi_SuperUser,        Analisi_Campione_Cod,            ")
            StrSQL.Append("            Analisi_Campione_Des,             Analisi_Campione_Coord_X,        ")
            StrSQL.Append("            Analisi_Campione_Coord_Y,         Analisi_Campione_Quantita,       ")
            StrSQL.Append("            Analisi_Campione_Profondita, Analisi_Campione_Profondita_Min,  Analisi_Campione_Profondita_Max, Analisi_Campione_Udm, ")
            StrSQL.Append("            Analisi_Campione_Riferimento_1,   Analisi_Campione_Riferimento_2,  ")
            StrSQL.Append("            Analisi_Campione_Riferimento_3,   Analisi_Campione_Riferimento_4,  Analisi_Campione_Riferimento_5, Analisi_Campione_Note, ")
            StrSQL.Append("            Analisi_Campione_Key_Piva,  Analisi_Campione_Key_SaCod,  Analisi_Campione_Key_IDGrafica, ")
            StrSQL.Append("            Analisi_Campione_Prov ,  Analisi_Campione_Com ,  Analisi_Campione_Sezione ,  Analisi_Campione_Foglio,  Analisi_Campione_Numero ,  Analisi_Campione_Subalterno , ")
            StrSQL.Append("            Data_Agg,                         DataLock, ")

            StrSQL.Append("            Inviato, DataInvio, ")
            StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("            Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("            ) ")


            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Campione_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Analisi_Campione_Des) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Campione_Coord_X) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Campione_Coord_Y) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Campione_Quantita) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Campione_Profondita) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Campione_Profondita_Min) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Campione_Profondita_Max) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Campione_UdM) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Analisi_Campione_Riferimento_1) & "' ")

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Analisi_Campione_Riferimento_2) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Analisi_Campione_Riferimento_3) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Analisi_Campione_Riferimento_4) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Analisi_Campione_Riferimento_5) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Analisi_Campione_Note) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Analisi_Campione_Key_Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Campione_Key_SaCod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Analisi_Campione_Key_IDGrafica) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Analisi_Campione_Prov) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Analisi_Campione_Com) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Analisi_Campione_Sezione) & "' ")

            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Campione_Foglio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Campione_Numero) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Analisi_Campione_Subalterno) & "' ")

            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(DataLock) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , NULL")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    '#########################################################################
    Public Function Modifica(ByVal Analisi_Campione_Cod As Integer,
                             ByVal Analisi_Campione_Des As String,
                             ByVal Analisi_Campione_Coord_X As Decimal,
                             ByVal Analisi_Campione_Coord_Y As Decimal,
                             ByVal Analisi_Campione_Quantita As Decimal,
                             ByVal Analisi_Campione_UdM As Integer,
                             ByVal Analisi_Campione_Profondita As Decimal,
                             ByVal Analisi_Campione_Profondita_Min As Decimal,
                             ByVal Analisi_Campione_Profondita_Max As Decimal,
                             ByVal Analisi_Campione_Riferimento_1 As String,
                             ByVal Analisi_Campione_Riferimento_2 As String,
                             ByVal Analisi_Campione_Riferimento_3 As String,
                             ByVal Analisi_Campione_Riferimento_4 As String,
                             ByVal Analisi_Campione_Riferimento_5 As String,
                             ByVal Analisi_Campione_Note As String,
                             ByVal Analisi_Campione_Key_Piva As String,
                             ByVal Analisi_Campione_Key_SaCod As Integer,
                             ByVal Analisi_Campione_Key_IDGrafica As String,
                             ByVal DataLock As Integer,
                             ByVal Analisi_Campione_Prov As String,
                             ByVal Analisi_Campione_Com As String,
                             ByVal Analisi_Campione_Sezione As String,
                             ByVal Analisi_Campione_Foglio As Integer,
                             ByVal Analisi_Campione_Numero As Integer,
                             ByVal Analisi_Campione_Subalterno As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Campione_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE Analisi_Campioni SET ")
            StrSQL.Append("     Analisi_Campione_Des            = '" & Agro_SQL_SaveText(Analisi_Campione_Des) & "'")
            StrSQL.Append("   , Analisi_Campione_Coord_X        = " & Agro_SQL_SaveNum(Analisi_Campione_Coord_X) & " ")
            StrSQL.Append("   , Analisi_Campione_Coord_Y        = " & Agro_SQL_SaveNum(Analisi_Campione_Coord_Y) & " ")
            StrSQL.Append("   , Analisi_Campione_Quantita       = " & Agro_SQL_SaveNum(Analisi_Campione_Quantita) & " ")
            StrSQL.Append("   , Analisi_Campione_Udm            = " & Agro_SQL_SaveNum(Analisi_Campione_UdM) & " ")
            StrSQL.Append("   , Analisi_Campione_Profondita     = " & Agro_SQL_SaveNum(Analisi_Campione_Profondita) & " ")
            StrSQL.Append("   , Analisi_Campione_Profondita_Min = " & Agro_SQL_SaveNum(Analisi_Campione_Profondita_Min) & " ")
            StrSQL.Append("   , Analisi_Campione_Profondita_Max = " & Agro_SQL_SaveNum(Analisi_Campione_Profondita_Max) & " ")
            StrSQL.Append("   , Analisi_Campione_Riferimento_1 = '" & Agro_SQL_SaveText(Analisi_Campione_Riferimento_1) & "'")
            StrSQL.Append("   , Analisi_Campione_Riferimento_2 = '" & Agro_SQL_SaveText(Analisi_Campione_Riferimento_2) & "'")
            StrSQL.Append("   , Analisi_Campione_Riferimento_3 = '" & Agro_SQL_SaveText(Analisi_Campione_Riferimento_3) & "'")
            StrSQL.Append("   , Analisi_Campione_Riferimento_4 = '" & Agro_SQL_SaveText(Analisi_Campione_Riferimento_4) & "'")
            StrSQL.Append("   , Analisi_Campione_Riferimento_5 = '" & Agro_SQL_SaveText(Analisi_Campione_Riferimento_5) & "'")

            StrSQL.Append("   , Analisi_Campione_Note = '" & Agro_SQL_SaveText(Analisi_Campione_Note) & "'")
            StrSQL.Append("   , Analisi_Campione_Key_Piva = '" & Agro_SQL_SaveText(Analisi_Campione_Key_Piva) & "'")
            StrSQL.Append("   , Analisi_Campione_Key_SaCod  =  " & Agro_SQL_SaveNum(Analisi_Campione_Key_SaCod) & " ")
            StrSQL.Append("   , Analisi_Campione_Key_IDGrafica = '" & Agro_SQL_SaveText(Analisi_Campione_Key_IDGrafica) & "' ")
            StrSQL.Append("   , Analisi_Campione_Prov  =  '" & Agro_SQL_SaveText(Analisi_Campione_Prov) & "' ")
            StrSQL.Append("   , Analisi_Campione_Com  =  '" & Agro_SQL_SaveText(Analisi_Campione_Com) & "' ")
            StrSQL.Append("   , Analisi_Campione_Sezione  =  '" & Agro_SQL_SaveText(Analisi_Campione_Sezione) & "' ")
            StrSQL.Append("   , Analisi_Campione_Foglio  =  " & Agro_SQL_SaveNum(Analisi_Campione_Foglio) & " ")
            StrSQL.Append("   , Analisi_Campione_Numero = " & Agro_SQL_SaveNum(Analisi_Campione_Numero) & " ")
            StrSQL.Append("   , Analisi_Campione_Subalterno  =  '" & Agro_SQL_SaveText(Analisi_Campione_Subalterno) & "' ")

            StrSQL.Append("   , Data_Agg =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   , DataLock = " & Agro_SQL_SaveNum(DataLock) & " ")
            StrSQL.Append("   , Inviato           =  0 ")
            StrSQL.Append("   , DataInvio         =  Null ")

            StrSQL.Append("   , Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   , Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   , Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   Analisi_Campione_Cod = " & Analisi_Campione_Cod & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Analisi_Campione_Cod"></param>
    ''' <param name="Analisi_Campione_Coord_X"></param>
    ''' <param name="Analisi_Campione_Coord_Y"></param>
    ''' <param name="Analisi_Campione_Key_Piva"></param>
    ''' <param name="Analisi_Campione_Key_SaCod"></param>
    ''' <param name="Analisi_Campione_Key_IDGrafica"></param>
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
    Public Function ModificaSoloGrafica(ByVal Analisi_Campione_Cod As Integer,
                                        ByVal Analisi_Campione_Coord_X As Decimal,
                                        ByVal Analisi_Campione_Coord_Y As Decimal,
                                        ByVal Analisi_Campione_Key_Piva As String,
                                        ByVal Analisi_Campione_Key_SaCod As Integer,
                                        ByVal Analisi_Campione_Key_IDGrafica As String,
                                        ByVal Validita_Inizio As Date,
                                        ByVal Validita_Fine As Date,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Campione_W.ModificaSoloGrafica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            'Query per la modifica dei dati                 ' #### CLASSE ####
            StrSQL.Append("UPDATE Analisi_Campioni SET ")
            StrSQL.Append("    Analisi_Campione_Coord_X        = " & Agro_SQL_SaveNum(Analisi_Campione_Coord_X) & " ")
            StrSQL.Append("   ,Analisi_Campione_Coord_Y        = " & Agro_SQL_SaveNum(Analisi_Campione_Coord_Y) & " ")

            StrSQL.Append("   ,Analisi_Campione_Key_Piva = '" & Agro_SQL_SaveText(Analisi_Campione_Key_Piva) & "'")
            StrSQL.Append("   ,Analisi_Campione_Key_SaCod  =  " & Agro_SQL_SaveNum(Analisi_Campione_Key_SaCod) & " ")
            StrSQL.Append("   ,Analisi_Campione_Key_IDGrafica = '" & Agro_SQL_SaveText(Analisi_Campione_Key_IDGrafica) & "' ")
            StrSQL.Append("   ,Data_Agg =  " & Agro_SQL_SaveDate(Date.Now) & " ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   Analisi_Campione_Cod = " & Analisi_Campione_Cod & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '#########################################################################
    Public Function Cancella(ByVal Analisi_Campione_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Campione_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.Append(" UPDATE  Analisi_Campioni ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & objParametri.UsernameOperazione & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Inviato >= 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Analisi_Campioni ")
                StrSQL.Append(" WHERE    Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Inviato = 0 ")

            End If

            If Analisi_Campione_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_Campioni.Analisi_Campione_Cod = " & Analisi_Campione_Cod & " ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
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

Public Class Analisi_Campione_Read
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Analisi_Campione_Cod As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Campione_Read.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Analisi_Campioni.* ")
                    StrSQL.Append(" FROM   Analisi_Campioni ")
                    StrSQL.Append(" WHERE  Analisi_Campioni.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Analisi_Campioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    Analisi_Campioni.Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    If Analisi_Campione_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_Campioni.Analisi_Campione_Cod = " & Analisi_Campione_Cod & " ")
                    End If
                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Analisi_Campioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Analisi_Campioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Analisi_Campioni.*, ISNULL(UnitaMisura.UDM_SIM,'') AS Analisi_Campione_Udm_Sim, ISNULL(UnitaMisura.UDM_DES,'') AS Analisi_Campione_Udm_Des ")
                    StrSQL.Append(" FROM   Analisi_Campioni LEFT OUTER JOIN ")
                    StrSQL.Append("        UnitaMisura ON Analisi_Campioni.Analisi_Campione_UdM = UnitaMisura.UDM_COD ")
                    StrSQL.Append(" WHERE  Analisi_Campioni.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Analisi_Campioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    Analisi_Campioni.Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    If Analisi_Campione_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_Campioni.Analisi_Campione_Cod = " & Analisi_Campione_Cod & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Analisi_Campioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Analisi_Campioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
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
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '##############################################################################################
    Public Function LeggidaTestataCod(ByVal Analisi_Testata_Cod As Integer,
                                      ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As DataTable


        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Campione_Read.LeggidaTestataCod()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Analisi_CampionixDettagli.Analisi_Testata_Cod ,Analisi_Campioni.*, ISNULL(UnitaMisura.UDM_SIM,'') AS Analisi_Campione_Udm_Sim, ISNULL(UnitaMisura.UDM_DES,'') AS Analisi_Campione_Udm_Des ")
                    StrSQL.Append(" FROM   Analisi_CampionixDettagli INNER JOIN ")
                    StrSQL.Append(" Analisi_Campioni ON Analisi_CampionixDettagli.Analisi_SuperUser = Analisi_Campioni.Analisi_SuperUser AND ")
                    StrSQL.Append(" Analisi_CampionixDettagli.Analisi_Campione_Cod = Analisi_Campioni.Analisi_Campione_Cod LEFT OUTER JOIN ")
                    StrSQL.Append("        UnitaMisura ON Analisi_Campioni.Analisi_Campione_UdM = UnitaMisura.UDM_COD ")

                    StrSQL.Append(" WHERE  Analisi_Campioni.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Analisi_Campioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    Analisi_Campioni.Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    If Analisi_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_CampionixDettagli.Analisi_Testata_Cod  = " & Analisi_Testata_Cod & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Analisi_Testata.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Analisi_Testata.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
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
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '############################################################


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' da utilizzare con Selezione_JoinCompleta
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy "></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	05/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    '############################################################
    Public Function LeggiXCentroAziendale(ByVal Piva As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable


        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Campione_Read.LeggiXCentroAziendale()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '------------------------------

                    'Query per il prelievo dei dati                 ' #### CLASSE ####

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT     Analisi_Campioni.*")
                    StrSQL.Append(" FROM       Analisi_Campioni INNER JOIN ")
                    StrSQL.Append("            Analisi_CampionixDettagli ON Analisi_Campioni.Analisi_SuperUser = Analisi_CampionixDettagli.Analisi_SuperUser AND ")
                    StrSQL.Append("            Analisi_Campioni.Analisi_Campione_Cod = Analisi_CampionixDettagli.Analisi_Campione_Cod INNER JOIN ")
                    StrSQL.Append("            Analisi_Dettagli ON Analisi_CampionixDettagli.Analisi_SuperUser = Analisi_Dettagli.Analisi_SuperUser AND ")
                    StrSQL.Append("            Analisi_CampionixDettagli.Analisi_Testata_Cod = Analisi_Dettagli.Analisi_Testata_Cod AND ")
                    StrSQL.Append("            Analisi_CampionixDettagli.Analisi_Dettaglio_Cod = Analisi_Dettagli.Analisi_Dettaglio_Cod INNER JOIN ")
                    StrSQL.Append("            Analisi_EntitaxTestata ON Analisi_Dettagli.Analisi_SuperUser = Analisi_EntitaxTestata.Analisi_SuperUser AND ")
                    StrSQL.Append("            Analisi_Dettagli.Analisi_Testata_Cod = Analisi_EntitaxTestata.Analisi_Testata_Cod ")
                    StrSQL.Append(" WHERE     (Analisi_EntitaxTestata.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ") ")
                    StrSQL.Append(" AND       (Analisi_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Piva) & "') ")
                    StrSQL.Append(" AND        Analisi_Campioni.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND        Analisi_Campioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND        Analisi_Campioni.Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
                    StrSQL.Append(" ORDER BY   Analisi_Campioni.Analisi_Campione_Des ASC ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Analisi_Testata.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Analisi_Testata.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
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
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '#################################################################################################################
    Public Sub Recupera_Coordinate_DaCampione(ByVal Analisi_Campione_Cod As Integer,
                                              ByRef coord_x As Decimal,
                                              ByRef coord_y As Decimal,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Campione_Read.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        ' Recupera le coordinate del campione dalla tabella Campioni
        ' in questa tabella ci sono le stesse coordinate della tabella grafica (se il campione è stato graficato)
        ' altrimenti ci sono le coordinate inserite a mano dall'utente

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Analisi_Campione_Coord_X, Analisi_Campione_Coord_Y ")
            StrSQL.Append(" FROM Analisi_Campioni ")
            StrSQL.Append(" WHERE Analisi_Campione_Cod = " & Agro_SQL_SaveNum(Analisi_Campione_Cod))

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count > 0 Then
                coord_x = dt.Rows(0).Item("Analisi_Campione_Coord_X")
                coord_y = dt.Rows(0).Item("Analisi_Campione_Coord_Y")
            Else
                coord_x = 0.0
                coord_y = 0.0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

End Class