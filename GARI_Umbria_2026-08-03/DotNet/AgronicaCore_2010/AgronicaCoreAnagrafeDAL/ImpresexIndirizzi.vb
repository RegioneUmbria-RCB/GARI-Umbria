Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class ImpresexIndirizzi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                          ByVal Cod_Indirizzo As Integer,
                          ByVal Tipo_Indirizzo As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Cod_Indirizzo = 0           =>  si leggono tutti gli indirizzi
        '   Tipo_Indirizzo = 0          =>  si leggono tutti i tipi di indirizzo
        '
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT   PIVA, cod_indirizzo, Tipo_Indirizzo  ")
                    strSql.AppendLine(" FROM    ImpresexIndirizzi ")
                    strSql.AppendLine(" WHERE   ImpresexIndirizzi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND     ImpresexIndirizzi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND ImpresexIndirizzi.Piva = '" & Agro_SQL_SaveText(Piva).Trim & "' ")
                    End If

                    If Cod_Indirizzo <> 0 Then
                        strSql.AppendLine(" AND ImpresexIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(Cod_Indirizzo) & " ")
                    End If

                    If Tipo_Indirizzo <> 0 Then
                        strSql.AppendLine(" AND ImpresexIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   ImpresexIndirizzi.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   ImpresexIndirizzi.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT   PIVA, cod_indirizzo, Tipo_Indirizzo, inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine, Validazione, Data_Validazione, UserName_Validazione  ")
                    strSql.AppendLine(" FROM    ImpresexIndirizzi ")
                    strSql.AppendLine(" WHERE   ImpresexIndirizzi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND     ImpresexIndirizzi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND     ImpresexIndirizzi.Piva = '" & Agro_SQL_SaveText(Piva).Trim & "' ")
                    End If

                    If Cod_Indirizzo <> 0 Then
                        strSql.AppendLine(" AND ImpresexIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(Cod_Indirizzo) & " ")
                    End If

                    If Tipo_Indirizzo <> 0 Then
                        strSql.AppendLine(" AND ImpresexIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   ImpresexIndirizzi.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   ImpresexIndirizzi.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '---------------------------------------------
                    strSql.Length = 0
                    'vecchia versione dava problemi con il com_des
                    'strSql.AppendLine(" SELECT  ImpresexIndirizzi.*, Indirizzi.*, ")
                    'strSql.AppendLine("         ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, ")
                    'strSql.AppendLine("         ISNULL(Lista_Province.PROVINCIA, '') AS pro_des   ")


                    strSql.AppendLine(" SELECT   rag_Soc,  ImpresexIndirizzi.PIVA, ImpresexIndirizzi.cod_indirizzo, ImpresexIndirizzi.Tipo_Indirizzo, Indirizzi.ind_des,  ")
                    strSql.AppendLine(" Indirizzi.frz_des, Indirizzi.CAP,Indirizzi.stato, Indirizzi.note, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat,")
                    strSql.AppendLine(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, ")
                    strSql.AppendLine(" ISNULL(Lista_Province.PROVINCIA, '') AS pro_des, ImpresexIndirizzi.Validita_inizio, Indirizzi.Validita_Fine, ")
                    strSql.AppendLine(" ISNULL(Lista_Province.REG, '') AS reg_cod, ")
                    strSql.AppendLine(" Indirizzi.Validazione, Indirizzi.Data_Validazione, Indirizzi.UserName_Validazione, ")
                    strSql.AppendLine(" Indirizzi.Codice_Lingua, Indirizzi.Codice_Alternativo, ")
                    strSql.AppendLine(" ImpresexIndirizzi.Data_Creazione, ImpresexIndirizzi.Data_Modifica, ")
                    strSql.AppendLine(" ImpresexIndirizzi.Username_Creazione, ImpresexIndirizzi.Username_Modifica ")

                    strSql.AppendLine(" FROM    ImpresexIndirizzi  ")
                    strSql.AppendLine(" INNER JOIN Indirizzi ON ImpresexIndirizzi.Cod_Indirizzo = Indirizzi.Cod_Indirizzo  ")
                    strSql.AppendLine(" INNER JOIN Imprese ON ImpresexIndirizzi.Piva = Imprese.Piva ")
                    strSql.AppendLine(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
                    strSql.AppendLine(" LEFT OUTER JOIN Lista_Province ON Indirizzi.pro_cod_istat = Lista_Province.PROV ")
                    strSql.AppendLine(" WHERE   ImpresexIndirizzi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND     ImpresexIndirizzi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND     Indirizzi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND     Indirizzi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    strSql.AppendLine(" AND     ImpresexIndirizzi.Piva = '" & Agro_SQL_SaveText(Piva).Trim & "' ")

                    If Cod_Indirizzo <> 0 Then
                        strSql.AppendLine(" AND ImpresexIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(Cod_Indirizzo) & " ")
                    End If

                    If Tipo_Indirizzo <> 0 Then
                        strSql.AppendLine(" AND ImpresexIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Indirizzi.Inviato >=0 ")
                            strSql.AppendLine(" AND   ImpresexIndirizzi.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Indirizzi.Inviato =-1 ")
                            strSql.AppendLine(" AND   ImpresexIndirizzi.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY ImpresexIndirizzi.Validita_inizio ASC")
                    End If

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
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
    Public Function CodIndirizzo_from_Piva(ByVal Piva As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R.CodIndirizzo_from_Piva()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim Cod_Indirizzo As Integer = 0

        Try

            dt = Leggi(CStr(Piva),
                       0,
                       1,
                       enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                       "", "",
                       objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                Cod_Indirizzo = dt.Rows(0).Item("Cod_Indirizzo")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Cod_Indirizzo

    End Function

    '##############################################################################################
    Public Sub Indirizzo_from_Piva(ByVal Piva As String,
                                   ByRef rag_soc As String,
                                   ByRef ind_des As String,
                                   ByRef frz_des As String,
                                   ByRef CAP As String,
                                   ByRef com_des As String,
                                   ByRef pro_cod As String,
                                   ByRef pro_cod_istat As String,
                                   ByRef com_cod_istat As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   )

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R.Indirizzo_from_Piva()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Try

            dt = Leggi(CStr(Piva),
                       0,
                       1,
                       enumSelezioneVariabile.Selezione_JoinCompleta,
                       "", "",
                       objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                With dt.Rows(0)
                    rag_soc = .Item("rag_soc")
                    ind_des = .Item("ind_des")
                    frz_des = .Item("frz_des")
                    CAP = .Item("CAP")
                    com_des = .Item("com_des")
                    pro_cod = .Item("pro_cod")
                    pro_cod_istat = .Item("pro_cod_istat")
                    com_cod_istat = .Item("com_cod_istat")
                End With
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    '#######################################################################################################
    'era NewCom_Imprese_Leggi
    Public Function Leggi2(ByVal Piva As String,
                           ByVal FlagIndirizzi As Boolean,
                           ByVal Cod_Indirizzo As Integer,
                           ByVal Tipo_Indirizzo As Integer,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R.Leggi2()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT  Imprese.*   ")

            If FlagIndirizzi = True Then
                strSql.AppendLine(" , ImpresexIndirizzi.Tipo_Indirizzo, Indirizzi.Cod_Indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, ")
                strSql.AppendLine(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, ISNULL(Lista_Province.Provincia, '') As pro_des  ")
            End If

            strSql.AppendLine(" FROM    Imprese ")
            strSql.AppendLine(" INNER JOIN UtentiXImprese ON Imprese.PIVA = UtentiXImprese.PIVA ")

            If FlagIndirizzi = True Then
                strSql.AppendLine("  INNER JOIN  ImpresexIndirizzi ON Imprese.Piva = ImpresexIndirizzi.Piva ")
                strSql.AppendLine("  INNER JOIN Indirizzi ON ImpresexIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo ")
                strSql.AppendLine("  LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
                strSql.AppendLine("  LEFT OUTER JOIN Lista_Province ON Indirizzi.pro_cod_istat = Lista_Province.PROV ")
            End If

            strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" + Agro_SQL_SaveText(objParametri.PivaSuperUser) + "' ")

            If Piva <> "" Then
                strSql.AppendLine(" AND     Imprese.Piva = '" & Agro_SQL_SaveText(Piva).Trim & "' ")
            End If

            If FlagIndirizzi = True Then
                If Cod_Indirizzo <> 0 Then
                    strSql.AppendLine(" AND ImpresexIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(Cod_Indirizzo) & " ")
                End If

                If Tipo_Indirizzo <> 0 Then
                    strSql.Append(" AND ImpresexIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " " & vbCrLf)
                End If
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   ImpresexIndirizzi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   ImpresexIndirizzi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Rag_Soc " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '################################################################################
    Public Function IndirizzoStrUnica(ByVal Piva As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R.IndirizzoStrUnica()"

        Dim messaggioErrore As String = ""
        Dim Indirizzo As String = ""

        Dim dt As DataTable
        dt = Leggi2(Piva,
                    True,
                    0, 0,
                    "",
                    "",
                    objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Indirizzo = CStr(dt.Rows(0).Item("ind_des")) & " " &
                            CStr(dt.Rows(0).Item("frz_des")) & " " &
                            CStr(dt.Rows(0).Item("com_des")) & " " &
                            CStr(dt.Rows(0).Item("pro_cod"))
            End If
        End If

        Return Indirizzo

    End Function

    '###################################################################################
    Public Function Regione_From_Piva(ByVal Piva As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R.Regione_From_Piva()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim REG As String = ""

        Try

            dt = Leggi(Piva, 0, 0, enumSelezioneVariabile.Selezione_JoinCompleta, "", "",
                       objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                REG = dt.Rows(0).Item("reg_cod")
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            Return ""
        End Try

        Return REG

    End Function

    Public Function Stato_From_Piva(ByVal Piva As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R.Stato_From_Piva()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim Stato_Cod As String = "IT"

        Try

            dt = Leggi(Piva, 0, 0, enumSelezioneVariabile.Selezione_JoinCompleta, "", "",
                       objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 AndAlso Not IsDBNull(dt.Rows(0).Item("stato")) Then
                Stato_Cod = dt.Rows(0).Item("stato")
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            Return ""
        End Try

        Return Stato_Cod

    End Function


End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§



Public Class ImpresexIndirizzi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Piva As String,
                           ByVal cod_indirizzo As Integer,
                           ByVal Tipo_Indirizzo As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal Validazione As Integer = 0,
                           Optional ByVal Data_Validazione As DateTime = #2/1/1900#,
                           Optional ByVal UserName_Validazione As String = ""
                           ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.ImpresexIndirizzi_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False


        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        If Data_Validazione = #2/1/1900# Then
            Data_Validazione = Now
        End If

        Try
            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine("INSERT INTO ImpresexIndirizzi( ")
            strSql.AppendLine("                    Piva,      ")
            strSql.AppendLine("                    Cod_Indirizzo,    ")
            strSql.AppendLine("                    Tipo_Indirizzo,    ")
            strSql.AppendLine("                    Validazione,   Data_Validazione,  UserName_Validazione, ")

            strSql.AppendLine("                    Inviato, DataInvio, ")
            strSql.AppendLine("                    Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("                    UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("                    Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("                    ) ")

            strSql.AppendLine("VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(cod_indirizzo) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Indirizzo) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Validazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_Validazione) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(UserName_Validazione) & "' ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.AppendLine(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica(ByVal Piva As String,
                             ByVal cod_indirizzo As Integer,
                             ByVal Tipo_Indirizzo As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal Validazione As Integer? = Nothing,
                             Optional ByVal Data_Validazione As DateTime? = Nothing,
                             Optional ByVal UserName_Validazione As String = Nothing
                             ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.ImpresexIndirizzi_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.AppendLine("UPDATE ImpresexIndirizzi SET ")
            strSql.AppendLine("    Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            If Not IsNothing(Validazione) Then
                strSql.AppendLine("   ,Validazione =  " & Agro_SQL_SaveNum(Validazione) & " ")
            End If

            If Not IsNothing(Data_Validazione) Then
                strSql.AppendLine("   ,Data_Validazione =  " & Agro_SQL_SaveDateTime(Data_Validazione) & " ")
            End If

            If Not IsNothing(UserName_Validazione) Then
                strSql.AppendLine("   ,UserName_Validazione =  '" & Agro_SQL_SaveText(UserName_Validazione) & "' ")
            End If

            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            strSql.AppendLine(" AND   Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")
            strSql.AppendLine(" AND   Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Cancella(ByVal Piva As String,
                             ByVal cod_indirizzo As Integer,
                             ByVal Tipo_Indirizzo As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.ImpresexIndirizzi_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE ImpresexIndirizzi ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.AppendLine(" AND Inviato >= 0")

                If cod_indirizzo <> 0 Then

                    strSql.AppendLine(" AND      Cod_Indirizzo = " & cod_indirizzo & " ")

                    If Tipo_Indirizzo <> 0 Then

                        strSql.AppendLine(" AND      Tipo_Indirizzo = " & Tipo_Indirizzo & " ")

                    End If

                End If

            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM     ImpresexIndirizzi ")
                strSql.AppendLine(" WHERE    Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

                If cod_indirizzo <> 0 Then

                    strSql.AppendLine(" AND      Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")

                    If Tipo_Indirizzo <> 0 Then

                        strSql.AppendLine(" AND      Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")

                    End If

                End If

            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function AggiornaValiditaInizio(ByVal PIVA As String,
                                           ByVal cod_indirizzo As Integer,
                                           ByVal Tipo_Indirizzo As Integer,
                                           ByVal Validita_Inizio As Date,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.ImpresexIndirizzi_W.AggiornaValiditaInizio()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.AppendLine("UPDATE ImpresexIndirizzi SET ")
            strSql.AppendLine("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            strSql.AppendLine(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))
            
            If cod_indirizzo <> 0 Then

                strSql.AppendLine(" AND Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")

                If Tipo_Indirizzo <> 0 Then
                    strSql.AppendLine(" AND Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")
                End If

            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function AggiornaValiditaFine(ByVal PIVA As String,
                                         ByVal cod_indirizzo As Integer,
                                         ByVal Tipo_Indirizzo As Integer,
                                         ByVal Validita_Fine As Date,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.ImpresexIndirizzi_W.AggiornaValiditaFine()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.AppendLine("UPDATE ImpresexIndirizzi SET ")
            strSql.AppendLine("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            strSql.AppendLine(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))
            
            If cod_indirizzo <> 0 Then

                strSql.AppendLine(" AND Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")

                If Tipo_Indirizzo <> 0 Then
                    strSql.AppendLine(" AND Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")
                End If

            End If
            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Aggiorna_Indirizzo(ByVal PIVA As String,
                                       ByVal Cod_Indirizzo_Da_Eliminare As Integer,
                                       ByRef Cod_Indirizzo_Eliminato As Integer,
                                       ByRef Num_Indirizzi_Eliminati As Integer,
                                       ByVal PermettiEliminazioneUnSoloIndirizzo As Boolean,
                                       ByRef Cod_Indirizzo_New As Integer,
                                       ByVal Tipo_Indirizzo As Integer,
                                       ByVal Ind_Des As String,
                                       ByVal Frz_Des As String,
                                       ByVal CAP As String,
                                       ByVal Com_Des As String,
                                       ByVal Pro_Cod As String,
                                       ByVal Stato As String,
                                       ByVal Note As String,
                                       ByVal Pro_Cod_Istat As String,
                                       ByVal Com_Cod_Istat As String,
                                       ByVal Validita_Inizio As Date,
                                       ByVal Validita_Fine As Date,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       Optional ByVal Data_creazione As Date = #2/1/1900#,
                                       Optional ByVal Data_modifica As Date = #2/1/1900#,
                                       Optional ByVal username_creazione As String = "",
                                       Optional ByVal username_modifica As String = ""
                                       ) As Boolean


        'per evitare di eliminare tutto
        If PIVA = "" Then
            Throw New Exception("Occorre specificare una Piva")
        End If

        Dim xRisp As Boolean = False

        'INDIRIZZI

        'Persona fisica
        'Codice            Tipo indirizzo
        '2	                Domicilio
        '3	                Residenza
        '4	                Residenza Estiva
        '5	                Luogo di nascita

        'Persona giuridica
        'Codice            Tipo indirizzo
        '1	                Sede operativa
        '101	            Sede legale
        '102	            Sede aziendale
        '103	            Stabilimento

        Dim indirizzi As New AgronicaCoreAnagrafeDAL.Indirizzi_Write
        Dim ImpresexIndirizzi_R As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R


        Dim dt As DataTable = ImpresexIndirizzi_R.Leggi(PIVA, Cod_Indirizzo_Da_Eliminare, Tipo_Indirizzo, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

        If dt.Rows.Count > 0 Then
            'Modifico
            'Modifico l'indirizzo

            If PermettiEliminazioneUnSoloIndirizzo AndAlso dt.Rows.Count > 1 Then
                Throw New Exception("Impossibile eliminare più indirizzi contemporaneamente con PermettiEliminazioneUnSoloIndirizzo=true")
            End If

            Dim i As Integer
            For i = 0 To dt.Rows.Count - 1

                If dt.Rows(i).Item("PIVA") = "" Or dt.Rows(i).Item("PIVA") <> PIVA Then
                    Throw New Exception(" dt.Rows(i).Item(PIVA) =  Or dt.Rows(i).Item(PIVA) <> PIVA ")
                End If

                Cod_Indirizzo_Eliminato = dt.Rows(i).Item("Cod_Indirizzo")
                Cancella(PIVA, Cod_Indirizzo_Eliminato, Tipo_Indirizzo, "", objParametri)
                indirizzi.Cancella(Cod_Indirizzo_Eliminato, "", objParametri)

                Num_Indirizzi_Eliminati = i + 1
            Next

        Else
    
        End If

        Cod_Indirizzo_New = indirizzi.ScriviNuovo(Ind_Des,
                                                  Frz_Des,
                                                  CAP,
                                                  Com_Des,
                                                  Pro_Cod,
                                                  Stato,
                                                  Note,
                                                  Pro_Cod_Istat,
                                                  Com_Cod_Istat,
                                                  Validita_Inizio,
                                                  Validita_Fine,
                                                  objParametri)

        xRisp = Scrivi(PIVA, Cod_Indirizzo_New, Tipo_Indirizzo, AGRODATAINIZIO, AGRODATAFINE, objParametri)
        
        Return xRisp

    End Function

End Class
