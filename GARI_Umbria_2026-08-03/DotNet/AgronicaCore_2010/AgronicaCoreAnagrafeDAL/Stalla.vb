Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.ListExtensions

Public Class Stalla_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Int32,
                          ByVal Sta_Num As Int32,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal BDN_Codice_Azienda As String = ""
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Stalla_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM    Stalla ")
                    StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Sta_Num <> 0 Then
                        StrSQL.Append(" AND Sta_Num = " & Agro_SQL_SaveNum(Sta_Num) & "  ")
                    End If

                    If BDN_Codice_Azienda <> "" Then
                        StrSQL.Append(" AND BDN_Codice_Azienda = '" & Agro_SQL_SaveText(Trim(BDN_Codice_Azienda)) & "'  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Stalla.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Stalla.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY STA_DES ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM    Stalla ")
                    StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Sta_Num <> 0 Then
                        StrSQL.Append(" AND Sta_Num = " & Agro_SQL_SaveNum(Sta_Num) & "  ")
                    End If

                    If BDN_Codice_Azienda <> "" Then
                        StrSQL.Append(" AND BDN_Codice_Azienda = '" & Agro_SQL_SaveText(Trim(BDN_Codice_Azienda)) & "'  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Stalla.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Stalla.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY STA_DES ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  Imprese.rag_soc, Centri_Aziendali.sa_nome, Lista_Specie_Animali.SPE_DES, Stalla.* ")
                    StrSQL.Append(" FROM    Stalla ")
                    StrSQL.Append(" JOIN Centri_Aziendali ON Stalla.Piva = Centri_Aziendali.Piva AND Stalla.sa_cod = Centri_Aziendali.sa_cod ")
                    StrSQL.Append(" JOIN Imprese ON Stalla.Piva = Imprese.Piva ")
                    StrSQL.Append(" JOIN Lista_Specie_Animali ON Stalla.GEN_COD = Lista_Specie_Animali.GEN_COD AND Stalla.SPE_COD = Lista_Specie_Animali.SPE_COD ")
                    StrSQL.Append(" WHERE   Stalla.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Stalla.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    If Piva <> "" Then
                        StrSQL.Append(" AND Stalla.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Stalla.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Sta_Num <> 0 Then
                        StrSQL.Append(" AND Stalla.Sta_Num = " & Agro_SQL_SaveNum(Sta_Num) & "  ")
                    End If

                    If BDN_Codice_Azienda <> "" Then
                        StrSQL.Append(" AND Stalla.BDN_Codice_Azienda = '" & Agro_SQL_SaveText(Trim(BDN_Codice_Azienda)) & "'  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Stalla.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Stalla.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Stalla.STA_DES ASC ")
                    End If

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

    Public Function IndirizzoStalla(ByVal Piva As String,
                                    ByVal listaStalle As List(Of String),
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Stalla_R.IndirizzoStalla()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("select ind.*, Stalla.BDN_Codice_Azienda, Stalla.STA_DES from Stalla")
            StrSQL.AppendLine("INNER JOIN Fabbricati as fab on (fab.PIVA = Stalla.PIVA AND fab.Fabbricato_Cod = Stalla.STA_NUM AND fab.SA_COD = Stalla.sa_cod)")
            StrSQL.AppendLine("INNER JOIN Indirizzi as ind on (ind.cod_indirizzo = fab.Indirizzo_Cod)")
            StrSQL.AppendLine(String.Format("WHERE Stalla.BDN_Codice_Azienda IN {0} And Stalla.PIVA = '{1}'", listaStalle.ToQueryInExpression, Piva))

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Stalla.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Stalla.Inviato =-1 ")
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
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Stalla_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Int32,
                           ByVal STA_NUM As Int32,
                           ByVal Sta_Des As String,
                           ByVal Ausl_Cod As String,
                           ByVal Dat_Costr As Date,
                           ByVal Dat_Chiu As Date,
                           ByVal Cod_Fabb As String,
                           ByVal Gen_Cod As Int32,
                           ByVal Spe_Cod As Int32,
                           ByVal Ipro_Cod As Int32,
                           ByVal X As String,
                           ByVal Y As String,
                           ByVal BDN_Codice_Azienda As String,
                           ByVal BDN_Allev_IdFiscale As String,
                           ByVal Dat_Ult_Agg As Date,
                           ByVal Latitudine As Int32,
                           ByVal Longitudine As Int32,
                           ByVal CUAA_Proprietario As String,
                           ByVal Denominazione_Proprietario As String,
                           ByVal CUAA_Detentore As String,
                           ByVal Denominazione_Detentore As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Stalla_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO Stalla  (PIVA,   Sa_Cod,    STA_NUM, ")
            StrSQL.Append("          STA_DES, AUSL_COD, DAT_COSTR, DAT_CHIU, ")
            StrSQL.Append("          COD_FABB, GEN_COD, SPE_COD, IPRO_COD, X, Y, ")
            StrSQL.Append("          BDN_Codice_Azienda, BDN_Allev_IdFiscale, ")
            StrSQL.Append("          DAT_ULT_AGG, Latitudine, Longitudine,  ")
            StrSQL.Append("          CUAA_Proprietario, ")
            StrSQL.Append("          Denominazione_Proprietario, ")
            StrSQL.Append("          CUAA_Detentore, ")
            StrSQL.Append("          Denominazione_Detentore, ")

            StrSQL.Append("          Inviato,            DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("         ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Sta_Des)) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Ausl_Cod) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Dat_Costr) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Dat_Chiu) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Cod_Fabb)) & "' ")

            StrSQL.Append("         , " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(X)) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Y)) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(BDN_Codice_Azienda)) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(BDN_Allev_IdFiscale) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Dat_Ult_Agg) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Latitudine) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Longitudine) & "  ")

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(CUAA_Proprietario) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Denominazione_Proprietario) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(CUAA_Detentore) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Denominazione_Detentore) & "' ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            StrSQL.Append(") ")

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
    Public Function Modifica(ByVal Piva As String,
                             ByVal Sa_Cod As Int32,
                             ByVal Sta_Num As Int32,
                             ByVal Sta_Des As String,
                             ByVal Ausl_Cod As String,
                             ByVal Dat_Costr As Date,
                             ByVal Dat_Chiu As Date,
                             ByVal Cod_Fabb As String,
                             ByVal Gen_Cod As Int32,
                             ByVal Spe_Cod As Int32,
                             ByVal Ipro_Cod As Int32,
                             ByVal X As String,
                             ByVal Y As String,
                             ByVal BDN_Codice_Azienda As String,
                             ByVal BDN_Allev_IdFiscale As String,
                             ByVal Dat_Ult_Agg As Date,
                             ByVal Latitudine As Int32,
                             ByVal Longitudine As Int32,
                             ByVal CUAA_Proprietario As String,
                             ByVal Denominazione_Proprietario As String,
                             ByVal CUAA_Detentore As String,
                             ByVal Denominazione_Detentore As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Stalla_W.Modifica()"

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

            If Sta_Num = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sta_Num obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Stalla SET ")
            StrSQL.Append("    STA_DES           = '" & Agro_SQL_SaveText(Sta_Des) & "' ")
            StrSQL.Append("   ,AUSL_COD          = '" & Agro_SQL_SaveText(Ausl_Cod) & "' ")
            StrSQL.Append("   ,DAT_COSTR         =  " & Agro_SQL_SaveDate(Dat_Costr) & " ")
            StrSQL.Append("   ,DAT_CHIU          =  " & Agro_SQL_SaveDate(Dat_Chiu) & " ")
            StrSQL.Append("   ,COD_FABB          = '" & Agro_SQL_SaveText(Cod_Fabb) & "' ")
            StrSQL.Append("   ,GEN_COD           =  " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
            StrSQL.Append("   ,SPE_COD           =  " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
            StrSQL.Append("   ,IPRO_COD          =  " & Agro_SQL_SaveNum(Ipro_Cod) & "  ")
            StrSQL.Append("   ,X                 = '" & Agro_SQL_SaveText(X) & "' ")
            StrSQL.Append("   ,Y                 = '" & Agro_SQL_SaveText(Y) & "' ")
            StrSQL.Append("   ,BDN_Codice_Azienda  = '" & Agro_SQL_SaveText(BDN_Codice_Azienda) & "' ")
            StrSQL.Append("   ,BDN_Allev_IdFiscale  = '" & Agro_SQL_SaveText(BDN_Allev_IdFiscale) & "' ")
            StrSQL.Append("   ,DAT_ULT_AGG       =  " & Agro_SQL_SaveDate(Dat_Ult_Agg) & " ")
            StrSQL.Append("   ,Latitudine        =  " & Agro_SQL_SaveNum(Latitudine) & "  ")
            StrSQL.Append("   ,Longitudine       =  " & Agro_SQL_SaveNum(Longitudine) & "  ")

            StrSQL.Append("   ,CUAA_Proprietario            = '" & Agro_SQL_SaveText(CUAA_Proprietario) & "' ")
            StrSQL.Append("   ,Denominazione_Proprietario   = '" & Agro_SQL_SaveText(Denominazione_Proprietario) & "' ")
            StrSQL.Append("   ,CUAA_Detentore               = '" & Agro_SQL_SaveText(CUAA_Detentore) & "' ")
            StrSQL.Append("   ,Denominazione_Detentore      = '" & Agro_SQL_SaveText(Denominazione_Detentore) & "' ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE   PIVA        ='" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append(" AND     Sa_Cod      = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append(" AND     Sta_Num     = " & Agro_SQL_SaveNum(Sta_Num) & "  ")
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

    '============================================================================
    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Int32,
                             ByVal Sta_Num As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Stalla_W.Cancella()"

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

            If Sta_Num = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sta_Num obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Stalla ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Stalla ")
                StrSQL.Append(" WHERE  1=1 ")

            End If


            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Sta_Num <> 0 Then
                StrSQL.Append(" AND Sta_Num = " & Agro_SQL_SaveNum(Sta_Num) & "   ")
            End If


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

End Class
