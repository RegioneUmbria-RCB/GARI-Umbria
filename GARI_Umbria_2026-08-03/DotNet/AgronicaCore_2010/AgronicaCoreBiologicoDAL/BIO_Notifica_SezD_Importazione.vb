Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class BIO_Notifica_SezD_Importazione_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Notifica_ID As Integer,
                          ByVal CentroRicevimento_Piva As String,
                          ByVal CentroRicevimento_SaCod As Int32,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezD_Importazione_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT BIO_Notifica_SezD_Importazione.* ")
            StrSQL.Append(" FROM  BIO_Notifica_SezD_Importazione ")
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Notifica_ID <> 0 Then
                StrSQL.Append(" AND BIO_Notifica_SezD_Importazione.Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")
            End If

            If CentroRicevimento_Piva <> "" Then
                StrSQL.Append(" AND CentroRicevimento_Piva = '" & Agro_SQL_SaveText(CentroRicevimento_Piva) & "'  ")
            End If

            If CentroRicevimento_SaCod <> 0 Then
                StrSQL.Append(" AND CentroRicevimento_SaCod = " & Agro_SQL_SaveNum(CentroRicevimento_SaCod) & "  ")
            End If
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   BIO_Notifica_SezD_Importazione.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   BIO_Notifica_SezD_Importazione.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'StrSQL.Append(" ORDER BY Desc_CorpoEstraneo")
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

