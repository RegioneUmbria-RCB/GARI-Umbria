Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

' ---------------------------------------------- 
'  
'  Galassi, 24/02/2017 11.43.52: IMPORTANTE: Usare i Core di ConcimazioneBIZ e DAL; Questi verranno cancellati!!!!!!!
' ---------------------------------------------- 


''' <summary>
''' IMPORTANTE: Usare i Core di ConcimazioneBIZ e DAL; Questi verranno cancellati!!!!!!!
''' </summary>
''' <remarks></remarks>
Public Class PianoConcimazione_FosforoPotassio_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal PC_Testata_Cod As Integer, _
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_FosforoPotassio_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * " & vbCrLf)
                    StrSQL.Append(" FROM  PianoConcimazione_FosforoPotassio " & vbCrLf)
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)
                    StrSQL.Append(" AND     PC_SuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & " " & vbCrLf)

                    If PC_Testata_Cod <> 0 Then
                        StrSQL.Append(" AND PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " " & vbCrLf)
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 " & vbCrLf)
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 " & vbCrLf)
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)" & vbCrLf)
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta


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

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

' ---------------------------------------------- 
'  
'  Galassi, 24/02/2017 11.43.52: IMPORTANTE: Usare i Core di ConcimazioneBIZ e DAL; Questi verranno cancellati!!!!!!!
' ---------------------------------------------- 


''' <summary>
''' IMPORTANTE: Usare i Core di ConcimazioneBIZ e DAL; Questi verranno cancellati!!!!!!!
''' </summary>
''' <remarks></remarks>
Public Class PianoConcimazione_FosforoPotassio_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Scrivi(ByVal PC_Testata_Cod As Integer,
                            ByVal PC_PK_Appezzamento As String,
                            ByVal PC_PK_Coltura As String,
                            ByVal PC_PK_Resa As Decimal,
                            ByVal PC_PK_Duratadelciclo As Integer,
                            ByVal PC_PK_Coefficientetempo As Decimal,
                            ByVal PC_PK_Fase As String,
                            ByVal PC_PK_Coeffcorrettivoasportazk As Decimal,
                            ByVal PC_PK_Valutazioneasportazioni As String,
                            ByVal PC_PK_Asportazioniunitariep2o5 As Decimal,
                            ByVal PC_PK_Asportazioniunitariek2o As Decimal,
                            ByVal PC_PK_Dotazioneinizialep2o5 As Decimal,
                            ByVal PC_PK_Dotazioneinizialetestop2o5 As String,
                            ByVal PC_PK_Dotazioneinizialek2o As Decimal,
                            ByVal PC_PK_Dotazioneinizialetestok2o As String,
                            ByVal PC_PK_Arricchimentop2o5 As Decimal,
                            ByVal PC_PK_Arricchimentok2o As Decimal,
                            ByVal PC_PK_asportazionicolturalip2o5 As Decimal,
                            ByVal PC_PK_asportazionicolturalik2o As Decimal,
                            ByVal PC_PK_Quotadibasep2o5 As Decimal,
                            ByVal PC_PK_Quotadibasek2o As Decimal,
                            ByVal PC_PK_lisciviazionipotassiop2o5 As Decimal,
                            ByVal PC_PK_lisciviazionipotassiok2o As Decimal,
                            ByVal PC_PK_asportazioniannifuturianticipatep2o5 As Decimal,
                            ByVal PC_PK_asportazioniannifuturianticipatek2o As Decimal,
                            ByVal PC_PK_daprecedentiapportiorganicip2o5 As Decimal,
                            ByVal PC_PK_daprecedentiapportiorganicipk2o As Decimal,
                            ByVal PC_PK_daprecedentiapportiorganicip2o5tempo As Decimal,
                            ByVal PC_PK_daprecedentiapportiorganicipk2otempo As Decimal,
                            ByVal PC_PK_Fabbisognop2o5 As Decimal,
                            ByVal PC_PK_Fabbisognok2o As Decimal,
                            ByVal PC_PK_Limitemassimop2o5 As Decimal,
                            ByVal PC_PK_Limitemassimok2o As Decimal,
                            ByVal PC_PK_Quantitamassimap2o5 As Decimal,
                            ByVal PC_PK_Quantitamassimak2o As Decimal,
                            ByVal PC_PK_Kgchemodificanoiltenorep2o5 As Decimal,
                            ByVal PC_PK_Kgchemodificanoiltenorek2o As Decimal,
                            ByVal PC_PK_Dotazfinalep2o5 As Decimal,
                            ByVal PC_PK_Dotazfinalek2o As Decimal,
                            ByVal PC_PK_Distribuzionep2o5 As String,
                            ByVal PC_PK_Distribuzionek2o As String,
                            ByVal PC_PK_Notep2o5 As String,
                            ByVal PC_PK_Note1p2o5 As String,
                            ByVal PC_PK_Piva As String,
                            ByVal PC_PK_Ragsoc As String,
                            ByVal PC_PK_AreaVulnerabile As String,
                            ByVal PC_PK_Anno As Integer,
                            ByVal PC_PK_Areaomogenea As String,
                            ByVal PC_PK_Precessione As String,
                            ByVal PC_PK_ColturaPrincipale As String,
                            ByVal PC_PK_Finalita As String,
                            ByVal PC_PK_Ciclo As String,
                            ByVal PC_PK_Condizionidelterreno As String,
                            ByVal PC_PK_Argilla As Decimal,
                            ByVal PC_PK_P2o5 As Decimal,
                            ByVal PC_PK_K2o As Decimal,
                            ByVal PC_PK_Caco3 As Decimal,
                            ByVal PC_PK_N As Decimal,
                            ByVal PC_PK_Tessitura As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_FosforoPotassio_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO PianoConcimazione_FosforoPotassio " & vbCrLf)

            StrSQL.Append(" (PC_SuperUser, PC_Testata_Cod, PC_PK_Appezzamento, PC_PK_Coltura, PC_PK_Resa, PC_PK_Duratadelciclo, PC_PK_Coefficientetempo, " & vbCrLf)
            StrSQL.Append(" PC_PK_Fase, PC_PK_Coeffcorrettivoasportazk, PC_PK_Valutazioneasportazioni, PC_PK_Asportazioniunitariep2o5, PC_PK_Asportazioniunitariek2o, " & vbCrLf)
            StrSQL.Append(" PC_PK_Dotazioneinizialep2o5, PC_PK_Dotazioneinizialetestop2o5, PC_PK_Dotazioneinizialek2o, PC_PK_Dotazioneinizialetestok2o, " & vbCrLf)
            StrSQL.Append(" PC_PK_Arricchimentop2o5, PC_PK_Arricchimentok2o, PC_PK_asportazionicolturalip2o5, PC_PK_asportazionicolturalik2o, PC_PK_Quotadibasep2o5, " & vbCrLf)
            StrSQL.Append(" PC_PK_Quotadibasek2o, PC_PK_lisciviazionipotassiop2o5, PC_PK_lisciviazionipotassiok2o, PC_PK_asportazioniannifuturianticipatep2o5, " & vbCrLf)
            StrSQL.Append(" PC_PK_asportazioniannifuturianticipatek2o, PC_PK_daprecedentiapportiorganicip2o5, PC_PK_daprecedentiapportiorganicipk2o, " & vbCrLf)
            StrSQL.Append(" PC_PK_daprecedentiapportiorganicip2o5tempo, PC_PK_daprecedentiapportiorganicipk2otempo, PC_PK_Fabbisognop2o5, PC_PK_Fabbisognok2o, " & vbCrLf)
            StrSQL.Append(" PC_PK_Limitemassimop2o5, PC_PK_Limitemassimok2o, PC_PK_Quantitamassimap2o5, PC_PK_Quantitamassimak2o, " & vbCrLf)
            StrSQL.Append(" PC_PK_Kgchemodificanoiltenorep2o5, PC_PK_Kgchemodificanoiltenorek2o, PC_PK_Dotazfinalep2o5, PC_PK_Dotazfinalek2o, " & vbCrLf)
            StrSQL.Append(" PC_PK_Distribuzionep2o5, PC_PK_Distribuzionek2o, PC_PK_Notep2o5, PC_PK_Note1p2o5, PC_PK_Piva, PC_PK_Ragsoc, PC_PK_AreaVulnerabile, " & vbCrLf)
            StrSQL.Append(" PC_PK_Anno, PC_PK_Areaomogenea, PC_PK_Precessione, PC_PK_ColturaPrincipale, PC_PK_Finalita, PC_PK_Ciclo, PC_PK_Condizionidelterreno, " & vbCrLf)
            StrSQL.Append(" PC_PK_Argilla, PC_PK_P2o5, PC_PK_K2o, PC_PK_Caco3, PC_PK_N, PC_PK_Tessitura,  " & vbCrLf)

            StrSQL.Append("              DATA_AGG,  Inviato,            datainvio, " & vbCrLf)
            StrSQL.Append("              Data_Creazione,     Data_Modifica, " & vbCrLf)
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, " & vbCrLf)
            StrSQL.Append("              Validita_Inizio,    Validita_Fine  , DataLock " & vbCrLf)
            StrSQL.Append("              ) " & vbCrLf)

            StrSQL.Append(" VALUES (" & vbCrLf)

            StrSQL.Append(" '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "', " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_Testata_Cod)) & ", " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Appezzamento)) & "', " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Coltura)) & "', " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Resa)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Duratadelciclo)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Coefficientetempo)) & ", " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Fase)) & "', " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Coeffcorrettivoasportazk)) & ", " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Valutazioneasportazioni)) & "', " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Asportazioniunitariep2o5)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Asportazioniunitariek2o)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Dotazioneinizialep2o5)) & ", " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Dotazioneinizialetestop2o5)) & "', " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Dotazioneinizialek2o)) & ", " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Dotazioneinizialetestok2o)) & "', " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Arricchimentop2o5)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Arricchimentok2o)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_asportazionicolturalip2o5)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_asportazionicolturalik2o)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Quotadibasep2o5)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Quotadibasek2o)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_lisciviazionipotassiop2o5)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_lisciviazionipotassiok2o)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_asportazioniannifuturianticipatep2o5)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_asportazioniannifuturianticipatek2o)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_daprecedentiapportiorganicip2o5)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_daprecedentiapportiorganicipk2o)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_daprecedentiapportiorganicip2o5tempo)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_daprecedentiapportiorganicipk2otempo)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Fabbisognop2o5)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Fabbisognok2o)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Limitemassimop2o5)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Limitemassimok2o)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Quantitamassimap2o5)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Quantitamassimak2o)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Kgchemodificanoiltenorep2o5)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Kgchemodificanoiltenorek2o)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Dotazfinalep2o5)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Dotazfinalek2o)) & ", " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Distribuzionep2o5)) & "', " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Distribuzionek2o)) & "', " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Notep2o5)) & "', " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Note1p2o5)) & "', " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Piva)) & "', " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Ragsoc)) & "', " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_AreaVulnerabile)) & "', " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Anno)) & ", " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Areaomogenea)) & "', " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Precessione)) & "', " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_ColturaPrincipale)) & "', " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Finalita)) & "', " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Ciclo)) & "', " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Condizionidelterreno)) & "', " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Argilla)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_P2o5)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_K2o)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_Caco3)) & ", " & vbCrLf)
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(PC_PK_N)) & ", " & vbCrLf)
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(PC_PK_Tessitura)) & "' " & vbCrLf)

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  " & vbCrLf)
            StrSQL.Append("         , 0  " & vbCrLf)
            StrSQL.Append("         , Null  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  " & vbCrLf)
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' " & vbCrLf)
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  " & vbCrLf)
            StrSQL.Append("         , 0  ")
            StrSQL.Append(") ")

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
    Public Function Cancella(ByVal PC_Testata_Cod As Integer,
                               ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_FosforoPotassio_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PC_Testata_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (PC_Testata_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE PianoConcimazione_FosforoPotassio ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   PC_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")
                StrSQL.Append(" AND     Inviato >= 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    PianoConcimazione_FosforoPotassio ")
                StrSQL.Append(" WHERE   PC_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")

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



End Class
