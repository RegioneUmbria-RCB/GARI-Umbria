Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Analisi_Certificato_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '#########################################################################
    Public Function Scrivi(ByVal Analisi_Certificato_Cod As Integer,
                           ByVal Analisi_Certificato_Des As String,
                           ByVal Analisi_Certificato_Data_Inizio As Date,
                           ByVal Analisi_Certificato_Data_Fine As Date,
                           ByVal Analisi_Certificato_Laboratorio As String,
                           ByVal Analisi_Certificato_TipologiaCod As String,
                           ByVal Analisi_Certificato_TipoCampione As String,
                           ByVal Analisi_Certificato_Provenienza As String,
                           ByVal Analisi_Certificato_Verbale As String,
                           ByVal Analisi_Certificato_Richiedente As String,
                           ByVal Analisi_Certificato_PrelevatoDa As String,
                           ByVal Analisi_Certificato_Comune As String,
                           ByVal Analisi_Certificato_Protocollo As String,
                           ByVal Analisi_Certificato_NumRegistro As String,
                           ByVal Analisi_Certificato_Sezione As String,
                           ByVal Analisi_Certificato_DataFirma As Date,
                           ByVal Analisi_Certificato_Responsabile As String,
                           ByVal Analisi_Certificato_Analista As String,
                           ByVal Data_Agg As Date,
                           ByVal DataLock As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Certificato_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Analisi_Certificato( ")
            StrSQL.AppendLine("                    Analisi_SuperUser, Analisi_Certificato_Cod, Analisi_Certificato_Des,  Analisi_Certificato_Data_Inizio ,Analisi_Certificato_Data_Fine , ")
            StrSQL.AppendLine("                    Analisi_Certificato_Laboratorio, Analisi_Certificato_TipologiaCod, Analisi_Certificato_TipoCampione, Analisi_Certificato_Provenienza, Analisi_Certificato_Verbale, ")
            StrSQL.AppendLine("                    Analisi_Certificato_Richiedente, Analisi_Certificato_PrelevatoDa, Analisi_Certificato_Comune, Analisi_Certificato_Protocollo, Analisi_Certificato_NumRegistro,  ")
            StrSQL.AppendLine("                    Analisi_Certificato_Sezione, Analisi_Certificato_DataFirma, Analisi_Certificato_Responsabile, Analisi_Certificato_Analista,   ")
            StrSQL.AppendLine("                    DATA_AGG, Inviato, DataInvio, ")
            StrSQL.AppendLine("                    Data_Creazione, Data_Modifica, ")
            StrSQL.AppendLine("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                    Validita_Inizio, Validita_Fine, DataLock  ")
            StrSQL.AppendLine("                   ) ")
            StrSQL.AppendLine(" VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Certificato_Cod) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Certificato_Des) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Analisi_Certificato_Data_Inizio) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Analisi_Certificato_Data_Fine) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Certificato_Laboratorio) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Certificato_TipologiaCod) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Certificato_TipoCampione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Certificato_Provenienza) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Certificato_Verbale) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Certificato_Richiedente) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Certificato_PrelevatoDa) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Certificato_Comune) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Certificato_Protocollo) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Certificato_NumRegistro) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Certificato_Sezione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Certificato_DataFirma) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Certificato_Responsabile) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Certificato_Analista) & "' ")

            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_Agg) & "  ")
            StrSQL.AppendLine("         , 0 ")
            StrSQL.AppendLine("         , NULL  ")

            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(DataLock) & "  ")
            StrSQL.AppendLine("         )")

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
    Public Function Modifica(ByVal Analisi_Certificato_Cod As Integer,
                             ByVal Analisi_Certificato_Des As String,
                             ByVal Analisi_Certificato_Data_Inizio As Date,
                             ByVal Analisi_Certificato_Data_Fine As Date,
                             ByVal Analisi_Certificato_Laboratorio As String,
                             ByVal Analisi_Certificato_TipologiaCod As String,
                             ByVal Analisi_Certificato_TipoCampione As String,
                             ByVal Analisi_Certificato_Provenienza As String,
                             ByVal Analisi_Certificato_Verbale As String,
                             ByVal Analisi_Certificato_Richiedente As String,
                             ByVal Analisi_Certificato_PrelevatoDa As String,
                             ByVal Analisi_Certificato_Comune As String,
                             ByVal Analisi_Certificato_Protocollo As String,
                             ByVal Analisi_Certificato_NumRegistro As String,
                             ByVal Analisi_Certificato_Sezione As String,
                             ByVal Analisi_Certificato_DataFirma As Date,
                             ByVal Analisi_Certificato_Responsabile As String,
                             ByVal Analisi_Certificato_Analista As String,
                             ByVal DataLock As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Certificato_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Analisi_Certificato SET ")
            StrSQL.AppendLine("     Analisi_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine("   , Analisi_Certificato_Cod  =  " & Agro_SQL_SaveNum(Analisi_Certificato_Cod) & " ")
            StrSQL.AppendLine("   , Analisi_Certificato_Des = '" & Agro_SQL_SaveText(Analisi_Certificato_Des) & "' ")
            StrSQL.AppendLine("   , Analisi_Certificato_Data_Inizio =  " & Agro_SQL_SaveDate(Analisi_Certificato_Data_Inizio))
            StrSQL.AppendLine("   , Analisi_Certificato_Data_fine =  " & Agro_SQL_SaveDate(Analisi_Certificato_Data_Fine))
            StrSQL.AppendLine("   , Analisi_Certificato_Laboratorio  =  '" & Agro_SQL_SaveText(Analisi_Certificato_Laboratorio) & "' ")
            StrSQL.AppendLine("   , Analisi_Certificato_TipologiaCod  =  '" & Agro_SQL_SaveText(Analisi_Certificato_TipologiaCod) & "' ")
            StrSQL.AppendLine("   , Analisi_Certificato_TipoCampione  = '" & Agro_SQL_SaveText(Analisi_Certificato_TipoCampione) & "' ")
            StrSQL.AppendLine("   , Analisi_Certificato_Provenienza  = '" & Agro_SQL_SaveText(Analisi_Certificato_Provenienza) & "' ")
            StrSQL.AppendLine("   , Analisi_Certificato_Verbale  = '" & Agro_SQL_SaveText(Analisi_Certificato_Verbale) & "' ")
            StrSQL.AppendLine("   , Analisi_Certificato_Richiedente  = '" & Agro_SQL_SaveText(Analisi_Certificato_Richiedente) & "' ")
            StrSQL.AppendLine("   , Analisi_Certificato_PrelevatoDa  = '" & Agro_SQL_SaveText(Analisi_Certificato_PrelevatoDa) & "' ")
            StrSQL.AppendLine("   , Analisi_Certificato_Comune  = '" & Agro_SQL_SaveText(Analisi_Certificato_Comune) & "' ")
            StrSQL.AppendLine("   , Analisi_Certificato_Protocollo  = '" & Agro_SQL_SaveText(Analisi_Certificato_Protocollo) & "' ")
            StrSQL.AppendLine("   , Analisi_Certificato_NumRegistro  = '" & Agro_SQL_SaveText(Analisi_Certificato_NumRegistro) & "' ")
            StrSQL.AppendLine("   , Analisi_Certificato_Sezione  = '" & Agro_SQL_SaveText(Analisi_Certificato_Sezione) & "' ")
            StrSQL.AppendLine("   , Analisi_Certificato_DataFirma = '" & Agro_SQL_SaveText(Analisi_Certificato_DataFirma) & "'")
            StrSQL.AppendLine("   , Analisi_Certificato_Responsabile  =  '" & Agro_SQL_SaveText(Analisi_Certificato_Responsabile) & "' ")
            StrSQL.AppendLine("   , Analisi_Certificato_Analista  =  '" & Agro_SQL_SaveText(Analisi_Certificato_Analista) & "' ")

            StrSQL.AppendLine("   , Data_Agg =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("   , DataLock = " & Agro_SQL_SaveNum(DataLock) & " ")
            StrSQL.AppendLine("   , Inviato           =  0 ")
            StrSQL.AppendLine("   , DataInvio         =  Null ")

            StrSQL.AppendLine("   , Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("   , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   , Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   , Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.AppendLine(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND   Analisi_Certificato_Cod = " & Agro_SQL_SaveNum(Analisi_Certificato_Cod) & " ")

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
    Public Function Cancella(ByVal Analisi_Certificato_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Certificato_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.AppendLine(" UPDATE  Analisi_Certificato ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.AppendLine("         ,Data_Agg = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.AppendLine("         ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE Analisi_SuperUser = '" & Trim(Agro_SQL_SaveText(objParametri.PivaSuperUser)) & "' ")
                StrSQL.AppendLine(" AND   Inviato > 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM     Analisi_Certificato ")
                StrSQL.AppendLine(" WHERE Analisi_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.AppendLine(" AND Inviato = 0 ")
            End If

            If Analisi_Certificato_Cod <> 0 Then
                StrSQL.AppendLine(" AND Analisi_Certificato_Cod = " & Agro_SQL_SaveNum(Analisi_Certificato_Cod) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

'############################################################################
'############################################################################
'############################################################################

Public Class Analisi_Certificato_Read
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Analisi_Certificato_Cod As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Certificato_Read.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT  * ")
                    StrSQL.AppendLine(" FROM    Analisi_Certificato ")
                    StrSQL.AppendLine(" WHERE   Analisi_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.AppendLine(" AND     Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Analisi_Certificato_Cod <> 0 Then
                        StrSQL.AppendLine(" AND     Analisi_Certificato_Cod = " & Agro_SQL_SaveNum(Analisi_Certificato_Cod) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Analisi_Certificato.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Analisi_Certificato.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta


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
    Public Function LeggiConFiltroTestata(ByVal Analisi_Certificato_Cod As Integer,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          Optional Analisi_Testata_Cod As Integer = 0
                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Certificato_Read.LeggiConFiltroTestata()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT  Analisi_Certificato.* ")
            StrSQL.AppendLine(" FROM    Analisi_Certificato ")
            StrSQL.AppendLine(" INNER JOIN    Analisi_Testata ON Analisi_Certificato.Analisi_Certificato_Cod = Analisi_Testata.Analisi_Certificato_Cod ")
            StrSQL.AppendLine(" WHERE   Analisi_Certificato.Analisi_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND     Analisi_Certificato.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND     Analisi_Certificato.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Analisi_Certificato_Cod <> 0 Then
                StrSQL.AppendLine(" AND     Analisi_Certificato.Analisi_Certificato_Cod = " & Agro_SQL_SaveNum(Analisi_Certificato_Cod) & " ")
            End If

            If Analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine(" AND     Analisi_Testata.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Analisi_Certificato.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Analisi_Certificato.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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


    Public Function LeggiCertificatoCodDaTestata(ByVal Analisi_Testata_Cod As Integer,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As Integer

        Dim dt As DataTable

        Dim objCertificato As New Analisi_Certificato_Read

        dt = objCertificato.LeggiConFiltroTestata(0, "", "",
                                                  objParametri, Analisi_Testata_Cod)

        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Analisi_Certificato_Cod")
        Else
            Return 0
        End If

    End Function

End Class