End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class BIO_Notifica_SezD_Importazione_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    'Data_Creazione e username_creazione sono passati come parametri per tenere traccia 
    'dei dati originali di creazione
    '(la modifica avviene con cancella e scrivi)
    Public Function Scrivi(ByVal Notifica_ID As Integer,
                            ByVal NumeroOrdine As Integer,
                            ByVal CentroRicevimento_Piva As String,
                            ByVal CentroRicevimento_SaCod As Integer,
                            ByVal CentroRicevimento_RagSoc As String,
                            ByVal CentroRicevimento_Via As String,
                            ByVal CentroRicevimento_Numero As String,
                            ByVal CentroRicevimento_CAP As String,
                            ByVal CentroRicevimento_Sigla_Provincia As String,
                            ByVal CentroRicevimento_CodIstat_Provincia As String,
                            ByVal CentroRicevimento_Comune As String,
                            ByVal CentroRicevimento_CodIstat_Comune As String,
                            ByVal CentroRicevimento_Telefono As String,
                            ByVal CentroRicevimento_Fax As String,
                            ByVal CentroRicevimento_Email As String,
                            ByVal CentroRicevimento_Flag_Proprieta_Terzi As Integer,
                            ByVal TPI_PRODOTTIVEGETALI As Integer,
                            ByVal TPI_ProdottiVegetali_SemiLavorati As Integer,
                            ByVal TPI_ProdottiVegetali_Preparati As Integer,
                            ByVal TPI_PRODOTTIANIMALI As Integer,
                            ByVal TPI_ProdottiAnimali_SemiLavorati As Integer,
                            ByVal TPI_ProdottiAnimali_Preparati As Integer,
                            ByVal TPI_MezziTecnici As Integer,
                            ByVal TPI_MatRiprodVeget As Integer,
                            ByVal TPI_Altro As Integer,
                            ByVal TPI_AltroDes As String,
                            ByVal TSR_SILI As Integer,
                            ByVal TSR_Sili_StoccaggioGranaglie As Integer,
                            ByVal TSR_Sili_StoccaggioColtIndustriali As Integer,
                            ByVal TSR_CELLEFRIGORIFERE As Integer,
                            ByVal TSR_Celle_ProduzioniVegetali As Integer,
                            ByVal TSR_Celle_ProduzioniZootecniche As Integer,
                            ByVal TSR_IMPIANTIPREPARAZIONIALIM As Integer,
                            ByVal TSR_ImpiantiPreparazioniAlim_Importatore As Integer,
                            ByVal TSR_ImpiantiPreparazioniAlim_Esterni As Integer,
                            ByVal TSR_ImpiantiPreparazioniAlim_Altro As Integer,
                            ByVal TSR_ImpiantiPreparazioniAlim_AltroDes1 As String,
                            ByVal TSR_ImpiantiPreparazioniAlim_AltroDes2 As String,
                            ByVal Username_Creazione As String,
                               ByVal Data_Creazione As Date,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreParametri _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_modifica As String = ""
                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezD_Importazione_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO BIO_Notifica_SezD_Importazione " & vbCrLf)

            StrSQL.Append("             (Notifica_SuperUser, " & vbCrLf)
            StrSQL.Append("             Notifica_ID, " & vbCrLf)

            StrSQL.Append("              CentroRicevimento_Piva, CentroRicevimento_SaCod, CentroRicevimento_RagSoc, CentroRicevimento_Via, CentroRicevimento_Numero, " & vbCrLf)
            StrSQL.Append("              CentroRicevimento_CAP, CentroRicevimento_Sigla_Provincia, CentroRicevimento_CodIstat_Provincia, CentroRicevimento_Comune, CentroRicevimento_CodIstat_Comune, CentroRicevimento_Telefono, " & vbCrLf)
            StrSQL.Append("              CentroRicevimento_Fax, CentroRicevimento_Email, CentroRicevimento_Flag_Proprieta_Terzi, TPI_PRODOTTIVEGETALI, " & vbCrLf)
            StrSQL.Append("              TPI_ProdottiVegetali_SemiLavorati, TPI_ProdottiVegetali_Preparati, TPI_PRODOTTIANIMALI, TPI_ProdottiAnimali_SemiLavorati, " & vbCrLf)
            StrSQL.Append("              TPI_ProdottiAnimali_Preparati, TPI_MezziTecnici, TPI_MatRiprodVeget, TPI_Altro, TPI_AltroDes, TSR_SILI, TSR_Sili_StoccaggioGranaglie, " & vbCrLf)
            StrSQL.Append("              TSR_Sili_StoccaggioColtIndustriali, TSR_CELLEFRIGORIFERE, TSR_Celle_ProduzioniVegetali, TSR_Celle_ProduzioniZootecniche, " & vbCrLf)
            StrSQL.Append("              TSR_IMPIANTIPREPARAZIONIALIM, TSR_ImpiantiPreparazioniAlim_Importatore, TSR_ImpiantiPreparazioniAlim_Esterni, " & vbCrLf)
            StrSQL.Append("              TSR_ImpiantiPreparazioniAlim_Altro, TSR_ImpiantiPreparazioniAlim_AltroDes1, TSR_ImpiantiPreparazioniAlim_AltroDes2, " & vbCrLf)

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine  , DataLock ")
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(Notifica_ID)) & "" & vbCrLf)

            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroRicevimento_Piva)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(CentroRicevimento_SaCod)) & " " & vbCrLf)
            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroRicevimento_RagSoc)) & "' " & vbCrLf)
            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroRicevimento_Via)) & "' " & vbCrLf)
            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroRicevimento_Numero)) & "' " & vbCrLf)
            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroRicevimento_CAP)) & "' " & vbCrLf)
            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroRicevimento_Sigla_Provincia)) & "' " & vbCrLf)
            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroRicevimento_CodIstat_Provincia)) & "' " & vbCrLf)
            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroRicevimento_Comune)) & "' " & vbCrLf)
            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroRicevimento_CodIstat_Comune)) & "' " & vbCrLf)
            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroRicevimento_Telefono)) & "' " & vbCrLf)
            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroRicevimento_Fax)) & "' " & vbCrLf)
            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(CentroRicevimento_Email)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(CentroRicevimento_Flag_Proprieta_Terzi)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TPI_PRODOTTIVEGETALI)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TPI_ProdottiVegetali_SemiLavorati)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TPI_ProdottiVegetali_Preparati)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TPI_PRODOTTIANIMALI)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TPI_ProdottiAnimali_SemiLavorati)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TPI_ProdottiAnimali_Preparati)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TPI_MezziTecnici)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TPI_MatRiprodVeget)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TPI_Altro)) & " " & vbCrLf)
            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(TPI_AltroDes)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TSR_SILI)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TSR_Sili_StoccaggioGranaglie)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TSR_Sili_StoccaggioColtIndustriali)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TSR_CELLEFRIGORIFERE)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TSR_Celle_ProduzioniVegetali)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TSR_Celle_ProduzioniZootecniche)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TSR_IMPIANTIPREPARAZIONIALIM)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TSR_ImpiantiPreparazioniAlim_Importatore)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TSR_ImpiantiPreparazioniAlim_Esterni)) & " " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(TSR_ImpiantiPreparazioniAlim_Altro)) & " " & vbCrLf)
            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(TSR_ImpiantiPreparazioniAlim_AltroDes1)) & "' " & vbCrLf)
            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(TSR_ImpiantiPreparazioniAlim_AltroDes2)) & "' " & vbCrLf)

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_Creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(Username_Creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")


            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("         , 0  ")
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


    '##############################################################################################
    Public Function Cancella(ByVal Notifica_ID As Int32,
                              ByVal CentroRicevimento_Piva As String,
                              ByVal CentroRicevimento_SaCod As Int32,
                              ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As Boolean

        Const nomeRoutine = "AgronicaCoreBiologicoDAL.BIO_Notifica_SezD_Importazione_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Notifica_ID = 0 Then
                Throw New Exception("Parametro non corretto nella query (Notifica_ID obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE BIO_Notifica_SezD_Importazione ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & " ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM BIO_Notifica_SezD_Importazione ")
                StrSQL.Append(" WHERE Notifica_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Notifica_ID = " & Agro_SQL_SaveNum(Notifica_ID) & "  ")
            End If
            '---------------------------------------------

            If CentroRicevimento_Piva <> "" Then
                StrSQL.Append(" AND CentroRicevimento_Piva = '" & Agro_SQL_SaveText(CentroRicevimento_Piva) & "'  ")
            End If

            If CentroRicevimento_SaCod <> 0 Then
                StrSQL.Append(" AND CentroRicevimento_SaCod = " & Agro_SQL_SaveNum(CentroRicevimento_SaCod) & "  ")
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
