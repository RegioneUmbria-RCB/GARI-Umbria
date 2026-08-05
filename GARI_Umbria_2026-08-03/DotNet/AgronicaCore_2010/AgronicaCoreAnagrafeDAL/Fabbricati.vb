Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text

Public Class Fabbricati_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '################################################################################
    Public Shared Function FabbricatoDes_from_FabbricatoCod(ByVal Piva As String,
                                                            ByVal Sa_Cod As Integer,
                                                            ByVal Fabbricato_Cod As Integer,
                                                            ByRef objParametri As AgronicaCoreParametri,
                                                                Optional aggiungiRagioneSociale As Boolean = False
                                                            ) As String


        Dim dt As DataTable
        Dim fabbricato_des As String = ""

        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        dt = objFabbricati.Leggi_2(Piva,
                                   Sa_Cod,
                                   Fabbricato_Cod,
                                   0,
                                   "",
                                   "",
                                   objParametri)

        If dt.Rows.Count <> 0 Then
            fabbricato_des = dt.Rows(0).Item("Fabbricato_Des")

            If aggiungiRagioneSociale Then
                fabbricato_des += $" ({dt.Rows(0).Item("Rag_Soc")})"
            End If

            Return fabbricato_des
        Else
            Return ""
        End If

    End Function

    '################################################################################
    Public Function Leggi_Magazzini_Organismoreferente(ByVal Piva As String,
                                                       ByVal Sa_Cod As Integer,
                                                       ByVal Fabbricato_Cod As Integer,
                                                       ByVal Tipo_Fabbricato_Cod As Integer,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByVal xOrderBy As String,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.Leggi_Magazzini_Organismoreferente()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim i As Integer

        Try

            StrSQL.Length = 0


            StrSQL.Append(" SELECT  contatti.rag_soc ,  Centri_Aziendali.PIVA, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Fabbricati.Fabbricato_Cod, Fabbricati.Fabbricato_Des,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Indirizzo_Cod, Fabbricati.PROV, Fabbricati.COM, Fabbricati.SEZIONE, Fabbricati.FOGLIO, Fabbricati.NUMERO, Fabbricati.SUBALTERNO,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.MC_Convenzionale, Fabbricati.MC_Conversione, Fabbricati.MC_Biologico, Fabbricati.Regolamento_Cod, Fabbricati.TitoloPossesso,  " & vbCrLf)
            StrSQL.Append("   Fabbricati.Conversione_Inizio, Fabbricati.Conversione_Fine, Fabbricati.Idoneo_Costruzione, Fabbricati.Idoneo_SeparazAmbienti,  " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_SeparazProdotti, Fabbricati.Idoneo_CondIgieniche, Fabbricati.Idoneo_AutorizSanitaria, Fabbricati.Idoneo_HACCP,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_Planimetria, Fabbricati.Idoneo_Layout, Fabbricati.Idoneo_DiagrammiFlusso, Fabbricati.Idoneo_CDX_M004,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_SupMinCoperte, Fabbricati.Idoneo_SupMinScoperte, Fabbricati.Tipo_Fabbricato_Cod, Fabbricati_Tipi.Tipo_Fabbricato_Des, " & vbCrLf)
            StrSQL.Append(" Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, " & vbCrLf)
            StrSQL.Append(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, ISNULL(Lista_Province.REG, '') AS REG  " & vbCrLf)


            StrSQL.Append(" FROM    Fabbricati " & vbCrLf)
            'JOIN TIPO FABBRICATO
            StrSQL.Append(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod " & vbCrLf)
            'JOIN CENTRI AZIENDALI
            StrSQL.Append(" INNER JOIN Centri_Aziendali ON Fabbricati.PIVA = Centri_Aziendali.PIVA AND Fabbricati.SA_COD = Centri_Aziendali.sa_cod" & vbCrLf)

            StrSQL.Append(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = Fabbricati.Indirizzo_Cod " & vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN Lista_Province ON Indirizzi.pro_cod_istat = Lista_Province.PROV " & vbCrLf)

            StrSQL.Append(" INNER JOIN Contatti ON Contatti.cod_contatto = fabbricati.piva" & vbCrLf)
            StrSQL.Append(" INNER JOIN Risorse_Umane ON Contatti.cod_contatto = Risorse_Umane.cod_contatto AND Contatti.piva = Risorse_Umane.piva and cod_rapporto= " & CStr(COD_ORGANISMO_REFERENTE) & vbCrLf)
            StrSQL.Append(" " & vbCrLf)
            StrSQL.Append(" " & vbCrLf)

            StrSQL.Append(" WHERE Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            StrSQL.Append(" AND   Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)

            If Piva <> "" Then
                StrSQL.Append(" AND    (Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     (Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")   " & vbCrLf)
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.Append(" AND (Fabbricati.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ) ")
                    End If
                End If
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND     (Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & ")   " & vbCrLf)
            End If

            If Tipo_Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND     (Fabbricati.Tipo_Fabbricato_Cod = " & Agro_SQL_SaveNum(Tipo_Fabbricato_Cod) & ")   " & vbCrLf)
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY contatti.rag_soc, Fabbricati.Fabbricato_Des ")
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

    '#####################################################################################################
    Public Sub MagazzinoIndirizzo_Leggi(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Fabbricato_Cod As Integer,
                                        ByRef x_Ind_Des As String,
                                        ByRef x_Frz_Des As String,
                                        ByRef x_Cap As String,
                                        ByRef x_Comune As String,
                                        ByRef x_Provincia As String,
                                        ByRef objParametri As AgronicaCoreParametri)

        Dim dtIndirizzi As DataTable

        dtIndirizzi = Leggi_2(Piva, Sa_Cod, Fabbricato_Cod, 0,
                              "", "", objParametri)

        If Not IsNothing(dtIndirizzi) AndAlso dtIndirizzi.Rows.Count <> 0 Then

            x_Ind_Des = dtIndirizzi.Rows(0).Item("Ind_Des")
            x_Frz_Des = dtIndirizzi.Rows(0).Item("Frz_Des")
            x_Cap = dtIndirizzi.Rows(0).Item("Cap")
            x_Comune = dtIndirizzi.Rows(0).Item("Com_Des")
            'x_Provincia = "(" & DT_Indirizzi.Rows(0).Item("Pro_Cod") & ")"
            x_Provincia = dtIndirizzi.Rows(0).Item("Pro_Cod")

        End If

    End Sub

    '#####################################################################################################
    'aggiunti pro_cod_istat, com_cod_istat e regione rispetto alla precedente
    Public Sub MagazzinoIndirizzo_Leggi_2(ByVal Piva As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal Fabbricato_Cod As Integer,
                                          ByRef x_Ind_Des As String,
                                          ByRef x_Frz_Des As String,
                                          ByRef x_Cap As String,
                                          ByRef x_Comune As String,
                                          ByRef x_Provincia As String,
                                          ByRef x_pro_cod_istat As String,
                                          ByRef x_com_cod_istat As String,
                                          ByRef x_Reg As String,
                                          ByRef objParametri As AgronicaCoreParametri)

        Dim dtIndirizzi As DataTable

        dtIndirizzi = Leggi_2(Piva, Sa_Cod, Fabbricato_Cod, 0,
                              "", "", objParametri)

        If Not IsNothing(dtIndirizzi) AndAlso dtIndirizzi.Rows.Count <> 0 Then

            x_Ind_Des = dtIndirizzi.Rows(0).Item("Ind_Des")
            x_Frz_Des = dtIndirizzi.Rows(0).Item("Frz_Des")
            x_Cap = dtIndirizzi.Rows(0).Item("Cap")
            x_Comune = dtIndirizzi.Rows(0).Item("Com_Des")
            'x_Provincia = "(" & DT_Indirizzi.Rows(0).Item("Pro_Cod") & ")"
            x_Provincia = dtIndirizzi.Rows(0).Item("Pro_Cod")
            x_pro_cod_istat = dtIndirizzi.Rows(0).Item("pro_cod_istat")
            x_com_cod_istat = dtIndirizzi.Rows(0).Item("com_cod_istat")
            x_Reg = dtIndirizzi.Rows(0).Item("reg")

        End If

    End Sub

    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Fabbricato_Cod As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal TipoG2G As Integer = 0
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   sa_cod =0
        '   fabbricato_cod =0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim i As Integer

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT   PIVA, SA_COD, Fabbricato_Cod, Fabbricato_Des, TitoloPossesso, Tipo_Fabbricato_Cod  ")
                    StrSQL.AppendLine(" FROM     Fabbricati ")
                    StrSQL.AppendLine(" WHERE    Fabbricati.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND      Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND  Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND  Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    Else
                        'leggo se ci sono filtri sui centri
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim FiltroCentri As String = ""
                        Dim DtCentriVisibili As DataTable
                        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                        If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                            For i = 0 To DtCentriVisibili.Rows.Count - 1
                                FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                            Next
                            If FiltroCentri <> "" Then
                                StrSQL.AppendLine(" AND (Fabbricati.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ) ")
                            End If
                        End If
                    End If

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.AppendLine(" AND  Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.AppendLine(" AND not exists ( ")
                            StrSQL.AppendLine(" select 1 from g2g_Recode_Fabbricati rr where rr.FromPiva = Fabbricati.piva and rr.FromSa_cod = Fabbricati.Sa_cod and rr.From_FabbricatoCod = Fabbricati.Fabbricato_Cod ")
                            StrSQL.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            StrSQL.AppendLine("and exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_Recode_Fabbricati rr ")
                            StrSQL.AppendLine("    where rr.DataInvio < Fabbricati.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.FromPiva = Fabbricati.piva  ")
                            StrSQL.AppendLine("    and rr.FromSa_cod = Fabbricati.Sa_cod  ")
                            StrSQL.AppendLine("    and rr.From_FabbricatoCod = Fabbricati.Fabbricato_Cod ")
                            StrSQL.AppendLine(" ) ")

                    End Select

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   (Fabbricati.inviato >= 0) ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   (Fabbricati.inviato =-1) ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Fabbricati.Piva asc, Fabbricati.Sa_Cod ASC, Fabbricati.Fabbricato_Cod asc")
                    End If

                    '############################################################


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT   Fabbricati.* , Regolamenti.Reg_Des , Regolamenti.Reg_Des_Estesa ,  ")
                    StrSQL.AppendLine("          Fabbricati_Tipi.Tipo_Fabbricato_Cod , ")
                    StrSQL.AppendLine("          Fabbricati_Tipi.Tipo_Fabbricato_Des , ")
                    StrSQL.AppendLine("          Fabbricati_Tipi.Validita_inizio as xValidita_inizio , ")
                    StrSQL.AppendLine("          Fabbricati_Tipi.Validita_fine as xValidita_fine ")
                    StrSQL.AppendLine(" FROM     Fabbricati, Fabbricati_Tipi , Regolamenti ")
                    StrSQL.AppendLine(" WHERE    Fabbricati.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND      Fabbricati.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND      Fabbricati_Tipi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND      Fabbricati_Tipi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND      Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")
                    StrSQL.AppendLine(" AND      Fabbricati.Regolamento_Cod = Regolamenti.Reg_Cod ")

                    If Piva <> "" Then

                        StrSQL.AppendLine(" AND  Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva) & "'")

                        If Sa_Cod <> 0 Then

                            StrSQL.AppendLine(" AND  Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

                            If Fabbricato_Cod <> 0 Then
                                StrSQL.AppendLine(" AND  Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
                            End If

                        Else
                            'leggo se ci sono filtri sui centri
                            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                            Dim FiltroCentri As String = ""
                            Dim DtCentriVisibili As DataTable
                            DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                            If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                                For i = 0 To DtCentriVisibili.Rows.Count - 1
                                    FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                                Next
                                If FiltroCentri <> "" Then
                                    StrSQL.AppendLine(" AND (Fabbricati.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ) ")
                                End If
                            End If

                        End If

                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.AppendLine(" AND not exists ( ")
                            StrSQL.AppendLine(" select 1 from g2g_Recode_Fabbricati rr where rr.FromPiva = Fabbricati.piva and rr.FromSa_cod = Fabbricati.Sa_cod and rr.From_FabbricatoCod = Fabbricati.Fabbricato_Cod ")
                            StrSQL.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            StrSQL.AppendLine("and exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_Recode_Fabbricati rr ")
                            StrSQL.AppendLine("    where rr.DataInvio < Fabbricati.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.FromPiva = Fabbricati.piva  ")
                            StrSQL.AppendLine("    and rr.FromSa_cod = Fabbricati.Sa_cod  ")
                            StrSQL.AppendLine("    and rr.From_FabbricatoCod = Fabbricati.Fabbricato_Cod ")
                            StrSQL.AppendLine(" ) ")

                    End Select

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   (Fabbricati.inviato >= 0) ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   (Fabbricati.inviato =-1) ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Fabbricati.Piva asc, Fabbricati.Sa_Cod ASC, Fabbricati.Fabbricato_Cod asc")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.AppendLine(" SELECT  Fabbricati.PIVA,  ")
                    StrSQL.AppendLine("         Fabbricati.SA_COD,  ")
                    StrSQL.AppendLine("         Fabbricati.Fabbricato_Cod,  ")
                    StrSQL.AppendLine("         Fabbricati.Fabbricato_Des,  ")
                    StrSQL.AppendLine("         Fabbricati.Tipo_Fabbricato_Cod,  ")
                    StrSQL.AppendLine("         Fabbricati_Tipi.Tipo_Fabbricato_Des,  ")
                    StrSQL.AppendLine("         Fabbricati.Validita_Inizio,  ")
                    StrSQL.AppendLine("         Fabbricati.Validita_Fine, ")
                    StrSQL.AppendLine("         Fabbricati.Validazione ")

                    StrSQL.AppendLine(" FROM    Fabbricati INNER JOIN ")
                    StrSQL.AppendLine("         Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")
                    StrSQL.AppendLine(" WHERE   (Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    StrSQL.AppendLine(" AND     (Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")  ")


                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND  Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                        If Sa_Cod <> 0 Then
                            StrSQL.AppendLine(" AND  Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                            If Fabbricato_Cod <> 0 Then
                                StrSQL.AppendLine(" AND  Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
                            End If
                        Else
                            'leggo se ci sono filtri sui centri
                            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                            Dim FiltroCentri As String = ""
                            Dim DtCentriVisibili As DataTable
                            DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                            If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                                For i = 0 To DtCentriVisibili.Rows.Count - 1
                                    FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                                Next
                                If FiltroCentri <> "" Then
                                    StrSQL.AppendLine(" AND (Fabbricati.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ) ")
                                End If
                            End If
                        End If
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.AppendLine(" AND not exists ( ")
                            StrSQL.AppendLine(" select 1 from g2g_Recode_Fabbricati rr where rr.FromPiva = Fabbricati.piva and rr.FromSa_cod = Fabbricati.Sa_cod and rr.From_FabbricatoCod = Fabbricati.Fabbricato_Cod ")
                            StrSQL.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            StrSQL.AppendLine("and exists ( ")
                            StrSQL.AppendLine("    select 1 ")
                            StrSQL.AppendLine("    from g2g_Recode_Fabbricati rr ")
                            StrSQL.AppendLine("    where rr.DataInvio < Fabbricati.Data_Modifica ")
                            StrSQL.AppendLine("    and rr.FromPiva = Fabbricati.piva  ")
                            StrSQL.AppendLine("    and rr.FromSa_cod = Fabbricati.Sa_cod  ")
                            StrSQL.AppendLine("    and rr.From_FabbricatoCod = Fabbricati.Fabbricato_Cod ")
                            StrSQL.AppendLine(" ) ")

                    End Select

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   (Fabbricati.inviato >= 0) ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   (Fabbricati.inviato =-1) ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Fabbricati.Tipo_Fabbricato_Cod ASC")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT  Distinct Fabbricati.PIVA, Fabbricati.SA_COD, Fabbricati.Fabbricato_Cod, Fabbricati.Fabbricato_Des, Fabbricati.TitoloPossesso, Sa_Nome, IsNull(OGenerazioni_Anagrafe_Log.Modulo_Generazione, 0) as Modulo_Generazione, IsNull(OGenerazioni_Anagrafe_Log.Codice_Generazione, 0) as Codice_Generazione,  Fabbricati.Fabbricato_Des + ' (' + Centri_Aziendali.Sa_Nome + ')' AS Fabbricato_Des_Estesa, ")
                    StrSQL.AppendLine("         Convert(varchar(10), Fabbricati.Sa_Cod) + '|' + CONVERT(varchar(10), Fabbricati.Fabbricato_Cod) as Key1, ISNULL(Fabbricati_Codici.id_cod, 0) as Fabbricato_Codice, ISNULL(Fabbricati_Codici.val_cod, '') as Fabbricato_Codice_Val  ")
                    StrSQL.AppendLine(" FROM     Centri_Aziendali, Fabbricati ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN OGenerazioni_Anagrafe_Log On (Fabbricati.Piva = OGenerazioni_Anagrafe_Log.Piva And Fabbricati.Sa_Cod = OGenerazioni_Anagrafe_Log.Sa_Cod And Fabbricati.Fabbricato_Cod = OGenerazioni_Anagrafe_Log.Key1 And Tipo_Generazione = 15 ) ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN Fabbricati_Codici On (Fabbricati.Piva =  Fabbricati_Codici.Piva And Fabbricati.Sa_Cod =  Fabbricati_Codici.Sa_Cod And Fabbricati.Fabbricato_Cod =  Fabbricati_Codici.Fabbricato_Cod) ")
                    StrSQL.AppendLine(" WHERE    Fabbricati.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND      Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND      Fabbricati.Piva = Centri_Aziendali.Piva ")
                    StrSQL.AppendLine(" AND      Fabbricati.Sa_Cod = Centri_Aziendali.Sa_Cod ")


                    If Piva <> "" Then
                        StrSQL.AppendLine(" And  Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 AndAlso Sa_Cod <> SACOD_NOFILTRO Then
                        StrSQL.AppendLine(" AND  Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    Else
                        'leggo se ci sono filtri sui centri
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim FiltroCentri As String = ""
                        Dim DtCentriVisibili As DataTable
                        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                        If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                            For i = 0 To DtCentriVisibili.Rows.Count - 1
                                FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                            Next
                            If FiltroCentri <> "" Then
                                StrSQL.AppendLine(" AND (Fabbricati.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ) ")
                            End If
                        End If
                    End If

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.AppendLine(" AND  Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If



                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   (Fabbricati.inviato >= 0) ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   (Fabbricati.inviato =-1) ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Fabbricati.Piva asc, Fabbricati.Sa_Cod ASC, Fabbricati.Fabbricato_Cod asc")
                    End If


                Case enumSelezioneVariabile.Selezione_LogOmni
                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT  Distinct Fabbricati.PIVA, Fabbricati.SA_COD, Fabbricato_Cod, Fabbricato_Des, Fabbricati.TitoloPossesso, Sa_Nome, IsNull(OGenerazioni_Anagrafe_Log.Modulo_Generazione, 0) as Modulo_Generazione, IsNull(OGenerazioni_Anagrafe_Log.Codice_Generazione, 0) as Codice_Generazione,  Fabbricati.Fabbricato_Des + ' (' + Centri_Aziendali.Sa_Nome + ')' AS Fabbricato_Des_Estesa, ")
                    StrSQL.AppendLine("         Convert(varchar(10), Fabbricati.Sa_Cod) + '|' + CONVERT(varchar(10), Fabbricati.Fabbricato_Cod) as Key1 ")
                    StrSQL.AppendLine(" FROM     Centri_Aziendali, Fabbricati ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN OGenerazioni_Anagrafe_Log On (Fabbricati.Piva = OGenerazioni_Anagrafe_Log.Piva And Fabbricati.Sa_Cod = OGenerazioni_Anagrafe_Log.Sa_Cod And Fabbricati.Fabbricato_Cod = OGenerazioni_Anagrafe_Log.Key1 And Tipo_Generazione = 15 ) ")
                    StrSQL.AppendLine(" WHERE    Fabbricati.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND      Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND      Fabbricati.Piva = Centri_Aziendali.Piva ")
                    StrSQL.AppendLine(" AND      Fabbricati.Sa_Cod = Centri_Aziendali.Sa_Cod ")


                    If Piva <> "" Then
                        StrSQL.AppendLine(" And  Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 AndAlso Sa_Cod <> SACOD_NOFILTRO Then
                        StrSQL.AppendLine(" AND  Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    Else
                        'leggo se ci sono filtri sui centri
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim FiltroCentri As String = ""
                        Dim DtCentriVisibili As DataTable
                        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                        If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                            For i = 0 To DtCentriVisibili.Rows.Count - 1
                                FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                            Next
                            If FiltroCentri <> "" Then
                                StrSQL.AppendLine(" AND (Fabbricati.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ) ")
                            End If
                        End If
                    End If

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.AppendLine(" AND  Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If



                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   (Fabbricati.inviato >= 0) ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   (Fabbricati.inviato =-1) ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Fabbricati.Piva asc, Fabbricati.Sa_Cod ASC, Fabbricati.Fabbricato_Cod asc")
                    End If

                    '############################################################





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

    Public Function Leggi_3(ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Fabbricato_Cod As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreParametri,
                            Optional ByVal FlagIndirizzo As Boolean = False
                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.Leggi_3()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   sa_cod =0
        '   fabbricato_cod =0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT  Fabbricati.* ")

            If FlagIndirizzo Then
                StrSQL.Append(" , Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, ISTAT.LOCALITA, ISTAT.COMUNI_PROV, Indirizzi.stato, Indirizzi.note  ")
            End If

            StrSQL.AppendLine(" FROM    Fabbricati  WITH(NOLOCK)")

            If FlagIndirizzo Then
                StrSQL.Append(" LEFT JOIN Indirizzi ON Fabbricati.Indirizzo_Cod = Indirizzi.cod_indirizzo ")
                StrSQL.Append(" LEFT JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
            End If

            StrSQL.AppendLine(" WHERE   1=1 ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND  Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND  Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.AppendLine(" AND  Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   (Fabbricati.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   (Fabbricati.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Fabbricati.Piva asc, Fabbricati.Sa_Cod ASC, Fabbricati.Fabbricato_Cod asc")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Numero_Fabbricati(ByVal Piva As String,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal TipoG2G As Integer = 0
                                      ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT  count( *) as numero_fabbricati ")
            StrSQL.Append(" FROM     Fabbricati ")
            StrSQL.Append(" WHERE    Fabbricati.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND      Fabbricati.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.Append(" AND  Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Return 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt.Rows(0).Item("numero_fabbricati")

    End Function

    Public Function Leggi_x_anagrafica(ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Fabbricato_Cod As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.Leggi_x_anagrafica()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   sa_cod =0
        '   fabbricato_cod =0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim i As Integer

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT        Fabbricati.PIVA + '_' + CAST(Fabbricati.sa_cod AS nvarchar(15)) + '_' + CAST(Fabbricati.Fabbricato_Cod AS nvarchar(15))  AS chiave,  ")
            StrSQL.Append("      Fabbricato_des As Fabbricato, Tipo_fabbricato_des as Tipo, Fabbricati.Tipo_Fabbricato_Cod,   ")

            StrSQL.Append("      Fabbricati.Data_Creazione,   ")
            StrSQL.Append("      Fabbricati.Data_Modifica,   ")
            StrSQL.Append(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = Fabbricati.Username_Creazione), ' ') AS Utente_Creazione, ")
            StrSQL.Append(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = Fabbricati.Username_Modifica), ' ') AS Utente_Modifica ")

            StrSQL.Append(" FROM     Fabbricati, Fabbricati_Tipi , Regolamenti ")
            StrSQL.Append(" WHERE    Fabbricati.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND      Fabbricati.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND      Fabbricati_Tipi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND      Fabbricati_Tipi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND      Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")
            StrSQL.Append(" AND      Fabbricati.Regolamento_Cod = Regolamenti.Reg_Cod ")

            If Piva <> "" Then

                StrSQL.Append(" AND  Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva) & "'")

                If Sa_Cod <> 0 Then

                    StrSQL.Append(" AND  Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.Append(" AND  Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
                    End If

                Else
                    'leggo se ci sono filtri sui centri
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim FiltroCentri As String = ""
                    Dim DtCentriVisibili As DataTable
                    DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Piva & "'", "", objParametri)
                    If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                        For i = 0 To DtCentriVisibili.Rows.Count - 1
                            FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                        Next
                        If FiltroCentri <> "" Then
                            StrSQL.Append(" AND (Fabbricati.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ) ")
                        End If
                    End If

                End If

            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Fabbricati.Piva asc, Fabbricati.Sa_Cod ASC, Fabbricati.Fabbricato_Cod asc")
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


    Public Function Leggi_x_anagraficaNG(ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Fabbricato_Cod As Integer,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                         ByRef objParametri_Utenti As AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.Leggi_x_anagraficaNG()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   sa_cod =0
        '   fabbricato_cod =0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim i As Integer

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Fabbricati.PIVA + '_' + CAST(Fabbricati.sa_cod AS nvarchar(15)) + '_' + CAST(Fabbricati.Fabbricato_Cod AS nvarchar(15))  AS chiave,   ")
            StrSQL.AppendLine("     Fabbricato_des, Tipo_fabbricato_des as Tipo, Fabbricati.Tipo_Fabbricato_Cod,    ")
            StrSQL.AppendLine("     Fabbricati.Piva, Fabbricati.Sa_Cod, Fabbricato_Cod,    ")
            StrSQL.AppendLine("     Centri_Aziendali.Sa_Nome,    ")
            StrSQL.AppendLine("     Fabbricati.Data_Creazione,    ")
            StrSQL.AppendLine("     Fabbricati.Data_Modifica,    ")
            StrSQL.AppendLine("     Fabbricati.Validita_Inizio,    ")
            StrSQL.AppendLine("     Fabbricati.Validita_Fine,    ")
            StrSQL.AppendLine(" 	uc.username as Username_Creazione, ")
            StrSQL.AppendLine(" 	um.username as Username_Modifica ")
            StrSQL.AppendLine(" FROM     Fabbricati ")
            StrSQL.AppendLine(" JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")
            StrSQL.AppendLine(" JOIN Regolamenti ON Fabbricati.Regolamento_Cod = Regolamenti.Reg_Cod ")
            StrSQL.AppendLine(" JOIN Centri_Aziendali ON Fabbricati.Sa_Cod = Centri_Aziendali.sa_cod  ")
            StrSQL.AppendLine(" 					AND Fabbricati.Piva = Centri_Aziendali.Piva ")
            StrSQL.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Dettagli uc ON Fabbricati.Username_Creazione = uc.codFisc ")
            StrSQL.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Dettagli um ON Fabbricati.Username_Creazione = um.codFisc ")
            StrSQL.AppendLine(" WHERE    Fabbricati.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND      Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND      Fabbricati_Tipi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND      Fabbricati_Tipi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then

                StrSQL.AppendLine(" AND  Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva) & "'")

                If Sa_Cod <> 0 Then

                    StrSQL.AppendLine(" AND  Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.AppendLine(" AND  Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
                    End If

                Else
                    'leggo se ci sono filtri sui centri
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim FiltroCentri As String = ""
                    Dim DtCentriVisibili As DataTable
                    DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Piva & "'", "", objParametri_Server)
                    If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                        For i = 0 To DtCentriVisibili.Rows.Count - 1
                            FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                        Next
                        If FiltroCentri <> "" Then
                            StrSQL.AppendLine(" AND (Fabbricati.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ) ")
                        End If
                    End If

                End If

            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   (Fabbricati.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   (Fabbricati.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                StrSQL.AppendLine(" ORDER BY Fabbricati.Piva asc, Fabbricati.Sa_Cod ASC, Fabbricati.Fabbricato_Cod asc")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function LeggiStalle_x_anagrafica(ByVal Piva As String,
                                             ByVal Sa_Cod As Integer,
                                             ByVal Fabbricato_Cod As Integer,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri,
                                             Optional ByVal JoinCentri As Boolean = False,
                                             Optional ByVal JoinImprese As Boolean = False
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.LeggiStalle_x_anagrafica()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   sa_cod =0
        '   fabbricato_cod =0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim i As Integer

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("SELECT Fabbricati.PIVA + '_' + CAST(Fabbricati.sa_cod AS nvarchar(15)) + '_' + CAST(Fabbricati.Fabbricato_Cod AS nvarchar(15)) AS chiave,")
            StrSQL.AppendLine("Fabbricato_des As Fabbricato, Tipo_fabbricato_des As Tipo, Fabbricati.Tipo_Fabbricato_Cod, ")
            StrSQL.AppendLine("Lista_Specie_Animali.SPE_DES, Lista_IndirizziProd_Animali.IPRO_DES, ")
            StrSQL.AppendLine("Stalla.BDN_Codice_Azienda, Stalla.BDN_Allev_IdFiscale, Stalla.Gen_Cod as xGen_Cod, Stalla.Spe_Cod as xSpe_Cod, Stalla.Ipro_Cod as xIpro_Cod, ")

            StrSQL.Append("Fabbricati.Data_Creazione, ")
            StrSQL.Append("Fabbricati.Data_Modifica, ")

            If JoinImprese Then
                StrSQL.Append("Imprese.Piva, ")
                StrSQL.Append("Imprese.Rag_Soc, ")
            End If
            If JoinCentri Then
                StrSQL.Append("Centri_Aziendali.sa_cod, ")
                StrSQL.Append("Centri_Aziendali.sa_nome, ")
            End If

            StrSQL.Append("ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = Fabbricati.Username_Creazione), Fabbricati.Username_Creazione) AS Utente_Creazione, ")
            StrSQL.Append("ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = Fabbricati.Username_Modifica), Fabbricati.Username_Modifica) AS Utente_Modifica ")

            StrSQL.AppendLine("FROM  Fabbricati ")
            StrSQL.AppendLine(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")
            StrSQL.AppendLine(" INNER JOIN Stalla ON Fabbricati.PIVA = Stalla.PIVA And Fabbricati.SA_COD = Stalla.sa_cod And Fabbricati.Fabbricato_Cod = Stalla.STA_NUM ")
            StrSQL.AppendLine(" INNER JOIN Lista_Specie_Animali ON Stalla.GEN_COD = Lista_Specie_Animali.GEN_COD And Stalla.SPE_COD = Lista_Specie_Animali.SPE_COD ")
            StrSQL.AppendLine(" INNER JOIN Lista_IndirizziProd_Animali ON Stalla.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD And Stalla.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD And Stalla.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD ")
            If JoinImprese Then
                StrSQL.AppendLine(" INNER JOIN Imprese ON Fabbricati.PIVA = Imprese.Piva ")
            End If
            If JoinCentri Then
                StrSQL.AppendLine(" INNER JOIN Centri_Aziendali ON Fabbricati.Piva = Centri_aziendali.Piva AND Fabbricati.sa_Cod = Centri_aziendali.sa_cod ")
            End If
            StrSQL.AppendLine(" LEFT JOIN Regolamenti ON Fabbricati.Regolamento_Cod = Regolamenti.Reg_Cod")

            StrSQL.AppendLine("WHERE Fabbricati.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND Fabbricati.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND Fabbricati_Tipi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND Fabbricati_Tipi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then

                StrSQL.AppendLine(" AND  Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva) & "'")

                If Sa_Cod <> 0 Then

                    StrSQL.AppendLine(" AND  Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

                    If Fabbricato_Cod <> 0 Then
                        StrSQL.AppendLine(" AND  Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
                    End If

                Else
                    'leggo se ci sono filtri sui centri
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim FiltroCentri As String = ""
                    Dim DtCentriVisibili As DataTable
                    DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                    If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                        For i = 0 To DtCentriVisibili.Rows.Count - 1
                            FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                        Next
                        If FiltroCentri <> "" Then
                            StrSQL.AppendLine(" AND (Fabbricati.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ) ")
                        End If
                    End If

                End If

            End If

            StrSQL.AppendLine(" AND Fabbricati.Tipo_Fabbricato_Cod IN (170, 171, 172, 173, 174, 175, 176, 177, 178, 179) ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   (Fabbricati.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   (Fabbricati.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Fabbricati.Piva asc, Fabbricati.Sa_Cod ASC, Fabbricati.Fabbricato_Cod asc")
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

    Public Function LeggiStalleVisibili(ByVal Filtro_visibilita_utente As Boolean,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        Optional ByVal JoinCentri As Boolean = False,
                                        Optional ByVal JoinImprese As Boolean = False
                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.LeggiStalleVisibili()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("SELECT Fabbricati.PIVA + '_' + CAST(Fabbricati.sa_cod AS nvarchar(15)) + '_' + CAST(Fabbricati.Fabbricato_Cod AS nvarchar(15)) AS chiave,")
            StrSQL.AppendLine("Fabbricato_des As Fabbricato, Tipo_fabbricato_des As Tipo, Fabbricati.Tipo_Fabbricato_Cod, ")
            StrSQL.AppendLine("Lista_Specie_Animali.SPE_DES, Lista_IndirizziProd_Animali.IPRO_DES, ")
            StrSQL.AppendLine("Stalla.BDN_Codice_Azienda, Stalla.BDN_Allev_IdFiscale, Stalla.Gen_Cod as xGen_Cod, Stalla.Spe_Cod as xSpe_Cod, Stalla.Ipro_Cod as xIpro_Cod, Stalla.STA_DES, ")

            StrSQL.Append("Fabbricati.Data_Creazione, ")
            StrSQL.Append("Fabbricati.Data_Modifica, ")

            If JoinImprese Then
                StrSQL.Append("Imprese.Piva, ")
                StrSQL.Append("Imprese.Rag_Soc, ")
            End If
            If JoinCentri Then
                StrSQL.Append("Centri_Aziendali.sa_cod, ")
                StrSQL.Append("Centri_Aziendali.sa_nome, ")
            End If

            StrSQL.Append("ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = Fabbricati.Username_Creazione), Fabbricati.Username_Creazione) AS Utente_Creazione, ")
            StrSQL.Append("ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = Fabbricati.Username_Modifica), Fabbricati.Username_Modifica) AS Utente_Modifica ")

            StrSQL.AppendLine("FROM  Fabbricati ")
            StrSQL.AppendLine(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")
            StrSQL.AppendLine(" INNER JOIN Stalla ON Fabbricati.PIVA = Stalla.PIVA And Fabbricati.SA_COD = Stalla.sa_cod And Fabbricati.Fabbricato_Cod = Stalla.STA_NUM ")
            StrSQL.AppendLine(" INNER JOIN Lista_Specie_Animali ON Stalla.GEN_COD = Lista_Specie_Animali.GEN_COD And Stalla.SPE_COD = Lista_Specie_Animali.SPE_COD ")
            StrSQL.AppendLine(" INNER JOIN Lista_IndirizziProd_Animali ON Stalla.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD And Stalla.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD And Stalla.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD ")
            If JoinImprese Then
                StrSQL.AppendLine(" INNER JOIN Imprese ON Fabbricati.PIVA = Imprese.Piva ")
            End If
            If JoinCentri Then
                StrSQL.AppendLine(" INNER JOIN Centri_Aziendali ON Fabbricati.Piva = Centri_aziendali.Piva AND Fabbricati.sa_Cod = Centri_aziendali.sa_cod ")
            End If
            If Filtro_visibilita_utente Then
                StrSQL.AppendLine(" INNER JOIN utenti_Visibilita_Appoggio p (NOLOCK) ON Fabbricati.Piva = p.piva AND p.Sa_Cod = Fabbricati.Sa_Cod  AND p.entita_Cod = 2 AND p.Appezza = 0 AND p.Id_Reg = 0 AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
            End If


            StrSQL.AppendLine(" LEFT JOIN Regolamenti ON Fabbricati.Regolamento_Cod = Regolamenti.Reg_Cod")

            StrSQL.AppendLine("WHERE Fabbricati.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND Fabbricati.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND Fabbricati_Tipi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND Fabbricati_Tipi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.AppendLine(" AND Fabbricati.Tipo_Fabbricato_Cod IN (170, 171, 172, 173, 174, 175, 176, 177, 178, 179) ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   (Fabbricati.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   (Fabbricati.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Fabbricati.Piva asc, Fabbricati.Sa_Cod ASC, Fabbricati.Fabbricato_Cod asc")
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


    '###############################################################################
    Public Function Esiste_Fabbricato_NelCentro(ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByRef Fabbricato_Cod As Integer,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As Boolean

        Dim dt As DataTable
        Dim esiste As Boolean = False

        Fabbricato_Cod = 0

        dt = Leggi(Piva, Sa_Cod, 0,
                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                   "", "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
            Fabbricato_Cod = dt.Rows(0).Item("Fabbricato_Cod")
            esiste = True
        End If

        Return esiste


    End Function

    '###############################################################################
    Public Sub Ricava_PrimoMagazzino_Impresa(ByVal Piva As String,
                                             ByRef Sa_Cod As Integer,
                                             ByRef Fabbricato_Cod As Integer,
                                             ByRef objParametri As AgronicaCoreParametri)

        Dim dt As DataTable

        Sa_Cod = 0
        Fabbricato_Cod = 0

        dt = Leggi(Piva, 0, 0,
                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                   "", "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
            Sa_Cod = dt.Rows(0).Item("Sa_Cod")
            Fabbricato_Cod = dt.Rows(0).Item("Fabbricato_Cod")
        End If

    End Sub

    '###############################################################################
    'filtra per tipologia magazzino e imposta l'ordinamento alfabetico
    Public Sub Ricava_PrimoMagazzino_Impresa2(ByVal Piva As String,
                                              ByRef Sa_Cod As Integer,
                                              ByRef Fabbricato_Cod As Integer,
                                              ByRef objParametri As AgronicaCoreParametri)

        Dim dt As DataTable
        Dim filtro As String = " Fabbricati.Tipo_Fabbricato_Cod = " & CStr(MAGAZZINO)

        Sa_Cod = 0
        Fabbricato_Cod = 0

        dt = Leggi(Piva, 0, 0,
                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                   filtro, " Fabbricato_Des ASC ", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
            'prendo il primo (che è in ordine alfabetico)
            Sa_Cod = dt.Rows(0).Item("Sa_Cod")
            Fabbricato_Cod = dt.Rows(0).Item("Fabbricato_Cod")
        End If

    End Sub


    Public Function LeggixCostiAccessori(ByVal Piva As String,
                                         ByVal Sa_Cod As Int32,
                                         ByVal Qs_Data As Date,
                                         ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.LeggixCostiAccessori()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim i As Integer

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Fabbricato_Des, Fabbricato_Cod, Sa_Cod ")
                    StrSQL.Append(" FROM   Fabbricati ")
                    StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.Append(" AND    Tipo_Fabbricato_Cod = 20 ")
                    StrSQL.Append(" AND    Validita_Fine > " & Agro_SQL_SaveDate(Qs_Data) & " ")

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND    Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    Else
                        'leggo se ci sono filtri sui centri
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim FiltroCentri As String = ""
                        Dim DtCentriVisibili As DataTable
                        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                        If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                            For i = 0 To DtCentriVisibili.Rows.Count - 1
                                FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                            Next
                            If FiltroCentri <> "" Then
                                StrSQL.Append(" AND sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                            End If
                        End If
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   (Fabbricati.inviato >= 0) ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   (Fabbricati.inviato =-1) ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Fabbricati.Fabbricato_Des ")
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

    Public Function LeggiMagazzinixVisibilita(ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.LeggiMagazzinixVisibilita()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT f.Fabbricato_Des, f.Fabbricato_Cod, f.Sa_Cod, ca.sa_nome ")
                    StrSQL.Append(" FROM   Fabbricati f ")
                    StrSQL.Append(" INNER JOIN   Centri_Aziendali ca ")
                    StrSQL.Append(" ON f.piva=ca.piva AND f.sa_cod=ca.sa_cod ")
                    StrSQL.Append(" WHERE  f.Tipo_Fabbricato_Cod IN (20,50,120,121,122,123) ")
                    StrSQL.Append(" AND    f.Validita_Fine > " & Agro_SQL_SaveDate(Date.Now) & " ")

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   (f.inviato >= 0) ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   (f.inviato =-1) ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY ca.sa_nome, f.Fabbricato_Des ")
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


    Public Function LeggiFabbricatiConElemCodMovimentati(
                        ByVal Piva As String,
                        ByVal Sa_Cod As Int32,
                        ByVal Elem_Cod_InMagazzino As Integer,
                        ByVal xSelezioneVariabile As enumSelezioneVariabile,
                        ByVal xFiltroAggiuntivo As String,
                        ByVal xOrderBy As String,
                        ByRef objParametri As AgronicaCoreParametri
                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.LeggiFabbricatiConElemCodMovimentati()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim i As Integer

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT distinct f.Fabbricato_Des, f.Fabbricato_Cod, f.Sa_Cod ")
                    StrSQL.AppendLine(" FROM   movimenti m ")
                    StrSQL.AppendLine(" INNER JOIN movimenti_dettagli md ")
                    StrSQL.AppendLine(" ON m.id_agenda=md.Id_Agenda AND m.Id_Mov=md.Id_Mov ")
                    StrSQL.AppendLine(" INNER JOIN mov_destinazioni dest ")
                    StrSQL.AppendLine(" ON dest.id_agenda=md.Id_Agenda AND dest.Id_Mov=md.Id_Mov AND dest.Id_Mov_det=md.Id_Mov_det ")
                    StrSQL.AppendLine(" INNER JOIN fabbricati f ")
                    StrSQL.AppendLine(" ON dest.piva=f.piva AND dest.sa_cod=f.sa_cod AND dest.id_destinazione=f.fabbricato_cod ")
                    StrSQL.AppendLine(" WHERE m.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.AppendLine(" AND f.Tipo_Fabbricato_Cod = 20 ")
                    StrSQL.AppendLine(" AND m.cau_mov in ('" & CAU_CARICO & "','" & CAU_SCARICO & "') ")
                    StrSQL.AppendLine(" AND md.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod_InMagazzino) & " ")

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND f.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    Else
                        'leggo se ci sono filtri sui centri
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim FiltroCentri As String = ""
                        Dim DtCentriVisibili As DataTable
                        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                        If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                            For i = 0 To DtCentriVisibili.Rows.Count - 1
                                FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                            Next
                            If FiltroCentri <> "" Then
                                StrSQL.Append(" AND f.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                            End If
                        End If
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   (f.inviato >= 0) ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   (f.inviato =-1) ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY f.Fabbricato_Des ")
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

    Public Function Fabbricati_Leggi(ByVal Piva_SuperUser As String,
                                     ByRef MessaggioErrore As String,
                                     ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Fabbricato_Cod As Integer,
                                     ByVal FlagIndirizzo As Boolean,
                                     ByVal Flag_CodificaCliente As Boolean,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByVal xOrderBy As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.Fabbricati_Leggi()"

        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim i As Integer = 0

        Try

            StrSQL.Length = 0

            'StrSQL.Append(" SELECT  Fabbricati.* ")

            'If Flag_CodificaCliente = False Then
            '    StrSQL.Append(" , Fabbricati_Tipi.Tipo_Fabbricato_Des, ")
            'Else
            '    StrSQL.Append(" , ISNULL(CAC_Codifica_Fabbricati.Tipo_Fabbricato_Cod_Cliente,'') AS Tipo_Fabbricato_Cod_Cliente, ISNULL(CAC_Codifica_Fabbricati.Descrizione,'') AS Tipo_Fabbricato_Des, ")
            'End If

            'StrSQL.Append("ISNULL((SELECT TOP 1 ImpreseXParticelle.Validita_Inizio FROM ImpreseXParticelle ")
            'StrSQL.Append("        WHERE Fabbricati.Prov = ImpreseXParticelle.prov ")
            'StrSQL.Append("	       AND Fabbricati.com = ImpreseXParticelle.com ")
            'StrSQL.Append("	       AND Fabbricati.sezione = ImpreseXParticelle.sezione ")
            'StrSQL.Append("	       AND Fabbricati.foglio = ImpreseXParticelle.foglio ")
            'StrSQL.Append("	       AND Fabbricati.numero = ImpreseXParticelle.numero ")
            'StrSQL.Append("	       AND Fabbricati.subalterno = ImpreseXParticelle.subalterno ")
            'StrSQL.Append("	       AND Fabbricati.piva = ImpreseXParticelle.piva ")
            'StrSQL.Append("	       AND Fabbricati.sa_cod = ImpreseXParticelle.sa_cod ")
            'StrSQL.Append("	       ORDER BY ImpreseXParticelle.Validita_Inizio DESC ), '01/01/1900') AS Validita_Inizio_Particella, ")
            'StrSQL.Append("ISNULL((SELECT TOP 1 ImpreseXParticelle.Validita_Fine FROM ImpreseXParticelle ")
            'StrSQL.Append("        WHERE Fabbricati.Prov = ImpreseXParticelle.prov ")
            'StrSQL.Append("	       AND Fabbricati.com = ImpreseXParticelle.com ")
            'StrSQL.Append("	       AND Fabbricati.sezione = ImpreseXParticelle.sezione ")
            'StrSQL.Append("	       AND Fabbricati.foglio = ImpreseXParticelle.foglio ")
            'StrSQL.Append("	       AND Fabbricati.numero = ImpreseXParticelle.numero ")
            'StrSQL.Append("	       AND Fabbricati.subalterno = ImpreseXParticelle.subalterno ")
            'StrSQL.Append("	       AND Fabbricati.piva = ImpreseXParticelle.piva ")
            'StrSQL.Append("	       AND Fabbricati.sa_cod = ImpreseXParticelle.sa_cod ")
            'StrSQL.Append("	       ORDER BY ImpreseXParticelle.Validita_Fine DESC ), '31/12/2100') AS Validita_Fine_Particella ")

            'If FlagIndirizzo = True Then
            '    StrSQL.Append(" , Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, ISTAT.LOCALITA, ISTAT.COMUNI_PROV  ")
            'End If

            'StrSQL.Append(" FROM Fabbricati INNER JOIN   ")
            'StrSQL.Append(" Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")

            'If FlagIndirizzo = True Then
            '    StrSQL.Append(" LEFT OUTER JOIN Indirizzi ON Fabbricati.Indirizzo_Cod = Indirizzi.cod_indirizzo INNER JOIN  ")
            '    StrSQL.Append(" ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
            'End If

            'If Flag_CodificaCliente = True Then
            '    StrSQL.Append(" LEFT OUTER JOIN CAC_Codifica_Fabbricati ON Fabbricati.Tipo_Fabbricato_Cod = CAC_Codifica_Fabbricati.Tipo_Fabbricato_Cod_Gias ")
            'End If


            'StrSQL.Append(" WHERE 1 = 1 ")

            'If Piva <> "" Then
            '    StrSQL.Append(" AND    Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            'End If

            'If Sa_Cod <> 0 Then
            '    StrSQL.Append(" AND    (Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString) & ")   ")
            'End If

            'If Fabbricato_Cod <> 0 Then
            '    StrSQL.Append(" AND    (Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod.ToString) & ")   ")
            'End If

            'If FiltroAggiuntivo <> "" Then
            '    StrSQL.Append(FiltroAggiuntivo)
            'End If

            'If Ordinamento = "" Then
            '    StrSQL.Append(" ORDER BY Tipo_Fabbricato_Des, Fabbricato_Des  ")
            'Else
            '    StrSQL.Append(Ordinamento)
            'End If

            StrSQL.Append(" SELECT  Fabbricati.* ")

            If Not Flag_CodificaCliente Then
                StrSQL.Append(" , Fabbricati_Tipi.Tipo_Fabbricato_Des, ")
            Else
                StrSQL.Append(" , ISNULL(CAC_Codifica_Fabbricati.Tipo_Fabbricato_Cod_Cliente,'') AS Tipo_Fabbricato_Cod_Cliente, ISNULL(CAC_Codifica_Fabbricati.Descrizione,'') AS Tipo_Fabbricato_Des, ")
            End If

            StrSQL.Append("ISNULL((SELECT TOP 1 ImpreseXParticelle.Validita_Inizio FROM ImpreseXParticelle ")
            StrSQL.Append("        WHERE Fabbricati.Prov = ImpreseXParticelle.prov ")
            StrSQL.Append("	       AND Fabbricati.com = ImpreseXParticelle.com ")
            StrSQL.Append("	       AND Fabbricati.sezione = ImpreseXParticelle.sezione ")
            StrSQL.Append("	       AND Fabbricati.foglio = ImpreseXParticelle.foglio ")
            StrSQL.Append("	       AND Fabbricati.numero = ImpreseXParticelle.numero ")
            StrSQL.Append("	       AND Fabbricati.subalterno = ImpreseXParticelle.subalterno ")
            StrSQL.Append("	       AND Fabbricati.piva = ImpreseXParticelle.piva ")
            StrSQL.Append("	       AND Fabbricati.sa_cod = ImpreseXParticelle.sa_cod ")
            StrSQL.Append("	       ORDER BY ImpreseXParticelle.Validita_Inizio DESC ), '01/01/1900') AS Validita_Inizio_Particella, ")
            StrSQL.Append("ISNULL((SELECT TOP 1 ImpreseXParticelle.Validita_Fine FROM ImpreseXParticelle ")
            StrSQL.Append("        WHERE Fabbricati.Prov = ImpreseXParticelle.prov ")
            StrSQL.Append("	       AND Fabbricati.com = ImpreseXParticelle.com ")
            StrSQL.Append("	       AND Fabbricati.sezione = ImpreseXParticelle.sezione ")
            StrSQL.Append("	       AND Fabbricati.foglio = ImpreseXParticelle.foglio ")
            StrSQL.Append("	       AND Fabbricati.numero = ImpreseXParticelle.numero ")
            StrSQL.Append("	       AND Fabbricati.subalterno = ImpreseXParticelle.subalterno ")
            StrSQL.Append("	       AND Fabbricati.piva = ImpreseXParticelle.piva ")
            StrSQL.Append("	       AND Fabbricati.sa_cod = ImpreseXParticelle.sa_cod ")
            StrSQL.Append("	       ORDER BY ImpreseXParticelle.Validita_Fine DESC ), '31/12/2100') AS Validita_Fine_Particella, ")
            StrSQL.Append("ISNULL((SELECT TOP 1 Fabbricati_Codici.Val_Cod FROM Fabbricati_Codici ")
            StrSQL.Append("        WHERE    (Fabbricati_Codici.Id_Cod = " & Agro_SQL_SaveNum(enum_DatiAnagrafici_CodiciAnagrafe.Fabbricati) & ")   ")
            StrSQL.Append("        AND Fabbricati.Piva = Fabbricati_Codici.Piva AND Fabbricati.Sa_Cod = Fabbricati_Codici.Sa_Cod AND Fabbricati.Fabbricato_Cod = Fabbricati_Codici.Fabbricato_Cod),'0') ")
            StrSQL.Append("        AS Modifica ")

            If FlagIndirizzo Then
                StrSQL.Append(" , Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, ISTAT.LOCALITA, ISTAT.COMUNI_PROV  ")
            End If

            StrSQL.Append(" FROM Fabbricati INNER JOIN   ")
            StrSQL.Append(" Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")

            If FlagIndirizzo Then
                StrSQL.Append(" LEFT OUTER JOIN Indirizzi ON Fabbricati.Indirizzo_Cod = Indirizzi.cod_indirizzo INNER JOIN  ")
                StrSQL.Append(" ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
            End If

            If Flag_CodificaCliente Then
                StrSQL.Append(" LEFT OUTER JOIN CAC_Codifica_Fabbricati ON Fabbricati.Tipo_Fabbricato_Cod = CAC_Codifica_Fabbricati.Tipo_Fabbricato_Cod_Gias ")
            End If


            StrSQL.Append(" WHERE 1 = 1 ")

            If Piva <> "" Then
                StrSQL.Append(" AND    Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND    (Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString) & ")   ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.Append(" AND (Fabbricati.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ) ")
                    End If
                End If
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND    (Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod.ToString) & ")   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Fabbricati.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Fabbricati.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Tipo_Fabbricato_Des, Fabbricato_Des  ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function


    Public Function Stalle_Leggi(ByVal Piva_SuperUser As String,
                                 ByRef MessaggioErrore As String,
                                 ByVal Piva As String,
                                 ByVal Sa_Cod As Integer,
                                 ByVal Fabbricato_Cod As Integer,
                                 ByVal FlagIndirizzo As Boolean,
                                 ByVal FlagConsistenze As Boolean,
                                 ByVal FlagSpecie_Cliente As Boolean,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.Stalle_Leggi()"

        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT  Fabbricati.*, Stalla.*, ")
            StrSQL.Append("         Fabbricati_Tipi.Tipo_Fabbricato_Des, Lista_Tipi_Fabbricati.DESCR, ")
            StrSQL.Append("         Lista_Specie_Animali.SPE_DES, Lista_IndirizziProd_Animali.IPRO_DES, ")
            StrSQL.Append("         ISNULL((SELECT TOP 1 Fabbricati_Codici.Val_Cod FROM Fabbricati_Codici ")
            StrSQL.Append("                 WHERE    (Fabbricati_Codici.Id_Cod = " & Agro_SQL_SaveNum(enum_DatiAnagrafici_CodiciAnagrafe.Allevamenti) & ")   ")
            StrSQL.Append("                 AND Fabbricati.Piva = Fabbricati_Codici.Piva AND Fabbricati.Sa_Cod = Fabbricati_Codici.Sa_Cod AND Fabbricati.Fabbricato_Cod = Fabbricati_Codici.Fabbricato_Cod),'0') ")
            StrSQL.Append("                 AS Modifica ")

            If FlagIndirizzo Then
                StrSQL.Append(" , Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, ISTAT.LOCALITA, ISTAT.COMUNI_PROV  ")
            End If

            If FlagConsistenze Then
                StrSQL.Append(" , ISNULL(Stalla_Consistenze.ID_CONSISTENZE,0) AS ID_CONSISTENZE, ISNULL(Stalla_Consistenze.RAZ_COD,-1) AS RAZ_COD, ISNULL(Stalla_Consistenze.CAT_COD,-1) AS CAT_COD, ISNULL(Stalla_Consistenze.NUM_CAPI,0) AS NUM_CAPI  ")
            End If

            If FlagSpecie_Cliente Then
                StrSQL.Append(" , ISNULL(CAC_Codifica_Animali.Cod_Cliente,'') AS Cod_Cliente, ISNULL(CAC_Codifica_Animali.Descrizione,'') AS Specie_Cliente  ")
            End If

            StrSQL.Append(" FROM Stalla INNER JOIN   ")
            StrSQL.Append(" Lista_Tipi_Fabbricati ON Stalla.COD_FABB = Lista_Tipi_Fabbricati.COD_FABB INNER JOIN ")
            StrSQL.Append(" Lista_Specie_Animali ON Stalla.GEN_COD = Lista_Specie_Animali.GEN_COD AND Stalla.SPE_COD = Lista_Specie_Animali.SPE_COD INNER JOIN ")
            StrSQL.Append(" Lista_IndirizziProd_Animali ON Stalla.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD AND  ")
            StrSQL.Append(" Stalla.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD AND Stalla.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD INNER JOIN ")
            StrSQL.Append(" Fabbricati ON Stalla.PIVA = Fabbricati.PIVA AND Stalla.sa_cod = Fabbricati.SA_COD AND Stalla.STA_NUM = Fabbricati.Fabbricato_Cod INNER JOIN ")
            StrSQL.Append(" Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod ")

            If FlagIndirizzo Then
                StrSQL.Append(" LEFT OUTER JOIN Indirizzi ON Fabbricati.Indirizzo_Cod = Indirizzi.cod_indirizzo INNER JOIN  ")
                StrSQL.Append(" ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
            End If

            If FlagConsistenze Then
                StrSQL.Append(" LEFT OUTER JOIN Stalla_Consistenze ON Stalla.piva COLLATE SQL_Latin1_General_CP850_CI_AS = Stalla_Consistenze.piva  ")
                StrSQL.Append(" AND Stalla.sa_cod = Stalla_Consistenze.sa_cod  AND Stalla.sta_num = Stalla_Consistenze.sta_num ")
            End If

            If FlagSpecie_Cliente Then
                StrSQL.Append(" LEFT OUTER JOIN CAC_Codifica_Animali ON Stalla_Consistenze.Gen_Cod = CAC_Codifica_Animali.Gen_Cod_Gias   ")
                StrSQL.Append(" AND Stalla_Consistenze.spe_Cod = CAC_Codifica_Animali.spe_Cod_Gias  AND Stalla_Consistenze.ipro_Cod = CAC_Codifica_Animali.ipro_Cod_Gias ")
                StrSQL.Append(" AND Stalla_Consistenze.cat_Cod = CAC_Codifica_Animali.cat_Cod_Gias  AND Stalla_Consistenze.raz_Cod = CAC_Codifica_Animali.raz_Cod_Gias ")
            End If

            StrSQL.Append(" WHERE 1 = 1 ")

            If Piva <> "" Then
                StrSQL.Append(" AND    Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND    (Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString) & ")   ")
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND    (Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod.ToString) & ")   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Descr, Sta_des ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

    '################################################################################
    'query che era nell'online e stampe: NewCom_Fabbricati_Leggi
    'legge fabbricati, centri e fabbricati tipi
    Public Function Leggi_2(ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Fabbricato_Cod As Int32,
                            ByVal Tipo_Fabbricato_Cod As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.Leggi_2()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim i As Integer

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.Append(" SELECT     Centri_Aziendali.PIVA, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Fabbricati.Fabbricato_Cod, Fabbricati.Fabbricato_Des,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Indirizzo_Cod, Fabbricati.PROV, Fabbricati.COM, Fabbricati.SEZIONE, Fabbricati.FOGLIO, Fabbricati.NUMERO, Fabbricati.SUBALTERNO,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.MC_Convenzionale, Fabbricati.MC_Conversione, Fabbricati.MC_Biologico, Fabbricati.Regolamento_Cod, Fabbricati.TitoloPossesso,  " & vbCrLf)
            StrSQL.Append("   Fabbricati.Conversione_Inizio, Fabbricati.Conversione_Fine, Fabbricati.Idoneo_Costruzione, Fabbricati.Idoneo_SeparazAmbienti,  " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_SeparazProdotti, Fabbricati.Idoneo_CondIgieniche, Fabbricati.Idoneo_AutorizSanitaria, Fabbricati.Idoneo_HACCP,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_Planimetria, Fabbricati.Idoneo_Layout, Fabbricati.Idoneo_DiagrammiFlusso, Fabbricati.Idoneo_CDX_M004,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_SupMinCoperte, Fabbricati.Idoneo_SupMinScoperte, Fabbricati.Tipo_Fabbricato_Cod, Fabbricati_Tipi.Tipo_Fabbricato_Des, " & vbCrLf)
            StrSQL.Append(" Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, " & vbCrLf)
            StrSQL.Append(" Imprese.Rag_Soc, " & vbCrLf)
            StrSQL.Append(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, ISNULL(Lista_Province.REG, '') AS REG  " & vbCrLf)


            StrSQL.Append(" FROM    Fabbricati " & vbCrLf)
            'JOIN TIPO FABBRICATO
            StrSQL.Append(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod " & vbCrLf)
            'JOIN CENTRI AZIENDALI
            StrSQL.Append(" INNER JOIN Centri_Aziendali ON Fabbricati.PIVA = Centri_Aziendali.PIVA AND Fabbricati.SA_COD = Centri_Aziendali.sa_cod" & vbCrLf)

            StrSQL.Append(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = Fabbricati.Indirizzo_Cod " & vbCrLf)
            StrSQL.Append(" INNER JOIN Imprese ON Fabbricati.Piva = Imprese.Piva " & vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            StrSQL.Append(" LEFT OUTER JOIN Lista_Province ON Indirizzi.pro_cod_istat = Lista_Province.PROV " & vbCrLf)


            StrSQL.Append(" WHERE Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            StrSQL.Append(" AND   Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)

            If Piva <> "" Then
                StrSQL.Append(" AND    (Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     (Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")   " & vbCrLf)
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.Append(" AND (Fabbricati.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ) ")
                    End If
                End If
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND     (Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & ")   " & vbCrLf)
            End If

            If Tipo_Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND     (Fabbricati.Tipo_Fabbricato_Cod = " & Agro_SQL_SaveNum(Tipo_Fabbricato_Cod) & ")   " & vbCrLf)
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'StrSQL.append( " ORDER BY Sa_Nome, Fabbricati.Fabbricato_Des "
                StrSQL.Append(" ORDER BY Fabbricati.Sa_Cod, Fabbricati.Fabbricato_Cod ")
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


    '################################################################################
    Public Function LeggiUNIONFabbricatiImpresaPadre(ByVal Piva_1 As String,
                                                     ByVal Sa_Cod_1 As Int32,
                                                     ByVal Fabbricato_Cod_1 As Int32,
                                                     ByVal Piva_2 As String,
                                                     ByVal Sa_Cod_2 As Int32,
                                                     ByVal Fabbricato_Cod_2 As Int32,
                                                     ByVal Flag_Indirizzo As Boolean,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByVal xFiltroAggiuntivo2 As String,
                                                     ByVal xOrderBy As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.LeggiUNIONFabbricatiImpresaPadre()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * " & vbCrLf)
            StrSQL.Append(" FROM " & vbCrLf)

            StrSQL.Append(" ( " & vbCrLf)

            '/*************************************************************************************
            '/**************** MAGAZZINI DELL'IMPRESA       *************************
            '/*************************************************************************************

            StrSQL.Append(" ( " & vbCrLf)

            StrSQL.Append(" SELECT  Rag_Soc,   Centri_Aziendali.PIVA, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Fabbricati.Fabbricato_Cod, Fabbricati.Fabbricato_Des,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Indirizzo_Cod, Fabbricati.PROV, Fabbricati.COM, Fabbricati.SEZIONE, Fabbricati.FOGLIO, Fabbricati.NUMERO, Fabbricati.SUBALTERNO,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.MC_Convenzionale, Fabbricati.MC_Conversione, Fabbricati.MC_Biologico, Fabbricati.Regolamento_Cod, Fabbricati.TitoloPossesso,  " & vbCrLf)
            StrSQL.Append("   Fabbricati.Conversione_Inizio, Fabbricati.Conversione_Fine, Fabbricati.Idoneo_Costruzione, Fabbricati.Idoneo_SeparazAmbienti,  " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_SeparazProdotti, Fabbricati.Idoneo_CondIgieniche, Fabbricati.Idoneo_AutorizSanitaria, Fabbricati.Idoneo_HACCP,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_Planimetria, Fabbricati.Idoneo_Layout, Fabbricati.Idoneo_DiagrammiFlusso, Fabbricati.Idoneo_CDX_M004,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_SupMinCoperte, Fabbricati.Idoneo_SupMinScoperte, Fabbricati.Tipo_Fabbricato_Cod, Fabbricati_Tipi.Tipo_Fabbricato_Des " & vbCrLf)

            If Flag_Indirizzo Then
                StrSQL.Append(" , Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, " & vbCrLf)
                StrSQL.Append(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod  " & vbCrLf)
            End If

            StrSQL.Append(" FROM    Fabbricati " & vbCrLf)
            'JOIN TIPO FABBRICATO
            StrSQL.Append(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod " & vbCrLf)
            'JOIN CENTRI AZIENDALI
            StrSQL.Append(" INNER JOIN Centri_Aziendali ON Fabbricati.PIVA = Centri_Aziendali.PIVA AND Fabbricati.SA_COD = Centri_Aziendali.sa_cod" & vbCrLf)

            StrSQL.Append(" INNER JOIN Imprese ON Fabbricati.PIVA = Imprese.PIVA " & vbCrLf)

            If Flag_Indirizzo Then
                StrSQL.Append(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = Fabbricati.Indirizzo_Cod " & vbCrLf)
                StrSQL.Append(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            End If

            StrSQL.Append(" WHERE Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            StrSQL.Append(" AND   Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)

            If Piva_1 <> "" Then
                StrSQL.Append(" AND    (Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva_1) & "')   " & vbCrLf)
            End If

            If Sa_Cod_1 <> 0 Then
                StrSQL.Append(" AND     (Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_1) & ")   " & vbCrLf)
            End If

            If Fabbricato_Cod_1 <> 0 Then
                StrSQL.Append(" AND     (Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod_1) & ")   " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            StrSQL.Append(" ) " & vbCrLf)



            StrSQL.Append(" UNION ALL " & vbCrLf)

            '/*************************************************************************************
            '/******************    MAGAZZINI DELLE IMPRESE PADRE         *************************
            '/*************************************************************************************

            StrSQL.Append(" (" & vbCrLf)

            StrSQL.Append(" SELECT  Rag_Soc,   Centri_Aziendali.PIVA, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Fabbricati.Fabbricato_Cod, Fabbricati.Fabbricato_Des,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Indirizzo_Cod, Fabbricati.PROV, Fabbricati.COM, Fabbricati.SEZIONE, Fabbricati.FOGLIO, Fabbricati.NUMERO, Fabbricati.SUBALTERNO,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.MC_Convenzionale, Fabbricati.MC_Conversione, Fabbricati.MC_Biologico, Fabbricati.Regolamento_Cod, Fabbricati.TitoloPossesso,  " & vbCrLf)
            StrSQL.Append("   Fabbricati.Conversione_Inizio, Fabbricati.Conversione_Fine, Fabbricati.Idoneo_Costruzione, Fabbricati.Idoneo_SeparazAmbienti,  " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_SeparazProdotti, Fabbricati.Idoneo_CondIgieniche, Fabbricati.Idoneo_AutorizSanitaria, Fabbricati.Idoneo_HACCP,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_Planimetria, Fabbricati.Idoneo_Layout, Fabbricati.Idoneo_DiagrammiFlusso, Fabbricati.Idoneo_CDX_M004,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_SupMinCoperte, Fabbricati.Idoneo_SupMinScoperte, Fabbricati.Tipo_Fabbricato_Cod, Fabbricati_Tipi.Tipo_Fabbricato_Des " & vbCrLf)

            If Flag_Indirizzo Then
                StrSQL.Append(" , Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, " & vbCrLf)
                StrSQL.Append(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod  " & vbCrLf)
            End If

            StrSQL.Append(" FROM    Fabbricati " & vbCrLf)
            'JOIN TIPO FABBRICATO
            StrSQL.Append(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod " & vbCrLf)
            'JOIN CENTRI AZIENDALI
            StrSQL.Append(" INNER JOIN Centri_Aziendali ON Fabbricati.PIVA = Centri_Aziendali.PIVA AND Fabbricati.SA_COD = Centri_Aziendali.sa_cod" & vbCrLf)

            StrSQL.Append(" INNER JOIN Imprese ON Fabbricati.PIVA = Imprese.PIVA " & vbCrLf)

            If Flag_Indirizzo Then
                StrSQL.Append(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = Fabbricati.Indirizzo_Cod " & vbCrLf)
                StrSQL.Append(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            End If

            StrSQL.Append(" WHERE Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            StrSQL.Append(" AND   Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)

            If Piva_2 <> "" Then
                StrSQL.Append(" AND    (Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva_2) & "')   " & vbCrLf)
            End If

            If Sa_Cod_2 <> 0 Then
                StrSQL.Append(" AND     (Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_2) & ")   " & vbCrLf)
            End If

            If Fabbricato_Cod_2 <> 0 Then
                StrSQL.Append(" AND     (Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod_2) & ")   " & vbCrLf)
            End If

            If xFiltroAggiuntivo2 <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo2, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            StrSQL.Append(" )" & vbCrLf)

            '/*************************************************************************************

            StrSQL.Append(" ) FABBRICATI " & vbCrLf)

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'StrSQL.append( " ORDER BY Sa_Nome, Fabbricati.Fabbricato_Des "
                StrSQL.Append(" ORDER BY Fabbricati.Sa_Cod, Fabbricati.Fabbricato_Cod ")
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

    Public Function LeggiUNIONFabbricatiImpreseTerzisti(ByVal Piva_1 As String,
                                                  ByVal Sa_Cod_1 As Int32,
                                                  ByVal Fabbricato_Cod_1 As Int32,
                                                  ByVal strPiveTerzisti As String,
                                                  ByVal Flag_Indirizzo As Boolean,
                                                      ByVal xFiltroAggiuntivo As String,
                                                      ByVal xFiltroAggiuntivo2 As String,
                                                      ByVal xOrderBy As String,
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Fabbricati_R.LeggiUNIONFabbricatiImpresaPadre()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * " & vbCrLf)
            StrSQL.Append(" FROM " & vbCrLf)

            StrSQL.Append(" ( " & vbCrLf)

            '/*************************************************************************************
            '/**************** MAGAZZINI DELL'IMPRESA       *************************
            '/*************************************************************************************

            StrSQL.Append(" ( " & vbCrLf)

            StrSQL.Append(" SELECT  Rag_Soc,   Centri_Aziendali.PIVA, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Fabbricati.Fabbricato_Cod, Fabbricati.Fabbricato_Des,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Indirizzo_Cod, Fabbricati.PROV, Fabbricati.COM, Fabbricati.SEZIONE, Fabbricati.FOGLIO, Fabbricati.NUMERO, Fabbricati.SUBALTERNO,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.MC_Convenzionale, Fabbricati.MC_Conversione, Fabbricati.MC_Biologico, Fabbricati.Regolamento_Cod, Fabbricati.TitoloPossesso,  " & vbCrLf)
            StrSQL.Append("   Fabbricati.Conversione_Inizio, Fabbricati.Conversione_Fine, Fabbricati.Idoneo_Costruzione, Fabbricati.Idoneo_SeparazAmbienti,  " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_SeparazProdotti, Fabbricati.Idoneo_CondIgieniche, Fabbricati.Idoneo_AutorizSanitaria, Fabbricati.Idoneo_HACCP,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_Planimetria, Fabbricati.Idoneo_Layout, Fabbricati.Idoneo_DiagrammiFlusso, Fabbricati.Idoneo_CDX_M004,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_SupMinCoperte, Fabbricati.Idoneo_SupMinScoperte, Fabbricati.Tipo_Fabbricato_Cod, Fabbricati_Tipi.Tipo_Fabbricato_Des " & vbCrLf)

            If Flag_Indirizzo Then
                StrSQL.Append(" , Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, " & vbCrLf)
                StrSQL.Append(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod  " & vbCrLf)
            End If

            StrSQL.Append(" FROM    Fabbricati " & vbCrLf)
            'JOIN TIPO FABBRICATO
            StrSQL.Append(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod " & vbCrLf)
            'JOIN CENTRI AZIENDALI
            StrSQL.Append(" INNER JOIN Centri_Aziendali ON Fabbricati.PIVA = Centri_Aziendali.PIVA AND Fabbricati.SA_COD = Centri_Aziendali.sa_cod" & vbCrLf)

            StrSQL.Append(" INNER JOIN Imprese ON Fabbricati.PIVA = Imprese.PIVA " & vbCrLf)

            If Flag_Indirizzo Then
                StrSQL.Append(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = Fabbricati.Indirizzo_Cod " & vbCrLf)
                StrSQL.Append(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            End If

            StrSQL.Append(" WHERE Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            StrSQL.Append(" AND   Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)

            If Piva_1 <> "" Then
                StrSQL.Append(" AND    (Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva_1) & "')   " & vbCrLf)
            End If

            If Sa_Cod_1 <> 0 Then
                StrSQL.Append(" AND     (Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_1) & ")   " & vbCrLf)
            End If

            If Fabbricato_Cod_1 <> 0 Then
                StrSQL.Append(" AND     (Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod_1) & ")   " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            StrSQL.Append(" ) " & vbCrLf)



            StrSQL.Append(" UNION ALL " & vbCrLf)

            '/*************************************************************************************
            '/******************    MAGAZZINI DELLE IMPRESE TERZISTE      *************************
            '/*************************************************************************************

            StrSQL.Append(" (" & vbCrLf)

            StrSQL.Append(" SELECT  Rag_Soc,   Centri_Aziendali.PIVA, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Fabbricati.Fabbricato_Cod, Fabbricati.Fabbricato_Des,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Indirizzo_Cod, Fabbricati.PROV, Fabbricati.COM, Fabbricati.SEZIONE, Fabbricati.FOGLIO, Fabbricati.NUMERO, Fabbricati.SUBALTERNO,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.MC_Convenzionale, Fabbricati.MC_Conversione, Fabbricati.MC_Biologico, Fabbricati.Regolamento_Cod, Fabbricati.TitoloPossesso,  " & vbCrLf)
            StrSQL.Append("  Fabbricati.Conversione_Inizio, Fabbricati.Conversione_Fine, Fabbricati.Idoneo_Costruzione, Fabbricati.Idoneo_SeparazAmbienti,  " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_SeparazProdotti, Fabbricati.Idoneo_CondIgieniche, Fabbricati.Idoneo_AutorizSanitaria, Fabbricati.Idoneo_HACCP,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_Planimetria, Fabbricati.Idoneo_Layout, Fabbricati.Idoneo_DiagrammiFlusso, Fabbricati.Idoneo_CDX_M004,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Idoneo_SupMinCoperte, Fabbricati.Idoneo_SupMinScoperte, Fabbricati.Tipo_Fabbricato_Cod, Fabbricati_Tipi.Tipo_Fabbricato_Des " & vbCrLf)

            If Flag_Indirizzo Then
                StrSQL.Append(" , Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, " & vbCrLf)
                StrSQL.Append(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod  " & vbCrLf)
            End If

            StrSQL.Append(" FROM    Fabbricati " & vbCrLf)
            'JOIN TIPO FABBRICATO
            StrSQL.Append(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod " & vbCrLf)
            'JOIN CENTRI AZIENDALI
            StrSQL.Append(" INNER JOIN Centri_Aziendali ON Fabbricati.PIVA = Centri_Aziendali.PIVA AND Fabbricati.SA_COD = Centri_Aziendali.sa_cod" & vbCrLf)

            StrSQL.Append(" INNER JOIN Imprese ON Fabbricati.PIVA = Imprese.PIVA " & vbCrLf)

            If Flag_Indirizzo Then
                StrSQL.Append(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = Fabbricati.Indirizzo_Cod " & vbCrLf)
                StrSQL.Append(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            End If

            StrSQL.Append(" WHERE Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            StrSQL.Append(" AND   Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)

            If strPiveTerzisti <> "" Then
                StrSQL.Append(" AND    Fabbricati.Piva IN (" & Agro_SQL_Save_Clausola_IN(strPiveTerzisti, True) & ")   " & vbCrLf)
            End If

            'If Sa_Cod_2 <> 0 Then
            '    StrSQL.Append(" AND     (Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_2) & ")   " & vbCrLf)
            'End If

            'If Fabbricato_Cod_2 <> 0 Then
            '    StrSQL.Append(" AND     (Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod_2) & ")   " & vbCrLf)
            'End If

            If xFiltroAggiuntivo2 <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo2, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            StrSQL.Append(" )" & vbCrLf)

            '/*************************************************************************************

            StrSQL.Append(" ) FABBRICATI " & vbCrLf)

            '(22/11/2018 fede) commentato per avere i magazzini dei terzisti dopo quelli dell'azienda
            'If xOrderBy <> "" Then
            '    strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'Else
            '    'StrSQL.append( " ORDER BY Sa_Nome, Fabbricati.Fabbricato_Des "
            '    StrSQL.Append(" ORDER BY Fabbricati.Sa_Cod, Fabbricati.Fabbricato_Cod ")
            'End If

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


    '################################################################################
    Public Function Fabbricati_TipologiaUtilizzoDes_from_Fabbricati_TipologiaUtilizzoCod(
                                    ByVal TipologiaUtilizzoCod As Integer) _
                                    As String

        Dim Des As String

        Select Case TipologiaUtilizzoCod

            Case 0
                Des = "Non Definito"
            Case 1
                Des = "Fiori Recisi"
            Case 2
                Des = "Ortofrutta"
            Case Else
                Des = "Non Definito"

        End Select

        'Restituisco il risultato
        Return Des

    End Function

    '################################################################################
    Public Function Magazzini_UNION_Vasche(ByVal Piva_1 As String,
                                           ByVal Sa_Cod_1 As Int32,
                                           ByVal Fabbricato_Cod As Int32,
                                           ByVal Piva_2 As String,
                                           ByVal Sa_Cod_2 As Int32,
                                           ByVal Vas_Cod As Int32,
                                           ByVal Flag_Indirizzo As Boolean,
                                           ByVal xFiltroAggiuntivo1 As String,
                                           ByVal xFiltroAggiuntivo2 As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.Magazzini_UNION_Vasche()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * " & vbCrLf)
            StrSQL.Append(" FROM " & vbCrLf)

            StrSQL.Append(" ( " & vbCrLf)

            '/*************************************************************************************
            '/**************** MAGAZZINI        *************************
            '/*************************************************************************************

            StrSQL.Append(" ( " & vbCrLf)

            StrSQL.Append(" SELECT  Rag_Soc,   Centri_Aziendali.PIVA, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome,    " & vbCrLf)
            StrSQL.Append(" Fabbricati.Fabbricato_Cod AS Codice, Fabbricati.Fabbricato_Des AS descrizione,   " & vbCrLf)
            StrSQL.Append("  Fabbricati.Tipo_Fabbricato_Cod, Fabbricati_Tipi.Tipo_Fabbricato_Des, " & vbCrLf)
            StrSQL.Append(" 0 AS Piano_Cod, '' AS Numero_Serie, 0 AS Capacita_Nominale, 0 AS Capacita_Effettiva ")

            If Flag_Indirizzo Then
                StrSQL.Append(" , Fabbricati.Indirizzo_Cod, Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, " & vbCrLf)
                StrSQL.Append(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod  " & vbCrLf)
            End If

            StrSQL.Append(" FROM    Fabbricati " & vbCrLf)
            'JOIN TIPO FABBRICATO
            StrSQL.Append(" INNER JOIN Fabbricati_Tipi ON Fabbricati.Tipo_Fabbricato_Cod = Fabbricati_Tipi.Tipo_Fabbricato_Cod " & vbCrLf)
            'JOIN CENTRI AZIENDALI
            StrSQL.Append(" INNER JOIN Centri_Aziendali ON Fabbricati.PIVA = Centri_Aziendali.PIVA AND Fabbricati.SA_COD = Centri_Aziendali.sa_cod" & vbCrLf)

            StrSQL.Append(" INNER JOIN Imprese ON Fabbricati.PIVA = Imprese.PIVA " & vbCrLf)

            If Flag_Indirizzo Then
                StrSQL.Append(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = Fabbricati.Indirizzo_Cod " & vbCrLf)
                StrSQL.Append(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            End If

            StrSQL.Append(" WHERE Fabbricati.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            StrSQL.Append(" AND   Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)

            If Piva_1 <> "" Then
                StrSQL.Append(" AND    (Fabbricati.Piva = '" & Agro_SQL_SaveText(Piva_1) & "')   " & vbCrLf)
            End If

            If Sa_Cod_1 <> 0 Then
                StrSQL.Append(" AND     (Fabbricati.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_1) & ")   " & vbCrLf)
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND     (Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & ")   " & vbCrLf)
            End If

            If xFiltroAggiuntivo1 <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo1, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato >= 0) ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (Fabbricati.inviato =-1) ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            StrSQL.Append(" ) " & vbCrLf)



            StrSQL.Append(" UNION ALL " & vbCrLf)

            '/*************************************************************************************
            '/******************    VASCHE        *************************
            '/*************************************************************************************

            StrSQL.Append(" ( " & vbCrLf)

            StrSQL.Append(" SELECT  Rag_Soc,   Centri_Aziendali.PIVA, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome,    " & vbCrLf)
            StrSQL.Append(" Vas_Cod AS Codice, 'Vasca' + Identificativo AS descrizione,   " & vbCrLf)
            StrSQL.Append("  0 AS Tipo_Fabbricato_Cod, '' AS Tipo_Fabbricato_Des, " & vbCrLf)
            StrSQL.Append(" Piano_Cod, Numero_Serie,Capacita_Nominale,Capacita_Effettiva ")

            If Flag_Indirizzo Then
                StrSQL.Append(" , 0 AS Indirizzo_Cod, '' AS ind_des, '' AS frz_des, '' AS CAP, '' AS com_cod_istat, '' AS pro_cod_istat, '' AS stato, '' AS note, " & vbCrLf)
                StrSQL.Append(" '' AS com_des, '' AS pro_cod  " & vbCrLf)
            End If

            StrSQL.Append(" FROM  Cantina_Vasche ")
            'JOIN CENTRI AZIENDALI
            StrSQL.Append(" INNER JOIN Centri_Aziendali ON Cantina_Vasche.PIVA = Centri_Aziendali.PIVA AND Cantina_Vasche.SA_COD = Centri_Aziendali.sa_cod" & vbCrLf)
            StrSQL.Append(" INNER JOIN Imprese ON Cantina_Vasche.PIVA = Imprese.PIVA " & vbCrLf)
            StrSQL.Append(" WHERE 1=1")

            If Piva_2 <> "" Then
                StrSQL.Append(" AND Cantina_Vasche.Piva = '" & Agro_SQL_SaveText(Piva_2) & "' ")
            End If

            If Sa_Cod_2 <> 0 Then
                StrSQL.Append(" AND Cantina_Vasche.Sa_Cod = " & Agro_SQL_SaveText(Sa_Cod_2) & " ")
            End If

            If Vas_Cod <> 0 Then
                StrSQL.Append(" AND Cantina_Vasche.Vas_Cod = " & Agro_SQL_SaveText(Vas_Cod) & " ")
            End If
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo2 <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo2, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Cantina_Vasche.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Cantina_Vasche.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '------------------------------------------------------------------------

            StrSQL.Append(" )" & vbCrLf)

            '/*************************************************************************************

            StrSQL.Append(" ) MAGAZZINI_VASCHE " & vbCrLf)



            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Descrizione")
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


    Public Function Forni_Leggi(ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.Forni_Leggi()"

        Dim dt As DataTable
        Dim StrSQL As New Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.Append(" select distinct *, f1.val_cod as Combustibile, f2.val_cod as Fiamma, f3.val_cod as Cantiere, f4.val_cod as Umidificazione, f5.val_cod as Tipo from Fabbricati f")
            StrSQL.Append(" left join Fabbricati_Codici f1 on f1.PIVA=f.piva and  f1.sa_cod=f.sa_cod and  f1.Fabbricato_cod=f.Fabbricato_cod and  f1.id_cod=1270")
            StrSQL.Append(" left join Fabbricati_Codici f2 on f2.PIVA=f.piva and  f2.sa_cod=f.sa_cod and  f2.Fabbricato_cod=f.Fabbricato_cod and  f2.id_cod=1271")
            StrSQL.Append(" left join Fabbricati_Codici f3 on f3.PIVA=f.piva and  f3.sa_cod=f.sa_cod and  f3.Fabbricato_cod=f.Fabbricato_cod and  f3.id_cod=1272")
            StrSQL.Append(" left join Fabbricati_Codici f4 on f4.PIVA=f.piva and  f4.sa_cod=f.sa_cod and  f4.Fabbricato_cod=f.Fabbricato_cod and  f4.id_cod=1273")
            StrSQL.Append(" left join Fabbricati_Codici f5 on f5.PIVA=f.piva and  f5.sa_cod=f.sa_cod and  f5.Fabbricato_cod=f.Fabbricato_cod and  f5.id_cod=1274")
            StrSQL.Append(" where f.Tipo_Fabbricato_Cod=222")

            If Piva <> "" Then
                StrSQL.Append(" AND    (f.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND     (f.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")   " & vbCrLf)
            End If

            StrSQL.Append(" order by f.Fabbricato_Des ")

            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)

        Catch ex As Exception
            Dim MessaggioErrore As String = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function


    Public Function Lista_piva_Con_Forni(ByRef objParametri As AgronicaCoreParametri)

        Dim StrSQL As New Text.StringBuilder
        StrSQL.Append(" select distinct piva FROM    Fabbricati " & vbCrLf)
        StrSQL.Append(" where Fabbricati.Tipo_Fabbricato_Cod = 222 " & vbCrLf)

        Dim lista As String = ""

        Dim dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, "Lista_piva_Con_Forni")
        Dim i As Integer = 0
        lista = " '-1' "
        For i = 0 To dt.Rows.Count - 1
            lista &= " , '" & dt.Rows(i).Item("Piva") & "' "
        Next
        Return lista
    End Function

    Function Lista_Sacod_Con_Forni(ByVal piva As String, ByVal objParametri As AgronicaCoreParametri) As String

        Dim StrSQL As New Text.StringBuilder
        StrSQL.Append(" select distinct sa_cod FROM    Fabbricati " & vbCrLf)
        StrSQL.Append(" where Fabbricati.piva='" & Agro_SQL_SaveText(piva) & "' and Fabbricati.Tipo_Fabbricato_Cod = 222 " & vbCrLf)

        Dim lista As String = ""

        Dim DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, "Lista_piva_Con_Forni")
        Dim i As Integer = 0
        lista = " -1 "
        For i = 0 To DT.Rows.Count - 1
            lista &= " , " & DT.Rows(i).Item("Sa_Cod") & " "
        Next
        Return lista
    End Function

    Public Function LeggiMagazzino(ByVal Piva As String,
                                   ByVal Sa_Cod As Integer,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Integer

        Dim StrSQL As New Text.StringBuilder
        StrSQL.Append(" select * FROM    Fabbricati " & vbCrLf)
        StrSQL.Append(" where Fabbricati.piva='" & Agro_SQL_SaveText(Piva) & "' and Fabbricati.Sa_Cod=" & Sa_Cod & " and Fabbricati.Tipo_Fabbricato_Cod = 20  order by fabbricato_cod desc" & vbCrLf)

        Dim dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, "Lista_piva_Con_Forni")

        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Fabbricato_Cod")
        Else
            Return 0
        End If

    End Function

    Public Function LeggiMagazzinoFarmaci(ByVal Piva As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal CodiceBDN As String,
                                          ByVal ProprietarioCapi As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable
        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.LeggiMagazzinoFarmaci()"
        Dim dt As DataTable
        Dim StrSQL As New Text.StringBuilder

        Try
            StrSQL.AppendLine(" select * FROM  Fabbricati ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")
            If Piva <> "" Then
                StrSQL.AppendLine(" AND Fabbricati.piva='" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" and Fabbricati.Sa_Cod=" & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If CodiceBDN <> "" Then
                StrSQL.AppendLine(" AND Fabbricati.CodiceBDN = '" & Agro_SQL_SaveText(CodiceBDN) & "' ")
            End If

            If ProprietarioCapi <> "" Then
                StrSQL.AppendLine(" AND Fabbricati.ProprietarioCapi = '" & Agro_SQL_SaveText(ProprietarioCapi) & "' ")
            End If

            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)

        Catch ex As Exception
            Dim MessaggioErrore As String = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiMagazzino2_passopivasolo(ByVal Piva As String,
                                                  ByRef sa_cod_ritorno As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As Integer

        sa_cod_ritorno = 0
        Dim StrSQL As New Text.StringBuilder
        StrSQL.Append(" select * FROM    Fabbricati " & vbCrLf)
        StrSQL.Append(" where Fabbricati.piva='" & Agro_SQL_SaveText(Piva) & "' and Fabbricati.Tipo_Fabbricato_Cod = 20  order by fabbricato_cod desc" & vbCrLf)

        Dim dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, "Lista_piva_Con_Forni")

        If dt.Rows.Count > 0 Then
            sa_cod_ritorno = dt.Rows(0).Item("Sa_Cod")
            Return dt.Rows(0).Item("Fabbricato_Cod")
        Else
            Return 0
        End If

    End Function


    Public Function Stato_from_PivaSaCodFabbricatoCod(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer, ByVal Fabbricato_Cod As Integer,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.Stato_from_PivaSaCodFabbricatoCod()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim Stato_Cod As String = "IT"

        '---------------

        Try
            dt = Leggi_2(Piva, Sa_Cod, Fabbricato_Cod, 0,
                         "", "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 AndAlso Not IsDBNull(dt.Rows(0).Item("stato")) Then
                Stato_Cod = dt.Rows(0).Item("stato")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Stato_Cod

    End Function


    Public Function Leggi_Fabbricati_Uso_da_Terzi_Visibilita_Utente(ByRef objParametri_Server As AgronicaCoreParametri,
                                                                    ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                    ByVal Filtro_Visibilita_Utente As Boolean) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Fabbricati_R.Leggi_Fabbricati_Uso_da_Terzi_Visibilita_Utente()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Try

            stb.Length = 0
            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            stb.AppendLine(" SELECT DISTINCT Fabbricati.PIVA, Fabbricati.SA_COD, Fabbricati.Fabbricato_Cod, Fabbricati.Fabbricato_Des ")
            stb.AppendLine(" FROM Fabbricati ")
            stb.AppendLine(" Join Fabbricati_Codici FC on FC.Piva = Fabbricati.Piva And FC.Sa_Cod = Fabbricati.Sa_Cod And FC.Fabbricato_Cod = Fabbricati.Fabbricato_Cod And FC.id_cod = " & enum_CodiciAnagrafe.Fabbricato_Uso_da_Terzi & " ")

            If Filtro_Visibilita_Utente Then
                stb.AppendLine("   LEFT JOIN Utenti_Visibilita_Appoggio (NOLOCK) On Fabbricati.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1  ")
            End If

            stb.AppendLine(" WHERE 1=1  ")

            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & "  ")
            End If

            stb.AppendLine(" AND   Fabbricati.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & " ")
            stb.AppendLine(" AND   Fabbricati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & " ")

            stb.AppendLine(" ORDER BY dbo.Fabbricati.Fabbricato_Des ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

    ''' <summary>
    ''' Verifica se esistono movimenti di tipo trasferimento nei raggruppamenti di una determinata stalla
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Sta_Num"></param>
    ''' <returns></returns>
    Public Function MovimentiBoxDaStalla(ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Sta_Num As Integer,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_R.PresenzaMovimentiBoxDaStalla()"

        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0
            strSql.AppendLine($"SELECT COUNT(*) AS numero_movimenti
                                FROM Stalla (NOLOCK) INNER JOIN Stalla_Raggruppamenti 
                                    ON Stalla.PIVA = Stalla_Raggruppamenti.PIVA 
                                    AND Stalla.sa_cod = Stalla_Raggruppamenti.sa_cod 
                                    AND Stalla.STA_NUM = Stalla_Raggruppamenti.STA_NUM
                                INNER JOIN Mov_Destinazioni (NOLOCK)
                                    ON Stalla_Raggruppamenti.PIVA = Mov_Destinazioni.Piva 
                                    AND Stalla_Raggruppamenti.sa_cod = Mov_Destinazioni.Sa_Cod
                                WHERE Stalla.PIVA = '{Agro_SQL_SaveText(Piva)}'
                                AND Stalla.sa_cod = {Agro_SQL_SaveNum(Sa_Cod)} 
                                AND Stalla.STA_NUM = {Agro_SQL_SaveNum(Sta_Num)}
                                AND Tipo_Destinazione = {Agro_SQL_SaveNum(TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA)}")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt.Rows(0).Item("numero_movimenti")

    End Function
End Class



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Fabbricati_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Fabbricato_Cod As Integer,
                           ByVal Fabbricato_Des As String,
                           ByVal Indirizzo_Cod As Integer,
                           ByVal Tipo_Fabbricato_Cod As Integer,
                           ByVal PROV As String,
                           ByVal COM As String,
                           ByVal SEZIONE As String,
                           ByVal FOGLIO As Integer,
                           ByVal NUMERO As Int16,
                           ByVal SUBALTERNO As String,
                           ByVal MC_Convenzionale As Decimal,
                           ByVal MC_Conversione As Decimal,
                           ByVal MC_Biologico As Decimal,
                           ByVal Regolamento_Cod As Integer,
                           ByVal TitoloPossesso As Integer,
                           ByVal Idoneo_Costruzione As Int16,
                           ByVal Idoneo_SeparazAmbienti As Int16,
                           ByVal Idoneo_SeparazProdotti As Int16,
                           ByVal Idoneo_CondIgieniche As Int16,
                           ByVal Idoneo_AutorizSanitaria As Int16,
                           ByVal Idoneo_HACCP As Int16,
                           ByVal Idoneo_Planimetria As Int16,
                           ByVal Idoneo_LayOut As Int16,
                           ByVal Idoneo_DiagrammiFlusso As Int16,
                           ByVal Idoneo_CDX_M004 As Int16,
                           ByVal Idoneo_SupMinCoperte As Int16,
                           ByVal Idoneo_SupMinScoperte As Int16,
                           ByVal Conversione_Inizio As Date,
                           ByVal Conversione_Fine As Date,
                           ByVal MQ_Convenzionale As Decimal,
                           ByVal MQ_Conversione As Decimal,
                           ByVal MQ_Biologico As Decimal,
                           ByVal MQ_Convenzionale_Scoperto As Decimal,
                           ByVal MQ_Conversione_Scoperto As Decimal,
                           ByVal MQ_Biologico_Scoperto As Decimal,
                           ByVal N_Piani As Integer,
                           ByVal Sup_Piano As Decimal,
                           ByVal Num_Autorizzazione As String,
                           ByVal Data_Richiesta_Autorizzazione As Date,
                           ByVal Tipologia_Utilizzo As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal ChkVirtuale As Integer = 0,
                           Optional ByVal ProprietarioCapi As String = "",
                           Optional ByVal ChkMagazzinoFarmaci As Integer = 0,
                           Optional ByVal CodiceBDN As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
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

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Fabbricati ( ")
            StrSQL.AppendLine("                    Piva,   Sa_Cod,       Fabbricato_Cod,          Fabbricato_Des,  ")
            StrSQL.AppendLine("                    Indirizzo_Cod,        Tipo_Fabbricato_Cod,     Prov, ")
            StrSQL.AppendLine("                    Com,   Sezione,       Foglio,   Numero,        Subalterno,  ")
            StrSQL.AppendLine("                    MC_Convenzionale,     MC_Conversione,          MC_Biologico,           Regolamento_Cod,  ")
            StrSQL.AppendLine("                    Idoneo_Costruzione,   Idoneo_SeparazAmbienti,  Idoneo_SeparazProdotti,  ")
            StrSQL.AppendLine("                    Idoneo_CondIgieniche, Idoneo_AutorizSanitaria, Idoneo_HACCP,  ")
            StrSQL.AppendLine("                    Idoneo_Planimetria,   Idoneo_LayOut,           Idoneo_DiagrammiFlusso,  ")
            StrSQL.AppendLine("                    Idoneo_CDX_M004,     Idoneo_SupMinCoperte,     Idoneo_SupMinScoperte,     Conversione_Inizio,      Conversione_Fine, ")
            StrSQL.AppendLine("                    TitoloPossesso,       ")

            StrSQL.AppendLine("                    MQ_Convenzionale, ")
            StrSQL.AppendLine("                    MQ_Conversione, ")
            StrSQL.AppendLine("                    MQ_Biologico, ")
            StrSQL.AppendLine("                    MQ_Convenzionale_Scoperto, ")
            StrSQL.AppendLine("                    MQ_Conversione_Scoperto, ")
            StrSQL.AppendLine("                    MQ_Biologico_Scoperto, ")
            StrSQL.AppendLine("                    N_Piani, ")
            StrSQL.AppendLine("                    Sup_Piano, ")

            StrSQL.AppendLine("                    Num_Autorizzazione, ")
            StrSQL.AppendLine("                    Data_Richiesta_Autorizzazione, ")
            StrSQL.AppendLine("                    Tipologia_Utilizzo, ")
            StrSQL.AppendLine("                    ChkVirtuale, ")

            StrSQL.AppendLine("                    Inviato,              DataInvio, ")
            StrSQL.AppendLine("                    Data_Creazione,       Data_Modifica, ")
            StrSQL.AppendLine("                    UserName_Creazione,   UserName_Modifica, ")
            StrSQL.AppendLine("                    Validita_Inizio,      Validita_Fine, ProprietarioCapi,")
            StrSQL.AppendLine("                    ChkmagazzinoFarmaci, CodiceBDN")
            StrSQL.AppendLine("                    ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Fabbricato_Des) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Indirizzo_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Fabbricato_Cod) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(PROV) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(COM) & "' ")
            StrSQL.AppendLine("         ,'" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.AppendLine("         , " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.AppendLine("         , " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(MC_Convenzionale) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(MC_Conversione) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(MC_Biologico) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Idoneo_Costruzione) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Idoneo_SeparazAmbienti) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Idoneo_SeparazProdotti) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Idoneo_CondIgieniche) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Idoneo_AutorizSanitaria) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Idoneo_HACCP) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Idoneo_Planimetria) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Idoneo_LayOut) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Idoneo_DiagrammiFlusso) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Idoneo_CDX_M004) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Idoneo_SupMinCoperte) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Idoneo_SupMinScoperte) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Conversione_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Conversione_Fine) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(TitoloPossesso) & " ")

            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(MQ_Convenzionale) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(MQ_Conversione) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(MQ_Biologico) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(MQ_Convenzionale_Scoperto) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(MQ_Conversione_Scoperto) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(MQ_Biologico_Scoperto) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(N_Piani) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sup_Piano) & " ")

            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Num_Autorizzazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Data_Richiesta_Autorizzazione) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipologia_Utilizzo) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ChkVirtuale) & " ")

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(ProprietarioCapi) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ChkMagazzinoFarmaci) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(CodiceBDN) & "' ")
            StrSQL.AppendLine(")")

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

    '============================================================================
    Public Function Fabbricati_MarcaComeInviato(ByVal Piva As String,
                                                ByVal Sa_Cod As Int32,
                                                ByVal Fabbricato_Cod As Int32,
                                                ByVal Data_invio As DateTime,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Appezzamento_Write.Fabbricati_MarcaComeInviato()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Fabbricato_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Fabbricato_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Fabbricati SET ")
            StrSQL.Append("     inviato         =  -2 ")
            StrSQL.Append("    ,datainvio         =  " & Agro_SQL_SaveDateTime(Data_invio))

            StrSQL.Append(" WHERE   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            StrSQL.Append(" AND     Sa_Cod   = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append(" AND     Fabbricato_Cod   = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    '##############################################################################################
    Public Function Modifica(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Fabbricato_Cod As Integer,
                             ByVal Fabbricato_Des As String,
                             ByVal Indirizzo_Cod As Integer,
                             ByVal Tipo_Fabbricato_Cod As Integer,
                             ByVal PROV As String,
                             ByVal COM As String,
                             ByVal SEZIONE As String,
                             ByVal FOGLIO As Integer,
                             ByVal NUMERO As Integer,
                             ByVal SUBALTERNO As String,
                             ByVal MC_Convenzionale As Decimal,
                             ByVal MC_Conversione As Decimal,
                             ByVal MC_Biologico As Decimal,
                             ByVal Regolamento_Cod As Integer,
                             ByVal TitoloPossesso As Integer,
                             ByVal Idoneo_Costruzione As Int16,
                             ByVal Idoneo_SeparazAmbienti As Int16,
                             ByVal Idoneo_SeparazProdotti As Int16,
                             ByVal Idoneo_CondIgieniche As Int16,
                             ByVal Idoneo_AutorizSanitaria As Int16,
                             ByVal Idoneo_HACCP As Int16,
                             ByVal Idoneo_Planimetria As Int16,
                             ByVal Idoneo_LayOut As Int16,
                             ByVal Idoneo_DiagrammiFlusso As Int16,
                             ByVal Idoneo_CDX_M004 As Int16,
                             ByVal Idoneo_SupMinCoperte As Int16,
                             ByVal Idoneo_SupMinScoperte As Int16,
                             ByVal Conversione_Inizio As Date,
                             ByVal Conversione_Fine As Date,
                             ByVal MQ_Convenzionale As Decimal,
                             ByVal MQ_Conversione As Decimal,
                             ByVal MQ_Biologico As Decimal,
                             ByVal MQ_Convenzionale_Scoperto As Decimal,
                             ByVal MQ_Conversione_Scoperto As Decimal,
                             ByVal MQ_Biologico_Scoperto As Decimal,
                             ByVal N_Piani As Integer,
                             ByVal Sup_Piano As Decimal,
                             ByVal Num_Autorizzazione As String,
                             ByVal Data_Richiesta_Autorizzazione As Date,
                             ByVal Tipologia_Utilizzo As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                             Optional ByVal username_modifica As String = "",
                             Optional ByVal ChkVirtuale As Integer? = Nothing,
                             Optional ByVal ProprietarioCapi As String = "",
                             Optional ByVal ChkMagazzinoFarmaci As Integer = 0,
                             Optional ByVal CodiceBDN As String = ""
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Fabbricati_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try
            If Fabbricato_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Fabbricato_Cod obbligatorio)")
            End If
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE Fabbricati SET ")
            StrSQL.AppendLine("    Fabbricato_Des           = '" & Agro_SQL_SaveText(Fabbricato_Des) & "'")
            StrSQL.AppendLine("   ,Indirizzo_Cod            =  " & Agro_SQL_SaveNum(Indirizzo_Cod) & " ")
            StrSQL.AppendLine("   ,Tipo_Fabbricato_Cod      =  " & Agro_SQL_SaveNum(Tipo_Fabbricato_Cod) & " ")
            StrSQL.AppendLine("   ,Prov                     = '" & Agro_SQL_SaveText(PROV) & "'")
            StrSQL.AppendLine("   ,Com                      = '" & Agro_SQL_SaveText(COM) & "'")
            StrSQL.AppendLine("   ,Sezione                  = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'")
            StrSQL.AppendLine("   ,Foglio                   =  " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & " ")
            StrSQL.AppendLine("   ,Numero                   =  " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & " ")
            StrSQL.AppendLine("   ,Subalterno               = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "'")
            StrSQL.AppendLine("   ,MC_Convenzionale         =  " & Agro_SQL_SaveNum(MC_Convenzionale) & " ")
            StrSQL.AppendLine("   ,MC_Conversione           =  " & Agro_SQL_SaveNum(MC_Conversione) & " ")
            StrSQL.AppendLine("   ,MC_Biologico             =  " & Agro_SQL_SaveNum(MC_Biologico) & " ")
            StrSQL.AppendLine("   ,Regolamento_Cod          =  " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            StrSQL.AppendLine("   ,Idoneo_Costruzione       =  " & Agro_SQL_SaveNum(Idoneo_Costruzione) & " ")
            StrSQL.AppendLine("   ,Idoneo_SeparazAmbienti   =  " & Agro_SQL_SaveNum(Idoneo_SeparazAmbienti) & " ")
            StrSQL.AppendLine("   ,Idoneo_SeparazProdotti   =  " & Agro_SQL_SaveNum(Idoneo_SeparazProdotti) & " ")
            StrSQL.AppendLine("   ,Idoneo_CondIgieniche     =  " & Agro_SQL_SaveNum(Idoneo_CondIgieniche) & " ")
            StrSQL.AppendLine("   ,Idoneo_AutorizSanitaria  =  " & Agro_SQL_SaveNum(Idoneo_AutorizSanitaria) & " ")
            StrSQL.AppendLine("   ,Idoneo_HACCP             =  " & Agro_SQL_SaveNum(Idoneo_HACCP) & " ")
            StrSQL.AppendLine("   ,Idoneo_Planimetria       =  " & Agro_SQL_SaveNum(Idoneo_Planimetria) & " ")
            StrSQL.AppendLine("   ,Idoneo_LayOut            =  " & Agro_SQL_SaveNum(Idoneo_LayOut) & " ")
            StrSQL.AppendLine("   ,Idoneo_DiagrammiFlusso   =  " & Agro_SQL_SaveNum(Idoneo_DiagrammiFlusso) & " ")
            StrSQL.AppendLine("   ,Idoneo_CDX_M004          =  " & Agro_SQL_SaveNum(Idoneo_CDX_M004) & " ")
            StrSQL.AppendLine("   ,Idoneo_SupMinCoperte     =  " & Agro_SQL_SaveNum(Idoneo_SupMinCoperte) & " ")
            StrSQL.AppendLine("   ,Idoneo_SupMinScoperte    =  " & Agro_SQL_SaveNum(Idoneo_SupMinScoperte) & " ")
            StrSQL.AppendLine("   ,Conversione_Inizio       =  " & Agro_SQL_SaveDate(Conversione_Inizio))
            StrSQL.AppendLine("   ,Conversione_Fine         =  " & Agro_SQL_SaveDate(Conversione_Fine))
            StrSQL.AppendLine("   ,TitoloPossesso           =  " & Agro_SQL_SaveNum(TitoloPossesso) & " ")

            StrSQL.AppendLine("   ,MQ_Convenzionale             =  " & Agro_SQL_SaveNum(MQ_Convenzionale) & " ")
            StrSQL.AppendLine("   ,MQ_Conversione               =  " & Agro_SQL_SaveNum(MQ_Conversione) & " ")
            StrSQL.AppendLine("   ,MQ_Biologico                 =  " & Agro_SQL_SaveNum(MQ_Biologico) & " ")
            StrSQL.AppendLine("   ,MQ_Convenzionale_Scoperto    =  " & Agro_SQL_SaveNum(MQ_Convenzionale_Scoperto) & " ")
            StrSQL.AppendLine("   ,MQ_Conversione_Scoperto      =  " & Agro_SQL_SaveNum(MQ_Conversione_Scoperto) & " ")
            StrSQL.AppendLine("   ,MQ_Biologico_Scoperto        =  " & Agro_SQL_SaveNum(MQ_Biologico_Scoperto) & " ")
            StrSQL.AppendLine("   ,N_Piani                      =  " & Agro_SQL_SaveNum(N_Piani) & " ")
            StrSQL.AppendLine("   ,Sup_Piano                    =  " & Agro_SQL_SaveNum(Sup_Piano) & " ")

            StrSQL.AppendLine("   ,Num_Autorizzazione               = '" & Agro_SQL_SaveText(Num_Autorizzazione) & "'")
            StrSQL.AppendLine("   ,Data_Richiesta_Autorizzazione    =  " & Agro_SQL_SaveDate(Data_Richiesta_Autorizzazione))
            StrSQL.AppendLine("   ,Tipologia_Utilizzo               =  " & Agro_SQL_SaveNum(Tipologia_Utilizzo) & " ")
            StrSQL.AppendLine("   ,ProprietarioCapi             =  '" & Agro_SQL_SaveText(ProprietarioCapi) & "' ")
            StrSQL.AppendLine("   ,ChkMagazzinoFarmaci          =  " & Agro_SQL_SaveNum(ChkMagazzinoFarmaci) & " ")
            StrSQL.AppendLine("   ,CodiceBDN             =  '" & Agro_SQL_SaveText(CodiceBDN) & "' ")

            If Not IsNothing(ChkVirtuale) Then
                StrSQL.AppendLine("   ,ChkVirtuale =  " & Agro_SQL_SaveNum(ChkVirtuale) & " ")
            End If

            StrSQL.AppendLine("   ,Inviato                  =  0 ")
            StrSQL.AppendLine("   ,DataInvio                =  Null ")
            StrSQL.AppendLine("   ,Data_Modifica            =  " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine("   ,UserName_Modifica        = '" & Agro_SQL_SaveText(username_modifica) & "'")
            StrSQL.AppendLine("   ,Validita_Inizio          =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   ,Validita_Fine            =  " & Agro_SQL_SaveDate(Validita_Fine))

            '---------------------------------------------
            StrSQL.AppendLine(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.AppendLine(" AND     Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.AppendLine(" AND     Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")

            '----------------------------------------------------------------------
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

    Public Function Modifica2(ByVal Piva As String,
                              ByVal Sa_Cod As Integer,
                              ByVal Fabbricato_Cod As Integer,
                              ByVal Fabbricato_Des As String,
                              ByVal Validita_Inizio As Date,
                              ByVal Validita_Fine As Date,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Fabbricati_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Fabbricati SET ")
            StrSQL.Append("    Fabbricato_Des           = '" & Agro_SQL_SaveText(Fabbricato_Des) & "'")
            StrSQL.Append("   ,Validita_Inizio          =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine            =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.Append(" AND     Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append(" AND     Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")
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

    '##############################################################################################
    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Fabbricato_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Fabbricati_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Fabbricati ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND    Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                StrSQL.Append(" AND    Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Fabbricati ")
                StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND    Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                StrSQL.Append(" AND    Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")

            End If
            '---------------------------------------------

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

    '##############################################################################################
    Public Function AggiornaValiditaInizio(ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Fabbricato_Cod As Integer,
                                           ByVal Validita_Inizio As Date,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Fabbricati_W.AggiornaValiditaInizio()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Fabbricati SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND   Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
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

    '##############################################################################################
    Public Function AggiornaValiditaFine(ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Fabbricato_Cod As Integer,
                                         ByVal Validita_Fine As Date,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Fabbricati_W.AggiornaValiditaFine()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Fabbricati SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND   Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
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

    Public Function AggiornaFabbricato_Des(ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Fabbricato_Cod As Integer,
                                           ByVal Fabbricato_Des As String,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.Fabbricati_W.AggiornaFabbricato_Des()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Fabbricati SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Fabbricato_Des   =  '" & Agro_SQL_SaveText(Fabbricato_Des) & "' ")
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.Append(" AND   Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
            End If

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    Public Function CancellaParticella(ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Fabbricato_Cod As Integer,
                                           ByVal PROV As String,
                                           ByVal COM As String,
                                           ByVal SEZIONE As String,
                                           ByVal FOGLIO As Integer,
                                           ByVal NUMERO As Integer,
                                           ByVal SUBALTERNO As String,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Fabbricati_W.AggiornaFabbricato_Des()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.AppendLine(" UPDATE Fabbricati ")
            StrSQL.AppendLine(" SET Prov='000', Com='000', Sezione='0', Foglio=0, Numero=0, Subalterno='0' ")
            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Fabbricato_Cod <> 0 Then
                StrSQL.AppendLine(" AND   Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod) & " ")
            Else
                StrSQL.AppendLine(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                StrSQL.AppendLine(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                StrSQL.AppendLine(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
                StrSQL.AppendLine(" AND      FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
                StrSQL.AppendLine(" AND      Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
                StrSQL.AppendLine(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            End If

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
